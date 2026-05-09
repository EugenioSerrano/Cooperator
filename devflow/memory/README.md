# Memory (Memoria del Proyecto)

## Propósito

Esta carpeta almacena la **memoria del proyecto**: documentos que capturan QUÉ se hizo,
CÓMO se hizo, QUÉ se decidió y QUÉ se aprendió durante la implementación.

Es el "diario de bitácora" técnico. No describe lo que SE DEBE hacer (→ Specs) ni lo que
SE DECIDIÓ a nivel arquitectónico (→ ADRs). Documenta lo que **efectivamente ocurrió**.

---

## ¿Qué documentos van aquí?

- Resumen de la unidad de trabajo ejecutada.
- Lista de archivos creados/modificados y su propósito.
- Decisiones de diseño tomadas durante la implementación.
- Bugs encontrados y corregidos.
- Deuda técnica generada o identificada.
- Resultado de tests y cobertura.
- Stubs o pendientes para futuros trabajos.
- Contexto práctico para retomar el trabajo sin arqueología de commits.

---

## Convención de nombres

```
MEM-YYMMDD-HHmm-descripcion-breve-del-trabajo.md
```

Donde:
- `MEM` — Prefijo fijo.
- `YYMMDD` — Fecha de creación (año 2 dígitos, mes, día).
- `HHmm` — Hora de creación (hora, minutos).
- `descripcion-breve` — Resumen del trabajo en kebab-case.
- `.md` — Extensión Markdown.

**Ejemplo:** `MEM-YYMMDD-HHmm-descripcion-breve-del-trabajo.md`

> ⚠️ **IMPORTANTE — Timestamp del sistema:** Los valores `YYMMDD` y `HHmm` DEBEN
> obtenerse de la fecha y hora REAL del sistema operativo al momento de crear el archivo.
> **NUNCA inventar ni estimar** la hora. Usar el comando del sistema para obtenerla:
> - **PowerShell:** `Get-Date -Format "yyMMdd-HHmm"`
> - **Bash/Zsh:** `date +"%y%m%d-%H%M"`
>
> Si el agente AI no puede ejecutar comandos del sistema, debe solicitar al usuario
> que proporcione la fecha/hora actual, o usar la fecha/hora del contexto de la conversación.

---

## Estructura recomendada

1. **Título y metadatos** — ID (MEM-YYMMDD-HHmm), nombre, fecha, autor (`git config user.name`), estado, referencias.
2. **Propósito** — Qué unidad de trabajo cubre y cuál era su objetivo.
3. **Qué se creó** — Archivos nuevos con ruta y propósito.
4. **Qué se modificó** — Archivos cambiados y descripción del cambio.
5. **Decisiones de diseño** — Trade-offs prácticos tomados en la implementación.
6. **Bugs y correcciones** — Bugs encontrados y cómo se resolvieron.
7. **Tests** — Cantidad, tipo, cobertura, resultado.
8. **Pendientes y stubs** — Lo que quedó para futuros trabajos.

### Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas, gráficos y cualquier otro elemento visual (no ASCII art ni imágenes embebidas).

---

## Importancia

- **Continuidad**: retomar trabajo sin pérdida de contexto.
- **Contexto para AI**: los agentes leen la memoria para entender patrones e historia.
- **Onboarding**: nuevos integrantes entienden la historia de implementación.
- **Trazabilidad**: Spec → Memoria → Código.

---

## Estándar de calidad mínimo (OBLIGATORIO)

Un MEM **debe ser autocontenido y explicativo**. Cualquier persona (o agente AI) que lo lea
meses después debe poder entender QUÉ se hizo, POR QUÉ se tomaron ciertas decisiones,
y CUÁL fue el resultado sin necesidad de revisar commits o leer otros documentos.

### Reglas de contenido mínimo

1. **Resumen ejecutivo** — SIEMPRE incluir un párrafo que explique qué se implementó y cuál
   fue el resultado final (build status, tests, errores encontrados). No solo bullets.
2. **Archivos con propósito** — No listar archivos sin explicar QUÉ hacen. Cada archivo
   creado/modificado debe tener una descripción que explique su rol en el sistema.
