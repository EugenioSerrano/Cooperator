---
id: "MEM-261016-0107"
title: "Partición de `Cooperator.Framework.Modeling` (SPEC rev.2) — V-Bounce N°2 detenido por el tooling (`Data.BaseRule` ausente + provider de ADR-005)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-003" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0004" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008", "ADR-002"] # governing ADRs — none of them materialised a change here (V-Bounce stopped before delivering)
manifest: "US-000.BOLT-003-particion-modeling.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §11)
review_ready_at: "2026-10-16T01:07:17-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:10:17-03:00"
  decided_at: "2026-10-16T01:10:17-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible (fase D): verificado en el código que `1.4/Cooperator.Framework.Utility/DBReverseHelper/Providers/MSSQLProvider.cs:499` declara `private class DataAccess : Cooperator.Framework.Data.BaseRule` (doc-comment L497: «hereda de BaseRule para poder acceder a la base de datos»); `2.0/src` tiene **0 archivos `.cs`** (glob `2.0/src/**/*.cs` vacío); `System.Data.SqlClient` queda fuera de `net10.0-windows` sin paquete (sonda CS1069 del MEM) y su reposición es de ADR-005 §3.1 (Bolt 014, versión + smoke test real). §D.1 «conserva su acceso a datos **tal como está**» entra en conflicto con §4 (capa `Data`/Bolt 014 out of scope) y §15 («el reverse se mueve **como está**»; reescribirlo es el Bolt 016). La condición de parada citada coincide."
    - "Compliant — la corrección de la rev.2 FUNCIONA (prueba medida, no auto-declarada): el MEM N°2 §9 muestra el build del paquete portable EXIT=0 (0 errores/0 advertencias) y el **reporte de dependencias** —`Cooperator.Framework.Modeling.dll` referencia **sólo `netstandard 2.1.0.0`** (0 aristas a Data/Library/Core/tooling/UI), 1 `PackageReference` (`System.Text.Json`) y 0 `ProjectReference`—. Los **8 sitios** se resuelven dentro del paquete (verificado en la re-aprobación de la rev.2: L1226/L1635/L1838/L2187/L2191/L2206/L2215/L2222, con los 4 métodos invocados en la mitad de árbol de `Modeling`). El árbol fue revertido; el claim es medido."
    - "Compliant — hallazgo de nulabilidad correcto y escalada apropiada: verificado `2.0/Directory.Build.props` con `<Nullable>enable</Nullable>` (L19) + `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` (L21); los **28 diagnósticos** (21×CS8618 / 4×CS8603 / 3×CS8625) contra EXIT=0 con `#nullable disable`. El mecanismo es de una línea por archivo portado; el SPEC no menciona nullable (0 ocurrencias), así que es una decisión de política de build que no corresponde al ejecutor — medirla y escalarla es lo correcto."
    - "Compliant — incidente declarado (§8.3) contenido y honesto: la herramienta auxiliar con rutas relativas sobrescribió **2 archivos de su propio aporte generado** (`Reverse/DBReverse.cs`, `Reverse/DBReverseHelper.cs`); se detectó, se regeneró desde el generador determinista y el build volvió a verde. Verificado que el árbol quedó sano (2.0/src sólo stubs — glob de `.cs` vacío; suites 20/20 + 83/83; sin residuos fuera de sus rutas). La declaración honesta se documenta, no se penaliza."
    - "Compliant — cierre correcto: T06 (`blocked`→`in-development`, 01:06:51) y T07 (`in-development`→`blocked`, 01:07:17) con reloj real; MEM N°2 único (`MEM-261016-0107` referencia `US-000.BOLT-003`; el N°1 `MEM-261016-0046` queda como historia inmutable); manifiesto `v_bounces[1]` con `number: 2`, `spec_revision: 2`, `execution_outcome: blocked` y los 8 campos; `1.4/**` intacto (git diff --quiet)."
    - "Compliant — desbloqueo propuesto correcto y agéntico: revisión 3 que **difiere la fase D con obligación registrada** (patrón 011 rev.2) + fija la **política de nulabilidad** (mecanismo medido). Es una decisión de secuencia (la capa `Data` y el provider son de los Bolts 012/013/014/016 + ADR-005) y de política de build — ninguna del ejecutor. La re-aprobación del SPEC la hace el revisor en otro acto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0107 — Partición de `Modeling` (SPEC rev.2): V-Bounce N°2 detenido por el tooling

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) |
| **SPEC**        | [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md), **revisión 2** (aprobada) |
| **V-Bounce**    | **2** (el N°1 está en [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md), `blocked`) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-005](../adrs/ADR-005-provider-de-datos.md) (gobierna el artifacto bloqueante) |

> ⚠️ **V-Bounce NUEVO (no un retry):** timestamp nuevo, MEM nuevo, entrada `v_bounces[]` número 2. El N°1 sigue
> siendo historia inmutable con su propio MEM.
>
> ⚠️ **Este V-Bounce NO dejó código en el árbol.** El aporte se generó, se midió y se **revirtió**. Es obligatorio
> registrar el bloqueo (§3.3, G17) y este MEM es lo que permite desbloquear: **la partición de la revisión 2
> funciona y está probada**; lo que falta es una pieza de otro Bolt.

---

## 1. Executive summary

