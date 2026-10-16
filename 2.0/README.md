# Cooperator 2.0 — árbol de la migración

Árbol **paralelo** a `1.4/`, que es **sólo lectura** en todo el ciclo de migración.
Este README documenta la invocación, la matriz de proyectos/TFMs que fija
[ADR-001](../devflow/adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) §3.1, qué
constructor usar en cada subconjunto y cómo leer los reportes.

> Estado: el árbol nace en el Bolt `US-000.BOLT-001` (arnés de tests, build
> reproducible y gate de API). Los cuatro proyectos de `src/` son **proyectos sin
> fuentes**: fijan TFM, firma, versión y política de dependencias, y son el punto de
> montaje de los Bolts de migración. **No contienen código de producto todavía.**

---

## Comando reproducible único

```powershell
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
```

Parámetros:

| Parámetro | Valores | Efecto |
|-----------|---------|--------|
| `-Scope` | `Hermetic` (default) · `Integration` · `All` | Qué suite corre. `Hermetic` filtra `Category!=Integration`. |
| `-Configuration` | `Release` (default) · `Debug` | Configuración de la solución 2.0. |
| `-ApiCompatDemo` | switch | Corre las tres corridas del gate de compatibilidad de API (verde, roja, informativa). |
| `-VerifyReproducibility` | switch | Corre la doble corrida del build del 1.4 congelado (fase B). |
| `-ProduceBaseline` | switch | Con `-VerifyReproducibility`: además produce la línea base 1.4 versionada. |
| `-EmitCharacterizationFixtures` | switch | Con `-VerifyReproducibility`: además emite los fixtures de caracterización (vectores de cifrado y digests de plantillas). |

Todos los reportes quedan en `2.0/artifacts/<timestamp>/` (ignorado por git: la
evidencia versionada vive en `tests/fixtures/**`). El script escribe `run-summary.json`
con el resultado de cada paso.

### Códigos de salida

Vocabulario compartido con el migrador y con el CLI del motor
(ADR-002 §3.2 / ADR-003 §3.2), de modo que un pipeline futuro los trate igual:

| Código | Significado |
|--------|-------------|
| `0` | ok |
| `1` | error de build, de herramienta o de suite |
| `2` | fallo de gate (diferencias de API, o el build del 1.4 no reproducible) |
| `3` | bloqueado por prerequisito o entorno |

---

## Toolchain fijado

| Componente | Valor | Dónde se fija |
|------------|-------|---------------|
| .NET SDK | **10.0.402**, `rollForward: disable` | `2.0/global.json` |
| Constructor de la solución 2.0 completa | **MSBuild de Visual Studio**, vswhere con rango `[18.11.0,19.0)` | `2.0/build/frozen-source.json`, resuelto por `build.ps1` |
| Targeting pack | **.NET Framework 4.8** | requisito de la máquina, verificado por `build.ps1` y `frozen-1.4.ps1` |
| Comparador de API | **Microsoft.DotNet.ApiCompat 10.0.402** | `2.0/.config/dotnet-tools.json` |
| Restore | `RestorePackagesWithLockFile=true` + `packages.lock.json` versionados | `Directory.Build.props` |

### ¿Por qué MSBuild de Visual Studio y no `dotnet build`?

El proyecto de caracterización referencia los `.csproj` **classic** del 1.4 por
`ProjectReference` —el mismo patrón que ADR-002 §3.2 fija para el migrador— y
`dotnet build` no resuelve esa cadena de destino. `build.ps1` orquesta los dos
mundos: MSBuild para construir, `dotnet` para restaurar herramientas, restaurar
paquetes del subconjunto moderno y correr los tests. El subconjunto moderno
(portables + suite de invariantes) sí es construible con `dotnet build`.

---

## Matriz de proyectos y TFMs (ADR-001 §3.1)

### Portables de producto — `src/`

| Proyecto | TFM | Firma | Dependencias de terceros |
|----------|-----|-------|--------------------------|
| `Cooperator.Framework.Core` | `netstandard2.1` | `CooperatorFramework.snk` | **0** |
| `Cooperator.Framework.Library` | `netstandard2.1` | `CooperatorFramework.snk` | **0** |
| `Cooperator.Framework.Data` | `netstandard2.1` | `CooperatorFramework.snk` | **0** (el provider lo fija el Bolt de ADR-005 §3) |
| `Cooperator.Framework.Modeling` | `netstandard2.1` | `CooperatorFramework.snk` | `System.Text.Json` |

Los cuatro se crean **sin fuentes** y con `IsPackable=true`, versión `2.0.0` y
`AssemblyVersion` fijo `2.0.0.0` (ADR-001 §3.3). La copia de `CooperatorFramework.snk`
es **byte-idéntica** a la del 1.4 (`Core/CooperatorFramework.snk`), y la suite de
invariantes lo verifica: es la identidad de firma de la línea 2.0.x.

### Arnés de verificación — `tests/`

| Proyecto | TFM | Qué verifica |
|----------|-----|--------------|
| `Cooperator.Harness.Tests` | `net10.0` | Invariantes del build centralizado (AC-6) y **self-test del gate de API** (AC-5). No referencia el 1.4. |
| `Cooperator.Framework.Characterization.Tests` | `net48` | Comportamiento observable del 1.4 congelado (áreas `Contracts/` · `Surface/` · `Cryptography/` · `TaxId/` · `Generation/`) y la suite `Category=Integration` (`Integration/`). Referencia los proyectos del 1.4 **tal como están**. |
| `tests/fixtures/api-compat-fixtures/` | `netstandard2.1` | Par sintético baseline/roto que prueba el gate de forma determinista y hermética. |

