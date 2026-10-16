---
id: "SPEC-261016-0028"
title: "Retiro del gate por `PublicKeyToken` en `Data` y de su contraparte en el código generado: la firma queda como identidad"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-015" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-001", "ADR-008", "ADR-003"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, suite de caracterización y gate de compatibilidad de API con su línea base 1.4 (Bolt 001)
  - "SPEC-261016-0020-provider-de-datos.md" # la capa de datos ya sobre su forma final: provider único, interior tipado (Bolt 014)
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (debt): retiro de un control de ejecución y de su contraparte en el código generado; el resultado se verifica sobre el árbol compilado y los artefactos del set, sin base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:34:07-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:34:07-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:34:07-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-015 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent; su acknowledgment «Dep 019 en draft no bloquea READY»), ADR-001/ADR-008/ADR-003 accepted (tech_lead), REV-001 approved, OQ-003 answered; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → applicability: n/a con razón; inventario cerrado del gate (GetAssemblyToken, contador, CheckForToken con sus 3 salidas silenciosas, GetEntryAssemblyTokenString, GetPublicKeyTokenFromArray, las 13 invocaciones en BaseGateway) + limpieza asociada; superficie (firma = identidad, sin tocar .snk/naming/versionado; ruptura dicha con precisión)."
    - "Compliant — contraparte generada: verificación estática sobre el set vigente sin reescribirlo (frontera con el Bolt 019) + cita de que el flag del formato ya lo resolvió el Bolt 002; ruptura registrada (ADR-008 §3.1) + integridad de despliegue declarada pendiente con dueño y fecha (tech_lead, ADR-001 §3.6) + verificación de ausencia de control sustituto (0 comparaciones de PublicKeyToken en runtime)."
    - "ADJUDICACIÓN (orden 019→015) — ALCANZA con la condición de parada: el Bolt 015 ya declara BOLT-019 en `dependencies` (grafo completo, a diferencia del SPEC-005 donde el Bolt omitía la dependencia); el SPEC no inventa el id de SPEC-019 (aún no está en disco) y lo trata como prerrequisito con condición de parada (§5/§15: si el set vigente todavía emite el token → parar y ordenar la ejecución del Bolt 019 antes). Apruebo la secuencia 019→015. FINDING menor: cuando SPEC-019 se redacte, agregar su id a `prerequisites` (corrección de frontmatter, no material)."
    - "Compliant — la precisión del retiro: InsufficientSecurityRightsException se conserva (chequeos legítimos de derechos de seguridad de BaseGateway/BaseLoader con mensaje propio); sólo cae el camino del gate, con la distinción verificada por búsqueda de mensaje + prueba de no regresión (AC-2)."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-015`. El set de plantillas que
     deja de emitir el token es del **Bolt 019** (acá se **verifica su ausencia** y se registra la frontera); el
     proveedor y el interior de `Data` son del Bolt 014; la política criptográfica de valores es del ADR-007
     (Bolts 022/023) y **no** se toca: un token de ensamblado no es una clave de protección.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ PRECISIÓN QUE GOBIERNA EL RETIRO (§14): `InsufficientSecurityRightsException` **se queda** —la usan los
     chequeos legítimos de derechos de seguridad de `BaseLoader`/`BaseGateway`—; lo que desaparece es el camino
     de comparación de tokens y su throw particular ("This application is not authorized to use this library").
-->

# SPEC-261016-0028 — Retiro del gate por `PublicKeyToken` en `Data` y de su contraparte en el código generado: la firma queda como identidad

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 11) |
| **Bolt** | [US-000.BOLT-015](../functional/bolts/US-000.BOLT-015-retiro-del-gate-token.md) |
| **ADRs** | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-003](../adrs/ADR-003-motor-scriban-headless-y-cli.md) |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se retira de Cooperator 2.0 **todo control de integridad de despliegue en tiempo de ejecución**: desaparece el
**gate por `PublicKeyToken` del ensamblado de entrada** de la capa de datos —sus miembros, su contador, sus
caminos de salida silenciosa y sus invocaciones— y se verifica que su **contraparte en el código generado**
(el token embebido y el `override` que el set de plantillas emitía) ya **no** está presente en la salida del set
vigente. La **firma de los ensamblados se mantiene exactamente igual**: es **identidad**, no un control en
runtime (ADR-001 §3.3). **No se introduce ningún control sustituto en código**: la integridad de despliegue, si
alguna vez importa, pertenece a la capa de empaquetado y su canal sigue siendo una decisión del `tech_lead` con
dueño y fecha (ADR-001 §3.6), ajena a este Bolt.

El problema que resuelve está documentado (REV-001 F-10 y §6 ítem 11; `OQ-003`, **answered** el 2026-10-15,
opción A): el gate comparaba el `PublicKeyToken` del **ensamblado de entrada** contra un token embebido en el
código generado y lanzaba `InsufficientSecurityRightsException("This application is not authorized to use this
library")` — con tres comportamientos que lo volvían **inefectivo y errático**: un **contador estático** que
cortaba el chequeo tras 200 llamadas, un **salto silencioso cuando había `HttpContext`** (web) y un **salto
silencioso cuando `Assembly.GetEntryAssembly()` era `null`** (IIS, *test runners*, hosts con carga dinámica,
single-file). Su origen declarado por el dueño fue "por las dudas me cambiaran una DLL": era**detección de
manipulación, no licenciamiento comercial**, y no hay clientes licenciados por este mecanismo.

Si no se implementa: la 2.0 conserva un control que no puede cumplir lo que promete (falsos positivos y falsos
negativos), arrastra superficie muerta en la capa de datos y en los artefactos que genera, y deja una ruptura de
superficie sin declarar.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` F-10 ubicó el mecanismo con evidencia de archivo y línea:
`Data/BaseGateway.cs:94-137` (`protected virtual byte[] GetAssemblyToken()`, `_checkForTokenCounter`,
`CheckForToken()`) y `:139-155` (`_entryAssemblyTokenString`, `GetEntryAssemblyTokenString()`), más la
contraparte generada en `Tools/CooperatorModeler/Templates/CSharpClasses/Gateway.Auto.cs:406-407` y
`Mapper.Auto.cs:444-445` (`<% if (parameters["GenerateCheckForToken"] == "TRUE") { %>` → `protected override
byte[] GetAssemblyToken()`), alimentada por el flag `Snapshot.GenerateCheckForToken` del modelo. El §6 ítem 11
ruteó el retiro al ADR de empaquetado y `OQ-003` lo decidió (opción A).

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-001 §3.3** — **el gate por `PublicKeyToken` se retira** (F-10/OQ-003 A): se eliminan `CheckForToken`,
  `GetAssemblyToken`, `GetEntryAssemblyTokenString` **y su contraparte embebida en el código generado**; **no se
  reemplaza por otro gate en código** (el mecanismo era detección de manipulación, no licenciamiento, y en hosts
  modernos se salteaba o daba falsos positivos); la **firma se mantiene como identidad** en todos los ensamblados
  de la 2.0 (mismo `snk`; `Modeling` firma como el resto), **sin** cambios de naming ni de versionado; y la
  integridad de despliegue se cubre en la **capa de empaquetado** (feed controlado, Authenticode cuando se defina
  el canal), **nunca** en el runtime del framework.
- **ADR-001 §3.6** — el **canal de distribución** (y con él la verificación de integridad del despliegue) es una
  decisión **pendiente con dueño y fecha**: la define el `tech_lead` **antes del release 2.0.0**. No se reabre
  acá y **no** es una dependencia de este Bolt.
- **ADR-008 §3.1** — **toda ruptura se documenta** en las notas de migración con tres datos obligatorios: qué
  miembro o atributo desaparece, **por qué** (la decisión que lo ordena) y **con qué se reemplaza** en el código
  del consumidor (o "sin reemplazo, porque el escenario no existe en el runtime objetivo"); y **ADR-008 §5** — el
  **gate de compatibilidad de API** contra la línea base 1.4 sigue verde **con la ruptura declarada**: una
  diferencia no listada **falla**.
- **ADR-003 §3.1/§3.4** — el **set de artefactos** que el motor genera y la **regla de propiedad** del código
  generado son del motor y de su set: es el **Bolt de plantillas (019)** quien decide qué se emite. Este Bolt
  **consume** esa salida y **verifica la ausencia** del gate.
- **`OQ-003` (answered, opción A)** — el retiro **no deja el binario sin identidad** (la firma se mantiene) y la
  integridad de despliegue se cubre en la capa de empaquetado, no en código del framework.

**Qué decide este SPEC, dentro de esas restricciones.** El **inventario exacto** de lo que se retira en la capa
de datos (miembros, contador, caminos de salida y **todas** las invocaciones), la **precisión** de que el tipo de
excepción de derechos **se conserva** por el camino legítimo de `SecurityRights`, la **forma de la verificación**
sobre la contraparte generada (estática sobre el set vigente y sobre los artefactos disponibles), la **entrada de
la ruptura** en el registro de rupturas y las **notas de migración** (documento del Bolt 028), y la **constancia**
de que no aparece ningún control sustituto.

**Frontera y alcance cruzado.** Este Bolt **no** cambia la firma, el naming ni el versionado (ADR-001 §3.3),
**no** diseña el canal de distribución ni su integridad (ADR-001 §3.6, decisión del `tech_lead`), **no** toca la
política criptográfica ni la protección de valores (ADR-007; Bolts 022/023: un token de ensamblado **no** es una
clave), **no** reescribe el set de plantillas (**Bolt 019**: lo verifica), **no** ejecuta el batch del corpus de
modelos (**Bolt 029**) y **no** migra los artefactos que un consumidor haya generado con el set viejo (los
regenera con el set vigente, según las notas).

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-015`](../functional/bolts/US-000.BOLT-015-retiro-del-gate-token.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — **F-10** (el gate por `PublicKeyToken`: ubicación, comportamiento en hosts modernos y decisión de retiro) y **§6 ítem 11** | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| Open question (decisión de negocio) | [`OQ-003`](../analysis/open-questions/OQ-003-gate-de-licenciamiento.md) — `status: answered` (2026-10-15), opción A: **retirar** el gate; la firma se mantiene como identidad | `answered` ✓ — sin ciclo `AITL-*-Approval` (no es fuente con checkpoint propio); decisión del dueño registrada |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.3 (el gate se retira y **no** se reemplaza por otro gate en código; la firma como identidad; limpieza obligatoria de proyecto), §3.6 (el canal de distribución y la integridad de despliegue como decisión pendiente con dueño y fecha), §3.1 (layout de `Data`) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (rupturas documentadas con reemplazo), §3.5 (registro de rupturas verificado por gate), §5 (gate de compatibilidad de API contra la línea base 1.4) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.1 (el set de artefactos que el motor genera) y §3.4 (regla de propiedad del código generado: quién decide qué se emite) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-014`](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — su SPEC canónica ([`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md)) deja la capa de datos sobre su forma final | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — arnés, suite de caracterización y **gate de compatibilidad de API** | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Cooperator.Framework.Data/BaseGateway.cs` — `protected virtual byte[] GetAssemblyToken()` (94-97, con el
cuerpo `return null;` que hace que un gateway no generado no pida nada), `private static int
_checkForTokenCounter` y `private void CheckForToken()` (100-137: corte por contador `> 200`, salto silencioso
con `HttpContext.Current != null`, salto silencioso con `Assembly.GetEntryAssembly() == null`, y el
`throw new Exceptions.InsufficientSecurityRightsException("This application is not authorized to use this
library")`), `private static string _entryAssemblyTokenString` y `GetEntryAssemblyTokenString()` (139-151),
`GetPublicKeyTokenFromArray` (154-161 y siguientes) y las **trece invocaciones** de `CheckForToken()` (líneas
238, 483, 686, 730, 786, 839, 893, 959, 1037, 1080, 1150, 1191, 1275); `1.4/Tools/CooperatorModeler/Templates/
CSharpClasses/Gateway.Auto.cs:406-407` y `Mapper.Auto.cs:444-445` (la emisión condicionada por
`GenerateCheckForToken`); `1.4/Tools/CooperatorModeler/Snapshot.cs` (la propiedad `GenerateCheckForToken` del
modelo); y `1.4/Cooperator.Framework.Data/BaseLoader.cs` + `BaseGateway.cs` para la **distinción crítica**: los
`throw new InsufficientSecurityRightsException("Your's security rihgts are not good enough for this
operation")` (p. ej. `BaseGateway.cs:242/487/…`, `BaseLoader.cs:106/309/…`) son **chequeos legítimos de derechos
de seguridad** y **no** forman parte del gate.

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- El **retiro en la capa de datos** de todo el gate: `GetAssemblyToken()` (miembro protegido virtual), el
  contador estático `_checkForTokenCounter`, `CheckForToken()`, `_entryAssemblyTokenString`,
  `GetEntryAssemblyTokenString()`, el helper `GetPublicKeyTokenFromArray` **si sólo lo usa el gate**, y **todas**
  las invocaciones de `CheckForToken()` desde los gateways.
- La **verificación de la contraparte generada**: que la salida del set de plantillas **vigente** (Bolt 019) no
  contenga el `override` de `GetAssemblyToken`, ni el chequeo, ni un token embebido, ni la condición del flag; y
  la **constancia de la frontera** con ese Bolt (dueño del set).
- La **constancia de la firma como identidad**: ningún cambio de material de firma, de naming ni de versionado
  (ADR-001 §3.3).
- La **constancia de la ausencia de un control sustituto**: nada compara tokens, nada bloquea por token, nada
  implementa un chequeo equivalente (ni un control de licenciamiento, ni una verificación de Authenticode) en el
  runtime del framework.
- El **registro de la ruptura** con qué desaparece, por qué y con qué se reemplaza (o sin reemplazo), como
  insumo de las **notas de migración** (documento del Bolt 028, ADR-008 §3.1) y del **gate de compatibilidad de
  API**.
- La **declaración del NFR de integridad de despliegue** como pendiente de la capa de empaquetado, con su dueño
  y su fecha (ADR-001 §3.6), sin reabrirlo acá.
- El **retiro de la limpieza obligatoria** que ADR-001 §3.3 asocia a este mecanismo en la capa de datos (restos
  de la integración del gate, si quedaran).

### Out of scope

- El **set de plantillas** (qué emite y cómo lo decide): **Bolt 019**, dueño del set (ADR-003 §3.4). Este Bolt
  **verifica la ausencia** en su salida y **no** lo reescribe.
- La **firma, el naming y el versionado** de los ensamblados: ADR-001 §3.3 (sin cambios; el `.snk` y la política
  de firma quedan como están).
- El **canal de distribución** y la **integridad de despliegue** (feed, Authenticode, firmado de paquetes): la
  decisión es del `tech_lead` con dueño y fecha (ADR-001 §3.6). Este Bolt sólo deja constancia de que sigue
  pendiente y que **no** vive en el runtime del framework.
- La **política criptográfica** y la **protección de valores** (la cadena de conexión, los valores protegidos):
  **ADR-007 / Bolts 022-023**. Un token de ensamblado **no** es una clave de protección ni un valor protegido.
- El **flag del gate en el modelo** (`GenerateCheckForToken`) y su ausencia del formato: **ya resuelto por el
  Bolt 002** — el `.coop2` lo excluye y el migrador lo reporta con una nota informativa (ADR-002 §3.1/§3.3,
  `SPEC-261016-0002` §6/C.1). Este SPEC no toca el formato ni el migrador.
- El **batch del corpus** y los `.coop` de 1.4: **Bolt 029**.
- La **regeneración de los artefactos** que un consumidor tenga generados con el set viejo: es trabajo de
  migración del consumidor, guiado por las notas.
- El **proveedor de datos, el interior tipado y el borde de conversión**: **Bolt 014**.
- La **caché de la capa de datos**: **Bolt 013**.

---

## 5. Prerequisites and baseline

- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md), `prerequisites`):** la suite de
  caracterización (verde antes y después), la configuración de build compartida y el **gate de compatibilidad de
  API** con su línea base 1.4 — que es la evidencia de que el retiro está declarado como ruptura y no aparece
  como sorpresa (ADR-008 §5).
