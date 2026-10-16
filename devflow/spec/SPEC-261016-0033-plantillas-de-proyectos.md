---
id: "SPEC-261016-0033"
title: "Plantillas de proyectos y de configuración: descriptores SDK-style, archivo de configuración sin credenciales y arranque de precedencia"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-021" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-003", "ADR-004", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, build compartido y comprobaciones de invariantes (Bolt 001)
  - "SPEC-261016-0023-motor-headless.md" # el motor headless y su set de plantillas embebido: donde viven estas plantillas (Bolt 017)
  - "SPEC-261016-0018-configuracion-portable.md" # el contrato de configuración y su catálogo: de dónde salen las claves que el archivo declara y el contrato que el arranque entrega (Bolt 011)
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (refactor): lo que se entrega son los descriptores de proyecto y el arranque de configuración de la aplicación generada; el resultado se verifica dentro de esa aplicación, sin base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:37:49-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:37:49-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:37:49-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-021 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent; su acknowledgment declara que la dependencia del canal —ADR-001 §3.6, pendiente con dueño y fecha— no bloquea READY), ADR-003/ADR-004/ADR-001 accepted (tech_lead), REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → applicability: n/a con razón; descriptores SDK-style con el inventario de lo que NO se emite (metadata 1.4, HintPath, System.Web, ClickOnce, Compile manuales) + esqueleto de app sólo bajo su opción; appsettings.json con claves del catálogo y sin credenciales/providerName + arranque con precedencia entorno > archivo > default; clasificación editable; UI no emitida por defecto + frontera del canal; compilación de los dos workspaces; idempotencia con normalización CRLF."
    - "ADJUDICACIÓN (a) — config + arranque con el esqueleto, destinatario = la app: APROBADA. ADR-004 §3.4 fija «quién lee el archivo es la aplicación» y que la plantilla emite el bootstrap que arma el ICooperatorConfiguration a partir del archivo y del entorno. El archivo y el bootstrap pertenecen a la aplicación, que se emite como parte del esqueleto (bajo su opción); el framework no lee archivos (sólo consume el contrato, default = variables de entorno). Emitirlos sin la opción obligaría a decidir dónde viven en un consumidor que sólo genera librerías."
    - "ADJUDICACIÓN (b) — precedencia demostrada sobre la aplicación generada en el repositorio (3 observaciones): APROBADA. La precedencia es propiedad del arranque (bootstrap), no del framework (ADR-004 §3.2: el framework no define precedencias). Se demuestra con 3 escenarios observables sobre la app generada in-repo. El Bolt fue aprobado con not-required exactamente por esto (nota del arquitecto: «si exigiera un entorno real, el valor correcto sería required»). Un entorno real sería un Bolt amendment (required + revisión de SPEC + plan applicable) — no es el caso."
    - "Compliant — clasificación editable con contraste (un artefacto poseído sí se reescribe); sin dependencias nuevas no autorizadas (ADR-001 §3.4); la lista de lo NO emitido como evidencia explícita; frontera del canal declarada (ADR-001 §3.6)."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-021`. El motor y el mecanismo de
     resolución del set son del Bolt 017; el set de clases C# es del Bolt 019; el de stored procedures, del Bolt
     020; el contrato de configuración y su catálogo son del Bolt 011 (**consumidos**); el shell del Modeler es
     del Bolt 025.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ LA PRECEDENCIA ES DE ESTE ARRANQUE, NO DEL FRAMEWORK (ADR-004 §3.4): el framework sólo lee el contrato que se
     le entrega y su implementación por defecto lee **variables de entorno**. Que el archivo se lea y que el
     entorno gane es propiedad del arranque que esta plantilla emite — y por eso se demuestra acá, nivel por
     nivel, y no se enuncia.
-->

# SPEC-261016-0033 — Plantillas de proyectos y de configuración: descriptores SDK-style, archivo de configuración sin credenciales y arranque de precedencia

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítems 9 y 15) |
| **Bolt** | [US-000.BOLT-021](../functional/bolts/US-000.BOLT-021-plantillas-de-proyectos.md) |
| **ADRs** | [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega la **familia de plantillas de andamiaje** de la aplicación generada: los **descriptores de proyecto**
de las librerías del consumidor con la **forma moderna de consumo** (proyecto SDK-style, un solo target moderno,
consumo por **referencia de paquete**, sin copia de ensamblados, sin `HintPath`, sin `SpecificVersion=False`, sin
la dependencia de la plataforma web y sin la metadata de publicación del tooling anterior); el **archivo de
configuración** que reemplaza al `App.config` clásico, con las claves del **catálogo del framework** y **ninguna
credencial en claro**; y el **arranque** de la aplicación generada que combina sus fuentes con la precedencia
declarada —**entorno por encima de archivo, archivo por encima de valor por defecto**— y entrega al framework el
**contrato de configuración** por el que éste lo lee. Estas piezas son **artefactos editables del consumidor**:
se crean sólo si faltan, **nunca** se sobrescriben, su deriva se advierte y forzar es una acción explícita. El
esqueleto de aplicación se emite **sólo** bajo su opción, de modo que el camino por defecto del motor **no** fija
ninguna interfaz de usuario.

El problema que resuelve está documentado (REV-001 F-37 y §6 ítems 9 y 15): lo que hoy emiten las plantillas de
proyecto y de configuración ata al consumidor al formato anterior —`Templates/CSharpProjects/DataProject.csproj`
(y sus hermanos) declara `TargetFrameworkVersion v4.0`, referencias con
`Version=1.4.5.0` + `PublicKeyToken=8f7f403e20325116` + `<SpecificVersion>False</SpecificVersion>` +
`<HintPath>..\CooperatorAssemblies\…dll</HintPath>`, la referencia a `System.Web` y metadata de publicación del
tooling (`PublishUrl`, `Install`, `BootstrapperPackage`, `ApplicationVersion`)— y
`Templates/ConfigFiles/App.config.cs` emite un `App.Config` con la **cadena de conexión del modelo dentro del
archivo** y `providerName="System.Data.SqlClient"`.

Si no se implementa: el consumidor que migra recibe de nuevo un proyecto .NET Framework con copia de ensamblados
y una configuración con credenciales en un archivo versionado, y la precedencia de los valores queda sin dueño.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 15 ruteó F-36/F-37 (los artefactos del formato anterior: la
dependencia de la plataforma web, el consumo por copia de ensamblados y la cadena en el archivo) a un Bolt de
plantillas, y el §6 ítem 9 a la configuración portable (ADR-004), que decide **qué** reemplaza al `App.config` y
**de quién** es la precedencia.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-003 §3.1 (artefactos del set)** — los artefactos de 2.0 incluyen: `*.Auto.cs`, los partials editables, el
  provider Lazy por defecto, el ejemplo de regla, **los proyectos SDK-style de las librerías**
  `Data`/`Entities`/`Rules` (**`net10.0`**, con **`PackageReference`** — sin `HintPath`, sin
  `CooperatorAssemblies`, sin `System.Web`), y los **scripts de stored procedures**. El **esqueleto de app** del
  consumidor (`net10.0-windows`) queda **detrás de un flag explícito**, para que el camino por defecto del motor
  **no** fije interfaz de usuario. El motor **emite** artefactos y **nunca** ejecuta SQL ni lee configuración de
  runtime.
- **ADR-003 §3.4 (propiedad de archivos)** — los artefactos que el motor posee (`*.Auto.cs`, el manifiesto de
  propiedad, los scripts de SP) se reescriben siempre; los **artefactos editables** —partials, `Program.cs`, el
  ejemplo de regla **y los `*.csproj`/`.sln` de los proyectos generados**— se **crean sólo si faltan** y
  **nunca** se sobrescriben: si el contenido calculado difiere del archivo existente, el motor lo deja intacto y
  registra un **warning de deriva**; `--force` es la única forma de sobrescribirlos y es explícito.
- **ADR-003 §3.3 (determinismo e idempotencia)** — archivos en UTF-8 sin BOM, `LF`, sin espacios finales, orden
  canónico, sin marcas temporales, escritura sólo si cambia, y normalización CRLF→LF antes de decidir si un
  archivo difiere.
- **ADR-004 §3.4 (configuración de las apps generadas)** — `App.config` **desaparece** y su reemplazo es
  **`appsettings.json`** junto al ejecutable (UTF-8 sin BOM) **más variables de entorno**, con precedencia
  **entorno > archivo > default**; **quién lee el archivo es la aplicación** —la plantilla emite el bootstrap que
  arma el `ICooperatorConfiguration` a partir del archivo y del entorno— porque el framework **sólo consume el
  contrato** y su implementación por defecto lee **sólo variables de entorno**, sin archivos ni precedencias
  propias (ADR-004 §3.2); **ninguna credencial en claro en un archivo versionado** (si la cadena lleva
  credenciales, va por entorno o protegida por el ADR-007); el valor de conexión declara su **prefijo de esquema
  explícito** y `providerName` **deja de existir** en el framework y en la configuración.
- **ADR-004 §3.2 (catálogo)** — las claves que el archivo declara salen del catálogo del framework
  (`Cooperator:ConnectionStrings:<nombre>`, `Cooperator:Data:CommandTimeoutSeconds`,
  `Cooperator:Cache:DefaultTtlSeconds`, `Cooperator:Cache:ParameterMetadataTtlSeconds`,
  `Cooperator:Mail:SmtpServer`, `Cooperator:LazyLoad:Providers:<entidad>:Assembly|:Class`) y **el contrato lo
  implementa el Bolt 011**.
- **ADR-001 §3.1/§3.4 (layout y dependencias)** — el consumo es por `PackageReference` y el `CooperatorAssemblies`
  + `HintPath` + `SpecificVersion=False` desaparecen como modelo de consumo; el **folder feed** con el juego de
  `.nupkg` viaja junto al release del Modeler para que el consumo offline no dependa de infraestructura extra; y
  **toda dependencia de la aplicación generada se autoriza en un ADR**, con versión fijada y centralizada.
- **ADR-001 §3.5 (deprecaciones)** — sin `System.Deployment`/ClickOnce; sin VB (las plantillas `VisualBasic*` se
  eliminan); el tooling se distribuye por `dotnet publish`.
- **ADR-001 §3.6 (canal)** — el **canal de distribución** (feed privado vs. público, firma de paquetes y
  publicación de los `.nupkg`) es una decisión **pendiente con dueño y fecha** (el `tech_lead` antes del release
  2.0.0): los descriptores **referencian paquetes** y la forma de publicarlos **no** la decide este Bolt.

**Qué decide este SPEC, dentro de esas restricciones.** El **contenido** de cada descriptor (un TFM, propiedades
mínimas, `PackageReference` por paquete, sin las referencias heredadas ni la metadata del tooling anterior), el
**contenido** del archivo de configuración (las claves del catálogo, sin credenciales y sin `providerName`), la
**forma del arranque** que implementa la precedencia y fija el contrato una vez al inicio, y la **clasificación
de estos artefactos como editables** con su regla de no sobrescritura y su aviso de deriva.

**Fronteras declaradas.** El **mecanismo** de plantillas, el contexto de render y la resolución del set son del
**Bolt 017** (ADR-003 §3.1/§3.5) — estas plantillas son **contenido** del set; el set de **clases C#** es del
**Bolt 019** y el de **stored procedures** del **Bolt 020**; el **contrato de configuración y su catálogo** son
del **Bolt 011** (se consumen); la **protección de valores** que una connection string puede usar es del
**ADR-007 / Bolt 022**; la **interfaz de usuario** del consumidor no es alcance de este Bolt (el esqueleto va
detrás de su opción y su UI pertenece al consumidor); y el **shell del Modeler** es del Bolt 025.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-021`](../functional/bolts/US-000.BOLT-021-plantillas-de-proyectos.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-37 (plantillas que emiten el formato anterior: proyecto .NET Framework, `System.Web`, copia de ensamblados y cadena en el archivo), F-36 (metadata de publicación/ClickOnce), F-13 (configuración clásica) y §6 ítems 9 y 15 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.1 (artefactos del set: proyectos **SDK-style** `net10.0` con `PackageReference`, sin `HintPath`/`CooperatorAssemblies`/`System.Web`; esqueleto de app **detrás de un flag**; el motor emite y no ejecuta SQL), §3.4 (artefactos **editables**: se crean sólo si faltan, nunca se sobrescriben, deriva advertida, `--force` explícito), §3.3 (determinismo e idempotencia), §3.5 (plantillas como datos, set embebido y overrides) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.4 (`App.config` → **`appsettings.json`** + entorno, precedencia **entorno > archivo > default**, quién lee el archivo, claves del catálogo, prohibición de credenciales en un archivo versionado), §3.2 (contrato `ICooperatorConfiguration`, catálogo de claves y su default por entorno), §3.3 (prefijo de esquema explícito; `providerName` fuera) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (consumo por `PackageReference`; el conjunto de paquetes y el **folder feed** junto al release), §3.4 (toda dependencia se autoriza en un ADR, con versión fijada y centralizada), §3.5 (sin ClickOnce; sin VB), §3.6 (el **canal** de distribución: decisión pendiente con dueño y fecha) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-017`](../functional/bolts/US-000.BOLT-017-motor-headless.md) — su SPEC canónica ([`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md)) entrega el motor y el set embebido donde viven estas plantillas | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — su SPEC canónica ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md)) entrega el contrato de configuración y el catálogo cuyas claves declara el archivo | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, build compartido y comprobaciones de invariantes | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de plantillas y código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar
una línea): `Tools/CooperatorModeler/Templates/CSharpProjects/{DataProject,EntitiesProject,RulesProject,
AppProject}.csproj` y `Solution.sln` — en `DataProject.csproj`: `ToolsVersion 4.0`,
`<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>`, las tres `<Reference>` a los ensamblados del framework
con `Version=1.4.5.0`, `PublicKeyToken=8f7f403e20325116`, `<SpecificVersion>False</SpecificVersion>` y
`<HintPath>..\CooperatorAssemblies\…dll</HintPath>` (líneas 61-72), la referencia a `System.Web` (línea 76), la
metadata de publicación (`PublishUrl`, `Install`, `InstallFrom`, `UpdateEnabled`, `ApplicationVersion`,
líneas 21-35) y los `BootstrapperPackage` de .NET 3.5 (líneas 108-124); `Templates/ConfigFiles/App.config.cs` —
el `App.Config` emitido con `<connectionStrings><add name="CooperatorConnectionString"
connectionString="<la cadena del modelo>" providerName="System.Data.SqlClient" />` (la **credencial en el
archivo**) más un `appSettings` propio; `Templates/Assemblies/*` (el mecanismo de copia de ensamblados con su
`Readme.txt`); `Templates/CSharpProjects/AppProjectFiles/{Program.cs,Form1.cs}` (el esqueleto de app WinForms) y
`Templates/VisualBasicProjects/*` (el set VB, retirado por ADR-001 §3.5).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- Las plantillas de los **descriptores de proyecto** de las librerías generadas (`Data`, `Entities`, `Rules`) y de
  la solución: **SDK-style**, **un solo** target moderno, **consumo por `PackageReference`**, y **sin** copia de
  ensamblados (`CooperatorAssemblies`), sin `HintPath`, sin `SpecificVersion=False`, sin la dependencia de la
  plataforma web, sin `ToolsVersion`/`TargetFrameworkVersion` del formato anterior y sin la metadata de
  publicación del tooling anterior (ClickOnce/bootstrapper).
