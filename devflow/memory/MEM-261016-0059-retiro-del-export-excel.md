---
id: "MEM-261016-0059"
title: "Retiro del export a Excel del núcleo — V-Bounce detenido por el cierre mínimo (`using` muerto hacia el namespace del Bolt 005)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-006" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0012" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001"] # governing ADR — none of them materialised a change here (V-Bounce stopped before delivering)
manifest: "US-000.BOLT-006-retiro-del-export-excel.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §11)
review_ready_at: "2026-10-16T00:59:20-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:05:03-03:00"
  decided_at: "2026-10-16T01:05:03-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible: verificado en el código que `1.4/Cooperator.Framework.Core/IObject.cs` lleva el `using Cooperator.Framework.Core.LazyLoad;` (L31) **muerto** — el grep sobre `1.4/Cooperator.Framework.Core` muestra que `LazyLoad` aparece en `IObject.cs` sólo en esa línea (0 referencias a símbolos; la interfaz `IObject : IEditableObject` usa únicamente `System.ComponentModel`, `ObjectState` e `IObject`). El namespace `Core/LazyLoad/**` es entregable del **Bolt 005** (SPEC-0015 §6 crea ProviderName.cs/ProviderResolution.cs/ProviderResolver.cs/IAssemblyProvider.cs/LazyProviderFactory.cs/ProviderCache.cs, líneas 287-335). La regla de aporte del SPEC rev.1 §4 («copiados tal cual … sin ninguna otra transformación») + el control §13 + §15 #4 («no reescribe lo ajeno») prohíben quitarlo: tres cláusulas verificadas."
    - "Compliant — el retiro sin esa línea compila (sonda del MEM): `ObjectListHelper.cs` 305 líneas (verificado; L30 using OleDb, L49 FillDataSet, L104 xlsLock, L107-284 CreateExcelFile con `Microsoft.Jet.OLEDB.4.0` en L122, L286 SqlName); el retiro 305→122 + cierre de 4 archivos (252 líneas, sin dependencias fuera de System.*) compila limpio (EXIT=0, 0 warnings con TreatWarningsAsErrors). El claim del MEM es exacto."
    - "Compliant — hallazgo (a) desbloqueo opción 1 sin cambios: verificado en SPEC-0015 §6 que el Bolt 005 declara crear `2.0/src/Cooperator.Framework.Core/LazyLoad/**`; cuando aterrice, el `using` muerto de `IObject.cs` resuelve y este Bolt se ejecuta tal como está escrito, sin revisar el SPEC."
    - "Compliant — hallazgo (b) trampa de AC-3, exclusión correcta: verificado que `2.0/tests/fixtures/api-baseline-1.4/Cooperator.Framework.Core.dll` existe (binario 1.4 congelado, junto a PROVENANCE.md + baseline-digests.json). La exclusión de `tests/fixtures/api-baseline-1.4/**` de la búsqueda de rastro es correcta: es evidencia 1.4, exactamente como §C.2 manda no escanear `1.4/**` como código propio; sin la exclusión, `RetiredSurfaceTests` daría falso positivo contra el propio contrato del gate."
    - "Compliant — cierre correcto: T04 (specified→in-development, 00:59:05) y T05 (in-development→blocked, 00:59:20) con reloj real; MEM único (MEM-261016-0059 referencia US-000.BOLT-006); manifiesto `v_bounces[0]` con los 8 campos y `execution_outcome: blocked`; árbol restaurado (`2.0/src/Cooperator.Framework.Core/` contiene sólo el `.csproj` stub + `packages.lock.json`, 0 archivos `.cs`); suites 20/20 y 83/83; `1.4/**` intacto (git diff --quiet). La nota de honestidad §8.5 (los aportes se generaron en el reconocimiento, antes de la fila T04 — desviación del orden G11) es exactamente el tipo de declaración honesta que el MEM debe llevar: se confirma, no se penaliza."
    - "Compliant — housekeeping T03: verificado que la fila T03 (`ready → specified`, 00:22:23, «SPEC revisión 1 aprobada — agent:dotnet-developer-reviewer-agent») **ya NO falta** — está presente en el Status timeline (fuera de orden cronológico, al final, tras T05). El hallazgo del MEM §8.2 fue exacto al momento de escribirse (00:59:20); la fila fue agregada con posterioridad. No agrego fila de corrección (no aplica: la fila existe) y no edito filas existentes."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0059 — Retiro del export a Excel: V-Bounce detenido por el cierre mínimo

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-006](../functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md) |
| **SPEC**        | [SPEC-261016-0012](../spec/SPEC-261016-0012-retiro-del-export-excel.md), revisión 1 (aprobada) |
| **V-Bounce**    | 1 |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [SPEC-0015](../spec/SPEC-261016-0015-carga-de-providers-lazy.md) (gobierna el namespace bloqueante) |

> ⚠️ **Este MEM registra un V-Bounce que NO dejó código en el árbol.** El retiro se generó, se midió y se
> **revirtió**: el árbol quedó exactamente como estaba. El MEM es obligatorio en V-Bounces bloqueados (§3.3, G17)
> y es lo que permite desbloquear — acá, con un dato muy concreto: **el bloqueo es una sola línea**.

