---
id: "MEM-261016-0343"
title: "Provider de datos y capa `Data` (SPEC rev.2) — V-Bounce N°1: BLOQUEADO por el caché del host dentro de la capa (condición de parada declarada por el SPEC)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-014" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0020" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-005", "ADR-004", "ADR-001"] # ningún cambio los contradice: el V-Bounce se detuvo en una condición de parada declarada por el SPEC (§6/C.5, §15) antes de escribir código
manifest: "US-000.BOLT-014-provider-de-datos.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:43:28-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T03:46:27-03:00"
  decided_at: "2026-10-16T03:46:27-03:00"
  findings:
    - "Compliant — bloqueo REPRODUCIDO: en 1.4 `BaseGateway.cs` `using System.Web` + `HttpContext`×20, `BaseLoader.cs` ×19, `SQLHelper.cs` ×4, `SqlHelperParameterCache.cs` ×4 (todos con `using System.Web`); el proyecto 2.0 es `netstandard2.1` (sin `System.Web`); los cinco archivos de §6 **ausentes** del 2.0; `integration_test_runs[]` vacío declarado. La circularidad 014↔13 es real."
    - "Compliant — las citas coinciden: §6/C.5 + §15 («si el retiro aparece como paso necesario para compilar ⇒ parar y reportar: no se absorbe la sustitución del 013») **activada correctamente**; y la circularidad con el MEM del 013 (`MEM-261016-0328`: «este Bolt no crea la capa de datos») está bien identificada: 014 no compila sin el retiro (aspecto del 013) y 013 no sustituye puntos que no existen."
    - "Compliant — **0** aportes/transformaciones: 0 archivos de producto creados/modificados/retirados; **0** archivos del 013 tocados (ni SPEC, ni MEM, ni caché); `.csproj`/`Directory.Packages.props` intactos; suite **230/230 verde**; `1.4/**` intacto; `integration_test_runs[]` vacío."
    - "ADJUDICACIÓN (c)/(d) — **ENDOSO (d)**, con el encuadre **«el retiro es parte del port; la sustitución es del 013»**: ampliar el alcance del 014 (revisión material de `SPEC-261016-0020`) para que el **aporte** incluya el **retiro de las llamadas al caché del host** —`HttpContext`/`System.Web`—, que es **obligatorio para compilar** en `netstandard2.1`; la **SUSTITUCIÓN** por `ICacheProvider` (regiones, TTL, un solo camino) sigue siendo del **Bolt 013**, re-encolado después. No es absorber el 013: es reconocer que el retiro mecánico de la plataforma web es parte del port, distinto de la sustitución por contrato."
    - "AJUSTE del endoso anterior (`MEM-261016-0328`): la secuencia final es **`014-ampliado → 013`** — el 014 aporta/retypea la capa `Data` **incluido el retiro del caché del host** (paso de compilación, no de diseño) y el 013 sustituye **después** los puntos de caché por `ICacheProvider`, sobre la capa ya presente. Mi constancia anterior («014 aporta/retypea, 013 sustituye después») se **afina**: el retiro mecánico pasa al 014; la sustitución queda en el 013."
    - "Compliant — el inventario del aporte y de sus bloqueantes (§9.1: 5 archivos, 5 549 líneas, 4 con `HttpContext` = 47 usos, 1 espera activa, 11 casts) queda como insumo del re-run; cierre T04/T05; MEM único; manifiesto válido (v_bounces[0] `blocked`, spec_revision 2)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0343 — Provider de datos y capa `Data` (rev.2) — V-Bounce N°1 **bloqueado**

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — `type: non-functional`, `work_category: refactor`, `risk_class: high`, `data_classification: internal`, `test_integration: required` |
| **SPEC**        | [SPEC-261016-0020](../spec/SPEC-261016-0020-provider-de-datos.md), **revisión 2** (aprobada `2026-10-16T03:42:01-03:00`) |
| **V-Bounce**    | **1** — el primero del Bolt: reconocimiento → **condición de parada del SPEC** (§6/C.5 y §15, con el aviso de frontera del revisor) **antes** de escribir código |
| **ADRs**        | [ADR-005](../adrs/ADR-005-provider-de-datos.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

El V-Bounce se **detuvo sin escribir código de producto** por la **condición de parada que el propio SPEC declara**:
el **caché del host** (`HttpContext`/`System.Web`) vive **dentro** de la capa `Data` del 1.4 y **su retiro es el
aspecto del `US-000.BOLT-013`**; aportar la capa —que es el objeto de este Bolt— **exige retirarlo**, porque el
proyecto `Data` del 2.0 es `netstandard2.1` y ahí `System.Web` **no existe**.

Medido en el árbol y en la línea base congelada:

```
2.0/src/Cooperator.Framework.Data:  16 archivos .cs — TODOS en Exceptions\   ·  TFM netstandard2.1
Los cinco archivos que §6 declara (aportar/retypear): AUSENTES en el 2.0
  BaseRule.cs 536 l · SQLHelper.cs 2532 l · SqlHelperParameterCache.cs 269 l · BaseGateway.cs 1557 l · BaseLoader.cs 655 l
El caché del host, DENTRO de la capa (el aspecto del Bolt 013):
  BaseGateway.cs             using System.Web;  ·  HttpContext×20  (HttpContext.Current.Cache[…], System.Web.Caching.Cache)
  BaseLoader.cs              using System.Web;  ·  HttpContext×19  (ídem)
  SQLHelper.cs               using System.Web;  ·  HttpContext×4   (el caché del timeout: "Cooperator_DAL_SP_CommandTimeOut")
  SqlHelperParameterCache.cs using System.Web;  ·  HttpContext×4   (el caché de metadata: "Cooperator_DAL_SP_Params_Cache")
No hay subconjunto compilable: BaseRule.cs (18 refs a SqlHelper, 5 a InternalDBHelper, 2 a DataBaseHelper),
BaseGateway.cs (3 / 2 / 26) y BaseLoader.cs (21 DataBaseHelper) consumen el interior que vive en SQLHelper.cs —
justamente uno de los archivos con HttpContext. Sin tocar el caché del host, NINGUNO de los cinco compila.
```

Las dos fronteras que el SPEC fija para este caso dicen exactamente qué hacer:

- §6/C.5 (nota de mecánica) y §15: «*`HttpContext`/`System.Web` viven **dentro** de `BaseGateway.cs`/`BaseLoader.cs`
  y su retiro es el **aspecto del Bolt 013**. Si al aportar la capa ese retiro aparece como **paso necesario para
  compilar**, **parar y reportar**: no se absorbe la sustitución del 013*».
- Y el `AITL-SPEC-Approval` de la revisión 2 lo endosa como frontera explícita (finding del revisor, §13/§14/§15):
  «*se declara **condición de parada** (si el retiro aparece como paso necesario para compilar ⇒ parar y reportar,
  patrón de la revisión de secuencia); **nunca** absorber la sustitución del 013 (G11)*».

**La secuencia endosada por el revisor no es ejecutable tal como está.** El `AITL-MEM-Approval` del Bolt 013
([`MEM-261016-0328`](MEM-261016-0328-cache-en-data.md), finding «CORRECCIÓN DE SECUENCIA») dejó constancia de que
**`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`** ✔ — y este V-Bounce lo intentó por ese camino ✔ — pero se
encontró con que **014 no puede aportar la capa sin el retiro que es del 013** y **013 no puede sustituir puntos de
caché que no existen** (su propio MEM §1 lo dice: «*este Bolt no crea la capa de datos*»). Es una **dependencia
circular** entre dos alcances que **no se resuelve ejecutando**: la resolución es del **dueño del orden**, con una
decisión de secuencia o una revisión material (§15).

**Qué se hizo y qué no.** Se verificaron las precondiciones (SPEC `approved` rev.2 ✔; Bolt `specified` con T01–T03 ✔;
arnés verde **230 casos** ✔; contrato de configuración del 011 **presente** ✔ con `Cooperator:Data:CommandTimeoutSeconds`
y su default documentado 30 ✔; la secuencia 014→013 citada ✔), se levantó el **inventario de lo que el aporte
exige** (los cinco archivos, el caché del host dentro de ellos, las esperas activas, los casts, el tamaño de cada
archivo) y se **paró** ✔. **No** se aportó ni transformó ningún archivo del 1.4 ✔, **no** se tocó ningún artefacto
del Bolt 013 ✔ (ni su SPEC, ni su MEM, ni la caché), **no** se tocó ningún `.csproj` ni `Directory.Packages.props` ✔ y
**no** se escribió la capa ni su punto de conversión. El árbol queda **exactamente** como estaba: la suite sigue
verde (§9.3) y el registro de rupturas **no** cambia (las declaraciones de ADR-005 §3.1/§3.3 siguen correctamente en
`pendiente`).

---

## 2. Implemented phases

| Fase de §6 | Estado | Por qué |
|---|---|---|
| **A — El proveedor único y su línea fijada** | **no iniciada** ✗ | Es **dependency-free respecto de la capa** (versión centralizada + `PackageReference` + retiro de `System.Data.SqlClient`) — salvo que su **criterio de aceptación (2) es el smoke real** contra SQL Server, que sólo tiene sentido con la capa compilando. Ver §13.1: es la única parte que el dueño podría decidir adelantar, y **no** se ejecutó porque §15 reserva al dueño el cambio de plan. |
| **B — El punto único de conversión** | **no iniciada** ✗ | También **dependency-free de la capa** (dos archivos nuevos: el adaptador y `InvalidTransactionTypeException`) pero **depende de Phase A** (el paquete del provider). Mismo criterio: §15 reserva la decisión de partir el Bolt. |
| **C — Interior tipado, transacciones y superficie** | **bloqueada** ✗ | Es el **corazón del aporte**: `SQLHelper.cs` (2532 líneas), `SqlHelperParameterCache.cs`, `BaseRule.cs`, `BaseGateway.cs` y `BaseLoader.cs`. **Cuatro de ellos llevan el caché del host** y **ninguno compila** sin retirarlo (§1) ⇒ condición de parada ✔. |
| **D — Timeout y cadena de conexión** | **bloqueada** ✗ | Sus archivos de destino (`BaseRule.cs`, `SQLHelper.cs`) son los mismos del aporte bloqueado; el **catálogo** del 011 (fuente del timeout y de la cadena) **sí** está presente ✔ (no es el bloqueo). |
| **E — Cambios de comportamiento, rupturas y evidencia** | **bloqueada** ✗ | Su insumo es la capa retypeada (el inventario de casts y esperas, la corrida de integración del provider, el fragmento de rupturas). El **inventario de lo que hay que retirar** quedó levantado (§9.1) y es el insumo de esa evidencia. |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `devflow/memory/MEM-261016-0343-provider-de-datos.md` | Este MEM (exactamente uno por V-Bounce). |

> **Ningún archivo de producto, de prueba, de documentación ni de empaquetado fue creado en este V-Bounce.**

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-014-provider-de-datos.md` | `status: specified` → `in-development` → **`blocked`** + filas **T04** y **T05** | La toma por G11 y el cierre del V-Bounce con su bloqueo. |
| `devflow/metrics/bolts/US-000.BOLT-014-provider-de-datos.json` | `v_bounces[]` += la entrada (`number: 1`, `spec_revision: 2`, `blocked`) | El registro del resultado (§3.12). |

**No se tocó** ningún archivo del Bolt 013 (ni su SPEC, ni su MEM, ni la caché): su frontera se respeta
íntegra ✔ **ni** ningún `.csproj`, `Directory.Packages.props` del árbol 2.0 ✗.

---

## 5. Files deleted / 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

### 7.1 Parar en la frontera declarada, y no absorber el Bolt ajeno

El SPEC tiene una **condición de parada específica** para este desenlace y su revisor la **endosó** (§15, finding
del `AITL-SPEC-Approval`): *si el retiro del caché del host aparece como paso necesario para compilar, parar y
reportar; nunca absorber la sustitución del 013 (G11)*. Retirar `HttpContext`/`System.Web` de los cuatro archivos
habría sido exactamente eso: **implementar el §6/B.1 y §6/C.1 del SPEC-0019** (sustituir los tres puntos de caché
por el contrato `ICacheProvider` y dejar **un solo camino** para todos los hosts), con dos consecuencias peores que
el bloqueo: (i) dos Bolts escribiendo la misma sustitución con dos evidencias mezcladas, y (ii) **el Bolt 013
quedaría sin objeto** —su contrato y sus regiones no tendrían consumidor—. El reconocimiento previo existió
precisamente para no descubrir esto a mitad del aporte.

### 7.2 Lo que **no** se hizo, para que la decisión del dueño sea informada

- **Lo dependency-free respecto de la capa:** Phase A (la versión del provider centralizada y el `PackageReference`
  de `Data`, con el retiro de `System.Data.SqlClient`) y Phase B (los dos archivos nuevos del punto único de
  conversión). Ninguna toca el caché del host ni los cinco archivos bloqueados; ambas **sí** tocan artefactos de
  empaquetado (`.props`/`.csproj`) que el SPEC autoriza expresamente (§6/A.1).
- **Lo estrictamente bloqueado:** Phases C, D y E (el aporte y el retypeo de la capa, la espera activa, los casts,
  el hazard, el timeout por comando, la cadena sin inspección, la evidencia de integración).
- **El catálogo del 011 no bloquea:** `Cooperator:Data:CommandTimeoutSeconds` está publicado con su default
  documentado (30, leído de `(new SqlCommand()).CommandTimeout` en 1.4) y la clave de cadena de conexión también:
  no hay nada que pedirle al Bolt 011 (§15 no se activa por el catálogo).
- **El 013 no está en el árbol y eso NO es la precondición incumplida** (SPEC §5, constancia del revisor): la
  resolución del timeout se apoya en el catálogo del 011 y no en una caché propia; el bloqueo **no** es «falta el
  013», es «**el retirar su aspecto es requisito de compilación del aporte de este Bolt**».

### 7.3 La vía que **no** se toma

Aportar la capa con el caché del host **retirado** (aunque sea "provisoriamente", con la idea de que el 013 ponga
después el contrato) queda descartado por §15 y por la frontera de §14: es la sustitución del 013 —el retiro del
caché del host y de la plataforma web, con su **un solo camino** y sus regiones—, y hacerlo acá dejaría al 013 sin
objeto y a la evidencia del 014 mezclada con la de otro Bolt. Tampoco se aporta un subconjunto "que compile"
(`BaseRule.cs` aislado): §9.1 muestra que **no existe** —el interior (`SqlHelper`/`DataBaseHelper`) vive en
`SQLHelper.cs`, que es uno de los archivos con el caché del host.

---

## 8. Deviations and assumptions

### 8.1 La precondición de §5 y el encabezado de §6 no pueden cumplirse juntas, y la parada lo resuelve

§5 pide el proyecto `Data` «*con su superficie y sus puntos de caché **ya sustituidos** (Bolt 013)*» y §6/C.5 pide
aportar la capa **sin** tocar el caché del host: hoy **ninguna** de las dos cosas es posible a la vez, porque los
puntos de caché **existen dentro** de los archivos que hay que aportar y `netstandard2.1` no tiene `System.Web`.
La lectura ejecutada es la de la **condición de parada** (§6/C.5/§15 + el endoso del revisor): **parar y reportar**
sin absorber el 013. Se registra acá porque es el punto que el revisor debe adjudicar.

### 8.2 Lo que este V-Bounce **sí** dejó como trabajo útil

El **inventario del aporte y de sus bloqueantes** (§9.1): los cinco archivos con su tamaño, el caché del host
**con sus líneas y sus claves**, las **esperas activas** y los **casts** con su ubicación, y la **cadena de
dependencia** que impide todo subconjunto compilable. Es exactamente el insumo que §6/B.1, §6/C.1, §6/C.4 y §6/E.1
piden para retypear la capa, y ahora está **medido** en vez de supuesto.

### 8.3 Cambios de comportamiento del proveedor: no verificados (y no se declaran como si lo estuvieran)

Los tres cambios de ADR-005 §3.1/§6/E.1 (cifrado obligatorio y validación de certificado, `DbType.Time`/`DbType.Date`,
mapeos de namespace) requieren la capa y la corrida real. Este V-Bounce **no** los verifica ni los declara
verificados: quedan como **insumo pendiente** de las notas de migración (§13) y de la corrida de integración del
Bolt, tal como el SPEC lo ordena.

### 8.4 El ciclo de test de integración (plan `applicable`) queda sin ejecutar por el bloqueo

El SPEC declara `integration_test_plan.applicability: "applicable"` y los cuatro tests de integración del frontmatter.
Sin capa y sin corrida no hay nada que escribir ni que ejecutar: **no** se crean los archivos de prueba vacíos ni se
declara una corrida que no ocurrió. El bloqueo lo hace **moot** y el Bolt **no** pasa a `ready-for-acceptance`
(§5/§15).

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 2`** y `review.decision: approved` (`2026-10-16T03:42:01-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.3). El bloqueo **no** proviene de un cambio
de fuente: proviene de la **frontera entre dos alcances** (014 y 013), que el SPEC declara como parada.

### 8.6 Presupuesto de turnos (declarado)

Este V-Bounce usó **3 loops** (guía 10), con la disciplina de la ola: **reconocimiento → verificación del bloqueo →
cierre**. No hubo iteraciones de convergencia porque **no hubo implementación**: el bloqueo se detectó en el
reconocimiento, antes de tocar un archivo.

---

## 9. Verification evidence

### 9.1 El inventario que prueba el bloqueo

```
Los cinco archivos de §6 en el 2.0:            AUSENTES (Data = 16 .cs, todos en Exceptions\)   ·  TFM netstandard2.1
Caché del host DENTRO de la capa (aspecto del Bolt 013):
  BaseGateway.cs  (1557 l): using System.Web; HttpContext×20 — HttpContext.Current.Cache[key], System.Web.Caching.Cache.Add/Remove, CacheItemPriority, NoSlidingExpiration (L112, 323, 332, 336, 369, 377, 383, 391, 401, 403-404, 409, 423, 431, 438, 445, 455, 457-459, 463)
  BaseLoader.cs   ( 655 l): using System.Web; HttpContext×19 — las mismas claves y el mismo patrón (L183, 192, 196, 214, 221, 226, 234, 242-243, 245, 258, 265, 270, 278, 286-287, 289)
  SQLHelper.cs    (2532 l): using System.Web; HttpContext×4  — el caché del TIMEOUT: "Cooperator_DAL_SP_CommandTimeOut" (L66, 70, 73, 76)
  SqlHelperParameterCache.cs (269 l): using System.Web; HttpContext×4 — el caché de METADATA: "Cooperator_DAL_SP_Params_Cache" (L52, 56, 57, 59)
Cadena de dependencia que impide todo subconjunto compilable:
  BaseRule.cs    -> SqlHelper×18 · InternalDBHelper×5 · DataBaseHelper×2 · SqlTransaction×10 · SqlConnection×4
  BaseGateway.cs -> DataBaseHelper×26 · SqlHelper×3 · InternalDBHelper×2 · SqlTransaction×1
  BaseLoader.cs  -> DataBaseHelper×21        (el interior vive en SQLHelper.cs: el archivo con HttpContext)
Aspectos de ESTE Bolt, inventariados para cuando se ejecute:
  Thread.Sleep(0) en BaseRule.cs:185   ·   casts (SqlTransaction)/(SqlConnection) ×11 en BaseRule.cs + BaseGateway.cs:640
  System.Data.SqlClient en BaseRule.cs (×2), SQLHelper.cs, SqlHelperParameterCache.cs, BaseGateway.cs  ·  ConfigurationManager en BaseRule.cs/SQLHelper.cs
Precondiciones OK: contrato del 011 presente (CommandTimeoutSeconds + default documentado 30) · arnés verde 230 · 1.4 congelado
```

### 9.2 La condición de parada que se activó

```
§6/C.5 + §15 (nota de frontera): «HttpContext/System.Web viven DENTRO de BaseGateway.cs/BaseLoader.cs y su retiro
  es el aspecto del Bolt 013. Si al aportar la capa ese retiro aparece como paso necesario para compilar,
  PARAR Y REPORTAR: no se absorbe la sustitución del 013»
AITL-SPEC-Approval rev.2 (finding del revisor): «se declara condición de parada … nunca absorber la sustitución
  del 013 (G11)»                                                              → CONDICIÓN DE PARADA ACTIVADA
MEM-261016-0328 (Bolt 013, finding «CORRECCIÓN DE SECUENCIA»): «014 ejecuta ANTES que 013»  → intentado por ese
  camino y NO ejecutable: 014 no compila sin el retiro que es del 013, y 013 no sustituye puntos que no existen
```

### 9.3 El árbol quedó intacto y verde

```
git status --porcelain -- 1.4                                  → (vacío): el árbol congelado no se tocó
0 archivos de producto creados / modificados / retirados por este V-Bounce
0 artefactos del Bolt 013 tocados   ·   0 .csproj / Directory.Packages.props tocados
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 17 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 135, Skipped: 0, Total: 135
  mapping-hermetic          → Passed! - Failed: 0, Passed:  12, Skipped: 0, Total:  12
  characterization-hermetic → Passed! - Failed: 0, Passed:  83, Skipped: 0, Total:  83
Resultado: ok (codigo de salida 0)          ← corrida DESPUÉS del cierre: nada cambió
```

### 9.4 Gates (§9) — lo que puede declararse sin la capa

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **n/a (razón)** | No hay cambio de código que construir; el árbol (17 proyectos) sigue sin advertencias (§9.3). La directiva de §C.5 **no se ejerció**: no hubo aporte |
| Unit / component tests | **n/a (razón)** | Sin código nuevo; la suite hermética queda verde (**230** casos, §9.3) |
| Consumer-side contract tests (test doubles) | **n/a (razón)** | Dependen de Phases B/C, bloqueadas |
| SAST / Dependency scanning / OWASP / Secret-leak / PII-DLP / IP / Dependency-confusion | **n/a (razón)** | Sin superficie nueva, sin paquetes y sin dependencias: **0** archivos de producto tocados |
| Hallucination lint | **pass** | Todo lo declarado acá está medido sobre el árbol y el 1.4 (§9.1); **0** archivos, claves o líneas inventados |
| Behavioral reproducibility (hermetic suite) | **n/a (razón)** | Sin cambio de comportamiento |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.5) |
| API-compat con la línea base 1.4 (ADR-008 §5) | **n/a (razón)** | No hubo cambio de superficie (y el retiro de superficie de `SqlHelper`/`SqlHelperParameterCache` es de la capa que no se aportó) |
| Integration tests (real dependencies) | **n/a (razón)** | El plan es `applicable` pero **el bloqueo lo hace moot**: sin capa no hay smoke ni corrida (§8.4). Queda **pendiente del ciclo**, no ejecutado |

### 9.5 Manifest

`devflow/metrics/bolts/US-000.BOLT-014-provider-de-datos.json`: entrada **`v_bounces[0]`** (`number: 1`,
**`spec_revision: 2`**, `execution_outcome: **blocked**`), validada contra `manifest-v5-bolt.schema.json`.
`integration_test_runs[]` queda **vacío**: no hubo corrida (§8.4).

---

## 10. Manual interventions

**Ninguna intervención humana.** El reconocimiento, la verificación del bloqueo y el cierre se hicieron en este
V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""` (el V-Bounce no escribió código).
- **SPEC ejecutado:** [SPEC-261016-0020](../spec/SPEC-261016-0020-provider-de-datos.md) — revisión **2**
  (`AITL-SPEC-Approval` del `2026-10-16T03:42:01-03:00`): §4 (fronteras), §5 (precondición y **corrección de
  secuencia**), §6/C.5 (**frontera del caché del host**), §14 y §15 (condiciones de parada).
- **Bolt:** [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — filas **T04** y **T05**.
- **Frontera citada (★):** [`MEM-261016-0328`](MEM-261016-0328-cache-en-data.md) — el V-Bounce bloqueado del **Bolt
  013** y su finding «CORRECCIÓN DE SECUENCIA» (`014` ejecuta antes que `013`), **intentado por ese camino** y **no
  ejecutable** por el bloqueo de compilación de este MEM (§1/§9.2).
- **Prerrequisitos consumidos, presentes:** `Core/Configuration/{ICooperatorConfiguration,ConfigurationKeys,CooperatorConfigurationException}.cs` (Bolt 011) con `Cooperator:Data:CommandTimeoutSeconds` y su default documentado.
- **Evidencia de 1.4 citada:** `Data/BaseGateway.cs` (caché del host y `:640` cast), `Data/BaseLoader.cs` (caché del
  host), `Data/SQLHelper.cs` (caché del timeout, `commandTimeOut`), `Data/SqlHelperParameterCache.cs` (caché de
  metadata y el hazard de la conexión), `Data/BaseRule.cs` (`:185` espera activa, los castos y la cadena de conexión).
- **Manifiesto:** [US-000.BOLT-014-provider-de-datos.json](../metrics/bolts/US-000.BOLT-014-provider-de-datos.json).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-014) — **`blocked`** |
| SPEC ejecutado | SPEC-261016-0020 revisión **2** |
| Tiempo de generación IA | ~10 min de reloj (03:33 → 03:43) |
| Fases implementadas | **0 de 5** (A–E): el aporte del que dependen está bloqueado por una frontera declarada |
| Archivos de producto creados / modificados / retirados | **0 / 0 / 0** |
| Loops usados | 3 (guía 10; reconocimiento → verificación → cierre) |
| Inventario levantado | 5 archivos (5 549 líneas) · 4 con `HttpContext` (47 usos) · 1 espera activa · 11 casts · los cambios de comportamiento del provider pendientes de corrida |
| Tests de la suite | **230** herméticos (135 + 12 + 83), verdes **después** del cierre |

---

## 13. Pending items and stubs

1. **La decisión de escalación (dueño del orden).** El bloqueo es una **dependencia circular** entre dos alcances:
   `014` no puede aportar la capa sin el retiro del caché del host (aspecto del `013`) y el `013` no puede sustituir
   puntos que no existen. Las salidas viables que se dejan informadas —**la decisión es del dueño** y no se ejecuta
   acá—:
   - **(c) La recomendada:** una **decisión de secuencia con un acto de aporte único** —que un Bolt (el 001, o un
     alcance nuevo) aporte la capa `Data` del 1.4 **con el retiro del caché del host**, y que **014** y **013**
     corran después en el orden que se fije (el 014 sobre la capa ya presente, el 013 sustituyendo los puntos por el
     contrato). Preserva las fronteras de los dos SPECs y no obliga a absorber nada.
   - **(d)** **Ampliar el alcance del 014** (revisión material de `SPEC-261016-0020` + ajuste del endoso del 013) para
     que el aporte incluya el retiro del caché del host, y **re-encolar el 013** con su Phase A (contrato
     `ICacheProvider` + implementación en memoria + regiones, que es dependency-free) como sustitución posterior.
     Viable, pero deja en el 014 una caché interina (miembros en memoria) que el 013 reemplaza: trabajo doble acotado
     y declarado.
   - **(descartadas):** invertir la secuencia endosada (`013` primero) —imposible: su propio MEM dice que **no crea la
     capa de datos**— y aportar la capa retirando el caché "de paso" —prohibido por §15 y por la frontera de §14
     (absorber el 013).
2. **Lo dependency-free que el dueño podría decidir adelantar** (§7.2): Phase A (la línea del provider centralizada
   + el `PackageReference` de `Data` y el retiro de `System.Data.SqlClient`) y Phase B (el punto único de conversión
   y su excepción). **No** se ejecutaron: §15 reserva al dueño el cambio de plan (partir el Bolt).
3. **Los cambios de comportamiento del proveedor** (cifrado obligatorio/validación de certificado, `DbType.Time`/
   `DbType.Date`, mapeos de namespace) quedan **sin verificar** y así se declaran (§8.3): su verificación es de la
   corrida de integración, que el bloqueo dejó sin ejecutar (§8.4).
4. **La deuda declarada del DAAB** (`SqlHelper`, 2532 líneas) y el resto de los trade-offs de ADR-005 §4 siguen
   vigentes: este V-Bounce no los toca ni los cambia.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 2`, `blocked`) + el
  **diagnóstico del bloqueo** con su medición (el caché del host dentro de la capa, la cadena de dependencia que
  impide todo subconjunto compilable) y las salidas de escalación (§13.1). **No hay árbol entregado**: el V-Bounce no
  escribió código.
- **Qué se pide decidir:** (1) la **adjudicación del bloqueo** (la frontera declarada por §6/C.5/§15 + el endoso del
  revisor se activó: aportar la capa exige el retiro que es del Bolt 013) y (2) la **decisión de secuencia** entre
  las opciones informadas en §13.1.
