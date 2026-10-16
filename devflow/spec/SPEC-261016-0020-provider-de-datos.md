---
id: "SPEC-261016-0020"
title: "Provider de datos único con conversión explícita: `Microsoft.Data.SqlClient`, punto único validado, timeout y cadena de conexión desde la configuración portable"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-014" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 4 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-005", "ADR-004", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, build compartido, gate de compatibilidad de API y lane de integración (Bolt 001)
  - "SPEC-261016-0018-configuracion-portable.md" # contrato de configuración y catálogo de claves: timeout, cadena de conexión y error tipado (Bolt 011)
  - "SPEC-261016-0019-cache-en-data.md" # Bolt 013 — **frontera y paso SIGUIENTE**: su contrato `ICacheProvider` y sus regiones **sustituyen** los puntos de caché que este Bolt **retira**; secuencia `014-ampliado → 013`, **no** una puerta de entrada de este Bolt (revisión 3, §5)
risk_class: "high" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L2" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: 25 # revisión 4: presupuesto **explícito** (integer ≥ 1) para completar la unidad restante en un solo V-Bounce (§14) — el default de la plataforma/agente (10) se agotó dos veces en el Bolt 009 y con 25 se completó su unidad en una pasada (§18)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "applicable" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests:            # applicable — the integration tests a run executes: escritos por este V-Bounce (ver §8.2)
    - "2.0/tests/Cooperator.Framework.Data.Integration.Tests/ProviderSmokeTests.cs — conexión real y ejecución de consultas y procedimientos almacenados (scalar, non-query, reader, DataSet) con parámetros y transacciones (commit y rollback)"
    - "2.0/tests/Cooperator.Framework.Data.Integration.Tests/StoredProcedureMetadataTests.cs — descubrimiento de parámetros contra un procedimiento real, verificando que no abre ni cierra la conexión que provee el llamador"
    - "2.0/tests/Cooperator.Framework.Data.Integration.Tests/CommandTimeoutIntegrationTests.cs — timeout aplicado sobre un comando real, con su valor configurado y su default documentado"
    - "2.0/tests/Cooperator.Framework.Data.Integration.Tests/ProviderBehaviorTests.cs — cambios de comportamiento del proveedor verificados contra la base (cifrado obligatorio y validación de certificado del servidor; `DbType.Time` con `TimeSpan`; `DbType.Date`)"
  environment: "el entorno de CI del proyecto con SQL Server (la lane de integración del arnés del Bolt 001; si su SPEC fija un nombre de entorno, se usa ese)"
  data: "una base de datos de prueba con esquema conocido y un procedimiento almacenado con parámetros, provistos por el entorno; credenciales por variables de entorno, nunca en el repositorio; sin datos de negocio ni personales"
  reason: ""        # n/a — no aplica: el plan es `applicable`
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T03:53:49-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T03:53:49-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T03:53:49-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — re-aprobación de REVISIÓN 4 (material, G15/G16): misma SPEC canónica `SPEC-261016-0020`, `revision: 4`, `turn_budget: 25`, `status` re-abierto a `draft` y re-aprobado acá. Fuentes gobernadas sin cambio material: Bolt `US-000.BOLT-014` (`blocked`), ADR-005/ADR-004/ADR-001 `accepted`, REV-001 `approved`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Misma mecánica que mi precedente del Bolt 009."
    - "CONFIRMACIÓN (el único cambio de fondo) — CORRECTA: `turn_budget: ""` → **`25`** (entero ≥ 1), justificado en §3, §14, §16, §18 y §19 con la **unidad restante medida** (5 archivos / 5.549 líneas + el retiro de §C.6 + retipado + fases C/D/E + pruebas + los ciclos diagnóstico→directiva→build de §C.5) y con la **evidencia del 009** (default 10 agotado dos veces → 25 completó B–F en una pasada)."
    - "CONFIRMACIÓN (la limpieza señalada) — CORRECTA y **sin cambio de alcance**: §1 tabla de metadatos a **`Revision 4`**; la fila del Bolt 013 en §3 anotada como **frontera y paso siguiente** (su contrato `ICacheProvider` sustituye lo que este Bolt retira; secuencia `014-ampliado → 013`, no una puerta de entrada)."
    - "CONFIRMACIÓN (§18/§19) — CORRECTA: el historial tiene las **cuatro filas** (rev.1/rev.2/rev.3/rev.4, **sin reemplazo**); la nota al aprobador (§19) declara que **sólo el presupuesto cambia** y que §C.5 (nulabilidad), §C.6 (ampliación) y la secuencia siguen **en vigor**."
    - "CONFIRMACIÓN (§C.5/§C.6/secuencia en vigor) — CORRECTA: la nulabilidad preventiva (§C.5), la ampliación del retiro del caché del host (§C.6) y la secuencia `014-ampliado → 013` no cambiaron."
    - "ADJUDICACIÓN (valor 25) — **ADECUADO**: la unidad restante es **grande** (aporte completo de la capa + retiro + retipado + fases + pruebas, con los ciclos build de §C.5), y el **precedente del 009** lo respalda directamente (default 10 agotado dos veces → 25 completó la unidad en una pasada). Bloquear a mitad del aporte desperdiciaría el inventario ya medido (`MEM-261016-0343` §9.1). No lo cambio."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ REVISIÓN 4 — REVISIÓN MATERIAL (G15/G16) que fija el **presupuesto de turnos** antes del V-Bounce N°2. La
     unidad restante de este Bolt es **grande** —el aporte de la capa `Data` (5 archivos / 5.549 líneas), el
     **retiro** de §C.6, el retipado al proveedor, las fases C/D/E y las pruebas (§C.5 y §C.6 incluyen ciclos
     **diagnóstico → directiva → build** que consumen turnos)— y el **default de la plataforma (10) ya se agotó dos
     veces** en el Bolt 009, mientras que con **`turn_budget: 25`** ese Bolt completó su unidad equivalente (B–F) en
     **una sola pasada**. Bloquear por presupuesto a mitad del aporte **desperdiciaría el inventario ya medido**
     (`MEM-261016-0343` §9.1). Cambia **un solo valor del frontmatter**: `turn_budget: ""` → **`turn_budget: 25`**
     (§3, §14, §16, §18 y §19). **Nada más cambia** —ni el alcance ampliado de §C.6, ni la política de nulabilidad
     de §C.5, ni el resto del plan— y el `review` se re-abre vacío (es una revisión material).
     ⚠️ (Histórica) **REVISIÓN 3 — REVISIÓN MATERIAL (G15/G16), disparada por el V-Bounce N°1 de este Bolt detenido en `blocked`
     (`MEM-261016-0343`). El bloqueo es una **circularidad estructural** entre dos alcances: el **caché del host**
     (`System.Web`/`HttpContext`) vive **dentro** de la capa `Data` (los **cuatro** puntos medidos: `BaseGateway` y
     `BaseLoader` para las colecciones, `SQLHelper` para el timeout, `SqlHelperParameterCache` para la metadata) y
     **el aporte no compila en `netstandard2.1` sin retirarlo** — mientras que el retiro lo reclamaba el Bolt 013,
     que a su vez no puede ejecutar porque la capa no existe (`MEM-261016-0328`). Esta revisión implementa la
     **salida (d)** del MEM: **el alcance del aporte se amplía con el RETIRO del caché del host** (el retiro es
     **parte del port** —la capa no compila sin él—), y la **SUSTITUCIÓN** por `ICacheProvider` sigue siendo del
     **Bolt 013**, con la secuencia final **`014-ampliado → 013`**. Ajusta §1, §4, §5, §6 (notas + §C.6), §7
     (AC-10 y su mapeo), §8.1, §9, §13, §14, §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** el proveedor único, el punto de conversión, el interior tipado, la superficie (con
     `SqlHelper`/`SqlHelperParameterCache` a `internal`), el retiro de las esperas activas, el timeout y la cadena
     desde la configuración portable, la política de nulabilidad de §C.5, las fronteras con los Bolts 011/015/016/006
     y el plan de integración `applicable` siguen **en vigor** (el `review` se re-abre vacío: es una revisión
     material).
     ⚠️ (Histórica — revisión 2) **REVISIÓN 2 — REVISIÓN MATERIAL (G15/G16) PREVENTIVA**, cursada **antes** de que la trampa muerda y sin que exista
     todavía un V-Bounce de este Bolt. **Este Bolt aporta la capa `Data` completa desde el 1.4** (`BaseRule.cs`,
     `SQLHelper.cs`, `SqlHelperParameterCache.cs`, `BaseGateway.cs`, `BaseLoader.cs` y lo que el aporte traiga,
     `MEM-261016-0328` §1) y el árbol 2.0 hereda del Bolt 001 `Nullable=enable` + `TreatWarningsAsErrors=true`: es
     el escenario que ya detuvo a los Bolts **003, 007 y 009** y que los **022, 008 y 013** previeron por la misma
     vía, obligando a una **revisión material por cada caso** para autorizar la directiva. Esta revisión
     **adelanta esa autorización** —`#nullable disable` como primera línea útil de **cada archivo aportado del
     1.4** que **produzca diagnósticos** (`CS8618`/`CS8600`/`CS8603`/`CS8625`), **jamás** en código nuevo (**el
     punto único de conversión**, la **excepción tipada** y las pruebas conservan la nulabilidad activa),
     **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como
     prueba**—, espejo exacto de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2,
     `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3 y `SPEC-261016-0019` §B.2. Ajusta §1, §4, §5, §6 (nota de
     mecánica + §C.5), §7 (AC-9 y su mapeo), §8.1, §9, §13, §14 (fila ⚠), §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** el proveedor único, el punto de conversión, el interior tipado, la superficie, el
     timeout, la cadena de conexión, las fronteras con los Bolts 011/013/015/016/006 y el plan de integración
     `applicable` siguen **en vigor** (el `review` se re-abre vacío: es una revisión material).
     ⚠️ **Nota de secuencia (constancia del revisor — sin cambio de alcance, §5/§17/§19):** la **corrección de
     secuencia** endosada en el `AITL-MEM-Approval` del Bolt 013 (`MEM-261016-0328`, finding «CORRECCIÓN DE
     SECUENCIA») establece que **`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`**: la dependencia declarada
     del 014 sobre el 013 era **precautoria de doble-toque** sobre los archivos compartidos y **no** una puerta de
     entrada de este Bolt. Se cita para que el ejecutor no se detenga por una precondición invertida.
     ⚠️ **Aviso de frontera del caché del host (condición de parada — §15):** `HttpContext`/`System.Web` viven
     **dentro** de `BaseGateway.cs`/`BaseLoader.cs` (`MEM-261016-0328` §1) y su retiro es el **aspecto del Bolt
     013**. Si al aportar la capa el retiro del caché del host aparece como **paso necesario para compilar**,
     **parar y reportar** (patrón de la revisión de secuencia): **no** se absorbe la sustitución del 013.
     *(Histórica — **reemplazada por la revisión 3**: esa condición de parada se activó (`MEM-261016-0343`) y la
     salida elegida fue **ampliar el alcance**: el **retiro** está ahora **en alcance**, y lo que sigue **prohibido**
     es la **sustitución** por `ICacheProvider` desde este Bolt. §15 conserva el historial.)*
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-014`. El contrato de
     configuración y su catálogo son del Bolt 011 y el caché de la capa es del Bolt 013 (**consumidos**); el gate
     por token y su contraparte generada son del Bolt 015; el reverse que deja de heredar es del Bolt 016; la
     protección criptográfica de la cadena es del Bolt 022 (**se consume la forma, no la protección**).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `applicable`, porque el Bolt declaró `test_integration: required` — la
     corrida contra SQL Server real se ejecuta en el ciclo de test de integración del Bolt, después de
     `AITL-MEM-Approval`, y su resultado queda como hecho en `integration_test_runs[]` del manifiesto.
  ⚠️ TRAMPA DEL ASSET (ADR-005 §3.1): está **prohibido** correr el arnés de `Data` —o cualquier host que cargue
     `Data`— desde `lib/` sin selección de asset por runtime: ahí lo que se carga es el stub y todo método falla
     con `PlatformNotSupportedException`. El arnés vive en `2.0/tests/` sobre `net10.0` y resuelve assets.
-->

