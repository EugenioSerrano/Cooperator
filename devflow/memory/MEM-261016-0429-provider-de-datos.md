---
id: "MEM-261016-0429"
title: "Provider de datos y capa `Data` (SPEC rev.4) - V-Bounce N°3: la unidad completa y la suite hermética en verde (pruebas de §8.1, lane de integración de §8.2 escrita y compilada, constancia de rupturas, las dos invariantes autorizadas); READY FOR REVIEW"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> - the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null - never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-014" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN - the Bolt this V-Bounce executed
spec: "SPEC-261016-0020" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 4 # revision of the canonical SPEC used by this V-Bounce (rev.4: turn_budget 25 y el retiro del caché del host EN ALCANCE, §C.6)
v_bounce: 3 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled - describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-005", "ADR-004", "ADR-001", "ADR-008"] # ADR-005 §3.1/§3.2/§3.3/§3.4/§5 (provider único, punto único, interior tipado, hazard, NFRs), ADR-004 §3.2/§3.3 (catálogo del timeout y cadena resuelta sin inspección), ADR-001 §3.1/§3.4/§5 (TFM, presupuesto de dependencias, portabilidad sin la plataforma web), ADR-008 §3.1/§3.2 (registro de rupturas: nada se retira sin declararse)
manifest: "US-000.BOLT-014-provider-de-datos.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only - no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T04:29:00-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval - filled by the human reviewer (§3.0). El ejecutor NO se auto-aprueba.
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T04:35:38-03:00"
  decided_at: "2026-10-16T04:35:38-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°3 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0020` revisión 4 gobernante, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia)."
    - "Compliant — corrida hermética VERDE reproducida (`pwsh 2.0/build/build.ps1 -Scope Hermetic`): 21 proyectos sin advertencias (tratadas como error), 135 arnés + 12 mapeo + 11 data-hermetic + 11 encriptor + 83 caracterización = 252 casos, exit 0. `migration-notes.ps1` regenerado → 74 entradas de 10 fragmentos, «sin cambios (consolidación determinista)»."
    - "Compliant — constancia verificada: 4 entradas (`BOLT014-01` method-removed `SetConnectionStringValue` + 3 behavior-change) + 2 obligaciones (`BOLT014-OBL-01` integración, `BOLT014-OBL-02` ConfigurationManager transitivo); cierra las 2 diferencias de cobertura (SetConnectionStringValue era la última); `1.4/**` intacto."
    - "ADJUDICACIÓN (provider 5.2.2, §8.2) — ENDOSO, con la constancia de los avisos: ADR-005 §3.1 autoriza «5.x o 6.1+, nunca 6.0» (5.2.2 es 5.x); el cambio evita 2 avisos de seguridad del transitivo `System.Security.Cryptography.Pkcs 9.0.11` de la línea 6.x (GHSA-2fmv-rv2c-59x7 moderada + GHSA-846f-f764-76mx alta); el smoke real (criterio 2 de §3.1) sigue pendiente del ciclo de integración; la prohibición de la 6.0 se respeta. El porqué del cambio de línea queda registrado en el MEM §8.2 y esta constancia."
    - "Compliant — las 2 invariantes (alcance exacto respetado, verificado por lectura): `Data_declares_zero_third_party_dependencies` → `Assert.Equal(["Microsoft.Data.SqlClient"], …)` (exactamente el provider autorizado) y `No_lock_file_pulls_configuration_manager` → prohibida la declaración Direct + permitida la Transitive (inspecciona el `type` de cada dependencia del lock), ambas con comentario de procedencia + referencia cruzada al MEM-261016-0407 §8.3; nada más se tocó del arnés. Transparencia menor: el nombre del método conserva «zero» aunque ya no aserta cero — cosmético, no defecto."
    - "Compliant — desviaciones: (a) presupuesto §8.1: 27/25 con verde, declarado con causa medida (4 hallazgos antes de la primera corrida completa) y sin residuo — la condición de parada del SPEC («no llegar a verde») no se alcanzó; (b) §8.5: el fix del `tokens` de la entrada N°2 (texto → objeto) verificado en el manifiesto — corrección del propio artefacto del ejecutor, no un cambio ajeno."
    - "Compliant — DoD + MEM completo (fases F1–F6, traceability AC-1…AC-10, §9.1–§9.7) + manifiesto válido (4 entradas v_bounces, spec_revisions 1–4). `test_integration: required` → el ciclo de integración queda pendiente (entorno no provisionado)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0429 - Provider de datos y capa `Data` (rev.4) - V-Bounce N°3 **cerrado con la unidad completa**

