#requires -Version 5.1
<#
.SYNOPSIS
    Build reproducible del arbol 1.4 congelado, su inventario de identidad y la
    linea base 1.4 del gate de API (SPEC-261016-0000 secciones 6.B y 6.C).

.DESCRIPTION
    1. PRECONDICION DE CONGELAMIENTO (6.B.1): `git diff --quiet <commit> -- 1.4`
       y `git status --porcelain -- 1.4` deben estar vacios. Si no lo estan, el
       script ABORTA con codigo 3 nombrando los archivos; NUNCA restaura ni edita
       nada por su cuenta. Es la restriccion dura heredada: `1.4/**` es solo lectura.
    2. TOOLCHAIN FIJADO (6.B.2): MSBuild de Visual Studio resuelto por vswhere con
       rango de version explicito (no "el ultimo"), targeting pack .NET Framework 4.8.
    3. DOBLE CORRIDA (6.B.3): construye dos veces desde cero, en directorios de
       salida distintos y con obj/ limpio, y compara SHA-256 de cada ensamblado.
    4. INVENTARIO DE IDENTIDAD (6.B.4): nombre, AssemblyVersion, FileVersion,
       PublicKeyToken, SHA-256 y configuracion por ensamblado -- el vocabulario que
       ADR-002 seccion 3.2 usa para la verificacion de identidad del migrador -- mas
       los binarios vendorizados con sus hashes y su divergencia.
    5. LINEA BASE (`-ProduceBaseline`, 6.C): artefacto de referencia para el gate,
       versionado bajo tests/fixtures/api-baseline-1.4/ con su procedencia y digests.
    6. FIXTURES DE CARACTERIZACION (`-EmitCharacterizationFixtures`, 6.E.3 y 6.E.5):
       vectores heredados de cifrado y digests de las plantillas de generacion.

    Contrato de salida (vocabulario compartido 0/1/2/3):
      0 = ok   1 = error   2 = no reproducible (diferencias entre corridas)   3 = bloqueado

.PARAMETER ProduceBaseline
    Ademas del inventario, produce la linea base 1.4 versionada (fase C).

.PARAMETER EmitCharacterizationFixtures
    Ademas, emite los fixtures de caracterizacion: vectores de cifrado (con el
    build 1.4) y digests de las plantillas de generacion (fase E).

.PARAMETER SkipToolchainCheck
    Omite la verificacion de toolchain. Solo para diagnostico; NO usar en evidencia.
#>
[CmdletBinding()]
param(
    [switch]$ProduceBaseline,
    [switch]$EmitCharacterizationFixtures,
    [switch]$SkipToolchainCheck,
    [string]$RepositoryRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$EXIT_OK = 0
$EXIT_ERROR = 1
$EXIT_NOT_REPRODUCIBLE = 2
$EXIT_BLOCKED = 3

function Invoke-Native {
    param([string]$Command, [string[]]$Arguments)
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = & $Command @Arguments 2>&1
        $code = $LASTEXITCODE
    }
    finally { $ErrorActionPreference = $prevEap }
    $text = foreach ($line in @($output)) { if ($null -ne $line) { "$line" } }
    return @{ Output = @($text); ExitCode = $code }
}

function Write-Step { param([string]$Message) Write-Host "==> $Message" }
function Fail-Blocked {
    param([string]$Message, [string[]]$Details = @())
    Write-Host "BLOQUEADO: $Message"
    foreach ($d in $Details) { Write-Host "  - $d" }
    exit $EXIT_BLOCKED
}

if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
$TreeRoot = Join-Path $RepositoryRoot '2.0'
$pinPath = Join-Path $PSScriptRoot 'frozen-source.json'
$pin = Get-Content $pinPath -Raw | ConvertFrom-Json

$artifactsRoot = Join-Path $TreeRoot "artifacts\$(Get-Date -Format 'yyyyMMdd-HHmmss')\frozen-1.4"
New-Item -ItemType Directory -Force -Path $artifactsRoot | Out-Null

