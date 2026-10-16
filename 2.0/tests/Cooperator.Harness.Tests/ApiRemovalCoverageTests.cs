using System.Text.Json;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-4 y AC-5 del SPEC-261016-0012 §7, y la parte ejecutable del NFR de ADR-008
/// §5: <b>el gate detecta la diferencia, la constancia la explica, y una diferencia
/// sin explicación FALLA</b>. El caso lee el reporte real del gate de compatibilidad
/// (línea base 1.4 ↔ superficie 2.0: <c>api-compat.ps1 -Mode Current</c>) y cruza
/// <b>toda</b> diferencia de miembro retirado (diagnóstico <c>CP0002</c>) contra la
/// unión de los fragmentos de <c>2.0/docs/api-removals/*.json</c> —el de este Bolt
/// (US-000.BOLT-006) y los de los Bolts que ya aportaron superficie—.
/// <para>
/// El modo <c>Current</c> es la corrida <b>informativa</b> del gate (el SPEC del
/// arnés lo dice: reporta la brecha completa mientras el porteo es parcial), así que
/// su código de salida esperado es <c>2</c> —«diferencias de API» del vocabulario
/// 0/1/2/3—: lo que este caso exige no es que la brecha esté cerrada, sino que cada
/// <b>miembro retirado de un tipo que ya existe</b> esté declarado en una constancia.
/// </para>
/// </summary>
public sealed class ApiRemovalCoverageTests
{
    private const string GateRelativePath = @"build\api-compat.ps1";

    /// <summary>Diagnóstico del comparador para «miembro que existe en el contrato y no en la implementación».</summary>
    private const string RemovedMemberCode = "CP0002";

    /// <summary>Diagnóstico del comparador para «ensamblado del contrato sin contraparte».</summary>
    private const string MissingAssemblyCode = "CP0004";

    /// <summary>Fragmento de constancia que aporta ESTE Bolt (SPEC §6/B.1).</summary>
    private const string FragmentName = "US-000.BOLT-006.json";

    /// <summary>
    /// Ensamblados sin contraparte que el gate reporta y que este caso <b>declara
    /// pendientes</b> (no los da por verdes): la brecha de un porteo parcial es
    /// trabajo de los Bolts que aportan cada tipo, y su ausencia se declara con su
    /// razón — el edge case «el ensamblado en alcance ausente del árbol» del SPEC §8.1.
    /// </summary>
    private static readonly (string Assembly, string Why)[] PendingAssemblies =
    [
        ("Cooperator.Framework.Utility", "ADR-001 §3.1: Cooperator.Framework.Utility no existe en la 2.0 — su contenido se reparte entre Modeling y el tooling, y cada Bolt declara los tipos que aporta."),
        ("Cooperator.Framework.Modeling", "ADR-001 §3.1: ensamblado NUEVO de la 2.0 (partición del modelo), sin contraparte en la línea base 1.4."),
    ];

    private static readonly Lazy<GateRun> Current = new(RunCurrentGate);

