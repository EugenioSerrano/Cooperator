---
id: "SPEC-261016-0019"
title: "`Data`: caché tras `ICacheProvider` (default en memoria acotado y con vencimiento) y retiro del caché del host"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-013" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 2 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-004", "ADR-001"]
prerequisites: ["SPEC-261016-0000-arnes-tests-y-build-reproducible.md", "SPEC-261016-0018-configuracion-portable.md"] # arnés, build compartido y suite de caracterización (Bolt 001) · contrato de configuración y catálogo de claves: TTL por región y cap (Bolt 011) — ver §5
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
  reason: "Bolt `test_integration: not-required` (refactor): la caché vive dentro del proceso con su comportamiento por defecto en memoria; el resultado no toca base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T03:17:26-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T03:17:26-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T03:17:26-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — re-aprobación de REVISIÓN 2 (material PREVENTIVA, G15/G16): misma SPEC canónica `SPEC-261016-0019`, `revision: 2`, `status` re-abierto a `draft` y re-aprobado acá. Fuentes gobernadas sin cambio material: Bolt `US-000.BOLT-013` (`ready`), ADR-004/ADR-001 `accepted`, REV-001 `approved`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Cursada **antes** de que la trampa muerda (sin V-Bounce aún): **quinta** instancia del mismo patrón."
    - "CONFIRMACIÓN (§6/B.2 — política de nulabilidad preventiva) — CORRECTA: espejo exacto de los **cuatro** precedentes `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3, citados con sus mediciones; mecanismo (`#nullable disable` primera línea útil, sólo en aportados **con diagnóstico**); criterio objetivo + **build como prueba**; límites duros (**jamás** código nuevo —el contrato `ICacheProvider` conserva la nulabilidad activa— y **prohibido** `.csproj`); límite del `using` muerto ⇒ parar (§15)."
    - "CONFIRMACIÓN (alternativas descartadas + matiz Bolt 014) — CORRECTA: **anotar/corregir** es transformación no ordenada **y** tocaría, de paso, archivos cuyo aspecto pertenece al **Bolt 014** (`SQLHelper.cs`/`SqlHelperParameterCache.cs`); **diferir** dejaría al Bolt sin la capa donde sustituir los puntos de caché. El matiz de frontera de aspecto es el correcto."
    - "CONFIRMACIÓN (nota de secuencia §17/§19 + §13/§15) — CORRECTA y **sin cambio de alcance**: el precedente del 011 (`MEM-261016-0053`, `SPEC-261016-0018` §4/§6/C.2) queda citado como **condición de parada** (dependencia de secuencia al aportar ⇒ parar y escalar; nunca invadir el Bolt ajeno), **sin especular archivos concretos** (§15 L794-798, §13 L739, §17 L830, §19 L867-872)."
    - "CONFIRMACIÓN (§4/§7-AC-9/§8.1/§9/§13/§14/§16/§18/§19) — CORRECTA: in-scope; AC-9; §8.1 verificación por lectura + build + `using` muerto; §9 gate de Build; §13 riesgo (nulabilidad + secuencia); §14 fila ⚠; §16 DoD; §18 con las **tres filas** (rev.1 + aclaración en draft + rev.2, **sin reemplazo**); §19 nota al aprobador (4 puntos)."
    - "ADJUDICACIÓN — CONFIRMADA: la autorización preventiva ALCANZA —es el **quinto espejo** del mismo patrón ya endosado cuatro veces (SPEC-0004 §A.3, SPEC-0013 §A.3, SPEC-0037 §B.2, SPEC-0035 §E.3)— y **no habilita nada más**: el `using` muerto hacia namespace inexistente queda fuera (§15), la directiva está prohibida en código nuevo y en `.csproj`, y el contrato de caché y las fronteras no cambian. Suficiente y agéntico."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ REVISIÓN 2 — REVISIÓN MATERIAL (G15/G16) PREVENTIVA, cursada **antes** de que la trampa muerda y sin que exista
     todavía un V-Bounce de este Bolt. **Este Bolt aporta/transforma archivos del 1.4** (`BaseGateway.cs`,
     `BaseLoader.cs`, `SQLHelper.cs`, `SqlHelperParameterHelper`/`SqlHelperParameterCache.cs` y lo que el aporte
     traiga) y el árbol 2.0 hereda del Bolt 001 `Nullable=enable` + `TreatWarningsAsErrors=true`: es el escenario
     que ya detuvo a los Bolts **003, 007 y 009** y que el **022** previno por la misma vía, obligando a una
     **revisión material por cada caso** para autorizar la directiva. Esta revisión **adelanta esa autorización**
     —`#nullable disable` como primera línea útil de **cada archivo aportado del 1.4** que **produzca diagnósticos**
     (`CS8618`/`CS8603`/`CS8625`), **jamás** en código nuevo (la implementación de caché nueva conserva la
     nulabilidad activa), **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y
     **el build como prueba**—, espejo exacto de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
     `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3. Ajusta §1, §4, §6 (nota de mecánica + §B.2), §7 (AC-9 y su
     mapeo), §8.1, §9, §13, §14 (fila ⚠), §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** el contrato `ICacheProvider`, la implementación en memoria, los tres puntos de caché,
     el retiro del caché del host, los TTL/cap desde el catálogo, las fronteras con los Bolts 011/014/005 y el
     control `internal` siguen **en vigor** (el `review` se re-abre vacío: es una revisión material).
     ⚠️ **Nota de secuencia (precedente citado, sin cambio de alcance — §17/§19):** la frontera conocida con la capa
     `Data` tipada sobre el proveedor clásico —el caso que el **Bolt 011** tuvo que resolver con una revisión de
     secuencia (`MEM-261016-0053`, `SPEC-261016-0018` §4/§6/C.2)— puede repetirse al ejecutar este Bolt: si al
     aportar se descubre que un archivo depende de una pieza que **otro** Bolt debe portar primero, el camino es
     **parar y escalar la secuencia** y, si corresponde, **revisar este SPEC** — **nunca** invadir el Bolt ajeno.
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-013`. El contrato de
     configuración y su catálogo de claves son del Bolt 011 (**consumidos**, no definidos); el proveedor de datos,
     los casts y el tipado del interior son del Bolt 014 (**frontera declarada** en §4/§14 sobre los dos
     archivos compartidos); el caché de providers Lazy es del Bolt 005 (otro contrato, otro dueño).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ SIN DEFAULTS ESCONDIDOS: el Bolt prohíbe expresamente el default en el código. El TTL de cada región y el
     límite de entradas salen del catálogo de ADR-004 §3.2; si el catálogo no publica una clave para un default,
     la salida es pedirla al Bolt 011 (§15), nunca esconderla acá.
-->

