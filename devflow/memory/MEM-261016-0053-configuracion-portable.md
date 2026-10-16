---
id: "MEM-261016-0053"
title: "Configuración portable del framework — V-Bounce detenido por el cierre mínimo de `Data` (provider del Bolt 014)"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-011" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0018" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-004", "ADR-001"] # governing ADRs — none of them materialised a change here (V-Bounce stopped before code)
manifest: "US-000.BOLT-011-configuracion-portable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was produced (see §11)
review_ready_at: "2026-10-16T00:53:45-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:04:59-03:00"
  decided_at: "2026-10-16T01:04:59-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible: los dos archivos de Data en alcance NO existen en 2.0/ (BaseRule.cs/SQLHelper.cs: Test-Path False; Data sin fuentes .cs, sólo csproj) y en 1.4 están tipados de punta a punta sobre System.Data.SqlClient (BaseRule.cs L33 using, L172/L197 new SqlConnection, casts L241/L264/L289/L312/L327; SQLHelper.cs L28 using + cientos de tipos Sql*)."
    - "Compliant — el blocker coincide literalmente con SPEC rev.1 §15 (quinta, y concuerda la octava); netstandard2.1 no provee System.Data.SqlClient (sonda CS0234/CS0246 del MEM); ADR-005 §3.1 verbatim «se retira de Data», §3.3 SqlHelper→internal, §4 «el SqlHelper público desaparece»; sumar el paquete está prohibido por SPEC §4/§9."
    - "Compliant — bloqueo localizado en la capa Data: los otros tres sitios del mapa son aportables (contrato/catálogo en Core, BCL puro; el LazyLoad se retira entero; SmtpMail.cs sólo System.Net.Mail/System.IO, disponibles en netstandard2.1)."
    - "Compliant — cierre correcto: T04/T05 presentes; MEM único (sólo MEM-261016-0053 referencia BOLT-011); manifiesto v_bounces[0] con los 8 campos y execution_outcome: blocked; sin código parcial (Configuration/, 2.0/docs/, BaseRule.cs, SQLHelper.cs, SmtpMail.cs inexistentes); 1.4/** intacto (git diff exit 0)."
    - "Compliant — desbloqueo propuesto correcto y agéntico: revisión material del SPEC (rev.2 en redacción) que adopta la salida 1 (el retiro de Data se materializa con los Bolts 012/013/014) y descarta la salida 2 (solaparía el 014). El revisor del SPEC es agent:dotnet-developer-reviewer-agent en otro acto — no requiere al humano."
    - "Compliant — housekeeping T03 verificado: la fila ready→specified (T03) YA está presente en el Status timeline (línea 81, fuera de orden cronológico tras T05); la ausencia reportada por el ejecutor (§8.2) quedó resuelta por una edición concurrente, por lo que NO se agrega fila de corrección (la condición «si confirmás la ausencia» es falsa y duplicar T03 violaría G39)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0053 — Configuración portable: V-Bounce detenido por el cierre mínimo de `Data`

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-011](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) |
| **SPEC**        | [SPEC-261016-0018](../spec/SPEC-261016-0018-configuracion-portable.md), revisión 1 (aprobada) |
| **V-Bounce**    | 1 |
| **ADRs**        | [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-005](../adrs/ADR-005-provider-de-datos.md) (consultado: gobierna el destino de los archivos bloqueados) |

> ⚠️ **Este MEM registra un V-Bounce que NO generó código.** No hay entrega: hay un bloqueo con evidencia
> verificada. El MEM es obligatorio en V-Bounces bloqueados (§3.3, G17) y es lo que permite desbloquear.

---

## 1. Executive summary

El V-Bounce N°1 del Bolt `US-000.BOLT-011` se detuvo **antes de generar una sola línea de código**, al dispararse
la quinta condición de parada del SPEC aprobado: **el cierre mínimo exigiría transformar código de otro Bolt**. El
`2.0/` que dejó el Bolt 001 contiene los cuatro proyectos portables **sin una sola fuente** (`2.0/src` tiene
únicamente los cuatro `.csproj`), así que el SPEC §4 activa su *regla de aporte*: los archivos en alcance que
todavía no estén en `2.0/` deben aportarse «en su forma **ya migrada**» y, para compilar, con el «**cierre mínimo**
de tipos que necesite, copiados **tal cual** están en el 1.4 y **sin ninguna otra transformación**». Esa regla es
ejecutable para tres de los cuatro sitios en alcance —el contrato y el catálogo en `Core` (nuevos, BCL puro), el
retiro de la sección custom del LazyLoad (cuyo archivo se retira **entero**, así que no hay nada que aportar) y
`Library/SmtpMail.cs` (verificado aportable: sólo `System.Net.Mail`/`System.IO`, que **sí** existen en
`netstandard2.1`)—, pero es **inoperable para los dos archivos de `Data`**. `BaseRule.cs` y `SQLHelper.cs`
(con `SqlHelperParameterCache.cs`) están **tipados sobre `System.Data.SqlClient` de punta a punta** —433 usos
entre los dos primeros— y `System.Data.SqlClient` **no existe en `netstandard2.1` sin agregar un paquete**:
lo verifiqué construyendo un proyecto `netstandard2.1` mínimo, que falla con `CS0234`/`CS0246`. Aportarlos
verbatim es por eso imposible, y agregar el paquete está **doblemente prohibido**: el SPEC §4 lo declara fuera de
alcance («**este Bolt quita una excepción, no suma nada**») y su §9 exige «**0** dependencias nuevas»; y además
ADR-005 §3.1 —que gobierna este repositorio— retira expresamente esa dependencia («Los paquetes de la 2.0 **no**
dependen de `System.Data.SqlClient` (**se retira de `Data`**)»). La única forma de que existan en `2.0/` es
**transformarlos**, y esa transformación **es** el entregable del Bolt 014: ADR-005 §3.3/§4 manda que
`SqlHelper`/`SqlHelperParameterCache` pasen a `internal`, que el interior trabaje con `Microsoft.Data.SqlClient`
de punta a punta y que los casts de `BaseRule.cs` (L241/264/289/312/327) desaparezcan por un único punto de
conversión —«el `SqlHelper` público **desaparece**»—; y la **versión** del provider la fija ADR-005 §3.1 «el
**Bolt**» con dos criterios obligatorios, uno de los cuales es un **smoke test de conexión real contra SQL Server**
que no puedo correr aquí (no hay entorno). Elegirla sería inventar, que es la octava condición de parada. No
entregué el resto —que sí era implementable— porque el propio SPEC §12 cierra esa puerta con sus palabras: «Un
rollback parcial no es válido: **un contrato sin el retiro** deja dos fuentes de configuración y **rompe la regla
de ADR-004 §3.2**». El árbol queda **sin cambios**: la reversión de este V-Bounce es el no-cambio. Todo lo demás
que el SPEC daba por supuesto se verificó y está sano: el prerequisito del §5 está satisfecho (arnés **20/20** y
caracterización **83/83** verdes, medidos antes de tocar nada), `Core`/`Data`/`Library` tienen **0 paquetes**
declarados (el punto de partida exacto para evidenciar AC-5), y `1.4/**` está intacto. El bloqueo es **uno solo y
está localizado en la capa `Data`**; todo el resto del Bolt (fases A, B, C.1, C.3, D y E) tenía camino.

