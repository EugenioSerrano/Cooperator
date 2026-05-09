# Onboarding — Avenga AI-Native-SDLC Dev Flow

## Orden de lectura recomendado (30-45 min)

Si sos nuevo en el proyecto, leé en este orden:

1. **`avenga-sdlc/Avenga AI-Native-SDLC.md`** — La metodología base: V-Bounce, Bolts, gates, métricas. Es el "por qué" de todo.
2. **`README.md`** (este folder) — Estructura de carpetas, flujos, nomenclaturas, reglas de oro.
3. **`input/`** — Material crudo del proyecto. Entender qué se pidió originalmente.
4. **`analysis/README.md`** — Cómo se analiza el dominio: entrevistas, domain-model, BPMN.
5. **`discovery/INDEX.md`** + **`functional/INDEX.md`** — Qué se investigó y qué se definió construir.
6. **`adrs/INDEX.md`** — Decisiones arquitectónicas vigentes (si las hay).

Después, explorá lo que necesites según tu rol (ver abajo).

---

## Mapa de roles

| Rol | Lee esto | Escribe en |
|-----|----------|------------|
| **Stakeholder / Product Owner** | `functional/INDEX.md`, `analysis/bpmn/` | No escribe — valida User Stories y ACs |
| **Analista funcional** | `analysis/interviews/`, `analysis/domain-model/`, `discovery/` | `functional/` (US + Bolts), `analysis/glossary/` |
| **Arquitecto** | `functional/`, `discovery/`, `analysis/domain-model/` | `adrs/` (decisiones), `risks/` |
| **Desarrollador / Agente AI** | `functional/` (US + Bolt), `adrs/`, `spec/` de la tarea | `spec/` (SPEC), `memory/` (MEM), `src/` (código) |
| **QA / Revisor** | `adrs/`, `spec/`, `memory/` | `reviews/` (REV), `bugs/` (BUG) |

---

## Glosario de la metodología

| Término | Definición | Dónde se define |
|---------|-----------|-----------------|
| **V-Bounce** | Micro-ciclo: Input → AI genera → humano revisa → AI refina → aprueba → captura conocimiento | `avenga-sdlc/` |
| **Bolt** | Unidad de trabajo de 2h a 1 día (AI-time). Demo medible. | `functional/` como Bolt, `spec/` como SPEC |
| **Intent** | Objetivo de negocio de alto nivel. Agrupa Units. | `functional/` |
| **Unit** | Bloque cohesivo de valor (tipo Epic). Agrupa HUs. | `functional/` |
| **User Story (US)** | "Como [rol], quiero [acción], para [beneficio]" + ACs Given/When/Then | `functional/` |
| **DISC** | Documento de Discovery: investigación exploratoria | `discovery/DISC-NNN` |
| **ADR** | Architecture Decision Record: decisión técnica inmutable | `adrs/ADR-NNN` |
| **SPEC** | Especificación de implementación: plano de construcción | `spec/SPEC-YYMMDD-HHmm` |
| **REV** | Review formal: auditoría técnica, no modifica código | `reviews/REV-NNN` |
| **BUG** | Defecto confirmado con causa raíz. Se corrige con TDD. | `bugs/BUG-NNN` |
| **RISK** | Riesgo del proyecto con mitigación y contingencia | `risks/RISK-NNN` |
| **MEM** | Memoria: registro de lo implementado | `memory/MEM-YYMMDD-HHmm` |
| **DoR** | Definition of Ready: condiciones para empezar un Bolt | `functional/TEMPLATE-BOLT.md` |
| **DoD** | Definition of Done: condiciones para completar un Bolt | `functional/TEMPLATE-BOLT.md` |
| **PROC** | Proceso de negocio documentado en BPMN/Mermaid | `analysis/bpmn/PROC-NNN` |

---

## Preguntas frecuentes

**¿Creo un DISC o voy directo a FA?**
Si necesitás investigar antes de definir → DISC. Si ya sabés qué construir → FA.

**¿Un Bolt siempre genera una SPEC?**
Sí. Sin SPEC no se implementa (salvo que el usuario explícitamente pida excepción).

**¿Puedo editar un ADR?**
No. Los ADRs son readonly. Si cambia una decisión, creá un ADR nuevo.

**¿Dónde guardo docs de una API externa?**
En `other-docs/`, organizado por proveedor.

**¿Cómo sé qué documentos están vigentes?**
Cada carpeta con INDEX.md tiene secciones ✅ Vigentes y ⛔ Deprecados.

**¿Qué hago si encuentro un bug?**
Documentalo en `bugs/BUG-NNN.md`. Luego creá una SPEC de test (Skip/Rojo) y una SPEC de fix (Verde).
