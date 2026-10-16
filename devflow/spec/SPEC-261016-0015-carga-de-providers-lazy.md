---
id: "SPEC-261016-0015"
title: "Carga y diagnóstico de providers Lazy: nombres explícitos, resolución del host, hook, caché thread-safe y errores tipados"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-005" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-006", "ADR-001"]
prerequisites: ["SPEC-261016-0000-arnes-tests-y-build-reproducible.md"] # arnés, configuración de build y gate de compatibilidad de API (Bolt 001) — ver §5
risk_class: "medium" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L3" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (refactor): carga y diagnostica providers de la propia aplicación, con ensamblados de prueba del mismo árbol — el resultado no toca base de datos, cola, servicio externo ni entorno desplegado."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T00:19:59-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T00:19:59-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T00:19:59-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — gate pre-SPEC PASA: Bolt US-000.BOLT-005 aprobado (AITL-BOLT-READY-Approval, agent:dotnet-architect-reviewer-agent), ADR-006/ADR-001 accepted (tech_lead), ADR-004 accepted (fuente de datos consumida), REV-001 approved; baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4; ninguna fuente gobernada en draft."
    - "Compliant — coherencia Bolt↔SPEC: test_integration: not-required → applicability: n/a con razón; alcance §4 = Bolt §2; exclusiones correctas (config/catálogo en Bolt 011, F-47 en Bolt 008, plantillas en 019)."
    - "Compliant — nombre calificado (display name simple + tipo completo calificado por namespace) con resolución por defecto del host y hook; caché thread-safe por clave calificada sin cachear fallos; las 5 excepciones tipadas coinciden con ADR-006 §3.3 (ProviderNotConfigured/ProviderAssemblyNotFound/ProviderTypeNotFound/ProviderTypeIncompatible/ProviderResolutionFailed); validación de arranque opt-in con entrada explícita."
    - "Compliant — prueba del ensamblado compilado-pero-no-referenciado (AC-7, §8.1): mide el NFR «sin carga implícita» con el camino por defecto fallando y el hook obteniéndolo. Correcto."
    - "DECISIÓN (orden 011↔005) — ALCANZA con el prerrequisito + stop condition: apruebo el SPEC ordenando la secuencia 011→005; NO exijo enmienda del Bolt 005 re-aprobada en READY. Motivo: la dependencia es sobre el contrato ICooperatorConfiguration + catálogo de claves, cuya definición normativa es ADR-004 §3.2 (ADR accepted), no sobre decisiones de implementación del Bolt 011; el Bolt 005 §2 ya reconoce la frontera (ADR-004 §3.2 / Bolt 011); la stop condition §15 #1 (nunca caer a App.config/ProvidersSection) es la red correcta. NOTA registrada: el campo dependencies del Bolt 005 omite el Bolt 011 — reconciliarlo es una corrección de secuencia (no de alcance) que el dueño puede registrar, sin bloquear esta aprobación."
    - "NOTA (divergencia de forma del nombre) — ADR-004 §3.4 ejemplifica «Class»: «CustomerLazyProvider» (sin calificar), mientras ADR-006 §3.1 exige el nombre completo calificado («Ventas.LazyProviders.DefaultLazyProvider»). ADR-006 §3.1 gobierna la forma; el framework la exige y un valor sin calificar produce ProviderTypeNotFoundException. La corrección del ejemplo/etiquetado del catálogo corresponde al SPEC del Bolt 011 (en redacción paralela) — queda registrada para su aprobación."
    - "ADJUDICACIÓN (⚠ aprobador §14) — (a) la validación de arranque recibe entrada explícita (el contrato ADR-004 §3.2 es lectura por clave, sin enumeración del catálogo): APROBADA; la alternativa de enumerar el catálogo exigiría un cambio del contrato ADR-004 → stop, no una invención. (b) el hook devuelve instancia y la caché aplica a ambos caminos: APROBADA, coherente con ADR-006 §3.1/§3.2."
    - "Compliant — retiro de la superficie prohibida (sobrecarga LoadFrom por ruta, sondeo implícito, construcción por nombre simple, caché por clave concatenada) + gate de API del Bolt 001 + lista de rupturas (ADR-008 §3.1); Core conserva 0 dependencias de terceros (ADR-001 §3.4/§5)."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-005`. La configuración de
     providers y su catálogo de claves son del Bolt 011 —este SPEC los **consume**—; el motor, el CLI, el shell
     y las plantillas son de los Bolts 017/018/025/019; el arreglo de `Type.GetType` por nombre simple
     (F-47) es del Bolt 008, y este SPEC sólo no puede reintroducir lo que ADR-006 elimina.
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ PUNTO QUE LA APROBACIÓN DEBE MIRAR (§5, §14, §15): el Bolt declara como única dependencia el Bolt 001, pero
     el camino por entidad y el diagnóstico de "provider no configurado" **consumen el contrato de configuración
     de ADR-004 §3.2**, cuyo entregable es el **Bolt 011**. Este SPEC lo declara prerrequisito con condición de
     parada: nunca se cae a leer `App.config` ni la sección custom (esa ruta se retira).
-->

# SPEC-261016-0015 — Carga y diagnóstico de providers Lazy: nombres explícitos, resolución del host, hook, caché thread-safe y errores tipados

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 10) |
| **Bolt** | [US-000.BOLT-005](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) |
| **ADRs** | [ADR-006](../adrs/ADR-006-carga-de-ensamblados.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | medium |
| **Revision** | 1 |

---

## 1. Objective

Se reescribe la **carga de providers Lazy** de `Cooperator.Framework.Core`: los providers se resuelven
**por nombre explícito** —nombre de ensamblado en su forma simple de *display name* y **nombre completo del
tipo calificado por namespace**— a través de la **resolución normal del host**; el host dispone de un **hook**
explícito (registrado una vez) para imponer su propia estrategia (un `AssemblyLoadContext` propio, una carpeta
de plugins, su contenedor de DI); la **caché de providers** pasa a ser **thread-safe**, con **clave por nombre
calificado**, **reinicio explícito** y sin **envenenar** el proceso ante un fallo; **cada** modo de fallo
produce su **excepción tipada** con la clave de configuración y los nombres intentados; y se ofrece una
**validación de arranque opt-in** que resuelve todos los providers declarados y falla con el **listado
completo** en una sola pasada. **Ningún camino por defecto carga desde una ruta provista por configuración.**

El problema que resuelve está documentado (REV-001 F-02, §6 ítem 10): en 1.4
`SingleFactory.CreateObject(assemblyName, className)` usa `Assembly.Load(<nombre simple>)` —un `AssemblyName`
parcial resuelto contra el `AppDomain` que **sondeaba el directorio base**— y existe además una sobrecarga por
ruta con `Assembly.LoadFrom`; `FactoryCache` guarda instancias en un `Dictionary` estático **sin
sincronización** con clave concatenada por puntos (`"{assemblyName}.{className}"`, que puede colisionar); y todo
fallo degrada en un `NullReferenceException` o un `"file not found"` **lejos de la configuración que lo causó**.
En el runtime objetivo el ALC por defecto **no sondea directorios**: el patrón "copiar DLLs a una carpeta" —que
el propio producto fomentaba con `CooperatorAssemblies` + `HintPath`— deja de resolverse, y una RUTA en
configuración se convierte en un vector de carga arbitraria de código.

Si no se implementa: cualquier configuración de provider incompleta falla **tarde y sin contexto**; la caché
puede corromperse bajo concurrencia; y el framework conserva una superficie de carga que el runtime moderno ya
no justifica y que el producto debe dejar de tolerar.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 10 ruteó el hallazgo **F-02**, el único bloqueante del
inventario cuya severidad es **de comportamiento y no de compilación**: `Assembly.LoadFrom(String)` y
`Assembly.Load(String)` existen en la superficie de `netstandard2.0/2.1`, así que el retargeteo compila y el
fallo aparece en producción (corrección de la revisión, ADR-006 §1). El modelo de uso real —evidencia del
propio producto— es que **el provider del caso normal es una clase del ensamblado de reglas del cliente** (la
plantilla `DefaultLazyProvider` se genera en `*.LazyProviders`), es decir una **dependencia declarada** de su
aplicación; el escenario "plugin externo en una carpeta" es la excepción.

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-006 §3.1** — **nombres obligatorios y explícitos**: `…:Assembly` (nombre de ensamblado en su forma
  simple de display name, p. ej. `Ventas.Lazy`) y `…:Class` (**nombre completo del tipo calificado por
  namespace**, p. ej. `Ventas.LazyProviders.DefaultLazyProvider`); la versión, la cultura y el token **no** se
  exigen ni se verifican (un valor que los incluya se acepta como documentación y **no** cambia la resolución);
  **resolución por defecto = la del host** (`Assembly.Load(new AssemblyName(<nombre simple>))` + tipo por nombre
  completo, delegando en el ALC por defecto); **hook del host** como **segundo camino explícito** (una
  factory/`IAssemblyProvider` que el host registra **una vez**, y si está registrado el framework **no** usa la
  resolución por defecto); **nada de rutas arbitrarias por defecto**; se **elimina** la sobrecarga con
  `LoadFrom`, el **sondeo implícito por directorio base** y la carpeta `CooperatorAssemblies` como mecanismo de
  resolución en runtime; y la regla de nombres es la misma que necesita F-47 en `BaseObjectList` (Bolt 008:
  **no** puede reintroducir resolución implícita).
- **ADR-006 §3.2** — la caché es **thread-safe**, **una instancia por provider y por proceso**, con **clave por
  nombre calificado** (no la concatenación con puntos que puede colisionar), con **reset explícito** y sin
  cachear un fallo como "ausente" (la excepción se propaga y el siguiente intento vuelve a resolver).
- **ADR-006 §3.3** — el **cuadro de diagnósticos**: `ProviderNotConfiguredException`,
  `ProviderAssemblyNotFoundException`, `ProviderTypeNotFoundException`, `ProviderTypeIncompatibleException` y
  `ProviderResolutionFailedException`, cada una con su contenido obligatorio; **sin fallos silenciosos** (nunca
  `null`, nunca "probar con otro nombre", nunca un provider por defecto sin decirlo); y **fail-fast disponible**
  mediante una validación de arranque que resuelve **todos** los providers y falla con el listado completo.
- **ADR-006 §5 (NFRs)** — `no-implicit-assembly-loading` (0 llamadas a `LoadFrom`/`LoadFile`/sondeo de
  directorio en el camino por defecto; 0 claves de configuración capaces de provocar una carga desde ruta) y
  `provider-resolution-fail-fast` (100 % de las causas del cuadro con excepción propia; 0 retornos `null` o
  degradaciones silenciosas; la validación reporta **todos** los fallos en una sola pasada).
- **ADR-001 §3.6/§3.4** — la carga de providers es el slot que este Bolt cierra; `Core` es `netstandard2.1`
  con **cero dependencias** (no se toma `System.Runtime.Loader`: la maquinaria de ALC es del **host** y llega
  por el hook).
- **ADR-004 §3.2** — el **catálogo de configuración** es del ADR-004 (claves
  `Cooperator:LazyLoad:Providers:<entidad>:Assembly` y `…:Class`) y su **entregable es el Bolt 011**; el
  contrato de configuración (`ICooperatorConfiguration`, `Core`) es de ese Bolt. Este SPEC **consume** ambas
  cosas y **no** las define (frontera declarada en §4 y §5).

**Qué decide este SPEC, dentro de esas restricciones.** La forma concreta del resolvedor y del hook, la
estructura de la clave de caché, la forma de la validación de arranque y su entrada, la ubicación de los
nuevos tipos de excepción, el punto de fijación del hook y —sobre todo— **cómo se verifica herméticamente** que
el camino por defecto no sondea directorios (que es el NFR de ADR-006 §5 y el corazón del hallazgo F-02).

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-005`](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) — `status: ready`, `risk_class: medium`, `data_classification: internal`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:00:56-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-02 (carga por nombre/ruta, fallo silencioso en el ALC moderno) y F-47 (`Type.GetType` por nombre simple; consistencia de nombres) y §6 ítem 10 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-006`](../adrs/ADR-006-carga-de-ensamblados.md) — §3.1 (nombres, resolución por defecto, hook, prohibición de rutas, superficie eliminada, consistencia con F-47), §3.2 (caché), §3.3 (cuadro de diagnósticos y validación de arranque), §3.4 (interplay), §5 (NFRs) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.6 (slot de la carga de providers), §3.4 (cero dependencias en la capa base), §3.3 (firma como identidad, sin gate en runtime) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| ADR (fuente de datos que este Bolt consume) | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) §3.2 — catálogo de claves de provider y contrato `ICooperatorConfiguration` | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) — arnés, configuración de build compartida y **gate de compatibilidad de API** | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura):
`1.4/Cooperator.Framework.Core/Factory/Factories.cs` (`SingleFactory.CreateObject(assemblyName, className)` con
`Assembly.Load` + `CreateInstance(className, true)` líneas 48-55; la sobrecarga con `LoadFrom`
líneas 64-72; y `FactoryCache.Get` con `Dictionary` estático sin sincronización y clave
`$"{assemblyName}.{className}"`, líneas 100-118), `1.4/Cooperator.Framework.Core/LazyLoad/LazyProviderFactory.cs`
(líneas 46-49), `1.4/Cooperator.Framework.Core/LazyLoad/ILazyProvider.cs` (el contrato),
`1.4/Cooperator.Framework.Core/LazyLoad/LazyProviderFactoryConfigHelper.cs` (la sección custom
`LazyLoad/ProvidersSection` y su escritura — **se retira en el Bolt 011**, no acá),
`1.4/Tools/CooperatorModeler/Templates/CSharpClasses/DefaultLazyProvider.cs` y
`.../Templates/CSharpClasses/Entity.Auto.cs:185/203` (el provider del caso normal **se genera en el proyecto de
reglas del cliente** y el código generado invoca la fábrica con nombres literales) y
`1.4/Cooperator.Framework.Core/BaseObjectList.cs:409-415` (F-47 — Bolt 008; se cita sólo por la consistencia de
nombres que ADR-006 §3.1 exige).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- La **resolución por nombre explícito**: el valor de nombre (ensamblado en forma simple + tipo completo
  calificado), su validación y la resolución por defecto vía la vía natural del host
  (`Assembly.Load(new AssemblyName(<simple>))` + tipo por nombre completo), sin heurísticas y sin "el primero
  que aparezca".
- El **hook del host** como segundo camino explícito, registrado **una vez**, con precedencia sobre la
  resolución por defecto.
- La **caché de providers**: thread-safe, **una instancia por provider y por proceso**, clave por **nombre
  calificado** (libre de la colisión de la concatenación con puntos), **reinicio explícito** y **sin cachear
  fallos**.
- El **cuadro de diagnósticos tipados** de ADR-006 §3.3: las cinco causas con su excepción y su contenido
  (clave de configuración + nombres intentados + causa subyacente / alternativas del ensamblado / motivo de
  incompatibilidad / origen en el hook).
- La **validación de arranque** opt-in: resuelve los providers declarados en **una sola pasada** y falla con
  el **listado completo** de los que no resolvieron.
- El **retiro del camino de carga por ruta** y del **sondeo implícito de directorio** en el camino por defecto
  (la sobrecarga con `LoadFrom` y el mecanismo de la carpeta `CooperatorAssemblies` en runtime).
- La evidencia del **gate de compatibilidad de API** del Bolt 001 sobre la capa base y la **entrada de las
  rupturas** en las notas de migración (ADR-008 §3.1; documento del Bolt 028).

### Out of scope

- La **configuración de providers y su catálogo de claves**: **ADR-004 §3.2 / Bolt 011**. Este SPEC **consume**
  el contrato de configuración y las claves `Cooperator:LazyLoad:Providers:<entidad>:Assembly|:Class`; no los
  define, no los renombra y no los valida en cuanto a su forma de archivo.
- El **retiro de la sección custom** `LazyLoad/ProvidersSection` y de la escritura del archivo de configuración
  del host: **Bolt 011** (ADR-004 §3.2).
- El **contrato del provider del cliente** (`ILazyProvider`) y los providers del consumidor: no se modifican.
- La **resolución del host**: la aporta el host si registra el hook; el framework no implementa ALC propio ni
  carga desde carpetas.
- El arreglo de **F-47** (`Type.GetType` por nombre simple en `BaseObjectList`): **Bolt 008**. Este SPEC sólo
  respeta la misma regla de nombres y **no** reintroduce resolución implícita (ADR-006 §3.1).
- El **formato `.coop2`**, el **motor de generación**, el **CLI**, el **shell** y las **plantillas**: Bolts
  004/017/018/025/019. Las plantillas que hoy emiten `LazyProviderFactory.Get(<assembly>, <class>)` con nombres
  literales se ajustan en **su** Bolt, no acá.
- **Cualquier capacidad nueva** (un contenedor de plugins del framework, resolución por ruta "opcional", un
  segundo camino implícito): si un host la necesita, va en **su** hook.

---

## 5. Prerequisites and baseline

- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md), `prerequisites`):** la
  configuración de build compartida (`Directory.Build.props`: analizadores, determinismo, warnings como error,
  firma), el **arnés de pruebas** y el **gate de compatibilidad de API** con su línea base 1.4 — que es la
  evidencia explícita de este Bolt para el retiro de superficie.
- **El proyecto `Cooperator.Framework.Core` en el árbol 2.0** (`netstandard2.1`, sin dependencias de terceros,
  con la superficie base y `ILazyProvider` presentes). Si el árbol 2.0 no lo tiene al iniciar el V-Bounce,
  **parar y reportar** (§15): este Bolt no crea la capa base.
- **El contrato de configuración de ADR-004 §3.2 (`ICooperatorConfiguration` y su acceso ambiental) y las
  claves del catálogo de providers** — entregables del **Bolt 011**, que **no** figura en las `dependencies`
  declaradas del Bolt 005. Son la fuente de los nombres en el camino por entidad y la referencia de la
  `ProviderNotConfiguredException` (§7/AC-4). **Este SPEC los declara prerrequisito de ejecución con condición
  de parada** (§15): si no están presentes, la corrida **se detiene** y la salida es una **enmienda del Bolt**
  (agregar la dependencia) o una decisión de orden del dueño — **nunca** caer a leer `App.config`, la sección
  custom ni ninguna fuente propia, que es exactamente lo que ADR-004 §3.2 retira.
- **Toolchain:** el SDK .NET pinneado por el Bolt 001 (`Core` es `netstandard2.1`; el arnés y las pruebas corren
  sobre `net10.0`).
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de la superficie que se
  retira.
- **Build state de partida:** el árbol 2.0 compila con su arnés; este Bolt **no** cambia TFMs ni el mapa de
  paquetes (ADR-001 §3.1) y **no** agrega dependencias a `Core` (ADR-001 §3.4/§5).
- **Decisión de ejecución registrada (no es supuesto):** los ensamblados de los casos de prueba son
  **ensamblados del propio árbol 2.0** (proyectos de soporte de test), no plugins externos: el escenario normal
  es el provider como dependencia declarada (§6/A y §8.1). Nada se verifica contra un despliegue real, de ahí
  el plan de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-005`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).