# SPEC-261016-0020 — Provider de datos único con conversión explícita: `Microsoft.Data.SqlClient`, punto único validado, timeout y cadena de conexión desde la configuración portable

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 5) |
| **Bolt** | [US-000.BOLT-014](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) |
| **ADRs** | [ADR-005](../adrs/ADR-005-provider-de-datos.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | high |
| **Revision** | 4 |

---

## 1. Objective

Se reescribe el acceso a datos de Cooperator 2.0 sobre **un solo proveedor**: **`Microsoft.Data.SqlClient`**.
La capa deja de *aparentar* neutralidad de proveedor —que hoy se rompe en la primera conversión de tipos
(`(SqlTransaction)transaction` en `BaseGateway.cs:640` y sus equivalentes)— y pasa a tener: **un único punto de
conversión** en el borde público, **validado** y que falla con **excepción tipada** en lugar de
`InvalidCastException`; el **interior tipado** de punta a punta sobre el proveedor elegido; la
**sincronización por espera activa retirada** y reemplazada por el manejo correcto de transacciones; el
**timeout de comandos** tomado del catálogo de configuración portable y aplicado a **todo** comando; y la
**cadena de conexión** obtenida de la configuración resuelta, **sin inspeccionar su contenido** para adivinar su
estado.

El problema que resuelve está documentado (REV-001 F-09, F-14 y §6 ítem 5): el proveedor `System.Data.SqlClient`
está en mantenimiento; la abstracción multi-proveedor (`DbProviderFactory`, `providerName`) es una promesa que
el código no cumple —basta un objeto que no sea `SqlTransaction` para que explote a mitad de una operación—; la
capa espera con `Thread.Sleep(0)` para "ordenar" transacciones; y la cadena de conexión se adivina por
heurística de substrings con un `Dictionary` estático como caché.

Si no se implementa: la capa de datos no puede migrar de runtime, el fallo de un objeto del tipo equivocado
sigue siendo tardío y genérico, y el producto queda atado a un proveedor en mantenimiento con una abstracción
que nadie puede usar.

**Autorización preventiva de la revisión 2 (la trampa conocida, adelantada).** Este Bolt **aporta la capa `Data`
completa desde el 1.4** (`MEM-261016-0328` §1: los archivos están **ausentes** en el 2.0) y el árbol hereda
`Nullable=enable` + `TreatWarningsAsErrors` del Bolt 001: sin autorización explícita, el aporte de código legacy
produce `CS8618`/`CS8600`/`CS8603`/`CS8625`, el build los convierte en **errores** y el gate de Build no puede
cerrar en `pass`. Eso ya ocurrió **tres veces** en esta ola (Bolts 003, 007 y 009, cada una con su revisión
material) y los Bolts 022, 008 y 013 lo previeron por la misma vía. Esta revisión **autoriza de antemano** la
respuesta ya probada —`#nullable disable` **por archivo aportado del 1.4** que la requiera (§C.5)— y **nada más**.
El resto del Bolt **no se reabre**.

**Ampliación de alcance de la revisión 3 (el retiro del caché del host es parte del port).** El V-Bounce N°1 de
este Bolt se detuvo en la condición de parada de la revisión 2: el **caché del host** (`System.Web`/`HttpContext`)
vive **dentro** de la capa `Data` y **el aporte no compila en `netstandard2.1` sin retirarlo**; el retiro lo
reclamaba el Bolt 013, que no puede ejecutar porque la capa **no existe** (`MEM-261016-0328`) — una **circularidad
estructural** entre dos alcances (`MEM-261016-0343` §1/§13.1). La salida elegida es la **(d)**: **el alcance de
este Bolt se amplía con el RETIRO del caché del host** en los **cuatro puntos medidos** —`BaseGateway.cs`,
`BaseLoader.cs` (colecciones), `SQLHelper.cs` (timeout) y `SqlHelperParameterCache.cs` (metadata)—, que quedan
**sin caché**, con el **comportamiento sin-caché declarado** y con su **paso siguiente declarado** (la
**sustitución** por `ICacheProvider`, que sigue siendo del **Bolt 013**, ya re-encolado después). El retiro **no**
es un trabajo ajeno apropiado: es **condición de compilación** del port, y la evidencia de esta V-Bounce lo dejó
medido (§C.6).

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 5 ruteó F-09 (acoplamiento a `System.Data.SqlClient`, casts
`Db*` → `Sql*`, proveedor en mantenimiento), F-14 (`Thread.Sleep` como sincronización) y F-29 (el reverse
hereda de `BaseRule`, que es del Bolt 016) a un ADR, que es ADR-005.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-005 §3.1** — **`Microsoft.Data.SqlClient` es el único** provider; los paquetes de la 2.0 **no** dependen
  de `System.Data.SqlClient` (se retira de `Data`); la **versión la fija el Bolt**, centralizada en
  `Directory.Packages.props` (ADR-001 §3.4) y con **dos criterios obligatorios de aceptación**: (1) la línea
  elegida debe ofrecer un **asset consumible desde `netstandard2.1`**, y (2) el arnés debe pasar un **smoke test
  de conexión real** contra SQL Server (no basta con que compile). **Riesgo del asset y nota de hiato**: en las
  líneas nuevas el asset que un consumidor `netstandard2.1` ve es un **stub `netstandard2.0` cuyos métodos
  lanzan `PlatformNotSupportedException`**; la **6.0 retiró** ese asset y la **6.1 lo restauró como stub**, así
  que hay que fijar una línea que **publique el stub (5.x o 6.1+, nunca 6.0)**; y queda **prohibido** correr el
  arnés de `Data` desde `lib/` sin selección de asset por runtime. **Disparadores de escalamiento**: (a) que
  **ninguna** línea vigente publique un asset compilable desde `netstandard2.1` (ni el stub), o (b) que el
  **smoke test de conexión real sobre `net10.0`** falle con el asset que el runtime resuelve para la línea
  elegida — en cualquiera de los dos casos **se reporta y se decide con el dueño** (cambiar el TFM de `Data` no
  es una salida por conveniencia: exigiría un ADR que reemplace a ADR-001).
- **ADR-005 §3.1 (cambios de comportamiento a absorber y documentar)** — (a) `Encrypt` pasa a tener
  **`Mandatory` por defecto** y `TrustServerCertificate` sigue en `false` ⇒ las conexiones contra servidores con
  certificado autofirmado **empiezan a fallar** y hay que arreglar el certificado o declarar
  `TrustServerCertificate=true`/`Encrypt=false` **explícitamente**; (b) un `SqlParameter` con `DbType.Time`
  requiere **`TimeSpan`** (con `DateTime` deja de funcionar) y `DbType.Date` **trunca** la parte horaria; (c) el
  namespace cambia en todos los tipos, con mapeos puntuales (`SqlDataRecord`/`SqlMetaData` →
  `Microsoft.Data.SqlClient.Server`). Todo va a las **notas de migración** (documento del Bolt 028).
- **ADR-005 §3.2** — **cero casts `Db*` → `Sql*` fuera de un punto**: el borde público los recibe, los valida y
  los transforma **en un único lugar documentado** (`SqlTransactionAdapter`/`AsSqlTransaction`) que lanza una
  **excepción tipada y accionable** (`InvalidTransactionTypeException`, con el tipo recibido y el esperado);
  **interior tipado** (`SqlHelper` y `SqlHelperParameterCache` trabajan con `Microsoft.Data.SqlClient` de punta
  a punta y **no convierten nada**), con la firma preservada (`GetAndBeginTransaction()` devuelve `DbTransaction`
  cuyo objeto concreto es un `SqlTransaction`; `GetNewConnection()` devuelve una `SqlConnection` como
  `DbConnection`); **sincronización explícita sin esperas activas** (se retira `Thread.Sleep(0)` de
  `BaseRule.cs:185`; si el comportamiento "zombi" de la transacción reaparece con el proveedor nuevo se resuelve
  con una **política de retry explícita y acotada**, nunca con espera activa; si no se logra reproducir, queda
  como **deuda declarada en el MEM**); y **timeout de comandos acotado** desde
  `Cooperator:Data:CommandTimeoutSeconds`, con **default documentado** y validación del valor, eliminando la
  construcción de un `SqlCommand` descartable que hoy se usa sólo para leer el default del proveedor.
- **ADR-005 §3.3** — se **preserva la forma BCL-typed** de lo que el código generado consume (`IBaseGateway`,
  `BaseGateway`, `BaseRule`, `InternalDBHelper` con firmas sobre `DbConnection`/`DbTransaction`/`DbDataReader`/
  `DataSet`), se **declara la naturaleza** de esa abstracción ("forma de firma, no promesa de multi-motor") en la
  documentación pública de `Data`, y **`SqlHelper` y `SqlHelperParameterCache` pasan a `internal`** — ruptura de
  superficie frente a 1.4 **agregada por el ADR** (extensión del ruteo de REV-001 §6 ítem 5), cuya **constancia
  de alcance va al MEM** del Bolt. Se eliminan los restos de la abstracción muerta: **`providerName` deja de
  existir** y no hay selección de provider por string.
- **ADR-005 §3.4 (cruce con el Bolt 013)** — `SqlHelperParameterCache` usa el **`ICacheProvider` del ADR-004**
  con su propia región y TTL por defecto (1 día) en lugar del `Hashtable` estático con espejo en `HttpContext`;
  y el **hazard verificado** que se corrige acá: `DiscoverSpParameterSet` hace `connection.Open() … Close()`
  sobre una conexión **que recibe del llamador** — en 2.0 **el descubrimiento no abre ni cierra conexiones
  ajenas** (si necesita una, la pide y la dispone), apoyándose en `SqlCommandBuilder.DeriveParameters`.
- **ADR-005 §3.5 / ADR-004 §3.2-§3.3** — la cadena se obtiene de `ICooperatorConfiguration` con **prefijo de
  esquema explícito**; `Data` **no** conoce algoritmo, claves ni formato protegido (la protección es del Bolt
  022) y **no inspecciona el contenido** del valor (se eliminan la heurística de substrings y el `Dictionary`
  estático).
- **ADR-005 §5 (NFRs)** — *No hidden casts* (**0** ocurrencias de `(SqlTransaction)`, `(SqlConnection)` y
  equivalentes fuera del adaptador; tests negativos con excepción tipada) y *Command timeout bounded* (**100 %**
  de los comandos con timeout de la configuración o del default documentado; **0** con timeout infinito o no
  validado).
- **ADR-001 §3.4** — un solo provider ADO.NET en `Data` y política de dependencias (versión centralizada,
  licencia permisiva).

**Qué decide este SPEC, dentro de esas restricciones.** La **forma concreta del punto único** de conversión y
dónde vive, el **mecanismo de fijación y verificación de la línea del proveedor** (asset + smoke) y qué hacer si
no se cumple, el **inventario de casts y de esperas activas** que se retiran, la forma de la **resolución del
timeout** y de la **cadena de conexión** desde el contrato de configuración, y la **frontera con el Bolt 013**
sobre los dos archivos que ambos tocan (`SQLHelper.cs`: el caché del timeout es del 013; el proveedor, el tipado
y la regla del timeout por comando son de acá).

**Frontera y alcance cruzado.** Este Bolt **no** implementa la caché ni sus puntos de uso (Bolt 013), **no**
define el contrato de configuración ni su catálogo (Bolt 011: los **consume**), **no** retira el gate por token
ni su contraparte generada (Bolt 015), **no** hace que el reverse deje de heredar de la capa (Bolt 016), **no**
toca el SQL que emite el motor ni sus plantillas (Bolts 020/021), **no** decide ni implementa protección
criptográfica de la cadena (Bolt 022: consume el valor ya resuelto) y **no** cambia el TFM ni el mapa de
paquetes (ADR-001 §3.1).

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt (`required` → `applicable`).

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-014`](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — `status: ready`, `risk_class: high`, `data_classification: internal`, `test_integration: required`; el `acknowledgment_reason` declara que el cambio de comportamiento del proveedor se verifica en la corrida contra base real | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-09, F-14 y §6 ítem 5 (F-29 y F-34/F-46 se citan como relacionados y son de otros Bolts/DISC) | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) — §3.1 (provider y versión, asset/línea, smoke y disparadores de escalamiento, cambios de comportamiento), §3.2 (punto único, interior tipado, transacciones sin espera activa, timeout), §3.3 (superficie pública, `internal` y la extensión del ruteo), §3.4 (parámetros, caché de metadata y el hazard de la conexión), §3.5 (cadena de conexión: forma, no criptografía), §5 (NFRs) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (catálogo: `Cooperator:Data:CommandTimeoutSeconds` y la clave de la cadena de conexión) y §3.3 (regla del prefijo explícito, error tipado, sin heurística, sin `Dictionary` estático) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.4 (un solo provider ADO.NET, versiones centralizadas y licencias), §3.1/§3.2 (TFM de `Data`, piso de consumo .NET 10+, sin shims ni multi-target), §3.5 (limpieza de referencias muertas) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — su SPEC canónica ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md)) entrega el contrato de configuración y el catálogo de claves | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-013`](../functional/bolts/US-000.BOLT-013-cache-en-data.md) — su SPEC canónica ([`SPEC-261016-0019`](./SPEC-261016-0019-cache-en-data.md)) entrega el contrato de caché y las regiones de metadata y timeout *(revisión 3: **frontera y paso siguiente** — la secuencia final es `014-ampliado → 013` y **no** una puerta de entrada de este Bolt, §5)* | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — arnés, build compartido, **lane de integración** y gate de compatibilidad de API | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`Data/BaseRule.cs` (los casts en `ExecuteScalar*`/`ExecuteNonQuery*`/`ExecuteReader*`/`FillDataSet*`, el
`Thread.Sleep(0)` en la línea 185; y en 56/85/104-129 el nombre por defecto de la cadena, el `Dictionary`
estático y la heurística de descifrado), `Data/BaseGateway.cs` (el cast `(SqlTransaction)transaction` en 640 y
653; además los puntos de caché que son del Bolt 013), `Data/SQLHelper.cs` (el `commandTimeOut` estático y su
lectura de configuración en 44-79, con el `SqlCommand` descartable) y
`Data/SqlHelperParameterCache.cs` (el `Hashtable` estático con espejo en el caché del host —Bolt 013— y el
`connection.Open()`/`Close()` sobre la conexión del llamador en 73-84, que es el hazard de **este** Bolt).
Evidencia de la superficie que el consumidor compila: `Tools/CooperatorModeler/Templates/CSharpClasses/*`
(plantillas que usan `DbTransaction`/`DbConnection` y que este Bolt **no** toca — son del Bolt 019).

**Coherencia del plan de integración:** el Bolt declara `test_integration: required` (§3.2.2) y este SPEC declara
`integration_test_plan.applicability: "applicable"` **con sus tests, su entorno y sus datos** (§8.2 y
frontmatter). **No hay desajuste** y por lo tanto no hay reporte bloqueante.

> **Presupuesto de turnos (revisión 4).** Este SPEC fija **`turn_budget: 25`** —parámetro de ejecución declarado en
> el frontmatter— para que la **unidad restante** de este Bolt se complete en **un solo** V-Bounce: §14 lo justifica
> y §16 lo refleja. **No es una fuente gobernada**: es un parámetro del SPEC y se cambia por revisión material,
> como cualquier otro valor del frontmatter. *(Precedente exacto del mecanismo: `SPEC-261016-0037` rev.3 — el Bolt
> 009 agotó el default de la plataforma (10) **dos veces** por presupuesto y, con `turn_budget: 25`, completó su
> unidad equivalente en **una sola pasada**.)*

---

## 4. Scope

### In scope

- El **proveedor único** `Microsoft.Data.SqlClient`: versión fijada centralmente, verificación de que la línea
  elegida **publica un asset consumible desde `netstandard2.1`** (nunca la 6.0) y **smoke de conexión real**, y
  el retiro de `System.Data.SqlClient` de la capa.
- El **punto único de conversión** en el borde público: un lugar documentado que recibe `DbTransaction`/
  `DbConnection`, valida que sean del proveedor y los transforma, con **excepción tipada** y accionable cuando
  no lo son (tipo recibido y esperado).
- El **interior tipado** de `Data` sobre el proveedor elegido, sin conversiones, con la **superficie pública
  preservada** (forma BCL-typed para lo que consume el código generado) y su naturaleza declarada.
- El retiro de la **sincronización por espera activa** y su sustitución por el manejo correcto de
  transacciones (dispose/commit/rollback deterministas), con **política de retry explícita y acotada** sólo si
  el comportamiento heredado reaparece.
- El **timeout de comandos** desde el catálogo (`Cooperator:Data:CommandTimeoutSeconds`) con su default
  documentado y su validación, aplicado a **todo** comando, y la eliminación del `SqlCommand` descartable.
- La **cadena de conexión** obtenida de la configuración portable **sin inspeccionar su contenido**, con el
  retiro de la heurística de substrings y del `Dictionary` estático.
- El retiro de los **restos de la abstracción multi-proveedor** (`providerName`, selección por string).
- El retiro de los **casts** `Db*` → `Sql*` fuera del punto único y de la **selección de proveedor por nombre**.
- **`SqlHelper` y `SqlHelperParameterCache` a `internal`**, con la **constancia de la extensión del ruteo** para
  el MEM del Bolt (ADR-005 §3.3).
- La **corrección del hazard de la conexión del llamador** en el descubrimiento de parámetros de stored
  procedure (no abrir ni cerrar conexiones ajenas).
- La **declaración de los cambios de comportamiento del proveedor** (cifrado obligatorio y validación de
  certificado del servidor; `DbType.Time`/`DbType.Date`; mapeos de namespace) y de las rupturas de superficie,
  como insumo de las notas de migración (ADR-008 §3.1; documento del Bolt 028).
- La **evidencia del ciclo de test de integración** contra SQL Server real (smoke, transacciones, parámetros,
  timeout, comportamiento del proveedor) y del gate de compatibilidad de API.
- La **política de nulabilidad del código aportado del 1.4** (revisión 2, §C.5): la directiva `#nullable disable`
  **por archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —**jamás** en código nuevo (el punto
  único de conversión, la excepción tipada y las pruebas se escriben con la nulabilidad activa) y **prohibida** a
  nivel de proyecto—, como mecánica de build del aporte.
- El **retiro del caché del host** dentro del aporte (revisión 3, §C.6): las llamadas a `HttpContext`/`System.Web`
  de los **cuatro puntos medidos** —colecciones en `BaseGateway.cs` y `BaseLoader.cs`, timeout en `SQLHelper.cs` y
  metadata de parámetros en `SqlHelperParameterCache.cs`— **se retiran**, porque **el aporte no compila en
  `netstandard2.1` sin hacerlo**; los puntos quedan **sin caché**, con el **comportamiento sin-caché declarado** y
  con su entrada en la constancia de rupturas/comportamiento (incluido su **paso siguiente**: la sustitución por
  `ICacheProvider`, del Bolt 013). **No** es la sustitución del 013: es la condición de compilación del port.

### Out of scope

- La **sustitución del caché de la capa** por el contrato `ICacheProvider` (su contrato, sus regiones —metadata y
  timeout—, el cap, los TTL y la invalidación por clave): **Bolt 013**, que ejecuta **después** de este Bolt
  (secuencia `014-ampliado → 013`). Este Bolt **retira** las llamadas al caché del host porque **el aporte no
  compila sin eso** (§C.6) y **no** sustituye nada; desde acá, sustituir sería absorber el 013 (§15).
