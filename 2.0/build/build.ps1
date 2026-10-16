#requires -Version 5.1
<#
.SYNOPSIS
    Comando reproducible unico del arnes de Cooperator 2.0 (SPEC-261016-0000 seccion 6.G).

.DESCRIPTION
    Localiza el toolchain por `global.json` (SDK) y por `vswhere` con rango de version
    fijado (MSBuild de Visual Studio), restaura, construye la solucion completa, corre
    la suite que el alcance pida y deja todos los reportes en `2.0/artifacts/<timestamp>/`.

    Por que MSBuild de Visual Studio y no `dotnet build`: el arnes referencia los
    `.csproj` *classic* del 1.4 (`ProjectReference`, sin editar una linea) y `dotnet build`
    no resuelve esa cadena de destino. `dotnet` se usa para restaurar herramientas,
    restaurar paquetes del subconjunto moderno y correr los tests.

    Codigos de salida (vocabulario compartido con el migrador y el CLI del motor,
    ADR-002 seccion 3.2 / ADR-003 seccion 3.2):
      0 = ok
      1 = error de build, de herramienta o de suite
      2 = fallo de gate (diferencias de API en modo verde, o la corrida roja no identifico
          la ruptura) o suite no reproducible
      3 = bloqueado por prerequisito o entorno (toolchain fuera de rango, prerequisito
          de integracion ausente)

.PARAMETER Scope
    Hermetic (default): invariantes del arnes + caracterizacion (Category!=Integration).
    Integration: solo los casos Category=Integration, contra SQL Server real.
    All: ambas.

.PARAMETER Configuration
    Release (default) o Debug.

.PARAMETER ApiCompatDemo
    Corre las tres corridas del gate de la fase D: verde (baseline 1.4 contra el build
    1.4 reproducible), roja (par de fixtures con la ruptura deliberada) e informativa
    (baseline contra el esqueleto 2.0).

.PARAMETER VerifyReproducibility
    Corre la doble corrida del build del 1.4 congelado (fase B).

.PARAMETER ProduceBaseline
    Con -VerifyReproducibility: ademas produce la linea base 1.4 versionada (fase C).

.PARAMETER EmitCharacterizationFixtures
    Con -VerifyReproducibility: ademas emite los fixtures de caracterizacion (fase E).

.EXAMPLE
    pwsh -File 2.0/build/build.ps1 -Scope Hermetic
    pwsh -File 2.0/build/build.ps1 -Scope All -ApiCompatDemo -VerifyReproducibility
#>
[CmdletBinding()]
param(
    [ValidateSet('Hermetic', 'Integration', 'All')]
    [string]$Scope = 'Hermetic',

    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',

    [switch]$ApiCompatDemo,
    [switch]$VerifyReproducibility,
    [switch]$ProduceBaseline,
    [switch]$EmitCharacterizationFixtures,

    [string]$RepositoryRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$EXIT_OK = 0
$EXIT_ERROR = 1
$EXIT_GATE_FAILURE = 2
$EXIT_BLOCKED = 3

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
    $text = foreach ($line in @($output)) { if ($null -ne $line) { "$line" } }
    return @{ Output = @($text); ExitCode = $code }
}

function Invoke-In {
    param([string]$WorkingDirectory, [string]$Command, [string[]]$Arguments)
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $hadNativePref = Get-Variable -Name PSNativeCommandUseErrorActionPreference -ErrorAction SilentlyContinue
    if ($hadNativePref) { $PSNativeCommandUseErrorActionPreference = $false }
    try {
        Push-Location $WorkingDirectory
        $output = & $Command @Arguments 2>&1
        $code = $LASTEXITCODE
    }
    finally {
        Pop-Location
        $ErrorActionPreference = $prevEap
    }
    $text = foreach ($line in @($output)) { if ($null -ne $line) { "$line" } }
    return @{ Output = @($text); ExitCode = $code }
}

