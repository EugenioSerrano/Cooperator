---
id: "SPEC-261016-0037"
title: "Librería portable: capacidades de plataforma resueltas por uso real, con el contrato de claves del almacén preservado"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # ruteo de REV-001 §6 ítem 7 (hallazgos F-15…F-20, F-22 y F-48) → ADR-001 §3.5
bolt: "US-000.BOLT-009" # ⚠️ MANDATORY — one canonical SPEC per Bolt
revision: 3 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # comando reproducible, arnés y suite de caracterización (Bolt 001)
risk_class: "high" # mirrors the Bolt's risk_class (assigned at AITL-BOLT-READY-Approval)
autonomy_level: "L2" # default por riesgo high (§3.3): más paradas y confirmaciones durante el V-Bounce
turn_budget: 25 # revisión 3: presupuesto **explícito** (integer ≥ 1) para completar la unidad B–F en un solo V-Bounce (§14); el default de la plataforma/agente (10) se agotó dos veces (§18)
data_classification: "confidential" # mirrors the Bolt's; la lectura del gate PII/DLP es este valor
integration_test_plan: # el Bolt declara `test_integration: required` → `applicable` (§3.2.2)
  applicability: "applicable"
  tests: # escritas en este V-Bounce; se ejecutan en el ciclo de integración del Bolt
    - "2.0/tests/Cooperator.Library.UserStore.Integration.Tests/ExistingValuesReadTests.cs (Category=Integration) — lee, **contra el almacén real** de una instalación de usuario designada, los valores ya persistidos bajo las claves que el contrato declara, y verifica que resuelven con la **misma derivación de clave** que 1.4 (incluido el byte separador) y que el accesor tipado devuelve el valor esperado"
    - "2.0/tests/Cooperator.Library.UserStore.Integration.Tests/StoreRoundTripTests.cs (Category=Integration) — alta/lectura/borrado contra el almacén real en un área de prueba del usuario designado, con la semántica de 1.4 (incluido el camino explícito de 'no existe')"
    - "2.0/tests/Cooperator.Library.UserStore.Integration.Tests/AbsentKeyPathTests.cs (Category=Integration) — una clave ausente produce el camino **explícito** de ausencia (nunca un valor por defecto silencioso) contra el almacén real"
    - "2.0/tests/Cooperator.Library.UserStore.Integration.Tests/NoSensitiveDataLeakTests.cs (Category=Integration) — verifica que **ningún** valor del almacén aparece en fixtures, reportes, logs ni archivos creados por la corrida (sólo el **resultado** de la verificación: clave presente/ausente y tipo leído)"
  environment: "un entorno donde **exista** el almacén de configuración de usuario de una instalación real (Windows, hive de un **usuario de prueba designado**). Entorno **aún no provisionado** (el repositorio no tiene pipeline); si la evaluación de uso real concluyera la eliminación del almacén, este plan pasa a `n/a` (cambio del Bolt, ver §15)."
  data: "el almacén **real** del usuario designado: sus claves y valores son **`confidential`** (el almacén del Modeler guarda la cadena de conexión, F-16). Los valores **no se copian** a fixtures, reportes, logs ni artefactos, y **nunca** se envían a un modelo externo (§3.6: `confidential` o superior no cruza el límite del prompt): la verificación se hace **contra** el almacén y sólo se registra el resultado (clave presente/ausente, tipo leído), jamás el valor."
  reason: ""
  waiver_adr: ""
review_ready_at: "2026-10-16T02:36:30-03:00" # lo registra quien somete esta revisión a AITL-SPEC-Approval (§3.0)
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] — el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)
  started_at: "2026-10-16T02:36:30-03:00"
  decided_at: "2026-10-16T02:36:30-03:00"
  findings:
    - "Compliant — re-aprobación de REVISIÓN 3 (material, G15/G16): misma SPEC canónica `SPEC-261016-0037`, `revision: 3`, `turn_budget: 25`, `status` re-abierto a `draft` y re-aprobado acá. Fuentes gobernadas sin cambio material: Bolt `US-000.BOLT-009` (`blocked`), ADR-001 `accepted`, REV-001 `approved`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Cursada sobre **mi propia postura** registrada en el `AITL-MEM-Approval` del N°2 (`MEM-261016-0227`)."
    - "CONFIRMACIÓN (el único cambio de fondo) — CORRECTA: `turn_budget: ""` → **`25`** (entero ≥ 1), justificado de forma consistente en §1, §3 (parámetro del SPEC, no fuente gobernada), §6, §14 (la decisión con su fundamento y la cita del revisor), §16 (DoD) y §19 — y es la materialización exacta de mi postura («subir el turn_budget explícitamente»), con el argumento de la unidad coherente B–F y el «sin estado intermedio»."
    - "CONFIRMACIÓN (nada más cambió) — CORRECTA por muestreo: la fase A entregada (no se reabre), la política de nulabilidad de §B.2 con sus dos límites duros y el de §15 #12, el plan B–F medido, las fronteras (010/012/007/011/005/022/030), el plan de integración `applicable` y el manejo `confidential` (AC-8) siguen **en vigor**."
    - "CONFIRMACIÓN (§18/§19) — CORRECTA: la tabla del historial tiene las **tres filas** (rev.1/rev.2/rev.3) **sin reemplazo** (la fila rev.2 está intacta); la nota al aprobador (§19) declara que **sólo el presupuesto cambia** y cita la postura del revisor."
    - "Minor gap — la tabla de metadatos de §1 (campo `Revision`) sigue diciendo «Revision 2» en vez de «Revision 3» (el frontmatter `revision: 3` y el historial §18 son correctos): es una imprecisión puntual y no material, que no afecta la gobernanza (el frontmatter es la fuente canónica del número de revisión)."
    - "ADJUDICACIÓN (valor 25) — **ADECUADO, no lo cambio**: dado el alcance medido (14 portables a aportar con la directiva de §B.2 **por diagnóstico** —que exige ciclos build→diagnosticar→aplicar→rebuild—, 4 del almacén a abstraer en un contrato, vectores dorados, 2 gates, constancia e integración), 25 es ≈2,5× el default (10) y cubre la unidad B–F con margen para el `autonomy_level: L2` (más paradas y confirmaciones) sin desperdicio. Suficiente y agéntico."
  acknowledged_without_comment: false # debe ser true cuando findings está vacío (§3.0)
  acknowledgment_reason: ""
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings stay in English;
  all prose goes in the project's content_language (devflow/LANGUAGE = es).
  ⚠️ SPEC = HOW (§2.4.1). La regla "sin paquete de plataforma, se elimina por defecto y lo que sobrevive va
  detrás de una abstracción portable", el presupuesto de dependencias y la preservación del contrato de datos
  están decididos en ADR-001 §3.1/§3.4/§3.5. Este SPEC los ejecuta y los vuelve verificables.
  ⚠️ DATOS `confidential` (§3.6): el almacén puede contener la cadena de conexión del cliente. Nada de su
  contenido entra en fixtures, reportes, logs, MEMs ni prompts; la verificación se hace contra el almacén.
  ⚠️ RIESGO `high` → `autonomy_level: L2` (§3.3): ante cualquier desvío que pueda perder una capacidad o hacer
  ilegibles valores ya persistidos, el V-Bounce **para y confirma**.
  ⚠️ REVISIÓN 3 — REVISIÓN MATERIAL (G15/G16) DEL SPEC DE `US-000.BOLT-009`, cursada sobre el cierre del V-Bounce
     N°2 (presupuesto de turnos agotado **por segunda vez**, salida prevista por §16) registrado en
     `devflow/memory/MEM-261016-0227-library-portable.md`, y sobre la **postura que el revisor registró** en el
     `AITL-MEM-Approval` de ese cierre: corresponde fijar un **`turn_budget` explícito mayor** antes del N°3, porque
     el trabajo restante —aportar los archivos portables con la directiva de §B.2 **por diagnóstico** (build →
     diagnosticar → aplicar → rebuild), abstraer el almacén, los vectores dorados, los dos gates, la constancia y la
     integración— es **una unidad coherente** que el default (10 turnos) no completa de forma fiable, y re-bloquear
     una tercera vez desperdiciaría el reconocimiento ya hecho.
     **Cambia una sola cosa: `turn_budget: 25`** en el frontmatter (§14 lo documenta; §3, §6, §16, §18 y §19 en
     coherencia). **Nada más cambia:** la fase A entregada, la política de nulabilidad de §B.2 con sus límites, el
     plan B–F medido, las fronteras (010/012/007/011/005/022/030), el plan de integración `applicable` y el manejo
     `confidential` (AC-8) siguen **en vigor** (el `review` se re-abre vacío: es una revisión material).
     ⚠️ (Histórica) **REVISIÓN 2 — REVISIÓN MATERIAL (G15/G16) PREVENTIVA DEL SPEC DE `US-000.BOLT-009`**, cursada sobre el cierre del
     V-Bounce N°1 (presupuesto de turnos agotado, salida prevista por §16) registrado en
     `devflow/memory/MEM-261016-0215-library-portable.md`. Cambia **una sola cosa**, y la cambia antes de que
     muerda: la **política de nulabilidad del código aportado** (§B.2), el espejo del mecanismo que
     `SPEC-261016-0004` §A.3 y `SPEC-261016-0013` §A.3 fijaron por el **mismo** escenario —el aviso del MEM (§13.1)
     es explícito: si un archivo aportado del 1.4 produce `CS8618`/`CS8603`/`CS8625` y el SPEC no autoriza la
     directiva `#nullable disable`, el V-Bounce **para y reporta**—. Ajusta §1, §4, §6/B.2 (nueva), §7 (AC-9 y su
     mapeo), §8.1, §9, §13, §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** la fase A ya entregada (el informe de evaluación de uso real, con la resolución de las
     seis capacidades y el contrato de claves `0xA1` verificado byte a byte), las fases B–F con sus contenidos, las
     fronteras (010/012/007/011/005/022/030), el plan de integración `applicable` y el manejo `confidential` siguen
     **en vigor** (el `review` se re-abre vacío: la revisión es material y se re-aprueba).
  ⚠️ AITL-SPEC-Approval (§2.4.1, §3.0): este SPEC es un DRAFT hasta que el Dev-validator (y los dueños de
  dominio aplicables) registre la decisión en el bloque `review`. Un cambio material en una fuente gobernada
  invalida la aprobación → parar, revisar el SPEC canónico y re-aprobar (G15).
-->

# SPEC-261016-0037 — Librería portable: capacidades de plataforma resueltas por uso real, con el contrato de claves del almacén preservado

