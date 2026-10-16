# MIGRATION-PARTITION — Partición de `Cooperator.Framework.Utility` en la 2.0

**Bolt:** `US-000.BOLT-003` · **SPEC:** `SPEC-261016-0004` **revisión 4** (aprobada 2026-10-16T01:35:00-03:00) ·
**Paquete entregado:** `Cooperator.Framework.Modeling` (`netstandard2.1`).

Este documento es la **constancia de la partición**, versionada junto al código. Es el insumo de las **notas de
migración del consumidor**, cuyo documento es del **Bolt 028** (ADR-008 §3.1): acá está el mapa tipo → destino, la
constancia de lo que dejó de existir en la 2.0, la lista de rupturas que este Bolt introduce y las obligaciones que
quedan registradas. Las decisiones no se reinterpretan acá: se citan (ADR-001 §3.1/§3.3/§3.5, ADR-002 §3.1,
ADR-003 §3.5, ADR-008 §3.1/§3.2, ADR-009 §3.2).

---

## 1. El criterio de la partición

`Cooperator.Framework.Utility` del 1.4 no entra completo en la 2.0: se parte con una regla **objetiva y
verificable** (SPEC §6/B.2), aplicada método por método sobre la fuente congelada:

> Si el método **instancia la factory concreta, recibe una cadena de conexión o toma tipos del esquema de base de
> datos** (`DataBase`, `DBTable`) → **tooling**. Si **sólo recorre el árbol** (`BaseTreeNode`, `EntityNode`,
> `PropertyNode`, `RelatedField`, cadenas) → **`Modeling`**.

`DomainTreeHelper` (14 métodos en `DomainTree.cs` L76–1056 de la fuente de 2672 líneas) queda partido:

| Destino | Métodos |
|---|---|
| **`Modeling`** — mitad de árbol (11) | `GetDescriptionFieldNameForEntity` (L840), `GetGenerateAsForEntity` (L855), **`GetFullGenerateAsForEntity`** (L866), `GetGenerateAsForProperty` (L876), `FindPropertyInTree` (L898), `GetNamespaceCollection` (L922), `ResetNamespaceCollection` (L942), **`FindEntityInTree`** (L953), **`CorrectCLRType`** (L969), **`CorrectSelectedCollections`** (L1011), `CorrectGenerationNamesForRelatedFields` (L1036), con el campo `private static Dictionary<string,string> _NamespaceCollectionDictionary` (L917) |
| **Tooling — DIFERIDO** (3) | `UpdateDomainTree(string, …)` (L85), `GetDomainTree(string, …)` (L198) y `ADDNodeFromDBObject(DataBase, EntityNode, DBTable)` (L312, privado) |

En negrita, los cuatro métodos que **el modelo invoca** (ocho sitios: `EntityNode.GenerateAs` L1226 y
`GenerateAsChildOf` L1635; `PropertyNode` setter de tipo L1838 y `GenerateAsType` L2187/L2191/L2206/L2215/L2222):
por eso la mitad de árbol entra al paquete portable — si quedaba afuera, el paquete no compilaba (`netstandard2.1`
no puede referenciar el tooling `net10.0-windows`).

**Direcciones (verificadas en la fuente):** la mitad reverse **llama** a la mitad portable —`CorrectGenerationNamesForRelatedFields`
(L184), `FindEntityInTree` (L222/L297), `CorrectSelectedCollections` (L292)—, y eso está permitido (tooling →
`Modeling`, ADR-001 §3.1). Lo que **no** existe es la dirección contraria: ningún tipo del modelo llama a la mitad
reverse.

> **Aviso para quien ejecute la fase D (obligación registrada, §4):** el tooling tendrá una clase con **el mismo
> nombre** (`DomainTreeHelper`) en su propio namespace, así que esas **cuatro llamadas cruzadas debe calificarlas**
> como `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X`.

---

## 2. Mapa tipo → destino