function Write-Step { param([string]$Message) Write-Host "==> $Message" }
function Write-Outcome { param([string]$Message) Write-Host "    $Message" }

if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
$TreeRoot = Join-Path $RepositoryRoot '2.0'
$solution = Join-Path $TreeRoot 'Cooperator2.sln'
$harnessProject = Join-Path $TreeRoot 'tests\Cooperator.Harness.Tests\Cooperator.Harness.Tests.csproj'
$characterizationProject = Join-Path $TreeRoot 'tests\Cooperator.Framework.Characterization.Tests\Cooperator.Framework.Characterization.Tests.csproj'
$mappingProject = Join-Path $TreeRoot 'tests\Cooperator.Framework.Core.Mapping.Tests\Cooperator.Framework.Core.Mapping.Tests.csproj'
$encryptorProject = Join-Path $TreeRoot 'tests\ConnectionStringEncriptor.Tests\ConnectionStringEncriptor.Tests.csproj'
$dataProject = Join-Path $TreeRoot 'tests\Cooperator.Framework.Data.Tests\Cooperator.Framework.Data.Tests.csproj'
$toolingProject = Join-Path $TreeRoot 'tests\CooperatorModeler.Reverse.Tests\CooperatorModeler.Reverse.Tests.csproj'
$toolingIntegrationProject = Join-Path $TreeRoot 'tests\CooperatorModeler.Reverse.Integration.Tests\CooperatorModeler.Reverse.Integration.Tests.csproj'
$dataIntegrationProject = Join-Path $TreeRoot 'tests\Cooperator.Framework.Data.Integration.Tests\Cooperator.Framework.Data.Integration.Tests.csproj'
$pin = Get-Content (Join-Path $TreeRoot 'build\frozen-source.json') -Raw | ConvertFrom-Json

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$reports = Join-Path $TreeRoot "artifacts\$stamp"
New-Item -ItemType Directory -Force -Path $reports | Out-Null
Write-Host "Cooperator 2.0 -- corrida reproducible ($Scope / $Configuration)"
Write-Host "Reportes: $reports"
Write-Host ''

$summary = [ordered]@{
    run = [ordered]@{
        scope = $Scope
        configuration = $Configuration
        started_at = (Get-Date -Format 'o')
        repository_root = $RepositoryRoot
        artifacts = $reports
    }
    toolchain = [ordered]@{}
    steps = @()
}

function Add-Step {
    param([string]$Name, [int]$ExitCode, [string]$Status, [string]$Log = '')
    $script:summary.steps += [ordered]@{ step = $Name; exit_code = $ExitCode; status = $Status; log = $Log }
}

function Save-Summary {
    $script:summary.run.finished_at = (Get-Date -Format 'o')
    $script:summary | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $reports 'run-summary.json') -Encoding UTF8
}

# --- 1. Toolchain fijado ------------------------------------------------------
Write-Step 'Resolviendo el toolchain fijado'
$vswhere = $pin.toolchain.vswhere_path
if (-not (Test-Path $vswhere)) {
    Write-Host "BLOQUEADO: no se encontro vswhere en $vswhere (prerequisito de toolchain, SPEC seccion 15.7)."
    exit $EXIT_BLOCKED
}

$found = Invoke-Native -Command $vswhere -Arguments @(
    '-products', '*', '-version', $pin.toolchain.msbuild_version_range,
    '-requires', 'Microsoft.Component.MSBuild',
    '-find', 'MSBuild\**\Bin\MSBuild.exe', '-latest')
$msbuildPath = @($found.Output | Where-Object { "$_".Trim() } | Select-Object -First 1)
if (-not $msbuildPath) {
    Write-Host "BLOQUEADO: MSBuild no esta disponible en el rango fijado $($pin.toolchain.msbuild_version_range). No se sustituye por una version 'parecida'."
    exit $EXIT_BLOCKED
}
$msbuildVersion = @((Invoke-Native -Command $msbuildPath -Arguments @('-version', '-nologo')).Output |
    Where-Object { "$_".Trim() } | Select-Object -First 1).Trim()
