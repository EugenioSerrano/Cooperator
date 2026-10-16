---
id: "SPEC-261016-0050"
title: "Ejecución del batch real del corpus de modelos: conversión con reporte por archivo, originales conservados e inventariados"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 3 (hallazgo F-41) → ADR-002 §3.2/§3.4
bolt: "US-000.BOLT-029" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-002"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y build (Bolt 001)
  - "SPEC-261016-0002-migrador-coop-a-coop2.md" # la herramienta de conversión, sus fixtures dorados y su nivel t1 (Bolt 002)
  - "SPEC-261016-0010-contrato-coop2-en-modeling.md" # el contrato del formato y su test bilateral: el estado t2 es condición de estado de esta corrida (Bolt 004)
risk_class: "high" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L2" # default por riesgo high (§3.3): más paradas y confirmaciones durante el V-Bounce
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "confidential" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: not-required` → `n/a`
  applicability: "n/a"
  tests: []
  environment: ""
  data: ""
  reason: "Bolt `not-required` (refactor): convierte los modelos del repositorio con verificación por digests; el resultado no toca base de datos, cola ni servicio externo."
  waiver_adr: ""
review_ready_at: "2026-10-16T00:53:53-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:53:53-03:00"
  decided_at: "2026-10-16T00:53:53-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-029 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «READY concedido CON CONDICION ESCRITA: el batch no se ejecuta hasta t2 verde…»), ADR-002 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class high → autonomy_level L2; data_classification confidential; test_integration not-required → applicability n/a (razón). Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: las precondiciones del READY son la PRIMERA stop condition (§15 #1) —sin t2 verde y sin corpus respaldado/commiteado, el batch NO corre—; verificación previa sobre el corpus anonimizado contra los digests dorados del 002 antes de tocar modelos reales (Fase A.2); atomicidad por archivo sin salida parcial; conservación/inventario de originales por hash; reporte con la cadena de conexión como warning SIN su valor; cláusula de re-migración (Fase E); manejo confidential (nada del corpus cruza el límite del prompt; gate de secretos en 0, AC-5)."
    - "CONFIRMACIÓN (condición de ejecución, no trámite) — CORRECTA: §14 declara «la corrida no empieza sin t2 verde ni sin respaldo, aunque el Bolt esté ready» y §15 #1 es la primera y más estricta (el Bolt no corre 'un ratito' para probar). El READY lo registró como condición escrita, no como autorización para correr antes."
    - "CONFIRMACIÓN (desbloqueo real depende del corpus, Bolt 002 blocked) — CORRECTA: las precondiciones son t2 verde (Bolt 004, test bilateral) y corpus respaldado/commiteado (Bolt 002), y el desbloqueo real del corpus es del Bolt 002 — actualmente `blocked` (su V-Bounce N°1 detenido por corpus anonimizado ausente). La condición t2 y el respaldo son la red de una operación irreversible; sin ellas el batch no corre. El SPEC no finge lo contrario."
    - "Compliant — manejo confidential: §10 declara que una clase confidential NO cruza el límite del prompt hacia un modelo externo (§3.6), el gate PII/DLP lo bloquea, y el resultado esperado del gate de secretos es 0; la cadena de conexión se excluye del formato nuevo y se reporta como warning sin su valor (AC-5). La condición de retiro de los originales se declara (no se ejecuta) con dueño, y la irreversibilidad se cubre con respaldo + originales intactos + cláusula de re-migración."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). El procedimiento de conversión, los niveles de conformidad (t1/t2), la atomicidad por
  archivo, la conservación de los originales y su condición de retiro están decididos en ADR-002 §3.2/§3.3/§3.4.
  Este SPEC ejecuta la corrida real y la vuelve verificable, sin reabrir la decisión.
  ⚠️ CONDICIÓN DEL READY (registrada por el revisor del Bolt): la corrida **no** se ejecuta hasta que la
  conformidad del formato esté cerrada (**t2 verde**) y el corpus esté **respaldado/commiteado**. Es condición de
  ejecución, no una autorización para correr antes.
  ⚠️ DATOS `confidential` (§3.6): los modelos legacy pueden transportar la cadena de conexión. El formato vigente
  la **excluye**; el reporte la registra como hallazgo y **nunca** expone su valor; nada del corpus cruza el
  límite del prompt hacia un modelo externo.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de dominio
  aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada invalida la
  aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0050 — Ejecución del batch real del corpus de modelos: conversión con reporte por archivo, originales conservados e inventariados

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 3 (hallazgo F-41) |
| **Bolt** | [US-000.BOLT-029](../functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md) |
| **ADRs** | [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.2, §3.3, §3.4 |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se ejecuta la **conversión del corpus real de modelos** al formato vigente (`.coop2`), con su **reporte por
archivo** —clasificación, conteos, **digests** y hallazgos con severidad—, dejando los **originales conservados e
inventariados** como red de recuperación y declarando el estado de cada modelo **no** convertible con su motivo.

El problema que resuelve está verificado: el `.coop` es un blob de serialización binaria (F-41), la conversión es
**irreversible en la práctica** —no hay conversor inverso ni se entregará— y el corpus convertido es **provisional
hasta que la conformidad del formato cierre (t2)**. Si este Bolt no se ejecuta, el producto sigue con sus modelos
en un formato que el runtime objetivo ya no puede leer, y el trabajo de la migración no llega al activo que más
importa: los modelos del cliente.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 3 como ADR + BOLT→SPEC. ADR-002 (`accepted`)
decide el formato, la herramienta (`net48`, `cooperator-migration convert`), la **atomicidad por archivo** (cada
`.coop2` se escribe a temporal y se renombra: si un archivo falla, **no deja salida parcial**), la **conservación de
los originales** hasta validar el release 2.0.0, y la **secuencia**: convertir **primero**, antes de tocar el TFM
del Modeler. Este SPEC ejecuta **la corrida real** sobre esa base.

**Condición de estado (del READY, registrada por el revisor del Bolt):** la corrida **no** se ejecuta hasta que
**(a)** la conformidad del formato esté cerrada —**t2 verde**: el test bilateral `write(read(fixture)) == fixture`
del Bolt de `Modeling` pasando— y **(b)** el corpus esté **respaldado/commiteado** (ADR-002 §3.2: el control de
versiones es el respaldo, y se recomienda además una copia fuera del repositorio). Es **condición de ejecución**,
nunca una autorización para correr antes: un corpus convertido antes de t2 es provisional y se re-migra desde los
originales si t2 revela una divergencia.

**Propiedades de la corrida que el SPEC hace verificables (ADR-002 §3.2/§3.3):**

| Propiedad | Decisión | Cómo se verifica acá |
|---|---|---|
| La herramienta **nunca** modifica ni borra un `.coop` y no acepta `--in-place` | ADR-002 §3.2 | Hash de los originales **antes y después**: idénticos; el inventario los ubica |
| **Atomicidad por archivo** | ADR-002 §3.2 | Un archivo que falla **bloquea** ese archivo (entrada en el reporte, exit `3`) y **no** deja salida parcial; el resto del batch continúa |
| **`--dry-run` obligatorio en la primera pasada** | ADR-002 §3.2 | La primera pasada clasifica cada archivo (binario legado / ya `.coop2` / ilegible) **sin escribir nada** |
| Reporte **por archivo** con digests | ADR-002 §3.2 | Por archivo: rutas, *magic bytes*, `modelerVersion` legado, conteos, `schemaVersion` de salida, digest del modelo normalizado y hallazgos con severidad `error`/`warning` |
| La **cadena de conexión no viaja** al formato nuevo | ADR-002 §3.3 | Se registra como **warning** ("secreto no trasladado"); su **valor** no aparece en ningún artefacto |
| **VB.NET bloquea por defecto** (OQ-005) | ADR-002 §3.3 | Un modelo `language: VB` bloquea con error claro; `--allow-vb-to-csharp` lo migra como C# con *warning* destacado |
| Oracle y export a Excel: **nada que migrar**, declarado | ADR-002 §3.3 | El reporte lo declara explícitamente para que la ausencia no se lea como pérdida |
| **Conservación de los originales** hasta validar 2.0.0 | ADR-002 §3.2 | Inventario con ubicación y correspondencia; la **condición de retiro** queda declarada (**no** se ejecuta acá) |

**Estado del árbol.** El migrador (Bolt 002) y el contrato del formato (Bolt 004) ya tienen SPEC en disco; el
corpus real es el activo que esta corrida toca. Este Bolt escribe **sólo** su SPEC y, en su V-Bounce, la corrida y
su evidencia (reporte, inventario); **no** modifica la herramienta ni el contrato.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-002 es la decisión vigente del formato y del migrador; ADR-001 §3.2
fija el orden del camino de migración, que este Bolt ejecuta como **paso 1**), y el plan de integración declarado
(`n/a`) coincide con el `test_integration: not-required` del Bolt (§3.2.2). No se emite reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-029` (`devflow/functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md`) | AITL-BOLT-READY-Approval ✓ — `approved` **con condición escrita** (el batch no corre hasta t2 verde y corpus respaldado/commiteado), `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:12:35-03:00; `risk_class: high`, `test_integration: not-required`, `data_classification: confidential` |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-002` §3.2 (niveles t1/t2, batch real y su carácter provisional, irreversibilidad, conservación de originales, reporte y validaciones, `--dry-run`, códigos `0/1/2/3`), §3.3 (deprecaciones: VB, Oracle, Excel, `connectionString` como warning) y §3.4 (secuencia y cierre de la conformidad) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T22:32:47-03:00 |
| REV evidence | `REV-001` §4 F-41 y §6 ítem 3 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0002-…` (herramienta, fixtures dorados y t1), `SPEC-261016-0010-…` (**t2**, condición de estado) y `SPEC-261016-0000-…` (arnés/build) | Los tres **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otros V-Bounces; **este** Bolt no lo toca) |

---

## 4. Scope

### In scope

1. **La verificación de las precondiciones de estado** antes de tocar el corpus: **t2 verde** (evidencia del test
   bilateral del contrato sobre los mismos fixtures del Bolt 002) y **corpus respaldado/commiteado** (evidencia del
   respaldo; recomendación de copia fuera del repositorio), más la **verificación previa sobre el corpus
   anonimizado** de fixtures: la clasificación, los conteos y los digests deben coincidir con los `.coop2` dorados
   **antes** de convertir un solo modelo real.
2. **La corrida del batch real** sobre el corpus: `--dry-run` obligatorio en la primera pasada (sin escribir),
   `--allow-vb-to-csharp` explícito sólo cuando corresponda por decisión, y la conversión con **reporte**.
3. **El reporte por archivo y el agregado**: clasificación, *magic bytes*, `modelerVersion` legado, conteos,
   `schemaVersion` de salida, **digest del modelo normalizado** y hallazgos con severidad; la lista de **no
   migrables** con su motivo; el resumen de la corrida.
4. **La conservación y el inventario de los originales**: los `.coop` **no modificados ni borrados**, inventariados
   con su ubicación y su **correspondencia** con cada modelo convertido.
5. **La declaración de la condición de retiro de los originales** (hasta validar el release 2.0.0; después pueden
   retirarse del control de versiones dejando los fixtures anonimizados como evidencia) — su **ejecución no** es de
   este Bolt.
6. **El tratamiento declarado de los no migrables**: listado con motivo y próximo paso (intervención humana), sin
   salida parcial y sin corrección automática.
7. **La constancia** del cambio para las notas de migración (ADR-008 §3.1): qué corpus quedó convertido, qué
   originales quedan conservados y bajo qué condición, y qué modelos quedaron fuera con su motivo.

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| La **herramienta de conversión** y sus fixtures | Bolt 002 (SPEC-0002) | El Bolt lo excluye: acá se **ejecuta** la herramienta, no se implementa ni se modifica |
| El **contrato del formato** y su test bilateral | Bolt 004 (SPEC-0010) | Se **consume** su estado t2 como precondición |
| **Retirar del control de versiones** los originales | Posterior a la validación del release | ADR-002 §3.2: la condición se declara acá, la ejecución es después |
| El retiro de la **capacidad de descifrado legacy** y el cierre de su registro | Bolt 030 | Otro entregable (el corpus migrado es su condición de cierre, no su ejecución) |
| La **protección de valores** y sus comandos | Bolts 022/023 | Otras superficies |
| El **motor**, el **shell** y la **capa de datos** | Bolts 017/018/025/026 y 013/014/016 | No se tocan |
| **Corregir** los modelos no migrables | — (nadie acá) | El Bolt lo excluye: se **reportan** para intervención humana |

**Regla de aporte (bounded, explícita).** Este Bolt **ejecuta** una corrida y produce su evidencia. No modifica la
herramienta, el contrato ni el corpus original; no corrige modelos; no decide el retiro de los originales ni el de
la capacidad legacy. Si apareciera la necesidad de cambiar la herramienta o el contrato, se **detiene** y se
reporta contra su Bolt (el corpus convertido no se "arregla" a mano).

---

## 5. Prerequisites and baseline

- **Bolt 004 y su SPEC** (`SPEC-261016-0010-…`): el contrato del formato con su **test bilateral** — el estado
  **t2 verde** es la **condición de estado** de esta corrida; su evidencia se adjunta.
- **Bolt 002 y su SPEC** (`SPEC-261016-0002-…`): la **herramienta de conversión** (`cooperator-migration convert`),
  sus **fixtures dorados** (corpus anonimizado + `.coop2` esperado), su nivel t1 y la verificación de identidad de
  los binarios 1.4 que carga.
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el build 1.4 reproducible (que el
  migrador necesita) y el arnés.
- **Corpus respaldado y commiteado:** evidencia del respaldo **antes** de la corrida (control de versiones; copia
  fuera del repositorio recomendada). Es la única red si algo se migra mal (ADR-002 §3.2).
- **Riesgo `high` → autonomía `L2` (§3.3):** el V-Bounce **para y confirma** ante cualquier desvío que ponga en
  riesgo un modelo (una clasificación dudosa, un hallazgo `error` inesperado, un original que cambie). La
  aprobación del SPEC y del MEM exige independencia **endurecida a nivel de modelo** (§3.0).
- **Estado del árbol 2.0:** en construcción por otros V-Bounces; este Bolt escribe su SPEC y, **en su** V-Bounce, la
  corrida y su evidencia.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline; el migrador **compila** contra el fuente 1.4
  congelado y **no** lo edita (ADR-002 §3.2).

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-029`), **1 V-Bounce** esperado. El orden es el de la seguridad del activo: primero las
> precondiciones, después la verificación previa sobre los fixtures, después la corrida, y al final la conservación,
> el inventario y la evidencia.

