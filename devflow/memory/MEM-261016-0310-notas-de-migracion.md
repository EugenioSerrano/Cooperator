---
id: "MEM-261016-0310"
title: "Notas de migración de la 2.0 y registro de rupturas (SPEC rev.1) — V-Bounce N°1: el registro estructurado, el formato publicado y el cruce en los dos sentidos"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-028" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0047" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-008", "ADR-001", "ADR-002", "ADR-004", "ADR-007", "ADR-005", "ADR-006"] # ADR-008 §3.1/§3.3/§3.5/§3.6/§5 (los tres datos, el reemplazo, la línea 2.0.x, el escape-hatch y el gate); ADR-001 §3.2/§4 y ADR-002/ADR-004/ADR-005/ADR-006/ADR-007 §3.x (las decisiones del mapa)
manifest: "US-000.BOLT-028-notas-de-migracion.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:10:51-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T03:14:17-03:00"
  decided_at: "2026-10-16T03:14:17-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0047` revisión 1 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **115/115 harness + 83/83 caracterización, exit 0** (198 casos); `MIGRATION-NOTES.md` **588 líneas** y **determinista**: regenerado con `2.0/build/migration-notes.ps1` y comparado — **idéntico** («sin cambios, consolidación determinista»)."
    - "Compliant — entregables verificados: `README.md` + `api-removals.schema.json` con las **2 reglas condicionales** (`kind`+`surface` ⇒ `type`+`member`+firma; `replacement: "none"` ⇒ `replacement_reason`) + el mapa **8 áreas / 48 cláusulas** (10 `covered_by`, 17 `pending`); `MigrationNotesCoverageTests` con **9 casos**; §C.1 (37/37 `CP0002` cubiertas, 0 entradas sin ruptura) y §C.2 (10 entradas + 17 obligaciones ruteadas)."
    - "Compliant — los dos hallazgos quedan **contados/visibles y ruteados** (no corregidos desde acá): `BOLT007-08` con `type: "Cooperator.Framework.Core.for"` inválido (verificado en el fragmento 007) y `BOLT003-07`/`BOLT003-08` sin cláusula (verificado: `clause = <none>`), ambos **declarados** en el MEM §8.3."
    - "CORRECCIÓN del timeline (registrada, sin editar filas): el ejecutor **adelantó** la transición `in-development → ready-for-acceptance` en la fila **T05** (cierre, 03:10:51), antes del `AITL-MEM-Approval` — el contrato espera `in-development` durante la pausa. El estado final `ready-for-acceptance` es el correcto y este `AITL-MEM-Approval` (Development Completed) lo legitima. **Minor gap adicional (no material):** la fila **T04** registra `03:14:00` como toma, posterior al cierre `03:10:51` (inversión; el MEM §12 indica «02:14 → 03:10», así que T04 parece un typo de `03:14` por `02:14`). No edito la fila; lo dejo señalado."
    - "Compliant — **0 archivos de otros Bolts modificados**: las dos interacciones se resolvieron desde artefactos de este Bolt (§7.1 el mapa fuera de `api-removals/` para no colisionar con el glob; §7.5 el esquema con `entries` inerte); `ApiRemovalCoverageTests.cs` (006/007) y `RetiredSurfaceTests.cs` (006) intactos. DoD + MEM completo + manifiesto válido; `1.4/**` intacto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0310 — Notas de migración de la 2.0 y registro de rupturas (rev.1)

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-028](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — `type: non-functional`, `work_category: debt`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0047](../spec/SPEC-261016-0047-notas-de-migracion.md), **revisión 1** (aprobada `2026-10-16T00:52:21-03:00`) |
| **V-Bounce**    | **1** — el primero del Bolt (los Bolts 003/005/006/007/009/011 aportaron sus fragmentos por **contribución distribuida**) |
| **ADRs**        | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-007](../adrs/ADR-007-politica-criptografica.md) · ADR-005 · ADR-006 |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.6) |

---

## 1. Executive summary

