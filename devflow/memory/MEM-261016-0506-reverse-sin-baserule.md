---
id: "MEM-261016-0506"
title: "El reverse sin `BaseRule` y la materialización del tooling (SPEC rev.3) - V-Bounce N°1: el tooling creado con el acceso propio, la cadena del contrato y la mitad reverse; BLOQUEADO en el cuarto destino de `OBLIGATION-TOOLING-01` (los `PropertyEditors` frente al analizador WinForms)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-016" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0027" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 3 # revision of the canonical SPEC used by this V-Bounce (rev.3: turn_budget 25)
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-005", "ADR-001", "ADR-004"] # ADR-005 §3.6 (el reverse deja de heredar y vive en el tooling) y §3.1 (provider único); ADR-001 §3.1 (el tooling junto al shell y al motor, el modelo sin aristas hacia Data) y §3.6 (la fuente única del mapeo); ADR-004 §3.2/§3.3 (la cadena desde el contrato y el resolutor invocado por el consumidor)
manifest: "US-000.BOLT-016-reverse-sin-baserule.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T05:06:37-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0). El ejecutor NO se auto-aprueba.
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T05:11:03-03:00"
  decided_at: "2026-10-16T05:11:03-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `blocked`, SPEC `SPEC-261016-0027` revisión 3 gobernante (`turn_budget: 25`), baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — corrida hermética VERDE reproducida (`pwsh 2.0/build/build.ps1 -Scope Hermetic`): 24 proyectos, 0 advertencias, 135 arnés + 12 mapeo + 11 encriptor + 17 tooling + 46 data + 83 caracterización = 304 casos, exit 0. Los `PropertyEditors` están retirados del árbol (0 archivos `*Editor*` en el componente) — coherente con el MEM §5/§8.1."
    - "Compliant — entregables verificados por lectura: `CooperatorModeler.csproj` (`net10.0-windows`, `ProjectReference` Core+Modeling, **0** a Data, `PackageReference Microsoft.Data.SqlClient` sin versión); **0** `BaseRule`/`DataAccess`/`Cooperator.Framework.Data`/`System.Data.SqlClient` en el componente; **4** llamadas cruzadas calificadas `Cooperator.Framework.Modeling.Model.DomainTreeHelper.`; `SqlClientSchemaReader` (4 rutas + `dbo.`); `ConnectionStringResolver` (prefijo obligatorio + resolutor por consumidor + error nombrando el prefijo); lane de integración (4 clases + `SchemaProvisioning.sql`). `1.4/**` intacto."
    - "ADJUDICACIÓN (bloqueo §8.1) — ENDOSO: el `WFO1000` del analizador WinForms + `TreatWarningsAsErrors` convierte en error las propiedades del form portado; las dos únicas salidas —anotar el código aportado (prohibido por §D.3-a) o silenciar el analizador (excepción de gate: G21 exige ADR, **nunca** decisión del ejecutor)— están fuera de autoridad; **G21 bien citado**; el aviso del propio 003 (`MEM-261016-0046` §13: «el camino es el retiro por escalamiento») se activa correctamente."
    - "ADJUDICACIÓN (dirección §13.1) — **(a) RETIRAR los `PropertyEditors`**, con fundamento: la partición (003) ya retiró los atributos de diseñador del modelo (ADR-001 §3.1, ADR-009 §3.2) y el acoplamiento modelo→editor (`DomainTree.cs` L1621); en 2.0 la edición es **dirigida por contrato**; **nada** los consume (no tienen vehículo); retirarlos cierra la obligación como **3 destinos + retiro declarado**, coherente con la anticipación del 003 («retiro diferido a la UI nueva 025»). (b) exigiría un ADR/excepción (G21) para sostener UI muerta — ceremonia innecesaria."
    - "ADJUDICACIÓN (§8.4) — la lista de aplicaciones es el **mecanismo propio del arnés** (extensible por diseño: su comentario dice «La lista es EXPLÍCITA para que un proyecto nuevo tenga que declararse acá»); agregar `CooperatorModeler` es el uso previsto, **no** una edición que necesite legitimización con procedencia (ninguna aserción cambió: el conteo usa `applications.Length`). ADJUDICACIÓN (§8.5) — la ruptura de superficie (`UpdateDomainTree`/`GetDomainTree` cambian de firma; la cadena ya no entra por parámetro) queda declarada en el MEM como **insumo del 028**, coherente."
    - "Compliant — §8.3 nulabilidad (directiva sólo en los 6 aportados con diagnóstico, familia `CS8765`/`CS8622` incluida; 0 en código nuevo; 0 `<Nullable>` en `.csproj`; tras el retiro quedan 2 en el árbol, coherente); 25 loops (verde en 23, cierre por **ALCANCE** no presupuesto). Transparencia: la obligación `BOLT003-09` sigue `deferred` en la constancia — la parcialidad consta en el MEM §9.2 y la actualizará la rev4 + N°2."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0506 — El reverse sin `BaseRule` y la materialización del tooling (rev.3) — V-Bounce N°1 **bloqueado**