- **Bolt 014 y su SPEC canónica
  ([`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md), `prerequisites`):** el retiro se hace sobre la
  **forma final** de la capa de datos (provider único, interior tipado, sin casts), no sobre el código que ese
  Bolt todavía está moviendo.
- **El set de plantillas vigente (Bolt 019) — prerrequisito de la verificación de la contraparte generada.** Su
  SPEC canónica **no está en disco al redactar esta revisión** (el set se redacta en una ola posterior, según el
  propio `acknowledgment_reason` del `AITL-BOLT-READY-Approval` del Bolt: "Dep 019 en draft (lote 4) no bloquea
  READY: relación por id, resuelta antes de ejecutar"), así que no se enumera en `prerequisites` (no se inventa
  su id): se declara acá y en §15 como **prerrequisito con condición de parada**. El prerrequisito es su
  **ejecución**: si el set vigente todavía emite el token y su chequeo, la evidencia del lado generado **no se
  puede producir** (el Bolt quedaría verificando algo que ya no es cierto) → **parar y reportar** y ordenar la
  ejecución del Bolt 019 antes.
- **El proyecto `Cooperator.Framework.Data` del árbol 2.0** con sus gateways y loaders en su forma final. Si no
  existe, **parar y reportar**.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario completo (miembros,
  invocaciones y contraparte generada).
- **Build state de partida:** el árbol 2.0 compila; la suite de caracterización y el gate de API están verdes.
  Este Bolt **no** agrega dependencias, no cambia TFMs y no toca el material de firma.
- **Material de firma presente y sin cambios:** el `CooperatorFramework.snk` y la política de firma del árbol 2.0
  (ADR-001 §3.3). Este Bolt **no** lo modifica; su constancia es que sigue igual.
- **Decisión de ejecución registrada (no es supuesto):** nada de este Bolt se verifica contra una base de datos,
  un servicio ni un entorno: es un retiro de código y una verificación de ausencia sobre el árbol y sobre la
  salida del set. De ahí el plan de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-015`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — El inventario y el retiro del gate en la capa de datos

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 Lo que desaparece (inventario cerrado)

| Pieza | Ubicación 1.4 | Acción |
|---|---|---|
| `GetAssemblyToken()` (`protected virtual byte[]`, cuerpo `return null;`) | `BaseGateway.cs:94-97` | **Se retira** el miembro virtual completo |
| `_checkForTokenCounter` (`private static int`) | `BaseGateway.cs:100` | **Se retira** (el corte por 200 llamadas desaparece con el gate) |
| `CheckForToken()` (`private void`) | `BaseGateway.cs:101-137` | **Se retira** completo, incluidos sus tres caminos de salida (contador, `HttpContext`, `Assembly.GetEntryAssembly() == null`) y su `throw` particular |
| `_entryAssemblyTokenString` (`private static string`) y `GetEntryAssemblyTokenString()` | `BaseGateway.cs:139-151` | **Se retiran** |
| `GetPublicKeyTokenFromArray(byte[])` | `BaseGateway.cs:154` y siguientes | **Se retira** si —y sólo si— su único uso era el gate (se verifica; si tuviera otro uso legítimo, se **reporta** antes de tocarlo) |
| Las **trece** invocaciones de `CheckForToken()` de los gateways | `BaseGateway.cs:238, 483, 686, 730, 786, 839, 893, 959, 1037, 1080, 1150, 1191, 1275` | **Se retiran** todas (inventario verificado por búsqueda; ninguna puede sobrevivir) |
| El `throw new InsufficientSecurityRightsException("This application is not authorized to use this library")` del gate | `BaseGateway.cs:135` | **Se retira** (su causa desaparece) |

**Lo que NO se toca (precisión que gobierna el retiro):** `InsufficientSecurityRightsException` **se conserva**
como tipo —los **chequeos legítimos de derechos de seguridad** de `BaseGateway` y de `BaseLoader` lo siguen
lanzando con su mensaje propio ("Your's security rihgts are not good enough for this operation")— y ninguna de
esas comprobaciones forma parte del gate. Confundir ambos usos retiraría una comprobación de permisos legítima:
es la trampa de este Bolt y se declara explícitamente (§14).

**Files modified:**
- `2.0/src/Cooperator.Framework.Data/BaseGateway.cs` — retiro del gate (miembros, contador, helpers y las trece invocaciones), conservando los chequeos de derechos
- `2.0/src/Cooperator.Framework.Data/BaseLoader.cs` — sólo si albergara alguna pieza del gate (verificación; los `InsufficientSecurityRightsException` que lanza son del camino de derechos y **no** se tocan)

#### A.2 La limpieza asociada

Se retiran, si quedaran en la capa de datos, los restos de la integración del gate (referencias, `using` o
miembros auxiliares que sólo existieran para él). ADR-001 §3.3 asocia este retiro a la **limpieza obligatoria de
proyecto** de la 2.0: nada del mecanismo retirado sobrevive como código muerto.

---

### Phase B — La superficie: la firma queda como identidad

**Duration:** 2h total cycle — **Complexity:** Low

#### B.1 La ruptura, dicha con precisión

- `GetAssemblyToken()` era un miembro **`protected virtual`** de `BaseGateway`: su retiro es una **ruptura de
  superficie** para un consumidor que lo hubiera sobrescrito (el `override` que el set viejo **emitía** en
  `Gateway.Auto.cs`/`Mapper.Auto.cs` de sus productos). Se registra con los tres datos obligatorios de
  ADR-008 §3.1: **qué** desaparece (el miembro virtual y el token embebido en los artefactos generados),
  **por qué** (ADR-001 §3.3: era detección de manipulación, no licenciamiento; y en hosts modernos se salteaba o
  daba falsos positivos —F-10/OQ-003—) y **con qué se reemplaza**: en el código del consumidor, **borrar el
  `override`** y **regenerar** con el set vigente; sin reemplazo funcional, porque el escenario (comparar el
  token del ensamblado de entrada) **no existe** como control del producto.
- La **firma se mantiene como identidad**: **no** se cambia el `.snk`, ni la política de firma, ni el nombre, ni
  el versionado (ADR-001 §3.3). La firma sigue estando y sigue significando lo mismo para el runtime (identidad
  del ensamblado); lo que desaparece es **la comparación de tokens hecha por el producto**.
- **No hay control sustituto en código:** ni licenciamiento, ni verificación de Authenticode, ni "chequeo de
  integridad" en el runtime del framework. Si alguien lo pidiera, su lugar **no** es el runtime: exigiría un ADR
  que reemplace a ADR-001 (§15).

**Files modified:** los de §A.1 (la documentación pública del paquete deja de mencionar el miembro retirado).

---

### Phase C — La contraparte generada (frontera con el Bolt 019)

**Duration:** 2h total cycle — **Complexity:** Medium

#### C.1 Verificar la ausencia, no reescribir el set

La contraparte del gate vive en el **set de plantillas** (dueño: **Bolt 019**, ADR-003 §3.4). Este Bolt **no**
lo reescribe: **verifica** que la salida del set **vigente** ya no la emite:

- **Sobre las plantillas del set vigente** (verificación estática, disponible siempre): no existe la emisión del
  `override` de `GetAssemblyToken`, no existe la condición del flag del gate, y no hay ningún token embebido ni
  literal de token.
- **Sobre los artefactos generados disponibles** (corroboración): los artefactos generados que existan en el
  árbol o en los fixtures del arnés no contienen el `override`, ni el chequeo, ni un token. Si el motor
  (Bolts 017/018) está disponible, se agrega una corrida de generación sobre el modelo de referencia del
  arnés y se inspecciona su salida; si no lo está, **la verificación estática sobre el set es la que manda** y
  el MEM lo registra así.
- **Sobre el modelo y el formato** (ya resuelto, se cita y no se repite): el flag del gate (`GenerateCheckForToken`)
  **no** forma parte del `.coop2` y el migrador lo descarta con una nota informativa — `SPEC-261016-0002` §6/C.1,
  ADR-002 §3.1/§3.3.

**Consumado en el set vigente del Bolt 019**: la frase del Bolt se cumple **cuando el set vigente es el que no lo
emite**. Si al iniciar el V-Bounce el set todavía lo emite (Bolt 019 no ejecutado), **parar y reportar** (§15).

**Files created:**
- `2.0/tests/Cooperator.Framework.Data.Tests/GateRetirementEvidence.md` — la evidencia del retiro: inventario de lo retirado, salida de las búsquedas (0 ocurrencias de los miembros y del token), verificación sobre las plantillas del set vigente, y la lista de rupturas para las notas

---

### Phase D — La ruptura registrada y la ausencia de sustituto

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 El registro de la ruptura

La ruptura entra en el **registro de rupturas** de la 2.0 (ADR-008 §3.1/§3.5) con su reemplazo, y de ahí a las
**notas de migración** (documento del **Bolt 028**). El **gate de compatibilidad de API** del Bolt 001 compara la
superficie contra la línea base 1.4: **cada** diferencia debe estar cubierta por esa entrada — una diferencia no
listada **falla** (ADR-008 §5).

#### D.2 La integridad de despliegue: declarada, no reabierta

Se deja **constancia explícita** de que la integridad de despliegue **no** queda cubierta en código y que su
cobertura pertenece a la **capa de empaquetado**, cuya decisión (canal, feed, Authenticode) sigue **pendiente con
dueño y fecha**: el `tech_lead` (`human:eugenioserrano`) antes del release **2.0.0** (ADR-001 §3.6). Esta
constancia es parte de la evidencia (que "no hay gate" no se lea como "no hay integridad"), y **no** habilita
ningún control sustituto.

#### D.3 Sin sustituto en código

Verificación de que en el árbol 2.0 **no** aparece ningún control que ocupe el lugar del gate: 0 comparaciones de
`PublicKeyToken` en runtime, 0 lecturas del ensamblado de entrada con fines de autorización, 0 rutas que
bloqueen por identidad de ensamblado.

---

### Phase E — Evidencia

**Duration:** 1h total cycle — **Complexity:** Low

- **Superficie**: la superficie publicada de la capa de datos **sin** los miembros del gate, y la constancia de
  que **ningún** consumidor interno del árbol los invocaba (búsqueda completa; el inventario de las trece
  invocaciones es la referencia).
- **Contraparte generada**: la verificación de §C.1 con su salida.
- **Árbol compilado**: build sin errores ni warnings (warnings como error), con la suite de caracterización verde
  y el gate de compatibilidad de API verde **con la ruptura declarada**.
- **Firma intacta**: la constancia de que el material de firma, el naming y el versionado no cambiaron.
- **Ruptura registrada**: la entrada con sus tres datos obligatorios y la remisión a las notas del Bolt 028.
- **Sin sustituto**: la salida de las búsquedas de §D.3.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: La capa de datos no tiene el gate

**Given** el código de `Data` construido
**When** se busca el mecanismo del gate
**Then** **0** ocurrencias de `GetAssemblyToken`, `CheckForToken`, `GetEntryAssemblyTokenString`,
`_checkForTokenCounter` y `_entryAssemblyTokenString`, **0** comparaciones de `PublicKeyToken` en runtime y **0**
invocaciones del chequeo (las trece del inventario desaparecieron)

### AC-2: Ningún camino del framework lanza el error del gate

**Given** el árbol 2.0
**When** se buscan los lanzamientos del error de permisos por comparación de token
**Then** **0** rutas lanzan "This application is not authorized to use this library" **And** el tipo
`InsufficientSecurityRightsException` **sigue existiendo** y los **chequeos legítimos de derechos de seguridad**
de gateways y loaders lo siguen lanzando con su mensaje propio (no se retiró una comprobación de permisos)

### AC-3: La contraparte generada no está en la salida del set vigente

**Given** el set de plantillas vigente (Bolt 019)
**When** se inspeccionan sus plantillas y los artefactos generados disponibles
**Then** **no** existe la emisión del `override` de `GetAssemblyToken`, **no** existe la condición del flag del
gate y **no** hay ningún token ni literal de token en la salida

### AC-4: La firma sigue siendo identidad y no hay control sustituto

**Given** el árbol 2.0 y su material de firma
**When** se revisan la firma, el naming y el versionado, y se busca un control que reemplace al gate
**Then** el `.snk`, el naming y el versionado **no** cambiaron (la firma se mantiene como identidad, ADR-001
§3.3) y **no** aparece ningún control sustituto en código (ni licenciamiento, ni verificación de integridad en
runtime)

### AC-5: La ruptura está registrada con su reemplazo y el gate de API la cubre

**Given** el gate de compatibilidad de API contra la línea base 1.4
**When** se corre y se compara con el registro de rupturas
**Then** corre **verde**, la desaparición del miembro `GetAssemblyToken` y del token generado está **declarada**
con qué desaparece, por qué y con qué se reemplaza (borrar el `override` y regenerar con el set vigente), y **no**
hay ninguna diferencia de superficie sin declarar

### AC-6: La suite de caracterización queda verde

**Given** la suite de caracterización del Bolt 001
**When** corre sobre el árbol con el gate retirado
**Then** queda verde, con el comportamiento observable de la capa de datos intacto (el gate no participaba del
resultado de una operación legítima en los hosts que la 2.0 soporta)

### AC-7: La integridad de despliegue queda declarada como pendiente, con dueño y fecha

**Given** la evidencia del V-Bounce
**When** se lee la constancia sobre la integridad de despliegue
**Then** queda escrito que **no** se cubre en código, que pertenece a la capa de empaquetado y que su decisión
sigue pendiente con dueño (`tech_lead`) y fecha (antes del release 2.0.0) —sin reabrirla acá y sin sustituirla
por un control en runtime (ADR-001 §3.6)

### AC-8: El flag del gate no está en el formato (coherencia con el Bolt 002)

**Given** el contrato del `.coop2` y el migrador del Bolt 002
**When** se revisa el tratamiento del flag del gate retirado
**Then** el flag **no** forma parte del formato y el migrador lo descarta con una nota informativa —este Bolt
**no** toca el formato ni el migrador, y cita esa resolución como parte de la coherencia del retiro

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — la superficie de `Data` ya no expone los miembros del gate ni los invoca | §6/A.1 | AC-1 · búsqueda/inventario + gate de API |
| Bolt §2 — el código que produce el set vigente no contiene token ni implementa el chequeo | §6/C.1 | AC-3 · verificación estática del set + artefactos generados disponibles |
| Bolt §2 — ningún camino del framework lanza el error de permisos por comparación de token | §6/A.1, §6/D.3 | AC-2 · búsqueda de lanzamientos + verificación de que el camino de derechos sobrevive |
| Bolt §2 — la firma sigue siendo la misma, sin control sustituto | §6/B.1, §6/D.3 | AC-4 · revisión de material/naming/versionado + búsqueda de sustituto |
| Bolt §2 — la ruptura registrada en las notas, en el mismo acto | §6/D.1 | AC-5 · registro de rupturas + gate de API verde con la ruptura declarada |
| Bolt §2 — la suite de caracterización en verde | §6/E | AC-6 · corrida de la suite |
| ADR-001 §3.3 — el gate se retira, no se reemplaza, la firma queda como identidad | §6/B.1, §6/D.2 | AC-4, AC-7 · constancias de evidencia |
| ADR-008 §3.1/§5 — ruptura documentada con reemplazo y gate de API que la exige | §6/D.1 | AC-5 · registro de rupturas |
| ADR-003 §3.1/§3.4 — el set es del Bolt 019 y acá se verifica su ausencia | §6/C.1 | AC-3 · verificación sobre el set vigente |
| `OQ-003` (answered) — era detección de manipulación, no licenciamiento; sin clientes licenciados | §1, §2 | AC-4, AC-7 · narrativa de la ruptura en el MEM y en las notas |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Búsquedas/inventario (el núcleo de este Bolt):** verificación de **0** ocurrencias de `GetAssemblyToken`,
  `CheckForToken`, `GetEntryAssemblyTokenString`, `_checkForTokenCounter` y `_entryAssemblyTokenString` en el
  árbol 2.0; verificación de **0** comparaciones de `PublicKeyToken` en runtime; verificación de **0** rutas que
  lanzen el mensaje del gate ("This application is not authorized to use this library"); verificación de que las
  **trece** invocaciones del inventario desaparecieron (la búsqueda se corre **antes** del retiro para fijar el
  punto de partida y **después** para probar el cierre).
- **Verificación de no regresión del camino legítimo:** que `InsufficientSecurityRightsException` sigue en la
  superficie y que los chequeos de derechos de gateways/loaders siguen lanzándola con su mensaje propio (test
  unitario sobre un caso de derechos insuficientes, o búsqueda + su test de caracterización existente si el
  arnés ya lo cubre).
- **Verificación de la contraparte generada (estática):** sobre las plantillas del set vigente — ausencia de la
  emisión del `override`, de la condición del flag y de cualquier literal de token; sobre los artefactos
  generados disponibles en el árbol y en los fixtures — ausencia de las mismas cosas.
- **Verificación de la firma y del naming** (constancia): el material de firma y la política no cambiaron
  (comparación contra la línea base del repositorio) y no hay cambios de nombre ni de versión de ensamblado.
- **Verificación de ausencia de sustituto:** 0 controles nuevos de identidad de ensamblado, licenciamiento o
  integridad en el runtime del framework.
- **Consumer-side contract tests (test doubles):** `n/a` — el retiro elimina una superficie; lo que se verifica
  es **ausencia** (búsquedas, superficie publicada y gate de API) más la no regresión del camino de derechos, que
  es una prueba unitaria ordinaria. No hay contrato de componente que doblar.
- **Edge cases:** un `override` de `GetAssemblyToken` presente en un artefacto **del árbol** (no debería existir:
  si aparece, se **reporta** como frontera con el Bolt 019 y no se "arregla" acá); un `using` o miembro auxiliar
  que sólo existía para el gate; el helper `GetPublicKeyTokenFromArray` con otro uso legítimo (verificación antes
  de retirarlo); el mensaje del gate vs. el mensaje de derechos (no confundirlos en las búsquedas); documentación
  pública que mencionaba el miembro retirado; y el caso de un gateway **no** generado (que en 1.4 se saltaba el
  gate por `GetAssemblyToken() == null`) — el comportamiento observable del gateway común no cambia.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la salida de las búsquedas (antes/después), la superficie publicada sin
  los miembros, la verificación del set vigente y de los artefactos disponibles, la constancia de firma/naming y
  de ausencia de sustituto, la corrida de la suite de caracterización y el resultado del gate de compatibilidad de
  API **con la ruptura declarada**. Todo se pega al MEM y a `GateRetirementEvidence.md`.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `debt`: retira un control de ejecución y su
  contraparte generada, y su verificación es **sobre el árbol compilado, la superficie publicada y la salida del
  set**. No hay base de datos, cola, servicio, almacenamiento ni entorno desplegado involucrado: el gate no
  participaba de ninguna operación contra una dependencia real (de hecho, en 1.4 se **salteaba** cuando había
  `HttpContext` o cuando el ensamblado de entrada no existía).
- **Contract / E2E against the real providers:** `n/a` — el retiro no cruza un límite de proceso. La verificación
  de que la superficie del producto no rompe a un consumidor es **de fuente**, y su instrumento es el **gate de
  compatibilidad de API** (nivel V-Bounce, con la ruptura declarada), no un E2E.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales (el "caso real" —un host moderno donde el gate se saltaba— es precisamente el que
  desaparece).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); 0 código muerto del mecanismo retirado | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluida la prueba de no regresión del camino de derechos de seguridad | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el entregable es un **retiro**; se verifica por ausencia (búsquedas, superficie, gate de API) y por la no regresión del camino legítimo | n/a (razón) |
| SAST | V-Bounce | 0 hallazgos altos; 0 comparaciones de identidad de ensamblado en runtime | pass |
| Dependency scanning / licenses | V-Bounce | 0 dependencias nuevas; el retiro no cambia el presupuesto (ADR-001 §3.4/§5) | pass |
| OWASP Top 10 — static review | V-Bounce | Se retira un control **anti-manipulación**, no una autorización: en el runtime objetivo era **eludible** (web, `EntryAssembly` nulo, corte por contador) y producía falsos positivos/negativos (F-10/OQ-003). No queda ninguna decisión de seguridad delegada a un chequeo que no puede cumplirse; la verificación de integridad del despliegue se declara en la capa de empaquetado (ADR-001 §3.6) | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción ni se procesa material externo | n/a (razón) |
| Secret-leak scan | V-Bounce | 0 token embebido, 0 literal de token y 0 material de firma privado en el código, en la documentación o en los artefactos generados | pass |
| Hallucination lint | V-Bounce | Cada pieza retirada trazable a la ubicación del código 1.4 citada; 0 miembros "retirados" que en realidad no existían; 0 retiros de piezas con otro uso legítimo sin reportarlo | pass |
| IP / license provenance | V-Bounce | **Este gate tiene historia propia en este Bolt:** el mecanismo retirado **no** era licenciamiento (OQ-003: no hay clientes licenciados y el dueño declaró que se agregó como detección de manipulación). El retiro **no** altera la licencia del producto ni de las dependencias; el encabezado de licencia del código se conserva | pass |
| PII / DLP | V-Bounce | `internal`: el entregable no procesa datos personales (retiro de un control) | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes; y el retiro **elimina** una comparación de identidad de ensamblado hecha por el producto, que era un control de identidad mal puesto (su lugar es el empaquetado) | n/a (razón) |
| Test-first evidence | V-Bounce | El **inventario** (miembros, trece invocaciones, contraparte generada) se levanta **antes** de retirar y es el punto de partida del cierre por búsqueda; la lista de rupturas se redacta desde ADR-008 §3.1 **antes** de cerrar el V-Bounce | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la suite y de las búsquedas dan el mismo resultado (el retiro no introduce comportamiento no determinista: elimina un control que sí lo era) | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: la compatibilidad de fuente se mide con el gate de API (V-Bounce) | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: el retiro quita trabajo del camino de ejecución (una comparación y un contador); no hay objetivo de rendimiento que medir | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: la capa no agrega ni quita telemetría (§11) | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). El entregable retira un control y no
  procesa datos: no hay datos personales, ni credenciales, ni valores protegidos en su alcance.