3. **Decisiones de implementación** — SIEMPRE documentar los trade-offs prácticos:
   por qué se eligió un enfoque sobre otro, qué alternativas se descartaron.
4. **Contexto de integración** — Explicar cómo lo implementado se conecta con el
   resto del sistema. ¿Qué componentes consumen lo nuevo? ¿Qué flujo completa?
5. **Tests con resultado** — No solo "3 tests". Indicar qué verifican y resultado total
   del suite (regresiones, total passing).

### Anti-patrones (NO HACER)

| ❌ Anti-patrón | ✅ Lo correcto |
|----------------|----------------|
| MEM de 10 líneas tipo telegrama | Mínimo: resumen + archivos con propósito + decisiones + tests |
| "Creado X, modificado Y" sin explicar qué hace | Cada archivo lleva descripción de su responsabilidad |
| Solo listar archivos sin explicar decisiones | Sección de decisiones explica POR QUÉ se hizo así |
| Omitir resultado de build/tests | Siempre incluir estado final del build y suite de tests |
| MEM sin contexto de la SPEC origen | Referenciar SPEC y explicar qué objetivo cubría |

### Métrica de completitud

Un MEM bien escrito responde estas preguntas:
- ¿Puedo retomar este trabajo mañana sin perder contexto? → Si no, falta detalle.
- ¿Entiendo por qué se tomó cada decisión? → Si no, faltan decisiones.
- ¿Sé qué se rompió/encontró durante la implementación? → Si no, faltan bugs/issues.
- ¿Un developer nuevo puede entender qué existe ahora? → Si no, falta explicación.

---

---

## Ciclo de vida de un MEM

Un MEM no tiene estados — es un registro histórico inmutable de lo que ocurrió. Una vez escrito, no se modifica (salvo correcciones de forma). No se depreca ni se cierra: es la bitácora del proyecto.

Esta carpeta no usa INDEX.md — los documentos se listan por timestamp.

---

## Documentos

Los documentos de esta carpeta se listan directamente sin INDEX.md.
Cada archivo sigue la convención `MEM-YYMMDD-HHmm-descripcion.md` y se
identifica por su timestamp único.

---

## Métricas del Proyecto (DORA + SDLC)

La metodología AI-Native-SDLC define métricas clave para medir la salud del
desarrollo. Los documentos de memoria son el lugar natural para registrar
estas mediciones, ya que capturan el resultado real de cada unidad de trabajo.

### Métricas a registrar en cada MEM

| Métrica | Qué mide | Cómo registrar |
|---------|----------|----------------|
| **Lead Time** | Tiempo desde SPEC creada hasta MEM completada | Diferencia entre timestamps de SPEC y MEM |
| **AI-time por Bolt** | Tiempo de ejecución del agente AI | Duración real del V-Bounce en cada fase |
| **Bounces por Bolt** | Iteraciones agente-humano por unidad de trabajo | Cantidad de ciclos V-Bounce hasta aprobación |
| **Tests creados** | Cobertura generada | Cantidad y tipo de tests en la sección de verificación |
| **% Código AI-generated** | Proporción de código generado por AI | Debería ser 100% si se sigue la metodología |
| **Throughput** | Bolts completados por período | Se calcula agregando MEMs por semana/sprint |
| **Defectos post-entrega** | Bugs encontrados después de completar | Referencias a BUG-NNN en MEMs posteriores |

### Ejemplo de sección de métricas en un MEM

```markdown
## Métricas

| Métrica | Valor |
|---------|-------|
| Lead Time (SPEC → MEM) | 4h AI-time |
| Bounces | 2 (SPEC → impl → review → fix → aprobado) |
| Tests creados | 17 (10 unit + 4 integration + 3 e2e) |
| Código AI-generated | 100% |
```

> **Nota:** Las métricas agregadas del proyecto (dashboards, tendencias) se
> calculan a partir de los datos individuales de cada MEM. La herramienta
> de gestión puede extraerlos automáticamente si se usa un formato consistente.