### Phase A — Nombres explícitos y resolución por defecto del host

**Duration:** 4h total cycle — **Complexity:** High

#### A.1 El nombre calificado como valor del contrato

Se introduce el valor que representa el destino de un provider: **nombre de ensamblado** (forma simple de
display name) + **nombre completo del tipo calificado por namespace**, ambos **obligatorios** (ADR-006 §3.1).
Su validación es estructural y honesta: ambos valores presentes y no vacíos, y el tipo **calificado por
namespace** (contiene un namespace). La versión, la cultura y el token **no** se exigen, no se verifican y, si
vienen en el valor, se aceptan **como documentación** sin cambiar la resolución (ADR-006 §3.1: el framework no
impone binding; la firma es identidad para el host, no un gate en runtime, ADR-001 §3.3).

**Files created:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderName.cs` — el nombre calificado (ensamblado simple + tipo completo), su validación estructural y su representación canónica para la clave de caché y los mensajes
- `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderResolution.cs` — el punto de fijación **única** del hook del host y el acceso al resolvedor vigente (mismo patrón de fijación al arranque que ADR-004 §3.2)

#### A.2 Resolución por defecto: la vía natural del host

Sin hook registrado, el framework resuelve con **`Assembly.Load(new AssemblyName(<nombre simple>))`** —el
nombre simple que declara la configuración, **no** "el primero que aparezca"— y obtiene el tipo por su **nombre
completo**, delegando en el **ALC por defecto** del host (ADR-006 §3.1). **No** hay sondeo de directorios, **no**
hay `LoadFrom`/`LoadFile`, **no** hay búsqueda por nombre simple entre los ensamblados cargados y **no** hay
rutas: la cobertura del caso normal es el provider como **dependencia declarada** de la aplicación (lo que la
plantilla `DefaultLazyProvider` ya produce en el proyecto de reglas del cliente).

La compatibilidad del tipo se verifica **antes** de construir: implementa `ILazyProvider`, no es
abstracto ni interfaz, y tiene constructor público sin parámetros; si no, es
`ProviderTypeIncompatibleException` con el motivo (ADR-006 §3.3).

**Files created:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderResolver.cs` — resolución por defecto (ensamblado + tipo + verificación de compatibilidad) y despacho al hook cuando está registrado
- `2.0/src/Cooperator.Framework.Core/LazyLoad/IAssemblyProvider.cs` — el **hook** del host: resuelve con su ALC, desde una carpeta de plugins (con `AssemblyDependencyResolver`), o devuelve una instancia ya construida (por ejemplo desde su contenedor de DI) — es **su** política de confianza, fuera del framework