Los proyectos de **tooling** (`Cooperator.Framework.CodeGeneration`, `Cooperator.Cli`)
y los de **aplicación** (`CooperatorModeler`, `ConnectionStringEncriptor`) **no** se
materializan aquí: sus identificadores, contenidos y decisiones pertenecen a
ADR-003/ADR-009 y a sus Bolts. Crearlos en este Bolt pre-decidiría lo ajeno.

---

## Evidencia versionada — `tests/fixtures/`

| Carpeta | Qué es | Quién la produce |
|---------|--------|------------------|
| `frozen-1.4/` | Inventario de identidad del build reproducible del 1.4 (nombre, `AssemblyVersion`, `FileVersion`, `PublicKeyToken`, SHA-256) y su procedencia, incluida la divergencia de los binarios vendorizados de `Templates/Assemblies` | `pwsh -File 2.0/build/frozen-1.4.ps1` |
| `api-baseline-1.4/` | Línea base 1.4 del gate (cuatro ensamblados del framework) + `PROVENANCE.md` + `baseline-digests.json` | `pwsh -File 2.0/build/frozen-1.4.ps1 -ProduceBaseline` |
| `legacy-crypto/` | Vectores heredados de cifrado: texto cifrado por el build 1.4 y su texto claro esperado. **Sin material de clave** | `… -EmitCharacterizationFixtures` |
| `golden-generation/` | Digests del contenido de las plantillas de generación del 1.4 | `… -EmitCharacterizationFixtures` |
| `sqlserver/` | `schema.sql` + `seed.sql`: esquema y datos sintéticos del ciclo de integración | versionado a mano |

---

## Gate de compatibilidad de API (ADR-008 §5)

```powershell
pwsh -File 2.0/build/api-compat.ps1 -Mode Baseline      # verde: baseline 1.4 ↔ build 1.4 reproducible
pwsh -File 2.0/build/api-compat.ps1 -Mode FixturePair   # roja: par sintético con ruptura deliberada
pwsh -File 2.0/build/api-compat.ps1 -Mode Current       # informativa: baseline 1.4 ↔ esqueleto 2.0
```

| Modo | `contract` (izquierda) | `implementation` (derecha) | Resultado esperado | Código |
|------|------------------------|----------------------------|--------------------|--------|
| `Baseline` | línea base 1.4 | ensamblados del build 1.4 reproducible | 0 diferencias | `0`, `2` si hay diferencias |
| `FixturePair` | fixture sintético "bueno" | fixture sintético con la ruptura | **debe fallar** nombrando las dos rupturas | `2` |
| `Current` | línea base 1.4 | ensamblados vacíos del esqueleto 2.0 | brecha completa (informativa) | `2` |

`-BaselineDirectory <ruta>` permite apuntar a una copia del baseline (útil para probar
la guarda anti-manipulación). `-Contract`/`-Implementation` aceptan rutas explícitas.

**El gate no autoriza rupturas** (ADR-008 §4): una diferencia no documentada falla, y
lo que la vuelve documentable son las notas de migración, que son un Bolt propio.
`EnablePackageValidation` está cableado en `Directory.Build.targets` para los proyectos
empaquetables, pero `PackageValidationBaselineVersion` queda **sin valor**: 2.0.0 es la
primera versión y no hay línea anterior que comparar. Su activación es un acto de la
primera release 2.0.x.

---

## Suite de integración (dependencias reales)

Los casos `Category=Integration` corren **sólo** en el ciclo de integración del Bolt.
Resuelven la conexión por **variable de entorno**, nunca por un archivo versionado:

| Variable | Superficie | Estado |
|----------|------------|--------|
| `COOPERATOR_IT_LEGACY_CONNECTIONSTRING` | 1.4 | usado por esta suite |
| `COOPERATOR_IT_TARGET_CONNECTIONSTRING` | 2.0 | **reservado y vacío**: todavía no hay código 2.0 que verificar |

**Guarda dura:** el arnés sólo opera contra una base cuyo nombre contenga `cooperator_it`
y aborta en cualquier otro caso. La corrida hermética **excluye** estos casos por filtro
de categoría: no hay `Skip`, `waiting for an environment` no consume presupuesto de turnos.

```powershell
pwsh -File 2.0/build/build.ps1 -Scope Integration
```

---

## Cómo leer los reportes

| Archivo en `artifacts/<timestamp>/` | Qué contiene |
|-------------------------------------|--------------|
| `run-summary.json` | Resultado de cada paso con su código de salida, toolchain usado y `outcome` de la corrida |
| `build.log`, `restore.log`, `tool-restore.log` | Salida del constructor y del restore |
| `packages.txt` | `dotnet list package --include-transitive` por proyecto (evidencia de dependencias y del conteo de terceros por proyecto) |
| `harness-hermetic.log`, `characterization-hermetic.log` | Resultado de cada suite hermética |
| `characterization-integration.log` | Resultado del ciclo de integración (o su bloqueo por entorno) |
| `frozen-1.4.log` | Doble corrida del build del 1.4 con `-VerifyReproducibility` |
| `api-compat/<corrida>/api-compat-report.json` | Reporte legible por máquina del gate: modo, herramienta fijada, diferencias una por una, digests y códigos de salida |

Los reportes del gate en JSON son el contrato estable: `status`, `differences[]`,
`difference_count`, `comparator_exit_code`, `tool_version`, `strict_mode`.

---

## Regla dura: `1.4/**` no se toca

`1.4/**` es **sólo lectura** en todo el ciclo. `frozen-1.4.ps1` verifica antes de
construir que el árbol coincide con el commit congelado (`git diff --quiet <commit> -- 1.4`
y `git status --porcelain -- 1.4` vacíos) y **aborta con código 3** si no es así; nunca
restaura ni edita nada por su cuenta. La suite de invariantes verifica la misma condición
al final del V-Bounce.
