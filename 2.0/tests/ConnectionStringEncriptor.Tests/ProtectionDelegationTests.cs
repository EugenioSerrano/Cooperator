using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;
using ConnectionStringEncriptor.Protection;

namespace ConnectionStringEncriptor.Tests;

/// <summary>
/// AC-2, AC-5, AC-6 y AC-7 del SPEC-261016-0043 §7: la herramienta <b>no contiene protección propia</b> (0
/// primitivas, 0 claves, 0 composición del valor), los <b>tres diagnósticos</b> de fallo del motor son accionables y
/// sin material, hay <b>una sola implementación y un solo formato</b> (el del motor) y el <b>cambio de superficie de
/// usuario</b> queda declarado para las notas de migración.
/// </summary>
public sealed class ProtectionDelegationTests
{
    private const string Destination = "Cooperator:ConnectionStrings:CooperatorConnectionString";

    [Fact]
    public void The_tool_has_no_crypto_primitive_no_key_and_no_own_composition()
    {
        string[] sources = [.. AppSources()];
        Assert.True(sources.Length >= 6, $"El escaneo sólo vio {sources.Length} archivos de la utilidad.");

        // Las primitivas y las claves se ENSAMBLAN para que este archivo —prueba propia— no sea una ocurrencia.
        string[] forbidden =
        [
            "System.Security.Cryptography",
            "CreateEncryptor", "CreateDecryptor", "ComputeHash",
            "MD5" + "CryptoServiceProvider", "Triple" + "DES", "DES" + "CryptoServiceProvider", "RC2", "SHA1",
            "Rijndael", "CipherMode", "PaddingMode",
            "_Crypto" + "Key", "_" + "Iv",
            "Cryptography" + ".Encrypt", "Cryptography" + ".Decrypt",
            "Convert.FromBase64String", "Convert.ToBase64String",
        ];

        foreach (string source in sources)
        {
            string text = Code(File.ReadAllText(source));
            foreach (string token in forbidden)
            {
                Assert.False(
                    text.Contains(token, StringComparison.Ordinal),
                    Path.GetFileName(source) + " usa '" + token + "': la protección es del motor (ADR-007 §3.5).");
            }
        }

        // Gate de constantes de alta entropía (ADR-007 §5): ninguna cadena con forma de clave en la utilidad.
        // El detector se prueba contra un valor generado EN MEMORIA (nunca un literal en el repositorio).
        Assert.True(IsHighEntropyConstant(Convert.ToBase64String(NewKey())), "El detector de entropía no tiene dientes.");
        string[] entropyHits =
        [
            .. sources
                .Where(file => Path.GetExtension(file) == ".cs")
                .SelectMany(file => HighEntropyLiterals(File.ReadAllText(file)).Select(literal => Path.GetFileName(file) + ": " + literal.Length + " caracteres")),
        ];
        Assert.Empty(entropyHits);

        // Y el binario de la utilidad referencia el motor, no una criptografía propia.
        string[] referenced = [.. typeof(ProtectValueStep).Assembly.GetReferencedAssemblies().Select(assembly => assembly.Name ?? string.Empty)];
        Assert.Contains("Cooperator.Framework.Library", referenced);
        Assert.DoesNotContain("System.Security.Cryptography.Algorithms", referenced);
        Assert.DoesNotContain("System.Data", referenced);
    }

    [Fact]
    public void The_single_format_is_the_engine_format_and_the_legacy_shape_does_not_exist_here()
    {
        byte[] key = NewKey();
        ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(new FakeKeyProvider(new Dictionary<string, byte[]> { ["kidShell"] = key }));
        keys.SetActive("kidShell");
        AesGcmValueResolver engine = new AesGcmValueResolver(keys);
        ProtectValueStep step = new ProtectValueStep(engine);

        string protectedValue = step.Protect(Destination, "un valor").ProtectedValue;

        // Un solo formato: el del motor, con su composición y su codificación; se resuelve con SU resolutor.
        Assert.StartsWith(ProtectedValueFormat.AesGcmScheme + ":", protectedValue, StringComparison.Ordinal);
        Assert.Equal(
            "un valor",
            engine.Resolve(Destination, protectedValue.Substring(ProtectedValueFormat.AesGcmScheme.Length + 1)));

        // La única operación de protección de la utilidad es la del motor: no hay una segunda ruta ni un modo
        // "compatibilidad" que lea o escriba el formato viejo (esa capacidad es del Bolt 023).
        string[] sources = [.. AppSources()];
        foreach (string source in sources)
        {
            string text = Code(File.ReadAllText(source));
            foreach (string token in new[] { "compatibilidad", "legacy", "formato viejo", "3DES", "BinaryFormatter" })
            {
                Assert.False(
                    text.Contains(token, StringComparison.OrdinalIgnoreCase),
                    Path.GetFileName(source) + " menciona '" + token + "' en código: no hay segunda ruta (ADR-009 §3.6).");
            }
        }

        // Y la operación de protección del paso es, literalmente, la del motor (una sola implementación).
        Assert.Contains(
            "engine.Protect",
            File.ReadAllText(AppTreePath("src", "ConnectionStringEncriptor", "Protection", "ProtectValueStep.cs")),
            StringComparison.Ordinal);
    }

