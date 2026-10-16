# Evaluación de uso real por capacidad — `Cooperator.Framework.Library` (2.0)

**Bolt:** `US-000.BOLT-009` · **SPEC:** `SPEC-261016-0037-library-portable` **revisión 1** (aprobada
2026-10-16T00:39:45-03:00) · **Fase:** §6/A.1 · **Ejecutado en:** V-Bounce N°1 ·
**Manejo de datos:** el almacén es `confidential`: este informe **no contiene ningún valor del almacén** (sólo
nombres de claves, archivos y líneas de los llamadores, y estados).

Este informe es el artefacto que hace **auditable** la regla de ADR-001 §3.5 —«eliminar por defecto, **previa
verificación del uso real**; lo que sobreviva, detrás de una abstracción portable y nunca detrás de un TFM
Windows»— y la que el SPEC §6/A.2 fija: una capacidad se elimina **sólo** si la evidencia sostiene que no se usa.

---

## 1. Método y alcance de la búsqueda (con su límite declarado, §6/A.2)

- **Qué se buscó:** por cada capacidad, la **definición** y **todos sus usos** (llamadas, menciones en proyectos y
  en documentación), con el patrón de cada símbolo de la capacidad.
- **Dónde:** todo el árbol del producto — `1.4/**` (congelado, incluido el tooling `Tools/CooperatorModeler`) y
  `2.0/**` — sobre `*.cs`, `*.csproj` y `*.md`, excluyendo salidas de build (`bin/`, `obj/`, `artifacts/`).
- **Límite declarado:** la búsqueda cubre **el árbol del producto** (las fuentes del framework y el tooling que
  viaja en él). **No** cubre el código del **consumidor fuera del repositorio**: eso lo aporta el consumidor. Por
  eso el resultado «0 llamadores» se lee como «sin uso **dentro del árbol**» y se registra con ese alcance, y por
  eso la capacidad con uso **declarado por el consumidor** se conserva (§3).
- **Evidencia por capacidad:** archivos y líneas donde aparece el símbolo. La definición **no** cuenta como uso.

---

## 2. Las capacidades sin uso en el árbol → **ELIMINADAS**

| Capacidad | Dónde vive hoy | Evidencia de la búsqueda (re-verificada en este V-Bounce) | Consecuencia de eliminarla | Resolución |
|---|---|---|---|---|
| **Impersonation** (`GetImpersonatedUser`, `DllImport("Advapi32.dll")` para `GetUserName`/`LogonUser` → `WindowsIdentity`) | `1.4/Cooperator.Framework.Library/IO/CurrentUserInfo.cs` | **0 llamadores**: las 8 ocurrencias del patrón están **dentro del propio archivo**; la única externa es su entrada en el `.csproj` | Ninguna dentro del árbol. Si un consumidor necesitara la identidad del usuario, el reemplazo declarado es `Environment.UserName` (ADR-001 §3.5), que es portable y no requiere invocación nativa | **Eliminada** |
| **Ajuste del working set** (`SetProcessWorkingSetSize`) y su envoltorio `SaveMemory()` | `IO/NativeMethods.cs` + `IO/Memory.cs` | **Cadena sin uso**: `NativeMethods` sólo lo usa `Memory` (3 ocurrencias en `Memory.cs`) y `Memory` **no tiene llamadores** | Ninguna dentro del árbol; el runtime gestiona su memoria. La invocación nativa es `kernel32.dll` puro | **Eliminada** |
| **Log de eventos** (`System.Diagnostics.EventLog`, `WriteEntry`) | `IO/EventViewer.cs` | **0 llamadores**: las 6 ocurrencias están en el propio archivo | Ninguna dentro del árbol. La abstracción de logging del **tooling** tiene su dueño y no vive en la librería (SPEC §4) | **Eliminada** |
| **Identidad de máquina por WMI** (`GetDriveSerialNumber` → `System.Management` / `win32_logicaldisk`) | `Utility.cs` | **0 llamadores**: las 7 ocurrencias están **dentro de `Utility.cs`** (su definición y su implementación); la otra es su entrada en el `.csproj` | Ninguna dentro del árbol. Si el consumidor la reclamara **con evidencia**, su reemplazo es una **abstracción** (`IMachineIdentity`), nunca WMI dentro de la librería (ADR-001 §3.5) | **Eliminada** |
| **Código muerto de correo** (`SendMailViaCDONTS` con `throw new NotImplementedException` inmediato) | `SmtpMail.cs` | **Muerto por construcción**: 1 ocurrencia y es su propia definición; además su firma pública arrastra `System.Web.HttpContext` (F-22) | Ninguna: nunca tuvo comportamiento | **Eliminado** — y con él `System.Web` sale de una firma pública de la librería |

---

## 3. La capacidad **con uso** → **SOBREVIVE detrás de una abstracción portable**

### 3.1 La evidencia de uso (lo que impide eliminarla)

| Dato | Evidencia |
|---|---|
| La capacidad | **Almacén de configuración de usuario** (`IO/Registry.cs`, `Repository/{RegistryRepository,RepositoryEntity,RepositoryMapper}.cs`) |
| **Quién la usa** | El **consumidor** que viaja en el árbol: `1.4/Tools/CooperatorModeler/ConnectForm.cs` (8 ocurrencias del patrón), que **guarda y lee** la configuración de conexión del Modeler bajo su propia clave de aplicación |
| Alcance del hallazgo | **Dentro del árbol**: el Modeler es el consumidor identificado. **Fuera del árbol** no se buscó (límite declarado, §1); un consumidor externo que use el almacén queda cubierto por la misma resolución (la capacidad sobrevive) |
| Consecuencia de eliminarla | **Pérdida de datos y de comportamiento del consumidor**: el Modeler dejaría de recordar su configuración de conexión; y las claves ya persistidas quedarían **inaccesibles** (el fallo sería **silencioso**, ver §3.2) |