    [Fact]
    public void Every_removed_member_reported_by_the_gate_is_covered_by_a_fragment()
    {
        var run = Current.Value;
        var entries = Fragments();

        // El gate corrió de verdad y comparó los dos lados (sin esto, un reporte vacío
        // daría cobertura «total» sobre nada).
        Assert.Equal("Current", run.Mode);
        Assert.Equal("differences", run.Status);
        Assert.NotEqual(0, run.ComparatorExitCode);
        Assert.Contains(run.ImplementationSide, path => path.EndsWith("Cooperator.Framework.Core.dll", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(run.ContractSide, path => path.EndsWith("Cooperator.Framework.Utility.dll", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(run.DifferenceCount, run.Differences.Length);

        var removedMembers = RemovedMembers(run.Differences);
        Assert.Contains(removedMembers, member => member == "CreateExcel" + "File");

        Assert.Empty(UncoveredRemovedMembers(run.Differences, entries));
    }

    [Fact]
    public void This_bolts_removal_is_declared_with_the_three_data_of_adr_008()
    {
        var entry = Fragments()
            .SingleOrDefault(candidate => candidate.Source == FragmentName && candidate.Member == CapacityMember())
            ?? throw new InvalidOperationException($"El fragmento {FragmentName} no declara la ruptura del miembro retirado de ObjectListHelper.");

        // AC-5: qué desaparece (tipo, miembro y firma exacta).
        Assert.Contains("Cooperator.Framework.Core.ObjectListHelper", entry.Type, StringComparison.Ordinal);
        Assert.Equal(CapacityMember(), entry.Member);
        Assert.Contains("CreateExcel" + "File(", entry.Signature, StringComparison.Ordinal);
        Assert.Contains("rowSet", entry.Signature, StringComparison.Ordinal);

        // La decisión que lo ordena y el reemplazo: ninguno DENTRO del framework.
        Assert.Contains("ADR-001", entry.Why, StringComparison.Ordinal);
        Assert.Contains("Ninguno dentro del framework", entry.Replacement, StringComparison.Ordinal);

        // Y la entrada está completa: dueño y evidencia de verificación (AC-4).
        Assert.Equal("US-000.BOLT-006", entry.OwnerBolt);
        Assert.False(string.IsNullOrWhiteSpace(entry.Evidence));

        // La ruptura que este Bolt produce es exactamente la que el gate reporta.
        var reported = RemovedMembers(Current.Value.Differences);
        Assert.Contains(reported, member => member == entry.Member);
    }

    [Fact]
    public void A_removed_member_without_an_entry_fails_the_coverage()
    {
        // Edge case del SPEC §8.1: «miembro retirado que reaparece con la misma firma
        // (falla: contradice el retiro)» — y, por el mismo cruce, un miembro retirado
        // que NADIE declara. Se ejerce con una diferencia sintética: el caso real debe
        // quedar cubierto, y el sintético NO.
        var entries = Fragments();
        var synthetic = $"CP0002: Member 'void Cooperator.Framework.Core.ObjectListHelper<T, T2>.FillDataSet(System.Data.DataSet, T2)' exists on baseline.dll but not on built.dll";

        var uncovered = UncoveredRemovedMembers([synthetic], entries);

        Assert.Single(uncovered);
        Assert.Contains("FillDataSet", uncovered[0], StringComparison.Ordinal);
    }

    [Fact]
    public void An_entry_without_a_declared_replacement_does_not_cover()
    {
        // Edge case del SPEC §8.1: «una entrada sin reemplazo declarado (falla)».
        // Nombrar el miembro no alcanza: la constancia de ADR-008 §3.1 exige decir
        // también con qué se reemplaza.
        var incomplete = new FragmentEntry(
            Source: FragmentName,
            Id: "SYNTHETIC-01",
            Kind: "method-removed",
            Type: "Cooperator.Framework.Core.ObjectListHelper<T, T2>",
            Member: CapacityMember(),
            Signature: "sin firma declarada",
            What: "el miembro retirado sin reemplazo declarado",
            Why: "ADR-001",
            Replacement: "   ",
            Evidence: "—",
            OwnerBolt: "US-000.BOLT-006");

        var difference = $"CP0002: Member 'void Cooperator.Framework.Core.ObjectListHelper<T, T2>.{CapacityMember()}(T2, string, string)' exists on baseline.dll but not on built.dll";

        Assert.Single(UncoveredRemovedMembers([difference], [incomplete]));
        Assert.Empty(UncoveredRemovedMembers([difference], [Fragments().Single(entry => entry.Member == CapacityMember())]));
    }

    [Fact]
    public void The_assemblies_without_a_counterpart_are_declared_pending()
    {
        var run = Current.Value;

        // §8.1: «el ensamblado en alcance ausente del árbol: el caso lo declara como
        // pendiente y no lo da por verde». Se declara con su razón y se exige que el
        // declarado coincida con lo que el gate reporta — ni más ni menos.
        var reported = run.Differences
            .Where(difference => difference.StartsWith(MissingAssemblyCode, StringComparison.Ordinal))
            .SelectMany(difference => PendingAssemblies.Select(pending => pending.Assembly)
                .Where(name => difference.Contains(name, StringComparison.Ordinal)))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            PendingAssemblies.Select(pending => pending.Assembly).OrderBy(name => name, StringComparer.Ordinal).ToArray(),
            reported);
        Assert.All(PendingAssemblies, pending => Assert.False(string.IsNullOrWhiteSpace(pending.Why)));

        // Y la brecha de porteo (tipos que aún no viven en la 2.0) queda CONTADA, no
        // escondida: es el trabajo declarado de los Bolts que aportan cada archivo.
        var portGap = run.Differences.Count(difference => difference.StartsWith("CP0001", StringComparison.Ordinal));
        Assert.True(portGap > 0, "El porteo figura completo: revisar el gate antes de dar la brecha por cerrada.");
    }

    // ------------------------------------------------------------------ helpers

    private sealed record FragmentEntry(
        string Source,
        string Id,
        string Kind,
        string Type,
        string Member,
        string Signature,
        string What,
        string Why,
        string Replacement,
        string Evidence,
        string OwnerBolt)
    {
        public string Text => $"{Type} {Member} {Signature} {Kind} {What} {Why} {Replacement} {Evidence}";
    }

    private sealed record GateRun(
        string Mode,
        string Status,
        int DifferenceCount,
        int ComparatorExitCode,
        string[] Differences,
        string[] ContractSide,
        string[] ImplementationSide);

    private static string CapacityMember() => "CreateExcel" + "File";

    private static GateRun RunCurrentGate()
    {
        var outputDirectory = Path.Combine(Path.GetTempPath(), "cooperator-harness-removals-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        var result = Shell.Run(
            Shell.PowerShell7,
            Repo.TreeRoot,
            [
                "-NoProfile",
                "-ExecutionPolicy", "Bypass",
                "-File", Repo.InTree(GateRelativePath.Split('\\')),
                "-Mode", "Current",
                "-OutputDirectory", outputDirectory,
            ]);

        // Si el host no arrancó, el caso debe fallar de forma explícita y no dar un
        // verde falso por un gate que nunca corrió.
        Assert.DoesNotContain("is not recognized", result.AllText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CommandNotFoundException", result.AllText, StringComparison.Ordinal);

        var reportPath = Path.Combine(outputDirectory, "api-compat-report.json");
        Assert.True(File.Exists(reportPath), $"El gate no dejó reporte JSON en {reportPath}:{Environment.NewLine}{result.AllText}");

        using var report = JsonDocument.Parse(File.ReadAllText(reportPath));
        var root = report.RootElement;

        var status = root.GetProperty("status").GetString() ?? string.Empty;
        Assert.Equal("Current", root.GetProperty("mode").GetString());

        // Salida esperada de la corrida informativa: 2 («diferencias») mientras el
        // porteo sea parcial; 0 sólo si no hubiera ninguna diferencia. Nunca 3 (bloqueo).
        Assert.Contains(result.ExitCode, new[] { 0, 2 });

        return new GateRun(
            root.GetProperty("mode").GetString() ?? string.Empty,
            status,
            root.GetProperty("difference_count").GetInt32(),
            root.GetProperty("comparator_exit_code").GetInt32(),
            [.. root.GetProperty("differences").EnumerateArray().Select(item => item.GetString() ?? string.Empty)],
            [.. (root.GetProperty("contract_side").GetString() ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
            [.. (root.GetProperty("implementation_side").GetString() ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)]);
    }

    /// <summary>Lee la constancia completa: TODOS los fragmentos de <c>2.0/docs/api-removals/</c>.</summary>
    private static IReadOnlyList<FragmentEntry> Fragments()
    {
        var directory = Repo.InTree("docs", "api-removals");
        Assert.True(Directory.Exists(directory), $"Falta el directorio de constancias de rupturas: {directory}.");
        Assert.True(File.Exists(Path.Combine(directory, FragmentName)), $"Falta la constancia de este Bolt: {FragmentName}.");

        var entries = new List<FragmentEntry>();
        foreach (var file in Directory.GetFiles(directory, "*.json"))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file));
            foreach (var element in document.RootElement.GetProperty("entries").EnumerateArray())
            {
                entries.Add(new FragmentEntry(
                    Path.GetFileName(file),
                    Text(element, "id"),
                    Text(element, "kind"),
                    Text(element, "type"),
                    Text(element, "member"),
                    Text(element, "signature"),
                    Text(element, "what"),
                    Text(element, "why"),
                    Text(element, "replacement"),
                    Text(element, "evidence"),
                    Text(element, "owner_bolt")));
            }
        }

        Assert.NotEmpty(entries);
        return entries;

        static string Text(JsonElement element, string property) =>
            element.TryGetProperty(property, out var value) ? value.GetString() ?? string.Empty : string.Empty;
    }

    /// <summary>
    /// Nombres de miembro de cada diferencia de miembro retirado: del texto del
    /// comparador (<c>Member '&lt;firma&gt;'</c>) se toma el nombre que sigue al último
    /// punto de la firma.
    /// </summary>
    private static string[] RemovedMembers(IEnumerable<string> differences) =>
    [
        .. differences
            .Where(difference => difference.StartsWith(RemovedMemberCode, StringComparison.Ordinal))
            .Select(MemberName)
            .Where(name => name.Length > 0),
    ];

    private static string MemberName(string difference)
    {
        const string marker = "Member '";
        var start = difference.IndexOf(marker, StringComparison.Ordinal);
        if (start < 0)
        {
            return string.Empty;
        }

        start += marker.Length;
        var end = difference.IndexOf('\'', start);
        if (end < 0)
        {
            return string.Empty;
        }

        var signature = difference[start..end];
        var parenthesis = signature.IndexOf('(', StringComparison.Ordinal);
        var withoutParameters = parenthesis < 0 ? signature : signature[..parenthesis];
        var separator = withoutParameters.LastIndexOf('.');
        return separator < 0 ? withoutParameters : withoutParameters[(separator + 1)..];
    }

    /// <summary>
    /// Diferencias de miembro retirado que <b>ninguna</b> constancia explica: la
    /// cobertura exige nombrar el miembro <b>y</b> declarar su reemplazo (ADR-008 §3.1).
    /// </summary>
    private static string[] UncoveredRemovedMembers(IEnumerable<string> differences, IReadOnlyList<FragmentEntry> entries) =>
    [
        .. differences
            .Where(difference => difference.StartsWith(RemovedMemberCode, StringComparison.Ordinal))
            .Where(difference => !entries.Any(entry =>
                entry.Text.Contains(MemberName(difference), StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(entry.Replacement))),
    ];
}