- El **contrato de configuración y su catálogo**: **ADR-004 §3.2 / Bolt 011** (se **consumen**: la clave del
  timeout, la clave de la cadena de conexión, el error tipado de configuración inválida y la validación del
  prefijo de esquema).
- La **protección criptográfica** de la cadena de conexión y la migración de las cadenas ofuscadas de 1.4:
  **Bolt 022 / ADR-007**. Acá se consume el **valor ya resuelto** (`plain:` es la única condición que el
  framework implementa hasta ese Bolt).
- El **gate por token** (`CheckForToken`) de la superficie de datos y su contraparte generada: **Bolt 015**.
- El **reverse que deja de heredar** de esta capa: **Bolt 016**.
- El **SQL que emite el motor** y sus plantillas (incluidos `providerName`, `App.config` y el T-SQL
  modernizado): Bolts 019/020/021, ADR-003.
- El **reverse-engineering** y el provider concreto de esquema (`MSSQLProvider`, su herencia y la cadena desde
  el tooling): Bolt 016 (ADR-005 §3.6).
- El **TFM** y el **mapa de paquetes**: ADR-001 §3.1 (un cambio de TFM exigiría un ADR que lo reemplace).
- La **modernización del DAAB heredado** (`SqlHelper`, 2532 líneas): deuda declarada por ADR-005 §4, candidata
  a un Bolt posterior.

---

## 5. Prerequisites and baseline

- **Bolt 011 y su SPEC canónica
  ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md), `prerequisites`):** el contrato de
  configuración (`ICooperatorConfiguration` y su acceso) y su **catálogo de claves** — de donde salen el
  **timeout** (`Cooperator:Data:CommandTimeoutSeconds`) y la **cadena de conexión**
  (`Cooperator:ConnectionStrings:<nombre>` con su prefijo explícito y su error tipado). Sin él, este Bolt no
  tendría de dónde leer ninguno de los dos valores.
- **Bolt 013 y su SPEC canónica
  ([`SPEC-261016-0019`](./SPEC-261016-0019-cache-en-data.md)):** el contrato `ICacheProvider`, sus regiones
  (metadata y timeout), el cap y los TTL. **La secuencia final es `014-ampliado → 013`**, con sus dos constancias
  (el `AITL-MEM-Approval` del Bolt 013 —`MEM-261016-0328`, finding «CORRECCIÓN DE SECUENCIA»— y el bloqueo de este
  Bolt —`MEM-261016-0343` §13.1, salida **(d)**—): **este Bolt ejecuta primero** y **aporta la capa con el retiro
  del caché del host** (§C.6); el 013 ejecuta **después** y **sustituye** los puntos por su contrato. Este Bolt
  **no** consume el contrato del 013 ni implementa una caché propia: el timeout sale del **catálogo** (Bolt 011) y
  la metadata se consulta **por operación** (§C.6). La frontera de aspecto se declara en §4/§14/§15.

  > **Corrección de secuencia (revisión 2 — el punto de la secuencia lo **completa** la revisión 3 con
  > `014-ampliado → 013`; el resto del aviso sigue vigente).** El `AITL-MEM-Approval`
  > del V-Bounce N°1 del Bolt 013 (`MEM-261016-0328`, finding «CORRECCIÓN DE SECUENCIA») dejó constancia de que
  > **`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`**: la dependencia declarada del 014 sobre el 013 era
  > **precautoria de doble-toque** sobre los dos archivos compartidos (`SQLHelper.cs`/`SqlHelperParameterCache.cs`)
  > y **no** una puerta de entrada. La secuencia correcta es **014 → 013**: este Bolt aporta/retypea la capa `Data`
  > (proveedor, casts, tipado del interior, hazard de la conexión) y el 013 sustituye **después** los puntos de
  > caché, sobre la capa ya presente. **Consecuencia para este Bolt:** su ejecución **no espera** al 013; si al
  > iniciar el V-Bounce el contrato de caché no está en el árbol, eso **no** es una precondición incumplida de este
  > Bolt (la resolución del timeout se apoya en el **catálogo de configuración**, Bolt 011, y **no** en una caché
  > propia). La frontera de aspecto con el 013 sigue **exactamente** como está declarada (§4/§14/§15).
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés (`2.0/tests/` sobre
  `net10.0`, con resolución de assets por runtime), la **lane de integración** con SQL Server y el **gate de
  compatibilidad de API** con su línea base 1.4 — que es la evidencia del retiro de superficie y de la
  extensión del ruteo declarada por ADR-005 §3.3.
- **El proyecto `Cooperator.Framework.Data` en el árbol 2.0** (`netstandard2.1`) como **destino del aporte**: **este
  Bolt aporta la superficie de la capa** desde el 1.4 (`BaseRule.cs`, `SQLHelper.cs`, `SqlHelperParameterCache.cs`,
  `BaseGateway.cs`, `BaseLoader.cs` — hoy **ausentes** en el 2.0, medido en `MEM-261016-0343` §9.1). *(Revisión 3:
  la precondición de la revisión 2 —«con su superficie y sus puntos de caché **ya sustituidos** (Bolt 013)»—
  **queda sin efecto**; con la secuencia `014-ampliado → 013` los puntos los sustituye el 013 **después**, y este
  Bolt los **retira** porque el aporte no compila sin eso (§C.6). Se conserva el historial del cambio.)* Si el
  **proyecto** no existe, **parar y reportar**: este Bolt no crea el proyecto; sí aporta su contenido.
- **Acceso a una instancia de SQL Server** para la corrida de integración (dependencia externa declarada por el
  Bolt) y **la lane de integración del arnés** ejecutable. Si el entorno no está disponible, el V-Bounce puede
  cerrar con los tests escritos, construidos y verificados estáticamente, pero el **ciclo de test de integración
  queda pendiente** y el Bolt **no** pasa a `ready-for-acceptance` (§15).
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de los casts, de las esperas
  activas y de la abstracción muerta.
- **Build state de partida:** el árbol 2.0 compila; este Bolt **no** cambia TFMs, **no** agrega paquetes más allá
  del provider (que reemplaza al que sale) y **no** publica artefactos de distribución.
- **Decisión de ejecución registrada (no es supuesto):** el arnés **no** corre desde `lib/` sin resolución de
  assets (ADR-005 §3.1: ahí se carga el stub y todo método falla). Las pruebas de integración viven en
  `2.0/tests/` sobre `net10.0`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-014`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).
>
> **Mecánica de build del aporte del 1.4 (revisión 2, §C.5).** Cualquier archivo que este Bolt **aporte del 1.4**
> —`BaseRule.cs`, `SQLHelper.cs`, `SqlHelperParameterCache.cs`, `BaseGateway.cs`, `BaseLoader.cs` y lo que el
> aporte traiga— sigue la **política de nulabilidad de §C.5**: `#nullable disable` como primera línea útil **sólo**
> en los archivos aportados que **produzcan** `CS8618`/`CS8600`/`CS8603`/`CS8625` (criterio objetivo; el build es la
> prueba), **nunca** en código nuevo (el punto único de conversión, la excepción tipada y las pruebas conservan la
> nulabilidad activa) y **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es la trampa que ya
> detuvo a los Bolts 003/007/009 —y que los Bolts 022/008/013 prevén igual—: queda autorizada **antes** del
> V-Bounce.
>
> **Frontera del caché del host (revisión 2 — **reemplazada** por la ampliación de alcance de la revisión 3).**
> `HttpContext`/`System.Web` viven **dentro** de `BaseGateway.cs`/`BaseLoader.cs` y su retiro es el **aspecto del
> Bolt 013**. Si al aportar la capa ese retiro aparece como **paso necesario para compilar**, **parar y reportar**
> (§15): no se absorbe la sustitución del 013.
>
> **El retiro del caché del host está EN ALCANCE (revisión 3, §C.6).** Ese paso necesario **se confirmó** (`MEM-261016-0343` §1/§9.1) y la salida elegida es la **(d)**: el **retiro** de las llamadas al caché del host en los
> **cuatro puntos medidos** es **parte del port** —la capa no compila en `netstandard2.1` sin él—, y lo que **sigue
> prohibido** desde acá es la **sustitución** por `ICacheProvider` (del **Bolt 013**, que ejecuta **después**:
> secuencia `014-ampliado → 013`) y tocar archivos fuera de los del aporte (§15).

### Phase A — El proveedor único y su línea fijada

**Duration:** 3h total cycle — **Complexity:** Medium

#### A.1 Versión centralizada, asset y smoke como criterios de aceptación

Se fija **`Microsoft.Data.SqlClient`** como único provider, con su versión declarada **una sola vez** en
`Directory.Packages.props` (ADR-001 §3.4) y con los **dos criterios obligatorios** de ADR-005 §3.1: (1) la línea
elegida **publica un asset consumible desde `netstandard2.1`** —el stub `netstandard2.0` de las líneas nuevas,
cuyos métodos lanzan `PlatformNotSupportedException`— y (2) el **smoke de conexión real** contra SQL Server
pasa. **Nota de hiato que restringe la elección: 5.x o 6.1+, nunca 6.0** (la 6.0 retiró el asset y la 6.1 lo
restauró como stub). La verificación es **evidencia del V-Bounce** (no una suposición): la versión elegida y el
resultado de ambos criterios quedan registrados en el MEM con la corrida que los demuestra.

Se retira `System.Data.SqlClient` de la capa (ADR-005 §3.1) y **no** se agrega ningún otro paquete: `Data`
conserva **exactamente un** provider ADO.NET (ADR-001 §3.4).

**Files modified:**
- `2.0/Directory.Packages.props` — versión fijada del provider único (una sola declaración para el árbol)
- `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` — `PackageReference` al provider único y retiro de `System.Data.SqlClient`

#### A.2 La trampa del asset, explícita

Queda escrito —y verificado por la ubicación de las pruebas— que el arnés **no** puede correr desde `lib/` sin
selección de asset por runtime: en ese escenario lo que se carga es el **stub** y todo método falla. El arnés
vive en `2.0/tests/` sobre `net10.0` y resuelve assets (ADR-005 §3.1, F-50).

---

### Phase B — El punto único de conversión, con excepción tipada

**Duration:** 3h total cycle — **Complexity:** Medium

#### B.1 Un solo lugar convierte, y si no puede, lo dice

Se implementa el **punto único** de ADR-005 §3.2 en el borde público: recibe `DbTransaction`/`DbConnection`,
**valida** que sean `SqlTransaction`/`SqlConnection` del provider elegido y los entrega al interior ya tipados.
Si el objeto no es del proveedor, lanza **`InvalidTransactionTypeException`** con el **tipo recibido** y el
**esperado** (nunca un `InvalidCastException` genérico a mitad de una operación). El punto está **documentado en
la superficie pública** de `Data` y es el **único** lugar del paquete donde una conversión de tipos de ADO.NET
ocurre: la verificación es *No hidden casts* de ADR-005 §5 (0 ocurrencias fuera de él).

**Files created:**
- `2.0/src/Cooperator.Framework.Data/Casting/TransactionAdapter.cs` — el punto único (`AsSqlTransaction`/`AsSqlConnection`) con la validación y el error tipado
- `2.0/src/Cooperator.Framework.Data/Exceptions/InvalidTransactionTypeException.cs` — la excepción del borde, con el tipo recibido y el esperado

---

### Phase C — Interior tipado, transacciones sin espera activa y superficie declarada

**Duration:** 5h total cycle — **Complexity:** High

#### C.1 Interior tipado sobre el proveedor

`SqlHelper` y `SqlHelperParameterCache` pasan a trabajar con `Microsoft.Data.SqlClient` de punta a punta
(`SqlConnection`, `SqlTransaction`, `SqlCommand`, `SqlParameter`) y **no convierten nada**. Se preserva la firma
que consume el código generado: `GetAndBeginTransaction()` sigue devolviendo `DbTransaction` —cuyo objeto
concreto es un `SqlTransaction`— y `GetNewConnection()` devuelve una `SqlConnection` como `DbConnection`
(ADR-005 §3.2). Se eliminan **todos** los casts `Db*` → `Sql*` del interior (inventario de evidencia) y los
**restos de la abstracción muerta**: `providerName` deja de existir y no queda ninguna selección de provider por
string (ADR-005 §3.3).

#### C.2 Transacciones: dispose/commit/rollback deterministas, sin esperar

Se retira la **sincronización por espera activa** (`BaseRule.cs:185`, `Thread.Sleep(0)`) y se implementa el orden
correcto: dispose/commit/rollback deterministas, sin liberar la transacción antes de terminar el comando
(ADR-005 §3.2). Si al correr contra base real **reaparece** el comportamiento "zombi" que el `Sleep` tapaba, se
resuelve con una **política de retry explícita y acotada** (intentos y backoff documentados). Si **no** se logra
reproducir, queda como **deuda técnica declarada en el MEM** —nunca un parche silencioso—.

> **Frontera declarada (ver §14):** el `Thread.Sleep(100)` "bajo lock del Core" que menciona ADR-005 §3.2 vive en
> el código del **export a Excel** de `Core` (`ObjectListHelper.cs:282`, dentro del camino retirado por ADR-001
> §3.5), cuyo dueño es el **Bolt 006**. Este Bolt retira la espera activa **de la capa de datos** y, si al
> iniciar el V-Bounce esa otra espera todavía existe, la **reporta** como frontera, sin borrar el objetivo de
> otro Bolt.

#### C.3 La superficie: qué se preserva, qué se declara y qué se vuelve interno

- Se **preserva** la forma BCL-typed de `IBaseGateway`, `BaseGateway`, `BaseRule` e `InternalDBHelper`
  (`DataBaseHelper`) con sus firmas sobre `DbConnection`/`DbTransaction`/`DbDataReader`/`DataSet`
  (ADR-005 §3.3): es lo que el código generado consume y lo que sostiene la meta de cambios mínimos (OQ-006).
- Se **declara** en la documentación pública del paquete que esa abstracción es **forma de firma**, no una
  promesa de multi-motor: nadie debe volver a construir sobre la idea de que un provider alternativo va a
  funcionar (la trampa que F-09 documentó).
- **`SqlHelper` y `SqlHelperParameterCache` pasan a `internal`.** Es una **ruptura de superficie** frente a 1.4
  **agregada por ADR-005 §3.3** (extensión del ruteo de REV-001 §6 ítem 5, que sólo pedía eliminar casts y
  tipar). Su **constancia de alcance** —para que la ruptura sea trazable a una decisión y no a un efecto
  colateral— entra en el **MEM** del Bolt, con quién los usaba, por qué se retiran (el código generado **no** los
  consume: verificado) y con qué se reemplazan (usar el provider directamente).

**Files modified:**
- `2.0/src/Cooperator.Framework.Data/SQLHelper.cs` — interior tipado, sin casts, con la administración de transacción corregida; **se retira** la lectura/escritura del caché del host del **timeout** (§C.6) — su **sustitución** por el contrato es del Bolt 013
- `2.0/src/Cooperator.Framework.Data/SqlHelperParameterCache.cs` — interior tipado, `internal`, y el hazard de la conexión del llamador corregido (§C.4); **se retira** el caché del host de la **metadata** (§C.6) — su **sustitución** por el contrato es del Bolt 013
- `2.0/src/Cooperator.Framework.Data/BaseRule.cs` — retiro de los casts y de `Thread.Sleep(0)`, y la cadena de conexión por el contrato (§D.2)
- `2.0/src/Cooperator.Framework.Data/BaseGateway.cs`, `BaseLoader.cs` — retiro de los casts y **retiro del caché del host de las colecciones** (§C.6) — su **sustitución** por el contrato es del Bolt 013
- `2.0/src/Cooperator.Framework.Data/IBaseGateway.cs` — documentación de la naturaleza de la abstracción (forma de firma)

