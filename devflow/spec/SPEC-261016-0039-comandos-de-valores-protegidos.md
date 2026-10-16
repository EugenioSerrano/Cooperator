---
id: "SPEC-261016-0039"
title: "Comandos de valores protegidos: inspección, migración y rotación con la capacidad legacy aislada, bajo su excepción acotada"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-023" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-007", "ADR-004", "ADR-003", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, lane de integración y build compartido (Bolt 001)
  - "SPEC-261016-0025-cli-del-motor.md" # el CLI, su vocabulario de comandos, su reporte estable y su mapeo de códigos de salida (Bolt 018)
  - "SPEC-261016-0035-resolutor-aesgcm.md" # el motor de protección: esquema vigente, clave activa, `kid`, AAD y provisión de claves (Bolt 022)
risk_class: "high" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L2" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "confidential" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "applicable" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests:            # applicable — the integration tests a run executes: escritos por este V-Bounce (ver §8.2)
    - "2.0/tests/Cooperator.Cli.Integration.Tests/SecretsInspectTests.cs (Category=Integration) — el cuadro de clasificación sobre almacenes reales: valor vigente (`aesgcm:`), identidad explícita (`plain:`), candidato del formato viejo legible, ilegible y con pérdida de información en origen, cada uno con su clasificación y su motivo"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/SecretsDefaultNoWriteTests.cs (Category=Integration) — la corrida de migración **sin** la opción de escritura no modifica ningún almacén (comparación byte a byte de los almacenes antes y después)"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/SecretsMigrationTests.cs (Category=Integration) — la migración real con escritura explícita: validación del resultado antes de reemplazar, respaldo del original, reemplazo efectivo y reporte final con migrados, pendientes y `kid` usado"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/SecretsReprotectTests.cs (Category=Integration) — rotación a la clave activa y re-targeteo por AAD contra el almacén real: el valor re-protegido resuelve bajo la clave/clave-de-configuración nueva y el movimiento de slot sin re-proteger falla explícitamente"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/LegacyIsolationTests.cs (Category=Integration) — el componente de descifrado legacy no es alcanzable desde el runtime desplegado ni desde los paquetes portables (inspección de referencias de los ensamblados del runtime y de los paquetes del folder feed), y su uso exige la bandera que reconoce el carácter público de la clave vieja"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/FipsContextTests.cs (Category=Integration) — la corrida bajo contexto criptográfico restringido: el comando detecta y reporta con mensaje accionable qué valor no pudo evaluar, nunca 'sin candidatos'"
    - "2.0/tests/Cooperator.Cli.Integration.Tests/SecretsReportTests.cs (Category=Integration) — el reporte final identifica por clave de configuración y `kid` y no contiene ningún valor, clave ni material derivado"
  environment: "el entorno de CI del proyecto con SQL Server y con una instalación de prueba **escribible** de los almacenes de configuración (carpeta temporal del entorno); si el SPEC del arnés (Bolt 001) fija un nombre de entorno, se usa ese"
  data: "almacenes sintéticos preparados por el aprovisionamiento de las pruebas: un archivo de configuración con un valor `plain:`, un candidato del formato viejo legible, un valor ilegible y uno con pérdida de información en origen (no ASCII), más una variable de entorno y una segunda clave (`kid`) para la rotación; una base de datos de prueba para el smoke de validación; todas las claves del entorno de prueba **generadas para la corrida** (nunca comiteadas) y ningún valor de cliente real"
  reason: ""        # n/a — no aplica: el plan es `applicable`
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:47:21-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:47:21-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:47:21-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-023 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:09:32; acknowledgment «único lugar autorizado a primitivas legacy…; F-23 pendiente como frontera»), ADR-007/ADR-004/ADR-003/ADR-001 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class high → autonomy_level L2; data_classification confidential; test_integration required → applicability applicable. Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: familia `secrets inspect|migrate|reprotect` agregada al vocabulario del CLI (Bolt 018) sin redefinirlo y con mapeo único de códigos (0/2/1/3, sin usar `4`); clasificación explícita por forma con reglas publicadas + reporte previo obligatorio; componente legacy aislado con las 5 condiciones verificables (ámbito interno, `--allow-legacy-decrypt`, supresión por miembro, sólo descifra, retiro atado al 030) + probe de contexto criptográfico restringido (FIPS); procedimiento por valor (clasificar → descifrar → validar → re-proteger → respaldar → registrar) con rotación y re-targeteo por AAD; registro de vencimiento (dueño/condición/revisita); búsqueda del texto plano en todo lo escrito = 0 (AC-6)."
    - "ADJUDICACIÓN (a) reglas de forma publicadas acá — CONFIRMADA: §B.1 fija y publica la tabla de categorías (vigente/identidad/candidato/ilegible/no migrable/no evaluable) con su regla exacta; §14 fila 1 la deja a la vista del aprobador. Es verificable por la suite, no heurística."
    - "ADJUDICACIÓN (b) código `2` (warning) para «no migrable», no `3` — CONFIRMADA: §A.1 y §14 fila 2 mapean no migrable/pendiente a `2` (éxito con warnings) y reservan `3` para entrada inválida; `4` queda exclusivo de `verify` (Bolt 018). Un pipeline no confunde «resultado pendiente» con «error de uso»."
    - "ADJUDICACIÓN (c) probe FIPS por seam en hermético y contra la máquina real en integración — CONFIRMADA: §8.1 verifica el comportamiento accionable con un seam que reporta «restringido»; §8.2/§9 ejecutan `FipsContextTests` contra el estado criptográfico real de la máquina y, si no está restringido, el MEM registra que ese camino se verificó por seam (§14 fila 3). Sin forzar FIPS en el CI; sin reportar «sin candidatos»."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-023`. El motor de protección y
     la provisión de claves son del Bolt 022 (**consumidos**); el CLI, su vocabulario y sus códigos de salida son
     del Bolt 018 (**usados**); el **retiro** de la capacidad legacy y el **cierre** de su registro son del
     **Bolt 030**; la corrección del defecto de F-23 es de **su BUG/Bolt**; el shell delgado del encriptador es
     del Bolt 024.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `applicable`, porque el Bolt declaró `test_integration: required` — la
     corrida reescribe **almacenes reales** y valida contra una base: se ejecuta en el ciclo de test de
     integración del Bolt, después de `AITL-MEM-Approval`, y su resultado queda como hecho en
     `integration_test_runs[]` del manifiesto.
  ⚠️ EXCEPCIÓN ACOTADA (ADR-007 §3.4): este Bolt es **el único lugar del árbol autorizado** a usar las
     primitivas del formato viejo, y sólo con sus cinco condiciones: ámbito interno, bandera explícita,
     supresión **por miembro**, **sin generalización** y **con retiro** (Bolt 030). La verificación de esas
     condiciones es parte de la aceptación, no un comentario.
-->

# SPEC-261016-0039 — Comandos de valores protegidos: inspección, migración y rotación con la capacidad legacy aislada, bajo su excepción acotada

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 12) |
| **Bolt** | [US-000.BOLT-023](../functional/bolts/US-000.BOLT-023-comandos-de-valores-protegidos.md) |
| **ADRs** | [ADR-007](../adrs/ADR-007-politica-criptografica.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se entrega la **familia de comandos de valores protegidos** del CLI: **inspeccionar** los valores de una
instalación y clasificarlos **explícitamente** —vigente, identidad, candidato del formato viejo, ilegible y no
migrable por pérdida de información en origen—; **migrar** los que quedaron cifrados con el formato de 1.4
**sin heurística** —reportando primero, **sin escribir** salvo opción explícita, **validando antes de reemplazar**
y con **respaldo** del original—; y **rotar / re-targetear** los valores ya protegidos, con la **capacidad capaz
de leer el formato viejo aislada** en un componente interno bajo la **excepción acotada** de la política
(ADR-007 §3.4: ámbito interno, bandera explícita que reconoce que la clave del 1.4 es **pública**, supresión de
advertencias **por miembro**, **sin generalización** y **con retiro** agendado), más la **detección del contexto
criptográfico restringido** con mensaje accionable y el **registro de vencimiento** de esa capacidad (dueño,
condición de cierre y fecha de revisita).

El problema que resuelve está documentado (REV-001 F-23 y §6 ítem 12): las cadenas de conexión de 1.4 están
"cifradas" con una clave **pública** (está en el binario), un **IV fijo** y `Encoding.ASCII` — ofuscación con
pérdida real de información en valores no ASCII (ADR-007 §3.3). Sin una herramienta que **inventaríe, migre y
rote** con evidencia, esos valores quedan inaccesibles para la 2.0 (que ya no implementa ese formato) o se
re-cifran mal.

Si no se implementa: el corpus de valores de cada cliente queda sin camino de migración, no hay `kid` ni esquema
de salida que reportar, y la política criptográfica queda sin su herramienta de cumplimiento.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 12 separó el **BUG de seguridad** (el defecto del cifrado
legacy en ejecución) de la **política criptográfica** y de **sus herramientas**; ADR-007 §3.3 decidió el
principio **"migrar, no re-cifrar"** —con la capacidad legacy **aislada y exceptuada**, y con **vencimiento con
registro**— y el §3.5 ubicó los comandos en el **CLI** (`net10.0`), con el encriptador de escritorio como shell
delgado **aditivo**.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-007 §3.3 (migración)** — **detección explícita, nunca heurística**: el conversor **no adivina** qué está
  cifrado; examina las entradas que se le indican, clasifica **candidatos** por forma (Base64 válido + longitud
  múltiplo del bloque de 8 bytes del algoritmo legacy) y **reporta antes de tocar nada** (`--dry-run` por
  defecto, `--apply` explícito). La **capacidad de descifrado del formato de 1.4** vive **sólo** en la
  herramienta, en un **componente interno**, detrás de una **bandera que reconoce por escrito que la clave del
  formato viejo es pública**; **nunca** entra al runtime del framework ni a un paquete que se despliega en
  producción, y **se retira del producto** cuando el corpus esté migrado. **Vencimiento con registro (no "algún
  día")**: dueño, **condición de cierre** (100 % de los valores clasificados como candidatos legacy convertidos a
  `aesgcm:` o declarados no migrables con su motivo) y **fecha de revisita obligatoria** (una fecha concreta).
  **Procedimiento por valor, con evidencia**: (1) inventariar y clasificar; (2) descifrar el candidato legacy;
  (3) **validar el resultado** —parseo de la cadena y, cuando sea posible, un **smoke test de conexión**—;
  (4) re-proteger bajo `aesgcm:` con la clave **activa**; (5) **reemplazar sólo tras la validación**, con
  **respaldo** del original; (6) registrar en el reporte qué se migró, qué quedó pendiente y por qué. El texto
  plano **nunca** se escribe en disco, en logs ni en el reporte. **Límite que no se repara solo**: como 1.4 cifraba
  con `Encoding.ASCII`, todo valor con caracteres no ASCII **ya perdió información** en origen: el conversor lo
  **detecta y lo marca como no migrable automáticamente** (la re-protección parte del dato real con intervención
  humana; no se adivina la reparación). **`reprotect`** re-protege valores ya `aesgcm:` bajo la clave activa
  (rotación) o los **re-targetea** cuando cambian de slot (por el AAD de §3.1), con el mismo esquema de validación
  y respaldo.
- **ADR-007 §3.4 (excepción única y acotada)** — el componente de descifrado legacy es la **única** pieza del
  árbol autorizada a usar las primitivas del formato viejo, y sólo con **cinco condiciones**: (1) **ámbito**:
  componente **interno** de la herramienta de migración, aislado del camino de runtime, **nunca** en
  `Core`/`Library`/`Data` ni en ningún paquete desplegado; (2) **bandera explícita** que reconoce por escrito que
  la clave del formato 1.4 es pública y que el descifrado es una operación de migración; (3) **supresión en el
  ámbito más estrecho** (las advertencias de obsolescencia se silencian **por miembro**, con comentario que cita
  la excepción y su condición de retiro; **nunca** a nivel de proyecto o solución); (4) **sin generalización**
  (ninguna otra pieza reutiliza ese código para "cifrar": la herramienta **no protege** con el formato viejo);
  (5) **retiro** junto con el cierre del inventario. Y las **primitivas prohibidas** del §3.4 (MD5/SHA-1,
  DES/3DES/RC2, ECB, IV/nonce fijos, claves embebidas, comparación de tags no constante, `*CryptoServiceProvider`
  concretos) siguen prohibidas **fuera** de esa excepción.
- **ADR-007 §3.5 (herramientas)** — los comandos viven en el **CLI** (`net10.0`) y **este ADR fija los comandos**
  (`cooperator secrets inspect|migrate|reprotect`, nombres tentativos); **ADR-003 §3.2 los reservó** al dejar el
  conversor de connection strings como candidato del CLI. Corren en `net10`, **no** en `net48` (el `net48` del
  migrador de modelos existe por `BinaryFormatter`, no por criptografía). **Contexto FIPS**: con FIPS habilitado,
  las primitivas legacy **lanzan**; la herramienta debe **detectarlo y reportarlo** con un mensaje accionable
  (qué valor no se pudo descifrar y por qué), **nunca** fallar en silencio ni reportar "sin candidatos" cuando en
  realidad no pudo evaluar. **Reproducibilidad**: la herramienta reporta el `kid` usado y el esquema de salida;
  **no inventa claves** ni genera "valores por defecto" cuando falta la configuración (falla con el error tipado
  correspondiente).
- **ADR-004 §3.2/§3.3** — los valores se leen del **contrato de configuración** (`ICooperatorConfiguration`) con el
  **prefijo de esquema obligatorio**; el **motor de detección por substrings está eliminado** y **quien invoca al
  resolutor es la capa que consume el valor**; un valor sin prefijo o con prefijo desconocido es **error tipado**
  (nunca un intento de descifrado a ciegas).
- **ADR-003 §3.2/§3.6** — el **CLI**, su **reporte estable** y su **vocabulario de códigos de salida** (`0`
  éxito, `1` error inesperado, `2` éxito con warnings, `3` entrada inválida, `4` el mundo no coincide con el
  modelo —propio de `verify`—) son del Bolt 018: esta familia **se agrega** a ese vocabulario y **no** lo
  redefine; y **una sola implementación** de la capacidad, consumida por CLI y shell.
- **ADR-001 §3.1/§3.4/§3.6** — el **slot** y el TFM del CLI; la **política de dependencias** (sin terceros para
  criptografía); y el **canal de distribución**, ajeno a este Bolt.

**Qué decide este SPEC, dentro de esas restricciones.** El **contrato de cada comando** (flags, entradas,
salidas y códigos), la **forma exacta de la clasificación** (las reglas de forma por las que un valor es
candidato, ilegible o no migrable), **dónde vive físicamente** el componente legacy y cómo se garantiza que **no**
sea alcanzable desde el runtime, el **formato del reporte** (por clave de configuración y `kid`, sin contenido),
el **esquema de respaldo** antes del reemplazo y la **forma del registro de vencimiento** que este Bolt
**contribuye** y el Bolt 030 cierra.

**Fronteras declaradas.** El **motor de protección**, sus esquemas, la **provisión de claves** y el AAD son del
**Bolt 022** (se consumen); el **CLI**, su vocabulario y sus códigos son del **Bolt 018** (se usan); el **retiro**
de la capacidad legacy y el **cierre** del registro son del **Bolt 030**; la corrección del **defecto** de F-23
es de su **BUG/Bolt**; el **shell delgado del encriptador** es del **Bolt 024**; el **formato del modelo** y el
corpus `.coop` son de los Bolts 002/004/029.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt (`required` → `applicable`).

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-023`](../functional/bolts/US-000.BOLT-023-comandos-de-valores-protegidos.md) — `status: ready`, `risk_class: high`, `data_classification: confidential`, `test_integration: required`; el `acknowledgment_reason` registra que es el único lugar autorizado a las primitivas legacy y que F-23 sigue pendiente como frontera | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — **F-23** (el defecto verificado del cifrado de 1.4 y la necesidad del plan de migración de los valores ya cifrados), **F-40** (el encriptador de connection strings como herramienta candidata del CLI), **F-16/F-48** (el "Repository" del Registro y el separador `0xA1` de su contrato de claves) y §6 ítem 12 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (esquema vigente, AAD y composición, que este Bolt **consume**), §3.2 (claves: activa y de lectura; nada de secretos en logs ni reportes), **§3.3** (migrar no re-cifrar: detección explícita, capacidad legacy acotada y guardada, **vencimiento con registro**, procedimiento por valor con validación y respaldo, y el límite del ASCII no reparable), **§3.4** (la **excepción única y acotada** con sus cinco condiciones y las primitivas prohibidas), **§3.5** (los comandos en el CLI, el contexto FIPS y la reproducibilidad del reporte), §3.6 (qué no cubre) y §5 (NFRs) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato de configuración y catálogo: de dónde se leen los valores) y §3.3 (prefijo de esquema obligatorio, prohibición de la heurística, error tipado, quién invoca al resolutor, y la eliminación del `Dictionary` estático de cadenas) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.2 (el CLI: comandos, reporte estable, códigos de salida y **el conversor de connection strings reservado como candidato de este CLI**) y §3.6 (una sola implementación de la capacidad, consumida por CLI y shell) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (slot y TFM del CLI), §3.4 (política de dependencias), §3.6 (canal de distribución, ajeno) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-018`](../functional/bolts/US-000.BOLT-018-cli-del-motor.md) — su SPEC canónica ([`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md)) entrega el CLI, su parser, su reporte y su **mapeo único de códigos de salida** | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — su SPEC canónica ([`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md)) entrega el **motor de protección**, la **clave activa**, el `kid` y la **provisión de claves** | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, lane de integración y build compartido | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Cooperator.Framework.Library/Cryptography.cs` — la **forma del formato viejo** que la detección explícita
debe reconocer y que el componente aislado debe poder leer: `Encrypt`/`Decrypt` con
`TripleDESCryptoServiceProvider` (bloque de 8 bytes) + `MD5CryptoServiceProvider` + `Encoding.ASCII` +
`des.IV = _Iv` (líneas 67-76 y 97-108), es decir: salida **Base64** de un bloque de 8 bytes y **pérdida de
información no ASCII**; `1.4/Cooperator.Framework.Data/BaseRule.cs:104-129` (la heurística de detección por
substrings y el `Dictionary` estático de cadenas que la 2.0 eliminó —ADR-004 §3.3— y que este Bolt **no**
reintroduce: la clasificación es por forma y explícita); y `1.4/Tools/ConnectionStringEncriptor/MainForm.cs` (el
uso de la superficie estática retirada por el Bolt 022, cuyo sucesor delgado es el Bolt 024).

**Coherencia del plan de integración:** el Bolt declara `test_integration: required` (§3.2.2) y este SPEC declara
`integration_test_plan.applicability: "applicable"` **con sus tests, su entorno y sus datos** (§8.2 y
frontmatter). **No hay desajuste** y por lo tanto no hay reporte bloqueante.

---

## 4. Scope

### In scope

- La **familia de comandos** del CLI —inspección, migración y rotación/re-protección— con su contrato (flags,
  entradas, salida y códigos), agregada al vocabulario del CLI existente sin redefinirlo.
- La **clasificación explícita** de cada valor de los almacenes indicados: vigente (`aesgcm:`), identidad
  explícita (`plain:`), **candidato** del formato viejo (por **forma**), **ilegible** y **no migrable por pérdida
  de información en origen** (no ASCII), con su **motivo** y **sin heurística**.
- El **reporte previo obligatorio** (plan sin escritura) y la **escritura sólo con opción explícita**.
- El **procedimiento por valor** completo: descifrado del candidato por el componente aislado → **validación del
  resultado** (parseo y, cuando aplique, smoke de conexión) → **re-protección** con el esquema vigente y la
  **clave activa** → **reemplazo sólo tras validar** y **con respaldo** del original → registro en el reporte.
- La **rotación** (re-protección bajo la clave activa) y el **re-targeteo por AAD** (cuando el valor cambia de
  clave de configuración), con el mismo esquema de validación y respaldo.
- El **componente interno de descifrado legacy**, **aislado**, bajo la **excepción acotada** con sus cinco
  condiciones (ámbito, bandera explícita, supresión **por miembro**, sin generalización, con retiro).
- La **detección del contexto criptográfico restringido** (FIPS) con **mensaje accionable**, y la constancia de
  que un valor que no se pudo evaluar **no** se reporta como "sin candidatos".
- La **contribución al registro de vencimiento** de la capacidad legacy: **dueño**, **condición de cierre** y
  **fecha de revisita** (el **cierre** es del Bolt 030).
- La **evidencia** del aislamiento (que el componente legacy **no** es alcanzable desde el runtime ni desde los
  paquetes desplegados), de la ausencia de supresiones a nivel de proyecto/solución, del reporte sin contenido y
  del comportamiento por defecto sin escritura.

### Out of scope

- El **motor de protección** (esquema vigente, clave activa, `kid`, AAD, provisión de claves): **Bolt 022**
  (ADR-007 §3.1/§3.2). Acá se **consume**.
- El **CLI**, su parser, su ayuda, su reporte estable y su **mapeo de códigos de salida**: **Bolt 018** (ADR-003
  §3.2). Acá se **agregan comandos** a ese vocabulario.
- El **retiro** de la capacidad de descifrado legacy y el **cierre** del registro de vencimiento: **Bolt 030**.
- La **corrección del defecto** de F-23 en ejecución (eliminación del cifrado legacy del runtime y de la clave
  embebida): su **BUG + Bolt**; su estándar es ADR-007 (este Bolt es la herramienta que opera, no el fix).
- La **reparación** de los valores cuya información se perdió en 1.4: se **marcan no migrables**; la re-protección
  parte del dato real con **intervención humana**.
- El **shell delgado del encriptador** de escritorio: **Bolt 024** (ADR-009 §3.6).
- El **catálogo de claves y de dónde se leen los valores**: **Bolt 011 / ADR-004** (se consume el contrato).
- El **formato del modelo** y el **corpus `.coop`**: **Bolts 002/004/029**. Este Bolt no toca el modelo.
- Un **esquema propio del host** (DPAPI, X.509, KMS): los aporta el host por el hook del Bolt 022.

---

## 5. Prerequisites and baseline

- **Bolt 018 y su SPEC canónica
  ([`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md), `prerequisites`):** el CLI (`2.0/src/Cooperator.Cli`,
  `net10.0`), su `ArgumentParser`, su ayuda como contrato, su **reporte estable** (`Reporting/JsonReportWriter`,
  `Reporting/ReportModel`) y su **mapeo único de códigos de salida** (`Commands/ExitCodes.cs`) — con la
  adjudicación ya aprobada (`2` = éxito con warnings, `3` = entrada inválida, `4` = sólo `verify`). Esta familia
  se agrega ahí. Si el CLI no está, **parar y reportar**.
- **Bolt 022 y su SPEC canónica
  ([`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md), `prerequisites`):** el **esquema vigente**
  (`aesgcm:`), la **clave activa** y las de lectura, el `kid`, el **AAD** (nombre completo de la clave de
  configuración) y la **provisión de claves** (entorno/hook) con sus errores tipados. Sin él no hay a qué migrar
  ni con qué re-proteger. Si no está, **parar y reportar**.
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés, el build compartido
  y la **lane de integración** (con SQL Server y con una instalación de prueba escribible) — el Bolt declara
  `test_integration: required` y su corrida reescribe **almacenes reales**.
- **Claves del entorno de prueba:** generadas para la corrida y provistas por **variables de entorno** (nunca
  comiteadas). La disponibilidad de las claves del entorno operativo es una **propiedad operativa** del cliente y
  su ausencia es **error tipado** del motor (Bolt 022), no algo que el comando supla con un valor por defecto.
- **Almacenes de prueba escribibles:** una **copia temporal** de los almacenes de configuración del entorno (un
  archivo y variables de entorno) que el ciclo de integración puede reescribir sin tocar nada real del proyecto.
  El **almacén de usuario** de la 2.0 es del Bolt 010 y el **"Repository" del Registro** de 1.4 es materia de
  ADR-001 §3.5/F-16: si durante la corrida aparece la necesidad de tocar ese almacén legacy, **se preserva
  exactamente el separador `0xA1` del contrato de claves** (F-48) y, si eso exigiera una API Windows-only que el
  layout retira, **parar y reportar** (§15).
- **Árbol `1.4/` presente y congelado** en la línea base de §3: es la referencia de la **forma** del formato viejo
  que la detección debe reconocer (Base64 + bloque de 8 bytes) y de lo que **no** se reintroduce (la heurística de
  substrings).
- **Build state de partida:** el CLI del Bolt 018 compila y su suite corre. Este Bolt **no** agrega dependencias de
  terceros (ADR-001 §3.4) y **no** cambia el TFM del CLI.
- **Decisión de ejecución registrada (no es supuesto):** la corrida de integración prepara **almacenes
  sintéticos** (un archivo y variables de entorno con un valor `plain:`, un candidato legacy, un ilegible y uno
  con pérdida ASCII) y una base de prueba para la validación; **ningún** valor de un cliente real entra al ciclo,
  y las claves de la corrida se generan y se descartan.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-023`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — La familia de comandos y su contrato

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 Los comandos, sobre el vocabulario existente

Se agregan al CLI los comandos de la familia (`cooperator secrets inspect`, `… migrate`, `… reprotect` —los
nombres que ADR-007 §3.5 fija como tentativos y que este SPEC **adopta**), con su contrato publicado en la ayuda
(el `HelpText` es contrato en este CLI, Bolt 018):

| Comando | Entradas | Comportamiento por defecto | Escritura |
|---|---|---|---|
| `secrets inspect` | los **almacenes/entradas** a examinar (ruta del archivo, prefijo de variables de entorno, claves de configuración) + `--report` | **plan sin escritura**: clasifica y reporta | **ninguna** |
| `secrets migrate` | ídem + `--apply` + (si el candidato es una cadena de conexión) `--validate <clave>` | **plan sin escritura** (equivalente a `inspect` + plan de migración) | **sólo** con `--apply` |
| `secrets reprotect` | ídem + `--apply` (+ `--to-key <clave-de-configuración>` para el re-targeteo) | **plan sin escritura** | **sólo** con `--apply` |

- **Códigos de salida**: se **mapean en el módulo único** de códigos del Bolt 018 —`0` todo migrado/re-protegido,
  `2` con warnings (por ejemplo valores no migrables o pendientes), `1` error de ejecución, `3` entrada inválida
  (almacén inexistente o flag desconocido)—. **No** se usa `4`: ese código significa "el mundo no coincide con el
  modelo" y es propio de `verify` (adjudicación del Bolt 018). Un valor **no migrable** es un **warning con
  motivo**, no un error de entrada.
- La **reproducibilidad** del reporte se hereda del CLI (mismo input ⇒ mismos bytes) y la **ausencia de escritura**
  por defecto es el comportamiento central de esta familia.

**Files created:**
- `2.0/src/Cooperator.Cli/Commands/SecretsInspectCommand.cs` — el comando de inspección (clasificación y plan)
- `2.0/src/Cooperator.Cli/Commands/SecretsMigrateCommand.cs` — el comando de migración (plan y aplicación)
- `2.0/src/Cooperator.Cli/Commands/SecretsReprotectCommand.cs` — el comando de rotación/re-targeteo
- `2.0/src/Cooperator.Cli/Commands/SecretsArguments.cs` — las entradas de la familia (almacenes, `--apply`, `--report`, `--validate`, `--to-key`)

**Files modified:**
- `2.0/src/Cooperator.Cli/CommandLine/ArgumentParser.cs` y `…/HelpText.cs` — alta de la familia y su contrato en la ayuda
- `2.0/src/Cooperator.Cli/Commands/ExitCodes.cs` — el mapeo de los desenlaces de esta familia (**el** módulo único)

---

### Phase B — La clasificación explícita del inventario

**Duration:** 3h total cycle — **Complexity:** High

#### B.1 Reglas de forma, publicadas, sin heurística

La clasificación es **explícita** y por **forma**, y cada categoría tiene su razón publicada (ADR-007 §3.3):

| Categoría | Regla |
|---|---|
| **Vigente** (`aesgcm:`) | Prefijo `aesgcm:` con `kid` y carga: es del esquema del framework — **no** se toca en `migrate` |
| **Identidad explícita** (`plain:`) | Prefijo `plain:` declarado: **no** se toca (decide el operador si quiere protegerlo) |
| **Candidato del formato viejo** | Prefijo **ausente** y carga **Base64 válido** con **longitud múltiplo del bloque del algoritmo legacy** (8 bytes) ⇒ candidato a migrar |
| **Ilegible** | Prefijo ausente y carga que **no** cumple la forma de candidato: se reporta ilegible, **no** se intenta descifrar |
| **No migrable por pérdida en origen** | El descifrado produce bytes con caracteres **no ASCII**: la pérdida ocurrió en 1.4 (`Encoding.ASCII`) — se marca **no migrable automáticamente**, con motivo, y **no** se reemplaza |
| **No evaluable** | Bajo **contexto criptográfico restringido** (FIPS) la primitiva legacy no corre: se **detecta y se reporta** con mensaje accionable — **nunca** "sin candidatos" |

- **Reporte previo obligatorio**: el comando **siempre** emite el plan (clasificación + qué haría) **antes** de
  cualquier escritura; sin `--apply`, **no** escribe (la corrida de integración lo verifica comparando los
  almacenes byte a byte).
- **Sin heurística de "adivinar"**: no hay detección por substrings, ni intento de descifrado "a ver si anda", ni
  interpretación de un valor que no declare su estado (ADR-004 §3.3: el motor de substrings está eliminado).
- **El reporte identifica por clave de configuración y `kid`**, nunca por contenido: ni el valor, ni la clave, ni
  el texto plano descifrado aparecen en el reporte, en los logs ni en un archivo intermedio.

**Files created:**
- `2.0/src/Cooperator.Cli/Secrets/ValueInventory.cs` — el inventario de entradas y su clasificación por forma
- `2.0/src/Cooperator.Cli/Secrets/LegacyValueShape.cs` — las reglas de forma del candidato del formato viejo (Base64 válido + múltiplo del bloque) y de la marca de no migrable por ASCII
- `2.0/src/Cooperator.Cli/Secrets/SecretsStoreReader.cs` — la lectura de los almacenes indicados (archivo de configuración y variables de entorno) a través del contrato de configuración

---

### Phase C — El componente legacy, aislado bajo su excepción

**Duration:** 3h total cycle — **Complexity:** High

#### C.1 Las cinco condiciones de la excepción, verificables

1. **Ámbito:** el descifrado del formato viejo vive en **un** componente **interno** del CLI
   (`Secrets/Legacy/LegacyValueDecryptor.cs`), **aislado del camino de runtime**: **ningún** paquete desplegado
   (`Core`, `Library`, `Data`, `Modeling`, el motor, el encriptor) lo referencia y el componente **no** se publica
   como API. La verificación es de **referencias de ensamblado** sobre el runtime y sobre los paquetes del folder
   feed (integración), no una convención de nombres.
2. **Bandera explícita:** su uso exige `--allow-legacy-decrypt`, cuya ayuda **reconoce por escrito** que la clave
   del formato 1.4 es **pública** y que la operación es de **migración**, no de producción. Sin la bandera, los
   candidatos se clasifican y se reportan **pero no se descifran**.
3. **Supresión en el ámbito más estrecho:** si el build emite advertencias de obsolescencia por esas primitivas,
   se silencian **por miembro** (con `#pragma warning disable` sobre el miembro y un comentario que cita
   ADR-007 §3.3/§3.4 y su condición de retiro) — **nunca** a nivel de proyecto o solución. La verificación es la
   **ausencia** de supresiones de proyecto/solución y la de supresiones residuales fuera de ese miembro.
4. **Sin generalización:** el componente **sólo descifra**; **no** cifra con el formato viejo y **no** se
   reutiliza desde otra pieza. La verificación incluye que no exista un camino de "proteger" con el formato viejo
   (ni en el CLI, ni en el encriptor, ni en un test que lo ofrezca como utilidad).
5. **Retiro:** el componente se elimina junto con el cierre del inventario (ADR-007 §3.3/§3.4) — **Bolt 030** — y
   su condición de retiro queda registrada (§E.1).

**Files created:**
- `2.0/src/Cooperator.Cli/Secrets/Legacy/LegacyValueDecryptor.cs` — el componente interno y aislado (descifrado del formato viejo; **sin** capacidad de cifrar; supresión por miembro si hiciera falta, con su cita)
- `2.0/src/Cooperator.Cli/Secrets/Legacy/LegacyCryptoContextProbe.cs` — la detección del **contexto criptográfico restringido** (FIPS) con su mensaje accionable

---

### Phase D — El procedimiento por valor: validar, respaldar, reemplazar

**Duration:** 4h total cycle — **Complexity:** High

#### D.1 Los seis pasos, en orden

Por cada candidato, y **sólo** con `--apply`: **(1)** clasificar (ya hecho en el plan); **(2)** descifrar con el
componente aislado y la bandera; **(3)** **validar el resultado** —parseo del valor y, cuando el valor sea una
cadena de conexión y se indique `--validate`, un **smoke test de conexión** contra la base—; **(4)** re-proteger
con el **esquema vigente y la clave activa** (el `kid` usado va al reporte; **nunca** se inventa una clave ni se
genera un valor por defecto: si falta configuración, falla con el error tipado del motor del Bolt 022);
**(5)** **reemplazar sólo tras la validación**, con **respaldo** del original (el respaldo es un artefacto del
operador, fuera del camino del framework, y su formato no expone el valor —una copia del almacén original o el
valor original en su formato **protegido/legado**, nunca el texto plano—); **(6)** registrar qué se migró, qué
quedó pendiente y por qué.

- **El texto plano nunca toca disco, log ni reporte**: el descifrado vive en memoria; el reporte nombra la **clave
  de configuración** y el `kid`. La verificación es una prueba que corre el flujo completo sobre almacenes
  temporales y **busca el texto plano en todo lo escrito** (almacenes, respaldos, reportes, logs): **0**
  apariciones.
- **Rotación y re-targeteo:** `reprotect` re-protege los valores `aesgcm:` con la **clave activa** (rotación) y,
  con `--to-key <clave-de-configuración>`, los **re-targetea** a otro slot —recordando que el **AAD** es el nombre
  de la clave de configuración (ADR-007 §3.1): mover un valor de slot **exige** pasar por acá, y el valor
  re-targeteado sólo resuelve bajo la clave nueva—, con el mismo esquema de validación y respaldo.
- **Límite declarado y no reparado:** los valores cuyo descifrado produce bytes **no ASCII** se marcan **no
  migrables** con su motivo; el reporte recuerda que la re-protección parte del **dato real** con intervención
  humana (no se adivina la reparación).

**Files created:**
- `2.0/src/Cooperator.Cli/Secrets/SecretsMigrationPlan.cs` — el plan (qué se migraría, qué no y por qué) y su ejecución con respaldo
- `2.0/src/Cooperator.Cli/Secrets/SecretsStoreWriter.cs` — la escritura de los almacenes indicados, con respaldo previo y reemplazo sólo tras validar
- `2.0/src/Cooperator.Cli/Secrets/SecretsReprotectPlan.cs` — la rotación y el re-targeteo por AAD
- `2.0/src/Cooperator.Cli/Reporting/SecretsReportModel.cs` — el reporte de la familia (por clave de configuración y `kid`, con clasificación, motivos y resultado), serializado con el escritor estable del CLI

---

### Phase E — El registro de vencimiento y la evidencia

**Duration:** 2h total cycle — **Complexity:** Low

#### E.1 El registro (que este Bolt contribuye y el Bolt 030 cierra)

El registro de vencimiento de la capacidad legacy se entrega con sus **tres datos** (ADR-007 §3.3): **dueño**
(el responsable del inventario de migración por cliente), **condición de cierre** (100 % de los valores
clasificados como candidatos legacy convertidos a `aesgcm:` **o** declarados no migrables con su motivo) y
**fecha de revisita** (concreta, no "cuando se pueda"). El registro **acompaña** al entregable (en la
documentación de la herramienta) y **el cierre y el retiro son del Bolt 030**. ADR-007 §3.3 aclara que exige el
**registro**, **no** un artefacto formal de riesgo: este Bolt no crea uno.

**Files created:**
- `2.0/src/Cooperator.Cli/Secrets/LegacyCapabilityRetirement.md` — el registro: dueño, condición de cierre, fecha de revisita y la cita de la excepción

#### E.2 La evidencia

- **Aislamiento**: la inspección de referencias demuestra que **ningún** ensamblado del runtime ni paquete del
  folder feed referencia el componente legacy, y que su uso exige la **bandera** que reconoce el carácter público
  de la clave vieja.
- **Supresión acotada**: 0 supresiones a nivel de proyecto o solución; la única (si existe) está **por miembro**,
  con su comentario y su cita.
- **Sin escritura por defecto**: la corrida sin `--apply` deja los almacenes **idénticos byte a byte**.
- **Validación y respaldo**: la evidencia del smoke de conexión previo al reemplazo y del respaldo del original.
- **Reporte sin contenido**: el reporte nombra clave de configuración y `kid`; la búsqueda del texto plano, la
  clave y material derivado en el reporte (y en todo lo escrito) da **0**.
- **Contexto restringido**: la corrida con el contexto criptográfico restringido y su mensaje accionable (más la
  constancia de qué valor no se pudo evaluar), y la verificación de que **no** se reporta "sin candidatos".
- **Registro**: el estado del registro de vencimiento que este Bolt contribuye (y que el Bolt 030 cierra).
- **Rupturas**: si la familia agrega o cambia superficie del CLI, la entrada correspondiente para las notas de
  migración (ADR-008 §3.1; documento del Bolt 028).

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: Los tres comandos existen con contrato y reporte

**Given** el CLI construido
**When** se invoca la ayuda y cada comando sobre almacenes de prueba
**Then** inspección, migración y rotación existen con su contrato (entradas, flags y códigos), su ayuda los
describe como contrato y cada corrida produce un **reporte estable** con la clasificación, los motivos y el
resultado

### AC-2: La detección es explícita y reporta antes de tocar

**Given** almacenes con un valor vigente, uno `plain:`, un candidato del formato viejo, uno ilegible y uno con
pérdida de información en origen
**When** se corre la inspección
**Then** cada valor queda en su categoría **con su motivo**, la regla de forma de cada categoría está publicada, y
**no** hay ningún camino que "adivine" el estado de un valor (sin detección por substrings ni descifrado
especulativo)

### AC-3: Por defecto no se escribe nada

**Given** los mismos almacenes y una corrida de migración **sin** la opción de escritura
**When** termina
**Then** los almacenes quedan **idénticos byte a byte** y el comando emitió el plan (qué migraría, qué no y por
qué) — la escritura exige la opción explícita

### AC-4: La re-protección usa la clave activa y reporta el `kid`

**Given** un candidato legible y `--apply`
**When** se migra
**Then** el valor resultante usa el **esquema vigente** con la **clave activa**, el reporte declara el **`kid`
usado** y el esquema de salida, y **no** se inventó ninguna clave ni se generó un valor por defecto (si falta
configuración, el comando falla con el error tipado del motor)

### AC-5: El reemplazo ocurre sólo tras validar y con respaldo

**Given** un candidato cuya validación **falla** (no parsea o el smoke de conexión falla)
**When** se corre con escritura
**Then** el original **no** se reemplaza, el valor queda **pendiente** con su motivo en el reporte y el respaldo
del original existe **And Given** un candidato que valida, el original se respalda y **luego** se reemplaza

### AC-6: El texto plano no sale a disco, log ni reporte

**Given** el flujo completo de migración sobre almacenes temporales
**When** se buscan el valor en claro, la clave y material derivado en todo lo escrito (almacenes, respaldos,
reportes y logs)
**Then** hay **0** apariciones: el reporte identifica por **clave de configuración** y `kid`

### AC-7: La capacidad legacy está aislada bajo sus cinco condiciones

**Given** el árbol 2.0 y los paquetes del release
**When** se inspeccionan las referencias y las supresiones
**Then** (1) **ningún** ensamblado del runtime ni paquete desplegado referencia el componente legacy; (2) su uso
exige la **bandera** que reconoce por escrito que la clave del formato viejo es pública; (3) **0** supresiones a
nivel de proyecto o solución (la única, si existe, está **por miembro** con su cita); (4) el componente **sólo**
descifra y no se reutiliza para cifrar; (5) su **retiro** está registrado y pertenece al Bolt 030

### AC-8: Los valores con pérdida en origen se marcan no migrables

**Given** un candidato cuyo descifrado produce bytes no ASCII
**When** se inspecciona y se migra
**Then** se clasifica **no migrable** con su motivo, **no** se reemplaza, y el reporte indica que la
re-protección parte del dato real con intervención humana (no se adivina la reparación)

### AC-9: El contexto criptográfico restringido se detecta y se reporta

**Given** un contexto criptográfico restringido (FIPS) donde la primitiva legacy no puede correr
**When** se corre la inspección o la migración
**Then** el comando **detecta** el contexto y **reporta con mensaje accionable** qué valor no pudo evaluar, y
**nunca** reporta "sin candidatos" ni falla en silencio

### AC-10: La rotación y el re-targeteo por AAD funcionan con el mismo esquema

**Given** un valor `aesgcm:` y `--apply` (con o sin `--to-key`)
**When** se corre `reprotect`
**Then** el valor queda re-protegido con la **clave activa** (rotación) o re-targeteado a la clave de
configuración indicada (re-targeteo) y resuelve **sólo** bajo esa clave; mover un valor de slot **sin** pasar por
el comando falla explícitamente (el AAD es el nombre de la clave de configuración)

### AC-11: El registro de vencimiento existe con sus tres datos

**Given** el entregable y su documentación
**When** se lee el registro de la capacidad legacy
**Then** declara **dueño**, **condición de cierre** (100 % de candidatos convertidos o declarados no migrables con
motivo) y **fecha de revisita** concreta, y queda claro que el **cierre y el retiro** son del Bolt 030

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — los comandos existen con su contrato y su reporte documentados | §6/A.1 | AC-1 · ayuda + corridas + `SecretsReportTests` |
| Bolt §2 — la detección es explícita (clasifica por forma y reporta antes de tocar) | §6/B.1 | AC-2 · `SecretsInspectTests` (integración) + unitarios de forma |
| Bolt §2 — el valor por defecto no escribe y la escritura exige la opción explícita | §6/A.1, §6/D.1 | AC-3 · `SecretsDefaultNoWriteTests` (comparación byte a byte) |
| Bolt §2 — la re-protección usa la clave activa y reporta `kid` y esquema, sin inventar claves | §6/D.1 | AC-4 · `SecretsMigrationTests` |
| Bolt §2 — el reemplazo sólo tras validar y con respaldo; el texto plano no toca disco/log/reporte | §6/D.1 | AC-5, AC-6 · `SecretsMigrationTests` + búsqueda del texto plano |
| Bolt §2 — la capacidad legacy aislada, con bandera, supresión por miembro y sin generalización | §6/C.1 | AC-7 · `LegacyIsolationTests` + inspección de supresiones |
| Bolt §2 — los no ASCII se detectan y se marcan no migrables | §6/D.1 | AC-8 · `SecretsInspectTests` + unitarios de forma |
| Bolt §2 — el contexto restringido se detecta y se reporta con mensaje accionable | §6/C.1 | AC-9 · `FipsContextTests` |
| Bolt §2 — rotación/re-protección y re-targeteo | §6/D.1 | AC-10 · `SecretsReprotectTests` |
| Bolt §2 — el registro de vencimiento (dueño, condición de cierre, fecha) | §6/E.1 | AC-11 · `LegacyCapabilityRetirement.md` + MEM |
| ADR-007 §3.3 — detección explícita, procedimiento con validación y respaldo, límite del ASCII, vencimiento con registro | §6/B.1, §6/D.1, §6/E.1 | AC-2, AC-5, AC-8, AC-11 |
| ADR-007 §3.4 — las cinco condiciones de la excepción acotada | §6/C.1 | AC-7 · `LegacyIsolationTests` |
| ADR-007 §3.5 — los comandos en el CLI, el contexto FIPS y la reproducibilidad del reporte | §6/A.1, §6/C.1 | AC-1, AC-9 · contratos + corrida FIPS |
| ADR-004 §3.3 — prefijo obligatorio, sin heurística, error tipado | §6/B.1 | AC-2 · unitarios de clasificación |
| ADR-003 §3.2 — el vocabulario y los códigos de salida del CLI | §6/A.1 | AC-1 · mapeo único de códigos + `CommandContract` de la familia |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Clasificación (unitarios sobre almacenes sintéticos):** cada categoría de §B.1 con su regla —`aesgcm:` válido
  y con `kid` desconocido; `plain:`; candidato (Base64 válido con longitud múltiplo del bloque); ilegible (no
  Base64, longitud que no es múltiplo, texto libre); ausencia de prefijo (que es **error tipado del contrato**,
  no candidato, ADR-004 §3.3); valor con caracteres no ASCII en claro; carga vacía; carga muy larga— y la
  verificación de que **no** hay camino de descifrado especulativo (un valor ilegible **no** se intenta).
- **Reporte:** el modelo del reporte y su serialización estable (dos corridas ⇒ bytes idénticos), con
  clasificación, motivos, `kid` y resultado; y la prueba de **no contenido**: el texto plano, la clave y material
  derivado **no** aparecen en el reporte ni en sus mensajes.
- **Mapeo de códigos:** un caso por desenlace (todo migrado `0`; con warnings —no migrables/pendientes— `2`;
  entrada inválida `3`; error de ejecución `1`), y la constancia de que **`4` no se usa** en esta familia.
- **Plan y ejecución (con almacenes temporales):** el plan sin escritura **no** modifica los archivos (comparación
  byte a byte) y la aplicación **sí**; la validación previa al reemplazo (un candidato que no parsea y uno cuyo
  smoke de conexión falla ⇒ **no** se reemplaza y queda pendiente); el **respaldo** del original existe antes del
  reemplazo y su contenido **no** es texto plano.
- **No fuga del texto plano:** corrida completa instrumentada sobre almacenes temporales y búsqueda del valor en
  claro (y de la clave) en **todo** lo escrito —almacenes, respaldos, reportes, logs, archivos temporales del
  proceso—: **0** apariciones.
- **Ciclo de rotación/re-targeteo (hermético, con el motor del Bolt 022):** un valor `aesgcm:` re-protegido con la
  clave activa resuelve; re-targeteado con `--to-key`, resuelve bajo la clave nueva y **falla** bajo la vieja
  (AAD); sin `--to-key`, el valor movido de slot falla al resolver.
- **Contexto criptográfico restringido:** el **probe** se prueba con un **seam** que reporta "restringido"
  (unitario) —el comportamiento del mensaje accionable y la ausencia de "sin candidatos"—; en la corrida de
  integración se ejecuta contra el **estado real** de la máquina y, si el contexto no está restringido, el MEM
  deja constancia de que ese camino se verificó por seam y no contra el estado del SO (ver §15 para el caso en que
  el aprobador quiera un entorno FIPS).
- **Aislamiento (invariantes):** inspección de referencias de los ensamblados del runtime y de los paquetes del
  folder feed: **0** referencias al componente legacy; **0** supresiones de advertencia a nivel de proyecto o
  solución; el componente legacy **no** expone una operación de cifrado; y la bandera del comando es la única vía
  de habilitarlo (sin bandera, los candidatos se clasifican y **no** se descifran).
- **Consumer-side contract tests (test doubles):** el contrato de configuración (Bolt 011) se ejercita con su
  implementación de entorno y con un doble para los casos de clave ausente y valor sin prefijo; el **motor de
  protección** (Bolt 022) se ejercita con su implementación real (es local) y con un doble para simular el fallo
  de clave activa (para verificar que el comando **no** inventa un valor por defecto).
- **Edge cases:** almacén inexistente y ruta sin permisos (entrada inválida `3`); almacén vacío; valor repetido en
  dos almacenes (dos entradas con la misma clave de configuración ⇒ decisión documentada y reportada); valor
  `aesgcm:` con `kid` que ya no está (warning del motor, no migración sorpresa); candidato que descifra a una
  cadena vacía; candidato que descifra a una cadena de conexión y `--validate` sin base disponible (pendiente con
  motivo, no reemplazo); re-ejecución de la migración sobre un almacén ya migrado (idempotencia: nada que hacer,
  `0`); respaldo ya existente de una corrida anterior (no se pisa el respaldo: se versiona o se reporta).
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1). El defecto de F-23 sigue pendiente en su BUG/Bolt y **este** Bolt es su
  herramienta de cumplimiento.
