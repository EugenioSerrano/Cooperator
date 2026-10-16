#requires -Version 5.1
<#
.SYNOPSIS
    Gate de compatibilidad de API de Cooperator 2.0 (ADR-008 §5, SPEC-261016-0000 §6.D).

.DESCRIPTION
    Compara un lado CONTRACT (la línea base 1.4) contra un lado IMPLEMENTATION
    (los ensamblados del 1.4 reproducible, o los del esqueleto 2.0) en modo
    estricto, usando el comparador que el ADR nombra — Microsoft.DotNet.ApiCompat —
    resuelto por el manifiesto de herramientas versionado `2.0/.config/dotnet-tools.json`.

    Modos:
      Baseline     — corrida VERDE (AC-4): baseline 1.4 ↔ build 1.4 reproducible.
                     Es el control que prueba que el pipeline funciona de punta a
                     punta y que el artefacto de referencia representa fielmente
                     lo que el 1.4 congelado produce. Espera 0 diferencias.
      FixturePair  — corrida ROJA (AC-5): par de fixtures sintéticos versionados
                     con una ruptura deliberada (un miembro público retirado y una
                     firma alterada). El gate DEBE fallar NOMBRANDO ambas rupturas.
                     Es hermética: no requiere el build del 1.4.
      Current      — corrida INFORMATIVA: baseline 1.4 ↔ ensamblados vacíos del
                     esqueleto 2.0. Reporta la brecha completa (todo miembro del
                     baseline ausente). NO es un resultado de gate: es la evidencia
                     de la forma del insumo que las notas de migración deberán cubrir.

    Contrato de salida (vocabulario compartido con el migrador y el CLI del motor,
    ADR-002 §3.2 / ADR-003 §3.2):
      0 = ok (sin diferencias, o rupturas esperadas identificadas)
      1 = error de ejecución del gate
      2 = diferencias de API (fallo de gate)
      3 = bloqueado por prerequisito o entorno (baseline ausente o manipulado,
          ensamblados del lado de implementación ausentes, herramienta no restaurada)

    Integridad del baseline: los digests de `baseline-digests.json` se verifican
    ANTES de comparar; un baseline alterado NO se compara, se reporta como bloqueo (3).

.PARAMETER Mode
    Baseline | FixturePair | Current.

.PARAMETER FixturePair
    Atajo de `-Mode FixturePair` (el nombre que el SPEC §6.D.1 fija).

.PARAMETER Contract
    Rutas explícitas del lado contract. Sustituye al conjunto que el modo resolvería.

.PARAMETER Implementation
    Rutas explícitas del lado implementation. Sustituye al conjunto del modo.

.PARAMETER OutputDirectory
    Directorio de reportes. Por defecto `2.0/artifacts/<timestamp>/api-compat`.

.EXAMPLE
    pwsh -File 2.0/build/api-compat.ps1 -Mode FixturePair
    pwsh -File 2.0/build/api-compat.ps1 -Mode Baseline -VerifyBaselineDigests