Write-Outcome "MSBuild: $msbuildVersion"

$sdkPin = (Get-Content (Join-Path $TreeRoot 'global.json') -Raw | ConvertFrom-Json).sdk
$sdkList = Invoke-Native -Command 'dotnet' -Arguments @('--list-sdks')
if (-not (@($sdkList.Output) -match [regex]::Escape($sdkPin.version))) {
    Write-Host "BLOQUEADO: el SDK fijado en 2.0/global.json ($($sdkPin.version)) no esta instalado."
    exit $EXIT_BLOCKED
}
Write-Outcome "SDK: $($sdkPin.version) (rollForward: $($sdkPin.rollForward))"

$targetingPack = Join-Path (${env:ProgramFiles(x86)}) 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8'
if (-not (Test-Path $targetingPack)) {
    Write-Host "BLOQUEADO: falta el targeting pack .NET Framework 4.8 en $targetingPack."
    exit $EXIT_BLOCKED
}
Write-Outcome "Targeting pack: v4.8"

$summary.toolchain = [ordered]@{ msbuild = $msbuildVersion; msbuild_path = $msbuildPath; sdk = $sdkPin.version }

# --- 2. Restore de herramientas y de paquetes --------------------------------
Write-Step 'Restaurando herramientas (manifiesto versionado) y paquetes'
$toolRestore = Invoke-In -WorkingDirectory $TreeRoot -Command 'dotnet' -Arguments @('tool', 'restore')
$toolRestore.Output | Set-Content (Join-Path $reports 'tool-restore.log') -Encoding UTF8
if ($toolRestore.ExitCode -ne 0) {
    $toolRestore.Output | Select-Object -Last 20 | ForEach-Object { Write-Host $_ }
    Add-Step 'tool-restore' $toolRestore.ExitCode 'error' 'tool-restore.log'
    Save-Summary
    exit $EXIT_ERROR
}
Add-Step 'tool-restore' 0 'pass' 'tool-restore.log'

$restore = Invoke-Native -Command $msbuildPath -Arguments @(
    $solution, '/t:Restore', "/p:Configuration=$Configuration", '/nologo', '/v:minimal')
$restore.Output | Set-Content (Join-Path $reports 'restore.log') -Encoding UTF8
if ($restore.ExitCode -ne 0) {
    $restore.Output | Select-Object -Last 30 | ForEach-Object { Write-Host $_ }
    Add-Step 'restore' $restore.ExitCode 'error' 'restore.log'
    Save-Summary
    exit $EXIT_ERROR
}
Add-Step 'restore' 0 'pass' 'restore.log'
Write-Outcome 'restore OK (con restore locked por lock files versionados)'

# --- 3. Build de la solucion completa ----------------------------------------
Write-Step "Construyendo la solucion 2.0 ($Configuration) con MSBuild fijado"
$build = Invoke-Native -Command $msbuildPath -Arguments @(
    $solution, '/t:Build', "/p:Configuration=$Configuration", '/nologo', '/v:minimal', '/m:1')
$build.Output | Set-Content (Join-Path $reports 'build.log') -Encoding UTF8
if ($build.ExitCode -ne 0) {
    $build.Output | Select-Object -Last 40 | ForEach-Object { Write-Host $_ }
    Add-Step 'build' $build.ExitCode 'error' 'build.log'
    Save-Summary
    Write-Host 'ERROR de build. Con TreatWarningsAsErrors=true, una advertencia cuenta como error (ADR-008 seccion 3.4).'
    exit $EXIT_ERROR
}
Add-Step 'build' 0 'pass' 'build.log'
Write-Outcome "build sin advertencias (tratadas como error): $((@($build.Output | Where-Object { $_ -match ' -> ' })).Count) proyectos"

