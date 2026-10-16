---
id: "SPEC-261016-0045"
title: "Shell ↔ motor: plan antes de escribir, generación asíncrona y cancelable, reporte coincidente con el CLI y handoff al comando"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 14 (hallazgos F-33 y F-42) → ADR-009 §3.1 / ADR-003 §3.1/§3.6
bolt: "US-000.BOLT-026" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-009", "ADR-003"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0044-shell-wpf.md" # el esqueleto del shell, el motor hosteado y el estado del usuario (Bolt 025)
  - "SPEC-261016-0023-motor-headless.md" # el punto de entrada tipado del motor, su plan y su resultado (Bolt 017)
  - "SPEC-261016-0025-cli-del-motor.md" # el CLI: el plan, el reporte y los códigos con los que el shell coincide y hacia el que hace handoff (Bolt 018)
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y build (Bolt 001)
risk_class: "medium" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L3" # default por riesgo low/medium (§3.3)
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "internal" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: not-required` → `n/a`
  applicability: "n/a"
  tests: []
  environment: ""
  data: ""
  reason: "Bolt `not-required` (refactor): el shell hospeda el motor en el mismo proceso y su plan y su reporte se producen sobre el archivo de modelo; el resultado no toca base de datos, cola ni servicio externo. La respuesta de la interfaz y la cancelación se miden en la aceptación sobre el modelo de referencia (NFR de ADR-009 §5), no en un ciclo de integración."
  waiver_adr: ""
review_ready_at: "2026-10-16T00:48:41-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:48:41-03:00"
  decided_at: "2026-10-16T00:48:41-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-026 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «DoR verificado; sin observaciones»), ADR-009/ADR-003 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class medium → autonomy_level L3; data_classification internal; test_integration not-required → applicability n/a (razón). Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: flujo plan → confirmar → generar → reporte sobre el punto de entrada único (sin Process), plan mostrado antes de escribir; asíncrono con progreso y cancelable (cancelación = workspace consistente + desenlace explícito de usuario, no un fallo); reporte coincidente con el CLI (criterio del DoR); fallos propagados con causa; handoff con el comando equivalente construido desde la misma petición ejecutada; sin secretos en estado/log (AC-8)."
    - "ADJUDICACIÓN (a) evidencia de aceptación = coincidencia con el CLI — CONFIRMADA: §C.2/AC-2/§14/§19 nota 1 fijan que la aceptación se apoya en la coincidencia de plan y reporte (mismos artefactos, contadores y advertencias sobre el mismo modelo), no en una prueba de interfaz aislada; es además la prueba de que no hay segunda implementación de generación."
    - "ADJUDICACIÓN (b) cancelación ≠ error — CONFIRMADA: §B.2/AC-3/§14/§19 nota 5 declaran el desenlace de cancelación como explícito (cancelado por el usuario), con workspace consistente apoyado en la regla de propiedad y escritura por artefacto del motor (ADR-003 §3.4); nunca se reporta como fallo del motor ni error inesperado."
    - "ADJUDICACIÓN (c) fronteras 025/026/027 — CONFIRMADAS: §4/§19 nota 3 recortan el flujo (plan/reporte/asincronía/cancelación/handoff) sobre el esqueleto/editor/estado del Bolt 025, sin invadir empaquetado/asociación (Bolt 027); el §15 #7 devuelve el trabajo derivado a su Bolt dueño. Sin solapamiento."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). El punto de entrada único del motor, el flujo plan→generar→reporte, la operación
  asíncrona y cancelable fuera del hilo de interfaz y el handoff al CLI están decididos en ADR-009 §3.1/§3.2 y en
  ADR-003 §3.1/§3.6. Este SPEC los ejecuta y los vuelve verificables, sin prescribir UI ni patrones de interfaz.
  ⚠️ FRONTERA (declarada, no resuelta por omisión): el **esqueleto del shell, el editor dirigido por el esquema y
  el estado del usuario** son del Bolt 025 (SPEC-0044); el **empaquetado, la asociación y la integración con el
  sistema operativo** son del Bolt 027 (SPEC-0027 del backlog). Acá se entrega la **costura** entre el shell y el
  motor.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de dominio
  aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada invalida la
  aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0045 — Shell ↔ motor: plan antes de escribir, generación asíncrona y cancelable, reporte coincidente con el CLI y handoff al comando

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 14 (hallazgos F-33 y F-42) |
| **Bolt** | [US-000.BOLT-026](../functional/bolts/US-000.BOLT-026-integracion-shell-motor.md) |
| **ADRs** | [ADR-009](../adrs/ADR-009-shell-del-modeler.md) §3.1, §3.2, §3.3, §5 · [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1, §3.3, §3.4, §3.6 |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega la **integración del shell con el motor**: el flujo **cargar/editar → plan → generar → reporte** sobre la
**misma** implementación que usa el CLI, ejecutado **fuera del hilo de interfaz** y **cancelable**, con el **comando
equivalente** a la vista para el handoff entre el flujo humano y el flujo agente.

El problema que resuelve está verificado en el tool de 1.4: la generación corría **en el hilo de interfaz** con
`Application.DoEvents()` alrededor de `CodeGenerator.GenerateStoredProcedures`/`GenerateClasses`
(`GenerationDialog.cs`) —el antipatrón que el NFR `shell-ui-responsiveness` de ADR-009 §5 prohíbe—, **no existía un
plan** (el tool generaba directamente, con el modo completo que sobrescribía también lo editable) y **no existía un
reporte como contrato**: lo que se veía era un `MessageBox` y el resultado de abrir la solución generada con
`Process.Start`. Sin este Bolt, el shell tendría el motor disponible pero **nadie conduciría** la operación: el
humano no vería qué va a pasar antes de que ocurra, ni qué pasó después, y la ventana se congelaría mientras el
motor trabaja.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 14 al **ADR-009** (`accepted`), que decide el
flujo (cargar/editar → pedir el **plan** equivalente al `--dry-run` → confirmar → generar → mostrar el **reporte**
con el mismo contenido que el del CLI), la operación **asíncrona y cancelable** fuera del hilo de UI, y el **handoff**
mostrando el **comando CLI equivalente** de lo que se acaba de hacer (ADR-009 §3.1). La regla de fondo viene de
ADR-003 §3.1/§3.6: **el shell es un host del motor**, usa su **mismo punto de entrada tipado** y **no puede
divergir del CLI**.

**El punto de partida (evidencia verificada en esta sesión):**

| Aspecto | Evidencia 1.4 | Qué implica para este Bolt |
|---|---|---|
| La generación corría en el hilo de UI | `GenerationDialog.cs`: `Application.DoEvents()` antes y después de `CodeGenerator.GenerateStoredProcedures(this.currentSnapshot)` y de `CodeGenerator.GenerateClasses(currentSnapshot)` | Toda operación larga va **fuera** del hilo de interfaz, con progreso y cancelación (NFR `shell-ui-responsiveness`) |
| No había plan | Los botones de generación llamaban al generador directamente; el *staging* se borraba antes de cada corrida y el modo completo sobrescribía también lo editable (`FolderHelper.CopyFolder`) | El **plan** se muestra **antes** de escribir nada: qué se crea, qué se actualiza, qué se omite por ser editable y qué huérfanos se eliminan |
| No había reporte como contrato | El desenlace se veía como mensajes de UI y con `Process.Start(solFile)` (`GenerationDialog.cs:176,207`) para abrir la solución | El **reporte** del shell tiene el **mismo contenido** que el del CLI (artefactos, contadores, advertencias); su forma legible por máquina es el documento del CLI (Bolt 018) |
| No había camino headless ni handoff | El único punto de automatización era abrir el tool con un archivo como argumento (`MainForm.cs:509-512`) | El **comando equivalente** a la vista es el puente entre el flujo humano y el flujo agente (el CLI ya existe: Bolt 018) |

**Lo que este Bolt **consume** (y no reimplementa):** el **plan sin escritura** y la **cancelación** del motor
(Bolt 017: `--dry-run` es el plan; el `CancellationToken` del contexto de render); el **vocabulario del plan, del
reporte y de los códigos de salida** del CLI (Bolt 018, SPEC-0025); el **esqueleto, el editor y el estado** del shell
(Bolt 025, SPEC-0044).

**Estado del árbol 2.0.** `2.0/` está siendo construido por el V-Bounce del Bolt 001. El motor (017), el CLI (018) y
el shell (025) ya tienen SPEC en disco. Este Bolt escribe **sólo** su SPEC y, en su V-Bounce, la costura del flujo
en el proyecto del shell.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-009 §3.1 ejecuta lo que ADR-003 §3.1/§3.6 ya fija: una sola
implementación de generación; el plan y el reporte son del motor y del CLI, no de la interfaz), y el plan de
integración declarado (`n/a`) coincide con el `test_integration: not-required` del Bolt (§3.2.2). No se emite
reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-026` (`devflow/functional/bolts/US-000.BOLT-026-integracion-shell-motor.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:12:35-03:00; `risk_class: medium`, `test_integration: not-required`, `data_classification: internal`; su DoR apoya la aceptación en la **coincidencia** de plan y reporte con el CLI |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-009` §3.1 (el shell como **host** del motor con el mismo punto de entrada que el CLI; el flujo plan→generar→reporte; la operación **asíncrona y cancelable** fuera del hilo de UI; el **handoff** al CLI), §3.2 (validación en vivo con **las reglas del motor**), §3.3 (nada de secretos en el estado del shell) y §5 (NFR `shell-ui-responsiveness` y su medición) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-003` §3.1 (punto de entrada único y tipado; el motor emite y no aplica SQL), §3.3 (determinismo), §3.4 (propiedad de artefactos: lo editable nunca se sobrescribe) y §3.6 (una sola implementación: el Modeler no puede divergir del CLI) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T22:32:47-03:00 |
| REV evidence | `REV-001` §4 F-33 y F-42, y §6 ítem 14 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0044-…` (shell), `SPEC-261016-0023-…` (motor), `SPEC-261016-0025-…` (CLI) y `SPEC-261016-0000-…` (arnés) | Los cuatro **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otro V-Bounce; **este** Bolt no lo toca) |

