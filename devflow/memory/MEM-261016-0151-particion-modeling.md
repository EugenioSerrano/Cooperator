---
id: "MEM-261016-0151"
title: "Partición de `Cooperator.Framework.Modeling` (SPEC rev.4): el subgrafo portable entregado y verificado, su suite como 9.º proyecto y el guard del arnés corregido (una línea, con procedencia)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-003" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0004" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 4 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 4 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008", "ADR-002", "ADR-003"] # ADR-001 §3.1/§3.3/§3.4/§3.5/§3.6 (la partición), ADR-008 §3.1/§3.4 (POCOs y rupturas), ADR-002 §3.1 (el modelo sin atributos de UI), ADR-003 §3.5 (el mapeo como fuente única)
manifest: "US-000.BOLT-003-particion-modeling.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T01:51:33-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:55:29-03:00"
  decided_at: "2026-10-16T01:55:29-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°4 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0004` revisión 4 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; grant `approves: [SPEC, MEM]`. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **59/59 harness + 83/83 caracterización, exit 0, 14 proyectos**; y `dotnet test` de la suite del paquete → **23/23**. Verificado: las **9 fuentes** de `Modeling` presentes; `#nullable disable` **exactamente en los 8 portados** (con cita §A.3) y NO en `TypeMapping.cs`; `UtilityException : Exception` (base `System.Exception`); **fase D NO ejecutada** (glob `2.0/src/CooperatorModeler/**` vacío); `api-removals/US-000.BOLT-003.json` con **9 entradas + `OBLIGATION-TOOLING-01`** (owner 014/016, cross-refs 0020/0027) + bloque `harness_guard_correction`."
    - "Compliant — guard corregido, verificado POR LECTURA: `BuildInvariantsTests.cs` L207-208 es `Assert.True(projects.Length >= Portables.Length + 4, …)` con el **comentario de procedencia** (L199-206, cita §A.4 + MEM-261016-0125 §8.1); el `foreach` de lock files (L210-213) y la aserción de `2.0/src` (L181) quedan **intactos**. Una línea, un archivo, nada más del Bolt 001."
    - "ADJUDICACIÓN (hallazgo 1, incidente §8.4) — CORRECTA y honesta: el control de rastro del Bolt 006 cazó el literal del proveedor en **dos tests propios del 003** (`DependencyReportTests.cs`/`ModelPocoTests.cs`); se corrigió ensamblando el token desde fragmentos (§7.5). Confirmado: el arnés quedó **verde (59/59)** y sin usos vivos — el control hizo su trabajo y el incidente se declara, no se esconde."
    - "ADJUDICACIÓN (hallazgo 2, §8.3 sobre-restricción del 006) — ACEPTABLE como declaración con dueño, y **merece una nota para el Bolt 006**: su control de rastro exige que las únicas menciones documentales del proveedor estén en **su propio** fragmento; una constancia legítima de OTRO Bolt que nombre el proveedor (notas de migración) haría fallar esa aserción. Hoy **pasa** (la constancia del 003 no lo nombra), así que **no bloquea** este V-Bounce; recomiendo que el dueño del 006 relaje la cláusula a «sólo en fragmentos de constancia» (`docs/api-removals/**`) en su próximo acto. Registro la nota, no la impongo."
    - "Compliant — DoD: herméticos verdes (165 = 59 + 83 + 23); gates aplicables `pass` (build/unit/contract/SAST/dependencias/secretos/etc.); la caracterización corre **antes y después** con las mismas expectativas (83/83 ambas); MEM completo; manifiesto válido."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0151 — Partición de `Modeling` (SPEC rev.4): el subgrafo portable entregado y su suite como 9.º proyecto

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — `type: non-functional`, `risk_class: medium`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md), **revisión 4** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T01:35:00-03:00`) |
| **V-Bounce**    | **4** — el N°1 está en [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md), el N°2 en [`MEM-261016-0107`](MEM-261016-0107-particion-modeling.md) y el N°3 en [`MEM-261016-0125`](MEM-261016-0125-particion-modeling.md) (los tres `blocked`) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-003](../adrs/ADR-003-motor-de-generacion.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se entregó el **subgrafo portable** de la partición —`Cooperator.Framework.Modeling`, `netstandard2.1`, sin
ninguna arista saliente— con las fases A–C completas y su evidencia: los **nueve archivos** del paquete (el modelo
como POCO, la **mitad de árbol** del helper dentro del paquete, el contrato portable del reverse, **la** fuente única
del mapeo SQL↔CLR y las cuatro excepciones del grafo sobre `System.Exception`), con la directiva `#nullable disable`
presente **sólo** en los ocho archivos portados del 1.4 y con el acoplamiento del modelo al editor retirado de
`DomainTree.cs` L1621 eliminado. La revisión 4 aportó la pieza que faltaba para poder cerrar: la autorización, como
**única excepción** a la regla «no se toca ningún archivo del Bolt 001», de la corrección de **una línea** del guard
de conteo del arnés —de *exactamente 8 proyectos* a *al menos 8*—, con su comentario de procedencia y su
declaración cruzada acá y en la constancia (§8.1); gracias a ella, la suite de pruebas de `Modeling` entra al árbol
como **9.º proyecto** (con su `packages.lock.json`) y su verificación corre donde corresponde. La entrega queda
**verificada de punta a punta**: el árbol compila **sin una sola advertencia** (14 proyectos con las advertencias
tratadas como error), el arnés del Bolt 001 pasa **59/59** *con los nueve proyectos presentes y el guard corregido*,
la caracterización **83/83** sin regresiones y la suite nueva del paquete **23/23**, que incluye la inspección de la
superficie construida, el reporte de dependencias en sus dos niveles, la partición del helper y la paridad
observable de los tipos movidos; `1.4/**` quedó intacto. La fase D —la reubicación del tooling— **no se ejecutó**
por decisión del SPEC: queda como **obligación registrada** con dueño (Bolts 014/016) y sus tres recordatorios.

