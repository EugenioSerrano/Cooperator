---
id: "ADR-002"
title: "Formato .coop2 y migrador del modelo"
date: "2026-10-15"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-architect-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "accepted" # draft | accepted | rejected | deprecated | superseded
decision_makers: ["architect", "tech_lead"] # Roles that participated in the decision
sources: # REVs, DISCs, previous ADRs, interviews that motivated this decision
  - "devflow/reviews/REV-001-analisis-migracion-cooperator.md"
  - "devflow/analysis/open-questions/OQ-004-formato-del-coop-json.md"
  - "devflow/analysis/open-questions/OQ-005-alcance-features-en-riesgo.md"
  - "devflow/analysis/open-questions/OQ-001-particion-de-utility.md"
  - "devflow/analysis/open-questions/OQ-006-politica-de-compatibilidad.md"
  - "devflow/adrs/ADR-001-layout-y-empaquetado-cooperator-2.md"
  - "1.4/Tools/CooperatorModeler/Snapshot.cs"
supersedes: [] # ADRs this one replaces (§3.5 conflict resolution may supersede several)
conflicts_with: [] # ADRs whose decisions this ADR contradicts (optional; must be resolved by a superseding ADR before it governs a SPEC — §2.8)
tags: ["2.0", "coop2", "format", "json", "migration", "modeler", "codegeneration", "deprecation"]
nfrs: ["determinism", "secret-free-model"] # NFRs governed by this ADR (performance, security, availability, etc.)
waiver: # Only for gate-override ADRs (§3.6)
  gate: "" # Gate being waived
  reason: "" # Why the gate is waived
  owner: "" # Person responsible for the exception
  compensating_control: "" # Control that mitigates the waived gate
  expires_at: "" # YYYY-MM-DD — mandatory, no open-ended waivers
review_ready_at: "2026-10-15T22:22:28-03:00" # When this exact version is submitted for review (§3.0)
review: # AITL-ADR-Approval — decisión de human:eugenioserrano; transcrita por el Coordinador (writing is not signing, §3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "human:eugenioserrano", role: "tech_lead", model: null}]
  written_by: [{actor: "agent:avenga-devflow", role: "coordinator", model: "deepseek/deepseek-flash"}]
  started_at: "2026-10-15T22:32:47-03:00"
  decided_at: "2026-10-15T22:32:47-03:00"
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "Sin observaciones adicionales; aprobación directa en la sesión interactiva del 2026-10-15 (revisión de pares completa)."
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

# ADR-002 — Formato `.coop2` y migrador del modelo

| Field          | Value |
|----------------|-------|
| **Status**     | accepted |
| **Decision-makers** | architect, tech_lead |
| **Sources**    | REV-001 (aprobado 2026-10-15), OQ-004 y OQ-005 (respondidas 2026-10-15), OQ-001/OQ-006 (contexto de layout), ADR-001 (borrador del mismo ciclo: slots) |
| **Supersedes** | None |
| **Conflicts with** | None — no hay ADRs previos (`adrs/INDEX.md`) y este ADR no contradice el ADR-001 en curso: respeta sus slots y su orden de migración |

---

## 1. Context

`REV-001` (§6 ítem 3, hallazgos F-41/F-42/F-43) estableció que el `.coop` es **un blob `BinaryFormatter` de un `Snapshot`**: `Savemodel()` serializa el objeto completo (`MainForm.cs:203-222`) y `OpenCoopFile()` lo deserializa (`MainForm.cs:261-302`), validando sólo un string `ModelerVersion` que el propio Modeler **reescribe a la fuerza** al abrir (`MainForm.cs:280-281`) y fallando con el mensaje único `"File not valid or file corrupt."`. El grafo serializado son las clases `[Serializable]` de `Snapshot.cs` (configuración del modelo: `DataBaseName`, `SPPrefix`, nombres de proyecto, `DeployFolder`, `Language`, `PostGenerationScript`; más `ConnectionString`, `FileName`, `HaveChanges`, `GenerateCheckForToken` y los flags `GenerateAutoFileOnly`/`StampDateAndTimeOnAutoFiles`, que §3.1 deja **fuera** del formato — los dos últimos, retirados por ADR-003) y de `DomainTree.cs` (`EntityNode`, `PropertyNode`, `RelatedField`, `ToStringStruct`, `DatabaseQuery`), colgado de un nodo raíz sintético `new EntityNode(null, "Model")` (`DomainTree.cs:211`) cuyos hijos son las entidades.

