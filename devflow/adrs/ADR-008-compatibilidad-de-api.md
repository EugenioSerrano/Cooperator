---
id: "ADR-008"
title: "Compatibilidad de API de Cooperator 2.0"
date: "2026-10-15"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-architect-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "accepted" # draft | accepted | rejected | deprecated | superseded
decision_makers: ["architect", "tech_lead"] # Roles that participated in the decision
sources: # REVs, DISCs, previous ADRs, interviews that motivated this decision
  - "devflow/reviews/REV-001-analisis-migracion-cooperator.md"
  - "devflow/analysis/open-questions/OQ-006-politica-de-compatibilidad.md"
  - "devflow/adrs/ADR-001-layout-y-empaquetado-cooperator-2.md"
  - "devflow/adrs/ADR-002-formato-coop2-y-migrador.md"
  - "devflow/adrs/ADR-003-motor-scriban-headless-y-cli.md"
  - "devflow/adrs/ADR-005-provider-de-datos.md"
  - "devflow/adrs/ADR-007-politica-criptografica.md"
  - "1.4/Cooperator.Framework.Core/BaseObject.cs"
supersedes: [] # ADRs this one replaces (§3.5 conflict resolution may supersede several)
conflicts_with: [] # ADRs whose decisions this ADR contradicts (optional; must be resolved by a superseding ADR before it governs a SPEC — §2.8)
tags: ["2.0", "api-compatibility", "semver", "serialization", "breaking-changes", "migration-notes", "netstandard2.1"]
nfrs: ["serialization-surface", "breaking-change-traceability"] # NFRs governed by this ADR (performance, security, availability, etc.)
waiver: # Only for gate-override ADRs (§3.6)
  gate: "" # Gate being waived
  reason: "" # Why the gate is waived
  owner: "" # Person responsible for the exception
  compensating_control: "" # Control that mitigates the waived gate
  expires_at: "" # YYYY-MM-DD — mandatory, no open-ended waivers
review_ready_at: "2026-10-15T23:06:38-03:00" # When this exact version is submitted for review (§3.0)
review: # AITL-ADR-Approval — decisión de human:eugenioserrano; transcrita por el Coordinador (writing is not signing, §3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "human:eugenioserrano", role: "tech_lead", model: null}]
  written_by: [{actor: "agent:avenga-devflow", role: "coordinator", model: "deepseek/deepseek-flash"}]
  started_at: "2026-10-15T23:11:40-03:00"
  decided_at: "2026-10-15T23:11:40-03:00"
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "Sin observaciones adicionales; aprobación directa en la sesión interactiva del 2026-10-15 (revisión de pares completa; ruteo §6 cerrado)."
---

