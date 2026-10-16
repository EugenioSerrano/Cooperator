---
id: "MEM-261016-0526"
title: "El reverse sin `BaseRule`: cierre de la obligación del tooling (SPEC rev.4) - V-Bounce N°2: retiro de los `PropertyEditors` verificado y declarado, `OBLIGATION-TOOLING-01` cerrada y suite verde"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-016" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0027" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 4 # revision of the canonical SPEC used by this V-Bounce (rev.4: el retiro ordenado de los PropertyEditors y el cierre de la obligación)
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-005", "ADR-001", "ADR-004"] # ADR-005 §3.6 (el reverse sin herencia, con el provider propio) y §3.1; ADR-001 §3.1/§3.4/§3.6 (el tooling, el presupuesto de dependencias y la fuente única del mapeo); ADR-004 §3.2/§3.3 (la cadena desde el contrato y el resolutor del consumidor)
manifest: "US-000.BOLT-016-reverse-sin-baserule.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T05:20:04-03:00" # When this version is submitted for review (§3.0) — el mismo acto que la entrada del manifiesto
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0). El ejecutor NO se auto-aprueba.
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T05:22:52-03:00"
  decided_at: "2026-10-16T05:22:52-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°2 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0027` revisión 4 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — corrida hermética VERDE reproducida: 24 proyectos, 0 advertencias, 135 arnés + 12 mapeo + 11 encriptor + 17 tooling + 46 data + 83 caracterización = 304 casos, exit 0. `migration-notes.ps1` → 78 entradas de 11 fragmentos («sin cambios, consolidación determinista»); 0 diferencias sin cobertura (harness 135/135 incluye los 2 tests de cobertura)."
    - "Compliant — constancia propia (`US-000.BOLT-016.json`): 4 entradas (2 `signature-changed` de `UpdateDomainTree`/`GetDomainTree` + 1 `behavior-change` de la cadena + 1 `type-removed` de los `PropertyEditors` con reemplazo cero en prosa), **VALIDADA** contra `api-removals.schema.json`."
    - "Compliant — obligación cerrada: `BOLT003-09` → `status: materialized-and-closed` + `materialized_by: US-000.BOLT-016`; `OBLIGATION-TOOLING-01` con `closed_by: US-000.BOLT-016` + `cross_references` actualizadas (fragmento 016 + `CooperatorModeler/DBReverseHelper`); sólo ese bloque tocado (el `what`/`evidence` originales conservados como historia)."
    - "ADJUDICACIÓN (§8.1) — ACEPTO: la declaración del reemplazo cero **en prosa** es fiel a la rev.4 (qué/por qué/reemplazo declarados); la contradicción esquema↔validador es real y medida (`entry.Field(name)` es un switch de 6 casos que no lee `replacement_reason` → `replacement: "none"` es insatisfacible). El arreglo del `Field()` (una línea: extender a `replacement_reason`) queda registrado como **obligación con dueño** (arnés/registro) — patrón conocido: no tocar el arnés sin autorización."
    - "ADJUDICACIÓN (§8.2) — cosmético: la ruta de `MIGRATION-PARTITION.md` que cita el SPEC §17 está desactualizada (la real es la del paquete `Modeling`); señalada para la próxima revisión del documento, sin impacto en el alcance ni en los entregables."
    - "Compliant — G11/toma (T06) + estado final `in-development` (correcto para un cierre exitoso con `test_integration: required`); DoD + MEM completo + manifiesto válido (2 entradas v_bounces); `1.4/**` intacto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0526 — El reverse sin `BaseRule`: cierre de la obligación del tooling (rev.4) — V-Bounce N°2

| Field | Value |
|-------|-------|
| **Bolt** | [US-000.BOLT-016](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) — `non-functional`, `refactor`, `medium`, `internal`, `test_integration: required` |
| **SPEC** | [SPEC-261016-0027](../spec/SPEC-261016-0027-reverse-sin-baserule.md), **revisión 4** (`approved` 2026-10-16T05:13:37-03:00) |
| **V-Bounce** | **2** — la pasada corta: **verificar, declarar y cerrar** (el N°1 hizo el aporte y midió el bloqueo) |
| **V-Bounce anterior** | [`MEM-261016-0506`](MEM-261016-0506-reverse-sin-baserule.md) — el aporte, los tres recordatorios y el bloqueo medido del cuarto destino |
| **Baseline** | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — sin cambios materiales (§8.5) |

