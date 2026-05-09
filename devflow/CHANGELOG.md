# Changelog — Avenga AI-Native-SDLC Dev Flow

Registro de cambios al propio framework de documentación.
Este archivo documenta la evolución de la estructura, templates,
convenciones y flujos del Dev Flow.

> **Formato:** Cada entrada incluye fecha, descripción y archivos afectados.
> Ordenado de más reciente a más antiguo.

---

## [2026-05-06] — Estándares de calidad documental + campo LLM + timestamp obligatorio

- **Problema detectado:** Diferentes modelos LLM producen documentos con niveles de detalle muy dispares (Claude genera SPECs/MEMs detallados y autocontenidos; DeepSeek genera documentos telegráficos de pocas líneas sin contexto suficiente).
- **Nuevo estándar de calidad en `spec/README.md`** — Sección "Estándar de calidad mínimo (OBLIGATORIO)" con reglas de contenido mínimo, tabla de anti-patrones vs. lo correcto, y métricas de completitud.
- **Nuevo estándar de calidad en `memory/README.md`** — Misma estructura: reglas, anti-patrones y preguntas-guía de completitud.
- **Templates SPEC y MEM mejorados** — Comentarios HTML `⚠️ OBLIGATORIO` en secciones clave (Contexto, Fases, Resumen ejecutivo, Archivos, Decisiones) explicando el nivel de detalle mínimo esperado. Checklist de SPEC ampliado con 3 items nuevos.
- **Campo `llm` agregado a TODOS los templates** (12 archivos) — Trazabilidad de qué modelo generó cada artefacto. Templates afectados: SPEC, MEM, ADR, DISC, US, BOLT, REV, RISK, BUG, INTERVIEW, ENTITY, GLOSSARY.
- **Regla de timestamp del sistema** — Instrucciones explícitas en READMEs, templates y agents: `YYMMDD-HHmm` DEBE obtenerse con `Get-Date -Format "yyMMdd-HHmm"` (PowerShell) o `date +"%y%m%d-%H%M"` (Bash). NUNCA inventar.
- **Agents actualizados** (`gh-copilot/AvengaDevFlow.agent.md` + `open-code/AvengaDevFlow.md`) — Nuevas secciones: "Documentation Quality Standards (MANDATORY)", "Timestamp Rule (MANDATORY)", "LLM Field Rule (MANDATORY)" con ejemplos comparativos de estilo telegráfico vs. explicativo.

**Archivos afectados:**
- `devflow/spec/README.md`
- `devflow/spec/TEMPLATE-SPEC.md`
- `devflow/memory/README.md`
- `devflow/memory/TEMPLATE-MEM.md`
- `devflow/adrs/TEMPLATE-ADR.md`
- `devflow/discovery/TEMPLATE-DISC.md`
- `devflow/functional/TEMPLATE-US.md`
- `devflow/functional/TEMPLATE-BOLT.md`
- `devflow/reviews/TEMPLATE-REV.md`
- `devflow/risks/TEMPLATE-RISK.md`
- `devflow/bugs/TEMPLATE-BUG.md`
- `devflow/analysis/interviews/TEMPLATE-INTERVIEW.md`
- `devflow/analysis/domain-model/TEMPLATE-ENTITY.md`
- `devflow/analysis/glossary/TEMPLATE-GLOSSARY.md`
- `devflow/agents/gh-copilot/AvengaDevFlow.agent.md`
- `devflow/agents/open-code/AvengaDevFlow.md`

## [2026-05-02] — Nueva subcarpeta `analysis/domain-model/`

- **Creada carpeta `domain-model/`** dentro de `analysis/` — representación legible por humanos del modelo de dominio:
  - `README.md` — propósito, estructura, flujo de trabajo y ciclo de vida
  - `TEMPLATE-ENTITY.md` — plantilla con YAML frontmatter + tablas de propiedades/relaciones/reglas/ejemplo
  - `INDEX.md` — índice de entidades del dominio
  - `entities/` — un `.md` por entidad (PascalCase)
  - `relationships.md` — vista centralizada de todas las relaciones con diagrama Mermaid ER
  - `enums.md` — catálogo de estados, códigos y value sets reutilizables
- **Actualizado `analysis/README.md`** — agregado `domain-model/` a estructura, diagrama de flujo y pasos de trabajo
- **Flujo:** `domain-model/` (editable, legible) → alimenta → `functional/` (User Stories) y `adrs/` (decisiones de modelado)

## [2026-05-02] — Nueva carpeta `analysis/` (Análisis de Dominio)

