---
id: "MEM-261016-0325"
title: "Resolutor de valores protegidos `aesgcm:` (SPEC rev.2) — V-Bounce N°1: AEAD con AAD por clave de configuración, provisión externa de claves y retiro de la superficie estática"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-022" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0035" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 2 # revision of the canonical SPEC used by this V-Bounce (revisión 2 = nulabilidad preventiva §E.3)
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "ready_for_review" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-007", "ADR-004", "ADR-001"] # ADR-007 §3.1/§3.2/§3.4/§5 (mecanismo, esquemas, claves, primitivas y NFRs), ADR-004 §3.3 (forma del prefijo y quién invoca al resolutor), ADR-001 §3.4/§3.5 (dónde vive el contrato y la implementación, capa portable sin terceros)
manifest: "US-000.BOLT-022-resolutor-aesgcm.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # Working tree only — no git operations (G34); the human owns the commit
review_ready_at: "2026-10-16T03:25:37-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T03:28:25-03:00"
  decided_at: "2026-10-16T03:28:25-03:00"
  findings:
    - "Compliant — gate pre-MEM y grant verificado: V-Bounce N°1 `ready_for_review` (Development Completed), SPEC `SPEC-261016-0035` revisión 2 gobernante (nulabilidad preventiva §E.3), baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Ejecutor ≠ revisor (independencia; riesgo `high`)."
    - "Compliant — evidencia REPRODUCIDA: `pwsh -File 2.0/build/build.ps1 -Scope Hermetic` → **135/135 harness + 83/83 caracterización, exit 0** (16 proyectos, **218 casos**, 20 nuevos); `Cryptography.cs` **retirado** (ausente de `2.0/src/Library`); **0** apariciones de la clave legacy en `2.0/src`."
    - "ADJUDICACIÓN (§E.3 no ejercida) — **CORRECTO**: no hubo aporte del 1.4 (la intervención fue un **retiro** de `Cryptography.cs`, no un porteo); 0 directivas `#nullable disable`, 0 `.csproj` con la etiqueta, build limpio (16 proyectos, 0 diagnósticos) — el criterio objetivo (*archivo aportado + diagnóstico*) produjo lista **vacía** por construcción, y queda constancia explícita (MEM §8.1)."
    - "ADJUDICACIÓN (retiro del tipo completo) — la justificación **se sostiene**: la clave y el IV eran campos privados que usaban las sobrecargas retiradas **y** las con clave, y estas últimas son 3DES+MD5+IV fijo+`*CryptoServiceProvider` concretos —**prohibidos** en cualquier paquete desplegado por ADR-007 §3.4—; conservarlas habría dejado material de clave y primitivas prohibidas en el producto (contra AC-5 y el NFR *No embedded secrets*). El fragmento registra las **3 caras** (2 sobrecargas implícitas + el tipo) con reemplazo y paso siguiente."
    - "Compliant — entregables verificados: fragmento `US-000.BOLT-022.json` con **3 entradas** (BOLT022-01/02 `method-removed`, BOLT022-03 `type-removed`) + **1 obligación** (`BOLT022-OBL-01` → el catálogo del 011 sólo admite `plain:` en la forma, **declarada con dueño, no corregida**); mapa actualizado (ADR-007 §3.1 `covered_by` 022; §3.3 re-ruteada a BOLT-023); consolidado **610 líneas / 7 fragmentos / 66 entradas** y **determinista** (regenerado → idéntico)."
    - "CORRECCIÓN del timeline (registrada, sin editar filas): el ejecutor **adelantó** la transición `in-development → ready-for-acceptance` en la fila **T05** (cierre, 03:25:37), antes del `AITL-MEM-Approval` — el contrato espera `in-development` durante la pausa. El estado final `ready-for-acceptance` es el correcto (`test_integration: not-required`) y este `AITL-MEM-Approval` (Development Completed) lo legitima. DoD + MEM completo + manifiesto válido; `1.4/**` intacto."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0325 — Resolutor de valores protegidos `aesgcm:` (rev.2)

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-022](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — `type: non-functional`, `work_category: hardening`, `risk_class: high`, `data_classification: confidential`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0035](../spec/SPEC-261016-0035-resolutor-aesgcm.md), **revisión 2** (aprobada `2026-10-16T03:15:52-03:00`; la revisión 2 es **preventiva**: autoriza la política de nulabilidad de §E.3 **antes** del aporte del 1.4) |
| **V-Bounce**    | **1** — el primero del Bolt |
| **ADRs**        | [ADR-007](../adrs/ADR-007-politica-criptografica.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.6) |