# SPEC-261016-0019 — `Data`: caché tras `ICacheProvider` (default en memoria acotado y con vencimiento) y retiro del caché del host

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 4) |
| **Bolt** | [US-000.BOLT-013](../functional/bolts/US-000.BOLT-013-cache-en-data.md) |
| **ADRs** | [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | medium |
| **Revision** | 2 |

---

## 1. Objective

Se entrega el **caché de la capa de datos detrás de un contrato propio**: `ICacheProvider` en
`Cooperator.Framework.Data`, con una **implementación por defecto en memoria** —sin dependencias de terceros,
thread-safe, con **TTL por entrada**, **expiración perezosa**, **límite de entradas con desalojo** y los
**defaults por región** que ADR-004 §3.1 fija— y con la **sustitución de todos los puntos de caché de la capa**
por ese contrato: **colecciones**, **metadata de parámetros de stored procedure** y **timeout de comandos**. Con
eso **desaparece la dependencia del caché del host** —y la plataforma web— de la capa de datos: el framework
deja de tener **dos caminos según el host** y pasa a tener **un comportamiento único, acotado y predecible**.

El problema que resuelve está documentado (REV-001 F-07/F-08 y §6 ítem 4, con la corrección factual de la
revisión: el caché de escritorio **existe**, es en memoria **por instancia** y **sin vencimiento**): en 1.4 el
caché de colecciones vive en `HttpContext.Current.Cache` cuando hay contexto web (`BaseGateway.cs:323-463`,
`BaseLoader.cs:183-282`, claves `Coop_DAL_{RuleName}GetAll_Cache` / `…GetAllDictionary_Cache`, 1 h) y en un
miembro en memoria cuando no lo hay —dos caminos, dos comportamientos—; la **metadata de parámetros** de stored
procedures se guarda en un `Hashtable` estático con espejo en `HttpContext` y 1 día
(`SqlHelperParameterCache.cs:46-63`); y el **timeout de comandos** se lee de `AppSettings` y se espeja en el
mismo caché del host (`SQLHelper.cs:55-79`). El resultado es una capa que **no puede ser portable sin el
`System.Web`** y cuyo comportamiento depende del host.

Si no se implementa: `Data` no puede ser `netstandard2.1` (bloqueante `System.Web`, ADR-001 §5), no hay una
respuesta única para el crecimiento de memoria en un proceso de escritorio de larga vida, y el caché sigue sin
poder reemplazarse sin tocar el núcleo (que es lo que resolvería una futura satélite).

**Autorización preventiva de la revisión 2 (la trampa conocida, adelantada).** Este Bolt **aporta y transforma
archivos del 1.4** (los de §B.1 y lo que el aporte traiga) y el árbol hereda `Nullable=enable` +
`TreatWarningsAsErrors` del Bolt 001: sin autorización explícita, el porteo de un archivo legacy produce
`CS8618`/`CS8603`/`CS8625`, el build los convierte en **errores** y el gate de Build no puede cerrar en `pass`. Eso
ya ocurrió **tres veces** en esta ola (Bolts 003, 007 y 009, cada una con su revisión material) y el Bolt 022 lo
previno por la misma vía. Esta revisión **autoriza de antemano** la respuesta ya probada —`#nullable disable` **por
archivo aportado del 1.4** que la requiera (§B.2)— y **nada más**. El resto del Bolt **no se reabre**.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 4 ruteó F-07/F-08 (caché atado a `HttpContext`, con sus
cuatro puntos en archivos centrales de `Data`, incluido el helper de 2532 líneas) a un **ADR**, que es ADR-004.
`OQ-002` fijó la decisión de alcance: **`ICacheProvider` con default en memoria** para todos los hosts y
adaptadores intercambiables **por configuración**, con el **adaptador web dedicado descartado** como entregable.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-004 §3.1** — el contrato `ICacheProvider` vive en `Data` (único consumidor: gateways, loaders y la
  caché de parámetros) con **superficie mínima** —`TryGet<T>(key)`, `Set<T>(key, value, ttl)`,
  `GetOrCreate<T>(key, factory, ttl)`, `Remove(key)`— y **claves del llamador** bajo un esquema documentado
  (`coop:{scope}:{rule}:{kind}`); **no hay `RemoveByPrefix`** en 2.0.0 (quien invalida conoce sus claves);
  **default en memoria para todos los hosts**, implementación propia **sin dependencias de terceros** (BCL),
  **thread-safe**, con **TTL por entrada** y **expiración perezosa**, y un **cap de entradas configurable con
  desalojo** (primero las vencidas) para que un proceso de escritorio largo no acumule memoria sin límite;
  **defaults por región**, iguales a los de 1.4 donde existían: colecciones **1 h**, metadata de parámetros de
  stored procedure **1 día**; **`System.Web` fuera del framework** (un solo camino, sin `HttpContext`); y **dos
  cambios de comportamiento** que hay que declarar: (a) en escritorio el caché de colecciones **pasa a tener
  TTL** (antes no expiraba nunca) y (b) cuando se configure un adaptador compartido, el alcance pasa a ser
  **entre instancias** (antes era por instancia).
- **ADR-004 §3.2** — el **catálogo de configuración** es la fuente de los vencimientos: `Cooperator:Cache:DefaultTtlSeconds`
  y `Cooperator:Cache:ParameterMetadataTtlSeconds` (default 1 día, igual que 1.4), y **"toda región de caché con
  default propio tiene su clave: el default no vive escondido en el código"**.
- **ADR-004 §5 (NFRs)** — *Cache memory bound* (con más entradas que el cap, el total almacenado **nunca** lo
  supera; **0** entradas vencidas servidas) y *Config fail-fast* (una configuración inválida o ausente **no**
  produce fallback silencioso: falla con la excepción propia del framework en `Core`).
- **ADR-001 §3.4/§5/§3.1/§3.5** — presupuesto de dependencias (`Data`: **exactamente un** provider ADO.NET y
  nada más) y umbral de portabilidad (0 referencias a APIs Windows-only en los paquetes portables; `Data` sin
  la plataforma web); sin paquete `-windows` ni `-web`.
- **ADR-005 §3.4 (cruce, del Bolt 014)** — `SqlHelperParameterCache` **usa** este contrato con su propia región
  y TTL por defecto (1 día) en lugar del `Hashtable` estático con espejo en `HttpContext`.

**Qué decide este SPEC, dentro de esas restricciones.** La forma concreta de la implementación en memoria
(estructura, desalojo, expiración), los **nombres concretos de las regiones y de las claves** dentro del esquema
documentado, el punto donde se leen el TTL y el cap del catálogo, la sustitución punto por punto de los tres
puntos de caché (con su inventario), y el **borde con el Bolt 014** sobre los dos archivos que ambos tocan
(`SQLHelper.cs` y `SqlHelperParameterCache.cs`): **acá se cambia el caché; el proveedor, el tipado del interior
y el hazard de la conexión del llamador son de su Bolt** (§4/§14).

**Frontera y alcance cruzado.** Este Bolt **no** define el contrato de configuración ni su catálogo (Bolt 011:
los **consume**), **no** publica ningún adaptador de caché (ADR-004 §3.1: en 2.0.0 no se publica ninguno),
**no** toca el caché de providers Lazy (Bolt 005: otro contrato y otro dueño) y **no** cambia el proveedor de
datos ni las firmas de la capa (Bolt 014).

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-013`](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-07, F-08 (caché atado a `System.Web` y a la plataforma; corrección factual del caché de escritorio) y §6 ítem 4 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.1 (contrato, default en memoria, TTL/cap/desalojo, `System.Web` fuera y cambios de comportamiento), §3.2 (claves de TTL por región y regla del default publicado), §3.5 (vencimiento de la excepción transitoria que este Bolt ejecuta al eliminar la configuración clásica), §5 (NFRs) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1/§3.5 (`Data` portable, sin la plataforma web, sin paquete `-windows`/`-web`), §3.4 (presupuesto de dependencias: un solo provider ADO.NET), §5 (NFR de portabilidad y huella de dependencias) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR (cruce declarado, del Bolt 014) | [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) §3.4 — `SqlHelperParameterCache` usa `ICacheProvider` con su región y TTL, y el hazard de la conexión del llamador | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — el contrato de configuración y su catálogo de claves: **fuente de los valores** de TTL/cap. Su SPEC canónica ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md)) está en disco y declarada en `prerequisites` (§5/§15) | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — arnés, build compartido y suite de caracterización | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Cooperator.Framework.Data/BaseGateway.cs` (`CheckForToken` en 100-129 y sus llamadas; el caché de
colecciones en 315-463: `ResetCache()` en 315, claves `Coop_DAL_{RuleName}GetAll_Cache` /
`…GetAllDictionary_Cache` en 330/334/374/397/428/451, `HttpContext.Current == null` como discriminador del camino
en 323/369/391/423/445, `Cache.Add(..., DateTime.Now.AddHours(1), …)` en 403/457, el cast
`(SqlTransaction)transaction` en 640), `BaseLoader.cs` (177-282: `ResetCache()` y las mismas claves),
`SQLHelper.cs` (44-79: `commandTimeOut` estático, `readCommandTimeOut()`, `HttpContext` y la clave
`Cooperator_DAL_SP_CommandTimeOut` con 1 día; el default leído de la configuración),
`SqlHelperParameterCache.cs` (46-63: `Hashtable` estático sincronizado, espejo en `HttpContext` con la clave
`Cooperator_DAL_SP_Params_Cache` y 1 día; 73-84: apertura/cierre de la conexión del llamador — hazard del Bolt
014) y `BaseRule.cs` (la cadena de conexión y su `Dictionary` estático — Bolt 014/ADR-004 §3.3).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- El **contrato de caché** `ICacheProvider` con su **superficie mínima** (`TryGet<T>`, `Set<T>`,
  `GetOrCreate<T>`, `Remove`) y el **esquema de claves del llamador** (`coop:{scope}:{rule}:{kind}`).
- La **implementación por defecto en memoria**: BCL puro, **sin dependencias de terceros**, **thread-safe**,
  **TTL por entrada** con **expiración perezosa**, **cap de entradas configurable con desalojo** (primero las
  vencidas) y los **defaults por región** de ADR-004 §3.1.
- La **sustitución de los tres puntos de caché de la capa** por el contrato: **colecciones**
  (`GetAll`/`GetAllDictionary` de gateways y loaders), **metadata de parámetros de stored procedure**
  (`SqlHelperParameterCache`) y **timeout de comandos** (`SQLHelper`), cada uno con su **región**, su **clave
  publicada** y su **TTL**.
- La **invalidación del consumidor que conoce sus claves**: `ResetCache()` de cada gateway/loader remueve
  **sus** claves con `Remove(key)` (sin `RemoveByPrefix`, que no existe en 2.0.0).
- El **retiro de la dependencia de la plataforma web** de la capa de datos: 0 usos de `HttpContext` y de su
  caché, un solo camino para todos los hosts, y la referencia a `System.Web` fuera del proyecto.
- Los **TTL por región y el cap** tomados del **catálogo de configuración** (Bolt 011), con su default
  **publicado** en el catálogo y nunca escondido en el código.
- La declaración de los **cambios de comportamiento observables** (vencimiento del caché de colecciones en
  escritorio y alcance entre instancias con un adaptador compartido) para las **notas de migración**, y la
  entrada de las rupturas de superficie que este Bolt introduzca (ADR-008 §3.1).
- La **política de nulabilidad del código aportado del 1.4** (revisión 2, §B.2): la directiva `#nullable disable`
  **por archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —**jamás** en código nuevo (el contrato
  de caché y su implementación se escriben con la nulabilidad activa) y **prohibida** a nivel de proyecto—, como
  mecánica de build del aporte.

### Out of scope

- El **contrato de configuración y su catálogo de claves**: **ADR-004 §3.2 / Bolt 011** (se **consumen**). Este
  SPEC no define claves, no renombra las existentes y no implementa el contrato ni su fuente por defecto.
- **Publicar un adaptador de caché** o definir su paquete satélite: ADR-004 §3.1 (en 2.0.0 no se publica
  ninguno; el naming queda definido para no improvisarlo).
- El **proveedor de datos**, sus casts, el **tipado del interior** de `Data`, el **timeout como regla de
  comando** (todo comando con timeout explícito y acotado) y el **hazard de la conexión del llamador** en el
  descubrimiento de parámetros: **Bolt 014** (ADR-005 §3.2/§3.4). Los dos archivos compartidos llevan la
  frontera declarada en §14.
- El **caché de providers Lazy** (`Core`) y su reseteo: **Bolt 005** — otro contrato, otro dueño, otro alcance.
- El **retiro de la selección de provider por nombre** y de la configuración clásica del `.config`: Bolts 014 y
  011 respectivamente.
- El **gate por token** (`CheckForToken`) de la superficie de datos y su contraparte generada: **Bolt 015**.
- **Invalidación por prefijo** o cualquier mecanismo de barrido global: ADR-004 §3.1 (quien invalida conoce sus
  claves).

---

## 5. Prerequisites and baseline

- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md), `prerequisites`):** la
  configuración de build compartida (`Directory.Build.props`: analizadores, determinismo, warnings como error),
  el arnés de pruebas y la **suite de caracterización** que corre **antes y después** de sustituir los puntos de
  caché, más la evidencia de build del NFR de portabilidad (0 referencias Windows-only/web en los paquetes
  portables).