#>
[CmdletBinding()]
param(
    [ValidateSet('Baseline', 'FixturePair', 'Current')]
    [string]$Mode = 'Baseline',

    [switch]$FixturePair,

    [string[]]$Contract,
    [string[]]$Implementation,

    [string]$OutputDirectory,
    [string]$RepositoryRoot,
    [string]$BaselineDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Invocación de comandos nativos robusta en Windows PowerShell 5.1 y en
# PowerShell 7.x: la salida de stderr de un ejecutable no debe convertirse en
# un error terminante (el comparador escribe su diagnóstico en stderr y su
# veredicto viaja en el CÓDIGO DE SALIDA, que es el contrato del gate).
function Invoke-Native {
    param([string]$Command, [string[]]$Arguments)
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $hadNativePref = Get-Variable -Name PSNativeCommandUseErrorActionPreference -ErrorAction SilentlyContinue
    if ($hadNativePref) { $PSNativeCommandUseErrorActionPreference = $false }
    try {
        $output = & $Command @Arguments 2>&1
        $code = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $prevEap
    }
    # Normalización a texto: en PowerShell 7 la salida de stderr llega como
    # ErrorRecord cuando se fusiona con `2>&1`; el contrato del gate es texto.
    $text = foreach ($line in @($output)) { if ($null -eq $line) { continue } else { "$line" } }
    return @{ Output = @($text); ExitCode = $code }
}

# --- Vocabulario de códigos de salida (ADR-002 §3.2, ADR-003 §3.2) -----------
$EXIT_OK = 0
$EXIT_ERROR = 1
$EXIT_DIFFERENCES = 2
$EXIT_BLOCKED = 3

if ($FixturePair) { $Mode = 'FixturePair' }

if (-not $RepositoryRoot) {
    # <repo>/2.0/build/api-compat.ps1 -> <repo>
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
$TreeRoot = Join-Path $RepositoryRoot '2.0'

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $TreeRoot "artifacts\$stamp\api-compat\$($Mode.ToLowerInvariant())"
}
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$reportJsonPath = Join-Path $OutputDirectory 'api-compat-report.json'
$reportTextPath = Join-Path $OutputDirectory 'api-compat-report.txt'
$toolLogPath = Join-Path $OutputDirectory 'apicompat-tool-output.txt'

function Write-Report {
    param(
        [string]$Status,
        [string]$Summary,
        [string[]]$Differences,
        [string[]]$Notes,
        [string]$ContractSide,
        [string]$ImplementationSide
    )
    $report = [ordered]@{
        tool                  = 'Microsoft.DotNet.ApiCompat'
        tool_version          = $script:toolVersion
        mode                  = $Mode
        status                = $Status            # pass | differences | blocked | error
        contract_side         = $ContractSide
        implementation_side   = $ImplementationSide
        contract_assemblies   = @($script:contractResolved)
        implementation_assemblies = @($script:implementationResolved)
        strict_mode           = $true
        differences           = @($Differences)
        difference_count      = @($Differences).Count
        comparator_exit_code  = $script:comparatorExitCode
        notes                 = @($Notes)
        generated_at          = (Get-Date -Format 'o')
        repository_root       = $RepositoryRoot
    }
    $report | ConvertTo-Json -Depth 6 | Set-Content -Path $reportJsonPath -Encoding UTF8

    $text = New-Object System.Text.StringBuilder
    [void]$text.AppendLine("Cooperator API-compat gate — modo $Mode")
    [void]$text.AppendLine("herramienta : Microsoft.DotNet.ApiCompat $script:toolVersion (manifiesto 2.0/.config/dotnet-tools.json)")
    [void]$text.AppendLine("contract    : $ContractSide")
    [void]$text.AppendLine("implementation: $ImplementationSide")
    [void]$text.AppendLine("resultado   : $Status — $Summary")
    [void]$text.AppendLine("diferencias : $((@($Differences)).Count)")
    foreach ($d in @($Differences)) { [void]$text.AppendLine("  - $d") }
    foreach ($n in @($Notes)) { [void]$text.AppendLine("nota        : $n") }
    [void]$text.AppendLine("reporte json: $reportJsonPath")
    $text.ToString() | Set-Content -Path $reportTextPath -Encoding UTF8

    Write-Host $text.ToString().TrimEnd()
    Write-Host "REPORT: $reportJsonPath"
}

function Fail-Blocked {
    param([string]$Summary, [string[]]$Notes, [string]$ContractSide = '', [string]$ImplementationSide = '')
    Write-Report -Status 'blocked' -Summary $Summary -Differences @() -Notes $Notes `
        -ContractSide $ContractSide -ImplementationSide $ImplementationSide
    exit $EXIT_BLOCKED
}

$script:contractResolved = @()
$script:implementationResolved = @()
$script:toolVersion = 'unknown'
$script:comparatorExitCode = $null

# --- 1. Herramienta fijada por manifiesto -------------------------------------
Push-Location $TreeRoot
try {
    $restore = Invoke-Native -Command 'dotnet' -Arguments @('tool', 'restore')
    if ($restore.ExitCode -ne 0) {
        Fail-Blocked -Summary 'La herramienta Microsoft.DotNet.ApiCompat no se pudo restaurar desde el manifiesto versionado.' `
            -Notes @($restore.Output | ForEach-Object { "$_" })
    }
    $help = Invoke-Native -Command 'dotnet' -Arguments @('apicompat', '--version')
    $versionLine = ($help.Output | Where-Object { "$_".Trim() } | Select-Object -First 1)
    $script:toolVersion = if ($versionLine) { "Microsoft.DotNet.ApiCompat $($versionLine.Trim())" } else { 'Microsoft.DotNet.ApiCompat (versión no reportada)' }
}
finally {
    Pop-Location
}

# --- 2. Resolución de los lados ----------------------------------------------
$baselineDir = if ($BaselineDirectory) { (Resolve-Path -LiteralPath $BaselineDirectory).Path } else { Join-Path $TreeRoot 'tests\fixtures\api-baseline-1.4' }
$fixtureDir = Join-Path $TreeRoot 'tests\fixtures\api-compat-fixtures'
$baselineFramework = @('Cooperator.Framework.Core', 'Cooperator.Framework.Data', 'Cooperator.Framework.Library', 'Cooperator.Framework.Utility')

$contractSideLabel = ''
$implementationSideLabel = ''