Tres problemas convergen (F-41): **(1)** un binario no se mergea ni se revisa — es la motivación declarada del dueño; **(2)** `BinaryFormatter` está **eliminado en .NET 9+**, de modo que el Modeler migrado a `net10.0-windows` no podría leer ni escribir sus propios archivos sin un opt-in inseguro; **(3)** deserializar de un archivo que un tercero puede enviar es una vulnerabilidad conocida. El pedido del dueño es un formato **texto, versionado y amigable para agentes**: diffeable y mergeable.

**Restricciones fijadas aguas arriba** (`OQ-004`, opción A, 2026-10-15): el formato es **`.coop2`** (extensión nueva; el `.coop` queda como legado binario); el migrador es un **entregable obligatorio** que corre sobre **.NET Framework 4.8** con `BinaryFormatter` aún disponible, **antes** de migrar el Modeler; la operación es **irreversible**; y **la ubicación del migrador quedó diferida a este ADR** (es la decisión de §3.2). `OQ-005` deprecó VB.NET, Oracle y el export a Excel, y `OQ-001`/`OQ-006` fijan el contexto de layout: el contrato del formato vive en `Cooperator.Framework.Modeling` (`netstandard2.1`), el motor lo consume (ADR-003) y los consumidores son .NET 10+.

**Frontera**: decide el formato `.coop2`, la política de evolución del esquema, la ubicación/UX del migrador, cómo lee el binario legado, cómo valida y qué pasa con el contenido deprecado. **No** decide el diseño interno del motor de generación (ADR-003), ni el mapa de paquetes/TFMs (ADR-001), ni el esquema JSON propiedad-por-propiedad, que es el contrato normativo que produce el Bolt de `Modeling` respetando las reglas de §3.1.

---

## 2. Alternatives considered

### Alternative A — JSON canónico en `Modeling` + migrador standalone `net48` (✅ Selected)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Un formato texto con orden y codificación canónicos habilita diff/merge reales y edición por agentes (F-41/F-42) y elimina `BinaryFormatter` y la deserialización insegura del camino de lectura. El migrador como herramienta **separada** deja el árbol 1.4 intacto, es batch-able sin GUI y no ata la conversión al ciclo de vida del Modeler. Una sola implementación de lectura/escritura (`Modeling`) evita la divergencia entre el que escribe y el que lee. |
| **Cons** | El migrador no puede compartir código con `Modeling` (`net48` no consume `netstandard2.1`): el contrato se duplica y hay que probarlo bilateralmente. Obliga a conservar los `.coop` originales hasta validar el release 2.0.0, porque no hay conversión inversa. |

### Alternative B — Conservar el `.coop` binario como formato nativo y convertir al vuelo

| Aspect   | Detail |
|----------|--------|
| **Pros** | Cero migración de archivos: los modelos existentes siguen abriéndose tal cual, sin batch ni ventana de transición. |
| **Cons** | `BinaryFormatter` no existe en el runtime objetivo: el "al vuelo" exigiría un shim con opt-in inseguro (`EnableUnsafeBinaryFormatterSerialization`, eliminado en .NET 9+) o reimplementar el parser binario. Mantiene el binario imposible de mergear —el problema de negocio sin resolver—, la dependencia del layout de campos de las clases y el riesgo de deserialización. |

### Alternative C — XML (XSD + `XmlWriter`) en lugar de JSON

| Aspect   | Detail |
|----------|--------|
| **Pros** | Esquema formal con XSD y validación estándar; ecosistema XML maduro en .NET Framework; el orden de elementos es explícito por naturaleza. |
| **Cons** | Verbosidad y ergonomía pobres para edición manual y para agentes (el objetivo explícito del pedido); sin herramientas de merge análogas a las de JSON; `DataContractSerializer` no da control fino de orden sin trabajo extra. No hay ninguna restricción del proyecto que favorezca XML. |

