using System;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// El tipo existe pero no sirve como provider.
    /// <para>
    /// ADR-006 §3.3, cuarta fila: el diagnóstico lleva el <b>motivo</b>, que es uno de los
    /// tres del contrato: no implementa <see cref="ILazyProvider"/>, es abstracto o
    /// interfaz, o no tiene constructor público sin parámetros. La verificación ocurre
    /// <b>antes</b> de construir.
    /// </para>
    /// </summary>
    public sealed class ProviderTypeIncompatibleException : CoreException
    {
        /// <summary>Motivo: el tipo no implementa el contrato del provider.</summary>
        public const string DoesNotImplementContract = "no implementa ILazyProvider";

        /// <summary>Motivo: el tipo es abstracto o es una interfaz (no se puede construir).</summary>
        public const string IsAbstractOrInterface = "es abstracto o interfaz: no se puede construir";

        /// <summary>Motivo: el tipo no expone un constructor público sin parámetros.</summary>
        public const string HasNoPublicParameterlessConstructor = "no tiene constructor público sin parámetros";

        /// <summary>
        /// 
        /// </summary>
        public ProviderTypeIncompatibleException()
            : base("El tipo del provider no es compatible con el contrato.")
        {
            TypeName = string.Empty;
            Reason = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ProviderTypeIncompatibleException(string message)
            : base(message)
        {
            TypeName = string.Empty;
            Reason = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProviderTypeIncompatibleException(string message, Exception innerException)
            : base(message, innerException)
        {
            TypeName = string.Empty;
            Reason = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">Nombre completo del tipo declarado.</param>
        /// <param name="reason">Motivo de la incompatibilidad (ver las constantes).</param>
        public ProviderTypeIncompatibleException(string typeName, string reason)
            : base($"El tipo '{typeName}' no es compatible como provider: {reason}.")
        {
            TypeName = typeName;
            Reason = reason;
        }

        /// <summary>Nombre completo del tipo declarado.</summary>
        public string TypeName { get; }

        /// <summary>Motivo de la incompatibilidad.</summary>
        public string Reason { get; }
    }
}
