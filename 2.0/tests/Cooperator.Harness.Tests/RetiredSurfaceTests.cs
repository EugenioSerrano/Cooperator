using System.Reflection;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-1, AC-2 y AC-3 del SPEC-261016-0012 §7: la capacidad de export a Excel no
/// existe en el árbol 2.0 y su rastro de plataforma tampoco.
/// <list type="bullet">
///   <item><b>AC-1</b> (inspección dura): sobre el ensamblado <b>construido</b>
///         <c>Cooperator.Framework.Core.dll</c>, con <c>MetadataLoadContext</c>
///         (se inspeccionan metadatos: sin cargar ni ejecutar el ensamblado), el
///         tipo <c>ObjectListHelper&lt;T,T2&gt;</c> no expone ningún miembro de
///         export y conserva <c>FillDataSet</c> con su firma y su forma.</item>
///   <item><b>AC-2</b>: la capacidad no tiene <b>ningún uso vivo</b> y sus
///         menciones quedan <b>clasificadas</b> (la constancia de la superficie
///         retirada nombra a propósito lo que desapareció).</item>
///   <item><b>AC-3</b>: el rastro de plataforma (OLE DB y los proveedores de
///         archivos de Microsoft) da <b>0 ocurrencias</b> en el código, los
///         proyectos, los scripts y la documentación propios del árbol 2.0.</item>
/// </list>
/// <para>
/// <b>Alcance de la búsqueda (parte del criterio, SPEC §6/A.2, §6/C.2 y AC-3).</b>
/// Se excluyen <c>1.4/**</c> —el árbol congelado, que no se escanea como código
/// propio— y <c>2.0/tests/fixtures/api-baseline-1.4/**</c> —la evidencia 1.4
/// congelada que el Bolt 001 versionó como <b>contrato del gate</b>: contiene el
/// tipo del helper y su referencia al proveedor de datos de plataforma, y sin la
/// exclusión el propio contrato que el gate necesita haría fallar AC-3—. La
/// exclusión <b>no se autoproclama</b>: el caso
/// <c>The_excluded_evidence_is_what_the_exclusion_declares</c> la mide.
/// </para>
/// </summary>
public sealed class RetiredSurfaceTests
{
    // Los marcadores NO se escriben literales: se ENSAMBLAN, para que este mismo
    // archivo —que es código propio del árbol— no sea una ocurrencia del rastro
    // que AC-3 exige en cero. Sin esto, el control sería su propio hallazgo.
    private static readonly string[] PlatformMarkers =
    [
        "Ole" + "Db",
        "Microsoft." + "Jet",
        "Microsoft." + "ACE",
    ];

    private static readonly string[] CapacityMarkers =
    [
        "CreateExcel" + "File",
        "xls" + "Lock",
    ];

    /// <summary>Extensiones de texto propio que el rastro no puede alcanzar.</summary>
    private static readonly string[] OwnedExtensions =
    [
        ".cs", ".csproj", ".props", ".targets", ".sln",
        ".ps1", ".psm1", ".psd1", ".json", ".md", ".txt", ".xml", ".ruleset",
    ];

    /// <summary>Directorios que no son código propio: salidas de build y resultados.</summary>
    private static readonly string[] ExcludedSegments = ["bin", "obj", "artifacts", "TestResults"];

    private const string BaselineFixtureRelative = @"tests\fixtures\api-baseline-1.4";