---

## 1. Executive summary

El framework tiene **estándar criptográfico**: el **resolutor de valores protegidos** del esquema **`aesgcm:`**
—**AES-GCM de 256 bits**, **nonce aleatorio de 12 bytes por operación**, **tag de 16 bytes verificado antes de
usar**, **AAD = la clave de configuración** que contiene el valor y carga publicada
`aesgcm:<kid>:<base64>` con **`nonce | tag | ciphertext`**— con **provisión externa de claves** (variables de
entorno por defecto; hook del host para un almacén propio), **clave activa + conjunto de lectura** con **ventana de
solapamiento fechada**, **punto de registro por esquema con fijación única al arranque** (`plain:` identidad +
`aesgcm:` del framework, los del host por el mismo punto), **cuadro de errores tipados sin filtrado** (clave de
configuración y `kid`, nunca el valor ni el material) y el **retiro de la superficie estática con clave implícita**
—con su constancia en las notas de migración—.

La evidencia pedida por §6/E.2 está corrida: la **batería de casos negativos** con su tipo de excepción (manipulación
del ciphertext, del tag, tag truncado, Base64 inválido, carga por debajo del mínimo, AAD de otra clave de
configuración, `kid` ausente/desconocido/sin clave, clave de entorno mal formada, esquema sin resolvedor, fallos del
hook del host, valor sin prefijo) y la verificación de que **ningún mensaje filtra** el valor ni el material; la
**corrida de nonces** (**1 000** protecciones del mismo valor con la misma clave ⇒ **1 000** nonces distintos, y el
nonce no es constante ni derivable del texto plano); la **provisión** por entorno y por hook con sus roles
(activa/lectura) y sus fallos; la **verificación de material** (**0** apariciones de la clave del formato legacy del
1.4 en el árbol 2.0 —en cualquier forma— y **0** constantes de alta entropía con forma de clave en las fuentes del
producto) y la **ausencia de la superficie estática** en el ensamblado construido y en el diff del gate de API.

La suite queda **verde: 135/135 del arnés + 83/83 de caracterización = 218 casos** (20 nuevos), 16 proyectos sin
advertencias y `1.4/**` intacto. Y el registro de rupturas **creció por contribución**: el fragmento de este Bolt
(3 entradas + 1 obligación registrada), la actualización del mapa (la declaración de ADR-007 §3.1 pasó de
`pendiente` a **cubierta**, y la de §3.3 se **re-ruteó** a su dueño real, el Bolt 023) y el documento consolidado
(610 líneas).