### 2.1 Entregado en este Bolt (`2.0/src/Cooperator.Framework.Modeling/`)

| Tipo (1.4) | Archivo entregado | Namespace 2.0 | Qué es |
|---|---|---|---|
| `CollectionTypeEnum`, `BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQueryReturnType`, `DatabaseQuery` | `Model/DomainTree.cs` | `Cooperator.Framework.Modeling.Model` | El modelo del grafo, como **POCO** (sin atributos de editor, sin serialización binaria, sin el `using` del provider ni el acoplamiento al editor de L1621) |
| `DomainTreeHelper` (**mitad de árbol**) | `Model/DomainTreeHelper.cs` | `Cooperator.Framework.Modeling.Model` | La lógica pura del árbol: los 11 métodos del §1 + el caché de namespaces |
| `OrderEnum`, `DBObject`, `DBField`, `DBFieldKey`, `DBFieldFKey`, `DBKey<T>`, `DBPrimaryKey`, `DBForeignKey`, `DBTable`, `DataBase` | `Reverse/DBReverse.cs` | `Cooperator.Framework.Modeling.Reverse` | Los tipos del esquema de base de datos (portables, sin cambios de comportamiento) |
| `IDBReverseProvider`, `IDBTypeConverter`, `DBReverseEngine`, `DBReverseEngineFactory`, `IncludedEnum`, `DBParentsHelper` | `Reverse/DBReverseHelper.cs` | `Cooperator.Framework.Modeling.Reverse` | El contrato portable del reverse y su motor (el motor de generación los consume; el reverse concreto los implementa) |
| `MSSQLTypeConverter` (**extraído** de `Providers/MSSQLCommon.cs`) | `Reverse/TypeMapping.cs` | `Cooperator.Framework.Modeling.Reverse` | **La** fuente única del mapeo SQL↔CLR, como **dato** (28 tipos nativos) + `NativeLengthRule` + la implementación de `IDBTypeConverter`. **Código nuevo** de la 2.0 |
| `UtilityException` | `Exceptions/UtilityException.cs` | `Cooperator.Framework.Modeling.Exceptions` | Base de las excepciones del grafo, ahora sobre **`System.Exception`** |
| `UnknownDBTypeExeception` | `Exceptions/UnknownDBTypeExeception.cs` | `…Modeling.Exceptions` | Tipo nativo desconocido en el mapeo (error del contrato, no coerción silenciosa) |
| `NodeAlreadyExistException` | `Exceptions/NodeAlreadyExistException.cs` | `…Modeling.Exceptions` | Alta de nodo duplicado |
| `CannotAddOrRemoveItemException` | `Exceptions/CannotAddOrRemoveItemException.cs` | `…Modeling.Exceptions` | Operación inválida sobre la colección de nodos |

### 2.2 Diferido al tooling (**obligación registrada**, fase D — NO entregado en este Bolt)

| Tipo / archivo (1.4) | Destino especificado | Qué se hace al materializarse |
|---|---|---|
| `Providers/MSSQLProvider.cs` (`MSSQLReverseFactory`, `MSSQLProvider`) | `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs` | Reubicar; su conversor de tipos pasa a ser el de `Modeling`; conserva su acceso a datos **tal como está** (el retiro de la herencia de la capa de datos es del Bolt 016) |
| `Providers/MSSQLCommon.cs` — el resto (`SP_HelpTablesEnum`, `sp_help`/`INFORMATION_SCHEMA`/`sp_fkeys` y helpers de lectura) | `…/CooperatorModeler/DBReverseHelper/Providers/MSSQLCommon.cs` | Reubicar; **la tabla del mapeo ya no está acá** (vive en `Modeling`, §C.2) |
| `DomainTree.cs` — **mitad reverse** de `DomainTreeHelper` (3 métodos, §1) | `…/CooperatorModeler/DBReverseHelper/DomainTreeHelper.cs` | Reubicar **sólo la mitad reverse**; llama a la mitad portable de `Modeling` (dirección permitida) |
| `PropertyEditors/*` (`DropDownListEditor`, `StringEditor` + `StringEditorForm` + `.Designer` + `.resx`, `ToStringInfoEditor` + `ToStringInfoEditorForm` + `.Designer` + `.resx`) | `…/CooperatorModeler/DBReverseHelper/PropertyEditors/*` | Reubicar como están, con sus recursos |