El V-Bounce N°2 del Bolt `US-000.BOLT-003` se detuvo **antes de entregar**, y esta vez con una diferencia
sustancial respecto del N°1: **la corrección que introdujo la revisión 2 funciona y quedó demostrada con un
build real.** Porté el paquete portable completo —el modelo como POCO (1.384 líneas), la **mitad de árbol** de
`DomainTreeHelper` (247 líneas, los 11 métodos de §B.2), el contrato del reverse (`DBReverse` + `DBReverseHelper`),
las excepciones del grafo con base `System.Exception`, y la **tabla del mapeo SQL↔CLR extraída como dato
portable** (`TypeMapping.cs`)— y el ensamblado **compila limpio: 0 errores y 0 advertencias** sobre
`netstandard2.1` con `TreatWarningsAsErrors=true`, firmado y con analizadores. Más aún: **cero errores
estructurales** — ni un solo `CS0234`/`CS0246`, es decir que cada tipo, namespace y llamada resuelve, y los
**ocho sitios** del modelo (`EntityNode.GenerateAs`/`GenerateAsChildOf`, `PropertyNode` tipo/`GenerateAsType`)
se resuelven **dentro** del paquete, que era exactamente el defecto que la revisión 2 vino a corregir. El reporte
de dependencias es la prueba dura: `Cooperator.Framework.Modeling.dll` referencia **sólo `netstandard 2.1.0.0`**
—cero aristas a `Data`, `Library`, `Core`, UI o tooling— y el proyecto no declara **ningún `ProjectReference`** y
exactamente una `PackageReference` (`System.Text.Json`, la invariante del arnés del Bolt 001). El bloqueo está
**enteramente en la fase D**: el proyecto del tooling (`2.0/src/CooperatorModeler`, `net10.0-windows`) **no puede
compilar** porque el SPEC manda reubicar `MSSQLProvider.cs` «**como está**» —y "como está" conserva
`private class DataAccess : Cooperator.Framework.Data.BaseRule`— mientras ese tipo **no existe en el árbol 2.0**:
`2.0/src` tiene **cero archivos `.cs`** en sus cuatro proyectos, el ensamblado stub de `Data` no contiene
`BaseRule` (verificado por sonda: `CS0246`), y `System.Data.SqlClient` en `net10.0-windows` exige un paquete
cuyo reemplazo es de **ADR-005 §3.1** (`System.Data.SqlClient` se retira y `Microsoft.Data.SqlClient` es el único
provider, con la **versión** y un *smoke test* de conexión real asignados al Bolt 014). Las dos salidas están
**vetadas por el propio SPEC**: §15 prohíbe «aprovechar y **reescribir el reverse** (quitarle la herencia,
cambiarle el acceso a datos)» —eso es el Bolt 016— y §4 pone la capa `Data` y su provider fuera de alcance. Se
suma un **segundo hallazgo** que afecta a las fases A–C y que el SPEC **no menciona en ninguna parte**: el árbol
hereda de la configuración compartida del Bolt 001 `Nullable=enable` + `TreatWarningsAsErrors=true`, de modo que
el código del 1.4 (previo a nullable) produce **28 diagnósticos de nulabilidad** (21× `CS8618`, 4× `CS8603`,
3× `CS8625`) que se convierten en errores; con una directiva `#nullable disable` al frente de los cuatro archivos
portados el build queda en **0 errores y 0 advertencias**, así que el mecanismo existe y es de una línea por
archivo — pero **no lo autoricé**: lo medí y lo dejo a la decisión del revisor. Los aportes se revirtieron, el
árbol se restauró (las suites siguen **20/20 y 83/83** verdes y `1.4/**` intacto) y no quedó ni un archivo basura
fuera de mis rutas.

---

## 2. Implemented phases

**Ninguna fase quedó entregada.** Se generó el aporte de las fases A, B y C, se **midió**, y se revirtió al
detectarse el bloqueo de la fase D. No existe hoy `2.0/src/Cooperator.Framework.Modeling/{Model,Reverse,Exceptions}`,
ni `2.0/src/CooperatorModeler/`, ni `2.0/tests/Cooperator.Framework.Modeling.Tests/`, ni `MIGRATION-PARTITION.md`.

### Estado medido, fase por fase

| Fase | ¿Se pudo ejecutar? | Evidencia |
|---|---|---|
| **A — proyecto portable** | ✅ **Compatible con el stub del Bolt 001 tal cual** | El `.csproj` del Bolt 001 ya tiene `netstandard2.1` + **exactamente** `System.Text.Json` + **cero** `ProjectReference` ⇒ no hizo falta tocarlo. §A.1 pedía justamente eso |
| **B — modelo POCO + partición del helper** | ✅ **PORTADO Y COMPILADO**(1) | `Model/DomainTree.cs` 1.384 líneas (modelo sin helper, sin atributos de diseñador, sin `[Serializable]`); `Model/DomainTreeHelper.cs` 247 líneas (**mitad de árbol**, L834–1056 del 1.4); los 8 sitios del modelo resueltos **dentro** del paquete |
| **C — contrato portable + tabla de mapeo** | ✅ **PORTADO Y COMPILADO**(1) | `Reverse/DBReverse.cs` (574), `Reverse/DBReverseHelper.cs` (301) tal cual + namespace; `Reverse/TypeMapping.cs` (NUEVO): la tabla de **28 tipos nativos** extraída entera del `switch` del 1.4, ordenada por nombre (ordinal) y con `MSSQLTypeConverter : IDBTypeConverter` respondiendo sobre ella |
| **D — reubicación al tooling** | ❌ **BLOQUEO** | Ver §8.1: el tooling no compila por `Data.BaseRule` ausente + `System.Data.SqlClient` sin provider |
| **E — evidencia** | 🟡 **Parcial y medida** | El **reporte de dependencias por ensamblado** de `Modeling` se produjo y es limpio (§9). Los tests, `MIGRATION-PARTITION.md` y la constancia del tooling no se escribieron: dependen de la fase D |

