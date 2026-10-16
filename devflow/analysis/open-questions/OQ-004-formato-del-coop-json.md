---
id: "OQ-004"
title: "¿Formato oficial y extensión del archivo de modelo en la 2.0 (.coop JSON)?"
date: "2026-10-15"
author: "human:eugenioserrano"
agent: "agent:avenga-devflow"
llm: "deepseek/deepseek-flash"
status: "answered"
priority: "P1"
owner: "human:eugenioserrano"
validator: "human:eugenioserrano"
targets:
  - "../../reviews/REV-001-analisis-migracion-cooperator.md"
sources:
  - "../../reviews/REV-001-analisis-migracion-cooperator.md"
related: []
tags: ["migration", "coop-format", "json", "modeler"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta del análisis de migración 1.4 → 2.0 (REV-001, F-41/F-42/F-43).
  La respuesta produce: un ADR (formato del archivo de modelo). Nunca solo (G40).
-->

# OQ-004 — ¿Formato oficial y extensión del archivo de modelo en la 2.0 (.coop JSON)?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿El archivo de modelo de la 2.0 es JSON conservando la extensión `.coop` (ahora texto), o cambia de nombre/extensión (p. ej. `.coop.json`)?
> - **Opciones:** A — JSON con extensión `.coop` · B — `.coop.json` · C — ambas aceptadas
> - **Recomendado:** A — no rompe asociaciones ni rutinas existentes; el contenido pasa a texto igual.

## 1. Pregunta

¿Cuál es el formato oficial y la extensión del archivo de modelo de la 2.0: JSON con la extensión actual `.coop`, o un nombre nuevo (`.coop.json` u otro)?

## 2. Contexto

### 2.1 De qué se trata

Hoy el archivo `.coop` es un **blob binario** (`BinaryFormatter` sobre un `Snapshot`, REV-001 F-41) que: (a) es imposible de mergear en git, (b) depende del layout binario de las clases, y (c) **el runtime objetivo ya no puede deserializar** sin opt-in inseguro (eliminado en .NET 9+). El plan del REV (F-41) es JSON + `schemaVersion` + un conversor one-shot que **debe correr sobre .NET Framework 4.8, antes de migrar** (irreversible). El Modeler 1.4 registró la asociación de la extensión `.coop` en el Registro (F-35).

### 2.2 La pregunta en detalle

- **Lo que pesa:** familiaridad y rutinas de los usuarios actuales (doble clic abre el Modeler); compatibilidad de la asociación existente; ergonomía de herramientas (diff/merge, editores JSON); claridad de que el archivo es texto.
- **Lo decidido aguas arriba:** que sea **texto (JSON) es objetivo del negocio** (editable por agentes, mergeable). Lo que queda abierto es la **extensión** y si se aceptan ambos formatos durante una transición.

### 2.3 Un ejemplo

Dos desarrolladores editan el mismo modelo: hoy el `.coop` binario es un conflicto irresoluble; con JSON, un merge normal resuelve las líneas en conflicto — y un agente puede editarlo como cualquier texto.

## 3. Opciones

| # | Opción | Qué significa | Qué cambia (en `targets`) | Riesgo / costo |
|---|--------|---------------|-----------------------------|----------------|
| A | **JSON con extensión `.coop`** | Mismo nombre de archivo, contenido texto; asociación existente sigue funcionando (re-registrada por la 2.0) | ADR de formato: `.coop` = JSON desde `schemaVersion 1` | Editores genéricos pueden no reconocer `.coop` como JSON |
| B | **`.coop.json`** | Extensión doble: explícita como JSON | Re-registro de asociación; los `.coop` viejos no abren sin conversión | Rompe rutinas/asociaciones de usuarios existentes |
| C | **Aceptar ambas al abrir** | El Modeler 2.0 abre `.coop` y `.coop.json`; guarda en la elegida | Más soporte en el Modeler | Más complejidad sin beneficio si no hay legado conviviendo |

## 4. Recomendación

- **Opción recomendada:** A
- **Por qué:** mantiene la continuidad operativa (asociación, doble clic, tutoriales) y el contenido igual será texto; el costo de B (re-registro + confusión) no compra nada que el contenido no dé.
- **Confianza:** alta
- **Qué la cambiaría:** que prefieras que editores JSON lo abran por defecto (→ B) o que haya archivos de ambos mundos conviviendo largo tiempo (→ C).

## 5. Hipótesis de trabajo

> Asumiendo option A (`.coop` pasa a ser JSON con `schemaVersion`); si es incorrecta, cambia la asociación registrada y la documentación del Modeler.

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | agent:avenga-devflow | Abierta desde REV-001 (run ARUN-261015-0022, F-41/F-42/F-43) |
| 2026-10-15 | human:eugenioserrano | Respondida en sesión interactiva: extensión `.coop2` para el JSON + migrador `.coop` → `.coop2` requerido (ubicación a definir). |

## 7. Resolución

- **Decisión:** **extensión `.coop2`** para el nuevo formato JSON (los `.coop` v1 quedan como legado binario) **+ un migrador `.coop` → `.coop2` como entregable requerido** (ubicación a definir en el ADR/SPEC — candidatos: modo conversor del Modeler 1.4 o utilidad CLI standalone).
- **Respuesta vs recomendación:** elección propia (distinta de las opciones A/B/C presentadas): nueva extensión explícita para v2 + herramienta de conversión, en lugar de reutilizar `.coop`/`.coop.json`.
- **Restricción de secuencia:** el migrador debe correr sobre .NET Framework 4.8 con el binario aún legible (prerequisito de F-41: una vez migrado el Modeler, `BinaryFormatter` ya no está disponible; conversión irreversible — batch de todos los `.coop` históricos ANTES de migrar).
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva.
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas»). Produce (propuesta — G40): ADR «formato `.coop2`» (esquema JSON, `schemaVersion`, extensión, UX/ubicación del migrador) + Bolt del migrador (4.8) + Bolt del formato en el Modeler 2.0.

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta (draft del run ARUN-261015-0022) | agent:avenga-devflow |
| 2026-10-15 | Status → answered (extensión `.coop2` + migrador requerido); propagada a REV-001 §7 | human:eugenioserrano |