**Por qué está diferido:** `MSSQLProvider.cs` conserva `private class DataAccess : Cooperator.Framework.Data.BaseRule`
(L499 de la fuente) —un tipo que **no existe** en el árbol 2.0— y `System.Data.SqlClient`, que en `net10.0-windows`
exige un paquete **retirado** por ADR-005 §3.1 (cuya versión y *smoke test* de conexión real son del Bolt 014).
Medido en el V-Bounce N°2 (`MEM-261016-0107` §8.1) y acotado por el SPEC en su revisión 3.

### 2.3 Fuera de la partición (no migra a `Modeling`)

| Elemento | Por qué | Dueño |
|---|---|---|
| `ScriptingHost`, `ScriptsEnvironment`, `CompilerException` | Su slot es el **motor de generación** (ADR-003), no el modelo | Bolts 017/018/019/020/021 |
| Los **comentarios de ejemplo** de las plantillas que mencionan el helper | El 1.4 está congelado; los emite el set de plantillas | El Bolt del set (ADR-003) |
| `Cooperator.Framework.Utility` **como ensamblado y namespace** | ADR-001 §3.1: no existe en la 2.0 | — (esta partición) |

---

## 3. Constancia de lo que dejó de existir en la 2.0

- **El paquete `Cooperator.Framework.Utility`** y **su namespace** (`Cooperator.Framework.Utility.DBReverseHelper`,
  `…DBReverseHelper.Providers`, `…DBReverseHelper.PropertyEditors`, `…Utility.Exceptions`): no existen en la 2.0.
  Su contenido se reparte entre `Modeling` (§2.1, entregado) y el tooling (§2.2, diferido).
- **`DomainTreeHelper` como una sola clase**: existía una vez; ahora son **dos tipos con el mismo nombre** en dos
  ensamblados (la mitad de árbol en `Modeling`, la reverse en el tooling al materializarse la fase D).
- **Los atributos de diseñador** (`[EditorAttribute]`, `[DesignerSerializationVisibility]`, `[Description]`,
  `[Browsable]`, `[DefaultValue]`, `[Category]`) y los `using` de `System.ComponentModel.Design`,
  `System.Windows.Forms*`, `System.Drawing*`: no están en el modelo. La edición de la 2.0 es **dirigida por el
  esquema del contrato** (ADR-009 §3.2), no por atributos en las clases.
- **La serialización binaria** en los tipos que migran (`[Serializable]`, `SerializationInfo`, `ISerializable`,
  `GetObjectData`) **incluidas las excepciones del grafo**: su persistencia es el **`.coop2`** (ADR-008 §3.2), con
  los derivados recalculados al leer. En la superficie de la 2.0 no hay `ISerializable`/`GetObjectData` y el build
  no emite ningún `SYSLIB0051`.
- **El acoplamiento del modelo al editor**: la sentencia `PropertyEditors.DropDownListEditor.stringList = _ParentCandidates;`
  (`DomainTree.cs` L1621, getter `GenerateAsChildOf` de `EntityNode`) **desaparece** del modelo.
- **El `using` del provider concreto** en el modelo (`DomainTree.cs` L35,
  `using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;`) y el `using` **muerto** de
  `IObject.cs`/`DomainTree.cs` hacia `LazyLoad`: el modelo no nombra al provider ni a la UI.
- **El `using System.Runtime.Serialization.Formatters.Binary`** inerte del modelo (F-30): no queda ni el rastro.

---

