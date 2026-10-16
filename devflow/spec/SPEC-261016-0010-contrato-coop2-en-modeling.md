---
id: "SPEC-261016-0010"
title: "Contrato `.coop2` en `Cooperator.Framework.Modeling`: esquema v1 publicado, escritor canónico, lector con recálculo de derivados y conformidad bilateral t2"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-004" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-002", "ADR-001", "ADR-009", "ADR-008"]
prerequisites: ["SPEC-261016-0000-arnes-tests-y-build-reproducible.md", "SPEC-261016-0002-migrador-coop-a-coop2.md", "SPEC-261016-0004-particion-modeling.md"] # arnés/build (transitivo) · fixtures dorados y t1 · modelo portable, POCOs y tabla de mapeo — ver §5
risk_class: "high" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L2" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (refactor): el contrato y su lector/escritor se verifican sobre archivos de modelo de prueba — fixtures del Bolt 002— y el resultado no toca base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:19:59-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:19:59-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:19:59-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-004 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent; su acknowledgment adjudica la enumeración de campos al SPEC), ADR-002/ADR-001/ADR-009/ADR-008 accepted (tech_lead), REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → applicability: n/a con razón; alcance §4 = Bolt §2; exclusiones correctas (t1 en Bolt 002, partición en 003, batch en 029, consumidores 017/018/025)."
    - "ADJUDICACIÓN (lista enumerada §6/A.3) — APROBADA y verificada contra el código 1.4: CollectionTypeEnum = NotApplicable/Entity/Object/EntityList/ObjectList (DomainTree.cs:49-70); DatabaseQueryReturnType = ManyRecords/OneRecord/Scalar/DataReader/DataTable/NoQuery (:2527-2553); OrderEnum = Asc/Desc/Unknown; IncludedEnum = ieNotIncluded/ieIncludedButNotEqual/ieIncludedAndEqual; GenerateAsType : CollectionTypeEnum (DomainTree.cs:2173). Los campos nativeType/clrType/related.type/scalarClrType toman su dominio de la tabla única del Bolt 003 — correcto y no duplicada (ADR-009 §3.2 / ADR-001 §3.6)."
    - "ADJUDICACIÓN (regla de inventario «modelo − exclusiones», §14 ⚠ aprobador) — APROBADA: ADR-002 §3.1 declara su bloque JSON «ilustrativo» y sus reglas «normativas»; el contraejemplo generateAsType (estado editable almacenado, ausente del bloque y de las exclusiones) valida la regla. Coherente con el ADR."
    - "Compliant — escritor canónico (UTF-8 sin BOM, LF, indentación 2, orden fijo por esquema, sin reflexión, determinista) y digest con la semántica del migrador (SHA-256 de bytes canónicos, Coop2Digest); lector único con rechazos tipados que nombran campo/valor/dominio (§C.3), sin coerción silenciosa."
    - "Compliant — conformidad bilateral t2 byte a byte sobre los mismos fixtures del Bolt 002 (§D.1), sin normalización de la comparación (la normalización CRLF→LF se aplica sólo a «no reescribir», nunca al contrato); fixtures consumidos de sólo lectura (G11)."
    - "Compliant — criterios ADR-009 §3.2 verificados como AC-6/AC-7 (enums reales + tabla de mapeo accesible y única), no como recomendación."
    - "ADJUDICACIÓN (riesgo cruzado declarado) — condición y ruteo CORRECTOS: si el inventario exige un campo que el migrador (SPEC-002 revisión 1) no emite → parar y reportar → revisión de SPEC-002 por su executor + V-Bounce nuevo + regeneración de fixtures por ese Bolt (§15 #1). Respeta G11 (un solo executor por Bolt) y G16 (un V-Bounce no abarca dos revisiones). Puede implicar un V-Bounce extra del 002; es el costo correcto, no un ensanchamiento del 004."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-004`. El migrador y su nivel t1
     son del Bolt 002 —este SPEC **no** lo modifica (G11)—; el modelo portable y la tabla de mapeo son del
     Bolt 003; el motor, el CLI y el shell son de los Bolts 017/018/025.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ CRITERIOS DE ACEPTACIÓN CON DUEÑO (ADR-009 §3.2): los **enums reales** de los campos enumerados y la
     **tabla de mapeo tipos SQL↔CLR** son criterio **vinculante** de este Bolt. La lista de campos enumerados
     **no existía en ningún ADR**: el revisor de arquitectura adjudicó que enumerarla es trabajo de **este SPEC**
     (ver el `acknowledgment_reason` del `AITL-BOLT-READY-Approval` del Bolt) y esta revisión la fija en §6/A.3.
  ⚠️ ALCANCE CRUZADO CRÍTICO (§15): si el cierre bilateral revela que el esquema necesita un campo que el
     migrador **no** emite, este Bolt **no** toca el migrador: para y reporta (revisión del Bolt 002 por su
     executor, SPEC revisada y V-Bounce nuevo). Es la única forma de respetar G11 y G16.
-->

# SPEC-261016-0010 — Contrato `.coop2` en `Cooperator.Framework.Modeling`: esquema v1 publicado, escritor canónico, lector con recálculo de derivados y conformidad bilateral t2

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 3) |
| **Bolt** | [US-000.BOLT-004](../functional/bolts/US-000.BOLT-004-contrato-coop2-en-modeling.md) |
| **ADRs** | [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-009](../adrs/ADR-009-shell-del-modeler.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se entrega, dentro de `Cooperator.Framework.Modeling`, el **contrato normativo del archivo de modelo de la
2.0**: el **esquema v1 publicado y versionado**, el **escritor canónico** y el **lector/escritor único** del
formato, con el **recálculo de los derivados y del estado al leer**, la **política de `schemaVersion`** y la
**exposición del contrato** para que la edición y la validación sean dirigidas por esquema —los **enums reales**
de los campos enumerados y la **tabla de mapeo tipos SQL↔CLR** que el Bolt 003 dejó como fuente única—.

Con ello se cierra la **conformidad bilateral de nivel t2** con el migrador (ADR-002 §3.2): el **mismo** fixture
dorado que el Bolt 002 escribió se lee y se reescribe con el lector/escritor del paquete y debe salir **byte a
byte idéntico** (`write(read(fixture)) == fixture`, sin diff). Es el cierre del formato: hasta que esto esté
verde, el corpus convertido del batch es **provisional** (ADR-002 §3.2).

Sin este Bolt: el `.coop2` no tiene contrato normativo (sólo dos implementaciones que se parecen), el corpus
migrado no está cerrado y el editor del shell (Bolt 025) degeneraría en campos de texto libre, porque
ADR-009 §3.2 hace de los enums reales y de la tabla de mapeo un **criterio de aceptación vinculante** de este
Bolt.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` (§4.6 y §6 ítem 3) estableció con evidencia que el `.coop` es un blob
`BinaryFormatter` **sin esquema, sin versión de formato y sin merge posible** (F-41/F-42), y que la
serialización binaria es además un riesgo de deserialización y una dependencia muerta en el runtime objetivo
(F-43). ADR-002 decidió el reemplazo: **`.coop2`** —JSON canónico, con `schemaVersion`, orden fijo, sin
derivados y sin secretos—, con **`Modeling` como único lector/escritor** y con una conformidad probada en **dos
niveles**: t1 en el migrador (`net48`, que no puede consumir `netstandard2.1` y por eso **duplica** el contrato)
y **t2 acá**, con el **test bilateral sobre los mismos fixtures**.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-002 §3.1** — las reglas normativas del formato: `schemaVersion` **entero monotónico** que arranca en
  `1` (el lector acepta `≤` su versión y **rechaza** `>` con error explícito, nunca coerción silenciosa; los
  cambios **aditivos** suben la versión y el lector aplica defaults a los archivos viejos; un cambio
  **rupturista** exige un ADR que reemplace a ADR-002 y un **upgrader explícito**); `producer` es informativo y
  **no** gobierna compatibilidad; **UTF-8 sin BOM**, saltos `\n`, indentación de 2 espacios, sin espacios
  finales, orden de propiedades **fijo por el esquema** (nunca orden de reflexión); las colecciones que son
  **conjuntos** (entidades, propiedades, `queries`, `related`) se ordenan **por nombre, ordinal** y las que son
  **secuencias semánticas** (`primaryKey`, `toString.arguments`, `toString.properties`) **conservan el orden del
  modelo**; **no se serializan** los derivados ni el estado (`haveChanges`/`HaveChanges` y flags por nodo, la
  back-reference `parent`, `children`, `parentCandidates`, `selectedNativePropertiesCount`, los calculados
  —`fullGenerateAs`, `formattedNamespace`, `nameWithoutSchema`— y los predicados derivados —`isCollection`,
  `isEntity`, `isDescriptionField`, `isOneToOneRelation`, `isLazyLoadable`—, el nodo raíz sintético `"Model"`,
  el estado de UI y `FileName`), y **no entran nunca** los secretos, los datos de negocio, las rutas absolutas,
  los artefactos generados, el gate retirado `generateCheckForToken` ni los flags del generador retirados por
  ADR-003; y existe un **límite declarado**: el modelo no distingue vistas de tablas.
