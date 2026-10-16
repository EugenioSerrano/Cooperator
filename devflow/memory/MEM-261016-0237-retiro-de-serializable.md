---
id: "MEM-261016-0237"
title: "Retiro de `[Serializable]`/`SerializationInfo` (SPEC rev.3) — V-Bounce N°3: aporte y controles ejecutados y medidos, cerrado `blocked` por presupuesto con los tres defectos de test diagnosticados"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-007" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0013" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 3 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 3 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-008", "ADR-002", "ADR-001"] # ADR-008 §3.2/§3.3/§3.4, ADR-002 §3.2 y ADR-001 §3.1/§3.4 — ninguno materializó un cambio: los aportes se revirtieron (§6)
manifest: "US-000.BOLT-007-retiro-de-serializable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §6) — working tree only, no git operations (G34)
review_ready_at: "2026-10-16T02:37:13-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:39:49-03:00"
  decided_at: "2026-10-16T02:39:49-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°3 `blocked` por condición de parada §15 **#6** (la suite no llega a verde) más el presupuesto de turnos **superado** mientras convergía; SPEC `SPEC-261016-0013` revisión 3 gobernante; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — árbol revertido y verde, verificado: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **90/90 harness + 83/83 caracterización, exit 0** (16 proyectos, 173 casos); la **arista `Data → Library` revertida** (0 `ProjectReference` en `Cooperator.Framework.Data.csproj`); la **aserción del 011 restaurada** (`Assert.DoesNotContain("Cooperator.Framework.Data", layer)` presente en `HostConfigurationAbsenceTests.cs`); `1.4/**` intacto."
    - "Compliant — clasificación de los 3 defectos correcta: son de los **dos archivos de test nuevos** (`SerializationSurfaceTests` y `BinaryTrailTests`), no del entregable de producción, que compila `EXIT=0` con 0 diagnósticos; **2 con corrección exacta** (el fallback del nombre de tipo para `BaseObject` en la raíz de `Core` —usar la convención de carpeta sólo para `Exceptions/`—; y el alcance del control de rastro —acotar a `2.0/src/**` + proyectos, excluyendo las suites verificadoras con su razón declarada—); el **3°** (aserción de jerarquía con mensaje vacío sobre `Modeling`) declarado **sin diagnóstico preciso**."
    - "ADJUDICACIÓN (3° defecto sin diagnóstico preciso) — la declaración honesta es **SUFICIENTE** para el N°4, no exijo un diagnóstico antes: (a) el producto está `EXIT=0` y los 3 rojos son del test nuevo; (b) el fallo «mensaje vacío» (una excepción sin texto, no una aserción) es en sí una pista que apunta a la **resolución de metadatos** de `Modeling`, con hipótesis declarada y **corrección probable** (resolver cada ensamblado con el contexto de los **4** DLLs del alcance, no re-crear por ensamblado); (c) exigir un diagnóstico previo gastaría un V-Bounce en diagnosticar en vez de diagnosticar+corregir en la misma pasada. Suficiente y honesta."
    - "Compliant — lección de proceso declarada y correcta: `String.Replace()` **sin coincidencia es silencioso** en PowerShell —el parche de alcance de `BinaryTrailTests` no se aplicó y se detectó por el síntoma—; la lección («los parches deben **verificar la sustitución**») está bien registrada y es la causa raíz del defecto #3."
    - "Compliant — trabajo medido que se reproduce (por muestreo en el MEM §9.1/§9.2): retiro de 32 archivos (0 residuos), inventario 34 entradas/39 declaraciones/31 constructores, directiva de §A.3 por criterio (2 archivos), arista de §B.4 (una línea), relajación de §C.2 (una aserción), constancia 32 entradas (31 con `member: "ctor"` cubriendo las `CP0002`); cierre T08/T09; MEM único; manifiesto 8 campos. **El N°4 no requiere revisión de SPEC**: mismo rev3 gobernante, sólo presupuesto/convergencia (§13 lista los 3 defectos con sus correcciones)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0237 — Retiro de `[Serializable]`/`SerializationInfo` (rev.3): ejecutado, convergido y detenido por presupuesto

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `type: non-functional`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md), **revisión 3** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T02:26:54-03:00`) |
| **V-Bounce**    | **3** (el N°1 es [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md) y el N°2 [`MEM-261016-0220`](MEM-261016-0220-retiro-de-serializable.md), ambos `blocked`) |
| **ADRs**        | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.4) |

---

## 1. Executive summary

Se ejecutó el entregable completo de este V-Bounce con las **tres autorizaciones** de la revisión 3 puestas en
práctica y **medidas**: el **aporte** (el retiro de los 32 archivos del alcance, con **0 residuos** de superficie de
serialización en `2.0/src` y **`EXIT=0` con 0 diagnósticos en los tres ensamblados**), la **§A.3** aplicada **por su
criterio objetivo** —el build reportó los archivos con diagnósticos y la directiva quedó **sólo** en
`BaseObject.cs` y `DescriptionFieldEventArg.cs**, jamás en código nuevo y en ningún `.csproj`—, la **§B.4** (la
arista `Data → Library` de una línea, que es la que hace compilar las 16 excepciones de `Data`), la **§C.2** (la
relajación acotada de la aserción del Bolt 011, con su comentario de procedencia y su referencia cruzada) y la
**constancia** de la fase F (`api-removals/US-000.BOLT-007.json`, 32 entradas —las 31 con `member: "ctor"` para
cubrir las `CP0002` de los constructores de serialización— más la corrección del arnés y el escape-hatch negativo).
Los dos controles nuevos (`SerializationSurfaceTests` y `BinaryTrailTests`, fases E y D) se escribieron y se
**midieron en bucle**: el árbol pasó de **7 a 3** pruebas rojas, cada iteración con una causa distinta y precisa
(resolver de metadatos incompleto, un `Path.GetFileNameWithoutExtension` aplicado a un nombre con puntos, las bases
abstractas con constructor `protected`, un identificador de test que contenía el propio token que persigue, y las
suites que *verifican* la ausencia de esa superficie, que la nombran por diseño). Con el presupuesto de turnos
**agotado y superado** mientras convergía, se aplicó la disciplina del contrato —«los tests no llegan a verde
dentro del presupuesto → parar»— y, para **no dejar el árbol en un estado intermedio**, **se revirtieron los
aportes**: el árbol queda **verde y como estaba** (16 proyectos, 0 advertencias; arnés **90/90**; caracterización
**83/83**; `EXIT=0`). Los **tres defectos de test** que faltaban quedan **diagnosticados** (§8.2) —dos de ellos con
su corrección exacta—, de modo que el próximo V-Bounce cierra el Bolt con trabajo acotado y ya identificado.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — Inventario «antes»** | **completada** ✔ | 34 entradas, **39** declaraciones `[Serializable]`, **31** constructores de serialización (todos `protected`), 0 `ISerializable`, 0 `GetObjectData`, **0 contexto** (§A.2 PASA) y la clasificación por dueño (AC-7). |
| **A.3 — Nulabilidad (rev.3)** | **completada por criterio** ✔ | Directiva en **2** archivos (`BaseObject.cs`, `DescriptionFieldEventArg.cs`), que son exactamente los que el build reportó con `CS8618`; `ObjectStateHelper.cs` **no** la lleva; **0** archivos nuevos con la directiva; **0** `.csproj` con `<Nullable>disable</Nullable>`. |
| **B — El retiro** | **completada y revertida** | 32 archivos en su forma retirada (atributo + constructor + el `using` sin uso), los **3 `[field: NonSerialized]`** fuera, constructores públicos y jerarquía intactos; **0 residuos**; los 3 ensamblados `EXIT=0` con 0 diagnósticos. |
| **B.4 — La arista `Data → Library`** | **completada y revertida** | El `ProjectReference` de una línea con su comentario de procedencia; sin él, `Data` no compila (`CS0234`/`CS0246`, medido en el N°2). |
| **C.2 — La relajación del arnés (rev.3)** | **completada y revertida** | Ejercida sobre la **única** aserción autorizada: `Assert.DoesNotContain("Cooperator.Framework.Data", layer)` → **`Assert.NotEmpty(layer)`**, con el comentario de procedencia (§C.2, `MEM-261016-0220` §8.1), la derivación de las capas intacta y las **dos aserciones positivas** (`Core`, `Library`) intactas. |
| **C — Frontera (verificación)** | **completada** ✔ | Los tipos del grafo y sus excepciones → Bolt 003; `CompilerException` → Bolt 017; `Snapshot`/plantillas → 002/025 y el set. Nada se transformó (AC-7). |
| **D — Rastro binario (test)** | **escrito y en convergencia** | `BinaryTrailTests.cs` (3 casos): rastro, allowlist del migrador y supresiones acotadas. Queda **1 defecto** (§8.2.2). |
| **E — Gate de superficie (test)** | **escrito y en convergencia** | `SerializationSurfaceTests.cs` (3 casos) con `MetadataLoadContext` sobre los 4 ensamblados: 0 atributo, 0 `GetObjectData` declarado, 0 constructores de serialización, 0 interfaz declarada por cuenta propia, la jerarquía y los constructores públicos, y el inventario. Quedan **2 defectos** (§8.2.1). |
| **F — Constancia** | **completada y revertida** ✔ | `2.0/docs/api-removals/US-000.BOLT-007.json`: **32 entradas** (qué desaparece, por qué, con qué se reemplaza, dueño y evidencia), la **corrección del arnés** con archivo/línea/antes-después (§C.2.3) y el **escape-hatch negativo** con sus cinco criterios. |