#### C.4 El hazard de la conexión del llamador

`DiscoverSpParameterSet` deja de hacer `connection.Open()` … `connection.Close()` sobre una conexión **que
recibe del llamador**: en 2.0 la conexión la administra quien la provee; el descubrimiento de parámetros **no
abre ni cierra conexiones ajenas** y, si necesita una, la pide y la dispone (ADR-005 §3.4). El descubrimiento
sigue apoyándose en `SqlCommandBuilder.DeriveParameters`, sin cambios de semántica más allá de la conexión. La
verificación es **de integración** (con una conexión real abierta que debe seguir abierta y no dispuesta al
terminar).

#### C.5 La mecánica de build del aporte del 1.4: política de nulabilidad (revisión 2)

El árbol 2.0 hereda de `2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`,
mientras que el código del 1.4 es **anterior** a las anotaciones de nulabilidad. Es la trampa **ya medida** en esta
ola —`MEM-261016-0107` §8.2 (28 diagnósticos `CS86xx` al portar el modelo), `MEM-261016-0209` §8.1 (`CS8618` en
`BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`) y `MEM-261016-0227` §13 (el aviso declarado del
Bolt 009)— y afectaría a **este Bolt** en cuanto aporte la capa `Data` completa del 1.4 (`MEM-261016-0328` §1), con
elementos sin inicializar, retornos posiblemente nulos, `null` literales o posibles desreferencias nulas: los
diagnósticos `CS8618`/`CS8600`/`CS8603`/`CS8625` que el árbol convierte en **errores** y que dejan el gate de Build
sin poder cerrar en `pass`.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3,
`SPEC-261016-0037` §B.2, `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3 y `SPEC-261016-0019` §B.2):** cada archivo
**aportado del 1.4** que **produzca diagnósticos de nulabilidad** empieza con la directiva `#nullable disable`
(primera línea útil, antes de los `using`), y **sólo** esos archivos la llevan:

- **Sí la llevan:** los archivos **aportados del 1.4** por este Bolt —`BaseRule.cs`, `SQLHelper.cs`,
  `SqlHelperParameterCache.cs`, `BaseGateway.cs`, `BaseLoader.cs` y el cierre mínimo que la compilación exija—
  **cuando produzcan** `CS8618`/`CS8600`/`CS8603`/`CS8625`. El **criterio es objetivo** —*archivo aportado del 1.4 +
  diagnóstico de nulabilidad*— y la lista exacta la fija la **ejecución con el build**, no un nombre supuesto. La
  directiva **apaga el análisis**: no cambia una instrucción de IL y deja la semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —`Casting/TransactionAdapter.cs`, la excepción
  `InvalidTransactionTypeException` (§B.1), los archivos de documentación de la superficie y las pruebas—, que
  conserva la nulabilidad **activa** del árbol porque está escrito para el mundo anotado; y queda **prohibido**
  `<Nullable>disable</Nullable>` en **cualquier** `.csproj`, incluido el de `Data`: apagaría el análisis del proyecto
  entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta el build
  **falla** con el diagnóstico exacto (`CS8618`/`CS8600`/`CS8603`/`CS8625`); si **sobra** en código nuevo, se ve en la
  lectura de los archivos entregados. Se verifica en §8.1, §9 y AC-9.

**Un `using` que quede sin uso** como consecuencia de una corrección **propia** de este Bolt (el retiro del
`providerName` y de la selección de proveedor por nombre, los casts, la construcción del `SqlCommand` descartable)
se retira con ella: es parte de la forma ya corregida del archivo. Un `using` muerto **hacia un namespace que no
existe en el árbol 2.0** que **no** provenga de una corrección propia **no** está cubierto por esta autorización: se
**detiene y se reporta** (§15).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`,
inicializadores o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que la ordene —y en
este Bolt sería un cambio de comportamiento en el camino por el que **toda** consulta pasa, sin ADR que lo ordene,
justo lo que la frontera de §4 evita—; **diferir el aporte** dejaría al Bolt sin la capa que debe retypear. La
directiva, en cambio, es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no
cambia el comportamiento observable**: es el mismo criterio con el que se resolvió en los Bolts 003, 007, 009, 022,
008 y 013.

#### C.6 El retiro del caché del host: parte del port (ampliación de alcance, revisión 3)

**El hecho que obliga (medido en el V-Bounce N°1; `MEM-261016-0343` §1/§9.1).** Los cinco archivos que esta fase
aporta **no existen** todavía en el 2.0 y **cuatro de ellos llevan el caché del host dentro**: el proyecto `Data` es
`netstandard2.1`, donde `System.Web` **no existe**, así que **ninguno compila** sin retirarlo. No hay subconjunto
compilable: el interior (`SqlHelper`/`DataBaseHelper`) vive justamente en uno de los archivos con `HttpContext`.

| Punto de caché del host (1.4) | Dónde (líneas medidas) | Qué se retira | Comportamiento sin caché (declarado) |
|---|---|---|---|
| **Colecciones del gateway** (`Coop_DAL_{RuleName}GetAll_Cache` / `…GetAllDictionary_Cache`, 1 h) | `BaseGateway.cs` (1557 l): `using System.Web;` · `HttpContext`**×20** — `HttpContext.Current.Cache[key]`, `Cache.Add/Remove`, `CacheItemPriority`, `NoSlidingExpiration` (L112, 323, 332, 336, 369, 377, 383, 391, 401, 403-404, 409, 423, 431, 438, 445, 455, 457-459, 463) | La lectura/escritura en el caché del host y el `using System.Web;` | Cada `GetAll`/`GetAllDictionary` **consulta la base** (sin caché). El punto queda **sin caché** hasta que el Bolt 013 lo sustituya |
| **Colecciones del loader** (las mismas claves y el mismo patrón) | `BaseLoader.cs` (655 l): `using System.Web;` · `HttpContext`**×19** (L183, 192, 196, 214, 221, 226, 234, 242-243, 245, 258, 265, 270, 278, 286-287, 289) | Ídem | Ídem |
| **Timeout de comandos** (`Cooperator_DAL_SP_CommandTimeOut`, espejado en el host) | `SQLHelper.cs` (2532 l): `using System.Web;` · `HttpContext`**×4** (L66, 70, 73, 76) | La lectura/escritura en el caché del host | El timeout sale **del catálogo** (`Cooperator:Data:CommandTimeoutSeconds`, con su default documentado — §D.1) en cada uso: **no** hay valor en caché |
| **Metadata de parámetros de SP** (`Cooperator_DAL_SP_Params_Cache`, espejado en el host) | `SqlHelperParameterCache.cs` (269 l): `using System.Web;` · `HttpContext`**×4** (L52, 56, 57, 59) | La lectura/escritura en el caché del host (el `Hashtable` estático de 1.4 se retira con ella) | La metadata se **consulta por operación** (`SqlCommandBuilder.DeriveParameters`), sin caché, hasta que el Bolt 013 la sustituya |

**Encuadre (lo que este retiro es y no es).**

- **Es parte del port:** el retiro es **condición de compilación** del aporte —sin él, `Data` no puede ser
  `netstandard2.1` (ADR-001 §5)—, y la evidencia de esta V-Bounce lo dejó **medido** antes de decidir.
- **No es la sustitución del 013:** acá **no** se introduce `ICacheProvider`, ni regiones, ni claves, ni cap, ni TTL,
  ni una caché interina propia. Los puntos quedan **sin caché**, con el comportamiento **declarado**. La
  **sustitución** —contrato, regiones, cap, TTL, invalidación por clave— es del **Bolt 013**, que ejecuta
  **después** (secuencia `014-ampliado → 013`, constancias en §5/§17/§19).
- **Queda declarado, no escondido:** los cuatro puntos entran en la **constancia de rupturas y cambios de
  comportamiento** (`§E.1`/`§E.2` para las notas de migración, ADR-008 §3.1) con **qué** se retira, **por qué**
  (compilación del port, ADR-001 §5) y **cuál es su paso siguiente** (la sustitución por el contrato del Bolt 013),
  incluido el efecto observable: **más consultas a la base** donde antes había caché (colecciones y metadata) y el
  **timeout leído del catálogo en cada uso**.
- **Doble trabajo acotado y declarado:** el parche intermedio sin caché es el mínimo para que la capa compile; el
  013 lo reemplaza con su contrato sobre la capa ya presente (`MEM-261016-0343` §13.1, salida (d)). La deuda queda
  en el MEM del V-Bounce y en las notas.

**Alcance del permiso (bounded).** Esta ampliación alcanza a: los **cinco archivos** del aporte (§C.1–§C.4), el
`.csproj`/`Directory.Packages.props` de §A.1 y los dos archivos nuevos de §B.1. **Ningún otro archivo** se toca: ni
los de otro Bolt, ni el caché de providers Lazy (`Core`, Bolt 005), ni el `Thread.Sleep(100)` del export a Excel
(Bolt 006, §C.2), ni la configuración del host (Bolt 011).

**Evidencia de esta fase:** `0` `HttpContext` y `0` `System.Web` en los cinco archivos entregados (y el
`using System.Web;` retirado con ellos), la capa compilando en `netstandard2.1` **sin** la referencia web, y las
cuatro entradas de ruptura/comportamiento con su paso siguiente. Se verifica en §8.1, §9 y AC-10.

---

### Phase D — Timeout y cadena de conexión desde la configuración portable

**Duration:** 3h total cycle — **Complexity:** Medium

#### D.1 Timeout de comandos: del catálogo, acotado y en todos los comandos

El timeout sale del **catálogo de configuración** (`Cooperator:Data:CommandTimeoutSeconds`, ADR-004 §3.2), con
su **default documentado** y **validación del valor** (un valor inválido o ausente con default no habilita un
fallback propio: el error tipado es el del contrato de configuración, Bolt 011). **Todo** comando corre con
timeout **explícito y acotado** (NFR *Command timeout bounded* de ADR-005 §5: 0 comandos con timeout infinito o
no validado) y se elimina la construcción del **`SqlCommand` descartable** que hoy se usa sólo para leer el
default del proveedor.

> **Frontera declarada (Bolt 013):** el **punto de caché** del timeout (su región y su clave) es de
> `SPEC-261016-0019`; acá se cambia **cómo se resuelve el valor y cómo se aplica a los comandos**, no dónde se
> guarda.

#### D.2 Cadena de conexión: resuelta por el contrato, sin inspeccionar su contenido

La cadena se obtiene de la **configuración portable** (ADR-004 §3.2/§3.3) —el valor llega **resuelto**, con su
prefijo de esquema validado por el contrato— y `Data` **no** conoce algoritmo, claves ni formato protegido
(ADR-005 §3.5; la protección es del Bolt 022). Se **eliminan** la **heurística de detección por substrings** y el
**`Dictionary` estático** de cadenas (F-13): no queda ningún punto en `Data` que inspeccione el contenido de la
cadena para adivinar su estado. El nombre por defecto de la clave se conserva para continuidad (ADR-004 §3.2).

**Files modified:** los de §C.3 (la resolución del valor y su caché estático salen de `BaseRule.cs`).

---

### Phase E — Cambios de comportamiento del proveedor, rupturas y evidencia

**Duration:** 3h total cycle — **Complexity:** Medium

#### E.1 Lo que el proveedor cambia y hay que absorber

| Cambio | Consecuencia | Tratamiento |
|---|---|---|
| `Encrypt` **`Mandatory`** por defecto y `TrustServerCertificate=false` | Las conexiones contra servidores con **certificado autofirmado empiezan a fallar** | **Riesgo operativo declarado** + verificación en la corrida real + entrada en las **notas de migración** con las dos salidas explícitas (arreglar el certificado o declarar `TrustServerCertificate=true`/`Encrypt=false`) |
| `DbType.Time` requiere **`TimeSpan`**; `DbType.Date` **trunca** la parte horaria | Cambios **silenciosos en compilación** y visibles en runtime | Revisión de los sitios que construyen parámetros + corrida real con parámetros + entrada en las notas |
| Cambio de **namespace** con mapeos puntuales (`SqlDataRecord`/`SqlMetaData` → `Microsoft.Data.SqlClient.Server`) | Ruptura de compilación en el consumidor que use esos tipos | Entrada en las notas con el reemplazo exacto |
| **Retiro del caché del host** (revisión 3, §C.6): los **cuatro** puntos de la capa —colecciones en `BaseGateway.cs`/`BaseLoader.cs`, timeout en `SQLHelper.cs`, metadata de parámetros en `SqlHelperParameterCache.cs`— dejan de usar `HttpContext`/`System.Web` | Los puntos quedan **sin caché**: **más consultas a la base** (colecciones y metadata) y el **timeout leído del catálogo en cada uso**; el paquete deja de arrastrar la plataforma web | **Ruptura de comportamiento declarada** con su **paso siguiente** declarado (la **sustitución** por `ICacheProvider`, Bolt 013) + verificación de `0 HttpContext`/`0 System.Web` (§8.1/AC-10) + **entrada en las notas de migración** (ADR-008 §3.1) |

#### E.2 Evidencia

- **Corrida de integración** (el plan `applicable` de §8.2): smoke de conexión y ejecución de consultas y
  procedimientos con transacciones y parámetros; descubrimiento de parámetros sin abrir/cerrar la conexión del
  llamador; timeout aplicado y su origen en el catálogo; los cambios de comportamiento del proveedor
  verificados contra la base.
- **Veredicto del punto único de conversión**: 0 conversiones fuera del punto (NFR *No hidden casts*), con los
  casos negativos (objeto de transacción/conexión del tipo equivocado ⇒ excepción tipada, nunca
  `InvalidCastException`).
- **Gate de compatibilidad de API** (Bolt 001) sobre la capa de datos: las rupturas (`SqlHelper`/
  `SqlHelperParameterCache` a `internal`, retiro de `System.Data.SqlClient`, `providerName` fuera del
  framework) aparecen como diferencias contra la línea base 1.4 y **cada una** está cubierta por la **lista de
  rupturas** de este SPEC con su reemplazo (ADR-008 §3.1) — una diferencia no listada **falla** el gate.
- **Lista de cambios de comportamiento del proveedor** para las **notas de migración** (ADR-005 §3.1; documento
  del Bolt 028), y la **constancia de la extensión del ruteo** exigida por ADR-005 §3.3.
- **Presupuesto de dependencias**: `Data` con **exactamente un** provider ADO.NET y sin `System.Data.SqlClient`
  (ADR-001 §3.4/§5).
- **Retiro del caché del host (revisión 3, §C.6):** `0` `HttpContext` y `0` `System.Web` en los cinco archivos
  entregados (y el `using System.Web;` retirado con ellos), la capa compilando en `netstandard2.1` **sin** la
  referencia web, y las **cuatro entradas** de ruptura/comportamiento —con su paso siguiente (la sustitución del
  Bolt 013)— en la lista que va a las notas de migración.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: Un solo proveedor, con línea verificada y smoke real

**Given** el árbol 2.0 construido
**When** se revisa el presupuesto de dependencias de `Data` y la línea del provider
**Then** hay **un solo** provider ADO.NET (`Microsoft.Data.SqlClient`), **no** queda `System.Data.SqlClient`,
**no** queda selección de proveedor por nombre (`providerName`) en el framework, la línea fijada **publica un
asset consumible desde `netstandard2.1`** (nunca la 6.0) y el **smoke de conexión real** contra SQL Server pasa
en el ciclo de integración

### AC-2: Cero conversiones fuera del punto único, y el fallo es tipado

**Given** el código de `Data`
**When** se buscan conversiones de tipos de ADO.NET a tipos del proveedor
**Then** hay **0** ocurrencias de `(SqlTransaction)`, `(SqlConnection)` y equivalentes **fuera** del punto único
documentado (NFR *No hidden casts*)
**And Given** un objeto de transacción o conexión que **no** es `SqlTransaction`/`SqlConnection`
**When** atraviesa el borde público
**Then** se obtiene `InvalidTransactionTypeException` con el **tipo recibido** y el **esperado**, y **nunca**
una `InvalidCastException` genérica a mitad de la operación

### AC-3: El interior está tipado sobre el proveedor y la firma pública se preserva

**Given** el interior de la capa (`SqlHelper`, `SqlHelperParameterCache`)
**When** se inspecciona su uso de tipos
**Then** trabaja con `SqlConnection`/`SqlTransaction`/`SqlCommand`/`SqlParameter` **sin convertir**, y la
superficie que consume el código generado conserva sus firmas sobre `DbConnection`/`DbTransaction`/
`DbDataReader`/`DataSet` (`GetAndBeginTransaction()` devuelve `DbTransaction` con un `SqlTransaction` dentro;
`GetNewConnection()` devuelve una `SqlConnection` como `DbConnection`)

### AC-4: Todo comando corre con timeout acotado, tomado del catálogo

**Given** un comando construido por la capa
**When** se ejecuta (herméticamente y contra la base real en el ciclo de integración)
**Then** lleva un timeout **explícito y acotado**, proveniente de `Cooperator:Data:CommandTimeoutSeconds` o de
su default documentado, **0** comandos quedan con timeout infinito o no validado, y la construcción del
`SqlCommand` descartable para leer el default del proveedor **no existe**

### AC-5: La cadena de conexión viene resuelta y no se inspecciona

**Given** una cadena de conexión provista por la configuración portable
**When** la capa la necesita
**Then** la toma **resuelta** del contrato (con su prefijo de esquema validado por el contrato, Bolt 011), **sin**
inspeccionar su contenido para adivinar su estado, con **0** heurísticas de substrings y **sin** el `Dictionary`
estático de cadenas

### AC-6: No hay esperas activas y las transacciones son deterministas

**Given** el camino de transacciones de la capa
**When** se ejecutan commit y rollback (contra la base real)
**Then** el resultado es determinista (dispose/commit/rollback en el orden correcto, sin liberar la transacción
antes de terminar el comando), **0** `Thread.Sleep` queda en el camino de datos, y si el comportamiento heredado
reaparece queda una **política de retry explícita y acotada** documentada (o deuda declarada en el MEM si no se
reproduce)

### AC-7: El descubrimiento de parámetros no toca la conexión ajena

**Given** una conexión **abierta** provista por el llamador
**When** se descubre el conjunto de parámetros de un stored procedure
**Then** la conexión **sigue abierta** y **no** fue dispuesta por el descubrimiento, y la metadata resultante es
correcta (verificado contra la base real)

### AC-8: La superficie se ajusta con constancia y las rupturas quedan registradas

**Given** el paquete `Data` construido y el gate de compatibilidad de API corrido
**When** se revisa la superficie y la evidencia del V-Bounce
**Then** `SqlHelper` y `SqlHelperParameterCache` son `internal`, la forma BCL-typed del contrato del consumidor
está preservada y su naturaleza declarada en la documentación, la **extensión del ruteo** de ADR-005 §3.3 está
constada en el MEM, y **cada** ruptura y **cada** cambio de comportamiento del proveedor (cifrado obligatorio y
validación de certificado; `DbType.Time`/`DbType.Date`; namespaces) está declarado con su reemplazo para las
notas de migración (ADR-008 §3.1)

### AC-9: El código aportado del 1.4 no rompe el build por nulabilidad

**Given** los archivos del 1.4 que este Bolt aporta (`BaseRule.cs`, `SQLHelper.cs`, `SqlHelperParameterCache.cs`,
`BaseGateway.cs`, `BaseLoader.cs` y el cierre que la compilación exija)
**When** se construye la capa con advertencias tratadas como error
**Then** no queda **ningún** diagnóstico de nulabilidad: cada archivo **aportado del 1.4** que lo requiera lleva la
directiva de **§C.5** como primera línea útil, **ningún** archivo nuevo la lleva, y **ningún** `.csproj` declara
`<Nullable>disable</Nullable>`

### AC-10: La capa compila sin el caché del host y el retiro queda declarado

**Given** los cinco archivos de la capa aportados desde el 1.4 (con el caché del host dentro en cuatro de ellos)
**When** se construye `Data` en `netstandard2.1` y se revisan las fuentes entregadas
**Then** hay **0** `HttpContext` y **0** `System.Web` en ellos (el `using System.Web;` se retira con las llamadas),
la capa compila **sin** la referencia a la plataforma web, los puntos quedan **sin caché** con el comportamiento
declarado (el timeout sale del **catálogo**; la metadata se **consulta por operación**), y las **cuatro entradas**
de ruptura/comportamiento están en la lista que va a las notas con su **paso siguiente** (la sustitución por
`ICacheProvider`, Bolt 013) — **sin** introducir `ICacheProvider` ni una caché interina propia en este Bolt

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — la capa usa un único proveedor, sin dependencia del que está en mantenimiento | §6/A.1 | AC-1 · presupuesto de dependencias + línea/asset + smoke |
| Bolt §2 — ninguna conversión de tipos fuera del punto único declarado | §6/B.1 | AC-2 · *No hidden casts* (búsqueda/analizador) + `TransactionAdapterTests` (negativos) |
| Bolt §2 — el interior está tipado sobre el proveedor elegido | §6/C.1 | AC-3 · inspección del interior + corrida real |
| Bolt §2 — el timeout proviene del catálogo y todo comando corre con timeout explícito y acotado | §6/D.1 | AC-4 · test del timeout (valor y default) + corrida real |
| Bolt §2 — la cadena se obtiene de la configuración resuelta, sin inspeccionar su contenido | §6/D.2 | AC-5 · test con doble de configuración + 0 heurísticas |
| Bolt §2 — no queda ningún rastro de selección de proveedor por nombre | §6/C.1 | AC-1 · búsqueda de `providerName`/selección por string |
| Bolt §2 — el manejo de transacciones es determinista y sin espera activa | §6/C.2 | AC-6 · corrida real con commit/rollback + 0 `Thread.Sleep` en el camino de datos |
| ADR-005 §3.4 — el descubrimiento no abre ni cierra la conexión del llamador | §6/C.4 | AC-7 · `StoredProcedureMetadataTests` (integración) |
| ADR-005 §3.3 — superficie preservada, `internal` declarado y extensión del ruteo constada | §6/C.3, §6/E.2 | AC-8 · gate de API + MEM |
| ADR-005 §5 — NFRs *No hidden casts* y *Command timeout bounded* | §6/B.1, §6/D.1 | AC-2, AC-4 · NFR medidos con su evidencia |
| ADR-005 §3.1 — cambios de comportamiento del proveedor documentados y verificados | §6/E.1 | AC-8 · `ProviderBehaviorTests` (integración) + notas |
| ADR-001 §3.4 — un solo provider ADO.NET y huella de dependencias | §6/A.1, §6/E.2 | AC-1 · listado de dependencias del árbol |
| Bolt §2 — el código del 1.4 que este Bolt aporta no rompe el build por nulabilidad (mecánica ya medida en los Bolts 003/007/009 y prevenida en 022/008/013) | §6/C.5: directiva `#nullable disable` **por archivo aportado del 1.4 + diagnóstico** | AC-9 · build de `Data` + lectura de los archivos entregados |
| Bolt §2 — el aporte incluye el **retiro** del caché del host (el port no compila sin él), y la **sustitución** sigue siendo del Bolt 013 | §6/C.6, §6/E.1 | AC-10 · build sin la referencia web + `0 HttpContext`/`0 System.Web` + las cuatro entradas de la constancia |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests:** del **punto único de conversión** —caso correcto con objetos reales del proveedor y **casos
  negativos por tipo** con dobles que derivan de `DbTransaction`/`DbConnection` sin ser del proveedor, esperando
  `InvalidTransactionTypeException` con el tipo recibido y el esperado—; de la **resolución del timeout**
  (valor configurado, default documentado, valor inválido ⇒ error tipado del contrato de configuración, nunca un
  fallback propio); de la **resolución de la cadena** con un **doble de `ICooperatorConfiguration`** que entrega
  un valor ya resuelto (y verificación de que el código **no** inspecciona su contenido); de la **construcción de
  parámetros** en los tipos que cambian de comportamiento (`DbType.Time` con `TimeSpan` y `DbType.Date`) a nivel
  de armado del parámetro, sin ejecutar contra la base.
