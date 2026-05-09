# Domain Model (Modelo de Dominio legible)

## Propósito

Esta carpeta contiene la **representación legible por humanos** del modelo de
dominio: entidades, sus propiedades, relaciones entre ellas y enumeraciones/estados.

Es la **fuente de verdad editable** del dominio.

---

## Estructura

```
domain-model/
├── README.md              ← Este archivo
├── INDEX.md               ← Índice rápido de todas las entidades
├── TEMPLATE-ENTITY.md     ← Plantilla para documentar una entidad
├── entities/              ← Un archivo .md por entidad del dominio
│   ├── MiEntidad.md
│   └── ...
├── relationships.md       ← Vista centralizada de TODAS las relaciones
└── enums.md               ← Value sets, estados, códigos reutilizables
```

---

## Formato de cada entidad

Cada archivo en `entities/` sigue el `TEMPLATE-ENTITY.md`. Incluye:

- **YAML frontmatter** — metadatos parseable (nombre, label, módulo, versión)
- **Descripción** — qué representa la entidad en el negocio
- **Propiedades** — tabla con nombre, tipo, restricciones y descripción
- **Relaciones** — tabla con destino, cardinalidad y descripción
- **Reglas de negocio** — invariantes o restricciones propias de la entidad
- **Ejemplo** — instancia concreta para validar comprensión

---

## Cómo usar

### Agregar una entidad nueva

1. Copiar `TEMPLATE-ENTITY.md` a `entities/NombreEntidad.md`
2. Completar frontmatter, propiedades y relaciones
3. Actualizar `INDEX.md` con la nueva entrada
4. Si hay relaciones nuevas, actualizar `relationships.md`
5. Si hay enums/estados nuevos, actualizar `enums.md`

---

---

## Ciclo de vida de una entidad

| Estado | Significado |
|--------|-------------|
| **borrador** | Entidad identificada pero no completamente modelada. Propiedades y relaciones tentativas. |
| **estable** | Entidad completamente modelada y validada. Lista para ser usada en User Stories y Specs. |
| **deprecado** | La entidad dejó de ser relevante para el dominio actual. Se mantiene como referencia. |

El INDEX.md refleja el estado en la columna "Estado".

---

## Notas

- Las entidades se nombran en **PascalCase** (ej. `SolicitudPago.md`).
- Los nombres de propiedades usan **camelCase** (ej. `fechaVto1`).
- Los tipos de datos siguen la convención XSD: `string`, `integer`, `decimal`,
  `boolean`, `dateTime`, `date`.
- La cardinalidad se expresa como: `1`, `0..1`, `1..N`, `0..N`.