<!-- §3.15 language policy: YAML keys, status enums, IDs and section headings (##) in English (the schema);
     ADR title and prose in the project's content_language (devflow/LANGUAGE); `AITL-ADR-Approval` is never translated.
     ⚠️ AITL-ADR-Approval: this ADR stays a DRAFT — it cannot govern a SPEC, establish an NFR or authorize an
     exception — until an Architect / Tech Lead records the decision in the `review` block (§2.8, §3.5); no
     approval is inherited from related artifacts.
     ⚠️ ADR conflicts (§2.8): check the decision log for active ADRs contradicting this one before approval; a
     conflict is declared in `conflicts_with` and resolved by a superseding ADR.
     ⚠️ Content scope (§2.8): an ADR records architecture decisions and constraints, not functional behaviour —
     functional requirements live in the feature US and its ACs. -->

# ADR-008 — Compatibilidad de API de Cooperator 2.0

| Field          | Value |
|----------------|-------|
| **Status**     | accepted |
| **Decision-makers** | architect, tech_lead |
| **Sources**    | REV-001 (aprobado 2026-10-15), OQ-006 (respondida 2026-10-15), ADR-001/ADR-002/ADR-003 (accepted) y ADR-005/ADR-007 (mismo ciclo) |
| **Supersedes** | None |
| **Conflicts with** | None — **formaliza como política** lo que los ADR accepted ya decidieron en concreto (ADR-001 §3.2, ADR-003 §3.1) y no reabre el escape-hatch que ADR-001 §3.2 dejó definido: lo vuelve verificable |

---

## 1. Context

`REV-001` §6 ítem 13 ruteó a un ADR la pregunta que F-05 dejó abierta: **qué se hace con `[Serializable]` y con los constructores `SerializationInfo` en la superficie pública**. El inventario de 1.4, verificado por búsqueda sobre los 168 `.cs`:

| Dónde | Qué hay |
|---|---|
| `Cooperator.Framework.Core/BaseObject.cs:64` | `[Serializable]` sobre la clase base **abstracta** de todos los objetos de negocio (`IObject`), sin constructor de serialización: serializa automáticamente sus campos |
| Los 4 proyectos, carpeta `Exceptions/` | **35 tipos `[Serializable]`** con el patrón `protected X(SerializationInfo, StreamingContext)` (36 apariciones contando la base), uno por excepción del framework |
| `Utility/DBReverseHelper/**` | **6 clases `[Serializable]`** — el grafo del modelo: `BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery` |
| `Tools/CooperatorModeler/Snapshot.cs:40` | `[Serializable]` sobre el contenedor que se persistía con `BinaryFormatter` (el `.coop`, F-41) |
| `Templates/CSharpClasses/{Entity,Object}.cs:20` | `[Serializable]` en los partials **editables** de las entidades generadas, más `[DataContract]`/`[CollectionDataContract]` en los generados `*.Auto.cs` |

**Dos datos verificados que reducen el problema a una decisión simple**: (1) **ninguno** de esos constructores lleva datos propios —no hay una sola llamada a `info.AddValue`/`info.GetValue` en el árbol—, así que son el boilerplate que las plantillas de Visual Studio generaban: **quitarlos no pierde información**; y (2) **no existe ninguna implementación de `ISerializable`** ni override de `GetObjectData` más allá de esos constructores, de modo que la serialización binaria de tipos del framework nunca fue un contrato explícito, sólo un atributo heredado.

**Por qué hay que decidirlo ahora**: `[Serializable]` compila en `netstandard2.1`, pero en el runtime objetivo emite obsolescencia (`SYSLIB0051`) y perpetúa la dependencia conceptual de `BinaryFormatter`, que está **eliminado en .NET 9+** (F-42/F-43). Es decir: un consumidor de la 2.0 (que por OQ-006 es .NET 10+) **no puede** serializar binariamente esos tipos aunque el atributo esté. La superficie promete algo que el runtime ya no puede cumplir.

**Lo ya decidido que este ADR formaliza**: ADR-001 §3.2 retira `[Serializable]`/`SerializationInfo` de la superficie pública con un **escape-hatch** ("excepción justificada por Bolt, nunca como mecanismo de persistencia nuevo"); ADR-003 §3.1 declara las entidades generadas **POCOs sin `[Serializable]`**; ADR-002 convirtió la persistencia del modelo a **`.coop2` (JSON)**, con los derivados recalculados al leer y no serializados; ADR-005 §3.3 preservó la **forma** BCL-typed del contrato que consume el código generado mientras retiraba implementación de la superficie; ADR-007 retiró la API estática de `Cryptography`; y OQ-006 fijó que **no hay compatibilidad binaria 1.4 → 2.0** (y ADR-001 §3.3 que hay SemVer con `AssemblyVersion` fijo en `2.0.0.0` para toda la línea 2.0.x).

**Frontera**: decide la política de ruptura entre 1.4 y 2.0 y la de evolución dentro de 2.0.x, el tratamiento de `[Serializable]`/`SYSLIB0051`, qué reemplaza al estado serializado y qué promete la superficie que queda. **No** decide el contenido del set de plantillas (ADR-003), ni el esquema del `.coop2` (ADR-002), ni los formatos de configuración (ADR-004) o de valores protegidos (ADR-007), ni qué API se retira por otras razones ya decididas (`SqlHelper` en ADR-005 §3.3, `Cryptography` en ADR-007 §3.1).

---

## 2. Alternatives considered

### Alternative A — Ruptura limpia en 2.0.0, documentada, con política de evolución explícita (✅ Selected)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Es la única opción coherente con el runtime objetivo: los tipos que prometen serialización binaria no pueden cumplirla en .NET 10, y mantener el atributo es prometer algo falso. La remoción es **gratuita en información** (verificado: los constructores no llevan datos) y **sin costo de compatibilidad real** (no hay una sola implementación de `ISerializable`, y la compatibilidad binaria con 1.4 ya está descartada por OQ-006). A cambio, se gana una superficie pública **explicable**: cada miembro existe por una razón y la evolución dentro de 2.0.x queda gobernada por reglas escritas. |
| **Cons** | Un consumidor que serializara binariamente entidades derivadas de `BaseObject` (posible en 1.4, imposible en .NET 10 de todos modos) pierde incluso la ilusión de que podía hacerlo. Obliga a mantener las notas de migración al día ruptura por ruptura, con la disciplina de un gate que las cruce contra el diff de API. |

### Alternative B — Conservar `[Serializable]` y los constructores "por las dudas"

| Aspect   | Detail |
|----------|--------|
| **Pros** | Cero trabajo: los constructores ya existen, compilan, y quitar un atributo siempre parece más riesgoso que dejarlo. |
| **Cons** | Es el caso de manual de superficie muerta: nadie puede usarla en el runtime objetivo, emite `SYSLIB0051` en cada build (o exige supresiones), arrastra el vocabulario de `BinaryFormatter` a la documentación de la 2.0 y mantiene 36 constructores que ningún test puede justificar. La deuda se paga igual el día que alguien quiera "endurecer" la superficie, con más código encima. |

### Alternative C — Shim de compatibilidad binaria (mantener todo + un `SerializationBinder`/convertidor)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Permitiría leer estados serializados binariamente con la 1.4 desde la 2.0, preservando escenarios de datos históricos. |
| **Cons** | `BinaryFormatter` está **deshabilitado por defecto en .NET 8 y eliminado en .NET 9+**: el shim exigiría un opt-in inseguro y deserialización de datos no confiables — exactamente el riesgo que el ADR-002 eliminó del modelo. Habilita un canal peligroso para servir a un caso del que no hay evidencia (no hay `ISerializable`, no hay payload). |

### Alternative D — Doble superficie (mantener `Cooperator.Framework.*` legado intacto y agregar `…2` moderno)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Nadie se rompe nunca: cada consumidor elige su superficie y la migración se hace a su ritmo. |
| **Cons** | Duplica paquetes, tests y documentación para una compatibilidad que **no puede existir** (el código 1.4 corre en .NET Framework y los ensamblados 2.0 son `netstandard2.1`: no conviven en el mismo proceso, ADR-001 §3.2). El consumidor no gana una opción: gana dos cosas que mantener, y el "legado" queda congelado de todos modos — que es justamente el 1.4 sin cambios. |

### Alternative E — Retiro asimétrico: conservar `[Serializable]` sólo en las excepciones

| Aspect   | Detail |
|----------|--------|
| **Pros** | Mantiene la convención histórica de .NET para excepciones (serializables para escenarios de remoting/persistencia de errores) con un costo aparentemente bajo: son clases sin estado propio. |
| **Cons** | Es la mitad del problema sin ninguna de las ventajas: sigue emitiendo `SYSLIB0051`, sigue exigiendo los 35 constructores y **no** protege nada útil, porque las excepciones del framework no llevan datos propios y `BinaryFormatter` no existe en el runtime objetivo. Además deja una regla ambigua ("las excepciones sí, el resto no") que cada Bolt tendría que re-litigar. |

---

## 3. Decision

Se adopta la **Alternativa A**: la 2.0 rompe la superficie de serialización **de una vez y documentada**; el framework **no promete ningún mecanismo de serialización** sobre sus tipos; la evolución dentro de 2.0.x se gobierna por reglas explícitas; y cada ruptura tiene que estar en las notas de migración con su reemplazo, verificado por un gate contra el diff de API.

### 3.1 Política de ruptura 1.4 → 2.0

- **No hay compatibilidad binaria ni de comportamiento prometida** entre 1.4 y 2.0 (OQ-006): cambian runtime, TFM y modelo de configuración. La compatibilidad que **sí** se promete es de **fuente** y acotada a una meta: *el código del consumidor que no usa las features cortadas compila y opera con cambios mínimos*, y en particular la **forma** del contrato que consumen los `*.Auto.cs` generados (ADR-005 §3.3).
- **Ruptura limpia, no shims de compatibilidad**: nada de type-forwarders, dobles superficies ni capas "compat". Un consumidor que no pueda migrar se queda en el **1.4 congelado**, que es una opción soportada y explícita (ADR-001 §3.2), no un accidente.
- **Toda ruptura se documenta en las notas de migración** de la 2.0, con tres datos obligatorios: (1) qué miembro o atributo desaparece; (2) **por qué** (la decisión que lo ordena); (3) **con qué se reemplaza** en el código del consumidor (o "sin reemplazo, porque el escenario no existe en el runtime objetivo"), y el ítem del ruteo/Bolt que lo ejecuta.
- **Sin supresiones globales de advertencias**: la política no se implementa apagando analizadores. `SYSLIB0051` no debe aparecer en el build de la 2.0 (ver §3.4).

### 3.2 Qué se retira, exactamente

| Superficie | Acción | Decisión que lo ordena |
|---|---|---|
| `BaseObject` (Core) — `[Serializable]` sobre la clase base | **Se retira** | ADR-001 §3.2 (formalizado acá) |
| 35 excepciones del framework — `[Serializable]` + `protected X(SerializationInfo, StreamingContext)` | **Se retiran ambos**, en los cuatro proyectos | ADR-001 §3.2; verificado que no llevan datos: la remoción no pierde información |
| Grafo del modelo (`BaseTreeNode`, `EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery`) | **Se retira** al migrar a `Modeling` como POCOs; su persistencia es el **`.coop2`** (JSON, con derivados recalculados al leer) | ADR-001 §3.1, ADR-002 §3.1 |
| `Snapshot` (Modeler) — `[Serializable]` del contenedor binario | **Desaparece** con el formato: el modelo es el `.coop2` y el migrador es lo único que lee binario | ADR-002 §3.2 |
| Entidades generadas — `[Serializable]` en los partials de plantilla | **Se retira**: son POCOs | ADR-003 §3.1 (F-43) |
| Entidades generadas — `[DataContract]`/`[CollectionDataContract]` en los `*.Auto.cs` | **Se retiran**: criterio de aceptación **vinculante** del **Bolt del set de plantillas (ADR-003)** — ver §3.7, que fija el criterio, el dueño y su justificación | Este ADR (§3.7). ADR-003 es accepted e inmutable: el criterio se ejecuta en **su** Bolt, no enmendando su texto |

### 3.3 Reemplazos: qué ocupa el lugar del estado serializado

- **El framework no expone ningún serializador.** No hay una API de serialización genérica que reemplazar: los contratos de datos del producto son **datos explícitos** —el esquema `.coop2` (ADR-002) para el modelo— y todo lo demás es responsabilidad del consumidor (que elige STJ, su ORM o lo que necesite). El framework no prescribe ni reexporta un mecanismo de serialización.
- **Ningún dato que viajaba por `SerializationInfo` necesita reemplazo** (verificado: no hay payload). Para el futuro la regla es explícita: si una excepción necesita transportar contexto, ese contexto se expone como **propiedades tipadas** en la excepción (legibles, testeables y visibles en logs estructurados), nunca como un diccionario serializado.
- **Diagnóstico**: las excepciones conservan su jerarquía y sus constructores públicos (`CoreException`/`DataException`/…), que es lo que el consumidor captura; lo que desaparece es sólo el constructor protegido de serialización.
- **Entidades del consumidor**: si un consumidor necesita persistir o transportar entidades, lo declara **en su lado** (sus partials/propios DTOs). El framework deja de imponer un atributo que sugiere un mecanismo.

### 3.4 `[Serializable]` y `SYSLIB0051` en adelante

- **Prohibido** en la superficie pública de los paquetes 2.0: `[Serializable]`, `ISerializable`, `GetObjectData`, y todo constructor `(SerializationInfo, StreamingContext)`.
- **Prohibido** cualquier uso de `BinaryFormatter` en el árbol 2.0 (librerías, tooling, migrador y el código generado): la única excepción del producto es el **migrador** del ADR-002, que corre en `net48` contra `BinaryFormatter` **por diseño y con fecha de retiro** (los `.coop` originales no se conservan para siempre).
- **Cero `SYSLIB0051` en el build**: la política se verifica en CI, no se silencia. Un caso que sobreviva por el escape-hatch (§3.6) se suprime **en el ámbito más estrecho posible** (`#pragma warning disable` sobre el miembro, con comentario que cita la excepción y su vencimiento), jamás a nivel de proyecto o de solución.
- **Atributos de contrato** (`DataContract`, `JsonSerializable`, etc.) no se agregan a los tipos del framework: si un tipo necesita participar de un mecanismo, el consumidor lo declara en su lado.

### 3.5 Evolución dentro de la línea 2.0.x

- **SemVer** (ADR-001 §3.3) con la superficie pública como contrato:
  - **Patch (2.0.x)** — sólo correcciones: **cero** cambios de superficie (ni adiciones que alteren el contrato visible, ni cambios de comportamiento no documentados).
  - **Minor (2.x.0)** — adiciones compatibles y **deprecaciones**: un miembro que se va a retirar en el próximo major se marca **obsoleto** (atributo `[Obsolete]` con mensaje que nombra el reemplazo) y entra en las notas, con al menos **una minor de aviso** antes de su retiro.
  - **Major (3.0)** — rupturas, con el mismo procedimiento de documentación que §3.1.
- **`AssemblyVersion` se mantiene fijo** durante toda la línea 2.0.x (`2.0.0.0`): un parche no provoca binding nuevo para el consumidor (ADR-001 §3.3). `FileVersion`/`InformationalVersion` llevan la versión publicada.
- **La superficie se congela por escrito**: el gate de §5 compara el API público real contra el de la versión anterior del paquete y contra 1.4, y el resultado es lo que las notas de migración deben cubrir — sin huecos y sin invenciones.

### 3.6 Criterios de aceptación para las excepciones (escape-hatch de ADR-001 §3.2)

Conservar `[Serializable]`/`SerializationInfo` en un tipo requiere, **todo junto**, en el Bolt que lo pida:

1. **Un consumidor identificado con evidencia** (código, ticket o declaración del dueño del producto) que hoy haga eso y no pueda migrar; "podría servirle a alguien" no es evidencia.
2. **La imposibilidad demostrada** de resolverlo con un mecanismo portable (JSON/propiedades tipadas/contratos explícitos): el motivo por el que la vía normal no sirve.
3. **Nunca como mecanismo de persistencia nuevo**: el escenario tiene que ser de **compatibilidad** con un uso existente, no la puerta de entrada a un formato nuevo (ADR-001 §3.2).
4. **Un test que reproduce el escenario** y que se retira junto con la excepción.
5. **Vencimiento declarado**: la excepción vale hasta el próximo major y queda registrada (nombre del tipo, motivo, consumidor, fecha) en las notas de migración y en el MEM del Bolt.

Por defecto la respuesta es **no**: sin los cinco puntos, el miembro se retira y el consumidor recibe en las notas de migración su camino alternativo.

### 3.7 Criterio vinculante sobre el set de plantillas (ADR-003)

ADR-003 es `accepted` e **inmutable**: su §3.1 fija que las entidades generadas son POCOs sin `[Serializable]`, y este ADR no puede enmendarlo. Por eso la caída de `[DataContract]`/`[CollectionDataContract]` de los `*.Auto.cs` se establece acá como **criterio de aceptación del Bolt del set de plantillas**, con dueño explícito:

- **Qué se exige**: las entidades y colecciones generadas **no** emiten atributos de contrato de serialización (`[DataContract]`, `[CollectionDataContract]`). Una excepción requiere un **contrato de consumidor identificado** que los use, con la misma evidencia y las mismas reglas que el escape-hatch de §3.6 (aplicadas al set, no a la superficie del framework). El cambio es **sólo el retiro de atributos**: no toca nombres, miembros ni comportamiento de los artefactos generados, así que no altera el contrato que el consumidor compila.
- **Dueño**: el **Bolt del set de plantillas** (el mismo que ejecuta las plantillas del motor de ADR-003: es quien puede decidir qué se emite). Su MEM registra el cambio como **ruptura** y su entrada correspondiente en las notas de migración (§3.1).
- **Por qué es vinculante y no una recomendación**: un atributo de contrato en el código generado es una **promesa de superficie** del producto hacia el consumidor. El framework ya no respalda ningún mecanismo de serialización (§3.3), así que el atributo sólo puede inducir a un consumidor a construir sobre un mecanismo que nadie sostiene — exactamente la superficie muerta que este ADR prohíbe. Dejarlo como recomendación era confiar en que el set lo hiciera por convicción, no por criterio de aceptación.

### 3.8 Interplay con ADR-001, ADR-002, ADR-003, ADR-005 y ADR-007

- **ADR-001**: formaliza su §3.2 (retiro + escape-hatch) y su §3.3 (SemVer/`AssemblyVersion`), sin agregar ni quitar paquetes.
- **ADR-002**: el reemplazo del estado serializado del modelo es **su** formato; los derivados no se serializan y se recalculan al leer (una consecuencia directa de retirar la serialización automática de campos).
- **ADR-003**: hereda la regla para las entidades generadas (POCOs sin `[Serializable]`) y, por §3.7, su **Bolt del set de plantillas** recibe como **criterio de aceptación** el retiro de `[DataContract]`/`[CollectionDataContract]` de los `*.Auto.cs` — ADR-003 queda intacto como documento; lo que cambia es lo que su Bolt debe cumplir.
- **ADR-005**: la forma BCL-typed del contrato preservada es exactamente el límite de lo que este ADR promete como compatibilidad de fuente; los retiros de implementación (`SqlHelper` → `internal`) se documentan con el mismo procedimiento de §3.1.
- **ADR-007**: su retiro de `Cryptography.Encrypt(string)`/`Decrypt(string)` es otra ruptura que entra por el procedimiento de §3.1 (queda como ejemplo de que el registro de rupturas no es "sólo serialización").

---

## 4. Consequences

**Positive:**

- La 2.0 deja de prometer serialización binaria en un runtime donde no existe: la superficie pública pasa a ser **honesta** y explicable miembro por miembro.
- La remoción es **verificada y sin pérdida**: los 36 constructores no llevan datos y no hay implementaciones de `ISerializable`, así que no hay información ni contrato efectivo detrás de lo que se elimina.
- El build queda limpio: sin `SYSLIB0051` y sin el vocabulario de `BinaryFormatter` en la documentación del framework (salvo el migrador, acotado y con fecha).
- La evolución dentro de 2.0.x deja de ser improvisación: patch sin cambios, minor con deprecación avisada, major para romper; y el `AssemblyVersion` fijo evita que un parche obligue a los consumidores a re-resolver bindings.
- El escape-hatch queda con criterios duros y verificables, lo que evita que "excepción justificada" se convierta en la puerta por la que vuelve la superficie muerta.

**Trade-offs:**

- Quien hoy serialice entidades derivadas de `BaseObject` (o persista excepciones) **no tiene un reemplazo automático**: debe elegir un mecanismo propio. La compatibilidad binaria con 1.4 no era una opción real, pero el cambio igual se percibe como pérdida.
- Mantener las notas de migración al día es trabajo **continuo**: cada Bolt que toque superficie tiene que registrar su ruptura, y ese registro se verifica con un gate (si el gate no se ejecuta en CI, la política se degrada a buena intención).
- Las entidades generadas pierden los atributos de contrato de serialización por **criterio de aceptación del Bolt del set de plantillas** (§3.7): si ese Bolt no los retira, el consumidor seguirá viendo una superficie que sugiere un mecanismo que el framework ya no respalda. Es el precio de que ADR-003 sea inmutable: la política se ejecuta donde se emite el código, no enmendando el ADR.
- Renunciar a shims significa que no hay red de contención para un consumidor que se equivoque de versión: la contención es el 1.4 congelado, y elegir mal se descubre en compilación (lo cual es preferible a descubrirlo en runtime).

**Technical debt:**

- El `[DataContract]`/`[CollectionDataContract]` de los `*.Auto.cs` queda como deuda **hasta que se ejecute el criterio de aceptación de §3.7** en el Bolt del set de plantillas (dueño explícito): la política ya dice cuál es el destino y quién lo cierra.
- El registro de rupturas depende de que exista el gate de comparación de API (§5): hasta que esté en CI —y con la **línea base 1.4** producida (§5)—, la trazabilidad es manual y por lo tanto falible.
- El migrador del ADR-002 seguirá siendo la única pieza que carga `BinaryFormatter` mientras existan `.coop` sin convertir; su retiro está atado a la validación del release y al archivo del corpus.
- Las notas de migración de la 2.0 acumulan en un solo documento rupturas de naturaleza muy distinta (serialización, `SqlHelper` a `internal`, `Cryptography` estático, `App.config` → `appsettings.json`, splits de paquetes): sin una estructura por área, el consumidor las leerá peor de lo que la política exige.

---

## 5. Applicable NFRs

| NFR | Description | Threshold | How it is measured |
|-----|-------------|-----------|-------------------|
| Serialization surface | La superficie pública de los paquetes 2.0 no promete ningún mecanismo de serialización por atributo, y el build no arrastra obsolescencia de `BinaryFormatter` | **0** `[Serializable]`, `ISerializable`, `GetObjectData` o constructores `(SerializationInfo, StreamingContext)` en los paquetes 2.0; **0** advertencias `SYSLIB0051`; **0** usos de `BinaryFormatter` fuera del migrador (ADR-002) | Gate de API/atributos en CI (analizadores + inspección del API público publicado) y build con advertencias tratadas como error; lista blanca explícita de una sola excepción: `Cooperator.Framework.Migration` (`net48`, con fecha de retiro) |
| Breaking-change traceability | Toda ruptura de superficie frente a 1.4 y toda deprecación dentro de 2.0.x está documentada con reemplazo y con la decisión que la ordena | **100 %** de las diferencias del API público cubiertas por las notas de migración; **0** rupturas sin entrada; **0** entradas sin ruptura asociada | Comparación de API público cruzada contra las notas de migración, en CI, con **`Microsoft.DotNet.PackageValidation`** (`EnablePackageValidation` + `PackageValidationBaselineVersion` para la línea 2.0.x anterior) y el **dotnet tool `Microsoft.DotNet.ApiCompat`** para contrastar contra la línea base 1.4; la discrepancia falla el gate |

**Cómo se instrumenta el gate (dueño y paso preparatorio, no obvio):**

- **Paso preparatorio — línea base 1.4**: el 1.4 **nunca se distribuyó como paquete** (se consumía por DLLs con `HintPath` + `CooperatorAssemblies`, ADR-001 §3.1), así que no hay `PackageValidationBaselineVersion` que apuntar. Antes de poder comparar hay que **producir un artefacto de referencia 1.4** a partir de los binarios congelados (`AssemblyVersion 1.4.5.0`, firmados con `CooperatorFramework.snk`) —un *reference assembly* o un paquete *baseline* sólo para el gate— y versionarlo junto a los fixtures. Sin ese paso el gate no puede existir, y hay que hacerlo **una vez**, en el primer Bolt que lo implemente.
- **Dueño**: el **Bolt del arnés (F-50)** es el dueño de dejarlo **en CI como criterio de aceptación** (junto con el build reproducible y los fixtures); a partir de ahí, cada Bolt que toque superficie pública es responsable de que la comparación siga verde y de registrar su entrada en las notas.
- **Qué NO es**: el gate **no** reemplaza las notas de migración (detecta la diferencia, no explica el reemplazo) ni autoriza rupturas: una diferencia no documentada **falla**, no se aprueba por existir en el diff.

---

## 6. References

- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — revisión aprobada el 2026-10-15; §6 ítem 13; hallazgo F-05 (`[Serializable]`/`SerializationInfo` en toda la jerarquía, obsolescencia y dependencia conceptual de `BinaryFormatter`); F-42/F-43 (formato y serialización binaria erradicados del modelo y de las plantillas).
- [`OQ-006`](../analysis/open-questions/OQ-006-politica-de-compatibilidad.md) — moderno-only (.NET 10+): no hay compatibilidad binaria 1.4 → 2.0 y el 1.4 queda congelado como opción soportada.
- [`ADR-001`](ADR-001-layout-y-empaquetado-cooperator-2.md) — accepted: retiro de `[Serializable]`/`SerializationInfo` y escape-hatch (§3.2), SemVer y `AssemblyVersion` fijo (§3.3), sin `-windows` ni shims.
- [`ADR-002`](ADR-002-formato-coop2-y-migrador.md) — accepted: la persistencia del modelo es el `.coop2` (JSON) y los derivados se recalculan al leer.
- [`ADR-003`](ADR-003-motor-scriban-headless-y-cli.md) — accepted: entidades generadas como POCOs sin `[Serializable]` (§3.1); el criterio vinculante sobre los atributos de contrato de los `*.Auto.cs` se ejecuta en el Bolt de su set de plantillas (§3.7 de este ADR).
- [`ADR-005`](ADR-005-provider-de-datos.md) y [`ADR-007`](ADR-007-politica-criptografica.md) — del mismo ciclo: la compatibilidad de fuente que se preserva (forma BCL-typed) y ejemplos de ruptura que entran por el procedimiento de §3.1.
- Evidencia de código 1.4: `Core/BaseObject.cs:64` (`[Serializable]` sin constructor de serialización), `Core|Data|Library|Utility/Exceptions/*.cs` (35 tipos con el patrón `SerializationInfo`), `Utility/DBReverseHelper/**` (6 clases del grafo), `Tools/CooperatorModeler/Snapshot.cs:40`, `Templates/CSharpClasses/{Entity,Object}.cs:20` y `{Entity,Object}.Auto.cs` (`[DataContract]`/`[CollectionDataContract]`); búsqueda de `info.AddValue`/`info.GetValue`: **0 resultados** (los constructores no transportan datos).

---

## 7. AITL-ADR-Approval

> **AITL-ADR-Approval** (Avenga DevFlow §2.8, §3.0, §3.5). An ADR does not
> become `accepted` — and therefore governing — without the approval of an
> Architect / Tech Lead. This ADR is the **source of truth for its own
> approval** (recorded in the `review` frontmatter block with review
> evidence); when it governs a SPEC revision, its path appears in that
> revision's `sources`. ADR approvals are never copied to the Bolt manifest.

| Field | Value |
|-------|-------|
| **Architect / Tech Lead** | `human:eugenioserrano` |
| **Role** | tech_lead |
| **Decision** | **approved** |
| **review_ready_at** | `2026-10-15T23:06:38-03:00` |
| **review.started_at** | `2026-10-15T23:11:40-03:00` |
| **review.decided_at** | `2026-10-15T23:11:40-03:00` |
| **Findings** | Sin hallazgos adicionales — `acknowledged_without_comment: true` (motivo en el frontmatter) |