### Phase A — Precondiciones de estado verificadas

**Duration:** ~1h de ciclo — **Complexity:** Low

#### A.1 t2 verde y corpus respaldado

Se reúne la evidencia de las **dos** condiciones del READY: **(a)** el **test bilateral** del contrato
(`write(read(fixture)) == fixture`, byte a byte) sobre **los mismos** fixtures del Bolt 002 —el cierre de la
conformidad del formato—; y **(b)** el **respaldo/commit** del corpus previo a la corrida, con su ubicación. Si
alguna falta, el batch **no corre** (§15 #1).

#### A.2 Verificación previa sobre el corpus anonimizado

Antes de convertir un solo modelo real, la corrida se ejerce sobre el **corpus anonimizado** de fixtures
(Bolt 002): la clasificación de `--dry-run`, los conteos y los **digests** deben coincidir con los `.coop2`
dorados. Es el control que detecta una herramienta o un contrato desalineados **antes** de tocar el activo.

**Files created:**
- `2.0/artifacts/corpus-batch/preconditions.md` — evidencia de t2, del respaldo y de la verificación previa sobre los fixtures

---

### Phase B — La corrida del batch real

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### B.1 Primera pasada, sin escribir

`cooperator-migration convert … --dry-run` sobre el corpus: clasifica **cada** archivo (binario legado / ya
`.coop2` / ilegible) **sin escribir nada**. El resultado de esta pasada es la expectativa de la corrida real: si
entre ambas cambia la clasificación de un archivo, se **detiene** y se investiga.

#### B.2 La conversión, con atomicidad por archivo

La corrida real escribe **un** `.coop2` por archivo, a temporal y **renombrado** al final: un archivo que falla
**bloquea ese archivo** (entrada en el reporte con severidad `error`, exit `3`) y **no deja salida parcial**,
**sin** frenar el resto del batch. Los *warnings* se aceptan (exit `2`) y una corrida completa sin hallazgos da
exit `0`. La herramienta **nunca** escribe sobre un `.coop`, nunca borra y no acepta `--in-place`: los originales
quedan intactos **por construcción** y se verifican por hash.

#### B.3 Las decisiones por archivo que el SPEC no toma por su cuenta

Un modelo **VB.NET** bloquea por defecto (OQ-005) y sólo se migra como C# con `--allow-vb-to-csharp` explícito,
registrando un *warning* destacado: la decisión es **humana y por archivo**, nunca silenciosa. Una **divergencia**
entre la expectativa del `--dry-run` y el resultado real, o un hallazgo `error` que no corresponda a un modelo
conocido como problemático, detiene la corrida y se reporta (§15).

**Files created:**
- `2.0/artifacts/corpus-batch/dry-run-report.json` · `2.0/artifacts/corpus-batch/convert-report.json` — el reporte de la pasada de clasificación y el de la conversión

---

### Phase C — El reporte por archivo y el agregado

**Duration:** ~1,5h de ciclo — **Complexity:** Low

Se consolida el reporte —por archivo y agregado— con: rutas de origen y destino, ***magic bytes*** detectados,
`modelerVersion` legado, **conteos** (entidades, propiedades, `related`, `queries`), `schemaVersion` de salida,
**digest del modelo normalizado** (SHA-256 del JSON canónico) y **hallazgos con severidad** `error`/`warning`; más
el resumen de la corrida (cuántos convertidos, cuántos ya en el formato vigente, cuántos **no migrables**) y la
**lista de no migrables con su motivo**. Dos reglas de higiene del dato: la **cadena de conexión** se registra como
*warning* ("secreto no trasladado") y **su valor no aparece** en ningún artefacto; y las ausencias esperadas
(Oracle, export a Excel, el gate por token) se **declaran** para que no se lean como pérdidas.

**Files created:**
- `2.0/artifacts/corpus-batch/report-summary.md` — el resumen legible de la corrida (convertidos, no migrables, hallazgos)
- `2.0/artifacts/corpus-batch/non-migrable.md` — la lista de no migrables con su motivo y su próximo paso

---

### Phase D — Conservación, inventario y condición de retiro

**Duration:** ~1h de ciclo — **Complexity:** Low

#### D.1 Los originales, intactos e inventariados

Se verifica por **hash** que los `.coop` originales **no** fueron modificados ni borrados, y se produce el
**inventario**: por cada original, su ubicación, su digest y la **correspondencia** con el modelo convertido (y su
digest). El inventario es la pieza que hace que la red de recuperación sea real y no una intención.

#### D.2 La condición de retiro, declarada (no ejecutada)

Se declara la condición bajo la que los originales pueden retirarse del control de versiones: **hasta que el
release 2.0.0 esté validado**; recién entonces, dejando los fixtures anonimizados como evidencia (ADR-002 §3.2). Su
**ejecución no** es de este Bolt: queda declarada, con su dueño, para que no se pierda.

**Files created:**
- `2.0/artifacts/corpus-batch/originals-inventory.json` — inventario de originales conservados con su correspondencia y digests
- `2.0/artifacts/corpus-batch/originals-retirement-condition.md` — la condición de retiro declarada, con su dueño y su vencimiento

---

### Phase E — Cierre, constancia y evidencia

**Duration:** ~1h de ciclo — **Complexity:** Low

Se cierra con la evidencia del Bolt y su constancia: el paquete (reporte agregado y por archivo, inventario de
originales, lista de no migrables, evidencia de t2 y del respaldo), y la entrada para las notas de migración
(Bolt 028) con los tres datos de ADR-008 §3.1 — qué cambia (el corpus pasa al formato vigente), por qué (ADR-002:
el formato binario es inmanejable y el runtime objetivo no lo lee) y con qué se reemplaza (el `.coop2` y, para lo
no migrable, la intervención humana) — más la **cláusula de re-migración**: si una validación posterior del release
detecta una divergencia del formato, el corpus se **re-migra desde los originales intactos**; la corrida no es un
punto sin retorno mientras los originales existan.

**Files created:**
- `2.0/docs/api-removals/US-000.BOLT-029.json` — constancia del cambio del corpus y de la condición de los originales (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-002, expresados como Given/When/Then verificables.

### AC-1: Las precondiciones estaban cumplidas antes de tocar el corpus

**Given** el corpus real y las dos condiciones del READY
**When** se revisa la evidencia previa
**Then** el **test bilateral (t2)** estaba verde sobre los mismos fixtures y el corpus estaba **respaldado y
commiteado** antes de la primera escritura, y la clasificación/digests sobre el corpus **anonimizado** coincidían
con los dorados.

### AC-2: Todo modelo del corpus está convertido, con su reporte y su digest

**Given** la corrida sobre el corpus
**When** se leen el reporte por archivo y el agregado
**Then** cada modelo está convertido o listado como no migrable, y cada archivo convertido tiene su entrada con
rutas, conteos, `schemaVersion` de salida, **digest** y hallazgos con severidad.

### AC-3: Los originales están intactos e inventariados

**Given** los `.coop` originales
**When** se comparan sus hashes antes y después de la corrida
**Then** son **idénticos** (no modificados ni borrados) y el inventario los ubica con su **correspondencia** con
cada modelo convertido.

### AC-4: Los no migrables están listados con motivo y sin salida parcial

**Given** un archivo no migrable (VB sin la bandera, ilegible, o con un hallazgo `error`)
**When** termina la corrida
**Then** está en la lista con su **motivo**, no produjo **salida parcial** y el resto del batch continuó.

### AC-5: La cadena de conexión no se traslada y no se exponen valores

**Given** un modelo legacy con cadena de conexión
**When** se convierte
**Then** el `.coop2` **no** la contiene, el reporte la registra como *warning* ("secreto no trasladado") y **ningún**
artefacto de la corrida contiene su valor.

### AC-6: La condición de retiro de los originales queda declarada

**Given** el cierre del Bolt
**When** se lee la constancia
**Then** la condición de retiro (hasta validar el release 2.0.0, dejando los fixtures anonimizados como evidencia)
está **declarada con su dueño** y **no** fue ejecutada en este Bolt.

### AC-7: El `--dry-run` precede a la escritura y la atomicidad se respeta

**Given** la corrida real
**When** se revisa su secuencia
**Then** la primera pasada fue `--dry-run` **sin** escritura, la clasificación coincidió con la corrida real, y un
archivo fallido **bloqueó** ese archivo sin dejar salida parcial ni frenar el resto.

### AC-8: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde y
`git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "el corpus real está convertido y cada archivo convertido tiene su reporte con digests coincidentes con su origen" | Fases B y C: corrida + reporte por archivo | AC-2 · `convert-report.json` |
| "los originales **no** fueron modificados ni borrados y quedan inventariados junto a los convertidos" | Fase D.1: hash antes/después + inventario | AC-3 · `originals-inventory.json` |
| "cada archivo no migrable está listado con su motivo y sin salida parcial" | Fases B.2, B.3 y C | AC-4 · `non-migrable.md` |
| "la conservación de los originales queda declarada con su condición de retiro posterior" | Fase D.2 | AC-6 · `originals-retirement-condition.md` |
| "el estado de conformidad del formato está cerrado (**t2 verde**) **antes** de esta ejecución" | Fase A.1 | AC-1 · evidencia del test bilateral |
| "la constancia del respaldo/commit del corpus previo a la corrida" | Fase A.1 | AC-1 · evidencia del respaldo |
| (riesgo declarado por el Bolt) "material sensible dentro de los modelos legacy" | Fase C: la cadena de conexión como *warning*, sin valores | AC-5 · reporte sin valores |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Precondiciones (`preconditions`):** presencia y lectura de la evidencia de **t2 verde** y del
  **respaldo/commit**; si falta alguna, la corrida **no** empieza (§15 #1).
- **Verificación previa sobre los fixtures:** `--dry-run` y conversión sobre el **corpus anonimizado** del Bolt 002
  con **coincidencia** de clasificación, conteos y **digests** contra los `.coop2` dorados (round-trip de la
  herramienta sobre lo conocido antes de tocar el activo).
- **Atomicidad y ausencia de salida parcial:** con un archivo preparado para fallar (ilegible o con un hallazgo
  `error`), se verifica que **no** queda `.coop2` a medias, que el archivo aparece con severidad `error` en el
  reporte y que el resto del batch continúa.
- **Originales intactos:** hash del conjunto de `.coop` antes y después de la corrida: idéntico.
- **Reporte completo y sin valores:** cada entrada tiene los campos exigidos (rutas, *magic bytes*,
  `modelerVersion`, conteos, `schemaVersion`, digest, severidad); la cadena de conexión aparece como *warning* y
  **ningún** valor del modelo aparece en el reporte (control por búsqueda de patrones de conexión/credenciales).
- **Inventario y no migrables:** el inventario cubre **todos** los originales con su correspondencia; la lista de no
  migrables tiene un motivo por entrada.
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** archivo ya en el formato vigente (se clasifica y **no** se reescribe); archivo ilegible (error y
  bloqueo); modelo VB sin la bandera (bloqueo con el mensaje accionable) y con la bandera (*warning* destacado);
  archivo con la cadena de conexión; archivo con caracteres no ASCII en nombres (identificadores inválidos y
  colisión de `generateAs` como `error`); corpus vacío (corrida sin archivos: no es un éxito silencioso, se
  declara); un `.coop` que cambia entre la pasada de clasificación y la corrida (se detiene, §15).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio (su evidencia es el
  reporte de la corrida y la verificación por digests).

### 8.2 Integration test level

- **`n/a`** — el Bolt declara `test_integration: not-required` y el plan del frontmatter dice `n/a` con su razón:
  la conversión opera sobre archivos del repositorio con verificación por digests y **no** toca base de datos, cola
  ni servicio externo. La «base externa» que este Bolt **sí** verifica es su **condición de estado** (t2) y el
  **respaldo**, ambos evidenciados en la Fase A y no un entorno en ejecución.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (§3.6).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El árbol compila con advertencias tratadas como error (incluida la herramienta del migrador) | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (precondiciones, fixtures, atomicidad, originales, reporte, inventario) | `pass` — esperado (AC-1…AC-7) |
| Consumer-side contract tests (test doubles) | V-Bounce | El «contrato» de esta corrida es el **reporte** y el **inventario**: se verifican por estructura y por digests | `pass` — esperado (AC-2, AC-3) |
| SAST | V-Bounce | El cambio es de **datos** (corpus convertido) y de evidencia; no introduce código de producto | `n/a` — sin código nuevo |
| Dependency scanning / licenses | V-Bounce | Sin dependencias nuevas; el migrador conserva las suyas (ADR-002) | `pass` — esperado |
| OWASP Top 10 — static review | V-Bounce | Sin superficie de red ni servicio; el riesgo es de **datos** (ver §10) | `n/a` — sin superficie alcanzable |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts; los modelos son **datos** | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | **0** valores de credenciales en el reporte, el inventario y los artefactos de la corrida (la cadena de conexión se reporta como *warning*, nunca con su valor) | `pass` — esperado (AC-5) |
| Hallucination lint | V-Bounce | Rutas, digests y archivos referenciados existen en el corpus | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `confidential`: el gate bloquea el envío de esa clase fuera del límite del prompt; el corpus y sus valores **no** cruzan ese límite | `pass` — esperado (§10) |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; los **fixtures dorados** (Bolt 002) existen antes y son la expectativa de la verificación previa | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | La verificación previa sobre los fixtures reproduce los digests dorados; una segunda corrida del `--dry-run` clasifica igual | `pass` — esperado (AC-7) |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | La corrida afecta **datos** (modelos), no superficie de los paquetes | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | No aplica: `test_integration: not-required` (plan `n/a`) | `n/a` — verificación sobre archivos (§3.2.2) |
| Contract / E2E against the real providers | Integration test | El contrato verificado es el **formato** (por digests y por el test bilateral ya cerrado), no un proveedor | `n/a` — sin dependencias reales |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint; el costo de conversión es de la herramienta (Bolt 002) | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | La observabilidad es el **reporte** de la corrida (§11) | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: not-required` → `integration_test_plan.applicability: n/a`
> con su razón.

---

## 10. Security and data

- **El activo es el corpus y el riesgo es irreversible:** la única red real es el **respaldo** (control de versiones
  y copia fuera del repositorio) más los **originales intactos** y su inventario. La corrida **no** modifica ni
  borra un `.coop`: eso se verifica por hash, no se promete.
- **Datos `confidential`:** los modelos legacy pueden transportar la **cadena de conexión** (el formato viejo la
  llevaba). El formato vigente **no** la transporta: se registra como *warning* ("secreto no trasladado") y su
  **valor** no aparece en el reporte, el inventario ni ningún artefacto de la corrida. Una clase `confidential`
  **no** cruza el límite del prompt hacia un modelo externo (§3.6) y el gate PII/DLP lo bloquea: nada del corpus se
  copia a un prompt, a un log ni a un artefacto de evidencia.
- **Sin superficie nueva:** la corrida no abre red, no aplica SQL y no escribe fuera de su carpeta de salida; la
  herramienta del migrador es `net48`, offline y autocontenida (ADR-002 §3.2).
- **Integridad del árbol congelado:** `1.4/**` no se modifica; el migrador compila contra el fuente congelado y
  verifica la identidad de los binarios 1.4 que carga (anti-deriva).
- **Clasificación:** `data_classification: confidential` (espeja el Bolt) — es la clasificación del **corpus**, no
  un adorno: manda el manejo de §10 y el resultado esperado **0** del gate de secretos.

---

## 11. Monitoring and observability

- **El reporte de la corrida es la observabilidad del batch:** por archivo (clasificación, conteos, digest,
  hallazgos con severidad) y agregado (convertidos, ya vigentes, no migrables), más el inventario de originales.
  Es lo que permite revisar la corrida **sin** re-ejecutarla.
- **Evidencia versionada:** `2.0/artifacts/corpus-batch/` (precondiciones, `dry-run`, reporte de conversión,
  resumen, no migrables, inventario, condición de retiro) y la constancia para las notas
  (`2.0/docs/api-removals/US-000.BOLT-029.json`).
- **Sin telemetría de servicio:** la corrida es una herramienta offline (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration:** es **la** migración de datos del proyecto en su paso 1 (ADR-001 §3.2): los modelos pasan al
  formato vigente y el resto del camino (repuntar el proyecto, resolver features cortadas, regenerar con el motor
  2.0) se apoya en un corpus ya convertible y legible.
- **Compatibility:** el `.coop2` es un formato **nuevo**; un consumidor que necesite el formato viejo se queda con
  su copia congelada (los originales conservados) y con el **1.4 congelado**. La convivencia de dos formatos es
  transitoria y está declarada (ADR-002 §4).
- **Rollback:** **no hay conversor inverso** (ADR-002 §3.2) y esa es la razón por la que la red es el respaldo y los
  originales: revertir el commit del Bolt devuelve el árbol, pero **el activo no se "des-convierte"** — se
  re-convierte desde los originales si el formato cambia (cláusula de re-migración, Fase E). La irreversibilidad es
  la razón del riesgo `high` y de la autonomía `L2`.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Irreversibilidad efectiva sobre modelos reales** (no hay conversor inverso) | 3 | 5 | Respaldo y commit previos + originales conservados e inventariados + conformidad del formato (**t2**) cerrada **antes** de la corrida + cláusula de re-migración desde los originales |
| **Pérdida de un modelo** por un archivo que falla a mitad | 2 | 4 | Escritura atómica por archivo (temporal + renombrado) y bloqueo con diagnóstico: **sin** salida parcial; el resto del batch continúa (AC-4, AC-7) |
| **Material sensible** dentro de los modelos legacy (la cadena de conexión viaja en el formato viejo) | 3 | 4 | Se **excluye** del formato nuevo y se reporta como *warning*; **ningún** valor en el reporte ni en los artefactos (AC-5); clase `confidential` sin cruzar el prompt (§10) |
| **Corrida prematura** (antes de t2 verde o sin respaldo) | 2 | 5 | La condición del READY es la **primera** stop condition del SPEC (§15 #1) y la Fase A la evidencia |
| **No migrables sin dueño** | 3 | 2 | Listado con motivo y próximo paso, y declarado en el resumen y en la constancia (AC-4) |
| Un archivo cambia entre la clasificación y la corrida real | 2 | 3 | La clasificación del `--dry-run` es la expectativa: una diferencia **detiene** la corrida (§15) |
| La herramienta o el contrato están desalineados y el error se descubre sobre el activo | 2 | 4 | **Verificación previa sobre el corpus anonimizado** con coincidencia de digests contra los dorados (Fase A.2) |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| La corrida **no** empieza sin t2 verde ni sin respaldo, aunque el Bolt esté `ready` | Es la **condición escrita** del READY: t2 cierra la conformidad del formato y el respaldo es la única red de una operación irreversible (ADR-002 §3.2) |
| Se verifica primero sobre el **corpus anonimizado** de fixtures | Detectar un desalineamiento de herramienta o contrato sobre fixtures **antes** de tocar el activo es la diferencia entre una prueba y un incidente |
| La primera pasada es **`--dry-run`** y su clasificación es la expectativa | ADR-002 §3.2 lo exige; además convierte "qué espero" en algo comparable con el resultado real |
| La cadena de conexión se reporta como **warning**, nunca con su valor | El formato nuevo la excluye por diseño; el hallazgo es información útil **sin** trasladar el secreto al reporte (clase `confidential`) |
| Los **no migrables** se listan con motivo y no se corrigen | El Bolt lo excluye: una corrección automática de un modelo es una decisión de negocio, no un paso de conversión |
| El **inventario** es un artefacto, no una nota | La red de recuperación sólo existe si se puede encontrar cada original y su correspondencia; un inventario en prosa no se verifica |
| La **condición de retiro** de los originales se declara acá y se ejecuta después | ADR-002 §3.2: el retiro llega con el release 2.0.0 validado; declararlo acá evita que la condición se pierda y evita retirar antes de tiempo |
| Un modelo **VB** requiere una bandera explícita y deja *warning* | OQ-005 sacó VB del alcance de la 2.0: la decisión de migrarlo como C# es humana y por archivo, jamás silenciosa |

---

## 15. Stop conditions

> Con `autonomy_level: L2` (riesgo `high`), ante cualquiera de estos casos el V-Bounce **para y reporta** en lugar
> de continuar.

1. **No están cumplidas las condiciones del READY — la primera y más estricta:** sin **t2 verde** (test bilateral
   del contrato passing sobre los mismos fixtures) **y** sin el **corpus respaldado/commiteado**, el batch **no se
   ejecuta**. Se reporta y se espera; el Bolt no corre "un ratito" para probar.
2. **La verificación previa sobre los fixtures no coincide** con los digests dorados (clasificación, conteos o
   digests divergentes): detener y reportar contra la herramienta (Bolt 002) o el contrato (Bolt 004); el activo
   **no** se toca.
3. **La clasificación del `--dry-run` y el resultado real difieren** en cualquier archivo: detener y reportar; algo
   cambió entre las dos pasadas y ninguna de las dos es confiable sin explicación.
4. **Un `.coop` original aparece modificado o borrado** (hash distinto antes/después): detener de inmediato; el
   contrato del migrador prohíbe escribir sobre un original y su violación es un incidente.
5. **Un hallazgo `error` que no corresponde a un modelo conocido como problemático**, o un fallo **sistemático**
   (el mismo error en todos los archivos): detener y reportar; un archivo bloqueado es esperable, un patrón no.
6. **Se necesita corregir la herramienta, el contrato o un modelo** para que la corrida avance: detener y escalar a
   su Bolt. El corpus **no** se "arregla a mano".
7. **Aparece material sensible donde no debe** (un valor de conexión en el reporte, el inventario o un artefacto):
   detener de inmediato y reportar; es la fuga que la clase `confidential` prohíbe.
8. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
9. **El arnés del Bolt 001 no está disponible** (comando reproducible, build 1.4 o suite): detener y escalar la
   secuencia.
10. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, la clasificación de un archivo que la
    herramienta no sabe leer ni declarar ilegible): parar y reportar; no se rellena el hueco con una suposición
    (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–E implementadas
- [ ] Todos los criterios de aceptación (AC-1…AC-8) verificados
- [ ] Las **condiciones del READY** (t2 verde + respaldo/commit) evidenciadas **antes** de la primera escritura
- [ ] El reporte agregado y por archivo con digests, más el resumen y la lista de no migrables con motivo
- [ ] El inventario de originales con su correspondencia y la **condición de retiro declarada** (no ejecutada)
- [ ] La cadena de conexión **no** trasladada y **ningún** valor sensible en los artefactos
- [ ] La verificación previa sobre el corpus anonimizado coincide con los digests dorados
- [ ] Los originales intactos por hash; `1.4/**` intacto respecto del commit baseline
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la constancia del cambio
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos)
- [ ] AITL-MEM-Approval registrado (riesgo `high`: independencia de aprobación **endurecida a nivel de modelo**, §3.0)

---

## 17. References

- Bolt: [`US-000.BOLT-029`](../functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md) y su manifiesto
  [`US-000.BOLT-029-…json`](../metrics/bolts/US-000.BOLT-029-batch-real-del-corpus.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADR gobernante: [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.2, §3.3, §3.4
- SPECs prerrequisito: [`SPEC-261016-0002`](./SPEC-261016-0002-migrador-coop-a-coop2.md) (herramienta, fixtures y t1) ·
  [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) (**t2**) · [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y build 1.4 reproducible)
- SPECs relacionados: [`SPEC-261016-0048`](./SPEC-261016-0048-retiro-descifrado-legacy.md) (el retiro de la capacidad de descifrado legacy, cuya condición de cierre del lado del modelo es este corpus)
  ver §5 de ese SPEC para la frontera
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-41 y §6 ítem 3
- Evidencia de código 1.4: el formato legacy es un blob de serialización binaria del contenedor del modelo (F-41);
  `1.4/Tools/CooperatorModeler/Snapshot.cs` (el contenedor serializable) y `MainForm.cs:215,270`
  (`BinaryFormatter`), con la lista de binarios vendorizados en `1.4/Tools/CooperatorModeler/Templates/Assemblies/`
  (cuya divergencia con el fuente 1.4 —`Utility` 1.3.3.0— registra el inventario del Bolt 001)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|---------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-029`: ejecución del batch real del corpus tras cerrar la conformidad del formato (t2) y con el corpus respaldado, con `--dry-run` previo, verificación sobre el corpus anonimizado, reporte por archivo con digests, originales conservados e inventariados, no migrables listados con motivo y condición de retiro declarada |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** This SPEC remains a draft until the
> Dev-validator (+ applicable domain owners) records `AITL-SPEC-Approval`
> (in the `review` frontmatter block). Bolt approval (`AITL-BOLT-READY-Approval`)
> authorizes SPEC preparation; **SPEC approval** authorizes the code-run /
> V-Bounce. A material source change invalidates this approval — stop,
> revise, re-approve (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de datos (corpus y su respaldo) + dueño de arquitectura (condición t2 y contrato del formato)] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **La condición del READY es la primera stop condition** (§15 #1): sin **t2 verde** y sin **corpus
   respaldado/commiteado**, el batch no se ejecuta. Es el punto que el revisor del Bolt dejó escrito y el SPEC lo
   trata como condición de ejecución, no como trámite.
2. **Verificación previa sobre el corpus anonimizado** (Fase A.2): coincidencia de clasificación, conteos y digests
   con los `.coop2` dorados del Bolt 002 **antes** de tocar el activo. Confirmar que es la precondición esperada.
3. **Manejo `confidential`:** la cadena de conexión se reporta como *warning* sin su valor y nada del corpus cruza
   el límite del prompt (§3.6). Confirmar el criterio y el resultado esperado **0** del gate de secretos.
4. **Retiro de los originales:** se **declara** la condición (hasta validar el release 2.0.0) con su dueño y **no**
   se ejecuta acá. Confirmar.
5. **Riesgo `high` → `L2`:** el V-Bounce para ante clasificación dudosa, hallazgo inesperado o un original que
   cambie; la aprobación del SPEC/MEM exige independencia endurecida a nivel de modelo (§3.0). Confirmar.
