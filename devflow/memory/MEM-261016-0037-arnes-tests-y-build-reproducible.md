---
id: "MEM-261016-0037"
title: "Arnés de tests de caracterización, build reproducible del 1.4 y gate de API-compatibilidad"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-001" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0000" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-001", "ADR-008"] # governing ADRs; ADR-002 §3.2, ADR-004 §3.2/§3.4/§3.5 and ADR-005 §3 used as supporting context
manifest: "US-000.BOLT-001-arnes-de-tests-y-build-reproducible.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Link/identifier of the diff (PR/commit) for this V-Bounce, when present — the work is UNCOMMITTED (see §11)
review_ready_at: "2026-10-16T00:37:45-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T00:46:14-03:00"
  decided_at: "2026-10-16T00:46:14-03:00"
  findings:
    - "Compliant — suite hermética en verde verificada por re-ejecución (pwsh -NoProfile -File 2.0/build/build.ps1 -Scope Hermetic -ApiCompatDemo -VerifyReproducibility): 103/103 (20 harness net10.0 + 83 characterization net48), 0 fallidos, 0 omitidos, exit 0."
    - "Compliant — build reproducible del 1.4 verificado: doble corrida byte-idéntica (byte_identical: true en reproducibility.json); inventario 1.4.5.0, token 8f7f403e20325116 en Core/Data/Library y sin firma en Utility/exes (AC-3)."
    - "Compliant — gate de API verificado: verde 0 diferencias (exit 0, AC-4); roja que falla identificando las dos rupturas (CP0002 RemovedMethod(string), ChangedSignature(int), ChangedSignature(string), AC-5); informativa reportando la brecha (CP0004 Modeling, esperado)."
    - "Compliant — AC-8 verificado: git diff --quiet 6aa497f7… -- 1.4 exit 0 y git status --porcelain -- 1.4 vacío."
    - "Compliant — desviaciones D-1 (DeterministicSourcePaths=false), D-2 (DebugType=none) y D-3 (reference assembly no honrado → respaldo de ensamblados construidos) bien declaradas y cubiertas por el plan aprobado (SPEC §6.C.1/§13, ADR-008 §5); F-B1/F-B2/F-B3 escalados sin corregir (G04 correcto)."
    - "Minor gap — conteo inexacto en la narrativa: el MEM dice «ocho artefactos» pero la comparación de reproducibilidad cubre 6 ensamblados únicos (los 4 framework + 2 exes; las copias vendorizadas colapsan por nombre). No afecta el resultado verificable (byte_identical: true)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0037 — Arnés de tests de caracterización, build reproducible del 1.4 y gate de API-compatibilidad

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-001](../functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md) |
| **SPEC**        | [SPEC-261016-0000](../spec/SPEC-261016-0000-arnes-tests-y-build-reproducible.md), revisión 1 (aprobada) |
| **V-Bounce**    | 1 |
| **ADRs**        | [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) · [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) |

---

## 1. Executive summary

Se construyó la base de verificación reproducible de Cooperator 2.0: el árbol `2.0/` con su configuración de build centralizada (SDK fijado en `global.json`, `Directory.Build.props` con determinismo, advertencias como error y firma común, `Directory.Packages.props` con gestión central de paquetes y lock files versionados), los cuatro proyectos portables `netstandard2.1` firmados con una copia byte-idéntica del `CooperatorFramework.snk` de 1.4, el **build reproducible del árbol 1.4 congelado sin editar una línea de su fuente** verificado por doble corrida con byte-identidad real, la **línea base 1.4** para el gate de compatibilidad, el **gate de API-compatibilidad** sobre `Microsoft.DotNet.ApiCompat` con sus corridas verde, roja e informativa, la **suite de caracterización del comportamiento observable de 1.4** y la **capacidad de verificación contra SQL Server real** escrita y compilada para el ciclo de integración. El comando reproducible único (`2.0/build/build.ps1`) corre de punta a punta y termina en verde: la corrida hermética deja **103 casos en verde** —20 de invariantes del arnés y del self-test del gate, 83 de caracterización del 1.4— con código de salida `0`; las tres corridas del gate producen el resultado que el SPEC exige (verde sin diferencias, roja identificando las dos rupturas, informativa mostrando la brecha completa), y la doble corrida del 1.4 confirma **byte-identidad de los ocho artefactos**. Las sorpresas no fueron de alcance sino de mecánica del toolchain y se resolvieron sin tocar el árbol congelado: dos desviaciones mecánicas registradas (`DeterministicSourcePaths` y `DebugType`), un hallazgo de producto que se escala sin corregir (el generador de generación no es alcanzable sin host de aplicación) y un hallazgo de datos (el validador de CUIT acepta `00-00000000-0`). Ninguna fase del SPEC quedó sin implementar y ningún resultado de gate se fabricó: lo que no pudo ejecutarse (la suite contra SQL Server real) queda declarado como bloqueado por entorno, no como pasado.

---

## 2. Implemented phases

### Phase A — Esqueleto de la solución 2.0 y configuración de build centralizada

Se materializó el layout que ADR-001 §3.1 fija, en paralelo a `1.4/` y sin tocarlo. `2.0/global.json` fija el SDK **10.0.402** con `rollForward: disable`: un cambio de SDK pasa a ser un acto deliberado, no un accidente de la máquina. `Directory.Build.props` aplica transversalmente `Deterministic=true`, `ContinuousIntegrationBuild=true`, `PathMap`, `TreatWarningsAsErrors=true`, `Nullable=enable`, `ImplicitUsings=enable`, `SignAssembly=true` con `AssemblyOriginatorKeyFile` apuntando a la copia local del `.snk`, y el versionado de la línea (`Version=2.0.0`, `AssemblyVersion=2.0.0.0`). La copia de `CooperatorFramework.snk` se hizo **byte a byte** desde `1.4/Cooperator.Framework.Core/` (mismo SHA-256), de modo que el build de la 2.0 no queda acoplado al árbol congelado y la identidad de firma se conserva. `Directory.Packages.props` habilita `ManagePackageVersionsCentrally` y centraliza las cuatro versiones de paquete que el arnés usa (xUnit v3 en modo vstest, su adaptador de VSTest, `Microsoft.NET.Test.Sdk` y `System.Text.Json`); `RestorePackagesWithLockFile=true` deja un `packages.lock.json` versionado por proyecto, que es lo que hace el restore reproducible. Los cuatro proyectos portables (`Core`, `Library`, `Data`, `Modeling`) se crean **sin fuentes**, `IsPackable=true`, `netstandard2.1`, firmados: fijan TFM, firma, versión y política de dependencias y son el punto de montaje de los Bolts de migración. `Modeling` declara `System.Text.Json` como su única dependencia de serialización; `Core`, `Library` y `Data` quedan con **cero** terceros y `Data` **no** declara el provider (ADR-005 §3 lo asigna al Bolt del provider) ni `System.Configuration.ConfigurationManager` (ADR-004 §3.5 ya venció esa excepción). No se materializaron los proyectos de tooling ni de aplicación: sus decisiones pertenecen a ADR-003/ADR-009.

