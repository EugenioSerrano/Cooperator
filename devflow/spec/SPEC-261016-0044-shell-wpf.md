---
id: "SPEC-261016-0044"
title: "Shell del Modeler (WPF): esqueleto que hospeda el motor, editor dirigido por el esquema y estado del usuario sin secretos"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 14 (hallazgos F-33, F-35 y F-38) → ADR-009
bolt: "US-000.BOLT-025" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-009", "ADR-003", "ADR-002", "ADR-004"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y build (Bolt 001)
  - "SPEC-261016-0023-motor-headless.md" # el motor y su punto de entrada tipado: lo que el shell hospeda en proceso (Bolt 017)
  - "SPEC-261016-0010-contrato-coop2-en-modeling.md" # el contrato del `.coop2` (lector/escritor único, esquema, derivados) que dirige el editor (Bolt 004)
  - "SPEC-261016-0004-particion-modeling.md" # el paquete `Modeling`, sus POCOs y el mapeo SQL↔CLR que el editor consume (Bolt 003)
risk_class: "medium" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L3" # default por riesgo low/medium (§3.3)
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "internal" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: required` → `applicable` (§3.2.2)
  applicability: "applicable"
  tests: # escritas en este V-Bounce; se ejecutan en el ciclo de integración del Bolt
    - "2.0/tests/Cooperator.Modeler.Integration.Tests/ModelRoundTripTests.cs (Category=Integration) — el shell abre el modelo de referencia, edita valores y guarda **a través del contrato**, con round-trip verificado, sin tocar ningún valor derivado y con el archivo producido **byte-compatible** con el que produce el CLI"
    - "2.0/tests/Cooperator.Modeler.Integration.Tests/EditorSchemaDrivenTests.cs (Category=Integration) — los campos, sus tipos, sus opciones y sus **valores enumerados reales** y el **mapeo de tipos** se leen del contrato en tiempo de ejecución: no existe una definición de campos en la interfaz y un valor fuera del conjunto enumerado no se puede cargar desde la UI"
    - "2.0/tests/Cooperator.Modeler.Integration.Tests/UserStateSecretFreeTests.cs (Category=Integration) — se opera el flujo (abrir, conectar, generar, cerrar) y se inspecciona `%APPDATA%\\<producto>\\settings.json` y el log del shell: **0** connection strings, valores protegidos o material de claves (NFR `shell-state-secret-free`)"
    - "2.0/tests/Cooperator.Modeler.Integration.Tests/NoRegistryWriteTests.cs (Category=Integration) — el shell no escribe HKCU/HKLM para su estado ni HKCR para la asociación (esa es del instalador, Bolt 027)"
    - "2.0/tests/Cooperator.Modeler.Integration.Tests/ModelIoResponsivenessTests.cs (Category=Integration) — abrir y guardar el modelo de referencia no bloquea el hilo de interfaz: **0** operaciones de E/S de modelo prolongada ejecutadas en ese hilo y la ventana responde (NFR `shell-ui-responsiveness`, tramo de E/S de modelo)"
  environment: "un entorno de escritorio **Windows** con un **perfil de usuario real** (el estado del usuario vive ahí) y .NET 10; entorno **aún no provisionado** (el repositorio no tiene pipeline). La corrida automatiza el arranque del shell en ese perfil."
  data: "el modelo de referencia `.coop2` del arnés (fixtures **sintéticos**, `internal`) y un **perfil de usuario designado y desechable** para el estado del shell; **sin datos de cliente** y sin credenciales: la inspección del estado busca material de conexión/clave y su resultado esperado es 0."
  reason: ""
  waiver_adr: ""
review_ready_at: "2026-10-16T00:48:41-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:48:41-03:00"
  decided_at: "2026-10-16T00:48:41-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-025 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «Fronteras 025/026/027 declaradas sin solapamiento»), ADR-009/ADR-003/ADR-002/ADR-004 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class medium → autonomy_level L3; data_classification internal; test_integration required → applicability applicable. Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: esqueleto WPF net10.0-windows sin dependencias por defecto + motor hosteado en proceso por el punto de entrada tipado (0 lógica propia, 0 Process); editor dirigido por el esquema (enums y mapeo leídos del contrato en runtime; 0 duplicación; derivados no editables; límite declarado vs PropertyGrid); lectura/escritura sólo por el contrato (byte-compatible con el CLI); estado en %APPDATA% con versión de forma, sin secretos, sin Registro; log por usuario; UseShellExecute explícito; APIs retiradas (0 Application.ExecutablePath/Assembly.Location/DoEvents/ClickOnce/WinForms)."
    - "ADJUDICACIÓN (a) criterio con dueño — CONFIRMADA: §B.2/AC-7/§15 #2 fijan que si el contrato del Bolt 004 no expone enums reales o el mapeo de tipos accesible, el fallo se reporta contra ese Bolt y jamás se parchea con una lista propia en la UI (duplicación prohibida por ADR-009 §3.2)."
    - "ADJUDICACIÓN (b) estado del usuario como capacidad nueva — CONFIRMADA: §2 (tabla de evidencia 1.4) y §14 constatan que el tool 1.4 no persistía estado (Settings.Designer.cs sin usos de Settings.Default, sin .settings ni user.config, F-38); el SPEC lo trata como requisito nuevo con política de forma propia y versión de archivo, no como migración. Encuadre correcto."
    - "ADJUDICACIÓN (c) reparto de shell-ui-responsiveness — CONFIRMADA: §9/AC-6 miden el tramo de E/S de modelo en este Bolt (0 operaciones de E/S prolongada en el hilo de UI) y §14/§6 declaran que el tramo de generación con cancelación es del Bolt 026 (NFR de ADR-009 §5). Sin solapamiento de medición."
    - "ADJUDICACIÓN (d) plan de integración applicable — CONFIRMADA como objetivo: 5 suites en Cooperator.Modeler.Integration.Tests + entorno de escritorio Windows con perfil de usuario designado y desechable + datos sintéticos (modelo .coop2 de referencia, internal); entorno aún NO provisionado (el repositorio no tiene pipeline). Es la dependencia externa declarada, no un bloqueo del revisor."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). El stack WPF, el shell como **host** del motor con el mismo punto de entrada que el CLI,
  el editor dirigido por el esquema del contrato, la validación con las reglas del motor y el estado del usuario en
  un archivo propio sin secretos están decididos en ADR-009 §3.1–§3.3 (+ §5). Este SPEC los ejecuta.
  ⚠️ FRONTERA (declarada, no resuelta por omisión): el flujo de **plan y reporte**, su ejecución asíncrona y
  cancelable y el **handoff al CLI** son del Bolt 026; el **empaquetado, la asociación y la integración con el
  sistema operativo** son del Bolt 027.
  ⚠️ CRITERIO CON DUEÑO (ADR-009 §3.2): que el contrato exponga **enums reales** y el **mapeo SQL↔CLR** accesible es
  criterio del Bolt 004; si falta, **este** Bolt no puede cumplir su criterio y el fallo se **reporta contra ese
  Bolt**, nunca se parchea en la interfaz.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de dominio
  aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada invalida la
  aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0044 — Shell del Modeler (WPF): esqueleto que hospeda el motor, editor dirigido por el esquema y estado del usuario sin secretos

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 14 (hallazgos F-33, F-35 y F-38) |
| **Bolt** | [US-000.BOLT-025](../functional/bolts/US-000.BOLT-025-shell-wpf.md) |
| **ADRs** | [ADR-009](../adrs/ADR-009-shell-del-modeler.md) §3.1, §3.2, §3.3, §5 · [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1, §3.4 · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.1, §3.4 · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) §3.4 |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega el **shell del Modeler**: el esqueleto de la aplicación que **hospeda el motor en proceso**, el
**editor del modelo dirigido por el contrato del formato** y el **estado del usuario** recordado en un archivo
propio **sin secretos**.

El problema que resuelve está verificado en el tool de 1.4: es una aplicación WinForms clásica
(`Program.cs`: `[STAThread]` + `Application.EnableVisualStyles()` + `Application.Run`, con el **único** punto de
automatización en el argumento de línea de comandos) cuya edición se apoyaba en el **`PropertyGrid`** —que
mostraba el modelo **porque las clases del modelo llevaban atributos de diseñador**—, que resolvía la **asociación
de la extensión escribiendo HKCR en runtime** (`FileExtensionAssociation.cs`: `Registry.ClassesRoot.CreateSubKey(".coop")`
y el comando con `Application.ExecutablePath`), y que traía un almacén de *settings* (`Properties/Settings.Designer.cs`:
`Settings : ApplicationSettingsBase`) cuyo modelo **no existe** en el runtime objetivo (F-38). Retargetear esa UI no
reproduce su experiencia: sin atributos de diseñador el `PropertyGrid` aparece casi vacío. Si este Bolt no se
implementa, el producto pierde su herramienta de edición, y hacerlo "portando pantallas" reproduciría el
acoplamiento UI↔modelo que el ciclo anterior desarmó.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 14 al **ADR-009** (`accepted`), que decide:
shell **WPF** sobre `net10.0-windows` **sin dependencias de terceros por defecto**; el shell es un **host** del
motor con **el mismo punto de entrada tipado que el CLI** (sin `Process` en el camino normal y sin una segunda
implementación de generación); la edición es un **editor dirigido por el esquema** cuya fuente de verdad es el
**contrato versionado** de `Modeling` (**cero duplicación del esquema en la UI**, derivados no editables, lectura y
escritura **sólo** por el contrato, validación con **las reglas del motor**); el estado del usuario es un
**`settings.json` por usuario** en los datos del usuario (**nada de Registro**, **ningún secreto**); y la
**fidelidad objetivo** es "cubre lo que el esquema declara editable", **no** la paridad con el `PropertyGrid`
del 1.4 (límite declarado).

**Lo que el tool 1.4 hacía (evidencia verificada en esta sesión):**

| Aspecto | Evidencia | Qué implica para el shell 2.0 |
|---|---|---|
| Arranque clásico y único punto de automatización | `Program.cs`: `[STAThread]`, `Application.EnableVisualStyles()`, `MainForm form = new MainForm(); form.args = args; Application.Run(form);` | El shell 2.0 arranca por WPF y **sigue aceptando un modelo por argumento** como comodidad; la automatización de verdad es el CLI (Bolt 018) |
| Edición acoplada a atributos de diseñador | `MainForm.cs:161-163` (`treeView1_AfterSelect` → `propertyGrid1.SelectedObject = ((TreeNode)e.Node).Tag`), `MainForm.cs:115-154` (`RecreateTreeView`/`PopulateNode`) | El editor debe salir del **contrato**, no de atributos: el `PropertyGrid` no tiene de dónde leer su experiencia (F-27/F-28) |
| Asociación de la extensión en **HKCR** desde la aplicación | `FileExtensionAssociation.cs`: `Registry.ClassesRoot.CreateSubKey(".coop")`, `SetValue(null, "coopmodelerfile")` y el comando con `System.Windows.Forms.Application.ExecutablePath` (F-35) | **Se elimina**: la asociación la aporta el instalador (Bolt 027); el shell funciona sin asociación |
| Estado/Settings con `ApplicationSettingsBase` | `Properties/Settings.Designer.cs`: `internal sealed partial class Settings : ApplicationSettingsBase` (F-38); **sin** archivo `.settings` ni usos de `Settings.Default` en el código del tool | **No había estado de usuario persistido**: el estado del shell es **capacidad nueva** (no una migración) y su requisito es el NFR `shell-state-secret-free` |
| Rutas por API de WinForms / `Assembly.Location` | `About.cs:56`, `CodeGenerator.cs:141,162,272,288,359,460,503` usan `Library.IO.Path.AppPath(true)`; ADR-009 §3.4 cita `Assembly.Location` (F-25) | Las rutas van por **BCL** (`Environment.ProcessPath`, `AppContext.BaseDirectory`) |
| `Process.Start` sin declarar el *shell* | `GenerationDialog.cs:176,207`, `About.cs:156` | Se conserva lo portable (abrir la solución/carpeta generada, "acerca de") con `UseShellExecute = true` **explícito** (ADR-009 §3.5) |

**Fronteras declaradas (el DoR del Bolt las exige):** el **flujo de plan y reporte con su ejecución asíncrona y
cancelable y el handoff al CLI** son del **Bolt 026**; el **empaquetado, la asociación y la integración con el
sistema operativo** son del **Bolt 027**. Este Bolt entrega el esqueleto que hospeda el motor, el editor y el
estado.

**Estado del árbol 2.0.** `2.0/` está siendo construido por el V-Bounce del Bolt 001 (arnés y comando reproducible);
el motor (Bolt 017) y el contrato del formato (Bolt 004) ya tienen SPEC en disco. Este Bolt escribe **sólo** su
SPEC y, en su V-Bounce, el proyecto del shell.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-009 cierra el slot de ADR-001 §3.6 y respeta ADR-002/ADR-003/
ADR-004: no reabre el formato, el motor ni la configuración), y el plan de integración declarado (`applicable`)
coincide con el `test_integration: required` del Bolt (§3.2.2). No se emite reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-025` (`devflow/functional/bolts/US-000.BOLT-025-shell-wpf.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:12:35-03:00; `risk_class: medium`, `test_integration: required` (app de escritorio: comportamiento, estado por usuario y respuesta se demuestran ejecutándose), `data_classification: internal`; fronteras 025/026/027 declaradas sin solapamiento |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-009` §3.1 (stack y arquitectura: host del motor, mismo punto de entrada que el CLI, flujo, asincronía, handoff), §3.2 (edición dirigida por el esquema y el **criterio con dueño** del contrato), §3.3 (estado del usuario: qué guarda y qué **nunca**) y §5 (NFRs `shell-ui-responsiveness`, `shell-state-secret-free` y sus mediciones) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-003` §3.1 (punto de entrada único y tipado; el Modeler como host) y §3.4 (convivencia: los avisos de deriva que el shell muestra) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T22:32:47-03:00 |
| ADR (gobernante) | `ADR-002` §3.1 (los derivados se recalculan al leer) y §3.4 (`Modeling` como **único** lector/escritor) · `ADR-004` §3.4 (la configuración del shell sigue el modelo de una app generada) | AITL-ADR-Approval ✓ — `accepted` (2026-10-15) |
| REV evidence | `REV-001` §4 F-33, F-35, F-38 y §6 ítem 14 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0000-…` (arnés/build), `SPEC-261016-0023-…` (motor), `SPEC-261016-0010-…` (contrato `.coop2`) y `SPEC-261016-0004-…` (partición y mapeo) | Los cuatro **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otro V-Bounce; **este** Bolt no lo toca) |