**Lo que este Bolt NO hizo** (y es su exclusión explícita): no migró ni inspeccionó ningún valor del formato de 1.4,
no habilitó la **capacidad legacy** (vive aislada y exceptuada en la herramienta del Bolt 023), no corrigió el
defecto de F-23 (tiene su propio BUG/Bolt; este Bolt es el **estándar** contra el que se verifica) y **no tocó
`Core`** (el contrato del Bolt 011 se consume tal cual).

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo (medido) |
|---|---|---|
| **A — Registro de resolvedores y esquema del framework** | **completada** ✔ | `ProtectedValueResolvers.cs`: el punto de registro por esquema con **fijación única al arranque** (instancia + fachada del proceso; registro tardío o repetido ⇒ error tipado, comportamiento **documentado y probado**), `plain:` (identidad, `PlainTextValueResolver.cs`) y `aesgcm:` del framework; los esquemas del host por el mismo punto. `ProtectedValueFormat.cs`: la composición publicada (parseo, composición, tamaños, alfabeto del `kid`, decodificación de la clave de 32 bytes en Base64). |
| **B — El mecanismo: proteger y resolver con integridad** | **completada** ✔ | `AesGcmValueResolver.cs`: **proteger** exige la referencia de clave (clave activa del registro) y genera **nonce nuevo por operación** (`RandomNumberGenerator`); **resolver** verifica el tag **antes** de devolver el valor y ata el valor a su slot por **AAD**; la clave se limpia de los búferes temporales (`CryptographicMaterial.cs`). Sin primitivas prohibidas y sin comparación de tags propia. |
| **C — Provisión de claves y rotación por `kid`** | **completada** ✔ | `EnvironmentValueKeyProvider.cs` (variable `Cooperator__ProtectedValues__Keys__<kid>`, Base64 de 32 bytes; ausente = sin clave; mal formada = error tipado **sin** el material), `IProtectedValueKeyProvider.cs` (el hook del host) y `ProtectedValueKeyRegistry.cs` (clave **activa** + claves de **lectura**, ventana de solapamiento **con fecha**, `kid` sin clave ⇒ error tipado). |
| **D — Validación de esquemas y errores tipados** | **completada** ✔ | La familia completa: `ProtectedValueException` (base con el contexto permitido), `ProtectedValueIntegrityException`, `ProtectedValueKeyNotFoundException`, `ProtectedValueSchemeException` y `ProtectedValueKeyProviderException`; los ocho casos del cuadro de §D.1, cada uno con su tipo y su mensaje sin contenido. |
| **E — Retiro de la superficie estática y evidencia** | **completada** ✔ | `2.0/src/Cooperator.Framework.Library/Cryptography.cs` **retirado** (4 sobrecargas públicas, la clave y el IV embebidos, 3DES/MD5): la ruptura queda registrada con su reemplazo y su **paso siguiente obligatorio** en `2.0/docs/api-removals/US-000.BOLT-022.json`. §E.3 **no se ejerció**: ver §8.1. |

---

## 3. Files created

**Producto — `2.0/src/Cooperator.Framework.Library/Cryptography/ProtectedValues/` (14 archivos, 1 313 líneas):**

| Path | Propósito |
|---|---|
| `ProtectedValueResolvers.cs` | El punto de registro/resolución por esquema (instancia + fachada del proceso con fijación única). |
| `AesGcmValueResolver.cs` | El mecanismo: proteger (clave activa, nonce por operación) y resolver (tag y AAD antes del uso). |
| `ProtectedValueFormat.cs` | El **formato publicado**: `aesgcm:<kid>:<base64>`, `nonce 12 | tag 16 | ciphertext`, clave de 32 bytes en Base64, alfabeto del `kid`. |
| `ProtectedValuePayload.cs` | Las partes de una carga ya parseada. |
| `IProtectedValueKeyProvider.cs` | El contrato del proveedor de claves (el hook del host). |
| `EnvironmentValueKeyProvider.cs` | El proveedor por defecto (variables de entorno, Base64 de 32 bytes). |
| `ProtectedValueKeyRegistry.cs` | Clave activa + claves de lectura + ventana de solapamiento fechada. |
| `PlainTextValueResolver.cs` | El resolvedor de identidad (`plain:`). |
| `CryptographicMaterial.cs` | Limpieza de los búferes temporales del material. |
| `ProtectedValueException.cs` + los 4 tipados | La familia de errores de la política, con el contexto permitido (clave de configuración y `kid`). |

**Pruebas — `2.0/tests/Cooperator.Harness.Tests/` (5 archivos, 771 líneas, 20 casos):**
`ProtectedValueTestSupport.cs` (claves generadas **en memoria**, proveedor del host, manipulaciones) ·
`ProtectedValueIntegrityTests.cs` (6) · `KeyProvisionTests.cs` (7) · `SchemeValidationTests.cs` (4) ·
`HostSchemeTests.cs` (3).

