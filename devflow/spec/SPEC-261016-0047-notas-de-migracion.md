---
id: "SPEC-261016-0047"
title: "Notas de migración de la 2.0 y registro de rupturas: contribución distribuida por Bolt, formato validado y cobertura cruzada con el gate"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-028" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-008", "ADR-001", "ADR-002", "ADR-004", "ADR-007"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # el arnés: dueño del gate de compatibilidad de API y de la línea base 1.4 (Bolt 001)
  - "SPEC-261016-0012-retiro-del-export-excel.md" # el fragmento `2.0/docs/api-removals/<BOLT>.json` y su prueba de cobertura (Bolt 006)
  - "SPEC-261016-0013-retiro-de-serializable.md" # el mismo formato por ruptura y la constancia del escape-hatch (Bolt 007)
risk_class: "low" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (debt): entrega documentación de la migración —el registro de rupturas y las notas— y no cambia nada desplegado ni toca un servicio externo."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:52:21-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:52:21-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:52:21-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-028 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «Contribucion distribuida del registro confirmada…»), ADR-008/ADR-001/ADR-002/ADR-004/ADR-007 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class low → autonomy_level L3; data_classification internal; test_integration not-required → applicability n/a (razón). Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: contribución distribuida (cada Bolt entrega su fragmento; 028 no es prerequisito de nadie) + formato publicado compartido (api-removals/<BOLT>.json + schema + MIGRATION-NOTES.md); cobertura en los DOS sentidos contra el gate (0 diferencias sin entrada / 0 entradas sin diferencia) + entradas de comportamiento/deprecación contra las decisiones; registro del escape-hatch; consolidación determinista (orden canónico)."
    - "ADJUDICACIÓN (a) documento consolidado en 2.0/docs/MIGRATION-NOTES.md junto a los fragmentos — CONFIRMADA: §14 fila 1 adopta la convención ya fijada por 006/007 (api-removals/<BOLT>.json) y fija el documento consolidado en MIGRATION-NOTES.md, que ninguna fuente había fijado; no se inventa un segundo registro ni un formato nuevo (§14: «No se define un formato nuevo de notas narrativas separado del registro»)."
    - "ADJUDICACIÓN (b) dos clases de entrada con validaciones distintas — CONFIRMADA: §14 fila 2 y §C separan api_removal (cruzada contra el gate en los dos sentidos) de behavior_change/deprecation (cruzada contra las decisiones que las ordenan, con el mapa de §A.1). Es correcto: el gate sólo compara API pública, y las no-atributivas (caché que expira, App.config→appsettings.json, lo que el modelo deja de transportar, migración de valores) no aparecen en el reporte del gate —validarlas contra su decisión es lo que las hace trazables sin fingir que el gate las cubre (AC-4/AC-5)."
    - "ADJUDICACIÓN (c) validación en la suite del arnés (MigrationNotesCoverageTests) — CONFIRMADA: §14 fila 3 y §E fijan la prueba en 2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs, no en una herramienta nueva (el gate y su línea base son del Bolt 001, ADR-008 §5); la prueba precedente ApiRemovalCoverageTests se CONSERVA y este Bolt generaliza su criterio agregando la validación de esquema y el cruce no atributivo (§E nota de arnés)."
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
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-028`. El gate de compatibilidad
     de API y la línea base son del Bolt 001 (**consumidos**); cada ruptura la ordena su ADR y la ejecuta su
     Bolt; las notas **no** deciden rupturas ni enmiendan ADRs.
  ⚠️ PROPIEDAD DISTRIBUIDA DEL REGISTRO (resolución obligatoria del par revisor, en el DoR del Bolt): cada Bolt
     que rompe superficie entrega **su propia entrada** en `2.0/docs/api-removals/<BOLT>.json` en el mismo acto
     que su cambio (ADR-008 §3.1); este Bolt **estructura, valida y consolida** — no es dueño exclusivo, porque
     si lo fuera los Bolts que rompen superficie fallarían su gate **antes** de que las notas existieran
     (ADR-008 §5). Está recogido en §2/§4/§14 y es la razón del `acknowledgment_reason` de su READY.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
-->

# SPEC-261016-0047 — Notas de migración de la 2.0 y registro de rupturas: contribución distribuida por Bolt, formato validado y cobertura cruzada con el gate

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 12 y §6 ítem 15, vía ADR-008) |
| **Bolt** | [US-000.BOLT-028](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) |
| **ADRs** | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-007](../adrs/ADR-007-politica-criptografica.md) |
| **Risk Class** | low |
| **Revision** | 1 |

---

## 1. Objective

Se entregan las **notas de migración de la 2.0** —el **registro estructurado por área** de las rupturas de
superficie y de las deprecaciones, cada una con **qué** desaparece, **por qué** y **con qué** se reemplaza, más el
ítem que la ejecuta— en una forma que **cualquier Bolt que rompa superficie pueda completar con su propia
entrada**, y con una **cobertura verificable contra el gate de compatibilidad de API**: la comparación de
superficie cruzada contra el registro **no deja diferencias sin cubrir** ni **entradas sin ruptura** (ADR-008 §5).

El problema que resuelve está documentado (ADR-008 §4, "Technical debt"): las rupturas de naturaleza muy distinta
—serialización, `SqlHelper` a `internal`, criptografía estática, `App.config` → `appsettings.json`, splits de
paquetes— se acumulan en un solo documento y, **sin una estructura por área y sin cobertura verificable**, dos
cosas se rompen: el consumidor las lee peor y el gate **falla sin explicación** (una diferencia no documentada
falla) o **pasa sin trazabilidad** (una nota que no corresponde a ninguna diferencia).

Si no se implementa: el gate de API del Bolt 001 queda con diferencias sin explicar, la ruptura deja de ser
trazable a la decisión que la ordena y el camino de migración del consumidor (ADR-001 §3.2) queda sin su
documento.

---

## 2. Context

**De dónde viene la necesidad.** ADR-001 §3.2 fijó el **camino de migración del consumidor** (convertir, repuntar,
resolver, regenerar) y ADR-008 §3.1 obligó a **documentar cada ruptura con su reemplazo**; ADR-008 §5 creó el
**gate** que cruza la superficie contra las notas y **falla** ante una diferencia no cubierta. Lo que faltaba era
el **registro** con el que ese gate convive, en una forma que **exista mientras las rupturas ocurren**.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-008 §3.1** — toda ruptura de superficie frente a 1.4 se documenta con **tres datos obligatorios**:
  (1) qué miembro, atributo o comportamiento desaparece; (2) **por qué** (la decisión que lo ordena); (3) **con
  qué se reemplaza** en el código del consumidor, o "**sin reemplazo**, porque el escenario no existe en el
  runtime objetivo" —más el ítem del ruteo/Bolt que lo ejecuta—. **Sin supresiones globales de advertencias** y
  sin promesas de mecanismos que el framework ya no sostiene (§3.1/§3.3).
- **ADR-008 §3.5/§3.6** — la línea 2.0.x evoluciona con SemVer (patch sin cambios de superficie, minor con
  deprecación **avisada**, major para romper) y el **escape-hatch** (conservar `[Serializable]`/
  `SerializationInfo`) tiene **cinco criterios** que deben cumplirse en conjunto; por defecto la respuesta es
  **no**, y una excepción que exista queda **registrada** (tipo, motivo, consumidor identificado y vencimiento).
- **ADR-008 §5 (el gate y su dueño)** — el NFR de **trazabilidad de rupturas** se mide cruzando el **API público**
  contra la línea base 1.4 y contra la línea anterior de la 2.0, con las notas de migración: **100 %** de las
  diferencias cubiertas, **0** rupturas sin entrada, **0** entradas sin ruptura asociada. El **dueño del gate y de
  la línea base** es el **Bolt del arnés (001)** — este Bolt lo **consume** y **no** lo implementa.
- **Propiedad distribuida del registro (resolución obligatoria recogida en el DoR del Bolt)** — el registro es de
  **contribución distribuida**: **cada Bolt que rompe superficie entrega su propia entrada** en el mismo acto que
  su cambio, y este Bolt **estructura, valida y consolida**. Hacer de este Bolt el dueño exclusivo haría que los
  Bolts que crean o rompen superficie (003, 009, 011, 022, 027…) **fallaran su gate antes** de que las notas
  existieran — exactamente lo que la exclusión impide.
- **La convención ya establecida por los Bolts 006 y 007** — el fragmento vive en
  **`2.0/docs/api-removals/<BOLT>.json`** (una entrada por miembro retirado con los tres datos, el Bolt
  responsable y la evidencia), y la **cobertura** se verifica en el arnés
  (`2.0/tests/Cooperator.Harness.Tests/ApiRemovalCoverageTests.cs`) cruzando el reporte del gate contra los
  fragmentos. Este Bolt **adopta y generaliza** esa convención: es el formato que publica y el cruce que valida en
  los dos sentidos.
- **ADR-001 §3.2** — el camino de migración del consumidor (y que la coexistencia **no** es lado a lado) es lo que
  las notas deben hacer entendible; **ADR-002 §3.1/§3.3** (lo que el modelo deja de transportar y las
  deprecaciones de alcance del corpus legacy, con su **paso siguiente obligatorio**), **ADR-004 §3.1** (los cambios
  de comportamiento del caché, que son notas y no sorpresas de ejecución) y **ADR-007 §3.1/§3.3** (el retiro de la
  superficie estática de criptografía y la migración de valores como ruptura con reemplazo **procedimental**)
  aportan las rupturas **no atributivas** que el registro debe cubrir además de las de API.

**Qué decide este SPEC, dentro de esas restricciones.** La **estructura del registro por área** y su mapa con las
decisiones del ciclo; el **formato publicado** del fragmento (con su esquema y su validador, para que cualquier
Bolt contribuya sin adivinar); las **dos clases de entrada** (ruptura de **superficie**, cruzable contra el gate, y
**comportamiento/deprecación**, cruzable contra las decisiones que la ordenan) y **cómo se valida cada una**; la
**consolidación** en el documento de notas; y el **registro de las excepciones del escape-hatch** (o su decisión
negativa documentada).

**Fronteras declaradas.** El **gate**, el **reporte de compatibilidad** y la **línea base 1.4** son del **Bolt
001** (ADR-008 §5); cada ruptura la **ordena su ADR** (que es **inmutable**: este Bolt no lo enmienda) y la
**ejecuta su Bolt**; el **camino de migración paso a paso por aplicación** es de ADR-001 §3.2 y de la
documentación de cada herramienta; y los **fragmentos** son de cada Bolt que rompe superficie.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — `status: ready`, `risk_class: low`, `data_classification: internal`, `test_integration: not-required`; el `acknowledgment_reason` confirma la **contribución distribuida** del registro y que este Bolt **no** es dueño exclusivo | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:12:35-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| ADR | [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (los tres datos obligatorios de cada ruptura y el procedimiento de documentación), §3.3 (qué ocupa el lugar del estado serializado: "sin reemplazo" con su razón), §3.5/§3.6 (SemVer y el escape-hatch con sus cinco criterios), §5 (el NFR de trazabilidad y **el gate** con su dueño) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.2 (el camino de migración del consumidor y la no-coexistencia lado a lado) y §4 (las rupturas que **deben** aparecer en las notas: retiro de la serialización, de las features de plataforma y reemplazo de la configuración) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (lo que el modelo deja de transportar) y §3.3 (las deprecaciones de alcance en el corpus legacy y el **paso siguiente obligatorio**) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.1 (los cambios de comportamiento del caché que son notas) y §3.4 (`App.config` → `appsettings.json` + entorno, con su precedencia) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (el retiro de la superficie estática de criptografía) y §3.3 (la migración de valores como **ruptura con reemplazo procedimental**) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — su SPEC canónica ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)) entrega el **gate**, el **reporte de compatibilidad** y la **línea base 1.4** | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Previous Bolts (convención del fragmento, no fuente gobernada) | [`US-000.BOLT-006`](../functional/bolts/US-000.BOLT-006-retiro-del-export-excel.md) ([`SPEC-261016-0012`](./SPEC-261016-0012-retiro-del-export-excel.md)) y [`US-000.BOLT-007`](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) ([`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md)) — fijaron `2.0/docs/api-removals/<BOLT>.json` y la prueba de cobertura | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de artefactos usados como evidencia** (existentes o fijados por otros SPECs, citados como convención):
`2.0/docs/api-removals/US-000.BOLT-006.json` y `…/US-000.BOLT-007.json` (los dos fragmentos ya definidos, con los
tres datos y su lista en prosa en el MEM de cada Bolt) y
`2.0/tests/Cooperator.Harness.Tests/ApiRemovalCoverageTests.cs` (la prueba de cobertura que ambos SPECs declaran
—extendida por cada Bolt que rompe superficie—). Nada de esto se modifica acá: este Bolt **publica el formato**,
**generaliza la validación** y **consolida**.

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- La **estructura del registro por área** —contenedor y empaquetado, formato del modelo, configuración y caché,
  criptografía, superficie de serialización, capa de datos y conjuntos de plantillas— con los **tres datos
  obligatorios** de cada entrada y el **ítem/Bolt** que la ejecuta.