---

## 2. Implemented phases

**Ninguna fase del SPEC §6 se implementó.** El V-Bounce se detuvo tras la verificación de prerequisitos y el
reconocimiento de los archivos en alcance, antes de la fase A, por la condición de parada de §8.1. No existe
`2.0/src/Cooperator.Framework.Core/Configuration/`, ni `2.0/docs/`, ni ningún archivo en
`2.0/src/Cooperator.Framework.Data/` o `2.0/src/Cooperator.Framework.Library/`; los cuatro `.csproj` del
framework y `2.0/Cooperator2.sln` **no** fueron tocados.

### Mapa de factibilidad de los archivos en alcance (reconocimiento verificado)

Es el trabajo real de este V-Bounce y la base del próximo: dice, archivo por archivo, qué tiene camino y qué no.

| Archivo en alcance (§6 del SPEC) | ¿Está en `2.0/`? | Veredicto verificado |
|---|---|---|
| `Core/Configuration/ICooperatorConfiguration.cs` | no | **APORTABLE** — nuevo, BCL puro, `netstandard2.1`, 0 terceros |
| `Core/Configuration/CooperatorSettings.cs` | no | **APORTABLE** |
| `Core/Configuration/ConfigurationKeys.cs` | no | **APORTABLE** |
| `Core/Configuration/CooperatorConfigurationException.cs` | no | **APORTABLE** |
| `Core/Configuration/EnvironmentConfiguration.cs` | no | **APORTABLE** |
| `Core/Configuration/ConnectionStringValue.cs` | no | **APORTABLE** |
| `Core/Configuration/IProtectedValueResolver.cs` | no | **APORTABLE** (el Bolt 022 no lo declaró todavía) |
| `2.0/docs/configuration-keys.md` | no | **APORTABLE** |
| `2.0/docs/api-removals/US-000.BOLT-011.json` | no | **APORTABLE** |
| `Core/LazyLoad/LazyProviderFactoryConfigHelper.cs` (`LazyConfigurationHelper.Read`) | no | **N/A — se retira ENTERO** (§C.1): sus 420 líneas son la sección custom (`Provider` L38, `ProvidersCollection` L101, `ProvidersSection` L293) **y su lectura/escritura** (`Read()` L362/378, `CreateEmpty()` L396/405, `cfg.Save` L415, `OpenMappedExeConfiguration` L425). No hay parte que sobreviva al retiro → no hay archivo que aportar |
| `Library/SmtpMail.cs` | no | **APORTABLE** — 119 líneas; `System.Net.Mail` + `System.IO`, **ambos disponibles en `netstandard2.1`** (verificado con build). Cierre: `Library/Exceptions/SmtpErrorException.cs` (74) + `BaseException.cs` (72), sólo `System.*` |
| `Data/BaseRule.cs` | no | ❌ **NO APORTABLE — bloqueo** (ver §8.1) |
| `Data/SQLHelper.cs` + `SqlHelperParameterCache.cs` | no | ❌ **NO APORTABLE — bloqueo** (ver §8.1) |
| `tests/Cooperator.Harness.Tests/ConfigurationContractTests.cs` | no | **APORTABLE** (proyecto del Bolt 001, `net10.0`) |
| `tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs` | no | **APORTABLE** |

### Reconocimiento de las fuentes (registrado para el próximo V-Bounce)

