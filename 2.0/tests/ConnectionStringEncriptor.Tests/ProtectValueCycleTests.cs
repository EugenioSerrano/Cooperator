using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;
using ConnectionStringEncriptor;
using ConnectionStringEncriptor.Protection;

namespace ConnectionStringEncriptor.Tests;

/// <summary>
/// AC-1, AC-3, AC-4 y AC-5 del SPEC-261016-0043 §7: el <b>ciclo completo</b> (proteger con la clave activa y
/// <b>resolver</b> lo protegido con el motor), el manejo del <b>valor en claro</b> (no se persiste, no se muestra, no
/// se registra), las <b>entradas mínimas</b> del paso y el <b>diagnóstico accionable</b> de los fallos del motor.
/// <para>
/// Las claves y los valores de las pruebas son <b>sintéticos y generados en memoria</b> (el SPEC es
/// <c>confidential</c>): ninguna credencial real entra al repositorio ni a la evidencia.
/// </para>
/// </summary>
public sealed class ProtectValueCycleTests
{
    private const string Destination = "Cooperator:ConnectionStrings:CooperatorConnectionString";
    private const string Kid = "kidShell";

    [Fact]
    public void The_full_cycle_protects_with_the_active_key_and_resolves_back_to_the_input()
    {
        byte[] key = NewKey();
        AesGcmValueResolver engine = EngineFor(key, Kid);
        ProtectValueStep step = new ProtectValueStep(engine);

        foreach (string value in new[]
                 {
                     "Server=.;Database=Ventas;Integrated Security=SSPI",
                     "acentuación y símbolos: ñ, ü, 漢字",
                     new string('x', 4096),
                     string.Empty,
                     "  con espacios en los extremos  ",
                 })
        {
            ProtectionResult result = step.Protect(Destination, value);

            // El esquema vigente y el kid de la clave activa (el valor lo produce EL MOTOR).
            Assert.StartsWith("aesgcm" + ":" + Kid + ":", result.ProtectedValue, StringComparison.Ordinal);
            Assert.Equal(Kid, result.Kid);
            Assert.Equal("aesgcm", result.Scheme);

            // Y **se resuelve** con el resolutor del motor: es la prueba de que el shell produce un valor útil.
            string payload = result.ProtectedValue.Substring("aesgcm".Length + 1);
            Assert.Equal(value, engine.Resolve(Destination, payload));
        }
    }

    [Fact]
    public void Two_protections_of_the_same_value_both_resolve_with_distinct_payloads()
    {
        byte[] key = NewKey();
        AesGcmValueResolver engine = EngineFor(key, Kid);
        ProtectValueStep step = new ProtectValueStep(engine);

        ProtectionResult first = step.Protect(Destination, "el mismo valor");
        ProtectionResult second = step.Protect(Destination, "el mismo valor");

        // El payload es no determinista (nonce por operación, propiedad del motor) y AMBOS resuelven.
        Assert.NotEqual(first.ProtectedValue, second.ProtectedValue);
        Assert.Equal("el mismo valor", engine.Resolve(Destination, first.ProtectedValue.Substring("aesgcm".Length + 1)));
        Assert.Equal("el mismo valor", engine.Resolve(Destination, second.ProtectedValue.Substring("aesgcm".Length + 1)));
    }

    [Fact]
    public void A_value_that_is_already_protected_is_protected_again_without_inspection()
    {
        byte[] key = NewKey();
        AesGcmValueResolver engine = EngineFor(key, Kid);
        ProtectValueStep step = new ProtectValueStep(engine);

        // Esta herramienta NO inspecciona el estado de un valor (eso es del Bolt 023): lo protege igual.
        string alreadyProtected = step.Protect(Destination, "un valor").ProtectedValue;
        ProtectionResult again = step.Protect(Destination, alreadyProtected);

        Assert.Equal(
            alreadyProtected,
            engine.Resolve(Destination, again.ProtectedValue.Substring("aesgcm".Length + 1)));
    }

    [Fact]
    public void The_destination_key_is_required_and_the_step_refuses_a_malformed_one()
    {
        ProtectValueStep step = new ProtectValueStep(EngineFor(NewKey(), Kid));

        Assert.Throws<ArgumentNullException>(() => step.Protect(null!, "un valor"));
        Assert.Throws<ArgumentException>(() => step.Protect(string.Empty, "un valor"));
        Assert.Throws<ArgumentException>(() => step.Protect("   ", "un valor"));
        Assert.Throws<ArgumentException>(() => step.Protect(" " + Destination + " ", "un valor"));
        Assert.Throws<ArgumentNullException>(() => step.Protect(Destination, null!));
    }