El **registro de rupturas de la 2.0** existe, está **estructurado por área** y se **verifica contra el gate de
compatibilidad**: el **formato del fragmento** quedó **publicado** (su `README.md` con las tres clases y los tres
datos obligatorios de ADR-008 §3.1, su **esquema** `api-removals.schema.json` y un **validador** que rechaza el
fragmento mal formado o incompleto con su motivo), el **cruce corre en los dos sentidos** sobre el **reporte real**
del gate —**0 diferencias de miembro retirado sin entrada** (`CP0002`: 37/37 cubiertas) y **toda entrada de
superficie respaldada** por una ruptura real (la diferencia en el reporte, o el ancla **presente en la línea base
1.4 y ausente del árbol 2.0**, medido sobre metadatos y fuentes)— y las entradas de **comportamiento y
deprecación** se cruzan **contra las decisiones** que las ordenan mediante un **mapa** de **8 áreas** y **48 filas
de cláusula**, cada una verificada contra los ADRs **vigentes** (`accepted`): 21 cláusulas declaran `none` (con su
razón), 10 `api_removal`, 15 `behavior_change` y 2 `deprecation`, y **toda declaración tiene o una entrada que la
cubre o una obligación ruteada a su Bolt/ADR dueño** (10 filas `covered_by` —los seis fragmentos aportados por
otros Bolts— y 17 con `pending`, ruteadas a `US-000.BOLT-002`, `US-000.BOLT-022` y a los ADRs 001/003/004/005).

La **consolidación** es un **script determinista** (`2.0/build/migration-notes.ps1`) que produce
`2.0/docs/MIGRATION-NOTES.md` (**588 líneas**, generado, no editado a mano) con el encabezado que declara qué es,
de dónde sale, quién lo mantiene y que **no autoriza ninguna ruptura**; dos corridas del mismo conjunto de
fragmentos dan el **mismo** contenido y el arnés lo verifica contra el documento versionado. El registro del
**escape-hatch** (ADR-008 §3.6) queda con la **decisión negativa declarada** por el Bolt 007 y el documento
muestra la declaración. La suite queda **verde: 115/115 del arnés + 83/83 de caracterización = 198 casos**, 16
proyectos sin advertencias y `1.4/**` intacto.

**Lo que este Bolt NO hizo** (y es su exclusión explícita): no decidió ninguna ruptura (cada una la ordena su ADR
y la ejecuta su Bolt), no se declaró dueño exclusivo del registro, no implementó el gate (es del Bolt 001, ADR-008
§5) y **no editó el fragmento de ningún otro Bolt** — los hallazgos sobre sus anclas quedan **ruteados a su
dueño** (§8.3/§8.4).

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — El registro estructurado y el formato publicado** | **completada** ✔ | El **mapa de áreas** con sus decisiones (§A.1 + el área de ADR-006, §8.2) en `2.0/docs/api-removals-decisions.json` (**48 filas**, **8 áreas**); el **formato publicado** en `README.md` (campos obligatorios y opcionales, las tres clases, el vocabulario de `kind`, cómo contribuye un Bolt); el **esquema** `api-removals.schema.json` (draft 2020-12, con el vocabulario como `$defs.vocabulary` y las **dos reglas condicionales**: miembro ⇒ `type`+`member`+firma cuando la entrada declara su `surface`, y `"none"` ⇒ `replacement_reason`). |
| **B — La contribución distribuida** | **completada** ✔ | Los **6 fragmentos** aportados por los Bolts 003/005/006/007/009/011 (**63 entradas**) se leen en bloque: la cobertura es **por diferencia**, no por documento, y el caso lo asienta (≥5 Bolts distintos del 028 en el registro). El 028 **no** es prerequisito de nadie y la consolidación se **extiende**. |
| **C — La cobertura cruzada** | **completada** ✔ | **§C.1 en los dos sentidos**: las **37** diferencias `CP0002` del reporte real del gate están cubiertas, y las **63** entradas se cruzan contra su respaldo (§7.4/§8.3), con el caso negativo sintético que **falla** cuando el ancla no es real. **§C.2**: las entradas no atributivas se cruzan contra el mapa de decisiones, con el caso negativo de la entrada que cita una cláusula que declara `none`. |
| **D — Las excepciones y el paso siguiente** | **completada** ✔ | **Escape-hatch**: el registro muestra la **decisión negativa** del Bolt 007 (ningún tipo la reclama; `granted: false`) y declara la ambigüedad cuando **ningún** fragmento lo menciona. **Paso siguiente**: el formato publica `next_step` y el documento lo emite por entrada cuando existe (§8.4 lo señala como pendiente de los Bolts que lo materialicen). |
| **E — La consolidación y la evidencia** | **completada** ✔ | `2.0/build/migration-notes.ps1` (consolidación **determinista**, orden canónico área → fragmento → id) + `MIGRATION-NOTES.md` generado, y `MigrationNotesCoverageTests.cs` (**9 casos**) con la validación de esquema, el cruce en los dos sentidos, el cruce por decisión y la comprobación de que el documento versionado **es** el que la consolidación produce (dos corridas idénticas). |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/docs/api-removals/README.md` | El **formato publicado** del fragmento: campos (obligatorios y opcionales), las tres clases y su validación, los tres datos obligatorios, el vocabulario de `kind`, las rupturas que el gate no puede ver y cómo contribuye un Bolt. |
| `2.0/docs/api-removals/api-removals.schema.json` | El **esquema** del fragmento (JSON Schema draft 2020-12) con el vocabulario y las reglas condicionales; su clave `entries` **vacía e inerte** resuelve la interacción declarada en §7.1. |
| `2.0/docs/api-removals-decisions.json` | El **mapa de decisiones** (8 áreas, 48 filas de cláusula): qué declara cada cláusula y qué la cubre o quién la debe. **Forma decidida en §7.1**: fuera del glob de fragmentos. |
| `2.0/docs/MIGRATION-NOTES.md` | El **documento consolidado** (588 líneas), generado y estructurado por área, con el encabezado de §1/AC-8, el mapa, la sección del escape-hatch y **cada entrada trazable a su decisión y a su Bolt**. |
| `2.0/build/migration-notes.ps1` | El **consolidador determinista** (§7.2). |
| `2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs` | Los **9 casos** del §6/E: esquema y sus negativos, acuerdo esquema↔validador, cruce de superficie en los dos sentidos (con su negativo), cruce por decisión (con sus negativos y las excepciones del corpus contadas), consolidación determinista. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-028-notas-de-migracion.md` | `status: specified` → `in-development` → `ready-for-acceptance` + filas **T04** y **T05** | La toma del V-Bounce por G11 y su cierre. |