**Registro:** `2.0/docs/api-removals/US-000.BOLT-022.json` (3 entradas + 1 obligación registrada, según el formato
publicado por el Bolt 028).

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md` | `status: specified` → `in-development` → `ready-for-acceptance` + filas **T04** y **T05** | La toma por G11 y el cierre del V-Bounce. |
| `2.0/docs/api-removals-decisions.json` | La declaración de ADR-007 §3.1: `pending` → **`covered_by: [US-000.BOLT-022]`**; y la de §3.3: **re-ruteada** a `US-000.BOLT-023` | El registro es de **contribución distribuida** (SPEC-0047 §6/B): al entregar este Bolt, la obligación que apuntaba a él **se cumple** y la que no era suya se **corrige al dueño real** (§8.4). |
| `2.0/docs/MIGRATION-NOTES.md` | Regenerado por la consolidación determinista | El documento consolidado es **generado** (SPEC-0047 §6/E): 610 líneas, 7 fragmentos, 66 entradas. |

---

## 5. Files deleted

| Path | Por qué |
|---|---|
| `2.0/src/Cooperator.Framework.Library/Cryptography.cs` | §E.1: la superficie estática con clave implícita **y el camino por el que una clave vive dentro del producto**. El archivo existía **sólo** como el mecanismo legacy (clave y IV embebidos, 3DES con clave derivada por MD5, encoding ASCII, tipos `*CryptoServiceProvider` concretos): sin las cuatro sobrecargas no quedaba nada que no fuera material de clave y primitivas **prohibidas** en un paquete desplegado por ADR-007 §3.4. Su capacidad de **descifrado** legacy vive aislada en la herramienta del Bolt 023, con bandera explícita y registro de retiro (§8.2). |

---

## 6. Files renamed

Ninguno.

---

## 7. Implementation decisions

1. **El punto de despacho es una instancia con una fachada del proceso.** `ProtectedValueResolverRegistry` (instancia)
   concentra el registro, la fijación, el despacho y el error de «esquema sin resolvedor»; `ProtectedValueResolvers`
   (estática) es el punto del **proceso** con la **fijación única al arranque** (ADR-004 §3.2, ADR-006 §3.1): un
   segundo `Initialize` o un registro posterior son **error tipado**, y el comportamiento queda **probado**, no
   indeterminado (SPEC §14).
2. **La frontera con el Bolt 011 se respeta sin tocar `Core`.** La **forma** del prefijo sigue siendo del contrato
   (`ConnectionStringValue.Parse`); mi punto de registro resuelve los esquemas **registrados** y produce el error
   **propio** de «esquema sin resolvedor». El valor **sin** prefijo produce el error del **contrato**
   (`CooperatorConfigurationException`, construido, no reimplementado): un valor sin prefijo **nunca** se asume
   texto plano.
3. **El `AAD` es la clave de configuración que el consumidor declara** (ADR-007 §3.1): el resolutor **no** adivina de
   dónde salió el valor. El seam del Bolt 011 (`Resolve(key, payload)`) ya recibe esa clave: no hubo que enmendarlo
   (§8.5).
4. **El registro de claves falla al FIJAR, no al proteger**: `SetActive` verifica que la clave activa tenga material
   (error tipado inmediato, con la activa anterior intacta) y las claves de lectura se resuelven por el proveedor en
   cada operación (rotación sin big-bang: la nueva es la activa y la anterior sigue leyendo).
5. **La familia de excepciones deriva de `Library.Exceptions.BaseException`**, no de
   `CooperatorConfigurationException`: el contrato del Bolt 011 la declara **`sealed`** y la política criptográfica
   tiene su propia familia. Cada excepción implementa los constructores canónicos (los que el framework ya usa en
   sus excepciones portadas) **más** los de contexto.
6. **El material se limpia de los búferes temporales** (`Array.Clear`) y **el framework no guarda claves**: viven en
   el proveedor. La documentación declara que esto **no** es una garantía absoluta de borrado (el runtime puede
   haber movido una copia), sino la reducción de la ventana de exposición.
7. **La verificación de material se hace en la suite, no en una herramienta nueva**: el «gate de constantes de alta
   entropía» de ADR-007 §5 **no existe** en el árbol (§8.3), así que el control se entrega en el arnés —donde el
   SPEC pide la evidencia— con **detector probado** (se demuestra con un valor generado en memoria que sí lo
   detecta) y con el alcance declarado.

---

## 8. Deviations and assumptions

### 8.1 La política de nulabilidad de §E.3 **no se ejerció** (y eso es el resultado correcto)

§E.3 autoriza `#nullable disable` en los archivos **aportados del 1.4** que produzcan `CS8618`/`CS8603`/`CS8625`.
Este Bolt **no aportó** código del 1.4 al árbol: su intervención sobre el archivo legacy fue un **retiro**
(`Cryptography.cs`, §5) y todo el código nuevo —el resolutor, el formato, la provisión, la familia de excepciones y
las pruebas— está escrito para el mundo anotado y conserva la **nulabilidad activa**. Por lo tanto:

```
Directiva #nullable disable en 2.0/src: 0 archivos (el retiro no deja archivo que la necesite)
<Nullable>disable</Nullable> en .csproj: 0
Build con advertencias como error: 16 proyectos, 0 diagnósticos  ← la prueba de AC-11
```

El criterio del SPEC es **objetivo** (*archivo aportado + diagnóstico*): la lista quedó **vacía** porque no hay
archivo aportado. Se deja constancia explícita para que no se lea como autorización no usada por descuido.

### 8.2 El retiro del tipo completo (y no sólo de las dos sobrecargas)

§E.1 nombra las sobrecargas **sin** clave y «el camino por el que una clave vive dentro del producto». La lectura
ejecutada es que ese camino **es** la clase entera: la clave y el IV son campos privados que sólo usaban las
sobrecargas retiradas y las **con clave**, y estas últimas son 3DES + MD5 + IV fijo + `*CryptoServiceProvider`
concretos —**prohibidos** en cualquier paquete desplegado por ADR-007 §3.4, que declara que el estándar es el que
**todo** el árbol 2.0 debe cumplir, con la excepción de descifrado legacy acotada a un componente **interno de la
herramienta** (nunca en `Library`)—. Conservarlas habría dejado el material de clave y las primitivas prohibidas en
el producto, contra AC-5 y el NFR *No embedded secrets*. El fragmento registra las **tres** caras de la ruptura
(las dos sobrecargas implícitas y el tipo) con su reemplazo y el **paso siguiente obligatorio** del consumidor.

### 8.3 El «gate de constantes de alta entropía» de ADR-007 §5 no estaba en el árbol

El SPEC lo atribuye al Bolt 001 (`SPEC-261016-0000`); la búsqueda en el arnés da **0** coincidencias. Como AC-5 y
el NFR lo exigen **en este Bolt**, el control se entregó acá, en el arnés: búsqueda de la clave del formato legacy
**ensamblada** (para que el propio archivo de la prueba no sea una ocurrencia) y detector de constantes de alta
entropía con base **binaria** (un literal con forma de clave decodifica a ≥ 32 bytes con entropía ≥ 4,5 y con bytes
no imprimibles; un identificador escrito por una persona decodifica a ASCII y **no** se señala). El detector se
prueba contra un valor generado **en memoria**: un control que no tenga dientes no puede declarar ausencia.
**Deuda declarada**: si el gate se implementa en CI (Bolt del arnés), este control debería consumirlo en vez de
convivir con él.

### 8.4 El registro se extendió, y una ruta se corrigió

