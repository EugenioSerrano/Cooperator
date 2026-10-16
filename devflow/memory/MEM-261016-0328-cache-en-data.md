---
id: "MEM-261016-0328"
title: "Caché en `Data` (SPEC rev.2) — V-Bounce N°1: BLOQUEADO por dependencia de secuencia (la superficie base de `Data` no está en el árbol y aportarla es el aspecto del Bolt 014)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-013" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0019" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-004", "ADR-001"] # ningún cambio los contradice: el V-Bounce se detuvo por una condición de parada del SPEC (§5/§15) antes de escribir código
manifest: "US-000.BOLT-013-cache-en-data.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:28:18-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T03:31:06-03:00"
  decided_at: "2026-10-16T03:31:06-03:00"
  findings:
    - "Compliant — bloqueo por dependencia de secuencia REPRODUCIDO: `2.0/src/Cooperator.Framework.Data` tiene **16 `.cs` todos en `Exceptions/`** (aportados por el Bolt 007) y `BaseGateway.cs`/`BaseLoader.cs`/`SQLHelper.cs`/`SqlHelperParameterCache.cs` están **ausentes**; en 1.4 los conteos del MEM coinciden (muestreado: `SQLHelper.cs` `SqlConnection`×123, `BaseGateway.cs` `HttpContext`×20). La precondición de §5 no se cumple y aportar la superficie es el aspecto del 014."
    - "Compliant — las citas del SPEC coinciden: §5 («el proyecto `Cooperator.Framework.Data` … **con la superficie base y sus puntos de caché actuales**; si no existe, **parar y reportar**: este Bolt **no crea la capa de datos**») y §15 («Al aportar aparece una dependencia de secuencia …: **parar y escalar la secuencia** … **nunca** aportar ni transformar código de otro Bolt (G11)», más la condición del aspecto del 014). Ambas se activaron correctamente."
    - "Compliant — **0** aporte/transformación: 0 archivos de producto creados/modificados/retirados; `Data` intacta con sus 16; suite **218/218 verde** después del cierre; `1.4/**` intacto; el registro de rupturas **no cambia** (ADR-004 §3.1 sigue correctamente `pendiente`, con su evidencia aún verdadera)."
    - "ADJUDICACIÓN (a)/(b) — **ENDOSO (b)**: re-encolar `US-000.BOLT-013` **después** del `US-000.BOLT-014` **sin cambiar el SPEC**. No parte el Bolt, no obliga a revisión material, y el valor completo de la caché sólo se cierra cuando los puntos de caché de la capa existan y la consuman (§7.2-a). La opción (a) queda disponible y es la de menor latencia si se quiere el contrato en el árbol antes."
    - "CORRECCIÓN DE SECUENCIA (constancia para el 014): **`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`**. La dependencia declarada del 014 sobre el 013 era **precautoria de doble-toque** sobre los archivos compartidos (`SQLHelper.cs`/`SqlHelperParameterCache.cs`); con el bloqueo medido, la secuencia correcta es **014 → 013**: el 014 aporta/retypea la capa `Data` (proveedor, casts, tipado del interior, hazard de la conexión) y el 013 sustituye los **puntos de caché DESPUÉS**, sobre la capa ya presente. La ejecución del 014 citará esta constancia."
    - "Compliant — el inventario de puntos de caché del 1.4 con sus dependencias (§9.1) queda asentado como insumo del V-Bounce N°2; cierre T04/T05; MEM único; manifiesto válido (v_bounces[0] `blocked`, spec_revision 2)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0328 — Caché en `Data` (rev.2) — V-Bounce N°1 **bloqueado**

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-013](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — `type: non-functional`, `work_category: refactor`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0019](../spec/SPEC-261016-0019-cache-en-data.md), **revisión 2** (aprobada `2026-10-16T03:17:26-03:00`) |
| **V-Bounce**    | **1** — el primero del Bolt: se detuvo por **condición de parada del SPEC** (§5 y §15) **antes** de escribir código |
| **ADRs**        | [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.3) |

