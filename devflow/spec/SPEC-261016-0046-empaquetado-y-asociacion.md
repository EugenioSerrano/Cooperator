---
id: "SPEC-261016-0046"
title: "Empaquetado y distribución del shell: artefacto autocontenido, asociación del `.coop2` por instalador y retiro de la publicación anterior"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-027" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-009", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, lane de integración y gates de invariantes (Bolt 001)
  - "SPEC-261016-0044-shell-wpf.md" # el shell del Modeler sobre el que se empaqueta y se asocia el formato (Bolt 025)
  - "SPEC-261016-0023-motor-headless.md" # el motor que viaja dentro del mismo payload (Bolt 017)
  - "SPEC-261016-0025-cli-del-motor.md" # el CLI que viaja dentro del mismo payload, para el flujo agente (Bolt 018)
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "applicable" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests:            # applicable — the integration tests a run executes: escritos por este V-Bounce (ver §8.2)
    - "2.0/tests/Cooperator.Tooling.Packaging.Integration.Tests/PublishArtifactTests.cs (Category=Integration) — el artefacto publicable se produce en el entorno, su registro de composición coincide con lo que lleva adentro (shell + motor + CLI) y arranca en una máquina sin runtime previo"
    - "2.0/tests/Cooperator.Tooling.Packaging.Integration.Tests/InstallerAssociationTests.cs (Category=Integration) — la instalación deja la asociación del formato funcionando (aportada de forma declarativa) y el uso normal de la aplicación no escribe el Registro"
    - "2.0/tests/Cooperator.Tooling.Packaging.Integration.Tests/NoAssociationOpenTests.cs (Category=Integration) — sin asociación presente, el formato se abre por el diálogo de archivos y por argumento de línea de comandos"
    - "2.0/tests/Cooperator.Tooling.Packaging.Integration.Tests/SystemIntegrationTests.cs (Category=Integration) — la apertura de la solución/carpeta generada con su declaración explícita y su degradación al mensaje previsto cuando no hay asociación del sistema operativo; el archivo de diagnóstico por usuario con su política de tamaño y rotación; y la ausencia de escritura en el registro de eventos del sistema"
    - "2.0/tests/Cooperator.Tooling.Packaging.Integration.Tests/RetiredSurfaceTests.cs (Category=Integration) — el artefacto y todo el tooling sin la metadata de publicación anterior ni su referencia, y sin las APIs retiradas en las fuentes y los binarios publicados"
  environment: "un runner de CI Windows capaz de ejecutar la instalación y la corrida en un entorno limpio (sin runtime previo del proyecto) y, para la vía declarativa, con la plataforma de instalación disponible; si el SPEC del arnés (Bolt 001) fija un nombre de entorno, se usa ese"
  data: "el artefacto publicado por la propia corrida, el modelo de referencia del arnés para el humo de generación, una asociación ausente y una presente (para los dos escenarios de apertura) y un perfil de usuario limpio para el archivo de diagnóstico; sin credenciales ni datos personales"
  reason: ""        # n/a — no aplica: el plan es `applicable`
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:52:21-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:52:21-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:52:21-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-027 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «Publicacion excluida y condicionada al canal (ADR-001 3.6); no bloquea READY»), ADR-009/ADR-001/ADR-008 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class medium → autonomy_level L3; data_classification internal; test_integration required → applicability applicable. Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: artefacto publicable autocontenido win-x64 (carpeta/zip primario; MSIX opcional para instalación+asociación; sin ClickOnce) con registro de composición (shell + motor + CLI); retiro de la forma anterior y APIs muertas (en fuentes Y binarios publicados); integración con el SO declarada + diagnóstico a archivo por usuario con política declarada; funcionamiento SIN asociación obligatoria (diálogo/argumento); publicación EXCLUIDA y condicionada al canal (ADR-001 §3.6, dueño tech_lead con fecha anterior al release)."
    - "CONFIRMACIÓN (exclusión de la publicación) — CORRECTA: §4/§13/§15 declaran el feed, la firma y la publicación de los .nupkg como exclusión explícita y riesgo con dueño (tech_lead) y fecha (antes del release 2.0.0), ADR-001 §3.6; el Bolt no firma, no publica y no elige canal. La decisión del canal que cambiara la forma del artefacto se resuelve por enmienda del Bolt + revisión de SPEC, nunca silenciosamente (AC-9)."
    - "ADJUDICACIÓN (a) forma primaria carpeta/zip + MSIX opción — CONFIRMADA: §14 fila 1 fija la carpeta/zip autocontenida como canal primario y MSIX como opción (ADR-009 §3.6 dice exactamente eso); exigir MSIX obligatorio cambiaría el environment de la corrida (depender de la plataforma de instalación). La asociación se demuestra con MSIX cuando está disponible y, si no, se documenta como paso de instalación (vía carpeta/zip), que es lo que el propio ADR prevé. §13 registra el riesgo de que MSIX no esté disponible en el entorno sin bloquear la vía primaria."
    - "ADJUDICACIÓN (b) política del diagnóstico fijada en el SPEC — CONFIRMADA: §14 fila 2 y §C.3 fijan la política explícita (ubicación en los datos del usuario, tamaño máximo por archivo, número de archivos retenidos y comportamiento ante fallo de escritura —no rompe, informa una vez—) en LogPolicy.md; ADR-009 §3.5 exige «archivo por usuario con política de tamaño y rotación» pero no fija números, y el SPEC los declara para que sean verificables (AC-8)."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-027`. El editor, el estado de
     usuario y el stack del shell son del Bolt 025; el flujo de plan/reporte, del Bolt 026; la **publicación**
     (feed y firma) está **excluida** por ADR-001 §3.6.
  ⚠️ EXCLUSIÓN CONDICIONADA (el riesgo principal): el **canal de distribución** —feed, firma de paquetes y
     publicación de los `.nupkg`— es una decisión pendiente con dueño (`tech_lead`) y fecha anterior al release
     2.0.0 (ADR-001 §3.6). Este Bolt **no** firma, **no** publica y **no** elige canal: entrega el artefacto,
     la instalación, la asociación y el retiro de la forma anterior, y declara la exclusión como exclusión y
     como riesgo (§4/§13/§15).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `applicable`, porque el Bolt declaró `test_integration: required` — el
     artefacto y su asociación se demuestran **sobre una instalación real**: la corrida se ejecuta en el ciclo
     de test de integración del Bolt, después de `AITL-MEM-Approval`, y su resultado queda como hecho en
     `integration_test_runs[]` del manifiesto.
-->

# SPEC-261016-0046 — Empaquetado y distribución del shell: artefacto autocontenido, asociación del `.coop2` por instalador y retiro de la publicación anterior

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 15) |
| **Bolt** | [US-000.BOLT-027](../functional/bolts/US-000.BOLT-027-empaquetado-y-asociacion.md) |
| **ADRs** | [ADR-009](../adrs/ADR-009-shell-del-modeler.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega la **forma distribuible** del shell: un **artefacto autocontenido publicable** (`dotnet publish`
self-contained `win-x64` como canal primario) que **arranca en una máquina sin runtime previo** y que **viaja con
el motor y con el CLI** dentro del mismo payload; la **instalación** que deja la **asociación declarativa** del
formato `.coop2` —aportada por el instalador, **nunca** escrita por la aplicación— con el ícono y el
identificador de programa como **recursos del paquete**; el **retiro** de la forma de publicación anterior y de
las APIs de las que dependía; la **integración con el sistema operativo** con la **declaración explícita** que el
runtime moderno exige (y su **degradación con mensaje** cuando no hay asociación del sistema operativo); y el
**diagnóstico a archivo por usuario** con política de tamaño y rotación — nunca el registro de eventos del
sistema—. El producto **funciona aunque no exista la asociación**: el formato se abre por el **diálogo de
archivos** y por **argumento** de línea de comandos.

El problema que resuelve está documentado (REV-001 F-35/F-36 y §6 ítem 15): la herramienta anterior **escribía la
asociación de la extensión en el Registro en runtime** (`FileExtensionAssociation`/HKCR), resolvía el ejecutable
por una **API de la interfaz retirada** (`Application.ExecutablePath`) y el ícono por una ruta que aportaba la
librería, y los proyectos del tooling arrastraban la **metadata de publicación anterior** (ClickOnce/bootstrapper)
con su referencia muerta.

Si no se implementa: el producto no tiene una forma de entrega instalable y verificable, la asociación del
formato depende de que la aplicación escriba el Registro, y el tooling sigue atado a la plataforma de publicación
que el ciclo retira.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 15 ruteó F-35 (la asociación de la extensión escrita por la
aplicación) y F-36 (la metadata de publicación anterior en los proyectos de ambos tools y su referencia muerta) a
un Bolt de empaquetado, y ADR-009 §3.4–§3.6 decidió la **forma de distribución** y **dónde vive la asociación**.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-009 §3.4 (asociación y rutas)** — la asociación del `.coop2` la aporta el **instalador** y **no** la
  aplicación; el shell **funciona sin asociación**; las **rutas** se resuelven por la **BCL** —`Environment.ProcessPath`
  para el ejecutable y `AppContext.BaseDirectory` para la carpeta de la aplicación— y se retiran
  `Application.ExecutablePath` y cualquier uso de `Assembly.Location` (que devuelve vacío en escenarios
  *single-file*); el ícono y el identificador de programa son **recursos del paquete**.
- **ADR-009 §3.5 (integración con el sistema)** — se **elimina** lo que era Windows-only innecesario o dependía de
  APIs muertas (`Application.EnableVisualStyles`, `Application.DoEvents`, `System.Deployment`/ClickOnce,
  `System.Windows.Forms` en el tooling y las referencias muertas que ADR-001 §3.3 ordena limpiar); la apertura de
  la solución o carpeta generada y el enlace informativo siguen funcionando **con la declaración explícita** que
  el runtime moderno exige; y el **diagnóstico** del shell va a un **archivo por usuario**, nunca al registro de
  eventos del sistema.
- **ADR-009 §3.6 (empaquetado y distribución)** — **`dotnet publish` self-contained `win-x64`** como canal
  primario (ADR-001 §3.5), en la carpeta/zip que define la distribución del tooling, con **MSIX como opción**
  cuando se necesiten instalación y asociación declarativa de archivos; **sin ClickOnce** y sin instalador propio;
  el shell **viaja con el motor** (referencia los paquetes del motor y las librerías) y el **CLI** puede
  distribuirse junto al shell (mismo payload) para que el flujo agente esté disponible en la misma instalación; y
  el folder feed con los `.nupkg` para las apps generadas lo define ADR-001 §3.1.
- **ADR-001 §3.5 (supervivencia Windows-only explícita)** — el shell del Modeler, el encriptador de connection
  strings y el migrador `net48` son las únicas piezas Windows-only esperadas; **sin ClickOnce**; `dotnet publish`
  self-contained para `win-x64`; MSIX queda como opción si más adelante hace falta instalación y asociación
  —lo que resuelve **el instalador o el runtime con verificación de SO**, nunca una escritura de la aplicación—.
- **ADR-001 §3.6 (canal)** — el **canal de distribución** (feed privado vs. público, firma de paquetes y
  publicación de los `.nupkg`) es una decisión **pendiente con dueño y fecha** (el `tech_lead`, antes del release
  2.0.0). Esa decisión **no** la toma este Bolt: la parte de **publicación** queda **excluida** y **condicionada**.
- **ADR-008 §3.1** — toda ruptura de superficie se registra con sus tres datos obligatorios; si este Bolt retira
  superficie del consumidor, entrega su entrada.

**Qué decide este SPEC, dentro de esas restricciones.** La **forma concreta del artefacto** (publicación
autocontenida por RID, en carpeta y/o *single-file*, con el registro de su composición) y su **capacidad de
arrancar sin runtime previo**; el **reparto entre instalador y aplicación** de la asociación y las rutas (con la
**verificación** de que la aplicación no escribe el Registro); la **forma del diagnóstico** por usuario (ubicación,
tamaño y rotación); la **declaración explícita** de la apertura de lo generado y su **mensaje de degradación**; y
el **inventario de lo que se retira** de la forma de publicación anterior y de las APIs muertas.

**Fronteras declaradas.** El **editor**, el **estado de usuario** y el **stack** del shell son del **Bolt 025**;
el **flujo de plan y reporte** del shell es del **Bolt 026**; el **motor** y el **CLI** que viajan en el payload
son de los Bolts 017/018 (y su inclusión acá es **composición del artefacto**, no modificación de su código); el
**feed** con los `.nupkg` de las apps generadas es de ADR-001 §3.1; y la **publicación y la firma** del artefacto
—el **canal**— son de ADR-001 §3.6, **excluidas** de este Bolt.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt (`required` → `applicable`).

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-027`](../functional/bolts/US-000.BOLT-027-empaquetado-y-asociacion.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: required`; el `acknowledgment_reason` confirma la exclusión de la publicación y su condición al canal | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:12:35-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — **F-35** (la asociación de la extensión escrita en el Registro por la aplicación, con el ejecutable resuelto por una API de la interfaz retirada y el ícono por una ruta de la librería) y **F-36** (la metadata de publicación anterior en los proyectos de ambos tools y su referencia muerta), más **§6 ítem 15** | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — §3.4 (la asociación es del **instalador**; el shell funciona **sin** asociación; rutas por la **BCL**; ícono e identificador de programa como recursos del paquete), §3.5 (integración con el sistema: la apertura de lo generado y el enlace informativo con **declaración explícita**, el retiro de las APIs de la interfaz retirada y de `System.Deployment`/ClickOnce, y el diagnóstico a **archivo por usuario**), §3.6 (publicación autónoma como canal primario, instalación declarativa como opción, **sin ClickOnce**, y la **firma/el canal fuera de este ADR**), §3.8 (interplay) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (layout, distribución del tooling y el **folder feed** junto al release), §3.3 (naming/versionado y la limpieza obligatoria de proyecto: metadata ClickOnce y `System.Deployment` fuera), §3.4 (política de dependencias), §3.5 (supervivencia Windows-only explícita, `dotnet publish` self-contained, MSIX como opción, **sin ClickOnce**), **§3.6** (el **canal de distribución** pendiente con dueño y fecha antes del release 2.0.0) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (toda ruptura se documenta con sus tres datos y su reemplazo) y §5 (el gate de compatibilidad de API contra la línea base 1.4) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) — su SPEC canónica ([`SPEC-261016-0044`](./SPEC-261016-0044-shell-wpf.md)) entrega el **shell** sobre el que se empaqueta y se asocia el formato | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolts (composición del payload, no fuente gobernada) | [`US-000.BOLT-017`](../functional/bolts/US-000.BOLT-017-motor-headless.md) ([`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md)) y [`US-000.BOLT-018`](../functional/bolts/US-000.BOLT-018-cli-del-motor.md) ([`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md)) — el motor y el CLI que viajan dentro del mismo artefacto | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, build compartido y lane de integración | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Tools/CooperatorModeler/FileExtensionAssociation.cs` (la asociación escrita por la **aplicación**), su
invocación desde `MainForm.cs` (el botón "File Association" y su mensaje de éxito) y `CooperatorModeler.csproj`
(la metadata de publicación anterior y `System.Deployment`); y —para el contraste de lo que se retira— el uso de
`Application.ExecutablePath` y de `Assembly.Location` en el tooling 1.4. La referencia de la **forma de
publicación** que el ciclo retira está en `ADR-001 §3.5` y `ADR-009 §3.5`.

**Coherencia del plan de integración:** el Bolt declara `test_integration: required` (§3.2.2) y este SPEC declara
`integration_test_plan.applicability: "applicable"` **con sus tests, su entorno y sus datos** (§8.2 y frontmatter).
**No hay desajuste** y por lo tanto no hay reporte bloqueante.

---

## 4. Scope

### In scope

- El **artefacto publicable** del shell: `dotnet publish` **self-contained `win-x64`** como canal primario, en la
  carpeta/zip del tooling, con el **registro de su composición** (el shell, el **motor** y el **CLI** dentro del
  mismo payload) y con la capacidad de **arrancar en una máquina sin runtime previo**.
- La **instalación** y la **asociación declarativa** del `.coop2` por el **instalador** (con el ícono y el
  identificador de programa como **recursos del paquete**), y la constancia de que la **aplicación no escribe el
  Registro** —ni para la asociación ni para su estado—.
- El **funcionamiento sin asociación**: la apertura del formato por el **diálogo de archivos** y por
  **argumento** de línea de comandos.
- El **retiro** de la metadata de publicación anterior y de su referencia en **todo** el tooling, y de las APIs
  que ataban la aplicación a la forma vieja (la inicialización de la interfaz retirada y la resolución de rutas
  por la ubicación de la librería), con las **rutas por la BCL**.
- La **integración con el sistema operativo**: la apertura de la solución o carpeta generada y el enlace
  informativo con su **declaración explícita**, y su **degradación con el mensaje previsto** cuando no hay
  asociación del sistema operativo.
- El **diagnóstico a archivo por usuario** con su **política de tamaño y rotación**, y la constancia de que **ni
  la instalación ni el uso** escriben el registro de eventos del sistema ni el Registro del sistema.
- La **ruptura de superficie** que este Bolt introduzca, registrada con sus tres datos (ADR-008 §3.1) para las
  notas de migración (Bolt 028).

### Out of scope

- La **publicación**: el **feed** (privado o público), la **firma de paquetes** y la **publicación de los
  `.nupkg`** dependen de la decisión del **canal de distribución** (dueño `tech_lead`, fecha anterior al release
  2.0.0, ADR-001 §3.6): **exclusión explícita y condicionada** (§13/§15). Este Bolt **no** firma, **no** publica y
  **no** elige canal.
- El **editor**, el **estado de usuario** y el **stack** del shell: **Bolt 025** (ADR-009 §3.1/§3.2).
- El **flujo de plan y reporte** del shell y su puente al comando del CLI: **Bolt 026** (ADR-009 §3.1).
- El **motor** y el **CLI** en sí (su código, su distribución como herramienta y su `dotnet tool`): **Bolts
  017/018**; acá se los **incluye en el payload**, no se los modifica.
- El **folder feed con los `.nupkg`** de las aplicaciones generadas: ADR-001 §3.1 (y el set de plantillas, Bolt 021).
- La **enumeración de servidores** y las demás incógnitas de descubrimiento: ADR-009 §3.7 (fuera, con su DISC).
- La **suerte del encriptador de connection strings** y de las demás aplicaciones: **Bolts 024 y su empaquetado**
  (ADR-009 §3.6/§3.8).
- Un **instalador propio** más allá de la opción **declarativa** que el ADR habilita: no se construye MSI/EXE de
  instalación propio (ADR-009 §3.6).
- **Dependencias nuevas** en el paquete del shell: ADR-001 §3.4.

---

## 5. Prerequisites and baseline

- **Bolt 025 y su SPEC canónica
  ([`SPEC-261016-0044`](./SPEC-261016-0044-shell-wpf.md), `prerequisites`):** el **shell** sobre el que se empaqueta
  y se asocia el formato (su proyecto, su stack y su estado de usuario). Sin él no hay artefacto que publicar ni
  aplicación que asociar. Si no está en el árbol 2.0, **parar y reportar**.
- **El motor (Bolt 017, [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md)) y el CLI (Bolt 018,
  [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md)):** son la **composición** del payload —el artefacto
  viaja con ellos para que el flujo agente esté disponible en la misma instalación (ADR-009 §3.6)—. Se incluyen;
  **no** se modifican.
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés, el build compartido
  y la **lane de integración** — este Bolt declara `required` y su evidencia es la corrida sobre una instalación
  real.
- **Plataforma y entorno de verificación:** un **runner de CI Windows** capaz de publicar el artefacto y de
  ejecutar la instalación y la corrida en un entorno **sin runtime previo del proyecto**; para la vía
  **declarativa**, la plataforma de instalación disponible (MSIX) o la vía de carpeta/zip con la asociación
  documentada como paso de instalación (ADR-009 §3.6: la asociación se documenta para el usuario o el
  administrador).
- **Sin dependencias externas de producto:** el artefacto es autocontenido; el único "externo" es el entorno de
  verificación y el canal (excluido).
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de lo que se retira (la
  asociación en runtime, la metadata de publicación anterior y las APIs muertas).
- **Build state de partida:** el shell del Bolt 025 compila y su suite corre; este Bolt **no** cambia el código
  funcional del shell —cambia su **forma de entrega** y **retira** APIs y metadata— y **no** agrega dependencias.
- **Decisión de ejecución registrada (no es supuesto):** la verificación de la asociación y del arranque limpio se
  ejecuta **sobre una instalación real** del artefacto producido por la propia corrida; no se declara verificada
  por inspección del `.csproj`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-027`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — El artefacto publicable (con el motor y el CLI adentro)

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 La forma de publicación

El shell se publica con **`dotnet publish` self-contained para `win-x64`** (ADR-009 §3.6, ADR-001 §3.5), como
**carpeta/zip autocontenida** en la ubicación que define la distribución del tooling, **sin ClickOnce** y **sin
instalador propio de la aplicación**. Se definen las propiedades de publicación **en el árbol de build**
(`Directory.Build.props` / el perfil de publicación del proyecto del shell), no en la máquina del operador, para
que la forma sea **reproducible**: `SelfContained`, el RID, la ausencia de metadata de publicación anterior y la
inclusión del **motor** y el **CLI** en el mismo payload.

**La vía *single-file* es válida y queda soportada** (es el escenario donde las APIs retiradas se comportan
distinto —`Assembly.Location` vacío—): las rutas se resuelven por la BCL (§C.2) y la corrida de integración
verifica el artefacto en la forma que se publique.

**Files created:**
- `2.0/src/CooperatorModeler/Properties/PublishProfiles/win-x64-selfcontained.pubxml` — el perfil de publicación autocontenida del shell
- `2.0/docs/packaging/composition.json` — el **registro de la composición** del artefacto (qué lleva adentro: shell, motor, CLI, con sus versiones y digests)
- `2.0/build/publish-tooling.ps1` — el paso reproducible de publicación y el registro de la composición (usado por la corrida de integración y por quien empaqueta)

**Files modified:**
- `2.0/Directory.Build.props` — las propiedades de publicación compartidas (RID, self-contained, sin la forma anterior)

#### A.2 MSIX como opción declarativa (instalación + asociación)

Cuando se necesiten **instalación** y **asociación declarativa** de archivos, la opción es **MSIX** (ADR-009 §3.6,
ADR-001 §3.5): el paquete declara la extensión `.coop2`, el ícono y el identificador de programa como **recursos
del paquete**, y **no** requiere que la aplicación escriba nada. La instalación es **por usuario** (sin
elevación) siempre que la plataforma lo permita; la vía carpeta/zip **no** instala nada y documenta la asociación
como **paso de instalación** para el usuario o el administrador (§D.3).

**Files created:**
- `2.0/src/CooperatorModeler/Package.appxmanifest` — la declaración de la asociación del `.coop2`, el ícono y el identificador de programa como recursos del paquete

> **Frontera:** el **feed**, la **firma** y la **publicación** de estos artefactos **no** son de este Bolt
> (ADR-001 §3.6, §4/§13).

---

### Phase B — El retiro de la forma anterior y de las APIs muertas

**Duration:** 3h total cycle — **Complexity:** Medium

#### B.1 El inventario de lo que se retira

| Pieza | Dónde estaba (1.4) | Acción |
|---|---|---|
| Asociación de la extensión escrita por la **aplicación** | `Tools/CooperatorModeler/FileExtensionAssociation.cs` y su invocación desde el shell | **Se retira** del código: la asociación es **del instalador** (ADR-009 §3.4) |
| Metadata de publicación anterior (ClickOnce/bootstrapper) y su **referencia** | los `.csproj` del tooling | **Se retiran** de **todo** el tooling (ADR-009 §3.5, ADR-001 §3.3) |
| `Application.ExecutablePath` (resolución del ejecutable por la interfaz retirada) | el tooling 1.4 | **Se retira**: la ruta sale de `Environment.ProcessPath` (ADR-009 §3.4) |
| `Assembly.Location` (excepción en *single-file*) | el tooling 1.4 | **Se retira**: la carpeta sale de `AppContext.BaseDirectory` (ADR-009 §3.4) |
| Inicialización de la interfaz retirada (`Application.EnableVisualStyles`, `Application.DoEvents`) | el tooling 1.4 | **Se retira** (ADR-009 §3.5) |
| `System.Deployment` / ClickOnce | el tooling 1.4 | **Se retira** (ADR-009 §3.5, ADR-001 §3.3/§3.5) |

La verificación es de **ausencia** sobre las **fuentes y los binarios publicados** (no sólo sobre el `.csproj`), y
la búsqueda abarca **todo** el tooling, no sólo el shell: es el modo en que la forma anterior "reaparece por
copiar el proyecto viejo".

**Files modified:**
- `2.0/src/CooperatorModeler/` — retiro de la asociación en runtime, de la inicialización retirada y de las APIs muertas; rutas por la BCL
- los `.csproj` del tooling — retiro de la metadata de publicación anterior y de su referencia

---

### Phase C — La integración con el sistema operativo

**Duration:** 3h total cycle — **Complexity:** Medium

#### C.1 Abrir la solución o carpeta generada, con declaración explícita

La apertura de lo generado usa `Process.Start` **con la declaración explícita** que el runtime moderno exige
(`UseShellExecute = true` para delegar en el shell del sistema operativo), y cuando **no** hay asociación del
sistema operativo para el tipo de archivo, **degrada a abrir la carpeta** con un **mensaje que dice exactamente
eso** (nunca una degradación silenciosa). El **enlace informativo** (por ejemplo el de la licencia) sigue el mismo
patrón: declaración explícita y **verificación de SO** donde la plataforma lo requiera.

**Files modified:**
- `2.0/src/CooperatorModeler/` — la apertura de lo generado y el enlace informativo con su declaración explícita y su mensaje de degradación

#### C.2 Las rutas por la BCL

El ejecutable se resuelve por **`Environment.ProcessPath`** y la carpeta de la aplicación por
**`AppContext.BaseDirectory`** (ADR-009 §3.4): es lo que hace que el artefacto funcione igual en carpeta y en
*single-file*, y lo que retira la dependencia de la ubicación del ensamblado.

#### C.3 El diagnóstico: archivo por usuario, nunca el registro de eventos

El shell escribe su **diagnóstico en un archivo por usuario**, con **política de tamaño y rotación** declarada
(ADR-009 §3.5), y **nunca** en el registro de eventos del sistema. La política es explícita: ubicación dentro de
los datos del usuario, tamaño máximo por archivo, número de archivos retenidos y comportamiento ante fallo de
escritura del diagnóstico (no rompe la aplicación, lo informa una vez). **Nada de secretos** en el diagnóstico
(ADR-009 §3.5/§3.8): las credenciales no se registran.

**Files created:**
- `2.0/src/CooperatorModeler/Diagnostics/UserFileLog.cs` — el diagnóstico a archivo por usuario con su política de tamaño y rotación
- `2.0/src/CooperatorModeler/Diagnostics/LogPolicy.md` — la política declarada (ubicación, tamaño, retención, qué nunca se registra)

---

### Phase D — El funcionamiento sin asociación y la evidencia

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 Sin asociación, el producto funciona

El formato `.coop2` se abre **por el diálogo de archivos** y **por argumento** de línea de comandos sin que la
asociación exista (ADR-009 §3.4/§3.6). Es el escenario que hace honesta la frase "la asociación la aporta el
instalador": el producto **no depende** de ella.

#### D.2 La verificación de la no-escritura del Registro

Ni la **instalación** (la declaración la aporta el paquete) ni el **uso normal** de la aplicación escriben el
Registro: ni para la asociación, ni para el estado del usuario (que vive en su archivo propio, ADR-009 §3.3 y
Bolt 025). La verificación es de **comportamiento** sobre la instalación real (comparación del estado relevante
antes y después del uso), no de inspección del código.

#### D.3 La documentación de instalación

Para la vía carpeta/zip —donde no hay asociación declarativa— la asociación se documenta como **paso de
instalación** para el usuario o el administrador, con el efecto de no hacerlo (el formato igual se abre por
diálogo y por argumento). Se evita que "no hay asociación automática" se lea como una regresión.

**Files created:**
- `2.0/docs/packaging/INSTALL.md` — cómo se instala el artefacto, dónde vive la asociación (instalador), qué pasa sin ella y qué queda fuera (la publicación)

#### D.4 Evidencia

- El **artefacto publicable** producido, con su **registro de composición** verificado contra lo que lleva
  adentro, y su corrida en una **máquina limpia** (arranque, apertura del formato por diálogo y por argumento,
  generación del modelo de referencia y cierre).
- La instalación dejando la **asociación funcionando**, y la constancia de que la aplicación **no** escribió el
  Registro en el uso normal.
- La apertura del formato **sin** asociación presente.
- La **ausencia** de la metadata de publicación anterior y de su referencia en **todo** el tooling, y la
  **ausencia** de las APIs retiradas en las fuentes y en los binarios publicados.
- La apertura de la solución/carpeta generada con su **declaración explícita** y su **degradación al mensaje
  previsto**.
- El **archivo de diagnóstico por usuario** con su política de tamaño y rotación.
- La **lista de rupturas** de este Bolt (qué desaparece del producto: la asociación en runtime y las APIs
  muertas) para las **notas de migración** (ADR-008 §3.1; documento del Bolt 028), y el **registro de la
  exclusión condicionada** de la publicación (§4/§13).

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: El artefacto se produce, arranca sin runtime previo y viaja con el motor y el CLI

**Given** la corrida de publicación del tooling
**When** el artefacto se ejecuta en una máquina **sin runtime previo** del proyecto
**Then** el shell **arranca**, abre el formato por diálogo y por argumento, genera el modelo de referencia y
cierra; y su **registro de composición** coincide con lo que el artefacto lleva adentro (shell, **motor** y
**CLI**)

### AC-2: La asociación la aporta el instalador y funciona

**Given** la instalación del artefacto por la vía declarativa
**When** se abre un `.coop2` desde el sistema operativo
**Then** la **asociación deja de ser un paso del usuario** (la aportó el instalador) y el producto **no** escribe
el Registro para lograrlo

### AC-3: La aplicación no escribe el Registro, ni para la asociación ni para su estado

**Given** una instalación real y un uso normal de la aplicación
**When** se compara el estado relevante del sistema antes y después
**Then** no hubo escritura de la aplicación en el Registro —ni por la asociación, ni por el estado del usuario, ni
por el diagnóstico— y el ícono y el identificador de programa son **recursos del paquete**

### AC-4: Sin asociación, el formato se abre igual

**Given** un entorno **sin** la asociación del `.coop2`
**When** el usuario abre el formato por el diálogo de archivos y por argumento de línea de comandos
**Then** en los dos casos el formato se abre y el producto funciona con normalidad

### AC-5: No queda la forma de publicación anterior en ningún proyecto del tooling

**Given** el árbol 2.0 y el artefacto publicado
**When** se busca la metadata de publicación anterior y su referencia
**Then** **0** apariciones en **todos** los proyectos del tooling, en las propiedades de publicación y en el
artefacto (sin ClickOnce, sin `System.Deployment`, sin bootstrapper)

### AC-6: No quedan las APIs que ataban la aplicación a la forma vieja

**Given** las fuentes y los binarios publicados
**When** se busca la inicialización de la interfaz retirada y la resolución de rutas por la ubicación de la
librería
**Then** **0** apariciones de las APIs retiradas (`Application.ExecutablePath`, `Assembly.Location`,
`Application.EnableVisualStyles`, `Application.DoEvents`) y las rutas salen de **`Environment.ProcessPath`** y
**`AppContext.BaseDirectory`**

### AC-7: La apertura de lo generado declara y degrada con mensaje

**Given** la apertura de la solución o carpeta generada y el enlace informativo
**When** se ejecutan (con y sin asociación del sistema operativo)
**Then** usa la **declaración explícita** que el runtime moderno exige y, **sin** asociación, degrada a la carpeta
con un **mensaje que dice exactamente eso** (nunca una degradación silenciosa)

### AC-8: El diagnóstico es un archivo por usuario, con política declarada

**Given** el shell en uso
**When** se revisa su diagnóstico
**Then** escribe en un **archivo por usuario** con **política de tamaño y rotación** declarada, **no** escribe en el
registro de eventos del sistema y **no** registra credenciales ni material sensible

### AC-9: La publicación queda excluida y declarada como riesgo con dueño y fecha

**Given** la evidencia del V-Bounce y su documentación
**When** se busca qué quedó fuera
**Then** está escrito —como **exclusión explícita** y como **riesgo** con dueño (`tech_lead`) y fecha (antes del
release 2.0.0)— que el **feed**, la **firma de paquetes** y la **publicación de los `.nupkg`** son del **canal de
distribución** (ADR-001 §3.6), y que este Bolt **no** firma, **no** publica y **no** elige canal

### AC-10: La ruptura queda registrada

**Given** lo que este Bolt retira del producto (la asociación en runtime y las APIs muertas)
**When** se revisa la constancia del cambio
**Then** cada ruptura está registrada con sus tres datos y su reemplazo (la asociación pasa al instalador; las
rutas pasan a la BCL) para las notas de migración (ADR-008 §3.1)

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — el shell se produce como artefacto autónomo publicable y arranca en una máquina sin runtime ni componentes previos | §6/A.1 | AC-1 · `PublishArtifactTests` |
| Bolt §2 — el payload lleva el motor y el CLI | §6/A.1 | AC-1 · registro de composición |
| Bolt §2 — la asociación la aporta el instalador de forma declarativa y la aplicación no escribe el Registro | §6/A.2, §6/D.2 | AC-2, AC-3 · `InstallerAssociationTests` |
| Bolt §2 — sin asociación, el formato se abre por diálogo y por argumento | §6/D.1 | AC-4 · `NoAssociationOpenTests` |
| Bolt §2 — el ícono y el identificador de programa son recursos del paquete | §6/A.2 | AC-3 · inspección del paquete + `InstallerAssociationTests` |
| Bolt §2 — no queda metadata de publicación anterior ni su referencia en ningún proyecto del tooling | §6/B.1 | AC-5 · `RetiredSurfaceTests` |
| Bolt §2 — se retiran las APIs de las que dependía la forma vieja y las rutas salen de la BCL | §6/B.1, §6/C.2 | AC-6 · `RetiredSurfaceTests` + inspección de fuentes y binarios |
| Bolt §2 — la apertura de lo generado y el enlace informativo con declaración explícita y degradación con mensaje | §6/C.1 | AC-7 · `SystemIntegrationTests` |
| Bolt §2 — el diagnóstico a archivo por usuario, con política de tamaño y rotación, nunca el registro de eventos | §6/C.3 | AC-8 · `SystemIntegrationTests` + `LogPolicy.md` |
| Bolt §2 — la publicación queda excluida y condicionada (ADR-001 §3.6) | §4, §6/D.4, §13 | AC-9 · `INSTALL.md` + MEM |
| ADR-009 §3.4/§3.5/§3.6 y ADR-001 §3.5 — asociación por instalador, rutas por la BCL, integración declarada, publicación autónoma sin ClickOnce | §6/A.1, §6/A.2, §6/B.1, §6/C | AC-1…AC-8 |
| ADR-008 §3.1 — la ruptura registrada con su reemplazo | §6/D.4 | AC-10 · lista de rupturas en el MEM |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **La forma de publicación (verificable sin instalar):** las propiedades de publicación producen un artefacto
  **autocontenido por RID** (sin la metadata anterior) y el **registro de composición** se genera y coincide con
  el contenido (una verificación de que el motor y el CLI están en el payload y de que las versiones declaradas
  son las del árbol).
- **Ausencia de lo retirado (invariante):** búsqueda en **todos** los proyectos del tooling de la metadata de
  publicación anterior y de `System.Deployment`/ClickOnce; búsqueda en las fuentes del shell de
  `FileExtensionAssociation`, HKCR, `Application.ExecutablePath`, `Assembly.Location`,
  `Application.EnableVisualStyles` y `Application.DoEvents`: **0** apariciones. La búsqueda se corre **antes** del
  retiro para fijar el inventario y **después** para probar el cierre.
- **Las rutas por la BCL (unitario):** la resolución del ejecutable y de la carpeta de la aplicación se prueba
  sobre las dos APIs de la BCL (con el proceso del test como sujeto) y se verifica que la lógica **no** consulta la
  ubicación del ensamblado (búsqueda + prueba de que el valor provisto no cambia si el ensamblado se carga desde
  otra ubicación —escenario *single-file*—).
- **La política del diagnóstico (unitario, sin UI):** el componente de archivo por usuario se ejercita sobre una
  carpeta temporal: tamaño máximo por archivo, rotación por número de archivos, comportamiento ante fallo de
  escritura (no rompe la operación y lo informa una vez) y **0** secretos en lo escrito (búsqueda del valor/clave
  de una operación de prueba).
- **La apertura declarada y su degradación (unitario con doble del shell del SO):** la apertura de lo generado
  invoca el shell del sistema operativo con la **declaración explícita** y, cuando la asociación no existe, el
  resultado es la **apertura de la carpeta** más el **mensaje previsto** (el mensaje se verifica como texto
  declarado: dice que no hay asociación).
- **Consumer-side contract tests (test doubles):** `n/a` para servicios (no hay ninguno en el alcance); el doble
  que **sí** aplica es el del **shell del sistema operativo** (la apertura y su fallo por falta de asociación),
  que es lo que permite probar AC-7 sin instalar.
- **Edge cases:** ruta de la aplicación con espacios y con caracteres no ASCII; ejecución desde una unidad de red
  (las rutas de la BCL no dependen del directorio de trabajo); carpeta de diagnóstico **inexistente** (se crea) y
  **de sólo lectura** (no rompe la aplicación: registra el aviso una vez); archivo de diagnóstico en el límite de
  tamaño (rota) y con más archivos que la retención (descarta el más viejo); apertura con un archivo que **no**
  existe (mensaje accionable); apertura con la asociación **presente** pero el archivo **bloqueado** por otro
  proceso.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida de publicación con su registro de composición, las búsquedas
  de ausencia (antes/después), la prueba de las rutas por la BCL en el escenario *single-file*, la política del
  diagnóstico con sus bordes y la prueba de la apertura declarada con su degradación. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "applicable"` porque el Bolt declaró `test_integration: required`
> (§3.2.2). Los tests se **escriben, construyen y verifican estáticamente** en este V-Bounce y se **ejecutan** en
> el ciclo de test de integración del Bolt, **después** de `AITL-MEM-Approval`; el resultado queda como **hecho**
> en `integration_test_runs[]` del manifiesto.

- **Integration tests (real dependencies — una instalación real):**
  - `PublishArtifactTests` — se publica el artefacto en el entorno y se ejecuta en una **máquina/imagen sin
    runtime previo**: arranque, apertura del formato por diálogo y por argumento, generación del modelo de
    referencia y cierre; el registro de composición se contrasta con el contenido real (shell, motor, CLI).
  - `InstallerAssociationTests` — la instalación por la vía declarativa deja la **asociación funcionando** (se
    abre un `.coop2` desde el sistema operativo) y el **uso normal** de la aplicación no escribe el Registro.
  - `NoAssociationOpenTests` — en un entorno **sin** asociación, el formato se abre por el diálogo y por argumento.
  - `SystemIntegrationTests` — la apertura de la solución/carpeta generada con su declaración explícita y su
    **degradación al mensaje previsto** cuando no hay asociación del sistema operativo; el **archivo de
    diagnóstico por usuario** con su política de tamaño y rotación; y la constancia de que **nada** escribe el
    registro de eventos del sistema.
  - `RetiredSurfaceTests` — el artefacto publicado y **todo** el tooling sin la metadata de publicación anterior ni
    su referencia, y sin las APIs retiradas (sobre los binarios publicados, no sólo sobre las fuentes).
  - **Quién los escribió:** este V-Bounce, en un proyecto de integración del tooling, con la marca de categoría
    del arnés del Bolt 001.
- **Contract / E2E against the real providers:** el "proveedor real" de este Bolt es el **sistema operativo y su
  instalador**: la asociación, la apertura y la ausencia de escrituras son contratos con la plataforma y se
  ejercen de verdad en esta corrida (no hay dobles en el nivel de integración).
- **Edge cases (los que sólo aparecen en una instalación real):** instalación **por usuario** sin elevación;
  instalación con el artefacto **ya instalado** (reinstalación/actualización: la asociación no se duplica ni se
  rompe); desinstalación que **retira** la asociación (y el producto sigue funcionando por diálogo/argumento);
  usuario sin permisos de escritura en la carpeta de diagnóstico; ejecución desde una ruta con espacios; un
  `.coop2` abierto por doble clic **con** la asociación y **sin** ella.
- **Humo de arranque y generación en la máquina limpia:** se registra como evidencia de la corrida (arranque,
  generación del modelo de referencia y cierre) **sin umbral numérico**, porque ninguna fuente fija un objetivo de
  rendimiento para esta pieza (§9, fila de *Perf-smoke*).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.
- **Cierre del plan antes de la aceptación (§3.2.2):** una **corrida cubriente que pasa** sobre el commit del
  Bolt, registrada en `integration_test_runs[]`. Si el entorno no está disponible, el Bolt queda **bloqueado en su
  ciclo** (no pasa a `ready-for-acceptance`) y el MEM lo registra; **no** se declara el plan satisfecho sin corrida
  (y una exención por `waived` exigiría un ADR de excepción de gate, G21).

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* se declaran **`pass`** porque el plan de integración es
> `applicable` y su corrida es la que cierra el Bolt (§8.2).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); la publicación se produce y el artefacto es autocontenido por RID | pass |
| Unit / component tests | V-Bounce | 100 % verdes: rutas por la BCL (incluido el escenario *single-file*), política del diagnóstico con sus bordes y apertura declarada con su degradación | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El **shell del sistema operativo** se dobla para probar la apertura declarada y su degradación sin instalación; no hay servicios externos en el alcance | pass |
| SAST | V-Bounce | 0 hallazgos altos; 0 escritura del Registro desde la aplicación; 0 APIs retiradas | pass |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas en el paquete del shell (ADR-001 §3.4); el artefacto es autocontenido por diseño | pass |
| OWASP Top 10 — static review | V-Bounce | Ejecución de procesos: `Process.Start` con **declaración explícita** y sin concatenación de entrada del usuario en la apertura; sin secretos en el diagnóstico; sin privilegios requeridos para el uso normal (instalación por usuario, vía carpeta/zip sin instalación) | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción en esta entrega | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** secretos en el diagnóstico, en el paquete, en el registro de composición y en la documentación de instalación | pass |
| Hallucination lint | V-Bounce | Cada pieza retirada y cada decisión de empaquetado trazable a ADR-009 §3.4-§3.6 / ADR-001 §3.3/§3.5 y al código 1.4 citado; 0 capacidades de otros Bolts (editor, estado de usuario, flujo de plan) adelantadas | pass |
| IP / license provenance | V-Bounce | Código propio; la única dependencia de terceros del artefacto es la del producto (ya autorizada); **0** dependencias nuevas | pass |
| PII / DLP | V-Bounce | `internal`: el artefacto y su diagnóstico no contienen datos personales; el diagnóstico **no** registra credenciales (regla declarada en `LogPolicy.md`) | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes ni se resuelven componentes por nombre; la instalación es declarativa | n/a (razón) |
| Test-first evidence | V-Bounce | El **inventario de lo retirado** (§B.1) y la **política del diagnóstico** se escriben **antes** de tocar el shell; y la prueba de "sin asociación" se escribe antes de retirar la asociación en runtime (es la que demuestra que el producto no la necesita) | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | El paso de publicación es reproducible (mismas propiedades ⇒ misma composición declarada); el registro de composición se genera con orden canónico y dos corridas dan el mismo contenido | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | Corrida **cubriente que pasa** sobre una instalación real (artefacto, asociación declarativa, apertura sin asociación, integración con el sistema, ausencia de lo retirado); registrada en `integration_test_runs[]` | pass |
| Contract / E2E against the real providers | Integration test | La misma corrida ejerce el contrato real con el **sistema operativo y su instalador** (asociación, apertura, ausencia de escrituras) | pass |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — la aplicación no expone superficie de red ni servicio: es una aplicación de escritorio local | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | `n/a` — ninguna fuente fija un objetivo de rendimiento para esta pieza; el **humo de arranque y generación** en la máquina limpia se registra como evidencia de la corrida sin umbral numérico (§8.2) | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | El shell entrega su **diagnóstico a archivo por usuario** (política declarada) y no emite telemetría a ningún colector (§11) | pass |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). La entrega es un **artefacto
  distribuible** y su integración con el sistema: no procesa datos personales ni de negocio.
