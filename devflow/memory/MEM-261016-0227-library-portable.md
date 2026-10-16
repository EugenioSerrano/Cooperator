---
id: "MEM-261016-0227"
title: "`Library` portable (SPEC rev.2) — V-Bounce N°2: revisión 2 verificada, alcance de B–F medido y cierre por presupuesto de turnos (§16), sin tocar el árbol"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-009" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0037" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008"] # ADR-001 §3.1/§3.4/§3.5 (la regla de supervivencia y el presupuesto), ADR-008 §3.1 (la constancia) — el V-Bounce se detuvo por presupuesto antes de materializar cambios
manifest: "US-000.BOLT-009-library-portable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the tree was NOT touched (§6)
review_ready_at: "2026-10-16T02:27:52-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:33:06-03:00"
  decided_at: "2026-10-16T02:33:06-03:00"
  findings:
    - "Compliant — independencia endurecida a nivel de modelo REGISTRADA (riesgo `high`, SPEC §16): revisor `deepseek/deepseek-v4-pro` ≠ ejecutor `deepseek/deepseek-flash` — modelo distinto, no sólo agente/rol distinto."
    - "Compliant — cierre corresponde al SPEC §16 (presupuesto de turnos agotado) y **ninguna** condición de §15 se activó: verificadas una por una en MEM §8.1 — #12 negativa (no se aplicó ninguna directiva de §B.2 ni se tocó ningún `.csproj`), #3 negativa «por ahora» (el contrato sigue verificado: `0xA1` en su offset), #6 negativa (la evaluación del N°1 concluye que el almacén **NO se elimina** — uso verificado en `ConnectForm.cs`—, el `test_integration: required` sigue en pie)."
    - "Compliant — elección de entregar **nada al árbol** correcta (sin estado intermedio): verificado que `2.0/src/Cooperator.Framework.Library` sigue con sus **3 fuentes** (`SmtpMail.cs`, `BaseException.cs`, `SmtpErrorException.cs`) y que `Storage/IUserSettingsStore.cs` **no existe** — el aporte a medias o la abstracción sin cerrar habría sido exactamente lo que el contrato del V-Bounce prohíbe."
    - "Compliant — re-verificación del contrato y conteos del alcance: `0xA1` re-leído en el **offset 3222** de `Repository/RepositoryMapper.cs` (0xA1 = 161, único byte no ASCII); alcance medido **25 archivos / 3.125 líneas** (re-conteo exacto: 26/3194 incluyendo `Properties/AssemblyInfo.cs`, que el MEM excluye con razón — es metadata de proyecto, no código portable—: 26−1=25, 3194−69=3125); **4 / 921** del almacén exacto (`Registry` 302 + `RegistryRepository` 270 + `RepositoryEntity` 238 + `RepositoryMapper` 111 = 921); **4** eliminadas exacto (`CurrentUserInfo` 85, `EventViewer` 57, `Memory` 53, `NativeMethods` 50)."
    - "Minor gap — el encabezado «16 archivos / 1.447 líneas» de PORTABLE en §9.1 **no reconcilia exacto** con la lista operativa de §13 (que enumera **14 a aportar**, porque `SmtpMail`/`BaseException`/`SmtpErrorException` «ya están en el 2.0») ni con la suma de líneas por archivo (~1654): los conteos por archivo y el total 25/3125 son **exactos**; es una imprecisión contable del encabezado, no un defecto material — no afecta el cierre ni el plan."
    - "Compliant — el plan de B–F (§13) es **ejecutable y suficiente** para el N°3: 9 pasos concretos (aportar los 14 portables en forma resuelta con la directiva de §B.2 por diagnóstico y el `using` sin uso retirado; abstraer el almacén en `IUserSettingsStore.cs`; vectores dorados; gate de portabilidad; constancia + integración) con sus **dos trampas ya declaradas** (la preservación byte a byte del contrato, §15 #3/#5, y el límite de la autorización de §B.2, §15 #12)."
    - "ADJUDICACIÓN (turn_budget del N°3) — POSTURA PARA EL REGISTRO: corresponde una revisión que **suba el turn_budget** (fijar `turn_budget` explícito en el SPEC), no correr el N°3 con el default 10. La estimación del SPEC §16 («el Bolt estima 2») quedó **demostrablemente excedida** (2 consumidos y B–F restantes), y el trabajo restante —aportar 14 archivos con la directiva de §B.2 **por diagnóstico** (build→diagnosticar→aplicar→rebuild), abstraer 4, vectores dorados, 2 gates, constancia e integración— es **una unidad coherente** que 10 turnos no completan de forma fiable; re-bloquear una tercera vez desperdiciaría el reconocimiento ya hecho en N°1/N°2 y un presupuesto amplio respeta el «sin estado intermedio» (un solo V-Bounce que complete B–F). Es decisión del humano/ejecutor; no la ejecuto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0227 — `Library` portable (rev.2): alcance de B–F medido y cierre por presupuesto, sin tocar el árbol

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — `risk_class: high`, `test_integration: required`, `data_classification: confidential` |
| **SPEC**        | [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md), **revisión 2** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T02:23:10-03:00`) |
| **V-Bounce**    | **2** (el N°1 es [`MEM-261016-0215`](MEM-261016-0215-library-portable.md), `blocked`; el SPEC estima **2**) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se verificó la **revisión 2** —la nulabilidad **preventiva** de §6/B.2, con su criterio objetivo (archivo aportado del
1.4 **+** diagnóstico), sus dos límites duros (jamás en código nuevo; prohibido en `.csproj`) y su límite restante
(§15 #12: el `using` muerto hacia un namespace inexistente que **no** venga de las eliminaciones de la fase B sigue
siendo parada)— y se **midió el alcance real** de las fases B–F sobre la fuente congelada: de los **25 archivos /
3.125 líneas** de la librería, **16 archivos / 1.447 líneas** son el contenido portable a aportar (8 excepciones,
`Cryptography`, `TimeFunctions`, `Utility` sin la capacidad WMI, `TaxIDVerification/Argentina`, `Resources`,
`IO/{File,Path,OperatingSystemInfo}` y `SmtpMail`), **4 archivos / 921 líneas** son el almacén que debe quedar
detrás de la **abstracción portable** (`Storage/IUserSettingsStore.cs`, que el SPEC nombra y que **no existe** aún
en el 2.0: es el entregable de §6/C.1) detrás del cual la implementación sustituta es del **Bolt 010**, y **4
archivos** son las capacidades eliminadas (`IO/{CurrentUserInfo,EventViewer,Memory,NativeMethods}.cs`). El contrato
de claves volvió a verificarse **byte a byte**: el separador `0xA1` sigue siendo el único byte no ASCII de
`Repository/RepositoryMapper.cs`, en el **offset 3222**. Con el presupuesto de turnos de esta V-Bounce consumido en
esa verificación y medición, **entregar B–F ahora garantizaría un estado intermedio del árbol** (un aporte a medias
y una abstracción sin cerrar), que es exactamente lo que el contrato del V-Bounce prohíbe: se cierra por la salida
**prevista** del SPEC (§16: «si el presupuesto se agota, progreso en el MEM y pendiente explícito para un V-Bounce
nuevo») **sin tocar el árbol** —el único cambio son las filas del Bolt— y con el **plan exacto** para el siguiente
(§13), incluidos los dos puntos delicados que ya quedan declarados antes de que muerdan.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo |
|---|---|---|
| **A — La evaluación de uso real** | **COMPLETADA en el V-Bounce N°1** ✔ | El informe `2.0/docs/library-platform-features-assessment.md`, con la evidencia re-medida y la resolución por capacidad (5 eliminadas, el almacén conservado). La revisión 2 **no lo reabre** ✔. |
| **B — Las eliminaciones y el aporte portable** | **pendiente** (alcance **medido**, §9.1) | Aportar los 16 archivos portables en su forma resuelta (sin las 4 capacidades eliminadas y sin `GetDriveSerialNumber` en `Utility.cs`), con la directiva de §B.2 aplicada **por diagnóstico** y el retiro del `using` que quede sin uso **por las eliminaciones** (el caso `System.Web` al desaparecer `SendMailViaCDONTS`). |
| **C — La abstracción portable del almacén** | **pendiente** | El contrato `2.0/src/Cooperator.Framework.Library/Storage/IUserSettingsStore.cs` (lectura/escritura/borrado por dueño y parámetro, con el tipo de valor) y la **ubicación declarada** del sustituto (§C.2: la implementación es del **Bolt 010**). |
| **D — El contrato de claves y los valores persistidos** | **pendiente** (el contrato ya está verificado, §9.2) | Los **vectores dorados** como pruebas, con el recorte/mayúsculas **por parte**, el separador `0xA1`, los destinos `CurrentUser`/`LocalMachine`, los accesores tipados y el camino **explícito** de ausencia; la limitación de cultura **declarada** como tal. |
| **E — La portabilidad verificada** | **pendiente** | El gate: 0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`, 0 `System.Diagnostics.EventLog`, 0 `System.Web`, 0 `System.Windows.Forms` y **0 dependencias de terceros** en la librería. |
| **F — Constancia y cierre** | **pendiente** | `2.0/docs/api-removals/US-000.BOLT-009.json` (una entrada por capacidad eliminada con su consecuencia) y las **pruebas de integración** contra el **almacén real** —escritas y compiladas en el V-Bounce; su corrida es del ciclo de integración (entorno **no provisionado**: no bloquea)— con la disciplina `confidential` intacta (0 valores del almacén en artefactos, AC-8). |

