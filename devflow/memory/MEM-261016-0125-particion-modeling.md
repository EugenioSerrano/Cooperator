---
id: "MEM-261016-0125"
title: "Partición de `Cooperator.Framework.Modeling` (SPEC rev.3) — V-Bounce N°3 detenido por la invariante de conteo del árbol en el arnés del Bolt 001"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-003" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0004" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 3 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 3 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008", "ADR-002"] # governing ADRs — none of them materialised a change here (V-Bounce stopped before delivering)
manifest: "US-000.BOLT-003-particion-modeling.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §6 and §9.4)
review_ready_at: "2026-10-16T01:25:41-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:31:01-03:00"
  decided_at: "2026-10-16T01:31:01-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible: `BuildInvariantsTests.cs` L195-205 (`Every_project_of_the_tree_has_a_versioned_lock_file`) asserta `Assert.Equal(Portables.Length + 4, projects.Length)` (L198, comentario «4 portables + 2 suites + 2 fixtures») sobre `TreeProjects()` (L212-222, todo `.csproj` del árbol excluyendo bin/obj/artifacts/TestResults). Verificado en el árbol: **exactamente 8 `.csproj`** (glob: 4 portables en `src` + 2 suites + 2 fixtures) y **0 `.cs` en `Modeling`**. Con el 9.º proyecto (§E.1, el de pruebas) el guard cae `Expected: 8 / Actual: 9`. Las **4 cláusulas** «no se toca ningún archivo del Bolt 001» (SPEC rev.3 §5 L386/L402, §14 L1074/L1231) prohíben editar el guard; §15 décima (gate sin `pass` → ADR o revisión del SPEC) es la condición de parada aplicada."
    - "Compliant — las dos instrucciones incompatibles del SPEC rev.3 (§6/E.1 ordena crear el 9.º proyecto de pruebas; §5/§14 prohíben tocar el 001 cuyo guard asserta 8) es un conflicto de **alcance del SPEC**, no del ejecutor. La decisión de no editar el guard ni reformular §E.1 es correcta (§15 décima + principio §15 primera → reversión completa, sin estado intermedio)."
    - "Compliant — fase D no ejecutada (correcto, §15 undécima: el tooling exige `Data.BaseRule` + provider de ADR-005, Bolts 014/016); árbol restaurado y verde (arnés 49/49 + caracterización 83/83, `EXIT=0`, 13 proyectos sin warnings); `1.4/**` intacto (git status vacío)."
    - "Compliant — cierre correcto: T08 (`blocked`→`in-development`, 01:20:02) y T09 (`in-development`→`blocked`, 01:25:41) con reloj real; MEM N°3 único (`MEM-261016-0125` referencia `US-000.BOLT-003`; N°1/N°2 son historia); manifiesto `v_bounces[2]` con `number: 3`, `spec_revision: 3`, `execution_outcome: blocked` y los 8 campos."
    - "Compliant — desbloqueo propuesto correcto y agéntico: revisión 4 que autoriza el ajuste de **una línea** del guard (`Assert.Equal` → `Assert.True(projects.Length >= Portables.Length + 4)`), preservando la intención (el `foreach` que verifica `packages.lock.json` por proyecto queda intacto) y haciéndolo extensible. El aviso estructural es correcto —la colisión **recurre** en cada Bolt que agregue una suite o fixture (004, 013, 017…)—: conviene resolverla una sola vez. Es decisión de secuencia/alcance del SPEC, no del ejecutor."
    - "ADJUDICACIÓN (salvaguardas de la excepción del guard) — SUFICIENTES: comentario de procedencia + constancia en el MEM + el resto del archivo intacto (el `foreach` de lock files) preservan la intención de la invariante; el `>=` conserva el piso mínimo (8) y el **otro** assert (L181, `Assert.Equal(Portables.Length, ...)` sobre `2.0/src`) sigue cubriendo la pérdida de un portable. La excepción es de una línea, documentada y acotada: la apruebo."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0125 — Partición de `Modeling` (SPEC rev.3): V-Bounce N°3 detenido por la invariante de conteo del árbol

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — `type: non-functional`, `risk_class: medium`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md), **revisión 3** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T01:15:21-03:00`, `agent:dotnet-developer-reviewer-agent`) |
| **V-Bounce**    | **3** — el N°1 está en [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md) (`blocked`) y el N°2 en [`MEM-261016-0107`](MEM-261016-0107-particion-modeling.md) (`blocked`) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1/§3.3/§3.6 · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) §3.2/§3.4 · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) §3.1 |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.3) |

---

## 1. Executive summary

Este V-Bounce N°3 tomó el Bolt desde `blocked` (T08, `2026-10-16T01:20:02-03:00`) bajo la **revisión 3** aprobada del
SPEC —la que difiere la fase D como obligación registrada y fija la política de nulabilidad del código portado—,
verificó los prerequisitos de §5 y reprodujo el entregable entero de las fases A–C: los nueve archivos del
subgrafo portable (`Model` + `Reverse` + `Exceptions` + `TypeMapping.cs`), los POCOs sin atributos de diseñador ni
`[Serializable]`, la base de las excepciones del grafo en `System.Exception`, la **fuente única** del mapeo SQL↔CLR
y la directiva `#nullable disable` **sólo** en los ocho archivos portados del 1.4; el build del paquete
`Cooperator.Framework.Modeling` dio **`EXIT=0`, 0 errores y 0 advertencias**, es decir que el mecanismo de §A.3 se
reproduce (los 28 diagnósticos `CS86xx` medidos en el V-Bounce N°2 desaparecen). Al materializar el **proyecto de
pruebas** que ordena §6/E.1 apareció el bloqueo: el árbol 2.0 pasaría de **8** a **9** proyectos y la invariante
del arnés del Bolt 001 `BuildInvariantsTests.Every_project_of_the_tree_has_a_versioned_lock_file` asserta
**exactamente** ocho (`Assert.Equal(Portables.Length + 4, projects.Length)`, `BuildInvariantsTests.cs` L198) —medido:
`Failed: 1, Passed: 48, Total: 49`, con `Expected: 8 / Actual: 9`—, de modo que la suite hermética **no puede quedar
verde** sin editar un archivo del Bolt 001; y el SPEC, en cuatro lugares, prohíbe exactamente eso («**No se toca
ningún archivo del Bolt 001**», L386/L402/L1074/L1231). Como esa decisión no es del ejecutor (§15, décima: un gate
de §9 que no cierra en `pass` se escala y su salida por excepción exige ADR o revisión del SPEC), el V-Bounce se
detuvo, **los aportes se revirtieron** y el árbol queda **sin cambios y verde** (13 proyectos con 0 advertencias,
arnés 49/49, caracterización 83/83, `EXIT=0`). El desbloqueo es de una línea y está listo en §8.4.