- **Evidencia que produce el V-Bounce:** la corrida de la clasificación completa, la prueba de no escritura por
  defecto, la de validación-before-replace con respaldo, la búsqueda de texto plano en todo lo escrito (0
  apariciones), el mapeo de códigos, la inspección de aislamiento y de supresiones, el probe de contexto
  restringido y el registro de vencimiento. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "applicable"` porque el Bolt declaró `test_integration: required`
> (§3.2.2). Los tests se **escriben, construyen y verifican estáticamente** en este V-Bounce y se **ejecutan** en
> el ciclo de test de integración del Bolt, **después** de `AITL-MEM-Approval`; el resultado queda como **hecho**
> en `integration_test_runs[]` del manifiesto.

- **Integration tests (real dependencies — almacenes reales + SQL Server):**
  - `SecretsInspectTests` — el cuadro de clasificación sobre almacenes **reales** del entorno de prueba (un
    archivo de configuración y variables de entorno) con un valor vigente, uno `plain:`, un candidato legible, uno
    ilegible y uno con pérdida ASCII, cada uno con su clasificación y su motivo.
  - `SecretsDefaultNoWriteTests` — la corrida sin la opción de escritura **no** modifica ningún almacén
    (comparación **byte a byte** antes y después).
  - `SecretsMigrationTests` — la migración real con escritura explícita: descifrado del candidato, **validación**
    (parseo y smoke de conexión contra la base del entorno), **respaldo**, reemplazo y reporte final con migrados,
    pendientes y `kid` usado; y el caso del candidato que **no** valida (no se reemplaza).
  - `SecretsReprotectTests` — rotación a la clave activa y **re-targeteo** por AAD contra el almacén real: el valor
    re-protegido resuelve bajo la clave/clave-de-configuración nueva y el movimiento de slot sin re-proteger falla
    explícitamente.
  - `LegacyIsolationTests` — el componente legacy **no** es alcanzable desde el runtime desplegado ni desde los
    paquetes portables del folder feed (inspección de referencias de los ensamblados construidos), y su uso exige
    la bandera que reconoce el carácter público de la clave vieja.
  - `FipsContextTests` — la corrida bajo el **estado criptográfico real** de la máquina: si está restringido, el
    mensaje accionable y la constancia de qué valor no se pudo evaluar; si **no** lo está, la constancia de que el
    camino se verificó por seam (§8.1) y el resultado queda registrado en el MEM.
  - `SecretsReportTests` — el reporte final identifica por clave de configuración y `kid` y **no** contiene ningún
    valor, clave ni material derivado (búsqueda sobre el reporte producido en el entorno real).
  - **Quién los escribió:** este V-Bounce, en la misma suite del CLI (`Cooperator.Cli.Integration.Tests`, con la
    marca de categoría de integración del arnés del Bolt 001).
- **Contract / E2E against the real providers:** el contrato que cruza el límite es el de los **almacenes de
  configuración** del entorno (archivo y variables de entorno) y el de la **base** contra la que se valida: ambos
  se ejercen de verdad en esta corrida (no hay dobles en el nivel de integración). El motor de protección es local
  (no es un proveedor externo).
- **Edge cases (los que sólo aparecen contra almacenes reales):** un almacén con formato/indentación propios que la
  escritura debe **preservar** (la escritura reescribe sólo el valor, no el archivo entero); finales de línea del
  archivo (`CRLF`/`LF`) preservados; un valor escrito en un archivo con permisos de sólo lectura (falla explícita y
  **sin** reemplazo parcial); re-ejecución sobre el almacén ya migrado (idempotencia, `0`); un valor cuya clave de
  configuración cambió de nombre entre corridas (pendiente con motivo).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.
- **Cierre del plan antes de la aceptación (§3.2.2):** una **corrida cubriente que pasa** sobre el commit del
  Bolt, registrada en `integration_test_runs[]`. Si el entorno no está disponible, el Bolt queda **bloqueado en su
  ciclo** (no pasa a `ready-for-acceptance`) y el MEM lo registra; **no** se declara el plan satisfecho sin
  corrida (y una exención por `waived` exigiría un ADR de excepción de gate, G21).

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* se declaran **`pass`** porque el plan de integración es
> `applicable` y su corrida es la que cierra el Bolt (§8.2).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); **0** supresiones de advertencia a nivel de proyecto o solución | pass |
| Unit / component tests | V-Bounce | 100 % verdes: clasificación, reporte, códigos, plan/ejecución, no fuga, rotación y probe de contexto | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato de configuración (Bolt 011) y el motor de protección (Bolt 022) se ejercitan con su implementación real y con dobles para clave ausente/valor sin prefijo/fallo de clave activa | pass |
| SAST | V-Bounce | 0 hallazgos altos; **0** generalización del componente legacy (no cifra, no se reutiliza); **0** heurística de detección | pass |
| Dependency scanning / licenses | V-Bounce | 0 dependencias de terceros nuevas (ADR-001 §3.4); las primitivas legacy **no** entran como dependencia (son de la plataforma, bajo la excepción) | pass |
| OWASP Top 10 — static review | V-Bounce | Secretos y criptografía: la capacidad legacy está **aislada** y no alcanzable desde el runtime; el texto plano no toca disco/log/reporte; la validación precede al reemplazo; el comando **no** descifra sin bandera ni "adivina" el estado de un valor | pass |
| Prompt-injection scan | V-Bounce | `n/a` — el comando procesa valores de configuración con un formato declarado; no interpreta contenido textual como instrucción | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** valores, claves o material derivado en el código, en los fixtures, en los reportes y en los respaldos; **0** supresiones residuales que tapen un uso de primitiva prohibida | pass |
| Hallucination lint | V-Bounce | Cada capacidad trazable a ADR-007 §3.3/§3.4/§3.5 y al Bolt 023; 0 pasos de migración inventados fuera del procedimiento; 0 capacidades de otro Bolt (retiro/cierre → 030; shell → 024) adelantadas | pass |
| IP / license provenance | V-Bounce | Código propio; 0 código de terceros; el componente legacy **no** copia la implementación del framework (la reimplementa aislada y sólo para descifrar) | pass |
| PII / DLP | V-Bounce | `confidential` (mirror del Bolt): la corrida usa **almacenes sintéticos** y claves **generadas para la corrida**; ningún valor de cliente real entra al ciclo ni a la evidencia; el reporte identifica por clave y `kid` | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes; los almacenes y claves se indican explícitamente por clave de configuración | n/a (razón) |
| Test-first evidence | V-Bounce | El **cuadro de clasificación** y las reglas de forma se escriben desde ADR-007 §3.3 **antes** de implementar la detección; la verificación de aislamiento y de supresiones se escribe antes de retirar la superficie estática del Bolt 022 | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de inspección del mismo almacén producen el **mismo reporte** byte a byte; las operaciones con escritura son idempotentes al repetirse sobre un almacén ya migrado | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | Corrida **cubriente que pasa** contra almacenes reales y SQL Server (inspección, no-escritura por defecto, migración con validación y respaldo, rotación/re-targeteo, aislamiento, contexto criptográfico, reporte sin contenido); registrada en `integration_test_runs[]` | pass |
| Contract / E2E against the real providers | Integration test | La misma corrida ejerce los contratos reales de los almacenes de configuración y de la base de validación | pass |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — no hay superficie HTTP ni servicio: es una herramienta de línea de comandos que corre en el entorno del operador | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | `n/a` — el alcance es una operación de migración sobre almacenes, sin objetivo de latencia; su costo es el del número de valores a procesar | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | `n/a` — la herramienta no agrega telemetría (§11) y, por política, **no** debe registrar valores ni material | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `confidential`** (espeja el Bolt). La materia son **valores
  protegidos** y **material de claves**, y —durante la migración— **texto plano descifrado en memoria**: es la
  categoría más sensible del ciclo.
- **El texto plano es material transitorio y no persistente:** vive en memoria durante la operación, **nunca** se
  escribe en disco, en logs, en el reporte ni en un archivo temporal; el respaldo del original conserva el valor
  en su formato **original (protegido/legado)**, nunca en claro. La verificación es explícita (búsqueda del valor
  en todo lo escrito).
- **La excepción acotada es el punto de seguridad de este Bolt** (ADR-007 §3.4): el descifrado del formato viejo
  —cuyas primitivas están **prohibidas en el resto del árbol** y cuya clave es **pública** (está en el binario de
  1.4)— vive **aislado**: componente interno, bandera explícita que reconoce esa publicidad, supresión **por
  miembro**, **sin generalización** (no cifra, no se reutiliza) y **con retiro** agendado (Bolt 030). Nada de eso
  es una convención: cada condición tiene su verificación (§C.1/§E.2/AC-7).
- **Sin heurística y sin degradación:** un valor que no declara su esquema es **error tipado** (ADR-004 §3.3) y un
  valor que no cumple la forma de candidato es **ilegible**, no "algo que se prueba a ver si descifra" — la
  detección especulativa es exactamente el patrón que la política eliminó.
- **Validación antes de reemplazar:** reemplazar sin validar es la forma de perder un valor. El comando valida el
  resultado (parseo y, cuando el valor es una cadena de conexión, smoke de conexión) y **sólo entonces** reemplaza,
  con respaldo previo.
- **Rotación con dos roles:** re-proteger con la clave **activa** y aceptar las de **lectura** es lo que permite
  rotar sin big-bang (ADR-007 §3.2); el **AAD** (nombre de la clave de configuración) hace que mover un valor de
  slot **exija** pasar por el comando, en lugar de "funcionar" con la credencial equivocada.
- **Contexto criptográfico restringido (FIPS):** las primitivas legacy **lanzan**; el comando **detecta y
  reporta** con mensaje accionable y **nunca** reporta "sin candidatos" cuando no pudo evaluar (ADR-007 §3.5). La
  política vigente no depende de FIPS (`AesGcm` es FIPS-compatible) y las primitivas legacy **no** están en el
  runtime.
- **Nada de secretos en la evidencia:** la corrida usa almacenes sintéticos y claves generadas; el reporte y el
  MEM identifican por clave de configuración y `kid`; ningún valor de cliente real entra al ciclo (control DLP
  explícito).
- **Almacén legacy del Registro:** si la corrida llegar a tocar el "Repository" del Registro (F-16), se
  **preserva exactamente el separador `0xA1`** del contrato de claves (F-48) y no se introduce una dependencia
  Windows-only en el CLI (que es cross-platform): si eso fuera necesario, **parar y reportar** (§15).

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: la entrega son **comandos de CLI**; no emiten métricas ni trazas a ningún
  colector y no se les agrega instrumentación. El **reporte** del comando es su salida observable, y por política
  **no** incluye valores ni material.
- **La observabilidad de este Bolt es su evidencia:** el cuadro de clasificación, la corrida sin escritura
  (comparación byte a byte), la corrida con escritura (validación + respaldo + reemplazo), la búsqueda del texto
  plano en todo lo escrito (0 apariciones), la inspección de aislamiento y de supresiones, la corrida del
  contexto criptográfico restringido y el registro de vencimiento.
- **Diagnóstico en runtime (parte del entregable):** cada valor queda con su **clasificación y su motivo**; los
  fallos de configuración son los **errores tipados** del motor (Bolt 022) y de la configuración (Bolt 011); y el
  contexto restringido tiene su **mensaje accionable**. Es lo que convierte "un valor que no se migró" en algo
  explicable sin abrir un depurador —y sin exponer el contenido—.
- La telemetría del consumidor (cuántos valores migró cada cliente, con qué `kid`) es del **reporte** del propio
  comando, que el operador archiva; el framework no la recolecta.

---

## 12. Migration, compatibility and rollback

- **Migration:** **este Bolt es la migración.** Su procedimiento por valor (clasificar → descifrar → validar →
  re-proteger → respaldar y reemplazar → registrar) es el camino soportado para los valores que quedaron en el
  formato de 1.4, con **detección explícita** y **sin heurística**, y con el **límite declarado** de los valores
  que ya perdieron información en origen (no ASCII).
- **Compatibility:** los comandos **se agregan** al vocabulario del CLI (Bolt 018) sin redefinirlo y **no** usan
  el código `4` (propio de `verify`); el reporte conserva el formato estable del CLI. Del lado de los valores, el
  resultado es el **esquema vigente** (`aesgcm:`) que el motor del Bolt 022 resuelve: los consumidores no cambian
  su camino de lectura (leen el valor por el contrato y lo resuelven con el resolutor).
- **Vencimiento con registro:** la capacidad legacy **no** es permanente: su registro (dueño, condición de cierre,
  fecha de revisita) es parte de la entrega y su **cierre/retiro** pertenece al **Bolt 030**. Mientras exista, el
  riesgo de que se generalice está acotado por las cinco condiciones, verificadas.
- **Rollback:** revertir el commit del V-Bounce. La corrida de migración es **reversible por su respaldo** (el
  original se conserva antes de reemplazar) y por la **copia de los almacenes** del entorno de prueba; no hay
  migración de datos aplicada por este Bolt fuera de los almacenes que el operador indica. Si un valor ya se
  re-protegió y hubiera que volver atrás, el respaldo es la vía, y el `kid` de origen está en el reporte (que es
  reproducible por diseño).
- **Lo que este Bolt no hace (y por lo tanto no promete):** no repara la pérdida de información de 1.4 (la marca
  y la intervención humana son la respuesta), no corrige el defecto en ejecución (su BUG/Bolt), no retira la
  capacidad legacy (Bolt 030) y no protege valores por su cuenta (el motor es del Bolt 022).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **La capacidad legacy se generaliza o se filtra al producto** (el riesgo más grave: quedaría una primitiva prohibida en un paquete desplegado) | 2 | 5 | Ámbito interno verificado por **inspección de referencias** (runtime y paquetes del feed), bandera explícita, **sin** operación de cifrado en el componente y prohibición de reutilizarlo; las cinco condiciones con su verificación (AC-7) |
| **Supresión de advertencias que se extiende** (un `#pragma` o `NoWarn` de proyecto que taparía usos prohibidos) | 3 | 4 | Supresión **por miembro** con comentario que cita la excepción; verificación de **0** supresiones de proyecto/solución y de supresiones residuales |
| **Heurística que "adivina" qué está cifrado** (reintroducir el patrón que la 2.0 eliminó) | 3 | 4 | Clasificación por **forma** publicada + reporte previo + **0** descifrado especulativo (un ilegible **no** se intenta); la heurística de substrings del 1.4 no se reintroduce |
| **Escritura sin decisión explícita** (el comando "arregla" almacenes por su cuenta) | 3 | 5 | **Por defecto no escribe**; la corrida sin la opción se verifica con comparación **byte a byte** de los almacenes |
| **Pérdida de un valor por reemplazo sin validar** | 2 | 5 | **Validación antes de reemplazar** (parseo + smoke de conexión cuando aplica) + **respaldo** + reemplazo sólo tras validar; el caso que no valida queda **pendiente** con su motivo |
| **Fuga del texto plano** (a un log, al reporte, a un temporal o al respaldo) | 3 | 5 | El descifrado vive en memoria y el respaldo conserva el formato protegido/legado; búsqueda explícita del valor en **todo** lo escrito (0 apariciones) + gate de secretos |
| **Claves ausentes o mal configuradas** en el entorno operativo y "valor por defecto" inventado | 3 | 4 | El comando **no** inventa claves ni valores: falla con el error tipado del motor (Bolt 022) y queda como entrada pendiente, nunca como valor nuevo |
| **Contexto criptográfico restringido interpretado como "sin candidatos"** | 3 | 4 | Detección del contexto + mensaje accionable + prueba de que un valor no evaluable **no** se reporta como inexistente; si el entorno no está restringido, el MEM registra que se verificó por seam |
| **Caracteres no ASCII re-protegidos "a ciegas"** (arrastrar el valor corrupto al formato nuevo) | 3 | 4 | Detección y marca **no migrable** automática, con motivo; el reporte recuerda que la re-protección parte del dato real |
| **El retiro de la capacidad legacy se posterga sin control** | 4 | 3 | El registro con **dueño, condición de cierre y fecha de revisita** es parte de la entrega; el cierre es del Bolt 030 y queda declarado como riesgo con dueño |
| **Escritura que daña el almacén del operador** (formato, indentación, finales de línea) | 3 | 4 | La escritura reescribe **sólo el valor** preservando formato y finales de línea; respaldo previo; almacén de sólo lectura ⇒ falla explícita sin reemplazo parcial |
| **Confusión de frontera con el Bolt 022** (implementar acá protección o provisión de claves) | 2 | 3 | §4 lo excluye: acá se **consume** el motor; la re-protección usa su clave activa y su esquema |
| **Confusión de frontera con el Bolt 030** (retirar la capacidad o cerrar el registro acá) | 2 | 3 | §6/E.1 y §15: el registro se **contribuye**; el cierre y el retiro son del 030 |
| **El comando se apoya en el "Repository" del Registro** (almacén legacy, Windows-only, con el contrato `0xA1`) | 2 | 3 | Si se toca ese almacén se preserva **exactamente** el separador `0xA1` (F-48) y el CLI sigue siendo cross-platform; si eso exigiera una API Windows-only, **parar y reportar** |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-007/ADR-004/ADR-003/ADR-001;
> acá se aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) Las reglas de forma de la clasificación se fijan acá y se publican** (Base64 válido + longitud múltiplo del bloque del algoritmo legacy ⇒ candidato; sin prefijo y sin esa forma ⇒ ilegible; descifrado con bytes no ASCII ⇒ no migrable) | ADR-007 §3.3 dice "clasifica candidatos por **forma** (Base64 válido + longitud múltiplo del bloque de 8 bytes del algoritmo legacy)" y "examina las entradas que se le indican": la regla exacta debe estar **escrita** para que el operador entienda por qué un valor es candidato o no, y para que la suite la verifique. Si el aprobador quiere otro criterio (p. ej. exigir además que la clave de configuración esté en una lista declarada), es una revisión de este SPEC |
| **(⚠ aprobador) El código de salida de "valor no migrable" es `2` (warning), no `3`** | El vocabulario del Bolt 018 distingue `3` = **entrada inválida** y `2` = **éxito con warnings**; un valor no migrable (por pérdida en origen o porque no valida) no es una entrada inválida: es un resultado del proceso que el operador debe ver. Mapearlo a `3` haría que un pipeline lo tratara como error de uso; mapearlo a `2` permite "migré 9 de 10, mirá el reporte" |
| **(⚠ aprobador) El probe de contexto criptográfico restringido se verifica por seam en el nivel hermético y contra el estado real de la máquina en integración, aceptando que ese estado puede no estar restringido** | Forzar FIPS en el entorno de CI es una decisión de infraestructura (y no siempre disponible); el comportamiento a verificar es "detecta y reporta accionable", que se prueba por seam, y en integración se registra el estado real. Si el aprobador exige un entorno FIPS de verdad, eso cambia el plan de integración (y probablemente su `environment`) |
| El respaldo del original conserva el valor en su formato **protegido/legado**, nunca en claro | Un respaldo en texto plano sería exactamente la fuga que el NFR *No embedded secrets* y la clasificación `confidential` prohíben; y el original ya está cifrado (aunque sea mal): copiarlo tal cual es suficiente para revertir |
| Los comandos adoptan los nombres que ADR-007 §3.5 fija como tentativos (`secrets inspect|migrate|reprotect`) | El ADR "fija los comandos" y los declara tentativos; adoptarlos tal cual evita inventar un segundo vocabulario y deja el CLI (Bolt 018) con su superficie ampliada sin redefinirla |
| La bandera de la excepción se llama `--allow-legacy-decrypt` y su ayuda **dice por escrito** el carácter público de la clave del formato viejo | Es la condición 2 de la excepción (ADR-007 §3.4) y su texto debe ser explícito para el operador: la bandera no es un detalle técnico, es el reconocimiento de que se está usando una capacidad exceptuada |
| El componente legacy **reimplementa** el descifrado en vez de reutilizar código del 1.4 | Su ámbito es el CLI (`net10`, cross-platform) y el 1.4 es `net48` congelado; y la implementación debe quedar **sólo** para descifrar, sin arrastrar la superficie estática que el Bolt 022 retira. Que sea propia y pequeña también hace verificable la condición "sin generalización" |
| El inventario cubre **archivo de configuración y variables de entorno** (y, si aparece, el almacén legacy con su contrato `0xA1`) | Son los almacenes de ADR-007 §3.3 que la 2.0 puede leer con su contrato de configuración; el "Repository" del Registro es de ADR-001 §3.5/F-16 y se trata con su separador, o se reporta si exigiera una API Windows-only |
| El reporte de la familia se construye sobre el **escritor estable del CLI** (no uno propio) | ADR-003 §3.3/§3.5 exige determinismo y un reporte parseable por agentes; tener dos escritores en el mismo CLI sería dos formatos que se desincronizan |
| La ejecución es **idempotente** sobre un almacén ya migrado (nada que hacer, `0`) | Un operador va a re-ejecutar el comando; que la segunda corrida no haga nada (y no reescriba ni re-respalde) es lo que hace seguro el uso repetido y verificable por la suite |