- **ADR-002 §3.2/§3.4** — los dos niveles de conformidad y el **test bilateral** como cierre; `Modeling` es el
  **único** lector/escritor del formato y los consumidores (motor y CLI) **no** serializan por su cuenta; el
  migrador usa **la misma versión exacta** del paquete de serialización, fijada de forma centralizada.
- **ADR-001 §3.1/§3.4** — el paquete `Modeling` (`netstandard2.1`) hospeda el contrato; `System.Text.Json` es su
  **única** dependencia de serialización, declarada y fijada en `Directory.Packages.props`.
- **ADR-009 §3.2/§3.8** — la cadena de doble mano con el shell: el editor **no** hardcodea el esquema y **el
  contrato debe exponer** lo necesario para editarlo de verdad —**enums reales** para los campos enumerados y la
  **tabla de mapeo tipos SQL↔CLR** accesible— o el Bolt de `Modeling` **no está terminado**.
- **ADR-008 §3.2/§3.4** — el grafo llega como **POCOs** sin atributos de serialización y su persistencia es el
  `.coop2`, con los derivados recalculados al leer; sin `[Serializable]`/`SerializationInfo` y sin `SYSLIB0051`.

**Qué decide este SPEC, dentro de esas restricciones.** El **inventario de campos** del esquema v1 (el bloque
JSON de ADR-002 §3.1 es **ilustrativo**; las reglas son normativas y el esquema es de este Bolt), la
**enumeración de los campos enumerados** con su dominio (adjudicado al SPEC), la forma publicada del esquema, la
política concreta de recálculo de derivados al leer, el rechazo tipado de valores fuera de dominio y el
procedimiento del cierre bilateral.

**Frontera y alcance cruzado.** Este Bolt **no** toca el migrador (Bolt 002) ni sus fixtures como código: los
**consume** como contrato de prueba. Si el cierre bilateral exigiera un campo que el migrador no emite, la
respuesta **no** es ensanchar este Bolt: es **parar y reportar** para que el Bolt 002 reciba su revisión de SPEC
y su V-Bounce (§15). La tabla de mapeo tipos SQL↔CLR **no** se reimplementa acá: se **expone** la del Bolt 003.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-004`](../functional/bolts/US-000.BOLT-004-contrato-coop2-en-modeling.md) — `status: ready`, `risk_class: high`, `data_classification: internal`, `test_integration: not-required`; `acknowledgment_reason` adjudica al SPEC enumerar los campos enumerados | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:00:56-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-41, F-42, F-43 y §6 ítem 3 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (formato, orden, exclusiones, `schemaVersion`), §3.2 (niveles t1/t2, fixtures, versión exacta de serialización), §3.4 (`Modeling` único lector/escritor) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (paquete `Modeling`, tabla de mapeo como dato portable con fuente única), §3.3 (naming/firma/versionado), §3.4 (`System.Text.Json` única dependencia de serialización, versiones centralizadas) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — §3.2 (criterio de aceptación **vinculante**: enums reales y tabla de mapeo accesible), §3.8 (cadena de doble mano con el shell) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.2 (el grafo como POCOs; la persistencia es el `.coop2`), §3.4 (sin `BinaryFormatter` ni `SYSLIB0051` en el paquete), §3.1 (rupturas documentadas) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-002`](../functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md) — sus **fixtures dorados** y su nivel **t1** son el contrato de prueba de t2 | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-003`](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — el modelo portable, los POCOs y la **tabla de mapeo única** que este Bolt expone | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura): los tipos del modelo
que el esquema debe cubrir —`DBReverseHelper/DomainTree.cs`: `BaseTreeNode`, `EntityNode`, `PropertyNode`
(incluida la propiedad enumerada `GenerateAsType : CollectionTypeEnum`, líneas 2167-2228), `RelatedField`,
`ToStringStruct`, `DatabaseQuery` (con `ReturnType : DatabaseQueryReturnType` y `ScalarCLRType : string`), y los
enums `CollectionTypeEnum` (líneas 49-70) y `DatabaseQueryReturnType` (líneas 2527-2553)— más
`Tools/CooperatorModeler/Snapshot.cs` (los campos de la configuración del modelo y `Language`), y
`DBReverseHelper/DBReverseHelper.cs` (los enums del contrato portable `OrderEnum`, `IncludedEnum`).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- El **esquema v1 normativo** del `.coop2`, **publicado y versionado junto al paquete**: el inventario de
  campos, el orden fijo, los dominios enumerados, las reglas de colecciones (conjunto vs. secuencia), la lista
  explícita de lo que no se serializa y la política de `schemaVersion`.
- El **escritor canónico**: UTF-8 sin BOM, saltos `LF`, indentación de 2 espacios, sin espacios finales, orden
  fijo por el esquema (sin reflexión) y **determinismo total** (el mismo modelo ⇒ los mismos bytes).
- El **lector único** del formato, con el **recálculo de los derivados y del estado al leer** y la aplicación de
  **valores por defecto** a los archivos de versión anterior.
- La **política de `schemaVersion`**: entero monotónico, aceptación de lo igual o anterior, **rechazo explícito**
  de lo superior, y el rechazo tipado de un valor fuera de dominio en un campo enumerado.
- La **exposición del contrato** para edición y validación dirigidas por esquema: los **enums reales** de los
  campos enumerados y la **tabla de mapeo tipos SQL↔CLR** accesible desde el paquete (la del Bolt 003, sin
  duplicarla).
- El **digest del modelo normalizado** con la misma semántica que reporta el migrador (para que el cierre del
  corpus hable de lo mismo en los dos lados).
- La **conformidad bilateral de nivel t2** sobre **los mismos fixtures dorados** del Bolt 002
  (`write(read(fixture)) == fixture`, byte a byte, sin diff) y su evidencia.

### Out of scope

- El **migrador**, su CLI, su reporte y su round-trip interno: **Bolt 002** (nivel t1). Este SPEC lo consume y
  **no** lo modifica (G11); una divergencia se rutea a su Bolt (§15).
- El **modelo portable**, los POCOs, la rotura del acoplamiento y la **tabla de mapeo como artefacto único**:
  **Bolt 003** (se exponen acá, no se crean).
- El **batch real del corpus**, el archivo y el retiro de los originales: **Bolt 029**.
- El **motor de generación**, sus plantillas y el **CLI**: ADR-003 y Bolts 017-021 (el motor **consume** el
  contrato, no lo define).
- El **shell del Modeler** y su editor dirigido por esquema: **Bolt 025** (este Bolt entrega lo que ese editor
  necesita consumir).
- **Capacidades nuevas del modelo**: distinguir tabla de vista, o cualquier campo que el modelo de origen no
  tiene. La limitación se reporta; no se rellena con un dato inventado (ADR-002 §3.1).
- La **ubicación del contrato** y el **slot de cualquier paquete**: fijados por ADR-001 §3.1.
- La **subida de `schemaVersion`** o un **upgrader**: sólo con un ADR que reemplace a ADR-002 (§3.1).

---

## 5. Prerequisites and baseline

- **Bolt 003 y su SPEC canónica
  ([`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md), `prerequisites`):** el paquete
  `Cooperator.Framework.Modeling` (`netstandard2.1`, sin aristas salientes) con el modelo como POCO, el contrato
  portable del reverse y **la tabla de mapeo tipos SQL↔CLR como artefacto único**. Sin él no hay tipos que
  serializar ni tabla que exponer.
