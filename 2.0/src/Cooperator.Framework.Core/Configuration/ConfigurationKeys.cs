using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// El <b>catálogo de claves</b> del framework (ADR-004 §3.2), publicado también como documento versionado
    /// (<c>2.0/docs/configuration-keys.md</c>).
    /// <para>
    /// Existe para que ni el nombre de una clave ni su default vivan escondidos en una implementación:
    /// «toda región de caché con default propio tiene su clave» (ADR-004 §3.2).
    /// </para>
    /// <para>
    /// Los <b>valores</b> de los <see cref="Defaults"/> son los decididos por ADR-004: los de 1.4 se conservan
    /// donde existían.
    /// </para>
    /// </summary>
    public static class ConfigurationKeys
    {
        /// <summary>Separador de jerarquía del catálogo.</summary>
        public const string SectionSeparator = ":";

        /// <summary>Raíz común de todas las claves del framework.</summary>
        public const string Root = "Cooperator";

        /// <summary>
        /// Convención de la fuente por defecto (variables de entorno): la jerarquía se expresa con doble
        /// guion bajo (<c>Cooperator__Data__CommandTimeoutSeconds</c>). ADR-004 §3.2.
        /// </summary>
        public const string EnvironmentSectionSeparator = "__";

        /// <summary>Claves de cadenas de conexión.</summary>
        public static class ConnectionStrings
        {
            /// <summary>Prefijo de las claves de cadena de conexión.</summary>
            public const string Prefix = Root + SectionSeparator + "ConnectionStrings" + SectionSeparator;

            /// <summary>
            /// Nombre de la cadena de conexión por defecto. Se <b>conserva</b> de 1.4 por continuidad declarada
            /// (ADR-004 §3.2).
            /// </summary>
            public const string DefaultName = "CooperatorConnectionString";

            /// <summary>Clave completa de una cadena de conexión por nombre.</summary>
            /// <param name="name">Nombre de la cadena.</param>
            /// <returns>La clave del catálogo.</returns>
            public static string For(string name)
            {
                return Prefix + name;
            }
        }

        /// <summary>Claves de la capa de datos.</summary>
        public static class Data
        {
            /// <summary>
            /// Timeout de comandos, en segundos. En 1.4 se leía de la sección de ajustes de la aplicación
            /// (procedencia documentada en <c>2.0/docs/configuration-keys.md</c>).
            /// </summary>
            public const string CommandTimeoutSeconds = Root + SectionSeparator + "Data" + SectionSeparator + "CommandTimeoutSeconds";
        }

        /// <summary>Claves de la caché (ADR-004 §3.1/§3.2).</summary>
        public static class Cache
        {
            /// <summary>TTL de la región de colecciones, en segundos.</summary>
            public const string DefaultTtlSeconds = Root + SectionSeparator + "Cache" + SectionSeparator + "DefaultTtlSeconds";

            /// <summary>TTL de la región de metadata de stored procedures, en segundos.</summary>
            public const string ParameterMetadataTtlSeconds = Root + SectionSeparator + "Cache" + SectionSeparator + "ParameterMetadataTtlSeconds";

            /// <summary>
            /// Cap de entradas de la caché en memoria (ADR-004 §3.1: «un cap de entradas <b>configurable</b> con
            /// desalojo (primero las vencidas)»). La clave y su <b>tipo</b> se publican acá para que el default no
            /// viva escondido en el código (ADR-004 §3.2, y el `AITL-SPEC-Approval` de este SPEC lo ordena como
            /// obligación del V-Bounce). <b>La cifra no la fija este archivo</b>: ningún ADR ni SPEC decide un
            /// número, y publicarlo sería rellenar el hueco con una suposición (SPEC-261016-0018 §15 #2/#8). El
            /// valor del default es del Bolt 013 —dueño de la caché—, que es quien lo pidió a este Bolt
            /// (`SPEC-261016-0019` §15) y quien lo publica en el catálogo al aportar la caché.
            /// </summary>
            public const string MaxEntries = Root + SectionSeparator + "Cache" + SectionSeparator + "MaxEntries";
        }

        /// <summary>Claves de correo.</summary>
        public static class Mail
        {
            /// <summary>
            /// Servidor SMTP. En 1.4 se leía al cargar el tipo, en un inicializador estático que reventaba si el
            /// valor faltaba (F-22; procedencia documentada en <c>2.0/docs/configuration-keys.md</c>).
            /// </summary>
            public const string SmtpServer = Root + SectionSeparator + "Mail" + SectionSeparator + "SmtpServer";
        }

        /// <summary>Claves de la carga de providers LazyLoad (ADR-004 §3.2, ADR-006 §3.1).</summary>
        public static class LazyLoad
        {
            /// <summary>Prefijo de las claves por entidad.</summary>
            public const string ProvidersPrefix = Root + SectionSeparator + "LazyLoad" + SectionSeparator + "Providers" + SectionSeparator;

            /// <summary>Sufijo de la clave del ensamblado del provider.</summary>
            public const string AssemblySuffix = SectionSeparator + "Assembly";

            /// <summary>Sufijo de la clave de la clase del provider.</summary>
            public const string ClassSuffix = SectionSeparator + "Class";

            /// <summary>Clave del ensamblado del provider de una entidad.</summary>
            /// <param name="entity">Nombre de la entidad.</param>
            /// <returns>La clave del catálogo.</returns>
            public static string AssemblyFor(string entity)
            {
                return ProvidersPrefix + entity + AssemblySuffix;
            }

            /// <summary>
            /// Clave de la clase del provider de una entidad. Su valor es el <b>nombre completo calificado por
            /// namespace</b> (por ejemplo <c>Ventas.LazyProviders.CustomerLazyProvider</c>), nunca un nombre simple:
            /// ADR-006 §3.1.
            /// </summary>
            /// <param name="entity">Nombre de la entidad.</param>
            /// <returns>La clave del catálogo.</returns>
            public static string ClassFor(string entity)
            {
                return ProvidersPrefix + entity + ClassSuffix;
            }
        }

        /// <summary>
        /// Defaults documentados del framework. Un valor con default <b>no</b> falla si la clave está ausente; un
        /// valor sin default sí (<see cref="CooperatorConfigurationException"/>).
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// Timeout de comandos por defecto, en segundos. Es el de 1.4 cuando la clave no estaba definida:
            /// <c>(new SqlCommand()).CommandTimeout</c> (<c>Data/SQLHelper.cs:88</c>).
            /// </summary>
            public const int CommandTimeoutSeconds = 30;

            /// <summary>TTL de las colecciones: 1 hora, igual que 1.4 (ADR-004 §3.1).</summary>
            public const int CacheDefaultTtlSeconds = 3600;

            /// <summary>TTL de la metadata de stored procedures: 1 día, igual que 1.4 (ADR-004 §3.1).</summary>
            public const int CacheParameterMetadataTtlSeconds = 86400;

            /// <summary>
            /// Cap de entradas de la caché en memoria (ADR-004 §3.1: «un cap de entradas <b>configurable</b> con
            /// desalojo»). El Bolt 011 publicó la <b>clave</b> y declaró que la <b>cifra</b> es de su dueño
            /// (<c>MEM-261016-0117</c> §8.1); la fija el Bolt 013 al aportar la caché. Acota la memoria que un
            /// proceso de escritorio de larga vida puede retener (lo que 1.4 no hacía: su caché de colecciones no
            /// expiraba ni estaba acotada) y queda holgadamente por encima del número de colecciones y stored
            /// procedures que la capa cachea en un proceso real; cada entrada está a su vez acotada por el límite
            /// por colección de 1.4 (<c>MaxObjectsInCacheForReadOnlyTable</c>, 5000).
            /// </summary>
            public const int CacheMaxEntries = 1024;
        }
    }
}