---

## 3. Files created

**Ninguno.** Este V-Bounce no materializó ningún artefacto en el árbol: su producto es la **verificación de la
revisión 2** y la **medición del alcance** (§9.1), más el plan de §13. El árbol queda **sin cambios** (§6, §9.3).

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-009-library-portable.md` | `status: blocked` → `in-development` y fila **T06**; al cerrar, `in-development` → `blocked` con la fila **T07** | Las transiciones de estado del Bolt con su fila de `Status timeline` (G11 y el cierre por presupuesto). |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Ninguno: no había nada que revertir, porque no se generó nada en el árbol.

---

## 7. Implementation decisions

1. **Medir primero, y medir en el árbol real.** Antes de tocar nada se recorrió la fuente congelada y se clasificó
   **cada** archivo de la librería por su suerte (portable / almacén / eliminada / capacidad WMI dentro de
   `Utility.cs`), con su número de líneas: es lo que convierte el «alcance» en un dato y lo que permite dimensionar
   el trabajo con evidencia en lugar de con una estimación.
2. **No empezar lo que no se puede terminar.** El contrato del V-Bounce pide un árbol sin estado intermedio: un
   aporte de 1.447 líneas a medias —o una abstracción sin sus vectores dorados— dejaría el árbol peor que como
   estaba. Se prefirió el cierre por presupuesto con el plan exacto (§13) a un intento que hubiera obligado a
   revertir.
3. **Los dos puntos delicados quedan declarados antes de empezar** (§8.2), para que el próximo V-Bounce los trate
   como decisiones y no como sorpresas.

---

## 8. Deviations and assumptions

### 8.1 Por qué se detuvo (y por qué no es un blocker sustantivo)

El cierre responde a la salida **prevista por el propio SPEC** (§16: «si el presupuesto se agota, progreso en el MEM
y pendiente explícito para un V-Bounce nuevo») y al presupuesto de turnos de la ejecución. **Ninguna** condición de
parada de §15 se activó en el reconocimiento:

- **#1 (cambio material):** negativa (§8.5).
- **#2 (uso de algo dado por eliminable):** negativa —la evaluación del N°1 ya lo resolvió, con el almacén
  conservado y las cinco capacidades sin llamadores ✔ (§2/A).
- **#3 (el contrato de claves no puede preservarse byte a byte):** **negativa por ahora** y es el **primer punto
  delicado** del próximo V-Bounce (§8.2): el contrato sigue verificado (el `0xA1` en el offset 3222 de
  `RepositoryMapper.cs`) y **la abstracción deberá preservarlo exactamente**; si no pudiera, la condición se dispara
  y se detiene.
- **#4 (dependencia nueva):** negativa —la abstracción es **sin dependencias** (ADR-001 §3.4).
- **#5 (re-cifrar/descifrar/migrar valores):** negativa —no se tocó ningún valor.
- **#6 (la evaluación concluye que el almacén se elimina):** negativa (uso verificado; el `test_integration:
  required` sigue en pie).
- **#7 (el sustituto crece en alcance):** negativa —no se implementa sustituto alguno (es del Bolt 010).
- **#8 (un valor del almacén en un artefacto):** **negativa** —cero valores; este MEM y el manifiesto sólo nombran
  claves, archivos y recuentos (AC-8).
- **#9 (un gate de §9 no cierra):** no evaluable —no se materializaron cambios.
- **#10 (arnés ausente):** negativa —el comando reproducible está disponible y la última medición del árbol es verde
  (§9.3).
- **#11 (ambigüedad que sólo se resolvería inventando):** negativa —el contrato de la abstracción está **nombrado**
  por el SPEC (`Storage/IUserSettingsStore.cs`) y no se inventó nada.
- **#12 (extensión de la autorización de §B.2):** **negativa** —no se aplicó ninguna directiva (no hubo aporte), no
  se tocó ningún `.csproj` y no se resolvió ningún `using` muerto con esa autorización.

### 8.2 Los dos puntos delicados, declarados para el próximo V-Bounce

1. **La preservación byte a byte del contrato de claves** (F-48) al construir la abstracción: el recorte y el
   `ToLower()` se aplican **por parte** (`ownerName.Trim().ToLower()` + `0xA1` + `parameterName.Trim().ToLower()`), el
   separador es `0xA1` y el camino de ausencia es explícito. Si la abstracción no pudiera preservarlo —o si hubiera
   que **migrar** algo—, rige **§15 #3/#5**: detener y escalar. Los **vectores dorados** deben derivarse del
   comportamiento del 1.4 (la suite de caracterización del Bolt 001 referencia el árbol congelado, así que puede
   producir los bytes esperados) y quedar como expectativas del contrato nuevo.
2. **El límite de §15 #12**: con la autorización de §B.2 se retira el `using` que queda **sin uso por las
   eliminaciones** (el caso `System.Web` al desaparecer `SendMailViaCDONTS`) y la directiva se pone **sólo** en
   archivos aportados **con diagnóstico**; un `using` muerto **hacia un namespace que no existe** que no provenga de
   esas eliminaciones **no** se resuelve con esa autorización: **parar y reportar**.

### 8.3 Lo que la revisión 2 confirma (y lo que no habilita)

Confirmado por el revisor: el mecanismo de §B.2 es el espejo exacto de `SPEC-261016-0004 §A.3` y
`SPEC-261016-0013 §A.3` rev.2 (medidos en disco), con el criterio objetivo y el build como prueba; y **no habilita
nada más**: ni código nuevo con la directiva, ni `<Nullable>disable</Nullable>` en un `.csproj`, ni resolver el
`using` muerto de §15 #12. La **fase A ya entregada no se reabre** y las fronteras (010/012/007/011/005/022/030)
siguen en vigor.

### 8.4 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 2`, `review.decision: approved`
(`2026-10-16T02:23:10-03:00`); el Bolt entró en `blocked` con T01–T05 presentes y se tomó con **T06**; ADR-001 y
ADR-008 siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.4). No hay cambio material → G15 no se activa.