    [Fact]
    public void The_three_engine_failures_are_actionable_and_carry_no_value_or_material()
    {
        string syntheticValue = "synthetic-" + Guid.NewGuid().ToString("N");
        byte[] key = NewKey();
        string material = Convert.ToBase64String(key);

        // (1) clave ACTIVA ausente: el motor falla tipado y el paso NO devuelve un resultado.
        ProtectedValueKeyRegistry withoutActive = new ProtectedValueKeyRegistry(new FakeKeyProvider(new Dictionary<string, byte[]> { ["kidShell"] = key }));
        ProtectedValueKeyNotFoundException missingActive = Assert.Throws<ProtectedValueKeyNotFoundException>(
            () => new ProtectValueStep(new AesGcmValueResolver(withoutActive)).Protect(Destination, syntheticValue));

        // (2) kid sin clave disponible.
        ProtectedValueKeyRegistry withActive = new ProtectedValueKeyRegistry(new FakeKeyProvider(new Dictionary<string, byte[]> { ["kidShell"] = key }));
        withActive.SetActive("kidShell");
        AesGcmValueResolver engine = new AesGcmValueResolver(withActive);
        string foreign = "kidDesconocido" + ":" + engine.Protect(Destination, syntheticValue).Substring("aesgcm".Length + 1).Split(':')[1];
        ProtectedValueKeyNotFoundException unknownKid = Assert.Throws<ProtectedValueKeyNotFoundException>(
            () => engine.Resolve(Destination, foreign));

        // (3) proveedor del host que falla.
        InvalidOperationException cause = new InvalidOperationException("el almacén del host no responde");
        ProtectedValueKeyRegistry failing = new ProtectedValueKeyRegistry(new ThrowingKeyProvider(cause));
        ProtectedValueKeyProviderException providerFailure = Assert.Throws<ProtectedValueKeyProviderException>(() => failing.RequireReadKey("kidShell"));
        Assert.Same(cause, providerFailure.InnerException);

        foreach (Exception failure in new Exception[] { missingActive, unknownKid, providerFailure })
        {
            string message = ProtectionDiagnostics.Describe(failure);

            Assert.True(message.Length > 40, "El diagnóstico debe decir qué falta y qué hacer.");
            Assert.DoesNotContain(syntheticValue, message, StringComparison.Ordinal);
            Assert.DoesNotContain(material, message, StringComparison.Ordinal);
            Assert.DoesNotContain("32 bytes en Base64\").GetHashCode", message, StringComparison.Ordinal);
            Assert.Contains("NO se protegió", message, StringComparison.Ordinal);
        }

        // El diagnóstico nombra la variable de entorno de la referencia afectada (accionable).
        Assert.Contains(EnvironmentValueKeyProvider.VariableName("kidDesconocido"), ProtectionDiagnostics.Describe(unknownKid), StringComparison.Ordinal);
    }

    [Fact]
    public void The_user_surface_change_is_declared_in_the_migration_register()
    {
        string fragment = AppTreePath("docs", "api-removals", "US-000.BOLT-024.json");
        Assert.True(File.Exists(fragment), $"Falta la constancia de la ruptura para las notas: {fragment}");

        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(fragment));
        JsonElement root = document.RootElement;
        Assert.Equal("US-000.BOLT-024", root.GetProperty("bolt").GetString());

        JsonElement[] entries = [.. root.GetProperty("entries").EnumerateArray()];
        Assert.NotEmpty(entries);