---

## 4. Scope

### In scope

1. **El flujo del motor desde el shell**: cargar/editar → **pedir el plan** → **confirmar** → **generar** → **mostrar
   el reporte**, sobre el **punto de entrada único y tipado** del motor (el mismo que usa el CLI), **sin** `Process`
   intermedio en el camino normal y **sin** lógica de generación en la interfaz.
2. **El plan, mostrado antes de escribir**: qué se crea, qué se actualiza, qué se **omite por ser editable** y qué
   **huérfanos** se eliminan (equivalente al `--dry-run` del motor). El workspace **no** se toca hasta que el
   usuario confirma.
3. **La operación asíncrona y cancelable**: la generación (y cualquier operación larga asociada) corre **fuera del
   hilo de interfaz**, reporta progreso y es **cancelable** mediante la cancelación del motor. Una cancelación deja
   el **workspace consistente** (apoyada en la regla de propiedad de artefactos del motor, ADR-003 §3.4) y su
   resultado dice **explícitamente** que la operación la canceló el usuario — **no** es un fallo del motor.
4. **El reporte del shell con el mismo contenido que el del CLI**, incluidas sus advertencias (artefactos escritos,
   omitidos y por qué, eliminados y faltantes; contadores; *warnings*). El shell **no** inventa un formato de reporte
   paralelo: la forma legible por máquina es el documento del CLI (Bolt 018) y su esquema está versionado.