## 4. Rupturas que este Bolt introduce (insumo de las notas de migración, Bolt 028)

| Ruptura | Qué cambia | Decisión que la ordena |
|---|---|---|
| Tipos que cambian de ensamblado **y** de namespace | `Cooperator.Framework.Utility.DBReverseHelper` → `Cooperator.Framework.Modeling.Model` / `.Reverse`; `…Utility.Exceptions` → `….Modeling.Exceptions` | ADR-001 §3.1/§3.3 |
| Atributos de diseñador retirados | `[EditorAttribute]`, `[DesignerSerializationVisibility]`, `[Description]`, `[Browsable]`, `[DefaultValue]`, `[Category]` | ADR-001 §3.1, ADR-002 §3.1, ADR-009 §3.2 |
| **Acoplamiento del modelo al editor retirado** | la sentencia de `DomainTree.cs` L1621 (getter `GenerateAsChildOf`) desaparece del modelo | ADR-001 §3.1 (§B.1.4) |
| `[Serializable]` / `SerializationInfo` retirados | de los tipos del grafo y de las excepciones que migran; su persistencia es el `.coop2` | ADR-008 §3.2 |
| Base de las excepciones del grafo | `Library.Exceptions.BaseException` → `System.Exception` (`UtilityException` y sus derivadas) | ADR-001 §3.1 (§14(a)) |
| `DomainTreeHelper` partido | dos tipos con el mismo nombre en dos ensamblados; la mitad reverse se reubica con la fase D | ADR-001 §3.3 (§B.2, §D.3) |
| `CompilerException` fuera de la partición | no migra a `Modeling`: su slot es el motor de generación | ADR-003 (§4) |
| **Política de nulabilidad del código portado** | los archivos portados del 1.4 declaran `#nullable disable`; el código **nuevo** mantiene la nulabilidad activa | §A.3 (decisión de la revisión 3) |
| **Fase D diferida (obligación registrada)** | la reubicación del tooling y el proyecto `CooperatorModeler` **no** se entregan: los cierra el porte de la capa `Data` (Bolts 014/016) | §4, §D.3 |
| **Corrección del guard del arnés del Bolt 001** (§A.4) | una línea de `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` (L198): de *exactamente 8 proyectos* a *al menos 8* | §A.4 (revisión 4) — ver §6 |

---

## 5. La obligación registrada de la fase D (§D.3)

**Qué queda diferido.** La **ejecución** de la fase D —la creación de `2.0/src/CooperatorModeler/` y la reubicación
de los cuatro destinos de §2.2— no se entrega en este Bolt. Su **especificación** sí (tipo por tipo, §2.2).

**Quién la cierra.** Al aportar la capa `Data` y reescribir el reverse, los Bolts **014**
([`SPEC-261016-0020`](../../../devflow/spec/SPEC-261016-0020-provider-de-datos.md), provider de ADR-005) y **016**
([`SPEC-261016-0027`](../../../devflow/spec/SPEC-261016-0027-reverse-sin-baserule.md), el reverse que deja de
heredar). Mientras la capa `Data` no exista, la obligación **no se puede cumplir** y su incumplimiento **no** es un
defecto de este Bolt: está declarada, medida y acotada.

**Recordatorios obligatorios para quien la ejecute** (insumos verificados, `MEM-261016-0107` §2 y §13):

1. **Calificar las cuatro llamadas cruzadas** de la mitad reverse a la portable (§1): el tooling tendrá una clase
   con el mismo nombre.
2. **La lista de rupturas (§4) es el insumo de las notas** del Bolt 028: tipos que cambian de ensamblado y
   namespace, atributos de diseñador retirados, `[Serializable]` retirado, base de las excepciones cambiada,
   `DomainTreeHelper` partido, `CompilerException` fuera y el acoplamiento al editor retirado.
