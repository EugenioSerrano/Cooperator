---
id: "MEM-261016-0220"
title: "Retiro de `[Serializable]`/`SerializationInfo` (SPEC rev.2) — V-Bounce N°2: el retiro y las dos autorizaciones ejecutadas y verdes, detenido por una invariante ajena que el propio SPEC invalida"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-007" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0013" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-008", "ADR-002", "ADR-001"] # ADR-008 §3.2/§3.3/§3.4 (el retiro y su constancia), ADR-002 §3.2 (la excepción del migrador), ADR-001 §3.1/§3.4 (direcciones permitidas y cero dependencias)
manifest: "US-000.BOLT-007-retiro-de-serializable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §6) — working tree only, no git operations (G34)
review_ready_at: "2026-10-16T02:20:59-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: ""
  reviewers: []
  written_by: []
  started_at: ""
  decided_at: ""
  findings: []
  acknowledged_without_comment: true
  acknowledgment_reason: ""
---

# MEM-261016-0220 — Retiro de `[Serializable]`/`SerializationInfo` (rev.2): ejecutado y verde, detenido por una invariante ajena

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `type: non-functional`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md), **revisión 2** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T02:15:49-03:00`) |
| **V-Bounce**    | **2** (el N°1 es [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md), `blocked`) |
| **ADRs**        | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.4) |

---

## 1. Executive summary

La revisión 2 desbloqueó las dos piezas que el V-Bounce N°1 había medido y no podía autorizar, y **ambas se
ejecutaron y se midieron verdes**: la **política de nulabilidad de §6/A.3** se aplicó por su **criterio objetivo**
—el generador construyó el árbol con el retiro, el build **reportó los archivos con diagnósticos**, se puso
`#nullable disable` como primera línea útil **sólo** en los que los produjeron (**`BaseObject.cs`** y
**`DescriptionFieldEventArg.cs`**; `ObjectStateHelper.cs` **no** produjo ninguno, así que el criterio lo dejó sin la
directiva) y el build volvió a correr— y la **arista `Data → Library` de §6/B.4** se agregó como el
`ProjectReference` de una línea que la jerarquía de `DataException` necesita. Con eso, los cuatro ensamblados del
alcance quedaron **sin superficie de serialización** (0 `[Serializable]`, 0 `ISerializable`, 0 `GetObjectData`, 0
constructores `(SerializationInfo, StreamingContext)`; **0 residuos** en `2.0/src`) y **los tres ensamblados
compilan `EXIT=0` con 0 diagnósticos**. Al correr el árbol completo aparecieron **dos** pruebas rojas: una es
trabajo pendiente mío (la constancia de la fase F, que aún no escribí) y la otra es **una aserción de otro Bolt que
el propio SPEC invalida**: `HostConfigurationAbsenceTests` (Bolt 011) afirma que `Cooperator.Framework.Data` **no**
figura entre las capas con fuentes —y el §B.2 de esta revisión ordena aportar las **16 excepciones de `Data`**,
porque su jerarquía deriva de `BaseException`—. Ajustar esa aserción es una decisión del dueño de ese Bolt, no del
ejecutor (§15 #9: un gate que no cierra en `pass` se detiene y se escala), así que el V-Bounce se detuvo y **los
aportes se revirtieron**: el árbol queda **sin cambios y verde** (16 proyectos, 0 advertencias; arnés 85/85;
caracterización 83/83; `EXIT=0`).

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo |
|---|---|---|
| **A — Inventario «antes»** | **completada** | El inventario legible por máquina: **34 entradas**, **39 declaraciones `[Serializable]`**, **31 constructores** de serialización (todos `protected`), **0** `ISerializable`, **0** `GetObjectData`, **0** constructores con contexto (§A.2 **PASA**), las anotaciones (`[field: NonSerialized]` ×3 en `BaseObject`) y la **clasificación por dueño** (AC-7). |
| **A.3 — Política de nulabilidad (nueva, rev.2)** | **ejecutada y medida** | Directiva aplicada por **archivo aportado + diagnóstico**: 2 archivos (`BaseObject.cs`, `DescriptionFieldEventArg.cs`); `ObjectStateHelper.cs` no la necesita (0 diagnósticos). **Ningún** archivo nuevo la lleva y **ningún** `.csproj` declara `<Nullable>disable</Nullable>` (§9.2). |
| **B — Retiro (`BaseObject` + 31 excepciones)** | **ejecutada y medida** | 32 archivos en su forma retirada (atributo + constructor + `using` sin uso), los 3 `[field: NonSerialized]` fuera, constructores públicos y jerarquía intactos. **0 residuos** de superficie en `2.0/src`. |
| **B.4 — Arista `Data → Library` (nueva, rev.2)** | **ejecutada** | El `ProjectReference` de una línea en `Cooperator.Framework.Data.csproj`, con su comentario de procedencia; `Data` compila `EXIT=0`. |
| **C — Frontera (verificación)** | **completada en el inventario** | Los tipos del grafo y sus excepciones → **Bolt 003** (POCOs, verificados por su suite); `CompilerException` → **Bolt 017**; `Snapshot`/plantillas → 002/025 y el set. Nada se transformó (AC-7). |
| **D — Rastro binario y allowlist** | **pendiente** (V-Bounce N°3) | El control `BinaryTrailTests` (rastro, allowlist del migrador y supresiones acotadas). |
| **E — Enforcement de superficie** | **pendiente** | El gate `SerializationSurfaceTests` sobre los cuatro ensamblados y la verificación positiva de la jerarquía. |
| **F — Constancia, escape-hatch y cierre** | **pendiente** | `2.0/docs/api-removals/US-000.BOLT-007.json` (la corrida del arnés ya midió qué diferencias debe cubrir: los **constructores de serialización** de las excepciones de `Core` y `Library` aparecen como `CP0002`) y la decisión negativa del escape-hatch en su forma final. |

---

## 3. Files created

Todos **se generaron y se midieron en este V-Bounce, y se revirtieron al cerrarlo** (§6).

| Path | Propósito |
|---|---|
| `2.0/tests/fixtures/serialization-surface/baseline-inventory.json` | El inventario «antes» de §A.1 (34 entradas con dueño, declaraciones, constructores, visibilidad, anotaciones y la precondición de §A.2). |
| `2.0/src/Cooperator.Framework.{Core,Data,Library}/Exceptions/*.cs` (**27 nuevos + 4 transformados**) | Las excepciones del alcance en su forma retirada (5 nuevas de `Core`, 16 de `Data`, 6 de `Library`) más el retiro aplicado a las 4 preexistentes (`CoreException`, `InvalidDataSetException`, `BaseException`, `SmtpErrorException`). |
| `2.0/src/Cooperator.Framework.Core/BaseObject.cs` | `BaseObject` en su forma retirada (sin `[Serializable]` ni los 3 `[field: NonSerialized]`) **y con la directiva de §A.3** (es uno de los 2 archivos que produjo diagnósticos). |
| `2.0/src/Cooperator.Framework.Core/{ObjectStateHelper,DescriptionFieldEventArg}.cs` | El **cierre mínimo** de `BaseObject`, aportado verbatim; `DescriptionFieldEventArg.cs` lleva la directiva de §A.3, `ObjectStateHelper.cs` **no** (no produjo diagnósticos). |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-007-retiro-de-serializable.md` | `status: blocked` → `in-development` y fila **T06**; al cerrar, `in-development` → `blocked` con la fila **T07** | Las transiciones de estado del Bolt con su fila de `Status timeline`. |
| `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` | Un `ProjectReference` a `Library` (§6/B.4), **revertido** al cerrar | La arista que la jerarquía de `DataException` necesita y que ADR-001 §3.1 permite. |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Todo lo de §3 se **revirtió** (rollback = no-cambio) y los **4 archivos preexistentes** del alcance se
**restauraron verbatim** desde el 1.4, de modo que el árbol queda **idéntico** al estado inicial y **verde**:

```
Core: 28 archivos .cs · Data: 0 · Library: 3      (como al inicio)
fixture del inventario: retirado · arista Data → Library: retirada · directivas aplicadas: retiradas con sus archivos
arnés 85/85 · caracterización 83/83 · build 16 proyectos sin advertencias · EXIT=0        (§9.4)
1.4/**: git status vacío                                                                    (§9.5)
```

---

## 7. Implementation decisions

1. **La directiva de §A.3 se aplicó por medición, no por nombre.** El generador construye el árbol, el **build
   reporta** los archivos con diagnósticos de nulabilidad, la directiva se inserta **sólo** en esos (primera línea
   útil, tras el encabezado de licencia) y el build se repite hasta quedar limpio. Es el **criterio objetivo** que el
   SPEC fija: *archivo aportado del 1.4 + diagnóstico*. El resultado fue **2 archivos**, no 3: `ObjectStateHelper.cs`
   no produjo ninguno y el criterio —y el propio SPEC— dicen que **sólo** los que los producen la llevan.
2. **La arista `Data → Library` se agregó como una línea con su procedencia.** Es la **única** arista que este Bolt
   agrega, la permite ADR-001 §3.1 y la exige el §B.2 al fijar `BaseException` como base de la jerarquía de `Data`.
3. **El retiro sigue siendo mecánico y trazable**, con las tres operaciones de siempre (atributo, constructor con su
   doc-comment y su cuerpo, y el `using System.Runtime.Serialization` sólo si quedó sin uso) más los 3
   `[field: NonSerialized]` de `BaseObject`.

---

## 8. Deviations and assumptions

### 8.1 El bloqueo: una aserción de otro Bolt que esta revisión invalida

```
dotnet test … Cooperator.Harness.Tests            (con el aporte aplicado)
  Failed HostConfigurationAbsenceTests.The_absence_control_states_which_layer_it_measured
    Assert.DoesNotContain() Failure: Item found in collection
    Collection: ["Cooperator.Framework.Core", "Cooperator.Framework.Data", "Cooperator.Framework.Library", "Cooperator.Framework.Modeling"]
    Found:      "Cooperator.Framework.Data"
  Failed ApiRemovalCoverageTests.Every_removed_member_reported_by_the_gate_is_covered_by_a_fragment
    Collection: ["CP0002: Member 'Cooperator.Framework.Core.Exceptio…", …]        ← la constancia de la fase F aún no existe
  Failed! - Failed: 2, Passed: 83, Total: 85
```

**Las dos fallas tienen causas distintas y sólo una es mía:**

- **`ApiRemovalCoverageTests`** (Bolt 006) reclama **mi** constancia: las diferencias de miembro que el retiro
  produce —los constructores de serialización de las excepciones de `Core` y `Library`, que el gate reporta como
  `CP0002`— deben estar cubiertas por `2.0/docs/api-removals/US-000.BOLT-007.json`, que es el **entregable de la
  fase F** y todavía no lo escribí. Es **trabajo pendiente**, no un defecto: la lista exacta a cubrir ya está medida
  por esa misma corrida.
- **`HostConfigurationAbsenceTests`** (Bolt 011) afirma que `Cooperator.Framework.Data` **no** está entre las capas
  que la ausencia-control midió —una **instantánea** del árbol de aquel cierre, cuando `Data` no tenía fuentes— y
  **el §B.2 de esta revisión ordena aportar las 16 excepciones de `Data`** (su jerarquía deriva de
  `BaseException`). Es decir: **el SPEC vigente y esa aserción son mutuamente excluyentes**, y cuál de los dos cede
  es una decisión del **dueño** de ese artefacto, no del ejecutor (condición de parada §15 #9: un gate que no cierra
  en `pass` se detiene y se escala).

**La salida mínima (una línea, el patrón que el proyecto ya usó en SPEC-0004 §A.4):** que la aserción de
`HostConfigurationAbsenceTests` deje de exigir que `Data` esté ausente de las capas con fuentes —su intención
(declarar **qué** capa midió el control) se conserva íntegra— o, alternativamente, que la revisión declare que el
aporte de `Data` lo hace otro Bolt. Con cualquiera de las dos, este V-Bounce se reproduce completo: el retiro, las
dos autorizaciones de la revisión 2 y el inventario están **medidos y verdes** (§9.2, §9.3).

### 8.2 Lo que se midió verde antes de la reversión

| Medición | Resultado |
|---|---|
| Superficie de serialización en `2.0/src` tras el retiro | **0** residuos (`[Serializable]`, `SerializationInfo`, `ISerializable`, `GetObjectData`) |
| `Cooperator.Framework.Core` (Rebuild, Release) | **`EXIT=0`**, 0 diagnósticos — con la directiva de §A.3 en los 2 archivos que la necesitaban |
| `Cooperator.Framework.Data` (Rebuild, Release) | **`EXIT=0`**, 0 diagnósticos — con la arista `Data → Library` de §B.4 |
| `Cooperator.Framework.Library` (Rebuild, Release) | **`EXIT=0`**, 0 diagnósticos |
| Archivos con la directiva de §A.3 (aportados por este Bolt) | **2**: `BaseObject.cs` y `DescriptionFieldEventArg.cs` — `ObjectStateHelper.cs` no produjo diagnósticos |
| Archivos **nuevos** con la directiva | **0** (las pruebas y el inventario conservan la nulabilidad activa) |
| `.csproj` con `<Nullable>disable</Nullable>` **declarado** | **0** (una coincidencia de la búsqueda por texto era el **comentario** del `.csproj` de pruebas del Bolt 003 que *menciona* la etiqueta para explicar que no la usa: no es una declaración) |

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 2`, `review.decision: approved`
(`2026-10-16T02:15:49-03:00`); el Bolt entró en `blocked` con T01–T05 presentes y se tomó con **T06**; los ADR
aplicados siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.5). No hay cambio material → G15 no se activa.

### 8.4 Comprobación de las condiciones de parada (§15)

- **#1 (cambio material):** negativa (§8.3).
- **#2 (escape-hatch):** negativa — ningún tipo lo reclama (§9.3: 0 `ISerializable`, 0 `GetObjectData`, 0 contexto);
  la decisión negativa queda documentada con sus cinco criterios y la vía para una futura excepción es el Bolt que
  la pida.
- **#3 (un constructor transporta contexto):** negativa — 0 ocurrencias.
- **#4 (editar `1.4/`):** negativa.
- **#5 (el cierre exigiría transformar código de otro Bolt):** negativa — el cierre se aportó verbatim.
- **#6 (un gate de §9 no cierra en `pass`):** **APLICADA** — es el bloqueo de §8.1: el gate de tests no cierra
  porque una aserción **ajena** quedó obsoleta por el propio §B.2 de esta revisión; se detiene y se escala.
- **#7 (arnés ausente):** negativa.
- **#8 (superficie no inventariada):** negativa — el inventario cubrió los 32 tipos del alcance más los del borde,
  todos con dueño; no se retiró nada por analogía.

---

## 9. Verification evidence

### 9.1 El inventario «antes» (fase A) y la frontera

```
Tipos inventariados 34 (32 en alcance + 2 archivos del borde) · Declaraciones [Serializable] 39
Constructores de serialización 31 (protected ×31) · ISerializable 0 · GetObjectData 0
Constructores con contexto 0  ← precondición §A.2 PASA
Anotaciones: BaseObject.cs → [field: NonSerialized] ×3
Dueños: 32 → US-000.BOLT-007 · grafo y sus excepciones → US-000.BOLT-003 · CompilerException → US-000.BOLT-017
```

### 9.2 El retiro y la política de nulabilidad (rev.2), medidos

```
Retiro aplicado: 32 archivos (31 excepciones + BaseObject) · 3 [field: NonSerialized] fuera · 0 residuos
§A.3 por criterio: intento 1 → archivos con diagnósticos: BaseObject.cs, DescriptionFieldEventArg.cs
                   directiva aplicada a esos 2 · intento 2 → sin diagnósticos
§B.4: arista Data → Library agregada (una línea)
Builds: Core EXIT=0 · Data EXIT=0 · Library EXIT=0   (0 diagnósticos los tres)
```

### 9.3 El árbol como se deja (corrida de control, tras la reversión)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 85, Skipped: 0, Total: 85
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
Core: 28 archivos .cs · Data: 0 · Library: 3
```

### 9.4 Gates (§9)

No se declaran gates en `pass`: el V-Bounce se detuvo antes de entregar y el árbol quedó sin cambios. El gate de
**tests** es, precisamente, el que no cierra por el bloqueo de §8.1 (condición §15 #6).

### 9.5 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.6 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json`: se agregó la entrada **`v_bounces[1]`**
  (`number: 2`, `spec_revision: 2`, `execution_outcome: blocked`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23).

---

## 10. Manual interventions

**Ninguna intervención humana.** El retiro, la directiva de §A.3, la arista de §B.4, las mediciones y la reversión
fueron mecánicos. **Tampoco hubo operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol quedó **sin cambios** (§6, §9.3).
- **SPEC ejecutado:** [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md) — **revisión 2**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T02:15:49-03:00`), con **§6/A.3** (nulabilidad) y **§6/B.4** (la arista).
- **Bolt:** [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `Status timeline`,
  filas **T06** (toma) y **T07** (cierre `blocked`).
- **V-Bounce N°1:** [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md) — la medición que originó las dos
  autorizaciones de esta revisión.
- **Manifiesto:** [US-000.BOLT-007-retiro-de-serializable.json](../metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json)
  — `v_bounces[1]`.
- **El artefacto ajeno que bloquea:** `2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs`
  (`The_absence_control_states_which_layer_it_measured`), del Bolt 011.
- **Precedente de la salida mínima:** SPEC-0004 **§A.4** (la corrección de una línea de una invariante del arnés,
  autorizada por revisión material).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 2 (de US-000.BOLT-007) — `blocked` |
| SPEC ejecutado | SPEC-261016-0013 revisión **2** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~3 min de reloj (02:18:15 → 02:20:59) |
| Archivos entregados al árbol | **0** (reversión completa) |
| Archivos generados y revertidos | **32** del alcance + **2** de cierre + **1** inventario (+1 arista revertida) |
| Diagnósticos de nulabilidad tras §A.3 | **0** en los tres ensamblados |
| Directivas de §A.3 aplicadas | **2** (`BaseObject.cs`, `DescriptionFieldEventArg.cs`) — sólo las que producían diagnósticos |
| Pruebas rojas al aplicar el aporte | **2** (una mía y pendiente: la constancia de F; una ajena y obsoleta: la de Bolt 011) |
| Tests corridos como control | **168** (85 + 83), todos verdes tras la reversión |
| Condición de parada | §15 **#6** (un gate de §9 no cierra en `pass`) por una aserción ajena que el §B.2 invalida |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |

---

## 13. Pending items and stubs

1. **El bloqueo (§8.1):** la aserción de `HostConfigurationAbsenceTests` (Bolt 011) que exige que `Data` **no** esté
   entre las capas con fuentes. Salida mínima: **una línea**, con su comentario de procedencia, o la declaración de
   que el aporte de `Data` es de otro Bolt.
2. **Lo demás está medido y se reproduce en minutos** (§9.2): el retiro de los 32 archivos, el inventario con dueños,
   la directiva de §A.3 por criterio y la arista de §B.4. Al desbloquearse, faltan sólo:
   - **Fase D**: `BinaryTrailTests.cs` — el rastro binario (`BinaryFormatter`, `IFormatter`, `ObjectStateFormatter`,
     `NetDataContractSerializer`, `ISerializable`, `GetObjectData`), la **allowlist** del migrador
     (`Migration/Conversion/LegacyReader.cs`, que hoy no existe ⇒ el control exige **0** ocurrencias) y las
     supresiones acotadas (`#pragma warning disable SYSLIB0051` sólo dentro de la allowlist, en el miembro, con la
     cita y el vencimiento).
   - **Fase E**: `SerializationSurfaceTests.cs` — el gate por **superficie** con `MetadataLoadContext` sobre `Core`,
     `Data`, `Library` y `Modeling` (0 atributos, 0 `ISerializable`, 0 `GetObjectData`, 0 constructores de
     serialización) **más** la verificación positiva de la jerarquía (bases abstractas, constructores públicos, la
     forma que hace posible un `catch (CoreException)` idéntico) y la comparación contra el inventario (una remoción
     no declarada **falla**).
   - **Fase F**: `2.0/docs/api-removals/US-000.BOLT-007.json` — una entrada por tipo con lo que desaparece (el
     atributo y el constructor, con firma), por qué (ADR-008 §3.2/§3.4) y con qué se reemplaza («ninguno: el
     framework no expone serializador», ADR-008 §3.3) — y, al escribirlo, cubrir las diferencias que el gate reporta
     como `CP0002` (los constructores de serialización de las excepciones de `Core` y `Library`, ya medidas).
3. **La decisión negativa del escape-hatch (AC-6)** queda documentada acá: ningún tipo lo reclama y los cinco
   criterios no se cumplen para ninguno; la vía para una futura excepción es el Bolt que la pida con esa evidencia.
4. **Frontera:** los tipos del grafo (Bolt 003, ya POCOs en `Modeling`), `CompilerException` (Bolt 017), los
   `[DataContract]` de los `*.Auto.cs` (el set), `Snapshot`/`BinaryFormatter` del Modeler (Bolts 002/025) y el
   cargador del migrador (Bolt 002, la única excepción declarada).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto (número 2, revisión 2) + el árbol **sin
  cambios** (§9.3) + las mediciones del retiro y de las dos autorizaciones de la revisión 2 (§9.2).
- **Qué se pide decidir:** la salida mínima para la aserción ajena de §8.1 (una línea en
  `HostConfigurationAbsenceTests`, o la declaración de dueño del aporte de `Data`), que desbloquea las fases D–F.