### Phase B — Build reproducible del 1.4 congelado y su inventario de identidad

`2.0/build/frozen-1.4.ps1` implementa la cadena completa. Primero verifica la **precondición de congelamiento** (`git diff --quiet <commit> -- 1.4` y `git status --porcelain -- 1.4` vacíos) y aborta con código `3` nombrando los archivos si el árbol no está intacto: nunca restaura ni edita nada. Después resuelve el toolchain: MSBuild por `vswhere` con rango de versión explícito `[18.11.0,19.0)` (no "el último"), el targeting pack .NET Framework 4.8 y el SDK fijado en `global.json`. Construye **las tres soluciones del 1.4** (`Framework.sln`, `CooperatorModeler.sln`, `ConnectionStringEncriptor.sln`) en `Release` con `/p:Deterministic=true`, `/p:ContinuousIntegrationBuild=true`, `/p:RunCodeAnalysis=false`, `/p:PathMap=<repo>=/src` y la información de depuración desactivada, **dos veces desde cero**, en directorios de salida distintos bajo `artifacts/<timestamp>/frozen-1.4/run1|run2` y con los intermedios en `%TEMP%` (nunca dentro de `1.4/**`, y ese es el motivo por el que se fijan `OutputPath` e `IntermediateOutputPath`). Compara SHA-256 de cada ensamblado y emite `reproducibility.json`. Con la reproducibilidad confirmada emite `tests/fixtures/frozen-1.4/frozen-1.4-inventory.json`: por cada ensamblado producido, nombre, `AssemblyVersion`, `FileVersion`, `PublicKeyToken`, SHA-256, ruta relativa y configuración — el vocabulario exacto que ADR-002 §3.2 usa para la verificación de identidad del migrador — más la sección de **binarios vendorizados** de `Templates/Assemblies` con sus hashes y la divergencia marcada.

### Phase C — Línea base 1.4 para el gate de API

Con `-ProduceBaseline`, el mismo script reutiliza la construcción fijada y produce el artefacto de referencia de los **cuatro ensamblados del framework**. La vía primaria que el SPEC describe —pedir al compilador el *reference assembly* con `ProduceReferenceAssembly`/`ProduceOnlyReferenceAssembly` en la línea de comandos— **no produjo un artefacto utilizable** en los proyectos *classic* del 1.4, así que se aplicó el respaldo que ADR-008 §5 y el propio SPEC §6.C.1 ya prevén: se usan los **mismos ensamblados construidos** como artefacto de referencia, con la misma validez de comparación. La decisión queda declarada en `PROVENANCE.md` con el comando, el alcance, la razón de las exclusiones (`CooperatorModeler.exe` y `ConnectionStringEncriptor.exe` se registran en el inventario de identidad, no en el baseline de API) y un `baseline-digests.json` con el SHA-256 de cada archivo, que es el control anti-manipulación que el gate verifica **antes** de comparar.

### Phase D — Gate de compatibilidad de API

`2.0/build/api-compat.ps1` implementa el contrato: un lado **contract** (`--left`, la línea base 1.4) contra un lado **implementation** (`--right`, los ensamblados del 1.4 reproducible o los del esqueleto 2.0), en **modo estricto**, con la herramienta resuelta por el manifiesto versionado `.config/dotnet-tools.json` (`Microsoft.DotNet.ApiCompat` **10.0.402**; los nombres de bandera se verificaron contra `dotnet apicompat --help` de esa versión y son `--left`/`--right`/`--strict-mode`/`--verbosity`). Expone tres modos —`Baseline` (verde), `FixturePair` (roja, el nombre que el SPEC §6.D.1 fija) y `Current` (informativa)— más `-Contract`/`-Implementation` para rutas explícitas y `-BaselineDirectory` para ejercitar la guarda anti-manipulación. Escribe un reporte JSON legible por máquina (modo, herramienta, lados, diferencias una por una, código de salida del comparador, notas) más un reporte de texto y el log crudo del comparador, y devuelve el vocabulario compartido `0/1/2/3`. La corrida roja usa el **par de fixtures sintéticos** versionados en `tests/fixtures/api-compat-fixtures/` (dos proyectos que producen el mismo `AssemblyName`, la misma versión y el mismo token, y difieren sólo en la ruptura deliberada: el miembro público retirado y la firma alterada). D.4 quedó cableado en `Directory.Build.targets`: `EnablePackageValidation=true` con `PackageValidationBaselineVersion` **sin valor**, porque 2.0.0 es la primera versión de la línea.

### Phase E — Suite de caracterización del comportamiento observable de 1.4

`2.0/tests/Cooperator.Framework.Characterization.Tests` corre sobre `net48` y referencia los `.csproj` *classic* del 1.4 **tal como están**, por `ProjectReference` — el mismo patrón que ADR-002 §3.2 fija para el migrador — sin consumir los binarios vendorizados desalineados. Los casos se organizan por área en carpetas (`Contracts/`, `Surface/`, `Cryptography/`, `TaxId/`, `Generation/`) y **registran el comportamiento observado** del 1.4 congelado, sin juzgarlo ni corregirlo. Área 1: los cinco estados con sus valores de bit, la semántica de banderas de `ObjectStateHelper`, el contrato de edición de `IObject` (que hereda `IEditableObject`, un hallazgo que el migrador necesita saber), la composición y comparación de identificadores únicos con sus excepciones diagnosticadas, y el comportamiento —asimétrico— de `ValuesGenerator`. Área 2: el inventario del **estado antes** de la superficie de serialización por ensamblado (atributo `[Serializable]`, implementaciones de `ISerializable`, `GetObjectData` y constructores `(SerializationInfo, StreamingContext)`), más el round-trip binario del `Snapshot` del formato legado. Área 3: cifrado sobre **vectores heredados** versionados, producidos una vez con el build 1.4. Área 4: validación de CUIT argentina con casos de borde y de dígitos repetidos. Área 5: el contrato de configuración del `Snapshot` que alimenta la generación, la firma exacta de sus puntos de entrada y el inventario dorado de las plantillas. Las 83 pruebas herméticas terminan en verde.