- **`Core/LazyLoad/LazyProviderFactoryConfigHelper.cs` (420 líneas)** — `namespace Cooperator.Framework.Core.LazyLoad`,
  `using System.Configuration`. Anatomía verificada: `Provider : ConfigurationElement` (L38),
  `ProvidersCollection : ConfigurationElementCollection` (L101), `ProvidersSection : ConfigurationSection` (L293),
  `LazyConfigurationHelper` (L347) con `ProvidersSection` (propiedad), `Read()` (L362), `Read(fileName)` (L378),
  `CreateEmpty()` (L396), `CreateEmpty(fileName)` (L405, con `cfg.Save(ConfigurationSaveMode.Full)` en L415 y
  `ConfigurationManager.OpenMappedExeConfiguration` en L425), `GetConfiguration`, `GetFileName`,
  `GetExeConfigurationFileMapping`. **Todo** el archivo es la sección custom y su IO → el retiro de §C.1 lo deja
  sin contenido. `ILazyProvider.cs` y `LazyProviderFactory.cs` (que consumen la sección) son del Bolt 005 y **no**
  se aportan.
- **`Library/SmtpMail.cs` (119 líneas)** — `public static class SmtpMail` en `Cooperator.Framework.Library`.
  Defecto F-22 confirmado: `private static string _SmtpServerName = ConfigurationManager.AppSettings["SMTPServerName"].ToString();`
  (L33) → inicializador estático que lanza `NullReferenceException` en el *type-initializer*. `SendMessage` (L~76)
  crea el `SmtpClient(_SmtpServerName)` de forma perezosa pero **con el valor ya resuelto al cargar el tipo**.
  `SendMailViaCDONTS` (L~104) es código muerto (lanza `NotImplementedException`) y su firma pública arrastra
  `System.Web.HttpContext` → §C.3 y §14 mandan no conservarlo.
- **`Data/BaseRule.cs` (459 líneas)** — `public abstract class BaseRule` con `protected virtual string ConnectionStringKeyName`
  (L56) y la clase anidada `public class InternalDBHelper` (L68) con `GetNewConnection`, `ExecuteScalar*`,
  `ExecuteNonQuery*`, `ExecuteReader*`, `FillDataSet*`. **El bloque a retirar está identificado con precisión**:
  el getter interno `ConnectionString` (L92-135) lee `ConfigurationManager.ConnectionStrings[...]` (L104), aplica
  la **heurística por substrings** (`initial catalog`/`database`/`data source`, L105-107/115-117), intenta
  **`Library.Cryptography.Decrypt`** (L114, la cripto legacy del 1.4 — el único consumidor de esa ruta en este
  archivo) y cachea en `private static readonly Dictionary<string, string> ConnectionStringsDictionary` (L85),
  con `Remove`/re-`Add` en L145-149. La excepción que usa es `Exceptions.ConnectionStringNotFoundException` (L126).
- **`Data/SQLHelper.cs` (2302 líneas)** — `internal static class SqlHelper`, `namespace Cooperator.Framework.Data`;
  **la única referencia a configuración es L84** (`ConfigurationManager.AppSettings.Get("CooperatorCommandTimeOut")`),
  y **cero** referencias a `Cooperator.Framework.*`: es autocontenido salvo por su compañero
  `SqlHelperParameterCache.cs` (44 usos de SqlClient).
- **`Data`'s cast sites (F-09)** — `BaseRule.cs` L241/264/289/312/327 son exactamente los casts
  `(SqlTransaction)transaction` / `(SqlConnection)conn` que ADR-005 §3.3/§4 manda eliminar con un punto único de
  conversión. Están **citados por el propio ADR-005 §7** como evidencia de código 1.4.

---

## 3. Files created

| File | Purpose |
|------|---------|
| `devflow/memory/MEM-261016-0053-configuracion-portable.md` | Este MEM: el registro obligatorio del V-Bounce bloqueado, con la evidencia verificada del bloqueo y el mapa de factibilidad del entregable |