El mapa de decisiones es del **registro** (artefacto del Bolt 028, `ready-for-acceptance`) y su naturaleza es de
**contribución distribuida** (SPEC-0047 §6/B y §13): al entregar este Bolt, la declaración de **ADR-007 §3.1** pasó
de `pending → US-000.BOLT-022` a **`covered_by: [US-000.BOLT-022]`** (si no, el control del registro falla: un
`pending` cuyo dueño ya aportó su fragmento **no** puede quedar así). Además se **corrigió** la ruta de **ADR-007
§3.3** (la migración de los valores ya cifrados), que en el Bolt 028 se había ruteado a **este** Bolt: el SPEC-0035
§4 la declara explícitamente del **Bolt 023**, así que la obligación se re-ruteó a su dueño real y este Bolt la
registra como **paso siguiente obligatorio** en su fragmento.

### 8.5 El seam del Bolt 011 alcanzó, sin enmendarlo

El contrato `IProtectedValueResolver` ofrece `Scheme` y `Resolve(key, payload)`: la **carga** trae el `kid`
(`<kid>:<base64>`) y la **clave** es el AAD, así que no hizo falta un parámetro adicional. La operación de
**proteger** no está en el seam (es del mecanismo): se expone en `AesGcmValueResolver.Protect(clave, valor)` y
**exige** la referencia de clave —no existe sobrecarga con clave implícita, que es exactamente el retiro de §E.1—
(AC-1 verificado también **por reflexión** sobre la superficie pública del ensamblado).

### 8.6 G15 — verificación de fuentes gobernadas (sin cambio material)

El SPEC está `approved` con **`revision: 2`** y `review.decision: approved` (`2026-10-16T03:15:52-03:00`); el Bolt
entró `specified` con T01–T03 y se tomó con **T04**; los ADRs aplicados siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.4). No se activa ninguna revisión nueva.

### 8.7 Presupuesto de turnos (declarado)

El V-Bounce usó **12 loops** (la guía era 10): dos se consumieron en corregir **mis propias pruebas** (el helper de
manipulación tomaba el valor completo en vez de la carga; el detector de entropía marcaba identificadores; y
`SetActive` falla **al fijar** por diseño, que es lo que el SPEC pide) y uno en un error de JSON en el fragmento del
registro que el propio validador del arnés cazó. El producto nunca quedó a medio camino y cada corrección se
**verificó** antes de seguir; la suite quedó verde antes del cierre.

---

## 9. Verification evidence

### 9.1 La suite (218 casos)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 135, Skipped: 0, Total: 135
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

Los **20** casos nuevos: 6 de integridad (con la corrida de **1 000** nonces distintos y los casos de manipulación),
7 de provisión (entorno, roles activa/lectura con ventana fechada, hook que falla, material), 4 de esquemas
(identidad, sin resolvedor, prefijo del contrato, registro único) y 3 de host (esquema del host, fallo del
resolvedor del host, fijación única del proceso).

### 9.2 El cuadro de casos negativos, ejercido

```
ciphertext alterado · tag alterado · tag truncado · Base64 inválido · carga bajo el mínimo · AAD de otra clave
de configuración · kid ausente · kid desconocido · kid sin clave · clave de entorno no Base64 · clave de entorno
≠ 32 bytes · esquema sin resolvedor · hook del host que falla · valor sin prefijo · prefijo del contrato
   → cada uno con su TIPO de excepción, y el mensaje: nombra la clave de configuración y el kid,
     NO contiene el valor, la clave ni la carga (AC-10).
nonce: 1 000 protecciones ⇒ 1 000 nonces distintos (y nunca el bloque de ceros)
```

### 9.3 Material de clave (NFR *No embedded secrets*)

```
clave del formato legacy del 1.4 en el árbol 2.0 (cualquier forma): 0 apariciones   (el 1.4 la conserva: 1 archivo, intacto)
constantes de alta entropía con forma de clave en 2.0/src/**/*.cs:  0
claves en configuración versionada / fixtures / .coop2:             ninguna (las de la suite se generan en memoria)
superficie estática con clave implícita en el ensamblado:           0 (el tipo ya no existe; verificado por reflexión)
```

