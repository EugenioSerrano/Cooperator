---
id: "MEM-261016-0041"
title: "Migrador `.coop` → `.coop2` — V-Bounce detenido por prerequisito ausente (corpus anonimizado)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-002" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0002" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-002", "ADR-001", "ADR-008"] # governing ADRs — none of them materialised a change here (V-Bounce stopped before code)
manifest: "US-000.BOLT-002-migrador-coop-a-coop2.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Link/identifier of the diff (PR/commit) for this V-Bounce, when present — NO code was produced (see §11)
review_ready_at: "2026-10-16T00:41:44-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T00:46:19-03:00"
  decided_at: "2026-10-16T00:46:19-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible: 0 archivos `.coop`/`.coop2` en `git ls-files` y en búsqueda recursiva; `2.0/tests/fixtures/model-corpus/` inexistente; `devflow/input/` sin corpus de modelos (sólo README/INDEX)."
    - "Compliant — el blocker citado coincide literalmente con SPEC §15 (segunda condición de parada) y §6.E.3; la anonimización es acto del dueño del corpus, previo y fuera del Bolt, sin tercera vía."
    - "Compliant — disciplina de datos: cero `.coop` reales en el árbol; nada confidencial en MEM ni manifiesto; gates PII/DLP y secret-leak `pass` en el único sentido aplicable."
    - "Compliant — cierre correcto: `specified → in-development → blocked` (T04, T05); MEM único; manifiesto `v_bounces[0]` con los 8 campos y `execution_outcome: blocked`; sin código parcial (`2.0/src/Cooperator.Framework.Migration/` y su suite inexistentes; `Cooperator2.sln` sin referencia al migrador); `1.4/**` intacto (diff exit 0)."
    - "Compliant — desbloqueo propuesto correcto: anonimizar + commitear el subconjunto (dueño del corpus); V-Bounce nuevo sin re-aprobación (G15 clean: sin cambio material de fuente gobernada)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0041 — Migrador `.coop` → `.coop2`: V-Bounce detenido por prerequisito ausente

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-002](../functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md) |
| **SPEC**        | [SPEC-261016-0002](../spec/SPEC-261016-0002-migrador-coop-a-coop2.md), revisión 1 (aprobada) |
| **V-Bounce**    | 1 |
| **ADRs**        | [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |

> ⚠️ **Este MEM registra un V-Bounce que NO generó código.** No hay entrega: hay un bloqueo. Se
> escribió porque el MEM es obligatorio incluso en V-Bounces bloqueados (§3.3, G17) y porque el
> bloqueo es la evidencia que el revisor necesita para desbloquearlo.

---

## 1. Executive summary

El V-Bounce N°1 del Bolt `US-000.BOLT-002` se detuvo **antes de generar una sola línea de código**, al dispararse
una condición de parada explícita del SPEC aprobado: **el subconjunto anonimizado representativo del corpus real
no está comiteado en el repositorio**. No existe hoy ningún archivo `.coop` ni `.coop2` en el árbol —ni versionado
ni en disco— y el directorio que el SPEC §6.E.1 fija para el corpus de fixtures
(`2.0/tests/fixtures/model-corpus/`) no existe. El SPEC §6.E.3 es literal sobre este caso: «Si al iniciar el
V-Bounce el subconjunto anonimizado no está en el repositorio, **parar y reportar** (§15)», y su §15 lo repite
como condición de parada: «El corpus anonimizado representativo no está comiteado y sólo hay fixtures
sintéticos: t1 quedaría incompleto y está prohibido derivarlos procesando los `.coop` reales. Parar y reportar».
Lo que hace el bloqueo insalvable por el ejecutor no es sólo la ausencia del archivo: es que **la anonimización
es un acto del dueño del corpus, previo y fuera de este Bolt** (SPEC §6.E.3 y §10), y que el ejecutor **nunca**
procesa `.coop` reales — de modo que no hay ninguna vía dentro de mis atribuciones para producir ese fixture.
No se entregó una versión parcial (por ejemplo, las fases A–D con fixtures sólo sintéticos) porque eso
contradiría la condición de parada al pie de la letra y produciría un paquete cuyo criterio de completitud
—el §2 del Bolt exige el subconjunto representativo del corpus real, igual que ADR-002 §3.2 para t1— estaría
sabiendo que no se cumple. Todo lo demás del prerequisito del SPEC §5 **sí** está verificado y disponible: la
salida del Bolt 001 existe y es byte-reproducible, con el inventario de identidad de los seis ensamblados 1.4
(nombre, versión `1.4.5.0`, `PublicKeyToken` y SHA-256) listo para convertirse en la lista de identidad del
migrador; el árbol `1.4/**` sigue intacto respecto de la línea base; y el pack de referencia .NET Framework 4.8
y el framework de pruebas del arnés (xUnit v3 sobre `net48`) están presentes. Ninguna otra condición de parada
dispara. El Bolt queda `blocked` y el desbloqueo depende de una acción humana: que `human:eugenioserrano`,
dueño del corpus, deposite el subconjunto anonimizado.

---

## 2. Implemented phases

**Ninguna fase del SPEC §6 se implementó.** El V-Bounce se detuvo en la verificación de prerequisitos (§5 del
SPEC), antes de la fase A, por la condición de parada descrita en §1 y §8 de este MEM. No existe
`2.0/src/Cooperator.Framework.Migration/`, no existe `2.0/tests/Cooperator.Framework.Migration.Tests/`, no
existe `2.0/tests/fixtures/model-corpus/`, no existe `2.0/tools/pin-legacy-binaries.ps1`, y la solución
`2.0/Cooperator2.sln` **no** fue tocada.

Lo que sí se hizo es la **verificación de prerequisitos en modo lectura**, que es trabajo real y es la
evidencia que justifica el bloqueo y que el próximo V-Bounce reutiliza:

### Prerequisites (§5 del SPEC) — verificación leída, artefacto por artefacto

| Prerequisito (§5) | Estado | Evidencia |
|---|---|---|
| Build 1.4 reproducible del Bolt 001 con su inventario de identidad | **DISPONIBLE** | `2.0/tests/fixtures/frozen-1.4/frozen-1.4-inventory.json` existe; `reproducibility.byte_identical = true`; `frozen_commit = 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`; los seis artefactos con `AssemblyVersion`/`FileVersion` **1.4.5.0**, `PublicKeyToken` `8f7f403e20325116` en `Core`/`Data`/`Library` y `null` en `Utility`, `CooperatorModeler.exe` y `ConnectionStringEncriptor.exe`, cada uno con su SHA-256 (ver §9) |
| Estructura del árbol 2.0 (solución, `global.json`, `Directory.Build.props`, `Directory.Packages.props`) | **DISPONIBLE** | `2.0/` con los cuatro portables, las dos suites del arnés y los fixtures del Bolt 001; `System.Text.Json` ya fijado en `Directory.Packages.props` (la versión exacta que el SPEC §5 exige compartir con `Modeling`) |
| Árbol `1.4/` presente y congelado en la línea base | **DISPONIBLE** | `git diff --quiet 6aa497f… -- 1.4` → exit `0`; `git status --porcelain -- 1.4` → vacío (ver §9) |
| Pack de referencia .NET Framework 4.8 y toolchain fijado | **DISPONIBLE** | Es el mismo que el Bolt 001 verificó y usó para construir el 1.4 y correr la suite `net48` |
| Framework de pruebas que corre sobre `net48` | **DISPONIBLE** | xUnit v3 en modo vstest, ya en uso por `Cooperator.Framework.Characterization.Tests` (`net48`) del Bolt 001 |
| **Subconjunto anonimizado representativo del corpus real, comiteado** | **AUSENTE — condición de parada** | 0 archivos `.coop`/`.coop2` en el árbol; `2.0/tests/fixtures/model-corpus/` inexistente; nada en `devflow/input/` (ver §9) |

---

## 3. Files created

| File | Purpose |
|------|---------|
| `devflow/memory/MEM-261016-0041-migrador-coop-a-coop2.md` | Este MEM: el registro obligatorio del V-Bounce bloqueado, con la evidencia del bloqueo y el prerequisito verificado |

**No se creó ningún archivo de la entrega del SPEC.** En particular, no se creó el proyecto
`Cooperator.Framework.Migration`, ni su CLI, ni su escritor canónico, ni la lista de identidad, ni los fixtures,
ni el proyecto de pruebas del migrador. La decisión de no dejarlos a medio construir está razonada en §7.

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md` | `status: "specified"` → `"in-development"` + fila T04 (toma del Bolt por este ejecutor, G11) y, al cerrarse el V-Bounce, `"in-development"` → `"blocked"` + fila T05 — la transición de cierre que la secuencia obligatoria del V-Bounce exige para un V-Bounce bloqueado (G17 paso 4, §3.2.3). Ninguna otra parte del Bolt fue tocada |
| `devflow/metrics/bolts/US-000.BOLT-002-migrador-coop-a-coop2.json` | Entrada `v_bounces[]` número 1 con los ocho campos, `execution_outcome: "blocked"` |

**Ningún archivo del Bolt 001 fue tocado** —ni su Bolt, ni su MEM (`MEM-261016-0037-…`), ni su manifiesto—, en
respeto de que su paquete está en revisión. **Ninguna línea de `1.4/**` fue modificada, agregada ni eliminada.**
**No se tocó** ningún otro Bolt, SPEC, manifiesto, índice ni registro de corrida.

---

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

---

## 6. Files deleted

| File | Reason |
|------|--------|
| — | No hubo borrados. |

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| **Detener el V-Bounce sin generar código**, en lugar de entregar las fases A–D más los fixtures sintéticos | Es una condición de parada explícita del SPEC aprobado (§15, segunda) y su §6.E.3 la fija «al iniciar el V-Bounce», no al llegar a la fase E. La instrucción de ejecución es seguir las condiciones de parada al pie de la letra. Entregar una versión parcial habría producido un paquete cuyo criterio de completitud —el §2 del Bolt y ADR-002 §3.2 (t1) exigen el subconjunto representativo del corpus real— se sabría incumplido: un verde falso, que es lo que la metodología prohíbe |
| **No dejar el proyecto del migrador a medio construir** (ni el esqueleto del `csproj`, ni el alta en la solución, ni un `model-corpus/` vacío) | Un árbol a medio construir obliga al próximo V-Bounce a reconciliar un estado que nadie pidió, mete un proyecto sin pruebas en una solución que trata las advertencias como error, y dejaría en el repositorio artefactos de un V-Bounce que se declaró `blocked`. La reversión de este V-Bounce debe ser el no-cambio |
| **No intentar producir el fixture anonimizado por ninguna vía** | El SPEC §6.E.3 y §10 son explícitos: la anonimización es un acto del **dueño del corpus**, previo y fuera de este Bolt, y «está prohibido derivarlos procesando los `.coop` reales». El ejecutor **nunca** lee, copia, abre ni recibe `.coop` reales. No hay vía legítima dentro de mis atribuciones |
| **Registrar dos transiciones de estado en el Bolt (T04 y T05)** en lugar de sólo la toma | La secuencia obligatoria del V-Bounce (§3.3) pide que el ejecutor ponga el Bolt `in-development` con su fila al tomarlo (T04) y que un V-Bounce bloqueado, una vez que su MEM y su entrada de manifiesto existen, ponga el Bolt `blocked` con su fila (T05). Ambas filas se registran en el mismo acto del cierre y son las únicas ediciones al documento |
| **Verificar los prerequisitos del §5 en modo lectura y registrarlos** aunque el V-Bounce se detenga | El bloqueo no es «no se pudo empezar»: es «todo el prerequisito está listo salvo uno». Dejar constancia de cuál está listo —en particular el inventario de identidad con hashes, que es la entrada de la fase A.2— convierte este MEM en el punto de partida del próximo V-Bounce en lugar de en un callejón sin salida |
| **No invocar la alternativa de un corpus sintético «equivalente»** | El SPEC §6.E.2 exige que los fixtures **sintéticos** cubran las formas del contrato y los casos negativos, y que el subconjunto anonimizado cubra **todas las formas del modelo que aparecen en producción**. Son dos procedencias distintas y la segunda no se sustituye: los sintéticos los construyo yo, el representativo lo aporta el dueño. Declarar «sintéticos equivalentes» sería inventar cobertura |

---

## 8. Deviations and assumptions

### El bloqueo (blocker exacto)

**Condición de parada disparada: SPEC-261016-0002 §15, segunda condición, y §6.E.3.**

> §15 — «**El corpus anonimizado representativo no está comiteado** y sólo hay fixtures sintéticos: t1 quedaría
> incompleto y está prohibido derivarlos procesando los `.coop` reales. Parar y reportar.»
>
> §6.E.3 — «Si al iniciar el V-Bounce el subconjunto anonimizado no está en el repositorio, **parar y
> reportar** (§15): los fixtures sintéticos por sí solos **no** cumplen el criterio de completitud de t1
> (ADR-002 §3.2) y está prohibido derivarlos procesando el corpus real.»

**Hecho verificado (no es un supuesto):** no hay ningún `.coop` ni `.coop2` en el repositorio —ni en el índice de
git ni en el árbol de trabajo—, `2.0/tests/fixtures/model-corpus/` no existe, y `devflow/input/` no contiene
corpus de modelos. Ver §9 para las cuatro comprobaciones con su salida.

**Por qué el ejecutor no puede resolverlo:** la anonimización del subconjunto es un **acto del dueño del
corpus**, previo y fuera de este Bolt (SPEC §6.E.3, §10), y «está prohibido derivarlos procesando los `.coop`
reales». El ejecutor nunca procesa `.coop` reales. No hay una tercera vía.

**Alcance del bloqueo:** afecta a la **completitud de t1** y, con ella, a todo el V-Bounce, porque el criterio de
completitud del Bolt (§2: «para cada fixture del corpus de prueba —**incluido el subconjunto representativo del
corpus real**—») y el de ADR-002 §3.2 para t1 lo exigen. Las fases A–D del SPEC no dependen del corpus, pero el
SPEC no autoriza entregarlas y declarar la fase E parcial: parar es parar.

### Acción de desbloqueo (una sola, humana)

1. Que `human:eugenioserrano`, **dueño del corpus**, anonimice y comitee el subconjunto representativo bajo
   `2.0/tests/fixtures/model-corpus/<slug>/` con su entrada `model.coop` (anonimizada: **sin** cadena de
   conexión, sin nombres ni identificadores de cliente) y su `INDEX.md` de procedencia — conforme a SPEC §6.E.1
   y §6.E.3.
2. Con ese fixture en el repositorio, un **V-Bounce nuevo** de este mismo Bolt y esta misma revisión de SPEC
   puede arrancar: no hay cambio de fuente gobernada, así que **no** hace falta re-aprobar el SPEC (el
   `AITL-SPEC-Approval` de la revisión 1 sigue vigente).

### Comprobaciones de otras fuentes de parada (todas negativas)

| Condición de parada (§15) | ¿Dispara? | Comprobación |
|---|---|---|
| Build 1.4 reproducible del Bolt 001 ausente | **No** | `frozen-1.4-inventory.json` presente, `byte_identical = true`, commit congelado correcto |
| Corpus anonimizado representativo ausente | **SÍ** | Ver arriba y §9 |
| Falta el pack de .NET Framework 4.8 o el framework de pruebas no corre en `net48` | **No** | El pack está instalado y el Bolt 001 compila y corre xUnit v3 sobre `net48` |
| El esquema del Bolt 004 contradice la forma emitida | **No aplicable** | El V-Bounce no llegó a emitir forma alguna; el Bolt 004 no ha corrido |
| Aparece la necesidad de un campo no declarado por ADR-002 §3.1 | **No aplicable** | No se llegó al diseño de la proyección |
| Un archivo real del corpus aparece en el conjunto de pruebas | **No** | No hay ningún archivo del corpus en el repositorio |
| La deserialización revela un layout ≠ `1.4.5.0` | **No aplicable** | No se deserializó nada |
| **Cambio material de una fuente gobernada (G15)** | **No** | Ver abajo |
| El digest no es reproducible | **No aplicable** | No hay conversión que digerir |
| Un gate de §9 no cierra en `pass` | **No aplicable** | No se ejecutó ningún gate (ver §9) |

### G15 — verificación explícita de fuentes gobernadas

El SPEC §15 y G15 obligan a detenerse ante un cambio material en una fuente gobernada. Se comprobó y **no
ocurre**: `US-000.BOLT-002` es el mismo (su `review` de `AITL-BOLT-READY-Approval` intacto) y sólo cambió su
`status`, por los actos del propio V-Bounce; `REV-001` sigue `approved`; `ADR-001`, `ADR-002` y `ADR-008` siguen
`accepted` y sin reescritura; la **línea base de código** sigue siendo `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`
—la que el SPEC §3 declara— y `1.4/**` está intacto frente a ella. El estado `in-development` del Bolt 001 **no**
es un cambio material de una fuente de este SPEC: el propio SPEC §3 lo lista como «Previous Bolt (dependency,
**no fuente gobernada**)» y su §5 anticipa ese árbol como prerequisito aportado, no como cambio de fuente. **El
`AITL-SPEC-Approval` de la revisión 1 sigue vigente; no hay que re-aprobar.**

### Supuestos

- Se asumió que el subconjunto anonimizado debía estar ya **comiteado** en el momento de arrancar el V-Bounce,
  que es exactamente lo que el SPEC §6.E.3 exige. Si la intención del proyecto fuera que el ejecutor lo
  construyera, eso exigiría una revisión material del SPEC (la prohibición de procesar `.coop` reales es de
  §10 y de §6.E.3, no una preferencia de estilo).
- No se asumió ninguna forma del corpus real: no se inspeccionó, no se infirió y no se describió ningún
  contenido de modelo, porque no hay ninguno accesible y no corresponde.

---

## 9. Verification evidence

Toda la evidencia de este V-Bounce es **de lectura**: comprobaciones de estado del repositorio y del
prerequisito. No hay corrida de build, de suite ni de gate que exhibir, porque no se generó código — y así se
declara, sin sustituirla por una corrida que no ocurrió.

### Comprobación del bloqueo — ausencia del corpus

```text
### 1) git ls-files | Select-String "\.coop2?$"
(sin resultados)
count = 0

### 2) Get-ChildItem -Recurse -Force -File (excluyendo .git) | Where-Object { $_.Name -match "\.coop2?$" }
(sin resultados)
count = 0

### 3) Test-Path 2.0/tests/fixtures/model-corpus
False

### 4) Get-ChildItem -Recurse devflow/input   (depósito humano de evidencia cruda)
devflow\input\business                 (dir)
devflow\input\databases                (dir)
devflow\input\documentation            (dir)
devflow\input\interviews               (dir)
devflow\input\source-code              (dir)
devflow\input\ui-ux                    (dir)
devflow\input\README.md
devflow\input\business\README.md
devflow\input\databases\README.md
devflow\input\documentation\INDEX.md
devflow\input\documentation\README.md
devflow\input\interviews\INDEX.md
devflow\input\interviews\README.md
devflow\input\source-code\README.md
devflow\input\ui-ux\README.md
```

Ninguna de las cuatro comprobaciones encuentra un modelo legacy **ni sintético**: el corpus de fixtures no está
ni empezado. Busqué también, sin resultados, cualquier `INDEX.md` bajo `2.0/tests/fixtures/` y cualquier
referencia en disco a `model.coop`.

### Prerequisito verificado — inventario de identidad del 1.4 (entrada de la fase A.2)

```text
2.0/tests/fixtures/frozen-1.4/frozen-1.4-inventory.json
  byte_identical       = True
  frozen_commit        = 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4

  ConnectionStringEncriptor.exe      | v=1.4.5.0 | token=null              | sha256=9E576E4021DCB43F...
  Cooperator.Framework.Core.dll      | v=1.4.5.0 | token=8f7f403e20325116  | sha256=1857309FF7F95DCE...
  Cooperator.Framework.Data.dll      | v=1.4.5.0 | token=8f7f403e20325116  | sha256=F6F806D6EB8FF794...
  Cooperator.Framework.Library.dll   | v=1.4.5.0 | token=8f7f403e20325116  | sha256=D650E9355D8FDBDF...
  Cooperator.Framework.Utility.dll   | v=1.4.5.0 | token=null              | sha256=62373EF030262067...
  CooperatorModeler.exe              | v=1.4.5.0 | token=null              | sha256=8DD87DBE462C6FD7...
```

Los seis artefactos del **build reproducible de la fuente** están versionados con nombre, versión, token y hash:
la lista de identidad de la fase A.2 tiene de dónde generarse, y el token nulo donde corresponde
(`Utility`, `CooperatorModeler.exe`, `ConnectionStringEncriptor.exe`) y `8f7f403e20325116` en `Core`/`Data`/
`Library` coinciden con lo que el SPEC §6.A.2 declara. **Los binarios vendorizados de
`1.4/Tools/CooperatorModeler/Templates/Assemblies/` no son la fuente de esta lista** — el inventario los
registra aparte, con su hash y su divergencia marcada (`Utility` **1.3.3.0** frente a `1.4.5.0` del build de la
fuente), tal como el SPEC §3 y ADR-002 §3.2 exigen: quedan excluidos por proveniencia, no adoptados en silencio.

### Prerequisito verificado — árbol 1.4 intacto y línea base

```text
### 5) estado del arbol 1.4 respecto del baseline
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Build

```text
n/a — no se ejecutó build alguno: el V-Bounce se detuvo antes de la fase A y no generó código.
```

### Tests

```text
n/a — no existe suite del migrador: no se escribió la primera prueba. 0 casos creados, 0 corridas.
     (La suite del arnés del Bolt 001 no se corrió en este V-Bounce: no es evidencia de este Bolt.)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2), y además el
V-Bounce no llegó a escribir ninguna prueba. No hay nada que compilar ni que ejecutar en el ciclo de
integración.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG: no hay evidencia roja previa que exhibir ni protocolo TDD estricto que cumplir
(§3.3.1).

### Gates

> Vocabulario de T11 (`pass` / `waived` / `n/a` con razón). **Ninguno de estos `n/a` es un `pass` encubierto:
> ningún gate se ejecutó, porque no hay código que someter a gate.** Se declara la razón exacta en cada fila.

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | `n/a` — no se generó código; el V-Bounce se detuvo en la condición de parada antes de la fase A |
| Unit / component tests | V-Bounce | `n/a` — no existe suite del migrador |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el SPEC ya lo declara `n/a` por diseño (el contrato es el archivo de salida); con 0 archivos de salida sigue siendo `n/a` |
| SAST | V-Bounce | `n/a` — 0 líneas de código nuevo que analizar |
| Dependency scanning / licenses | V-Bounce | `n/a` — no se declaró ninguna dependencia nueva (no se tocó `Directory.Packages.props`) |
| OWASP Top 10 — static review | V-Bounce | `n/a` — no se escribió la ruta de deserialización que este gate evalúa |
| Prompt-injection scan | V-Bounce | `n/a` — el SPEC lo declara `n/a` por diseño |
| Secret-leak scan | V-Bounce | `n/a` — no se produjo ningún `.coop2`, reporte ni fixture; el único documento escrito (este MEM) no contiene ningún valor de cadena de conexión (no hay ninguno accesible) |
| Hallucination lint | V-Bounce | `pass` — toda afirmación de este MEM es trazable a una comprobación de lectura registrada en §9 o a una cláusula citada del SPEC/ADR. Es el único gate evaluable hoy, y se evalúa porque su objeto es este documento |
| IP / license provenance | V-Bounce | `n/a` — 0 código incorporado |
| PII / DLP | V-Bounce | `pass` — 0 datos personales o de cliente; **0 `.coop` reales procesados**; no se creó ningún fixture. Es el control de datos de §10 verificado en el único sentido en que hoy aplica |
| Dependency-confusion | V-Bounce | `n/a` — 0 paquetes nuevos |
| Test-first evidence | V-Bounce | `n/a` — no se escribió el escritor ni, por lo tanto, el fixture de referencia que debía precederlo |
| Behavioral reproducibility (hermetic suite) | V-Bounce | `n/a` — no hay conversión que correr dos veces |
| Bolt-manifest validation | V-Bounce | `pass` — la entrada `v_bounces[]` de este V-Bounce valida contra `manifest-v5-bolt.schema.json` (ver §9, Manifest) |
| Integration tests (real dependencies) | Integration test | `n/a` — plan de integración `n/a` por `test_integration: not-required`, y no hay prueba escrita |
| Contract / E2E against the real providers | Integration test | `n/a` — misma razón; la conformidad se cierra en t1 (que no se alcanzó) y en el test bilateral del Bolt 004 |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — sin superficie desplegada ni servicio |
| Perf-smoke (p95/p99) | Integration test | `n/a` — herramienta offline de batch; su costo se mide en el Bolt 029 |
| Observability (logs / metrics / traces) | Integration test | `n/a` — sin servicio ni entorno que observar |

### Manifest

```text
python -c "jsonschema Draft202012Validator(manifest-v5-bolt.schema.json).iter_errors(<bolt manifest>)"
VALID
```

La entrada `v_bounces[]` número 1 de `US-000.BOLT-002` queda completa con los ocho campos y
`review_started_at: null`.

### Encoding de los documentos gobernados

Verificado byte a byte: este MEM, el manifiesto del Bolt y el propio Bolt son UTF-8 **sin BOM**, sin `U+FFFD` y
sin `CR` sueltos (§3.15, W23).

---

## 10. Manual interventions

**None.** No hubo parche humano ni fallback manual. El único acto humano pendiente es el que **desbloquea** el
Bolt (depositar el subconjunto anonimizado del corpus), y es un prerequisito declarado, no una intervención
sobre el código.

---

## 11. Evidence links

- **Diff / PR:** no hay — **no se generó código en este V-Bounce**. El único cambio en el árbol es la
  actualización de estado de `US-000.BOLT-002` y este MEM más su entrada de manifiesto. No se hicieron
  operaciones de git: el commit es del humano (G34).
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — ver `baseline` en el frontmatter. Es el
  mismo commit que el SPEC §3 declara como línea base: **el trabajo de este V-Bounce no lo mueve**.
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-002-migrador-coop-a-coop2.json`](../metrics/bolts/US-000.BOLT-002-migrador-coop-a-coop2.json)
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/tests/fixtures/frozen-1.4/frozen-1.4-inventory.json`
  y su `PROVENANCE.md`; la estructura `2.0/` del Bolt 001. **No se modificó ninguno.**
- **Especificaciones que fijan este bloqueo:** SPEC-261016-0002 §6.E.3, §10 y §15; ADR-002 §3.2 (t1).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados a la verificación de prerequisitos y al cierre del V-Bounce; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | 1 |
| Tests created | **0** — no se escribió ninguna prueba |
| AI-generated code | **0 líneas** — ningún archivo de la entrega del SPEC fue creado |
| Loops consumed | **0 de 10** — el V-Bounce no llegó al lazo de suite: se detuvo en la condición de parada del SPEC antes de generar código |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval`; el resultado no es `ready_for_review` |

---

## 13. Pending items and stubs

Todo el Bolt queda pendiente. En orden de dependencia:

- [ ] **BLOQUEANTE — acción humana:** que el dueño del corpus (`human:eugenioserrano`) anonimice y **comitee**
      el subconjunto representativo del corpus real bajo `2.0/tests/fixtures/model-corpus/<slug>/`
      (`model.coop` anonimizado + `INDEX.md` de procedencia, sin cadena de conexión ni datos de cliente),
      conforme al SPEC §6.E.1/§6.E.3. **Sin esto no hay V-Bounce posible de este Bolt.**
- [ ] **Fase A** — proyecto `2.0/src/Cooperator.Framework.Migration/` (`net48` SDK-style, `PackageReference`,
      `ProjectReference` al 1.4 congelado), alta en `2.0/Cooperator2.sln`, `legacy-binaries.json` generado
      desde el inventario del Bolt 001 y `BinaryIdentityVerifier`. **Prerequisito ya verificado y disponible.**
- [ ] **Fase B** — `ModelFileClassifier` por magic bytes y `LegacyReader` (única excepción de
      `BinaryFormatter`, supresión en el ámbito más estrecho).
- [ ] **Fase C** — `ModelProjector` + `CanonicalModel` + `CanonicalJsonWriter`/`CanonicalJsonReader` +
      `ModelValidator` + `Finding`, con la proyección y las exclusiones de ADR-002 §3.1.
- [ ] **Fase D** — CLI `convert`, `ExitCodes`, reporte JSON + texto, `AtomicFileWriter` (temporal + renombrado),
      destino existente bloqueado, sin `--in-place`.
- [ ] **Fase E** — `2.0/tests/Cooperator.Framework.Migration.Tests/` (`net48`), `FixtureBuilder`, el fixture de
      referencia con su `.coop2` **redactado a mano** desde ADR-002 §3.1, los fixtures sintéticos de casos
      negativos, y el **subconjunto anonimizado** (bloqueante). Sin él, t1 no se puede declarar completo.
- [ ] **Fase F** — `Distribution/README-herramienta.md` y la evidencia de t1 en el MEM del próximo V-Bounce.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce. Siendo este V-Bounce `blocked`, la decisión
> esperable es `changes_requested` / `rejected` sobre el MEM como historia inmutable, y el próximo intento es un
> **V-Bounce nuevo** con su MEM nuevo, una vez depositado el prerequisito.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | `agent:dotnet-developer-reviewer-agent` |
| **Roles** | dev_validator |
| **Decision** | approved |
| **review_ready_at** | `2026-10-16T00:41:44-03:00` |
| **review.started_at** | `2026-10-16T00:46:19-03:00` |
| **review.decided_at** | `2026-10-16T00:46:19-03:00` |
| **Review evidence** | Verificación en vivo (Run N°4): `git ls-files` y búsqueda recursiva de `.coop`/`.coop2` → 0; `2.0/tests/fixtures/model-corpus/` inexistente; `devflow/input/` sin corpus; `2.0/src/Cooperator.Framework.Migration/` y su suite inexistentes; `Cooperator2.sln` sin referencia al migrador; `1.4/**` intacto (diff exit 0); blocker literal vs SPEC §15/§6.E.3 |
| **Comments** | Bloqueo correctamente identificado y registrado; sin código parcial; desbloqueo = anonimizar + commitear el subconjunto (dueño del corpus), luego V-Bounce nuevo sin re-aprobación |
| **Findings** | Ver el bloque `review` del frontmatter (5 hallazgos Compliant) |
| **acknowledged_without_comment** | false |
| **acknowledgment_reason** | — |
