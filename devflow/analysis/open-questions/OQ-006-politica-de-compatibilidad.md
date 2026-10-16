---
id: "OQ-006"
title: "¿Qué política de compatibilidad rige la 2.0 para las apps generadas?"
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
related: ["OQ-001", "OQ-005"]
tags: ["migration", "compatibility", "net10", "packaging"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta surgida en la sesión interactiva de respuesta de OQs (el "tema muy importante"
  planteado por el dueño del producto durante la aprobación temprana del REV-001).
  Respondida en la misma sesión: opción A (moderno-only).
-->

# OQ-006 — ¿Qué política de compatibilidad rige la 2.0 para las apps generadas?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿Las apps generadas con Cooperator que **no usan las features cortadas** deben seguir funcionando al pasar a la 2.0, y **sobre qué runtimes** se garantiza la compatibilidad?
> - **Opciones:** A — moderno-only (.NET 10+) · B — puente multi-target (ns2.0+ns2.1 para apps .NET Framework) · C — rediseño sin garantías
> - **Recomendado / elegido:** A — las apps .NET Framework permanecen en 1.4 hasta su migración de runtime; la 2.0 se consume desde .NET 10+.

## 1. Pregunta

¿Qué política de compatibilidad rige la 2.0: las apps generadas que no usan las features cortadas deben seguir funcionando, y sobre qué runtimes se garantiza?

## 2. Contexto

### 2.1 De qué se trata

- Las apps **generadas** por el Modeler referencian `Core`/`Data`/`Library` (verificado en las plantillas); no tocan `Utility`/modelo (eso es mundo Modeler).
- El impacto de los cortes recientes (OQ-005: VB.NET, Oracle, export a Excel) sobre las apps generadas es mínimo: VB es del generador; Oracle nunca funcionó; el export solo afecta a quien lo use.
- El **1.4 se congela**: nada de lo decidido toca las apps ya generadas y en producción.

### 2.2 La pregunta en detalle

El punto duro es el **runtime**: `netstandard2.1` **no es consumible desde .NET Framework** (las apps WinForms 4.8 no pueden cargar las DLLs de la 2.0). Las salidas eran: (A) moderno-only — las apps 4.8 quedan en 1.4 hasta migrar a .NET moderno; (B) multi-target (ns2.0 + ns2.1) para que las apps 4.8 consuman la 2.0 sin migrar runtime; (C) sin garantías.

### 2.3 Un ejemplo

Una app WinForms 4.8 generada ayer: con **A** sigue en 1.4 intacta y, el día que migre, pasa a `net10` y consume la 2.0. Con **B** podría consumir la 2.0 sin migrar de runtime, a costa de una matriz doble de compilación/tests.

## 3. Opciones

| # | Opción | Qué significa | Riesgo / costo |
|---|--------|---------------|----------------|
| A | **Moderno-only (.NET 10+)** | La 2.0 se consume solo desde .NET moderno; las apps 4.8 permanecen en 1.4 hasta su migración de runtime | Ninguna app queda sin camino; quien se quede en 4.8 no recibe features nuevas |
| B | **Puente multi-target (ns2.0+ns2.1)** | Las apps .NET Framework consumen la 2.0 sin migrar runtime | Condicionales de compilación y matriz de tests doble |
| C | **Sin garantías** | Cada app se adapta/re-genera según su caso | Riesgo de migraciones one-off sin criterio común |

## 4. Recomendación

- **Opción recomendada:** A (elegida por el dueño).
- **Por qué:** el consumo de la 2.0 será exclusivamente desde .NET 10 en adelante; el puente ns2.0 no aporta a ningún consumidor real y duplica la matriz de build.
- **Confianza:** alta (declaración directa del dueño del producto).
- **Qué la cambiaría:** que aparezcan apps .NET Framework con vida larga que necesiten features de la 2.0 sin migrar de runtime → reabrir con opción B.

## 5. Hipótesis de trabajo

> Opción A: Cooperator 2 = consumo desde .NET 10+; premisa de librerías `netstandard2.1` se mantiene (el ADR de TFM comparará ns2.1 vs target directo `net10.0`).

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | human:eugenioserrano | Abierta durante la sesión interactiva de OQs: «hay un tema muy importante: todas las aplicaciones que generamos con cooperator, si no usan algunas de las características que vamos a cortar, deberían seguir funcionando, ¿sí?» |
| 2026-10-15 | human:eugenioserrano | Respondida en la misma sesión: «solo voy a usar Cooperator 2 con .NET 10 en adelante» → opción A. |

## 7. Resolución

- **Decisión:** **Opción A — moderno-only**: Cooperator 2 se consume con **.NET 10+**; **no** se multi-targetea el puente ns2.0. Las apps .NET Framework permanecen en el **1.4 congelado** hasta su migración de runtime. Para las apps migradas rige la meta: *«las que no usan las features cortadas siguen funcionando — compilan y operan con cambios mínimos»* como criterio de diseño de los ADRs y de la guía de migración.
- **Respuesta vs recomendación:** seguida (A).
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva.
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas»). Produce (propuesta — G40): criterios de compatibilidad en los ADRs de layout/TFM/packaging + banco de pruebas de compatibilidad con los proyectos generados de muestra (p. ej. `Ventas`).

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta y respondida (opción A) en la sesión interactiva; propagada a REV-001 §7 | human:eugenioserrano |
