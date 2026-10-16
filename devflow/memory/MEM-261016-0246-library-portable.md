---
id: "MEM-261016-0246"
title: "`Library` portable (SPEC rev.3) — V-Bounce N°3: fases B–F completas y verdes (aporte sin las cinco capacidades, abstracción del almacén, vectores dorados, gate de portabilidad y constancia)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-009" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0037" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 3 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 3 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008", "ADR-004"] # ADR-001 §3.1/§3.4/§3.5 (la regla de supervivencia y el presupuesto), ADR-008 §3.1/§3.2 (la constancia y las rupturas), ADR-004 §3.2 (el patrón de fijación única al arranque)
manifest: "US-000.BOLT-009-library-portable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T02:46:52-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:51:28-03:00"
  decided_at: "2026-10-16T02:51:28-03:00"
  findings:
    - "Compliant — independencia endurecida a nivel de modelo REGISTRADA (riesgo `high`, SPEC §16): revisor `deepseek/deepseek-v4-pro` ≠ ejecutor `deepseek/deepseek-flash` — modelo distinto, no sólo agente/rol distinto."
    - "Compliant — entregables verificados por muestreo: `Utility.cs` **493 líneas, 0 rastros WMI** (`GetDriveSerialNumber`/`win32_logicaldisk`/`System.Management` = 0) y **1 directiva de §B.2** (la única); `Storage/` con los 3 archivos (`IUserSettingsStore.cs` con el camino de ausencia explícito —`null`, nunca un default silencioso—, `UserSettingsKey.cs` con `Separator = '\u00A1'` y `UserSettingsStore.cs`); `api-removals/US-000.BOLT-009.json` con **5 entradas** (BOLT009-01..05); el guard `IsGenericTypeDefinition` presente en `LibraryPortabilityTests.cs`; los 2 casos de integración con `[Trait("Category", "Integration")]`."
    - "Compliant — los dos incidentes declarados son correctos: (1) el `StackOverflow` (`0xC00000FD`) por `GetGenericTypeDefinition` recursando sobre `Nullable<T>`, corregido con el guard `if (type.IsGenericType && !type.IsGenericTypeDefinition)` —verificado en el archivo—; (2) los casos de integración fallan sin filtro **por diseño** (exigen el almacén fijado del Bolt 010) y el hermético los excluye por `Category=Integration` —declarado para no leerse como defecto—."
    - "Compliant — las dos trampas verificadas: el contrato de claves **intacto byte a byte** (`U+00A1` como único byte no ASCII de `RepositoryMapper.cs`, recorte/plegado por parte, camino de ausencia explícito; §15 #3 no se disparó); y §15 **#12 no se disparó** (0 `using` muerto introducido —el `System.Management` estaba cualificado—; la única directiva es `Utility.cs`, archivo aportado con diagnóstico, exactamente lo autorizado)."
    - "Compliant — AC-8 (0 valores del almacén: los casos usan un valor sintético propio `Cooperator.Bolt009.Integration`) + DoD + MEM completo + manifiesto válido."
    - "Compliant (nota de transparencia) — la corrida actual **no reproduce 100/100**: da **7 fallas**, todas de **trabajo concurrente del BOLT-007** (`HostConfigurationAbsenceTests` del 011 por las excepciones de `Data` aportadas, `ApiRemovalCoverageTests` del 006 por las `CP0002` sin constancia, y `BinaryTrailTests`/`SerializationSurfaceTests` del propio 007 en convergencia), **no** de los tests del BOLT-009. El `100/100` del MEM fue exacto a la hora de su V-Bounce (02:46:52), antes de que aterrizaran los aportes concurrentes del 007. No es un defecto del BOLT-009."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0246 — `Library` portable: fases B–F completas en una pasada y verdes

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — `risk_class: high`, `test_integration: required`, `data_classification: confidential` |
| **SPEC**        | [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md), **revisión 3** (`turn_budget: 25`, aprobada `2026-10-16T02:36:30-03:00`) |
| **V-Bounce**    | **3** — el N°1 es [`MEM-261016-0215`](MEM-261016-0215-library-portable.md) (la evaluación de uso real) y el N°2 [`MEM-261016-0227`](MEM-261016-0227-library-portable.md) (el plan medido de B–F) |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.6) |

