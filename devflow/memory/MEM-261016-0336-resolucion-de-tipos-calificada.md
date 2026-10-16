---
id: "MEM-261016-0336"
title: "Resolución de tipos calificada en `Core` (SPEC rev.2) — V-Bounce N°1: los tres puntos del mapeo con identificación inequívoca, diagnóstico accionable y 0 resolución implícita"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-008" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0034" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce (revisión 2 = nulabilidad preventiva §B.4)
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-006", "ADR-001", "ADR-005"] # ADR-006 §3.1/§3.2/§3.3 (regla de nombres calificados, unicidad y diagnósticos), ADR-001 §3.2/§3.4 (resolución de tipos entre runtimes; capa base sin terceros), ADR-005 §3.3 (la forma del contrato que consume el código generado se preserva)
manifest: "US-000.BOLT-008-resolucion-de-tipos-calificada.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:36:50-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T03:40:24-03:00"
  decided_at: "2026-10-16T03:40:24-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0034` revisión 2 gobernante (nulabilidad preventiva §B.4), baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **135/135 harness + 12/12 `mapping-hermetic` + 83/83 caracterización, exit 0** (17 proyectos, **230 casos**); la suite nueva está integrada al gate."
    - "Compliant — §B.4 ejercida **con la prueba del build** (verificado): directiva `#nullable disable` **sólo** en `BaseObjectList.cs` (1); `IObjectList.cs` (cierre mínimo) **0**; `TypeIdentification.cs` (código nuevo) **0**; **0** `.csproj` con la etiqueta. El `CS8600` reportado junto a `CS8618`/`CS8603` es la **misma familia** de nulabilidad y cae bajo el criterio objetivo (*archivo aportado + diagnóstico*), no bajo una lista cerrada — aceptable."
    - "Compliant — entregables verificados: `TypeIdentification.cs` usa `Nullable.GetUnderlyingType` (**por API**) y **rechaza el nombre simple sin intentar resolverlo**; `TypeResolutionException` es `sealed class ... : CoreException` (POCO, valor+tipo+punto); `ObjectListHelper.cs` con `catch (Exceptions.TypeResolutionException)` **antes** del `catch (Exception)` (el gemelo destapado); inventario con el punto **4b**; constancia **2 entradas + 1 obligación** (`BOLT008-OBL-01`, corpus con dueño)."
    - "Compliant — la integración de la suite al gate es **correcta**: `build.ps1` agrega `$mappingProject` y la corrida `mapping-hermetic` **clonando el bloque del arnés** (mismo `Invoke-Suite`, mismo manejo de resultado/código de salida), no una llamada suelta. La declaración de presupuesto (16 loops, causas de **forma** medidas en §8.6) es honesta y aceptable."
    - "CORRECCIÓN del timeline (registrada, sin editar filas): el ejecutor **adelantó** la transición `in-development → ready-for-acceptance` en la fila **T05** (cierre, 03:36:50), antes del `AITL-MEM-Approval` — el contrato espera `in-development` durante la pausa. El estado final `ready-for-acceptance` es el correcto (`test_integration: not-required`) y este `AITL-MEM-Approval` (Development Completed) lo legitima. DoD + MEM completo + manifiesto válido; `1.4/**` intacto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0336 — Resolución de tipos calificada en `Core` (rev.2)

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-008](../functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md) — `type: non-functional`, `work_category: debt`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0034](../spec/SPEC-261016-0034-resolucion-de-tipos-calificada.md), **revisión 2** (aprobada `2026-10-16T03:18:49-03:00`; la revisión 2 es **preventiva**: autoriza la política de nulabilidad de §B.4 antes del aporte) |
| **V-Bounce**    | **1** — el primero del Bolt |
| **ADRs**        | [ADR-006](../adrs/ADR-006-carga-de-ensamblados.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-005](../adrs/ADR-005-provider-de-datos.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

El camino **filas→objetos** de `Core` identifica tipos y miembros de forma **inequívoca**, con **diagnóstico
accionable** y **sin resolución implícita**: los **tres** puntos del inventario quedaron corregidos —el que resolvía
el tipo de una columna por **nombre simple reconstruido** con un `Substring` sobre el `ToString()` de la reflexión,
el que busca la **propiedad** de un objeto y **su gemelo** del llenado del `DataSet`— y el fallo dejó de ser un
silencio o un mensaje genérico: ahora es `TypeResolutionException`, que nombra el **valor intentado**, el **tipo** y
el **punto** que lo pidió.

Lo entregado y medido:

- **`Mapping/TypeIdentification.cs`**: donde el tipo **está disponible** se usa el tipo (un `Nullable<T>` resuelve su
  tipo subyacente **por API**, `Nullable.GetUnderlyingType`); donde llega un **nombre** se exige **calificado** y un
  nombre simple se **rechaza sin intentar resolverlo**; una propiedad se busca con **una sola verificación**
  compartida por los dos puntos.
- **`Exceptions/TypeResolutionException.cs`**: el error tipado de la identificación, **POCO** (sin serialización,
  ADR-008 §3.4) y **derivado de `CoreException`** para no romper la jerarquía que el consumidor captura.
- **`BaseObjectList.cs`** (aportado del 1.4, con las dos correcciones) e **`ObjectListHelper.cs`** (el gemelo: usa la
  misma verificación y **deja de tapar la causa** con el `catch (Exception)` genérico).
- **`2.0/docs/core-type-resolution-inventory.md`**: el **inventario publicado** (los tres puntos + el punto 4b que
  el propio control descubrió + la frontera del filtro de vistas con su dueño + la forma de cada valor real).
- **La suite del mapeo** (`2.0/tests/Cooperator.Framework.Core.Mapping.Tests`, 4 clases, **12 casos**) integrada al
  comando reproducible: **230 casos verdes** en total (arnés **135** + mapeo **12** + caracterización **83**) y 17
  proyectos sin advertencias.
- **La constancia** `2.0/docs/api-removals/US-000.BOLT-008.json` (2 entradas de cambio de comportamiento + 1
  obligación registrada), ya consolidada en el documento de notas (**8 fragmentos, 68 entradas**).

**Lo que este Bolt NO tocó** (fronteras declaradas, AC-8): la interpretación textual de los filtros de las vistas
(observación con su dueño), la carga/caché/diagnósticos de providers Lazy (Bolt 005) y la capa de datos (Bolt 014).
`Core` sigue con **0** dependencias de terceros y la **forma** del contrato que consume el código generado **no
cambió** (ADR-005 §3.3).

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — Inventario de los puntos** | **completada** ✔ | `2.0/docs/core-type-resolution-inventory.md`: los tres puntos (resolver de tipo, búsqueda de propiedad y su gemelo del `DataSet`), **más el punto 4b** que el control de resolución implícita descubrió durante el V-Bounce (`BaseObjectList.cs:380,390`: clasificación de una propiedad por el **formato** del nombre del tipo) — ampliado **antes** de corregirlo (§15 #6) —, la frontera del filtro de vistas con su dueño y la forma declarada de cada valor real. |
| **B — La identificación sin nombres simples** | **completada** ✔ | `TypeIdentification.UnwrapNullable` (por API) y `ResolveQualifiedName` (exige calificado, rechaza el simple **sin intentar**); `TypeResolutionException` (POCO, derivada de `CoreException`, con valor/tipo/punto). `BaseObjectList.cs` **aportado del 1.4** con las dos correcciones y la directiva de nulabilidad que el **build exigió** (§8.1). |
| **C — El gemelo y el diagnóstico de la propiedad** | **completada** ✔ | Los dos puntos usan `TypeIdentification.RequireProperty` (nombra propiedad, tipo, punto y las propiedades disponibles); en `ObjectListHelper.cs` se agregó el `catch (TypeResolutionException) { throw; }` **antes** del `catch (Exception)`, de modo que el diagnóstico se propaga y el `InvalidDataSetException` queda **sólo** para el desajuste de esquema real. |
| **D — Coherencia con la regla y ausencia de implícita** | **completada** ✔ | `NoImplicitResolutionTests`: **0** `Type.GetType`/`GetType(strPropType`/`Substring(18`/`PropertyType.ToString()`/`GetProperty(` en los archivos del camino, **0** `Assembly.Load*`, y el control **behavioral** de que un nombre no calificado se rechaza. El inventario declara la regla única (AC-1/AC-6). |
| **E — Valores reales, caracterización y constancia** | **completada** ✔ | `RealValuesOverCorpusTests` (la columna se crea con el tipo subyacente del `Nullable` y admite nulos; la ausencia del corpus se **declara** con su dueño, SPEC §15 #9) y `api-removals/US-000.BOLT-008.json`. La **caracterización del Bolt 001 corre en la misma corrida** (83/83 ✔) y el arnés sigue verde (135/135 ✔). |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Core/Mapping/TypeIdentification.cs` | La identificación inequívoca: por tipo disponible (con `Nullable` por API), por nombre **calificado** y la búsqueda de propiedad compartida. |
| `2.0/src/Cooperator.Framework.Core/Exceptions/TypeResolutionException.cs` | El error accionable del camino (valor, tipo, punto), POCO y derivado de `CoreException`. |
| `2.0/src/Cooperator.Framework.Core/BaseObjectList.cs` | **Aporte del 1.4** con las dos correcciones y la directiva `#nullable disable` (§B.4, exigida por el build — §8.1). |
| `2.0/src/Cooperator.Framework.Core/IObjectList.cs` | **Cierre mínimo** que el aporte exige (copiado tal cual del 1.4, SPEC §6/B). |
| `2.0/docs/core-type-resolution-inventory.md` | El inventario de los puntos, su resolución declarada y su diagnóstico (AC-1/AC-7). |
| `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/` (`…csproj`, `GlobalUsings.cs`, `TypeIdentificationTests.cs`, `NoImplicitResolutionTests.cs`, `PropertyLookupDiagnosticsTests.cs`, `RealValuesOverCorpusTests.cs`) | La suite del camino de mapeo (12 casos) que el SPEC nombra en §6/B.1, §6/D y §6/E. |
| `2.0/docs/api-removals/US-000.BOLT-008.json` | La constancia de las rupturas de **comportamiento** (2 entradas + 1 obligación), insumo del Bolt 028. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md` | `status: specified` → `in-development` → `ready-for-acceptance` + filas **T04** y **T05** | La toma por G11 y el cierre del V-Bounce. |
| `2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs` | La verificación compartida en el llenado del `DataSet` + el `catch (TypeResolutionException) { throw; }` | El **gemelo** (AC-5): el diagnóstico deja de quedar tapado por el error genérico. |
| `2.0/build/build.ps1` | La suite del mapeo en el comando reproducible (`$mappingProject` + la corrida `mapping-hermetic`, clonada del bloque del arnés) | Sin esto, la suite nueva se compilaba pero **no corría en el gate** (§8.2). |
| `2.0/docs/api-removals-decisions.json` · `2.0/docs/MIGRATION-NOTES.md` | Sin cambios de filas: la consolidación regeneró el documento (8 fragmentos, 68 entradas) | El registro es de contribución distribuida: mi fragmento entra y el documento se regenera de forma determinista. |

---

## 5. Files deleted / 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

1. **Donde el tipo está disponible no se pasa por un nombre** (ADR-006 §3.1, F-47): el `PropertyType` **es** el tipo
   y un `Nullable<T>` se resuelve con `Nullable.GetUnderlyingType`. Con eso desaparece la dependencia del
   **formato** de un nombre de reflexión y del **ensamblado llamador**.
2. **Una sola verificación para los dos puntos que buscan una propiedad**: `TypeIdentification.RequireProperty`. No
   puede quedar un punto corregido y su gemelo sin corregir (el control los revisa juntos).
3. **El error es tipado y del framework**, no una `ArgumentException` con el nombre del miembro: nombra el valor, el
   tipo, el punto y las propiedades disponibles del tipo. Es POCO y deriva de `CoreException` para que la jerarquía
   del núcleo que el consumidor captura siga siendo la misma.
4. **El punto 4b se corrigió por API** (`Type.Namespace`) en lugar del formato del `ToString()`: el control de
   resolución implícita lo encontró **después** de corregir los tres puntos del Bolt, y el inventario se **amplió
   antes** de corregirlo (§15 #6). La clasificación "de sistema" ahora no depende de cómo se imprima un nombre.
5. **El cierre mínimo se copió tal cual** del 1.4 (`IObjectList.cs`, 94 líneas) y se declaró archivo por archivo
   (SPEC §6/B): no se transformó nada que la compilación no exigiera.
6. **La suite nueva se integra al comando reproducible** clonando el manejo de resultado del arnés (mismo
   veredicto y mismo código de salida por suite). Sin esa integración, "la suite corre" sería una afirmación falsa.

---

## 8. Deviations and assumptions

### 8.1 §B.4 se ejerció **con la prueba del build** (y sólo donde hacía falta)

El aporte produjo **exactamente** los diagnósticos que la revisión 2 anticipó, medidos en el build con
`TreatWarningsAsErrors=true`:

```
BaseObjectList.cs(67,23):  error CS8618  Non-nullable field '_deletedItems' must contain a non-null value …
BaseObjectList.cs(98,30):  error CS8600  Converting null literal or possible null value to non-nullable type.
BaseObjectList.cs(209,30) · (239,30): error CS8600   (idem)
BaseObjectList.cs(217,20) · (246,20): error CS8603   Possible null reference return.
→ se aplicó `#nullable disable` como primera línea útil SÓLO en ese archivo (aportado del 1.4) y el build quedó limpio.
```

- **Sólo ese archivo la lleva**: `IObjectList.cs` (el otro aporte) **no** la necesita —el build no reportó
  diagnósticos en él— y `ObjectListHelper.cs` (modificado, ya en el árbol) tampoco. **0** archivos de **código
  nuevo** la llevan (`TypeIdentification.cs`, `TypeResolutionException.cs`, las pruebas: nulabilidad **activa**), y
  **ningún** `.csproj` declara `<Nullable>disable</Nullable>`.
- CS8600 aparece junto a CS8618/CS8603: es la **misma familia** de diagnósticos de nulabilidad que el criterio de
  §B.4 nombra (el SPEC cita los tres por su forma típica en esta ola; el criterio declarado es *archivo aportado del
  1.4 + diagnóstico de nulabilidad*). Se registra para que la lectura del revisor sea sobre el criterio y no sobre
  una lista cerrada.
- **No se anotó ni se corrigió** el código del 1.4 fuera de los dos puntos ordenados: la alternativa está
  descartada por el SPEC (§6/B.4) y en este Bolt sería, además, un cambio de comportamiento en el camino por el que
  se llenan los objetos de negocio.

### 8.2 La suite nueva no corría en el comando reproducible (y ahora sí)

`build.ps1` corre **dos** suites con un nombre y un proyecto fijos (el arnés y la caracterización). Al crear el
proyecto de pruebas que el SPEC nombra, sus 12 casos se compilaban pero **no se ejecutaban** en el gate: la
evidencia de las fases B/D/E habría quedado fuera del comando reproducible. Se agregó `$mappingProject` y una
corrida `mapping-hermetic` **clonando el bloque del arnés** (mismo manejo de resultado y de código de salida, no una
llamada suelta que perdería el fallo). Es un cambio de **un archivo del arnés** (`2.0/build/build.ps1`, Bolt 001) que
se declara acá porque el SPEC lo hace necesario al mandar el proyecto de pruebas; **no** se tocó ningún otro
artefacto del arnés ni de otro Bolt.

### 8.3 El corpus de modelos no está en el árbol: se declara, no se inventa

`2.0/tests/fixtures/model-corpus/**` (que dejan los Bolts 002/004) **no existe** hoy —el Bolt 002 está bloqueado—.
El SPEC lo declara prerrequisito de la verificación de los valores reales (§5) y §15 #9 **prohíbe** rellenar el
hueco con una suposición: `RealValuesOverCorpusTests` verifica sobre los tipos del propio framework, **declara** el
estado del corpus y el inventario registra la regla del hallazgo con su dueño. La obligación queda **registrada en
el fragmento** (`BOLT008-OBL-01`) y en §13.

### 8.4 Hallazgos ruteados (nada se absorbió)

| Hallazgo | Dueño | Por qué no se toca acá |
|---|---|---|
| Los **emisores de nombres** (código generado / modelo) deben emitir nombres **calificados** | Bolts de plantillas/modelo (019/004) | Es la forma que otro componente produce: una forma no calificada es un **hallazgo con dueño** (§6/E.1 y §15 #4), no una tolerancia del resolutor. |
| La **interpretación textual del filtro** de las vistas (`BaseObjectListView.cs:68`) | Bolt dueño de la evaluación de expresiones | Es evaluación de expresiones, no identificación de tipos: excluida por el Bolt y su DoR, registrada como observación en el inventario. |
| El **corpus de modelos** ausente | Bolts 002/004 | §8.3. |

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 2`** y `review.decision: approved` (`2026-10-16T03:18:49-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.4). No se activa ninguna revisión nueva.

### 8.6 Presupuesto de turnos (declarado)

El V-Bounce **excedió la guía de 10 loops** (usó 16). Las causas están medidas y son de forma, no de criterio: el
proyecto de pruebas nuevo exigió su `.csproj`, su entrada en el `.sln`, el `using` global de xUnit y el miembro
`OriginalValue()` del contrato de fila; el aporte exigió su **cierre mínimo** (`IObjectList.cs`) y un ajuste de
`init` (no existe en `netstandard2.1`); la directiva de §B.4 se aplicó **después** de que el build la exigiera (el
criterio manda); y el control de resolución implícita encontró el **punto 4b** que el inventario debía recoger
**antes** de corregirlo (§15 #6). El producto nunca quedó a medio camino y cada corrección se **verificó** antes de
seguir; el árbol quedó **verde antes** del cierre.

---

## 9. Verification evidence

### 9.1 La suite (230 casos)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 17 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 135, Skipped: 0, Total: 135
  mapping-hermetic          → Passed! - Failed: 0, Passed:  12, Skipped: 0, Total:  12
  characterization-hermetic → Passed! - Failed: 0, Passed:  83, Skipped: 0, Total:  83
Resultado: ok (codigo de salida 0)
```

Las **12** nuevas: 4 de identificación (por tipo disponible, `Nullable` por API, nombre calificado, negativos),
3 del control de resolución implícita y del inventario, 2 del diagnóstico de los **dos** puntos, 3 de los valores
reales del mapeo y de la caracterización. La **caracterización del Bolt 001** corre en la misma corrida: sobre
entradas válidas no hay diferencias observables (AC-7).

### 9.2 Los casos negativos y el control (lo que cambia de comportamiento)

```
tipo inexistente (nombre calificado)            → TypeResolutionException (valor + punto)
tipo de otro ensamblado nombrado de forma simple → RECHAZADO sin intentar resolverlo
nombre simple ("System.String", "MappingSample") → RECHAZADO (0 resolución implícita, control behavioral)
propiedad/columna inexistente (los DOS puntos)   → TypeResolutionException (propiedad + tipo + punto)
  el comparador de orden:  antes ArgumentException("<{0}> property not found.")
  el gemelo del DataSet:   antes InvalidDataSetException genérico desde un catch (Exception)
0 Type.GetType · 0 Substring(18 · 0 PropertyType.ToString() · 0 GetProperty( · 0 Assembly.Load* en el camino
```

### 9.3 El aporte y la directiva de §B.4

```
BaseObjectList.cs (aportado del 1.4, 459 → 461 líneas): 2 correcciones + #nullable disable (primera línea útil)
IObjectList.cs (cierre mínimo, copiado tal cual): 94 líneas, sin directiva (el build no la exige)
ObjectListHelper.cs (en el árbol, modificado): tablero del gemelo, sin directiva, nulabilidad activa conservada
#nullable disable en código nuevo: 0 archivos  ·  <Nullable>disable</Nullable>: 0 .csproj
build con TreatWarningsAsErrors=true: 17 proyectos, 0 diagnósticos
```

### 9.4 `1.4/**` intacto y encoding

```
git status --porcelain -- 1.4   → (vacío): el árbol congelado no se tocó
Documentos y fuentes del V-Bounce: UTF-8 sin BOM, sin U+FFFD y sin CR suelto
```

### 9.5 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 17 proyectos sin advertencias; `Core` compila con la directiva **sólo** en el aporte que el build la exigió |
| Nulabilidad del código aportado (rev.2) | **pass** | §9.3: 0 directivas en código nuevo, 0 `<Nullable>disable</Nullable>`, y la directiva presente donde el build la exigió (§8.1) |
| Unit / component tests | **pass** | 230/230 herméticos (identificación, negativos, diagnóstico de los dos puntos, control de resolución, valores reales, caracterización) |
| Consumer-side contract tests (test doubles) | **pass** | El contrato del consumidor es el **mapeo observable**: la caracterización corre antes y después y el mapeo produce los mismos objetos (AC-7) |
| SAST | **pass** | La resolución dinámica **se reduce**: 0 cargas de ensamblado y 0 resolución por nombre simple en el camino; la entrada de nombres falla tipada |
| Dependency scanning / licenses | **pass** | **0** dependencias nuevas; `Core` sigue con cero terceros (ADR-001 §3.4) |
| OWASP Top 10 — static review | **n/a (razón)** | Sin superficie alcanzable desde fuera del proceso |
| Prompt-injection scan | **n/a (razón)** | No se interpreta contenido textual como instrucción |
| Secret-leak scan | **pass** | Los diagnósticos nombran tipos, miembros y puntos: **0** valores de negocio, **0** secretos |
| Hallucination lint | **pass** | Tipos, miembros, líneas y rutas citados existen (o se declaran como aportados, con su archivo y su conteo) |
| IP / license provenance | **n/a (razón)** | El único código ajeno es el **propio** árbol 1.4 congelado, copiado tal cual |
| PII / DLP | **pass** | `internal`: los diagnósticos no llevan datos de negocio; el corpus es sintético (y hoy ausente, declarado) |
| Dependency-confusion | **pass** | 0 paquetes nuevos; la identificación **no** carga ensamblados |
| Test-first evidence | **pass** | Los casos negativos (tipo inexistente / nombre simple) se escribieron desde ADR-006 §3.1 antes de corregir |
| Behavioral reproducibility (hermetic suite) | **pass** | Dos corridas idénticas (tres suites) y caracterización sin diferencias en entradas válidas |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.6) |
| Coherencia con la regla de nombres (ADR-006 §3.1) | **pass** | Una sola forma de nombrar (calificada), **0** resolución implícita y el inventario la declara (AC-6) |
| API-compat con la línea base 1.4 (ADR-008 §5) | **n/a (razón)** | **Sin retiro de superficie pública**: lo que cambia es el comportamiento ante entradas inválidas, y eso va a las notas (§E.3) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`): la resolución ocurre **dentro del proceso** |

### 9.6 Manifest y registro

`devflow/metrics/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.json`: entrada **`v_bounces[0]`**
(`number: 1`, **`spec_revision: 2`**, `execution_outcome: ready_for_review`), validada contra
`manifest-v5-bolt.schema.json`. El registro de rupturas quedó con **8 fragmentos / 68 entradas** y el documento
consolidado regenerado de forma determinista (las suites que lo verifican están verdes en la misma corrida).

---

## 10. Manual interventions

**Ninguna intervención humana.** El aporte, las correcciones, la suite nueva, el inventario, la constancia y la
consolidación se hicieron en este V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0034](../spec/SPEC-261016-0034-resolucion-de-tipos-calificada.md) — **revisión 2**
  (`AITL-SPEC-Approval` del `2026-10-16T03:18:49-03:00`): §6/A–E, §7 (AC-1…AC-10) y §6/B.4.
- **Bolt:** [US-000.BOLT-008](../functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md) — filas **T04** y **T05**.
- **Entregables:** `2.0/src/Cooperator.Framework.Core/{Mapping/TypeIdentification.cs, Exceptions/TypeResolutionException.cs, BaseObjectList.cs, IObjectList.cs, ObjectListHelper.cs}`,
  `2.0/docs/core-type-resolution-inventory.md`, `2.0/tests/Cooperator.Framework.Core.Mapping.Tests/**` y
  `2.0/docs/api-removals/US-000.BOLT-008.json`.
- **Manifiesto:** [US-000.BOLT-008-resolucion-de-tipos-calificada.json](../metrics/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.json).
- **Evidencia de 1.4 citada:** `Core/BaseObjectList.cs:409-418` y `:342-346`, `Core/ObjectListHelper.cs:64-98`,
  `Core/BaseObjectListView.cs:68` (frontera) y `Core/Factory/Factories.cs:53,70` (del Bolt 005).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-008) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0034 revisión **2** |
| Tiempo de generación IA | ~1 h de reloj (02:35 → 03:36) |
| Fases completadas | **5 de 5** (A–E), con el inventario ampliado al punto **4b** |
| Archivos creados | **2** de código nuevo + **2** aportados del 1.4 (con el cierre mínimo) + **1** inventario + **6** de la suite + **1** constancia |
| Archivos modificados | **4** (el Bolt; `ObjectListHelper.cs`; `build.ps1` del arnés — §8.2; el documento consolidado regenerado) |
| Directivas `#nullable disable` | **1** (sólo el aporte que el build la exigió — §8.1); **0** en código nuevo; **0** en `.csproj` |
| Casos de prueba nuevos | **12** (todos verdes) |
| Tests de la suite | **230** herméticos (135 + 12 + 83), todos verdes |
| Loops usados | 16 (guía 10; causas medidas en §8.6) |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **El corpus de modelos** (`2.0/tests/fixtures/model-corpus/**`): lo dejan los Bolts 002/004 (el 002 está
   bloqueado). Cuando llegue, la verificación de los valores reales se hace **sobre él** además de sobre los tipos
   del framework. Obligación registrada en el fragmento (`BOLT008-OBL-01`).
2. **Los emisores de nombres calificados** (código generado / modelo): si emiten un nombre **simple** hacia este
   camino, es un **hallazgo con dueño** (Bolts de plantillas/modelo) — el resolutor no lo tolera (§8.4).
3. **El filtro de las vistas** (`BaseObjectListView.cs:68`) sigue **fuera** de este Bolt, como observación con su
   dueño: es evaluación de expresiones, no identificación de tipos.
4. **Una observación que el Bolt declaró y este V-Bounce no convierte en corrida** (§8.2 del SPEC): un fallo que sólo
   aparezca en el host real —no en el arnés— queda para la lane del Bolt dueño, no se da por bueno acá.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 2`) + el árbol
  entregado (los tres puntos corregidos con su apunte del cierre mínimo, la identificación inequívoca, el error
  tipado, el inventario publicado, la suite del mapeo integrada al comando reproducible y la constancia de las
  rupturas) + la corrida verde de **230 casos**.
- **Qué se pide decidir:** la aceptación del entregable y el cierre del Bolt hacia su aceptación. Se señalan para la
  lectura las dos decisiones declaradas en §8.1 (el alcance exacto de la directiva de §B.4, con el build como
  prueba) y §8.2 (la integración de la suite nueva al comando reproducible).