- **Component/inspection tests (NFR):** verificación de **0 conversiones** `(Sql*)$` fuera del punto único y de
  **0** `Thread.Sleep` en el camino de datos (búsqueda sobre el código del paquete, tal como ADR-005 §5 propone
  medirlo: gate de analizadores/expresión regular en CI); verificación de que **no queda** `providerName` ni
  selección por string; verificación del listado de dependencias (`Data` con un solo provider ADO.NET y sin
  `System.Data.SqlClient`) y de la declaración de naturaleza de la abstracción en la documentación pública
  (lectura del documento generado).
- **Política de nulabilidad del código aportado (§C.5, revisión 2):** chequeo por **lectura** de los archivos
  entregados —cada archivo **aportado del 1.4** que produzca diagnósticos de nulabilidad empieza con
  `#nullable disable` y **ningún** archivo nuevo la lleva—, más el propio `build` con
  `TreatWarningsAsErrors=true` (la prueba real: sin la directiva, el build falla con
  `CS8618`/`CS8600`/`CS8603`/`CS8625`). Se verifica además que **no** hay `<Nullable>disable</Nullable>` en ningún
  `.csproj` y que ningún `using` muerto hacia un namespace inexistente quedó sin resolver ni autorizado (`using`
  muerto que **no** provenga de una corrección propia ⇒ parar, §15).
- **Retiro del caché del host (§C.6, revisión 3):** barrido de los **cinco archivos** entregados buscando
  `HttpContext` y `System.Web`: **0** ocurrencias en cada uno, con el `using System.Web;` **retirado**; el build de
  `Data` en `netstandard2.1` **sin** la referencia web; y la verificación de que los cuatro puntos quedan **sin
  caché** con el comportamiento declarado (el timeout se resuelve por el **catálogo** en cada uso; la metadata se
  **consulta por operación**) y de que **no** se introdujo `ICacheProvider` ni una caché interina propia.
- **Consumer-side contract tests (test doubles):** el **doble de `ICooperatorConfiguration`** (el contrato es del
  Bolt 011) es el nivel correcto para verificar timeout y cadena sin depender de la configuración real del host;
  el **doble de caché** (contrato del Bolt 013) verifica que la resolución del timeout usa **la región** que le
  corresponde y no una caché propia. Las conexiones y transacciones **no** se doblan: cuando el objeto no es del
  proveedor, el caso correcto **es** el fallo tipado (AC-2).