---

## 1. Executive summary

El V-Bounce se **detuvo sin escribir código de producto** por una **dependencia de secuencia**: la precondición
de §5 —«el proyecto `Cooperator.Framework.Data` en el árbol 2.0 (`netstandard2.1`) **con la superficie base y sus
puntos de caché actuales**»— **no se cumple**, y aportar esa superficie es el **aspecto del `US-000.BOLT-014`**
(proveedor de datos, casts, tipado del interior, hazard de la conexión del llamador), no de este Bolt.

Medido sobre el árbol y sobre la línea base congelada:

```
2.0/src/Cooperator.Framework.Data:  16 archivos .cs — TODOS en Exceptions\ (aportados por el Bolt 007)
  BaseGateway.cs            en 2.0: AUSENTE   ·  en 1.4: presente (1557 líneas)
  BaseLoader.cs             en 2.0: AUSENTE   ·  en 1.4: presente ( 655 líneas)
  SQLHelper.cs              en 2.0: AUSENTE   ·  en 1.4: presente (2532 líneas)
  SqlHelperParameterCache.cs en 2.0: AUSENTE  ·  en 1.4: presente ( 269 líneas)
  (BaseRule.cs: también ausente — del Bolt 014/ADR-004 §3.3)

Dependencias de esos archivos del 1.4 (las que obligarían a portar el proveedor clásico, aspecto del Bolt 014):
  SQLHelper.cs               SqlConnection×123 · SqlParameter×135 · SqlCommand×102 · SqlTransaction×81 ·
                             SqlDataReader×36 · SqlDataAdapter×9 · System.Data.SqlClient · ConfigurationManager
  SqlHelperParameterCache.cs SqlParameter×35 · SqlConnection×16 · SqlCommand×3 · System.Data.SqlClient
  BaseGateway.cs             DbTransaction×26 · HttpContext×20 · System.Web×5 · SqlTransaction · DbConnection
  BaseLoader.cs              DbConnection×7 · DbTransaction×13 · HttpContext×19 · System.Web×5
```

§6/B.1 dice **«Files modified»** sobre esos cuatro archivos: la sustitución de los **tres puntos de caché** —el
objeto del Bolt (§2 de su Bolt y §1 del SPEC)— sólo puede ocurrir **modificando** archivos que hoy **no existen**
en el 2.0, y crearlos exige portar `SqlConnection`/`SqlCommand`/`SqlParameter`… es decir, **el proveedor y el
tipado del interior**, que §4 y §14 declaran explícitamente del **Bolt 014**. El SPEC previó este desenlace en su
**§15**: «*Al aportar aparece una dependencia de secuencia con otro Bolt …: **parar y escalar la secuencia**. La
salida es una decisión de orden del dueño y, si cambia el plan, una **revisión material de este SPEC**; **nunca**
aportar ni transformar código de otro Bolt (G11)*».

**Qué se hizo y qué no.** Se verificaron las precondiciones (SPEC `approved` rev.2 ✔; Bolt `specified` con T01–T03
✔; contrato de configuración del Bolt 011 **presente** ✔ con la clave `Cooperator:Cache:MaxEntries` **publicada**
✔; suite hermética **verde 218/218** ✔; suite de caracterización presente ✔), se levantó el **inventario de los
puntos de caché** del 1.4 con sus dependencias y se **paró** ✔. **No** se aportó ni transformó ningún archivo del
1.4 ✔, **no** se tocó ningún archivo del Bolt 014 ✔, **no** se escribió la implementación de caché (ver §7.2: es
la parte que el dueño puede decidir adelantar) y **no** se tocó `1.4/**` ✔. El árbol queda **exactamente** como
estaba: la suite sigue verde (§9.3) y el registro de rupturas **no** cambia (la declaración de ADR-004 §3.1 sigue
correctamente en `pendiente`, con su evidencia aún verdadera: «el `ICacheProvider` de `Data` todavía no está en el
árbol»).

