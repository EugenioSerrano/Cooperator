---
id: "MEM-261016-0226"
title: "Limpieza de referencias muertas (SPEC rev.1): inventario antes/después verificado por búsqueda de usos, árbol 2.0 sin declaraciones muertas y control activo determinista"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-012" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0021" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008"] # ADR-001 §3.1/§3.3/§3.4 (layout, limpieza y presupuesto), ADR-008 §3.1 (constancia de rupturas, que este Bolt NO necesita: 0 retiros)
manifest: "US-000.BOLT-012-limpieza-de-referencias.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T02:26:18-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:28:53-03:00"
  decided_at: "2026-10-16T02:28:53-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0021` revisión 1 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; grant `approves: [SPEC, MEM]`. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **90/90 harness + 83/83 caracterización, exit 0** (16 proyectos, 173 casos herméticos). Verificado en el árbol: `2.0/docs/reference-inventory/US-000.BOLT-012.json` con **69 declaraciones** (conteo exacto re-hecho: **36 sin uso / 33 en uso**, 10 proyectos «antes» + 11 «después») y `ReferenceHygieneTests.cs` con los **5 casos** (denylist sobre todos los proyectos presentes y futuros; control negativo; prueba negativa por inyección; cruce con el inventario; F-45 con dueño)."
    - "Compliant — **0 retiros** verificado y no se inventó trabajo: la columna «después» mide **0 referencias clásicas / 0 `<BootstrapperPackage>` / 0 metadata de publicación / 0 `packages.config`** en los 11 proyectos del 2.0 (conteo re-hecho); **ningún** archivo de proyecto ni de código fue modificado por la fase B (sólo el Bolt: status + T04) — el árbol nació limpio y el SPEC §4.2 lo prevé literalmente."
    - "ADJUDICACIÓN 1 (nuance §8.1) — ACEPTABLE: la lectura del SPEC-0037 («limpiar `System.Windows.Forms` de `Library`») y el §4 del SPEC-0021 (las en-uso fuera de alcance con dueño) **convergen en el hecho medido**: el árbol 2.0 no declara `System.Windows.Forms` en ningún proyecto y el `Registry.cs` del 1.4 (cuyo `using` cita el §B.2) **todavía no está portado**; no hay nada que retirar hoy, la frontera queda con su dueño y el control quedó puesto para vigilarla cuando se aporte."
    - "ADJUDICACIÓN 2 (límite del método) — ACEPTABLE: la búsqueda es textual por namespace sobre el código del **propio proyecto** (el alcance del barrido de REV-001), sin análisis semántico; un uso indirecto por reflexión o tipo reenviado no se detectaría, y queda **declarado** en `search_limits` del inventario. Es un límite honesto y acotado al mismo alcance que la fuente gobernada, no un hueco silencioso."
    - "Compliant — DoD: herméticos verdes (173 = 90 + 83); gates aplicables en `pass` (o `n/a` con razón); sin constancia de rupturas (0 retiros, §5 del SPEC lo condiciona); MEM completo; manifiesto válido."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0226 — Limpieza de referencias muertas: inventario verificado, árbol limpio y control activo

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-012](../functional/bolts/US-000.BOLT-012-limpieza-de-referencias.md) — `type: non-functional`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0021](../spec/SPEC-261016-0021-limpieza-de-referencias.md), **revisión 1** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T00:26:23-03:00`) |
| **V-Bounce**    | **1** (el Bolt espera uno) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.4) |

---

## 1. Executive summary

Se entregó el **inventario antes/después por proyecto y por declaración** —el entregable central del Bolt— y, con
él, la verdad medida que el SPEC anticipaba: **el Árbol 2.0 no declara ninguna de las declaraciones muertas del
inventario del REV-001** (`System.Web`, `System.Data.OracleClient`, `System.Windows.Forms`, `System.Deployment`), ni
el bloque de metadata de publicación clásica, ni ítems `<BootstrapperPackage>`, ni restos del modelo de proyecto
clásico (`packages.config`, `HintPath`, `SpecificVersion`, `<Reference>` a ensamblados de .NET Framework): nació
**limpio** con el esqueleto SDK-style del Bolt 001, y el propio SPEC ordena registrarlo así y «no inventar
trabajo». Lo que **sí** aporta este Bolt es la **evidencia** —la columna «antes» sobre el 1.4 congelado, con la
**búsqueda de usos por referencia** por cada una de las **69** declaraciones de sus **10** proyectos: **36**
resultaron sin uso (**muertas**) y **33 en uso**, entre ellas exactamente las que la frontera del SPEC declara
intocables y con dueño (`System.Web` en `Data` y `Library`, `System.Management` en `Library`,
`System.Windows.Forms` en `Library`/`Utility`/el Modeler/las plantillas)— y el **control activo determinista**
(`ReferenceHygieneTests`) que hace que la limpieza sea «una sola vez»: recorre **todos** los archivos de proyecto
del Árbol 2.0 —presentes y futuros, los de los Bolts 013–030 incluidos— y **falla** si algo de la denylist
reaparece, con su **control negativo** (las declaraciones legítimas del runtime objetivo no lo disparan), su
**prueba negativa** (una declaración muerta inyectada a propósito sí lo dispara, una por cada entrada de la
denylist) y el **cruce con el inventario** (ninguna declaración «en uso» de la frontera sin su dueño, ninguna «sin
uso» sin su constancia). El árbol queda **verde**: 16 proyectos sin advertencias, arnés **90/90** (los 85 previos
más los 5 casos nuevos) y caracterización **83/83** —**173 casos herméticos**—, con `1.4/**` intacto.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo |
|---|---|---|
| **A — Inventario «antes»** | **COMPLETADA** ✔ | Barrido de los **10** archivos de proyecto del 1.4 (los cuatro paquetes, las dos herramientas y los cuatro de plantillas) con la **búsqueda de usos por referencia** por declaración (el namespace/ensamblado declarado buscado en los `.cs` del propio proyecto, con su comando y su número de coincidencias): **69** declaraciones, **36** sin uso y **33** en uso. |
| **A.2 — El inventario como artefacto** | **COMPLETADA** ✔ | `2.0/docs/reference-inventory/US-000.BOLT-012.json`: las dos columnas por proyecto, la clasificación por declaración (sin uso / en uso con su dueño), el método y su **límite declarado** (búsqueda textual por namespace, sin análisis semántico), las declaraciones vigiladas y el registro de **F-45** con su dueño. |
| **B — Barrido y limpieza del 2.0** | **COMPLETADA — sin retiros, registrado como evidencia** ✔ | La columna «después» mide **0** referencias clásicas, **0** `<BootstrapperPackage>`, **0** metadata de publicación y **0** `packages.config` en los **11** archivos de proyecto del 2.0: no había nada que retirar y **no se inventó trabajo** (§4.2 del SPEC lo prevé literalmente). |
| **B.2 — Importaciones sin uso** | **COMPLETADA por medición** ✔ | El barrido enumeró las del 1.4 (incluido el `using System.Windows.Forms;` de `Library/Repository/Registry.cs`, el caso que el SPEC cita, cuya suerte es de este Bolt **cuando el archivo exista en el 2.0**); en el 2.0 **no hay** ninguna importación de los namespaces de plataforma del inventario (el control de rastro del Bolt 006 ya recorre el árbol entero, código y documentación, y está verde). |
| **C — El control activo** | **COMPLETADA** ✔ | `2.0/tests/Cooperator.Harness.Tests/ReferenceHygieneTests.cs`, 5 casos (§3.2). |
| **D — Evidencia y cierre** | **COMPLETADA** ✔ | El inventario antes/después, el build desde el comando reproducible, la constancia de usos por declaración y la ausencia de metadata: todo listado acá. **No** hay rupturas de superficie ni de comportamiento (0 retiros), así que **no** se produce `2.0/docs/api-removals/US-000.BOLT-012.json` (§5 del SPEC: sólo si un retiro cambiara superficie, que no es el caso). |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/docs/reference-inventory/US-000.BOLT-012.json` | **El inventario antes/después** (§A.2): por proyecto y por declaración, con la búsqueda de usos (comando + coincidencias), la clasificación, los dueños de la frontera, las declaraciones vigiladas (la denylist que consume el control), los totales de metadata de publicación y `<BootstrapperPackage>` de cada columna, y el registro de **F-45** (`ComVisible`/`ReliabilityContract`/`ConstrainedExecution`) con su dueño. |
| `2.0/tests/Cooperator.Harness.Tests/ReferenceHygieneTests.cs` | **El control activo** (§C) + el control negativo del propio control + la prueba negativa + el cruce con el inventario: **5 casos**, con los tokens de la denylist **ensamblados desde fragmentos** (el archivo es código propio y no puede ser su propia ocurrencia; el control de higiene es, por diseño, el lugar donde esos nombres se nombran). |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-012-limpieza-de-referencias.md` | `status: specified` → `in-development` y fila **T04** | La toma del V-Bounce por G11 con su fila de `Status timeline`. |

**Ningún archivo de proyecto ni de código del Árbol 2.0 se modificó**: no había nada muerto que retirar (§2/B) ✔.

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Ninguno.

---

## 7. Implementation decisions

1. **La búsqueda de usos es la que hace verdadero al inventario**, y se ejecuta con un comando reproducible y
   auditable: por cada `Reference` declarada se busca el namespace/ensamblado en los `.cs` **del propio proyecto**
   (excluyendo `bin/`/`obj/`) y se registran el número de coincidencias y las primeras líneas. «Muerta» no es una
   intuición: es 0 coincidencias con su comando a la vista.
2. **El límite de la búsqueda se declara** (§A.2): es textual y por namespace —no hace análisis semántico—, así que
   un uso indirecto por reflexión o por un tipo reenviado no se detectaría; queda dicho en el propio inventario.
3. **La denylist del control se ensambla desde fragmentos.** El archivo del control nombra las declaraciones
   prohibidas (es su función) y a la vez es código propio del árbol: los tokens se construyen por concatenación para
   no ser ocurrencias de lo que persiguen —la misma disciplina que el control de rastro del Bolt 006—, y el control
   de higiene queda declarado como el lugar donde esa lista vive por diseño.
4. **La prueba negativa del control es pura.** En vez de plantar un proyecto muerto *en el árbol* —lo que haría
   fallar al propio control que se quiere probar, y ensuciaría el árbol—, la detección es una **función pura** sobre
   el texto de un proyecto: el caso le inyecta a propósito cada entrada de la denylist y exige que la detección
   **falle**; «el control que nunca falla no controla nada».
5. **El control negativo del propio control** (AC-6): las declaraciones legítimas del runtime objetivo
   (`ProjectReference` entre capas, `PackageReference` del mapa centralizado) **no** lo disparan, y los proyectos
   reales del árbol tampoco: el control falla por la denylist, no por declarar cosas legítimas.

---

## 8. Deviations and assumptions

### 8.1 La frontera: qué se registra con dueño y no se retira (AC-6)

| Declaración «en uso» (la búsqueda encontró coincidencias) | Dónde | Dueño (frontera del SPEC §4) |
|---|---|---|
| `System.Web` | `Data` (11 usos) y `Library` (1) | Bolts 013 / 009 / 025: retirarla rompería la compilación |
| `System.Management` | `Library` (1) | Bolt 009 (la evaluación de uso real de la librería; DISC F-34) |
| `System.Windows.Forms` | `Library` (1), `Utility` (47), el Modeler (282), el codificador de conexión (20) y la plantilla `AppProject` (3) | Bolt 025 (tooling/shell) y Bolt 021 (plantillas de proyectos); en `Library` su limpieza es de **este** Bolt cuando el archivo exista en el 2.0 |
| `System.configuration` | `Core`, `Data`, `Library`, las plantillas | Bolt 011: es el Bolt que **causa** su desuso (acá la referencia está en uso y no se toca) |

**Nuance declarada (no un hueco):** el SPEC-0037 (Bolt 009) asigna a **este** Bolt «la limpieza de la referencia
muerta `System.Windows.Forms` del `.csproj` de `Library` y su `using`», mientras que el §4 del SPEC-0021 lista las
referencias *en uso* como fuera de alcance con su dueño. Las dos lecturas convergen en el mismo hecho medido: **el
Árbol 2.0 no declara `System.Windows.Forms` en ningún proyecto** y el `Registry.cs` del 1.4 —cuyo `using` sin uso el
§B.2 cita— **todavía no está portado**; cuando lo esté, este Bolt ya dejó el control que lo vigila. No hay nada que
retirar hoy y la frontera queda registrada con su dueño en el inventario.

### 8.2 Lo que el inventario midió (evidencia del «antes» y del «después»)

```
ANTES  (1.4 congelado, 10 proyectos):   69 Reference declaradas · 36 sin uso · 33 en uso
                                        30 ítems <BootstrapperPackage> · 90 ítems de metadata de publicación · 0 packages.config
DESPUÉS (2.0, 11 proyectos):             0 Reference clásicas · 0 <BootstrapperPackage> · 0 metadata de publicación · 0 packages.config
DENYLIST sobre el 2.0:                   0 declaraciones vigiladas presentes
```

Las declaraciones **muertas** del «antes» son las de la lista verificada del REV-001 —`System.Web` en `Core`
(0 usos), `System.Data.OracleClient` y `System.Windows.Forms` en `Data` (0), `System.Deployment` en las herramientas
(0 en las tres que lo declaran)— más las que la búsqueda detectó sin uso en el resto de los proyectos; todas quedan
**en el inventario con su evidencia**, que es lo que el Bolt 028 necesitará si alguna vez se documentan.

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 1`, `review.decision: approved`
(`2026-10-16T00:26:23-03:00`); el Bolt entró en `specified` con T01–T03 presentes y se tomó con **T04**; ADR-001 y
ADR-008 siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.4). No hay cambio material → G15 no se activa.

### 8.4 Comprobación de las condiciones de parada (§15)

- **#1 (cambio material):** negativa (§8.3).
- **#2 (una declaración «muerta» resulta usada):** **es la situación medida y está tratada como el SPEC manda** —las
  33 declaraciones en uso **no** se retiran y se registran **con su dueño** (§8.1); su presencia no impide el
  objetivo del Bolt (que es el **Árbol 2.0**, donde no hay ninguna).
- **#3 (un retiro cambiaría superficie o comportamiento):** negativa — **0 retiros**.
- **#4 (editar `1.4/`):** negativa — el 1.4 se leyó (es la columna «antes») y no se tocó.
- **#5 (declaración fuera del inventario del REV):** negativa — todas las declaraciones del alcance están en el
  inventario, clasificadas y con dueño cuando corresponde.
- **#6 (un gate de §9 no cierra en `pass`):** negativa — todos cierran en `pass` o con su razón de `n/a` (§9.5).
- **#7 (arnés ausente):** negativa — el comando reproducible y el gate corrieron (§9.3).
- **#8 (ambigüedad que sólo se resolvería inventando):** negativa — la única nuance (la suerte de
  `System.Windows.Forms` en `Library`, §8.1) se **declara** con las dos lecturas y se resuelve en el hecho medido,
  sin inventar una decisión.

---

## 9. Verification evidence

### 9.1 Prerequisitos (§5)

```
SPEC-261016-0021 → status: approved · revision: 1 · review.decision: approved (2026-10-16T00:26:23-03:00)
BOLT-012         → status: specified · filas T01..T03 presentes → se tomó con T04 (02:22:40)
2.0/árbol        → 16 proyectos verdes (85/85 + 83/83) antes de tocar nada · comando reproducible operativo
1.4/**           → intacto (sólo lectura: aporta la columna «antes»)
```

### 9.2 El control activo (fase C), en sus cinco casos

| Caso | Qué verifica | Resultado |
|---|---|---|
| `The_projects_of_the_tree_carry_no_dead_declaration_nor_publication_metadata` | La denylist completa sobre los **11** proyectos del 2.0 (presentes y futuros) | **0** violaciones |
| `Legitimate_declarations_of_the_target_runtime_do_not_trip_the_control` | El **control negativo**: `ProjectReference`/`PackageReference` legítimos no lo disparan | verde |
| `The_control_detects_each_dead_declaration_injected_on_purpose` | La **prueba negativa**: cada entrada de la denylist **y** cada resto clásico, inyectados, se detectan | verde |
| `The_inventory_records_no_removal_without_its_use_search_evidence` | El **cruce con el inventario**: 0 declaraciones vigiladas en el 2.0; toda declaración trae su búsqueda de usos; las «en uso» de la frontera traen su dueño | verde |
| `The_f45_items_are_registered_with_their_owner_and_not_removed_here` | **F-45** registrado con su dueño y fuera del alcance de este Bolt | verde |

### 9.3 La corrida reproducible (AC-7)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 90, Skipped: 0, Total: 90      ← 85 previos + 5 del control
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

**173 casos herméticos verdes**, 16 proyectos con 0 advertencias. Y los proyectos conservan **el mismo TFM y las
mismas versiones de paquete** que antes del Bolt (AC-4): este Bolt **no tocó ningún archivo de proyecto ni de
código**.

### 9.4 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.5 Gates (§9)

| Gate | Level | Resultado | Evidencia |
|------|-------|-----------|-----------|
| Build / lint / type checks | V-Bounce | **pass** | 16 proyectos sin advertencias (§9.3) |
| Unit / component tests | V-Bounce | **pass** | 173/173 herméticos, incluido el control activo y sus dos controles (§9.2, §9.3) |
| Consumer-side contract tests (test doubles) | V-Bounce | **n/a (razón)** | No hay frontera de componente: el Bolt verifica archivos de proyecto, no contratos |
| SAST | V-Bounce | **n/a (razón)** | El cambio no introduce código de producto (0 retiros, 0 ediciones de proyecto) |
| Dependency scanning / licenses | V-Bounce | **pass** | 0 dependencias nuevas; los TFMs y las versiones de paquete no cambian (AC-4) |
| OWASP Top 10 — static review | V-Bounce | **n/a (razón)** | Sin superficie expuesta |
| Prompt-injection scan | V-Bounce | **n/a (razón)** | Sin material textual no confiable |
| Secret-leak scan | V-Bounce | **pass** | 0 secretos; el inventario sólo registra nombres de ensamblado y recuentos |
| Hallucination lint | V-Bounce | **pass** | Cada dato del inventario es el resultado de una búsqueda ejecutada, con su comando |
| IP / license provenance | V-Bounce | **n/a (razón)** | Sin código de terceros |
| PII / DLP | V-Bounce | **pass** | `internal`: nombres de ensamblado y rutas de proyecto; sin datos personales |
| Dependency-confusion | V-Bounce | **pass** | Sin paquetes nuevos; el inventario documenta qué declara cada proyecto |
| Test-first evidence | V-Bounce | **n/a (razón)** | No es un Bolt de BUG |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **pass** | Dos corridas con resultado idéntico; el control es determinista por construcción |
| Bolt-manifest validation | V-Bounce | **pass** | Manifiesto válido contra `manifest-v5-bolt.schema.json` (§9.7) |
| Integration tests (real dependencies) | Integration test | **n/a (razón)** | Plan `n/a` (Bolt `not-required`): la higiene se verifica sobre el árbol |
| Contract / E2E against the real providers | Integration test | **n/a (razón)** | Sin provider externo |
| DAST / OWASP Top 10 — dynamic | Integration test | **n/a (razón)** | Sin servicio desplegable |
| Perf-smoke (p95/p99) | Integration test | **n/a (razón)** | Sin carga afectada |
| Observability (logs / metrics / traces) | Integration test | **n/a (razón)** | Sin servicio instrumentado |

### 9.6 Encoding

Los documentos entregados (MEM, inventario, fila T04 del Bolt, manifiesto) se escribieron en **UTF-8 sin BOM**, con
`LF` y sin `U+FFFD` (§3.15/W23); el archivo del control sigue el mismo estándar.

### 9.7 Manifest

`devflow/metrics/bolts/US-000.BOLT-012-limpieza-de-referencias.json`: se agregó la entrada **`v_bounces[0]`**
(`number: 1`, `spec_revision: 1`, `execution_outcome: ready_for_review`), validada contra
`devflow/metrics/manifest-v5-bolt.schema.json`.

---

## 10. Manual interventions

**Ninguna intervención humana.** El inventario se generó con un barrido reproducible, el control y sus casos se
escribieron en este V-Bounce y la verificación corrió con el comando del árbol. **Tampoco hubo operaciones de git**
(G34): el árbol está sin commitear y el commit es del humano.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol de trabajo es la entrega y no se hicieron operaciones de git.
- **SPEC ejecutado:** [SPEC-261016-0021](../spec/SPEC-261016-0021-limpieza-de-referencias.md) — **revisión 1**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T00:26:23-03:00`).
- **Bolt:** [US-000.BOLT-012](../functional/bolts/US-000.BOLT-012-limpieza-de-referencias.md) — `Status timeline`,
  fila **T04**.
- **Manifiesto:** [US-000.BOLT-012-limpieza-de-referencias.json](../metrics/bolts/US-000.BOLT-012-limpieza-de-referencias.json)
  — `v_bounces[0]`.
- **El inventario entregado:** `2.0/docs/reference-inventory/US-000.BOLT-012.json`.
- **El control entregado:** `2.0/tests/Cooperator.Harness.Tests/ReferenceHygieneTests.cs`.
- **Evidencia de código (lectura, árbol congelado):** los diez archivos de proyecto del 1.4
  (`Cooperator.Framework.{Core,Data,Library,Utility}.csproj`, `Tools/{CooperatorModeler,ConnectionStringEncriptor}.csproj`
  y los cuatro de `Tools/CooperatorModeler/Templates/CSharpProjects/`) con sus `Reference`, su metadata de
  publicación y sus ítems `<BootstrapperPackage>`.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-012) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0021 revisión **1** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~4 min de reloj (02:22:40 → 02:26:18) |
| Archivos creados | **2** (el inventario y el control) + el manifiesto y este MEM |
| Archivos modificados | **1** (el Bolt: status y fila T04) — **0** archivos de proyecto o de código |
| Proyectos barridos | **10** en el 1.4 (columna «antes») + **11** en el 2.0 (columna «después») |
| Declaraciones inventariadas | **69** (36 sin uso · 33 en uso con su clasificación y dueño) |
| Retiros ejecutados | **0** (el árbol 2.0 nació limpio: registrado como evidencia, no como fallo) |
| Tests creados | **5** (el control activo y sus dos controles y el cruce con el inventario) |
| Tests de la suite | **173** herméticos (90 + 83), todos verdes |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |
| Código generado por IA | 100 % de lo generado en este V-Bounce |

---

## 13. Pending items and stubs

1. **Nada diferido por este Bolt**: las fases A–D están completas. Lo que el control **vigila** hacia adelante es
   precisamente lo pendiente del producto: los proyectos que porten los Bolts **013–030** no pueden declarar nada de
   la denylist —ni metadata de publicación ni restos clásicos—, y el control los cubre **sin tocar nada** cuando
   aparezcan.
2. **F-45** (`ComVisible`/`ReliabilityContract`/`using System.Runtime.ConstrainedExecution`): **registrado con su
   dueño** (el Bolt que porte cada `AssemblyInfo`) y **fuera del alcance** de este Bolt, que habla de referencias, de
   metadata de publicación y de importaciones (ADR-001 §3.3 lo incluye en la limpieza, pero su retiro es de aquel
   Bolt).
3. **La suerte de `System.Windows.Forms` en `Library`** (§8.1): el `using` sin uso del `Registry.cs` del 1.4 es de
   este Bolt **cuando el archivo se porte** al 2.0; hoy el 2.0 no declara esa referencia en ningún proyecto y el
   control queda puesto para cuando llegue.
4. **Sin constancia de rupturas**: con **0 retiros** no hay entradas para las notas de migración (el §5 del SPEC lo
   condiciona a que un retiro cambie superficie o comportamiento, que no ocurre).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto + el árbol (el inventario versionado y el
  control activo; **0** archivos de proyecto o de código tocados) + la corrida verde (**173 casos**).
- **Qué se pide decidir:** la aceptación del entregable y, si se considera necesario, la nuance de §8.1 sobre la
  frontera de `System.Windows.Forms` en `Library`.