- **Privilegio mínimo:** la instalación declarativa es **por usuario** (sin elevación) y la vía carpeta/zip **no
  instala nada** (se ejecuta desde su carpeta); el uso normal **no** requiere permisos de administrador y la
  aplicación **no** escribe el Registro —ni para la asociación ni para su estado— (ADR-009 §3.4/§3.5). Es el
  control que reemplaza al patrón anterior, donde la aplicación modificaba HKCR en runtime.
- **Sin secretos en lo que se distribuye ni en lo que se registra:** el diagnóstico por usuario **no** incluye
  credenciales ni material de claves (ADR-009 §3.5/§3.8) y su política lo declara; el registro de composición
  lleva versiones y digests, **no** contenido sensible.
- **Ejecución de procesos con declaración explícita:** la apertura de lo generado y el enlace informativo pasan
  por el shell del sistema operativo con la declaración que el runtime moderno exige; el valor que se abre
  proviene de una ruta del propio producto (no de una entrada no confiable) y, aun así, la apertura **no**
  concatena entrada del usuario en una línea de comandos.
- **La firma y la procedencia del artefacto son una frontera declarada:** la **firma de paquetes** y el **canal**
  (feed) están **fuera** de este Bolt (ADR-001 §3.6) y su ausencia **no** se disimula: queda como **exclusión** y
  como **riesgo con dueño y fecha**. Este Bolt produce el artefacto; **no** garantiza su procedencia criptográfica.