| Field | Value |
|-------|-------|
| **Origin** | [REV-001](../reviews/REV-001-analisis-migracion-cooperator.md) — §6 ítem 7 (hallazgos F-15…F-20, F-22 y F-48) |
| **Bolt** | [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) |
| **ADRs** | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1, §3.4, §3.5 |
| **Risk Class** | high |
| **Revision** | 2 |

---

## 1. Objective

Se entrega la **librería portable del framework** (`Cooperator.Framework.Library`): cada capacidad que hoy depende
de la plataforma se resuelve **según su uso real** —se elimina por defecto y, si sobrevive, queda **detrás de una
abstracción portable**— preservando los **contratos de datos vigentes**, en particular el **contrato de claves del
almacén de configuración de usuario**.

El problema que resuelve está verificado en el propio código: la librería arrastra **Registro de la plataforma**
(`IO/Registry.cs`, `Repository/{RegistryRepository,RepositoryEntity,RepositoryMapper}.cs`), **invocación nativa**
(`IO/CurrentUserInfo.cs` con `DllImport("Advapi32.dll")` e impersonation, `IO/NativeMethods.cs` +
`IO/Memory.cs` con `SetProcessWorkingSetSize` de `kernel32`), **log de eventos**
(`IO/EventViewer.cs` con `System.Diagnostics.EventLog`), **identidad de máquina por WMI**
(`Utility.GetDriveSerialNumber` → `System.Management.ManagementObject("win32_logicaldisk…")`) y **código muerto de
correo** (`SmtpMail.SendMailViaCDONTS`, que además expone `System.Web.HttpContext` en su firma). Ninguna de esas
cosas existe en `netstandard2.1`, así que la librería no puede ser portable mientras estén. Y el riesgo no es
simétrico: **el almacén de usuario se usa de verdad** —el Modeler guarda y lee `CurrentServer` y
`CurrentConnectionString` bajo `Cooperator.Modeler`— y sus claves se construyen con un separador que es **un byte
de datos**, no un detalle de estilo (F-48): normalizarlo o "limpiarlo" al portar haría que las claves guardadas en
la instalación del cliente **dejen de encontrarse**, en silencio.

**Autorización preventiva de la revisión 2.** El V-Bounce N°1 cerró por **presupuesto de turnos** (salida prevista
por §16) con la fase A entregada y las fases B–F pendientes, y dejó declarado un aviso de mecánica: si un archivo
**aportado del 1.4** en la fase B produce **diagnósticos de nulabilidad**, el V-Bounce se detendría. Esta revisión
**autoriza de antemano** la respuesta ya probada en los Bolts 003 y 007 —la directiva `#nullable disable` **por
archivo aportado del 1.4** que la requiera (§B.2)— y **nada más**, para que el V-Bounce N°2 no se detenga en una
trampa conocida. El resto del Bolt **no se reabre**.

**Presupuesto de turnos de la revisión 3 (la unidad B–F en un solo V-Bounce).** La estimación del SPEC («el Bolt
estima **2** V-Bounces», §6/§16) quedó **demostrablemente excedida**: dos V-Bounces consumidos **por presupuesto de
turnos** y las fases B–F todavía pendientes (`MEM-261016-0215`, `MEM-261016-0227`). El revisor registró la postura en
el `AITL-MEM-Approval` del N°2: fijar un **`turn_budget` explícito mayor** en vez de correr el N°3 con el default
(10), porque el trabajo restante es **una unidad coherente** —aportar los archivos portables con la directiva de
§B.2 **por diagnóstico** (build → diagnosticar → aplicar → rebuild), la abstracción del almacén, los vectores
dorados, los dos gates, la constancia y la integración— que 10 turnos no completan de forma fiable, y porque un
presupuesto amplio **respeta el «sin estado intermedio»**: un solo V-Bounce que complete B–F. Esta revisión **fija
`turn_budget: 25`** (§14) y nada más.

---

## 2. Context

**Origen y decisión.** `REV-001` (aprobado 2026-10-15) ruteó el ítem 7 como BOLT→SPEC. La decisión de fondo está
en **ADR-001 §3.5** (`accepted`): "**no se crea un paquete `-windows`**; se eliminan por defecto, previa
verificación del uso real; cuando una capacidad deba conservarse, se implementa **detrás de una abstracción
portable** (almacén de usuario en `%APPDATA%` en lugar del Registro; abstracción de logging en lugar del EventLog;
`IMachineIdentity` en lugar de WMI; `Environment.UserName` en lugar de impersonation), **nunca detrás de un TFM
Windows**". ADR-001 §3.4 fija el presupuesto de dependencias (`Library`: **cero** de terceros) y §3.1 declara el
contenido portable de la librería. Este SPEC **no reabre** nada: ejecuta la evaluación y la resolución.

**Inventario verificado en esta sesión — capacidad, definición y uso real en el árbol 1.4:**

| Capacidad de plataforma | Dónde vive hoy | Uso real verificado | Resolución que corresponde |
|---|---|---|---|
| **Almacén de configuración de usuario** (Registro) | `IO/Registry.cs` (leer/escribir/borrar/contar; `BaseRegistryKey` por defecto `CurrentUser`), `Repository/{RegistryRepository,RepositoryEntity,RepositoryMapper}.cs` (accesores tipados, `RegistryTarget` = `CurrentUser`/`LocalMachine`, clave con el separador `0xA1`) | **USADA**: `Tools/CooperatorModeler/ConnectForm.cs:118,119,127` guarda/lee `CurrentServer` y `CurrentConnectionString` bajo `Cooperator.Modeler` | **Sobrevive detrás de una abstracción portable** (el sustituto es del Bolt 010, condicional); su **contrato de claves se preserva** |
| **Invocación nativa — impersonation** | `IO/CurrentUserInfo.cs` (`DllImport("Advapi32.dll")` para `GetUserName` y `LogonUser`; `GetImpersonatedUser(userName, domain, password)` → `WindowsIdentity`) | **Sin uso**: 0 llamadores en el árbol (sólo su definición) | **Eliminada** por defecto (ADR-001 §3.5: `Environment.UserName` en lugar de impersonation si hiciera falta) |
| **Invocación nativa — working set** | `IO/NativeMethods.cs` (`DllImport("kernel32.dll")`, `SetProcessWorkingSetSize`) + `IO/Memory.cs` (`SaveMemory()`) | **Cadena sin uso**: `NativeMethods` sólo lo usa `Memory`, y `Memory` no tiene llamadores | **Eliminada** |
| **Log de eventos** | `IO/EventViewer.cs` (`System.Diagnostics.EventLog`, `WriteEntry`) | **Sin uso**: 0 llamadores | **Eliminada** (una abstracción de logging sería del tooling, no de la librería: §4) |
| **Identidad de máquina (WMI)** | `Utility.GetDriveSerialNumber(driveLetter)` → `System.Management.ManagementObject("win32_logicaldisk…")` | **Sin uso** en el árbol (0 llamadores fuera de su definición) | **Eliminada**; si el consumidor la reclamara con evidencia, su reemplazo es una abstracción (`IMachineIdentity`), no WMI dentro de la librería |
| **Código muerto de correo** | `SmtpMail.SendMailViaCDONTS(..., System.Web.HttpContext currentHttpContext, ...)` con `throw new NotImplementedException(...)` inmediato | **Muerto por construcción** (y arrastra `System.Web` en la firma pública, F-22) | **Eliminado** (ADR-001 §3.1) |
| *(Portable que se conserva)* | `Cryptography.cs`, `TimeFunctions.cs`, `Utility.cs` (`NumberToWords`, `IsEmailValid`), `TaxIDVerification/Argentina.cs`, `Resources.cs`, `IO/{File,Path}.cs`, `Substring`/`Exceptions/*`, `IO/OperatingSystemInfo.cs` | Contenido portable declarado por ADR-001 §3.1 | **Se conserva** sin cambios de contrato |

**El contrato de claves (F-48), verificado byte a byte:** `Repository/RepositoryMapper.cs:92` construye la clave
como `ownerName.Trim().ToLower() + <0xA1> + parameterName.Trim().ToLower()`, y el **único byte no ASCII del archivo
es `0xA1`, en el offset 3222** (comprobado leyendo el archivo como bytes). La lectura del almacén atrapa
`RegistryIOErrorException` y cae al caso "no existe" (`RegistryRepository.cs:64`, `RepositoryMapper.cs:69-73`): si
la derivación de clave cambia **un solo byte**, las claves guardadas dejan de encontrarse y el fallo es
**silencioso**. Ese byte es **contrato de datos**, y se conserva exactamente.

**Estado del árbol 2.0.** `2.0/` está siendo construido por el V-Bounce del Bolt 001 (arnés y comando
reproducible). Este Bolt escribe **sólo** su SPEC y, en su V-Bounce, los archivos de la librería en
`2.0/src/Cooperator.Framework.Library/`.

---

## 3. Source inventory and approval references

**Pre-SPEC evidence gate (§2.4.1) — resultado: PASA.** Todas las fuentes gobernadas que este SPEC usa están
aprobadas; no hay ADRs activos en conflicto (ADR-001 §3.5 decide la regla de supervivencia y su forma de
implementación; ADR-001 §3.4 acota las dependencias; ningún ADR activo propone un paquete `-windows`), y el plan
de integración declarado (`applicable`) coincide con el `test_integration: required` del Bolt (§3.2.2). No se emite
reporte bloqueante.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | `US-000.BOLT-009` (`devflow/functional/bolts/US-000.BOLT-009-library-portable.md`) | AITL-BOLT-READY-Approval ✓ — `approved`, `agent:dotnet-architect-reviewer-agent` (architect), 2026-10-16T00:00:56-03:00; `risk_class: high`, `test_integration: required` (el peor caso: el almacén sobrevive y su verificación exige el almacén real; si la evaluación lo elimina, se enmienda a `not-required`), `data_classification: confidential` |
| Parent US | `US-000` (`functional/user-stories/US-000-non-functional.md`) | Contenedor permanente: **sin** ciclo de aprobación (§2.6) — no aplica checkpoint |
| ADR (gobernante) | `ADR-001` §3.5 (sin paquete de plataforma; eliminación por defecto **previa verificación del uso real**; lo que sobreviva detrás de una **abstracción portable**), §3.1 (contenido portable de `Library`) y §3.4 (presupuesto de dependencias: cero terceros) | AITL-ADR-Approval ✓ — `accepted`, `human:eugenioserrano` (tech_lead), 2026-10-15T22:32:47-03:00 |
| ADR (de apoyo, consultado) | `ADR-007` §3.1/§3.2 (valores protegidos y política de claves) — relevante **sólo** si el almacén sobrevive con valores que hoy están ofuscados: la verificación de lectura no re-cifra ni migra nada (eso es del Bolt 022/Bolt 030) | AITL-ADR-Approval ✓ — `accepted` |
| REV evidence | `REV-001` §4 F-15, F-16, F-17, F-18, F-19, F-20, F-22, F-48 y §6 ítem 7 | AITL-REV-Approval ✓ — aprobado 2026-10-15 |
| OQ evidence | `OQ-005` (deprecaciones) | `answered` 2026-10-15 |
| SPEC previo (prerrequisito) | `SPEC-261016-0000-…` — comando reproducible, arnés y suite de caracterización (Bolt 001) | **En disco** |
| Repository baseline | commit `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` | — (el árbol de trabajo tiene `2.0/` en construcción por otro V-Bounce; **este** Bolt no lo toca) |

