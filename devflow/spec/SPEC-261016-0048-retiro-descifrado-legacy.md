---
id: "SPEC-261016-0048"
title: "Retiro de la capacidad de descifrado legacy y cierre del registro de vencimiento"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 12 (hallazgo F-23) → ADR-007 §3.3/§3.4
bolt: "US-000.BOLT-030" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-007", "ADR-008", "ADR-004"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0039-comandos-de-valores-protegidos.md" # los comandos de valores protegidos con la capacidad legacy aislada, su bandera y su reporte (Bolt 023)
  - "SPEC-261016-0050-batch-real-del-corpus.md" # el corpus de modelos migrado y sus originales conservados: condición de cierre del lado del modelo (Bolt 029)
  - "SPEC-261016-0025-cli-del-motor.md" # el proyecto del CLI donde viven los comandos de valores protegidos (Bolt 018)
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y build (Bolt 001)
risk_class: "high" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L2" # default por riesgo high (§3.3): más paradas y confirmaciones durante el V-Bounce
turn_budget: "" # vacío — se usa el default de la plataforma/agente (§3.3)
data_classification: "confidential" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: required` → `applicable` (§3.2.2)
  applicability: "applicable"
  tests: # escritas en este V-Bounce; se ejecutan en el ciclo de integración del Bolt
    - "2.0/tests/Cooperator.Cli.Secrets.Integration.Tests/LegacyFormatNoLongerDecryptsTests.cs (Category=Integration) — corrida de los comandos de valores protegidos **después** del retiro sobre un valor del formato viejo tomado del inventario real: falla con **error explícito y accionable** que nombra la situación (nunca degradación silenciosa ni heurística)"
    - "2.0/tests/Cooperator.Cli.Secrets.Integration.Tests/CurrentSchemesStillWorkTests.cs (Category=Integration) — los comandos siguen funcionando para los esquemas vigentes (`plain:`, `aesgcm:`) sobre las instalaciones reales: inspeccionar, migrar y re-proteger siguen operativos"
    - "2.0/tests/Cooperator.Cli.Secrets.Integration.Tests/InventoryClosureTests.cs (Category=Integration) — la **condición de cierre** verificada contra el inventario real declarado por su dueño: 100 % de los valores clasificados como candidatos legacy convertidos a `aesgcm:` o declarados **no migrables con su motivo** (lectura de **sólo lectura**, sin extraer valores)"
    - "2.0/tests/Cooperator.Cli.Secrets.Integration.Tests/NoEmbeddedSecretsOnArtifactsTests.cs (Category=Integration) — sobre los artefactos construidos y publicados de la herramienta: la clave y las primitivas del formato viejo **no** reaparecen en ninguna forma (NFR `no-embedded-secrets` de ADR-007 §5)"
  environment: "el entorno de las **instalaciones reales** donde viven los valores (Windows, con el inventario por cliente y su almacén) — **aún no provisionado**; el lane de integración con acceso de **sólo lectura** al inventario que su dueño declara."
  data: "el **inventario migrado** declarado por su dueño (ADR-007 §3.3), consultado **en sólo lectura**: la corrida verifica el **estado** de cada valor (migrado a `aesgcm:` o no migrable con su motivo) y **nunca** extrae, copia ni registra valores ni material de claves. Clase `confidential`: nada del inventario cruza el límite del prompt (§3.6)."
  reason: ""
  waiver_adr: ""
review_ready_at: "2026-10-16T00:53:53-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T00:53:53-03:00"
  decided_at: "2026-10-16T00:53:53-03:00"
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-030 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent, 00:12:35; acknowledgment «La salida legitima puede ser postergar…; required confirmado (cierre del registro contra inventario real)»), ADR-007/ADR-008/ADR-004 accepted, REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; risk_class high → autonomy_level L2; data_classification confidential; test_integration required → applicability applicable. Ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: verificación de la condición de cierre contra el inventario real del dueño (evidencia de ESTADOS, nunca valores) → decisión retirar o postergar; retiro del componente/bandera/supresión por miembro sin huellas (proyecto+solución incluidos); fallo tipado y accionable ante el formato viejo (nunca heurística ni degradación); huellas en 0 (NFR no-embedded-secrets); cierre del registro o postergación con fecha concreta (no «cuando se pueda»)."
    - "ADJUDICACIÓN (a) dos desenlaces legítimos y retirar con condición incompleta PROHIBIDO — CONFIRMADA: §2 (tabla), §6/A.3, §14 y §15 #1 tratan la postergación como resultado del Bolt (no fracaso) y declaran que retirar con la condición incumplida es «la única acción prohibida de forma absoluta». Un valor pendiente ⇒ postergar, nunca retirar (§8.1 edge case)."
    - "ADJUDICACIÓN (b) el cierre es acto humano con el registro delante — CONFIRMADA: §D.1/§14/§19 nota 2 declaran que el agente aporta la evidencia y el registro pero NO se autocierra; la firma del cierre (y del MEM) corresponde al actor; con registro incompleto (sin dueño ni condición escrita) el Bolt NO cierra y escala (§15 #2)."
    - "ADJUDICACIÓN (c) contexto FIPS (reporte accionable) — CONFIRMADA: §8.2/§13/§19 nota 5 declaran que si las primitivas legacy lanzan bajo FIPS, la herramienta lo reporta con mensaje accionable y nunca falla en silencio ni reporta «sin candidatos» (ADR-007 §3.5); tras el retiro, el escenario desaparece del producto porque las primitivas ya no están."
    - "ADJUDICACIÓN (d) fronteras 023/029/022 — CONFIRMADAS: §4 (out of scope) y §14 recortan: la capacidad interna es del Bolt 023 (acá se le RETIRA, no se reimplementa); el corpus de modelos y sus originales son del Bolt 029 (sólo aportan la condición de cierre del lado del modelo); el resolutor y la provisión de claves del Bolt 022 NO se tocan. Sin solapamiento."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). El ámbito, la bandera, la supresión por miembro y —sobre todo— el **retiro** de la
  excepción legacy están decididos en ADR-007 §3.3/§3.4; el paralelo del retiro de una excepción acotada y la
  entrada en las notas están en ADR-008 §3.4/§3.1. Este SPEC ejecuta el retiro y el cierre del registro.
  ⚠️ IRREVERSIBILIDAD: retirar la única pieza que lee el formato viejo **antes** de cumplir su condición de cierre
  deja valores ilegibles para el cliente. Si la condición **no** se cumple, la salida legítima es **postergar con
  motivo y nueva fecha** — nunca retirar igual. El cierre es un acto humano con el registro delante.
  ⚠️ DATOS `confidential` (§3.6): el inventario describe valores protegidos. La verificación lee su **estado** y
  **nunca** su contenido; nada del inventario cruza el límite del prompt ni entra en un artefacto de evidencia.
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de dominio
  aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada invalida la
  aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0048 — Retiro de la capacidad de descifrado legacy y cierre del registro de vencimiento

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 12 (hallazgo F-23) |
| **Bolt** | [US-000.BOLT-030](../functional/bolts/US-000.BOLT-030-retiro-descifrado-legacy.md) |
| **ADRs** | [ADR-007](../adrs/ADR-007-politica-criptografica.md) §3.3, §3.4, §3.5, §5 · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) §3.1, §3.4 · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) §3.3 |
| **Risk Class** | high |
| **Revision** | 1 |

---

## 1. Objective

Se entrega el **retiro de la única pieza del árbol autorizada a leer el formato de protección de 1.4** —su
excepción, su **bandera** explícita y su **supresión de advertencias por miembro**— y el **cierre del registro de
vencimiento** que la mantenía viva con dueño, condición de cierre y fecha de revisita.

El problema que resuelve está verificado: `REV-001` F-23 documenta que 1.4 "cifraba" con **TripleDES + clave
derivada por MD5 y encoding ASCII** —una clave que está **en el binario 1.4**, o sea pública— y que la capacidad
de leer ese formato es una **excepción transitoria** de la política criptográfica (ADR-007 §3.4), admitida **sólo**
mientras el corpus esté migrado. Una excepción sin vencimiento se vuelve permanente por olvido, y el olvido aquí
significa: una herramienta del producto conserva las primitivas prohibidas (3DES, MD5) y su supresión de
advertencias **para siempre**. Si este Bolt no se ejecuta, el producto no cierra su deuda de seguridad ni puede
declarar que el formato viejo murió.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 12 como BUG de seguridad + ADR. ADR-007
(`accepted`) decide: la capacidad legacy **vive sólo** en la herramienta de conversión, en un **componente interno**
aislado del runtime, **detrás de una bandera** que reconoce por escrito que la clave del formato viejo es pública;
su uso se admite bajo **cinco condiciones** (§3.4: ámbito interno, bandera explícita, supresión **por miembro**,
sin generalización a "cifrar", y **retiro** al cerrar §3.3); y su **vencimiento se registra** (§3.3) con
**dueño** (el responsable del inventario de migración por cliente), **condición de cierre** (100 % de los valores
clasificados como candidatos legacy convertidos a `aesgcm:` **o** declarados no migrables con su motivo) y **fecha
de revisita obligatoria** (una fecha concreta). Este SPEC ejecuta el retiro **y** el cierre; no reabre la política.

**El estado del registro es el que manda.** La decisión de este Bolt es **irreversible** para el cliente si se toma
con la condición sin cumplir: una vez retirada la única pieza que lee el formato viejo, los valores que quedaron en
ese formato **no se pueden recuperar**. Por eso hay **dos desenlaces legítimos** y el SPEC los trata a los dos:

| Desenlace | Cuándo | Qué se entrega |
|---|---|---|
| **Retiro + cierre** | La condición de cierre está **cumplida** y evidenciada contra el inventario real | El componente, la bandera y la supresión **fuera**; el registro **cerrado** con su estado, su dueño y la fecha |
| **Postergación registrada** | La condición **no** está cumplida (o su evidencia no puede declararse) | **Nada se retira**; el registro queda con la **postergación**: motivo y **nueva fecha de revisita** concreta |

**Lo que este Bolt consume (y no reimplementa):** los **comandos de valores protegidos** con la capacidad legacy
aislada, su bandera y su reporte (Bolt 023, SPEC-0039 — es el Bolt que **contribuye** el registro); el **corpus de
modelos migrado** y sus originales conservados (Bolt 029, SPEC-0050 — la condición de cierre del lado del modelo);
el **resolutor y la provisión de claves** vigentes (Bolt 022, SPEC-0035 — no se tocan).

**Contexto del defecto (evidencia de la política, ADR-007 §3.3/§3.4/§3.5):** el conversor **no adivina** qué está
cifrado (clasifica candidatos por forma y **reporta** antes de tocar nada); los valores con **caracteres no ASCII**
**ya perdieron información** en 1.4 (cifrado con `Encoding.ASCII`) y se marcan **no migrables automáticamente**, con
intervención humana — es la razón por la que "100 % migrado **o** declarado no migrable con motivo" es la
condición, y no un porcentaje optimista. Con **FIPS habilitado**, las primitivas legacy **lanzan**: la herramienta
debe detectarlo y reportarlo, nunca fallar en silencio ni reportar "sin candidatos" cuando en realidad no pudo
evaluar. Y después de la migración **no queda heurística**: todo valor declara su esquema (`plain:` o `aesgcm:`) y
el mecanismo de detección por substrings del 1.4 está eliminado (ADR-004 §3.3).

**Estado del árbol.** El CLI (Bolt 018), los comandos de valores protegidos (Bolt 023) y el batch del corpus
(Bolt 029) tienen SPEC en disco. Este Bolt escribe **sólo** su SPEC y, en su V-Bounce, el retiro y el cierre del
registro.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-007 §3.4 exige el retiro con **el mismo estándar** que la excepción
del `BinaryFormatter` del migrador —ADR-002 §3.2 / ADR-008 §3.4—; ADR-008 §3.1 fija cómo se documenta lo que
desaparece; ADR-004 §3.3 declara que después de la migración no queda heurística, lo que este Bolt hace efectivo),
y el plan de integración declarado (`applicable`) coincide con el `test_integration: required` del Bolt (§3.2.2).
No se emite reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-030` (`devflow/functional/bolts/US-000.BOLT-030-retiro-descifrado-legacy.md`) | AITL-BOLT-READY-Approval ✓ — `approved` (el READY acepta explícitamente que la salida legítima pueda ser **postergar** con motivo y nueva fecha, en lugar de retirar), `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:12:35-03:00; `risk_class: high`, `test_integration: required`, `data_classification: confidential` |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-007` §3.3 (vencimiento **con registro**: dueño, condición de cierre, fecha de revisita; migrar y no re-cifrar; el límite del ASCII), §3.4 (la **excepción única y acotada** y su **retiro** con las mismas condiciones que la del migrador), §3.5 (herramientas, FIPS y reproducibilidad) y §5 (NFRs `no-embedded-secrets`, `protected-value-integrity`) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-008` §3.4 (retiro de una excepción acotada y supresión **por miembro**, nunca a nivel de proyecto o solución) y §3.1 (la entrada del retiro en las notas de migración con qué, por qué y con qué se reemplaza) | AITL-ADR-Approval ✓ — `accepted`, 2026-10-15T23:11:40-03:00 |
| ADR (gobernante) | `ADR-004` §3.3 (después de la migración **no** queda heurística: todo valor declara su esquema) | AITL-ADR-Approval ✓ — `accepted` (2026-10-15) |
| REV evidence | `REV-001` §4 F-23 y §6 ítem 12 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| SPECs previos (prerrequisitos) | `SPEC-261016-0039-…` (comandos con la capacidad legacy aislada — Bolt 023), `SPEC-261016-0050-…` (corpus migrado — Bolt 029), `SPEC-261016-0025-…` (el CLI donde viven los comandos) y `SPEC-261016-0000-…` (arnés/build) | Los cuatro **en disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otros V-Bounces; **este** Bolt no lo toca) |

---

## 4. Scope

### In scope

1. **La verificación de la condición de cierre** del registro, con su evidencia contra el **inventario real**
   declarado por su **dueño**: 100 % de los valores clasificados como **candidatos legacy** convertidos a
   `aesgcm:` **o** declarados **no migrables con su motivo**.
2. **Los dos desenlaces legítimos**, ejecutados como tales: el **retiro con cierre** (condición cumplida y
   evidenciada) o la **postergación registrada** con **motivo** y **nueva fecha de revisita** concreta (condición no
   cumplida o no declarable). La postergación **no** es un fracaso del Bolt: es su resultado posible y queda
   registrada, nunca omitida.
3. **El retiro del componente interno de descifrado legacy**, de la **opción que lo habilitaba** y de la
   **supresión de advertencias por miembro** que la excepción exigía — sin supresiones residuales a nivel de
   proyecto ni de solución.
4. **El comportamiento explícito** ante un valor del formato viejo que llegue a los comandos **después** del
   retiro: error **tipado y accionable** que nombra la situación — nunca degradación silenciosa, nunca un intento
   de descifrado "a ver qué pasa", nunca una heurística.
5. **La verificación de huellas**: la **clave** y las **primitivas** del formato viejo (3DES, MD5, la derivación, el
   IV fijo) **no** reaparecen en ninguna forma en el árbol 2.0 ni en artefactos versionados ni en los artefactos
   construidos de la herramienta (NFR `no-embedded-secrets`).
6. **El cierre del registro de vencimiento** —con su estado, su dueño y la fecha del cierre— o su **postergación
   registrada**, y su reflejo donde el producto lo declara: la **documentación de la herramienta** y las **notas de
   migración**.
7. **La confirmación de que los comandos siguen operativos** para los esquemas **vigentes** (`plain:`, `aesgcm:`):
   inspeccionar, migrar y re-proteger no dependen de la capacidad retirada.
8. **La constancia** del retiro con los tres datos de ADR-008 §3.1 (qué desaparece, por qué, con qué se reemplaza).

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **defecto de cifrado** del runtime y su **BUG** | Su BUG y su estándar (ADR-007 §3.6) | El Bolt lo excluye: acá se retira la capacidad **de leer** el formato viejo, no se corrige el defecto de 1.4 |
| El **resolutor** y la **provisión de claves** (`aesgcm:`, `kid`, rotación, hooks) | Bolt 022 (SPEC-0035) | Otra superficie; no se toca |
| Los comandos en sí: **inspección, migración y rotación** | Bolt 023 (SPEC-0039) | Acá se les **retira** la capacidad legacy y se cierra el registro; su funcionalidad vigente se conserva |
| El **batch del corpus de modelos** y el archivo de sus originales | Bolt 029 (SPEC-0050) | Otro entregable: el corpus migrado es la **condición de cierre del lado del modelo**, no la ejecución de este Bolt |
| El **catálogo de claves** y la configuración | Bolt 011 / ADR-004 | Otras superficies |
| Un **artefacto formal de riesgo** | El dueño, si lo propone | ADR-007 §3.3 exige el **registro**, no crea el artefacto |
| Reintroducir cualquier **control de integridad** en código o reabrir el gate retirado | — (nadie) | ADR-001 §3.3/§3.6 / ADR-007 §3.6 |
| **Migrar valores por su cuenta** | El dueño del inventario | El Bolt lo excluye: si al cerrar apareciera un valor sin migrar, la salida es **volver al inventario**, no extender la excepción |
| Retirar el formato de protección **vigente** | — (nadie) | Lo que se retira es la capacidad de leer el **viejo** |

**Regla de aporte (bounded, explícita).** Este Bolt **retira** una capacidad de la herramienta y **cierra** un
registro. No corrige el defecto de 1.4, no toca el resolutor, no reimplementa los comandos, no migra valores y no
reintroduce controles. Si la condición de cierre no está cumplida, **no retira**: posterga y lo registra.

---

## 5. Prerequisites and baseline

- **Bolt 023 y su SPEC** (`SPEC-261016-0039-…`): los **comandos de valores protegidos** con la **capacidad legacy
  aislada** (componente interno, bandera explícita, supresión por miembro, reporte) y el **registro de vencimiento**
  al que ese Bolt contribuyó. Su frontera se declara en §4.
- **Bolt 029 y su SPEC** (`SPEC-261016-0050-…`): la **ejecución del batch real del corpus** con sus originales
  conservados e inventariados — la **condición de cierre del lado del modelo**: sin corpus migrado, el retiro no
  puede declararse seguro.
- **Bolt 018 y su SPEC** (`SPEC-261016-0025-…`): el **proyecto del CLI** donde viven los comandos que pierden la
  capacidad, con su contrato de códigos de salida (la bandera retirada deja de existir y una invocación con ella es
  **entrada inválida**, exit `3`).
- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible**, el build y el arnés.
- **Dependencia externa (la condición):** el **inventario migrado por cliente**, cuyo **dueño** es el responsable
  del inventario (ADR-007 §3.3). **No** está bajo control del equipo: si no puede declararse cumplido, este Bolt
  **posterga** con motivo y nueva fecha en lugar de retirar.
- **Riesgo `high` → autonomía `L2` (§3.3):** el V-Bounce **para y confirma** ante cualquier señal de que la
  condición de cierre no está cumplida o su evidencia es dudosa. La aprobación del SPEC y del MEM exige
  independencia **endurecida a nivel de modelo** (§3.0).
- **Estado del árbol 2.0:** en construcción por otros V-Bounces; este Bolt escribe su SPEC y, **en su** V-Bounce, el
  retiro y el cierre.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline; el árbol congelado es la evidencia del
  formato viejo, no un insumo que se edite.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-030`), **1–2 V-Bounces** esperados más el ciclo de integración. El orden pone la