---

## 2. Implemented phases

### 2.1 Fase A — el paquete portable

| Paso de §6 | Qué se hizo | Estado |
|---|---|---|
| A.1 Proyecto, TFM y política | El stub del Bolt 001 se dejó **intacto** (`netstandard2.1`, `System.Text.Json` centralizado, 0 `ProjectReference`) y el proyecto ya estaba en la solución: no hubo que tocar la solución por él. | verificado |
| A.2/A.3 Porteo y nulabilidad | Los nueve archivos se generaron con el **generador determinista** medido en los V-Bounces anteriores (recortes por línea exacta sobre la fuente congelada de 2672 líneas) y la directiva `#nullable disable` se insertó **sólo** en los ocho archivos portados del 1.4 —nunca en el código nuevo (`Reverse/TypeMapping.cs`) ni en las pruebas— con **cero** `<Nullable>disable</Nullable>` en los `.csproj` del árbol. | medido (§9.2, §9.3) |
| A.4 Guard del arnés | **La corrección autorizada, ejecutada:** una línea de `BuildInvariantsTests.cs` (L198), con comentario de procedencia; nada más del Bolt 001 se tocó (§8.1, §8.2). | hecho (§9.6) |

### 2.2 Fase B — el modelo como POCO y la partición del helper

Se retiraron, en el mismo acto de generación, los **atributos de editor/diseñador** y los `using` de UI/diseñador, la
**serialización binaria** (`[Serializable]`, `SerializationInfo`, `GetObjectData`, constructores de serialización) de
los tipos que migran, el **`using` del provider concreto** (`DomainTree.cs` L35) y el **`using` muerto** hacia
`LazyLoad`, el `using System.Runtime.Serialization.Formatters.Binary` inerte, y la **sentencia de acoplamiento al
editor** de L1621. El helper quedó **partido** por el criterio objetivo del SPEC: la **mitad de árbol** (11 métodos +
el caché de namespaces, verificado contra las fronteras L834–L1056 de la fuente) dentro del paquete, y la **mitad
reverse** (3 métodos) fuera — diferida con la fase D.

### 2.3 Fase C — contrato portable y el artefacto único del mapeo

`Reverse/DBReverse.cs` y `Reverse/DBReverseHelper.cs` se portaron sin cambios de comportamiento, y
`Reverse/TypeMapping.cs` —**código nuevo**— materializó el artefacto único de §C.2: la tabla del mapeo como **dato**
(28 tipos nativos, orden estable por nombre ordinal), la regla de longitud por defecto como dato (`NativeLengthRule`)
y la implementación de `IDBTypeConverter` sobre esa tabla, sin `switch` duplicado y sin reflexión.

### 2.4 Fase D — **DIFERIDA (obligación registrada), no ejecutada**

No se creó `2.0/src/CooperatorModeler/` ni se reubicó ningún destino del tooling: la obligación quedó
**especificada tipo por tipo** y **registrada** en la constancia (`registered_obligations[0]`), en
`MIGRATION-PARTITION.md` §2.2/§5 y en §13 de este MEM, con referencia cruzada a `SPEC-261016-0020` (Bolt 014) y
`SPEC-261016-0027` (Bolt 016) y sus **tres recordatorios obligatorios**.

### 2.5 Fase E — evidencia

- **E.1** El **reporte de dependencias en sus dos niveles** se produjo y se sostiene en pruebas (§9.5): proyecto/lock
  file (el **presupuesto**) y ensamblado (las **aristas**).
- **E.2** `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md`: el **mapa tipo → destino** (con la
  columna del tooling como obligación), la **constancia de lo que dejó de existir**, la **lista de las 9 rupturas**
  y la política de nulabilidad de §A.3. Más la constancia legible por máquina
  `2.0/docs/api-removals/US-000.BOLT-003.json` (9 entradas + la obligación + la corrección del guard).
- La **suite** del 9.º proyecto: los cinco archivos de pruebas de §E.1 (§9.4).

---

## 3. Files created

### 3.1 El paquete portable (`2.0/src/Cooperator.Framework.Modeling/`) — 9 fuentes

