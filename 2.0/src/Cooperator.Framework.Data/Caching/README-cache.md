# El caché de la capa de datos (`Cooperator.Framework.Data.Caching`)

Documentación del contrato publicado por `US-000.BOLT-013` (SPEC-261016-0019, ADR-004 §3.1/§3.2/§3.5).
Lo que sigue es **contrato público**: vive aquí, no en un comentario interno.

## 1. El contrato

`ICacheProvider` — superficie **mínima**, claves del llamador y TTL por entrada:

| Miembro | Qué hace |
|---|---|
| `bool TryGet<T>(string key, out T value)` | Lee una entrada **viva**. Una entrada vencida **nunca** se sirve: se descarta al leerla y la lectura falla. |
| `void Set<T>(string key, T value, TimeSpan ttl)` | Guarda con su vencimiento. Un valor `null` y un TTL no positivo **no se cachean**. |
| `T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl)` | Devuelve la entrada viva o la crea con la fábrica y la guarda. Si la fábrica falla, **no** se cachea el fallo. |
| `void Remove(string key)` | Remueve esa clave (y sólo esa). |

**No hay invalidación por prefijo** (ADR-004 §3.1): quien invalida **conoce sus claves**. En 2.0.0 no se publica
ningún adaptador de caché; el default en memoria vale para todos los hosts.

`InMemoryCacheProvider` es la implementación por defecto: BCL puro (sin dependencias de terceros), thread-safe,
TTL por entrada, expiración perezosa y **cap de entradas con desalojo** (primero las vencidas y, si no alcanza, la
de vencimiento más cercano). El cap se recibe por constructor y el **reloj es sustituible** para poder probar el
vencimiento sin esperas reales. El invariante del límite se sostiene en **todo instante** —la escritura hace lugar
antes de insertar, dentro de la misma sección crítica—, no sólo al final de una tanda de escrituras.

## 2. El esquema de claves

`coop:{scope}:{rule}:{kind}` — el contrato **no** interpreta las claves; las recibe del llamador.

| Región | Clave publicada | Qué guarda | TTL |
|---|---|---|---|
| `collections` | `coop:data:{rule}:collection-all` · `coop:data:{rule}:collection-dictionary` | la colección completa y el diccionario de una regla (`TableName`) | `Cooperator:Cache:DefaultTtlSeconds` (**3600** s = 1 h, como 1.4) |
| `parameter-metadata` | `coop:data:sp-parameters:{sp}:metadata` | la metadata de parámetros descubierta de un stored procedure | `Cooperator:Cache:ParameterMetadataTtlSeconds` (**86400** s = 1 día, como 1.4) |
| `command-timeout` | `coop:data:command-timeout:value` | el timeout de comandos ya resuelto | el TTL genérico publicado (`Cooperator:Cache:DefaultTtlSeconds`): la región **no** tiene default propio en ADR-004 §3.1 |

`{rule}` es el nombre de la regla/tabla de la capa. Para el flag `includeReturnValueParameter` del descubrimiento
de parámetros la clave del caso común es la publicada (`…:metadata`) y la variante que exige el parámetro de retorno
usa `…:metadata-with-return-value`: la forma del valor **cambia** con ese flag, así que no puede compartir clave.

## 3. De dónde salen los límites

Ningún default vive en el código (ADR-004 §3.2). El catálogo (`2.0/docs/configuration-keys.md`, Bolt 011) publica:
`Cooperator:Cache:DefaultTtlSeconds`, `Cooperator:Cache:ParameterMetadataTtlSeconds` y
`Cooperator:Cache:MaxEntries`. La **cifra** del cap (`1024` entradas) la fija el Bolt 013 —dueño de la caché—, que
es a quien el Bolt 011 delegó el valor (`MEM-261016-0117` §8.1). El cap acota la memoria que un proceso de
escritorio de larga vida puede retener en la caché (lo que 1.4 **no** hacía: su caché no expiraba ni estaba acotada)
y queda holgadamente por encima del número de colecciones y stored procedures que la capa cachea en un proceso real;
cada entrada está a su vez acotada por el límite por colección de 1.4 (`MaxObjectsInCacheForReadOnlyTable`, 5000).

Un valor de configuración **inválido** no produce un fallback silencioso: se propaga el error tipado del contrato
de configuración (NFR *Config fail-fast*, ADR-004 §5).

## 4. Advertencias de comportamiento (para las notas de migración)

1. **El caché de colecciones ahora vence.** En 1.4, sin host web, vivía **para siempre** en memoria por instancia;
   en 2.0.0 tiene TTL (**1 h** por defecto) y además está acotado por el cap. Menos memoria retenida y menos datos
   rancios; a cambio, una colección cacheada se vuelve a leer cuando vence.
2. **Con un adaptador compartido (futuro, no en 2.0.0) el alcance pasa a ser entre instancias**, cuando en 1.4 era
   por instancia. En 2.0.0 no se publica ningún adaptador: es una propiedad del contrato, no un efecto de este
   entregable.
3. **Desaparece el espejo en el caché del host** y con él la plataforma web en la capa de datos: un solo camino
   para todos los hosts, sin `HttpContext`.

El inventario completo de los puntos de caché de la capa, cada uno con su resolución, está en el MEM del V-Bounce
que entregó esta caché.
