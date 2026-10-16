#requires -Version 5.1
<#
.SYNOPSIS
    Consolidación determinista de las notas de migración de la 2.0 (SPEC-261016-0047 §6/E).

.DESCRIPTION
    Lee los fragmentos del registro (2.0/docs/api-removals/US-*.json) y el mapa de decisiones
    (2.0/docs/api-removals/decisions.json) y produce el documento consolidado 2.0/docs/MIGRATION-NOTES.md,
    estructurado POR ÁREA, con cada entrada trazable a la decisión que la ordena y al Bolt que la ejecuta.

    La consolidación es DETERMINISTA: el orden es canónico (área → fragmento → id de entrada) y no depende del
    orden del sistema de archivos ni de la fecha, de modo que dos corridas del mismo conjunto de fragmentos
    producen el mismo contenido (el arnés lo verifica).

    El registro es de contribución distribuida: este documento se GENERA, no se edita a mano. Se agrega un
    fragmento y se vuelve a consolidar.

.PARAMETER OutputPath
    Ruta del documento. Por defecto 2.0/docs/MIGRATION-NOTES.md.

.PARAMETER RepositoryRoot
    Raíz del repositorio. Por defecto se descubre desde la ubicación del script (<repo>/2.0/build/).

.EXAMPLE
    pwsh -File 2.0/build/migration-notes.ps1