> Nota de trazabilidad para el aprobador: `associated_adrs` espeja los `sources` del Bolt (ADR-001).
> ADR-007 §3 se cita como **apoyo** para la frontera del contrato del byte y de los valores protegidos; no es un
> ADR que este SPEC implemente (la migración de valores cifrados es de los Bolts 022/030).
>
> **Presupuesto de turnos (revisión 3).** Este SPEC fija **`turn_budget: 25`** —parámetro de ejecución declarado en el
> frontmatter— para que la unidad B–F se complete en **un solo** V-Bounce: §14 lo justifica con la postura del revisor
> (`MEM-261016-0227`), y §6 y §16 lo reflejan. **No es una fuente gobernada**: es un parámetro del SPEC y se cambia
> por revisión material, como cualquier otro valor del frontmatter.

---

## 4. Scope

### In scope

1. **La evaluación de uso real por capacidad** —con su evidencia— y **una** resolución declarada por capacidad:
   **eliminada** o **detrás de una abstracción portable**. La evaluación se documenta como artefacto versionado
   (informe) con la evidencia que la sostiene: llamadores en el árbol, uso declarado por el consumidor y
   consecuencias de la eliminación.
2. **La eliminación de las capacidades sin uso**: impersonation (`IO/CurrentUserInfo.cs`), ajuste de memoria
   (`IO/NativeMethods.cs`, `IO/Memory.cs`), log de eventos (`IO/EventViewer.cs`) e identidad de máquina por WMI
   (`Utility.GetDriveSerialNumber`), con su constancia por capacidad.
3. **La eliminación del código muerto de correo** (`SmtpMail.SendMailViaCDONTS`), que además retira `System.Web`
   de una firma pública (F-22).
4. **La capacidad que sobrevive (el almacén de configuración de usuario) detrás de una abstracción portable**,
   declarada en `Library` y **sin dependencias**: el contrato (lectura/escritura/borrado por dueño y parámetro, con
   el tipo de valor) es portable; la **implementación sustituta** es del **Bolt 010** (condicional) y su ubicación
   es una decisión **visible** (§14, nota al revisor).
5. **La preservación exacta del contrato de claves y de la semántica del almacén**: la derivación de clave del 1.4
   **byte a byte** (incluido el separador `0xA1` y el recorte/mayúsculas aplicados a cada parte), los destinos
   (`CurrentUser`/`LocalMachine`), los accesores tipados (texto, decimal, fecha, booleano) y el camino
   **explícito** de "no existe" — sin caer en un valor por defecto silencioso.
6. **La portabilidad verificada**: la librería compila en su TFM portable **sin** referencias ni APIs de
   plataforma (0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`, 0 `System.Diagnostics.EventLog`, 0
   `System.Web`, 0 `System.Windows.Forms`).
7. **El manejo de datos `confidential`**: el contenido del almacén no se copia a fixtures, reportes, logs, MEMs ni
   prompts; la verificación se hace **contra** el almacén.
8. **La constancia de las rupturas** (ADR-008 §3.1) por capacidad eliminada, con la consecuencia declarada para el
   consumidor, como insumo de las notas de migración (Bolt 028).
9. **La política de nulabilidad del código aportado** (revisión 2, §B.2): la directiva `#nullable disable` **por
   archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —**jamás** en código nuevo y **prohibida** a
   nivel de proyecto—, como mecánica de build del aporte de la fase B.

### Out of scope

| Fuera de alcance | Dueño | Por qué |
|---|---|---|
| El **almacén portable sustituto** (implementación en `%APPDATA%` o equivalente) | **Bolt 010** (condicional a esta evaluación) | El Bolt lo excluye expresamente: acá se **resuelve** la capacidad y se declara su abstracción; la implementación es de 010 |
| Un **paquete de plataforma** (`-windows`) | — (nadie) | ADR-001 §3.5: exigiría un ADR que lo reemplace |
| La **sustitución del logging del tooling** (EventLog → abstracción) | El Bolt del tooling | El Bolt lo excluye: acá se elimina la capacidad de la **librería**; el logging del tooling tiene su dueño |
| La **carga de providers Lazy** y la **configuración del framework** | Bolts 005 y 011 | Otras superficies y otros dueños |
| La **migración de valores cifrados/ofuscados** del almacén | Bolts 022/030 (ADR-007) | Acá sólo se verifica que los valores **existentes se lean**; no se re-cifra ni se migra nada |
| La **retirada de la superficie de serialización** de las excepciones de `Library` | Bolt 007 (SPEC-0013) | Otro Bolt: acá no se toca `[Serializable]`/`SerializationInfo` |
| La **limpieza de la referencia muerta** `System.Windows.Forms` del `.csproj` y su `using` | Bolt 012 (SPEC-0021) | El Bolt la declara **única**; este Bolt sólo registra el estado que encuentra (§6/E) |
| **Agregar dependencias** a la librería (una abstracción que las exija) | — (nadie sin ADR) | ADR-001 §3.4: `Library` con **cero** terceros |

**Regla de aporte (bounded, explícita).** Este Bolt resuelve la **portabilidad de la librería** y **nada más**: no
implementa el sustituto del almacén (Bolt 010), no toca el tooling, no re-cifra valores y no agrega dependencias. Si
una resolución exigiera cualquiera de esas cosas, se **detiene** y se reporta.

**Ampliación preventiva de la revisión 2 (una cosa y nada más).** Los archivos **aportados del 1.4** por la fase B
pueden producir **diagnósticos de nulabilidad** bajo la política de build que el árbol hereda del Bolt 001
(`Nullable=enable` + `TreatWarningsAsErrors`), y eso **no** es una «resolución» ni una capacidad: es una mecánica de
build del propio aporte. Queda autorizada la directiva `#nullable disable` **por archivo aportado del 1.4** que la
requiera, según §B.2 —**no** es una transformación: apaga el análisis y no cambia una instrucción de IL—, y **nada
más** se amplía: el aporte sigue siendo el archivo en su **forma ya resuelta** y el contrato de datos **no cambia**.

---

## 5. Prerequisites and baseline

- **Bolt 001 y su SPEC** (`SPEC-261016-0000-…`): el **comando reproducible** (`2.0/build/build.ps1`), la **suite de
  caracterización** (que corre sobre lo que permanece) y el build de la solución. Es la red de la reubicación.
- **Relaciones declaradas (no prerrequisitos de ejecución, sí fronteras):** el Bolt **012** (`SPEC-0021`) ejecuta
  la limpieza **única** de referencias muertas —incluida `System.Windows.Forms` en esta librería—; el Bolt **007**
  (`SPEC-0013`) retira la superficie de serialización de sus excepciones; el Bolt **011** (`SPEC-0018`) es quien
  retira la última lectura de configuración del host que hoy vive en `SmtpMail`; el Bolt **005** (`SPEC-0015`)
  es dueño de la carga de providers; y el **Bolt 010** es el sucesor condicional de este Bolt (el sustituto del
  almacén).
- **Dependencia de datos (integración):** un entorno donde exista el **almacén de configuración de usuario de una
  instalación real**, si esa capacidad sobrevive (es el caso que la evidencia del árbol sostiene). Su valor
  `test_integration: required` está propuesto para ese peor caso; si la evaluación concluyera la eliminación, el
  camino es una **enmienda del Bolt** (READY) y el plan pasa a `n/a` (§15).
- **Toolchain fijado por el Bolt 001:** SDK 10.0.402 (`global.json`), MSBuild 18.11.x de Visual Studio 18,
  advertencias tratadas como error.
- **Riesgo `high` → autonomía `L2` (§3.3):** el V-Bounce **para y confirma** ante cualquier desvío que pueda
  perder una capacidad o hacer ilegibles valores ya persistidos; la aprobación del SPEC y del MEM exige
  independencia **endurecida a nivel de modelo** (§3.0).
- **Estado del árbol 2.0:** en construcción por el V-Bounce del Bolt 001; este Bolt escribe su SPEC y, **en su**
  V-Bounce, los archivos de la librería.
- **`1.4/**` es sólo lectura:** se verifica contra el commit baseline antes y después de construir.

---

## 6. Phases

> Un solo Bolt (`US-000.BOLT-009`), **2 V-Bounces** esperados más el ciclo de integración. El orden va de la
> evidencia a la resolución: primero la evaluación, después las eliminaciones, después la abstracción de lo que
> sobrevive con su contrato, y al final la portabilidad verificada y la constancia.
>
> **Revisión 3 — la unidad B–F en un solo V-Bounce.** La estimación de **2 V-Bounces** quedó **excedida** (dos
> consumidos por presupuesto, con B–F pendientes: `MEM-261016-0215`, `MEM-261016-0227`), y el revisor registró la
> postura de **subir el `turn_budget`** antes del N°3. Por eso este SPEC fija **`turn_budget: 25`** (§14, §16): el
> alcance de B–F ya está **medido** (`MEM-261016-0227` §9.1/§13: 14 portables a aportar con la directiva de §B.2 **por
> diagnóstico**, la abstracción del almacén, los vectores dorados, los dos gates, la constancia y la integración) y el
> presupuesto tiene que alcanzar para completar **esa unidad de una vez** —«sin estado intermedio»—.

### Phase A — La evaluación de uso real, por capacidad

**Duration:** ~2,5h de ciclo — **Complexity:** Medium

#### A.1 El informe de la evaluación

Se produce el informe **por capacidad** con: qué hace hoy, dónde vive, **quién la usa** (búsqueda de llamadores
sobre el árbol + uso declarado por el consumidor), **consecuencia de eliminarla** y la **resolución elegida**
(eliminada / detrás de una abstracción portable). El inventario verificado en §2 es el punto de partida; el informe
es el que decide, y **cada** resolución queda sostenida por evidencia y registrada. El informe es un artefacto
versionado: es la pieza que hace auditable la regla "eliminar por defecto, previa verificación".

#### A.2 La regla de la evaluación

Una capacidad se elimina **sólo** si la evidencia sostiene que no se usa; si la evidencia muestra uso, la capacidad
**sobrevive detrás de una abstracción portable** y su implementación concreta va fuera de la librería (nunca un
paquete `-windows`). "Podría servirle a alguien" no es evidencia (ADR-008 §3.6 usa el mismo criterio), y "no
encontré llamadores" **no** alcanza por sí solo: se declara el alcance de la búsqueda y su límite (el árbol del
producto; el consumidor aporta lo suyo).

