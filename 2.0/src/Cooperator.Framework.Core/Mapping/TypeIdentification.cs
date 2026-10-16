using System;
using System.Reflection;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.Mapping
{
    /// <summary>
    /// La <b>identificación inequívoca</b> del camino filas→objetos (ADR-006 §3.1, SPEC-261016-0034 §6/B).
    /// <list type="bullet">
    ///   <item><b>Donde el tipo ya está disponible</b> (el <c>PropertyType</c> de una propiedad) <b>no se pasa por un
    ///         nombre</b>: se usa el tipo y, si es un <see cref="Nullable{T}"/>, su <b>tipo subyacente resuelto por
    ///         API</b> —nunca por <c>Substring</c> del <c>ToString()</c>, que dependía del formato del nombre—.</item>
    ///   <item><b>Donde un nombre llega de afuera</b>, se exige <b>calificado</b> (con su ensamblado): un nombre simple
    ///         se <b>rechaza</b>, porque <c>Type.GetType</c> con un nombre simple resuelve sólo contra el ensamblado
    ///         llamador y el núcleo —el «funciona por casualidad según el llamador» de F-47—.</item>
    ///   <item><b>Una identificación que no resuelve no es un silencio</b>: es <see cref="TypeResolutionException"/>,
    ///         que nombra el valor intentado, el tipo y el punto que lo pidió.</item>
    ///   <item>La regla de nombres es <b>una sola</b> y es la del resolutor de providers del Bolt 005 (ADR-006 §3.1):
    ///         <b>0</b> resolución implícita. Este tipo no carga ensamblados: la carga vive en el Bolt 005.</item>
    /// </list>
    /// </summary>
    public static class TypeIdentification
    {
        /// <summary>Separador entre el nombre de tipo y su ensamblado en un nombre calificado (<c>Ns.Tipo, Ensamblado</c>).</summary>
        public const char AssemblySeparator = ',';

        /// <summary>
        /// Devuelve el tipo con el que se identifica una propiedad: si es un <see cref="Nullable{T}"/>, su tipo
        /// subyacente, resuelto <b>por API</b>.
        /// </summary>
        /// <param name="propertyType">El <c>PropertyType</c> de la propiedad.</param>
        /// <param name="origin">Punto del camino que lo pide, para el diagnóstico.</param>
        /// <returns>El tipo subyacente de un <see cref="Nullable{T}"/>, o el tipo tal cual.</returns>
        /// <exception cref="TypeResolutionException">El tipo declarado no está disponible.</exception>
        public static Type UnwrapNullable(Type? propertyType, string origin)
        {
            if (propertyType == null)
            {
                throw new TypeResolutionException(
                    "El camino de mapeo pidió el tipo de una propiedad que no lo declara (punto: " + origin + ").",
                    string.Empty, origin);
            }

            return Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        }

        /// <summary>
        /// Resuelve un <b>nombre calificado</b> de tipo (con su ensamblado) y falla si no resuelve.
        /// <para>
        /// Un nombre <b>simple</b> se rechaza sin intentar resolverlo: admitirlo reintroduciría la resolución
        /// implícita que ADR-006 §3.1 elimina (AC-2).
        /// </para>
        /// </summary>
        /// <param name="qualifiedName">Nombre calificado, por ejemplo <c>Ventas.Entities.Cliente, Ventas.Entities</c>.</param>
        /// <param name="origin">Punto del camino que lo pide, para el diagnóstico.</param>
        /// <returns>El tipo resuelto.</returns>
        /// <exception cref="TypeResolutionException">El nombre está vacío, no es calificado, o no resuelve.</exception>
        public static Type ResolveQualifiedName(string? qualifiedName, string origin)
        {
            if (string.IsNullOrWhiteSpace(qualifiedName))
            {
                throw new TypeResolutionException(
                    "El camino de mapeo recibió un nombre de tipo vacío (punto: " + origin + "). Se exige un nombre" +
                    " calificado, con su ensamblado.", qualifiedName ?? string.Empty, origin);
            }

            string name = qualifiedName!.Trim();
            if (!name.Contains(AssemblySeparator))
            {
                throw new TypeResolutionException(
                    "El nombre de tipo '" + name + "' no está CALIFICADO (no declara su ensamblado) y el camino de" +
                    " mapeo no resuelve nombres simples: un nombre simple resuelve contra el ensamblado llamador y" +
                    " el núcleo, así que el resultado dependería de dónde vive el tipo (ADR-006 §3.1). Punto: " +
                    origin + ".", name, origin);
            }

            Type? resolved = Type.GetType(name, throwOnError: false);
            if (resolved == null)
            {
                throw new TypeResolutionException(
                    "El nombre de tipo calificado '" + name + "' no resolvió a ningún tipo (punto: " + origin + ").",
                    name, origin);
            }

            return resolved;
        }

        /// <summary>
        /// Busca una propiedad de un tipo y falla con diagnóstico accionable si no existe.
        /// <para>
        /// Es la verificación <b>única</b> de los dos puntos que buscan una propiedad por nombre —el comparador de
        /// orden y el llenado del <c>DataSet</c>—, de modo que no quede uno corregido y su gemelo sin corregir.
        /// </para>
        /// </summary>
        /// <param name="type">Tipo en el que se busca la propiedad.</param>
        /// <param name="propertyName">Nombre de la propiedad (o de la columna que debería corresponderle).</param>
        /// <param name="origin">Punto del camino que la pide, para el diagnóstico.</param>
        /// <returns>La propiedad.</returns>
        /// <exception cref="TypeResolutionException">El tipo no está disponible, o la propiedad no existe en él.</exception>
        public static PropertyInfo RequireProperty(Type? type, string? propertyName, string origin)
        {
            if (type == null)
            {
                throw new TypeResolutionException(
                    "El camino de mapeo no tiene el tipo en el que buscar la propiedad '" + (propertyName ?? string.Empty) +
                    "' (punto: " + origin + ").", propertyName ?? string.Empty, origin);
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new TypeResolutionException(
                    "El camino de mapeo recibió un nombre de propiedad vacío para el tipo '" + type.FullName +
                    "' (punto: " + origin + ").", propertyName ?? string.Empty, origin)
                {
                    TypeName = type.FullName,
                };
            }

            PropertyInfo? property = type.GetProperty(propertyName!.Trim());
            if (property == null)
            {
                string[] available = [.. Array.ConvertAll(type.GetProperties(), candidate => candidate.Name)];
                Array.Sort(available, StringComparer.Ordinal);

                throw new TypeResolutionException(
                    "El tipo '" + type.FullName + "' no tiene ninguna propiedad '" + propertyName + "' (punto: " +
                    origin + "). Propiedades del tipo: " + string.Join(", ", available) + ".", propertyName, origin)
                {
                    TypeName = type.FullName,
                };
            }

            return property;
        }
    }
}
