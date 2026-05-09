---
id: "REV-NNN"
titulo: ""
fecha: "YYYY-MM-DD"
autor: "" # git config user.name
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
alcance: "" # Módulos, proyectos o sistemas revisados
metodologia: "" # Inspección estática, revisión contra ADRs, análisis grep, etc.
artefactos_revisados: [] # archivos, módulos o sistemas evaluados
adrs_verificados: [] # ADRs contra los que se revisa
specs_verificadas: [] # SPECs implementadas que se revisan
---

# REV-NNN — [Título descriptivo]

| Campo          | Valor |
|----------------|-------|
| **Alcance**    | [Módulos/proyectos revisados] |
| **Metodología**| [Inspección estática, revisión contra ADRs, etc.] |
| **Criterios**  | [ADRs y estándares contra los que se revisa] |

---

## 1. Propósito

[Por qué existe esta revisión. Qué se busca verificar.]

---

## 2. Artefactos revisados

| Artefacto | Archivos | Notas |
|-----------|----------|-------|
|           |          |       |

---

## 3. Leyenda de severidad

| Categoría           | Significado |
|---------------------|-------------|
| ✅ Cumple            | Implementado correctamente según lo definido |
| ⚠️ Desvío documentado | Diferencia justificada y registrada en MEM |
| 🔶 Gap menor         | Inconsistencia sin impacto funcional, reduce calidad |
| 🔴 Gap mayor         | Problema que puede causar errores en runtime o seguridad |

---

## 4. Hallazgos

### 4.1 — [Dominio / Categoría]

#### H-01 🔴 [Título del hallazgo]

**Ubicación:** `ruta/archivo.ext` línea N

**Actual:** [Qué dice/hace el código hoy]

**Esperado:** [Qué debería decir/hacer según ADR/estándar]

**Impacto:** [Consecuencias de no corregir]

**Recomendación:** [Fix propuesto]

---

#### H-02 🔶 [Título del hallazgo]

**Ubicación:** `ruta/archivo.ext` línea N

**Actual:** [...]

**Esperado:** [...]

**Impacto:** [...]

**Recomendación:** [...]

---

### 4.2 — [Otro dominio / Categoría]

#### H-03 ✅ [Título — qué está bien]

[Breve descripción de lo que cumple correctamente.]

---

## 5. Resumen

[Síntesis del estado general en 2-3 frases.]

---

## 6. Plan de acción

| # | Hallazgo | Severidad | Acción | Destino |
|---|----------|-----------|--------|---------|
| 1 | H-01     | 🔴        |        | SPEC / BUG / ADR |
| 2 | H-02     | 🔶        |        | SPEC |

---

## 7. Conclusiones

[Estado general y recomendación: ¿se puede seguir adelante? ¿Se necesita
otro ciclo de revisión?]
