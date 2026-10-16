# Formato publicado del registro de rupturas (`api-removals/`)

> **SPEC-261016-0047 §6/A.2.** Este directorio es **el registro de rupturas de la 2.0** y este archivo es su
> **formato publicado**: cualquiera que necesite registrar una ruptura encuentra acá sus campos, sus clases, los
> **tres datos obligatorios** (ADR-008 §3.1) y **el validador** que la revisa — sin adivinar la forma ni los datos
> exigidos.

## Qué es y qué NO es

- **Es** el registro estructurado de lo que la 2.0 rompe frente a 1.4 y de lo que depreca dentro de su línea, con
  **qué** desaparece, **por qué** y **con qué** se reemplaza; y es de **contribución distribuida**: **cada Bolt que
  rompe superficie entrega su propia entrada** en el mismo acto que su cambio (ADR-008 §3.1). El Bolt 028
  (`SPEC-261016-0047`) **estructura, valida y consolida** — no es dueño exclusivo y su entrega **no** es
  prerequisito del gate de ningún otro Bolt.
- **No** autoriza ninguna ruptura: una ruptura la **ordena su ADR** (inmutable) y la **ejecuta su Bolt**. El gate de
  compatibilidad (`2.0/build/api-compat.ps1`, ADR-008 §5, Bolt 001) **falla** ante una diferencia **no cubierta**
  por el registro, y **una entrada no habilita nada por sí sola**.
- **No** promete compatibilidad binaria ni un mecanismo de serialización que el framework ya no sostiene: cuando no
  hay reemplazo, la entrada dice **por qué** el escenario no existe en el runtime objetivo (ADR-008 §3.3).

## Archivos del directorio

| Archivo | Qué es |
|---|---|
| `<BOLT>.json` | **Un fragmento por Bolt** que rompe superficie (por ejemplo `US-000.BOLT-006.json`). Es la unidad del registro. |
| *(fuera de este directorio)* `../api-removals-decisions.json` | El **mapa de decisiones**: qué cláusula de qué ADR declara un cambio de comportamiento o una deprecación, y qué entrada (o qué Bolt dueño, si aún no se ejecutó) lo cubre. **No** es un fragmento y **no vive acá** a propósito: los lectores toman `api-removals/*.json` y un archivo sin `entries` en este directorio rompería a los que ya existen. |
| `api-removals.schema.json` | El **esquema** del fragmento: la validación estructural de cada entrada, publicada. Declara una clave `entries` **vacía e inerte** (JSON Schema ignora las palabras clave que no conoce) para que los lectores de fragmentos que recorren `api-removals/*.json` —la prueba precedente de los Bolts 006/007, que este Bolt **conserva**— no interpreten el esquema como un fragmento. |
| `README.md` | Este archivo. |

El **documento consolidado** de las notas vive en [`../MIGRATION-NOTES.md`](../MIGRATION-NOTES.md) y se **genera**
desde los fragmentos con `2.0/build/migration-notes.ps1` (consolidación **determinista**: orden canónico
`área → Bolt → id`). No se edita a mano: se edita el fragmento y se vuelve a consolidar.

## La entrada: campos

**Obligatorios** (el validador rechaza la entrada si falta alguno, con su motivo):

| Campo | Qué declara |
|---|---|
| `id` | Identificador de la entrada dentro del fragmento, con la convención `BOLT<NNN>-<NN>` (por ejemplo `BOLT007-04`). |
| `kind` | La clase de la ruptura, del **vocabulario publicado** (tabla siguiente). |
| `what` | **(1) Qué desaparece**: el tipo, miembro y firma exacta de lo retirado —o el comportamiento que cambia, con su descripción y su alcance—. |
| `why` | **(2) Por qué**: la decisión que lo ordena (ADR y cláusula, y cuando exista la OQ/REV que la motivó). |
| `replacement` | **(3) Con qué se reemplaza** en el código del consumidor. Si no hay reemplazo, `"none"` **con `replacement_reason` obligatoria**. |
| `evidence` | La evidencia de la verificación de ese Bolt (qué se midió y dónde). |

**El dueño:** `owner_bolt` en la entrada, o el `bolt` de la raíz del fragmento (todas las entradas comparten dueño
salvo que una entrada declare el suyo). Los fragmentos que registran una ruptura **materializada por otro Bolt**
agregan `owner_bolts` y `materialized_by`.

**Opcionales** (los usa el vocabulario o los agregó el corpus; el esquema los acepta):

| Campo | Para qué |
|---|---|
| `surface` | `public` \| `protected` \| `internal` — el alcance de lo retirado, cuando aplica. |
| `type`, `member`, `signature` | El ancla **estructural** de la ruptura: el tipo (calificado), el miembro y la **firma exacta**. Es lo que permite cruzar la entrada contra el reporte del gate. |
| `class` | La clase derivada de `kind` (ver abajo). Se puede declarar de forma explícita **sólo si coincide** con la derivada. |
| `next_step` | El **paso siguiente obligatorio** del consumidor cuando la ruptura exige más que un cambio de código (regenerar artefactos, migrar valores, cambiar de runtime) — AC-7. |
| `announce_window` | En una `deprecation`: la **ventana de aviso** (ADR-008 §3.5: al menos una minor con `[Obsolete]` antes del retiro en el próximo major). |
| `status`, `note`, `cross_references`, `materialized_by`, `owner_bolts` | Trazabilidad del corpus: estado de ejecución, notas de alcance, referencias cruzadas entre entradas y el Bolt que materializa la ruptura. |

