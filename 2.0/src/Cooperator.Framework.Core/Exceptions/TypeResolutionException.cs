using System;

namespace Cooperator.Framework.Core.Exceptions
{
    /// <summary>
    /// La <b>identificación de un tipo</b> o de un <b>miembro</b> del camino de mapeo no resolvió: el valor
    /// intentado no existe, vive en otro ensamblado y se nombró de forma simple, o la propiedad buscada no
    /// corresponde al tipo.
    /// <para>
    /// <b>Diagnóstico accionable (ADR-006 §3.3, SPEC-261016-0034 §6/B.3).</b> El error nombra el <b>valor
    /// intentado</b>, el <b>tipo</b> involucrado —cuando se conoce— y el <b>punto que lo pidió</b>: reemplaza al
    /// silencio de un <c>null</c> que explota más tarde y al mensaje genérico que no dice quién preguntó. No lleva
    /// datos de negocio: sólo nombres de tipo, de miembro y del punto.
    /// </para>
    /// <para>
    /// Nace <b>POCO</b>: sin atributo de serialización ni constructor de serialización (ADR-008 §3.4), y deriva de
    /// <see cref="CoreException"/> para que el consumidor que captura la jerarquía del núcleo la siga capturando.
    /// </para>
    /// </summary>
    public sealed class TypeResolutionException : CoreException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public TypeResolutionException()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public TypeResolutionException(string message)
            : this(message, string.Empty, string.Empty)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public TypeResolutionException(string message, Exception innerException)
            : this(message, string.Empty, string.Empty, innerException)
        {
        }

        /// <summary>Inicializa la excepción con el valor intentado y el punto que lo pidió.</summary>
        /// <param name="message">Mensaje del error, con el valor y el punto.</param>
        /// <param name="value">Valor intentado (el nombre de tipo o de miembro que no resolvió).</param>
        /// <param name="origin">Punto del camino de mapeo que pidió la identificación.</param>
        public TypeResolutionException(string message, string value, string origin)
            : base(message)
        {
            Value = value;
            Origin = origin;
        }

        /// <summary>Inicializa la excepción con el valor intentado, el punto y la causa.</summary>
        /// <param name="message">Mensaje del error, con el valor y el punto.</param>
        /// <param name="value">Valor intentado (el nombre de tipo o de miembro que no resolvió).</param>
        /// <param name="origin">Punto del camino de mapeo que pidió la identificación.</param>
        /// <param name="innerException">Causa original.</param>
        public TypeResolutionException(string message, string value, string origin, Exception innerException)
            : base(message, innerException)
        {
            Value = value;
            Origin = origin;
        }

        /// <summary>Valor intentado que no resolvió (nombre de tipo o de miembro).</summary>
        public string Value { get; }

        /// <summary>Punto del camino de mapeo que pidió la identificación.</summary>
        public string Origin { get; }

        /// <summary>Tipo involucrado en la identificación, si se conoce al construir el error.</summary>
        public string? TypeName { get; set; }
    }
}