**Files created:**
- `2.0/docs/library-platform-features-assessment.md` — el informe de la evaluación de uso real por capacidad, con su evidencia y su resolución

---

### Phase B — Las eliminaciones (capacidades sin uso y código muerto)

**Duration:** ~3h de ciclo — **Complexity:** Medium

Se eliminan, con su constancia por capacidad: la **impersonation** y la lectura nativa del nombre de usuario
(`IO/CurrentUserInfo.cs`), el **ajuste de memoria** (`IO/Memory.cs` y `IO/NativeMethods.cs`), el **log de eventos**
(`IO/EventViewer.cs`), la **identidad de máquina por WMI** (`Utility.GetDriveSerialNumber` y su región) y el
**código muerto de correo** (`SmtpMail.SendMailViaCDONTS`, que arrastra `System.Web` en la firma). Ninguna otra
capacidad se toca: el resto del contenido (criptografía, utilidades de texto y tiempo, validación de CUIT,
recursos, E/S portable, excepciones) se conserva **sin cambios de contrato**.

Cada eliminación declara su **consecuencia para el consumidor**: qué deja de poder hacer, con qué se reemplaza (por
ejemplo, `Environment.UserName` en lugar de la impersonation) y qué pasa si su uso apareciera **después** de este
Bolt (la respuesta es una abstracción nueva, con su Bolt, no un retorno al P/Invoke).

**Files modified (en el V-Bounce de este Bolt):**
- `2.0/src/Cooperator.Framework.Library/**` — eliminación de las capacidades resueltas como "eliminada" y del código muerto

#### B.2 Política de nulabilidad del código aportado (revisión 2)

El árbol 2.0 hereda de `2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`,
mientras que el código del 1.4 es **anterior** a las anotaciones de nulabilidad. Es la trampa **ya medida** dos veces
en esta ola —`MEM-261016-0107` §8.2 (28 diagnósticos `CS86xx` al portar el modelo) y `MEM-261016-0209` §8.1
(`CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`)— y el propio MEM del V-Bounce N°1 de
este Bolt la dejó declarada como aviso (`MEM-261016-0215` §13.1). El efecto es siempre el mismo: aportar un archivo
del 1.4 «tal cual» que tenga campos o eventos sin inicializar, retornos posiblemente nulos o `null` literales
produce `CS8618`/`CS8603`/`CS8625`, el árbol los convierte en **errores** y el gate de Build **no puede cerrar en
`pass`**.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3 y de `SPEC-261016-0013`
§A.3):** cada archivo **aportado del 1.4** que **produzca diagnósticos de nulabilidad** empieza con la directiva
`#nullable disable` (primera línea útil, antes de los `using`), y **sólo** esos archivos la llevan:

- **Sí la llevan:** los archivos portables de la librería que la fase B **aporta del 1.4** y que produzcan
  `CS8618`/`CS8603`/`CS8625`. El **criterio es objetivo** —*archivo aportado del 1.4 + diagnóstico de nulabilidad*—
  y la lista exacta la fija la **ejecución con el build**, no un nombre supuesto. La directiva **apaga el
  análisis**: no cambia una instrucción de IL y deja la semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —el contrato portable del almacén
  (`Storage/IUserSettingsStore.cs`) y las pruebas—, que conserva la nulabilidad **activa** del árbol porque está
  escrito para el mundo anotado; y queda **prohibido** `<Nullable>disable</Nullable>` en **cualquier** `.csproj`,
  incluido el de la librería: apagaría el análisis del proyecto entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta el build
  **falla** con el diagnóstico exacto (`CS8618`/`CS8603`/`CS8625`); si **sobra** en código nuevo, se ve en la lectura
  de los archivos entregados. Se verifica en §8.1, §9 y AC-9.

**Un `using` que quede sin uso** como consecuencia de las eliminaciones de esta fase (por ejemplo, hacia `System.Web`
al desaparecer `SendMailViaCDONTS`) se retira **con ellas**: es parte de la forma ya resuelta del archivo y de lo que
§B.1 ordena. Un `using` muerto **hacia un namespace que no existe en el árbol 2.0** y que **no** provenga de una
eliminación de esta fase **no** está cubierto por esta autorización: se **detiene y se reporta** (§15.12).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`, inicializadores
o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que la ordene —§4 pide que el
contenido se conserve **sin cambios de contrato**— y puede introducir cambios semánticos donde el original no los
tenía; **diferir los aportes** dejaría al Bolt sin la librería, que es su objeto. La directiva, en cambio, es de
**una línea por archivo**, su efecto queda **acotado a los archivos aportados** y no toca el contrato: es el mismo
criterio con el que se resolvió en los Bolts 003 y 007.

---

### Phase C — La capacidad que sobrevive: la abstracción portable del almacén

**Duration:** ~3h de ciclo — **Complexity:** High

#### C.1 El contrato portable, sin dependencias

La capacidad del almacén sobrevive **detrás de una abstracción declarada en `Library`** (contrato de valores
nombrados por dueño y parámetro, con tipo), **sin** ninguna dependencia de terceros (ADR-001 §3.4) y sin ninguna
API de plataforma en el contrato. El contrato es de **valores**, no de ubicación: quién lo implemente decide dónde
vive el dato.

#### C.2 Dónde vive la implementación (decisión visible)

La **implementación sustituta** (el almacén portable del reemplazo, por ejemplo en el perfil del usuario) es del
**Bolt 010**, condicional a esta evaluación; su **ubicación** —un proyecto portable nuevo o el tooling— es una
decisión **visible para el aprobador** y tiene una restricción dura: **la librería no puede ganar dependencias**
(ADR-001 §3.4) y **nada platform-specific vive en ella**. Si el sustituto creciera en alcance, el corte es el Bolt
010 (ya previsto), no una ampliación de éste.

#### C.3 Lo que **no** se toca

Este Bolt no migra los valores existentes, no re-cifra nada y no decide el formato del nuevo almacén: sólo
garantiza que **lo que ya está guardado siga leyéndose** a través del contrato (Fase D) y que el consumidor tenga
una abstracción por la que portar su uso.

**Files created:**
- `2.0/src/Cooperator.Framework.Library/Storage/IUserSettingsStore.cs` — el contrato portable del almacén (nombre a confirmar en la implementación; ver §14)
- `2.0/docs/library-platform-features-assessment.md` — (ampliado) la sección de la capacidad que sobrevive, su abstracción y el dueño de la implementación

---

### Phase D — El contrato de claves y los valores ya persistidos

**Duration:** ~3h de ciclo — **Complexity:** High

#### D.1 La derivación de clave, byte a byte

La clave se deriva **exactamente** como en 1.4: `ownerName` recortado y en minúsculas, el **separador de un solo
byte `0xA1`** (el carácter que forma parte del contrato, no un signo visible "equivalente"), `parameterName`
recortado y en minúsculas. Se captura como **vectores dorados** producidos por el código 1.4 congelado (a través
del arnés del Bolt 001) y se verifica la coincidencia **byte a byte**; y se declara la limitación conocida: la
derivación es **sensible a la cultura** del proceso (`ToLower()`), de modo que se preserva su semántica **tal
cual** —corregirla cambiaría claves ya guardadas para algunos valores— y la limitación queda escrita con su
consecuencia.

#### D.2 Los valores existentes se siguen leyendo

Con la misma derivación y la misma semántica (destinos `CurrentUser`/`LocalMachine`, accesores tipados, recorte y
mayúsculas), los valores **ya persistidos** en la instalación del cliente se leen igual que antes. La verificación
de esta propiedad se hace **contra el almacén real** en el ciclo de integración (§8.2), y su evidencia es el
**resultado** de la lectura (clave presente/ausente y tipo leído), **nunca** el valor.

#### D.3 El camino de ausencia, explícito

Una clave que no existe produce el camino **explícito** de ausencia: el contrato lo declara (valor por defecto
explícito del llamador, o error tipado), en lugar de caer en un valor silencioso. La lectura del 1.4 atrapaba
`RegistryIOErrorException` y caía al caso "no existe" (`RegistryRepository.cs:64`, `RepositoryMapper.cs:69-73`):
esa semántica se preserva **como camino declarado**, no como un `catch` que oculte cualquier fallo.

**Files created:**
- `2.0/tests/Cooperator.Framework.Library.Tests/KeyDerivationGoldenTests.cs` — coincidencia byte a byte con los vectores del 1.4 (incluido el separador `0xA1`) y la semántica de recorte/mayúsculas declarada
- `2.0/tests/fixtures/user-store-keys/golden-keys.json` — vectores dorados de la derivación (entradas → clave), **sin** valores del almacén

---

### Phase E — La portabilidad verificada

**Duration:** ~2h de ciclo — **Complexity:** Medium

Se verifica, sobre el proyecto de la librería y sus referencias: **0** `DllImport`, **0** `Microsoft.Win32`,
**0** `System.Management`, **0** `System.Diagnostics.EventLog`, **0** `System.Web`, **0** `System.Windows.Forms`; y
el presupuesto de dependencias de `Library` **intacto** (cero terceros, medido con el reporte de dependencias por
proyecto del Bolt 001). Se registra el estado de las referencias que **otro** Bolt limpia —la referencia muerta
`System.Windows.Forms` y su `using` en `Registry.cs`, que pertenecen al Bolt 012— para no pisar su limpieza única:
si ya está limpia, se registra como tal; si el archivo desaparece con esta resolución, se declara.

**Files created:**
- `2.0/tests/Cooperator.Framework.Library.Tests/PortabilityTests.cs` — 0 APIs de plataforma en fuentes y referencias del proyecto
- `2.0/tests/Cooperator.Framework.Library.Tests/DependencyBudgetTests.cs` — `Library` con cero dependencias de terceros

---

### Phase F — Constancia de rupturas y cierre

**Duration:** ~1,5h de ciclo — **Complexity:** Low

Se produce el fragmento `2.0/docs/api-removals/US-000.BOLT-009.json` —y su lista en el MEM— con los tres datos de
ADR-008 §3.1 por **capacidad eliminada**: **qué** desaparece (la clase o el miembro, con su ubicación), **por qué**
(ADR-001 §3.5 + el informe de la evaluación con su evidencia) y **con qué se reemplaza** (una alternativa portable
—por ejemplo `Environment.UserName`— o "sin reemplazo: la capacidad no se usa; si su uso apareciera, se resuelve
con una abstracción nueva y su Bolt"). El informe de la evaluación se referencia desde el MEM como la evidencia de
las resoluciones.

**Files created:**
- `2.0/docs/api-removals/US-000.BOLT-009.json` — constancia de rupturas (insumo del Bolt 028)

---

## 7. Acceptance criteria

> Bolt no funcional (parent `US-000`, sin ACs): los criterios son los **resultados medibles** del Bolt (§2) y las
> restricciones de ADR-001 §3.1/§3.4/§3.5, expresados como Given/When/Then verificables.

### AC-1: La librería es portable

**Given** el proyecto de la librería
**When** se construye y se barre su fuente y sus referencias
**Then** compila en su TFM portable con **0** `DllImport`, **0** APIs de Registro, **0** `System.Management`,
**0** `System.Diagnostics.EventLog`, **0** `System.Web` y **0** `System.Windows.Forms`.

### AC-2: Cada capacidad tiene una resolución evidenciada

**Given** el informe de la evaluación
**When** se lee por capacidad
**Then** cada una del inventario declara **una** resolución (eliminada / detrás de una abstracción portable) con la
evidencia que la sostiene, el alcance de la búsqueda de usos y la consecuencia de la eliminación.

### AC-3: El código muerto de correo desaparece

**Given** la librería
**When** se busca el código muerto de correo
**Then** el método que enviaba por CDONTS no existe y `System.Web` ya no aparece en su firma pública.

### AC-4: Los valores ya persistidos se siguen leyendo (almacén real)

**Given** una instalación real de un usuario designado, con valores ya guardados por el producto 1.4
**When** el ciclo de integración lee las claves a través del contrato
**Then** las claves **se encuentran** (misma derivación, incluido el separador `0xA1`), los accesores tipados
devuelven el valor esperado y el camino de ausencia es explícito — y la evidencia registrada es el **resultado**
(clave presente/ausente, tipo leído), nunca el valor.

### AC-5: El contrato de claves es byte-idéntico

**Given** los vectores dorados producidos por el código 1.4 congelado
**When** la derivación de la clave corre sobre las mismas entradas
**Then** el resultado coincide **byte a byte** (incluido el separador de un solo byte), la semántica de recorte y
mayúsculas se preserva **tal cual** y la limitación de sensibilidad a la cultura queda declarada con su
consecuencia.

### AC-6: Sin dependencias nuevas y sin plataforma en la librería

**Given** el presupuesto de dependencias de ADR-001 §3.4
**When** se mide por proyecto
**Then** `Library` sigue con **cero** dependencias de terceros y la abstracción del almacén no introduce ninguna;
el contrato es portable y la implementación sustituta **no** vive en la librería.

### AC-7: Frontera respetada

**Given** el Bolt 010 (sustituto), el Bolt 012 (limpieza de referencias), el Bolt 007 (serialización), el Bolt 011
(configuración), el Bolt 005 (providers) y el Bolt 022/030 (valores protegidos)
**When** se revisa el cambio
**Then** ninguno de sus ámbitos fue invadido: no se implementó el sustituto, no se tocó `[Serializable]`, no se
re-cifró ni migró ningún valor, y la limpieza de la referencia muerta se registra sin pisarla.

### AC-8: Manejo de datos `confidential`

**Given** el contenido del almacén
**When** termina el V-Bounce y el ciclo de integración
**Then** **ningún** valor del almacén aparece en fixtures, reportes, logs, artefactos ni MEMs, y ningún valor de
esa clasificación se envía a un modelo externo (§3.6).

### AC-9: Verificación reproducible y árbol congelado intacto

**Given** el toolchain fijado
**When** se ejecuta `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`
**Then** el árbol compila sin advertencias tratadas como error —**con la directiva de nulabilidad de §B.2 presente
sólo en los archivos aportados del 1.4 que la requieren, y ningún `.csproj` con `<Nullable>disable</Nullable>`**—, la
suite hermética está verde (con los casos de integración filtrados por categoría) y `git diff --quiet <baseline> --
1.4` y `git status --porcelain -- 1.4` están vacíos.

### AC mapping to source (measurable outcome) — non-functional

| Source outcome (Bolt §2) | How this SPEC satisfies it | Verifying test/evidence |
|--------------------------|----------------------------|--------------------------|
| "la librería compila en su TFM portable sin referencias ni APIs de plataforma" | Fase E: barrido de fuentes y referencias del proyecto | AC-1 · `PortabilityTests` |
| "cada capacidad del alcance tiene **una** resolución declarada… con la evidencia que la sostiene" | Fase A: informe de evaluación por capacidad | AC-2 · `library-platform-features-assessment.md` |
| "el código muerto de correo desaparece" | Fase B: eliminación de `SendMailViaCDONTS` | AC-3 · barrido de fuentes |
| "si el almacén de configuración de usuario sobrevive, su contrato de claves y los valores ya persistidos **siguen leyéndose**" | Fases C y D: abstracción portable + derivación byte-idéntica | AC-4, AC-5 · `KeyDerivationGoldenTests` + ciclo de integración |
| "no incorpora dependencias nuevas —si una resolución las exigiera, requiere un ADR" | Fases C/E: contrato portable sin dependencias | AC-6 · `DependencyBudgetTests` |
| "el listado de capacidades eliminadas y de las conservadas detrás de abstracción" | Fases A/B/C + F: informe + constancia | AC-2, AC-3 · informe + fragmento |
| "la entrada propia de las rupturas en las notas de migración" | Fase F: fragmento + MEM | AC-9 · `api-removals/US-000.BOLT-009.json` |
| "datos sensibles en el almacén… no se trasladan a fixtures, reportes ni logs" | §8.2 y §10: manejo `confidential` | AC-8 · `NoSensitiveDataLeakTests` |
| "el código aportado del 1.4 no rompe el build por nulabilidad" (mecánica de build declarada por el V-Bounce N°1, §13.1 de su MEM) | Fase B.2: directiva `#nullable disable` **por archivo aportado** | AC-9 · build de la librería + lectura de los archivos entregados |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Portabilidad (`PortabilityTests`):** 0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`,
  0 `System.Diagnostics.EventLog`, 0 `System.Web`, 0 `System.Windows.Forms` en la fuente y en las referencias del
  proyecto de la librería.
