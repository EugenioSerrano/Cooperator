# Discovery

## Propósito

Esta carpeta almacena los documentos de **descubrimiento (Discovery)** del proyecto.
Un documento de Discovery captura los hallazgos, investigaciones y conocimiento
obtenido durante la exploración de sistemas existentes, tecnologías, dominios de
negocio, código legado, bases de datos, integraciones, o cualquier otro aspecto
que requiera ser comprendido antes de poder diseñar e implementar la solución.

El Discovery es la fase de **investigación exploratoria**. Es el equivalente a
"hacer arqueología" del sistema o dominio: se excava, se documenta lo que se
encuentra, se mapean relaciones, se identifican brechas y se genera el
conocimiento base que alimentará las decisiones de arquitectura (ADRs) y las
especificaciones de implementación (Specs).

---

## ¿Qué tipo de documentos van aquí?

Cada archivo en esta carpeta es un documento de Discovery que captura los
hallazgos de una investigación específica. Ejemplos típicos incluyen:

- Análisis de código fuente legado (estructura, procedimientos, lógica de
  negocio, dependencias entre módulos).
- Análisis de esquemas de bases de datos (tablas, relaciones, tipos de datos,
  índices, vistas, stored procedures).
- Mapeo de entidades del sistema legado al modelo de dominio del sistema nuevo.
- Análisis de brechas (Gap Analysis) entre el sistema legado y el nuevo.
- Extracción y documentación de reglas de negocio embebidas en código legado.
- Historias de usuario derivadas del análisis del sistema existente.
- Documentación de datos de ejemplo, sets de datos de prueba y su setup.
- Inventario de integraciones del sistema existente (APIs, archivos, sistemas).
- Análisis de configuraciones del sistema legado.
- Documentación de hallazgos de entrevistas con usuarios expertos.
- Mapeo de flujos de usuario del sistema actual.
- Análisis de reportes existentes (estructura, campos, filtros, lógica).
- Documentación de datos de referencia (catálogos, lookups, enumeraciones).
- **Análisis de hardware**: pinout, clocks, componentes, señales, niveles lógicos.
- **Especificaciones de dispositivos externos**: datasheets resumidos, análisis eléctricos.

---

## Convención de nombres

```
DISC-NNN-descripcion-breve-en-kebab-case.md
```

Donde:
- `DISC` — Prefijo fijo que identifica el tipo de documento.
- `NNN` — Número secuencial de 3 dígitos (001, 002, 003...).
- `descripcion-breve` — Resumen del tema en kebab-case.
- `.md` — Extensión Markdown.

---

## Estructura recomendada de un documento Discovery

1. **Título y metadatos** — Número, nombre descriptivo, fecha, autor, fuentes consultadas.
2. **Resumen ejecutivo** — Hallazgo principal o conclusión más relevante.
3. **Inventario / Mapeo** — Listado detallado de lo encontrado: tablas, campos, señales, pines, etc.
4. **Hallazgos detallados** — Descripción en profundidad con pseudocódigo o fragmentos si es necesario.
5. **Brechas y pendientes** — Lo que NO se encontró o no se pudo determinar.
6. **Recomendaciones** — Próximos pasos, ADRs necesarios, impacto estimado.
7. **Referencias** — Links a datasheets, documentos internos, fuentes externas.

### Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas, gráficos y cualquier otro elemento visual (no ASCII art ni imágenes embebidas):

```markdown
` ` `mermaid
flowchart TB
    A["Componente A"] --> B["Componente B"]
` ` `
```

---

## Relación con otras carpetas

| Carpeta | Relación con Discovery |
|---------|----------------------|
| `adrs/` | Los Discovery alimentan las decisiones de arquitectura |
| `memory/` | Los Discovery informan la implementación documentada en Memory |
| `spec/` | Los Discovery se convierten en especificaciones técnicas |
| `reviews/` | Las revisiones pueden generar nuevos Discovery |

---

---

## Ciclo de vida de un DISC

| Estado | Significado |
|--------|-------------|
| **vigente** | Investigación vigente. Sus hallazgos son relevantes para el desarrollo actual. |
| **deprecado** | La investigación ya no es relevante (sistema legacy retirado, tecnología descartada). Se mantiene como referencia histórica. |

El INDEX.md refleja el estado: ✅ Vigentes (vigente) / ⛔ Deprecados (deprecado).

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado completo de documentos vigentes.