- **Sin superficie nueva de red ni de archivos:** el artefacto es autocontenido, funciona sin conexión y escribe
  sólo su diagnóstico en el perfil del usuario.

---

## 11. Monitoring and observability

- **Diagnóstico del producto (parte del entregable):** el shell escribe su diagnóstico en un **archivo por
  usuario** con política de **tamaño y rotación** declarada (`2.0/src/CooperatorModeler/Diagnostics/LogPolicy.md`),
  y **nunca** en el registro de eventos del sistema (ADR-009 §3.5). Ese archivo es la observabilidad del producto
  en la máquina del usuario.
- **`n/a` en el sentido de telemetría remota:** la aplicación no emite métricas ni trazas a ningún colector y no
  se le agrega instrumentación (ninguna fuente lo pide; y su estado de usuario/diagnóstico es local).
- **La observabilidad de este Bolt es su evidencia:** el artefacto con su **registro de composición**, la corrida
  en la **máquina limpia**, la asociación aportada por el instalador, la apertura sin asociación, la ausencia de
  escrituras (Registro/registro de eventos) y las búsquedas de lo retirado en fuentes y binarios.
- **Del lado operativo:** la instalación y su asociación quedan documentadas para el usuario o el administrador
  (`INSTALL.md`), que es lo que permite diagnosticar "no se abre el archivo" sin abrir un depurador.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay es un **cambio en la forma de entrega y de instalación**
  y un **retiro** en el producto: la asociación del formato pasa de la **aplicación** al **instalador**, las rutas
  pasan de la interfaz retirada y de la ubicación del ensamblado a la **BCL**, y el diagnóstico pasa del registro
  de eventos a un **archivo por usuario**. Todo entra en las **notas de migración** (documento del **Bolt 028**,
  ADR-008 §3.1) con sus tres datos: qué cambia (la asociación ya no la escribe la aplicación; quien publicaba el
  tool anterior deja de tener esa forma), por qué (ADR-009 §3.4-§3.6, ADR-001 §3.5) y con qué se reemplaza (la
  asociación declarativa del instalador y, sin ella, el diálogo de archivos y el argumento).
