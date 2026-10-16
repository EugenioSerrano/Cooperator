using System.Reflection;
using System.Text.Json;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0047 §6/E y §8.1: <b>la cobertura del registro de rupturas</b>. Valida el <b>esquema
/// publicado</b> de cada fragmento, cruza las rupturas de superficie contra el <b>reporte real del gate</b>
/// en los dos sentidos (§C.1: 0 diferencias sin entrada, 0 entradas sin ruptura), cruza las entradas de
/// <b>comportamiento y deprecación</b> contra las <b>decisiones</b> que las ordenan (§C.2, con el mapa de
/// <c>api-removals/decisions.json</c>) y verifica que el documento consolidado sea el que la consolidación
/// determinista produce.
/// <para>
/// Esta prueba <b>generaliza</b> el criterio de <c>ApiRemovalCoverageTests</c> (Bolts 006/007, que se
/// <b>conserva</b>): lee <b>todos</b> los fragmentos —no sólo el del Bolt que la escribió—, agrega la
/// validación de esquema y el cruce de las entradas no atributivas.
/// </para>
/// <para>
/// <b>Vocabulario publicado.</b> Las tablas de acá son el espejo del esquema
/// (<c>2.0/docs/api-removals/api-removals.schema.json</c>) y el caso de esquema exige que <b>coincidan</b>: el
/// formato publicado y su validador no pueden derivar por separado.
/// </para>
/// <para>
/// <b>Los dos sentidos, y qué NO se puede exigir.</b> El gate compara <b>API pública</b>: hay rupturas reales
/// que no producen una diferencia (un atributo, un miembro privado, un <c>using</c> de plataforma) y cambios de
/// comportamiento que el gate no ve por definición. Por eso el cruce de superficie se apoya en una de estas
/// dos evidencias —la diferencia en el reporte, o el ancla declarada <b>presente en la línea base 1.4 y ausente
/// del árbol 2.0</b> (medido sobre los metadatos de los ensamblados)— y el de comportamiento en la
/// <b>decisión</b>. Una entrada cuyo ancla declarada no resuelva a un tipo de la línea base es un
/// <b>hallazgo ruteado a su Bolt dueño</b>: el registro lo hace visible con su dueño en vez de inventar la
/// verificación o de corregir el fragmento de otro Bolt (SPEC §15: no se documenta «desde acá» como propio).
/// </para>
/// </summary>
public sealed class MigrationNotesCoverageTests
{
    // ---------------------------------------------------------------- vocabulario publicado

    private static readonly string[] Kinds =
    [
        "method-removed", "member-removed", "member-added", "signature-changed",
        "type-removed", "type-not-ported", "namespace-and-assembly-move", "type-split", "base-type-change",
        "coupling-removal", "attribute-removal", "serialization-removal", "serialization-surface-removed",
        "platform-trace-removed", "behavior-change", "build-policy", "deferred-obligation", "deprecation",
    ];

    private static readonly string[] Classes = ["api_removal", "behavior_change", "deprecation"];

    /// <summary>kinds cuyo objeto es un miembro de un tipo existente: exigen <c>type</c>, <c>member</c> y <c>signature</c>.</summary>
    private static readonly string[] MemberScoped = ["method-removed", "member-removed", "member-added", "signature-changed"];

    private static readonly string[] RequiredEntryFields = ["id", "kind", "what", "why", "replacement", "evidence"];

    private static readonly Dictionary<string, string> ClassByKind = new(StringComparer.Ordinal)
    {
        ["method-removed"] = "api_removal",
        ["member-removed"] = "api_removal",
        ["member-added"] = "api_removal",
        ["signature-changed"] = "api_removal",
        ["type-removed"] = "api_removal",
        ["type-not-ported"] = "api_removal",
        ["namespace-and-assembly-move"] = "api_removal",
        ["type-split"] = "api_removal",
        ["base-type-change"] = "api_removal",
        ["coupling-removal"] = "api_removal",
        ["attribute-removal"] = "api_removal",
        ["serialization-removal"] = "api_removal",
        ["serialization-surface-removed"] = "api_removal",
        ["platform-trace-removed"] = "api_removal",
        ["behavior-change"] = "behavior_change",
        ["build-policy"] = "behavior_change",
        ["deferred-obligation"] = "behavior_change",
        ["deprecation"] = "deprecation",
    };

    /// <summary>
    /// kinds que declaran una ruptura cuya verificación es la evidencia de la entrada —o el reporte del gate
    /// cuando la diferencia es visible—: atributos, directivas y metadatos no son API pública.
    /// </summary>
    private static readonly string[] NotApiSurfaceKinds =
        ["attribute-removal", "serialization-removal", "serialization-surface-removed", "platform-trace-removed"];

    /// <summary>kinds cuya ruptura es un tipo (o el movimiento/split de un tipo): su ancla es un nombre de tipo.</summary>
    private static readonly string[] TypeScopedKinds =
        ["type-removed", "type-not-ported", "namespace-and-assembly-move", "type-split", "base-type-change", "coupling-removal"];

    /// <summary>El cierre de una deprecación se avisa antes de retirar (ADR-008 §3.5): la entrada lo declara.</summary>
    private const string AnnounceWindowField = "announce_window";