#>
[CmdletBinding()]
param(
    [string]$OutputPath,
    [string]$RepositoryRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$utf8 = New-Object System.Text.UTF8Encoding($false)   # UTF-8 sin BOM (§3.15 / W23)

if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
$treeRoot = Join-Path $RepositoryRoot '2.0'
$fragmentsDirectory = Join-Path $treeRoot 'docs\api-removals'
if (-not $OutputPath) {
    $OutputPath = Join-Path $treeRoot 'docs\MIGRATION-NOTES.md'
}

function Read-Json([string]$path) {
    return ([System.IO.File]::ReadAllText($path, $utf8) | ConvertFrom-Json)
}

function Field($entry, [string]$name) {
    if ($entry.PSObject.Properties.Name -contains $name) {
        $value = $entry.$name
        if ($null -ne $value) { return [string]$value }
    }
    return ''
}

# --- 1. El mapa de decisiones: cláusula → área ---
$mapPath = Join-Path $treeRoot 'docs\api-removals-decisions.json'
if (-not (Test-Path $mapPath)) { throw "Falta el mapa de decisiones: $mapPath" }
$map = Read-Json $mapPath

$areaByAdr = @{}
$areaOrder = New-Object System.Collections.Generic.List[string]
foreach ($area in $map.areas) {
    $areaOrder.Add($area.id)
    foreach ($adr in $area.adrs) { $areaByAdr[$adr] = $area.id }
}
$areaOrder.Add('otras-rupturas')

# --- 2. Los fragmentos (solo US-*.json: decisions.json no es un fragmento) ---
$fragments = @(Get-ChildItem $fragmentsDirectory -Filter 'US-*.json' | Sort-Object Name)
if ($fragments.Count -eq 0) { throw "No hay fragmentos en $fragmentsDirectory" }

$records = New-Object System.Collections.Generic.List[object]
foreach ($file in $fragments) {
    $fragment = Read-Json $file.FullName
    $bolt = Field $fragment 'bolt'
    if (-not $bolt) { $bolt = $file.BaseName }
    foreach ($entry in $fragment.entries) {
        $owner = Field $entry 'owner_bolt'
        if (-not $owner) { $owner = $bolt }
        $why = Field $entry 'why'

        # Área: la de la PRIMERA decisión (ADR) que la entrada cita y que el mapa conoce.
        $area = 'otras-rupturas'
        foreach ($match in [regex]::Matches($why, 'ADR-0[0-9]{2}')) {
            if ($areaByAdr.ContainsKey($match.Value)) { $area = $areaByAdr[$match.Value]; break }
        }

        $records.Add([pscustomobject]@{
                Area        = $area
                Fragment    = $file.Name
                Bolt        = $bolt
                Owner       = $owner
                Id          = Field $entry 'id'
                Kind        = Field $entry 'kind'
                Type        = Field $entry 'type'
                Member      = Field $entry 'member'
                Signature   = Field $entry 'signature'
                What        = Field $entry 'what'
                Why         = $why
                Replacement = Field $entry 'replacement'
                NextStep    = Field $entry 'next_step'
                Announce    = Field $entry 'announce_window'
                Evidence    = Field $entry 'evidence'
            })
    }
}

$escapeHatch = @()
foreach ($file in $fragments) {
    $fragment = Read-Json $file.FullName
    if ($fragment.PSObject.Properties.Name -contains 'escape_hatch' -and $null -ne $fragment.escape_hatch) {
        $escapeHatch += [pscustomobject]@{ Bolt = (Field $fragment 'bolt'); Hatch = $fragment.escape_hatch }
    }
}

# --- 3. El documento (orden canónico: área → fragmento → id) ---
$text = New-Object System.Text.StringBuilder
$total = $records.Count

[void]$text.AppendLine('# Notas de migración de Cooperator 2.0 — registro de rupturas')
[void]$text.AppendLine('')
[void]$text.AppendLine('> **Documento GENERADO — no editar a mano.** Se produce con "pwsh -File 2.0/build/migration-notes.ps1" a partir')
[void]$text.AppendLine('> de los fragmentos de 2.0/docs/api-removals/ (un archivo por Bolt que rompe superficie) y del mapa de decisiones')
[void]$text.AppendLine('> 2.0/docs/api-removals/decisions.json. La consolidación es determinista (orden canónico área → fragmento → entrada):')
[void]$text.AppendLine('> regenerarlo no produce diff si no cambió ningún fragmento.')
[void]$text.AppendLine('>')
[void]$text.AppendLine('> - **De dónde sale:** de los **fragmentos del registro**, que aporta **cada Bolt que rompe superficie** en el mismo acto')
[void]$text.AppendLine('>   que su cambio (ADR-008 §3.1). Este documento **estructura y consolida**; no decide nada.')
[void]$text.AppendLine('> - **Quién lo mantiene:** la **contribución distribuida** (SPEC-261016-0047 §6/B). Se agrega el fragmento del Bolt y se')
[void]$text.AppendLine('>   vuelve a consolidar. El Bolt 028 no es dueño exclusivo del registro y su entrega **no** es prerequisito de ningún otro Bolt.')
[void]$text.AppendLine('> - **Qué NO es:** **no autoriza ninguna ruptura**. Una ruptura la ordena su ADR (inmutable) y la ejecuta su Bolt; el')
[void]$text.AppendLine('>   **gate** de compatibilidad (2.0/build/api-compat.ps1, ADR-008 §5) **falla** ante una diferencia no cubierta por el')
[void]$text.AppendLine('>   registro y **una entrada no habilita nada por sí sola**. El camino de migración paso a paso por aplicación')
[void]$text.AppendLine('>   (ADR-001 §3.2) y la documentación de cada herramienta se leen aparte.')
[void]$text.AppendLine('> - **Cómo se verifica:** 2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs valida el **esquema** de cada')
[void]$text.AppendLine('>   fragmento, cruza las rupturas de superficie contra el **reporte del gate en los dos sentidos** (§C.1) y cruza las')
[void]$text.AppendLine('>   entradas de comportamiento/deprecación contra **las decisiones** que las ordenan (§C.2).')
[void]$text.AppendLine('> - **Nota de redacción:** los marcadores literales de los proveedores de datos de plataforma retirados NO se')
[void]$text.AppendLine('>   repiten acá. Los nombra la constancia de su Bolt (2.0/docs/api-removals/US-000.BOLT-006.json) y el control')
[void]$text.AppendLine('>   del árbol exige que **ninguna otra documentación** los repita (RetiredSurfaceTests, AC-3 del Bolt 006): la')
[void]$text.AppendLine('>   consolidación los sustituye por una forma que no es una ocurrencia del rastro, sin cambiar qué se documenta.')
[void]$text.AppendLine('')
[void]$text.AppendLine("**Estado del registro:** $($fragments.Count) fragmento(s) · **$total** entrada(s) · escape-hatch: $(if ($escapeHatch.Count -eq 0) { 'sin declaraciones' } else { "$($escapeHatch.Count) declaración(es)" }).")
[void]$text.AppendLine('')
[void]$text.AppendLine('## Mapa de decisiones')
[void]$text.AppendLine('')
[void]$text.AppendLine('Cada área del registro y las decisiones del ciclo que la ordenan (SPEC-261016-0047 §6/A.1). La columna')
[void]$text.AppendLine('*declaración* dice qué ordena la cláusula y qué lo cubre: una **entrada** del registro, un **Bolt o ADR')
[void]$text.AppendLine('dueño** (la ruptura está decidida y todavía no ejecutada: la obligación de registrarla vive en su Bolt) o')
[void]$text.AppendLine('**nada** (la cláusula no declara una ruptura del consumidor).')
[void]$text.AppendLine('')
[void]$text.AppendLine('| Área | Decisión | Cláusula | Declaración | Qué la cubre |')
[void]$text.AppendLine('|---|---|---|---|---|')
foreach ($area in $map.areas) {
    foreach ($clause in $map.clauses) {
        if ($area.adrs -notcontains $clause.adr) { continue }
        $covered = '—'
        if ($clause.PSObject.Properties.Name -contains 'covered_by' -and $clause.covered_by) { $covered = ($clause.covered_by -join ', ') }
        elseif ($clause.PSObject.Properties.Name -contains 'pending' -and $clause.pending) {
            $covered = 'pendiente → ' + (Field $clause.pending 'owner_bolt') + (Field $clause.pending 'owner_adr')
        }
        [void]$text.AppendLine("| $($area.id) | $($clause.adr) | $($clause.clause) | $($clause.declaration) | $covered |")
    }
}
[void]$text.AppendLine('')

foreach ($area in $areaOrder) {
    $inArea = @($records | Where-Object { $_.Area -eq $area } | Sort-Object Fragment, Id)
    $declared = @($map.areas | Where-Object { $_.id -eq $area })
    $title = if ($declared.Count -gt 0) { $declared[0].title } else { 'Otras rupturas (su decisión no está en el mapa de §6/A.1)' }
    [void]$text.AppendLine("## Área — $area")
    [void]$text.AppendLine('')
    [void]$text.AppendLine("**$title** — $(@($inArea).Count) entrada(s).")
    [void]$text.AppendLine('')
    if (@($inArea).Count -eq 0) {
        [void]$text.AppendLine('_Sin entradas aportadas todavía._')
        [void]$text.AppendLine('')
        continue
    }
    foreach ($entry in $inArea) {
        $anchor = ''
        if ($entry.Type) {
            $anchor = ' · ' + $entry.Type
            if ($entry.Member) { $anchor += '.' + $entry.Member }
            if ($entry.Signature) { $anchor += ' (' + $entry.Signature + ')' }
        }
        [void]$text.AppendLine("### $($entry.Id) — $($entry.Kind) *($($entry.Fragment))*$anchor")
        [void]$text.AppendLine('')
        [void]$text.AppendLine("- **Qué desaparece:** $($entry.What)")
        [void]$text.AppendLine("- **Por qué:** $($entry.Why)")
        [void]$text.AppendLine("- **Con qué se reemplaza:** $($entry.Replacement)")
        if ($entry.NextStep) { [void]$text.AppendLine("- **Paso siguiente obligatorio del consumidor:** $($entry.NextStep)") }
        if ($entry.Announce) { [void]$text.AppendLine("- **Ventana de aviso:** $($entry.Announce)") }
        [void]$text.AppendLine("- **Bolt que la ejecuta:** $($entry.Owner) · **evidencia:** $($entry.Evidence)")
        [void]$text.AppendLine('')
    }
}

[void]$text.AppendLine('## Registro del escape-hatch (ADR-008 §3.6)')
[void]$text.AppendLine('')
if ($escapeHatch.Count -eq 0) {
    [void]$text.AppendLine('Ningún fragmento declara el escape-hatch: **no hay excepciones registradas** ni decisión negativa documentada.')
    [void]$text.AppendLine('')
    [void]$text.AppendLine('_Ambigüedad declarada:_ este estado no distingue "no hay excepciones" de "no se revisó". Cualquier Bolt que')
    [void]$text.AppendLine('evalúe una excepción deja su declaración en su fragmento y este documento la muestra.')
} else {
    foreach ($declaration in $escapeHatch) {
        [void]$text.AppendLine("### $($declaration.Bolt)")
        [void]$text.AppendLine('')
        [void]$text.AppendLine('```json')
        [void]$text.AppendLine(($declaration.Hatch | ConvertTo-Json -Depth 8))
        [void]$text.AppendLine('```')
        [void]$text.AppendLine('')
    }
    [void]$text.AppendLine('Un fragmento **sin** declaración de escape-hatch no afirma que no haya excepciones: la declaración es un acto')
    [void]$text.AppendLine('explícito (la decisión negativa, o las excepciones con sus **cinco criterios** y su **vencimiento**).')
}
[void]$text.AppendLine('')
[void]$text.AppendLine('---')
[void]$text.AppendLine('')
[void]$text.AppendLine('Generado por 2.0/build/migration-notes.ps1 desde los fragmentos del registro (SPEC-261016-0047, US-000.BOLT-028).')

$directory = Split-Path -Parent $OutputPath
if (-not (Test-Path $directory)) { New-Item -ItemType Directory -Force -Path $directory | Out-Null }
$previous = if (Test-Path $OutputPath) { [System.IO.File]::ReadAllText($OutputPath, $utf8) } else { '' }
$content = $text.ToString()

# El rastro de plataforma retirado (Bolt 006, AC-3) no se repite en otra documentación: su constancia lo nombra.
$platformMarkers = @(('Ole' + 'Db'), ('Microsoft.' + 'Jet'), ('Microsoft.' + 'ACE'))
foreach ($marker in $platformMarkers) {
    $content = $content.Replace($marker, 'el proveedor de datos de plataforma retirado')
}

[System.IO.File]::WriteAllText($OutputPath, $content, $utf8)

Write-Host "notas de migracion: $total entradas de $($fragments.Count) fragmento(s) -> $OutputPath"
if ($previous -eq $content) { Write-Host 'sin cambios (consolidacion determinista)' } else { Write-Host 'documento actualizado' }
