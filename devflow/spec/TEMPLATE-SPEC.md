---
id: "SPEC-YYMMDD-HHmm"
titulo: ""
fecha: "YYYY-MM-DD"
autor: "" # git config user.name
llm: "" # Modelo LLM utilizado (ej. "Claude Sonnet 4", "DeepSeek V3", "GPT-4o")
estado: "pendiente" # pendiente | en-progreso | completada
origen: "" # FA (funcionalidad nueva) | REV-NNN (corrección) | BUG-NNN (fix)
fa_asociado: "" # Documento funcional de origen
adrs_asociados: [] # ADRs que aplican
bolts: [] # Bolts que implementa esta Spec (puede ser 1 o más)
prerequisitos: "" # SPECs previas, builds, estados requeridos
build_baseline: "" # Estado del build antes de empezar (ej. "0 errors, 0 warnings, 106 tests passing")
riesgo_estimado: "" # Bajo | Medio | Medio-Alto | Alto
proyectos_afectados: [] # Proyectos/módulos que se tocan
---

<!--
⚠️ TIMESTAMP OBLIGATORIO: Para generar el ID y nombre del archivo:
1. Ejecutar: Get-Date -Format "yyMMdd-HHmm" (PowerShell) o date +"%y%m%d-%H%M" (Bash)
2. Usar ese valor REAL para reemplazar YYMMDD-HHmm en el id, título y nombre de archivo.
3. NUNCA inventar ni estimar la hora. Si no podés ejecutar el comando, pedí la hora al usuario.
⚠️ LLM OBLIGATORIO: Completar el campo llm con el modelo exacto que genera este documento.
-->

# SPEC-YYMMDD-HHmm — [Título descriptivo]

| Campo                   | Valor |
|-------------------------|-------|
| **Origen**              | [US-NNN Bolts NNN–NNN / REV-NNN / BUG-NNN] |
| **ADRs Aplicables**     | [links a ADRs] |
| **Prerequisitos**       | [SPEC anterior completada, build limpio, etc.] |
| **Proyectos Afectados** | [lista de proyectos/módulos] |
| **Riesgo Estimado**     | [Bajo / Medio / Alto] |
| **Build Baseline**      | [0 errors, 0 warnings, N tests passing] |

---

## 1. Contexto y motivación

<!--
⚠️ OBLIGATORIO: Esta sección NO puede estar vacía ni ser de una sola línea.
Debe explicar: de dónde surge la necesidad, qué problema resuelve,
qué pasa si NO se implementa, y cómo se relaciona con el sistema existente.
Un párrafo mínimo de contexto de negocio + un párrafo de contexto técnico.
-->

### 1.1 Origen

[Referencia al FA/REV/BUG que origina esta spec. Explicar el contexto de negocio:
qué necesita el usuario/cliente y por qué. Tabla de bolts si aplica.]

| Bolt | Descripción | Timebox |
|------|-------------|---------|
|      |             |         |

### 1.2 Objetivo

[Qué se construye. Declaración clara en 2-3 frases que un developer nuevo pueda entender
sin contexto previo. Incluir el valor que aporta al sistema.]

---

## 2. Alcance

### 2.1 Qué cubre

- ✅ [Funcionalidad o componente incluido]
- ✅ [Funcionalidad o componente incluido]

### 2.2 Qué NO cubre

- ❌ [Funcionalidad excluida explícitamente]
- ❌ [Funcionalidad excluida explícitamente]

### 2.3 Impacto en proyectos

| Proyecto | Archivos nuevos | Archivos modificados | Tests nuevos |
|----------|----------------|---------------------|-------------|
|          |                |                     |             |

---

## 3. Stack tecnológico

| Componente | Tecnología | Versión | Notas |
|------------|------------|---------|-------|
|            |            |         |       |

---

## 4. Implementación por fases

<!--
⚠️ OBLIGATORIO: Cada fase debe ser EXPLICATIVA, no telegráfica.
Describir: qué archivos se crean/modifican, qué patrones se aplican,
cómo interactúa con componentes existentes, y por qué se eligió ese enfoque.
Una fase de 2 líneas es INSUFICIENTE. Mínimo: descripción + archivos + lógica.
-->

### Fase A — [Nombre] (BOLT-NNN)

**Duración:** Xh AI-time — **Complejidad:** Baja/Media/Alta

#### A.1 [Subtarea]

[Detalle técnico: código, configuración, archivos a crear/modificar.
Explicar QUÉ se hace, POR QUÉ se hace así, y CÓMO se integra con lo existente.]

#### A.2 [Subtarea]

[Detalle técnico. Incluir nombres de clases/métodos/endpoints concretos,
patrones aplicados, y referencias a ADRs si corresponde.]

---

### Fase B — [Nombre] (BOLT-NNN)

**Duración:** Xh AI-time — **Complejidad:** Baja/Media/Alta

#### B.1 [Subtarea]

[Detalle técnico.]

---

## 5. Tests y criterios de aceptación

| AC | Descripción | Tipo de test | Estado |
|----|-------------|-------------|--------|
|    |             |             |        |

---

## 6. Checklist de completitud

- [ ] Contexto explica POR QUÉ se hace (no solo QUÉ)
- [ ] Alcance definido (qué cubre y qué NO)
- [ ] Stack tecnológico documentado
- [ ] Fases de implementación detalladas (no telegráficas)
- [ ] Cada fase menciona archivos concretos y patrones aplicados
- [ ] Criterios de aceptación con tests verificables
- [ ] ADRs referenciados
- [ ] Build baseline registrado
- [ ] Un developer nuevo puede implementar esto sin preguntar nada
- [ ] Nada requiere "inventar o decidir" (si sí → FA o ADR)
