---
id: "MEM-261016-0046"
title: "Partición de `Cooperator.Framework.Modeling` — V-Bounce detenido por dependencia del modelo hacia el tooling"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-003" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0004" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008", "ADR-002"] # governing ADRs — none of them materialised a change here (V-Bounce stopped before code)
manifest: "US-000.BOLT-003-particion-modeling.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was produced (see §11)
review_ready_at: "2026-10-16T00:46:54-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T00:51:54-03:00"
  decided_at: "2026-10-16T00:51:54-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible: los 8 sitios del modelo hacia DomainTreeHelper verificados en 1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs (EntityNode L1226/L1635; PropertyNode L1838/L2187/L2191/L2206/L2215/L2222). DomainTreeHelper (L76) instancia MSSQLReverseFactory (L201) y el SPEC rev.1 lo clasifica tooling (§B.2/§D.1). Las dos llamadas en L222/L297 están dentro del propio helper (L76-L1056), no en el modelo: el conteo de 8 es exacto."
    - "Compliant — el blocker coincide literalmente con SPEC §15 (segunda condición de parada: un tipo del modelo depende de algo fuera del paquete más allá de los dos acoplamientos conocidos); las tres salidas están vetadas por el propio SPEC/ADRs — §14 prohíbe partir el helper, ADR-001 §3.1/§D.2/AC-1/AC-4 prohíben la arista Modeling->tooling, AC-3 prohíbe retirar las llamadas — y netstandard2.1 no puede referenciar el tooling net10.0-windows."
    - "Compliant — segundo hallazgo correctamente escalado: ADR-001 §3.4 nombra System.Text.Json como única dependencia de serialización de Modeling (verificado) y §5 NFR exige 0 dependencias de terceros; la invariante del Bolt 001 Modeling_declares_exactly_system_text_json existe (BuildInvariantsTests.cs:160) y el stub actual de Modeling.csproj declara esa PackageReference, en conflicto con SPEC-003 §A.1/§14/AC-1 (sin ninguna PackageReference). Reconciliación con dueño: no se resuelve desde el ejecutor."
    - "Compliant — cierre correcto: specified->in-development->blocked (T04/T05); MEM único (sólo MEM-261016-0046 referencia US-000.BOLT-003); manifiesto v_bounces[0] con los 8 campos y execution_outcome: blocked; sin código parcial (Model/Reverse/Exceptions inexistentes; Modeling.csproj SHA-256 BCF8E17A... = stub del Bolt 001; CooperatorModeler y Modeling.Tests inexistentes; Cooperator2.sln sin CooperatorModeler); 1.4/** intacto (git diff --quiet exit 0)."
    - "Compliant — desbloqueo propuesto correcto y agéntico: revisión material del SPEC (nueva revisión + spec_revisions[] + AITL-SPEC-Approval) que resuelva el acoplamiento modelo->DomainTreeHelper, seguida de V-Bounce nuevo. El revisor del SPEC es agent:dotnet-developer-reviewer-agent en otro acto — no requiere al humano."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0046 — Partición de `Modeling`: V-Bounce detenido por dependencia del modelo hacia el tooling

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-003](../functional/bolts/US-000.BOLT-003-particion-modeling.md) |
| **SPEC**        | [SPEC-261016-0004](../spec/SPEC-261016-0004-particion-modeling.md), revisión 1 (aprobada) |
| **V-Bounce**    | 1 |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) |

> ⚠️ **Este MEM registra un V-Bounce que NO generó código.** No hay entrega: hay un bloqueo con evidencia de
> código. El MEM es obligatorio en V-Bounces bloqueados (§3.3, G17) y es lo que permite desbloquear.

---

## 1. Executive summary