---

## 2. Implemented phases

| Fase de §6 | Estado | Por qué |
|---|---|---|
| **A — El contrato de caché y su implementación en memoria** | **no iniciada** ✗ | Es **dependency-free** (contrato + implementación en memoria + regiones, BCL puro: no necesita la capa aportada) y por eso es la **opción de escalación más barata** (§7.2-a). No se ejecutó porque §15 reserva al **dueño** la decisión de **cambiar el plan** (partir el Bolt) y su posible **revisión material del SPEC**; adelantarla por decisión propia sería fijar la secuencia sin la decisión. |
| **B — Los tres puntos de caché de la capa** | **bloqueada** ✗ | §6/B.1 son **«Files modified»** de archivos que **no existen** en el 2.0. Aportarlos = portar el **proveedor clásico** = aspecto del **Bolt 014** (§4/§14), prohibido por §15 (G11). |
| **C — Retiro del caché del host y de la plataforma web** | **bloqueada** ✗ | Los `HttpContext`/`System.Web` a retirar viven **dentro** de esos cuatro archivos del 1.4 (20 y 19 usos en `BaseGateway`/`BaseLoader`): el retiro no tiene destinatario hasta que la capa exista en el 2.0. |
| **D — TTL por región y cap desde el catálogo** | **bloqueada** ✗ (por B) | La lectura del catálogo es de `CacheRegions.cs` (fase A, dependency-free) y su **consumo** por los puntos de caché depende de B. El **catálogo ya publica** `Cooperator:Cache:MaxEntries` ✔ y los dos TTL ✔ (C.1 de §D.1 **no** es un bloqueo). |
| **E — Evidencia y cambios de comportamiento declarados** | **bloqueada** ✗ (por B/C) | La evidencia que pide §E.1 (build de la capa sin la plataforma web, inventario con cada punto resuelto, `ResetCache()` removiendo sus claves, caracterización antes y después) presupone la capa sustituida. Lo que **sí** quedó levantado es el **inventario del 1.4** con sus dependencias (arriba), que es el insumo de esa evidencia. |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `devflow/memory/MEM-261016-0328-cache-en-data.md` | Este MEM (exactamente uno por V-Bounce). |