---

## 4. Scope

### In scope

1. **El esqueleto del shell** sobre su stack y su *target* (`net10.0-windows`, WPF), **sin dependencias de terceros
   por defecto** (binding de WPF + *view-models* propios alcanzan; si un Bolt concluyera que necesita un *toolkit*
   MVVM o un control de terceros, entra con las reglas de ADR-001 §3.4 y se registra: no cambia el stack).
2. **El motor hosteado en proceso** por su **punto de entrada tipado** —el mismo que usa el CLI—: **cero** lógica
   de generación en la interfaz, **cero** `Process` intermedio en el camino normal y **cero** segunda
   implementación de generación.
3. **El editor dirigido por el contrato del formato**: árbol del modelo (entidades → propiedades, consultas,
   relaciones) más panel de detalle del nodo; los **campos, sus tipos, sus opciones y sus valores enumerados
   reales** se leen del contrato **en tiempo de ejecución**; el **tipo nativo y el tipo CLR** de una propiedad se
   ofrecen y se validan desde el **mapeo de tipos del contrato**; **ningún** valor derivado es editable (se
   recalculan al leer); **cero** duplicación del esquema en la interfaz.
4. **La lectura y la escritura del modelo exclusivamente por el contrato** (el shell **no** interpreta ni escribe
   el documento por su cuenta): el archivo que produce el shell es **byte-compatible** con el que produce el CLI.