### 9.4 El gate de API y `1.4/**`

```
api-compat.ps1 -Mode Current → 193 diferencias (178 + las 15 del código nuevo de este Bolt)
  la AUSENCIA del tipo retirado se reporta nombrando la línea base 1.4 (Library.dll) y la cubre el fragmento del Bolt
git status --porcelain -- 1.4 → (vacío): el árbol congelado no se tocó
```

### 9.5 Gates (§9)

| Gate | Resultado | Evidencia |
|------|-----------|-----------|
| Build / lint / type checks | **pass** | 16 proyectos sin advertencias; `Library` compila sobre `netstandard2.1` con `AesGcm` in-box; **0** diagnósticos de nulabilidad y **0** archivos con la directiva de §E.3 |
| Unit / component tests | **pass** | 218/218 herméticos, incluidos el cuadro de casos negativos, la corrida de nonces y la provisión por entorno/hook |
| Consumer-side contract tests (test doubles) | **pass** | Un proveedor de claves del host, un resolvedor de esquema del host y el punto de fijación única ejercitados como contrato |
| SAST | **pass** | 0 claves literales, 0 primitivas prohibidas (MD5/SHA-1/DES/3DES/RC2/ECB/`*CryptoServiceProvider` concretos), 0 degradación a texto plano en el camino de resolución |
| Dependency scanning / licenses | **pass** | **0** dependencias nuevas: el mecanismo es de la plataforma (`System.Security.Cryptography`) |
| OWASP Top 10 — static review | **pass** | AEAD (autenticado), nonce aleatorio por operación, tag verificado **antes** de usar, comparación de tags por la plataforma, AAD por clave de configuración, sin modos/primitivas prohibidas y sin oráculo de contenidos en los errores |
| Prompt-injection scan | **n/a (razón)** | Se procesan valores de configuración con un formato declarado, no contenido interpretado como instrucción |
| Secret-leak scan | **pass** | §9.3: 0 material en el código, el repositorio, la configuración versionada, los fixtures y los mensajes; la clave del formato legacy no reaparece en ninguna forma |
| Hallucination lint | **pass** | Cada pieza trazable a ADR-007 §3.1/§3.2/§3.4 y al seam del Bolt 011; 0 capacidades ajenas (migración, rotación operativa, encriptor) adelantadas |
| IP / license provenance | **pass** | Código propio sobre la API de la plataforma; **no** se copió la implementación del formato legacy (que se retiró) |
| PII / DLP | **pass** | `confidential`: la suite usa claves **generadas en memoria** y ningún valor real entra al repositorio ni a los reportes |
| Dependency-confusion | **n/a (razón)** | No se agregan paquetes; la resolución de esquemas es explícita (registro), sin búsqueda por nombre |
| Test-first evidence | **pass** | El cuadro de casos negativos se escribió desde ADR-007 §5 antes de implementar, y la verificación de material antes de retirar la superficie |
| Behavioral reproducibility (hermetic suite) | **pass** | Dos corridas dan el mismo veredicto; el payload es **deliberadamente no determinista** y eso se verifica **como propiedad** (AC-6) |
| Bolt-manifest validation | **pass** | El manifiesto valida contra `manifest-v5-bolt.schema.json` tras registrar el V-Bounce (§9.6) |
| Integration tests (real dependencies) | **n/a (razón)** | Plan `n/a` (Bolt `not-required`): el control se verifica **en proceso**, sin superficie desplegada ni servicio externo |

### 9.6 Manifest

`devflow/metrics/bolts/US-000.BOLT-022-resolutor-aesgcm.json`: entrada **`v_bounces[0]`** (`number: 1`,
**`spec_revision: 2`**, `execution_outcome: ready_for_review`), validada contra `manifest-v5-bolt.schema.json`.

