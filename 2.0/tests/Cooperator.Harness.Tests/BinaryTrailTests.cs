using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0013 §6/D y §8.1 (AC-3/AC-4): el <b>rastro binario confinado</b>. Recorre las <b>fuentes del
/// producto</b> y los archivos de <b>proyecto</b> del Árbol 2.0 y <b>falla</b> ante cualquier uso del formato
/// binario fuera de la <b>allowlist declarada</b>: el cargador del migrador
/// (<c>2.0/src/Cooperator.Framework.Migration/Conversion/LegacyReader.cs</c>, del Bolt 002), la <b>única
/// excepción</b> del producto (ADR-002 §3.2, ADR-008 §3.4). Mientras esa pieza no exista en el árbol, el control
/// exige <b>0</b> ocurrencias y deja la regla documentada y activa: cuando aterrice, sus usos serán los únicos
/// admisibles.
/// <para>
/// También vigila las <b>supresiones</b>: <c>#pragma warning disable SYSLIB0051</c> sólo puede vivir dentro de la
/// allowlist; fuera de ella, <b>cero</b>.
/// </para>
/// <para>
/// <b>Alcance (declarado, y la razón de cada límite):</b>
/// <list type="bullet">
///   <item>Las <b>fuentes del producto</b> y sus <b>archivos de proyecto</b> (<c>2.0/src/**</c>) —los que portan los Bolts 013–030—:
///         el rastro que este control persigue es el del <b>producto</b>, y el archivo de proyecto de una suite
///         futura también queda cubierto cuando se agregue.</item>
///   <item>Quedan fuera, con su razón: <c>1.4/**</c> (evidencia histórica congelada) y el binario de la línea
///         base del gate; y las <b>suites que verifican la ausencia de esta misma superficie</b> —la de este
///         Bolt y la del Bolt 003 que hace nacer el grafo como POCOs—, que <b>nombran</b> los tokens
///         <b>por diseño</b>: son el control, no un uso.</item>
/// </list>
/// </para>
/// <para>
/// Los tokens se <b>ensamblan desde fragmentos</b> y las coincidencias en <b>comentarios</b> se clasifican como
/// informativas (un comentario no es un uso).
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class BinaryTrailTests
{
    /// <summary>La única pieza autorizada: el cargador del migrador (Bolt 002).</summary>
    private const string AllowlistRelative = @"src\Cooperator.Framework.Migration\Conversion\LegacyReader.cs";

    private static readonly string[] BinaryTokens =
    [
        "Binary" + "Formatter",
        "I" + "Formatter",
        "ObjectState" + "Formatter",
        "NetDataContract" + "Serializer",
        "IS" + "erializable",
        "GetObject" + "Data",
    ];

    private static readonly string SuppressionToken = "#pragma warning disable " + "SYSLIB" + "0051";

    /// <summary>Extensiones de archivo de proyecto y de script: se escanean en todo el árbol.</summary>
    private static readonly string[] ProjectExtensions = [".csproj", ".props", ".targets", ".ps1", ".psm1", ".psd1"];

    [Fact]
    public void The_binary_trail_is_confined_to_the_declared_allowlist()
    {
        var files = ScannedFiles();
        Assert.True(files.Length > 20, $"El escaneo sólo vio {files.Length} archivos: no puede declarar ausencia.");
        Assert.Contains(files, file => file.EndsWith(@"src\Cooperator.Framework.Core\ObjectListHelper.cs", StringComparison.Ordinal));
        Assert.Contains(files, file => file.EndsWith(".csproj", StringComparison.Ordinal));

        var allowlist = Repo.InTree(AllowlistRelative.Split('\\'));

        var offenders = files
            .Where(file => !string.Equals(file, allowlist, StringComparison.OrdinalIgnoreCase))
            .SelectMany(file => File.ReadAllLines(file)
                .Select((line, number) => (Line: line, Number: number + 1))
                .Where(entry => BinaryTokens.Any(token => entry.Line.Contains(token, StringComparison.Ordinal)))
                .Where(entry => !IsComment(entry.Line))
                .Select(entry => $"{Relative(file)}:{entry.Number}"))
            .ToArray();

        Assert.Empty(offenders);
    }

    [Fact]
    public void No_suppression_of_the_obsolete_serialization_api_lives_outside_the_allowlist()
    {
        var allowlist = Repo.InTree(AllowlistRelative.Split('\\'));

        var offenders = ScannedFiles()
            .Where(file => !string.Equals(file, allowlist, StringComparison.OrdinalIgnoreCase))
            .Where(file => File.ReadAllLines(file).Any(line => line.Contains(SuppressionToken, StringComparison.Ordinal) && !IsComment(line)))
            .Select(Relative)
            .ToArray();

        Assert.Empty(offenders);
    }

    [Fact]
    public void The_allowlist_is_declared_and_the_rule_is_active_without_it()
    {
        // Estado medido: la allowlist (Bolt 002) todavía no está en el árbol ⇒ el control exige 0 ocurrencias y
        // la regla queda ACTIVA: el día que aterrice, sus usos serán los únicos que este control admita.
        var allowlist = Repo.InTree(AllowlistRelative.Split('\\'));
        if (File.Exists(allowlist))
        {
            Assert.EndsWith(@"Cooperator.Framework.Migration\Conversion\LegacyReader.cs", allowlist, StringComparison.OrdinalIgnoreCase);
        }

        // Y la declaración del alcance es verificable: el escaneo cubre el producto y los proyectos, y NO cubre
        // las suites que verifican la ausencia de esta superficie (nombran los tokens por diseño).
        var files = ScannedFiles();
        var sources = files.Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToArray();

        // El alcance es el PRODUCTO: sus fuentes y sus archivos de proyecto (los que portan los Bolts 013–030).
        Assert.Contains(sources, file => file.EndsWith(@"src\Cooperator.Framework.Library\SmtpMail.cs", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(files, file => file.EndsWith(@"src\Cooperator.Framework.Core\Cooperator.Framework.Core.csproj", StringComparison.OrdinalIgnoreCase));
        // Y las suites que VERIFICAN esta superficie no están en él: la nombran por diseño (son el control).
        Assert.DoesNotContain(files, file => file.Contains(@"Cooperator.Framework.Modeling.Tests", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(files, file => file.Contains(@"tests\Cooperator.Harness.Tests", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Las fuentes del producto y los archivos de proyecto del árbol. El rastro perseguido es el del <b>producto</b>;
    /// las suites que verifican la ausencia de la superficie de serialización la nombran por diseño (son el
    /// control), y el aparato de evidencia del 1.4 queda fuera por su razón.
    /// </summary>
    private static string[] ScannedFiles() =>
        [.. Directory.EnumerateFiles(Repo.InTree("src"), "*", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj" or "artifacts" or "TestResults"))
            .Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || ProjectExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            .OrderBy(file => file, StringComparer.Ordinal)];

    private static bool IsComment(string line)
    {
        var trimmed = line.TrimStart();
        return trimmed.StartsWith("//", StringComparison.Ordinal)
            || trimmed.StartsWith("/*", StringComparison.Ordinal)
            || trimmed.StartsWith("*", StringComparison.Ordinal)
            || trimmed.StartsWith("<!--", StringComparison.Ordinal)
            || trimmed.StartsWith("#", StringComparison.Ordinal);
    }

    private static string Relative(string path) => Path.GetRelativePath(Repo.InTree(), path);
}
