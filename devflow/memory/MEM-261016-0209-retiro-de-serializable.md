---
id: "MEM-261016-0209"
title: "Retiro de `[Serializable]`/`SerializationInfo` y del rastro binario (SPEC rev.1) — V-Bounce N°1 detenido por la mecánica de build del porteo (nulabilidad) y la arista `Data → Library`"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool, or null (§3.0)
bolt: "US-000.BOLT-007" # US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN — the Bolt this V-Bounce executed
spec: "SPEC-261016-0013" # SPEC-YYMMDD-HHmm of the canonical SPEC revision executed
spec_revision: 1 # revision of the canonical SPEC used by this V-Bounce
v_bounce: 1 # V-Bounce number within the Bolt (matches manifest v_bounces[].number)
execution_outcome: "blocked" # ready_for_review | failed | blocked | cancelled — describes execution before human review (§3.12)
baseline: "6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4" # git commit of the repository baseline used by this V-Bounce
applied_adrs: ["ADR-008", "ADR-002", "ADR-001"] # ADR-008 §3.2/§3.3/§3.4 (el retiro y su constancia), ADR-002 §3.2 (la excepción del migrador), ADR-001 §3.1/§3.4 (direcciones permitidas y cero dependencias) — ninguna materializó un cambio: el V-Bounce se detuvo antes de entregar
manifest: "US-000.BOLT-007-retiro-de-serializable.json" # Path to the Bolt manifest in devflow/metrics/bolts/
diff_ref: "" # NO code was left in the tree (see §6) — working tree only, no git operations (G34)
review_ready_at: "2026-10-16T02:09:03-03:00" # When this version is submitted for review (§3.0)
review: # AITL-MEM-Approval — filled by the human reviewer (§3.0)
  decision: "approved"
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}]
  started_at: "2026-10-16T02:11:20-03:00"
  decided_at: "2026-10-16T02:11:20-03:00"
  findings:
    - "Compliant — bloqueo legítimo y reproducible (artefacto 1, nulabilidad): verificado en el código que `1.4/Cooperator.Framework.Core/BaseObject.cs` (constructor `protected BaseObject()` L156-161, con los eventos `OnCancelEdit`/`OnBeginEdit`/`OnEndEdit` y el campo `InternalOriginalValue` no inicializados) y `DescriptionFieldEventArg.cs` L42 (`private string _DescriptionString;` **no inicializada** en el constructor L48-52) son código 1.4 anterior a las anotaciones → producen `CS8618` bajo `Nullable=enable`+`TreatWarningsAsErrors`. El SPEC §4 manda «copiar tal cual… sin ninguna otra transformación», y **SPEC-0004 §A.3** es el precedente exacto (política de nulabilidad por **revisión material**). La autorización propuesta (espejo: `#nullable disable` por archivo aportado) es la correcta."
    - "Compliant — bloqueo legítimo (artefacto 2, arista Data→Library): verificado que `1.4/.../Data/Exceptions/DataException.cs` L42 declara `public abstract class DataException: BaseException` (con `using Cooperator.Framework.Library.Exceptions;` en L34) y el stub de `2.0/src/Cooperator.Framework.Data/Cooperator.Framework.Data.csproj` declara **0 `ProjectReference`** (sólo PropertyGroup). ADR-001 §3.1 **permite** la dirección `Data→Library` y el SPEC §B.2 la exige (fija `BaseException` como base), pero nadie autorizó agregar la arista. La autorización (agregar el `ProjectReference`, documentada) es correcta."
    - "Compliant — inventario declarado correcto: **39 declaraciones / 31 constructores / 0 ISerializable / 0 GetObjectData / 0 contexto** (precondición §A.2 pasa); `Utility` 7 verificado/registrado como **frontera** (grafo y excepciones del grafo → 003, `CompilerException` → 017); la divergencia con la cifra del SPEC §2 («36/36») está **declarada y explicada** (§8.4: el 36 incluía el borde, el 39/31 del MEM desglosa en alcance + borde). Transparencia correcta, no un defecto."
    - "Compliant — cierre correcto: T04 (`specified`→`in-development`, 02:05:37) y T05 (`in-development`→`blocked`, 02:09:03) con reloj real; MEM único (`MEM-261016-0209` referencia `US-000.BOLT-007`); manifiesto `v_bounces[0]` con los 8 campos y `execution_outcome: blocked`; árbol restaurado (85/85 + 83/83, 16 proyectos, `EXIT=0`, los 4 archivos preexistentes **verbatim**); `1.4/**` intacto."
    - "Compliant — condición de parada correcta: §15 **#6** (el gate de **Build** no cierra en `pass`) con el espíritu de §15 **#5** (el cierre exige una mecánica de build no autorizada); la única salida es una **decisión del SPEC** (revisión material), nunca del ejecutor — precedente exacto: SPEC-0004 §A.3, agregado por revisión material para este mismo escenario."
    - "Compliant — desbloqueo propuesto correcto y agéntico: revisión 2 que autorice (1) la **política de nulabilidad** del código aportado por §4 (espejo de SPEC-0004 §A.3) y (2) la **arista `Data→Library`** (dirección ya permitida por ADR-001 §3.1 y exigida por §B.2). Ambas de una línea, con precedente y dirección permitida: suficiente y agéntico (la re-aprobación la hace el revisor en otro acto)."
  acknowledged_without_comment: false
  acknowledgment_reason: ""