- **Presupuesto de dependencias (`DependencyBudgetTests`):** `Library` con **cero** dependencias de terceros
  (reporte de dependencias por proyecto del Bolt 001).
- **Contrato de claves (`KeyDerivationGoldenTests`):** vectores dorados del 1.4 para un conjunto de entradas
  (mayúsculas y minúsculas mezcladas, espacios al borde, acentos, nombres largos, parámetro vacío) → coincidencia
  **byte a byte**, incluido el separador; se declara la semántica de recorte/mayúsculas y la limitación de cultura.
- **Contrato del almacén (doble):** los accesores tipados (texto, decimal, fecha, booleano) y el camino de
  ausencia se ejercen contra una implementación de prueba del contrato portable (sin tocar el almacén real), para
  verificar la semántica antes de la corrida real.
- **Caracterización:** la suite del Bolt 001 sobre el contenido que permanece, sin diferencias observables.
- **Política de nulabilidad del código aportado (§B.2, revisión 2):** chequeo por **lectura** de los archivos
  entregados —cada archivo aportado del 1.4 que produzca diagnósticos de nulabilidad empieza con `#nullable disable`
  y **ningún** archivo nuevo la lleva—, más el propio `build` con `TreatWarningsAsErrors=true` (la prueba real: sin
  la directiva, el build falla con `CS8618`/`CS8603`/`CS8625`). Se verifica además que **no** hay
  `<Nullable>disable</Nullable>` en ningún `.csproj` y que ningún `using` muerto hacia un namespace inexistente
  quedó sin resolver ni autorizado (§B.2).
- **Build y suite:** `2.0/build/build.ps1 -Scope Hermetic` verde.
- **Edge cases:** nombre de dueño o parámetro con espacios/acentos/mayúsculas; valor booleano y numérico en el
  camino tipado; clave ausente (camino explícito); valor presente con longitud distinta de la esperada; cadena
  vacía como nombre; el separador presente **dentro** de un nombre (no debe alterar la derivación); cultura del
  proceso distinta (la limitación declarada se comprueba, no se "corrige").
- **BUG evidence:** `n/a` — el Bolt no corrige un BUG; no hay TDD rojo→verde obligatorio.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

- **Integration tests (real dependencies):** los cuatro archivos de
  `2.0/tests/Cooperator.Library.UserStore.Integration.Tests/` (fase §6/D + frontmatter), `Category=Integration`,
  ejecutados en el ciclo de integración del Bolt contra el **almacén real** de una instalación de usuario
  designada: lectura de los valores ya persistidos, ida y vuelta (alta/lectura/borrado) en un área de prueba,
  camino de ausencia y verificación de **no fuga** de valores.
- **Contract / E2E against the real providers:** el "proveedor" acá es el **almacén real** del sistema (el hive
  del usuario designado): lo que se verifica end-to-end es que el contrato portable, implementado contra el
  almacén, **encuentra las claves que el producto 1.4 escribió** y devuelve los valores con su tipo.
- **Edge cases:** clave inexistente; valor de tipo distinto al pedido (camino de error tipado); permisos
  restringidos de lectura (camino de error explícito, no silencioso); almacén ausente o vacío en el equipo de la
  corrida (la corrida **falla declarándolo**, no pasa por vacuidad).
- **Manejo de datos:** sólo el **usuario de prueba designado**; ninguna corrida toca el hive de un usuario real de
  trabajo; la evidencia registrada es el **resultado** (clave presente/ausente, tipo leído) y **nunca** el valor;
  `NoSensitiveDataLeakTests` verifica que ningún valor aparezca en fixtures, reportes ni logs.
- **BUG evidence:** `n/a` — no hay defecto en corrección.

---

## 9. Quality gates