- **El contrato de configuración de ADR-004 §3.2 (Bolt 011) — prerrequisito directo.** Su entregable
  (`ICooperatorConfiguration` con su acceso ambiental y el **catálogo de claves**) es de donde salen el TTL de
  cada región y el cap, y de donde sale la excepción tipada de configuración inválida (NFR *Config fail-fast*).
  Su SPEC canónica es
  [`SPEC-261016-0018-configuracion-portable.md`](./SPEC-261016-0018-configuracion-portable.md) —redactada por
  otra sesión y **ya en disco** al cerrar esta revisión: por eso figura en `prerequisites`—. El prerrequisito es
  su **ejecución**, no su documento: si al iniciar el V-Bounce el contrato y el catálogo no están en el árbol
  2.0, la corrida **se detiene** (§15). La salida es una **enmienda del Bolt** o una decisión de orden del
  dueño, **nunca** esconder el default en el código (que es exactamente lo que el Bolt prohíbe) ni leer
  `App.config`.
- **El proyecto `Cooperator.Framework.Data` en el árbol 2.0** (`netstandard2.1`) con la superficie base y sus
  puntos de caché actuales. Si no existe, **parar y reportar**: este Bolt no crea la capa de datos.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de los puntos de caché y de
  los defaults que se migran.
- **Toolchain:** el SDK .NET pinneado por el Bolt 001; `Data` sigue con **exactamente un** proveedor ADO.NET
  (ADR-001 §3.4) y este Bolt **no** agrega dependencias.
- **Build state de partida:** el árbol 2.0 compila con su arnés y la suite de caracterización está verde. Este
  Bolt **no** cambia TFMs, no agrega paquetes y no publica artefactos de distribución.
- **Decisión de ejecución registrada (no es supuesto):** nada de este Bolt se verifica contra la base de datos:
  la caché es de proceso y su comportamiento por defecto es en memoria. La corrida real de la capa de datos es
  del Bolt 014, de ahí el plan de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-013`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).
>
> **Mecánica de build del aporte del 1.4 (revisión 2, §B.2).** Cualquier archivo que este Bolt **aporte del 1.4**
> —los de §B.1 (`BaseGateway.cs`, `BaseLoader.cs`, `SQLHelper.cs`, `SqlHelperParameterCache.cs` y su cierre) y lo
> que el aporte traiga— sigue la **política de nulabilidad de §B.2**: `#nullable disable` como primera línea útil
> **sólo** en los archivos aportados que **produzcan** `CS8618`/`CS8603`/`CS8625` (criterio objetivo; el build es la
> prueba), **nunca** en código nuevo (el contrato de caché y su implementación conservan la nulabilidad activa) y
> **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es la trampa que ya detuvo a los Bolts
> 003/007/009 —y que el 022 previno igual—: queda autorizada **antes** del V-Bounce.

### Phase A — El contrato de caché y su implementación en memoria

**Duration:** 4h total cycle — **Complexity:** High

#### A.1 El contrato y el esquema de claves

`ICacheProvider` se declara en `Data` con la superficie mínima de ADR-004 §3.1 —
`TryGet<T>(key)`, `Set<T>(key, value, ttl)`, `GetOrCreate<T>(key, factory, ttl)`, `Remove(key)`— y **sin**
`RemoveByPrefix`. El contrato **no interpreta** las claves: las recibe del llamador. El **esquema documentado**
es `coop:{scope}:{rule}:{kind}` y este SPEC fija los nombres concretos de las claves y regiones que la propia
capa de datos usa (§B.1), de modo que queden **publicados** en la documentación pública del paquete junto al
contrato —no en un comentario interno—.

**Files created:**
- `2.0/src/Cooperator.Framework.Data/Caching/ICacheProvider.cs` — el contrato de caché (superficie mínima, claves del llamador, TTL por entrada)
- `2.0/src/Cooperator.Framework.Data/Caching/CacheKey.cs` — el esquema de clave (`coop:{scope}:{rule}:{kind}`) y las claves publicadas de las regiones de `Data`
- `2.0/src/Cooperator.Framework.Data/Caching/CacheRegions.cs` — las regiones de la capa, su TTL por defecto y las claves del catálogo de las que se leen

#### A.2 La implementación por defecto en memoria

La implementación es **propia y sólo BCL** (ADR-004 §3.1: sin dependencias de terceros; la pila
`Microsoft.Extensions.*` está descartada por ADR-004), **thread-safe**, con **TTL por entrada** y **expiración
perezosa** (una entrada vencida **nunca** se sirve: se descarta al leerla), y con un **cap de entradas
configurable con desalojo** que empieza por las **vencidas** y, si hace falta, por las de vencimiento más
cercano —de modo que el total almacenado **nunca** supera el cap (NFR *Cache memory bound*, ADR-004 §5)—. Los
**defaults por región** son los de ADR-004 §3.1: colecciones **1 h**, metadata de parámetros de stored
procedure **1 día**; el **cap** y cada TTL con default propio se leen del catálogo (§D.1) y **ningún** default
vive en el código.

**Files created:**
- `2.0/src/Cooperator.Framework.Data/Caching/InMemoryCacheProvider.cs` — la implementación en memoria (TTL, expiración perezosa, cap con desalojo, thread-safety, BCL puro)

---

### Phase B — Los tres puntos de caché de la capa

**Duration:** 4h total cycle — **Complexity:** High

#### B.1 Sustitución punto por punto, con el inventario como evidencia

| Punto de caché (1.4) | Qué se hace | Clave/región en 2.0 |
|---|---|---|
| Colecciones de **gateway** (`Coop_DAL_{RuleName}GetAll_Cache`, `…GetAllDictionary_Cache`, 1 h, `HttpContext` o miembro en memoria) | Se sustituye por el contrato; `ResetCache()` remueve **sus** claves con `Remove(key)`; desaparece el discriminador por host | `coop:data:{rule}:collection-all` / `coop:data:{rule}:collection-dictionary` (región de colecciones) |
| Colecciones de **loader** (las mismas claves) | Ídem, con la misma región y el mismo TTL | `coop:data:{rule}:collection-all` / `…collection-dictionary` (región de colecciones) |
| **Metadata de parámetros de SP** (`Hashtable` estático + espejo en `HttpContext`, `Cooperator_DAL_SP_Params_Cache`, 1 día) | Se sustituye por el contrato con **su propia región** y TTL por defecto (1 día); el `Hashtable` estático y el espejo en el caché del host desaparecen | `coop:data:sp-parameters:{sp}:metadata` (región de metadata, TTL 1 día) |
| **Timeout de comandos** (`Cooperator_DAL_SP_CommandTimeOut`, 1 día, espejado en el caché del host) | El punto de caché pasa a la región propia del contrato; **su valor** sale del catálogo (`Cooperator:Data:CommandTimeoutSeconds`, ADR-004 §3.2) | `coop:data:command-timeout:value` (región de timeout) |

- La **invalidación** sigue siendo la del consumidor que conoce sus claves: cada `ResetCache()` remueve
  **exactamente** las suyas y **no** toca las de otro (ADR-004 §3.1). Una operación de escritura que hoy
  invalida (`BaseGateway.cs:1002/1114/1239/1410`) lo sigue haciendo, con las claves nuevas.
- **Frontera con el Bolt 014 (declarada):** acá se cambia **dónde se guarda** la metadata y el timeout. En
  `SqlHelperParameterCache` **no** se cambia el descubrimiento de parámetros ni el hazard de abrir/cerrar la
  conexión del llamador (ADR-005 §3.4: es de su Bolt), y en `SQLHelper` **no** se cambia el proveedor, el tipado
  del interior ni la regla "todo comando con timeout explícito y acotado" ni el retiro del `SqlCommand`
  descartable (ADR-005 §3.2: también de su Bolt).