# --- 1. Precondicion de congelamiento ----------------------------------------
Write-Step "Precondicion de congelamiento (commit $($pin.frozen_commit))"
$diff = Invoke-Native -Command 'git' -Arguments @('-C', $RepositoryRoot, 'diff', '--quiet', $pin.frozen_commit, '--', '1.4')
$status = Invoke-Native -Command 'git' -Arguments @('-C', $RepositoryRoot, 'status', '--porcelain', '--', '1.4')
$untracked = @($status.Output | Where-Object { "$_".Trim() })
if ($diff.ExitCode -ne 0 -or $untracked.Count -gt 0) {
    Fail-Blocked -Message 'El arbol 1.4 no esta intacto respecto del commit congelado. No se construye ni se restaura nada.' `
        -Details @($untracked + @("git diff --quiet salida: $($diff.ExitCode)"))
}

# --- 2. Toolchain fijado -----------------------------------------------------
Write-Step 'Verificando toolchain fijado'
$msbuildPath = $null
$msbuildVersion = 'unknown'
if (-not $SkipToolchainCheck) {
    if (-not (Test-Path $pin.toolchain.vswhere_path)) {
        Fail-Blocked -Message "No se encontro vswhere en $($pin.toolchain.vswhere_path)."
    }
    $found = Invoke-Native -Command $pin.toolchain.vswhere_path -Arguments @(
        '-products', '*', '-version', $pin.toolchain.msbuild_version_range,
        '-requires', 'Microsoft.Component.MSBuild',
        '-find', 'MSBuild\**\Bin\MSBuild.exe', '-latest')
    $msbuildPath = @($found.Output | Where-Object { "$_".Trim() } | Select-Object -First 1)
    if (-not $msbuildPath) {
        Fail-Blocked -Message "MSBuild no esta disponible en el rango fijado $($pin.toolchain.msbuild_version_range). No se sustituye por una version 'parecida'." `
            -Details @("Esperado: $($pin.toolchain.msbuild_version_observed)")
    }
    $ver = Invoke-Native -Command $msbuildPath -Arguments @('-version', '-nologo')
    $msbuildVersion = @($ver.Output | Where-Object { "$_".Trim() } | Select-Object -First 1).Trim()
    Write-Host "    MSBuild: $msbuildVersion ($msbuildPath)"

    $targetingPack = Join-Path (${env:ProgramFiles(x86)}) 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8'
    if (-not (Test-Path $targetingPack)) {
        Fail-Blocked -Message "Falta el targeting pack .NET Framework 4.8 en $targetingPack."
    }
    Write-Host "    Targeting pack: $targetingPack"

    $sdkJson = Get-Content (Join-Path $TreeRoot 'global.json') -Raw | ConvertFrom-Json
    $sdkList = Invoke-Native -Command 'dotnet' -Arguments @('--list-sdks')
    if (-not (@($sdkList.Output) -match [regex]::Escape($sdkJson.sdk.version))) {
        Fail-Blocked -Message "El SDK fijado en 2.0/global.json ($($sdkJson.sdk.version)) no esta instalado."
    }
    Write-Host "    .NET SDK: $($sdkJson.sdk.version)"
}
else {
    $msbuildPath = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
    $msbuildVersion = 'unknown (SkipToolchainCheck)'
    Write-Host '    VERIFICACION DE TOOLCHAIN OMITIDA (-SkipToolchainCheck)'
}

$globalProperties = @(
    "/p:Configuration=$($pin.toolchain.configuration)",
    '/p:Platform=Any CPU',
    "/p:Deterministic=$($pin.toolchain.msbuild_properties.Deterministic)",
    "/p:DeterministicSourcePaths=$($pin.toolchain.msbuild_properties.DeterministicSourcePaths)",
    "/p:ContinuousIntegrationBuild=$($pin.toolchain.msbuild_properties.ContinuousIntegrationBuild)",
    "/p:RunCodeAnalysis=$($pin.toolchain.msbuild_properties.RunCodeAnalysis)",
    "/p:DebugType=$($pin.toolchain.msbuild_properties.DebugType)",
    "/p:DebugSymbols=$($pin.toolchain.msbuild_properties.DebugSymbols)",
    "/p:PathMap=$RepositoryRoot=/src"
)