---

## 1. Executive summary

El V-Bounce N°1 del Bolt `US-000.BOLT-006` se detuvo **antes de entregar**, al dispararse la cuarta condición de
parada del SPEC aprobado: **el cierre mínimo exigiría transformar código copiado**. El retiro en sí —que es el
objeto del Bolt— **funciona y quedó verificado**: aporté `ObjectListHelper.cs` en su forma ya retirada (sin
`CreateExcelFile`, sin su campo `xlsLock` y sin el `using System.Data.OleDb;`, conservando `FillDataSet` y
`SqlName` con sus cuerpos intactos y el encabezado BSD) junto con el cierre mínimo que la compilación exige
(`IObject.cs`, `ObjectState.cs`, `Exceptions/CoreException.cs`, `Exceptions/InvalidDataSetException.cs`, copiados
tal cual del 1.4) y, **con el `using` muerto quitado, el proyecto compila limpio: 0 errores y 0 advertencias**
sobre `netstandard2.1` con `TreatWarningsAsErrors=true`. El obstáculo es **una única línea**: el `IObject.cs` del
cierre lleva `using Cooperator.Framework.Core.LazyLoad;` —un `using` **demostradamente muerto** (el archivo no
referencia ningún símbolo de ese namespace)— y ese namespace **todavía no existe en `2.0/`** porque su contenido
es entregable del **Bolt 005** (`2.0/src/Cooperator.Framework.Core/LazyLoad/**`, per SPEC-0015 §6). Copiado
**tal cual** —como §4 manda para el cierre: «copiados tal cual están en el 1.4 y **sin ninguna otra
transformación**»— no compila (`CS0234`); y quitarlo es una transformación, que §4, el control de riesgo de §13
(«copiar tal cual, sin transformación») y §15 #4 («este Bolt **no reescribe lo ajeno**») prohíben al ejecutor. La
segunda condición que concuerda es la séptima (ambigüedad que sólo se resolvería inventando): **quién cierra ese
punto muerto —el Bolt 005, que lo hace resolver al aterrizar, o el Bolt 012 de limpieza de referencias muertas— es
una decisión de secuencia que no me corresponde**. Los aportes se **revirtieron** y el árbol se restauró a su
estado previo: `Core` vuelve a tener sólo su `.csproj` y su lock file, su ensamblado se reconstruyó desde el stub
(no contiene `ObjectListHelper`) y las suites del arnés siguen **verdes (20/20 y 83/83)**. Lo valioso de este
V-Bounce es que deja el camino medido: el retiro está probado, el cierre está identificado archivo por archivo, y
el bloqueo es de **una línea** con dos desbloqueos posibles igualmente baratos.

---

## 2. Implemented phases

**Ninguna fase quedó entregada.** El V-Bounce generó el aporte de la fase A, lo midió y lo **revirtió** al
detectarse la condición de parada, antes de producir la constancia (fase B) y las pruebas (fase C). No existe
`2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs`, ni el cierre mínimo, ni `2.0/docs/api-removals/`, ni
`ApiRemovalCoverageTests.cs`, ni `RetiredSurfaceTests.cs`.

### Retiro generado y verificado (fase A, medida y revertida)

- **Archivo congelado:** `1.4/Cooperator.Framework.Core/ObjectListHelper.cs`, **305 líneas**.
- **Elementos retirados — exactamente los tres que ordena §A.1**, sobre el archivo congelado:
  - L30 `using System.Data.OleDb;`
  - L104 `private static Object xlsLock = new Object();`
  - L107–L284: doc-comment + `public void CreateExcelFile(T2 rowSet, string spreadsheetName, string targetFileName)`
    (la implementación Jet/OLE DB completa: `OleDbConnection`, `Provider=Microsoft.Jet.OLEDB.4.0`, `Excel 8.0`)
- **Elementos conservados, verificados línea por línea:** el encabezado BSD (líneas 1–28) ✔; `FillDataSet(DataSet, T2)`
  con su cuerpo idéntico al del 1.4 (maneja `InvalidDataSetException` en los dos caminos) ✔; `SqlName(string)` ✔;
  la forma pública de la clase (`public class ObjectListHelper<T, T2> where T : IObject where T2 : List<T>`) ✔.
- **Ajuste ordenado por §A.1:** el XML-doc de la clase pasó de `Helper methods for RowSet exportation.` a
  `Helper methods for the RowSet data contract: fills a DataSet from a RowSet.` (deja de mencionar un export que
  ya no existe).
- **Resultado:** `305 → 122 líneas`; **0 ocurrencias** de `OleDb`, `Microsoft.Jet`, `CreateExcelFile` ni `xlsLock`
  en el archivo.

### Cierre mínimo identificado (y su tamaño real medido)