- **Bolt 002 y su SPEC canónica
  ([`SPEC-261016-0002`](./SPEC-261016-0002-migrador-coop-a-coop2.md), `prerequisites`):** los **fixtures
  dorados** (`2.0/tests/fixtures/model-corpus/<slug>/model.coop` + `model.coop2` byte a byte, con su `INDEX.md`
  de procedencia) y su **t1**. Son el contrato de prueba del cierre bilateral y se consumen **de sólo lectura**.
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés de tests, la
  configuración de build compartida (`Directory.Build.props`: analizadores, determinismo, warnings como error,
  `AssemblyVersion`/firma) y las **versiones centralizadas** (`Directory.Packages.props`). Es prerrequisito
  transitivo: sin arnés no hay dónde correr la suite ni el gate que verifica la superficie.
- **La misma versión exacta del paquete de serialización** que usa el migrador, fijada **una sola vez** en
  `Directory.Packages.props` (ADR-002 §3.2, ADR-001 §3.4). Si los dos lados no comparten versión exacta, la
  conformidad byte a byte **no es alcanzable** y el formato de escape puede divergir: es condición de parada
  (§15), no un detalle.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de campos de §6/A.1.
- **Build state de partida:** `Modeling` compila y su suite corre (Bolt 003) con **cero dependencias de
  terceros**; este Bolt incorpora `System.Text.Json` como su **única** dependencia, con la versión centralizada.
- **Decisión de ejecución registrada (no es supuesto):** nada de este Bolt se verifica contra una base de datos
  ni contra un servicio: la conformidad es **sobre archivos** y por eso el plan de integración es `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-004`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — El esquema v1: inventario, orden, dominios y política de versión

**Duration:** 4h total cycle — **Complexity:** High

#### A.1 Inventario de campos y regla de completitud

