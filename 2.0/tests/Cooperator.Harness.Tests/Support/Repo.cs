using System.Diagnostics;
using System.Text;

namespace Cooperator.Harness.Tests.Support;

/// <summary>
/// Localización del árbol del repositorio y ejecución de comandos externos.
/// El arnés vive en <c>2.0/tests/&lt;proyecto&gt;/bin/&lt;config&gt;/&lt;tfm&gt;/</c>:
/// la raíz se descubre caminando hacia arriba hasta la solución <c>2.0/Cooperator2.sln</c>,
/// nunca por ruta absoluta codificada.
/// </summary>
internal static class Repo
{
    public static string Root { get; } = LocateRoot();

    public static string TreeRoot => Path.Combine(Root, "2.0");

    public static string InTree(params string[] parts) =>
        parts.Length == 0 ? TreeRoot : Path.Combine([TreeRoot, .. parts]);

    public static string InRepo(params string[] parts) =>
        parts.Length == 0 ? Root : Path.Combine([Root, .. parts]);

    private static string LocateRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "2.0", "Cooperator2.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            $"No se encontró la raíz del repositorio (2.0/Cooperator2.sln) partiendo de '{AppContext.BaseDirectory}'.");
    }
}

internal sealed record CommandResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string AllText => StandardOutput + Environment.NewLine + StandardError;
}

internal static class Shell
{
    /// <summary>PowerShell de PowerShell 7 (el host documentado por el SPEC §6.G).</summary>
    public static string PowerShell7 { get; } = Resolve("pwsh");

    /// <summary>Windows PowerShell 5.1, el host de .NET Framework.</summary>
    public static string WindowsPowerShell { get; } = Resolve("powershell");

    public static CommandResult Run(string fileName, params string[] arguments) =>
        Run(fileName, Repo.TreeRoot, arguments);

    public static CommandResult Run(string fileName, string workingDirectory, params string[] arguments)
    {
        var psi = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var argument in arguments)
        {
            psi.ArgumentList.Add(argument);
        }

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"No se pudo iniciar '{fileName}'.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit(600_000);
        return new CommandResult(process.ExitCode, stdout, stderr);
    }

    public static CommandResult Git(params string[] arguments)
    {
        var all = new List<string> { "-C", Repo.Root };
        all.AddRange(arguments);
        return Run("git", Repo.Root, [.. all]);
    }

    private static string Resolve(string command) => command;
}
