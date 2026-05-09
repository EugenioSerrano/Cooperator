# Input (Material de Entrada)

## Propósito

Esta carpeta almacena **material crudo de entrada**: toda la información original
que sirve como punto de partida para el análisis. Es el "volcado inicial" de
documentos, código y datos que aún no han sido procesados ni analizados.

A diferencia de las demás carpetas del flujo, aquí se deposita la información
**tal cual se recibe**, sin procesar, clasificar ni interpretar. Una vez analizada,
el conocimiento extraído se documenta en `discovery/` como un DISC.

---

## ¿Qué documentos van aquí?

- Código fuente legacy de sistemas existentes (repositorios completos o extractos).
- Esquemas de bases de datos (DDL, diagramas ER, backups de estructura).
- Documentación de sistemas existentes (manuales, especificaciones funcionales).
- Archivos de configuración de sistemas legacy.
- PDFs, hojas de cálculo, diagramas y cualquier material de referencia del negocio.
- Dumps de datos de ejemplo o sets de prueba del sistema actual.
- Capturas de pantalla, grabaciones o cualquier evidencia del funcionamiento actual.
- Datasheets, especificaciones técnicas de hardware o dispositivos involucrados.

> **Nota sobre entrevistas:** Las transcripciones de entrevistas con stakeholders
> van en `analysis/interviews/`, no aquí. En `input/` solo van grabaciones
> originales (audio/video) sin transcribir si se necesitan como respaldo.

**Regla:** si es material que viene "de afuera" y aún no se analizó, va aquí.

---

## Organización

Sin convención estricta — el objetivo es preservar los archivos en su formato
original. Se recomienda organizar en subcarpetas por fuente o sistema para
mantener el orden:

```
input/
├── sistema-legacy/       → Código fuente y docs del sistema actual
├── base-de-datos/        → Esquemas, DDL, diagramas
├── documentacion-cliente/ → Manuales, specs funcionales del cliente
└── ...
```

---

## Relación con el flujo

```
INPUT (material crudo) → DISCOVERY (análisis y hallazgos) → ADRs / FA → SPEC → IMPL
```

El material en `input/` **alimenta directamente** la fase de Discovery. Una vez
que un archivo o conjunto de archivos es analizado y sus hallazgos se documentan
en un DISC, el material original se conserva aquí como referencia y trazabilidad.

---

## Notas

- Los archivos aquí **no se modifican**. Son de solo lectura.
- No requieren formato Markdown — se mantienen en su formato original.
- Si un archivo es muy grande (ej. backups de BD), considerar incluir solo
  extractos representativos o un puntero al almacenamiento externo.
- Esta carpeta **no tiene INDEX.md** ya que contiene material no estructurado.