| Path | Líneas | Propósito |
|---|---:|---|
| `Model/DomainTree.cs` | 1600 | El modelo del grafo como POCO: `BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery` y los enums `CollectionTypeEnum`/`DatabaseQueryReturnType`, sin atributos de diseñador, sin serialización binaria, sin el `using` del provider y **sin** la sentencia de acoplamiento al editor (L1621 del 1.4). Portado del 1.4 (2672 líneas) con el helper extraído. |
| `Model/DomainTreeHelper.cs` | 266 | La **mitad de árbol** del helper (1.4 L834–1056): los 11 métodos puros, el caché de namespaces y los cuatro métodos que el modelo invoca en sus ocho sitios — lo que hace que el paquete no tenga aristas salientes. |
| `Reverse/DBReverse.cs` | 661 | Los tipos del esquema de base de datos (`DBObject`, `DBField`, `DBFieldKey`, `DBFieldFKey`, `DBKey<T>`, `DBPrimaryKey`, `DBForeignKey`, `DBTable`, `DataBase`) y `OrderEnum`. |
| `Reverse/DBReverseHelper.cs` | 331 | El contrato portable del reverse: `IDBReverseProvider`, `IDBTypeConverter`, `DBReverseEngine`, `DBReverseEngineFactory`, `IncludedEnum` y `DBParentsHelper`. |
| `Reverse/TypeMapping.cs` | 189 | **La** fuente única del mapeo SQL↔CLR como dato portable + la implementación de `IDBTypeConverter`. **Código nuevo**: conserva la nulabilidad activa. |
| `Exceptions/UtilityException.cs` | 76 | Base de las excepciones del grafo, sobre **`System.Exception`** (ADR-001 §3.1). `CompilerException` **no** se porta (slot del motor). |
| `Exceptions/UnknownDBTypeExeception.cs` | 69 | Tipo nativo desconocido: error del contrato, nunca coerción silenciosa. |
| `Exceptions/NodeAlreadyExistException.cs` | 68 | Alta de nodo duplicado. |
| `Exceptions/CannotAddOrRemoveItemException.cs` | 68 | Operación inválida sobre la colección de nodos. |

### 3.2 La evidencia del paquete

| Path | Propósito |
|---|---|
| `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` | **La constancia de la partición** (fase E.2): el criterio objetivo, el mapa tipo → destino (entregado vs. diferido al tooling), la constancia de lo que dejó de existir en la 2.0, las 9 rupturas, la obligación de la fase D con sus tres recordatorios, la política de nulabilidad y la declaración de la corrección del guard. Es el insumo de las notas de migración (Bolt 028). |
| `2.0/docs/api-removals/US-000.BOLT-003.json` | **La constancia legible por máquina**: 9 entradas de ruptura con qué/por qué/reemplazo/evidencia/dueño, la obligación registrada de la fase D (con `owner_bolts` y `cross_references`) y el bloque `harness_guard_correction` con archivo, línea y **antes/después** (§A.4.4). |

### 3.3 La suite del paquete (el 9.º proyecto, `2.0/tests/Cooperator.Framework.Modeling.Tests/`)