---

## 9. Verification evidence

### 9.1 El alcance de B–F, medido sobre la fuente congelada

```
Librería 1.4 (25 archivos / 3.125 líneas), clasificada por su suerte:

PORTABLE — a aportar (16 archivos / 1.447 líneas)
  Utility.cs 512* · SmtpMail.cs 144 · IO/Path.cs 143 · Cryptography.cs 119 · TimeFunctions.cs 99
  Resources.cs 86 · IO/File.cs 83 · IO/OperatingSystemInfo.cs 71 · TaxIDVerification/Argentina.cs 47
  Exceptions/{ApplicationError,CryptoError,FileAlreadyExist,ItemExistsButDiferentType,NullValueNotAllowed,
              RegistryIOError} 6×~82 · Exceptions/{BaseException,SmtpErrorException} (ya en el 2.0)
  (*) Utility.cs pierde la capacidad de identidad de máquina por WMI (fase B)

ALMACÉN — a abstraer (4 archivos / 921 líneas)
  IO/Registry.cs 302 · Repository/RegistryRepository.cs 270 · Repository/RepositoryEntity.cs 238
  Repository/RepositoryMapper.cs 111      →  contrato: Storage/IUserSettingsStore.cs (NO existe aún: §6/C.1)

ELIMINADAS (fase B, 4 archivos)
  IO/CurrentUserInfo.cs 85 · IO/EventViewer.cs 57 · IO/Memory.cs 53 · IO/NativeMethods.cs 50
```