        string all = string.Join("\n", entries.Select(entry => entry.GetRawText()));
        // Los tres datos de ADR-008 §3.1: qué cambia, por qué (ADR-009 §3.6 / ADR-007 §3.5) y con qué se reemplaza.
        Assert.Contains("WinForms", all, StringComparison.Ordinal);
        Assert.Contains("WPF", all, StringComparison.Ordinal);
        Assert.Contains("ADR-009", all, StringComparison.Ordinal);
        Assert.Contains("ADR-007", all, StringComparison.Ordinal);
        Assert.Contains("clave activa", all, StringComparison.Ordinal);

        // La superficie anterior era WinForms y en la utilidad no queda ningún resto de ella.
        foreach (string source in AppSources())
        {
            Assert.False(
                Code(File.ReadAllText(source)).Contains("System.Windows.Forms", StringComparison.Ordinal),
                Path.GetFileName(source) + " referencia System.Windows.Forms: ADR-009 §3.5 lo retira del tooling.");
        }
    }

    /// <summary>
    /// El texto de un archivo <b>sin comentarios</b>: un comentario no es un uso (misma clasificación que los
    /// controles del arnés). Evita que la documentación que <i>nombra</i> lo prohibido cuente como una ocurrencia.
    /// </summary>
    /// <param name="text">El contenido del archivo.</param>
    /// <returns>Las líneas de código, sin líneas de comentario.</returns>
    private static string Code(string text)
    {
        System.Text.StringBuilder code = new System.Text.StringBuilder();
        bool inBlock = false;

        foreach (string line in text.Split('\n'))
        {
            string trimmed = line.TrimStart();
            if (inBlock)
            {
                if (trimmed.StartsWith("-->", StringComparison.Ordinal) || trimmed.StartsWith("*/", StringComparison.Ordinal))
                {
                    inBlock = false;
                }

                continue;
            }

            if (trimmed.StartsWith("<!--", StringComparison.Ordinal) || trimmed.StartsWith("/*", StringComparison.Ordinal))
            {
                inBlock = true;
                continue;
            }

            if (trimmed.StartsWith("///", StringComparison.Ordinal)
                || trimmed.StartsWith("//", StringComparison.Ordinal)
                || trimmed.StartsWith("*", StringComparison.Ordinal))
            {
                continue;
            }

            code.AppendLine(line);
        }

        return code.ToString();
    }

    private static IEnumerable<string> AppSources() =>
        Directory
            .EnumerateFiles(AppTreePath("src", "ConnectionStringEncriptor"), "*", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .Where(file => Path.GetExtension(file) is ".cs" or ".xaml" or ".csproj")
            .OrderBy(file => file, StringComparer.Ordinal);

    private static IEnumerable<string> HighEntropyLiterals(string source) =>
        [.. System.Text.RegularExpressions.Regex
            .Matches(source, "\"([A-Za-z0-9+/=]{40,})\"")
            .Select(match => match.Groups[1].Value)
            .Where(IsHighEntropyConstant)];

    private static bool IsHighEntropyConstant(string candidate)
    {
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(candidate);
        }
        catch (FormatException)
        {
            return false;
        }

        if (bytes.Length < ProtectedValueFormat.KeySize || bytes.Count(value => value < 0x20 || value > 0x7E) * 4 < bytes.Length)
        {
            return false;
        }

        int[] histogram = new int[256];
        foreach (byte value in bytes)
        {
            histogram[value]++;
        }

        double entropy = 0;
        foreach (int count in histogram)
        {
            if (count == 0)
            {
                continue;
            }

            double probability = (double)count / bytes.Length;
            entropy -= probability * Math.Log2(probability);
        }

        return entropy >= 4.5;
    }

    private static byte[] NewKey()
    {
        byte[] key = new byte[ProtectedValueFormat.KeySize];
        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetBytes(key);
        }

        return key;
    }

    private static string AppTreePath(params string[] parts) => Path.Combine([RepositoryRoot(), "2.0", .. parts]);

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

    private sealed class FakeKeyProvider : IProtectedValueKeyProvider
    {
        private readonly Dictionary<string, byte[]> _keys;

        internal FakeKeyProvider(Dictionary<string, byte[]> keys)
        {
            _keys = keys;
        }

        public byte[]? TryGetKey(string kid) => _keys.TryGetValue(kid, out byte[]? key) ? key : null;
    }

    private sealed class ThrowingKeyProvider : IProtectedValueKeyProvider
    {
        private readonly Exception _failure;

        internal ThrowingKeyProvider(Exception failure)
        {
            _failure = failure;
        }

        public byte[]? TryGetKey(string kid) => throw _failure;
    }
}
