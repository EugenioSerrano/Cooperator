---
id: "MEM-261016-0215"
title: "`Library` portable (SPEC rev.1) — V-Bounce N°1: la evaluación de uso real por capacidad entregada; fases B–F pendientes para el V-Bounce N°2 (presupuesto de turnos agotado, §16)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-009" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0037" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008"] # ADR-001 §3.1/§3.4/§3.5 (la regla de supervivencia y el presupuesto), ADR-008 §3.1 (la constancia de rupturas) — el V-Bounce se detuvo por presupuesto antes de materializar cambios
manifest: "US-000.BOLT-009-library-portable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T02:15:57-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:21:33-03:00"
  decided_at: "2026-10-16T02:21:33-03:00"
  findings:
    - "Compliant — independencia endurecida a nivel de modelo REGISTRADA (riesgo `high`, SPEC §16 «independencia de aprobación endurecida a nivel de modelo»): revisor `deepseek/deepseek-v4-pro` ≠ ejecutor `deepseek/deepseek-flash` — modelo distinto, no sólo agente/rol distinto. Grant `approves: [SPEC, MEM]` verificado."
    - "Compliant — fase A verificada por lectura y muestreo: el informe declara método+alcance+límite («0 llamadores **dentro del árbol**»), las 5 eliminaciones con evidencia re-medida (impersonation 8 ocurrencias intra-archivo, working set cadena sin uso, `EventViewer` 6 intra-archivo, WMI 7 intra-archivo, `SendMailViaCDONTS` muerto por construcción) y la **superviviente** (almacén) con uso verificado — muestreé `1.4/Tools/CooperatorModeler/ConnectForm.cs`: `RegistryRepository.SaveItem`/`GetItemAsString` en L118-119/127-128 bajo la clave `Cooperator.Modeler` (`CurrentServer`/`CurrentConnectionString`)."
    - "Compliant — contrato de claves verificado **byte a byte**: el separador `0xA1` es el único byte no ASCII de `Repository/RepositoryMapper.cs` y está en el **offset 3222** (re-verificado leyendo el archivo como bytes: offset 3222 = 0xA1, flanqueado por `"` 0x22 y en la concatenación `+`); regla `ownerName.Trim().ToLower() + ¡ + parameterName.Trim().ToLower()` con recorte/mayúsculas por parte, camino de ausencia explícito y limitación de cultura declarada (no «corregida», para no alterar claves ya persistidas)."
    - "Compliant — **ninguna** condición de parada de §15 activada (las 11 verificadas negativa una por una en MEM §8.1; en particular la #6 no aplica porque la evaluación concluye que el almacén **NO se elimina** y la #3 no aplica porque el contrato se preserva); el cierre corresponde a la salida del SPEC §16 (DoD: «si el presupuesto se agota, progreso en el MEM y pendiente explícito para un V-Bounce nuevo — el Bolt estima 2»); la elección de no dejar árbol intermedio (sólo el informe) es correcta."
    - "Compliant — confidencialidad (AC-8): **0 valores** del almacén en el informe, el MEM y el manifiesto (sólo nombres de claves, archivos, líneas y estados); la verificación contra el almacén real es del ciclo de integración (§8.2) y no se ejecutó acá."
    - "Compliant — cierre y árbol: T04 (02:14:50) y T05 (02:15:57 `blocked` por presupuesto) con reloj real; MEM único; manifiesto `v_bounces[0]` con 8 campos y `execution_outcome: blocked`; pendiente B–F y avisos de patrón (nulabilidad/`using` muerto) bien listados en MEM §13. Build reproducible **verde** (16 proyectos, 85/85 + 83/83, exit 0) y `1.4/**` intacto; el único añadido del BOLT-009 es el informe (documento que no altera el build). Nota de transparencia: durante la revisión el árbol estuvo transitoriamente en flujo por un V-Bounce concurrente del BOLT-007 (`CS8618` en `Core/BaseObject.cs`), ya revertido — no atribuible al BOLT-009."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0215 — `Library` portable: la evaluación de fase A entregada; B–F pendientes para el V-Bounce N°2

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — `risk_class: high`, `test_integration: required`, `data_classification: confidential` |
| **SPEC**        | [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md), **revisión 1** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T00:39:45-03:00`) |
| **V-Bounce**    | **1** (el Bolt estima **2**) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1/§3.4/§3.5 · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) §3.1 |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se completó la **fase A** —la pieza que hace auditable la regla «eliminar por defecto, previa verificación del uso
real»— y se entregó su artefacto versionado, `2.0/docs/library-platform-features-assessment.md`: la **evaluación de
uso real por capacidad**, con la evidencia re-medida en este V-Bounce (no copiada del inventario del SPEC) y una
resolución declarada por cada una. El resultado es el que el SPEC anticipaba y **por eso el Bolt no se enmienda**:
las cuatro capacidades de plataforma del inventario —impersonation (`IO/CurrentUserInfo.cs`), ajuste del working
set (`IO/NativeMethods.cs` + `IO/Memory.cs`), log de eventos (`IO/EventViewer.cs`) e identidad de máquina por WMI
(`Utility.GetDriveSerialNumber`)— tienen **0 llamadores dentro del árbol** (sólo sus definiciones y su entrada en el
`.csproj`) y el código muerto de correo (`SmtpMail.SendMailViaCDONTS`) es muerto por construcción, así que las
**cinco se eliminan**; en cambio el **almacén de configuración de usuario TIENE USO verificado** (el Modeler que
viaja en el árbol: `1.4/Tools/CooperatorModeler/ConnectForm.cs`, 8 ocurrencias), de modo que **sobrevive detrás de
una abstracción portable** (su implementación concreta es del Bolt 010) y su **contrato de claves se verificó byte
a byte** —el separador `0xA1` es el único byte no ASCII de `Repository/RepositoryMapper.cs` y está en el **offset
3222**— con su camino de ausencia explícito y su limitación de cultura declarada. El V-Bounce se detuvo **por
presupuesto de turnos**, que es una salida prevista por el propio SPEC (§16: «si el presupuesto se agota, progreso
en el MEM y pendiente explícito para un V-Bounce nuevo — el Bolt estima 2»): **ninguna** condición de parada de §15
se activó (en particular **no** la #6, porque la evaluación concluye que el almacén **no** se elimina, y **no** la
#3, porque el contrato de claves se preserva intacto). Las fases B–F quedan enumeradas para el V-Bounce N°2 (§13)
con todo lo que este V-Bounce ya midió para desbloquearlas.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo |
|---|---|---|
| **A — La evaluación de uso real, por capacidad** | **COMPLETADA** ✔ | El informe `2.0/docs/library-platform-features-assessment.md` con: el **método y el alcance de la búsqueda** (y su límite declarado: «0 llamadores **dentro del árbol**»), la evidencia por capacidad (archivo + líneas), la consecuencia de eliminarla y la **resolución** elegida; el **contrato de claves** verificado byte a byte; y el resumen de resoluciones. |
| **B — Las eliminaciones** | pendiente (V-Bounce N°2) | El retiro de las cinco capacidades en la librería 2.0 (aportando los archivos portables en su forma ya resuelta) — la resolución de cada una ya está tomada y evidenciada en el informe de A. |
| **C — La abstracción portable del almacén** | pendiente | Su contrato (lectura/escritura/borrado por dueño y parámetro con el tipo de valor) y su **ubicación declarada** (decisión visible al revisor, §C.2); el sustituto es del Bolt 010. |
| **D — El contrato de claves y los valores persistidos** | pendiente (el contrato **ya está verificado** en A) | Los **vectores dorados** del 1.4 como pruebas, el recorte/mayúsculas por parte, los destinos y el camino explícito de ausencia. |
| **E — La portabilidad verificada** | pendiente | El gate de portabilidad: 0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`, 0 `System.Diagnostics.EventLog`, 0 `System.Web`, 0 `System.Windows.Forms` y 0 dependencias de terceros. |
| **F — Constancia de rupturas y cierre** | pendiente | `2.0/docs/api-removals/US-000.BOLT-009.json` con una entrada por capacidad eliminada y su consecuencia, más las pruebas de integración contra el almacén real (ciclo de integración). |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/docs/library-platform-features-assessment.md` | **El informe de la evaluación de uso real por capacidad** (fase A, §6/A.1): método y alcance de la búsqueda con su límite, evidencia por capacidad (llamadores: archivo y líneas), consecuencia de eliminar cada una, resolución elegida, el contrato de claves verificado byte a byte con su limitación declarada, y el resumen de resoluciones. **No contiene ningún valor del almacén** (AC-8): sólo nombres de claves, archivos y estados. |

Es el **único** artefacto de este V-Bounce: es un documento (no altera el build ni la suite) y es el entregable
completo de su fase.

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-009-library-portable.md` | `status: specified` → `in-development` y fila **T04**; al cerrar, `in-development` → `blocked` con la fila **T05** | Las transiciones de estado del Bolt con su fila de `Status timeline` (G11 y el cierre del contrato del doc). |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Ninguno.

---

## 7. Implementation decisions

1. **La evidencia se re-midió, no se copió.** El inventario del SPEC §2 es el punto de partida (§6/A.1 lo dice), pero
   cada resolución se sostiene con una **búsqueda re-ejecutada** en este V-Bounce sobre `1.4/**` + `2.0/**`
   (excluidas las salidas de build), con el patrón de cada símbolo de la capacidad y contando la **definición** como
   no-uso. Los números del informe son los de esa corrida.
2. **El alcance de la búsqueda se declara con su límite** (§6/A.2): «0 llamadores» se lee como «sin uso **dentro
   del árbol**», y el uso **declarado por el consumidor** es el que decide la supervivencia. Es exactamente lo que
   separa las cinco eliminaciones (sin uso) de la que sobrevive (con uso verificado en el Modeler).
3. **La fase A se entrega completa aunque el V-Bounce se detenga.** El informe es la pieza que hace auditable la
   regla de ADR-001 §3.5 y el SPEC §16 prevé expresamente este cierre («progreso en el MEM y pendiente explícito
   para un V-Bounce nuevo»), así que entregarla —y **no** iniciar las fases B–F que no se pueden terminar— es lo que
   deja el árbol **sin estado intermedio**: el único cambio es un documento.
4. **La confidencialidad se respeta en el artefacto** (AC-8): el informe nombra **claves** (identificadores) y
   **estados** (presente/ausente, tipo leído), **nunca valores**; no hay ningún valor del almacén en el informe, ni
   en este MEM, ni en la entrada del manifiesto. La verificación contra el almacén real es del ciclo de integración
   (§8.2) y no se ejecutó acá.

---

## 8. Deviations and assumptions

### 8.1 Por qué se detuvo: presupuesto de turnos, y por qué **no** es un blocker sustantivo

El cierre `blocked` responde a la **salida prevista** por el SPEC §16 («si el presupuesto se agota, progreso en el
MEM y pendiente explícito para un V-Bounce nuevo — el Bolt estima 2») y al presupuesto de turnos de la ejecución
(regla 10 loops → stop-and-ask). **No** se activó ninguna condición de parada de §15:

- **#1 (cambio material de fuente):** negativa — el spec sigue `approved` rev1 con la misma línea base (§8.5).
- **#2 (aparece evidencia de uso de algo dado por eliminable):** **negativa**, y es el hallazgo central: las cuatro
  capacidades de plataforma y el código muerto de correo tienen 0 llamadores **fuera de su definición**; y la que
  **sí** tiene uso (el almacén) ya estaba resuelta en el SPEC como la que sobrevive ✔.
- **#3 (el contrato de claves no puede preservarse byte a byte):** **negativa** — se preserva: el separador es
  `0xA1` en el offset 3222 de `RepositoryMapper.cs`, el recorte y las mayúsculas se aplican por parte (§3.2 del
  informe) y el camino de ausencia es explícito ✔.
- **#4 (una resolución exige una dependencia nueva):** negativa — la abstracción del almacén es **sin
  dependencias** (ADR-001 §3.4); ninguna de las cinco eliminaciones suma nada ✔.
- **#5 (re-cifrar/descifrar/migrar valores):** negativa — no se tocó ningún valor ni se ejecutó ninguna lectura
  contra el almacén ✔.
- **#6 (la evaluación concluye que el almacén se elimina):** **negativa** — la evaluación concluye lo contrario
  (uso verificado) → el `test_integration: required` del Bolt y su plan siguen en pie; **no** hay enmienda ✔.
- **#7 (el sustituto crece en alcance):** negativa — no se implementó ningún sustituto (es del Bolt 010) ✔.
- **#8 (un valor del almacén aparece en un artefacto):** **negativa** — el informe, este MEM y el manifiesto
  contienen claves y estados, ningún valor ✔.
- **#9 (un gate de §9 no cierra en `pass`):** no evaluable — el V-Bounce se detuvo antes de materializar cambios; el
  control corrido (§9.3) está verde ✔.
- **#10 (arnés ausente):** negativa — el arnés y su línea base están disponibles y verdes ✔.
- **#11 (ambigüedad que sólo se resolvería inventando):** negativa — el nombre definitivo de la abstracción y el
  formato del sustituto son de las fases C/D y del Bolt 010: **no se inventó nada** ✔.

### 8.2 La conclusión de la evaluación y su efecto sobre el Bolt

| Capacidad | Resolución | Evidencia (re-medida) |
|---|---|---|
| Impersonation (`CurrentUserInfo`) | **Eliminada** | 8 ocurrencias del patrón, **todas dentro del archivo**; la única externa es su entrada en el `.csproj` |
| Working set (`NativeMethods` + `Memory`) | **Eliminada** | `NativeMethods` sólo lo usa `Memory`; `Memory` **sin llamadores** |
| `EventViewer` | **Eliminada** | 6 ocurrencias, todas dentro del archivo |
| Identidad de máquina por WMI (`GetDriveSerialNumber`) | **Eliminada** | 7 ocurrencias, todas dentro de `Utility.cs` |
| `SendMailViaCDONTS` | **Eliminado** | 1 ocurrencia = su definición; `NotImplementedException` inmediato |
| **Almacén de configuración de usuario** | **Sobrevive detrás de una abstracción portable** (implementación: Bolt 010) | **uso verificado**: `1.4/Tools/CooperatorModeler/ConnectForm.cs` (8 ocurrencias) |

Y el **contrato de claves** (F-48), verificado byte a byte en este V-Bounce: `ownerName.Trim().ToLower()` +
`\u00A1` + `parameterName.Trim().ToLower()`; el `0xA1` es el **único** byte no ASCII de
`Repository/RepositoryMapper.cs` y está en el **offset 3222**; destinos `CurrentUser`/`LocalMachine`; accesores
tipados; camino de ausencia **explícito**; y la **limitación heredada** del lowercase sensible a la cultura
**declarada** (no se «corrige», porque cambiarla alteraría las claves ya persistidas).

### 8.3 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 1`, `review.decision: approved`
(`2026-10-16T00:39:45-03:00`); el Bolt conserva su `AITL-BOLT-READY-Approval` y entró a este V-Bounce en `specified`
con T01–T03 presentes; ADR-001 y ADR-008 siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.4). No hay cambio material → G15 no se
activa.

---

## 9. Verification evidence

### 9.1 Prerequisitos (§5) verificados antes de generar

```
SPEC-261016-0037 → status: approved · revision: 1 · review.decision: approved (2026-10-16T00:39:45-03:00)
BOLT-009         → status: specified · filas T01..T03 presentes → se tomó con T04 (02:14:50)
2.0/src/Library  → Exceptions/{BaseException,SmtpErrorException}.cs + SmtpMail.cs (aporte del Bolt 011: lazy + validado)
1.4/src/Library  → 26 archivos (~3.300 líneas): Exceptions 8 · IO 8 · Repository 3 · TaxIDVerification 1 · raíz 6
arnés            → 85/85 + 83/83 verde antes de tocar nada · línea base, gate y vectores disponibles
1.4/**           → intacto
```

### 9.2 La búsqueda de llamadores (la evidencia de la fase A)

```
Impersonation (CurrentUserInfo|GetImpersonatedUser|LogonUser|GetUserName)   → 1 archivo de definición + su .csproj
Working set  (NativeMethods|SaveMemory|SetProcessWorkingSetSize)            → Memory.cs (3) + NativeMethods.cs (2) + .csproj
EventViewer  (EventViewer|WriteEntry|EventLog)                              → EventViewer.cs (6) + .csproj
WMI          (GetDriveSerialNumber|System.Management|win32_logicaldisk)     → Utility.cs (7) + .csproj
Correo       (SendMailViaCDONTS)                                            → SmtpMail.cs (1: su definición, NotImplementedException)
Almacén      (RegistryRepository|RepositoryMapper|RepositoryEntity)          → Repository/* (26) + .csproj + Tools/CooperatorModeler/ConnectForm.cs (8)  ← USO
0xA1         RepositoryMapper.cs: único byte no ASCII, offset 3222                        ← contrato de claves preservado
```

### 9.3 Control del árbol (el informe es el único añadido)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 85, Skipped: 0, Total: 85
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

El informe es un documento en `2.0/docs/`: **no** altera el build ni la suite, y las suites del arnés —incluido el
control de rastro del Bolt 006, que escanea la documentación propia— quedan **verdes** (el informe no introduce
ninguno de los marcadores de plataforma que ese control persigue).

### 9.4 `1.4/**` intacto y árbol sin cambios materiales

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
Cambios de este V-Bounce en el árbol 2.0: 2.0/docs/library-platform-features-assessment.md   (un documento)
```

### 9.5 Gates (§9)

No se declaran gates de este Bolt en `pass`: el V-Bounce se detuvo por presupuesto antes de las fases que los
producen. El **control** del árbol (§9.3) está verde y es la línea base sobre la que el V-Bounce N°2 trabajará.

### 9.6 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-009-library-portable.json`: se agregó la entrada **`v_bounces[0]`**
  (`number: 1`, `spec_revision: 1`, `execution_outcome: blocked`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce (MEM, informe, fila T04/T05 del Bolt, manifiesto) se escribieron en **UTF-8 sin
  BOM**, con `LF` y sin `U+FFFD` (§3.15/W23). **Ningún valor del almacén** aparece en ninguno de ellos (AC-8).

---

## 10. Manual interventions

**Ninguna intervención humana.** Las transiciones del Bolt, la búsqueda de llamadores, la verificación del byte
`0xA1` y el informe se hicieron en este V-Bounce. **Tampoco hubo operaciones de git** (G34): el árbol está sin
commitear y el commit es del humano.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el único cambio es el informe de la fase A.
- **SPEC ejecutado:** [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md) — **revisión 1**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T00:39:45-03:00`).
- **Bolt:** [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — `Status timeline`, filas
  **T04** (toma) y **T05** (cierre `blocked`).
- **Manifiesto:** [US-000.BOLT-009-library-portable.json](../metrics/bolts/US-000.BOLT-009-library-portable.json) —
  `v_bounces[0]`.
- **El entregable de este V-Bounce:** `2.0/docs/library-platform-features-assessment.md`.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Library/IO/{CurrentUserInfo,
  NativeMethods,Memory,EventViewer}.cs`, `…/Utility.cs`, `…/SmtpMail.cs`, `…/Repository/{RegistryRepository,
  RepositoryEntity,RepositoryMapper}.cs` y `1.4/Tools/CooperatorModeler/ConnectForm.cs` (el consumidor del almacén).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-009) — `blocked` por presupuesto (§16) |
| SPEC ejecutado | SPEC-261016-0037 revisión **1** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~2 min de reloj (02:14:50 → 02:15:57) |
| Fases completadas | **1 de 6** (A: la evaluación de uso real, con su informe versionado) |
| Capacidades evaluadas | **6** (5 eliminadas con evidencia, 1 que sobrevive con uso verificado) |
| Archivos creados | **1** (`2.0/docs/library-platform-features-assessment.md`) |
| Archivos modificados | **1** (el Bolt: status y filas T04/T05) |
| Valores del almacén en artefactos | **0** (AC-8) |
| Condiciones de §15 activadas | **ninguna** (verificadas una por una, §8.1) |
| Tests corridos como control | **168** (85 del arnés + 83 de caracterización), todos verdes |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |

---

## 13. Pending items and stubs (para el V-Bounce N°2 — el Bolt estima 2)

1. **Fase B — las eliminaciones:** aportar los archivos portables de `Library` en su forma ya resuelta, **sin** las
   cinco capacidades eliminadas (impersonation, working set, `EventViewer`, `GetDriveSerialNumber`, `SendMailViaCDONTS`)
   y con `System.Web` fuera de toda firma pública. **Aviso declarado de mecánica:** si algún archivo aportado del
   1.4 produce diagnósticos de **nulabilidad** (`CS8618`/`CS8603`/`CS8625`) y el SPEC no autoriza la directiva
   `#nullable disable`, **se detendrá y se reportará** (es revisión del SPEC, como en los Bolts 003 y 007), y si
   aparece un **`using` muerto** hacia un namespace inexistente se aplicará el mismo criterio.
2. **Fase C — la abstracción portable del almacén:** su contrato (leer/escribir/borrar por dueño y parámetro, con el
   tipo de valor) y su **ubicación declarada** (§C.2, decisión visible al revisor); el **sustituto es del Bolt 010**.
3. **Fase D — el contrato de claves y los valores persistidos:** los **vectores dorados** del 1.4 como pruebas
   (incluido el recorte/mayúsculas por parte y el `0xA1`), los destinos y el **camino explícito de ausencia**; la
   **limitación de cultura declarada** como tal.
4. **Fase E — la portabilidad verificada:** gate de 0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`, 0
   `System.Diagnostics.EventLog`, 0 `System.Web`, 0 `System.Windows.Forms` y **0 dependencias de terceros**.
5. **Fase F — la constancia y el ciclo de integración:** `2.0/docs/api-removals/US-000.BOLT-009.json` con una
   entrada por capacidad eliminada y su consecuencia, y las **pruebas de integración** contra el **almacén real**
   (escritas y compiladas en el V-Bounce; su corrida es del ciclo de integración) — con la disciplina
   `confidential` intacta.
6. **Frontera (no es de este Bolt):** el retiro de la superficie de serialización de las excepciones de `Library`
   (Bolt 007) y la limpieza de la referencia muerta `System.Windows.Forms` del `.csproj` (Bolt 012): este Bolt
   **registra** el estado que encuentra y no lo toca (AC-7).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión. **Riesgo `high`:** el
SPEC §16 exige independencia de aprobación **endurecida a nivel de modelo** —el evaluador electrónico del entregable
debe ser un modelo de familia distinta a la del ejecutor—, y este MEM lo declara para su registro.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto + el árbol con **un** añadido (el informe de la
  fase A) y el control verde (§9.3).
- **Qué se pide decidir:** la aceptación de la **fase A** y del cierre por presupuesto, con el pendiente explícito
  de las fases B–F para el **V-Bounce N°2** (§13).