**Files modified:**
- `2.0/src/Cooperator.Framework.Data/BaseGateway.cs` — caché de colecciones por el contrato, `ResetCache()` sobre sus claves, retiro del camino por host
- `2.0/src/Cooperator.Framework.Data/BaseLoader.cs` — ídem
- `2.0/src/Cooperator.Framework.Data/SqlHelperParameterCache.cs` — metadata de parámetros por el contrato (clave y TTL publicados); el resto del archivo es del Bolt 014
- `2.0/src/Cooperator.Framework.Data/SQLHelper.cs` — el punto de caché del timeout por el contrato; el resto del archivo es del Bolt 014

#### B.2 La mecánica de build del aporte: política de nulabilidad (revisión 2)

El árbol 2.0 hereda de `2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`,
mientras que el código del 1.4 es **anterior** a las anotaciones de nulabilidad. Es la trampa **ya medida** en esta
ola —`MEM-261016-0107` §8.2 (28 diagnósticos `CS86xx` al portar el modelo), `MEM-261016-0209` §8.1 (`CS8618` en
`BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`) y `MEM-261016-0227` §13 (el aviso declarado del
Bolt 009)— y afectaría a **este Bolt** en cuanto aporte o transforme un archivo del 1.4 con elementos sin
inicializar, retornos posiblemente nulos o `null` literales: los diagnósticos `CS8618`/`CS8603`/`CS8625` que el
árbol convierte en **errores** y que dejan el gate de Build sin poder cerrar en `pass`.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
`SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3):** cada archivo **aportado del 1.4** que **produzca diagnósticos
de nulabilidad** empieza con la directiva `#nullable disable` (primera línea útil, antes de los `using`), y **sólo**
esos archivos la llevan:

- **Sí la llevan:** los archivos **aportados del 1.4** por este Bolt que produzcan `CS8618`/`CS8603`/`CS8625` (los
  de §B.1 y lo que el aporte traiga). El **criterio es objetivo** —*archivo aportado del 1.4 + diagnóstico de
  nulabilidad*— y la lista exacta la fija la **ejecución con el build**, no un nombre supuesto. La directiva
  **apaga el análisis**: no cambia una instrucción de IL y deja la semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —el contrato `ICacheProvider`, `CacheKey`, `CacheRegions`,
  `InMemoryCacheProvider` y las pruebas—, que conserva la nulabilidad **activa** del árbol porque está escrito para
  el mundo anotado; y queda **prohibido** `<Nullable>disable</Nullable>` en **cualquier** `.csproj`, incluido el de
  `Data`: apagaría el análisis del proyecto entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta, el build
  **falla** con el diagnóstico exacto (`CS8618`/`CS8603`/`CS8625`); si **sobra** en código nuevo, se ve en la lectura
  de los archivos entregados. Se verifica en §8.1, §9 y AC-9.

**Un `using` que quede sin uso** como consecuencia del **retiro** de este Bolt (los usos de la plataforma web y del
caché del host, §C.1) se retira **con él**: es parte de la forma ya resuelta del archivo. Un `using` muerto **hacia
un namespace que no existe en el árbol 2.0** que **no** provenga de un retiro propio **no** está cubierto por esta
autorización: se **detiene y se reporta** (§15).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`,
inicializadores o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que la ordene y que
además tocaría, de paso, archivos cuyo aspecto pertenece al **Bolt 014**; **diferir el aporte** dejaría al Bolt sin
la capa sobre la que debe sustituir los puntos de caché. La directiva, en cambio, es de **una línea por archivo**,
su efecto queda **acotado a los archivos aportados** y **no toca el comportamiento observable**: es el mismo
criterio con el que se resolvió en los Bolts 003, 007 y 009.

---

### Phase C — Retiro de la dependencia del host y de la plataforma web

**Duration:** 3h total cycle — **Complexity:** Medium

#### C.1 Un solo camino para todos los hosts

Desaparecen los **caminos condicionados al host**: no hay `if (HttpContext.Current == null)` para elegir dónde
cachear, no hay espejo en el caché del host, no hay `HttpContext` en la capa de datos y **no queda ningún punto
de caché condicionado al host** (criterio de aceptación explícito del Bolt). La referencia a `System.Web` sale
del proyecto, con lo que `Data` deja de ser Windows/web-exclusiva por su caché (ADR-001 §5, NFR de
portabilidad).

**Files modified:**
- `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` — retiro de la referencia a la plataforma web
- `2.0/src/Cooperator.Framework.Data/BaseGateway.cs`, `BaseLoader.cs`, `SQLHelper.cs`,
  `SqlHelperParameterCache.cs` — retiro de los usos de la plataforma (§B.1)

---

### Phase D — TTL por región y cap desde el catálogo de configuración

**Duration:** 2h total cycle — **Complexity:** Medium

#### D.1 Ningún default escondido en el código

Cada región con default propio obtiene su valor del **catálogo de configuración** (ADR-004 §3.2), a través del
**contrato de configuración del Bolt 011**:

- `Cooperator:Cache:DefaultTtlSeconds` → TTL de la **región de colecciones** (default documentado **1 h**, igual
  a 1.4).
- `Cooperator:Cache:ParameterMetadataTtlSeconds` → TTL de la **región de metadata de parámetros** (default
  documentado **1 día**, igual a 1.4).
- **Cap de entradas**: el **límite** de la implementación en memoria (ADR-004 §3.1: "cap de entradas
  configurable con desalojo") necesita **su clave publicada** —la regla de ADR-004 §3.2 es que todo default
  propio vive en el catálogo—. La clave concreta del cap la **propone** este SPEC en §14 y su publicación queda
  del lado del catálogo (**Bolt 011**): si el catálogo no la declara, **parar y pedirla** (§15) — esconderla en
  el código está prohibido por el Bolt.
- Un valor **inválido o ausente** de una clave con default **no** produce un fallback silencioso: el error
  tipado es el del **contrato** de configuración (NFR *Config fail-fast* de ADR-004 §5, Bolt 011). Lo que este
  Bolt garantiza es que **no** exista un camino propio de lectura ni un fallback escondido.

**Files modified:**
- `2.0/src/Cooperator.Framework.Data/Caching/CacheRegions.cs` — lectura del TTL por región y del cap desde el contrato de configuración, con las claves del catálogo

---

### Phase E — Evidencia y cambios de comportamiento declarados

**Duration:** 2h total cycle — **Complexity:** Low

#### E.1 Evidencia

- **Build de la capa sin la plataforma web** (NFR de portabilidad, ADR-001 §5) y sin dependencias nuevas
  (ADR-001 §3.4).
- **NFR *Cache memory bound*** (ADR-004 §5): con escrituras concurrentes **por encima del cap**, el total
  almacenado **nunca** lo supera y **cero** entradas vencidas se sirven; con la inspección del conteo interno.
- **TTL por región verificado** contra el catálogo (valor configurado y default documentado).
- **Inventario de los puntos de caché** del alcance, cada uno con su resolución (§B.1) — es la evidencia de que
  **no queda** ningún punto condicionado al host.
- **Suite de caracterización del Bolt 001 verde** antes y después, y `ResetCache()` de cada gateway/loader
  removiendo **sus** claves (y no las ajenas).
- **Cambios de comportamiento observables** identificados y declarados: vencimiento del caché de colecciones en
  escritorio (antes vivía para siempre) y alcance entre instancias cuando se configure un adaptador compartido,
  más el retiro del espejo en el caché del host. Van a las **notas de migración** (ADR-004 §3.1; documento del
  Bolt 028) junto con las **rupturas de superficie** de este Bolt (ADR-008 §3.1).

**Files created:**
- `2.0/src/Cooperator.Framework.Data/Caching/README-cache.md` — el contrato publicado: superficie, esquema de claves, regiones y sus claves de catálogo, límites (cap, TTL) y las dos advertencias de comportamiento

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: El caché vive detrás del contrato, con la superficie mínima

**Given** la capa de datos construida
**When** se inventaría cómo cachea
**Then** todos sus puntos de caché (colecciones, metadata de parámetros de stored procedure y timeout de
comandos) pasan por `ICacheProvider` con la superficie mínima de ADR-004 §3.1 (`TryGet<T>`, `Set<T>`,
`GetOrCreate<T>`, `Remove`) y **no** existe una operación de invalidación por prefijo

### AC-2: Un solo camino, sin el caché del host ni la plataforma web

**Given** el código y el build de `Data`
**When** se inspeccionan sus dependencias y sus caminos de caché
**Then** hay **0** usos de `HttpContext` y de su caché, **0** referencias a la plataforma web en el proyecto, y
**ningún** punto de caché condicionado a si hay host web o no

### AC-3: El límite se respeta y ninguna entrada vencida se sirve

**Given** una instancia del caché con su cap configurado
**When** se escriben **más entradas que el cap** —incluidas escrituras concurrentes— y se leen entradas vencidas
**Then** el total almacenado **nunca** supera el cap (el desalojo empieza por las vencidas) y el número de
entradas vencidas **servidas** es **cero** (NFR *Cache memory bound*)

### AC-4: El TTL de cada región sale del catálogo, no del código

**Given** el contrato de configuración del Bolt 011
**When** se construyen los puntos de caché
**Then** el TTL de la región de colecciones y el de la región de metadata de parámetros se leen de
`Cooperator:Cache:DefaultTtlSeconds` y `Cooperator:Cache:ParameterMetadataTtlSeconds` (con sus defaults
documentados de 1 h y 1 día), cada región con default propio tiene su clave **publicada** en el catálogo, y **no
existe** un default escondido en el código ni un camino propio de lectura

### AC-5: La implementación se sostiene bajo concurrencia

**Given** varias tareas concurrentes escribiendo, leyendo y desalojando sobre la misma instancia
**When** la corrida termina
**Then** no hay corrupción, no hay excepciones de concurrencia y los invariantes de AC-3 se mantienen

### AC-6: La invalidación es la del consumidor que conoce sus claves

**Given** un gateway/loader con sus entradas cacheadas y entradas de **otra** región presentes
**When** se invoca su `ResetCache()`
**Then** se remueven **exactamente** sus claves y las de otro consumidor/región permanecen intactas

### AC-7: El inventario de puntos de caché está completo y cada uno tiene su clave publicada

**Given** el entregable
**When** se lee el inventario de §B.1 y la documentación del contrato
**Then** cada punto de caché del alcance figura con su región, su clave y su TTL, y la documentación pública
publica el esquema de claves y las regiones (no un comentario interno)

### AC-8: La capa no arrastra dependencias nuevas y los cambios de comportamiento quedan declarados

**Given** el árbol 2.0 construido y la evidencia del V-Bounce
**When** se corre la suite de caracterización y se revisa el presupuesto de dependencias
**Then** la suite queda verde, `Data` conserva **exactamente un** proveedor ADO.NET y **ninguna** dependencia
nueva (el caché es BCL puro), y los cambios de comportamiento observables y las rupturas están declarados para
las notas de migración (ADR-004 §3.1, ADR-008 §3.1)

### AC-9: El código aportado del 1.4 no rompe el build por nulabilidad

**Given** los archivos del 1.4 que este Bolt aporta o transforma (los de §B.1 y lo que el aporte traiga)
**When** se construye la capa con advertencias tratadas como error
**Then** no queda **ningún** diagnóstico de nulabilidad: cada archivo **aportado del 1.4** que lo requiera lleva la
directiva de **§B.2** como primera línea útil, **ningún** archivo nuevo la lleva, y **ningún** `.csproj` declara
`<Nullable>disable</Nullable>`

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — la capa no referencia la plataforma web ni depende del host para cachear | §6/C.1 | AC-2 · build + inventario de §B.1 |
| Bolt §2 — todos los puntos de caché del alcance usan el contrato | §6/B.1 | AC-1, AC-7 · `CachePointTests` + inventario |
| Bolt §2 — el default en memoria respeta el límite y no sirve entradas vencidas, y se sostiene bajo concurrencia | §6/A.2 | AC-3, AC-5 · `InMemoryCacheProviderTests` (cap, vencimiento, concurrencia) |
| Bolt §2 — el TTL de cada región sale de la configuración y no de un valor escondido | §6/D.1 | AC-4 · `CacheRegionsTests` (doble de configuración) |
| Bolt §2 — la invalidación sigue siendo la del consumidor que conoce sus claves | §6/B.1 | AC-6 · `CacheInvalidationTests` |
| Bolt §2 — inventario de los puntos de caché, cada uno con su resolución | §6/B.1, §6/E.1 | AC-7 · inventario + `README-cache.md` |
| Bolt §2 — cambios de comportamiento y rupturas declarados (ADR-004 §3.1 / ADR-008 §3.1) | §6/E.1 | AC-8 · lista en el MEM para el Bolt 028 |
| ADR-004 §3.1 — superficie mínima del contrato y ausencia de `RemoveByPrefix` | §6/A.1 | AC-1 · inspección del contrato + gate de API |
| ADR-004 §5 — NFRs *Cache memory bound* y *Config fail-fast* | §6/A.2, §6/D.1 | AC-3, AC-4 · los tests del NFR + verificación de que no hay fallback propio |
| ADR-001 §3.4/§5 — un solo provider y portabilidad sin la plataforma web | §6/C.1, §6/E.1 | AC-2, AC-8 · build del árbol + listado de dependencias |
| Bolt §2 — el código del 1.4 que este Bolt aporta no rompe el build por nulabilidad (mecánica ya medida en los Bolts 003/007/009 y prevenida en el 022) | §6/B.2: directiva `#nullable disable` **por archivo aportado del 1.4 + diagnóstico** | AC-9 · build de `Data` + lectura de los archivos entregados |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests:** de la implementación en memoria — `Set`/`TryGet` (acierto, falla, valor `null` permitido o
  prohibido según la firma), **TTL por entrada** (entrada viva, entrada **vencida que no se sirve** y se
  descarta al leerla, TTL explícito por entrada distinto del de la región), **cap** (con cap N: escribir N, N+1 y
  N×2 entradas; el conteo **nunca** supera N; el desalojo empieza por las vencidas y si no hay, por la de
  vencimiento más cercano), `GetOrCreate` (crea una vez, no recrea si hay valor vivo, **recrea** si el valor
  venció, y propaga la excepción de la factory **sin cachear** el fallo), `Remove` (remueve su clave y no otras),
  y **concurrencia** (varias tareas escribiendo/leyendo/desalojando: invariantes de AC-3 sostenidos, 0
  excepciones de concurrencia, corrida repetida N veces para descartar falsos verdes).