> Los estados son el **resultado esperado**; el resultado real se registra en el MEM del V-Bounce (V-Bounce) y en
> `integration_test_runs[]` del manifiesto (ciclo de integración) (§3.6, §3.12).

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | La librería compila en su TFM portable con advertencias tratadas como error; sin diagnósticos de nulabilidad (la directiva de §B.2, sólo en los archivos aportados del 1.4 que la requieren) | `pass` — esperado (§8.1) |
| Unit / component tests | V-Bounce | Suite hermética verde (portabilidad, presupuesto, claves doradas, contrato con doble) | `pass` — esperado (AC-1, AC-5, AC-6) |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato portable del almacén se satisface con una implementación de prueba (semántica y camino de ausencia) | `pass` — esperado (AC-5) |
| SAST | V-Bounce | El cambio **elimina** invocación nativa y acceso al Registro: reduce la superficie de plataforma | `pass` — esperado (AC-1) |
| Dependency scanning / licenses | V-Bounce | **0** dependencias de terceros en `Library` (ADR-001 §3.4) | `pass` — esperado (AC-6) |
| OWASP Top 10 — static review | V-Bounce | No hay superficie externa; el cambio retira P/Invoke y acceso a un almacén del sistema | `n/a` — sin superficie alcanzable desde fuera del proceso |
| Prompt-injection scan | V-Bounce | No hay construcción de prompts | `n/a` — no aplica |
| Secret-leak scan | V-Bounce | 0 secretos en el repositorio: el contenido del almacén **no** se copia a ningún artefacto (y es `confidential`) | `pass` — esperado (AC-8) |
| Hallucination lint | V-Bounce | Tipos, miembros y rutas referenciados existen (o se declaran como aportados) | `pass` — esperado |
| IP / license provenance | V-Bounce | No se incorpora código de terceros | `n/a` — sin código de terceros |
| PII / DLP | V-Bounce | `confidential`: el gate bloquea el envío de esa clase fuera del límite del prompt; ningún valor del almacén entra en fixtures, reportes, logs ni MEMs | `pass` — esperado (AC-8) |
| Dependency-confusion | V-Bounce | Sin paquetes nuevos; la abstracción es BCL puro | `pass` — esperado (AC-6) |
| Test-first evidence | V-Bounce | No es un Bolt de corrección de BUG; los **vectores dorados del 1.4 existen antes** (arnés del Bolt 001) y son la expectativa de partida | `n/a` — sin TDD obligatorio |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas idénticas de la suite y de la caracterización | `pass` — esperado |
| Bolt-manifest validation | V-Bounce | Manifiesto válido contra `manifest-v5-bolt.schema.json` con la entrada `v_bounces[]` completa | `pass` — esperado |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | **Sí** hay retiro de superficie (las capacidades eliminadas): aparece identificado por el gate y **cubierto** por el fragmento de rupturas | `pass` — esperado (AC-2, AC-3) |
| Integration tests (real dependencies) | Integration test | Los valores ya persistidos se leen contra el **almacén real** del usuario designado, con la derivación de clave preservada y sin fuga de valores | `pass` — **en el ciclo de integración (entorno por provisionar)** |
| Contract / E2E against the real providers | Integration test | Contrato portable implementado contra el almacén real: alta, lectura, borrado y camino de ausencia | `pass` — **en el ciclo de integración** |
| DAST / OWASP Top 10 — dynamic | Integration test | No hay servicio desplegable | `n/a` — sin superficie ejecutable de red |
| Perf-smoke (p95/p99) | Integration test | No hay endpoint ni carga crítica afectada | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | No hay servicio instrumentado; el resultado del ciclo es la evidencia de lectura (clave presente/ausente) | `n/a` — sin telemetría de servicio |

> Coherencia con el Bolt (§3.2.2): `test_integration: required` → `integration_test_plan.applicability: applicable`
> con sus tests, su entorno y sus datos. Si la evaluación concluyera la **eliminación** del almacén, el plan pasa a
> `n/a` **mediante una enmienda del Bolt** (READY) y su revisión de SPEC (§15 #6), nunca por decisión del V-Bounce.

---

## 10. Security and data

- **El cambio reduce superficie de plataforma:** desaparecen la invocación nativa (impersonation, working set), el
  acceso al Registro desde una biblioteca portable, el log de eventos y la consulta WMI. Menos P/Invoke y menos
  acceso al sistema es menos superficie de ataque y menos dependencia del host.
- **Datos `confidential`: el punto más delicado.** El almacén del Modeler guarda la **cadena de conexión** del
  cliente (F-16). Por eso: (a) su contenido **no** se copia a fixtures, reportes, logs, artefactos ni MEMs; (b) la
  verificación se hace **contra** el almacén y sólo registra el **resultado** (clave presente/ausente, tipo leído);
  (c) una clase `confidential` **no cruza el límite del prompt** hacia un modelo externo (§3.6), y el gate PII/DLP
  lo bloquea; (d) la corrida de integración usa un **usuario de prueba designado**, nunca el hive de un usuario real
  de trabajo.
- **Sin criptografía nueva ni migración de valores:** este Bolt **no** re-cifra, no descifra y no migra valores; la
  política criptográfica y la migración de lo ya protegido son de ADR-007 y de sus Bolts (022/030). Acá sólo se
  preserva que lo guardado **se lea**.
- **Clasificación:** `data_classification: confidential` (espeja el Bolt). Es la primera vez en el árbol 2.0 que una
  verificación opera sobre datos que **no deben salir** de su entorno; el manejo de §8.2 es parte del criterio de
  aceptación (AC-8).
- **Integridad del árbol congelado:** `1.4/**` no se modifica; su código es la evidencia del hallazgo.

---

## 11. Monitoring and observability

- **El informe de la evaluación es la observabilidad de las decisiones:** por capacidad, quién la usa, con qué
  evidencia y con qué consecuencia se elimina o se abstrae. Es lo que permite auditar la regla "eliminar por
  defecto" sin depender de la memoria de quien la aplicó.
- **La constancia de rupturas** (`2.0/docs/api-removals/US-000.BOLT-009.json`) y los **vectores dorados de claves**
  (`2.0/tests/fixtures/user-store-keys/golden-keys.json`) quedan versionados.
- **El ciclo de integración** deja su evidencia en `integration_test_runs[]` del manifiesto del Bolt: resultado de
  la lectura contra el almacén real (clave presente/ausente, tipo leído), **sin valores**.
- **Sin telemetría de servicio:** la librería no instrumenta nada (`n/a` con esa razón en la tabla de gates).

---

## 12. Migration, compatibility and rollback

- **Migration (del consumidor):** quien usaba una capacidad eliminada tiene su consecuencia declarada —con su
  reemplazo portable cuando existe (por ejemplo, el nombre de usuario del entorno en lugar de la impersonation)— y
  el camino está en las notas de migración (Bolt 028). Quien usaba el **almacén** no migra datos: el contrato de
  claves se preserva byte a byte y los valores ya guardados se siguen leyendo; el **sustituto** del almacén
  (implementación) es del Bolt 010 y su adopción es del consumidor cuando llegue.
- **Compatibility:** **sí** hay ruptura de superficie en la librería (desaparecen las clases y miembros de las
  capacidades eliminadas y el código muerto de correo): el gate de compatibilidad contra la línea base 1.4 la
  identifica y el fragmento de rupturas la cubre con su reemplazo, como ADR-008 §5 exige. Lo que **no** cambia es
  el contrato de datos del almacén ni la semántica de sus accesores.
- **Rollback:** revertir el commit del Bolt. Un rollback **parcial** —conservar una capacidad sin resolver while el
  resto ya portado— es inválido: la librería o es portable o no lo es, y un P/Invoke superviviente la deja fuera del
  TFM objetivo.

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|--------------|------------|
| **Pérdida silenciosa de una capacidad que un consumidor use** | 3 | 4 | La evaluación de uso real con evidencia y su informe por capacidad (AC-2); lo que se elimina queda con su consecuencia declarada y su reemplazo en las notas |
| **Valores ya persistidos que dejan de leerse** (F-48: la clave cambia un byte y el fallo es silencioso) | 2 | 5 | Vectores **dorados** del 1.4 y coincidencia **byte a byte** (AC-5) + verificación **contra el almacén real** (AC-4); el camino de ausencia, explícito |
| **Exposición de datos `confidential`** (el almacén puede contener la cadena de conexión) | 2 | 5 | Manejo declarado (§10, §8.2): nada del contenido entra en fixtures, reportes, logs, MEMs ni prompts; usuario de prueba designado; evidencia sólo del **resultado** (AC-8) |
| **Resolver una capacidad agregando una dependencia** a la librería | 2 | 4 | ADR-001 §3.4: prohibido sin ADR; el contrato es portable y la implementación vive fuera (AC-6) |
| **Pérdida de la identidad de máquina usada para *binding*** | 2 | 3 | Su resolución (eliminada, por falta de uso) es explícita y su consecuencia va a las notas; si apareciera uso, el reemplazo es una abstracción, no WMI en la librería |
| **El sustituto del almacén crece en alcance** y se cuela en este Bolt | 2 | 3 | El Bolt 010 es el corte natural (ya previsto y condicional): acá sólo se declara la abstracción y se detiene si el alcance crece (§15) |
| **La evaluación se apoya en "no encontré llamadores"** sin declarar el alcance de la búsqueda | 2 | 3 | El informe declara el alcance y el límite de la evidencia (árbol del producto; el consumidor aporta lo suyo) (AC-2) |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8603`/`CS8625` | 3 | 3 | §B.2 la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-9 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |

---

## 14. Decisions and trade-offs

| Decision | Reason |
|----------|--------|
| El **almacén sobrevive** detrás de una abstracción portable (y no se elimina) | La evidencia muestra que se usa: el Modeler guarda y lee `CurrentServer`/`CurrentConnectionString`. ADR-001 §3.5 manda conservar detrás de una abstracción a lo que tenga uso real |
| La **implementación** del sustituto **no** entra acá ni en la librería | El Bolt lo excluye (Bolt 010, condicional) y ADR-001 §3.4 lo exige: la librería no puede ganar dependencias. Su ubicación es una decisión visible (§19, nota 2) |
| La clave se deriva **byte a byte** como en 1.4, **incluida su sensibilidad a la cultura** | F-48: el separador es un byte de datos y un cambio de derivación hace ilegibles las claves del cliente **en silencio**. Corregir la cultura cambiaría claves para algunos valores: se preserva y se **declara** la limitación |
| El camino de ausencia se declara explícito en lugar de reproducir el `catch` que cae a "no existe" | La semántica observable se preserva (una clave ausente no es un valor), pero sin que un `catch` amplio oculte **otros** fallos (permisos, almacén inaccesible) |
| Las capacidades sin uso se **eliminan** en lugar de abstraerse "por si acaso" | ADR-001 §3.5: se elimina por defecto **previa verificación del uso real**; abstraer lo que nadie usa agrega superficie y mantenimiento sin consumidor |
| Se conserva el **contrato de correo** que sí se mantiene y se retira sólo el método muerto | El Bolt lo excluye del cambio ("no cambia el contrato de correo que sí se mantiene"); el CDONTS está muerto por construcción (lanza `NotImplementedException`) y arrastra `System.Web` |
| El logging del tooling **no** se decide acá | El Bolt lo excluye: acá se elimina la capacidad de la **librería**; la sustitución del logging del tooling tiene su dueño |
| La evidencia del ciclo de integración es el **resultado**, nunca el valor | El dato es `confidential`: registrar el valor sería la fuga que el manejo busca evitar (§3.6, §10) |
| **El `turn_budget` se fija explícitamente en `25`** (revisión 3), en lugar de correr el N°3 con el default de la plataforma (10) | **Fuente de la decisión: la postura que el revisor registró** en el `AITL-MEM-Approval` del V-Bounce N°2 (`MEM-261016-0227`): la estimación del SPEC («el Bolt estima 2») quedó **demostrablemente excedida** —2 V-Bounces consumidos por presupuesto con B–F pendientes— y el trabajo restante es **una unidad coherente** que 10 turnos no completan de forma fiable: aportar los archivos portables con la directiva de §B.2 **por diagnóstico** (build → diagnosticar → aplicar → rebuild), abstraer el almacén, los vectores dorados, los dos gates, la constancia y la integración. Fijarlo **antes** del N°3 (que es lo que esta revisión hace) evita un tercer cierre por presupuesto que **desperdiciaría el reconocimiento ya hecho** en N°1/N°2 y respeta el «sin estado intermedio»: **un solo V-Bounce que complete B–F**. El parámetro se declara en el frontmatter (`turn_budget: 25`), se documenta en §3 y se refleja en §6/§16; su cambio es por revisión material, como cualquier otro valor del frontmatter |

---

## 15. Stop conditions

> Con `autonomy_level: L2` (riesgo `high`), ante cualquiera de estos casos el V-Bounce **para y reporta** en lugar
> de continuar con una decisión propia.

1. **Cambio material en una fuente gobernada** (Bolt, ADR-001, ADR-007 si se toca la frontera de valores, el REV
   usado, baseline de código): detener, revisar este SPEC canónico y re-aprobar (G15). Una V-Bounce nunca abarca
   dos revisiones (G16).
2. **Aparece evidencia de uso de una capacidad que el inventario daba por eliminable**: detener y resolverla como
   "detrás de una abstracción portable" —y si esa abstracción exigiera una dependencia, detener y escalar
   (ADR-001 §3.4)—. No se elimina lo que se usa, ni se conserva lo que no.
3. **El contrato de claves no puede preservarse byte a byte** (el byte separador, el recorte o las mayúsculas
   cambian): **detener**. F-48 es explícito: el fallo sería silencioso y del lado del cliente.
4. **La resolución de una capacidad exige una dependencia nueva** para la librería: detener y escalar; exige un ADR
   que reemplace a ADR-001 §3.4.
5. **Aparece la necesidad de re-cifrar, descifrar o migrar valores del almacén**: detener y escalar a los Bolts
   022/030 (ADR-007). Acá sólo se verifica que lo guardado **se lea**.
6. **La evaluación concluye que el almacén se elimina**: detener y escalar — el `test_integration: required` del
   Bolt y su plan de integración cambian por **enmienda del Bolt** (READY), no por decisión del V-Bounce.
7. **El sustituto del almacén crece en alcance** (implementación, formato nuevo, migración): detener y derivar al
   Bolt 010, que existe para eso.
8. **Cualquier valor del almacén aparece en un artefacto** (fixture, reporte, log, MEM, prompt): detener
   inmediatamente y reportar; es el manejo `confidential` que el criterio de aceptación protege (AC-8).
9. **Un gate de §9 no cierra en `pass`**: detener y escalar. Una salida por excepción exige un ADR aprobado (G21).
10. **El arnés del Bolt 001 no está disponible** (comando reproducible, suite o vectores dorados): detener y
    escalar la secuencia.
11. **Ambigüedad que sólo se resolvería inventando** (por ejemplo, el nombre definitivo de la abstracción o el
    formato del sustituto): parar y reportar; no se rellena el hueco con una suposición (§2.4.1).
12. **Alguien extiende la autorización de §B.2**: poner la directiva `#nullable disable` en **código nuevo** (o
    `<Nullable>disable</Nullable>` en un `.csproj`), o intentar resolver con esa misma autorización un `using` muerto
    **hacia un namespace que no existe en el árbol** que **no** provenga de una eliminación de esta fase. Parar y
    reportar: la autorización es **una** y está acotada en §B.2.