---

## 2. Implemented phases

Las fases entregables de la revisión 3 son **A–C más la evidencia portable de E**; la **fase D (tooling) no se
ejecutó** (§D.3, condición de parada §15 undécima). Todo lo generado en A–C se revirtió al cerrar (§6), así que lo
que sigue es el registro de lo **medido** en este V-Bounce, no un estado existente en el árbol.

### 2.1 Fase A — el paquete portable

| Paso de §6/A | Qué se hizo | Estado |
|---|---|---|
| A.1 Proyecto, TFM y política de dependencias | **Nada que hacer**: `2.0/src/Cooperator.Framework.Modeling/Cooperator.Framework.Modeling.csproj` ya es el stub del Bolt 001 (`netstandard2.1`, `System.Text.Json` centralizado, cero `ProjectReference`) y el proyecto **ya está en `2.0/Cooperator2.sln`** (L14, alta del Bolt 001) — no hubo que tocar la solución. Se verificó que **no** se agregó `CooperatorModeler` (fase D diferida). | verificado |
| A.2 Regla de porteo | Los archivos portados se generaron con el **generador determinista** medido en el V-Bounce N°2 (leído del 1.4 congelado, recortado por líneas exactas, namespace reescrito): conservan nombre de archivo y estructura interna, y cambian sólo el namespace, lo que este SPEC ordena retirar y la directiva de §A.3. | medido |
| A.3 Política de nulabilidad | `#nullable disable` insertada como primera línea útil (tras el header de licencia de 28 líneas) **sólo** en los **8** archivos portados del 1.4; **no** en `Reverse/TypeMapping.cs` (código nuevo) ni en las pruebas, y **no** hay `<Nullable>disable</Nullable>` en ningún `.csproj`. | medido (§9.2) |

### 2.2 Fase B — el modelo como POCO

Sobre los tipos del modelo portados se retiró, en el mismo acto de generación: los **atributos de
editor/diseñador** (`[EditorAttribute]`, `[DesignerSerializationVisibility]`, `[Description]`, `[Browsable]`,
`[DefaultValue]`, `[Category]`) junto con los `using` de `System.ComponentModel.Design`, `System.Windows.Forms*`,
`System.Drawing*` y del serializador binario; **`[Serializable]` y todo `SerializationInfo`/`ISerializable`/
`GetObjectData`** de los tipos que migran, incluidos los del grafo y las excepciones; el **`using System.Runtime.
Serialization.Formatters.Binary` inerte** (F-30); el **`using` del provider** (`DomainTree.cs` L35:
`using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;` — verificado línea por línea en la fuente
congelada); y el **acoplamiento del modelo al editor retirado** de §B.1.4 — la sentencia de `DomainTree.cs` L1621
`Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors.DropDownListEditor.stringList = _ParentCandidates;`,
verificada en su línea exacta dentro del getter `GenerateAsChildOf` de `EntityNode`.

### 2.3 Fase B.2 — la partición de `DomainTreeHelper` (reproducida y verificada contra la fuente)

El generador cortó el helper por las fronteras que el SPEC fija en §B.2 y que **este V-Bounce volvió a verificar
sobre el archivo congelado** (2672 líneas):

| Frontera | Línea de `1.4/…/DBReverseHelper/DomainTree.cs` | Verificado |
|---|---|---|
| Arranque de la mitad portable (doc-comment del helper) | `L834` = `/// <summary>` | ✔ |
| Cierre de la mitad portable | `L1056` = `}` (fin de la clase estática) | ✔ |
| Sentencia de acoplamiento al editor retirada | `L1621` (getter `GenerateAsChildOf` de `EntityNode`) | ✔ |
| `using` del provider extraído | `L35` | ✔ |

