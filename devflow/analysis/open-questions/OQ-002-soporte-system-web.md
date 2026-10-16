---
id: "OQ-002"
title: "¿La 2.0 corta la dependencia de System.Web o preserva apps web .NET Framework?"
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
tags: ["migration", "netstandard2.1", "system-web", "caching"]
revisit_on: ""
closed_on: "2026-10-15"
closed_by: "human:eugenioserrano"
---

<!--
  Pregunta abierta del análisis de migración 1.4 → 2.0 (REV-001, F-07/F-08/F-22).
  La respuesta produce: un ADR (caché/configuración) y Bolts. Nunca se crean solos (G40).
-->

# OQ-002 — ¿La 2.0 corta la dependencia de System.Web o preserva apps web .NET Framework?

> **Resumen para quien valida** — leé esto primero.
> - **Pregunta:** ¿La 2.0 corta `System.Web` (caché vía `HttpContext.Cache`) y moderniza a una abstracción portable, o necesita seguir soportando apps web .NET Framework existentes?
> - **Opciones:** A — cortar y abstraer · B — preservar compatibilidad web · C — abstracción + adaptador web opcional
> - **Recomendado:** C — corta el acoplamiento del core y deja el adaptador web como paquete aparte, si existen consumidores IIS.

## 1. Pregunta

¿La 2.0 puede cortar la dependencia de `System.Web` (caché por `HttpContext.Cache` en `Data` y el tipo en la firma de `SmtpMail`) o debe preservar compatibilidad con aplicaciones web .NET Framework que hoy consumen el framework?

## 2. Contexto

### 2.1 De qué se trata

- `Cooperator.Framework.Data` cachea `GetAll`/`GetAllDictionary` contra `System.Web.HttpContext.Cache` en **cuatro archivos centrales** (`BaseGateway`, `BaseLoader`, `SQLHelper`, `SqlHelperParameterCache`). Fuera de una request IIS el caché de colecciones **ocurre igual en memoria a nivel de instancia** (sin expiración); en web es compartido con expiración de 1 h — con `System.Web` de por medio (REV-001 F-07/F-08; contexto corregido el 2026-10-15, ver Historial).
- `Cooperator.Framework.Library` expone `System.Web.HttpContext` en la firma de un método muerto (`SmtpMail.SendMailViaCDONTS`, siempre lanza `NotImplementedException`) — hoy arrastra la referencia para todos los consumidores (REV-001 F-22).

### 2.2 La pregunta en detalle

- **Por qué se pregunta ahora:** `System.Web` no existe en `netstandard2.1`/moderno — es el bloqueante más grande de `Data`. Sin decidir esto no se puede cerrar el ADR de caché/configuración ni migrar `Data`.
- **Lo que pesa:** ¿hay aplicaciones web (IIS, .NET Framework 4.x) que seguirán consumiendo el framework en paralelo a la 2.0? Si las hay, cortar sin más rompe su cacheo; si no las hay, mantener `System.Web` es deuda pura.
- **Nota:** la decisión no afecta al Modeler (desktop), solo a las librerías y a los consumidores web.

### 2.3 Un ejemplo

Una app ASP.NET 4.8 que usa `BaseGateway.GetAll()` hoy obtiene caché de 1 hora por request web. Si la 2.0 reemplaza eso por `ICacheProvider` con implementación in-memory, esa app (si pasa a la 2.0) cambia de comportamiento de caché; si se queda en 1.4, no cambia nada.

## 3. Opciones

| # | Opción | Qué significa | Qué cambia (en `targets`) | Riesgo / costo |
|---|--------|---------------|-----------------------------|----------------|
| A | **Cortar y abstraer** | `ICacheProvider` portable (`Microsoft.Extensions.Caching.Memory`); adiós `System.Web` en el core | `Data`/`Library` portables; los consumidores web actuales deben adaptarse | Ruptura para apps IIS que usen la 2.0 sin adaptador |
| B | **Preservar compatibilidad** | Mantener `System.Web` (paquete `-windows-web` o shims) para no romper consumidores | El core queda atado a Windows/web | Mata la portabilidad del core |
| C | **Abstracción + adaptador web opcional** | Core portable con `ICacheProvider`; un paquete aparte implementa el adaptador web/IIS | Core portable **y** camino de compatibilidad para IIS | Un paquete extra y documentar la elección |

## 4. Recomendación

- **Opción recomendada:** C
- **Por qué:** resuelve las dos realidades (portabilidad del core + consumidores existentes) sin arrastrar `System.Web` a todo. Si se confirma que **no hay** ninguna app web consumidora, la recomendación pasa a A (más simple).
- **Confianza:** media
- **Qué la cambiaría:** evidencia de que ninguna aplicación web consume el framework (→ A) o de que varias no pueden migrar (→ más peso a B).

## 5. Hipótesis de trabajo

> Asumiendo option C (core con `ICacheProvider` + adaptador web como paquete aparte); si es incorrecta, el ADR de caché cambia de forma y la migración de `Data` puede bloquearse.

## 6. Bitácora de investigación (append-only)

| Fecha | Autor | Nota |
|-------|-------|------|
| 2026-10-15 | agent:avenga-devflow | Abierta desde REV-001 (run ARUN-261015-0022, F-07/F-08/F-22) |
| 2026-10-15 | human:eugenioserrano | Respondida en sesión interactiva: `ICacheProvider` con default en memoria, reemplazable (Redis, etc.) — variante C. Contexto: mayoría de apps WinForms; algunas web. |

## 7. Resolución

- **Decisión:** **Opción C — variante «default en memoria»**: `ICacheProvider` con una **implementación en memoria por defecto** usada por Cooperator sin configuración (todos los hosts, incluidos escritorio) y **reemplazable por configuración** (Redis u otro) detrás de la misma interfaz. `System.Web` sale del core.
- **Respuesta vs recomendación:** variante de C ajustada por el dueño (el provider en memoria por defecto cubre también a las apps web; no se necesita un adaptador web dedicado como entregable).
- **Contexto declarado:** la mayoría de las aplicaciones consumidoras son WinForms; algunas web. El caché de escritorio actual es en memoria por instancia (corrección factual registrada en REV-001 F-07/F-08).
- **Validada por:** human:eugenioserrano — 2026-10-15, respuesta directa en sesión interactiva.
- **Propagada a:** [REV-001 §7](../../reviews/REV-001-analisis-migracion-cooperator.md) («Preguntas abiertas · Resueltas»). Produce (propuesta — G40): ADR «caché y configuración de la 2.0» (TTL por defecto, invalidación, desactivable, mecanismo de reemplazo) + Bolts de F-07/F-08/F-13/F-22.

## 8. Historial

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-10-15 | Abierta (draft del run ARUN-261015-0022) | agent:avenga-devflow |
| 2026-10-15 | Status → answered (variante C: default en memoria, intercambiable); propagada a REV-001 §7 | human:eugenioserrano |