- **Compatibility:** el producto **funciona sin asociación** (escenario verificado), de modo que una instalación
  por carpeta/zip no pierde funcionalidad: pierde el doble clic, que se documenta como paso de instalación. El
  formato `.coop2` y el flujo de generación no cambian (son de los Bolts 003/004/017/018/025/026).
- **Lo que queda fuera y se declara:** la **publicación** (feed/firma) es del **canal** (ADR-001 §3.6). Si la
  decisión del canal cambiara la **forma** del artefacto (por ejemplo exigir otro empaquetado o un canal
  firmado), este Bolt se **revisa por el camino normal** (enmienda del Bolt y revisión de este SPEC), no por
  acuerdo verbal.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido por esta entrega (el artefacto se
  produce en la corrida; el diagnóstico es del usuario y se puede borrar). En una máquina instalada, la
  desinstalación retira la asociación y el producto vuelve a la ejecución desde su carpeta; la ventana de
  asociación la controla el instalador, no la aplicación, de modo que revertir el paquete **no** deja el sistema
  con una asociación huérfana escrita por el producto (nunca lo hizo).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **La publicación sin decidir bloquea el release 2.0.0** (riesgo principal y declarado) | 4 | 4 | La decisión del **canal** tiene **dueño** (`tech_lead`) y **fecha** anterior al release (ADR-001 §3.6); este Bolt entrega todo lo demás (artefacto, instalación, asociación, retiro) y **no** lo sustituye; queda como exclusión explícita, riesgo declarado y en el MEM |