> decisión irreversible al final y su condición al principio: primero se verifica el registro, **después** se
> retira (sólo si corresponde), después se verifican las huellas y al final se cierra (o se posterga) el registro.

### Phase A — La condición de cierre, verificada contra el inventario real

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### A.1 El estado del registro

Se reúne el **registro de vencimiento** con sus tres datos: **dueño**, **condición de cierre** (100 % de los
valores clasificados como candidatos legacy convertidos a `aesgcm:` **o** declarados no migrables con su motivo) y
**fecha de revisita**. Sin dueño declarado y sin condición escrita, el Bolt **no puede cerrar** (§15 #2): un
registro incompleto no se completa desde acá — se reporta.

#### A.2 La evidencia de la condición

La condición se verifica con la evidencia del **inventario real** que el dueño declara, leído **en sólo lectura**:
por cada valor clasificado como candidato legacy, su **estado** (migrado, o no migrable con su motivo). Reglas del
manejo del dato: la verificación registra **estados**, **nunca** valores; el inventario es `confidential` y **no**
se copia a fixtures, reportes, logs ni MEMs; y ninguna parte de él cruza el límite del prompt (§3.6). El detalle
del camino ya está decidido en el Bolt 023 (clasificación explícita, sin heurística, `--dry-run` por defecto) y este
Bolt **no lo reinterpreta**: lee su resultado.

#### A.3 La decisión: retirar o postergar

- **Condición cumplida y evidenciada** → se procede al retiro (Fases B–D).
- **Condición no cumplida, o evidencia no declarable** → **no se retira nada**: se registra la **postergación**
  con su **motivo** y una **nueva fecha de revisita concreta** (Fase D.2). Es el desenlace que el READY declara
  legítimo, y el único admisible cuando la condición no está cumplida.

**Files created:**
- `2.0/artifacts/legacy-decryption-retirement/closure-evidence.json` — el estado verificado del inventario (por valor: clasificación y estado, **sin** valores) y la conclusión de la condición
- `2.0/artifacts/legacy-decryption-retirement/retirement-decision.md` — la decisión (retiro o postergación) con su fundamento y, si posterga, el motivo y la nueva fecha

---

### Phase B — El retiro (sólo si la condición está cumplida)

**Duration:** ~3h de ciclo — **Complexity:** High

#### B.1 Fuera el componente, la bandera y la supresión

Se elimina el **componente interno** de descifrado legacy, la **opción explícita** que lo habilitaba y la
**supresión de advertencias por miembro** (`SYSLIB0020`/`SYSLIB0021`) que la excepción exigía, con su comentario de
cita y vencimiento: la excepción cumple su condición y **desaparece entera**. La verificación incluye que **no**
sobreviva ninguna supresión a nivel de **proyecto** ni de **solución** (ADR-007 §3.4 / ADR-008 §3.4) y que no quede
el código "por si acaso": un método huérfano sin llamadores sigue siendo huella y sigue siendo deuda.

#### B.2 Ninguna generalización que sobreviva

La herramienta **nunca** cifraba con el formato viejo (la excepción era sólo de lectura): al retirarla, no debe
quedar ninguna pieza que "cifre" con 3DES/MD5 ni ninguna abstracción que permita reintroducirlo sin decisión. Las
primitivas **vigentes** (AEAD de 256 bits, `*.Create()` en lugar de los `*CryptoServiceProvider`) siguen siendo las
únicas del árbol.

#### B.3 El fallo explícito ante el formato viejo

Un valor del formato viejo que llegue a los comandos **después** del retiro produce un **error tipado y
accionable** que nombra la situación: que el formato es el viejo (anterior a la migración), que su lectura fue
**retirada** tras cerrar el inventario y qué hacer (volver al inventario / a la fuente original del valor). Nunca
degradación silenciosa, nunca un intento de descifrado a ciegas, nunca reportar el valor como texto plano.

#### B.4 Los comandos vigentes siguen operativos

Los comandos de valores protegidos conservan su funcionalidad sobre los esquemas **vigentes** (`plain:` y
`aesgcm:`): inspeccionar, migrar (de `plain:`/entrada a `aesgcm:`) y re-proteger siguen funcionando, y su contrato
de códigos de salida no cambia salvo por la **desaparición de la bandera** — una invocación con ella es ahora
**entrada inválida** (exit `3`), no un `--ignore` silencioso.

**Files modified (en el V-Bounce de este Bolt):**
- `2.0/src/Cooperator.Cli/**` — eliminación del componente legacy, de su bandera y de su supresión por miembro

---

### Phase C — La verificación de huellas

**Duration:** ~1,5h de ciclo — **Complexity:** Medium

Se verifica, con evidencia, que del formato viejo **no queda huella** en ninguna forma: ninguna referencia a las
primitivas prohibidas (3DES, MD5) en el camino de valores protegidos, ninguna constante o derivación de la clave
**pública** de 1.4, ningún IV fijo, ninguna supresión de advertencias residual, y ninguna mención que reintroduzca
el mecanismo de detección por substrings (ADR-004 §3.3: después de la migración **no queda heurística**). El
control se ejerce sobre el **código, la configuración y los artefactos versionados** y sobre los **artefactos
construidos** de la herramienta (NFR `no-embedded-secrets` de ADR-007 §5).

**Files created:**
- `2.0/tests/Cooperator.Cli.Secrets.Tests/NoLegacyTracesTests.cs` — 0 primitivas, 0 clave, 0 supresiones residuales, 0 heurística (control estático sobre fuentes, configuración y artefactos)

---

### Phase D — El cierre del registro (o su postergación) y la constancia

**Duration:** ~2h de ciclo — **Complexity:** Medium

#### D.1 El registro cerrado

El registro de vencimiento queda **cerrado** con su **estado** (condición de cierre cumplida, con la referencia a la
evidencia de la Fase A), su **dueño** y la **fecha del cierre**. El cierre es un **acto humano con el registro
delante**; este Bolt aporta la evidencia y el registro, no se autocierra.

#### D.2 La postergación registrada (desenlace alternativo)

Si la condición no está cumplida, **no** se retira y el registro queda con la **postergación**: **motivo** y
**nueva fecha de revisita concreta**. Postergar **se registra**, nunca se omite: la política explícita del ADR es que
la fecha es "una fecha concreta, no *cuando se pueda*". En ese caso el Bolt cierra como **postergación
documentada** —con su evidencia y su fecha— y **no** como retiro a medias.

#### D.3 La constancia y su reflejo

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-030.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1 por lo que desaparece (**el componente de descifrado legacy, su bandera y su supresión por miembro**;
**por qué**: ADR-007 §3.3/§3.4, la excepción es transitoria y su condición de cierre se cumplió; **con qué se
reemplaza**: nada — el formato viejo dejó de ser legible a propósito, y los valores que quedaron en él se declaran
no migrables con su motivo, sin reemplazo posible). El retiro (o la postergación) se refleja además en la
**documentación de la herramienta** y en las **notas de migración** (Bolt 028), que es donde el consumidor lo lee.

**Files created:**
- `2.0/docs/legacy-decryption-retirement.md` — el registro de vencimiento cerrado (o su postergación) con estado, dueño y fecha, publicado con el producto
- `2.0/docs/api-removals/US-000.BOLT-030.json` — constancia del retiro (insumo del Bolt 028)
- `2.0/tests/Cooperator.Cli.Secrets.Tests/LegacyValueFailsExplicitlyTests.cs` — un valor del formato viejo falla con error tipado y accionable

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-007/ADR-008/ADR-004, expresados como Given/When/Then verificables.

### AC-1: La condición de cierre fue verificada (o el Bolt posterga registrando)

**Given** el registro de vencimiento y el inventario real declarado por su dueño
**When** se evalúa la condición
**Then** **(a)** la condición está **cumplida y evidenciada** —100 % de los candidatos legacy convertidos a
`aesgcm:` **o** declarados no migrables con su motivo— y el Bolt procede; **o** **(b)** la condición no está
cumplida o su evidencia no es declarable, y el Bolt **no retira** y registra la **postergación con motivo y nueva
fecha concreta**.

### AC-2: El formato viejo ya no se descifra, y el fallo es explícito

**Given** un valor del formato de protección de 1.4 después del retiro
**When** se le pasa a los comandos de valores protegidos
**Then** falla con un **error tipado y accionable** que nombra la situación y qué hacer, sin degradación silenciosa,
sin intento a ciegas y sin tratarlo como texto plano.

### AC-3: El componente, la bandera y la supresión por miembro desaparecieron

**Given** el árbol 2.0
**When** se busca la capacidad legacy
**Then** no existe el componente, no existe la opción que lo habilitaba, y **no** queda ninguna supresión de
`SYSLIB0020`/`SYSLIB0021` —ni a nivel de miembro, ni de proyecto, ni de solución— ni código huérfano de esa
capacidad.

### AC-4: Ninguna huella de la clave ni de las primitivas del formato viejo

**Given** el código, la configuración y los artefactos versionados (y los artefactos construidos de la herramienta)
**When** corre el control de huellas
**Then** **0** referencias a las primitivas prohibidas en el camino de valores protegidos, **0** constantes o
derivaciones de la clave pública de 1.4, **0** IV fijo y **0** rastro del mecanismo de detección por substrings
(NFR `no-embedded-secrets`).

### AC-5: Los comandos vigentes siguen operativos

**Given** los esquemas vigentes (`plain:`, `aesgcm:`)
**When** se ejecutan los comandos de valores protegidos
**Then** inspeccionar, migrar y re-proteger siguen funcionando con su contrato de códigos; sólo desaparece la
bandera legacy, y usarla es **entrada inválida** (exit `3`).

### AC-6: El registro queda cerrado (o postergado) y reflejado

**Given** el desenlace del Bolt
**When** se lee el registro publicado con el producto
**Then** queda **cerrado** con su estado, su dueño y la **fecha del cierre** —o **postergado** con motivo y nueva
fecha concreta— y el cambio está en la **documentación de la herramienta** y en las **notas de migración**.

### AC-7: El corpus migrado ya no depende de esta capacidad

**Given** el corpus convertido (Bolt 029) y sus originales conservados
**When** se revisa la dependencia
**Then** el corpus migrado **no** requiere la capacidad retirada, los originales están conservados/inventariados
según su condición, y la cláusula de re-migración desde los originales está declarada (§12).

### AC-8: Ningún valor ni material de clave se extrae ni se registra

**Given** el inventario real y las corridas de verificación
**When** se revisan sus artefactos
**Then** **0** valores y **0** material de claves aparecen en fixtures, reportes, logs, MEMs ni prompts: sólo
**estados** (migrado / no migrable con motivo) y los `kid`/esquemas que el propio producto reporta.

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error, la suite hermética está verde (con los casos de
integración filtrados por categoría) y `git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4`
están vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "los comandos de valores protegidos **ya no** ofrecen descifrar el formato viejo" | Fase B.1/B.3 | AC-2, AC-3 · `LegacyFormatNoLongerDecryptsTests` |
| "el componente interno, su bandera y la supresión **por miembro** desaparecen del árbol" | Fase B.1 | AC-3 · `NoLegacyTracesTests` |
| "**no** queda ninguna referencia a las primitivas ni a la clave del formato viejo —incluida una supresión que sobreviviera—" | Fase C | AC-4 · `NoLegacyTracesTests` + `NoEmbeddedSecretsOnArtifactsTests` |
| "un valor del formato viejo… falla con un **error explícito y accionable**" | Fase B.3 | AC-2 · `LegacyValueFailsExplicitlyTests` |
| "el registro de vencimiento queda **cerrado**… **o** con la **postergación registrada**, su motivo y su **nueva fecha**" | Fases A.3 y D.1/D.2 | AC-1, AC-6 · registro publicado |
| "el cierre queda reflejado donde el producto lo declara —documentación de la herramienta y notas de migración—" | Fase D.3 | AC-6 · `docs/legacy-decryption-retirement.md` + fragmento |
| "la constancia de que el corpus migrado ya **no** depende de esta capacidad" | Fase D.3 + Bolt 029 | AC-7 · evidencia de SPEC-0050 |
| (riesgo declarado por el Bolt) "la postergación que se convierte en 'algún día'" | Fases A.3 y D.2 | AC-1, AC-6 · fecha concreta registrada |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Control de huellas (`NoLegacyTracesTests`):** sobre fuentes, configuración y artefactos versionados: **0**
  primitivas prohibidas (3DES/MD5) en el camino de valores protegidos, **0** constantes o derivaciones de la clave
  del formato viejo, **0** IV fijo, **0** supresiones `SYSLIB0020`/`SYSLIB0021` (ni de miembro, ni de proyecto, ni
  de solución) y **0** rastro del mecanismo de detección por substrings.
- **Comportamiento posterior al retiro (`LegacyValueFailsExplicitlyTests`):** con un valor del formato viejo
  (fixture **sintético** de forma compatible, nunca un valor real), los comandos fallan con el **error tipado**
  esperado; el mensaje nombra la situación y no expone material.
- **Comandos vigentes:** inspeccionar/migrar/re-proteger sobre `plain:` y `aesgcm:` siguen operativos; invocar con
  la bandera retirada es **entrada inválida** (exit `3`).
- **Registro:** el registro publicado valida su estructura (estado, dueño, condición, fecha de cierre **o** motivo
  y nueva fecha de revisita) según el desenlace.
- **Ausencia de generalización:** no existe ningún camino que cifre con el formato viejo ni una abstracción que
  permita reintroducirlo sin una decisión nueva.
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** valor del formato viejo con carga truncada; valor que **parece** legacy pero es `plain:`; bandera
  retirada presente en un script de un consumidor (entrada inválida explícita); registro sin dueño o sin condición
  (el Bolt **no** cierra: §15 #2); condición "casi" cumplida (un valor pendiente ⇒ **postergar**, nunca retirar).
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG (el defecto de 1.4 tiene su propio BUG); su evidencia es el
  control de huellas y el comportamiento posterior al retiro.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

- **Integration tests (real dependencies):** los cuatro archivos de
  `2.0/tests/Cooperator.Cli.Secrets.Integration.Tests/` (frontmatter), `Category=Integration`, ejecutados en el
  **ciclo de integración** del Bolt contra el entorno donde **viven los valores**:
  **(a)** la **condición de cierre** verificada contra el inventario real declarado por su dueño (100 % de los
  candidatos legacy convertidos o declarados no migrables con motivo); **(b)** los comandos **después** del retiro
  sobre un valor del formato viejo del inventario, con el error explícito; **(c)** los esquemas **vigentes**
  operativos; y **(d)** el control de huellas sobre los **artefactos construidos/publicados** de la herramienta.
- **Contract / E2E against the real providers:** el "proveedor" acá es el **entorno real del cliente** (su
  inventario y su almacén): lo que se verifica end-to-end es que **el inventario está cerrado** y que, tras el
  retiro, la herramienta no puede leer el formato viejo **ni siquiera con datos reales delante**.
- **Edge cases:** inventario parcialmente declarado (⇒ postergación registrada); valor no migrable por ASCII (el
  caso que el ADR marca como no migrable automáticamente); contexto **FIPS** habilitado en la máquina de la corrida
  (la herramienta ya no depende de las primitivas legacy y debe reportar el contexto, no fallar en silencio);
  instalación sin inventario accesible (⇒ **no** se cierra).
- **Manejo de datos:** acceso de **sólo lectura** al inventario; la evidencia registra **estados**, nunca valores;
  clase `confidential` sin cruzar el límite del prompt; ningún valor del cliente entra en fixtures, reportes ni
  logs.
- **BUG evidence:** `n/a` — no hay defecto en corrección.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (V-Bounce) y en
> `integration_test_runs[]` del manifiesto (ciclo de integración) (§3.6, §3.12).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | El árbol compila con advertencias tratadas como error **sin** la supresión retirada | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (huellas, comportamiento posterior al retiro, comandos vigentes, registro) | `pass` — esperado (AC-2…AC-6) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del consumidor es el **contrato de códigos** de los comandos y el **error accionable** del formato viejo | `pass` — esperado (AC-2, AC-5) |
| SAST | V-Bounce | Cambio **en la superficie criptográfica de la herramienta**: se revisa el retiro y la ausencia de generalización | `pass` — esperado (AC-4) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias nuevas; el retiro no agrega paquetes | `pass` — esperado |
| OWASP Top 10 — static review | V-Bounce | El cambio **reduce** superficie: elimina primitivas prohibidas del árbol (3DES/MD5) | `pass` — esperado (AC-4) |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | **0** secretos ni material de claves en el código, la configuración, los fixtures, los reportes y los MEMs | `pass` — esperado (AC-8) |
| Hallucination lint | V-Bounce | Rutas, archivos y comandos referenciados existen | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `confidential`: el gate bloquea el envío de esa clase fuera del límite del prompt; el inventario **no** cruza ese límite | `pass` — esperado (AC-8) |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos | `pass` — esperado |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; su evidencia negativa es el **error explícito** posterior al retiro | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas del control de huellas y del comportamiento posterior al retiro | `pass` — esperado |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| No embedded secrets (ADR-007 §5) | V-Bounce + integración | **0** huellas de la clave y de las primitivas del formato viejo en el árbol 2.0 y en los artefactos versionados y construidos | `pass` — esperado (AC-4) |
| Protected value integrity (ADR-007 §5) | V-Bounce | El cambio **no** toca el camino de resolución vigente: 0 degradaciones silenciosas; el formato viejo deja de resolverse **por decisión**, no por fallo | `pass` — esperado (AC-2, AC-5) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | El retiro es de una capacidad **interna de la herramienta** (fuera del baseline de API de los cuatro ensamblados); su ruptura de herramienta va a las notas | `n/a` — sin superficie del baseline afectada |
| Integration tests (real dependencies) | Integration test | Inventario cerrado verificado en el entorno real, comportamiento posterior al retiro con datos reales delante y artefactos sin huellas | `pass` — **en el ciclo de integración (entorno por provisionar)** |
| Contract / E2E against the real providers | Integration test | El contrato verificado end-to-end es el **estado del inventario** y la **incapacidad de leer el formato viejo** | `pass` — **en el ciclo de integración** |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable de red |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint ni carga crítica afectada | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | La herramienta reporta por su **reporte** y sus códigos de salida (§11) | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: required` → `integration_test_plan.applicability: applicable`
> con sus tests, su entorno y sus datos. Si el desenlace fuera la **postergación**, el Bolt **no** pasa por el
> ciclo como "retiro verificado": su aceptación se apoya en el registro de la postergación y su evidencia (§6/A.3).

---

## 10. Security and data

- **El cambio reduce la superficie criptográfica del producto:** desaparecen del árbol las **primitivas
  prohibidas** por la política (3DES, MD5) y su **supresión de advertencias**: la excepción acotada cumple su
  condición y se retira, que es exactamente lo que ADR-007 §3.4 exige de ella.
- **El riesgo real es la irreversibilidad, y su control es la condición:** retirar antes de tiempo deja valores
  **ilegibles para el cliente**, sin recuperación posible. Por eso la condición de cierre se verifica **contra el
  inventario real** y, si no se cumple, **no se retira**: se posterga con motivo y nueva fecha.
- **Datos `confidential`:** el inventario describe **valores protegidos**. La verificación lee su **estado**, nunca
  su contenido; ningún valor ni material de clave entra en fixtures, reportes, logs, MEMs ni prompts; y una clase
  `confidential` **no** cruza el límite del prompt hacia un modelo externo (§3.6).
- **Sin claves ni material en ninguna parte:** la clave del formato viejo es **pública por definición** (está en el
  binario 1.4) y su presencia en el árbol 2.0 es precisamente la huella que este Bolt elimina; las claves del
  formato **vigente** siguen fuera del producto (variables de entorno o hook del host, ADR-007 §3.2).
- **Sin heurística después de la migración:** todo valor declara su esquema (`plain:` o `aesgcm:`); el mecanismo de
  detección por substrings del 1.4 no reaparece (ADR-004 §3.3).
- **Clasificación:** `data_classification: confidential` (espeja el Bolt) — es la del **inventario**, y manda el
  manejo de §10 y el resultado esperado **0** de los gates de secretos y de PII/DLP.
- **Integridad del árbol congelado:** `1.4/**` no se modifica; su código es la evidencia del formato viejo.

---

## 11. Monitoring and observability

- **La observabilidad del retiro son dos artefactos:** el **control de huellas** (qué dejó de existir y por qué se
  puede afirmar que no queda rastro) y el **registro de vencimiento publicado** (cerrado con su fecha, o
  postergado con su motivo y su nueva fecha). El consumidor los lee donde el producto los declara.
- **El comportamiento posterior al retiro es observable por contrato:** un valor del formato viejo produce un
  error accionable con su código; la herramienta no falla "raro" ni en silencio.
- **Evidencia versionada:** `2.0/artifacts/legacy-decryption-retirement/` (evidencia del cierre y decisión),
  `2.0/docs/legacy-decryption-retirement.md` (el registro publicado) y
  `2.0/docs/api-removals/US-000.BOLT-030.json` (constancia para las notas).
- **Sin telemetría de servicio:** la herramienta es un CLI (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration (del consumidor):** quien tenga valores en el formato viejo los migra con los comandos de valores
  protegidos **antes** del retiro (procedimiento de ADR-007 §3.3: inventariar, descifrar, **validar**, re-proteger
  bajo `aesgcm:`, reemplazar tras la validación con respaldo, registrar). Después del retiro, el camino para un
  valor que quedó atrás es la **fuente original** del valor (o el inventario del dueño), y la herramienta lo dice
  con un error accionable.
- **Compatibility:** se retira una **capacidad interna de la herramienta**, no superficie de los paquetes del
  framework: el gate de compatibilidad de API no la alcanza. Lo que cambia para un consumidor está declarado: la
  opción **desaparece** (usarla es entrada inválida) y el formato viejo deja de poder leerse **a propósito**, con su
  entrada en las notas de migración (Bolt 028) y en la documentación de la herramienta.
- **Rollback:** el código puede revertirse, pero **la lectura del formato viejo no se recupera por sí sola** —si el
  registro se cerró con la condición cumplida, revertir el retiro reabre una capacidad que la política ya cerró; y
  si la postergación es el desenlace, **no hay nada que revertir** (no se retiró). Un rollback deliberado exige
  **reabrir el registro** con motivo y fecha, no un commit silencioso.
- **Cláusula de re-migración del corpus (Bolt 029):** los originales del corpus de **modelos** se conservan hasta
  validar el release 2.0.0; este Bolt **no** los toca ni depende de ellos para su propio rollback.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Retirar antes de tiempo y dejar valores ilegibles para el cliente** (irreversible) | 2 | 5 | El retiro se apoya en la **condición de cierre** verificada contra el inventario real; si no se cumple, **se posterga** con motivo y nueva fecha (AC-1, §6/A.3) |
| **La postergación se convierte en "algún día"** | 3 | 3 | El registro exige **fecha de revisita concreta** y motivo: postergar **se registra**, nunca se omite (AC-6) |
| **Huellas residuales** (una referencia, una constante, una supresión que sobrevive) | 3 | 3 | Control explícito de huellas sobre fuentes, configuración y artefactos, incluido el nivel de proyecto/solución (AC-3, AC-4) |
| **Un valor del formato viejo tratado como texto plano o "adivinado"** tras el retiro | 2 | 4 | Error **tipado y accionable**; nunca heurística ni degradación (AC-2) |
| **Un cierre registrado sin que el inventario esté migrado** | 2 | 5 | El cierre es un **acto humano con la evidencia delante**; este Bolt aporta la evidencia, no se autocierra; sin dueño/condición declarados, **no** cierra (§15 #2) |
| **Inventario por cliente fuera del control del equipo** | 3 | 4 | El registro tiene **dueño** declarado y la condición es suya: si no puede declararse cumplida, el Bolt **posterga** (no fuerza) |
| **El retiro se lee como "la 2.0 ya no puede migrar nada"** | 2 | 2 | El retiro llega cuando el inventario está cerrado y la postergación es una salida explícita y registrada; se documenta en la herramienta y las notas (AC-6) |
| Contexto **FIPS** en la máquina de la corrida | 2 | 2 | La política ya lo prevé: la herramienta debe **reportarlo** con mensaje accionable, nunca reportar "sin candidatos" cuando no pudo evaluar (ADR-007 §3.5) |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| El retiro y el cierre son **una sola decisión**, y la condición se verifica **antes** de retirar | Dejar de poder leer el formato viejo **porque** el inventario está cerrado: retirar sin cerrar rompe al cliente; cerrar sin retirar deja la excepción viva sin vencimiento (doctrina del Bolt) |
| La **postergación con motivo y nueva fecha** es un desenlace **legítimo** del Bolt | El READY lo declara así y ADR-007 §3.3 lo exige: la fecha es concreta, no "cuando se pueda". Forzar el retiro con la condición incompleta sería el único error irreversible |
| El registro **no se autocierra** desde el agente | El cierre es un acto humano con el registro delante; el agente aporta la evidencia y el registro, y la aprobación del MEM es el punto donde el humano lo firma |
| El control de huellas cubre también **proyecto y solución**, no sólo el miembro | Es la regla explícita de ADR-007 §3.4 y ADR-008 §3.4: una supresión amplia sobrevive a la excepción y la hace permanente |
| Los comandos **vigentes** no cambian salvo por la desaparición de la bandera | La excepción era de **lectura** del formato viejo; retirarla no debe tocar inspección, migración ni rotación de los esquemas vigentes |
| La evidencia del inventario registra **estados**, nunca valores | Clase `confidential`: registrar un valor sería la fuga que la clasificación prohíbe; y no hace falta para verificar la condición |
| El fragmento de ruptura nombra "sin reemplazo" para el formato viejo | Es exactamente lo que ocurre: el formato viejo deja de ser legible y los valores que quedaron en él se declaran no migrables con su motivo; un "reemplazo" inventado sería mentir sobre la recuperabilidad |
| El corpus de **modelos** (Bolt 029) aporta la condición de cierre del lado del modelo y **no** se toca acá | Su frontera es explícita: este Bolt retira una capacidad de **valores**, no re-ejecuta ni archiva el corpus |

---

## 15. Stop conditions

> Con `autonomy_level: L2` (riesgo `high`), ante cualquiera de estos casos el V-Bounce **para y reporta** en lugar
> de continuar.

1. **La condición de cierre no está cumplida o su evidencia no es declarable:** **no se retira nada**. La salida es
   la **postergación registrada** con motivo y **nueva fecha concreta** (§6/A.3 y D.2). Retirar igual es la única
   acción prohibida de forma absoluta por el Bolt y por el ADR.
2. **El registro está incompleto** (sin dueño, sin condición de cierre escrita, sin fecha de revisita): detener y
   reportar. Un registro incompleto **no** se completa desde acá: el Bolt no cierra y se escala a su dueño.
3. **Aparece un valor sin migrar al momento de cerrar:** detener y **volver al inventario** (ADR-007 §3.3). La
   excepción **no** se extiende ni se prorroga "un poco más" para taparlo.
4. **La verificación de huellas encuentra una supresión residual amplia** (proyecto o solución) o código de la
   capacidad sobreviviendo sin llamadores: detener; el retiro no está completo mientras exista.
5. **Se necesitaría tocar el resolutor, los comandos vigentes o el defecto de cifrado de 1.4** para cerrar:
   detener y escalar a su Bolt/ADR. Este Bolt **retira** una capacidad; no corrige ni rediseña criptografía.
6. **Un valor o material de clave aparece en un artefacto de evidencia** (reporte, log, fixture, MEM, prompt):
   detener de inmediato y reportar; es la fuga que la clase `confidential` prohíbe.
7. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
8. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite o build): detener y escalar la
   secuencia.
9. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, el tratamiento de un valor que el inventario
   declara en un estado que no está previsto): parar y reportar; no se rellena el hueco con una suposición
   (§2.4.1).

---

## 16. Definition of Done (DoD)

- [ ] Fases A–D implementadas, con **uno** de los dos desenlaces declarados: **retiro + cierre** o **postergación
      registrada**
- [ ] Todos los criterios de aceptación (AC-1…AC-9) verificados en el desenlace que corresponda
- [ ] Suite hermética verde (§8.1) y suite de integración escrita, compilada y estáticamente verificada (§8.2)
- [ ] Si el desenlace es el **retiro**: el componente, la bandera y la supresión por miembro **fuera**; **0**
      huellas de la clave y de las primitivas del formato viejo; el formato viejo falla con error accionable; los
      comandos vigentes operativos
- [ ] Si el desenlace es la **postergación**: **nada** retirado y el registro con motivo y **nueva fecha concreta**
- [ ] El registro queda **publicado** con el producto (estado, dueño, fecha) y la constancia está en el fragmento de
      rupturas y en las notas (Bolt 028)
- [ ] El código sigue ADR-007 (§3.3/§3.4/§3.5), ADR-008 (§3.1/§3.4) y ADR-004 (§3.3)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9, con los NFR `no-embedded-secrets` y
      `protected-value-integrity` verificados
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la constancia del retiro **sin**
      ningún valor del inventario
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos)
- [ ] AITL-MEM-Approval registrado (riesgo `high`: independencia de aprobación **endurecida a nivel de modelo**, §3.0)
- [ ] Plan de integración satisfecho antes de la aceptación: corrida de cobertura pasante sobre el entorno real del
      inventario, registrada en `integration_test_runs[]` (o el desenlace de postergación declarado y aceptado)
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-030`](../functional/bolts/US-000.BOLT-030-retiro-descifrado-legacy.md) y su manifiesto
  [`US-000.BOLT-030-…json`](../metrics/bolts/US-000.BOLT-030-retiro-descifrado-legacy.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADRs gobernantes: [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) · [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) · [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md)
- SPECs prerrequisito: [`SPEC-261016-0039`](./SPEC-261016-0039-comandos-de-valores-protegidos.md) (comandos con la capacidad legacy aislada y el registro) ·
  [`SPEC-261016-0050`](./SPEC-261016-0050-batch-real-del-corpus.md) (corpus de modelos migrado y originales conservados) ·
  [`SPEC-261016-0025`](./SPEC-261016-0025-cli-del-motor.md) (el CLI donde viven los comandos) ·
  [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés y build)
- SPECs relacionados: [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) (el resolutor y los esquemas vigentes, que **no** se tocan)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-23 y §6 ítem 12
- Evidencia de código 1.4 (contexto del formato viejo, verificado en esta sesión): `Library/Cryptography.cs`
  (clave `_CryptoKey` y `_Iv` hardcodeados; TripleDES con clave derivada por MD5 y `Encoding.ASCII`);
  `Data/BaseRule.cs:104` (la heurística por substrings que la 2.0 elimina); `Library/Repository/RepositoryMapper.cs:92`
  (el separador `0xA1` del contrato de claves del «Repository»)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|---------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-030`: verificación de la condición de cierre contra el inventario real, retiro del componente de descifrado legacy con su bandera y su supresión por miembro (o **postergación con motivo y nueva fecha** como desenlace legítimo), control de huellas de la clave y las primitivas, fallo explícito ante el formato viejo, comandos vigentes operativos y cierre del registro de vencimiento |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** This SPEC remains a draft until the
> Dev-validator (+ applicable domain owners) records `AITL-SPEC-Approval`
> (in the `review` frontmatter block). Bolt approval (`AITL-BOLT-READY-Approval`)
> authorizes SPEC preparation; **SPEC approval** authorizes the code-run /
> V-Bounce. A material source change invalidates this approval — stop,
> revise, re-approve (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de seguridad (retiro de la excepción criptográfica) + dueño del inventario (condición de cierre) o su representante + dueño de arquitectura (frontera con el Bolt 023 y el 029)] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **Los dos desenlaces son legítimos** (el READY lo declara): **retiro + cierre** si la condición está cumplida y
   evidenciada; **postergación registrada** con motivo y nueva fecha concreta si no lo está. El SPEC trata la
   postergación como resultado del Bolt, no como fracaso — y **prohíbe** retirar con la condición incompleta.
2. **El cierre es un acto humano con el registro delante:** este Bolt aporta la evidencia y el registro; la firma
   del cierre (y del MEM) corresponde al actor. Si el registro está incompleto (sin dueño o sin condición escrita),
   el Bolt **no cierra** y se escala (§15 #2).
3. **Manejo `confidential`:** la evidencia del inventario registra **estados** (migrado / no migrable con motivo),
   **nunca** valores; nada del inventario entra en fixtures, reportes, logs, MEMs ni prompts. Confirmar el criterio.
4. **Frontera:** el retiro es de una capacidad **interna de la herramienta** (Bolt 023); el corpus de **modelos** y
   sus originales son del Bolt 029 y sólo aportan la condición de cierre del lado del modelo. Confirmar que el
   recorte es el esperado.
5. **Contexto FIPS** (ADR-007 §3.5): la corrida puede encontrarse con que las primitivas legacy **lancen**; la
   herramienta debe **reportarlo** con mensaje accionable, y tras el retiro el escenario desaparece del producto.
   Confirmar que el manejo esperado es el reportado.
6. **Riesgo `high` → `L2`:** el V-Bounce para ante cualquier señal de condición incumplida o evidencia dudosa; la
   aprobación del SPEC/MEM exige independencia endurecida a nivel de modelo (§3.0).
