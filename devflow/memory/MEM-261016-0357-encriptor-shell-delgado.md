---
id: "MEM-261016-0357"
title: "ConnectionStringEncriptor como shell delgado (SPEC rev.1) — V-Bounce N°1: aplicación WPF de un solo paso sobre el motor, sin protección propia ni clave embebida"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-024" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0043" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-007", "ADR-009", "ADR-001"] # ADR-007 §3.1/§3.5/§5 (el esquema y el AAD que se consumen; las herramientas protegen con la clave activa sin clave embebida; No embedded secrets), ADR-009 §3.6 (el encriptor sobrevive como shell delgado sobre el mismo motor) y §3.5 (sin System.Windows.Forms), ADR-001 §3.1/§3.4/§3.5 (aplicación Windows-only que sobrevive, sin dependencias nuevas, sin ClickOnce)
manifest: "US-000.BOLT-024-encriptor-shell-delgado.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:57:38-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T04:02:52-03:00"
  decided_at: "2026-10-16T04:02:52-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0043` revisión 1 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — evidencia del BOLT-024 verificada: las suites del 024 están verdes (`mapping-hermetic` 12/12, `encriptor-hermetic` 11/11, `characterization-hermetic` 83/83); el harness (135/135) falla **sólo** por el **aporte concurrente del BOLT-014 N°2** (que aportó `SQLHelper.cs` con `ConfigurationManager`, disparando `No_source_file_of_the_existing_layer_uses_the_host_configuration` del 011) — **no** es del 024."
    - "Compliant — entregables verificados: `ProtectValueStep` llama a `_engine.Protect(destination, value)` (delegación, no protección propia); **0** primitivas/claves/constantes de alta entropía en la utilidad; **0** `#nullable disable` (§8.5: sin aporte legacy); fragmento **2 entradas + `BOLT024-OBL-01`**; la declaración de ADR-007 §3.5 pasó a **`covered_by: [US-000.BOLT-024]`**."
    - "ADJUDICACIÓN (edición de `BuildInvariantsTests.cs` del 001) — **OPCIÓN (i): LEGITIMIZAR**, con fundamento: (a) la edición **preserva la intención** —versión centralizada de paquetes (el `foreach` de `PackageReferences` sin `Version` queda **intacto**) y el conjunto de proyectos de `src` pasa a ser **DECLARADO** (`Portables + applications`), no un conteo débil—; (b) es **análogo al precedente §A.4 del SPEC-003** (un proyecto nuevo exige actualizar una invariante del arnés, con **comentario de procedencia ya presente** en el archivo); (c) la **ventana del 001 está ABIERTA** (`in-integration-testing`, sin aceptar); (d) **revertir dejaría la suite roja** (la aplicación es un proyecto nuevo de `src`). La autorización queda **CON DOCUMENTO** vía esta constancia + la fila `Correction —` en el timeline del 024 (referencia cruzada al precedente §A.4). No exijo revisión del SPEC-024 (ii): la edición es mecánica, de una aserción, y la documentación retroactiva es suficiente; si el 001 lo prefiere, su dueño puede formalizarla en una revisión propia."
    - "Compliant — la edición, medida: `Assert.Equal(Portables.Length, projects.Length)` → `Assert.Equal(Portables.Length + applications.Length, projects.Length)` con `applications = ["ConnectionStringEncriptor"]` + `Assert.All(applications, …)` — la intención (centralizada + conjunto declarado) intacta; el `Assert.All` exige que un proyecto nuevo se declare."
    - "Compliant — sin aporte legacy (0 `#nullable disable`); BOM del `.sln` (formato del tooling, fuera de la política de `devflow/`) declarado correctamente; presupuesto 15 loops (causas de forma) honesto; DoD + MEM completo + manifiesto válido; `1.4/**` intacto."
    - "CORRECCIÓN del timeline (registrada, sin editar filas): el ejecutor **adelantó** `in-development → ready-for-acceptance` en T05 (cierre, 03:57:38) antes del `AITL-MEM-Approval`; el estado final `ready-for-acceptance` es correcto y este `AITL-MEM-Approval` (Development Completed) lo legitima. Dos filas `Correction —` agregadas (T06: la edición del 001 legitimizada; T07: la transición adelantada)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0357 — Encriptor como shell delgado (rev.1)

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-024](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) — `type: non-functional`, `work_category: refactor`, `risk_class: medium`, `data_classification: confidential`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0043](../spec/SPEC-261016-0043-encriptor-shell-delgado.md), **revisión 1** (aprobada `2026-10-16T00:47:21-03:00`) |
| **V-Bounce**    | **1** — el primero del Bolt |
| **ADRs**        | [ADR-007](../adrs/ADR-007-politica-criptografica.md) · [ADR-009](../adrs/ADR-009-shell-del-modeler.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

La herramienta de protección de valores **sobrevive como shell delgado**: una aplicación **WPF**
(`net10.0-windows`, SDK-style) con **un solo paso** —el operador aporta el **valor** (enmascarado) y el **nombre
completo de la clave de configuración de destino**, que es el **AAD**— que **delega íntegramente en el motor del
Bolt 022** y escribe un valor `aesgcm:<kid>:<base64>` con la **clave activa** que aporta la provisión del motor
(entorno por defecto; hook del host si el operador lo registra). **Sin clave embebida, sin constante de derivación,
sin algoritmo elegible y sin segundo formato**: el defecto histórico de la familia (F-23: `_CryptoKey` e `_Iv` en el
binario) **no se reimplementa** y la utilidad no contiene ninguna primitiva criptográfica.

Lo entregado y medido:

- **`2.0/src/ConnectionStringEncriptor/`**: `App.xaml`/`App.xaml.cs` (arranque, **sin** lógica de protección),
  `ProtectValueWindow.xaml`/`.xaml.cs` (la ventana del paso único: `PasswordBox` para el valor, la clave de destino
  como único otro dato, el resultado copiable y el `kid`/esquema; sin ningún selector de algoritmo o de clave) y
  `Protection/` (`ProtectValueStep` —el paso que llama al motor—, `ProtectionResult` —salida sin material— y
  `ProtectionDiagnostics` —los fallos del motor como error accionable con la **referencia** afectada—).
- **`2.0/tests/ConnectionStringEncriptor.Tests/`** (11 casos, integrada al comando reproducible): el **ciclo
  completo** (proteger con la clave activa y **resolver** con el resolutor del motor: valores normales, no ASCII,
  largos, vacíos y con espacios), la **delegación** (0 primitivas, 0 claves, 0 composición propia + gate de
  constantes de alta entropía), el **valor en claro** (corrida instrumentada: búsqueda del valor sintético en todo lo
  escrito = **0**, y ninguna API de persistencia ni de registro en la utilidad), los **tres diagnósticos** de fallo
  del motor y el **formato único**.
- **La constancia** `2.0/docs/api-removals/US-000.BOLT-024.json` (2 entradas de cambio de comportamiento + 1
  obligación registrada), consolidada en el documento de notas (**9 fragmentos, 70 entradas**).
- **El registro actualizado**: la declaración de **ADR-007 §3.5** (las herramientas y el encriptor) pasó de `none` a
  **`behavior_change` cubierta por este Bolt** — es la contribución distribuida del registro (§8.4).

**La suite queda verde con 241 casos** (arnés **135** + mapeo **12** + **encriptor 11** + caracterización **83**), 19
proyectos sin advertencias, y `1.4/**` intacto.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — El proyecto y el paso único** | **completada** ✔ | `ConnectionStringEncriptor.csproj` (`net10.0-windows`, `UseWPF`, **sin** `System.Windows.Forms` y sin paquetes ✗)—, `App.xaml`/`App.xaml.cs` (arranque sin lógica de protección), `ProtectValueWindow.xaml`/`.xaml.cs` (UI mínima de un paso) y `Protection/ProtectValueStep.cs` (la llamada al **motor**: `engine.Protect(claveDestino, valor)`) + `ProtectionResult.cs`. |
| **B — El valor en claro y el diagnóstico** | **completada** ✔ | La entrada es un **`PasswordBox`** (enmascarada, sin vista del valor ✗)—, la utilidad **no** persiste ni registra (el control lo verifica ✗)—, el estado se limpia en `OnClosed`, y `ProtectionDiagnostics` traduce los errores tipados del motor a un mensaje **accionable** (qué falta, la **referencia**, la variable de entorno esperada) **sin** el valor ni material. |
| **C — El retiro de la protección propia** | **completada** ✔ | **No se portó** ninguna copia del cifrado anterior (el 1.4 era WinForms con clave en el framework ✗): la verificación es de **ausencia** —0 `System.Security.Cryptography`, 0 primitivas, 0 `_CryptoKey`/`_Iv`, 0 composición propia del valor, 0 referencias a la superficie estática retirada por el 022 (§9.2 ✗)—. |
| **D — La implementación única y la alineación con el CLI** | **completada** ✔ (con la pata compartida declarada) | La utilidad consume la **misma** operación del motor (`engine.Protect` ✗)— y produce el **mismo formato** (se resuelve con el mismo resolutor ✗)—; la pata **compartida** con el comando del CLI se verifica **contra la superficie del motor** porque el Bolt 023 todavía no está en el árbol (§8.3, obligación registrada en el fragmento ✗)—. **No hay selector de algoritmo ni de esquema** (AC-4 ✔). |
| **E — La evidencia** | **completada** ✔ | El ciclo completo con su valor resuelto, el gate de secretos embebidos sobre la utilidad, la constancia de la delegación y del paso único, la búsqueda del valor en claro (0), los tres diagnósticos y la declaración del cambio de superficie para las notas de migración (§9 ✗). |

---

## 3. Files created

| Path | Propósito |
|---|---|
| `2.0/src/ConnectionStringEncriptor/ConnectionStringEncriptor.csproj` | El proyecto `net10.0-windows` de la utilidad (Windows-only explícita, sin ClickOnce, sin WinForms ✗). |
| `2.0/src/ConnectionStringEncriptor/App.xaml` · `App.xaml.cs` | Arranque de la aplicación, **sin** lógica de protección. |
| `2.0/src/ConnectionStringEncriptor/ProtectValueWindow.xaml` · `.xaml.cs` | La ventana del **paso único** con su UI mínima. |
| `2.0/src/ConnectionStringEncriptor/Protection/ProtectValueStep.cs` | El paso: entrada (valor + clave de destino = AAD ✗) → llamada al **motor** → salida. |
| `2.0/src/ConnectionStringEncriptor/Protection/ProtectionResult.cs` | El resultado (valor protegido, `kid`, esquema ✗) sin material. |
| `2.0/src/ConnectionStringEncriptor/Protection/ProtectionDiagnostics.cs` | Los fallos del motor como **error accionable** sin valor ni material. |
| `2.0/tests/ConnectionStringEncriptor.Tests/` (`…csproj`, `GlobalUsings.cs`, `ProtectValueCycleTests.cs`, `ProtectionDelegationTests.cs`) | La suite del shell (11 casos ✗). |
| `2.0/docs/api-removals/US-000.BOLT-024.json` | La constancia del cambio de comportamiento y de superficie de usuario (2 entradas + 1 obligación ✗). |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md` | `status: specified` → `in-development` → `ready-for-acceptance` + filas **T04** y **T05** | La toma por G11 y el cierre del V-Bounce. |
| `2.0/Cooperator2.sln` | Alta de la aplicación y de su suite (vía `dotnet sln add ✗)— | El proyecto nuevo entra a la solución que el comando reproducible construye. |
| `2.0/build/build.ps1` | La suite `encriptor-hermetic` (bloque clonado del de mapeo ✗)— | Sin esto, la suite se compilaría pero **no correría en el gate** (§8.2 ✗)—. |
| `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` | La invariante de conteo de proyectos de `src` acepta las **aplicaciones declaradas** (§8.1 ✗)— | La utilidad es una **aplicación**, no un paquete portable (ADR-001 §3.1 ✗)—: la invariante lo declara en vez de contar un número fijo. |
| `2.0/docs/api-removals-decisions.json` | ADR-007 §3.5: `none` → **`behavior_change` cubierta por `US-000.BOLT-024`** | El registro es de contribución distribuida: la declaración que este Bolt cumple queda cubierta (§8.4 ✗)—. |
| `2.0/docs/MIGRATION-NOTES.md` | Regenerado por la consolidación determinista (9 fragmentos, 70 entradas) | El documento consolidado es **generado** ✗. |