El **bloque JSON de ADR-002 §3.1 es ilustrativo**; las **reglas** de esa cláusula son normativas y el **esquema
es de este Bolt** (ADR-002 §3.1: "el esquema normativo es el contrato JSON versionado que produce el Bolt de
`Modeling` con estas reglas"). La regla de completitud que este SPEC fija es una sola:

> **Se serializa todo campo del modelo que no esté en la lista de exclusiones de ADR-002 §3.1**, con el nombre
> que el esquema declare. La lista de exclusiones —derivados, estado y lo que no entra nunca— es **cerrada** y
> se aplica tal cual.

El inventario se levanta de los tipos del modelo del Bolt 003 (que son los portados de `DomainTree.cs`:
`BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery`, sus enums) y de
la configuración del modelo que el formato transporta (`databaseName`, `spPrefix`, `projects`, `deployFolder`,
`language`, `postGenerationScript`). El inventario **se publica con el esquema** y su revisión contra los tipos
es explícita: un campo del modelo que quede fuera sin estar en la lista de exclusiones es un **defecto del
esquema**, no una decisión (§13, §15).

**Files created:**
- `2.0/src/Cooperator.Framework.Modeling/Contract/coop2-v1.schema.json` — el **esquema v1 publicado y versionado**: campos, orden, tipos, dominios enumerados, obligatoriedad y reglas de colección
- `2.0/src/Cooperator.Framework.Modeling/Contract/Coop2Schema.cs` — la proyección mecánica del esquema en código: orden de campos, nombres, obligatoriedad y dominios; **nunca** orden por reflexión
- `2.0/src/Cooperator.Framework.Modeling/Contract/SchemaVersionPolicy.cs` — versión soportada, aceptación `≤`, rechazo `>`, y la aplicación de defaults por versión

#### A.2 Orden, colecciones y exclusiones

- **Orden fijo por el esquema** (ADR-002 §3.1): el orden de propiedades lo dicta `coop2-v1.schema.json`, no el
  orden de declaración de los tipos ni la reflexión. Un campo nuevo del formato obliga a declararlo en el
  esquema y a subir `schemaVersion`.
- **Colecciones**: las que son **conjuntos** (entidades, propiedades, `queries`, `related`) se escriben
  ordenadas **por nombre, ordinal**; las que son **secuencias semánticas** (`primaryKey`, `toString.arguments`,
  `toString.properties`) **conservan el orden del modelo**, porque alimentan código generado y el formato de
  `ToString`.
- **`producer` es informativo y no gobierna compatibilidad** (ADR-002 §3.1): se escribe con la herramienta y su
  versión, y el lector **no** lo usa para decidir nada. Reemplaza al `ModelerVersion` legado, que no era un dato
  de formato.
- **Lo que no se serializa** (lista cerrada de ADR-002 §3.1) se documenta **campo por campo** en el esquema, con
  su recálculo asociado (§C.2): es lo que hace que el archivo sea diff-friendly y que no arrastre ruido.

#### A.3 Los campos enumerados y la exposición de sus dominios (criterio vinculante, ADR-009 §3.2)

La lista de campos enumerados **no existía en ningún ADR**: el `AITL-BOLT-READY-Approval` del Bolt adjudicó
enumerarla a este SPEC. Queda fijada así —estos son **todos** los campos enumerados del modelo y los dominios
que el contrato debe exponer como **valores enumerados reales**, nunca como texto libre—:

| Campo | Tipo | Dominio enumerado | Origen en el modelo 1.4 |
|---|---|---|---|
| `model.language` | enumerado del formato | `C#` · `VB` | `Snapshot.Language` (`CS` \| `VB`, según `GenerationDialog.cs:120-122`) |
| `entities[].properties[].generateAsType` | `CollectionTypeEnum` | `NotApplicable` · `Entity` · `Object` · `EntityList` · `ObjectList` | `PropertyNode.GenerateAsType` (`DomainTree.cs:2167-2228`), campo **almacenado** del modelo y editable |
| `entities[].queries[].returnType` | `DatabaseQueryReturnType` | `ManyRecords` · `OneRecord` · `Scalar` · `DataReader` · `DataTable` · `NoQuery` | `DatabaseQuery.ReturnType` (`DomainTree.cs:2527-2553`, `:2642-2650`) |
| `entities[].properties[].nativeType` | dominio del **mapeo de tipos** | las claves nativas de la tabla única del Bolt 003 | `PropertyNode.NativeType` |
| `entities[].properties[].clrType` | dominio del **mapeo de tipos** | los tipos CLR de la tabla única del Bolt 003 | `PropertyNode.CLRType` |
| `entities[].properties[].related[].type` | dominio del **mapeo de tipos** | los tipos CLR de la tabla única del Bolt 003 | `RelatedField` (`DomainTree.cs:2306`) |
| `entities[].queries[].scalarClrType` | dominio del **mapeo de tipos** | los tipos CLR de la tabla única del Bolt 003 | `DatabaseQuery.ScalarCLRType` (`DomainTree.cs:2652-2660`) |
| *(fuera del documento)* `OrderEnum` del contrato portable | enumerado del contrato | `Asc` · `Desc` · `Unknown` | `Reverse/DBReverse.cs` (Bolt 003) |
| *(fuera del documento)* `IncludedEnum` del contrato portable | enumerado del contrato | `ieNotIncluded` · `ieIncludedButNotEqual` · `ieIncludedAndEqual` | `Reverse/DBReverseHelper.cs` (Bolt 003) |

- **Exposición**: los dominios se exponen como **valores consultables en tiempo de ejecución** (el consumidor
  los lee del contrato, no los copia), junto con el nombre del campo y la obligatoriedad: es lo que habilita el
  editor dirigido por esquema y la validación del Bolt 025 **sin duplicar el esquema en la interfaz**
  (ADR-009 §3.2).
- **Tabla de mapeo de tipos**: se **expone** la del Bolt 003 (fuente única, ADR-001 §3.1/§3.6) como dominio de
  los campos tipados del documento. Este SPEC **no** declara una segunda tabla ni un segundo conversor
  (§7/AC-7).
- **`language` y la deprecación de VB**: el dominio del formato incluye `VB` porque el dato existe en el modelo
  de origen; qué se hace con él es decisión **de otros** (ADR-002 §3.3 y ADR-001 §3.5 gobiernan el bloqueo del
  migrador y la ausencia de plantillas VB en el motor). El contrato **no** inventa una deprecación nueva ni la
  relaja: expone el dominio y rechaza un valor fuera de él (§C.3).

---

### Phase B — El escritor canónico

**Duration:** 4h total cycle — **Complexity:** High

#### B.1 Escritura determinista

El escritor construye el documento con **`System.Text.Json`** sobre **`Utf8JsonWriter`**, emitiendo el orden
**fijo del esquema** y sin reflexión: **UTF-8 sin BOM**, saltos `\n` (`LF`), indentación de **2 espacios**, sin
espacios finales de línea y **determinismo total** —el mismo modelo produce **bytes idénticos**— (ADR-002 §3.1).
Las reglas de escape y de formato son las del escritor canónico del migrador: es la condición de la conformidad
byte a byte, y por eso la **versión exacta** del paquete de serialización es la misma en ambos lados
(ADR-002 §3.2).

**Files created:**
- `2.0/src/Cooperator.Framework.Modeling/Contract/Coop2Writer.cs` — el escritor canónico del `.coop2`
- `2.0/src/Cooperator.Framework.Modeling/Contract/Coop2Digest.cs` — SHA-256 del documento canónico (digest del modelo normalizado, misma semántica que el reporte del migrador)

#### B.2 Escritura sólo si cambia (idempotencia de archivo)

El escritor expone una operación que **no reescribe** un archivo cuyo contenido calculado es idéntico al
existente: es la regla que el motor necesita para que dos corridas del mismo modelo produzcan el mismo diff y
para que un checkout con `core.autocrlf` no vea todo el árbol como cambiado (ADR-003 §3.3 define la regla para
los artefactos generados; acá se ofrece la misma semántica para el archivo de modelo, que es el insumo del flujo
agente). La comparación **normaliza `CRLF`→`LF`** antes de decidir.

---

### Phase C — El lector único y el recálculo de los derivados

**Duration:** 4h total cycle — **Complexity:** High

#### C.1 Lectura y verificación del contrato

El lector es el **único** del formato (ADR-002 §3.4): los consumidores —motor, CLI, shell— leen a través de él y
**no** interpretan el documento por su cuenta. Al leer: `schemaVersion` se compara con la soportada; los campos
obligatorios ausentes y los valores fuera de dominio se rechazan con error explícito (§C.3); los archivos de
**versión anterior** se leen con los **valores por defecto** que el esquema declara para cada campo (ADR-002
§3.1).

**Files created:**
- `2.0/src/Cooperator.Framework.Modeling/Contract/Coop2Reader.cs` — el lector único del formato
- `2.0/src/Cooperator.Framework.Modeling/Contract/ModelRecalculator.cs` — el recálculo de los derivados, los predicados y el estado al leer

#### C.2 Recálculo al leer (lo que el archivo no arrastra)

Al leer se **recalculan** —nunca se leen del archivo— los valores que ADR-002 §3.1 excluye: `haveChanges` y los
flags por nodo, la back-reference `parent`, los hijos, `parentCandidates`,
`selectedNativePropertiesCount`, los calculados (`fullGenerateAs`, `formattedNamespace`, `nameWithoutSchema`) y
los predicados derivados (`isCollection`, `isEntity`, `isDescriptionField`, `isOneToOneRelation`,
`isLazyLoadable`), además del nodo raíz sintético (`entities[]` se aplana) y del estado de UI. El recálculo se
hace **a partir del propio dato persistido** del modelo (declaraciones del esquema, relaciones y opciones), que
es lo que hace que "el archivo no arrastre ruido" (F-42) sin perder comportamiento.

**Nota de fidelidad (punto que el V-Bounce debe cerrar y no suponer):** si al implementar el recálculo aparece
un derivado que **no** puede reproducirse desde el dato persistido sin pérdida, la respuesta **no** es guardar el
derivado en el archivo (rompería la regla y ensuciaría el diff) ni degradarlo en silencio: es una **brecha del
inventario de campos** y se resuelve **antes** de cerrar el esquema, con el campo almacenado que corresponda
(§A.1) — y si esa resolución exigiera una **capacidad nueva** del modelo, para y se rutea a ADR (§15).

#### C.3 Rechazos explícitos (nunca coerción silenciosa)

| Situación | Resultado |
|---|---|
| `schemaVersion` **mayor** a la soportada | Error explícito del contrato: versión encontrada, versión soportada y qué hacer (nunca coerción silenciosa, ADR-002 §3.1) |
| `schemaVersion` **ausente**, no entera o **menor a 1** | Error explícito del contrato (el archivo no es un `.coop2` válido) |
| **Valor fuera de dominio** en un campo enumerado | Error explícito que **nombra el campo, el valor y el dominio admitido** |
| **Campo obligatorio ausente** | Error explícito que nombra el campo y su ubicación en el documento |
| `schemaVersion` **igual o anterior** a la soportada | Se acepta; los campos que el esquema declara con default y no vienen en el archivo se completan con su default |

**Files created:**
- `2.0/src/Cooperator.Framework.Modeling/Contract/Coop2ContractException.cs` — la excepción tipada del contrato (versión, campo, valor, dominio), con el mensaje accionable
- `2.0/src/Cooperator.Framework.Modeling/Contract/EnumeratedDomains.cs` — la exposición de los dominios de §A.3 y el acceso a la tabla de mapeo de tipos del Bolt 003 (sin duplicarla)

---

### Phase D — La conformidad bilateral (nivel t2)

**Duration:** 3h total cycle — **Complexity:** Medium

#### D.1 El test bilateral sobre los mismos fixtures

El cierre es mecánico y no admite interpretación: para **cada** fixture dorado del Bolt 002
(`2.0/tests/fixtures/model-corpus/<slug>/model.coop2`), se **lee** con el lector del contrato y se **reescribe**
con el escritor canónico, y el resultado debe ser **byte a byte idéntico** al fixture
(`write(read(fixture)) == fixture`, **sin diff**) — ADR-002 §3.2. La comparación es **byte a byte**, no
"semántica": un orden distinto, un espacio de más o una diferencia de escape es un **fallo**, no un aviso.

La corrida cubre el conjunto **completo**, incluidos los fixtures correspondientes al subconjunto representativo
del corpus real (el `INDEX.md` del Bolt 002 declara cuáles son): un caso feliz **no** es cobertura (riesgo
declarado en el Bolt).

**Files created:**
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/Cooperator.Framework.Modeling.Contract.Tests.csproj` — proyecto de pruebas `net10.0` del contrato (framework y versiones los fija el Bolt 001)
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/BilateralConformityTests.cs` — el test bilateral sobre los fixtures del Bolt 002, byte a byte
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/CanonicalWriterTests.cs` — orden de conjuntos vs. secuencias, `LF`, sin BOM, sin espacios finales y **determinismo** (dos escrituras ⇒ bytes idénticos)
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/SchemaVersionTests.cs` — los casos negativos de §C.3 y los defaults de los archivos de versión anterior
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/EnumerationExposureTests.cs` — los dominios de §A.3 expuestos como valores reales; la tabla de mapeo accesible y **única**
- `2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/RecalculationTests.cs` — los derivados y el estado de la lista de exclusión **no** están en el archivo y **sí** están recalculados tras leer

#### D.2 Evidencia y cierre de t2

El MEM registra el **estado t2** de ADR-002 §3.2 con: la corrida del test bilateral y su resultado **sin
diferencias** sobre el conjunto completo; la evidencia de determinismo; los casos negativos de versión y de
dominio; la constancia de que el contrato expone los enums reales y la tabla de mapeo; y la constancia de que el
corpus convertido del batch **deja de ser provisional** —es la entrada del Bolt 029—. Se registran además las
rupturas de superficie que este Bolt introduzca, para las notas de migración (ADR-008 §3.1; el documento es del
**Bolt 028**).

---

### Phase E — Cierre del formato y aviso de frontera

**Duration:** 2h total cycle — **Complexity:** Low

- **Cierre**: el esquema v1 queda **publicado y versionado junto al paquete** (artefacto del paquete, no un
  apunte de taller): quien consuma el formato lee el esquema y los dominios del propio paquete.
- **Frontera con el motor**: el motor (Bolt 017) y el CLI (018) **consumen** este contrato; si su Bolt necesita
  algo que el contrato no expone, es una **revisión material de este SPEC**, no una serialización paralela
  (ADR-002 §3.4 lo prohíbe explícitamente).
- **Frontera con el shell**: los dos criterios con dueño (enums reales y tabla de mapeo) se **verifican acá**;
  su **consumo** se demuestra en el Bolt 025 (ADR-009 §3.8).

**Files created:**
- `2.0/src/Cooperator.Framework.Modeling/Contract/README-contrato.md` — el contrato y su esquema: versión, dominio de cada campo enumerado, la política de evolución (`≤` acepta, `>` rechaza, aditivo sube versión, rupturista exige ADR + upgrader) y la referencia a los fixtures como contrato de prueba

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan. Los criterios 6 y 7 son los **criterios con dueño** de
> ADR-009 §3.2, verificados como aceptación y no como recomendación.

### AC-1: El esquema v1 está publicado y versionado con el paquete

**Given** el paquete `Modeling` construido
**When** se inspecciona su contenido y su superficie
**Then** el esquema v1 del formato está publicado y versionado junto al paquete, declara el inventario de
campos, el orden, los dominios enumerados, la obligatoriedad y la política de `schemaVersion`, y coincide con lo
que el escritor y el lector aplican

### AC-2: La escritura es determinista

**Given** un modelo cargado
**When** se escribe **dos veces** el mismo modelo
**Then** los dos documentos son **byte a byte idénticos** (mismo orden, mismo escape, mismos saltos `LF`, sin
BOM y sin espacios finales), y una tercera escritura sobre un archivo ya idéntico **no lo reescribe** (§B.2)

### AC-3: La conformidad bilateral cierra sin diferencias (t2)

**Given** **los mismos** fixtures dorados del Bolt 002
**When** cada `model.coop2` se lee con el contrato y se reescribe con el escritor canónico
**Then** el resultado es **byte a byte idéntico** al fixture, **sin diff**, para el conjunto completo —incluido
el subconjunto representativo del corpus real— y el MEM registra el **estado t2**

### AC-4: La política de versión se cumple en los dos sentidos

**Given** un archivo con `schemaVersion` **mayor** a la soportada
**When** se intenta leerlo
**Then** falla con error explícito que nombra la versión encontrada y la soportada, **sin** coerción silenciosa
**And Given** un archivo de **versión anterior** con campos con default ausentes
**Then** se lee aplicando el default que el esquema declara

### AC-5: Los derivados y el estado no viajan en el archivo y se recalculan al leer

**Given** un documento del formato
**When** se inspecciona su contenido
**Then** **ninguno** de los valores de la lista de exclusión de ADR-002 §3.1 está presente (flags de cambios,
back-reference `parent`, hijos, `parentCandidates`, contadores, calculados y predicados, estado de UI,
`FileName`)
**And When** se lee el archivo
**Then** el modelo resultante tiene esos valores **recalculados** y el comportamiento observable de los tipos
del modelo (Bolt 003) no cambia

### AC-6: El contrato expone los enums reales de los campos enumerados (criterio con dueño)

**Given** el contrato construido
**When** un consumidor consulta los campos enumerados en tiempo de ejecución
**Then** obtiene, para **cada** campo de la tabla de §6/A.3, su **dominio de valores enumerados reales** y su
obligatoriedad —nunca cadenas libres— y el conjunto coincide exactamente con la tabla (ni un campo enumerado
fuera de la lista, ni un dominio incompleto)

### AC-7: La tabla de mapeo tipos SQL↔CLR es accesible y sigue siendo única (criterio con dueño)

**Given** el contrato construido
**When** un consumidor pide el tipo nativo y el tipo CLR de una propiedad
**Then** el contrato le entrega el dato del **mapeo único** entregado por el Bolt 003 —sin una segunda tabla y
sin un segundo conversor en el paquete ni en el tooling— y el dominio de los campos tipados del documento
coincide con las entradas de ese mapeo

### AC-8: Un valor fuera de dominio se rechaza nombrando el campo

**Given** un documento con un valor que no pertenece al dominio de un campo enumerado
**When** se intenta leerlo
**Then** falla con error explícito que nombra el **campo**, el **valor** y el **dominio admitido**, sin
degradarlo a un valor por defecto ni a texto libre

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — esquema v1 publicado y versionado como contrato normativo | §6/A.1, §6/E | AC-1 · esquema + `README-contrato.md` + inspección del paquete |
| Bolt §2 — el mismo modelo produce bytes idénticos en cada escritura | §6/B.1 | AC-2 · `CanonicalWriterTests` |
| Bolt §2 — `write(read(fixture)) == fixture` byte a byte sobre los fixtures del Bolt 002 | §6/D.1 | AC-3 · `BilateralConformityTests` + estado t2 en el MEM |
| Bolt §2 — versión superior rechazada con error explícito; versión anterior con defaults | §6/C.1, §6/C.3 | AC-4 · `SchemaVersionTests` |
| Bolt §2 — derivados y estado no se serializan, se recalculan al leer | §6/C.2 | AC-5 · `RecalculationTests` |
| Bolt §2 / ADR-009 §3.2 — el contrato expone los **enums reales** de los campos enumerados (criterio vinculante) | §6/A.3, §6/C.3 | AC-6, AC-8 · `EnumerationExposureTests` |
| Bolt §2 / ADR-009 §3.2 / ADR-001 §3.1 — la **tabla de mapeo tipos SQL↔CLR** accesible y única | §6/A.3, §6/C.3 | AC-7 · `EnumerationExposureTests` |
| Bolt §2 — `Modeling` como único lector/escritor (ADR-002 §3.4) | §6/C.1, §6/E | AC-1, AC-3 · contrato único + frontera declarada con motor/CLI/shell |
| Bolt §2 — el corpus migrado deja de ser provisional al cerrar t2 | §6/D.2 | AC-3 · MEM (t2) y habilitación del Bolt 029 |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests:** escritor (orden de campos por esquema, conjuntos ordenados por nombre ordinal, secuencias en
  orden del modelo, `LF`, sin BOM, sin espacios finales, indentación de 2, determinismo); lector (campos
  obligatorios, defaults por versión, recálculo de cada valor de la lista de exclusión); política de versión
  (`<`, `=`, `>`, ausente, no entera); dominio de cada campo enumerado de §A.3 y su rechazo cuando el valor está
  fuera; digest del documento (misma entrada ⇒ mismo digest, y sensible a cualquier byte).
- **Component tests:** el contrato como unidad: **escribir → leer → volver a escribir** sobre los fixtures
  dorados del Bolt 002 con comparación **byte a byte**; escritura sobre archivo idéntico sin reescritura;
  `schemaVersion` alta rechazada; documento con valor fuera de dominio rechazado. Para los casos negativos que
  no pueden venir de un fixture entregado (por ejemplo un `schemaVersion` futuro o un enum inválido) el caso se
  construye **modificando un fixture en memoria o en un directorio temporal** —nunca editando el fixture del
  repositorio— y la modificación se describe en el MEM.
- **Consumer-side contract tests (test doubles):** `n/a` — el contrato no consume un servicio ni un componente
  externo: su contrato es el **documento** y su verificación es la conformidad byte a byte contra fixtures
  (un doble de prueba no puede sustituir al archivo). La frontera con el consumidor real (el editor) se
  demuestra en el Bolt 025, que es su dueño.
- **Edge cases:** documento con `entities` vacío; entidad sin propiedades y sin `queries`; colecciones con
  nombres que difieren sólo en mayúsculas o con acentos (orden ordinal); `primaryKey` con varios campos (orden
  preservado); `toString.arguments` repetidos (secuencia, no conjunto); `related[]` con varias entradas sobre
  una misma propiedad; `generateAsType` en cada uno de sus cinco valores; `returnType` en cada uno de sus seis
  valores; `language` en `C#` y en `VB`; cadenas con caracteres que obligan a escape (`"`, `\`, `\n`, TAB),
  acentos y comillas; documento con sólo espacios/orden distinto (debe fallar el byte a byte); `schemaVersion`
  `1`, `0`, `2` y no entero; campos obligatorios ausentes; documento vacío y documento truncado; archivo con
  `CRLF` (normalización en la comparación de "no reescribir", nunca en el contrato del archivo).
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida del test bilateral sobre el **conjunto completo** de
  fixtures con su resultado sin diferencias; la corrida de determinismo (dos escrituras, hashes iguales); los
  casos negativos de versión y de dominio con su mensaje; la constancia de la exposición de dominios y del
  mapeo; el inventario de campos del esquema revisado contra los tipos del modelo. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `refactor` sobre archivos: el contrato y su
  lector/escritor se verifican sobre archivos de modelo de prueba. No hay base de datos, cola, servicio,
  almacenamiento ni entorno desplegado contra el que verificar nada (el `.coop` es autocontenido y el migrador
  no abre conexión, ADR-002 §3.2).
- **Contract / E2E against the real providers:** `n/a` — el único "contrato entre componentes" que cambia es el
  del **formato**, y su verificación es **bilateral y sobre archivos** (test bilateral, AC-3): un E2E contra
  proveedores reales no aplica a este cambio. El consumo real del contrato por el shell se demuestra en el
  Bolt 025 y su empaquetado en el 027.
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
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); cero `SYSLIB0051`; `Modeling` con **una sola** dependencia de terceros (`System.Text.Json`, versión centralizada) | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluido el **test bilateral** sobre el conjunto completo de fixtures | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el contrato del formato se verifica contra los archivos (byte a byte), no contra un doble; su consumidor real (el editor) es del Bolt 025 | n/a (razón) |
| SAST | V-Bounce | 0 hallazgos altos; el lector valida antes de construir (versión, obligatorios, dominio) y no ejecuta nada del documento | pass |
| Dependency scanning / licenses | V-Bounce | `System.Text.Json` (MIT, versión **exacta** compartida con el migrador) y ninguna otra; licencias permisivas (ADR-001 §3.4) | pass |
| OWASP Top 10 — static review | V-Bounce | Deserialización: el contrato **no** usa serialización binaria ni tipado polimórfico por nombre de tipo; lee un documento con esquema fijo, valida versión, obligatorios y dominios, y falla cerrado. Sin `BinaryFormatter` en el paquete (ADR-008 §3.4) | pass |
| Prompt-injection scan | V-Bounce | `n/a` — el contrato no interpreta instrucciones ni procesa lenguaje natural: valida un documento con esquema; los valores se tratan como datos | n/a (razón) |
| Secret-leak scan | V-Bounce | 0 secretos en el esquema, en el paquete y en los fixtures (los campos de secreto **no existen** en el formato: la cadena de conexión está excluida por ADR-002 §3.1) | pass |
| Hallucination lint | V-Bounce | Cada campo del esquema trazable a un miembro del modelo del Bolt 003 o a una regla de ADR-002 §3.1; 0 campos inventados; 0 dominios enumerados fuera de la tabla de §6/A.3 sin su origen en el código 1.4 | pass |
| IP / license provenance | V-Bounce | Código propio; 0 código de terceros copiado; el esquema es un artefacto del proyecto | pass |
| PII / DLP | V-Bounce | `internal`: el formato describe estructura del modelo, **no** datos de negocio ni datos personales; los fixtures son del Bolt 002 (anonimizados/sintéticos) y se consumen **de sólo lectura** | pass |
| Dependency-confusion | V-Bounce | 0 paquetes nuevos más allá del autorizado y fijado centralmente; ningún ID no calificado | pass |
| Test-first evidence | V-Bounce | El **fixture dorado se escribe antes**: la expectativa viene de los archivos del Bolt 002 (y, en el fixture de referencia, redactada a mano desde ADR-002 §3.1), no de la salida de este escritor. El inventario del esquema se levanta de los tipos **antes** de escribir el escritor, y la tabla de dominios enumerados de §6/A.3 se fija en el SPEC antes de implementarla | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas del test bilateral y dos escrituras del mismo modelo producen resultados y bytes idénticos | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: la conformidad del formato es bilateral y sobre archivos (§8.2) | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: el contrato se evalúa sobre archivos de prueba; su costo real aparece en el batch y en la generación, con Bolt propio | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: biblioteca sin runtime propio (§11) | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). El documento del formato describe la
  **estructura** del modelo del consumidor: información interna, sin datos de negocio (el archivo describe el
  modelo, no el contenido de la base) y **sin secretos por diseño** —la cadena de conexión está excluida del
  formato por ADR-002 §3.1 y su ausencia es una regla del esquema, no una omisión—.