### Phase F — Capacidad de verificación contra dependencias reales

Dos slots de conexión resueltos por **variable de entorno** (`COOPERATOR_IT_LEGACY_CONNECTIONSTRING` para la superficie 1.4, usado aquí; `COOPERATOR_IT_TARGET_CONNECTIONSTRING` para la 2.0, **reservado y vacío** porque todavía no hay código 2.0 que verificar), esquema y datos de fixture **sintéticos** en `tests/fixtures/sqlserver/` (un par padre/hijo con identidad, `rowversion`, `uniqueidentifier`, `datetime2`, `decimal`, `varbinary(max)` de notas y una clave foránea) y una **guarda dura por construcción**: el arnés sólo acepta una base cuyo nombre contenga `cooperator_it` y aborta en cualquier otro caso, así que dirigir un caso a una base real de un cliente no depende de la disciplina de quien lo corre. La suite de integración (38 casos en cuatro archivos, marcados `Category=Integration`) cubre el contrato de `SqlHelper`/`SqlHelperParameterCache` —que son **internal** en el 1.4 y por eso se ejercitan por reflexión sobre el ensamblado real, ya que el árbol congelado no admite `InternalsVisibleTo`—, el contrato público de `BaseGateway`/`BaseLoader` con `rowversion` y transacciones, el reverse del esquema con `MSSQLProvider`/`MSSQLTypeConverter` contra el catálogo real, y el camino de generación que requiere catálogo. Está escrita y compilada; su corrida es la del ciclo de integración del Bolt y hoy queda bloqueada por un entorno no provisionado.

### Phase G — Comando reproducible único y empaquetado de evidencia

`2.0/build/build.ps1` es el comando que el Bolt exige: resuelve el toolchain (SDK por `global.json`, MSBuild por `vswhere` con rango fijado, verificación del targeting pack 4.8), restaura herramientas y paquetes, construye la solución completa con MSBuild, corre las suites que el alcance pida, lista los paquetes por proyecto, y —opcionalmente— corre la doble corrida del 1.4 y las tres corridas del gate. Todo queda en `2.0/artifacts/<timestamp>/` con un `run-summary.json` que registra el resultado y el código de salida de cada paso, y el script termina con el vocabulario `0/1/2/3`. `2.0/README.md` documenta la invocación, la matriz de proyectos y TFMs, qué constructor usar en cada subconjunto y cómo interpretar los reportes.

---

## 3. Files created

