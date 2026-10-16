---
id: "MEM-261016-0407"
title: "Provider de datos y capa `Data` (SPEC rev.4) — V-Bounce N°2: la capa aportada y compilando (retipado, retiro del caché del host, punto único); BLOQUEADO por presupuesto con la constancia y las pruebas pendientes"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-014" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0020" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 4 # revision of the canonical SPEC used by this V-Bounce (rev.4: turn_budget 25 y el retiro del caché del host EN ALCANCE, §C.6)
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-005", "ADR-004", "ADR-001"] # ADR-005 §3.1/§3.2/§3.3/§3.4/§5 (provider único, punto único, interior tipado, hazard, NFRs), ADR-004 §3.2/§3.3 (catálogo del timeout y cadena resuelta sin inspección), ADR-001 §3.1/§3.4/§5 (TFM, presupuesto de dependencias, portabilidad sin la plataforma web)
manifest: "US-000.BOLT-014-provider-de-datos.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T04:07:42-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T04:15:21-03:00"
  decided_at: "2026-10-16T04:15:21-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°2 `blocked` (presupuesto 26/25 contra `turn_budget: 25` de la rev.4), SPEC `SPEC-261016-0020` revisión 4 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — capa aportada y compilando verificada por lectura + build: los 6 archivos del alcance (`BaseRule`, `BaseGateway`, `BaseLoader`, `SQLHelper`, `SqlHelperParameterCache`, `IBaseGateway`) + 3 de cierre en `Core` (`SecurityRights`, `IGateway`, `IValidable`) + 2 nuevos (`Casting/TransactionAdapter.cs`, `Exceptions/InvalidTransactionTypeException.cs`); `dotnet build` de `Data` → **0 errores / 0 advertencias** (`TreatWarningsAsErrors` activo); `Microsoft.Data.SqlClient` **6.1.7** centralizado en `Directory.Packages.props` + `PackageReference` sin versión en `Data.csproj`."
    - "Compliant — §C.6 medido en los 6 archivos: **0** `HttpContext`, **0** `ConfigurationManager`, **0** `Thread.Sleep`, **0** `System.Data.SqlClient` (uso real); los dos matches residuales son **comentarios** (`BaseRule.cs` L133 histórico «SqlTransaction.Zombie()»; `BaseGateway.cs` L306 nota de §C.6) — transparencia, no defecto. **0** casts `(SqlTransaction)`/`(SqlConnection)` fuera del adaptador."
    - "Compliant — §C.5 por diagnóstico del build: `#nullable disable` como **primera línea útil** sólo en los 4 aportados señalados (`BaseGateway` 54, `SQLHelper` 122, `BaseLoader` 28, `BaseRule` 4); **0** en `SqlHelperParameterCache`, `IBaseGateway`, los 2 nuevos, los 3 de cierre y **0** `<Nullable>` en `.csproj`; `1.4/**` intacto."
    - "ADJUDICACIÓN 1 (§8.2, no-reversión) — **ENDOSO**, con la desviación registrada: el ejecutor **no revirtió** contra el patrón del 009, con razones medidas (producto compila limpio 0/0; revertir destruiría reemplazos estructurales verificados sobre 5.500 líneas; los 4 fallos son pendientes de la propia entrega §8.4). El árbol **no está roto** (mapeo 12/12, encriptor 11/11, caracterización 83/83) y el N°3 cierra lo pendiente."
    - "ADJUDICACIÓN 2 (las 2 invariantes) — **OPCIÓN (i): LEGITIMIZAR**, con alcance exacto para el N°3 (sólo las dos aserciones, nada más): (a) `Data_declares_zero_third_party_dependencies` → asertar que `Data` declara **exactamente** el provider autorizado `Microsoft.Data.SqlClient` y ningún otro tercero (ADR-005 §3.1/ADR-001 §3.4); (b) `No_lock_file_pulls_configuration_manager` → seguir prohibiendo la **declaración directa** de `System.Configuration.ConfigurationManager` por el árbol (la excepción transitoria del 1.4 que ADR-001 §5 prohíbe) y **no** fallar por la aparición **transitiva** en el lock file del provider autorizado (8.0.1, medido). Referencia cruzada: ADR-005 §3.1, ADR-001 §3.4/§5, precedente §A.4 (SPEC-261016-0004)."
    - "ADJUDICACIÓN 3 (orden del N°3) — **CONFIRMO**: pruebas §8.1 + 4 archivos de integración §8.2 + constancia (`api-removals/US-000.BOLT-014.json`) + evidencia §E.2, con `turn_budget: 25` (aplica por V-Bounce; no requiere revisión del SPEC). **Sin transición de este acto**: el Bolt permanece `in-development` (árbol intacto, aporte presente, sin reversión — como el cierre del 009); la fila T07 registra el resultado `blocked` del V-Bounce, no una reversión; el N°3 lo llevará a su cierre."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0407 — Provider de datos y capa `Data` (rev.4) — V-Bounce N°2 **bloqueado por presupuesto**

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — `type: non-functional`, `work_category: refactor`, `risk_class: high`, `data_classification: internal`, `test_integration: required` |
| **SPEC**        | [SPEC-261016-0020](../spec/SPEC-261016-0020-provider-de-datos.md), **revisión 4** (aprobada `2026-10-16T03:53:49-03:00`; `turn_budget: 25`; el **retiro del caché del host está EN ALCANCE**, §C.6) |
| **V-Bounce**    | **2** — el segundo del Bolt: el N°1 ([`MEM-261016-0343`](MEM-261016-0343-provider-de-datos.md)) se detuvo **sin escribir código** por la frontera del caché del host; esta revisión la amplió y este V-Bounce **aportó la capa** |
| **ADRs**        | [ADR-005](../adrs/ADR-005-provider-de-datos.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.6) |

