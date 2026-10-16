---
id: "SPEC-261016-0029"
title: "Set de plantillas C#: `*.Auto.cs`, partials editables, propiedad incondicional y retiro de los atributos de contrato"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítems 3, 6 y 15 (hallazgos F-37 y F-43) → ADR-003 / ADR-008 §3.7
bolt: "US-000.BOLT-019" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-003", "ADR-008", "ADR-002", "ADR-005", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y artefactos dorados de la generación 1.4 (Bolt 001)
  - "SPEC-261016-0023-motor-headless.md" # el motor, su resolución de set, su proyección y su regla de propiedad (Bolt 017)
  - "SPEC-261016-0010-contrato-coop2-en-modeling.md" # el contrato del modelo y el recálculo de derivados que la plantilla recibe (Bolt 004)
  - "SPEC-261016-0004-particion-modeling.md" # los POCOs del modelo y la fuente única del mapeo SQL↔CLR (Bolt 003)
risk_class: "medium" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L3" # default por riesgo low/medium (§3.3)
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "internal" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: not-required` → `n/a`
  applicability: "n/a"
  tests: []
  environment: ""
  data: ""
  reason: "Bolt `not-required` (refactor): lo que se entrega es el conjunto de plantillas y el código que emiten, verificable sobre su referencia dorada; no toca base de datos ni servicio externo."
  waiver_adr: ""
review_ready_at: "2026-10-16T00:35:59-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:35:59-03:00"
  decided_at: "2026-10-16T00:35:59-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-019 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent; su acknowledgment «Dueño del criterio ADR-008 3.7 confirmado para las clases C# (019); 020/021 no lo son»), ADR-003/ADR-008/ADR-002/ADR-005/ADR-001 accepted (tech_lead), REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → applicability: n/a con razón; set como datos + manifiesto (trazo artefacto↔plantilla↔clase de propiedad); artefactos propios (Entity/Object/Mapper/Gateway + DefaultLazyProvider + RuleExample) sin plataforma web ni copia de ensamblados, con cabecera fija sin fecha; partials editables (creación-si-falta, nunca sobrescritura, deriva advertida, --force explícito)."
    - "ADJUDICACIÓN (dueño del criterio ADR-008 §3.7) — CONFIRMADO = BOLT-019: ADR-008 §3.7 fija «dueño: el Bolt del set de plantillas» (el que ejecuta las plantillas del motor). Para las clases C# —donde viven [DataContract]/[CollectionDataContract]/[Serializable]— es BOLT-019; BOLT-020 (stored procedures) y BOLT-021 (proyectos/configuración) emiten otros tipos de artefacto y NO son dueños de ese criterio. El acknowledgment del Bolt lo confirma explícitamente."
    - "Compliant — determinismo byte a byte + paridad diferencia-por-diferencia contra el código dorado 1.4 con lista versionada (declared-differences.json) contra la que el test falla ante una diferencia funcional no declarada; los derivados llegan calculados y no se emiten como estado."
    - "CONFIRMACIÓN (frontera del proyecto de verificación) — CORRECTA: el proyecto que compila el árbol emitido (2.0/tests/Cooperator.CSharpTemplates.Compilation.Tests/) es de verificación (scope de tests); los proyectos definitivos del consumidor son del BOLT-021. El 019 demuestra que el C# emitido compila contra los paquetes 2.0 sin producir los csproj finales del consumidor (frontera declarada y correcta)."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). El conjunto de artefactos del set C#, la regla de propiedad de archivos y la
  prohibición de los atributos de contrato están decididos en ADR-003 §3.1/§3.4/§3.5, ADR-008 §3.2/§3.7 y
  ADR-005 §3.3. Este SPEC los ejecuta y los vuelve verificables, sin reabrir la decisión.
  ⚠️ CRITERIO VINCULANTE CON DUEÑO (ADR-008 §3.7): el retiro de los atributos de contrato de serialización del
  código generado es criterio de aceptación de ESTE Bolt (el set que emite esas clases); su MEM lo registra como
  ruptura con su entrada en las notas de migración.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de
  dominio aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada
  invalida la aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0029 — Set de plantillas C#: `*.Auto.cs`, partials editables, propiedad incondicional y retiro de los atributos de contrato

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítems 3, 6 y 15 (hallazgos F-37 y F-43) |
| **Bolt** | [US-000.BOLT-019](../functional/bolts/US-000.BOLT-019-plantillas-csharp.md) |
| **ADRs** | [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1, §3.4, §3.5 · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) §3.2, §3.7 · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.1 · [ADR-005](../adrs/ADR-005-provider-de-datos.md) §3.3 · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1, §3.2 |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se entrega el **set de plantillas del código C# generado**: las clases que el motor **reescribe siempre**
(los `*.Auto.cs` de entidad, objeto, mapeo y acceso) y las **mitades editables** por el humano (los partials sin
`.Auto.`, el ejemplo de regla y el proveedor perezoso por defecto), con las entidades emitidas como **objetos
simples sin atributos de contrato de serialización** y con la regla de convivencia con el código del consumidor
aplicada de forma **incondicional**.

El problema que resuelve está verificado en dos frentes: **(1)** F-37 — las plantillas de 1.4 emiten proyectos
.NET Framework con `System.Web` y copian los ensamblados a `CooperatorAssemblies`, de modo que migrar las
librerías sin reescribir lo que se **emite** no entregaría el resultado de negocio; y **(2)** F-43/ADR-008 §3.2 y
§3.7 — el código generado declara atributos de contrato de serialización (`[DataContract]`/
`[CollectionDataContract]` en los `*.Auto.cs`) y la marca de serialización binaria en los partials
(`[Serializable]`), que en la 2.0 son **superficie muerta**: el framework ya no respalda ningún mecanismo de
serialización (ADR-008 §3.3), así que el atributo sólo puede inducir a un consumidor a construir sobre algo que
nadie sostiene.

Además, la **propiedad de archivos** deja de depender del modo de corrida: en 1.4 un `CopyFolder` con el modo
completo sobrescribía **también** los archivos editables a mano. Si este Bolt no se implementa, el consumidor
recibiría código que no compila en el runtime objetivo, que promete serialización inexistente y que puede perder
su trabajo.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó los ítems 3/6/15 hacia ADR-003 (motor y set) y
ADR-008 (§3.7). Ambos están `accepted`: ADR-003 §3.1 fija los artefactos del set y la forma de las entidades
(objetos simples, cabecera fija sin fecha), §3.4 la regla **incondicional** de propiedad, §3.5 las plantillas como
datos con versión y rango de `schemaVersion`; ADR-008 §3.7 establece, como **criterio de aceptación vinculante**
del Bolt del set de plantillas, que el código generado **no** emite atributos de contrato de serialización, y
§3.2 la caída de `[Serializable]` de los partials de plantilla. Este SPEC **no decide nada nuevo**: fija el set, la
referencia de paridad y las verificaciones.

**El set legacy que se reescribe (evidencia verificada en esta sesión)** — `Tools/CooperatorModeler/Templates/`:

| Artefacto legacy | Qué emite hoy | Destino en la 2.0 |
|---|---|---|
| `CSharpClasses/Entity.Auto.cs` | entidad con `[DataContract()]` en la clase (L33) y `[CollectionDataContract()]` en `<E>List` (L451) y en `<E>ListView` (L461) | mismo artefacto, **sin atributos de contrato** (criterio ADR-008 §3.7) |
| `CSharpClasses/Object.Auto.cs` | objeto de datos con `[DataContract()]` (L22) y dos `[CollectionDataContract()]` (L414, L425) | mismo artefacto, sin atributos de contrato |
| `CSharpClasses/Entity.cs` · `Object.cs` | **partial editable** con `[Serializable]` (L20) | mismo partial, **sin la marca binaria** (ADR-008 §3.2) |
| `CSharpClasses/Mapper.Auto.cs` · `Mapper.cs` · `Gateway.Auto.cs` · `Gateway.cs` | mapeo y acceso (reescrito + editable) | mismos artefactos, con la **forma del contrato preservada** (ADR-005 §3.3) |
| `CSharpClasses/DefaultLazyProvider.cs` | proveedor perezoso por defecto | mismo artefacto (el **mecanismo** de carga es del Bolt 005; acá sólo el artefacto) |
| `CSharpClasses/RuleExample.cs` | ejemplo de regla (editable) | mismo artefacto, **nunca** sobrescrito |
| (`CSharpProjects/**`, `ConfigFiles/**`) | proyectos y `App.config` | **fuera de alcance**: Bolt 021 |

Del contrato que el código generado compila se conocen además las bases y las interfaces que las plantillas
referencian (`Objects.<E>Object`, `IMappeable<E>`, `IEquatable<E>`, `ICloneable`, `ObjectList<E>`,
`ObjectListView<…>`) y la **forma** que se preserva por ADR-005 §3.3 (BCL-typed en el borde público del acceso).

**Referencia de paridad.** El código generado por el Modeler 1.4 (con `CodeGenerator.GenerateClasses` sobre el
`Snapshot` congelado) es el **artefacto dorado** que el Bolt 001 deja versionado en
`2.0/tests/fixtures/golden-generation/**`; ADR-003 §4 aclara por qué no se puede comparar contra el **motor**
legacy en ejecución (depende de CodeDOM, que no existe en el runtime objetivo). La paridad se declara **diferencia
por diferencia**, no por impresión.

**Estado del árbol 2.0.** `2.0/` todavía no existe en el repositorio: lo crea el Bolt 001. El motor (Bolt 017,
[SPEC-261016-0023](./SPEC-261016-0023-motor-headless.md)) ya tiene SPEC en disco y es la dependencia directa: sus
plantillas son **datos** que ese motor interpreta, y la superficie de datos que reciben la fija su proyección.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-008 §3.7 ejecuta lo que ADR-003 §3.1 ya declara —entidades como
POCOs— y ADR-005 §3.3 fija la forma del contrato que se preserva), y el plan de integración declarado (`n/a`)
coincide con el `test_integration: not-required` del Bolt (§3.2.2). No se emite reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-019` (`devflow/functional/bolts/US-000.BOLT-019-plantillas-csharp.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:09:32-03:00; `risk_class: medium`, `test_integration: not-required`; su DoR confirma que **este** Bolt es el dueño del criterio de ADR-008 §3.7 para las clases C# |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-003` §3.1 (artefactos del set, entidades como objetos simples, cabecera fija, datos derivados calculados), §3.3 (determinismo), §3.4 (propiedad incondicional), §3.5 (plantillas como datos, set y overrides) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T22:32:47-03:00 |
| ADR (gobernante · criterio vinculante) | `ADR-008` §3.7 (el código generado **no** emite atributos de contrato de serialización; dueño: el Bolt del set) y §3.2 (retiro de la marca binaria de los partials) y §3.1 (ruptura documentada con su reemplazo) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-002` §3.1 (los derivados se recalculan al leer: la plantilla los **recibe**) · `ADR-005` §3.3 (la **forma** del contrato que consume el código generado se preserva) · `ADR-001` §3.1/§3.2 (los artefactos generados consumen paquetes de la 2.0, sin copia de ensamblados) | AITL-ADR-Approval ✓ — `accepted` (2026-10-15) |
| REV evidence | `REV-001` §4 F-37, F-43 y §6 ítems 3, 6, 15 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0000-…` (arnés y dorados 1.4), `SPEC-261016-0023-…` (motor), `SPEC-261016-0010-…` (contrato del modelo), `SPEC-261016-0004-…` (POCOs y mapeo) | Los cuatro **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (nada bajo `2.0/` ni bajo `1.4/` en el árbol de trabajo) |

---

## 4. Scope

### In scope

1. **El set de plantillas C#**, como datos versionados y embebidos en el paquete del motor (el **mecanismo** de
   resolución es del Bolt 017), con su manifiesto: los `*.Auto.cs` de entidad, objeto, mapeo y acceso; los
   partials editables correspondientes; el ejemplo de regla; y el proveedor perezoso por defecto. Cada plantilla
   consume los datos del modelo **ya calculados** por el contrato (derivados, predicados y estado no se persisten
   ni se emiten como estado, ADR-002 §3.1).
2. **Las entidades como objetos simples** y el retiro de los **atributos de contrato de serialización**
   (`[DataContract]`, `[CollectionDataContract]`) del código emitido — criterio de aceptación **vinculante** de
   ADR-008 §3.7, con su constancia de ruptura— y de la **marca de serialización binaria** (`[Serializable]`) de los
   partials de plantilla (ADR-008 §3.2). Cambio **sólo de atributos**: no toca nombres, miembros, firmas ni
   comportamiento del código generado.
3. **La regla de propiedad incondicional**: los artefactos que el motor posee se reescriben siempre; los
   editables se crean sólo si faltan, **nunca** se sobrescriben, su deriva se **advierte** y forzar es una acción
   explícita. La regla no depende del modo de corrida.
4. **La cabecera fija** de cada artefacto generado (herramienta, versión del motor, versión del set,
   `schemaVersion` del modelo y el partial editable que lo acompaña) y la **ausencia de marcas temporales**.
5. **La referencia dorada y la paridad funcional** contra el código generado por el Modeler 1.4, con **cada
   diferencia declarada y atribuida** al runtime objetivo (sin la plataforma web, sin atributos de contrato, con
   referencias de paquete en lugar de copia de ensamblados).
6. **La verificación de que el código emitido compila** contra los paquetes de la 2.0, sin la dependencia de la
   plataforma web, y que una **edición a mano sobrevive** a la regeneración.
7. **La constancia** de las rupturas que este Bolt introduce (ADR-008 §3.1), como insumo de las notas de
   migración (Bolt 028).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **motor**, su resolución de set, la proyección y la propiedad de archivos como **mecanismo** | Bolt 017 (SPEC-0023) | El Bolt lo excluye: acá se escriben las plantillas y su referencia dorada |
| El set de **stored procedures** | Bolt 020 | Otro conjunto de artefactos |
| Las plantillas de **proyectos y configuración** (`*.csproj`, `*.sln`, `appsettings.json`, bootstrap) | Bolt 021 | Otro conjunto de artefactos (ADR-003 §3.4, F-37) |
| El **mecanismo de carga** de los proveedores perezosos y los nombres de provider | Bolt 005 (ADR-006) | Acá sólo se emite el artefacto del proveedor por defecto |
| El **provider de datos** y la forma del borde público del acceso | Bolt 014 / ADR-005 §3.3 | La **forma** se preserva, no se rediseña |
| **Completar** los partials del consumidor o su lógica de negocio | — (nadie) | El motor no escribe lo editable |
| Agregar dependencias o paquetes satélite al set | — (nadie) | El Bolt lo excluye y ADR-003 §3.6 acota las dependencias |
| La **modernización del T-SQL** emitido | Bolt 020 | Es del set de stored procedures |
| La **migración de datos** del consumidor o su esquema | — (nadie) | Fuera del alcance de la migración de código |

**Regla de aporte (bounded, explícita).** Este Bolt **escribe plantillas** y su referencia dorada; no crea
infraestructura. Si la superficie de datos que la proyección del motor expone no alcanza para un artefacto, se
**detiene** y se reporta: es una brecha del Bolt 017 o del contrato del modelo, y ampliarla no es de este Bolt.

---

## 5. Prerequisites and baseline

- **Bolt 017 y su SPEC** (`SPEC-261016-0023-…`): el motor que **interpreta** estas plantillas, el manifiesto y la
  resolución del set, la proyección del modelo, el contexto endurecido y la regla de propiedad como mecanismo.
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el arnés y los **artefactos
  dorados de la generación 1.4** (`2.0/tests/fixtures/golden-generation/**`), que son la referencia de paridad.
- **Bolt 004 y su SPEC** (`SPEC-261016-0010-…`): el **contrato del modelo** (`Coop2Reader`, recálculo de
  derivados, política de `schemaVersion`) que determina qué datos recibe cada plantilla.
- **Bolt 003 y su SPEC** (`SPEC-261016-0004-…`): los **POCOs** y la fuente única del mapeo SQL↔CLR que el código
  generado consume indirectamente a través del contrato.
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error. La verificación es `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`.
- **Estado del árbol 2.0 al escribir este SPEC:** `2.0/` todavía no existe (lo crea el Bolt 001) y el motor llega
  con el Bolt 017, dependencia declarada.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline antes y después de construir; sus plantillas
  legacy son **evidencia**, no un insumo que se edite.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-019`), **2 V-Bounces** esperados. El orden va del inventario al cierre: primero el
> set y su manifiesto, después los artefactos propios, después los editables y el retiro de atributos, y al final
> la paridad y la compilación.

### Phase A — El set C# como datos y su manifiesto

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### A.1 Inventario del set y su clase de propiedad

Se declara el inventario del set —qué plantilla produce qué artefacto y **quién es su dueño** (el motor, o el
humano)— a partir del set legacy como referencia funcional: `Entity.Auto.cs`/`Entity.cs`,
`Object.Auto.cs`/`Object.cs`, `Mapper.Auto.cs`/`Mapper.cs`, `Gateway.Auto.cs`/`Gateway.cs`,
`DefaultLazyProvider.cs` y `RuleExample.cs`. El inventario entra al **manifiesto del set** (nombre, versión propia,
rango de `schemaVersion` que entiende, plantillas conocidas y artefactos que produce), que es lo que el motor del
Bolt 017 consume para resolver, reportar y limpiar huérfanos.

#### A.2 Las plantillas como datos y su contrato de entrada

Cada plantilla se escribe como **dato** (Scriban interpretado, sin compilación ni C# empotrado) y consume
**exclusivamente**: el modelo proyectado (datos, no objetos por reflexión), los **derivados ya calculados** por el
contrato del modelo (nombres generados, namespaces, tipo CLR/nativo, campos de descripción y clave) y los helpers
de la lista blanca del motor. **Ninguna** plantilla consulta el sistema de archivos, la red o el runtime, ni
redefine datos del modelo.

**Files created:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/` — las plantillas del set C# (una por artefacto)
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/templates.manifest.json` — inventario, versión del set y rango de `schemaVersion`
- `2.0/docs/template-set-csharp.md` — el inventario publicado: artefacto, plantilla, clase de propiedad y datos de entrada

---

### Phase B — Los artefactos que el motor posee (`*.Auto.cs`)

**Duration:** ~4h de ciclo — **Complexity:** High

#### B.1 Los cuatro artefactos reescritos

Se escriben las plantillas de los artefactos del motor: entidad, objeto de datos, mapeo y acceso
(`Entity.Auto.cs`, `Object.Auto.cs`, `Mapper.Auto.cs`, `Gateway.Auto.cs`), más el proveedor perezoso por defecto y
el ejemplo de regla en su parte que el motor posee. La equivalencia funcional con 1.4 se mantiene: las clases
conservan los **nombres, miembros y firmas** que el consumidor compila, y las bases e interfaces del contrato
(`Objects.<E>Object`, `IMappeable<E>`, `ObjectListView<…>`, `ObjectList<E>`, `IEquatable<E>`, `ICloneable`)
siguen siendo las mismas, sobre la **forma** que ADR-005 §3.3 preserva.

#### B.2 Sin la plataforma web, sin copia de ensamblados

El código emitido **no** referencia `System.Web` (F-37) ni copia ensamblados a una carpeta de consumo: los
proyectos generados consumirán los **paquetes** de la 2.0 (el proyecto en sí es del Bolt 021, pero la plantilla no
puede emitir ninguna dependencia de plataforma ni de copia de binarios).

#### B.3 La cabecera fija

Cada artefacto emitido lleva la cabecera fija de ADR-003 §3.3: herramienta + versión del motor, versión del set,
`schemaVersion` del modelo y el nombre del partial editable que lo acompaña. **Sin** `Created: <fecha>` —la marca
temporal del legacy (`-- Created: …`, `DateTime.Now`) es lo que hace imposible que dos corridas den bytes
idénticos— y **sin** el digest del modelo (que va en el reporte, no en cada archivo).

**Files created:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/{Entity,Object,Mapper,Gateway}.Auto.scriban` — los artefactos propios
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/{DefaultLazyProvider,RuleExample}.Auto.scriban` — las partes propias del proveedor por defecto y del ejemplo de regla
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/_layout/header.scriban` — la cabecera fija compartida

---

### Phase C — Los partials editables y la regla incondicional

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### C.1 Los artefactos que el humano posee

Se escriben las plantillas de los artefactos **editables**: los partials `Entity.cs`, `Object.cs`, `Mapper.cs`,
`Gateway.cs`, el `RuleExample.cs` y la parte editable del proveedor por defecto. Su contrato de emisión es el de
ADR-003 §3.4: se **crean sólo si faltan** y **nunca** se sobrescriben; si el contenido calculado difiere del
archivo existente, el motor deja el archivo intacto y registra un **warning de deriva** (que el Bolt 017 hace
visible en el resultado y el reporte), y forzar es una acción **explícita**. No hay modo de corrida que cambie
esto.

#### C.2 El partial como contrato de extensión

Los partials emitidos son la mitad editable de la misma clase: se emiten con la forma mínima (declaración del
partial, espacios de nombres y los puntos de extensión documentados) y **nada** de lógica de negocio del
consumidor. La prueba de la regla es directa: una edición a mano sobrevive a la regeneración.

**Files created:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/{Entity,Object,Mapper,Gateway}.scriban` — los partials editables
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/RuleExample.scriban` — el ejemplo de regla (editable)
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/CSharpOwnershipTests.cs` — propios reescritos, editables intactos, deriva advertida y `--force`

---

### Phase D — El retiro de los atributos de contrato y de la marca binaria

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### D.1 El criterio vinculante (ADR-008 §3.7)

Las plantillas de los `*.Auto.cs` **dejan de emitir** `[DataContract]` y `[CollectionDataContract]` (el legacy los
ponía en la entidad, en el objeto de datos y en sus dos tipos de lista y de vista). El cambio es **sólo el retiro
de atributos**: no toca nombres, miembros, firmas ni comportamiento del código generado, así que **no altera el
contrato que el consumidor compila** (ADR-008 §3.7 lo dice expresamente). Un consumidor que necesite atributos de
contrato los declara **en su lado** (sus partials), que es donde el framework ya no impone un mecanismo
(ADR-008 §3.3).

#### D.2 La marca de serialización binaria en los partials

Las plantillas de los partials editables **dejan de emitir** `[Serializable]` (el legacy lo ponía en el partial de
la entidad y en el del objeto de datos): son POCOs, y la marca prometía una serialización que el runtime objetivo
no puede cumplir (ADR-008 §3.2/§3.4).

#### D.3 La constancia de la ruptura

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-019.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1 por entrada: **qué** deja de emitirse (los atributos, con su artefacto), **por qué** (ADR-008
§3.2/§3.7 sobre ADR-003 §3.1: el framework no expone ningún serializador) y **con qué se reemplaza** (nada: el
consumidor declara su mecanismo en su lado; el código generado no cambia de forma). El MEM registra el cambio,
como pide el criterio del ADR.

**Files modified:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/{Entity,Object}.Auto.scriban` — sin atributos de contrato
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/CSharp/{Entity,Object}.scriban` — sin la marca binaria

**Files created:**
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/GeneratedSurfaceTests.cs` — barrido de lo emitido: 0 `[DataContract]`/`[CollectionDataContract]`/`[Serializable]`
- `2.0/docs/api-removals/US-000.BOLT-019.json` — constancia de la ruptura (insumo del Bolt 028)

---

### Phase E — Determinismo y paridad contra el código dorado 1.4

**Duration:** ~3h de ciclo — **Complexity:** High

#### E.1 Determinismo byte a byte

Dos corridas del mismo modelo producen el mismo árbol: sin marcas temporales, con orden canónico de artefactos y
de colecciones internas (por nombre, ordinal), con `LF` y sin espacios finales (garantías del motor que estas
plantillas no pueden romper: la cabecera es fija y ninguna plantilla emite la hora). La evidencia es el `git diff`
vacío y la ausencia de escrituras, incluido un *checkout* Windows con `core.autocrlf=true`.

#### E.2 Los derivados llegan calculados y no se emiten como estado

Los datos derivados del modelo (nombres generados, espacios de nombres, predicados de colección, campos de
descripción y clave) los calcula el contrato al leer (ADR-002 §3.1) y la plantilla los **recibe**: ninguna
plantilla los recalcula ni los emite como estado persistido (lo que además duplicaría el dato y ensuciaría el
diff del consumidor).

#### E.3 La paridad, diferencia por diferencia

Sobre el modelo de referencia se compara el árbol emitido contra el **código generado dorado de 1.4** y se
declara cada diferencia con su motivo: (a) **atributos de contrato retirados** (criterio ADR-008 §3.7);
(b) **marca de serialización binaria retirada** de los partials (ADR-008 §3.2); (c) **cabecera sin fecha**
(determinismo, ADR-003 §3.3); (d) **sin dependencia de la plataforma web** y sin copia de ensamblados (F-37,
ADR-001 §3.1); (e) cualquier ajuste de forma exigido por el contrato del modelo o por el runtime objetivo. Lo que
**no** puede diferir es la función: nombres, miembros, firmas y el comportamiento observable de las clases
generadas.

**Files created:**
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/CSharpDeterminismTests.cs` — dos corridas byte a byte, 0 escrituras, sin marcas temporales
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/CSharpGoldenParityTests.cs` — paridad contra `2.0/tests/fixtures/golden-generation/**` con las diferencias declaradas
- `2.0/tests/fixtures/csharp-parity/declared-differences.json` — las diferencias declaradas y su motivo (una por una)

---

### Phase F — El código emitido compila contra los paquetes de la 2.0

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### F.1 Compilación del código generado

El árbol emitido sobre el modelo de referencia se compila **contra los paquetes de la 2.0** en un proyecto de
verificación (los proyectos definitivos del consumidor son del Bolt 021): la compilación es el criterio que
demuestra que el código generado es propio del runtime objetivo, y falla si una plantilla reintroduce una
dependencia de plataforma. Se verifican además dos ausencias sobre lo emitido: **0** referencias a la plataforma
web y **0** atributos de contrato o de serialización binaria.

#### F.2 Una edición a mano sobrevive

Se edita a mano un partial emitido, se vuelve a generar y se comprueba que: el archivo **no** cambió, apareció el
**warning de deriva** en el resultado/reporte, y forzar explícitamente sí lo reemplaza. Es la demostración de la
regla incondicional desde el consumidor.

**Files created:**
- `2.0/tests/Cooperator.CSharpTemplates.Compilation.Tests/` (proyecto de verificación) — el árbol emitido se compila contra los paquetes 2.0
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/GeneratedCodeCompilesTests.cs` — invoca la generación y compila el resultado (o falla nombrando el artefacto)
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/EditableEditSurvivesTests.cs` — la edición a mano sobrevive y su deriva se advierte

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-003/ADR-008/ADR-002/ADR-005/ADR-001, expresados como Given/When/Then verificables.

### AC-1: El set cubre los artefactos C# y declara su propiedad

**Given** el manifiesto del set y el inventario publicado
**When** se generan los artefactos sobre el modelo de referencia
**Then** el set produce **todos** los artefactos C# previstos (los `*.Auto.cs`, los partials, el ejemplo de regla y
el proveedor por defecto) y cada uno está declarado en el manifiesto con su **clase de propiedad** (el motor lo
reescribe / es editable).

### AC-2: Sin atributos de contrato ni marca binaria (criterio vinculante ADR-008 §3.7)

**Given** el árbol emitido
**When** se barre lo emitido
**Then** hay **0** ocurrencias de `[DataContract]`, `[CollectionDataContract]` y `[Serializable]`, las entidades
son objetos simples y el cambio se registró como **ruptura** con su entrada para las notas de migración.

### AC-3: La regla de propiedad es incondicional

**Given** un workspace con artefactos propios y editables
**When** se regenera (y cuando una edición a mano cambia un partial)
**Then** lo propio se reescribe siempre; lo editable **nunca** se sobrescribe y su deriva se **advierte**; forzar
es explícito; y el comportamiento **no** cambia según el modo de corrida.

### AC-4: Determinismo

**Given** el modelo de referencia
**When** se corre la generación dos veces (incluido un *checkout* con `core.autocrlf=true`)
**Then** los archivos son idénticos **byte a byte**, `git diff` queda vacío, no hubo escrituras y ninguna cabecera
contiene una fecha de generación.

### AC-5: El código emitido compila en el runtime objetivo

**Given** el árbol emitido sobre el modelo de referencia
**When** se compila contra los paquetes de la 2.0
**Then** compila sin errores, con **0** referencias a la plataforma web y **0** copias de ensamblados.

### AC-6: Paridad funcional con el dorado, diferencia por diferencia

**Given** el código generado dorado de 1.4 (fixtures del Bolt 001)
**When** se compara con el árbol emitido
**Then** cada diferencia está en `declared-differences.json` con su motivo y su decisión, y **no** hay diferencias
de función (nombres, miembros, firmas y comportamiento observable) sin declarar.

### AC-7: Los derivados llegan calculados y no se emiten como estado

**Given** una entidad del modelo de referencia
**When** se inspecciona lo emitido
**Then** los datos derivados (nombres generados, namespaces, predicados, campos de clave y descripción) aparecen
**usados** por la plantilla y **no** persistidos como estado en el artefacto.

### AC-8: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde y
`git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "el set cubre los artefactos C# del motor y el mismo modelo produce siempre los mismos bytes" | Fases A y E.1: inventario + determinismo | AC-1, AC-4 · `CSharpDeterminismTests` |
| "las clases generadas **compilan** contra los paquetes de la 2.0 sin la dependencia de la plataforma web" | Fase F.1: compilación del árbol emitido | AC-5 · proyecto de verificación de compilación |
| "las entidades emitidas son objetos claros **sin atributos de contrato**… (criterio vinculante ADR-008 §3.7)" | Fase D.1/D.3: retiro de atributos + constancia | AC-2 · `GeneratedSurfaceTests` + fragmento de rupturas |
| "ningún artefacto del set emite la marca de serialización binaria" | Fase D.2: retiro de `[Serializable]` de los partials | AC-2 · barrido de lo emitido |
| "la cabecera fija identifica herramienta, versión del motor, versión del set y `schemaVersion`, **sin fecha**" | Fase B.3: cabecera fija compartida | AC-4 · inspección de la cabecera |
| "los artefactos que el motor posee se reescriben siempre y los editables **nunca** se sobrescriben… de forma incondicional" | Fase C.1 + regla del motor (Bolt 017) | AC-3 · `CSharpOwnershipTests`, `EditableEditSurvivesTests` |
| "el comportamiento funcional es equivalente al del código generado de 1.4" | Fase E.3: paridad declarada diferencia por diferencia | AC-6 · `CSharpGoldenParityTests` + `declared-differences.json` |
| "los datos derivados del modelo llegan calculados y no se emiten como estado persistido" | Fase A.2/E.2: la plantilla recibe y no recalcula | AC-7 · inspección del artefacto |
| "la entrada propia de las rupturas… registrada en las notas de migración en el mismo acto" | Fase D.3: fragmento + MEM | AC-2 · fragmento `api-removals/US-000.BOLT-019.json` |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Inventario y manifiesto:** el set produce exactamente los artefactos declarados, con su clase de propiedad; el
  manifiesto declara versión del set y rango de `schemaVersion`.
- **Superficie de lo emitido (`GeneratedSurfaceTests`):** barrido de todos los artefactos: 0 `[DataContract]`,
  0 `[CollectionDataContract]`, 0 `[Serializable]`, 0 referencias a la plataforma web; presencia de las bases e
  interfaces del contrato que el consumidor compila.
- **Propiedad (`CSharpOwnershipTests`):** propios reescritos siempre; editables creados sólo si faltan; contenido
  divergente ⇒ archivo intacto + *warning*; `--force` sí reemplaza; sin manifiesto, sin borrados.
- **Edición a mano (`EditableEditSurvivesTests`):** se edita un partial, se regenera, el archivo no cambia y la
  deriva se advierte.
- **Determinismo (`CSharpDeterminismTests`):** dos corridas byte a byte, 0 escrituras (ni `mtime`), cabecera sin
  fecha, incluido `core.autocrlf=true`.
- **Paridad (`CSharpGoldenParityTests`):** contra los dorados 1.4; toda diferencia debe estar declarada en
  `declared-differences.json` con motivo y decisión; el test **falla** ante una diferencia funcional no declarada.
- **Compilación del árbol emitido:** el proyecto de verificación compila el código generado contra los paquetes de
  la 2.0.
- **Derivados:** los artefactos usan los derivados calculados y no los persisten como estado.
- **Edge cases:** entidad sin propiedades; entidad con clave compuesta; relación uno-a-uno y uno-a-muchos (listas y
  vistas); campo de descripción ausente y presente; nombres con acentos y con caracteres que requieren escapado en
  el código emitido; artefacto preexistente idéntico (no se reescribe) y divergente (se advierte); plantilla
  override del consumidor sobre el set (resuelta por el motor) que produce el mismo resultado.
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio.

### 8.2 Integration test level

- **`n/a`** — el Bolt declara `test_integration: not-required` y el plan del frontmatter dice `n/a` con su razón:
  el resultado es el conjunto de plantillas y el código que emiten, verificable contra su referencia dorada y por
  compilación, sin base de datos ni servicio externo. La aplicación de scripts contra bases reales es del Bolt 020.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (§3.6).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El árbol compile con advertencias tratadas como error, incluido el **código emitido** compilado contra los paquetes 2.0 | `pass` — esperado (AC-5) |
| Unit / component tests | V-Bounce | Suite hermética verde (inventario, superficie, propiedad, determinismo, paridad, compilación) | `pass` — esperado (AC-1…AC-7) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del consumidor es el **código emitido** (nombres, miembros, firmas): se verifica sobre el árbol generado | `pass` — esperado (AC-6) |
| SAST | V-Bounce | El set emite código: se revisa que no introduzca dependencias de plataforma ni rutas de E/S en lo emitido | `pass` — esperado (AC-5) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas; lo emitido consume paquetes 2.0 ya fijados, sin copia de ensamblados | `pass` — esperado (AC-5) |
| OWASP Top 10 — static review | V-Bounce | No hay superficie externa; el código emitido es de la aplicación del consumidor, sin endpoints | `n/a` — sin superficie alcanzable en este Bolt |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts; las plantillas son datos interpretados, no instrucciones | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | 0 secretos en plantillas, fixtures y código emitido (el modelo no transporta credenciales por ADR-002 §3.1) | `pass` — esperado |
| Hallucination lint | V-Bounce | Tipos, miembros y rutas referenciados existen (o se declaran como aportados) | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros; el código emitido se deriva del modelo | `pass` — esperado |
| PII / DLP | V-Bounce | `data_classification: internal`; fixtures sintéticos; el código emitido no contiene datos | `pass` — esperado |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos; el código emitido se compila contra los paquetes fijados del árbol | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; el **código dorado 1.4 existe antes** (Bolt 001) y es la expectativa de partida | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas del árbol emitido | `pass` — esperado (AC-4) |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| Atributos de contrato de serialización en el código generado (ADR-008 §3.7) | V-Bounce | **0** `[DataContract]`/`[CollectionDataContract]`/`[Serializable]` en todo lo emitido, con su ruptura registrada | `pass` — esperado (AC-2) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | El set produce código del consumidor, no superficie de los paquetes 2.0 | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | No aplica: `test_integration: not-required` (plan `n/a`) | `n/a` — verificación contra el dorado y por compilación (§3.2.2) |
| Contract / E2E against the real providers | Integration test | No hay provider externo ni entorno en el alcance | `n/a` — sin dependencias reales |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint ni carga crítica; el costo de generación lo mide el Bolt 017 | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | El set no instrumenta: sus decisiones se ven en el resultado/reporte del motor | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: not-required` → `integration_test_plan.applicability: n/a`
> con su razón. Ningún gate del nivel de integración aplica.

---

## 10. Security and data

- **El código emitido es del consumidor, y su seguridad empieza por no prometer lo que no existe:** retirar los
  atributos de contrato y la marca de serialización binaria elimina de la superficie generada un mecanismo que el
  runtime objetivo ya no puede cumplir y que, en la práctica, sólo podía inducir a un consumidor a construir sobre
  él (ADR-008 §3.3).
- **Sin lógica de negocio ni secretos en las plantillas:** el set emite estructura (clases, miembros, mapeo), no
  datos: el modelo describe el esquema, no contiene credenciales (ADR-002 §3.1) y ninguna plantilla emite
  credenciales en claro (ADR-003 §4).
- **Sin superficie de ejecución:** las plantillas no acceden al sistema de archivos, a la red ni al runtime (eso lo
  garantiza el contexto endurecido del Bolt 017); este Bolt sólo aporta el contenido que ese contexto interpreta.
- **Datos y clasificación:** `data_classification: internal` (espeja el Bolt). Los fixtures son sintéticos y el
  código dorado 1.4 es código de producto, no datos de cliente.
- **Integridad del árbol congelado:** `1.4/**` no se modifica: las plantillas legacy son la referencia funcional
  que se lee, nunca un archivo que se edite.

---

## 11. Monitoring and observability

- **La observabilidad del set es la del motor:** el resultado y el reporte del Bolt 017 registran, por artefacto,
  si se escribió, se omitió (con motivo) o se eliminó, más los *warnings* de deriva. Este Bolt aporta el contenido
  que hace visibles esas decisiones.
- **Evidencia versionada:** el inventario publicado (`2.0/docs/template-set-csharp.md`), el manifiesto del set, las
  diferencias declaradas de paridad (`2.0/tests/fixtures/csharp-parity/declared-differences.json`) y el fragmento de
  rupturas (`2.0/docs/api-removals/US-000.BOLT-019.json`).
- **Sin telemetría de servicio:** el set no instrumenta nada (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration:** `n/a` en datos. El consumidor regenera su código con el motor 2.0 (flujo del Bolt 018) y migra
  sus proyectos al runtime objetivo (Bolt 021).
- **Compatibility:** el código generado **preserva la forma** que el consumidor compila (nombres, miembros, firmas
  y bases del contrato, ADR-005 §3.3) y cambia en dos cosas declaradas: **ya no emite atributos de contrato de
  serialización** (criterio ADR-008 §3.7: es un cambio de **atributos**, no de contrato compilable) y **ya no emite
  la marca de serialización binaria** en los partials. Un consumidor que necesite un mecanismo de serialización lo
  declara en su lado, sobre sus propios partials. La cabecera pierde la fecha de generación (ADR-003 §3.3), lo que
  es visible para cualquier herramienta que la leyera. Todas estas rupturas van a las notas de migración (Bolt 028)
  con su reemplazo.
- **Rollback:** revertir el commit del Bolt (el set es un conjunto de plantillas versionadas). Nada más depende de
  él que el motor que las interpreta; un rollback parcial (por ejemplo, conservar los atributos de contrato) es
  inválido: dejaría el criterio vinculante de ADR-008 §3.7 a medio cumplir.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Reescritura completa del set** (el mayor costo declarado por ADR-003 §4) y una pérdida funcional silenciosa | 3 | 4 | Referencia **dorada** 1.4 (Bolt 001) + paridad declarada **diferencia por diferencia** (`declared-differences.json`) + compilación del árbol emitido contra los paquetes 2.0 (AC-5, AC-6) |
| **Atributos de contrato que sobrevivan** en alguna parte del código generado | 2 | 3 | Criterio vinculante con dueño (ADR-008 §3.7) + barrido automatizado de **todo** lo emitido (0 ocurrencias) como parte de la aceptación (AC-2) |
| **Pérdida de una edición a mano** del consumidor | 2 | 5 | Regla de propiedad **incondicional** (motor, Bolt 017) + caso explícito de edición que sobrevive + *warning* de deriva + `--force` explícito (AC-3) |
| **Marcas temporales** que rompan la idempotencia | 2 | 3 | Cabecera fija compartida y prohibición de `DateTime.Now` en el set + test de dos corridas byte a byte (AC-4) |
| **Dependencia de la plataforma web reintroducida** por una plantilla | 2 | 4 | El código emitido debe **compilar** contra los paquetes 2.0 y el barrido verifica la ausencia (AC-5) |
| **Duplicación de derivados** en el artefacto (el estado del modelo persistido en el código) | 2 | 3 | Los derivados llegan calculados y la plantilla sólo los usa; se inspecciona lo emitido (AC-7) |
| La superficie de datos del motor (Bolt 017) no alcanza para un artefacto | 2 | 3 | Detener y reportar (§15): la brecha es de la proyección o del contrato del modelo, no del set |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| El retiro de los atributos de contrato se ejecuta **sólo** como retiro de atributos | ADR-008 §3.7 lo fija así: el cambio no toca nombres, miembros ni comportamiento, de modo que **no** altera el contrato que el consumidor compila. Cualquier otro ajuste sería una ruptura distinta y no decidida |
| La paridad se prueba contra el **código generado dorado**, no contra el motor legacy en ejecución | ADR-003 §4: el motor legacy depende de CodeDOM, que no existe en el runtime objetivo; la única referencia verificable es lo que generó, congelado |
| Las diferencias de paridad se registran en un artefacto versionado (`declared-differences.json`) y el test **falla** ante una diferencia funcional no declarada | "Diferencia declarada y justificada" es un criterio verificable sólo si hay una lista contra la que comparar; sin ella, la paridad es una impresión |
| La compilación del árbol emitido es un criterio de aceptación, no una comprobación manual | Es lo que demuestra que el código generado es propio del runtime objetivo; un `System.Web` reintroducido por una plantilla se detecta en el build, no en la revisión |
| El set **no** completa los partials del consumidor ni su lógica | El motor no escribe lo editable (ADR-003 §3.4) y no es dueño del negocio del consumidor |
| El inventario publicado (`template-set-csharp.md`) es parte de la entrega | Es el contrato de lo que el set produce y con qué clase de propiedad; sin él, el consumidor no sabe qué puede editar sin perder cambios |
| Los artefactos de **proyectos y configuración** no entran acá | Son del Bolt 021 (ADR-003 §3.4, F-37): mezclarlos ampliaría el set y su verificación con otro tipo de artefacto |

---

## 15. Stop conditions

1. **Cambio material en una fuente gobernada** (Bolt, ADR-003, ADR-008, ADR-005, el REV usado, baseline de
   código): detener, revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca dos revisiones (G16).
2. **La superficie de datos de la proyección (Bolt 017) o el contrato del modelo (Bolt 004) no alcanza para un
   artefacto del set**: detener y reportar — es una brecha de esas superficies, y el set no las amplía.
3. **Una diferencia funcional con el código dorado no puede declararse ni resolverse**: detener y escalar. La
   paridad funcional es el criterio del Bolt; una divergencia sin causa atribuible al runtime objetivo es un
   hallazgo, no un detalle.
4. **Algún artefacto del set exige conservar un atributo de contrato** (un consumidor identificado con evidencia):
   detener: el escape-hatch de ADR-008 §3.6 exige **los cinco** criterios juntos y se concede en el Bolt que lo
   pida, nunca por analogía dentro de éste.
5. **El código emitido no compila** contra los paquetes de la 2.0 y el motivo es una dependencia no disponible:
   detener y escalar (puede ser una brecha del Bolt 014/ADR-005, no del set).
6. **Se necesita una dependencia nueva o un paquete satélite** para el set: detener y escalar (ADR-003 §3.6).
7. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
8. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite o dorados 1.4): detener y escalar la
   secuencia; sin dorados no hay paridad verificable.
9. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, el contenido de un miembro generado que el
   modelo no declara): parar y reportar; no se rellena el hueco con una suposición (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–F implementadas (o, si el presupuesto se agota, progreso en el MEM y pendiente explícito para un
      V-Bounce nuevo — el Bolt estima 2)
- [ ] Todos los criterios de aceptación (AC-1…AC-8) verificados
- [ ] Suite hermética verde (§8.1), incluido el **código emitido compilando** contra los paquetes 2.0 y la paridad
      contra el dorado con diferencias declaradas; el plan de integración es `n/a` con su razón
- [ ] El inventario del set y su manifiesto están publicados y versionados
- [ ] El criterio vinculante de ADR-008 §3.7 verificado (0 atributos en todo lo emitido) y registrado como ruptura
      con su entrada para las notas de migración
- [ ] El código sigue ADR-003 (§3.1/§3.3/§3.4/§3.5), ADR-008 (§3.2/§3.7), ADR-002 (§3.1), ADR-005 (§3.3) y ADR-001 (§3.1/§3.2)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**)
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-019`](../functional/bolts/US-000.BOLT-019-plantillas-csharp.md) y su manifiesto
  [`US-000.BOLT-019-…json`](../metrics/bolts/US-000.BOLT-019-plantillas-csharp.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) ·
  [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) §3.2/§3.7 (criterio vinculante con dueño: **este** Bolt) ·
  [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) · [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) · [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md)
- SPEC prerrequisito directo: [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) (motor, set, proyección y propiedad)
- SPECs previos: [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y dorados 1.4) ·
  [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) (contrato del modelo) · [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) (POCOs y mapeo)
- SPECs relacionados: [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) (el CLI que regenera) · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) (retiro de la superficie de serialización del framework; declara el set de plantillas como dueño de los atributos del código generado)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-37, F-43 y §6 ítems 3, 6, 15
- Evidencia de código 1.4 (verificada en esta sesión): `Tools/CooperatorModeler/Templates/CSharpClasses/Entity.Auto.cs:33,451,461`
  (`[DataContract()]`, `[CollectionDataContract()]`), `Object.Auto.cs:22,414,425`, `Entity.cs:20` y `Object.cs:20`
  (`[Serializable]` en el partial editable), y el inventario del set (`Entity`/`Object`/`Mapper`/`Gateway`
  `{.Auto.,}`, `DefaultLazyProvider.cs`, `RuleExample.cs`); `Tools/CooperatorModeler/FolderHelper.cs`
  (`CopyFolder(source, destination, onlyAutoFiles, overwrite)`: el modo completo sobrescribía también lo editable)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-019`: set de plantillas C# con manifiesto e inventario, artefactos propios y editables con propiedad incondicional, retiro de los atributos de contrato (criterio vinculante de ADR-008 §3.7) y de la marca binaria de los partials, determinismo y paridad contra el código generado dorado de 1.4, con compilación del árbol emitido contra los paquetes de la 2.0 |

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
| **review.reviewers** | [Dev-validator + dueño de arquitectura (superficie del código generado y contrato del consumidor) + dueño de dominio de datos si se evalúa la forma del mapeo] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **El criterio vinculante de ADR-008 §3.7 es de este Bolt** para las clases C# (el DoR lo confirma): el retiro
   de `[DataContract]`/`[CollectionDataContract]` del código emitido, **más** el de `[Serializable]` de los
   partials (ADR-008 §3.2), se ejecuta acá y se registra como ruptura. El set de stored procedures (Bolt 020) y el
   de proyectos/configuración (Bolt 021) **no** son dueños de ese criterio.
2. **Método de paridad:** contra el **código generado dorado de 1.4** (fixtures del Bolt 001), no contra el motor
   legacy en ejecución (ADR-003 §4), y con las diferencias declaradas en un artefacto versionado contra el que el
   test compara. Confirmar el método.
3. **La compilación del árbol emitido** contra los paquetes de la 2.0 se propone como **criterio de aceptación**
   (hoy los proyectos generados son del Bolt 021; acá el proyecto de verificación es del arnés).
4. **La forma del contrato que se preserva** es la de ADR-005 §3.3; si el Bolt 014 cambia el borde público del
   acceso, la plantilla del `Gateway`/`Mapper` debe seguirlo — se resolvería como brecha declarada (§15 #5), no
   editando el ADR.
5. **Los artefactos de proyectos y configuración** quedan en el Bolt 021: acá **no** se emiten `*.csproj`, `*.sln`
   ni `appsettings.json`, aunque el set legacy los tenga en el mismo árbol de plantillas.