## 1. Executive summary

El V-Bounce N°3 cierra la unidad que el N°2 dejó aportada y compilando. En una sola pasada
se entregaron las cuatro piezas pendientes y la suite hermética quedó **VERDE**:

1. **Las pruebas de §8.1** (`Cooperator.Framework.Data.Tests`, 11 casos): el punto único de
   conversión con sus negativos tipados por tipo, el timeout y la cadena por el contrato, el
   armado de parámetros de los tipos que cambian de comportamiento, y los **controles NFR
   search-based** (0 casts fuera del punto, 0 esperas activas, 0 `providerName`, 0 rastro del
   caché del host, dependencias declaradas, política de nulabilidad del aporte).
2. **La lane de integración de §8.2** (`Cooperator.Framework.Data.Integration.Tests`): los
   cuatro archivos, **escritos y compilados**, enmarcados en `Category=Integration` e integrados
   al comando reproducible como la suite `data-integration`. **Su corrida pertenece al ciclo de
   test de integración del Bolt**, después de `AITL-MEM-Approval` (SPEC §5 y §8.2).
3. **La constancia** `2.0/docs/api-removals/US-000.BOLT-014.json`: 4 entradas (la ruptura de
   miembro retirado y tres cambios de comportamiento) y 2 obligaciones registradas. Es la que
   cierra los **dos fallos de cobertura** del arnés y el insumo de las notas de migración.
4. **Las dos aserciones de `BuildInvariantsTests`** que el aporte desactualizó, ajustadas
   **con el alcance exacto** que el revisor autorizó (adjudicación 2): `Data` declara
   exactamente `Microsoft.Data.SqlClient`; y la prohibición de
   `System.Configuration.ConfigurationManager` se aplica a la **declaración directa**, sin
   fallar por su aparición **transitiva** en el lock del provider autorizado.

Resultado medido: `pwsh 2.0\build\build.ps1 -Scope Hermetic` → **EXIT=0**, **0 advertencias**
(tratadas como error) sobre 21 proyectos, **252 casos verdes**: 135 arnés · 12 mapeo · 11
encriptor · 11 datos · 83 caracterización.

Estado: **`ready_for_review`**. Pausa en `AITL-MEM-Approval`; el ejecutor no lo aprueba.

## 2. Implemented phases

| Fase | Qué se hizo | Evidencia |
| --- | --- | --- |
| F1 - Pruebas de §8.1 | Proyecto `Cooperator.Framework.Data.Tests` (11 casos): `TransactionAdapterTests` (negativos por tipo con dobles de `DbTransaction`/`DbConnection`) y `DataLayerContractTests` (timeout y cadena por el contrato, NFR search-based, nulabilidad, dependencias, parámetros) | §9.1 |
| F2 - Lane de integración de §8.2 | Proyecto `Cooperator.Framework.Data.Integration.Tests` (base + 4 archivos), **compilado**; la cadena la aporta el entorno por variable, nunca el repositorio | §9.2 |
| F3 - Constancia de rupturas | `2.0/docs/api-removals/US-000.BOLT-014.json` conforme al esquema publicado por el BolT 028; cierra las 2 diferencias sin cobertura | §9.3 |
| F4 - Las dos invariantes del arnés | `BuildInvariantsTests`: la de `Data` y la del lock, **sólo esas dos**, con procedencia y referencia cruzada | §9.4 |
| F5 - Corrida hasta verde | Suite hermética completa: EXIT=0 | §9.5 |
| F6 - Cierre | MEM + entrada `v_bounces[3]` del manifiesto; pausa en el checkpoint | §9.6 |