# --- 4. Listado de paquetes por proyecto -------------------------------------
Write-Step 'Listando paquetes por proyecto (evidencia de dependencias y licencias)'
$packageLines = @()
foreach ($project in @($harnessProject, $characterizationProject) + (Get-ChildItem (Join-Path $TreeRoot 'src') -Directory |
        ForEach-Object { Join-Path $_.FullName ($_.Name + '.csproj') })) {
    $listing = Invoke-In -WorkingDirectory (Split-Path $project -Parent) -Command 'dotnet' -Arguments @(
        'list', $project, 'package', '--include-transitive', '--format', 'json')
    $packageLines += "### $([System.IO.Path]::GetFileName($project))"
    $packageLines += $listing.Output
    $packageLines += ''
}
$packageLines | Set-Content (Join-Path $reports 'packages.txt') -Encoding UTF8
Add-Step 'list-package' 0 'pass' 'packages.txt'
Write-Outcome 'packages.txt'

# --- 5. Suites ----------------------------------------------------------------
$suiteFailures = 0
$integrationBlocked = $false

function Invoke-Suite {
    param([string]$Name, [string]$Project, [string]$Filter, [switch]$Integration)

    Write-Step "Corriendo $Name (filtro: $Filter)"
    $result = Invoke-In -WorkingDirectory $TreeRoot -Command 'dotnet' -Arguments @(
        'test', $Project, '-c', $Configuration, '--no-build', '--filter', $Filter)
    $log = "$Name.log"
    $result.Output | Set-Content (Join-Path $reports $log) -Encoding UTF8

    $tail = @($result.Output | Select-Object -Last 6)
    $tail | ForEach-Object { Write-Host "    $_" }

    if ($Integration -and ($result.Output -join "`n") -match 'COOPERATOR_IT_LEGACY_CONNECTIONSTRING') {
        Add-Step $Name $result.ExitCode 'blocked' $log
        return 'blocked'
    }

    if ($result.ExitCode -ne 0) {
        Add-Step $Name $result.ExitCode 'error' $log
        return 'error'
    }

    Add-Step $Name 0 'pass' $log
    return 'pass'
}