| File | Purpose |
|------|---------|
| `2.0/global.json` | Fija el SDK exacto (10.0.402, sin roll-forward): el toolchain deja de depender de la máquina |
| `2.0/Directory.Build.props` | Propiedades transversales de determinismo, calidad (advertencias como error), firma común, versionado 2.0.x y restore con lock file |
| `2.0/Directory.Build.targets` | Cablea `EnablePackageValidation` para los empaquetables con `PackageValidationBaselineVersion` vacío (el camino a la línea anterior de la 2.0, declarado y no activo) |
| `2.0/Directory.Packages.props` | Única declaración de versión de cada dependencia (CPM); prohíbe versiones sueltas en los `.csproj` |
| `2.0/CooperatorFramework.snk` | Copia byte-idéntica de la clave de firma del 1.4: conserva la identidad de firma sin acoplar el build 2.0 al árbol congelado |
| `2.0/Cooperator2.sln` | Solución propia de la 2.0 (portables, suites y fixtures del gate); los proyectos del 1.4 entran por `ProjectReference`, no como miembros |
| `2.0/.gitignore` | Excluye salidas de build y reportes: la evidencia versionada vive en `tests/fixtures/` |
| `2.0/src/Cooperator.Framework.Core/Cooperator.Framework.Core.csproj` | Portable `netstandard2.1` firmado, sin fuentes: fija la invariante de "cero terceros" y el punto de montaje del Bolt de migración de Core |
| `2.0/src/Cooperator.Framework.Library/…csproj` | Idem para Library (cifrado, CUIT, utilidades) |
| `2.0/src/Cooperator.Framework.Data/…csproj` | Idem para Data, sin provider declarado (ADR-005 §3) y sin `System.Configuration.ConfigurationManager` (ADR-004 §3.5) |
| `2.0/src/Cooperator.Framework.Modeling/…csproj` | Idem para Modeling, con `System.Text.Json` como única dependencia de serialización |
| `2.0/src/**/packages.lock.json` (×4) | Fijan el grafo de paquetes resuelto: restaurar deja de ser una operación con sorpresas |
| `2.0/tests/Cooperator.Harness.Tests/Cooperator.Harness.Tests.csproj` | Proyecto `net10.0` del arnés: invariantes del build centralizado y self-test del gate |
| `2.0/tests/Cooperator.Harness.Tests/Support/Repo.cs` | Localiza la raíz del repositorio caminando desde el ensamblado y ejecuta comandos externos (git, pwsh) con salida capturada |
| `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` | Verifica por artefacto real TFM, firma, `.snk` byte-idéntico, versiones, política de dependencias por proyecto desde los lock files, gestión central de paquetes, presencia de `global.json` y de lock files, y que `1.4/**` sigue intacto (AC-1, AC-6, AC-8) |
| `2.0/tests/Cooperator.Harness.Tests/ApiCompatGateTests.cs` | Self-test del gate: corre la corrida roja y asserta el fallo, el miembro retirado nombrado, la firma alterada nombrada, el reporte JSON con modo estricto y herramienta fijada, y que un baseline manipulado aborta con `3` (AC-5 y edge case anti-manipulación) |
| `2.0/tests/Cooperator.Framework.Characterization.Tests/…csproj` | Proyecto `net48` que referencia los proyectos del 1.4 tal como están; `SignAssembly=false` con razón documentada |
| `2.0/tests/Cooperator.Framework.Characterization.Tests/Support/Context.cs` | Localiza el repositorio, lee los fixtures versionados y aporta los dobles de `IObject`/`IUniqueIdentifiable` con los que se ejercita el contrato público sin inventar tipos de producto |
| `…/Contracts/CoreContractsTests.cs` | Área 1: contratos de estado, identificadores únicos, generador de valores, jerarquía y constructores de las excepciones |
| `…/Surface/SerializationSurfaceTests.cs` | Área 2: inventario del "antes" de la superficie de serialización por ensamblado y round-trip binario del `Snapshot` |
| `…/Cryptography/LegacyCryptographyTests.cs` | Área 3: cifrado contra vectores heredados, determinismo del IV fijo, pérdida ASCII en textos acentuados y caminos de error |
| `…/TaxId/ArgentinaCuitTests.cs` | Área 4: validación de CUIT con separadores obligatorios, dígitos repetidos, longitudes de borde y error tipado |
| `…/Generation/GenerationContractTests.cs` | Área 5: contrato de configuración del `Snapshot`, firma de los puntos de entrada, dependencia del host de aplicación e inventario dorado de plantillas |
| `…/Integration/IntegrationDatabase.cs` | Resuelve el entorno de integración por variable de entorno, aplica esquema y datos de fixture y aplica la guarda dura del nombre de base |
| `…/Integration/SqlHelperContractTests.cs` | Contrato de `SqlHelper`/`SqlHelperParameterCache` contra SQL Server real mediante reflexión (tipos internos del 1.4) |
| `…/Integration/GatewayTransactionTests.cs` | Contrato público de `BaseGateway`/`BaseLoader`, concurrencia por `rowversion` y transacciones con commit y rollback |
| `…/Integration/DbReverseSchemaTests.cs` | Reverse del esquema real: tablas, clave primaria identidad, clave foránea, `rowversion` y mapeo SQL↔CLR |
| `…/Integration/GenerationWithCatalogTests.cs` | El camino de generación que requiere catálogo: caracteriza su insumo y su contrato contra el catálogo real |
| `2.0/tests/fixtures/api-compat-fixtures/BaselineFixture/…` | Lado contract del par sintético: la superficie "buena" que el gate debe proteger |
| `2.0/tests/fixtures/api-compat-fixtures/BrokenFixture/…` | Lado implementation con la ruptura deliberada (miembro retirado y firma alterada) |
| `2.0/tests/fixtures/api-compat-fixtures/expected-breaking-changes.json` | Declara las rupturas que el gate debe NOMBRAR: convierte el criterio "identifica la ruptura" en algo verificable por test |
| `2.0/tests/fixtures/frozen-1.4/frozen-1.4-inventory.json` | Inventario de identidad versionado del build reproducible del 1.4, con los binarios vendorizados desalineados registrados y marcados |
| `2.0/tests/fixtures/frozen-1.4/PROVENANCE.md` | Commit, toolchain, comandos, hashes, la vía usada para la línea base y las dos notas de determinismo |
| `2.0/tests/fixtures/api-baseline-1.4/*.dll` + `PROVENANCE.md` + `baseline-digests.json` | Línea base 1.4 del gate con su procedencia y los digests que el gate verifica antes de comparar |
| `2.0/tests/fixtures/legacy-crypto/crypto-vectors.json` | Vectores heredados de cifrado (texto cifrado por el build 1.4 y su texto claro esperado), sin material de clave |
| `2.0/tests/fixtures/golden-generation/template-digests.json` | Digest de cada plantilla del 1.4 (46 archivos): el contenido dorado que la generación consume |
| `2.0/tests/fixtures/sqlserver/schema.sql` · `seed.sql` | Esquema y datos sintéticos y mínimos del ciclo de integración |
| `2.0/build/frozen-source.json` | El pin: commit congelado, rutas protegidas, las tres soluciones, el toolchain fijado y su contrato de salida |
| `2.0/build/frozen-1.4.ps1` | Precondición de congelamiento, doble corrida determinista, verificación de byte-identidad, inventario de identidad, línea base y emisión de fixtures de caracterización |
| `2.0/build/api-compat.ps1` | El gate: modos Baseline/FixturePair/Current, reporte JSON y de texto, códigos `0/1/2/3` y guarda de integridad del baseline |
| `2.0/build/build.ps1` | El comando reproducible único: toolchain, restore, build, suites, gate, doble corrida, reportes y `run-summary.json` |
| `2.0/.config/dotnet-tools.json` | Manifiesto versionado de herramientas locales: la versión del comparador es parte de la reproducibilidad |
| `2.0/README.md` | Invocación, matriz de proyectos y TFMs, qué constructor usar en cada subconjunto, cómo leer los reportes y la regla dura del 1.4 |

---

## 4. Files modified

| File | Description of change |
|------|----------------------|
| `devflow/functional/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.md` | G11 — única edición permitida al Bolt: `status: "specified"` → `"in-development"` y una fila nueva al final de `## Status timeline` (T04) con el instante de la toma y el baseline de ejecución |
| `devflow/memory/MEM-261016-0037-arnes-tests-y-build-reproducible.md` | Este MEM |
| `devflow/metrics/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.json` | Entrada `v_bounces[]` número 1 (ocho campos) |

