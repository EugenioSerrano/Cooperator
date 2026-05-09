# Metodología de Desarrollo (SDLC)

## Propósito

Esta carpeta almacena la documentación de la **metodología de desarrollo** utilizada
en el proyecto. Contiene la definición formal del ciclo de vida de desarrollo de
software (SDLC) que gobierna cómo se planifica, ejecuta, verifica y entrega el trabajo.

La metodología viaja con el código — todo integrante del equipo tiene acceso
directo sin buscar en wikis externas.

---

## ¿Qué documentos van aquí?

Documentación del **proceso de trabajo**, no del proyecto en sí:

- Definición de la metodología de desarrollo (SDLC completo o adaptación).
- Framework de trabajo: roles, artefactos, ceremonias, gates de calidad.
- Definición de unidades de trabajo (sprints, iteraciones, bolts).
- Micro-ciclo operativo (paso a paso desde input hasta entregable).
- Definition of Ready (DoR) y Definition of Done (DoD).
- Estrategia de métricas (DORA metrics, lead time, throughput).
- Integración de herramientas AI en el flujo de trabajo.
- Mapa de artefactos del SDLC (qué documento se genera en cada fase).

---

## Convención de nombres

No requiere prefijo numérico secuencial (pocos documentos, naturaleza estable):

```
Nombre-metodologia-descripcion.md
```

---

## Estructura recomendada

1. **Introducción y filosofía** — Principios, objetivos, frameworks base.
2. **Estructura del trabajo** — Unidades operativas, jerarquía de descomposición.
3. **Roles y responsabilidades** — Roles humanos e interacción con AI.
4. **Flujo operativo** — Paso a paso, gates de calidad, DoR/DoD.
5. **Artefactos** — Documentos por fase, mapeo fase → artefacto → carpeta.
6. **Métricas** — Flujo y calidad, umbrales, alertas.
7. **Herramientas y automatización** — Stack, CI/CD, AI.
8. **Gobernanza** — Trazabilidad, política de documentación, mejora continua.

---

## Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas, gráficos y cualquier otro elemento visual (no ASCII art ni imágenes embebidas).

---

## Lineamientos

- La metodología es un documento **vivo pero estable**. Se actualiza con mejoras al proceso, no en cada iteración.
- Cambios deben ser consensuados y comunicados.
- No es lugar para decisiones técnicas del proyecto (→ `adrs/`) ni specs de implementación (→ `spec/`).

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado de documentos.