- **Sin serialización binaria en el paquete:** ADR-008 §3.4 la prohíbe fuera del migrador; el contrato lee JSON
  con esquema fijo y **no** resuelve tipos por nombre desde el documento (nada del archivo decide qué se
  instancia ni qué código se ejecuta): el lector construye los tipos del modelo que conoce.
- **Falla cerrada:** versión no soportada, campo obligatorio ausente o valor fuera de dominio ⇒ error explícito
  y ningún objeto a medio construir (§C.3). Sin coerción silenciosa y sin "lo mejor posible".
- **Higiene de `System.Text.Json`:** sin *source generation* ni *resolver* reflexivo que exponga tipos no
  previstos; sin `JsonSerializerOptions` permisivos (sin `AllowTrailingCommas`, sin comentarios, sin
  propiedades desconocidas ignoradas); el contrato nombra los campos explícitamente. Es lo que sostiene el
  determinismo y el rechazo tipado.
- **Dependencias:** `System.Text.Json` es la **única** dependencia de serialización del paquete autorizada por
  ADR-001 §3.4 y su **versión exacta** se comparte con el migrador (ADR-002 §3.2): si divergiera, no sólo se
  rompería la conformidad byte a byte, sino que aparecerían dos formatos de escape distintos para el mismo
  formato.