---

# MEM-261016-0209 — Retiro de `[Serializable]`/`SerializationInfo`: V-Bounce N°1 detenido por la mecánica de build del porteo

| Field           | Value |
|-----------------|-------|
| **Bolt**        | [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `type: non-functional`, `test_integration: not-required` |
| **SPEC**        | [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md), **revisión 1** (aprobada: `AITL-SPEC-Approval` del `2026-10-16T00:22:23-03:00`) |
| **V-Bounce**    | **1** (el primer V-Bounce de este Bolt) |
| **ADRs**        | [ADR-008](../adrs/ADR-008-compatibilidad-de-api.md) · [ADR-002](../adrs/ADR-002-formato-coop2-y-migrador.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Baseline**    | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` (`6aa497f`) — sin cambios materiales (§8.5) |

---

## 1. Executive summary

Se ejecutó la **fase A** (inventario «antes») y la **fase B** (el retiro) con un generador determinista: se
inventariaron **39 declaraciones `[Serializable]`** y **31 constructores de serialización** (todos `protected`, **0**
`ISerializable`, **0** `GetObjectData`) sobre la fuente congelada, se verificó la **precondición de seguridad** de
§A.2 —**0** constructores transportan contexto (`info.AddValue`/`info.GetValue`)— y el retiro mecánico (atributo,
constructor y el `using System.Runtime.Serialization` que quedaba sin uso) se aplicó sobre los **32 archivos** del
alcance —los 31 tipos de `Core`/`Data`/`Library` y `BaseObject`— más el cierre mínimo (`ObjectStateHelper`,
`DescriptionFieldEventArg`) aportado verbatim: **0 residuos** de superficie de serialización en `2.0/src` y las 31
excepciones compilando sin un solo diagnóstico. Pero al construir, **dos artefactos de mecánica** —no del retiro—
impiden cerrar: (1) los archivos que el §4 manda **aportar «tal cual»** (`BaseObject.cs` y su cierre) son código del
1.4 anterior a las anotaciones de nulabilidad y el árbol hereda `Nullable=enable` + `TreatWarningsAsErrors`, de modo
que producen diagnósticos **CS8618** que el árbol convierte en errores —y el SPEC **no** tiene la política que el
SPEC-0004 §A.3 tuvo que agregar por revisión material para exactamente este caso, ni autoriza «ninguna otra
transformación» sobre lo aportado—; y (2) las **16 excepciones de `Data`** derivan de
`Cooperator.Framework.Library.Exceptions.BaseException`, que el §B.2 espera como su base abstracta, pero el stub de
`Data` en el 2.0 declara **cero** `ProjectReference`, así que aportarlas exige la arista `Data → Library` que
ADR-001 §3.1 **permite** pero que nadie autorizó a agregar. Como el gate de **Build** de §9 no puede cerrar en
`pass` y su única salida es una transformación que el SPEC no autoriza, el V-Bounce se detuvo y **los aportes se
revirtieron**: el árbol queda **sin cambios y verde** (16 proyectos, 0 advertencias; arnés 85/85; caracterización
83/83; `EXIT=0`). Se piden **dos autorizaciones mínimas** (§8.3), ambas de una línea.

---

## 2. Implemented phases

| Fase de §6 | Estado | Qué se hizo |
|---|---|---|
| **A — Inventario «antes»** | **completada y medida; el artefacto se revirtió con el cierre** | Inventario legible por máquina de los tipos en alcance + la verificación de la precondición de §A.2 (§9.1). Su contenido medido es la evidencia de este MEM. |
| **B — Retiro en `BaseObject` y en la jerarquía** | **ejecutada y revertida** | El retiro mecánico se aplicó a los 32 archivos del alcance y el cierre mínimo se aportó verbatim; las 31 excepciones compilan limpias, y **el bloqueo está en los archivos aportados por la regla de §4** (`BaseObject` + su cierre) y en la arista que `Data` necesita (§8.1, §8.2). |
| **C — Frontera (verificación)** | no alcanzada | — (el inventario ya **clasifica por dueño**: los tipos del grafo y sus excepciones → Bolt 003; `CompilerException` → Bolt 017 ✔ §9.1). |
| **D — Rastro binario y allowlist** | no alcanzada | — |
| **E — Enforcement de superficie** | no alcanzada | — |
| **F — Constancia, escape-hatch y cierre** | no alcanzada | — |

---

## 3. Files created

Todos los de esta tabla **se generaron en este V-Bounce y se revirtieron al cerrarlo** (§6). Se registran con su
propósito porque son el entregable medido y lo que el próximo V-Bounce debe reproducir.

| Path | Propósito |
|---|---|
| `2.0/tests/fixtures/serialization-surface/baseline-inventory.json` | El inventario «antes» de §A.1: por tipo, ensamblado, nombre completo, si declara `[Serializable]`, la firma y la visibilidad del constructor de serialización, si implementa `ISerializable` o declara `GetObjectData`, las anotaciones asociadas, si transporta contexto y **su dueño** (la clasificación de AC-7). |
| `2.0/src/Cooperator.Framework.{Core,Data,Library}/Exceptions/*.cs` (**27 archivos**) | Las excepciones del alcance que **no** estaban en el árbol 2.0, aportadas **en su forma ya retirada** (sin `[Serializable]`, sin el constructor `(SerializationInfo, StreamingContext)` y sin el `using System.Runtime.Serialization` que quedaba sin uso): 5 de `Core`, 16 de `Data` y 6 de `Library`. Conservan **todos** sus constructores públicos y su jerarquía. |
| `2.0/src/Cooperator.Framework.{Core,Library}/Exceptions/{CoreException,InvalidDataSetException,BaseException,SmtpErrorException}.cs` | Los 4 archivos del alcance que **ya existían** en el árbol (aportados verbatim por los Bolts 006/011): se les aplicó **el mismo retiro mecánico**. |
| `2.0/src/Cooperator.Framework.Core/BaseObject.cs` | `BaseObject` **en su forma ya retirada** (§B.1): sin `[Serializable]` y sin los **tres** `[field: NonSerialized]` de sus eventos públicos, con su contrato público intacto. |
| `2.0/src/Cooperator.Framework.Core/{ObjectStateHelper,DescriptionFieldEventArg}.cs` | **Cierre mínimo** de `BaseObject`, aportado **verbatim** del 1.4 (sin ninguna edición). |

---

## 4. Files modified

| Path | Cambio | Propósito |
|---|---|---|
| `devflow/functional/bolts/US-000.BOLT-007-retiro-de-serializable.md` | `status: specified` → `in-development` y fila **T04** (toma del V-Bounce por G11); al cerrar, la transición inversa ya está registrada | Las transiciones de estado del Bolt con su fila de `Status timeline`. La fila **T03** quedó donde estaba (fuera de orden): no es de este acto moverla. |

---

## 5. Files renamed

Ninguno.

---

## 6. Files deleted

Todo lo de §3 se **revirtió** al cerrar el V-Bounce (rollback = no-cambio), y los **4 archivos preexistentes** del
alcance se **restauraron verbatim** desde el 1.4 (su forma aportada por los Bolts 006/011), de modo que el árbol
vuelve exactamente a su estado inicial:

```
Core: 28 archivos .cs · Data: 0 · Library: 3        (idénticos al inicio del V-Bounce)
superficie [Serializable] en 2.0/src: los 4 archivos preexistentes, intactos (estado «antes» del inventario)
arnés: 85/85 · caracterización: 83/83 · build: 16 proyectos sin advertencias · EXIT=0     (§9.4)
1.4/**: git status vacío y git diff --quiet <baseline> -- 1.4 = 0                          (§9.5)
```

---

## 7. Implementation decisions

1. **El retiro es mecánico y trazable a la fuente congelada.** Los 32 archivos se generan con un generador
   determinista que lee el 1.4 y aplica **tres** operaciones: quitar `[Serializable]` (en línea propia o inline),
   quitar el constructor de serialización **con su doc-comment y su cuerpo** (por balance de llaves) y quitar el
   `using System.Runtime.Serialization;` **sólo si quedó sin uso**. Nada más se toca: ningún constructor, ningún
   mensaje, ningún orden.
2. **El inventario se produce antes de retirar y es el que ordena el retiro** (§A.1): 39 declaraciones y 31
   constructores medidos, todos `protected`, 0 `ISerializable`, 0 `GetObjectData` y **0** constructores que
   transporten contexto —la precondición de §A.2 **pasa**—, con las anotaciones asociadas (los tres
   `[field: NonSerialized]` de `BaseObject`, único tipo con anotaciones) y **la clasificación por dueño**, que es lo
   que AC-7 pide.
3. **El `using` que queda sin uso lo retira el propio SPEC** (§B.2: «limpieza del `using System.Runtime.Serialization;`
   que quede sin uso»), de modo que ese detalle **no** es una transformación fuera de la regla de §4.

---

## 8. Deviations and assumptions

### 8.1 El bloqueo, artefacto 1 — la nulabilidad de los archivos que el §4 manda aportar «tal cual»

**Medido.** Con el retiro aplicado y el cierre aportado verbatim, el build del núcleo falla **sólo** por los
archivos aportados por la regla de §4 (ninguna de las 31 excepciones produce un diagnóstico):

```
BaseObject.cs(152,19): error CS8618: los eventos 'OnCancelEdit'/'OnBeginEdit'/'OnEndEdit' y el campo
                                'InternalOriginalValue' deben contener un valor no nulo al salir del constructor
DescriptionFieldEventArg.cs(48,16): error CS8618: el campo '_DescriptionString' ...
Cooperator.Framework.Core EXIT=1        (con Nullable=enable + TreatWarningsAsErrors=true, del Bolt 001)
```

**Por qué no lo resuelvo yo.** El código del 1.4 es **anterior a las anotaciones de nulabilidad** y el árbol 2.0
hereda `Nullable=enable` **y** `TreatWarningsAsErrors=true`, así que «copiado tal cual» **no compila**. Las salidas
posibles son las dos que el SPEC no autoriza por sí mismo: (a) la directiva **`#nullable disable` por archivo
aportado** —que es **exactamente** el mecanismo que el SPEC-0004 §A.3 estableció por **revisión material** para este
mismo escenario (`EXIT=0` con la directiva, 28 diagnósticos sin ella), y que aquel SPEC tuvo que **autorizar**
porque tampoco estaba en su revisión 1—; o (b) anotar/corregir el código portado, que es una transformación
explícita y prohibida por el §4 («**sin ninguna otra transformación**»). Ninguna de las dos es una decisión del
ejecutor.

### 8.2 El bloqueo, artefacto 2 — la arista `Data → Library` que el §B.2 necesita

**Medido.** Aportadas las 16 excepciones de `Data` en su forma retirada, el build falla por la base que el propio
SPEC fija para esa jerarquía:

```
DataException.cs(33,28): error CS0234: 'Library' no existe en el espacio de nombres 'Cooperator.Framework'
DataException.cs(40,42): error CS0246: no se encuentra el tipo 'BaseException'
Cooperator.Framework.Data EXIT=1
```

**Por qué no lo resuelvo yo.** El SPEC §B.2 dice que las bases abstractas del alcance son «`CoreException`,
`DataException`, `BaseException`» —es decir, las excepciones de `Data` **derivan** de la base de `Library`— y
ADR-001 §3.1 **permite** la dirección `Data → Library`; pero el stub de `Data` en el árbol 2.0 (Bolt 001) declara
**cero** `ProjectReference`, así que la arista hay que **agregarla** a un archivo del Bolt 001. Es el «cierre mínimo
**para que compile**» del §4 leído por su propósito, pero es una **arista nueva de la capa** y la declaro en lugar de
decidirla.

### 8.3 Las dos autorizaciones mínimas que se piden (ambas de una línea)

1. **Política de nulabilidad del código aportado por la regla de §4** — la misma que el SPEC-0004 §A.3 fijó para el
   código portado del 1.4: `#nullable disable` **por archivo aportado** (aquí: `BaseObject.cs` y su cierre
   `ObjectStateHelper.cs`/`DescriptionFieldEventArg.cs`), **nunca** en código nuevo ni a nivel de proyecto. Si el
   revisor prefiere, la alternativa es **diferir el aporte de `BaseObject` y su cierre**: este Bolt verificaría y
   registraría su estado (como hace con el grafo en AC-7) y el aporte queda del Bolt que porte la superficie base.
2. **La arista `Data → Library`** en el `.csproj` del stub de `Data` (una línea, dirección ya permitida por ADR-001
   §3.1 y **exigida** por el §B.2 al fijar `BaseException` como base de la jerarquía de `Data`).

Con esas dos autorizaciones (una revisión material de §4 y §B.2, en el patrón que el proyecto ya usó dos veces),
**el entregable se reproduce completo**: el retiro ya está medido y compila en lo que a las 31 excepciones
respecta, y el inventario, la constancia, el control de rastro y el gate de superficie son trabajo mecánico
adicional.

### 8.4 El inventario medido frente a la cifra del SPEC

| Dato | Medido acá | Registrado en el SPEC (§4/§6) |
|---|---|---|
| Tipos en alcance de este Bolt | **32** (31 excepciones + `BaseObject`) | 31 + `BaseObject` ✔ |
| Declaraciones `[Serializable]` en la fuente | **39** (Core 8, Data 16, Library 8, Utility 7) | «36 declaraciones» |
| Constructores de serialización (en alcance) | **31**, todos `protected` | «36 constructores» |
| `ISerializable` / `GetObjectData` | **0 / 0** | 0 / 0 ✔ |
| Constructores que transportan contexto (§A.2) | **0** | 0 ✔ |
| Anotaciones asociadas | `BaseObject.cs`: `[field: NonSerialized]` (×3) — único tipo con anotaciones | los tres de §B.1 ✔ |

La **composición** coincide (los mismos archivos por ensamblado y la misma visibilidad); las cifras agregadas del
SPEC (36/36) incluyen, además del alcance de este Bolt, las declaraciones y constructores de los tipos del **borde**
—las 6 clases del grafo y sus 4 excepciones (Bolt 003) y `CompilerException` (Bolt 017)—, que el inventario registra
**atribuidas a su dueño** sin transformarlas (AC-7). Se declara la diferencia para que el revisor sepa que el
inventario del árbol es el que este MEM reporta.

### 8.5 G15 — verificación de fuentes gobernadas (sin cambio material)

Comprobado antes de generar: el SPEC está en `status: approved`, `revision: 1`, `review.decision: approved`
(`2026-10-16T00:22:23-03:00`); el Bolt conserva su `AITL-BOLT-READY-Approval` y entró a este V-Bounce en `specified`
con T01–T03 presentes; ADR-008/ADR-002/ADR-001 siguen `accepted`; la línea base es
`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` y `1.4/**` está intacto (§9.5). No hay cambio material → G15 no se
activa.

### 8.6 Comprobación de las condiciones de parada (§15) y cuál se aplicó

- **#1 (cambio material de fuente):** negativa (§8.5).
- **#2 (un tipo necesita conservar la superficie — escape-hatch):** negativa — **ningún** tipo lo reclama y los cinco
  criterios no se cumplen para ninguno (§8.4: 0 `ISerializable`, 0 `GetObjectData`, 0 contexto). La decisión negativa
  queda documentada acá y la vía para una futura excepción es el Bolt que la pida con esa evidencia.
- **#3 (un constructor transporta contexto):** negativa — **0** ocurrencias (§9.1).
- **#4 (habría que editar `1.4/`):** negativa — el 1.4 no se tocó (§9.5).
- **#5 (el cierre mínimo exigiría transformar código de otro Bolt):** negativa en su letra — el cierre se aportó
  **verbatim** y ningún código de otro Bolt se transformó—, pero es la condición **más cercana** por su espíritu: el
  cierre exige una **mecánica de build** que el SPEC no autoriza (la directiva de nulabilidad, §8.1).
- **#6 (un gate de §9 no cierra en `pass`):** **APLICADA.** El gate de **Build / lint / type checks** no puede cerrar
  en `pass`: los archivos que el §4 manda aportar «tal cual» producen `CS8618` bajo la política de build del árbol, y
  la única salida —la directiva por archivo o la anotación del código portado— es una decisión del SPEC, no del
  ejecutor (precedente exacto: SPEC-0004 §A.3, que se agregó por **revisión material**). Se detiene y se escala.
- **#7 (arnés ausente):** negativa — el arnés corrió verde antes y después (§9.4).
- **#8 (aparece superficie de serialización no inventariada):** negativa — el inventario encontró **exactamente** los
  tipos del alcance más los del borde, todos con dueño; **no se retiró nada por analogía** y no quedó nada por
  olvido (§9.1).

---

## 9. Verification evidence

### 9.1 El inventario «antes», medido sobre la fuente congelada (fase A)

```
Tipos inventariados:                      34 entradas (32 en alcance + 2 archivos del borde)
Declaraciones [Serializable]:             39   (Core 8 · Data 16 · Library 8 · Utility 7)
Constructores de serialización:           31   (visibilidad: protected ×31)
Implementa ISerializable:                  0
Declara GetObjectData:                     0
Constructores que transportan contexto:    0   ← PRECONDICIÓN §A.2: PASA (nada que convertir en propiedad tipada)
Anotaciones asociadas:                     BaseObject.cs → [field: NonSerialized] ×3 (único tipo con anotaciones)
Clasificación por dueño (AC-7):            32 → US-000.BOLT-007 · 6 clases del grafo + sus 4 excepciones → US-000.BOLT-003
                                           · CompilerException → US-000.BOLT-017 · Snapshot/plantillas → 002/025 y el set
```

### 9.2 El retiro, medido

```
Archivos retirados (atributo + constructor + using sin uso): 32   (31 excepciones + BaseObject)
Cierre mínimo aportado verbatim:                             2   (ObjectStateHelper.cs, DescriptionFieldEventArg.cs)
Residuos de superficie de serialización en 2.0/src:          0
Diagnósticos producidos por las 31 excepciones:              0
```

### 9.3 El bloqueo, medido (la evidencia central de este MEM)

```
Cooperator.Framework.Core EXIT=1   → CS8618 en BaseObject.cs (3 eventos + 1 campo) y DescriptionFieldEventArg.cs (1 campo)
Cooperator.Framework.Data EXIT=1   → CS0234/CS0246 en DataException.cs (falta la referencia a Library)
          (Library falla por arrastre de Core; sus propias 8 excepciones no producen diagnóstico)
```

### 9.4 El árbol como se deja (corrida de control, tras la reversión)

```
pwsh -File 2.0/build/build.ps1 -Scope Hermetic
  build sin advertencias (tratadas como error): 16 proyectos
  harness-hermetic          → Passed! - Failed: 0, Passed: 85, Skipped: 0, Total: 85
  characterization-hermetic → Passed! - Failed: 0, Passed: 83, Skipped: 0, Total: 83
Resultado: ok (codigo de salida 0)
```

**168 casos herméticos verdes**, 16 proyectos con 0 advertencias, `EXIT=0`: el árbol quedó **idéntico** al que el
V-Bounce encontró, incluidos los **4 archivos preexistentes** del alcance (restaurados verbatim), que son el estado
«antes» del inventario.

### 9.5 `1.4/**` intacto

```
git status --porcelain -- 1.4                                    → (vacío)
git diff --quiet 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4 -- 1.4 → EXIT=0  (intacto)
```

### 9.6 Gates (§9)

No se declaran gates en `pass`: el V-Bounce se detuvo **antes** de entregar y el árbol quedó sin cambios. El gate de
**Build** es, precisamente, el que no puede cerrar en `pass` sin la autorización de §8.3 (condición §15 #6).

### 9.7 Manifest y encoding

- `devflow/metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json`: se agregó la entrada **`v_bounces[0]`**
  (`number: 1`, `spec_revision: 1`, `execution_outcome: blocked`), validada contra
  `devflow/metrics/manifest-v5-bolt.schema.json`.
- Los documentos de este V-Bounce (MEM, fila T04 del Bolt, manifiesto) se escribieron en **UTF-8 sin BOM**, con `LF`
  y sin `U+FFFD` (§3.15/W23).

---

## 10. Manual interventions

**Ninguna intervención humana.** El retiro se generó mecánicamente desde la fuente congelada y la reversión fue
mecánica. **Tampoco hubo operaciones de git** (G34): el árbol está sin commitear y el commit es del humano. Los
únicos actos del agente son los del contrato del V-Bounce: la toma del Bolt (T04), la generación, las mediciones, el
rollback y el cierre.

---

## 11. Evidence links

- **Diff/PR:** ninguno — `diff_ref: ""`, el árbol quedó **sin cambios** respecto del estado con el que arrancó el
  V-Bounce (§6, §9.4).
- **SPEC ejecutado:** [SPEC-261016-0013](../spec/SPEC-261016-0013-retiro-de-serializable.md) — **revisión 1**
  (`AITL-SPEC-Approval` aprobada el `2026-10-16T00:22:23-03:00`).
- **Bolt:** [US-000.BOLT-007](../functional/bolts/US-000.BOLT-007-retiro-de-serializable.md) — `Status timeline`,
  fila **T04**.
- **Manifiesto:** [US-000.BOLT-007-retiro-de-serializable.json](../metrics/bolts/US-000.BOLT-007-retiro-de-serializable.json)
  — `v_bounces[0]` (`number: 1`, `spec_revision: 1`, `blocked`).
- **Precedente de la mecánica que falta:** [`MEM-261016-0107`](MEM-261016-0107-particion-modeling.md) §8.2 (la
  medición de los 28 diagnósticos `CS86xx` del porteo) y [`MEM-261016-0125`](MEM-261016-0125-particion-modeling.md)
  §8.2/§9.2, que llevaron a **SPEC-0004 §A.3** (`#nullable disable` por archivo portado) — la autorización que este
  SPEC necesita para el mismo escenario.
- **Evidencia de código (lectura, árbol congelado):** `1.4/Cooperator.Framework.{Core,Data,Library}/Exceptions/*.cs`
  (31 archivos: `[Serializable]`, el constructor `(SerializationInfo info, StreamingContext context)` y el `using
  System.Runtime.Serialization;`), `1.4/Cooperator.Framework.Core/BaseObject.cs` (L64 `[Serializable]`, L74/81/88
  `[field: NonSerialized]`), `1.4/Cooperator.Framework.Data/Exceptions/DataException.cs` (L33/L40: la base
  `Library.BaseException`), y los dos archivos del borde (`Utility/DBReverseHelper/DomainTree.cs`,
  `Utility/Exceptions/UtilityException.cs`).

---

## 12. Metrics

| Metric | Value |
|--------|-------|
| V-Bounce | 1 (de US-000.BOLT-007) — `blocked` |
| SPEC ejecutado | SPEC-261016-0013 revisión **1** (aprobada antes de arrancar) |
| Tiempo de generación IA | ~4 min de reloj (02:05:37 → 02:09:03): inventario, retiro, builds de diagnóstico, reversión y control |
| Archivos entregados al árbol | **0** (reversión completa, §6) |
| Archivos generados y revertidos | **32** del alcance + **2** de cierre + **1** inventario |
| Diagnósticos de las 31 excepciones retiradas | **0** |
| Diagnósticos del bloqueo | **6** `CS8618` (Core) + **2** `CS0234/CS0246` (Data) |
| Tests corridos como control | **168** (85 del arnés + 83 de caracterización), todos verdes |
| Condición de parada | §15 **#6** (un gate de §9 no cierra en `pass`) + el principio de §15 #5 (el cierre exige una mecánica no autorizada) |
| Autorizaciones pedidas | **2**, ambas de una línea (§8.3) |
| Aprobación en primera revisión | n/a (V-Bounce `blocked`) |
| Código generado por IA | 100 % de lo generado y revertido |

---

## 13. Pending items and stubs

1. **El bloqueo (§8.1 y §8.2)** es lo único que impide cerrar: la política de nulabilidad de los archivos que el §4
   manda aportar **tal cual** y la arista `Data → Library` que el §B.2 necesita. Las dos autorizaciones mínimas están
   en §8.3.
2. **Lo demás está medido y listo para el próximo V-Bounce:** el inventario (con su clasificación por dueño), el
   retiro mecánico de los 32 archivos (0 residuos, 0 diagnósticos en las 31 excepciones) y el cierre verbatim. Falta,
   además de lo bloqueado: el control de rastro binario (`BinaryTrailTests`), el gate de superficie
   (`SerializationSurfaceTests`), la constancia `2.0/docs/api-removals/US-000.BOLT-007.json` y la decisión negativa
   del escape-hatch en su forma final (§8.6).
3. **La decisión negativa del escape-hatch (AC-6) queda documentada acá**: **ningún** tipo lo reclama —no hay
   `ISerializable` ni `GetObjectData` ni contexto transportado en la superficie inventariada— y la vía para una
   futura excepción es el Bolt que la pida con los cinco criterios y su evidencia; este Bolt **no la concede**.
4. **Fuera de alcance por decisión del SPEC (§4):** los tipos del grafo y las excepciones del grafo (Bolt 003, que
   las hace POCOs en `Modeling` —verificadas por su propia suite—), `CompilerException` (Bolt 017), los `[DataContract]`
   de los `*.Auto.cs` (Bolt del set), el `[Serializable]`/`BinaryFormatter` de `Snapshot` (Bolts 002/025) y el
   cargador del migrador (Bolt 002: la **única** excepción declarada del producto, que además el §D.2 trata como
   allowlist del control de rastro).

---

## 14. AITL-MEM-Approval

**Pendiente** — el V-Bounce se detiene acá (§3.3/§2.12: registro del resultado → un MEM → manifiesto → **PAUSA**).
Esta sección y el bloque `review:` del frontmatter los completa el actor que ocupe `AITL-MEM-Approval`; el ejecutor
no se auto-aprueba, no delega el checkpoint y no avanza a un V-Bounce nuevo sin esa decisión.

- **Artefacto presentado:** este MEM + `v_bounces[0]` del manifiesto + el árbol **sin cambios** (§9.4).
- **Qué se pide decidir:** las **dos autorizaciones mínimas** de §8.3 (la política de nulabilidad del código aportado
  por la regla de §4 —o, alternativamente, diferir el aporte de `BaseObject`— y la arista `Data → Library`), que
  habilitan la revisión material de §4/§B.2 y desbloquean el entregable completo.