---

## 15. Stop conditions

- **El CLI (Bolt 018) o el motor de protección (Bolt 022) no están** en el árbol 2.0: parar y reportar. Sin el
  primero no hay dónde agregar los comandos; sin el segundo no hay esquema vigente ni clave activa.
- **El entorno exige que la capacidad legacy esté disponible desde el runtime o desde un paquete desplegado**
  (por ejemplo porque una aplicación del cliente quiere leer un valor viejo en producción): **parar**. Esa es la
  condición 1 de la excepción acotada (ADR-007 §3.4) y habilitarla exigiría un ADR que reemplace a ADR-007; la
  respuesta al caso real es **migrar el valor** con esta herramienta.
- **Se pide que el componente legacy cifre** (para "proteger con el formato viejo") o que se reutilice desde otra
  pieza: parar. La condición 4 lo prohíbe expresamente.
- **Se pide leer o escribir el formato viejo sin la bandera explícita** (o silenciar advertencias a nivel de
  proyecto/solución): parar. Las condiciones 2 y 3 son verificables y no negociables dentro de un Bolt.
- **Se pretende retirar la capacidad legacy o cerrar el registro de vencimiento acá**: parar y reportar; el cierre
  y el retiro son del **Bolt 030** (ADR-007 §3.3/§3.4).