→ **Resolución: sobrevive detrás de una abstracción portable**, declarada en `Library` y **sin dependencias**
(ADR-001 §3.4: cero terceros; ADR-001 §3.5: nunca un paquete `-windows`). La **implementación sustituta** —el
almacén en `%APPDATA%` o equivalente— es del **Bolt 010** (SPEC §4/C.2), y su ubicación es una decisión visible al
revisor de este Bolt: **este V-Bounce no implementa el sustituto**.

**Nota de la condición de parada §15 #6:** la evaluación **concluye que el almacén NO se elimina** —hay uso
verificado—, de modo que la enmienda del Bolt (`test_integration` a `not-required`) **no aplica**: el
`test_integration: required` del Bolt y su plan de integración (verificación contra el almacén real) siguen en pie.

### 3.2 El contrato de claves, verificado **byte a byte** (F-48)

La derivación de clave del 1.4 es **contrato de datos**: si cambia un solo byte, las claves ya guardadas dejan de
encontrarse y el fallo es **silencioso** del lado del cliente.

| Elemento del contrato | Verificación en este V-Bounce |
|---|---|
| Regla de derivación | `ownerName.Trim().ToLower()` + **`\u00A1`** + `parameterName.Trim().ToLower()` (`Repository/RepositoryMapper.cs:92`) — el recorte y las mayúsculas se aplican a **cada parte**, por separado |
| El byte separador | **`0xA1`, y es el ÚNICO byte no ASCII del archivo** (`RepositoryMapper.cs`), en el **offset 3222** — re-medido leyendo el archivo como bytes en este V-Bounce |
| Destinos | `CurrentUser` (por defecto en `BaseRegistryKey`) y `LocalMachine`, según `RegistryTarget` |
| Accesores tipados | texto, decimal, fecha y booleano |
| Camino de ausencia | **explícito**: la lectura atrapa el error de E/S del almacén y cae al caso «no existe» (`RegistryRepository.cs:64`, `RepositoryMapper.cs:69-73`); **nunca** un valor por defecto silencioso |
| Limitación declarada | la derivación usa el **lowercase sensible a la cultura** del proceso (`ToLower()` sin cultura invariante): es una **limitación heredada** del contrato —cambiarla alteraría las claves ya persistidas en culturas con reglas especiales de casing (p. ej. tr/az)— y se **declara** en lugar de «corregirse» |

**Valores ya persistidos:** se siguen leyendo porque la derivación se conserva idéntica; este Bolt **no re-cifra,
no descifra y no migra** ningún valor (eso es de los Bolts 022/030; condición de parada §15 #5).

---

## 4. El contenido portable que se conserva sin cambios de contrato

`Cryptography.cs`, `TimeFunctions.cs`, `Utility.cs` (sin la identidad de máquina por WMI de §2), 
`TaxIDVerification/Argentina.cs`, `Resources.cs`, `IO/{File,Path,OperatingSystemInfo}.cs` y `Exceptions/*`: lo
declara ADR-001 §3.1 como contenido portable de la librería. El retiro de la superficie de serialización de las
excepciones es del **Bolt 007** (SPEC-0013) y la limpieza de la referencia muerta `System.Windows.Forms` del
`.csproj` es del **Bolt 012** (SPEC-0021): este Bolt **registra** el estado que encuentra y no toca ninguna de las
dos (AC-7).

---

## 5. Resumen de resoluciones

| Capacidad | Resolución | Evidencia |
|---|---|---|
| Impersonation (`CurrentUserInfo`) | Eliminada | 0 llamadores fuera de su definición |
| Working set (`NativeMethods`/`Memory`) | Eliminada | cadena sin uso (0 llamadores de `Memory`) |
| `EventViewer` | Eliminada | 0 llamadores fuera de su definición |
| Identidad de máquina por WMI (`GetDriveSerialNumber`) | Eliminada | 0 llamadores fuera de su definición en `Utility.cs` |
| `SendMailViaCDONTS` | Eliminado | muerto por construcción (`NotImplementedException` inmediato) |
| **Almacén de configuración de usuario** | **Sobrevive detrás de una abstracción portable** (implementación: Bolt 010) | **uso verificado** en `Tools/CooperatorModeler/ConnectForm.cs` |

> **Estado de este informe y de las fases siguientes.** Este informe completa la **fase A** (§6/A.1). Las fases
> **B–F** —el retiro de las capacidades eliminadas, la **abstracción** del almacén con su ubicación declarada, los
> **vectores dorados** del contrato de claves, la **portabilidad verificada** y la **constancia de rupturas**—
> quedan para el **V-Bounce N°2** de este Bolt, que el propio SPEC estima en dos (§16, DoD: «si el presupuesto se
> agota, progreso en el MEM y pendiente explícito para un V-Bounce nuevo»). Lo pendiente está enumerado en el MEM
> del V-Bounce N°1 (`MEM-261016-0215`), junto con lo ya medido que lo desbloquea.