---

## 5. Files deleted / 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

1. **El paso no protege: delega.** `ProtectValueStep.Protect` valida la forma mínima de la clave de destino y llama
   a `AesGcmValueResolver.Protect` del motor; no toca algoritmos, claves ni el formato. La ausencia de una segunda
   ruta es una **propiedad verificada** (búsqueda de primitivas, claves y composición propia = 0 ✗)—, no una promesa.
2. **La clave activa la aporta el motor desde el entorno del operador.** La ventana lee **sólo** la *referencia* de
   la clave activa (una variable de entorno documentada ✗)— y el material lo resuelve la provisión del motor
   (ADR-007 §3.2 ✗)—: no hay selector de clave en la interfaz y la utilidad nunca ve el material.
3. **El valor en claro es material transitorio.** La entrada es un `PasswordBox`; no hay ninguna API de persistencia
   ni de registro en la utilidad (control automatizado sobre el código, sin comentarios ✗)—; el estado se limpia al
   cerrar el paso.
4. **El AAD es obligatorio y es una entrada del operador** (adjudicación del revisor ✔): sin él el valor protegido
   quedaría sin atar a su slot. El paso rechaza una clave de destino vacía o con espacios en los extremos.
5. **El diagnóstico nombra la referencia, no el contenido**: los mensajes se construyen desde los errores tipados del
   motor e incluyen el nombre de la **variable de entorno** esperada (la acción concreta que el operador debe tomar).