- **Edge cases:** objeto `null` en el borde (transacción/conexión) ⇒ error tipado y accionable; `DbTransaction`
  de otro proveedor (doble de prueba) ⇒ `InvalidTransactionTypeException`; transacción ya completada o dispuesta
  ⇒ error claro, no un `InvalidOperationException` crudo; timeout `0`, negativo, no numérico y ausente ⇒ valor
  validado o error tipado del contrato; cadena vacía o ausente ⇒ error tipado del contrato (no un fallback);
  parámetro `DbType.Time` con `DateTime` (debe fallar de forma explícita, no truncar) y con `TimeSpan` (correcto);
  `DbType.Date` con parte horaria (truncado declarado); comando con `CommandTimeout` provisto por el default.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida de los tests negativos del punto único con su excepción y su
  contenido; la búsqueda de casts y de esperas activas con su resultado; la resolución del timeout y de la
  cadena con el doble de configuración; el listado de dependencias; el resultado del **gate de compatibilidad de
  API**; la lista de rupturas y de cambios de comportamiento para las notas; y la constancia de la extensión del
  ruteo. Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "applicable"` porque el Bolt declaró `test_integration: required`
> (§3.2.2). Los tests se **escriben, construyen y verifican estáticamente** en este V-Bounce y se **ejecutan** en
> el ciclo de test de integración del Bolt, **después** de `AITL-MEM-Approval`; el resultado queda como **hecho**
> en `integration_test_runs[]` del manifiesto (no hay checkpoint de integración, §3.2.2).

- **Integration tests (real dependencies — SQL Server):**
  - `ProviderSmokeTests` — el smoke de conexión real (criterio obligatorio de ADR-005 §3.1) y la ejecución de
    consultas y procedimientos almacenados: `scalar`, `non-query`, `reader` y `DataSet`, con **parámetros** y con
    **transacciones** (commit y rollback), ejercitando el borde público y el interior tipado.
  - `StoredProcedureMetadataTests` — descubrimiento de parámetros contra un procedimiento real con una conexión
    **provista y abierta por el llamador**: comprueba AC-7 (no la abre ni la cierra ni la dispone) y que la
    metadata resultante es correcta.
  - `CommandTimeoutIntegrationTests` — el timeout **aplicado** sobre un comando real (valor configurado y
    default) y su origen en el catálogo; es la mitad "contra la base" del NFR *Command timeout bounded*.
  - `ProviderBehaviorTests` — los cambios de comportamiento de ADR-005 §3.1 **verificados contra la base**: la
    conexión con la configuración por defecto del proveedor (cifrado obligatorio y validación de certificado del
    servidor) contra un servidor con certificado autofirmado —el resultado (falla explícita y su mensaje, o el
    ajuste declarado) queda registrado como la evidencia del riesgo operativo—, `DbType.Time` con `TimeSpan` y
    `DbType.Date` con parte horaria.
  - **Quién los escribió:** este V-Bounce. El arnés (Bolt 001) aporta el proyecto, la lane y la resolución de
    assets por runtime (nunca desde `lib/`, ADR-005 §3.1).
- **Contract / E2E against the real providers:** el contrato que cruza el límite **es** el del proveedor ADO.NET
  y **se ejerce contra él**: la corrida de arriba es el E2E contra el proveedor real (SQL Server). No hay un
  consumidor externo adicional que verificar (el código generado del consumidor es del Bolt 019 y su compatibilidad
  de **fuente** se verifica con el gate de API sobre la superficie).
- **Edge cases (los que sólo pueden verificarse contra la base):** transacción con rollback ante un error del
  comando (sin conexión colgada ni transacción abierta); procedimiento con parámetros de salida/retorno;
  parámetro con `null`; conexión que el llamador ya tenía abierta y que debe sobrevivir al descubrimiento de
  parámetros; comando que excede el timeout configurado (mensaje del proveedor, no una excepción genérica del
  framework); ejecución con `Encrypt=Mandatory` contra un certificado autofirmado (comportamiento documentado).
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
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); `Data` con un solo provider ADO.NET, **sin la referencia a la plataforma web** (0 `HttpContext`/`System.Web`) y **sin diagnósticos de nulabilidad** (la directiva de §C.5, sólo en los archivos aportados del 1.4 que la requieren) | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluidos los negativos del punto único y la resolución de timeout/cadena con dobles | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El doble de `ICooperatorConfiguration` (Bolt 011) y el de caché (Bolt 013) verifican la resolución del timeout y de la cadena sin depender del host | pass |
| SAST | V-Bounce | 0 hallazgos altos; sin heurística de inspección de la cadena, sin `Thread.Sleep` en el camino de datos, sin casts fuera del punto | pass |
| Dependency scanning / licenses | V-Bounce | `Data`: exactamente **un** provider ADO.NET, licencia permisiva, versión centralizada; **sin** `System.Data.SqlClient` (ADR-001 §3.4/§5) | pass |
| OWASP Top 10 — static review | V-Bounce | Inyección SQL: el borde de parámetros se mantiene (se tipa, no se concatena) y este Bolt **no** cambia el contrato de consultas; deserialización/SSRF: `n/a` (sin esa superficie); la cadena de conexión **no** se inspecciona ni se registra | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción ni se procesa material externo al código | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** credenciales en el repositorio y en los tests (la cadena de la corrida real viene del entorno); 0 valores de conexión en el código, en la documentación o en los mensajes de error | pass |
| Hallucination lint | V-Bounce | Cada retiro y cada cambio trazable al código 1.4 citado y a ADR-005 §3.1-§3.4; 0 rupturas sin entrada en la lista; 0 decisiones de otro Bolt tomadas "de paso" | pass |
| IP / license provenance | V-Bounce | Código propio; el único código de terceros es el provider autorizado, con su licencia permisiva verificada | pass |
| PII / DLP | V-Bounce | `internal`: no se introducen datos personales; los datos de la corrida real son de una base de prueba del entorno, sin datos de negocio | pass |
| Dependency-confusion | V-Bounce | Un solo paquete nuevo (el reemplazo del provider) con ID calificado y versión fijada centralmente; ningún ID sin calificar | pass |
| Test-first evidence | V-Bounce | Los casos negativos del punto único se escriben **desde ADR-005 §5** antes de implementar el adaptador, y la búsqueda de casts/esperas se corre **antes** del cambio para fijar el punto de partida del inventario | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la suite hermética dan el mismo resultado; la corrida de integración es reproducible en el mismo entorno y con los mismos datos | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | Corrida **cubriente que pasa** contra SQL Server real (smoke, transacciones, parámetros, metadata, timeout); registrada en `integration_test_runs[]` | pass |
| Contract / E2E against the real providers | Integration test | La misma corrida ejerce el contrato del proveedor real de punta a punta; los cambios de comportamiento de ADR-005 §3.1 quedan verificados contra la base | pass |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — no hay superficie HTTP ni servicio expuesto: la capa de datos es una biblioteca que un host invoca | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | `n/a` — este Bolt no introduce un cambio de rendimiento objetivo; la latencia depende del servidor y de las consultas del consumidor. El timeout acotado (AC-4) es el control de tiempo que sí aplica | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | `n/a` — la capa no agrega telemetría (§11); su diagnóstico son las excepciones tipadas y los mensajes del proveedor | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). Lo que se manipula son **metadatos de
  conexión** (claves de configuración y valores resueltos) y datos de una base de prueba del entorno: información
  interna, sin datos personales más allá de lo que el consumidor ya maneja.
- **Credenciales fuera del repositorio y fuera de los mensajes:** la cadena de conexión de la corrida real vive
  en la configuración del entorno (ADR-004 §3.2: el framework **no** lee archivos y la protección real es del
  ADR-007/Bolt 022). Este Bolt se asegura de que el valor **no** se inspecciona, **no** se registra y **no**
  aparece en mensajes de error ni en documentación.
- **Cambio de seguridad **a favor** con costo operativo:** `Encrypt=Mandatory` por defecto y
  `TrustServerCertificate=false` (ADR-005 §3.1) hacen que el transporte deje de ser silenciosamente inseguro;
  el costo es que las apps con certificado autofirmado **empiezan a fallar** y deben ajustarlo explícitamente. Es
  un **riesgo operativo declarado** y verificado en la corrida real, no un detalle que se descubra en producción.
- **Inyección SQL:** este Bolt **no** cambia el contrato de consultas (la capa recibe SQL y parámetros del
  consumidor y del código generado); lo que hace es **tipar el borde de parámetros** sobre el proveedor y
  eliminar la conversión implícita. Ninguna cadena de la configuración se concatena a una consulta, y la
  inspección heurística del valor de conexión —que era el otro uso indebido del dato— **se elimina**.
- **Superficie reducida:** `SqlHelper` y `SqlHelperParameterCache` dejan de ser públicos (ADR-005 §3.3), con lo
  que el consumidor no puede apoyarse en el DAAB heredado; y desaparece `providerName`, que era el punto por el
  que un valor de configuración podía cambiar el proveedor en runtime.
- **Sin degradación silenciosa:** un objeto del tipo equivocado en el borde produce **excepción tipada**
  (no un `InvalidCastException` a mitad de la operación); una configuración inválida produce el error tipado del
  contrato; y ningún camino queda con timeout infinito.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es una capa de biblioteca (`netstandard2.1`) y no emite
  logs, métricas ni trazas a ningún colector; no se le agrega instrumentación porque ninguna fuente lo pide.
- **La observabilidad de este Bolt es su evidencia:** la **corrida de integración** contra SQL Server real (con
  los cuatro suites de §8.2), el **veredicto de 0 conversiones fuera del punto único**, el **gate de
  compatibilidad de API**, la **verificación del timeout** y la lista de cambios de comportamiento y rupturas.
- **La observabilidad del diagnóstico en runtime** es parte del entregable: la excepción del borde
  (`InvalidTransactionTypeException` con el tipo recibido y el esperado) y los mensajes del proveedor. La
  instrumentación del host (logs de su aplicación) queda del lado del consumidor: la capa no la impone.
- **Observabilidad del entorno de integración:** la corrida queda registrada como hecho en
  `integration_test_runs[]` (entorno, commit cubierto y evidencia), que es la forma en que el ciclo de test de
  integración se audita sin checkpoint propio (§3.2.2).

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay es un conjunto de **cambios que el consumidor debe
  absorber**, y todos entran en las **notas de migración** (documento del **Bolt 028**, ADR-008 §3.1) con sus
  tres datos obligatorios: (a) **cambios de comportamiento del proveedor** —`Encrypt=Mandatory` por defecto y
  `TrustServerCertificate=false` (las conexiones contra certificados autofirmados empiezan a fallar: arreglar el
  certificado o declarar la excepción explícitamente), `DbType.Time` que exige `TimeSpan`, `DbType.Date` que
  trunca la parte horaria—; (b) **rupturas de superficie** —retiro de `System.Data.SqlClient`, retiro de
  `SqlHelper`/`SqlHelperParameterCache` de la superficie pública (ADR-005 §3.3, con la constancia de que es una
  **extensión del ruteo** y no el pedido original), retiro de `providerName`—; y (c) el **reemplazo** de cada
  una (el provider directo, `Microsoft.Data.SqlClient`, y los mapeos de namespace puntuales). El **gate de
  compatibilidad de API** del Bolt 001 compara contra la línea base 1.4: una diferencia no listada **falla**.
- **Compatibility:** se **preserva la forma BCL-typed** que consume el código generado (ADR-005 §3.3) —que es lo
  que sostiene la meta de cambios mínimos de OQ-006— y se declara en la documentación que esa forma es **firma**,
  no promesa de multi-motor. El proveedor de datos es, por decisión, **uno** (ADR-001 §3.4): un consumidor que
  use otro ADO.NET por su cuenta es su decisión y queda fuera del framework.
- **Deuda declarada:** el `SqlHelper` heredado del DAAB (2532 líneas) **no** se moderniza acá: pasa a `internal`
  y su simplificación es candidata a un Bolt posterior (ADR-005 §4). Si el comportamiento "zombi" de la
  transacción no se reproduce con el proveedor nuevo, el workaround retirado queda registrado como deuda con la
  evidencia del intento (ADR-005 §3.2/§4).
- **Rollback:** revertir el commit del V-Bounce y, si el paquete ya se hubiera distribuido, volver a la versión
  anterior (la contención soportada sigue siendo el 1.4 congelado, ADR-008 §3.1). No hay estado persistido, ni
  esquema migrado, ni dato transformado que deshacer. Si la corrida de integración **falla** después de la
  aprobación del MEM, el Bolt entra en `integration-tests-failed` y el ciclo se resuelve con la clasificación del
  run (código/test/SPEC/ambiente) antes de reintentar (§3.2.2): no se "arregla" el resultado, se clasifica.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Ruptura de conectividad en los consumidores por los nuevos defaults del proveedor** (cifrado obligatorio y validación de certificado) | 4 | 4 | Declarado como **cambio de comportamiento** en las notas de migración; **verificado en la corrida contra la base real** (incluido el caso del certificado autofirmado, con su mensaje); el MEM lo registra como riesgo operativo residual |
| **Conversiones residuales fuera del punto declarado** (un cast sobrevive y el fallo vuelve a ser tardío) | 3 | 4 | NFR *No hidden casts* medido con búsqueda/analizador en la suite + casos negativos por tipo con excepción tipada; el inventario de casts se levanta **antes** del cambio |
| **Cambio silencioso en el manejo de tipos de parámetros** (`DbType.Time`/`DbType.Date` se comportan distinto y no se nota en compilación) | 3 | 4 | Corrida real con parámetros + revisión de los sitios que los construyen + tests del armado del parámetro + entrada en las notas |
| **La línea elegida no publica un asset compilable desde `netstandard2.1`** (o es la 6.0, que lo retiró) | 3 | 5 | Disparador de escalamiento (a) de ADR-005 §3.1: se verifica la línea al fijarla (5.x o 6.1+, nunca 6.0) y, si ninguna sirve, **se para y se decide con el dueño** — cambiar el TFM de `Data` exige un ADR que reemplace a ADR-001 |
| **El smoke falla con el asset que el runtime resuelve** (el host carga el stub `netstandard2.0`) | 3 | 5 | Disparador de escalamiento (b): el smoke es criterio de aceptación **antes** de aceptar la línea; el arnés corre desde `2.0/tests/` con resolución de assets, y está prohibido correrlo desde `lib/` |
| **Manejo de transacciones** (el comportamiento "zombi" que el `Sleep` tapaba) | 3 | 4 | Dispose/commit/rollback deterministas + corrida real con commit y rollback; si reaparece, **política de retry explícita y acotada**; si no se reproduce, deuda declarada con la evidencia del intento |
| **El descubrimiento de parámetros sigue tocando la conexión ajena** (hazard de F-06/ADR-005 §3.4) | 3 | 3 | AC-7 verificado en integración con una conexión abierta por el llamador que debe sobrevivir |
| **Timeout no aplicado o infinito en algún camino** | 3 | 4 | NFR *Command timeout bounded*: 100 % de los comandos con timeout explícito; test del valor configurado y del default; retiro del `SqlCommand` descartable |
| **La cadena de conexión se inspecciona o se registra** (heurística eliminada que reaparece, o un mensaje con el valor) | 3 | 4 | 0 heurísticas verificadas por búsqueda; gate de secreto; la cadena nunca se registra ni se imprime; el valor llega resuelto del contrato |
| **Colisión de aspecto con el Bolt 013** en `SQLHelper.cs` y `SqlHelperParameterCache.cs` (los mismos archivos, dos ejecutores) | 3 | 3 | Frontera declarada (§4/§14): acá el proveedor, el tipado, la regla del timeout por comando y el hazard de la conexión; el **caché** es del 013. Un cambio de aspecto se reporta, no se ejecuta "de paso" |
| **El `Thread.Sleep(100)` de `Core`** que ADR-005 §3.2 menciona queda fuera de este Bolt (vive en el export a Excel retirado por el Bolt 006) | 3 | 2 | §C.2: la espera activa **de la capa de datos** se retira; la otra se **reporta** como frontera. Si sobrevive al ejecutar este Bolt, es responsabilidad del Bolt 006, no un objetivo ampliado por conveniencia |
| **Se cuela una dependencia o un segundo provider** ("por si acaso" multi-proveedor) | 2 | 4 | ADR-001 §3.4 (un solo provider) y el gate de dependencias; la abstracción multi-motor está cancelada por OQ-005 |
| **El DAAB heredado se rompe al pasar a `internal`** (algo del propio árbol lo usa) | 2 | 3 | Verificación de que el código generado **no** los consume (evidencia de ADR-005 §3.3) y gate de compilación del árbol completo |
| **El plan de integración no se puede ejecutar** (sin SQL Server disponible) | 3 | 3 | §8.2/§15: el Bolt **no** pasa a `ready-for-acceptance` sin corrida cubriente; no se declara satisfecho ni se pide `waived` (una excepción de gate exigiría ADR, G21) |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8600`/`CS8603`/`CS8625` | 3 | 3 | §C.5 la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-9 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |
| **El retiro del caché del host aparece como paso necesario para compilar** al aportar `BaseGateway.cs`/`BaseLoader.cs` (`HttpContext`/`System.Web` viven ahí y su retiro es el aspecto del **Bolt 013**) | 3 | 4 | **Revisión 3: el retiro está EN ALCANCE** (§C.6) — la salida (d) del `MEM-261016-0343`. Ya **no** se para: se **retira**, con el comportamiento declarado y su paso siguiente (la sustitución del 013). Lo que sigue prohibido es **sustituir** por `ICacheProvider` desde acá o tocar archivos fuera de los del aporte (§15) |
| **El comportamiento sin caché queda sin declarar** (más consultas a la base y el timeout leído del catálogo en cada uso) y el consumidor lo descubre en producción | 3 | 3 | §C.6/§E.1: las **cuatro** entradas de ruptura/comportamiento van a la constancia y a las notas (ADR-008 §3.1) con su efecto observable y su **paso siguiente** (la sustitución del Bolt 013); el MEM del V-Bounce registra la deuda interina |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-005/ADR-004/ADR-001; acá se
> aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) La línea concreta del provider se fija en el V-Bounce y su valor queda en el MEM** | ADR-005 §3.1 deja la versión al Bolt y fija dos criterios (asset consumible desde `netstandard2.1` + smoke real) más la restricción "5.x o 6.1+, **nunca** 6.0". Fijarla en el SPEC sería fijar un dato que se verifica ejecutando; lo verificable ahora es el **procedimiento** (comprobar la línea, correr el smoke) y sus dos disparadores de escalamiento. Si el aprobador prefiere una línea explícita en el SPEC, se agrega en la revisión |
| **(⚠ aprobador) El `Thread.Sleep(100)` "bajo lock del Core" (ADR-005 §3.2) queda fuera del alcance de este Bolt** | Está dentro del **export a Excel** de `Core` (`ObjectListHelper.cs:282`), camino que ADR-001 §3.5 retira y cuyo dueño es el **Bolt 006**. Ampliar el alcance para borrarlo acá sería ejecutar el objetivo de otro Bolt y mezclar dos evidencias. Se reporta como frontera (§C.2) |
| **(⚠ aprobador) La frontera de aspecto con el Bolt 013 sobre `SQLHelper.cs` y `SqlHelperParameterCache.cs`** | Los dos Bolts tocan los mismos archivos: el 013 cambia **dónde se guarda** (región/clave/TTL del caché); éste cambia el **proveedor**, el tipado del interior, la regla del timeout por comando y el hazard de la conexión del llamador. Se declara para que ningún ejecutor haga el trabajo del otro ni "aproveche" el archivo abierto |
| El punto único de conversión vive en un archivo propio del paquete (`Casting/TransactionAdapter.cs`) con el nombre que el ADR usa (`AsSqlTransaction`/`AsSqlConnection`) | ADR-005 §3.2 pide "un único lugar documentado" y nombra las operaciones; un archivo propio hace visible el punto y permite medir el NFR (*0 casts fuera de él*) con una búsqueda simple |
| Los casos negativos del borde usan **dobles** derivados de `DbTransaction`/`DbConnection`, no otro proveedor real | El escenario a cubrir es "el objeto no es del proveedor"; lograrlo con un doble mantiene la prueba **hermética** (nivel V-Bounce), sin arrastrar un segundo ADO.NET que ADR-001 §3.4 prohíbe en el producto |
| La verificación del **hazard de la conexión del llamador** es de **integración** y se prueba con una conexión real abierta que debe sobrevivir | `SqlConnection` es un tipo sellado del proveedor: no hay doble razonable que pruebe "no la abro ni la cierro". La propiedad sólo se puede demostrar contra una conexión real, que es exactamente lo que el NFR del ciclo de integración aporta |
| El timeout se resuelve con el **doble de `ICooperatorConfiguration`** y su región de caché es la del Bolt 013 | El contrato de configuración es del Bolt 011 y el de caché del 013: usar los contratos con dobles mantiene la prueba hermética y verifica que este Bolt **no** implementa una caché propia ni lee configuración por su cuenta |
| La **modernización del DAAB** no entra | ADR-005 §4 la declara deuda; el Bolt pide proveedor, borde, tipado, timeout y cadena, no reescribir 2532 líneas heredadas. Ampliarlo sería un Bolt distinto |
| La cadena de conexión se consume **resuelta** y no se valida su prefijo acá | ADR-004 §3.3 (y el Bolt 011) fijan la regla del prefijo y el error tipado; ADR-005 §3.5 dice que `Data` no conoce algoritmo ni formato. Duplicar la validación sería crear una segunda fuente de verdad |
| El retiro de la **selección de proveedor por nombre** se trata como ruptura interna, no como API pública | `providerName` era un dato de configuración, no una superficie tipada; se declara igual en las notas (ADR-008 §3.1) y el gate de API confirma que no queda ninguna firma pública afectada |
| La corrida de integración **no** se declara satisfecha sin ejecutarse | El Bolt es `required`: el ciclo de test de integración es lo que convierte la verificación del proveedor en un hecho. Declararla sin corrida —o pedir `waived`— sería fabricar evidencia (y una excepción de gate exigiría ADR, G21) |
| **(⚠ aprobador) Política de nulabilidad del código aportado del 1.4: `#nullable disable` por archivo aportado que lo requiera** (revisión 2) | Es la trampa **ya medida** tres veces en esta ola (Bolts 003/007/009) y **este Bolt aporta la capa `Data` completa desde el 1.4** (`MEM-261016-0328` §1). El mecanismo es el **espejo exacto** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2, `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3 y `SPEC-261016-0019` §B.2: preserva la semántica del 1.4 (apaga el análisis, **no cambia el IL**), es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no cambia el comportamiento observable**. Se **descartan**, declaradas, las alternativas de **anotar/corregir** el código aportado (transformación no ordenada en el camino por el que pasa **toda** consulta) y de **diferir** el aporte (dejaría al Bolt sin la capa que debe retypear). Se elige la opción que **evita una cuarta revisión material por el mismo motivo** |
| **(⚠ aprobador) La secuencia final es `014-ampliado → 013`** (dos constancias: el `AITL-MEM-Approval` del Bolt 013 —`MEM-261016-0328`, finding «CORRECCIÓN DE SECUENCIA»— y el bloqueo de este Bolt —`MEM-261016-0343` §13.1, salida **(d)**—) | La dependencia declarada de este Bolt sobre el 013 era **precautoria de doble-toque** sobre los dos archivos compartidos, **no** una puerta de entrada; y la secuencia `014 → 013` **no era ejecutable** tal cual (circularidad: 014 no compila sin el retiro que reclamaba el 013; el 013 no sustituye puntos que no existen). Con la **ampliación** de esta revisión, **este Bolt ejecuta primero** y aporta la capa **con el retiro del caché del host** (§C.6); el **013 ejecuta después** y **sustituye** los puntos por su contrato. Se **cita** en §5/§17/§19 |
| **(⚠ aprobador) El alcance del aporte se amplía con el RETIRO del caché del host (salida (d) del `MEM-261016-0343`)** | El retiro **no** es trabajo ajeno apropiado: es **condición de compilación** del port (el proyecto `Data` es `netstandard2.1` y `System.Web` **no existe** ahí; los **cuatro** puntos medidos viven **dentro** de los archivos que hay que aportar, y **ningún subconjunto compila** porque el interior está en uno de ellos). La **sustitución** por `ICacheProvider` **sigue siendo del Bolt 013** y **no** se absorbe: acá no se introduce el contrato, ni regiones, ni cap, ni TTL, ni una caché interina propia. El comportamiento sin caché **se declara** (más consultas a la base; timeout del catálogo en cada uso) con su paso siguiente (§C.6/§E.1/§15) |
| **El `turn_budget` se fija explícitamente en `25`** (revisión 4), en lugar de correr el V-Bounce N°2 con el default de la plataforma (10) | La **unidad restante** de este Bolt es grande: el aporte completo de la capa `Data` (**5 archivos / 5.549 líneas**, medidos en `MEM-261016-0343` §9.1), el **retiro** de §C.6, el retipado al proveedor, la superficie, las fases C/D/E y las pruebas —con los ciclos **diagnóstico → directiva → build** que §C.5 exige para la directiva de nulabilidad—. Es **evidencia medida**, no una estimación: en el **Bolt 009** el default (10) se **agotó dos veces** por presupuesto y, con **`turn_budget: 25`** (`SPEC-261016-0037` rev.3), su unidad equivalente (B–F) se completó en **una sola pasada**. Bloquear por presupuesto **a mitad del aporte** desperdiciaría el **inventario ya medido** (`MEM-261016-0343` §9.1) y obligaría a reanudar sobre un árbol intermedio —lo que el «sin estado intermedio» evita—. El parámetro se declara en el frontmatter, se documenta en §3 y se refleja en §16; su cambio es por revisión material |

---

## 15. Stop conditions

- **Ninguna línea vigente del proveedor publica un asset compilable desde `netstandard2.1`** (ni el stub):
  **parar y reportar** (disparador (a) de ADR-005 §3.1). Cambiar el TFM de `Data` **no** es una salida: exigiría
  un ADR que reemplace a ADR-001.
- **El smoke de conexión real falla con el asset que el runtime resuelve** para la línea elegida: **parar y
  reportar** (disparador (b)). Se reporta la línea, el asset resuelto y el error; no se "prueba otra cosa" hasta
  que pase.
- **El contrato de configuración (Bolt 011) o el de caché (Bolt 013) no están** en el árbol 2.0: parar y
  reportar. La salida es una enmienda de dependencia o una decisión de orden del dueño; **nunca** leer
  `App.config`, ni inspeccionar el valor de conexión, ni implementar un caché propio.
- **La protección criptográfica de la cadena aparece como necesaria** (un valor con un esquema que no sea
  `plain:`): parar y reportar. Es del **ADR-007 / Bolt 022**; acá se consume el valor resuelto y `plain:` es la
  única condición implementada por el framework hasta ese Bolt.
- **La corrida de integración no se puede ejecutar** (sin instancia de SQL Server o sin lane): el V-Bounce puede
  cerrar con los tests escritos y verificados estáticamente, pero el Bolt **no** pasa a `ready-for-acceptance`:
  queda en su ciclo de integración y el MEM registra el bloqueo. **No** se declara el plan satisfecho ni se pide
  un waiver sin ADR (G21).
- **El comportamiento "zombi" de la transacción reaparece** y no se resuelve con dispose/commit/rollback
  deterministas: parar y decidir la **política de retry explícita y acotada** con el dueño antes de inventar
  esperas. Una espera activa reintroducida es una violación directa de ADR-005 §3.2.
- **Aparece un cast nuevo** `Db*` → `Sql*` fuera del punto único, o una conversión que el punto no puede
  resolver con error tipado: parar y reportar; el NFR *No hidden casts* es 0, no "casi 0".
- **El cambio que se pide es del aspecto de otro Bolt** (caché → 013; gate por token → 015; herencia del reverse
  → 016; SQL y plantillas → 019/020/021; criptografía → 022; el `Thread.Sleep` del export a Excel → 006): parar y
  reportar. No se ejecuta el trabajo de otro Bolt (G11) ni se amplía el alcance por conveniencia.
- **Cambia materialmente una fuente gobernada** (el Bolt, un ADR asociado, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.
- **Alguien extiende la autorización de §C.5**: poner la directiva `#nullable disable` en **código nuevo** (el
  punto único de conversión, la excepción tipada o las pruebas) o `<Nullable>disable</Nullable>` en un `.csproj`;
  o intentar resolver con esa misma autorización un `using` muerto **hacia un namespace que no existe en el árbol
  2.0** que **no** provenga de una corrección propia. Parar y reportar: la autorización es **una** y está acotada
  en §C.5.