5. **La validación en vivo con las reglas del motor** (claves primarias que no resuelven, `childOf` fuera de
   candidatos, colisiones de nombre generado): la interfaz muestra esas validaciones y **no** inventa mensajes
   paralelos.
6. **El estado del usuario en un archivo propio** dentro de los datos del usuario (`%APPDATA%\<producto>\settings.json`,
   UTF-8 sin BOM), leído y escrito **por el shell**: qué guarda (estado de UI, último modelo y recientes, carpeta de
   plantillas propias, preferencias de generación, rótulo del último servidor/entorno si se decide ofrecerlo) y qué
   **nunca** guarda (**ningún secreto**). Sin `ApplicationSettingsBase`, sin `user.config` y sin **Registro** para el
   estado de la aplicación.
7. **El diagnóstico del shell**: los errores se muestran en la interfaz **y** se registran en un archivo de log por
   usuario (`%LOCALAPPDATA%`), con la abstracción de logging que reemplazó al Event Log (el shell **nunca** escribe
   en él) y **sin secretos**.
8. **Las rutas por BCL** (`Environment.ProcessPath`, `AppContext.BaseDirectory`) y la eliminación de
   `Application.ExecutablePath`/`Assembly.Location` en el tooling.
9. **La constancia de las rupturas y de las diferencias declaradas** frente al tool 1.4 (ADR-008 §3.1), como
   insumo de las notas de migración (Bolt 028).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **flujo de plan y reporte**, su ejecución **asíncrona y cancelable** y el **handoff al CLI** | **Bolt 026** | El DoR lo declara: este Bolt deja el motor hosteado y consumible, pero no orquesta ese flujo |
| El **empaquetado**, la **asociación** de la extensión y la integración con el sistema operativo y sus rutas | **Bolt 027** | La asociación la aporta el instalador, **nunca** la aplicación (ADR-009 §3.4) |
| El **motor**, su proyección y su punto de entrada | Bolt 017 (SPEC-0023) | Se **consume**; no se implementa ni se modifica |
| El **CLI** y su vocabulario de plan/reporte/códigos | Bolt 018 (SPEC-0025) | Otra superficie; el shell coincide con él y le hace handoff (Bolt 026) |
| El **contrato del formato** y su lector/escritor | Bolt 004 (SPEC-0010) | `Modeling` es el **único** lector/escritor (ADR-002 §3.4) |
| El **almacén de configuración de usuario del framework** | Bolt 010 (SPEC-0035 declara su frontera) | El estado del shell es **otro** archivo, del shell, sin secretos |
| La **configuración del framework** y la conexión al motor de base | Bolts 011 / 014 | Otras superficies |
| El **enumerador de servidores** (F-34) | DISC pendiente | ADR-009 §3.7: el diseño **no depende** de esa respuesta; la entrada manual es la línea base |
| La **paridad con la rejilla de propiedades del 1.4** | — (nadie) | ADR-009 §3.2: la fidelidad objetivo es "lo que el esquema declara editable" y debe documentarse la diferencia |

**Regla de aporte (bounded, explícita).** Este Bolt **crea** el proyecto del shell y sus archivos. No implementa
generación, no escribe el formato, no define reglas de validación propias y no toca el motor, el CLI, el contrato
ni el almacén del framework. Si algo de eso hiciera falta, se **detiene** y se reporta contra el Bolt dueño.

---

## 5. Prerequisites and baseline

- **Bolt 017 y su SPEC** (`SPEC-261016-0023-…`): el motor, su **punto de entrada tipado** (petición → resultado),
  su contexto endurecido y la regla de propiedad de archivos (cuyos avisos de deriva el shell muestra, ADR-003 §3.4).
- **Bolt 004 y su SPEC** (`SPEC-261016-0010-…`): el **contrato del `.coop2`** —lector/escritor único, esquema,
  política de `schemaVersion`, recálculo de derivados— y, con él, el **criterio con dueño** de ADR-009 §3.2: que el
  contrato exponga **enums reales** y el **mapeo de tipos** accesible.