(1) "Compilado" con la directiva `#nullable disable` que se mide y se declara en §8.2 — **no** se dejó en el árbol.

### Reconocimiento de la partición del helper — verificada contra la fuente

La revisión 2 asigna a `Modeling` **11 métodos** (mitad de árbol) y al tooling **3** (mitad reverse). Lo verifiqué
en la fuente congelada antes de portar:

| Destino | Fronteras en `1.4/.../DBReverseHelper/DomainTree.cs` | Verificación |
|---|---|---|
| **Tooling** (3 métodos) | `UpdateDomainTree` L85–197 · `GetDomainTree` L198–311 · `ADDNodeFromDBObject` L312–832 | Es la mitad que instancia `MSSQLReverseFactory` (L201), usa la cadena de conexión y toma `DataBase`/`DBTable` |
| **`Modeling`** (11 métodos) | doc-comment L834-839 + `GetDescriptionFieldNameForEntity` L840 → final de la clase L1056 | Recorre sólo el árbol (`BaseTreeNode`/`EntityNode`/`PropertyNode`/`RelatedField`/cadenas) + el campo estático `_NamespaceCollectionDictionary` |

Y las **direcciones** del reparto se confirman en el código: la mitad reverse **llama a la portable** en
L184 (`CorrectGenerationNamesForRelatedFields`), L222/L297 (`FindEntityInTree`) y L292
(`CorrectSelectedCollections`) — dirección permitida (tooling → `Modeling`) — y **ningún** tipo del modelo llama
a la mitad reverse. En el porteo del tooling esas cuatro llamadas deben ir **calificadas** como
`Cooperator.Framework.Modeling.Model.DomainTreeHelper.X`, porque el tooling tiene una clase con el mismo nombre
(§B.2, detalle declarado).

---

## 3. Files created

| File | Purpose |
|------|---------|
| `devflow/memory/MEM-261016-0107-particion-modeling.md` | Este MEM: registro obligatorio del V-Bounce N°2 bloqueado, con la prueba de que la partición de la revisión 2 funciona |

**Archivos del aporte generados, medidos y REVERTIDOS** (declarados uno por uno, como §4 exige; el árbol no los
contiene hoy):