- La plantilla del **esqueleto de aplicación** del consumidor **bajo su opción explícita** (con su descriptor de
  proyecto, su punto de entrada y su interfaz mínima).
- Las plantillas del **archivo de configuración** que reemplaza al `App.config`: `appsettings.json` con las
  **claves del catálogo** del framework, **sin credenciales en claro**, **sin material de claves** y **sin**
  `providerName`.
- El **arranque** de la aplicación generada: el código que combina **archivo** y **entorno** con la precedencia
  **entorno > archivo > default** y entrega al framework el **contrato de configuración**
  (`ICooperatorConfiguration`) fijado una vez al inicio.
- La **clasificación de estos artefactos como editables** (ADR-003 §3.4): se crean sólo si faltan, **nunca** se
  sobrescriben, la deriva se advierte y `--force` es la acción explícita; y su **determinismo** cuando el motor
  los crea (ADR-003 §3.3).
- La evidencia: los descriptores **compilando** contra los paquetes, el listado de lo emitido **y de lo que no se
  emite**, la **demostración de la precedencia nivel por nivel**, la ausencia de credenciales, la no
  sobrescritura con aviso de deriva, la emisión del esqueleto **sólo** con su opción y la idempotencia.
- La constancia de las **rupturas** que este Bolt introduzca (descriptores del formato anterior, `App.config`,
  `providerName`, el set VB) para las **notas de migración** (ADR-008 §3.1, documento del Bolt 028).