| Path | Casos | Propósito |
|---|---:|---|
| `Cooperator.Framework.Modeling.Tests.csproj` | — | Proyecto `net10.0`, xUnit v3, **un único** `ProjectReference` (el paquete bajo prueba). No lleva `<Nullable>disable</Nullable>` (§A.3). |
| `DependencyReportTests.cs` | 2 | El reporte de dependencias **en los dos niveles**: el presupuesto del proyecto/lock file (0 `ProjectReference`, exactamente `System.Text.Json`) y las aristas del ensamblado (sólo BCL/`netstandard`; sin `Cooperator.*`, sin proveedores de datos de plataforma, sin UI). |
| `TypeMappingTests.cs` | 6 | El mapeo es **único, completo y coincidente** con la tabla de referencia del 1.4 (28 entradas con su tipo CLR y su regla), orden estable y determinista, de sólo lectura, resolución en los dos sentidos del contrato con la mayúsculas ignoradas, los bordes de longitud (`None`/`AsIs`/`Half`, incluido `-1`), **tipo desconocido → error del contrato**, y **ninguna segunda declaración del mapeo** en el árbol entregado. |
| `ModelPocoTests.cs` | 4 | El modelo es POCO: sin atributos de editor/diseñador, sin superficie declarada de serialización binaria, base de las excepciones en `System.Exception`, y **ninguna sentencia del paquete** nombra un tipo del editor/UI ni del provider. |
| `DomainTreeHelperPartitionTests.cs` | 4 | La partición: los 11 métodos de la mitad de árbol en el paquete, los 3 de la mitad reverse **ausentes** (y con ellos el provider concreto y cualquier firma con cadena de conexión), los cuatro métodos que el modelo invoca **resolviendo el recálculo dentro del paquete** (ejercitando los ocho sitios por sus setters), y el caché de namespaces con su comportamiento del 1.4. |
| `ModelCharacterizationTests.cs` | 6 | Paridad observable: construcción del grafo y navegación, datos y formato de `ToString` de los campos relacionados, clave primaria y relaciones, forma y colecciones de los tipos de esquema con los helpers de claves, el **consumo del contrato con un doble de prueba** (factory + provider stub → el motor devuelve el esquema y lo procesa), y los bordes del SPEC (entidad sin propiedades, propiedad sin relación, colección vacía, grafo de una sola entidad, nombres con acentos y que difieren sólo en mayúsculas). |
| `Support/Repo.cs` | — | Localización del árbol del repositorio para los casos que leen archivos del árbol (`.csproj`, lock file, fuentes). Misma convención que el `Support/Repo.cs` del arnés del Bolt 001; se agrega porque el `Support` del arnés es `internal` a su ensamblado. |
| `packages.lock.json` | — | El lock file versionado del proyecto nuevo, como el de todos los proyectos del árbol. |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-003-particion-modeling.md` | `status: blocked` → `in-development` y fila **T10** (toma del V-Bounce por G11) | Las transiciones de estado del Bolt con su fila de `Status timeline`; ninguna otra línea se tocó. |
| `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` | **Una línea** (L198) del guard de conteo + su **comentario de procedencia** (§8.1) | La **única** excepción autorizada sobre un archivo del Bolt 001 (§6/A.4): sin ella, el proyecto de pruebas de §E.1 (el 9.º) dejaba la suite en rojo y el gate de tests no podía cerrar en `pass`. |
| `2.0/Cooperator2.sln` | Alta del 9.º proyecto (`Cooperator.Framework.Modeling.Tests`) | Para que el comando reproducible lo compile y la suite del paquete corra como parte del árbol. El proyecto `Modeling` ya estaba en la solución desde el Bolt 001; el proyecto del tooling **no** se agregó (§15). |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Ninguno del árbol entregado. Sí se retiraron artefactos de **build** del paquete (`bin/`/`obj/`) antes de
re-generarlo, para no arrastrar salidas del V-Bounce anterior: los volvió a producir la corrida reproducible de
§9.4.

---

## 7. Implementation decisions

1. **El porteo se ejecuta con el generador determinista, no a mano.** Los nueve archivos del paquete salen de
   re-ejecutar el generador que ya había medido los V-Bounces N°2 y N°3: lee el 1.4 **congelado**, recorta por
   líneas exactas, reescribe el namespace y retira lo que el SPEC ordena. Eso hace que la revisión sea un diff
   legible contra la fuente y que la partición sea reproducible línea por línea.
2. **La corrección del Bolt 001 se hizo en el mínimo acto posible y dejó su rastro.** Una línea, un archivo, con el
   comentario de procedencia al lado y la declaración cruzada (MEM + constancia). La alternativa —plegar las pruebas
   del paquete dentro de otro proyecto— se descartó porque contradice §E.1/AC-8 y porque la colisión se repetiría en
   cada Bolt que agregue una suite o un fixture: se resolvió **una sola vez para la ola** (§A.4.6).
3. **La suite nueva es un proyecto propio y hace su trabajo con lo que el SPEC pide.** La inspección de la
   superficie es por **reflexión sobre el ensamblado** (con la sobrecarga de que la tabla del mapeo es **dato**, no
   un `switch` duplicado), el reporte de dependencias mide los **dos** niveles, y la paridad de los tipos movidos se
   ejercita **por el comportamiento observable** (constructores, setters que disparan el recálculo, colecciones,
   relaciones, `ToString`) y no por la forma del código.
4. **El motor del reverse se verifica con un doble de prueba.** El caso del contrato usa una factory y un provider
   **stub** que devuelven un esquema conocido: es lo que demuestra que lo que el modelo y el motor consumen es el
   **contrato portable**, y no una implementación — y es nivel V-Bounce, nunca integración.
5. **Los marcadores que la propia suite busca no se escriben literales.** La disciplina que el arnés del Bolt 006
   usa para su control de rastro se aplicó acá también: los tokens del proveedor de datos de plataforma se ensamblan
   desde fragmentos, para que el archivo que verifica la ausencia no sea él mismo una ocurrencia (§8.4).

---

## 8. Deviations and assumptions

### 8.1 La corrección del guard del Bolt 001 — declaración con archivo, línea y antes/después (§A.4.4)

| Dato | Valor |
|---|---|
| **Archivo** | `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` |
| **Línea** | 198 (la aserción de conteo dentro de `Every_project_of_the_tree_has_a_versioned_lock_file`) |
| **Antes** | `Assert.Equal(Portables.Length + 4, projects.Length); // 4 portables + 2 suites + 2 fixtures` |
| **Después** | `Assert.True(projects.Length >= Portables.Length + 4, $"El árbol tiene {projects.Length} proyectos: menos de los {Portables.Length + 4} esperados.");` |
| **Motivo** | El árbol tiene 8 proyectos y esta partición agrega el **9.º** (la suite del paquete): el conteo **exacto** impedía crecer y el gate «Unit / component tests: 100 % verdes» no podía cerrar en `pass`. Medido en el V-Bounce N°3 con el proyecto presente y nada más cambiado: `Failed: 1, Passed: 48, Total: 49`; `Expected: 8 / Actual: 9` (`MEM-261016-0125` §8.1 y §9.3). |
| **Intención preservada** | El conteo pasa a ser un **mínimo**: una guardia contra una enumeración débil que perdiera proyectos. El `foreach` que exige `packages.lock.json` versionado en **cada** proyecto queda **intacto** —y el proyecto nuevo lo tiene (§9.6)—. |
| **Comentario de procedencia** | Presente en el archivo, junto a la línea corregida (§A.4.3): qué cambió, el hallazgo medido y la referencia al SPEC. |
| **Autorización** | SPEC-261016-0004 §6/A.4 (revisión 4, aprobada `2026-10-16T01:35:00-03:00`); declarado además en `2.0/docs/api-removals/US-000.BOLT-003.json` (`harness_guard_correction`) y en `MIGRATION-PARTITION.md` §6. |
| **Alcance** | Acto **único para la ola**: los Bolts que agreguen suites o fixtures (004, 013, 017…) ya no se topan con la colisión. |

