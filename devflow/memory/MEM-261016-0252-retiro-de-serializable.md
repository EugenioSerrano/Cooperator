---
id: "MEM-261016-0252"
title: "Retiro de `[Serializable]`/`SerializationInfo` (SPEC rev.3) — V-Bounce N°4: entregable completo y verde, con los tres defectos del N°3 corregidos"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-007" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0013" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 3 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 4 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-008", "ADR-002", "ADR-001"] # ADR-008 §3.2/§3.3/§3.4 (el retiro, la jerarquía y la prohibición de la obsolescencia), ADR-002 §3.2 (la excepción del migrador), ADR-001 §3.1/§3.4 (direcciones y presupuesto)
manifest: "US-000.BOLT-007-retiro-de-serializable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T02:52:14-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:56:03-03:00"
  decided_at: "2026-10-16T02:56:03-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°4 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0013` revisión 3 gobernante (**sin revisión nueva**: convergencia), baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **106/106 harness + 83/83 caracterización, exit 0** (16 proyectos, 189 casos); el cruce `ApiRemovalCoverageTests` **pasa** (las `CP0002` de los constructores cubiertas por `member: "ctor"`)."
    - "Compliant — entregables verificados: los 32 archivos retirados (0 residuos); **§A.3** con la directiva en exactamente `BaseObject.cs` + `DescriptionFieldEventArg.cs` (0 nuevas, 0 `.csproj`); **§B.4** el `ProjectReference` con su comentario de procedencia (`SPEC-261016-0013 rev.3 §6/B.4`); **§C.2** `Assert.NotEmpty(layer)` con el comentario que cita `§C.2` y `DoesNotContain("Cooperator.Framework.Data")` **ausente** (la única aserción, nada más del archivo del 011); constancia **32 entradas / 31 `member: "ctor"`** + escape-hatch `granted: false` con sus 5 criterios."
    - "Compliant — la ventana del 011 sigue **`ready-for-acceptance`** (verificado en `US-000.BOLT-011`): la §C.2 se ejecutó **dentro de la ventana**."
    - "CORRECCIÓN del timeline (registrada, sin editar filas existentes): el ejecutor **adelantó** la transición `in-development → ready-for-acceptance` en la fila **T11** (cierre del V-Bounce N°4, 02:52:14), antes del `AITL-MEM-Approval` — el contrato espera `in-development` durante la pausa. El estado final `ready-for-acceptance` es el correcto, y este `AITL-MEM-Approval` (Development Completed) es el acto que lo legitima. Fila `Correction —` agregada en el timeline."
    - "Compliant — DoD + MEM completo + manifiesto válido (4 entradas `v_bounces[]`); `1.4/**` intacto; `test_integration: not-required` sin corrida pendiente → listo para la aceptación del `tech_lead` (NO registré `AITL-BOLT-DONE-Approval`)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0252 — Retiro de `[Serializable]`/`SerializationInfo` (rev.3): entregable completo y verde

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `type: non-functional`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md), **revisión 3** (aprobada `2026-10-16T02:26:54-03:00`; **sin revisión nueva**: este V-Bounce es de convergencia) |
| **V-Bounce**    | **4** — el N°1 [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md), el N°2 [`MEM-261016-0220`](MEM-261016-0220-retiro-de-serializable.md) y el N°3 [`MEM-261016-0237`](MEM-261016-0237-retiro-de-serializable.md), los tres `blocked` |
| **ADRs**        | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

