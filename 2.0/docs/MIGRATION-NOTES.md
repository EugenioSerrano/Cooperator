# Notas de migración de Cooperator 2.0 — registro de rupturas

> **Documento GENERADO — no editar a mano.** Se produce con "pwsh -File 2.0/build/migration-notes.ps1" a partir
> de los fragmentos de 2.0/docs/api-removals/ (un archivo por Bolt que rompe superficie) y del mapa de decisiones
> 2.0/docs/api-removals/decisions.json. La consolidación es determinista (orden canónico área → fragmento → entrada):
> regenerarlo no produce diff si no cambió ningún fragmento.
>
> - **De dónde sale:** de los **fragmentos del registro**, que aporta **cada Bolt que rompe superficie** en el mismo acto
>   que su cambio (ADR-008 §3.1). Este documento **estructura y consolida**; no decide nada.
> - **Quién lo mantiene:** la **contribución distribuida** (SPEC-261016-0047 §6/B). Se agrega el fragmento del Bolt y se
>   vuelve a consolidar. El Bolt 028 no es dueño exclusivo del registro y su entrega **no** es prerequisito de ningún otro Bolt.
> - **Qué NO es:** **no autoriza ninguna ruptura**. Una ruptura la ordena su ADR (inmutable) y la ejecuta su Bolt; el
>   **gate** de compatibilidad (2.0/build/api-compat.ps1, ADR-008 §5) **falla** ante una diferencia no cubierta por el
>   registro y **una entrada no habilita nada por sí sola**. El camino de migración paso a paso por aplicación
>   (ADR-001 §3.2) y la documentación de cada herramienta se leen aparte.
> - **Cómo se verifica:** 2.0/tests/Cooperator.Harness.Tests/MigrationNotesCoverageTests.cs valida el **esquema** de cada
>   fragmento, cruza las rupturas de superficie contra el **reporte del gate en los dos sentidos** (§C.1) y cruza las
>   entradas de comportamiento/deprecación contra **las decisiones** que las ordenan (§C.2).
> - **Nota de redacción:** los marcadores literales de los proveedores de datos de plataforma retirados NO se
>   repiten acá. Los nombra la constancia de su Bolt (2.0/docs/api-removals/US-000.BOLT-006.json) y el control
>   del árbol exige que **ninguna otra documentación** los repita (RetiredSurfaceTests, AC-3 del Bolt 006): la
>   consolidación los sustituye por una forma que no es una ocurrencia del rastro, sin cambiar qué se documenta.

**Estado del registro:** 11 fragmento(s) · **78** entrada(s) · escape-hatch: 1 declaración(es).

## Mapa de decisiones

Cada área del registro y las decisiones del ciclo que la ordenan (SPEC-261016-0047 §6/A.1). La columna
*declaración* dice qué ordena la cláusula y qué lo cubre: una **entrada** del registro, un **Bolt o ADR
dueño** (la ruptura está decidida y todavía no ejecutada: la obligación de registrarla vive en su Bolt) o
**nada** (la cláusula no declara una ruptura del consumidor).

| Área | Decisión | Cláusula | Declaración | Qué la cubre |
|---|---|---|---|---|
| contenedor-y-empaquetado | ADR-001 | 3.1 | api_removal | US-000.BOLT-003 |
| contenedor-y-empaquetado | ADR-001 | 3.2 | api_removal | US-000.BOLT-007 |
| contenedor-y-empaquetado | ADR-001 | 3.3 | api_removal | pendiente → ADR-001 |
| contenedor-y-empaquetado | ADR-001 | 3.4 | none | — |
| contenedor-y-empaquetado | ADR-001 | 3.5 | api_removal | US-000.BOLT-006, US-000.BOLT-009 |
| contenedor-y-empaquetado | ADR-001 | 3.5 | deprecation | pendiente → US-000.BOLT-002 |
| contenedor-y-empaquetado | ADR-001 | 3.6 | none | — |
| formato-del-modelo | ADR-002 | 3.1 | behavior_change | pendiente → US-000.BOLT-002 |
| formato-del-modelo | ADR-002 | 3.2 | none | — |
| formato-del-modelo | ADR-002 | 3.3 | deprecation | pendiente → US-000.BOLT-002 |
| formato-del-modelo | ADR-002 | 3.4 | none | — |
| conjuntos-de-plantillas | ADR-003 | 3.1 | behavior_change | pendiente → ADR-003 |
| conjuntos-de-plantillas | ADR-003 | 3.2 | none | — |
| conjuntos-de-plantillas | ADR-003 | 3.3 | behavior_change | pendiente → ADR-003 |
| conjuntos-de-plantillas | ADR-003 | 3.4 | behavior_change | pendiente → ADR-003 |
| conjuntos-de-plantillas | ADR-003 | 3.5 | none | — |
| conjuntos-de-plantillas | ADR-003 | 3.6 | none | — |
| configuracion-y-cache | ADR-004 | 3.1 | behavior_change | pendiente → ADR-004 |
| configuracion-y-cache | ADR-004 | 3.2 | behavior_change | US-000.BOLT-011 |
| configuracion-y-cache | ADR-004 | 3.3 | behavior_change | US-000.BOLT-011 |
| configuracion-y-cache | ADR-004 | 3.4 | behavior_change | pendiente → ADR-003 |
| configuracion-y-cache | ADR-004 | 3.5 | none | — |
| configuracion-y-cache | ADR-004 | 3.6 | none | — |
| capa-de-datos | ADR-005 | 3.1 | behavior_change | pendiente → ADR-005 |
| capa-de-datos | ADR-005 | 3.2 | behavior_change | pendiente → ADR-005 |
| capa-de-datos | ADR-005 | 3.3 | api_removal | pendiente → ADR-005 |
| capa-de-datos | ADR-005 | 3.4 | none | — |
| capa-de-datos | ADR-005 | 3.5 | none | — |
| capa-de-datos | ADR-005 | 3.6 | behavior_change | pendiente → ADR-005 |
| carga-de-ensamblados-y-providers-lazy | ADR-006 | 3.1 | behavior_change | US-000.BOLT-005 |
| carga-de-ensamblados-y-providers-lazy | ADR-006 | 3.2 | behavior_change | US-000.BOLT-005 |
| carga-de-ensamblados-y-providers-lazy | ADR-006 | 3.3 | behavior_change | US-000.BOLT-005 |
| carga-de-ensamblados-y-providers-lazy | ADR-006 | 3.4 | none | — |
| criptografia | ADR-007 | 3.1 | api_removal | US-000.BOLT-022 |
| criptografia | ADR-007 | 3.2 | none | — |
| criptografia | ADR-007 | 3.3 | behavior_change | pendiente → US-000.BOLT-023 |
| criptografia | ADR-007 | 3.4 | none | — |
| criptografia | ADR-007 | 3.5 | behavior_change | US-000.BOLT-024 |
| criptografia | ADR-007 | 3.6 | none | — |
| superficie-de-serializacion | ADR-008 | 3.1 | none | — |
| superficie-de-serializacion | ADR-008 | 3.2 | api_removal | US-000.BOLT-007, US-000.BOLT-003 |
| superficie-de-serializacion | ADR-008 | 3.2 | api_removal | pendiente → US-000.BOLT-002 |
| superficie-de-serializacion | ADR-008 | 3.3 | api_removal | US-000.BOLT-007 |
| superficie-de-serializacion | ADR-008 | 3.4 | none | — |
| superficie-de-serializacion | ADR-008 | 3.5 | none | — |
| superficie-de-serializacion | ADR-008 | 3.6 | none | — |
| superficie-de-serializacion | ADR-008 | 3.7 | api_removal | pendiente → ADR-003 |
| superficie-de-serializacion | ADR-008 | 3.8 | none | — |

## Área — contenedor-y-empaquetado

**Contenedor y empaquetado: layout, coexistencia, naming/versionado y las features de plataforma** — 14 entrada(s).

### BOLT003-01 — namespace-and-assembly-move *(US-000.BOLT-003.json)*

- **Qué desaparece:** Los tipos del grafo y del reverse-engineering dejan el ensamblado y el namespace `Cooperator.Framework.Utility`: pasan a `Cooperator.Framework.Modeling` (netstandard2.1) con namespace = ensamblado = paquete — `…Utility.DBReverseHelper` → `Cooperator.Framework.Modeling.Model` / `.Reverse`; `…Utility.Exceptions` → `Cooperator.Framework.Modeling.Exceptions`.
- **Por qué:** ADR-001 §3.1/§3.3: el paquete nuevo `Cooperator.Framework.Modeling` es portable y su namespace sigue la regla de ADR-001 §3.3; `Cooperator.Framework.Utility` no existe en la 2.0.
- **Con qué se reemplaza:** Usar el paquete `Cooperator.Framework.Modeling` (mismos nombres de tipo, namespace nuevo). `Cooperator.Framework.Utility` desaparece como ensamblado y como namespace; no hay shim ni type-forwarder (ADR-008 §3.1).
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** 2.0/src/Cooperator.Framework.Modeling/ con Model/, Reverse/ y Exceptions/; el ensamblado construido referencia sólo `netstandard 2.1.0.0` (DependencyReportTests).

### BOLT003-02 — attribute-removal *(US-000.BOLT-003.json)*