### Out of scope

- El **motor**, su contexto de render y la **resolución del set** (embebido, overrides por nombre, versión):
  **Bolt 017** (ADR-003 §3.1/§3.5). Este Bolt entrega **contenido** del set, no el mecanismo.
- El **set de clases C#** (`*.Auto.cs`, partials, provider Lazy, ejemplo de regla): **Bolt 019** — su SPEC
  canónica es [`SPEC-261016-0029`](./SPEC-261016-0029-plantillas-csharp.md).
- El **set de stored procedures**: **Bolt 020**.
- El **contrato de configuración, su catálogo y la forma del prefijo de esquema**: **Bolt 011** (ADR-004 §3.2/§3.3).
  Este Bolt **consume** el catálogo: declara sus claves, no las define.
- La **protección de valores** (`aesgcm:` y ss.): **ADR-007 / Bolt 022**. El archivo emitido **no** contiene
  material de claves ni valores protegidos: un valor con esquema distinto de `plain:` lo aporta el **entorno**
  del consumidor.
- El **esqueleto de interfaz de usuario** del consumidor y su diseño: el esqueleto se emite **sólo** con su
  opción y su UI es del consumidor (ADR-003 §3.1: el camino por defecto no fija UI).
- El **canal de distribución** (feed, firma de paquetes, publicación de los `.nupkg`): **ADR-001 §3.6**, decisión
  pendiente del `tech_lead` con fecha antes del release 2.0.0.
- El **shell del Modeler**, su configuración y su empaquetado: **Bolts 025/027**.
- La **ejecución de SQL** y la aplicación de los scripts de SP: el motor **nunca** las ejecuta (ADR-003 §3.1).
- El **CLI** y sus comandos: **Bolt 018**.

---

## 5. Prerequisites and baseline