- El **mecanismo de contribución distribuida**: cada Bolt que rompe superficie entrega **su fragmento**
  (`2.0/docs/api-removals/<BOLT>.json`) en el mismo acto que su cambio, y este Bolt **estructura, valida y
  consolida** sin ser dueño exclusivo ni prerequisito de nadie.
- El **formato publicado** del fragmento —con su **esquema** y su **validador**— para que cualquier Bolt pueda
  contribuir sin adivinar la forma ni los datos exigidos.
- Las **dos clases de entrada y su validación**: (a) **ruptura de superficie** (miembro/atributo retirado) →
  cruzada contra el **reporte del gate** en los **dos sentidos**; (b) **comportamiento/deprecación** (cambios que
  no son diferencias de API: el caché que expira, `App.config` → `appsettings.json`, lo que el modelo deja de
  transportar, la migración de valores de criptografía) → cruzada contra las **decisiones que las ordenan**.
- El **registro de las excepciones del escape-hatch** (tipo, motivo, consumidor identificado y vencimiento) o la
  **decisión negativa documentada** de que ninguna existe.
- El **señalamiento del paso siguiente obligatorio** del consumidor cuando una ruptura lo exige más allá del
  cambio de versión (por ejemplo **regenerar** artefactos en lugar de convertir código).
