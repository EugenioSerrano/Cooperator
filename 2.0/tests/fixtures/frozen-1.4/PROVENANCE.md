# Procedencia -- build reproducible del 1.4 congelado

Generado por `2.0/build/frozen-1.4.ps1` (SPEC-261016-0000 seccion 6.B).

| Campo | Valor |
|-------|-------|
| Commit congelado | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` |
| Constructor | MSBuild (Visual Studio) `18.11.0.47511` -- rango fijado `[18.11.0,19.0)` |
| Configuracion | `Release` |
| Propiedades | `Deterministic=true`, `DeterministicSourcePaths=false` (ver nota), `ContinuousIntegrationBuild=true`, `RunCodeAnalysis=false`, `DebugType=none`, `DebugSymbols=false` (ver nota), `PathMap=<repo>=/src` |
| Salidas | fuera de `1.4/**` (`artifacts/<timestamp>/frozen-1.4/run<N>/`) e intermedios en `%TEMP%` |
| Reproducibilidad | doble corrida desde cero, SHA-256 por artefacto -- byte-identico: **True** |

## Nota sobre `DeterministicSourcePaths`

El SPEC seccion 6.B.2 pide `ContinuousIntegrationBuild=true`, que por defecto activa
`DeterministicSourcePaths=true`. En proyectos *classic* csproj el compilador aborta
con el error "SourceRoot items must include at least one top-level (not nested) item
when DeterministicSourcePaths is true" (Microsoft.Managed.Core.targets) porque esos
proyectos no contribuyen ningun item `SourceRoot`. Se fija
`DeterministicSourcePaths=false`, lo que ademas **preserva** el `PathMap` explicito
que el SPEC pide (con `DeterministicSourcePaths=true` el `PathMap` se recalcula
desde `SourceRoot`). `Deterministic=true` y `ContinuousIntegrationBuild=true` se
mantienen.

## Nota sobre `DebugType=none`

Con informacion de depuracion (`pdbonly`, que `Cooperator.Framework.Utility` y
`ConnectionStringEncriptor` fijan en Release) el ensamblado embebe el directorio de
depuracion con la **ruta absoluta del PDB**, que cambia entre corridas: la medicion
aislo 40 bytes de diferencia (el `TimeDateStamp` del PE, dos GUID y un unico
caracter de la ruta del PDB). El vocabulario de identidad que ADR-002 seccion 3.2 usa
para el migrador es nombre + version + token + hash del **ensamblado**; el PDB es un
artefacto aparte y no forma parte de esa identidad. Por eso la construccion
reproducible no emite informacion de depuracion.

## Binarios vendorizados `Templates/Assemblies`

Estan **desalineados con la fuente**: se registran con su hash y su divergencia para
que el Bolt del migrador **no** los adopte en silencio como lista de identidad
(ADR-002 seccion 3.2, SPEC seccion 13).

- `Cooperator.Framework.Core.dll`: version 1.4.5.0 en el binario versionado vs 1.4.5.0 del build de la fuente (sha256 `CCC57742E497F3C019D390E383B919BB7DDC75E3E0F1273151F5ED1827CDB43E`). Mismo AssemblyVersion (1.4.5.0) que el build de la fuente, pero contenido DISTINTO: no es un artefacto del build reproducible. No adoptarlo como lista de identidad.
- `Cooperator.Framework.Data.dll`: version 1.4.5.0 en el binario versionado vs 1.4.5.0 del build de la fuente (sha256 `D3D6EAC157426D916FA2875394701EC01834E47EC2A216CDE8B054D1C3C1B4E1`). Mismo AssemblyVersion (1.4.5.0) que el build de la fuente, pero contenido DISTINTO: no es un artefacto del build reproducible. No adoptarlo como lista de identidad.
- `Cooperator.Framework.Library.dll`: version 1.4.5.0 en el binario versionado vs 1.4.5.0 del build de la fuente (sha256 `381C5E8EC3BEB762023DED332F198C814F702670560B9932D19D90E6B6B020E7`). Mismo AssemblyVersion (1.4.5.0) que el build de la fuente, pero contenido DISTINTO: no es un artefacto del build reproducible. No adoptarlo como lista de identidad.
- `Cooperator.Framework.Utility.dll`: version 1.3.3.0 en el binario versionado vs 1.4.5.0 del build de la fuente (sha256 `0BAC0845EA89AA03E769C4D814EFF1831A8A81BE5DFD29F81C0164F149A8FCAD`). Mismo AssemblyVersion (1.4.5.0) que el build de la fuente, pero contenido DISTINTO: no es un artefacto del build reproducible. No adoptarlo como lista de identidad.