- **Fixtures de otros:** los fixtures del Bolt 002 se consumen **de sólo lectura** y no se editan ni se
  regeneran acá (G11): una edición de los fixtures en este Bolt sería, además de una violación de propiedad, la
  forma más fácil de fabricar un cierre bilateral falso. La comparación byte a byte sobre archivos comiteados
  es justamente el control que lo impide.
- **Integridad del esquema publicado:** el esquema y los dominios enumerados son parte del paquete: un cambio
  silencioso del esquema (sin subir versión y sin ADR) es una violación de ADR-002 §3.1 y el gate de API más la
  suite deben hacerlo fallar.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es una biblioteca portable. No emite logs, métricas ni
  trazas, y **no** se le agrega instrumentación: ninguna fuente lo pide y el contrato no tiene ciclo de vida que
  observar.
- **La observabilidad de este Bolt es su evidencia de cierre:** el **test bilateral sin diferencias** sobre el
  conjunto completo de fixtures, la corrida de **determinismo**, los **casos negativos** de versión y de dominio
  con su mensaje, y la constancia de la **exposición** de los dominios enumerados y de la tabla de mapeo. Un
  revisor audita el cierre del formato leyendo esos artefactos y el **estado t2** del MEM.
- **Diagnóstico en runtime (lo que sí existe y es parte del entregable):** los errores del contrato son
  **tipados y accionables** (versión, campo, valor, dominio, ubicación). Es el análogo, en este Bolt, de lo que
  ADR-006 §3.3 hace para los providers: un fallo en el punto de configuración no puede ser un mensaje genérico.