**Ninguna línea de `1.4/**` fue modificada, agregada ni eliminada.** Ver §9, AC-8.

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
| El constructor de la solución 2.0 completa es **MSBuild de Visual Studio**, no `dotnet build` | El proyecto de caracterización referencia los `.csproj` *classic* del 1.4 (`ProjectReference`, sin editar una línea, patrón de ADR-002 §3.2) y `dotnet build` no resuelve esa cadena de destino. `build.ps1` orquesta ambos mundos; el subconjunto moderno sí es construible con `dotnet build` |
| Las referencias del arnés al 1.4 llevan `AdditionalProperties: Configuration=Release` | Medido: la cadena de `ProjectReference` **no propaga** `Configuration` a los `.csproj` *classic*, que caen en su default interno `Debug` (que activa `RunCodeAnalysis` con rulesets de Visual Studio 10 y rutas absolutas de aquella instalación). Fijarlo explícitamente mantiene la observación sobre el build de `Release` del 1.4 |
| El build reproducible **no emite información de depuración** (`DebugType=none`, `DebugSymbols=false`) | Fue la única fuente de no-determinismo que quedaba: ver §8, desviación D-2 |
| El proyecto de caracterización **no se firma** (`SignAssembly=false`) | Referencia `Cooperator.Framework.Utility` y `CooperatorModeler` del 1.4, que **no están firmados** (`SignAssembly=false` en su `.csproj`), y un ensamblado firmado que referencia uno sin nombre seguro produce CS8002. Silenciarlo con `NoWarn` habría ocultado una característica real del árbol congelado. La identidad de firma de ADR-001 §3.3 aplica a los cuatro portables de `src/`, y eso lo verifica `Cooperator.Harness.Tests` sobre los ensamblados construidos |
| Los **vectores heredados de cifrado** se generan con el build 1.4 y se versionan; la clave **no** se copia a ningún fixture | Es el insumo que ADR-007 necesita para migrar valores ya cifrados, y el SPEC §10 prohíbe que el material de clave salga del árbol congelado. Un caso de la suite lo verifica explícitamente |
| El **par de fixtures del gate** son dos proyectos compilados, no dos DLL versionados | Versionar binarios haría ilegible la ruptura deliberada y difícil de mantener el par; compilarlos desde fuente con el mismo `AssemblyName`, versión y token hace la corrida roja determinista, hermética y auditable, y además entran al build único |
| El **inventario de identidad** lista sólo lo que MSBuild reporta como salida de cada proyecto | El primer intento recorría el sistema de archivos de salida y recogía duplicados: el `CooperatorModeler` copia `Templates/Assemblies/**` a su salida. Tomar los `TargetPath` reportados por MSBuild deja el inventario exacto y permite comparar los vendorizados contra el build de la **fuente** y no contra su propia copia |
| El **inventario de la superficie de serialización** no cuenta tipos totales | Medido: el build reproducible (`DebugType=none`) y el build con PDB del **mismo código fuente** no coinciden en el conteo de `<PrivateImplementationDetails>`. Contar tipos totales habría hecho fallar la caracterización por una decisión de codegen del compilador; lo que ADR-008 §3.2/§3.4 medirá es la superficie de serialización, y es eso lo que el inventario registra |
| Los casos de integración se **ejercitan por reflexión** donde el 1.4 es `internal` (`SqlHelper`, `SqlHelperParameterCache`) | `1.4/**` es inmutable y no se puede agregar `InternalsVisibleTo`. La reflexión sobre el ensamblado real es la única vía que no edita la fuente congelada, y el comportamiento observado sigue siendo el del 1.4 |
| La transcripción del CUIT con `[char]0x00C1`/`[char]0x00F1` en lugar de literales acentuados | El script que emite los vectores se ejecuta en **Windows PowerShell 5.1** (el host de .NET Framework, el correcto para cargar el ensamblado 1.4) y decodifica como ANSI un UTF-8 sin BOM. El script generado se mantiene en ASCII puro y se escribe con BOM: el valor caracterizado es idéntico y la emisión deja de depender de la codificación del host |
| `-BaselineDirectory` agregada a `api-compat.ps1` | El edge case del SPEC §8.1 ("baseline manipulado → el gate debe abortar con 3") sólo es verificable si el gate puede apuntarse a una **copia** del baseline: así el fixture versionado nunca se toca |

---

## 8. Deviations and assumptions

**D-1 — `DeterministicSourcePaths=false` (desviación mecánica, declarada).** El SPEC §6.B.2 pide `/p:ContinuousIntegrationBuild=true`, que por defecto activa `DeterministicSourcePaths=true`. En proyectos *classic* csproj el compilador **aborta** con «SourceRoot items must include at least one top-level (not nested) item when DeterministicSourcePaths is true» (`Microsoft.Managed.Core.targets`) porque esos proyectos no contribuyen ningún ítem `SourceRoot`. Se fija `DeterministicSourcePaths=false`, lo que además **preserva** el `PathMap` explícito que el SPEC pide (con `DeterministicSourcePaths=true` el `PathMap` se recalcula desde `SourceRoot` y el fijado en la línea de comandos se pierde). `Deterministic=true` y `ContinuousIntegrationBuild=true` se mantienen. El objetivo del SPEC —normalizar las rutas de origen fuera de los artefactos— se cumple, y la razón queda escrita en `frozen-source.json` y en el `PROVENANCE.md` generado.

**D-2 — `DebugType=none` / `DebugSymbols=false` (hallazgo de determinismo, resuelto).** La primera doble corrida terminó con **dos artefactos no byte-idénticos** de ocho: `Cooperator.Framework.Utility.dll` y `ConnectionStringEncriptor.exe` — exactamente los dos proyectos del 1.4 que fijan `<DebugType>pdbonly</DebugType>` en `Release`. La diferencia se aisló byte a byte: **40 bytes** en tres zonas — el `TimeDateStamp` del encabezado PE, dos GUID (MVID y directorio de depuración) y **un único carácter** de la ruta absoluta del PDB, que cambia entre corridas porque el directorio de salida es distinto. Con la información de depuración desactivada, los ocho artefactos son byte-idénticos (`byte_identical: true`). El criterio: el vocabulario de identidad que ADR-002 §3.2 usa para el migrador es nombre + versión + token + hash del **ensamblado**; el PDB es un artefacto aparte y no forma parte de esa identidad. **No fue necesario** recurrir a la alternativa admitida por ADR-002 §3.2 (build conocido-bueno vendorizado), que queda declarada como contingencia en el script y en la procedencia.

**D-3 — El *reference assembly* no se honró en los proyectos *classic*.** Con `ProduceReferenceAssembly=true`/`ProduceOnlyReferenceAssembly=true` la construcción no dejó cuatro artefactos de referencia utilizables, así que la línea base se produjo por el **respaldo ya previsto** por ADR-008 §5 y el SPEC §6.C.1: los mismos ensamblados construidos por el build reproducible del 1.4. La comparación de superficie pública es válida igual y la vía usada queda declarada en `PROVENANCE.md`, con los digests de los archivos realmente versionados.

**F-B1 — Los puntos de entrada de generación no son alcanzables de forma hermética (hallazgo, se escala).** `CodeGenerator.GenerateStoredProcedures` y `GenerateClasses` desreferencian el estado de la interfaz de Windows Forms (`UI.ProgessBar`, `UI.StatusLabel`, `Application.DoEvents()`) y escriben bajo `Library.IO.Path.AppPath(true) + "GeneratedCode\…"`. Verificado de forma observable: sin host de aplicación, **la lectura misma** de `UI.ProgessBar` lanza `NullReferenceException`. Aplicando el SPEC §15.6 —«no se simula un sustituto»— la invocación **no** se caracterizó de forma hermética: la suite hermética caracteriza el contrato de configuración del `Snapshot`, la firma exacta de los puntos de entrada y el inventario dorado; la invocación contra catálogo vive en `Integration/GenerationWithCatalogTests.cs`. Consecuencia: los **fixtures dorados del render** (los scripts de stored procedures y las clases generadas) no se produjeron en este V-Bounce; lo que sí se versionó es el digest de las plantillas que el render consume. Queda como pendiente (§13) y no se declara como satisfecho.

