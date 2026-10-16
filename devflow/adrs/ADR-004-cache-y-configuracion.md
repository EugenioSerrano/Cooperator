---
id: "ADR-004"
title: "Caché y configuración de Cooperator 2.0"
date: "2026-10-15"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-architect-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "accepted" # draft | accepted | rejected | deprecated | superseded
decision_makers: ["architect", "tech_lead"] # Roles that participated in the decision
sources: # REVs, DISCs, previous ADRs, interviews that motivated this decision
  - "devflow/reviews/REV-001-analisis-migracion-cooperator.md"
  - "devflow/analysis/open-questions/OQ-002-soporte-system-web.md"
  - "devflow/adrs/ADR-001-layout-y-empaquetado-cooperator-2.md"
  - "devflow/adrs/ADR-002-formato-coop2-y-migrador.md"
  - "devflow/adrs/ADR-003-motor-scriban-headless-y-cli.md"
  - "1.4/Cooperator.Framework.Data/BaseGateway.cs"
supersedes: [] # ADRs this one replaces (§3.5 conflict resolution may supersede several)
conflicts_with: [] # ADRs whose decisions this ADR contradicts (optional; must be resolved by a superseding ADR before it governs a SPEC — §2.8)
tags: ["2.0", "caching", "configuration", "netstandard2.1", "appsettings", "lazyload-config", "dependency-footprint"]
nfrs: ["cache-memory-bound", "config-fail-fast"] # NFRs governed by this ADR (performance, security, availability, etc.)
waiver: # Only for gate-override ADRs (§3.6)
  gate: "" # Gate being waived
  reason: "" # Why the gate is waived
  owner: "" # Person responsible for the exception
  compensating_control: "" # Control that mitigates the waived gate
  expires_at: "" # YYYY-MM-DD — mandatory, no open-ended waivers
review_ready_at: "2026-10-15T23:06:38-03:00" # When this exact version is submitted for review (§3.0)
review: # AITL-ADR-Approval — decisión de human:eugenioserrano; transcrita por el Coordinador (writing is not signing, §3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "human:eugenioserrano", role: "tech_lead", model: null}]
  written_by: [{actor: "agent:avenga-devflow", role: "coordinator", model: "deepseek/deepseek-flash"}]
  started_at: "2026-10-15T23:11:40-03:00"
  decided_at: "2026-10-15T23:11:40-03:00"
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "Sin observaciones adicionales; aprobación directa en la sesión interactiva del 2026-10-15 (revisión de pares completa; ruteo §6 cerrado)."
---

