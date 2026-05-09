---
id: "BUG-NNN"
titulo: ""
fecha: "YYYY-MM-DD"
autor: "" # git config user.name
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
severidad: "" # 🔴 crítica | 🔴 alta | 🟡 media | 🟢 baja
estado: "abierto" # abierto | en-progreso | resuelto
detectado_en: "" # REV-NNN | QA | producción | tests
artefactos_afectados: []
spec_test: "" # SPEC-YYMMDD-HHmm del test (fase 1)
spec_fix: "" # SPEC-YYMMDD-HHmm del fix (fase 2)
---

# BUG-NNN — [Título descriptivo]

| Campo          | Valor |
|----------------|-------|
| **Severidad**  | [🔴 Crítica / 🔴 Alta / 🟡 Media / 🟢 Baja] |
| **Detectado en** | [REV-NNN / QA / producción / tests] |
| **Artefactos** | [archivos/módulos afectados] |

## Resumen

[Descripción breve del defecto en 1-2 frases.]

---

## Condiciones para reproducir

[Pasos o escenarios que disparan el bug.]

1. ...
2. ...
3. Resultado: ...

---

## Causa raíz

[Análisis técnico del origen del defecto.]

---

## Impacto

[Consecuencias funcionales, de datos, contables, de UX.]

---

## Propuesta de corrección

[Fix propuesto con pseudocódigo o detalle técnico.]

---

## TDD — Estado del ciclo

| Fase | SPEC | Estado |
|------|------|--------|
| Fase 1: Test | SPEC-YYMMDD-HHmm | ⬜ Pendiente / ✅ Test en Skip |
| Fase 2: Fix  | SPEC-YYMMDD-HHmm | ⬜ Pendiente / ✅ Test en Verde |

---

## Relaciones

- **Detectado en:** [REV-NNN / QA / producción]
- **FAs relacionados:** [links]
- **ADRs relacionados:** [links]