5. **La propagación de los fallos del motor**: un modelo inválido o un `schemaVersion` fuera del rango que el set
   declara se muestran con el **error del motor y su causa** — el shell **no** genera con supuestos ni inventa un
   mensaje propio.
6. **El handoff al CLI**: mostrar y permitir **copiar** el **comando equivalente** de lo que se acaba de ejecutar,
   construido desde la **misma petición** que se le pasó al motor (modelo, salida, plantillas, y los flags que
   correspondan), para que quien quiera automatizar vea exactamente qué invocar.
7. **La ausencia de reglas duplicadas**: lo que la interfaz muestra temprano son las validaciones **del motor**
   (compartidas con el editor del Bolt 025); la interfaz **no** define reglas propias ni mensajes paralelos.
8. **La constancia** de lo que este Bolt rompe o cambia respecto del tool 1.4 (ADR-008 §3.1), como insumo de las
   notas de migración (Bolt 028).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **esqueleto del shell**, el **editor dirigido por el esquema** y el **estado del usuario** | Bolt 025 (SPEC-0044) | Acá se monta el flujo sobre ellos; no se redefinen |
| El **empaquetado**, la **asociación** del formato y la integración con el sistema operativo | Bolt 027 | Otro entregable |
| El **motor**, su punto de entrada, su plan sin escritura y su cancelación | Bolt 017 (SPEC-0023) | Se **consumen**: el shell no los reimplementa ni los simula |
| El **CLI**, su plan, su reporte y sus códigos de salida | Bolt 018 (SPEC-0025) | El shell **coincide** con él y hace **handoff** hacia él |
| El **contrato del formato** y su lector/escritor | Bolt 004 (SPEC-0010) | `Modeling` es el único lector/escritor |
| La **conexión a la base** y el **reverse** del esquema | Bolts 014/016 y el tooling | Otras superficies |
| El **enumerador de servidores** (F-34) | DISC pendiente | ADR-009 §3.7: el flujo **no depende** de esa incógnita; si se conservara, entra como Bolt propio sobre este diseño |
| Reglas de **validación** propias de la interfaz | — (nadie) | ADR-009 §3.2 lo prohíbe: las reglas son las del motor |
| Una **segunda implementación de generación** (en la UI o por `Process`) | — (nadie) | ADR-003 §3.6 y ADR-009 §3.1 |

**Regla de aporte (bounded, explícita).** Este Bolt implementa **la costura** entre el shell y el motor: orquesta el
punto de entrada, muestra el plan y el reporte, cancela y arma el comando equivalente. **No** implementa generación,
**no** valida por su cuenta y **no** toca el motor, el CLI, el contrato, el editor ni el estado. Si algo de eso
hiciera falta, se **detiene** y se reporta contra el Bolt dueño.

---

## 5. Prerequisites and baseline

- **Bolt 025 y su SPEC** (`SPEC-261016-0044-…`): el **esqueleto del shell**, el motor **hosteado en proceso**, el
  editor y el estado del usuario sobre los que este flujo se monta.
- **Bolt 017 y su SPEC** (`SPEC-261016-0023-…`): el **punto de entrada único y tipado** del motor, su **resultado**
  (artefactos escritos/omitidos, *warnings*, errores, digests), el **plan sin escritura** y el `CancellationToken`
  del contexto de render.
- **Bolt 018 y su SPEC** (`SPEC-261016-0025-…`): el **CLI** con el vocabulario del **plan**, del **reporte** y de
  los **códigos de salida** con los que el shell debe coincidir y hacia el que hace handoff.
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el build y el arnés.
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error.
- **Estado del árbol 2.0:** en construcción por el V-Bounce del Bolt 001; este Bolt escribe su SPEC y, **en su**
  V-Bounce, la costura del flujo.