### 9.2 El contrato de claves, re-verificado

```
Repository/RepositoryMapper.cs:  único byte no ASCII = 0xA1, offset 3222
Derivación: ownerName.Trim().ToLower() + 0xA1 + parameterName.Trim().ToLower()   (recorte y mayúsculas POR PARTE)
Camino de ausencia explícito: RegistryRepository.cs:64 · RepositoryMapper.cs:69-73
Storage/IUserSettingsStore.cs en el 2.0: NO existe (entregable de §6/C.1)
```

### 9.3 El árbol como se deja

```
Cambios en el árbol 2.0 por este V-Bounce: ninguno (los dos cambios son filas de estado del Bolt, en devflow/)
Última medición del árbol: la del cierre del Bolt 012 — 16 proyectos sin advertencias, arnés 90/90,
caracterización 83/83, EXIT=0 (173 casos herméticos) — y este V-Bounce no la altera, porque no tocó el árbol.
```

### 9.4 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.5 Gates (§9)

No se declaran gates en `pass`: el V-Bounce se detuvo por presupuesto **antes** de materializar cambios. No hay
efecto sobre el árbol (§9.3) ni sobre el plan de integración (`applicable`, con su entorno por provisionar).

### 9.6 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-009-library-portable.json`: se agregó la entrada **`v_bounces[1]`**
  (`number: 2`, `spec_revision: 2`, `execution_outcome: blocked`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Este MEM y la fila T06/T07 del Bolt se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23), y
  **sin ningún valor del almacén** (AC-8).