#### A.3 La fachada de la carga

`LazyProviderFactory` se reescribe sobre el resolvedor: conserva la entrada por **nombre explícito** que ya usa
el código generado y agrega el **camino por entidad**, que resuelve los nombres del **catálogo de configuración**
(ADR-004 §3.2) a través del contrato de configuración —éste es el camino donde la ausencia de configuración tiene
diagnóstico propio (§C)—. Ninguna de las dos entradas acepta rutas.

**Files modified:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/LazyProviderFactory.cs` — fachada sobre el resolvedor (entrada por nombre explícito y por entidad)

---

### Phase B — La caché de providers

**Duration:** 3h total cycle — **Complexity:** Medium

#### B.1 Caché thread-safe con clave calificada, reset explícito y sin envenenamiento

La caché de proveedores deja de ser un `Dictionary` estático mutado sin sincronización y pasa a ser una
estructura **thread-safe** con la semántica que el producto buscaba: **una instancia por provider y por
proceso** (el provider del cliente mantiene sus propios cachés internos; recrearlo por llamada es lo que la
caché evita). La **clave es el nombre calificado** —el valor de §A.1, no la concatenación con puntos que hoy
puede colisionar (`"A.B" + "C"` vs `"A" + "B.C"`)—. Se expone un **reinicio explícito** (para pruebas y para
hosts que recargan providers) y, ante un fallo de resolución, **la excepción se propaga y no se registra nada**:
el siguiente intento vuelve a resolver, de modo que una configuración corregida en caliente no queda
envenenada (ADR-006 §3.2).

**Files created:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderCache.cs` — caché thread-safe por nombre calificado, con reinicio explícito y sin cachear fallos

