---
id: "SPEC-261016-0034"
title: "`Core`: resolución de tipos calificada en el mapeo filas→objetos, con diagnóstico accionable y sin resolución implícita"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 19 (hallazgo F-47) → ADR-006 §3.1 (regla de nombres calificados)
bolt: "US-000.BOLT-008" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 2 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-006", "ADR-001", "ADR-005"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y suite de caracterización (Bolt 001)
  - "SPEC-261016-0015-carga-de-providers-lazy.md" # la regla de nombres calificados y su resolutor: la coherencia que este Bolt hereda (Bolt 005)
risk_class: "medium" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L3" # default por riesgo low/medium (§3.3)
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "internal" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: not-required` → `n/a`
  applicability: "n/a"
  tests: []
  environment: ""
  data: ""
  reason: "Bolt `not-required` (debt): la resolución de tipos ocurre dentro del proceso sobre los datos que el mapeo ya recibió; no toca base de datos, cola ni servicio externo."
  waiver_adr: ""
review_ready_at: "2026-10-16T03:18:49-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T03:18:49-03:00"
  decided_at: "2026-10-16T03:18:49-03:00"
  findings:
    - "Compliant — re-aprobación de REVISIÓN 2 (material PREVENTIVA, G15/G16): misma SPEC canónica `SPEC-261016-0034`, `revision: 2`, `status` re-abierto a `draft` y re-aprobado acá. Fuentes gobernadas sin cambio material: Bolt `US-000.BOLT-008` (`ready`), ADR-006/ADR-001/ADR-005 `accepted`, REV-001 `approved`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Cursada **antes** de que la trampa muerda (sin V-Bounce aún): **sexta** instancia del mismo patrón."
    - "CONFIRMACIÓN (§6/B.4 — política de nulabilidad preventiva) — CORRECTA: espejo exacto de los **cuatro** precedentes `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3, citados con sus mediciones; mecanismo (`#nullable disable` primera línea útil, sólo en aportados **con diagnóstico**); criterio objetivo + **build como prueba**; límites duros (**jamás** código nuevo —la identificación, la excepción y las pruebas conservan la nulabilidad activa— y **prohibido** `.csproj`); límite del `using` muerto ⇒ parar (§15 #10)."
    - "CONFIRMACIÓN (§4 regla de aporte + alternativas + matiz) — CORRECTA: la regla de aporte declara que `BaseObjectList.cs`/`ObjectListHelper.cs` pueden **no estar** en `2.0/` y se aportan del 1.4 con el cierre mínimo; **anotar/corregir** es transformación no ordenada **y** un cambio de comportamiento en el camino por el que se llenan los objetos de negocio; **diferir** dejaría al Bolt sin los dos archivos a corregir."
    - "CONFIRMACIÓN (§7 AC-10/§8.1/§9/§13/§14/§15 #10/§16 AC-1…AC-10) — CORRECTA: AC-10 (directiva sólo en aportados + build + «ningún `.csproj`»); §8.1 verificación por lectura + build; §9 gate de Build + gate de nulabilidad; §13 riesgo; §14 fila ⚠; §15 **#10** (extensión de §B.4); §16 DoD con AC-1…AC-10."
    - "CONFIRMACIÓN (§17/§19 secuencia + §18) — CORRECTA: el precedente 011/013 queda citado como **camino de revisión de secuencia disponible** (sin cambio de alcance, §17 L688-694 y §19 nota 7); §18 con la fila de rev.1 **anotada** («esa aprobación queda invalidada por la revisión material siguiente (G15)») + la fila de rev.2, **sin reemplazo**."
    - "ADJUDICACIÓN — CONFIRMADA: la autorización preventiva ALCANZA —es el **sexto espejo** del mismo patrón ya endosado cuatro veces (SPEC-0004 §A.3, SPEC-0013 §A.3, SPEC-0037 §B.2, SPEC-0035 §E.3)— y **no habilita nada más**: el `using` muerto hacia namespace inexistente queda fuera (§15 #10), la directiva está prohibida en código nuevo y en `.csproj`, y la identificación inequívoca y las fronteras no cambian. Suficiente y agéntico."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). La regla de nombres (calificado, nunca implícito), la coherencia con el resolutor del
  Bolt 005 y el cuadro de diagnóstico están decididos en ADR-006 §3.1/§3.2/§3.3 y en ADR-001 §3.4. Este SPEC los
  aplica al camino de mapeo y los vuelve verificables, sin reabrir la decisión.
  ⚠️ FRONTERA DECLARADA (§4): la interpretación textual de los filtros de las vistas aparece en el mismo hallazgo
  (F-47) pero **no** entra en este Bolt: es una evaluación de expresiones, no una identificación de tipos. Queda
  registrada como observación con su dueño.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de
  dominio aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada
  invalida la aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0034 — `Core`: resolución de tipos calificada en el mapeo filas→objetos, con diagnóstico accionable y sin resolución implícita

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 19 (hallazgo F-47) |
| **Bolt** | [US-000.BOLT-008](../functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md) |
| **ADRs** | [ADR-006](../adrs/ADR-006-carga-de-ensamblados.md) §3.1, §3.2 · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.2, §3.4 · [ADR-005](../adrs/ADR-005-provider-de-datos.md) §3.3 |
| **Risk Class** | medium |
| **Revision** | 2 |

---

## 1. Objective

Se entrega el **mapeo de filas a objetos del framework** resuelto por **identificadores de tipo inequívocos**:
ningún tipo se identifica por un nombre simple que dependa del ensamblado llamador, y una identificación que no
resuelve **deja de ser un silencio** para convertirse en un error accionable en el punto que la pidió.

El problema que resuelve está verificado en el propio código de 1.4: el camino de mapeo **reconstruye el nombre de
un tipo a partir de una cadena de reflexión y lo vuelve a resolver por nombre simple** —
`System.Type.GetType(strPropType2)` en `BaseObjectList.cs:415`, donde `strPropType2` se extrae con un
`Substring(18, length - 19)` del `ToString()` de un `Nullable`—. `Type.GetType(string)` con un nombre simple
resuelve **sólo** contra el ensamblado llamador y el núcleo: para un tipo que vive en otro ensamblado devuelve
`null` y el código sigue con ese `null` (o con un `DataColumn` de tipo nulo), que es exactamente el
"fallo silencioso" y el "funciona por casualidad según el ensamblado llamador" que F-47 señala. Al mismo tiempo,
el punto gemelo que busca la propiedad de un objeto por nombre (`BaseObjectList.cs:342`) sí falla, pero con un
mensaje que no nombra ni el tipo ni el punto que lo pidió (`ArgumentException("<{0}> property not found.")`), y su
equivalente en el camino de llenado del `DataSet` (`ObjectListHelper.cs:69`) queda **tapado** por un
`InvalidDataSetException` genérico proveniente de un `catch (Exception)`.

Si este Bolt no se implementa, el camino por el que se llenan los objetos de negocio seguirá dependiendo de dónde
viva un tipo y de si el nombre se pudo adivinar, y un fallo se manifestará lejos de su causa.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 19 como BOLT→SPEC y F-02/hallazgo de nombres
al **ADR-006** (`accepted`), que fija la **regla de nombres calificados** para la carga de providers y su
consistencia explícita con F-47: "la corrección usa un nombre calificado o el mismo resolutor y **no** puede
reintroducir la resolución implícita que el ADR elimina" (ADR-006 §3.1), con la clave de caché como nombre
calificado (§3.2). Este Bolt **aplica esa misma regla** al camino de mapeo; no la reabre.