- **Bolt 003 y su SPEC** (`SPEC-261016-0004-…`): el paquete `Modeling`, sus **POCOs** y la **fuente única** del
  mapeo SQL↔CLR.
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el build y el arnés.
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error.
- **Estado del árbol 2.0:** en construcción por el V-Bounce del Bolt 001; este Bolt escribe su SPEC y, **en su**
  V-Bounce, el proyecto del shell.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline antes y después de construir.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-025`), **2 V-Bounces** esperados más el ciclo de integración. El orden va del
> esqueleto al editor y al estado: primero el host del motor, después la edición dirigida por el esquema, después la
> escritura por el contrato y la validación, y al final el estado sin secretos y el diagnóstico.

### Phase A — El esqueleto y el motor hosteado en proceso

**Duration:** ~3h de ciclo — **Complexity:** Medium

#### A.1 Proyecto y stack

Se crea el proyecto del shell como aplicación WPF `net10.0-windows` (un solo TFM, sin paquete de terceros por
defecto), con las rutas resueltas por **BCL** (`Environment.ProcessPath`, `AppContext.BaseDirectory`) y sin las
API que el tooling retira (`Application.ExecutablePath`, `Assembly.Location`, `EnableVisualStyles`,
`Application.DoEvents`, `System.Deployment`/ClickOnce, `System.Windows.Forms`).

#### A.2 El motor, en proceso y por su punto de entrada

El shell referencia el motor y lo consume por su **punto de entrada tipado** —el mismo que usa el CLI—: no hay
`Process` intermedio en el camino normal, no hay lógica de generación en la interfaz y no existe una segunda
implementación. El shell arranca con un modelo por argumento como **comodidad** (el único punto de automatización
del 1.4), pero la automatización de verdad es el CLI (Bolt 018); el shell funciona **sin** asociación de archivos.

#### A.3 La configuración del shell

La configuración del shell sigue el modelo de ADR-004 §3.4 (`appsettings.json` + variables de entorno, precedencia
entorno > archivo): el shell la **arma** y la pasa al motor y a `Modeling` cuando haga falta, igual que una
aplicación generada. El shell no lee ni escribe la configuración de nadie más.

**Files created:**
- `2.0/src/CooperatorModeler/CooperatorModeler.csproj` — el proyecto del shell (`net10.0-windows`, WPF)
- `2.0/src/CooperatorModeler/App.xaml` · `App.xaml.cs` — arranque, argumentos y composición
- `2.0/src/CooperatorModeler/Services/GenerationHost.cs` — el motor hosteado por su punto de entrada único
- `2.0/src/CooperatorModeler/Services/ShellConfiguration.cs` — la configuración del shell (ADR-004 §3.4)

---

### Phase B — El editor dirigido por el esquema

**Duration:** ~5h de ciclo — **Complexity:** High

#### B.1 La forma y la fuente de verdad

El editor presenta el modelo como **árbol más panel de detalle** del nodo seleccionado. La lista de campos, sus
tipos, sus opciones y sus **valores enumerados reales** se leen del **contrato** en tiempo de ejecución: **ninguna**
definición de campos vive en la interfaz. Es la regla dura de ADR-009 §3.2: si el editor *hardcodea* un campo o un
rango, queda desincronizado en la próxima subida de `schemaVersion` y el shell **miente** sobre lo que el formato
admite.

#### B.2 Los valores enumerados y el mapeo de tipos, del contrato

Los campos enumerados se ofrecen como **valores enumerados reales** (no texto libre) y el **tipo nativo y el tipo
CLR** de una propiedad se ofrecen y se validan desde el **mapeo de tipos del contrato**. Es el **criterio con dueño
del Bolt 004** (ADR-009 §3.2): si el contrato no expone los enums reales o el mapeo accesible, **este** Bolt **no**
puede cumplir su criterio y el fallo se **reporta contra ese Bolt** — jamás se parchea con una lista propia en la
interfaz.

#### B.3 Los derivados no se editan

Los valores derivados (nombres generados, espacios de nombres, predicados, conteos) **no** son editables: se
recalculan al leer (ADR-002 §3.1) y el editor los muestra como información. Un campo que el esquema no declara no
se puede inventar desde la interfaz.

#### B.4 El límite declarado

La fidelidad objetivo es **"cubre lo que el esquema declara editable"**, no la paridad con la rejilla de
propiedades del diseñador de 1.4 (categorías, atributos, editores por tipo). La documentación del tool **dice** esa
diferencia; el SPEC no la disimula.

**Files created:**
- `2.0/src/CooperatorModeler/Editing/ModelTreeViewModel.cs` — el árbol del modelo desde el contrato
- `2.0/src/CooperatorModeler/Editing/NodeDetailViewModel.cs` — el detalle del nodo, sus campos, tipos y opciones
- `2.0/src/CooperatorModeler/Editing/SchemaDescriptor.cs` — la lectura del esquema del contrato en tiempo de ejecución
- `2.0/src/CooperatorModeler/Editing/TypeMappingAdapter.cs` — los tipos ofrecidos desde el mapeo del contrato
- `2.0/tests/Cooperator.Modeler.Tests/SchemaDrivenEditorTests.cs` — 0 definiciones de campos en la UI; enums y mapeo del contrato; derivados no editables

---

### Phase C — Lectura y escritura por el contrato, y validación con las reglas del motor

**Duration:** ~3h de ciclo — **Complexity:** High

#### C.1 Un solo lector y un solo escritor

La lectura y la escritura del modelo pasan **exclusivamente** por el lector/escritor del contrato: el shell **no**
parsea ni escribe el documento por su cuenta, ni guarda "por partes". La consecuencia verificable es que el archivo
producido por el shell es **byte-compatible** con el que produce el CLI sobre el mismo modelo (y con el que
verifica el test bilateral del Bolt 004).

#### C.2 La validación en vivo, sin reglas propias

Lo que la interfaz muestra temprano —claves primarias que no resuelven, `childOf` fuera de candidatos, colisiones
de nombre generado— son **las validaciones del motor**, compartidas con el flujo del Bolt 026: no se duplican
reglas ni se inventan mensajes paralelos. El shell **no** define reglas de validación.

#### C.3 Los avisos de deriva, visibles

Los *warnings* que el motor produce sobre artefactos editables (contenido que difiere sin sobrescribir, ADR-003
§3.4) se muestran en el shell: el usuario los ve **antes** de decidir forzar, y esa decisión es explícita (el
detalle del flujo es del Bolt 026).

**Files created:**
- `2.0/src/CooperatorModeler/Editing/ModelDocument.cs` — el documento del modelo a través del contrato
- `2.0/src/CooperatorModeler/Editing/LiveValidation.cs` — la validación del motor, expuesta a la UI
- `2.0/tests/Cooperator.Modeler.Tests/RoundTripThroughContractTests.cs` — el guardado pasa por el contrato (byte-compatible con el CLI)
- `2.0/tests/Cooperator.Modeler.Tests/NoDuplicatedValidationTests.cs` — las reglas son las del motor

---

### Phase D — El estado del usuario, sin secretos

**Duration:** ~2,5h de ciclo — **Complexity:** Medium

#### D.1 El archivo y su contenido

El estado vive en **`%APPDATA%\<producto>\settings.json`** (UTF-8 sin BOM), leído y escrito **por el shell** (ni por
el framework ni por las librerías): estado de UI (tamaño/posición, columnas), último modelo y **lista de
recientes**, carpeta de plantillas *override*, preferencias de generación y el **rótulo** del último
servidor/entorno si se decide ofrecerlo como comodidad.

#### D.2 Lo que **nunca** guarda

**Ningún secreto** (NFR `shell-state-secret-free`): ni cadenas de conexión, ni valores protegidos, ni material de
claves. La cadena de conexión se resuelve por la **configuración portable** (ADR-004 §3.2) o por un valor
protegido (ADR-007), y el shell la pide **en memoria** cuando hace falta conectarse; si el usuario quiere
recordarla, la recuerda el **almacén de configuración**, no el archivo de estado de la interfaz. El **log** del
shell (§6/E) tiene la misma regla.

#### D.3 Nada de Registro, y el archivo viejo

El shell **no** escribe HKCU/HKLM para sus preferencias (ni HKCR: la asociación es del instalador, Bolt 027). El
formato del archivo de estado está **versionado por el propio archivo**: un cambio de forma **maneja** el archivo
viejo o lo **descarta explícitamente** (ADR-009 §4 lo declara como deuda del shell y este SPEC lo convierte en
requisito del archivo).

**Files created:**
- `2.0/src/CooperatorModeler/State/UserStateStore.cs` — lectura/escritura del `settings.json` por usuario, con versión de forma
- `2.0/src/CooperatorModeler/State/UserState.cs` — qué persiste el shell (y qué no)
- `2.0/tests/Cooperator.Modeler.Tests/UserStateTests.cs` — contenido sin secretos, versión de forma y descarte explícito del archivo viejo

---

### Phase E — Diagnóstico, integración portable con el sistema y cierre

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### E.1 Diagnóstico

Los errores se muestran en el shell **y** se registran en un archivo de log por usuario (`%LOCALAPPDATA%`) a través
de la abstracción de logging que reemplazó al Event Log: el shell **nunca** escribe en el Event Log (ADR-001 §3.5) y
el log **no** contiene secretos. La política de tamaño/rotación del archivo forma parte del entregable.

#### E.2 Lo portable que se conserva

Abrir la **solución o carpeta generada** y el enlace de «acerca de» se conservan con `Process.Start` y
**`UseShellExecute = true` explícito** (en el runtime objetivo el *default* es `false` y sin declararlo el
comportamiento cambia): sin asumir Visual Studio, se abre con la asociación del sistema operativo y, si no hay
ninguna, el shell lo **dice**.

#### E.3 Constancia de las rupturas y de las diferencias declaradas

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-025.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1 para lo que cambia respecto del tool 1.4: la **UI es nueva** (no un retarget: no hay paridad con el
`PropertyGrid`), el estado deja de tener expectativa de `user.config` y vive en un JSON propio, la **asociación**
deja de ser una acción de la aplicación, y los **puntos de automatización** por argumento siguen existiendo como
comodidad mientras el CLI es el camino soportado.

**Files created:**
- `2.0/src/CooperatorModeler/Diagnostics/ShellLog.cs` — el log por usuario, con su política de rotación
- `2.0/src/CooperatorModeler/ShellIntegration.cs` — apertura de la solución/carpeta y «acerca de» con `UseShellExecute` explícito
- `2.0/docs/api-removals/US-000.BOLT-025.json` — constancia de rupturas y diferencias declaradas (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-009/ADR-003/ADR-002/ADR-004, expresados como Given/When/Then verificables.

### AC-1: El shell existe y hospeda el motor sin reimplementarlo

**Given** el proyecto del shell sobre su stack y su *target*
**When** se revisan sus referencias y su camino de generación
**Then** referencia el motor **en proceso** y lo consume por su **punto de entrada tipado** (el mismo que el CLI),
sin lógica de generación en la interfaz, sin `Process` en el camino normal y sin una segunda implementación.

### AC-2: El editor es dirigido por el esquema, sin duplicarlo

**Given** un modelo abierto y el contrato del formato
**When** se inspeccionan los campos del editor
**Then** la lista de campos, sus tipos, sus opciones y sus **valores enumerados reales** provienen del contrato **en
tiempo de ejecución**, no existe ninguna definición de campos en la interfaz, los tipos nativo/CLR se ofrecen desde
el **mapeo del contrato**, y un valor fuera del conjunto enumerado no se puede cargar desde la UI.

### AC-3: La lectura y la escritura son del contrato

**Given** el modelo editado en el shell
**When** se guarda
**Then** el archivo se produce a través del **lector/escritor del contrato**, es **byte-compatible** con el que
produce el CLI sobre el mismo modelo, y **ningún** valor derivado fue editado ni modificado.

### AC-4: La validación es la del motor

**Given** un modelo con una clave primaria que no resuelve, un `childOf` fuera de candidatos o una colisión de
nombre generado
**When** el shell lo muestra temprano
**Then** el mensaje es el **del motor** y **no** existe una regla ni un mensaje paralelo en la interfaz.

### AC-5: El estado del usuario está en su archivo y no contiene secretos

**Given** una sesión que abre un modelo, se conecta, genera y cierra
**When** se inspecciona `%APPDATA%\<producto>\settings.json` y el log del shell
**Then** el estado está en ese archivo, **0** secretos aparecen en él (ni cadenas de conexión, ni valores
protegidos, ni material de claves) y la aplicación **no** escribió en el Registro para su estado.

### AC-6: La E/S de modelo no bloquea la interfaz

**Given** el modelo de referencia
**When** se abre y se guarda
**Then** **0** operaciones de E/S de modelo prolongada corren en el hilo de interfaz y la ventana responde
(NFR `shell-ui-responsiveness`, tramo de E/S de modelo; el tramo de generación es del Bolt 026).

### AC-7: El contrato cumple su parte (criterio con dueño)

**Given** el contrato del formato (Bolt 004)
**When** el editor necesita los enums reales y el mapeo de tipos
**Then** los obtiene de él; si no los expusiera, el fallo se **reporta contra el Bolt 004** y **no** se parchea con
una lista propia en la interfaz.

### AC-8: Frontera respetada

**Given** los Bolts 026 (flujo, asincronía, handoff) y 027 (empaquetado, asociación), el motor, el CLI, el contrato
y el almacén del framework
**When** se revisa el cambio
**Then** ninguno de sus ámbitos fue invadido: no hay flujo de plan/reporte ni handoff implementados acá, no hay
empaquetado ni asociación, y el shell no parsea el formato ni define validaciones propias.

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde (con los casos de
integración filtrados por categoría) y `git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están
vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "el shell… referenciando el motor **en proceso** y usando el **mismo punto de entrada tipado**… sin lógica de generación" | Fase A: host del motor por su punto de entrada | AC-1 · revisión de referencias + `GenerationHost` |
| "el editor presenta el modelo como árbol más detalle… con los campos, sus tipos, sus opciones y sus **valores enumerados reales** tomados del contrato… **cero** duplicación del esquema" | Fase B: editor dirigido por el esquema | AC-2 · `SchemaDrivenEditorTests` + integración |
| "el tipo nativo y el tipo CLR… desde el **mapeo de tipos del contrato**" | Fase B.2: `TypeMappingAdapter` sobre el contrato | AC-2, AC-7 · integración |
| "la lectura y la escritura del modelo pasan **exclusivamente** por el lector/escritor del contrato… **ningún valor derivado** es editable" | Fase C.1 + B.3 | AC-3 · `ModelRoundTripTests` (byte-compatible con el CLI) |
| "la validación en vivo usa **las mismas reglas** que el motor y no inventa mensajes paralelos" | Fase C.2 | AC-4 · `NoDuplicatedValidationTests` |
| "el estado por usuario vive en un archivo propio dentro de los datos del usuario y **no contiene ningún secreto**… la aplicación no escribe en el registro del sistema" | Fase D | AC-5 · `UserStateSecretFreeTests`, `NoRegistryWriteTests` |
| "el usuario puede abrir el modelo sin que la aplicación escriba en el registro del sistema" | Fases A.1 y D.3 | AC-5 · integración |
| "la entrada propia de las rupturas que este Bolt introduce, registrada en las notas de migración" | Fase E.3 | AC-9 · `api-removals/US-000.BOLT-025.json` |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Editor dirigido por el esquema (`SchemaDrivenEditorTests`):** sobre los *view-models* (sin abrir ventanas): la
  lista de campos y sus opciones salen del contrato; 0 definiciones de campos en la UI; los enums se ofrecen como
  valores enumerados; el mapeo de tipos se consulta al contrato; los derivados no son editables.