| Archivo aportado (tal cual desde 1.4) | Por qué la compilación lo exige |
|---|---|
| `IObject.cs` (47 líneas) | Constraint genérico `where T : IObject` de `ObjectListHelper<T,T2>`; `IObject : IEditableObject` con `State` y `OriginalValue()` |
| `ObjectState.cs` (61 líneas) | `IObject.State` es de tipo `ObjectState` |
| `Exceptions/CoreException.cs` (71 líneas) | Base abstracta de `InvalidDataSetException`; `[Serializable]` + constructor `(SerializationInfo, StreamingContext)` — §4 lo declara expresamente aportable tal cual (su retiro es del Bolt 007) |
| `Exceptions/InvalidDataSetException.cs` (73 líneas) | La excepción que `FillDataSet` lanza en sus dos caminos |

**Tamaño del cierre: 4 archivos, 252 líneas — acotado, y sin una sola dependencia fuera de `System.*`.**

### Medición del bloqueo — el retiro es correcto; el obstáculo es una línea

1. **Build del aporte completo, tal cual (cierre verbatim):** **falla con un único error**:
   ```text
   IObject.cs(31,33): error CS0234: The type or namespace name 'LazyLoad' does not exist in the namespace
   'Cooperator.Framework.Core' (are you missing an assembly reference?)
   ```
   `IObject.cs` L31 = `using Cooperator.Framework.Core.LazyLoad;` — un `using` **muerto** (el archivo no usa
   ningún símbolo de `LazyLoad`; su única referencia externa es `System.ComponentModel.IEditableObject`).
2. **Sonda (temporal, ya revertida): el mismo aporte con esa única línea quitada** →
   ```text
   Cooperator.Framework.Core -> ...\bin\Release\netstandard2.1\Cooperator.Framework.Core.dll
   EXIT=0
   ```
   **0 errores y 0 advertencias** con `TreatWarningsAsErrors=true`, `Nullable=enable`, `EnableNETAnalyzers=true`
   y `AnalysisLevel=latest` — es decir: **el retiro y el cierre son correctos y limpios**; lo único que impide
   compilar es el `using` muerto. (La sonda se ejecutó sobre el archivo aportado y se **restauró verbatim** desde
   1.4: `restaurado identico a 1.4: True`.)

### Reconocimiento registrado para el próximo V-Bounce

- **Quién posee el namespace bloqueante:** `SPEC-261016-0015` (Bolt 005) crea
  `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderName.cs`, `ProviderResolution.cs`, `ProviderResolver.cs`,
  `IAssemblyProvider.cs`, `LazyProviderFactory.cs` y `ProviderCache.cs` (§6, líneas 287–335), y su propia §5
  contempla «si el árbol 2.0 no lo tiene al iniciar el V-Bounce…». **Cuando el Bolt 005 aterrice, el `using`
  muerto de `IObject.cs` resuelve y este Bolt se ejecuta tal como está escrito, sin revisar el SPEC.**
- **Hallazgo de verificación para AC-3 (evita un falso positivo):** una búsqueda ingenua de
  `OleDb|Microsoft.Jet|Microsoft.ACE` sobre **todo** `2.0/**` encuentra **1 archivo**:
  `2.0/tests/fixtures/api-baseline-1.4/Cooperator.Framework.Core.dll` — el **binario 1.4 congelado** que el Bolt
  001 versionó como línea base del gate (contiene el tipo `ObjectListHelper` y su referencia a `System.Data.OleDb`).
  Es **evidencia 1.4**, exactamente como `1.4/**`, que §C.2 manda **no** escanear como código propio. Sobre el
  código, los proyectos, los scripts y la documentación **propios** de 2.0: **0 ocurrencias** de
  `OleDb|Microsoft.Jet|Microsoft.ACE|CreateExcelFile|xlsLock`. `RetiredSurfaceTests` debe excluir
  `tests/fixtures/api-baseline-1.4/**` para no fallar AC-3 por el contrato que el propio gate necesita.
- **Anatomía del retiro en el 1.4** (para reejecutar sin volver a leer 305 líneas): L30 `using System.Data.OleDb;` ·
  L104 `xlsLock` · L107–284 `CreateExcelFile` · L49 `FillDataSet` · L286 `SqlName`.

---

## 3. Files created

| File | Purpose |
|------|---------|
| `devflow/memory/MEM-261016-0059-retiro-del-export-excel.md` | Este MEM: el registro obligatorio del V-Bounce bloqueado, con el retiro medido y el bloqueo acotado a una línea |