---

## 1. Executive summary

Las fases **B–F** se completaron **en una sola pasada**, como el `turn_budget: 25` de la revisión 3 lo previó, y el
árbol quedó **verde**: **16 proyectos sin advertencias**, arnés **100/100** (los 90 previos más **10** casos nuevos)
y caracterización **83/83** —**183 casos herméticos**— con `1.4/**` intacto. Se aportaron los **14 archivos
portables** de la librería (8 de contenido + 6 excepciones) sin las **cinco capacidades eliminadas**
—impersonation, ajuste del working set, log de eventos, identidad de máquina por WMI y el código muerto de correo,
que en el 2.0 ya estaba ausente por el Bolt 011— y con la directiva de **§B.2 aplicada por su criterio objetivo**:
el build reportó los archivos con diagnósticos y la llevó **uno solo** (`Utility.cs`, donde se retiró la capacidad
WMI de 512 → 493 líneas **sin dejar rastro**: ni el método, ni la región, ni la directiva huérfana que el primer
corte dejó). El **almacén de configuración de usuario** —la capacidad que la evaluación del N°1 halló **en uso**—
quedó detrás de una **abstracción portable sin dependencias**: el contrato `Storage/IUserSettingsStore.cs`, la
**derivación de clave byte a byte** del 1.4 (`Storage/UserSettingsKey.cs`, con el separador `U+00A1` preservado) y
el punto de fijación del host (`Storage/UserSettingsStore.cs`), con la **implementación sustituta declarada como
del Bolt 010**. Los **vectores dorados** se verifican contra el contrato del 1.4 —incluida la **evidencia del byte
`0xA1`** como único byte no ASCII de `RepositoryMapper.cs`— y la **limitación de cultura** del plegado se mide
**como hecho**, no se declara corregida; el **gate de portabilidad** exige 0 invocación nativa, 0 plataforma, 0
dependencias de terceros y una superficie de contrato sólo con tipos de la BCL; y la **constancia**
(`api-removals/US-000.BOLT-009.json`) declara las cinco capacidades retiradas con su consecuencia y la capacidad
que sobrevive, de modo que el cruce de cobertura del arnés queda **satisfecho**. Las **pruebas de integración**
contra el almacén real quedaron **escritas y compiladas** (marcadas `Category=Integration`, excluidas de la corrida
hermética; su corrida es del ciclo de integración, con el entorno por provisionar).

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — La evaluación de uso real** | completada en el N°1 ✔ | El informe `2.0/docs/library-platform-features-assessment.md` (no se reabre por esta revisión). |
| **B — El aporte portable** | **completada** ✔ | **14 archivos aportados** (Cryptography, TimeFunctions, Utility, Resources, TaxIDVerification/Argentina, IO/{File,Path,OperatingSystemInfo} y las **6 excepciones** que faltaban); `Utility.cs` **sin** la capacidad WMI (512 → **493** líneas); las **4 capacidades eliminadas no existen** y su rastro da **0**; **1 directiva** de §B.2 (`Utility.cs`, la única que produjo diagnósticos). La librería queda con **20 archivos**. |
| **C — La abstracción del almacén** | **completada** ✔ | `Storage/IUserSettingsStore.cs` (el contrato: leer/escribir/borrar por dueño y parámetro con el tipo del valor, el destino y el **camino de ausencia explícito**), `Storage/UserSettingsKey.cs` (la **derivación de clave byte a byte**) y `Storage/UserSettingsStore.cs` (el **punto de fijación** del host, con error explícito si no hay almacén fijado). **Sin dependencias** y con la implementación sustituta declarada **del Bolt 010**. |
| **D — El contrato de claves** | **completada** ✔ | Los **vectores dorados** (recorte y plegado **por parte**, el separador `U+00A1`, su posición y los pares que la concatenación habría confundido), la **evidencia** de la fuente congelada (el `0xA1` es el **único** byte no ASCII de `RepositoryMapper.cs`) y la **limitación de cultura** medida como hecho. |
| **E — La portabilidad verificada** | **completada** ✔ | El gate: **0** invocación nativa, **0** `System.Management`, **0** `Microsoft.Win32`, **0** log de eventos, **0** `System.Web`, **0** `System.Windows.Forms`; **0** `PackageReference` y **0** `<Reference>`; el ensamblado referencia sólo BCL y el propio framework; y la **superficie del contrato** usa sólo tipos de la BCL. |
| **F — Constancia y cierre** | **completada** ✔ | `2.0/docs/api-removals/US-000.BOLT-009.json` (5 entradas por capacidad retirada con su consecuencia + la capacidad que sobrevive con su contrato, su clave y el dueño de la implementación) y las **pruebas de integración** contra el almacén real, escritas y compiladas (§3.5). |