- **Bolt 017 y su SPEC canónica
  ([`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md), `prerequisites`):** el motor headless
  (`Cooperator.Framework.CodeGeneration`, `net10.0`) con el **set de plantillas embebido** y el mecanismo de
  resolución (set por defecto + overrides por nombre), la **proyección del modelo** como datos para las
  plantillas, el **contexto de render endurecido**, la **regla de propiedad** de los artefactos y el **manifiesto**
  de propiedad con su aviso de deriva. Sin el motor no hay dónde ni cómo emitir estas plantillas.
- **Bolt 011 y su SPEC canónica
  ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md), `prerequisites`):** el contrato
  `ICooperatorConfiguration` y el **catálogo de claves** — de donde salen las claves que el archivo declara y el
  contrato que el arranque entrega. Si no está en el árbol 2.0 al iniciar el V-Bounce, **parar y reportar** (§15).
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés (donde corren los
  tests que emiten, compilan y ejecutan la aplicación generada), el build compartido y las comprobaciones de
  invariantes.
- **El contrato del modelo (Bolt 004, [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md)) —
  prerrequisito transitivo** del motor: las plantillas iteran las entidades y propiedades del modelo y los
  descriptores declaran los archivos que esas entidades generan.
- **Los paquetes de la 2.0 resolubles** para que los descriptores emitidos **compilen**: el consumo es por
  `PackageReference` y el **folder feed** con el juego de `.nupkg` viaja junto al release del Modeler
  (ADR-001 §3.1). Para la evidencia del V-Bounce, los paquetes se resuelven desde la salida local del árbol
  (§8.1). Si no hay forma de resolverlos, **parar y reportar** (§15): un descriptor que no compila no es
  evidencia.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de lo que estas plantillas
  dejaban de emitir.
- **Build state de partida:** el motor del Bolt 017 compila y su suite corre; este Bolt **no** cambia TFMs del
  producto, **no** cambia el mapa de paquetes del framework y **no** agrega dependencias nuevas a la aplicación
  generada (ADR-001 §3.4: toda dependencia se autoriza en un ADR).
- **Decisión de ejecución registrada (no es supuesto):** la demostración de la precedencia es sobre la
  **aplicación generada dentro del repositorio** (el `AITL-BOLT-READY-Approval` del Bolt evaluó y **confirmó**
  `test_integration: not-required` con esa lectura, dejando constancia de que si exigiera un entorno real el valor
  correcto sería `required`). No hay servicio, base de datos ni entorno desplegado en el alcance.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-021`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — Los descriptores de proyecto

**Duration:** 4h total cycle — **Complexity:** High

#### A.1 Forma moderna de consumo

Se escriben las plantillas de los descriptores de proyecto de las librerías generadas (`Data`, `Entities`,
`Rules`) y de la solución, con la **forma moderna de proyecto**: SDK-style, **un solo** TFM (`net10.0`, el del
consumo de la 2.0), `PackageReference` a los paquetes del framework (versión **fijada y centralizada**,
ADR-001 §3.4) y las referencias entre proyectos del consumidor por ruta de proyecto. **No** se emite ninguna de
las piezas del formato anterior:

| Pieza de 1.4 que **no** se emite | Dónde estaba |
|---|---|
| `<Reference>` con `Version=1.4.5.0` + `PublicKeyToken=…` + `<SpecificVersion>False</SpecificVersion>` | `DataProject.csproj:61-72` (y sus hermanos) |
| `<HintPath>..\CooperatorAssemblies\…dll</HintPath>` (y con él, el mecanismo de **copia de ensamblados**) | `DataProject.csproj:63/67/71` + `Templates/Assemblies/*` |
| `<Reference Include="System.Web" />` | `DataProject.csproj:76` |
| `ToolsVersion`, `<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>`, `TargetFrameworkProfile` | `DataProject.csproj:2/16/36` |
| Metadata de publicación del tooling anterior: `PublishUrl`, `Install`, `InstallFrom`, `UpdateEnabled/…`, `ApplicationVersion`, `BootstrapperPackage` (.NET 3.5) | `DataProject.csproj:21-35/108-124` |
| Listas `<Compile Include>` manuales de los archivos generados | `DataProject.csproj:79-101` (los emite el propio proyecto SDK-style por inclusión por defecto) |

**La solución** (`Solution.sln`) se emite con los proyectos de la aplicación generada y el formato moderno de
solución; **las plantillas VB** no se emiten (ADR-001 §3.5: el motor es sólo C#).

**Regla de determinismo (ADR-003 §3.3):** estas plantillas escriben UTF-8 sin BOM, `LF`, sin espacios finales y
sin marcas temporales; el orden de los elementos es canónico, de modo que dos corridas del mismo modelo producen
bytes idénticos.

**Files created (contenido del set embebido del motor, Bolt 017 — la ruta física y la extensión de las plantillas
siguen la convención del set que ese Bolt fijó; los nombres lógicos son los del set conocido):**
- `…Templates/Projects/DataProject.csproj` — descriptor SDK-style de la librería de datos del consumidor
- `…Templates/Projects/EntitiesProject.csproj` — descriptor SDK-style de las entidades
- `…Templates/Projects/RulesProject.csproj` — descriptor SDK-style de las reglas
- `…Templates/Projects/Solution.sln` — solución de la aplicación generada
- `…Templates/Projects/AppProject.csproj` — descriptor del **esqueleto de aplicación** (sólo con su opción, §D)

#### A.2 El esqueleto de aplicación (bajo su opción)

El descriptor de la aplicación del consumidor y los archivos del esqueleto (`Program.cs` de punto de entrada y
una interfaz mínima) se emiten **sólo** cuando la corrida activa la opción del esqueleto: el **camino por
defecto** del motor no fija interfaz de usuario (ADR-003 §3.1). La interfaz del consumidor **no** se diseña acá:
el esqueleto queda funcional y mínimo, y su evolución es del consumidor.

**Files created:** `…Templates/Projects/AppProject.csproj` y `…Templates/Projects/AppProjectFiles/{Program.cs,Form1.cs}`
(equivalentes en 2.0 del esqueleto de 1.4), emitidos **condicionados** por la opción.

---

### Phase B — El archivo de configuración y el arranque de precedencia

**Duration:** 4h total cycle — **Complexity:** High

#### B.1 `appsettings.json`: las claves del catálogo, sin credenciales

Se escribe la plantilla del archivo que **reemplaza** al `App.config` (ADR-004 §3.4): `appsettings.json` junto al
ejecutable, UTF-8 **sin BOM**, con la sección del framework y las **claves del catálogo** (Bolt 011 / ADR-004
§3.2), tomando del modelo lo que el modelo ya declara (los nombres de proyecto, la base, el prefijo de SP, el
script de post-generación y los providers Lazy por entidad) y **sin** ninguna de las piezas que ADR-004 §3.2-§3.3
prohíbe.

**Contenido obligatorio y contenido prohibido:**

| Debe declarar | No debe declarar |
|---|---|
| `Cooperator:ConnectionStrings:<nombre>` con el **prefijo de esquema explícito** (`plain:`) y un valor **sin credenciales** (autenticación integrada o marcador no sensible) | **ninguna credencial en claro** ni material de claves (ADR-004 §3.4, NFR de ADR-007 §5) |
| `Cooperator:Data:CommandTimeoutSeconds`, `Cooperator:Cache:DefaultTtlSeconds`, `Cooperator:Cache:ParameterMetadataTtlSeconds`, `Cooperator:Mail:SmtpServer` | `providerName` (deja de existir en el framework y en la configuración, ADR-005 §3.3) |
| `Cooperator:LazyLoad:Providers:<entidad>:Assembly` y `…:Class` por entidad con provider configurado (los nombres vienen del modelo) | la cadena de conexión del modelo como valor real (el `.coop2` **no** la transporta: ADR-002 §3.1) — el valor real llega por el **entorno** |
| Claves del framework **solamente**; lo propio del consumidor va en su lado | valores de configuración **sin** su esquema cuando el esquema es obligatorio |

> El archivo es **del consumidor** y es **editable**: lo que el motor posee es que, si no existe, lo crea con
> este contenido (y si existe, no lo toca: §C). Un valor protegido (`aesgcm:` y ss.) lo aporta el **entorno** —su
> protección es del ADR-007/Bolt 022— y **no** se escribe en el archivo.

#### B.2 El arranque: la precedencia, implementada donde pertenece

Se emite el **arranque** de la aplicación generada: el código que (1) lee el archivo (`appsettings.json`, si
existe), (2) lee el **entorno** (variables de entorno con la convención estándar de jerarquía), (3) combina ambos
con la precedencia **entorno > archivo > valor por defecto**, y (4) entrega al framework el
**`ICooperatorConfiguration`** resultante, **fijado una vez** al inicio. El framework **no** lee archivos ni
define precedencias: sólo consume el contrato que se le entrega (ADR-004 §3.2/§3.4). El arranque **no** fija el
contrato más de una vez por proceso y **no** lo reemplaza a mitad de la ejecución (ADR-004 §3.2).

**Files created:**
- `…Templates/Projects/AppProjectFiles/CooperatorBootstrap.cs` — el arranque: lectura del archivo, lectura del entorno, combinación con precedencia y fijación única del contrato
- `…Templates/Config/appsettings.json` — la plantilla del archivo de configuración de §B.1

---

### Phase C — Artefactos editables: no sobrescritura, deriva advertida y forzar explícito

**Duration:** 2h total cycle — **Complexity:** Low

#### C.1 La clasificación y su regla

Los descriptores de proyecto, la solución, el archivo de configuración y el esqueleto de aplicación se registran
como **artefactos editables** en el manifiesto de propiedad del motor (ADR-003 §3.4): se **crean sólo si
faltan**; si el contenido calculado difiere del archivo existente, el motor **deja el archivo intacto** y
registra un **warning de deriva** con el archivo afectado; `--force` es la única forma de sobrescribirlos y es
explícito. El **manifiesto de propiedad** del motor incluye estos artefactos (qué generó, con qué digest) y el
motor **no** borra lo que no reconoce como propio.

> El mensaje del warning de deriva es el del motor (Bolt 017); acá se entrega **qué** artefactos se clasifican
> como editables y **cómo se verifica** que no se pisan, no el texto del aviso.

---

### Phase D — El esqueleto, la UI y la frontera del canal

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 La UI no se emite por defecto

Sin la opción del esqueleto, **no** aparece ningún descriptor de aplicación, ni punto de entrada, ni formulario:
el camino por defecto produce **sólo** las librerías del consumidor. La verificación es la ausencia del artefacto
en el workspace emitido.

#### D.2 La frontera del canal de paquetes

Los descriptores consumen por `PackageReference`: **de dónde** se publican y descargan esos paquetes (feed
privado vs. público, firma, `nupkg`) es el **canal de distribución**, decisión **pendiente con dueño y fecha**
(ADR-001 §3.6). Este Bolt declara esa dependencia como **consecuencia** —no la decide— y su evidencia usa la
resolución local de §8.1.

---

### Phase E — Evidencia

**Duration:** 2h total cycle — **Complexity:** Medium

- **Compilación**: los descriptores emitidos **compilan** contra los paquetes de la 2.0 (resolución local), sin
  referencias heredadas y sin la dependencia de la plataforma web.
- **Listado de lo emitido y de lo no emitido**: las propiedades y referencias de cada descriptor, con la
  constancia de lo que **no** se emite (§A.1) — es la evidencia de que la forma anterior desapareció.
- **Precedencia nivel por nivel**: sobre la **aplicación generada**, un valor presente en el **entorno** gana
  sobre el mismo valor presente en el **archivo**, y el archivo gana sobre el **valor por defecto**, con el
  **resultado observable de cada nivel** (tres escenarios, tres observaciones).
- **Sin credenciales**: el archivo emitido no contiene credenciales ni material de claves, y su contenido
  declarado son claves del catálogo.
- **Edición que sobrevive**: una edición a mano en un descriptor de proyecto y en el archivo de configuración
  **sobrevive** a la regeneración, con su **aviso de deriva**, y `--force` es la acción explícita que la
  reemplaza.
- **Esqueleto condicionado**: con la opción activa se emite; sin ella **no** existe en el workspace.
- **Idempotencia**: dos corridas del mismo modelo no cambian el workspace (NFR de ADR-003 §5).
- **Rupturas**: la lista de lo que este Bolt deja de emitir, para las **notas de migración** (ADR-008 §3.1;
  documento del Bolt 028).

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: Los descriptores son de la forma moderna de consumo y compilan

**Given** el set emitiendo sobre el modelo de referencia
**When** se inspeccionan los descriptores generados y se compilan contra los paquetes de la 2.0
**Then** son SDK-style, con **un solo** target moderno y `PackageReference`; **no** contienen `HintPath`,
`CooperatorAssemblies`, `SpecificVersion=False`, `System.Web`, `ToolsVersion`/`TargetFrameworkVersion` del
formato anterior ni la metadata de publicación/ClickOnce; y **compilan** sin referencias heredadas

### AC-2: El archivo de configuración reemplaza al clásico sin credenciales

**Given** el archivo emitido
**When** se revisa su contenido
**Then** declara las **claves del catálogo** del framework (conexión con prefijo explícito, timeout de comandos,
TTL de caché, correo y providers Lazy por entidad), **no** contiene credenciales en claro, **no** contiene
material de claves y **no** declara `providerName`

### AC-3: La precedencia se demuestra nivel por nivel

**Given** la aplicación generada y tres escenarios —valor en el entorno y en el archivo; valor sólo en el
archivo; valor en ninguno—
**When** la aplicación arranca en cada escenario
**Then** el resultado observable muestra **entorno > archivo > valor por defecto**, el contrato de configuración
se fija **una vez** al inicio y el **framework no lee el archivo** (lo lee el arranque y le entrega el contrato)

### AC-4: Los artefactos editables no se sobrescriben

**Given** un descriptor de proyecto y el archivo de configuración editados a mano por el consumidor
**When** el motor vuelve a correr sobre el mismo modelo
**Then** ambos archivos quedan **intactos**, se registra el **aviso de deriva**, y sólo `--force` los reemplaza

### AC-5: El esqueleto de aplicación se emite sólo con su opción

**Given** una corrida **sin** la opción del esqueleto
**When** se revisa el workspace emitido
**Then** **no** existe ningún descriptor de aplicación, punto de entrada ni formulario —el camino por defecto no
fija interfaz de usuario— **And Given** una corrida **con** la opción, el esqueleto se emite

### AC-6: La emisión es determinista e idempotente

**Given** el mismo modelo
**When** el set emite dos veces
**Then** los bytes de las plantillas creadas son idénticos y el workspace no cambia entre corridas (UTF-8 sin
BOM, `LF`, sin espacios finales y sin marcas temporales; NFR de ADR-003 §5)

### AC-7: No hay dependencias nuevas no autorizadas

**Given** los descriptores y el arranque emitidos
**When** se revisan las dependencias del consumidor
**Then** declaran los paquetes del framework (Bolt 011 y el resto del release) y **ninguna** dependencia nueva de
terceros (ADR-001 §3.4); el motor y el arranque **no** incorporan una dependencia de lectura de archivos de
configuración al framework —el framework sigue leyendo sólo variables de entorno y consumiendo el contrato—

### AC-8: Las rupturas quedan declaradas

**Given** la evidencia del V-Bounce
**When** se revisa la lista de lo que dejó de emitirse
**Then** están declaradas con su reemplazo —descriptores del formato anterior, `App.config`, `providerName`, el
set VB, el mecanismo de copia de ensamblados, la metadata de publicación— para las notas de migración
(ADR-008 §3.1)

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — descriptores de la forma moderna de proyecto, sin referencias heredadas, sin la plataforma web, sin metadata del tooling anterior | §6/A.2 | AC-1 · plantillas emitidas + compilación + listado de lo no emitido |
| Bolt §2 — el archivo de configuración con las claves del catálogo y ninguna credencial en claro | §6/B.1 | AC-2 · contenido del archivo + verificación de ausencia de credenciales/material |
| Bolt §2 — el arranque combina sus fuentes con la precedencia declarada y entrega el contrato al framework | §6/B.2 | AC-3 · demostración nivel por nivel sobre la aplicación generada |
| Bolt §2 — los descriptores y el archivo son **editables**: no se sobrescriben y la deriva se advierte | §6/C.1 | AC-4 · edición a mano + regeneración + warning de deriva + `--force` |
| Bolt §2 — el esqueleto de aplicación sólo bajo su opción | §6/A.2, §6/D.1 | AC-5 · ausencia/presencia del artefacto según la opción |
| Bolt §2 — el mismo modelo produce siempre los mismos bytes | §6/A.1, §6/E | AC-6 · dos corridas sin diferencias en el workspace |
| ADR-003 §3.1/§3.4 — proyectos SDK-style con `PackageReference`, propiedad de archivos y `--force` | §6/A.1, §6/C.1 | AC-1, AC-4 · inspección + comportamiento del motor |
| ADR-004 §3.2/§3.4 — claves del catálogo, `appsettings.json` + entorno, precedencia del arranque, sin credenciales | §6/B.1, §6/B.2 | AC-2, AC-3, AC-7 · contenido + demostración |
| ADR-001 §3.1/§3.4/§3.6 — consumo por paquete, dependencias autorizadas y canal como frontera | §6/D.2 | AC-1, AC-7 · descriptores + frontera declarada |
| ADR-008 §3.1 — rupturas declaradas con reemplazo | §6/E | AC-8 · lista en el MEM para el Bolt 028 |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Tests de contenido de lo emitido (por artefacto):** emitir el set sobre el **modelo de referencia** (el del
  arnés) en un directorio temporal y verificar, archivo por archivo: el descriptor es SDK-style con **un solo**
  TFM y `PackageReference`; **no** aparece `HintPath`, `CooperatorAssemblies`, `SpecificVersion=False`,
  `System.Web`, `ToolsVersion`, `TargetFrameworkVersion v4.0`, `PublishUrl`/`Install`/`BootstrapperPackage` ni
  `ApplicationVersion`; el archivo de configuración declara las claves del catálogo y **no** contiene
  `providerName`, credenciales ni material de claves (búsqueda de patrones + gate de secretos); las plantillas VB
  **no** se emiten; el archivo es UTF-8 sin BOM, con `LF` y sin espacios finales.
- **Test de compilación de lo generado:** los descriptores emitidos se **compilan** en el arnés contra los
  paquetes de la 2.0 (resolución local de §5): es la prueba de que el consumo por `PackageReference` es real y de
  que no quedó ninguna referencia heredada. Se compilan **con** y **sin** la opción del esqueleto (dos
  workspaces).
- **Test de precedencia (nivel por nivel):** tres escenarios sobre la aplicación generada —
  (a) el mismo valor en el entorno y en el archivo ⇒ gana el **entorno**; (b) el valor sólo en el archivo ⇒
  gana el **archivo**; (c) el valor en ninguno ⇒ se obtiene el **valor por defecto** documentado. Cada escenario
  se observa por un punto verificable (un valor que el arranque deja observable, sin depender de una UI) y el
  test afirma además que el **framework no lee el archivo**: el arranque lee y entrega el contrato.
- **Test de propiedad de archivos:** editar a mano un descriptor y el archivo de configuración → regenerar →
  ambos intactos + **warning de deriva** registrado; `--force` → reemplazados; un artefacto **poseído** por el
  motor (`*.Auto.cs` del Bolt 019) → sí se reescribe (el contraste prueba que la clasificación es la correcta).
- **Test de idempotencia/determinismo:** dos corridas del mismo modelo → mismos bytes en las plantillas creadas y
  ningún cambio en el workspace; y una corrida con `CRLF` en los archivos existentes → la comparación normaliza y
  **no** los marca como cambiados (ADR-003 §3.3).
- **Consumer-side contract tests (test doubles):** el arranque consume el **contrato de configuración** (Bolt 011)
  y en la prueba se ejercita con la implementación del **entorno** que el framework provee y con un **archivo de
  prueba** —el par (archivo, entorno) es el caso real y no un doble; lo que se dobla es el **valor por defecto**
  (documentado) para poder observarlo sin depender del host. `n/a` para servicios: no hay ninguno en el alcance.
- **Edge cases:** modelo sin entidades (los descriptores no deben romperse); modelo con entidades sin provider
  Lazy (la clave de LazyLoad no aparece por entidad sin provider); nombres de proyecto con caracteres que exigen
  escape en el archivo; archivo de configuración existente **vacío** o **inválido** (el bootstrap falla con error
  explícito y **no** lo sobrescribe —la escritura no es suya—); un valor del entorno con la convención de doble
  guion bajo; un valor del archivo y del entorno a la vez (gana el entorno); un valor con un esquema
  **desconocido** (el error tipado es del contrato de configuración, Bolt 011; el arranque **no** lo interpreta);
  el archivo con `BOM` (se reporta; el emitido nunca lo lleva); directorio de salida con archivos previos de una
  corrida anterior.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** el listado de artefactos emitidos con su propósito, el de lo que **no** se
  emite, la corrida de compilación de los dos workspaces (con y sin esqueleto), las **tres observaciones** de la
  precedencia, el contenido del archivo con la constancia de ausencia de credenciales, la prueba de no
  sobrescritura con aviso de deriva, las dos corridas de idempotencia y la lista de rupturas. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — lo que se entrega son **descriptores y arranque de una
  aplicación generada**, y su verificación es la aplicación generada **dentro del repositorio**: compilarla y
  observarla en los tres escenarios de precedencia. No hay base de datos, cola, servicio, almacenamiento ni
  entorno desplegado que aporte una verificación adicional (el motor no ejecuta SQL ni lee configuración de
  runtime, ADR-003 §3.1).
- **Contract / E2E against the real providers:** `n/a` — el único contrato que cruza un límite es el **de
  configuración**, y su verificación pertinente es la precedencia del arranque (nivel V-Bounce) más el contrato
  del Bolt 011. El proveedor de datos real es otro Bolt (014) y su ciclo de integración es el suyo.
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
| Build / lint / type checks | V-Bounce | Sin errores ni warnings en el motor y en el set; los **descriptores emitidos compilan** contra los paquetes de la 2.0 | pass |
| Unit / component tests | V-Bounce | 100 % verdes: contenido por artefacto, compilación de los dos workspaces, precedencia (tres escenarios), propiedad de archivos, idempotencia | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El arranque ejercita el **contrato de configuración** (Bolt 011) con el par real (archivo + entorno) y con el valor por defecto documentado | pass |
| SAST | V-Bounce | 0 hallazgos altos; el arranque no abre rutas ni valores provistos por el modelo sin declararlos | pass |
| Dependency scanning / licenses | V-Bounce | La aplicación generada declara **sólo** los paquetes del release, con versión fijada y centralizada; **0** dependencias nuevas no autorizadas (ADR-001 §3.4) | pass |
| OWASP Top 10 — static review | V-Bounce | Configuración y secretos: el archivo emitido **no** lleva credenciales ni material de claves y el valor real llega por el entorno; el arranque no registra valores (sólo claves) y no los imprime; no introduce lectura de rutas ni deserialización de contenido no confiable (el archivo es del consumidor y su formato es el declarado) | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción: las plantillas renderizan datos del modelo y el arranque lee claves/valores de configuración | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** credenciales en el archivo emitido, en las plantillas, en el modelo de referencia del arnés y en los tests; 0 material de claves (ADR-007 §5, NFR *No embedded secrets*) | pass |
| Hallucination lint | V-Bounce | Cada pieza emitida y cada pieza **no** emitida trazable a la plantilla 1.4 citada y a ADR-003/ADR-004; 0 claves de catálogo inventadas (las que no existan se piden al Bolt 011, §15) | pass |
| IP / license provenance | V-Bounce | Plantillas propias; 0 código de terceros; las dependencias declaradas son los paquetes autorizados del release | pass |
| PII / DLP | V-Bounce | `internal`: el modelo de referencia y el archivo emitido no contienen datos personales; el archivo emitido **no** contiene credenciales (única categoría sensible que podría aparecer aquí) | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes; los descriptores referencian IDs calificados con versión fijada | n/a (razón) |
| Test-first evidence | V-Bounce | La lista de **lo que no debe emitirse** (§A.1, derivada de las plantillas 1.4 y de ADR-003/ADR-004) se escribe **antes** de tocar las plantillas, y los tres escenarios de precedencia se escriben desde la regla de ADR-004 §3.4 antes de implementar el arranque | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de emisión producen bytes idénticos y el workspace no cambia (NFR de ADR-003 §5) | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: sin límite de proceso ni proveedor real en el alcance | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: el cambio no introduce un camino de ejecución medible; el arranque lee configuración una vez | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: ni el motor ni estas plantillas agregan telemetría (§11) | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). Los artefactos describen la estructura
  de la aplicación del consumidor (nombres de proyecto, claves de configuración) y **no** transportan datos
  personales ni de negocio.
- **El punto sensible del Bolt es la configuración, y su regla es explícita:** el archivo emitido **no** contiene
  credenciales en claro ni material de claves (ADR-004 §3.4, NFR *No embedded secrets* de ADR-007 §5). El valor
  real de la cadena de conexión **no** puede venir del modelo —el `.coop2` dejó de transportarla (ADR-002 §3.1)—
  ni escribirse en el archivo: llega por el **entorno** del despliegue o, si está protegido, por el esquema del
  **ADR-007** (Bolt 022), cuya protección **no** implementa ni invoca este Bolt.
- **El prefijo de esquema es obligatorio:** el valor de conexión declarado lleva `plain:` explícito (el valor
  sin prefijo es error tipado del contrato, Bolt 011): no hay valor que se "adivine" y no hay degradación a texto
  plano.
- **Sin `providerName`:** eliminarlo no es cosmético —era el punto por el que un valor de configuración podía
  cambiar el proveedor en runtime (ADR-005 §3.3)— y el descriptor emitido apunta a **un solo** provider.
- **El arranque no filtra valores:** lee claves y valores, y entrega el contrato al framework; **no** los
  registra, no los imprime y no los incluye en mensajes. Un valor con esquema desconocido produce el error tipado
  del contrato de configuración con la **clave** y el **prefijo** ofensores, nunca con el contenido.
- **La dependencia del canal está declarada y no se disimula:** los descriptores consumen por `PackageReference`,
  de modo que la cadena de suministro del consumidor pasa por el **feed** del release (ADR-001 §3.6). Este Bolt lo
  deja escrito como consecuencia (y su evidencia usa la resolución local); la decisión del canal y su firma son
  de su dueño.
- **Sin superficie nueva:** las plantillas emiten archivos de texto; el arranque lee un archivo **del propio
  consumidor** y variables de entorno. No hay red, ni ejecución remota, ni deserialización polimórfica.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: tanto el set de plantillas (contenido) como el arranque emitido son
  artefactos de andamiaje; **no** se les agrega instrumentación (ninguna fuente lo pide y el arranque no tiene
  ciclo de vida que observar más allá de la lectura de configuración).
- **La observabilidad de este Bolt es su evidencia:** el listado de artefactos emitidos y de lo **no** emitido, la
  compilación de los dos workspaces, las **tres observaciones** de la precedencia, la constancia de ausencia de
  credenciales, la prueba de no sobrescritura con aviso de deriva y las dos corridas de idempotencia.
- **Diagnóstico en runtime del consumidor:** el **warning de deriva** del motor cuando un artefacto editable
  difiere (ADR-003 §3.4) y los **errores tipados** del contrato de configuración (clave ausente, valor sin
  prefijo, prefijo desconocido — Bolt 011) son los dos diagnósticos que este entregable pone en juego; ninguno
  filtra contenido.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay es un **cambio del andamiaje** que el consumidor
  recibe al regenerar: el proyecto deja de ser .NET Framework con copia de ensamblados y pasa a SDK-style con
  referencias de paquete; el `App.config` deja de existir y su reemplazo es `appsettings.json` + entorno; y la
  cadena de conexión **sale del archivo** y va al entorno (o protegida por el ADR-007). Todo eso entra en las
  **notas de migración** (documento del **Bolt 028**, ADR-008 §3.1) con sus tres datos obligatorios: qué
  desaparece (los descriptores del formato anterior, `App.config`, `providerName`, el set VB, el mecanismo de
  copia de ensamblados y la metadata de publicación), por qué (ADR-003 §3.1/§3.4, ADR-004 §3.4, ADR-001 §3.1/§3.5)
  y con qué se reemplaza (descriptores SDK-style con `PackageReference`; `appsettings.json` + entorno con
  precedencia declarada).
- **Compatibility:** el consumidor que migra **edita** estos artefactos; la regla de no sobrescritura (ADR-003
  §3.4) es lo que hace que su edición sobreviva a la regeneración, con deriva advertida y `--force` explícito
  cuando decide aceptar el cambio. Los descriptores emitidos son de la **forma** que el consumidor espera de un
  proyecto moderno (SDK-style, un TFM, referencias de paquete) y compilan contra los paquetes del release.
- **Valores que el consumidor tenía en `App.config`:** su reubicación (entorno o archivo, según sensibilidad) es
  parte de las notas; la **protección** de los que hoy están ofuscados es del ADR-007/Bolt 023 y **no** se toca
  acá.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido ni artefacto del consumidor reescrito
  por el motor (los editables no se pisan y los poseídos se regeneran al volver atrás). El `1.4` congelado sigue
  siendo la contención soportada (ADR-008 §3.1).
- **Frontera del canal:** si el consumidor no tiene forma de resolver los paquetes (feed), la aplicación generada
  no compila: es la consecuencia declarada de ADR-001 §3.6 y **no** se parchea con una copia de ensamblados (eso
  sería reintroducir exactamente lo que este Bolt retira).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Credenciales en el archivo de configuración emitido** (el defecto que F-37 documentó: la cadena del modelo dentro del `App.config`) | 3 | 4 | AC-2 + gate de secretos: el archivo declara claves del catálogo con un valor **sin credenciales** y el valor real llega por el entorno; el `.coop2` ya no transporta la cadena (Bolt 002), así que no hay de dónde copiarla |
| **Una precedencia enunciada y no cumplida** (el archivo gana al entorno, o el "default" pisa el archivo) | 3 | 3 | AC-3: la precedencia se **observa** en tres escenarios sobre la aplicación generada, no se enuncia; el arranque es el único dueño de la combinación (el framework no lee archivos) |
| **Sobrescribir la edición del consumidor** (el punto donde más edita: los descriptores de proyecto) | 4 | 3 | Clasificación explícita de editables + prueba de no sobrescritura con aviso de deriva + `--force` explícito; el contraste con un artefacto poseído prueba que la clasificación no se corrió |
| **Reintroducir el consumo por copia de ensamblados o la plataforma web** (por conveniencia o por copiar una plantilla vieja) | 3 | 4 | La lista de **lo que no se emite** se verifica artefacto por artefacto y el proyecto emitido **compila** sin ninguna de las dos |
| **El esqueleto de app se emite por defecto** y fija una interfaz que el consumidor no pidió | 3 | 3 | AC-5: sin la opción, el artefacto **no existe** en el workspace; el camino por defecto produce sólo librerías |
| **Atadura al canal de paquetes** (los descriptores sólo compilan si existe el feed) | 3 | 3 | Consecuencia declarada de ADR-001 §3.6; la evidencia usa la resolución local y el canal sigue siendo decisión del `tech_lead` con fecha |
| **Claves de catálogo inventadas o desactualizadas** en el archivo emitido | 3 | 3 | Las claves salen del catálogo del Bolt 011; una clave que el catálogo no declare **no** se emite y se **pide** (§15), nunca se inventa |
| **Dependencia nueva colada en la aplicación generada** (un paquete "que hace falta") | 2 | 4 | ADR-001 §3.4 (toda dependencia se autoriza en un ADR) y el gate de dependencias sobre los descriptores emitidos |
| **El arranque filtra valores** en su salida o en sus errores | 3 | 3 | Regla explícita (§10) + gate de secretos + casos negativos: el error tipado nombra la **clave** y el **prefijo**, nunca el contenido |
| **Idempotencia rota** por orden no canónico o por finales de línea | 3 | 3 | NFR de ADR-003 §5 medido con dos corridas y con un workspace con `CRLF` (normalización antes de decidir si difiere) |
| **Colisión con el set de clases (Bolt 019) o con el de SP (Bolt 020)** sobre el mismo set embebido | 2 | 3 | Frontera declarada (§4): este Bolt entrega los descriptores, la configuración y el arranque; el resto es de esos Bolts, y el set es uno solo (Bolt 017) |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-003/ADR-004/ADR-001; acá se
> aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) El archivo de configuración y el arranque se emiten junto con el esqueleto de aplicación (bajo su opción), no en el camino por defecto** | Su destinatario es la **aplicación**: el archivo lo lee el arranque de la app y el contrato se fija al inicio de la app (ADR-004 §3.4). Emitirlos sin la opción obligaría a decidir dónde viven en un consumidor que sólo genera librerías (que provee su propio host). **Alternativa:** emitirlos siempre junto a la solución de librerías; si el aprobador lo prefiere, se agrega (y se cambia la evidencia de AC-5, que hoy verifica ausencia sin la opción) |
| **(⚠ aprobador) La precedencia se demuestra sobre la aplicación generada dentro del repositorio (compilada y observada), no en un entorno** | Es la lectura que el `AITL-BOLT-READY-Approval` del Bolt evaluó y confirmó (`test_integration: not-required` con la nota de que, si exigiera un entorno real, el valor correcto sería `required`). La observación es de tres escenarios, con un punto verificable por nivel. Si el aprobador cambia de opinión, es un **Bolt amendment** + revisión de este SPEC (y el plan pasa a `applicable`) |
| La plantilla del archivo emite un valor de conexión **sin credenciales** con `plain:` explícito y deja el valor real al entorno | El modelo **no** transporta la cadena (ADR-002 §3.1) y ADR-004 §3.4 prohíbe credenciales en un archivo versionado; el `plain:` explícito es la regla de ADR-004 §3.3 (sin prefijo = error tipado) |
| El archivo declara **sólo** claves del catálogo del framework | Las claves del framework son contrato (Bolt 011); lo propio del consumidor va en **su** lado, y el archivo es suyo (y editable). Así no se inventan claves ni se emiten las del 1.4 que ya no existen |
| La lista de lo que **no** se emite se entrega como evidencia explícita (tabla de §A.1) | Es la forma de probar que la forma anterior **desapareció**: un listado de lo emitido no distingue "no lo emito" de "me olvidé de quitarlo"; el listado de ausencias, sí |
| El test de propiedad de archivos incluye el **contraste** con un artefacto poseído | Sin el contraste, una clasificación corrida (todo "editable" o todo "poseído") pasaría la prueba; el contraste prueba que la clasificación es la que ADR-003 §3.4 ordena |
| El arranque **no** se implementa como dependencia del framework | El framework sólo consume el contrato y su default lee variables de entorno (ADR-004 §3.2): leer un archivo es de la **app**, no del framework; agregarlo al framework sería meterle una política de archivos y posiblemente una dependencia nueva (§4, ADR-001 §3.4) |
| La ruta física y la extensión de las plantillas dentro del set no se fijan acá | Son del **mecanismo** del set (Bolt 017, ADR-003 §3.5): este Bolt entrega **contenido** y **nombres lógicos** dentro del set conocido. Fijarlas acá crearía una segunda convención |
| El esqueleto de app queda funcional y mínimo, sin diseño de UI | ADR-003 §3.1: el camino por defecto no fija UI y la interfaz del consumidor no es alcance de este Bolt; un esqueleto elaborado sería trabajo sin dueño ni requisito |
| La evidencia incluye la **compilación** de lo generado y no sólo su inspección | Un descriptor "de forma moderna" que no compila no sirve al consumidor: la compilación contra los paquetes del release es la prueba de que el cambio de modelo de consumo está completo |

---

## 15. Stop conditions

- **El contrato de configuración y su catálogo (Bolt 011) no están en el árbol 2.0**: parar y reportar. La salida
  es una enmienda de dependencia o una decisión de orden del dueño; **nunca** inventar claves ni emitir el archivo
  con las claves del 1.4.
- **El motor y su set embebido (Bolt 017) no están** o no exponen el punto donde el set entrega estas plantillas:
  parar y reportar; este Bolt entrega **contenido**, no mecanismo.
- **El catálogo del framework no declara una clave que el archivo necesitaría** (por ejemplo un default propio
  sin clave): parar y pedirla al Bolt 011. El default **no** vive escondido en la plantilla (misma regla que la
  cache en el Bolt 013).
- **Los paquetes de la 2.0 no son resolubles** para compilar lo emitido: parar y reportar. Un descriptor que no
  compila no es evidencia, y **no** se parchea con `HintPath` ni copiando ensamblados: eso es exactamente lo que
  el Bolt retira.
- **Aparece la necesidad de que el framework lea el archivo de configuración** (o de mover la precedencia al
  framework): parar. ADR-004 §3.2/§3.4 lo prohíbe: el framework consume el contrato; el archivo lo lee la app.
- **Se pretende incluir una credencial real** (o un valor protegido) en el archivo emitido: parar. La regla es
  dura (ADR-004 §3.4 + NFR *No embedded secrets*): el valor va por el entorno o por el esquema protegido del
  ADR-007.
- **Se pretende emitir una dependencia nueva** en la aplicación generada (un lector de JSON, un toolkit de UI):
  parar: ADR-001 §3.4 exige un ADR que la autorice.
- **El esqueleto de app aparece en el camino por defecto** (o se pide que se emita siempre): parar y reportar;
  es el criterio de ADR-003 §3.1 y cambiarlo es una decisión del set, no una conveniencia.
- **La lista de rupturas no se puede describir con su reemplazo** (ADR-008 §3.1): parar; una ruptura sin
  reemplazo declarado no entra al registro.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-003 §3.1/§3.3/§3.4/§3.5, ADR-004 §3.2/§3.3/§3.4, ADR-001 §3.1/§3.4/§3.5/§3.6)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-021`](../functional/bolts/US-000.BOLT-021-plantillas-de-proyectos.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-37 (las plantillas de proyecto y de configuración emiten el formato anterior, con `System.Web`, copia de ensamblados y la cadena en el archivo), F-36 (metadata de publicación/ClickOnce), F-13 (configuración clásica) y §6 ítems 9 y 15.
- [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.1 (artefactos del set: proyectos SDK-style `net10.0` con `PackageReference`; esqueleto de app detrás de un flag; sin VB; el motor no ejecuta SQL), §3.3 (determinismo e idempotencia), §3.4 (artefactos editables: no sobrescritura, deriva advertida, `--force`), §3.5 (plantillas como datos, set embebido, overrides), §5 (NFRs).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato `ICooperatorConfiguration`, catálogo de claves y default por variables de entorno), §3.3 (prefijo de esquema explícito, error tipado, `providerName` fuera), §3.4 (`appsettings.json` + entorno, precedencia **entorno > archivo > default** propiedad del arranque de la app, claves del catálogo, sin credenciales en un archivo versionado).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (consumo por `PackageReference`, conjunto de paquetes y **folder feed**), §3.4 (toda dependencia se autoriza en un ADR), §3.5 (sin ClickOnce, sin VB), §3.6 (el canal de distribución como decisión pendiente con dueño y fecha).
- [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) — el motor headless y el set de plantillas embebido donde vive este contenido (prerrequisito, Bolt 017).
- [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) — el contrato de configuración y su catálogo (prerrequisito, Bolt 011).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés y build compartido (prerrequisito, Bolt 001).
- [`SPEC-261016-0029`](./SPEC-261016-0029-plantillas-csharp.md) · [`US-000.BOLT-020`](../functional/bolts/US-000.BOLT-020-plantillas-stored-procedures.md) — los otros dos sets de plantillas (frontera declarada).
- [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) — el contrato del `.coop2` que el motor consume para iterar el modelo (prerrequisito transitivo, Bolt 004).
- [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) · [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) — la protección de valores (frontera: el archivo no lleva material de claves), el documento de notas y el shell del Modeler.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-021`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, inventario de **lo que se emite y de lo que no se emite** (descriptores del formato anterior, `System.Web`, copia de ensamblados, metadata de publicación, `App.config` con la cadena), estrategia de pruebas (contenido por artefacto, compilación de los dos workspaces, precedencia nivel por nivel, propiedad de archivos, idempotencia), gates, control de datos `internal` y condiciones de parada. Declara la frontera con los Bolts 017/019/020 (set), 011 (catálogo), 022 (protección de valores) y 001 §3.6 (canal). `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (arquitectura / tooling)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
