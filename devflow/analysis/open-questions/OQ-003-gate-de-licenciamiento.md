---
id: "OQ-003"
title: "¿Se preserva el gate de licenciamiento por PublicKeyToken en la 2.0?"
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
tags: ["migration", "licensing", "strong-name", "2.0"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta del análisis de migración 1.4 → 2.0 (REV-001, F-10).
  La respuesta produce: un ADR (licenciamiento/identidad de ensamblados). Nunca solo (G40).
-->

# OQ-003 — ¿Se preserva el gate de licenciamiento por PublicKeyToken en la 2.0?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿El chequeo de licencia por `PublicKeyToken` es una funcionalidad de negocio vigente que la 2.0 debe preservar (con un equivalente moderno), o se retira?
> - **Opciones:** A — retirar · B — rediseñar portable · C — dejar como está
> - **Recomendado:** B si el licenciamiento sigue siendo negocio; si no lo es, A. (C no: hoy el gate se saltea solo en hosts modernos.)

## 1. Pregunta

¿El gate de licenciamiento basado en `PublicKeyToken` (`BaseGateway.CheckForToken`, contraparte en el código generado) es un requisito de negocio de la 2.0, o se retira del diseño?

## 2. Contexto

### 2.1 De qué se trata

En `Cooperator.Framework.Data/BaseGateway.cs` (líneas 94-155, REV-001 F-10), cada Gateway compara el `PublicKeyToken` del **assembly de entrada** contra un token embebido en el código generado por el Modeler; si no coincide, lanza `InsufficientSecurityRightsException`. Detalles: un contador estático corta el chequeo tras 200 llamadas, y si `Assembly.GetEntryAssembly()` es `null` (IIS, test runners, hosts con carga dinámica) el chequeo se **saltea silenciosamente**. La firma de ensamblados es requisito del mecanismo (los `.snk` existen; `Utility` quedó sin firmar — F-31).

### 2.2 La pregunta en detalle

- **Por qué se pregunta ahora:** la 2.0 cambiará de runtime y de modelo de firmas. `Assembly.GetEntryAssembly()` se comporta distinto en hosts modernos (null en IIS, single-file, runners) — el gate actual daría **falsos positivos y falsos negativos**.
- **Qué está en juego:** si el licenciamiento es parte del producto (protección comercial), hay que **rediseñarlo** como mecanismo portable (p. ej. licencia firmada con clave asimétrica, almacenable offline); si no lo es, mantenerlo es riesgo y fricción sin valor.
- **Lo que no sabemos:** si hay licencias/clientes activos hoy y cuál es su valor de negocio; el código no lo documenta. (Esta pregunta es para el dueño del producto.)

### 2.3 Un ejemplo

Un cliente legítimo corre su app en un host donde `GetEntryAssembly()` es `null`: el gate no se evalúa y "pasa" sin licencia (falso negativo). Otro cliente empaqueta su app como single-file: la identidad del ensamblado de entrada cambia y podría quedar bloqueado siendo legítimo (falso positivo).

## 3. Opciones

| # | Opción | Qué significa | Qué cambia (en `targets`) | Riesgo / costo |
|---|--------|---------------|-----------------------------|----------------|
| A | **Retirar** | Se elimina el chequeo y los tokens embebidos del código generado | Simplifica Data y el codegen | Pérdida de la "protección" si fuera negocio real |
| B | **Rediseñar portable** | Mecanismo nuevo (licencia firmada offline / clave pública en el binario), independiente de `EntryAssembly` | ADR de licenciamiento + Bolt | Trabajo de diseño; requiere decidir el modelo comercial |
| C | **Dejar como está** | Mantener el chequeo por token tal cual | — | Gate inefectivo/errático en hosts modernos; deuda de seguridad |

## 4. Recomendación

- **Opción recomendada:** B — **si el licenciamiento sigue siendo un requisito de producto**; de lo contrario, A. C solo como parche temporal documentado.
- **Por qué:** un gate que se saltea cuando `EntryAssembly` es null no protege nada en los hosts que la 2.0 quiere soportar; hay que decidirlo como negocio antes de escribir código.
- **Confianza:** media (depende de información que solo el dueño del producto tiene)
- **Qué la cambiaría:** que el licenciamiento nunca haya sido un mecanismo comercial real → A directo.

## 5. Hipótesis de trabajo

> Asumiendo option B (mecanismo de licencia moderno pendiente de diseño); si es incorrecta, Data va directo a A y el ADR de empaquetado pierde el requisito de firma.

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | agent:avenga-devflow | Abierta desde REV-001 (run ARUN-261015-0022, F-10) |
| 2026-10-15 | human:eugenioserrano | Respondida en sesión interactiva: opción A (retirar). El token era detección de cambio de DLL, no licenciamiento comercial. |

## 7. Resolución

- **Decisión:** **Opción A — retirar el gate** en la 2.0. Origen del mecanismo (contexto declarado por el dueño): se agregó como **detección de manipulación** («por las dudas me cambiaran una dll»); **no** es licenciamiento comercial y no hay clientes licenciados.
- **Respuesta vs recomendación:** seguida (A), con la motivación original documentada.
- **Nota de seguridad:** el retiro no deja el binario sin identidad — la **firma de los ensamblados** (`snk`, ver F-31/F-45) se mantiene como identidad/firma, y la integridad de despliegue (si alguna vez importa) se cubre en la capa de empaquetado (Authenticode/NuGet signing), no en código del framework.
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva.
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas»). Produce (propuesta — G40): ADR de empaquetado/firma (§6 ítems 1/11 — incluye la decisión de retiro) + Bolt de limpieza (quitar `CheckForToken`/`GetAssemblyToken` y la contraparte embebida en el código generado).

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta (draft del run ARUN-261015-0022) | agent:avenga-devflow |
| 2026-10-15 | Status → answered (opción A: retirar el gate); propagada a REV-001 §7 | human:eugenioserrano |
