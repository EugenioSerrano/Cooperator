---
id: "SPEC-261016-0002"
title: "Migrador `.coop` → `.coop2`: herramienta CLI, escritura canónica, reporte por archivo y fixtures dorados"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-002" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-002", "ADR-001", "ADR-008"]
prerequisites: ["SPEC-261016-0000-arnes-tests-y-build-reproducible.md"] # SPEC canónica del Bolt 001 (arnés, build 1.4 reproducible y gate de compatibilidad de API): de ella dependen la compilación contra el 1.4 y la lista de identidad de binarios — ver §5
risk_class: "high" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L2" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "confidential" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (refactor): su resultado se verifica con el round-trip interno y los fixtures dorados sobre archivos del repositorio — no toca base de datos, servicio externo, cola, almacenamiento ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:16:20-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:16:20-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:16:20-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-002 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent), ADR-002/ADR-001/ADR-008 accepted (tech_lead), REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → integration_test_plan.applicability: n/a con razón; el alcance §4 es exactamente lo que el Bolt §2 exige; t1 acá, t2 en el Bolt 004 y el batch real en el Bolt 029, correctamente acotados (ADR-002 §3.2)."
    - "Compliant — confinamiento de BinaryFormatter a un solo archivo del árbol 2.0 (LegacyReader.cs, §B.1/B.2): única excepción de ADR-008 §3.4, supresión en el ámbito más estrecho, cero usos fuera, verificado por build + chequeo de fuente (§10); la verificación de identidad corre antes de deserializar."
    - "Compliant — control de datos confidential: fixtures sintéticos o anonimizados por el dueño del corpus; el ejecutor nunca procesa .coop reales (§E.3, stop condition §15); la cadena de conexión nunca se traslada ni se imprime (AC-6, ReportTests, gate secret-leak + PII/DLP)."
    - "Compliant — atomicidad e irreversibilidad: AtomicFileWriter (temporal + rename), sin --in-place, original nunca abierto en escritura, destino existente bloqueado sin sobrescritura (AC-3/AC-4); rollback por abandono con los originales como red (ADR-002 §3.2)."
    - "Compliant — identidad de binarios: nombre + versión + SHA-256 + PublicKeyToken donde exista, verificada antes de deserializar (AC-7); la lista se genera del build 1.4 reproducible del Bolt 001 (1.4.5.0), NO del vendored Templates/Assemblies (Utility 1.3.3.0): correctamente excluido por proveniencia, sin adoptar el binario desalineado."
    - "Compliant — stop conditions (10) y risk matrix (12 filas) completas y con mitigación; redacción self-contained, fases A–F explicativas y AC mapping completo."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-002`. El esquema normativo del
     `.coop2` es del Bolt 004; el batch sobre el corpus real es del Bolt 029 — este SPEC no los invade.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; el plan y el
     Bolt coinciden y esa coincidencia es parte del gate de §3. Un desajuste sería reporte bloqueante.
  ⚠️ CLASIFICACIÓN `confidential`: la cadena de conexión viaja dentro de los modelos legacy. El control de
     anonimización de fixtures y la prohibición de procesar `.coop` reales en este Bolt son explícitos en §8.1
     y §10; no son un detalle de estilo.
-->

# SPEC-261016-0002 — Migrador `.coop` → `.coop2`: herramienta CLI, escritura canónica, reporte por archivo y fixtures dorados

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 3) |
| **Bolt** | [US-000.BOLT-002](../functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md) |
| **ADRs** | [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se construye la **herramienta de conversión** de Cooperator 2.0: un ejecutable de línea de comandos
(`Cooperator.Framework.Migration`, `net48`) que **lee** un modelo en el formato legacy `.coop` —un blob binario
`BinaryFormatter` de un `Snapshot`— **y escribe** el mismo modelo en el formato vigente `.coop2`, con un
**reporte por archivo** (clasificación, conteos, digests y hallazgos con severidad), **conservando intactos los
originales** y **sin ofrecer conversión inversa**.

El problema que resuelve es doble y ya está establecido por la evidencia aprobada (REV-001 F-41/F-42/F-43): el
`.coop` no se puede mergear ni revisar —motivación declarada del dueño— y, además, `BinaryFormatter` **está
eliminado en .NET 9+**, de modo que el Modeler migrado a `net10.0-windows` **no podría leer ni escribir sus
propios archivos**. Este paso es por eso **irreversible en la práctica**: una vez que el Modeler corre en el
runtime moderno, los `.coop` históricos dejan de ser legibles salvo por esta herramienta, que debe existir y
haber corrido **antes** de tocar el TFM del Modeler (ADR-001 §3.2, ADR-002 §3.2).

Si no se implementa, Cooperator 2.0 no puede abrir un solo modelo existente: no hay formato vigente para el
corpus histórico, no hay fixtures dorados sobre los que el Bolt 004 cierre la conformidad bilateral del formato
y no hay camino de migración para los consumidores.

Este SPEC entrega **t1** (ADR-002 §3.2): exit `0`, round-trip interno con digests coincidentes y **fixtures
dorados comiteados**, incluido un subconjunto representativo del corpus real. **No** cierra la conformidad del
formato —eso es t2, en el Bolt de `Modeling`— ni ejecuta el batch real, que es del Bolt 029.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` (§4.6, hallazgos F-41/F-42/F-43 y §6 ítem 3) documentó con evidencia
de archivo y línea que `Savemodel()` serializa el `Snapshot` completo con `BinaryFormatter`
(`Tools/CooperatorModeler/MainForm.cs:203-222`) y que `OpenCoopFile()` lo deserializa validando sólo un string
`ModelerVersion` que el propio Modeler reescribe a la fuerza al abrir (`:261-302`, `:280-281`), con el mensaje
único `"File not valid or file corrupt."`. El grafo serializado son las clases `[Serializable]` de
`Snapshot.cs` (configuración del modelo) y de `DBReverseHelper/DomainTree.cs` (`EntityNode`, `PropertyNode`,
`RelatedField`, `ToStringStruct`, `DatabaseQuery`) colgado de un nodo raíz sintético `"Model"`.

**Qué quedó decidido aguas arriba** (no se reabre): el formato es **`.coop2`** —JSON, extensión nueva, el `.coop`
queda como legado binario—, el migrador es un **entregable obligatorio** que corre sobre **.NET Framework 4.8**
con `BinaryFormatter` aún disponible y **antes** de migrar el Modeler, la operación es **irreversible**, y el
esquema normativo que el migrador debe respetar es el **contrato versionado que produce el Bolt de `Modeling`**
(ADR-002 §3.1/§3.2/§3.4; OQ-004 opción A).

**Qué decide este SPEC, dentro de esas restricciones.** La herramienta `net48` en el slot
`Cooperator.Framework.Migration` (ADR-001 §3.1) con su CLI, su lectura del binario contra los ensamblados 1.4
congelados con **verificación de identidad**, su **escritor canónico** sobre `System.Text.Json`/`Utf8JsonWriter`
(contrato **duplicado**: `net48` no puede consumir `netstandard2.1`, ADR-002 §3.2), su **reporte**, su
**atomicidad por archivo**, sus **códigos de salida** y su **set de fixtures dorados**. El esquema `.coop2`
normativo sigue siendo del Bolt 004; acá se fija la forma concreta que el migrador emite a partir de ADR-002
§3.1 y, sobre todo, la **salida byte a byte esperada** de cada fixture, que es el contrato que el Bolt 004 debe
satisfacer en su test bilateral.