- *(Revisión 2 — **reemplazada** por la revisión 3.)* «**Al aportar la capa, el retiro del caché del host aparece
  como paso necesario para compilar** ⇒ **parar y reportar**, escalando la secuencia». Esa condición **se activó**
  (`MEM-261016-0343`) y su salida fue la **ampliación de alcance** de la revisión 3: el retiro está ahora **en
  alcance** (§C.6) y **no** se para por él. El historial del cambio se conserva acá.
- **Al aportar, alguien pretende SUSTITUIR los puntos de caché por `ICacheProvider`** —introducir el contrato, sus
  regiones (metadata/timeout), el cap, los TTL o una caché interina propia "de paso"—: **parar y reportar**. La
  **sustitución** es del **Bolt 013**, que ejecuta **después** (secuencia `014-ampliado → 013`); acá sólo se
  **retira** el caché del host, porque el port **no compila** sin eso (§C.6). Absorberla dejaría al 013 sin objeto
  (G11).
- **Al aportar, se toca un archivo fuera del alcance del permiso** (los cinco del aporte, el
  `.csproj`/`Directory.Packages.props` de §A.1 y los dos archivos nuevos de §B.1): **parar y reportar**. El permiso
  de §C.6 es **bounded**: ni el caché de providers Lazy (`Core`, Bolt 005), ni el `Thread.Sleep(100)` del export a
  Excel (Bolt 006, §C.2), ni la configuración del host (Bolt 011), ni ningún archivo de otro Bolt.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented **en un solo V-Bounce**: el SPEC fija **`turn_budget: 25`** para la unidad restante (§3/§14/§16) —o, si aun así el presupuesto se agota, progreso en el MEM y pendiente explícito para un V-Bounce nuevo—
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG**
- [ ] Code follows applicable ADRs (ADR-005 §3.1–§3.5, ADR-004 §3.2/§3.3, ADR-001 §3.4)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] La directiva de nulabilidad de **§C.5** aplicada **sólo** a los archivos aportados del 1.4 que la requieren, y verificada (lectura + build); ningún `.csproj` con `<Nullable>disable</Nullable>` y ningún `using` muerto sin resolver ni autorizado
- [ ] El **retiro del caché del host** ejecutado en los **cuatro** puntos medidos (§C.6): `0` `HttpContext`/`System.Web` en los cinco archivos entregados, la capa compilando sin la referencia web, las **cuatro** entradas de ruptura/comportamiento en la constancia con su paso siguiente (Bolt 013) y **ninguna** sustitución por `ICacheProvider` introducida acá
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: **corrida cubriente que pasa** registrada en `integration_test_runs[]` del manifiesto (Bolt `test_integration: required`, plan `applicable`); sin corrida, el Bolt permanece en su ciclo de integración y no pasa a `ready-for-acceptance`

---

## 17. References