- **Es la retirada de un control de seguridad, y se documenta como tal.** La justificación es explícita y
  verificable (F-10 + `OQ-003` **answered**): el mecanismo comparaba el `PublicKeyToken` del **ensamblado de
  entrada** y **no podía** cumplir su promesa en los hosts que la 2.0 soporta —se **salteaba** con `HttpContext`
  presente, se **salteaba** cuando `Assembly.GetEntryAssembly()` era `null` (IIS, *test runners*, single-file) y
  **cortaba** tras 200 llamadas—, con lo que producía **falsos negativos** (un cliente que no se evalúa) y
  **falsos positivos** (una app legítima empaquetada de otra forma). Era **detección de manipulación, no
  licenciamiento**, y no hay clientes licenciados por él (declaración del dueño en `OQ-003`).
- **La identidad no se pierde:** la **firma** de los ensamblados se mantiene sin cambios (mismo `snk`, misma
  política, mismo naming y versionado). Lo que desaparece es la **comparación de tokens hecha por el producto**,
  no la firma.
- **La integridad de despliegue no se abandona: se reubica.** Queda declarada como responsabilidad de la **capa
  de empaquetado** (feed controlado y Authenticode cuando se defina el canal), con **dueño y fecha** (el
  `tech_lead` antes del release 2.0.0, ADR-001 §3.6). La constancia en la evidencia evita que la ausencia del
  gate se lea como "no hay integridad".
