---
id: "OQ-001"
title: "¿Particionar Framework.Utility en modelo portable + tooling Windows?"
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
tags: ["migration", "netstandard2.1", "modeling", "coop-format"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta del análisis de migración 1.4 → 2.0 (REV-001, F-27/F-28/F-32/F-41).
  La respuesta produce: un ADR (layout de la 2.0) y Bolts. Nunca se crean solos (G40).
-->

# OQ-001 — ¿Particionar Framework.Utility en modelo portable + tooling Windows?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿La 2.0 parte `Cooperator.Framework.Utility` en un paquete portable (`Cooperator.Framework.Modeling`, netstandard2.1) con el modelo del `.coop` + un resto Windows (editors/Forms), o mantiene `Utility` entero sobre Windows?
> - **Opciones:** A — particionar (modelo portable) · B — `Utility` entero en Windows · C — otra partición (3 paquetes)
> - **Recomendado:** A — porque es el desbloqueante técnico del formato `.coop` JSON y de la edición por agentes.

## 1. Pregunta

¿La 2.0 particiona `Cooperator.Framework.Utility` en un paquete portable (`Cooperator.Framework.Modeling`, netstandard2.1) que contenga el modelo del archivo `.coop` y el reverse-engineering de esquema, dejando los editors/Forms de diseño en el tooling Windows?

## 2. Contexto

### 2.1 De qué se trata

`Cooperator.Framework.Utility` (proyecto de ~19 archivos `.cs`, .NET Framework 4.8) contiene tres cosas mezcladas:

1. **El modelo del Modeler** (`DBReverseHelper/DomainTree.cs`): las clases que representan entidades, propiedades y queries — es lo que se serializa en el archivo `.coop`. Hoy el modelo lleva **atributos de diseñador** (`[Editor(typeof(…UITypeEditor))]`, `DesignerSerializationVisibility`) y depende de `System.Drawing.Design` y `System.Windows.Forms.Design`.
2. **Los Forms/editors de diseño** (`StringEditorForm`, `ToStringInfoEditorForm`, `PropertyEditors/*`): UI WinForms embebida en la librería.
3. **El reverse-engineering de esquema** (`DBReverse.cs`, `DBReverseHelper.cs`): POCOs y interfaces ya portables (REV-001 F-32, Compliant).

Evidencia completa en REV-001 (F-27 “el modelo acoplado a diseñadores”, F-28 “diálogos WinForms en la librería”, F-41 “el `.coop` es un blob binario del modelo”).

### 2.2 La pregunta en detalle

- **Por qué se pregunta ahora:** el objetivo declarado de la 2.0 incluye que el `.coop` se guarde en un formato texto editable por agentes (JSON). Eso **no es alcanzable** mientras el modelo arrastre atributos de UI y viva en un ensamblado que no puede ser `netstandard2.1`.
- **Qué depende de la respuesta:** el ADR de layout de la 2.0, el Bolt del formato `.coop` JSON, y la portabilidad de las librerías (hoy `Utility` corta la portabilidad en 5 proyectos).
- **Qué está en juego:** cuántos paquetes NuGet tendrá la 2.0, si `Cooperator.Framework.Utility` cambia de contenido/compatibilidad, y cuánto trabajo de refactor implica (mover clases, quitar atributos del modelo, conservar funcionalidad de UI en el Modeler).

### 2.3 Un ejemplo

Un agente (o un dev en CI/Linux) quiere leer y editar el modelo de un proyecto: hoy tendría que cargar un ensamblado Windows con atributos de diseñador WinForms — inviable. Con el modelo en un paquete portable, el archivo `.coop` (JSON) se edita con cualquier herramienta y el Modeler lo vuelve a abrir.

## 3. Opciones

| # | Opción | Qué significa | Qué cambia (en `targets`) | Riesgo / costo |
|---|--------|---------------|-----------------------------|----------------|
| A | **Particionar ahora** | Nuevo `Cooperator.Framework.Modeling` (netstandard2.1) con el modelo POCO + DBReverse + interfaces; editors/Forms/CodeDOM quedan en el Modeler o un paquete `-windows` | Habilita F-41/F-42 (`.coop` JSON + flujo agente); desbloquea la portabilidad | Refactor medio; coordinar con el ADR de layout |
| B | `Utility` entero en Windows | El modelo queda donde está; el `.coop` JSON se hace igual pero atado a Windows | El flujo “agente edita y mergea el modelo” queda limitado a hosts Windows; las librerías portables no incluyen el modelo | Bloquea el objetivo de la 2.0 declarado por el negocio |
| C | Otra partición (p. ej. 3 paquetes: Modeling / Reverse / Tooling) | Más granularidad | Igual que A, más fragmentación | Más paquetes de los necesarios si no hay consumidores separados |

## 4. Recomendación

- **Opción recomendada:** A
- **Por qué:** es el prerequisito técnico de F-41 (formato `.coop` JSON) y del flujo de edición por agentes que pediste; y `DBReverse.cs` ya es portable, así que el costo es de reubicación, no de reescritura.
- **Confianza:** alta
- **Qué la cambiaría:** que no exista ninguna necesidad de usar el modelo fuera del Modeler Windows y que el formato binario se acepte como deuda permanente (poco probable dado el pedido).

## 5. Hipótesis de trabajo

> Asumiendo option A (existe `Cooperator.Framework.Modeling` netstandard2.1 con el modelo sin atributos de UI); si es incorrecta, el formato `.coop` JSON quedaría limitado a Windows y el Bolt de F-41 cambiaría de alcance.

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | agent:avenga-devflow | Abierta desde REV-001 (run ARUN-261015-0022, F-27/F-28/F-41) |
| 2026-10-15 | human:eugenioserrano | Respondida en sesión interactiva: opción A (particionar en `Modeling` portable + tooling). |

## 7. Resolución

- **Decisión:** **Opción A** — nuevo paquete portable `Cooperator.Framework.Modeling` (netstandard2.1) con el modelo del archivo de modelo y el reverse-engineering de esquema; los editors/Forms de diseño se mueven al Modeler (o a un paquete `-windows`).
- **Respuesta vs recomendación:** seguida (A).
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva (dueño del producto; validador designado).
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas»). Produce (propuesta — se crea cuando se pida, G40): ADR «layout de la 2.0 / partición de `Utility`» + el Bolt de F-41 (formato `.coop` JSON).

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta (draft del run ARUN-261015-0022) | agent:avenga-devflow |
| 2026-10-15 | Status → answered (opción A); propagada a REV-001 §7 | human:eugenioserrano |