**Files modified:**
- `2.0/src/Cooperator.Framework.Core/Factory/Factories.cs` — `FactoryCache` deja de ser el almacén de instancias con clave concatenada; la construcción tipada que **no** es carga por nombre (`CreateObject(Type)`, `CreateObject<T>()`) se conserva

---

### Phase C — El cuadro de diagnósticos tipados

**Duration:** 3h total cycle — **Complexity:** Medium

#### C.1 Una excepción por causa, con el contexto para arreglarlo sin depurar

Se implementa el cuadro de ADR-006 §3.3 **completo**, con el contenido obligatorio de cada fila:

| Causa | Excepción | Contenido obligatorio |
|---|---|---|
| Falta la clave de configuración | `ProviderNotConfiguredException` | Entidad sin provider configurado + la **clave esperada** del catálogo (ADR-004 §3.2) |
| El ensamblado no se resuelve | `ProviderAssemblyNotFoundException` | **Nombre de ensamblado intentado** + si hay **hook registrado** + la **causa subyacente** (`FileNotFoundException`/`FileLoadException` del runtime) |
| El ensamblado existe pero el tipo no | `ProviderTypeNotFoundException` | **Nombre completo de tipo** + **ensamblado resuelto** + los tipos de ese ensamblado que **sí** implementan `ILazyProvider` (si hay alguno) |
| El tipo existe pero no sirve | `ProviderTypeIncompatibleException` | **Por qué**: no implementa `ILazyProvider`, es abstracto/interfaz, o no tiene constructor público sin parámetros |
| El hook del host falla | `ProviderResolutionFailedException` | Que el fallo **vino del hook** (no de la resolución por defecto) + la **excepción original** del host |

- **Sin fallos silenciosos**: nunca se devuelve `null`, nunca se "prueba con otro nombre", nunca se degrada a un
  provider por defecto sin decirlo (ADR-006 §3.3). Un provider que no se puede construir es un **error**, no una
  ausencia.
- **Jerarquía y forma**: las cinco son excepciones **propias del framework en `Core`**, derivadas de la
  excepción base de la capa base (`CoreException`), sin `[Serializable]` ni `SerializationInfo` —nacen POCOs,
  coherente con ADR-008 §3.2— y con constructores públicos que un consumidor pueda capturar.