---

## 16. Definition of Done (DoD)

- [ ] Fases A–F implementadas **en un solo V-Bounce**: el SPEC fija **`turn_budget: 25`** para esa unidad (§14) —o, si
      aun así el presupuesto se agota, progreso en el MEM y pendiente explícito para un V-Bounce nuevo (la estimación
      «el Bolt estima 2» quedó **excedida**: `MEM-261016-0227`)—
- [ ] Todos los criterios de aceptación (AC-1…AC-9) verificados
- [ ] Suite hermética verde (§8.1) y suite de integración escrita, compilada y estáticamente verificada (§8.2)
- [ ] El **informe de la evaluación de uso real** existe, versionado, con la evidencia y la resolución por capacidad
- [ ] Los **vectores dorados** de la derivación de claves coinciden **byte a byte** con el 1.4 (incluido el
      separador) y la limitación de cultura está declarada
- [ ] La librería compila en su TFM portable con **0** APIs de plataforma y **0** dependencias de terceros
- [ ] La directiva de nulabilidad de §B.2 aplicada **sólo** a los archivos aportados del 1.4 que la requieren, y verificada (lectura + build); ningún `.csproj` con `<Nullable>disable</Nullable>`
- [ ] El código sigue ADR-001 (§3.1/§3.4/§3.5)
- [ ] Gates aplicables en `pass` (o `waived`/`n/a` con su razón) según §9
- [ ] MEM creado en `devflow/memory/` (exactamente uno **por V-Bounce**), con la lista de rupturas y **sin ningún
      valor del almacén**
- [ ] Entrada `v_bounces[]` agregada al manifiesto en `devflow/metrics/bolts/` (ocho campos completos por entrada)
- [ ] AITL-MEM-Approval registrado (riesgo `high`: independencia de aprobación **endurecida a nivel de modelo**, §3.0)
- [ ] Plan de integración satisfecho antes de la aceptación: corrida de cobertura pasante sobre el **almacén real**
      del usuario designado, registrada en `integration_test_runs[]` (o el plan `n/a` por **enmienda del Bolt** si
      la evaluación elimina el almacén)
- [ ] `1.4/**` intacto respecto del commit baseline

---

## 17. References

- Bolt: [`US-000.BOLT-009`](../functional/bolts/US-000.BOLT-009-library-portable.md) y su manifiesto
  [`US-000.BOLT-009-…json`](../metrics/bolts/US-000.BOLT-009-library-portable.json)
