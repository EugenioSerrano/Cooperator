# Glossary (Ubiquitous Language)

## Propósito

Esta carpeta contiene el **glosario del dominio**: definiciones consensuadas de
los términos de negocio que usa el equipo. Es el *ubiquitous language* (DDD) —
el vocabulario compartido entre stakeholders, analistas, desarrolladores y
agentes AI.

Un término bien definido aquí elimina ambigüedad en User Stories, Specs, código
y conversaciones. Si dos personas usan la misma palabra con significados
distintos, el glosario es la fuente de verdad.

---

## ¿Qué se documenta aquí?

- **Términos de negocio** — Conceptos del dominio con definición precisa.
- **Sinónimos y alias** — Palabras distintas que refieren al mismo concepto.
- **Aclaraciones de contexto** — Cuándo un término cambia de significado según el módulo.
- **Términos prohibidos** — Palabras ambiguas que el equipo decidió NO usar.

---

## Formato

Se usa Markdown con una estructura tabular simple. Puede ser un solo archivo
(proyectos chicos) o múltiples archivos agrupados por módulo/bounded context.

### Estructura de una entrada

```markdown
### [Término]

| Campo | Valor |
|-------|-------|
| **Definición** | [Qué significa exactamente en este proyecto] |
| **Sinónimos** | [Otras palabras que refieren a lo mismo] |
| **No confundir con** | [Términos similares con significado distinto] |
| **Entidad** | [Link a la entidad en domain-model/ si existe] |
| **Ejemplo** | [Caso concreto que ilustra el uso] |
```

### Ejemplo

```markdown
### Cliente

| Campo | Valor |
|-------|-------|
| **Definición** | Persona física o jurídica que tiene al menos un contrato activo con la empresa. Un prospecto NO es un cliente hasta que firma. |
| **Sinónimos** | Cuenta, Customer |
| **No confundir con** | Prospecto (no tiene contrato), Usuario (puede ser empleado interno) |
| **Entidad** | [Cliente](../domain-model/entities/Cliente.md) |
| **Ejemplo** | "Juan Pérez, DNI 30.123.456, contrato #FC-2024-001 activo desde 2024-03-15" |
```

---

## Organización de archivos

La organización es libre:

```
glossary/
├── glosario-general.md          → Todo en un archivo (proyectos chicos)
```

O por bounded context:
```
glossary/
├── ventas.md
├── facturacion.md
├── logistica.md
└── README.md
```

---

## Cómo construir el glosario

1. **Extraer términos** de las entrevistas (`interviews/`) — anotar cada palabra
   que los stakeholders usan para referirse a conceptos del negocio.
2. **Detectar conflictos** — si dos personas usan el mismo término con distinto
   significado, resolver y documentar la definición oficial.
3. **Vincular con domain-model** — cada término que sea una entidad modelada debe
   referenciar su definición en `domain-model/`.
4. **Validar con el equipo** — el glosario se consensúa, no se impone.
5. **Mantener vivo** — actualizar cuando aparezcan términos nuevos o cambien
   definiciones.

---

## Relación con el flujo

```
interviews/ → glossary/ (términos consensuados) → domain-model/ (modelo formal)
                                                  → functional/ (US usan estos términos)
                                                  → spec/ (código usa estos nombres)
```

El glosario es el puente entre el lenguaje del negocio y el modelo técnico.
Los nombres de entidades en domain-model, variables en código y campos en APIs deberían
coincidir con los términos definidos aquí.

---

---

## Ciclo de vida del glosario

El glosario es un **documento vivo**: se edita in-place cuando surgen términos nuevos, cambian definiciones o se detectan conflictos de lenguaje. No se depreca ni se versiona por separado.

El INDEX.md lista todos los archivos del glosario activos.

---

## Índice de documentos

Ver **[INDEX.md](INDEX.md)** para el listado de archivos del glosario.