- **Round-trip por el contrato (`RoundTripThroughContractTests`):** cargar → modificar valores → guardar produce un
  documento que el contrato vuelve a leer; el shell no serializa por su cuenta (control de ausencia de
  `JsonSerializer`/`JsonDocument` en el proyecto del shell).
- **Validación (`NoDuplicatedValidationTests`):** los mensajes que la UI muestra son los del motor; 0 reglas propias.
- **Estado (`UserStateTests`):** el `settings.json` se escribe en la ubicación del usuario con UTF-8 sin BOM; el
  contenido declarado es el esperado; **0** secretos; el archivo viejo de otra versión se maneja o se descarta
  explícitamente.
- **Diagnóstico:** el log por usuario se escribe con su rotación y sin secretos.
- **Rutas y APIs retiradas:** 0 usos de `Application.ExecutablePath`, `Assembly.Location`, `EnableVisualStyles`,
  `Application.DoEvents`, `System.Deployment`/ClickOnce y `System.Windows.Forms` en el shell.
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** modelo sin entidades; entidad sin propiedades; propiedad con tipo nativo fuera del conjunto del
  mapeo (se reporta contra el contrato, no se inventa); campo opcional ausente; archivo de estado de una versión
  anterior; permiso de escritura denegado en `%APPDATA%`; modelo ilegible (el contrato falla y el shell lo muestra);
  ventana con estado guardado fuera de pantalla (posición inválida al restaurar).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