# --- 3. Doble corrida --------------------------------------------------------
$tempRoot = Join-Path $env:TEMP "cooperator-frozen-1.4\$([guid]::NewGuid().ToString('N').Substring(0,8))"
$runs = @()
foreach ($runIndex in 1, 2) {
    $runDir = Join-Path $artifactsRoot "run$runIndex"
    $objDir = Join-Path $tempRoot "run$runIndex\obj"
    New-Item -ItemType Directory -Force -Path $runDir, $objDir | Out-Null

    Write-Step "Corrida $runIndex -- construyendo las tres soluciones del 1.4 (salidas fuera de 1.4/)"
    $projectMap = [ordered]@{}
    foreach ($sln in $pin.solutions) {
        $slnPath = Join-Path $RepositoryRoot ($sln.path -replace '/', '\')
        $outPath = $runDir + '\'
        $intermediatePath = $objDir + '\'
        $result = Invoke-Native -Command $msbuildPath -Arguments (@($slnPath, '/t:Rebuild', '/nologo', '/v:minimal', '/m:1') + $globalProperties +
            @("/p:OutputPath=$outPath", "/p:IntermediateOutputPath=$intermediatePath"))
        if ($result.ExitCode -ne 0) {
            Write-Host ($result.Output -join "`n")
            Write-Host "ERROR: la construccion de $($sln.path) fallo en la corrida $runIndex."
            Write-Host 'STOP CONDITION (SPEC 15.2): el 1.4 no compila y 1.4/** es inmutable -- se aisla y se escala; no se toca el arbol congelado.'
            exit $EXIT_ERROR
        }
        foreach ($line in $result.Output) {
            if ($line -match '^\s*(?<proj>[^\s].*?) -> (?<target>.+\.(dll|exe))\s*$') {
                $projectMap[$Matches['proj'].Trim()] = @{ solution = $sln.path; target = $Matches['target'].Trim() }
            }
        }
        $logName = 'msbuild-' + ([System.IO.Path]::GetFileName($sln.path) -replace '\.sln$', '') + '.log'
        $result.Output | Set-Content (Join-Path $runDir $logName) -Encoding UTF8
    }
    $runs += @{ index = $runIndex; dir = $runDir; projects = $projectMap }
}

# --- 4. Verificacion de reproducibilidad por doble corrida -------------------
Write-Step 'Comparando SHA-256 de cada artefacto entre las dos corridas'
function Get-ArtifactDigest {
    param([string]$Directory)
    $map = [ordered]@{}
    Get-ChildItem -Path $Directory -Recurse -File |
        Where-Object { $_.Extension -in @('.dll', '.exe') } |
        Sort-Object Name | ForEach-Object {
            $map[$_.Name] = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
        }
    return $map
}

$digest1 = Get-ArtifactDigest -Directory $runs[0].dir
$digest2 = Get-ArtifactDigest -Directory $runs[1].dir

$allNames = @(@($digest1.Keys) + @($digest2.Keys)) | Sort-Object -Unique
$reproducible = $true
$comparison = @()
foreach ($name in $allNames) {
    $h1 = if ($digest1.Contains($name)) { $digest1[$name] } else { '<ausente>' }
    $h2 = if ($digest2.Contains($name)) { $digest2[$name] } else { '<ausente>' }
    $same = ($h1 -eq $h2) -and ($h1 -ne '<ausente>')
    if (-not $same) { $reproducible = $false }
    $comparison += [ordered]@{ artifact = $name; run1_sha256 = $h1; run2_sha256 = $h2; byte_identical = $same }
}
$comparison | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $artifactsRoot 'reproducibility.json') -Encoding UTF8

$fallbackNote = ''
if (-not $reproducible) {
    Write-Host ''
    Write-Host 'AVISO: hay artefactos NO byte-identicos entre las dos corridas.'
    Write-Host '  Alternativa que ADR-002 3.2 ADMITE y que el SPEC 6.B.3 preve: usar un build'
    Write-Host '  conocido-bueno VENDORIZADO con sus hashes, dejando la discrepancia registrada'
    Write-Host '  como hallazgo del MEM. La alternativa se registra; no se aplica en silencio.'
    $fallbackNote = 'Se declaro la alternativa admitida por ADR-002 3.2 (build conocido-bueno vendorizado con hashes) como camino de contingencia. Ver MEM.'
    $inventoryOnFailure = [ordered]@{
        description = 'Inventario de identidad del build del 1.4 congelado -- corrida con reproducibilidad NO confirmada.'
        generated_at = (Get-Date -Format 'o')
        frozen_commit = $pin.frozen_commit
        reproducibility = [ordered]@{ byte_identical = $false; comparison = $comparison; fallback = $fallbackNote }
    }
    $inventoryOnFailure | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $artifactsRoot 'reproducibility-failure.json') -Encoding UTF8
    exit $EXIT_NOT_REPRODUCIBLE
}