### Alternative D — Migrador embebido en el Modeler 1.4 (menú de conversión + switch de batch)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Corre en el mismo proceso que escribió los archivos: sin referencias a binarios externos ni verificación de identidad; UX inmediata para humanos (abrir → convertir). |
| **Cons** | Exige **modificar y reconstruir** el tool 1.4 — artefacto congelado (ADR-001 §3.1) — y volver a publicar un 1.4.x cuya única razón de ser es convertir; sigue sin ser batch-friendly para un agente/CI y mezcla conversión con UI. El código nuevo queda atado al tool viejo, que se retira. |

### Alternative E — Esquema plano (lista de nodos clave-valor) en lugar de anidado

| Aspect   | Detail |
|----------|--------|
| **Pros** | Diffs de una línea por cambio y estructura trivial de recorrer; evita archivos con anidamiento profundo cuando hay muchas propiedades por entidad. |
| **Cons** | Obliga a inventar un sistema de identificadores y referencias cruzadas (entidad↔propiedad↔relación) que el modelo no tiene hoy: el árbol ya expresa el anidamiento (`EntityNode.Children`, `PropertyNode.RelatedFields`), y aplanarlo agrega una capa de indirección que ningún consumidor necesita y que el merge puede resolver mal (referencias huérfanas). |

---

## 3. Decision

Se adopta la **Alternativa A**: `.coop2` es un documento **JSON canónico** cuyo contrato vive en `Cooperator.Framework.Modeling`, y el **migrador es una herramienta CLI standalone `net48`** (`Cooperator.Framework.Migration`, slot fijado por ADR-001) que lee el `.coop` con `BinaryFormatter` contra los ensamblados 1.4 congelados y escribe `.coop2` sin modificarlo nunca ni ofrecer conversión inversa.

### 3.1 Especificación del formato `.coop2`

Estructura de alto nivel (ilustrativa; el **esquema normativo** es el contrato JSON versionado que produce el Bolt de `Modeling` con estas reglas):

```json
{
  "schemaVersion": 1,
  "producer": { "tool": "Cooperator Modeler", "version": "2.0.0" },
  "model": {
    "databaseName": "Ventas",
    "spPrefix": "sp",
    "projects": { "entities": "Ventas.Entities", "data": "Ventas.Data",
                  "rules": "Ventas.Rules", "app": "Ventas.Win" },
    "deployFolder": ".",
    "language": "C#",
    "postGenerationScript": ""
  },
  "entities": [
    { "name": "Customer", "generateAs": "Cliente", "namespace": "Ventas.Entities",
      "options": { "generateObject": true, "generateEntity": true, "cacheable": false,
                   "readOnly": false, "versionable": true, "descriptionFunction": true },
      "primaryKey": ["Id"], "orderBy": "", "childOf": null,
      "toString": { "override": true, "format": "{0} - {1}",
                    "arguments": ["Code"], "properties": [] },
      "properties": [
        { "name": "Id", "generateAs": "Id", "clrType": "System.Int32", "nativeType": "int",
          "length": 4, "precision": 10, "scale": 0,
          "role": { "primaryKey": true, "foreignKey": false, "identity": true, "rowVersion": false },
          "options": { "generate": true, "readOnly": false, "nullable": false, "lazyLoad": false },
          "related": [ { "name": "CustomerId", "type": "System.Int32", "relatedName": "Id",
                         "generateAsForName": "Customer", "generateAsForRelatedName": "Orders" } ] }
      ],
      "queries": [ { "name": "ByCity", "generate": true, "parameters": "@City",
                     "text": "SELECT ...", "returnType": "Scalar",
                     "scalarClrType": "System.String" } ] }
  ]
}
```