- La **consolidación** en el documento de notas de migración de la 2.0, con su estructura por área.

### Out of scope

- **Decidir** rupturas: cada una la **ordena su ADR** (inmutable) y la **ejecuta su Bolt**. Este Bolt las
  **registra**; no las autoriza (una nota **no** autoriza nada, ADR-008 §5).
- Ser el **dueño exclusivo** del registro, o volverse **condición** para que otro Bolt registre su ruptura
  (exclusión explícita del Bolt, por la resolución del par revisor).
- El **gate de compatibilidad de API** y la **línea base 1.4**: **Bolt 001** (ADR-008 §5) — se **consumen**.
- El **camino de migración paso a paso por aplicación**: ADR-001 §3.2 y la documentación de cada herramienta
  (migrador, CLI, shell).
- **Enmendar** el texto de un ADR o prometer **compatibilidad binaria** / un mecanismo de serialización que el
  framework ya no sostiene (ADR-008 §3.3).
- Los **fragmentos de los demás Bolts** como trabajo de este Bolt: los produce cada Bolt; acá se valida su
  **cobertura** y se consolidan.

---

## 5. Prerequisites and baseline

- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md), `prerequisites`):** el **gate de
  compatibilidad de API** con su **línea base 1.4**, el **reporte de diferencias** y la suite del arnés donde vive
  la prueba de cobertura. Sin el gate, el cruce de este Bolt no tiene contra qué verificar y la trazabilidad
  quedaría **declarativa** (§15). Si no está en el árbol 2.0, **parar y reportar**.
- **La convención ya fijada por los Bolts 006 y 007
  ([`SPEC-261016-0012`](./SPEC-261016-0012-retiro-del-export-excel.md), [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md),
  `prerequisites`):** el fragmento `2.0/docs/api-removals/<BOLT>.json` con los tres datos, el Bolt responsable y
  la evidencia, y la prueba de cobertura que cruza el gate contra los fragmentos. Este Bolt **adopta** esa forma
  (no inventa una segunda) y la publica como formato.
- **Los fragmentos que otros Bolts aportan a medida que se ejecutan:** son la materia del registro. Este Bolt
  **no** espera a todos para existir (el registro se completa por contribución) y su consolidación se **extiende**
  con cada fragmento nuevo (§14).
- **Build state de partida:** el árbol 2.0 tiene el arnés y el gate corriendo; este Bolt **no** cambia código de
  producto: entrega **documentación**, un **esquema** y **casos de validación** en la suite del arnés.