| Field | Value |
|-------|-------|
| **Bolt** | [US-000.BOLT-016](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) — `non-functional`, `refactor`, `medium`, `internal`, `test_integration: required` |
| **SPEC** | [SPEC-261016-0027](../spec/SPEC-261016-0027-reverse-sin-baserule.md), **revisión 3** (`approved` 2026-10-16T04:55:04-03:00, `turn_budget: 25`) |
| **V-Bounce** | **1** — el primero del Bolt |
| **Bloqueo** | El **cuarto destino** de `OBLIGATION-TOOLING-01` (los `PropertyEditors/*`): el analizador WinForms moderno (`WFO1000`) y la regla de la excepción de gate (G21) |
| **Baseline** | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — sin cambios materiales (§8.7) |

---

## 1. Executive summary

El V-Bounce **crea el tooling y le retira al reverse la herencia de la capa de datos**, y lo hace con la suite
**VERDE** (24 proyectos, 0 advertencias, **304 casos**: 135 arnés · 12 mapeo · 11 encriptor · **17 tooling** · 46
datos · 83 caracterización) y con la **primera lane de integración del reverse** escrita y compilada (§8.2 del SPEC).
Queda **bloqueado** en **uno** de los cuatro destinos de la obligación vencida del Bolt 003, y ese bloqueo está
medido, acotado y escalado (§8.1).

**Lo entregado.**

1. **El proyecto del tooling** (`2.0/src/CooperatorModeler`, `net10.0-windows`, sin shell) con **dos únicas
   referencias**: `Modeling` (el modelo y el contrato portable del reverse) y `Core` (el contrato de configuración).
   **No** referencia la capa de datos: la arista que la herencia necesitaba **desaparece** (AC-1/AC-7).
2. **La herencia retirada:** la clase privada `DataAccess : Cooperator.Framework.Data.BaseRule` **ya no existe** —
   con ella se van `DataBaseHelper`, `SetConnectionStringValue` y `FillDataSetBySQLText`— y las **6 ejecuciones** de
   esquema pasan por el **lector propio** ✔ (0 `BaseRule`, 0 `Cooperator.Framework.Data`, 0 `System.Data.SqlClient`
   en el componente; medido por las comprobaciones estáticas y por el **ensamblado**).
3. **El lector propio** `SqlClientSchemaReader`: `Microsoft.Data.SqlClient`, **conexión propiedad del componente**
   (creada y dispuesta en la misma lectura, sin aceptar conexiones del llamador) y las **cuatro rutas de lectura del
   1.4 intactas** (`exec sp_help`, `INFORMATION_SCHEMA.TABLES`, `EXEC sp_help`, `EXEC sp_fkeys`, con el tratamiento
   `dbo.`).
4. **La cadena desde el contrato de configuración portable**: `ConnectionStringResolver` lee
   `Cooperator:ConnectionStrings:<nombre>` por el contrato (Bolt 011), lo interpreta con `ConnectionStringValue`
   (prefijo **obligatorio**) y **el consumidor** invoca al resolutor; un esquema sin resolutor **falla** nombrando el
   prefijo ofensor —nunca se lee a ciegas— (AC-4).
5. **La mitad reverse de `DomainTreeHelper`** reubicada, con sus **cuatro llamadas cruzadas calificadas** como
   `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X` (el tooling tiene una clase **homónima**: la calificación
   es lo único que resuelve el nombre) y sus dos puntos de entrada tomando la cadena del contrato.
6. **La constancia de la obligación** (aquí, §9.2) y los **tres recordatorios** cumplidos y medidos (§9.3).
7. **La lane de integración** (`CooperatorModeler.Reverse.Integration.Tests`): los cuatro archivos del §8.2 del SPEC
   más `SchemaProvisioning.sql` (el **insumo declarado de las expectativas**: tablas con tipos variados, PK simple y
   compuesta **en orden no alfabético**, FK, identidad, `rowversion`, columna anulable y una vista), **compilada**;
   su corrida pertenece al ciclo.

**Lo bloqueado.** Los **`PropertyEditors/*`** (el cuarto destino): portados **verbatim**, el analizador WinForms
moderno emite `WFO1000` sobre las propiedades del form y el árbol lo trata como **error**; las dos únicas salidas
—transformar el código aportado (prohibido por §D.3-a) o una **excepción de gate** (G21: exige ADR aprobado,
**nunca** una decisión del ejecutor)— son del **dueño**. Por eso el resultado es **`blocked`** y el cuarto destino
queda registrado con su causa exacta y sus dos opciones (§8.1, §13.1).