- **Unit tests de las regiones:** lectura del TTL de cada región desde un **doble de prueba** del contrato de
  configuración (valor configurado y default documentado) y del cap; verificación de que **no** hay un default
  en el código (búsqueda del valor literal en el ensamblado/constantes públicas) ni un camino propio de lectura.
- **Component tests:** los puntos de la capa con el caché real y una **configuración de prueba**: gateway/loader
  cacheando colecciones (`GetAll`, `GetAllDictionary`), `ResetCache()` removiendo sus claves y no las ajenas;
  `SqlHelperParameterCache` guardando la metadata por el contrato con su clave y TTL publicados (sin tocar el
  descubrimiento, que es del Bolt 014); el punto del timeout escribiendo y leyendo su clave.
- **Consumer-side contract tests (test doubles):** **sí aplica** en este Bolt: (a) un **doble de
  `ICooperatorConfiguration`** (el contrato es del Bolt 011) que entrega TTL/cap —o falla— permite verificar AC-4
  y el *Config fail-fast* sin depender de la configuración real del host; (b) un **doble de `ICacheProvider`**
  verifica que los puntos de la capa usan **el contrato** (y no una implementación concreta) y que la
  invalidación es por clave. Es exactamente el nivel hermético: el servicio real (la configuración del host) no
  participa.
- **Política de nulabilidad del código aportado (§B.2, revisión 2):** chequeo por **lectura** de los archivos
  entregados —cada archivo **aportado del 1.4** que produzca diagnósticos de nulabilidad empieza con
  `#nullable disable` y **ningún** archivo nuevo la lleva—, más el propio `build` con
  `TreatWarningsAsErrors=true` (la prueba real: sin la directiva, el build falla con `CS8618`/`CS8603`/`CS8625`). Se
  verifica además que **no** hay `<Nullable>disable</Nullable>` en ningún `.csproj` y que ningún `using` muerto
  hacia un namespace inexistente quedó sin resolver ni autorizado (`using` muerto que **no** provenga de un retiro
  propio ⇒ parar, §15).
- **Edge cases:** cap `1` y cap muy grande; TTL de valor `0`/negativo (contrato y comportamiento definidos);
  entrada que vence **mientras** se lee (no debe servirse); reloj inyectable para no depender de esperas reales
  —el TTL se prueba con un proveedor de tiempo sustituible o con TTLs mínimos, nunca con `Thread.Sleep` largos—;
  claves con el mismo `scope` y distinto `rule`; dos reglas distintas con la misma colección; valor `null`
  cacheado; desalojo con todas las entradas vivas; escritura concurrente desde muchos hilos sobre la misma
  clave (gana una instancia, no hay duplicados incoherentes); `ResetCache()` sobre una región vacía.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida de los tests del NFR (cap y vencimiento) con el conteo
  inspeccionado; la corrida concurrente repetida; la verificación de los TTL por región y de la ausencia de
  defaults en el código; el inventario de puntos de caché; el build de la capa sin la plataforma web y el
  listado de dependencias; la suite de caracterización antes y después. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `refactor` de la caché, y su comportamiento por
  defecto es **en proceso y en memoria**: no toca base de datos, cola, almacenamiento ni servicio externo. La
  corrida real del acceso a datos —y con ella la verificación del hazard de la conexión del llamador— es del
  **Bolt 014**, que sí la tiene declarada.