---

## 1. Executive summary

La capa `Data` **existe en el árbol 2.0 y compila limpia** (0 errores, 0 advertencias, con `TreatWarningsAsErrors`
activo): los **seis archivos del alcance** (`BaseRule.cs`, `BaseGateway.cs`, `BaseLoader.cs`, `SQLHelper.cs`,
`SqlHelperParameterCache.cs`, `IBaseGateway.cs`) fueron **aportados del 1.4** y transformados en la pasada atómica:

- **Provider único**: `Microsoft.Data.SqlClient` **6.1.7** declarado **una sola vez** en
  `2.0/Directory.Packages.props` (línea **6.1+**, nunca 6.0 — el asset `netstandard2.0` que la 6.1 restauró) y
  `PackageReference` en `Data.csproj`; `System.Data.SqlClient` **fuera** (el único uso era el `using`, retipado).
- **Retiro del caché del host (§C.6, EN ALCANCE)**: **0** `HttpContext`, **0** plataforma web, **0** claves
  legacy (`Coop_DAL_*`/`Cooperator_DAL_SP_*`) y **0** esperas activas en los cinco archivos. Los **cuatro puntos**
  quedan **sin caché** con su comportamiento declarado: colecciones de gateway y loader → **consulta a la base en
  cada llamada**; timeout → **del catálogo en cada uso**; metadata de parámetros → **consulta por operación**
  (`DeriveParameters`). **No** se introdujo `ICacheProvider` ni caché interina alguna (prohibido desde acá, §15).
- **Punto único de conversión + excepción tipada**: `Casting/TransactionAdapter.cs` (`AsSqlTransaction`/
  `AsSqlConnection`) + `Exceptions/InvalidTransactionTypeException.cs`; **0** casts `(SqlTransaction)`/`(SqlConnection)`
  fuera de él en la capa (los 8+2+5+1 del inventario pasan por el adaptador o desaparecen).