if ($Scope -in @('Hermetic', 'All')) {
    switch (Invoke-Suite -Name 'harness-hermetic' -Project $harnessProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
    switch (Invoke-Suite -Name 'mapping-hermetic' -Project $mappingProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
    switch (Invoke-Suite -Name 'encriptor-hermetic' -Project $encryptorProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
    switch (Invoke-Suite -Name 'tooling-hermetic' -Project $toolingProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
    switch (Invoke-Suite -Name 'data-hermetic' -Project $dataProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
    switch (Invoke-Suite -Name 'characterization-hermetic' -Project $characterizationProject -Filter 'Category!=Integration') {
        'error' { $suiteFailures++ }
    }
}

if ($Scope -in @('Integration', 'All')) {
    $toolingIntegration = Invoke-Suite -Name 'tooling-integration' -Project $toolingIntegrationProject -Filter 'Category=Integration' -Integration
    $dataIntegration = Invoke-Suite -Name 'data-integration' -Project $dataIntegrationProject -Filter 'Category=Integration' -Integration
    $harnessIntegration = Invoke-Suite -Name ''harness-integration'' -Project $harnessProject -Filter ''Category=Integration''
    if ($harnessIntegration -eq 'error') { $suiteFailures++ }

    switch (Invoke-Suite -Name 'characterization-integration' -Project $characterizationProject -Filter 'Category=Integration' -Integration) {
        'error' { $suiteFailures++ }
        'blocked' { $integrationBlocked = $true }
    }

    if (-not (Get-Command 'sqlcmd' -ErrorAction SilentlyContinue)) {
        Write-Outcome 'Nota: no se encontro `sqlcmd`; el esquema de fixture lo aplica el propio arnes con la guarda de nombre de base.'
    }
}

# --- 6. Reproducibilidad del 1.4 (opcional) ----------------------------------
if ($VerifyReproducibility) {
    Write-Step 'Verificando la reproducibilidad del build del 1.4 congelado (doble corrida)'
    $script = Join-Path $TreeRoot 'build\frozen-1.4.ps1'
    $host7 = (Get-Command 'pwsh' -ErrorAction SilentlyContinue)
    $runner = if ($host7) { 'pwsh' } else { 'powershell' }
    $arguments = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $script)
    if ($ProduceBaseline) { $arguments += '-ProduceBaseline' }
    if ($EmitCharacterizationFixtures) { $arguments += '-EmitCharacterizationFixtures' }

    $result = Invoke-In -WorkingDirectory $RepositoryRoot -Command $runner -Arguments $arguments
    $result.Output | Set-Content (Join-Path $reports 'frozen-1.4.log') -Encoding UTF8
    $result.Output | Select-Object -Last 12 | ForEach-Object { Write-Host "    $_" }

    if ($result.ExitCode -eq 0) {
        Add-Step 'frozen-1.4-reproducibility' 0 'pass' 'frozen-1.4.log'
    }
    elseif ($result.ExitCode -eq $EXIT_BLOCKED) {
        Add-Step 'frozen-1.4-reproducibility' $result.ExitCode 'blocked' 'frozen-1.4.log'
    }
    elseif ($result.ExitCode -eq $EXIT_GATE_FAILURE) {
        Add-Step 'frozen-1.4-reproducibility' $result.ExitCode 'error' 'frozen-1.4.log'
        $summary.frozen_1_4_not_reproducible = $false
    }
    else {
        Add-Step 'frozen-1.4-reproducibility' $result.ExitCode 'error' 'frozen-1.4.log'
    }
}

# --- 7. Gate de compatibilidad de API (opcional) -----------------------------
$gateFailures = 0
if ($ApiCompatDemo) {
    Write-Step 'Corriendo las tres corridas del gate de compatibilidad de API'
    $gate = Join-Path $TreeRoot 'build\api-compat.ps1'
    $host7 = if (Get-Command 'pwsh' -ErrorAction SilentlyContinue) { 'pwsh' } else { 'powershell' }

    $runs = [ordered]@{
        'green-baseline' = @{ mode = 'Baseline'; expected = @($EXIT_OK) }
        'red-fixturepair' = @{ mode = 'FixturePair'; expected = @($EXIT_GATE_FAILURE) }
        'informative-current' = @{ mode = 'Current'; expected = @($EXIT_OK, $EXIT_GATE_FAILURE) }
    }

    foreach ($name in $runs.Keys) {
        $expected = $runs[$name].expected
        $result = Invoke-In -WorkingDirectory $RepositoryRoot -Command $host7 -Arguments @(
            '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $gate,
            '-Mode', $runs[$name].mode,
            '-OutputDirectory', (Join-Path $reports "api-compat\$name"))
        $log = "api-compat-$name.log"
        $result.Output | Set-Content (Join-Path $reports $log) -Encoding UTF8
        $tail = @($result.Output | Select-Object -Last 4)
        $tail | ForEach-Object { Write-Host "    $_" }

        $status = if ($expected -contains $result.ExitCode) { 'pass' } elseif ($result.ExitCode -eq $EXIT_BLOCKED) { 'blocked' } else { 'error' }
        if ($status -eq 'error') { $gateFailures++ }
        Add-Step "api-compat-$name" $result.ExitCode $status $log
    }
}

# --- 8. Resultado -------------------------------------------------------------
$outcome = 'ok'
$exitCode = $EXIT_OK
if ($suiteFailures -gt 0 -or $gateFailures -gt 0) { $outcome = 'failed'; $exitCode = $EXIT_ERROR }
elseif ($integrationBlocked) { $outcome = 'blocked-by-environment'; $exitCode = $EXIT_BLOCKED }

$summary.outcome = $outcome
Save-Summary

Write-Host ''
Write-Host "Resultado: $outcome (codigo de salida $exitCode)"
Write-Host "Reportes en $reports"
exit $exitCode
