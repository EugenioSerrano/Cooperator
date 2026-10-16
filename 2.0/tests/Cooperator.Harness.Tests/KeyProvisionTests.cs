using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-1, AC-4 y AC-5 del SPEC-261016-0035 §7: proteger exige la <b>referencia explícita de clave</b>, un <c>kid</c>
/// sin clave es un <b>error</b> —nunca una degradación— y <b>no hay material de clave en el producto</b>
/// (NFR <i>No embedded secrets</i> de ADR-007 §5, incluida la constancia de que la clave del formato legacy
/// <b>no reaparece en ninguna forma</b>).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class KeyProvisionTests
{
    private const string ConfigurationKey = "Cooperator:ConnectionStrings:Primary";

    [Fact]
    public void The_environment_provider_reads_the_published_variable_and_cleans_up()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        string variable = EnvironmentValueKeyProvider.VariableName("kid-env");

        Assert.Equal("Cooperator" + "__ProtectedValues__Keys__" + "kid-env", variable);

        Environment.SetEnvironmentVariable(variable, Convert.ToBase64String(key));
        try
        {
            EnvironmentValueKeyProvider provider = new EnvironmentValueKeyProvider();
            byte[]? read = provider.TryGetKey("kid-env");
            Assert.NotNull(read);
            Assert.Equal(key, read!);
            Assert.Null(provider.TryGetKey("kid-ausente"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, null);
        }

        Assert.Null(new EnvironmentValueKeyProvider().TryGetKey("kid-env"));
    }

    [Fact]
    public void A_malformed_environment_key_is_a_typed_error_without_the_material()
    {
        string variable = EnvironmentValueKeyProvider.VariableName("kid-malformado");
        byte[] key = ProtectedValueTestSupport.NewKey();
        string material = Convert.ToBase64String(key);

        // No es Base64 válido.
        Environment.SetEnvironmentVariable(variable, "no-es-base64!!");
        try
        {
            ProtectedValueKeyProviderException exception = Assert.Throws<ProtectedValueKeyProviderException>(
                () => new EnvironmentValueKeyProvider().TryGetKey("kid-malformado"));
            Assert.DoesNotContain("no-es-base64!!", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, null);
        }

        // Longitud distinta de 32 bytes: no se recorta, no se deriva y no se completa con ceros.
        Environment.SetEnvironmentVariable(variable, Convert.ToBase64String(key, 0, 16));
        try
        {
            ProtectedValueKeyProviderException exception = Assert.Throws<ProtectedValueKeyProviderException>(
                () => new EnvironmentValueKeyProvider().TryGetKey("kid-malformado"));
            Assert.Contains("32", exception.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(Convert.ToBase64String(key, 0, 16), exception.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(material, exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, null);
        }
    }

    [Fact]
    public void An_unknown_kid_never_degrades_to_plain_text()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        string protectedValue = resolver.Protect(ConfigurationKey, "un valor");
        string payload = protectedValue.Substring("aesgcm".Length + 1);
        string foreign = "kidDesconocido" + ":" + payload.Substring(payload.IndexOf(':') + 1);

        ProtectedValueKeyNotFoundException exception = Assert.Throws<ProtectedValueKeyNotFoundException>(
            () => resolver.Resolve(ConfigurationKey, foreign));
        Assert.Equal("kidDesconocido", exception.Kid);

        // Y sin clave activa, proteger es un error: no hay clave implícita (AC-1).
        ProtectedValueKeyRegistry empty = ProtectedValueTestSupport.Registry();
        Assert.Throws<ProtectedValueKeyNotFoundException>(
            () => new AesGcmValueResolver(empty).Protect(ConfigurationKey, "un valor"));
    }

    [Fact]
    public void Protecting_requires_the_explicit_key_reference_and_no_implicit_overload_exists()
    {
        // AC-1/AC-9 por reflexión: la operación de proteger exige (clave de configuración, valor) y la superficie
        // pública del framework no expone ninguna API de cifrado con clave implícita.
        MethodInfo protect = typeof(AesGcmValueResolver).GetMethod("Protect")!;
        Assert.Equal(new[] { typeof(string), typeof(string) }, protect.GetParameters().Select(parameter => parameter.ParameterType).ToArray());

        string[] forbiddenNames = ["Encrypt", "Decrypt"];
        foreach (Type type in typeof(AesGcmValueResolver).Assembly.GetExportedTypes())
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Assert.DoesNotContain(method.Name, forbiddenNames);
            }
        }

        // La clase estática del formato legacy ya no existe en el ensamblado.
        Assert.Null(typeof(AesGcmValueResolver).Assembly.GetType("Cooperator.Framework.Library.Cryptography", throwOnError: false));
    }

    [Fact]
    public void The_active_key_protects_and_the_read_keys_keep_resolving_with_a_declared_overlap_window()
    {
        byte[] previous = ProtectedValueTestSupport.NewKey();
        byte[] current = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry(
            ("kidAnterior", previous),
            ("kidNuevo", current));

        DateTimeOffset until = new DateTimeOffset(2026, 11, 15, 0, 0, 0, TimeSpan.FromHours(-3));

        // La clave anterior protege mientras es la activa…
        keys.SetActive("kidAnterior");
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);
        string beforeRotation = resolver.Protect(ConfigurationKey, "valor previo a la rotación");
        Assert.StartsWith("aesgcm" + ":" + "kidAnterior" + ":", beforeRotation, StringComparison.Ordinal);

        // …y tras la rotación la nueva es la activa, con la ventana de solapamiento declarada con FECHA.
        keys.SetActive("kidNuevo", until);
        Assert.Equal("kidNuevo", keys.ActiveKid);
        Assert.Equal(until, keys.OverlapUntil);

        string afterRotation = resolver.Protect(ConfigurationKey, "valor posterior a la rotación");
        Assert.StartsWith("aesgcm" + ":" + "kidNuevo" + ":", afterRotation, StringComparison.Ordinal);

        // El valor de la clave anterior SIGUE resolviendo (está en lectura) y no se usa para proteger.
        Assert.Equal("valor previo a la rotación", resolver.Resolve(ConfigurationKey, beforeRotation.Substring("aesgcm".Length + 1)));
        Assert.Equal("valor posterior a la rotación", resolver.Resolve(ConfigurationKey, afterRotation.Substring("aesgcm".Length + 1)));

        // Un kid sin clave disponible es error tipado: falla al FIJAR la referencia activa —antes de proteger— y la
        // activa anterior queda intacta: nunca se "prueba con otra clave" ni se degrada a texto plano.
        Assert.Throws<ProtectedValueKeyNotFoundException>(() => keys.SetActive("kidSinClave"));
        Assert.Equal("kidNuevo", keys.ActiveKid);
        Assert.Throws<ProtectedValueKeyNotFoundException>(() => keys.RequireReadKey("kidSinClave"));

        // La activa anterior quedó intacta: proteger sigue funcionando con ELLA (no se probó con otra clave).
        string stillActive = resolver.Protect(ConfigurationKey, "otro valor");
        Assert.StartsWith("aesgcm" + ":" + "kidNuevo" + ":", stillActive, StringComparison.Ordinal);
        Assert.Equal("otro valor", resolver.Resolve(ConfigurationKey, stillActive.Substring("aesgcm".Length + 1)));
    }

    [Fact]
    public void The_host_key_hook_failure_is_a_typed_error_with_the_original_cause()
    {
        ProtectedValueTestSupport.TestKeyProvider provider = new ProtectedValueTestSupport.TestKeyProvider(
            (ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(provider);
        keys.SetActive(ProtectedValueTestSupport.KeyOne);

        InvalidOperationException original = new InvalidOperationException("el almacén del host no responde");
        provider.Failure = original;

        ProtectedValueKeyProviderException exception = Assert.Throws<ProtectedValueKeyProviderException>(
            () => keys.RequireReadKey(ProtectedValueTestSupport.KeyOne));

        Assert.Same(original, exception.InnerException);
        Assert.Contains(ProtectedValueTestSupport.KeyOne, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void No_key_material_lives_in_the_tree_and_the_legacy_key_does_not_reappear()
    {
        // (1) La clave del formato legacy del 1.4 en CUALQUIER forma: 0 apariciones en el árbol 2.0. El valor se
        // ENSAMBLA para que este archivo —código propio— no sea él mismo una ocurrencia.
        string legacyKey = "*1Am|" + "9Kh)+" + "/?sfGq88";
        string[] roots = [Repo.InTree("src"), Repo.InTree("tests"), Repo.InTree("docs"), Repo.InTree("build")];
        string[] extensions = [".cs", ".json", ".md", ".ps1", ".props", ".csproj", ".txt"];

        string[] scanned = [.. roots
            .Where(Directory.Exists)
            .SelectMany(root => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj" or "artifacts" or "TestResults"))
            .Where(file => !file.Contains(@"tests\fixtures\api-baseline-1.4", StringComparison.OrdinalIgnoreCase))
            .Where(file => extensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))];

        Assert.True(scanned.Length > 40, $"El escaneo sólo vio {scanned.Length} archivos: no puede declarar ausencia.");

        string[] legacyHits = [.. scanned.Where(file => File.ReadAllText(file).Contains(legacyKey, StringComparison.Ordinal))];
        Assert.Empty(legacyHits);

        // (2) Y el propio 1.4 lo conserva: si esto falla, la búsqueda no está mirando donde cree.
        string[] fourthLineEvidence = [.. Directory.EnumerateFiles(Repo.InRepo("1.4"), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .Where(file => File.ReadAllText(file).Contains(legacyKey, StringComparison.Ordinal))];
        Assert.NotEmpty(fourthLineEvidence);

        // (3) Gate de constantes de alta entropía (ADR-007 §5): ninguna cadena con forma de clave vive en las fuentes
        // del producto. El detector se prueba contra un valor generado EN MEMORIA (nunca un literal en el repo).
        Assert.True(IsHighEntropyConstant(Convert.ToBase64String(ProtectedValueTestSupport.NewKey())), "El detector de constantes de alta entropía no tiene dientes.");

        string[] entropyHits = [.. Directory.EnumerateFiles(Repo.InTree("src"), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .SelectMany(file => HighEntropyLiterals(File.ReadAllText(file)).Select(literal => Path.GetFileName(file) + ": " + literal.Length + " caracteres"))];
        Assert.Empty(entropyHits);
    }

    /// <summary>Busca literales entrecomillados con alfabeto Base64 y longitud de clave.</summary>
    private static IEnumerable<string> HighEntropyLiterals(string source) =>
        [.. System.Text.RegularExpressions.Regex
            .Matches(source, "\"([A-Za-z0-9+/=]{24,})\"")
            .Select(match => match.Groups[1].Value)
            .Where(IsHighEntropyConstant)];

    /// <summary>Un valor Base64 decodificable a 16 bytes o más con entropía alta es una constante de alta entropía.</summary>
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

        // Una CLAVE decodifica a 32 bytes de material con entropía alta y a bytes binarios: un identificador escrito
        // por una persona —aunque comparta el alfabeto Base64— decodifica a pocos bytes y a ASCII imprimible.
        if (bytes.Length < ProtectedValueFormat.KeySize
            || bytes.Count(value => value < 0x20 || value > 0x7E) * 4 < bytes.Length)
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
}