- **`schemaVersion` y evolución**: entero **monotónico** que arranca en `1`; es el único dato con semántica de compatibilidad. Un lector acepta `schemaVersion ≤ su versión` y **rechaza** `>` con error explícito (nunca coerción silenciosa). Los cambios **aditivos** (campo opcional nuevo) suben la versión y el lector aplica defaults a los archivos viejos; un cambio **rupturista** exige un ADR que reemplace a éste y un **upgrader explícito** como entregable. `producer` es informativo y **no** gobierna compatibilidad: reemplaza al `ModelerVersion` legado, que se reescribía al abrir y por eso no era un dato de formato (F-41).
- **Codificación y orden (diffs limpios)**: **UTF-8 sin BOM**, saltos `\n` (LF), indentación de **2 espacios**, sin espacios finales de línea, orden de propiedades **fijo por el esquema** (nunca orden de reflexión) y determinismo total: el mismo modelo produce bytes idénticos. Las colecciones que son **conjuntos** (entidades, propiedades, `queries`, `related`) se ordenan **por nombre, ordinal**; las que son **secuencias semánticas** (`primaryKey`, `toString.arguments`, `toString.properties`) **conservan el orden del modelo**, porque alimentan código generado y el formato de `ToString`.
- **Qué no se serializa (derivados y estado)**: `haveChanges` y `HaveChanges` por nodo; la **back-reference `parent`**; los hijos (`children`: son la anidación del JSON); `parentCandidates`; `selectedNativePropertiesCount`; los calculados (`fullGenerateAs`, `formattedNamespace`, `nameWithoutSchema`) y los predicados derivados (`isCollection`, `isEntity`, `isDescriptionField`, `isOneToOneRelation`, `isLazyLoadable`); el nodo raíz sintético `"Model"` (se **aplana** en `entities[]`); el estado de UI/editor; y `FileName` (ruta de máquina, no parte del modelo). Todos se recalculan al leer: el archivo no arrastra ruido que ensucie el diff (F-42).
- **Qué no entra nunca**: **secretos** — la `connectionString` del `Snapshot` queda **fuera** del formato (hoy está en claro o "cifrada" con una clave embebida en el IL, F-23, que es ofuscación: se trata como secreto) y el 2.0 la toma de la fuente de configuración portable (slot F-04/F-13, ADR-001 §3.6); **datos de negocio** (filas) — el archivo describe el modelo, no el contenido de la base; **rutas absolutas de máquina**; **artefactos generados**; el flag del gate retirado `generateCheckForToken` (OQ-003 / ADR-001 §3.3); y los **flags de comportamiento del generador** (`autoFilesOnly`, `stampDateAndTime`) **retirados por ADR-003**: cómo y qué se genera lo deciden las opciones del motor y del CLI, no el archivo de modelo.
- **Límite conocido y declarado**: el modelo 1.4 **no distingue vistas de tablas** (las vistas se agregaban a `databaseSchema.Tables` "para engañar al Modeler", `DomainTree.cs:204-207`). `.coop2` v1 tampoco puede: el migrador lo reporta como limitación en lugar de inventar el dato; un campo `kind` de tabla/vista sería una **capacidad nueva** (ADR/feature futuro), no una migración.
- El mismo esquema aplica a los **archivos de modelo escritos por el Modeler 2.0 y por el flujo agente**: no hay "formato de importación" distinto del formato nativo.

### 3.2 Migrador

