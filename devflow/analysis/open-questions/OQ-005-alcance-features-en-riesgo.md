---
id: "OQ-005"
title: "¿Qué features en riesgo (VB.NET, Oracle, export a Excel) siguen en el alcance de la 2.0?"
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
tags: ["migration", "scope", "codegen", "oracle", "excel"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta del análisis de migración 1.4 → 2.0 (REV-001, F-03/F-11/F-26/F-37/F-39).
  La respuesta produce: ajustes de alcance (DISC de uso real, ADRs/Bolts selectivos). Nunca solos (G40).
-->

# OQ-005 — ¿Qué features en riesgo (VB.NET, Oracle, export a Excel) siguen en el alcance de la 2.0?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿La 2.0 mantiene la generación VB.NET, el provider Oracle y el export a Excel, o alguna se depreca?
> - **Opciones:** A — mantener las 3 · B — deprecar las de uso no confirmado · C — investigar uso real por feature y decidir
> - **Recomendado:** C — las tres tienen costo real de migración y su uso actual es desconocido desde el código.

## 1. Pregunta

¿Qué features en riesgo —generación de código VB.NET, provider de base de datos Oracle y export a Excel— quedan dentro del alcance de la 2.0 y cuáles se depreca?

## 2. Contexto

### 2.1 De qué se trata

Tres funcionalidades del 1.4 que hoy funcionan solo por accidentes de plataforma y tienen costo concreto de migración:

1. **Generación VB.NET**: el motor de plantillas compila C#/VB en runtime con `CodeDomProvider`, que **no existe fuera de .NET Framework** (REV-001 F-26). Portar VB implicaría sumar Roslyn VB al rediseño del motor; y las plantillas VB generadas incluso emiten `System.Web` (F-37).
2. **Provider Oracle**: `OracleScriptor` es un esqueleto vacío (`// falta implementar`) y la referencia a `System.Data.OracleClient` está muerta (F-11/F-39). Hoy **solo funciona SQL Server**.
3. **Export a Excel**: se hace por `System.Data.OleDb` con el driver Jet 4.0 (32-bit, Windows) desde el Core (F-03).

### 2.2 La pregunta en detalle

- **Por qué se pregunta ahora:** cada una condiciona el diseño (el motor F-26, el particionado F-27, el Core F-03). Decidirlas tarde encarece el rediseño.
- **Qué está en juego:** trabajo de migración real (Roslyn VB / provider Oracle completo / reescritura del export con OpenXML) contra su **uso real**, que el código no revela. La fuente de verdad es el negocio (¿hay proyectos generados en VB? ¿clientes con Oracle? ¿reportes que usen el export?).
- **Nota:** las tres pueden deprecarse sin romper la migración de las librerías; mantenerlas es una decisión de alcance, no un bloqueante técnico.

### 2.3 Un ejemplo

Si ningún proyecto generado en los últimos años usa VB, sumar Roslyn VB al motor 2.0 es costo puro; si hay clientes con Oracle, el "provider" actual es solo una promesa y la 2.0 debería implementarlo de verdad o retirarlo con aviso.

## 3. Opciones

| # | Opción | Qué significa | Qué cambia (en `targets`) | Riesgo / costo |
|---|--------|---------------|-----------------------------|----------------|
| A | **Mantener las 3** | Roslyn VB en el motor, provider Oracle real, export reescrito (OpenXML) | Alcance 2.0 más grande y más Bolts | Costo alto por features posiblemente sin uso |
| B | **Deprecar las de uso no confirmado** | Retirar con nota de migración en la 2.0 | Menos alcance; comunicar a usuarios (si existen) | Riesgo de romper consumidores reales |
| C | **Investigar uso real y decidir por feature** | Un DISC corto (repos de clientes, soporte, históricos) por feature dudosa | Entrada para el ADR de alcance | Agrega un paso pequeño antes de decidir |

## 4. Recomendación

- **Opción recomendada:** C
- **Por qué:** el costo de mantener cada feature es verificable en el REV; el uso no. Un chequeo corto del uso real (o tu conocimiento directo) decide mejor que cualquiera de las dos posiciones extremas.
- **Confianza:** media-alta
- **Qué la cambiaría:** que ya sepas el uso real de cada una (entonces A o B por feature salteando el DISC).

## 5. Hipótesis de trabajo

> Asumiendo option C (investigar y decidir por feature); mientras tanto, los Bolts de migración tratan estas tres como fuera de alcance hasta que se decidan.

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | agent:avenga-devflow | Abierta desde REV-001 (run ARUN-261015-0022, F-03/F-11/F-26/F-37/F-39) |
| 2026-10-15 | human:eugenioserrano | Respondida en sesión interactiva: opción B ampliada — se deprecan las tres (VB.NET, Oracle, export a Excel). |

## 7. Resolución

- **Decisión:** **Opción B ampliada — se deprecan las TRES features** en la 2.0:
  - **VB.NET**: no continúa la generación VB (el motor rediseñado queda solo C#; no se suma Roslyn VB).
  - **Oracle**: no continúa (se eliminan el esqueleto `OracleScriptor` y la referencia muerta; SQL Server queda como único motor).
  - **Export a Excel**: se retira del framework (OleDb/Jet fuera; quien necesite exportar lo resuelve en su capa).
- **Respuesta vs recomendación:** elección propia (B directa, sin DISC): el uso real no justifica el costo de ninguna de las tres.
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva.
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas») + §6 (ítems 6 y 8 actualizados). Produce (propuesta — G40): Bolts de limpieza (F-11/F-39 Oracle, F-03 export) y reducción de alcance del motor (F-26 sin VB), documentado en las notas de migración de la 2.0.

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta (draft del run ARUN-261015-0022) | agent:avenga-devflow |
| 2026-10-15 | Status → answered (opción B: las tres features se deprecan); propagada a REV-001 §7 y §6 | human:eugenioserrano |