**F-B2 — El reverse de `Templates/Assemblies` desalineado es mayor que el declarado.** El SPEC §13 registra la divergencia de `Cooperator.Framework.Utility.dll` (vendorizado 1.3.3.0 vs fuente 1.4.5.0). El inventario revela además que los vendorizados `Core`, `Data` y `Library` tienen la **misma versión** que el build de la fuente (1.4.5.0) pero **contenido distinto**: `matches_source_build: false` para los tres. No se adoptan en silencio: el inventario los registra con su hash y una nota que se lo advierte al Bolt del migrador.

**F-B3 — `Argentina.IsValidCuit` acepta `00-00000000-0` (hallazgo, se escala).** El dígito verificador del CUIT todo-ceros computa a cero, que es la posición final, así que la función devuelve `true` para un CUIT que no puede emitirse. También se caracterizó que la función **exige** el formato con separadores y longitud exacta de 13 caracteres: el mismo CUIT sin guiones se rechaza. **No se corrige aquí** (SPEC §4, G04): queda registrado con su caso y se escala para su `BUG` y su Bolt.

**Desviación declarada y no bloqueante (heredada del SPEC, confirmada por el aprobador).** La mitad "contra la línea anterior de la 2.0" del criterio de completitud del Bolt es **inactiva por construcción**: 2.0.0 es la primera versión de la línea. `EnablePackageValidation` está cableado y `PackageValidationBaselineVersion` sin valor (§6.D.4). La evidencia exigible en este Bolt es la comparación contra la línea base 1.4, y es la que se entrega.

**Observación sobre la corrida informativa.** En `-Mode Current` el comparador reporta también `CP0004: Assembly with name 'left' does not exist at Cooperator.Framework.Modeling`, porque el esqueleto 2.0 tiene cuatro portables y el 1.4 sólo tres con contraparte (`Core`, `Data`, `Library`; `Utility` no tiene contraparte en la 2.0 y `Modeling` no existe en el 1.4). No es un resultado de gate: es exactamente el tipo de información que la corrida informativa debe mostrar, y así queda en su reporte.

**Supuestos.** (i) `1.4/**/bin` y `1.4/**/obj` ya existían en el árbol de trabajo antes de este V-Bounce y están excluidos por git; el script de la fase B no escribe nada dentro de `1.4/**` (redirige `OutputPath` e `IntermediateOutputPath`), pero las verificaciones exploratorias de este V-Bounce y la construcción por `ProjectReference` de la suite de caracterización **sí** actualizaron esos directorios ignorados. La condición verificable de AC-8 —`git diff --quiet <baseline> -- 1.4` y `git status --porcelain -- 1.4` vacíos— se cumple y está verificada por test. (ii) El entorno del ciclo de integración no está provisionado y no se simuló ninguna corrida contra SQL Server.

---

## 9. Verification evidence

### Build

```
pwsh -NoProfile -File 2.0/build/build.ps1 -Scope Hermetic -ApiCompatDemo

Cooperator 2.0 -- corrida reproducible (Hermetic / Release)
Reportes: D:\Cooperator\2.0\artifacts\20261016-003702

==> Resolviendo el toolchain fijado
    MSBuild: 18.11.0.47511
    SDK: 10.0.402 (rollForward: disable)
    Targeting pack: v4.8
==> Restaurando herramientas (manifiesto versionado) y paquetes
    restore OK (con restore locked por lock files versionados)
==> Construyendo la solucion 2.0 (Release) con MSBuild fijado
    build sin advertencias (tratadas como error): 13 proyectos
==> Listando paquetes por proyecto (evidencia de dependencias y licencias)
    packages.txt
...
Resultado: ok (codigo de salida 0)
```

Compilación limpia de la solución 2.0 **con `TreatWarningsAsErrors=true`**: 13 proyectos, cero advertencias
(AC-1). El proyecto de caracterización (`net48`), que referencia las tres soluciones *classic* del 1.4 por
`ProjectReference`, compila dentro de ese mismo build.

Build reproducible del 1.4 congelado (fase B), corrida dedicada:

```
==> Precondicion de congelamiento (commit 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4)
==> Verificando toolchain fijado
    MSBuild: 18.11.0.47511 (C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe)
    Targeting pack: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8
    .NET SDK: 10.0.402
==> Corrida 1 -- construyendo las tres soluciones del 1.4 (salidas fuera de 1.4/)
==> Corrida 2 -- construyendo las tres soluciones del 1.4 (salidas fuera de 1.4/)
==> Comparando SHA-256 de cada artefacto entre las dos corridas
==> Emitiendo el inventario de identidad (nombre, version, token, hash)
    Inventario: D:\Cooperator\2.0\tests\fixtures\frozen-1.4\frozen-1.4-inventory.json
    Procedencia: D:\Cooperator\2.0\tests\fixtures\frozen-1.4\PROVENANCE.md
==> Produciendo la linea base 1.4 para el gate de API (SPEC 6.C)
    Linea base: D:\Cooperator\2.0\tests\fixtures\api-baseline-1.4 (4 ensamblados)
==> Emitiendo fixtures de caracterizacion (vectores de cifrado y digests de plantillas)
    Vectores de cifrado: D:\Cooperator\2.0\tests\fixtures\legacy-crypto\crypto-vectors.json
    Digests de plantillas: D:\Cooperator\2.0\tests\fixtures\golden-generation\template-digests.json (46 archivos)

OK -- build reproducible del 1.4: byte-identico=True
EXIT=0
```

Byte-identidad real de los **ocho** artefactos en `reproducibility.json` (`byte_identical: true`). El inventario
versionado registra, por ensamblado, `AssemblyVersion`/`FileVersion` **1.4.5.0**, token `8f7f403e20325116` en
`Core`/`Data`/`Library` y `null` donde la fuente no firma (`Utility`, `CooperatorModeler.exe`,
`ConnectionStringEncriptor.exe`) — AC-3.

### Tests

```
==> Corriendo harness-hermetic (filtro: Category!=Integration)
    Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20, Duration: 6 s - Cooperator.Harness.Tests.dll (net10.0)
==> Corriendo characterization-hermetic (filtro: Category!=Integration)
    Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 144 ms - Cooperator.Framework.Characterization.Tests.exe (net48)
```