- **Ningún sustituto en código:** introducir en el runtime un control nuevo (licenciamiento, verificación de
  firma, "chequeo de integridad") sería exactamente lo que ADR-001 §3.3 prohíbe; si el negocio lo pidiera, exige
  un ADR que reemplace a ADR-001 (§15).
- **Riesgo residual declarado:** un consumidor que dependiera del **error de permisos** del gate deja de verlo.
  El retiro se documenta explícitamente para que no se lea como "se quitó la licencia" (ADR-001 §3.3 y el
  registro de rupturas), y su camino alternativo —si necesitara proteger su despliegue— es la capa de
  empaquetado, no el framework.
- **Sin secretos en el árbol:** el retiro elimina literales de token de la salida generada; ningún material de
  firma **privado** vive en el repositorio (sólo el `.snk` de identidad del proyecto, que ya estaba y no cambia).

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es un **retiro** en una capa de biblioteca; no agrega ni
  quita logs, métricas ni trazas, y no se le agrega instrumentación (ninguna fuente lo pide; el control retirado
  no tenía diagnóstico propio más allá de su excepción).
- **La observabilidad de este Bolt es su evidencia de ausencia:** las búsquedas (0 ocurrencias, antes/después),
  la superficie publicada sin los miembros del gate, la verificación del set vigente y de los artefactos
  disponibles, la constancia de firma intacta, la constancia de ausencia de sustituto y el resultado del **gate
  de compatibilidad de API** con la ruptura declarada.