    [Fact]
    public void The_built_core_assembly_does_not_expose_the_export_and_keeps_the_data_contract()
    {
        var dll = Repo.InTree("src", "Cooperator.Framework.Core", "bin", "Release", "netstandard2.1", "Cooperator.Framework.Core.dll");
        Assert.True(File.Exists(dll), $"Falta el ensamblado construido del núcleo: {dll}. Correr el comando reproducible antes.");

        using var context = NewContext(dll);
        var assembly = context.LoadFromAssemblyPath(dll);

        var helper = assembly.GetType("Cooperator.Framework.Core.ObjectListHelper`2", throwOnError: false)
            ?? throw new InvalidOperationException("El ensamblado construido del núcleo no contiene ObjectListHelper<T,T2>.");

        Assert.True(helper.IsPublic, "ObjectListHelper<T,T2> dejó de ser público.");
        Assert.True(helper.IsGenericTypeDefinition, "ObjectListHelper<T,T2> dejó de ser una definición genérica.");

        // Forma de la clase: dos parámetros genéricos, con las MISMAS restricciones.
        var genericParameters = helper.GetGenericArguments();
        Assert.Equal(2, genericParameters.Length);
        Assert.Equal(new[] { "T", "T2" }, genericParameters.Select(parameter => parameter.Name).ToArray());

        var firstConstraint = genericParameters[0].GetGenericParameterConstraints().Single();
        Assert.Equal("Cooperator.Framework.Core.IObject", firstConstraint.FullName);

        var secondConstraint = genericParameters[1].GetGenericParameterConstraints().Single();
        Assert.Equal("System.Collections.Generic.List`1", secondConstraint.GetGenericTypeDefinition().FullName);

        // AC-1: ningún miembro de export, con el resto de la superficie conservada.
        var declaredNames = helper
            .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(member => member.Name)
            .ToArray();

        Assert.DoesNotContain(CapacityMarkers[0], declaredNames);
        Assert.DoesNotContain(CapacityMarkers[1], declaredNames);

        var fillDataSet = helper
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .SingleOrDefault(method => method.Name == "FillDataSet")
            ?? throw new InvalidOperationException("ObjectListHelper<T,T2> perdió FillDataSet: es un contrato de datos.");

        Assert.Equal("System.Void", fillDataSet.ReturnType.FullName);

        var fillParameters = fillDataSet.GetParameters();
        Assert.Equal(2, fillParameters.Length);
        Assert.Equal("System.Data.DataSet", fillParameters[0].ParameterType.FullName);
        Assert.True(fillParameters[1].ParameterType.IsGenericParameter, "El segundo parámetro de FillDataSet dejó de ser el RowSet genérico.");
        Assert.Equal("T2", fillParameters[1].ParameterType.Name);
    }

    [Fact]
    public void The_platform_trace_is_absent_from_the_owned_tree()
    {
        var scan = Scan(PlatformMarkers);

        // Control contra el verde falso: el escaneo tiene que haber visto el árbol de verdad.
        Assert.True(scan.FilesScanned > 20, $"El escaneo sólo vio {scan.FilesScanned} archivos: no puede declarar ausencia.");
        Assert.Contains(scan.Files, file => file.EndsWith(@"src\Cooperator.Framework.Core\ObjectListHelper.cs", StringComparison.Ordinal));

        // AC-3, «0 ocurrencias» del rastro: el código, los proyectos y los scripts del
        // árbol propio no contienen ni una (ni el using, ni tipos, ni referencias, ni
        // paquetes, ni cadena de conexión).
        Assert.DoesNotContain(scan.Matches, match => match.Kind != MatchKind.Documentation);

        // La constancia de la superficie retirada nombra la directiva que desapareció
        // —SPEC §6/B.1 exige «qué desaparece … y firma exacta»—: es la mención
        // documental DECLARADA, y ninguna otra parte de la documentación la repite.
        Assert.NotEmpty(scan.Matches);
        Assert.All(scan.Matches, match => Assert.EndsWith(@"docs\api-removals\US-000.BOLT-006.json", match.Path, StringComparison.Ordinal));
    }

    [Fact]
    public void The_capacity_has_no_live_use_and_its_mentions_are_classified()
    {
        var scan = Scan(CapacityMarkers);

        // AC-2: 0 usos vivos. Las menciones admitidas son la documentación (la
        // constancia de la superficie retirada, que DEBE nombrar lo que desapareció).
        Assert.DoesNotContain(scan.Matches, match => match.Kind == MatchKind.Live);

        var documentary = scan.Matches.Where(match => match.Kind == MatchKind.Documentation).ToArray();
        Assert.Contains(documentary, match => match.Path.EndsWith(@"docs\api-removals\US-000.BOLT-006.json", StringComparison.Ordinal));
        Assert.Contains(
            CapacityMarkers,
            marker => documentary.Any(match => match.Text.Contains(marker, StringComparison.Ordinal)));
    }

    [Fact]
    public void The_excluded_evidence_is_what_the_exclusion_declares()
    {
        // AC-3: la exclusión de la evidencia 1.4 congelada del gate es PARTE DEL
        // CRITERIO, y se mide por qué: ese directorio contiene de verdad el rastro, de
        // modo que excluirlo es necesario (y declarado), no una forma de tapar un uso
        // vivo del árbol propio.
        var fixture = Repo.InTree("tests", "fixtures", "api-baseline-1.4");
        Assert.True(Directory.Exists(fixture), $"Falta la evidencia 1.4 congelada del gate: {fixture}.");

        var hits = Directory
            .GetFiles(fixture, "*.dll")
            .SelectMany(file =>
            {
                var text = Latin1(file);
                return PlatformMarkers
                    .Where(marker => text.Contains(marker, StringComparison.Ordinal))
                    .Select(marker => $"{Path.GetFileName(file)}: {marker}");
            })
            .ToArray();

        Assert.NotEmpty(hits);
        Assert.Contains(hits, hit => hit.StartsWith("Cooperator.Framework.Core.dll", StringComparison.Ordinal));
    }

