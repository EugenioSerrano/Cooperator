using System;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Modeler.DBReverseHelper
{
    /// <summary>
    /// De dónde sale la <b>cadena de conexión</b> del reverse (SPEC-261016-0027 §6/C.1; ADR-004 §3.2/§3.3):
    /// del <b>contrato de configuración portable</b> (Bolt 011), por la clave del catálogo
    /// <c>Cooperator:ConnectionStrings:&lt;nombre&gt;</c> —nunca de un formulario, de un archivo propio ni de un
    /// valor embebido—; el valor llega <b>crudo y con su prefijo de esquema</b>, y el <b>resolutor de valores
    /// protegidos</b> lo invoca <b>este</b> consumidor, que es quien usa el valor.
    /// </summary>
    /// <remarks>
    /// El valor crudo lo interpreta <see cref="ConnectionStringValue"/> (el tipo del contrato): exige el prefijo de
    /// esquema y <b>rechaza</b> un prefijo que no tenga resolutor —jamás se lee un valor a ciegas ni se asume texto
    /// plano (AC-4, §15)—. Un host con un resolutor registrado para otro esquema puede aportarlo con la sobrecarga
    /// que lo recibe.
    /// </remarks>
    public static class ConnectionStringResolver
    {
        /// <summary>Nombre de la cadena de conexión por defecto (el de 1.4, conservado por continuidad).</summary>
        public const string DefaultName = ConfigurationKeys.ConnectionStrings.DefaultName;

        /// <summary>Resuelve la cadena de conexión por defecto desde el contrato de configuración.</summary>
        /// <returns>La cadena de conexión resuelta.</returns>
        public static string Resolve() => Resolve(DefaultName);

        /// <summary>Resuelve una cadena de conexión con el resolutor de identidad del contrato (<c>plain:</c>).</summary>
        /// <param name="name">Nombre de la cadena en el catálogo.</param>
        /// <returns>La cadena de conexión resuelta.</returns>
        public static string Resolve(string name) => Resolve(name, null);

        /// <summary>
        /// Resuelve una cadena de conexión, invocando al resolutor del esquema declarado.
        /// </summary>
        /// <param name="name">Nombre de la cadena en el catálogo.</param>
        /// <param name="resolver">
        /// El resolutor del esquema declarado, si el host tiene uno registrado; <c>null</c> resuelve el esquema de
        /// identidad y <b>falla con error tipado</b> para cualquier otro (nunca se lee a ciegas).
        /// </param>
        /// <returns>La cadena de conexión resuelta.</returns>
        /// <exception cref="CooperatorConfigurationException">
        /// La clave no está, el valor no declara prefijo de esquema, o su esquema no tiene resolutor.
        /// </exception>
        public static string Resolve(string name, IProtectedValueResolver? resolver)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            string key = ConfigurationKeys.ConnectionStrings.For(name);
            string raw = CooperatorSettings.Current.GetConnectionString(name);
            ConnectionStringValue value = ConnectionStringValue.Parse(key, raw);

            // La invocación del resolutor es de ESTE consumidor (ADR-004 §3.3): la configuración no resuelve.
            return value.Resolve(resolver!);
        }
    }
}
