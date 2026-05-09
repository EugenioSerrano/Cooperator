# Interviews (Entrevistas con Stakeholders)

## Propósito

Esta carpeta almacena las **transcripciones de entrevistas** con stakeholders,
usuarios, expertos de dominio y cualquier persona relevante para el análisis
del negocio.

Las entrevistas son la **fuente primaria** de todo el flujo AI-Native-SDLC:
a partir de ellas, la IA transcribe y propone User Stories, criterios de
aceptación (ACs), riesgos, NFRs, entidades de dominio y procesos de negocio.
El equipo luego valida y ajusta.

---

## ¿Qué documentos van aquí?

- Transcripciones completas de entrevistas (audio → texto).
- Notas de reuniones con stakeholders.
- Resúmenes ejecutivos de sesiones de relevamiento.
- Minutas de workshops de descubrimiento.
- Cualquier registro textual de conversaciones con personas del negocio.

---

## Buenas prácticas para entrevistas

- **Agenda clara** — Definir objetivos y preguntas antes de la sesión.
- **Consentimiento de grabación** — Siempre pedir permiso antes de grabar.
- **Preguntas de clarificación** — No asumir, preguntar "¿qué pasa si...?".
- **Ejemplos concretos** — Pedir casos reales, no solo descripciones abstractas.
- **Métricas de éxito** — Preguntar cómo se mide el éxito del proceso/feature.
- **Acuerdos de privacidad** — Respetar información sensible o confidencial.

---

## Convención de nombres

La organización es libre. Algunas opciones:

```
interviews/
├── 2026-05-01-stakeholder-nombre-tema.md
├── 2026-05-03-workshop-modulo-pagos.md
└── sprint-01/
    ├── entrevista-usuario-operador.md
    └── entrevista-gerente-comercial.md
```

Se recomienda incluir **fecha** y **participante/tema** en el nombre del archivo
para facilitar la búsqueda.

---

## Estructura sugerida de una transcripción

```markdown
---
fecha: "YYYY-MM-DD"
participantes: ["nombre1", "nombre2"]
duracion: "45 min"
tema: "Descripción breve del tema tratado"
tags: []
---

# Entrevista — [Tema]

## Contexto
[Por qué se hizo esta entrevista, qué se buscaba entender.]

## Transcripción
[Contenido textual de la entrevista. Puede ser literal o resumido.]

## Puntos clave extraídos
- [Punto 1]
- [Punto 2]

## Entidades mencionadas
- [Entidad 1] — [descripción breve]
- [Entidad 2] — [descripción breve]

## Procesos mencionados
- [Proceso 1] — [descripción breve]

## Pendientes / Follow-ups
- [ ] [Pregunta o tema que quedó abierto]
```

---

## Relación con el flujo

```
Entrevistas → domain-model (entidades) + BPMN (procesos) → User Stories → Specs → Implementación
```

Las transcripciones alimentan directamente:
- `analysis/domain-model/` — Extracción de entidades, propiedades y relaciones.
- `analysis/bpmn/` — Identificación de procesos de negocio.
- `functional/` — Derivación de User Stories y criterios de aceptación.

---

---

## Ciclo de vida de una entrevista

Una entrevista transcripta es un **documento histórico**. No cambia — lo que se dijo, se dijo. Si se hace una entrevista de seguimiento, se crea un documento nuevo.

Esta carpeta no tiene estados activo/deprecado. El INDEX.md lista todas las entrevistas realizadas.

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado de entrevistas realizadas.