---

## 3. Files created

Todos **se generaron y se midieron en este V-Bounce, y se revirtieron al cerrarlo** (§6).

| Path | Propósito |
|---|---|
| `2.0/tests/fixtures/serialization-surface/baseline-inventory.json` | El inventario «antes» (fase A) con las dos columnas, las declaraciones, los constructores, las anotaciones y la precondición de §A.2. |
| `2.0/src/Cooperator.Framework.{Core,Data,Library}/Exceptions/*.cs` (**27 nuevos + 4 transformados**) | Las excepciones del alcance en su forma retirada. |
| `2.0/src/Cooperator.Framework.Core/BaseObject.cs` y su cierre `{ObjectStateHelper,DescriptionFieldEventArg}.cs` | `BaseObject` retirado (atributo + 3 `[field: NonSerialized]`) **con la directiva de §A.3**, y el cierre verbatim (sólo `DescriptionFieldEventArg.cs` con la directiva). |
| `2.0/tests/Cooperator.Harness.Tests/SerializationSurfaceTests.cs` | El gate de superficie (fase E). |
| `2.0/tests/Cooperator.Harness.Tests/BinaryTrailTests.cs` | El control de rastro binario y de supresiones (fase D). |
| `2.0/docs/api-removals/US-000.BOLT-007.json` | La constancia de rupturas (fase F), con la corrección de §C.2 y el escape-hatch negativo. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-007-retiro-de-serializable.md` | `status: blocked` → `in-development` y fila **T08**; al cerrar, `in-development` → `blocked` con la fila **T09** | Las transiciones de estado del Bolt con su fila de `Status timeline`. |
| `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` | El `ProjectReference` a `Library` (§B.4), **revertido** al cerrar | La arista autorizada por la revisión 3. |
| `2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs` | La **única** aserción de §C.2, **revertida** al cerrar | La relajación acotada autorizada por la revisión 3. |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Todo lo de §3 se **revirtió** y los **4 archivos preexistentes** del alcance se **restauraron verbatim** desde el
1.4, de modo que el árbol queda **idéntico** al estado inicial y **verde**:

```
Core: 28 archivos .cs · Data: 0 · Library: 3       (como al inicio)
arista Data → Library: quitada · aserción de §C.2: restaurada a su texto original
inventario, 2 tests nuevos y constancia: retirados
arnés 90/90 · caracterización 83/83 · 16 proyectos sin advertencias · EXIT=0        (§9.3)
1.4/**: git status vacío                                                             (§9.4)
```

---

## 7. Implementation decisions

1. **La §A.3 se aplicó por medición, no por nombre** (el criterio del SPEC): el generador construye el árbol, el
   build reporta los archivos con diagnósticos y la directiva entra **sólo** en esos. Resultado: **2** archivos, y
   `ObjectStateHelper.cs` sin directiva porque no produjo ninguno.
2. **Los tokens de los dos controles se ensamblan desde fragmentos** (igual que el control de rastro del Bolt 006):
   un archivo de test no puede ser su propia ocurrencia.
3. **La constancia cubre por máquina**: las 31 entradas de excepción llevan `member: "ctor"`, que es el nombre que
   el cruce de `ApiRemovalCoverageTests` extrae de las diferencias `CP0002` de los constructores de serialización —
   la falla que el V-Bounce N°2 reclamaba y que la fase F cierra.
4. **Los dos puntos que el V-Bounce N°2 declaró como trampas se ejercitaron sin dispararse**: el contrato de claves
   no está en juego acá y la autorización de §A.3 **no** se extendió (0 directivas en código nuevo, 0 `.csproj`).

---

## 8. Deviations and assumptions

### 8.1 Por qué se detuvo: el presupuesto de turnos, superado mientras convergía (declarado)

El árbol pasó de **7 → 5 → 3** pruebas rojas en tres iteraciones, cada una con una causa distinta y precisa (§8.2),
hasta agotar —y superar— el presupuesto de turnos de la ejecución. La regla del contrato es
explícita —«los tests no llegan a verde dentro del presupuesto de turnos → **paro**, MEM con el progreso»—, y **un
`blocked` no deja el árbol en un estado intermedio**: por eso se revirtió todo (§6) y el árbol quedó verde y como
estaba. **Lo que sí queda es el diagnóstico**: los defectos restantes son de los **dos archivos de test nuevos** (no
del entregable de producción, que compila `EXIT=0`) y dos de los tres tienen su corrección exacta identificada.

**Lección de proceso declarada:** en PowerShell, `String.Replace()` **sin coincidencia es silencioso**; uno de mis
parches de alcance (el de `BinaryTrailTests`) no se aplicó y lo detecté recién por el síntoma. Los parches de un
árbol deben **verificar la sustitución** (comparar longitud o re-leer el archivo), no confiar en que ocurrió.

### 8.2 Los tres defectos que faltaban, diagnosticados

1. **`SerializationSurfaceTests.Every_type_of_the_inventory_exists_in_the_built_assemblies`** — el *fallback* de
   nombre que agregué inserta `…Exceptions.<archivo>` para **cualquier** archivo, y `BaseObject` **no** vive en
   `Exceptions/` sino en la raíz de `Core`: su nombre correcto es `Cooperator.Framework.Core.BaseObject`.
   **Corrección exacta:** usar la convención de carpeta **sólo** cuando el `file` del inventario empieza con
   `Exceptions/`, y `Cooperator.Framework.{ns}.{archivo}` en cualquier otro caso.
2. **`SerializationSurfaceTests.The_hierarchy_and_the_public_constructors_of_the_scope_are_intact`** — falla en la
   línea 123 con un **mensaje vacío** (una excepción sin texto, no una aserción). Hipótesis declarada: la
   resolución de metadatos de `Modeling` al inspeccionar sus tipos (el ensamblado sólo referencia
   `netstandard`), que el resolvedor del contexto no completa. **Corrección probable:** resolver cada ensamblado con
   el contexto que ya incluye los **cuatro** DLLs del alcance y no re-crear el contexto por ensamblado (el
   `Cached`/`NewContext` actual mantiene un contexto por DLL). **No diagnosticado con precisión total** —se declara
   así en lugar de afirmarlo—.
3. **`BinaryTrailTests.The_binary_trail_is_confined_to_the_declared_allowlist`** — el parche de alcance **no se
   aplicó** (§8.1) y el control sigue marcando las suites que **verifican la ausencia** de esa superficie
   (`Cooperator.Framework.Modeling.Tests/**`, `ModelPocoTests`, y el propio `SerializationSurfaceTests`), que la
   nombran **por diseño**. **Corrección exacta:** acotar el escaneo a las **fuentes del producto** (`2.0/src/**`) y a
   los **archivos de proyecto** de todo el árbol, y excluir las suites verificadoras —con su razón declarada, la
   misma que el SPEC acepta para `1.4/**`—; o bien mover el escaneo de `.cs` a una lista de exclusión explícita y
   **verificada**.

### 8.3 Lo que la revisión 3 autorizó (ejercido y verificado)

| Autorización | Ejercida | Límite respetado |
|---|---|---|
| **§A.3** nulabilidad del código aportado | Sí — **2** archivos, los que el build reportó con `CS8618` | Sólo aportados **con diagnóstico**; **0** en código nuevo; **0** `.csproj` con la etiqueta |
| **§B.4** la arista `Data → Library` | Sí — una línea con su procedencia | Dirección permitida por ADR-001 §3.1; ninguna otra arista |
| **§C.2** la relajación de una aserción del Bolt 011 | Sí — **una** aserción de **un** archivo, con comentario de procedencia y referencia cruzada en el MEM y la constancia | Ninguna otra línea de ese archivo ni otro archivo del Bolt 011; ventana abierta (el 011 sigue `ready-for-acceptance`) |

### 8.4 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 3`, `review.decision: approved`
(`2026-10-16T02:26:54-03:00`); el Bolt entró en `blocked` con T01–T07 presentes y se tomó con **T08**; los ADR
aplicados siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.4). No hay cambio material → G15 no se activa.

### 8.5 Comprobación de las condiciones de parada (§15)

- **#1 (cambio material):** negativa (§8.4).
- **#2 (escape-hatch):** negativa — ningún tipo lo reclama; la decisión negativa está en la constancia (§2/F) con sus
  cinco criterios.
- **#3 (constructor con contexto):** negativa — **0** ocurrencias.
- **#4 (editar `1.4/`):** negativa.
- **#5 (transformar código de otro Bolt):** negativa — el cierre se aportó verbatim; lo único ajeno tocado fue la
  **única aserción autorizada por §C.2**.
- **#6 (un gate de §9 no cierra en `pass`):** **es lo que detuvo el V-Bounce** — la suite no llegó a verde dentro
  del presupuesto; se detuvo y se revirtió, y el árbol queda con los gates **verdes** en el estado en que se deja
  (§9.3).
- **#7 (arnés ausente):** negativa.
- **#8 (superficie no inventariada):** negativa — el inventario cubrió el alcance y el borde, todos con dueño.
- **§15 #10 (extender la excepción de §C.2):** **negativa** — se tocó **una** aserción de **un** archivo del Bolt
  011, y nada más.
- **La ventana del 011:** sigue en `ready-for-acceptance` (declarado en la constancia y acá) → no se disparó la
  parada por ventana cerrada.

---

## 9. Verification evidence

### 9.1 El aporte, medido

```
Inventario: 34 entradas · 39 declaraciones [Serializable] · 31 constructores (protected ×31) · 0 ISerializable
            0 GetObjectData · 0 constructores con contexto (§A.2 PASA) · 32 tipos en alcance
Retiro:     32 archivos en su forma retirada · 3 [field: NonSerialized] fuera · 0 residuos en 2.0/src
§A.3:       intento 1 → CS8618 en BaseObject.cs y DescriptionFieldEventArg.cs → directiva SOLO ahí
            intento 2 → sin diagnósticos · 0 archivos nuevos con la directiva · 0 .csproj con la etiqueta
§B.4:       arista Data → Library agregada (una línea)
Builds:     Core EXIT=0 · Data EXIT=0 · Library EXIT=0      (0 diagnósticos los tres)
```

### 9.2 Los dos controles nuevos: la convergencia, medida

```
Corrida 1: 7 rojas de 96   → resolver de metadatos (faltaban los 4 DLLs), Path.GetFileNameWithoutExtension sobre un
                             nombre con puntos, bases abstractas con ctor protected, el identificador `GetObjectDataName`
                             (contenía el token), las suites verificadoras, y mi `#pragma` en un comentario
Corrida 2: 5 rojas         → ruta del inventario corregida (tests/fixtures, no docs) + constancia generada
Corrida 3: 3 rojas         → bases abstractas y fallback de nombre de tipo aplicados; alcance del rastro NO aplicado
                             (el `.Replace` sin coincidencia fue silencioso — §8.1)
Estado de la constancia: 32 entradas, 31 con `member: "ctor"` (cubren las CP0002 de los constructores)
```

### 9.3 El árbol como se deja (tras la reversión)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 90, Skipped: 0, Total: 90
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
Core: 28 archivos .cs · Data: 0 · Library: 3 · arista revertida · aserción de §C.2 restaurada
```

### 9.4 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.5 Gates (§9)

No se declaran gates en `pass` **del entregable** (se revirtió); el **estado del árbol** que se deja tiene sus gates
verdes (§9.3). El gate de tests es el que detuvo el V-Bounce, con la causa diagnosticada (§8.2).

### 9.6 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json`: se agregó la entrada **`v_bounces[2]`**
  (`number: 3`, `spec_revision: 3`, `execution_outcome: blocked`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23).

---

## 10. Manual interventions

**Ninguna intervención humana.** El aporte, las directivas, la arista, la relajación, la constancia, los dos
controles y la reversión fueron actos del V-Bounce. **Tampoco hubo operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol quedó **sin cambios** (§6, §9.3).
- **SPEC ejecutado:** [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md) — **revisión 3**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T02:26:54-03:00`): §6/A.3, §6/B.4 y §6/C.2.
- **Bolt:** [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — filas **T08** y
  **T09**.
- **Los V-Bounces previos:** [`MEM-261016-0209`](MEM-261016-0209-retiro-de-serializable.md) (N°1: la medición que
  originó §A.3 y §B.4) y [`MEM-261016-0220`](MEM-261016-0220-retiro-de-serializable.md) (N°2: la medición de la
  aserción ajena que originó §C.2).
- **Manifiesto:** [US-000.BOLT-007-retiro-de-serializable.json](../metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json)
  — `v_bounces[2]`.
- **El artefacto ajeno tocado (y revertido):** `2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs`,
  la aserción de `The_absence_control_states_which_layer_it_measured` (Bolt 011).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 3 (de US-000.BOLT-007) — `blocked` |
| SPEC ejecutado | SPEC-261016-0013 revisión **3** (aprobada antes de arrancar) |
| Autorizaciones ejercidas | **3 de 3** (§A.3, §B.4, §C.2) — con sus límites verificados |
| Archivos entregados al árbol | **0** (reversión completa, §6) |
| Archivos generados y revertidos | 32 del alcance + 2 de cierre + 1 inventario + 2 tests + 1 constancia (+1 arista y 1 aserción revertidas) |
| Iteraciones de convergencia | **3** (7 → 5 → 3 pruebas rojas), cada una con causa precisa |
| Defectos restantes | **3**, de los dos archivos de test nuevos: **2 con corrección exacta**, 1 no diagnosticado con precisión total (§8.2) |
| Tests corridos como control | **173** (90 + 83), todos verdes tras la reversión |
| Condición de parada | §15 **#6** (la suite no llega a verde) + el presupuesto de turnos **superado** mientras convergía (§8.1) |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |

---

## 13. Pending items and stubs (para el próximo V-Bounce)

1. **Los 3 defectos de test** (§8.2), con la corrección exacta de dos de ellos: la convención de carpeta del nombre
   de tipo (sólo para `Exceptions/`), el alcance del control de rastro (producto + proyectos, o lista de exclusión
   **verificada**) y el diagnóstico del fallo sin mensaje de la aserción de jerarquía (revisar la resolución de
   metadatos de `Modeling` con el contexto completo del alcance).
2. **Todo lo demás está medido y se reproduce en minutos** (§9.1, §9.2): el retiro de los 32 archivos, el inventario
   con dueños, la directiva de §A.3 por criterio, la arista de §B.4, la relajación de §C.2 (una aserción, con su
   comentario) y la constancia con sus 32 entradas y la cobertura de las `CP0002`.
3. **La frontera sigue en pie:** los tipos del grafo (Bolt 003, ya POCOs en `Modeling`), `CompilerException`
   (Bolt 017), los `[DataContract]` del set, `Snapshot`/`BinaryFormatter` del Modeler (Bolts 002/025) y el
   **cargador del migrador** (Bolt 002): la **única** excepción declarada, que el control de rastro de la fase D
   trata como su allowlist y que hoy no existe en el árbol (por eso el control exige 0 usos y deja la regla activa).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[2]` del manifiesto (número 3, revisión 3) + el árbol **sin
  cambios** (§9.3) + el detalle medido de las tres autorizaciones ejercidas y de la convergencia de los controles
  (§9.1, §9.2).
- **Qué se pide decidir:** la aceptación del cierre por presupuesto y el arranque del próximo V-Bounce con los tres
  defectos de §8.2 ya diagnosticados.