- **Mediciones que este Bolt aporta:** la **respuesta de la interfaz** y la **cancelación** (NFR
  `shell-ui-responsiveness`, tramo de generación) se miden sobre el **modelo de referencia** como el ADR indica
  (revisión del patrón asíncrono + prueba de aceptación), y su resultado se registra en el MEM.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-026`), **1–2 V-Bounces** esperados. El orden va de la costura al cierre: primero el
> flujo con su plan, después la asincronía y la cancelación, después la coincidencia con el CLI, después el handoff
> y la propagación de fallos, y al final la evidencia.

### Phase A — El flujo sobre el punto de entrada único

**Duration:** ~3h de ciclo — **Complexity:** High

#### A.1 Pedir el plan, mostrarlo y confirmar

El shell arma **una** petición al motor (modelo, carpeta de salida, set de plantillas y opciones) y pide el **plan**
(equivalente al `--dry-run`): qué se **crea**, qué se **actualiza**, qué se **omite por ser editable** y qué
**huérfanos** se eliminan, con sus conteos. El plan se muestra **antes** de escribir nada y el workspace **no** se
toca hasta que el usuario confirma: la confirmación es el único punto que dispara la escritura.

#### A.2 Generar y mostrar el reporte

Al confirmar, el shell invoca **la misma** capacidad del motor sobre la **misma** petición y muestra su
**resultado**: artefactos escritos, omitidos (y por qué), eliminados, *warnings* y errores. El motor **emite** los
artefactos según su regla de propiedad (ADR-003 §3.4): lo que el motor posee se reescribe, lo editable **nunca** se
sobrescribe y su deriva se advierte — el shell **muestra** esos avisos, no decide sobre ellos.

#### A.3 Una sola implementación, sin proceso intermedio

El camino normal **no** lanza un proceso externo ni duplica lógica: el shell y el CLI consumen **el mismo** punto de
entrada. Si el shell necesitara algo que la superficie del motor no expone, la respuesta es **detenerse y reportarlo
contra el Bolt 017**, nunca reimplementarlo.

**Files created:**
- `2.0/src/CooperatorModeler/Flow/GenerationFlow.cs` — la orquestación plan → confirmar → generar → reporte
- `2.0/src/CooperatorModeler/Flow/GenerationPlanViewModel.cs` — el plan exhibido (creaciones, actualizaciones, omisiones, huérfanos, conteos)
- `2.0/src/CooperatorModeler/Flow/GenerationResultViewModel.cs` — el resultado/reporte exhibido
- `2.0/tests/Cooperator.Modeler.Tests/PlanBeforeWriteTests.cs` — el plan se muestra antes y el workspace no se toca hasta confirmar

---

### Phase B — Asíncrono, con progreso y cancelable

**Duration:** ~3h de ciclo — **Complexity:** High

#### B.1 Fuera del hilo de interfaz

Toda operación larga —la generación y la E/S prolongada que dispare— corre **fuera del hilo de interfaz**, con
**progreso** y sin bloquear la ventana. Es el reemplazo explícito y medido del `Application.DoEvents()` del tool de
1.4: no hay bombeo manual de mensajes para "disimular" el bloqueo, hay operación asíncrona de verdad.

#### B.2 La cancelación, y su desenlace

La cancelación viaja al motor (su `CancellationToken`) y el flujo la respeta: al cancelar, el workspace queda
**consistente** —lo ya escrito es lo que el motor declaró como propio y completo por artefacto, y no quedan
escrituras a medias— apoyándose en la regla de propiedad y en la escritura por artefacto del motor (ADR-003 §3.4).
El desenlace se muestra **explícitamente** como *cancelado por el usuario*: no se reporta como un fallo del motor
ni como un error inesperado.

**Files created:**
- `2.0/src/CooperatorModeler/Flow/GenerationRunner.cs` — ejecución asíncrona con progreso y cancelación
- `2.0/src/CooperatorModeler/Flow/CancellationOutcome.cs` — el desenlace de cancelación, explícito en el resultado
- `2.0/tests/Cooperator.Modeler.Tests/AsyncAndCancellationTests.cs` — 0 operaciones largas en el hilo de UI; cancelación con workspace consistente y desenlace explícito

---

### Phase C — La coincidencia con el CLI

**Duration:** ~2,5h de ciclo — **Complexity:** Medium

#### C.1 Mismo contenido, no un formato paralelo

El reporte que el shell muestra contiene **lo mismo** que el reporte del CLI sobre el **mismo modelo**: los mismos
artefactos, los mismos contadores y las mismas advertencias. El shell **no** define un formato propio ni un
vocabulario propio: la forma legible por máquina es el **documento del CLI** (con su esquema versionado, Bolt 018) y
el shell lo **renderiza** para el humano.

#### C.2 La verificación de la coincidencia

La evidencia es comparativa y directa: sobre el modelo de referencia, el flujo del shell y el mismo motor invocado
por el CLI producen el **mismo** resultado (mismos artefactos, mismos contadores, mismas advertencias). Es, además,
la prueba de que **no** existe una segunda implementación de generación: si divergieran, habría dos
comportamientos, que es lo que ADR-003 §3.6 prohíbe.

**Files created:**
- `2.0/src/CooperatorModeler/Flow/ReportMapper.cs` — el paso del resultado del motor al reporte exhibido, sin vocabulario propio
- `2.0/tests/Cooperator.Modeler.Tests/ReportMatchesCliTests.cs` — comparación de contenido con el CLI sobre el modelo de referencia (mismos artefactos, contadores y advertencias)

---

### Phase D — Fallos del motor, handoff al CLI y cierre

**Duration:** ~3h de ciclo — **Complexity:** Medium

#### D.1 Los fallos, tal como el motor los reporta

Un modelo inválido o un `schemaVersion` fuera del rango que el set declara se muestran con el **error del motor y su
causa** (el error tipado del contrato o del motor, con su mensaje accionable): el shell **no** genera con supuestos,
no degrada el error a un mensaje genérico y no ofrece "continuar igual". Lo que la interfaz muestra temprano son las
**validaciones del motor** (las mismas que usa el editor del Bolt 025): **cero** reglas duplicadas, **cero** mensajes
paralelos.

#### D.2 El comando equivalente, para el handoff

El shell muestra —y permite **copiar**— el **comando equivalente** de lo que acaba de ejecutar, construido desde la
**misma petición**: el modelo, la carpeta de salida, la carpeta de plantillas propias y los flags que correspondan
(incluido el plan sin escritura cuando lo que se ejecutó fue el plan). El comando se arma en **un** punto, desde la
petición real, para que no pueda divergir de lo que se hizo: es el puente entre el flujo humano y el flujo agente
(ADR-009 §3.1) y **no** duplica funcionalidad — el agente ejecuta el CLI, que es el mismo motor.

#### D.3 El flujo no agrega secretos

El flujo de conexión que el shell necesite para generar se resuelve **en memoria** (configuración portable o valor
protegido, ADR-004 §3.2 / ADR-007): **nada** de eso se persiste en el estado del shell ni en su log
(NFR `shell-state-secret-free` de ADR-009 §5, cuya verificación de fondo es del Bolt 025 y que este Bolt **no
puede degradar**). El SPEC incluye la comprobación: después de operar el flujo completo, el estado y el log siguen
sin material de conexión/clave.

#### D.4 La constancia

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-026.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1 para lo que cambia respecto del tool 1.4 en este flujo: la generación deja de correr en el hilo de
interfaz (adiós al bombeo manual de mensajes), el usuario **ve el plan antes** de que se escriba nada, el desenlace
es un **reporte** comparable con el del CLI en lugar de un mensaje de UI, y existe un **comando equivalente** para
automatizar lo mismo.

**Files created:**
- `2.0/src/CooperatorModeler/Flow/EngineFailurePresenter.cs` — la presentación de los fallos del motor con su causa
- `2.0/src/CooperatorModeler/Flow/CliCommandBuilder.cs` — el **único** punto que arma el comando equivalente desde la petición
- `2.0/tests/Cooperator.Modeler.Tests/EngineFailureAndHandoffTests.cs` — fallos propagados y comando equivalente fiel a la petición ejecutada
- `2.0/docs/api-removals/US-000.BOLT-026.json` — constancia de rupturas y cambios del flujo (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-009/ADR-003, expresados como Given/When/Then verificables.

### AC-1: El plan se muestra antes de escribir

**Given** un modelo abierto y una carpeta de salida
**When** el usuario pide el plan
**Then** se muestran las creaciones, actualizaciones, **omisiones por editable** y **huérfanos** con sus conteos, y
el workspace queda **intacto** hasta que el usuario confirma.

### AC-2: El reporte coincide con el del CLI

**Given** el modelo de referencia
**When** se genera desde el shell y, por separado, con el CLI
**Then** el reporte contiene los **mismos** artefactos, los **mismos** contadores y las **mismas** advertencias, y el
shell no usa un vocabulario ni un formato propios.

### AC-3: Asíncrono, con progreso y cancelable

**Given** una generación en curso sobre el modelo de referencia
**When** el usuario la cancela a mitad
**Then** **0** operaciones largas corrieron en el hilo de interfaz, la ventana respondió durante la operación, el
workspace quedó **consistente** (sin artefactos a medias) y el desenlace se reporta como **cancelado por el
usuario**, no como fallo del motor (NFR `shell-ui-responsiveness`).

### AC-4: Los fallos del motor se propagan con su causa

**Given** un modelo inválido y un modelo con `schemaVersion` fuera del rango que el set declara
**When** el flujo intenta generar
**Then** se muestra el **error del motor** con su causa y **no** se generó nada; la interfaz no ofrece continuar con
supuestos.

### AC-5: El comando equivalente es fiel y copiable

**Given** una operación ejecutada (o un plan pedido)
**When** el shell muestra el comando equivalente
**Then** el comando refleja **exactamente** la petición ejecutada (modelo, salida, plantillas y flags, incluido el
plan cuando lo que se hizo fue planificar) y se puede **copiar**.

### AC-6: Ninguna regla de validación duplicada

**Given** un modelo con un problema que el motor detecta
**When** la interfaz lo muestra temprano
**Then** el mensaje es el **del motor** y **no** existe una regla ni un mensaje paralelo en el flujo.

### AC-7: Una sola implementación de generación

**Given** el camino normal del flujo
**When** se revisa su implementación
**Then** consume el **punto de entrada único** del motor, **no** lanza un proceso intermedio y **no** contiene lógica
de generación, de propiedad de archivos ni de comparación de contenido.

### AC-8: El flujo no agrega secretos al estado del shell

**Given** una sesión que abre, conecta, planifica, genera y cierra
**When** se inspeccionan el estado del shell y su log
**Then** **0** cadenas de conexión, valores protegidos o material de claves aparecen en ellos (el NFR
`shell-state-secret-free` sigue en pie después del flujo).

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde y
`git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "el shell invoca **la misma** capacidad del motor que el CLI… sin proceso intermedio… sin lógica de generación" | Fase A: flujo sobre el punto de entrada único | AC-7 · revisión de la implementación + `PlanBeforeWriteTests` |
| "el flujo produce el **plan**… y lo muestra **antes** de generar" | Fase A.1 | AC-1 · `PlanBeforeWriteTests` |
| "el **reporte** mostrado tiene el mismo contenido que el del CLI, incluidas sus advertencias" | Fases A.2 y C | AC-2 · `ReportMatchesCliTests` |
| "la generación corre **fuera del hilo de la interfaz** y es **cancelable**, y una cancelación deja el espacio de trabajo **consistente**" | Fase B | AC-3 · `AsyncAndCancellationTests` + aceptación sobre el modelo de referencia |
| "ante un modelo inválido o fuera del rango… el shell muestra el fallo del motor con su causa" | Fase D.1 | AC-4 · `EngineFailureAndHandoffTests` |
| "el shell muestra el **comando equivalente**… para copiarlo" | Fase D.2 | AC-5 · `EngineFailureAndHandoffTests` |
| "**ninguna** regla de validación se duplica" | Fase D.1 | AC-6 · `NoDuplicatedValidationTests` (compartido con el Bolt 025) |
| "este flujo **no** incorpora a la configuración persistida del shell ni credenciales ni material secreto" | Fase D.3 | AC-8 · inspección del estado y del log tras el flujo |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Plan antes de escribir (`PlanBeforeWriteTests`):** sobre los *view-models* y la orquestación (sin abrir
  ventanas): el plan se arma desde el resultado del motor; el workspace no cambia hasta la confirmación (hash del
  árbol antes/después del paso de plan).