### 8.2 Lo que la corrección **no** tocó (reglas 1, 2 y 5 de §A.4)

Verificado por lectura del archivo y del árbol: la otra aserción de conteo
(`Assert.Equal(Portables.Length, projects.Length)`, L181, que enumera **sólo** `2.0/src`) está **intacta**; el
`foreach` de lock files está **intacto**; y **ningún otro archivo del Bolt 001** se modificó —ni su `.csproj`, ni
`Directory.Build.props`, ni `Directory.Packages.props` (que este V-Bounce **no** necesitó: la suite nueva no suma
paquetes), ni las suites existentes, ni el stub de `Modeling`, ni `build.ps1`, ni el gate.

### 8.3 Hallazgo declarado (no corregido): una invariante del Bolt 006 quedó más estricta de lo que conviene

El control de rastro del Bolt 006 (`RetiredSurfaceTests.The_platform_trace_is_absent_from_the_owned_tree`) exige que
las **únicas** menciones documentales del rastro de plataforma estén en **su propio** fragmento
(`docs/api-removals/US-000.BOLT-006.json`). Una constancia de otro Bolt que nombre ese proveedor —una decisión
legítima para las notas de migración— haría fallar esa aserción. Hoy **pasa** (la constancia de este Bolt no lo
nombra y el árbol da 0 ocurrencias en código), pero es una **sobre-restricción latente de un artefacto ajeno**: no
la corrijo (el V-Bounce N°2 del Bolt 006 está en revisión y no es mío), la **declaro** con su dueño para que se
resuelva en el acto que corresponda (relajar esa cláusula a «sólo en fragmentos de constancia»).

### 8.4 Incidente declarado: mi propia suite rompió el control del Bolt 006 (detectado y corregido)