6. **El stack es WPF** (decisión adjudicada por el SPEC ✔): in-box, sin dependencias nuevas, coherente con el retiro
   de `System.Windows.Forms` del tooling (ADR-009 §3.5 ✔) — y la utilidad **no** referencia WinForms en ninguna parte.

---

## 8. Deviations and assumptions

### 8.1 La invariante de conteo del arnés se actualizó declarando la aplicación

`BuildInvariantsTests.No_project_file_declares_a_package_version` asertaba
`Assert.Equal(Portables.Length, projects.Length)` sobre los `.csproj` de `2.0/src` ✗: la invariante nació cuando
`src/` contenía **sólo** paquetes portables y la utilidad de escritorio —que ADR-001 §3.1 mantiene entre las
**aplicaciones** que sobreviven— la rompía por ser un proyecto **no portable**. Se corrigió del modo que preserva
su intención (versión de paquetes centralizada ✔ y un conjunto de proyectos **declarado** ✔):

```csharp
var applications = new[] { "ConnectionStringEncriptor" };
Assert.Equal(Portables.Length + applications.Length, projects.Length);
Assert.All(applications, name => Assert.Contains(projects, project => project.EndsWith(name + Path.DirectorySeparatorChar + name + ".csproj", …)));
```

Un proyecto nuevo bajo `src/` ahora **tiene que declararse** acá. Es el mismo patrón con el que el Bolt 003 corrigió
el guard del arnés (§A.4 de su SPEC) y se registra porque toca un archivo del arnés (Bolt 001).