---

## 10. Manual interventions

**Ninguna intervención humana.** El mecanismo, el retiro, las pruebas, el fragmento del registro y las dos
consolidaciones se hicieron en este V-Bounce **sin operaciones de git** (G34).

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`.
- **SPEC ejecutado:** [SPEC-261016-0035](../spec/SPEC-261016-0035-resolutor-aesgcm.md) — **revisión 2**
  (`AITL-SPEC-Approval` del `2026-10-16T03:15:52-03:00`): §6/A–E, §7 (AC-1…AC-11) y §E.3.
- **Bolt:** [US-000.BOLT-022](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — filas **T04** y **T05**.
- **Entregables:** `2.0/src/Cooperator.Framework.Library/Cryptography/ProtectedValues/**`,
  `2.0/tests/Cooperator.Harness.Tests/{ProtectedValueIntegrityTests,KeyProvisionTests,SchemeValidationTests,HostSchemeTests}.cs`,
  `2.0/docs/api-removals/US-000.BOLT-022.json` y `2.0/docs/MIGRATION-NOTES.md`.
- **Manifiesto:** [US-000.BOLT-022-resolutor-aesgcm.json](../metrics/bolts/US-000.BOLT-022-resolutor-aesgcm.json).
- **Prerrequisito consumido:** `Core/Configuration/IProtectedValueResolver.cs` (Bolt 011) — **sin cambios**.

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-022) — `ready_for_review` |
| SPEC ejecutado | SPEC-261016-0035 revisión **2** |
| Tiempo de generación IA | ~1 h de reloj (02:30 → 03:25) |
| Fases completadas | **5 de 5** (A–E) |
| Archivos creados | **14** de producto (1 313 líneas) + **5** de pruebas (771 líneas, 20 casos) + **1** fragmento del registro |
| Archivos modificados | **3** (el Bolt; el mapa de decisiones del registro; el documento consolidado **regenerado**) |
| Archivos retirados | **1** (`Library/Cryptography.cs`: 4 sobrecargas públicas y el material de clave) |
| Directivas `#nullable disable` | **0** (§8.1: el aporte fue un retiro, no un porteo) |
| Casos de prueba nuevos | 20 (todos verdes) |
| Tests de la suite | **218** herméticos (135 + 83), todos verdes |
| Aprobación en primera revisión | n/a (pendiente del checkpoint de este V-Bounce) |

---

## 13. Pending items and stubs

1. **Obligación registrada** (en el fragmento del Bolt y en §8.2 del SPEC del área): el catálogo de valores de
   configuración (`ConnectionStringValue`, Bolt 011) admite hoy **sólo** `plain:` en su validación de la **forma**,
   así que un valor `aesgcm:` no llega al resolutor por esa vía. Es del **dueño del contrato** (Bolt 011 o su
   sucesor); este Bolt dejó el resolutor entregado y registrado, y **no** tocó `Core` porque el SPEC se lo prohíbe
   (§5: «`Core` no se toca»).
2. **La ruta corregida** del registro (§8.4): la migración de los valores ya cifrados quedó ruteada al
   `US-000.BOLT-023`, que es su dueño por SPEC.
3. **La deuda del gate de entropía** (§8.3): si el Bolt del arnés implementa el gate de constantes de alta entropía
   de ADR-007 §5, el control de este Bolt debería consumirlo en vez de convivir con él.
4. **Los esquemas del host** (`dpapi:`, `x509:`, `kv:`) siguen siendo del host por el hook: el framework **no** los
   implementa (y no lo hará por esta vía).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto (`number: 1`, `spec_revision: 2`) + el árbol
  entregado (el resolutor `aesgcm:` con su formato publicado, la provisión de claves, la familia de errores, el
  retiro de la superficie estática, el fragmento del registro y el documento consolidado) + la corrida verde de
  **218 casos**.
- **Qué se pide decidir:** la aceptación del entregable y el cierre del Bolt hacia su aceptación.