La mitad portable resultante (11 métodos + el campo estático `_NamespaceCollectionDictionary` + los 4 métodos que
el modelo invoca: `CorrectCLRType`, `CorrectSelectedCollections`, `GetFullGenerateAsForEntity`, `FindEntityInTree`)
compiló **dentro** del paquete `netstandard2.1`, con lo que los **ocho sitios** del modelo (L1226/L1635/L1838/
L2187/L2191/L2206/L2215/L2222) quedan resueltos dentro de `Modeling` y **sin ninguna arista saliente**. La **mitad
reverse** (`UpdateDomainTree`, `GetDomainTree`, `ADDNodeFromDBObject`) **no** se materializó: es la fase D diferida
(§D.3) y su host es el proyecto del tooling, que **no** se creó (§15 undécima).

### 2.4 Fase C — contrato portable y artefacto único del mapeo

`Reverse/DBReverse.cs` (tipos de esquema + `OrderEnum`) y `Reverse/DBReverseHelper.cs` (`IDBReverseProvider`,
`IDBTypeConverter`, `DBReverseEngine`, `DBReverseEngineFactory`, `IncludedEnum`, `DBParentsHelper`) se portaron
sin cambios de comportamiento; y `Reverse/TypeMapping.cs` —**código nuevo**— materializó el artefacto único de
§C.2: la tabla del mapeo como **dato** de solo lectura (orden estable por nombre de tipo nativo), con la regla de
longitud por defecto de `GetLength` expresada como dato (`NativeLengthRule`: `None`/`AsIs`/`Half`) y la
implementación de `IDBTypeConverter` sobre esa tabla (sin `switch` duplicado y sin reflexión). No se agregó
conocimiento de SQL Server más allá de los nombres de tipo nativo: ni `sp_help`, ni quoting, ni queries de schema —
eso es del tooling y del motor.

### 2.5 Fases D y E

- **Fase D:** **no ejecutada** — condición de parada §15 undécima respetada (el tooling exige `Data.BaseRule` y la
  decisión de provider/versión de ADR-005 §3.1 del Bolt 014). No se creó `2.0/src/CooperatorModeler/`.
- **Fase E:** la constancia (§E.2 con sus 9 rupturas), `MIGRATION-PARTITION.md` y
  `2.0/docs/api-removals/US-000.BOLT-003.json` **no se escribieron**: su contenido pertenece al paquete que se
  entrega, y el V-Bounce se detuvo antes (§8.1). El reporte de dependencias medido en el V-Bounce N°2
  (`netstandard 2.1.0.0` y nada más) sigue siendo válido y se cita como insumo en §8.4.

---

## 3. Files created

Todos estos archivos **se generaron en este V-Bounce y se revirtieron al cerrarlo** (§6). Se registran con su
propósito porque son el entregable medido y son lo que el próximo V-Bounce debe reproducir.

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Modeling/Model/DomainTree.cs` | El modelo del grafo como POCO: `BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery` y los enums `CollectionTypeEnum`/`DatabaseQueryReturnType`, portados desde `1.4/…/DomainTree.cs` (2672 líneas) sin el helper, sin atributos de diseñador, sin serialización binaria y sin la sentencia L1621. |
| `2.0/src/Cooperator.Framework.Modeling/Model/DomainTreeHelper.cs` | La **mitad de árbol** del helper (`1.4` L834–1056): los 11 métodos puros, el caché de namespaces y los 4 métodos que el modelo invoca — con lo que las ocho llamadas del modelo se resuelven dentro del paquete. |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/DBReverse.cs` | Los tipos del esquema de base de datos y `OrderEnum` (namespace `Cooperator.Framework.Modeling.Reverse`). |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/DBReverseHelper.cs` | El contrato portable del reverse (`IDBReverseProvider`, `IDBTypeConverter`), el `DBReverseEngine` con su factory y los enums/helpers de la capa. |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/TypeMapping.cs` | **La** fuente única del mapeo SQL↔CLR como dato portable: la tabla, la regla de longitud por defecto y la implementación de `IDBTypeConverter`. **Código nuevo**: conserva la nulabilidad activa (§A.3). |
| `2.0/src/Cooperator.Framework.Modeling/Exceptions/UtilityException.cs` | Base abstracta de las excepciones del grafo, ahora sobre **`System.Exception`** (ADR-001 §3.1: `Modeling` no tiene aristas salientes) — `CompilerException` **no** se porta (slot del motor, Bolt 017). |
| `2.0/src/Cooperator.Framework.Modeling/Exceptions/UnknownDBTypeExeception.cs` | Excepción del contrato del mapeo: tipo nativo desconocido (deja de ser una coerción silenciosa). |
| `2.0/src/Cooperator.Framework.Modeling/Exceptions/NodeAlreadyExistException.cs` | Excepción del modelo: alta de nodo duplicado. |
| `2.0/src/Cooperator.Framework.Modeling/Exceptions/CannotAddOrRemoveItemException.cs` | Excepción del modelo: operación inválida sobre la colección de nodos. |
| `2.0/tests/Cooperator.Framework.Modeling.Tests/Cooperator.Framework.Modeling.Tests.csproj` | Proyecto de pruebas `net10.0` que ordena §E.1 (con sus `PackageReference` de xUnit v3 y un único `ProjectReference` a `Modeling`). **Es el archivo que materializa el bloqueo de §8.1**: su sola existencia lleva el árbol a 9 proyectos. Se creó **sólo para medir** la colisión; se retiró (dos veces) y no queda en el árbol. |