- **Ubicación (decisión)**: **herramienta CLI standalone `net48`** — el slot `Cooperator.Framework.Migration` que ADR-001 §3.1 reservó justamente porque `Cooperator.Cli` es `net10` y no puede leer binario. Se distribuye como **carpeta/zip autocontenida** (`Cooperator.Framework.Migration.exe` + ensamblados 1.4 fijados), sin instalador, sin registro (F-35/F-36) y sin depender de `dotnet tool`. Un **host GUI opcional** (selector de archivos + progreso + reporte) puede agregarse sobre el **mismo core de conversión**; el core es un proyecto de la herramienta, no un paquete nuevo, y **el Modeler 1.4 no se modifica ni se reconstruye** (Alternative D, descartada).
- **UX para agentes (primaria)**: `cooperator-migration convert --input <archivo|glob|dir> --output <dir> --report <archivo> [--allow-vb-to-csharp] [--dry-run]`. `--dry-run` clasifica cada archivo (binario legado / ya `.coop2` / ilegible) sin escribir nada y es obligatorio en la primera pasada. Códigos de salida: `0` todo migrado, `2` migrado con warnings, `3` archivos bloqueados o no migrables, `1` error de ejecución — el mismo vocabulario base `0/1/2/3` que el CLI del motor (ADR-003 §3.2), para que un agente o CI los trate igual en ambos pasos de la migración. El reporte se emite en JSON y en texto legible. El migrador **nunca** escribe sobre un `.coop` ni acepta `--in-place`.
- **Cómo lee el formato legacy**: `BinaryFormatter.Deserialize` sobre el `FileStream`, referenciando los **ensamblados 1.4 congelados** — el tipo `Snapshot` vive en **`CooperatorModeler.exe`** (WinExe `net48`, `AssemblyVersion 1.4.5.0`) y el grafo en `Cooperator.Framework.Utility`/`Core`/`Data`/`Library`. Se referencian **los proyectos 1.4 tal como están** (`ProjectReference`, sin editar una línea) y en runtime el migrador **verifica la identidad** de los binarios que carga contra una lista fijada: **nombre + versión + hash de contenido**, y **`PublicKeyToken` sólo donde exista** — `CooperatorModeler.exe` (donde vive `Snapshot`) y `Cooperator.Framework.Utility` (donde vive el grafo) van **sin firmar** (token nulo), mientras que `Core`/`Data`/`Library` están firmados con `8f7f403e20325116` (F-31). El hash es del **contenido de los binarios**, no del fuente, y es un control **anti-deriva / anti-manipulación** contra la lista vendorizada de binarios 1.4: detecta que se está leyendo un binario distinto del acordado (build rehecho, DLL pisada). La garantía del **mismo layout binario** no la da el hash sino la **versión `1.4.5.0`** más el **build 1.4 reproducible** (prerequisito del Bolt, bullet siguiente); si algo no coincide, el migrador **falla ruidosamente** en lugar de deserializar contra tipos parecidos. **Detección por magic bytes** antes de deserializar (cabecera `BinaryFormatter` `00 01 00 00 00 FF FF FF FF` vs `.coop2` que empieza en `{`), en lugar del `"File not valid or file corrupt."` genérico de F-41. Se registra el `modelerVersion` leído y se **advierte** si ≠ `1.4.5.0` (el valor no es confiable y un layout viejo puede no deserializar): una excepción de deserialización es **error duro** del archivo, con el tipo y el mensaje de la excepción en el reporte. La conversión es **puramente de formato**: no abre conexión a base de datos (el `.coop` es autocontenido; refrescar el esquema contra SQL Server es otra operación).
- **Proyecto del migrador y prerequisito de build**: el migrador es un proyecto **SDK-style `net48` con `PackageReference`** (nada de `packages.config`): así los *binding redirects* de las dependencias transitivas de `System.Text.Json` se generan solos y el build es reproducible. **Prerequisito del Bolt del migrador**: antes del batch real debe existir un **build 1.4 reproducible** (o un build 1.4 **conocido-bueno vendorizado** con sus hashes), porque contra él se compila y contra sus binarios se verifica la identidad — sin eso, el batch no arranca.
- **Cómo escribe el JSON en `net48`**: **`System.Text.Json`** (paquete NuGet; soporta `net462+`) con un **escritor canónico sobre `Utf8JsonWriter`** que emite el orden fijo del esquema, en la **misma versión exacta** que usa `Modeling` (versión centralizada en `Directory.Packages.props`, ADR-001 §3.4): si los dos lados no comparten versión, el formato de escape puede divergir y la conformidad byte a byte que exige el test bilateral se vuelve inalcanzable. Justificación: mantiene **una sola familia de serialización** con `Modeling` (`netstandard2.1`, dependencia ya autorizada por ADR-001 §3.4), con las mismas reglas de escape y de orden; no depende de reflexión ni del orden de propiedades; y deja `JsonSerializer` disponible para leer y revalidar. Alternativas descartadas: **Newtonsoft.Json** (maduro en `net48`, pero sería una familia distinta a la del lector → riesgo de diferencias sutiles de formato entre el migrador y `Modeling`), **`DataContractJsonSerializer`** (in-box, sin control fino de orden ni de formato) y **`JavaScriptSerializer`** (legacy de `System.Web.Extensions`, dirección contraria a la 2.0). El migrador **no puede referenciar** `Modeling` (`net48` no consume `netstandard2.1`): el contrato se duplica y la conformidad se prueba con **fixtures dorados** (`.coop` anonimizados + el `.coop2` esperado, comiteados por el Bolt del migrador, incluyendo un **subconjunto representativo del corpus real**: todas las formas del modelo que aparecen en producción, no sólo un caso feliz) y un **test bilateral** que el Bolt de `Modeling` debe pasar: `write(read(fixture)) == fixture`, byte a byte, sin diff.
- **Irreversibilidad**: **no hay conversor inverso** y no se entregará uno (reconstruiría el artefacto no mergeable y exigiría reimplementar el escritor binario). El migrador **nunca modifica ni borra el `.coop`** — sólo escribe archivos nuevos, y cada `.coop2` se escribe a temporal y se renombra (fallo atómico por archivo: si un archivo falla, no deja salida parcial). Antes del batch: el corpus se **commitea/taggea** (el control de versiones es el respaldo), y recomendamos además una copia fuera del repositorio. Los `.coop` originales **se conservan hasta que el release 2.0.0 esté validado**; recién entonces pueden retirarse del control de versiones, dejando los fixtures anonimizados como evidencia. La secuencia de la migración es la de ADR-001 §3.2: convertir **primero** (paso 1), antes de tocar el TFM del Modeler.
- **Validación y diagnósticos**: el reporte registra, por archivo: ruta de origen y destino, magic bytes detectados, `modelerVersion` legado, conteos (entidades, propiedades, `related`, `queries`), `schemaVersion` de salida, el **digest del modelo normalizado** (hash del JSON canónico) y los hallazgos con severidad `error`/`warning`. Validaciones semánticas: nombre de entidad presente; `primaryKey` resuelve a propiedades existentes; `childOf` ∈ candidatos; `related[].name` resuelve dentro de la entidad; `toString.arguments` ⊆ propiedades; identificadores válidos y **sin colisión de `generateAs`** dentro de una entidad o entre entidades (colisión = error: generaría código roto). Un error **bloquea ese archivo** (entrada en el reporte + exit `3`) sin frenar el resto del batch; los warnings se aceptan (exit `2`). Verificación: `--verify` re-lee el `.coop2` producido con el lector canónico y compara su digest contra el de la construcción en memoria (round-trip interno).
- **Dos niveles de éxito (t1/t2), y un batch provisional**: **(t1) Bolt del migrador** — exit `0`, round-trip interno con digests coincidentes y **fixtures dorados** comiteados (con un subconjunto representativo del corpus real). **(t2) Bolt de `Modeling`** — el **test bilateral** sobre **los mismos fixtures** (`write(read(fixture)) == fixture`) es lo que **completa la conformidad**: recién ahí el formato está cerrado de punta a punta. Por lo tanto, el **batch real sobre el corpus es provisional hasta t2**: si t2 revela una divergencia, se corrige el escritor/schema y **se vuelve a migrar desde los `.coop`** (intactos, §3.2 "Irreversibilidad") — el corpus no se retira del control de versiones hasta que t2 esté verde y el release 2.0.0 validado.

