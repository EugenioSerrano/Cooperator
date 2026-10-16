---
id: "SPEC-261016-0043"
title: "Herramienta de protección de valores como shell delgado: un paso valor → valor protegido sobre el mismo motor, sin clave embebida"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-024" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-007", "ADR-009", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, build compartido y gates de invariantes (Bolt 001)
  - "SPEC-261016-0035-resolutor-aesgcm.md" # el motor de protección: esquema vigente, clave activa, `kid`, AAD y provisión de claves (Bolt 022)
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "confidential" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (refactor): produce un valor protegido desde un valor de entrada y el resultado se verifica resolviéndolo en el mismo proceso — sin base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:47:21-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:47:21-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:47:21-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-024 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:09:32; acknowledgment «Encryptor sobrevive como shell delgado (ADR-009 3.6)»), ADR-007/ADR-009/ADR-001 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class medium → autonomy_level L3; data_classification confidential; test_integration not-required → applicability n/a (razón). Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: shell delgado `net10.0-windows` con UI mínima y un solo paso (valor + nombre de clave de destino = AAD → valor protegido con la clave activa), delegando íntegro en el motor (Bolt 022); manejo del valor en claro (enmascarado, sin persistencia/nunca mostrado); retiro de la protección propia (0 primitivas/0 claves/0 segundo formato); implementación única (misma operación que el CLI); búsqueda del valor en claro en todo lo escrito = 0 (AC-3)."
    - "ADJUDICACIÓN (a) stack WPF — CONFIRMADA: §14 fila 1 fija WPF sobre `net10.0-windows` (in-box, sin dependencias nuevas), coherente con ADR-009 §3.5 que retira `System.Windows.Forms` del tooling (el stack WinForms de la herramienta anterior) y con ADR-001 §3.1/§3.5; §15 declara stop si WPF no fuera viable."
    - "ADJUDICACIÓN (b) AAD obligatorio — CONFIRMADA: §A.2/§10/§14 fila 2 exigen el nombre completo de la clave de configuración de destino como entrada (el AAD de ADR-007 §3.1); sin él el valor protegido quedaría sin atar a su slot. La alternativa (derivar el AAD) exigiría conocer la configuración del consumidor (Bolt 011), fuera del paso único."
    - "ADJUDICACIÓN (c) cruce 023↔024 — CONFIRMADA, sin prerrequisito duro: §5/§D.1/§14 fila 7 declaran la verificación de «una sola implementación» en dos patas —la estática (sin protección propia) es concluyente sobre el riesgo principal, y la compartida se verifica contra la superficie del motor; si el Bolt 023 no se ejecutó al cerrar, el MEM lo registra y la pata compartida queda verificada contra el motor, sin bloquear el shell. El shell no queda encadenado al CLI."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-024`. El motor de protección,
     sus esquemas y la provisión de claves son del Bolt 022 (**consumidos**); los comandos de inspección,
     migración y rotación son del Bolt 023; el shell del Modeler es del Bolt 025; la corrección del defecto de
     F-23 es de su BUG/Bolt.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ SIN CLAVE EMBEBIDA NI PROTECCIÓN PROPIA: la herramienta **no** implementa protección, **no** guarda claves
     ni constantes de derivación y **no** conserva la copia del cifrado anterior. Protege **con el motor** y
     escribe un valor `aesgcm:` con la clave activa (ADR-009 §3.6, ADR-007 §3.5).
-->

# SPEC-261016-0043 — Herramienta de protección de valores como shell delgado: un paso valor → valor protegido sobre el mismo motor, sin clave embebida

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 12) |
| **Bolt** | [US-000.BOLT-024](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) |
| **ADRs** | [ADR-007](../adrs/ADR-007-politica-criptografica.md) · [ADR-009](../adrs/ADR-009-shell-del-modeler.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega la herramienta de escritorio de protección de valores como **shell delgado** sobre el **motor de
protección ya entregado**: **un solo paso** —valor de entrada → valor protegido con el **esquema vigente** y la
**clave activa**—, **sin clave embebida**, **sin constante de derivación** y **sin implementar protección por su
cuenta**: delega íntegramente en el motor (Bolt 022). La herramienta **sobrevive** como utilidad de escritorio
(`net10.0-windows`, entre las aplicaciones que sobreviven al ciclo por ADR-001 §3.1/§3.5) y su comando homólogo
del CLI es **aditivo**, no sustituto: ambos consumen **la misma** implementación, sin dos formatos (ADR-009 §3.6,
ADR-007 §3.5).

El problema que resuelve está documentado (REV-001 F-40 y §6 ítem 12): la herramienta anterior cifraba con una
**copia propia** del algoritmo y una **clave embebida** en el binario (`Library/Cryptography.cs`: `_CryptoKey`,
`_Iv` fijo, MD5 como derivación — el defecto de F-23). Si el shell se dejara como está, el producto tendría
**dos** implementaciones de protección y una de ellas con la clave dentro: exactamente lo que el estándar
criptográfico prohíbe.

Si no se implementa: proteger un valor desde el escritorio exige usar el CLI (y la utilidad desaparece), o peor,
sigue existiendo un camino alternativo de protección con clave embebida.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 12 ruteó el defecto de F-23 y sus herramientas: la política
criptográfica (ADR-007) y los comandos del CLI (§3.5), y ADR-009 §3.6 decidió la **suerte** de
`ConnectionStringEncriptor`: **sobrevive como shell delgado** `net10.0-windows` sobre el mismo motor, con UI
mínima, y el comando del CLI **aditivo**.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-009 §3.6** — `ConnectionStringEncriptor` **sobrevive como aplicación**: es un **shell delgado**
  `net10.0-windows` sobre el **mismo motor de protección** (ADR-007 §3.5) —**protege escribiendo un valor
  `aesgcm:` con la clave activa**, **sin clave embebida**— y **no** se retira; ADR-001 §3.1/§3.5 lo listan entre
  las aplicaciones que sobreviven al ciclo. El **comando del CLI es aditivo**, no sustituto: existe para agentes y
  pipelines, y **ambos consumen la misma implementación de protección — no hay dos implementaciones ni dos
  formatos**—. Su UI se mantiene **mínima** (su función es de un solo paso) y el detalle de pantallas **lo fija la
  SPEC**.
- **ADR-007 §3.5 (herramientas)** — la herramienta **protege escribiendo un valor `aesgcm:` con la clave activa,
  sin clave embebida**, y el comando del CLI es aditivo; **la reproducibilidad** (el `kid` usado y el esquema de
  salida) y la prohibición de **inventar claves** o generar valores por defecto cuando falta configuración.
- **ADR-007 §3.1/§3.2 (lo que se consume)** — el esquema `aesgcm:<kid>:<base64>` con su composición, el **AAD =
  nombre completo de la clave de configuración** que contiene el valor, la **clave activa** y el conjunto de
  **lectura**, y la provisión por **variables de entorno** (default) o **hook** del host: el framework consume la
  **referencia** de la clave (`kid`), **nunca el material**.
- **ADR-007 §5 (NFR *No embedded secrets*)** — **0** claves literales o derivadas de constantes en el árbol 2.0:
  es el gate que mide esta herramienta en particular (era su defecto).
- **ADR-001 §3.1/§3.5** — la herramienta es **una de las aplicaciones que sobreviven** y es **Windows-only
  explícita**; el tooling se distribuye por `dotnet publish` self-contained, **sin ClickOnce**; `System.Web` y
  `System.Windows.Forms` **no** son la dirección del tooling nuevo (ADR-009 §3.5 retira `System.Windows.Forms`
  del tooling) y el **canal de distribución** sigue siendo la decisión pendiente de ADR-001 §3.6 (ajena a este
  Bolt).

**Qué decide este SPEC, dentro de esas restricciones.** El **stack de UI** de esta utilidad (que ADR-009 §3.6
deja a la SPEC) y su forma de **un solo paso**; **qué** entradas exige el paso (el **valor** a proteger y el
**nombre de la clave de configuración de destino** —que es el **AAD**—) y de dónde toma la clave activa; la
**política de manejo del valor en claro** (no persistirlo, no mostrarlo, no registrarlo); la **forma del
diagnóstico** de los fallos del motor; y el **retiro de la copia propia de protección** que la herramienta tuviera.

**Fronteras declaradas.** El **motor de protección**, sus esquemas, la clave activa, el `kid` y la **provisión de
claves** son del **Bolt 022** (ADR-007 §3.1/§3.2); los **comandos** de inspección, migración y rotación son del
**Bolt 023**; el **shell del Modeler**, su stack y su empaquetado son de los **Bolts 025/027**; el **canal de
distribución** es de ADR-001 §3.6; y la corrección del **defecto** de F-23 es de su **BUG/Bolt** (este Bolt
retira la copia propia *de esta herramienta*, no el defecto del framework, que ya retiró el Bolt 022 al eliminar
la superficie estática).

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-024`](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) — `status: ready`, `risk_class: medium`, `data_classification: confidential`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — **F-40** (el encriptador de connection strings: herramienta candidata y su uso de la superficie estática con clave implícita) y §6 ítem 12 (el ruteo que separa el defecto, la política y sus herramientas) | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (esquema vigente, AAD y composición, que la herramienta **consume**; retiro de la superficie estática con clave implícita), §3.2 (claves: activa y de lectura; nada de secretos en logs ni reportes), §3.4 (primitivas permitidas y prohibidas), **§3.5** (la herramienta protege escribiendo un valor `aesgcm:` con la clave activa, sin clave embebida; el comando del CLI es aditivo; reproducibilidad y prohibición de inventar claves), §3.6 (qué no cubre) y §5 (NFR *No embedded secrets*) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — **§3.6** (`ConnectionStringEncriptor` **sobrevive como shell delgado** `net10.0-windows` sobre el mismo motor; UI mínima; el detalle de pantallas es de la SPEC; el comando del CLI es **aditivo**), §3.5 (limpieza del tooling: sin `System.Windows.Forms`, sin ClickOnce) y §3.8 (interplay con ADR-007) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (la herramienta entre las aplicaciones `net10.0-windows` que sobreviven; dependencias), §3.4 (política de dependencias), §3.5 (supervivencia Windows-only explícita; sin ClickOnce), §3.6 (canal de distribución pendiente) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — su SPEC canónica ([`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md)) entrega el **motor de protección**, la **clave activa**, el `kid`, el AAD y la provisión de claves | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, build compartido y gates de invariantes | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Tools/ConnectionStringEncriptor/MainForm.cs` y su `ConnectionStringEncriptor.csproj` (la herramienta
anterior: WinForms, con el uso de la superficie estática de `Library` —`Cryptography.Encrypt(string)`/`Decrypt`
sin clave— que el Bolt 022 retira, y **sin** clave propia en el tool: la clave estaba en el framework); y
`1.4/Cooperator.Framework.Library/Cryptography.cs` (líneas 40-108: `_CryptoKey` literal, `_Iv` fijo, MD5 como
derivación y las sobrecargas estáticas) — es el defecto de F-23 y **exactamente** lo que esta herramienta **no**
debe conservar ni reimplementar.

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- La **herramienta de escritorio** de protección de valores (`ConnectionStringEncriptor`, `net10.0-windows`,
  SDK-style) como **shell delgado**: **un solo paso** —valor de entrada → valor protegido—, con el **esquema
  vigente** y la **clave activa**.
- El **stack de UI** de esta utilidad y la forma de ese paso único (que ADR-009 §3.6 deja a la SPEC), con **UI
  mínima** y **sin** `System.Windows.Forms` (ADR-009 §3.5) ni ClickOnce (ADR-001 §3.5).
- Las **entradas del paso**: el **valor** a proteger y el **nombre completo de la clave de configuración de
  destino** —que es el **AAD** del valor (ADR-007 §3.1)—; la **clave activa** y su `kid` los aporta el motor
  (provisión de claves del Bolt 022), **no** la herramienta.
- La **política de manejo del valor en claro**: no persistirlo (ni en disco, ni en el estado de usuario, ni en
  historial), no mostrarlo en claro y no registrarlo; y el **diagnóstico accionable** de los fallos del motor.
- El **retiro de la copia propia de protección** (si la herramienta tuviera una) y la constancia de que **no**
  existe una ruta alternativa de protección ni un segundo formato.
- La **evidencia**: el ciclo completo valor → protegido → resuelto con una clave de prueba, el resultado del
  **gate de secretos embebidos** (ADR-007 §5) sobre esta herramienta, la constancia de la implementación única
  (misma que el comando del CLI) y la ausencia del valor en claro en cualquier artefacto.

### Out of scope

- El **motor de protección**, sus esquemas, la **clave activa**/de lectura, el `kid`, el AAD y la **provisión de
  claves**: **Bolt 022** (ADR-007 §3.1/§3.2). Acá se **consume**: la herramienta no implementa algoritmo, no
  guarda claves y no deriva material.
- Los **comandos** de inspección, migración y rotación de valores protegidos y la **capacidad legacy aislada**:
  **Bolt 023** (ADR-007 §3.3/§3.4/§3.5). Esta herramienta **no** descifra el formato viejo ni migra nada.
- El **shell del Modeler** (su stack, su editor, su estado de usuario) y el **empaquetado/asociación** del
  tooling: **Bolts 025/026/027** (ADR-009 §3.1-§3.6).
- El **canal de distribución** (feed, firma, publicación): **ADR-001 §3.6**, decisión pendiente del `tech_lead`.
- La **corrección del defecto** de F-23 en el framework: su **BUG/Bolt** (el Bolt 022 ya retiró la superficie
  estática con clave implícita que esta herramienta usaba).
- La **configuración de la aplicación generada** y su precedencia: **ADR-004 §3.4 / Bolt 021** (los valores que
  esta herramienta produce terminan en la configuración del consumidor, que es de ese Bolt).
- La **protección de valores como servicio** o cualquier forma de exposición remota: fuera del alcance del
  producto.

---

## 5. Prerequisites and baseline

- **Bolt 022 y su SPEC canónica
  ([`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md), `prerequisites`):** el **motor de protección**
  (`aesgcm:` con su composición y sus tamaños), la **operación de proteger** con **referencia explícita de
  clave**, la **clave activa** y el conjunto de lectura, el **AAD** (nombre completo de la clave de
  configuración) y la **provisión de claves** (entorno por defecto, hook del host) con sus **errores tipados**.
  Sin él no hay motor que consumir y la herramienta volvería a "implementar" protección, que es lo que el Bolt
  prohíbe. Si no está en el árbol 2.0, **parar y reportar** (§15).
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés, el build compartido
  (`Directory.Build.props`: analizadores, determinismo, warnings como error) y los gates de invariantes —
  incluido el **gate de constantes de alta entropía** con el que se mide *No embedded secrets*.
- **El comando homólogo del CLI (Bolt 023, [`SPEC-261016-0039`](./SPEC-261016-0039-comandos-de-valores-protegidos.md))
  — cruce de verificación, no prerrequisito de ejecución:** la afirmación "una sola implementación, compartida
  con el comando del CLI" (ADR-009 §3.6) tiene dos patas: (a) **estática** —esta herramienta no contiene
  protección propia, claves ni constantes— y (b) **compartida** —ambos consumen la misma API del motor—. La pata
  (b) se verifica contra la superficie del motor y, cuando el comando del CLI ya exista, sobre su código; si el
  Bolt 023 no se ha ejecutado al cerrar este V-Bounce, el MEM lo registra y la pata (b) queda verificada contra
  el motor, sin bloquear esta entrega (el shell es independiente del CLI).
- **Stack de UI decidido por esta SPEC:** WPF sobre `net10.0-windows` (in-box, sin dependencias nuevas), coherente
  con ADR-009 §3.5 —que **retira `System.Windows.Forms` del tooling**— y con ADR-001 §3.1/§3.5. La herramienta
  anterior era WinForms: **su superficie de usuario cambia** y eso se declara (§E/AC-7).
- **Claves para la verificación:** generadas **en memoria** para las pruebas (o provistas por variables de
  entorno del entorno de prueba); **nunca** comiteadas. La ausencia de clave en el operador es un **error tipado
  del motor** (Bolt 022), que esta herramienta debe mostrar como diagnóstico accionable.
- **Sin dependencias externas:** el entregable no agrega paquetes (ADR-001 §3.4) y no requiere red, base de datos
  ni servicio.
- **Decisión de ejecución registrada (no es supuesto):** el ciclo valor → protegido → resuelto se verifica **en
  proceso** (proteger con la clave activa y resolver con el resolutor), sin superficie desplegada: de ahí el plan
  de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-024`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — El proyecto y el paso único sobre el motor

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 El proyecto y su forma

Se crea `2.0/src/ConnectionStringEncriptor/` como proyecto SDK-style **`net10.0-windows`** (ADR-001 §3.1), sin
`packages.config`, sin `HintPath`, sin referencias muertas y **sin dependencias nuevas** más allá de las del
producto (el motor de protección y las bibliotecas del release). La aplicación es **Windows-only explícita**
(ADR-001 §3.5) y se distribuye por `dotnet publish` self-contained, **sin ClickOnce**.

**Files created:**
- `2.0/src/ConnectionStringEncriptor/ConnectionStringEncriptor.csproj` — el proyecto `net10.0-windows` de la utilidad
- `2.0/src/ConnectionStringEncriptor/App.xaml` / `App.xaml.cs` — arranque de la aplicación (WPF), sin lógica de protección
- `2.0/src/ConnectionStringEncriptor/ProtectValueWindow.xaml` / `.xaml.cs` — la ventana del **paso único**, con su UI mínima

#### A.2 Un solo paso: valor → valor protegido (con el AAD de destino)

La operación que expone la herramienta es **una**: dado un **valor** y el **nombre completo de la clave de
configuración de destino** (que es el **AAD** del valor, ADR-007 §3.1), produce el valor protegido con el
**esquema vigente** y la **clave activa**, y lo muestra para copiar. La herramienta:

- **no** elige ni configura el algoritmo ni el esquema: eso es del motor;
- **no** conoce ni administra claves: el `kid` y el material los aporta la **provisión de claves** del motor
  (entorno por defecto; hook del host si el operador lo registra);
- **no** implementa ninguna primitiva criptográfica ni guarda constante alguna de derivación (ADR-007 §3.4);
- **no** ofrece una segunda ruta (ni "cifrar con el formato viejo", ni un modo que evite el AAD): proteger **es**
  este paso.

**Files created:**
- `2.0/src/ConnectionStringEncriptor/Protection/ProtectValueStep.cs` — el paso único: entrada (valor + clave de configuración de destino), llamada al **motor** y salida (valor protegido)
- `2.0/src/ConnectionStringEncriptor/Protection/ProtectionResult.cs` — el resultado del paso (valor protegido, `kid` usado y esquema de salida) sin exponer material

---

### Phase B — El manejo del valor en claro y el diagnóstico

**Duration:** 3h total cycle — **Complexity:** Medium

#### B.1 El valor en claro no se persiste, no se muestra y no se registra

La herramienta trata el valor de entrada como **material sensible transitorio** (es el punto por el que pasa una
credencial):

- la entrada se captura **enmascarada** y **no** se muestra en claro en la ventana;
- el valor **no** se escribe en disco, ni en el estado de usuario, ni en un historial, ni en un archivo temporal,
  ni en un registro de diagnóstico;
- el **resultado** (el valor protegido) sí se muestra y se puede copiar —es el artefacto que el operador
  necesita—, y la ventana **no** ofrece "ver el valor original";
- el estado de la ventana se limpia al cerrar el paso (no queda el valor en memoria retenida más allá de lo
  necesario ni en el estado persistente de la aplicación).

#### B.2 El diagnóstico de los fallos del motor

Un fallo del motor —clave activa ausente o mal formada, `kid` sin clave, proveedor del host que falla, valor de
entrada inválido según el contrato— se muestra como **error accionable** (qué falta y qué hacer: la clave de
entorno o el hook, con la **referencia** afectada), **nunca** como "valor inválido" silencioso ni como un valor
protegido dudoso. La herramienta **no** captura el error para seguir: si el motor no puede proteger, no hay
resultado.

**Files created:**
- `2.0/src/ConnectionStringEncriptor/Protection/ProtectionDiagnostics.cs` — la traducción de los errores tipados del motor (Bolt 022) a un mensaje accionable de la utilidad, sin material ni valor

---

### Phase C — El retiro de la protección propia

**Duration:** 2h total cycle — **Complexity:** Low

#### C.1 Lo que deja de existir en esta herramienta

Se retira —o simplemente **no se porta**— cualquier copia propia del cifrado anterior que la herramienta hubiera
tenido: **no** hay código de cifrado propio, **no** hay clave ni constante de derivación, **no** hay segundo
formato y **no** hay un modo "compatibilidad" que lea o escriba el formato viejo (esa capacidad es del
**Bolt 023**, aislada y bajo su excepción, y **no** de una utilidad de escritorio). La verificación es de
**ausencia**: búsqueda de primitivas criptográficas, de material de clave y de formatos alternativos en el
proyecto de esta herramienta.

> La superficie **estática** con clave implícita (`Cryptography.Encrypt(string)`/`Decrypt(string)`) la retiró el
> Bolt 022 del framework: esta herramienta **no** la reintroduce ni la reimplementa, y no depende de ella.

---

### Phase D — La implementación única y la alineación con el CLI

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 Misma implementación, mismo formato

La herramienta consume **la misma** API del motor que el comando homólogo del CLI (ADR-009 §3.6): el valor que
produce es **byte-compatible en formato** con el que produce el comando (mismo esquema, misma composición, misma
codificación), de modo que un valor protegido con la utilidad se resuelve igual —por el resolutor del Bolt 022—
que uno producido por el CLI y viceversa. La verificación tiene dos patas (§5):

- **estática:** la utilidad no contiene una segunda implementación (no hay primitivas, no hay claves, no hay
  composición propia del valor);
- **compartida:** la utilidad y el comando del CLI invocan **la misma** operación de protección del motor (se
  verifica sobre la superficie del motor y, cuando el comando exista, sobre su código).

#### D.2 El algoritmo no es una decisión del usuario

El operador **no** elige el algoritmo ni el esquema: sólo aporta el valor y la clave de configuración de destino
(y el entorno aporta la clave). Cualquier selector de algoritmo o de "fuerza" sería un camino a una decisión que
la política ya tomó (ADR-007 §3.1/§3.4) y **no** se emite.

---

### Phase E — Evidencia

**Duration:** 2h total cycle — **Complexity:** Low

- **El ciclo completo:** valor de entrada → valor protegido (con la clave activa de prueba) → **valor resuelto**
  por el resolutor, comparando entrada y salida: la prueba de que el shell produce un valor **útil**, no sólo
  plausible.
- **Gate de secretos embebidos** (ADR-007 §5) sobre esta herramienta: **0** claves literales, **0** constantes de
  derivación, **0** material en configuración o en fixtures del proyecto.
- **Constancia de la implementación única:** la inspección de que la utilidad no tiene protección propia y de que
  consume la misma operación del motor que el comando del CLI (con su pata pendiente declarada si el Bolt 023 no
  se ha ejecutado, §5).
- **Ausencia del valor en claro:** la verificación de que el valor de entrada **no** quedó en disco, en el estado
  de usuario, en registros ni en archivos temporales de la corrida.
- **Diagnóstico accionable:** los casos de fallo del motor (clave ausente, `kid` sin clave, proveedor que falla)
  con su mensaje y la referencia afectada, **sin** material ni valor.
- **Superficie de usuario:** la constancia del cambio respecto de la herramienta anterior (WinForms → el stack
  elegido) para las **notas de migración** (ADR-008 §3.1; documento del Bolt 028), si el consumidor la usaba.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: El ciclo completo produce un valor que se resuelve

**Given** un valor de entrada, el nombre de la clave de configuración de destino y la clave activa disponible
**When** la herramienta protege
**Then** el resultado es un valor `aesgcm:` (esquema vigente) con el `kid` de la clave activa, y **ese valor se
resuelve** por el resolutor del motor devolviendo el valor de entrada

### AC-2: La herramienta no contiene protección propia ni material de clave

**Given** el proyecto y el binario de la utilidad
**When** se busca código de cifrado propio, claves, constantes de derivación y composición propia del valor
**Then** hay **0** apariciones (gate de secretos embebidos de ADR-007 §5) y la protección ocurre **íntegramente**
por el motor

### AC-3: El valor en claro no se persiste, no se muestra y no se registra

**Given** una corrida del paso único instrumentada sobre carpetas temporales y de estado de usuario
**When** se buscan el valor de entrada en todo lo escrito (disco, estado de usuario, historial, registros,
temporales)
**Then** hay **0** apariciones: la entrada se captura enmascarada, no se muestra en claro y no queda en el estado
de la aplicación al cerrar el paso

### AC-4: El algoritmo no es una decisión del usuario y las entradas son las mínimas

**Given** la interfaz del paso único
**When** se revisan sus entradas
**Then** el operador aporta **sólo** el valor y la clave de configuración de destino (que es el **AAD**), la clave
activa la resuelve el motor por su provisión (entorno/hook), y **no** existe selector de algoritmo, de esquema ni
de clave

### AC-5: Un fallo del motor es un error accionable

**Given** un fallo del motor (clave activa ausente o mal formada, `kid` sin clave, proveedor del host que falla)
**When** se intenta proteger
**Then** la herramienta muestra un **error accionable** con la **referencia** afectada y **sin** material ni
valor, **nunca** un "valor inválido" silencioso ni un resultado dudoso

### AC-6: Una sola implementación y un solo formato

**Given** la utilidad y el comando homólogo del CLI (Bolt 023)
**When** se comparan sus caminos de protección
**Then** **ambos** consumen la misma operación del motor, la utilidad **no** tiene una ruta alternativa (ni un
modo de formato viejo) y el valor que produce es del **mismo formato** que el del comando (se resuelve con el
mismo resolutor)

### AC-7: El cambio de superficie de usuario queda declarado

**Given** la herramienta anterior (WinForms, con la superficie estática retirada por el Bolt 022)
**When** se revisa la evidencia del cambio
**Then** el cambio de superficie de usuario (stack y forma del paso) está declarado con su reemplazo para las
notas de migración (ADR-008 §3.1)

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — la herramienta produce un valor protegido con el esquema vigente y la clave activa, y ese valor se resuelve | §6/A.2, §6/E | AC-1 · `ProtectValueCycleTests` (ciclo completo con clave de prueba) |
| Bolt §2 — no contiene claves, constantes de derivación ni implementación propia de protección | §6/C.1 | AC-2 · gate de secretos embebidos (ADR-007 §5) + búsqueda de primitivas |
| Bolt §2 — no escribe el valor en claro en disco, en registros ni en su interfaz | §6/B.1 | AC-3 · corrida instrumentada + búsqueda del valor |
| Bolt §2 — funciona sin que el usuario configure el algoritmo, sólo la clave | §6/D.2 | AC-4 · inspección de la interfaz del paso |
| Bolt §2 — un fallo del motor se muestra como error accionable | §6/B.2 | AC-5 · casos de fallo del motor con su mensaje |
| Bolt §2 — no hay ninguna ruta alternativa de protección (una sola implementación compartida con el CLI) | §6/D.1 | AC-6 · inspección estática + superficie del motor (+ código del CLI cuando exista) |
| Bolt §2 — entrada propia en las notas de migración si la superficie de usuario cambia | §6/E | AC-7 · lista de rupturas en el MEM para el Bolt 028 |
| ADR-009 §3.6 — el shell sobrevive como shell delgado sobre el mismo motor, sin clave embebida, con UI mínima | §6/A.1, §6/A.2, §6/D.1 | AC-1, AC-2, AC-6 |
| ADR-007 §3.5 — proteger escribiendo un valor `aesgcm:` con la clave activa; el comando del CLI es aditivo | §6/A.2, §6/D.1 | AC-1, AC-6 |
| ADR-007 §5 — NFR *No embedded secrets* | §6/C.1, §6/E | AC-2 · gate de entropía |
| ADR-001 §3.1/§3.4/§3.5 — la aplicación entre las que sobreviven, sin dependencias nuevas, Windows-only, sin ClickOnce | §6/A.1 | AC-4 y el build del proyecto |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Ciclo completo (la prueba central):** con una **clave de prueba generada en memoria** (y el proveedor del
  entorno apuntando a ella por variable de entorno del test), proteger un valor de entrada y **resolverlo** con el
  resolutor del motor: la salida debe ser igual a la entrada, el valor debe empezar con el esquema vigente y
  llevar el `kid` de la clave activa. Incluye un valor con caracteres no ASCII (que en la 2.0 **no** se pierden:
  el valor se trata como texto) y un valor largo.
- **La delegación (invariante):** la utilidad **no** contiene primitivas criptográficas, ni claves, ni constantes
  de derivación, ni una composición propia del valor: búsqueda sobre el código y sobre el binario + **gate de
  constantes de alta entropía** (ADR-007 §5). Y su ensamblado referencia el motor (y las bibliotecas del release),
  **no** una implementación de criptografía propia.
- **El paso único, sin UI en la prueba:** la lógica del paso (`ProtectValueStep`) se ejercita **sin** la ventana
  (la ventana sólo la invoca): un test la llama con (valor, clave de configuración) y verifica el resultado y los
  diagnósticos. La UI se verifica por inspección de que **no** contiene lógica de protección ni entradas
  adicionales (algoritmo, esquema, clave).
- **Manejo del valor en claro (invariante):** corrida instrumentada que escribe en carpetas temporales y de
  estado de usuario propias del test y luego **busca el valor de entrada** en todo lo escrito (carpetas, estado,
  logs del arnés): **0** apariciones. Además: la entrada es enmascarada (el control de entrada no expone el
  texto) y el estado de la ventana **no** retiene el valor al cerrar el paso.
- **Diagnóstico:** los tres casos de fallo del motor (clave activa ausente, `kid` sin clave disponible, proveedor
  del host que falla) con un doble del proveedor y/o del motor: el mensaje es **accionable** (nombra la
  referencia y qué falta), **no** contiene el valor ni material, y la herramienta **no** produce un resultado
  dudoso.
- **Un solo formato:** el valor producido se resuelve con **el** resolutor del motor del Bolt 022 (misma
  composición y codificación) y **no** existe en la utilidad ningún camino que produzca otro formato ni que lea el
  formato viejo (búsqueda de la forma del formato legacy en el proyecto: 0 apariciones).
- **Consumer-side contract tests (test doubles):** el **proveedor de claves** del motor se ejercita con su
  implementación de entorno (caso real) y con un doble que falla (para AC-5); el motor se usa **real** (es local)
  y se dobla sólo para forzar el fallo de la clave activa.
- **Edge cases:** valor vacío; valor con espacios iniciales/finales (se protege tal cual: no se recorta); clave de
  configuración de destino vacía o mal formada (validation del paso, error accionable); el mismo valor protegido
  dos veces (nonces distintos —propiedad del motor— y ambos resuelven); valor ya protegido en `aesgcm:` como
  entrada (se protege igual: no se detecta ni se rechaza, porque esta herramienta **no** inspecciona el estado de
  un valor —eso es del Bolt 023—); operador que cierra la ventana a mitad del paso (no queda estado ni parcial);
  proveedor del host registrado y no registrado.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida del ciclo completo (con su valor resuelto), el gate de
  secretos embebidos, la inspección de delegación y de paso único, la búsqueda del valor en claro en todo lo
  escrito (0 apariciones), los tres diagnósticos de fallo y la constancia del formato único y del cambio de
  superficie de usuario. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `refactor` y produce un valor **en proceso**: el
  ciclo valor → protegido → resuelto se verifica con el motor local y una clave de prueba. No hay base de datos,
  cola, servicio, almacén ni entorno desplegado que aporte una verificación adicional (la utilidad no toca la
  configuración de nadie: **produce un texto que el operador pega**).
- **Contract / E2E against the real providers:** `n/a` — el único "proveedor" del caso real es el **entorno del
  operador** (variables de entorno) o su **hook** de claves, y ambos tienen la misma forma en el nivel hermético.
  Un almacén de claves corporativo (Key Vault/KMS/DPAPI) es del **host** y su verificación es suya.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales.
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); el proyecto `net10.0-windows` compila sin `System.Windows.Forms` ni ClickOnce | pass |
| Unit / component tests | V-Bounce | 100 % verdes: ciclo completo, delegación, paso único sin UI, manejo del valor en claro, diagnósticos y formato único | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El proveedor de claves del motor se ejercita con su implementación de entorno (caso real) y con un doble que falla (AC-5) | pass |
| SAST | V-Bounce | 0 hallazgos altos; 0 primitivas criptográficas, 0 claves y 0 composición propia del valor en la utilidad | pass |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas (el motor y las bibliotecas del release ya están autorizadas); licencias permisivas (ADR-001 §3.4) | pass |
| OWASP Top 10 — static review | V-Bounce | Manejo de secretos: entrada enmascarada, valor en claro **nunca** persistido ni registrado, sin rutas alternativas de protección, sin algoritmo elegible por el usuario y con el diagnóstico sin material; el único canal de salida del valor en claro es la memoria del paso | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción: el valor de entrada es un dato que se protege, no una instrucción | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** claves, constantes de derivación o material en el código, en la configuración versionada y en los fixtures; 0 supresiones de advertencia que tapen un uso prohibido (NFR *No embedded secrets*) | pass |
| Hallucination lint | V-Bounce | Cada pieza trazable a ADR-009 §3.6 / ADR-007 §3.5 / ADR-001 §3.1; 0 capacidades de otro Bolt (migración/rotación → 023; motor → 022) adelantadas | pass |
| IP / license provenance | V-Bounce | Código propio; 0 código de terceros; la utilidad **no** copia la implementación del cifrado anterior | pass |
| PII / DLP | V-Bounce | `confidential` (mirror del Bolt): la utilidad **maneja credenciales en claro** — el control es que **no** se persisten, no se muestran ni se registran (AC-3); las pruebas usan valores **sintéticos** y claves generadas | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes; la aplicación es de escritorio y no resuelve plugins | n/a (razón) |
| Test-first evidence | V-Bounce | El ciclo completo y la verificación de **no persistencia** del valor en claro se escriben **antes** de construir la ventana: el control de datos no es una revisión posterior, es el criterio de la utilidad | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas del ciclo producen valores que **ambos** resuelven al mismo texto (el payload protegido es no determinista por el nonce: se verifica la propiedad, no la igualdad de bytes) | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: el proveedor del caso real (entorno/hook) tiene la misma forma en el nivel hermético | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: aplicación de escritorio local, sin superficie de red ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: operación de un solo paso; su costo es el de la primitiva de la plataforma | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: la utilidad no agrega telemetría (§11) y por política **no** debe registrar valores | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `confidential`** (espeja el Bolt). La utilidad **existe para manejar
  una credencial**: el valor de entrada es material sensible y, por un instante, está **en claro en el proceso**.
- **El control central es el no-persistir:** el valor en claro **no** se escribe en disco, ni en el estado de
  usuario, ni en un historial, ni en registros, ni en archivos temporales; la entrada se captura **enmascarada**;
  al cerrar el paso, la ventana no lo retiene. La verificación es explícita (búsqueda del valor en todo lo
  escrito).
- **Sin material de clave en el producto:** la utilidad **no** guarda claves ni constantes de derivación (es el
  defecto histórico de la familia: `_CryptoKey` e `_Iv` en el binario, F-23); la clave activa proviene de la
  **provisión externa** del motor (variables de entorno por defecto, hook del host) y el framework consume la
  **referencia** (`kid`), no el material (ADR-007 §3.2). El gate de constantes de alta entropía de ADR-007 §5
  mide esta herramienta en particular.
- **Sin protección propia:** la utilidad **no** implementa algoritmo, **no** elige esquema y **no** produce un
  segundo formato; la superficie estática con clave implícita que usaba antes (`Encrypt(string)`/`Decrypt(string)`)
  la retiró el Bolt 022 y acá **no** se reintroduce.
- **El AAD ata el valor a su destino:** la utilidad **exige** el nombre completo de la clave de configuración de
  destino, de modo que el valor producido **no** se puede mover a otro slot sin re-protegerlo (ADR-007 §3.1) — y
  re-targetear es operación del Bolt 023, no un copiar/pegar.
- **Diagnóstico sin material:** los fallos nombran la **referencia** afectada (la clave activa o el `kid`) y qué
  falta, **nunca** el valor ni la clave.
- **Superficie mínima y local:** la aplicación es de escritorio, **no** expone red, **no** lee archivos de
  configuración de nadie y **no** toca el Registro; el único canal del valor protegido es la pantalla/copia.
- **Sin secretos en la evidencia:** las pruebas usan valores sintéticos y claves generadas en memoria (o por
  variables de entorno del entorno de prueba): ninguna credencial real entra al repositorio ni a la evidencia.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: la entrega es una **aplicación de escritorio** de un solo paso; **no**
  emite métricas ni trazas a ningún colector y **no** se le agrega instrumentación. Y hay una razón de política:
  **no debe** registrar el valor ni material (ADR-007 §3.2).
- **La observabilidad de este Bolt es su evidencia:** el ciclo completo con su valor resuelto, el gate de
  secretos embebidos, la inspección de delegación y de paso único, la búsqueda del valor en claro en todo lo
  escrito (0 apariciones), los tres diagnósticos de fallo y la constancia del formato único y del cambio de
  superficie de usuario.
- **Diagnóstico en pantalla (parte del entregable):** el operador ve **qué falta** cuando el motor no puede
  proteger (clave ausente, `kid` sin clave, proveedor que falla) y ve el **`kid` usado** y el esquema de salida
  del valor producido — que es la única información útil para verificar que protegió con la clave que creía.
- La telemetría del consumo (cuántos valores protegió cada cliente) no existe: es una utilidad local, y el
  reporte de una migración masiva es del Bolt 023.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de valores en este Bolt (es del Bolt 023). Lo que hay es la **supervivencia**
  de la utilidad con una **superficie de usuario nueva** (stack y forma del paso) y la constancia de que su
  comportamiento anterior —proteger con la clave del framework— se reemplaza por **proteger con el motor y la
  clave activa del operador**. Ese cambio entra en las **notas de migración** (documento del **Bolt 028**,
  ADR-008 §3.1) con sus tres datos obligatorios: qué cambia (la superficie de la utilidad y el origen de la
  clave), por qué (ADR-009 §3.6 + ADR-007 §3.5: no hay dos implementaciones ni dos formatos, y no hay clave
  embebida) y con qué se reemplaza (el mismo paso, con la clave proveniente del entorno o del hook del host).
- **Compatibility:** el valor que produce la utilidad es del **mismo formato** que el del comando del CLI
  (`aesgcm:` con la composición del motor), así que un valor protegido desde el escritorio se resuelve por el
  framework igual que uno producido por el CLI; y los valores que la herramienta anterior hubiera producido con el
  formato viejo **no** se resuelven en la 2.0: se migran con el Bolt 023 (su capacidad legacy aislada).
- **Nada de datos en juego:** la utilidad no escribe configuración de nadie, no migra y no rota —produce un texto
  que el operador decide dónde usar—, así que no hay estado que reconciliar.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido ni valor reescrito por esta utilidad; si
  un operador hubiera protegido un valor y volviera atrás el framework, ese valor se re-protege con el motor
  vigente (o se recupera el original, que el operador conserva hasta reemplazarlo). El camino de retorno del
  consumidor sigue siendo el 1.4 congelado (ADR-008 §3.1).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Reintroducir una clave o un algoritmo "por conveniencia"** (el defecto histórico de esta familia) | 3 | 5 | Gate de **secretos embebidos** (ADR-007 §5) sobre esta herramienta + prohibición explícita en el alcance + la utilidad **no** contiene primitivas ni constantes: la protección es del motor |
| **Un valor protegido que no se resuelve** (por una clave distinta de la activa, o por un AAD equivocado) | 3 | 4 | El **ciclo completo** es la prueba central (proteger y **resolver** comparando con la entrada) + el `kid` usado y el esquema de salida visibles + el AAD exigido como entrada (no adivinado) |
| **Fuga del valor en claro** (a un log, al estado de usuario, a un temporal o a la pantalla) | 3 | 5 | Entrada enmascarada + **no persistencia** verificada por búsqueda del valor en todo lo escrito + gate de secretos; la entrada no se muestra en claro ni se retiene al cerrar el paso |
| **Divergencia entre la herramienta y el comando del CLI** (dos implementaciones o dos formatos) | 2 | 4 | Misma operación del motor, verificada en dos patas (estática y compartida, §5) y la constancia de que la utilidad no tiene protección propia; el formato se comprueba resolviendo el valor con el resolutor |
| **Un fallo del motor mostrado como "valor inválido"** (el operador no sabe qué hacer) | 3 | 3 | Los tres casos de fallo con **mensaje accionable** y la referencia afectada (AC-5); la utilidad **no** produce un resultado cuando el motor falla |
| **Selector de algoritmo o de esquema** que reintroduce una decisión ya tomada | 2 | 3 | La interfaz no lo ofrece (AC-4): las entradas son el valor y la clave de configuración de destino |
| **La utilidad se convierte en un lector del formato viejo** (alguien pide "abrir" un valor de 1.4 desde el escritorio) | 2 | 4 | §4 lo excluye: la capacidad legacy es del **Bolt 023**, aislada bajo su excepción; acá se verifica que no haya forma del formato viejo ni primitivas legacy en el proyecto |
| **Cambio de superficie de usuario no declarado** (el consumidor usaba la herramienta WinForms y no sabe qué cambió) | 3 | 2 | AC-7: el cambio se declara con su reemplazo para las notas (ADR-008 §3.1) |
| **Dependencia nueva** (un toolkit de UI, un lector de configuración) | 2 | 3 | ADR-001 §3.4 y el gate de dependencias: WPF es in-box y la utilidad no lee configuración de nadie |
| **El valor de prueba termina en el repositorio** (un fixture "de ejemplo" con una clave o un valor real) | 2 | 4 | La suite usa valores **sintéticos** y claves **generadas**; el gate de secretos cubre el repositorio y el MEM lo registra |
| **La utilidad queda sin poder proteger porque el operador no configuró la clave** | 4 | 2 | Es el comportamiento correcto y está documentado: **error accionable** que indica la variable de entorno o el hook; la disponibilidad de claves es una propiedad operativa del cliente (deuda declarada por ADR-007 §4) |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-007/ADR-009/ADR-001; acá se
> aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) El stack de UI de la utilidad es WPF sobre `net10.0-windows`** | ADR-009 §3.6 deja **el detalle de pantallas a la SPEC** y fija el TFM (`net10.0-windows`) por ADR-001 §3.1; y ADR-009 §3.5 **retira `System.Windows.Forms` del tooling** — que es exactamente el stack que usaba la herramienta anterior. WPF es **in-box** (sin dependencias nuevas), coherente con el shell del Modeler (Bolt 025) y con la limpieza del layout. **Alternativa:** un retarget WinForms de la utilidad (menos trabajo) pero contradice ADR-009 §3.5 y reintroduce el stack retirado; si el aprobador lo prefiere, es una revisión de este SPEC |
| **(⚠ aprobador) La utilidad exige el nombre completo de la clave de configuración de destino (el AAD) como entrada** | Sin él, el valor protegido no se puede atar a su slot (ADR-007 §3.1: el AAD **es** el nombre de la clave de configuración) y el resultado sería un valor que funciona en cualquier slot —precisamente lo que el AAD evita—. La alternativa (una utilidad que derive el AAD de algo) exigiría que la herramienta conozca la configuración de alguien, que es del Bolt 011 |
| El **paso único** se implementa como una pieza testeable sin UI (`ProtectValueStep`) y la ventana sólo la invoca | Permite probar el ciclo y los diagnósticos **herméticamente**, sin UI, y deja la ventana con cero lógica de protección (que es también lo que hace verificable "no hay protección propia") |
| La utilidad **no** muestra el valor original ni ofrece "verlo" | El valor en claro ya está en la memoria del proceso por un instante; mostrarlo otra vez y permitir copiarlo multiplica la exposición sin aportar nada a la tarea (el operador ya lo tiene) |
| La utilidad **no** detecta ni inspecciona el estado de un valor de entrada (un `aesgcm:` se protege igual) | Inspeccionar/clasificar valores es del Bolt 023 (`inspect`); que la utilidad decida "esto ya está protegido" sería un segundo criterio de clasificación y una razón para no hacer lo que el operador pidió |
| La utilidad **no** lee ni escribe archivos de configuración | Su contrato es "valor → valor protegido" (ADR-009 §3.6); leer/escribir la configuración de un consumidor es de los Bolts 011/021 y de la herramienta de migración (023), y agregarlo acá metería política de archivos en una utilidad de un paso |
| La verificación de "una sola implementación" tiene **dos patas** y la pata compartida puede quedar pendiente si el CLI (023) no se ejecutó | El shell es independiente del comando del CLI: bloquearlo en el 023 haría que un Bolt no pudiera cerrar por otro. La pata estática (esta utilidad no tiene protección propia) es concluyente sobre el riesgo principal —la clave o el algoritmo "por conveniencia"—; la pata compartida se verifica contra la superficie del motor y, cuando el comando exista, sobre su código |
| El diagnóstico traduce los errores del motor a un mensaje de la utilidad | El operador de una utilidad de escritorio no ve un `kid` ni una excepción tipada: ve una frase accionable ("definí `COOPERATOR_KEY_<kid>` o registrá el hook", con la referencia). Se traduce **sin** perder la causa ni exponer material |
| No se define un modo "batch" en la utilidad | El uso masivo (migrar/rotar un corpus) es del **CLI** (Bolt 023, para agentes y pipelines); duplicarlo en el escritorio crearía dos caminos de la misma operación (ADR-009 §3.6: el comando es aditivo, no sustituto) |

---

## 15. Stop conditions

- **El motor de protección (Bolt 022) no está en el árbol 2.0**: parar y reportar. Sin él, la utilidad tendría que
  implementar protección —exactamente lo que el Bolt prohíbe—.
- **Se pide que la utilidad implemente protección, guarde una clave o derive material**: parar. ADR-007 §3.2/§3.4
  y ADR-009 §3.6 lo prohíben, y una clave en el producto exigiría un ADR que reemplace a ADR-007.
- **Se pide un selector de algoritmo/esquema** o un modo de "compatibilidad" que lea o escriba el **formato
  viejo**: parar. La política fija el mecanismo y la capacidad legacy es del **Bolt 023**, aislada y exceptuada;
  en una utilidad de escritorio **no** va.
- **Se pide que la utilidad lea o escriba archivos de configuración** (o el Registro) para "hacer todo el
  trabajo": parar y reportar; eso es la configuración del consumidor (Bolts 011/021) y la operación masiva del
  CLI (023), no el paso único de esta utilidad.
- **Se pide persistir el valor en claro** (historial, "recientes", estado de usuario) o mostrarlo en claro: parar.
  El control de datos de esta utilidad es no persistirlo (AC-3) y su clasificación `confidential` lo respalda.
- **El valor en claro aparece escrito en algún lado** (disco, estado, log, temporal): parar; es una fuga de una
  categoría `confidential` y se corrige antes de cerrar el V-Bounce.
- **Se pretende que la utilidad entregue la migración o la rotación**: parar; son del Bolt 023 (y su capacidad
  legacy está aislada ahí, no acá).
- **Se pretende resolver acá el canal de distribución o el empaquetado/asociación**: parar; son de ADR-001 §3.6 y
  de los Bolts 027.
- **El stack de UI decidido (WPF) no es viable** en el entorno objetivo por una restricción no prevista: parar y
  reportar antes de cambiar de stack (la alternativa WinForms contradice ADR-009 §3.5 y es una decisión, no una
  conveniencia).
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado por
  `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-009 §3.5/§3.6, ADR-007 §3.1/§3.2/§3.4/§3.5 y §5, ADR-001 §3.1/§3.4/§3.5)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-024`](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-40 (el encriptador de connection strings y su uso de la superficie con clave implícita) y §6 ítem 12.
- [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — **§3.6** (el encriptor sobrevive como shell delgado sobre el mismo motor, UI mínima, detalle de pantallas a la SPEC, comando del CLI **aditivo**), §3.5 (limpieza del tooling: sin `System.Windows.Forms`, sin ClickOnce), §3.8 (interplay con ADR-007).
- [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (esquema vigente, composición y AAD; retiro de la superficie estática con clave implícita), §3.2 (claves: activa y de lectura; provisión externa; nada de secretos en logs ni reportes), §3.4 (primitivas permitidas y prohibidas), **§3.5** (la herramienta protege escribiendo un valor `aesgcm:` con la clave activa, sin clave embebida; el comando del CLI es aditivo, no sustituto), §3.6 (qué no cubre), §4 (trade-offs y deuda) y §5 (NFR *No embedded secrets*).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (la herramienta entre las aplicaciones `net10.0-windows` que sobreviven), §3.4 (política de dependencias), §3.5 (supervivencia Windows-only explícita; sin ClickOnce), §3.6 (canal de distribución pendiente con dueño y fecha).
- [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) — el motor de protección, la clave activa, el `kid`, el AAD y la provisión de claves (prerrequisito, Bolt 022).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y gates de invariantes (prerrequisito, Bolt 001).
- [`SPEC-261016-0039`](./SPEC-261016-0039-comandos-de-valores-protegidos.md) · [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) · [`US-000.BOLT-027`](../functional/bolts/US-000.BOLT-027-empaquetado-y-asociacion.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el comando homólogo del CLI (cruce de la implementación única), el shell del Modeler, el empaquetado del tooling y el documento de notas (fronteras declaradas).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-024`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-7, decisión del **stack de UI** (WPF `net10.0-windows`, coherente con ADR-009 §3.5/§3.6), el **paso único** con el **AAD** como entrada, la política de **no persistencia del valor en claro**, el retiro de la protección propia, la verificación de la **implementación única** (dos patas) y la estrategia de pruebas (ciclo completo, delegación, valor en claro, diagnósticos). Declara las fronteras con los Bolts 022 (motor), 023 (comandos) y 025/027 (shell y empaquetado). `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (**seguridad**) + el dueño del tooling de escritorio] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
