using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Data;
using Cooperator.Framework.Data.Caching;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// AC-1, AC-4, AC-5, AC-6 y AC-9 del SPEC-261016-0020 §7, con los **controles NFR search-based** de §8.1: el
/// provider único, el timeout del catálogo, la cadena sin inspección, la ausencia de esperas activas y de
/// `providerName`, el retiro del caché del host (§C.6) y la política de nulabilidad del aporte (§C.5).
/// <para>
/// Los tokens prohibidos se <b>ensamblan desde fragmentos</b> para que este archivo —código propio— no sea él mismo
/// una ocurrencia de lo que persigue.
/// </para>
/// </summary>
[Collection("DataCaching")]
public sealed class DataLayerContractTests
{
    private static readonly string[] LayerFiles =
    [
        "BaseRule.cs", "BaseGateway.cs", "BaseLoader.cs", "SQLHelper.cs", "SqlHelperParameterCache.cs", "IBaseGateway.cs",
    ];

    private static readonly string HostCacheToken = "Http" + "Context";
    private static readonly string WebPlatformToken = "System." + "Web";
    private static readonly string LegacyProviderToken = "System.Data." + "SqlClient";
    private static readonly string ConfigManagerToken = "Configuration" + "Manager";

    [Fact]
    public void The_layer_has_no_host_cache_no_web_platform_and_no_legacy_keys()
    {
        foreach (string file in LayerFiles)
        {
            string text = Source(file);
            Assert.DoesNotContain(HostCacheToken, text, StringComparison.Ordinal);
            Assert.DoesNotContain(WebPlatformToken, text, StringComparison.Ordinal);
            Assert.DoesNotContain("Coop_DAL_", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Cooperator_DAL_", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_layer_has_no_active_waiting_no_provider_name_and_no_legacy_provider()
    {
        foreach (string file in LayerFiles)
        {
            string text = Source(file);
            Assert.DoesNotContain("Thread." + "Sleep", text, StringComparison.Ordinal);
            Assert.DoesNotContain("provider" + "Name", text, StringComparison.Ordinal);
            Assert.DoesNotContain(LegacyProviderToken, text, StringComparison.Ordinal);
            Assert.DoesNotContain(ConfigManagerToken, text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_nfr_no_hidden_casts_holds_outside_the_single_conversion_point()
    {
        foreach (string file in LayerFiles)
        {
            string text = Source(file);
            // El punto único (Casting/TransactionAdapter.cs) no usa casts: valida con patrones de tipo.
            Assert.DoesNotContain("(SqlTransaction)", text, StringComparison.Ordinal);
            Assert.DoesNotContain("(SqlConnection)", text, StringComparison.Ordinal);
        }

        string adapter = Source(@"Casting\TransactionAdapter.cs");
        Assert.DoesNotMatch(@"(?<!typeof)\(SqlTransaction\)", adapter);
        Assert.DoesNotMatch(@"(?<!typeof)\(SqlConnection\)", adapter);
        Assert.Contains("is SqlTransaction", adapter, StringComparison.Ordinal);
    }

    [Fact]
    public void The_chain_comes_from_the_contract_without_inspection_and_without_the_static_dictionary()
    {
        string rule = Source("BaseRule.cs");

        // La cadena entra RESUELTA por el contrato (ADR-004 §3.2/§3.3) y la capa no inspecciona su contenido.
        Assert.Contains("CooperatorSettings.Current.GetConnectionString(", rule, StringComparison.Ordinal);
        Assert.DoesNotContain("ConnectionStringsDictionary", rule, StringComparison.Ordinal);
        Assert.DoesNotContain("initial catalog", rule, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("data source", rule, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Cryptography", rule, StringComparison.Ordinal);
        Assert.DoesNotContain("SetConnectionStringValue", rule, StringComparison.Ordinal);
    }

    [Fact]
    public void The_command_timeout_comes_from_the_catalog_and_validates_the_value()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            // SPEC-261016-0019 6/B.1: el punto del timeout usa el contrato de caché (Bolt 013). Cada fase estrena
            // caché para que la lectura vuelva al catálogo: lo que se verifica es la RESOLUCIÓN, no la retención.
            CacheRegions.Provider = new InMemoryCacheProvider(16);
            CooperatorSettings.Current = new StubConfiguration(configured: 45);
            Assert.Equal(45, ReadCommandTimeout());

            // El default documentado del catálogo es el que la capa usa cuando la clave no está.
            CacheRegions.Provider = new InMemoryCacheProvider(16);
            CooperatorSettings.Current = new StubConfiguration(configured: null);
            Assert.Equal(ConfigurationKeys.Defaults.CommandTimeoutSeconds, ReadCommandTimeout());

            // Y un valor inválido NO habilita un fallback propio: se propaga el error del contrato (AC-4).
            CacheRegions.Provider = new InMemoryCacheProvider(16);
            CooperatorSettings.Current = new StubConfiguration(exception: CooperatorConfigurationException.NotAnInteger("x", "abc"));
            // La capa se invoca por reflexi\u00f3n (es implementaci\u00f3n): el error del CONTRATO viaja como causa interna.
            System.Reflection.TargetInvocationException wrapped = Assert.Throws<System.Reflection.TargetInvocationException>(() => ReadCommandTimeout());
            Assert.IsType<CooperatorConfigurationException>(wrapped.InnerException);
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_parameter_types_of_the_provider_change_are_declared_at_construction()
    {
        // §8.1: el armado del parámetro de los tipos que cambian de comportamiento, sin ejecutar contra la base.
        SqlParameter time = new SqlParameter("@when", SqlDbType.Time) { Value = TimeSpan.FromMinutes(90) };
        Assert.Equal(typeof(TimeSpan), time.Value.GetType());

        SqlParameter date = new SqlParameter("@day", SqlDbType.Date) { Value = new DateTime(2026, 10, 16) };
        Assert.Equal(new DateTime(2026, 10, 16), date.Value);
    }

    [Fact]
    public void Data_declares_exactly_the_authorized_provider()
    {
        string props = File.ReadAllText(TreePath("Directory.Packages.props"));
        Assert.Contains("Microsoft.Data.SqlClient", props, StringComparison.Ordinal);
        Assert.DoesNotContain(LegacyProviderToken, props, StringComparison.Ordinal);

        string project = File.ReadAllText(TreePath("src", "Cooperator.Framework.Data", "Cooperator.Framework.Data.csproj"));
        string[] references =
        [
            .. System.Text.RegularExpressions.Regex.Matches(project, "<PackageReference Include=\"([^\"]+)\"")
                .Select(match => match.Groups[1].Value),
        ];
        Assert.Equal(["Microsoft.Data.SqlClient"], references);
        Assert.DoesNotContain("Version=", project, StringComparison.Ordinal);
    }

    [Fact]
    public void The_nullability_directive_lives_only_in_the_aportes_the_build_required()
    {
        string[] withDirective = ["BaseRule.cs", "BaseGateway.cs", "BaseLoader.cs", "SQLHelper.cs"];

        foreach (string file in LayerFiles)
        {
            bool hasDirective = RawSource(file).Contains("#nullable disable", StringComparison.Ordinal);
            Assert.Equal(withDirective.Contains(file), hasDirective);
        }

        // Y NINGÚN .csproj del árbol apaga el análisis del proyecto entero.
        foreach (string project in Directory.GetFiles(TreePath("src"), "*.csproj", SearchOption.AllDirectories))
        {
            Assert.DoesNotContain("<Nullable>disable</Nullable>", File.ReadAllText(project), StringComparison.Ordinal);
        }
    }

    /// <summary>Invoca la resolución del timeout de la capa (privada: es implementación, no superficie).</summary>
    private static int ReadCommandTimeout()
    {
        Type helper = typeof(BaseRule).Assembly.GetType("Cooperator.Framework.Data.SqlHelper", throwOnError: true)!;
        MethodInfo method = helper.GetMethod("GetCommnandTimeOut", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (int)method.Invoke(null, null)!;
    }

    /// <summary>
    /// El CODIGO de un archivo de la capa, <b>sin comentarios</b>: un comentario no es un uso (misma clasificacion
    /// que los controles del arnes). Evita que la documentacion que NOMBRA lo prohibido cuente como ocurrencia.
    /// </summary>
    private static string Source(params string[] parts) => StripComments(RawSource(parts));

    /// <summary>El archivo tal cual (para la politica de nulabilidad, cuya directiva ES un comentario).</summary>
    private static string RawSource(params string[] parts) => File.ReadAllText(TreePath(["src", "Cooperator.Framework.Data", .. parts]));

    private static string StripComments(string text)
    {
        System.Text.StringBuilder code = new System.Text.StringBuilder();
        bool inBlock = false;

        foreach (string line in text.Split('\n'))
        {
            string trimmed = line.TrimStart();
            if (inBlock)
            {
                if (trimmed.StartsWith("*/", StringComparison.Ordinal))
                {
                    inBlock = false;
                }

                continue;
            }

            if (trimmed.StartsWith("/*", StringComparison.Ordinal))
            {
                inBlock = true;
                continue;
            }

            if (trimmed.StartsWith("///", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal) || trimmed.StartsWith("*", StringComparison.Ordinal))
            {
                continue;
            }

            code.AppendLine(line);
        }

        return code.ToString();
    }

    private static string TreePath(params string[] parts) => Path.Combine([RepositoryRoot(), "2.0", .. parts]);

    private static string RepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "2.0", "Cooperator2.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio (2.0/Cooperator2.sln).");
    }

    /// <summary>Doble del contrato de configuración: entrega un valor, el default del catálogo, o falla.</summary>
    private sealed class StubConfiguration : ICooperatorConfiguration
    {
        private readonly int? _configured;
        private readonly Exception? _exception;

        internal StubConfiguration(int? configured = null, Exception? exception = null)
        {
            _configured = configured;
            _exception = exception;
        }

        public string GetString(string key) => throw new NotSupportedException();

        public string GetString(string key, string defaultValue) => defaultValue;

        public int GetInt(string key)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            return _configured ?? throw CooperatorConfigurationException.MissingKey(key);
        }

        public int GetInt(string key, int defaultValue) => _exception != null ? throw _exception : _configured ?? defaultValue;

        public string GetConnectionString(string name) => throw new NotSupportedException();
    }
}