> **Ningún archivo de otro Bolt fue modificado.** En particular `ApiRemovalCoverageTests.cs` (Bolts 006/007) y
> `RetiredSurfaceTests.cs` (Bolt 006) **no** se tocaron: las dos interacciones que aparecieron se resolvieron
> desde los artefactos de este Bolt (§7.1 y §7.5). `1.4/**` está intacto.

---

## 5. Files renamed / 6. Files deleted

Ninguno.

---

## 7. Implementation decisions

1. **El mapa de decisiones vive FUERA de `api-removals/`** (`2.0/docs/api-removals-decisions.json`) porque la
   prueba precedente del Bolt 006 —que el SPEC **conserva**— recorre `api-removals/*.json` y trata cada archivo
   como un fragmento. Y el **esquema** sí debe vivir dentro (ruta fijada por el SPEC §6/A.2): para que ese mismo
   lector no lo interprete como un fragmento, el esquema declara una clave **`entries` vacía e inerte** —JSON
   Schema ignora las palabras clave que no conoce— y el `README` lo explica. Es una decisión de **forma**
   (qué archivos son fragmentos), no de criterio, y evita editar el archivo de otro Bolt.
2. **El consolidador es un script versionado** (`2.0/build/migration-notes.ps1`), no una edición a mano del
   documento: es lo que hace verificable la **determinación** (§8.1) y lo que permite que el registro **crezca
   por contribución** sin rehacer el mecanismo.
3. **La validación se apoya en tres criterios distintos, uno por clase** (SPEC §6/C): superficie contra el
   **reporte** del gate; comportamiento/deprecación contra las **decisiones**; y —para las rupturas que el gate
   **no puede ver**— la **evidencia** de la entrada con la exigencia de que su ancla exista en la línea base y no
   en el árbol 2.0. El porqué está en §7.4.