if ($Contract -and $Contract.Count -gt 0) {
    $script:contractResolved = @($Contract | ForEach-Object { (Resolve-Path -LiteralPath $_).Path })
} elseif ($Mode -eq 'FixturePair') {
    $script:contractResolved = @((Resolve-Path (Join-Path $fixtureDir 'BaselineFixture\bin\Release\netstandard2.1\Cooperator.ApiCompatFixture.dll')).Path)
} else {
    if (-not (Test-Path $baselineDir)) {
        Fail-Blocked -Summary "Falta la línea base 1.4 versionada en $baselineDir." `
            -Notes @('Producirla con `2.0/build/frozen-1.4.ps1` (fase C del SPEC) antes de correr el gate.')
    }
    $script:contractResolved = @($baselineFramework | ForEach-Object { Join-Path $baselineDir "$_.dll" })
}

# Integridad del baseline: digests ANTES de comparar (SPEC §10).
if ($Mode -ne 'FixturePair' -and -not $Contract) {
    $digestPath = Join-Path $baselineDir 'baseline-digests.json'
    if (-not (Test-Path $digestPath)) {
        Fail-Blocked -Summary 'Falta baseline-digests.json: no se puede verificar la integridad del baseline.' `
            -Notes @("Esperado en $digestPath") -ContractSide ($script:contractResolved -join '; ')
    }
    $expected = Get-Content $digestPath -Raw | ConvertFrom-Json
    $mismatches = @()
    foreach ($entry in $expected.files.PSObject.Properties) {
        $filePath = Join-Path $baselineDir $entry.Name
        if (-not (Test-Path $filePath)) { $mismatches += "$($entry.Name): ausente"; continue }
        $actual = (Get-FileHash -LiteralPath $filePath -Algorithm SHA256).Hash
        if ($actual -ne $entry.Value) { $mismatches += "$($entry.Name): $actual != $($entry.Value)" }
    }
    if ($mismatches.Count -gt 0) {
        Fail-Blocked -Summary 'El baseline 1.4 fue manipulado: su digest no coincide. El gate no compara contra un baseline alterado.' `
            -Notes $mismatches -ContractSide ($script:contractResolved -join '; ')
    }
}

if ($Implementation -and $Implementation.Count -gt 0) {
    $script:implementationResolved = @($Implementation | ForEach-Object { (Resolve-Path -LiteralPath $_).Path })
} elseif ($Mode -eq 'FixturePair') {
    $broken = Join-Path $fixtureDir 'BrokenFixture\bin\Release\netstandard2.1\Cooperator.ApiCompatFixture.dll'
    if (-not (Test-Path $broken)) {
        Fail-Blocked -Summary 'El lado roto del par de fixtures no está construido.' `
            -Notes @("Esperado en $broken", 'Construir la solución 2.0 (o el proyecto BrokenFixture) antes de correr la corrida roja.')
    }
    $script:implementationResolved = @((Resolve-Path $broken).Path)
} elseif ($Mode -eq 'Baseline') {
    # Ensamblados del build 1.4 reproducible (fase B). Se toma el directorio más
    # reciente de `artifacts/<ts>/frozen-1.4/run1`, que frozen-1.4.ps1 declara.
    $frozen = Get-ChildItem (Join-Path $TreeRoot 'artifacts') -Recurse -Directory -Filter 'frozen-1.4' -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $frozen) {
        Fail-Blocked -Summary 'Faltan los ensamblados del build 1.4 reproducible (fase B).' `
            -Notes @('Correr `pwsh -File 2.0/build/frozen-1.4.ps1` antes de la corrida verde.') `
            -ContractSide ($script:contractResolved -join '; ')
    }
    $run1 = Join-Path $frozen.FullName 'run1'
    $script:implementationResolved = @($baselineFramework | ForEach-Object { Join-Path $run1 "$_.dll" })
} else {
    # Current: esqueleto 2.0 (ensamblados vacíos de los cuatro portables).
    $script:implementationResolved = @(
        'Cooperator.Framework.Core', 'Cooperator.Framework.Data', 'Cooperator.Framework.Library', 'Cooperator.Framework.Modeling'
    ) | ForEach-Object { Join-Path $TreeRoot "src\$_\bin\Release\netstandard2.1\$_.dll" }
}

# --- 3. Verificación de existencia de ambos lados -----------------------------
$missing = @()
foreach ($f in $script:contractResolved) { if (-not (Test-Path -LiteralPath $f)) { $missing += "contract: $f" } }
foreach ($f in $script:implementationResolved) { if (-not (Test-Path -LiteralPath $f)) { $missing += "implementation: $f" } }
if ($missing.Count -gt 0) {
    Fail-Blocked -Summary 'Ensamblados ausentes en alguno de los lados de la comparación.' `
        -Notes $missing -ContractSide ($script:contractResolved -join '; ') `
        -ImplementationSide ($script:implementationResolved -join '; ')
}

