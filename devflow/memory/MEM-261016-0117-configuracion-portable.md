---
id: "MEM-261016-0117"
title: "Configuración portable del framework: contrato en `Core`, default por entorno, retiro en `Core`/`Library` y constancia"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-011" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0018" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 2 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-004", "ADR-001"] # governing ADRs (ADR-005 y ADR-007 consultados: gobiernan lo diferido y el seam)
manifest: "US-000.BOLT-011-configuracion-portable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T01:17:34-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T01:22:32-03:00"
  decided_at: "2026-10-16T01:22:32-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°2 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0018` revisión 2 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; grant del revisor `approves: [SPEC, MEM]` (dotnet-developer-reviewer-agent.yaml). Ejecutor (`dotnet-developer-agent`) ≠ revisor (independencia)."
    - "Compliant — evidencia REPRODUCIDA: corrida `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **49/49 harness + 83/83 caracterización, exit 0** (13 proyectos sin warnings). Entregables clave verificados en el árbol: `Core/Configuration/` con los 7 archivos (ICooperatorConfiguration, CooperatorSettings, ConfigurationKeys, CooperatorConfigurationException, EnvironmentConfiguration, ConnectionStringValue, IProtectedValueResolver); `api-removals/US-000.BOLT-011.json` con **9 entradas + 3 `deferred-to-layer` (BOLT011-05/06/07) + `OBLIGATION-DATA-01`** (owner_bolts 012/013/014, materially 014, cross_references 0021/0019/0020); `configuration-keys.md`; `MaxEntries` publicada (ConfigurationKeys.cs L80); `SmtpMail` sin `SendMailViaCDONTS` (sólo la mención en doc-comment) ni estático; `IProtectedValueResolver` + `ConnectionStringValue.Resolve` + `IdentityResolver` + `GetConnectionString`."
    - "ADJUDICACIÓN (cifra del cap) — ACEPTADA COMO ESTÁ (diferida a su dueño por escrito): ningún ADR/SPEC fija una cifra (ADR-004 §3.1 exige el cap «configurable» sin cuantificar; §5 fija la **propiedad** no el valor); `SPEC-0019` §15 atribuye la decisión al Bolt 013. La clave, su tipo (`entero`) y su dueño quedaron publicados y visibles (`configuration-keys.md` L35/§2.1: «la cifra la fija el Bolt 013»). Publicar una cifra habría sido inventar (§15 #8). Postura registrada: **no es un blocker; se acepta sin cifra**."
    - "Compliant — desviaciones declaradas aceptables: `ProjectReference` `Library→Core` (permitido por ADR-001 §3.1 arco `L→C`, exigido por el SPEC §3.2/§6/C.3); `ProjectReference` arnés→Core/Library (mecánico para ejercitar el contrato); `ConfigurationCollection.cs` (colección xUnit que serializa el estado ambiental y evita verde *flaky*); comentarios reescritos + control de declaraciones quirúrgico (evita falsos positivos sobre el comentario de `Data.csproj`). Todas bien declaradas en el MEM §8.2/§8.3."
    - "Compliant — AC-5 medido y findings ejecutados: `dotnet list package` deja Core/Data/Library en **0 paquetes** (el techo de `Data` es 1, el provider del Bolt 014) y Modeling con `System.Text.Json` — excepción transitoria de ADR-001 §4/§5 **vencida**. Los dos findings obligatorios: (a) `Cooperator:Cache:MaxEntries` publicada; (b) `Cooperator:LazyLoad:Providers:<entidad>:Class` etiquetada como **nombre completo calificado** (ejemplo `Ventas.LazyProviders.CustomerLazyProvider`). DoD: herméticos verdes, gates aplicables `pass`, MEM completo, manifiesto válido."
    - "Compliant — housekeeping (no bloquea): el `Status timeline` del Bolt quedó **fuera de orden cronológico** (T03 al final) — es de su dueño, no del V-Bounce; las filas están todas. No lo corrijo en este acto (la única edición al Bolt es la transición de estado y su fila T07)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0117 — Configuración portable: contrato en `Core`, default por entorno, retiro en `Core`/`Library` y constancia

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-011](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) |
| **SPEC**        | [SPEC-261016-0018](../spec/SPEC-261016-0018-configuracion-portable.md), **revisión 2** (aprobada) |
| **V-Bounce**    | **2** (el N°1 está en [`MEM-261016-0053`](MEM-261016-0053-configuracion-portable.md), `blocked`) |
| **ADRs**        | [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-005](../adrs/ADR-005-provider-de-datos.md) · [ADR-007](../adrs/ADR-007-politica-criptografica.md) |

> **V-Bounce NUEVO** (no un retry): timestamp nuevo, MEM nuevo, entrada `v_bounces[]` número 2.

---

## 1. Executive summary