---

## 3. Files created

| Path | Líneas | Propósito |
|---|---:|---|
| `2.0/src/Cooperator.Framework.Library/Cryptography.cs` | 119 | La criptografía portable de la librería (aporte verbatim). |
| `2.0/src/Cooperator.Framework.Library/TimeFunctions.cs` | 99 | Las funciones de tiempo (aporte verbatim). |
| `2.0/src/Cooperator.Framework.Library/Utility.cs` | **493** | Las utilidades portables, **sin** la identidad de máquina por WMI (`GetDriveSerialNumber` y su región retirados; 512 → 493) y con la directiva de §B.2. |
| `2.0/src/Cooperator.Framework.Library/Resources.cs` | 86 | El acceso a recursos (aporte verbatim). |
| `2.0/src/Cooperator.Framework.Library/TaxIDVerification/Argentina.cs` | 47 | La verificación de CUIT/CUIL (aporte verbatim). |
| `2.0/src/Cooperator.Framework.Library/IO/File.cs` · `IO/Path.cs` · `IO/OperatingSystemInfo.cs` | 83 · 143 · 71 | Las utilidades de E/S portables (aporte verbatim). |
| `2.0/src/Cooperator.Framework.Library/Exceptions/{ApplicationError,CryptoError,FileAlreadyExist,ItemExistsButDiferentType,NullValueNotAllowed,RegistryIOError}*.cs` | 6 × ~82 | Las seis excepciones que faltaban (aporte verbatim; su superficie de serialización es del **Bolt 007**, SPEC-0037 §4). |
| `2.0/src/Cooperator.Framework.Library/Storage/IUserSettingsStore.cs` | 78 | **El contrato portable del almacén** (ADR-001 §3.5): leer/escribir/borrar por dueño y parámetro con el tipo del valor, el destino y el camino de ausencia explícito. |
| `2.0/src/Cooperator.Framework.Library/Storage/UserSettingsKey.cs` | 45 | **La derivación de la clave**, byte a byte la del 1.4 (`U+00A1` incluido), con la limitación de cultura declarada. |
| `2.0/src/Cooperator.Framework.Library/Storage/UserSettingsStore.cs` | 70 | **El punto de fijación** del almacén (mismo patrón de arranque que ADR-004 §3.2), con error explícito si no hay almacén fijado. |
| `2.0/tests/Cooperator.Harness.Tests/LibraryPortabilityTests.cs` | 4 casos | El gate de portabilidad (fase E). |
| `2.0/tests/Cooperator.Harness.Tests/UserSettingsStoreContractTests.cs` | 6 casos | Los vectores dorados, la evidencia del `0xA1`, el camino de ausencia, el round-trip por tipo, el punto de fijación y la limitación de cultura (fases C/D). |
| `2.0/tests/Cooperator.Harness.Tests/UserSettingsStoreIntegrationTests.cs` | 2 casos | Las pruebas de integración contra el almacén **real** (`Category=Integration`, fase F/§8.2). |
| `2.0/docs/api-removals/US-000.BOLT-009.json` | — | **La constancia**: las cinco capacidades retiradas con su consecuencia y la capacidad que sobrevive (fase F). |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-009-library-portable.md` | `status: blocked` → `in-development` y fila **T08** | La toma del V-Bounce por G11 con su fila de `Status timeline`. |

**Ningún archivo preexistente del árbol 2.0 se modificó**: los tres archivos de `Library` que ya existían
(`SmtpMail.cs`, `BaseException.cs`, `SmtpErrorException.cs`, del Bolt 011) quedan intactos.

---

## 5. Files renamed / 6. Files deleted

Ninguno (el retiro de las cinco capacidades se materializa **no aportándolas**: el árbol 2.0 nunca las tuvo, y la
evidencia de su ausencia es el gate de portabilidad y el inventario del N°1).

---

## 7. Implementation decisions

1. **El aporte se midió antes de hacerlo y se verificó después.** Cada escritura del generador **verifica** que el
   archivo quedó como se pretendía (la lección del V-Bounce N°3 del Bolt 007: `String.Replace()` sin coincidencia es
   **silencioso**), y el inventario de cierre se comparó contra el alcance del plan del N°2.
2. **La capacidad WMI se retiró completa, y su primer corte dejó una huella que se corrigió.** El corte del método
   dejó el `#endregion` de su región **huérfano** (el compilador lo rechazó: `CS1028`) y, corregido eso, quedó el
   `#region` sin cerrar: los dos se retiraron con la capacidad, de modo que `Utility.cs` no conserva **ningún**
   rastro de lo eliminado (ni el método, ni el nombre, ni los marcadores de región).
