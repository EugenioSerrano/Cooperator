# Functional (Análisis Funcional)

## Propósito

Esta carpeta contiene **análisis funcionales**: documentos que definen QUÉ debe hacer
el sistema desde la perspectiva del usuario/operador, ANTES de decidir cómo implementarlo.

Un FA captura historias de usuario, reglas de negocio, criterios de aceptación y el
comportamiento esperado. No define implementación (→ Spec).

### Rol clave en el flujo

Los documentos funcionales son la **fuente primaria** para crear Specs de funcionalidad
nueva. Dentro de la metodología, cada FA debería contener las **User Stories** y sus
**Bolts** (unidades atómicas de trabajo) ya definidos y priorizados, de modo que al
momento de generar una Spec, el equipo (o el agente AI) pueda tomar directamente
las historias y bolts como input sin ambigüedad.

```
FA (User Stories + Bolts) + ADR (decisiones técnicas) → SPEC → IMPL → MEM
```

---

## ¿Qué documentos van aquí?

- **Historias de usuario (User Stories)** con criterios de aceptación.
- **Bolts** — descomposición atómica de cada historia en unidades de trabajo implementables.
- Reglas de negocio y restricciones funcionales.
- Flujos de usuario / operador.
- Definición de estados y transiciones a nivel funcional.
- Requisitos de interacción (interfaz de usuario, controles, indicadores).

---

## Organización y convención de nombres

La organización de esta carpeta es **completamente libre**. El analista funcional
(o el equipo) define la estructura que mejor se adapte al proyecto. No hay prefijos
obligatorios ni convención impuesta.

> **Recomendación:** Se sugiere usar el prefijo `US-NNN` para User Stories (como en
> `TEMPLATE-US.md`) por trazabilidad, pero no es obligatorio. El equipo puede optar
> por otros esquemas (nombres descriptivos, agrupación por módulo, etc.).

Cada proyecto puede organizarse por Epics, Features, User Stories, módulos, o cualquier
otro criterio. Lo importante es que los documentos sean fáciles de encontrar y que el
INDEX.md se mantenga actualizado como índice navegable.

---

## Sincronización con herramientas SDLC (MCP Server)

Esta carpeta está diseñada para sincronizarse automáticamente con herramientas de
gestión de ciclo de vida (SDLC) como **Azure DevOps**, **Jira**, **GitHub Projects**,
etc., a través de un **MCP Server**.

El flujo típico es:

```
Herramienta SDLC (Tickets / Work Items / PBIs)
        ↕  MCP Server (sincronización bidireccional)
Documentos Markdown en functional/
        ↓
INDEX.md (índice actualizado automáticamente)
```

Esto permite que:
- Los tickets/work items se reflejen como documentos Markdown versionados en Git.
- Los documentos en `functional/` sean la fuente de verdad para los agentes AI.
- Los cambios en la herramienta SDLC se sincronicen con el repositorio y viceversa.

---

## Estructura recomendada

1. **Título y metadatos** — Nombre, fecha, estado, stakeholders.
2. **Contexto** — Problema que resuelve, situación actual.
3. **Historias de usuario** — Formato: "Como [rol], quiero [acción], para [beneficio]".
4. **Bolts** — Descomposición de cada historia en unidades atómicas de trabajo.
5. **Reglas de negocio** — Restricciones y condiciones del dominio.
6. **Criterios de aceptación** — Condiciones verificables de completitud.
7. **Flujos de usuario** — Secuencia de interacciones, escenarios principales y alternativos.
8. **Mockups / Wireframes** — Representación visual si aplica.
9. **Impacto** — Módulos afectados, dependencias, riesgos.
10. **Alineación con herramienta de gestión** — Work Items, Sprint, Board asociados.

### Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas, gráficos y cualquier otro elemento visual (no ASCII art ni imágenes embebidas).

---

---

## Ciclo de vida de US y Bolts

| Estado | Significado |
|--------|-------------|
| **vigente** | La US o Bolt está en scope. Se implementa o se implementará. |
| **deprecado** | La US o Bolt fue eliminada del scope (decisión de producto, cambio de prioridades). Se mantiene como referencia. |

El INDEX.md refleja el estado: ✅ Vigentes (vigente) / ⛔ Deprecados (deprecado).

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado completo.