- **Qué desaparece:** Desaparecen del modelo los atributos de editor/diseñador —`[EditorAttribute]`, `[DesignerSerializationVisibility]`, `[Description]`, `[Browsable]`, `[DefaultValue]`, `[Category]`— y los `using` de `System.ComponentModel.Design`, `System.Windows.Forms*` y `System.Drawing*`.
- **Por qué:** ADR-001 §3.1 y ADR-002 §3.1: el modelo de la 2.0 es POCO, sin atributos de editor ni tipos de UI; la edición es dirigida por el esquema del contrato (ADR-009 §3.2).
- **Con qué se reemplaza:** El esquema del contrato del Bolt 004 y el editor dirigido por esquema del shell (Bolt 025). Los consumidores que dependían de los atributos para armar formularios deben leer el esquema.
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** ModelPocoTests: inspección por reflexión de la superficie pública de Cooperator.Framework.Modeling.dll (0 atributos de diseñador, 0 tipos de UI).

### BOLT003-03 — coupling-removal *(US-000.BOLT-003.json)*

- **Qué desaparece:** El modelo deja de escribir en el editor retirado: la sentencia `PropertyEditors.DropDownListEditor.stringList = _ParentCandidates;` (DomainTree.cs L1621, getter `GenerateAsChildOf` de EntityNode) desaparece, junto con el `using` del provider concreto (L35) y el `using` muerto hacia `LazyLoad`.
- **Por qué:** ADR-001 §3.1: mientras ese acoplamiento exista, `Modeling` no puede ser portable (es una arista del modelo hacia la UI, y `netstandard2.1` no puede referenciar el tooling `net10.0-windows`). Hallazgo declarado en el V-Bounce N°2 (MEM-261016-0107 §8.6) e incorporado al SPEC en su revisión 3.
- **Con qué se reemplaza:** Ninguno dentro del modelo: la lista de candidatos la arma quien edita (el tooling/shell), no el modelo.
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** El archivo aportado no contiene la sentencia ni tipos de UI; el build del paquete en netstandard2.1 es la prueba dura (los tipos de UI no existen en ese TFM).

### BOLT003-05 — base-type-change *(US-000.BOLT-003.json)*

- **Qué desaparece:** La base de las excepciones del grafo cambia: `UtilityException` (y con ella `UnknownDBTypeExeception`, `NodeAlreadyExistException` y `CannotAddOrRemoveItemException`) deja de derivar de `Cooperator.Framework.Library.Exceptions.BaseException` y pasa a derivar de `System.Exception`.
- **Por qué:** ADR-001 §3.1: `Modeling` no tiene aristas salientes —no puede referenciar `Library`— y es la tensión §14(a) que el SPEC aprobó.
- **Con qué se reemplaza:** Ninguno: la jerarquía entre las excepciones del grafo se conserva (las tres derivan de `UtilityException`). Un consumidor que atrapara `BaseException` debe atrapar `System.Exception` (o el tipo del grafo).
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** Exceptions/UtilityException.cs (`: Exception`) y el reporte de dependencias por ensamblado: sin aristas hacia Core/Library/Data.

### BOLT006-01 — method-removed *(US-000.BOLT-006.json)* · Cooperator.Framework.Core.ObjectListHelper<T, T2>.CreateExcelFile (public void CreateExcelFile(T2 rowSet, string spreadsheetName, string targetFileName))

- **Qué desaparece:** Desaparece de la superficie pública del núcleo el método de export a Excel del helper de RowSet: escribía un libro .xls abriendo una conexión con un proveedor de datos de plataforma (el de Microsoft para archivos de Access/Excel), con su cadena de conexión 'Excel 8.0' sobre un archivo temporal TMP.xls.
- **Por qué:** ADR-001 §3.5 (y OQ-005, que decide lo mismo): el export a Excel «se elimina» del framework — no se reescribe ni se mueve a un paquete -windows; era la única razón por la que el núcleo no podía ser portable (REV-001 F-03, §6 ítem 8).
- **Con qué se reemplaza:** Ninguno dentro del framework (ADR-008 §3.1: sin type-forwarders, sin dobles superficies, sin capas compat). El export queda del lado del consumidor, que lo resuelve en su propia capa con la biblioteca de hojas de cálculo que elija; un consumidor que lo necesite tal cual puede quedarse en el 1.4 congelado, que es una opción soportada y explícita.
- **Bolt que la ejecuta:** US-000.BOLT-006 · **evidencia:** 2.0/src/Cooperator.Framework.Core/ObjectListHelper.cs se aporta sin el método, sin su auxiliar privado y sin el using del proveedor (119 líneas frente a las 305 del 1.4); el ensamblado 2.0 no expone ningún miembro CreateExcelFile (RetiredSurfaceTests, inspección con MetadataLoadContext) y el gate lo reporta como diferencia de miembro retirado cubierta por esta entrada (ApiRemovalCoverageTests).

### BOLT006-02 — member-removed *(US-000.BOLT-006.json)* · Cooperator.Framework.Core.ObjectListHelper<T, T2>.xlsLock (private static Object xlsLock)

- **Qué desaparece:** Desaparece el campo privado estático que serializaba el acceso al archivo .xls: existía únicamente para la capacidad retirada y no participa de la superficie pública del tipo.
- **Por qué:** ADR-001 §3.5: sin la capacidad, el campo es rastro de plataforma y de código muerto por arrastre (SPEC-261016-0012 §6/A.1).
- **Con qué se reemplaza:** Ninguno: el campo no era observable desde fuera del ensamblado.
- **Bolt que la ejecuta:** US-000.BOLT-006 · **evidencia:** ObjectListHelper.cs aportado sin el campo; la búsqueda de rastro sobre 2.0/src da 0 coincidencias de xlsLock (RetiredSurfaceTests).

### BOLT006-03 — platform-trace-removed *(US-000.BOLT-006.json)* · Cooperator.Framework.Core.ObjectListHelper<T, T2>.using System.Data.el proveedor de datos de plataforma retirado (using System.Data.el proveedor de datos de plataforma retirado;)

