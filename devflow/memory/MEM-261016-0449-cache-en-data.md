---
id: "MEM-261016-0449"
title: "Caché en `Data` (SPEC rev.2) - V-Bounce N°2: el contrato `ICacheProvider`, su default en memoria acotado con vencimiento, la sustitución de los CUATRO puntos de caché y la cifra del cap; READY FOR REVIEW"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-013" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0019" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-004", "ADR-001", "ADR-005"] # ADR-004 §3.1/§3.2/§3.5 (contrato y superficie mínima, default en memoria con TTL/cap/desalojo, catálogo de claves, vencimiento de la excepción transitoria), §5 (NFRs Cache memory bound y Config fail-fast); ADR-001 §3.4/§5 (presupuesto de dependencias y portabilidad sin la plataforma web); ADR-005 §3.4 (cruce: SqlHelperParameterCache usa el contrato, sin tocar el hazard ni el descubrimiento — del Bolt 014)
manifest: "US-000.BOLT-013-cache-en-data.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T04:49:09-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0). El ejecutor NO se auto-aprueba.
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T04:53:14-03:00"
  decided_at: "2026-10-16T04:53:14-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°2 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0019` revisión 2 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — corrida hermética VERDE reproducida (`pwsh 2.0/build/build.ps1 -Scope Hermetic`): 21 proyectos sin advertencias, 135 arnés + 12 mapeo + 11 encriptor + 46 data (+35 del 013) + 83 caracterización = 287 casos, exit 0."
    - "Compliant — entregables verificados por lectura: `ICacheProvider` superficie mínima exacta (`TryGet`/`Set`/`GetOrCreate`/`Remove`, sin invalidación por prefijo); `InMemoryCacheProvider` (vencida **nunca** servida L85-92; **cap jamás superado** — sección crítica «hacer lugar antes de insertar» L125-134 + `MakeRoom` L174-212 dentro de `_writeGate`; reloj sustituible L49-57); `ResetCache()` = exactamente sus dos claves en `BaseGateway` (L317-318) y `BaseLoader` (L186-187); seam `protected virtual ICacheProvider CacheProvider => CacheRegions.Provider` en ambos (L60/L175); `CacheMaxEntries = 1024` en `ConfigurationKeys.cs` L153 + `configuration-keys.md`; **0** literales de catálogo en el código de la caché; `README-cache.md`."
    - "ADJUDICACIÓN (las 3 desviaciones) — ENDOSO: §8.1 el caso del timeout del 014 cambió de «cada uso» a «cada caché nueva» (coherente con la caché que este Bolt reintroduce por orden de su SPEC; **ninguna** aserción relajada); §8.2 se tocó `ConfigurationKeys.cs`/doc del 011 **por delegación explícita** de su constancia (`MEM-261016-0117` §8.1: la cifra es del 013, dueño de la caché), sólo la cifra, sin claves nuevas; §8.3 los puntos de colecciones verificados por código (no hay corrida de integración: `not-required`) con obligación registrada (§13.1)."
    - "ADJUDICACIÓN (cifra 1024 + TTL del timeout) — ENDOSO: el cap `1024` está delegado por `MEM-261016-0117` §8.1 y es holgado (por encima del número de colecciones/SPs, cada entrada acotada por `MaxObjectsInCacheForReadOnlyTable`=5000), una línea si el dueño prefiere otra; el TTL del timeout reusa `Cooperator:Cache:DefaultTtlSeconds` (3600) en vez de estrenar una tercera clave sin decisión (más reactivo que el espejo de 1 día del caché del host, sin default inventado)."
    - "ADJUDICACIÓN (pendiente del registro §13.2) — ACEPTO como obligación registrada del ciclo (**no** `changes_requested`): la fila de ADR-004 §3.1 del mapa (`pending` «`ICacheProvider` todavía no está en el árbol») ya es falsa, pero el SPEC-013 no lista el fragmento ni el mapa entre sus entregables y la contribución es distribuida: este Bolt aporta su paso siguiente listado (§13.2) + `README-cache.md` §4 + este MEM, y la **obligación explícita** —el fragmento `US-000.BOLT-013` + la actualización de esa fila— queda registrada con su dueño (el registro de migración del Bolt 028 / dueño del mapa)."
    - "Compliant — desviación de presupuesto 13 loops (guía 10) declarada con causa (§8.6, sin residuo, verde en el loop 12); DoD + MEM completo (fases A–E, AC-1…AC-9) + manifiesto válido; `1.4/**` intacto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0449 — Caché en `Data` (rev.2) — V-Bounce N°2 **con la entrega completa**