3. **El contrato del almacén no implementa nada y lo dice.** El framework declara el contrato, la derivación de
   clave y el punto de fijación; la **implementación** es del Bolt 010, y consumir el almacén sin fijarlo lanza un
   error que lo nombra (nunca un almacén "por defecto").
4. **La clave es dato, no implementación** (REV-001 F-48): vive en `UserSettingsKey` y cualquier implementación la
   usa tal cual; la limitación de plegado sensible a la cultura se **conserva y se mide**, porque cambiarla
   alteraría las claves ya persistidas.
5. **Las pruebas de integración se escriben contra el contrato y el almacén real, con datos sintéticos propios**:
   crean y borran su propio valor (`Cooperator.Bolt009.Integration`), así que **no** leen ni escriben un valor del
   cliente (AC-8).

---

## 8. Deviations and assumptions

### 8.1 Incidente declarado: un `StackOverflow` en el gate de portabilidad (causa exacta y corregida)

El primer gate de portabilidad **crasheó el proceso de pruebas** (`[FATAL ERROR] … Test process crashed with exit
code -1073741571` = `0xC00000FD`, `STATUS_STACK_OVERFLOW`): su resolutor de tipos recursaba sobre un
`Nullable<T>` —el contrato declara `decimal?`/`DateTime?`/`bool?`— porque `GetGenericTypeDefinition()` aplicado a
una **definición** genérica devuelve ella misma, y el guardia que faltaba era `IsGenericTypeDefinition`. Se
corrigió con ese guardia (verificado por sustitución) y el proceso volvió a correr: es el caso que muestra por qué
el gate tiene que **ejecutarse**, no razonarse.

### 8.2 Los casos de integración fallan bajo un `dotnet test` sin filtro (por diseño, declarado)

`UserSettingsStoreIntegrationTests` exige que el host haya fijado el almacén real (la implementación del Bolt 010)
y lo declara con un mensaje explícito. La corrida **hermética** los excluye por `Category=Integration` —el
`build.ps1 -Scope Hermetic` filtra `Category!=Integration`— de modo que el gate del V-Bounce es **verde**; un
`dotnet test` **sin** el filtro los muestra fallando, que es exactamente el contrato del ciclo de integración
(entorno por provisionar). Queda declarado para que no se lea como un defecto.

### 8.3 Lo que la fase B **no** hizo (fronteras respetadas)

- **No** se aportó la superficie de serialización retirada: las 6 excepciones entran con su forma del 1.4 y su
  `[Serializable]`/constructor de serialización, porque su retiro es del **Bolt 007** (SPEC-0037 §4).
- **No** se tocó la referencia muerta `System.Windows.Forms` del `.csproj` (es del **Bolt 012**, que además dejó su
  control de higiene vigilando el árbol).
