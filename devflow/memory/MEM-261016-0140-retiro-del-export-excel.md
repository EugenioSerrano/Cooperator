---
id: "MEM-261016-0140"
title: "Retiro del export a Excel del núcleo: la capacidad y su rastro de plataforma fuera de 2.0, con la constancia de la superficie retirada cubierta por el gate"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-006" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0012" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008"] # ADR-001 §3.1/§3.4/§3.5 (el retiro decidido) y ADR-008 §3.1/§5 (la constancia y su cobertura por el gate)
manifest: "US-000.BOLT-006-retiro-del-export-excel.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T01:40:26-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:43:49-03:00"
  decided_at: "2026-10-16T01:43:49-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°2 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0012` revisión 2 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; grant `approves: [SPEC, MEM]`. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **59/59 harness + 83/83 caracterización, exit 0** (13 proyectos sin warnings). Verificado en el árbol: `ObjectListHelper.cs` en forma retirada (grep: sólo `class ObjectListHelper<T,T2>` L40, `FillDataSet` L48, `SqlName` L100 — sin `CreateExcelFile`/`xlsLock`/`OleDb`); `IObject.cs` con **sólo** L31 retirada (queda `using System.ComponentModel;`); `api-removals/US-000.BOLT-006.json` con **3 entradas** (BOLT006-01/02/03, firma en la forma del gate)."
    - "ADJUDICACIÓN 1 (AC-3 documentación) — ACEPTABLE: la lectura «0 usos vivos / 0 en código-proyectos-scripts» es la correcta —el `Then` de AC-3 y §8.1 piden exactamente eso— y las 2 menciones del rastro viven **sólo** en la constancia, donde §6/B.1 las **exige** («qué desaparece… y firma exacta»). La aserción dura (0 en código/proyectos/scripts) se cumple; la lectura estricta (0 también en documentación) contradiría §B.1. No exijo el cambio."
    - "ADJUDICACIÓN 2 (`SqlName` sobreviviente) — ACEPTABLE: §14 acota el retiro a **exactamente tres elementos** y §2/§A.1 nombran a `SqlName` como miembro que **sobrevive**; es un auxiliar **privado sin llamadores** (su único llamador era el método retirado), no aparece en la superficie pública, no afecta AC-1 ni el gate, y el build no emite ninguna advertencia (medido). Retirarlo excedería el alcance literal de §14. Lo acepto como está, con el señalamiento declarado; si se quisiera, es candidato de un Bolt de limpieza futuro, no de éste."
    - "Compliant — discrepancia 252 vs 280 líneas del cierre: misma composición (4 archivos), diferencia declarada y **no ajustada** — el árbol tiene lo que compila (280 líneas, `EXIT=0`). Aceptable; es varianza de medición (el 252 fue del V-Bounce N°1), no un defecto de alcance."
    - "Compliant — DoD: herméticos verdes (142/142); gates aplicables `pass`; el gate de API en **modo informativo** (`Current`, código 2): las **2 CP0002 cubiertas** (la de este Bolt + la de BOLT011-04), 132 CP0001 como brecha de porteo **declarada**, 0 diferencias de miembro retirado sin explicación; MEM completo; manifiesto válido."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0140 — Retiro del export a Excel: la capacidad y su rastro fuera de 2.0, con la constancia cubierta por el gate

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-006](../functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md) — `type: non-functional`, `risk_class: low`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0012](../spec/SPEC-261016-0012-retiro-del-export-excel.md), **revisión 2** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T01:08:28-03:00`) |
| **V-Bounce**    | **2** — el N°1 está en [`MEM-261016-0059`](MEM-261016-0059-retiro-del-export-excel.md) (`blocked`) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1/§3.4/§3.5 · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) §3.1/§5 |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se retiró del núcleo la capacidad de exportar a Excel y con ella todo su rastro de plataforma: el archivo del
helper de RowSet se aportó **en su forma ya retirada** —sin el método público `CreateExcelFile`, sin su auxiliar
privado exclusivo y sin el `using` del proveedor de datos de plataforma, con su XML-doc ajustado a lo que la clase
realmente hace— y con él entró el **cierre mínimo** de cuatro archivos que el núcleo necesita para compilar
(`IObject`, `ObjectState`, `CoreException`, `InvalidDataSetException`), copiados tal cual desde el 1.4 con **una
sola** transformación: el retiro del `using` **muerto** de `IObject.cs:31` que la revisión 2 autorizó como excepción
acotada y que estaba bloqueando el V-Bounce N°1. El build del núcleo compila sobre `netstandard2.1` con **`EXIT=0`,
0 errores y 0 advertencias** (advertencias tratadas como error), y el árbol entero —13 proyectos, incluido el
arnés con dos casos nuevos— queda **verde**: **59/59** en el arnés (eran 49) y **83/83** en la caracterización, sin
regresiones. La verificación es **dura y no declarativa**: la superficie se inspecciona sobre el **ensamblado
construido** con `MetadataLoadContext` (sin cargarlo ni ejecutarlo), la búsqueda de rastro recorre el árbol propio y
**clasifica** cada coincidencia, y la **constancia de la superficie retirada**
(`2.0/docs/api-removals/US-000.BOLT-006.json`) se **cruza contra el reporte real del gate** de compatibilidad: las
dos únicas diferencias de miembro retirado que el gate reporta hoy (la de este Bolt y la que ya declaró el Bolt
011) están cubiertas, y una diferencia sin entrada **falla** el caso. `1.4/**` quedó intacto frente al commit
baseline.

---

## 2. Implemented phases

### 2.1 Fase A — el retiro y el rastro ausente

| Paso de §6 | Qué se hizo | Estado |
|---|---|---|
| A.1 El archivo del núcleo sin la capacidad | `ObjectListHelper.cs` se aportó **en su forma ya retirada** (119 líneas frente a las 305 del 1.4): se retiraron **exactamente** los tres elementos que la capacidad introdujo —el método `CreateExcelFile` con su doc-comment, el campo `private static Object xlsLock` y el `using System.Data.OleDb;`— y se **ajustó el XML-doc de la clase**, que anunciaba «RowSet exportation», para describir lo que la clase realmente hace. `FillDataSet` y `SqlName` se conservan tal cual. | hecho |
| A.1 Cierre mínimo (aportado tal cual) | Los **4 archivos** del cierre: `IObject.cs` (49 líneas de las 50 del 1.4 — ver §8.2, la **única** transformación autorizada), `ObjectState.cs` (68), `Exceptions/CoreException.cs` (80) y `Exceptions/InvalidDataSetException.cs` (83). Ninguno sale de `System.*`; ninguno se transformó de ningún otro modo. | hecho |
| A.2 Rastro de plataforma ausente | Control automatizado sobre **código, proyectos, scripts y documentación propios** de `2.0/`: el rastro de datos de plataforma da **0 ocurrencias** y la capacidad **0 usos vivos** (§9.4). La prueba dura es el build: el núcleo compila en `netstandard2.1`, donde esos tipos no existen. | hecho |

### 2.2 Fase B — la constancia de la superficie retirada y su cobertura

- **B.1** `2.0/docs/api-removals/US-000.BOLT-006.json`: **3 entradas** (una por elemento retirado) con los datos
  que ADR-008 §3.1 exige —**qué** desaparece (tipo, miembro y **firma exacta**), **por qué** (la decisión:
  ADR-001 §3.5 y OQ-005) y **con qué se reemplaza** (ninguno dentro del framework: el export queda del lado del
  consumidor)— más el Bolt que lo ejecuta y la evidencia de su verificación. La entrada de la ruptura pública lleva
  además los campos `type`/`member`, en la **misma forma en que el gate nombra la firma**, para que la cobertura sea
  verificable por máquina.
- **B.2** `ApiRemovalCoverageTests.cs`: **5 casos**. El central corre el gate (`api-compat.ps1 -Mode Current`:
  línea base 1.4 ↔ superficie 2.0), lee su reporte y cruza **toda** diferencia de miembro retirado (`CP0002`)
  contra la **unión** de los fragmentos de `2.0/docs/api-removals/*.json` —el de este Bolt y los de los Bolts que
  ya aportaron superficie—: nombrar el miembro **y** declarar su reemplazo es lo que la constancia debe hacer, y
  una diferencia sin explicación **falla**. Además: la ruptura de este Bolt declara los tres datos de ADR-008 §3.1;
  una entrada sin reemplazo **no** cubre; y los ensamblados sin contraparte se declaran **pendientes** con su razón
  (nunca verdes).

### 2.3 Fase C — verificación sobre el árbol compilado

- **C.1** `RetiredSurfaceTests.cs` inspecciona **el ensamblado construido** con `MetadataLoadContext` (metadatos, sin
  cargar ni ejecutar): la definición genérica `ObjectListHelper<T,T2>` existe, es **pública**, tiene los dos
  parámetros y **las mismas restricciones** (`T : IObject`, `T2 : List<T>`), **no expone** ningún miembro de export
  y conserva `FillDataSet` con su firma (`void (System.Data.DataSet, T2)`).
- **C.2** La búsqueda de usos residuales clasifica cada coincidencia (documentación / comentario / uso vivo) y
  excluye **por diseño** `1.4/**` y la evidencia 1.4 congelada que el gate versiona como su contrato.

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs` | El helper de RowSet del núcleo **sin la capacidad**: `FillDataSet` (que llena un `DataSet` desde un RowSet — es un contrato de datos, no Excel) y su auxiliar `SqlName`, con la clase pública, genérica y con las mismas restricciones. Aportado en su forma retirada según §6/A.1: sin `CreateExcelFile`, sin `xlsLock`, sin el `using` del proveedor de datos de plataforma y con el XML-doc ajustado. 119 líneas (el 1.4 tiene 305). |
| `2.0/src/Cooperator.Framework.Core/IObject.cs` | **Cierre mínimo**: la interfaz del contrato de fila (`IObject : IEditableObject`, con `State` y `OriginalValue()`), que `ObjectListHelper` exige por su restricción `where T : IObject`. Copiado tal cual del 1.4 **salvo la línea autorizada** (§8.2). |
| `2.0/src/Cooperator.Framework.Core/ObjectState.cs` | **Cierre mínimo**: el enum de estado de la fila que `IObject.State` declara. Copiado tal cual. |
| `2.0/src/Cooperator.Framework.Core/Exceptions/CoreException.cs` | **Cierre mínimo**: la excepción base del núcleo (raíz de la jerarquía de excepciones del ensamblado). Copiada tal cual. |
| `2.0/src/Cooperator.Framework.Core/Exceptions/InvalidDataSetException.cs` | **Cierre mínimo**: la excepción tipada que `FillDataSet` lanza cuando el `DataSet` no tiene tablas o su esquema no coincide con el RowSet — sin ella el método aportado no compila ni conserva su contrato de error. Copiada tal cual. |
| `2.0/docs/api-removals/US-000.BOLT-006.json` | **La constancia de la superficie retirada** (fase B.1): 3 entradas con qué/por qué/reemplazo/evidencia/dueño, en la convención de fragmento por Bolt que fijaron los SPECs 002/003/011. Es el insumo de las notas de migración del consumidor (documento del Bolt 028). |
| `2.0/tests/Cooperator.Harness.Tests/RetiredSurfaceTests.cs` | **La verificación** (fase C): inspección de la superficie del ensamblado construido con `MetadataLoadContext` (AC-1) y búsqueda de rastro clasificada (AC-2/AC-3), más el control que **mide** por qué la evidencia 1.4 del gate está excluida y el caso de clasificación de un comentario. 5 casos. |
| `2.0/tests/Cooperator.Harness.Tests/ApiRemovalCoverageTests.cs` | **La cobertura de la constancia** (fase B.2): cruza el reporte del gate contra la unión de los fragmentos (AC-4), exige los tres datos de ADR-008 §3.1 en la ruptura de este Bolt (AC-5) y ejerce los bordes «diferencia sin entrada», «entrada sin reemplazo» y «ensamblado sin contraparte». 5 casos. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md` | `status: blocked` → `in-development` y fila **T06**; al cerrar, la misma transición inversa ya está registrada como corresponde | Las transiciones de estado del Bolt con su fila de `Status timeline` (G11). La fila **T03** quedó donde estaba (fuera de orden): no es de este acto moverla. |
| `2.0/Directory.Packages.props` | + `System.Reflection.MetadataLoadContext` `10.0.13` en el grupo `Test` | El SPEC §6/C.1 y §14 autorizan esta dependencia **de alcance de tests** (inspección de metadatos sin cargar el ensamblado) con su versión **fijada centralmente** —que es exactamente el mecanismo que el archivo cablea—, en la misma familia de servicing que el `System.Text.Json 10.0.13` ya fijado. Ninguna dependencia de producto: los cuatro portables de `2.0/src` mantienen su política (la invariante del arnés la verifica y sigue verde). |
| `2.0/tests/Cooperator.Harness.Tests/Cooperator.Harness.Tests.csproj` | + `PackageReference` a `System.Reflection.MetadataLoadContext` (sin `Version=`, según ADR-001 §3.1) | Consumir la versión centralizada desde el único proyecto que la necesita. |
| `2.0/tests/Cooperator.Harness.Tests/packages.lock.json` | Regenerado por el restore | El lock file versionado incorpora el paquete y su resolución exacta (`10.0.13`); el restore del comando reproducible es en modo **locked** y sin esto el árbol no restauraría. |

---

## 5. Files renamed

Ninguno. El retiro se materializa en el árbol 2.0 y `1.4/**` no se toca (verificado, §9.5).

---

## 6. Files deleted

Ninguno: **no había nada que borrar** —el núcleo de la 2.0 no tenía fuentes propias (`2.0/src/Cooperator.Framework.Core/` sólo tenía su `.csproj`, su lock file y los archivos de configuración del Bolt 011), así que la regla de aporte de §4 se aplicó en su forma completa. Tampoco se borró nada del 1.4: es el árbol congelado.

---

## 7. Implementation decisions

1. **El archivo se aporta en su forma retirada y el cierre se copia, no se reescribe.** El helper se generó
   **recortando las líneas exactas** del archivo congelado (no se retipeó código), y el cierre se copió archivo por
   archivo. Eso hace que la revisión sea un diff legible contra el 1.4 y que el alcance sea auditable: quien lea
   `ObjectListHelper.cs` puede comprobar que sólo cambiaron los tres elementos retirados y el XML-doc ordenado.
2. **Los marcadores de la búsqueda no se escriben literales en el propio buscador.** El caso de rastro **ensambla**
   sus patrones (`"Ole" + "Db"`, `"Microsoft." + "Jet"`, …) para que el archivo que busca no sea, él mismo, una
   ocurrencia de lo que declara ausente. Sin eso, AC-3 («0 ocurrencias en el código propio») se cumpliría por
   accidente o fallaría por el control. Es la misma disciplina que el caso aplica a la evidencia 1.4 del gate: la
   exclusión es parte del criterio **y se mide** (§9.4).
3. **La inspección es del ensamblado, no de la fuente.** `MetadataLoadContext` lee los metadatos del `.dll`
   construido: un miembro retirado no puede «verse bien» por accidente, y el caso no depende de que el ensamblado
   sea cargable en el host del arnés. Es la decisión que el SPEC fijó en §14 y la que hace que la verificación sea
   dura.
4. **La cobertura se cruza contra el reporte real del gate, no contra una lista escrita a mano.** El caso corre el
   gate en modo `Current` (la corrida informativa: línea base 1.4 ↔ superficie 2.0) y exige que **cada** `CP0002`
   esté explicado por **algún** fragmento —el de este Bolt o el de otro—: la constancia no se autoproclama, y una
   ruptura futura sin entrada fallará el caso en lugar de pasar desapercibida.
5. **La brecha de porteo se declara, no se esconde.** El gate reporta 132 tipos que el 1.4 tiene y el 2.0 todavía
   no: es el trabajo declarado de los Bolts que aportan cada archivo, no una ruptura de este Bolt. El caso lo deja
   **medido y contado** (y falla si algún día figura en cero mientras el porteo esté incompleto), y trata igual a
   los ensamblados sin contraparte (`Cooperator.Framework.Utility`, pendiente por ADR-001 §3.1;
   `Cooperator.Framework.Modeling`, ensamblado nuevo sin contraparte en la línea base).
6. **La dependencia de tests se fija en el mecanismo central, no en el `.csproj`.** La versión sólo vive en
   `Directory.Packages.props`; el proyecto la referencia sin `Version=`. Es la política de ADR-001 §3.1/§3.4 y una
   de las invariantes del arnés (`No_project_file_declares_a_package_version`, verde).

---

## 8. Deviations and assumptions

### 8.1 El aporte y su alcance (regla de §4): qué se aportó y qué no

Se aportaron **5 archivos de producto** (el helper en su forma retirada + el cierre de 4) y **3 artefactos de
evidencia** (constancia + 2 archivos de prueba). Nada más: no se tocó ningún archivo de otro Bolt, no se aportó
ningún tipo del set de plantillas, ni del motor, ni del tooling, ni de `LazyLoad` (el namespace que el `using`
muerto de `IObject.cs` mencionaba es entregable del Bolt 005, y esta revisión **no** lo aporta).

### 8.2 La única transformación autorizada, documentada línea por línea (§4/§15 #4)

| Archivo del cierre | Línea | Transformación aplicada | Por qué |
|---|---|---|---|
| `1.4/Cooperator.Framework.Core/IObject.cs` | **L31** | retirada `using Cooperator.Framework.Core.LazyLoad;` | **0 usos** en el archivo (su única referencia externa es `System.ComponentModel.IEditableObject`, del `using` de L30) y el namespace es del Bolt 005: conservarlo prometería una dependencia que este Bolt no posee y, copiado tal cual, impide compilar (`CS0234`). Es exactamente la línea que la revisión 2 autorizó, y el resultado tiene **un solo** `using` (`System.ComponentModel`). |

**Ninguna otra transformación** se aplicó al cierre: los otros tres archivos se copiaron tal cual, y el único
archivo que sí se editó es el que §6/A.1 manda aportar **en su forma retirada** (`ObjectListHelper.cs`).

### 8.3 Lecturas que tomé y declaro (para la decisión del revisor)

1. **AC-3 y la documentación de la constancia.** AC-3 pide «0 ocurrencias (código, referencias, paquetes,
   cadenas)» del rastro de plataforma, y a la vez §6/B.1 exige que la constancia declare «qué desaparece … **y
   firma exacta**» —lo que obliga a nombrar la directiva retirada—. Las resolví a favor del **Then de AC-3 y de
   §8.1** («**0 usos vivos**; las coincidencias … se reportan clasificadas»): el **código, los proyectos y los
   scripts** del árbol propio dan **0 ocurrencias** —la aserción dura— y las **dos únicas** menciones del rastro
   viven en la constancia, que es la que **debe** nombrarlo; el caso exige que esas menciones estén *sólo* ahí. Si
   el revisor prefiere la lectura estricta («0 también en documentación»), el cambio es de una línea: la entrada
   `BOLT006-03` del fragmento describe la directiva sin escribirla y el caso deja de admitir la clase
   documental —queda dicho acá para que la decisión no dependa de una elección no declarada.
2. **`SqlName` sobrevive al retiro.** §6/A.1 manda retirar «**exactamente** los tres elementos» y §2/§A.1 nombran
   a `SqlName` como miembro que sobrevive («`FillDataSet` y `SqlName` no usan `xlsLock` ni tipos de OLE DB»), así
   que **se conservó**. Hecho a declarar: su único llamador era el método retirado, de modo que hoy es un auxiliar
   **privado sin llamadas** (no aparece en la superficie pública, no afecta AC-1 ni el gate, y el build no emite
   ninguna advertencia por él —medido—). Lo señalo porque el SPEC también pide verificar «que no quede código
   muerto por arrastre»: si el revisor quiere que ese auxiliar salga, es una línea más en el mismo archivo, y
   quedaría fuera del alcance literal que §14 fija («se retira **sólo** `CreateExcelFile` + su `xlsLock` + su
   `using`»).
3. **Normalización del copiado.** El **contenido** de los 4 archivos del cierre es verbatim (incluido el
   tabulador interno de `IObject.cs` L36 y las líneas sueltas en blanco); lo único que cambia es la **codificación
   de archivo**: el 1.4 está en CRLF y el árbol 2.0 en UTF-8 sin BOM con LF, así que el aporte entra con la
   convención del árbol destino (§3.15/W23). No es una transformación de código: no se alteró ninguna línea.

### 8.4 El cierre mínimo, medido (y la diferencia con la cifra del SPEC)

| Dato | Valor medido acá | Registrado en el SPEC (§6/A.1) |
|---|---|---|
| Archivos del cierre | **4** | 4 ✔ |
| Líneas del cierre (tras la excepción de §4) | **280** (`IObject` 49 + `ObjectState` 68 + `CoreException` 80 + `InvalidDataSetException` 83) | 252 (medido en el V-Bounce N°1) |
| Dependencias externas | **sólo `System.*`** (0 paquetes, 0 `ProjectReference`) | idem ✔ |

La composición (qué 4 archivos) coincide; la cifra de líneas difiere en 28. No la ajusté ni la reinterpreté: el
cierre es el que compila y el que el núcleo necesita (medido: `EXIT=0` con 0 advertencias), y la diferencia queda
declarada para que el revisor sepa que la cifra del SPEC no es la que el árbol tiene.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 2`, `review.decision: approved`
(`2026-10-16T01:08:28-03:00`) y su §18 registra que la aprobación de la revisión 1 quedó **invalidada** por la
revisión material 2; el Bolt conserva su `AITL-BOLT-READY-Approval` (`2026-10-16T00:00:56-03:00`) y entró a este
V-Bounce en `blocked` con T01–T05 presentes; ADR-001 sigue `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.5). No hay cambio material → G15 no se
activa, y este V-Bounce no abarca dos revisiones (G16).

### 8.6 Comprobación de las condiciones de parada (§15)

- **#1 (cambio material de fuente):** negativa (§8.5).
- **#2 (aparece un consumidor con evidencia):** negativa — la capacidad no tiene un solo llamador en el árbol (las
  únicas referencias del 1.4 son líneas **comentadas** de ejemplo del set de plantillas, que quedan clasificadas
  como informativas y cuyo dueño es el Bolt del set).
- **#3 (habría que editar `1.4/`):** negativa — no se editó ni se escribió ningún artefacto dentro del árbol
  congelado (§9.5).
- **#4 (el cierre exigiría transformar código de otro Bolt):** **la excepción autorizada fue la única**
  transformación aplicada (§8.2); ninguna otra —ni una línea, ni un miembro, ni un atributo— se transformó.
- **#5 (arnés ausente):** negativa — comando reproducible, línea base y gate presentes y corridos (§9.3, §9.6).
- **#6 (un gate de §9 no cierra en `pass`):** negativa — todos los gates aplicables cierran en `pass` o tienen su
  razón de `n/a` (§9.7).
- **#7 (ambigüedad que sólo se resolvería inventando):** no se inventó nada; las dos lecturas interpretativas que
  tomé quedan **declaradas** en §8.3 con su alternativa y su costo, para la decisión del revisor.

---

## 9. Verification evidence

### 9.1 Prerequisitos (§5) verificados antes de generar

```
SPEC-261016-0012 → status: approved · revision: 2 · review.decision: approved (2026-10-16T01:08:28-03:00)
BOLT-006         → status: blocked · filas T01..T05 presentes (T03 fuera de orden, no se mueve) → se tomó con T06
2.0/src/Core     → sólo .csproj + packages.lock.json + los Configuration/*.cs del Bolt 011 (0 fuentes propias del núcleo)
arnés            → 49/49 verde antes de tocar nada · línea base 1.4 versionada · gate api-compat.ps1 operativo
1.4/**           → intacto (git status vacío)
línea base        → 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4
```

### 9.2 El retiro, medido sobre la fuente congelada (sin retipear código)

| Archivo 1.4 | Líneas 1.4 | Líneas aportadas | Qué cambió |
|---|---|---|---|
| `Core/ObjectListHelper.cs` | 305 | **119** | −`CreateExcelFile` (con su doc-comment), −`xlsLock`, −`using System.Data.OleDb;`, XML-doc de la clase ajustado; el resto idéntico |
| `Core/IObject.cs` | 50 | **49** | −L31 `using Cooperator.Framework.Core.LazyLoad;` (la excepción de §4, §8.2) |
| `Core/ObjectState.cs` | 68 | **68** | verbatim |
| `Core/Exceptions/CoreException.cs` | 80 | **80** | verbatim |
| `Core/Exceptions/InvalidDataSetException.cs` | 83 | **83** | verbatim |

Y las tres anclas del retiro, verificadas contra la fuente congelada **antes** de recortar (el generador falla si
no coinciden): `ObjectListHelper.cs` L30 = `using System.Data.OleDb;`; L104 = `private static Object xlsLock …`;
L110 = `public void CreateExcelFile(T2 rowSet, string spreadsheetName, string targetFileName)`.

### 9.3 Build — el núcleo portable y el árbol entero sin advertencias

```
MSBuild 2.0/src/Cooperator.Framework.Core (Rebuild, Release, netstandard2.1)
  Cooperator.Framework.Core -> …\bin\Release\netstandard2.1\Cooperator.Framework.Core.dll
EXIT=0        (0 errores · 0 advertencias, con TreatWarningsAsErrors=true y AnalysisLevel=latest)
```

La compilación en `netstandard2.1` es la prueba dura del retiro: los tipos de datos de plataforma **no existen**
en ese TFM, así que el rastro no puede sobrevivir escondido.

### 9.4 Rastro y capacidad: la búsqueda clasificada (AC-2/AC-3)

Escaneo del árbol propio (`2.0/**`: código, proyectos, scripts y documentación; excluidas las salidas de build y la
evidencia 1.4 del gate), **71 archivos**:

| Familia de marcadores | En código / proyectos / scripts | En documentación | Usos vivos |
|---|---|---|---|
| Rastro de plataforma | **0** | 2 (ambas en `docs/api-removals/US-000.BOLT-006.json`, la constancia) | **0** |
| Capacidad | **0** | la constancia (la nombra a propósito) | **0** |

El caso **mide además por qué** la exclusión del fixture es parte del criterio: el `Cooperator.Framework.Core.dll`
de `2.0/tests/fixtures/api-baseline-1.4/` (el contrato del gate) contiene **3** ocurrencias del rastro de
plataforma y las 2 de la capacidad (**5** en total) — de modo que sin la exclusión el propio contrato del gate haría
fallar AC-3, que es exactamente el hallazgo que la revisión 2 atendió.

### 9.5 `1.4/**` intacto (AC-6)

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.6 La corrida reproducible (AC-6) y la suite

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  restore OK (con restore locked por lock files versionados)
  build sin advertencias (tratadas como error): 13 proyectos
  harness-hermetic        → Passed! - Failed: 0, Passed: 59, Skipped: 0, Total: 59   (eran 49: +10 casos nuevos)
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

**142 casos herméticos verdes** (59 + 83), 13 proyectos con 0 advertencias, `EXIT=0`. Los 10 casos nuevos son los 5
de `RetiredSurfaceTests` (superficie construida, rastro de plataforma, capacidad sin uso vivo, la exclusión medida,
clasificación de un comentario) y los 5 de `ApiRemovalCoverageTests` (cobertura contra el gate, los tres datos de
ADR-008 §3.1, y los tres bordes declarados). Las invariantes del Bolt 001 siguen verdes —incluidas las de
presupuesto de dependencias de los cuatro portables y la de lock files versionados—: el paquete nuevo es de alcance
de **tests** y ninguna las toca.

### 9.7 Gates (§9)

| Gate | Level | Resultado | Evidencia |
|------|-------|-----------|-----------|
| Build / lint / type checks | V-Bounce | **pass** | Árbol 2.0 sin advertencias (13 proyectos) y núcleo en `netstandard2.1` (§9.3, §9.6) |
| Unit / component tests | V-Bounce | **pass** | 142/142 herméticos verdes; incluye la inspección de superficie, la búsqueda clasificada y la cobertura de la constancia (§9.6) |
| Consumer-side contract tests (test doubles) | V-Bounce | **n/a** | El arnés no expone superficie de consumo propia; el contrato de API lo cubre el gate (§9.8) |
| SAST | V-Bounce | **n/a** | El cambio **elimina** código: no introduce superficie que analizar |
| Dependency scanning / licenses | V-Bounce | **pass** | Ninguna dependencia de plataforma entra al árbol 2.0; el único paquete nuevo es de tests, con versión centralizada y lock file (§9.4, §4) |
| OWASP Top 10 — static review | V-Bounce | **n/a** | No se expone ni se altera interfaz alcanzable desde fuera del proceso; el retiro **elimina** una ruta de escritura de archivos |
| Prompt-injection scan | V-Bounce | **n/a** | No hay construcción de prompts ni entrada no confiable |
| Secret-leak scan | V-Bounce | **pass** | El retiro quita del árbol una cadena de conexión a un proveedor de archivos; no se introduce ningún secreto |
| Hallucination lint | V-Bounce | **pass** | Rutas, tipos y miembros referenciados existen (los aportados, declarados uno por uno en §3) |
| IP / license provenance | V-Bounce | **n/a** | No se incorpora código de terceros; el Retiro no suma dependencias de producto |
| PII / DLP | V-Bounce | **pass** | `data_classification: internal`; no se manipulan datos personales ni se copian datos del 1.4 |
| Dependency-confusion | V-Bounce | **pass** | Sin paquetes nuevos de producto; versiones centralizadas y lock files intactos |
| Test-first evidence | V-Bounce | **n/a** | No es un Bolt de corrección de BUG |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **pass** | Dos corridas del comando reproducible con resultado idéntico (§9.6 y la corrida previa del árbol) |
| Bolt-manifest validation | V-Bounce | **pass** | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa (§9.9) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | **pass** | La ruptura aparece **identificada** en el reporte y **cubierta** por la constancia; 0 diferencias de miembro retirado sin explicación (§9.8) |
| Integration tests (real dependencies) | Integration test | **n/a** | `test_integration: not-required`; el plan declara `n/a` con su razón (no hay corrida pendiente) |
| Contract / E2E contra los providers reales | Integration test | **n/a** | Sin provider externo ni entorno en el alcance |
| DAST / OWASP Top 10 — dynamic | Integration test | **n/a** | Sin servicio desplegable |
| Perf-smoke (p95/p99) | Integration test | **n/a** | Sin endpoint ni carga crítica afectada |
| Observability (logs / metrics / traces) | Integration test | **n/a** | Sin servicio instrumentado |

### 9.8 La constancia cruzada contra el gate (AC-4/AC-5) — el reporte real

```
pwsh -File 2.0/build/api-compat.ps1 -Mode Current     (línea base 1.4 ↔ superficie 2.0)
  resultado   : differences — 139 diferencias · código del comparador 1 · salida del gate 2
  CP0002 (miembro retirado de un tipo que existe en el 2.0): 2 → AMBAS cubiertas
    · Cooperator.Framework.Core.ObjectListHelper<T, T2>.CreateExcelFile(T2, string, string)  ← este Bolt
    · Cooperator.Framework.Library.SmtpMail.SendMailViaCDONTS(…)                             ← ya declarada (BOLT011-04)
  CP0001 (tipos que aún no viven en el 2.0): 132 → brecha de porteo, DECLARADA pendiente (trabajo de otros Bolts)
  CP0003 (versión de ensamblado 2.0.0.0 vs 1.4.5.0): 3 → por diseño (ADR-001 §3.3)
  CP0004 (ensamblados sin contraparte): 2 → Cooperator.Framework.Utility (pendiente, ADR-001 §3.1) y
        Cooperator.Framework.Modeling (ensamblado nuevo sin contraparte en el baseline) — DECLARADOS
  Diferencias de miembro retirado SIN explicación: 0
```

El código de salida de la corrida `Current` es **2** —«diferencias de API» del vocabulario 0/1/2/3— porque es la
corrida **informativa** del gate: mientras el porteo sea parcial, la brecha existe y lo que se exige no es que esté
cerrada, sino que **cada miembro retirado esté declarado**. Nunca es `3` (bloqueo): no hubo baseline manipulado ni
ensamblados ausentes de los lados.

### 9.9 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-006-retiro-del-export-excel.json`: se agregó la entrada **`v_bounces[1]`**
  (`number: 2`, `spec_revision: 2`, `execution_outcome: ready_for_review`, `code_generation`, `mem.ref`), validada
  contra `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce (MEM, fila T06 del Bolt, fragmento de constancia, manifiesto) se escribieron en
  **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23); verificado tras escribirlos.

---

## 10. Manual interventions

**Ninguna intervención humana sobre el código.** El aporte se generó recortando la fuente congelada con un
generador determinista (que valida sus anclas antes de tocar nada), el cierre se copió mecánicamente, y los dos
archivos de prueba y la constancia se escribieron en este V-Bounce. **Tampoco hubo operaciones de git** (G34): el
árbol está sin commitear y el commit es del humano. Los únicos actos del agente son los del contrato del V-Bounce:
la toma del Bolt (T06), la generación, las mediciones, el cierre y la pausa.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol de trabajo es la entrega y no se hicieron operaciones de git (G34).
- **SPEC ejecutado:** [SPEC-261016-0012](../spec/SPEC-261016-0012-retiro-del-export-excel.md) — **revisión 2**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T01:08:28-03:00`).
- **Bolt:** [US-000.BOLT-006](../functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md) — `Status timeline`,
  fila **T06** (toma del V-Bounce N°2).
- **V-Bounce N°1 (bloqueado, ahora resuelto):** [`MEM-261016-0059`](MEM-261016-0059-retiro-del-export-excel.md) —
  de donde vienen la medición del cierre mínimo y el hallazgo del `using` muerto que la revisión 2 autorizó.
- **Manifiesto:** [US-000.BOLT-006-retiro-del-export-excel.json](../metrics/bolts/US-000.BOLT-006-retiro-del-export-excel.json)
  — `v_bounces[1]` (`number: 2`, `spec_revision: 2`).
- **La constancia entregada:** `2.0/docs/api-removals/US-000.BOLT-006.json` (3 entradas + los campos de match).
- **Los dos casos que la verifican:** `2.0/tests/Cooperator.Harness.Tests/RetiredSurfaceTests.cs` y
  `2.0/tests/Cooperator.Harness.Tests/ApiRemovalCoverageTests.cs`.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Core/ObjectListHelper.cs`
  (L30 `using System.Data.OleDb;`, L104 `xlsLock`, L110 `CreateExcelFile`, L117-133 conexión del proveedor, L49
  `FillDataSet`, L286 `SqlName`), `IObject.cs` (L31 el `using` muerto), `ObjectState.cs`,
  `Exceptions/{CoreException,InvalidDataSetException}.cs`; contrato del gate:
  `2.0/tests/fixtures/api-baseline-1.4/Cooperator.Framework.Core.dll` (3 ocurrencias del rastro de plataforma —
  el falso positivo que la revisión 2 excluyó por criterio).
- **Gate:** `2.0/build/api-compat.ps1` (modos `Baseline`/`FixturePair`/`Current`) y la corrida informativa de §9.8.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 2 (de US-000.BOLT-006) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0012 revisión **2** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~10 min de reloj (01:30:21 → 01:40:26): reconocimiento, aporte, build, gate, dos casos de prueba, corrida hermética y cierre |
| Archivos creados | 8 (5 de producto —el retiro y su cierre— + 1 constancia + 2 de prueba) |
| Archivos modificados | 4 (Bolt, versiones centralizadas, `.csproj` del arnés y su lock file) |
| Tests creados | **10** (5 de superficie y rastro + 5 de cobertura de la constancia) |
| Tests de la suite | 142 herméticos (59 arnés + 83 caracterización), **todos verdes** |
| Entradas de constancia | 3 (retiro público + auxiliar privado + rastro de plataforma) |
| Diferencias de miembro retirado sin explicación | **0** |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |
| Código generado por IA | 100 % de lo generado en este V-Bounce |
| Desviaciones del SPEC | 0 de alcance; 2 lecturas interpretativas declaradas (§8.3) y 1 cifra del SPEC que el árbol no reproduce (§8.4) |

---

## 13. Pending items and stubs

1. **Las dos lecturas de §8.3** quedan para la decisión del revisor en `AITL-MEM-Approval`: (i) el alcance de AC-3
   respecto de la documentación —el Then enumera código/referencias/paquetes/cadenas y §8.1 pide «0 usos vivos» con
   clasificación; hoy el código/proyectos/scripts dan 0 y la constancia nombra la directiva porque §B.1 la exige—;
   (ii) `SqlName`, que sobrevive al retiro (auxiliar privado hoy sin llamadas) porque §14 acota el retiro a tres
   elementos. Cada una se cambia con una línea si el revisor prefiere la otra lectura.
2. **La cifra de líneas del cierre** (§8.4): el SPEC registra 252 y el árbol tiene **280** en los mismos 4
   archivos. Se declara; no se ajusta.
3. **La brecha de porteo (132 tipos `CP0001`) sigue abierta y declarada**: es el trabajo de los Bolts que aportan
   cada archivo (el núcleo tiene muchos tipos además de los cinco de este Bolt), no una ruptura de este Bolt. El
   caso la deja contada y exige que no figure en cero mientras el porteo esté incompleto.
4. **Insumo para las notas de migración (Bolt 028)**: la lista de rupturas vive en
   `2.0/docs/api-removals/US-000.BOLT-006.json` (3 entradas) y en §3/§9.8 de este MEM. Un consumidor que usara el
   export lo resuelve en su capa, o se queda en el 1.4 congelado — opción soportada y explícita. Si el Bolt 028 fija
   otro contenedor para las notas, mover el fragmento es un acto mecánico.
5. **Fuera de alcance por decisión (no pendiente de este Bolt):** los comentarios de ejemplo de las plantillas del
   1.4 que mencionan el helper (dueño: el Bolt del set de plantillas, ADR-003) y el retiro de
   `[Serializable]`/`SerializationInfo` de la superficie restante (Bolt 007).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto + el árbol entregado (5 archivos de producto,
  la constancia, 2 archivos de prueba y los 3 archivos de configuración que la dependencia de tests exige).
- **Qué se pide decidir:** la aceptación del retiro y —si se considera necesario— las dos lecturas declaradas en
  §8.3.