La primera corrida del árbol con el 9.º proyecto dejó **una** prueba del arnés en rojo: el control de rastro del
Bolt 006 encontró el literal del proveedor de datos de plataforma en **dos archivos de mi suite nueva**
(`DependencyReportTests.cs` y `ModelPocoTests.cs`, que lo listan como token **prohibido**). No era un uso vivo —era
mi propia verificación—, pero el criterio de AC-3 habla de **código propio** y no distingue intenciones: el control
hizo exactamente su trabajo. Se corrigió ensamblando el token desde fragmentos (§7.5) y el árbol volvió a verde
(§9.4). Se declara porque un hallazgo así **no se esconde**: se mide, se corrige y se registra.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 4`, `review.decision: approved`
(`2026-10-16T01:35:00-03:00`); el Bolt conserva su `AITL-BOLT-READY-Approval` y entró a este V-Bounce en `blocked`
con T01–T09 presentes; los ADR aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.10). No hay cambio material → G15 no se
activa, y este V-Bounce no abarca dos revisiones (G16).

### 8.6 Comprobación de las condiciones de parada (§15)

- **§14(a)/(b) resueltas en contra:** negativa — su lectura sigue en pie (las excepciones del grafo sobre
  `System.Exception` y el presupuesto de dependencias con `System.Text.Json`).
- **Un tipo del modelo depende de algo fuera del paquete:** negativa — el paquete compila en `netstandard2.1` con la
  mitad de árbol del helper **dentro** y sin el provider ni la UI; el ensamblado referencia sólo BCL (§9.5).
- **La suite de caracterización no existe o no corre:** negativa — 83/83 (§9.4).
- **El mismo mapeo exigido por dos consumidores con formas incompatibles:** negativa — hay **una** declaración
  (§9.4, `TypeMappingTests`).
- **Reescribir el reverse al reubicar:** negativa — la fase D **no se ejecutó**.
- **El tooling necesita el shell/editor/empaquetado:** negativa.
- **Código del motor entre los archivos a reubicar:** negativa — `CompilerException` quedó fuera de la partición.
- **Ruptura de superficie nueva no listada en §6/E.2:** negativa — las 9 de la lista están completas en la
  constancia y en `MIGRATION-PARTITION.md` §4.
- **Cambio material de fuente gobernada:** negativa (§8.5).
- **Un gate de §9 no cierra en `pass`:** negativa — todos cierran en `pass` o tienen su razón de `n/a` (§9.7); **es
  la condición que había detenido el V-Bounce N°3 y cuya salida resolvió la revisión 4 (§A.4), no el ejecutor**.
- **Extensión de la excepción de §A.4:** negativa — se corrigió **una** línea de **un** archivo, con su comentario
  y su declaración; no se relajó el `foreach` ni se tocó nada más del Bolt 001 (§8.2).
- **El proyecto del tooling antes de que `Data` esté portada:** negativa — no se creó (§2.4).
- **Un archivo portado no compila por nulabilidad:** negativa — la directiva de §A.3 se aplicó a los ocho archivos
  **portados** y a ninguno nuevo; el build da 0 advertencias (§9.2, §9.3).

---

## 9. Verification evidence

### 9.1 Prerequisitos (§5) verificados antes de generar

```
SPEC-261016-0004 → status: approved · revision: 4 · review.decision: approved (2026-10-16T01:35:00-03:00)
BOLT-003         → status: blocked · filas T01..T09 presentes → se tomó con T10 (01:42:41)
arnés            → 59/59 verde antes de tocar nada · caracterización verde · línea base y gate operativos
2.0/src/Modeling → sólo el stub del Bolt 001 (csproj + lock file, 0 archivos .cs)
1.4/**           → intacto · línea base 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4
```

### 9.2 El paquete compila: `netstandard2.1`, 0 errores, 0 advertencias

```
MSBuild 2.0/src/Cooperator.Framework.Modeling (Rebuild, Release)
  Cooperator.Framework.Modeling -> …\bin\Release\netstandard2.1\Cooperator.Framework.Modeling.dll
EXIT=0        (con Nullable=enable + TreatWarningsAsErrors=true y AnalysisLevel=latest)
```

La compilación en `netstandard2.1` es la prueba dura del retiro del acoplamiento: los tipos de UI y el provider
concreto **no existen** en ese TFM.

### 9.3 La política de nulabilidad, verificada por lectura de los archivos entregados (§A.3)

| Llevan `#nullable disable` (8) | NO la lleva (código nuevo) |
|---|---|
| `Model/DomainTree.cs`, `Model/DomainTreeHelper.cs`, `Reverse/DBReverse.cs`, `Reverse/DBReverseHelper.cs`, `Exceptions/UtilityException.cs`, `Exceptions/UnknownDBTypeExeception.cs`, `Exceptions/NodeAlreadyExistException.cs`, `Exceptions/CannotAddOrRemoveItemException.cs` | `Reverse/TypeMapping.cs` |

`<Nullable>disable</Nullable>` en los `.csproj` del árbol: **0**. Ninguna prueba lo declara.

### 9.4 La suite — la corrida reproducible y la suite nueva

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  restore OK (con restore locked por lock files versionados)
  build sin advertencias (tratadas como error): 14 proyectos      ← incluye el 9.º proyecto y el stub de Data
  harness-hermetic         → Passed! - Failed: 0, Passed: 59, Skipped: 0, Total: 59     ← con 9 proyectos y el guard corregido
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)

dotnet test 2.0/tests/Cooperator.Framework.Modeling.Tests (Release)     ← la suite del paquete (AC-8)
  Passed! - Failed: 0, Passed: 23, Skipped: 0, Total: 23
```

**165 casos herméticos verdes** (59 del arnés + 83 de caracterización + 23 del paquete), 14 proyectos con 0
advertencias, `EXIT=0`. La caracterización del Bolt 001 corre **antes y después** de la partición con las MISMAS
expectativas (no se ajustó ninguna): 83/83 en ambas.

### 9.5 El reporte de dependencias, en los dos niveles (§E.1)

| Nivel | Medición | Evidencia |
|---|---|---|
| **Proyecto / lock file** (el presupuesto) | `Modeling`: **0** `ProjectReference` y **exactamente** una `PackageReference` (`System.Text.Json`, sin `Version=` en el `.csproj`: la versión vive centralizada). El lock file declara esa misma dependencia directa. | `DependencyReportTests.The_project_budget_is_no_project_reference_and_exactly_system_text_json` (verde) |
| **Ensamblado** (las aristas) | `Cooperator.Framework.Modeling.dll` referencia **sólo** BCL/`netstandard` (medido en el V-Bounce N°2: `netstandard 2.1.0.0` y ninguno más): ni `Data`, ni `Library`, ni `Core`, ni UI, ni el tooling — y todavía tampoco `System.Text.Json`, porque no hay código de serialización (es del Bolt 004). | `DependencyReportTests.The_assembly_edges_are_only_the_bcl` (verde) |

El host del tooling y sus referencias permitidas pertenecen a la fase D diferida: **no** se miden en este Bolt.

### 9.6 La corrección del guard, verificada sobre el árbol con 9 proyectos

```
dotnet test … --filter FullyQualifiedName~Every_project_of_the_tree_has_a_versioned_lock_file
  Passed! - Failed: 0, Passed: 1, Skipped: 0, Total: 1        ← con 9 proyectos en el árbol

Proyectos del árbol 2.0: 9   ·   sin packages.lock.json: 0
  src/Cooperator.Framework.{Core,Data,Library,Modeling}
  tests/Cooperator.Framework.Characterization.Tests · tests/Cooperator.Framework.Modeling.Tests · tests/Cooperator.Harness.Tests
  tests/fixtures/api-compat-fixtures/{BaselineFixture,BrokenFixture}
```

### 9.7 Gates (§9)

| Gate | Level | Resultado | Evidencia |
|------|-------|-----------|-----------|
| Build / lint / type checks | V-Bounce | **pass** | 14 proyectos sin advertencias; el paquete en `netstandard2.1`; la directiva de §A.3 sólo en los 8 portados; **0** `SYSLIB0051` (§9.2, §9.3) |
| Unit / component tests | V-Bounce | **pass** | 165/165 verdes, incluida la caracterización antes/después, **corriendo sobre el árbol con el proyecto de pruebas creado (9 proyectos) y el guard corregido** (§9.4, §9.6) |
| Consumer-side contract tests (test doubles) | V-Bounce | **pass** | `ModelCharacterizationTests.The_engine_consumes_the_portable_contract_through_a_test_double`: una factory + provider stub devuelven un esquema conocido y el motor lo procesa sin conocer implementación alguna (§9.4) |
| SAST | V-Bounce | **pass** | 0 hallazgos: el porteo no introduce código de terceros ni superficie expuesta; el paquete no lee entrada externa |
| Dependency scanning / licenses | V-Bounce | **pass** | `Modeling`: exactamente `System.Text.Json` con versión centralizada; `Core`/`Library` con 0 (invariantes del arnés, verdes); dependencias nuevas: **ninguna** (§9.5) |
| OWASP Top 10 — static review | V-Bounce | **n/a (razón)** | El paquete no tiene superficie expuesta: no lee entrada externa ni abre conexiones; su entrada son datos ya validados por el consumidor |
| Prompt-injection scan | V-Bounce | **n/a (razón)** | El entregable no interpreta contenido textual como instrucciones; los comentarios XML portados del 1.4 se conservan como documentación |
| Secret-leak scan | V-Bounce | **pass** | 0 secretos: la cadena de conexión **no** es un dato del modelo (y el retiro del `using` del provider y de la firma con `connectionString` lo confirman, §9.4) |
| Hallucination lint | V-Bounce | **pass** | Cada tipo reubicado tiene su origen verificable en `1.4/` (L35/L834/L1056/L1621 y las líneas de los 14 métodos) y su destino declarado en `MIGRATION-PARTITION.md`; 0 tipos inventados |
| IP / license provenance | V-Bounce | **pass** | Código propio con el encabezado BSD del 1.4 conservado en los archivos portados; ninguna dependencia nueva más allá de la admitida |
| PII / DLP | V-Bounce | **pass** | `data_classification: internal`; el modelo es estructura —no contiene filas, datos de negocio, datos personales ni credenciales— |
| Dependency-confusion | V-Bounce | **pass** | `Modeling`: una sola `PackageReference` con ID calificado y versión centralizada, y ningún `ProjectReference`; el proyecto del tooling pertenece a la fase D |
| Test-first evidence | V-Bounce | **pass** | La caracterización corre **verde antes** de mover los tipos y otra vez después, sin ajustar expectativas (AC-3); las pruebas nuevas cubren los tipos movidos y la partición |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **pass** | Dos corridas del comando reproducible con el mismo resultado; el orden de la tabla del mapeo es estable y determinista (§9.4) |
| Bolt-manifest validation | V-Bounce | **pass** | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa (§9.8) |
| Integration tests (real dependencies) | Integration test | **n/a (razón)** | Plan de integración `n/a` (Bolt `not-required`); no hay corrida pendiente antes de la aceptación |
| Contract / E2E against the real providers | Integration test | **n/a (razón)** | El único contrato verificado es el de ensamblados (hermético); el del formato es del Bolt 004 |
| DAST / OWASP Top 10 — dynamic | Integration test | **n/a (razón)** | Sin superficie desplegada ni servicio |
| Perf-smoke (p95/p99) | Integration test | **n/a (razón)** | Sin superficie desplegada; la partición no cambia ningún camino de ejecución medible |
| Observability (logs / metrics / traces) | Integration test | **n/a (razón)** | La biblioteca no agrega telemetría |

### 9.8 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json`: se agregó la entrada **`v_bounces[3]`**
  (`number: 4`, `spec_revision: 4`, `execution_outcome: ready_for_review`, `code_generation`, `mem.ref`), validada
  contra `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce (MEM, fila T10 del Bolt, fragmento de constancia, `MIGRATION-PARTITION.md`, la
  suite y los archivos del paquete) se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23).

### 9.9 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

---

## 10. Manual interventions

**Ninguna intervención humana sobre el código.** El paquete se generó con el generador determinista (que valida sus
anclas contra la fuente congelada antes de recortar nada), la corrección del guard es la línea autorizada por el
SPEC, y la suite y las constancias se escribieron en este V-Bounce. **Tampoco hubo operaciones de git** (G34): el
árbol está sin commitear y el commit es del humano. Los únicos actos del agente son los del contrato del V-Bounce:
la toma del Bolt (T10), la generación, las mediciones, la corrección autorizada, el cierre y la pausa.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol de trabajo es la entrega y no se hicieron operaciones de git.
- **SPEC ejecutado:** [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md) — **revisión 4**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T01:35:00-03:00`), cuyo §6/A.4 autoriza la corrección de una línea.
- **Bolt:** [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — `Status timeline`,
  fila **T10**.
- **V-Bounces anteriores:** [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md) (N°1: los ocho sitios del
  modelo hacia el helper), [`MEM-261016-0107`](MEM-261016-0107-particion-modeling.md) (N°2: las fronteras del
  helper, los 28 diagnósticos de nulabilidad y el bloqueo de la fase D) y
  [`MEM-261016-0125`](MEM-261016-0125-particion-modeling.md) (N°3: la medición del guard y la propuesta que la
  revisión 4 implementó).
- **Manifiesto:** [US-000.BOLT-003-particion-modeling.json](../metrics/bolts/US-000.BOLT-003-particion-modeling.json)
  — `v_bounces[3]`.
- **La constancia entregada:** `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` y
  `2.0/docs/api-removals/US-000.BOLT-003.json`.
- **La suite entregada:** `2.0/tests/Cooperator.Framework.Modeling.Tests/` (5 archivos de prueba + `Support/Repo.cs`).
- **La corrección del Bolt 001:** `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` L198 (§8.1).
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs`
  (2672 líneas: L35 `using …Providers.MSSQL`, helper L76–1056, mitad de árbol L834–1056, mitad reverse L76–833,
  acoplamiento al editor en L1621, los ocho sitios del modelo), `DBReverseHelper/DBReverse.cs` (660),
  `DBReverseHelper/DBReverseHelper.cs` (330), `DBReverseHelper/Providers/MSSQLCommon.cs` (260, de donde se extrajo
  la tabla del mapeo), `Exceptions/*.cs`.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 4 (de US-000.BOLT-003) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0004 revisión **4** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~9 min de reloj (01:42:41 → 01:51:33): toma, porteo, corrección del guard, suite nueva, corridas y cierre |
| Archivos creados | **18** (9 fuentes del paquete + `MIGRATION-PARTITION.md` + la constancia + el proyecto de pruebas con sus 5 casos y su `Support` + el lock file) |
| Archivos modificados | **3** (el Bolt, la línea del guard del Bolt 001 y la solución) |
| Tests creados | **23** (2 de dependencias + 6 del mapeo + 4 de POCO + 4 de la partición + 6 de caracterización y contrato) |
| Tests de la suite | **165** herméticos (59 + 83 + 23), todos verdes |
| Rupturas declaradas | 9 (especificadas en `MIGRATION-PARTITION.md` §4 y en la constancia) |
| Obligaciones registradas | 1 (la fase D, con dueño 014/016 y referencia cruzada) |
| Líneas de la corrección del Bolt 001 | **1** (+ su comentario de procedencia) |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |
| Código generado por IA | 100 % de lo generado en este V-Bounce |
| Desviaciones del SPEC | 0 de alcance; 2 hallazgos declarados (§8.3 sobre un artefacto ajeno y §8.4 incidente propio, corregido) |

---

## 13. Pending items and stubs

1. **La obligación registrada de la fase D** (lo único diferido por decisión): la creación de
   `2.0/src/CooperatorModeler/` y la reubicación de sus cuatro destinos (§2.4, `MIGRATION-PARTITION.md` §2.2/§5).
   Dueños: **Bolts 014 y 016** (al aportar la capa `Data` y reescribir el reverse); mientras `Data.BaseRule` no
   exista, la obligación no se puede cumplir y su incumplimiento no es un defecto de este Bolt. Sus **tres
   recordatorios** están declarados con su insumo verificado.
2. **El hallazgo de §8.3** — la sobre-restricción latente en el control de rastro del Bolt 006 (su cláusula exige
   que las únicas menciones documentales del proveedor de plataforma estén en su propio fragmento). No es mío y no
   lo corrijo: **pasa hoy** y su dueño debe decidir si la relaja a «sólo en fragmentos de constancia».
3. **La brecha de porteo del núcleo sigue abierta y declarada** (no es de este Bolt): `Modeling` entrega el
   subgrafo portable; el resto de `Core`, `Data` y `Library` lo aportan sus Bolts. El reporte de dependencias
   por ensamblado del árbol lo mide en cada corrida.
4. **Insumo para las notas de migración (Bolt 028):** la lista de rupturas vive en
   `MIGRATION-PARTITION.md` §4 y en `2.0/docs/api-removals/US-000.BOLT-003.json`; si el Bolt 028 fija otro
   contenedor, mover el fragmento es un acto mecánico.
5. **Caveat heredado y declarado (no introducido acá):** el caché estático de namespaces de la mitad de árbol del
   helper (`_NamespaceCollectionDictionary` + `GetNamespaceCollection`/`ResetNamespaceCollection`) es preexistente
   del 1.4; viaja tal cual porque cambiarlo alteraría el comportamiento que la paridad mide (§B.2). La suite lo
   aísla reseteándolo, y su caveat de concurrencia queda **heredado**, no introducido por esta partición.
6. **Fuera de alcance por decisión:** el esquema del `.coop2` y su lector/escritor (Bolt 004), el retiro de
   `[Serializable]`/`SerializationInfo` del resto de la superficie (Bolt 007), el motor de generación (Bolts
   017–021) y el shell del Modeler (Bolts 025–027).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[3]` del manifiesto + el árbol entregado (el paquete portable con
  sus 9 fuentes, las dos constancias, el 9.º proyecto con su suite y su lock file, la corrección declarada de una
  línea del guard del Bolt 001 y el alta del proyecto en la solución).
- **Qué se pide decidir:** la aceptación del entregable y —si se considera necesario— el hallazgo de §8.3 sobre el
  artefacto del Bolt 006.