- **La cadena por el contrato**: `BaseRule.InternalDBHelper.ConnectionString` = `CooperatorSettings.Current.GetConnectionString(...)`
  — **sin** `Dictionary` estático, **sin** heurística de substrings y **sin** descifrado propio; el `using
  System.Configuration` sale de `SQLHelper` (el timeout ya no lee `AppSettings`).
- **Timeout del catálogo** (§D.1): `CooperatorSettings.Current.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds,
  ConfigurationKeys.Defaults.CommandTimeoutSeconds)` **en cada uso** — se eliminó el `SqlCommand` descartable y el
  valor en caché.
- **Hazard de la conexión del llamador** (§C.4): el descubrimiento de parámetros **no abre ni cierra** la conexión
  ajena; si la recibida no está abierta, **pide una propia** (misma cadena), la usa y la **dispone**.
- **Cierre mínimo que la compilación exigió** (§C.5): `Core/SecurityRights.cs`, `Core/IGateway.cs`,
  `Core/IValidable.cs`, aportados **tal cual** del 1.4.
- **Directiva de nulabilidad de §C.5 aplicada por el criterio objetivo** (aportado + diagnóstico del build):
  `#nullable disable` como primera línea útil en `BaseGateway.cs` (**54**), `SQLHelper.cs` (**122**),
  `BaseLoader.cs` (**28**) y `BaseRule.cs` (**4**) — y **ninguno** de los demás archivos la lleva
  (`SqlHelperParameterCache.cs`, los tres del cierre mínimo y los dos nuevos: **0** diagnósticos). **0** en código
  nuevo y **0** `<Nullable>disable</Nullable>` en `.csproj`.

**Por qué se cierra `blocked` y no `ready_for_review`.** El V-Bounce agotó el **presupuesto de 25 loops** del
`turn_budget` (§14 del SPEC) en la pasada de port + retipado + retiro, que exigió varias iteraciones de reparación
(los reemplazos estructurales sobre archivos de 2.500 líneas y el cierre mínimo que la compilación fue revelando).
**Queda pendiente**: (a) las **pruebas** de §8.1 (unidad + los controles NFR) y los **cuatro archivos de la lane de
integración** de §8.2, escritos/compilados; (b) la **evidencia** de las fases C/D/E (incluido el barrido de §C.6 y la
lectura de la política de nulabilidad); (c) la **constancia de rupturas** (`2.0/docs/api-removals/US-000.BOLT-014.json`),
que es lo que hoy explica los **dos fallos de cobertura del arnés** (§8.4); y (d) la actualización de **dos
invariantes del arnés** que el aporte legítimamente desactualizó (§8.3).