---

## 2. Implemented phases

| Fase de §6 | Estado | Evidencia |
|---|---|---|
| **A — El inventario y el retiro de la herencia** | **implementada** ✔ | §9.1 (0 `BaseRule`/`DataAccess`/`DataBaseHelper`/`FillDataSetBySQLText`; el `csproj` sin la referencia a la capa de datos; el ensamblado sin la arista) |
| **B — El acceso propio a la base** | **implementada** ✔ | `Providers/SqlClientSchemaReader.cs` + los 6 sitios de lectura por el lector; las 4 consultas intactas (§9.4) |
| **C — La cadena desde la configuración portable** | **implementada** ✔ | `ConnectionStringResolver` + los dos puntos de entrada del helper; `ConnectionResolutionTests` (8 casos con dobles) ✔ |
| **D — Ubicación, dependencias y fuente única del mapeo** | **parcial** ⚠ | D.1 ✔ (el tooling, sin arista hacia `Data`), D.2 ✔ (el conversor lo devuelve `Modeling`; 0 segunda tabla), **D.3-a** ✔ (la directiva por archivo aportado **con diagnóstico**, probada por el build), **D.3-b: 3 de 4 destinos** ⚠ (los `PropertyEditors` bloqueados, §8.1) |
| **E — La paridad observable** | **implementada** (escrita y compilada) ✔ | Los 4 archivos de la lane + `SchemaProvisioning.sql`; la corrida es del ciclo |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/CooperatorModeler/CooperatorModeler.csproj` | El proyecto del tooling (`net10.0-windows`, referencias `Core` + `Modeling` y el provider único). |
| `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs` | Aportado del 1.4: `MSSQLReverseFactory` + `MSSQLProvider`, **sin** `DataAccess : BaseRule`, leyendo por el lector propio. |
| `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLCommon.cs` | Aportado del 1.4: sólo `SP_HelpTablesEnum` (la tabla del mapeo vive en `Modeling`). |
| `2.0/src/CooperatorModeler/DBReverseHelper/Providers/SqlClientSchemaReader.cs` | **Código nuevo**: el lector de esquema propio (conexión del componente, provider único). |
| `2.0/src/CooperatorModeler/DBReverseHelper/DomainTreeHelper.cs` | Aportado del 1.4: la **mitad reverse** (`UpdateDomainTree`, `GetDomainTree`, `ADDNodeFromDBObject`) con las cuatro llamadas cruzadas calificadas. |
| `2.0/src/CooperatorModeler/DBReverseHelper/ConnectionStringResolver.cs` | **Código nuevo**: la cadena desde el contrato de configuración, con el resolutor invocado por este consumidor. |
| `2.0/tests/CooperatorModeler.Reverse.Tests/*` (5 archivos) | La suite hermética del componente (17 casos). |
| `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/*` (6 archivos) | La lane de integración del reverse: 4 clases + la base + `SchemaProvisioning.sql`. |

---

## 4. Files modified

| Path | Cambio |
|---|---|
| `2.0/Cooperator2.sln` | Alta de los tres proyectos nuevos (el `.sln` conserva su BOM: formato del tooling). |
| `2.0/build/build.ps1` | Suite `tooling-hermetic` y lane `tooling-integration` (patrón de los precedentes). |
| `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` | La lista **explícita** de aplicaciones no portables suma `CooperatorModeler` — es el mecanismo que el propio arnés exige ("para que un proyecto nuevo tenga que declararse acá") y mantiene la igualdad del conteo (§8.4). |
| `devflow/functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md` | G11: toma desde `specified` (T04) y cierre `blocked` (T05). |
| `devflow/metrics/bolts/US-000.BOLT-016-reverse-sin-baserule.json` | `v_bounces[0]` (`number: 1`, `spec_revision: 3`, `blocked`). |

---

## 5. Files deleted / 6. Files renamed

Ninguno **del árbol 1.4** ni de otro Bolt. Los nueve archivos de `PropertyEditors/*` que se habían portado
verbatim se **retiraron del árbol** al quedar bloqueado su destino (§8.1): se declaran como **no entregados**, con
su causa medida, en lugar de dejar un proyecto que no compila.

---

## 7. Implementation decisions

### 7.1 El aporte se hizo por copia transformada con verificaciones contadas

Los archivos aportados del 1.4 se copiaron **verbatim** y se transformaron con sustituciones **contadas y
verificadas** (la lección de los Bolts 013/014): la herencia (`new DataAccess(...)` → `new SqlClientSchemaReader(…)`,
**6 sitios**), el `using` del provider (`System.Data.SqlClient` → `Microsoft.Data.SqlClient`, 1), el namespace
(`Cooperator.Framework.Utility.DBReverseHelper*` → `Cooperator.Framework.Modeler.DBReverseHelper*`), las **3**
referencias a `Cooperator.Framework.Utility.Exceptions` (`NodeAlreadyExistException`) al namespace de `Modeling`, y
las **4** llamadas cruzadas calificadas. El texto de las consultas de esquema **no se tocó** (§9.4).

### 7.2 El lector abre y dispone su conexión en cada lectura

`SqlClientSchemaReader` es **stateless** y no es `IDisposable`: cada `ExecuteSQLText` crea, abre, usa y **dispone**
su `SqlConnection`, y su API **sólo** recibe la cadena ya resuelta —no hay forma de pasarle una conexión del
llamador—. Eso hace el ciclo de vida verificable de forma directa (AC-3) y deja el hazard equivalente donde
corresponde: en la capa de datos (Bolt 014).

### 7.3 La cadena entra por el contrato, y el resolutor lo invoca este consumidor

`ConnectionStringResolver` no lee archivo, ni entorno, ni inspecciona el valor: pide la clave del catálogo al
contrato, la interpreta con `ConnectionStringValue` (prefijo **obligatorio**; un prefijo sin resolutor **falla**
nombrando el ofensor) y llama a `Resolve` —con la sobrecarga que acepta un resolutor registrado por el host—. Los
dos puntos de entrada públicos de la mitad reverse (`UpdateDomainTree(BaseTreeNode)`,
`GetDomainTree(BaseTreeNode)`) toman la cadena **de ahí**; las sobrecargas que la reciben por parámetro quedaron
**privadas** (son implementación): esa es la ruptura de superficie de este Bolt, registrada en §8.5 (ADR-008 §3.1).

### 7.4 Los editores quedan fuera hasta que el dueño decida el mecanismo

No se "arregló" el form portado (agregar `[DesignerSerializationVisibility]` es **transformar el código aportado**,
lo que §D.3-a descarta explícitamente) ni se silenció el analizador (es una **excepción de gate**: G21 exige ADR).
Se retiraron del árbol para no dejar un proyecto que no compila, y el destino queda como obligación con su causa y
sus opciones (§13.1). El `csproj` **no** declara `UseWindowsForms` —nada del componente entregado tiene UI— y lo
recuperará el V-Bounce que materialice los editores.

---

## 8. Deviations and assumptions

### 8.1 El cuarto destino de `OBLIGATION-TOOLING-01` no se pudo materializar (el bloqueo, medido)

**Qué pasa.** Los `PropertyEditors/*` se portaron verbatim (namespace ajustado, recursos incluidos). El build del
tooling —con `UseWindowsForms=true` y el árbol bajo `TreatWarningsAsErrors=true`— emite **`WFO1000`** sobre el form
portado:

```
ToStringInfoEditorForm.cs(19,21): error WFO1000: Property 'OverrideToString' does not configure the code
                                   serialization for its property content
ToStringInfoEditorForm.cs(31,23): error WFO1000: Property 'StringFormat' does not configure the code
                                   serialization for its property content
```

**Por qué no lo resuelvo yo.** Las dos salidas posibles quedan fuera de mi autoridad:

1. **Anotar el código aportado** (`[DesignerSerializationVisibility]`): es exactamente la transformación que
   **§D.3-a descarta** ("anotar o «corregir» el código aportado … es una transformación que cambia el código del 1.4
   sin decisión que la ordene").
2. **Silenciar el analizador** (`NoWarn`/retiro del analizador): es una **excepción de gate** y §15 lo dice sin
   ambigüedad —"si la única salida fuera una excepción de gate, exige un ADR aprobado por `AITL-ADR-Approval`
   (G21)— **nunca** una decisión del ejecutor".

Además, el propio Bolt 003 dejó **avisado** este riesgo al diferir la fase D: *"la compilación de `PropertyEditors/*`
en `net10.0-windows` (`System.Design` / `IWindowsFormsEditorService`), con su nota de control del finding §14(b) — el
camino es el **retiro por escalamiento** vía condición de parada, **nunca** un shim de compatibilidad"*
(`MEM-261016-0046` §13). Es la condición de parada que este V-Bounce activa.

**Contexto que el dueño necesita para decidir** (medido en este V-Bounce): la partición **retiró** los atributos de
diseñador del modelo (ADR-001 §3.1, ADR-009 §3.2) y **eliminó** el acoplamiento modelo→editor
(`DomainTree.cs` L1621 en 1.4); en la 2.0 la edición es **dirigida por el esquema del contrato**. Los editores
portados, por lo tanto, son alcanzables sólo si algo les vuelve a dar un vehículo —es decir: su materialización es
una decisión de **producto**, no un detalle de build—.

**Las dos opciones para el dueño (§13.1):** (a) **retirarlos** de la obligación con una revisión del SPEC/ADR que lo
ordene (la edición de la 2.0 no los usa: la partición ya retiró los atributos que los invocaban), cerrando la
obligación como 3 destinos + retiro declarado; o (b) **autorizar el mecanismo** que los haga compilar sin cambiar su
semántica —una exención de gate por ADR (G21), o una anotación autorizada por revisión material— y ejecutarlos en el
V-Bounce siguiente.

### 8.2 El SPEC asumía el tooling creado; no lo estaba, y la obligación manda materializarlo

§2 del SPEC dice que el reverse "**ya está relocalizado en el tooling**" por el Bolt 003, y su §5 lista el
`2.0/src/CooperatorModeler/` como entregado. **No existe**: la constancia del 003 lo dice con su propia evidencia
—"`2.0/src/CooperatorModeler/` no existe (verificado)" (`US-000.BOLT-003.json`, `BOLT003-09`, `OBLIGATION-TOOLING-01`)—
y este V-Bounce lo midió antes de escribir. La lectura ejecutada es la de **§D.3-b y la tarea del Bolt**: la
obligación se materializa **acá** (la capa `Data` ya está portada, que era su condición), con sus tres
recordatorios. Se registra porque el revisor debe verlo: la premisa de §2 estaba **desactualizada respecto de la
constancia del propio Bolt 003**, y la evidencia del 003 es la que manda.

### 8.3 El `#nullable disable` se aplicó por archivo y con el build como prueba

No se puso ninguna directiva "por las dudas": se compiló **sin** directivas, se relevaron los diagnósticos de
nulabilidad por archivo y se aplicó la directiva **sólo** a los **6** archivos aportados del 1.4 que los producían
(`MSSQLProvider.cs`, `DomainTreeHelper.cs`, `DropDownListEditor.cs`, `StringEditor.cs`, `ToStringInfoEditor.cs`,
`ToStringInfoEditorForm.cs`). La familia incluyó `CS8765`/`CS8622` (nulabilidad de miembros sobrescritos), del mismo
criterio objetivo —*archivo aportado + diagnóstico de nulabilidad*—. El **código nuevo** (`SqlClientSchemaReader`,
`ConnectionStringResolver`) y las **pruebas** conservan la nulabilidad **activa**, y **ningún** `.csproj` declara
`<Nullable>disable</Nullable>`. Los archivos aportados que **no** produjeron diagnóstico **no** la llevan.

### 8.4 Se tocó una línea del arnés (el mecanismo que el propio arnés exige)

`BuildInvariantsTests.cs` declara las aplicaciones no portables en una lista **explícita** —con el comentario que
explica por qué: *"La lista es EXPLÍCITA para que un proyecto nuevo tenga que declararse acá y no pase por
descuidado"*—. El tooling es la segunda aplicación (tras `ConnectionStringEncriptor`): se agregó a esa lista, que es
la forma prevista por el propio archivo. No se cambió ninguna aserción ni ningún otro archivo del Bolt 001.

### 8.5 La ruptura de superficie del tooling (AC-8)

La superficie pública del componente cambia: `DomainTreeHelper.UpdateDomainTree(string, BaseTreeNode)` y
`GetDomainTree(string, BaseTreeNode)` dejan de ser públicos —la cadena **no** entra por un parámetro de un
formulario, entra por el contrato— y en su lugar quedan `UpdateDomainTree(BaseTreeNode)` y
`GetDomainTree(BaseTreeNode)`, más los tipos nuevos `SqlClientSchemaReader` y `ConnectionStringResolver`. Es una
**adición con reemplazo**, y se declara para las notas de migración (ADR-008 §3.1) junto con el cambio de
comportamiento que el consumidor percibe: **la conexión ya no se provee por un mecanismo propio del tool, sino por
la configuración portable** (dato que el Bolt 025 —la UI— necesita). Queda en este MEM como insumo del Bolt 028 en
lugar de abrir un fragmento del registro, porque el SPEC de este Bolt no lo lista entre sus entregables.

### 8.6 Presupuesto de turnos (declarado)

El SPEC fija **`turn_budget: 25`**. El V-Bounce llegó al **verde** en el turno **23** y consumió **dos más** para
cerrar (la constancia y la entrada del manifiesto): **25** en total. La causa está medida: el reconocimiento (el
SPEC de 900+ líneas, el blueprint de la partición y el código del 1.4 a portar), el aporte con su ciclo
`diagnóstico → directiva → build` (§D.3-a, que el propio SPEC anticipó que consume turnos) y las tres rondas de
compilación del andamiaje de pruebas. **La condición de parada del presupuesto no se activó**: la unidad hermética
está **verde** y completa. El Bolt cierra `blocked` por el **alcance** del cuarto destino (§8.1), no por el
presupuesto.

### 8.7 G15 — verificación de fuentes gobernadas (sin cambio material)

SPEC-261016-0027 `approved` con **`revision: 3`**, `turn_budget: 25` y `review.decision: approved`
(2026-10-16T04:55:04-03:00); el Bolt estaba `specified` y se tomó (G11, T04); ADR-004/ADR-001/ADR-005 siguen
`accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` queda intacto. La premisa
desactualizada de §2 se registra en §8.2: **no** es un cambio de fuente, es una diferencia entre el SPEC y la
constancia del Bolt 003 que la obligación ya había declarado.

### 8.8 Supuestos declarados

- El nombre de la clase del tooling es homónimo de la portable (`DomainTreeHelper`) **a propósito**: lo fija la
  partición (dos tipos con el mismo nombre en dos ensamblados) y por eso las llamadas cruzadas van calificadas.
- La lane de integración lee la cadena del **entorno** publicándola en la configuración portable; el nombre de la
  base de prueba y su esquema (`cooperator_reverse_test`) los fija `SchemaProvisioning.sql`.
- La paridad del **árbol** se verifica en dos niveles: las entidades por nombre (recorriendo el árbol que produce
  `GetDomainTree`) y el resto de la estructura sobre el **esquema leído** (`DataBase`: campos, PK simple y compuesta
  **con su orden**, identidad, `rowversion`, anulable, FK con su tabla destino y vistas). El detalle por propiedad
  del árbol queda para la corrida del ciclo.

---

## 9. Verification evidence

### 9.1 La herencia está retirada (AC-1, AC-7)

`DependencyChecks.The_reverse_no_longer_inherits_from_the_data_layer` y
`The_assembly_has_no_edge_to_the_data_layer`: **0** `BaseRule`, **0** `DataAccess`, **0** `DataBaseHelper`, **0**
`FillDataSetBySQLText`, **0** `Cooperator.Framework.Data` en las cinco fuentes del componente y en el `csproj`; el
**ensamblado** referencia `Cooperator.Framework.Modeling` y `Microsoft.Data.SqlClient`, y **ninguna** arista hacia
la capa de datos (el ensamblado se inspecciona con `GetReferencedAssemblies`).

### 9.2 La constancia de la materialización de `OBLIGATION-TOOLING-01`

- **Referencia:** `2.0/docs/api-removals/US-000.BOLT-003.json` → `entries[]` `BOLT003-09`
  (`kind: deferred-obligation`, `status: deferred`, `materialized_by: US-000.BOLT-014`) y
  `registered_obligations[]` `OBLIGATION-TOOLING-01` (`owner_bolts: [US-000.BOLT-014, US-000.BOLT-016]`,
  `materially_satisfied_by: US-000.BOLT-014`); su especificación tipo por tipo está en
  `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` §2.2 y sus recordatorios en §5.
- **Estado final que este V-Bounce consta: `partially-materialized`.**
  - **Destino 1 — `Providers/MSSQLProvider.cs` (`MSSQLReverseFactory`, `MSSQLProvider`)**: **materializado** ✔ en
    `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs`, con el conversor de tipos de `Modeling`
    (`The_type_converter_is_the_single_source_of_modeling`: el tipo devuelto por la factory vive en
    `Cooperator.Framework.Modeling.Reverse`) y **sin** la herencia de la capa de datos (que la partición dejó "tal
    como está" y que **este** Bolt retira, ADR-005 §3.6).
  - **Destino 2 — el resto de `Providers/MSSQLCommon.cs`**: **materializado** ✔ como `SP_HelpTablesEnum` en
    `…/Providers/MSSQLCommon.cs`; la tabla del mapeo **no** se duplica (vive en `Modeling`).
  - **Destino 3 — la mitad reverse de `DomainTreeHelper`** (`UpdateDomainTree`, `GetDomainTree`,
    `ADDNodeFromDBObject`): **materializada** ✔ en `…/DBReverseHelper/DomainTreeHelper.cs`, llamando a la mitad
    portable de `Modeling` (dirección permitida).
  - **Destino 4 — los `PropertyEditors/*` con sus recursos**: **NO materializado** ✗ — bloqueado por `WFO1000` y por
    la regla de la excepción de gate (§8.1). Escalado al dueño (§13.1).

### 9.3 Los tres recordatorios obligatorios (medidos)

1. **Las cuatro llamadas cruzadas, calificadas**:
   `The_four_cross_calls_of_the_reverse_half_are_qualified` cuenta **4** ocurrencias de
   `Cooperator.Framework.Modeling.Model.DomainTreeHelper.` (`CorrectGenerationNamesForRelatedFields`,
   `FindEntityInTree` ×2, `CorrectSelectedCollections`) y comprueba que el tooling **no** define su propia copia de
   esos métodos. *La ambigüedad es real y se vio en el propio build*: al escribir las pruebas, `DomainTreeHelper`
   quedó **ambiguo** entre los dos tipos homónimos hasta calificarlo.
2. **La lista de rupturas como insumo de la constancia**: `MIGRATION-PARTITION.md` **§4** (ocho rupturas
   declaradas) es el insumo; este MEM agrega la que **este** Bolt introduce (la ruptura de superficie de §8.5 y el
   cambio de comportamiento de la cadena) para las notas del Bolt 028.
3. **El reporte de dependencias a dos niveles**: **presupuesto** —`The_dependency_budget_of_the_tooling_is_the_authorized_provider_only`:
   el `csproj` declara **exactamente** `Microsoft.Data.SqlClient` sin `Version` (centralizado) y su
   `packages.lock.json` versiona esa dependencia directa— y **aristas del ensamblado** —§9.1—.

### 9.4 Las consultas de esquema y la conexión (AC-3)

`The_schema_queries_of_1_4_are_intact`: `exec sp_help`, `INFORMATION_SCHEMA.TABLES`, `EXEC sp_help`, `EXEC sp_fkeys`
y el tratamiento `dbo.` están en el texto del provider. `The_connection_is_the_components_own`: el lector usa
`new SqlConnection(_connectionString)`, la abre y la dispone en la misma lectura, y su único constructor recibe un
`string` (no acepta conexiones). AC-5: el ensamblado referencia `Microsoft.Data.SqlClient` y **ninguno** de los
proveedores retirados.

### 9.5 La suite hermética del árbol: VERDE

```
build sin advertencias (tratadas como error): 24 proyectos
Passed!  - Failed: 0, Passed: 135  - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  12  - Cooperator.Framework.Core.Mapping.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  11  - ConnectionStringEncriptor.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  17  - CooperatorModeler.Reverse.Tests.dll (net10.0)   ← la suite nueva
Passed!  - Failed: 0, Passed:  46  - Cooperator.Framework.Data.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  83  - Cooperator.Framework.Characterization.Tests.exe (net48)
Resultado: ok (codigo de salida 0)     → 304 casos
```

### 9.6 La lane de integración: escrita y compilada

`CooperatorModeler.Reverse.Integration.Tests` compila con sus cuatro clases (`SchemaParityTests`,
`ViewsAndKeysTests`, `ConnectionStringResolutionTests`, `SchemaReaderLifecycleTests`), la base del entorno y
`SchemaProvisioning.sql`. **Su corrida es del ciclo de test de integración del Bolt** (SPEC §8.2; el Bolt es
`test_integration: required`): sin esa corrida el Bolt **no** pasa a `ready-for-acceptance` (§15).

### 9.7 `1.4/**` intacto y rutas propias

Ninguna escritura tocó `1.4/**`. Escrituras de este V-Bounce: `2.0/src/CooperatorModeler/**`,
`2.0/tests/CooperatorModeler.Reverse.Tests/**`, `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/**`,
`2.0/build/build.ps1`, `2.0/Cooperator2.sln`, la lista de aplicaciones de
`2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` (§8.4), el Bolt (la toma y el cierre) y este MEM con su
entrada de manifiesto.

### 9.8 Gates (§9 del SPEC)

`pass`: Build/lint/type checks (24 proyectos, 0 advertencias; el tooling sin la referencia a la capa de datos y sin
diagnósticos de nulabilidad) · Unit/component (304 verdes) · Consumer-side contract tests (dobles de
`ICooperatorConfiguration` y del resolutor) · SAST/OWASP estático (el texto de las consultas no cambia; sin entrada
externa nueva; sólo lectura) · Secret-leak (**0** cadenas en el código, en las pruebas o en la documentación: el
sistema de pruebas usa variables de entorno y valores sintéticos) · Hallucination lint (cada pieza retirada y cada
consulta conservada trazable al 1.4 citado) · IP/license (código propio) · PII/DLP (`internal`: sólo estructura de
una base de prueba) · Dependency-confusion (0 paquetes nuevos) · Test-first evidence (las expectativas se declaran
desde `SchemaProvisioning.sql` y la tabla de mapeo del 003) · Behavioral reproducibility (dos corridas de la suite
dan lo mismo) · Bolt-manifest validation ✓. `n/a` con razón: prompt-injection (no interpreta texto como
instrucción) y las filas de integración de DAST/Perf/Observabilidad.
**`n/a` declarado con causa**: *Integration tests* — el plan es `applicable` pero su corrida es del **ciclo** (por
diseño de §8.2), y la obligación del §D.3-b queda **parcial** (§8.1): ambos son hechos que se declaran, no gates que
se cierren acá.

---

## 10. Manual interventions

Ninguna intervención humana durante este V-Bounce: el relevamiento, el aporte, la suite, la lane de integración y la
constancia los hizo el ejecutor. **Sin operaciones de git** (G34). La decisión humana que se **pide** está en §13.1.

---

## 11. Evidence links

- **SPEC ejecutado:** [SPEC-261016-0027](../spec/SPEC-261016-0027-reverse-sin-baserule.md) — revisión **3**: §4
  (alcance), §5 (prerrequisitos y la obligación), §6/A–E (incluidas §D.3-a y §D.3-b), §7 (AC-1…AC-9), §8.1/§8.2,
  §9, §14, §15.
- **La obligación y su blueprint:** `2.0/docs/api-removals/US-000.BOLT-003.json` (`BOLT003-09`,
  `OBLIGATION-TOOLING-01`) · `2.0/src/Cooperator.Framework.Modeling/MIGRATION-PARTITION.md` §2.2/§4/§5 ·
  `devflow/memory/MEM-261016-0046-particion-modeling.md` §13 (el aviso del riesgo de los editores).
- **La capa de datos (precondición cumplida):** `devflow/memory/MEM-261016-0429-provider-de-datos.md` (el Bolt 014
  cerró su Development Completed) · `MEM-261016-0449-cache-en-data.md` (la caché del 013).
- **El contrato de configuración:** `2.0/src/Cooperator.Framework.Core/Configuration/{ICooperatorConfiguration,
  ConnectionStringValue,IProtectedValueResolver,ConfigurationKeys}.cs` (Bolt 011).
- **El código entregado:** `2.0/src/CooperatorModeler/**` y las dos suites bajo `2.0/tests/CooperatorModeler.*`.

---

## 12. Metrics

| Metric | Value |
|---|---|
| V-Bounce | 1 (de US-000.BOLT-016) — **`blocked`** |
| SPEC ejecutado | SPEC-261016-0027 revisión **3** (`turn_budget: 25`) |
| Fases implementadas | **4 de 5** (A–C y E completas; **D parcial**: 3 de los 4 destinos de la obligación) |
| Archivos de producto creados / modificados | **6** creados (proyecto + 5 fuentes) / 0 de otro Bolt |
| Archivos de prueba creados | **11** (5 herméticos + 6 de la lane de integración) + `SchemaProvisioning.sql` |
| Casos de la suite nueva | **17** (herméticos) · la lane de integración: 4 clases, 10 casos, escritos y compilados |
| Suite hermética del árbol | **304 verdes** (135+12+11+17+46+83), exit 0, **24** proyectos sin advertencias |
| Loops usados | **25** (presupuesto del SPEC; el verde se alcanzó en el 23) |

---

## 13. Pending items and stubs

1. **[DECISIÓN HUMANA — el bloqueo de este V-Bounce] El cuarto destino de `OBLIGATION-TOOLING-01`.** Dos opciones,
   ambas fuera de la autoridad del ejecutor: **(a) retirar** los `PropertyEditors/*` de la obligación con una
   revisión que lo ordene (la partición ya retiró los atributos de diseñador del modelo y el acoplamiento
   modelo→editor: en la 2.0 la edición es dirigida por el contrato, así que los editores no tienen vehículo),
   cerrando la obligación como **3 destinos + retiro declarado**; o **(b) autorizar el mecanismo** —una exención de
   gate por **ADR** (G21) o la anotación del código aportado por **revisión material**— y materializarlos en el
   V-Bounce siguiente. Con (b) el `csproj` recupera `UseWindowsForms` y el tooling vuelve a `net10.0-windows` con su
   UI de edición.
2. **La corrida del ciclo de test de integración** (§8.2 del SPEC): la lane está escrita y compilada; su corrida
   —contra SQL Server real, con `SchemaProvisioning.sql` aprovisionado y la cadena por el entorno— es la que cierra
   el plan y habilita `ready-for-acceptance` (§15). Es la **primera prueba de integración del reverse**.
3. **El detalle por propiedad del árbol** en la paridad: la corrida del ciclo compara la estructura sobre el esquema
   leído (§8.8) y las entidades sobre el árbol; el tipo nativo/CLR por propiedad del árbol se completa ahí.
4. **Las notas de migración (Bolt 028):** la ruptura de superficie del tooling y el cambio de comportamiento de la
   cadena (§8.5), más la lista de rupturas de la partición (§9.3, recordatorio 2).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 3`, `blocked`) +
  el árbol entregado: el tooling creado con el acceso propio, la cadena del contrato, la mitad reverse calificada y
  sus tres recordatorios, con la suite hermética en verde (304) y la lane de integración compilada.
- **Qué se pide decidir:** (1) la **adjudicación del bloqueo** del cuarto destino (§8.1) y cuál de las dos opciones
  de §13.1 se toma; (2) la constancia de la **materialización parcial** de `OBLIGATION-TOOLING-01` (§9.2) y su
  estado final; (3) la ruptura de superficie declarada en §8.5 y su destino en las notas del Bolt 028.