- **Decisión de ejecución registrada (no es supuesto):** el cruce se hace sobre el **reporte del gate** de la línea
  base 1.4 y sobre los **fragmentos** del repositorio; no hay dependencia externa, base de datos ni entorno (de ahí
  el plan de integración `n/a`).

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-028`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — El registro estructurado y el formato publicado

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 La estructura por área

El registro se organiza **por área**, cada una con su lista de entradas, y el área no es un adorno: es lo que
resuelve la deuda declarada por ADR-008 §4 ("el consumidor las leerá peor"). El mapa entre áreas y decisiones del
ciclo es explícito:

| Área | Decisiones que la ordenan |
|---|---|
| **Contenedor y empaquetado** | ADR-001 §3.1/§3.2/§3.3/§3.5 (layout, coexistencia, naming/versionado, retiro de features de plataforma) |
| **Formato del modelo** | ADR-002 §3.1/§3.3 (lo que el `.coop2` deja de transportar y las deprecaciones de alcance del corpus) |
| **Configuración y caché** | ADR-004 §3.1/§3.2/§3.3/§3.4 (cambios de comportamiento del caché, catálogo, prefijo, `appsettings.json`) |
| **Criptografía** | ADR-007 §3.1/§3.2/§3.3 (retiro de la superficie estática y la migración de valores) |
| **Superficie de serialización** | ADR-008 §3.2/§3.3 (lo que se retira y qué lo reemplaza) |
| **Capa de datos** | ADR-005 §3.1/§3.3 (proveedor único y superficie que pasa a `internal`) |
| **Conjuntos de plantillas** | ADR-003 §3.1/§3.4/§3.7 del ADR-008 (lo que el set deja de emitir) |

#### A.2 El formato del fragmento, publicado

Se **publica** el formato del fragmento —que los Bolts 006 y 007 ya usan— con su **esquema** y un **validador**,
de modo que un Bolt nuevo no tenga que adivinar la forma ni los datos exigidos:

- **Un archivo por Bolt**: `2.0/docs/api-removals/<BOLT>.json` (por ejemplo `US-000.BOLT-006.json`).
- **Una entrada por ruptura** con: **(1)** qué desaparece (tipo, miembro **y firma exacta**; o el **comportamiento**
  que cambia, con su descripción y su alcance); **(2)** **por qué** (la decisión que lo ordena: ADR y cláusula —
  y, cuando exista, la OQ/REV que la motivó—); **(3)** **con qué se reemplaza** (la forma concreta en el código del
  consumidor) o `"none"` con la **razón obligatoria** ("el escenario no existe en el runtime objetivo"); más el
  **Bolt responsable**, la **evidencia** de su verificación y, si corresponde, el **paso siguiente obligatorio**
  del consumidor.
- **La clase de la entrada** es explícita (`api_removal` | `behavior_change` | `deprecation`), porque **cada clase
  se valida distinto** (§C).

**Files created:**
- `2.0/docs/api-removals/README.md` — el formato publicado del fragmento: campos, las tres clases, los tres datos obligatorios y cómo contribuye un Bolt
- `2.0/docs/api-removals/api-removals.schema.json` — el **esquema** del fragmento (validación estructural de cada entrada)
- `2.0/docs/MIGRATION-NOTES.md` — el **documento de notas de migración** de la 2.0, estructurado por área y consolidado desde los fragmentos

---

### Phase B — La contribución distribuida

**Duration:** 2h total cycle — **Complexity:** Low

#### B.1 El mecanismo, y su prueba

Cada Bolt que rompe superficie entrega **su** fragmento en el mismo acto que su cambio (ADR-008 §3.1) —lo que los
SPECs de los Bolts 006 y 007 ya hacen, y varios más declaran hacer (§3)— y **este Bolt no es prerequisito de
nadie**: no hay dependencia "primero las notas, después la ruptura". La verificación es **por diferencia**, no por
documento: un Bolt cuya ruptura esté cubierta por su fragmento pasa su gate **aunque estas notas todavía no
existan** — y ese es exactamente el criterio que el DoR del Bolt fijó.

La **consolidación** de este Bolt lee los fragmentos presentes y arma el documento por área; cuando llega un
fragmento nuevo, la consolidación se **extiende** (no se rehace el mecanismo): es la propiedad que el Bolt pide
("el registro existe mientras las rupturas ocurren").

---

### Phase C — La cobertura cruzada (los dos sentidos, por clase)

**Duration:** 3h total cycle — **Complexity:** High

#### C.1 Las rupturas de superficie: contra el gate, en los dos sentidos

- **Diferencia sin entrada (falla):** toda diferencia de tipo «miembro retirado» o «atributo retirado» entre la
  **línea base 1.4** y la superficie 2.0 —según el **reporte del gate** del Bolt 001, para los ensamblados en
  alcance— debe estar cubierta por **al menos una** entrada de algún fragmento.
- **Entrada sin diferencia (falla):** toda entrada de clase `api_removal` debe corresponder a una diferencia real
  del reporte (no se registran retiros que no ocurrieron ni se "explica" lo que no cambió).

Esto es la parte **ejecutable** del NFR de trazabilidad de ADR-008 §5, y **extiende** la prueba de cobertura que
los Bolts 006/007 ya declaran: la prueba lee **todos** los fragmentos (`api-removals/*.json`), no sólo el del Bolt
que la escribió.

#### C.2 Las entradas de comportamiento y de deprecación: contra las decisiones

Las entradas que **no** son diferencias de API (`behavior_change`, `deprecation`) no pueden cruzarse contra el
reporte del gate: se cruzan contra las **decisiones que las ordenan**:

- **Cobertura por decisión:** cada ADR del ciclo que **declara** un cambio de comportamiento o una deprecación
  debe tener **al menos una** entrada en el área que le corresponde (por ejemplo: el vencimiento del caché de
  colecciones y el reemplazo de `App.config`; lo que el modelo deja de transportar; la migración de valores de
  criptografía; lo que el set de plantillas deja de emitir). La lista de decisiones a cubrir es la del **mapa de
  §A.1** y se verifica contra los ADRs vigentes del proyecto.
- **Sin invenciones:** una entrada de comportamiento cita **la cláusula** que la ordena; si el registro contiene
  un cambio que ninguna decisión ordena, **falla** (sería documentación inventada, con el mismo criterio que la
  entrada sin diferencia de §C.1).
- **La entrada de deprecación** declara además su **ventana de aviso** cuando la línea 2.0.x la tenga (ADR-008
  §3.5: minor con `[Obsolete]` avisado antes del retiro en el próximo major).

---

### Phase D — Las excepciones y el paso siguiente

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 El registro de las excepciones del escape-hatch

Si algún tipo **reclama** el escape-hatch de ADR-008 §3.6, el registro lo declara con sus **cinco criterios**
verificados (consumidor identificado con evidencia, imposibilidad demostrada de resolverlo portablemente, nunca
como mecanismo de persistencia nuevo, test del escenario y **vencimiento** declarado). Si **ninguna** existe, se
documenta la **decisión negativa** con su constancia (que es lo que el Bolt 007 ya declaró para los 35 tipos del
framework y los del grafo) — así el registro no queda ambigua entre "no hay excepciones" y "no se miró".

#### D.2 El paso siguiente obligatorio del consumidor

Cuando una ruptura exige **más** que un cambio de código (el caso paradigmático: **regenerar** artefactos con el
motor en lugar de convertir código —ADR-002 §3.3 y ADR-003—, o **migrar valores** con la herramienta del ADR-007
§3.3), la entrada lo señala como **paso siguiente obligatorio** con su referencia. Es lo que hace que las notas
sirvan para migrar y no sólo para explicar.

---

### Phase E — La consolidación y la evidencia

**Duration:** 2h total cycle — **Complexity:** Medium

- **El documento de notas** (`2.0/docs/MIGRATION-NOTES.md`) consolidado desde los fragmentos, **por área**, con
  cada entrada trazable a la **decisión** que la ordena y al **Bolt** que la ejecuta, y con el **encabezado** que
  declara: qué es este documento, de dónde sale (los fragmentos), quién lo mantiene (contribución distribuida) y
  qué **no** es (no autoriza rupturas: el gate no autoriza, ADR-008 §5).
- **La evidencia del mecanismo:** un fragmento **llegado desde otro Bolt** presente en el registro con su
  reemplazo —ejercitando el mecanismo en lugar de enunciarlo (los de 006/007 son la prueba disponible; cualquier
  Bolt posterior que aporte el suyo la refuerza)—.
- **El resultado del cruce** en los dos sentidos (diferencia sin entrada / entrada sin diferencia) y por clase
  (§C.1 y §C.2), tal como ADR-008 §5 lo hace fallar.
- **El registro del escape-hatch**: las excepciones con sus cinco criterios, o la decisión negativa documentada.
- **La constancia del paso siguiente** obligatorio donde corresponda.

**Files created:**
- `2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs` — los casos de cobertura cruzada en los dos sentidos y por clase (superficie contra el reporte del gate; comportamiento/deprecación contra las decisiones), más la validación del **esquema** de los fragmentos

> **Nota de arnés:** la prueba de cobertura **precedente** (`ApiRemovalCoverageTests.cs`, declarada por los Bolts
> 006/007) se **conserva**: este Bolt no la reemplaza —generaliza su criterio y agrega la validación de esquema y
> el cruce de las entradas no atributivas—. Si al ejecutar se decide consolidar ambas en un solo archivo, se
> registra en el MEM (es una decisión de forma, no de criterio).

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: El registro existe, estructurado por área, con los tres datos

**Given** el documento de notas y los fragmentos presentes
**When** se recorren sus entradas
**Then** el registro está **estructurado por área** (§A.1) y **cada** entrada tiene los **tres datos
obligatorios** (qué desaparece con su firma o su comportamiento; por qué, citando la decisión y su cláusula; con
qué se reemplaza, o `"none"` con su razón) más el **ítem/Bolt** que la ejecuta y la evidencia de su verificación

### AC-2: La contribución es distribuida y **no** depende de este Bolt

**Given** un Bolt que rompe superficie y ejecuta su gate
**When** entrega su fragmento
**Then** su ruptura queda **cubierta** por su propia entrada, sin que estas notas sean prerequisito ni condición
de su gate — y el registro muestra **al menos una** entrada llegada desde otro Bolt

### AC-3: El formato está publicado y es validable

**Given** un Bolt que necesita registrar una ruptura
**When** consulta cómo hacerlo
**Then** encuentra el **formato publicado** (`README.md`) con su **esquema** (`api-removals.schema.json`) y un
**validador** que rechaza un fragmento mal formado o incompleto (falta de alguno de los tres datos, clase
desconocida, `"none"` sin razón), de modo que no tiene que adivinar la forma

### AC-4: La cobertura de superficie cierra en los dos sentidos

**Given** el reporte del gate de compatibilidad (línea base 1.4 ↔ superficie 2.0) y los fragmentos
**When** se corre el cruce
**Then** **toda** diferencia de tipo «miembro retirado»/«atributo retirado» de los ensamblados en alcance está
cubierta por alguna entrada, y **toda** entrada de clase `api_removal` corresponde a una diferencia real:
**0** rupturas sin entrada y **0** entradas sin ruptura

### AC-5: Las entradas de comportamiento y de deprecación se cruzan contra sus decisiones

**Given** las decisiones del ciclo que declaran cambios de comportamiento o deprecaciones (el mapa de §A.1)
**When** se corre el cruce de las entradas no atributivas
**Then** cada decisión con declaración tiene **al menos una** entrada en su área, cada entrada cita **la cláusula**
que la ordena, y **ninguna** entrada corresponde a un cambio que ninguna decisión ordena

### AC-6: El escape-hatch queda registrado (o su decisión negativa documentada)

**Given** la superficie del framework
**When** se revisa el registro del escape-hatch de ADR-008 §3.6
**Then** o bien están las excepciones con sus **cinco criterios** verificados y su **vencimiento**, o bien está la
**decisión negativa** documentada con su constancia — sin ambigüedad entre "no hay excepciones" y "no se revisó"

### AC-7: El paso siguiente obligatorio está señalado

**Given** una ruptura que exige más que un cambio de código (regenerar artefactos, migrar valores, cambiar de
runtime)
**When** se lee su entrada
**Then** declara el **paso siguiente obligatorio** del consumidor con su referencia, y **no** sugiere un mecanismo
que el framework ya no sostiene (nada de compatibilidad binaria ni de serialización prometida)

### AC-8: Las notas no autorizan rupturas y el gate sigue siendo el que decide

**Given** el documento de notas y su encabezado
**When** se lee qué es y qué no es
**Then** declara que su fuente son los **fragmentos**, que el mantenimiento es de **contribución distribuida** y
que **no autoriza** ninguna ruptura: el cruce del gate **falla** ante una diferencia no cubierta y una nota no
habilita nada por sí sola (ADR-008 §5)

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — registro estructurado por área con una entrada por ruptura y por deprecación, con los tres datos | §6/A.1 | AC-1 · `MIGRATION-NOTES.md` + validación de esquema |
| Bolt §2 — el registro es acumulativo y de contribución distribuida; no es propiedad exclusiva de este Bolt ni prerequisito | §6/B.1 | AC-2 · fragmento de otro Bolt en el registro + gate de un Bolt sin depender de las notas |
| Bolt §2 — el cruce encuentra todas las diferencias cubiertas y ninguna entrada sin ruptura | §6/C.1 | AC-4 · `MigrationNotesCoverageTests` (dos sentidos) |
| Bolt §2 — las entradas pueden llegar desde otro Bolt sin depender de éste | §6/B.1, §6/E | AC-2 · evidencia del mecanismo ejercido |
| Bolt §2 — el registro de las excepciones del escape-hatch | §6/D.1 | AC-6 · sección del registro |
| Bolt §2 — el señalamiento del paso siguiente obligatorio | §6/D.2 | AC-7 · entradas que lo declaran |
| ADR-008 §3.1 — los tres datos obligatorios y el ítem que ejecuta | §6/A.1, §6/A.2 | AC-1, AC-3 · esquema + validador |
| ADR-008 §5 — el NFR de trazabilidad y el gate (100 % cubierto, 0 en cada sentido) | §6/C.1 | AC-4 · el cruce |
| ADR-008 §3.5/§3.6 — la línea 2.0.x y el escape-hatch | §6/C.2, §6/D.1 | AC-5, AC-6 |
| ADR-001 §3.2 y §4 — el camino de migración y las rupturas que deben aparecer | §6/A.1, §6/D.2 | AC-1, AC-7 |
| ADR-002 §3.1/§3.3 · ADR-004 §3.1/§3.4 · ADR-007 §3.1/§3.3 — las entradas no atributivas | §6/C.2 | AC-5 · el mapa de áreas y su cruce |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Validación de esquema (por fragmento):** cada `2.0/docs/api-removals/*.json` valida contra
  `api-removals.schema.json`; un fragmento **mal formado** o **incompleto** (falta uno de los tres datos, clase
  desconocida, `api_removal` sin firma, `"none"` sin razón, sin Bolt responsable o sin evidencia) **falla** con su
  motivo. Se prueban además los casos de borde del esquema (entrada vacía, campos extraños, tipos incorrectos).
- **Cruce de superficie en los dos sentidos (con casos de prueba):** sobre el reporte del gate y un conjunto de
  fragmentos de fixture —uno **completo** (verde), uno con una **diferencia sin entrada** (rojo, nombrando el
  miembro) y uno con una **entrada sin diferencia** (rojo, nombrando la entrada)—: la prueba demuestra los dos
  sentidos de ADR-008 §5 sin depender de que el árbol real esté completo.
- **Cruce de las entradas no atributivas (contra las decisiones):** el mapa de §A.1 se traduce a una lista
  verificable (decisión → área → al menos una entrada) y la prueba falla si una decisión con declaración queda sin
  entrada y si una entrada cita una decisión que no declara ese cambio.
- **El mecanismo de contribución:** una prueba que **lee todos** los fragmentos (no sólo el propio) y verifica que
  una entrada aportada por otro Bolt queda cubierta — es la forma de ejercitar el mecanismo (hoy con los
  fragmentos de 006/007; con cualquier otro que exista al ejecutar).
- **La estructura y el encabezado del documento:** la consolidación produce el documento **por área** (cada
  entrada bajo su área, con su decisión y su Bolt) y su encabezado declara fuente, mantenimiento y el "no
  autoriza"; dos consolidaciones del mismo conjunto de fragmentos producen el **mismo** contenido (orden
  canónico).
- **El registro del escape-hatch:** la sección existe y, si no hay excepciones, la **decisión negativa** está
  documentada con su constancia; si hay una, sus **cinco criterios** están listados y verificables.
- **Consumer-side contract tests (test doubles):** `n/a` — el contrato que se consume es el **reporte del gate**
  (un artefacto, no un servicio): se ejercita sobre el reporte real del arnés y, para los casos negativos, sobre
  reportes de fixture construidos a mano. No hay componente externo que doblar.
- **Edge cases:** fragmento de un Bolt **sin** rupturas (archivo con lista vacía: válido y sin entradas); dos
  fragmentos que cubren **el mismo** miembro (se acepta y se reporta como duplicación a revisar — no falla, pero se
  señala); una entrada de clase `behavior_change` que **también** es una diferencia de API (se exige que declare
  ambas clases o que la diferencia esté cubierta por una entrada `api_removal`); un fragmento cuyo Bolt **no**
  existe en el registro de Bolts (se señala); un miembro retirado en **dos** ensamblados (dos entradas o una con
  alcance múltiple, con su convención declarada); una decisión del mapa **sin** declaración de ruptura (§A.1) que
  queda sin entradas (no falla: la lista de decisiones con declaración se deriva de los ADRs, no se asume).
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida del validador de esquema (verde y con sus casos negativos), el
  cruce de superficie en los dos sentidos (verde y con sus dos casos rojos), el cruce de las entradas no
  atributivas, la constancia del fragmento aportado por otro Bolt, el documento consolidado con su encabezado y la
  sección del escape-hatch. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `debt` (documentación y trazabilidad): el cruce se
  hace sobre el **reporte del gate** y los **fragmentos** del repositorio, en el nivel hermético, con la suite del
  arnés. No hay base de datos, cola, servicio, almacenamiento ni entorno desplegado involucrado.
- **Contract / E2E against the real providers:** `n/a` — no hay proveedor real: el "contrato" es el reporte del
  gate (artefacto) y la estructura de los fragmentos (archivos del repositorio).
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca con
  dependencias reales.
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); el esquema de los fragmentos es JSON válido y el documento de notas se produce | pass |
| Unit / component tests | V-Bounce | 100 % verdes: validador de esquema con sus casos negativos, cruce de superficie en los dos sentidos (con sus dos casos rojos), cruce no atributivo, mecanismo de contribución y estructura del documento | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — el contrato consumido es un **artefacto** (el reporte del gate), no un servicio; se ejercita sobre el reporte real y sobre reportes de fixture para los casos negativos | n/a (razón) |
| SAST | V-Bounce | `n/a` — el entregable es documentación, un esquema y casos de prueba; no hay código de producto nuevo que analizar | n/a (razón) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas (el validador y los casos usan el framework de pruebas del arnés) | pass |
| OWASP Top 10 — static review | V-Bounce | `n/a` — no hay superficie ejecutable ni entrada externa: son documentos del repositorio y casos de prueba | n/a (razón) |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** secretos: las notas describen **procedimientos** (por ejemplo la migración de valores) y **nunca** incluyen claves, credenciales ni material derivado | pass |
| Hallucination lint | V-Bounce | **Este gate es el corazón del Bolt:** cada entrada cita la **decisión** que la ordena (con su cláusula) y el **Bolt** que la ejecuta; **0** rupturas inventadas y **0** reemplazos inventados (un reemplazo que el framework no ofrece **no** se insinúa: se declara "sin reemplazo" con su razón) | pass |
| IP / license provenance | V-Bounce | Documentación propia del repositorio; 0 contenido de terceros | pass |
| PII / DLP | V-Bounce | `internal`: las notas no contienen datos personales ni de negocio (hablan de tipos, miembros y comportamientos) | pass |
| Dependency-confusion | V-Bounce | `n/a` — no se agregan paquetes | n/a (razón) |
| Test-first evidence | V-Bounce | El **mapa de áreas** (§A.1) y el **criterio de cobertura** en los dos sentidos se escriben **antes** de consolidar, y el validador de esquema se escribe antes de publicar el formato (el formato debe ser válido por construcción, no por costumbre) | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos consolidaciones del mismo conjunto de fragmentos producen el **mismo** documento (orden canónico) y el cruce es determinista | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: sin proveedor real en el alcance | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: sin camino de ejecución medible | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: el entregable es documentación y casos de prueba (§11) | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). El registro describe **tipos, miembros y
  comportamientos** del framework y su reemplazo en el código del consumidor; no contiene datos personales ni de
  negocio.
- **Sin secretos en las notas:** las entradas de la **criptografía** describen el **procedimiento** de migración
  (clasificar, validar, re-proteger, respaldar, registrar) y **nunca** material de claves, credenciales ni valores
  (ADR-007 §3.2/§5 y ADR-009 §3.8). El gate de secretos aplica sobre el repositorio que incluye estas notas.
- **Las notas no son una autorización:** una entrada **documenta** una ruptura **ya decidida** por su ADR; no la
  crea, no la autoriza y no reemplaza el gate (ADR-008 §5: el cruce **falla** ante una diferencia no cubierta, y
  una nota no habilita nada por sí sola). Esto se declara en el encabezado del documento (AC-8) precisamente para
  que nadie lo lea al revés.
- **Sin promesas inseguras:** el registro **no** promete compatibilidad binaria ni un mecanismo de serialización
  que el runtime objetivo no sostiene (ADR-008 §3.3): cuando no hay reemplazo, la entrada dice **por qué** el
  escenario no existe —que es también la forma de evitar que un consumidor invente un shim.
- **Trazabilidad como control:** cada entrada es trazable a una **decisión** y a un **Bolt con su evidencia**, lo
  que hace que una ruptura no pueda ser "anónima" ni quedar sin dueño.
- **Datos del registro:** archivos JSON y Markdown del repositorio; ningún dato en reposo fuera del control de
  versiones.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es documentación, un esquema y casos de prueba; no hay
  servicio ni proceso que observar.
- **La observabilidad de este Bolt es su evidencia:** la corrida del **validador de esquema** (verde y con sus
  negativos), el **cruce en los dos sentidos** por clase (con sus dos casos rojos demostrados sobre fixtures), la
  constancia del fragmento **aportado por otro Bolt**, el documento consolidado con su encabezado y la sección del
  **escape-hatch**.
- **La observabilidad del proceso (lo que el gate mide):** el resultado del cruce es, en sí, el indicador de salud
  del registro: **0** diferencias sin entrada y **0** entradas sin diferencia es el verde del NFR de ADR-008 §5, y
  cualquier fallo nombra el miembro o la entrada responsable.
- **Diagnóstico para quien migra:** el documento se lee por área (no por fecha de Bolt), y cada entrada dice qué
  hacer; el **paso siguiente obligatorio** (por ejemplo regenerar en lugar de convertir) está señalado en la
  entrada, que es donde el consumidor lo va a buscar.

---

## 12. Migration, compatibility and rollback

- **Migration:** **este Bolt documenta** la migración del consumidor y no migra nada por sí mismo. Su aporte al
  camino de ADR-001 §3.2 es hacerlo **entendible**: qué cambia, por qué y con qué se reemplaza, área por área, más
  el **paso siguiente obligatorio** cuando la ruptura exige más que un cambio de versión (regenerar artefactos,
  migrar valores, cambiar de runtime).
- **Compatibility:** las notas **no** prometen compatibilidad; declaran lo contrario cuando corresponde ("sin
  reemplazo, porque el escenario no existe en el runtime objetivo", ADR-008 §3.3) y sostienen la contención
  soportada: el **1.4 congelado** para quien no pueda migrar (ADR-008 §3.1). La línea 2.0.x evoluciona con SemVer y
  las deprecaciones se **avisan** antes del retiro (§3.5), y la entrada de deprecación declara esa ventana.
- **Crecimiento del registro:** el registro es **acumulativo**: cada Bolt que rompa superficie agrega su fragmento
  y la consolidación se extiende. Ese crecimiento **no** es una revisión material de este SPEC (es su resultado
  natural); sí lo sería un cambio de **criterio** de cobertura, de **formato** o del **mapa de áreas**.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido ni efecto en artefactos desplegados: el
  entregable son documentos del repositorio y casos de prueba. Si un fragmento de otro Bolt ya fue comiteado, la
  reversión de **este** Bolt no lo afecta (y no debería: la obligación de registrar vive en el Bolt que rompe,
  ADR-008 §3.1).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Que el gate falle porque las notas todavía no existen** (el riesgo que motivó la resolución del par revisor) | 3 | 4 | Contribución **distribuida** verificable: la ruptura se cubre con el fragmento de **su** Bolt y este Bolt **no** es prerequisito; la verificación es **por diferencia**, no por documento (AC-2) |
| **Rupturas sin entrada** (un miembro retirado que nadie documentó) | 3 | 4 | Cruce contra el **reporte del gate** con **0** diferencias sin cubrir (AC-4), sobre **todos** los fragmentos — no sólo el del Bolt que escribe la prueba |
| **Entradas inventadas** (documentar retiros que no ocurrieron o reemplazos que el framework no ofrece) | 3 | 3 | Cruce en el **otro sentido** (0 entradas sin diferencia) + gate de *hallucination lint* con la regla de que un reemplazo inexistente se declara "sin reemplazo" **con su razón** |
| **El registro envejece y deja de cubrir lo nuevo** | 3 | 3 | La obligación vive en el Bolt que rompe (ADR-008 §3.1) y este Bolt la deja **operativa** con su evidencia de que se ejerce (un fragmento de otro Bolt dentro del registro) |
| **Documento que promete un camino que nadie sostiene** (insinuar compatibilidad binaria o un serializador inexistente) | 2 | 4 | Regla explícita (AC-7): la entrada "sin reemplazo" dice **por qué**; el gate detecta el caso en el cruce |
| **Lectura peor por acumulación heterogénea** (deuda declarada por ADR-008 §4) | 4 | 2 | La **estructura por área** es criterio de la entrega (§A.1), con el mapa área ↔ decisión; la consolidación es determinista |
| **Las notas se leen como autorización de rupturas** | 3 | 3 | El encabezado lo declara (AC-8) y el gate sigue siendo el que decide: una diferencia sin nota **falla** y una nota no habilita nada (ADR-008 §5) |
| **El formato del fragmento deriva** (cada Bolt inventa su forma y el validador deja de servir) | 3 | 3 | Formato **publicado** con esquema y validador, adoptado de la convención ya usada por 006/007 (no una segunda forma); el validador rechaza lo incompleto (AC-3) |
| **Las entradas no atributivas quedan sin criterio de cobertura** (no se pueden cruzar contra el gate) | 3 | 3 | Se cruzan contra las **decisiones** que las ordenan, con el mapa de §A.1 como lista verificable (AC-5): una decisión con declaración sin entrada **falla** |
| **Duplicación o solapamiento entre fragmentos** | 3 | 2 | Se **acepta y se señala** en el cruce (no falla): la duplicación es información para el revisor; el criterio de fallo es la falta de cobertura o la invención |
| **Alguien pretende que este Bolt sea el dueño exclusivo del registro** | 2 | 4 | Está **excluido** (§4) y declarado como riesgo: haría fallar el gate de los Bolts que rompen superficie antes de que las notas existan. La salida es la contribución distribuida, no un cambio de propiedad |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-008/ADR-001/ADR-002/ADR-004/
> ADR-007; acá se aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) El registro se consolida en `2.0/docs/MIGRATION-NOTES.md` y los fragmentos viven en `2.0/docs/api-removals/<BOLT>.json` (la convención ya usada por 006/007)** | Los fragmentos ya tienen **convención fijada** por dos Bolts (y otros SPECs declaran aportar su lista): inventar otra ruta o otro formato crearía dos registros que se desincronizan. El documento consolidado no tenía ruta fijada por ninguna fuente y este SPEC la fija junto a los fragmentos, que es donde el consumidor la va a buscar |
| **(⚠ aprobador) Hay dos clases de entrada con validaciones distintas: `api_removal` (contra el gate, en los dos sentidos) y `behavior_change`/`deprecation` (contra las decisiones, con el mapa de áreas)** | El gate compara **API pública**: sólo las entradas atributivas se pueden cruzar contra él. Las otras (el caché que expira, `App.config` → `appsettings.json`, lo que el modelo deja de transportar, la migración de valores) **no aparecen** en el reporte del gate y quedarían sin verificación. Validarlas contra la decisión que las ordena es lo que las hace trazables sin fingir que el gate las cubre |
| **(⚠ aprobador) La validación vive en la suite del arnés (`MigrationNotesCoverageTests`), no en una herramienta nueva** | El gate y su línea base son del Bolt 001 (ADR-008 §5) y la prueba de cobertura precedente ya vive ahí (006/007); una herramienta propia sería una segunda fuente de verdad sobre **el mismo** cruce. La nota de §E deja explícito que la prueba precedente **se conserva** |
| Las entradas citan **la cláusula** de la decisión (no sólo el ADR) | La trazabilidad que exige ADR-008 §3.1 es "la decisión que lo ordena": sin la cláusula, un ADR largo no dice **qué** parte ordena la ruptura y el revisor tiene que buscarla |
| La clase `deprecation` declara su **ventana de aviso** | ADR-008 §3.5 exige al menos una minor de aviso antes del retiro de un miembro; si la entrada no lo declara, el dato se pierde cuando alguien consuma las notas fuera del ciclo |
| El **paso siguiente obligatorio** se declara dentro de la entrada | Es lo que hace que las notas sirvan para **migrar** (y no sólo para explicar): el caso paradigmático es "regenerar con el motor" en lugar de convertir código (ADR-002 §3.3) |
| La **duplicación** entre fragmentos se señala y **no** falla | Dos Bolts pueden tocar el mismo miembro (por ejemplo uno lo retira y otro lo mueve): el criterio de fallo es la **falta de cobertura** o la **invención**, no el solapamiento, que es información para el revisor |
| El documento declara explícitamente que **no autoriza** rupturas | Es el malentendido más probable de un registro de rupturas (leerlo como permiso). El gate es el que decide (ADR-008 §5) y el encabezado lo dice |
| No se define un formato nuevo de "notas narrativas" separado del registro | El Bolt pide **un** registro estructurado por área; un documento paralelo narrativo se desincronizaría del registro y volvería a la deuda declarada por ADR-008 §4 |
| La consolidación es **determinista** (orden canónico) | El registro se versiona y se revisa por diff: sin orden canónico, cada consolidación produciría un diff enorme y la revisión del cambio real se perdería (mismo criterio que el reporte del motor, ADR-003 §3.3) |

---

## 15. Stop conditions

- **El gate de compatibilidad y su línea base (Bolt 001) no están** en el árbol 2.0: parar y reportar. Sin el
  reporte de diferencias, el cruce en los dos sentidos no tiene contra qué verificar y la trazabilidad quedaría
  **declarativa** — que es exactamente lo que este Bolt debe evitar.
- **Se pretende que este Bolt sea el dueño exclusivo del registro** (o que su entrega sea prerequisito del gate de
  otro Bolt): parar y reportar. Haría fallar el gate de los Bolts que rompen superficie **antes** de que las notas
  existan; la salida es la contribución distribuida (exclusión explícita del Bolt).
- **Una ruptura no se puede describir con sus tres datos** (no hay reemplazo que describir **ni** razón por la que
  el escenario no existe): parar y reportar; la respuesta es del **Bolt que la ejecuta** (y, si el caso es nuevo,
  de un ADR), nunca una invención en las notas.
- **Se pretende documentar una ruptura que ninguna decisión ordena**: parar. Si el cambio es real y no tiene
  decisión, falta un ADR; el registro **no** es el lugar donde se autoriza.
- **Se pretende enmendar un ADR desde las notas** o prometer compatibilidad binaria / un serializador inexistente:
  parar. Los ADRs son inmutables y ADR-008 §3.3 fija qué reemplaza al estado serializado (con su razón para el
  caso "sin reemplazo").
- **El formato del fragmento cambia de forma incompatible** con los fragmentos ya comiteados (por ejemplo cambiar
  el nombre de los campos o el significado de las clases): parar y reportar; el cambio de formato es una revisión
  de este SPEC y exige migrar los fragmentos existentes en el mismo acto.
- **Aparece una ruptura sin entrada y el Bolt que la ejecutó ya está cerrado** (o no existe): parar y reportar; se
  rutea al Bolt/ADR correspondiente, no se documenta "desde acá" como si fuera propio.
- **Se pretende que las notas incluyan material sensible** (claves, credenciales, valores de ejemplo reales):
  parar. Las entradas describen procedimientos y miembros, nunca material (ADR-007 §3.2/§5).
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado por
  `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-008 §3.1/§3.3/§3.5/§3.6/§5, ADR-001 §3.2/§4, ADR-002 §3.1/§3.3, ADR-004 §3.1/§3.4, ADR-007 §3.1/§3.3)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (los tres datos obligatorios de cada ruptura y el procedimiento de documentación de la 2.0), §3.3 (qué reemplaza al estado serializado; el caso "sin reemplazo" con su razón), §3.5 (SemVer y las deprecaciones avisadas), §3.6 (el escape-hatch con sus cinco criterios), §4 (la deuda de legibilidad del registro), §5 (el NFR de trazabilidad de rupturas y **el gate**, con su dueño).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.2 (el camino de migración del consumidor y la no-coexistencia lado a lado) y §4 (las rupturas que **deben** aparecer en las notas).
- [`ADR-002`](../adrs/ADR-002-formato-coop2-y-migrador.md) — §3.1 (lo que el modelo deja de transportar) y §3.3 (las deprecaciones de alcance del corpus legacy y su paso siguiente obligatorio).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.1 (los cambios de comportamiento del caché) y §3.4 (`App.config` → `appsettings.json` + entorno).
- [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (el retiro de la superficie estática de criptografía) y §3.3 (la migración de valores como ruptura con reemplazo procedimental).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — el gate, el reporte de compatibilidad y la línea base 1.4 (prerrequisito, Bolt 001).
- [`SPEC-261016-0012`](./SPEC-261016-0012-retiro-del-export-excel.md) · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) — la convención del fragmento `2.0/docs/api-removals/<BOLT>.json` y la prueba de cobertura precedente (Bolts 006/007).
- [`US-000.BOLT-003`](../functional/bolts/US-000.BOLT-003-particion-modeling.md) · [`US-000.BOLT-009`](../functional/bolts/US-000.BOLT-009-library-portable.md) · [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) · [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) · [`US-000.BOLT-027`](../functional/bolts/US-000.BOLT-027-empaquetado-y-asociacion.md) — los Bolts que crean o rompen superficie y aportan sus fragmentos (la contribución distribuida que este Bolt estructura y valida).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-028`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, estructura por área con su mapa de decisiones, **formato publicado** del fragmento (esquema + validador) adoptando la convención de 006/007, mecanismo de **contribución distribuida** y su prueba, **cobertura cruzada en dos sentidos** para las rupturas de superficie y **cruce contra las decisiones** para las entradas de comportamiento/deprecación, registro del escape-hatch y del paso siguiente obligatorio, y la consolidación determinista del documento de notas. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (arquitectura / documentación de la API pública)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