3. **`DependencyReportTests`**: el presupuesto de dependencias se mide en el **proyecto/lock file** (`Modeling`
   declara **exactamente** `System.Text.Json`, como lo asserta la invariante del arnés del Bolt 001) y las **aristas
   prohibidas** en el **ensamblado** (hoy referencia **sólo** `netstandard 2.1.0.0`, porque todavía no hay código de
   serialización: es del Bolt 004).

---

## 6. La política de nulabilidad del código portado (§A.3) y el guard corregido (§A.4)

**Nulabilidad.** El árbol 2.0 hereda `Nullable=enable` **y** `TreatWarningsAsErrors=true` (Bolt 001), y el código
del 1.4 es anterior a las anotaciones: portado tal cual produce **28 diagnósticos** (`21× CS8618`, `4× CS8603`,
`3× CS8625`) que el árbol convierte en errores. El mecanismo autorizado es **una directiva por archivo portado del
1.4**, y **sólo** en ellos:

| Llevan `#nullable disable` (archivos **portados** del 1.4) | NO la llevan |
|---|---|
| `Model/DomainTree.cs`, `Model/DomainTreeHelper.cs`, `Reverse/DBReverse.cs`, `Reverse/DBReverseHelper.cs`, `Exceptions/UtilityException.cs`, `Exceptions/UnknownDBTypeExeception.cs`, `Exceptions/NodeAlreadyExistException.cs`, `Exceptions/CannotAddOrRemoveItemException.cs` | `Reverse/TypeMapping.cs` (**código nuevo**: conserva la nulabilidad activa) y las pruebas |

Está **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj` —apagaría el análisis del paquete entero,
incluido el contrato del Bolt 004—.

**El guard del arnés del Bolt 001 (§A.4, revisión 4).** La única excepción autorizada sobre un archivo del Bolt 001,
declarada con archivo, línea y antes/después:

- **Archivo:** `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` — **línea 198** (la aserción de conteo
  dentro de `Every_project_of_the_tree_has_a_versioned_lock_file`).
- **Antes:** `Assert.Equal(Portables.Length + 4, projects.Length); // 4 portables + 2 suites + 2 fixtures`
- **Después:** `Assert.True(projects.Length >= Portables.Length + 4, $"El árbol tiene {projects.Length} proyectos: menos de los {Portables.Length + 4} esperados.");`
- **Por qué:** el árbol tiene **8** proyectos y esta partición agrega el **9.º** (la suite de `Modeling`), de modo que
  el conteo **exacto** impedía crecer (medido en el V-Bounce N°3, `MEM-261016-0125` §8.1: `Failed: 1, Passed: 48,
  Total: 49`; `Expected: 8 / Actual: 9`). El conteo pasa a ser un **mínimo** (guardia contra una enumeración débil).
- **Qué NO cambió:** el `foreach` que exige `packages.lock.json` versionado en **cada** proyecto del árbol, la
  aserción de conteo de `2.0/src` (`Assert.Equal(Portables.Length, projects.Length)`, L181) y **ningún otro archivo
  del Bolt 001**. Es un acto **único para la ola** (los Bolts que agreguen suites o fixtures ya no se topan con la
  colisión).

---

## 7. Cómo se verifica

| Evidencia | Dónde |
|---|---|
| El paquete compila sin aristas salientes y sin dependencias fuera de la admitida | `DependencyReportTests` (dos niveles: proyecto/lock file y ensamblado) |
| El mapeo de tipos es **uno solo** y coincide con la tabla del 1.4 | `TypeMappingTests` |
| El modelo es POCO (sin atributos de diseñador ni serialización binaria) | `ModelPocoTests` |
| La partición del helper y los ocho sitios del modelo resueltos **dentro** del paquete | `DomainTreeHelperPartitionTests` |
| Paridad observable de los tipos movidos (grafo, derivados, orden, `ToString`, claves y relaciones) | `ModelCharacterizationTests` |
| El árbol entero, con el **9.º proyecto** y el guard corregido | `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` |