### 3.3 Deprecaciones (OQ-005) en el corpus legacy

- **VB.NET** (`Snapshot.Language` ∈ `{"CS","VB"}`, según `GenerationDialog.cs:120-122`): por **defecto bloquea** el archivo con un error claro —*"modelo VB.NET: la generación VB salió del alcance de la 2.0 (OQ-005); el modelo es agnóstico del lenguaje, usá `--allow-vb-to-csharp` para migrarlo como C#"*— y con `--allow-vb-to-csharp` lo migra reescribiendo `language` a C# y registrando un **warning destacado** en el reporte. No hay conversión automática de VB a C# ni la habrá: la decisión es explícita y por archivo, jamás silenciosa. Los **artefactos VB ya generados** en los repositorios de los consumidores no se convierten: se regeneran con el motor 2.0 (ADR-003) y las plantillas `VisualBasic*` se eliminan (ADR-001 §3.5), así que el reporte lo recuerda como paso siguiente obligatorio.
- **Oracle**: **no hay contenido Oracle que migrar** — el `OracleScriptor` era un esqueleto vacío y la referencia del csproj estaba muerta (F-11/F-39); el modelo 1.4 es SQL Server por construcción (`MSSQLReverseFactory`). El reporte lo declara explícitamente para que la ausencia no se confunda con una pérdida.
- **Export a Excel**: tampoco está en el modelo (es un helper de runtime del `Core`, F-03): nada que migrar; el `providerName`/las plantillas sí son artefactos del motor y se resuelven en ADR-003 (F-37).
- **Gate por token retirado** (OQ-003 / ADR-001 §3.3): `GenerateCheckForToken` se **descarta** con una nota informativa en el reporte; el código generado deja de emitir la contraparte del gate.
- **`connectionString`**: excluida del `.coop2`; el migrador la reporta como `warning` (secreto no trasladado) y la 2.0 la toma de la configuración (slot F-04/F-13). `DataBaseName` se conserva: no es secreto y es el nombre por defecto del archivo.