> **Ningún archivo de producto, de prueba o de documentación del árbol 2.0 fue creado en este V-Bounce.**

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-013-cache-en-data.md` | `status: specified` → `in-development` → **`blocked`** + filas **T04** y **T05** | La toma por G11 y el cierre del V-Bounce con su bloqueo. |
| `devflow/metrics/bolts/US-000.BOLT-013-cache-en-data.json` | `v_bounces[]` += la entrada de este V-Bounce (`number: 1`, `spec_revision: 2`, `blocked`) | El registro del resultado (§3.12). |

**No se tocó ningún archivo del Bolt 014** (ni `BaseRule.cs`, ni el `csproj` de `Data`, ni la referencia web: todo
eso es de la capa que el 014 porta) **ni ningún artefacto del registro** (sus filas siguen correctas: §1).

---

## 5. Files deleted / 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

### 7.1 Parar antes de escribir, y no repartir el Bolt por decisión propia

§15 tiene una condición de parada **específica** para este desenlace y dice qué se hace: *parar y escalar la
secuencia*, con la salida en manos del **dueño** («una decisión de orden del dueño y, si cambia el plan, una
revisión material de este SPEC») y con una prohibición explícita: **nunca** aportar ni transformar código de otro
Bolt (G11). Aportar `BaseGateway.cs`/`BaseLoader.cs`/`SQLHelper.cs`/`SqlHelperParameterCache.cs` habría sido
exactamente eso: portar el **proveedor clásico** y el **tipado del interior** que §4/§14 atribuyen al Bolt 014, con
`System.Web` y `ConfigurationManager` incluidos. La consecuencia práctica de no parar habría sido peor que el
bloqueo: dos Bolts escribiendo la misma capa con dos evidencias mezcladas, contra la frontera que el propio SPEC
declaró.

### 7.2 Lo que **no** se hizo, para que la decisión del dueño sea informada

- **(a) Phase A es dependency-free.** `ICacheProvider`, `CacheKey`, `CacheRegions` e `InMemoryCacheProvider`
  (§6/A.1–A.2) no tocan la capa: son contrato + implementación en memoria + regiones, con la nulabilidad activa y
  sin dependencias nuevas. Si el dueño decide **partir el Bolt**, esa parte puede entregarse en un V-Bounce N°2
  **sin esperar al 014**, y su evidencia (cap, vencimiento, concurrencia, TTL por región con doble de
  configuración) es la que §8.1 pide — pero **su** valor completo (AC-1/AC-2/AC-6/AC-7) sólo se cierra cuando los
  puntos de caché de la capa existan y la consuman.
- **(b) El catálogo del Bolt 011 no bloquea.** Las tres claves que §D.1 necesita están publicadas
  (`Cooperator:Cache:DefaultTtlSeconds`, `…:ParameterMetadataTtlSeconds` y **`…:MaxEntries`**), así que la parada
  de §15 «el catálogo no publica una clave» **no** aplica y no hay nada que pedirle al Bolt 011.
- **(c) El contrato de configuración tampoco.** `ICooperatorConfiguration` y `CooperatorConfigurationException`
  están en el árbol (Bolt 011), con lo que el *Config fail-fast* y la lectura de TTL/cap son ejercitables ya.

### 7.3 La vía que **no** se toma

Reasignar a este Bolt el porteo de la capa de datos (o "aprovechar" y portar un archivo de paso) queda descartado
por §15 y por §14 (frontera de aspecto): mezclaría dos aspectos en una misma evidencia y dejaría al Bolt 014 sin su
objeto. Si el dueño concluyera que la secuencia correcta es **014 primero**, este Bolt se re-ejecuta después con la
capa ya presente, sin cambiar ni una línea de su SPEC.

---

## 8. Deviations and assumptions

### 8.1 La precondición de §5 y el encabezado de §6 no dicen lo mismo, y manda §5 + §15

El **encabezado de §6** («Mecánica de build del aporte del 1.4») habla de «cualquier archivo que este Bolt
**aporte** del 1.4 —los de §B.1 …», lo que sugiere que el aporte está previsto; pero **§5** fija la precondición
—«el proyecto `Cooperator.Framework.Data` … **con la superficie base y sus puntos de caché actuales**; si no
existe, **parar y reportar**: este Bolt **no crea la capa de datos**»— y **§B.1** los lista como **Files
modified** (no *created*). La lectura ejecutada es la de **§5 + §15** (la condición de parada específica para la
dependencia de secuencia), que es la única compatible con la frontera de aspecto de §4/§14 y con la prohibición de
G11. Se registra acá porque es el punto que el revisor debe adjudicar: **no** es una ambigüedad que se resuelva
portando de más.

### 8.2 Lo que este V-Bounce **sí** dejó como trabajo útil

El **inventario de los puntos de caché del 1.4 con sus dependencias** (§1 y §9.1). Es exactamente el insumo que
§6/B.1 y §E.1 piden para sustituir los tres puntos, y ahora está medido: no es una impresión, es el conteo de usos
por archivo que prueba *por qué* el aporte no puede hacerse desde este Bolt sin pisar al 014.

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 2`** y `review.decision: approved` (`2026-10-16T03:17:26-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.3). El bloqueo **no** proviene de un cambio
de fuente: proviene del **orden de ejecución** entre Bolts.

### 8.4 Presupuesto de turnos (declarado)

Este V-Bounce usó **3 loops** (guía 10): verificación de precondiciones + lectura del SPEC completo, relevamiento
de la dependencia y cierre. No hubo iteraciones de convergencia porque no hubo implementación: el bloqueo se
detectó en el relevamiento.

---

## 9. Verification evidence

### 9.1 El inventario que prueba el bloqueo

```
Data en 2.0:        16 .cs, todos en Exceptions\  ·  csproj netstandard2.1 con ProjectReference a Library
Archivos de §B.1:   BaseGateway.cs · BaseLoader.cs · SQLHelper.cs · SqlHelperParameterCache.cs → AUSENTES en 2.0
                    (en 1.4: 1557 · 655 · 2532 · 269 líneas)
Dependencia que obliga al Bolt 014 (el proveedor clásico y el tipado del interior):
                    SqlHelper.cs: SqlConnection×123, SqlParameter×135, SqlCommand×102, SqlTransaction×81,
                                  SqlDataReader×36, SqlDataAdapter×9, System.Data.SqlClient, ConfigurationManager
                    SqlHelperParameterCache.cs: SqlParameter×35, SqlConnection×16, System.Data.SqlClient
                    BaseGateway.cs: DbTransaction×26, HttpContext×20, System.Web×5  ·  BaseLoader.cs: HttpContext×19
Precondiciones OK:  contrato del Bolt 011 presente · catálogo publica DefaultTtlSeconds, ParameterMetadataTtlSeconds
                    y MaxEntries · suite de caracterización presente · suite hermética verde antes de empezar
```

### 9.2 Las dos condiciones de parada del SPEC que se activaron

```
§5  — «El proyecto … con la superficie base y sus puntos de caché actuales. Si no existe, parar y reportar:
       este Bolt no crea la capa de datos»                                        → precondición NO cumplida
§15 — «Al aportar aparece una dependencia de secuencia con otro Bolt …: parar y escalar la secuencia … nunca
       aportar ni transformar código de otro Bolt (G11)»                           → condición de parada ESPECÍFICA
§15 — «El cambio que se pide es del aspecto del Bolt 014 (proveedor, casts, tipado del interior, hazard de la
       conexión del llamador) …: parar y reportar»                                 → aportar §B.1 sería eso
```

### 9.3 El árbol quedó intacto y verde

```
git status --porcelain -- 1.4          → (vacío)                (intacto)
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 135, Skipped: 0, Total: 135
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)          ← corrida DESPUÉS del cierre: nada cambió
0 archivos de producto creados/ modificados/ retirados por este V-Bounce
```

### 9.4 Gates (§9) — lo que puede declararse sin la sustitución

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **n/a (razón)** | No hay cambio de código que construir; el árbol (16 proyectos) sigue sin advertencias (§9.3) |
| Unit / component tests | **n/a (razón)** | No hay código nuevo; la suite hermética queda verde 218/218 (§9.3) |
| Consumer-side contract tests (test doubles) | **n/a (razón)** | Dependen de las fases B/C, bloqueadas |
| SAST / Dependency scanning / OWASP / Secret-leak / PII-DLP / IP / Dependency-confusion | **n/a (razón)** | Sin superficie nueva ni dependencias: **0** archivos de producto tocados |
| Hallucination lint | **pass** | Todo lo declarado acá está medido sobre el árbol y el 1.4 (§9.1); **0** archivos o claves inventados |
| Behavioral reproducibility (hermetic suite) | **n/a (razón)** | Sin cambio de comportamiento |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.5) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`) y además bloqueado por B/C |

