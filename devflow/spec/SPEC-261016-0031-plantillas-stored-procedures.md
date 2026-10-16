---
id: "SPEC-261016-0031"
title: "Set de plantillas de stored procedures: un artefacto por procedimiento, T-SQL modernizado y verificación sobre base real"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 18 (hallazgo F-46) y F-37 → ADR-003
bolt: "US-000.BOLT-020" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-003", "ADR-001", "ADR-002"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, fixtures dorados 1.4 y capacidad de verificación contra SQL Server real (Bolt 001)
  - "SPEC-261016-0023-motor-headless.md" # el motor, su resolución de set, sus helpers de lista blanca y la regla "emite, no aplica" (Bolt 017)
  - "SPEC-261016-0004-particion-modeling.md" # la fuente única del mapeo SQL↔CLR que las plantillas consumen (Bolt 003)
  - "SPEC-261016-0010-contrato-coop2-en-modeling.md" # el contrato del modelo y el recálculo de derivados (Bolt 004)
risk_class: "high" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L2" # default por riesgo high (§3.3): más paradas y confirmaciones durante el V-Bounce
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "internal" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: required` → `applicable` (§3.2.2)
  applicability: "applicable"
  tests: # escritas en este V-Bounce; se ejecutan en el ciclo de integración del Bolt
    - "2.0/tests/Cooperator.SqlTemplates.Integration.Tests/ApplyScriptsOnCleanDatabaseTests.cs (Category=Integration) — aplica **todos** los artefactos emitidos sobre una base limpia del motor soportado y verifica que no hay error; repetido una segunda vez para comprobar que el set es re-aplicable"
    - "2.0/tests/Cooperator.SqlTemplates.Integration.Tests/StoredProcedureContractTests.cs (Category=Integration) — cada procedimiento y función emitido existe con la firma declarada por el modelo y responde: smoke funcional de alta, modificación, borrado, consulta por clave, consultas del modelo y función de descripción"
    - "2.0/tests/Cooperator.SqlTemplates.Integration.Tests/ObsoleteObjectsIndependenceTests.cs (Category=Integration) — los artefactos aplicados no dependen de los objetos de sistema obsoletos: la comprobación de existencia modernizada es la única que corre y el objeto se crea y se re-crea sin residuos"
    - "2.0/tests/Cooperator.SqlTemplates.Integration.Tests/BatchSeparationTests.cs (Category=Integration) — un artefacto cuyo contenido incluye el separador de lotes se aplica **intacto** (sin romperse) sobre la base"
    - "2.0/tests/Cooperator.SqlTemplates.Integration.Tests/PostGenerationContentTests.cs (Category=Integration) — el contenido que el modelo declara como paso posterior a la generación se aplica desde el lane cuando corresponde y **nadie** lo ejecuta durante la generación"
  environment: "el lane de integración del proyecto con una instancia de **SQL Server** (el motor soportado) donde una herramienta **externa** aplica los scripts emitidos; **aún no provisionado** (el repositorio no tiene pipeline). Reutiliza la capacidad y la guarda de base de fixtures del arnés del Bolt 001 (`cooperator_it`)."
  data: "una base **limpia y desechable por corrida** (esquema y datos sintéticos del arnés, `2.0/tests/fixtures/sqlserver/`), sin datos reales ni de cliente; el modelo de referencia `.coop2` del arnés; los scripts los aplica el lane —**nunca** el motor— y el resultado (objetos creados y smoke funcional) es la evidencia del ciclo."
  reason: ""
  waiver_adr: ""
review_ready_at: "2026-10-16T00:35:59-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:35:59-03:00"
  decided_at: "2026-10-16T00:35:59-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-020 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent; su acknowledgment «required confirmado»), ADR-003/ADR-001/ADR-002 accepted (tech_lead), REV-001 approved, OQ-005 answered; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class high → autonomy_level L2; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: required → applicability: applicable; un archivo por procedimiento/función (trazo al nodo del modelo) + el caso del separador dentro del contenido (separación estructural, no de contenido); T-SQL modernizado (comprobación/drop sobre catálogo vigente —equivalente a sys.objects/OBJECT_ID—, también para la función de descripción; sin marca temporal); mapeo del contrato del modelo (sin segunda tabla) + sin scriptors/VB/Oracle; post-generación emitida y no ejecutada (0 conexiones)."
    - "CONFIRMACIÓN (plan de integración real, applicable) — CORRECTA: 5 suites (ApplyScriptsOnCleanDatabase/StoredProcedureContract/ObsoleteObjectsIndependence/BatchSeparation/PostGenerationContent) + environment (CI + SQL Server, herramienta externa del lane) + data (base limpia y desechable, guarda cooperator_it); §9 con filas de integración en pass; §16 DoD + §8.2 exigen la corrida cubriente en integration_test_runs[] antes de ready-for-acceptance."
    - "CONFIRMACIÓN (herramienta externa de aplicación) — CORRECTA: ADR-003 §3.1 «el motor emite, nunca aplica»; la aplicación de los scripts en el ciclo de integración la hace una herramienta externa del lane (2.0/build/apply-sql-fixtures.ps1), nunca el motor, verificado por construcción (0 conexiones, 0 ejecución SQL — AC-7)."
    - "CONFIRMACIÓN (nota L2 — independencia endurecida a nivel de modelo) — CUMPLIDA: mi modelo es deepseek/deepseek-v4-pro; el ejecutor (dotnet-developer-agent) usa deepseek/deepseek-flash. Modelos distintos, satisfaciendo la independencia endurecida que el riesgo high exige para las aprobaciones de este Bolt."
    - "Compliant — cambio de empaquetado declarado (un archivo por procedimiento: el consumidor que aplicaba «el archivo de SPs» ahora aplica el conjunto de archivos en orden canónico) + rupturas registradas (ADR-008 §3.1, fragmento api-removals/US-000.BOLT-020.json); guarda de datos cooperator_it reutilizada del arnés."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). Un archivo por procedimiento, el fin de la separación de lotes por texto, el T-SQL
  que se emite, el consumo del mapeo de tipos y la regla "el motor emite, nunca aplica" están decididos en
  ADR-003 §3.1/§3.5, ADR-001 §3.1/§3.5 y ADR-002 §3.1. Este SPEC los ejecuta y los vuelve verificables.
  ⚠️ RIESGO `high` → `autonomy_level: L2` (§3.3): el V-Bounce para y confirma ante cualquier desvío que afecte
  lo que el consumidor aplicará contra su base; los scripts terminan aplicándose contra bases reales.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de
  dominio aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada
  invalida la aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0031 — Set de plantillas de stored procedures: un artefacto por procedimiento, T-SQL modernizado y verificación sobre base real

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 18 (hallazgo F-46) y F-37 |
| **Bolt** | [US-000.BOLT-020](../functional/bolts/US-000.BOLT-020-plantillas-stored-procedures.md) |
| **ADRs** | [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1, §3.3, §3.5, §3.6 · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1, §3.5 · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.1 |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se entrega el **set de plantillas de stored procedures**: los scripts que el consumidor aplica contra su base,
emitidos **uno por procedimiento** —trazable al nodo del modelo que lo origina—, con **T-SQL propio del motor
soportado** (sin los objetos de sistema obsoletos que usaba el set legacy) y **sin la separación de lotes frágil**
que dependía del contenido del script.

El problema que resuelve está verificado: `REV-001` F-46 muestra que el set de 1.4 emite, en cada script, una
comprobación de existencia sobre **objetos de sistema obsoletos** —`if exists (select * from dbo.sysobjects where id
= object_id(N'[dbo].[…]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)`— y que el empaquetado depende de **separar
lotes por coincidencia de texto** (el string `GO`), lo que convierte cualquier ocurrencia del separador dentro del
contenido en un script roto. A eso se suma F-37 (el set legacy arrastra la plataforma web). Si este Bolt no se
implementa, el consumidor aplicaría contra su base scripts que funcionan pero están deprecados y que se rompen (o
se pierden) según lo que contengan — y el riesgo es **alto** precisamente porque el destino final es una base de
datos real: es el Bolt con mayor exposición de la familia.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 18 como BOLT→SPEC y el ítem 6 a ADR-003
(`accepted`), que decide: **un archivo por stored procedure** —con lo que el *split* de lotes por el string `GO`
**deja de existir**—, el motor que **emite y nunca aplica** SQL, los helpers que **consumen** el mapeo de tipos del
contrato del modelo (que no se redefine), y la modernización del T-SQL emitido como criterio del set. Este SPEC
**no reabre** nada: fija las plantillas, su referencia de paridad y las verificaciones.

**El set legacy que se reescribe (evidencia verificada en esta sesión)** — `Tools/CooperatorModeler/Templates/StoredProcedures/`:

| Plantilla legacy | Qué emite | Destino en la 2.0 |
|---|---|---|
| `Insert.cs`, `Update.cs`, `Delete.cs` | alta, modificación y borrado por clave; comprobación de existencia con `dbo.sysobjects` + `OBJECTPROPERTY(…, N'IsProcedure')` y `drop procedure` | un artefacto por procedimiento, con comprobación de existencia modernizada |
| `GetOne.cs`, `GetAll.cs`, `GetByParent.cs`, `GetByParents.cs`, `DeleteByParents.cs` | consultas y operaciones por relación | un artefacto por procedimiento, trazado a la relación del modelo |
| `Queries.cs` | las consultas que declara el modelo, con su texto (`<FIELDS>`/`<TABLE>` resueltos) y sus parámetros | un artefacto por consulta, con el mismo contrato de texto resuelto |
| `DescriptionFunction.cs` | la **función** de descripción (`_GetDescription`), con `xtype in (N'FN', N'IF', N'TF')` en su comprobación | un artefacto por función, con la comprobación modernizada |
| — | cabecera `-- Author: Cooperator Modeler, Version …` + **`-- Created: <fecha> - <hora>`** | cabecera fija **sin** fecha (ADR-003 §3.3) |
| — | `Response.SaveBuffer(<nombre>)` por objeto, y separación de lotes por texto en el armado | **un archivo por procedimiento**: la separación es estructural (límite de archivo), no de contenido |

Los helpers que las plantillas legacy usaban (`ALL_FIELDS_AS_PARAMETERS`, `SET_GUID_VALUES`,
`ALL_VARIABLES_OF_FIELDS_SEPARATED_BY_COMMA`, `SET_ROWVERSION`, `CORRECT_TABLENAME`, `ALL_FIELDS_FOR_SELECT`,
`parameters["SpPrefix"]`) tienen su equivalente en la **lista blanca de helpers del motor** (escritura T-SQL de
tipos, *quoting* de identificadores con default `[dbo]`, nombre de archivo/lote y utilidades de texto), que
consumen el **mapeo de tipos** de `Modeling` (ADR-001 §3.1: una sola tabla; el set **no** define una segunda).

**Referencia de paridad.** Los scripts generados por el Modeler 1.4 (con `CodeGenerator.GenerateStoredProcedures`
sobre el `Snapshot` congelado) son el **artefacto dorado** que el Bolt 001 deja versionado en
`2.0/tests/fixtures/golden-generation/**`; ADR-003 §4 aclara por qué la comparación es contra el **código
generado**, no contra el motor legacy en ejecución. La paridad se declara **diferencia por diferencia**.

**Estado del árbol 2.0.** `2.0/` todavía no existe en el repositorio: lo crea el Bolt 001, que también deja la
capacidad de verificación contra SQL Server real (`2.0/tests/fixtures/sqlserver/`, guarda `cooperator_it`). El
motor (Bolt 017, [SPEC-261016-0023](./SPEC-261016-0023-motor-headless.md)) ya tiene SPEC en disco y es la
dependencia directa.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-003 §3.1 consuma el mapeo de tipos que ADR-001 §3.1 declara como
dato portable único; ADR-001 §3.5 fija un **solo** motor de base soportado; ADR-002 §3.1 declara la limitación
conocida del modelo, que se declara y no se rellena), y el plan de integración declarado (`applicable`) coincide con
el `test_integration: required` del Bolt (§3.2.2). No se emite reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-020` (`devflow/functional/bolts/US-000.BOLT-020-plantillas-stored-procedures.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:09:32-03:00; `risk_class: high`, `test_integration: required` (confirmado en el READY: los scripts se aplican contra bases reales y su validez no se demuestra sólo con la referencia dorada) |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-003` §3.1 (un archivo por procedimiento; fin del *split* por texto; el motor **emite y nunca aplica**; el mapeo de tipos se consume, no se redefine), §3.3 (determinismo), §3.5 (plantillas como datos y helpers), §3.6 (dependencias) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T22:32:47-03:00 |
| ADR (gobernante) | `ADR-001` §3.1 (el mapeo SQL↔CLR como dato portable único, consumido por el reverse y por el motor) y §3.5 (un solo motor de base soportado: sin Oracle) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T22:32:47-03:00 |
| ADR (gobernante) | `ADR-002` §3.1 (los derivados se recalculan al leer; la limitación conocida del modelo se declara y **no** se rellena) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T22:32:47-03:00 |
| REV evidence | `REV-001` §4 F-46 y F-37, y §6 ítem 18 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| OQ evidence | `OQ-005` (deprecaciones: VB.NET, Oracle, export a Excel) | `answered` 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0000-…` (arnés, dorados 1.4 y capacidad SQL Server), `SPEC-261016-0023-…` (motor), `SPEC-261016-0004-…` (mapeo de tipos), `SPEC-261016-0010-…` (contrato del modelo) | Los cuatro **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (nada bajo `2.0/` ni bajo `1.4/` en el árbol de trabajo) |

---

## 4. Scope

### In scope

1. **El set de plantillas de stored procedures**, como datos versionados y embebidos en el paquete del motor (el
   **mecanismo** de resolución es del Bolt 017), con su manifiesto: los procedimientos que **el modelo declara**
   (alta, modificación, borrado, consulta por clave, consultas del modelo y las operaciones que acompañan a las
   relaciones entre entidades) y la **función de descripción** cuando el modelo la pide.
2. **Un artefacto por procedimiento** (y por función), con **trazo al nodo del modelo** que lo origina y
   colección en orden canónico, de modo que ningún script se pierda ni se pise por empaquetado.
3. **El fin de la separación de lotes por contenido**: la separación es el límite del archivo; un literal que
   contenga el separador **no** rompe nada. Es consecuencia directa de (2) y se verifica de forma explícita.
4. **El T-SQL emitido, sin los objetos de sistema obsoletos**: las comprobaciones de existencia y los `drop`
   pasan a apoyarse en los objetos de catálogo vigentes del motor soportado (equivalente moderno de
   `sys.objects`/`OBJECT_ID`), para procedimientos y para funciones; y la cabecera deja de emitir la marca
   temporal, conservando la cabecera fija de ADR-003 §3.3.
5. **El consumo del mapeo de tipos del contrato del modelo** para escribir tipos e identificadores (*quoting*
   `[schema].[objeto]` con default `[dbo]`), **sin** redefinirlo y sin una segunda tabla en el set; y el uso de la
   **lista blanca de helpers** del motor (nombre de archivo/lote, utilidades de texto), sin la indirección de
   scriptor por nombre de clase y **sin** VB ni Oracle.
6. **La emisión del contenido que el modelo marca como paso posterior a la generación**: se **emite** como
   artefacto; **nunca** se ejecuta (el motor no aplica SQL).
7. **La referencia dorada y la paridad**: comparación contra los scripts generados de 1.4, con cada diferencia
   declarada y atribuida (modernización del T-SQL, empaquetado por procedimiento, retiro de la marca temporal).
8. **La verificación sobre base real** (ciclo de integración): los artefactos emitidos se aplican **sin error**
   sobre una base limpia del motor soportado, y los objetos creados responden.
9. **La constancia** de las rupturas que este Bolt introduce (ADR-008 §3.1), como insumo de las notas de
   migración (Bolt 028).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **motor**, su contexto de render, sus límites y su resolución de set | Bolt 017 (SPEC-0023) | El Bolt lo excluye: acá se escriben plantillas |
| **Aplicar** los scripts contra una base, o implementar el pipeline que los aplica | — (el pipeline del consumidor / el lane de verificación) | ADR-003 §3.1: el motor **emite**, el consumidor **aplica**. La verificación de este Bolt los aplica desde el lane, no desde el motor |
| El set de **clases C#** | Bolt 019 (SPEC-0029) | Otro conjunto de artefactos |
| Las plantillas de **proyectos y configuración** | Bolt 021 | Otro conjunto de artefactos |
| El **esquema** de la base del consumidor y la **migración de sus datos** | — (nadie) | El set emite objetos de programa (procedimientos y funciones); no altera tablas ni migra datos |
| Motores de base **alternativos** (Oracle u otros) | — (nadie) | ADR-001 §3.5 y OQ-005: SQL Server es el único motor soportado |
| El **mapeo de tipos** | Bolt 003 / contrato del modelo (ADR-001 §3.1) | Es el dato portable del contrato; el set lo **consume** y **no** lo duplica |
| Capacidades del modelo que el **formato no declara** (por ejemplo la limitación conocida sobre vistas) | — (nadie) | ADR-002 §3.1: se **declara** la limitación; no se inventa el dato que no existe |
| La modernización del T-SQL de las **clases/gateways** | Bolt 019 | Es texto de otro conjunto de artefactos |

**Regla de aporte (bounded, explícita).** Este Bolt **escribe plantillas**; no crea infraestructura ni toca el
motor. Si la superficie de datos o los helpers de lista blanca del Bolt 017 no alcanzan para un artefacto, o si el
mapeo de tipos del contrato no cubre un tipo nativo que el modelo declara, se **detiene** y se reporta: es una
brecha de esas superficies, y ampliarla no es de este Bolt.

---

## 5. Prerequisites and baseline

- **Bolt 017 y su SPEC** (`SPEC-261016-0023-…`): el motor que interpreta estas plantillas, su **lista blanca de
  helpers** (escritura de tipos, *quoting*, nombre de archivo/lote, utilidades de texto), su manifiesto de set y la
  regla "emite, nunca aplica".
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el arnés, los **artefactos dorados**
  (`2.0/tests/fixtures/golden-generation/**`) y la **capacidad de verificación contra SQL Server real** con su
  guarda de base de fixtures (`2.0/tests/fixtures/sqlserver/`, `cooperator_it`).
- **Bolt 003 y su SPEC** (`SPEC-261016-0004-…`): la **fuente única** del mapeo SQL↔CLR (`Reverse/TypeMapping`), que
  las plantillas consumen a través de los helpers.
- **Bolt 004 y su SPEC** (`SPEC-261016-0010-…`): el contrato del modelo (lectura, derivados recalculados,
  `schemaVersion`) que determina qué procedimientos y funciones se emiten.
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error. La verificación hermética es `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`.
- **Riesgo `high` → autonomía `L2` (§3.3):** durante el V-Bounce se **para y confirma** ante cualquier desvío que
  cambie lo que el consumidor aplicará contra su base (T-SQL emitido, nombre o conjunto de artefactos, contrato de
  parámetros). La aprobación del MEM y del SPEC exige independencia **endurecida a nivel de modelo** (§3.0).
- **Estado del árbol 2.0 al escribir este SPEC:** `2.0/` todavía no existe (lo crea el Bolt 001); el motor llega con
  el Bolt 017, dependencia declarada.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline antes y después de construir; sus plantillas
  legacy son **evidencia funcional**, no un insumo que se edite.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-020`), **2 V-Bounces** esperados más el ciclo de integración. El orden va del
> inventario al cierre: primero el set y su trazo, después el empaquetado por procedimiento y el T-SQL, después el
> mapeo y los helpers, y al final la paridad y la aplicación sobre base real.

### Phase A — El set de SP como datos, con trazo al modelo

**Duration:** ~2,5h de ciclo — **Complexity:** Medium

#### A.1 Inventario y manifiesto del set

Se declara el inventario del set con el set legacy como referencia funcional: por cada **procedimiento** que el
modelo declara (alta, modificación, borrado, consulta por clave, consultas del modelo, operaciones por relación y
borrado por relación) y por cada **función** de descripción cuando el modelo la pide. Cada plantilla produce **un
artefacto**, y su nombre se construye con el prefijo de procedimientos que el modelo declara y el nombre del nodo
(entidad o consulta): el inventario entra al **manifiesto del set** (nombre, versión propia, rango de
`schemaVersion`, plantillas conocidas, artefactos que produce) que el motor del Bolt 017 consume para resolver,
reportar y limpiar huérfanos.

#### A.2 Las plantillas como datos, con helpers de lista blanca

Cada plantilla se escribe como **dato** (Scriban interpretado, sin C# empotrado ni compilación) y consume
**exclusivamente**: el modelo proyectado (datos, no objetos por reflexión), los **derivados ya calculados** por el
contrato (nombres, predicados, claves, relaciones) y los helpers de la lista blanca del motor. Ninguna plantilla
accede al sistema de archivos, a la red ni al runtime, y **ninguna** define su propio mapeo de tipos.

**Files created:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/Sql/` — las plantillas de stored procedures y funciones
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/Sql/templates.manifest.json` — inventario, versión del set y rango de `schemaVersion`
- `2.0/docs/template-set-sql.md` — el inventario publicado: artefacto, plantilla, nodo del modelo que lo origina y datos de entrada

---

### Phase B — Un artefacto por procedimiento (el fin de la separación por contenido)

**Duration:** ~3h de ciclo — **Complexity:** Medium

#### B.1 La empaquetadura estructural

El legacy emitía los procedimientos **juntos** (buffer por objeto con `Response.SaveBuffer(<nombre>)` y separación
de lotes por coincidencia de texto); la 2.0 emite **un archivo por procedimiento** (y por función). La separación
de lotes deja de existir como problema: el límite del archivo **es** la separación, y ningún contenido puede
romperla. El nombre del artefacto lo produce el **helper** de nombre de archivo/lote del motor, de modo que el
nombre sea estable y trazable al nodo.

#### B.2 El caso del separador dentro del contenido

Se escribe el caso explícito que el Bolt exige: un artefacto cuyo contenido (un literal, un comentario o un
identificador) incluye el separador de lotes **no** se rompe ni se parte: se aplica intacto. La prueba vive en dos
niveles: hermética (el árbol emitido tiene exactamente un artefacto por procedimiento, sin particiones) e
integración (la aplicación sobre la base limpia no falla).

**Files created:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/Sql/_{Insert,Update,Delete,GetOne,GetAll,GetByParent,GetByParents,DeleteByParents}.scriban`
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/Sql/_Queries.scriban` · `_DescriptionFunction.scriban` — las consultas del modelo y la función de descripción
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlArtifactPerProcedureTests.cs` — un artefacto por procedimiento, trazado al nodo, sin particiones por contenido

---

### Phase C — El T-SQL modernizado y la cabecera fija

**Duration:** ~3h de ciclo — **Complexity:** High

#### C.1 Fuera los objetos de sistema obsoletos

La comprobación de existencia y el `drop` previo se apoyan en los objetos de catálogo **vigentes** del motor
soportado (el equivalente moderno de `sys.objects`/`OBJECT_ID`), tanto para **procedimientos** como para la
**función** de descripción —el legacy usaba `dbo.sysobjects` + `OBJECTPROPERTY(id, N'IsProcedure')` para
procedimientos y `xtype in (N'FN', N'IF', N'TF')` para funciones—. La creación es **re-ejecutable**: aplicar el set
dos veces sobre la misma base no falla y no deja residuos.

#### C.2 La cabecera, sin la marca temporal

Cada artefacto lleva la cabecera fija de ADR-003 §3.3: herramienta + versión del motor, versión del set,
`schemaVersion` del modelo y el artefacto editable que lo acompaña (si lo hay). **Sin** `-- Created: <fecha>`: la
marca temporal del legacy (`DateTime.Today`/`DateTime.Now`) es lo que hace imposible que dos corridas den bytes
idénticos, y su retiro es parte del determinismo del set.

#### C.3 El *quoting* y los identificadores

El `quoting` de identificadores (`[schema].[objeto]`, con el esquema por defecto del motor soportado cuando el
modelo no lo declara) y la escritura de tipos los aportan los **helpers del motor** que consumen el mapeo del
contrato: ninguna plantilla arma a mano el nombre calificado ni traduce un tipo nativo a su equivalente CLR o
viceversa.

**Files modified:**
- `2.0/src/Cooperator.Framework.CodeGeneration/Templates/Sql/**` — comprobación de existencia modernizada y cabecera fija

**Files created:**
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlModernizationTests.cs` — 0 objetos de sistema obsoletos, 0 marcas temporales, re-ejecutabilidad textual (drop-guard presente y moderno)

---

### Phase D — El mapeo de tipos del contrato y la lista blanca de helpers

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### D.1 Una sola tabla de mapeo

La traducción tipo nativo ↔ tipo CLR y la escritura del tipo en el script se resuelven con el **mapeo del contrato
del modelo** (`Modeling`, fuente única, ADR-001 §3.1) a través de los helpers del motor: el set **no** define una
tabla propia, ni una copia parcial, ni una lista de equivalencias "por si falta". La verificación es explícita: no
existe una segunda tabla de mapeo en el set y todo tipo emitido se resuelve por el helper.

#### D.2 Sin indirección de scriptor, sin VB, sin Oracle

El concepto legacy de **scriptor** (selección por el nombre de la clase dentro de un string) y su esqueleto de
Oracle **no** se reproducen: la escritura de SQL Server vive en las plantillas del set y en los helpers del motor,
ambos versionados con él. No se emite VB (el motor genera sólo C# del lado del código) ni ningún artefacto de un
motor de base distinto del soportado.

**Files created:**
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlTypeMappingTests.cs` — el mapeo viene del contrato; no hay segunda tabla; 0 selección de scriptor por nombre de clase
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlHelperWhitelistTests.cs` — el set usa sólo los helpers de la lista blanca

---

### Phase E — Contenido post-generación, "emite y no aplica", determinismo y paridad

**Duration:** ~3h de ciclo — **Complexity:** High

#### E.1 El contenido post-generación se emite, no se ejecuta

El contenido que el modelo declara como paso posterior a la generación se emite como **artefacto** (archivo cuando
no está vacío) y **nadie** lo ejecuta durante la generación. La prueba es de construcción y de integración: el
motor no abre ninguna conexión y el contenido aparece en el árbol emitido.

#### E.2 El motor emite; el set no aplica nada

Se verifica la propiedad central del ADR: ninguna pieza del motor ni del set abre una conexión ni ejecuta SQL
(0 referencias a tipos de acceso a datos en el motor y 0 aperturas de conexión en la generación). La aplicación es
del consumidor —y, para la verificación de este Bolt, del **lane de integración**, con una herramienta externa—.

#### E.3 Determinismo byte a byte

Dos corridas del mismo modelo producen el mismo árbol: sin marcas temporales, con orden canónico de artefactos
(por nombre, ordinal) y de las colecciones internas de cada script (campos, parámetros, consultas), con `LF` y sin
espacios finales. La evidencia es el `git diff` vacío y la ausencia de escrituras, incluido un *checkout* Windows
con `core.autocrlf=true`.

#### E.4 La paridad, diferencia por diferencia

Sobre el modelo de referencia se compara el árbol emitido contra los **scripts generados dorados de 1.4** y se
declara cada diferencia con su motivo: (a) **empaquetado** —un artefacto por procedimiento en lugar del conjunto
separado por texto—; (b) **modernización del T-SQL** —comprobaciones de existencia sobre catálogo vigente—;
(c) **cabecera sin marca temporal** (determinismo); (d) cualquier ajuste exigido por el contrato del modelo. Lo que
**no** puede diferir es la función: los procedimientos y funciones que el modelo declara, su contrato de parámetros
y su efecto observable.

**Files created:**
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlPostGenerationTests.cs` — el contenido post-generación se emite y no se ejecuta
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlNoSqlExecutionTests.cs` — 0 conexiones y 0 ejecución de SQL
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlDeterminismTests.cs` — dos corridas byte a byte, 0 escrituras, sin marcas temporales
- `2.0/tests/Cooperator.Framework.CodeGeneration.Tests/SqlGoldenParityTests.cs` — paridad contra los dorados 1.4
- `2.0/tests/fixtures/sql-parity/declared-differences.json` — las diferencias declaradas y su motivo (una por una)

---

### Phase F — La verificación sobre base real (ciclo de integración)

**Duration:** ~2,5h de ciclo — **Complexity:** High

#### F.1 Aplicación sobre una base limpia

Los cinco tests de integración del frontmatter aplican **todos** los artefactos emitidos sobre una base **limpia y
desechable** del motor soportado —con una herramienta **externa**, nunca el motor— y verifican: que aplican sin
error; que re-aplicarlos tampoco falla (el set es re-ejecutable por su *drop-guard* modernizado); que los objetos
creados existen con la firma que el modelo declara; y un smoke funcional por familia (alta, modificación, borrado,
consulta por clave, consultas del modelo, operación por relación y función de descripción).

#### F.2 La guarda de datos

La corrida usa la guarda del arnés: sólo una base cuyo nombre contiene `cooperator_it`, creada y descartada por
test. Ninguna corrida toca una base de cliente; los datos son sintéticos. Esta guarda es la diferencia entre una
verificación y un incidente.

#### F.3 Qué se registra

El resultado del ciclo queda en `integration_test_runs[]` del manifiesto del Bolt (modo, entorno, resultado,
evidencia y quién lo ejecutó): objetos creados, resultado del smoke y la salida de la re-aplicación.

**Files created:**
- `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/ApplyScriptsOnCleanDatabaseTests.cs`
- `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/StoredProcedureContractTests.cs`
- `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/ObsoleteObjectsIndependenceTests.cs`
- `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/BatchSeparationTests.cs`
- `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/PostGenerationContentTests.cs`
- `2.0/build/apply-sql-fixtures.ps1` — el aplicador **del lane** (herramienta externa al motor) para el ciclo de integración
- `2.0/docs/api-removals/US-000.BOLT-020.json` — constancia de las rupturas (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-003/ADR-001/ADR-002, expresados como Given/When/Then verificables.

### AC-1: Un artefacto por procedimiento, trazado al modelo

**Given** el modelo de referencia y su prefijo de procedimientos
**When** se generan los scripts
**Then** hay **exactamente un artefacto por procedimiento** (y por función), cada uno trazado al nodo del modelo
que lo origina (entidad o consulta) en el inventario publicado, y la colección está en orden canónico.

### AC-2: La separación de lotes no depende del contenido

**Given** un artefacto cuyo contenido incluye el separador de lotes (en un literal, un comentario o un
identificador)
**When** se genera y se aplica
**Then** el artefacto no se parte ni se pierde, y su aplicación no falla: la separación es estructural (límite de
archivo).

### AC-3: El T-SQL no se apoya en objetos de sistema obsoletos

**Given** el árbol emitido
**When** se barre su texto
**Then** hay **0** ocurrencias de los objetos de sistema obsoletos del set legacy en las comprobaciones de
existencia y en los `drop`, tanto para procedimientos como para la función de descripción, y la creación es
**re-ejecutable**.

### AC-4: El mapeo de tipos se consume; no se duplica

**Given** el set y los helpers del motor
**When** se inspecciona la generación de tipos e identificadores
**Then** todo tipo nativo y todo nombre calificado se resuelve por el helper que consume el **mapeo del contrato
del modelo**, y **no** existe una segunda tabla de mapeo ni un *quoting* armado a mano en ninguna plantilla.

### AC-5: Determinismo y cabecera sin fecha

**Given** el modelo de referencia
**When** se genera dos veces (incluido un *checkout* con `core.autocrlf=true`)
**Then** los artefactos son idénticos **byte a byte**, `git diff` queda vacío, no hubo escrituras y **ninguna**
cabecera contiene una fecha de generación.

### AC-6: Paridad funcional con el dorado, diferencia por diferencia

**Given** los scripts generados dorados de 1.4 (fixtures del Bolt 001)
**When** se comparan con el árbol emitido
**Then** cada diferencia está en `sql-parity/declared-differences.json` con su motivo y su decisión (empaquetado,
modernización, cabecera), y **no** hay diferencias de función (procedimientos y funciones declarados por el modelo,
contrato de parámetros, efecto observable) sin declarar.

### AC-7: El motor no aplica SQL y el contenido post-generación se emite

**Given** una corrida de generación
**When** se instrumenta el motor
**Then** hay **0** conexiones y **0** ejecución de SQL, y el contenido que el modelo declara como paso posterior a
la generación aparece como **artefacto** en el árbol emitido.

### AC-8: Los scripts aplican sin error sobre una base limpia (ciclo de integración)

**Given** una base limpia y desechable del motor soportado (`cooperator_it`)
**When** el lane aplica **todos** los artefactos emitidos, dos veces, con una herramienta externa
**Then** no hay error en ninguna de las dos aplicaciones, los procedimientos y la función existen con la firma
declarada, el smoke funcional responde y el resultado queda registrado en `integration_test_runs[]`.

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde (con los casos de
integración filtrados por categoría) y `git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están
vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "cada procedimiento se emite como un artefacto propio, trazable al nodo del modelo" | Fase A.1/B.1: un artefacto por procedimiento + inventario | AC-1 · `SqlArtifactPerProcedureTests` |
| "la separación de lotes **no depende del contenido** del script" | Fase B.2: separación estructural + caso explícito | AC-2 · hermético + integración |
| "el T-SQL emitido **no** se apoya en los objetos de sistema obsoletos" | Fase C.1: comprobación y `drop` modernizados (procedimientos y función) | AC-3 · `SqlModernizationTests` + integración |
| "el mapeo de tipos se **consume** del contrato del modelo y no se redefine" | Fase D.1: helper + mapeo único | AC-4 · `SqlTypeMappingTests` |
| "el motor **emite** los scripts y **no** los aplica contra ninguna base" | Fase E.2: 0 conexiones, 0 ejecución | AC-7 · `SqlNoSqlExecutionTests` |
| "el contenido… como paso posterior a la generación se emite como artefacto, nunca se ejecuta" | Fase E.1 | AC-7 · `SqlPostGenerationTests` |
| "el comportamiento es equivalente al de los scripts generados de 1.4, con las diferencias declaradas" | Fase E.4: paridad declarada diferencia por diferencia | AC-6 · `SqlGoldenParityTests` + `declared-differences.json` |
| "lo emitido aplica **sin error** sobre una base limpia del motor soportado" | Fase F: aplicación en el ciclo de integración | AC-8 · corrida registrada en `integration_test_runs[]` |
| "la entrada propia de las rupturas… registrada en las notas de migración en el mismo acto" | Fase F.3: fragmento + MEM | AC-8 · fragmento `api-removals/US-000.BOLT-020.json` |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Inventario y trazo (`SqlArtifactPerProcedureTests`):** un artefacto por procedimiento y por función; cada uno
  trazado al nodo del modelo; nombres construidos por el helper (prefijo + nombre del nodo); orden canónico; el
  manifiesto del set coincide con el inventario publicado.
- **Modernización (`SqlModernizationTests`):** barrido de todo el texto emitido: 0 objetos de sistema obsoletos, 0
  marcas temporales; presencia del `drop-guard` modernizado para procedimientos y para funciones.
- **Mapeo y helpers (`SqlTypeMappingTests`, `SqlHelperWhitelistTests`):** todo tipo y todo identificador se resuelve
  por los helpers del motor; no hay segunda tabla de mapeo ni *quoting* manual; el set usa sólo la lista blanca.
- **Post-generación y no-ejecución (`SqlPostGenerationTests`, `SqlNoSqlExecutionTests`):** el contenido declarado se
  emite; 0 conexiones y 0 ejecución de SQL en la generación.
- **Determinismo (`SqlDeterminismTests`):** dos corridas byte a byte, 0 escrituras (ni `mtime`), incluido
  `core.autocrlf=true`.
- **Paridad (`SqlGoldenParityTests`):** contra los dorados 1.4; toda diferencia debe estar declarada en
  `declared-differences.json` con motivo y decisión; el test **falla** ante una diferencia funcional no declarada.
- **Edge cases:** entidad sin clave primaria (no genera el procedimiento que la requiere, y el set lo declara);
  relación uno-a-uno y uno-a-muchos (operaciones por relación); consulta del modelo con parámetros y con el texto
  que resuelve sus marcadores; campo de descripción ausente y presente (la función se emite sólo cuando el modelo la
  pide); identificador con caracteres que requieren *quoting*; esquema distinto del por defecto; contenido con el
  separador de lotes dentro de un literal; artefacto preexistente idéntico (no se reescribe) y huérfano (se elimina
  con manifiesto).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

- **Integration tests (real dependencies):** los cinco archivos de `2.0/tests/Cooperator.SqlTemplates.Integration.Tests/`
  (fase F), `Category=Integration`. La aplicación de los scripts la hace el **lane con una herramienta externa**
  (`2.0/build/apply-sql-fixtures.ps1`), **nunca** el motor: es lo que permite verificar sobre una base real sin
  violar ADR-003 §3.1.
- **Contract / E2E against the real providers:** el contrato verificado end-to-end es el de los **objetos de
  programa** emitidos contra el motor de base real: existencia con la firma declarada por el modelo, contrato de
  parámetros, re-aplicación sin error y smoke funcional por familia.
- **Edge cases:** base recién creada y base con los objetos de una corrida previa (re-ejecutabilidad); contenido con
  el separador de lotes; identificador con caracteres especiales; esquema distinto del por defecto; función de
  descripción ausente (no se aplica nada por ella).
- **Guarda de datos:** sólo bases cuyo nombre contiene `cooperator_it`, creadas y descartadas por test; datos
  sintéticos del arnés; ninguna corrida contra una base real de cliente.
- **BUG evidence:** `n/a` — no hay defecto en corrección.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (V-Bounce) y en
> `integration_test_runs[]` del manifiesto (ciclo de integración) (§3.6, §3.12).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El árbol compila con advertencias tratadas como error | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (inventario, modernización, mapeo, post-generación, determinismo, paridad) | `pass` — esperado (AC-1…AC-7) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del consumidor es el **texto emitido** y su re-ejecutabilidad, verificado por barrido y por *drop-guard* | `pass` — esperado (AC-2, AC-3) |
| SAST | V-Bounce | El set emite T-SQL: se revisa que no introduzca ejecución dinámica ni datos embebidos | `pass` — esperado (AC-3) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas; el set es dato del motor | `pass` — esperado |
| OWASP Top 10 — static review | V-Bounce | Sin superficie de red en el alcance; el T-SQL emitido se revisa contra ejecución dinámica y construcción por concatenación de valores | `pass` — esperado (AC-3) |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts; las plantillas son datos, no instrucciones | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | 0 secretos en plantillas, fixtures y scripts emitidos (el modelo no transporta credenciales, ADR-002 §3.1) | `pass` — esperado |
| Hallucination lint | V-Bounce | Tipos, helpers y rutas referenciados existen | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros: el set se deriva del modelo | `pass` — esperado |
| PII / DLP | V-Bounce | `data_classification: internal`; los fixtures son sintéticos y las bases de prueba son desechables | `pass` — esperado |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un BUG Bolt; los **scripts dorados 1.4 existen antes** (Bolt 001) y son la expectativa de partida | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas del árbol emitido | `pass` — esperado (AC-5) |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | El set produce scripts de base, no superficie de los paquetes 2.0 | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | Los artefactos emitidos aplican **sin error** sobre una base limpia y desechable del motor soportado, dos veces; objetos con la firma declarada y smoke funcional | `pass` — **en el ciclo de integración (entorno por provisionar)** |
| Contract / E2E against the real providers | Integration test | Contrato de los objetos de programa contra el motor de base real, con la guarda de datos activa | `pass` — **en el ciclo de integración** |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable; la base se usa como destino de scripts de fixture, no como servicio expuesto | `n/a` — sin superficie ejecutable de red |
| Perf-smoke (p95/p99) | Integration test | El costo de generación lo mide el Bolt 017; acá no hay umbral de latencia | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | No hay servicio instrumentado: el resultado del ciclo es la evidencia de los objetos creados y el smoke | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: required` → `integration_test_plan.applicability: applicable`
> con sus tests, su entorno y sus datos. El Bolt pasa por su ciclo de integración antes de la aceptación.

---

## 10. Security and data

- **El riesgo mayor es el destino de los scripts:** lo que el set emite termina aplicándose contra bases reales.
  Por eso la aceptación incluye tres controles y **no** se sostiene con la referencia dorada sola: la
  **emitibilidad por nodo** (un artefacto por procedimiento, trazado), la **re-aplicación** sin error
  (`drop-guard` modernizado) y la **verificación sobre base limpia** en el ciclo de integración.
- **Confinamiento de la ejecución:** el motor **no** abre conexiones ni ejecuta SQL (ADR-003 §3.1) y este Bolt lo
  verifica por construcción; la aplicación de los scripts en el ciclo de integración la hace una herramienta
  **externa** del lane, nunca el producto. No hay en el alcance ninguna ruta en la que el framework aplique SQL.
- **Guarda de datos del ciclo de integración:** sólo bases cuyo nombre contiene `cooperator_it`, creadas y
  descartadas por test, con datos sintéticos del arnés. Ninguna corrida apunta a una base de cliente — es la
  guarda que el Bolt 001 dejó declarada y este Bolt reutiliza.
- **Sin datos embebidos en lo emitido:** el T-SQL emite **estructura de programa** (parámetros, tipos,
  identificadores) parametrizada por el modelo; ninguna plantilla emite valores de datos ni credenciales (ADR-003
  §4, ADR-002 §3.1).
- **Clasificación:** `data_classification: internal` (espeja el Bolt). El modelo describe esquema, no datos de
  negocio; los fixtures y las bases de prueba son sintéticos y desechables.
- **Integridad del árbol congelado:** `1.4/**` no se modifica; sus plantillas de SP son la referencia funcional que
  se lee, no un archivo que se edite.

---

## 11. Monitoring and observability

- **La observabilidad del set es la del motor:** el resultado y el reporte del Bolt 017 registran, por artefacto,
  si se escribió, se omitió o se eliminó, más los *warnings*; este Bolt aporta el contenido que hace visibles esas
  decisiones.
- **Evidencia versionada:** el inventario publicado (`2.0/docs/template-set-sql.md`), el manifiesto del set, las
  diferencias declaradas de paridad (`2.0/tests/fixtures/sql-parity/declared-differences.json`) y el fragmento de
  rupturas (`2.0/docs/api-removals/US-000.BOLT-020.json`).
- **Evidencia del ciclo de integración:** el reporte del aplicador
  (`2.0/build/apply-sql-fixtures.ps1`), la lista de objetos creados, el resultado del smoke funcional y la salida de
  la re-aplicación, registrados en `integration_test_runs[]` del manifiesto del Bolt.
- **Sin telemetría de servicio:** el set no instrumenta nada (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration:** el consumidor regenera sus scripts con el motor 2.0 y los aplica contra su base con su propio
  pipeline (el framework **no** los aplica). La re-ejecutabilidad (`drop-guard` modernizado) es lo que hace que
  aplicar el set sobre una base con los objetos ya creados no falle — el mismo comportamiento que el consumidor
  esperaba del set legacy.
- **Compatibility:** es una **ruptura de empaquetado y de texto**, no de función: los procedimientos y funciones que
  el modelo declara siguen siendo los mismos, con el mismo contrato de parámetros y el mismo efecto observable. Lo
  que cambia y se declara en las notas (Bolt 028) es: **un archivo por procedimiento** (el script que antes
  reunía varios objetos ya no existe como tal — un consumidor que aplicara "el archivo de SPs" ahora aplica el
  conjunto de archivos, en orden canónico); el **T-SQL modernizado** (comprobación de existencia sobre catálogo
  vigente); la **cabecera sin fecha**; y la **ausencia de Oracle/VB**. La limitación conocida del modelo
  (capacidades que el formato no declara) se **declara**, no se rellena (ADR-002 §3.1).
- **Rollback:** revertir el commit del Bolt (el set son plantillas versionadas). Nada más depende de él que el motor
  que las interpreta; un rollback parcial —por ejemplo, conservar el empaquetado legacy y modernizar el T-SQL—
  dejaría el objetivo del Bolt a medio cumplir y volvería a exponer la fragilidad de la separación por contenido.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **El consumidor aplica estos scripts contra su base**: un error de emisión llega a una base real | 3 | 5 | Es el riesgo más alto de la familia: emitibilidad **por nodo**, paridad declarada contra el dorado y **verificación sobre base limpia** en el ciclo de integración (AC-1, AC-6, AC-8) |
| **Modernizar el T-SQL y romper una comprobación de existencia** (el `drop-guard` deja de funcionar o falla el `IF`) | 3 | 4 | Verificación del texto (`SqlModernizationTests`) **y** re-aplicación sobre la base en el ciclo de integración (AC-3, AC-8) |
| **La separación de lotes vuelve a depender del contenido** | 2 | 4 | Un artefacto por procedimiento (separación estructural) + caso explícito con el separador dentro del contenido (AC-2) |
| **Un script se pierde por empaquetarlo mal** | 2 | 4 | Un artefacto por nodo con trazo en el inventario y colección canónica; el manifiesto del motor detecta huérfanos (AC-1) |
| **Duplicación del mapeo de tipos** (una tabla propia en el set que derive del contrato) | 2 | 4 | El mapeo es el del contrato y se consume por helper; verificación explícita de que no existe una segunda tabla (AC-4) |
| **Ejecución accidental de SQL desde el motor** | 1 | 5 | Contrato de exclusión explícito (ADR-003 §3.1) verificado por ausencia de conexión y de ejecución (AC-7) |
| **Divergencia funcional con el dorado** no detectada (un procedimiento con otro contrato de parámetros) | 3 | 3 | Comparación diferencia por diferencia con lista versionada, y el test **falla** ante diferencia funcional no declarada (AC-6) |
| El entorno de integración no está provisionado y la verificación se posterga | 3 | 2 | El plan es el objetivo declarado; los tests se escriben, compilan y verifican estáticamente en el V-Bounce y el resultado se registra en el ciclo (AC-8, §8.2) |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| La separación de lotes se resuelve **estructuralmente** (un archivo por procedimiento), no con un separador más robusto | ADR-003 §3.1 fija "una plantilla por stored procedure ⇒ un archivo por SP, con lo que el *split* de lotes por el string `GO` **deja de existir**". Un separador "más robusto" seguiría dependiendo del contenido: es la fragilidad que el ADR retira |
| La comprobación de existencia se emite **modernizada** y el set queda re-ejecutable | Es el criterio de F-46/ADR-003 §3.1 y lo que permite aplicar el set sobre una base con los objetos ya creados sin fallar. La re-ejecutabilidad es comportamiento esperado por el consumidor, no un extra |
| La aplicación de los scripts en la verificación la hace una **herramienta externa** del lane | El motor **emite y nunca aplica** (ADR-003 §3.1): si el aplicador fuera el motor, la verificación contradiría la propiedad que se está verificando |
| La paridad se prueba contra los **scripts generados dorados** y las diferencias se registran en un artefacto versionado | ADR-003 §4: el motor legacy no es ejecutable en el runtime objetivo; "diferencia declarada y justificada" sólo es verificable contra una lista |
| El set **no** define su propio mapeo de tipos ni un *quoting* propio | ADR-001 §3.1: el mapeo es dato portable único; dos tablas divergirían con el reverse y con el motor |
| No se reproduce la indirección de **scriptor** ni el esqueleto de Oracle/VB | F-39 y OQ-005: se eliminan por decisión; el conocimiento de SQL Server vive en plantillas + helpers, no en una selección por nombre de clase dentro de un string |
| La limitación conocida del modelo (capacidades que el formato no declara) se **declara** y no se rellena | ADR-002 §3.1: inventar el dato que no existe produciría scripts que el contrato no puede sostener |
| El contenido post-generación se **emite**, no se ejecuta | ADR-003 §3.1: el campo del modelo se conserva como contenido a emitir; ejecutarlo reintroduciría el acoplamiento que el ADR retira |

---

## 15. Stop conditions

> Con `autonomy_level: L2` (riesgo `high`), ante cualquiera de estos casos el V-Bounce **para y reporta** en lugar
> de continuar con una decisión propia.

1. **Cambio material en una fuente gobernada** (Bolt, ADR-003, ADR-001, ADR-002, el REV usado, baseline de código):
   detener, revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca dos revisiones (G16).
2. **El mapeo de tipos del contrato no cubre un tipo nativo que el modelo declara**: detener y reportar — es una
   brecha del contrato (Bolt 003/004), y el set **no** define su propia tabla ni un mapeo de respaldo.
3. **Un artefacto emitido no aplica sin error sobre una base limpia** (el criterio de aceptación por excelencia de
   este Bolt): detener y escalar. No se "arregla" el script a mano ni se relaja la verificación.
4. **La superficie de datos o los helpers de lista blanca del motor (Bolt 017) no alcanzan** para un artefacto:
   detener y reportar; es una brecha de esa superficie.
5. **Una diferencia funcional con el dorado no puede declararse ni resolverse** (por ejemplo, un contrato de
   parámetros distinto sin causa atribuible al contrato del modelo): detener y escalar; es un hallazgo, no un
   detalle.
6. **Se necesita un motor de base distinto**, un artefacto de VB/Oracle o una dependencia nueva: detener y escalar
   (ADR-001 §3.5, OQ-005, ADR-003 §3.6).
7. **El formato del modelo no declara algo que una plantilla necesita** (incluida la limitación conocida sobre
   vistas): se **declara** la limitación y se detiene el artefacto afectado; **no** se inventa el dato
   (ADR-002 §3.1).
8. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
9. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite, dorados o capacidad de SQL Server):
   detener y escalar la secuencia.
10. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, el nombre de un procedimiento que el modelo no
    declara): parar y reportar; no se rellena el hueco con una suposición (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–F implementadas (o, si el presupuesto se agota, progreso en el MEM y pendiente explícito para un
      V-Bounce nuevo — el Bolt estima 2)
- [ ] Todos los criterios de aceptación (AC-1…AC-9) verificados
- [ ] Suite hermética verde (§8.1) y suite de integración escrita, compilada y estáticamente verificada (§8.2)
- [ ] El inventario del set y su manifiesto están publicados y versionados, con el trazo de cada artefacto a su nodo
- [ ] Las diferencias de paridad están declaradas y el fragmento de rupturas existe (insumo del Bolt 028)
- [ ] El código sigue ADR-003 (§3.1/§3.3/§3.5/§3.6), ADR-001 (§3.1/§3.5) y ADR-002 (§3.1)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la lista de rupturas
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado (riesgo `high`: independencia de aprobación **endurecida a nivel de modelo**, §3.0)
- [ ] Plan de integración satisfecho antes de la aceptación: corrida de cobertura pasante en el lane (aplicación
      sobre base limpia, objetos con su firma y smoke funcional), registrada en `integration_test_runs[]`
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-020`](../functional/bolts/US-000.BOLT-020-plantillas-stored-procedures.md) y su manifiesto
  [`US-000.BOLT-020-…json`](../metrics/bolts/US-000.BOLT-020-plantillas-stored-procedures.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) §3.1/§3.3/§3.5/§3.6 ·
  [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1/§3.5 · [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.1
- SPEC prerrequisito directo: [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) (motor, helpers de lista blanca, "emite y no aplica")
- SPECs previos: [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés, dorados 1.4, capacidad de SQL Server y guarda `cooperator_it`) ·
  [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) (fuente única del mapeo) · [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) (contrato del modelo)
- SPECs relacionados: [`SPEC-261016-0029`](./SPEC-261016-0029-plantillas-csharp.md) (el otro set de artefactos) · [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) (`sql` emite estos artefactos)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-46, F-37 y §6 ítem 18
- Evidencia de código 1.4 (verificada en esta sesión): `Tools/CooperatorModeler/Templates/StoredProcedures/Insert.cs:7-8,11-12`
  (`dbo.sysobjects` + `OBJECTPROPERTY(id, N'IsProcedure')`, `drop procedure`, cabecera `-- Author … -- Created <fecha> <hora>`),
  `Queries.cs:7-8,13-16` (la misma comprobación por consulta y la resolución de `<FIELDS>`/`<TABLE>`),
  `DescriptionFunction.cs:15-16` (la comprobación de la **función** con `xtype in (N'FN', N'IF', N'TF')` y `drop function`),
  y el inventario del set (`Insert`, `Update`, `Delete`, `GetOne`, `GetAll`, `GetByParent`, `GetByParents`,
  `DeleteByParents`, `Queries`, `DescriptionFunction`) con `Response.SaveBuffer(<nombre>)` por objeto
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-020`: set de stored procedures con un artefacto por procedimiento trazado al modelo, fin de la separación de lotes por contenido, T-SQL modernizado (sin objetos de sistema obsoletos, sin marca temporal), consumo del mapeo de tipos del contrato, determinismo y paridad contra los scripts dorados de 1.4, con la verificación de aplicación sobre base limpia en el ciclo de integración |

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
| **review.reviewers** | [Dev-validator + dueño de dominio de datos (T-SQL emitido y contrato de los objetos de programa) + dueño de arquitectura (frontera "emite y no aplica" y consumo del mapeo)] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **Riesgo `high` → `autonomy_level: L2`** (§3.3): durante el V-Bounce se para y confirma ante cualquier desvío
   del T-SQL, del conjunto de artefactos o del contrato de parámetros. La aprobación del SPEC y del MEM exige
   independencia **endurecida a nivel de modelo** (§3.0). Confirmar.
2. **La aplicación de los scripts en la verificación la hace una herramienta externa del lane**, no el motor
   (ADR-003 §3.1). Es lo que permite verificar sobre base real sin contradecir la propiedad "emite y nunca aplica".
3. **Entorno del ciclo de integración aún no provisionado:** el plan declara una base limpia y desechable del motor
   soportado con la guarda `cooperator_it` del arnés (Bolt 001). Confirmar el entorno disponible o validar el
   objetivo.
4. **Un archivo por procedimiento cambia el consumo**: quien aplicara "el archivo de SPs" ahora aplica el conjunto
   de archivos. Se declara como ruptura de empaquetado (con su entrada para las notas); confirmar que el cambio es
   esperado.
5. **Método de paridad:** contra los **scripts generados dorados de 1.4** con una lista de diferencias declaradas
   contra la que el test falla. Confirmar el método y la lista mínima (empaquetado, modernización, cabecera).
6. **Limitación conocida del modelo** (capacidades que el formato no declara): se declara y se detiene el artefacto
   afectado; **no** se inventa el dato (ADR-002 §3.1). Confirmar que es el tratamiento esperado.