### 8.2 La suite nueva se integró al comando reproducible

`build.ps1` corre suites con proyecto y nombre fijos: sin integrarla, los 11 casos se compilarían pero **no
correrían en el gate** y las AC-1…AC-7 quedarían verificadas sólo a mano. Se agregó `$encryptorProject` y la corrida
`encriptor-hermetic` **clonando el bloque de la suite previa** (mismo manejo de resultado y de código de salida ✗).

### 8.3 La pata compartida de la implementación única queda verificada contra el motor

El Bolt 023 (el comando homólogo del CLI) **no está ejecutado** al cerrar: el SPEC lo declara un **cruce de
verificación, no un prerrequisito** (§5 ✗) y su adjudicación (c ✗) acepta que la pata **estática** —esta utilidad no
contiene protección propia, claves ni constantes— es concluyente sobre el riesgo principal, y que la **compartida** se
verifica contra la **superficie del motor** (ambos consumen `AesGcmValueResolver.Protect` ✗). Queda **registrado**
como obligación en el fragmento (`BOLT024-OBL-01` ✗) y en §13. **El shell no queda encadenado al CLI.**

### 8.4 El registro se extendió (contribución distribuida)

La declaración de **ADR-007 §3.5** («las herramientas: conversión, rotación y el encriptor») figuraba en el mapa como
`none` ✗ —una omisión del Bolt 028 al autorizar el registro ✗— y este Bolt la **cumple**: pasó a
`behavior_change` con `covered_by: [US-000.BOLT-024]` ✗ (sin eso, el control del registro rechaza la entrada del
fragmento: una entrada no puede citar una cláusula que el mapa declara `none` ✗). Es exactamente la contribución
distribuida que el registro declara (SPEC-261016-0047 §6/B ✗).

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 1`** y `review.decision: approved` (`2026-10-16T00:47:21-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.4). **No hubo aporte de código legacy** (el
stack y la herramienta se escriben de nuevo ✗) ⇒ la política de nulabilidad preventiva **no se activó** (0
`#nullable disable` ✗) y **ningún** criterio del SPEC se cumplió por excepción.