<!-- §3.15 language policy: YAML keys, status enums, IDs and section headings (##) in English (the schema);
     ADR title and prose in the project's content_language (devflow/LANGUAGE); `AITL-ADR-Approval` is never translated.
     ⚠️ AITL-ADR-Approval: this ADR stays a DRAFT — it cannot govern a SPEC, establish an NFR or authorize an
     exception — until an Architect / Tech Lead records the decision in the `review` block (§2.8, §3.5); no
     approval is inherited from related artifacts.
     ⚠️ ADR conflicts (§2.8): check the decision log for active ADRs contradicting this one before approval; a
     conflict is declared in `conflicts_with` and resolved by a superseding ADR.
     ⚠️ Content scope (§2.8): an ADR records architecture decisions and constraints, not functional behaviour —
     functional requirements live in the feature US and its ACs. -->

# ADR-004 — Caché y configuración de Cooperator 2.0

| Field          | Value |
|----------------|-------|
| **Status**     | accepted |
| **Decision-makers** | architect, tech_lead |
| **Sources**    | REV-001 (aprobado 2026-10-15), OQ-002 (respondida 2026-10-15), ADR-001/ADR-002/ADR-003 (accepted) |
| **Supersedes** | None |
| **Conflicts with** | None — no contradice a los ADR-001/002/003 accepted: consume sus slots (ADR-001 §3.6), **vence** la excepción transitoria que el ADR-001 §4/§5 dejó abierta a propósito y respeta su política de dependencias (ADR-001 §3.4/§5) |

---

## 1. Context

`REV-001` §6 ruteó a **una sola** decisión el ítem 4 (abstracción de caché) y el ítem 9 (configuración portable), porque en 1.4 son la misma dependencia: los dos bloqueantes vienen de la misma familia (`System.Web` + `System.Configuration`). Los hallazgos que la motivan:

- **F-07/F-08 — `System.Web` en el corazón del framework**: el caché de `GetAll`/`GetAllDictionary` vive en `BaseGateway`/`BaseLoader` y el de parámetros en `SqlHelperParameterCache`, todos contra `HttpContext.Current.Cache` (claves `Coop_DAL_{RuleName}GetAll_Cache`, TTL 1 h; `Cooperator_DAL_SP_Params_Cache`, TTL 1 día), más el timeout de comandos cacheado en `SQLHelper`. La corrección factual del 2026-10-15 es importante: **fuera de web el caché igual ocurre, en memoria y por instancia, sin TTL** — no es que no exista, es que cambia de alcance y de vigencia según el host. `System.Web` no existe en `netstandard2.1`: sin resolver esto, `Data` no compila.
- **F-13 — connection string con heurística**: `BaseRule` lee `ConfigurationManager.ConnectionStrings["CooperatorConnectionString"]` y, *adivinando por substrings* (`initial catalog`/`database`/`data source`), decide si el texto está cifrado y hay que descifrarlo; el resultado se guarda en un `Dictionary` **estático**, con lo que la cadena no puede cambiar en caliente y una cadena válida sin esas palabras (p. ej. `Server=…`) se interpreta mal.
- **F-04 — configuración custom del LazyLoad que el framework *escribe***: `LazyProviderFactoryConfigHelper` define una `ConfigurationSection` propia (grupo `LazyLoad`, colección de `Provider(entityname, assemblyname, classname)`), la lee con `OpenMappedExeConfiguration`/`ExeConfigurationFileMap` y, si no existe, la **crea y la persiste** en el `.config` del ejecutable host (`CreateEmpty()` + `cfg.Save(ConfigurationSaveMode.Full)`). Una librería que reescribe la configuración de su host es un side-effect invasivo y no tiene equivalente en el runtime moderno.
- **F-22 — config leída en un inicializador estático**: `SmtpMail` hace `ConfigurationManager.AppSettings["SMTPServerName"].ToString()` en un campo estático, con `NullReferenceException` poco diagnosticable si falta la clave.

**La superficie real de configuración de 1.4 es chica y está inventariada** (verificada con grep sobre los 168 `.cs`): (1) una connection string por nombre (`CooperatorConnectionString`); (2) `CooperatorCommandTimeOut`; (3) `SMTPServerName`; (4) la colección de providers del LazyLoad. No hay más. Eso hace viable una abstracción mínima y sin dependencias, en lugar de adoptar una pila completa.

**Restricciones fijas**: `netstandard2.1` y consumidores .NET 10+ (OQ-006/ADR-001 §3.2); sin paquete `-windows`; **minimizar dependencias** — `Core`/`Library` con **cero** terceros y `Data` con **un** provider ADO.NET (ADR-001 §3.4/§5); el ADR-001 §4/§5 dejó una **excepción transitoria** (`System.Configuration.ConfigurationManager` en `Data`) viva hasta el release 2.0.0, cuyo dueño es *"el Bolt que introduzca la fuente portable"* — esta decisión es la que la define y por lo tanto la que la vence (no el paso del tiempo); el ADR-002 §3.1 dejó la connection string **fuera** del `.coop2`; y el ADR-003 §3.6 difirió a este ADR **qué reemplaza al `App.config`** de las apps generadas.

**Frontera**: decide la forma del caché, la fuente/abstracción de configuración, el contrato de la connection string frente al cifrado y la configuración de las apps generadas. **No** decide el **mecanismo de carga** de los providers Lazy (F-02, slot del §6 ítem 10 en ADR-001 §3.6: nombre calificado + `AssemblyLoadContext` + error diagnóstico) ni la **criptografía** (F-23, ADR en preparación), ni el provider ADO.NET (ADR-005).

---

## 2. Alternatives considered

### Alternative A — Caché y configuración propias, sin dependencias; apps generadas con `appsettings.json` + entorno (✅ Selected)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Deja `Core`/`Library` en **cero** dependencias y a `Data` con su **único** provider ADO.NET: el NFR `dependency-footprint` (ADR-001 §5) se mantiene sin excepciones nuevas. Una abstracción mínima cubre exactamente las cuatro claves reales del inventario, en lugar de adaptar el framework a una pila de configuración/caching pensada para aplicaciones. El caché queda detrás de una interfaz propia, con default en memoria para **todos** los hosts (OQ-002), y los adaptadores (Redis u otros) entran después como paquetes satélite sin tocar el núcleo. Que el framework **no lea archivos** elimina el modo de fallo clásico (librería que busca un `.config` que ya no existe) y hace que el valor útil sea explícito. |
| **Cons** | Hay que escribir y testear la implementación en memoria (concurrencia + TTL + cap) y mantener el contrato de claves. El consumidor que quiera leer `appsettings.json` debe armar un objeto de configuración (pocas líneas, pero es trabajo suyo). No se obtiene de arriba la interoperabilidad con `IConfiguration`/`IMemoryCache`. |

### Alternative B — Adoptar la pila `Microsoft.Extensions.*` (`IConfiguration` + `IMemoryCache` + `IOptions`) en el framework

| Aspect   | Detail |
|----------|--------|
| **Pros** | Estándar de facto del ecosistema; resuelve fuentes, precedencias, recarga, binding y TTL/expiración deslizante con código probado; interoperabilidad inmediata con el resto del mundo .NET. |
| **Cons** | Para `netstandard2.1` arrastra una decena de paquetes transitivos (`Configuration.*`, `FileProviders.*`, `Primitives`, `Options`, `DependencyInjection.Abstractions`, `Logging.Abstractions`, `System.Text.Json`) dentro de paquetes que hoy tienen cero o una dependencia; obliga a exponer o adaptar `IConfiguration` en la superficie pública (acoplando el framework a esa pila para siempre) y modela más de lo que el inventario necesita (cuatro claves). |

### Alternative C — Caché propia + configuración totalmente inyectada (sin implementación por defecto)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Máxima pureza: el framework no decide nada sobre el entorno; todo lo provee el consumidor (ideal para tests y para hosts con DI). |
| **Cons** | Rompe la meta de OQ-006 ("cambios mínimos"): cada app generada tendría que implementar la configuración antes de poder conectarse a la base, y el default en memoria del caché dejaría de ser "sin configuración". Convierte una decisión de plataforma en trabajo repetido en cada consumidor. |

### Alternative D — Mantener `app.config` + `System.Configuration.ConfigurationManager` (sólo sacar `System.Web`)

| Aspect   | Detail |
|----------|--------|
| **Pros** | Migración mínima: las claves siguen donde están, el consumidor no cambia nada y las secciones custom siguen funcionando. |
| **Cons** | `app.config`/`web.config` no es el modelo de configuración del runtime moderno (sobrevive por un shim de compatibilidad), `OpenMappedExeConfiguration`/`ExeConfigurationFileMap` **no** están disponibles, y mantener la escritura de la configuración del host perpetúa el side-effect de F-04. Sostiene la excepción transitoria del ADR-001 §4/§5 como estado permanente, contra su vencimiento explícito. |

### Alternative E — Que el **framework** lea el archivo JSON (con `System.Text.Json`) directamente

| Aspect   | Detail |
|----------|--------|
| **Pros** | Experiencia inmediata: existe un `appsettings.json` y el framework lo lee solo, sin que el consumidor escriba una línea. |
| **Cons** | Mete una dependencia de serialización en paquetes que hoy no la tienen (`Core`/`Library`/`Data`) — sólo `Modeling` tiene autorizado STJ (ADR-001 §3.4) — y le da al framework una política de archivo/ubicación que no le corresponde (el ejecutable decide su configuración). Además duplica el lector con el que ya existe del lado de la app. |

---

## 3. Decision

Se adopta la **Alternativa A**: `Data` obtiene un `ICacheProvider` propio con **default en memoria sin dependencias** y adaptadores intercambiables como paquetes satélite; la configuración se lee a través de un contrato mínimo (`ICooperatorConfiguration`) cuya implementación por defecto **solo usa variables de entorno**; el framework **deja de leer y de escribir archivos de configuración**; y las apps generadas pasan de `App.config` a **`appsettings.json` + variables de entorno**, leído por la aplicación.

### 3.1 Caché (F-07/F-08; OQ-002)

- **Contrato**: `ICacheProvider` en `Cooperator.Framework.Data` (único consumidor: gateways, loaders y la caché de parámetros), con una superficie mínima —`TryGet<T>(key)`, `Set<T>(key, value, ttl)`, `GetOrCreate<T>(key, factory, ttl)`, `Remove(key)`— y **claves del llamador** bajo un esquema documentado (`coop:{scope}:{rule}:{kind}`). No hay `RemoveByPrefix` en 2.0.0: quien invalida conoce sus claves (`ResetCache()` de cada gateway remueve las suyas, que es lo que hace hoy con dos claves exactas). El contexto no interpreta las claves.
- **Default en memoria para todos los hosts** (OQ-002): implementación propia **sin dependencias de terceros** (BCL), thread-safe, con **TTL por entrada** y expiración perezosa, y un **cap de entradas configurable** con desalojo (primero las vencidas) para que un proceso de escritorio largo no acumule memoria sin límite. Defaults por región, iguales a los de 1.4 donde existían: colecciones **1 h**, metadata de parámetros de stored procedure **1 día**.
- **Adaptadores intercambiables = paquetes satélite**: convención de nombre `Cooperator.Framework.Data.Caching.<Adapter>` (p. ej. `.Caching.Redis`), con licencia permisiva y versión centralizada (ADR-001 §3.4). **Decisión de este ADR**: en 2.0.0 **no se publica ningún adaptador**, porque el default en memoria cubre a todos los hosts (la mayoría WinForms y algunas web, según OQ-002) y publicar un adaptador sin consumidor que lo pida sería empaquetado sin demanda; un adaptador se construye **a demanda** cuando exista ese consumidor. Precisión sobre el alcance: OQ-002 sólo descartó explícitamente el **adaptador web dedicado** como entregable y dejó los adaptadores como "reemplazables por configuración"; la decisión de no publicar ninguno en 2.0.0 es de este ADR, no un mandato literal de la OQ.
- **`System.Web` fuera del framework**: el caché deja de tener dos caminos según el host y `HttpContext` desaparece de `Data`; el timeout de comandos y la caché de parámetros pasan a usar el mismo `ICacheProvider` (con sus propias regiones).
- **Cambios de comportamiento documentados** (F-07 lo marca explícitamente): (a) en escritorio el caché de colecciones pasa a tener **TTL** (antes no expiraba nunca) y (b) cuando se configure un adaptador compartido, el alcance pasa a ser **entre instancias** (antes era por instancia). Ambos son parte de las notas de migración de la 2.0, no sorpresas de runtime.

### 3.2 Configuración portable (F-04/F-13/F-22)

- **Contrato**: `ICooperatorConfiguration` —lectura por clave, con variantes tipadas y con `GetConnectionString(name)`— declarado en `Cooperator.Framework.Core` (capa base, cero dependencias) y **consumido** por `Data` y `Library`. El contrato es de **valores nombrados**: el framework no conoce archivos, rutas ni formatos.
- **Catálogo de claves** (documentado y versionado con el framework; es el reemplazo directo del inventario de §1): `Cooperator:ConnectionStrings:<nombre>` (nombre por defecto `CooperatorConnectionString`, conservado para continuidad), `Cooperator:Data:CommandTimeoutSeconds`, `Cooperator:Cache:DefaultTtlSeconds`, **`Cooperator:Cache:ParameterMetadataTtlSeconds`** (TTL de la región de metadata de stored procedures, default **1 día** igual que 1.4 — §3.1), `Cooperator:Mail:SmtpServer`, `Cooperator:LazyLoad:Providers:<entidad>:Assembly` y `…:Class`. Toda región de caché con default propio tiene su clave: el default no vive escondido en el código.
- **Implementación por defecto: sólo variables de entorno** (BCL, cero dependencias), con la convención estándar de doble guion bajo para la jerarquía (`Cooperator__Data__CommandTimeoutSeconds`). Ése es el **default del framework** y su única fuente propia: **no** lee archivos y **no** define precedencias. Un host sin variables y sin configuración provista obtiene **defaults documentados** para lo que tiene default (timeout) y **error tipado** para lo que no lo tiene (connection string). *La precedencia **entorno > archivo > default** no es una promesa del framework: es propiedad del **bootstrap de las apps** que emite el set de plantillas de ADR-003 (§3.4), que combina el archivo y el entorno en un `ICooperatorConfiguration` y lo entrega al framework.*
- **Acceso**: `CooperatorSettings.Current` (instancia ambiental, fijable **una vez** al arranque por la app; el framework la lee de forma perezosa) + sobrecargas explícitas que aceptan `ICooperatorConfiguration` para tests y hosts con inyección de dependencias. Se elige ambiental-por-defecto (no inyección obligatoria) porque el código de consumidor que la 2.0 debe migrar con cambios mínimos no instancia gateways a mano; la inyección queda disponible, no impuesta.
- **El framework no lee ni escribe archivos de configuración**: se elimina `OpenMappedExeConfiguration`/`ExeConfigurationFileMap`, la sección custom `LazyLoad/ProvidersSection` **y su escritura** (`CreateEmpty()` + `cfg.Save`). El `Core` no vuelve a tocar la configuración del host.
- **`SmtpMail`** (F-22): lee `Cooperator:Mail:SmtpServer` de forma **perezosa y validada** (sin inicializador estático que explota con `NullReferenceException`), y `SendMailViaCDONTS` —código muerto— ya sale por ADR-001 §3.1.

### 3.3 Connection strings (F-13) — sin heurística

- **Se elimina la detección por substrings**: el valor declara **explícitamente** su condición con un **prefijo de esquema obligatorio** (`plain:` = texto plano, identidad). Cualquier otro esquema lo resuelve el **`IProtectedValueResolver`**, cuya implementación y esquemas fija el **ADR-007** (criptografía, mismo ciclo): este ADR fija la *forma del contrato* —prefijo obligatorio, resolutor intercambiable, `Data` sin conocimiento del algoritmo— y **no** decide algoritmo, claves ni migración de las cadenas ya cifradas.
- **Regla única para el valor sin prefijo: error tipado.** Un valor **sin** prefijo **no** se asume texto plano: se rechaza con `CooperatorConfigurationException` (excepción **propia del framework**, definida en `Core` —no un tipo de `System.Configuration`), indicando la clave y que hay que declarar `plain:` explícitamente. Vale lo mismo para un prefijo **desconocido**: error tipado con la clave y el prefijo ofensor. Nunca hay intento de descifrado a ciegas, ni fallback silencioso, ni adivinanza de "esto parece plano". Si el ADR-007 introduce esquemas nuevos, `Data` **no cambia**: cambia el resolutor.
- **Quién invoca al resolutor** (seam de integración): el **proveedor de configuración** entrega el valor **crudo, con su prefijo**, y **la capa que lo consume** —`Data` para connection strings, el tooling para el reverse— es la que invoca a `IProtectedValueResolver` con el esquema y la carga. El resolutor no se ejecuta en la configuración (que no sabe de esquemas) ni en el framework entero (que no conoce el algoritmo).
- **Se elimina el `Dictionary` estático** de connection strings: la resolución es responsabilidad del proveedor de configuración (que puede cachear con su propio criterio). Cambiar de configuración en caliente —tests, multi-tenant— deja de ser imposible; el reemplazo de la instancia ambiental es la operación documentada.
- **Cero escritura de configuración**: ninguna biblioteca de la 2.0 escribe el archivo de configuración de su host (F-04).

### 3.4 Configuración de las apps generadas (lo diferido por ADR-003 §3.6)

- **`App.config` desaparece** y su reemplazo es **`appsettings.json`** junto al ejecutable (UTF-8 sin BOM) **más variables de entorno**, con precedencia **entorno > archivo > default**: lo que está en el entorno gana (CI, contenedores, secretos inyectados) y el archivo es el default versionable.

```json
{
  "Cooperator": {
    "ConnectionStrings": { "CooperatorConnectionString": "plain:Server=.;Database=Ventas;Integrated Security=SSPI" },
    "Data": { "CommandTimeoutSeconds": 30 },
    "Cache": { "DefaultTtlSeconds": 3600, "ParameterMetadataTtlSeconds": 86400 },
    "LazyLoad": { "Providers": { "Customer": { "Assembly": "Ventas.Lazy", "Class": "CustomerLazyProvider" } } }
  }
}
```

- **Quién lee el archivo**: la **aplicación** (la plantilla del set de ADR-003 emite el bootstrap que arma el `ICooperatorConfiguration` a partir del archivo y del entorno, con el lector que el Bolt elija); el framework sólo consume el contrato (§3.2). Así el framework no gana dependencias y la app mantiene el control de su configuración.
- **Ninguna credencial en claro en un archivo versionado**: si la connection string lleva credenciales, va por entorno o protegida según el ADR-007 (F-13/F-23); el ejemplo de arriba usa autenticación integrada a propósito. Este ADR sólo fija la regla; la protección la gobierna el ADR-007.
- **La precedencia es del bootstrap** (§3.2): el orden **entorno > archivo > default** lo implementa el código que la plantilla emite en la app, no el framework —que no lee archivos—. Este ADR define **qué** reemplaza al `App.config` (archivo + formato + claves) y **de quién es** la precedencia; el lector concreto es del Bolt del set.
- **La configuración del LazyLoad sale del `.config`**: la colección de providers pasa a las claves `Cooperator:LazyLoad:Providers:*` de esta misma fuente. El **mecanismo de carga** de esos providers sigue siendo del slot F-02 (otro ADR): acá se decide **de dónde se leen**, no cómo se resuelven.

### 3.5 Vencimiento de la excepción transitoria de ADR-001 §4/§5

Esta decisión **es** la "fuente portable" que el ADR-001 §4/§5 dejó pendiente: al entrar en vigencia, `Data` **no** toma `System.Configuration.ConfigurationManager` y el umbral del NFR `dependency-footprint` vuelve a "exactamente 1 dependencia de terceros (el provider ADO.NET del ADR-005)". La excepción caduca **con esta decisión**, no con el paso del tiempo, y no se hereda a ningún Bolt posterior. Si un Bolt de migración necesita un puente temporal mientras convive con código 1.4, eso es un **desvío a registrar en su MEM**, no una excepción vigente de este ADR.

*Nota para el aprobador (histórica, no una enmienda):* esta decisión **reemplaza el wording** del ADR-001 §5, cuyo umbral decía "**más la excepción transitoria y acotada** de `System.Configuration.ConfigurationManager` **hasta el release 2.0.0**". El ADR-001 §5 sigue siendo válido como criterio (el umbral vuelve a 1 dependencia) y su §4 anticipaba exactamente este desenlace ("dueño: el Bolt que introduzca la fuente portable"); la referencia temporal se lee ahora como *"hasta que este ADR entre en vigencia"*.

### 3.6 Interplay con ADR-001, ADR-002 y ADR-003

- **ADR-001**: los slots de caché y configuración (§3.6) quedan cerrados por este ADR; no se agregan paquetes a la capa portable (el contrato va en `Core` y la implementación en memoria en `Data`); los adaptadores de caché son paquetes satélite bajo la regla de dependencias de §3.4 y se publican **sólo a demanda**.
- **ADR-002**: el `.coop2` **no** transporta connection strings (ADR-002 §3.1) ni TTLs de caché: la configuración de runtime es del host, no del modelo. La clave por defecto conserva el nombre `CooperatorConnectionString` para que la migración de una app siga siendo reconocible.
- **ADR-003**: el motor no ejecuta SQL ni lee configuración de runtime (emite artefactos); el set de plantillas es quien emite el `appsettings.json` inicial y el bootstrap de §3.4, y quien deja de emitir `App.config` (F-37). Este ADR define **qué** reemplaza al `App.config`; el **cómo** de la plantilla es del set.

---

## 4. Consequences

**Positive:**

- `Data` compila sobre `netstandard2.1` con `System.Web` eliminado de raíz: los cuatro puntos de caché dejan de tener dos caminos según el host y el framework gana un comportamiento único y predecible.
- El inventario completo de configuración (cuatro claves) queda detrás de un contrato mínimo, sin sumar dependencias a `Core`, `Library` ni `Data`: el NFR `dependency-footprint` de ADR-001 se sostiene sin excepciones nuevas.
- Desaparece el side-effect más invasivo de 1.4: ninguna biblioteca lee **ni escribe** el archivo de configuración de su host.
- La connection string deja de adivinarse: prefijo explícito, error tipado ante lo desconocido y sin `Dictionary` estático que impida cambiarla en caliente.
- El caché pasa a ser reemplazable (Redis u otro) sin tocar el núcleo, y el default en memoria sirve a todos los hosts sin configuración — exactamente la resolución de OQ-002.
- Las apps generadas obtienen un modelo de configuración moderno (`appsettings.json` + entorno) con precedencia explícita y sin `App.config`, y el framework queda agnóstico de dónde viven los valores.

**Trade-offs:**

- Hay que **implementar y testear** la caché en memoria (concurrencia, TTL, cap, desalojo) y mantener el catálogo de claves: es código propio que la pila `Microsoft.Extensions.*` daría hecho (Alternative B), a cambio de no arrastrarla.
- El consumidor que quiera leer un archivo debe armar su objeto de configuración (una línea con bajo costo, pero es un paso explícito) en lugar de que el framework lo haga solo (Alternative E).
- Los defaults de acceso son **ambientales**: la instancia de configuración se fija al arranque y reemplazarla a mitad de proceso es una operación documentada, no un patrón libre (evita carreras y cambios de comportamiento a mitad de request).
- En escritorio, el caché de colecciones ahora expira (1 h por defecto) donde antes vivía para siempre: menos memoria retenida y menos datos rancios, pero es un cambio observable de comportamiento.
- El reemplazo de `App.config` obliga a tocar el arranque de las apps migradas (una vez por app) y a re-ubicar los valores: trabajo real del camino de migración de ADR-001 §3.2.

**Technical debt:**

- La implementación en memoria es **nueva y sin historial de producción**: hasta que acumule uso, sus bordes (desalojo bajo presión, TTL vencido en lectura concurrente) son deuda de calidad a cubrir con los tests del arnés (F-50).
- El catálogo de claves es un contrato **string-based**: un typo en una clave se comporta como "clave ausente". Se mitiga con constantes publicadas por el framework y con el error tipado de §3.3 para los casos obligatorios, no por validación del conjunto.
- Sin adaptador de caché en 2.0.0 (decisión de §3.1), un consumidor que hoy comparta caché entre instancias web pierde esa propiedad hasta que se construya el satélite correspondiente: queda documentado y con naming ya definido para no improvisarlo.
- La protección real de las connection strings depende del **[ADR-007](ADR-007-politica-criptografica.md)** (mismo ciclo): hasta que entre en vigencia, `plain:` es la única condición que el framework implementa, y las cadenas "cifradas" por la ofuscación de 1.4 deben migrarse a un esquema válido antes de usarse.
- El arranque de las apps generadas queda a medio camino hasta que el set de plantillas de ADR-003 emita el bootstrap de §3.4 y el `appsettings.json`: mientras tanto, la configuración por entorno es el único camino soportado.

---

## 5. Applicable NFRs

| NFR | Description | Threshold | How it is measured |
|-----|-------------|-----------|-------------------|
| Cache memory bound | El caché en memoria no crece sin límite: respeta el cap configurado y desaloja | Con más entradas que el cap, el total almacenado **nunca** lo supera; 0 entradas vencidas servidas | Test del arnés con escrituras concurrentes por encima del cap + inspección del conteo interno; el cap y el TTL por región salen del catálogo de §3.2 |
| Config fail-fast | Una configuración inválida o ausente **no** produce un fallback silencioso: falla con **`CooperatorConfigurationException`** (excepción propia del framework, definida en `Core`), accionable y en el primer uso | 0 rutas de fallback silencioso (connection string ausente, valor **sin prefijo** o con esquema desconocido, timeout no numérico); 100 % de esos casos con `CooperatorConfigurationException` | Tests negativos por cada caso (clave ausente, sin prefijo, prefijo desconocido, valor no parseable) verificando el tipo de excepción (`CooperatorConfigurationException`, nunca un tipo de `System.Configuration`), el nombre de la clave y que el mensaje sea accionable |

---

## 6. References

- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — revisión aprobada el 2026-10-15; §6 ítems 4 y 9; hallazgos F-07/F-08 (`System.Web` en el caché y corrección factual del caché de escritorio), F-13 (connection string con heurística y caché estático), F-04 (sección custom del LazyLoad y escritura del `.config`), F-22 (`SMTPServerName` en inicializador estático), F-23 (cifrado ofuscado, frontera de este ADR) y F-02 (carga de providers, fuera de alcance).
- [`OQ-002`](../analysis/open-questions/OQ-002-soporte-system-web.md) — `ICacheProvider` con **default en memoria** para todos los hosts y adaptadores intercambiables por configuración; el adaptador web dedicado **no** es un entregable.
- [`ADR-001`](ADR-001-layout-y-empaquetado-cooperator-2.md) — accepted: slots de caché/configuración (§3.6), política de dependencias (§3.4), NFRs (§5) y la excepción transitoria que este ADR vence (§4/§5).
- [`ADR-002`](ADR-002-formato-coop2-y-migrador.md) y [`ADR-003`](ADR-003-motor-scriban-headless-y-cli.md) — accepted: la connection string queda fuera del modelo; el motor no ejecuta SQL y la configuración de las apps generadas (§3.6) se resuelve acá.
- Evidencia de código 1.4: `Data/BaseGateway.cs` (claves `Coop_DAL_*`, TTL 1 h, `ResetCache()`), `Data/BaseLoader.cs`, `Data/SQLHelper.cs:84` (timeout desde `AppSettings`), `Data/SqlHelperParameterCache.cs:46-63` (TTL 1 día + `HttpContext`), `Data/BaseRule.cs:56/85/104-129` (nombre por defecto, `Dictionary` estático, heurística de descifrado), `Core/LazyLoad/LazyProviderFactoryConfigHelper.cs` (`ProvidersSection`, `CreateEmpty()`, `cfg.Save`), `Library/SmtpMail.cs:45`.

---

## 7. AITL-ADR-Approval

> **AITL-ADR-Approval** (Avenga DevFlow §2.8, §3.0, §3.5). An ADR does not
> become `accepted` — and therefore governing — without the approval of an
> Architect / Tech Lead. This ADR is the **source of truth for its own
> approval** (recorded in the `review` frontmatter block with review
> evidence); when it governs a SPEC revision, its path appears in that
> revision's `sources`. ADR approvals are never copied to the Bolt manifest.

| Field | Value |
|-------|-------|
| **Architect / Tech Lead** | `human:eugenioserrano` |
| **Role** | tech_lead |
| **Decision** | **approved** |
| **review_ready_at** | `2026-10-15T23:06:38-03:00` |
| **review.started_at** | `2026-10-15T23:11:40-03:00` |
| **review.decided_at** | `2026-10-15T23:11:40-03:00` |
| **Findings** | Sin hallazgos adicionales — `acknowledged_without_comment: true` (motivo en el frontmatter) |