---

## 10. Manual interventions

**Ninguna intervención humana.** El reconocimiento, la clasificación de la fuente congelada, la re-verificación del
byte `0xA1` y el cierre fueron actos del V-Bounce. **Tampoco hubo operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`; el árbol **no se tocó** (§9.3).
- **SPEC ejecutado:** [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md) — **revisión 2**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T02:23:10-03:00`), con **§6/B.2** (nulabilidad preventiva) y su
  límite en **§15 #12**.
- **Bolt:** [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — `Status timeline`, filas
  **T06** (toma) y **T07** (cierre `blocked`).
- **V-Bounce N°1:** [`MEM-261016-0215`](MEM-261016-0215-library-portable.md) — la evaluación de uso real, el
  informe entregado y el pendiente explícito de B–F.
- **Manifiesto:** [US-000.BOLT-009-library-portable.json](../metrics/bolts/US-000.BOLT-009-library-portable.json) —
  `v_bounces[1]`.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Library/**` (las 25 fuentes
  clasificadas en §9.1), con `Repository/RepositoryMapper.cs` (la derivación de clave y el byte `0xA1`) y
  `IO/Registry.cs` + `Repository/*` (el almacén a abstraer).
- **Precedentes del mecanismo de §B.2:** SPEC-0004 §A.3 y SPEC-0013 §A.3 rev.2 (mecanismo idéntico, ya endosado dos
  veces).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 2 (de US-000.BOLT-009) — `blocked` por presupuesto (§16) |
| SPEC ejecutado | SPEC-261016-0037 revisión **2** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~2 min de reloj (02:27:52 → cierre) |
| Fases completadas | **1 de 6** (la A, en el V-Bounce N°1) · B–F **medidas y planificadas** (§13) |
| Alcance medido de B–F | **16 archivos / 1.447 líneas** a aportar · **4 / 921** a abstraer · **4** eliminadas · el contrato de la abstracción + 2 gates + la constancia + la integración |
| Archivos entregados al árbol | **0** (sin estado intermedio) |
| Valores del almacén en artefactos | **0** (AC-8) |
| Condiciones de §15 activadas | **ninguna** (verificadas una por una, §8.1, incluida la #12) |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |

---

## 13. Pending items and stubs (el plan exacto para el V-Bounce N°3)

**Fase B — el aporte portable (16 archivos / 1.447 líneas).**
1. Aportar, **en su forma resuelta**, los portables de §9.1: `Cryptography.cs`, `TimeFunctions.cs`, `Utility.cs`
   (**sin** `GetDriveSerialNumber` ni el uso de `System.Management`), `TaxIDVerification/Argentina.cs`,
   `Resources.cs`, `IO/{File,Path,OperatingSystemInfo}.cs` y las 6 excepciones que faltan
   (`ApplicationErrorException`, `CryptoErrorException`, `FileAlreadyExist`, `ItemExistsButDiferentTypeException`,
   `NullValueNotAllowed`, `RegistryIOErrorException`). `SmtpMail.cs`, `BaseException` y `SmtpErrorException` **ya
   están** en el 2.0 (Bolt 011): no se re-aportan.
2. **No** aportar las 4 eliminadas (`IO/{CurrentUserInfo,EventViewer,Memory,NativeMethods}.cs`).
3. Aplicar la directiva de **§B.2 por diagnóstico** (el build reporta; sólo los archivos aportados que produzcan
   `CS8618`/`CS8603`/`CS8625`), **jamás** en código nuevo y **jamás** en un `.csproj`.
4. Retirar el `using` que quede **sin uso por las eliminaciones** (caso `System.Web` al desaparecer
   `SendMailViaCDONTS`); **no** tocar ningún `using` muerto hacia un namespace inexistente (§15 #12: parar y
   reportar).
5. **Frontera**: no tocar la superficie de serialización de las excepciones (Bolt 007) ni la referencia muerta
   `System.Windows.Forms` del `.csproj` y su `using` (Bolt 012 lo registró; su control de higiene ya vigila el
   árbol).

**Fase C — la abstracción del almacén.**
6. El contrato `2.0/src/Cooperator.Framework.Library/Storage/IUserSettingsStore.cs` (leer/escribir/borrar por dueño y
   parámetro, con el tipo de valor) **sin dependencias**, y la **ubicación declarada** del sustituto (§C.2: la
   implementación es del **Bolt 010**; este Bolt no la escribe).

**Fase D — el contrato de claves.**
7. Los **vectores dorados**: derivar del 1.4 (vía la suite de caracterización del Bolt 001, que referencia el árbol
   congelado) la clave exacta de varios pares (dueño, parámetro) —incluidos los bordes de recorte, mayúsculas y
   **acentos**— y dejarlos como expectativas del contrato nuevo; declarar la **limitación de cultura** como tal.

**Fase E — el gate de portabilidad.**
8. 0 `DllImport`, 0 `Microsoft.Win32`, 0 `System.Management`, 0 `System.Diagnostics.EventLog`, 0 `System.Web`,
   0 `System.Windows.Forms` y **0 dependencias de terceros** en la librería.

**Fase F — constancia y cierre.**
9. `2.0/docs/api-removals/US-000.BOLT-009.json` (una entrada por capacidad eliminada con su consecuencia) y las
   **pruebas de integración** contra el almacén real (escritas y compiladas; corrida en el ciclo de integración,
   entorno por provisionar), con la disciplina `confidential` intacta.

**Trampas ya declaradas** (§8.2): la preservación **byte a byte** del contrato de claves (§15 #3/#5) y el límite de
la autorización de §B.2 (§15 #12).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión. **Riesgo `high`:** el
SPEC §16 exige independencia de aprobación **endurecida a nivel de modelo**, y este MEM lo declara para su registro.

- **Artefacto presentado:** este MEM + `v_bounces[1]` del manifiesto + el árbol **sin cambios** (§9.3).
- **Qué se pide decidir:** la aceptación del cierre por presupuesto y el arranque del **V-Bounce N°3** con el plan de
  §13, que ya deja declaradas sus dos trampas.