- **Contract / E2E against the real providers:** `n/a` — el contrato que se implementa no cruza un límite de
  proceso; el contrato de configuración se ejerce con un doble **en el nivel hermético** (§8.1), que es donde
  corresponde mientras el proveedor real de configuración del host es del Bolt 011. Un adaptador de caché
  compartido (Redis u otro) **no existe en 2.0.0** (ADR-004 §3.1), así que no hay proveedor real contra el que
  verificar.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales (el borde de memoria y el de concurrencia son los del propio proceso).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); `Data` compila sin la referencia web y **sin diagnósticos de nulabilidad** (la directiva de §B.2, sólo en los archivos aportados del 1.4 que la requieren) | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluidos los tests del cap, del vencimiento y de la concurrencia | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El doble de `ICooperatorConfiguration` ejerce el contrato de configuración (TTL/cap y fallo de configuración) y el doble de `ICacheProvider` verifica que los puntos usan el **contrato** y no una implementación concreta | pass |
| SAST | V-Bounce | 0 hallazgos altos en el código nuevo de caché | pass |
| Dependency scanning / licenses | V-Bounce | `Data`: exactamente **un** provider ADO.NET y **ninguna** dependencia nueva (caché BCL puro); licencias permisivas (ADR-001 §3.4) | pass |
| OWASP Top 10 — static review | V-Bounce | `n/a` — el cambio no agrega superficie expuesta: no hay entrada externa, ni red, ni proceso; la caché guarda valores ya producidos por la propia capa | n/a (razón) |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción ni se procesa material externo | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** credenciales en las regiones de este Bolt (guardan colecciones, metadata de parámetros y un timeout), 0 valores de connection string en el código, en la documentación o en los tests; el valor de conexión es de la configuración y su resolución es del Bolt 014 | pass |
| Hallucination lint | V-Bounce | Cada punto de caché y cada default trazable al código 1.4 citado y a ADR-004 §3.1/§3.2; 0 claves de catálogo inventadas (las que no existan se piden, §15) | pass |
| IP / license provenance | V-Bounce | Código propio; 0 código de terceros (no se copia ninguna implementación de caché) | pass |
| PII / DLP | V-Bounce | `internal`: las regiones no guardan datos personales ni filas de negocio más allá de lo que la capa ya cacheaba (colecciones del modelo y metadata de esquema) | pass |
| Dependency-confusion | V-Bounce | 0 paquetes nuevos; ningún adaptador satélite publicado (ADR-004 §3.1) | pass |
| Test-first evidence | V-Bounce | Los tests del NFR **se escriben desde ADR-004 §5** (cap nunca superado, 0 vencidas servidas) antes de implementar el desalojo, y el inventario de puntos de caché se levanta del código 1.4 antes de sustituirlos | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Los invariantes (conteo ≤ cap, 0 vencidas) se verifican con la corrida repetida; el tiempo se prueba con un proveedor de reloj sustituible, sin esperas largas ni resultados dependientes de la máquina | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: sin límite de proceso ni adaptador publicado en 2.0.0 | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: el cambio no introduce un camino de red; su efecto de costo es el ahorro de memoria y se mide con el NFR *Cache memory bound* | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: la capa no agrega telemetría (§11); el conteo interno se expone para la verificación del NFR | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). Las regiones en alcance guardan
  **colecciones de negocio ya producidas por la capa**, **metadata de parámetros** de stored procedures y un
  **timeout**: información interna del proceso, sin credenciales y sin datos personales más allá de lo que la
  capa ya cacheaba en 1.4.
- **Ninguna credencial en la caché:** la cadena de conexión **no** es un valor de estas regiones. ADR-004 §3.3
  además elimina el `Dictionary` estático de connection strings (Bolt 011/014) y la resolución del valor es de
  la capa que lo consume, no del caché.
- **Cota de memoria como propiedad de robustez:** el cap con desalojo no es sólo eficiencia —una caché sin
  límite en un proceso de escritorio de larga vida es un camino de agotamiento de memoria—; el NFR
  *Cache memory bound* (ADR-004 §5) lo mide con escrituras por encima del límite.
- **Sin superficie nueva:** el caché es en proceso, no expone red ni archivos, no acepta entrada externa y no
  ejecuta nada del contenido que guarda (son valores, no instrucciones).
- **Sin secretos en el repositorio:** la configuración de prueba de los tests usa valores inocuos; ninguna
  credencial se documenta ni se comitea (la del entorno de integración es del Bolt 014 y vive en el entorno).
- **Sin degradación silenciosa:** un valor de configuración inválido o ausente de una clave con default no
  habilita un camino propio de fallback: el error tipado es el del contrato de configuración (NFR *Config
  fail-fast*). Que **no** exista un default escondido en el código es, además, un control de comportamiento:
  el operador no queda con dos fuentes de verdad.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es una capa de biblioteca (`netstandard2.1`) y no emite
  logs, métricas ni trazas a ningún colector; no se le agrega instrumentación porque ninguna fuente lo pide y el
  caché en proceso no tiene ciclo de vida que observar.
- **La observabilidad de este Bolt es su evidencia:** el **conteo interno** del caché (expuesto para la
  verificación del NFR: total ≤ cap y 0 vencidas servidas), el **inventario de puntos de caché** con su
  resolución, la verificación de los **TTL por región** contra el catálogo y el **build sin la plataforma web**.
- **La observabilidad en runtime del consumidor** es la que ya existe y no cambia: el caché no reporta su
  estado al host (en 1.4 tampoco); quien necesite medir el efecto de la caché mide en su propia capa. Si en el
  futuro un adaptador satélite necesita telemetría, es de su Bolt, no de éste.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay son dos **cambios de comportamiento observables**
  (ADR-004 §3.1) que se declaran explícitamente para las **notas de migración** (documento del **Bolt 028**):
  (a) en **escritorio** el caché de colecciones **pasa a tener TTL** —antes vivía para siempre—, con 1 h por
  defecto; y (b) cuando en el futuro se configure un **adaptador compartido**, el alcance del caché pasa a ser
  **entre instancias**, mientras que en 1.4 era por instancia. En 2.0.0 **no se publica ningún adaptador**
  (ADR-004 §3.1), así que (b) es una propiedad del contrato a documentar, no un efecto de este entregable. La
  caché de la capa **no** persiste nada: no hay datos que migrar ni que reconciliar.
- **Compatibility:** el contrato (`ICacheProvider`) es **aditivo**. Las claves internas cambian de nombre
  (`Coop_DAL_*`, `Cooperator_DAL_SP_*` → el esquema `coop:{scope}:{rule}:{kind}`), pero **no** eran contrato
  público; `ResetCache()` conserva su firma y su significado ("remueve lo mío"). El **retiro del espejo en el
  caché del host** no rompe una API: elimina un camino. Las **rupturas de superficie** que este Bolt introduzca
  se registran con su reemplazo (ADR-008 §3.1) y el **gate de API** del Bolt 001 las contrasta contra la línea
  base 1.4: una diferencia no listada **falla**. Si la implementación en memoria se expone como tipo público
  (§14), es una adición, no una ruptura.
- **Vencimiento de la excepción transitoria:** ADR-004 §3.5 declara que con esta decisión `Data` **no** toma
  `System.Configuration.ConfigurationManager` y el umbral del NFR *dependency-footprint* vuelve a "exactamente
  una dependencia de terceros". Este Bolt **ejecuta** ese vencimiento: retira la referencia a la plataforma web
  y **no** hereda la excepción. La evidencia es el build del árbol con su listado de dependencias.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido (la caché es de proceso), ni archivo
  migrado, ni valor reescrito. Un consumidor que hubiera quedado con más memoria retenida por el cambio de TTL
  recupera el comportamiento anterior revirtiendo el paquete; no hay migración que deshacer. El árbol `1.4/`
  permanece intacto y es la contención soportada (ADR-008 §3.1).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Quedan dos caminos según el host** (sobrevive un `if (HttpContext.Current == null)` o un espejo en el caché del host) | 3 | 4 | AC-2 con **dos** verificaciones complementarias: el inventario de puntos de caché (§B.1) y el build sin la plataforma web; la búsqueda de `HttpContext` en la capa es parte de la evidencia |