**Restricciones técnicas del entorno.** La herramienta es Windows-only por construcción (`net48` y
`BinaryFormatter`): forma parte de la supervivencia Windows-only explícita de ADR-001 §3.5. No abre conexión a
base de datos —el `.coop` es autocontenido—, no tiene GUI propia, no se distribuye como `dotnet tool` y **no
modifica ni reconstruye el Modeler 1.4** (ADR-002 §3.2). El árbol `1.4/` se referencia **tal como está**.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-002`](../functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md) — `status: ready`, `risk_class: high`, `data_classification: confidential`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-41, F-42, F-43, §6 ítem 3 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (formato), §3.2 (migrador, niveles t1/t2, identidad de binarios, irreversibilidad), §3.3 (deprecaciones), §3.4 (interplay) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (slot `Cooperator.Framework.Migration` `net48`, layout `2.0/`, versiones centralizadas), §3.2 (secuencia de migración), §3.4 (política de dependencias) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.2 (`Snapshot` y el grafo: la persistencia es el `.coop2`), §3.4 (la **única** excepción de `BinaryFormatter`, acotada y con fecha de retiro; cero `SYSLIB0051`) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — build 1.4 reproducible + arnés + versiones centralizadas | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Tools/CooperatorModeler/Snapshot.cs` (los campos serializados del `.coop`: `ModelerVersion`,
`ConnectionString`, `DataBaseName`, `FileName`, `SPPrefix`, `RulesProjectName`, `DataProjectName`,
`EntitiesProjectName`, `AppProjectName`, `DeployFolder`, `Language`, `GenerateAutoFileOnly`,
`StampDateAndTimeOnAutoFiles`, `GenerateCheckForToken`, `PostGenerationScript`, `DomainTree`, `HaveChanges`),
`1.4/Tools/CooperatorModeler/MainForm.cs:203-222`/`:261-302` (guardar y abrir), y
`1.4/Cooperator.Framework.Utility/DBReverseHelper/DomainTree.cs` (los nodos del modelo).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

**Material externo no gobernado (control de datos):** el corpus real de `.coop` **no es una fuente de este
SPEC** y **no lo es de su ejecución**. Sólo entra al ciclo —en el Bolt 029— y en este Bolt únicamente como
**fixtures anonimizados** aportados por el dueño (§8.1, §10). No existe hoy ningún `.coop` ni `.coop2` dentro
del repositorio.

---

## 4. Scope

### In scope

- El proyecto **`2.0/src/Cooperator.Framework.Migration/`** (`net48`, SDK-style, `PackageReference`, un solo
  TFM) con su `ProjectReference` a los proyectos 1.4 congelados y su **verificación de identidad** de los
  binarios legacy en runtime.
- La **lectura** del formato legacy: detección por *magic bytes*, deserialización contra los ensamblados 1.4,
  registro del `ModelerVersion` leído y error duro y explicable cuando el archivo no deserializa.
- La **clasificación** de cada entrada (binario legacy / ya `.coop2` / ilegible / inexistente) y el
  **`--dry-run`** sin escritura.
- La **escritura canónica** del `.coop2`: orden fijo del esquema, UTF-8 sin BOM, `LF`, indentación de 2
  espacios, sin espacios finales, determinismo total y las exclusiones de derivados, estado y secretos.
- La **proyección del modelo** legacy (`Snapshot` + grafo `DomainTree`) al documento `.coop2`.
- La **validación semántica** por archivo y el **bloqueo con diagnóstico** del contenido no migrable, sin
  frenar el resto del conjunto.
- El **reporte por archivo** en JSON y en texto, con digests, conteos y hallazgos por severidad, y los
  **códigos de salida** `0/1/2/3`.
- La **conservación del original**, la **atomicidad por archivo** y la ausencia de `--in-place` y de
  conversión inversa.
- La **clasificación de las deprecaciones** (VB.NET, Oracle, export a Excel, gate por token retirado, flags
  del generador retirados por ADR-003, `FileName`, `connectionString`).
- Los **fixtures dorados** (`.coop` anonimizado de entrada + `.coop2` esperado byte a byte), incluido el
  **subconjunto representativo del corpus real**, su índice de procedencia y cobertura, y el **round-trip
  interno** (`--verify`).
- La **forma publicable** autocontenida de la herramienta (carpeta/zip: ejecutable + binarios 1.4 fijados), sin
  instalador y sin registro.

### Out of scope

- El **esquema normativo** del `.coop2`, su lector/escritor canónico definitivo y su test bilateral: son del
  **Bolt 004** (ADR-002 §3.1/§3.4). Este SPEC produce una **implementación conforme y duplicada**, y los
  fixtures que ese Bolt consume; no declara el esquema.
- El **batch sobre el corpus real**: **Bolt 029** (ADR-002 §3.2).
- El **retiro del descifrado legacy** y cualquier operación criptográfica: Bolts 022/023/030 (ADR-007).
- El **modelo portable** `Cooperator.Framework.Modeling` y el reverse-engineering: **Bolt 003** (ADR-001 §3.1).
- El **motor de generación**, el **CLI del motor** y el **shell del Modeler**: Bolts 017/018/025/026.
- Las **notas de migración** como documento: **Bolt 028**. Este Bolt sólo aporta su lista de rupturas si
  introdujera alguna (no las introduce: es una herramienta nueva, ver §12).
- Cualquier modificación del árbol `1.4/`, del Modeler 1.4, de su build o de sus fuentes.
- La conversión inversa `.coop2` → `.coop` (no existe ni existirá, ADR-002 §3.2).

---

## 5. Prerequisites and baseline

- **Bolt 001 (`US-000.BOLT-001`, `ready`)** y su SPEC canónica
  ([`SPEC-261016-0000-arnes-tests-y-build-reproducible.md`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md),
  redactada en la misma ola): el migrador se **compila contra el build 1.4 reproducible** (o el build
  conocido-bueno vendorizado con sus hashes) y su verificación de identidad se apoya en esa salida (ADR-002
  §3.2). Su SPEC es el `prerequisites` declarado en el frontmatter y el Bolt ya lo registra en `dependencies`.
  Si al iniciar el V-Bounce la salida del Bolt 001 no existe, **parar y reportar** (§15).
- **Estructura del árbol 2.0 provista por el Bolt 001:** `2.0/Cooperator2.sln`, `global.json`,
  `Directory.Build.props` (analizadores, determinismo, warning-as-error, `AssemblyVersion` `2.0.0.0`, firma) y
  `Directory.Packages.props` (versión **única y centralizada** de cada dependencia, ADR-001 §3.1/§3.4). Este
  SPEC **no crea** esa infraestructura ni declara versiones por proyecto: la consume. La versión de
  `System.Text.Json` que use el migrador es **la que fija `Directory.Packages.props`** y debe ser **la misma
  exacta** que consuma `Modeling` (ADR-002 §3.2).
- **Toolchain:** el mismo que el arnés fija para construir el 1.4 (SDK/MSBuild pinneados en `global.json` y en
  la máquina de build) y el **pack de referencia de .NET Framework 4.8** presente. Si falta el pack de 4.8,
  **parar y pedir decisión** (§15): no se agrega una dependencia de build no autorizada por ADR-001 §3.4.
- **Framework de pruebas:** el que fije el Bolt 001. Este SPEC no introduce uno nuevo. Si el fijado **no corre
  sobre `net48`**, parar y pedir decisión (§15): el proyecto de pruebas del migrador debe ser `net48` porque el
  migrador y el formato legacy son `net48` (un host moderno no puede referenciar el proyecto `net48` ni
  serializar el fixture sintético contra los tipos 1.4).
- **Árbol `1.4/` presente y congelado** en la línea base registrada en §3.
- **Build state de partida:** `2.0/` existe vacío en la línea base; el árbol 2.0 nace con el Bolt 001. Este
  SPEC agrega su proyecto a la solución y no toca ningún TFM ni paquete ya decidido.