### 3.4 Interplay con ADR-001 y ADR-003

- **Contrato y serialización**: viven en `Cooperator.Framework.Modeling` (`netstandard2.1`) — ADR-001 §3.1. `Modeling` es el **único** lector/escritor del formato: el motor (ADR-003) y el CLI `net10` **consumen** `.coop2` a través de él y no serializan por su cuenta, para que no existan dos definiciones del mismo archivo.
- **Slots respetados sin renombrar**: `Cooperator.Framework.Migration` (herramienta `net48` del migrador), `Cooperator.Framework.Modeling` (contrato) y `Cooperator.Framework.CodeGeneration` + `Cooperator.Cli` (`net10`, motor — ADR-003). El migrador **no** es un comando del CLI `net10`: la restricción de runtime (`BinaryFormatter` y los ensamblados 1.4) es la razón por la que existe el slot `net48`, y una precisión del ADR-003 sobre el CLI no cambia este layout.
- **Secuencia**: el migrador es el **paso 1** del camino de migración de ADR-001 §3.2 y su salida —el corpus `.coop2`— es la entrada de `Modeling` y del motor. La conformidad se cierra en **dos niveles** (§3.2): el Bolt del migrador entrega t1 (round-trip interno + fixtures) y el Bolt de `Modeling` entrega **t2** con el test bilateral sobre esos mismos fixtures; el corpus convertido en el medio es **provisional** y se re-migra desde los `.coop` si t2 encuentra una divergencia.

---

## 4. Consequences

**Positive:**

- Diff y merge reales sobre el modelo: el pedido de negocio que originó todo el análisis queda resuelto, con archivos de una línea por cambio lógico y sin ruido de campos derivados.
- Se elimina `BinaryFormatter` del camino de lectura y escritura del modelo: desaparece el riesgo de deserialización de archivos recibidos y el bloqueo de runtime que impedía al Modeler migrado abrir sus propios archivos (F-41/F-43).
- Un solo lector/escritor (`Modeling`) más detección por magic bytes y diagnósticos por archivo: los errores de modelo se explican, en lugar del `"File not valid or file corrupt."` genérico.
- El formato no lleva secretos: una `connectionString` que hoy viaja en el binario (en claro u ofuscada) deja de distribuirse con el modelo.
- El migrador no toca el árbol 1.4 ni el corpus: es re-ejecutable mientras los `.coop` existan, y el batch no necesita base de datos ni GUI.

**Trade-offs:**

- **Dos implementaciones del contrato** (migrador `net48` y `Modeling` `netstandard2.1`) que no comparten código: la conformidad se sostiene con fixtures dorados y el test bilateral `write(read(x)) == x`, que pasa a ser obligatorio para cualquier cambio de esquema.
- Transición con dos formatos conviviendo y `.coop` conservados hasta validar 2.0.0 (historia de git más pesada, aunque los binarios puedan retirarse después).
- El `.coop2` no recuerda la connection string: el usuario la vuelve a ingresar una vez por proyecto (costo único a cambio de no distribuir credenciales).
- El migrador depende de que el fuente 1.4 siga siendo compilable con el toolchain de la máquina donde corre el batch, y de la disponibilidad de .NET Framework 4.8 (parte de Windows).
- `--allow-vb-to-csharp` deja una decisión por archivo en manos del operador: el batch no puede "adivinar" qué modelos VB valen la pena.