**En la raíz** del fragmento: `bolt` (obligatorio), `entries` (obligatorio, puede ser vacío: un Bolt sin rupturas
es un archivo válido) y, opcionalmente, `spec`, `spec_revision`, `produced_by`, `produced_at`, `purpose`,
`source_of_shape`, `escape_hatch`, `registered_obligations` y los campos de corrección de arnés que el corpus ya
usa.

## Las clases de entrada y el vocabulario de `kind`

La **clase** dice **cómo se valida** la entrada. Se **deriva de `kind`** (tabla de abajo) y el validador exige que
una `class` explícita coincida con la derivada.

| Clase | Cómo se valida | `kind` del vocabulario |
|---|---|---|
| `api_removal` | **Contra el reporte del gate**, en los dos sentidos (§C.1): toda diferencia de miembro/atributo retirado está cubierta por una entrada, y toda entrada de superficie corresponde a una ruptura real —el tipo que declara **existe en la línea base 1.4** y su diferencia es visible en el reporte (o su ensamblado es el que el reporte declara ausente). | `method-removed`, `member-removed`, `member-added`, `signature-changed`, `type-removed`, `type-not-ported`, `namespace-and-assembly-move`, `type-split`, `base-type-change`, `coupling-removal`, `attribute-removal`, `serialization-removal`, `serialization-surface-removed`, `platform-trace-removed` |
| `behavior_change` | **Contra las decisiones** que la ordenan (§C.2): la cláusula citada existe en su ADR y el mapa de `decisions.json` la declara. | `behavior-change`, `build-policy`, `deferred-obligation` |
| `deprecation` | Igual que `behavior_change`, y además declara su **ventana de aviso** (`announce_window`). | `deprecation` |

Una ruptura que sea **a la vez** diferencia de API y cambio de comportamiento se registra como **dos entradas**
(una de cada clase), o como una entrada `api_removal` cuya diferencia esté cubierta: el cruce exige que ninguna de
las dos caras quede sin entrada.

### Rupturas que el gate no puede ver

El gate compara **API pública**. Hay rupturas reales que **no** producen una diferencia:

- **Atributos y directivas** (`attribute-removal`, `serialization-removal`, `platform-trace-removed`,
  `serialization-surface-removed` cuando el tipo no pierde además un miembro público): `[Serializable]`,
  `[field: NonSerialized]` y un `using` no son miembros de la API.
- **Miembros que no eran públicos** (un campo privado retirado, un tipo `internal`).
- **Comportamiento y deprecaciones**: el caché que pasa a expirar, `App.config` → `appsettings.json`, lo que el
  modelo deja de transportar, la migración de valores de criptografía.

Para las dos primeras, la verificación de la entrada es su **`evidence`** y su `type` debe existir en la línea base
(no se registra un retiro que nunca ocurrió); el registro las **señala** como no observables por el gate. Las
terceras se validan contra su decisión, nunca fingiendo que el gate las cubre (§C.2).

## Cómo contribuye un Bolt (y cómo se valida)

1. **En el mismo acto que su cambio** (ADR-008 §3.1), el Bolt agrega **su** archivo `2.0/docs/api-removals/<BOLT>.json`
   con **una entrada por ruptura** y los tres datos obligatorios.
2. **Valida el esquema**: `MigrationNotesCoverageTests` recorre **todos** los fragmentos y rechaza el que esté mal
   formado o incompleto —falta de uno de los tres datos, `kind` fuera del vocabulario, `api_removal` de miembro sin
   firma, `"none"` sin razón, sin Bolt responsable o sin evidencia— con el motivo exacto.
3. **Corre el cruce**: `dotnet test` sobre `2.0/tests/Cooperator.Harness.Tests` (o el arnés completo con
   `pwsh -File 2.0/build/build.ps1 -Scope Hermetic`) ejerce los dos sentidos de §C.1 y el cruce de §C.2 sobre el
   **reporte real** del gate (`2.0/build/api-compat.ps1 -Mode Current`).
4. **Consolida**: `pwsh -File 2.0/build/migration-notes.ps1` regenera `MIGRATION-NOTES.md` (determinista; el arnés
   verifica que el documento versionado sea exactamente el que produce la consolidación).

> **El registro crece por contribución**: este Bolt no espera a nadie y **no** es prerequisito de nadie. La
> verificación es **por diferencia**, no por documento: un Bolt cuya ruptura esté cubierta por su propio fragmento
> pasa su gate aunque el consolidado se regenere después.