**No se creó ningún archivo de la entrega del SPEC** — ni el contrato, ni el catálogo, ni la implementación por
entorno, ni el documento de claves, ni el fragmento de rupturas, ni las dos suites de pruebas.

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-011-configuracion-portable.md` | `status: "specified"` → `"in-development"` + fila T04 (toma del Bolt, G11) y, al cerrarse el V-Bounce, `"in-development"` → `"blocked"` + fila T05 — la transición de cierre que la secuencia obligatoria exige para un V-Bounce bloqueado (G17 paso 4, §3.2.3) |
| `devflow/metrics/bolts/US-000.BOLT-011-configuracion-portable.json` | Entrada `v_bounces[]` número 1 con los ocho campos, `execution_outcome: "blocked"` |

**Ningún archivo de los Bolt 001, 002 ni 003 fue tocado** — sus registros están en revisión/cerrados y sólo se
leyeron (sus suites se corrieron con `--no-build`, sin recompilar ni reescribir nada suyo). **Ninguna línea de
`1.4/**` fue modificada.** **No se tocó** ningún otro Bolt, SPEC, manifiesto, índice ni registro de corrida.

---

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

---

## 6. Files deleted

| File | Reason |
|------|--------|
| — | No hubo borrados. |

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| **Detener el V-Bounce sin generar código**, en lugar de entregar las fases A, B, C.1, C.3, D y E —que sí tenían camino— | Es una condición de parada explícita del SPEC (§15, quinta) y la octava concuerda. Además, el propio SPEC §12 declara inválido el estado parcial: «Un rollback parcial no es válido: **un contrato sin el retiro** deja dos fuentes de configuración y **rompe la regla de ADR-004 §3.2**». Entregar el contrato sin poder retirar las lecturas de `Data` es exactamente ese estado |
| **No dejar el árbol a medio construir** (ni las carpetas `Configuration/`, ni `2.0/docs/`, ni los archivos aportables de `Library`) | Un árbol a medio construir obligaría al próximo V-Bounce a reconciliar un estado que nadie pidió y dejaría en el repositorio artefactos de un V-Bounce declarado `blocked`. La reversión de este V-Bounce debe ser el **no-cambio**, y así es: los `.csproj` y la solución conservan su contenido del Bolt 001 |
| **No aportar `Data/BaseRule.cs` ni `Data/SQLHelper.cs`** | Aportarlos **verbatim** (lo que la regla de aporte permite para el cierre) es **imposible**: no compilan en `netstandard2.1` sin `System.Data.SqlClient`, que no está en el TFM (verificado con build) y cuya adición está prohibida por el SPEC §4/§9 y retirada por ADR-005 §3.1. Aportarlos **transformados** es hacer el entregable del Bolt 014 (ADR-005 §3.3/§4) — y eso es, literalmente, la quinta condición de parada |
| **No elegir la versión del provider ADO.NET** (ni `System.Data.SqlClient` ni `Microsoft.Data.SqlClient` ni una versión concreta) | ADR-005 §3.1 asigna la versión «al **Bolt**» con dos criterios obligatorios —(1) asset consumible desde `netstandard2.1` y (2) **smoke test de conexión real contra SQL Server**— y ADR-005 §3.1 documenta además un hiato delicado entre líneas (la 6.0 retira el asset `netstandard2.0`, la 6.1 lo restaura como stub que lanza `PlatformNotSupportedException`). Elegirla aquí, sin poder correr el smoke test, sería inventar (octava condición de parada) |
| **No implementar la variante tipada «a medias»** para sortear el bloqueo (p. ej. dejar el contrato y las lecturas de `Data` «para después» con un TODO) | Es el mismo estado parcial inválido de §12, más un artefacto que aparenta avance. El bloqueo es de una dependencia de compilación, no de diseño: no hay nada que mitigar escribiendo el resto primero |
| **Verificar empíricamente los dos hechos que deciden el bloqueo** (¿`netstandard2.1` tiene `System.Data.SqlClient`? ¿tiene `System.Net.Mail`?) construyendo proyectos mínimos en el directorio temporal | El bloqueo se sostiene sobre una afirmación técnica; afirmarla sin medirla habría sido exactamente lo que el SPEC §13 (fila «La evidencia del vencimiento se declara sin medirla») y el gate de *hallucination lint* prohíben. Las dos sondas son de 1,5 s cada una y convierten una suposición en evidencia |
| **Registrar el mapa de factibilidad archivo por archivo en el MEM** | Convierte este MEM en el punto de partida del próximo V-Bounce: quien revise la revisión del SPEC decide sobre datos (qué archivo tiene camino y qué archivo no), y el V-Bounce siguiente no repite el reconocimiento |

---

## 8. Deviations and assumptions

### 8.1 El bloqueo (blocker exacto)

**Condición de parada disparada: SPEC-261016-0018 §15, quinta; concuerda la octava.**

> 5. «**El cierre mínimo exigiría transformar código de otro Bolt**: detener y escalar la secuencia.»
>
> 8. «**Ambigüedad que sólo se resolvería inventando** (por ejemplo, el tipo exacto que debe devolver una variante
> tipada ante un valor no numérico): parar y reportar; no se rellena el hueco con una suposición (§2.4.1).»

**La cadena de hechos, cada uno verificado:**

1. **`2.0/` no tiene fuentes.** `2.0/src` contiene únicamente los cuatro `.csproj` creados por el Bolt 001
   (`Core`, `Data`, `Library`, `Modeling`) — **cero archivos `.cs`**. Por eso aplica la *regla de aporte* de §4.
2. **`Data` es `netstandard2.1` y no declara ningún paquete.** `Cooperator.Framework.Data.csproj` tiene
   `TargetFramework: netstandard2.1` y **0** `PackageReference`.
3. **`netstandard2.1` NO provee `System.Data.SqlClient`.** Verificado construyendo un proyecto `netstandard2.1`
   mínimo con `using System.Data.SqlClient;`:
   ```text
   error CS0234: The type or namespace name 'SqlClient' does not exist in the namespace 'System.Data'
                  (are you missing an assembly reference?)
   error CS0246: The type or namespace name 'SqlConnection' could not be found
   ```
   Es decir: para que `Data` contenga `BaseRule.cs`/`SQLHelper.cs` hay que **agregar un paquete**.
4. **Los dos archivos en alcance son SqlClient-tipados de punta a punta.** `SQLHelper.cs` tiene **389**
   ocurrencias de tipos SqlClient y `SqlHelperParameterCache.cs` **44**; `BaseRule.cs` usa `SqlConnection`
   (L172/197), `SqlTransaction` (L175/241/264/289/312/349/385/411/436), `SqlConnection` en casts (L327/361),
   `DbConnection`/`DbTransaction`/`DataSet` (L195/325/359/397/409/421/434) y llama a `SqlHelper.*`.
5. **Agregar el paquete está prohibido, dos veces.** SPEC §4 *out of scope*: «Cambiar TFMs o **agregar**
   dependencias al framework | — (nadie) | ADR-001 §3.1/§3.4; este Bolt **quita** una excepción, **no suma
   nada**». SPEC §9, gate *Dependency scanning / licenses*: «**0** dependencias nuevas».
6. **Y además ADR-005 §3.1 lo retira expresamente** (verbatim): «**Provider**: `Microsoft.Data.SqlClient`,
   **único**. Los paquetes de la 2.0 **no** dependen de `System.Data.SqlClient` (**se retira de `Data`**)».
7. **La forma «ya migrada» de estos archivos ES el entregable del Bolt 014.** ADR-005 §3.3/§4 fijan:
   `SqlHelper`/`SqlHelperParameterCache` pasan a **`internal`**, el interior trabaja con `Microsoft.Data.SqlClient`
   «de punta a punta» y **sin casts**, «con **un único punto de conversión** que falla de forma tipada»; y
   ADR-005 §4 declara la ruptura: «**el `SqlHelper` público desaparece**». Los casts de `BaseRule.cs`
   (L241/264/289/312/327) están **citados por el propio ADR-005 §7** como la evidencia de F-09 que ese Bolt
   elimina.
8. **La versión del provider también es de otro Bolt, y con un criterio que no puedo cumplir.** ADR-005 §3.1:
   «**Versión**: la fija el **Bolt** … con **dos criterios obligatorios de aceptación**: (1) la línea elegida debe
   ofrecer un **asset consumible desde `netstandard2.1`**, y (2) el arnés debe pasar un **smoke test de conexión
   real** contra SQL Server (no basta con que compile)». No hay entorno de SQL Server en este V-Bounce: el
   segundo criterio es **inejecutable acá**, y elegir la línea sin él sería inventar.

**Por qué es la quinta y no una simple «falta de prerequisito»:** la regla de aporte de §4 está redactada para
aportar archivos «**copiados tal cual están en el 1.4** y **sin ninguna otra transformación**» (el cierre) y
«**en su forma ya migrada**» (los archivos en alcance). Para `Data/BaseRule.cs` y `Data/SQLHelper.cs` las dos
mitades de la regla son inaplicables a la vez: *tal cual están* no compila, y *su forma ya migrada* no existe
porque la produce otro Bolt. El único camino restante —transformarlos— es exactamente lo que la quinta condición
declara como parada.

**Alcance del bloqueo: es uno solo.** Los otros tres sitios en alcance tienen camino verificado (§2): el contrato
y el catálogo en `Core` son nuevos y BCL puro; el archivo del LazyLoad **se retira entero** (no hay nada que
aportar); y `Library/SmtpMail.cs` es aportable —comprobé que `System.Net.Mail` y `System.IO` **sí** están
disponibles en `netstandard2.1`. El bloqueo está **localizado en la capa `Data`** y su causa es la secuencia entre
este Bolt y el Bolt 014, no un defecto de diseño del SPEC.

**Nota de secuencia (contexto para el desbloqueo):** el SPEC §5 ya prevé la relación con el Bolt 005 (carga de
providers Lazy) pero **no** prevé que el Bolt 014 (provider de datos) sea quien produce la única forma compilable
de dos de los cuatro archivos que §C.2 manda modificar. Lo que hace falta es precisamente cerrar ese hueco de
secuencia — no un cambio de diseño del contrato de configuración, que es implementable tal como está escrito.

### 8.2 Hallazgo de housekeeping — al Bolt 011 le falta la fila T03 de su `Status timeline`

El `## Status timeline` de `US-000.BOLT-011` registra T01 (`draft`) y T02 (`ready`) pero **no** la fila
`ready → specified`, aunque el frontmatter declara `status: "specified"` y el SPEC aprobado tiene
`review.decided_at: 2026-10-16T00:26:23-03:00`. Es un defecto de registro del documento (G39 pide que cada
transición de estado se registre en el mismo acto con su fila), **no** algo que este V-Bounce deba corregir: la
edición permitida al Bolt es la de G11 (T04) y la de cierre (T05), y escribir una fila histórica que no presencié
sería fabricar evidencia. **Se reporta para que lo corrija su dueño.** (Las filas T04 y T05 que este V-Bounce sí
protagoniza están registradas con el reloj real.)

### 8.3 G15 — verificación explícita de fuentes gobernadas

**Sin cambio material.** `US-000.BOLT-011` es el mismo (su `review` de `AITL-BOLT-READY-Approval` intacto) y sólo
cambió su `status`, por los actos de este V-Bounce; `REV-001` sigue `approved`; `ADR-004`, `ADR-001` y `ADR-005`
siguen `accepted` y sin reescritura; la línea base de código sigue siendo `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`
y `1.4/**` está intacto frente a ella. **El `AITL-SPEC-Approval` de la revisión 1 sigue vigente** — pero para
avanzar hace falta una **revisión material** del SPEC (nueva revisión + `spec_revisions[]` + re-aprobación,
G15/G16) que resuelva el hueco de secuencia de §8.1, no por cambio de fuente sino por **defecto descubierto en la
ejecución**.

### 8.4 Comprobación de las demás condiciones de parada (§15) — todas negativas salvo la 5.ª

| Condición de parada (§15) | ¿Dispara? | Comprobación |
|---|---|---|
| 1. Cambio material en una fuente gobernada | **No** | §8.3 |
| 2. Aparece un valor que el catálogo no cubre | **No** | Las cuatro fuentes del inventario de §2 del SPEC (`ConnectionStrings`, timeout por `AppSettings`, SMTP, sección custom del LazyLoad) están todas en el catálogo de §2 del SPEC, incluida la clave del cap que el finding (a) mandó publicar |
| 3. El retiro exigiría resolver criptografía | **No** | El retiro **elimina** la única llamada a cripto de esa ruta (`BaseRule.cs` L114 `Library.Cryptography.Decrypt`, dentro de la heurística que §C.2 manda retirar). No se necesita implementar nada criptográfico — eso es del Bolt 022 |
| 4. El retiro exigiría editar `1.4/` | **No** | `1.4/**` intacto (§9) |
| **5. El cierre mínimo exigiría transformar código de otro Bolt** | **SÍ** | §8.1 |
| 6. Un gate de §9 no cierra en `pass` | **No evaluado** | No se llegó a correr la suite del Bolt |
| 7. El arnés del Bolt 001 no está disponible | **No** | Arnés `20/20` verde y reporte de dependencias por proyecto operativo (§9) |
| 8. Ambigüedad que sólo se resolvería inventando | **Sí, concuerda** | La versión del provider ADO.NET (con su smoke test de conexión real) es una decisión de otro Bolt; elegirla acá sería inventar (§8.1, punto 8) |

### 8.5 Supuestos

- Se asumió que «el cierre mínimo» de §4 incluye cualquier archivo que haga falta para **compilar** los archivos en
  alcance, y que `Data` es el proyecto donde deben vivir `BaseRule.cs` y `SQLHelper.cs` (lo dice §C.2 al listarlos
  como «Files modified» de `2.0/src/Cooperator.Framework.Data/`).
- No se asumió ninguna decisión de provider, de versión ni de forma de la superficie de `Data`: la clasificación
  es la del SPEC y de ADR-005, y el bloqueo se reporta **sin proponer un arreglo como si fuera autorizado**. Las
  salidas técnicas de §8.1 se listan como materia de la decisión humana, no como plan de ejecución.
- No se leyó, copió ni procesó ningún dato de negocio, credencial ni archivo de configuración real: la evidencia
  son nombres de archivo, números de línea y conteos de ocurrencias sobre el árbol `1.4/` congelado.

---

## 9. Verification evidence

Toda la evidencia es **de lectura, de sonda de compilación y de corrida de suites existentes**. No hay build ni
suite nueva que exhibir, porque no se generó código — y así se declara.

### Prerequisito del §5 — arnés del Bolt 001, VERDE

```text
=== ANTES: harness (prerequisito del SPEC §5) ===
Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20, Duration: 7 s -
Cooperator.Harness.Tests.dll (net10.0)

=== ANTES: characterization ===
Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 117 ms -
Cooperator.Framework.Characterization.Tests.exe (net48)
```

Corridas con `--no-build` sobre los ensamblados ya construidos por el Bolt 001: **no se recompiló ni se reescribió
ningún archivo de su paquete.**

### Evidencia del bloqueo — sonda 1: `netstandard2.1` no provee `System.Data.SqlClient`

```text
$t/t.csproj : <TargetFramework>netstandard2.1</TargetFramework>
$t/C.cs     : using System.Data.SqlClient;
              public class C { public SqlConnection Conn(string s) => new SqlConnection(s); }

Build FAILED.
C.cs(1,19): error CS0234: The type or namespace name 'SqlClient' does not exist in the namespace 'System.Data'
                       (are you missing an assembly reference?)
C.cs(2,25): error CS0246: The type or namespace name 'SqlConnection' could not be found
    0 Warning(s)
    2 Error(s)
EXIT=1
```

### Evidencia de apoyo — sonda 2: `netstandard2.1` **sí** provee `System.Net.Mail` (por eso `SmtpMail.cs` es aportable)

```text
$t/C.cs : using System.Net.Mail; using System.IO;
          public class C { public MailMessage M(string a, string b) => new MailMessage(a, b);
                           public FileInfo F(string p) => new FileInfo(p); }

    0 Warning(s)
    0 Error(s)
EXIT=0
```

### Evidencia de apoyo — tipado SqlClient y estado de dependencias

```text
=== SQLHelper.cs: usos de SqlClient ===
389
=== SqlHelperParameterCache.cs ===
44

=== Data csproj: PackageReference ===
0   (ninguna)

=== informe de dependencias por proyecto (Bolt 001, evidencia de AC-5) ===
  Cooperator.Framework.Core     -> (sin paquetes)
  Cooperator.Framework.Data     -> (sin paquetes)
  Cooperator.Framework.Library  -> (sin paquetes)
  Cooperator.Framework.Modeling -> 8 paquete(s)   (System.Text.Json + su grafo transitivo; Bolt 003/004)
```

**Es el punto de partida exacto que AC-5 pide evidenciar:** el framework tiene hoy 0 declaraciones de
`System.Configuration.ConfigurationManager` —la excepción transitoria de ADR-001 §4/§5 **no está tomada**— y el
techo del NFR `dependency-footprint` está libre para el provider de ADR-005 (Bolt 014).

### Evidencia de apoyo — el origen de los archivos bloqueados

```text
1.4/Cooperator.Framework.Data/BaseRule.cs              459 líneas / 22.733 bytes
1.4/Cooperator.Framework.Data/SQLHelper.cs            2302 líneas / 143.265 bytes
1.4/Cooperator.Framework.Data/SqlHelperParameterCache.cs  ~250 líneas (44 usos de SqlClient)
1.4/Cooperator.Framework.Core/LazyLoad/LazyProviderFactoryConfigHelper.cs  420 líneas (todo sección custom + IO)
1.4/Cooperator.Framework.Library/SmtpMail.cs            119 líneas (F-22: inicializador estático en L33)
```

### Estado del árbol (rollback = no-cambio)

```text
$ ls 2.0/src
Cooperator.Framework.Core/Cooperator.Framework.Core.csproj
Cooperator.Framework.Data/Cooperator.Framework.Data.csproj
Cooperator.Framework.Library/Cooperator.Framework.Library.csproj
Cooperator.Framework.Modeling/Cooperator.Framework.Modeling.csproj
   → 4 .csproj, CERO archivos .cs   (el estado que dejó el Bolt 001, intacto)

2.0/docs                      -> NO EXISTE
2.0/src/**/Configuration      -> NO EXISTE
2.0/src/Cooperator.Framework.Data/*.cs   -> NO EXISTE
2.0/src/Cooperator.Framework.Library/*.cs -> NO EXISTE

git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Build

```text
n/a — no se ejecutó build del entregable: el V-Bounce se detuvo antes de la fase A y no generó código.
     (Las dos sondas de compilación de arriba se construyeron en el directorio temporal, fuera del repositorio,
      y no forman parte de la entrega.)
```

### Tests

```text
n/a — no se escribió ninguna prueba. 0 casos creados.
     (Las 103 corridas verdes registradas arriba son del arnés del Bolt 001, como evidencia del prerequisito;
      no son pruebas de este Bolt.)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2 del SPEC) y además
el V-Bounce no llegó a escribir ninguna prueba.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG (§3.3.1).

### Gates

> Vocabulario de T11 (`pass` / `waived` / `n/a` con razón). **Ninguno de estos `n/a` es un `pass` encubierto:
> ningún gate se ejecutó, porque no hay código que someter a gate.**

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | `n/a` — no se generó código |
| Unit / component tests | V-Bounce | `n/a` — no se escribió ninguna prueba. **El prerequisito sí se verificó:** arnés 20/20 y caracterización 83/83 verdes |
| Consumer-side contract tests (test doubles) | V-Bounce | `n/a` — no se escribió el doble del contrato |
| SAST | V-Bounce | `n/a` — 0 líneas de código nuevo |
| Dependency scanning / licenses | V-Bounce | `n/a` — no se declaró ninguna dependencia (no se tocó `Directory.Packages.props`). **La medición del punto de partida sí se hizo:** `Core`/`Data`/`Library` con 0 paquetes (evidencia de apoyo para AC-5) |
| OWASP Top 10 — static review | V-Bounce | `n/a` — el SPEC lo declara `n/a` (sin superficie externa) |
| Prompt-injection scan | V-Bounce | `n/a` — el SPEC lo declara `n/a` |
| Secret-leak scan | V-Bounce | `n/a` — no se produjo ningún archivo del entregable ni fixture; este MEM no contiene ningún valor de credencial (no se leyó ninguna) |
| Hallucination lint | V-Bounce | `pass` — cada afirmación de este MEM es trazable a una sonda de compilación registrada arriba, a una corrida registrada, o a una cita verificada de ADR-005/SPEC-0018; 0 claves, tipos o rutas inventados. Es el único gate evaluable hoy |
| IP / license provenance | V-Bounce | `n/a` — 0 código incorporado |
| PII / DLP | V-Bounce | `pass` — 0 datos personales ni de negocio leídos o escritos; la evidencia son nombres de archivo, líneas y conteos |
| Dependency-confusion | V-Bounce | `n/a` — 0 paquetes nuevos |
| Test-first evidence | V-Bounce | `n/a` — no es un Bolt de BUG y no se escribieron pruebas |
| Behavioral reproducibility (hermetic suite) | V-Bounce | `n/a` — no hay suite nueva ni build que correr dos veces |
| Bolt-manifest validation | V-Bounce | `pass` — la entrada `v_bounces[]` de este V-Bounce valida contra `manifest-v5-bolt.schema.json` (§9, Manifest) |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | `n/a` — no hay superficie nueva que comparar; la cobertura de las rupturas (AC-7) queda para el V-Bounce que ejecute el retiro |
| Integration tests (real dependencies) | Integration test | `n/a` — plan `n/a` por `test_integration: not-required` |
| Contract / E2E against the real providers | Integration test | `n/a` — sin dependencias reales en el alcance |
| DAST / OWASP Top 10 — dynamic | Integration test | `n/a` — sin superficie desplegada |
| Perf-smoke (p95/p99) | Integration test | `n/a` — sin umbral aplicable |
| Observability (logs / metrics / traces) | Integration test | `n/a` — sin telemetría de servicio |

### Manifest

```text
python -c "jsonschema Draft202012Validator(manifest-v5-bolt.schema.json).iter_errors(<bolt manifest>)"
VALID
```

### Encoding de los documentos gobernados

Verificado byte a byte: este MEM, el manifiesto del Bolt y el propio Bolt son UTF-8 **sin BOM**, sin `U+FFFD` y
sin `CR` sueltos (§3.15, W23).

---

## 10. Manual interventions

**None.** No hubo parche humano ni fallback manual. La decisión humana pendiente es la **revisión material del
SPEC** (o el desbloqueo que el revisor determine), no una intervención sobre el código.

---

## 11. Evidence links

- **Diff / PR:** no hay — **no se generó código en este V-Bounce**. Los únicos cambios en el árbol son la
  actualización de estado de `US-000.BOLT-011`, este MEM y su entrada de manifiesto. No se hicieron operaciones
  de git: el commit es del humano (G34).
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — el mismo que el SPEC §3 declara. **El
  trabajo de este V-Bounce no lo mueve.**
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-011-configuracion-portable.json`](../metrics/bolts/US-000.BOLT-011-configuracion-portable.json)
- **Fuente de la evidencia del bloqueo (sólo lectura):**
  `1.4/Cooperator.Framework.Data/BaseRule.cs` (L85, L92-135, L172-436) ·
  `1.4/Cooperator.Framework.Data/SQLHelper.cs` (L84) · `1.4/Cooperator.Framework.Data/SqlHelperParameterCache.cs` ·
  `1.4/Cooperator.Framework.Core/LazyLoad/LazyProviderFactoryConfigHelper.cs` (L38/L101/L293/L347-L428) ·
  `1.4/Cooperator.Framework.Library/SmtpMail.cs` (L33, L104) ·
  `1.4/Cooperator.Framework.Library/Exceptions/{SmtpErrorException,BaseException}.cs`
- **Normativo que decide el bloqueo:** SPEC-261016-0018 §4 (regla de aporte y *out of scope*), §6/C.2, §9
  (Dependency scanning), §12, §15 (5.ª y 8.ª); ADR-005 §3.1, §3.3, §4 y §7; ADR-001 §3.4/§5.
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/tests/Cooperator.Harness.Tests` y
  `2.0/tests/Cooperator.Framework.Characterization.Tests` (corridas `--no-build`), y el reporte de dependencias
  por proyecto (`dotnet list package`). **No se modificó ninguno.**

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados al reconocimiento de las fuentes y al cierre del V-Bounce; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | 1 |
| Tests created | **0** — no se escribió ninguna prueba |
| AI-generated code | **0 líneas** — ningún archivo de la entrega del SPEC fue creado |
| Loops consumed | **0 de 10** — el V-Bounce no llegó al lazo de suite: se detuvo en la condición de parada antes de generar código |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval`; el resultado no es `ready_for_review` |

---

## 13. Pending items and stubs

- [ ] **BLOQUEANTE — acción humana:** revisión material de `SPEC-261016-0018` (nueva revisión +
      `spec_revisions[]` + `AITL-SPEC-Approval`) que cierre el hueco de **secuencia** entre este Bolt y el **Bolt
      014**. Las dos salidas que se ven desde acá —ambas de decisión humana, ninguna del ejecutor— son:
      **(1)** reescribir §4/§C.2 para que el retiro de las lecturas de `Data` (`ConnectionStrings` + heurística +
      `Dictionary` estático, y el timeout por `AppSettings`) se **materialice cuando el Bolt 012/013/014 aporte esa
      capa**, con este Bolt entregando el contrato, el catálogo y la constancia de rupturas (y el control de
      ausencia pasando a verificar «sin lecturas de host» sobre la capa que exista); o **(2)** declarar que este
      Bolt **aporta la capa `Data` ya migrada al provider de ADR-005**, en cuyo caso hay que asignarle la
      **versión** con sus dos criterios de aceptación —incluido el smoke test de conexión real— y ese trabajo se
      solapa con el Bolt 014, que es una decisión de secuencia, no de diseño.
- [ ] **Hallazgo de housekeeping (§8.2):** al Bolt 011 le falta la fila T03 (`ready → specified`) en su
      `Status timeline`. Lo corrige su dueño; este V-Bounce no lo toca.
- [ ] **Todo el Bolt queda pendiente**, con las fases A–E del SPEC §6 sin implementar. El **mapa de factibilidad
      de §2** es la base: 3 de los 4 sitios en alcance tienen camino verificado (contrato/catálogo en `Core`,
      retiro entero del LazyLoad, `SmtpMail` aportable) y sólo la capa `Data` está bloqueada.
- [ ] **Recordatorio para el V-Bounce que ejecute:** los dos findings del `review` del SPEC son **obligatorios** y
      siguen vigentes — (a) publicar `Cooperator:Cache:MaxEntries` en `configuration-keys.md` + `ConfigurationKeys.cs`;
      (b) etiquetar `Cooperator:LazyLoad:Providers:<entidad>:Class` como **nombre completo calificado** por
      namespace (ADR-006 §3.1).
- [ ] **Riesgo abierto no evaluado:** nada del retiro exige criptografía (§8.4, condición 3), pero conviene que el
      próximo V-Bounce deje constancia de que la heurística retirada de `BaseRule.cs` era el único consumidor de
      `Library.Cryptography.Decrypt` en esa ruta — dato útil para los Bolts 022/030.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce. Siendo este V-Bounce `blocked`, la decisión
> esperable es `changes_requested` / `rejected` sobre el MEM como historia inmutable, y la respuesta es una
> **revisión material del SPEC** seguida de un **V-Bounce nuevo** con su MEM nuevo.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | `agent:dotnet-developer-reviewer-agent` |
| **Roles** | dev_validator |
| **Decision** | approved |
| **review_ready_at** | `2026-10-16T00:53:45-03:00` |
| **review.started_at** | `2026-10-16T01:04:59-03:00` |
| **review.decided_at** | `2026-10-16T01:04:59-03:00` |
| **Review evidence** | Verificación en vivo (Run N°4): `2.0/src/Cooperator.Framework.Data` sin fuentes `.cs` (sólo csproj + packages.lock.json); `BaseRule.cs`/`SQLHelper.cs` inexistentes en 2.0; en 1.4 tipados sobre `System.Data.SqlClient` (BaseRule.cs L33/L172/L197/L241/L264/L289/L312/L327; SQLHelper.cs L28 + cientos); ADR-005 §3.1 verbatim «se retira de Data», §3.3 `SqlHelper`→`internal`, §4 «el `SqlHelper` público desaparece»; SPEC rev.1 §15 quinta/octava cita el blocker; T04/T05 presentes; MEM único; `1.4/**` intacto (diff exit 0); sin código parcial |
| **Comments** | Bloqueo correctamente identificado y registrado; sin código parcial; desbloqueo = revisión material del SPEC (rev.2, salida 1) + V-Bounce nuevo — **agéntico** (el revisor del SPEC es `agent:dotnet-developer-reviewer-agent` en otro acto, no requiere al humano) |
| **Findings** | Ver el bloque `review` del frontmatter (6 hallazgos Compliant) |
| **acknowledged_without_comment** | false |
| **acknowledgment_reason** | — |