| **El cap no se respeta bajo concurrencia** (crecimiento de memoria en un proceso largo, que es el motivo del cap) | 3 | 4 | Implementación thread-safe con desalojo y **NFR medido** con escrituras concurrentes por encima del cap, con el conteo inspeccionado y la corrida repetida |
| **Una entrada vencida se sirve** (expiración no perezosa o comparación de tiempo mal hecha) | 3 | 3 | NFR *Cache memory bound*: **0** vencidas servidas, con un proveedor de reloj sustituible para probarlo sin esperas |
| **Un default esconde el código** (TTL o cap hardcodeado, contra la prohibición expresa del Bolt) | 3 | 4 | AC-4: lectura desde el catálogo con doble de configuración + verificación de ausencia de literales; y la regla de §D.1: si el catálogo no publica la clave, **se pide** (§15) |
| **Cambio de comportamiento no declarado** (el vencimiento del caché de colecciones en escritorio se descubre en producción) | 3 | 3 | §E.1: los dos cambios de ADR-004 §3.1 se identifican y se declaran en el MEM para las notas del Bolt 028 |
| **La implementación en memoria es nueva y sin historial** (deuda declarada por ADR-004 §4) | 4 | 3 | Bordes cubiertos con evidencia explícita: cap superado, vencida en lectura concurrente, escritura concurrente, `GetOrCreate` con factory que falla |
| **Colisión de aspecto con el Bolt 014** en `SQLHelper.cs` y `SqlHelperParameterCache.cs` (dos Bolts, dos ejecutores, los mismos archivos) | 3 | 3 | §B.1 y §14 declaran la frontera: acá el **caché**; el proveedor, el tipado del interior, la regla del timeout por comando y el hazard de la conexión del llamador son del 014. Un cambio de aspecto se reporta, no se ejecuta "de paso" |
| **El contrato de configuración del Bolt 011 no está presente** (prerrequisito de orden) | 4 | 3 | §5/§15 con condición de parada; la alternativa prohibida (leer `App.config` o esconder el default) queda explícita para que no se tome por conveniencia |
| **El catálogo no publica una clave** (cap, o un TTL de región) | 3 | 3 | §D.1/§15: se pide al Bolt 011 (enmienda/SPEC revisada), nunca se esconde en el código; el nombre concreto del cap se propone en §14 para que la petición sea concreta |
| **El consumidor esperaba caché compartido entre instancias** (comportamiento de 1.4 en web) | 3 | 3 | Sin adaptador en 2.0.0 (ADR-004 §3.1): el cambio se declara en las notas y el naming del satélite queda definido para no improvisarlo; el NFR de alcance entre instancias sólo aplica a un adaptador futuro |
| **Regresión del camino de toda consulta** (la capa de datos es crítica) | 3 | 4 | La suite de caracterización del Bolt 001 corre **antes y después**; el comportamiento por defecto en memoria preserva el resultado observable de 1.4 (incluida la clave del caché de colecciones, que era por regla) |
| **Dependencia de terceros colada** ("caché lista" de la pila de extensiones) | 2 | 4 | ADR-004 §3.1 lo descarta explícitamente y el gate de dependencias de `Data` está en el umbral de "exactamente un provider" |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8603`/`CS8625` | 3 | 3 | §B.2 la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-9 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |
| **Un archivo del 1.4 depende de una pieza que otro Bolt debe portar primero** (frontera de secuencia conocida: el caso del Bolt 011, `MEM-261016-0053`) | 2 | 4 | El precedente queda **citado** en §17/§19: al ejecutar, si aparece esa dependencia, la salida es **parar y escalar la secuencia** y, si corresponde, **revisar este SPEC** — nunca invadir el Bolt ajeno (§15, G11) |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-004 y ADR-001; acá se aplican.
> Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) La clave del catálogo para el cap de entradas: este SPEC propone una y su publicación es del Bolt 011** | ADR-004 §3.1 exige el "cap de entradas **configurable**" y §3.2 fija la regla "toda región de caché con default propio **tiene su clave**", pero el catálogo de §3.2 **no nombra** la clave del cap. Se propone una clave bajo el prefijo del catálogo (`Cooperator:Cache:*`, con la forma de las existentes) y su **publicación** queda del lado del Bolt 011: si el revisor prefiere otro nombre o prefiere que el cap no sea configurable por catálogo, se ajusta en la aprobación —lo que **no** se negocia es esconder el default en el código |
| **(⚠ aprobador) Frontera de aspecto con el Bolt 014 sobre `SQLHelper.cs` y `SqlHelperParameterCache.cs`** | Los dos Bolts tocan los mismos archivos por motivos distintos: acá el **caché** (dónde y con qué TTL se guarda la metadata y el timeout); en el 014 el **proveedor**, el tipado del interior, la regla "todo comando con timeout explícito y acotado" y el hazard de abrir/cerrar la conexión del llamador (ADR-005 §3.2/§3.4). Se declara para que ningún ejecutor "aproveche" y haga el trabajo del otro: un cambio de aspecto se reporta |
| Las **regiones** son concretas y sus claves siguen el esquema documentado (`coop:{scope}:{rule}:{kind}`) | El ADR documenta el esquema pero no las claves de la capa; nombrarlas acá (y publicarlas junto al contrato) es lo que permite el inventario de AC-7 y la invalidación por clave sin prefijos |
| El **reloj** de la expiración es sustituible en las pruebas | Un test de TTL con esperas reales es lento y frágil (y "reproducible" sólo en apariencia); el proveedor de tiempo sustituible permite probar el vencimiento y el desalojo sin dormir el proceso, sin cambiar el comportamiento de producción |
| El **conteo interno** se expone para la verificación del NFR | Las dos métricas del NFR (total ≤ cap, 0 vencidas servidas) necesitan inspección; exponer el conteo a la prueba es la forma honesta de medirlo sin agregar instrumentación al producto |
| La implementación por defecto se ofrece como **tipo público** construible (contrato público + default público, ambos BCL) | El default es "para todos los hosts" (ADR-004 §3.1) y un host con contenedor propio necesita poder registrarlo explícitamente; además es la pieza sobre la que un futuro satélite se puede decorar. No contradice el principio de "implementación interna" de ADR-005 §3.3, que aplica al DAAB heredado de la capa de datos |
| El **cambio de claves internas** no se trata como ruptura | Las claves `Coop_DAL_*`/`Cooperator_DAL_SP_*` no eran contrato público (viven en el caché del host); la equivalencia funcional se verifica con la suite de caracterización. Se declara igual en las notas por transparencia |
| Los **dos cambios de comportamiento** se declaran sin intentar evitarlos | ADR-004 §3.1 los fija como consecuencia aceptada: el vencimiento en escritorio es el punto (menos memoria retenida y menos datos rancios) y el alcance compartido sólo aplica a un adaptador que en 2.0.0 no se publica. "Evitarlos" sería desobedecer la decisión |
| La caché **no** guarda la cadena de conexión | ADR-004 §3.3 elimina el `Dictionary` estático de connection strings y la resolución del valor es de la capa que lo consume; las regiones de este Bolt guardan colecciones, metadata de esquema y un timeout. Deja el control de AC-8 y del gate de secretos |
| El inventario de puntos de caché es entregable del MEM **y** documentación del paquete | Es la evidencia de que no queda ningún punto condicionado al host (criterio del Bolt) y, al mismo tiempo, la referencia para el Bolt 014 y para el mantenimiento: sin él, "todos los puntos" es una afirmación sin medición |
| **(⚠ aprobador) Política de nulabilidad del código aportado del 1.4: `#nullable disable` por archivo aportado que lo requiera** (revisión 2) | Es la trampa **ya medida** tres veces en esta ola (Bolts 003/007/009: 28 diagnósticos `CS86xx` en el modelo, `CS8618` en `BaseObject.cs(152,19)`/`DescriptionFieldEventArg.cs(48,16)`, y el aviso declarado del 009 —`MEM-261016-0107` §8.2, `MEM-261016-0209` §8.1, `MEM-261016-0227` §13—) y **este Bolt aporta/transforma archivos del 1.4**. El mecanismo es el **espejo exacto** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3: preserva la semántica del 1.4 (apaga el análisis, **no cambia el IL**), es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no toca el comportamiento observable**. Se **descartan**, declaradas, las alternativas de **anotar/corregir** el código aportado (transformación no ordenada y con riesgo de pisar el aspecto del Bolt 014) y de **diferir** el aporte (dejaría al Bolt sin la capa donde sustituir los puntos de caché). Se elige la opción que **evita una cuarta revisión material por el mismo motivo** |

---

## 15. Stop conditions

- **El contrato de configuración del Bolt 011 (y su catálogo de claves) no está en el árbol 2.0** al iniciar el
  V-Bounce —su SPEC ya está en disco, pero el prerrequisito es la **ejecución** de ese Bolt—: **parar y
  reportar**. La salida es una **enmienda del Bolt** (dependencia) o una decisión de orden del dueño, con la
  SPEC revisada si cambia el plan. **Nunca** leer `App.config`, la sección custom ni ninguna fuente propia, y
  **nunca** esconder el default en el código: es exactamente lo que el Bolt prohíbe.
- **El catálogo no publica una clave que un default necesita** (el cap, o el TTL de una región): parar y
  pedirla al Bolt 011. Un default en el código, aunque sea "documentado en un comentario", **no** es una salida.
- **Un punto de caché de la capa no puede resolverse por el contrato** (por ejemplo porque su clave necesita
  algo que el contrato no expresa): parar y reportar; la respuesta es del contrato (Bolt 011), no un camino
  paralelo de caché.
- **Aparece la necesidad de un mecanismo que el ADR descartó** (invalidación por prefijo, adaptador publicado,
  caché compartido por defecto, dependencia de terceros): parar. ADR-004 §3.1 lo descartó explícitamente y una
  conveniencia dentro de un Bolt no cambia el layout ni el presupuesto de dependencias (ADR-001 §3.4).
- **El cambio que se pide es del aspecto del Bolt 014** (proveedor, casts, tipado del interior, hazard de la
  conexión del llamador, regla del timeout por comando) o del Bolt 005 (caché de providers Lazy): parar y
  reportar. No se ejecuta el trabajo de otro Bolt (G11) ni se mezclan dos aspectos en una misma evidencia.
- **La suite de caracterización del Bolt 001 no está verde** antes del cambio: parar. Es la red que hace
  verificable la sustitución de los puntos de caché (el camino de toda consulta).
- **El árbol 2.0 no tiene el proyecto `Cooperator.Framework.Data`**: parar y reportar; este Bolt no crea la capa
  de datos.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.