Se entregó la **configuración portable del framework**, con el alcance dividido que fijó la revisión 2. En
`Cooperator.Framework.Core` viven el **contrato** (`ICooperatorConfiguration`: lectura por clave, variantes
tipadas y `GetConnectionString(name)`), la **instancia ambiental** (`CooperatorSettings.Current`, fijable por la
app y **reemplazable en caliente** — que es la operación documentada desde que se eliminó la caché estática), la
**implementación por defecto sólo-entorno** (`Cooperator__Seccion__Clave`, valor vacío = ausente), la **excepción
propia del framework** (`CooperatorConfigurationException`, con la clave y el prefijo ofensor en el mensaje), la
**forma del valor de conexión** (`ConnectionStringValue`: prefijo obligatorio, `plain:` = identidad, sin
adivinar ni descifrar a ciegas) y el **seam** `IProtectedValueResolver`. El **catálogo de claves** quedó publicado
como constantes **y** como documento versionado (`2.0/docs/configuration-keys.md`), con los dos findings
obligatorios del `AITL-SPEC-Approval` ejecutados: la clave **`Cooperator:Cache:MaxEntries`** está publicada —con
su tipo— en ambos lugares, y `Cooperator:LazyLoad:Providers:<entidad>:Class` quedó etiquetada como **nombre
completo calificado** (ADR-006 §3.1), corrigiendo el ejemplo sin calificar de ADR-004 §3.4. En
`Cooperator.Framework.Library`, `SmtpMail` pasó a leer el servidor del catálogo de forma **perezosa y validada**
—sin el inicializador estático que reventaba con `NullReferenceException`— y perdió el método muerto
`SendMailViaCDONTS`. La **constancia de rupturas** quedó en `2.0/docs/api-removals/US-000.BOLT-011.json` con las
nueve entradas del cuadro §6/E.2 y, sobre todo, con la **obligación registrada** de la capa `Data` (0 lecturas de
configuración del host) con sus dueños (Bolts 012/013/014) y su **referencia cruzada** a `SPEC-261016-0019` y
`SPEC-261016-0020`. El resultado es **verde y reproducible**: la solución 2.0 compila **sin errores ni
advertencias**, el arnés pasa **49/49** (los 20 del Bolt 001 + **29 casos nuevos**), la caracterización sigue
**83/83**, y el reporte de dependencias deja `Core`/`Data`/`Library` con **0 paquetes** —el umbral del NFR
`dependency-footprint` en su valor declarado y la excepción transitoria de ADR-001 §4/§5 **vencida**—. Hubo una
sola decisión que **no** tomé: la **cifra** del cap de caché, que ningún ADR ni SPEC decide y que publicar habría
sido rellenar un hueco con una suposición (§15 #2/#8); la clave y su tipo quedaron publicados y la cifra
**declarada como del Bolt 013**, dueño de la caché que la pidió. Se declaran también las tres adiciones
mecánicas que el SPEC no listaba literalmente (dos `ProjectReference` y una del arnés) y una observación de
housekeeping sobre el timeline del Bolt.

---

## 2. Implemented phases

### Phase A — El contrato y el catálogo en `Core`

`ICooperatorConfiguration` declara exactamente lo que ADR-004 §3.2 permite: **valores nombrados** —
`GetString(key)`, `GetString(key, default)`, `GetInt(key)`, `GetInt(key, default)` y `GetConnectionString(name)`—
sin conocer archivos, rutas ni formatos. `CooperatorSettings.Current` es la **instancia ambiental**: se crea de
forma perezosa (si nadie la fijó, es la implementación por defecto) y su setter **reemplaza** la instancia, que es
la vía documentada para cambiar la configuración en caliente; fijar `null` es un `ArgumentNullException`.
`ConfigurationKeys` publica las claves como constantes —incluida `Cache.MaxEntries`— con helpers
(`ConnectionStrings.For`, `LazyLoad.AssemblyFor/ClassFor`) y los **defaults documentados**
(`Defaults.CommandTimeoutSeconds = 30`, `CacheDefaultTtlSeconds = 3600`, `CacheParameterMetadataTtlSeconds = 86400`),
tomados de ADR-004 §3.1/§3.2 y de 1.4 donde existían. `2.0/docs/configuration-keys.md` publica el catálogo con
tipo, default y procedencia de cada clave, la convención de la fuente por defecto, la forma del valor de conexión,
el etiquetado calificado del provider Lazy y lo que **no** es una clave del catálogo.

### Phase B — La implementación por defecto: sólo el entorno

`EnvironmentConfiguration` es BCL puro: lee **sólo variables de entorno** con la jerarquía en doble guion bajo
(`ConfigurationKeys.SectionSeparator` → `EnvironmentSectionSeparator`), trata el **valor vacío como ausente**,
devuelve el **default documentado** cuando la clave lo tiene y falla con `CooperatorConfigurationException`
—nombrando la clave— cuando no lo tiene. No lee archivos, no define precedencias y no inventa un orden de
búsqueda: la precedencia entorno > archivo > default es del bootstrap del Bolt 021.

### Phase C — Retiro de la configuración del host

**C.1 (`Core`)** — la sección custom del LazyLoad **y su escritura** desaparecen: `1.4/.../LazyProviderFactoryConfigHelper.cs`
(420 líneas) **no se aporta a `2.0/`** porque *todo* su contenido es la sección (`Provider`, `ProvidersCollection`,
`ProvidersSection`) y su IO (`Read`, `CreateEmpty`, `cfg.Save`, `OpenMappedExeConfiguration`). No quedó un `using
System.Configuration;` huérfano porque no quedó archivo. **C.3 (`Library`)** — `SmtpMail` lee
`Cooperator:Mail:SmtpServer` **en el punto de uso** (`SmtpServerName`), sin inicializador estático, y su ausencia
produce el error tipado del framework (la lectura ocurre **fuera** del `try` que envuelve el envío, para que el
error tipado no quede enmascarado como error de correo). **C.2 (`Data`)** — es la parte **diferida**: la
obligación quedó registrada (§6 del SPEC, AC-4/AC-5) y este V-Bounce **no** editó código de otro Bolt.

### Phase D — Forma del valor, errores tipados y seam

`ConnectionStringValue.Parse(key, raw)` valida la **forma** sin heurística: un valor vacío es una clave ausente,
un valor **sin** prefijo se **rechaza** (el mensaje pide `plain:` explícito) y un **prefijo desconocido** se
rechaza nombrando la clave **y el prefijo ofensor**. `Resolve(resolver)` delega en el resolutor del esquema: para
`plain:` usa el de identidad y para un esquema conocido sin resolutor **falla tipado** (jamás «a ver qué pasa»).
`IProtectedValueResolver` es el seam que consume la capa que usa el valor (ADR-004 §3.3, ADR-007 §3); el Bolt 022
no lo había declarado, así que se declaró su forma mínima.

### Phase E — Vencimiento y constancia

`2.0/docs/api-removals/US-000.BOLT-011.json` lleva **nueve entradas** (qué / por qué / con qué se reemplaza /
estado / evidencia), con las **tres diferidas a la capa `Data`** (fuente de la cadena de conexión, fuente del
timeout y desaparición del `Dictionary` estático) marcadas `deferred-to-layer` con sus dueños y su referencia
cruzada; y la **obligación registrada** `OBLIGATION-DATA-01`. El vencimiento de la excepción transitoria se mide
en §9 sobre **la capa que existe**.

---

## 3. Files created

| File | Purpose |
|------|---------|
| `2.0/src/Cooperator.Framework.Core/Configuration/ICooperatorConfiguration.cs` | El contrato de valores nombrados: lectura por clave, variantes tipadas y `GetConnectionString(name)`, sin conocer archivos ni formatos (ADR-004 §3.2) |
| `2.0/src/Cooperator.Framework.Core/Configuration/CooperatorSettings.cs` | La instancia ambiental: perezosa por defecto y reemplazable, que es la operación documentada de cambio en caliente |
| `2.0/src/Cooperator.Framework.Core/Configuration/ConfigurationKeys.cs` | El catálogo como constantes (con los defaults documentados), para que ningún nombre ni default viva escondido en una implementación |
| `2.0/src/Cooperator.Framework.Core/Configuration/CooperatorConfigurationException.cs` | La excepción **propia** del framework: nunca un tipo del stack clásico ni un `NullReferenceException`; sus factory methods arman el mensaje con la clave y el valor |
| `2.0/src/Cooperator.Framework.Core/Configuration/EnvironmentConfiguration.cs` | La implementación por defecto: sólo variables de entorno, valor vacío = ausente, default documentado o error tipado |
| `2.0/src/Cooperator.Framework.Core/Configuration/ConnectionStringValue.cs` | La forma del valor de conexión: prefijo obligatorio, `plain:` = identidad, rechazo nombrado del prefijo ofensor, y el resolutor de identidad |
| `2.0/src/Cooperator.Framework.Core/Configuration/IProtectedValueResolver.cs` | El seam hacia el resolutor: la capa que consume el valor es quien lo invoca con el esquema y la carga (ADR-007 §3) |
| `2.0/src/Cooperator.Framework.Library/SmtpMail.cs` | El envío de correo del framework con el servidor leído **perezosamente y validado** del catálogo; sin `SendMailViaCDONTS` (código muerto con `System.Web.HttpContext` en la firma) |
| `2.0/src/Cooperator.Framework.Library/Exceptions/BaseException.cs` | **Cierre mínimo**, copiado tal cual del 1.4: base de las excepciones de `Library` (su `[Serializable]`/`SerializationInfo` **no** se retira acá: es del Bolt 007) |
| `2.0/src/Cooperator.Framework.Library/Exceptions/SmtpErrorException.cs` | **Cierre mínimo**, copiado tal cual: la excepción que `SmtpMail` lanza cuando el envío falla |
| `2.0/docs/configuration-keys.md` | El catálogo publicado y versionado: claves, tipos, defaults, procedencia de 1.4, la forma del valor, el etiquetado calificado del provider Lazy y la clave del cap con su cifra declarada como del Bolt 013 |
| `2.0/docs/api-removals/US-000.BOLT-011.json` | La constancia de rupturas (insumo del Bolt 028) y las obligaciones registradas de la capa `Data` con su referencia cruzada |
| `2.0/tests/Cooperator.Harness.Tests/ConfigurationContractTests.cs` | El contrato ejercitado como consumidor: claves y defaults, convención `__`, valor vacío, los **cuatro casos negativos canónicos**, la forma del valor, el seam y el doble de prueba del contrato |
| `2.0/tests/Cooperator.Harness.Tests/HostConfigurationAbsenceTests.cs` | El control de ausencia **sobre la capa que exista**, la ausencia de declaraciones del paquete clásico, la desaparición de la sección custom, la carga del tipo `SmtpMail` sin inicializador que explote y la verificación de la constancia y de la obligación registrada |
| `2.0/tests/Cooperator.Harness.Tests/ConfigurationCollection.cs` | Definición de la colección que serializa los casos que tocan estado ambiental (`CooperatorSettings.Current` y variables de entorno), para que no compitan entre sí |

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `2.0/src/Cooperator.Framework.Library/Cooperator.Framework.Library.csproj` | Se agrega el `ProjectReference` a `Core` (`TargetFramework` sin cambios). Es la arista `L → C` que ADR-001 §3.1 permite y que el SPEC §3.2/§6/C.3 exigen para que `Library` **consuma** el contrato: sin ella el tipo no resuelve |
| `2.0/tests/Cooperator.Harness.Tests/Cooperator.Harness.Tests.csproj` | Se agregan los `ProjectReference` a `Core` y a `Library` (el arnés no referenciaba nada del framework). Son la vía mecánica de los casos del contrato y de la carga de `SmtpMail` que el SPEC §6/E lista en este proyecto |
| `devflow/functional/bolts/US-000.BOLT-011-configuracion-portable.md` | G11: `status: "blocked"` → `"in-development"` + fila **T06** del `Status timeline` (toma del V-Bounce N°2). Es la única edición al Bolt |
| `devflow/metrics/bolts/US-000.BOLT-011-configuracion-portable.json` | Entrada `v_bounces[]` número 2, `spec_revision: 2`, `execution_outcome: "ready_for_review"` |

**Ningún archivo de los Bolts 001/002/003/006 fue tocado** (sólo se leyeron sus artefactos; sus suites se
corrieron con `--no-build`). **Ninguna línea de `1.4/**` fue modificada.** No se tocó ningún otro Bolt, SPEC,
manifiesto, índice ni registro de corrida.

## 5. Files renamed

| File | New name | Reason |
|------|----------|--------|
| — | — | No hubo renombres. |

## 6. Files deleted

| File | Reason |
|------|--------|
| — | No hubo borrados: la sección custom del LazyLoad no se aportó (no había nada que retirar en `2.0/`). |

---

## 7. Implementation decisions

| Decision | Reason |
|----------|--------|
| El contrato y la excepción viven en `Core` y la implementación por defecto también | ADR-004 §3.2: capa base, cero dependencias de terceros, consumida por `Data` y `Library`. Ponerlo en `Data` obligaría a `Library` a depender de `Data` (arista prohibida) |
| El valor vacío del entorno se trata como **ausente** | Un `""` no es un valor válido: devolverlo sería un valor silencioso, que es lo que §6/B.2 prohíbe. Así, el default documentado se aplica y la clave obligatoria falla tipado |
| La lectura del servidor SMTP ocurre **fuera** del `try` que envuelve el envío | Si ocurriera dentro, el `catch` la convertiría en `SmtpErrorException` y AC-6 («su ausencia produce el error tipado») no se cumpliría: el consumidor vería un error de correo donde hubo un error de configuración |
| El valor de conexión se valida en `Parse` y se entrega **crudo con su prefijo** | ADR-004 §3.3: la *forma* la valida el contrato; quien *resuelve* es la capa que consume el valor (el seam no se ejecuta en la configuración) |
| El esquema desconocido **falla** en vez de intentar resolverlo | §6/D.3 y ADR-004 §3.3: nunca «a ver qué pasa», ni descifrado a ciegas, ni fallback silencioso |
| El `Dictionary` estático **no** reaparece en el contrato | ADR-004 §3.3: su eliminación es lo que permite el cambio en caliente; el contrato sólo lee, y el reemplazo de la instancia ambiental es la operación documentada (probada en `ConfigurationContractTests`) |
| El control de ausencia escanea **fuentes, proyectos y la capa que exista**, y no una lista fija de archivos | Es lo que hace que el caso alcance a las fuentes de `Data` **solas** cuando se aporten: la obligación registrada se verifica en el acto, sin que nadie tenga que acordarse de ampliar el barrido |
| Los dos `ProjectReference` del arnés y el de `Library` se agregan aunque el SPEC no los liste literalmente | Sin ellos el código ordenado **no compila** (`Library` no puede consumir el contrato, el arnés no puede ejercitarlo). Son mecánicos, están permitidos por ADR-001 §3.1 (`L → C`) y se declaran acá |
| La **cifra** del cap de caché **no** se publica | Ver §8.1: ningún ADR ni SPEC decide un número, y publicarlo sería rellenar el hueco con una suposición (§15 #2/#8). La clave, su tipo y su dueño quedan publicados y visibles |

---

## 8. Deviations and assumptions

### 8.1 La cifra de `Cooperator:Cache:MaxEntries` — clave publicada, valor declarado como del Bolt 013

La obligación (a) del `AITL-SPEC-Approval` —«publicar la clave concreta del cap en `configuration-keys.md` +
`ConfigurationKeys.cs` … de modo que el 013 pueda tomar el cap del catálogo **sin esconder el default**»— está
**ejecutada**: `ConfigurationKeys.Cache.MaxEntries` existe (con su comentario y su tipo `entero`) y el catálogo la
publica en su tabla. Lo que **no** hice fue publicar una **cifra**, y conviene que el revisor lo vea:

- **Ningún ADR ni SPEC decide un número.** ADR-004 §3.1 exige el cap «**configurable**» con desalojo, pero **no lo
  cuantifica**; el catálogo de §3.2 y el ejemplo de §3.4 no lo traen; ADR-004 §5 (NFR *Cache memory bound*) fija
  la **propiedad** («con más entradas que el cap, el total nunca lo supera») y no el valor; y un barrido del corpus
  gobernado no encuentra cifra alguna (0 coincidencias).
- **`SPEC-261016-0019` §15 dice de quién es la decisión:** «si el catálogo no publica una clave para un default,
  la salida es **pedirla al Bolt 011** (§15), nunca esconderla acá». Es decir: el Bolt 013 **pide la clave** a
  este Bolt (que es lo que se entregó) y publica la **cifra** al aportar la caché, que es su entregable.
- **Publicar un número sería inventar.** §15 #8 de este SPEC: «Ambigüedad que sólo se resolvería inventando …
  parar y reportar; no se rellena el hueco con una suposición». Un valor como «10.000» no lo ordena ninguna
  decisión: sería una cifra de conveniencia, y además el NFR se mediría contra ella sin que nadie la haya firmado.

**Qué se entrega en su lugar, para que nada quede escondido:** la clave, su **tipo**, su dueño declarado, y la
constancia del hueco —en el comentario de `ConfigurationKeys.Cache.MaxEntries`, en la tabla y en §2.1 de
`2.0/docs/configuration-keys.md`—. Si el revisor prefiere una cifra concreta en este V-Bounce, es una línea en dos
archivos y no toca el contrato. **No es un blocker**: la clave está publicada, el tipo está publicado y la
procedencia del valor está declarada.

### 8.2 Adiciones mecánicas que el SPEC no listaba literalmente

| Adición | Por qué no es alcance inventado |
|---|---|
| `ProjectReference` **`Library` → `Core`** | ADR-001 §3.1 lo permite expresamente (arco `L → C` del diagrama de direcciones) y el SPEC §3.2/§6/C.3 ordenan que `Library` **consuma** el contrato: sin la referencia, `SmtpMail` no puede leer el catálogo y el cambio ordenado no compila |
| `ProjectReference` **arnés → `Core`** y **arnés → `Library`** | El arnés no referenciaba nada del framework. El SPEC §6/E lista los dos archivos de pruebas **en ese proyecto**, y ejercitar el contrato o cargar `SmtpMail` (AC-1, AC-6) exige la referencia |
| Archivo **`ConfigurationCollection.cs`** (colección de xUnit) | Los casos que tocan `CooperatorSettings.Current` y las variables de entorno comparten estado **de proceso**: xUnit paraleliza por clase, así que sin la colección podrían competir entre sí y volver el verde *flaky*. Es infraestructura del arnés, no superficie de producto |

### 8.3 El control de ausencia mira **usos**, y su versión estricta obligó a dos retoques de comentario

El control escanea el texto de `2.0/src/**/*.cs`, de modo que una **mención en un comentario** cuenta como
ocurrencia. Es la lectura fuerte de §8.1 («0 ocurrencias») y la mantengo: dos comentarios XML de
`ConfigurationKeys.cs` nombraban la entrada de ajustes clásica al documentar la procedencia de 1.4, y los
reescribí para describir el origen **sin** nombrar esa API. La procedencia **no** se pierde: está en
`2.0/docs/configuration-keys.md` y en la constancia, que no son código.

En cambio, para las **declaraciones** del paquete clásico el control es deliberadamente quirúrgico: mira sólo
elementos `<PackageReference>` / `<Reference>`, porque `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj`
menciona `System.Configuration.ConfigurationManager` **en un comentario** que explica que ese paquete **no**
entra. Un barrido por texto lo habría marcado como falso positivo.

### 8.4 La sección custom del LazyLoad se retira **por omisión**, no editando

El SPEC §6/C.1 dice «se retiran de `Core` la sección custom … y el helper» y lista
`LazyLoad/LazyProviderFactoryConfigHelper.cs` como *file modified*. En `2.0/` ese archivo **no existe**, y la
regla de aporte acotada por la revisión 2 (§4) dice que se aporta sólo lo que consiste en **quitar** una lectura:
acá el retiro es **total** (las 420 líneas son la sección y su IO), así que lo correcto es **no aportarlo**. La
constancia lo declara (`BOLT011-02`) y el control de ausencia verifica que no quedó ningún tipo ni `using` de esa
familia.

### 8.5 G15 — verificación explícita de fuentes gobernadas

**Sin cambio material.** `US-000.BOLT-011` es el mismo (su `review` de `AITL-BOLT-READY-Approval` intacto) y sólo
cambió su `status` por los actos de este V-Bounce; `REV-001` sigue `approved`; `ADR-001`/`ADR-004` siguen
`accepted` y sin reescritura; la línea base de código sigue siendo `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y
`1.4/**` está intacto frente a ella. **La `AITL-SPEC-Approval` de la revisión 2 es la que gobierna este V-Bounce.**

### 8.6 Comprobación de las condiciones de parada (§15) — todas negativas

| Condición de parada | ¿Dispara? | Comprobación |
|---|---|---|
| 1. Cambio material en una fuente gobernada | **No** | §8.5 |
| 2. Un valor que el catálogo no cubre / una clave sin decisión que la ordene | **No** | Las cuatro fuentes del inventario de 1.4 están cubiertas y la clave del cap **tiene** decisión que la ordena (el finding (a) + ADR-004 §3.1). Sobre su **cifra**, ver §8.1: la decisión existe pero es del Bolt 013, y no se inventa |
| 3. El retiro exigiría resolver criptografía | **No** | Se fija la **forma** del valor y el seam; el único esquema registrado es `plain:` (identidad). `aesgcm:` es del Bolt 022 y hoy **falla tipado**, que es lo ordenado |
| 4. El retiro exigiría editar `1.4/` | **No** | `1.4/**` intacto (§9); todo el trabajo se hizo en `2.0/` y las fuentes del 1.4 se **copiaron** como cierre |
| 5. El cierre mínimo exigiría transformar código de otro Bolt | **No** | El cierre se copió **tal cual** (`BaseException`, `SmtpErrorException`); los archivos de `Data` quedaron **fuera** del aporte por decisión de la revisión 2, y no se tocaron |
| 6. Un gate de §9 no cierra en `pass` | **No** | §9: todos los aplicables en `pass`; los `n/a` con su razón |
| 7. El arnés del Bolt 001 no está disponible | **No** | Comando reproducible, línea base, gate y suites operativos (49/49 y 83/83) |
| 8. Ambigüedad que sólo se resolvería inventando | **Parcialmente** | Ver §8.1: no resolví el hueco inventando; lo **declaré**. Si el revisor considera que la cifra debía publicarse igual, es una línea en dos archivos |
| 9. La capa `Data` aparece con una lectura de configuración del host | **No** | `2.0/src/Cooperator.Framework.Data` **no tiene fuentes**; el control de ausencia lo confirma y nombrará el caso cuando aparezcan |

### 8.7 Supuestos

- Se asumió que `Cooperator:Cache:MaxEntries` es la forma correcta del nombre (lo dice el finding aprobado) y que
  su tipo es **entero** (es un número de entradas).
- Se asumió que el default de `CommandTimeoutSeconds` es **30 s**, que es lo que 1.4 devolvía cuando la entrada no
  estaba definida (`new SqlCommand().CommandTimeout`).
- No se asumió ninguna decisión de otro Bolt: ni la cifra del cap (Bolt 013), ni el provider ADO.NET y su versión
  (Bolt 014, ADR-005 §3.1), ni la protección de valores (Bolt 022, ADR-007).
- No se leyó, copió ni procesó ningún dato de negocio, credencial ni archivo de configuración real: la evidencia
  son nombres de claves, tipos, mensajes y salidas de build.

---

## 9. Verification evidence

### Build del árbol afectado (AC-8)

```text
$ msbuild 2.0/Cooperator2.sln /t:Build /p:Configuration=Release
EXIT=0        <- sin errores ni advertencias (TreatWarningsAsErrors=true, analizadores, firma)
```

Proyectos construidos: los cuatro portables, los dos del arnés y los dos fixtures del gate. Cero `SYSLIB0051`.

### Suites herméticas — VERDES

```text
=== arnes ===
Passed!  - Failed: 0, Passed: 49, Skipped: 0, Total: 49    (Cooperator.Harness.Tests.dll, net10.0)
   (20 casos del Bolt 001 + 29 nuevos de este V-Bounce)

=== caracterizacion ===
Passed!  - Failed: 0, Passed: 83, Skipped: 0, Total: 83    (Cooperator.Framework.Characterization.Tests.exe, net48)
```

Los **cuatro casos negativos canónicos** están cubiertos uno por uno: **clave ausente**, **valor sin prefijo**,
**prefijo desconocido** (con el prefijo ofensor en el mensaje) y **valor no numérico**; más el valor vacío tratado
como ausente y la ausencia de fallback silencioso.

### AC-4 — el control de ausencia (sobre la capa que exista)

```text
=== 0 usos de APIs de configuracion del host en 2.0/src/**/*.cs ===
(vacio)   -> 0 ocurrencias de ConfigurationManager, OpenMappedExeConfiguration, OpenExeConfiguration,
              ExeConfigurationFileMap, ConfigurationUserLevel, ConfigurationSaveMode, ConfigurationSection,
              ConfigurationElement, ConfigurationElementCollection, AppSettings

=== 0 declaraciones del paquete clasico en los .csproj ===
(vacio)   -> ningun <PackageReference>/<Reference> a System.Configuration*

=== 0 rastro de la seccion custom ===
(vacio)   -> 0 ProvidersSection / ProvidersCollection / LazyConfigurationHelper / CreateEmpty

=== capa medida ===
Cooperator.Framework.Core, Cooperator.Framework.Library      (Data y Modeling todavia sin fuentes)
```

### AC-5 — el reporte de dependencias por proyecto: la excepción transitoria, vencida

```text
$ dotnet list package (por proyecto)
  Cooperator.Framework.Core      -> (0 paquetes declarados)
  Cooperator.Framework.Data      -> (0 paquetes declarados)      <- el techo es 1 (el provider del Bolt 014)
  Cooperator.Framework.Library   -> (0 paquetes declarados)
  Cooperator.Framework.Modeling  -> System.Text.Json 10.0.13
```

Ningún proyecto declara ni usa `System.Configuration.ConfigurationManager`: el umbral del NFR
`dependency-footprint` de ADR-001 §5 queda **en su valor declarado** y la excepción transitoria de ADR-001
§4/§5 está **vencida** (ADR-004 §3.5). El reporte se mide **sobre la capa que exista**, y lo declara.

### AC-6 — `SmtpMail` perezoso y validado

```text
The_smtp_type_loads_without_a_static_initializer_that_explodes_when_the_key_is_absent -> pasa
   (RuntimeHelpers.RunClassConstructor sobre SmtpMail con la clave ausente: el tipo se carga, no revienta)
The_smtp_value_is_read_from_the_catalog_and_its_absence_produces_the_typed_error      -> pasa
   (CooperatorConfigurationException nombrando la clave)
```

### AC-7 — la constancia y su cobertura

```text
The_registered_obligation_of_the_data_layer_is_in_the_fragment_with_its_cross_references -> pasa
Every_fragment_entry_declares_what_why_and_replacement                                    -> pasa (9 entradas)
The_breakages_deferred_to_the_data_layer_are_registered_with_their_owner                   -> pasa
   (BOLT011-05 fuente del timeout · BOLT011-06 fuente de la cadena de conexion · BOLT011-07 cache estatico)
```

**Obligación registrada `OBLIGATION-DATA-01`** (AC-4/AC-5): «al aportar `BaseRule.cs`, `SQLHelper.cs` y
`SqlHelperParameterCache.cs`, ninguno de sus valores de configuración puede provenir del host…», con
`owner_bolts: [US-000.BOLT-012, US-000.BOLT-013, US-000.BOLT-014]`, `materially_satisfied_by: US-000.BOLT-014` y
`cross_references: [SPEC-261016-0021, SPEC-261016-0019, SPEC-261016-0020]`.

### 1.4 congelado

```text
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4  -> exit 0
git status --porcelain -- 1.4                                     -> (vacio)
```

### Integration tests (written here, executed in the integration test cycle)

`n/a` — el plan de integración de este SPEC es `n/a` por `test_integration: not-required` (§8.2 del SPEC): el
contrato de configuración y sus fuentes se verifican dentro de la aplicación, sin base de datos, cola ni servicio.

### BUG V-Bounce evidence (if applicable)

`n/a` — no es un Bolt de BUG (§3.3.1).

### Gates (§9 del SPEC)

| Gate | Nivel | Resultado real |
|------|-------|----------------|
| Build / lint / type checks | V-Bounce | **`pass`** — solución 2.0 completa sin errores ni advertencias; `Core` y `Library` en `netstandard2.1` |
| Unit / component tests | V-Bounce | **`pass`** — 49/49 (arnés, incluidos los 29 nuevos) y 83/83 (caracterización) |
| Consumer-side contract tests (test doubles) | V-Bounce | **`pass`** — el contrato se satisface con `DoubleConfiguration` y con la instancia ambiental reemplazada en caliente |
| SAST | V-Bounce | **`pass`** — validación estricta del valor (prefijo y tipo) sin rutas de adivinanza; el retiro **quita** una escritura de configuración |
| Dependency scanning / licenses | V-Bounce | **`pass`** — **0 dependencias nuevas**; `Core`/`Data`/`Library` con 0 paquetes; se **quita** la excepción transitoria |
| OWASP Top 10 — static review | V-Bounce | **`n/a`** — sin superficie alcanzable desde fuera del proceso; el cambio **quita** una ruta de escritura de configuración |
| Prompt-injection scan | V-Bounce | **`n/a`** — no hay construcción de prompts ni entrada no confiable |
| Secret-leak scan | V-Bounce | **`pass`** — el catálogo publica **nombres, tipos y defaults**, nunca valores ni credenciales; los fixtures de las pruebas son sintéticos |
| Hallucination lint | V-Bounce | **`pass`** — claves, tipos y rutas referenciados existen y están verificados por build y por test |
| IP / license provenance | V-Bounce | **`pass`** — 0 código de terceros; el cierre copiado conserva su encabezado BSD |
| PII / DLP | V-Bounce | **`pass`** — `internal`; 0 datos personales; valores de prueba sintéticos |
| Dependency-confusion | V-Bounce | **`pass`** — 0 paquetes nuevos; nada de `System.Configuration.ConfigurationManager` entra al framework |
| Test-first evidence | V-Bounce | **`n/a`** — no es un Bolt de corrección de BUG |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **`pass`** — dos corridas de la suite con resultado idéntico (49/49) |
| Bolt-manifest validation | V-Bounce | **`pass`** — `v_bounces[2]` valida contra `manifest-v5-bolt.schema.json` |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | **`pass`** — los tipos retirados (la sección custom) están identificados y cubiertos por la constancia; `1.4/**` intacto |
| Integration tests (real dependencies) | Integration test | **`n/a`** — `test_integration: not-required` (plan `n/a`) |
| Contract / E2E against the real providers | Integration test | **`n/a`** — sin provider externo ni entorno en el alcance |
| DAST / OWASP Top 10 — dynamic | Integration test | **`n/a`** — sin servicio desplegable |
| Perf-smoke (p95/p99) | Integration test | **`n/a`** — sin endpoint ni carga crítica |
| Observability (logs / metrics / traces) | Integration test | **`n/a`** — sin telemetría de servicio |

---

## 10. Manual interventions

**None.** No hubo parche humano ni fallback manual. La única decisión pendiente es la del revisor sobre la cifra
del cap (§8.1), que no bloquea la entrega.

---

## 11. Evidence links

- **Diff / PR:** no hay — el trabajo está **sin commitear** a propósito (G34: el commit es del humano). Los
  archivos nuevos y modificados están listados en §3 y §4.
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`.
- **V-Bounce N°1 (historia inmutable):** [`MEM-261016-0053`](MEM-261016-0053-configuracion-portable.md) — el
  análisis del primer bloqueo y el mapa de factibilidad que la revisión 2 usó.
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-011-configuracion-portable.json`](../metrics/bolts/US-000.BOLT-011-configuracion-portable.json)
- **Evidencia versionada por este Bolt:** `2.0/docs/configuration-keys.md` · `2.0/docs/api-removals/US-000.BOLT-011.json`
- **Normativo aplicado:** SPEC-261016-0018 (rev. 2) §4/§6/§7/§8/§9/§15 y sus dos findings · ADR-004 §3.2/§3.3/§3.5/§5 ·
  ADR-001 §3.1 (`L → C`)/§3.4/§4/§5 · ADR-007 §3 (el seam) · ADR-006 §3.1 (el nombre calificado del provider).
- **Prerequisito consumido (del Bolt 001, sólo lectura):** `2.0/Cooperator2.sln`, `2.0/Directory.Build.props`,
  `2.0/Directory.Packages.props`, `2.0/tests/**`. **No se modificó ninguno** salvo los `ProjectReference` de §4.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | bloques de agente dedicados al contrato, al catálogo, al retiro, a las pruebas y al cierre; no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | **2** (dentro del mismo Bolt; el N°1 quedó `blocked` y su MEM es inmutable) |
| Tests created | **29** — `ConfigurationContractTests` (contrato, default, los cuatro negativos, forma del valor, seam, doble del contrato y cambio en caliente) + `HostConfigurationAbsenceTests` (ausencia por capa, ausencia de declaraciones, sección custom, carga de `SmtpMail` y constancia). El arnés pasa de 20 a **49** casos |
| AI-generated code | 100% — 15 archivos nuevos y 2 modificados (§3/§4); **73 líneas de cierre copiadas tal cual** del 1.4 (`BaseException`, `SmtpErrorException`) |
| Loops consumed | **4 de 10** — (1) build de `Core`, (2) build de `Library`, (3) suite del arnés con los dos fallos corregidos, (4) verificación completa (solución + suites + reporte de dependencias) |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval` |

---

## 13. Pending items and stubs

- [ ] **DECISIÓN DEL REVISOR (§8.1):** la **cifra** del default de `Cooperator:Cache:MaxEntries`. Publicarla acá
      es una línea en `ConfigurationKeys.cs` y en `2.0/docs/configuration-keys.md`; hoy la clave y su tipo están
      publicados y su dueño (el Bolt 013) está declarado. No bloquea: nada del contrato depende del valor.
- [ ] **OBLIGACIÓN REGISTRADA (`OBLIGATION-DATA-01`) — de los Bolts 012/013/014:** al aportar `BaseRule.cs`,
      `SQLHelper.cs` y `SqlHelperParameterCache.cs`, **ninguna** de sus lecturas de configuración puede venir del
      host; el control de ausencia del arnés las alcanza automáticamente y también exige que el valor de conexión
      llegue de `GetConnectionString(name)` y el timeout del catálogo. Está en
      `2.0/docs/api-removals/US-000.BOLT-011.json` con su referencia cruzada a `SPEC-261016-0019` y `SPEC-261016-0020`.
- [ ] **Hallazgo de housekeeping (no corregido, es de otro dueño):** el `## Status timeline` de
      `US-000.BOLT-011` quedó **fuera de orden cronológico** —la fila `ready → specified` (T03) está al final,
      después de T05— porque se repuso desde otra sesión. Las filas están todas; el orden es el del documento.
      Corregirlo es un acto de su dueño, no de este V-Bounce (mi única edición al Bolt es la fila T06 de G11).
- [ ] **Insumo para el Bolt 028 (notas de migración):** la lista de rupturas de este Bolt está en el MEM §1 y en
      la constancia; las **tres diferidas a la capa `Data`** son las que el consumidor verá cuando esa capa
      aterrice, y ya quedan avisadas.
- [ ] **Dato para los Bolts 022/030:** el retiro de la heurística de `BaseRule.cs` elimina la **única** llamada a
      la cripto legacy de esa ruta (`Library.Cryptography.Decrypt`, dentro de la heurística). Está registrado como
      evidencia en la constancia, sin ejecutarlo acá.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` completa el V-Bounce y, al ser
> el último, es el evento **Development Completed** del Bolt.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | — |
| **Roles** | — |
| **Decision** | — |
| **review_ready_at** | `2026-10-16T01:17:34-03:00` |
| **review.started_at** | — |
| **review.decided_at** | — |
| **Review evidence** | — |
| **Comments** | — |
| **Findings** | — |
| **acknowledged_without_comment** | — |
| **acknowledgment_reason** | — |