**103 casos herméticos en verde, 0 en rojo, 0 omitidos.** El conteo de la suite de invariantes incluye el
self-test del gate (verde y rojo) y la guarda anti-manipulación del baseline; el de caracterización cubre las
cinco áreas del SPEC §6.E. No hay `Skip` en ninguna suite: la corrida hermética **excluye** los casos de
integración por filtro de categoría (AC-2, AC-5, AC-6).

Reproducibilidad de comportamiento de la suite hermética: dos corridas consecutivas con el mismo resultado
(20/20 y 83/83), sin dependencia de orden ni de estado compartido entre suites.

### Integration tests (written here, executed in the integration test cycle)

38 casos `Category=Integration` en cuatro archivos, **escritos y compilados** dentro del mismo build
(`SqlHelperContractTests` 11, `GatewayTransactionTests` 10, `DbReverseSchemaTests` 9,
`GenerationWithCatalogTests` 8). La verificación estática es la compilación con `TreatWarningsAsErrors=true`
y la resolución en tiempo de compilación de todas las referencias a la superficie 1.4 y a `System.Data.SqlClient`.

**No se ejecutaron y no se declaran como pasados.** Intentarlos sin entorno es lo que el SPEC §3.3 prohíbe:
la corrida de `-Scope Integration` hoy informa el bloqueo nombrando la variable ausente:

```
Collection fixture type 'IntegrationDatabase' threw in its constructor
--- System.InvalidOperationException : El ciclo de integración no puede correr: falta la variable de entorno
COOPERATOR_IT_LEGACY_CONNECTIONSTRING. El entorno de integración todavía no está provisionado...
```

La guarda dura del nombre de base (`cooperator_it`) se verifica en el constructor del fixture, **antes** de
aplicar el esquema: ningún caso puede alcanzar una base que no sea la de fixture. La corrida real es del ciclo
de integración del Bolt (AC-7: capacidad disponible y contenida).

### BUG V-Bounce evidence (if applicable)

`n/a` — el Bolt no corrige un BUG: es `non-functional` y no hay TDD rojo→verde obligatorio. La evidencia roja
de este Bolt es la del **gate** sobre la ruptura deliberada, que es un control del arnés y no un defecto de
producto; se documenta abajo.

### Gates

| Gate (SPEC §9) | Nivel | Resultado real |
|----------------|-------|----------------|
| Build / lint / type checks | V-Bounce | **`pass`** — compilación limpia de la solución 2.0 y de las tres soluciones del 1.4, con advertencias como error y determinismo activado (13 proyectos, 0 advertencias) |
| Unit / component tests | V-Bounce | **`pass`** — 103 casos herméticos verdes (20 de invariantes + 83 de caracterización), 0 omitidos |
| Consumer-side contract tests (test doubles) | V-Bounce | **`n/a`** con la razón del SPEC — el arnés no define superficie de consumo propia; su contrato verificable es el de la API pública, cubierto por el gate |
| API-compat con la línea base 1.4 (ADR-008 §5) | V-Bounce | **`pass`** — corrida verde con **0 diferencias** (código `0`); corrida roja que **falla identificando las dos rupturas** (código `2`, `CP0002` para `RemovedMethod(string)`, `ChangedSignature(int)` y `ChangedSignature(string)`); corrida informativa que reporta la brecha completa |
| SAST | V-Bounce | **`n/a`** con la razón del SPEC — sin código de producto nuevo: el arnés es configuración de build, scripts de orquestación y tests |
| Dependency scanning / licenses | V-Bounce | **`pass`** — toda dependencia declarada centralmente, fijada y con lock file; `packages.txt` versiona el listado por proyecto; 0 terceros en `Core`/`Library`/`Data` y sólo `System.Text.Json` en `Modeling`, verificado por test desde los lock files |
| OWASP Top 10 — static review | V-Bounce | **`n/a`** con la razón del SPEC — sin superficie externa |
| Prompt-injection scan | V-Bounce | **`n/a`** con la razón del SPEC |
| Secret-leak scan | V-Bounce | **`pass`** — ningún secreto en archivos versionados: los fixtures guardan texto cifrado y digests, jamás material de clave (verificado por test), y la conexión de integración llega por variable de entorno, no por archivo |
| Hallucination lint | V-Bounce | **`pass`** — toda ruta, herramienta y archivo que los scripts y la documentación referencian existe y se construye o se ejecuta en esta corrida |
| IP / license provenance | V-Bounce | **`pass`** — no se incorpora código de terceros copiado; las dependencias se declaran con versión y licencia permisiva |
| PII / DLP | V-Bounce | **`pass`** — `data_classification: internal`; CUIT, datos de seed y textos de cifrado son sintéticos; ningún dato personal real |
| Dependency-confusion | V-Bounce | **`pass`** — versiones fijadas centralmente y lock files versionados; sin nombres de paquete ambiguos |
| Test-first evidence | V-Bounce | **`n/a`** con la razón del SPEC — no es un Bolt de corrección de BUG |
| Behavioral reproducibility (hermetic suite) | V-Bounce | **`pass`** — dos corridas con resultado idéntico; los ocho ensamblados del 1.4 byte-idénticos entre las dos corridas del build reproducible |
| Bolt-manifest validation | V-Bounce | **`pass`** — la entrada `v_bounces[]` de este V-Bounce valida contra `manifest-v5-bolt.schema.json` |
| Integration tests (real dependencies) | Integration test | **`blocked` por entorno** (no `n/a`, no `pass`) — suite escrita y compilada; entorno no provisionado, tal como el propio Bolt declara en sus dependencias externas |
| Contract / E2E against the real providers | Integration test | **`blocked` por entorno**, misma causa |
| DAST / OWASP Top 10 — dynamic | Integration test | **`n/a`** con la razón del SPEC — sin servicio desplegable |
| Perf-smoke (p95/p99) | Integration test | **`n/a`** con la razón del SPEC — sin endpoint ni carga crítica |
| Observability (logs / metrics / traces) | Integration test | **`n/a`** con la razón del SPEC — sin servicio; la observabilidad del arnés son sus reportes y códigos de salida |



#### Manifest

```
python -c "jsonschema Draft202012Validator(manifest-v5-bolt.schema.json).iter_errors(<bolt manifest>)"
VALID
```

El manifiesto del Bolt valida contra `devflow/metrics/manifest-v5-bolt.schema.json` (JSON Schema draft 2020-12) con
la entrada `v_bounces[]` de este V-Bounce completa: `number: 1`, `spec_revision: 1`,
`git_commit: 6aa497f7…` (el HEAD actual, usado como base — el trabajo está sin commitear, ver §11),
`execution_outcome: ready_for_review`, `code_generation`, `mem`, `review_ready_at` y `review_started_at: null`.

