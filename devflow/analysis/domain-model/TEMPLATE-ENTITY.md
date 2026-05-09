---
entity: NombreEntidad
label: "Nombre Legible"
module: nombre-modulo
version: "1.0"
status: borrador | estable | deprecado
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
---

# NombreEntidad

## Descripción

> Breve descripción de qué representa esta entidad en el dominio de negocio.
> Incluir contexto suficiente para que alguien de negocio entienda el concepto.

---

## Propiedades

| Propiedad | Tipo | Obligatorio | Restricciones | Descripción |
|-----------|------|:-----------:|---------------|-------------|
| `propiedad1` | string | ✅ | max 50 chars | Descripción de la propiedad |
| `propiedad2` | integer | ✅ | PK | Descripción de la propiedad |
| `propiedad3` | decimal | ❌ | ≥ 0 | Descripción de la propiedad |
| `propiedad4` | boolean | ✅ | — | Descripción de la propiedad |
| `propiedad5` | dateTime | ❌ | ISO 8601 | Descripción de la propiedad |
| `propiedad6` | → Enum:`NombreEnum` | ✅ | ver enums.md | Descripción de la propiedad |

---

## Relaciones

| Relación | Destino | Cardinalidad | Descripción |
|----------|---------|:------------:|-------------|
| `nombreRelacion` | EntidadDestino | 1:N | Descripción de la relación |
| `otraRelacion` | OtraEntidad | N:1 | Descripción de la relación |

---

## Reglas de negocio

- **RN-01**: Descripción de una regla o invariante.
- **RN-02**: Otra regla de negocio relevante.

---

## Ejemplo

```yaml
propiedad1: "valor ejemplo"
propiedad2: 42
propiedad3: 1500.00
propiedad4: true
propiedad5: "2026-01-15T10:30:00Z"
propiedad6: "VALOR_ENUM"
```