| **La asociación escrita por la aplicación** (el defecto de F-35 que reaparece) | 2 | 4 | La asociación es **del instalador** y se verifica que la aplicación **no** escribe el Registro en el uso normal (AC-2/AC-3); búsqueda de HKCR y de la clase de asociación en las fuentes |
| **El artefacto no arranca en una máquina limpia** (dependencia implícita del runtime o de componentes previos) | 3 | 4 | La evidencia es la corrida en una **máquina sin runtime previo** con el **registro de composición** verificado; el artefacto es autocontenido por diseño |
| **La instalación por carpeta/zip deja la asociación como paso manual y se lee como regresión** | 3 | 2 | La asociación se **documenta como paso de instalación** (`INSTALL.md`) y el producto **funciona sin ella** (AC-4): la pérdida es el doble clic, no la funcionalidad |
| **La apertura de la solución generada degrada en silencio** | 3 | 3 | La apertura usa la **declaración explícita** y, sin asociación del SO, degrada a la **carpeta** con un **mensaje que lo dice** (AC-7) |
| **Las rutas vuelven a depender de la ubicación del ensamblado** (falla en *single-file*) | 3 | 3 | Rutas por `Environment.ProcessPath`/`AppContext.BaseDirectory` + búsqueda de las APIs retiradas + prueba del escenario *single-file* |
| **El diagnóstico vuelve al registro de eventos del sistema** | 2 | 3 | Política declarada y verificada: archivo por usuario con tamaño y rotación, y ningún uso del registro de eventos (AC-8) |
| **La forma de publicación anterior reaparece** por copiar el proyecto viejo | 3 | 3 | Búsqueda de la metadata y de su referencia en **todo** el tooling, antes y después, y sobre los **binarios publicados** |
| **`Process.Start` sin la declaración explícita** (no abre, o abre de otra forma) | 3 | 2 | La declaración es explícita y se prueba con y sin asociación (doble del shell del SO en lo hermético; corrida real en integración) |
| **El paquete MSIX (opción) no está disponible en el entorno de verificación** | 3 | 3 | La vía **primaria** es la carpeta/zip autocontenida, que se verifica siempre; MSIX es **opción** para instalación + asociación (ADR-009 §3.6) y, si no está disponible en el entorno, la corrida lo declara y la asociación se documenta como paso de instalación. Si el aprobador exige la vía declarativa como obligatoria, cambia el `environment` del plan |
| **Procedencia del artefacto** (sin firma ni canal) | 4 | 3 | Declarado como **frontera**: la firma y el canal son de ADR-001 §3.6; el registro de composición da trazabilidad del contenido, **no** autenticidad |
| **Dependencia nueva colada** en el paquete del shell | 2 | 3 | ADR-001 §3.4 y el gate de dependencias |
| **El artefacto se publica con propiedades definidas en la máquina** (forma no reproducible) | 3 | 3 | Las propiedades viven en el árbol de build (perfil + props compartidas) y el paso de publicación es el del repositorio |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-009/ADR-001; acá se aplican.
> Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) La forma primaria es la carpeta/zip autocontenida y MSIX es opción (no obligatoria) para la verificación** | ADR-009 §3.6 dice exactamente eso ("`dotnet publish` self-contained `win-x64` como canal primario… MSIX queda como opción cuando se necesiten instalación y asociación declarativa"). Exigir MSIX como obligatorio cambiaría el `environment` de la corrida y haría depender el Bolt de la plataforma de instalación; la asociación se demuestra con MSIX cuando está disponible y, cuando no, se documenta como paso de instalación (que es lo que el propio ADR prevé para la vía carpeta/zip). Si el aprobador quiere MSIX obligatorio, es una revisión de este SPEC |
| **(⚠ aprobador) El diagnóstico a archivo por usuario se implementa como componente propio con política declarada (ubicación, tamaño por archivo, retención, comportamiento ante fallo)** | ADR-009 §3.5 exige "archivo por usuario con política de tamaño y rotación" pero no fija los números; el SPEC los declara para que sean verificables y para que el operador sepa dónde mirar. La alternativa (dejar la política a criterio del ejecutor) haría la evidencia no comparable |
| Las propiedades de publicación viven en el **árbol de build** (perfil + props compartidas) y no en la máquina | Es lo que hace **reproducible** la forma del artefacto y evita que "la forma de publicación" dependa de quién lo publica; el registro de composición se genera en el mismo paso |
| El **registro de composición** (`2.0/docs/packaging/composition.json`) es un artefacto versionado del repositorio | El Bolt pide "el registro de su composición (qué lleva adentro, motor y CLI incluidos)": tenerlo como artefacto permite contrastarlo con el contenido real y sirve de insumo para quien empaqueta |
| La verificación "sin asociación" se hace **antes** de retirar la asociación en runtime | Es la prueba de que el producto **no depende** de la asociación (y no una justificación a posteriori de haberla quitado): el orden de la evidencia importa |
| La búsqueda de lo retirado mira también los **binarios publicados** | La metadata anterior y las APIs pueden sobrevivir en un `.csproj` sin usarse o reaparecer en un artefacto generado; mirar sólo el fuente deja pasar el caso "reaparece por copiar el proyecto viejo" |
| MSIX (opción) declara la extensión, el ícono y el identificador **como recursos del paquete** | Es la diferencia con el patrón retirado: el paquete **declara**, la aplicación **no escribe**. Y es lo que ADR-009 §3.4 pide para el ícono y el identificador de programa |
| La **instalación por usuario** (sin elevación) se prefiere siempre que la plataforma lo permita | El producto es de escritorio y no necesita privilegios; pedirlos ampliaría el impacto de la instalación sin necesidad (y la vía carpeta/zip no necesita ninguno) |
| No se construye un instalador propio (MSI/EXE) | ADR-009 §3.6: la instalación es **declarativa** (MSIX) o **carpeta/zip**; un instalador propio sería una pieza más del producto, con su propio ciclo de vida, sin que ninguna fuente lo pida |
| La publicación (feed/firma) se declara **exclusión** y **riesgo**, no "pendiente de hacer acá" | Es una decisión de otro dueño con fecha (ADR-001 §3.6); tratarla como trabajo pendiente de este Bolt haría que su aceptación dependiera de algo que no controla |