El V-Bounce N°1 del Bolt `US-000.BOLT-003` se detuvo **antes de generar una sola línea de código**, al dispararse
la segunda condición de parada del SPEC aprobado: **un tipo del modelo depende de algo fuera del paquete más allá
de los dos acoplamientos conocidos**. La evidencia es de código y es concluyente: los tipos del modelo llaman
**ocho veces** a `DomainTreeHelper` —`DomainTreeHelper.CorrectCLRType` en 2 sitios (`EntityNode.cs` L1226,
`PropertyNode` L1838), `CorrectSelectedCollections` en 1 (L1635), `GetFullGenerateAsForEntity` en 3 (L2187, 2206,
2215) y `FindEntityInTree` en 2 (L2191, 2222)— y ese helper es, por decisión explícita del propio SPEC (§B.2 y
§D.1), **excluido del modelo y reubicado en el tooling**, porque instancia `MSSQLReverseFactory` y usa la cadena
de conexión. El SPEC no vio esa dependencia en el sentido contrario: describió el acoplamiento como "el helper usa
el modelo" y no como "el modelo usa el helper", de modo que su clasificación deja al paquete portable con ocho
referencias a un tipo que ya no está en él. Las tres salidas posibles están **todas prohibidas por el propio
SPEC**: dejar la arista `Modeling → tooling` viola ADR-001 §3.1, §D.2, AC-1 y AC-4 (y ni compila: `netstandard2.1`
no puede referenciar `net10.0-windows`); retirar o reescribir las llamadas cambia el comportamiento observable que
AC-3 exige preservar (recálculo de derivados y de colecciones) y §15 prohíbe "arreglar la clasificación moviendo
el tipo de lugar sin evidencia"; y partir el helper —la única salida técnicamente sana— es exactamente lo que
§14 declara **no autorizado** para el ejecutor ("separarla exigiría diseñar un seam de inyección que ningún ADR
ordena"). Por eso paro y reporto: la decisión es una revisión material del SPEC y una re-aprobación, no una
decisión del ejecutor. Todo lo demás que el SPEC daba por supuesto **se verificó y está sano**: el prerequisito
del §5 está satisfecho (la suite de caracterización del Bolt 001 corre **verde: 83/83**, y las invariantes del
arnés **20/20**, medidas antes de tocar nada), el árbol `1.4/**` está intacto, y `DBReverse.cs` y
`DBReverseHelper.cs` son efectivamente POCOs portables sin una sola referencia Windows, a `Data`, `Library` ni
`Core` (F-32 confirmado) — el bloqueo es **uno solo** y está localizado. El árbol queda **sin cambios**: no se
creó ningún archivo del entregable, de modo que la reversión es el no-cambio. Se registra además un **segundo
hallazgo** que quien desbloquee este Bolt va a encontrar de inmediato y que también requiere una decisión humana:
SPEC-003 §A.1/§14/AC-1 exigen que `Modeling` no declare **ninguna** `PackageReference`, mientras la suite de
invariantes del Bolt 001 —entregada y en revisión— asserta que `Modeling` declara exactamente `System.Text.Json`
(§8.2 de este MEM).

---

## 2. Implemented phases

**Ninguna fase del SPEC §6 se implementó.** El V-Bounce se detuvo en la verificación de prerequisitos y en la
reconocimiento de las fuentes a portar, antes de la fase A, por la condición de parada descrita en §1 y §8. No
existe `2.0/src/Cooperator.Framework.Modeling/Model|Reverse|Exceptions`, ni `2.0/src/CooperatorModeler`, ni
`2.0/tests/Cooperator.Framework.Modeling.Tests`. El `Cooperator.Framework.Modeling.csproj` sigue siendo,
byte a byte, el stub que dejó el Bolt 001 (SHA-256 `BCF8E17AD7B72F9939FF69ACB756AB09CE45B85E537E195CB61731BD9C135D21`),
y `2.0/Cooperator2.sln` **no** fue tocada.

Lo que sí se hizo es trabajo real de **lectura y verificación**: el prerequisito del §5, el reconocimiento
completo de las fuentes a portar y la clasificación tipo por tipo que el SPEC §E.2 exige. Todo ello queda
registrado acá porque es la base del próximo V-Bounce.

### Prerequisites (§5 del SPEC) — verificación leída

| Prerequisito (§5) | Estado | Evidencia |
|---|---|---|
| Estructura de solución, `Directory.Build.props` (determinismo, warnings-as-error, `AssemblyVersion 2.0.0.0`, firma) y `Directory.Packages.props` del Bolt 001 | **DISPONIBLE** | `2.0/` con los cuatro portables, las dos suites y los fixtures (§9) |
| **Suite de caracterización del Bolt 001 ejecutable y verde antes de la partición** | **DISPONIBLE Y VERDE** | Corrida antes de tocar nada: `83/83` verdes, 0 rojos, 0 omitidos (§9). Las invariantes del arnés, además: `20/20` |
| Árbol `1.4/` presente y congelado en la línea base | **DISPONIBLE** | `git diff --quiet 6aa497f… -- 1.4` → exit `0`; `git status --porcelain -- 1.4` → vacío (§9) |
| Toolchain .NET 10 (SDK pinneado) y pack de referencia `netstandard2.1` | **DISPONIBLE** | El mismo toolchain que el Bolt 001 fijó (`global.json`, SDK 10.0.402) y que construyó el 1.4 |
| Fuentes del modelo/reverse disponibles para el porte | **DISPONIBLE** | `1.4/Cooperator.Framework.Utility/DBReverseHelper/**` y `Exceptions/**`, leídas (no modificadas) |
| **Clasificación tipo por tipo implementable tal como el SPEC la fija** | **NO — condición de parada** | El modelo depende de `DomainTreeHelper`, que el SPEC asigna al tooling (§8.1) |

### Reconocimiento de las fuentes (base del próximo V-Bounce)

**`DBReverseHelper/DomainTree.cs` (2360 líneas) — anatomía verificada:**

| Rango | Contenido | Destino per SPEC |
|---|---|---|
| L1–L43 | encabezado BSD + `using` | encabezado se conserva (gate IP de §9); 5 `using` se retiran |
| L44–L75 | `enum CollectionTypeEnum` | modelo (POCO) |
| L76–L1056 | `static class DomainTreeHelper` (13 métodos) | **tooling** (§B.2/§D.1) ← **origen del bloqueo** |
| L1058–L1142 | `abstract class BaseTreeNode` `[Serializable]` | modelo (POCO) |
| L1144–L1697 | `class EntityNode : BaseTreeNode` `[Serializable]` | modelo (POCO) |
| L1699–L2304 | `class PropertyNode : BaseTreeNode` `[Serializable]` | modelo (POCO) |
| L2306–L2443 | `class RelatedField` `[Serializable]` | modelo (POCO) |
| L2445–L2526 | `class ToStringStruct` `[Serializable]` | modelo (POCO) |
| L2527–L2558 | `enum DatabaseQueryReturnType` | modelo (POCO) |
| L2560–fin | `class DatabaseQuery` `[Serializable]` | modelo (POCO) |

**Metadata a retirar en los tipos que migran (contada sobre la fuente):** 6 × `[Serializable]`,
3 × `[EditorAttribute]` (L1616 `DropDownListEditor`, L1672 `ToStringInfoEditor`, L2630 `StringEditor`),
1 × `[DesignerSerializationVisibility]` (L1646), 10 × `[Browsable]`, 53 × `[Description]`,
2 × `[DefaultValue]`, 0 × `[Category]`. **0 × `SerializationInfo` / `GetObjectData` / `ISerializable`**: los
`[Serializable]` del 1.4 son serialización automática, sin contrato explícito. Los 5 `using` no portables son
`System.Runtime.Serialization.Formatters.Binary` (import muerto, F-30), `System.ComponentModel.Design`,
`System.Drawing`, `System.Windows.Forms.Design` y `System.Windows.Forms`. **Ningún atributo es multilínea** y
**ninguno de los tipos del modelo expone un tipo de UI en su firma**: el acoplamiento a la UI es solo por
atributos y por **una** sentencia en un `get` (L1621, `DropDownListEditor.stringList = _ParentCandidates;`) — es
decir, un acoplamiento **conocido y retirable**, no un bloqueo.

**`DBReverse.cs` (574 líneas) y `DBReverseHelper.cs` (301 líneas):** `namespace Cooperator.Framework.Utility.DBReverseHelper`,
usings `System`/`Collections.Generic`/`Text` **solamente**, y **cero** referencias a `BaseRule`,
`Cooperator.Framework.Data|Library|Core`, `System.Windows*`, `System.Drawing`, `System.ComponentModel`,
`[Serializable]` o `MSSQL`. Confirman F-32 (Compliant): se portan limpios, y su único cambio es el namespace.

**`Providers/MSSQLCommon.cs` (244 líneas):** contiene exactamente `enum SP_HelpTablesEnum` (→ tooling) y
`class MSSQLTypeConverter : IDBTypeConverter` con la tabla del mapeo (`NativeTypeExist` 27 tipos nativos,
`NativeType2CLRType`, `GetLength`) → la tabla se **extrae** a `Modeling` (§C.2) y el resto va al tooling (§D.1).

**`Exceptions/**`:** `UtilityException : Cooperator.Framework.Library.Exceptions.BaseException` (la arista
prohibida `Modeling → Library` que §14(a) resuelve cambiando la base a `System.Exception` — **decisión aprobada
en `review.findings`**), `UnknownDBTypeExeception`, `NodeAlreadyExistException`,
`CannotAddOrRemoveItemException : UtilityException`, los cuatro `[Serializable]` con constructor
`(SerializationInfo, StreamingContext)`. **`CompilerException : UtilityException` vive en el MISMO archivo**
`Exceptions/UtilityException.cs` y **no se porta**: su destino es el slot del motor (Bolt 017) — el finding del
SPEC lo registra como "correctamente excluido".

**`Providers/MSSQLProvider.cs` (461 líneas):** `MSSQLReverseFactory : DBReverseEngineFactory` (L43) cuyo
`CreateDBTypeConverter()` devuelve `new MSSQLTypeConverter()` (L69-71) → pasa a devolver el conversor de
`Modeling` (§D.1); `MSSQLProvider : IDBReverseProvider` (L78); `private class DataAccess : Cooperator.Framework.Data.BaseRule`
(L499) que **se conserva**: el retiro de la herencia es del Bolt 016.

**`PropertyEditors/*` (8 archivos, ~600 líneas + 2 `.resx`):** todos dependen de `System.Drawing`,
`System.Windows.Forms.Design`, `System.Windows.Forms` y `System.Drawing.Design` → son los archivos que §D.1
reubica al tooling y cuya compilación en `net10.0-windows` es el riesgo declarado en el finding §14(b).

---

## 3. Files created

| File | Purpose |
|------|---------|
| `devflow/memory/MEM-261016-0046-particion-modeling.md` | Este MEM: el registro obligatorio del V-Bounce bloqueado, con la evidencia de código del bloqueo y el reconocimiento del prerequisito |

**No se creó ningún archivo de la entrega del SPEC** — ni el proyecto portable, ni sus carpetas `Model/`,
`Reverse/`, `Exceptions/`, ni el host del tooling, ni el proyecto de pruebas, ni `MIGRATION-PARTITION.md`.

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-003-particion-modeling.md` | `status: "specified"` → `"in-development"` + fila T04 (toma del Bolt, G11) y, al cerrarse el V-Bounce, `"in-development"` → `"blocked"` + fila T05 — la transición de cierre que la secuencia obligatoria exige para un V-Bounce bloqueado (G17 paso 4, §3.2.3) |
| `devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json` | Entrada `v_bounces[]` número 1 con los ocho campos, `execution_outcome: "blocked"` |

**Ningún archivo del Bolt 001 ni del Bolt 002 fue tocado.** Sus paquetes están en revisión y sólo se leyeron
(se ejecutaron sus suites con `--no-build`, sin recompilar ni reescribir nada suyo). **Ninguna línea de `1.4/**`
fue modificada.** **No se tocó** ningún otro Bolt, SPEC, manifiesto, índice ni registro de corrida.

---

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

---

## 6. Files deleted

| File | Reason |
|------|--------|
| — | No hubo borrados. |

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| **Detener el V-Bounce sin generar código**, en lugar de entregar las fases que sí son implementables (A, C, y el reconocimiento de B/D) | Es una condición de parada explícita del SPEC aprobado (§15, segunda) y su texto cierra la puerta a la salida creativa: «La clasificación no se 'arregla' **moviendo el tipo de lugar sin evidencia**». Entregar un `Modeling` al que le faltan ocho llamadas, o al que se le inventó un arreglo de clasificación, produciría un paquete que se sabe mal clasificado: un verde falso |
| **No dejar el árbol a medio construir** (ni carpetas vacías, ni el `csproj` reescrito, ni el alta en la solución) | Un árbol a medio construir obliga al próximo V-Bounce a reconciliar un estado que nadie pidió y mete en la solución un paquete que no compila. La reversión de este V-Bounce debe ser el **no-cambio**, y así es: el `Modeling.csproj` conserva su SHA-256 del Bolt 001 |
| **No inventar el arreglo de la clasificación** (ni partir el helper, ni inyectar una factory, ni inlinear los 4 métodos portables en el modelo, ni aceptar la arista) | Las cuatro son decisiones de arquitectura y las cuatro están vetadas: partir el helper lo prohíbe §14; el seam lo prohíbe §14; la arista `Modeling → tooling` la prohíben ADR-001 §3.1, §D.2, AC-1 y AC-4 (y es además imposible: `netstandard2.1` no referencia `net10.0-windows`); y borrar las llamadas viola AC-3 |
| **No cerrar el Bolt como `failed` ni como `ready_for_review`** | No falló nada por un defecto de mi ejecución (`failed` sería engañoso) y no hay entrega que revisar (`ready_for_review` sería fabricar completitud). Corresponde `blocked`: el trabajo no puede avanzar sin una decisión humana |
| **Registrar la anatomía del porteo y la evidencia de código del bloqueo en detalle** | El próximo V-Bounce no debe repetir el reconocimiento, y quien revise la revisión material del SPEC necesita los números de línea y los conteos exactos para decidir sin volver a leer 2360 líneas |
| **Reportar el segundo hallazgo (`System.Text.Json` frente a la invariante del Bolt 001)** aunque el V-Bounce se detenga antes de tocarlo | Es un conflicto entre dos artefactos aprobados que aparecerá en el primer minuto del próximo intento. Dejarlo sin registrar sería esconder un problema conocido (§8.2) |
| **Ejecutar las suites del Bolt 001 con `--no-build`** | Da la evidencia del prerequisito sin recompilar ni reescribir nada de un artefacto cuyo paquete está en revisión |

---

## 8. Deviations and assumptions

### 8.1 El bloqueo (blocker exacto)

**Condición de parada disparada: SPEC-261016-0004 §15, segunda condición.**

> «**Un tipo del modelo resulta depender de algo fuera del paquete** más allá de los dos acoplamientos conocidos
> (provider concreto y UI): **parar y reportar**. La clasificación no se "arregla" moviendo el tipo de lugar sin
> evidencia.»

**Hecho verificado (de código, no inferido).** Los tipos del modelo invocan `DomainTreeHelper` en **ocho**
lugares; `DomainTreeHelper` es un tipo que el propio SPEC clasifica como **tooling** y excluye del paquete
portable (§B.2: «**se excluye** de `Model/domainTree.cs` y se reubica con el reverse (§D.1)»).

| # | Sitio | Tipo del modelo contenedor | Llamada |
|---|---|---|---|
| 1 | L1226 | `EntityNode` (`GenerateAs` setter) | `DomainTreeHelper.CorrectCLRType(this.Parent)` |
| 2 | L1635 | `EntityNode` (`GenerateAsChildOf` setter) | `DomainTreeHelper.CorrectSelectedCollections(this.Parent)` |
| 3 | L1838 | `PropertyNode` (setter de tipo) | `DomainTreeHelper.CorrectCLRType(this.Parent.Parent)` |
| 4 | L2187 | `PropertyNode` (`GenerateAsType` setter) | `DomainTreeHelper.GetFullGenerateAsForEntity(this.Parent.Parent, this.RelatedTableName)` |
| 5 | L2191 | `PropertyNode` | `DomainTreeHelper.FindEntityInTree(this.Parent.Parent, this.RelatedTableName)` |
| 6 | L2206 | `PropertyNode` | `DomainTreeHelper.GetFullGenerateAsForEntity(...)` |
| 7 | L2215 | `PropertyNode` | `DomainTreeHelper.GetFullGenerateAsForEntity(...)` |
| 8 | L2222 | `PropertyNode` | `DomainTreeHelper.FindEntityInTree(...)` |

**Por qué es el bloqueo y no un acoplamiento conocido.** El SPEC reconoce **dos** acoplamientos ("provider
concreto y UI"). `DomainTreeHelper` **no es ninguno de los dos**: §B.2 lo excluye por su propia causa —instancia
`MSSQLReverseFactory` y usa la cadena de conexión (`GetDomainTree`, L198-207)— y §D.1 le da destino propio
(`2.0/src/CooperatorModeler/DBReverseHelper/DomainTreeHelper.cs`). Es, literalmente, **un tercer acoplamiento del
modelo hacia fuera del paquete**.

**Por qué ninguna salida está disponible para el ejecutor.**

| Salida | Por qué no |
|---|---|
| Dejar `Modeling` con la referencia al tooling | ADR-001 §3.1 (sin aristas salientes de `Modeling`), SPEC §D.2, AC-1 y AC-4 lo prohíben; y es **imposible**: `netstandard2.1` no puede referenciar un proyecto `net10.0-windows` |
| Retirar las 8 llamadas o reescribirlas | Cambia el comportamiento observable que AC-3 exige preservar (recálculo de derivados y de colecciones). §15: «La clasificación no se "arregla" moviendo el tipo de lugar sin evidencia» |
| Partir `DomainTreeHelper`: la mitad puramente de árbol (los 4 métodos que el modelo usa) al paquete, el resto al tooling | §B.2 y §14 lo **prohíben explícitamente** para este Bolt: «Su parte puramente de árbol **no se separa** en este Bolt: separarla exigiría diseñar un seam de inyección que ningún ADR ordena» |
| Dejar `DomainTreeHelper` entero en `Modeling` con una factory inyectada | Es el «seam de inyección» que §14 declara no autorizado |
| Mover el modelo al tooling | Destruye el objetivo del Bolt y de ADR-001 §3.1 (el modelo portable es el punto) |

**La causa raíz es una lectura incompleta en el SPEC, no un defecto del 1.4.** El SPEC describió el
acoplamiento modelo↔helper en un solo sentido (el helper *usa* el modelo: diagrama §2, arco `DT → T2`) y no vio
el contrario (el modelo *llama* al helper). El texto de §B.2 —"su parte puramente de árbol"— reconoce que el
helper tiene una mitad pura, pero nunca advierte que **el modelo depende de ella**. Por eso el SPEC, tal como
está aprobado, **no es implementable**: cualquier camino viola al menos uno de sus propios criterios.

**Alcance del bloqueo:** es **único y localizado**. Todo lo demás verificado está sano: el prerequisito del §5
(suite de caracterización verde), `DBReverse.cs`/`DBReverseHelper.cs` (POCOs limpios), la tabla de mapeo
(extraíble sin ambigüedad), las excepciones (el cambio de base ya está aprobado) y el acoplamiento a la UI
(4 sitios, todos retirables). Con las 8 llamadas resueltas, el resto del SPEC es implementable como está escrito.

### 8.2 Segundo hallazgo — `System.Text.Json` en `Modeling` frente a la invariante del Bolt 001

**No es una condición de parada** (no la ejecuté hasta ese punto), pero es un conflicto entre artefactos
aprobados que el próximo V-Bounce encontrará de inmediato y que **también requiere decisión humana**.

- **SPEC-003 §A.1:** el proyecto se crea «**sin ninguna `PackageReference`**» y «la dependencia de serialización
  (`System.Text.Json`) llega con el contrato del Bolt 004, que es su dueño». §14 lo repite: «El paquete portable
  **no** incorpora todavía `System.Text.Json`». §4 (out of scope) lo tercia: «por eso **no** incorpora todavía la
  dependencia de serialización». **AC-1** lo convierte en criterio: «el proyecto no declara `ProjectReference` ni
  `PackageReference`». Y el gate de §9 «Dependency scanning / licenses» exige `Modeling`: **0 dependencias de
  terceros**.
- **Bolt 001, artefacto entregado y en revisión:** `2.0/src/Cooperator.Framework.Modeling/Cooperator.Framework.Modeling.csproj`
  declara hoy `<PackageReference Include="System.Text.Json" />`, y la suite de invariantes
  `Cooperator.Harness.Tests/BuildInvariantsTests.Modeling_declares_exactly_system_text_json` asserta que el lock
  file de `Modeling` declara **exactamente** `System.Text.Json` como dependencia directa de terceros.

**Consecuencia:** si el próximo V-Bounce cumple SPEC-003 §A.1 y retira la `PackageReference`, esa invariante del
Bolt 001 **pasa a rojo**, y no puedo corregirla (los archivos del Bolt 001 no se tocan en este V-Bounce). SPEC-003
AC-8 exige el árbol afectado y «la suite completa (caracterización + las pruebas nuevas)» en verde — la suite de
caracterización (`net48`, sobre el 1.4) y las pruebas nuevas no se ven afectadas, pero la suite de invariantes del
Bolt 001 sí. **Reconciliación necesaria, con dueño humano:** o el paquete del Bolt 001 corrige su invariante a
`Assert.Empty` (si SPEC-003 es la lectura correcta), o SPEC-003 §A.1 se revisa. No se resuelve desde el ejecutor.
Se deja constancia también de que ambas lecturas citan el mismo ADR-001: §3.4 lo nombra como «única dependencia de
serialización» de `Modeling`, mientras el NFR §5 exige «0 dependencias de terceros» — la lectura razonable es que
`System.Text.Json` no es "de terceros" por ser de la plataforma, y que SPEC-003 decide sólo **cuándo** entra.

### 8.3 G15 — verificación explícita de fuentes gobernadas

**Sin cambio material.** Se comprobó: `US-000.BOLT-003` es el mismo (su `review` de `AITL-BOLT-READY-Approval`
intacto) y sólo cambió su `status`, por los actos de este V-Bounce; `REV-001` sigue `approved`; `ADR-001`,
`ADR-002` y `ADR-008` siguen `accepted` y sin reescritura; la línea base de código sigue siendo
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto frente a ella. **El `AITL-SPEC-Approval` de la
revisión 1 sigue vigente** — pero este hallazgo exige, para avanzar, una **revisión material** del SPEC (nueva
revisión + `spec_revisions[]` + re-aprobación, G15/G16), no por cambio de fuente sino por **defecto de
clasificación descubierto en la ejecución**.

### 8.4 Comprobación de las demás condiciones de parada (§15) — todas negativas

| Condición de parada (§15) | ¿Dispara? | Comprobación |
|---|---|---|
| La tensión §14(a)/(b) se resuelve en contra durante la aprobación | **No** | `review.findings` del SPEC dice «DECISIÓN (a) — APROBADA» y «DECISIÓN (b) — APROBADA»: ambas a favor de la lectura del SPEC |
| **Un tipo del modelo depende de algo fuera del paquete más allá de los dos acoplamientos conocidos** | **SÍ** | §8.1: 8 sitios del modelo hacia `DomainTreeHelper` (tooling) |
| La suite de caracterización del Bolt 001 no existe o no corre | **No** | Corre verde: `83/83` (§9) |
| El mismo mapeo exigido por dos consumidores con formas incompatibles | **No** | Un solo consumidor presente en este árbol (el reverse relocalizado); el motor no existe todavía |
| Tentación de reescribir el reverse al reubicar | **No** | No se llegó a reubicar; `DataAccess : BaseRule` se conserva, como ordena §4 |
| El tooling necesita el shell/el editor/el empaquetado para compilar | **No evaluado** | No se llegó a construir el tooling; el riesgo de `PropertyEditors` en `net10.0-windows` queda como estaba (declarado en el finding §14(b) con su nota de control) |
| Aparece código del motor entre los archivos a reubicar | **No** | `ScriptingHost`/`ScriptsEnvironment` no están entre los archivos a portar; `CompilerException` comparte archivo con `UtilityException` y el SPEC lo excluye expresamente (se porta el archivo **sin** él) |
| Una ruptura nueva no listada en §6/E.2 | **No evaluado** | No se llegó a producir el paquete |
| Cambio material de una fuente gobernada | **No** | §8.3 |
| El digest no es reproducible | **No evaluado** | No hay conversión que digerir |
| Un gate de §9 no cierra en `pass` | **No evaluado** | No hay código que someter a gate |

### 8.5 Supuestos

- Se asumió que `DomainTreeHelper` es exactamente L76–L1056 de `DomainTree.cs` (lo confirman el `enum
  CollectionTypeEnum` que lo precede en L49–L75 y el comentario de `BaseTreeNode` en L1058–L1060) y que, por lo
  tanto, los 8 sitios listados están todos **fuera** del helper, en tipos del modelo. Todos los números de línea
  citados son de `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` en el commit congelado.
- No se asumió ninguna forma del corpus real ni se infirió ninguna decisión arquitectónica: la clasificación es
  la del SPEC y el bloqueo se reporta sin proponer un arreglo como si fuera autorizado. Las dos salidas técnicas
  se listan (§8.1) **como materia de la decisión humana**, no como plan de ejecución.

---

## 9. Verification evidence

Toda la evidencia de este V-Bounce es **de lectura y de corrida de suites existentes**. No hay build ni suite
nueva que exhibir, porque no se generó código — y así se declara.

### Prerequisito del §5 — suite de caracterización del Bolt 001, VERDE antes de la partición

```text
=== ANTES: characterization (Bolt 001, net48) ===
Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 118 ms -
Cooperator.Framework.Characterization.Tests.exe (net48)

=== ANTES: harness invariants (Bolt 001, net10.0) ===
Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20, Duration: 6 s -
Cooperator.Harness.Tests.dll (net10.0)
```

Corridas con `--no-build` sobre los ensamblados ya construidos por el Bolt 001: **no se recompiló ni se
reescribió ningún archivo de su paquete.** Es la evidencia que el SPEC §5 exige como red de la reubicación y la
que el gate «Test-first evidence» de §9 pide («corre verde **antes** de mover los tipos»).

### Evidencia de código del bloqueo (§15, segunda condición)

```text
=== metodos de DomainTreeHelper usados POR EL MODELO (linea >= 1058) ===
  DomainTreeHelper.FindEntityInTree                 -> 2 sitio(s) EN EL MODELO
  DomainTreeHelper.CorrectCLRType                   -> 2 sitio(s) EN EL MODELO
  DomainTreeHelper.CorrectSelectedCollections       -> 1 sitio(s) EN EL MODELO
  DomainTreeHelper.GetFullGenerateAsForEntity       -> 3 sitio(s) EN EL MODELO
  TOTAL: 8 sitios, en EntityNode (L1226, L1635) y PropertyNode (L1838, L2187, L2191, L2206, L2215, L2222)
```

```text
--- L1226 (EntityNode) ---
  1224: if (_GenerateAs != value) base.HaveChanges = true;
  1225: _GenerateAs = value;
  1226: DomainTreeHelper.CorrectCLRType(this.Parent);
--- L1635 (EntityNode) ---
  1633: _GenerateAsChildOf = value;
  1634: // Refresh Collections
  1635: DomainTreeHelper.CorrectSelectedCollections(this.Parent);
--- L1838 (PropertyNode) ---
  1837: base.HaveChanges = true;
  1838: DomainTreeHelper.CorrectCLRType(this.Parent.Parent);
--- L2187 (PropertyNode) ---
  2187: _CLRType = DomainTreeHelper.GetFullGenerateAsForEntity(this.Parent.Parent, this.RelatedTableName) + value.ToString().Replace("EntityList", "List");
--- L2191 (PropertyNode) ---
  2191: EntityNode relatedNode = DomainTreeHelper.FindEntityInTree(this.Parent.Parent, this.RelatedTableName);
```

### Confirmación de que el resto de la clasificación está sana

```text
=== dependencias no portables en DBReverse.cs / DBReverseHelper.cs ===
(fin)  -> 0 ocurrencias de BaseRule | Cooperator.Framework.Data|Library|Core | System.Windows* |
          System.Drawing | System.ComponentModel | [Serializable] | MSSQL

=== cualquier referencia del MODELO a Cooperator.Framework.Data|Library|Core ===
(fin)  -> 0 ocurrencias
```

Es decir: los únicos acoplamientos del modelo hacia fuera del paquete son los **4 sitios a los `PropertyEditors`
(UI — acoplamiento conocido, retirable)** y los **8 sitios a `DomainTreeHelper` (tooling — el bloqueo)**.

### Estado del árbol (rollback = no-cambio)

```text
2.0/src/Cooperator.Framework.Modeling/Model          -> False
2.0/src/Cooperator.Framework.Modeling/Reverse        -> False
2.0/src/Cooperator.Framework.Modeling/Exceptions     -> False
2.0/src/CooperatorModeler                            -> False
2.0/tests/Cooperator.Framework.Modeling.Tests        -> False
2.0/src/Cooperator.Framework.Modeling/Cooperator.Framework.Modeling.csproj
  sha256 = BCF8E17AD7B72F9939FF69ACB756AB09CE45B85E537E195CB61731BD9C135D21   (el stub del Bolt 001, intacto)

git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Build

```text
n/a — no se ejecutó build alguno: el V-Bounce se detuvo antes de la fase A y no generó código.
```

### Tests

```text
n/a — no se escribió ninguna prueba. 0 casos creados.
     (Las 103 corridas verdes registradas arriba son del arnés del Bolt 001, como evidencia del prerequisito;
      no son pruebas de este Bolt.)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2) y además el
V-Bounce no llegó a escribir ninguna prueba.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG (§3.3.1).

### Gates

> Vocabulario de T11 (`pass` / `waived` / `n/a` con razón). **Ninguno de estos `n/a` es un `pass` encubierto:
> ningún gate se ejecutó, porque no hay código que someter a gate.**

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | `n/a` — no se generó código; el V-Bounce se detuvo antes de la fase A |
| Unit / component tests | V-Bounce | `n/a` — no se escribió ninguna prueba. **El prerequisito sí se verificó:** la suite de caracterización del Bolt 001 está verde (83/83) |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — no se escribió el doble del contrato portable |
| SAST | V-Bounce | `n/a` — 0 líneas de código nuevo |
| Dependency scanning / licenses | V-Bounce | `n/a` — no se declaró ninguna dependencia (no se tocó `Directory.Packages.props`). **Nota:** el hallazgo §8.2 es precisamente sobre este gate y queda para la decisión humana |
| OWASP Top 10 — static review | V-Bounce | `n/a` — sin superficie que revisar (el SPEC ya lo declara `n/a` con razón) |
| Prompt-injection scan | V-Bounce | `n/a` — el SPEC lo declara `n/a` con razón |
| Secret-leak scan | V-Bounce | `n/a` — no se produjo ningún archivo del paquete; este MEM no contiene ningún valor de credencial (no se leyó ninguna) |
| Hallucination lint | V-Bounce | `pass` — cada afirmación de este MEM es trazable a una corrida registrada arriba o a un número de línea verificado de la fuente congelada; 0 tipos inventados y 0 afirmaciones de paridad sin corrida. Es el único gate evaluable hoy |
| IP / license provenance | V-Bounce | `n/a` — 0 código incorporado (el encabezado BSD del 1.4 se conservará al portar, según el plan registrado en §2) |
| PII / DLP | V-Bounce | `pass` — 0 datos personales, 0 filas, 0 credenciales leídas o escritas; no se leyó ningún `.coop` |
| Dependency-confusion | V-Bounce | `n/a` — 0 paquetes nuevos |
| Test-first evidence | V-Bounce | `n/a` — no se escribió el escritor ni las pruebas. **La parte que sí aplica se cumplió:** la suite de caracterización del Bolt 001 corrió **verde antes** de mover cualquier tipo, y así queda registrado |
| Behavioral reproducibility (hermetic suite) | V-Bounce | `n/a` — no hay suite nueva ni build que correr dos veces |
| Bolt-manifest validation | V-Bounce | `pass` — la entrada `v_bounces[]` de este V-Bounce valida contra `manifest-v5-bolt.schema.json` (§9, Manifest) |
| Integration tests (real dependencies) | Integration test | `n/a` — plan `n/a` por `test_integration: not-required` |
| Contract / E2E against the real providers | Integration test | `n/a` — misma razón; el contrato del formato es del Bolt 004 |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — sin superficie desplegada |
| Perf-smoke (p95/p99) | Integration test | `n/a` — sin superficie desplegada |
| Observability (logs / metrics / traces) | Integration test | `n/a` — la biblioteca no agrega telemetría |

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

**None.** No hubo parche humano ni fallback manual. La decisión humana pendiente es la **revisión material del
SPEC** (o el desbloqueo que el revisor determine), no una intervención sobre el código.

---

## 11. Evidence links

- **Diff / PR:** no hay — **no se generó código en este V-Bounce**. Los únicos cambios en el árbol son la
  actualización de estado de `US-000.BOLT-003`, este MEM y su entrada de manifiesto. No se hicieron operaciones
  de git: el commit es del humano (G34).
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — el mismo que el SPEC §3 declara. **El
  trabajo de este V-Bounce no lo mueve.**
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-003-particion-modeling.json`](../metrics/bolts/US-000.BOLT-003-particion-modeling.json)
- **Fuente de la evidencia del bloqueo (sólo lectura):**
  `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` (L44–L75, L76–L1056, L1058–fin; llamadas en
  L1226, L1635, L1838, L2187, L2191, L2206, L2215, L2222).
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/tests/Cooperator.Framework.Characterization.Tests`
  y `2.0/tests/Cooperator.Harness.Tests` (corridas `--no-build`). **No se modificó ninguno.**
- **Especificaciones que fijan este bloqueo:** SPEC-261016-0004 §15 (segunda), §B.2, §D.1, §D.2, §14, AC-1, AC-3,
  AC-4; ADR-001 §3.1.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados al reconocimiento de las fuentes y al cierre del V-Bounce; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | 1 |
| Tests created | **0** — no se escribió ninguna prueba |
| AI-generated code | **0 líneas** — ningún archivo de la entrega del SPEC fue creado |
| Loops consumed | **0 de 10** — el V-Bounce no llegó al lazo de suite: se detuvo en la condición de parada antes de generar código |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval`; el resultado no es `ready_for_review` |

---

## 13. Pending items and stubs

- [ ] **BLOQUEANTE — acción humana:** revisión material de `SPEC-261016-0004` (nueva revisión +
      `spec_revisions[]` + `AITL-SPEC-Approval`) que resuelva el acoplamiento **modelo → `DomainTreeHelper`**.
      Las dos salidas técnicas, ambas hoy vetadas por §14, son: (1) **partir el helper** — los 4 métodos que el
      modelo usa (`CorrectCLRType`, `CorrectSelectedCollections`, `GetFullGenerateAsForEntity`,
      `FindEntityInTree`) al paquete portable y el resto (`UpdateDomainTree`, `GetDomainTree`,
      `ADDNodeFromDBObject`, `ResetNamespaceCollection`) al tooling; o (2) **dejar el helper portable con un
      seam de inyección** de la factory. Cualquiera de las dos cambia la clasificación de §B.2/§D.1 y el texto de
      §14, y por eso necesita re-aprobación, no una decisión del ejecutor.
- [ ] **DECISIÓN HUMANA — segundo hallazgo (§8.2):** reconciliar SPEC-003 §A.1/§14/AC-1 (`Modeling` sin ninguna
      `PackageReference`) con la invariante entregada por el Bolt 001
      (`Modeling_declares_exactly_system_text_json`). Se resuelve corrigiendo la invariante del Bolt 001 a
      `Assert.Empty`, o revisando SPEC-003 §A.1 — nunca por el ejecutor.
- [ ] **Todo el Bolt queda pendiente**, con las fases A–E del SPEC §6 sin implementar. El reconocimiento de §2 de
      este MEM es la base: la anatomía de `DomainTree.cs`, los 6 `[Serializable]`, los atributos de diseñador con
      sus conteos exactos, los 5 `using` no portables, la tabla de mapeo de `MSSQLCommon.cs` y los destinos de
      `MSSQLProvider.cs`, `DomainTreeHelper` y `PropertyEditors/*`.
- [ ] **Riesgo abierto no evaluado:** la compilación de `PropertyEditors/*` en `net10.0-windows`
      (`System.Design` / `IWindowsFormsEditorService`), con su nota de control del finding §14(b) — el camino es
      el retiro por escalamiento vía condición de parada, **nunca** un shim de compatibilidad.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce. Siendo este V-Bounce `blocked`, la decisión
> esperable es `changes_requested` / `rejected` sobre el MEM como historia inmutable, y la respuesta es una
> **revisión material del SPEC** seguida de un **V-Bounce nuevo** con su MEM nuevo.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | `agent:dotnet-developer-reviewer-agent` |
| **Roles** | dev_validator |
| **Decision** | approved |
| **review_ready_at** | `2026-10-16T00:46:54-03:00` |
| **review.started_at** | `2026-10-16T00:51:54-03:00` |
| **review.decided_at** | `2026-10-16T00:51:54-03:00` |
| **Review evidence** | Verificación en vivo (Run N°4): los 8 sitios del modelo hacia `DomainTreeHelper` reproducidos en `1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` (EntityNode L1226/L1635; PropertyNode L1838/L2187/L2191/L2206/L2215/L2222); `DomainTreeHelper` (L76) instancia `MSSQLReverseFactory` (L201) y el SPEC rev.1 lo clasifica tooling (§B.2/§D.1); ADR-001 §3.4 nombra `System.Text.Json` para `Modeling` y la invariante `Modeling_declares_exactly_system_text_json` existe (BuildInvariantsTests.cs:160); `Model/Reverse/Exceptions` inexistentes; `Modeling.csproj` SHA-256 = stub del Bolt 001; `CooperatorModeler`/`Modeling.Tests` inexistentes; sln sin `CooperatorModeler`; `1.4/**` intacto (git diff --quiet exit 0) |
| **Comments** | Bloqueo correctamente identificado y registrado; sin código parcial; desbloqueo = revisión material del SPEC (nueva revisión + re-aprobación) + V-Bounce nuevo — **agéntico** (el revisor del SPEC es `agent:dotnet-developer-reviewer-agent` en otro acto, no requiere al humano) |
| **Findings** | Ver el bloque `review` del frontmatter (5 hallazgos Compliant) |
| **acknowledged_without_comment** | false |
| **acknowledgment_reason** | — |