- La telemetría del shell, del motor y del CLI es de sus Bolts (025/017/018); este SPEC no la anticipa.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de archivos en este Bolt: el **migrador** es del Bolt 002 y el **batch
  real** es del Bolt 029. Lo que este Bolt decide es la **política de evolución del formato** (ADR-002 §3.1):
  versión **monotónica** desde `1`; el lector acepta `≤` y rechaza `>` con error explícito; un cambio **aditivo**
  (un campo opcional nuevo) sube la versión y el lector aplica defaults a los archivos viejos; un cambio
  **rupturista** exige un **ADR que reemplace a ADR-002** y un **upgrader explícito** como entregable propio
  (no un parche silencioso en el lector). Este Bolt **no** sube la versión: entrega la v1.
- **Compatibility:** el consumidor del formato son el motor, el CLI y el shell, todos dentro del árbol 2.0 y con
  Bolt propio; el 1.4 sigue en su formato binario y queda congelado. Lo que **sí** cambia respecto de 1.4 es el
  archivo de modelo: el `.coop2` reemplaza al `.coop` —ruptura ya decidida por ADR-002— y sus notas de
  migración (documento del Bolt 028) reciben de acá: (a) el formato nuevo y su esquema publicado, (b) la
  política de versión que un consumidor debe respetar si alguna vez escribe el archivo por su cuenta —lo cual
  ADR-002 §3.4 desaconseja: el contrato es el único lector/escritor— y (c) la lista de campos que dejan de
  existir en el archivo por ser derivados o estado (ADR-008 §3.1 exige cada ruptura con su reemplazo).
- **Cierre del corpus:** este Bolt **habilita** el batch definitivo (Bolt 029) al cerrar t2; hasta entonces el
  corpus convertido es **provisional** y se re-migra desde los `.coop` intactos si aparece una divergencia
  (ADR-002 §3.2). El SPEC **no** ejecuta ese batch ni retira originales.
- **Rollback:** revertir el commit del V-Bounce. El esquema y el contrato viven en el paquete; no hay estado
  persistido propio, ni migración aplicada, ni dato transformado que deshacer. Los `.coop2` ya convertidos por
  el Bolt 002 **siguen siendo válidos** mientras el esquema no cambie: si este Bolt tuviera que corregir el
  esquema, la corrección **no** invalida los originales (intactos) pero **sí** obliga a re-migrar
  (`write(read(fixture))` deja de cerrar contra los fixtures viejos) y eso es una **revisión material del
  Bolt 002** (§15) — no una edición de fixtures desde acá.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **El inventario de campos del esquema queda incompleto** (un campo del modelo sin declarar, o un derivado declarado por error) y el formato pierde o ensucia el modelo | 3 | 5 | La regla de completitud de §A.1 (modelo **menos** la lista cerrada de exclusiones) + revisión explícita del inventario contra los tipos del Bolt 003, registrada en el MEM; el test bilateral sobre el corpus representativo es la red que expone una pérdida de fidelidad |