- **Integration tests (real dependencies):** los cinco archivos de
  `2.0/tests/Cooperator.Modeler.Integration.Tests/` (frontmatter), `Category=Integration`, ejecutados en el ciclo de
  integración del Bolt sobre un **entorno de escritorio Windows con un perfil de usuario real**: el round-trip por
  el contrato con byte-compatibilidad frente al CLI, el editor dirigido por el esquema, el estado sin secretos, la
  ausencia de escritura en el Registro y la respuesta ante la E/S del modelo.
- **Contract / E2E against the real providers:** el "proveedor" del shell es el **contrato del formato** (y, a
  través de él, el documento): lo que se verifica end-to-end es que el shell **no** altera el documento (byte
  a byte frente al CLI) y que el contrato **sí** le entrega lo que el editor necesita (enums y mapeo).
- **Edge cases:** perfil de usuario **sin** estado previo; perfil con estado de una versión anterior; `%APPDATA%` de
  sólo lectura; modelo de referencia grande (E/S prolongada) con la ventana respondiendo; un modelo de otra
  `schemaVersion` (el contrato rechaza y el shell lo muestra).
- **Manejo de datos:** modelo de referencia **sintético** (`internal`); el perfil de usuario es **designado y
  desechable**; la inspección del estado busca material de conexión/clave y su resultado esperado es **0**; ninguna
  corrida usa un perfil de trabajo real.
