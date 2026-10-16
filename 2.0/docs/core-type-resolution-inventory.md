# Inventario de los puntos de resolución de tipos del camino filas→objetos

> **SPEC-261016-0034 §6/A y AC-1/AE-7.** Inventario de los puntos del camino de mapeo de `Cooperator.Framework.Core`
> que identifican un **tipo** o un **miembro**, con su resolución **declarada**, su diagnóstico y su **punto
> gemelo**. La evidencia de 1.4 está en el árbol congelado y se cita con su línea; el código 2.0 la corrige sin
> reintroducir ninguna forma de resolución implícita (ADR-006 §3.1).

## 1. Los puntos (completos: los tres, con su gemelo)

| # | Punto (1.4) | Qué identifica | De dónde viene el valor | Resolución **declarada** en 2.0 | Diagnóstico |
|---|---|---|---|---|---|
| 1 | `Core/BaseObjectList.cs:409-418` — el mapeo construye una columna del `DataSet` | un **tipo** (el de la propiedad de destino) | del **propio tipo**: `prop.PropertyType` | **No se pasa por un nombre**: se usa el tipo y, si es `Nullable<T>`, su tipo subyacente **por API** (`Nullable.GetUnderlyingType`) — `TypeIdentification.UnwrapNullable` | `TypeResolutionException` si el tipo declarado no está disponible |
| 2 | `Core/BaseObjectList.cs:342-346` — comparador de orden | una **propiedad** del tipo destino | un **nombre de propiedad** recibido del llamador | `TypeIdentification.RequireProperty(type, propertyName, "BaseObjectList")`: una sola verificación compartida | `TypeResolutionException` que nombra la **propiedad**, el **tipo** y el **punto** (y las propiedades existentes del tipo) |
| 3 (**gemelo**) | `Core/ObjectListHelper.cs:64-98` — llenado del `DataSet` (`propertyArray[counter] = typeof(T).GetProperty(column.ColumnName)` dentro de un `catch (Exception)`) | una **propiedad** del tipo `T` a partir del **nombre de columna** | un **nombre de columna** del `DataSet` | `TypeIdentification.RequireProperty(typeof(T), column.ColumnName, "ObjectListHelper.FillDataSet")` — misma verificación que el punto 2 | `TypeResolutionException` **propagada**: el `catch (Exception)` deja de tragarse la causa; el `InvalidDataSetException` queda **sólo** para lo que efectivamente es un desajuste de esquema |
| 4 | `Core/BaseObjectList.cs` — nombre de tipo que llega de afuera (si llega) | un **tipo** por **nombre** | un **nombre emitido por otro componente** | `TypeIdentification.ResolveQualifiedName(name, origin)`: **exige** nombre calificado con ensamblado y **falla** si no resuelve. Un nombre **simple** se rechaza **sin intentar resolverlo** | `TypeResolutionException` con el valor intentado y el punto |
| 4b | `Core/BaseObjectList.cs:380,390` — el mapeo **clasifica** una propiedad como de sistema o de negocio | la **categoría** de un tipo (¿del sistema?) | del **propio tipo** | Por **API**: `Type.Namespace` (nunca el formato de `ToString()`, que puede cambiar entre runtimes y era la otra cara de la fragilidad de F-47) | Si el tipo no declara namespace, se trata como propiedad **de negocio** (el caso del consumidor) |
| 5 | `Core/Factory/Factories.cs:53,70` — `Assembly.Load`/`LoadFrom` | un **tipo** por nombre de provider | configuración del host | **Fuera de este camino y de este Bolt**: es del **Bolt 005** (ADR-006 §3.1/§3.2). Acá sólo se **hereda su regla** y se verifica que no se reintroduzca resolución implícita | — (dueño: Bolt 005) |

## 2. Frontera declarada (excluida, con su dueño)

| Observación | Dueño | Por qué está fuera |
|---|---|---|
| `Core/BaseObjectListView.cs:68` — `_Filter.ToLower().Replace(" ", "").Contains("deleted=false")` (interpretación textual del filtro de las vistas) | Bolt dueño de la **evaluación de expresiones** (observación registrada en el DoR del Bolt) | Es **evaluación de expresiones**, no identificación de tipos: el Bolt lo excluye expresamente (SPEC §2 y §4) y ADR-006 §3.1 no lo alcanza |
| El **contrato del modelo** y el código generado que consume el mapeo | Bolts 003/004 y 019 (ADR-005 §3.3) | La **forma** del contrato se **preserva**, no se rediseña |

## 3. La regla de nombres aplicada (una sola)

La identificación de tipos del camino de mapeo usa **la misma regla** que el resolutor de providers del Bolt 005
(ADR-006 §3.1/§3.2): **una sola forma de nombrar un tipo en la 2.0, la calificada**. El camino de mapeo:

- **no** identifica tipos por nombre simple (0 `Type.GetType(<nombre simple>)`),
- **no** reconstruye nombres por manipulación de cadenas de reflexión (0 `Substring`/`ToString()` sobre
  `PropertyType`),
- **no** carga ensamblados (0 `Assembly.Load*`: la carga vive en el Bolt 005 con su contrato).

El control automatizado de las tres condiciones vive en `NoImplicitResolutionTests`.

## 4. Los valores reales que alimentan cada punto

| Punto | Valor real | Forma en que llega | Dueño de la forma |
|---|---|---|---|
| 1 | `prop.PropertyType` del modelo de negocio | **tipo disponible** (objeto `Type`): no hay nombre que adivinar | — (el propio camino) |
| 2 | nombre de propiedad del comparador de orden | cadena del llamador (dentro del framework) | — (el propio camino) |
| 3 | `column.ColumnName` del `DataSet` que se llena | nombre de **columna**, producido por el esquema que el mapeo construye en el punto 1 | — (el propio camino construye el `DataColumn` con el tipo del punto 1) |
| 4 | nombre de tipo emitido por otro componente (si existiera) | **debe llegar calificado**; una forma no calificada es un **hallazgo con dueño**, no una tolerancia del resolutor | el componente emisor (Bolts de plantillas/modelo: 019/004) |

> **Verificación sobre el corpus:** `RealValuesOverCorpusTests` recorre `2.0/tests/fixtures/model-corpus/**`
> cuando el corpus está presente y, siempre, verifica sobre los tipos del propio framework la forma declarada de
> cada punto. Si el corpus **no** está en el árbol, el caso lo **declara** (no se inventa un corpus: §15 #9).

## 5. Trazabilidad

- Decisión: **ADR-006 §3.1/§3.2/§3.3** (regla de nombres calificados, unicidad y diagnósticos) · **ADR-001 §3.2/§3.4**
  (piso de consumo, capa base sin terceros) · **ADR-005 §3.3** (la forma del contrato se preserva).
- Hallazgo de origen: **REV-001 F-47** y su **§6 ítem 19**.
- Corrección: `2.0/src/Cooperator.Framework.Core/Mapping/TypeIdentification.cs` +
  `Exceptions/TypeResolutionException.cs`; correcciones en `BaseObjectList.cs` y `ObjectListHelper.cs`.
- Constancia de las rupturas de **comportamiento**: `2.0/docs/api-removals/US-000.BOLT-008.json` (insumo del Bolt 028).