**Files created:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderNotConfiguredException.cs`
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderAssemblyNotFoundException.cs`
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderTypeNotFoundException.cs`
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderTypeIncompatibleException.cs`
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderResolutionFailedException.cs`

---

### Phase D — Validación de arranque (fail-fast opt-in)

**Duration:** 2h total cycle — **Complexity:** Low

#### D.1 Una pasada, todos los fallos, un solo resultado

El framework expone la **validación de arranque** de ADR-006 §3.3: resuelve **todos** los providers que se le
indican y **no** se detiene en el primero —acumula los fallos y devuelve (o lanza, en su variante estricta) el
**listado completo**, cada uno con su excepción tipada y su contexto—, para que la aplicación la invoque al
iniciar en lugar de descubrir el problema en el primer lazy load de una request de producción. Es **opt-in**:
una app que no la invoque no la ejecuta.

**La entrada de la validación es explícita** (las entidades/providers a validar). Es una decisión de este SPEC
con su razón y su alternativa a la vista del aprobador (§14): el contrato de configuración de ADR-004 §3.2 es de
**lectura por clave** y no expone enumeración del catálogo, de modo que "todos los providers configurados" no
puede descubrirse desde el contrato; la app conoce sus entidades y las declara en el arranque.

**Files created:**
- `2.0/src/Cooperator.Framework.Core/LazyLoad/ProviderStartupValidator.cs` — validación opt-in en una pasada, con el listado completo de fallos
- `2.0/src/Cooperator.Framework.Core/LazyLoad/Exceptions/ProviderValidationException.cs` — el resultado-agregado de la validación: **todos** los fallos tipados, con su causa, en un solo objeto capturable

---

### Phase E — Retiro de la superficie prohibida y evidencia

**Duration:** 2h total cycle — **Complexity:** Low

#### E.1 Qué deja de existir

- **La sobrecarga `CreateObject(assemblyPath, assemblyName, className)` con `Assembly.LoadFrom`** (la firma por
  ruta): se elimina. Es la superficie que convierte un valor de configuración en ejecución de código desde una
  ruta arbitraria (ADR-006 §3.1/§4).
- **El sondeo implícito por directorio base**: no se reproduce (el ALC moderno no lo hace y ADR-006 descarta
  portar la semántica de `AppDomain`).
- **El mecanismo de la carpeta `CooperatorAssemblies` en runtime**: desaparece como camino de resolución (su
  contraparte de compilación ya murió con los `PackageReference`, ADR-001 §3.1).
- **La construcción por nombre simple sin calificar** y el `CreateInstance(className, true)` con binder por
  defecto: reemplazados por la resolución tipada de §A.2.
- **La caché de instancias por clave concatenada**: reemplazada por §B.1.

**Files modified:**
- `2.0/src/Cooperator.Framework.Core/Factory/Factories.cs` — retiro de la sobrecarga por ruta y de la construcción por nombre simple

> **Frontera declarada:** la sección custom `LazyLoad/ProvidersSection` y la escritura del archivo de
> configuración del host **no** se retiran acá: son alcance del **Bolt 011** (ADR-004 §3.2). Este Bolt retira el
> **camino de carga**, no el de configuración.

#### E.2 Evidencia

- **Gate de compatibilidad de API (Bolt 001)** sobre la capa base: cada retiro de §E.1 aparece como diferencia
  contra la línea base 1.4 y debe estar cubierto por la **lista de rupturas** de este SPEC, con su reemplazo
  (ADR-008 §3.1: qué desaparece, qué decisión lo ordena y **con qué se reemplaza**), destinada a las **notas de
  migración** cuyo documento es del **Bolt 028**.
- **Verificación del NFR `no-implicit-assembly-loading`**: 0 llamadas a `LoadFrom`/`LoadFile` y 0 sondeo de
  directorio en el camino por defecto, y 0 claves de configuración capaces de provocar una carga desde ruta
  (ADR-006 §5). Se sostiene con la prueba de §8.1/AC-7, no con una inspección visual.
- **Verificación de la huella de dependencias**: `Core` sigue con **0 dependencias de terceros** (ADR-001
  §3.4/§5, NFR `dependency-footprint`), sin `System.Runtime.Loader`.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: Los nombres son obligatorios y explícitos

**Given** una resolución de provider sin el nombre de ensamblado o sin el nombre completo del tipo
**When** se intenta resolver
**Then** la carga **no ocurre** y se reporta el error con la **clave de configuración** que lo pidió — sin
heurística, sin valor por defecto y sin buscar "algún tipo con ese nombre" entre los ensamblados cargados

### AC-2: El caso normal se resuelve por la vía del host, sin cargar nada a mano

**Given** un provider que es una **dependencia declarada** de la aplicación (el caso de la plantilla
`DefaultLazyProvider` en el proyecto de reglas del cliente)
**When** se pide el provider por su nombre calificado
**Then** se obtiene la instancia por la resolución por defecto del host, y el framework no efectuó ninguna carga
desde ruta, ningún sondeo de directorio y ninguna búsqueda por nombre no calificado

### AC-3: La caché es thread-safe, por provider y por proceso, y no se envenena

**Given** N hilos concurrentes que piden el mismo conjunto de providers (y pares de nombres cuya concatenación
con puntos colisionaría: `("A.B","C")` vs `("A","B.C")`)
**When** las resoluciones terminan
**Then** hay **una sola instancia** por provider en el proceso, cada par se resuelve como **clave distinta** (sin
colisión), no hubo corrupción ni excepciones de concurrencia, el **reinicio explícito** vuelve a construir, y un
fallo de resolución **no** queda cacheado: corregida la configuración, el siguiente intento resuelve

### AC-4: Cada causa produce su excepción tipada con su contexto

**Given** cada uno de los cinco modos de fallo de ADR-006 §3.3
**When** se intenta resolver
**Then** se obtiene la excepción correspondiente (`ProviderNotConfiguredException`,
`ProviderAssemblyNotFoundException`, `ProviderTypeNotFoundException`, `ProviderTypeIncompatibleException`,
`ProviderResolutionFailedException`) y el mensaje contiene lo que la tabla de ADR-006 §3.3 exige: la clave de
configuración, el ensamblado y el tipo intentados, y la causa (subyacente, del hook, o el motivo de
incompatibilidad), **sin** `null` ni degradación silenciosa

### AC-5: El hook del host reemplaza la resolución por defecto, y si falla, se sabe que fue el hook

**Given** un host que registra el hook
**When** se pide un provider
**Then** la resolución por defecto **no** se usa y la instancia la aporta el hook
**And Given** un hook que lanza
**Then** se obtiene `ProviderResolutionFailedException` indicando que el fallo vino del hook y llevando la
excepción original del host

### AC-6: La validación de arranque reporta todos los fallos en una sola pasada

**Given** varias entidades cuyos providers no resuelven por causas distintas (una sin configuración, una con
ensamblado inexistente, una con tipo inexistente)
**When** se invoca la validación de arranque
**Then** el resultado incluye **todos** los fallos (no sólo el primero), cada uno con su excepción tipada y su
contexto, en **una sola pasada**, y la validación **no** se ejecuta si la aplicación no la invoca

### AC-7: El camino por defecto no carga desde rutas ni sondea directorios

**Given** un ensamblado presente en una carpeta del despliegue pero **no declarado** como dependencia de la
aplicación
**When** se intenta resolver por defecto
**Then** la resolución **falla** con el nombre intentado y la causa del runtime (no lo encuentra por sondeo ni
por ruta) **And** el mismo destino, resuelto a través del **hook** del host, **sí** se obtiene: es la evidencia
de que la carga por carpeta es responsabilidad del host y no un camino del framework

### AC-8: La superficie se retira y el presupuesto de dependencias se mantiene

**Given** el paquete `Core` construido
**When** se corre el gate de compatibilidad de API del Bolt 001 y el listado de dependencias
**Then** la sobrecarga de carga por ruta y la construcción por nombre simple **no** existen, cada ruptura está
cubierta por la lista de rupturas de este SPEC con su reemplazo, no hay llamadas a `LoadFrom`/`LoadFile` en el
camino por defecto, y `Core` conserva **0 dependencias de terceros**

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — todo provider configurado se resuelve por su nombre de ensamblado y su nombre completo de tipo | §6/A.1, §6/A.3 | AC-1, AC-2 · `ProviderResolutionTests` |
| Bolt §2 — la caché es segura en concurrencia, su clave no puede colisionar y admite reinicio explícito | §6/B.1 | AC-3 · `ProviderCacheTests` (concurrencia + colisión de concatenación + reset) |
| Bolt §2 — cada causa del cuadro de ADR-006 §3.3 produce su excepción tipada con clave y nombres | §6/C.1 | AC-4 · `ProviderDiagnosticsTests` |
| Bolt §2 — registrar el hook del host reemplaza la resolución por defecto | §6/A.2, §6/C.1 | AC-5 · `ProviderHookTests` |
| Bolt §2 — la validación de arranque reporta todos los providers fallidos en una sola pasada | §6/D.1 | AC-6 · `ProviderStartupValidationTests` |
| Bolt §2 — no existe ningún camino por defecto que cargue desde una ruta provista por configuración | §6/A.2, §6/E.1 | AC-7, AC-8 · `NoImplicitLoadingTests` (ensamblado no declarado) + chequeo de superficie |
| Bolt §2 — el gate de compatibilidad de API del Bolt 001 y la entrada propia de rupturas (ADR-008 §3.1) | §6/E.2 | AC-8 · gate de API + lista de rupturas en el MEM |
| ADR-006 §5 — NFR `no-implicit-assembly-loading` (0 `LoadFrom`/`LoadFile`/sondeo; 0 claves de ruta) | §6/A.2, §6/E.2 | AC-7 · `NoImplicitLoadingTests` + verificación de fuente/API |
| ADR-006 §5 — NFR `provider-resolution-fail-fast` (100 % de las causas con excepción propia; 0 `null`; validación completa) | §6/C.1, §6/D.1 | AC-4, AC-6 · `ProviderDiagnosticsTests`, `ProviderStartupValidationTests` |
| ADR-001 §3.4/§5 — `Core` con cero dependencias de terceros | §6/A.2, §6/E.2 | AC-8 · listado de dependencias del árbol |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests:** validación estructural del nombre calificado (falta ensamblado, falta tipo, tipo sin
  namespace, valores con versión/token aceptados como documentación); clave de caché (dos pares que colisionarían
  por concatenación producen claves distintas); comportamiento de la caché (una instancia por provider, reinicio
  explícito, fallo no cacheado y resolución posterior exitosa); forma y contenido de cada una de las cinco
  excepciones; comportamiento de la validación de arranque (acumula todos los fallos, no corta en el primero).
- **Component tests:** la carga real contra **ensamblados del propio árbol 2.0**:
  - un **proyecto de soporte de test** con providers válidos, referenciado por el proyecto de pruebas (por lo
    tanto **dependencia declarada**): `Assembly.Load(<nombre simple>)` los resuelve y se ejercita el camino por
    defecto de AC-2;
  - un **segundo proyecto de soporte** con un provider válido que se **compila** pero **no** se referencia
    (`ReferenceOutputAssembly=false`) y se copia a una subcarpeta del directorio de salida: su simple name
    **no** es una dependencia, de modo que el camino por defecto **debe fallar** (AC-7) y el **hook** del host
    —que carga desde esa carpeta con su propio ALC— **debe** obtenerlo: es la prueba de que la carga por carpeta
    es del host y no una capacidad implícita del framework;
  - un **tercer conjunto de tipos de prueba** (en el proyecto de providers válidos) para las causas de tipo:
    un tipo que no implementa `ILazyProvider`, uno abstracto, uno sin constructor público sin parámetros y un
    tipo inexistente con namespace declarado.
- **Consumer-side contract tests (test doubles):** `n/a` — el "proveedor externo" de este Bolt **es** el
  ensamblado de prueba del caso normal: no hay un servicio real que doblar ni un límite de proceso que
  cruzar. El hook se ejercita con una implementación de prueba (que es el caso de uso real: el host aporta la
  política de carga).
- **Edge cases:** nombre de ensamblado con versión/cultura/token en el valor (se acepta como documentación y no
  cambia la resolución); tipo con namespace de un solo segmento; provider con constructor sin parámetros
  `internal` (incompatible) y con constructor estático que falla (fallo del tipo, no del framework); hook
  registrado después de la primera resolución (fijación única: se documenta y se prueba el comportamiento
  elegido); reset de la caché en medio de resoluciones concurrentes; resolución pedida desde varios hilos con
  el mismo nombre y con nombres distintos; ensamblado presente pero de tipo distinto al esperado; cadena vacía
  y `null` en los valores de configuración; entidad sin ninguna de las dos claves y entidad con una sola.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1).
- **Evidencia que produce el V-Bounce:** la corrida de los casos negativos por causa con el **tipo de excepción
  y el contenido del mensaje** (clave + nombres); la corrida concurrente de la caché con el conteo de
  instancias; el resultado de la validación de arranque con **todos** los fallos; el resultado del **gate de
  compatibilidad de API** sobre `Core`; y la verificación del NFR (sin `LoadFrom`/sondeo en el camino por
  defecto). Todo se pega al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `refactor` de la carga de providers:
  los ensamblados involucrados son del propio árbol 2.0 (proyectos de soporte de test), no plugins ni servicios
  desplegados. No hay base de datos, cola, servicio, almacenamiento ni entorno contra el que verificar nada.
- **Contract / E2E against the real providers:** `n/a` — el contrato del provider (`ILazyProvider`) **no
  cambia** en este Bolt y los providers del cliente no se modifican; lo que cambia es cómo se **resuelven**, y
  eso se verifica con ensamblados de prueba. El proveedor de datos real es otro Bolt (014) y otro dominio.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales (el caso "despliegue real" es precisamente el que este Bolt convierte en **error
  tipado** y documenta en las notas de migración).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); `Core` compila sobre `netstandard2.1` sin `System.Runtime.Loader` | pass |
| Unit / component tests | V-Bounce | 100 % verdes, con los casos negativos por causa y la corrida concurrente de la caché | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato del provider **no** cambia; el caso normal y el hook se ejercitan contra ensamblados de prueba (declarado y no declarado), que es el escenario real de la resolución | pass |
| SAST | V-Bounce | 0 hallazgos altos; sin `LoadFrom`/`LoadFile`, sin rutas en el camino por defecto, sin binder por defecto y sin resolución por nombre simple | pass |
| Dependency scanning / licenses | V-Bounce | `Core`: **0 dependencias de terceros** (ADR-001 §3.4/§5); ninguna dependencia nueva | pass |
| OWASP Top 10 — static review | V-Bounce | Carga de código: el único camino por defecto es **por nombre de un destino declarado**; no hay rutas en configuración ni sondeo de directorios; el hook es código del host con **su** política de confianza. La superficie que convertía un valor de configuración en carga arbitraria **se retira** | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no hay material textual no confiable en el camino: los nombres provienen del catálogo de configuración del despliegue y el contenido del tipo nunca se interpreta como instrucción | n/a (razón) |
| Secret-leak scan | V-Bounce | 0 secretos introducidos; los mensajes de diagnóstico incluyen **nombres** y claves de configuración, **nunca** valores sensibles (una cadena de conexión no participa de este camino) | pass |
| Hallucination lint | V-Bounce | Cada decisión trazable a ADR-006/ADR-001 o al Bolt; 0 causas de diagnóstico fuera del cuadro de §3.3; 0 rutas de carga "de conveniencia" introducidas | pass |
| IP / license provenance | V-Bounce | Código propio; 0 dependencias de terceros nuevas; ninguna copia de implementación de ALC de terceros (esa maquinaria es del host) | pass |
| PII / DLP | V-Bounce | `internal`: no se procesan datos personales; el Bolt manipula metadatos de carga (nombres de ensamblado y tipo) y claves de configuración | pass |
| Dependency-confusion | V-Bounce | 0 paquetes nuevos; ninguna resolución por nombre no calificado (la regla de ADR-006 §3.1 es, de hecho, la mitigación del riesgo de confusión de nombres) | pass |
| Test-first evidence | V-Bounce | Los casos negativos por causa se escriben **desde el cuadro de ADR-006 §3.3** antes de implementar el diagnóstico, y la prueba del NFR (ensamblado no declarado) se escribe antes de retirar la superficie de §E.1: sin ella, "no hay sondeo" sería una afirmación sin medición | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la suite (incluidas las concurrentes) dan el mismo resultado; el conteo de instancias por provider es estable | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: el contrato del provider no cambia y los ensamblados son del árbol de pruebas | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: la resolución ocurre una vez por provider y por proceso (la caché es la mitigación de costo por diseño) | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: biblioteca sin runtime propio (§11); su "observabilidad" son los diagnósticos tipados | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `internal`** (espeja el Bolt). El entregable manipula **metadatos de
  carga** (nombres de ensamblado y de tipo) y **claves de configuración**: información interna del despliegue,
  sin datos personales y sin secretos.
- **El cambio es, en sí, una reducción de superficie de ataque** (ADR-006 §4): desaparece la carga por ruta, con
  lo que **un valor de configuración comprometido ya no puede provocar la ejecución de código desde una ruta
  elegida por un atacante**, y desaparece el sondeo de directorios, con lo que el orden del sistema de archivos
  no influye en qué código se ejecuta.
- **Nombres como control de confusión**: exigir ambos valores y el tipo **calificado por namespace** es también
  una mitigación de *dependency/type confusion*: no hay resolución por nombre simple entre los ensamblados
  cargados, ni "el primero que aparezca" (coherente con la regla que ADR-006 §3.1 fija para F-47).
- **La política de confianza avanzada es del host:** si un host necesita cargar desde una carpeta de plugins o
  aislar con un ALC propio, lo hace en **su** hook, con **su** política (verificación de ruta, resolución de
  dependencias, permisos). El framework no la implementa, no la configura desde el catálogo y no la habilita por
  un valor de configuración.
- **Diagnósticos sin filtración:** los mensajes reportan claves de configuración y nombres intentados —que es lo
  que hace útil el error— y **nunca** valores sensibles; una ruta (si el host la usa en su hook) no se refleja
  desde el framework.
- **Sin fallos silenciosos = sin degradación de seguridad:** un provider que no resuelve **no** se sustituye por
  otro ni por una instancia vacía; un fallo no se cachea (ADR-006 §3.2/§3.3). Un `null` silencioso sería, en la
  práctica, un bypass de la lógica de carga de la entidad.

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es una biblioteca de la capa base (`netstandard2.1`) y no
  emite logs, métricas ni trazas a ningún colector; no se le agrega instrumentación porque ninguna fuente lo pide
  y el framework no tiene runtime propio que observar.
- **La observabilidad de este Bolt es el diagnóstico del producto, que es parte del entregable**: las cinco
  excepciones tipadas con la **clave de configuración**, los **nombres intentados** y la **causa** (incluida la
  alternativa disponible —los tipos del ensamblado que sí implementan `ILazyProvider`—). Es exactamente el
  reemplazo del fallo silencioso y lejano que F-02 documentó: el error aparece **en el punto de configuración**.
- **La observabilidad del arranque** es la validación opt-in de §D.1: una app que la invoque obtiene en una
  pasada la lista completa de providers que no resuelven, antes de servir una sola request.
- **La observabilidad de la verificación** (para el revisor): la corrida de los casos negativos con su mensaje,
  el conteo de instancias de la corrida concurrente, el resultado del gate de API y la prueba del ensamblado no
  declarado (que mide el NFR `no-implicit-assembly-loading`). Todo se pega al MEM.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de datos. Lo que hay es una **ruptura de comportamiento deliberada y
  documentada** (ADR-006 §4): un despliegue que hoy funciona "copiando DLLs a una carpeta" **deja de funcionar
  por defecto** y necesita una de dos cosas —declarar el provider como **dependencia** de la aplicación (lo
  correcto, y lo que las plantillas de ADR-003 ya hacen vía `PackageReference`) o **registrar el hook** del
  host—; y las configuraciones con nombres incompletos (sólo el tipo, o un nombre simple) pasan a ser **error de
  configuración** en lugar de "resolverse por casualidad". Ambas cosas entran en las **notas de migración**
  (documento del **Bolt 028**, ADR-008 §3.1) con sus tres datos obligatorios: qué desaparece, qué decisión lo
  ordena y con qué se reemplaza.
- **Compatibility (superficie):** la lista de rupturas de este Bolt —que la evidencia del MEM entrega para las
  notas— comprende: el retiro de la sobrecarga de carga por ruta (`LoadFrom`), el retiro de la construcción por
  nombre simple y del binder por defecto, y el cambio de semántica y de clave de la caché de providers (más el
  nuevo **reset explícito**). El contrato del provider (`ILazyProvider`) **no** cambia. El **gate de
  compatibilidad de API** del Bolt 001 compara contra la línea base 1.4 y una ruptura no listada hace **fallar**
  el gate (ADR-008 §5): la lista y el diff se cierran en el mismo acto.
- **Coherencia con F-47 (Bolt 008):** la regla de nombres que este Bolt implementa ("el nombre identifica
  unívocamente el destino, sin resolución implícita") es la misma que ese Bolt necesita en `BaseObjectList`
  (ADR-006 §3.1). Este SPEC **no** toca ese camino y **no** introduce ninguna resolución implícita que aquel
  Bolt deba deshacer.
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido, ni archivo migrado, ni dato
  transformado: la caché es de proceso y la configuración no se toca. Si el cambio se revirtiera después de
  migrar un consumidor, ese consumidor volvería a la versión anterior del paquete (la contención de ADR-008 §3.1
  sigue siendo el 1.4 congelado); el framework no mantiene un camino "compatibilidad" (§3.1 de ADR-008: sin
  shims).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Regresión silenciosa del LazyLoad configurado por nombre** (un provider que "resolvía por casualidad" deja de hacerlo) | 4 | 4 | Casos negativos por causa con excepción tipada, validación de arranque opt-in, y las notas de migración (Bolt 028) declarando el cambio de despliegue y la corrección de nombres |
| **El NFR "sin carga implícita" se declara cumplido sin medirlo** | 3 | 4 | AC-7 se verifica con un **ensamblado no declarado** presente en el directorio: si el framework lo resolviera, el test falla. La prueba se escribe **antes** de retirar la superficie (§8.1, gate *Test-first evidence*) |
| **Colisión de clave en la caché** (la concatenación con puntos del 1.4) | 3 | 4 | Clave = nombre calificado estructurado (ensamblado, tipo) y prueba explícita con los dos pares que colisionarían (`("A.B","C")` vs `("A","B.C")`) |
| **Corrupción bajo concurrencia** (el `Dictionary` estático del 1.4) | 2 | 4 | Estructura thread-safe + corrida concurrente que además cuenta instancias para probar "una por provider y por proceso" |
| **Caché envenenada**: un fallo queda registrado como ausencia y la corrección en caliente no se nota | 3 | 4 | AC-3: el fallo se propaga y **no** se cachea; prueba explícita de "fallo → corrección → resolución exitosa" |
| **El hook no se ejerce** y queda como superficie sin uso probado | 2 | 3 | Prueba del hook que resuelve un ensamblado **no declarado** desde su propia carpeta (el caso de uso real de ADR-006 §3.1) y prueba del hook que falla |
| **`FactoryCache` conserva un segundo camino** (construcción por nombre concatenado) y el NFR queda a medias | 3 | 3 | §E.1 retira la superficie y el gate de API exige que cada retiro esté en la lista de rupturas; la revisión de §8.1 verifica que no queden llamadas de carga en el camino por defecto |
| **La validación de arranque corta en el primer fallo** y "reporta todos" queda sin cumplir | 3 | 3 | AC-6 con causas mixtas: el resultado se inspecciona por **cantidad y contenido** de los fallos, no por el hecho de haber fallado |
| **Dependencia de terceros colada en `Core`** (p. ej. `System.Runtime.Loader` para "ayudar" al host) | 2 | 4 | ADR-001 §3.4/§5 y el gate de dependencias: `Core` con 0 dependencias; la maquinaria de ALC es del host y entra por el hook |
| **Dependencia de orden con el Bolt 011** (el contrato de configuración no existe todavía) | 4 | 3 | Declarado en §5 y §15 como prerrequisito con **condición de parada**: el V-Bounce se detiene y la salida es una enmienda del Bolt o una decisión de orden — nunca una fuente propia de configuración |
| **Desalineación de la forma del nombre con el catálogo de ADR-004 §3.2** (su ejemplo muestra una clase sin calificar) | 3 | 3 | ADR-006 §3.1 gobierna la forma (tipo calificado por namespace); el framework lo exige y un valor no calificado produce `ProviderTypeNotFoundException` con el nombre intentado; la corrección del ejemplo/etiquetado del catálogo es del Bolt 011 (§14) |
| **Rupturas sin documentar** de la superficie de `Core` | 2 | 3 | ADR-008 §3.1 + gate de API del Bolt 001: una diferencia no listada **falla**; la lista viaja al Bolt 028 |
| **Alcance invadido**: tocar el motor, las plantillas o el camino de F-47 | 2 | 3 | §4 lo excluye; las plantillas que invocan la fábrica son del Bolt 019 y el camino de F-47 del Bolt 008; este SPEC sólo no reintroduce lo prohibido |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-006 y ADR-001; acá se aplican.
> Las filas marcadas **(⚠ aprobador)** admiten más de una lectura del material gobernado y quedan a la vista
> para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) La validación de arranque recibe de forma explícita las entidades/providers a validar** | ADR-006 §3.3 exige que resuelva "**todos** los providers configurados" y reporte el listado completo, pero el contrato de configuración de ADR-004 §3.2 es de **lectura por clave** y **no expone enumeración** del catálogo: el framework no puede descubrir el conjunto. La app conoce sus entidades (las usa en su modelo y en su código generado) y las declara al arrancar. **Alternativa:** si el aprobador quiere que el framework enumere el catálogo, eso es un cambio del contrato de ADR-004 → parar, no una invención de este Bolt |
| **(⚠ aprobador) El hook devuelve la instancia del provider y la caché aplica a los dos caminos** (una instancia por provider y por proceso, con reset explícito) | ADR-006 §3.1 dice que el hook puede "devolver una instancia ya construida (p. ej. desde su contenedor de DI)" y §3.2 fija la semántica de caché "una instancia por provider y por proceso" sin distinguir caminos. Un host que necesite **lifetimes por alcance** puede reiniciar la caché explícitamente o devolver un adaptador que delegue en su contenedor. **Alternativa:** no cachear lo que devuelve el hook (el host ya construye): se acepta si el aprobador lo prefiere, y es una revisión de este SPEC, no una decisión del ejecutor |
| La fachada conserva la entrada por **nombre explícito** y **agrega** el camino por entidad | El código generado (plantilla `DefaultLazyProvider`, `Entity.Auto.cs`) ya invoca la fábrica con nombres explícitos; el camino por entidad es el que hace falta para que el diagnóstico de "provider no configurado" **tenga sentido** (ADR-006 §3.3, primera fila: la entidad sin provider y **la clave esperada del catálogo**) |
| Las cinco excepciones nuevas viven en `Core/LazyLoad/Exceptions/` y derivan de la excepción base de la capa base (`CoreException`) | Son fallos del subsistema de carga, no de datos ni de la capa portable: co-locarlas con la capacidad las hace encontrables y mantiene la jerarquía del framework (ADR-008 §3.2: las excepciones conservan su jerarquía y sus constructores públicos), sin `[Serializable]`/`SerializationInfo` porque nacen POCOs |
| El punto de fijación del hook es **único y al arranque**, con la misma forma que la configuración de ADR-004 §3.2 | ADR-006 §3.1 lo pide así ("que el host registra una vez") y compartir el patrón evita dos estilos de inicialización en la capa base. El comportamiento de un registro tardío se prueba y se documenta (no se deja indeterminado) |
| La **caché tipada de construcción** que no carga por nombre (`CreateObject(Type)`, `CreateObject<T>()`) se conserva | ADR-006 §3.1 elimina la carga por **nombre/ruta** y el sondeo implícito, no la construcción genérica de tipos conocidos; retirarla sería una ruptura no ordenada por ningún ADR |
| La prueba del caso normal usa un **proyecto de soporte referenciado** y la del NFR un **proyecto no referenciado copiado a una subcarpeta** | Es la forma de medir el NFR sin dependencias reales: el primero prueba que la vía natural del host funciona cuando el provider **es** una dependencia (el caso de la plantilla generada); el segundo prueba que **no** hay sondeo de directorio, porque ese ensamblado existe en el despliegue y el camino por defecto debe fallar igual |
| Los casos negativos se escriben **desde el cuadro de ADR-006 §3.3**, no desde la implementación | El cuadro es normativo y su contenido obligatorio (clave, nombres, causa, alternativas) es lo que se verifica: escribir los casos desde la implementación validaría la implementación contra sí misma |
| La **sección custom** de configuración y la escritura del archivo del host **no** se retiran acá | Son alcance del Bolt 011 (ADR-004 §3.2). Este Bolt retira el **camino de carga**; mezclar ambos retiros en un Bolt haría que su evidencia (y su gate) hablen de dos cosas distintas |
| La forma del nombre se toma de **ADR-006 §3.1** (tipo calificado por namespace) aunque el ejemplo de ADR-004 §3.2 muestre una clase sin calificar | ADR-006 es la decisión específica sobre **cómo se resuelven** los nombres; el ejemplo de ADR-004 ilustra **dónde** viven las claves. El framework exige la forma de ADR-006 y un valor no calificado falla con `ProviderTypeNotFoundException` nombrando lo intentado; alinear el ejemplo del catálogo es del Bolt 011 |

---

## 15. Stop conditions

- **El contrato de configuración de ADR-004 §3.2 no está presente** (el Bolt 011 no se ha ejecutado) y por lo
  tanto no existe la fuente de los nombres del camino por entidad ni la clave que la
  `ProviderNotConfiguredException` debe reportar: **parar y reportar**. La salida es una **enmienda del Bolt**
  (agregar la dependencia) o una decisión de orden del dueño, con la SPEC revisada si cambia el plan. **Nunca**
  leer `App.config`, la sección custom `LazyLoad/ProvidersSection` ni ninguna fuente propia: es exactamente la
  ruta que ADR-004 §3.2 retira.
- **El proyecto `Cooperator.Framework.Core` del árbol 2.0 no existe** con su superficie base y `ILazyProvider`:
  parar y reportar; este Bolt no crea la capa base.
- **Se necesita cargar desde una ruta, una carpeta de plugins o un ALC propio** dentro del framework (no en el
  host): parar. Eso es el **hook** del host (ADR-006 §3.1) y, si se pretendiera como capacidad del framework,
  exigiría un ADR que reemplace a ADR-006.
- **`Core` necesitaría una dependencia de terceros** (por ejemplo `System.Runtime.Loader` para "ayudar" al
  host): parar. ADR-001 §3.4/§5 fija cero dependencias en la capa base y la maquinaria de ALC es del host.
- **El gate de compatibilidad de API marca una ruptura que no está en la lista** de §12/§E.2: agregarla a la
  lista **con su reemplazo** antes de cerrar el V-Bounce (ADR-008 §3.1); si no se puede describir el reemplazo,
  parar y reportar.
- **La validación de arranque no puede reportar todos los fallos** por una limitación del contrato de
  configuración (por ejemplo, no hay forma de saber si una entidad tiene configuración): parar y reportar; la
  respuesta es del contrato (ADR-004/Bolt 011), no una heurística acá.
- **Aparece la necesidad de un segundo camino implícito** (por ejemplo "si no está el ensamblado, probar en la
  carpeta de la aplicación"): parar. ADR-006 lo prohíbe explícitamente y su NFR lo mide en cero.
- **Cambia materialmente una fuente gobernada** (el Bolt, ADR-006/ADR-001, REV-001, la línea base de código):
  parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-006 §3.1–§3.4 y §5, ADR-001 §3.4/§3.6)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-005`](../functional/bolts/US-000.BOLT-005-carga-de-providers-lazy.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-02 (carga por nombre simple y por ruta; el ALC moderno no sondea directorios; caché sin sincronización), F-47 (`Type.GetType` por nombre simple: coherencia de nombres) y §6 ítem 10.
- [`ADR-006`](../adrs/ADR-006-carga-de-ensamblados.md) — §3.1 (nombres obligatorios, resolución por defecto del host, hook, prohibición de rutas, superficie eliminada, consistencia con F-47), §3.2 (caché thread-safe por nombre calificado, reset explícito, sin envenenar), §3.3 (cuadro de diagnósticos y validación de arranque), §3.4 (interplay), §5 (NFRs `no-implicit-assembly-loading` y `provider-resolution-fail-fast`).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.6 (slot de la carga de providers), §3.4 (cero dependencias en `Core`), §3.3 (firma como identidad; el gate por token se retira: el framework no impone binding).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato `ICooperatorConfiguration`, catálogo `Cooperator:LazyLoad:Providers:<entidad>:Assembly|:Class`, patrón de fijación única al arranque) — **consumido por este Bolt; su entregable es el Bolt 011**.
- [`ADR-008`](../adrs/ADR-008-compatibilidad-de-api.md) — §3.1 (rupturas documentadas con reemplazo), §3.2 (jerarquía de excepciones; sin `[Serializable]`/`SerializationInfo` en la superficie 2.0), §5 (gate de compatibilidad de API y su dueño).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y gate de compatibilidad de API (prerrequisito).
- [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — el contrato de configuración y el catálogo de claves (prerrequisito de orden, §5/§15) y el retiro de la sección custom.
- [`US-000.BOLT-008`](../functional/bolts/US-000.BOLT-008-resolucion-de-tipos-calificada.md) · [`US-000.BOLT-019`](../functional/bolts/US-000.BOLT-019-plantillas-csharp.md) · [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — los Bolts que continúan la misma regla de nombres, las plantillas que invocan la fábrica y el documento de notas que recibe las rupturas.

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-005`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-8, estrategia de pruebas (incluida la medición del NFR con un ensamblado no declarado), gates, control de datos `internal` y condiciones de parada. Declara la dependencia de orden con el contrato de configuración del Bolt 011 (§5/§15). `integration_test_plan: n/a` coherente con el Bolt. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (arquitectura de la capa base)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