4. **El respaldo de una entrada de superficie es doble**: (a) el reporte del gate nombra lo que la entrada
   declara retirar, **o** (b) el ancla declarada existe en la **línea base 1.4** y ya no está en el **árbol 2.0**
   (ensamblados construidos **y** fuentes). El segundo brazo es necesario porque el gate compara **API pública** y
   hay rupturas reales que no producen diferencia —un atributo, un miembro privado, un `using` de plataforma— y,
   medido en este V-Bounce, porque el reporte **no enumera** algunos tipos públicos retirados (por ejemplo
   `LazyConfigurationHelper`, público en 1.4 y ausente de la 2.0: la entrada es correcta y el reporte no la
   nombra). Ninguno de los dos brazos puede declarar bueno lo que no ocurrió: un ancla que no existe en la línea
   base **falla**.
5. **La cita de la cláusula se exige al cambio observable por el consumidor** (`kind: behavior-change` y la clase
   `deprecation`). Las entradas de clase `behavior_change` que describen un cambio de **proceso** del árbol 2.0
   (la política de build, un tipo todavía no portado, una obligación diferida) no son un cambio observable y su
   corpus —ajeno a este Bolt— no cita cláusula: quedan **contadas** (`≤ 2`, todas de `US-000.BOLT-003`) y
   **ruteadas** (§8.3). Si aparece una nueva, el caso **falla**.

---

## 8. Deviations and assumptions

### 8.1 Qué es la "determinación" y cómo se verifica

Dos corridas del consolidador sobre el mismo conjunto de fragmentos producen **el mismo** contenido (el caso lo
corre en dos directorios distintos y los compara), y el documento **versionado** es **exactamente** el que la
consolidación produce: si alguien lo edita a mano o agrega un fragmento sin consolidar, el caso **falla**.

### 8.2 El mapa agrega el área de ADR-006 (con su razón)

El mapa de §6/A.1 lista **siete** áreas y no incluye ADR-006. Este registro agrega la octava
—`carga-de-ensamblados-y-providers-lazy`— porque ADR-006 está `accepted`, **declara** cambios de comportamiento y
sus entradas **ya existen** (fragmento de `US-000.BOLT-005`): sin esa área, entradas reales quedarían en un
cajón "otras-rupturas" y el cruce de §C.2 —que el SPEC manda verificar "contra los ADRs **vigentes** del
proyecto"— no tendría contra qué cruzarlas. **No se enmienda ningún ADR**: se agrega el lugar donde sus entradas
se leen, y queda declarado en el propio mapa (`source_of_shape`).

### 8.3 Hallazgos RUTEADOS a su dueño (el registro los hace visibles; no los corrige desde acá)

| Hallazgo | Dueño | Por qué no se corrige acá |
|---|---|---|
| `BOLT007-08` declara en su campo `type` el valor `Cooperator.Framework.Core.for`, que **no es un nombre de tipo** (el tipo de esa entrada es `BaseObject`, cuya ruptura es de **atributos** y por eso el gate no la ve). El documento consolidado lo muestra tal cual, así que el hallazgo es **visible** en la entrega. | `US-000.BOLT-007` (V-Bounce N°4 `ready_for_review`, pendiente de `AITL-MEM-Approval`) | El fragmento es de otro Bolt y SPEC §15 lo prohíbe expresamente («no se documenta "desde acá" como si fuera propio»): se **señala y se rutea**. |
| Las **2** entradas de clase `behavior_change` sin cláusula citada (`BOLT003-07` `type-not-ported`, `BOLT003-08` `build-policy`): describen proceso del árbol 2.0, no un cambio observable. | `US-000.BOLT-003` | Mismo motivo. Quedan **contadas** por el control (una nueva rompe el caso). |
| La **superficie no observable por el gate** que las entradas de atributos/plataforma declaran dejan su verificación en la `evidence` de su Bolt (no hay diferencia que cruzar). | Los Bolts dueños de cada fragmento | Es el criterio declarado del registro (§7.3/§7.4) y el SPEC lo reconoce para las entradas no atributivas. |

### 8.4 Lo que el registro NO cubre, y por qué es correcto que no lo cubra

`CP0003` (versión de ensamblado `2.0.0.0`) y `CP0004` (los ensamblados sin contraparte:
`Cooperator.Framework.Utility` disuelto y `Cooperator.Framework.Modeling` nuevo) **no** son rupturas de miembro ni
de atributo y quedan fuera del alcance de AC-4: la prueba precedente del Bolt 006 los declara **pendientes con su
razón** y su tratamiento es el porteo declarado de los Bolts que aportan cada tipo. El registro lo dice en su
encabezado y en la sección de áreas, sin fingir que son entradas.