- **Coincidencia con el CLI (`ReportMatchesCliTests`):** mismo modelo, el flujo del shell y el CLI producen los
  mismos artefactos, contadores y advertencias; el mapeo del reporte no introduce vocabulario propio.
- **Asincronía y cancelación (`AsyncAndCancellationTests`):** 0 operaciones largas en el hilo de interfaz (revisión
  del patrón + test de la orquestación); cancelación con desenlace explícito y workspace consistente; ausencia de
  bombeo manual de mensajes.
- **Fallos y handoff (`EngineFailureAndHandoffTests`):** modelo inválido y `schemaVersion` fuera de rango → error del
  motor con su causa y **sin** artefactos; el comando equivalente refleja la petición ejecutada (y el flag de plan
  cuando correspondió).
- **Sin reglas propias:** los mensajes tempranos son los del motor (caso compartido con el Bolt 025).
- **Sin secretos:** tras el flujo, el estado y el log del shell no contienen material de conexión/clave.
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** modelo sin cambios respecto del workspace (plan vacío: no se escribe nada); artefacto editable
  divergente (se **omite** con *warning* visible); huérfano a eliminar con manifiesto y **sin** manifiesto (modo
  conservador: no se borra); cancelación inmediata (antes de la primera escritura); cancelación después de la primera
  escritura; set de plantillas con overrides del consumidor (el comando equivalente los refleja); importes de flags
  incompatibles (`--dry-run` con `--force`); modelo con `schemaVersion` menor a la soportada (se acepta).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio.