    [Fact]
    public void A_match_inside_a_comment_is_not_a_live_use()
    {
        // Edge case del SPEC §8.1: una coincidencia en un comentario NO es un uso vivo
        // —es un caso de clasificación—, mientras que una línea de código sí lo es.
        Assert.Equal(MatchKind.Comment, Classify(@"src\a.cs", "        // " + CapacityMarkers[0] + " se retiró en otro Bolt"));
        Assert.Equal(MatchKind.Comment, Classify(@"src\a.cs", "        /// " + CapacityMarkers[1] + " serializaba el acceso al archivo"));
        Assert.Equal(MatchKind.Comment, Classify(@"build\a.ps1", "# " + CapacityMarkers[0]));
        Assert.Equal(MatchKind.Live, Classify(@"src\a.cs", "        helper." + CapacityMarkers[0] + "(rowSet, \"Hoja1\", \"salida.xls\");"));
        Assert.Equal(MatchKind.Documentation, Classify(@"docs\api-removals\US-000.BOLT-006.json", "  \"member\": \"" + CapacityMarkers[0] + "\","));
    }

    // ------------------------------------------------------------------ escaneo

    private enum MatchKind
    {
        /// <summary>Uso vivo: código propio que usa la capacidad o su rastro.</summary>
        Live,

        /// <summary>Mención en un comentario: informativa (SPEC §8.1).</summary>
        Comment,

        /// <summary>Mención en la documentación o en la constancia: informativa y a propósito.</summary>
        Documentation,
    }

    private sealed record Match(string Path, int Line, string Text, MatchKind Kind);

    private sealed record ScanResult(string[] Files, Match[] Matches)
    {
        public int FilesScanned => Files.Length;
    }

    private static ScanResult Scan(string[] markers)
    {
        var files = OwnedFiles().ToArray();
        var matches = new List<Match>();

        foreach (var file in files)
        {
            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (IOException)
            {
                continue; // un archivo bloqueado por el build no invalida el escaneo
            }

            var lines = text.Split('\n');
            for (var index = 0; index < lines.Length; index++)
            {
                if (!markers.Any(marker => lines[index].Contains(marker, StringComparison.Ordinal)))
                {
                    continue;
                }

                matches.Add(new Match(Relative(file), index + 1, lines[index].Trim(), Classify(file, lines[index])));
            }
        }

        return new ScanResult(files, [.. matches]);
    }

    private static IEnumerable<string> OwnedFiles()
    {
        var tree = Repo.InTree();

        return Directory
            .EnumerateFiles(tree, "*", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => ExcludedSegments.Contains(segment, StringComparer.OrdinalIgnoreCase)))
            .Where(file => !file.Contains(BaselineFixtureRelative, StringComparison.OrdinalIgnoreCase))
            .Where(file => OwnedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));
    }

    private static MatchKind Classify(string path, string line)
    {
        var relative = Relative(path);

        if (relative.StartsWith(@"docs\", StringComparison.OrdinalIgnoreCase)
            || relative.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            || relative.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            return MatchKind.Documentation;
        }

        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("//", StringComparison.Ordinal)
            || trimmed.StartsWith("/*", StringComparison.Ordinal)
            || trimmed.StartsWith("*", StringComparison.Ordinal)
            || trimmed.StartsWith("#", StringComparison.Ordinal)
            || trimmed.StartsWith("<!--", StringComparison.Ordinal)
            || trimmed.StartsWith("'", StringComparison.Ordinal))
        {
            return MatchKind.Comment;
        }

        return MatchKind.Live;
    }

    private static string Relative(string path) => Path.GetRelativePath(Repo.InTree(), path);

    /// <summary>Lectura de un binario sin interpretación: sólo se busca texto en sus bytes.</summary>
    private static string Latin1(string path) => System.Text.Encoding.Latin1.GetString(File.ReadAllBytes(path));

    private static MetadataLoadContext NewContext(params string[] assemblyPaths)
    {
        // El resolvedor incluye el runtime del host (para resolver la BCL: DataSet,
        // Object, List…) y el ensamblado inspeccionado. MetadataLoadContext no ejecuta
        // nada: sólo lee los metadatos que cada miembro necesita.
        var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var paths = Directory.GetFiles(runtime, "*.dll").ToList();
        paths.AddRange(assemblyPaths);

        return new MetadataLoadContext(new PathAssemblyResolver(paths));
    }
}