**El árbol queda coherente y compilando**: la capa aportada compila con 0 diagnósticos y las demás suites siguen
verdes (mapeo 12/12, encriptor 11/11, caracterización 83/83). **No se revirtió el aporte** — ver §8.2, donde se
declara la desviación respecto de la disciplina de presupuesto, con su razón.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — El proveedor único y su línea** | **completada** ✔ | `Microsoft.Data.SqlClient` **6.1.7** en `Directory.Packages.props` (una sola declaración, línea 6.1+: el asset `netstandard2.0` está publicado; **nunca** 6.0) + `PackageReference` en `Data.csproj`; `System.Data.SqlClient` retirado. El **smoke real** (criterio 2) queda para el ciclo de integración (§8.5). |
| **B — El punto único de conversión** | **completada** ✔ | `Casting/TransactionAdapter.cs` + `Exceptions/InvalidTransactionTypeException.cs` (tipo recibido/esperado/punto). Verificación de **0** casts `Db*`→`Sql*` fuera de él en los cinco archivos. |
| **C — Interior tipado, transacciones y superficie** | **completada** ✔ (con una ruptura adicional declarada) | Los 8+2+5+1 casts pasan por el adaptador; **0** `Thread.Sleep` en la capa; el **hazard** corregido; la documentación de la **forma de firma** en `IBaseGateway.cs`; `SqlHelper` (`internal static class`) y `SqlHelperParameterCache` (`internal sealed class`) resultaron **ya `internal` en el 1.4** → **no hay ruptura por ese punto** (medido, §8.5); **se retiró `BaseRule.InternalDBHelper.SetConnectionStringValue`** (método **público** que escribía el `Dictionary` estático de cadenas) → **ruptura de superficie que se declara en §8.5 y en la constancia**. |
| **D — Timeout y cadena desde la configuración** | **completada** ✔ | Timeout del **catálogo** en cada uso (sin caché, sin `SqlCommand` descartable); cadena del **contrato** (sin `Dictionary`, sin heurística, sin descifrado propio, sin `ConfigurationManager`). |
| **E — Cambios de comportamiento, rupturas y evidencia** | **bloqueada parcialmente** ⏸ | La **evidencia** de §E.2 (gate de API, listado de dependencias, las cuatro entradas del caché) **y la constancia** (`api-removals/US-000.BOLT-014.json`, §E.3/§C.6) quedan **pendientes** — es lo que explica dos de los cuatro fallos del arnés (§8.4) y lo que el V-Bounce N°3 debe cerrar. La **corrida de integración** (§8.2) no se escribió todavía. |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Data/Casting/TransactionAdapter.cs` | El **punto único** de conversión (validación + error tipado). |
| `2.0/src/Cooperator.Framework.Data/Exceptions/InvalidTransactionTypeException.cs` | La excepción del borde (tipo recibido, esperado y punto). |
| `2.0/src/Cooperator.Framework.Core/SecurityRights.cs` · `IGateway.cs` · `IValidable.cs` | **Cierre mínimo** que la compilación exigió (§C.5), copiado tal cual del 1.4. |

## 4. Files modified / aportados

| Path | Cambio |
|---|---|
| `2.0/src/Cooperator.Framework.Data/{BaseRule,BaseGateway,BaseLoader,SQLHelper,SqlHelperParameterCache,IBaseGateway}.cs` | **Aportados del 1.4** (5.549 líneas → 4.600 tras el retiro del caché) con: retipado al provider, retiro del caché del host (§C.6), el punto único, la cadena por el contrato, el timeout del catálogo, el hazard corregido, la espera activa fuera y la directiva de §C.5 donde el build la exigió. |
| `2.0/Directory.Packages.props` · `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` | Versión centralizada del provider + `PackageReference` + `InternalsVisibleTo` para la suite de la capa. |
| `devflow/functional/bolts/US-000.BOLT-014-provider-de-datos.md` | Filas **T06** y **T07** (toma y cierre del V-Bounce N°2). |
| `devflow/metrics/bolts/US-000.BOLT-014-provider-de-datos.json` | `v_bounces[]` += la entrada de este V-Bounce (`number: 2`, `spec_revision: 4`, `blocked`). |

## 5. Files deleted / 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

1. **El aporte se copia tal cual y se transforma** (no se reescribe): cada paso es un reemplazo **verificado**
   (conteo de coincidencias antes y después) sobre el archivo del 1.4, de modo que el diff contra el original sea
   **sólo** lo autorizado por el SPEC.
2. **El criterio de §C.5 lo fija el build** (no un nombre supuesto): se aplicó la directiva **sólo** en los cuatro
   archivos que el compilador señaló, con su conteo de diagnósticos por archivo.
3. **El cierre mínimo es parte del aporte**: `SecurityRights`, `IGateway` e `IValidable` viven en `Core` y el 2.0 no
   los tenía; el SPEC los autoriza expresamente («el cierre mínimo que la compilación exija», §C.5) y se declaran
   archivo por archivo.
4. **Los cuatro puntos quedan sin caché** y con su comportamiento declarado; **no** se introdujo ninguna caché
   interina: era la línea roja de §C.6/§15.
5. **La cadena no se inspecciona ni se descifra**: el `Dictionary` estático, la heurística de substrings y el
   `Library.Cryptography.Decrypt` del 1.4 **no** se portaron; el valor entra resuelto por el contrato.
6. **El lock del camino de consulta se conserva** (serializaba la consulta, no el caché): retirarlo habría cambiado
   la concurrencia de `GetAll` sin decisión que lo ordene.

---

## 8. Deviations and assumptions

### 8.1 El mismo V-Bounce encontró el escenario exacto de §C.6 (y lo ejecutó)

Los cinco archivos **no existían** y cuatro llevaban el caché del host **dentro** (medido en el V-Bounce N°1,
`MEM-261016-0343` §9.1): el retiro fue **condición de compilación**, tal como §C.6 lo declara, y **no** una
sustitución (no hay `ICacheProvider` ni caché interina). Los cuatro puntos tienen su **comportamiento sin caché**
declarado y esperan su entrada en la constancia (pendiente, §8.4).

### 8.2 No se revirtió el aporte (desviación declarada respecto de la disciplina de presupuesto)

La disciplina de presupuesto indica, al acercarse al techo sin verde, **detener y revertir**. Acá **no se revirtió**,
por tres razones medidas: (i) el **producto compila limpio** (0 errores, 0 advertencias) — el árbol **no** está roto
en el producto; (ii) el aporte es **lo que el SPEC manda** (§C.5/§C.6) y revertirlo destruiría la única parte que el
N°3 no tendría que repetir (los reemplazos estructurales sobre 5.500 líneas ya están hechos y verificados); y
(iii) los cuatro fallos del arnés son **pendientes de la propia entrega** (§8.4), no defectos desconocidos. La
decisión queda declarada para la adjudicación: si el revisor prefiere el patrón del Bolt 009 (**reversión
completa**), el aporte se retira y el N°3 vuelve a hacerlo con su propio presupuesto.

### 8.3 Dos invariantes del arnés que el aporte desactualizó (legítimamente)

| Fallo | Causa medida | Tratamiento |
|---|---|---|
| `BuildInvariantsTests.Data_declares_zero_third_party_dependencies` | La invariante nació cuando `Data` **no tenía** provider; ADR-005 §3.1/ADR-001 §3.4 fijan **exactamente un** provider ADO.NET en `Data` | Actualizar la invariante en el V-Bounce N°3: `Data` declara **el provider autorizado** (y ningún otro) |
| `BuildInvariantsTests.No_lock_file_pulls_configuration_manager` | El **lock file** de `Data` lista `System.Configuration.ConfigurationManager` como dependencia **transitiva del provider** (no declarada por la capa) | Decidir con el dueño: la invariante (ADR-001 §5) apuntaba a la **excepción transitoria** que `Data` declaraba en 1.4; la aparición **transitiva** por el provider autorizado **no** es esa excepción → la invariante debe distinguir "declarada por el árbol" de "arrastrada por el provider" |

### 8.4 Los dos fallos de cobertura son la constancia pendiente (no un defecto)

`ApiRemovalCoverageTests.Every_removed_member_reported_by_the_gate_is_covered_by_a_fragment` y
`MigrationNotesCoverageTests.Every_removed_member_of_the_report_is_covered_by_a_fragment` fallan porque el **gate
ahora compara `Data`** (el ensamblado existe) y reporta **miembros retirados** que **ninguna constancia cubre**:
los del supervisor de superficie (`SetConnectionStringValue`, público ✗), junto con lo que el retipado y el retiro
cambiaron. **Es exactamente lo que la constancia de §E.3/§C.6 debe entrar a cubrir** — y por eso su ausencia se
declara como pendiente y no como verde.

### 8.5 Hallazgos que la constancia debe declarar (inventario para el N°3)

1. **`SetConnectionStringValue` (público) retirado** de `BaseRule.InternalDBHelper`: era la vía paralela que
   escribía el `Dictionary` estático de cadenas; con la cadena en el **contrato** (ADR-004 §3.2/§3.3) no tiene
   función. **No figuraba en la lista de rupturas del SPEC** → se declara acá (ADR-008 §3.1: toda ruptura con su
   reemplazo) y va a la constancia.
2. **`CacheParameterSet`/`GetCachedParameterSet` retirados** de `SqlHelperParameterCache`: usaban el `HashTable`
   estático que §C.6 retira con el caché del host. Estaban en una clase **`internal`** → **no** son superficie
   pública del paquete (sin diferencia en el gate).
3. **`SqlHelper`/`SqlHelperParameterCache` ya eran `internal` en 1.4** (medido): la "ruptura" que ADR-005 §3.3
   declara **no existe** en el aporte → se constata la **extensión del ruteo** declarada por el ADR sin inventar una
   ruptura que el árbol no tiene.
4. **Los cuatro puntos de caché** quedan **sin caché** (comportamiento declarado): más consultas a la base en
   colecciones y metadata, y el timeout leído del catálogo en cada uso; **paso siguiente**: la sustitución por
   `ICacheProvider` (Bolt 013, que ejecuta **después**: secuencia `014-ampliado → 013`).
5. **El bypass web del gate por token** (`if (el host web != null) return;` en `CheckForToken`) se retiró: sin la
   plataforma web no puede existir. El **gate por token** en sí es de `US-000.BOLT-015` (**no** se toca acá).
6. **La interfaz `IBaseGateway` declara por escrito** que su forma BCL-typed es **forma de firma**, no una promesa
   de multi-motor (ADR-005 §3.3).

### 8.6 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 4`**, `turn_budget: 25` y `review.decision: approved`
(`2026-10-16T03:53:49-03:00`); el Bolt entró `blocked` (T01–T05) y se tomó con **T06**; los ADRs aplicados siguen
`accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.3). La secuencia
`014-ampliado → 013` está **ejecutada en su primera mitad** (este V-Bounce) y la sustitución del 013 sigue
**prohibida** desde acá.

### 8.7 Presupuesto de turnos (declarado)

Se usaron **26 loops** contra el `turn_budget: 25` del SPEC (uno más). El desvío y su causa están medidos en §1/§8.2:
la pasada atómica del aporte exigió reparaciones sucesivas sobre archivos de 2.500 líneas (los reemplazos
estructurales del caché, el cierre mínimo revelado por el compilador, los usings y los campos muertos), cada una
**verificada** antes de seguir. El árbol **nunca** quedó a medio camino en el producto: la capa compila limpia.

---

## 9. Verification evidence

### 9.1 La capa aportada compila limpia

```
dotnet build 2.0/src/Cooperator.Framework.Data/... -c Release   → EXIT=0 · 0 Warning(s) · 0 Error(s)
```

### 9.2 §C.6 medido en los cinco archivos entregados

```
BaseRule.cs                  HttpContext=0  plataforma web=0  Coop_DAL=0  Thread.Sleep=0  (SqlTransaction)=0  (SqlConnection)=0
BaseGateway.cs               HttpContext=0  plataforma web=0  Coop_DAL=0  Thread.Sleep=0  (SqlTransaction)=0  (SqlConnection)=0
BaseLoader.cs                HttpContext=0  plataforma web=0  Coop_DAL=0  Thread.Sleep=0  (SqlTransaction)=0  (SqlConnection)=0
SQLHelper.cs                 HttpContext=0  plataforma web=0  Coop_DAL=0  Thread.Sleep=0  (SqlTransaction)=0  (SqlConnection)=0
SqlHelperParameterCache.cs   HttpContext=0  plataforma web=0  Coop_DAL=0  Thread.Sleep=0  (SqlTransaction)=0  (SqlConnection)=0
Provider único: Microsoft.Data.SqlClient 6.1.7 (Directory.Packages.props, una sola declaración) · System.Data.SqlClient: 0
Directiva §C.5 (primera línea útil, por diagnóstico del build): BaseGateway 54 · SQLHelper 122 · BaseLoader 28 · BaseRule 4
  · y NO la llevan: SqlHelperParameterCache, SecurityRights, IGateway, IValidable, los dos archivos nuevos