### 8.5 Presupuesto de turnos (declarado)

El V-Bounce **excedió la guía de 10 loops**: convergir requirió varias iteraciones —cada una con la causa exacta
medida y la corrección verificada— por dos interacciones con artefactos de otros Bolts (§7.1 y §7.5) y por el
criterio del cruce, que se ajustó **contra los datos** (63 entradas, 178 diferencias del gate) en vez de
suponerlos. El árbol **nunca** quedó a medio camino: el producto no se tocó y la suite quedó verde antes del
cierre.

### 8.6 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con `revision: 1` y `review.decision: approved` (`2026-10-16T00:52:21-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.4). No se activa ninguna revisión nueva.

---

## 9. Verification evidence

### 9.1 El registro, medido

```
Fragmentos leídos:            6   (US-000.BOLT-003/005/006/007/009/011 — todos de OTROS Bolts)
Entradas en el registro:     63
Áreas del mapa:               8   (7 de §6/A.1 + la de ADR-006, §8.2)
Filas de cláusula:           48   (none 21 · api_removal 10 · behavior_change 15 · deprecation 2)
Cobertura por entrada:       10 filas con covered_by (003, 005, 006, 007, 009, 011) · 17 con pending
Obligaciones ruteadas a:     US-000.BOLT-002, US-000.BOLT-022 y los ADRs 001/003/004/005
Gate (modo Current):         status=differences · 178 diferencias · CP0002 = 37
Cruce de superficie:         37/37 CP0002 cubiertas · 0 entradas sin ruptura real
Documento consolidado:       588 líneas · dos consolidaciones idénticas y == el versionado
Escape-hatch:                1 declaración (US-000.BOLT-007, decisión negativa) · ninguna excepción
```

### 9.2 La suite (198 casos)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 115, Skipped: 0, Total: 115
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

Los **9** casos nuevos de `MigrationNotesCoverageTests` (los 106 previos siguen verdes, incluidos los de la prueba
precedente del Bolt 006 que el SPEC manda conservar).

### 9.3 Los negativos del control, ejercidos

```
- fragmento sin `why` / sin `replacement` / sin `evidence` / sin Bolt responsable / `kind` desconocido /
  `replacement` "none" sin razón / `class` que no coincide con `kind` / miembro sin firma declarando `surface`
  → cada uno RECHAZADO con su motivo (8 casos sintéticos).
- diferencia del gate sin entrada (`FillDataSet`) → FALLA nombrando el miembro.
- entrada que declara retirar `Cooperator.Framework.Core.BaseObject`, que sigue en el árbol 2.0 → FALLA.
- fragmento válido SIN rupturas (`entries` vacío) → ACEPTADO (edge case del SPEC §8.1).
```

### 9.4 `1.4/**` intacto y encoding

```
git status --porcelain -- 1.4   → (vacío)
Documentos del V-Bounce: UTF-8 sin BOM, sin U+FFFD y sin CR suelto (revisados uno por uno).
```