- **Decisión de ejecución registrada (no es supuesto):** el `.coop` **no** aporta datos de negocio ni requiere
  base de datos; el migrador es offline por diseño (ADR-002 §3.2). Nada de este Bolt se verifica contra un
  servicio real, de ahí su plan de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-002`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14): el V-Bounce arranca cuando el bloque `review` del frontmatter esté decidido.

### Phase A — Proyecto de la herramienta y verificación de identidad de los binarios legacy

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 Proyecto `net48` SDK-style con `ProjectReference` al árbol 1.4 congelado

Se crea `2.0/src/Cooperator.Framework.Migration/Cooperator.Framework.Migration.csproj`: **SDK-style**, un solo
`TargetFramework` (`net48`), **`PackageReference`** (nunca `packages.config` — es lo que hace que los *binding
redirects* de las dependencias transitivas de `System.Text.Json` se generen solos y el build sea reproducible,
ADR-002 §3.2), versiones tomadas de `Directory.Packages.props` y **cero** declaraciones locales de versión. Se
agrega el proyecto a `2.0/Cooperator2.sln`. `ProjectReference` a los proyectos 1.4 **tal como están**
(`1.4/Tools/CooperatorModeler/CooperatorModeler.csproj` —donde vive `Snapshot`—,
`1.4/Cooperator.Framework.Utility/Cooperator.Framework.Utility.csproj` —el grafo del modelo—,
`1.4/Cooperator.Framework.Core|Data|Library/*.csproj`) **sin editar una línea de `1.4/`**: es la excepción de
`BinaryFormatter` del producto y la única (ADR-008 §3.4).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Cooperator.Framework.Migration.csproj` — la herramienta CLI `net48` con las referencias 1.4 congeladas
- `2.0/src/Cooperator.Framework.Migration/Program.cs` — punto de entrada del ejecutable (delega en el parser de CLI)

**Files modified:**
- `2.0/Cooperator2.sln` — alta del proyecto del migrador (creado por el Bolt 001)

#### A.2 Lista fijada de identidad de los binarios 1.4

Se fija la identidad de los binarios contra los que se va a deserializar: **nombre + versión + hash de
contenido (SHA-256)** y **`PublicKeyToken` sólo donde exista** (ADR-002 §3.2). `CooperatorModeler.exe` (donde
vive `Snapshot`) y `Cooperator.Framework.Utility.dll` (donde vive el grafo) van **sin firmar** —token nulo—;
`Core`, `Data` y `Library` están firmados con token `8f7f403e20325116` (F-31). La lista se genera desde la
salida del build 1.4 del Bolt 001 y **se comitea** como dato de la herramienta.

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Identity/legacy-binaries.json` — lista fijada: por binario, `file`, `assembly`, `version`, `public_key_token` (o `null`), `sha256`
- `2.0/src/Cooperator.Framework.Migration/Identity/BinaryIdentityVerifier.cs` — resuelve cada binario en la carpeta de la aplicación, compara nombre, versión, token y SHA-256, y devuelve el detalle de la discrepancia
- `2.0/tools/pin-legacy-binaries.ps1` — regenera `legacy-binaries.json` desde la salida del build 1.4 reproducible (Bolt 001); no se ejecuta en el build normal

**Decisiones de implementación (no arquitectura, ya decidida en ADR-002 §3.2):** la verificación corre
**antes** de cualquier deserialización; una discrepancia **detiene la herramienta** con mensaje explícito que
nombra el archivo y el campo que no coincide, en lugar de deserializar contra tipos parecidos; el hash es del
**contenido de los binarios**, no del fuente, y es un control anti-deriva / anti-manipulación, no la garantía de
layout (la da la versión `1.4.5.0` más el build 1.4 reproducible).

---

### Phase B — Lectura del formato legacy y clasificación de entradas

**Duration:** 3h total cycle — **Complexity:** Medium

#### B.1 Clasificación por *magic bytes*

Antes de deserializar, cada entrada se clasifica por sus primeros bytes: cabecera `BinaryFormatter`
(`00 01 00 00 00 FF FF FF FF`) = **binario legacy**; `{` = **ya `.coop2`**; cualquier otra cosa, un archivo
inexistente o inaccesible = **ilegible/no aplicable**. Esto reemplaza al `"File not valid or file corrupt."`
genérico del 1.4 (F-41) y es lo que hace auditable el `--dry-run`.

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Conversion/ModelFileClassifier.cs` — clasificación de la entrada y resultado tipado con los bytes detectados
- `2.0/src/Cooperator.Framework.Migration/Conversion/LegacyReader.cs` — **único** punto de uso de `BinaryFormatter.Deserialize` en todo el árbol 2.0

#### B.2 Lectura del binario, versión legada y error duro

`LegacyReader` abre el archivo, deserializa a `Snapshot` (tipo de `CooperatorModeler.exe`) y registra el
`modelerVersion` leído, **advirtiendo** cuando es distinto de `1.4.5.0` (el dato no es confiable: el Modeler lo
reescribía al abrir, y un layout anterior puede no deserializar). Una excepción de deserialización es **error
duro de ese archivo**: el reporte lleva el tipo y el mensaje de la excepción, no un texto genérico. La
conversión es **puramente de formato**: no se abre ninguna conexión (el `.coop` es autocontenido).

`BinaryFormatter` se usa con la única excepción autorizada por ADR-008 §3.4 y con **fecha de retiro**; si el
build emitiera un diagnóstico de obsolescencia por ese uso, se suprime **en el ámbito más estrecho posible**
(el miembro que contiene la llamada) con un comentario que cita ADR-002 §3.2 / ADR-008 §3.4 y su vencimiento.
**Nunca** una supresión de proyecto o de solución (ADR-008 §3.4) y **cero** usos de `BinaryFormatter` fuera de
este archivo, en todo el árbol 2.0.

---

### Phase C — Escritura canónica del `.coop2` y proyección del modelo

**Duration:** 4h total cycle — **Complexity:** High

#### C.1 Proyección del grafo legacy al documento del formato

Se proyecta el `Snapshot` deserializado al documento `.coop2` respetando **ADR-002 §3.1** —cuyo bloque JSON es
ilustrativo y cuyas reglas son normativas— y usando los nombres de campo de ese bloque:

| Origen legacy (`.coop`) | Campo del `.coop2` | Regla |
|---|---|---|
| `DataBaseName` | `model.databaseName` | Se conserva (no es secreto; es el nombre por defecto del archivo) |
| `SPPrefix` | `model.spPrefix` | Se conserva |
| `EntitiesProjectName`, `DataProjectName`, `RulesProjectName`, `AppProjectName` | `model.projects.entities\|data\|rules\|app` | Se conservan |
| `DeployFolder` | `model.deployFolder` | Se conserva |
| `Language` | `model.language` | `"CS"` → `C#`; `"VB"` → bloqueo por defecto (§C.3) |
| `PostGenerationScript` | `model.postGenerationScript` | Se conserva como contenido (el motor no lo ejecuta: ADR-003) |
| raíz sintética `"Model"` + `DomainTree.Children` (`EntityNode`) | `entities[]` | **Se aplana**: la raíz no se serializa (ADR-002 §3.1) |
| `EntityNode.*` | `entities[].name`, `generateAs`, `namespace`, `options`, `primaryKey`, `orderBy`, `childOf`, `toString` | Campo homónimo según la tabla del ADR; los derivados no |
| `PropertyNode.*` | `entities[].properties[]` | `name`, `generateAs`, `clrType`, `nativeType`, `length`, `precision`, `scale`, `role`, `options` |
| `RelatedField` (`PropertyNode.RelatedFields`) | `properties[].related[]` | `name`, `type`, `relatedName`, `generateAsForName`, `generateAsForRelatedName` |
| `ToStringStruct` | `entities[].toString` | `override`, `format`, `arguments` (secuencia: conserva el orden del modelo), `properties` (secuencia) |
| `DatabaseQuery` | `entities[].queries[]` | `name`, `generate`, `parameters`, `text`, `returnType`, `scalarClrType` |
| `ModelerVersion` | *(no se serializa)* | Es informativo: se registra en el reporte y reemplaza al `producer` del formato, que es informativo y **no gobierna compatibilidad** (ADR-002 §3.1) |

**Qué no se serializa** (ADR-002 §3.1, lista cerrada): `HaveChanges`/`haveChanges` y los flags por nodo; la
**back-reference `parent`**; los hijos (`children`: son la anidación del JSON); `parentCandidates`;
`selectedNativePropertiesCount`; los calculados (`fullGenerateAs`, `formattedNamespace`, `nameWithoutSchema`) y
los predicados derivados (`isCollection`, `isEntity`, `isDescriptionField`, `isOneToOneRelation`,
`isLazyLoadable`); el nodo raíz sintético `"Model"`; el estado de UI/editor; `FileName` (ruta de máquina).
**Todos se recalculan al leer.**

**Qué no entra nunca**: `ConnectionString` (secreto: hoy viaja en claro o "cifrada" con clave embebida — F-23 —
y se trata como secreto; **se reporta como warning**, nunca se traslada ni se imprime), datos de negocio,
rutas absolutas, artefactos generados, el flag del gate retirado `GenerateCheckForToken` (OQ-003 / ADR-001 §3.3)
y los flags de comportamiento del generador `GenerateAutoFileOnly` / `StampDateAndTimeOnAutoFiles` (**retirados
por ADR-003**): los tres se descartan con una nota informativa en el reporte.

**Límite declarado**: el modelo 1.4 **no distingue vistas de tablas**. El migrador **no inventa** un campo
`kind`: emite un hallazgo informativo con esa limitación (ADR-002 §3.1).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Conversion/ModelProjector.cs` — proyección del `Snapshot` legacy al documento del formato
- `2.0/src/Cooperator.Framework.Migration/Conversion/CanonicalModel.cs` — tipos del documento (contrato **duplicado**, no compartido con `Modeling`)
- `2.0/src/Cooperator.Framework.Migration/Conversion/ModelDigest.cs` — digest del modelo normalizado (SHA-256 del JSON canónico)

#### C.2 Escritor canónico determinista

Se escribe con **`System.Text.Json`** y un **escritor canónico sobre `Utf8JsonWriter`** que emite el orden fijo
del esquema (nunca orden de reflexión): **UTF-8 sin BOM**, saltos `\n` (`LF`), indentación de 2 espacios, sin
espacios finales de línea y **determinismo total** (el mismo modelo produce bytes idénticos). Las colecciones
que son **conjuntos** (entidades, propiedades, `queries`, `related`) se ordenan **por nombre, ordinal**; las que
son **secuencias semánticas** (`primaryKey`, `toString.arguments`, `toString.properties`) **conservan el orden
del modelo**, porque alimentan código generado y el formato de `ToString` (ADR-002 §3.1).

El escritor no serializa **más** campos que los de §C.1: un campo nuevo del formato exige subir `schemaVersion` y
un ADR que reemplace a ADR-002 (§3.1). `schemaVersion` de salida es `1`.

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Conversion/CanonicalJsonWriter.cs` — escritor de orden fijo sobre `Utf8JsonWriter`
- `2.0/src/Cooperator.Framework.Migration/Conversion/CanonicalJsonReader.cs` — lector del `.coop2` producido, para el round-trip interno de `--verify`

#### C.3 Validación semántica y clasificación de deprecaciones

Validaciones por archivo (ADR-002 §3.2): nombre de entidad presente; `primaryKey` resuelve a propiedades
existentes; `childOf` ∈ candidatos; `related[].name` resuelve dentro de la entidad; `toString.arguments` ⊆
propiedades; identificadores válidos; **sin colisión de `generateAs`** dentro de una entidad ni entre
entidades. Un **error bloquea ese archivo** (entrada en el reporte + exit `3`) **sin frenar el resto del
batch**; los warnings se aceptan (exit `2`).

Deprecaciones (ADR-002 §3.3): **VB.NET** (`Language = "VB"`) **bloquea por defecto** con el error claro del ADR
y, con `--allow-vb-to-csharp`, migra reescribiendo `language` a `C#` y registrando un **warning destacado**;
jamás en silencio ni convirtiendo VB a C#. **Oracle** y **export a Excel**: **no hay contenido que migrar** y el
reporte lo declara explícitamente (que la ausencia no se confunda con una pérdida).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Conversion/ModelValidator.cs` — validaciones semánticas y colisiones de nombres generados
- `2.0/src/Cooperator.Framework.Migration/Conversion/Finding.cs` — hallazgo tipado (`severity` `error`/`warning`/`info`, `code`, `message`, `location`) y el catálogo de códigos

---

### Phase D — CLI, reporte por archivo y atomicidad

**Duration:** 3h total cycle — **Complexity:** Medium

#### D.1 Superficie de línea de comandos

```
cooperator-migration convert --input <archivo|glob|dir> --output <dir> --report <archivo>
                             [--allow-vb-to-csharp] [--dry-run] [--verify]
```

`--dry-run` **clasifica cada archivo sin escribir nada** y es la primera pasada obligatoria de la operación
(ADR-002 §3.2): la herramienta no lo impone por sí misma —son banderas independientes—, pero la evidencia de la
corrida **exige** el `--dry-run` previo (§8.1). **Códigos de salida**: `0` todo migrado, `2` migrado con
warnings, `3` archivos bloqueados o no migrables, `1` error de ejecución — el mismo vocabulario base `0/1/2/3`
que el CLI del motor (ADR-003 §3.2). No existe `--in-place` y el migrador **nunca** escribe sobre un `.coop`.

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Cli/ConvertCommand.cs` — verbo `convert`, flags y resolución de la entrada (`archivo`, `glob`, `dir`)
- `2.0/src/Cooperator.Framework.Migration/Cli/ExitCodes.cs` — `0/1/2/3` como constantes con nombre

#### D.2 Reporte por archivo (JSON + texto)

El reporte se emite en **JSON y en texto legible** y registra, por archivo: ruta de origen y destino, *magic
bytes* detectados, `modelerVersion` legado, conteos (entidades, propiedades, `related`, `queries`),
`schemaVersion` de salida, el **digest del modelo normalizado** y los hallazgos con severidad. Con `--report
<archivo.json>` se escribe el JSON en esa ruta exacta y el texto legible en su gemelo `<archivo>.txt` (mismo
nombre base); el resumen humano va además a la salida estándar. El orden de las entradas es **canónico** (ruta
de origen, ordinal) para que dos corridas del mismo conjunto produzcan el mismo reporte.

**Regla de datos (derivada de la clasificación `confidential`):** el reporte registra el **hecho** de que la
cadena de conexión quedó fuera del formato, **nunca su valor**; tampoco la imprime a la salida estándar
(§10, §8.1).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Reporting/ConversionReport.cs` — modelo del reporte y su resumen
- `2.0/src/Cooperator.Framework.Migration/Reporting/JsonReportWriter.cs` — reporte JSON determinista
- `2.0/src/Cooperator.Framework.Migration/Reporting/TextReportWriter.cs` — reporte legible para humanos

#### D.3 Atomicidad por archivo y conservación del original

Cada `.coop2` se escribe a un **temporal en el directorio de destino y se renombra** al nombre final: si algo
falla, **no queda salida parcial**. Un archivo bloqueado **no produce salida** de ningún tipo. El original
**nunca** se abre en modo escritura ni se borra. Si el destino ya existe, ese archivo queda **bloqueado con
diagnóstico** (exit `3`) y **no se sobrescribe**: sin bandera de sobrescritura en la superficie aprobada, la
opción conservadora es la correcta frente a una operación irreversible (§14).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Conversion/AtomicFileWriter.cs` — escritura por temporal + renombrado en el mismo directorio
- `2.0/src/Cooperator.Framework.Migration/Conversion/ConversionRunner.cs` — orquestación del conjunto: clasificar, verificar identidad, convertir por archivo, acumular reporte y resolver el código de salida

---

### Phase E — Fixtures dorados y control de datos anonimizados

**Duration:** 3h total cycle — **Complexity:** Medium

#### E.1 Estructura y procedencia de los fixtures

Los fixtures viven bajo `2.0/tests/fixtures/model-corpus/<slug>/` con la entrada `model.coop` (binaria,
anonimizada) y la salida esperada `model.coop2` (byte a byte). **Los mismos fixtures** son el contrato que el
Bolt 004 consume para el test bilateral `write(read(fixture)) == fixture` (ADR-002 §3.2): su ruta y su nombre
quedan fijados acá.

Dos procedencias, ambas explícitas en el índice:
1. **Sintéticos** — construidos herméticamente por el arnés de pruebas serializando un `Snapshot` **de
   laboratorio** (nombres de fantasía, sin cadena de conexión real) contra los tipos 1.4. Cubren las formas del
   contrato y los casos negativos.
2. **Subconjunto representativo del corpus real** — copias **anonimizadas** de modelos reales, aportadas por
   `human:eugenioserrano` (dueño del corpus), que cubren **todas las formas del modelo que aparecen en
   producción**, no un caso feliz.

**Files created:**
- `2.0/tests/fixtures/model-corpus/INDEX.md` — un renglón por fixture: procedencia (sintético \| anonimizado del corpus real), formas del modelo que cubre, si la salida esperada es **redactada a mano** desde ADR-002 §3.1 o **producida por la herramienta y revisada**, y el hash del `.coop` de entrada
- `2.0/tests/fixtures/model-corpus/<slug>/model.coop` — entrada legacy (sintética o anonimizada)
- `2.0/tests/fixtures/model-corpus/<slug>/model.coop2` — salida canónica esperada, byte a byte

#### E.2 Fixtures y proyecto de pruebas del migrador

Un fixture del conjunto —el de referencia— tiene su `.coop2` esperado **redactado a mano** desde ADR-002 §3.1 y
revisado: es la prueba de que el escritor no se está validando contra sí mismo. Los demás se producen con la
herramienta y se **verifican** con `--verify` más una revisión de su diff por el humano; el índice registra cuál
es cuál y el MEM lo repite. La circularidad la cierra el **test bilateral del Bolt 004** (t2, ADR-002 §3.2).

Casos negativos mínimos del conjunto: cabecera desconocida; texto plano; binario truncado; un tipo que no
deserializa; `Language = "VB"`; `GenerateCheckForToken = true`; `ConnectionString` no vacía; colisión de
`generateAs`; `primaryKey` que no resuelve; `related[].name` que no resuelve; `toString.arguments` fuera de las
propiedades; destino ya existente.

**Files created:**
- `2.0/tests/Cooperator.Framework.Migration.Tests/Cooperator.Framework.Migration.Tests.csproj` — proyecto de pruebas **`net48`** (el framework y las versiones son los que fija el Bolt 001; ver §5)
- `2.0/tests/Cooperator.Framework.Migration.Tests/FixtureBuilder.cs` — construcción de los fixtures sintéticos serializando el `Snapshot` de laboratorio contra los tipos 1.4
- `2.0/tests/Cooperator.Framework.Migration.Tests/GoldenFixtureTests.cs` — por fixture: exit esperado, salida byte a byte, conteos, digests y hallazgos
- `2.0/tests/Cooperator.Framework.Migration.Tests/ImmutabilityTests.cs` — SHA-256 del `.coop` antes y después de la corrida; ausencia de `--in-place`; destino existente bloqueado
- `2.0/tests/Cooperator.Framework.Migration.Tests/IdentityTests.cs` — identidad correcta, identidad manipulada (hash alterado) y token/versión discrepantes
- `2.0/tests/Cooperator.Framework.Migration.Tests/CanonicalWriterTests.cs` — orden de conjuntos vs. secuencias, `LF`, sin BOM, sin espacios finales, determinismo (dos corridas idénticas) y `schemaVersion`
- `2.0/tests/Cooperator.Framework.Migration.Tests/ReportTests.cs` — forma del reporte, severidades, códigos de salida y **ausencia del valor de la cadena de conexión** en reporte y salida estándar
- `2.0/tests/Cooperator.Framework.Migration.Tests/RoundTripTests.cs` — `--verify` sobre cada salida producida: digests coincidentes (t1)
- `2.0/tests/Cooperator.Framework.Migration.Tests/ExclusionTests.cs` — los campos excluidos (derivados, estado, `FileName`, flags retirados) **no** aparecen en ningún `.coop2`; aparece el hallazgo correspondiente
- `2.0/tests/Cooperator.Framework.Migration.Tests/DeprecationTests.cs` — VB bloquea por defecto y migra con la bandera; Oracle y Excel declarados como "nada que migrar"; gate por token informado

#### E.3 Control de datos: el ejecutor nunca procesa `.coop` reales

- El V-Bounce del Bolt 002 **no lee, no copia, no abre y no recibe `.coop` reales**. Su conjunto de pruebas es
  el corpus de fixtures comiteado.
- La anonimización del subconjunto representativo es un **acto del dueño del corpus**, previo y fuera de este
  Bolt; el `.coop` anonimizado se comitea y se revisa por su contenido (sin cadena de conexión, sin nombres ni
  identificadores del cliente).
- Si al iniciar el V-Bounce el subconjunto anonimizado no está en el repositorio, **parar y reportar** (§15):
  los fixtures sintéticos por sí solos **no** cumplen el criterio de completitud de t1 (ADR-002 §3.2) y está
  prohibido derivarlos procesando el corpus real.

---

### Phase F — Forma publicable y evidencia de t1

**Duration:** 2h total cycle — **Complexity:** Low

#### F.1 Forma publicable de la herramienta

Publicación como **carpeta/zip autocontenida** con `Cooperator.Framework.Migration.exe` y los ensamblados 1.4
fijados, **sin instalador**, **sin registro** y sin depender de `dotnet tool` (ADR-002 §3.2). El host GUI
opcional **no** se implementa acá: si se pidiera, es un consumidor del mismo core de conversión y entra por su
propio Bolt. La herramienta no se distribuye junto al Modeler (eso es el empaquetado del shell, Bolt 027).

**Files created:**
- `2.0/src/Cooperator.Framework.Migration/Distribution/README-herramienta.md` — uso, códigos de salida, garantías (original intacto, sin conversión inversa) y la advertencia de que la verificación de identidad puede detener la corrida

#### F.2 Evidencia de t1

El MEM de la ejecución registra el **estado t1** del ADR-002 §3.2 con: el `--dry-run` previo y su reporte, la
corrida de conversión sobre el conjunto de fixtures, el round-trip interno con digests coincidentes, la prueba
de inmutabilidad de los originales, la verificación de identidad y la lista de fixtures con su procedencia. La
evidencia se pega al MEM como salida de los comandos, no como resumen.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`), así que estos criterios **no** derivan de ACs de una US: son el
> resultado medible del §2 del Bolt y sus restricciones son los ADR que lo gobiernan.

### AC-1: La primera pasada no escribe nada

**Given** un directorio con modelos legacy, un `.coop2` ya existente y un archivo ilegible
**When** se corre `convert --dry-run` con ese directorio de entrada
**Then** no se crea ni modifica ningún archivo, y el reporte clasifica cada entrada (binario legacy / ya
`.coop2` / ilegible) con sus *magic bytes*

### AC-2: La conversión reproduce la salida esperada, byte a byte

**Given** el conjunto de fixtures dorados
**When** se corre `convert` sobre él
**Then** cada `model.coop2` producido es **idéntico byte a byte** al esperado comiteado, y el exit es `0` sin
warnings o `2` con ellos según el conjunto

### AC-3: El original queda intacto

**Given** un `.coop` de entrada con su SHA-256 registrado
**When** la conversión termina —con éxito, con bloqueo o con error—
**Then** el `.coop` conserva su SHA-256 y su fecha de modificación, el modo de escritura nunca se abrió sobre
él y `--in-place` no existe en la superficie de la herramienta

### AC-4: Un archivo no migrable se bloquea sin frenar el resto

**Given** un conjunto donde un archivo tiene una colisión de `generateAs`
**When** se corre `convert`
**Then** ese archivo queda bloqueado con su hallazgo de severidad `error`, **no produce ninguna salida**, los
demás se convierten, y el exit es `3`

### AC-5: VB.NET no se convierte en silencio

**Given** un modelo con `Language = "VB"`
**When** se corre `convert` sin `--allow-vb-to-csharp`
**Then** el archivo se bloquea con el error explícito del ADR-002 §3.3
**And When** se corre con `--allow-vb-to-csharp`
**Then** el modelo se migra con `language` reescrito a `C#` y un warning destacado en el reporte

### AC-6: La cadena de conexión no viaja y no se expone

**Given** un modelo legacy con `ConnectionString` no vacía
**When** se corre `convert` y se escribe el reporte
**Then** el `.coop2` producido contiene **cero** ocurrencias de esa cadena, el reporte emite el warning de
"secreto no trasladado" **sin su valor**, y ni el reporte ni la salida estándar contienen el valor

### AC-7: Una identidad discrepante detiene la herramienta antes de deserializar

**Given** un binario 1.4 alterado (hash, versión o token distintos de la lista fijada)
**When** se corre `convert`
**Then** la herramienta falla con exit `1` y un mensaje que nombra el binario y el campo discrepante, **sin**
haber deserializado ningún archivo de entrada

### AC-8: Round-trip interno con digests coincidentes (t1)

**Given** los `.coop2` producidos por la conversión de los fixtures
**When** se corre `convert --verify`
**Then** cada salida se re-lee con el lector canónico y su digest coincide con el de la construcción en memoria,
y la corrida termina en exit `0`

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — conversión verificable con salida conforme a ADR-002 §3.1 y round-trip con digests coincidentes (t1) | §6/C (escritor canónico + proyector) y §6/F.2 (evidencia t1) | AC-2, AC-8 · `GoldenFixtureTests`, `RoundTripTests` |
| Bolt §2 — originales conservados y atomicidad por archivo | §6/D.3 (`AtomicFileWriter`, sin `--in-place`) | AC-3, AC-4 · `ImmutabilityTests` |
| Bolt §2 — archivos no migrables bloqueados con diagnóstico sin detener el conjunto | §6/C.3 (`ModelValidator`) y §6/D.2 (reporte) | AC-4 · `GoldenFixtureTests`, `ReportTests` |
| Bolt §2 — fixtures dorados con el subconjunto representativo del corpus real | §6/E.1/E.2 | `INDEX.md` del corpus + lista de fixtures en el MEM |
| Bolt §2 — verificación de identidad de los binarios 1.4 | §6/A.2 (`BinaryIdentityVerifier` + lista fijada) | AC-7 · `IdentityTests` |
| Bolt §2 — clasificación de las deprecaciones según ADR-002 §3.3 | §6/C.3 | AC-5, AC-6 · `DeprecationTests`, `ExclusionTests` |
| Bolt §2 — ningún `.coop` real procesado (control de fixtures anonimizados) | §6/E.3 y §10 | `INDEX.md` (procedencia), gate PII/DLP y secret-leak de §9 |
| ADR-002 §3.2 — reporte por archivo con digests y severidades en los dos modos | §6/D.1 y §6/D.2 | AC-1, AC-6 · `ReportTests` |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests (herméticos):** clasificador por *magic bytes* (legacy, `.coop2`, ilegible, inexistente);
  verificador de identidad (nombre/versión/token/hash: caso correcto y tres casos de discrepancia); proyector
  por campo (los campos de la tabla de §C.1, uno por uno, incluidos los "no se serializa" y los "no entran
  nunca"); validador semántico (una prueba por regla del ADR-002 §3.2 y una por colisión de `generateAs`);
  escritor canónico (orden de conjuntos vs. secuencias, `LF`, sin BOM, sin espacios finales, dos corridas
  idénticas ⇒ bytes idénticos, `schemaVersion`); resolución de códigos de salida; formato del reporte.
- **Component tests:** la herramienta completa **en proceso**, con un directorio de trabajo temporal y el
  conjunto de fixtures: `--dry-run` (nada escrito — se compara el inventario y los hashes del directorio),
  conversión completa, `--verify`, destino existente, y el caso de identidad manipulada (exit `1`).
- **Consumer-side contract tests (test doubles):** `n/a` — el migrador no consume ningún servicio ni componente
  externo; su contrato de salida es el **archivo** y se verifica byte a byte (no hay doble que valga).
- **Edge cases:** documento vacío; un solo byte; cabecera legacy y cuerpo vacío; `.coop2` con `schemaVersion`
  mayor al soportado (se clasifica "ya `.coop2`", no se toca); modelo sin entidades; entidad sin propiedades;
  nombres con mayúsculas/espacios/acentos (colisión de identificadores); `primaryKey` vacía; `childOf` con valor
  que no está en candidatos; `toString.arguments` repetidos (secuencia: se conserva el orden); `related` con
  nombres cruzados; rutas con espacios y con nombres largos; entrada por `glob` que no matchea nada; `--output`
  inexistente (se crea) y `--output` que es un archivo; archivo de entrada bloqueado por el sistema operativo.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto
  (ese protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la salida del `--dry-run`, la corrida de conversión con su reporte
  JSON y de texto, la salida de `--verify`, los hashes antes/después del directorio de entrada, la corrida del
  proyecto de pruebas (verde) y el inventario de fixtures con su procedencia. Todo eso se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `refactor`: convierte archivos del repositorio
  y su verificación es el round-trip interno y los fixtures dorados. No hay base de datos, servicio, cola,
  almacenamiento ni entorno desplegado contra el que verificar nada (ADR-002 §3.2: la conversión es puramente
  de formato y **no abre conexión**).
- **Contract / E2E against the real providers:** `n/a` — la conformidad del formato se cierra en dos niveles
  que **no** son de integración: t1 acá y el **test bilateral** sobre los mismos fixtures en el Bolt 004
  (ADR-002 §3.2). El batch real sobre el corpus es el Bolt 029, con su propia clasificación de riesgo.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales.
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (el árbol trata warnings como error, Bolt 001) | pass |
| Unit / component tests | V-Bounce | 100 % verdes; la suite de fixtures completa | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el migrador no consume componentes externos; su contrato es el archivo de salida, verificado byte a byte | n/a (razón) |
| SAST | V-Bounce | 0 hallazgos altos en el código nuevo | pass |
| Dependency scanning / licenses | V-Bounce | Dependencias sólo las autorizadas por ADR-001 §3.4 y ADR-002 §3.2 (`System.Text.Json` + referencias al árbol 1.4 congelado); licencias permisivas; versiones centralizadas | pass |
| OWASP Top 10 — static review | V-Bounce | Revisión de deserialización no confiable: el riesgo está **declarado y aceptado** por ADR-002 §3.2/ADR-008 §3.4, con controles compensatorios (sólo archivos del corpus controlado; herramienta offline, sin superficie de red ni de servicio; verificación de identidad previa a deserializar; única excepción con fecha de retiro) y sin hallazgos nuevos | pass |
| Prompt-injection scan | V-Bounce | `n/a` — el entregable no interpreta contenido textual como instrucciones ni expone material a un agente en runtime; lee binario validado por identidad | n/a (razón) |
| Secret-leak scan | V-Bounce | 0 ocurrencias de cadena de conexión (ni de material de credenciales) en todo `.coop2` producido, en el reporte, en la salida estándar y en los fixtures comiteados | pass |
| Hallucination lint | V-Bounce | Toda afirmación del MEM y del reporte trazable a una corrida registrada o a un ADR citado; 0 campos del formato fuera de ADR-002 §3.1 | pass |
| IP / license provenance | V-Bounce | Código propio portado del repositorio; 0 dependencias nuevas de terceros; el código 1.4 conserva su encabezado de licencia al ser referenciado (no copiado) | pass |
| PII / DLP | V-Bounce | Fixtures **anonimizados** y sintéticos; 0 datos personales ni de cliente; 0 `.coop` reales procesados en este Bolt | pass |
| Dependency-confusion | V-Bounce | 0 paquetes nuevos con ID no calificado; sólo el paquete autorizado y fijado en `Directory.Packages.props` | pass |
| Test-first evidence | V-Bounce | El fixture de referencia tiene su salida esperada **redactada a mano** desde ADR-002 §3.1 antes de escribir el escritor; el resto de las salidas esperadas se producen sólo después de la revisión humana y quedan marcadas como tales en el `INDEX.md` | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la conversión del mismo conjunto producen bytes idénticos (salidas y reporte) | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: la conformidad se cierra en t1 (acá) y en el test bilateral del Bolt 004 | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: herramienta offline de batch; su costo se mide en el batch real (Bolt 029) | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: no hay servicio ni entorno que observar (§11) | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `confidential`** (espeja el Bolt). El material que justifica esa
  clasificación es la **cadena de conexión que viaja dentro de los modelos legacy** (F-23: en claro u ofuscada
  con clave embebida en el IL — se trata como secreto).
- **El secreto no se traslada y no se expone:** la `connectionString` escapa del formato (ADR-002 §3.1), se
  reporta como hallazgo **sin su valor**, y nunca se imprime a la salida estándar ni se escribe en el reporte.
  Es un criterio de aceptación (AC-6) y un gate (secret-leak scan).
- **Control de fixtures (explícito, es el corazón del control de datos):** el ejecutor de este Bolt **nunca**
  procesa `.coop` reales. Los fixtures son sintéticos (construidos en el V-Bounce) o **anonimizados por el
  dueño del corpus** antes de entrar al repositorio; el `INDEX.md` declara la procedencia de cada uno y la
  ausencia de cadena de conexión es verificable por inspección y por el escaneo del gate. El corpus real se
  convierte en el Bolt 029, con su propio respaldo y su propio registro.
- **Deserialización no confiable — riesgo declarado:** la herramienta deserializa un archivo que en el 1.4 podía
  venir de terceros. Los controles compensatorios están fijados por ADR-002 §3.2/ADR-008 §3.4: la verificación
  de identidad ocurre **antes** de deserializar; el uso de `BinaryFormatter` está confinado a **un** archivo del
  árbol 2.0; la herramienta es offline (sin puertos, sin servicio, sin GUI) y su operación es un batch sobre un
  corpus controlado; la capacidad legacy de descifrado no la habilita este Bolt (ADR-007 §3.3/§3.4) y su retiro
  tiene Bolt propio (030).
- **Integridad de los binarios que se cargan:** la lista fijada (nombre + versión + SHA-256 de contenido, y
  `PublicKeyToken` donde exista) es un control anti-deriva/anti-manipulación: un binario rehecho o pisado
  detiene la herramienta en lugar de deserializar contra tipos parecidos.
- **Sin superficie nueva de red ni de escritura ajena:** el migrador sólo escribe en el directorio de salida
  indicado, un archivo por modelo, de forma atómica; no borra nada y no toca el original.
- **Cero `SYSLIB0051` y cero `BinaryFormatter` fuera de la excepción:** la política de ADR-008 §3.4 se verifica
  en el build y con un chequeo de fuente (un único archivo contiene el uso).

---

## 11. Monitoring and observability

- **`n/a` en el sentido de telemetría de servicio:** el migrador es una herramienta CLI offline; no emite logs,
  métricas ni trazas a ningún colector y **no** se le agrega instrumentación (no hay a quién observarla, y
  agregarla sería alcance inventado).
- **La observabilidad real de este Bolt es su evidencia operativa**: el **reporte por archivo** (JSON y texto)
  con clasificación, conteos, digests y hallazgos por severidad; los **códigos de salida** `0/1/2/3`; y el
  **detalle de discrepancia** del verificador de identidad. Un agente o un humano audita una corrida leyendo el
  reporte, no un tablero.
- **Trazabilidad para el batch real (Bolt 029):** el reporte es la única evidencia del batch (ADR-002 §4,
  "Technical debt"), así que su formato es **estable y determinista** por diseño y se archiva junto al corpus
  convertido. Este SPEC no cambia ese formato por conveniencia: un cambio del reporte es una revisión material.

---

## 12. Migration, compatibility and rollback

- **Migration:** el entregable **es** una migración de formato, y su naturaleza queda fijada así: la dirección
  es `.coop` → `.coop2` y **no hay conversión inversa** (ADR-002 §3.2). Los `.coop` originales **se conservan**
  —nunca se modifican ni se borran— hasta que el release 2.0.0 esté validado, y recién entonces pueden
  retirarse del control de versiones dejando los fixtures anonimizados como evidencia. El batch sobre el corpus
  real es del **Bolt 029** y es **provisional hasta t2**: si el Bolt 004 revela una divergencia, se corrige el
  escritor y se **vuelve a migrar desde los `.coop`** (intactos).
- **Compatibility:** el `.coop2` que produce el migrador es **provisional** y debe ser byte-compatible con el que
  produzca `Modeling` (ADR-002 §3.2): por eso el escritor canónico es determinista, ordenado por esquema y sin
  campos fuera de ADR-002 §3.1. La herramienta **no** cambia ninguna superficie pública del framework ni del
  consumidor: es un ejecutable nuevo en un slot ya reservado por ADR-001 §3.1, por lo que **no** aporta rupturas
  a las notas de migración (Bolt 028). Excepción declarada y acotada: el uso de `BinaryFormatter`, la única
  excepción del producto, con su fecha de retiro y registrada en la política de ADR-008 §3.4.
- **Rollback:** la conversión es **reversible por abandono**, no por una herramienta: los originales intactos
  son la red de recuperación y no hay estado que deshacer más allá de los archivos de salida. Para revertir:
  descartar el directorio de salida (y su reporte) y volver a correr la herramienta con el escritor corregido
  sobre los mismos `.coop`. El código se revierte revirtiendo el commit del V-Bounce. **No** se desinstala nada:
  no hay instalador, ni registro, ni servicio.
- **Compatibility con el árbol 1.4:** nula edición del árbol congelado; la herramienta lo **referencia**. Si el
  build 1.4 no fuera reproducible, la lista de identidad fallaría ruidosamente en lugar de leer binarios
  distintos de los acordados — que es el comportamiento buscado, no un defecto a esquivar.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Pérdida de un modelo por una conversión mal hecha** (el paso es irreversible) | 2 | 5 | El original nunca se modifica; escritura atómica por archivo; bloqueo con diagnóstico sin salida parcial; fixtures dorados + round-trip interno antes de cualquier batch; el batch real es otro Bolt con respaldo previo |
| **Material sensible (cadena de conexión) filtrado en salida, reporte o fixtures** | 2 | 5 | Campo excluido por diseño + hallazgo sin valor + AC-6 + gate secret-leak + PII/DLP; fixtures anonimizados por el dueño; prohibición explícita de procesar `.coop` reales en este Bolt |
| **Deriva de formato entre el migrador y `Modeling`** (contrato duplicado: `net48` no consume `netstandard2.1`) | 4 | 4 | Mismo esquema (ADR-002 §3.1), misma versión exacta de `System.Text.Json` fijada en `Directory.Packages.props`, salida canónica determinista, fixtures dorados comiteados que el Bolt 004 usa para su test bilateral (t2 cierra la conformidad; el corpus del batch es provisional hasta t2) |
| **Escritura de un `.coop2` que el lector canónico no pueda reproducir** (orden o escape divergentes) | 3 | 4 | Escritor sin reflexión, orden fijo por esquema, mismas reglas de escape, `--verify` obligatorio en la evidencia y el test bilateral del Bolt 004 sobre los mismos fixtures |
| **Identidad de binarios 1.4 no reproducible en la máquina de build** (build rehecho ⇒ hashes distintos) | 3 | 4 | La lista se genera del build reproducible del Bolt 001 y se comitea; una discrepancia **detiene** la herramienta; el pre-requisito de build es explícito (§5) y su ausencia es condición de parada (§15) |
| **Deserialización de un archivo manipulado o corrupto** | 3 | 4 | Prueba de identidad **antes** de deserializar; detección por *magic bytes*; error duro con tipo y mensaje; operación offline sobre corpus controlado; excepción única y con fecha de retiro (ADR-008 §3.4) |
| **Colisión o identificador inválido que produciría código roto aguas abajo** | 3 | 3 | Validación semántica previa a la escritura; la colisión es **error** que bloquea ese archivo, no un warning |
| **VB.NET migrado en silencio a C#** | 2 | 4 | Bloqueo por defecto; la bandera es explícita y por corrida; warning destacado; la decisión del operador queda en el reporte |
| **Sobrescribir un `.coop2` existente** (pérdida de una migración previa o de un archivo editado a mano) | 2 | 4 | Destino existente ⇒ archivo bloqueado con diagnóstico; no existe bandera de sobrescritura en la superficie aprobada |
| **El esquema normativo del Bolt 004 difiere de la forma emitida acá** | 3 | 3 | Los fixtures son el contrato: un cambio de esquema obliga a regenerarlos y a re-migrar desde los `.coop` (ADR-002 §3.2); el `schemaVersion` de salida es `1` y su subida exige un ADR que reemplace a ADR-002 |
| **Framework de pruebas del arnés incompatible con `net48`** | 2 | 3 | §5 lo declara como prerrequisito y §15 como condición de parada: no se introduce un segundo framework por conveniencia (ADR-001 §3.4) |
| **Falta el pack de referencia de .NET Framework 4.8 en la máquina de build** | 3 | 3 | Prerrequisito explícito (§5) y condición de parada (§15): la única salida es una decisión humana, nunca una dependencia de build inventada |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC que no merecen un ADR propio. Las decisiones de arquitectura ya estaban tomadas
> en ADR-001/002/008 y acá sólo se aplican.

| Decision | Reason |
|----------|--------|
| El escritor canónico del migrador es una **implementación propia duplicada** sobre `System.Text.Json`, no un paquete compartido con `Modeling` | `net48` no puede consumir `netstandard2.1`: la duplicación es la decisión de ADR-002 §3.2/§3.4. Se mitiga con la versión exacta común del paquete y con los fixtures dorados |
| La lista de identidad vive **dentro del proyecto** del migrador (`Identity/legacy-binaries.json`) y se regenera con un script del repositorio | Es un dato de la herramienta (qué binarios acepta), no del árbol 1.4 congelado, que no se toca. El script hace la regeneración reproducible y auditable |
| El proyecto de pruebas del migrador es **`net48`** | El migrador es `net48` y el fixture sintético se construye serializando contra los tipos 1.4: un host moderno no puede referenciar el proyecto ni ejercitar ese camino |
| El fixture de referencia tiene su `.coop2` esperado **redactado a mano** desde ADR-002 §3.1 | Un golden que se genera con la implementación y luego se "bendice" valida la implementación contra sí misma. El `INDEX.md` marca qué fixtures son los verificados a mano |
| Los fixtures **sintéticos** son parte del conjunto y no dejan de serlo | El subconjunto anonimizado del corpus real cubre las formas de producción; los sintéticos cubren los casos negativos imposibles o indeseables de reproducir desde el corpus (VB, colisión, truncado) |
| **Destino existente ⇒ bloqueo**, sin bandera de sobrescritura | La superficie de banderas está fijada por ADR-002 §3.2 y no incluye una bandera de sobrescritura; frente a una operación irreversible, la opción conservadora es la correcta y no se inventa una bandera nueva |
| El `--dry-run` **no se fuerza** en el programa | El ADR lo declara obligatorio en la primera pasada como práctica operativa; la herramienta expone las dos banderas y la evidencia del V-Bounce exige el `dry-run` previo. Forzarlo crearía un estado oculto entre corridas |
| El reporte de texto se escribe en `<archivo>.txt` junto al JSON pedido por `--report` | El ADR exige los dos modos; fijar la convención evita una bandera extra y deja el reporte legible junto al JSON que consumen agentes y CI |
| `--verify` se implementa con un **lector canónico propio** del `.coop2` producido | El round-trip interno de t1 necesita releer con las mismas reglas de orden y escape; el lector es parte de la herramienta y su conformidad definitiva se cierra con el test bilateral del Bolt 004 |
| No se separa la lógica "puramente de formato" del `Snapshot` en más tipos de los necesarios | Cada tipo extra de contrato en el lado `net48` es otra pieza que puede derivar de `Modeling`; el contrato duplicado se mantiene **mínimo** y su forma se prueba por bytes |

---

## 15. Stop conditions

- **El build 1.4 reproducible (o el build conocido-bueno con sus hashes) del Bolt 001 no existe** al iniciar el
  V-Bounce: sin él no hay contra qué compilar ni identidad que verificar. Parar y reportar.
- **El corpus anonimizado representativo no está comiteado** y sólo hay fixtures sintéticos: t1 quedaría
  incompleto y está prohibido derivarlos procesando los `.coop` reales. Parar y reportar.
- **Falta el pack de referencia de .NET Framework 4.8**, o el framework de pruebas fijado por el Bolt 001 **no
  corre sobre `net48`**: parar y pedir decisión (no se agregan dependencias no autorizadas por ADR-001 §3.4).
- **El esquema que el Bolt 004 termine fijando contradice la forma emitida acá** (nombres de campo, orden o
  reglas de escape): parar; la corrección es una revisión material de este SPEC más la regeneración de los
  fixtures, nunca "acomodar" el fixture a la salida.
- **Aparece la necesidad de un campo del formato que ADR-002 §3.1 no declara** (p. ej. distinguir vistas de
  tablas): parar. Ese campo es una capacidad nueva que exige un ADR que suba `schemaVersion`; el migrador
  reporta la limitación, no la resuelve.
- **Un archivo real del corpus aparece en el conjunto de pruebas** (o alguien pide procesarlo para "generar
  fixtures"): parar. El control de datos de §10 no admite excepciones dentro de este Bolt.
- **La deserialización de un fixture revela un layout que no es `1.4.5.0`** y no se puede explicar con la
  evidencia aprobada: parar y reportar; no se ajusta la herramienta a ciegas para que "pase".
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **El digest no es reproducible** (dos corridas del mismo conjunto producen bytes distintos): parar; el
  determinismo es un NFR de ADR-002 §5 y no un detalle de implementación.
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-002 §3.1–§3.4, ADR-001 §3.1/§3.2/§3.4, ADR-008 §3.2/§3.4)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-002`](../functional/bolts/US-000.BOLT-002-migrador-coop-a-coop2.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-41 (mecanismo binario del `.coop`, prerequisito irreversible de secuencia), F-42 (orden canónico, derivados, `schemaVersion`), F-43 (`BinaryFormatter` y `[Serializable]`), §6 ítem 3.
- [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (formato), §3.2 (migrador, identidad de binarios, t1/t2, irreversibilidad), §3.3 (deprecaciones), §3.4 (contrato en `Modeling`, secuencia), §5 (NFRs de determinismo y modelo sin secretos).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (slot `Cooperator.Framework.Migration` `net48`, layout `2.0/`, versiones centralizadas), §3.2 (secuencia de migración del consumidor), §3.4 (política de dependencias), §3.5 (supervivencia Windows-only explícita).
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.2 (el grafo del modelo y `Snapshot` dejan la serialización binaria), §3.4 (única excepción de `BinaryFormatter`, cero `SYSLIB0051`, supresión en el ámbito más estrecho).
- [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — build 1.4 reproducible, arnés y versiones centralizadas (prerrequisito).
- [OQ-004](../analysis/open-questions/OQ-004-formato-del-coop-json.md) — extensión `.coop2`, migrador obligatorio sobre .NET Framework 4.8, operación irreversible.
- [`US-000.BOLT-029`](../functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md) — el batch sobre el corpus real (fuera de este SPEC).
- [`US-000.BOLT-004`](../functional/bolts/US-000.BOLT-004-contrato-coop2-en-modeling.md) — el contrato normativo del `.coop2` y el test bilateral que consume estos fixtures.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-002`: pre-SPEC evidence gate (§3), fases A–F, criterios AC-1…AC-8, estrategia de pruebas, gates, control de datos `confidential` y condiciones de parada. `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (arquitectura / datos según el riesgo `high`)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
