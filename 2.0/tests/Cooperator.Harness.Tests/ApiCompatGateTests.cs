using System.Text.Json;
using System.Text.RegularExpressions;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Self-test automatizado del gate de compatibilidad de API (SPEC-261016-0000
/// §6.D.3, AC-4 y AC-5). Verifica por test hermético y reproducible —no por
/// observación manual— que:
/// <list type="bullet">
///   <item>ante el par de fixtures con la ruptura deliberada el gate FALLA
///         (código de salida 2 del vocabulario 0/1/2/3);</item>
///   <item>el reporte IDENTIFICA el miembro público retirado y la firma alterada;</item>
///   <item>el reporte JSON existe y declara el modo estricto y la herramienta fijada;</item>
///   <item>un baseline manipulado ABORTA con código 3 en lugar de compararse.</item>
/// </list>
/// </summary>
public sealed class ApiCompatGateTests
{
    private const string GateRelativePath = @"build\api-compat.ps1";

    [Fact]
    public void Gate_fails_and_names_every_deliberate_break()
    {
        var outputDirectory = NewOutputDirectory();
        var result = RunGate("FixturePair", outputDirectory);

        // AC-5: el gate FALLA. Código 2 = "diferencias de API" del vocabulario compartido.
        Assert.Equal(2, result.ExitCode);

        var reportPath = Path.Combine(outputDirectory, "api-compat-report.json");
        Assert.True(File.Exists(reportPath), $"El gate no dejó reporte JSON en {reportPath}:{Environment.NewLine}{result.AllText}");

        using var report = JsonDocument.Parse(File.ReadAllText(reportPath));
        var differences = report.RootElement.GetProperty("differences")
            .EnumerateArray().Select(x => x.GetString() ?? string.Empty).ToArray();

        // El miembro público retirado y la firma alterada deben quedar NOMBRADOS.
        Assert.Contains(differences, d => d.Contains("Cooperator.ApiCompatFixture.Sample.RemovedMethod", StringComparison.Ordinal));
        Assert.Contains(differences, d => d.Contains("Cooperator.ApiCompatFixture.Sample.ChangedSignature(int)", StringComparison.Ordinal));
        Assert.Contains(differences, d => d.Contains("Cooperator.ApiCompatFixture.Sample.ChangedSignature(string)", StringComparison.Ordinal));
    }

    [Fact]
    public void Gate_report_declares_strict_mode_the_pinned_tool_and_a_nonzero_comparator_exit()
    {
        var outputDirectory = NewOutputDirectory();
        var result = RunGate("FixturePair", outputDirectory);
        Assert.Equal(2, result.ExitCode);

        using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(outputDirectory, "api-compat-report.json")));

        Assert.True(report.RootElement.GetProperty("strict_mode").GetBoolean());
        Assert.Equal("FixturePair", report.RootElement.GetProperty("mode").GetString());
        Assert.Equal(3, report.RootElement.GetProperty("difference_count").GetInt32());

        var toolVersion = report.RootElement.GetProperty("tool_version").GetString()!;
        Assert.Contains("Microsoft.DotNet.ApiCompat", toolVersion, StringComparison.Ordinal);
        Assert.Contains("10.0.402", toolVersion, StringComparison.Ordinal);

        Assert.NotEqual(0, report.RootElement.GetProperty("comparator_exit_code").GetInt32());
    }

    [Fact]
    public void Gate_writes_a_report_file_the_harness_can_read()
    {
        var outputDirectory = NewOutputDirectory();
        RunGate("FixturePair", outputDirectory);

        var text = File.ReadAllText(Path.Combine(outputDirectory, "api-compat-report.txt"));
        Assert.Contains("Cooperator API-compat gate", text, StringComparison.Ordinal);
        Assert.Contains("resultado   : differences", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Gate_aborts_with_3_when_the_baseline_was_tampered_with()
    {
        // SPEC §8.1, edge case: «baseline manipulado (digest distinto) → el gate debe
        // abortar con 3, no comparar contra un baseline alterado». Se trabaja sobre una
        // COPIA del baseline versionado: el fixture real nunca se toca.
        var copy = Path.Combine(Path.GetTempPath(), "cooperator-harness-baseline-" + Guid.NewGuid().ToString("N"));
        CopyDirectory(Repo.InTree("tests", "fixtures", "api-baseline-1.4"), copy);
        try
        {
            var victim = Path.Combine(copy, "Cooperator.Framework.Core.dll");
            var bytes = File.ReadAllBytes(victim);
            bytes[bytes.Length - 1] ^= 0xFF;
            File.WriteAllBytes(victim, bytes);

            var outputDirectory = NewOutputDirectory();
            var result = RunGate("Baseline", outputDirectory, "-BaselineDirectory", copy);

            Assert.Equal(3, result.ExitCode);

            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(outputDirectory, "api-compat-report.json")));
            Assert.Equal("blocked", report.RootElement.GetProperty("status").GetString());
            Assert.Contains(
                report.RootElement.GetProperty("notes").EnumerateArray().Select(x => x.GetString() ?? string.Empty),
                note => note.Contains("Cooperator.Framework.Core.dll", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(copy, recursive: true);
        }
    }

    private static string NewOutputDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "cooperator-harness-gate-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static CommandResult RunGate(string mode, string outputDirectory, params string[] extra)
    {
        var arguments = new List<string>
        {
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", Repo.InTree(GateRelativePath.Split('\\')),
            "-Mode", mode,
            "-OutputDirectory", outputDirectory,
        };
        arguments.AddRange(extra);

        var result = Shell.Run(Shell.PowerShell7, Repo.InTree(), [.. arguments]);
        // Si el host no arrancó (por ejemplo `pwsh` ausente), el test debe fallar de forma
        // explícita y no dar un verde falso por un gate que nunca corrió.
        Assert.DoesNotContain("is not recognized", result.AllText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CommandNotFoundException", result.AllText, StringComparison.Ordinal);
        return result;
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
        }
    }
}