- **No** se implementó el sustituto del almacén (es del **Bolt 010**) ni se re-cifró, descifró o migró ningún valor
  (§15 #5: acá sólo se verifica que lo guardado **se lea**).

### 8.4 El límite de §15 #12 (el `using` muerto hacia un namespace inexistente)

**No se disparó**: el aporte no introdujo ningún `using` muerto (las 14 fuentes usan sólo la BCL y su propio
namespace; el uso de `System.Management` de `Utility.cs` estaba **cualificado**, así que al retirar la capacidad no
quedó ninguna directiva sin uso que la autorización de §B.2 tuviera que resolver). La única directiva aplicada
—`Utility.cs`— es un archivo **aportado con diagnóstico**, que es exactamente lo autorizado.

### 8.5 La clave sigue siendo la del 1.4 (la trampa de §15 #3, verificada)

El contrato de claves se preserva **byte a byte**: el separador `U+00A1`, el recorte y el plegado **por parte**, y
el camino de ausencia explícito. Se verificó de las dos maneras: por el **contrato** (los vectores dorados) y por
la **fuente congelada** (el `0xA1` como único byte no ASCII de `RepositoryMapper.cs`, en el offset 3222 medido en
los V-Bounces N°1 y N°2 de este Bolt). **No** se disparó la parada de §15 #3.

### 8.6 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está en `status: approved`, `revision: 3`, `turn_budget: 25`, `review.decision: approved`
(`2026-10-16T02:36:30-03:00`); el Bolt entró en `blocked` con T01–T07 presentes y se tomó con **T08**; los ADR
aplicados siguen `accepted`; la línea base es `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto
(§9.5). No hay cambio material → G15 no se activa.

---

## 9. Verification evidence

### 9.1 El aporte y la portabilidad, medidos

```
Librería 2.0: 20 archivos .cs (3 preexistentes del Bolt 011 + 14 aportados + 3 de Storage)
Utility.cs:   512 -> 493 líneas  ·  GetDriveSerialNumber / win32_logicaldisk / System.Management: 0 ocurrencias
Directivas §B.2: 1 archivo (Utility.cs, el único con diagnóstico) · 0 en código nuevo · 0 .csproj con la etiqueta
Capacidades eliminadas: los 4 archivos NO existen · DllImport / SetProcessWorkingSetSize / GetImpersonatedUser /
                        EventLog / GetDriveSerialNumber / System.Management: 0 ocurrencias en la librería
Builds (Release): Core EXIT=0 · Data EXIT=0 · Library EXIT=0 · Modeling EXIT=0      (0 diagnósticos los cuatro)
```

### 9.2 La suite (183 casos herméticos)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 100, Skipped: 0, Total: 100   ← 90 previos + 10 nuevos
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

Los **10 casos nuevos** son los 4 del gate de portabilidad y los 6 del contrato del almacén (vectores dorados,
evidencia del byte, camino de ausencia, round-trip por tipo, punto de fijación y limitación de cultura). Los **2**
de integración quedan fuera por `Category=Integration` (§8.2).

### 9.3 El gate de cobertura del arnés (fase F) y su cruce

El caso `ApiRemovalCoverageTests` cruza el reporte del gate de compatibilidad con las constancias: el retiro de
`Utility.GetDriveSerialNumber` aparece como **diferencia de miembro** (`CP0002`) y queda cubierto por
`BOLT009-04`; ninguna diferencia de miembro queda sin explicación en todo el árbol.

### 9.4 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 16 proyectos sin advertencias; la librería en su TFM portable (§9.1) |
| Unit / component tests | **pass** | 183/183 herméticos (§9.2) |
| Portabilidad / rastro de plataforma | **pass** | 0 plataforma y 0 terceros en la librería (§9.1) |
| Presupuesto de dependencias | **pass** | 0 `PackageReference`; el ensamblado referencia sólo BCL y el framework |
| Constancia y cobertura | **pass** | La constancia existe y el cruce de cobertura queda satisfecho (§9.3) |
| Integration tests (real dependencies) | **applicable — pendiente de su ciclo** | Escritos y compilados (§3.5); su corrida es del ciclo de integración con el entorno por provisionar (§8.2) |

### 9.5 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.6 Manifest y encoding

`devflow/metrics/bolts/US-000.BOLT-009-library-portable.json`: entrada **`v_bounces[2]`** (`number: 3`,
`spec_revision: 3`, `execution_outcome: ready_for_review`), validada contra `manifest-v5-bolt.schema.json`. Los
documentos y fuentes de este V-Bounce se escribieron en **UTF-8 sin BOM**, con `LF` y sin `U+FFFD` (§3.15/W23), y
**ningún valor del almacén** aparece en ellos (AC-8): los casos usan un valor sintético propio.

---

## 10. Manual interventions

**Ninguna intervención humana.** El aporte, la retirada de la capacidad WMI, la abstracción, los tests, la
constancia y las correcciones se hicieron en este V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0037](../spec/SPEC-261016-0037-library-portable.md) — **revisión 3**
  (`turn_budget: 25`, aprobada el `2026-10-16T02:36:30-03:00`): §6/B.2 (nulabilidad preventiva) y §15 #12 (su límite).
- **Bolt:** [US-000.BOLT-009](../functional/bolts/US-000.BOLT-009-library-portable.md) — fila **T08**.
- **V-Bounces previos:** [`MEM-261016-0215`](MEM-261016-0215-library-portable.md) (la evaluación de uso real) y
  [`MEM-261016-0227`](MEM-261016-0227-library-portable.md) (el alcance medido y el plan de B–F).
- **Manifiesto:** [US-000.BOLT-009-library-portable.json](../metrics/bolts/US-000.BOLT-009-library-portable.json).
- **Entregables:** `2.0/src/Cooperator.Framework.Library/**` (20 archivos), `2.0/docs/library-platform-features-assessment.md`,
  `2.0/docs/api-removals/US-000.BOLT-009.json` y los tres archivos de prueba del arnés.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.Library/Repository/RepositoryMapper.cs`
  (la derivación de clave y su único byte no ASCII), `Utility.cs` (la región y el método retirados), e
  `IO/{CurrentUserInfo,EventViewer,Memory,NativeMethods}.cs` (las capacidades eliminadas).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 3 (de US-000.BOLT-009) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0037 revisión **3** (`turn_budget: 25`) |
| Tiempo de generación IA | ~8 min de reloj (02:39:10 → 02:46:52) |
| Fases completadas | **6 de 6** (A en el N°1; **B–F en este V-Bounce**) |
| Archivos creados | **18** (14 del aporte + 3 de la abstracción + 3 de prueba) + la constancia |
| Archivos modificados | **1** (el Bolt: status y fila T08) — **0** del árbol 2.0 |
| Directivas de §B.2 | **1** (`Utility.cs`, la única con diagnóstico) |
| Tests creados | **12** (10 herméticos + 2 de integración) |
| Tests de la suite | **183** herméticos (100 + 83), todos verdes |
| Capacidades retiradas | **5** (con su constancia y su consecuencia) |
| Valores del almacén en artefactos | **0** (AC-8) |
| Condiciones de §15 disparadas | **ninguna** (incluidas #3 y #12, verificadas) |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **La corrida de las pruebas de integración** contra el almacén real (el usuario designado, con la implementación
   sustituta del Bolt 010 fijada): escrita y compilada acá; su corrida es del **ciclo de integración**, con el
   entorno por provisionar (§8.2).
2. **El sustituto del almacén**: su implementación —almacén de usuario en `%APPDATA%` o equivalente— es del
   **Bolt 010**; este Bolt entregó el contrato, la derivación de clave y el punto de fijación, y **no** la escribió
   (frontera de §4/§C.2).
3. **La frontera de otros Bolts sigue en pie:** la superficie de serialización de las excepciones de la librería
   (Bolt 007), la referencia muerta `System.Windows.Forms` del `.csproj` (Bolt 012, ya vigilada por su control de
   higiene) y la migración de valores cifrados del almacén (Bolts 022/030, ADR-007).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión. **Riesgo `high`:** el
SPEC §16 exige independencia de aprobación **endurecida a nivel de modelo**, y este MEM lo declara para su registro.

- **Artefacto presentado:** este MEM + `v_bounces[2]` del manifiesto (número 3, revisión 3) + el árbol entregado
  (los 14 portables, la abstracción, los 3 archivos de prueba y la constancia) + la corrida verde de **183 casos**.
- **Qué se pide decidir:** la aceptación del entregable de B–F y el paso del Bolt a su ciclo de integración.