| Archivo (generado y revertido) | Líneas | Qué es |
|---|---|---|
| `2.0/src/Cooperator.Framework.Modeling/Model/DomainTree.cs` | 1.384 | Modelo POCO: `CollectionTypeEnum`, `BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQueryReturnType`, `DatabaseQuery` — sin atributos de diseñador ni `[Serializable]` |
| `2.0/src/Cooperator.Framework.Modeling/Model/DomainTreeHelper.cs` | 247 | **Mitad de árbol** del helper (L834–1056 del 1.4): los 11 métodos portables |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/DBReverse.cs` | 574 | Tipos de esquema + `OrderEnum`, tal cual (sólo namespace) |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/DBReverseHelper.cs` | 301 | `IDBReverseProvider`, `IDBTypeConverter`, `DBReverseEngine`, `DBReverseEngineFactory`, `IncludedEnum`, `DBParentsHelper`, tal cual (sólo namespace) |
| `2.0/src/Cooperator.Framework.Modeling/Reverse/TypeMapping.cs` | 233 | **La** fuente única del mapeo SQL↔CLR: tabla de 28 entradas + `MSSQLTypeConverter : IDBTypeConverter` (§C.2) |
| `2.0/src/Cooperator.Framework.Modeling/Exceptions/UtilityException.cs` | 66 | Base de las excepciones del grafo: **`System.Exception`** (ruptura documentada §14(a)); `CompilerException` **excluida** (slot del motor, Bolt 017) |
| `…/Exceptions/UnknownDBTypeExeception.cs` · `NodeAlreadyExistException.cs` · `CannotAddOrRemoveItemException.cs` | 62 c/u | POCOs: sin `[Serializable]`, sin `ISerializable`, sin ctor `(SerializationInfo, StreamingContext)`, derivando de `UtilityException` |

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-003-particion-modeling.md` | `status: "blocked"` → `"in-development"` + fila **T06** (toma del V-Bounce N°2, G11) y `"in-development"` → `"blocked"` + fila **T07** (cierre). Ambas filas con el reloj real |
| `devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json` | Entrada `v_bounces[]` **número 2** con los ocho campos, `spec_revision: 2`, `execution_outcome: "blocked"` |

**Ningún archivo de los Bolt 001/002/006/011 fue tocado** — se leyeron sus artefactos y se corrieron sus suites
con `--no-build`. **Ninguna línea de `1.4/**` fue modificada.** **No se tocó** ningún otro Bolt, SPEC, manifiesto,
índice ni registro de corrida. **No se tocó ningún archivo del Bolt 001** (el `.csproj` de `Modeling` quedó
exactamente como estaba, lo que además cumple §A.1 y §5).

---

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

---

## 6. Files deleted

| File | Reason |
|------|--------|
| Los 8 archivos del aporte listados en §3 (Model/, Reverse/, Exceptions/) | Revertidos: el V-Bounce cerró `blocked` y el árbol debe quedar sin cambios |

> Tras la reversión se reconstruyó `Modeling` desde el stub para no dejar un `.dll` obsoleto (el que inspeccionan
> los tests del arnés), y se verificó que **no quedó ningún archivo basura** fuera de mis rutas
> (`D:\Cooperator\Model` y `D:\Cooperator\Reverse` → no existen).

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| **Medir el aporte completo antes de detenerme**, en lugar de detenerme con la sola lectura | La duda central del revisor era si la corrección de la revisión 2 (partir el helper) resuelve el defecto del V-Bounce N°1. Medirlo convierte "creo que sí" en "compila limpio con 0 errores estructurales y el paquete es hoja, acá está el reporte de dependencias". Sin esa medición, el próximo intento podría chocar con un segundo defecto tardío |
| **Detener el V-Bounce por la fase D** en lugar de entregar `Modeling` solo | AC-4, AC-5 y AC-6 exigen el tooling: «el provider concreto y la mitad reverse del helper **compilan fuera** del paquete portable», «el tooling **no** tiene una segunda tabla y el provider resuelve por la de `Modeling`», «el reverse y los editores están **en el tooling**». Y AC-8 exige el build del **árbol afectado**. Entregar sin el tooling dejaría 3 de 8 ACs incumplidos a sabiendas: un verde falso |
| **No quitarle la herencia a `MSSQLProvider` ni tocar su acceso a datos** para que compilara | §15 es explícito: «Al reubicar aparece la tentación de "aprovechar" y **reescribir el reverse** (quitarle la herencia, cambiarle el acceso a datos o su configuración): **parar**. Eso es el Bolt 016; acá el reverse se mueve **como está**» |
| **No aportar la capa `Data`** (su `BaseRule` y su cierre) para que el tooling compile | §4 pone fuera de alcance «El **reemplazo del proveedor de datos** de la capa `Data` y su borde de conversión: **Bolt 014**» y §13 declara el riesgo de que «el porte arrastra … la herencia de `BaseRule`». Portar `Data` es trabajo de los Bolts 012/013/014/016, y requiere además la decisión de provider de ADR-005 §3.1 |
| **No elegir ni declarar el paquete del provider ADO.NET** | ADR-005 §3.1 retira `System.Data.SqlClient` del framework, hace de `Microsoft.Data.SqlClient` el único provider y asigna la **versión** al Bolt del provider con **dos criterios de aceptación**, uno de los cuales es un *smoke test* de conexión real que este V-Bounce no puede correr (no hay SQL Server). Elegirlo sería inventar |
| **No dejar `#nullable disable` en el aporte**, aunque es lo que hace compilar | El SPEC (§A.2: «cambia sólo: el namespace, lo que este SPEC ordena retirar, y **nada más**») **no menciona nullable en ninguna parte** (0 ocurrencias). Añadir una directiva a 4 archivos portados es una decisión de política de build que no me corresponde: la **medí** y la dejo al revisor con su evidencia (§8.2) |
| **Regenerar el aporte tras el incidente del script** (§8.3) y declararlo | La honestidad del registro es parte del paquete: el incidente afectó sólo a archivos **míos generados** (no a artefactos gobernados ni a `1.4/**`), se detectó de inmediato y se corrigió regenerando desde el generador determinista |
| **Revertir todo y reconstruir el artefacto de build** | La reversión de un V-Bounce bloqueado debe ser el **no-cambio**: `Modeling` vuelve a tener sólo su `.csproj` y su lock, y su `.dll` se reconstruyó desde el stub para que las invariantes del arnés sigan verdes |

---

## 8. Deviations and assumptions

### 8.1 El bloqueo (blocker exacto) — la fase D no compila

**Condición de parada disparada: SPEC-261016-0004 §15, «el reverse se mueve como está».**

> «**Al reubicar aparece la tentación de "aprovechar" y reescribir el reverse** (quitarle la herencia, cambiarle
> el acceso a datos o su configuración): **parar**. Eso es el Bolt 016; acá el reverse se mueve **como está**.»

**La cadena, cada eslabón verificado con una sonda de compilación:**

1. §D.1 ordena reubicar `Providers/MSSQLProvider.cs` al tooling y **«conserva su acceso a datos tal como está»**.
2. Ese archivo declara, en su línea 499, `private class DataAccess : Cooperator.Framework.Data.BaseRule`.
3. **`Cooperator.Framework.Data.BaseRule` no existe en el árbol 2.0.** `2.0/src` tiene **cero archivos `.cs`** en
   sus cuatro proyectos; el ensamblado stub de `Data` (4.608 bytes) no contiene `BaseRule`. Sonda: un archivo
   con `class DataAccess : Cooperator.Framework.Data.BaseRule` más `ProjectReference` al `Data` real del árbol
   → `error CS0246: The type or namespace name 'Cooperator' could not be found`.
4. **`System.Data.SqlClient` tampoco está disponible en el TFM del tooling.** Sonda en `net10.0-windows`:
   `error CS1069: The type name 'SqlConnection' could not be found in the namespace 'System.Data.SqlClient'.
   This type has been forwarded to assembly 'System.Data.SqlClient, …' Consider adding a reference to that
   assembly.` ⇒ exige declarar un paquete.
5. **Ese paquete no es mío para elegir.** ADR-005 §3.1: «**Provider**: `Microsoft.Data.SqlClient`, **único**.
   Los paquetes de la 2.0 **no** dependen de `System.Data.SqlClient` (**se retira de `Data`**)»; y la **versión**
   «la fija el **Bolt** … con **dos criterios obligatorios de aceptación**: (1) asset consumible desde
   `netstandard2.1`, y (2) el arnés debe pasar un **smoke test de conexión real** contra SQL Server». El segundo
   criterio es **inejecutable** acá (no hay entorno), y el Bolt del provider es el **014**.

**Por qué es un bloqueo y no un ajuste:** las dos únicas formas de que la fase D compile son (i) **cambiar el
reverse** —quitarle la herencia o su acceso a datos—, que §15 prohíbe y asigna al Bolt 016, o (ii) **aportar la
capa `Data`** (su `BaseRule` y su cierre) **y** decidir el provider con su versión y su smoke test, que §4 pone
fuera de alcance y que es de los Bolts 012/013/014/016 + ADR-005. **No hay tercera vía** y ninguna de las dos es
del ejecutor.

**Nota de secuencia (lo que el SPEC dio por supuesto):** §E.1 declara que el tooling referencia «`Modeling`,
`Core`, `Library`, `Data`» y §D.1 que el reverse conserva su herencia; ambas cosas **asumen que `Data` ya tiene
su capa portada**. §5, en cambio, sólo exige la infraestructura del Bolt 001. El gap no está en las fuentes
gobernadas ni en el baseline — está en el orden: **este Bolt necesita la capa `Data` antes de la fase D**, y hoy
`2.0/` no tiene una sola línea de código de framework.

### 8.2 Segundo hallazgo — `Nullable=enable` vs. el código portado del 1.4 (afecta a las fases A–C)

El SPEC **no menciona nullable en ninguna parte** (0 ocurrencias en `SPEC-261016-0004`). Pero el árbol hereda de
`2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`, y el código del 1.4
es previo a nullable:

```text
Build de Cooperator.Framework.Modeling con el aporte portado TAL CUAL (§A.2):
    21  CS8618   (campo/evento no anulable sin inicializar en el constructor)
     4  CS8603   (retorno de referencia posiblemente nulo)
     3  CS8625   (no se puede convertir null literal a tipo de referencia no anulable)
    ------
    28 errores  —  y CERO errores de otro tipo (ni un CS0234/CS0246)
```

Es decir: **el porteo es estructuralmente correcto** (todo tipo, namespace y llamada resuelve) y lo único que
lo detiene es una **colisión de política de build** contra código escrito para un mundo sin anotaciones de
nulabilidad. Con `#nullable disable` (una línea) al frente de los **cuatro** archivos portados con código net48:

```text
Cooperator.Framework.Modeling -> ...\bin\Release\netstandard2.1\Cooperator.Framework.Modeling.dll
EXIT=0     0 errores, 0 advertencias
```

**Opciones que el revisor tiene sobre la mesa** (ninguna la elegí yo):
1. **`#nullable disable` por archivo portado** ← la que medí. Preserva la semántica exacta del 1.4 (la directiva
   apaga el *análisis*, no cambia una instrucción de IL), es de una línea por archivo, y su efecto queda acotado
   a los archivos portados (el código nuevo de la 2.0 sigue con `Nullable=enable`).
2. `<Nullable>disable</Nullable>` en el `.csproj` de `Modeling` — desactivaría la nulabilidad para todo el
   paquete, incluido el contrato del Bolt 004.
3. Una **revisión material del SPEC** que fije el mecanismo (es una decisión de política, y la revisión 2 no
   la tomó).

### 8.3 Incidente del script de porteo (declarado, contenido, corregido)

Durante la preparación de la sonda de §8.2, un script auxiliar mío usó rutas **relativas** contra
`[System.IO.File]`, que resuelve contra el directorio del *proceso* y no contra la ubicación de PowerShell. El
efecto fue que **dos archivos de mi propio aporte generado** (`Reverse/DBReverse.cs` y `Reverse/DBReverseHelper.cs`)
quedaron sobrescritos con contenido vacío. Se detectó al primer build posterior, se regeneraron desde el generador
determinista y el build volvió a verde. **Alcance del incidente: nulo sobre artefactos gobernados** — afectó sólo
a archivos que este V-Bounce había creado y que igualmente se revirtieron; **no** tocó `1.4/**`, ni archivos de
otros Bolts, ni ningún documento gobernado. Se verificó además que **no quedó ningún archivo basura** fuera de mis
rutas. Se declara porque el MEM es el registro de lo que pasó, no de lo que debería haber pasado.

### 8.4 G15 — verificación explícita de fuentes gobernadas

**Sin cambio material.** `US-000.BOLT-003` es el mismo (su `review` de `AITL-BOLT-READY-Approval` intacto) y sólo
cambió su `status`, por los actos de este V-Bounce; `REV-001` sigue `approved`; `ADR-001`/`ADR-002`/`ADR-008`/`ADR-005`
siguen `accepted` y sin reescritura; la línea base de código sigue siendo
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto frente a ella. **El `AITL-SPEC-Approval` de la
revisión 2 sigue vigente** — pero para avanzar hace falta resolver el gap de secuencia de §8.1 (y, para las fases
A–C, la política de nulabilidad de §8.2), no por cambio de fuente sino por **hallazgo de ejecución**.

### 8.5 Comprobación de las demás condiciones de parada (§15) — todas negativas salvo la del reverse

| Condición de parada (§15) | ¿Dispara? | Comprobación |
|---|---|---|
| La tensión §14(a)/(b) se resuelve en contra durante la aprobación | **No** | El `review` de la revisión 1 aprobó (a) y (b) y la revisión 2 las mantiene en vigor; la revisión 2 aprobó la partición del helper y el presupuesto STJ |
| Un tipo del modelo depende de algo fuera del paquete más allá de los declarados | **No** | Verificado por el build y por el reporte de dependencias: `Modeling` referencia **sólo `netstandard`**; los 8 sitios se resuelven adentro (era el defecto del V-Bounce N°1 y está **corregido**) |
| La suite de caracterización del Bolt 001 no existe o no corre | **No** | Corre verde: **83/83** (y el arnés **20/20**), medida antes de tocar nada (§9) |
| El mismo mapeo exigido por dos consumidores con formas incompatibles | **No** | Un solo consumidor presente (el reverse relocalizado); el motor no existe todavía |
| **Al reubicar aparece la tentación de reescribir el reverse** | **SÍ** | §8.1: el reverse «como está» no compila; cambiarlo es el Bolt 016 |
| El tooling necesita que este Bolt entregue el shell, el editor o el empaquetado | **No** | Sonda: los `PropertyEditors` **sí** compilan en `net10.0-windows` con `UseWindowsForms=true` (`UITypeEditor` + `IWindowsFormsEditorService` disponibles) ⇒ la nota de control del finding (b) **no** se activa |
| Aparece código del motor entre los archivos a reubicar | **No** | `ScriptingHost`/`ScriptsEnvironment` no están entre los archivos a portar; `CompilerException` **no** se portó (comparte archivo con `UtilityException`) |
| Una ruptura nueva no listada en §6/E.2 | **No evaluado** | No se llegó a producir la constancia; se registran sus insumos en §13 |
| Cambio material de una fuente gobernada | **No** | §8.4 |
| Un gate de §9 no cierra en `pass` | **No evaluado** | No hay entrega que someter a gate |

### 8.6 Supuestos

- Se asumió que el helper se parte **por frontera de línea** de la fuente congelada: `Modeling` toma
  L834–1056 (los 11 métodos con sus doc-comments) y el tooling L76–833 (los 3 métodos). Lo confirmé contra la
  fuente y contra las líneas que el `review` de la revisión 2 verificó.
- Se asumió que la sentencia `PropertyEditors.DropDownListEditor.stringList = _ParentCandidates;` (L1621 del 1.4,
  dentro del getter `GenerateAsChildOf` de `EntityNode`) debe retirarse junto con los atributos de editor: es
  parte del mismo acoplamiento a la UI, §B.1 lo cubre con «**cualquier otro con esta función**» y **AC-1/AC-4 la
  exigen** (dejarla sería una arista del modelo hacia el tooling, es decir el propio §15). **Es un retiro que el
  SPEC no enumera literalmente**: se declara acá y en la lista de rupturas (§13) para que las notas del Bolt 028
  lo recojan.
- No se asumió ninguna decisión de otro Bolt: ni la forma de la capa `Data`, ni el provider ADO.NET ni su versión
  (ADR-005 §3.1 = Bolt 014), ni la política de nulabilidad (§8.2). Las salidas se listan como **materia de la
  decisión humana**, no como plan de ejecución.
- No se leyó, copió ni procesó ningún dato de negocio, credencial ni archivo de configuración: la evidencia son
  líneas de código del árbol `1.4/` congelado, conteos y salidas de compilación.

---

## 9. Verification evidence

### Prerequisito del §5 — suites del Bolt 001, VERDES antes de tocar nada

```text
Passed!  - Failed: 0, Passed: 20, Skipped: 0, Total: 20   - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed: 83, Skipped: 0, Total: 83   - Cooperator.Framework.Characterization.Tests.exe (net48)
```

Corridas con `--no-build` sobre los ensamblados del Bolt 001: **no se recompiló ni se reescribió nada suyo**.

### La partición de la revisión 2 FUNCIONA — build del paquete portable (medido, luego revertido)

```text
$ msbuild 2.0/src/Cooperator.Framework.Modeling/Cooperator.Framework.Modeling.csproj /t:Rebuild /p:Configuration=Release

  Cooperator.Framework.Modeling -> ...\bin\Release\netstandard2.1\Cooperator.Framework.Modeling.dll
EXIT=0        0 errores, 0 advertencias
```

Con `TreatWarningsAsErrors=true`, `Nullable=enable`, `EnableNETAnalyzers=true`, `AnalysisLevel=latest` y firma
strong-name — **el mismo conjunto de propiedades que aplica el árbol**. Y `EXIT=0` **sólo** con la directiva de
§8.2; sin ella, los 28 diagnósticos de nulabilidad (todos `CS86xx`, **cero** estructurales).

### AC-1/AC-4 — reporte de dependencias por ensamblado (la prueba dura)

```text
=== ensamblados referenciados por Cooperator.Framework.Modeling.dll ===
  netstandard 2.1.0.0                        <- y NINGUNO mas

=== aristas prohibidas ===
  Cooperator.Framework.Data      -> False
  Cooperator.Framework.Library   -> False
  Cooperator.Framework.Core      -> False
  CooperatorModeler              -> False
  System.Windows.Forms           -> False
  System.Drawing                 -> False

=== nivel de proyecto (2.0/src/Cooperator.Framework.Modeling/*.csproj) ===
  <TargetFramework>netstandard2.1</TargetFramework>
  <PackageReference Include="System.Text.Json" />      <- exactamente una, y cero ProjectReference
```

`Modeling` es el **paquete hoja** del diagrama de ADR-001 §3.1, con las **ocho invocaciones** del modelo a
`DomainTreeHelper` resueltas **dentro** del paquete (si no lo estuvieran, el build habría fallado: un paquete
`netstandard2.1` no puede referenciar el proyecto `net10.0-windows` del tooling).
Observación: el ensamblado **no** referencia `System.Text.Json` porque todavía no hay código de serialización
(es del Bolt 004); la dependencia se mide en el **proyecto** y en el lock file — que es lo que asserta la
invariante del arnés del Bolt 001.

### Sondas del bloqueo (§8.1)

```text
SONDA 1 — PropertyEditors en net10.0-windows (UseWindowsForms=true):
  Build succeeded.  0 Warning(s)  0 Error(s)
  -> System.Drawing.Design.UITypeEditor y System.Windows.Forms.Design.IWindowsFormsEditorService EXISTEN
  -> la nota de control del finding (b) NO se activa

SONDA 2 — el reverse "como esta" (DataAccess : BaseRule) en net10.0-windows
          + ProjectReference al proyecto Data real del arbol:
  error CS0246: The type or namespace name 'Cooperator' could not be found
  -> Cooperator.Framework.Data.BaseRule NO EXISTE (2.0/src tiene cero .cs; el stub de Data no contiene BaseRule)

SONDA 2b — System.Data.SqlClient en net10.0-windows sin paquete:
  error CS1069: The type name 'SqlConnection' could not be found in the namespace 'System.Data.SqlClient'.
                This type has been forwarded to assembly 'System.Data.SqlClient, ...'
  -> exige declarar un paquete; su reposicion es de ADR-005 3.1 (Bolt 014, con smoke test de conexion real)
```

### Estado del árbol tras la reversión (rollback = no-cambio)

```text
2.0/src/Cooperator.Framework.Modeling/  ->  Cooperator.Framework.Modeling.csproj + packages.lock.json  (0 .cs)
2.0/src/  (todos los proyectos)         ->  0 archivos .cs      (estado del Bolt 001, intacto)
D:\Cooperator\Model / D:\Cooperator\Reverse  ->  no existen (sin archivos basura)

Re-verificación de las suites del arnes DESPUES de revertir:
  Passed!  - Failed: 0, Passed: 20, Total: 20   (Harness.Tests, net10.0)
  Passed!  - Failed: 0, Passed: 83, Total: 83   (Characterization.Tests, net48)

git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Build

```text
n/a — no hay build de entregable que exhibir: el aporte se revirtió tras medir el bloqueo.
     (Los dos builds registrados arriba —el del paquete portable y las tres sondas— se ejecutaron para MEDIR
      el bloqueo y no forman parte de una entrega; las sondas corrieron en el directorio temporal, fuera del
      repositorio.)
```

### Tests

```text
n/a — no se escribió ninguna prueba. 0 casos creados.
     (Las 103 corridas verdes son del arnés del Bolt 001, como evidencia del prerequisito y de que el árbol
      quedó sano tras la reversión; no son pruebas de este Bolt.)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2 del SPEC) y el
V-Bounce no llegó a escribir ninguna prueba.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG (§3.3.1).

### Gates

> Vocabulario de T11 (`pass` / `waived` / `n/a` con razón). **Ninguno de estos `n/a` es un `pass` encubierto:
> ningún gate se ejecutó sobre una entrega, porque no hay entrega.**

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | `n/a` — no hay entrega. **La medición sí se hizo:** el paquete portable compila con **0 errores y 0 advertencias** (con el mecanismo de §8.2); el tooling **no** compila (§8.1) |
| Unit / component tests | V-Bounce | `n/a` — no se escribieron pruebas. Las del arnés del Bolt 001 quedaron verdes (20/20 y 83/83) antes y después |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — no se escribió el doble del contrato portable (depende de la entrega) |
| SAST | V-Bounce | `n/a` — 0 líneas de código nuevo en el árbol |
| Dependency scanning / licenses | V-Bounce | `n/a` — no se declaró ninguna dependencia nueva. **La medición del presupuesto sí se hizo:** `Modeling` con exactamente `System.Text.Json` y cero `ProjectReference` (AC-1) |
| OWASP Top 10 — static review | V-Bounce | `n/a` — el SPEC lo declara `n/a` con razón |
| Prompt-injection scan | V-Bounce | `n/a` — el SPEC lo declara `n/a` |
| Secret-leak scan | V-Bounce | `n/a` — no se produjo ningún entregable; este MEM no contiene credenciales |
| Hallucination lint | V-Bounce | `pass` — cada afirmación de este MEM es trazable a una salida de compiler registrada arriba, a una sonda registrada, a una búsqueda registrada, o a una línea verificada del árbol congelado. Es el único gate evaluable hoy |
| IP / license provenance | V-Bounce | `n/a` — 0 código incorporado (el encabezado BSD se conservó en las copias y se revirtió con ellas) |
| PII / DLP | V-Bounce | `pass` — 0 datos personales ni de negocio leídos o escritos |
| Dependency-confusion | V-Bounce | `n/a` — 0 paquetes nuevos |
| Test-first evidence | V-Bounce | `n/a` — no se escribieron pruebas. **La parte que aplica se cumplió:** la suite de caracterización corrió verde **antes** de mover cualquier tipo |
| Behavioral reproducibility (hermetic suite) | V-Bounce | `n/a` — no hay suite nueva |
| Bolt-manifest validation | V-Bounce | `pass` — `v_bounces[2]` valida contra `manifest-v5-bolt.schema.json` (§9, Manifest) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | `n/a` — no hay superficie 2.0 nueva que comparar (el paquete se revirtió) |
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

**None.** No hubo parche humano ni fallback manual. Las decisiones humanas pendientes son las de §8.1 (secuencia
con la capa `Data`) y §8.2 (política de nulabilidad), no intervenciones sobre el código.

---

## 11. Evidence links

- **Diff / PR:** no hay — **no quedó código en el árbol**. Los únicos cambios son la actualización de estado de
  `US-000.BOLT-003`, este MEM y su entrada de manifiesto. No se hicieron operaciones de git: el commit es del
  humano (G34).
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — el mismo que el SPEC §3 declara. **El
  trabajo de este V-Bounce no lo mueve.**
- **V-Bounce N°1 (historia inmutable):** [`MEM-261016-0046`](MEM-261016-0046-particion-modeling.md) — el
  análisis del primer bloqueo (los 8 sitios).
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json`](../metrics/bolts/US-000.BOLT-003-particion-modeling.json)
- **Fuente de la evidencia (sólo lectura):** `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs`
  (L76–1056 helper: L85/L198/L312 reverse, L834–1056 mitad portable; L1058–fin modelo) ·
  `…/DBReverse.cs` · `…/DBReverseHelper.cs` · `…/Providers/MSSQLCommon.cs` (la tabla extraída) ·
  `…/Providers/MSSQLProvider.cs` (L499, la herencia bloqueante) · `…/PropertyEditors/*` · `…/Exceptions/*`
- **Normativo que decide el bloqueo:** SPEC-261016-0004 §4 (out of scope: capa `Data`/Bolt 014), §6/D.1, §6/E.1,
  §13 (riesgo «el porte arrastra la herencia de `BaseRule`»), §15; **ADR-005 §3.1** (provider único, retiro de
  `System.Data.SqlClient`, versión con smoke test de conexión real); ADR-001 §3.1/§3.4.
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/Directory.Build.props` (la política
  `Nullable=enable` + warnings-as-errors de §8.2), `2.0/Cooperator2.sln`, `2.0/src/**/*.csproj`,
  `2.0/tests/**`. **No se modificó ninguno.**

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados al porteo, a las tres sondas y al cierre; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | **2** (dentro del mismo Bolt; el N°1 quedó `blocked` y su MEM es inmutable) |
| Tests created | **0** — no se escribió ninguna prueba |
| AI-generated code | **0 líneas permanecen** — el aporte se generó para medir (≈3.325 líneas portadas, con el retiro ordenado) y se revirtió |
| Loops consumed | **3 de 10** — (1) build estructural del aporte, (2) sonda de la política de nulabilidad, (3) re-verificación tras la reversión; el V-Bounce se detuvo en la condición de parada sin entregar |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval` |

---

## 13. Pending items and stubs

- [ ] **BLOQUEANTE 1 — acción humana (decisión de secuencia):** resolver el gap de §8.1. Opciones visibles:
      **(a) secuenciar** — ejecutar el porte de la capa `Data` (Bolts 012/013/014/**016**, con la decisión de
      provider y versión de ADR-005 §3.1 y su smoke test de conexión real) **antes** de la fase D de este Bolt;
      **(b) partir el Bolt/SPEC** — entregar `Modeling` (fases A, B, C y la evidencia portable de E) como unidad
      propia y diferir la reubicación del tooling (fase D) a cuando `Data` exista; **(c) si el tooling debe
      compilar ya**, el SPEC tiene que decidir qué pasa con el acceso a datos de `MSSQLProvider` (hoy Bolt 016 +
      ADR-005), lo que es una revisión material. **Ninguna de las tres es del ejecutor.**
- [ ] **BLOQUEANTE 2 — acción humana (decisión de política):** fijar el mecanismo de nulabilidad para el código
      portado (§8.2). Recomendación medida: `#nullable disable` por archivo portado (una línea, semántica del 1.4
      intacta, efecto acotado a los archivos portados). Alternativas: `<Nullable>disable</Nullable>` por proyecto,
      o una revisión material del SPEC.
- [ ] **Recordatorio para el V-Bounce que entregue:** el porteo del tooling debe **calificar** las cuatro llamadas
      de la mitad reverse a la portable (`CorrectGenerationNamesForRelatedFields` L184, `FindEntityInTree` L222/L297,
      `CorrectSelectedCollections` L292) como `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X`, porque el
      tooling tiene una clase con el mismo nombre (§B.2).
- [ ] **Recordatorio — rupturas a declarar en §6/E.2 y en las notas del Bolt 028** (insumos ya identificados):
      tipos que cambian de ensamblado **y de namespace** (`Cooperator.Framework.Utility.DBReverseHelper` →
      `Cooperator.Framework.Modeling.Model`/`.Reverse`; `…Utility.Exceptions` → `….Modeling.Exceptions`);
      **atributos de diseñador retirados** (`[EditorAttribute]`, `[DesignerSerializationVisibility]`,
      `[Description]`, `[Browsable]`, `[DefaultValue]`); **`[Serializable]` retirado** de los tipos que migran;
      **base de las excepciones del grafo** cambiada a `System.Exception`; **`DomainTreeHelper` partido en dos
      ensamblados** con el mismo nombre de tipo; **`CompilerException` fuera** de la partición (slot del motor);
      y **el acoplamiento del modelo al editor retirado** (la sentencia de `DropDownListEditor.stringList`, §8.6).
- [ ] **Nota de verificación para el próximo intento:** el ensamblado `Modeling` **no** referencia
      `System.Text.Json` porque todavía no hay código de serialización (es del Bolt 004); `DependencyReportTests`
      debe medir el **proyecto/lock file** para el presupuesto de dependencias y el **ensamblado** para las
      aristas prohibidas (sólo `netstandard` hoy).

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce. Siendo este V-Bounce `blocked`, la decisión
> esperable es `changes_requested` / `rejected` sobre el MEM como historia inmutable, y la respuesta es una
> decisión de secuencia (y de política de nulabilidad) seguida de un **V-Bounce nuevo** con su MEM nuevo.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | — |
| **Roles** | — |
| **Decision** | — |
| **review_ready_at** | `2026-10-16T01:07:17-03:00` |
| **review.started_at** | — |
| **review.decided_at** | — |
| **Review evidence** | — |
| **Comments** | — |
| **Findings** | — |
| **acknowledged_without_comment** | — |
| **acknowledgment_reason** | — |