## 3. Files created

- `2.0/tests/Cooperator.Framework.Data.Tests/Cooperator.Framework.Data.Tests.csproj`
- `2.0/tests/Cooperator.Framework.Data.Tests/GlobalUsings.cs`
- `2.0/tests/Cooperator.Framework.Data.Tests/TransactionAdapterTests.cs`
- `2.0/tests/Cooperator.Framework.Data.Tests/DataLayerContractTests.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/Cooperator.Framework.Data.Integration.Tests.csproj`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/GlobalUsings.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/IntegrationTestBase.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/ProviderSmokeTests.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/StoredProcedureMetadataTests.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/CommandTimeoutIntegrationTests.cs`
- `2.0/tests/Cooperator.Framework.Data.Integration.Tests/ProviderBehaviorTests.cs`
- `2.0/docs/api-removals/US-000.BOLT-014.json` (la constancia)

## 4. Files modified / aportados

- `2.0/Directory.Packages.props` - la línea del provider pasó de `6.1.7` a **`5.2.2`** (ver §8.2:
  la línea 6.x arrastra un transitivo con dos avisos de vulnerabilidad y el audit de NuGet es error).
- `2.0/build/build.ps1` - se agregaron las suites `data-hermetic` (filtro por defecto) y
  `data-integration` (`-Filter 'Category=Integration' -Integration`), siguiendo el patrón de los
  precedentes. **Sin** cambios en el resto del contrato del arnés.
- `2.0/Cooperator2.sln` - alta de los dos proyectos de prueba (el archivo conserva su BOM: es
  formato del tooling, fuera de la política de §3.15).
- `2.0/src/Cooperator.Framework.Data/SQLHelper.cs` y `BaseGateway.cs` - las **6 conversiones** que
  quedaban (`(SqlTransaction)`/`(SqlConnection)`) pasaron al punto único
  `TransactionAdapter.AsSqlTransaction/AsSqlConnection`, con su `using` del namespace `Casting`.
  Sólo cuerpos de método: **0 cambios de superficie** (ver §8.3).
- `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs` - las dos aserciones autorizadas.
- `2.0/docs/MIGRATION-NOTES.md` - regenerado por `migration-notes.ps1`: **74 entradas de 10
  fragmentos**.

## 5. Files deleted / 6. Files renamed

Ninguno. `1.4/**` no se tocó (ver §9.7).

## 7. Implementation decisions

- **La línea del provider se elige por medición, no por gusto.** Con `6.1.7`, el audit de NuGet
  reporta `System.Security.Cryptography.Pkcs 9.0.11` (transitivo de la línea 6.x) con un aviso
  *moderate* y otro *high*: como el árbol trata las advertencias como error, el build no pasa.
  `5.2.2` -la otra línea que ADR-005 §3.1 permite- deja el grafo limpio y con un footprint de
  dependencias mucho menor. La prohibición de la 6.**0** se respeta.
- **El interior se ejerce por reflexión, no por `InternalsVisibleTo`.** Los ensamblados 2.0 están
  **firmados** (ADR-001 §3.3): `InternalsVisibleTo` exigiría la clave pública en el `csproj` y
  ensancharía innecesariamente el contrato del ensamblado. Las suites alcanzan `SqlHelper` y
  `SqlHelperParameterCache` por `typeof(BaseRule).Assembly.GetType(...)`: se prueba la
  implementación **sin** abrir el ensamblado.
- **Los controles NFR leen el CÓDIGO, no los comentarios.** Un comentario que *nombra* lo
  prohibido no es un uso: `Source()` recorta comentarios de línea, de bloque y XML, y
  `RawSource()` (archivo crudo) se usa sólo para la política de nulabilidad, cuya directiva **es**
  un comentario. Los tokens prohibidos se **ensamblan desde fragmentos** en el propio test, para
  que el test no sea él mismo una ocurrencia de lo que persigue.
- **La cadena y el timeout se prueban con dobles del contrato.** El timeout de la capa vive en un
  miembro **privado** (es implementación, no superficie): la prueba lo invoca por reflexión con un
  doble de `ICooperatorConfiguration` que entrega un valor, el default documentado del catálogo, o
  el error del contrato -que es lo que AC-4 exige que se propague sin fallback propio-.
- **La constancia ancla sus citas a cláusulas que el mapa declara.** El registro de rupturas
  exige que toda entrada de comportamiento cite una decisión vigente cuya cláusula exista **y**
  esté declarada (≠ `none`) en el mapa. La entrada del caché cita **ADR-004 §3.1** (el mapa la
  declara `behavior_change`), no la cláusula de compatibilidad que el mapa declara `none`.

## 8. Deviations and assumptions

### 8.1 Presupuesto de turnos: excedido y declarado

El SPEC fija `turn_budget: 25` y este V-Bounce consumió **más** (la corrida a verde llegó en el
turno 27). La desviación se declara con su causa medida: **el presupuesto se gastó antes de la
primera corrida completa**, no después. Cuatro hallazgos que ninguna lectura del SPEC anticipaba
-consumidos, en orden, antes de tener una suite ejecutable-: (a) el colapso de un *here-string*
de PowerShell al arreglar el archivo de arranque; (b) el **audit de NuGet** con dos avisos de
vulnerabilidad en un transitivo del provider (obligó a medir la otra línea); (c) `InternalsVisibleTo`
es **inválido** en un ensamblado firmado (rehacer la lane por reflexión); (d) mi propio reemplazo
de las conversiones pegó el token siguiente (`(SqlTransaction)transaction` → `…)transaction`, seis
puntos) y hubo que recomponerlo con regex. Ninguno dejó residuo: el estado final compila sin
advertencias y pasa 252 casos. **La condición de parada del SPEC (no llegar a verde) no se
alcanzó**: la unidad **sí** cerró en verde, y a partir del turno 27 no se consumió ninguno más
antes de la pausa.

### 8.2 La versión del provider cambió respecto de lo que el N°2 entregó (6.1.7 → 5.2.2)

La decisión del N°2 (6.1.7, la línea "6.1+") era correcta en el papel pero **no sobrevive al
audit** del árbol: el transitivo `System.Security.Cryptography.Pkcs 9.0.11` trae GHSA-2fmv-rv2c-59x7
(moderada) y GHSA-846f-f764-76mx (alta) y el build es *warnings-as-errors*. Se pasó a **5.2.2**,
la otra línea que ADR-005 §3.1 autoriza explícitamente. Si el dueño prefiere la 6.x, el camino
-que **no** es mío- es fijar ese transitivo a una versión parcheada en el mapa de paquetes: queda
registrado como hallazgo, no se resolvió por mi cuenta.

### 8.3 Seis conversiones recomputadas en `SQLHelper.cs`/`BaseGateway.cs` (incidente propio, sin residuo)

Mi reemplazo literal anterior pegó el identificador siguiente al texto sustituido en seis puntos
(`(SqlTransaction)transaction` → `TransactionAdapter.AsSqlTransaction(transaction, …)transaction`).
Se detectó por el **build** (sintaxis) y se recomputó con regex, quedando
`AsSqlTransaction(<identificador>, "<origen>")`. Los seis puntos son **cuerpos de método y
comentarios**: 0 cambios de superficie (el gate de compatibilidad lo confirma, §9.3). Los
comentarios que **nombraban** el proveedor legado y la plataforma web se reformularon para no
mencionarlos: un barrido literal del entregable no encuentra rastro de ninguno de los dos.

### 8.4 Las dos invariantes del arnés: alcance exacto del revisor

Se ajustaron **sólo** las dos aserciones que la adjudicación 2 del `AITL-MEM-Approval` del N°2
autorizó (`MEM-261016-0407` §8.3), cada una con su comentario de procedencia y la referencia
cruzada al MEM y a la constancia. La primera deja de afirmar "cero dependencias de terceros" y
afirma la lista **exacta** (`Microsoft.Data.SqlClient`), que es lo que ADR-005 §3.1/ADR-001 §3.4
exigen; la segunda mantiene prohibida la declaración **directa** de
`System.Configuration.ConfigurationManager` y deja de fallar por su aparición **transitiva** en el
lock del provider autorizado (se inspecciona el `type` de cada dependencia del lock). Nada más se
tocó del arnés.

### 8.5 Corrección de formato en la entrada `v_bounces[2]` del manifiesto

La entrada del N°2 tenía `tokens` serializado como **texto** (`"@{…}"`), un artefacto de la
herramienta con la que la escribí. Se corrigió a objeto (como la del N°1). Es mi propio artefacto
y mi propia ruta de escritura; se declara para que el revisor no lo lea como un cambio ajeno.

### 8.6 G15 - verificación de fuentes gobernadas (sin cambio material)

Se releyeron al inicio del V-Bounce el SPEC (rev.4 `approved`) y el Bolt (`in-development`): sin
cambios materiales respecto de lo que el N°2 ejecutó. Las adjudicaciones del revisor se leyeron del
MEM N°2 §8.3 y se aplicaron con el alcance que declaran. Ninguna fuente gobernada cambió a mitad
de la ejecución.

### 8.7 Supuestos declarados

- El procedimiento de prueba de la lane de integración (`cooperator_test_parameters`) y la forma
  de la cadena los **aporta el entorno del ciclo**; los casos fallan con un mensaje explícito si la
  variable de entorno no está. No hay ninguna cadena ni credencial en el repositorio.
- La cláusula **ADR-005 §3.5** se cita en la constancia porque existe en su ADR; el mapa no la
  declara, y el control sólo exige que **al menos una** de las cláusulas citadas esté declarada
  (lo está, ADR-004 §3.1).

## 9. Verification evidence

### 9.1 Las pruebas de §8.1 corren y pasan

`dotnet test 2.0\tests\Cooperator.Framework.Data.Tests\… -c Release` → **11/11 verdes**:
`A_null_transaction_is_allowed_and_returned_as_null`,
`A_foreign_transaction_is_a_typed_error_with_the_received_and_expected_types`,
`A_foreign_or_missing_connection_is_a_typed_error`,
`The_layer_has_no_host_cache_no_web_platform_and_no_legacy_keys`,
`The_layer_has_no_active_waiting_no_provider_name_and_no_legacy_provider`,
`The_nfr_no_hidden_casts_holds_outside_the_single_conversion_point`,
`The_chain_comes_from_the_contract_without_inspection_and_without_the_static_dictionary`,
`The_command_timeout_comes_from_the_catalog_and_validates_the_value`,
`The_parameter_types_of_the_provider_change_are_declared_at_construction`,
`Data_declares_exactly_the_authorized_provider`,
`The_nullability_directive_lives_only_in_the_aportes_the_build_required`.

### 9.2 La lane de integración está escrita y **compila**

Los 4 archivos + la base entraron en el build de 21 proyectos sin advertencias. La corrida
(conexión real, hazard de la conexión del llamador, timeout aplicado, cifrado y `DbType`)
pertenece al **ciclo** del Bolt. Registrado como obligación en la constancia
(`BOLT014-OBL-01`).

### 9.3 La constancia cierra las dos diferencias sin cobertura

`api-compat.ps1 -Mode Current` reporta para el ensamblado `Data`:
`CP0002: Member 'void Cooperator.Framework.Data.BaseRule.InternalDBHelper.SetConnectionStringValue(string)'
exists on …api-baseline-1.4\Cooperator.Framework.Data.dll but not on …Data.dll`. Era la **única**
diferencia sin cobertura del árbol: la entrada `BOLT014-01` la cubre, y los dos casos que fallaban
(`ApiRemovalCoverageTests.Every_removed_member_reported_by_the_gate_is_covered_by_a_fragment` y
`MigrationNotesCoverageTests.Every_removed_member_of_the_report_is_covered_by_a_fragment`) pasan.
Las otras 182 diferencias siguen cubiertas por los fragmentos de sus Bolts.

### 9.4 Las dos invariantes del arnés pasan

`BuildInvariantsTests`: 135 casos del arnés en verde, incluidos
`Data_declares_zero_third_party_dependencies` (ahora "declara **exactamente** el provider
autorizado") y `No_lock_file_pulls_configuration_manager` (prohibición de la declaración directa).

### 9.5 La suite hermética del árbol: VERDE

```
build sin advertencias (tratadas como error): 21 proyectos
Passed!  - Failed: 0, Passed: 135  - Cooperator.Harness.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  12  - Cooperator.Framework.Core.Mapping.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  11  - ConnectionStringEncriptor.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  11  - Cooperator.Framework.Data.Tests.dll (net10.0)
Passed!  - Failed: 0, Passed:  83  - Cooperator.Framework.Characterization.Tests.exe (net48)
Resultado: ok (codigo de salida 0)
```
`migration-notes.ps1` → `74 entradas de 10 fragmento(s) -> 2.0/docs/MIGRATION-NOTES.md`.

### 9.6 Gates (§9 del SPEC)

`pass` para: build reproducible del árbol (21 proyectos, 0 advertencias), suite hermética verde,
compatibilidad de API cubierta por el registro, documentación de migración regenerada, y los
controles NFR de la capa (search-based) verdes. `n-a` para la corrida de integración (pertenece al
ciclo del Bolt; §8.2 del SPEC lo declara).

### 9.7 `1.4/**` intacto y rutas propias

Ninguna escritura de este V-Bounce tocó `1.4/**` ni el fragmento de otro Bolt. Las escrituras
fueron: `2.0/tests/Cooperator.Framework.Data*.Tests/**`, `2.0/docs/api-removals/US-000.BOLT-014.json`,
`2.0/docs/MIGRATION-NOTES.md` (regenerado), `2.0/Directory.Packages.props`, `2.0/build/build.ps1`,
`2.0/Cooperator2.sln`, los tres archivos de `2.0/src/Cooperator.Framework.Data/`, las dos
aserciones de `2.0/tests/Cooperator.Harness.Tests/BuildInvariantsTests.cs`, y este MEM con su
entrada de manifiesto.

## 10. Manual interventions

Ninguna intervención manual durante este V-Bounce. Las decisiones humanas vigentes son las
adjudicaciones del `AITL-MEM-Approval` del MEM N°2 (`checkpoint_approvals`), que este V-Bounce
obedece con el alcance que declaran.

## 11. References

- `devflow/spec/SPEC-261016-0020-provider-de-datos.md` (rev.4, `approved` 03:53:49): §6/A.1, §6/C.3,
  §6/C.5, §6/C.6, §6/D.2, §6/E.1, §6/E.2, §7, §8.1, §8.2, §8.3, §9, §15.
- `devflow/memory/MEM-261016-0407-provider-de-datos.md` (N°2) §8.2/§8.3/§8.4/§8.5 y su
  `checkpoint_approvals` (adjudicaciones 1, 2 y 3).
- `devflow/memory/MEM-261016-0343-provider-de-datos.md` (N°1) §9.1 (la medición del caché del host).
- `devflow/adrs/`: ADR-001 (layout/empaquetado), ADR-004 (caché y configuración), ADR-005
  (provider de datos), ADR-008 (compatibilidad de API).
- `2.0/docs/api-removals/US-000.BOLT-014.json` (la constancia) y
  `2.0/docs/api-removals/api-removals.schema.json` (esquema publicado por el Bolt 028).
- `2.0/docs/api-removals-decisions.json` (mapa de decisiones y cláusulas) y
  `2.0/docs/MIGRATION-NOTES.md` (documento consolidado).

## 12. Traceability (AC → evidencia)

| AC | Qué exige | Evidencia |
| --- | --- | --- |
| AC-1 | un solo provider ADO.NET; sin el legado, sin `providerName` | `Data_declares_exactly_the_authorized_provider`, control NFR de la capa, `Directory.Packages.props` |
| AC-2 | punto único de conversión, sin casts ocultos, con error tipado | `TransactionAdapterTests` (3 casos) + control NFR de casts + `ProviderSmokeTests` (integración) |
| AC-3 | interior tipado, sin `InvalidCastException` a mitad de operación | `InvalidTransactionTypeException` con tipo recibido/esperado/origen (casos 2 y 3) |
| AC-4 | timeout del catálogo, acotado, sin fallback propio | `The_command_timeout_comes_from_the_catalog_and_validates_the_value` + `CommandTimeoutIntegrationTests` |
| AC-5 | cadena por el contrato, sin inspección ni `Dictionary` estático | `The_chain_comes_from_the_contract_without_inspection_and_without_the_static_dictionary` |
| AC-6 | sin esperas activas; portabilidad sin la plataforma web | control NFR (`Thread.`+`Sleep`, `System.`+`Web`, `Http`+`Context`) |
| AC-7 | el descubrimiento de parámetros no abre ni cierra la conexión del llamador | `StoredProcedureMetadataTests` (integración, escrito y compilado) |
| AC-8 | los cambios de comportamiento del proveedor verificados y documentados | `ProviderBehaviorTests` (integración) + `BOLT014-04` de la constancia + `MIGRATION-NOTES.md` |
| AC-9 | política de nulabilidad del aporte acotada a lo aportado | `The_nullability_directive_lives_only_in_the_aportes_the_build_required` (directiva sólo en los 4 archivos que el build exige; 0 en código nuevo; 0 en `.csproj`) |
| AC-10 | retiro del caché del host (§C.6) | control NFR de la capa + `BOLT014-02`/`BOLT014-03` de la constancia |

## 13. Remaining work (no bloquea esta revisión)

1. **El ciclo de test de integración del Bolt**: correr las 4 clases de la lane contra SQL Server
   real (obligación `BOLT014-OBL-01` de la constancia).
2. **La sustitución de los cuatro puntos de caché** por `ICacheProvider`: es del **US-000.BOLT-013**,
   que ejecuta después (secuencia `014-ampliado → 013`). Este V-Bounce **no** la introduce ni crea
   una caché interina propia: lo deja declarado con su paso siguiente en la constancia.
3. **La cifra de `Cooperator:Cache:MaxEntries`** sigue abierta y es del dueño del Bolt 013.
4. **La aparición transitiva de `System.Configuration.ConfigurationManager`** en el lock del
   provider autorizado: queda registrada (`BOLT014-OBL-02`) por si el dueño quiere revisarla de
   fondo en el Bolt del arnés.
5. **La línea del provider (5.2.2 vs 6.1.x parcheada)**: decisión del dueño (§8.2 del MEM N°2 quedó
   adjudicada; esta es nueva y se declara aquí, no se resuelve por mi cuenta).

## 14. Sign-off readiness

El entregable de la unidad está completo y verificado de forma hermética: código, pruebas de §8.1,
lane de integración escrita y compilada, constancia de rupturas y las dos invariantes autorizadas.
**Listo para `AITL-MEM-Approval`.** Lo que el revisor debería mirar primero: (a) la constancia y sus
cuatro entradas frente al reporte del gate; (b) las dos aserciones del arnés y que el alcance no se
excedió; (c) el cambio de línea del provider (§8.2) y su registro; (d) las tres declaraciones del
N°2 que este V-Bounce materializó (`SetConnectionStringValue`, `internal` ya en 1.4, nulabilidad
por el build).