- **Efecto observable en el consumidor (lo que la evidencia debe dejar claro):** desaparece la excepción de
  permisos del gate y desaparece el `override` que el set viejo emitía en sus artefactos; lo primero es una
  mejora (el control no podía cumplirse) y lo segundo se resuelve regenerando con el set vigente. Ambas cosas se
  dicen en las notas, que es donde un consumidor las va a buscar.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay es una **ruptura de superficie documentada** más una
  **acción de migración del consumidor**: sus artefactos generados con el set viejo (que contienen
  `protected override byte[] GetAssemblyToken()`) deben **regenerarse** con el set vigente; y si el consumidor
  hubiera escrito su propio `override` (o dependido del error de permisos del gate), el reemplazo está declarado:
  **borrar el `override`** y **regenerar**, sin control equivalente (el escenario no existe en el runtime
  objetivo). El **1.4 sigue congelado** y es la contención soportada (ADR-008 §3.1): no hay compatibilidad binaria
  que preservar.
- **Compatibility:** las notas de migración (documento del **Bolt 028**, ADR-008 §3.1) reciben de acá la entrada
  con sus tres datos obligatorios: **qué** desaparece (el miembro `protected virtual byte[] GetAssemblyToken()`
  de `BaseGateway` y el token embebido en los artefactos generados), **por qué** (ADR-001 §3.3 con la evidencia
  de F-10/OQ-003) y **con qué se reemplaza** (borrar el `override`, regenerar con el set vigente; sin reemplazo
  funcional). El **gate de compatibilidad de API** del Bolt 001 verifica que esa entrada cubre **todas** las
  diferencias contra la línea base 1.4: una no declarada **falla** (ADR-008 §5).