---

## 15. Stop conditions

- **El shell (Bolt 025) o el motor/CLI (Bolts 017/018) no están** en el árbol 2.0: parar y reportar. Sin ellos no
  hay artefacto que publicar ni composición que armar.
- **La decisión del canal de distribución cambia la forma del artefacto** (otro empaquetado, firma obligatoria del
  binario, canal que impone un instalador distinto): parar y reportar. La salida es una **enmienda del Bolt** +
  revisión de este SPEC (ADR-001 §3.6), nunca un cambio silencioso en la corrida.
- **Se pide firmar, publicar o elegir el canal** en este Bolt: parar. Es la exclusión explícita (§4) y una decisión
  del `tech_lead` con fecha anterior al release.
- **Se pide que la aplicación escriba la asociación** (o el estado del usuario en el Registro, o el diagnóstico en
  el registro de eventos): parar. ADR-009 §3.4/§3.5 lo prohíbe y es el defecto que el Bolt retira.
- **Se pide un instalador propio** (MSI/EXE a medida) o la reincorporación de ClickOnce/bootstrapper: parar y
  reportar; ADR-009 §3.5/§3.6 y ADR-001 §3.3/§3.5 fijan la forma y el ciclo la retira.
- **El producto no funciona sin asociación** (algo lo hace depender de ella): parar. El escenario "sin asociación"
  es un criterio de aceptación (AC-4), no una concesión.