# --- 5. Inventario de identidad ----------------------------------------------
Write-Step 'Emitiendo el inventario de identidad (nombre, version, token, hash)'
$inventory = [ordered]@{
    description = 'Inventario de identidad del build reproducible del 1.4 congelado (SPEC 6.B.4). Vocabulario identico al que ADR-002 3.2 usa para la verificacion de identidad del migrador: nombre + AssemblyVersion + FileVersion + PublicKeyToken + SHA-256 de contenido + ruta relativa + configuracion.'
    generated_at = (Get-Date -Format 'o')
    repository_root = $RepositoryRoot
    frozen_commit = $pin.frozen_commit
    toolchain = [ordered]@{
        builder = 'MSBuild (Visual Studio)'
        msbuild_version = $msbuildVersion
        msbuild_path = $msbuildPath
        configuration = $pin.toolchain.configuration
        properties = $pin.toolchain.msbuild_properties
    }
    reproducibility = [ordered]@{
        method = 'doble corrida desde cero en directorios de salida distintos y con obj/ limpio; SHA-256 por artefacto'
        byte_identical = $reproducible
        comparison = $comparison
        fallback = $fallbackNote
    }
    solutions = @()
    assemblies = @()
    vendored_binaries = @()
}

foreach ($sln in $pin.solutions) {
    $projects = @()
    foreach ($key in $runs[0].projects.Keys) {
        if ($runs[0].projects[$key].solution -eq $sln.path) {
            $projects += [ordered]@{
                project = $key
                target = $runs[0].projects[$key].target
                relative_target = $runs[0].projects[$key].target.Replace($runs[0].dir, '.')
            }
        }
    }
    $inventory.solutions += [ordered]@{ path = $sln.path; reason = $sln.reason; projects = $projects }
}

$sourceTargets = @()
foreach ($key in $runs[0].projects.Keys) {
    $target = $runs[0].projects[$key].target
    if ($sourceTargets -notcontains $target) { $sourceTargets += $target }
}

