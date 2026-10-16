# Catálogo de claves de configuración — Cooperator 2.0

Documento **versionado junto al código** que publica las claves del framework con su **tipo**, su **default** y su
correspondencia con el inventario de 1.4. Existe por ADR-004 §3.2: *«toda región de caché con default propio tiene
su clave: el default no vive escondido en el código»*, y por el `AITL-SPEC-Approval` de `SPEC-261016-0018`, que
ordena publicar acá las claves del catálogo.

Los **nombres** de las claves son constantes del contrato (`Cooperator.Framework.Core.Configuration.ConfigurationKeys`),
de modo que ninguna viaja como cadena suelta.

---

## 1. Fuente por defecto y convención

El framework lee **sólo variables de entorno** (`EnvironmentConfiguration`), con la jerarquía en doble guion bajo:

| Clave del catálogo | Variable de entorno |
|---|---|
| `Cooperator:Data:CommandTimeoutSeconds` | `Cooperator__Data__CommandTimeoutSeconds` |
| `Cooperator:ConnectionStrings:CooperatorConnectionString` | `Cooperator__ConnectionStrings__CooperatorConnectionString` |
| `Cooperator:Mail:SmtpServer` | `Cooperator__Mail__SmtpServer` |

El framework **no** lee archivos ni define precedencias: la precedencia **entorno > archivo > default** es
propiedad del bootstrap de las aplicaciones generadas (ADR-004 §3.4, Bolt 021). Un valor **vacío** se trata como
**ausente**; no es un valor válido.

## 2. Claves publicadas

| Clave | Tipo | Default | En 1.4 |
|---|---|---|---|
| `Cooperator:ConnectionStrings:<nombre>` | cadena con **prefijo de esquema obligatorio** | **ninguno** — es obligatoria: su ausencia es error tipado | `ConnectionStrings[...]` del host. El nombre por defecto **se conserva**: `CooperatorConnectionString` |
| `Cooperator:Data:CommandTimeoutSeconds` | entero (segundos) | **`30`** — el de 1.4 cuando la clave no estaba definida (`new SqlCommand().CommandTimeout`) | `AppSettings["CooperatorCommandTimeOut"]` |
| `Cooperator:Cache:DefaultTtlSeconds` | entero (segundos) | **`3600`** (1 h, igual que 1.4) | — (caché de colecciones, sin TTL en escritorio) |
| `Cooperator:Cache:ParameterMetadataTtlSeconds` | entero (segundos) | **`86400`** (1 día, igual que 1.4) | — (caché de metadata de SP, en `Hashtable` estático) |
| `Cooperator:Cache:MaxEntries` | entero (entradas) | **`1024`** (la cifra la fija el Bolt 013, dueño de la caché — `MEM-261016-0117` §8.1) | — (el cap es nuevo en 2.0: ADR-004 §3.1) |
| `Cooperator:Mail:SmtpServer` | cadena | **ninguno** — obligatoria **al usarse** (lectura perezosa y validada) | `AppSettings["SMTPServerName"]`, leído en un inicializador estático (F-22) |
| `Cooperator:LazyLoad:Providers:<entidad>:Assembly` | cadena | **ninguno** — par obligatorio por entidad | sección custom `LazyLoad/ProvidersSection` del `.config` |
| `Cooperator:LazyLoad:Providers:<entidad>:Class` | cadena (**nombre completo calificado**) | **ninguno** — par obligatorio por entidad | ídem |

### 2.1 `Cooperator:Cache:MaxEntries` — clave publicada, cifra del dueño de la caché

ADR-004 §3.1 exige un **cap de entradas configurable** con desalojo (primero las vencidas), y §3.2 manda que su
default **no** viva escondido en el código. La clave y su **tipo** se publican acá —y como constante del contrato—
para que el default tenga su lugar:

```csharp
ConfigurationKeys.Cache.MaxEntries   // "Cooperator:Cache:MaxEntries"
```

**La cifra la fija el Bolt 013 —dueño de la caché—: `1024` entradas.** Publicar un número en esta entrega habría sido rellenar el hueco con una suposición; su dueño lo fijó al aportar la caché (`MEM-261016-0117` §8.1 lo delegó) y acota la memoria que un proceso de escritorio de larga vida retiene en la caché —cada entrada está a su vez acotada por el límite por colección de 1.4, `MaxObjectsInCacheForReadOnlyTable` = 5000—. Lo que sigue describe la clave y su lugar: Ningún ADR ni SPEC decide un número —ADR-004 §3.1 dice *configurable* pero no
lo cuantifica, y el catálogo de §3.2 y el ejemplo de §3.4 no lo traen—, de modo que publicar un valor sería
rellenar el hueco con una suposición (SPEC-261016-0018 §15 #2 y #8). El **dueño de la caché** es el **Bolt 013**
(`SPEC-261016-0019`), que fue quien pidió esta clave a este Bolt (§15 de su SPEC) y quien publica la cifra en este
mismo catálogo al aportar la caché. Queda **declarado y visible**, nunca escondido en el código.

## 3. Forma del valor de una cadena de conexión

| Forma | Resultado |
|---|---|
| `plain:Server=.;Database=Ventas;…` | **identidad**: el valor es el texto que sigue al prefijo |
| `Server=.;Database=Ventas;…` (sin prefijo) | **rechazado** con `CooperatorConfigurationException`: un valor sin prefijo **no** se asume texto plano |
| `aesgcm:<kid>:<base64>` | **rechazado en esta entrega**: el esquema no tiene resolutor registrado (el Bolt 022 lo aporta). El error nombra la clave **y el prefijo ofensor** |

El `Data` recibe el valor **crudo, con su prefijo**, y es quien invoca al `IProtectedValueResolver` (ADR-004 §3.3).
El framework **no** descifra, no adivina y no cae a un fallback silencioso.

## 4. Etiquetado de los providers LazyLoad (ADR-006 §3.1)

```json
{ "Cooperator": { "LazyLoad": { "Providers": {
    "Customer": { "Assembly": "Ventas.Lazy",
                  "Class": "Ventas.LazyProviders.CustomerLazyProvider" } } } } }
```

`…:Class` lleva el **nombre completo del tipo, calificado por namespace**, y `…:Assembly` el nombre **simple** del
ensamblado. La versión, la cultura y el token del ensamblado **no** se configuran (ADR-006 §3.1). El ejemplo sin
calificar de ADR-004 §3.4 queda **corregido por este catálogo**, que es el que gobierna la forma.

## 5. Qué **no** es una clave de este catálogo

- **Precedencias**: no hay clave que ordene fuentes — eso es del bootstrap (ADR-004 §3.4).
- **Criptografía**: no hay claves ni parámetros criptográficos acá; la protección es de ADR-007 y del Bolt 022.
- **La cadena de conexión de un host**: es un **valor**, nunca un default; los defaults no llevan credenciales.
