---
id: "MEM-YYMMDD-HHmm"
titulo: ""
fecha: "YYYY-MM-DD"
autor: "" # git config user.name
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
estado: "completa" # completa | parcial
spec_origen: "" # SPEC-YYMMDD-HHmm que implementó
bolts: [] # Bolts cubiertos
adrs_aplicados: []
---

<!--
⚠️ TIMESTAMP OBLIGATORIO: Para generar el ID y nombre del archivo:
1. Ejecutar: Get-Date -Format "yyMMdd-HHmm" (PowerShell) o date +"%y%m%d-%H%M" (Bash)
2. Usar ese valor REAL para reemplazar YYMMDD-HHmm en el id, título y nombre de archivo.
3. NUNCA inventar ni estimar la hora. Si no podés ejecutar el comando, pedí la hora al usuario.
⚠️ LLM OBLIGATORIO: Completar el campo llm con el modelo exacto que genera este documento.
-->

# MEM-YYMMDD-HHmm — [Título descriptivo]

| Campo           | Valor |
|-----------------|-------|
| **SPEC Fuente** | [link a la SPEC] |
| **Bolts**       | [IDs de bolts cubiertos] |
| **ADRs**        | [links a ADRs aplicados] |

---

## Resumen ejecutivo

<!--
⚠️ OBLIGATORIO: NO puede ser una lista de bullets ni una línea.
Debe ser un párrafo narrativo que explique:
- Qué se implementó (funcionalidad concreta, no solo nombres de archivos)
- Cuál fue el resultado (build OK, N tests, 0 regresiones)
- Si hubo sorpresas, problemas o desviaciones respecto a la SPEC
Mínimo 3-5 frases con contexto suficiente para entender el trabajo completo.
-->

[Descripción concisa de lo implementado + resultado clave
(ej. "0 errors, 17 tests nuevos passed, 106 existentes sin regresión").
Incluir qué funcionalidad queda disponible para el usuario/sistema tras este trabajo.]

---

## Fases implementadas

<!--
⚠️ OBLIGATORIO: Cada fase debe explicar QUÉ se construyó y CÓMO.
No es suficiente decir "Se creó X". Explicar qué hace X, qué patrón sigue,
y cómo se integra con el resto del sistema. Un developer nuevo debe poder
entender la arquitectura implementada leyendo solo este MEM.
-->

### Fase A — [Nombre] (BOLT-NNN)

[Descripción de lo construido en esta fase. Qué componentes se crearon,
qué patrones se aplicaron, cómo interactúan entre sí. Decisiones clave
tomadas durante la implementación.]

### Fase B — [Nombre] (BOLT-NNN)

[Descripción de lo construido en esta fase. Mismos criterios de detalle.]

---

## Archivos creados

<!--
⚠️ OBLIGATORIO: "Propósito" NO es el nombre del archivo repetido.
Debe explicar QUÉ responsabilidad tiene el archivo en el sistema.
Ej: NO "EnteResponse DTO" → SÍ "DTO que expone código, nombre y rubro del ente al frontend via REST"
-->

| Archivo | Propósito |
|---------|-----------|
|         |           |

---

## Archivos modificados

| Archivo | Descripción del cambio |
|---------|----------------------|
|         |                      |

---

## Decisiones de implementación

<!--
⚠️ OBLIGATORIO: Esta sección NO puede estar vacía.
Siempre hay decisiones durante la implementación: qué enfoque se eligió,
qué alternativas se descartaron, qué trade-offs se aceptaron.
Si "no hubo decisiones" es porque no se documentaron — siempre las hay.
-->

| Decisión | Razón |
|----------|-------|
|          |       |

---

## Verificaciones post-implementación

### Build
```
[output de dotnet build / npm run build]
```

### Tests
```
[output de dotnet test / npm test — cantidad passed/failed/skipped]
```

---

## Métricas

| Métrica | Valor |
|---------|-------|
| Lead Time (SPEC → MEM) | [Xh AI-time] |
| Bounces | [N ciclos V-Bounce hasta aprobación] |
| Tests creados | [N (desglose por tipo)] |
| Código AI-generated | [100%] |

---

## Pendientes y stubs

[Lo que quedó para futuros trabajos.]

- [ ] Pendiente 1
- [ ] Pendiente 2