- **BUG evidence:** `n/a` — no hay defecto en corrección.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (V-Bounce) y en
> `integration_test_runs[]` del manifiesto (ciclo de integración) (§3.6, §3.12).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El shell compila en `net10.0-windows` con advertencias tratadas como error | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (editor, round-trip por el contrato, validación, estado, diagnóstico) | `pass` — esperado (AC-1…AC-4) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del formato se ejerce desde los *view-models*: el editor no depende de la UI para verificar su fidelidad | `pass` — esperado (AC-2, AC-3) |
| SAST | V-Bounce | El shell lee/escribe el modelo y abre archivos/carpetas: se revisa el manejo de rutas y la ausencia de APIs retiradas | `pass` — esperado (§8.1) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias de terceros por defecto; si un *toolkit* entrara, con licencia permisiva, versión fijada y centralizada (ADR-001 §3.4) | `pass` — esperado (§6/A.1) |
| OWASP Top 10 — static review | V-Bounce | Sin superficie de red ni servicio; la apertura de archivos/carpetas usa la asociación del sistema operativo | `n/a` — sin superficie alcanzable desde fuera del proceso |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | **0** secretos en el estado y en el log del shell (NFR `shell-state-secret-free`) | `pass` — esperado (AC-5) |
| Hallucination lint | V-Bounce | Tipos, miembros y rutas referenciados existen (o se declaran como aportados) | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `data_classification: internal`; fixtures sintéticos; el estado del shell no contiene datos de negocio | `pass` — esperado |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos; versiones centralizadas si entrara alguno | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; el contrato del formato y el motor existen **antes** (Bolts 003/004/017) | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas de la suite; el round-trip es determinista | `pass` — esperado |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| Shell UI responsiveness (ADR-009 §5) | V-Bounce + integración | **0** operaciones de E/S de modelo prolongada en el hilo de UI; la ventana responde (el tramo de generación se mide en el Bolt 026) | `pass` — esperado (AC-6, medido en el ciclo) |
| Shell state secret-free (ADR-009 §5) | V-Bounce + integración | **0** conexiones/valores protegidos/claves en el estado y en el log | `pass` — esperado (AC-5, medido en el ciclo) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | El shell es una **aplicación nueva**: no retira superficie de los paquetes del framework | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | El shell abre, edita y guarda por el contrato en un perfil real, sin secretos en su estado y sin escribir en el Registro | `pass` — **en el ciclo de integración (entorno por provisionar)** |
| Contract / E2E against the real providers | Integration test | El documento producido por el shell coincide **byte a byte** con el del CLI sobre el mismo modelo | `pass` — **en el ciclo de integración** |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable de red |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint; el costo de generación lo mide el Bolt 017 | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | La observabilidad del shell es su log por usuario y los mensajes del motor (§11) | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: required` → `integration_test_plan.applicability: applicable`
> con sus tests, su entorno y sus datos.

---

## 10. Security and data

- **El shell no crea superficie de red ni servicio:** es una aplicación de escritorio que lee y escribe **un**
  archivo de modelo a través del contrato, y su integración con el sistema se limita a abrir la solución/carpeta
  generada y un enlace informativo (con `UseShellExecute = true` explícito).
- **NFR `shell-state-secret-free`:** el estado persistido y el log **no** contienen credenciales ni material de
  claves; la cadena de conexión se resuelve por la configuración portable o por un valor protegido y se pide **en
  memoria**. Este es el control que evita que una comodidad de UI se convierta en una fuga de datos del cliente.
- **Nada de Registro:** el shell no escribe HKCU/HKLM para su estado (ni HKCR: la asociación es del instalador) —
  menos superficie, menos privilegios y ninguna sorpresa en la máquina del usuario.
- **Diagnóstico sin secretos:** el log por usuario registra el fallo y su causa, nunca el valor de una conexión ni
  material de claves; su rotación evita que crezca sin límite.
- **Clasificación:** `data_classification: internal` (espeja el Bolt). El modelo describe **estructura**, no datos
  de negocio; los fixtures son sintéticos y el perfil de la corrida es designado y desechable.
- **Integridad del árbol congelado:** `1.4/**` no se modifica: el tool legacy es la evidencia del punto de partida.

---

## 11. Monitoring and observability

- **La observabilidad del shell es su log por usuario** (`%LOCALAPPDATA%`) más los mensajes del motor que la
  interfaz muestra: un fallo de generación o de lectura del modelo queda diagnosticable **sin** pedirle al usuario
  que reproduzca la sesión (ADR-009 §3.5).
- **Evidencia versionada:** la constancia de rupturas y diferencias declaradas
  (`2.0/docs/api-removals/US-000.BOLT-025.json`) y las salidas de la suite y del ciclo de integración en
  `2.0/artifacts/<timestamp>/` y en `integration_test_runs[]`.
- **Sin telemetría de servicio:** el shell no envía nada a ninguna parte (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration (del usuario del tool):** el shell es una **UI nueva**, no un retarget. Las diferencias frente al
  tool 1.4 se **declaran** y se documentan con el tool: no hay paridad con la rejilla de propiedades del diseñador
  (categorías, editores por tipo), la asociación de la extensión la aporta el instalador (Bolt 027) y el estado vive
  en un JSON propio en lugar de la expectativa de `user.config`. El modelo **migra** con el migrador (Bolt 002) y el
  editor lo abre a través del contrato; el shell funciona **sin** asociación (diálogo de archivos o argumento).
- **Compatibility:** la aplicación es **nueva**: no retira superficie de los paquetes del framework (el gate de API
  no la alcanza). Lo que cambia para el usuario son las diferencias declaradas de §12 y Fase E.3.
- **Rollback:** revertir el commit del Bolt. Un rollback parcial —por ejemplo, conservar el editor pero volver a un
  estado en Registro— es inválido: reintroduciría justo lo que el ADR retira (F-38 y la escritura en el Registro).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Duplicar el esquema en la interfaz** (la UI miente en la próxima subida de `schemaVersion`) | 3 | 4 | Los campos y sus opciones se leen del contrato **en tiempo de ejecución** y la ausencia de definiciones en la UI se verifica (AC-2) |
| **Que los campos enumerados degeneren en texto libre** | 2 | 3 | Criterio con dueño en el Bolt 004 (enums reales + mapeo accesible): su consumo se demuestra acá y su ausencia se **reporta contra ese Bolt** (AC-7) |
| **Un segundo lector/escritor del formato** | 2 | 4 | Sólo el contrato lee y escribe + verificación de byte-compatibilidad con la salida del CLI (AC-3) |
| **Secretos en el estado del usuario o en el log** | 2 | 5 | NFR `shell-state-secret-free` verificado por inspección del archivo y del log tras operar el flujo (AC-5) |
| **Reglas de validación duplicadas** y mensajes divergentes | 2 | 3 | La interfaz muestra las validaciones **del motor** y no inventa mensajes paralelos (AC-4) |
| **Interfaz congelada** por una operación larga (el legacy usaba `Application.DoEvents()`) | 2 | 3 | La E/S de modelo va fuera del hilo de UI (AC-6) y el flujo de generación, con su cancelación, en el Bolt 026 (NFR medido) |
| **Trabajo que no entra en un Bolt** (solapamiento con 026/027) | 2 | 3 | La frontera está declarada en el DoR y en §4; se resuelve por contrato de entregables, no por omisión |
| **La UI nueva se lee como regresión** frente al tool viejo | 3 | 2 | El límite se declara y **se documenta** con el tool (ADR-009 §3.2/§4): la fidelidad objetivo es lo que el esquema declara editable |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| El shell **hospeda** el motor por su punto de entrada tipado, sin `Process` en el camino normal | ADR-009 §3.1 y ADR-003 §3.6: una sola implementación de generación; el Modeler no puede divergir del CLI |
| El editor **no** define campos ni listas: todo sale del contrato en tiempo de ejecución | Es la regla dura de ADR-009 §3.2; cualquier definición en la UI queda desincronizada en la próxima subida de `schemaVersion` |
| Los **derivados** se muestran como información y no se editan | ADR-002 §3.1: los derivados se recalculan al leer; editarlos crearía un dato que el formato no persiste |
| La **validación** es la del motor, nunca una propia | ADR-009 §3.2: reglas duplicadas divergen y hacen que la UI mienta sobre lo que el motor aceptará |
| El **estado** es un JSON propio del shell, con versión de forma, en los datos del usuario | ADR-009 §3.3: `ApplicationSettingsBase` no existe en el runtime objetivo y el estado del shell no es el almacén de configuración del framework (Bolt 010) |
| El archivo de estado **maneja o descarta explícitamente** el formato anterior | ADR-009 §4 declara esa deuda; un archivo de forma no gobernada por un esquema debe tener política propia |
| La **E/S de modelo** también sale del hilo de UI | Es parte de la medición del NFR `shell-ui-responsiveness` (ADR-009 §5) y el tramo de generación (con cancelación) es del Bolt 026 |
| Lo portable del 1.4 se conserva con **`UseShellExecute = true` explícito** | ADR-009 §3.5: en el runtime objetivo el *default* cambió y sin declararlo el comportamiento es otro |
| No se persigue la **paridad con el `PropertyGrid`** | La experiencia legacy estaba construida sobre atributos de diseñador que el modelo ya no lleva; recuperarla entera no es objetivo de la 2.0 y debe estar documentado |
| Un *toolkit* MVVM o un control de terceros **no** cambia el stack, pero entra con las reglas de ADR-001 §3.4 | ADR-009 §3.1 lo permite explícitamente: licencia permisiva, versión fijada y centralizada, registrada en el MEM |

---

## 15. Stop conditions

1. **Cambio material en una fuente gobernada** (Bolt, ADR-009/ADR-003/ADR-002/ADR-004, el REV usado, baseline de
   código): detener, revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca dos revisiones (G16).
2. **El contrato del formato no expone los enums reales o el mapeo de tipos accesible**: detener y **reportar
   contra el Bolt 004** (es su criterio de aceptación, ADR-009 §3.2). El editor **no** se parchea con una lista
   propia: sería exactamente la duplicación del esquema que el ADR prohíbe.
3. **El editor necesitaría un campo o un valor que el esquema no declara**: detener y escalar al **ADR-002**
   (subida de `schemaVersion`). Nunca un campo oculto en la UI (ADR-009 §4).
4. **El shell tendría que parsear o escribir el documento por su cuenta** (por ejemplo, para guardar "por partes"):
   detener; `Modeling` es el **único** lector/escritor (ADR-002 §3.4).
5. **El estado del usuario tendría que guardar un secreto** (una cadena de conexión, un valor protegido): detener;
   el NFR `shell-state-secret-free` no admite excepción (ADR-009 §3.3). La credencial se pide en memoria.
6. **El trabajo deriva al flujo de plan/reporte, a la asincronía o al handoff al CLI** (Bolt 026) **o** al
   empaquetado/asociación (Bolt 027): detener y devolverlo a su Bolt; la frontera del DoR se respeta por contrato
   de entregables.
7. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
8. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite o build): detener y escalar la
   secuencia.
9. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, qué campos opcionales mostrar por defecto): parar
   y reportar; no se rellena el hueco con una suposición (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–E implementadas (o, si el presupuesto se agota, progreso en el MEM y pendiente explícito para un
      V-Bounce nuevo — el Bolt estima 2)
- [ ] Todos los criterios de aceptación (AC-1…AC-9) verificados
- [ ] Suite hermética verde (§8.1) y suite de integración escrita, compilada y estáticamente verificada (§8.2)
- [ ] El editor no contiene ninguna definición de campos; los enums y el mapeo vienen del contrato (o el fallo está
      reportado contra el Bolt 004)
- [ ] El estado del usuario está en su archivo, con versión de forma, y **0** secretos en estado y log
- [ ] El código sigue ADR-009 (§3.1/§3.2/§3.3), ADR-003 (§3.1/§3.4), ADR-002 (§3.1/§3.4) y ADR-004 (§3.4)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9, con los dos NFR de ADR-009 §5 medidos
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la lista de rupturas y diferencias
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado
- [ ] Plan de integración satisfecho antes de la aceptación: corrida de cobertura pasante en el entorno de
      escritorio real (perfil de usuario), registrada en `integration_test_runs[]`
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) y su manifiesto
  [`US-000.BOLT-025-…json`](../metrics/bolts/US-000.BOLT-025-shell-wpf.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) · [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) ·
  [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) · [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md)
- SPECs prerrequisito: [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) (motor y punto de entrada) ·
  [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) (contrato del `.coop2`) ·
  [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) (POCOs y mapeo) ·
  [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y build)
- SPECs relacionados (fronteras): [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) (el CLI con el que el shell coincide) ·
  [`SPEC-261016-0035`](./SPEC-261016-0035-library-portable.md) (el almacén del framework, que **no** es el estado del shell)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-33, F-35, F-38 y §6 ítem 14
- Evidencia de código 1.4 (verificada en esta sesión): `Tools/CooperatorModeler/Program.cs` (`[STAThread]`,
  `Application.EnableVisualStyles()`, `Application.Run(new MainForm { args })`),
  `Tools/CooperatorModeler/MainForm.cs:115-165` (`RecreateTreeView`/`PopulateNode` y
  `propertyGrid1.SelectedObject = ((TreeNode)e.Node).Tag`),
  `Tools/CooperatorModeler/FileExtensionAssociation.cs` (`Registry.ClassesRoot.CreateSubKey(".coop")`,
  `SetValue(null, "coopmodelerfile")`, comando con `Application.ExecutablePath`),
  `Tools/CooperatorModeler/Properties/Settings.Designer.cs` (`Settings : ApplicationSettingsBase`; sin `.settings` y
  **sin** usos de `Settings.Default` en el código del tool),
  `Tools/CooperatorModeler/{About.cs:56, CodeGenerator.cs:141,162,272,288,359,460,503}` (`Library.IO.Path.AppPath`),
  `Tools/CooperatorModeler/{About.cs:156, GenerationDialog.cs:176,207}` (`Process.Start` sin `UseShellExecute`),
  `Tools/CooperatorModeler/GenerationDialog.cs` (`Application.DoEvents()` alrededor de la generación)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|---------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-025`: esqueleto WPF `net10.0-windows` que hospeda el motor en proceso por su punto de entrada único, editor dirigido por el esquema del contrato (cero duplicación, enums y mapeo del contrato, derivados no editables), lectura/escritura sólo por el contrato con byte-compatibilidad frente al CLI, validación con las reglas del motor, estado del usuario en JSON por usuario sin secretos y sin Registro, y constancia de las diferencias declaradas frente al tool 1.4 |

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
| **review.reviewers** | [Dev-validator + dueño de arquitectura (stack, frontera shell↔motor) + dueño de dominio de datos si se evalúa el editor dirigido por esquema + dueño de seguridad si se evalúa el NFR sin secretos] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **Frontera declarada (DoR):** este Bolt entrega esqueleto + motor hosteado + editor + estado; el **flujo de plan
   y reporte con asincronía/cancelación y el handoff al CLI** son del Bolt 026 y el **empaquetado/asociación** del
   Bolt 027. Confirmar que el recorte es el esperado.
2. **Criterio con dueño (ADR-009 §3.2):** el editor **depende** de que el contrato exponga enums reales y el mapeo
   de tipos accesible (Bolt 004). Si falta, el fallo se **reporta contra ese Bolt** y no se parchea en la UI.
   Confirmar.
3. **El estado del usuario es capacidad nueva:** verifiqué que el tool 1.4 **no** persistía estado (el
   `Settings.Designer.cs` existe pero no se usa y no hay `.settings` ni `user.config`). El SPEC lo trata como
   requisito nuevo con política de forma propia, no como migración de un archivo existente. Confirmar.
4. **Medición de `shell-ui-responsiveness`:** acá se mide el tramo de **E/S de modelo**; el tramo de **generación
   con cancelación** se mide en el Bolt 026 (ADR-009 §5). Confirmar el reparto de la medición.
5. **Dependencias de UI:** por defecto **cero** terceros; si aparece la necesidad de un *toolkit* MVVM o un control
   de terceros, entra con las reglas de ADR-001 §3.4 y se registra en el MEM (no cambia el stack). Confirmar.
6. **Entorno del ciclo de integración:** el plan declara un entorno de escritorio **Windows** con un **perfil de
   usuario designado y desechable** (el estado vive ahí); **aún no provisionado**. Confirmar el entorno o validar el
   objetivo.