- **Se pretende reparar los valores cuya información se perdió en 1.4** (adivinar el dato original) o
  **re-protegerlos** desde el valor corrupto: parar. Se marcan **no migrables** con su motivo y la re-protección
  parte del dato real con intervención humana.
- **Un valor no se puede validar y el operador pide reemplazarlo igual**: parar. El procedimiento exige
  validación antes del reemplazo (ADR-007 §3.3); sin ella, el valor queda **pendiente** con su motivo.
- **Tocar el almacén legacy del Registro exigiría una API Windows-only en el CLI** (que es `net10`
  cross-platform): parar y reportar. Si se toca, se preserva exactamente el separador `0xA1` (F-48) y el layout no
  cambia por conveniencia (ADR-001 §3.1).
- **El texto plano aparece escrito en algún lado** (disco, log, reporte, temporal, respaldo): parar. Es una fuga
  de la categoría `confidential` y el NFR de ADR-007 §5 la prohíbe; se corrige antes de cerrar el V-Bounce.
- **La corrida de integración no se puede ejecutar** (sin almacenes escribibles o sin base para la validación): el
  V-Bounce puede cerrar con los tests escritos y verificados estáticamente, pero el Bolt **no** pasa a
  `ready-for-acceptance`: queda en su ciclo de integración y el MEM registra el bloqueo. **No** se declara el plan
  satisfecho ni se pide un waiver sin ADR (G21).