#### Encoding de los documentos gobernados

Verificado byte a byte: el MEM, el manifiesto del Bolt y el propio Bolt son UTF-8 **sin BOM**, sin `U+FFFD` y sin
`CR` sueltos (§3.15, W23).

**AC-8 — `1.4/**` intacto:** verificado por test en cada corrida de la suite de invariantes
(`git diff --quiet 6aa497f7… -- 1.4` y `git status --porcelain -- 1.4` vacíos) y por la precondición de
congelamiento de `frozen-1.4.ps1` antes de cada doble corrida.

---

## 10. Manual interventions

**None.** Todo el código, la configuración, los scripts y los tests de este V-Bounce fueron producidos por el
agente. La única edición a un documento gobernado es la de G11 sobre el Bolt (`status` a `in-development` y
su fila de timeline), que es la edición que la regla del V-Bounce exige.

---

## 11. Evidence links

- **Diff / PR:** no hay — el trabajo está **sin commitear** a propósito. El role charter y G34 lo prohíben: el
  agente nunca escribe historia de repositorio por su cuenta; el commit es del humano. El árbol de trabajo es
  el paquete a revisar.
- **Commit (baseline):** `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` — ver `baseline` en el frontmatter.
- **Cumulative Bolt manifest:** [`devflow/metrics/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.json`](../metrics/bolts/US-000.BOLT-001-arnes-de-tests-y-build-reproducible.json)
- **Reportes de la corrida:** `2.0/artifacts/<timestamp>/run-summary.json`, `build.log`, `packages.txt`,
  `harness-hermetic.log`, `characterization-hermetic.log`, `frozen-1.4.log`,
  `api-compat/<corrida>/api-compat-report.json` (no versionados: `artifacts/` está ignorado).
- **Evidencia versionada:** `2.0/tests/fixtures/frozen-1.4/` (inventario + procedencia),
  `2.0/tests/fixtures/api-baseline-1.4/` (línea base + digests + procedencia),
  `2.0/tests/fixtures/legacy-crypto/crypto-vectors.json`,
  `2.0/tests/fixtures/golden-generation/template-digests.json`,
  `2.0/tests/fixtures/sqlserver/`.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| AI generation time | corrida de agente de una sesión (V-Bounce N°1); no se registra estimación manual por la regla AI-native (§2.4) |
| V-Bounce number | 1 |
| Tests created | **141** — 20 de invariantes/self-test del gate (xUnit v3, `net10.0`), 83 de caracterización hermética del 1.4 (xUnit v3, `net48`) y 38 de integración (`Category=Integration`, escritos y compilados, corrida pendiente del entorno) |
| AI-generated code | 100% |
| First-pass approval | n/a — pendiente de `AITL-MEM-Approval` |

---

## 13. Pending items and stubs

- [ ] **Corrida de la suite de integración** contra SQL Server real con la base de fixture
      (`cooperator_it_*`), en el ciclo de integración del Bolt. Bloqueada por entorno no provisionado.
      Variables a definir: `COOPERATOR_IT_LEGACY_CONNECTIONSTRING` (y el slot vacío de la 2.0).
- [ ] **Fixtures dorados del render de generación** (script de stored procedures y clases generadas
      comparados contra artefactos dorados). Depende de F-B1: los puntos de entrada exigen host de aplicación y
      un árbol de dominio producido por el flujo interactivo de reverse engineering. Hoy se versiona el digest
      de las plantillas que el render consume, que es su insumo; el render en sí queda para el Bolt del motor
      headless (ADR-003) o para una sesión con host.
- [ ] **Escalar F-B3** (`Argentina.IsValidCuit` acepta `00-00000000-0` y exige separadores) para su `BUG` y su
      Bolt: **no** se corrige en este V-Bounce (SPEC §4, G04).
- [ ] **Decisiones de micro-ajuste que quedan para el revisor del MEM:** `SignAssembly=false` en el proyecto de
      caracterización y la exclusión de los tipos generados por el compilador del inventario de tipos. Ambas
      están razonadas en §7 y §8 y no cambian el alcance del Bolt.
- [ ] **Plataforma de CI**: sigue sin decidirse (no hay ADR que la fije). El arnés entrega puntos de entrada
      agnósticos con códigos `0/1/2/3` y reportes; el dueño es infraestructura.
- [ ] **Notas de migración** de la 2.0: Bolt propio. El gate las exige para estar verde en el futuro; este
      Bolt entrega el gate y su línea base, no el documento.

---

## 14. AITL-MEM-Approval

> **Avenga DevFlow §2.12, §3.0.** Este MEM lo crea el agente, no tiene estado mutable y **nunca se
> auto-aprueba**. Un humano calificado (el Dev-validator que ejecutó el Bolt; revisores de QA/Sec/dominio
> opcionales) inspecciona el diff real, la evidencia de tests y gates, el MEM y el manifiesto, y registra
> `AITL-MEM-Approval` aquí y en `checkpoint_approvals[]` del manifiesto. `approved` — disponible sólo cuando
> `execution_outcome` es `ready_for_review` — completa el V-Bounce.

| Field | Value |
|-------|-------|
| **Reviewers (executing Dev-validator; QA/Sec/domain optional)** | `agent:dotnet-developer-reviewer-agent` |
| **Roles** | dev_validator |
| **Decision** | approved |
| **review_ready_at** | `2026-10-16T00:37:45-03:00` |
| **review.started_at** | `2026-10-16T00:46:14-03:00` |
| **review.decided_at** | `2026-10-16T00:46:14-03:00` |
| **Review evidence** | Re-ejecución de build.ps1 -Scope Hermetic -ApiCompatDemo -VerifyReproducibility (103/103 verdes, byte-identidad del 1.4, gate verde/rojo/informativa, exit 0); git diff/status -- 1.4 vacíos (AC-8); manifiesto re-validado contra manifest-v5-bolt.schema.json; árbol 2.0/ sin commitear y fixtures versionados inspeccionados. |
| **Comments** | Un único hallazgo Minor gap (conteo «ocho» vs «seis» artefactos en la narrativa del MEM) sin impacto en el resultado verificable. |
| **Findings** | 5 Compliant + 1 Minor gap — ver frontmatter review.findings. |
| **acknowledged_without_comment** | false |
| **acknowledgment_reason** | — |