- **Alguien extiende la autorización de §B.2**: poner la directiva `#nullable disable` en **código nuevo** (el
  contrato de caché, su implementación o las pruebas) o `<Nullable>disable</Nullable>` en un `.csproj`; o intentar
  resolver con esa misma autorización un `using` muerto **hacia un namespace que no existe en el árbol 2.0** que
  **no** provenga de un retiro propio. Parar y reportar: la autorización es **una** y está acotada en §B.2.
- **Al aportar aparece una dependencia de secuencia con otro Bolt** (un archivo del 1.4 que no compila porque la
  pieza que necesita la porta **otro** Bolt, como el caso que el Bolt 011 resolvió con su revisión de secuencia —
  `MEM-261016-0053`, `SPEC-261016-0018` §4/§6/C.2—): **parar y escalar la secuencia**. La salida es una decisión de
  orden del dueño y, si cambia el plan, una **revisión material de este SPEC**; **nunca** aportar ni transformar
  código de otro Bolt (G11).

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-004 §3.1/§3.2/§3.5, ADR-001 §3.4/§3.5/§5)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] La directiva de nulabilidad de **§B.2** aplicada **sólo** a los archivos aportados del 1.4 que la requieren, y verificada (lectura + build); ningún `.csproj` con `<Nullable>disable</Nullable>` y ningún `using` muerto sin resolver ni autorizado
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-013`](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-07/F-08 (caché atado a `System.Web`; corrección factual del caché de escritorio: en memoria por instancia y sin vencimiento) y §6 ítem 4.
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.1 (contrato y superficie mínima, default en memoria, TTL por entrada, expiración perezosa, cap con desalojo, `System.Web` fuera del framework, cambios de comportamiento), §3.2 (catálogo de claves, TTL por región, regla del default publicado), §3.5 (vencimiento de la excepción transitoria), §5 (NFRs *Cache memory bound* y *Config fail-fast*), §4 (deuda de la implementación nueva).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1/§3.5 (`Data` portable, sin paquete `-windows`/`-web`), §3.4 (un solo provider ADO.NET y política de dependencias), §5 (NFR de portabilidad y huella de dependencias).
- [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) — §3.4 (cruce: `SqlHelperParameterCache` usa `ICacheProvider` con su región y TTL; el hazard de la conexión del llamador) — **el resto de ADR-005 es del Bolt 014**.
- [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — el contrato de configuración y el catálogo de claves (prerrequisito de orden; su SPEC canónica es [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md), §5/§15).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y suite de caracterización (prerrequisito).
- [`US-000.BOLT-014`](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) · [`US-000.BOLT-005`](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — los Bolts con frontera declarada (proveedor y caché de providers) y el documento de notas que recibe los cambios de comportamiento.
- [`OQ-002`](../analysis/open-questions/OQ-002-soporte-system-web.md) — `ICacheProvider` con default en memoria para todos los hosts y adaptadores intercambiables; el adaptador web dedicado no es entregable.
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** · [`SPEC-261016-0037`](./SPEC-261016-0037-library-portable.md) **§B.2** · [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) **§E.3** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado por **revisión material** con su medición (`MEM-261016-0107` §8.2: 28 diagnósticos `CS86xx`; `MEM-261016-0209` §8.1: `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`; `MEM-261016-0227` §13: el aviso declarado del Bolt 009). Es el mecanismo que esta revisión **espeja**, **antes** de que el aporte de este Bolt lo requiera.
- **Precedente de la frontera de secuencia (citado sin cambio de alcance — revisión 2):** [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) **§4/§6/C.2** y [`MEM-261016-0053`](../memory/MEM-261016-0053-configuracion-portable.md) §8.1 — el caso en que un Bolt descubre, **al ejecutar**, que un archivo de su alcance sólo existe en su **forma ya migrada** cuando **otro** Bolt aporta la capa; la salida fue una **revisión material por secuencia** (nunca una decisión del ejecutor). Se cita porque la misma situación puede aparecer al aportar archivos de la capa `Data`: el camino existe y es el de §15 — **no** se especula acá con archivos concretos.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-013`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, estrategia de pruebas (incluidos los dobles del contrato de configuración y de la caché), gates, control de datos `internal` y condiciones de parada. Declara la dependencia de orden con el contrato de configuración del Bolt 011 (§5/§15) y la frontera de aspecto con el Bolt 014 sobre los dos archivos compartidos (§4/§14). `integration_test_plan: n/a` coherente con el Bolt. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Aclaración en la misma revisión (draft)** — el SPEC del Bolt 011 apareció en disco durante la redacción de esta ola: se agrega [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) a `prerequisites` y se nombran su archivo y su condición de parada en §5/§17 (antes se describía como "en redacción"). No cambia el plan, ni el inventario de fuentes gobernadas, ni la estrategia de pruebas. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16) PREVENTIVA**, cursada **antes** de que la trampa muerda y sin que exista todavía un V-Bounce de este Bolt: los Bolts que aportaron código legacy en esta ola (003/007/009) chocaron con la **nulabilidad** del árbol (`Nullable=enable` + `TreatWarningsAsErrors`, del Bolt 001) y cada uno necesitó su **revisión material** para autorizar la directiva —y el Bolt 022 la previno por la misma vía—, y **este Bolt aporta/transforma archivos del 1.4** (los de §B.1: `BaseGateway.cs`, `BaseLoader.cs`, `SQLHelper.cs`, `SqlHelperParameterCache.cs` y su cierre). Cambia **una sola cosa**: la **política de nulabilidad del código aportado del 1.4** (§B.2, nueva) —`#nullable disable` como primera línea útil **sólo** en los archivos aportados que **produzcan** `CS8618`/`CS8603`/`CS8625`, **jamás** en código nuevo (el contrato de caché, su implementación y las pruebas conservan la nulabilidad activa), **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**—, con el **límite** del `using` muerto hacia un namespace inexistente que no provenga de un retiro propio (parada y reporte, §15). Es el espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3. Se **cita** además el precedente de la frontera de secuencia del Bolt 011 (`MEM-261016-0053`) en §17/§19 y §15 —**sin** cambio de alcance y sin especular con archivos—, para que el ejecutor sepa que ese camino existe. Se actualizan §1, §4, §6 (nota de mecánica + §B.2), §7 (AC-9 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el contrato `ICacheProvider`, la implementación en memoria, los tres puntos de caché, el retiro del caché del host, los TTL/cap desde el catálogo, las fronteras con 011/014/005 y el control `internal` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

**Nota al aprobador — qué cambia la revisión 2, y por qué (material, G15/G16).** Es una revisión **preventiva**:
**no** responde a un bloqueo de este Bolt —no hay V-Bounce todavía— sino a la **trampa ya medida** en esta ola (los
Bolts **003, 007 y 009** chocaron con la nulabilidad del árbol al aportar código del 1.4 y cada uno necesitó su
revisión material; el **022** la previno por la misma vía), sabiendo que **este Bolt aporta/transforma archivos del
1.4**. Cambia **una sola cosa**:

1. **Política de nulabilidad del código aportado del 1.4 (§B.2, nueva).** `#nullable disable` como **primera línea
   útil** **sólo** de los archivos **aportados del 1.4** que **produzcan** `CS8618`/`CS8603`/`CS8625` —criterio
   **objetivo**: *archivo aportado + diagnóstico de nulabilidad*; **el build es la prueba**—, **jamás** en código
   nuevo (el contrato `ICacheProvider`, su implementación y las pruebas conservan la nulabilidad **activa**) y
   **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es el **espejo** de `SPEC-261016-0004`
   §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0035` §E.3 (los cuatro ya aprobados).
2. **Límite explícito (no autorizado):** un `using` muerto **hacia un namespace que no existe en el árbol** que
   **no** provenga de un retiro propio sigue siendo **parada y reporte** (§B.2, §15); lo que **sí** se retira con el
   retiro de la plataforma web y del caché del host (§C.1) es el `using` que queda **sin uso** por él.
3. **Precedente de secuencia, citado y sin cambio de alcance:** la frontera conocida con la capa `Data` tipada
   sobre el proveedor clásico —el caso que el **Bolt 011** resolvió con una **revisión de secuencia**
   (`MEM-261016-0053`, `SPEC-261016-0018` §4/§6/C.2)— queda **citada** en §17, §19 y §15 como el camino a seguir
   **si al ejecutar** aparece que un archivo del 1.4 depende de una pieza que **otro** Bolt debe portar primero:
   **parar y escalar** (y revisar este SPEC si cambia el plan), **nunca** invadir el Bolt ajeno. **No** se especula
   con archivos concretos.
4. **Lo que NO cambia: todo lo demás.** El contrato `ICacheProvider` y su superficie mínima, la implementación en
   memoria (TTL, expiración perezosa, cap con desalojo, concurrencia), los tres puntos de caché con su inventario,
   el retiro del caché del host y de la plataforma web, los TTL/cap desde el catálogo (con la clave del cap
   alineada al Bolt 011), las fronteras con los Bolts 011/014/005 y el control `internal` siguen **en vigor**. El
   `review` vuelve a vacío porque la revisión es material y se re-aprueba por el revisor.

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (datos / plataforma)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