---

## 1. Executive summary

El V-Bounce cierra lo que el N°1 dejó abierto **sin tocar una línea del componente**: la revisión 4 del SPEC
**ordena el retiro** de los `PropertyEditors/*` (la adjudicación (a) que el N°1 escaló) y este V-Bounce lo
**verifica**, lo **declara** en el registro y **cierra la obligación del Bolt 003**, con la suite **VERDE**.

1. **El retiro está efectivo y medido** (nada que retirar: el N°1 no dejó los editores en el árbol): **0** archivos
   `*Editor*`/`*Form*` (`.cs`/`.resx`) bajo `2.0/src` y `2.0/tests`, **0** menciones de `PropertyEditors` en el
   componente y **0** `UseWindowsForms`/`System.Windows.Forms`/`System.Design`/`System.Drawing` en sus fuentes y en
   su `.csproj`.
2. **La constancia propia del registro**: `2.0/docs/api-removals/US-000.BOLT-016.json` (4 entradas) — las **dos
   rupturas de firma** del helper (`UpdateDomainTree`/`GetDomainTree` ya no reciben la cadena), el **cambio de
   comportamiento** de la conexión (de mecanismo propio del tool a la configuración portable, con su `next_step`
   hacia el Bolt 025) y el **retiro de los editores con reemplazo cero** (qué, por qué, reemplazo y evidencia).
3. **La obligación `OBLIGATION-TOOLING-01` queda cerrada**: `status: "deferred" → "materialized-and-closed"`,
   `materialized_by` → **`US-000.BOLT-016`**, `closed_by` en el bloque de la obligación y las referencias cruzadas
   (el fragmento de este Bolt y el directorio del componente). El **cierre es 3 destinos materializados + retiro
   declarado del 4.º**, exactamente como la rev.4 lo ordena (§16/§17).
4. **El cruce de cobertura**: el consolidado se regeneró (**78 entradas de 11 fragmentos**) y el gate corre
   (189 diferencias, 40 de ellas `CP0002`) con **0 diferencias sin cobertura** — los dos casos de cobertura del
   arnés pasan, y el `PropertyEditors` no aporta `CP0002` porque el tipo nunca existió en un ensamblado de la 2.0.
5. **La suite hermética**: **304 casos verdes** (135 arnés · 12 mapeo · 11 encriptor · 17 tooling · 46 datos ·
   83 caracterización), 24 proyectos, **0 advertencias** (tratadas como error), `EXIT=0`.

**Dos hallazgos medidos** que quedan declarados y escalados (§8.1 y §8.2), y el estado final del Bolt:
**`in-development`** — la aprobación de este MEM lo lleva a su **ciclo de test de integración** (el Bolt es
`test_integration: required`; la lane del reverse está escrita y compilada, §9.4).

---

## 2. Implemented phases

| Fase de §6 | Estado | Evidencia |
|---|---|---|
| **A — El retiro de la herencia** | **implementada** (V-Bounce N°1) ✔ | §9.1: 0 `BaseRule`/`DataAccess`/`DataBaseHelper`/`FillDataSetBySQLText`; el ensamblado sin arista hacia `Data` |
| **B — El acceso propio (lector)** | **implementada** (N°1) ✔ | `SqlClientSchemaReader` + las 4 consultas de esquema intactas |
| **C — La cadena desde el contrato** | **implementada** (N°1) ✔ | `ConnectionStringResolver` + los 8 casos herméticos de resolución |
| **D — Ubicación, dependencias y mapeo** | **implementada** ✔ (D.3-b **cerrada** acá) | D.1/D.2 ✔; **D.3-a** ✔ (6 archivos aportados con directiva, probada por el build); **D.3-b**: 3 destinos materializados + **retiro declarado** del 4.º (§9.2) |
| **E — La paridad observable** | **implementada** (escrita y compilada) ✔ | La lane de integración (§9.4); su corrida es del ciclo |