- **Se pretende implementar acá el shell delgado del encriptador o el motor de protección**: parar; son de los
  Bolts 024 y 022 respectivamente (una sola implementación, consumida por ambos).
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado por
  `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG**
- [ ] Code follows applicable ADRs (ADR-007 §3.2/§3.3/§3.4/§3.5 y §5, ADR-004 §3.2/§3.3, ADR-003 §3.2/§3.6, ADR-001 §3.1/§3.4)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: **corrida cubriente que pasa** registrada en `integration_test_runs[]` del manifiesto (Bolt `test_integration: required`, plan `applicable`); sin corrida, el Bolt permanece en su ciclo de integración y no pasa a `ready-for-acceptance`

---

## 17. References

- [`US-000.BOLT-023`](../functional/bolts/US-000.BOLT-023-comandos-de-valores-protegidos.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-23 (clave e IV hardcodeados, MD5, ASCII: ofuscación como defecto de seguridad, con plan de migración necesario), F-40 (el encriptador como candidato del CLI), F-16/F-48 (el "Repository" del Registro y su separador `0xA1`) y §6 ítem 12.
- [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (esquema vigente y AAD, que se consumen), §3.2 (claves: activa y de lectura; nada de secretos en reportes), **§3.3** (migrar no re-cifrar: detección explícita, capacidad legacy aislada y guardada, vencimiento con registro, procedimiento con validación y respaldo, límite del ASCII, `reprotect`), **§3.4** (excepción única y acotada con sus cinco condiciones; primitivas prohibidas), **§3.5** (los comandos en el CLI, el contexto FIPS, la reproducibilidad y el encriptor como shell aditivo), §3.6 (qué no cubre), §5 (NFRs).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato de configuración y catálogo: de dónde se leen los valores) y §3.3 (prefijo obligatorio, sin heurística, error tipado, quién invoca al resolutor).
- [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.2 (el CLI: comandos, reporte estable, códigos de salida y el conversor de connection strings reservado como candidato de este CLI) y §3.6 (una sola implementación consumida por CLI y shell).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (slot y TFM del CLI), §3.4 (dependencias), §3.5 (features Windows-only y su retiro), §3.6 (canal de distribución, ajeno).
- [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) — el CLI, su parser, su reporte y su mapeo único de códigos (prerrequisito, Bolt 018).
- [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) — el motor de protección, la clave activa, el `kid` y la provisión de claves (prerrequisito, Bolt 022).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build y lane de integración (prerrequisito, Bolt 001).
- [`US-000.BOLT-030`](../functional/bolts/US-000.BOLT-030-retiro-descifrado-legacy.md) · [`US-000.BOLT-024`](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el retiro/cierre del registro, el shell delgado del encriptor y el documento de notas (fronteras declaradas).
- [`ADR-009`](../adrs/ADR-009-shell-del-modeler.md) — §3.6: el encriptor sobrevive como shell delgado y el comando del CLI es **aditivo**; ambos consumen la **misma** implementación (frontera del Bolt 024).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-023`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-11, contrato de los tres comandos sobre el vocabulario del CLI, reglas de forma de la clasificación **publicadas**, el componente legacy **aislado** con sus cinco condiciones verificables, el procedimiento por valor (validar → respaldar → reemplazar), la rotación/re-targeteo por AAD, la detección del contexto criptográfico restringido, el registro de vencimiento y la **estrategia de pruebas con el plan de integración `applicable`** declarado (tests, entorno y datos). Declara las fronteras con los Bolts 022 (motor), 030 (retiro/cierre), 024 (shell) y 018 (CLI). |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (**seguridad**, riesgo `high`) + el dueño del inventario de migración] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