- Parent: [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor no funcional
- ADR gobernante: [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1, §3.4, §3.5
- ADR de apoyo: [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) §3.1/§3.2 (frontera de valores protegidos: la migración no es de este Bolt)
- SPEC previo: [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) (arnés, comando reproducible y vectores de la generación 1.4)
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** y [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado por revisión material con su medición (`MEM-261016-0107` §8.2: 28 diagnósticos; `MEM-261016-0209` §8.1: `CS8618`). Es el mecanismo que esta revisión **espeja** para el aporte de la fase B
- V-Bounce previo de este Bolt: [`MEM-261016-0215`](../memory/MEM-261016-0215-library-portable.md) — el cierre `blocked` **por presupuesto de turnos** (la salida que §16 prevé) con la **fase A entregada**, las fases B–F pendientes (§13) y el **aviso de mecánica de nulabilidad** (§13.1) que esta revisión atiende
- V-Bounce previo (revisión 3): [`MEM-261016-0227`](../memory/MEM-261016-0227-library-portable.md) — el cierre `blocked` **por presupuesto, por segunda vez**, con el **alcance de B–F medido** (25 archivos/3.125 líneas de la librería; 14 a aportar; 4 del almacén; 4 eliminadas; el `0xA1` re-verificado en el offset 3222) y la **postura del revisor** que funda el `turn_budget: 25` de esta revisión
- SPECs relacionados (fronteras): [`SPEC-261016-0021`](./SPEC-261016-0021-limpieza-de-referencias.md) (limpieza **única** de referencias muertas) ·
  [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) (serialización de las excepciones de `Library`) ·
  [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) (retiro de la última lectura de configuración del host) ·
  [`SPEC-261016-0023`](./SPEC-261016-0023-motor-headless.md) (donde el set de plantillas y el motor tocan `Library`)
- Revisión origen: [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-15…F-20, F-22, F-48 y §6 ítem 7
- Evidencia de código 1.4 (verificada en esta sesión): `Library/IO/Registry.cs` (leer/escribir/borrar/contar;
  `BaseRegistryKey` por defecto `CurrentUser`), `Library/Repository/RegistryRepository.cs:54-258` (accesores
  tipados y `catch (RegistryIOErrorException)`), `Library/Repository/RepositoryMapper.cs:69-103`
  (`GenKey` con el separador y `GetRegistryHelper`), **`RepositoryMapper.cs:92`** (el separador: **un solo byte
  `0xA1`**, único byte no ASCII del archivo, en el offset 3222), `Library/IO/CurrentUserInfo.cs` (`DllImport
  Advapi32`; `GetImpersonatedUser`), `Library/IO/NativeMethods.cs` + `Library/IO/Memory.cs:48`
  (`SetProcessWorkingSetSize`), `Library/IO/EventViewer.cs:48-50` (`System.Diagnostics.EventLog`),
  `Library/Utility.cs:498-507` (`GetDriveSerialNumber` → `System.Management.ManagementObject`),
  `Library/SmtpMail.cs:106` (`SendMailViaCDONTS` con `System.Web.HttpContext` y `NotImplementedException`),
  `Tools/CooperatorModeler/ConnectForm.cs:118,119,127` (uso real del almacén)
- Normativo: `devflow/avenga-devflow/Avenga-DevFlow.md` §2.4.1, §3.2.2, §3.3, §3.6 · `devflow/GUARDRAILS.md`
  (G07, G10, G13, G14, G15, G16, G17, G21, G23, W02, T05, T11)

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (transcripción de `human:eugenioserrano`) | Revision 1 — SPEC inicial del Bolt `US-000.BOLT-009`: evaluación de uso real por capacidad de plataforma con su informe, eliminación de las no usadas y del código muerto de correo, abstracción portable del almacén de configuración de usuario con preservación **byte a byte** del contrato de claves (incluido el separador `0xA1`), portabilidad verificada y verificación contra el almacén real con manejo `confidential` de los datos |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16) PREVENTIVA**, cursada sobre el cierre del V-Bounce N°1 del Bolt (presupuesto de turnos agotado: la salida que §16 prevé; fase A entregada; B–F pendientes) registrado en `MEM-261016-0215`. Cambia **una sola cosa**, y la cambia **antes** de que muerda: la **política de nulabilidad del código aportado** (§B.2, nueva) —`#nullable disable` **por archivo aportado del 1.4** que produzca `CS8618`/`CS8603`/`CS8625`, **jamás** en código nuevo, **prohibido** a nivel de proyecto y **el build como prueba**—, espejo del mecanismo que `SPEC-261016-0004` §A.3 y `SPEC-261016-0013` §A.3 fijaron por revisión material para el **mismo** escenario, y que el propio MEM del V-Bounce N°1 dejó declarado como aviso (§13.1). Se excluye expresamente de la autorización el caso del `using` muerto **hacia un namespace inexistente** que no provenga de una eliminación de la fase B (sigue siendo parada y reporte). Se actualizan §1, §4 (in-scope 9 + ampliación de la regla de aporte), §6/B.2, §7 (AC-9 y su mapeo), §8.1, §9, §13, §15 (#12), §16 y §17. **Nada más cambia:** la fase A ya entregada y su contrato de claves `0xA1` verificado, las fases B–F con sus contenidos, las fronteras y el plan de integración `applicable` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 3 — revisión MATERIAL (G15/G16)**, cursada sobre el cierre del V-Bounce N°2 (**presupuesto de turnos agotado por segunda vez**, la salida que §16 prevé; sin cambios en el árbol) registrado en `MEM-261016-0227` y sobre la **postura que el revisor registró** en su `AITL-MEM-Approval`: subir el presupuesto antes del N°3. Cambia **un solo valor del frontmatter**: **`turn_budget: ""` → `turn_budget: 25`** (entero ≥ 1) para que la unidad B–F —el alcance ya **medido** en el N°2: 14 portables a aportar con la directiva de §B.2 **por diagnóstico**, la abstracción del almacén, los vectores dorados, los dos gates, la constancia y la integración— se complete en **un solo V-Bounce** («sin estado intermedio»), porque la estimación «el Bolt estima 2» quedó **demostrablemente excedida** y 10 turnos no la completan de forma fiable; re-bloquear una tercera vez desperdiciaría el reconocimiento ya hecho. Se documenta en §1, §3 (parámetro del SPEC, no fuente), §6, §14 (la decisión con su fundamento y la cita del revisor), §16, §17 y §19. **Nada más cambia:** la fase A entregada, la política de nulabilidad de §B.2 con sus límites, el plan B–F, las fronteras, el plan de integración `applicable` y el manejo `confidential` siguen **en vigor**. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** This SPEC remains a draft until the
> Dev-validator (+ applicable domain owners) records `AITL-SPEC-Approval`
> (in the `review` frontmatter block). Bolt approval (`AITL-BOLT-READY-Approval`)
> authorizes SPEC preparation; **SPEC approval** authorizes the code-run /
> V-Bounce. A material source change invalidates this approval — stop,
> revise, re-approve (G15).

**Nota al aprobador — qué cambia la revisión 3, y por qué (material, G15/G16).** Es la revisión **más acotada** de
este SPEC: cambia **un solo valor del frontmatter**, sobre una postura que el revisor **ya registró**. Enumerado:

1. **`turn_budget: ""` → `turn_budget: 25`** (un entero ≥ 1 — el campo del template). **Fuente de la decisión:** la
   postura del revisor en el `AITL-MEM-Approval` del V-Bounce N°2 (`MEM-261016-0227`): corresponde **subir el
   presupuesto explícitamente** en lugar de correr el N°3 con el default de la plataforma (10), porque (a) la
   estimación del SPEC («el Bolt estima 2») quedó **demostrablemente excedida** —2 V-Bounces consumidos **por
   presupuesto** con B–F todavía pendientes—, (b) el trabajo restante es **una unidad coherente** que 10 turnos no
   completan de forma fiable —aportar los archivos portables con la directiva de §B.2 **por diagnóstico** (build →
   diagnosticar → aplicar → rebuild), la abstracción del almacén, los vectores dorados, los dos gates, la constancia
   y la integración— y (c) re-bloquear una tercera vez **desperdiciaría el reconocimiento ya hecho** en N°1/N°2,
   mientras que un presupuesto amplio respeta el «sin estado intermedio»: **un solo V-Bounce que complete B–F**. Se
   documenta en §3 (parámetro del SPEC, no una fuente), §6 y §14 (la decisión con su fundamento) y §16 (el DoD).
2. **Lo que NO cambia: todo lo demás.** La **fase A entregada** —el informe de evaluación de uso real, la resolución
   de las seis capacidades y el **contrato de claves `0xA1` verificado byte a byte**—, la **política de nulabilidad de
   §B.2** con sus dos límites duros y el de §15 #12, el **plan B–F ya medido** (`MEM-261016-0227` §13), las fronteras
   (Bolts 010/012/007/011/005/022/030), el plan de integración `applicable` con su entorno por provisionar y el manejo
   `confidential` (AC-8) siguen **en vigor**. El `review` vuelve a vacío porque la revisión es material y se re-aprueba
   por el revisor.

**Nota al aprobador — (histórica) qué cambió la revisión 2 (material, G15/G16).** Es una revisión **preventiva**,
cursada sobre el cierre del V-Bounce N°1 (presupuesto de turnos agotado: la salida que §16 prevé, sin ninguna
condición de §15 activada) y sobre el **aviso de mecánica** que ese cierre dejó declarado (`MEM-261016-0215` §13.1).
Cambia **una sola cosa**:

1. **Política de nulabilidad del código aportado (§B.2, nueva).** `#nullable disable` **por archivo aportado del
   1.4** que produzca diagnósticos de nulabilidad (`CS8618`/`CS8603`/`CS8625`), **jamás** en código nuevo (el
   contrato del almacén y las pruebas conservan la nulabilidad **activa**) y **prohibido**
   `<Nullable>disable</Nullable>` en cualquier `.csproj`. **El criterio es objetivo** —*archivo aportado del 1.4 +
   diagnóstico de nulabilidad*— y **el build es la prueba**. Es el **espejo** de `SPEC-261016-0004` §A.3 y
   `SPEC-261016-0013` §A.3, las dos instancias ya aprobadas de este mismo mecanismo, con sus mediciones
   (`MEM-261016-0107` §8.2 y `MEM-261016-0209` §8.1).
2. **Límite explícito (no autorizado):** un `using` muerto **hacia un namespace que no existe en el árbol** que **no**
   provenga de una eliminación de la fase B sigue siendo **parada y reporte** (§B.2, §15 #12); lo que sí se retira
   con las eliminaciones de la fase es el `using` que queda **sin uso** por ellas (p. ej. `System.Web`).
3. **Lo que NO cambia:** la **fase A ya entregada** —el informe de evaluación de uso real, con la resolución de las
   seis capacidades y el **contrato de claves `0xA1` verificado byte a byte**— **no se reabre**; las fases B–F con
   sus contenidos, las fronteras (Bolts 010/012/007/011/005/022/030), el plan de integración `applicable` con su
   entorno por provisionar, el manejo `confidential` (AC-8) y el `autonomy_level: L2` con su independencia de
   aprobación endurecida siguen **en vigor**. El `review` vuelve a vacío porque la revisión es material y se
   re-aprueba por el revisor.

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de arquitectura (portabilidad y presupuesto de dependencias) + dueño de datos/seguridad (manejo `confidential` del almacén) + dueño de dominio del producto si se evalúa la supervivencia de una capacidad] |
| **review.written_by** | [opcional — quien escribe este registro: el revisor, o el agente que transcribe la decisión; escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [hallazgos o `acknowledged_without_comment` + motivo] |

### Reviewer notes (puntos explícitos para la decisión)

1. **`test_integration: required` depende de que el almacén sobreviva** (el Bolt lo advierte en su READY). La
   evidencia del árbol sostiene que **sí** sobrevive (el Modeler lo usa para `CurrentServer` y
   `CurrentConnectionString`). Si la evaluación en ejecución concluyera lo contrario, el camino es una **enmienda
   del Bolt** (READY) y el plan pasa a `n/a` — nunca una decisión del V-Bounce (§15 #6).
2. **Decisión visible — dónde vive la implementación del sustituto del almacén** (§6/C.2): el contrato portable va
   en `Library`; la implementación es del **Bolt 010**, y su ubicación (proyecto portable nuevo vs. tooling) tiene
   una restricción dura: **`Library` no gana dependencias** (ADR-001 §3.4) y nada platform-specific vive en ella.
   Confirmar el reparto.
3. **El contrato de claves se preserva byte a byte, incluida la sensibilidad a la cultura del `ToLower()` de 1.4**
   (§6/D.1): corregirla cambiaría claves ya guardadas para algunos valores. La limitación queda **declarada**.
   Confirmar que es el tratamiento esperado (y no una "mejora" silenciosa).
4. **Manejo `confidential`:** la verificación se hace **contra** el almacén real de un **usuario de prueba
   designado**, y la evidencia registrada es el **resultado** (clave presente/ausente, tipo leído), **nunca** el
   valor; ningún valor entra en fixtures, reportes, logs, MEMs ni prompts (§3.6). Confirmar que el entorno del
   ciclo de integración dispone de esa instalación, o validar el objetivo.
5. **Capacidades eliminadas por falta de uso** (impersonation, working set, log de eventos, identidad de máquina por
   WMI) y **código muerto de correo**: el informe de la evaluación declara el alcance de la búsqueda y la
   consecuencia de cada eliminación. Si el dueño del producto conoce un consumidor que las use **con evidencia**,
   el camino es conservarlas **detrás de una abstracción portable** (con su Bolt), no reintroducir P/Invoke.
6. **Fronteras:** el sustituto del almacén (Bolt 010), la limpieza única de referencias (Bolt 012), la
   serialización de las excepciones (Bolt 007), la configuración del framework (Bolt 011) y la migración de valores
   protegidos (Bolts 022/030) **no** son de este Bolt.
7. **Política de nulabilidad del código aportado (§B.2, revisión 2).** Confirmar la autorización preventiva de la
   directiva `#nullable disable` **por archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —espejo de
   `SPEC-261016-0004` §A.3 y `SPEC-261016-0013` §A.3, ya aprobadas— frente a las alternativas declaradas (anotar el
   código aportado / diferir los aportes), y el **límite** de la autorización: el `using` muerto hacia un namespace
   inexistente que no provenga de las eliminaciones de la fase B sigue siendo parada y reporte (§15 #12).