**Lo que este V-Bounce agrega:** la **verificación** del retiro, la **constancia** del registro (fragmento propio +
cierre de la obligación del 003) y el **cruce de cobertura** con el consolidado regenerado.

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/docs/api-removals/US-000.BOLT-016.json` | **La constancia propia** del Bolt en el registro distribuido: las dos rupturas de firma, el cambio de comportamiento de la cadena y el retiro de los editores con reemplazo cero. |
| `devflow/memory/MEM-261016-0526-reverse-sin-baserule.md` | Este MEM (exactamente uno por V-Bounce). |

---

## 4. Files modified

| Path | Cambio |
|---|---|
| `2.0/docs/api-removals/US-000.BOLT-003.json` | **Sólo el bloque/estado de la obligación**, como el SPEC autoriza: la entrada `BOLT003-09` pasa a `status: "materialized-and-closed"` y `materialized_by: "US-000.BOLT-016"`; el bloque `OBLIGATION-TOOLING-01` suma `closed_by: "US-000.BOLT-016"` y sus referencias cruzadas (el fragmento de este Bolt y `2.0/src/CooperatorModeler/DBReverseHelper`). |
| `2.0/docs/MIGRATION-NOTES.md` | **Regenerado** por `migration-notes.ps1`: 78 entradas de 11 fragmentos (antes 74/10). |
| `devflow/functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md` | G11: toma del V-Bounce N°2 y el estado final `in-development` (fila **T06**). |
| `devflow/metrics/bolts/US-000.BOLT-016-reverse-sin-baserule.json` | `v_bounces[]` += la entrada de este V-Bounce (`number: 2`, `spec_revision: 4`). |

**No** se tocó el componente del tooling ni ninguna otra parte del fragmento del 003 (§8.3).

---

## 5. Files deleted / 6. Files renamed

Ninguno. `1.4/**` intacto (§9.6).

---

## 7. Implementation decisions

### 7.1 El reemplazo cero se declara en prosa, no con el literal `"none"` (y por qué)

El esquema publicado (`api-removals.schema.json`, `allOf[2]`) **exige** `replacement_reason` cuando el reemplazo es
el literal `"none"` — regla que este Bolt estrena, porque es el primer fragmento que declara un retiro sin
reemplazo. El **validador propio del arnés**, en cambio, resuelve los campos con un `switch` **hardcodeado a seis
nombres** (`id`, `kind`, `what`, `why`, `replacement`, `evidence`) y **no lee** `replacement_reason`: con `"none"`,
el esquema exige la razón y el validador la considera ausente ⇒ **es imposible satisfacer a los dos** (§8.1).
La salida tomada, dentro de mi autoridad: declarar el **reemplazo cero en el texto** del campo `replacement`
("REEMPLAZO CERO (no hay reemplazo): …"), que es **exactamente lo que la rev.4 ordena** —el retiro se declara con su
qué, su por qué y su reemplazo— sin usar el literal que dispara la contradicción. No se tocó el arnés (§8.1).

### 7.2 La obligación se cierra en el bloque del 003, y sólo ahí

El registro es de **contribución distribuida** (SPEC-0047 §6/B): cada Bolt entrega **su** fragmento, y el cierre de
una obligación registrada por otro Bolt se declara en **su** fragmento, en el **bloque de la obligación** —que es lo
que el SPEC autoriza expresamente—. El `what`/`evidence` originales de la entrada `BOLT003-09` se conservan como
**historia** (dicen lo que era verdad cuando se escribieron: que el tooling no existía); el **estado** y el
`closed_by` son los que declaran el cierre (§8.3).

### 7.3 Las rupturas de firma se declaran aunque el gate no las vea

`signature-changed` para los dos puntos de entrada del helper: el tooling **no tiene ensamblado en la línea base
1.4** (el ensamblado `Cooperator.Framework.Utility` no existe en la 2.0), así que el gate **no puede** ver esa
diferencia y el registro la **rutea** a su dueño en lugar de inventarla —que es el comportamiento previsto: el
registro hace visible lo que no puede verificar—. La constancia existe para el **consumidor** del tool, no para el
gate.

---

## 8. Deviations and assumptions

### 8.1 Contradicción medida entre el esquema publicado y el validador del arnés (escalada a su dueño)

**Hecho.** `MigrationNotesCoverageTests.Field(string name)` es un `switch` con seis casos:

```csharp
public string Field(string name) => name switch
{
    "id" => Id, "kind" => Kind, "what" => What, "why" => Why,
    "replacement" => Replacement, "evidence" => Evidence,
    _ => string.Empty,
};
```

El validador del fragmento, entonces, comprueba `entry.Field("replacement_reason").Trim().Length == 0` —una
expresión que **siempre** es verdadera— y emite el hallazgo `` `replacement` es "none" y no declara
`replacement_reason` `` para **cualquier** entrada con `replacement: "none"`, aunque la razón esté declarada y el
esquema la exija. Medido: con `"none"` + `replacement_reason` presente, el caso
`Every_fragment_of_the_register_validates_against_the_published_format` **falla** (`MEM-261016-0526` §9.5, corrida
con el hallazgo literal).

**Qué hice y qué no.** **No** toqué el arnés: es un archivo del Bolt 001 y la única vía para cambiarlo sería una
autorización que este SPEC no da (§15 del SPEC-0027: una excepción se resuelve por ADR, **nunca** por decisión del
ejecutor). Declaré el reemplazo cero en prosa (§7.1) —fiel a la rev.4— y dejo el hallazgo **escalado**: extender
`Field()` al campo `replacement_reason` es una línea del arnés, y es del dueño del Bolt del registro/arnés.

### 8.2 La ruta de `MIGRATION-PARTITION.md` que cita el SPEC no es la del árbol (observación)

El §17 del SPEC cita la especificación tipo por tipo en `2.0/docs/MIGRATION-PARTITION.md`; en el árbol el documento
vive en `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` (donde el Bolt 003 lo publicó). Es una
referencia desactualizada de una línea de prosa: **no** cambia el alcance, **no** toca un entregable y queda
señalada para la próxima revisión del documento (no la corrijo: el SPEC es de otro actor y su §18 fija el
procedimiento).

### 8.3 El fragmento del 003 conserva su historia; el cierre vive en el estado

La entrada `BOLT003-09` conserva sus campos `what`/`evidence` originales —"`2.0/src/CooperatorModeler/` no existe
(verificado)"—, que eran verdad cuando se escribieron. El cierre se declara donde la tarea y el SPEC lo autorizan:
`status`, `materialized_by` y el bloque de la obligación (`closed_by` + referencias cruzadas). Es intencional: el
registro es acumulativo y su valor es que se pueda leer **cuándo** se dijo cada cosa.

### 8.4 Presupuesto de turnos (declarado)

La guía de la pasada era **10 loops**. El trabajo de verificación, constancia y cierre llegó al **verde en el loop
8**; el cierre (MEM + manifiesto) y su alineación consumen los siguientes: **11** en total. El gasto no fue de
convergencia sino de la **contradicción del §8.1**: descubrirla costó una corrida del arnés, un run dirigido y un
relevamiento del validador. **No** se activó ninguna condición de parada: la unidad está **verde** y la obligación
**cerrada**.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

SPEC-261016-0027 `approved` con **`revision: 4`** y `review.decision: approved` (`decided_at`
2026-10-16T05:13:37-03:00); el Bolt estaba `blocked` y se tomó (G11, T06); ADR-004/ADR-001/ADR-005 siguen
`accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` queda intacto. La rev.4 **formaliza
la adjudicación (a)** del `MEM-261016-0506` (retirar los editores) y **no** cambia nada más del plan: el aporte del
N°1 sigue en vigor sin una línea modificada.

---

## 9. Verification evidence

### 9.1 El retiro está efectivo (y el componente sigue sin la herencia)

```
archivos *Editor*/*Form* (.cs/.resx) bajo 2.0/src y 2.0/tests .......... 0
'PropertyEditors' en las fuentes del tooling .......................... 0
UseWindowsForms / System.Windows.Forms / System.Design / Drawing ...... 0
```

Y el componente conserva lo del N°1: 0 `BaseRule`, 0 `DataAccess`, 0 `Cooperator.Framework.Data`; el ensamblado
referencia `Modeling` y `Microsoft.Data.SqlClient`, y ninguna arista hacia la capa de datos
(`DependencyChecks`, 17 casos verdes).

### 9.2 La constancia de la materialización (referencia y estado final)

- **Referencia:** `OBLIGATION-TOOLING-01` en `2.0/docs/api-removals/US-000.BOLT-003.json` (entrada `BOLT003-09` +
  bloque `registered_obligations[]`), con su especificación tipo por tipo en
  `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` §2.2 y sus recordatorios en §5.
- **Estado final declarado en este V-Bounce:** `status: "materialized-and-closed"`, `materialized_by:
  "US-000.BOLT-016"`, `closed_by: "US-000.BOLT-016"`, y las referencias cruzadas que suman
  `2.0/docs/api-removals/US-000.BOLT-016.json` y `2.0/src/CooperatorModeler/DBReverseHelper`.
- **Los tres destinos y el retiro**: `MSSQLProvider`/`MSSQLReverseFactory` ✔ · el resto de `MSSQLCommon`
  (`SP_HelpTablesEnum`) ✔ · la mitad reverse de `DomainTreeHelper` ✔ · los `PropertyEditors/*` **retirados** (no
  reubicados) con su ruptura declarada ✔. **Cierre: 3 destinos + retiro** (SPEC §16/§17).

### 9.3 Los tres recordatorios obligatorios (evidenciados y re-verificados)

1. **Las cuatro llamadas cruzadas calificadas** como `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X`
   (`DependencyChecks.The_four_cross_calls_of_the_reverse_half_are_qualified`, verde).
2. **La lista de rupturas como insumo**: `MIGRATION-PARTITION.md` §4 (ocho rupturas) + las que este Bolt introduce,
   ya declaradas en el fragmento (las dos de firma, el cambio de comportamiento y el retiro) para las notas del
   Bolt 028.
3. **El reporte de dependencias a dos niveles**: presupuesto (el `csproj` con exactamente
   `Microsoft.Data.SqlClient` sin `Version` y su `packages.lock.json`) y aristas del ensamblado (sin `Data`) —
   `DependencyChecks`, verde.

### 9.4 La lane de integración (escrita y compilada; su corrida es del ciclo)

`CooperatorModeler.Reverse.Integration.Tests` compila con sus cuatro clases y `SchemaProvisioning.sql`. Es la
**primera prueba de integración del reverse** y la que cierra el plan del Bolt (§8.2 del SPEC): sin esa corrida el
Bolt **no** pasa a `ready-for-acceptance`.

### 9.5 El cruce de cobertura

```
migration-notes.ps1 → 78 entradas de 11 fragmentos → 2.0/docs/MIGRATION-NOTES.md
api-compat.ps1 -Mode Current → status=differences · 189 diferencias · 40 CP0002
```

Los dos casos de cobertura del arnés (`ApiRemovalCoverageTests.Every_removed_member_reported_by_the_gate_is_covered_by_a_fragment`
y `MigrationNotesCoverageTests.Every_removed_member_of_the_report_is_covered_by_a_fragment`) pasan: **0 diferencias
sin cobertura**. El retiro de los `PropertyEditors` no produce `CP0002` (el tipo no existía en un ensamblado de la
2.0) y por eso su declaración es de las notas, no del gate.

### 9.6 `1.4/**` intacto y rutas propias

Escrituras de este V-Bounce: `2.0/docs/api-removals/US-000.BOLT-016.json` (nuevo), el **bloque/estado** de la
obligación en `2.0/docs/api-removals/US-000.BOLT-003.json` (§4, autorizado por el SPEC), `2.0/docs/MIGRATION-NOTES.md`
(regenerado), el Bolt (la toma y el estado final) y este MEM con su entrada de manifiesto. `1.4/**`: **0 cambios**.

### 9.7 La suite hermética del árbol: VERDE

```
build sin advertencias (tratadas como error): 24 proyectos
Passed!  - Failed: 0, Passed: 135  - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  12  - Cooperator.Framework.Core.Mapping.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  11  - ConnectionStringEncriptor.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  17  - CooperatorModeler.Reverse.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  46  - Cooperator.Framework.Data.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  83  - Cooperator.Framework.Characterization.Tests.exe (net48)
Resultado: ok (codigo de salida 0)     → 304 casos
```

### 9.8 Gates (§9 del SPEC)

`pass`: Build/lint/type checks (24 proyectos, 0 advertencias) · Unit/component (304 verdes) · Consumer-side contract
tests (dobles del contrato de configuración y del resolutor) · SAST/OWASP estático · Secret-leak (0 cadenas: valores
sintéticos y variables de entorno) · Hallucination lint (cada ruptura trazable al código y a la rev.4) ·
IP/license · PII/DLP · Dependency-confusion (0 paquetes nuevos) · Test-first evidence · Behavioral reproducibility ·
Bolt-manifest validation. `n/a` con razón: prompt-injection y las filas dinámicas de integración.
**`n/a` declarado con causa**: *Integration tests* — el plan es `applicable` y su corrida es del **ciclo** (§9.4).

---

## 10. Manual interventions

Ninguna intervención humana durante este V-Bounce. **Sin operaciones de git** (G34). La decisión humana que se pide
está en §13.1 (el cierre de la obligación y el hallazgo del §8.1).

---

## 11. Evidence links

- **SPEC ejecutado:** [SPEC-261016-0027](../spec/SPEC-261016-0027-reverse-sin-baserule.md) — revisión **4**: §D.3-b
  (el retiro ordenado), AC-8 (la constancia con qué/por qué/reemplazo), §16 (la obligación materializada y cerrada) y
  §17 (el estado final: 3 destinos + retiro).
- **V-Bounce anterior (el aporte y el bloqueo):** [`MEM-261016-0506`](MEM-261016-0506-reverse-sin-baserule.md) —
  §8.1 (el bloqueo medido) y §9.2 (la materialización parcial).
- **La obligación y su blueprint:** `2.0/docs/api-removals/US-000.BOLT-003.json` ·
  `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` §2.2/§4/§5 · `MEM-261016-0046` §13.
- **El mecanismo del registro:** [SPEC-261016-0047](SPEC-261016-0047-notas-de-migracion.md) §6/B (contribución
  distribuida) y `2.0/docs/api-removals/api-removals.schema.json` (el formato publicado).
- **El entregable:** `2.0/docs/api-removals/US-000.BOLT-016.json` · `2.0/docs/MIGRATION-NOTES.md` ·
  `2.0/src/CooperatorModeler/**`.

---

## 12. Metrics

| Metric | Value |
|---|---|
| V-Bounce | 2 (de US-000.BOLT-016) — **`ready_for_review`** |
| SPEC ejecutado | SPEC-261016-0027 revisión **4** |
| Entregable de esta pasada | El retiro verificado + el fragmento propio (4 entradas) + la obligación cerrada + el consolidado regenerado |
| Fragmentos del registro | **11** · entradas del consolidado: **78** (antes 74/10) |
| Suite hermética del árbol | **304 verdes**, exit 0, 24 proyectos sin advertencias |
| Loops usados | **11** (guía 10; el verde se alcanzó en el 8) |

---

## 13. Pending items and stubs

1. **[DECISIÓN HUMANA — lo que se pide adjudicar] El cierre de `OBLIGATION-TOOLING-01`** como **3 destinos
   materializados + retiro declarado del 4.º** (SPEC §16/§17) y la constancia del registro (§9.2).
2. **Hallazgo escalado (§8.1):** el validador del arnés (`MigrationNotesCoverageTests.Field`) no lee
   `replacement_reason`, de modo que el literal `"none"` del esquema es insatisfacible. Dueño: el Bolt del arnés /
   del registro; la corrección es extender ese `switch` a los campos del esquema (una línea).
3. **La corrida del ciclo de test de integración** (SPEC §8.2): la lane está escrita y compilada; su corrida cierra
   el plan y habilita `ready-for-acceptance`.
4. **La referencia del §17 del SPEC** a `2.0/docs/MIGRATION-PARTITION.md` (la ruta real es la del paquete
   `Modeling`), para la próxima revisión del documento (§8.2).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto (`number: 2`, `spec_revision: 4`,
  `ready_for_review`) + el registro actualizado (el fragmento propio y la obligación cerrada) + el consolidado
  regenerado, con la suite hermética en verde (304).
- **Qué se pide decidir:** (1) la adjudicación del **cierre de la obligación** (§9.2) sobre el entregable de este
  V-Bounce y el del N°1; (2) el hallazgo del §8.1 y su dueño; (3) el paso del Bolt a su **ciclo de integración**
  (su `test_integration: required`).
