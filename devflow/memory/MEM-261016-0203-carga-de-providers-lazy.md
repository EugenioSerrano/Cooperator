---
id: "MEM-261016-0203"
title: "Carga de providers Lazy (SPEC rev.1): nombres calificados, resolución del host, hook, caché thread-safe por nombre calificado y cuadro de diagnósticos tipados"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-005" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0015" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-006", "ADR-001", "ADR-008", "ADR-004"] # ADR-006 §3.1–§3.4/§5 (la carga), ADR-001 §3.4/§3.6 (cero dependencias, el slot), ADR-008 §3.1/§3.2 (rupturas y jerarquía), ADR-004 §3.2 (el catálogo y el contrato, consumidos)
manifest: "US-000.BOLT-005-carga-de-providers-lazy.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T02:03:34-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:06:36-03:00"
  decided_at: "2026-10-16T02:06:36-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0015` revisión 1 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; grant `approves: [SPEC, MEM]`. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **85/85 harness + 83/83 caracterización, exit 0, 16 proyectos**. Verificado en el árbol: `Core/LazyLoad/**` con los 14 archivos (ProviderName/ProviderResolution/ProviderResolver/IAssemblyProvider/ProviderCache/ProviderStartupValidator/LazyProviderFactory/ILazyProvider + 6 excepciones); `api-removals/US-000.BOLT-005.json` con **5 entradas** (2 sobrecargas retiradas, FactoryCache, sondeo implícito, GetForEntity agregado)."
    - "Compliant — NFR `no-implicit-assembly-loading` con control NO VACUO (solidez confirmada): grep sobre `2.0/src/**/*.cs` de `LoadFrom|LoadFile` da **1 match y es un doc-comment** en `ProviderResolver.cs` L17 («no hay LoadFrom/LoadFile»), 0 usos reales; el **1.4 sí las tiene** (`Factories.cs` L70 `Assembly.LoadFrom(...)` real) — de modo que el control detectaría una reintroducción. El ensamblado no-declarado falla por defecto con nombre+causa y el hook lo obtiene."
    - "ADJUDICACIÓN 1 (ILazyProvider aportado verbatim, §8.1) — ACEPTABLE: el SPEC §4 declara el contrato **fuera de alcance para modificar** y ADR-001 §3.6 asigna a este Bolt el **slot** de la carga; la lectura de §15/§5 «no crea la capa base» como «no porta la capa base **más allá de su slot**» es correcta — no se aportó BaseObject/BaseObjectList/IGateway/SecurityRights ni ningún otro tipo del núcleo, sólo el contrato y su cierre (`IUniqueIdentifiable`), verbatim. Es secuencia, no un desvío de alcance."
    - "ADJUDICACIÓN 2 (alcance del gate «Test-first evidence», §8.2) — CORRECTO: los casos negativos derivan del **cuadro normativo** de ADR-006 §3.3 (cada fila con su contenido obligatorio), no de la implementación; el aporte **nace retirado** (en una 2.0 sin superficie vieja) y la retirada se mide **contra la línea base 1.4** vía el gate — no se declara un RED que no ocurrió—; y el control del NFR es **no vacuo** (el 1.4 sí tiene `LoadFrom`). Lectura correcta para un refactor."
    - "Compliant — DoD: herméticos verdes (168 = 85 + 83); gates aplicables `pass`; el cruce del gate de API **pasa** (5 CP0002 explicadas — las 2 retiradas de este Bolt + CreateExcelFile del 006 + GetForEntity agregado + SendMailViaCDONTS del 011 — y **0 sin explicación**); `Core` con 0 dependencias de terceros; MEM completo; manifiesto válido."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0203 — Carga de providers Lazy: nombres calificados, hook, caché y diagnósticos tipados

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-005](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) — `type: non-functional`, `risk_class: medium`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0015](../spec/SPEC-261016-0015-carga-de-providers-lazy.md), **revisión 1** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T00:19:59-03:00`) |
| **V-Bounce**    | **1** (el primer V-Bounce de este Bolt) |
| **ADRs**        | [ADR-006](../adrs/ADR-006-carga-de-ensamblados.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se reescribió la **carga de providers Lazy** de `Cooperator.Framework.Core` según ADR-006: los providers se
resuelven por **nombre explícito** —ensamblado en su forma simple de *display name* + **nombre completo del tipo
calificado por namespace**— por la **vía natural del host**, con un **hook** (`IAssemblyProvider`) que el host
registra **una vez** y que tiene precedencia; la **caché** pasó a ser **thread-safe**, con **clave por nombre
calificado** (el par estructurado, libre de la colisión de la concatenación con puntos), **una instancia por
provider y por proceso**, **reinicio explícito** y **sin envenenarse** ante un fallo; **cada** causa del cuadro de
ADR-006 §3.3 produce su **excepción tipada** con la clave de configuración y los nombres intentados; y se entregó la
**validación de arranque opt-in** que reporta **todos** los fallos en una sola pasada. El retiro es el del **camino
de carga**: se eliminó la sobrecarga con `Assembly.LoadFrom` (la que convertía un valor de configuración en
ejecución de código desde una ruta arbitraria) y la construcción por nombre simple con binder por defecto, y la
caché de clave concatenada desapareció con `FactoryCache`; la construcción **tipada** que no carga por nombre se
conservó, porque ningún ADR ordena retirarla. La verificación incluye la **medición dura del NFR**: hay un
ensamblado de prueba con un provider válido que **existe en el despliegue** (en una subcarpeta) pero **no está
declarado** como dependencia —el patrón "copiar DLLs a una carpeta" que el producto fomentaba—, y el camino por
defecto **falla** con el nombre intentado y la causa del runtime mientras el **hook** del host, con su propio
`AssemblyLoadContext`, **sí lo obtiene**. El árbol queda verde de punta a punta —16 proyectos sin una sola
advertencia y **85/85** en el arnés (26 casos nuevos) más **83/83** de caracterización—, con `1.4/**` intacto y
`Core` con **0 dependencias de terceros**.

---

## 2. Implemented phases

### 2.1 Fase A — Nombres explícitos y resolución por defecto del host

| Paso de §6/A | Qué se hizo | Estado |
|---|---|---|
| A.1 El nombre calificado | `LazyLoad/ProviderName.cs`: el destino del provider (ensamblado simple + tipo completo calificado), con **validación estructural honesta** (ambos obligatorios y no vacíos; el tipo debe estar calificado por namespace; versión/cultura/token se aceptan **como documentación** y no cambian la resolución) y con **igualdad estructural** sobre el par, que es la clave de la caché. `LazyLoad/ProviderResolution.cs`: el **punto de fijación única** del hook y el acceso a la estrategia vigente. | hecho |
| A.2 La resolución por defecto | `LazyLoad/ProviderResolver.cs`: sin hook, `Assembly.Load(new AssemblyName(<nombre simple>))` + tipo por **nombre completo**, delegando en el ALC por defecto; **sin** sondeo de directorios, **sin** `LoadFrom`/`LoadFile`, **sin** búsqueda por nombre simple y **sin** rutas. La compatibilidad (implementa `ILazyProvider`, no abstracto/interfaz, constructor público sin parámetros) se verifica **antes** de construir. `LazyLoad/IAssemblyProvider.cs`: el hook del host. | hecho |
| A.3 La fachada | `LazyLoad/LazyProviderFactory.cs`: conserva la entrada por **nombre explícito** que usa el código generado (`Get(assemblyName, className)`) y **agrega** el **camino por entidad** (`GetForEntity(entity)`), que lee el catálogo `Cooperator:LazyLoad:Providers:<entidad>:Assembly|:Class` mediante el contrato de configuración (ADR-004 §3.2). Ninguna entrada acepta rutas. | hecho |

### 2.2 Fase B — La caché de providers

`LazyLoad/ProviderCache.cs`: `ConcurrentDictionary<ProviderName, Lazy<ILazyProvider>>` con publicación exclusiva
—la resolución corre **dentro** del `Lazy`, de modo que N hilos construyen **una sola** instancia— y **sin cachear
fallos**: si la resolución lanza, la entrada se descarta y el siguiente intento vuelve a resolver. `ProviderCache.Reset()`
es el reinicio explícito. **`Factory/Factories.cs`** se aportó en su forma retirada: el `FactoryCache` de clave
concatenada **deja de existir** y la construcción tipada que **no** carga por nombre (`CreateObject(Type)`,
`CreateObject<T>()`) se conserva.

### 2.3 Fase C — El cuadro de diagnósticos tipados

`LazyLoad/Exceptions/`: las seis excepciones del cuadro (las cinco de ADR-006 §3.3 más el agregado de la
validación), todas **derivadas de `CoreException`**, con constructores públicos, **sin `[Serializable]` ni
`SerializationInfo`** (nacen POCOs, ADR-008 §3.2) y con el **contenido obligatorio** de cada fila: la entidad y la
clave esperada; el nombre de ensamblado intentado, si hay hook y la causa subyacente; el tipo y el ensamblado
resuelto más los tipos que **sí** implementan `ILazyProvider`; el motivo de incompatibilidad (los tres del
contrato, como constantes); y que el fallo **vino del hook** con la excepción original. **Sin fallos silenciosos**:
nunca `null`, nunca "probar con otro nombre", nunca un provider por defecto sin decirlo.

### 2.4 Fase D — Validación de arranque opt-in

`LazyLoad/ProviderStartupValidator.cs`: `Validate(entities)` resuelve **todos** los providers indicados en **una
sola pasada** y devuelve el **listado completo** de fallos (no corta en el primero), y `ValidateOrThrow(entities)`
lanza el **agregado** `ProviderValidationException` con todos ellos. Es **opt-in** (una app que no la invoca no la
ejecuta) y su **entrada es explícita** (una entidad a la vez), porque el contrato de configuración de ADR-004 §3.2
es de lectura por clave y no expone enumeración del catálogo —la decisión ⚠ que el SPEC aprobó—. La validación usa
el **mismo camino** que el runtime, así que una pasada limpia deja los providers ya en la caché.

### 2.5 Fase E — Retiro de la superficie prohibida y evidencia

| Elemento de §E.1 | Estado |
|---|---|
| La sobrecarga `CreateObject(assemblyPath, assemblyName, className)` con `Assembly.LoadFrom` | **Retirada** (no existe en el aporte) |
| El sondeo implícito por directorio base | **No reproducido** (el ALC moderno no lo hace y ADR-006 lo descarta) |
| El mecanismo de la carpeta `CooperatorAssemblies` en runtime | **No existe** como camino de resolución |
| La construcción por nombre simple sin calificar y el `CreateInstance(className, true)` con binder por defecto | **Retirada**, reemplazada por la resolución tipada de §A.2 |
| La caché de instancias por clave concatenada (`FactoryCache`) | **Retirada**, reemplazada por `ProviderCache` |

**Evidencia:** el fragmento de rupturas `2.0/docs/api-removals/US-000.BOLT-005.json` (5 entradas con su reemplazo,
destinadas a las notas de migración del Bolt 028), el **reporte del gate** de compatibilidad de API del Bolt 001
sobre `Core` (§9.5) y la **medición del NFR** (§9.6).

---

## 3. Files created

### 3.1 El subsistema de carga (`2.0/src/Cooperator.Framework.Core/`) — 16 archivos

| Path | Líneas | Propósito |
|---|---:|---|
| `LazyLoad/ProviderName.cs` | 124 | El nombre calificado: ensamblado (forma simple, con la versión/token aceptados como documentación) + tipo completo calificado por namespace; validación estructural e igualdad estructural (clave de la caché, sin la colisión de la concatenación). |
| `LazyLoad/ProviderResolution.cs` | 82 | El punto de **fijación única** del hook del host (registro al arranque, un segundo registro rechazado) y el acceso a la estrategia vigente; el registro reinicia la caché. |
| `LazyLoad/ProviderResolver.cs` | 146 | La resolución: despacho al hook cuando está registrado, y la **vía por defecto** (`Assembly.Load` del nombre simple + tipo por nombre completo + verificación de compatibilidad **antes** de construir) con sus diagnósticos tipados. |
| `LazyLoad/IAssemblyProvider.cs` | 28 | El **hook del host**: resuelve con su ALC, su carpeta de plugins o su contenedor de DI y devuelve la instancia; es **su** política de confianza, fuera del framework. |
| `LazyLoad/ProviderCache.cs` | 76 | La caché thread-safe por nombre calificado: una instancia por provider y por proceso, reinicio explícito y **sin cachear fallos**. |
| `LazyLoad/ProviderStartupValidator.cs` | 76 | La validación de arranque **opt-in** en una sola pasada, con el listado completo de fallos y su variante estricta. |
| `LazyLoad/LazyProviderFactory.cs` | 78 | La fachada: entrada por **nombre explícito** (la del código generado) y **camino por entidad** con el catálogo de ADR-004 §3.2 (donde la ausencia de configuración tiene diagnóstico propio). |
| `LazyLoad/Exceptions/ProviderNotConfiguredException.cs` | 68 | Primera fila del cuadro: la **entidad** sin provider configurado + la **clave esperada** del catálogo. |
| `LazyLoad/Exceptions/ProviderAssemblyNotFoundException.cs` | 76 | Segunda fila: el **nombre de ensamblado intentado** + si hay **hook** + la **causa subyacente** del runtime. |
| `LazyLoad/Exceptions/ProviderTypeNotFoundException.cs` | 89 | Tercera fila: el **tipo** y el **ensamblado resuelto** + los tipos que **sí** implementan `ILazyProvider`. |
| `LazyLoad/Exceptions/ProviderTypeIncompatibleException.cs` | 77 | Cuarta fila: el **motivo** (los tres del contrato, como constantes). |
| `LazyLoad/Exceptions/ProviderResolutionFailedException.cs` | 77 | Quinta fila: que el fallo **vino del hook** + la **excepción original** del host. |
| `LazyLoad/Exceptions/ProviderValidationException.cs` | 66 | El **agregado** de la validación: todos los fallos tipados con su causa, en un solo objeto capturable (con `ProviderValidationFailure`). |
| `LazyLoad/ILazyProvider.cs` | 57 | **El contrato del provider**, aportado **verbatim** del 1.4 (AAPORTE: es el slot de este Bolt y el SPEC §4 prohíbe modificarlo — §8.1). |
| `IUniqueIdentifiable.cs` | 48 | **Cierre mínimo** del contrato: el tipo del parámetro de `ILazyProvider.GetEntity`, aportado **verbatim** del 1.4 (§8.1). |
| `Factory/Factories.cs` | 72 | **El aporte en su forma retirada**: `SingleFactory` conserva la construcción **tipada** (`CreateObject(Type)`, `CreateObject<T>()`) y pierde las dos sobrecargas que cargan por nombre/ruta; `FactoryCache` deja de existir. |

### 3.2 La suite hermética (7 archivos en `2.0/tests/Cooperator.Harness.Tests/`)

| Path | Casos | Propósito |
|---|---:|---|
| `LazyProviderTestBase.cs` | — | Aísla el estado ambiental (configuración vigente + hook/caché) entre casos y ofrece la configuración de prueba del catálogo. Las clases se marcan con la colección de configuración para serializarse con los casos del Bolt 011. |
| `ProviderResolutionTests.cs` | 6 | Validación estructural del nombre (falta ensamblado / falta tipo / tipo sin namespace / version-token como documentación / namespace de un segmento / la colisión de la concatenación como claves distintas); AC-1 (la clave esperada en el diagnóstico, con las dos claves y con el valor vacío) y AC-2 (el provider declarado resuelve por la vía del host; la entrada explícita del código generado). |
| `ProviderCacheTests.cs` | 5 | AC-3: una instancia por provider y por proceso bajo concurrencia (16 hilos con conteo de instancias), destinos distintos con su propia instancia, fallo **no** cacheado con corrección en caliente, reinicio explícito, y las dos puertas (entidad y nombre) a la misma instancia. |
| `ProviderDiagnosticsTests.cs` | 3 | AC-4: las cinco causas con su excepción tipada y **el contenido obligatorio** de cada fila del cuadro (clave, nombres intentados, causa subyacente, alternativas, motivo); el constructor estático que falla como **fallo del tipo**; y la forma de las seis excepciones (jerarquía `CoreException`, POCOs). |
| `ProviderHookTests.cs` | 4 | AC-5: el hook reemplaza la resolución por defecto y aporta la instancia; un hook que lanza produce `ProviderResolutionFailedException` con la excepción original; un hook que devuelve `null` es un fallo (no una ausencia); fijación única + registro tardío con reinicio de la caché. |
| `ProviderStartupValidationTests.cs` | 4 | AC-6: todos los fallos en una sola pasada y en orden (causas mixtas), el agregado estricto con todos ellos, la validación **opt-in** y una pasada limpia. |
| `NoImplicitLoadingTests.cs` | 4 | AC-7 y el NFR: el ensamblado **presente pero no declarado** no se encuentra y el hook sí lo obtiene (con su `AssemblyLoadContext`); 0 `Assembly.LoadFrom`/`LoadFile` en el código del framework **con el control no vacuo** (el 1.4 congelado sí las tiene); y ninguna clave de configuración puede provocar una carga desde ruta (las claves son exactamente `…:Assembly` y `…:Class` y un valor con forma de ruta no se carga). |

### 3.3 Proyectos y fuentes de soporte de prueba (§8.1)

| Path | Propósito |
|---|---|
| `2.0/tests/fixtures/lazy-providers/DeclaredProviders/DeclaredProviders.csproj` + `.cs` | El ensamblado del **caso normal**: `SampleLazyProvider` (con contador de instancias) y `SecondLazyProvider` (destinos distintos), más los tipos de las causas de tipo: `NotAProvider` (no implementa el contrato), `AbstractProvider`, `NoParameterlessCtorProvider`, `InternalCtorProvider` y `FailingStaticCtorProvider`. Es una **dependencia declarada** del proyecto de pruebas. |
| `2.0/tests/fixtures/lazy-providers/UndeclaredProviders/UndeclaredProviders.csproj` + `.cs` | El ensamblado del **NFR**: `UndeclaredLazyProvider`, **compilado pero no referenciado** (`ReferenceOutputAssembly=false`) y copiado a la subcarpeta `plugins-undeclared/` del directorio de salida. Su simple name no está en el `deps.json` (medido). |
| `2.0/docs/api-removals/US-000.BOLT-005.json` | La **constancia de rupturas** (fase E.2): las 5 entradas del retiro de superficie —con su reemplazo— y el punto de entrada nuevo del camino por entidad, insumo de las notas de migración del Bolt 028. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md` | `status: specified` → `in-development` y fila **T04** (toma del V-Bounce por G11) | Las transiciones de estado del Bolt con su fila de `Status timeline`; ninguna otra línea se tocó. |
| `2.0/tests/Cooperator.Harness.Tests/Cooperator.Harness.Tests.csproj` | Alta de los dos proyectos de soporte de prueba (el referenciado y el **no** referenciado) y una target que copia el segundo a `plugins-undeclared/` | Es lo que hace **medible** el NFR y ejercitable el caso normal contra ensamblados del propio árbol, como el SPEC §8.1 exige. No se tocó ningún otro artefacto de otro Bolt. |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Ninguno. El retiro de superficie de §E.1 se materializa en el **aporte en su forma retirada** de `Factory/Factories.cs`
(que llega al árbol 2.0 sin las dos sobrecargas de carga por nombre/ruta y sin `FactoryCache`): el árbol 2.0 no tuvo
nunca esa superficie, de modo que no hubo nada que borrar —la retirada se mide **contra la línea base 1.4** en el
gate de compatibilidad (§9.5), que es donde el 1.4 la tiene.

---

## 7. Implementation decisions

1. **La clave de la caché es el valor del contrato, no una cadena armada.** `ProviderName` implementa igualdad y
   hash estructurales sobre el par (ensamblado simple, tipo completo), y **eso** es la clave del
   `ConcurrentDictionary`: la colisión del 1.4 (`"A.B" + "." + "C"` vs `"A" + "." + "B.C"`) desaparece por
   construcción, no por elegir un separador "que no aparezca".
2. **La resolución corre dentro del `Lazy`, no fuera.** Es la diferencia entre "gana el primero que publica" (varias
   instancias construidas y descartadas) y "una instancia por provider y por proceso" —que es la semántica que
   ADR-006 §3.2 pide y que el caso concurrente **cuenta**—. Y el `try/catch` alrededor de `Lazy.Value` **descarta la
   entrada** cuando falla: un `Lazy` normal habría cacheado la excepción y envenenado el proceso.
3. **El fallo del tipo es del tipo.** Un provider cuyo constructor **estático** falla no produce una fila del cuadro
   de ADR-006 §3.3 (no es una de las cinco causas), no se convierte en `null` ni en una "ausencia": el framework
   encuentra el tipo, verifica su compatibilidad y la construcción falla **adentro**, así que el fallo del runtime
   viaja tal cual. Se declara acá y se prueba, en lugar de inventar una sexta causa.
4. **El registro tardío del hook es determinista y está probado.** El SPEC §14 exige que "el comportamiento de un
   registro tardío se pruebe y se documente (no se deja indeterminado)": la decisión es que el registro **reinicie
   la caché** —cambiar la estrategia de resolución no puede dejar sirviendo instancias resueltas por la anterior— y
   que un **segundo** registro sea un error de programa (fijación única). Las dos cosas están cubiertas por casos.
5. **La compatibilidad se verifica antes de construir** y con el **orden** del contrato (primero "no implementa",
   después "abstracto/interfaz", después "sin constructor público"): el motivo del diagnóstico es el más
   específico posible y el caso de cada motivo es explícito (incluido el constructor sin parámetros **no público**).
6. **La validación de arranque usa el mismo camino que el runtime.** Además de reportar todos los fallos en una
   pasada, una validación limpia deja los providers **ya resueltos** en la caché: la app no paga dos veces la
   resolución que ya validó.
7. **El hook se ejerce con su caso de uso real.** El caso del NFR no simula el "plugin en una carpeta" con una
   cadena: registra un hook que resuelve con **su propio `AssemblyLoadContext`** desde la carpeta de plugins, que es
   literalmente lo que ADR-006 §3.1 describe como política del host.

---

## 8. Deviations and assumptions

### 8.1 El contrato del provider (`ILazyProvider`) **no estaba** en el árbol: se aporta, y se declara

El SPEC §5 lista como prerrequisito el proyecto `Core` "**con su superficie base y `ILazyProvider` presentes**" y
§15 tiene una condición de parada para el caso contrario ("este Bolt **no crea la capa base**"). Al iniciar el
V-Bounce, `2.0/src/Cooperator.Framework.Core/` tenía `Configuration/**` (Bolt 011), la superficie base del Bolt 006
(`IObject`, `ObjectState`, `ObjectListHelper`, dos excepciones) y **no** tenía `LazyLoad/ILazyProvider.cs` ni su
tipo de parámetro `IUniqueIdentifiable`.

**Resolución aplicada (declarada, no silenciosa):** la nota de ejecución del dueño para este V-Bounce establece que
**los archivos `Core/LazyLoad/**` los crea este Bolt —son su aporte**—, y eso es coherente con ADR-001 §3.6, que
asigna a este Bolt el **slot** de la carga de providers: el "no crea la capa base" de §15/§5 se lee como "no porta
la capa base **más allá de su slot**" (no se aportó `BaseObject`, `BaseObjectList`, `IGateway`, `SecurityRights` ni
ninguno de los otros tipos del núcleo). Se aportaron, entonces, **dos archivos** y ninguno más:

| Archivo aportado | Forma | Por qué |
|---|---|---|
| `LazyLoad/ILazyProvider.cs` (57 líneas) | **ver batim** del 1.4, sin una línea cambiada | Es el contrato del provider que este Bolt usa; el SPEC §4 lo declara **fuera de alcance para modificar** y su forma es la que el código generado del cliente implementa. |
| `IUniqueIdentifiable.cs` (48 líneas) | **ver batim** del 1.4 | Cierre mínimo: es el tipo del parámetro de `ILazyProvider.GetEntity(Type, IUniqueIdentifiable)`; sin él el contrato no compila. |

Lo único que cambia respecto de los originales es la **codificación de archivo** (el 1.4 usa CRLF y el árbol 2.0
UTF-8 sin BOM con LF; §3.15/W23): el **contenido** es idéntico, línea por línea. Si el revisor prefiere que ese
prerrequisito lo entregue otro Bolt, el ajuste es de secuencia y no cambia el entregable de este.

### 8.2 Lo que la conformidad con el gate "Test-first evidence" (§9) significa acá, medido

El SPEC pide que los casos negativos se escriban **desde el cuadro de ADR-006 §3.3** —no desde la implementación— y
que la prueba del NFR se escriba **antes de retirar la superficie de §E.1**. Lo cumplido, con su alcance exacto:

- **Los casos se derivan del cuadro normativo.** Cada aserción de `ProviderDiagnosticsTests` nombra el contenido
  **obligatorio** de su fila (entidad + clave esperada, nombre intentado + causa subyacente, tipo + ensamblado +
  alternativas, motivo, origen del hook). Ninguna expectativa se escribió mirando la implementación: las constantes
  del motivo viven en la excepción y el caso las compara contra el valor del contrato.
- **El aporte nace retirado, y el retiro se mide contra la línea base.** En una 2.0 sin fuentes del núcleo no hay
  una "superficie vieja" que desmontar en dos pasos: `Factories.cs` se aporta **ya sin** las dos sobrecargas y sin
  `FactoryCache`, y la prueba de que la superficie se retiró es el **gate de compatibilidad contra el 1.4**, que es
  donde esa superficie existe (§9.5). No se declara un RED que no haya ocurrido.
- **El control del NFR no es vacuo, y eso sí se prueba.** `NoImplicitLoadingTests` escanea el código del framework
  por `Assembly.LoadFrom`/`LoadFile` (0 ocurrencias **en 2.0/src**) y exige que el **1.4 congelado sí las contenga**
  —si no, el caso estaría midiendo nada—: es la evidencia de que el control detectaría una reintroducción.
- **Y el escenario del NFR es real, no simulado:** el ensamblado no declarado **existe** en la subcarpeta del
  despliegue y **no** figura en el `deps.json` (medido), de modo que la resolución por defecto falla por el sondeo
  que el ALC moderno **no** hace y el hook sí lo obtiene.

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 1`, `review.decision: approved`
(`2026-10-16T00:19:59-03:00`); el Bolt conserva su `AITL-BOLT-READY-Approval` y entró a este V-Bounce en
`specified` con T01–T03 presentes; ADR-006/ADR-001/ADR-008/ADR-004 siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.8). No hay cambio material → G15 no se
activa.

### 8.4 El prerrequisito de orden (011→005) y la frontera con el Bolt 011

El SPEC declara prerrequisito el contrato de configuración de ADR-004 §3.2 (entregable del Bolt 011) con condición
de parada. **Está satisfecho**: `ICooperatorConfiguration`, `CooperatorSettings.Current` y las claves
`Cooperator:LazyLoad:Providers:<entidad>:Assembly|:Class` existen en el árbol desde el V-Bounce N°2 del Bolt 011.
Este Bolt **no** define ni renombra el catálogo, **no** toca la sección custom `ProvidersSection` ni la escritura del
archivo de configuración del host (alcance del 011) y **nunca** cae a leer `App.config` ni una fuente propia: la
única fuente de nombres del camino por entidad es el contrato. Esa es exactamente la condición de parada §15 #1.

### 8.5 Comprobación de las condiciones de parada (§15)

- **El contrato de configuración no está presente:** negativa (§8.4).
- **El proyecto `Core` no existe con su superficie base y `ILazyProvider`:** es el punto que se resolvió aportando
  el contrato y su cierre (§8.1), que es el **slot** de este Bolt (ADR-001 §3.6); no se portó la capa base.
- **Hace falta cargar desde una ruta, una carpeta o un ALC propio DENTRO del framework:** negativa — es el **hook**
  del host; el framework no tiene ninguna de esas capacidades.
- **`Core` necesitaría una dependencia de terceros:** negativa — `netstandard2.1`, **0** paquetes directos
  (§9.7). `System.Runtime.Loader` **no** se toma: el ALC es del host y entra por el hook.
- **El gate marca una ruptura que no está en la lista:** negativa — las 5 entradas del fragmento cubren las
  diferencias de miembro del ensamblado (§9.5), incluida la **adición** del camino por entidad, que el gate reporta
  como diferencia de miembro en la dirección de adición.
- **La validación de arranque no puede reportar todos los fallos:** negativa — la entrada explícita lo permite
  (§2.4) y el caso con causas mixtas lo mide.
- **Aparece la necesidad de un segundo camino implícito:** negativa.
- **Cambio material de fuente gobernada:** negativa (§8.3).
- **Un gate de §9 no cierra en `pass`:** negativa — todos cierran en `pass` o tienen su razón de `n/a` (§9.9).

---

## 9. Verification evidence

### 9.1 Prerequisitos (§5) verificados antes de generar

```
SPEC-261016-0015 → status: approved · revision: 1 · review.decision: approved (2026-10-16T00:19:59-03:00)
BOLT-005         → status: specified · filas T01..T03 presentes → se tomó con T04 (01:54:15)
contrato ADR-004 → ICooperatorConfiguration + CooperatorSettings.Current + claves LazyLoad:Providers:<entidad>:*  ✔ (Bolt 011)
2.0/src/Core     → Configuration/** (011) + superficie base del 006 · SIN LazyLoad/** (aporte de este Bolt, §8.1)
arnés            → 59/59 verde antes de tocar nada · caracterización 83/83 · gate de API operativo
1.4/**           → intacto · línea base 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4
```

### 9.2 El núcleo compila: `netstandard2.1`, sin `System.Runtime.Loader`, 0 advertencias

```
MSBuild 2.0/src/Cooperator.Framework.Core (Rebuild, Release)
  Cooperator.Framework.Core -> …\bin\Release\netstandard2.1\Cooperator.Framework.Core.dll
EXIT=0        (con Nullable=enable + TreatWarningsAsErrors=true y AnalysisLevel=latest)
```

### 9.3 La suite hermética (AC-1…AC-8)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  restore OK (con restore locked por lock files versionados)
  build sin advertencias (tratadas como error): 16 proyectos      ← incluye los dos proyectos de soporte de prueba
  harness-hermetic          → Passed! - Failed: 0, Passed: 85, Skipped: 0, Total: 85      ← 59 previos + 26 de este Bolt
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83      ← sin regresiones
Resultado: ok (codigo de salida 0)
```

**168 casos herméticos verdes** (85 + 83), 16 proyectos con 0 advertencias, `EXIT=0`. Los 26 casos nuevos son los
seis archivos de §3.2: nombre calificado y AC-1/AC-2 (6), caché y concurrencia (5), diagnósticos (3), hook (4),
validación de arranque (4) y NFR/AC-7 (4).

### 9.4 El cuadro de diagnósticos, causa por causa (AC-4)

| Causa | Excepción | Contenido verificado en el caso |
|---|---|---|
| Falta la clave de configuración | `ProviderNotConfiguredException` | La **entidad** y la **clave esperada** (`Cooperator:LazyLoad:Providers:Customer:Assembly`) en el mensaje y en las propiedades |
| El ensamblado no se resuelve | `ProviderAssemblyNotFoundException` | El **nombre intentado**, si hay **hook** y la causa subyacente (`FileNotFoundException`/`FileLoadException`) |
| El ensamblado existe, el tipo no | `ProviderTypeNotFoundException` | El **tipo**, el **ensamblado resuelto** y los tipos que **sí** implementan el contrato |
| El tipo existe, no sirve | `ProviderTypeIncompatibleException` | El **motivo**: los tres del contrato, cada uno con su caso (incluido el constructor sin parámetros **no** público) |
| El hook falla | `ProviderResolutionFailedException` | Que **vino del hook** y la **excepción original** del host (idéntica, no envuelta de nuevo) |

### 9.5 La superficie retirada, medida con el gate de compatibilidad (AC-8)

```
pwsh -File 2.0/build/api-compat.ps1 -Mode Current      (línea base 1.4 ↔ superficie 2.0)
  differences: 183 · CP0002 (miembro que difiere con el tipo presente): 5
    · Core.ObjectListHelper<T,T2>.CreateExcelFile(...)          ← Bolt 006 (ya declarada)
    · Core.Factory.SingleFactory.CreateObject(string, string)   ← RETIRADA por este Bolt  (BOLT005-02)
    · Core.Factory.SingleFactory.CreateObject(string, string, string) ← RETIRADA (la de LoadFrom)  (BOLT005-01)
    · Core.LazyLoad.LazyProviderFactory.GetForEntity(string)    ← ADICIÓN de este Bolt   (BOLT005-05)
    · Library.SmtpMail.SendMailViaCDONTS(...)                   ← Bolt 011 (ya declarada)
  Diferencias de miembro SIN explicación en la constancia: 0   ← lo asserta ApiRemovalCoverageTests del arnés
```

Y el `FactoryCache` de clave concatenada aparece como **tipo ausente** (`CP0001`) contra el baseline, cubierto por
`BOLT005-03`.

### 9.6 El NFR `no-implicit-assembly-loading`, medido (AC-7)

| Medición | Resultado |
|---|---|
| El ensamblado no declarado existe en el despliegue | Sí: `plugins-undeclared/Cooperator.LazyFixtures.Undeclared.dll` (subcarpeta del directorio de salida) |
| Está declarado como dependencia | **No**: 0 ocurrencias en `Cooperator.Harness.Tests.deps.json` y no está en el directorio raíz de salida |
| El camino por defecto lo resuelve | **No**: `ProviderAssemblyNotFoundException` con el nombre intentado y la causa del runtime (`FileNotFoundException`) |
| El hook del host lo obtiene | **Sí**: con su propio `AssemblyLoadContext` desde la carpeta de plugins |
| `Assembly.LoadFrom`/`LoadFile` en el código del framework (`2.0/src`) | **0** ocurrencias — y el 1.4 congelado **sí** las tiene (el control no es vacuo) |
| Claves de configuración capaces de cargar desde ruta | **Ninguna**: las claves del camino son exactamente `…:Assembly` y `…:Class`, y un valor con forma de ruta no carga nada |

### 9.7 Huella de dependencias (AC-8)

`Cooperator.Framework.Core`: `netstandard2.1`, **0** `PackageReference` y **0** paquetes directos en su lock file
(§9.2). Ninguna dependencia nueva en ningún proyecto del árbol: los dos proyectos de soporte de prueba no declaran
paquetes (sólo referencian `Core`) y el arnés ya tenía los suyos. Las invariantes del Bolt 001
(`Core_and_Library_declare_zero_third_party_direct_dependencies`, `No_project_file_declares_a_package_version`,
`Every_project_of_the_tree_has_a_versioned_lock_file` con los proyectos nuevos y su lock file) están verdes (§9.3).

### 9.8 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.9 Gates (§9)

| Gate | Level | Resultado | Evidencia |
|------|-------|-----------|-----------|
| Build / lint / type checks | V-Bounce | **pass** | 16 proyectos sin advertencias; `Core` en `netstandard2.1` **sin** `System.Runtime.Loader` (§9.2, §9.3) |
| Unit / component tests | V-Bounce | **pass** | 168/168 herméticos verdes, con los casos negativos por causa y la corrida concurrente de la caché (§9.3, §9.4) |
| Consumer-side contract tests (test doubles) | V-Bounce | **pass** | El contrato del provider no cambia; el caso normal (dependencia declarada) y el hook se ejercitan contra **ensamblados del propio árbol** (§9.6) |
| SAST | V-Bounce | **pass** | 0 `LoadFrom`/`LoadFile`, sin rutas, sin binder por defecto y sin resolución por nombre simple en el camino por defecto (§9.6) |
| Dependency scanning / licenses | V-Bounce | **pass** | `Core`: 0 dependencias de terceros; ninguna dependencia nueva (§9.7) |
| OWASP Top 10 — static review | V-Bounce | **pass** | El único camino por defecto es **por nombre de un destino declarado**; la superficie que convertía un valor de configuración en carga arbitraria **se retira**; el hook es código del host con su política de confianza |
| Prompt-injection scan | V-Bounce | **n/a (razón)** | No hay material textual no confiable en el camino: los nombres vienen del catálogo del despliegue y el contenido del tipo nunca se interpreta como instrucción |
| Secret-leak scan | V-Bounce | **pass** | 0 secretos: los diagnósticos incluyen **nombres** y claves de configuración, nunca valores sensibles |
| Hallucination lint | V-Bounce | **pass** | Cada decisión es trazable a ADR-006/ADR-001/ADR-004 o al SPEC; 0 causas fuera del cuadro de §3.3; 0 rutas "de conveniencia" |
| IP / license provenance | V-Bounce | **pass** | Código propio; los dos archivos aportados conservan el encabezado BSD del 1.4; ninguna implementación de ALC de terceros |
| PII / DLP | V-Bounce | **pass** | `internal`: metadata de carga (nombres de ensamblado y tipo) y claves de configuración; sin datos personales |
| Dependency-confusion | V-Bounce | **pass** | Sin paquetes nuevos ni resolución por nombre no calificado (la regla de ADR-006 §3.1 es, de hecho, la mitigación del riesgo) |
| Test-first evidence | V-Bounce | **pass** | Los casos se derivan del **cuadro normativo** y el control del NFR no es vacuo — con el alcance declarado en §8.2 |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **pass** | Dos corridas con resultado idéntico, incluidas las concurrentes; el conteo de instancias por provider es estable |
| Bolt-manifest validation | V-Bounce | **pass** | Manifiesto válido contra `manifest-v5-bolt.schema.json` (§9.10) |
| Integration tests (real dependencies) | Integration test | **n/a (razón)** | Plan de integración `n/a` (Bolt `not-required`): los ensamblados son del propio árbol de pruebas |
| Contract / E2E against the real providers | Integration test | **n/a (razón)** | El contrato del provider no cambia y los providers del cliente no se modifican |
| DAST / OWASP Top 10 — dynamic | Integration test | **n/a (razón)** | Sin superficie desplegada ni servicio |
| Perf-smoke (p95/p99) | Integration test | **n/a (razón)** | La resolución ocurre una vez por provider y por proceso: la caché es la mitigación de costo por diseño |
| Observability (logs / metrics / traces) | Integration test | **n/a (razón)** | Biblioteca sin runtime propio: su observabilidad son los diagnósticos tipados |

### 9.10 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-005-carga-de-providers-lazy.json`: se agregó la entrada **`v_bounces[0]`**
  (`number: 1`, `spec_revision: 1`, `execution_outcome: ready_for_review`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos y fuentes de este V-Bounce se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD`
  (§3.15/W23); verificado tras escribirlos.

---

## 10. Manual interventions

**Ninguna intervención humana sobre el código.** Las fuentes del subsistema se escribieron en este V-Bounce, los dos
archivos del contrato se aportaron **verbatim** (sin una línea cambiada), y los proyectos de soporte de prueba y las
constancias también. **Tampoco hubo operaciones de git** (G34): el árbol está sin commitear y el commit es del
humano. Los únicos actos del agente son los del contrato del V-Bounce: la toma del Bolt (T04), la implementación,
las mediciones, el cierre y la pausa.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol de trabajo es la entrega y no se hicieron operaciones de git.
- **SPEC ejecutado:** [SPEC-261016-0015](../spec/SPEC-261016-0015-carga-de-providers-lazy.md) — **revisión 1**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T00:19:59-03:00`).
- **Bolt:** [US-000.BOLT-005](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) — `Status timeline`,
  fila **T04**.
- **Manifiesto:** [US-000.BOLT-005-carga-de-providers-lazy.json](../metrics/bolts/US-000.BOLT-005-carga-de-providers-lazy.json)
  — `v_bounces[0]`.
- **La constancia de rupturas:** `2.0/docs/api-removals/US-000.BOLT-005.json` (5 entradas con su reemplazo).
- **La suite y sus soportes:** `2.0/tests/Cooperator.Harness.Tests/{LazyProviderTestBase,ProviderResolutionTests,ProviderCacheTests,ProviderDiagnosticsTests,ProviderHookTests,ProviderStartupValidationTests,NoImplicitLoadingTests}.cs`
  y `2.0/tests/fixtures/lazy-providers/{DeclaredProviders,UndeclaredProviders}/`.
- **El entregable:** `2.0/src/Cooperator.Framework.Core/LazyLoad/**`, `…/Factory/Factories.cs`, `…/IUniqueIdentifiable.cs`.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Core/Factory/Factories.cs`
  (L48-55 la construcción por nombre simple, **L64-72 la sobrecarga con `LoadFrom`**, L100-118 `FactoryCache` con
  clave concatenada), `…/LazyLoad/ILazyProvider.cs` (el contrato), `…/LazyLoad/LazyProviderFactory.cs` (L46-49 la
  fachada del 1.4) y `…/LazyLoad/LazyProviderFactoryConfigHelper.cs` (la sección custom: alcance del Bolt 011).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-005) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0015 revisión **1** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~9 min de reloj (01:54:15 → 02:03:34): toma, implementación del subsistema, suite, mediciones y cierre |
| Archivos creados | **30** (16 del subsistema y su contrato, 7 de suite, 4 de los dos proyectos de soporte, 1 constancia, 2 lock files) |
| Archivos modificados | **2** (el Bolt y el `.csproj` del arnés que cablea los soportes de prueba) |
| Tests creados | **26** (6 de resolución + 5 de caché + 3 de diagnósticos + 4 de hook + 4 de validación + 4 del NFR) |
| Tests de la suite | **168** herméticos (85 del arnés + 83 de caracterización), todos verdes |
| Rupturas declaradas | 5 (2 sobrecargas retiradas, la caché de clave concatenada, el cambio de semántica del sondeo y la entrada nueva del camino por entidad) |
| Dependencias nuevas | **0** en todo el árbol; `Core` sigue con **0** de terceros |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |
| Código generado por IA | 100 % de lo generado en este V-Bounce |
| Desviaciones del SPEC | 0 de alcance; 1 prerrequisito resuelto por aporte declarado (§8.1) y 1 alcance de gate precisado (§8.2) |

---

## 13. Pending items and stubs

1. **La decisión del revisor sobre el aporte del contrato (§8.1):** el SPEC §5/§15 asumen `ILazyProvider` presente y
   el árbol no lo tenía. Se aportó **verbatim** (él y su cierre de un tipo), como el **slot** de este Bolt. Si el
   revisor prefiere otra secuencia (que lo entregue otro Bolt), el ajuste es de secuencia y no cambia este entregable.
2. **La superficie que este Bolt NO retira, por decisión del SPEC (§4):** la sección custom `LazyLoad/ProvidersSection`
   y la escritura del archivo de configuración del host son alcance del **Bolt 011**; el motor, el CLI, el shell y las
   plantillas son de los Bolts 017/018/025/019; y el arreglo de **F-47** (`Type.GetType` por nombre simple en
   `BaseObjectList`) es del **Bolt 008** —este Bolt sólo **no reintroduce** resolución implícita.
3. **Insumo para las notas de migración (Bolt 028):** la lista de rupturas vive en
   `2.0/docs/api-removals/US-000.BOLT-005.json` (§3.3) y en §2.5 de este MEM. El cambio de despliegue que el
   consumidor debe conocer —"copiar DLLs a una carpeta" deja de funcionar por defecto: declarar el provider como
   dependencia o registrar el hook— está declarado ahí con su reemplazo.
4. **Sin obligaciones registradas:** este Bolt no difiere nada (`registered_obligations: []`); el único punto
   diferido del sistema de providers era la configuración del host, que es del Bolt 011 y ya está entregada.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto + el árbol entregado (el subsistema de carga
  con sus 16 archivos, la constancia de rupturas, la suite de 26 casos y los dos proyectos de soporte de prueba).
- **Qué se pide decidir:** la aceptación del entregable y —si se considera necesario— el punto de §8.1 sobre el
  aporte del contrato del provider.