    [Fact]
    public void The_clear_value_never_lands_in_disk_state_or_logs()
    {
        // Corrida instrumentada (AC-3): el valor de entrada es sintético y distintivo, y se busca en TODO lo escrito.
        string syntheticValue = "synthetic-" + Guid.NewGuid().ToString("N") + "-clear-value";
        string scratch = Path.Combine(Path.GetTempPath(), "cooperator-encriptor-clear-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scratch);

        try
        {
            AesGcmValueResolver engine = EngineFor(NewKey(), Kid);
            ProtectValueStep step = new ProtectValueStep(engine);

            string previous = Environment.CurrentDirectory;
            Environment.CurrentDirectory = scratch;
            try
            {
                ProtectionResult result = step.Protect(Destination, syntheticValue);
                Assert.Equal(syntheticValue, engine.Resolve(Destination, result.ProtectedValue.Substring("aesgcm".Length + 1)));
            }
            finally
            {
                Environment.CurrentDirectory = previous;
            }

            // (a) nada se escribió en la carpeta de la corrida…
            Assert.Empty(Directory.GetFileSystemEntries(scratch));

            // …(b) ni en el árbol de la utilidad (fuentes, binarios y su salida), ni en la salida de las pruebas.
            string[] searched =
            [
                .. AppSources(),
                .. Files(AppTreePath("src", "ConnectionStringEncriptor", "bin")),
                .. Files(AppTreePath("tests", "ConnectionStringEncriptor.Tests", "bin")),
            ];
            Assert.True(searched.Length > 10, $"El escaneo sólo vio {searched.Length} archivos: no puede declarar ausencia.");
            Assert.DoesNotContain(searched, file => File.ReadAllText(file).Contains(syntheticValue, StringComparison.Ordinal));

            // (c) y el proyecto no tiene NINGUNA vía de persistencia de estado ni de registro del valor.
            foreach (string source in AppSources())
            {
                string text = Code(File.ReadAllText(source));
                foreach (string forbidden in new[] { "System.IO", "StreamWriter", "Microsoft.Win32", "RegistryKey", "Settings.Default", "Trace.", "Console.", "AppData" })
                {
                    Assert.False(
                        text.Contains(forbidden, StringComparison.Ordinal),
                        Path.GetFileName(source) + " usa '" + forbidden + "': el valor en claro no se persiste ni se registra (AC-3).");
                }
            }
        }
        finally
        {
            Directory.Delete(scratch, recursive: true);
        }
    }

    [Fact]
    public void The_ui_captures_the_value_masked_and_has_no_algorithm_or_key_selector()
    {
        string windowXaml = File.ReadAllText(AppTreePath("src", "ConnectionStringEncriptor", "ProtectValueWindow.xaml"));
        string windowCode = File.ReadAllText(AppTreePath("src", "ConnectionStringEncriptor", "ProtectValueWindow.xaml.cs"));

        // AC-3: la entrada del valor está ENMASCARADA y no hay ninguna vista del valor original.
        Assert.Contains("PasswordBox", windowXaml, StringComparison.Ordinal);
        Assert.DoesNotContain("ver el valor", windowXaml, StringComparison.OrdinalIgnoreCase);

        // AC-4: las entradas del paso son el valor y la clave de destino; NO hay selector de algoritmo ni de clave.
        Assert.DoesNotContain("ComboBox", windowXaml, StringComparison.Ordinal);
        Assert.DoesNotContain("Algorithm", windowXaml, StringComparison.Ordinal);
        Assert.Contains("DestinationKeyBox", windowXaml, StringComparison.Ordinal);
        Assert.Contains("ValueBox", windowXaml, StringComparison.Ordinal);

        // La clave activa la aporta el MOTOR por su provisión: la ventana sólo lee su referencia del entorno.
        Assert.Contains("EnvironmentValueKeyProvider", windowCode, StringComparison.Ordinal);
        Assert.Contains("SetActive", windowCode, StringComparison.Ordinal);
        Assert.Contains(ProtectValueWindow.ActiveKeyReferenceVariable, windowCode, StringComparison.Ordinal);

        // Y al cerrar el paso se limpia el estado (no queda el valor retenido).
        Assert.Contains("OnClosed", windowCode, StringComparison.Ordinal);
        Assert.Contains("ValueBox.Clear()", windowCode, StringComparison.Ordinal);
    }

    [Fact]
    public void A_missing_active_key_is_a_typed_failure_of_the_engine_and_of_the_step()
    {
        // Sin clave activa fijada, el MOTOR falla tipado y el paso NO devuelve un resultado dudoso (AC-5).
        ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(new FakeKeyProvider(new Dictionary<string, byte[]>()));
        ProtectValueStep step = new ProtectValueStep(new AesGcmValueResolver(keys));

        ProtectedValueKeyNotFoundException failure = Assert.Throws<ProtectedValueKeyNotFoundException>(
            () => step.Protect(Destination, "un valor"));
        Assert.Contains("ACTIVA", failure.Message, StringComparison.Ordinal);
    }

    private static AesGcmValueResolver EngineFor(byte[] key, string kid)
    {
        ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(new FakeKeyProvider(new Dictionary<string, byte[]> { [kid] = key }));
        keys.SetActive(kid);
        return new AesGcmValueResolver(keys);
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

    private static IEnumerable<string> AppSources() =>
        Directory
            .EnumerateFiles(AppTreePath("src", "ConnectionStringEncriptor"), "*", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .Where(file => Path.GetExtension(file) is ".cs" or ".xaml" or ".csproj")
            .OrderBy(file => file, StringComparer.Ordinal);

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

    private static IEnumerable<string> Files(string root) =>
        Directory.Exists(root)
            ? Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).OrderBy(file => file, StringComparer.Ordinal)
            : Enumerable.Empty<string>();

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

    /// <summary>Proveedor de claves en memoria: el doble del contrato de provisión del motor (ADR-007 §3.2).</summary>
    private sealed class FakeKeyProvider : IProtectedValueKeyProvider
    {
        private readonly Dictionary<string, byte[]> _keys;

        internal FakeKeyProvider(Dictionary<string, byte[]> keys)
        {
            _keys = keys;
        }

        public byte[]? TryGetKey(string kid) => _keys.TryGetValue(kid, out byte[]? key) ? key : null;
    }
}