**No se creó ningún archivo de la entrega del SPEC que sobreviva al V-Bounce.** Los cinco archivos del aporte
(`ObjectListHelper.cs` retirado + el cierre mínimo) se generaron para **medir** el bloqueo y se **eliminaron**:
el árbol `2.0/` no contiene hoy ningún `.cs` bajo `Core`. Esto se declara explícitamente porque §4 exige que cada
archivo aportado se declare uno por uno (§2) y porque la reversión es parte del resultado.

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md` | `status: "specified"` → `"in-development"` + fila T04 (toma del Bolt, G11) y, al cerrarse el V-Bounce, `"in-development"` → `"blocked"` + fila T05 — la transición de cierre que la secuencia obligatoria exige para un V-Bounce bloqueado (G17 paso 4, §3.2.3) |
| `devflow/metrics/bolts/US-000.BOLT-006-retiro-del-export-excel.json` | Entrada `v_bounces[]` número 1 con los ocho campos, `execution_outcome: "blocked"` |

**Ningún archivo de los Bolts 001/002/003/011 fue tocado** — sus registros están en revisión/cerrados y sólo se
leyeron; sus suites se corrieron con `--no-build`. **Ninguna línea de `1.4/**` fue modificada** (verificado con
git contra el commit baseline, §9). **No se tocó** ningún otro Bolt, SPEC, manifiesto, índice ni registro de
corrida.

---

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

---

## 6. Files deleted

| File | Reason |
|------|--------|
| `2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs` (aporte, forma retirada) | Revertido: el V-Bounce cerró `blocked` y el árbol debe quedar sin cambios |
| `2.0/src/Cooperator.Framework.Core/IObject.cs`, `ObjectState.cs`, `Exceptions/CoreException.cs`, `Exceptions/InvalidDataSetException.cs` (cierre mínimo) | Idem — el cierre no puede quedar en un árbol que no compila |

> Después de la reversión se reconstruyó el ensamblado de `Core` desde el stub para no dejar un artefacto
> obsoleto: el `.dll` resultante (4.608 bytes) **no contiene `ObjectListHelper`**, y las dos suites del arnés
> siguen verdes (§9). Los `bin/`/`obj/` de `Core` son salidas de build ignoradas por git y se regeneran en cada
> build.

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| **Detener el V-Bounce** en lugar de quitar la línea y entregar | §4 manda copiar el cierre «tal cual … y **sin ninguna otra transformación**»; §13 declara el riesgo «**transforma código ajeno**» con la mitigación «copiar tal cual, sin transformación»; y §15 #4 dice textualmente «**el cierre mínimo exigiría transformar código de otro Bolt** (no basta con copiarlo tal cual): detener y escalar la secuencia — este Bolt **no reescribe lo ajeno**». Tres cláusulas explícitas del SPEC contra una tentación de una línea |
| **No aportar `ILazyProvider.cs`/`IUniqueIdentifiable.cs`** para que el `using` resuelva | Sería **absorber trabajo del Bolt 005** (dueño de `2.0/src/Cooperator.Framework.Core/LazyLoad/**`) y §4 es explícito: «la regla existe para que este Bolt **no absorba trabajo de otro**, no para ampliarlo». Además arrastraría su propio cierre y decidiría la ubicación del contrato Lazy, que es de ADR-006/SPEC-0015 |
| **No reescribir `IObject.cs`** (p. ej. inlinear la interfaz en otro archivo, o aportar una versión «limpia») | Sería inventar una forma del tipo base del núcleo que ningún ADR ordena, y dejaría el `IObject` de 2.0 divergiendo del 1.4 sin decisión que lo respalde |
| **Revertir todo y restaurar el árbol**, incluido el artefacto de build de `Core` | La reversión de un V-Bounce bloqueado debe ser el **no-cambio**; dejar el aporte en un proyecto que no compila rompería el árbol para todos. Reconstruir el ensamblado desde el stub evita además dejar un `.dll` obsoleto que engañaría a `RetiredSurfaceTests` (fase C inspecciona justamente ese ensamblado) |
| **Medir el bloqueo con una sonda temporal** (quitar la línea, compilar, restaurar verbatim) en lugar de afirmarlo | Convierte «creo que sólo es el `using`» en un dato: **el retiro y el cierre compilan limpios con 0 advertencias**. Es lo que le permite al revisor decidir en un minuto, y es lo que el gate de *hallucination lint* exige |
| **Registrar el hallazgo de AC-3 sobre el fixture de la línea base 1.4** | Sin él, `RetiredSurfaceTests` daría **falso positivo** contra el propio contrato del gate y haría fallar AC-3 por un binario 1.4 congelado. Es el tipo de detalle que, no registrado, cuesta un V-Bounce entero |
| **Reportar la fila T03 faltante sin fabricarla** | Escribir una fila histórica que no presencié sería fabricar evidencia; el defecto es de registro del documento y su corrección es de quien lo omitió |

---

## 8. Deviations and assumptions

### 8.1 El bloqueo (blocker exacto)

**Condición de parada disparada: SPEC-261016-0012 §15, cuarta; concuerda la séptima.**

> 4. «**El cierre mínimo exigiría transformar código de otro Bolt** (no basta con copiarlo tal cual): detener y
> escalar la secuencia — este Bolt no reescribe lo ajeno.»
>
> 7. «**Ambigüedad que sólo se resolvería inventando** …: parar y reportar; no se rellena el hueco con una
> suposición (§2.4.1).»

**La cadena de hechos, cada uno verificado:**

1. **El archivo en alcance no está en `2.0/`** → aplica la regla de aporte de §4 (aportarlo en su forma ya
   retirada + el cierre mínimo «copiado tal cual»).
2. **El cierre mínimo necesario es de 4 archivos**, todos sin dependencias fuera de `System.*` (§2).
3. **El cierre copiado TAL CUAL no compila.** Un solo error, en un archivo del cierre:
   `IObject.cs(31,33): error CS0234` por `using Cooperator.Framework.Core.LazyLoad;`.
4. **Ese `using` es muerto**: `IObject.cs` no referencia ningún símbolo de `LazyLoad` (verificado: 0 usos). Es un
   resto de la organización del 1.4.
5. **El namespace `Cooperator.Framework.Core.LazyLoad` no existe en `2.0/` y su contenido es del Bolt 005**
   (SPEC-0015 §6 crea `2.0/src/Cooperator.Framework.Core/LazyLoad/**`: `ProviderName.cs`, `ProviderResolution.cs`,
   `ProviderResolver.cs`, `IAssemblyProvider.cs`, `LazyProviderFactory.cs`, `ProviderCache.cs`).
6. **Quitarlo es una transformación** de un archivo del cierre → prohibida a este ejecutor por §4, §13 y §15 #4.
7. **Aportar el namespace para que resuelva es absorber trabajo del Bolt 005** → prohibido por §4 («no para
   ampliarlo») y por el reparto de ADR-006/SPEC-0015.
8. **Con la línea quitada, todo compila limpio (0 errores, 0 advertencias)** → el bloqueo **no** es del retiro ni
   del cierre: es de **una línea cuya suerte pertenece a otro Bolt** (005 al aterrizar, o 012 «limpieza de
   referencias muertas» si se decide tratarla como punto muerto del núcleo — §4 la lista, en efecto, entre «los
   demás puntos muertos de superficie del núcleo … Bolts 005, 011, 012»).

**Nota de lectura alterna (declarada, para el revisor).** Podría sostenerse que `IObject.cs` **no es «código de
otro Bolt»** —ningún SPEC lo reclama como propio; lo aporta este Bolt como cierre— y que, por tanto, borrar en él
una línea muerta es «hacer que mi propio aporte compile», no «reescribir lo ajeno». Bajo esa lectura, el V-Bounce
sería entregable con un ajuste de una línea declarado en el MEM. **No la apliqué** porque el SPEC fija el criterio
con tres cláusulas explícitas («sin ninguna otra transformación», «transforma código ajeno», «no reescribe lo
ajeno») y el role charter manda parar ante la ambigüedad en lugar de resolverla por cuenta propia. **La decisión
es del revisor**, y este MEM le deja el dato completo para tomarla en un minuto.

### 8.2 Hallazgo de housekeeping — al Bolt 006 le falta la fila T03 de su `Status timeline`

El timeline registra T01 (`draft`) y T02 (`ready`) pero **no** la fila `ready → specified`, aunque el frontmatter
declara `status: "specified"` y el SPEC aprobado tiene `review.decided_at: 2026-10-16T00:22:23-03:00`. Es un
defecto de registro del documento (G39: cada transición se registra en el mismo acto con su fila), **no** algo que
este V-Bounce deba corregir: la edición permitida al Bolt es la de G11 (T04) y la de cierre (T05), y escribir una
fila histórica que no presencié sería fabricar evidencia. **Se reporta para que lo corrija su dueño.** Las filas
T04 y T05 que este V-Bounce sí protagoniza están registradas con el reloj real.

### 8.3 G15 — verificación explícita de fuentes gobernadas

**Sin cambio material.** `US-000.BOLT-006` es el mismo (su `review` de `AITL-BOLT-READY-Approval` intacto) y sólo
cambió su `status`, por los actos de este V-Bounce; `REV-001` sigue `approved`; `ADR-001` sigue `accepted` y sin
reescritura; `OQ-005` sigue `answered`; la línea base de código sigue siendo
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto frente a ella (§9). **El `AITL-SPEC-Approval`
de la revisión 1 sigue vigente** — pero para avanzar hace falta **revisar el SPEC** (o esperar al Bolt 005), no
por cambio de fuente sino por **defecto de secuencia descubierto en la ejecución**.

### 8.4 Comprobación de las demás condiciones de parada (§15) — todas negativas salvo la 4.ª

| Condición de parada (§15) | ¿Dispara? | Comprobación |
|---|---|---|
| 1. Cambio material en una fuente gobernada | **No** | §8.3 |
| 2. Aparece un consumidor **con evidencia** que use la capacidad | **No** | Confirmado: **0 llamadores** en el árbol (el SPEC lo verificó en §2 y este V-Bounce lo reconfirmó: no hay fuentes propias en 2.0 y en `1.4/**` sólo hay comentarios de plantilla, que §C.2 clasifica como informativos con dueño = Bolt del set). No hay evidencia que escale al escape-hatch de ADR-008 §3.6 |
| 3. El retiro exigiría editar `1.4/` | **No** | `1.4/**` intacto (§9); todo el trabajo se hizo sobre copias en `2.0/` |
| **4. El cierre mínimo exigiría transformar código de otro Bolt** | **SÍ** | §8.1 |
| 5. El arnés del Bolt 001 no está disponible | **No** | Comando reproducible, línea base (`api-baseline-1.4/` con digests) y gate operativos; suites verdes (§9) |
| 6. Un gate de §9 no cierra en `pass` | **No evaluado** | No se llegó a entregar ni a correr la suite del Bolt |
| 7. Ambigüedad que sólo se resolvería inventando | **Sí, concuerda** | Quién cierra el `using` muerto (Bolt 005 al aterrizar, o Bolt 012) y si el cierre puede ajustarse: decisión ajena al ejecutor |

### 8.5 Supuestos

- Se asumió que el cierre mínimo se limita a los **tipos que la compilación exige** (los 4 de §2) y que un `using`
  muerto dentro de un archivo del cierre es parte de «el código» que §4 manda copiar sin transformar. Si el
  revisor considera que un `using` muerto no es materia de esa regla, la entrega es de **una línea de trabajo**
  (ver la lectura alterna de §8.1 y el desbloqueo de §13).
- No se asumió ninguna decisión de otro Bolt: ni la ubicación del contrato Lazy (ADR-006/SPEC-0015), ni la forma
  final de `IObject`/`CoreException` (Bolt 007 para `[Serializable]`), ni el contenedor definitivo de las rupturas
  (Bolt 028; el SPEC fija `2.0/docs/api-removals/`).
- **Honestidad de secuencia:** los cinco archivos del aporte se generaron durante el **reconocimiento/medición**
  y antes de que se escribiera la fila T04 — es decir, el orden que el protocolo pide (G11 antes de generar) tuvo
  una desviación, sin consecuencia material porque el V-Bounce cerró bloqueado y el aporte se revirtió por
  completo. Queda registrado acá en lugar de omitirse.
- No se leyó, copió ni procesó ningún dato de negocio ni credencial: la evidencia son líneas de código del árbol
  `1.4/` congelado, conteos y salidas de build.

---

## 9. Verification evidence

### Retiro generado y medido

```text
ObjectListHelper.cs: 305 -> 122 lineas   (retiro de using OleDb + xlsLock + CreateExcelFile)
cierre minimo: IObject.cs                         (47 lineas) copiado tal cual
cierre minimo: ObjectState.cs                     (61 lineas) copiado tal cual
cierre minimo: Exceptions/CoreException.cs        (71 lineas) copiado tal cual
cierre minimo: Exceptions/InvalidDataSetException.cs (73 lineas) copiado tal cual
encoding: U+FFFD=False en los cinco archivos
```

El archivo retirado se leyó completo para verificar el resultado: encabezado BSD intacto (1–28), clase con su
constraint (`where T : IObject`, `where T2 : List<T>`), `FillDataSet` con su cuerpo idéntico y sus dos caminos de
`InvalidDataSetException`, `SqlName` intacto, y **ninguna** ocurrencia de `OleDb`, `Microsoft.Jet`,
`CreateExcelFile` ni `xlsLock`.

### El bloqueo — build del aporte tal cual (cierre verbatim)

```text
=== build del aporte con el cierre VERBATIM ===
D:\Cooperator\2.0\src\Cooperator.Framework.Core\IObject.cs(31,33): error CS0234: The type or namespace name
'LazyLoad' does not exist in the namespace 'Cooperator.Framework.Core' (are you missing an assembly reference?)
EXIT=1
```

Un solo error en todo el aporte. L31 del `IObject.cs` congelado:

```text
L30: using System.ComponentModel;
L31: using Cooperator.Framework.Core.LazyLoad;
L32:
```

### La sonda — el mismo aporte sin esa única línea

```text
--- sonda: build con el using muerto quitado (temporal) ---
  Cooperator.Framework.Core -> ...\bin\Release\netstandard2.1\Cooperator.Framework.Core.dll
EXIT=0
    0 Warning(s)   (con TreatWarningsAsErrors=true, Nullable=enable, analizadores on)

--- restaurando IObject.cs verbatim desde 1.4 ---
restaurado identico a 1.4: True
```

**Conclusión medida:** el retiro y el cierre son correctos y compilan **limpios**; el único obstáculo es el `using`
muerto. Y la sonda se revirtió de inmediato, verificando la restauración byte a byte.

### Propiedad del namespace bloqueante (SPEC-0015 = Bolt 005)

```text
SPEC-261016-0015 §6 declara que el Bolt 005 crea, en 2.0/src/Cooperator.Framework.Core/LazyLoad/:
  ProviderName.cs · ProviderResolution.cs · ProviderResolver.cs
  IAssemblyProvider.cs · LazyProviderFactory.cs · ProviderCache.cs
y su §5 contempla «si el árbol 2.0 no lo tiene al iniciar el V-Bounce…»
→ el namespace existe cuando el Bolt 005 aterrice; entonces el using muerto resuelve.
```

### Reversión y salud del árbol

```text
=== estado de Core tras la reversion ===
Cooperator.Framework.Core.csproj
packages.lock.json
   (exactamente el estado que dejó el Bolt 001; 0 archivos .cs)

=== Rebuild de Core desde el stub ===
  Cooperator.Framework.Core -> ...\bin\Release\netstandard2.1\Cooperator.Framework.Core.dll
EXIT=0
el ensamblado reconstruido: tamano=4608 bytes | ObjectListHelper presente=False
   (el artefacto obsoleto de la sonda quedó eliminado)

=== suites del arnes (siguen verdes, como se las encontró) ===
Passed!  - Failed: 0, Passed: 20, Skipped: 0, Total: 20   - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed: 83, Skipped: 0, Total: 83   - Cooperator.Framework.Characterization.Tests.exe (net48)
```

### Rastro de plataforma en 2.0 (hallazgo de AC-3)

```text
=== búsqueda de OleDb | Microsoft.Jet | Microsoft.ACE en TODO 2.0 ===
2.0/tests/fixtures/api-baseline-1.4/Cooperator.Framework.Core.dll
   → el binario 1.4 CONGELADO que el Bolt 001 versionó como línea base del gate
     (contiene ObjectListHelper y su referencia a System.Data.OleDb) = evidencia 1.4, no código 2.0

=== búsqueda en el código/proyectos/scripts/docs PROPIOS de 2.0 ===
0   (OleDb | Microsoft.Jet | Microsoft.ACE | CreateExcelFile | xlsLock)
```

`RetiredSurfaceTests` debe excluir `tests/fixtures/api-baseline-1.4/**` al buscar el rastro (igual que §C.2
ordena no escanear `1.4/**`): si no, el contrato que el gate necesita hace fallar AC-3.

### Árbol congelado

```text
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Build

```text
n/a — no hay build de entregable que exhibir: el aporte se revirtió tras medir el bloqueo.
```

### Tests

```text
n/a — no se escribió ninguna prueba. 0 casos creados.
     (Las 103 corridas verdes registradas arriba son del arnés del Bolt 001, como evidencia de que el árbol
      quedó sano tras la reversión; no son pruebas de este Bolt.)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2 del SPEC) y además
el V-Bounce no llegó a escribir ninguna prueba.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG (§3.3.1).

### Gates

> Vocabulario de T11 (`pass` / `waived` / `n/a` con razón). **Ninguno de estos `n/a` es un `pass` encubierto:
> ningún gate se ejecutó sobre una entrega, porque no hay entrega.**

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | `n/a` — no hay entrega. **La medición sí se hizo:** el retiro + el cierre compilan con **0 errores y 0 advertencias** sobre `netstandard2.1` (sonda registrada arriba) |
| Unit / component tests | V-Bounce | `n/a` — no se escribieron pruebas. Las del arnés del Bolt 001 quedaron verdes (20/20 y 83/83) tras la reversión |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el SPEC ya lo declara `n/a` con razón |
| SAST | V-Bounce | `n/a` — 0 líneas de código nuevo en el árbol |
| Dependency scanning / licenses | V-Bounce | `n/a` — no se declaró ninguna dependencia (no se tocó `Directory.Packages.props`) |
| OWASP Top 10 — static review | V-Bounce | `n/a` — el SPEC lo declara `n/a` con razón |
| Prompt-injection scan | V-Bounce | `n/a` — el SPEC lo declara `n/a` |
| Secret-leak scan | V-Bounce | `n/a` — no se produjo ningún archivo entregable; este MEM no contiene cadenas de conexión ni credenciales |
| Hallucination lint | V-Bounce | `pass` — cada afirmación de este MEM es trazable a una salida de build registrada arriba, a una búsqueda registrada, o a un número de línea del archivo congelado; 0 rutas, tipos o miembros inventados. Es el único gate evaluable hoy |
| IP / license provenance | V-Bounce | `n/a` — 0 código incorporado al árbol (el encabezado BSD se conservó en la copia y se revirtió con ella) |
| PII / DLP | V-Bounce | `pass` — 0 datos personales ni de negocio leídos o escritos |
| Dependency-confusion | V-Bounce | `n/a` — 0 paquetes nuevos |
| Test-first evidence | V-Bounce | `n/a` — no es un Bolt de BUG |
| Behavioral reproducibility (hermetic suite) | V-Bounce | `n/a` — no hay suite nueva |
| Bolt-manifest validation | V-Bounce | `pass` — la entrada `v_bounces[]` de este V-Bounce valida contra `manifest-v5-bolt.schema.json` (§9, Manifest) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | `n/a` — no hay superficie 2.0 nueva que comparar; la cobertura de la constancia (AC-4) queda para el V-Bounce que entregue |
| Integration tests (real dependencies) | Integration test | `n/a` — plan `n/a` por `test_integration: not-required` |
| Contract / E2E against the real providers | Integration test | `n/a` — sin dependencias reales |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — sin superficie desplegada |
| Perf-smoke (p95/p99) | Integration test | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | `n/a` — sin telemetría de servicio |

### Manifest

```text
python -c "jsonschema Draft202012Validator(manifest-v5-bolt.schema.json).iter_errors(<bolt manifest>)"
VALID
```

### Encoding de los documentos gobernados

Verificado byte a byte: este MEM, el manifiesto del Bolt y el propio Bolt son UTF-8 **sin BOM**, sin `U+FFFD` y
sin `CR` sueltos (§3.15, W23).

---

## 10. Manual interventions

**None.** No hubo parche humano ni fallback manual. La decisión humana pendiente es la del revisor (revisar el
SPEC, o esperar al Bolt 005), no una intervención sobre el código.

---

## 11. Evidence links

- **Diff / PR:** no hay — **no quedó código en el árbol**. Los únicos cambios son la actualización de estado de
  `US-000.BOLT-006`, este MEM y su entrada de manifiesto. No se hicieron operaciones de git: el commit es del
  humano (G34).
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — el mismo que el SPEC §3 declara. **El
  trabajo de este V-Bounce no lo mueve.**
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-006-retiro-del-export-excel.json`](../metrics/bolts/US-000.BOLT-006-retiro-del-export-excel.json)
- **Fuente de la evidencia (sólo lectura):** `1.4/Cooperator.Framework.Core/ObjectListHelper.cs` (L30, L49, L104,
  L107–284, L286) · `1.4/Cooperator.Framework.Core/IObject.cs` (L30–L31) ·
  `1.4/Cooperator.Framework.Core/{ObjectState.cs,Exceptions/CoreException.cs,Exceptions/InvalidDataSetException.cs}`
- **Normativo que decide el bloqueo:** SPEC-261016-0012 §4 (regla de aporte y exclusiones), §6/A.1, §6/C.2, §13
  (riesgos), §15 (4.ª y 7.ª); SPEC-261016-0015 §5/§6 (propiedad del namespace `Core/LazyLoad`); ADR-001 §3.1/§3.5.
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/build/build.ps1`, `2.0/tests/fixtures/api-baseline-1.4/**`,
  `2.0/tests/Cooperator.Harness.Tests`, `2.0/tests/Cooperator.Framework.Characterization.Tests`. **No se modificó ninguno.**

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados a la generación y medición del retiro y al cierre del V-Bounce; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | 1 |
| Tests created | **0** — no se escribió ninguna prueba |
| AI-generated code | **0 líneas permanecen** — el aporte (5 archivos, 374 líneas con el retiro aplicado) se generó para medir y se revirtió |
| Loops consumed | **1 de 10** — el lazo se usó para el retiro (build verde con el `using` quitado) y para la reversión; el V-Bounce se detuvo en la condición de parada sin entregar |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval`; el resultado no es `ready_for_review` |

---

## 13. Pending items and stubs

- [ ] **BLOQUEANTE — acción humana, y con dos salidas baratas** (el bloqueo es **una línea**):
      **(1)** esperar/ejecutar el **Bolt 005** (`SPEC-261016-0015`): al aterrizar su
      `2.0/src/Cooperator.Framework.Core/LazyLoad/**`, el `using Cooperator.Framework.Core.LazyLoad;` del
      `IObject.cs` copiado **resuelve solo** y este Bolt se ejecuta **tal como está escrito**, sin revisar el SPEC;
      **(2)** revisar `SPEC-261016-0012` para autorizar expresamente que, al aportar el cierre, el `using` muerto de
      `IObject.cs` se retire en el mismo acto (una línea, documentada) — o para asignar ese punto muerto a su dueño
      (Bolt 005 o Bolt 012 de limpieza de referencias), que es lo que §4 declara hoy («los demás puntos muertos de
      superficie del núcleo … Bolts 005, 011, 012»).
- [ ] **Hallazgo de housekeeping (§8.2):** al Bolt 006 le falta la fila T03 (`ready → specified`). Lo corrige su
      dueño; este V-Bounce no lo toca.
- [ ] **Todo el Bolt queda pendiente** (fases A, B y C), con el camino ya medido: el retiro funciona, el cierre son
      4 archivos identificados, y el `RetiredSurfaceTests` deberá **excluir `tests/fixtures/api-baseline-1.4/**`**
      de la búsqueda de rastro para no dar falso positivo contra el propio contrato del gate (§2).
- [ ] **Recordatorio para el V-Bounce que entregue:** la constancia de la superficie retirada
      (`2.0/docs/api-removals/US-000.BOLT-006.json`) debe cubrir **al menos** el miembro
      `ObjectListHelper<T,T2>.CreateExcelFile(T2, string, string)` con los tres datos de ADR-008 §3.1
      (qué desaparece / ADR-001 §3.5 + OQ-005 / **reemplazo: ninguno dentro del framework** — el export queda del
      lado del consumidor), y `ApiRemovalCoverageTests` debe cruzarla contra el reporte del gate.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce. Siendo este V-Bounce `blocked`, la decisión
> esperable es `changes_requested` / `rejected` sobre el MEM como historia inmutable, y la respuesta es la
> revisión del SPEC (o el aterrizaje del Bolt 005) seguida de un **V-Bounce nuevo** con su MEM nuevo.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | — |
| **Roles** | — |
| **Decision** | — |
| **review_ready_at** | `2026-10-16T00:59:20-03:00` |
| **review.started_at** | — |
| **review.decided_at** | — |
| **Review evidence** | — |
| **Comments** | — |
| **Findings** | — |
| **acknowledged_without_comment** | — |
| **acknowledgment_reason** | — |