### 9.5 Manifest

`devflow/metrics/bolts/US-000.BOLT-013-cache-en-data.json`: entrada **`v_bounces[0]`** (`number: 1`,
**`spec_revision: 2`**, `execution_outcome: **blocked**`), validada contra `manifest-v5-bolt.schema.json`.

---

## 10. Manual interventions

**Ninguna intervención humana.** La verificación, el relevamiento y el cierre se hicieron en este V-Bounce **sin
operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""` (el V-Bounce no escribió código).
- **SPEC ejecutado:** [SPEC-261016-0019](../spec/SPEC-261016-0019-cache-en-data.md) — revisión **2**
  (`AITL-SPEC-Approval` del `2026-10-16T03:17:26-03:00`): §4 (fronteras), §5 (precondición), §6/A–E, §14 (frontera
  de aspecto) y **§15** (condiciones de parada).
- **Bolt:** [US-000.BOLT-013](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — filas **T04** y **T05**.
- **Frontera citada:** [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) (proveedor,
  casts, tipado del interior, hazard de la conexión) — **no tocado**.
- **Prerrequisitos consumidos, presentes:** `Core/Configuration/{ICooperatorConfiguration,ConfigurationKeys,CooperatorConfigurationException}.cs` (Bolt 011).
- **Precedente de la misma frontera:** [`MEM-261016-0053`](MEM-261016-0053-configuracion-portable.md) §8.1 y
  [`SPEC-261016-0018`](../spec/SPEC-261016-0018-configuracion-portable.md) §4/§6/C.2 (citados por §17/§19 del SPEC).
- **Manifiesto:** [US-000.BOLT-013-cache-en-data.json](../metrics/bolts/US-000.BOLT-013-cache-en-data.json).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-013) — **`blocked`** |
| SPEC ejecutado | SPEC-261016-0019 revisión **2** |
| Tiempo de generación IA | ~15 min de reloj (03:15 → 03:28) |
| Fases implementadas | **0 de 5** (A–E): todas dependen del aporte de la capa o esperan la decisión de secuencia |
| Archivos de producto creados / modificados / retirados | **0 / 0 / 0** |
| Loops usados | 3 (guía 10; sin iteraciones de convergencia) |
| Tests de la suite | **218** herméticos (135 + 83), verdes **después** del cierre |

---

## 13. Pending items and stubs

1. **La decisión de escalación (dueño del orden):** qué va primero —(a) **partir** el Bolt con una **revisión
   material** de este SPEC que entregue la Phase A ahora (contrato + implementación en memoria + regiones,
   dependency-free) y deje B/C para después de la capa; o (b) **re-encolar** este Bolt **después** del `US-000.BOLT-014`
   (o del Bolt que aporte la superficie base de `Data`), sin cambiar el SPEC. Se recomienda **(b)**: no parte el
   Bolt, no cambia el plan y no obliga a revisar el SPEC, y el valor de la caché sólo se cierra con los puntos de
   caché consumiéndola (§7.2-a). La opción (a) queda disponible y es la de menor latencia si se quiere el contrato
   en el árbol antes.
2. **El inventario de puntos de caché del 1.4 con sus dependencias** queda asentado en este MEM (§9.1) como insumo
   del V-Bounce N°2: es lo que evita que la sustitución de §B.1 se improvise.
3. **El registro de rupturas no cambia:** la declaración de **ADR-004 §3.1** sigue correctamente en `pendiente`
   (la ejecuta este Bolt, y todavía no ocurrió): su evidencia («el `ICacheProvider` de `Data` todavía no está en el
   árbol») es verdadera y se vuelve falsa el día que la Phase A aterrice — en ese momento el mapa se actualiza con
   el mismo mecanismo de contribución distribuida que usaron los Bolts 022 y 028.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 2`,
  `blocked`) + el **diagnóstico del bloqueo** con su medición y las dos opciones de escalación (§13.1). **No hay
  árbol entregado**: el V-Bounce no escribió código.
- **Qué se pide decidir:** (1) la **adjudicación del bloqueo** (la precondición de §5 no se cumple y aportar la
  capa es el aspecto del Bolt 014 — §15) y (2) la **decisión de secuencia** entre las dos opciones de §13.1.