- **Qué desaparece:** Desaparece la directiva de importación del espacio de nombres de datos de plataforma que el archivo necesitaba sólo para la capacidad retirada, y con ella el rastro de OLE DB / Jet / ACE en todo el árbol 2.0: sin using, sin tipos, sin referencias ni paquetes, sin versión centralizada y sin cadena de conexión.
- **Por qué:** ADR-001 §3.4/§3.5 y SPEC-261016-0012 §4/§6/A.2: la política de dependencias del núcleo es cero dependencias de plataforma; dejarlo sería exactamente el rastro que el Bolt pide eliminar (era código muerto de plataforma).
- **Con qué se reemplaza:** Ninguno: el núcleo no declara ninguna dependencia y compila sobre netstandard2.1, TFM en el que esos tipos no existen.
- **Bolt que la ejecuta:** US-000.BOLT-006 · **evidencia:** La búsqueda de rastro sobre el código, los proyectos, los scripts y la documentación propios de 2.0 da 0 ocurrencias (RetiredSurfaceTests), excluyendo por diseño 1.4/** y la evidencia 1.4 congelada de 2.0/tests/fixtures/api-baseline-1.4/** (el contrato del gate); el build del núcleo sobre netstandard2.1 es la prueba dura.

### BOLT009-01 — type-removed *(US-000.BOLT-009.json)* · Cooperator.Framework.Library.IO.CurrentUserInfo (public static System.Security.Principal.WindowsIdentity GetImpersonatedUser(string userName, string domain, string password))

- **Qué desaparece:** Desaparece la invocación nativa de impersonation (DllImport de Advapi32 para GetUserName/LogonUser) y con ella el único camino del framework que suplantaba al usuario.
- **Por qué:** ADR-001 §3.5: se elimina por defecto previa verificación del uso real; la evaluación no encontró ningún llamador en el árbol.
- **Con qué se reemplaza:** Ninguno dentro del framework. Si un consumidor necesitara la identidad del usuario, el reemplazo declarado por la política es Environment.UserName (portable, sin invocación nativa).
- **Bolt que la ejecuta:** US-000.BOLT-009 · **evidencia:** IO/CurrentUserInfo.cs no existe en 2.0/src; la búsqueda de rastro da 0 DllImport y 0 GetImpersonatedUser en la librería (LibraryPortabilityTests).

### BOLT009-02 — type-removed *(US-000.BOLT-009.json)* · Cooperator.Framework.Library.IO.Memory (public static void SaveMemory() + Cooperator.Framework.Library.IO.NativeMethods.SetProcessWorkingSetSize)

- **Qué desaparece:** Desaparece el ajuste del working set del proceso: la cadena NativeMethods (DllImport de kernel32) + Memory no tenía llamadores.
- **Por qué:** ADR-001 §3.5: capacidad de plataforma sin uso; el runtime gestiona su memoria.
- **Con qué se reemplaza:** Ninguno: no había nada que reemplazar.
- **Bolt que la ejecuta:** US-000.BOLT-009 · **evidencia:** IO/NativeMethods.cs e IO/Memory.cs no existen en 2.0/src; 0 SetProcessWorkingSetSize en la librería (LibraryPortabilityTests).

### BOLT009-03 — type-removed *(US-000.BOLT-009.json)* · Cooperator.Framework.Library.IO.EventViewer (public static void WriteEntry(string message) (System.Diagnostics.EventLog))

- **Qué desaparece:** Desaparece el log de eventos del framework: no tenía llamadores.
- **Por qué:** ADR-001 §3.5: se elimina por defecto previa verificación del uso real. Una abstracción de logging sería del tooling, no de la librería (SPEC-261016-0037 §4).
- **Con qué se reemplaza:** Ninguno dentro de la librería: el logging del tooling tiene su dueño y su propio Bolt.
- **Bolt que la ejecuta:** US-000.BOLT-009 · **evidencia:** IO/EventViewer.cs no existe en 2.0/src; 0 System.Diagnostics.EventLog en la librería (LibraryPortabilityTests).

### BOLT009-04 — method-removed *(US-000.BOLT-009.json)* · Cooperator.Framework.Library.Utility.GetDriveSerialNumber (public static string GetDriveSerialNumber(string driveLetter))

- **Qué desaparece:** Desaparece la identidad de máquina por WMI: el método abría un ManagementObject sobre win32_logicaldisk para leer el número de serie del volumen. Con él se retira el uso de System.Management de la librería y la región que lo agrupaba.
- **Por qué:** ADR-001 §3.5: capacidad de plataforma sin uso (0 llamadores fuera de su definición) y WMI no existe en el TFM portable de la librería.
- **Con qué se reemplaza:** Ninguno dentro de la librería. Si el consumidor la reclamara CON EVIDENCIA, su reemplazo es una abstracción (IMachineIdentity) implementada fuera de la librería, nunca WMI dentro de ella.
- **Bolt que la ejecuta:** US-000.BOLT-009 · **evidencia:** Utility.cs aportado sin el método y sin su región (512 → 493 líneas) con 0 GetDriveSerialNumber y 0 System.Management; el gate de compatibilidad lo reporta como miembro retirado y esta entrada lo cubre (ApiRemovalCoverageTests, LibraryPortabilityTests).

### BOLT009-05 — method-removed *(US-000.BOLT-009.json)* · Cooperator.Framework.Library.SmtpMail.SendMailViaCDONTS (public void SendMailViaCDONTS(string from, string to, string subject, string body, System.Web.HttpContext currentHttpContext, params System.IO.FileInfo[] attachments))

- **Qué desaparece:** El código muerto de correo (lanzaba NotImplementedException de inmediato) y el System.Web que arrastraba en su firma pública.
- **Por qué:** ADR-001 §3.1: código muerto por construcción (REV-001 F-22).
- **Con qué se reemplaza:** Ninguno: nunca tuvo comportamiento.
- **Bolt que la ejecuta:** US-000.BOLT-009 · **evidencia:** Library/SmtpMail.cs sin el método (Bolt 011); la evaluación de uso real de este Bolt lo declara muerto por construcción.

### BOLT014-03 — behavior-change *(US-000.BOLT-014.json)*

- **Qué desaparece:** El gate por token de la capa pierde su bypass web: CheckForToken ya no retorna sin verificar cuando el host es web (el bypass leia el cache del host). Sin la plataforma web ese camino no puede existir.
- **Por qué:** SPEC-261016-0020 6/C.6 (el bypass estaba en la lista de usos medidos de BaseGateway.cs) y ADR-001 3.3: el gate por PublicKeyToken esta retirado del framework (F-10/OQ-003) y su retiro es del US-000.BOLT-015, que no se toca aca.
- **Con qué se reemplaza:** Ninguno: la verificacion que quedaba es la del gate retirado, que el Bolt 015 elimina por completo. Mientras exista, corre siempre (no hay camino que la saltee por ser web).
- **Paso siguiente obligatorio del consumidor:** El retiro completo del gate por token (CheckForToken y su contraparte generada) es del US-000.BOLT-015.
- **Bolt que la ejecuta:** US-000.BOLT-014 · **evidencia:** DataLayerContractTests: 0 HttpContext en BaseGateway.cs; el control de la capa incluye el archivo donde vivia el bypass.

### BOLT016-04 — type-removed *(US-000.BOLT-016.json)* · Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors

- **Qué desaparece:** El cuarto destino de la obligacion del tooling -los `PropertyEditors/*` del 1.4: `DropDownListEditor`, `StringEditor` (+ su Form y su Designer) y `ToStringInfoEditor` (+ su Form y su Designer), con sus recursos `.resx`- NO se reubica: se RETIRA. El proyecto del tooling no contiene ningun Form ni `UseWindowsForms`.
- **Por qué:** SPEC-261016-0027 revision 4 (6/D.3-b y 16): el V-Bounce N.1 midio que los editores portados verbatim NO compilan en `net10.0-windows` (`WFO1000` sobre las propiedades del form, con `TreatWarningsAsErrors` del arbol convirtiendolo en error) y las dos unicas salidas quedan fuera de la autoridad del ejecutor: anotar el codigo aportado (prohibido por 6/D.3-a) o silenciar el analizador (excepcion de gate, G21: exige ADR). Ademas, la particion del Bolt 003 ya retiro los atributos de disenador del modelo y el acoplamiento modelo-editor (ADR-001 3.1, ADR-009 3.2): en la 2.0 la edicion es dirigida por el esquema del contrato, de modo que los editores no tienen vehiculo.
- **Con qué se reemplaza:** REEMPLAZO CERO (no hay reemplazo): No hay reemplazo: la edicion de la 2.0 es dirigida por el esquema del contrato (ADR-009 3.2) y el modelo ya no lleva los atributos de editor que invocaban a estos tipos. El retiro deja el componente del reverse sin UI, que es su alcance (la pantalla es del Bolt 025). Nota de registro: el reemplazo se declara en prosa y no como el literal "none" porque el validador propio del arnes (MigrationNotesCoverageTests.Field) solo conoce seis campos y no lee replacement_reason - con "none" el esquema EXIGE la razon y el validador del arnes la considera ausente: contradiccion medida entre el esquema publicado y el validador, declarada en el MEM de este V-Bounce.
- **Paso siguiente obligatorio del consumidor:** Ninguno pendiente para este Bolt: la obligacion del Bolt 003 cierra con tres destinos materializados mas este retiro declarado. Si el producto quisiera una edicion con Forms propia, seria una capacidad nueva con su ADR, no la reubicacion de los editores del 1.4.
- **Bolt que la ejecuta:** US-000.BOLT-016 · **evidencia:** Verificado en el arbol: 0 archivos `*Editor*`/`*Form*` (`.cs`/`.resx`) bajo `2.0/src` y `2.0/tests`; 0 menciones de `PropertyEditors` en el tooling; 0 `UseWindowsForms`/`System.Windows.Forms`/`System.Design`/`System.Drawing` en sus fuentes y en su `.csproj`.

## Área — formato-del-modelo

**Formato del modelo: lo que el `.coop2` deja de transportar y las deprecaciones del corpus legacy** — 0 entrada(s).

_Sin entradas aportadas todavía._

## Área — conjuntos-de-plantillas

**Conjuntos de plantillas: lo que el set deja de emitir** — 1 entrada(s).

### BOLT003-07 — type-not-ported *(US-000.BOLT-003.json)*

- **Qué desaparece:** `CompilerException` no migra a `Modeling` (queda fuera de la partición), junto con `ScriptingHost` y `ScriptsEnvironment`.
- **Por qué:** ADR-003: su slot es el motor de generación, no el modelo.
- **Con qué se reemplaza:** El slot del motor (Bolts 017/018/019/020/021).
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** El paquete no contiene el tipo; Exceptions/UtilityException.cs lo declara como no portado.

## Área — configuracion-y-cache

**Configuración y caché: catálogo de claves, prefijo de esquema y comportamiento del caché** — 13 entrada(s).

### BOLT011-01 — type-removed *(US-000.BOLT-011.json)*

- **Qué desaparece:** Desaparecen de la superficie de Core los tipos de la sección custom del LazyLoad: ProvidersSection (ConfigurationSection), ProvidersCollection (ConfigurationElementCollection) y Provider (ConfigurationElement).
- **Por qué:** ADR-004 §3.2: el framework deja de leer y de escribir la configuración de su host.
- **Con qué se reemplaza:** Las claves del catálogo: Cooperator:LazyLoad:Providers:<entidad>:Assembly y …:Class (ver 2.0/docs/configuration-keys.md). El mecanismo de carga sigue siendo del Bolt 005 (ADR-006).
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** 2.0/src/Cooperator.Framework.Core no contiene los tipos; el control de ausencia (HostConfigurationAbsenceTests) da 0 usos de ConfigurationManager/ConfigurationSection/ConfigurationElement.

### BOLT011-02 — type-removed *(US-000.BOLT-011.json)*

- **Qué desaparece:** Desaparece LazyConfigurationHelper (Read, Read(fileName) y CreateEmpty/CreateEmpty(fileName)) con su ConfigurationManager.OpenMappedExeConfiguration y su cfg.Save(ConfigurationSaveMode.Full).
- **Por qué:** ADR-004 §3.2: era la única ruta del framework que leía y ESCRIBÍA el archivo de configuración del host (REV-001 F-04).
- **Con qué se reemplaza:** Ninguno dentro del framework: si el host necesita la sección, la crea él. El framework nunca más toca la configuración de su host.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** El archivo 1.4/Cooperator.Framework.Core/LazyLoad/LazyProviderFactoryConfigHelper.cs NO se aporta a 2.0 (no queda nada tras el retiro); el control de ausencia da 0 OpenMappedExeConfiguration y 0 escrituras de configuración.

### BOLT011-03 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** El servidor SMTP deja de leerse de AppSettings["SMTPServerName"] al cargar el tipo: la lectura pasa a ser perezosa y validada en el punto de uso, y su ausencia produce el error tipado del framework.
- **Por qué:** ADR-004 §3.2 (F-22): el inicializador estático lanzaba NullReferenceException en el type-initializer, un error poco diagnosticable.
- **Con qué se reemplaza:** Cooperator:Mail:SmtpServer (variable de entorno Cooperator__Mail__SmtpServer).
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** Library/SmtpMail.cs sin inicializador estático; ConfigurationContractTests y HostConfigurationAbsenceTests.

### BOLT011-04 — method-removed *(US-000.BOLT-011.json)*

- **Qué desaparece:** SmtpMail.SendMailViaCDONTS desaparece de la superficie pública (era código muerto: lanzaba NotImplementedException y su firma arrastraba System.Web.HttpContext).
- **Por qué:** ADR-004 §3.2 y ADR-001 §3.1 ya lo declaraban saliente.
- **Con qué se reemplaza:** Ninguno dentro del framework.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** Library/SmtpMail.cs sin el método.

### BOLT011-05 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** La fuente del timeout de comandos cambia de la entrada AppSettings["CooperatorCommandTimeOut"] a una clave del catálogo. Default documentado: 30 segundos (el de 1.4 cuando la clave no estaba definida).
- **Por qué:** ADR-004 §3.2.
- **Con qué se reemplaza:** Cooperator:Data:CommandTimeoutSeconds (Cooperator__Data__CommandTimeoutSeconds).
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** La clave y su default están publicados acá y en ConfigurationKeys/Defaults; el archivo 1.4/Cooperator.Framework.Data/SQLHelper.cs:84 no existe en 2.0 (su forma compilable es el entregable del Bolt 014).

### BOLT011-06 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** La fuente de la cadena de conexión cambia de ConfigurationManager.ConnectionStrings[...] del host a una clave del catálogo. Se retiran con ella la heurística por substrings que decidía a ciegas si el texto estaba cifrado y el Dictionary estático que la cacheaba.
- **Por qué:** ADR-004 §3.2/§3.3 (REV-001 F-13).
- **Con qué se reemplaza:** Cooperator:ConnectionStrings:<nombre> (nombre por defecto CooperatorConnectionString, conservado), con prefijo de esquema obligatorio.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** La clave y su forma están publicadas acá y en ConfigurationKeys/ConnectionStringValue; el archivo 1.4/Cooperator.Framework.Data/BaseRule.cs:104-121 no existe en 2.0 (su forma compilable es el entregable del Bolt 014).

### BOLT011-07 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** Desaparece el Dictionary estático de connection strings: la configuración puede cambiar entre lecturas (tests, multi-tenant), cosa que antes era imposible.
- **Por qué:** ADR-004 §3.3: la resolución es del proveedor de configuración, con su propio criterio; el reemplazo de la instancia ambiental es la operación documentada.
- **Con qué se reemplaza:** CooperatorSettings.Current (instancia ambiental reemplazable) y las sobrecargas explícitas que aceptan un ICooperatorConfiguration.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** El mecanismo de reemplazo en caliente está entregado y probado en Core (ConfigurationContractTests: se reemplaza la instancia y la lectura siguiente ve el valor nuevo).

### BOLT011-08 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** Un valor de conexión SIN prefijo de esquema deja de funcionar: hay que declarar plain: (o el esquema del resolutor que corresponda). Un prefijo desconocido se rechaza nombrando la clave y el prefijo ofensor.
- **Por qué:** ADR-004 §3.3: se elimina la adivinanza de «esto parece plano».
- **Con qué se reemplaza:** Declarar plain: explícito (identidad). La migración es una línea por valor.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** Core/Configuration/ConnectionStringValue.cs y los casos negativos de ConfigurationContractTests.

### BOLT011-09 — behavior-change *(US-000.BOLT-011.json)*

- **Qué desaparece:** Los fallos de configuración lanzan CooperatorConfigurationException (tipo propio del framework, en Core) y no un tipo de System.Configuration ni un NullReferenceException.
- **Por qué:** ADR-004 §3.2/§3.3 y §5 (config fail-fast).
- **Con qué se reemplaza:** CooperatorConfigurationException, con la clave —y el prefijo ofensor cuando aplica— en el mensaje.
- **Bolt que la ejecuta:** US-000.BOLT-011 · **evidencia:** Core/Configuration/CooperatorConfigurationException.cs y los cuatro casos negativos canónicos de ConfigurationContractTests.

### BOLT014-01 — method-removed *(US-000.BOLT-014.json)* · Cooperator.Framework.Data.BaseRule.InternalDBHelper.SetConnectionStringValue (public void SetConnectionStringValue(string value))

- **Qué desaparece:** Desaparece el metodo publico que fijaba la cadena de conexion de la regla y la guardaba en el Dictionary estatico de la capa. Era la via paralela a la configuracion: el consumidor podia inyectar una cadena en memoria fuera del contrato.
- **Por qué:** ADR-004 3.2/3.3 y ADR-005 3.5 (SPEC-261016-0020 6/D.2): la cadena la entrega el CONTRATO de configuracion portable, ya resuelta y con su prefijo de esquema validado; se eliminan el Dictionary estatico y la heuristica que inspeccionaba su contenido. Un camino paralelo de cadena contradice esa unica via. La ruptura no figuraba en la lista del SPEC: se declara aqui con su reemplazo (ADR-008 3.1).
- **Con qué se reemplaza:** Fijar la configuracion por el contrato (Bolt 011): la implementacion de ICooperatorConfiguration que la app asigna a CooperatorSettings.Current aporta la cadena, y la capa la lee resuelta por GetConnectionString(nombre).
- **Bolt que la ejecuta:** US-000.BOLT-014 · **evidencia:** El gate de compatibilidad reporta la diferencia de miembro (CP0002) de SetConnectionStringValue contra la linea base 1.4, y esta entrada la cubre; el control DataLayerContractTests verifica que BaseRule no conserva el metodo ni el Dictionary.

### BOLT014-02 — behavior-change *(US-000.BOLT-014.json)*

- **Qué desaparece:** Los CUATRO puntos de cache de la capa dejan de usar el cache del host (System.Web.HttpContext): las colecciones de BaseGateway y BaseLoader, el timeout de comandos de SQLHelper y la metadata de parametros de SqlHelperParameterCache. Los puntos quedan SIN CACHE: cada GetAll/GetAllDictionary consulta la base; el timeout se lee del catalogo en cada uso; la metadata se descubre por operacion (SqlCommandBuilder.DeriveParameters). El paquete deja de arrastrar la plataforma web y puede ser netstandard2.1.
- **Por qué:** SPEC-261016-0020 6/C.6 (ampliacion de alcance de la revision 3, sobre la medicion del V-Bounce N°1, MEM-261016-0343 9.1) y ADR-004 3.1 (la decision de cache del framework) mas ADR-001 5: el port no compila en netstandard2.1 con System.Web, asi que el retiro ES condicion de compilacion. No es la sustitucion del Bolt 013: no se introduce ICacheProvider ni una cache interina propia.
- **Con qué se reemplaza:** Nada que hacer en el consumidor: el resultado observable de cada operacion es el mismo (los datos se leen de la base). El EFECTO es de rendimiento: mas consultas donde antes habia cache, y el timeout leido del catalogo (Cooperator:Data:CommandTimeoutSeconds, default documentado 30 s) en cada uso.
- **Paso siguiente obligatorio del consumidor:** La SUSTITUCION de los cuatro puntos por el contrato ICacheProvider (con sus regiones, su capa de entradas, sus TTL y la invalidacion por clave) es del US-000.BOLT-013, que ejecuta DESPUES de este Bolt (secuencia 014-ampliado -> 013).
- **Bolt que la ejecuta:** US-000.BOLT-014 · **evidencia:** DataLayerContractTests verifica 0 HttpContext, 0 System.Web, 0 claves legacy (Coop_DAL_/Cooperator_DAL_) y 0 esperas activas en los cinco archivos de la capa, y el build de Data en netstandard2.1 sin la referencia web.

### BOLT016-01 — signature-changed *(US-000.BOLT-016.json)* · Cooperator.Framework.Modeler.DBReverseHelper.DomainTreeHelper.UpdateDomainTree (public static BaseTreeNode UpdateDomainTree(BaseTreeNode oldTree))

- **Qué desaparece:** La mitad reverse del helper del tooling cambia la FIRMA con la que se le entrega la conexion: era `UpdateDomainTree(string connectionString, BaseTreeNode oldTree)` y pasa a ser `UpdateDomainTree(BaseTreeNode oldTree)`. La sobrecarga que recibia la cadena por parametro queda privada (es implementacion).
- **Por qué:** AD-005 3.6 y SPEC-261016-0027 6/C.1: la cadena de conexion del reverse NO entra por un parametro provisto por un formulario ni por un mecanismo propio del tool, sino por el CONTRATO de configuracion portable (ADR-004 3.2/3.3) - y es el consumidor el que invoca al resolutor de valores protegidos.
- **Con qué se reemplaza:** Llamar `UpdateDomainTree(BaseTreeNode oldTree)`: la cadena la resuelve `ConnectionStringResolver` desde `Cooperator:ConnectionStrings:<nombre>` y su prefijo de esquema se valida con el contrato.
- **Bolt que la ejecuta:** US-000.BOLT-016 · **evidencia:** El tipo y su firma nueva estan en `2.0/src/CooperatorModeler/DBReverseHelper/DomainTreeHelper.cs` (medido por `DependencyChecks.The_four_cross_calls_of_the_reverse_half_are_qualified` y por el build del tooling).

### BOLT016-03 — behavior-change *(US-000.BOLT-016.json)*

- **Qué desaparece:** La conexion del reverse-engineering ya NO se provee por un mecanismo propio del tool: sale de la configuracion portable del framework (el catalogo del contrato), con su prefijo de esquema obligatorio y su resolutor invocado por el consumidor. Para quien usa el tool, 'conectar' deja de tener un camino propio: el valor se publica en la configuracion (ADR-004 3.2/3.3).
- **Por qué:** ADR-004 3.2 (el contrato de configuracion y su catalogo: `Cooperator:ConnectionStrings:<nombre>`) y 3.3 (prefijo de esquema explicito, error tipado y 'la capa que lo consume es la que invoca al resolutor'): el reverse es un consumidor mas, y el Bolt 025 (la UI del shell) es quien da la pantalla de conexion, no este componente.
- **Con qué se reemplaza:** Publicar la cadena en la configuracion del contrato con su prefijo (`plain:` hoy; los esquemas protegidos, del ADR-007) y dejar que el componente la resuelva. No hay accion de migracion de datos: el reverse solo lee esquema.
- **Paso siguiente obligatorio del consumidor:** El Bolt 025 (UI del shell) muestra donde se provee esa configuracion; este Bolt entrega el componente que la consume.
- **Bolt que la ejecuta:** US-000.BOLT-016 · **evidencia:** `ConnectionResolutionTests` (hermetico, con dobles) y `ConnectionStringResolutionTests` (lane de integracion, escrito y compilado); el valor del esquema desconocido FALLA con error tipado en lugar de leerse a ciegas.

## Área — capa-de-datos

**Capa de datos: proveedor único y superficie que pasa a `internal`** — 2 entrada(s).

### BOLT003-09 — deferred-obligation *(US-000.BOLT-003.json)*

- **Qué desaparece:** La reubicación al tooling NO se entrega en este Bolt: `2.0/src/CooperatorModeler/` no existe todavía y con él quedan pendientes la mitad reverse de `DomainTreeHelper` (`UpdateDomainTree`, `GetDomainTree`, `ADDNodeFromDBObject`), `Providers/MSSQLProvider.cs` (`MSSQLReverseFactory`, `MSSQLProvider`), el resto de `Providers/MSSQLCommon.cs` y los `PropertyEditors/*` con sus recursos.
- **Por qué:** SPEC §4/§6/D.3: reubicar el provider tal como está conserva `DataAccess : Cooperator.Framework.Data.BaseRule` —tipo ausente en el árbol 2.0— y `System.Data.SqlClient`, cuya reposición es de ADR-005 §3.1 (Bolt 014). Ni reescribir el reverse (Bolt 016) ni aportar la capa `Data` son decisiones del ejecutor; por eso la ejecución se difiere como obligación registrada en lugar de bloquear.
- **Con qué se reemplaza:** Los Bolts que aportan la capa `Data` y reescriben el reverse: **US-000.BOLT-014** y **US-000.BOLT-016** (`registered_obligations[0]`, con su especificación tipo por tipo en MIGRATION-PARTITION.md §2.2 y sus tres recordatorios obligatorios). Para el consumidor, el efecto ya es visible: `Cooperator.Framework.Utility` no existe en la 2.0 y todo lo que aporta este Bolt vive en `Cooperator.Framework.Modeling`.
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** `2.0/src/CooperatorModeler/` no existe (verificado); el paquete `Modeling` no contiene ningún tipo del tooling ni firma con cadena de conexión (DomainTreeHelperPartitionTests, verde); la obligación está registrada en `registered_obligations[0]` y en MIGRATION-PARTITION.md §2.2/§5.

### BOLT014-04 — behavior-change *(US-000.BOLT-014.json)*

- **Qué desaparece:** Cambios de comportamiento del PROVEEDOR que la migracion debe absorber: (a) Encrypt pasa a Mandatory por defecto y TrustServerCertificate sigue en false, asi que las conexiones contra servidores con certificado autofirmado empiezan a fallar; (b) un parametro con DbType.Time exige TimeSpan (con DateTime deja de funcionar) y DbType.Date trunca la parte horaria; (c) el namespace cambia en todos los tipos del proveedor, con mapeos puntuales (SqlDataRecord/SqlMetaData viven en Microsoft.Data.SqlClient.Server).
- **Por qué:** ADR-005 3.1 (SPEC-261016-0020 6/E.1): el proveedor unico es Microsoft.Data.SqlClient y estos tres cambios son documentados por el proveedor y verificados en su guia de migracion.
- **Con qué se reemplaza:** Para (a): arreglar el certificado del servidor o declarar explicitamente TrustServerCertificate=true / Encrypt=false en la cadena. Para (b): pasar TimeSpan en los parametros de hora y revisar los sitios que usan Date. Para (c): ajustar los using y los mapeos de namespace en el codigo del consumidor.
- **Paso siguiente obligatorio del consumidor:** Verificar los tres contra la base real en el CICLO de test de integracion del Bolt (ProviderBehaviorTests) y leerlos en las notas de migracion del Bolt 028.
- **Bolt que la ejecuta:** US-000.BOLT-014 · **evidencia:** ProviderBehaviorTests (lane de integracion, escrita y compilada en este V-Bounce; su corrida es del ciclo) y el test de armado de parametros de DataLayerContractTests.

## Área — carga-de-ensamblados-y-providers-lazy

**Carga de ensamblados y providers Lazy: resolución, caché de providers y diagnóstico** — 7 entrada(s).

### BOLT005-01 — method-removed *(US-000.BOLT-005.json)* · Cooperator.Framework.Core.Factory.SingleFactory.CreateObject (public static object CreateObject(string assemblyPath, string assemblyName, string className))

- **Qué desaparece:** Desaparece la sobrecarga de carga por RUTA: abría el ensamblado con Assembly.LoadFrom(assemblyPath + assemblyName). Es la superficie que convertía un valor de configuración en ejecución de código desde una ruta arbitraria.
- **Por qué:** ADR-006 §3.1 y §4: el runtime moderno no justifica la carga por ruta y el framework no la tolera; ninguna clave de configuración puede provocar una carga desde una ruta.
- **Con qué se reemplaza:** Ninguno dentro del framework: cargar desde una carpeta o una ruta es la política del HOST y entra por el hook (IAssemblyProvider, registrado una vez con ProviderResolution.Register). El framework sólo resuelve por nombre de un destino declarado.
- **Bolt que la ejecuta:** US-000.BOLT-005 · **evidencia:** No existe en 2.0/src (la búsqueda de Assembly.LoadFrom/LoadFile sobre el código propio da 0, y el control no es vacuo: el 1.4 congelado sí la contiene); el gate la reporta como miembro retirado cubierto por esta entrada (NoImplicitLoadingTests, ApiRemovalCoverageTests).

### BOLT005-02 — method-removed *(US-000.BOLT-005.json)* · Cooperator.Framework.Core.Factory.SingleFactory.CreateObject (public static object CreateObject(string assemblyName, string className))

- **Qué desaparece:** Desaparece la construcción por NOMBRE SIMPLE sin calificar: Assembly.Load(<nombre simple>) + CreateInstance(className, true) con el binder por defecto, que resolvía contra el AppDomain sondeando el directorio base y degradaba cualquier fallo en un NullReferenceException o un 'file not found' lejos de la configuración que lo causó.
- **Por qué:** ADR-006 §3.1/§3.3 (REV-001 F-02): el ALC por defecto no sondea directorios y el nombre debe identificar unívocamente el destino.
- **Con qué se reemplaza:** Cooperator.Framework.Core.LazyLoad.LazyProviderFactory.Get(<ensamblado simple>, <tipo completo calificado>) —que resuelve con Assembly.Load(new AssemblyName(<simple>)) + tipo por nombre completo— y, para el camino por entidad, LazyProviderFactory.GetForEntity(<entidad>), que lee el catálogo de ADR-004 §3.2. La caché pasa a ProviderCache. La construcción tipada que NO carga por nombre (SingleFactory.CreateObject(Type) y CreateObject<T>()) se conserva.
- **Bolt que la ejecuta:** US-000.BOLT-005 · **evidencia:** El ensamblado 2.0 no expone esa sobrecarga (el gate la reporta y esta entrada la cubre); los casos del cuadro de diagnósticos prueban que cada causa produce su excepción tipada con la clave y los nombres intentados (ProviderDiagnosticsTests).

### BOLT005-03 — type-removed *(US-000.BOLT-005.json)* · Cooperator.Framework.Core.Factory.FactoryCache.FactoryCache (public static class FactoryCache (Dictionary<string, object> estático con clave $\"{assemblyName}.{className}\"))

- **Qué desaparece:** Desaparece la caché de instancias con CLAVE CONCATENADA por puntos y sin sincronización: dos pares distintos podían colisionar ((\"A.B\",\"C\") vs (\"A\",\"B.C\")) y el Dictionary estático podía corromperse bajo concurrencia.
- **Por qué:** ADR-006 §3.2 (REV-001 F-02): la caché es thread-safe, con clave por nombre calificado, con reinicio explícito y sin cachear fallos.
- **Con qué se reemplaza:** Cooperator.Framework.Core.LazyLoad.ProviderCache: una instancia por provider y por proceso, clave por ProviderName (igualdad estructural sobre el par, sin colisión), thread-safe y con ProviderCache.Reset() explícito.
- **Bolt que la ejecuta:** US-000.BOLT-005 · **evidencia:** ProviderCacheTests: corrida concurrente con conteo de instancias (una sola), pares que colisionaban como claves distintas, reinicio explícito y fallo no cacheado. El tipo FactoryCache no existe en 2.0/src.

### BOLT005-04 — behavior-change *(US-000.BOLT-005.json)* · Cooperator.Framework.Core.LazyLoad.ProviderResolver ((sin superficie: es una semántica de resolución))

- **Qué desaparece:** El SONDEO IMPLÍCITO por directorio base desaparece como camino de resolución: un ensamblado presente en el despliegue pero no declarado como dependencia ya no se encuentra. Y el mecanismo de la carpeta CooperatorAssemblies deja de existir en runtime (su contraparte de compilación ya murió con los PackageReference).
- **Por qué:** ADR-006 §3.1/§5 (NFR no-implicit-assembly-loading): 0 llamadas a LoadFrom/LoadFile y 0 sondeo de directorio en el camino por defecto.
- **Con qué se reemplaza:** Declarar el provider como DEPENDENCIA de la aplicación (el caso de la plantilla DefaultLazyProvider: el provider se genera en el proyecto de reglas del cliente) o registrar el hook del host con su propia política de carga. Es un cambio de despliegue deliberado y documentado (ADR-006 §4).
- **Bolt que la ejecuta:** US-000.BOLT-005 · **evidencia:** NoImplicitLoadingTests: el ensamblado de prueba existe en una subcarpeta del despliegue y NO está declarado (0 ocurrencias en el deps.json); la resolución por defecto falla con el nombre intentado y la causa del runtime, y el hook del host —con su AssemblyLoadContext— sí lo obtiene.

### BOLT005-05 — member-added *(US-000.BOLT-005.json)* · Cooperator.Framework.Core.LazyLoad.LazyProviderFactory.GetForEntity (public static ILazyProvider GetForEntity(string entity))

- **Qué desaparece:** Se AGREGA al contrato de la fábrica el camino por ENTIDAD: resuelve los nombres del catálogo de configuración (Cooperator:LazyLoad:Providers:<entidad>:Assembly y …:Class, ADR-004 §3.2) y es el camino donde la ausencia de configuración tiene diagnóstico propio con la entidad y la clave esperada.
- **Por qué:** SPEC-261016-0015 §6/A.3 y §14: el código generado ya invoca la fábrica por nombre explícito y el camino por entidad es el que hace que la ProviderNotConfiguredException tenga sentido (ADR-006 §3.3, primera fila).
- **Con qué se reemplaza:** No reemplaza nada: es una ADICIÓN a la superficie del contrato de la fábrica (el gate de compatibilidad la reporta como diferencia de miembro en la dirección de adición). Un consumidor puede seguir usando LazyProviderFactory.Get(ensamblado, tipo) como hasta ahora.
- **Bolt que la ejecuta:** US-000.BOLT-005 · **evidencia:** ProviderResolutionTests: el camino por entidad resuelve por la vía del host y reporta la entidad y la clave esperada cuando falta configuración; el gate lo reporta como diferencia de miembro cubierta por esta entrada (ApiRemovalCoverageTests).

### BOLT008-01 — behavior-change *(US-000.BOLT-008.json)*

- **Qué desaparece:** La identificación del tipo de una columna del DataTable deja de reconstruir el nombre del tipo desde el ToString() de la reflexión (Substring(18, len-19) sobre el nombre de un Nullable) y de resolverlo después por NOMBRE SIMPLE con Type.GetType: ahora usa el TIPO DISPONIBLE (el PropertyType de la propiedad) y, para un Nullable<T>, su tipo subyacente resuelto por API (Nullable.GetUnderlyingType). La clasificación de una propiedad como de sistema o de negocio usa Type.Namespace en vez del formato del ToString() del tipo.
- **Por qué:** ADR-006 §3.1 sobre el hallazgo F-47 (REV-001 §6 ítem 19): la unicidad del nombre de un tipo se DECLARA, no se hereda; Type.GetType con un nombre simple resuelve sólo contra el ensamblado llamador y el núcleo, de modo que el resultado del mapeo dependía de dónde vive el tipo (y de un Substring que asumía el formato de un nombre de reflexión). ADR-001 §3.2 declara que la resolución de tipos cambia de comportamiento entre runtimes: por eso la unicidad se declara.
- **Con qué se reemplaza:** Nada que hacer en el consumidor para ENTRADAS VÁLIDAS: el comportamiento observable no cambia (la suite de caracterización corre antes y después sobre el corpus sin diferencias). Cambia el caso que hoy dependía de la casualidad: un Nullable de un tipo de otro ensamblado, que antes producía un DataColumn de tipo nulo (o resolvía si el tipo vivía en el ensamblado llamador), ahora resuelve por construcción con el tipo subyacente correcto.
- **Paso siguiente obligatorio del consumidor:** Si un componente emite nombres de tipo hacia este camino (plantillas/modelo), debe emitirlos CALIFICADOS (con su ensamblado): una forma no calificada es un hallazgo con dueño (Bolts de plantillas/modelo), no una tolerancia del resolutor.
- **Bolt que la ejecuta:** US-000.BOLT-008 · **evidencia:** TypeIdentificationTests (el Nullable de un tipo del ensamblado de pruebas resuelve por API y su nombre simple NO resuelve con Type.GetType), RealValuesOverCorpusTests (la columna del DataSet se crea con el tipo subyacente del Nullable y admite nulos) y la caracterización del Bolt 001 verde en la misma corrida.

### BOLT008-02 — behavior-change *(US-000.BOLT-008.json)*

- **Qué desaparece:** Un nombre de propiedad o de columna que no corresponde a ninguna propiedad del tipo deja de producir ArgumentException("<{0}> property not found.") —que no nombraba ni el tipo ni el punto— y, en el gemelo del llenado del DataSet, deja de quedar tapado por un InvalidDataSetException genérico proveniente de un catch (Exception). Ahora los DOS puntos fallan con TypeResolutionException, que nombra el valor intentado (la propiedad o la columna), el tipo y el punto que lo pidió, y enumera las propiedades disponibles del tipo.
- **Por qué:** ADR-006 §3.3 (diagnósticos) y el objeto del Bolt: el fallo deja de ser un silencio o un mensaje que obliga a depurar. Un nombre de tipo que llega de afuera, además, debe ser CALIFICADO: admitirlo reintroduciría la resolución implícita que ADR-006 §3.1 elimina.
- **Con qué se reemplaza:** Capturar CoreException (la jerarquía del núcleo se conserva: TypeResolutionException deriva de CoreException) o corregir el nombre. El error genérico InvalidDataSetException queda SÓLO para lo que efectivamente es un desajuste de esquema del DataSet.
- **Bolt que la ejecuta:** US-000.BOLT-008 · **evidencia:** PropertyLookupDiagnosticsTests verifica los dos puntos (el comparador de orden y el gemelo del llenado del DataSet) con el valor, el tipo y el origen en la excepción; Type.GetType y GetProperty dejaron de usarse en el camino (NoImplicitResolutionTests, control de 0 resolución implícita).

## Área — criptografia

**Criptografía: retiro de la superficie estática y migración de los valores ya cifrados** — 5 entrada(s).

### BOLT022-01 — method-removed *(US-000.BOLT-022.json)* · Cooperator.Framework.Library.Cryptography.Encrypt (public static string Encrypt(string sourceString))

- **Qué desaparece:** Desaparece la sobrecarga de cifrado con clave IMPLÍCITA: la clave vivía dentro del producto (una constante embebida en el IL, derivada por MD5) y el llamador no declaraba con qué clave se protegía el valor. Es la sobrecarga que usaba el encriptador de 1.4 y la que hacía posible cifrar sin referencia de clave.
- **Por qué:** ADR-007 §3.1: se retira la API estática con clave implícita (`Cryptography.Encrypt(string)`/`Decrypt(string)`) y con ella el camino por el que una clave vive dentro del producto; proteger y resolver pasan a exigir una referencia explícita de clave (`kid` + proveedor) y el resolutor del esquema. Es la materialización del defecto de F-23 que este Bolt convierte en estándar.
- **Con qué se reemplaza:** Los esquemas con referencia explícita de clave: `plain:` para lo que no necesita protección, o `aesgcm:<kid>:<base64>` con la clave ACTIVA del registro y su proveedor (entorno u hook del host). Proteger es `AesGcmValueResolver.Protect(claveDeConfiguración, valor)`: la clave de configuración viaja como AAD y ata el valor a su slot.
- **Paso siguiente obligatorio del consumidor:** Quien tenga valores cifrados con el formato de 1.4 los migra con la herramienta del Bolt 023 (inventario, clasificación, `--dry-run`, re-protección con respaldo): el framework ya no lee ese formato ni ofrece un camino de descifrado alternativo.
- **Bolt que la ejecuta:** US-000.BOLT-022 · **evidencia:** El tipo `Cooperator.Framework.Library.Cryptography` ya no existe en el ensamblado construido (verificado por reflexión en `KeyProvisionTests`), y la búsqueda de la clave del formato legacy sobre el árbol 2.0 da 0 apariciones (`KeyProvisionTests.No_key_material_lives_in_the_tree_and_the_legacy_key_does_not_reappear`).

### BOLT022-02 — method-removed *(US-000.BOLT-022.json)* · Cooperator.Framework.Library.Cryptography.Decrypt (public static string Decrypt(string encryptedString))

- **Qué desaparece:** Desaparece la sobrecarga de descifrado con clave IMPLÍCITA, la contraparte de la anterior: resolvía cualquier valor con la clave embebida del binario, con IV fijo y encoding ASCII. Un valor manipulado devolvía basura silenciosa en lugar de un error.
- **Por qué:** ADR-007 §3.1 (mismo retiro) y §3.4 (IV fijo y las primitivas legacy quedan prohibidas en el árbol 2.0; la excepción de descifrado legacy vive sólo en el componente interno de la herramienta del Bolt 023).
- **Con qué se reemplaza:** Resolver por el resolutor del esquema: `ProtectedValueResolvers.Resolve(claveDeConfiguración, valorCrudo)` para `plain:` y `aesgcm:`, con verificación de integridad ANTES de devolver el valor (un valor manipulado o movido de slot produce error tipado, nunca basura ni texto plano).
- **Paso siguiente obligatorio del consumidor:** Los valores existentes, cifrados con el formato de 1.4, se migran con la herramienta (Bolt 023), que es la única pieza autorizada a descifrar el formato viejo, con bandera explícita y registro de retiro.
- **Bolt que la ejecuta:** US-000.BOLT-022 · **evidencia:** No existe en el ensamblado construido de `Library` (mismo control que BOLT022-01); el ensamblado conserva únicamente la superficie nueva de valores protegidos.

### BOLT022-03 — type-removed *(US-000.BOLT-022.json)* · Cooperator.Framework.Library.Cryptography

- **Qué desaparece:** Desaparece el tipo completo `Cooperator.Framework.Library.Cryptography` (clase estática pública). Existía únicamente como el mecanismo legacy: la clave embebida y el IV fijo como campos privados, las sobrecargas con clave explícita sobre TripleDES con clave derivada por MD5, el encoding ASCII y los tipos `*CryptoServiceProvider` concretos. Sin las cuatro sobrecargas no quedaba nada que no fuera material de clave y primitivas prohibidas.
- **Por qué:** ADR-007 §3.1 (el retiro de la superficie estática y del camino por el que una clave vive dentro del producto) y §3.4 (el estándar de primitivas es el que TODO el árbol 2.0 debe cumplir, y la excepción de descifrado legacy está acotada al componente interno de la herramienta, nunca en `Library`). ADR-008 §5: el gate de compatibilidad reporta la ausencia del tipo y esta entrada la explica.
- **Con qué se reemplaza:** Ninguno dentro del framework: el framework no conserva ningún mecanismo de cifrado legacy. La superficie nueva de valores protegidos (esquema `aesgcm:` con `System.Security.Cryptography.AesGcm`) es el reemplazo para proteger y resolver, y el descifrado del formato de 1.4 existe sólo en la herramienta de migración (Bolt 023).
- **Paso siguiente obligatorio del consumidor:** Migrar los valores ya cifrados con la herramienta del Bolt 023, y re-proteger con el secreto fuera del producto (entorno u hook del host).
- **Bolt que la ejecuta:** US-000.BOLT-022 · **evidencia:** `2.0/src/Cooperator.Framework.Library/Cryptography.cs` retirado; 0 apariciones de la clave del formato legacy y de sus derivaciones en el árbol 2.0; el gate de API reporta el tipo ausente y `MigrationNotesCoverageTests` cruza esta entrada contra esa diferencia.

### BOLT024-01 — behavior-change *(US-000.BOLT-024.json)*

- **Qué desaparece:** La herramienta de escritorio de protección de valores cambia de SUPERFICIE DE USUARIO y de ORIGEN DE LA CLAVE: pasa de la aplicación WinForms anterior —que cifraba con la superficie estática de Library y la clave que venía embebida en el framework— a un shell delgado WPF (net10.0-windows) con un único paso: el operador aporta el valor (enmascarado) y el nombre completo de la clave de configuración de destino (que es el AAD del valor), y la clave activa la aporta la provisión del motor desde el entorno (o el hook del host). La herramienta ya no cifra: delega íntegramente en el motor del Bolt 022 y escribe un valor aesgcm:<kid>:<base64>.
- **Por qué:** ADR-009 §3.6 (el ConnectionStringEncriptor sobrevive como shell delgado sobre el MISMO motor de protección; no hay dos implementaciones ni dos formatos) y ADR-007 §3.5 (la herramienta protege escribiendo un valor aesgcm: con la clave activa, sin clave embebida). ADR-001 §3.5 retira System.Windows.Forms del tooling y ADR-001 §3.1 lo mantiene entre las aplicaciones Windows-only que sobreviven. El defecto de origen es F-23 (REV-001): clave literal e IV fijo en el binario.
- **Con qué se reemplaza:** El paso único de la utilidad (ProtectValueStep), que consume la MISMA operación del motor y produce el MISMO formato que el comando del CLI (mismo resolutor, misma composición): el valor que produce se resuelve con el resolutor del framework en la clave de configuración indicada. La clave activa se provee por la variable de entorno del proveedor por defecto (o por el hook del host): la utilidad consume la REFERENCIA (kid), nunca el material.
- **Paso siguiente obligatorio del consumidor:** Quien tenga valores cifrados con el formato viejo (la ofuscación de 1.4) los migra con la herramienta del Bolt 023 —inventario, clasificación, re-protección con respaldo—: la utilidad de escritorio NO lee ni escribe el formato anterior y no ofrece un modo de compatibilidad.
- **Bolt que la ejecuta:** US-000.BOLT-024 · **evidencia:** ProtectValueCycleTests (ciclo completo: proteger con la clave activa y RESOLVER lo protegido con el resolutor del motor, incluidos valores no ASCII, largos y vacíos) y ProtectionDelegationTests (0 primitivas, 0 claves, 0 composición propia; el formato es el del motor; el cambio de superficie declarado acá).

### BOLT024-02 — behavior-change *(US-000.BOLT-024.json)*

- **Qué desaparece:** El manejo del valor de entrada cambia de naturaleza: la utilidad anterior pasaba la credencial a una superficie cifradora con clave implícita; la nueva la trata como material sensible transitorio —se captura enmascarada, no se muestra en claro, no se persiste en disco, estado de usuario, historial ni registros, y se limpia al cerrar el paso— y, si el motor no puede proteger (clave activa ausente o mal formada, kid sin clave, proveedor del host que falla), muestra un error accionable con la referencia afectada y NO produce ningún valor.
- **Por qué:** ADR-007 §3.5 (las herramientas protegen con el esquema vigente y la clave activa, sin clave embebida, con el kid y el esquema de salida como reproducibilidad) y §3.2 (nada de secretos en logs ni reportes: los reportes identifican por clave de configuración y kid, nunca por contenido). SPEC-261016-0043 §10: el control central de la utilidad es no persistir el valor en claro (AC-3).
- **Con qué se reemplaza:** El paso único + su diagnóstico (ProtectionDiagnostics): el operador ve qué falta y qué hacer, con la referencia, y nunca el valor ni material de clave; el valor protegido es el único artefacto que se lleva.
- **Bolt que la ejecuta:** US-000.BOLT-024 · **evidencia:** ProtectValueCycleTests.The_clear_value_never_lands_in_disk_state_or_logs (corrida instrumentada: búsqueda del valor sintético en lo escrito = 0, sin APIs de persistencia ni de registro en la utilidad) y ProtectionDelegationTests.The_three_engine_failures_are_actionable_and_carry_no_value_or_material.

## Área — superficie-de-serializacion

**Superficie de serialización: lo que se retira y qué lo reemplaza** — 33 entrada(s).

### BOLT003-04 — serialization-removal *(US-000.BOLT-003.json)*

- **Qué desaparece:** Los tipos del grafo y las excepciones que migran dejan de ser serializables en formato binario: se retiran `[Serializable]`, `SerializationInfo`, `ISerializable` y los constructores `(SerializationInfo, StreamingContext)`.
- **Por qué:** ADR-008 §3.2: los tipos que migran nacen POCOs; su persistencia es el `.coop2`, con los derivados recalculados al leer.
- **Con qué se reemplaza:** El formato `.coop2` (contrato del Bolt 004). El retiro equivalente en el resto de la superficie del framework es del Bolt 007.
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** ModelPocoTests (0 `[Serializable]`/`ISerializable`/`SerializationInfo` en el ensamblado) y el build sin ningún `SYSLIB0051` (ADR-008 §3.4).

### BOLT007-01 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.CoreException.ctor (CoreException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (CoreException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/CoreException.cs del 1.4 (sólo lectura).

### BOLT007-02 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.InvalidDataSetException.ctor (InvalidDataSetException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidDataSetException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidDataSetException.cs del 1.4 (sólo lectura).

### BOLT007-03 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.InvalidFilterArgumentException.ctor (InvalidFilterArgumentException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidFilterArgumentException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidFilterArgumentException.cs del 1.4 (sólo lectura).

### BOLT007-04 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.InvalidRowStateException.ctor (InvalidRowStateException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidRowStateException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidRowStateException.cs del 1.4 (sólo lectura).

### BOLT007-05 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.InvalidSortArgumentException.ctor (InvalidSortArgumentException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidSortArgumentException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidSortArgumentException.cs del 1.4 (sólo lectura).

### BOLT007-06 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.ItemNotFoundInListException.ctor (ItemNotFoundInListException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (ItemNotFoundInListException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/ItemNotFoundInListException.cs del 1.4 (sólo lectura).

### BOLT007-07 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.Exceptions.RuleValidationException.ctor (RuleValidationException (SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (RuleValidationException (SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/RuleValidationException.cs del 1.4 (sólo lectura).

### BOLT007-08 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Core.for

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y los tres [field: NonSerialized] de sus eventos públicos (metadatos que existían sólo para la serialización binaria). El tipo NO tiene constructor de serialización (verificado).
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: BaseObject.cs del 1.4 (sólo lectura).

### BOLT007-09 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.ConnectionStringNotFoundException.ctor (ConnectionStringNotFoundException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (ConnectionStringNotFoundException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/ConnectionStringNotFoundException.cs del 1.4 (sólo lectura).

### BOLT007-10 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.DangerousQueryException.ctor (DangerousQueryException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (DangerousQueryException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/DangerousQueryException.cs del 1.4 (sólo lectura).

### BOLT007-11 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.DataException.ctor (DataException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (DataException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/DataException.cs del 1.4 (sólo lectura).

### BOLT007-12 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.DeleteCanceledException.ctor (DeleteCanceledException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (DeleteCanceledException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/DeleteCanceledException.cs del 1.4 (sólo lectura).

### BOLT007-13 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.InsertCanceledException.ctor (InsertCanceledException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InsertCanceledException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InsertCanceledException.cs del 1.4 (sólo lectura).

### BOLT007-14 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.InsufficientSecurityRightsException.ctor (InsufficientSecurityRightsException (SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InsufficientSecurityRightsException (SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InsufficientSecurityRihgtsException.cs del 1.4 (sólo lectura).

### BOLT007-15 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.InvalidConnectionStringException.ctor (InvalidConnectionStringException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidConnectionStringException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidConnectionStringException.cs del 1.4 (sólo lectura).

### BOLT007-16 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.InvalidMappingException.ctor (InvalidMappingException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (InvalidMappingException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/InvalidMappingException.cs del 1.4 (sólo lectura).

### BOLT007-17 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.NoRowAffectedException.ctor (NoRowAffectedException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (NoRowAffectedException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/NoRowAfectedException.cs del 1.4 (sólo lectura).

### BOLT007-18 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.RowConcurrenceException.ctor (RowConcurrenceException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (RowConcurrenceException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/RowConcurrenceException.cs del 1.4 (sólo lectura).

### BOLT007-19 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.RowDoNotExistException.ctor (RowDoNotExistException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (RowDoNotExistException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/RowDoNotExistException.cs del 1.4 (sólo lectura).

### BOLT007-20 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.SqlQueriesDisabledException.ctor (SqlQueriesDisabledException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (SqlQueriesDisabledException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/SqlQueriesDisabledException.cs del 1.4 (sólo lectura).

### BOLT007-21 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.TableHaveNoParentException.ctor (TableHaveNoParentException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (TableHaveNoParentException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/TableHaveNoParentException.cs del 1.4 (sólo lectura).

### BOLT007-22 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.TableIsReadOnlyException.ctor (TableIsReadOnlyException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (TableIsReadOnlyException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/TableIsReadOnlyException.cs del 1.4 (sólo lectura).

### BOLT007-23 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.TooMuchRowsInQueryException.ctor (TooMuchRowsInQueryException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (TooMuchRowsInQueryException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/TooMuchRowsInQueryException.cs del 1.4 (sólo lectura).

### BOLT007-24 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Data.Exceptions.UpdateCanceledException.ctor (UpdateCanceledException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (UpdateCanceledException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/UpdateCanceledException.cs del 1.4 (sólo lectura).

### BOLT007-25 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.ApplicationErrorException.ctor (ApplicationErrorException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (ApplicationErrorException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/ApplicationErrorException.cs del 1.4 (sólo lectura).

### BOLT007-26 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.BaseException.ctor (BaseException(SerializationInfo info, StreamingContext context) : base)

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (BaseException(SerializationInfo info, StreamingContext context) : base) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/BaseException.cs del 1.4 (sólo lectura).

### BOLT007-27 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.CryptoErrorException.ctor (CryptoErrorException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (CryptoErrorException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/CryptoErrorException.cs del 1.4 (sólo lectura).

### BOLT007-28 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.FileAlreadyExist.ctor (FileAlreadyExist(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (FileAlreadyExist(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/FileAlreadyExist.cs del 1.4 (sólo lectura).

### BOLT007-29 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.ItemExistButDiferentTypeException.ctor (ItemExistButDiferentTypeException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (ItemExistButDiferentTypeException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/ItemExistsButDiferentTypeException.cs del 1.4 (sólo lectura).

### BOLT007-30 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.NullValueNotAllowedException.ctor (NullValueNotAllowedException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (NullValueNotAllowedException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/NullValueNotAllowed.cs del 1.4 (sólo lectura).

### BOLT007-31 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.RegistryIOErrorException.ctor (RegistryIOErrorException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (RegistryIOErrorException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/RegistryIOErrorException.cs del 1.4 (sólo lectura).

### BOLT007-32 — serialization-surface-removed *(US-000.BOLT-007.json)* · Cooperator.Framework.Library.Exceptions.SmtpErrorException.ctor (SmtpErrorException(SerializationInfo info, StreamingContext context))

- **Qué desaparece:** Desaparecen la superficie de serialización binaria del tipo: el atributo [Serializable] y el constructor de serialización (SmtpErrorException(SerializationInfo info, StreamingContext context)) — de visibilidad protected —, más el using System.Runtime.Serialization que quedaba sin uso.
- **Por qué:** ADR-008 §3.2 (los tipos del framework migran como POCOs y su persistencia es el .coop2), §3.4 (la obsolescencia SYSLIB0051 no se admite: el retiro es la respuesta) y §3.3 (el framework no expone ningún serializador).
- **Con qué se reemplaza:** Ninguno dentro del framework: no expone ningún serializador (ADR-008 §3.3) y no acepta atributos de contrato. Un consumidor que necesitara serializar el objeto usa el formato .coop2 (contrato del Bolt 004) o su propio mecanismo en su capa.
- **Bolt que la ejecuta:** US-000.BOLT-007 · **evidencia:** 0 residuos de superficie de serialización en 2.0/src; los tres ensamblados compilan EXIT=0 con 0 diagnósticos; el gate de superficie (SerializationSurfaceTests) y el control de rastro (BinaryTrailTests) están verdes. Fuente: Exceptions/SmtpErrorException.cs del 1.4 (sólo lectura).

## Área — otras-rupturas

**Otras rupturas (su decisión no está en el mapa de §6/A.1)** — 3 entrada(s).

### BOLT003-06 — type-split *(US-000.BOLT-003.json)*

- **Qué desaparece:** `DomainTreeHelper` deja de ser una sola clase: se parte en dos tipos con el MISMO nombre en dos ensamblados —la mitad de árbol (11 métodos + el caché de namespaces) en `Cooperator.Framework.Modeling.Model`, y la mitad reverse (3 métodos: `UpdateDomainTree`, `GetDomainTree`, `ADDNodeFromDBObject`) en el tooling cuando la fase D se materialice—.
- **Por qué:** SPEC §6/B.2 (partición por criterio objetivo, aprobada en la revisión 2): la mitad que el modelo invoca en ocho sitios no puede vivir fuera del paquete portable.
- **Con qué se reemplaza:** Los métodos que el modelo usaba (`CorrectCLRType`, `CorrectSelectedCollections`, `GetFullGenerateAsForEntity`, `FindEntityInTree`) están en `Cooperator.Framework.Modeling.Model.DomainTreeHelper`. La mitad reverse la cierra la fase D (obligación registrada).
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** DomainTreeHelperPartitionTests: los 11 métodos y el caché están en el paquete, los 3 reverse NO, y los ocho sitios del modelo se resuelven dentro (mismo resultado observable).

### BOLT003-08 — build-policy *(US-000.BOLT-003.json)*

- **Qué desaparece:** Los archivos del 1.4 portados a `Modeling` declaran `#nullable disable` (una línea por archivo, al frente); el código NUEVO de la 2.0 conserva la nulabilidad activa del árbol.
- **Por qué:** SPEC §6/A.3 (decisión de la revisión 3): el árbol hereda `Nullable=enable` + `TreatWarningsAsErrors=true` y el código del 1.4 es anterior a las anotaciones (28 diagnósticos medidos).
- **Con qué se reemplaza:** Ninguno para el consumidor: es una directiva interna del paquete. Queda prohibido `<Nullable>disable</Nullable>` en cualquier `.csproj`.
- **Bolt que la ejecuta:** US-000.BOLT-003 · **evidencia:** Los 8 archivos portados la llevan y `Reverse/TypeMapping.cs` (código nuevo) no; 0 `.csproj` con `<Nullable>disable</Nullable>`; build del paquete EXIT=0 con 0 advertencias.

### BOLT016-02 — signature-changed *(US-000.BOLT-016.json)* · Cooperator.Framework.Modeler.DBReverseHelper.DomainTreeHelper.GetDomainTree (public static BaseTreeNode GetDomainTree(BaseTreeNode oldDomainTree))

- **Qué desaparece:** Mismo cambio en el otro punto de entrada del componente: era `GetDomainTree(string connectionString, BaseTreeNode oldDomainTree)` y pasa a ser `GetDomainTree(BaseTreeNode oldDomainTree)`.
- **Por qué:** AD-005 3.6 y SPEC-261016-0027 6/C.1: un lector de esquema no recibe la credencial de conexion por el borde publico del tool; la toma del contrato de configuracion portable, que es de donde la toma el resto de la 2.0.
- **Con qué se reemplaza:** Llamar `GetDomainTree(BaseTreeNode oldDomainTree)`; para resolver un nombre de cadena distinto del por defecto, publicar el valor en el catalogo del contrato (`Cooperator:ConnectionStrings:<nombre>`).
- **Bolt que la ejecuta:** US-000.BOLT-016 · **evidencia:** La firma nueva y los dos puntos de entrada estan en el mismo archivo; el caso `ConnectionResolutionTests` ejercita la resolucion con dobles del contrato y del resolutor.

## Registro del escape-hatch (ADR-008 §3.6)

### US-000.BOLT-007

```json
{
  "granted": false,
  "criteria": [
    "consumidor identificado con evidencia",
    "imposibilidad demostrada de resolverlo de forma portable",
    "nunca como mecanismo de persistencia nuevo",
    "test del escenario con su retiro",
    "vencimiento declarado"
  ],
  "note": "Ningún tipo reclama el escape-hatch de ADR-008 §3.6 y los cinco criterios no se cumplen para ninguno: la verificación es la misma que la del gate de superficie —no queda atributo ni constructor que lo insinúe (0 [Serializable], 0 GetObjectData, 0 constructores de serialización, 0 contexto transportado). La vía para una futura excepción es el Bolt que la pida, con esa evidencia."
}
```

Un fragmento **sin** declaración de escape-hatch no afirma que no haya excepciones: la declaración es un acto
explícito (la decisión negativa, o las excepciones con sus **cinco criterios** y su **vencimiento**).

---

Generado por 2.0/build/migration-notes.ps1 desde los fragmentos del registro (SPEC-261016-0047, US-000.BOLT-028).
