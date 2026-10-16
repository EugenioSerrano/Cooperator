using System;

namespace Cooperator.Framework.Data.Caching
{
    /// <summary>
    /// El <b>esquema de claves publicado</b> de la capa de datos (ADR-004 §3.1, SPEC-261016-0019 §6/A.1):
    /// <c>coop:{scope}:{rule}:{kind}</c>.
    /// <para>
    /// Vive junto al contrato —y no en un comentario interno— porque es la referencia con la que un consumidor
    /// invalida: quien invalida conoce sus claves (no hay invalidación por prefijo en 2.0.0). Las claves de 1.4
    /// (<c>Coop_DAL_*</c>, <c>Cooperator_DAL_SP_*</c>) no eran contrato público; su equivalencia funcional se
    /// verifica con la suite de caracterización.
    /// </para>
    /// </summary>
    public static class CacheKey
    {
        /// <summary>Raíz del esquema.</summary>
        public const string Scheme = "coop";

        /// <summary>Ámbito de la capa de datos.</summary>
        public const string DataScope = "data";

        /// <summary>Región/forma de clave de la colección completa de una regla.</summary>
        public const string CollectionAllKind = "collection-all";

        /// <summary>Región/forma de clave del diccionario de una regla.</summary>
        public const string CollectionDictionaryKind = "collection-dictionary";

        /// <summary>Región/forma de clave de la metadata de parámetros de un stored procedure.</summary>
        public const string ParameterMetadataKind = "metadata";

        /// <summary>
        /// Forma de clave de la metadata cuando el llamador pide el <b>parámetro de retorno</b>: la forma del valor
        /// cambia con ese flag, así que no puede compartir clave con el caso común.
        /// </summary>
        public const string ParameterMetadataWithReturnValueKind = "metadata-with-return-value";

        /// <summary>Región/forma de clave del valor del timeout de comandos.</summary>
        public const string CommandTimeoutKind = "value";

        /// <summary>Prefijo del "rule" de las claves de metadata de stored procedures.</summary>
        public const string StoredProcedureParametersRulePrefix = "sp-parameters:";

        /// <summary>El "rule" fijo de la clave del timeout de comandos.</summary>
        public const string CommandTimeoutRule = "command-timeout";

        /// <summary>
        /// Clave de la colección completa (<c>GetAll</c>) de una regla: <c>coop:data:{rule}:collection-all</c>.
        /// </summary>
        /// <param name="rule">Nombre de la regla/tabla (el <c>TableName</c> de la capa).</param>
        /// <returns>La clave.</returns>
        public static string CollectionAll(string rule) => Build(DataScope, rule, CollectionAllKind);

        /// <summary>
        /// Clave del diccionario (<c>GetAllDictionary</c>) de una regla:
        /// <c>coop:data:{rule}:collection-dictionary</c>.
        /// </summary>
        /// <param name="rule">Nombre de la regla/tabla.</param>
        /// <returns>La clave.</returns>
        public static string CollectionDictionary(string rule) => Build(DataScope, rule, CollectionDictionaryKind);

        /// <summary>
        /// Clave de la metadata de parámetros de un stored procedure:
        /// <c>coop:data:sp-parameters:{sp}:metadata</c>.
        /// </summary>
        /// <param name="storedProcedure">Nombre del stored procedure.</param>
        /// <returns>La clave.</returns>
        public static string StoredProcedureParameters(string storedProcedure) =>
            StoredProcedureParameters(storedProcedure, false);

        /// <summary>
        /// Clave de la metadata de parámetros de un stored procedure, según el llamador pida o no el parámetro de
        /// retorno: <c>coop:data:sp-parameters:{sp}:metadata</c> (caso común) o
        /// <c>coop:data:sp-parameters:{sp}:metadata-with-return-value</c>.
        /// </summary>
        /// <param name="storedProcedure">Nombre del stored procedure.</param>
        /// <param name="includeReturnValueParameter">Si el valor incluye el parámetro de retorno.</param>
        /// <returns>La clave.</returns>
        public static string StoredProcedureParameters(string storedProcedure, bool includeReturnValueParameter) =>
            Build(
                DataScope,
                StoredProcedureParametersRulePrefix + storedProcedure,
                includeReturnValueParameter ? ParameterMetadataWithReturnValueKind : ParameterMetadataKind);

        /// <summary>Clave del valor del timeout de comandos: <c>coop:data:command-timeout:value</c>.</summary>
        public static string CommandTimeout => Build(DataScope, CommandTimeoutRule, CommandTimeoutKind);

        /// <summary>Arma una clave con el esquema documentado.</summary>
        /// <param name="scope">Ámbito.</param>
        /// <param name="rule">Regla (tabla, stored procedure o punto).</param>
        /// <param name="kind">Tipo de la entrada dentro de la regla.</param>
        /// <returns>La clave del esquema.</returns>
        public static string Build(string scope, string rule, string kind)
        {
            if (string.IsNullOrEmpty(scope)) throw new ArgumentNullException(nameof(scope));
            if (string.IsNullOrEmpty(rule)) throw new ArgumentNullException(nameof(rule));
            if (string.IsNullOrEmpty(kind)) throw new ArgumentNullException(nameof(kind));

            return string.Concat(Scheme, ":", scope, ":", rule, ":", kind);
        }
    }
}