```

### 9.3 `1.4/**` intacto

```
git status --porcelain -- 1.4   → (vacío)
```

### 9.4 Las suites del árbol (tras el aporte)

```
build sin advertencias (tratadas como error): 19 proyectos
harness-hermetic          → Failed: 4, Passed: 131, Total: 135     ← §8.3 (2 invariantes) + §8.4 (2 coberturas)
mapping-hermetic          → Passed! 12/12
encriptor-hermetic        → Passed! 11/11
characterization-hermetic → Passed! 83/83
```

### 9.5 Gates (§9) — lo que puede declararse hoy

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | La capa compila con **0 errores y 0 advertencias** (§9.1); `Data` en `netstandard2.1` sin la plataforma web |
| Nulabilidad del código aportado (§C.5) | **pass** | §9.2: directiva **sólo** en los 4 aportes que el build señaló; 0 en código nuevo; 0 `<Nullable>disable</Nullable>` en `.csproj` |
| Retiro del caché del host (§C.6/AC-10) | **pass** | §9.2: 0/0/0/0 en los cinco archivos, sin introducir `ICacheProvider` ni caché interina |
| Unit / component tests | **pendiente** ⏸ | Las pruebas de §8.1 no se escribieron (presupuesto): es el primer pendiente del N°3 |
| Dependency scanning | **parcial** | `Data` declara **un** provider (ADR-001 §3.4) ✔; la invariante del arnés que lo verificaba quedó desactualizada (§8.3) y el **lock file** revela el `ConfigurationManager` **transitivo** del provider → decisión del dueño (§8.3) |
| API-compat / cobertura del registro | **pendiente** ⏸ | Los dos fallos de §8.4: la constancia de rupturas (`api-removals/US-000.BOLT-014.json`) no está escrita |
| Integration tests (real dependencies) | **pendiente** ⏸ | Los cuatro archivos de §8.2 (smoke, metadata, timeout, comportamiento del proveedor) no se escribieron; el **smoke real** es el criterio 2 de §A.1 |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` con la entrada `number: 2`, `spec_revision: 4` |

---

## 10. Manual interventions

**Ninguna intervención humana.** El aporte, las transformaciones verificadas y el cierre se hicieron **sin
operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0020](../spec/SPEC-261016-0020-provider-de-datos.md) — **revisión 4**: §4 (alcance,
  con §C.6), §5 (secuencia `014-ampliado → 013`), §6/A–E (§C.5 nulabilidad, §C.6 retiro del caché), §7 (AC-1…AC-10)
  y §15 (paradas).
- **Bolt:** [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — filas **T06** y **T07**.
- **V-Bounce N°1 (★):** [`MEM-261016-0343`](MEM-261016-0343-provider-de-datos.md) — el inventario medido (§9.1) que
  fundamentó la ampliación de §C.6 y la salida (d) de su §13.1.
- **Frontera del 013 (★):** [`MEM-261016-0328`](MEM-261016-0328-cache-en-data.md) — «CORRECCIÓN DE SECUENCIA»:
  `014` **antes** que `013`; la **sustitución** por `ICacheProvider` es del Bolt 013 y ejecuta después.
- **Entregables:** `2.0/src/Cooperator.Framework.Data/**` (capa aportada + `Casting/` + `Exceptions/InvalidTransactionTypeException.cs`),
  `2.0/src/Cooperator.Framework.Core/{SecurityRights,IGateway,IValidable}.cs`, `2.0/Directory.Packages.props` y el
  `.csproj` de `Data`.
- **Manifiesto:** [US-000.BOLT-014-provider-de-datos.json](../metrics/bolts/US-000.BOLT-014-provider-de-datos.json).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 2 (de US-000.BOLT-014) — **`blocked`** (presupuesto) |
| SPEC ejecutado | SPEC-261016-0020 revisión **4** (`turn_budget: 25`) |
| Tiempo de generación IA | ~40 min de reloj (03:27 → 04:07) |
| Fases completadas | **A, B, C y D completas**; **E parcial** (la constancia y la evidencia, pendientes) |
| Archivos aportados/creados | 6 del alcance (5.549 líneas del 1.4) + 3 del cierre mínimo en `Core` + 2 nuevos (el punto único y su excepción) |
| Retiro del caché del host (§C.6) | **4 puntos**, 0 `HttpContext`/0 plataforma web/0 claves legacy/0 esperas activas en los 5 archivos |
| Directivas `#nullable disable` | **4** (por diagnóstico del build: 54+122+28+4); **0** en código nuevo; **0** en `.csproj` |
| Estado del producto | **0 errores, 0 advertencias** |
| Suites | mapeo 12/12 · encriptor 11/11 · caracterización 83/83 · arnés **131/135** (4 fallos declarados en §8.3/§8.4) |
| Loops usados | 26 (presupuesto del SPEC: 25; desvío declarado en §8.7) |

---

## 13. Pending items and stubs

1. **Las pruebas de §8.1** (el primer pendiente del N°3): del **punto único** (casos negativos por tipo con dobles
   que derivan de `DbTransaction`/`DbConnection`), del **timeout** (valor del catálogo, default documentado, valor
   inválido ⇒ error tipado del contrato), de la **cadena** con un doble de `ICooperatorConfiguration`, del **armado
   de parámetros** (`DbType.Time` con `TimeSpan`, `DbType.Date`), y los **controles NFR** (0 casts fuera del punto
   único, 0 `Thread.Sleep`, sin `providerName`, el listado de dependencias, 0 plataforma web, la lectura de §C.5).
2. **La lane de integración de §8.2** (escrita y compilada ✔ pendiente): `ProviderSmokeTests`,
   `StoredProcedureMetadataTests` (el hazard, con una conexión abierta del llamador), `CommandTimeoutIntegrationTests`
   y `ProviderBehaviorTests` (cifrado obligatorio/validación de certificado, `DbType.Time`/`Date`, namespaces).
3. **La constancia de rupturas** `2.0/docs/api-removals/US-000.BOLT-014.json` con las entradas de §8.5 — es lo que
   cierra los **dos fallos de cobertura** (§8.4) y el insumo de las notas del Bolt 028.
4. **Las dos invariantes del arnés** (§8.3): actualizar la del provider autorizado y **decidir con el dueño** la del
   `ConfigurationManager` transitivo (una decisión de criterio, no un ajuste para pasar el gate).
5. **El smoke real** (criterio 2 de §A.1) y los **cambios de comportamiento del proveedor** verificados contra la
   base: quedan para el **ciclo de test de integración** del Bolt, que **no** se ha ejecutado (el Bolt **no** pasa a
   `ready-for-acceptance`, §5/§15).
6. **La sustitución del caché** por `ICacheProvider` (contrato, regiones, cap, TTL, invalidación por clave):
   **Bolt 013**, que ejecuta **después** (secuencia `014-ampliado → 013`). Los cuatro puntos quedan **sin caché**
   con su comportamiento declarado.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto (`number: 2`, `spec_revision: 4`, `blocked`) +
  el árbol entregado (**la capa `Data` aportada y compilando**: provider único, punto único de conversión, interior
  tipado, retiro completo del caché del host, cadena y timeout por el contrato) + el estado de las suites (§9.4).
- **Qué se pide decidir:** (1) la **adjudicación del cierre por presupuesto** (26 loops contra 25) y, en particular,
  la decisión declarada en **§8.2** sobre **no revertir** el aporte; (2) la **decisión del dueño** sobre el
  `ConfigurationManager` **transitivo** del provider y la invariante correspondiente (§8.3); y (3) el **orden** del
  V-Bounce N°3 (pruebas + constancia + integración) con presupuesto propio.