- **Lo que NO cambia (y se declara, porque es la pregunta natural):** la **firma** de los ensamblados, el
  **naming**, el **versionado** (ADR-001 §3.3) y el **camino legítimo de derechos de seguridad** (el tipo
  `InsufficientSecurityRightsException` y sus comprobaciones en gateways y loaders siguen intactos).
- **Documentación pública:** la superficie documentada del paquete deja de mencionar el miembro retirado; el
  `README`/API del paquete no puede seguir prometiendo un control que ya no existe (ADR-008 §3.1: la ruptura se
  documenta **con su reemplazo**).
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido, ni artefacto del consumidor
  reescrito, ni dato migrado. Si un consumidor ya hubiera regenerado con el set vigente, revertir el framework
  lo llevaría a una versión anterior del paquete (la contención sigue siendo el 1.4 congelado, ADR-008 §3.1); no
  se reintroduce el gate "por si acaso": que el gate vuelva exigiría revertir también la decisión (ADR-001 §3.3),
  no sólo el código.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Retiro parcial**: sobrevive una invocación, un `using` o el helper del gate | 3 | 3 | Inventario **antes** del retiro (miembros + **trece** invocaciones + contraparte generada) y cierre por **búsqueda** después (AC-1); la suite y el gate de API como red |
| **Se retira de más**: se elimina un chequeo **legítimo** de derechos de seguridad por confundirlo con el gate | 3 | 4 | §A.1 y §14 declaran la distinción (mensajes distintos, tipo que **se conserva**) y hay una prueba de **no regresión** del camino de derechos delante de AC-2 |
| **El set vigente todavía emite el token** (Bolt 019 no ejecutado) y la evidencia del lado generado es falsa o imposible | 3 | 4 | §5/§15: prerrequisito con **condición de parada**; la verificación se hace **sobre las plantillas del set vigente** y se registra qué se verificó, para que no se declare una ausencia que no se midió |
| **`GetPublicKeyTokenFromArray` se retira y tenía otro uso legítimo** | 2 | 3 | Verificación explícita de sus usos **antes** de retirarlo; si lo tiene, se **reporta** y se conserva (o se retira su parte del gate) |
| **Un consumidor dependía del error de permisos** del gate | 3 | 3 | La ruptura se documenta con su reemplazo y con la motivación (no era licenciamiento; OQ-003) para que no se lea como "se quitó la licencia"; el camino es la capa de empaquetado |
| **"No hay gate" se confunde con "no hay integridad"** | 3 | 3 | §D.2 y AC-7: la constancia con dueño y fecha (ADR-001 §3.6) es parte de la evidencia, y el riesgo queda declarado |
| **Aparece un control sustituto** (licenciamiento, verificación de integridad en runtime) | 2 | 4 | AC-4/§D.3: búsqueda de controles nuevos + ADR-001 §3.3 lo prohíbe (un sustituto exigiría un ADR que lo reemplace) |
| **La firma se toca "de paso"** (cambio de `snk`, de naming o de versionado) | 1 | 4 | AC-4 y §B.1: la constancia de material/naming/versionado sin cambios es evidencia obligatoria; ADR-001 §3.3 lo fija |
| **Documentación que sigue prometiendo el control** | 3 | 2 | §12: la superficie documentada del paquete deja de mencionarlo (ADR-008 §3.1: cada ruptura con su reemplazo) |
| **Artefactos generados del consumidor que dejan de compilar** | 4 | 2 | Es la ruptura esperada y declarada: regenerar con el set vigente. La nota de migración lo dice primero, antes de que aparezca como sorpresa |
| **Ruptura sin declarar** frente al gate de API | 2 | 3 | AC-5: una diferencia no listada **falla** el gate (ADR-008 §5); el registro de rupturas y el diff se cierran en el mismo acto |
| **El alcance se corre a la criptografía de valores** (confundir el token con una clave protegida) | 2 | 3 | §4: ADR-007/Bolts 022-023 están fuera; un token de ensamblado no es un valor protegido y este Bolt no toca la política criptográfica |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-001/ADR-008/ADR-003 y en la
> respuesta de `OQ-003`; acá se aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el
> `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) `InsufficientSecurityRightsException` se conserva; sólo cae el camino del gate** | El tipo lo usan los **chequeos legítimos de derechos de seguridad** (`BaseGateway.cs:242/487/…`, `BaseLoader.cs:106/309/…`) con un mensaje distinto del del gate ("Your's security rihgts are not good enough for this operation" vs. "This application is not authorized to use this library"). Retirar el tipo o esas comprobaciones sería retirar una comprobación de permisos, que **no** es lo que el Bolt ordena. La distinción se verifica por búsqueda de mensaje y por una prueba de no regresión |
| **(⚠ aprobador) La verificación de la contraparte generada se hace sobre las plantillas del set vigente (estática) y, si están disponibles, sobre los artefactos generados; la corrida del motor es corroboración opcional** | El set es del **Bolt 019** y el motor (Bolts 017/018) puede no estar ejecutado al hacer este Bolt. La verificación estática sobre el set **es concluyente** sobre qué emite; exigir la corrida del motor convertiría una dependencia de conveniencia en requisito. Si el aprobador quiere la corrida como obligatoria, se agrega (y se agrega el prerrequisito del motor) |
| El **token del formato** (`GenerateCheckForToken`) no se re-toca | Ya está resuelto por el Bolt 002: el `.coop2` lo excluye y el migrador lo reporta con una nota informativa (`SPEC-261016-0002` §6/C.1). Reabrirlo sería duplicar el trabajo de otro Bolt y tocar el formato, que este Bolt no tiene en alcance |
| La **limpieza de la documentación pública** se trata como parte del retiro | ADR-008 §3.1 exige la ruptura **con su reemplazo**, y ADR-001 §3.3 asocia al retiro la limpieza del proyecto: una superficie documentada que promete un control inexistente es deuda y confusión para el consumidor |
| La **constancia de la integridad de despliegue** (dueño y fecha) es una salida obligatoria del MEM | Es la respuesta al riesgo "no hay gate ≠ no hay integridad" y la única forma de que el retiro no se lea como abandono de una propiedad de seguridad (ADR-001 §3.3/§3.6) |
| Se verifica **0 comparaciones de `PublicKeyToken` en runtime** en lugar de "no hay gate" como afirmación narrativa | Un control sustituto no se llama igual: la verificación por mecanismo (comparar identidad de ensamblado) es la que detecta un reemplazo encubierto |
| No se crea una banda de tests nuevos más allá de la evidencia y la no regresión | El entregable es un **retiro**: el instrumento natural es la búsqueda/inventario, la superficie publicada y el gate de API del Bolt 001. Montar una suite propia para código que desaparece sería ceremonial |
| El inventario cita **trece** invocaciones y no "las invocaciones" | Un retiro "a ojo" es el modo de fallar de este Bolt: el inventario numerado y su cierre por búsqueda es lo que hace verificable el resultado (y lo que el MEM documenta) |

---

## 15. Stop conditions

- **El set de plantillas vigente (Bolt 019) todavía emite el token o su chequeo**: **parar y reportar**. La
  evidencia del lado generado no se puede producir y este Bolt **no** reescribe el set (es de otro Bolt): la
  salida es ordenar la ejecución del Bolt 019 antes y reanudar.
- **Aparece un `override` de `GetAssemblyToken` dentro del árbol 2.0** (no en artefactos de un consumidor): parar
  y reportar; es una frontera con el set del Bolt 019, no algo que se borre desde acá.
- **Se pide introducir un control sustituto** (licenciamiento, verificación de integridad de ensamblado, chequeo
  de firma en runtime): parar. ADR-001 §3.3 lo prohíbe expresamente; su lugar es la capa de empaquetado y hacerlo
  en el framework exigiría un **ADR que reemplace a ADR-001**.
- **La firma, el naming o el versionado tendrían que cambiar** para completar el retiro: parar y reportar
  (ADR-001 §3.3: la firma se mantiene como identidad y este Bolt no la toca).
- **`GetPublicKeyTokenFromArray` (u otro helper) tiene un uso legítimo fuera del gate**: parar y reportar antes de
  retirarlo; el retiro se limita a la parte del gate.
- **El camino legítimo de derechos de seguridad se vería afectado** por el retiro (una prueba de no regresión
  falla, o la búsqueda muestra que se retiraría una comprobación de permisos): parar y reportar; no se retira una
  autorización por parecerse a un gate retirado.
- **La ruptura no se puede describir con su reemplazo** (ADR-008 §3.1): parar; una ruptura sin reemplazo
  declarado no puede entrar al registro, y el gate de API la haría fallar sin explicación.
- **El canal de distribución o la integridad de despliegue pretenden resolverse en este Bolt**: parar. Es la
  decisión pendiente de ADR-001 §3.6, con dueño y fecha, y no se reabre desde un Bolt.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, `OQ-003`, la línea base de
  código): parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-001 §3.3/§3.6, ADR-008 §3.1/§5, ADR-003 §3.1/§3.4)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-015`](../functional/bolts/US-000.BOLT-015-retiro-del-gate-token.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-10 (el gate por `PublicKeyToken`: ubicación, comportamiento en hosts modernos, contraparte generada) y §6 ítem 11 (el retiro lo documenta el ADR de empaquetado).
- [`OQ-003`](../analysis/open-questions/OQ-003-gate-de-licenciamiento.md) — **answered** (2026-10-15): opción A, retirar el gate; era detección de manipulación, no licenciamiento comercial; no hay clientes licenciados; la firma se mantiene como identidad y la integridad de despliegue se cubre en el empaquetado.
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.3 (el gate se retira, no se reemplaza en código; la firma como identidad; limpieza obligatoria de proyecto), §3.6 (canal de distribución e integridad de despliegue: decisión pendiente con dueño y fecha), §3.1 (layout y `Data`).
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (rupturas documentadas con reemplazo), §3.5 (SemVer y registro verificado por gate), §5 (gate de compatibilidad de API con la línea base 1.4 y su dueño).
- [`ADR-003`](../adrs/ADR-003-motor-scriban-headless-y-cli.md) — §3.1 (el set de artefactos que el motor genera) y §3.4 (propiedad del código generado).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, suite de caracterización y gate de compatibilidad de API (prerrequisito, Bolt 001).
- [`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md) — la forma final de la capa de datos (prerrequisito, Bolt 014).
- [`SPEC-261016-0002`](./SPEC-261016-0002-migrador-coop-a-coop2.md) — el retiro del flag del gate en el formato y en el migrador (§6/C.1): la parte del modelo ya está resuelta y se cita como coherencia.
- [`US-000.BOLT-019`](../functional/bolts/US-000.BOLT-019-plantillas-csharp.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el dueño del set que deja de emitir el token (prerrequisito de la verificación) y el documento de notas que recibe la ruptura.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-015`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, inventario cerrado de lo que se retira (miembros, **trece** invocaciones y contraparte generada), estrategia de pruebas con la verificación de ausencia y la no regresión del camino de derechos, gates, control de datos `internal` y condiciones de parada. Declara la frontera con el Bolt 019 (dueño del set) y la dependencia de orden con su ejecución. `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (datos / seguridad de superficie)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
