# Spec (Especificaciones de Implementación)

## Propósito

Esta carpeta contiene **especificaciones de implementación**: documentos que definen
EXACTAMENTE cómo debe construirse una funcionalidad. Son los "planos" de construcción.

Una Spec no es creativa — es **mecánica**. El análisis funcional y las decisiones ya se
tomaron; aquí solo se define componentes, configuración, estructura de datos, API y flujo.

---

## ¿Qué documentos van aquí?

- Definición detallada de configuración de componentes (periféricos, servicios, infraestructura).
- APIs exactas (funciones, parámetros, retornos).
- Estructuras de datos y sus layouts.
- Flujos de ejecución paso a paso.
- Configuración de entorno y dependencias.
- Diagramas de secuencia y estado.

---

## Convención de nombres

```
SPEC-YYMMDD-HHmm-descripcion-breve.md
```

Donde:
- `SPEC` — Prefijo fijo.
- `YYMMDD` — Fecha de creación (año 2 dígitos, mes, día).
- `HHmm` — Hora de creación (hora, minutos).
- `descripcion-breve` — Resumen del tema en kebab-case.
- `.md` — Extensión Markdown.

**Ejemplo:** `SPEC-YYMMDD-HHmm-descripcion-breve-del-tema.md`

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

1. **Título y metadatos** — ID (SPEC-YYMMDD-HHmm), nombre, fecha, estado, autor (`git config user.name`), FA asociado.
2. **Objetivo** — Qué construir.
3. **Contexto** — Referencia al FA de origen, restricciones de hardware.
4. **Configuración técnica** — Componentes, dependencias, parámetros, registros.
5. **API** — Funciones públicas con firmas, parámetros y comportamiento.
6. **Estructuras de datos** — Structs, enums, constantes.
7. **Flujo de ejecución** — Secuencia paso a paso, preferentemente con diagramas Mermaid.
8. **Integración con el sistema** — Tasks, queues, middleware, servicios, concurrencia.
9. **Tests** — Criterios de aceptación y procedimiento de verificación.
10. **Checklist de completitud** — ¿Se definieron todos los aspectos?

### Nivel de detalle

Si lo que vas a escribir requiere *inventar o decidir*, el documento debería ser un FA, no un Spec.
Si lo que vas a escribir es *puramente mecánico*, es un Spec.

### Diagramas y Elementos Visuales

Usar **Mermaid** obligatoriamente para todos los diagramas, gráficos y cualquier otro elemento visual (no ASCII art ni imágenes embebidas).

---

## Estándar de calidad mínimo (OBLIGATORIO)

Una SPEC **debe ser autocontenida y explicativa**. Cualquier persona (o agente AI) que la lea
meses después debe poder entender QUÉ se va a construir, POR QUÉ, y CÓMO sin necesidad de
leer otros documentos ni hacer arqueología de commits.

### Reglas de contenido mínimo

1. **Contexto y motivación** — SIEMPRE explicar de dónde surge la necesidad, qué problema resuelve,
   y qué pasa si NO se implementa. Mínimo un párrafo con contexto de negocio.
2. **Alcance explícito** — Listar qué cubre Y qué NO cubre. Nunca dejar ambigüedad.
3. **Fases con detalle técnico** — Cada fase debe describir:
   - Qué archivos se crean o modifican y con qué propósito.
   - Qué patrones o convenciones se aplican (y referenciar ADRs).
   - Cómo interactúa con componentes existentes.
4. **Criterios de aceptación** — Cada AC debe ser verificable y testeable.
5. **Stack y dependencias** — Si se introduce algo nuevo, documentar versión y razón.

### Anti-patrones (NO HACER)

| ❌ Anti-patrón | ✅ Lo correcto |
|----------------|----------------|
| SPEC de 5 líneas sin contexto | Mínimo: contexto + alcance + fases + ACs |
| "Hacer endpoint GET /api/v1/foo" (sin más) | Explicar qué retorna, de dónde saca datos, cómo se relaciona con el dominio |
| Fases como lista de bullet points sin explicación | Cada fase explica el QUÉ, POR QUÉ y CÓMO |
| Omitir prerrequisitos o baseline | Siempre declarar estado actual del build y dependencias previas |
| Copiar la US textual sin elaborar | La SPEC debe traducir la US a instrucciones de implementación concretas |

### Métrica de completitud

Una SPEC bien escrita responde estas preguntas:
- ¿Puedo implementar esto sin preguntar nada a nadie? → Si no, falta detalle.
- ¿Entiendo por qué se hace esto? → Si no, falta contexto.
- ¿Sé cuándo terminé? → Si no, faltan criterios de aceptación.
- ¿Un developer nuevo puede entender esto? → Si no, falta explicación.

---

---

## Ciclo de vida de una SPEC

| Estado | Significado |
|--------|-------------|
| **pendiente** | En redacción, todavía no se empezó a implementar. |
| **en-progreso** | En implementación activa (V-Bounce en curso). |
| **completada** | Implementada completamente. Código + tests en verde, MEM creada. |

El estado se refleja en el frontmatter YAML de cada SPEC. Esta carpeta no usa INDEX.md — los documentos se listan por timestamp.

---

## Documentos

Los documentos de esta carpeta se listan directamente sin INDEX.md.
Cada archivo sigue la convención `SPEC-YYMMDD-HHmm-descripcion.md` y se
identifica por su timestamp único.