| **Divergencia con el migrador** en orden, escape o campos (contrato duplicado por construcción) | 4 | 5 | Misma versión **exacta** de `System.Text.Json` fijada centralmente + el **test bilateral byte a byte** sobre los **mismos fixtures** + escritor sin reflexión y con orden del esquema; un caso feliz no cuenta como cobertura |
| **Fabricar el cierre**: ajustar un fixture o "normalizar" la comparación para que el test pase | 2 | 5 | Los fixtures son del Bolt 002 y se consumen **de sólo lectura**; la comparación es **byte a byte sin normalización** del contrato; el `INDEX.md` de fixtures declara procedencia y el inventario se revisa contra los tipos, no contra la salida |
| **Derivado no recalculable** desde el dato persistido (pérdida de información al leer) | 3 | 4 | §C.2: el recálculo se implementa por campo y el caso se cierra **antes** de cerrar el esquema; si exige una capacidad nueva del modelo, para y se rutea a ADR (§15) |
| **Subida de versión silenciosa** (campos nuevos sin subir `schemaVersion`, o coerción de una versión superior) | 2 | 4 | AC-4: versión superior rechazada con error explícito; el esquema publicado es la referencia y un cambio sin subir versión es detectable por el inventario y por el gate de API |
| **El contrato no alcanza para editar de verdad** (enums o mapeo ausentes o parciales) | 3 | 4 | AC-6/AC-7 verifican los dos criterios con dueño de ADR-009 §3.2 como **aceptación**: el dominio de cada campo enumerado y el mapeo de tipos, accesibles en runtime; un dominio incompleto falla la suite |
| **`System.Text.Json` en versiones distintas** entre el migrador y el contrato | 2 | 5 | Versión fijada **una sola vez** en `Directory.Packages.props` (ADR-001 §3.4); condición de parada si falta o difiere (§15); el test bilateral detectaría el efecto |
| **El esquema se implementa "para el fixture"** en lugar de para el modelo (se codifica lo que el fixture muestra, no lo que el modelo tiene) | 3 | 4 | §A.1: el inventario se levanta de los tipos del modelo, no de los fixtures; el fixture es la **prueba**, no la fuente; el `README-contrato.md` publica el inventario para que el revisor lo contraste |
| **Frontera con el motor/shell invadida** (serialización paralela en el motor, o campos hardcodeados en el editor) | 2 | 4 | §6/E y ADR-002 §3.4: el contrato es el único lector/escritor; el motor y el shell **consumen**; el criterio con dueño del shell se verifica en su Bolt (ADR-009 §3.8) |
| **Se cuela una dependencia** (segundo serializador, un validador de JSON Schema de terceros) | 2 | 3 | AC-1 y el gate de dependencias: `System.Text.Json` es la única; la validación del esquema es propia (versión, obligatorios, dominios) y no agrega paquetes |
| **Capacidad nueva "de paso"** (distinguir tabla de vista, o un campo que el modelo no tiene) | 2 | 4 | ADR-002 §3.1 lo declara límite conocido; §4 lo excluye; un campo así exige ADR que suba `schemaVersion` → condición de parada |
| **Rupturas sin documentar** de la superficie del paquete | 2 | 3 | ADR-008 §3.1: la lista de rupturas entra en el MEM y alimenta las notas del Bolt 028; el gate de API compara contra la línea base |
| **Tentación de "cerrar" con una corrida parcial** (sólo el fixture feliz) | 2 | 4 | El Bolt lo declara como riesgo: la evidencia de t2 es sobre el **conjunto completo**, incluido el subconjunto representativo del corpus real, y el MEM nombra los fixtures corridos |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-002/ADR-001/ADR-009/ADR-008;
> acá se aplican. Las filas marcadas **(⚠ aprobador)** admiten más de una lectura del material gobernado y
> quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) El inventario de campos sigue la regla "modelo − lista de exclusiones de ADR-002 §3.1", no el bloque JSON de §3.1** | El ADR llama **ilustrativo** a su bloque y **normativas** a sus reglas ("el esquema normativo es el contrato JSON versionado que produce el Bolt de `Modeling` con estas reglas"); leerlo como inventario cerrado dejaría fuera campos almacenados del modelo —por ejemplo `generateAsType`, que es estado editable del modelo (`PropertyNode.GenerateAsType`, `DomainTree.cs:2167-2228`) y no está en la lista de exclusiones—. Si el aprobador prefiere el bloque como inventario cerrado, este SPEC se revisa: no se decide por silencio |
| **(⚠ aprobador) Los campos enumerados se enumeran en §6/A.3, con su origen en el código 1.4** | La lista **no existía** en ningún ADR; el propio `AITL-BOLT-READY-Approval` del Bolt adjudica enumerarla al SPEC. Fijarla acá (y no en la implementación) hace el criterio verificable y evita que el dominio dependa de lo que el ejecutor recuerde |
| **Un valor fuera de dominio se rechaza** (error tipado que nombra campo, valor y dominio) en lugar de degradarse | Coherente con la regla del formato en ADR-002 §3.1 ("nunca coerción silenciosa") y con el patrón de fallo cerrado del resto del ciclo (ADR-004 §3.3, ADR-006 §3.3). Un valor inválido con una versión aceptada sólo puede venir de un archivo corrupto o editado a mano: aceptarlo sería fabricar un modelo que nadie declaró |
| El esquema se publica como **artefacto versionado del paquete** y no sólo como código | El Bolt exige el esquema "publicado y versionado como contrato normativo"; publicarlo permite que un consumidor (y un revisor) lo lea sin leer el escritor, y es la referencia contra la que se detecta una subida de versión no declarada |
| El **digest** del documento se expone en el contrato, con la semántica del reporte del migrador | El cierre del corpus habla de digests (Bolt 002 §2 y Bolt 029): si cada lado definiera el digest a su manera, "digest coincidente" dejaría de significar lo mismo. La definición es una: SHA-256 de los bytes canónicos |
| La **normalización `CRLF`→`LF`** se aplica a la decisión de "no reescribir", **nunca** al contenido del archivo ni a la comparación bilateral | El contrato del archivo exige `LF` (ADR-002 §3.1); la normalización sólo evita que un checkout con `core.autocrlf` ensucie la idempotencia (misma regla que ADR-003 §3.3 para los artefactos). Aplicarla a la comparación bilateral sería exactamente "fabricar el cierre" |
| El test bilateral vive en un proyecto de pruebas **del contrato** (`2.0/tests/Cooperator.Framework.Modeling.Contract.Tests/`) y consume los fixtures del Bolt 002 | El cierre de la conformidad es de **este** Bolt (ADR-002 §3.2) y su insumo es la salida del Bolt 002: separar el proyecto de pruebas del contrato del paquete mantiene la propiedad clara entre dos Bolts y dos executors |
| Los casos negativos que no pueden venir de un fixture entregado se construyen **modificando una copia en un directorio temporal**, nunca el fixture del repositorio | Los fixtures son de otro Bolt y son el contrato de prueba: editarlos (incluso "temporalmente") es la forma más fácil de fabricar un cierre falso |
| El contrato **no** define una segunda tabla de mapeo ni un conversor propio | ADR-001 §3.1/§3.6 fija la fuente única en el Bolt 003 y ADR-003 §3.5 prohíbe al motor redefinirla; el contrato la **expone** para que el editor la consuma |
| `language` incluye `VB` en el dominio del formato | El dato existe en el modelo de origen y el dominio lo describe; **qué se hace** con un modelo VB lo gobiernan ADR-002 §3.3 (el migrador bloquea por defecto) y ADR-001 §3.5 (el motor es sólo C#). El contrato no relaja ni endurece esa política: expone el dominio y rechaza lo que esté fuera de él |
| La escritura "sólo si cambia" se ofrece en el contrato | Es el insumo del flujo agente y de la idempotencia del consumidor (ADR-003 §3.3); ofrecerla acá evita que cada consumidor reinvente la comparación y la normalización de finales de línea |

---

## 15. Stop conditions

- **El esquema necesita un campo que el migrador no emite** (o un orden, un nombre o un escape distintos de los
  que produce el Bolt 002): **parar y reportar**. Este Bolt **no** toca el migrador ni sus fixtures (G11): el
  camino es una **revisión de la SPEC del Bolt 002** por su executor, un V-Bounce nuevo de ese Bolt y la
  regeneración de los fixtures por **ese** Bolt. Ensanchar acá la salida o "acomodar" el fixture es fabricar la
  conformidad y está prohibido.
- **Falta el subconjunto representativo del corpus real** entre los fixtures del Bolt 002, o falta la SPEC/entrega
  del Bolt 002 en su conjunto: sin contrato de prueba no hay cierre bilateral posible (y un cierre sobre un caso
  feliz no es un cierre). Parar y reportar.
- **La versión de `System.Text.Json` no está fijada centralmente, o difiere de la que usa el migrador**: parar.
  Sin la misma versión exacta, la conformidad byte a byte no es alcanzable (ADR-002 §3.2) y no se resuelve
  fijando una versión "parecida".
- **Un derivado de la lista de exclusión no se puede recalcular** desde el dato persistido, o el recálculo
  cambiaría el comportamiento observable de los tipos del modelo: parar. La respuesta es completar el inventario
  del esquema con el campo almacenado que corresponda (§C.2); si eso exige una **capacidad nueva** del modelo
  (por ejemplo distinguir tabla de vista), es un **ADR** que suba `schemaVersion` (ADR-002 §3.1) y este Bolt no
  puede cerrarlo.
- **Aparece la tentación de subir `schemaVersion`** para acomodar un campo: parar. La v1 es la que se entrega; un
  cambio aditivo sube versión **y** obliga a re-migrar y a un upgrader explícito si es rupturista, con ADR que
  reemplace a ADR-002.
- **Un consumidor (motor, CLI, shell) necesita serializar por su cuenta** lo que el contrato no expone: parar.
  ADR-002 §3.4 hace de `Modeling` el único lector/escritor; la salida es una **revisión material de este SPEC**,
  nunca una segunda serialización.
- **El paquete `Modeling` del Bolt 003 no está** (ni el modelo portable ni la tabla de mapeo única): parar y
  reportar; este Bolt **no** crea el modelo ni la tabla.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-002 §3.1/§3.2/§3.4, ADR-001 §3.1/§3.4, ADR-009 §3.2, ADR-008 §3.2/§3.4)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-004`](../functional/bolts/US-000.BOLT-004-contrato-coop2-en-modeling.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-41 (mecanismo binario y ausencia de esquema/versión de formato), F-42 (orden canónico, derivados, política de versión), F-43 (`BinaryFormatter` y `[Serializable]`), §6 ítem 3.
- [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (formato, orden, exclusiones, `schemaVersion`, límite de las vistas), §3.2 (niveles t1/t2, fixtures dorados, test bilateral, versión exacta de serialización), §3.4 (`Modeling` único lector/escritor), §5 (NFRs de determinismo y modelo sin secretos).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (paquete `Modeling`, tabla de mapeo como dato portable con fuente única), §3.3 (naming/firma/versionado), §3.4 (`System.Text.Json` única dependencia de serialización, versiones centralizadas y licencias).
- [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — §3.1/§3.2 (editor dirigido por esquema; **criterios con dueño**: enums reales y tabla de mapeo accesible), §3.8 (cadena de doble mano entre el contrato y el shell).
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (rupturas documentadas con reemplazo), §3.2 (el grafo como POCOs; su persistencia es el `.coop2`), §3.4 (sin `BinaryFormatter` y sin `SYSLIB0051`).
- [`SPEC-261016-0002`](./SPEC-261016-0002-migrador-coop-a-coop2.md) — el migrador, t1 y los **fixtures dorados** que este Bolt consume (prerrequisito).
- [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) — el paquete `Modeling`, los POCOs y la tabla de mapeo única (prerrequisito).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y versiones centralizadas (prerrequisito transitivo).
- [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) · [`US-000.BOLT-017`](../functional/bolts/US-000.BOLT-017-motor-headless.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) · [`US-000.BOLT-029`](../functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md) — los Bolts que consumen o dependen de este cierre.
- [`OQ-004`](../analysis/open-questions/OQ-004-formato-del-coop-json.md) — extensión `.coop2` y migrador obligatorio sobre .NET Framework 4.8.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-004`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8 (incluidos los dos criterios con dueño de ADR-009 §3.2), la **enumeración de los campos enumerados** adjudicada al SPEC (§6/A.3), estrategia de pruebas con el test bilateral, gates, control de datos `internal` y condiciones de parada. `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (arquitectura / contrato del formato, riesgo `high`)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