**Technical debt:**

- El escritor canónico está duplicado en el migrador: hasta que el test bilateral esté verde del lado de `Modeling`, existe riesgo de deriva entre ambos (mitigado, no eliminado).
- El reporte del migrador (con los digests) es la única evidencia del batch: si no se archiva junto al corpus convertido, la trazabilidad de qué versión de cada modelo se migró se pierde.
- Las **vistas indiferenciadas de tablas** quedan como deuda de modelo: un eventual campo `kind` exige un ADR y una subida de `schemaVersion`, y el corpus migrado lo tendrá en `null`.
- Si un `.coop` histórico no deserializa (layout anterior a 1.4.5.0), su modelo queda efectivamente perdido salvo que exista una copia abierta en un Modeler 1.4 más antiguo: el migrador no puede hacer más que reportarlo.

---

## 5. Applicable NFRs

| NFR | Description | Threshold | How it is measured |
|-----|-------------|-----------|-------------------|
| Determinism | El formato es canónico: el mismo modelo produce siempre los mismos bytes, y leer y reescribir no introduce cambios | `write(read(x)) == x` byte a byte para todo archivo del corpus y para los fixtures | Test bilateral sobre fixtures dorados (migrador ↔ `Modeling`) + `git diff` vacío tras re-generar un modelo sin cambios |
| Secret-free model | El archivo de modelo no contiene credenciales ni material secreto | 0 ocurrencias de connection strings/credenciales en todo `.coop2` del corpus migrado | Reglas del esquema + escaneo del corpus convertido en el reporte del migrador (la `connectionString` excluida se reporta como warning) |

---

## 6. References

- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — revisión aprobada el 2026-10-15; hallazgos F-41 (mecanismo binario del `.coop` y prerequisito de secuencia), F-42 (orden canónico, campos derivados, política de `schemaVersion`), F-43 (`BinaryFormatter` y `[Serializable]` en plantillas), F-27/F-28 (modelo acoplado a UI), F-03/F-11/F-39 (features deprecadas), F-23 (cifrado ofuscado) y F-40 (encriptador de connection strings).
- [`OQ-004`](../analysis/open-questions/OQ-004-formato-del-coop-json.md) — extensión `.coop2`, migrador obligatorio sobre .NET Framework 4.8 y operación irreversible; ubicación diferida a este ADR.
- [`OQ-005`](../analysis/open-questions/OQ-005-alcance-features-en-riesgo.md) — VB.NET, Oracle y export a Excel deprecados. [`OQ-001`](../analysis/open-questions/OQ-001-particion-de-utility.md) (`Modeling` portable dueño del modelo) y [`OQ-006`](../analysis/open-questions/OQ-006-politica-de-compatibilidad.md) (moderno-only, sin puente) como contexto.
- [`ADR-001`](ADR-001-layout-y-empaquetado-cooperator-2.md) — borrador del mismo ciclo: slots de `Modeling`/`Migration`/`CodeGeneration`+`Cli`, política de dependencias y orden del camino de migración.
- Evidencia de código 1.4: `Tools/CooperatorModeler/Snapshot.cs`, `MainForm.cs:203-222` y `:261-302`, `GenerationDialog.cs:120-122`, `CooperatorModeler.csproj`, `Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` (clases `[Serializable]`, nodo raíz `"Model"`, vistas agregadas como tablas).

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
| **review_ready_at** | `2026-10-15T22:22:28-03:00` |
| **review.started_at** | `2026-10-15T22:32:47-03:00` |
| **review.decided_at** | `2026-10-15T22:32:47-03:00` |
| **Findings** | Sin hallazgos adicionales — `acknowledged_without_comment: true` (motivo en el frontmatter) |