- [`US-000.BOLT-014`](../functional/bolts/US-000.BOLT-014-provider-de-datos.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-09 (acoplamiento a `System.Data.SqlClient`, casts `Db*`→`Sql*`, proveedor en mantenimiento), F-14 (`Thread.Sleep` como sincronización) y §6 ítem 5 (F-29, F-34 y F-46 se citan como relacionados; son de otros Bolts/DISC).
- [`ADR-005`](../adrs/ADR-005-provider-de-datos.md) — §3.1 (provider único, versión, asset/línea y el hiato 6.0→6.1, smoke, disparadores de escalamiento, cambios de comportamiento), §3.2 (punto único con excepción tipada, interior tipado, transacciones sin espera activa, timeout desde el catálogo), §3.3 (superficie preservada, naturaleza declarada, `internal` con la extensión del ruteo), §3.4 (parámetros, caché de metadata y el hazard de la conexión del llamador), §3.5 (cadena de conexión: forma, no criptografía), §5 (NFRs *No hidden casts* y *Command timeout bounded*), §4 (deuda del DAAB y del "zombi").
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (catálogo: timeout y cadena de conexión; error tipado de configuración), §3.3 (prefijo explícito, sin heurística, sin `Dictionary` estático).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.4 (un solo provider ADO.NET, versiones centralizadas, licencias), §3.1/§3.2 (TFM de `Data`, piso de consumo, sin shims), §3.5 (referencias muertas y supervivencia Windows-only), §5 (NFRs).
- [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) — el contrato de configuración y su catálogo (prerrequisito, Bolt 011).
- [`SPEC-261016-0019`](./SPEC-261016-0019-cache-en-data.md) — el contrato de caché y las regiones de metadata y timeout (prerrequisito, Bolt 013).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, lane de integración, build compartido y gate de compatibilidad de API (prerrequisito, Bolt 001).
- [`US-000.BOLT-015`](../functional/bolts/US-000.BOLT-015-retiro-del-gate-token.md) · [`US-000.BOLT-016`](../functional/bolts/US-000.BOLT-016-reverse-sin-baserule.md) · [`US-000.BOLT-019`](../functional/bolts/US-000.BOLT-019-plantillas-csharp.md) · [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — los Bolts con frontera declarada y el documento de notas que recibe rupturas y cambios de comportamiento.
- [`OQ-005`](../analysis/open-questions/OQ-005-alcance-features-en-riesgo.md) y [`OQ-006`](../analysis/open-questions/OQ-006-politica-de-compatibilidad.md) — SQL Server como único motor (abstracción multi-proveedor cancelada) y la meta de cambios mínimos para el consumidor.
- Guía de migración de `Microsoft.Data.SqlClient` (namespace, `Encrypt` `Mandatory` desde 4.0/5.0, `TrustServerCertificate=false`, `DbType.Time`/`DbType.Date`, assets por plataforma y el stub `netstandard2.0`) — citada por ADR-005 §6.
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** · [`SPEC-261016-0034`](./SPEC-261016-0034-resolucion-de-tipos-calificada.md) **§B.4** · [`SPEC-261016-0035`](./SPEC-261016-0035-resolutor-aesgcm.md) **§E.3** · [`SPEC-261016-0037`](./SPEC-261016-0037-library-portable.md) **§B.2** · [`SPEC-261016-0019`](./SPEC-261016-0019-cache-en-data.md) **§B.2** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado por **revisión material** con su medición (`MEM-261016-0107` §8.2: 28 diagnósticos `CS86xx`; `MEM-261016-0209` §8.1: `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`; `MEM-261016-0227` §13: el aviso declarado del Bolt 009). Es el mecanismo que esta revisión **espeja**, **antes** de que el aporte de la capa `Data` lo requiera.
- **Constancia de la corrección de secuencia `014 → 013` (revisión 2 — **completada** por la revisión 3):** [`MEM-261016-0328`](../memory/MEM-261016-0328-cache-en-data.md) — el `AITL-MEM-Approval` del V-Bounce N°1 del Bolt 013, finding «CORRECCIÓN DE SECUENCIA»: **`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`**; la dependencia declarada de este Bolt sobre el 013 era **precautoria de doble-toque** sobre los dos archivos compartidos. El mismo MEM mide que la superficie base de `Data` **está ausente** en el 2.0 (16 `.cs`, todos en `Exceptions/`) y que `HttpContext`/`System.Web` viven **dentro** de `BaseGateway.cs`/`BaseLoader.cs`.
- **Constancia del bloqueo y de la ampliación de alcance (revisión 3):** [`MEM-261016-0343`](../memory/MEM-261016-0343-provider-de-datos.md) — el V-Bounce N°1 de **este** Bolt, `blocked` por una **circularidad estructural** entre dos alcances: el caché del host vive dentro de la capa y **el aporte no compila sin retirarlo**, mientras que el retiro lo reclamaba el Bolt 013, que a su vez no puede ejecutar (su propia constancia lo dice: «este Bolt no crea la capa de datos»). El MEM deja medido el **inventario del bloqueo** (§9.1: los cinco archivos con sus tamaños, los **cuatro** puntos de caché con sus **líneas y claves**, las refs internas 18/26/21 que impiden todo subconjunto compilable, la espera activa y los casts) y las salidas informadas (§13.1): **(c)** un acto de aporte único por otro alcance, y **(d)** —la elegida— **ampliar el alcance de este Bolt con el retiro**, dejando la **sustitución** al Bolt 013 re-encolado. **Secuencia final: `014-ampliado → 013`.**

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-014`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, estrategia de pruebas con el **plan de integración `applicable`** declarado (tests, entorno y datos), gates con las filas de integración en `pass`, control de datos `internal` y condiciones de parada. Declara la frontera de aspecto con el Bolt 013 sobre los dos archivos compartidos y el `Thread.Sleep` del export a Excel como objetivo del Bolt 006. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16) PREVENTIVA**, cursada **antes** de que la trampa muerda y sin que exista todavía un V-Bounce de este Bolt: los Bolts que aportaron código legacy en esta ola (003/007/009) chocaron con la **nulabilidad** del árbol (`Nullable=enable` + `TreatWarningsAsErrors`, del Bolt 001) y cada uno necesitó su **revisión material** para autorizar la directiva —y los Bolts 022/008/013 la previeron por la misma vía—, y **este Bolt aporta la capa `Data` completa del 1.4** (`MEM-261016-0328` §1). Cambia **una sola cosa**: la **política de nulabilidad del código aportado del 1.4** (§C.5, nueva) —`#nullable disable` como primera línea útil **sólo** en los archivos aportados que **produzcan** `CS8618`/`CS8600`/`CS8603`/`CS8625`, **jamás** en código nuevo (el punto único de conversión, la excepción tipada y las pruebas conservan la nulabilidad activa), **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**—, con el **límite** del `using` muerto hacia un namespace inexistente que no provenga de una corrección propia (parada y reporte, §15). Es el espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0034` §B.4, `SPEC-261016-0035` §E.3, `SPEC-261016-0037` §B.2 y `SPEC-261016-0019` §B.2. Además —**sin** cambio de alcance— se cita la **corrección de secuencia `014 → 013`** endosada por el revisor (`MEM-261016-0328`: este Bolt ejecuta **antes** que el 013; la dependencia era precautoria de doble-toque) en §5/§17/§19, y se agrega la **condición de parada** de la frontera del caché del host (`HttpContext`/`System.Web` en `BaseGateway`/`BaseLoader`, aspecto del 013): si su retiro aparece como paso necesario para compilar, **parar y reportar** — no absorber la sustitución del 013. Se actualizan §1, §4, §5, §6 (nota de mecánica + §C.5), §7 (AC-9 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el proveedor único, el punto de conversión, el interior tipado, la superficie, el timeout, la cadena, las fronteras y el plan de integración `applicable` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 3 — revisión MATERIAL (G15/G16)**, disparada por el V-Bounce N°1 de este Bolt detenido en `blocked` (`MEM-261016-0343`) por una **circularidad estructural**: el caché del host (`System.Web`/`HttpContext`) vive **dentro** de la capa `Data` y **el aporte no compila en `netstandard2.1` sin retirarlo**, mientras que el retiro lo reclamaba el Bolt 013, que a su vez no puede ejecutar porque la capa no existe (`MEM-261016-0328`). Se implementa la **salida (d)** del MEM: **el alcance del aporte se amplía con el RETIRO del caché del host** (§C.6, nueva) en los **cuatro puntos medidos** —`BaseGateway.cs` (colecciones, `HttpContext`×20, L112…463), `BaseLoader.cs` (colecciones, ×19, L183…289), `SQLHelper.cs` (timeout, ×4, L66…76, clave `Cooperator_DAL_SP_CommandTimeOut`) y `SqlHelperParameterCache.cs` (metadata, ×4, L52…59, clave `Cooperator_DAL_SP_Params_Cache`)— que quedan **sin caché** con el **comportamiento declarado** (el timeout sale del **catálogo**; la metadata se **consulta por operación**) y con sus **cuatro entradas** de ruptura/comportamiento en la constancia, con su **paso siguiente** declarado (la **sustitución** por `ICacheProvider`, del Bolt 013). La **sustitución NO se absorbe**: este Bolt **no** introduce el contrato, ni regiones, ni cap, ni TTL, ni una caché interina propia (§15). **Secuencia final `014-ampliado → 013`** (citadas las dos constancias). Se reemplaza la condición de parada de la frontera del caché (revisión 2) por el nuevo encuadre, conservando su historial, y se ajustan las precondiciones que quedaron sin efecto (§5: la capa «con sus puntos de caché ya sustituidos» deja de ser precondición; el frontmatter anota la frontera y el paso siguiente). Se actualizan §1, §4, §5, §6 (notas + §C.3/§C.5/§C.6 y §E.1/§E.2), §7 (AC-10 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el proveedor único, el punto de conversión, el interior tipado, la superficie, las esperas activas, el timeout y la cadena, la política de nulabilidad de §C.5 y el plan de integración `applicable` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 4 — revisión MATERIAL (G15/G16)** que fija el **presupuesto de turnos** antes del V-Bounce N°2. Cambia **un solo valor del frontmatter**: **`turn_budget: ""` → `turn_budget: 25`** (entero ≥ 1). Motivo: la **unidad restante** de este Bolt es **grande** —el aporte completo de la capa `Data` (los **5 archivos / 5.549 líneas** medidos en `MEM-261016-0343` §9.1: `BaseRule.cs` 536, `SQLHelper.cs` 2532, `SqlHelperParameterCache.cs` 269, `BaseGateway.cs` 1557, `BaseLoader.cs` 655), el **retiro** del caché del host de §C.6, el retipado al proveedor, la superficie, las fases C/D/E y las pruebas— y los ciclos **diagnóstico → directiva → build** que §C.5 exige para la directiva de nulabilidad **consumen turnos**; el **default de la plataforma (10)** ya se agotó **dos veces** en el **Bolt 009**, mientras que con **`turn_budget: 25`** (`SPEC-261016-0037` rev.3) su unidad equivalente (B–F) se completó en **una sola pasada**. Bloquear por presupuesto **a mitad del aporte** desperdiciaría el **inventario ya medido** (§9.1 de ese MEM) y dejaría un árbol intermedio. Se documenta en §3, §14, §16 y §19. Se corrige además, de paso, la **tabla de metadatos de §1** (queda en la revisión vigente) y se anota en §3 el rol del Bolt 013 (**frontera y paso siguiente**). **Nada más cambia:** el alcance ampliado de §C.6, la política de nulabilidad de §C.5, la secuencia `014-ampliado → 013`, el proveedor único, el punto de conversión, el interior tipado, la superficie, el timeout y la cadena y el plan de integración `applicable` siguen **en vigor**. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

**Nota al aprobador — qué cambia la revisión 4, y por qué (material, G15/G16).** Es la revisión **más acotada**
posible: cambia **un solo valor del frontmatter** (`turn_budget`), sobre **evidencia medida**. Enumerado:

1. **`turn_budget: ""` → `turn_budget: 25`** (entero ≥ 1 — el campo del template). Motivo: la **unidad restante** de
   este Bolt es **grande** —el aporte completo de la capa `Data` (**5 archivos / 5.549 líneas** medidos en
   `MEM-261016-0343` §9.1), el **retiro** del caché del host de §C.6, el retipado al proveedor, la superficie, las
   fases C/D/E y las pruebas— y los ciclos **diagnóstico → directiva → build** que §C.5 exige para la directiva de
   nulabilidad **consumen turnos**. La lección es **medida, no supuesta**: en el **Bolt 009** (`SPEC-261016-0037`) el
   **default de la plataforma (10)** se agotó **dos veces** por presupuesto, y con **`turn_budget: 25`** su unidad
   equivalente (B–F) se completó en **una sola pasada**. Bloquear a mitad del aporte **desperdiciaría el inventario
   ya medido** (`MEM-261016-0343` §9.1) y obligaría a reanudar sobre un árbol intermedio —lo que el «sin estado
   intermedio» evita—. Se documenta en §3, §14 y §16.
2. **Limpieza señalada, sin cambio de fondo:** la **tabla de metadatos de §1** queda en la **revisión vigente**, y la
   fila de §3 referida al Bolt 013 queda anotada como **frontera y paso siguiente** (secuencia `014-ampliado → 013`),
   en coherencia con lo que ya declaran §5 y el frontmatter.
3. **Lo que NO cambia: todo lo demás.** El **alcance ampliado** de §C.6 (el retiro del caché del host es parte del
   port), la **política de nulabilidad** de §C.5, la **secuencia `014-ampliado → 013`**, el proveedor único y su
   línea con los dos criterios, el punto único de conversión y su excepción tipada, el interior tipado y la
   superficie, el retiro de las esperas activas, el timeout y la cadena desde la configuración portable, las
   condiciones de parada de §15 y el plan de integración `applicable` con su corrida cubriente siguen **en vigor**.
   El `review` vuelve a vacío porque la revisión es material y se re-aprueba por el revisor.

**Nota al aprobador — (histórica) qué cambió la revisión 3, y por qué (material, G15/G16).** La revisión 2 fue aprobada y su
V-Bounce N°1 se detuvo **`blocked`** en la condición de parada que ella misma declaraba: el **caché del host** vive
**dentro** de la capa `Data` y **su retiro es condición de compilación** del aporte, mientras el retiro lo reclamaba
el Bolt 013 —que no puede ejecutar porque la capa no existe—. Es una **circularidad estructural** entre dos
alcances, medida y documentada (`MEM-261016-0343` §1/§9.1, con líneas y claves). La revisión 3 implementa la salida
**(d)**. Enumerado:

1. **El alcance del aporte se amplía con el RETIRO del caché del host (§C.6, nueva).** El **retiro** de las
   llamadas a `HttpContext`/`System.Web` en los **cuatro puntos medidos** —`BaseGateway.cs` y `BaseLoader.cs`
   (colecciones), `SQLHelper.cs` (timeout) y `SqlHelperParameterCache.cs` (metadata)— es **parte del port**: la capa
   es `netstandard2.1` y **no compila** sin él (y no hay subconjunto compilable: el interior vive en uno de los
   archivos con `HttpContext`). Los puntos quedan **sin caché**, con el **comportamiento declarado** (más consultas
   a la base; el timeout leído **del catálogo** en cada uso) y con sus **cuatro entradas** de
   ruptura/comportamiento —con su **paso siguiente**— para las notas de migración (§C.6, §E.1/§E.2, AC-10).
2. **La SUSTITUCIÓN por `ICacheProvider` NO se absorbe:** acá **no** se introduce el contrato, ni sus regiones, ni
   el cap, ni los TTL, ni una caché interina propia. Eso es del **Bolt 013**, que ejecuta **después**. §15 lo
   declara como condición de parada.
3. **Secuencia final `014-ampliado → 013`**, con sus **dos constancias** citadas (§5/§17/§19): la del Bolt 013
   (`MEM-261016-0328`, finding «CORRECCIÓN DE SECUENCIA») y la de este bloqueo (`MEM-261016-0343` §13.1, salida
   (d)). La condición de parada de la frontera del caché de la revisión 2 **se reemplaza** —el retiro **está en
   alcance**— y se **conserva su historial** (§15, §13, comentario de cabecera).
4. **Limpieza de lo que quedó sin efecto, señalada:** la precondición de §5 «la capa con sus **puntos de caché ya
   sustituidos** (Bolt 013)» **ya no aplica** (los sustituye el 013 **después**, y este Bolt los retira), y la
   entrada de `prerequisites` del Bolt 013 pasa a describir su rol real: **frontera y paso siguiente**, no puerta de
   entrada. Ambas quedan anotadas **señalando el cambio**, sin borrar el historial.
5. **Lo que NO cambia:** el proveedor único y su línea con los dos criterios, el punto único de conversión y su
   excepción tipada, el interior tipado y la superficie (`SqlHelper`/`SqlHelperParameterCache` a `internal`), el
   retiro de las esperas activas, el timeout y la cadena desde la configuración portable, la política de nulabilidad
   de §C.5, las fronteras con los Bolts 011/015/016/006 y el plan de integración `applicable` con su corrida
   cubriente siguen **en vigor**. El `review` vuelve a vacío porque la revisión es material y se re-aprueba por el
   revisor.

**Nota al aprobador — (histórica) qué cambió la revisión 2, y por qué (material, G15/G16).** Es una revisión **preventiva**:
**no** responde a un bloqueo de este Bolt —no hay V-Bounce todavía— sino a la **trampa ya medida** en esta ola (los
Bolts **003, 007 y 009** chocaron con la nulabilidad del árbol al aportar código del 1.4 y cada uno necesitó su
revisión material; los Bolts **022, 008 y 013** la previeron por la misma vía), sabiendo que **este Bolt aporta la
capa `Data` completa** (`MEM-261016-0328` §1). Cambia **una sola cosa** y **cita** dos constancias:

1. **Política de nulabilidad del código aportado del 1.4 (§C.5, nueva).** `#nullable disable` como **primera línea
   útil** **sólo** de los archivos **aportados del 1.4** que **produzcan** `CS8618`/`CS8600`/`CS8603`/`CS8625`
   —criterio **objetivo**: *archivo aportado + diagnóstico de nulabilidad*; **el build es la prueba**—, **jamás** en
   código nuevo (el punto único de conversión, la excepción tipada y las pruebas conservan la nulabilidad
   **activa**) y **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es el **espejo** de
   `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3, `SPEC-261016-0037` §B.2, `SPEC-261016-0034` §B.4,
   `SPEC-261016-0035` §E.3 y `SPEC-261016-0019` §B.2 (los seis ya aprobados).
2. **Límite explícito (no autorizado):** un `using` muerto **hacia un namespace que no existe en el árbol** que
   **no** provenga de una corrección propia sigue siendo **parada y reporte** (§C.5, §15); lo que **sí** se retira
   con las correcciones propias (el `providerName`, los casts, el `SqlCommand` descartable) es el `using` que queda
   **sin uso** por ellas.
3. **Constancia de secuencia, citada y sin cambio de alcance:** el `AITL-MEM-Approval` del Bolt 013 dejó constancia
   de que **`US-000.BOLT-014` ejecuta ANTES que `US-000.BOLT-013`** (`MEM-261016-0328`, finding «CORRECCIÓN DE
   SECUENCIA»): la dependencia declarada de este Bolt sobre el 013 era **precautoria de doble-toque** sobre los dos
   archivos compartidos, **no** una puerta de entrada. Queda citada en §5/§17/§19 para que el ejecutor **no** se
   detenga por una precondición invertida; la frontera de aspecto con el 013 sigue **idéntica** (§4/§14).
4. **Aviso de frontera del caché del host, con condición de parada:** `HttpContext`/`System.Web` viven **dentro**
   de `BaseGateway.cs`/`BaseLoader.cs` y su retiro es el **aspecto del Bolt 013**. Si al aportar la capa ese retiro
   aparece como **paso necesario para compilar**, la respuesta es **parar y reportar** y escalar la secuencia
   (patrón de la revisión de secuencia) — **nunca** absorber la sustitución del 013 ni el retiro del caché del host
   (G11). §15 lo declara como condición de parada propia.
5. **Lo que NO cambia: todo lo demás.** El proveedor único y su línea con los dos criterios, el punto único de
   conversión, el interior tipado y la superficie (con `SqlHelper`/`SqlHelperParameterCache` a `internal`), el
   retiro de las esperas activas, el timeout y la cadena de conexión desde la configuración portable, las fronteras
   con los Bolts 011/013/015/016/006 y el plan de integración `applicable` con su corrida cubriente siguen **en
   vigor**. El `review` vuelve a vacío porque la revisión es material y se re-aprueba por el revisor.

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (datos / DBA, riesgo `high`)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