**Los puntos de resolución del camino filas→objetos (evidencia verificada en esta sesión):**

| Punto | Qué hace hoy | Consecuencia verificada |
|---|---|---|
| `BaseObjectList.cs:415` — **resolver un tipo a partir de un nombre** | `string strPropType2 = strPropType.Substring(18, (strPropType.Length - 19));` sobre el `ToString()` de un `Nullable`, seguido de `System.Type myType = System.Type.GetType(strPropType2);` y `new DataColumn(prop.Name, myType)` | Nombre **simple** ⇒ `GetType` devuelve `null` para un tipo que vive en otro ensamblado; el `DataColumn` se crea con ese `null` (o el `Substring` produce basura con otra forma de nombre) ⇒ comportamiento dependiente del llamador y del formato del `ToString()` |
| `BaseObjectList.cs:342` — **buscar la propiedad de un objeto** (comparador de orden) | `_field = type.GetProperty(propertyName); if (_field == null) throw new ArgumentException("<{0}> property not found.")` | Falla, pero **no** nombra el tipo ni el punto que pidió la propiedad; el diagnóstico no es accionable |
| `ObjectListHelper.cs:69` — el **gemelo** del punto anterior (llenado del `DataSet`) | `propertyArray[counter] = typeof(T).GetProperty(column.ColumnName);` dentro de un `try { … } catch (Exception) { throw new InvalidDataSetException("The Dataset schema do not match with RowSet"); }` | Un nombre de columna que no corresponde a ninguna propiedad produce un `InvalidDataSetException` **genérico**: se pierde cuál propiedad y cuál tipo fallaron |

> **Frontera declarada (excluida, con su dueño):** la **interpretación textual de los filtros de las vistas**
> —`BaseObjectListView.cs:68` (`_Filter.ToLower().Replace(" ", "").Contains("deleted=false")`) y su evaluación de
> expresiones— aparece en el mismo hallazgo F-47, pero **no** es una identificación de tipos: se registra como
> observación en el inventario y **no** se toca en este Bolt (ADR-006 §3.1 no lo alcanza).

**Fuera de alcance por dueño:** la carga de providers Lazy y su resolutor (`Core/Factory/Factories.cs:53`
`Assembly.Load(assemblyName)` y `:70` `Assembly.LoadFrom(...)`) son del **Bolt 005** (ADR-006 §3.1/§3.2): acá sólo
se **hereda su regla** y se verifica que no se reintroduzca una resolución implícita.

**Estado del árbol 2.0.** `2.0/` está siendo construido por el V-Bounce del Bolt 001 (arnés, comando reproducible
y suite de caracterización) y el paquete `Core` llega con los Bolts 006/007 y el resto de la partición. Este Bolt
escribe **sólo** su SPEC y, en su V-Bounce, los archivos del camino de mapeo en `2.0/src/Cooperator.Framework.Core/`.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-006 §3.1 es la regla que se aplica; ADR-001 §3.2 declara que la
resolución de tipos cambia de comportamiento entre runtimes —por eso la unicidad se **declara**, no se hereda— y
ADR-005 §3.3 fija que la **forma** del contrato que consume el código generado se preserva), y el plan de
integración declarado (`n/a`) coincide con el `test_integration: not-required` del Bolt (§3.2.2). No se emite
reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-008` (`devflow/functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:00:56-03:00; `risk_class: medium`, `test_integration: not-required`; su DoR confirma el recorte (el parseo del filtro de vistas queda excluido y declarado) |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-006` §3.1 (regla de nombres calificados y consistencia con F-47: **no** reintroducir la resolución implícita), §3.2 (clave por nombre calificado: el criterio de unicidad que este Bolt comparte) y §3.3 (diagnósticos) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-001` §3.2 (el piso de consumo y el cambio de comportamiento de la resolución de tipos entre runtimes) y §3.4 (la capa base sin dependencias de terceros) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T22:32:47-03:00 |
| ADR (gobernante) | `ADR-005` §3.3 (la **forma** del contrato que consume el código generado es lo que se preserva) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15 |
| REV evidence | `REV-001` §4 F-47 y §6 ítem 19 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPEC previo (prerrequisito directo) | `SPEC-261016-0015-…` — la estrategia de nombres calificados y su resolutor (Bolt 005) | **En disco**; dependencia declarada del Bolt |
| SPEC previo (arnés) | `SPEC-261016-0000-…` — comando reproducible, suite de caracterización y build | **En disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otro V-Bounce; **este** Bolt no lo toca) |

---

## 4. Scope

### In scope

1. **El inventario de los puntos de resolución de tipos del camino filas→objetos**, cada uno con su **resolución
   declarada** y su punto gemelo identificado (los dos puntos del Bolt más el equivalente del camino de llenado
   del `DataSet`).
2. **La resolución del tipo a partir de un nombre, sin ambigüedad**: donde el tipo **ya está disponible** como
   objeto (`prop.PropertyType`) la identificación no pasa por un nombre (se usa el tipo y, para un `Nullable`, su
   tipo subyacente **resuelto por API**, no por `Substring`); donde un nombre llega de afuera, la identificación
   exige un **nombre calificado** (con su ensamblado) y **falla** si no resuelve.
3. **El error accionable en la identificación**: un valor que no resuelve produce una excepción del framework que
   **nombra el valor intentado**, el **tipo** y el **punto que lo pidió** — nunca un silencio, nunca un tipo
   equivocado, nunca un `null` que explota más tarde.
4. **La coherencia con la regla del Bolt 005** (ADR-006 §3.1): la misma regla de nombres, sin reintroducir ninguna
   forma de resolución implícita en el camino de mapeo.
5. **La verificación sobre los valores reales** que alimentan esos puntos, sobre el **corpus de modelos de prueba**,
   con la **forma emitida declarada** cuando la produce otro componente (y su dueño, si su corrección es de otro
   Bolt).
6. **La caracterización antes/después** (suite del Bolt 001) para demostrar que sobre entradas válidas el
   comportamiento observable **no** cambia.
7. **La constancia de las rupturas** que introduce el diagnóstico nuevo (ADR-008 §3.1), como insumo de las notas
   de migración (Bolt 028).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| La **interpretación textual de los filtros de las vistas** (`BaseObjectListView`) | Bolt dueño de la evaluación de expresiones (observación registrada) | Es evaluación de expresiones, no identificación de tipos: el Bolt lo excluye expresamente y el DoR lo confirma como observación |
| La **carga, la caché y los diagnósticos de providers Lazy** (`Factory/Factories.cs`) | Bolt 005 (ADR-006) | Otro contrato, otra superficie y otro dueño; este Bolt sólo hereda la regla |
| El **provider de datos**, los *casts* y las firmas de la capa de datos | Bolt 014 (ADR-005) | Otro Bolt |
| El **contrato del modelo** y el código generado que consume el mapeo | Bolts 003/004 y 019 | ADR-005 §3.3: la **forma** del contrato se preserva, no se rediseña |
| **Agregar dependencias** a la capa base | — (nadie) | ADR-001 §3.4: `Core` con cero dependencias de terceros |
| La **retirada de superficie** de serialización de las excepciones de `Core` | Bolt 007 (SPEC-0013) | Otro Bolt; acá sólo se usa el tipo de excepción que ya exista para el diagnóstico |