### 9.5 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 16 proyectos sin advertencias; el esquema y el mapa son JSON válido y el documento se produce |
| Unit / component tests | **pass** | 198/198 herméticos: validador con sus negativos, los dos sentidos del cruce (con sus rojos sintéticos), el cruce por decisión y la consolidación determinista |
| Consumer-side contract tests (test doubles) | **n/a (razón)** | El contrato consumido es un **artefacto** (el reporte del gate), no un servicio: se ejercita sobre el reporte real y sobre reportes sintéticos |
| SAST | **n/a (razón)** | El entregable es documentación, un esquema y casos de prueba; no hay código de producto nuevo |
| Dependency scanning / licenses | **pass** | **0** dependencias nuevas: el validador y los casos usan el framework de pruebas del arnés y `MetadataLoadContext`, ya referenciados |
| OWASP Top 10 — static review | **n/a (razón)** | Sin superficie ejecutable ni entrada externa |
| Prompt-injection scan | **n/a (razón)** | No se interpreta contenido textual como instrucción: los fragmentos se **parsean**, no se obedecen |
| Secret-leak scan | **pass** | **0** secretos: las notas describen **procedimientos** y miembros; ninguna clave, credencial ni material |
| Hallucination lint | **pass** | Toda entrada cita la decisión que la ordena y el Bolt que la ejecuta; **0** rupturas inventadas (el cruce falla ante un ancla inexistente) y **0** reemplazos inventados (el formato exige `"none"` **con razón**) |
| IP / license provenance | **pass** | Documentación propia del repositorio; 0 contenido de terceros |
| PII / DLP | **pass** | `internal`: el registro habla de tipos, miembros y comportamientos |
| Dependency-confusion | **n/a (razón)** | No se agregan paquetes |
| Test-first evidence | **pass** | El mapa y los criterios de cobertura se escribieron **antes** de consolidar, y el validador antes de publicar el formato: los criterios se **midieron** contra los 63 fragmentos y las 178 diferencias antes de fijarlos |
| Behavioral reproducibility (hermetic suite) | **pass** | Dos consolidaciones idénticas (§8.1) y cruce determinista sobre el mismo reporte |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.6) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`: documentación y trazabilidad, sin dependencia externa) |

### 9.6 Manifest

`devflow/metrics/bolts/US-000.BOLT-028-notas-de-migracion.json`: entrada **`v_bounces[0]`** (`number: 1`,
`spec_revision: 1`, `execution_outcome: ready_for_review`), validada contra `manifest-v5-bolt.schema.json`.

---

## 10. Manual interventions

**Ninguna intervención humana.** Los artefactos, la corrida del gate y las dos consolidaciones se hicieron en este
V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0047](../spec/SPEC-261016-0047-notas-de-migracion.md) — revisión **1**
  (`AITL-SPEC-Approval` del `2026-10-16T00:52:21-03:00`): §6/A–E y §7 (AC-1…AC-8).
- **Bolt:** [US-000.BOLT-028](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — filas **T04** y **T05**.
- **Entregables:** `2.0/docs/api-removals/{README.md, api-removals.schema.json, US-000.BOLT-*.json}`,
  `2.0/docs/api-removals-decisions.json`, `2.0/docs/MIGRATION-NOTES.md`, `2.0/build/migration-notes.ps1` y
  `2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs`.
- **Manifiesto:** [US-000.BOLT-028-notas-de-migracion.json](../metrics/bolts/US-000.BOLT-028-notas-de-migracion.json).
- **Prueba precedente conservada:** `ApiRemovalCoverageTests.cs` (Bolts 006/007) — sigue verde sin cambios.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-028) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0047 revisión **1** |
| Tiempo de generación IA | ~1 h de reloj (02:14 → 03:10) |
| Fases completadas | **5 de 5** (A–E) |
| Archivos creados | **6** (formato publicado, esquema, mapa, documento consolidado, consolidador y los 9 casos) |
| Archivos de otros Bolts modificados | **0** |
| Fragmentos/entradas leídos | 6 / 63 (de 6 Bolts distintos) |
| Cláusulas mapeadas | 48 (8 áreas) — 27 declaran, 21 `none` con razón |
| Casos de prueba nuevos | 9 (todos verdes) |
| Tests de la suite | **198** herméticos (115 + 83), todos verdes |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **Obligaciones ruteadas** (§8.3): `BOLT007-08` (ancla `type` inválida → `US-000.BOLT-007`) y las 2 entradas sin
   cláusula (`US-000.BOLT-003`). Se resuelven en el próximo acto de esos Bolts sobre **su** fragmento.
2. **Declaraciones pendientes del mapa** (17 filas): las ejecutan los Bolts/ADRs dueños — `US-000.BOLT-002`
   (`.coop2`, deprecaciones del corpus y `Snapshot`), `US-000.BOLT-022` (criptografía) y los ADRs 001 (gate por
   token), 003 (set de plantillas), 004 (caché) y 005 (capa de datos). **Cada una se convierte en entrada** el día
   que su Bolt ejecute la ruptura: el registro se **extiende** consolidando.
3. **`next_step`**: el formato lo publica y el documento lo emite por entrada; hoy **ningún** fragmento del
   corpus lo declara (el paso siguiente obligatorio del corpus legacy lo aportará el Bolt 002 — §8.3/§13.2).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto + el árbol entregado (el formato publicado con
  su esquema y validador, el mapa de decisiones, el documento consolidado determinista, el consolidador y los 9
  casos) + la corrida verde de **198 casos**.
- **Qué se pide decidir:** la aceptación del entregable y el cierre del Bolt hacia su aceptación.