- **Creada carpeta `analysis/`** con README principal y subcarpetas iniciales:
  - `interviews/` — Transcripciones de entrevistas con stakeholders (README + INDEX)
  - `bpmn/` — Procesos de negocio en notación BPMN/Mermaid (README + INDEX)
  - `glossary/` y `domain-model/` se agregaron en iteraciones posteriores
- **Actualizado `input/README.md`** — Las transcripciones ahora van en `analysis/interviews/`, no en input/
- **Actualizado README principal** — Agregado `analysis/` a estructura de carpetas, flujo, diagrama y tabla de mapeo SDLC
- **Actualizado diagrama de flujo** — Nuevo nodo ANALYSIS entre SDLC y DISC/FA

## [2026-05-02] — Sincronización nomenclatura y mejora de templates

- **Renombrado TEMPLATE-FA.md → TEMPLATE-US.md** — alineado con metodología (User Story)
- **Mejorado TEMPLATE-US** — organización de archivos libre (sin imponer subcarpetas backend/frontend), decisión del analista
- **Mejorado TEMPLATE-BOLT** — DoR y DoD verbose con checklists, explicaciones y ejemplos típicos; reordenado secciones (Descripción → Criterios de aceptación → Tareas → DoR → DoD)
- **Eliminadas referencias crípticas** — DoR/DoD sacadas del frontmatter y tabla de resumen, ahora secciones propias con guía detallada

## [2026-05-02] — Branding y vinculación SDLC ↔ Dev Flow

- **Renombrado framework** de "devflow" a "Avenga AI-Native-SDLC Dev Flow" en README principal y CHANGELOG
- **Agregada sección 5** en `Avenga AI-Native-SDLC.md` — "Dev Flow: implementación documental de la metodología" con tabla de mapeo y link al README
- **Agregada sección de métricas** en `TEMPLATE-MEM.md` (Lead Time, Bounces, Tests, % AI-generated)

## [2026-05-01] — Mejoras evolutivas (baja prioridad)

- **Creada carpeta `risks/`** con README, INDEX y TEMPLATE-RISK para el Risk Register del proyecto
- **Creado CHANGELOG.md** (este archivo) para meta-gobernanza del framework
- **Agregada sección de métricas DORA** en `memory/README.md`
- **Enriquecido `reviews/README.md`** con diagrama de destino de hallazgos
- **Agregado Quick Start** al README principal

## [2026-05-01] — Mejoras de templates (post-análisis de ejemplos reales)

- **Creado TEMPLATE-BOLT** en `functional/` — faltaba template para Bolts
- **Mejorado TEMPLATE-SPEC** — alcance ✅/❌, build baseline, fases por bolt, stack técnico
- **Mejorado TEMPLATE-FA** — 1 US = 1 doc, bolts con capa, links a subcarpetas
- **Mejorado TEMPLATE-REV** — hallazgos H-NNN con severidad, leyenda, ubicación/impacto
- **Mejorado TEMPLATE-MEM** — resumen ejecutivo, fases, verificaciones post-implementación
- **Mejorado TEMPLATE-DISC** — severidad en brechas (🔴/🟡/🟢), stack tecnológico
- **Mejorado TEMPLATE-ADR** — fuentes, supersede, alternativas en tabla
- **Mejorado TEMPLATE-BUG** — tabla de metadatos, emojis de severidad

## [2026-05-01] — Mejoras de media prioridad

- **Traducido Avenga SDLC a español** — documento completo reescrito
- **Bolts actualizados** — timebox de 2h a 1 día (era 2-8h), medido en AI-time
- **V-Bounce redefinido** como ciclo del agente AI (no humano)
- **Creados 7 templates** (DISC, ADR, SPEC, REV, BUG, MEM, FA) con YAML frontmatter
- **Agregada sección NFRs** en `adrs/README.md`
- **Integrado `other-docs/`** en el diagrama de flujo principal
- **Agregado diagrama de flujo de bugs** en `bugs/README.md`

## [2026-05-01] — Mejoras de alta prioridad

- **Integrado `bugs/`** en el diagrama de flujo principal (tercer subgraph con TDD)
- **Resuelto conflicto de nomenclatura** SPEC-FIX-BUG-NNN → unificado a SPEC-YYMMDD-HHmm-fix-bug-NNN
- **Creada tabla de mapeo** SDLC ↔ Dev Flow (20 filas) en README principal
- **Poblado INDEX.md** de `avenga-sdlc/` con el documento existente
- **Limpiados artefactos HTML** (`{=html}`, `<!-- -->`, `**\`) del SDLC

## [2026-05-01] — Análisis inicial

- Análisis completo de los 16 documentos del framework
- Identificadas 6 inconsistencias, 8 piezas faltantes, 15 oportunidades de mejora
- Clasificación por prioridad: 5 alta, 5 media, 5 baja