**Regla de aporte (bounded, explícita).** Este Bolt modifica **el camino de mapeo** de `Core` (y su gemelo de
llenado) y **nada más**. Si la corrección exigiera cambiar la **forma** del contrato que consume el código generado
(ADR-005 §3.3) o agregar una dependencia, se **detiene** y se reporta: es una decisión que este Bolt no puede tomar.

**Regla de aporte sobre el árbol 2.0 y política de nulabilidad (revisión 2).** El camino de mapeo vive en dos
archivos del núcleo —`BaseObjectList.cs` y `ObjectListHelper.cs`— que en el árbol 2.0 **pueden no estar todavía**
(hoy `2.0/src/Cooperator.Framework.Core/` no tiene fuentes `.cs`: sólo su `.csproj` y su lock). Si al ejecutar el
V-Bounce no están, éste los **aporta del 1.4** en su forma ya corregida y aporta el **cierre mínimo** de tipos que
la compilación exija, **copiados tal cual** y declarados archivo por archivo en el MEM. Como el árbol 2.0 hereda de
`Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`, el código del 1.4
—anterior a las anotaciones de nulabilidad— **rompe el build** en cuanto produce `CS8618`/`CS8603`/`CS8625`: para
eso rige la **política de nulabilidad de §6/B.4**, que es la **única** mecánica de build autorizada sobre el código
aportado. Un `using` muerto **hacia un namespace que no existe en el árbol 2.0** y que **no** provenga de un retiro
propio **no** queda cubierto por esa autorización: se **detiene y se reporta** (§15).

---

## 5. Prerequisites and baseline

- **Bolt 005 y su SPEC** (`SPEC-261016-0015-…`): la **regla de nombres calificados**, su resolutor, la clave de
  caché como nombre calificado y los diagnósticos tipados. Este Bolt **aplica la misma regla** al mapeo y verifica
  que no se reintroduzca la resolución implícita (ADR-006 §3.1).
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible** (`2.0/build/build.ps1`), la **suite de
  caracterización** (que debe correr **antes y después** sobre el corpus de modelos) y el build de la solución.
- **Corpus de modelos de prueba (insumo de la verificación):** los fixtures del corpus —`.coop`/`.coop2`— que dejan
  los Bolts 002/004 (`SPEC-261016-0002-…`, `SPEC-261016-0010-…`) en `2.0/tests/fixtures/model-corpus/**`; la
  verificación de los valores reales que alimentan los puntos se hace **sobre ese corpus**, no sobre un ejemplo
  inventado.
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error.
- **Estado del árbol 2.0:** en construcción por el V-Bounce del Bolt 001. Este Bolt escribe su SPEC y, **en su**
  V-Bounce, los archivos del mapeo; **no** toca `2.0/` durante la preparación del SPEC.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline antes y después de construir.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-008`), **1–2 V-Bounces** esperados. El orden es del inventario al cierre: primero se
> inventarían los puntos, después se corrige la identificación, después el gemelo, después la coherencia con la
> regla y al final la verificación sobre los valores reales con la caracterización verde.

### Phase A — Inventario de los puntos de resolución del camino

**Duration:** ~1,5h de ciclo — **Complexity:** Low

Se produce el inventario de **todos** los puntos de resolución de tipos del camino filas→objetos, cada uno con:
ubicación, qué identifica (un **tipo** o una **propiedad**), de dónde viene el valor (el propio tipo, una cadena
de reflexión, un nombre emitido por otro componente), su resolución **declarada** y su diagnóstico actual. El
inventario incluye explícitamente: el punto que **resuelve un tipo a partir de un nombre**, el que **busca la
propiedad de un objeto** y su **gemelo** en el camino de llenado del `DataSet`; y registra, como **observación con
su dueño**, la interpretación textual de los filtros de las vistas (fuera de alcance).

**Files created:**
- `2.0/docs/core-type-resolution-inventory.md` — inventario de los puntos, su resolución declarada y su diagnóstico

---

### Phase B — La identificación del tipo, sin nombres simples

**Duration:** ~3h de ciclo — **Complexity:** High

#### B.1 Donde el tipo ya está disponible, no se pasa por un nombre

La identificación deja de reconstruir un nombre desde una cadena de reflexión: cuando el tipo está disponible como
objeto (el `PropertyType` de una propiedad), se usa **el tipo** y, si es un `Nullable`, su tipo subyacente
**resuelto por API** —no por `Substring` sobre su `ToString()`—. Con eso desaparece la dependencia del **formato**
del nombre y del ensamblado llamador: un `Nullable<MiEnum>` deja de resolver por casualidad y pasa a resolver por
construcción.

#### B.2 Donde el nombre llega de afuera, se exige calificado

Cuando el valor que alimenta el punto es una cadena (un nombre emitido por otro componente), la identificación
exige un **nombre calificado** con su ensamblado y falla si no resuelve. El motor de resolución es **el mismo
criterio** de unicidad que el Bolt 005 aplica a los providers (ADR-006 §3.1/§3.2): una sola forma de nombrar un
tipo en la 2.0.

#### B.3 El fallo, accionable

Una identificación que no resuelve produce una excepción del **framework** que nombra: el **valor intentado**, el
**tipo** involucrado (cuando se conoce) y el **punto que lo pidió**. Nunca `null` silencioso, nunca un tipo
equivocado, nunca una excepción genérica cuya causa esté en otro lugar. El texto del mensaje es parte del
diagnóstico: se documenta en el inventario y se verifica en los casos negativos.

**Files created:**
- `2.0/src/Cooperator.Framework.Core/Mapping/TypeIdentification.cs` — la identificación inequívoca y su resolución
- `2.0/src/Cooperator.Framework.Core/Exceptions/TypeResolutionException.cs` — el error accionable de la identificación (si el tipo del diagnóstico no existe ya; ver §14)
- `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/TypeIdentificationTests.cs` — resolución por tipo disponible y por nombre calificado

#### B.4 La mecánica de build del aporte del 1.4: política de nulabilidad (revisión 2)

El árbol 2.0 hereda de `2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`,
mientras que el código del 1.4 es **anterior** a las anotaciones de nulabilidad. Es la trampa **ya medida** en esta
ola —`MEM-261016-0107` §8.2 (28 diagnósticos `CS86xx` al portar el modelo), `MEM-261016-0209` §8.1 (`CS8618` en
`BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`) y `MEM-261016-0227` §13 (el aviso declarado del
Bolt 009)— y afectaría **a este Bolt** en cuanto aporte `BaseObjectList.cs` u `ObjectListHelper.cs` (o el cierre
mínimo que la compilación exija) con elementos sin inicializar, retornos posiblemente nulos o `null` literales: los
diagnósticos `CS8618`/`CS8603`/`CS8625` que el árbol convierte en **errores** y que dejan el gate de Build sin poder
cerrar en `pass`.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
`SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3):** cada archivo **aportado del 1.4** que **produzca diagnósticos
de nulabilidad** empieza con la directiva `#nullable disable` (primera línea útil, antes de los `using`), y **sólo**
esos archivos la llevan:

- **Sí la llevan:** los archivos **aportados del 1.4** por este Bolt —`BaseObjectList.cs`, `ObjectListHelper.cs` y el
  cierre mínimo que la compilación exija— **cuando produzcan** `CS8618`/`CS8603`/`CS8625`. El **criterio es
  objetivo** —*archivo aportado del 1.4 + diagnóstico de nulabilidad*— y la lista exacta la fija la **ejecución con
  el build**, no un nombre supuesto. La directiva **apaga el análisis**: no cambia una instrucción de IL y deja la
  semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —`Mapping/TypeIdentification.cs`, la excepción del diagnóstico
  (§14) y las pruebas—, que conserva la nulabilidad **activa** del árbol porque está escrito para el mundo anotado; y
  queda **prohibido** `<Nullable>disable</Nullable>` en **cualquier** `.csproj`, incluido el de `Core`: apagaría el
  análisis del proyecto entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta el build
  **falla** con el diagnóstico exacto (`CS8618`/`CS8603`/`CS8625`); si **sobra** en código nuevo, se ve en la lectura
  de los archivos entregados. Se verifica en §8.1, §9 y AC-10.

**Un `using` que quede sin uso** como consecuencia de una corrección **propia** de este Bolt se retira con ella: es
parte de la forma ya corregida del archivo. Un `using` muerto **hacia un namespace que no existe en el árbol 2.0**
que **no** provenga de una corrección propia **no** está cubierto por esta autorización: se **detiene y se reporta**
(§15).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`,
inicializadores o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que lo ordene —y en
este Bolt sería además un cambio de comportamiento en el camino por el que se llenan los objetos de negocio, sin ADR
que lo ordene—; **diferir el aporte** dejaría al Bolt sin los dos archivos que debe corregir. La directiva, en
cambio, es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no cambia el
comportamiento observable**: es el mismo criterio con el que se resolvió en los Bolts 003, 007, 009 y 022.

---

### Phase C — El punto gemelo y el diagnóstico de la propiedad

**Duration:** ~2h de ciclo — **Complexity:** Medium

Los dos puntos que buscan una **propiedad por nombre** (el comparador de orden y el llenado del `DataSet`) pasan a
compartir **una** verificación que, al no encontrar la propiedad, falla nombrando la **propiedad**, el **tipo** y el
**punto**. El `catch (Exception)` que hoy convierte cualquier fallo en un `InvalidDataSetException` genérico deja de
tragarse la causa: el diagnóstico específico se propaga (y el error genérico queda **sólo** para lo que
efectivamente es un desajuste de esquema). Se verifica que **no** quede un punto corregido y su gemelo sin
corregir: el inventario los enumera y cada uno declara su resolución.

**Files modified (en el V-Bounce de este Bolt):**
- `2.0/src/Cooperator.Framework.Core/BaseObjectList.cs` — resolución del tipo y búsqueda de la propiedad (aportado
  del 1.4 si el árbol 2.0 no lo tiene, con el cierre mínimo que la compilación exija y la política de nulabilidad
  de §B.4)
- `2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs` — el gemelo del llenado y su diagnóstico (ídem: aportado
  del 1.4 si no está, con la política de §B.4)

---

### Phase D — Coherencia con la regla de nombres y ausencia de resolución implícita

**Duration:** ~1,5h de ciclo — **Complexity:** Medium

Se verifica y se deja como control automatizado que el camino de mapeo **no** contiene ninguna forma de resolución
implícita: 0 usos de resolución por nombre simple, 0 reconstrucción de nombres por manipulación de cadenas de
reflexión, 0 cargas de ensamblado (esas viven en el Bolt 005 y en su resolutor). La regla de nombres aplicada es
**la misma** que la del Bolt 005 y se declara en el inventario: una sola forma de nombrar un tipo, la calificada.

**Files created:**
- `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/NoImplicitResolutionTests.cs` — control de resolución implícita y de coherencia con ADR-006 §3.1
- `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/PropertyLookupDiagnosticsTests.cs` — el diagnóstico de los dos puntos, incluido el gemelo

---

### Phase E — Verificación sobre los valores reales y caracterización antes/después

**Duration:** ~2,5h de ciclo — **Complexity:** Medium

#### E.1 Los valores reales

Se verifica, sobre el **corpus de modelos de prueba**, qué valores alimentan cada punto del inventario y con qué
forma llegan: los que el propio camino conoce (el `PropertyType`) y los que **emite otro componente** (un nombre en
el código generado o en el modelo). Cuando la forma la produzca otro componente, se **declara** la forma exigida y
su dueño si su corrección depende de él, en lugar de silenciarlo: una forma no calificada que llegue de afuera es
un hallazgo con dueño, no una tolerancia del resolutor.

#### E.2 La caracterización, antes y después

La suite de caracterización del arnés (Bolt 001) corre **antes y después** del cambio sobre el corpus: sobre
entradas **válidas**, el comportamiento observable del mapeo **no** cambia (mismos objetos, mismos valores, mismo
orden). Los casos negativos (tipo inexistente y tipo de otro ensamblado nombrado de forma simple) son los que
cambian de comportamiento —de silencio a error accionable— y su consecuencia para el consumidor queda en la
constancia de rupturas.

#### E.3 La constancia

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-008.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1: **qué** cambia (la identificación deja de admitir un nombre simple y un fallo deja de ser un
silencio), **por qué** (ADR-006 §3.1 sobre F-47: la unicidad se declara, no se hereda) y **con qué se reemplaza**
(un nombre calificado; un error tipado que nombra el valor y el punto). Un consumidor que hoy "funciona por
casualidad" pasa a fallar **temprano y con causa**: es el cambio buscado y está documentado.

**Files created:**
- `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/RealValuesOverCorpusTests.cs` — los valores reales del corpus y su forma declarada
- `2.0/docs/api-removals/US-000.BOLT-008.json` — constancia de rupturas (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-006/ADR-001/ADR-005, expresados como Given/When/Then verificables.

### AC-1: Inventario completo y resolution declarada

**Given** el camino filas→objetos
**When** se lee el inventario
**Then** están **todos** sus puntos de resolución de tipos, cada uno con su resolución declarada y su diagnóstico, y
el punto gemelo del llenado del `DataSet` figura explícitamente.

### AC-2: Cero resolución por nombre simple en el camino

**Given** el camino de mapeo construido
**When** corre el control de resolución implícita
**Then** hay **0** identificaciones por nombre simple, **0** reconstrucción de nombres por manipulación de cadenas
de reflexión y **0** cargas de ensamblado en el camino.

### AC-3: Un valor que no resuelve falla nombrando el valor y el punto

**Given** un tipo inexistente y un tipo que vive en otro ensamblado referenciado por su nombre simple
**When** el camino intenta identificarlos
**Then** falla con el error **tipado** del framework, nombrando el valor intentado y el punto que lo pidió, sin
`null` silencioso y sin tipo equivocado.

### AC-4: La identificación por tipo disponible no depende del nombre

**Given** una propiedad cuyo tipo es un `Nullable` de un enum o de un tipo de otro ensamblado
**When** el camino lo identifica
**Then** el tipo subyacente se resuelve por **API** (no por `Substring` del `ToString()`) y el resultado es el
correcto con independencia del ensamblado llamador y del formato del nombre.

### AC-5: El diagnóstico de la propiedad, accionable (y en su gemelo)

**Given** un nombre de propiedad o de columna que no corresponde a ninguna propiedad del tipo
**When** el camino lo busca (en cualquiera de los dos puntos)
**Then** falla nombrando la **propiedad**, el **tipo** y el **punto**, y **no** se convierte en un error genérico
que oculte la causa.

### AC-6: Coherencia con la regla de nombres del Bolt 005

**Given** el resolutor y la regla de nombres del Bolt 005
**When** se compara con la identificación de este Bolt
**Then** la forma de nombrar un tipo es **una sola** (calificada) y el inventario lo declara; no se reintrodujo
ninguna forma de resolución implícita (ADR-006 §3.1).

### AC-7: La caracterización no cambia sobre entradas válidas

**Given** el corpus de modelos de prueba
**When** corre la suite de caracterización antes y después del cambio
**Then** sobre entradas válidas no hay diferencias observables (mismos objetos, valores y orden), y los valores que
alimentan los puntos quedan **verificados** con su forma declarada y su dueño cuando corresponde.

### AC-8: Frontera respetada

**Given** la interpretación textual de los filtros de las vistas, la carga de providers y la capa de datos
**When** se revisa el cambio
**Then** ninguno de esos tres frentes fue tocado, el `Core` sigue con **0** dependencias de terceros y la **forma**
del contrato que consume el código generado no cambió (ADR-005 §3.3).

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde y
`git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` están vacíos.

### AC-10: La mecánica de build del aporte (nulabilidad del código aportado)

**Given** los archivos **aportados del 1.4** por este Bolt y el árbol 2.0 con `Nullable=enable` **y**
`TreatWarningsAsErrors=true`
**When** se compila el proyecto de `Core`
**Then** cada archivo **aportado del 1.4** que **produzca** `CS8618`/`CS8603`/`CS8625` empieza con
`#nullable disable` (primera línea útil) y **ningún** archivo de **código nuevo** la lleva; **ningún** `.csproj`
declara `<Nullable>disable</Nullable>`; y **ningún** `using` muerto hacia un namespace que no existe en el árbol 2.0
queda sin resolver ni sin la autorización de §B.4 (§15).

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "el camino de mapeo no contiene ninguna resolución de tipos por nombre simple" | Fases A y D: inventario + control de resolución implícita | AC-1, AC-2 · `NoImplicitResolutionTests` |
| "una identificación que no resuelve **no** produce un silencio…, sino un error que nombra el valor intentado y el punto que lo pidió" | Fase B.3: excepción tipada con valor, tipo y punto | AC-3 · casos negativos |
| "los valores reales que hoy alimentan esos puntos quedan verificados…" | Fase E.1: verificación sobre el corpus con la forma declarada | AC-7 · `RealValuesOverCorpusTests` |
| "la suite de caracterización del mapeo pasa sin cambios de comportamiento observables" | Fase E.2: antes y después sobre el corpus | AC-7 · suite del Bolt 001 |
| "la constancia de que la regla de nombres aplicada es la misma que rige la carga de providers y de que **no** se reintrodujo ninguna forma de resolución implícita" | Fase D: control de coherencia con ADR-006 §3.1 | AC-6 · `NoImplicitResolutionTests` |
| "la entrada propia de lo que este Bolt rompa en la superficie, registrada en las notas de migración" | Fase E.3: fragmento + MEM | AC-3, AC-9 · `api-removals/US-000.BOLT-008.json` |
| (Bolt §2, riesgos) el código del 1.4 que este Bolt aporta **no rompe el build por nulabilidad** (mecánica ya medida en los Bolts 003/007/009/022) | §6/B.4 (revisión 2): directiva `#nullable disable` **por archivo aportado del 1.4 + diagnóstico** | AC-10 · build de `Core` + lectura de los archivos entregados |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Identificación (`TypeIdentificationTests`):** tipo disponible como objeto (incluido `Nullable` de un enum y de
  un tipo de otro ensamblado) resuelto por API; nombre **calificado** resuelto; nombre simple rechazado con el error
  tipado.