**No** se crearon los cinco archivos de prueba de §E.1 (`DependencyReportTests.cs`, `TypeMappingTests.cs`,
`ModelPocoTests.cs`, `DomainTreeHelperPartitionTests.cs`, `ModelCharacterizationTests.cs`): la existencia del
**proyecto** ya es, por sí sola, la condición que impide cerrar el V-Bounce, y escribir sus 40+ casos sobre un
proyecto que no puede quedar en el árbol habría sido trabajo no autorizado a un estado intermedio (§15 primera).

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-003-particion-modeling.md` | `status: blocked` → `in-development` y fila **T08** (toma del V-Bounce por G11); y `in-development` → `blocked` con la fila **T09** al cerrar | Las dos transiciones de estado del Bolt con su fila de `Status timeline` — es el registro mecánico del V-Bounce; ninguna otra línea del Bolt se tocó. |

**No** se modificó `2.0/Cooperator2.sln`: el proyecto `Modeling` **ya estaba** en la solución (alta del Bolt 001,
L14) y el proyecto del tooling **no** debe entrar (fase D diferida, §15 undécima).

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Todo lo de §3 se **revirtió** al cerrar el V-Bounce (rollback = no-cambio):

| Path | Por qué se retira |
|---|---|
| Los nueve `.cs` de §3 (`Model/`, `Reverse/`, `Exceptions/`) | Son el aporte de un V-Bounce que **no cierra** (`blocked`): dejarlos sería un estado intermedio no autorizado (§15 primera: «el ejecutor no decide por sí mismo ni deja el árbol en un estado intermedio»). |
| `2.0/tests/Cooperator.Framework.Modeling.Tests/` (proyecto y carpeta) | El proyecto ordenado por §E.1 es la **causa** del bloqueo; su presencia es exactamente lo que rompe la invariante del arnés. |
| `2.0/src/Cooperator.Framework.Modeling/{bin,obj}` | Salidas de compilación de los fuentes revertidos: se eliminan para que no quede en disco un ensamblado construido a partir de un aporte no aceptado. El build del control (§9.4) volvió a generarlas desde el stub del Bolt 001. |

Tras la reversión, `2.0/src/Cooperator.Framework.Modeling/` contiene **sólo** el stub del Bolt 001
(`Cooperator.Framework.Modeling.csproj` + `packages.lock.json`) y el árbol 2.0 vuelve a tener **8** `.csproj`.

---

## 7. Implementation decisions

1. **Portear con el generador determinista, no a mano.** El porteo del 1.4 a la 2.0 no se hizo editando archivos a
   mano (ilegible e irrepetible) sino re-ejecutando el generador medido en el V-Bounce N°2, que lee el árbol 1.4
   **congelado** y aplica recortes por línea exacta. Es lo que hace que la revisión sea un diff legible contra la
   fuente (§A.2) y lo que permite revertir limpiamente: la decisión se tomó pensando en que el cierre podía volver
   a ser `blocked`, que es lo que ocurrió.
2. **La directiva de nulabilidad es por archivo y sólo en los portados.** Se insertó en los **8** archivos
   portados (los 4 de §A.2 y los 4 de `Exceptions/`) y **no** en `Reverse/TypeMapping.cs` —que es código nuevo y
   compiló limpio con la nulabilidad **activa** del árbol—, ni a nivel de proyecto. La alternativa (apagar la
   nulabilidad en el `.csproj`) está **prohibida** por §A.3 y habría apagado el análisis del paquete entero,
   incluido el contrato del Bolt 004.
3. **Medir antes de decidir, y acotar el daño.** Ante la sospecha de la colisión, no se decidió por lectura: se
   creó el `.csproj` ordenado y se corrió el arnés. El resultado acota el problema a **una** prueba de 49 y a **una**
   línea de código (§9.3), lo que hace que la salida propuesta en §8.4 sea trivial de adjudicar.
4. **No inventar la forma del entregable.** Se consideraron y **descartaron** dos salidas que habrían dejado el
   árbol verde sin tocar el Bolt 001 —(i) crear los `.cs` de §E.1 **sin** su `.csproj` y compilarlos por
   `<Compile Include>` desde el arnés, y (ii) plegar los cinco archivos de prueba dentro del proyecto del arnés—:
   ambas contradicen el listado literal de §E.1 (`…/Cooperator.Framework.Modeling.Tests.csproj`) y de AC-8 («el
   paquete portable **y su proyecto de pruebas**»), y elegir entre ellas es una decisión de alcance del SPEC, no
   del ejecutor (§15 primera: la corrección de una tensión del SPEC es una **revisión material**, no una decisión
   del ejecutor).
5. **Revertir y dejar el árbol verde.** El cierre `blocked` se acompaña de un rollback completo **más** la corrida
   de control que demuestra que el árbol quedó como estaba (§9.4): un `blocked` no puede dejar el repositorio en un
   estado que nadie aprobó.

---

## 8. Deviations and assumptions

### 8.1 El bloqueo (blocker exacto)

**El SPEC revisión 3 contiene dos instrucciones mutuamente incompatibles, y resolverlas no es del ejecutor.**

1. **§6/E.1 (L739) ordena** crear `2.0/tests/Cooperator.Framework.Modeling.Tests/Cooperator.Framework.Modeling.
   Tests.csproj` —«proyecto de pruebas `net10.0` (nivel del árbol 2.0)»—, y §9 lo vuelve un umbral de gate
   («Build / lint / type checks: sin errores ni warnings en el paquete portable **y en su proyecto de pruebas**»),
   igual que AC-8 («el build del árbol afectado (**el paquete `Modeling` y su proyecto de pruebas**)»). Ese proyecto
   sería el **noveno** `.csproj` del árbol 2.0.
2. **La invariante del arnés del Bolt 001 aserta exactamente ocho.** En
   `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs`:

   ```csharp
   [Fact]
   public void Every_project_of_the_tree_has_a_versioned_lock_file()
   {
       var projects = TreeProjects();
       Assert.Equal(Portables.Length + 4, projects.Length); // 4 portables + 2 suites + 2 fixtures   ← L198
   ```

   con `TreeProjects()` (L217) enumerando **todo** `.csproj` bajo `Repo.InTree()` —que es el árbol `2.0`
   (`Repo.InTree("src", …)`, `Repo.InTree("build", …)`, `Repo.InTree()` como directorio de trabajo del build)—
   excluyendo sólo `bin`, `obj`, `artifacts` y `TestResults`. Hoy el árbol tiene **8** (4 portables de `2.0/src` +
   2 suites + 2 fixtures) y la constante del arnés da 8: **la invariante pasa hoy**.
3. **§5/§14/§A.3 prohíben la única corrección posible *dentro del árbol*.** El SPEC lo dice cuatro veces:
   «**No se toca ningún archivo del Bolt 001**» — §5 (L386, alineación del presupuesto de dependencias), §5
   (L402, política de build heredada), §14 (L1074, tabla de la excepción del `System.Text.Json`) y §13/§14
   (L1231). La corrección sería **una línea** de `BuildInvariantsTests.cs` (un archivo del Bolt 001).

**La medición (no una inferencia):** con el `.csproj` ordenado presente —y **ninguna** otra cosa cambiada— el
arnés pasa de 49/49 a:

```
Failed Cooperator.Harness.Tests.BuildInvariantsTests.Every_project_of_the_tree_has_a_versioned_lock_file
Error Message: Assert.Equal() Failure: Values differ
Expected: 8
Actual:   9
Failed!  - Failed: 1, Passed: 48, Skipped: 0, Total: 49
```

Es **una** prueba de 49 la que cae, y la otra aserción de conteo —`Assert.Equal(Portables.Length, projects.Length)`
(L181), que enumera **sólo** `2.0/src` (4 portables)— **sigue pasando**: la colisión está acotada a la invariante
que cuenta el árbol entero. Con eso, el umbral de §9 «Unit / component tests: **100 % verdes**, incluida la suite de
caracterización antes/después» **no puede cerrar en `pass`** mientras §E.1 se materialice como está ordenado.

### 8.2 Condición de parada aplicada (§15) y comprobación de las demás

- **Aplicada — §15, décima (L1118):** «Un gate de §9 no cierra en `pass`: si la única salida fuera una excepción de
  gate, exige un ADR aprobado por `AITL-ADR-Approval` (G21) — **nunca una decisión del ejecutor**». La única salida
  *dentro del árbol* es editar un archivo del Bolt 001 (prohibido cuatro veces por el propio SPEC); las salidas
  *fuera* de esa prohibición (reformular §E.1) son un cambio de alcance del SPEC. Ambas exceden al ejecutor.
- **Principio aplicado — §15, primera (L1092):** una tensión que el SPEC no resuelve «no se ejecuta; el SPEC se
  revisa (revisión material) y se re-aprueba. El ejecutor **no** decide por sí mismo ni deja el árbol en un estado
  intermedio» → por eso además del `blocked` hubo **reversión completa** (§6).
- **Comprobadas y negativas** (ninguna otra condición de §15 se activó): (2) ningún tipo del modelo depende de algo
  fuera del paquete más allá de los acoplamientos declarados —el paquete `netstandard2.1` compila con la mitad de
  árbol del helper dentro y sin el provider—; (3) la suite de caracterización existe y corre (83/83, §9.4); (4) no
  hay dos consumidores pidiendo tablas incompatibles: hay **una** declaración del mapeo; (5) no se reescribió el
  reverse (no se ejecutó la fase D); (6) el tooling no necesitó nada de este Bolt; (7) no apareció código del motor
  (`ScriptingHost`/`ScriptsEnvironment`/`CompilerException` quedaron fuera); (8) no apareció una ruptura de
  superficie nueva fuera de §6/E.2; (9) ninguna fuente gobernada cambió materialmente (§8.3); (11) el proyecto del
  tooling **no** se creó; (12) ningún archivo **nuevo** quedó con la directiva de nulabilidad.

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 3`, `review.decision: approved`
(`2026-10-16T01:15:21-03:00`); el Bolt conserva su `AITL-BOLT-READY-Approval` (`2026-10-15T23:47:42-03:00`) y
entró a este V-Bounce en `blocked` con T01–T07 presentes; los ADR aplicados (001, 002, 008) siguen `accepted`; la
línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está **intacto** (verificado con `git status`
sobre `1.4`: sin cambios). No hay cambio material de fuente → G15 no se activa.

### 8.4 El desbloqueo — dos salidas, ninguna del ejecutor (con la medición lista)

1. **Recomendada — revisión 4 que autorice la actualización del guard del arnés (una línea).** La invariante se
   llama `Every_project_of_the_tree_has_a_versioned_lock_file`: su **intención** es que todo proyecto tenga
   `packages.lock.json` versionado, y el conteo exacto es un **guardia contra una enumeración débil**. Basta
   conservar la intención y hacer extensible el guard:

   ```csharp
   // antes (Bolt 001, BuildInvariantsTests.cs L198)
   Assert.Equal(Portables.Length + 4, projects.Length); // 4 portables + 2 suites + 2 fixtures
   // después
   Assert.True(projects.Length >= Portables.Length + 4,
       $"El árbol tiene {projects.Length} proyectos: menos de los {Portables.Length + 4} esperados.");   // y sigue el foreach de lock files
   ```

   El `foreach` que verifica el `packages.lock.json` de **cada** proyecto —el verdadero propósito de la
   invariante— queda intacto, y el nuevo proyecto de pruebas **también** deberá tener su lock file (lo tendrá: el
   árbol lo exige en todos). **Aviso estructural:** esta colisión se repetirá en **cada Bolt que agregue una suite o
   un fixture** (Bolts 004, 013, 017…), así que la decisión conviene tomarla una sola vez.
2. **Alternativa — revisión 4 que redefina §E.1** para que las pruebas del paquete no agreguen un proyecto nuevo
   (p. ej. plegando §E.1 en el arnés existente, o montando sus fuentes por `<Compile Include>`), aceptando entonces
   que no habrá «proyecto de pruebas» propio y ajustando §9 y AC-8 en consecuencia.

Lo que **no** se hizo, para que quede explícito: **no** se editó `BuildInvariantsTests.cs` (§5), **no** se crearon
los `.cs` de §E.1 sin su proyecto (contradice el listado de §E.1), **no** se plegaron las pruebas en otro proyecto
(contradice AC-8) y **no** se agregó `CooperatorModeler` (§15 undécima). Con la salida 1 autorizada, el entregable
de la revisión 3 se reproduce **completo y en minutos**: el porteo ya está medido (§9.2) y sólo falta escribir los
cinco archivos de prueba de §E.1, la constancia §E.2 (`MIGRATION-PARTITION.md` +
`2.0/docs/api-removals/US-000.BOLT-003.json`) y el registro de la obligación de §D.3 — más su referencia cruzada a
`SPEC-261016-0020` (Bolt 014) y `SPEC-261016-0027` (Bolt 016) y los tres recordatorios obligatorios.

### 8.5 Supuestos

- Que §E.1 exige un **proyecto** real en la ruta que nombra (es lo que dice el listado de §E.1 y lo que asumen el
  umbral de §9 y AC-8 —«su proyecto de pruebas»—) y que, por lo tanto, el árbol pasa a 9 proyectos. No se asumió
  nada sobre si el SPEC *quería* ese efecto: se midió y se reporta.
- Que el resto del entregable de la revisión 3, tal como se generó, es el correcto: la evidencia está en §9.2 y
  §9.4 (build `EXIT=0` con las directivas y el acoplamiento al editor retirado, verificado además contra la fuente
  congelada en las líneas exactas L35/L834/L1056/L1621).
- Que la reversión es el estado correcto de cierre: el árbol queda **idéntico** al de la última aceptación
  (Bolt 011 `MEM-261016-0117`, aprobado) más el stub del Bolt 001 — es el estado con el que el próximo V-Bounce
  arranca.

---

## 9. Verification evidence

### 9.1 Prerequisitos de §5 (verificados antes de generar nada)

```
SPEC-261016-0004  → status: approved · revision: 3 · review.decision: approved (2026-10-16T01:15:21-03:00)
BOLT-003          → status: blocked · filas T01..T07 presentes  → se tomó con T08 (01:20:02)
2.0/src/Modeling  → sólo el stub del Bolt 001 (csproj + packages.lock.json) · 0 archivos .cs
Modeling en la sln → sí (L14) · CooperatorModeler en la sln → no (fase D diferida, correcto)
1.4/**            → intacto (git status sobre 1.4 sin cambios)
línea base         → 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4
```

### 9.2 El porteo compila y el mecanismo de §A.3 se reproduce (medido; luego revertido)

Con los nueve archivos de §3 en el árbol y la directiva insertada **sólo** en los 8 portados:

```
Cooperator.Framework.Modeling -> …\bin\Release\netstandard2.1\Cooperator.Framework.Modeling.dll
EXIT=0        (0 errores · 0 advertencias, con TreatWarningsAsErrors=true)
```

Verificación de la **ubicación** de la directiva (§A.3/§8.1) por lectura de los archivos entregados — es lo que
separa «funciona» de «funciona por el motivo autorizado»:

| Archivo | `#nullable disable` | Origen |
|---|---|---|
| `Model/DomainTree.cs` | **sí** | portado del 1.4 |
| `Model/DomainTreeHelper.cs` | **sí** | portado del 1.4 |
| `Reverse/DBReverse.cs` | **sí** | portado del 1.4 |
| `Reverse/DBReverseHelper.cs` | **sí** | portado del 1.4 |
| `Exceptions/UtilityException.cs` | **sí** | portado del 1.4 |
| `Exceptions/UnknownDBTypeExeception.cs` | **sí** | portado del 1.4 |
| `Exceptions/NodeAlreadyExistException.cs` | **sí** | portado del 1.4 |
| `Exceptions/CannotAddOrRemoveItemException.cs` | **sí** | portado del 1.4 |
| `Reverse/TypeMapping.cs` | **no** | **código nuevo** → nulabilidad activa (correcto) |

Y ningún `.csproj` del árbol declara `<Nullable>disable</Nullable>` (grep sobre `2.0/**/*.csproj`: 0 coincidencias).

### 9.3 La colisión, medida (la evidencia central de este MEM)

```
# con 2.0/tests/Cooperator.Framework.Modeling.Tests/Cooperator.Framework.Modeling.Tests.csproj presente
dotnet test 2.0/tests/Cooperator.Harness.Tests/Cooperator.Harness.Tests.csproj --no-build -c Release

  Failed Cooperator.Harness.Tests.BuildInvariantsTests.Every_project_of_the_tree_has_a_versioned_lock_file [8 ms]
   Assert.Equal() Failure: Values differ
Expected: 8
Actual:   9
Failed!  - Failed: 1, Passed: 48, Skipped: 0, Total: 49, Duration: 6 s
```

El guard se midió además **aislado** (`--filter FullyQualifiedName~Every_project_of_the_tree_has_a_versioned_lock_file`)
con el mismo resultado (`Expected: 8 / Actual: 9`, exit 1). El proyecto temporal se creó y se retiró **dos veces**
(sólo para medir); el conteo del árbol quedó verificado en **8** después de cada retiro.

### 9.4 El árbol como se deja (corrida de control, después de la reversión)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  ==> Construyendo la solucion 2.0 (Release) con MSBuild fijado
      build sin advertencias (tratadas como error): 13 proyectos
  ==> Corriendo harness-hermetic (filtro: Category!=Integration)
      Passed!  - Failed: 0, Passed: 49, Skipped: 0, Total: 49
  ==> Corriendo characterization-hermetic (filtro: Category!=Integration)
      Passed!  - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

**132 casos herméticos verdes** (49 del arnés + 83 de caracterización), 13 proyectos con 0 advertencias, `EXIT=0`,
`1.4/**` intacto. Este es el estado del árbol que el MEM entrega: el mismo que el V-Bounce encontró.

### 9.5 Gates

| Gate | Level | Resultado | Evidencia |
|------|-------|-----------|-----------|
| Build / lint / type checks | V-Bounce | **n/a** (V-Bounce `blocked`, sin entrega) | El aporte medido daba `EXIT=0` con 0 advertencias (§9.2) y se revirtió; el árbol queda con el build verde de la corrida de control (§9.4). |
| Unit / component tests | V-Bounce | **n/a — es el gate que no puede cerrar** | §9.3: con el proyecto de pruebas de §E.1, el arnés cae 1/49 (`Expected: 8 / Actual: 9`) → un umbral de «100 % verdes» no puede declararse `pass`. Es la condición de parada aplicada (§8.2). |
| Consumer-side contract tests (test doubles) | V-Bounce | **n/a** (sin entrega) | Los casos de §8.1 no se escribieron (dependen del proyecto de pruebas bloqueado). |
| SAST | V-Bounce | **n/a** (sin entrega) | No quedó código en el árbol: nada que analizar en este cierre. |

Ningún gate se declara `pass` en este V-Bounce: un `blocked` no lleva gates cerrados, y declarar `pass` sobre un
árbol sin el entregable sería un verde falso.

### 9.6 Manifest

`devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json` — se agregó la entrada **`v_bounces[2]`**:
`number: 3`, `spec_revision: 3`, `git_commit: 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`,
`execution_outcome: "blocked"`, `code_generation` (opencode/deepseek/deepseek-flash), `mem.ref` a este documento.
Validada contra `devflow/metrics/manifest-v5-bolt.schema.json` (§9.7).

### 9.7 Encoding de los documentos gobernados (§3.15 / W23)

Los documentos entregados por este V-Bounce (este MEM, la fila T09 del Bolt y la entrada del manifiesto) se
escriben en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD`; se verificó tras escribirlos.

---

## 10. Manual interventions

**Ninguna intervención humana.** Ningún archivo fue parcheado a mano por una persona durante este V-Bounce: el
porteo se generó con el generador determinista, las mediciones se corrieron con las herramientas del árbol y la
reversión fue mecánica. **Tampoco hubo operaciones de git** (G34): el árbol está sin commitear y el commit es del
humano. Los únicos actos del agente son los del contrato del V-Bounce: la toma del Bolt (T08), la generación y
medición del entregable, las dos mediciones de la colisión, la reversión y el cierre (T09 + MEM + manifiesto).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""` y el árbol quedó **sin cambios** respecto del estado con el que arrancó el
  V-Bounce (§6, §9.4). No hay operaciones de git en el V-Bounce (G34).
- **SPEC ejecutado:** [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md) — **revisión 3**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T01:15:21-03:00`).
- **Bolt:** [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — `Status timeline` filas
  T08 (toma) y T09 (cierre `blocked`).
- **Manifiesto:** [US-000.BOLT-003-particion-modeling.json](../metrics/bolts/US-000.BOLT-003-particion-modeling.json)
  — `v_bounces[2]` (`number: 3`, `spec_revision: 3`, `blocked`).
- **V-Bounces anteriores de este Bolt:** [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md) (N°1) y
  [`MEM-261016-0107`](MEM-261016-0107-particion-modeling.md) (N°2 — de donde vienen el generador determinista, las
  fronteras del helper y la medición de los 28 diagnósticos de nulabilidad).
- **Evidencia de código (lectura, en el árbol congelado):**
  `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` (2672 líneas; L35 `using …Providers.MSSQL`, L834
  inicio de la mitad portable, L1056 cierre, L1621 sentencia de acoplamiento al editor),
  `…/DBReverseHelper/DBReverse.cs` (660), `…/DBReverseHelper/DBReverseHelper.cs` (330),
  `…/Providers/MSSQLCommon.cs` (260 — de donde se extrajo la tabla del mapeo), `…/Exceptions/*.cs`
  (`UtilityException` 122, y los tres derivados 80/80/81).
- **Invariante que bloquea:** `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs`
  (`Every_project_of_the_tree_has_a_versioned_lock_file`, L195; `Assert.Equal(Portables.Length + 4, projects.Length)`,
  L198; `TreeProjects()`, L217).
- **Reporte de dependencias del paquete (medido en el V-Bounce N°2, sigue vigente):**
  `Cooperator.Framework.Modeling.dll` referencia **sólo** `netstandard 2.1.0.0`; 0 `ProjectReference`; 1
  `PackageReference` (`System.Text.Json`, versión centralizada).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 3 (de US-000.BOLT-003) — `blocked` |
| SPEC ejecutado | SPEC-261016-0004 revisión **3** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~5 min de reloj (01:20:02 → 01:25:41): generación del porteo, dos mediciones de la colisión, reversión, corrida de control y cierre |
| Archivos entregados al árbol | **0** (reversión completa; ver §6) |
| Tests creados | **0** — los cinco archivos de §E.1 no se escribieron (§3) |
| Tests corridos como control | 132 (49 del arnés + 83 de caracterización), **todos verdes** |
| Condición de parada | §15 décima (gate de §9 que no cierra en `pass`) + principio de §15 primera (reversión, no estado intermedio) |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |
| Código generado por IA | 100 % de lo generado y revertido |
| Desviaciones del SPEC | 0 en los archivos de A–C (todo se ciñó a §6); el desvío es de **alcance del SPEC**, no del ejecutor (§8.1) |

---

## 13. Pending items and stubs

1. **El bloqueo (§8.1)** — es el único punto que impide cerrar: la invariante de conteo del árbol del arnés del
   Bolt 001 (8 proyectos exactos) frente al proyecto de pruebas que ordena §E.1 (el 9.º). Necesita una **revisión 4
   del SPEC** con una de las dos salidas de §8.4 (recomendada: autorizar el ajuste de una línea del guard).
2. **Todo lo demás de la revisión 3 está medido y listo para el próximo V-Bounce:** el porteo de las fases A–C
   (9 archivos + la directiva de nulabilidad en los 8 portados + el acoplamiento al editor de L1621 retirado) se
   reproduce con el generador determinista y compila `EXIT=0` (§9.2). Falta, además de lo bloqueado: los **cinco
   archivos de prueba** de §E.1, la **constancia de §E.2** (mapa tipo→destino, lo que dejó de existir y las **9
   rupturas** para las notas de migración del Bolt 028) en `MIGRATION-PARTITION.md` y
   `2.0/docs/api-removals/US-000.BOLT-003.json`, y el **registro de la obligación de §D.3** (fase D diferida) con
   su **referencia cruzada** a `SPEC-261016-0020` (Bolt 014) y `SPEC-261016-0027` (Bolt 016) y sus **tres
   recordatorios obligatorios**.
3. **Aviso para el resto del Run:** la invariante `Every_project_of_the_tree_has_a_versioned_lock_file` del arnés
   del Bolt 001 se romperá en **cualquier** Bolt que agregue una suite o un fixture (004, 013, 017…). Conviene
   resolverla en una sola decisión (§8.4.1) y no Bolt por Bolt.
4. **Obligación de la fase D (§D.3):** sigue vigente y **no** se vio afectada por este cierre — su dueño son los
   Bolts 014/016 (mientras la capa `Data` no exista, la obligación no puede cumplirse y su incumplimiento no es un
   defecto de este Bolt).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[2]` del manifiesto + el árbol **sin cambios** (§9.4).
- **Qué se pide decidir:** el bloqueo de §8.1 y, con él, la salida de §8.4 que habilita la **revisión 4** del SPEC
  (ninguna de las dos es del ejecutor).