# --- 4. Comparación en modo estricto -----------------------------------------
# `--left` es el lado contract (la superficie de referencia) y `--right` el lado
# implementation. Los nombres de bandera están verificados contra
# `dotnet apicompat --help` de la versión fijada (10.0.402) y registrados en el MEM.
$cliArgs = @(
    'apicompat',
    '--left', ($script:contractResolved -join ','),
    '--right', ($script:implementationResolved -join ','),
    '--strict-mode',
    '--verbosity', 'normal'
)

$tool = $null
Push-Location $TreeRoot
try {
    $tool = Invoke-Native -Command 'dotnet' -Arguments $cliArgs
}
finally {
    Pop-Location
}
$toolOutput = $tool.Output
$toolExit = $tool.ExitCode
$script:comparatorExitCode = $toolExit
$toolOutput | Set-Content -Path $toolLogPath -Encoding UTF8

$differences = @($toolOutput | Where-Object { $_ -match '^\s*[A-Z]{2}\d{4}' } | ForEach-Object { "$_".Trim() })
if ($differences.Count -eq 0) {
    $differences = @($toolOutput | Where-Object { $_ -match 'does not exist in the implementation|Cannot change|Cannot remove|Cannot add' } | ForEach-Object { "$_".Trim() })
}

if ($Mode -eq 'FixturePair') {
    # AC-5 (SPEC §6.D.2/D.3): el gate DEBE fallar e IDENTIFICAR las dos rupturas.
    $expectedPath = Join-Path $fixtureDir 'expected-breaking-changes.json'
    $expected = Get-Content $expectedPath -Raw | ConvertFrom-Json
    $joined = ($toolOutput -join "`n")
    $unnamed = @()
    foreach ($needle in $expected.required_in_report) {
        if ($joined -notmatch [regex]::Escape($needle)) { $unnamed += $needle }
    }
    if ($toolExit -eq 0) {
        Write-Report -Status 'error' -Summary 'La corrida roja NO falló: el gate no detectó la ruptura deliberada.' `
            -Differences $differences -Notes @('El par de fixtures con la ruptura intencional debería producir código de salida != 0.') `
            -ContractSide ($script:contractResolved -join '; ') -ImplementationSide ($script:implementationResolved -join '; ')
        exit $EXIT_ERROR
    }
    if ($unnamed.Count -gt 0) {
        Write-Report -Status 'error' -Summary 'La corrida roja falló, pero el reporte NO nombra todas las rupturas esperadas.' `
            -Differences $differences -Notes @("No nombradas: " + ($unnamed -join ' | ')) `
            -ContractSide ($script:contractResolved -join '; ') -ImplementationSide ($script:implementationResolved -join '; ')
        exit $EXIT_ERROR
    }
    Write-Report -Status 'differences' `
        -Summary "ruptura deliberada identificada: el gate FALLA como debe (código $EXIT_DIFFERENCES; código del comparador $toolExit). Rupturas nombradas: $($expected.required_in_report.Count)/$($expected.required_in_report.Count)." `
        -Differences $differences -Notes @(
            'Corrida de control del arnés (AC-5): el fallo del gate es el RESULTADO ESPERADO, no un defecto.',
            'NO es un resultado de compatibilidad de producto: los fixtures son sintéticos y no representan tipos del framework.'
        ) `
        -ContractSide ($script:contractResolved -join '; ') -ImplementationSide ($script:implementationResolved -join '; ')
    exit $EXIT_DIFFERENCES
}

if ($toolExit -ne 0) {
    Write-Report -Status 'differences' -Summary "se detectaron diferencias de API (código del comparador $toolExit)." `
        -Differences $differences -Notes @("Salida completa: $toolLogPath") `
        -ContractSide ($script:contractResolved -join '; ') -ImplementationSide ($script:implementationResolved -join '; ')
    exit $EXIT_DIFFERENCES
}

$note = if ($Mode -eq 'Current') {
    @('Corrida INFORMATIVA (SPEC §6.D.2.3): contra los ensamblados vacíos del esqueleto 2.0 la brecha reportada es la forma real del insumo que las notas de migración deberán cubrir. NO es un resultado de gate.')
} else {
    @('El artefacto de referencia representa fielmente lo que el 1.4 congelado produce (AC-4).')
}
Write-Report -Status 'pass' -Summary '0 diferencias de API.' -Differences @() -Notes $note `
    -ContractSide ($script:contractResolved -join '; ') -ImplementationSide ($script:implementationResolved -join '; ')
exit $EXIT_OK