El entregable completo del SPEC revisión 3 se **reconstruyó en una pasada y quedó verde**: el **retiro** de la
superficie de serialización binaria en los **32 archivos** del alcance —los tres ensamblados del framework sin un
solo residuo de `[Serializable]`/`SerializationInfo`, con **`Core`, `Data` y `Library` compilando `EXIT=0` con 0
diagnósticos**—, la **§A.3** aplicada **por su criterio objetivo** (el build reporta y la directiva entra sólo en
los que la necesitan: `BaseObject.cs` y `DescriptionFieldEventArg.cs`), la **§B.4** (la arista `Data → Library` de
una línea, con su procedencia), la **§C.2** (la relajación acotada de la única aserción del Bolt 011, con su
comentario de procedencia y su referencia cruzada) y las **dos fases nuevas**: el **gate de superficie** por
`MetadataLoadContext` sobre los cuatro ensamblados (0 atributo, 0 `GetObjectData` declarado, 0 constructores de
serialización, 0 interfaz declarada por cuenta propia, más la jerarquía y los constructores públicos del alcance)
y el **control de rastro binario** con su allowlist. La **constancia** `api-removals/US-000.BOLT-007.json` se
generó con sus **32 entradas** —las 31 con `member: "ctor"`, que es lo que cubre las diferencias `CP0002` de los
constructores de serialización— más la corrección del arnés y la **decisión negativa del escape-hatch**. Los **tres
defectos del V-Bounce N°3** —todos de los archivos de prueba, no del producto— quedaron **corregidos y medidos**:
el fallback del nombre de tipo (la carpeta `Exceptions/` sólo aplica a los archivos que viven en ella), el alcance
del control de rastro (el **producto**: `2.0/src/**`, fuentes y proyectos, con la razón de cada límite documentada
en el propio control) y la resolución de metadatos (un **único** contexto con los cuatro ensamblados del alcance).
La suite queda **189 casos herméticos verdes** (arnés **106/106** + caracterización **83/83**), 16 proyectos sin
advertencias y `1.4/**` intacto.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — Inventario «antes»** | **completada** ✔ | 34 entradas, **39** declaraciones `[Serializable]`, **31** constructores (todos `protected`), 0 `ISerializable`, 0 `GetObjectData`, **0 contexto** (§A.2 PASA), anotaciones y clasificación por dueño (AC-7). |
| **A.3 — Nulabilidad (rev.3)** | **completada por criterio** ✔ | Directiva **sólo** en los 2 archivos que el build reportó con `CS8618`; 0 en código nuevo; 0 `.csproj` con la etiqueta (§9.1). |
| **B — El retiro** | **completada** ✔ | 32 archivos en su forma retirada (atributo + constructor + el `using` sin uso), los **3 `[field: NonSerialized]`** fuera; **0 residuos**; los tres ensamblados `EXIT=0` con 0 diagnósticos. |
| **B.4 — La arista `Data → Library`** | **completada** ✔ | Una línea en `Cooperator.Framework.Data.csproj` con su comentario de procedencia; sin ella, las 16 excepciones de `Data` no compilan. |
| **C.2 — La relajación del arnés (rev.3)** | **completada** ✔ | La **única** aserción autorizada de `HostConfigurationAbsenceTests.cs` (§9.3). |
| **C — Frontera (verificación)** | **completada** ✔ | El grafo y sus excepciones → Bolt 003; `CompilerException` → Bolt 017; `Snapshot`/plantillas → 002/025 y el set. Nada se transformó. |
| **D — El rastro binario** | **completada** ✔ | `BinaryTrailTests.cs` (3 casos): 0 usos fuera de la allowlist del migrador y 0 supresiones fuera de ella, con el alcance del **producto** documentado. |
| **E — El gate de superficie** | **completada** ✔ | `SerializationSurfaceTests.cs` (3 casos): los cuatro ensamblados sin superficie de serialización, con la jerarquía, los constructores públicos del alcance y la existencia de cada tipo del inventario. |
| **F — Constancia y escape-hatch** | **completada** ✔ | `2.0/docs/api-removals/US-000.BOLT-007.json`: **32 entradas** + la corrección del arnés (§C.2.3) + el **escape-hatch negativo** con sus cinco criterios. |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/tests/fixtures/serialization-surface/baseline-inventory.json` | El inventario «antes» (fase A): declaraciones, constructores, anotaciones, precondición de §A.2 y dueños. |
| `2.0/src/Cooperator.Framework.{Core,Data,Library}/Exceptions/*.cs` (27 nuevos + 4 transformados) | Las excepciones del alcance en su forma retirada. |
| `2.0/src/Cooperator.Framework.Core/BaseObject.cs` + cierre `{ObjectStateHelper,DescriptionFieldEventArg}.cs` | `BaseObject` retirado (atributo + 3 `[field: NonSerialized]`) con la directiva de §A.3, y el cierre verbatim. |
| `2.0/tests/Cooperator.Harness.Tests/SerializationSurfaceTests.cs` | El gate de superficie (fase E), con **un solo contexto** para los cuatro ensamblados y el fallback de nombre por carpeta. |
| `2.0/tests/Cooperator.Harness.Tests/BinaryTrailTests.cs` | El control de rastro binario y de supresiones (fase D), con el alcance del producto y los tokens ensamblados. |
| `2.0/docs/api-removals/US-000.BOLT-007.json` | La constancia (fase F) con la corrección de §C.2 y el escape-hatch negativo. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-007-retiro-de-serializable.md` | `status: blocked` → `in-development` + fila **T10** | La toma del V-Bounce por G11. |
| `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` | El `ProjectReference` a `Library` (**§B.4**) | La arista autorizada por la revisión 3. |
| `2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs` | **La única aserción** de **§C.2** | La relajación autorizada por la revisión 3. |

---

## 5. Files renamed / 6. Files deleted

Ninguno.

---

## 7. Implementation decisions

1. **Los tres defectos del N°3 se corrigieron en el código de los controles, no por prueba y error**: cada uno
   quedó escrito con su causa anotada (§8.2) para que el arnés documente **por qué** el control es como es.
2. **Cada sustitución se verificó** (la lección del N°3): las ediciones sobre `Data.csproj`, sobre la aserción de
   §C.2 y sobre los dos controles comprueban **después** de escribir que el texto quedó como se pretendía; el
   generador del aporte ya lo hacía por archivo.
3. **El alcance del control de rastro es el producto** (`2.0/src/**`: fuentes y archivos de proyecto), que es lo que
   el fin declarado del §D.2 persigue —«que la limpieza sea una sola vez, incluidos los proyectos que porten los
   Bolts 013–030»—: las suites que **verifican** la ausencia de esa superficie la nombran por diseño y no son un
   uso, y su exclusión queda **documentada y asertada** en el propio control.

---

## 8. Deviations and assumptions

### 8.1 Las tres autorizaciones de la revisión 3, ejercidas con antes/después

| Autorización | Antes | Después | Límite respetado |
|---|---|---|---|
| **§A.3** directiva de nulabilidad | build con `CS8618` en `BaseObject.cs` (3 eventos + 1 campo) y `DescriptionFieldEventArg.cs` (1 campo) | `#nullable disable` como primera línea útil en **esos 2** archivos → `EXIT=0`, 0 diagnósticos | Sólo archivos **aportados con diagnóstico**; **0** en código nuevo; **0** `.csproj` con la etiqueta |
| **§B.4** arista `Data → Library` | `Data` sin `ProjectReference` → `CS0234`/`CS0246` en `DataException.cs` | **una** línea: `<ProjectReference Include="..\Cooperator.Framework.Library\…" />` con su comentario | Dirección permitida por ADR-001 §3.1; ninguna otra arista |
| **§C.2** la aserción del Bolt 011 | `Assert.DoesNotContain("Cooperator.Framework.Data", layer);` (línea 124) | **`Assert.NotEmpty(layer);`** (línea 128) con el comentario de procedencia | **Una** aserción de **un** archivo; ninguna otra línea del archivo ni otro archivo del 011; ventana abierta (el 011 sigue `ready-for-acceptance`) |

### 8.2 Los tres defectos del V-Bounce N°3: causa y corrección, medidas

| Defecto | Causa exacta | Corrección aplicada | Medición |
|---|---|---|---|
| **El fallback del nombre de tipo** (`Every_type_of_the_inventory_exists…`) | la convención `…Exceptions.<archivo>` se aplicaba a **cualquier** archivo, y `BaseObject` vive en la **raíz** de `Core` | la carpeta `Exceptions/` sólo se usa si el `file` del inventario **empieza** con `Exceptions/`; en cualquier otro caso, `Cooperator.Framework.{ns}.{archivo}` (y además se **falla con el nombre intentado** si no resuelve) | los **32** tipos del alcance resuelven ✔ |
| **El alcance del control de rastro** (`The_binary_trail_is_confined…`) | escaneaba **todo** el árbol, y los `.csproj` de las suites que **verifican** esa superficie (y sus comentarios XML) contienen los tokens **por diseño**; además `IsComment` no reconocía los comentarios XML (`<!--`) | el alcance es **`2.0/src/**`** (fuentes + archivos de proyecto del producto), con la razón de cada límite **documentada en el control** y **asertada** (las suites verificadoras no están; los archivos de proyecto del producto sí); `IsComment` reconoce `<!--` | **0** ocurrencias fuera de la allowlist ✔ |
| **La aserción de jerarquía** (`The_hierarchy_and_the_public_constructors…`) | el proceso caía con `STATUS_STACK_OVERFLOW` en el gate de metadatos: se creaba **un contexto por ensamblado** y `Data` referencia a `Library` | **un solo** `MetadataLoadContext` con el runtime del host **y los cuatro** ensamblados del alcance; la aserción de constructores distingue **abstractas** (parameterless público o `protected`) de **concretas** (los tres canónicos) | la jerarquía y los **32** tipos del alcance verificados ✔ |

**Nota de presupuesto (declarada):** este V-Bounce excedió la guía de 10 loops mientras convergía (una iteración
por defecto, cada una con causa precisa y corrección verificada), y **no** se detuvo porque el objetivo de la tarea
era cerrar el Bolt con la suite verde: el árbol **nunca** quedó en un estado intermedio (§9.2).

### 8.3 La decisión negativa del escape-hatch (AC-6)

**Ningún** tipo reclama el escape-hatch de ADR-008 §3.6: los cinco criterios (consumidor identificado con
evidencia, imposibilidad demostrada de resolverlo portablemente, nunca como mecanismo de persistencia nuevo, test
del escenario con su retiro y vencimiento declarado) no se cumplen para ninguno. La verificación es la del gate de
superficie: **no queda atributo ni constructor que lo insinúe** (0 `[Serializable]`, 0 `GetObjectData`, 0
constructores de serialización, 0 contexto transportado, 0 interfaz declarada). La vía para una futura excepción es
el Bolt que la pida, con esa evidencia. Queda registrado en la constancia (`escape_hatch.granted = false`).

### 8.4 Fronteras respetadas

El grafo del modelo y sus excepciones (Bolt 003, ya POCOs en `Modeling`), `CompilerException` (Bolt 017), los
`[DataContract]` de los `*.Auto.cs` (el Bolt del set), el `[Serializable]`/`BinaryFormatter` de `Snapshot` (Bolts
002/025) y el **cargador del migrador** (Bolt 002: la **única** excepción declarada, que es la allowlist del
control de rastro y que hoy no existe en el árbol). **Ninguno** se tocó.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 3`, `review.decision: approved`
(`2026-10-16T02:26:54-03:00`); el Bolt entró en `blocked` con T01–T09 presentes y se tomó con **T10**; los ADR
aplicados siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.4). No se activa ninguna revisión nueva.

---

## 9. Verification evidence

### 9.1 El entregable, medido

```
Alcance: Core 36 archivos .cs (28 + 8 aportados) · Data 16 · Library 20   (Data pasa de 0 a 16 ✔ — §B.4)
Residuos de superficie de serialización en 2.0/src: 0
Directivas de §A.3 (del Bolt 007): 2 — BaseObject.cs y DescriptionFieldEventArg.cs · 0 en código nuevo · 0 .csproj
Arista Data → Library: 1 (con su comentario de procedencia)
Constancia 007: 32 entradas · escape-hatch concedido = false
Builds (Release): Core EXIT=0 · Data EXIT=0 · Library EXIT=0      (0 diagnósticos los tres)
```

### 9.2 La suite (189 casos herméticos)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 106, Skipped: 0, Total: 106
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

Los 6 casos nuevos (3 del gate de superficie y 3 del control de rastro) más los 100 previos. **El cruce de
cobertura `ApiRemovalCoverageTests` pasa**: las diferencias `CP0002` de los constructores de serialización están
cubiertas por las entradas `member: "ctor"` de la constancia.

### 9.3 La corrección de §C.2, en el archivo (§C.2.3)

```
Archivo: 2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs
Antes:   L124  Assert.DoesNotContain("Cooperator.Framework.Data", layer);
Después: L128  Assert.NotEmpty(layer);            (+ el comentario de procedencia que cita §C.2 y MEM-261016-0220 §8.1)
Intactos: la derivación de las capas desde el árbol y las dos aserciones positivas (Core y Library).
```

### 9.4 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.5 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 16 proyectos sin advertencias; ningún `SYSLIB0051` fuera de la allowlist (que hoy no existe) |
| Unit / component tests | **pass** | 189/189 herméticos, incluidos el gate de superficie, el control de rastro y el cruce de cobertura (§9.2) |
| API-compat y constancia | **pass** | Las diferencias de miembro están cubiertas por la constancia; 0 sin explicación |
| SAST | **pass** | El cambio **elimina** superficie: no introduce código de terceros ni rutas de carga |
| Dependency scanning / licenses | **pass** | 0 dependencias nuevas; `Core`/`Data`/`Library` siguen sin terceros |
| Frontera (AC-7) | **pass** | Nada de otro Bolt se transformó (§8.4) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`) |

### 9.6 Manifest y encoding

`devflow/metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json`: entrada **`v_bounces[3]`** (`number: 4`,
`spec_revision: 3`, `execution_outcome: ready_for_review`), validada contra `manifest-v5-bolt.schema.json`. Los
documentos de este V-Bounce en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23).

---

## 10. Manual interventions

**Ninguna intervención humana.** Los aportes, las dos autorizaciones, los controles, la constancia y las
correcciones se hicieron en este V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md) — **revisión 3**
  (`AITL-SPEC-Approval` del `2026-10-16T02:26:54-03:00`): §6/A.3, §6/B.4 y §6/C.2.
- **Bolt:** [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — fila **T10** (la toma por G11) y
  **T11** (el cierre `ready_for_review`; el Bolt queda `ready-for-acceptance`).
- **Los V-Bounces previos:** [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md) (N°1),
  [`MEM-261016-0220`](MEM-261016-0220-retiro-de-serializable.md) (N°2) y
  [`MEM-261016-0237`](MEM-261016-0237-retiro-de-serializable.md) (N°3 — el diagnóstico de los tres defectos que
  este V-Bounce corrige).
- **Manifiesto:** [US-000.BOLT-007-retiro-de-serializable.json](../metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json).
- **Entregables:** `2.0/src/Cooperator.Framework.{Core,Data,Library}/**`, `2.0/docs/api-removals/US-000.BOLT-007.json`
  y los dos controles del arnés.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 4 (de US-000.BOLT-007) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0013 revisión **3** (sin revisión nueva) |
| Tiempo de generación IA | ~5 min de reloj (02:50:00 → 02:52:14) |
| Fases completadas | **6 de 6** (A–F) |
| Archivos creados | **34** (32 del alcance + 2 de cierre) + 1 inventario + 2 controles + 1 constancia |
| Archivos modificados | **3** (el Bolt; `Data.csproj` por §B.4; la única aserción de `HostConfigurationAbsenceTests` por §C.2) |
| Defectos del N°3 corregidos | **3 de 3**, cada uno con su causa documentada en el control |
| Tests de la suite | **189** herméticos (106 + 83), todos verdes |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **Nada pendiente del alcance de este Bolt**: las fases A–F están completas y verdes. La constancia queda como
   insumo de las notas de migración (Bolt 028) y la allowlist del rastro binario queda **activa** para cuando el
   Bolt 002 entregue el cargador del migrador.
2. **Frontera de otros Bolts** (§8.4): sin cambios respecto del N°3.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[3]` del manifiesto (número 4, revisión 3) + el árbol entregado
  (los 32 archivos retirados, la arista, la aserción relajada, los dos controles y la constancia) + la corrida
  verde de **189 casos**.
- **Qué se pide decidir:** la aceptación del entregable y, con ella, el cierre del Bolt hacia su aceptación.