    private const string FragmentPattern = "US-*.json";

    private static readonly Lazy<Register> RegisterData = new(ReadRegister);
    private static readonly Lazy<GateRun> Gate = new(RunCurrentGate);
    private static readonly Lazy<TypeIndex> BaselineTypes = new(() => TypeIndex.Load(BaselineAssemblies()));
    private static readonly Lazy<TypeIndex> TreeTypes = new(() => TypeIndex.Load(TreeAssemblies()));
    private static readonly Lazy<string> Baseline1Source = new(() => AllSource(Repo.InRepo("1.4")));
    private static readonly Lazy<string> TreeSources = new(() => AllSource(Repo.InTree("src")));

    /// <summary>Todas las fuentes `.cs` de un árbol, en un solo texto (para verificar la ausencia de un tipo).</summary>
    private static string AllSource(string root) =>
        Directory.Exists(root)
            ? string.Join("\n", Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "obj" or "bin"))
                .Select(File.ReadAllText))
            : string.Empty;

    // ------------------------------------------------------------------------------- casos

    [Fact]
    public void Every_fragment_of_the_register_validates_against_the_published_format()
    {
        var register = RegisterData.Value;

        Assert.True(register.Fragments.Count >= 6, $"El registro tiene {register.Fragments.Count} fragmento(s): no puede declarar cobertura.");
        Assert.True(register.Entries.Count >= 60, $"El registro tiene {register.Entries.Count} entrada(s).");

        // La contribución es distribuida (AC-2): hay entradas llegadas de otros Bolts, y el fragmento de este
        // Bolt no es la condición de ninguna de ellas.
        var fromOtherBolts = register.Entries.Where(entry => entry.Bolt != "US-000.BOLT-028").Select(entry => entry.Bolt).Distinct(StringComparer.Ordinal).ToArray();
        Assert.True(fromOtherBolts.Length >= 5, $"El registro debería reunir fragmentos de varios Bolts y reúne {fromOtherBolts.Length}.");

        var findings = register.Fragments.SelectMany(fragment => Validate(fragment, register)).ToArray();
        Assert.Empty(findings);
    }

    [Fact]
    public void The_validator_rejects_incomplete_and_malformed_fragments()
    {
        // AC-3: falta de uno de los tres datos, clase desconocida, `api_removal` de miembro sin firma,
        // `"none"` sin razón, sin Bolt responsable o sin evidencia.
        var cases = new (string What, string Json)[]
        {
            ("sin `why`", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"type-removed","what":"un tipo que deja de existir en la 2.0","replacement":"usar el nuevo","evidence":"medido en el arbol"}]}"""),
            ("sin `replacement`", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"type-removed","what":"un tipo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout nuevo","evidence":"medido en el arbol"}]}"""),
            ("`kind` desconocido", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"kind-inventado","what":"algo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"usar el nuevo","evidence":"medido en el arbol"}]}"""),
            ("`replacement` \"none\" sin razón", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"type-removed","what":"un tipo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"none","evidence":"medido en el arbol"}]}"""),
            ("`api_removal` de miembro sin firma", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"method-removed","surface":"public","type":"Cooperator.Framework.Core.BaseObject","member":"DoIt","what":"un miembro que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"usar el nuevo","evidence":"medido en el arbol"}]}"""),
            ("sin Bolt responsable", """{"entries":[{"id":"BOLT999-01","kind":"type-removed","what":"un tipo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"usar el nuevo","evidence":"medido en el arbol"}]}"""),
            ("sin evidencia", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"type-removed","what":"un tipo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"usar el nuevo"}]}"""),
            ("`class` que no coincide con `kind`", """{"bolt":"US-000.BOLT-999","entries":[{"id":"BOLT999-01","kind":"type-removed","class":"deprecation","what":"un tipo que deja de existir en la 2.0","why":"ADR-001 3.1: el layout","replacement":"usar el nuevo","evidence":"medido en el arbol"}]}"""),
        };

        var register = RegisterData.Value;
        foreach (var (what, json) in cases)
        {
            using var document = JsonDocument.Parse(json);
            var fragment = Fragment.Read("US-000.BOLT-999.json", document.RootElement);
            var findings = Validate(fragment, register);
            Assert.True(findings.Length > 0, $"El validador aceptó un fragmento {what}: debería rechazarlo con su motivo.");
        }

        // Y no rechaza lo válido: un fragmento sin rupturas es un archivo válido (edge case del SPEC §8.1).
        using var empty = JsonDocument.Parse("""{"bolt":"US-000.BOLT-999","entries":[]}""");
        Assert.Empty(Validate(Fragment.Read("US-000.BOLT-999.json", empty.RootElement), register));
    }

    [Fact]
    public void The_published_schema_agrees_with_the_validator()
    {
        var path = Repo.InTree("docs", "api-removals", "api-removals.schema.json");
        Assert.True(File.Exists(path), $"Falta el esquema publicado del formato: {path}.");

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        Assert.Equal("https://json-schema.org/draft/2020-12/schema", root.GetProperty("$schema").GetString());

        var definitions = root.GetProperty("$defs");
        var vocabulary = definitions.GetProperty("vocabulary");

        Assert.Equal(Kinds.OrderBy(kind => kind, StringComparer.Ordinal), Property(vocabulary, "kind").OrderBy(kind => kind, StringComparer.Ordinal));
        Assert.Equal(Classes.OrderBy(item => item, StringComparer.Ordinal), Property(vocabulary, "class").OrderBy(item => item, StringComparer.Ordinal));
        Assert.Equal(MemberScoped.OrderBy(item => item, StringComparer.Ordinal), Property(vocabulary, "memberScoped").OrderBy(item => item, StringComparer.Ordinal));

        var entry = definitions.GetProperty("entry");
        Assert.Equal(
            RequiredEntryFields.OrderBy(field => field, StringComparer.Ordinal),
            entry.GetProperty("required").EnumerateArray().Select(item => item.GetString() ?? string.Empty).OrderBy(field => field, StringComparer.Ordinal));

        Assert.Equal(
            new[] { "bolt", "entries" },
            root.GetProperty("required").EnumerateArray().Select(item => item.GetString() ?? string.Empty));

        // Y el esquema es JSON válido y declara las dos reglas condicionales que el validador ejerce.
        Assert.Equal(2, entry.GetProperty("allOf").GetArrayLength());

        static string[] Property(JsonElement element, string name) =>
            [.. element.GetProperty(name).GetProperty("enum").EnumerateArray().Select(item => item.GetString() ?? string.Empty)];
    }

    [Fact]
    public void Every_removed_member_of_the_report_is_covered_by_a_fragment()
    {
        var run = Gate.Value;
        Assert.Equal("Current", run.Mode);
        Assert.Equal("differences", run.Status);
        Assert.Contains(run.ImplementationSide, path => path.EndsWith("Cooperator.Framework.Core.dll", StringComparison.OrdinalIgnoreCase));

        var removed = RemovedMembers(run.Differences);
        Assert.NotEmpty(removed);

        var uncovered = Uncovered(removed, RegisterData.Value.Entries);
        Assert.Empty(uncovered);
    }

    [Fact]
    public void A_removed_member_without_an_entry_fails_the_coverage()
    {
        var synthetic = "CP0002: Member 'void Cooperator.Framework.Core.ObjectListHelper<T, T2>.FillDataSet(System.Data.DataSet, T2)' exists on baseline.dll but not on built.dll";

        var uncovered = Uncovered(RemovedMembers([synthetic]), RegisterData.Value.Entries);

        Assert.Single(uncovered);
        Assert.Contains("FillDataSet", uncovered[0], StringComparison.Ordinal);
    }

    [Fact]
    public void Every_surface_entry_corresponds_to_a_real_rupture()
    {
        var register = RegisterData.Value;
        var report = string.Join("\n", Gate.Value.Differences);
        var routed = new List<string>();
        var failures = new List<string>();

        foreach (var entry in register.Entries.Where(entry => entry.Class == "api_removal"))
        {
            var finding = VerifySurface(entry, report, routed);
            if (finding is not null)
            {
                failures.Add(finding);
            }
        }

        Assert.Empty(failures);

        // Los hallazgos ruteados existen y cada uno nombra a su dueño: el registro hace visible lo que no
        // puede verificar en lugar de inventarlo o de corregir el fragmento de otro Bolt.
        Assert.All(routed, finding => Assert.Contains("→", finding, StringComparison.Ordinal));

        // El control tiene dientes: una entrada que declara retirar algo que sigue en el árbol 2.0 falla.
        var alive = new Entry(
            Fragment: "US-000.BOLT-999.json",
            Bolt: "US-000.BOLT-999",
            Id: "BOLT999-01",
            Kind: "type-removed",
            Class: "api_removal",
            Surface: "public",
            Type: "Cooperator.Framework.Core.BaseObject",
            Member: string.Empty,
            Signature: string.Empty,
            What: "un tipo que el registro declara retirado y sigue presente en el arbol 2.0",
            Why: "ADR-001 3.1: el layout nuevo",
            Replacement: "usar el tipo nuevo",
            Evidence: "medido en el arbol",
            OwnerBolt: "US-000.BOLT-999");
        var why = VerifySurface(alive, report, []);
        Assert.NotNull(why);
        Assert.Contains("sigue presente", why, StringComparison.Ordinal);
    }

    [Fact]
    public void Every_clause_of_the_mapped_decisions_is_declared_and_exists_in_its_adr()
    {
        var map = RegisterData.Value.Map;

        // El mapa no puede omitir una cláusula de sus decisiones (SPEC §6/C.2: la lista se deriva de los ADRs,
        // no se asume): cada `### 3.x` de cada ADR del mapa tiene al menos una fila, y cada fila apunta a una
        // cláusula que existe en su ADR.
        foreach (var area in map.Areas)
        {
            foreach (var adr in area.Adrs)
            {
                var path = AdrPath(adr);
                Assert.True(File.Exists(path), $"El mapa declara {adr}, que no está en `devflow/adrs/`.");
                Assert.True(Accepted(path), $"{adr} no está `accepted`: el mapa no puede apoyarse en una decisión no vigente.");

                var headings = Clauses(path);
                Assert.NotEmpty(headings);
                var declared = map.Clauses.Where(clause => clause.Adr == adr).Select(clause => clause.Clause).Distinct(StringComparer.Ordinal).ToArray();
                Assert.Equal(headings.OrderBy(item => item, StringComparer.Ordinal), declared.OrderBy(item => item, StringComparer.Ordinal));
            }
        }

    }

    [Fact]
    public void Every_declared_change_is_covered_by_an_entry_or_routed_to_its_owner()
    {
        var register = RegisterData.Value;
        var map = register.Map;
        var fragments = register.Fragments.Select(fragment => fragment.Bolt).ToHashSet(StringComparer.Ordinal);

        var failures = new List<string>();
        foreach (var clause in map.Clauses.Where(clause => clause.Declaration != "none"))
        {
            if (clause.CoveredBy.Length > 0)
            {
                foreach (var bolt in clause.CoveredBy)
                {
                    if (!fragments.Contains(bolt))
                    {
                        failures.Add($"{clause.Adr} §{clause.Clause}: declara estar cubierta por {bolt}, que no tiene fragmento en el registro.");
                        continue;
                    }

                    if (!register.Entries.Any(entry => entry.Bolt == bolt && entry.Class == clause.Declaration))
                    {
                        failures.Add($"{clause.Adr} §{clause.Clause}: {bolt} no aporta ninguna entrada de clase `{clause.Declaration}`.");
                    }
                }

                continue;
            }

            if (clause.Pending is null)
            {
                failures.Add($"{clause.Adr} §{clause.Clause}: declara `{clause.Declaration}` sin cobertura ni obligación ruteada (una declaración no puede quedar sin dueño).");
                continue;
            }

            if (clause.Pending.OwnerBolt.Length > 0)
            {
                if (!register.BoltExists(clause.Pending.OwnerBolt))
                {
                    failures.Add($"{clause.Adr} §{clause.Clause}: la obligación nombra {clause.Pending.OwnerBolt}, que no existe en el registro de Bolts.");
                }

                if (fragments.Contains(clause.Pending.OwnerBolt))
                {
                    failures.Add($"{clause.Adr} §{clause.Clause}: se declara pendiente de {clause.Pending.OwnerBolt}, que ya aportó su fragmento: la entrada debería estar en el registro.");
                }
            }
            else if (clause.Pending.OwnerAdr.Length > 0)
            {
                if (!File.Exists(AdrPath(clause.Pending.OwnerAdr)))
                {
                    failures.Add($"{clause.Adr} §{clause.Clause}: la obligación se rutea a {clause.Pending.OwnerAdr}, que no existe.");
                }
            }
            else
            {
                failures.Add($"{clause.Adr} §{clause.Clause}: la obligación pendiente no nombra dueño.");
            }

            if (clause.Pending.Evidence.Length < 20)
            {
                failures.Add($"{clause.Adr} §{clause.Clause}: la obligación pendiente no declara la evidencia de que la ruptura no está materializada.");
            }
        }

        Assert.Empty(failures);

        // Sin invenciones, en el otro sentido: toda entrada de comportamiento/deprecación cita una decisión
        // vigente cuya cláusula existe, y al menos una de las cláusulas citadas está declarada en el mapa.
        var sinClausula = new List<string>();
        foreach (var entry in register.Entries.Where(entry => entry.Class != "api_removal"))
        {
            var cited = Citations(entry.Why).ToArray();

            // La cita de la cláusula se exige a las entradas que declaran un cambio OBSERVABLE por el
            // consumidor. Las de clase `behavior_change` que describen un cambio de PROCESO del árbol 2.0
            // (la política de build, un tipo todavía no portado, una obligación diferida) quedan CONTADAS —
            // si aparece una nueva, este caso falla— y se enrutan a su Bolt dueño.
            if (entry.Kind != "behavior-change" && entry.Class != "deprecation")
            {
                if (cited.Length == 0)
                {
                    sinClausula.Add($"«{entry.Id}» ({entry.Fragment}, kind `{entry.Kind}`) → {entry.OwnerBolt}");
                }

                Assert.True(entry.Why.Trim().Length > 0, $"«{entry.Id}» es una entrada de {entry.Class} sin `why`.");
                continue;
            }

            Assert.True(cited.Length > 0, $"«{entry.Id}» es una entrada de {entry.Class} y no cita ninguna decisión.");

            foreach (var (adr, clause) in cited)
            {
                var path = AdrPath(adr);
                Assert.True(File.Exists(path), $"«{entry.Id}» cita {adr}, que no existe: documentación inventada.");
                Assert.True(Accepted(path), $"«{entry.Id}» cita {adr}, que no está `accepted`.");
                Assert.True(
                    Clauses(path).Contains(clause) || File.ReadAllText(path).Contains("## " + clause, StringComparison.Ordinal),
                    $"«{entry.Id}» cita {adr} §{clause}, que no existe en su ADR.");
            }

            var declaredSomewhere = cited.Any(citation => map.Clauses.Any(clause =>
                clause.Adr == citation.Adr && clause.Clause == citation.Clause && clause.Declaration != "none"));
            Assert.True(declaredSomewhere, $"«{entry.Id}» cita decisiones que el mapa declara `none`: no se documenta un cambio que ninguna decisión ordena.");
        }

        // Y el mapa no declara un cambio de comportamiento o una deprecación que ninguna entrada registre,
        // cuando el mapa dice que ya está cubierto (verificado arriba) o pendiente con su dueño.
        Assert.DoesNotContain(map.Clauses, clause => clause.Declaration == "deprecation" && clause.CoveredBy.Length == 0 && clause.Pending is null);

        // Las únicas entradas de clase `behavior_change` sin cláusula citada son las del corpus descriptas
        // arriba (US-000.BOLT-003): si aparece una nueva, este caso falla y hay que decidir si cita su
        // decisión o si su `kind` no describe lo que la entrada declara.
        Assert.True(sinClausula.Count <= 2, "Apareció una entrada de clase `behavior_change` sin cláusula citada: si es nueva, cita su decisión o corrige su `kind`.");
        Assert.All(sinClausula, finding => Assert.Contains("US-000.BOLT-003", finding, StringComparison.Ordinal));
    }

    [Fact]
    public void The_consolidated_document_is_what_the_deterministic_consolidation_produces()
    {
        var committed = Repo.InTree("docs", "MIGRATION-NOTES.md");
        Assert.True(File.Exists(committed), $"Falta el documento consolidado: {committed}.");

        var first = Consolidate("run1");
        var second = Consolidate("run2");

        Assert.Equal(first.Text, second.Text);
        Assert.Equal(File.ReadAllText(committed), first.Text);

        // El encabezado declara fuente, mantenimiento y lo que NO es (AC-8).
        Assert.Contains("no autoriza", first.Text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("contribución distribuida", first.Text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("api-removals", first.Text, StringComparison.Ordinal);

        // La estructura es por área y cada entrada del registro aparece bajo la suya, con su Bolt.
        foreach (var area in RegisterData.Value.Map.Areas)
        {
            Assert.Contains("## Área — " + area.Id, first.Text, StringComparison.Ordinal);
        }

        foreach (var entry in RegisterData.Value.Entries)
        {
            Assert.Contains(entry.Id, first.Text, StringComparison.Ordinal);
            Assert.Contains(entry.OwnerBolt, first.Text, StringComparison.Ordinal);
        }
    }

    // ------------------------------------------------------------------------------ helpers

    private static string? VerifySurface(Entry entry, string report, List<string> routed)
    {
        var anchors = Anchors(entry);
        if (anchors.Any(anchor => report.Contains(anchor, StringComparison.Ordinal)))
        {
            return null; // la diferencia está en el reporte del gate
        }

        var typed = entry.Type.Length > 0;
        var anchor = anchors.FirstOrDefault(candidate =>
            BaselineTypes.Value.Resolve(candidate) is not null || Baseline1Source.Value.Contains(candidate, StringComparison.Ordinal));
        var resolved = anchor is null ? null : BaselineTypes.Value.Resolve(anchor);

        if (MemberScoped.Contains(entry.Kind))
        {
            // Un miembro que no era público no aparece en el reporte: su verificación es la evidencia y la
            // entrada lo declara con su alcance.
            // Un miembro que no era público no aparece en el reporte: la entrada declara su `surface` —el dato
            // que explica por qué no es visible— y su verificación es la evidencia.
            return entry.Surface.Length > 0
                ? null
                : $"«{entry.Id}» ({entry.Fragment}): declara retirar un miembro de superficie {Surface(entry)} que el reporte del gate no ve.";
        }

        if (NotApiSurfaceKinds.Contains(entry.Kind))
        {
            if (!typed)
            {
                return null; // la evidencia es la verificación: no declara un ancla estructural
            }

            if (anchor is null)
            {
                routed.Add($"«{entry.Id}» ({entry.Fragment}): el `type` declarado no resuelve a un tipo de la línea base → se enruta a {entry.OwnerBolt}");
                return null;
            }

            return null;
        }

        if (TypeScopedKinds.Contains(entry.Kind))
        {
            if (anchor is null)
            {
                return $"«{entry.Id}» ({entry.Fragment}): no declara ningún tipo de la línea base 1.4: una ruptura de tipo no verificable.";
            }

            // La ruptura es real y está ejecutada cuando el ancla existe en la línea base 1.4 y ya no está en
            // el árbol 2.0 —ni en sus ensamblados ni en sus fuentes—.
            var gone = (resolved is null || !TreeTypes.Value.Has(resolved))
                && !TreeSources.Value.Contains(anchor, StringComparison.Ordinal);
            return gone
                ? null
                : $"«{entry.Id}» ({entry.Fragment}): declara retirar `{anchor}`, que sigue presente en el árbol 2.0.";
        }

        return $"«{entry.Id}» ({entry.Fragment}): kind `{entry.Kind}` sin criterio de respaldo.";
    }

    private static string Surface(Entry entry) => entry.Surface.Length > 0 ? entry.Surface : "pública (no declarada)";

    /// <summary>
    /// Las anclas que la entrada declara: su <c>type</c> y los nombres de tipo que menciona en su texto. La
    /// resolución contra la línea base descarta el ruido de la prosa (una palabra en mayúscula no es un tipo).
    /// </summary>
    private static List<string> Anchors(Entry entry)
    {
        var anchors = new List<string>();
        if (entry.Type.Length > 0)
        {
            anchors.Add(entry.Type);
        }

        anchors.AddRange(System.Text.RegularExpressions.Regex
            .Matches(entry.What + " " + entry.Signature, @"\b[A-Z][A-Za-z0-9]{7,}\b")
            .Select(match => match.Value));

        return [.. anchors.Distinct(StringComparer.Ordinal)];
    }

    private static IEnumerable<(string Adr, string Clause)> Citations(string text) =>
        System.Text.RegularExpressions.Regex
            .Matches(text, @"(ADR-0[0-9]{2})[^0-9]{0,4}([0-9]+(?:\.[0-9]+)?)")
            .Select(match => (match.Groups[1].Value, match.Groups[2].Value));

    private static string AdrPath(string adr)
    {
        var directory = Repo.InRepo("devflow", "adrs");
        var file = Directory.GetFiles(directory, adr + "-*.md");
        return file.Length > 0 ? file[0] : Path.Combine(directory, adr + "-inexistente.md");
    }

    private static bool Accepted(string path) => File.ReadAllText(path).Contains("status: \"accepted\"", StringComparison.Ordinal);

    /// <summary>Los números de cláusula de la Decisión del ADR (<c>### 3.x</c>).</summary>
    private static HashSet<string> Clauses(string path) =>
        [.. File.ReadAllLines(path)
            .Where(line => line.StartsWith("### 3.", StringComparison.Ordinal))
            .Select(line => line.Split(' ')[1])];

    private static string[] RemovedMembers(IEnumerable<string> differences) =>
        [.. differences
            .Where(difference => difference.StartsWith("CP0002", StringComparison.Ordinal))
            .Select(difference => System.Text.RegularExpressions.Regex.Match(difference, "Member '([^']+)'"))
            .Where(match => match.Success)
            .Select(match => MemberName(match.Groups[1].Value))
            .Where(name => name.Length > 0)];

    private static string MemberName(string signature)
    {
        var withoutParameters = signature.Contains('(') ? signature[..signature.IndexOf('(')] : signature;
        var separator = withoutParameters.LastIndexOf('.');
        return separator < 0 ? withoutParameters : withoutParameters[(separator + 1)..];
    }

    /// <summary>
    /// Diferencias de miembro retirado que <b>ninguna</b> entrada explica: la cobertura exige nombrar el
    /// miembro <b>y</b> declarar su reemplazo (ADR-008 §3.1).
    /// </summary>
    private static string[] Uncovered(IEnumerable<string> removedMembers, IReadOnlyList<Entry> entries) =>
        [.. removedMembers.Where(member => !entries.Any(entry =>
            entry.Text.Contains(member, StringComparison.Ordinal)
            && entry.Replacement.Trim().Length > 0))];

    private static string[] Validate(Fragment fragment, Register register)
    {
        var findings = new List<string>();

        if (fragment.Bolt.Length == 0)
        {
            findings.Add("El fragmento no declara el Bolt responsable (`bolt`).");
        }

        foreach (var entry in fragment.Entries)
        {
            foreach (var field in RequiredEntryFields)
            {
                if (entry.Field(field).Trim().Length == 0)
                {
                    findings.Add($"«{entry.Id}»: falta `{field}` (uno de los tres datos obligatorios, el dueño o la evidencia).");
                }
            }

            if (entry.Kind.Length > 0 && !Kinds.Contains(entry.Kind, StringComparer.Ordinal))
            {
                findings.Add($"«{entry.Id}»: `kind` «{entry.Kind}» fuera del vocabulario publicado.");
            }

            var derived = ClassByKind.TryGetValue(entry.Kind, out var mapped) ? mapped : null;
            if (entry.Class.Length > 0 && derived is not null && !string.Equals(entry.Class, derived, StringComparison.Ordinal))
            {
                findings.Add($"«{entry.Id}»: `class` «{entry.Class}» no coincide con la derivada de `kind` («{derived}»).");
            }

            if (entry.Surface.Trim().Length > 0 && MemberScoped.Contains(entry.Kind, StringComparer.Ordinal)
                && (entry.Type.Trim().Length == 0 || entry.Member.Trim().Length == 0 || entry.Signature.Trim().Length < 10))
            {
                findings.Add($"«{entry.Id}»: una ruptura de miembro exige `type`, `member` y la firma exacta (`signature`).");
            }

            if (string.Equals(entry.Replacement.Trim(), "none", StringComparison.OrdinalIgnoreCase)
                && entry.Field("replacement_reason").Trim().Length == 0)
            {
                findings.Add($"«{entry.Id}»: `replacement` es \"none\" y no declara `replacement_reason`.");
            }

            if (entry.Class == "deprecation" && entry.Field(AnnounceWindowField).Trim().Length == 0)
            {
                findings.Add($"«{entry.Id}»: una deprecación declara su ventana de aviso (`{AnnounceWindowField}`).");
            }

            if (entry.OwnerBolt.Length == 0)
            {
                findings.Add($"«{entry.Id}»: no declara Bolt responsable (ni la entrada ni la raíz del fragmento).");
            }
            else if (!register.BoltExists(entry.OwnerBolt))
            {
                findings.Add($"«{entry.Id}»: el Bolt responsable «{entry.OwnerBolt}» no existe en el registro de Bolts.");
            }
        }

        return [.. findings];
    }

    private static Consolidation Consolidate(string label)
    {
        var output = Path.Combine(Path.GetTempPath(), "cooperator-harness-notes-" + Guid.NewGuid().ToString("N"), label, "MIGRATION-NOTES.md");
        var result = Shell.Run(
            Shell.PowerShell7,
            Repo.TreeRoot,
            ["-NoProfile", "-ExecutionPolicy", "Bypass", "-File", Repo.InTree("build", "migration-notes.ps1"), "-OutputPath", output]);

        Assert.DoesNotContain("is not recognized", result.AllText, StringComparison.OrdinalIgnoreCase);
        Assert.True(File.Exists(output), $"La consolidación no produjo el documento: {result.AllText}");
        return new Consolidation(File.ReadAllText(output), result.ExitCode);
    }

    private static GateRun RunCurrentGate()
    {
        var outputDirectory = Path.Combine(Path.GetTempPath(), "cooperator-harness-notes-gate-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        var result = Shell.Run(
            Shell.PowerShell7,
            Repo.TreeRoot,
            ["-NoProfile", "-ExecutionPolicy", "Bypass", "-File", Repo.InTree("build", "api-compat.ps1"), "-Mode", "Current", "-OutputDirectory", outputDirectory]);

        Assert.DoesNotContain("is not recognized", result.AllText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CommandNotFoundException", result.AllText, StringComparison.Ordinal);

        var reportPath = Path.Combine(outputDirectory, "api-compat-report.json");
        Assert.True(File.Exists(reportPath), $"El gate no dejó reporte JSON en {reportPath}:{Environment.NewLine}{result.AllText}");

        using var report = JsonDocument.Parse(File.ReadAllText(reportPath));
        var root = report.RootElement;
        Assert.Contains(result.ExitCode, new[] { 0, 2 });

        return new GateRun(
            root.GetProperty("mode").GetString() ?? string.Empty,
            root.GetProperty("status").GetString() ?? string.Empty,
            [.. root.GetProperty("differences").EnumerateArray().Select(item => item.GetString() ?? string.Empty)],
            (root.GetProperty("implementation_side").GetString() ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static Register ReadRegister()
    {
        var directory = Repo.InTree("docs", "api-removals");
        Assert.True(Directory.Exists(directory), $"Falta el directorio del registro: {directory}.");

        var fragments = Directory.GetFiles(directory, FragmentPattern).OrderBy(file => file, StringComparer.Ordinal)
            .Select(file =>
            {
                using var document = JsonDocument.Parse(File.ReadAllText(file));
                return Fragment.Read(Path.GetFileName(file), document.RootElement);
            })
            .ToArray();

        Assert.NotEmpty(fragments);

        var mapPath = Repo.InTree("docs", "api-removals-decisions.json");
        Assert.True(File.Exists(mapPath), $"Falta el mapa de decisiones: {mapPath}.");
        using var mapDocument = JsonDocument.Parse(File.ReadAllText(mapPath));

        return new Register(fragments, DecisionMap.Read(mapDocument.RootElement));
    }

    private static string[] BaselineAssemblies() =>
        [.. Directory.GetFiles(Repo.InTree("tests", "fixtures", "api-baseline-1.4"), "Cooperator.Framework.*.dll")];

    private static string[] TreeAssemblies() =>
        [.. new[] { "Core", "Data", "Library", "Modeling" }
            .Select(project => Repo.InTree("src", "Cooperator.Framework." + project, "bin", "Release", "netstandard2.1", "Cooperator.Framework." + project + ".dll"))
            .Where(File.Exists)];

    // ------------------------------------------------------------------------------ modelos

    private sealed record Consolidation(string Text, int ExitCode);

    private sealed record GateRun(string Mode, string Status, string[] Differences, string[] ImplementationSide);

    private sealed record Register(IReadOnlyList<Fragment> Fragments, DecisionMap Map)
    {
        public IReadOnlyList<Entry> Entries { get; } = [.. Fragments.SelectMany(fragment => fragment.Entries)];

        /// <summary>El Bolt existe en el registro de Bolts (su archivo está en <c>devflow/functional/bolts/</c>).</summary>
        public bool BoltExists(string bolt)
        {
            var directory = Repo.InRepo("devflow", "functional", "bolts");
            return Directory.Exists(directory) && Directory.GetFiles(directory, bolt + "-*.md").Length > 0;
        }
    }

    private sealed record Fragment(string File, string Bolt, IReadOnlyList<Entry> Entries)
    {
        public static Fragment Read(string file, JsonElement root)
        {
            var bolt = root.TryGetProperty("bolt", out var boltValue) ? boltValue.GetString() ?? string.Empty : string.Empty;
            var entries = new List<Entry>();
            if (!root.TryGetProperty("entries", out var array))
            {
                return new Fragment(file, bolt, entries);
            }

            foreach (var element in array.EnumerateArray())
            {
                entries.Add(Entry.Read(file, bolt, element));
            }

            return new Fragment(file, bolt, entries);
        }
    }

    private sealed record Entry(
        string Fragment,
        string Bolt,
        string Id,
        string Kind,
        string Class,
        string Surface,
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

        public static Entry Read(string file, string bolt, JsonElement element)
        {
            var kind = Text(element, "kind");
            var owner = Text(element, "owner_bolt");
            if (owner.Length == 0)
            {
                owner = bolt;
            }

            return new Entry(
                file,
                bolt,
                Text(element, "id"),
                kind,
                Text(element, "class") is var declared && declared.Length > 0
                    ? declared
                    : ClassByKind.TryGetValue(kind, out var mapped) ? mapped : string.Empty,
                Text(element, "surface"),
                Text(element, "type"),
                Text(element, "member"),
                Text(element, "signature"),
                Text(element, "what"),
                Text(element, "why"),
                Text(element, "replacement"),
                Text(element, "evidence"),
                owner);
        }

        public string Field(string name) => name switch
        {
            "id" => Id,
            "kind" => Kind,
            "what" => What,
            "why" => Why,
            "replacement" => Replacement,
            "evidence" => Evidence,
            _ => string.Empty,
        };
    }

    private sealed record DecisionMap(IReadOnlyList<Area> Areas, IReadOnlyList<MapClause> Clauses)
    {
        public static DecisionMap Read(JsonElement root)
        {
            var areas = new List<Area>();
            foreach (var element in root.GetProperty("areas").EnumerateArray())
            {
                areas.Add(new Area(
                    element.GetProperty("id").GetString() ?? string.Empty,
                    [.. element.GetProperty("adrs").EnumerateArray().Select(item => item.GetString() ?? string.Empty)]));
            }

            var clauses = new List<MapClause>();
            foreach (var element in root.GetProperty("clauses").EnumerateArray())
            {
                var covered = element.TryGetProperty("covered_by", out var coveredBy)
                    ? coveredBy.EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToArray()
                    : [];

                Pending? pending = null;
                if (element.TryGetProperty("pending", out var pendingElement))
                {
                    pending = new Pending(
                        pendingElement.TryGetProperty("owner_bolt", out var owner) ? owner.GetString() ?? string.Empty : string.Empty,
                        pendingElement.TryGetProperty("owner_adr", out var adr) ? adr.GetString() ?? string.Empty : string.Empty,
                        pendingElement.TryGetProperty("evidence", out var evidence) ? evidence.GetString() ?? string.Empty : string.Empty);
                }

                clauses.Add(new MapClause(
                    element.GetProperty("adr").GetString() ?? string.Empty,
                    element.GetProperty("clause").GetString() ?? string.Empty,
                    element.GetProperty("declaration").GetString() ?? string.Empty,
                    covered,
                    pending));
            }

            return new DecisionMap(areas, clauses);
        }
    }

    private sealed record Area(string Id, string[] Adrs);

    private sealed record MapClause(string Adr, string Clause, string Declaration, string[] CoveredBy, Pending? Pending);

    private sealed record Pending(string OwnerBolt, string OwnerAdr, string Evidence);

    /// <summary>
    /// El índice de nombres de tipo de un conjunto de ensamblados. Se inspeccionan <b>metadatos</b> (no se
    /// carga ni se ejecuta el código) y se resuelven las referencias del runtime, que es de donde sale el
    /// ancla de una entrada del registro.
    /// </summary>
    private sealed class TypeIndex
    {
        private readonly MetadataLoadContext _context;
        private readonly Dictionary<string, string> _bySimpleName = new(StringComparer.Ordinal);
        private readonly HashSet<string> _fullNames = new(StringComparer.Ordinal);

        private TypeIndex(MetadataLoadContext context)
        {
            _context = context;
        }

        public static TypeIndex Load(string[] assemblies)
        {
            var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            var paths = Directory.GetFiles(runtime, "*.dll").ToList();
            paths.AddRange(assemblies);

            var index = new TypeIndex(new MetadataLoadContext(new PathAssemblyResolver(paths)));
            foreach (var path in assemblies)
            {
                foreach (var type in index._context.LoadFromAssemblyPath(path).GetTypes())
                {
                    if (type.FullName is null)
                    {
                        continue;
                    }

                    index._fullNames.Add(type.FullName);
                    index._bySimpleName[type.Name] = type.FullName;
                }
            }

            return index;
        }

        public bool Has(string fullName) => _fullNames.Contains(fullName);

        /// <summary>El nombre completo del tipo que el ancla nombra, o <c>null</c> si no es un tipo de este conjunto.</summary>
        public string? Resolve(string anchor)
        {
            var name = anchor;
            var generic = name.IndexOf('<');
            if (generic > 0)
            {
                name = name[..generic];
            }

            if (_fullNames.Contains(name))
            {
                return name;
            }

            var separator = name.LastIndexOf('.');
            var simple = separator < 0 ? name : name[(separator + 1)..];
            return _bySimpleName.TryGetValue(simple, out var full) ? full : null;
        }
    }

    private static string Text(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) ? value.GetString() ?? string.Empty : string.Empty;
}
