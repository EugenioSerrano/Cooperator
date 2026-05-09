---
id: "US-NNN.BOLT-NNN"
titulo: ""
fecha: "YYYY-MM-DD"
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
us: "" # US padre (ej. US-001)
capa: "" # Backend | Frontend | Infra | Full-stack
timebox: "" # Estimación en AI-time (2h a 1 día)
---

# US-NNN.BOLT-NNN — [Título descriptivo]

| Campo       | Valor |
|-------------|-------|
| **US**      | [US-NNN](../US-NNN-titulo.md) |
| **Timebox** | [2h-1d AI-time] |
| **Demo**    | [Cómo verificar: comando curl, captura, test específico] |

---

## Descripción

[Qué se construye en este Bolt. Una frase clara.]

---

## Criterios de aceptación

- **Given** [contexto], **When** [acción], **Then** [resultado esperado].
- **Given** [contexto], **When** [acción], **Then** [resultado esperado].

---

## Tareas

1. [Tarea concreta 1]
2. [Tarea concreta 2]
3. [Tarea concreta 3]

---

## Definition of Ready (DoR)

> La DoR responde: **¿Qué necesito tener ANTES de empezar a trabajar en este Bolt?**
> Si algún ítem no se cumple, el Bolt no está listo para iniciar.

- [ ] [Requisito previo 1 — ej. ADR-005 aprobado y disponible]
- [ ] [Requisito previo 2 — ej. Endpoint de autenticación (BOLT-001) deployado en dev]
- [ ] [Requisito previo 3 — ej. Credenciales/API keys configuradas en .env]
- [ ] [Requisito previo 4 — ej. Schema de base de datos migrado]

**Ejemplos de ítems DoR típicos:**
- ADRs relevantes aprobados.
- Dependencias externas (APIs, servicios, paquetes) accesibles.
- Bolts previos completados y verificados.
- Acceso a entornos necesarios (dev, staging).
- Datos de prueba disponibles.
- Diseño/mockup aprobado (si aplica UI).

---

## Definition of Done (DoD)

> La DoD responde: **¿Cómo sé que este Bolt está TERMINADO?**
> Todos los ítems deben cumplirse para marcar el Bolt como completo.

- [ ] [Criterio 1 — ej. Código compila sin errores ni warnings]
- [ ] [Criterio 2 — ej. Tests unitarios escritos y pasando (cobertura ≥ 80%)]
- [ ] [Criterio 3 — ej. Endpoint responde 200 con payload esperado]
- [ ] [Criterio 4 — ej. Code review aprobado (o self-review documentado)]
- [ ] [Criterio 5 — ej. Sin secrets hardcodeados]

**Ejemplos de ítems DoD típicos:**
- Compila/buildea sin errores.
- Tests unitarios/integración escritos y pasando.
- Linter/formatter sin violaciones.
- Funcionalidad verificable con el comando/paso de la sección Demo.
- Sin regresiones en tests existentes.
- Documentación actualizada (si aplica).
- PR creado / merge realizado.

---

## Dependencias

- **ADRs:** [links a ADRs que aplican]
- **Bolts previos:** [Bolts que deben completarse antes]
- **Externos:** [APIs, paquetes, servicios necesarios]

---

## ¿Este Bolt necesita dividirse?

> Regla general: **2h a 1 día de AI-time.** Si estimás más de 1 día, considerá dividir.

Usá estas heurísticas para decidir:

| Heurística | ¿Dividir? |
|-----------|-----------|
| **Archivos nuevos** | Si tocás más de 5-7 archivos nuevos → probablemente necesite split |
| **Capas** | Si el Bolt abarca domain + application + infrastructure + API → separar por capa |
| **Dependencias** | Si el Bolt B necesita que el Bolt A esté terminado → son dos Bolts distintos |
| **Testeabilidad** | Si una parte no se puede testear sin la otra → mantener juntas; si son independientes → separar |
| **Demo criterion** | Si tenés más de un criterio de demo → posible split (un Bolt = una demo) |
| **Riesgo** | Si una parte es de alto riesgo y otra es trivial → separar para no bloquear lo simple |

**Split por capas (ejemplo típico):**
- Bolt 1: Domain (entidades, value objects, interfaces de repositorio)
- Bolt 2: Application (use cases, servicios)
- Bolt 3: Infrastructure (adapters, controllers, configuración)

**Split por funcionalidad (ejemplo típico):**
- Bolt 1: Flujo principal (happy path)
- Bolt 2: Validaciones y edge cases
- Bolt 3: Integración con servicio externo

Si después de aplicar estas heurísticas el Bolt sigue pareciendo grande, **dividilo**. Es mejor tener Bolts chicos que fallen rápido que uno grande que nunca termina.

---

## Notas técnicas

[Detalles de implementación, pseudocódigo, patrones a seguir, warnings
conocidos. Esta sección es opcional para bolts simples.]