- **Casos negativos (los dos del Bolt):** tipo **inexistente** y tipo que **vive en otro ensamblado** referenciado
  por su nombre simple → error tipado que nombra el valor y el punto (y, cuando se conoce, el tipo).
- **Diagnóstico de la propiedad (`PropertyLookupDiagnosticsTests`):** los **dos** puntos (comparador de orden y
  llenado del `DataSet`) fallan nombrando propiedad, tipo y punto; el `catch (Exception)` genérico ya no oculta la
  causa.
- **Control de resolución implícita (`NoImplicitResolutionTests`):** 0 nombre simple, 0 reconstrucción de nombres
  por `Substring`/`ToString()` de reflexión, 0 `Assembly.Load*` en el camino.
- **Valores reales (`RealValuesOverCorpusTests`):** sobre el corpus de modelos, cada punto alimentado con su valor
  real; la forma de los valores que emite otro componente, declarada con su dueño.
- **Caracterización antes/después:** la suite del arnés (Bolt 001) sobre el corpus, sin diferencias observables en
  entradas válidas.
- **Mecánica de build del aporte (nulabilidad, §B.4 / AC-10):** lectura de los archivos entregados —cada archivo
  **aportado del 1.4** que produzca diagnósticos de nulabilidad empieza con `#nullable disable` (primera línea útil)
  y **ningún** archivo de código **nuevo** la lleva—, ausencia de `<Nullable>disable</Nullable>` en **cualquier**
  `.csproj`, y el **build con `TreatWarningsAsErrors=true` como prueba real**: sin la directiva donde hace falta, el
  build falla con `CS8618`/`CS8603`/`CS8625`. Se verifica además que ningún `using` muerto hacia un namespace
  inexistente quede sin resolver ni sin autorización (si no proviene de una corrección propia: §15).
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** `Nullable` de enum; tipo genérico anidado; propiedad de sólo escritura o heredada; nombre con
  espacios o mayúsculas distintas (no debe resolver "por parecido"); columna del `DataSet` sin propiedad
  correspondiente; tipo del propio ensamblado llamador (el caso que hoy "funciona por casualidad"); lista vacía
  (el mapeo no debe fallar por no tener filas).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio (aunque el cambio se
  ejecute con evidencia negativa explícita: los casos negativos son la prueba de la corrección).

### 8.2 Integration test level

- **`n/a`** — el Bolt declara `test_integration: not-required` y el plan del frontmatter dice `n/a` con su razón:
  la resolución de tipos ocurre **dentro del proceso** sobre los datos que el mapeo ya recibió, sin base de datos,
  cola ni servicio externo. No hay tests de integración que escribir ni corrida pendiente antes de la aceptación
  (§3.2.2). La observación registrada por el Bolt (un fallo que sólo aparezca en el host real) queda declarada para
  la lane del Bolt dueño, no como corrida de este Bolt.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (§3.6).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El árbol compila con advertencias tratadas como error, **sin** `#nullable disable` fuera de los archivos aportados que la requieren | `pass` — esperado (§8.1) |
