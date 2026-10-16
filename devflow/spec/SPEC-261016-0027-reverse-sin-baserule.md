---
id: "SPEC-261016-0027"
title: "El reverse-engineering deja de heredar de la capa de datos: acceso propio por el provider único, cadena desde la configuración portable y paridad del esquema leído"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-016" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 4 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-005", "ADR-001", "ADR-004"] # mirrors the Bolt's `sources` + ADR-004, que gobierna de dónde sale la cadena de conexión (ver §2/§3)
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, lane de integración, build compartido y reporte de dependencias por ensamblado (Bolt 001)
  - "SPEC-261016-0004-particion-modeling.md" # el modelo portable (POCOs), el contrato de tipos y la tabla de mapeo única; el reverse relocalizado en el tooling (Bolt 003)
  - "SPEC-261016-0020-provider-de-datos.md" # el provider único, el interior tipado y el borde de conversión de la capa de datos (Bolt 014)
  - "SPEC-261016-0018-configuracion-portable.md" # el contrato de configuración y su catálogo: de dónde sale la cadena de conexión (Bolt 011)
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: 25 # revisión 3: presupuesto **explícito** (integer ≥ 1) para completar la unidad restante en un solo V-Bounce (§14) — el default de la plataforma/agente (10) se agotó en los Bolts 009 (dos veces) y 014, y con 25 completaron su unidad (§18)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "applicable" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests:            # applicable — the integration tests a run executes: escritos por este V-Bounce (ver §8.2)
    - "2.0/tests/CooperatorModeler.Reverse.Integration.Tests/SchemaParityTests.cs — lectura del esquema de la base de esquema conocido y comparación del modelo resultante contra el modelo esperado (entidades, propiedades con tipo nativo y CLR, claves primarias, identidad/rowversion/nullable, relaciones) — la paridad observable del Bolt"
    - "2.0/tests/CooperatorModeler.Reverse.Integration.Tests/ViewsAndKeysTests.cs — las vistas leídas como tablas (la limitación declarada del modelo) y las claves foráneas resueltas por las rutas de `sp_fkeys`/`sp_help`"
    - "2.0/tests/CooperatorModeler.Reverse.Integration.Tests/ConnectionStringResolutionTests.cs — la cadena de conexión tomada de la configuración portable (contrato del Bolt 011) contra una base real, sin cadena embebida ni mecanismo propio"
    - "2.0/tests/CooperatorModeler.Reverse.Integration.Tests/SchemaReaderLifecycleTests.cs — la conexión la crea y la dispone el propio reverse: ninguna conexión del llamador, ninguna conexión abierta al terminar"
  environment: "el entorno de CI del proyecto con SQL Server (la lane de integración del arnés del Bolt 001; si su SPEC fija un nombre de entorno, se usa ese)"
  data: "una base de datos de prueba con esquema conocido —tablas con tipos nativos variados, clave primaria simple y compuesta, claves foráneas, columna identidad y rowversion, columna anulable y una vista—, creada por el script de aprovisionamiento comiteado junto a las pruebas; credenciales por variables de entorno, nunca en el repositorio; sin datos de negocio ni personales"
  reason: ""        # n/a — no aplica: el plan es `applicable`
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T05:13:37-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T05:13:37-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T05:13:37-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC y grant verificado: revisión 4 material (G15/G16) disparada por el V-Bounce N°1 detenido en `blocked` (`MEM-261016-0506`), SPEC `SPEC-261016-0027` revisión 4 `draft`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Redactor ≠ revisor (independencia)."
    - "Compliant — el retiro ordenado verificado: §D.3-b/§4/§5 registran el hecho medido (`WFO1000` en `ToStringInfoEditorForm.cs(19,21)/(31,23)` + `TreatWarningsAsErrors`), las dos salidas vetadas (anotar = transformación que §D.3-a descarta; silenciar = excepción de gate G21), la anticipación del 003 (`MEM-261016-0046` §13: «retiro por escalamiento, nunca un shim»), el cierre **3 destinos + retiro declarado** con reemplazo **cero** (la edición 2.0 es dirigida por contrato; la UI nueva es del Bolt 025) y que los editores **no se aportan** (§A.2/§D.3-b: sin Forms ni `UseWindowsForms`)."
    - "ADJUDICACIÓN — la rev4 **formaliza exactamente mi adjudicación (a)** del `MEM-261016-0506` (finding 5): retirar los `PropertyEditors` en lugar de reubicarlos, cerrar `OBLIGATION-TOOLING-01` como **3 destinos materializados + retiro declarado del 4.º**, con reemplazo cero y la UI dirigida por contrato como reemplazo funcional. Consistencia verificada punto por punto."
    - "Compliant — secciones verificadas: §7 (AC-8/AC-9 + mapeo, L722), §8.1 (verificación del retiro), §13/§14/§15 (la condición nueva: anotar/silenciar ⇒ parar, L996-999), §16 (DoD con la obligación «materializada y cerrada»), §17 (obligación «materializada y cerrada: 3 destinos + retiro»), §18 (**4 filas**), §19 (nota al aprobador)."
    - "Minor gap — `acknowledgment_reason: ""` aparecía **duplicado** en el frontmatter (error de redacción de la rev.4); corregido en este acto (sin impacto sustantivo: el valor era vacío y consistente)."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ REVISIÓN 4 — REVISIÓN MATERIAL (G15/G16) disparada por el V-Bounce N°1 detenido en `blocked`
     (`MEM-261016-0506`): **tres de los cuatro destinos** de `OBLIGATION-TOOLING-01` quedaron **materializados** ✔
     (el provider, el resto de `MSSQLCommon` y la mitad reverse del helper, con las cuatro llamadas cruzadas
     calificadas y la suite en verde) y el **cuarto** —los `PropertyEditors/*`— **no compila**: portados verbatim, el
     analizador WinForms moderno emite **`WFO1000`** sobre el form (`ToStringInfoEditorForm.cs` L19/L31) y
     `TreatWarningsAsErrors` lo convierte en **error**. Las dos salidas quedan fuera de la autoridad del ejecutor
     —anotar el código aportado es la **transformación** que §D.3-a descarta; silenciar el analizador es una
     **excepción de gate** y G21 exige ADR—, y el propio Bolt 003 **anticipó** este desenlace: «el camino es el
     **retiro por escalamiento** vía condición de parada, **nunca** un shim de compatibilidad». Esta revisión ordena
     ese camino: **el cuarto destino pasa de «reubicar» a RETIRAR los `PropertyEditors/*`** (§D.3-b), se declara
     como **ruptura de superficie del tooling** con su reemplazo (**cero**: la edición 2.0 es dirigida por el
     contrato y la UI nueva es del Bolt 025), y la obligación **cierra como 3 destinos materializados + retiro
     declarado del 4.º**. Ajusta §1, §4, §5, §6 (notas + §A.2/§D.3), §7 (AC-8/AC-9 y su mapeo), §8.1, §13, §14, §15,
     §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** los otros tres destinos, el retiro de la herencia, el lector propio, la cadena desde el
     contrato, la política de nulabilidad de §D.3-a, la ubicación en el tooling, la fuente única del mapeo, la
     paridad observable y el plan de integración `applicable` siguen **en vigor** (el `review` se re-abre vacío: es
     una revisión material).
     ⚠️ (Histórica) **REVISIÓN 3 — REVISIÓN MATERIAL (G15/G16) que fija el **presupuesto de turnos** antes del V-Bounce N°1. La
     unidad restante de este Bolt es **grande** —la **reubicación del tooling** (los cuatro destinos de
     `OBLIGATION-TOOLING-01`: `Providers/MSSQLProvider.cs` con `MSSQLReverseFactory`, el resto de
     `Providers/MSSQLCommon.cs`, la **mitad reverse** de `DomainTreeHelper` y los `PropertyEditors/*` con sus
     recursos), el **lector propio** (`SqlClientSchemaReader`), la **resolución de la cadena** desde el contrato, la
     suite hermética y la **primera prueba de integración del reverse**— y §D.3-a exige ciclos **diagnóstico →
     directiva → build** que consumen turnos. La lección es **medida**: el **default de la plataforma (10)** se
     agotó en el **Bolt 009** (dos veces) y en el **Bolt 014**, y con **`turn_budget: 25`** esos Bolts completaron su
     unidad (una pasada / casi dos). Cambia **un solo valor del frontmatter**: `turn_budget: ""` →
     **`turn_budget: 25`** (§3, §14, §16, §18 y §19). **Nada más cambia** —ni la política de nulabilidad de §D.3-a,
     ni la materialización de la obligación de §D.3-b, ni el resto del plan— y el `review` se re-abre vacío (es una
     revisión material).
     ⚠️ (Histórica) **REVISIÓN 2 — REVISIÓN MATERIAL (G15/G16) cursada **antes** del V-Bounce N°1, con dos cosas y nada más:
     **(1) la política de nulabilidad preventiva del código aportado** (§D.3, nueva): este Bolt **aporta archivos
     del 1.4** (`MSSQLProvider.cs`, el resto de `MSSQLCommon.cs`, la **mitad reverse** de `DomainTreeHelper`, los
     `PropertyEditors/*` y el `DBReverse*` del tooling) y el árbol hereda `Nullable=enable` +
     `TreatWarningsAsErrors` del Bolt 001 — la trampa que ya detuvo a los Bolts 003/007/009 y que los Bolts 008/013/
     014/022/016-vecinos previeron por la misma vía: `#nullable disable` como **primera línea útil** de cada archivo
     **aportado del 1.4** que **produzca diagnósticos** (`CS8618`/`CS8600`/`CS8603`/`CS8625`), **jamás** en código
     nuevo, **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build
     como prueba**; y **(2) la constancia de que la OBLIGACIÓN del Bolt 003 está VENCIDA**: `OBLIGATION-TOOLING-01`
     (`2.0/docs/api-removals/US-000.BOLT-003.json`, `status: deferred`, `materialized_by: US-000.BOLT-014`)
     difería la **fase D** —la reubicación del tooling— «cuando la capa `Data` sea portada», y **la capa ya está
     portada** (el BOLT-014 cerró su Development Completed): **este Bolt es su materializador**, con los **tres
     recordatorios obligatorios** del §D.3 del SPEC-003 (§5/§17/§19).
     Ajusta §1, §4, §5, §6 (notas + §D.3), §7 (AC-9 y su mapeo), §8.1, §9, §13, §14, §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** el retiro de la herencia, el lector propio con el provider único, la cadena desde la
     configuración portable, la ubicación en el tooling, la fuente única del mapeo, la paridad observable y el plan
     de integración `applicable` siguen **en vigor** (el `review` se re-abre vacío: es una revisión material).
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-016`. El modelo y el formato
     son de los Bolts 003/004; el provider y el borde de conversión de la capa de datos son del Bolt 014; la UI
     del shell es del Bolt 025; la protección de valores es del ADR-007 (Bolts 022/023).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `applicable`, porque el Bolt declaró `test_integration: required` — la
     corrida contra SQL Server real se ejecuta en el ciclo de test de integración del Bolt, después de
     `AITL-MEM-Approval`, y su resultado queda como hecho en `integration_test_runs[]` del manifiesto. El reverse
     **no tenía pruebas propias en 1.4**: esta es su primera prueba de integración.
-->

# SPEC-261016-0027 — El reverse-engineering deja de heredar de la capa de datos: acceso propio por el provider único, cadena desde la configuración portable y paridad del esquema leído

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 5) |
| **Bolt** | [US-000.BOLT-016](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) |
| **ADRs** | [ADR-005](../adrs/ADR-005-provider-de-datos.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) |
| **Risk Class** | medium |
| **Revision** | 4 |

---

## 1. Objective

Se separa el **reverse-engineering de esquema** de la capa de datos: el componente deja de **heredar** de ella
—hoy lo hace por una clase privada `DataAccess : Cooperator.Framework.Data.BaseRule`
(`MSSQLProvider.cs:499-520`, que usa `DataBaseHelper.SetConnectionStringValue` y `FillDataSetBySQLText`)—,
**accede a la base por sí mismo** con el **provider único** de la 2.0 (`Microsoft.Data.SqlClient`), **toma su
cadena de conexión de la configuración portable** y **vive del lado del tooling**, sin arrastrar la maquinaria
de reglas, transacciones y caché que nunca necesitó. Y lo hace **sin cambiar lo que lee**: el esquema que
devuelve —las entidades, sus propiedades con tipo nativo y CLR, las claves, las relaciones y las vistas leídas
como tablas— es **el mismo que antes** (paridad observable), verificado contra una base real.

El problema que resuelve está documentado (REV-001 F-29 y §6 ítem 5): ese `Utility → Data` por **herencia** es el
mismo acoplamiento que ADR-001 §3.1 cortó a nivel de paquete y que hay que cortar también a nivel de tipos; es lo
que ata el modelo y el tooling a la maquinaria de la capa de datos, y lo que obliga a que el lector de esquema
pague el modelo de configuración y caché de esa capa sin necesitarlo. Un lector de esquema **no** es una regla de
negocio.

Si no se implementa: el tooling sigue dependiendo de la capa de datos por una vía que ningún ADR justifica, el
modelo queda expuesto a una arista que ADR-001 §3.1 prohíbe en su dirección, y el reverse sigue siendo la pieza
que impide cerrar la independencia del tooling respecto de la capa de acceso a datos.

**Dos cosas que la revisión 2 declara antes de ejecutar.** **(1) La trampa de nulabilidad, adelantada:** este Bolt
aporta archivos del 1.4 al tooling y el árbol hereda `Nullable=enable` + `TreatWarningsAsErrors` del Bolt 001; la
respuesta ya probada —`#nullable disable` **por archivo aportado del 1.4** que produzca diagnósticos— queda
autorizada en **§D.3**. **(2) La obligación vencida del Bolt 003:** `OBLIGATION-TOOLING-01` difería la fase D —la
reubicación del tooling— «cuando la capa `Data` sea portada»; **la capa ya está portada** y **este Bolt es su
materializador** (§5/§17/§19), con los tres recordatorios obligatorios del §D.3 del SPEC-003.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` F-29 documentó que `DBReverseHelper/Providers/MSSQLProvider.cs` lee el
esquema con procedimientos de sistema de SQL Server (`exec sp_help`, `INFORMATION_SCHEMA.TABLES`, `EXEC sp_fkeys`)
y que lo hace **a través de una clase privada `DataAccess : Cooperator.Framework.Data.BaseRule`**
(`MSSQLProvider.cs:499`), con `base.DataBaseHelper.SetConnectionStringValue(connectionString)` en el constructor
y `base.DataBaseHelper.FillDataSetBySQLText(SQLText, …)` para ejecutar. El §6 ítem 5 ruteó la separación a un ADR.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-005 §3.6** — **el reverse deja de heredar de `BaseRule`**: `MSSQLProvider` (tooling, junto al Modeler y
  al motor) **usa `Microsoft.Data.SqlClient` directamente** para leer el esquema (`sp_help`,
  `INFORMATION_SCHEMA`, `sp_fkeys`) y **toma la connection string de la configuración portable**; es un lector de
  esquema, no una regla de negocio, y heredar transacciones/`ObjectState`/caché nunca estuvo justificado.
  Además: **`Modeling` mantiene su independencia** (nada del modelo ni de `IDBReverseProvider`/`IDBTypeConverter`
  depende de `Data`) y los POCOs y las interfaces quedan en `Modeling` mientras **el provider concreto vive en el
  tooling**.
- **ADR-005 §3.1** — el **provider único** de la 2.0 es `Microsoft.Data.SqlClient` (el Bolt 014 lo fija, con sus
  criterios de asset y smoke): este Bolt **lo consume**, no lo decide ni lo cambia.
- **ADR-001 §3.1** — el mapa de paquetes y **las direcciones de dependencia permitidas**: el **modelo no depende
  de la capa de datos**; el reverse —y con él los `Providers/*` y el `DomainTreeHelper`— **viven en el tooling**
  (la aplicación del Modeler / el host del tooling creado por el Bolt 003); y **ADR-001 §3.6** fija que el
  **mapeo de tipos SQL↔CLR** es **una sola fuente** como dato portable en `Modeling`, consumida por el reverse y
  por el motor (nadie la redefine).
- **ADR-004 §3.2/§3.3** — la cadena de conexión se obtiene de `ICooperatorConfiguration` con **prefijo de esquema
  explícito**; el valor llega **crudo, con su prefijo**, y **la capa que lo consume** —`Data` para el valor de la
  capa, **el tooling para el reverse**— es la que invoca al **resolutor de valores protegidos**; `Data`/el
  consumidor **no** conocen algoritmo ni claves. La protección real y los esquemas son del **ADR-007**
  (Bolts 022/023): hasta que exista, `plain:` es la única condición que el framework implementa.
- **ADR-005 §3.4** — el cruce que **no** es de este Bolt: `SqlHelperParameterCache` y la caché de la capa de
  datos (Bolt 013) y el hazard de la conexión del llamador (Bolt 014).

**Estado de partida del componente (entregado por el Bolt 003).** El reverse **ya está relocalizado en el
tooling** y **ya consume la tabla de mapeo portable**: `SPEC-261016-0004` §D.1 movió
`Providers/MSSQLProvider.cs`, el resto de `MSSQLCommon.cs` y el `DomainTreeHelper` a
`2.0/src/CooperatorModeler/DBReverseHelper/**`, con el conversor de tipos apuntando a la **fuente única** de
`Modeling`. Lo que **queda** para este Bolt es el corte de la **herencia** (el `DataAccess : BaseRule`), el
acceso propio a la base, la cadena de conexión desde la configuración portable y la **prueba de paridad** — que
hoy no existe: el reverse no tiene pruebas propias en el árbol 1.4.

**Fronteras declaradas.** El **modelo** portable y el **formato** son de los Bolts 003/004 (acá no se cambia ni un
tipo del modelo ni el `.coop2`); el **provider** y el **borde de conversión** de la capa de datos son del Bolt
014 (acá se **usa** el provider, sin tocar la capa); la **UI** del Modeler —dónde se pide la conexión, cómo se
muestra el esquema— es del Bolt 025 (acá no se toca ninguna pantalla); y la **protección de valores** es del
ADR-007.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt (`required` → `applicable`).

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-016`](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-29 (el reverse hereda de la capa de datos y acopla `Utility` a `Data`) y §6 ítem 5 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) — §3.6 (el reverse deja de heredar, accede con el provider y vive en el tooling; `Modeling` mantiene su independencia) y §3.1 (provider único y sus criterios) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (mapa de paquetes, direcciones de dependencia permitidas, contenido de `Modeling`, el modelo sin aristas hacia `Data`, el reverse en el tooling), §3.6 (la frontera del mapeo de tipos y su fuente única), §3.4 (presupuesto de dependencias) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR (gobierna de dónde sale la cadena) | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato `ICooperatorConfiguration` y catálogo: `Cooperator:ConnectionStrings:<nombre>`) y §3.3 (prefijo de esquema explícito, error tipado, y **quién invoca al resolutor**: la capa que consume el valor, aquí el tooling) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-003`](../functional/bolts/US-000.BOLT-003-particion-modeling.md) — su SPEC canónica ([`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md)) entregó el modelo portable, la tabla de mapeo única y la **relocalización del reverse en el tooling** | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-014`](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — su SPEC canónica ([`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md)) fija el provider único y el interior tipado de la capa | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — su SPEC canónica ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md)) entrega el contrato de configuración del que sale la cadena | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, **lane de integración** y reporte de dependencias por ensamblado | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Cooperator.Framework.Utility/DBReverseHelper/Providers/MSSQLProvider.cs` — `MSSQLReverseFactory`
(`:43-73`, con el constructor que recibe la cadena y crea `MSSQLProvider` y `MSSQLTypeConverter`),
`MSSQLProvider : IDBReverseProvider` (`:78-89`, con la cadena en un campo propio) y la clase privada
**`DataAccess : Cooperator.Framework.Data.BaseRule`** (`:499-520`: `SetConnectionStringValue` en el constructor y
`ExecuteSQLText` con `FillDataSetBySQLText`) que es **la herencia a retirar**; y las rutas de lectura del esquema
que deben conservarse idénticas: `exec sp_help` (`:152-154`), `INFORMATION_SCHEMA.TABLES` (`:166-173`),
`EXEC sp_help` (`:407-410`) y `EXEC sp_fkeys` (`:427-444`, con el armado de `DBForeignKey`/`DBFieldFKey` y el
tratamiento del esquema `dbo.` en `:480-486`). Complementos: `DBReverseHelper/Providers/MSSQLCommon.cs`
(`SP_HelpTablesEnum` `:39-80` y los helpers de lectura), `DBReverseHelper/DBReverse.cs` (`DBObject`, `DBField`,
`DBTable`, `DBForeignKey`, `OrderEnum` — hoy en `Modeling` por el Bolt 003) y
`DBReverseHelper/DomainTreeHelper.cs` (`GetDomainTree(string connectionString, …)` construye la factory
concreta; hoy vive en el tooling por el Bolt 003).

**Coherencia del plan de integración:** el Bolt declara `test_integration: required` (§3.2.2) y este SPEC declara
`integration_test_plan.applicability: "applicable"` **con sus tests, su entorno y sus datos** (§8.2 y
frontmatter). **No hay desajuste** y por lo tanto no hay reporte bloqueante.

> **Presupuesto de turnos (revisión 3).** Este SPEC fija **`turn_budget: 25`** —parámetro de ejecución declarado en
> el frontmatter— para que la **unidad restante** de este Bolt se complete en **un solo** V-Bounce: §14 lo justifica
> y §16 lo refleja. **No es una fuente gobernada**: es un parámetro del SPEC y se cambia por revisión material, como
> cualquier otro valor del frontmatter. *(Precedente exacto del mecanismo: `SPEC-261016-0020` rev.4 y
> `SPEC-261016-0037` rev.3 — el Bolt 009 agotó el default (10) **dos veces** y el Bolt 014 lo agotó también; con
> `turn_budget: 25` ambos completaron su unidad, en una pasada y en casi dos.)*

---

## 4. Scope

### In scope

- El **retiro de la herencia** del reverse respecto de la capa de datos: la clase privada
  `DataAccess : BaseRule` desaparece y con ella la arista `reverse → Data`.
- El **acceso propio a la base** del reverse con el **provider único** de la 2.0 (`Microsoft.Data.SqlClient`),
  conservando **idénticas** las rutas de lectura del esquema (`sp_help`, `INFORMATION_SCHEMA.TABLES`,
  `sp_fkeys`, `EXEC sp_help`), los `DataTable`/`DataSet` que consumen el armado del modelo y el mapeo de
  columnas de `SP_HelpTablesEnum`.
- La **propiedad de la conexión**: el reverse crea y **dispone** su propia conexión; no recibe ni usa conexiones
  ajenas (el hazard de la conexión del llamador es del Bolt 014, y acá simplemente no aplica).
- La **cadena de conexión desde la configuración portable**: lectura de `Cooperator:ConnectionStrings:<nombre>`
  a través del contrato de configuración (Bolt 011), con el **prefijo de esquema** y la invocación del
  **resolutor de valores protegidos** del lado de **este** consumidor (ADR-004 §3.3).
- La **ubicación en el tooling** y la limpieza de las referencias que existían **sólo** para sostener la
  herencia (incluida la referencia del proyecto a la capa de datos), con el **reporte de dependencias por
  ensamblado** como evidencia de la ausencia de la arista.
- La **paridad observable del esquema leído**, como criterio de aceptación y como **primera prueba de
  integración del reverse**: lectura contra una base de esquema conocido y comparación con el **modelo
  esperado**.
- La constancia del cambio en las **notas de migración** (ADR-008 §3.1) si la superficie del tooling se altera
  (y el cambio de comportamiento para quien usa el tool: la cadena ya no se pide por un mecanismo propio).
- La **política de nulabilidad del código aportado del 1.4** (revisión 2, §D.3): la directiva `#nullable disable`
  **por archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —**jamás** en código nuevo (el lector
  propio, la resolución de la cadena y las pruebas se escriben con la nulabilidad activa) y **prohibida** a nivel
  de proyecto—, como mecánica de build del aporte del tooling.
- La **materialización de la obligación vencida** `OBLIGATION-TOOLING-01` del Bolt 003 (revisión 2, §5/§17): este
  Bolt es su **materializador** —la reubicación del tooling se materializa «cuando la capa `Data` sea portada», y
  la capa **ya está portada**—, con sus **tres recordatorios obligatorios** cumplidos.
- El **retiro de los `PropertyEditors/*`** (revisión 4, §D.3-b): el **cuarto destino** de
  `OBLIGATION-TOOLING-01` **no se reubica: se retira**, porque portado verbatim **no compila** (el analizador
  WinForms `WFO1000` + `TreatWarningsAsErrors`) y anotarlo o silenciar el analizador son salidas vetadas (§D.3-a y
  G21). El retiro se declara como **ruptura de superficie del tooling** para las notas del Bolt 028 —con **qué**,
  **por qué** y su **reemplazo: cero** (la edición de la 2.0 es dirigida por el contrato; el vehículo de edición de
  la UI nueva es del Bolt 025)— y la obligación **cierra como 3 destinos materializados + retiro declarado del 4.º**.

### Out of scope

- El **modelo** y su **formato**: Bolts 003 y 004 (ya entregados). Este Bolt **no** cambia un tipo del modelo ni
  el `.coop2`.
- El **proveedor de datos**, su versión/línea, el **borde de conversión** y el **interior tipado de la capa de
  datos**: **Bolt 014** (se **consume** el provider; no se toca la capa).
- La **caché** de la capa de datos y sus regiones: **Bolt 013**.
- La **interfaz de usuario** del shell del Modeler (dónde se pide la conexión, cómo se muestra el árbol, la
  asociación de archivos): **Bolt 025**. Este Bolt entrega el componente que lee el esquema; no su pantalla. *(Con
  la revisión 4, el **vehículo de edición** —aquello que daría un consumidor a los editores retirados— también es de
  la UI nueva: acá los editores **no** se reubican ni se reemplazan.)*
- La **protección criptográfica** de la cadena y los esquemas de valores protegidos: **ADR-007 / Bolts 022-023**
  (acá se invoca el **seam** del resolutor y, hasta que exista, `plain:` es la única condición implementada).
- Un **motor alternativo** o un provider de reverse distinto de SQL Server: OQ-005 (SQL Server es el único
  motor).
- La **re-verificación de capacidades del reverse que no cambian** con esta separación (por ejemplo el conteo de
  servidores, F-34, que es materia de un DISC y del shell).
- El **motor de generación**, el **CLI**, las **plantillas** y el **batch del corpus**: Bolts 017-021 y 029.

---

## 5. Prerequisites and baseline

- **Bolt 003 y su SPEC canónica
  ([`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md), `prerequisites`):** el modelo portable con sus
  POCOs, el contrato del reverse (`IDBReverseProvider`, `IDBTypeConverter`), la **tabla de mapeo única** y —lo
  que hace posible este Bolt— la **relocalización del reverse en el tooling**
  (`2.0/src/CooperatorModeler/DBReverseHelper/**`). Sin eso, no habría de dónde partir.

  > **La obligación del Bolt 003 está VENCIDA y este Bolt es su materializador (revisión 2).** El Bolt 003, en su
  > revisión 3, **difirió** la fase D —la **reubicación del tooling**— como **obligación registrada**
  > (`OBLIGATION-TOOLING-01`; `2.0/docs/api-removals/US-000.BOLT-003.json` —`status: deferred`,
  > `materialized_by: US-000.BOLT-014`— y `MIGRATION-PARTITION.md` §2.2): la condición de la obligación es
  > textualmente «**cuando la capa `Data` sea portada**». **La capa ya está portada** —el **BOLT-014** cerró su
  > Development Completed con la capa completa (proveedor único, interior tipado, retiro del caché del host)—, de
  > modo que la **condición de la obligación se cumplió** y **este Bolt es su materializador**.
  >
  > **Los tres recordatorios obligatorios** que el §D.3 del SPEC-003 dejó para quien la ejecute (y que este Bolt
  > cumple, §D.3):
  > 1. **Calificar las cuatro llamadas cruzadas** de la mitad reverse del helper a la portable, como
  >    `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X` — `CorrectGenerationNamesForRelatedFields` (L184),
  >    `FindEntityInTree` (L222/L297) y `CorrectSelectedCollections` (L292) —, porque el tooling tiene una clase con
  >    **el mismo nombre**.
  > 2. **La lista de rupturas** del MEM del Bolt 003 (§13) como **insumo de la constancia** de este Bolt: tipos que
  >    cambian de ensamblado **y** de namespace, atributos de diseñador retirados, `[Serializable]` retirado de los
  >    tipos que migran, base de las excepciones del grafo en `System.Exception`, `DomainTreeHelper` partido en dos
  >    ensamblados con el mismo nombre de tipo, `CompilerException` fuera de la partición y **el acoplamiento del
  >    modelo al editor retirado** (`DomainTree.cs` L1621).
  > 3. **Medir el reporte de dependencias a dos niveles**: el **presupuesto** en el proyecto/lock file y las
  >    **aristas prohibidas** en el **ensamblado**.
  >
  > La **constancia de esta materialización** entra en el **MEM** del V-Bounce que ejecute este Bolt, con la
  > referencia a `OBLIGATION-TOOLING-01` y su estado final. *(Revisión 4: la obligación **cierra como 3 destinos
  > materializados + retiro declarado del 4.º** — los `PropertyEditors/*` se **retiran** en lugar de reubicarse,
  > §D.3-b —, y ese cierre es el estado final que se consta.)*
- **Bolt 014 y su SPEC canónica
  ([`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md), `prerequisites`):** el **provider único**
  (`Microsoft.Data.SqlClient`) fijado con sus dos criterios de aceptación (asset consumible desde
  `netstandard2.1` y smoke real) y el interior de la capa tipado. Este Bolt **usa** el provider; su línea y su
  fijación centralizada son de ese Bolt.
- **Bolt 011 y su SPEC canónica
  ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md), `prerequisites`):** el contrato de
  configuración y su catálogo —de donde sale la **cadena de conexión**— con la regla del prefijo y el error
  tipado. Si no está en el árbol 2.0 al iniciar el V-Bounce, **parar y reportar** (§15).
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés (`2.0/tests/` sobre
  `net10.0`, con resolución de assets por runtime), la **lane de integración** con SQL Server, el **reporte de
  dependencias por ensamblado** y el build compartido. La corrida de this Bolt es, en los hechos, la **primera
  prueba de integración del reverse** (en 1.4 no tenía ninguna).
- **Acceso a una instancia de SQL Server con una base de esquema conocido** (dependencia externa declarada por el
  Bolt) y **el script de aprovisionamiento** de esa base comiteado junto a las pruebas. Si el entorno no está
  disponible, el V-Bounce puede cerrar con los tests escritos, construidos y verificados estáticamente, pero el
  **ciclo de test de integración queda pendiente** y el Bolt **no** pasa a `ready-for-acceptance` (§15).
- **El resolutor de valores protegidos (ADR-007) — frontera, no prerrequisito.** El *seam* (`IProtectedValueResolver`
  con el esquema del valor) es de ADR-004 §3.3 y su implementación/sus esquemas, del Bolt 022. Hasta que exista,
  **`plain:` es la única condición implementada**: si la cadena de la configuración llegara con otro esquema,
  **parar** (§15) en lugar de intentar descifrar o de leer el valor a ciegas.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de las rutas de lectura del
  esquema y de la herencia que se retira.
- **Decisión de ejecución registrada (no es supuesto):** el reverse es un **lector**: sólo ejecuta las consultas
  de esquema de ADR-005 §3.6 y no escribe en la base. No hay migración de datos en su alcance.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-016`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).
>
> **Mecánica de build del aporte del 1.4 (revisión 2, §D.3).** Cualquier archivo que este Bolt **aporte del 1.4**
> al tooling —`Providers/MSSQLProvider.cs`, el resto de `Providers/MSSQLCommon.cs`, la **mitad reverse** de
> `DomainTreeHelper`, los `PropertyEditors/*` y lo que el aporte traiga— sigue la **política de nulabilidad de
> §D.3**: `#nullable disable` como primera línea útil **sólo** en los archivos aportados que **produzcan**
> `CS8618`/`CS8600`/`CS8603`/`CS8625` (criterio objetivo; el build es la prueba), **nunca** en código nuevo (el
> lector propio, la resolución de la cadena y las pruebas conservan la nulabilidad activa) y **prohibido**
> `<Nullable>disable</Nullable>` en cualquier `.csproj`.
>
> **Materialización de la obligación vencida del Bolt 003 (revisión 2, §5).** Este Bolt **es** el materializador de
> `OBLIGATION-TOOLING-01` (la capa `Data` **ya está portada**: el BOLT-014 cerró su Development Completed), con sus
> **tres recordatorios obligatorios** —calificar las cuatro llamadas cruzadas; la lista de rupturas como insumo; el
> reporte de dependencias a dos niveles— cumplidos y constados (§D.3).

### Phase A — El inventario y el retiro de la herencia

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 Lo que se retira y lo que se conserva (inventario cerrado)

| Pieza | Estado (Bolt 003) | Acción en este Bolt |
|---|---|---|
| Clase privada `DataAccess : Cooperator.Framework.Data.BaseRule` (con `SetConnectionStringValue` y `ExecuteSQLText` → `FillDataSetBySQLText`) | en `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs` | **Se retira** (es la herencia) |
| `MSSQLReverseFactory` (recibe la cadena y crea provider y conversor) | idem | Se **conserva**, con la cadena obtenida de la configuración (§C) |
| `MSSQLProvider : IDBReverseProvider` | idem | Se **conserva**; su lectura pasa a ejecutarse con `Microsoft.Data.SqlClient` (§B) |
| Rutas de lectura: `exec sp_help`, `INFORMATION_SCHEMA.TABLES`, `EXEC sp_help`, `EXEC sp_fkeys` y el armado de `DBForeignKey`/`DBFieldFKey` (incluido el tratamiento del esquema `dbo.`) | idem + `MSSQLCommon.cs` | Se **conservan idénticas** en su texto y en la forma de los resultados que consumen el armado del modelo |
| `SP_HelpTablesEnum` y los helpers de lectura de `MSSQLCommon.cs` | idem | Se **conservan**; su ejecución pasa por el lector propio (§B) |
| Tabla de mapeo de tipos | ya en `Modeling` (fuente única) | Se **consume**; **no** se duplica ni se redefine (§D.2) |

#### A.2 La limpieza de referencias

Se retiran del proyecto del tooling las referencias que existían **sólo** para sostener la herencia —en
particular la **referencia a la capa de datos**—, y se conserva lo que el tooling necesite por otras razones.
*(Revisión 4: ya **no** hay que conservar nada «por los editores/Forms»: los `PropertyEditors/*` **no se reubican**,
se **retiran** (§D.3-b), así que el `.csproj` **no** declara `UseWindowsForms`.)* El resultado se evidencia con el
**reporte de dependencias por ensamblado**: ninguna arista del componente del reverse hacia la capa de datos,
ni hacia el modelo en su dirección prohibida.

> **Señal de diseño (no una excepción):** si al retirar la herencia resultara que el reverse **necesita** algo de
> la capa de datos, eso **no** se resuelve reutilizando esa capa "por conveniencia" (el Bolt lo prohíbe
> explícitamente): se **para y se reporta** (§15).

---

### Phase B — El acceso propio a la base, con el provider único

**Duration:** 4h total cycle — **Complexity:** High

#### B.1 El lector de esquema

Se reemplaza el acceso heredado por un **lector propio** del tooling que crea y dispone **su** conexión y ejecuta
las consultas de esquema con `Microsoft.Data.SqlClient` (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`),
devolviendo **los mismos `DataSet`/`DataTable`** que el armado del modelo ya consume: **el texto de las
consultas y la forma de los resultados no cambian** (eso es lo que la paridad verifica). Se conservan
`SP_HelpTablesEnum`, el orden de lectura de columnas y el tratamiento ya conocido del esquema `dbo.` al armar las
relaciones.

**La conexión la posee el reverse:** se crea desde la cadena resuelta (§C), se abre y se **dispone** en el mismo
ámbito; no se acepta ni se modifica una conexión del llamador (el hazard equivalente en la capa de datos es del
Bolt 014) y no queda ninguna conexión abierta al terminar.

**Files created:**
- `2.0/src/CooperatorModeler/DBReverseHelper/Providers/SqlClientSchemaReader.cs` — el lector propio: conexión propiedad del componente, ejecución de las consultas de esquema y devolución de los conjuntos de datos con la forma esperada
- `2.0/tests/CooperatorModeler.Reverse.Tests/CooperatorModeler.Reverse.Tests.csproj` — proyecto de pruebas `net10.0` del componente (framework y versiones los fija el Bolt 001)

**Files modified:**
- `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs` — retiro de `DataAccess : BaseRule`; lectura por el lector propio
- `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLCommon.cs` — helpers de lectura sobre el lector propio (sin `BaseRule`)
- `2.0/src/CooperatorModeler/DBReverseHelper/DomainTreeHelper.cs` — construye la factory con la cadena resuelta (§C), sin tocar su semántica de árbol
- `2.0/src/CooperatorModeler/CooperatorModeler.csproj` — limpieza de referencias de §A.2

---

### Phase C — La cadena de conexión desde la configuración portable

**Duration:** 3h total cycle — **Complexity:** Medium

#### C.1 Del contrato, con su prefijo, y por el resolutor de este consumidor

La cadena se obtiene del **contrato de configuración** (`ICooperatorConfiguration`, Bolt 011), con el nombre del
catálogo (`Cooperator:ConnectionStrings:<nombre>`; el nombre por defecto se conserva para continuidad) y **con su
prefijo de esquema explícito**. El valor llega **crudo, con su prefijo**, y **este consumidor** —el tooling que
lo va a usar— es el que **invoca al resolutor de valores protegidos** con el esquema y la carga (ADR-004 §3.3).
Hasta que el Bolt 022 entregue el resolutor, **`plain:` es la única condición implementada**: cualquier otro
esquema **detiene** la operación con error (**§15**), nunca se lee un valor a ciegas.

Lo que **no** pasa: la cadena **no** se embebe en el tooling, **no** se duplica en otro mecanismo, **no** se
inspecciona su contenido para adivinar su estado y **no** se registra ni se muestra en mensajes. La
**configuración de la aplicación** (archivo + entorno, precedencia y su vínculo con el shell) es del Bolt 011/021
y la **pantalla** donde el usuario la provee es del Bolt 025: acá se consume el contrato.

**Files modified:**
- `2.0/src/CooperatorModeler/DBReverseHelper/Providers/MSSQLProvider.cs` y `DomainTreeHelper.cs` — la cadena entra por el contrato (no por un parámetro provisto por un formulario) y se resuelve con el seam del resolutor

---

### Phase D — Ubicación, dependencias y fuente única del mapeo

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 El reverse en el tooling, sin arista hacia la capa de datos

El componente vive donde el Bolt 003 lo dejó —el **tooling**, junto al shell y al motor (ADR-001 §3.1)— y con la
herencia retirada **no expone ninguna arista de dependencia hacia la capa de datos**: el reporte de dependencias
por ensamblado del árbol 2.0 es la evidencia (y el arnés lo sostiene como comprobación, no como inspección
visual).

#### D.2 La fuente única del mapeo de tipos no se toca

El conversor de tipos que la factory devuelve es el de **`Modeling`** (la fuente única que fijó el Bolt 003 y que
el motor también consume, ADR-001 §3.1/§3.6): este Bolt **no** declara una segunda tabla, **no** reimplementa la
correspondencia y **no** la reubica. La verificación es que el paquete de `Modeling` sigue siendo el único lugar
donde la correspondencia está declarada.

#### D.3 La mecánica del aporte y la obligación vencida (revisión 2)

**a) Política de nulabilidad del código aportado del 1.4.** El árbol 2.0 hereda de `2.0/Directory.Build.props`
(Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`, mientras que el código del 1.4 es **anterior** a
las anotaciones de nulabilidad. Es la trampa **ya medida** en esta ola —`MEM-261016-0107` §8.2 (28 diagnósticos
`CS86xx`), `MEM-261016-0209` §8.1 (`CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`) y
`MEM-261016-0227` §13 (el aviso declarado del Bolt 009)— y afectaría a **este Bolt** en cuanto aporte al tooling los
archivos del reverse del 1.4 (`MSSQLProvider.cs`, el resto de `MSSQLCommon.cs`, la **mitad reverse** de
`DomainTreeHelper`, los `PropertyEditors/*` y lo que el aporte traiga): `CS8618`/`CS8600`/`CS8603`/`CS8625` que el
árbol convierte en **errores**.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
`SPEC-261016-0020` §C.5, `SPEC-261016-0019` §B.2, `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3 y
`SPEC-261016-0037` §B.2):** cada archivo **aportado del 1.4** que **produzca diagnósticos de nulabilidad** empieza
con la directiva `#nullable disable` (primera línea útil, antes de los `using`), y **sólo** esos archivos la llevan
*(revisión 4: los `PropertyEditors/*` **ya no se aportan** — se retiran, §D.3-b —, de modo que la lista de archivos
aportados es la de los tres destinos que sí se materializan y lo que el aporte traiga)*:

- **Sí la llevan:** los archivos **aportados del 1.4** por este Bolt **cuando produzcan**
  `CS8618`/`CS8600`/`CS8603`/`CS8625`. El **criterio es objetivo** —*archivo aportado del 1.4 + diagnóstico de
  nulabilidad*— y la lista exacta la fija la **ejecución con el build**, no un nombre supuesto. La directiva
  **apaga el análisis**: no cambia una instrucción de IL y deja la semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —`Providers/SqlClientSchemaReader.cs` (§B.1), la resolución de
  la cadena (§C.1) y las pruebas—, que conserva la nulabilidad **activa** del árbol porque está escrito para el
  mundo anotado; y queda **prohibido** `<Nullable>disable</Nullable>` en **cualquier** `.csproj`, incluido el del
  tooling: apagaría el análisis del proyecto entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta el build
  **falla** con el diagnóstico exacto; si **sobra** en código nuevo, se ve en la lectura de los archivos entregados.
  Se verifica en §8.1, §9 y AC-9.

**Un `using` que quede sin uso** como consecuencia de una corrección **propia** de este Bolt (el retiro de la
herencia `DataAccess : BaseRule` y de las referencias que la sostenían, §A.2) se retira con ella: es parte de la
forma ya corregida del archivo. Un `using` muerto **hacia un namespace que no existe en el árbol 2.0** que **no**
provenga de una corrección propia **no** está cubierto por esta autorización: se **detiene y se reporta** (§15).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`,
inicializadores o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que la ordene y que
tocaría, de paso, el camino de lectura del esquema cuya **paridad** es el criterio de aceptación de este Bolt;
**diferir el aporte** dejaría al Bolt sin el componente que debe separar de la capa de datos. La directiva, en
cambio, es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no cambia lo que
el reverse lee**: es el mismo criterio con el que se resolvió en los Bolts 003, 007, 009, 008, 013, 014 y 022.

**b) La obligación vencida del Bolt 003 (materialización).** `OBLIGATION-TOOLING-01`
(`2.0/docs/api-removals/US-000.BOLT-003.json`, `status: deferred`) difería la **fase D** —la reubicación del
tooling— «cuando la capa `Data` sea portada». **La capa está portada** (el **BOLT-014** cerró su Development
Completed) y **este Bolt es su materializador**: **tres** de los cuatro destinos de §D.1 quedan en el tooling
(`MSSQLProvider`/`MSSQLReverseFactory`, el resto de `MSSQLCommon.cs` y la **mitad reverse** de `DomainTreeHelper`,
tal como `MIGRATION-PARTITION.md` §2.2 los especifica tipo por tipo) y **el cuarto —los `PropertyEditors/*`— se
RETIRA** (revisión 4). El cierre de la obligación es, por lo tanto, **3 destinos materializados + retiro declarado
del 4.º**.

> **El cuarto destino se RETIRA, no se reubica (revisión 4).** El V-Bounce N°1 lo midió: los `PropertyEditors/*`
> portados **verbatim**, con `UseWindowsForms=true` y el árbol bajo `TreatWarningsAsErrors=true`, hacen que el
> analizador WinForms moderno emita **`WFO1000`** sobre el form portado y el build lo trate como **error**
> (`ToStringInfoEditorForm.cs(19,21)` — `OverrideToString` — y `(31,23)` — `StringFormat` —; `MEM-261016-0506` §8.1).
> Las **dos** salidas quedan fuera de la autoridad del ejecutor —**anotar** el código aportado es la
> **transformación** que §D.3-a descarta; **silenciar** el analizador es una **excepción de gate** y G21 exige un
> ADR— y el propio Bolt 003 **anticipó** este desenlace al diferir la fase D: *«la compilación de `PropertyEditors/*`
> en `net10.0-windows` … el camino es el **retiro por escalamiento** vía condición de parada, **nunca** un shim de
> compatibilidad»* (`MEM-261016-0046` §13). Esta revisión **ordena ese camino**:
>
> - **Se retiran** los `PropertyEditors/*` (los Forms y sus recursos) de la obligación: **no** se reubican al
>   tooling, **no** se anotan, **no** se silencia ningún analizador y **no** se ejecuta ningún shim.
> - **Por qué es correcto retirarlos (y no un recorte de alcance):** la partición del Bolt 003 **ya retiró** los
>   atributos de diseñador del modelo y **eliminó** el acoplamiento modelo→editor (`DomainTree.cs` L1621); en la 2.0
>   la edición es **dirigida por el esquema del contrato** (ADR-001 §3.1, ADR-009 §3.2) y **nada los consume**: su
>   materialización sería darles un vehículo nuevo, que es una decisión de **producto** de la UI nueva (**Bolt 025**).
> - **Ruptura declarada con su reemplazo: cero.** El retiro entra en la constancia para las **notas de migración**
>   (ADR-008 §3.1) con sus tres datos —**qué** desaparece (los editores/Forms y sus recursos, que el Bolt 003 había
>   dejado en el tooling como destino diferido), **por qué** (no compilan bajo el analizador moderno + la edición 2.0
>   es dirigida por contrato; su materialización es decisión de producto) y **con qué se reemplazan: nada**—; el
>   consumidor que necesite edición la obtiene del **editor dirigido por esquema** del Bolt 025, no de un
>   `UITypeEditor`.
> - **El `.csproj` no declara `UseWindowsForms`** y el tooling queda **sin UI**: el vehículo de edición es del 025.

**Los tres recordatorios obligatorios** del §D.3 del SPEC-003, cumplidos por este Bolt:

1. **Calificar las cuatro llamadas cruzadas**: la mitad reverse invoca a la portable como
   `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X` en `CorrectGenerationNamesForRelatedFields` (L184),
   `FindEntityInTree` (L222/L297) y `CorrectSelectedCollections` (L292) — el tooling tiene una clase con el **mismo
   nombre** (`DomainTreeHelper`), así que la calificación es obligatoria (no un estilo).
2. **La lista de rupturas del MEM del Bolt 003 (§13) como insumo de la constancia**: tipos que cambian de
   ensamblado **y** de namespace, atributos de diseñador retirados, `[Serializable]` retirado de los tipos que
   migran, base de las excepciones del grafo en `System.Exception`, el helper partido en dos ensamblados, y **el
   acoplamiento del modelo al editor retirado**; la constancia de la materialización entra en el MEM de este
   V-Bounce con la referencia a `OBLIGATION-TOOLING-01`.
3. **El reporte de dependencias a dos niveles**: el **presupuesto** (proyecto/lock file) y las **aristas
   prohibidas** (**ensamblado**), que es la evidencia de §A.2/§D.1.

**Evidencia de esta fase:** la política aplicada por archivo y verificada por el build (con la lectura de los
archivos entregados), el reporte de dependencias a dos niveles, y la constancia de la obligación materializada.
Se verifica en §8.1, §9, AC-9 y AC-8.

---

### Phase E — La paridad observable del esquema leído (primera prueba de integración del reverse)

**Duration:** 4h total cycle — **Complexity:** High

#### E.1 Qué se compara y contra qué

La verificación del Bolt es de **paridad observable**: contra una base de **esquema conocido**, el modelo que
devuelve el reverse debe ser el mismo que antes del cambio. La comparación es **estructurada** y cubre:

- **entidades** (una por tabla, con su nombre y su `namespace` generado);
- **propiedades**: nombre, tipo **nativo** y tipo **CLR** resueltos por la **tabla de mapeo única**, longitud,
  precisión y escala;
- **roles y opciones**: clave primaria (simple y **compuesta**, con su orden), identidad, `rowversion`,
  anulable, calculado, y las marcas derivadas del nodo;
- **relaciones**: claves foráneas con su tabla destino, nombre de la FK y columnas (con el tratamiento del
  esquema `dbo.` ya conocido);
- **consultas**: el nodo de consultas de la entidad (existe en el modelo aunque no venga de la base);
- **vistas**: leídas **como tablas** —la limitación declarada del modelo (ADR-002 §3.1), que este Bolt
  **no** intenta resolver: la reporta igual que antes.

**El modelo esperado (la referencia) se declara desde el esquema de la base**: el script de aprovisionamiento de
la base de prueba está comiteado junto a las pruebas y de él se derivan las expectativas (entidades, columnas,
tipos, claves y relaciones). Es la forma correcta de fijar el resultado esperado: **desde el insumo y la
intención aprobada** —el contrato del modelo y la tabla de mapeo—, **no** desde el código que se está cambiando
(que sería validar la implementación contra sí misma). Como **corroboración adicional**, el MEM registra el
modelo capturado con el componente **antes** del cambio (en el entorno de integración, o con el build 1.4
congelado si el entorno lo permite); si esa captura no fuera posible, se deja constancia del motivo y **la
referencia declarada desde el esquema sigue siendo la que manda** (§14, a la vista del aprobador).

**Files created:**
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/CooperatorModeler.Reverse.Integration.Tests.csproj` — proyecto de pruebas de integración `net10.0`
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/SchemaProvisioning.sql` — el aprovisionamiento de la base de esquema conocido (tablas con tipos variados, PK simple y compuesta, FK, identidad, `rowversion`, columna anulable y una vista) y el **insumo declarado de las expectativas**
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/SchemaParityTests.cs` — la comparación estructurada de §E.1
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/ViewsAndKeysTests.cs` — vistas como tablas y relaciones por `sp_fkeys`/`sp_help`
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/ConnectionStringResolutionTests.cs` — la cadena desde la configuración portable contra la base real
- `2.0/tests/CooperatorModeler.Reverse.Integration.Tests/SchemaReaderLifecycleTests.cs` — la conexión se crea y se dispone en el componente (ninguna conexión ajena, ninguna abierta al terminar)
- `2.0/tests/CooperatorModeler.Reverse.Tests/SchemaMappingTests.cs` — el armado del modelo desde **tablas de datos en memoria** con las formas esperadas de `sp_help`/`INFORMATION_SCHEMA`/`sp_fkeys` (nivel hermético, §8.1)
- `2.0/tests/CooperatorModeler.Reverse.Tests/ConnectionResolutionTests.cs` — la resolución de la cadena con **dobles** del contrato de configuración y del resolutor (`plain:`), y el error tipado ante un esquema desconocido
- `2.0/tests/CooperatorModeler.Reverse.Tests/DependencyChecks.cs` — las comprobaciones estáticas: 0 usos de `BaseRule`/de la capa de datos, 0 segunda tabla de mapeo, el inventario de consultas de esquema intacto

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: El reverse no hereda de la capa de datos

**Given** el componente del reverse en el tooling
**When** se inspeccionan sus tipos y el reporte de dependencias por ensamblado del árbol 2.0
**Then** **no** existe ninguna clase que herede de la capa de datos (el `DataAccess : BaseRule` desapareció),
**no** hay ninguna arista de dependencia del componente hacia esa capa, y el proyecto no la referencia para
sostener la herencia

### AC-2: La paridad del esquema leído se mantiene

**Given** la base de prueba con el esquema conocido (y su script de aprovisionamiento comiteado)
**When** el reverse lee el esquema y su modelo se compara con el **modelo esperado**
**Then** coinciden en entidades, propiedades (nombre, tipo nativo y CLR por la tabla de mapeo única, longitud,
precisión y escala), claves primarias (incluida la compuesta con su orden), identidad/`rowversion`/anulable/
calculado, y relaciones (tabla destino, FK y columnas), con las vistas leídas como tablas —la limitación
declarada y no resuelta—

### AC-3: El acceso a la base es propio, con el provider único

**Given** el código del reverse
**When** se inspecciona cómo accede a la base
**Then** usa **`Microsoft.Data.SqlClient`** (el provider único de ADR-005 §3.1) con conexión **propia** —creada y
**dispuesta** por el componente, sin usar ni modificar una conexión del llamador**, sin conexiones abiertas al
terminar**— y las consultas de esquema (`sp_help`, `INFORMATION_SCHEMA.TABLES`, `EXEC sp_fkeys`, `EXEC sp_help`)
son **las mismas** de 1.4, en su texto y en la forma de sus resultados

### AC-4: La cadena de conexión sale de la configuración portable

**Given** una cadena provista por la configuración portable
**When** el reverse necesita conectarse
**Then** la obtiene del contrato de configuración (`Cooperator:ConnectionStrings:<nombre>`) con su **prefijo de
esquema**, e **invoca el resolutor** de valores protegidos desde **este** consumidor; **no** hay cadena embebida,
ni duplicada en otro mecanismo, ni inspección de su contenido, y un valor con un esquema que no sea `plain:`
(único implementado hasta el Bolt 022) **falla con error** en lugar de leerse

### AC-5: SQL Server sigue siendo el único motor

**Given** el componente
**When** se revisan sus dependencias y su código
**Then** **no** hay provider alternativo, ni selección de motor, ni ramas por tipo de base: el reverse lee SQL
Server con el provider único (OQ-005)

### AC-6: El conector de tipos sigue siendo la fuente única

**Given** el árbol 2.0
**When** se busca la correspondencia tipo nativo → tipo CLR
**Then** está declarada **una sola** vez, en `Modeling` (la del Bolt 003), el reverse la **consume** y **no**
existe una segunda tabla ni un conversor propio en el tooling

### AC-7: El componente vive en el tooling y el modelo no depende de la capa de datos

**Given** el reporte de dependencias por ensamblado del árbol 2.0
**When** se revisan las direcciones de dependencia
**Then** el reverse vive en el tooling (junto al shell y al motor, ADR-001 §3.1), el **modelo** (`Modeling`) no
tiene ninguna arista hacia la capa de datos, y ninguna arista prohibida por ADR-001 §3.1 aparece

### AC-8: El cambio queda declarado para las notas

**Given** la evidencia del V-Bounce
**When** se revisa la constancia del cambio
**Then** si la superficie del tooling se alteró, está registrada con su reemplazo (ADR-008 §3.1); y queda
declarado el cambio de comportamiento para quien usa el tool: **la conexión ya no se provee por un mecanismo
propio del tool, sino por la configuración portable** —dato que las notas de migración y el Bolt de la UI
(Bolt 025) necesitan— **y, con el retiro de los `PropertyEditors/*` (revisión 4), la constancia incluye esa
ruptura con sus tres datos: qué desaparece (los editores/Forms y sus recursos), por qué (no compilan bajo el
analizador moderno y la edición 2.0 es dirigida por contrato) y con qué se reemplazan (nada: el editor dirigido por
esquema es del Bolt 025)**

### AC-9: El código aportado del 1.4 no rompe el build por nulabilidad

**Given** los archivos del 1.4 que este Bolt aporta al tooling (`MSSQLProvider.cs`, el resto de `MSSQLCommon.cs`,
la **mitad reverse** de `DomainTreeHelper` y lo que el aporte traiga —los `PropertyEditors/*` **no** se aportan:
se retiran, §D.3-b—)
**When** se construye el tooling con advertencias tratadas como error
**Then** no queda **ningún** diagnóstico de nulabilidad: cada archivo **aportado del 1.4** que lo requiera lleva la
directiva de **§D.3** como primera línea útil, **ningún** archivo nuevo la lleva, y **ningún** `.csproj` declara
`<Nullable>disable</Nullable>`

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — el componente del reverse no hereda ni expone arista hacia la capa de datos | §6/A, §6/D.1 | AC-1, AC-7 · reporte de dependencias por ensamblado + `DependencyChecks` |
| Bolt §2 — la cadena de conexión se toma de la configuración portable | §6/C.1 | AC-4 · `ConnectionResolutionTests` (hermético) + `ConnectionStringResolutionTests` (integración) |
| Bolt §2 — lee el mismo esquema y produce el mismo modelo (paridad observable) | §6/E.1 | AC-2 · `SchemaParityTests` + `ViewsAndKeysTests` (integración) |
| Bolt §2 — SQL Server es el único motor, sin provider alternativo | §6/B.1, §6/D.2 | AC-5 · inspección + dependencias |
| ADR-005 §3.6 — acceso con `Microsoft.Data.SqlClient` y la cadena desde la configuración portable | §6/B.1, §6/C.1 | AC-3, AC-4 · código + pruebas de integración |
| ADR-001 §3.1/§3.6 — el reverse en el tooling, el modelo sin aristas hacia `Data`, la fuente única del mapeo | §6/D.1, §6/D.2 | AC-1, AC-6, AC-7 · reporte de dependencias + búsqueda de la tabla |
| ADR-004 §3.2/§3.3 — el valor desde el contrato, con prefijo, y el resolutor invocado por el consumidor | §6/C.1 | AC-4 · resolución con dobles + corrida real |
| ADR-008 §3.1 — constancia del cambio y de las rupturas | §6/E.1 | AC-8 · lista en el MEM para el Bolt 028 |
| Bolt §2 — el código del 1.4 que este Bolt aporta no rompe el build por nulabilidad (mecánica ya medida en los Bolts 003/007/009 y prevenida en 008/013/014/022) | §6/D.3-a: directiva `#nullable disable` **por archivo aportado del 1.4 + diagnóstico** | AC-9 · build del tooling + lectura de los archivos entregados |
| Bolt §2 — la obligación diferida del Bolt 003 (`OBLIGATION-TOOLING-01`) queda **materializada** por este Bolt, con el **cierre 3 destinos + retiro declarado del 4.º** (revisión 4) | §6/D.3-b: los **tres** destinos en el tooling + el **retiro** de los `PropertyEditors/*` con su constancia + los tres recordatorios | AC-1, AC-7, AC-8 · reporte de dependencias a dos niveles + constancia en el MEM + constancia de la ruptura |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests del armado del modelo (el corazón hermético):** el mapeo de **resultados de consulta → modelo** se
  prueba con **tablas de datos en memoria** que reproducen las formas que devuelven `sp_help`,
  `INFORMATION_SCHEMA.TABLES` y `sp_fkeys` (columnas y filas como **datos**, sin base de datos): tipos nativos
  variados con su tipo CLR esperado por la **tabla única**, longitud/precisión/escala, clave primaria simple y
  **compuesta** (con su orden), identidad, `rowversion`, anulable, calculado, claves foráneas con su tabla
  destino y columnas, el tratamiento del esquema `dbo.`, y las vistas tratadas como tablas. Es una prueba
  **real** del mapeo (no un simulacro del servicio): lo que se sustituye es la **ejecución SQL**, no la lógica.
- **Unit tests de la resolución de la cadena:** con un **doble del contrato de configuración** (Bolt 011) y un
  **doble del resolutor** (ADR-007): valor presente con `plain:`, valor **ausente** (error tipado del contrato),
  valor con **esquema desconocido** (fallo explícito, sin leer a ciegas), y verificación de que **este**
  consumidor es el que invoca al resolutor.
- **Comprobaciones estáticas (NFR del Bolt):** 0 usos de `BaseRule` y de cualquier tipo de la capa de datos en el
  componente; 0 referencias del proyecto para sostener la herencia; **0** segundas tablas de mapeo de tipos (la
  correspondencia sólo en `Modeling`); inventario de las consultas de esquema **intacto** (el texto de
  `sp_help`/`INFORMATION_SCHEMA`/`sp_fkeys` no cambió).
- **Ciclo de vida de la conexión (unitario, con el lector sustituido):** el componente crea su conexión y la
  dispone al terminar —y en el camino de error—, y **no** toca conexiones provistas por otro (no las hay: su API
  no las acepta).
- **Consumer-side contract tests (test doubles):** **sí aplica** en los dos contratos que este componente
  consume: el **contrato de configuración** (Bolt 011) y el **resolutor de valores** (ADR-007, aún sin
  implementación). Dobles para ambos permiten probar que el componente los usa **como contratos** y no
  implementa su propia lectura de configuración ni su propia interpretación del valor.
- **Política de nulabilidad del código aportado (§D.3-a, revisión 2):** chequeo por **lectura** de los archivos
  entregados —cada archivo **aportado del 1.4** que produzca diagnósticos de nulabilidad empieza con
  `#nullable disable` y **ningún** archivo nuevo la lleva—, más el propio `build` con
  `TreatWarningsAsErrors=true` (la prueba real: sin la directiva, el build falla con
  `CS8618`/`CS8600`/`CS8603`/`CS8625`). Se verifica además que **no** hay `<Nullable>disable</Nullable>` en ningún
  `.csproj` y que ningún `using` muerto hacia un namespace inexistente quedó sin resolver ni autorizado.
- **Materialización de la obligación del Bolt 003 (§D.3-b, revisión 2; cierre 3+retiro en la revisión 4):**
  verificación de que los **tres destinos** que se materializan están en el tooling (`MSSQLProvider`/
  `MSSQLReverseFactory`, el resto de `MSSQLCommon.cs` y la mitad reverse de `DomainTreeHelper`), de que **los
  `PropertyEditors/*` NO están** (retirados: no hay Forms ni `UseWindowsForms` en el proyecto), de que la **mitad
  reverse** califica sus **cuatro llamadas cruzadas** como
  `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X` (L184, L222/L297, L292), de que la **constancia de la
  obligación** (referencia a `OBLIGATION-TOOLING-01` y su **estado final: 3 destinos + retiro declarado del 4.º**)
  está en el MEM, y de que el retiro está en la **constancia de rupturas** con su reemplazo (cero).
- **Edge cases:** tabla sin columnas legibles; tabla con todas las columnas nulas (tipos nativos nulos);
  clave primaria compuesta de tres columnas en orden no alfabético; FK con `PKTABLE_OWNER` distinto de `dbo.`
  (el prefijo se conserva); vista con nombre que colisiona con una tabla; nombres de tabla con caracteres que el
  T-SQL de `sp_help` necesita citar; esquema vacío (sin tablas ni vistas) ⇒ modelo vacío válido; tipo nativo no
  presente en la tabla de mapeo ⇒ error tipado del contrato de tipos (no una coerción silenciosa);
  cadena ausente y cadena con prefijo desconocido (los dos ya cubiertos arriba); conexión que falla al abrir ⇒
  error explícito del provider, sin dejar recursos abiertos.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida de los tests del mapeo con las tablas en memoria, la de la
  resolución de la cadena con dobles, las comprobaciones estáticas (herencia ausente, tabla única, consultas
  intactas), el **reporte de dependencias por ensamblado** y la lista de cambios de comportamiento para las
  notas. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "applicable"` porque el Bolt declaró `test_integration: required`
> (§3.2.2). Los tests se **escriben, construyen y verifican estáticamente** en este V-Bounce y se **ejecutan** en
> el ciclo de test de integración del Bolt, **después** de `AITL-MEM-Approval`; el resultado queda como **hecho**
> en `integration_test_runs[]` del manifiesto (no hay checkpoint de integración, §3.2.2). **Es la primera prueba
> de integración del reverse**: en 1.4 no tenía ninguna.

- **Integration tests (real dependencies — SQL Server):**
  - `SchemaParityTests` — con la base de esquema conocido aprovisionada por `SchemaProvisioning.sql`, el reverse
    lee el esquema y el modelo resultante se compara **estructuradamente** contra el modelo esperado (§E.1):
    entidades, propiedades con tipo nativo/CLR, claves (incluida la compuesta y su orden), identidad,
    `rowversion`, anulable, calculado y relaciones. Es el criterio de paridad del Bolt.
  - `ViewsAndKeysTests` — las vistas leídas **como tablas** (la limitación declarada, verificada como
    comportamiento esperado y no como defecto) y las relaciones resueltas por las rutas de `sp_fkeys`/`sp_help`,
    incluido el tratamiento del esquema `dbo.`.
  - `ConnectionStringResolutionTests` — la cadena tomada de la configuración portable (contrato del Bolt 011)
    contra la base real, con el prefijo aplicado y el resolutor invocado por este consumidor; se verifica además
    que el componente **no** tiene otro mecanismo de obtención de la cadena.
  - `SchemaReaderLifecycleTests` — la conexión la crea y la dispone el componente: la corrida no deja conexiones
    abiertas y ningún objeto de conexión provisto por otro participa.
  - **Quién los escribió:** este V-Bounce. El arnés (Bolt 001) aporta el proyecto, la lane y la resolución de
    assets por runtime.
- **Contract / E2E against the real providers:** el contrato real que se ejerce es el de **SQL Server** a través
  del provider único: la corrida de arriba **es** el E2E del componente contra su dependencia real. El contrato
  de configuración se ejerce con un doble en el nivel hermético y, en integración, con la configuración del
  entorno (que es lo que el host haría).
- **Edge cases (los que sólo pueden verificarse contra la base):** una base con el esquema completo (tablas,
  PK compuesta, varias FK, vista) y una **base vacía**; permisos de sólo lectura del usuario de prueba (el
  reverse debe funcionar con lectura y **fallar de forma explícita** si le faltara el permiso, sin escribir
  nunca); nombres de tabla con mayúsculas/acentos; una tabla sin clave primaria (el modelo debe representarlo
  como lo hacía 1.4); la respuesta del servidor ante un esquema distinto de `dbo.`.
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.
- **Cierre del plan antes de la aceptación (§3.2.2):** una **corrida cubriente que pasa** sobre el commit del
  Bolt, registrada en `integration_test_runs[]`. Si el entorno no está disponible, el Bolt queda **bloqueado en
  su ciclo** (no pasa a `ready-for-acceptance`) y el MEM lo registra; **no** se declara el plan satisfecho sin
  corrida (y una exención por `waived` exigiría un ADR de excepción de gate, G21).

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* se declaran **`pass`** porque el plan de integración es
> `applicable` y su corrida es la que cierra el Bolt (§8.2).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); el tooling compila sin la referencia a la capa de datos y **sin diagnósticos de nulabilidad** (la directiva de §D.3-a, sólo en los archivos aportados del 1.4 que la requieren) | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluidos el mapeo con tablas en memoria, la resolución de la cadena y las comprobaciones estáticas | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | Dobles del contrato de configuración (Bolt 011) y del resolutor de valores (ADR-007) verifican que el componente los usa **como contratos** | pass |
| SAST | V-Bounce | 0 hallazgos altos; 0 concatenación de valores de entrada del usuario en el T-SQL de lectura (las consultas son las de 1.4 y los nombres interpolados provienen de los resultados del catálogo) | pass |
| Dependency scanning / licenses | V-Bounce | El componente usa el provider único ya autorizado y **ninguna** dependencia nueva; el tooling no toma la capa de datos para esta función (ADR-001 §3.4) | pass |
| OWASP Top 10 — static review | V-Bounce | Inyección: no se agrega ninguna entrada externa al camino (los nombres de objeto provienen del catálogo de la propia base) y el texto de las consultas no cambia; acceso: usuario de **sólo lectura** y operaciones de lectura únicamente; credenciales: fuera del código y de los mensajes | pass |
| Prompt-injection scan | V-Bounce | `n/a` — el componente no interpreta contenido textual como instrucción; lee catálogo de esquema | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** cadenas de conexión en el código, en los tests (usan variables de entorno) y en la documentación; 0 credenciales embebidas en el tooling | pass |
| Hallucination lint | V-Bounce | Cada pieza retirada y cada consulta conservada trazable al código 1.4 citado; 0 tablas de mapeo nuevas; 0 campos del modelo inventados para "mejorar" la paridad | pass |
| IP / license provenance | V-Bounce | Código propio; 0 dependencias nuevas más allá del provider ya autorizado | pass |
| PII / DLP | V-Bounce | `internal`: se lee **estructura** de una base de prueba del entorno, sin datos de negocio ni personales; las expectativas no contienen datos de filas | pass |
| Dependency-confusion | V-Bounce | 0 paquetes nuevos; ningún ID sin calificar | pass |
| Test-first evidence | V-Bounce | El **modelo esperado** se declara **desde el esquema** (`SchemaProvisioning.sql`) y la tabla de mapeo del Bolt 003 **antes** de tocar el código: las expectativas no salen de la implementación que se cambia | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la suite hermética dan el mismo resultado; la corrida de integración es reproducible con la misma base aprovisionada | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | Corrida **cubriente que pasa** contra SQL Server real (paridad del esquema, vistas y claves, cadena desde la configuración, ciclo de vida de la conexión); registrada en `integration_test_runs[]` | pass |
| Contract / E2E against the real providers | Integration test | La misma corrida ejerce el contrato de SQL Server a través del provider único: es el E2E del componente contra su dependencia real | pass |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — no hay superficie HTTP ni servicio expuesto: el reverse es un componente de tooling que se ejecuta en el proceso del host | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | `n/a` — el Bolt no fija un objetivo de rendimiento; la lectura de esquema es una operación de tooling y su costo es el del servidor. El control de recursos que **sí** aplica es el ciclo de vida de la conexión (AC-3) | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | `n/a` — el componente no agrega telemetría (§11); su diagnóstico son los errores del provider y los del contrato | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). El componente lee **metadatos de
  esquema** de una base del entorno y usa una **cadena de conexión** de la configuración: información interna,
  sin datos personales y sin datos de negocio (no lee filas).
- **Credenciales fuera del código:** la cadena viene de la configuración portable (contrato del Bolt 011) y, en
  la corrida de integración, del **entorno**; **no** se embebe en el tooling, **no** se duplica en un mecanismo
  propio, **no** se registra y **no** aparece en mensajes. El **valor** llega crudo con su prefijo y la
  **protección** (esquemas, algoritmo, claves) es del ADR-007: acá sólo se invoca el **seam** del resolutor, y
  hasta que exista `plain:` es la única condición implementada.
- **Acceso de sólo lectura:** el reverse ejecuta **consultas de catálogo** (`sp_help`, `INFORMATION_SCHEMA`,
  `sp_fkeys`); no escribe, no crea objetos, no ejecuta DDL ni DML. La recomendación de despliegue —usuario de
  sólo lectura— queda escrita, y la corrida de integración usa uno.
- **Sin superficie de inyección nueva:** el texto de las consultas no cambia y **no** se incorpora entrada del
  usuario a su construcción; los nombres de objeto que se interpolan provienen de los **resultados del catálogo**
  de la propia base, y donde el provider permite parametrizar, se parametriza.
- **Ciclo de vida de la conexión:** la conexión la crea y la dispone el componente; no se aceptan conexiones
  ajenas (el hazard equivalente en la capa de datos se corrigió en el Bolt 014). Esto evita fugas y efectos
  cruzados con la capa de datos.
- **El retiro de la herencia reduce superficie:** el tooling deja de arrastrar —para esta función— el modelo de
  configuración, caché y transacciones de la capa de datos, y con él sus puntos de fallo.
- **Sin datos en la evidencia:** las expectativas del modelo describen **estructura** (nombres, tipos, claves),
  no filas; el script de aprovisionamiento no contiene datos de negocio.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es un **componente de tooling** (no un servicio): no emite
  logs, métricas ni trazas a ningún colector y no se le agrega instrumentación (ninguna fuente lo pide, y el
  reverse no tiene ciclo de vida que observar más allá de una lectura de esquema).
- **La observabilidad de este Bolt es su evidencia:** la **corrida de paridad** contra la base real (con la
  comparación estructurada y su resultado), el **reporte de dependencias por ensamblado** (la ausencia de la
  arista hacia la capa de datos), la verificación de las **consultas conservadas** y de la **fuente única** del
  mapeo, y la constancia del **ciclo de vida de la conexión**.
- **Diagnóstico en runtime:** los errores del provider (`SqlException`) y los del contrato de configuración
  (clave ausente, prefijo desconocido) se propagan **explícitos**: el componente no los envuelve en mensajes
  genéricos ni los degrada a un modelo vacío (un esquema que no se puede leer **no** es un esquema sin tablas).
- **Observabilidad del entorno de integración:** la corrida queda registrada como hecho en
  `integration_test_runs[]` (entorno, commit cubierto y evidencia), que es cómo se audita el ciclo sin checkpoint
  propio (§3.2.2).

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos: el reverse **lee** esquema. Lo que sí cambia para quien usa el
  tool es **de dónde sale la conexión**: el componente ya no la recibe por un mecanismo propio del tool, sino de
  la **configuración portable** (contrato del Bolt 011). Ese cambio de comportamiento se declara en el MEM para
  las **notas de migración** (documento del Bolt 028, ADR-008 §3.1) y se coordina con el **Bolt 025**, dueño de
  la UI del shell: la pantalla y la experiencia de "conectar" son suyas, no de este Bolt.
- **Compatibility:** el cambio es **interno al tooling** (una clase privada heredada desaparece; el contrato del
  modelo y sus fixtures no se tocan). Si la superficie del tooling se alterara —por ejemplo por la firma que
  entrega la cadena—, la ruptura se registra con su reemplazo (ADR-008 §3.1). Para el **modelo** y el **formato**
  no hay ruptura: la paridad del esquema leído es precisamente el criterio de aceptación (AC-2), y los fixtures
  del Bolt 002/004 siguen siendo válidos.
- **Relación con el corpus:** este Bolt **no** ejecuta el batch ni convierte modelos (Bolt 029); su paridad es
  sobre el **esquema leído**, no sobre archivos `.coop`.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido, ni datos escritos, ni artefactos del
  consumidor reescritos; la base de prueba es de integración y se re-aprovisiona desde su script. El árbol `1.4/`
  (con su `DataAccess : BaseRule`) sigue congelado y es la contención soportada si algo hubiera que mirar.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Pérdida de paridad en el esquema leído** (un tipo, una clave o una relación se leen distinto y el modelo cambia en silencio) | 3 | 4 | **Criterio de aceptación** con comparación **estructurada** contra el modelo esperado (declarado desde el esquema), cubriendo tipos, claves —incluida la compuesta y su orden—, identidad/`rowversion`/anulable/calculado, relaciones y vistas; corrida contra base real |
| **Dependencia residual hacia la capa de datos** (una referencia del proyecto o un `using` sobrevive) | 3 | 4 | Comprobaciones estáticas (0 `BaseRule`, 0 tipos de la capa) + **reporte de dependencias por ensamblado** como evidencia, sostenido por el arnés y no por inspección visual |
| **Reutilizar la capa de datos "por conveniencia"** al retirar la herencia (por ejemplo para obtener la cadena o para ejecutar SQL) | 3 | 4 | El Bolt lo prohíbe explícitamente: si el reverse necesitara algo de la capa, es **señal de diseño** y **condición de parada** (§15), no una excepción |
| **La cadena de conexión se embebe o se duplica** en el tooling (por comodidad o como "default") | 2 | 5 | AC-4: el valor sólo entra por el contrato; búsqueda de cadenas en el código y en los tests + gate de secreto; el escenario de la corrida usa variables de entorno |
| **Valor con un esquema que no es `plain:`** y el resolutor (Bolt 022) todavía no existe | 3 | 3 | Frontera declarada: `plain:` es la única condición implementada hasta ese Bolt; cualquier otro esquema **detiene** con error (nunca se lee a ciegas) y se reporta |
| **El contrato de configuración (Bolt 011) no está en el árbol 2.0** | 3 | 3 | Prerrequisito con **condición de parada**: la salida es una enmienda de dependencia o una decisión de orden, nunca un mecanismo propio de configuración |
| **Fuga de conexiones o uso de una conexión ajena** | 3 | 3 | El componente crea y dispone su conexión (AC-3) y su API no acepta conexiones; el ciclo de vida se verifica en unitario (camino feliz y de error) y en integración |
| **Las consultas de esquema se "modernizan" de paso** (cambio no pedido que altera lo leído) | 3 | 3 | Inventario de consultas **intacto** como comprobación estática; un cambio de T-SQL es del Bolt del set de plantillas (ADR-003), no de este Bolt |
| **Aparece una segunda tabla de mapeo de tipos** en el tooling al reescribir el lector | 2 | 4 | AC-6 y búsqueda de la correspondencia: una sola declaración en `Modeling` (ADR-001 §3.1/§3.6) |
| **El componente se corre a la UI** (pantalla de conexión, mensajes al usuario) | 3 | 2 | §4/§12: la UI es del Bolt 025; este Bolt entrega el componente y declara el cambio de comportamiento para que su Bolt lo muestre |
| **La base de esquema conocido no está disponible** para la corrida (o no tiene las formas necesarias) | 3 | 3 | `SchemaProvisioning.sql` comiteado con las pruebas (tablas, PK compuesta, FK, identidad, `rowversion`, anulable, vista); si el entorno no está, el Bolt **no** cierra su ciclo (§15) |
| **El modelo esperado se deriva de la implementación** (validar el cambio contra sí mismo) | 2 | 4 | §E.1: las expectativas se declaran **desde el esquema** y la tabla de mapeo aprobada **antes** de tocar el código; la captura previa es corroboración, no la referencia |
| **Falta de pruebas propias del reverse** (histórico: en 1.4 no tenía) | 3 | 3 | Esta es su **primera prueba de integración** y nace con la separación: el mapa de la fase E es el piso de cobertura, no el techo |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8600`/`CS8603`/`CS8625` | 3 | 3 | §D.3-a la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-9 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |
| **La obligación del Bolt 003 se materializa a medias** (los tres destinos no quedan completos, las llamadas cruzadas sin calificar, el retiro sin declarar, o la constancia sin registrar) | 2 | 4 | §D.3-b fija los **tres destinos materializados + el retiro declarado del 4.º** y los **tres recordatorios** como evidencia verificable (reporte de dependencias a dos niveles + constancia en el MEM con la referencia a `OBLIGATION-TOOLING-01` y su estado final); AC-1/AC-7/AC-8 lo miden |
| **Se "recuperan" los editores por la puerta de atrás** (anotar el form aportado para que compile, o silenciar el analizador, en lugar de retirarlos) | 2 | 4 | §D.3-b lo ordena como **retiro** y §15 lo cierra: anotar es la transformación que §D.3-a descarta y silenciar el analizador es una **excepción de gate** (G21 exige ADR, nunca una decisión del ejecutor); el vehículo de edición es del **Bolt 025** |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-005/ADR-001/ADR-004; acá se
> aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) La referencia de la paridad es el modelo esperado declarado desde el esquema (el script de aprovisionamiento) y la tabla de mapeo aprobada; la captura del modelo previo es corroboración** | Lo que se compara debe salir del **insumo y de la intención aprobada** (el esquema y el contrato del modelo), no del código que se está cambiando: derivar el esperado de la implementación sería validar el cambio contra sí mismo. La captura previa (con el componente anterior o con el build 1.4) se registra en el MEM como corroboración cuando el entorno lo permite. **Alternativa:** exigir la captura previa como referencia obligatoria —implica correr el componente anterior contra la base antes del cambio—; si el aprobador la prefiere, se agrega como paso del ciclo de integración |
| **(⚠ aprobador) El lector propio se implementa como un componente del tooling (`SqlClientSchemaReader`) que devuelve los mismos conjuntos de datos que consumía el armado del modelo** | Es la forma más conservadora de conservar la paridad: **no** se reescribe el armado del modelo ni se "modernizan" las consultas; se cambia **quién** ejecuta y **cómo** se accede (provider propio, conexión propia), manteniendo el texto de las consultas y la forma de los resultados. Si el aprobador prefiriera un rediseño del lector (por ejemplo consultas nuevas a las vistas de catálogo), sería un cambio funcional y exigiría otro criterio de paridad |
| La conexión la crea y la dispone **el componente** | Es un lector autónomo (ADR-005 §3.6): no necesita una conexión del llamador y aceptarla reintroduciría el hazard que el Bolt 014 corrigió en la capa de datos. Además hace verificable el ciclo de vida (AC-3) |
| El **seam del resolutor de valores** se invoca en **este** consumidor | ADR-004 §3.3 lo dice explícitamente ("la capa que lo consume —`Data` para connection strings, **el tooling para el reverse**— es la que invoca a `IProtectedValueResolver`"); ponerlo en la configuración sería darle al contrato un conocimiento que no tiene |
| La **configuración de la aplicación** (archivo/entorno y su precedencia) no se implementa acá | El contrato es del Bolt 011 y el bootstrap de las apps generadas, del Bolt 021; el tooling **consume** el contrato, y el shell (Bolt 025) es quien arma la suya |
| Las **vistas leídas como tablas** se conservan tal cual y se verifican como **comportamiento esperado** | Es el límite declarado del modelo 1.4 (ADR-002 §3.1): distinguirlas exigiría una **capacidad nueva** del modelo (un campo de clase) y un ADR que suba `schemaVersion`. Este Bolt **reporta** la limitación, no la resuelve |
| El **armado del modelo** se prueba hermeticamente con **tablas de datos en memoria** | La lógica a preservar es el mapeo de resultados → modelo; sustituir la **ejecución SQL** por datos construidos es exactamente el nivel hermético correcto (no hay servicio real involucrado), y deja la corrida real para el ciclo de integración |
| Los tests de integración **declaran** el aprovisionamiento de su base en un script comiteado | Es lo que hace la corrida reproducible y lo que permite derivar las expectativas desde el esquema (primera fila de esta tabla); una base "que ya estaba ahí" no documenta qué se esperaba leer |
| No se toca el **`DomainTreeHelper`** más allá de la cadena | Su semántica de árbol (armar el grafo desde el esquema, fusionar con el árbol previo) **no** cambia con la separación: el Bolt separa la herencia y el acceso, no reescribe la construcción del modelo |
| El **reporte de dependencias** se sostiene como comprobación del arnés, no como inspección puntual | La arista prohibida debe **fallar** en cada corrida futura, no depender de que alguien mire un diagrama una vez (mismo criterio que el Bolt 003 aplicó a `Modeling`) |
| **(⚠ aprobador) Política de nulabilidad del código aportado del 1.4: `#nullable disable` por archivo aportado que lo requiera** (revisión 2) | Es la trampa **ya medida** tres veces en esta ola (Bolts 003/007/009) y **este Bolt aporta archivos del 1.4 al tooling**. El mecanismo es el **espejo** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0020` §C.5, `SPEC-261016-0019` §B.2, `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3 y `SPEC-261016-0037` §B.2: preserva la semántica del 1.4 (apaga el análisis, **no cambia el IL**), es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no cambia lo que el reverse lee**. Se **descartan**, declaradas, las alternativas de **anotar/corregir** el código aportado (transformación no ordenada, y en el camino cuya **paridad** es el criterio de aceptación) y de **diferir** el aporte (dejaría al Bolt sin el componente a separar) |
| **(⚠ aprobador) Este Bolt es el materializador de la obligación vencida `OBLIGATION-TOOLING-01` del Bolt 003, y la cierra como 3 destinos materializados + RETIRO declarado del 4.º** (revisión 2; cierre en la revisión 4) | La obligación difería la **fase D** «cuando la capa `Data` sea portada», y **la capa ya está portada** (el BOLT-014 cerró su Development Completed): la condición **se cumplió** y la materialización le corresponde a este Bolt —que es el que separa el reverse de la capa de datos—, con sus **tres recordatorios obligatorios** (calificar las cuatro llamadas cruzadas; la lista de rupturas como insumo; el reporte de dependencias a dos niveles). Los **`PropertyEditors/*`** se **retiran** en lugar de reubicarse (§D.3-b, revisión 4): portados verbatim **no compilan** (`WFO1000` + `TreatWarningsAsErrors`) y anotarlos/silenciar el analizador están **vetados**; el Bolt 003 **anticipó** ese camino («retiro por escalamiento … **nunca** un shim»). La constancia del cierre entra en el MEM (§D.3-b, §17) |
| **El `turn_budget` se fija explícitamente en `25`** (revisión 3), en lugar de correr el V-Bounce N°1 con el default de la plataforma (10) | La **unidad restante** es grande: la **reubicación del tooling** (los cuatro destinos de `OBLIGATION-TOOLING-01` — `MSSQLProvider.cs`/`MSSQLReverseFactory`, el resto de `MSSQLCommon.cs`, la **mitad reverse** de `DomainTreeHelper` y los `PropertyEditors/*` con sus recursos—), el **lector propio** `SqlClientSchemaReader`, la **resolución de la cadena** desde el contrato con el seam del resolutor, la **suite hermética** y la **primera prueba de integración del reverse**; y §D.3-a exige ciclos **diagnóstico → directiva → build** que consumen turnos. Es **evidencia medida**, no una estimación: el **default (10)** se agotó en el **Bolt 009** (dos veces) y en el **Bolt 014**, y con **`turn_budget: 25`** (`SPEC-261016-0037` rev.3 y `SPEC-261016-0020` rev.4) esos Bolts completaron su unidad —en una pasada y en casi dos—. Bloquear a mitad de la reubicación **desperdiciaría el inventario ya medido** (`MEM-261016-0343` §9.1: los cinco archivos, los cuatro puntos de caché y las refs internas 18/26/21) y dejaría un árbol intermedio. El parámetro se declara en el frontmatter, se documenta en §3 y se refleja en §16; su cambio es por revisión material |

---

## 15. Stop conditions

- **El contrato de configuración (Bolt 011) no está en el árbol 2.0** al iniciar el V-Bounce: **parar y
  reportar**. La salida es una enmienda del Bolt o una decisión de orden del dueño; **nunca** un mecanismo propio
  de configuración ni una cadena provista por otro medio.
- **La cadena llega con un esquema que no es `plain:`** y el resolutor del ADR-007 (Bolt 022) no existe: parar y
  reportar. **No** se lee el valor a ciegas, **no** se asume texto plano y **no** se implementa un descifrado
  propio: la protección es de ese Bolt.
- **Al retirar la herencia el reverse "necesita" algo de la capa de datos**: parar y reportar. Es una **señal de
  diseño** a revisar (el Bolt lo declara como prohibición explícita), no una excepción de alcance.
- **La paridad no se puede establecer** (no hay base de esquema conocido, no hay script de aprovisionamiento, o
  el modelo esperado no puede declararse desde el esquema): parar. Sin referencia legítima no hay criterio de
  aceptación, y fabricar el esperado desde la implementación está prohibido.
- **La corrida de integración no se puede ejecutar** (sin SQL Server o sin lane): el V-Bounce puede cerrar con
  los tests escritos y verificados estáticamente, pero el Bolt **no** pasa a `ready-for-acceptance`: queda en su
  ciclo de integración y el MEM registra el bloqueo. **No** se declara el plan satisfecho ni se pide un waiver
  sin ADR (G21).
- **El proyecto del tooling no existe** (el host que el Bolt 003 creó) o la referencia a la capa de datos no se
  puede retirar porque otro componente del mismo proyecto la necesita: parar y reportar; la frontera con los
  Bolts 003/025 se resuelve por contrato de entregables, no por omisión.
- **Aparece la tentación de "mejorar" el lector** (consultas nuevas, rediseño del mapeo, distinguir vistas de
  tablas, resolver F-34/el enumerador de servidores): parar. Eso es un cambio funcional con otro criterio de
  aceptación (y, en el caso de las vistas, un ADR que suba `schemaVersion`); acá se conserva la paridad.
- **Un tipo nativo de la base no está en la tabla de mapeo** y aparece la idea de agregarlo "de paso": parar y
  reportar; la tabla es la **fuente única** del Bolt 003 y su evolución pasa por su dueño (o por un ADR), no por
  este Bolt.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.
- **Alguien extiende la autorización de §D.3-a**: poner la directiva `#nullable disable` en **código nuevo** (el
  lector propio, la resolución de la cadena o las pruebas) o `<Nullable>disable</Nullable>` en un `.csproj`; o
  intentar resolver con esa misma autorización un `using` muerto **hacia un namespace que no existe en el árbol
  2.0** que **no** provenga de una corrección propia. Parar y reportar: la autorización es **una** y está acotada
  en §D.3-a.
- **Al aportar los `PropertyEditors/*` aparece `WFO1000`** (o cualquier otro analizador de UI) y alguien propone
  **anotar** el form aportado para que compile, o **silenciar** el analizador (`NoWarn`, retiro del analizador,
  retiro del TFM/`UseWindowsForms` "para que pase"): **parar**. El camino **ordenado** para ese destino es el
  **RETIRO** (§D.3-b, revisión 4) —los editores **no** se reubican, **no** se anotan y **no** se silencia ningún
  analizador—; anotar es la transformación que §D.3-a descarta y silenciar el analizador es una **excepción de gate**
  (G21: exige un ADR aprobado por `AITL-ADR-Approval`, **nunca** una decisión del ejecutor). El **vehículo de
  edición** de la 2.0 (editor dirigido por esquema) y la suerte del editor nuevo son del **Bolt 025**.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented **en un solo V-Bounce**: el SPEC fija **`turn_budget: 25`** para la unidad restante (§3/§14) —o, si aun así el presupuesto se agota, progreso en el MEM y pendiente explícito para un V-Bounce nuevo—
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG**
- [ ] Code follows applicable ADRs (ADR-005 §3.1/§3.6, ADR-001 §3.1/§3.4/§3.6, ADR-004 §3.2/§3.3)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] La directiva de nulabilidad de **§D.3-a** aplicada **sólo** a los archivos aportados del 1.4 que la requieren, y verificada (lectura + build); ningún `.csproj` con `<Nullable>disable</Nullable>` y ningún `using` muerto sin resolver ni autorizado
- [ ] La **obligación `OBLIGATION-TOOLING-01`** del Bolt 003 **materializada y cerrada** (§D.3-b, revisión 4): los **tres destinos** en el tooling (`MSSQLProvider`/`MSSQLReverseFactory`, el resto de `MSSQLCommon.cs` y la mitad reverse de `DomainTreeHelper`), los **`PropertyEditors/*` RETIRADOS** (no aportados: sin Forms ni `UseWindowsForms`), las **cuatro llamadas cruzadas calificadas**, la lista de rupturas como insumo y el **reporte de dependencias a dos niveles**; y la **constancia** del cierre (**3 destinos + retiro declarado del 4.º**) en el MEM, más la **constancia de la ruptura** del retiro con su reemplazo (cero) para las notas del Bolt 028
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: **corrida cubriente que pasa** registrada en `integration_test_runs[]` del manifiesto (Bolt `test_integration: required`, plan `applicable`); sin corrida, el Bolt permanece en su ciclo de integración y no pasa a `ready-for-acceptance`

---

## 17. References

- [`US-000.BOLT-016`](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-29 (el reverse hereda de `BaseRule` y acopla `Utility` a `Data`) y §6 ítem 5.
- [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) — §3.6 (el reverse deja de heredar, usa `Microsoft.Data.SqlClient` directamente y toma la cadena de la configuración portable; `Modeling` mantiene su independencia) y §3.1 (provider único, asset/línea y smoke).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.1 (mapa de paquetes, direcciones permitidas, `Modeling` sin aristas hacia `Data`, el reverse en el tooling), §3.6 (frontera y fuente única del mapeo de tipos), §3.4 (presupuesto de dependencias), §5 (NFRs de portabilidad).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato de configuración y catálogo: la clave de la cadena) y §3.3 (prefijo de esquema explícito, error tipado, y quién invoca al resolutor de valores protegidos).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, lane de integración y reporte de dependencias por ensamblado (prerrequisito, Bolt 001).
- [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) — el modelo portable, la tabla de mapeo única y la relocalización del reverse en el tooling (prerrequisito, Bolt 003).
- [`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md) — el provider único y el interior tipado de la capa de datos (prerrequisito, Bolt 014).
- [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) — el contrato de configuración y su catálogo (prerrequisito, Bolt 011).
- [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) · [`US-000.BOLT-025`](../functional/bolts/US-000.BOLT-025-shell-wpf.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) · [`US-000.BOLT-029`](../functional/bolts/US-000.BOLT-029-batch-real-del-corpus.md) — el resolutor de valores (frontera de esta SPEC), la UI del shell (dueña de la pantalla de conexión), el documento de notas y el batch del corpus (fuera de alcance).
- [`OQ-005`](../analysis/open-questions/OQ-005-alcance-features-en-riesgo.md) — SQL Server es el único motor: sin provider alternativo ni motores de reverse adicionales.
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** · [`SPEC-261016-0019`](./SPEC-261016-0019-cache-en-data.md) **§B.2** · [`SPEC-261016-0020`](./SPEC-261016-0020-provider-de-datos.md) **§C.5** · [`SPEC-261016-0034`](./SPEC-261016-0034-resolucion-de-tipos-calificada.md) **§B.4** · [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) **§E.3** · [`SPEC-261016-0037`](./SPEC-261016-0037-library-portable.md) **§B.2** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado por **revisión material** con su medición (`MEM-261016-0107` §8.2: 28 diagnósticos `CS86xx`; `MEM-261016-0209` §8.1: `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`; `MEM-261016-0227` §13: el aviso declarado del Bolt 009). Es el mecanismo que esta revisión **espeja**, **antes** de que el aporte del reverse lo requiera.
- **Obligación materializada y cerrada (revisión 2; cierre en la revisión 4):** `OBLIGATION-TOOLING-01` en `2.0/docs/api-removals/US-000.BOLT-003.json` (`status: deferred`, `materialized_by`/`materially_satisfied_by: US-000.BOLT-014`, `owner_bolts: [014, 016]`) y su especificación tipo por tipo en `2.0/docs/MIGRATION-PARTITION.md` §2.2 — la **fase D** diferida del **Bolt 003** («cuando la capa `Data` sea portada»): **la capa ya está portada** (el **BOLT-014** cerró su Development Completed) y **este Bolt la materializa** (§D.3-b), con los **tres recordatorios obligatorios** del §D.3 del SPEC-003. **Cierre: 3 destinos materializados + retiro declarado del 4.º** (los `PropertyEditors/*`, §D.3-b revisión 4).
- **El aviso que el Bolt 003 dejó para este destino (revisión 4):** `MEM-261016-0046` §13 — «la compilación de `PropertyEditors/*` en `net10.0-windows` (`System.Design` / `IWindowsFormsEditorService`) … el camino es el **retiro por escalamiento** vía condición de parada, **nunca** un shim de compatibilidad»: es exactamente el camino que esta revisión ordena.
- **Evidencia del bloqueo que motivó la revisión 4:** [`MEM-261016-0506`](../memory/MEM-261016-0506-reverse-sin-baserule.md) — el V-Bounce N°1 `blocked`: los tres destinos materializados con sus mediciones, el `WFO1000` medido (§8.1), los tres recordatorios cumplidos (§9.3), el estado `partially-materialized` de la obligación (§9.2), la ruptura de superficie del tooling (§8.5) y las dos opciones escaladas (§13.1).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-016`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, inventario de lo que se retira y de lo que se conserva (la herencia `DataAccess : BaseRule` y las cuatro rutas de lectura del esquema), estrategia de pruebas con el **plan de integración `applicable`** declarado (tests, entorno y datos), la referencia de paridad declarada desde el esquema, gates con las filas de integración en `pass`, control de datos `internal` y condiciones de parada. Declara las fronteras con los Bolts 003/004 (modelo), 014 (provider), 022 (resolutor) y 025 (UI). |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16)** cursada **antes** del V-Bounce N°1, con dos cosas y nada más: **(1) la política de nulabilidad preventiva del código aportado** (§D.3-a, nueva) —`#nullable disable` como primera línea útil **sólo** en los archivos **aportados del 1.4** que **produzcan** `CS8618`/`CS8600`/`CS8603`/`CS8625`, **jamás** en código nuevo, **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**, con el límite del `using` muerto hacia un namespace inexistente que no provenga de una corrección propia (§15)—, espejo de los **siete** precedentes ya aprobados; y **(2) la constancia de que la obligación del Bolt 003 está VENCIDA y este Bolt es su materializador** (§D.3-b): `OBLIGATION-TOOLING-01` (`2.0/docs/api-removals/US-000.BOLT-003.json`) difería la **fase D** —la reubicación del tooling— «cuando la capa `Data` sea portada», y **la capa ya está portada** (el **BOLT-014** cerró su Development Completed); se transcriben y se asumen los **tres recordatorios obligatorios** del §D.3 del SPEC-003 (calificar las cuatro llamadas cruzadas como `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X`; la lista de rupturas del MEM del 003 como insumo de la constancia; el reporte de dependencias a dos niveles). Se actualizan §1, §4, §5 (nota de la obligación), §6 (notas + §D.3), §7 (AC-9 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el retiro de la herencia, el lector propio con el provider único, la cadena desde la configuración portable, la ubicación en el tooling, la fuente única del mapeo, la paridad observable y el plan de integración `applicable` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 3 — revisión MATERIAL (G15/G16)** que fija el **presupuesto de turnos** antes del V-Bounce N°1. Cambia **un solo valor del frontmatter**: **`turn_budget: ""` → `turn_budget: 25`** (entero ≥ 1). Motivo: la **unidad restante** de este Bolt es **grande** —la **reubicación del tooling** (los cuatro destinos de `OBLIGATION-TOOLING-01`: `Providers/MSSQLProvider.cs` con `MSSQLReverseFactory`, el resto de `Providers/MSSQLCommon.cs`, la **mitad reverse** de `DomainTreeHelper` y los `PropertyEditors/*` con sus recursos), el **lector propio** `SqlClientSchemaReader`, la **resolución de la cadena** desde el contrato con el seam del resolutor, la suite hermética y la **primera prueba de integración del reverse**—, y §D.3-a exige ciclos **diagnóstico → directiva → build** que consumen turnos. La lección es **medida, no supuesta**: el **default de la plataforma (10)** se agotó en el **Bolt 009** (**dos veces**) y en el **Bolt 014**, mientras que con **`turn_budget: 25`** (`SPEC-261016-0037` rev.3 y `SPEC-261016-0020` rev.4) esos Bolts completaron su unidad —en **una pasada** y en **casi dos**—. Bloquear a mitad de la reubicación **desperdiciaría el inventario ya medido** (`MEM-261016-0343` §9.1) y obligaría a reanudar sobre un árbol intermedio. Se documenta en §3, §14, §16 y §19. **Nada más cambia:** la política de nulabilidad de §D.3-a, la materialización de `OBLIGATION-TOOLING-01` de §D.3-b, el retiro de la herencia, el lector propio, la cadena desde la configuración portable, la ubicación en el tooling, la fuente única del mapeo, la paridad observable y el plan de integración `applicable` siguen **en vigor**. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 4 — revisión MATERIAL (G15/G16)**, disparada por el V-Bounce N°1 detenido en `blocked` (`MEM-261016-0506`): **tres de los cuatro destinos** de `OBLIGATION-TOOLING-01` quedaron **materializados** ✔ (el provider con `MSSQLReverseFactory`, el resto de `MSSQLCommon.cs` y la mitad reverse de `DomainTreeHelper`, con las cuatro llamadas cruzadas calificadas, sin la herencia y con la suite en verde: 304 casos) y el **cuarto** —los `PropertyEditors/*`— **no compila**: portados verbatim, con `UseWindowsForms=true` y `TreatWarningsAsErrors=true`, el analizador WinForms moderno emite **`WFO1000`** (`ToStringInfoEditorForm.cs(19,21)` — `OverrideToString` — y `(31,23)` — `StringFormat` —) y el build lo trata como **error**; las dos salidas —**anotar** el código aportado (transformación que §D.3-a descarta) y **silenciar** el analizador (excepción de gate, G21 exige ADR)— son del dueño, y el Bolt 003 **anticipó** ese camino («el camino es el **retiro por escalamiento** … **nunca** un shim de compatibilidad», `MEM-261016-0046` §13). Se ordena ese camino: **el cuarto destino pasa de «reubicar» a RETIRAR los `PropertyEditors/*`** (§D.3-b), se declara como **ruptura de superficie del tooling** con sus tres datos —qué, por qué y **reemplazo: cero** (la edición 2.0 es dirigida por contrato; el vehículo de edición es del **Bolt 025**)— y la obligación **cierra como 3 destinos materializados + retiro declarado del 4.º**. Se actualizan §1, §4 (in-scope/out-of-scope), §5, §6 (notas + §A.2/§D.3-a/§D.3-b), §7 (AC-8/AC-9 y su mapeo), §8.1, §13, §14, §15, §16 y §17. **Nada más cambia:** los otros tres destinos, el retiro de la herencia, el lector propio, la cadena desde el contrato, la política de nulabilidad de §D.3-a, la ubicación en el tooling, la fuente única del mapeo, la paridad observable y el plan de integración `applicable` siguen **en vigor**. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

**Nota al aprobador — qué cambia la revisión 4, y por qué (material, G15/G16).** Viene de un **bloqueo medido**, no
de una preferencia: el V-Bounce N°1 de este Bolt entregó **tres de los cuatro destinos** de `OBLIGATION-TOOLING-01`
(el provider con `MSSQLReverseFactory`, el resto de `MSSQLCommon.cs` y la mitad reverse de `DomainTreeHelper`, con
las cuatro llamadas cruzadas calificadas, **sin** la herencia de la capa de datos y con la suite en verde: 304
casos) y se detuvo en el **cuarto**: los `PropertyEditors/*` portados **verbatim** hacen que el analizador WinForms
moderno emita **`WFO1000`** (`ToStringInfoEditorForm.cs(19,21)` y `(31,23)`) y `TreatWarningsAsErrors` lo convierte
en **error** (`MEM-261016-0506` §8.1). Cambia **una cosa**:

1. **El cuarto destino pasa de «reubicar» a RETIRAR (§D.3-b).** Las dos salidas quedan fuera de la autoridad del
   ejecutor —**anotar** el form aportado es la transformación que **§D.3-a descarta**, y **silenciar** el analizador
   es una **excepción de gate** (**G21** exige un ADR aprobado, nunca una decisión del ejecutor)—, y el propio Bolt
   003 **anticipó** este desenlace: *«el camino es el **retiro por escalamiento** vía condición de parada, **nunca**
   un shim de compatibilidad»* (`MEM-261016-0046` §13). Esta revisión **ordena ese camino**: los `PropertyEditors/*`
   **se retiran** (no se reubican, no se anotan, no se silencia ningún analizador). **Por qué es correcto:** la
   partición del 003 **ya retiró** los atributos de diseñador del modelo y **eliminó** el acoplamiento modelo→editor
   (`DomainTree.cs` L1621); en la 2.0 la edición es **dirigida por el esquema del contrato** y **nada los consume**
   —darles un vehículo es una decisión de **producto** de la UI nueva (**Bolt 025**)—.
2. **La ruptura queda declarada con su reemplazo: cero.** El retiro entra en la constancia (ADR-008 §3.1) con sus
   tres datos —**qué** desaparece (los editores/Forms y sus recursos), **por qué** y **con qué se reemplazan:
   nada**— para las notas del Bolt 028; el `.csproj` **no** declara `UseWindowsForms` y el tooling queda **sin UI**.
3. **La obligación cierra: 3 destinos materializados + retiro declarado del 4.º** — ese es el estado final que el
   MEM del V-Bounce que la cierre constará (junto con la ruptura de superficie del tooling que ya declaró el N°1).
4. **Lo que NO cambia:** los **otros tres destinos** (y su evidencia: el reporte de dependencias a dos niveles, las
   cuatro llamadas cruzadas calificadas, la lista de rupturas como insumo), el retiro de la herencia, el lector
   propio con el provider único, la conexión propiedad del componente, la cadena desde la configuración portable con
   el seam del resolutor, la política de nulabilidad de §D.3-a, la ubicación en el tooling, la fuente única del
   mapeo, la paridad observable, las condiciones de parada de §15 —**con una nueva**: anotar/silenciar en lugar de
   retirar ⇒ parar— y el plan de integración `applicable` con su corrida cubriente siguen **en vigor**. El `review`
   vuelve a vacío porque la revisión es material y se re-aprueba por el revisor.

**Nota al aprobador — (histórica) qué cambió la revisión 3, y por qué (material, G15/G16).** Es la revisión **más acotada**
posible: cambia **un solo valor del frontmatter** (`turn_budget`), sobre **evidencia medida**. Enumerado:

1. **`turn_budget: ""` → `turn_budget: 25`** (entero ≥ 1 — el campo del template). Motivo: la **unidad restante** de
   este Bolt es **grande** —la **reubicación del tooling** (los cuatro destinos de `OBLIGATION-TOOLING-01`), el
   **lector propio** `SqlClientSchemaReader`, la **resolución de la cadena** desde el contrato con el seam del
   resolutor, la suite hermética y la **primera prueba de integración del reverse**—, y §D.3-a exige ciclos
   **diagnóstico → directiva → build** que consumen turnos. La lección es **medida**: el **default de la plataforma
   (10)** se agotó en el **Bolt 009** (**dos veces**) y en el **Bolt 014**; con **`turn_budget: 25`** esos Bolts
   completaron su unidad —en **una pasada** y en **casi dos**—. Bloquear a mitad de la reubicación
   **desperdiciaría el inventario ya medido** (`MEM-261016-0343` §9.1: los cinco archivos con sus tamaños, los
   cuatro puntos de caché con líneas y claves, y las refs internas 18/26/21 que impiden todo subconjunto compilable)
   y dejaría un árbol intermedio. Se documenta en §3, §14 y §16.
2. **Lo que NO cambia: todo lo demás.** La **política de nulabilidad** de §D.3-a con sus límites duros, la
   **materialización de `OBLIGATION-TOOLING-01`** de §D.3-b con sus **tres recordatorios obligatorios**, el retiro de
   la herencia (`DataAccess : BaseRule`), el lector propio con el provider único, la conexión propiedad del
   componente, la cadena desde la configuración portable, la ubicación en el tooling, la fuente única del mapeo, la
   paridad observable como criterio y como primera prueba de integración del reverse, las condiciones de parada de
   §15 y el plan de integración `applicable` con su corrida cubriente siguen **en vigor**. El `review` vuelve a
   vacío porque la revisión es material y se re-aprueba por el revisor.

**Nota al aprobador — (histórica) qué cambió la revisión 2, y por qué (material, G15/G16).** Es una revisión **preventiva más
una constancia de cierre**: **no** responde a un bloqueo de este Bolt —no hay V-Bounce todavía— sino a la **trampa
ya medida** en esta ola (los Bolts **003, 007 y 009** chocaron con la nulabilidad del árbol al aportar código del
1.4; los Bolts **008, 013, 014 y 022** la previeron por la misma vía), sabiendo que **este Bolt aporta archivos del
1.4 al tooling**; y, en el mismo acto, **reconoce una obligación vencida**. Cambia **dos cosas**:

1. **Política de nulabilidad del código aportado del 1.4 (§D.3-a, nueva).** `#nullable disable` como **primera
   línea útil** **sólo** de los archivos **aportados del 1.4** que **produzcan**
   `CS8618`/`CS8600`/`CS8603`/`CS8625` —criterio **objetivo**: *archivo aportado + diagnóstico de nulabilidad*;
   **el build es la prueba**—, **jamás** en código nuevo (el lector propio `SqlClientSchemaReader`, la resolución de
   la cadena y las pruebas conservan la nulabilidad **activa**) y **prohibido** `<Nullable>disable</Nullable>` en
   cualquier `.csproj`. Es el **espejo** de los siete precedentes ya aprobados. Límite explícito: un `using` muerto
   **hacia un namespace que no existe en el árbol** que **no** provenga de una corrección propia sigue siendo
   **parada y reporte** (§15).
2. **La obligación `OBLIGATION-TOOLING-01` del Bolt 003 está VENCIDA y este Bolt es su materializador (§5/§D.3-b).**
   La obligación difería la **fase D** —la reubicación del tooling— con una condición textual: «**cuando la capa
   `Data` sea portada**». **La capa está portada**: el **BOLT-014** cerró su Development Completed con la capa
   completa (proveedor único, interior tipado, retiro del caché del host del §C.6). La condición **se cumplió** y
   la materialización le corresponde a **este** Bolt —el que separa el reverse de la capa de datos—, con los **tres
   recordatorios obligatorios** del §D.3 del SPEC-003: **(i)** calificar las **cuatro llamadas cruzadas** de la
   mitad reverse a la portable como `Cooperator.Framework.Modeling.Model.DomainTreeHelper.X` (L184, L222/L297,
   L292); **(ii)** tomar la **lista de rupturas** del MEM del Bolt 003 como **insumo de la constancia**; **(iii)**
   medir el **reporte de dependencias a dos niveles** (presupuesto en el proyecto/lock file y aristas en el
   ensamblado). La **constancia** de que la obligación quedó cumplida entra en el **MEM** del V-Bounce que la
   materialice, con su referencia y su estado final.
3. **Lo que NO cambia: todo lo demás.** El retiro de la herencia (`DataAccess : BaseRule`), el lector propio con el
   provider único, la conexión propiedad del componente, la cadena desde la configuración portable con el seam del
   resolutor, la ubicación en el tooling, la fuente única del mapeo, la paridad observable como criterio y como
   primera prueba de integración del reverse, las condiciones de parada de §15 y el plan de integración
   `applicable` con su corrida cubriente siguen **en vigor**. El `review` vuelve a vacío porque la revisión es
   material y se re-aprueba por el revisor.

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (datos / DBA)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