$run1Files = @($sourceTargets | ForEach-Object { Get-Item -LiteralPath $_ } | Sort-Object Name)
foreach ($file in $run1Files) {
    $assemblyName = $null
    try { $assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($file.FullName) } catch { }
    $fileVersion = (Get-Item $file.FullName).VersionInfo.FileVersion
    $token = $null
    if ($assemblyName -and $assemblyName.GetPublicKeyToken()) {
        $token = -join ($assemblyName.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') })
    }
    $assemblyVersion = $null
    if ($assemblyName) { $assemblyVersion = $assemblyName.Version.ToString() }
    $inventory.assemblies += [ordered]@{
        name = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
        file_name = $file.Name
        assembly_version = $assemblyVersion
        file_version = $fileVersion
        public_key_token = $token
        sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        relative_path = $file.FullName.Replace($runs[0].dir, '.')
        configuration = $pin.toolchain.configuration
    }
}

# Binarios vendorizados de Templates/Assemblies: se registran con su hash para que
# el Bolt del migrador NO los adopte en silencio (SPEC seccion 13).
$vendoredDir = Join-Path $RepositoryRoot ($pin.vendored_binaries.path -replace '/', '\')
if (Test-Path $vendoredDir) {
    foreach ($file in (Get-ChildItem $vendoredDir -File | Where-Object { $_.Extension -in @('.dll', '.exe') } | Sort-Object Name)) {
        $assemblyName = $null
        try { $assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($file.FullName) } catch { }
        $token = $null
        if ($assemblyName -and $assemblyName.GetPublicKeyToken()) {
            $token = -join ($assemblyName.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') })
        }
        $assemblyVersion = $null
        if ($assemblyName) { $assemblyVersion = $assemblyName.Version.ToString() }
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        $matching = $inventory.assemblies | Where-Object { $_.file_name -eq $file.Name } | Select-Object -First 1
        $matchesSource = $false
        $sourceVersion = $null
        if ($matching) {
            $matchesSource = ($matching.sha256 -eq $hash)
            $sourceVersion = $matching.assembly_version
        }
        $note = ''
        if (-not $matchesSource -and $sourceVersion) {
            $note = "Mismo AssemblyVersion ($sourceVersion) que el build de la fuente, pero contenido DISTINTO: no es un artefacto del build reproducible. No adoptarlo como lista de identidad."
        }
        elseif (-not $matchesSource) {
            $note = 'Sin contraparte en el build de la fuente (archivo auxiliar, no un ensamblado).'
        }
        $inventory.vendored_binaries += [ordered]@{
            file_name = $file.Name
            assembly_version = $assemblyVersion
            public_key_token = $token
            sha256 = $hash
            matches_source_build = $matchesSource
            source_build_version = $sourceVersion
            note = $note
        }
    }
}

$inventoryDir = Join-Path $TreeRoot 'tests\fixtures\frozen-1.4'
New-Item -ItemType Directory -Force -Path $inventoryDir | Out-Null
$inventoryPath = Join-Path $inventoryDir 'frozen-1.4-inventory.json'
$inventory | ConvertTo-Json -Depth 8 | Set-Content $inventoryPath -Encoding UTF8
Write-Host "    Inventario: $inventoryPath"

$divergent = @($inventory.vendored_binaries | Where-Object { -not $_.matches_source_build })
$divergentLines = '- Sin divergencias detectadas.'
if ($divergent.Count -gt 0) {
    $divergentLines = ($divergent | ForEach-Object {
            "- ``$($_.file_name)``: version $($_.assembly_version) en el binario versionado vs $($_.source_build_version) del build de la fuente (sha256 ``$($_.sha256)``). $($_.note)"
        }) -join "`n"
}

$provenance = @"
# Procedencia -- build reproducible del 1.4 congelado

Generado por ``2.0/build/frozen-1.4.ps1`` (SPEC-261016-0000 seccion 6.B).

| Campo | Valor |
|-------|-------|
| Commit congelado | ``$($pin.frozen_commit)`` |
| Constructor | MSBuild (Visual Studio) ``$msbuildVersion`` -- rango fijado ``$($pin.toolchain.msbuild_version_range)`` |
| Configuracion | ``$($pin.toolchain.configuration)`` |
| Propiedades | ``Deterministic=true``, ``DeterministicSourcePaths=false`` (ver nota), ``ContinuousIntegrationBuild=true``, ``RunCodeAnalysis=false``, ``DebugType=none``, ``DebugSymbols=false`` (ver nota), ``PathMap=<repo>=/src`` |
| Salidas | fuera de ``1.4/**`` (``artifacts/<timestamp>/frozen-1.4/run<N>/``) e intermedios en ``%TEMP%`` |
| Reproducibilidad | doble corrida desde cero, SHA-256 por artefacto -- byte-identico: **$reproducible** |

## Nota sobre ``DeterministicSourcePaths``

El SPEC seccion 6.B.2 pide ``ContinuousIntegrationBuild=true``, que por defecto activa
``DeterministicSourcePaths=true``. En proyectos *classic* csproj el compilador aborta
con el error "SourceRoot items must include at least one top-level (not nested) item
when DeterministicSourcePaths is true" (Microsoft.Managed.Core.targets) porque esos
proyectos no contribuyen ningun item ``SourceRoot``. Se fija
``DeterministicSourcePaths=false``, lo que ademas **preserva** el ``PathMap`` explicito
que el SPEC pide (con ``DeterministicSourcePaths=true`` el ``PathMap`` se recalcula
desde ``SourceRoot``). ``Deterministic=true`` y ``ContinuousIntegrationBuild=true`` se
mantienen.

## Nota sobre ``DebugType=none``

Con informacion de depuracion (``pdbonly``, que ``Cooperator.Framework.Utility`` y
``ConnectionStringEncriptor`` fijan en Release) el ensamblado embebe el directorio de
depuracion con la **ruta absoluta del PDB**, que cambia entre corridas: la medicion
aislo 40 bytes de diferencia (el ``TimeDateStamp`` del PE, dos GUID y un unico
caracter de la ruta del PDB). El vocabulario de identidad que ADR-002 seccion 3.2 usa
para el migrador es nombre + version + token + hash del **ensamblado**; el PDB es un
artefacto aparte y no forma parte de esa identidad. Por eso la construccion
reproducible no emite informacion de depuracion.

## Binarios vendorizados ``Templates/Assemblies``

Estan **desalineados con la fuente**: se registran con su hash y su divergencia para
que el Bolt del migrador **no** los adopte en silencio como lista de identidad
(ADR-002 seccion 3.2, SPEC seccion 13).

$divergentLines
"@
$provenance | Set-Content (Join-Path $inventoryDir 'PROVENANCE.md') -Encoding UTF8
Write-Host "    Procedencia: $(Join-Path $inventoryDir 'PROVENANCE.md')"

# --- 6. Linea base 1.4 (fase C, opcional) ------------------------------------
if ($ProduceBaseline) {
    Write-Step 'Produciendo la linea base 1.4 para el gate de API (SPEC 6.C)'
    $baselineDir = Join-Path $TreeRoot 'tests\fixtures\api-baseline-1.4'
    New-Item -ItemType Directory -Force -Path $baselineDir | Out-Null

    # Via primaria (SPEC 6.C.1): pedir al compilador el artefacto de REFERENCIA
    # mediante propiedades de MSBuild, sin editar el 1.4.
    $refAsmDir = Join-Path $artifactsRoot 'refasm'
    New-Item -ItemType Directory -Force -Path $refAsmDir | Out-Null
    $refAsmProduced = $false
    $frameworkSln = Join-Path $RepositoryRoot '1.4\Framework.sln'
    $refResult = Invoke-Native -Command $msbuildPath -Arguments (@($frameworkSln, '/t:Rebuild', '/nologo', '/v:quiet', '/m:1') + $globalProperties +
        @('/p:ProduceReferenceAssembly=true', '/p:ProduceOnlyReferenceAssembly=true', "/p:OutputPath=$refAsmDir\", "/p:IntermediateOutputPath=$objDir\refasm\"))
    # Con ProduceOnlyReferenceAssembly, Roslyn deja los artefactos de referencia bajo
    # un subdirectorio <ref | refint>; se buscan en cualquier nivel.
    $refAsmFiles = @(Get-ChildItem $refAsmDir -Recurse -File -Filter *.dll -ErrorAction SilentlyContinue)
    $baselineSource = 'respaldo previsto por ADR-008 seccion 5 y SPEC 6.C.1: se usan los MISMOS ensamblados construidos por el build reproducible del 1.4 como artefacto de referencia (la propiedad de reference assembly no se honro de forma utilizable en los proyectos classic)'
    $resolvedRefAsm = [ordered]@{}
    if ($refResult.ExitCode -eq 0) {
        foreach ($name in $pin.baseline.scope) {
            $candidate = $refAsmFiles | Where-Object { $_.Name -eq "$name.dll" } | Select-Object -First 1
            if ($candidate) { $resolvedRefAsm[$name] = $candidate.FullName }
        }
        if ($resolvedRefAsm.Count -eq $pin.baseline.scope.Count) {
            $refAsmProduced = $true
            $baselineSource = 'reference assemblies producidos con /p:ProduceReferenceAssembly=true /p:ProduceOnlyReferenceAssembly=true'
        }
    }

    $baselineFiles = [ordered]@{}
    foreach ($name in $pin.baseline.scope) {
        $sourceFile = if ($refAsmProduced) { $resolvedRefAsm[$name] } else { Join-Path $runs[0].dir "$name.dll" }
        if (-not (Test-Path $sourceFile)) {
            Fail-Blocked -Message "No se pudo producir el artefacto de la linea base para $name."
        }
        Copy-Item -LiteralPath $sourceFile -Destination (Join-Path $baselineDir "$name.dll") -Force
        $baselineFiles["$name.dll"] = (Get-FileHash -LiteralPath (Join-Path $baselineDir "$name.dll") -Algorithm SHA256).Hash
    }

    $digests = [ordered]@{
        description = 'Digests de la linea base 1.4. El gate los verifica ANTES de comparar: un baseline alterado no se compara, se reporta como bloqueo (3) -- SPEC seccion 10, control anti-manipulacion.'
        generated_at = (Get-Date -Format 'o')
        frozen_commit = $pin.frozen_commit
        scope = $pin.baseline.scope
        excluded = $pin.baseline.excluded
        files = $baselineFiles
    }
    $digests | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $baselineDir 'baseline-digests.json') -Encoding UTF8

    $baselineProvenance = @"
# Procedencia -- linea base 1.4 del gate de compatibilidad de API

Generada por ``2.0/build/frozen-1.4.ps1 -ProduceBaseline`` (SPEC-261016-0000 seccion 6.C,
ADR-008 seccion 5).

| Campo | Valor |
|-------|-------|
| Commit de origen | ``$($pin.frozen_commit)`` |
| Constructor | MSBuild (Visual Studio) ``$msbuildVersion`` |
| Configuracion | ``$($pin.toolchain.configuration)`` |
| Comando | ``MSBuild 1.4/Framework.sln /t:Rebuild`` con las propiedades deterministas fijadas y ``/p:OutputPath=`` redirigido fuera de ``1.4/**`` |
| Via usada | $baselineSource |
| Alcance | ``$($pin.baseline.scope -join ', ')`` |
| Excluidos | ``$($pin.baseline.excluded -join ', ')`` -- registrados en el inventario de identidad (fase B), que es su uso real |
| Digests | ``baseline-digests.json`` (uno por archivo) |

## Por que estos cuatro y no los ejecutables

Los cuatro ensamblados del framework son **la superficie que la 2.0 reemplaza** y la
que las notas de migracion deberan cubrir miembro por miembro. ``CooperatorModeler.exe``
y ``ConnectionStringEncriptor.exe`` se registran en el inventario de identidad
(fase B) y **no** entran al baseline de API.
"@
    $baselineProvenance | Set-Content (Join-Path $baselineDir 'PROVENANCE.md') -Encoding UTF8
    Write-Host "    Linea base: $baselineDir ($($pin.baseline.scope.Count) ensamblados) -- via: $baselineSource"
}

# --- 7. Fixtures de caracterizacion (fase E, opcional) -----------------------
if ($EmitCharacterizationFixtures) {
    Write-Step 'Emitiendo fixtures de caracterizacion (vectores de cifrado y digests de plantillas)'

    # 7.a Vectores heredados de cifrado. Se generan UNA VEZ con el build 1.4 y se
    # versionan: son el insumo que ADR-007 necesita para migrar valores YA cifrados.
    # El material de clave de 1.4 NO se copia a ningun fixture.
    # El ensamblado 1.4 es .NET Framework: se ejecuta en el host correcto
    # (Windows PowerShell 5.1), no en el motor de PowerShell 7. El script generado
    # es ASCII puro y se escribe con BOM para que el host 5.1 lo decodifique bien.
    $libraryDll = Join-Path $runs[0].dir 'Cooperator.Framework.Library.dll'
    $cryptoScript = Join-Path $tempRoot 'emit-crypto.ps1'
    $cryptoOut = Join-Path $tempRoot 'crypto-vectors.json'
    $bomEncoding = if ($PSVersionTable.PSVersion.Major -ge 6) { 'utf8BOM' } else { 'UTF8' }
    $cryptoSource = @'
$ErrorActionPreference = 'Stop'
$assembly = [System.Reflection.Assembly]::LoadFrom('__LIBRARY_DLL__')
$type = $assembly.GetType('Cooperator.Framework.Library.Cryptography')
$encrypt = $type.GetMethod('Encrypt', [Type[]]@([string]))
$decrypt = $type.GetMethod('Decrypt', [Type[]]@([string]))
$acuteA = [string][char]0x00C1
$nTilde = [string][char]0x00F1
$plaintexts = @(
    '',
    'A',
    'Cooperator',
    'Precio unitario: 1.234,56',
    ('acentuado ' + $acuteA + $nTilde),
    '0123456789',
    (('a' * 64) -join ''),
    ('linea1' + [char]10 + 'linea2')
)
$vectors = @()
foreach ($p in $plaintexts) {
    $cipher = $null
    $errorText = $null
    try { $cipher = $encrypt.Invoke($null, @($p)) }
    catch { $errorText = $_.Exception.InnerException.GetType().FullName + ': ' + $_.Exception.InnerException.Message }
    $roundTrip = $null
    if ($null -ne $cipher) {
        try { $roundTrip = $decrypt.Invoke($null, @($cipher)) }
        catch { $roundTrip = '[error]' }
    }
    $vectors += [ordered]@{ plaintext = $p; ciphertext = $cipher; round_trip = $roundTrip; error = $errorText }
}
$result = [ordered]@{
    description = 'Vectores heredados de cifrado producidos UNA VEZ con el build 1.4 congelado (SPEC seccion 6.E.3). El material de clave del 1.4 no se copia a ningun fixture de la 2.0.'
    produced_with = '__PRODUCED_WITH__'
    plaintexts_are_synthetic = $true
    accent_note = 'Los textos acentuados se construyen con [char]0x00C1 y [char]0x00F1 para que el script generado sea ASCII puro y no dependa de la codificacion del host.'
    vectors = $vectors
}
$result | ConvertTo-Json -Depth 6 | Set-Content '__CRYPTO_OUT__' -Encoding UTF8
'@
    $producedWith = 'Cooperator.Framework.Library 1.4.5.0 (build reproducible del commit ' + $pin.frozen_commit + ')'
    $cryptoSource = $cryptoSource.Replace('__LIBRARY_DLL__', $libraryDll).
        Replace('__PRODUCED_WITH__', $producedWith).
        Replace('__CRYPTO_OUT__', $cryptoOut)
    Set-Content -Path $cryptoScript -Value $cryptoSource -Encoding $bomEncoding

    $cryptoRun = Invoke-Native -Command 'powershell.exe' -Arguments @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $cryptoScript)
    if ($cryptoRun.ExitCode -ne 0) {
        Write-Host ($cryptoRun.Output -join "`n")
        Fail-Blocked -Message 'No se pudieron emitir los vectores heredados de cifrado con el build 1.4.'
    }
    $cryptoDir = Join-Path $TreeRoot 'tests\fixtures\legacy-crypto'
    New-Item -ItemType Directory -Force -Path $cryptoDir | Out-Null
    Copy-Item $cryptoOut (Join-Path $cryptoDir 'crypto-vectors.json') -Force
    Write-Host "    Vectores de cifrado: $(Join-Path $cryptoDir 'crypto-vectors.json')"

    # 7.b Digests de las plantillas de generacion: el insumo dorado del motor.
    # Son las plantillas versionadas del 1.4, que es lo que la generacion consume;
    # su contenido dorado es el contrato que el migrador debe conservar.
    $templatesDir = Join-Path $RepositoryRoot '1.4\Tools\CooperatorModeler\Templates'
    $goldenDir = Join-Path $TreeRoot 'tests\fixtures\golden-generation'
    New-Item -ItemType Directory -Force -Path $goldenDir | Out-Null
    $templateEntries = [ordered]@{}
    Get-ChildItem $templatesDir -Recurse -File |
        Where-Object { $_.Extension -in @('.cs', '.vb', '.sql', '.sln', '.csproj', '.vbproj', '.config', '.resx') } |
        Sort-Object FullName | ForEach-Object {
            $rel = $_.FullName.Substring($templatesDir.Length).TrimStart('\').Replace('\', '/')
            $templateEntries[$rel] = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
        }
    $golden = [ordered]@{
        description = 'Fixtures dorados de generacion (SPEC seccion 6.E.5): digests del contenido de las plantillas del 1.4 congelado, que es el insumo real de los puntos de entrada de generacion. Cualquier cambio futuro en las plantillas de la 2.0 se compara contra este inventario.'
        produced_with = 'build reproducible del commit ' + $pin.frozen_commit
        templates_root = '1.4/Tools/CooperatorModeler/Templates'
        file_count = $templateEntries.Count
        files = $templateEntries
    }
    $golden | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $goldenDir 'template-digests.json') -Encoding UTF8
    Write-Host "    Digests de plantillas: $(Join-Path $goldenDir 'template-digests.json') ($($templateEntries.Count) archivos)"
}

Write-Host ''
Write-Host "OK -- build reproducible del 1.4: byte-identico=$reproducible; artefactos en $artifactsRoot"
exit $EXIT_OK