| Nulabilidad del código aportado (revisión 2) | V-Bounce | **0** `#nullable disable` en código nuevo; **0** `<Nullable>disable</Nullable>` en `.csproj`; la directiva presente **sólo** donde el build la exige (archivo aportado del 1.4 + diagnóstico) | `pass` — esperado (AC-10)|
| Unit / component tests | V-Bounce | Suite hermética verde (identificación, negativos, diagnóstico, control de resolución, valores reales) | `pass` — esperado (AC-1…AC-7) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del consumidor es el **mapeo observable** (mismos objetos y valores en entradas válidas) | `pass` — esperado (AC-7) |
| SAST | V-Bounce | El cambio afecta el camino de datos: se revisa la entrada de nombres (validación y fallo tipado, sin construcción dinámica de tipos) | `pass` — esperado (AC-2, AC-3) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas; `Core` sigue con cero terceros (ADR-001 §3.4) | `pass` — esperado (AC-8) |
| OWASP Top 10 — static review | V-Bounce | No hay superficie externa; la resolución dinámica de tipos se reduce (menos superficie de *spoofing* de tipos) | `n/a` — sin superficie alcanzable desde fuera del proceso |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | 0 secretos introducidos; los mensajes de diagnóstico no incluyen valores de datos, sólo nombres de tipo | `pass` — esperado |
| Hallucination lint | V-Bounce | Tipos, miembros y rutas referenciados existen (o se declaran como aportados) | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `data_classification: internal`; los diagnósticos nombran tipos y miembros, no datos de negocio | `pass` — esperado |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos; la identificación de tipos no carga ensamblados | `pass` — esperado (AC-2) |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; los casos negativos se escriben antes del cambio (evidencia negativa) | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas de la suite y de la caracterización | `pass` — esperado |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| Coherencia con la regla de nombres (ADR-006 §3.1) | V-Bounce | Una sola forma de nombrar un tipo (calificada); 0 resolución implícita en el camino | `pass` — esperado (AC-6) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | No se retira superficie pública: cambia el comportamiento del camino ante entradas inválidas | `n/a` — sin cambio de superficie (las rupturas de comportamiento van a las notas) |
| Integration tests (real dependencies) | Integration test | No aplica: `test_integration: not-required` (plan `n/a`) | `n/a` — verificación dentro del proceso (§3.2.2) |
| Contract / E2E against the real providers | Integration test | No hay provider externo ni entorno en el alcance | `n/a` — sin dependencias reales |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint ni carga crítica afectada | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | No hay servicio instrumentado; el diagnóstico es parte del resultado del mapeo | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: not-required` → `integration_test_plan.applicability: n/a`
> con su razón. Ningún gate del nivel de integración aplica.

---

## 10. Security and data

- **La corrección reduce superficie de comportamiento impredecible:** hoy el resultado del mapeo depende del
  **ensamblado llamador** y del **formato de un nombre de reflexión**; pasar a una identificación inequívoca y con
  fallo explícito elimina una clase de fallo silencioso (y con él, la posibilidad de construir un objeto con un
  tipo equivocado).
- **Sin datos en los diagnósticos:** los mensajes nombran **tipos, miembros y el punto que pidió** la
  identificación; **no** incluyen valores de negocio ni contenido de filas.
- **Sin resolución de tipos desde datos no confiables:** el nombre que llega de afuera, si llega, debe ser
  **calificado** y su fallo es tipado; el camino no carga ensamblados (esa capacidad vive en el Bolt 005 con su
  propio contrato y sus propios controles).
- **Clasificación:** `data_classification: internal` (espeja el Bolt). El corpus de modelos describe **estructura**,
  no datos de negocio; los fixtures que use la verificación son sintéticos.
- **Integridad del árbol congelado:** `1.4/**` no se modifica; su código es la evidencia del hallazgo, no un insumo
  que se edite.

---

## 11. Monitoring and observability

- **El diagnóstico es la observabilidad del camino:** un fallo de identificación dice **qué** valor falló, **qué**
  tipo estaba en juego y **quién** lo pidió; eso es lo que convierte un incidente en una corrección.
- **Evidencia versionada:** el inventario (`2.0/docs/core-type-resolution-inventory.md`), la constancia de rupturas
  (`2.0/docs/api-removals/US-000.BOLT-008.json`) y las salidas de la suite y de la caracterización en
  `2.0/artifacts/<timestamp>/`.
- **Sin telemetría de servicio:** no hay logs, métricas ni trazas que agregar (`n/a` con esa razón en la tabla de
  gates).

---

## 12. Migration, compatibility and rollback

- **Migration:** `n/a` en datos y esquema. El cambio es de **comportamiento ante entradas inválidas**: un consumidor
  que hoy resolvía un tipo "por casualidad" (porque el tipo vivía en su propio ensamblado) pasa a tener que
  nombrarlo **calificado**, o recibe un error que se lo dice. La migración es una línea en el nombre, o ninguna.
- **Compatibility:** **sin ruptura de superficie pública** (no desaparece ningún miembro ni firma: la **forma** del
  contrato que consume el código generado se preserva, ADR-005 §3.3) y **sin cambio observable en entradas
  válidas** (caracterización antes/después). Lo que cambia es el **diagnóstico** de un fallo y la exigencia de
  unicidad en el nombre: ambas van a las notas de migración (Bolt 028) con su reemplazo.
- **Rollback:** revertir el commit del Bolt. Es un cambio acotado a dos puntos (y su gemelo); un rollback parcial
  —corregir un punto y dejar su gemelo— es inválido y el inventario lo detecta.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Es el camino por el que se llenan los objetos de negocio**: una regresión afecta toda carga de objetos | 3 | 4 | Suite de caracterización **antes y después** sobre el corpus (sin diferencias observables en entradas válidas) + el inventario con cada punto resuelto (AC-1, AC-7) |
| **Un error nuevo que rompa a quien hoy "funciona por casualidad"** | 3 | 3 | Es el cambio buscado (ADR-006 §3.1): el fallo pasa a ser **temprano y con causa**, y su consecuencia queda en las notas con su reemplazo (AC-3) |
| **Arreglar un punto y dejar su gemelo** | 3 | 3 | El inventario enumera **todos** los puntos del camino (incluido el del llenado del `DataSet`) y cada uno declara su resolución (AC-1, AC-5) |
| **Valores que otro componente siga emitiendo como nombre simple** | 2 | 3 | La forma emitida se verifica sobre el corpus y, si su corrección es de otro componente, queda **declarada con su dueño** en el inventario (AC-7) |
| **Reintroducir resolución implícita** al "simplificar" el diagnóstico | 2 | 4 | Control automatizado: 0 nombre simple, 0 reconstrucción de nombres, 0 cargas de ensamblado en el camino (AC-2) |
| Un fallo que sólo aparezca en el host real (no en el arnés) | 2 | 3 | Observación registrada por el Bolt: su verificación queda en la lane del Bolt dueño, **no** se da por buena acá (§8.2) |
| El cambio exige modificar la **forma** del contrato o agregar una dependencia | 1 | 4 | Detener y escalar (§15): son decisiones de otro Bolt/ADR |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8603`/`CS8625` | 3 | 3 | §B.4 la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-10 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| Donde el tipo **ya está disponible** como objeto, no se pasa por un nombre | El `PropertyType` **es** el tipo: reconstruir un nombre y volver a resolverlo es la causa raíz de F-47 (depende del ensamblado llamador y del formato del `ToString()`). Resolver el `Nullable` por API elimina la fragilidad del `Substring` |
| La resolución por nombre exige un **nombre calificado** y es la **misma** regla del Bolt 005 | ADR-006 §3.1: una sola forma de nombrar un tipo en la 2.0; dos reglas distintas volverían a hacer que el resultado dependa de dónde vive el tipo |
| El error de identificación es **tipado** y nombra valor, tipo y punto | Es el criterio del Bolt ("error accionable… en lugar de un silencio"); una excepción genérica no es accionable aunque exista |
| El `catch (Exception)` del llenado deja de tragarse la causa del diagnóstico | Hoy convierte cualquier fallo en un mensaje de desajuste de esquema; eso pierde el dato que hace falta para corregir. El error genérico queda sólo para lo que **realmente** es un desajuste |
| El tipo del diagnóstico se reutiliza si ya existe en `Core` | Evita duplicar jerarquía de excepciones; su **retiro de superficie de serialización** es del Bolt 007 (SPEC-0013), no de éste |
| La interpretación textual de los filtros de las vistas **no** entra | El Bolt y su DoR lo excluyen: es evaluación de expresiones, no identificación de tipos; mezclarlas dejaría dos resultados en un Bolt |
| Las rupturas de **comportamiento** se documentan en el fragmento de notas (no en el gate de API) | No hay cambio de superficie pública: el gate de compatibilidad no las vería. El vehículo correcto es la nota con sus tres datos (ADR-008 §3.1) |
| **(⚠ aprobador) Política de nulabilidad del código aportado del 1.4: `#nullable disable` por archivo que lo requiera** (revisión 2) | Es la trampa **ya medida** en esta ola (Bolts 003/007/009/022: 28 diagnósticos `CS86xx` al portar el modelo, `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`, y el aviso declarado del Bolt 009 — `MEM-261016-0107` §8.2, `MEM-261016-0209` §8.1, `MEM-261016-0227` §13—) y **este Bolt aporta código del 1.4**. Es el **espejo exacto** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3: preserva la semántica del 1.4 (apaga el análisis, **no cambia el IL**), es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no cambia el comportamiento observable**. Se **descartan**, declaradas, las alternativas de **anotar/corregir** el código aportado (transformación no ordenada, y aquí un cambio de comportamiento en el camino por el que se llenan los objetos de negocio) y de **diferir** el aporte (dejaría al Bolt sin los dos archivos que debe corregir). Se elige la opción que **evita una revisión material por un motivo ya conocido** |

---

## 15. Stop conditions

1. **Cambio material en una fuente gobernada** (Bolt, ADR-006, ADR-001, ADR-005, el REV usado, baseline de código):
   detener, revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca dos revisiones (G16).
2. **La corrección exigiría cambiar la forma del contrato** que consume el código generado (ADR-005 §3.3) o el
   contrato del modelo (ADR-002): detener y escalar; son decisiones de otros Bolts.
3. **La corrección exigiría agregar una dependencia** a la capa base: detener (ADR-001 §3.4).
4. **Un valor real que alimenta un punto llega con una forma no calificada y su corrección pertenece a otro
   componente**: se **declara con su dueño** en el inventario y se detiene ese punto si no puede resolverse sin
   tolerancia; el resolutor **no** reintroduce la resolución implícita para "seguir andando" (ADR-006 §3.1).
5. **La caracterización muestra un cambio observable en entradas válidas**: detener y escalar — el cambio buscado es
   de diagnóstico, no de comportamiento válido.
6. **Aparece un punto de resolución de tipos fuera del inventario** (incluido un gemelo en otro camino de la capa
   base): parar y ampliar el inventario **antes** de corregir; corregir a medias es el riesgo declarado por el Bolt.
7. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
8. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite de caracterización o corpus): detener y
   escalar la secuencia.
9. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, la forma exacta de un nombre que emite un
   componente que todavía no existe): parar y reportar; no se rellena el hueco con una suposición (§2.4.1).
10. **Alguien extiende la autorización de §B.4**: poner la directiva `#nullable disable` en **código nuevo** o
    `<Nullable>disable</Nullable>` en un `.csproj`, o usar esa misma autorización para resolver un `using` muerto
    **hacia un namespace que no existe en el árbol 2.0** que **no** provenga de una corrección propia: detener y
    reportar. La autorización es una **mecánica de build** acotada a los archivos aportados del 1.4, no una licencia
    para transformar código — y **evita** una cuarta revisión material por el mismo motivo.

---

## 16. Definition of Done (DoD)

- [ ] Fases A–E implementadas (o, si el presupuesto se agota, progreso en el MEM y pendiente explícito para un
      V-Bounce nuevo)
- [ ] Todos los criterios de aceptación (AC-1…AC-10) verificados, incluidos los dos casos negativos del Bolt
- [ ] Suite hermética verde (§8.1) y caracterización **antes/después** sin diferencias observables en entradas
      válidas; el plan de integración es `n/a` con su razón
- [ ] El inventario de los puntos de resolución está publicado y versionado, con la resolución de cada punto y el
      gemelo incluido
- [ ] El código sigue ADR-006 (§3.1/§3.2), ADR-001 (§3.2/§3.4) y ADR-005 (§3.3)
- [ ] La directiva de nulabilidad de **§B.4** aplicada **sólo** a los archivos **aportados del 1.4** que la
      requieren, y verificada por lectura **y** por el build (AC-10); ningún `.csproj` con `<Nullable>disable</Nullable>`
      y ningún `using` muerto hacia un namespace inexistente sin resolver ni autorizado
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la lista de rupturas
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-008`](../functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md) y su manifiesto
  [`US-000.BOLT-008-…json`](../metrics/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-006`](../adrs/ADR-006-carga-de-ensamblados.md) §3.1/§3.2/§3.3 ·
  [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.2/§3.4 · [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) §3.3
- SPEC prerrequisito directo: [`SPEC-261016-0015`](./SPEC-261016-0015-carga-de-providers-lazy.md) (regla de nombres calificados y resolutor)
- SPECs previos: [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y caracterización) ·
  [`SPEC-261016-0002`](./SPEC-261016-0002-migrador-coop-a-coop2.md) y [`SPEC-261016-0010`](./SPEC-261016-0010-contrato-coop2-en-modeling.md) (corpus de modelos de prueba)
- SPECs relacionados: [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) (retiro de la superficie de serialización de las excepciones — dueño del tipo de excepción) ·
  [`SPEC-261016-0029`](./SPEC-261016-0029-plantillas-csharp.md) (el código generado: emisor de nombres a verificar si aplica)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-47 y §6 ítem 19
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** ·
  [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** · [`SPEC-261016-0037`](./SPEC-261016-0037-library-portable.md) **§B.2** ·
  [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) **§E.3** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado
  por revisión material **con su medición** (`MEM-261016-0107` §8.2: 28 diagnósticos `CS86xx`; `MEM-261016-0209` §8.1:
  `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`; `MEM-261016-0227` §13: el aviso declarado
  del Bolt 009). Es el mecanismo que esta revisión **espeja** para el aporte de la fase B
- **Camino de revisión de secuencia (recordatorio, sin cambio de alcance):** si el ejecutor encontrara que el camino
  de mapeo depende de la capa `Data` —hoy tipada sobre el proveedor clásico—, el precedente de los Bolts **011/013**
  está declarado: [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) §4/§15 (el hueco de **secuencia** y su
  salida elegida —dividir el alcance, **no** absorber la capa `Data`, cuya versión de provider es criterio del Bolt 014,
  ADR-005 §3.1—) y [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) §B.4/§15 (la arista de proyecto de **una línea**
  cuando la base del cierre la exige, y la parada por «el cierre mínimo exigiría transformar código de otro Bolt»).
  Se registra como **camino disponible**, no como una dependencia de este SPEC
- Evidencia de código 1.4 (verificada en esta sesión): `Core/BaseObjectList.cs:409-418` (`prop.PropertyType.ToString()`,
  `Substring(18, length - 19)` y `System.Type.GetType(strPropType2)` con `new DataColumn(prop.Name, myType)`),
  `Core/BaseObjectList.cs:342-346` (`type.GetProperty(propertyName)` y `ArgumentException("<{0}> property not found.")`),
  `Core/ObjectListHelper.cs:64-98` (`typeof(T).GetProperty(column.ColumnName)` dentro de un `catch (Exception)` que
  lanza `InvalidDataSetException("The Dataset schema do not match with RowSet")`),
  `Core/Utility/…` (n/a) y `Core/Factory/Factories.cs:53,70` (`Assembly.Load`/`LoadFrom`: **del Bolt 005**),
  `Core/BaseObjectListView.cs:68` (la interpretación textual del filtro: **fuera de alcance**, declarada)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-008`: inventario de los puntos de resolución de tipos del camino filas→objetos, identificación inequívoca (tipo disponible o nombre calificado, nunca simple), diagnóstico accionable en los dos puntos y su gemelo, coherencia con la regla de nombres del Bolt 005 y verificación sobre los valores reales del corpus con caracterización antes/después. **Aprobada** por `agent:dotnet-developer-reviewer-agent` (dev_validator) el `2026-10-16T00:39:45-03:00` (`AITL-SPEC-Approval`); esa aprobación queda **invalidada** por la revisión material siguiente (G15) |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16) PREVENTIVA**, cursada **antes** de que la trampa muerda y sin que exista todavía un V-Bounce de este Bolt: los V-Bounces que aportaron código legacy en esta ola (003/007/009 y 022) chocaron con la **nulabilidad** del árbol (`Nullable=enable` + `TreatWarningsAsErrors`, del Bolt 001) y cada uno necesitó su **revisión material** para autorizar la directiva, y **este Bolt aporta código del 1.4** (`BaseObjectList.cs`, `ObjectListHelper.cs` y el cierre mínimo que la compilación exija). Cambia **una sola cosa**: la **política de nulabilidad del código aportado del 1.4** (§6/B.4, nueva) —`#nullable disable` como primera línea útil **sólo** en los archivos aportados que **produzcan** `CS8618`/`CS8603`/`CS8625`, **jamás** en código nuevo (la identificación, la excepción del diagnóstico y las pruebas conservan la nulabilidad activa), **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**—, con el **límite** del `using` muerto hacia un namespace inexistente que no provenga de una corrección propia (parada y reporte, §15 #10). Es el espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3. Se actualizan §1, §4 (regla de aporte + política), §6/B.4 (nueva) y §6/C, §7 (AC-10 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el inventario de los puntos, la identificación inequívoca, el diagnóstico accionable, la frontera del filtro de vistas, la caracterización antes/después y el plan de integración (`n/a`) siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor |

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
| **review.reviewers** | [Dev-validator + dueño de arquitectura (regla de nombres y frontera con el Bolt 005) + dueño de la capa base si se evalúa el diagnóstico] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

**Qué cambia en la revisión 2 (y qué no).** Cambia **una sola cosa, y es preventiva**: la **política de nulabilidad
del código aportado del 1.4** (§6/B.4) — `#nullable disable` como primera línea útil **sólo** de los archivos
**aportados del 1.4** que **produzcan** `CS8618`/`CS8603`/`CS8625`, **jamás** en código nuevo, **prohibido** en
`.csproj`, con criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**—, porque este Bolt
**aporta** `BaseObjectList.cs`, `ObjectListHelper.cs` y su cierre mínimo, y el árbol hereda `Nullable=enable` +
`TreatWarningsAsErrors` del Bolt 001. Es el **espejo** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
`SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3, y evita una revisión material por un motivo **ya medido** tres
veces en esta ola. **No** cambia: el inventario de los puntos, la identificación inequívoca (tipo disponible o
nombre calificado), el diagnóstico accionable, la **frontera del filtro de vistas** (fuera), la caracterización
antes/después, las demás fronteras y el plan de integración (`n/a`). La revisión 1 fue aprobada el
`2026-10-16T00:39:45-03:00`; esa aprobación queda **invalidada** por este cambio material (G15) y hace falta un
**`AITL-SPEC-Approval` nuevo** antes de ejecutar el V-Bounce (G15/G16).

### Reviewer notes (puntos explícitos para la decisión)

1. **Los tres puntos, no dos:** el inventario incluye el punto que resuelve un tipo por nombre, el que busca la
   propiedad de un objeto **y su gemelo** en el llenado del `DataSet` (que hoy oculta la causa tras un
   `InvalidDataSetException` genérico). Confirmar que el gemelo entra (el Bolt pide no "arreglar un punto y dejar su
   gemelo").
2. **La interpretación textual de los filtros de las vistas queda fuera** (declarada como observación, con su
   dueño): es evaluación de expresiones, no identificación de tipos, y el DoR del Bolt lo confirma.
3. **Tipo del diagnóstico:** se reutiliza una excepción existente de `Core` si la hay; su **retiro de superficie de
   serialización** pertenece al Bolt 007. Confirmar que crear un tipo nuevo (si no existe) es aceptable, o indicar
   el tipo a usar.
4. **Rupturas de comportamiento en las notas** (no en el gate de API): no hay cambio de superficie, así que el
   fragmento `api-removals/US-000.BOLT-008.json` es el vehículo. Confirmar.
5. **Corpus de modelos:** la verificación de los valores reales se hace sobre los fixtures de los Bolts 002/004
   (`2.0/tests/fixtures/model-corpus/**`), no sobre un ejemplo inventado.
6. **Política de nulabilidad del código aportado (§6/B.4, nueva en la revisión 2).** `#nullable disable` como
   **primera línea útil** **sólo** de los archivos **aportados del 1.4** que produzcan `CS8618`/`CS8603`/`CS8625`
   —criterio objetivo, la lista la fija el build—, **jamás** en código nuevo y **prohibido**
   `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es el **espejo** de `SPEC-261016-0004` §A.3,
   `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3, y es **preventiva**: evita repetir una
   revisión material por un motivo ya medido. La autorización **no** cubre un `using` muerto hacia un namespace
   inexistente que no provenga de una corrección propia (§15 #10).
7. **Camino de revisión de secuencia (recordatorio, sin cambio de alcance).** Si apareciera una dependencia del
   camino de mapeo con la capa `Data` —hoy tipada sobre el proveedor clásico—, el precedente de los Bolts **011/013**
   está declarado en §17 (`SPEC-261016-0018` §4/§15: el hueco de secuencia y su salida elegida; `SPEC-261016-0013`
   §B.4/§15: la arista de **una línea** cuando la base del cierre la exige, y la parada por transformar código ajeno).
   Se deja registrado como camino disponible, no como una dependencia de este SPEC.
8. **Secuencia (G16):** esta revisión **no** se ejecuta hasta que esté registrado el `AITL-SPEC-Approval` de la
   revisión 2; el V-Bounce arranca después, con su MEM propio.