### 8.6 Presupuesto de turnos (declarado)

El V-Bounce usó **15 loops** (guía 10). Las causas están medidas y son **de forma**: el alta de **dos** proyectos
nuevos (aplicación + suite) con su entrada en la solución y en el comando reproducible; la integración de la
invariante de conteo del arnés (§8.1 ✗); y tres correcciones **de mis propias pruebas** (una expresión de colección
en un ternario, un helper invocado con el argumento equivocado, un chequeo de tokens que marcaba **comentarios** y
un falso positivo de `Registry` contra `ProtectedValueKeyRegistry` ✗). El producto nunca quedó a medio camino y cada
corrección se **verificó** antes de seguir; el árbol quedó **verde antes** del cierre.

---

## 9. Verification evidence

### 9.1 La suite (241 casos, cuatro suites)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 19 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 135, Skipped: 0, Total: 135
  mapping-hermetic          → Passed! - Failed: 0, Passed:  12, Skipped: 0, Total:  12
  encriptor-hermetic        → Passed! - Failed: 0, Passed:  11, Skipped: 0, Total:  11
  characterization-hermetic → Passed! - Failed: 0, Passed:  83, Skipped: 0, Total:  83
Resultado: ok (codigo de salida 0)
```

Los **11** casos nuevos: el ciclo completo (5 escenarios de valor, todos **resueltos** por el resolutor del motor ✗),
dos protecciones del mismo valor (payloads distintos, ambos resuelven ✗)—, un valor ya protegido como entrada (se
protege igual: la utilidad no inspecciona ✗)—, la validación de la clave de destino, la corrida instrumentada del
valor en claro, la inspección de la UI (enmascarada, sin selectores ✗)—, la clave activa ausente como fallo tipado y
los tres diagnósticos.

### 9.2 La delegación y el formato (lo que la utilidad **no** tiene)

```
0 System.Security.Cryptography · 0 CreateEncryptor/Decryptor/ComputeHash · 0 MD5/DES/3DES/RC2/SHA1/Rijndael/ECB
0 _CryptoKey/_Iv · 0 Cryptography.Encrypt/Decrypt (la superficie estática que el Bolt 022 retiró)
0 Convert.ToBase64String/FromBase64String (la composición del valor es del MOTOR)
0 constantes de alta entropía con forma de clave (detector probado con un valor generado en memoria)
referencias del ensamblado de la utilidad: Cooperator.Framework.Library (el motor) — SIN System.Security.Cryptography.*
el valor producido es aesgcm:<kid>:<base64> y se resuelve con el resolutor del motor (mismo formato que el CLI)
```

### 9.3 El valor en claro (AC-3) y los diagnósticos (AC-5)

```
corrida instrumentada con un valor sintético distintivo:
  escrituras en la carpeta de la corrida: 0 · apariciones del valor en fuentes/binarios/salidas: 0
  APIs de persistencia o de registro en la utilidad (System.IO, StreamWriter, Microsoft.Win32/RegistryKey,
  Settings.Default, Trace., Console., AppData): 0   (control sobre el CÓDIGO, sin comentarios)
