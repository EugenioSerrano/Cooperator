# Riesgos (Risk Register)

## Propósito

Esta carpeta contiene el **registro de riesgos** del proyecto: amenazas
identificadas que podrían impactar el éxito, la calidad o los plazos del
desarrollo, junto con su evaluación, plan de mitigación y estado actual.

El Risk Register es un artefacto vivo que se alimenta desde múltiples fuentes:
Discovery (riesgos técnicos del dominio), Reviews (riesgos encontrados en
auditorías), y la propia experiencia del equipo.

---

## ¿Qué documentos van aquí?

- Riesgos técnicos identificados durante Inception o Discovery.
- Riesgos de integración con terceros.
- Riesgos de rendimiento, seguridad o escalabilidad.
- Dependencias externas con fecha límite o incertidumbre.
- Riesgos de equipo o proceso que impactan la entrega.

---

## Convención de nombres

```
RISK-NNN-descripcion-breve-en-kebab-case.md
```

---

## Estructura recomendada

1. **Título y metadatos** — ID, nombre, fecha, autor, categoría.
2. **Descripción** — Qué podría pasar y en qué contexto.
3. **Probabilidad** — Alta / Media / Baja.
4. **Impacto** — Crítico / Alto / Medio / Bajo.
5. **Nivel de riesgo** — Combinación probabilidad × impacto.
6. **Plan de mitigación** — Acciones para reducir probabilidad o impacto.
7. **Plan de contingencia** — Qué hacer si el riesgo se materializa.
8. **Estado** — Abierto / Mitigado / Materializado / Cerrado.
9. **Relaciones** — DISCs, ADRs, REVs o BUGs relacionados.

### Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas.

---

---

## Ciclo de vida de un RISK

| Estado | Significado |
|--------|-------------|
| **abierto** | Riesgo identificado, sin mitigación aplicada. |
| **mitigado** | Plan de mitigación implementado. El riesgo sigue monitoreado. |
| **materializado** | El riesgo ocurrió. Se activó el plan de contingencia. |
| **cerrado** | Riesgo dejó de ser relevante (contexto cambió, mitigación eliminó la amenaza). |

El INDEX.md refleja el estado: ✅ Vigentes (abierto, mitigado) / ⛔ Cerrados (materializado, cerrado).

---

## Resumen de riesgos

Para el listado de riesgos vigentes, ver **[INDEX.md](INDEX.md)**. Cada riesgo tiene su documento individual (`RISK-NNN-*.md`) con el detalle completo de probabilidad, impacto, mitigación y contingencia.

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado completo.