- **El entorno de integración no permite una instalación real** (sin máquina/imagen limpia o sin la plataforma
  declarativa): el V-Bounce puede cerrar con los tests escritos y verificados estáticamente, pero el Bolt **no**
  pasa a `ready-for-acceptance`: queda en su ciclo de integración y el MEM registra el bloqueo. **No** se declara
  el plan satisfecho ni se pide un waiver sin ADR (G21).
- **Se pretende que este Bolt entregue el editor, el estado de usuario o el flujo de plan/reporte del shell**:
  parar; son de los Bolts 025/026.
- **Se pretende que este Bolt modifique el motor o el CLI**: parar; se **incluyen** en el payload, no se modifican
  (sus Bolts son 017/018).
- **Un artefacto publicado arrastra la forma de publicación anterior o las APIs retiradas**: parar y corregir
  antes de cerrar el V-Bounce; la búsqueda corre sobre los binarios publicados.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado por
  `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG**
- [ ] Code follows applicable ADRs (ADR-009 §3.4/§3.5/§3.6, ADR-001 §3.1/§3.3/§3.4/§3.5/§3.6, ADR-008 §3.1)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: **corrida cubriente que pasa** registrada en `integration_test_runs[]` del manifiesto (Bolt `test_integration: required`, plan `applicable`); sin corrida, el Bolt permanece en su ciclo de integración y no pasa a `ready-for-acceptance`

---

## 17. References

- [`US-000.BOLT-027`](../functional/bolts/US-000.BOLT-027-empaquetado-y-asociacion.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-35 (la asociación escrita por la aplicación, con el ejecutable y el ícono resueltos por APIs/rutas de la forma vieja), F-36 (la metadata de publicación anterior y su referencia muerta) y §6 ítem 15.
- [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — §3.4 (asociación por el instalador, funcionamiento sin ella, rutas por la BCL, ícono e identificador como recursos del paquete), §3.5 (integración con el sistema, retiro de las APIs retiradas y de `System.Deployment`/ClickOnce, diagnóstico a archivo por usuario), §3.6 (publicación autónoma, instalación declarativa como opción, sin ClickOnce, firma/canal fuera), §3.8 (interplay).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (layout, distribución del tooling y folder feed), §3.3 (naming/versionado y limpieza obligatoria de metadata de publicación), §3.4 (dependencias), §3.5 (supervivencia Windows-only, self-contained, MSIX opción, sin ClickOnce), §3.6 (el **canal** pendiente con dueño y fecha).
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (rupturas documentadas con reemplazo) y §5 (el gate de compatibilidad de API).
- [`SPEC-261016-0044`](./SPEC-261016-0044-shell-wpf.md) — el shell sobre el que se empaqueta y se asocia (prerrequisito, Bolt 025).
- [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) · [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) — el motor y el CLI que viajan en el mismo payload (composición).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y lane de integración (prerrequisito, Bolt 001).
- [`US-000.BOLT-026`](../functional/bolts/US-000.BOLT-026-integracion-shell-motor.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el flujo de plan/reporte del shell y el documento de notas que recibe la ruptura (fronteras declaradas).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-027`: pre-SPEC evidence gate (§3), fases A–D, criterios AC-1…AC-10, forma de publicación (autocontenida por RID, MSIX opcional, sin ClickOnce) con registro de composición, asociación declarativa por instalador, inventario de lo retirado (asociación en runtime, metadata anterior, APIs muertas), rutas por la BCL, diagnóstico a archivo por usuario, funcionamiento sin asociación y la **exclusión explícita de la publicación** (canal, ADR-001 §3.6) declarada como riesgo con dueño y fecha; estrategia de pruebas con el **plan de integración `applicable`** (tests, entorno y datos). |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (infra / tooling de escritorio)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