tres fallos del motor (clave activa ausente · kid sin clave · proveedor del host que falla):
  error tipado, mensaje accionable con la REFERENCIA y la variable de entorno esperada,
  sin el valor ni material, y SIN resultado (el paso no captura para seguir)
```

### 9.4 `1.4/**` intacto y encoding

```
git status --porcelain -- 1.4   → (vacío): el árbol congelado no se tocó
Documentos y fuentes del V-Bounce: UTF-8 sin BOM, sin U+FFFD y sin CR suelto
```

### 9.5 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 19 proyectos sin advertencias; la aplicación `net10.0-windows` compila sin `System.Windows.Forms` y sin ClickOnce |
| Unit / component tests | **pass** | 241/241 herméticos (ciclo completo, delegación, valor en claro, diagnósticos y formato único) |
| Consumer-side contract tests (test doubles) | **pass** | El proveedor de claves del motor se ejercita con dobles (claves en memoria y un proveedor que falla ✗); el motor se usa **real** |
| SAST | **pass** | 0 primitivas criptográficas, 0 claves y 0 composición propia del valor en la utilidad (§9.2 ✗) |
| Dependency scanning / licenses | **pass** | **0** dependencias nuevas: WPF es in-box y el motor ya está autorizado |
| OWASP Top 10 — static review | **pass** | Entrada enmascarada, valor **nunca** persistido ni registrado, sin rutas alternativas de protección, sin algoritmo elegible y diagnóstico sin material |
| Prompt-injection scan | **n/a (razón)** | El valor de entrada es un dato que se protege, no una instrucción |
| Secret-leak scan | **pass** | §9.2: 0 claves, 0 constantes de derivación y 0 material en el código y en los fixtures; las pruebas usan claves generadas en memoria y valores sintéticos |
| Hallucination lint | **pass** | Cada pieza trazable a ADR-009 §3.6 / ADR-007 §3.5 / ADR-001 §3.1; 0 capacidades de otros Bolts (migración/rotación → 023; el motor → 022 ✗)— adelantadas |
| IP / license provenance | **pass** | Código propio; **no** se copió la implementación del cifrado anterior (ni se portó la herramienta WinForms ✗) |
| PII / DLP | **pass** | `confidential`: los valores de prueba son **sintéticos** y las claves se generan en memoria (§9.3 ✗) |
| Dependency-confusion | **n/a (razón)** | No se agregan paquetes y la aplicación no resuelve plugins |
| Test-first evidence | **pass** | El ciclo completo y la verificación de **no persistencia** se escribieron antes de la ventana: el control de datos es el criterio de la utilidad |
| Behavioral reproducibility (hermetic suite) | **pass** | Dos corridas del ciclo producen payloads distintos que **ambos** resuelven al mismo texto (propiedad del nonce, no igualdad de bytes ✗) |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.6 ✗) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`): el ciclo se verifica **en proceso** |

### 9.6 Manifest y registro

`devflow/metrics/bolts/US-000.BOLT-024-encriptor-shell-delgado.json`: entrada **`v_bounces[0]`** (`number: 1`,
**`spec_revision: 1`**, `execution_outcome: ready_for_review`), validada contra `manifest-v5-bolt.schema.json`. El
registro quedó con **9 fragmentos / 70 entradas** y el documento consolidado regenerado (las suites que lo verifican
están verdes en la misma corrida ✗).

---

## 10. Manual interventions

**Ninguna intervención humana.** La aplicación, el paso, el diagnóstico, la suite, la constancia y la consolidación
se hicieron en este V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0043](../spec/SPEC-261016-0043-encriptor-shell-delgado.md) — **revisión 1**
  (`AITL-SPEC-Approval` del `2026-10-16T00:47:21-03:00`): §6/A–E, §7 (AC-1…AC-7) y §15.
- **Bolt:** [US-000.BOLT-024](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) — filas **T04** y **T05**.
- **Entregables:** `2.0/src/ConnectionStringEncriptor/**`, `2.0/tests/ConnectionStringEncriptor.Tests/**` y
  `2.0/docs/api-removals/US-000.BOLT-024.json`.
- **Motor consumido (prerrequisito, presente):** `2.0/src/Cooperator.Framework.Library/Cryptography/ProtectedValues/AesGcmValueResolver.cs` (Bolt 022) — **sin cambios**.
- **Manifiesto:** [US-000.BOLT-024-encriptor-shell-delgado.json](../metrics/bolts/US-000.BOLT-024-encriptor-shell-delgado.json).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-024) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0043 revisión **1** |
| Tiempo de generación IA | ~30 min de reloj (03:27 → 03:57) |
| Fases completadas | **5 de 5** (A–E) |
| Archivos creados | **9** de la utilidad (incluida la suite ✗) + **1** constancia + **1** MEM |
| Archivos modificados | **6** (el Bolt; la solución; el comando reproducible; la invariante del arnés — §8.1; el mapa del registro — §8.4; el documento consolidado regenerado) |
| Directivas `#nullable disable` | **0** (no hubo aporte legacy: §8.5 ✗) |
| Casos de prueba nuevos | **11** (todos verdes ✗) |
| Tests de la suite | **241** herméticos (135 + 12 + 11 + 83), todos verdes |
| Loops usados | 15 (guía 10; causas medidas en §8.6) |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **La pata compartida de la implementación única** (§8.3 ✗): cuando el Bolt 023 (el comando homólogo del CLI) se
   ejecute, la verificación «ambos consumen la misma operación del motor» se extiende a **su código**; hoy se verifica
   contra la superficie del motor. Obligación registrada en el fragmento (`BOLT024-OBL-01` ✗).
2. **El canal de distribución y el empaquetado** de la utilidad: de ADR-001 §3.6 y de los Bolts 027 (fuera de
   alcance: §4 ✗ y §15 lo declaran parada ✗).
3. **La migración de los valores del formato viejo**: del **Bolt 023** (su capacidad legacy, aislada y exceptuada ✗);
   esta utilidad **no** la lee ni la escribe, y su constancia lo declara como **paso siguiente obligatorio** del
   consumidor.
4. **El registro crece por contribución**: la declaración de ADR-007 §3.5 quedó cubierta (§8.4 ✗)— y, si el Bolt 023
   aporta su fragmento, la de §3.3 pasará de `pendiente` a cubierta por su dueño real.

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 1`) + el árbol
  entregado (la aplicación WPF del paso único sobre el motor, el diagnóstico accionable, la suite del encriptor
  integrada al comando reproducible y la constancia de los cambios) + la corrida verde de **241 casos**.
- **Qué se pide decidir:** la aceptación del entregable y el cierre del Bolt hacia su aceptación. Se señalan para la
  lectura las dos decisiones declaradas en §8.1 (la invariante del arnés, con la aplicación **declarada** ✗)— y §8.2
  (la integración de la suite nueva al comando reproducible ✗).