| Field | Value |
|-------|-------|
| **Bolt** | [US-000.BOLT-013](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — `non-functional`, `refactor`, `medium`, `internal`, `test_integration: not-required` |
| **SPEC** | [SPEC-261016-0019](../spec/SPEC-261016-0019-cache-en-data.md), **revisión 2** (`approved` 2026-10-16T03:17:26-03:00) |
| **V-Bounce** | **2** — el que retoma el Bolt tras la corrección de secuencia `014 → 013` (el N°1 quedó `blocked`) |
| **Precondición de §5 verificada** | `2.0/src/Cooperator.Framework.Data` **existe** con su superficie base y sus puntos de caché: los aportó el **Bolt 014** (`MEM-261016-0343` §1, `MEM-261016-0429` §9.7) y los dejó **sin caché**, con su «paso siguiente: el 013» escrito en cada punto |
| **Baseline** | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — sin cambios materiales (§8.5) |

---

## 1. Executive summary

El Bolt entrega lo que el N°1 no podía entregar —el N°1 se detuvo, correctamente, por la dependencia de secuencia
(§5/§15) y el revisor adjudicó **re-encolar el 013 después del 014** (`MEM-261016-0328` §13)—. Ahora la capa existe
y **este V-Bounce sustituye sus cuatro puntos de caché por el contrato**, en una sola pasada hasta el verde:

1. **El contrato** `ICacheProvider` (`Data/Caching/ICacheProvider.cs`): superficie mínima —`TryGet<T>`, `Set<T>`,
   `GetOrCreate<T>`, `Remove`—, claves del llamador, TTL por entrada y **sin** invalidación por prefijo.
2. **La implementación por defecto** `InMemoryCacheProvider`: **BCL puro** (sin dependencias de terceros),
   thread-safe, TTL por entrada con **expiración perezosa** (una vencida **nunca** se sirve: se descarta al
   leerla), **cap con desalojo** (primero las vencidas, después la de vencimiento más cercano) y **reloj
   sustituible** para probarla sin esperas. El cap se hace lugar **antes** de insertar, dentro de la sección
   crítica: el total almacenado **no supera el cap en ningún instante**, tampoco durante una escritura concurrente.
3. **Los cuatro puntos** de la capa pasan por el contrato con las **claves y TTL publicados**: colecciones de
   `BaseGateway` y de `BaseLoader`, **timeout** de comandos (`SQLHelper`) y **metadata de parámetros**
   (`SqlHelperParameterCache`). `ResetCache()` de gateway y loader remueve **exactamente sus dos claves**.
4. **El TTL por región y el cap salen del catálogo** del Bolt 011, con sus defaults **publicados** y sin ningún
   default escondido en el código; la **cifra del cap** (`1024`) la fija **este Bolt**, que es a quien el Bolt 011
   delegó el valor (`MEM-261016-0117` §8.1).
5. **El README del contrato** (`Data/Caching/README-cache.md`): superficie, esquema de claves, regiones con sus
   claves de catálogo, límites y las dos advertencias de comportamiento —documentación pública, no un comentario
   interno (AC-7)—.

Resultado medido: `pwsh 2.0\build\build.ps1 -Scope Hermetic` → **EXIT=0**, **0 advertencias** (tratadas como error)
sobre 21 proyectos, **287 casos verdes**: 135 arnés · 12 mapeo · 11 encriptor · **46 datos** · 83 caracterización.
La suite de `Data` pasó de 11 a **46 casos** (+35 de este Bolt).

Estado: **`ready_for_review`**. Pausa en `AITL-MEM-Approval`; el ejecutor no lo aprueba.

---

## 2. Implemented phases

| Fase de §6 | Estado | Evidencia |
|---|---|---|
| **A — El contrato y su implementación en memoria** | **implementada** ✔ | `Caching/{ICacheProvider,CacheKey,CacheRegions,InMemoryCacheProvider}.cs`; §9.2 (TTL, cap, vencimiento, concurrencia, `GetOrCreate`) |
| **B — Los cuatro puntos de caché de la capa** | **implementada** ✔ | §9.3 (claves, TTL y uso del contrato en los cuatro; `ResetCache()` acotado); frontera con el 014 respetada: no se tocó el proveedor, el tipado del interior, el hazard ni la regla del timeout por comando |
| **C — Retiro de la dependencia del host y de la plataforma web** | **implementada** ✔ (parte por el 014) | El retiro del `HttpContext`/`System.Web` lo ejecutó el 014 como **condición de compilación** (`MEM-261016-0429` §C.6); acá se verifica **0** en los seis archivos de la capa y el `csproj` sin la referencia web (§9.4) |
| **D — TTL por región y cap desde el catálogo** | **implementada** ✔ | §9.2/§9.5; la cifra del cap fijada en el catálogo (§7.1) |
| **E — Evidencia y cambios de comportamiento declarados** | **implementada** ✔ | `README-cache.md` §4 con las dos advertencias; el inventario de puntos y los cambios en este MEM (§13.2) para el Bolt 028 |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Data/Caching/ICacheProvider.cs` | El contrato de caché (superficie mínima, claves del llamador, TTL por entrada). |
| `2.0/src/Cooperator.Framework.Data/Caching/CacheKey.cs` | El esquema publicado `coop:{scope}:{rule}:{kind}` y las claves de las regiones de `Data`. |
| `2.0/src/Cooperator.Framework.Data/Caching/CacheRegions.cs` | Las regiones, su TTL y el cap **leídos del catálogo**; el proveedor del proceso (reemplazable). |
| `2.0/src/Cooperator.Framework.Data/Caching/InMemoryCacheProvider.cs` | La implementación en memoria (TTL, expiración perezosa, cap con desalojo, thread-safe, BCL puro, reloj sustituible). |
| `2.0/src/Cooperator.Framework.Data/Caching/README-cache.md` | El contrato **publicado**: superficie, claves, regiones y claves de catálogo, límites y advertencias. |
| `2.0/tests/Cooperator.Framework.Data.Tests/InMemoryCacheProviderTests.cs` | 18 casos: TTL por entrada, vencida no servida y descartada, cap exacto, desalojo (vencidas primero / más cercana), `GetOrCreate`, `Remove`, concurrencia repetida. |
| `2.0/tests/Cooperator.Framework.Data.Tests/CacheRegionsTests.cs` | 8 casos: TTL/cap del catálogo con doble, defaults publicados, error tipado sin fallback, ausencia de literales, cap del proveedor por defecto. |
| `2.0/tests/Cooperator.Framework.Data.Tests/CachePointTests.cs` | 9 casos: esquema de claves, superficie mínima del contrato (sin prefijos), los puntos estáticos con **doble de `ICacheProvider`**, el alcance de `ResetCache()` y los controles de ausencia. |
| `2.0/tests/Cooperator.Framework.Data.Tests/DataCachingCollection.cs` | Colección de xUnit sin paralelización: los casos tocan el proveedor del proceso y la configuración ambiental (estado de proceso). |

---

## 4. Files modified

| Path | Cambio |
|---|---|
| `2.0/src/Cooperator.Framework.Data/BaseGateway.cs` | Punto de colecciones por el contrato (`CacheProvider.GetOrCreate(CacheKey.CollectionAll(TableName), …)` con `CacheRegions.CollectionsTtl`); `ResetCache()` remueve sus dos claves; propiedad `protected virtual ICacheProvider CacheProvider` (seam). |
| `2.0/src/Cooperator.Framework.Data/BaseLoader.cs` | Ídem (`GetAll` sólo cachea cuando la tabla es read only o cacheable; el resto sigue leyendo la base) + el productor privado `ReadAllObjects`. |
| `2.0/src/Cooperator.Framework.Data/SQLHelper.cs` | `GetCommnandTimeOut()` pasa por el contrato con `CacheKey.CommandTimeout` y el TTL de su región; **su valor** sigue saliendo del catálogo. |
| `2.0/src/Cooperator.Framework.Data/SqlHelperParameterCache.cs` | `GetSpParameterSetInternal` guarda la metadata por el contrato (clave por SP **y** por el flag del parámetro de retorno, TTL de la región); **no** se tocó el descubrimiento ni el hazard de la conexión (Bolt 014 / ADR-005 §3.4). |
| `2.0/src/Cooperator.Framework.Core/Configuration/ConfigurationKeys.cs` | **La cifra del cap**: `Defaults.CacheMaxEntries = 1024` (§7.1 — el Bolt 011 delegó el valor en su propia constancia). |
| `2.0/docs/configuration-keys.md` | La fila de `Cooperator:Cache:MaxEntries` con su cifra y §2.1 con su procedencia. |
| `2.0/tests/Cooperator.Framework.Data.Tests/DataLayerContractTests.cs` | La anotación de colección y el caso del timeout, que ahora **estrena caché en cada fase** para seguir verificando la **resolución** del catálogo (§8.1). |
| `devflow/functional/bolts/US-000.BOLT-013-cache-en-data.md` | G11: toma desde `blocked` (fila **T06** de la línea de tiempo). |
| `devflow/metrics/bolts/US-000.BOLT-013-cache-en-data.json` | `v_bounces[]` += la entrada de este V-Bounce (`number: 2`, `spec_revision: 2`). |

---

## 5. Files deleted / 6. Files renamed

Ninguno. `1.4/**` no se tocó (§9.6).

---

## 7. Implementation decisions

### 7.1 La cifra del cap: `1024` entradas, fijada por su dueño

ADR-004 §3.1 exige el cap «**configurable**» con desalojo pero **no lo cuantifica**, y el catálogo del Bolt 011
publicó la **clave** declarando que la **cifra** es del dueño de la caché (`MEM-261016-0117` §8.1: «el Bolt 013
**pide la clave** a este Bolt y publica la **cifra** al aportar la caché, que es su entregable»). La cifra es
`1024`: acota lo que 1.4 **no** acotaba —su caché de colecciones no expiraba ni tenía límite— y queda
holgadamente por encima del número de colecciones y stored procedures que la capa cachea en un proceso real,
mientras cada entrada está a su vez acotada por el límite por colección de 1.4
(`MaxObjectsInCacheForReadOnlyTable` = 5000). Vive en el **catálogo** (`Defaults.CacheMaxEntries`) y viaja al
proveedor por constructor: en el código de la caché no hay literal (probado, §9.2). Es **una línea** si el
revisor prefiere otra cifra.

### 7.2 El TTL de la región del timeout reusa el genérico publicado

ADR-004 §3.1 enumera los defaults por región que 1.4 tenía: colecciones **1 h** y metadata de parámetros
**1 día**. La región del timeout **no** tiene default propio declarado, así que el punto reusa el TTL genérico
**publicado** (`Cooperator:Cache:DefaultTtlSeconds`) en vez de estrenar una tercera clave con un tercer valor que
ninguna decisión ordena. El efecto es que el timeout se re-resuelve del catálogo cada hora (en 1.4 el espejo en el
caché del host vivía 1 día): más reactivo, sin default inventado.

### 7.3 El cap no se supera **en ningún instante**, no sólo al final

La escritura hace lugar —desalojando primero las vencidas y, si no alcanza, la de vencimiento más cercano—
**antes** de insertar y dentro de la misma sección crítica. Con eso el invariante del NFR *Cache memory bound*
vale en todo momento observable, incluida una escritura concurrente; el caso de concurrencia lo comprueba en cada
escritura y con la corrida repetida cinco veces.

### 7.4 La clave de la metadata varía con el flag del parámetro de retorno

La clave publicada por el inventario de §6/B.1 es `coop:data:sp-parameters:{sp}:metadata` para el caso común. El
descubrimiento **cambia la forma del valor** cuando se pide el parámetro de retorno, así que esa variante usa
`…:metadata-with-return-value` en lugar de compartir clave y servir una forma equivocada. Está documentado en el
README y probado con el doble.

### 7.5 El interior se ejerce por reflexión y los puntos estáticos con un doble del contrato

`SqlHelper` y `SqlHelperParameterCache` son `internal` y el ensamblado está **firmado** (ADR-001 §3.3:
`InternalsVisibleTo` exigiría la clave pública), así que sus puntos se ejercen por reflexión —igual que en el Bolt
014— y con un **doble de `ICacheProvider`**: el doble entrega un valor sembrado y **cuenta** si tuvo que invocar la
fábrica. Que el valor entregado sea el del doble —y que la fábrica **no** haya corrido— es la prueba de que el
punto usa **el contrato** y no una implementación concreta (AC-1) y de que el camino a la base no se toca.

### 7.6 El provider del proceso es reemplazable, y por eso hay una colección de pruebas

`CacheRegions.Provider` tiene get/set: los hosts con contenedor propio pueden registrar el suyo (ADR-004 §3.1:
default en memoria para todos) y las pruebas pueden inyectar dobles. Como es estado **de proceso**, y también lo
es `CooperatorSettings.Current`, los casos que lo tocan viven en la colección `DataCaching` con paralelización
deshabilitada: sin eso el verde sería flaky (mismo criterio que el Bolt 011).

---

## 8. Deviations and assumptions

### 8.1 El caso del timeout del Bolt 014 cambió de forma (declarado)

El Bolt 014 dejó un caso que verificaba que el timeout se leía del catálogo **en cada uso** (era su comportamiento
entonces: la capa no cacheaba). Este Bolt **reintroduce la caché** en ese punto por orden de su SPEC (§6/B.1), así
que el caso ahora **estrena caché en cada fase** y sigue verificando lo que importa: que la resolución sale del
catálogo y que un valor inválido propaga el error tipado **sin** fallback. No se relajó ninguna aserción: se
cambió de «cada uso» a «cada caché nueva», que es el comportamiento ordenado por este Bolt. Queda a la vista del
revisor porque toca un archivo de pruebas de otro Bolt ya entregado.

### 8.2 Se tocó un archivo del Bolt 011 (el catálogo), con autorización expresa

`ConfigurationKeys.cs` y `2.0/docs/configuration-keys.md` son del Bolt 011 y se modifican **por delegación
explícita** de su propia constancia: `MEM-261016-0117` §8.1 dice que el Bolt 011 publica la **clave** y que «el
valor del default es del Bolt 013 —dueño de la caché—, que es quien lo publica en el catálogo al aportar la
caché». La clave y el tipo ya estaban publicados; acá se publica **la cifra**. No se renombró ni se agregó ninguna
otra clave.

### 8.3 Los puntos de colecciones se verifican sobre el código, no con una conexión

Los puntos de `BaseGateway`/`BaseLoader` viven en clases genéricas cuyo productor necesita una **conexión real**, y
el Bolt declara `test_integration: not-required` con plan `n/a`: no hay una corrida de integración donde
ejercitarlos. Los dos puntos **estáticos** (timeout y metadata) sí se ejercen con el doble del contrato, y de los
de colecciones se verifica sobre el código —que el uso del contrato con la clave y el TTL publicados está ahí, y
que `ResetCache()` remueve **exactamente** sus dos claves y ninguna otra (AC-6)—. Queda registrado como obligación
(§13.1) para que el hueco no se lea como cobertura completa.

### 8.4 La nulabilidad del aporte (§B.2): ninguna directiva nueva

El SPEC autoriza `#nullable disable` para archivos **aportados del 1.4** que produzcan diagnósticos. Este V-Bounce
**no agregó ninguna directiva**: los cuatro archivos aportados conservan la suya (puesta por el Bolt 014) y el
**código nuevo** de la caché conserva la nulabilidad **activa** —el build con advertencias como error no emitió un
solo diagnóstico de nulabilidad—. Ningún `.csproj` declara `<Nullable>disable</Nullable>`, y no quedó ningún
`using` muerto: los `using` nuevos son los del namespace de la caché y no se retiró ningún uso.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

SPEC-261016-0019 `approved` con **`revision: 2`** y `review.decision: approved`
(`2026-10-16T03:17:26-03:00`); el Bolt estaba `blocked` y se tomó (G11, T06); ADR-004/ADR-001/ADR-005 siguen
`accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` queda intacto. La precondición de
§5 se cumple: la capa `Data` y sus puntos existen (los aportó el 014; `MEM-261016-0343` §1, `MEM-261016-0429` §9.7).

### 8.6 Presupuesto de turnos (declarado)

La guía era **10 loops**; se usaron **13** antes del cierre. El gasto se explica, no se justifica: (a) el
reconocimiento necesario —SPEC de 884 líneas con seis fases, la constancia del N°1 y el inventario del 1.4—;
(b) **tres** ciclos de corrección de compilación (dos de nulabilidad en mi propio código nuevo —el `out` anotado
del contrato y el pasaje a `Entry`— y uno de atributos en el doble de prueba) que sólo el build podía revelar;
(c) una corrección de mi propio andamiaje en la suite (la ruta de los archivos nuevos). El verde se alcanzó en el
loop 12 y a partir de ahí no se consumió ninguno antes de la pausa. La condición de parada del SPEC (verde dentro
del presupuesto) **no** se activó: la unidad cerró **en verde**.

---

## 9. Verification evidence

### 9.1 La suite hermética del árbol: VERDE

```
build sin advertencias (tratadas como error): 21 proyectos
Passed!  - Failed: 0, Passed: 135  - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  12  - Cooperator.Framework.Core.Mapping.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  11  - ConnectionStringEncriptor.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  46  - Cooperator.Framework.Data.Tests.dll (net10.0)   ← 11 del 014 + 35 del 013
Passed!  - Failed: 0, Passed:  83  - Cooperator.Framework.Characterization.Tests.exe (net48)
Resultado: ok (codigo de salida 0)
```

### 9.2 El NFR y el contrato (AC-3, AC-4, AC-5)

`InMemoryCacheProviderTests` (18) y `CacheRegionsTests` (8): TTL por entrada y por región; vencida **no servida** y
descartada al leer; cap 1 / N / N+1 / 2N y «cap muy grande»; desalojo de vencidas primero y de la más cercana
después; escrituras concurrentes (8 escritores × 150 escrituras, cinco corridas) con el invariante verificado en
cada escritura y **0** excepciones; `GetOrCreate` que crea una vez, recrea tras vencer y propaga el fallo **sin**
cachearlo; `Remove` que no toca las claves ajenas; TTL/cap del catálogo con doble y error tipado **sin** fallback;
y la ausencia de literales del catálogo en el código de la caché.

### 9.3 Los cuatro puntos (AC-1, AC-6, AC-7)

`CachePointTests` (9): el esquema publicado clave por clave; la superficie del contrato es **exactamente** las
cuatro operaciones y **sin** nada que nombre prefijos; el punto del timeout y el de metadata se ejercen con el
doble del contrato (valor entregado + fábrica **no** invocada + clave y TTL registrados); la clave de la metadata
varía con el flag del parámetro de retorno; los dos puntos de colecciones usan el contrato con su clave y su TTL,
y `ResetCache()` remueve **exactamente** sus dos claves (dos `Remove` por archivo, ningún barrido).

### 9.4 AC-2 y AC-8: un solo camino, sin la plataforma web, sin dependencias nuevas

Los seis archivos de la capa tienen **0** `HttpContext`, **0** `System.Web` y **0** claves de 1.4 (`Coop_DAL_*`,
`Cooperator_DAL_SP_*`) —tokens **ensamblados** en la prueba—; el `csproj` de `Data` no referencia la plataforma
web y conserva **exactamente un** proveedor ADO.NET (`Microsoft.Data.SqlClient`), sin ninguna dependencia nueva
(la caché es BCL puro).

### 9.5 Los TTL y el cap, desde el catálogo

`Cooperator:Cache:DefaultTtlSeconds` (**3600**) y `Cooperator:Cache:ParameterMetadataTtlSeconds` (**86400**)
gobiernan sus regiones; el cap `Cooperator:Cache:MaxEntries` tiene ahora su cifra publicada (**`1024`**) en
`ConfigurationKeys.Defaults` y en `2.0/docs/configuration-keys.md`. El proveedor por defecto se construye con el
cap del catálogo y **nada** en el código de la caché contiene un literal de esos valores (probado).

### 9.6 `1.4/**` intacto y rutas propias

Escrituras de este V-Bounce: `2.0/src/Cooperator.Framework.Data/**` (los cuatro archivos de la capa + el
namespace `Caching/` nuevo), `2.0/src/Cooperator.Framework.Core/Configuration/ConfigurationKeys.cs` (§8.2),
`2.0/docs/configuration-keys.md`, `2.0/tests/Cooperator.Framework.Data.Tests/**`,
`devflow/functional/bolts/US-000.BOLT-013-cache-en-data.md` (la toma), `devflow/metrics/bolts/…json` y este MEM.
Ninguna otra: **no** se tocó `1.4/**`, ni archivos del Bolt 014 (salvo el caso de prueba declarado en §8.1), ni del
Bolt 005, ni el registro de rupturas.

### 9.7 Gates (§9 del SPEC)

`pass`: Build/lint/type checks · Unit/component tests · Consumer-side contract tests (dobles de
`ICooperatorConfiguration` y de `ICacheProvider`) · SAST (código propio, sin copia de terceros) · dependency
scanning (un proveedor, sin dependencias nuevas) · secret-leak (ninguna cadena de conexión en caché, en código ni
en pruebas; las regiones guardan colecciones, metadata y un timeout) · hallucination lint (cada clave y default
trazable al inventario de §B.1 y al catálogo del Bolt 011) · IP/license (código propio) · PII/DLP (`internal`:
colecciones del modelo y metadata de esquema) · dependency-confusion (0 paquetes nuevos) · test-first evidence (los
casos del NFR se escribieron desde ADR-004 §5 y el inventario se tomó del código de 1.4 antes de sustituir) ·
behavioral reproducibility (reloj sustituible, corrida repetida, sin esperas) · manifest validation.
`n/a` **con razón**: OWASP (sin superficie expuesta) · prompt-injection (no se interpreta contenido externo) · y
todas las filas de nivel *Integration test*, porque el plan es `n/a` (Bolt `not-required`) — la corrida real de la
capa de datos es del Bolt 014.

---

## 10. Manual interventions

Ninguna intervención humana durante este V-Bounce. La decisión humana vigente es la adjudicación del
`AITL-MEM-Approval` del N°1 (`MEM-261016-0328` §13: **re-encolar el 013 después del 014**), que este V-Bounce
ejecuta. Sin operaciones de git (G34).

---

## 11. Evidence links

- **SPEC ejecutado:** [SPEC-261016-0019](../spec/SPEC-261016-0019-cache-en-data.md) — revisión **2**: §4 (fronteras),
  §5 (precondición), §6/A–E, §7 (AC-1…AC-9), §8.1/§8.2, §9, §14, §15.
- **Constancias del mismo Bolt:** [MEM-261016-0328](MEM-261016-0328-cache-en-data.md) (N°1, el bloqueo por
  secuencia y su adjudicación) · este MEM (N°2).
- **Precondición cumplida:** [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) y sus MEM
  (`MEM-261016-0343` §1: la capa ausente; `MEM-261016-0429` §9.7: la capa aportada con los cuatro puntos sin caché
  y la constancia que declara «paso siguiente: el 013»).
- **El catálogo y su delegación:** `MEM-261016-0117` §8.1 (la clave publicada y la cifra como entregable del 013) ·
  `2.0/docs/configuration-keys.md` §2.1.
- **Frontera respetada:** ADR-005 §3.4 y `MEM-261016-0429` §7 (el descubrimiento de parámetros y el hazard de la
  conexión del llamador son del Bolt 014: **no** se tocaron).
- **Contrato publicado:** `2.0/src/Cooperator.Framework.Data/Caching/README-cache.md`.

---

## 12. Metrics

| Metric | Value |
|---|---|
| V-Bounce | 2 (de US-000.BOLT-013) — **`ready_for_review`** |
| SPEC ejecutado | SPEC-261016-0019 revisión **2** |
| Fases implementadas | **5 de 5** (A–E; C completada por el 014 y verificada acá) |
| Archivos de producto creados / modificados | 5 (4 fuentes + README) / 6 (4 de la capa + catálogo + documento) |
| Archivos de prueba creados / modificados | 4 / 1 |
| Casos de la suite de `Data` | **46** (11 del 014 + **35** de este Bolt) |
| Suite hermética del árbol | **287 verdes** (135 + 12 + 11 + 46 + 83), exit 0, 21 proyectos sin advertencias |
| Loops usados | 13 (guía 10 — §8.6) |

---

## 13. Pending items and stubs

1. **Verificación conductual de los puntos de colecciones** (obligación declarada, §8.3): requieren una conexión
   real y el Bolt es `not-required` de integración; hoy se verifica su uso del contrato, su clave, su TTL y el
   alcance de `ResetCache()` sobre el código, y los dos puntos estáticos con el doble. Dueño: el ciclo que primero
   ejercite `GetAll` de un gateway/loader contra una base (el ciclo de integración del Bolt 014 sirve).
2. **Las declaraciones para las notas de migración** (AC-8, §6/E.1) quedan en este MEM y en `README-cache.md` §4:
   (a) en escritorio el caché de colecciones **pasa a tener TTL** (antes vivía para siempre) y ahora está acotado;
   (b) con un adaptador compartido —que en 2.0.0 **no** se publica— el alcance pasaría a ser entre instancias;
   (c) desaparece el espejo en el caché del host. **Pendiente para el registro del Bolt 028**: la fila de
   ADR-004 §3.1 del mapa (`2.0/docs/api-removals-decisions.json`) declaraba que «el `ICacheProvider` de `Data`
   todavía no está en el árbol»: eso **ya es falso** con esta entrega. No se tocó el mapa ni se agregó fragmento en
   este V-Bounce —el SPEC de este Bolt no lo lista entre sus entregables y tocar el mapa exige el fragmento
   correspondiente (su control de cobertura es estricto)—, así que queda registrado como el paso siguiente
   concreto, con su dueño: la contribución del fragmento `US-000.BOLT-013` y la actualización de esa fila.
3. **La integración real de la capa de datos** sigue siendo del Bolt 014 (su ciclo), incluido el smoke de conexión.
4. **La cifra del cap** es de este Bolt y queda a la vista del revisor: cambiarla es una línea en el catálogo
   (§7.1).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto (`number: 2`, `spec_revision: 2`,
  `ready_for_review`) + el árbol entregado: el contrato y su implementación, los cuatro puntos sustituidos, el
  README del contrato y 35 casos nuevos, con la suite hermética en verde (§9.1).
- **Qué se pide decidir:** (1) la adjudicación de la entrega y de sus tres declaraciones (§8.1 el caso del 014,
  §8.2 el archivo del 011, §8.3 el hueco de cobertura de los puntos de colecciones); (2) la **cifra del cap**
  (§7.1), que es de este Bolt por delegación del 011; (3) el TTL de la región del timeout (§7.2) y el paso
  siguiente del registro (§13.2).