### 8.2 Integration test level

- **`n/a`** — el Bolt declara `test_integration: not-required` y el plan del frontmatter dice `n/a` con su razón: el
  shell hospeda el motor **en el mismo proceso** y su plan y su reporte se producen sobre el archivo de modelo, sin
  base de datos, cola ni servicio externo. La **respuesta de la interfaz** y la **cancelación** se miden en la
  **aceptación** sobre el modelo de referencia como el ADR indica (revisión del patrón asíncrono + prueba de
  aceptación), y su resultado se registra en el MEM; la E/S de modelo en un perfil real es del ciclo de integración
  del Bolt 025.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (§3.6).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El shell compila en `net10.0-windows` con advertencias tratadas como error | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (plan, reporte, asincronía, cancelación, fallos, handoff) | `pass` — esperado (AC-1…AC-8) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del flujo es el **resultado del motor**: se ejerce desde los *view-models* sin abrir ventanas | `pass` — esperado (AC-1, AC-2) |
| SAST | V-Bounce | El flujo orquesta el motor y arma un comando de CLI: se revisa la construcción del comando (sin inyección de shell) y el manejo de rutas | `pass` — esperado (AC-5) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas por conveniencia; si alguna hiciera falta, entra con las reglas de ADR-001 §3.4 | `pass` — esperado |
| OWASP Top 10 — static review | V-Bounce | Sin superficie de red ni servicio; el flujo no aplica SQL (emite artefactos) | `n/a` — sin superficie alcanzable desde fuera del proceso |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | **0** secretos en el estado y en el log tras operar el flujo (NFR `shell-state-secret-free`) | `pass` — esperado (AC-8) |
| Hallucination lint | V-Bounce | Tipos, miembros y rutas referenciados existen (o se declaran como aportados) | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `data_classification: internal`; fixtures sintéticos | `pass` — esperado |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; el motor y su plan existen **antes** (Bolt 017) y el CLI fija el vocabulario (Bolt 018) | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas del flujo sobre el mismo modelo (mismos artefactos y mismo reporte) | `pass` — esperado (AC-2) |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| Shell UI responsiveness — tramo de generación (ADR-009 §5) | V-Bounce + aceptación | **0** operaciones de generación en el hilo de UI; cancelable; la cancelación deja el workspace consistente; la ventana responde | `pass` — esperado (AC-3, medido en la aceptación sobre el modelo de referencia) |
| Shell state secret-free (ADR-009 §5) | V-Bounce | **0** conexiones/valores protegidos/claves en el estado y en el log **después** de operar el flujo | `pass` — esperado (AC-8) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | El flujo vive en la aplicación del shell: no retira superficie de los paquetes del framework | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | No aplica: `test_integration: not-required` (plan `n/a`) | `n/a` — verificación en proceso (§3.2.2) |
| Contract / E2E against the real providers | Integration test | El "contrato" verificado end-to-end es la **coincidencia con el CLI** sobre el mismo modelo, y se verifica en la V-Bounce | `n/a` — sin dependencias reales |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable de red |
| Perf-smoke (p95/p99) | Integration test | El costo de generación lo mide el Bolt 017; acá no hay umbral de latencia | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | La observabilidad del shell es su log y el reporte del motor (§11) | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: not-required` → `integration_test_plan.applicability: n/a`
> con su razón. Los dos NFR de ADR-009 §5 se miden **en la aceptación** como el propio ADR indica, y su resultado se
> registra en el MEM.

---

## 10. Security and data

- **Sin superficie de red ni servicio:** el flujo opera sobre el archivo de modelo y la carpeta de salida; el motor
  **emite** artefactos y **no** aplica SQL (ADR-003 §3.1), así que este Bolt no abre conexiones.
- **NFR `shell-state-secret-free` intacto:** la conexión que el shell necesite se resuelve **en memoria**; el flujo
  **no** persiste credenciales, valores protegidos ni material de claves en el estado ni en el log — y el criterio lo
  verifica **después** de operar el flujo (AC-8), porque es exactamente en el camino de conexión donde un secreto se
  colaría.
- **El comando equivalente no introduce un canal nuevo:** es texto para el usuario copiar; se construye desde la
  misma petición y no se ejecuta desde el shell (el shell hospeda el motor en proceso; el agente ejecuta el CLI por
  su cuenta).
- **Diagnóstico sin secretos:** los fallos se muestran con la causa del motor y se registran en el log por usuario del
  shell, sin material de conexión/clave.
- **Clasificación:** `data_classification: internal` (espeja el Bolt). El modelo describe estructura; los fixtures
  son sintéticos.
- **Integridad del árbol congelado:** `1.4/**` no se modifica; su código es la evidencia del punto de partida del
  flujo.

---

## 11. Monitoring and observability

- **La observabilidad del flujo es doble:** el **plan** antes (qué va a pasar) y el **reporte** después (qué pasó,
  con contadores y advertencias), más el **log por usuario** del shell para el diagnóstico de un fallo.
- **Progreso y cancelación visibles:** la operación larga reporta progreso y su cancelación es explícita en el
  resultado — el usuario nunca queda adivinando si la operación terminó, falló o la canceló él.
- **Evidencia versionada:** la constancia de cambios del flujo (`2.0/docs/api-removals/US-000.BOLT-026.json`) y las
  salidas de la suite en `2.0/artifacts/<timestamp>/`; la medición de los NFR queda en el MEM.
- **Sin telemetría de servicio:** el shell no envía nada a ninguna parte (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration (del usuario):** el flujo cambia de forma visible y **a mejor**: antes se generaba directamente (con el
  modo completo que podía sobrescribir lo editable) y el desenlace era un mensaje; ahora hay **plan antes**,
  **reporte después** y **cancelación**. Para quien automatice, el camino es el **CLI** y el shell le **muestra el
  comando** que corresponde a lo que hizo.
- **Compatibility:** el flujo vive en la aplicación del shell (superficie nueva): no retira superficie de los
  paquetes del framework. Lo que cambia respecto del tool 1.4 está declarado en la constancia (§6/D.4) y va a las
  notas de migración (Bolt 028).
- **Rollback:** revertir el commit del Bolt. Un rollback parcial —por ejemplo, conservar el flujo pero volver a
  generar en el hilo de interfaz— es inválido: reintroduciría el bloqueo de la ventana que el NFR prohíbe y el modo
  sin plan que el ADR retira.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Dos comportamientos de generación** (el de la interfaz y el del CLI) | 2 | 4 | El shell consume la **misma** capacidad del motor y la evidencia es la **coincidencia** de plan y reporte sobre el mismo modelo (AC-2, AC-7) |
| **Interfaz congelada** por una operación larga (el legacy usaba bombeo manual de mensajes) | 2 | 3 | Toda operación larga corre fuera del hilo de UI, con progreso y cancelación; se mide en la aceptación (AC-3, NFR) |
| **Cancelación que deja el espacio de trabajo a medias** | 2 | 3 | Se apoya en la regla de propiedad y la escritura por artefacto del motor (ADR-003 §3.4) y se **demuestra** dejando el workspace consistente (AC-3) |
| **Una cancelación leída como fallo del motor** | 2 | 3 | El desenlace de cancelación es explícito en el resultado (§6/B.2) |
| **Reglas de validación duplicadas** y divergentes | 2 | 3 | La interfaz muestra las del motor y nunca mensajes paralelos (AC-6) |
| **Un modelo fuera de rango que se genere igual** | 2 | 2 | El shell propaga el fallo del motor en lugar de asumir (AC-4) |
| **Secretos que se cuelan al estado por el flujo de conexión** | 2 | 4 | La conexión se resuelve en memoria y la comprobación se hace **después** de operar el flujo (AC-8) |
| **El comando equivalente diverge de lo ejecutado** (una automatización que hace otra cosa) | 2 | 3 | Se arma en **un** punto, desde la petición real, incluido el flag de plan (AC-5) |
| Solapamiento con los Bolts 025 (esqueleto/editor/estado) y 027 (empaquetado/asociación) | 2 | 2 | Frontera declarada en el DoR y en §4; se resuelve por contrato de entregables |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| El **plan** es el del motor (no una estimación de la interfaz) | Es lo que permite mostrar "qué va a pasar" sin escribir nada y sin duplicar la lógica de propiedad de archivos (ADR-003 §3.4); una estimación propia divergiría del resultado real |
| El **reporte** del shell usa el contenido del CLI y **no** un vocabulario propio | ADR-009 §3.1 y ADR-003 §3.6: una sola implementación de generación y un solo contrato de salida; el shell renderiza, no redefine |
| El **comando equivalente** se arma desde la **misma petición** ejecutada, en un único punto | Un comando construido "a mano" desde la UI divergiría de lo que se ejecutó, y el handoff dejaría de ser confiable para automatizar |
| La **cancelación** es un desenlace explícito del usuario, no un error | El usuario necesita distinguir "lo cancelé" de "falló"; confundirlos convierte una decisión en un incidente |
| La **E/S y la generación** van fuera del hilo de UI, sin bombeo manual de mensajes | Es el reemplazo del antipatrón del tool 1.4 y lo que mide el NFR `shell-ui-responsiveness`; el bombeo manual "disimula" el bloqueo en lugar de eliminarlo |
| **Ninguna** regla de validación propia en el flujo | ADR-009 §3.2: reglas duplicadas divergen y la UI miente sobre lo que el motor aceptará; las validaciones tempranas son las del motor |
| Los **fallos** se muestran tal como el motor los reporta | Un fallo degradado a mensaje genérico hace que el usuario (o el dev-validator) pierda la causa; el motor ya produce errores accionables |
| El flujo **no** persiste nada del camino de conexión | El NFR `shell-state-secret-free` es duro: una comodidad de UI no puede convertirse en una fuga de credenciales del cliente |

---

## 15. Stop conditions

1. **Cambio material en una fuente gobernada** (Bolt, ADR-009/ADR-003, el REV usado, baseline de código): detener,
   revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca dos revisiones (G16).
2. **El motor no expone el plan, el progreso o la cancelación** que el flujo necesita: detener y **reportar contra el
   Bolt 017** (es su superficie). El shell **no** los simula ni los reimplementa.
3. **El reporte del shell no puede coincidir con el del CLI** (vocabulario o contenido divergentes): detener y
   escalar — la coincidencia **es** el criterio del Bolt y su DoR; si el CLI cambia su contrato, el camino es su
   Bolt/SPEC, no un formato propio del shell.
4. **Se necesitaría una segunda implementación de generación** (para "adelantar" parte del plan o generar por
   `Process`): detener; ADR-003 §3.6 y ADR-009 §3.1 lo prohíben.
5. **Se necesitarían reglas de validación propias** en la interfaz: detener; ADR-009 §3.2 lo prohíbe (las reglas son
   las del motor, y su exposición es del Bolt 017/004).
6. **El flujo persistiría un secreto** (una cadena de conexión, un valor protegido) en el estado o el log: detener;
   el NFR `shell-state-secret-free` no admite excepción.
7. **El trabajo deriva al esqueleto/editor/estado** (Bolt 025) **o** al empaquetado/asociación (Bolt 027): detener y
   devolverlo a su Bolt.
8. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
9. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite o build): detener y escalar la secuencia.
10. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, el conjunto exacto de flags que el comando
    equivalente debe reflejar para una opción que el ADR no enumera): parar y reportar; no se rellena el hueco con una
    suposición (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–D implementadas (o, si el presupuesto se agota, progreso en el MEM y pendiente explícito para un
      V-Bounce nuevo)
- [ ] Todos los criterios de aceptación (AC-1…AC-9) verificados
- [ ] Suite hermética verde (§8.1); el plan de integración es `n/a` con su razón
- [ ] El **plan** se muestra antes de escribir y el workspace queda intacto hasta la confirmación
- [ ] El **reporte** coincide con el del CLI (mismos artefactos, contadores y advertencias)
- [ ] La operación es **asíncrona y cancelable**, con la cancelación **demostrada** dejando el workspace consistente
      y su desenlace explícito; los dos NFR de ADR-009 §5 medidos y registrados en el MEM
- [ ] El **comando equivalente** se muestra y se copia, fiel a la petición ejecutada
- [ ] El código sigue ADR-009 (§3.1/§3.2/§3.3) y ADR-003 (§3.1/§3.3/§3.4/§3.6)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la lista de cambios
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-026`](../functional/bolts/US-000.BOLT-026-integracion-shell-motor.md) y su manifiesto
  [`US-000.BOLT-026-…json`](../metrics/bolts/US-000.BOLT-026-integracion-shell-motor.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) §3.1/§3.2/§3.3/§5 · [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1/§3.3/§3.4/§3.6
- SPEC prerrequisito directo: [`SPEC-261016-0044`](./SPEC-261016-0044-shell-wpf.md) (esqueleto del shell, motor hosteado y estado)
- SPECs previos: [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) (motor y punto de entrada) ·
  [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) (CLI: plan, reporte y códigos) ·
  [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y build)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-33, F-42 y §6 ítem 14
- Evidencia de código 1.4 (verificada en esta sesión): `Tools/CooperatorModeler/GenerationDialog.cs`
  (`Application.DoEvents()` alrededor de `CodeGenerator.GenerateStoredProcedures`/`GenerateClasses`; los desenlaces
  como mensajes de UI y `Process.Start(solFile)` en las líneas 176 y 207),
  `Tools/CooperatorModeler/FolderHelper.cs` (`CopyFolder`, cuyo modo completo sobrescribía también lo editable) y
  `Tools/CooperatorModeler/MainForm.cs:509-512` (el único punto de automatización: abrir el tool con un archivo)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-026`: flujo plan → confirmar → generar → reporte sobre el punto de entrada único del motor, ejecución asíncrona con progreso y cancelación fuera del hilo de interfaz (reemplazo del bombeo manual de mensajes del tool 1.4), reporte coincidente con el CLI, propagación de los fallos del motor y handoff con el comando equivalente a la vista |

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
| **review.reviewers** | [Dev-validator + dueño de arquitectura (frontera shell↔motor y coincidencia con el CLI) + dueño de seguridad si se evalúa el camino de conexión del flujo] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **La evidencia de aceptación es la coincidencia con el CLI** (lo declara el DoR del Bolt): mismos artefactos,
   contadores y advertencias sobre el mismo modelo. No es una prueba de interfaz aislada. Confirmar.
2. **Medición del NFR `shell-ui-responsiveness`:** acá se mide el tramo de **generación** (respuesta durante la
   operación larga + cancelación que deja el workspace consistente); el tramo de **E/S de modelo** se mide en el Bolt
   025. El ADR la describe como revisión del patrón asíncrono **más** prueba de aceptación sobre el modelo de
   referencia: se registra en el MEM. Confirmar.
3. **Frontera:** el flujo se monta sobre el esqueleto/editor/estado del Bolt 025 y **no** implementa empaquetado ni
   asociación (Bolt 027). Confirmar el recorte.
4. **El comando equivalente** refleja la **petición ejecutada**, incluido el flag de plan cuando lo que se ejecutó fue
   planificar. Confirmar que las opciones a reflejar son las del contrato del CLI (Bolt 018).
5. **Cancelación ≠ error:** el desenlace se reporta explícitamente como cancelado por el usuario. Confirmar que es la
   distinción esperada por los usuarios y por los criterios de aceptación.
6. **Sin secretos:** el flujo no persiste nada del camino de conexión; la comprobación se hace **después** de operar
   el flujo, sobre el estado y el log del shell.
