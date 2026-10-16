# Backlog de Bolts — Cooperator 2.0

> **Documento de trabajo — NO gobernado, NUNCA evidencia (G32).**
> Propuesta del `dotnet-architect-agent` en el Run N°4 (`ARUN-261015-2313`), a partir del ruteo `REV-001` §6 y de los nueve ADRs `accepted`.
> Se ajusta a medida que los Bolts se crean y aprueban; la fuente de verdad de cada Bolt es su propio documento + manifiesto.

## Leyenda

- **⚑US** = candidato a feature US (lo decide el dueño al proponerlo, según la nota de `REV-001` §6).
- **Deps** = Bolt del que depende. **cat** = `work_category` propuesta.
- `test_integration` y `risk_class` se proponen en el documento de cada Bolt y se fijan en `AITL-BOLT-READY-Approval`.

## Ola 0 — Pendientes/bloqueados externos (no creados)

| Ítem | Alcance | Origen | Estado |
|---|---|---|---|
| BUG de seguridad | Clave e IV embebidos + cifrado legacy en runtime | F-23, §6-12 | **No existe**: su Bolt dedicado queda bloqueado hasta `AITL-BUG-Approval` (ADR-007 es el estándar del fix) |
| DISC del enumerador | Dependencia real del equipo en el enumerador de servidores del Modeler | F-34, §6-21 | **No existe**: el Bolt condicional del shell se diseña sin depender de ella |
| Canal de distribución | Feed privado/público, firma de paquetes, folder feed | ADR-001 §3.6 | Decisión del dueño (`tech_lead`), antes del release 2.0.0; bloquea parcialmente el Bolt 027 |

## Ola 1 — Habilitador (BOLTs 001-003 creados 2026-10-15, presentados para READY)

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **001** | Arnés de caracterización + build reproducible (2.0 y 1.4) + estructura de solución + gate de API-compat con línea base 1.4 | F-50/§6-20; ADR-001 §3.1/§3.4; ADR-008 §5 | — | infra |
| **002** | Migrador `.coop` → `.coop2` con reporte y fixtures dorados (t1) | F-41/42/43; §6-3; ADR-002 | 001 | refactor |
| **003** | Partición `Cooperator.Framework.Modeling` | F-27/28/29; §6-2/5; ADR-001 §3.1 | 001 | refactor |

## Ola 2 — Contratos base y capas portables

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **005** | Carga de providers Lazy por nombre calificado + hook + diagnóstico tipado | F-02; §6-10; ADR-006 | 001 | refactor |
| **006** | `Core`: retiro del export a Excel y de los puntos muertos de superficie | F-03; §6-8; ADR-001 §3.5 | 001 | debt |
| **007** | Retiro de `[Serializable]`/`SerializationInfo` y del rastro `BinaryFormatter` | F-05/30/43; §6-13; ADR-008 §3.2/§3.4 | 001 | debt |
| **009** | `Library` portable: Registro/Repository, P/Invoke, EventLog y WMI resueltos según uso real | F-15…F-20/48; §6-7 | 001 | refactor |
| **011** | Configuración portable (`ICooperatorConfiguration` + provider por entorno; sin escritura del `.config`; smtp perezoso) | F-04/13/22; §6-9; ADR-004 §3.2/§3.3/§3.5 | 001 | refactor |
| **012** | Limpieza de referencias muertas y de proyecto en todo el árbol | F-11/12/21/36/44; §6-16 | 001 | debt |
| **028** | Notas de migración de la 2.0 con el registro de rupturas (acompaña a cada Bolt que rompe superficie) | ADR-008 §3.1/§5; ADR-001 §3.2 | 001 | debt |

## Ola 3 — Contrato de formato y primeros consumidores

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **004** | Contrato `.coop2` en `Modeling` (esquema v1 + escritor canónico + lector/escritor + conformidad bilateral **t2**) | F-41/42; §6-3; ADR-002 §3.1/§3.4 | 002, 003 | refactor |
| **008** | `Core`: resolución de tipos calificada en el mapeo filas→objetos | F-47; §6-19; ADR-006 §3.1 | 005 | debt |
| **010** | `Library`: almacén portable de configuración de usuario (reemplazo del Repository) — **condicional** a la evaluación de uso | F-16; ADR-001 §3.5 | 009 | refactor |
| **013** | `Data`: caché tras `ICacheProvider` (en memoria, regiones/TTL/cap) y retiro de `System.Web` | F-07/08; §6-4; ADR-004 §3.1 | 011 | refactor |
| **022** | Resolutor `aesgcm:` + key providers + validación de esquemas (`plain:`/desconocido) | F-23; §6-12; ADR-007 §3.1/§3.2; ADR-004 §3.3 | 011 | hardening |

## Ola 4 — Datos, motor base y batch provisional

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **014** | `Data`: provider único `Microsoft.Data.SqlClient`, sin casts, interior tipado, timeout y connection string por configuración | F-09/14; §6-5; ADR-005 §3.1–§3.4 | 011, 013 | refactor |
| **017** | Motor headless: punto de entrada tipado, proyección del modelo y contexto de render endurecido | F-26; §6-6; ADR-003 §3.1/§3.5 | 001, 004 | refactor |
| **024** | `ConnectionStringEncriptor` como shell delgado del motor de protección | F-40; ADR-007 §3.5; ADR-009 §3.6 | 022 | refactor |
| **029** | Ejecución del batch real del corpus `.coop` tras t2 + archivo/retiro de originales | ADR-002 §3.2/§3.4 | 002, 004 | refactor |

## Ola 5 — Motor completo (motor + CLI + plantillas) y shell

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **016** | El reverse deja de heredar de la capa de datos | F-29; ADR-005 §3.6 | 003, 014 | refactor |
| **018** | CLI del motor: comandos, códigos de salida, plan sin escritura, reporte estable | F-33/42; §6-6; ADR-003 §3.2/§3.3 | 017 | infra **⚑US** |
| **019** | Set de plantillas C# (`*.Auto.cs`/partials, entidades POCO sin atributos de contrato) | F-37/43; ADR-003 §3.1/§3.4; ADR-008 §3.7 | 017 | refactor |
| **020** | Set de plantillas de stored procedures (T-SQL modernizado, un archivo por SP) | F-46/37; §6-18; ADR-003 §3.1/§3.5 | 017 | refactor |
| **021** | Plantillas de proyectos y de configuración con bootstrap de precedencia | F-37/13; §6-15; ADR-003 §3.1/§3.4; ADR-004 §3.4 | 017 | refactor |
| **025** | Shell WPF: esqueleto + editor dirigido por esquema + settings JSON por usuario | F-33/38; §6-14; ADR-009 §3.1–§3.3 | 004, 017 | refactor **⚑US** |

## Ola 6 — Cierres de superficie, cripto operativa y shell completo

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **015** | Retiro del gate por `PublicKeyToken` en `Data` y de su contraparte generada | F-10; §6-11; ADR-001 §3.3 | 014, 019 | debt |
| **023** | Comandos de valores protegidos (inspección/migración/rotación) con descifrado legacy aislado y detección FIPS | F-23/40; ADR-007 §3.3–§3.5 | 018, 022 | hardening |
| **026** | Shell ↔ motor: plan/reporte, asíncrono y cancelable, handoff CLI | F-33/42; ADR-009 §3.1 | 017, 018, 025 | refactor |
| **027** | Shell: empaquetado, asociación `.coop2` por instalador, retiro de ClickOnce/APIs muertas | F-35/36; §6-15; ADR-009 §3.4–§3.6 | 025 | infra |

## Ola 7 — Cierre

| ID | Alcance | Origen | Deps | cat |
|---|---|---|---|---|
| **030** | Retiro de la capacidad de descifrado legacy y cierre del registro de vencimiento | ADR-007 §3.3/§3.4 | 023, 029 | debt |

## Observaciones de la revisión de pares (2026-10-15) — a considerar al redactar esos Bolts

1. **028 ↔ gate de API**: ADR-008 §5 hace fallar el gate ante una diferencia no cubierta por notas. Si 028 fuera el dueño exclusivo de las notas, los Bolts que crean paquetes (003/009…) fallarían el gate antes de que existan las notas. Resolución: cada Bolt que rompe superficie registra **su propia entrada** (ADR-008 §3.1) y 028 estructura/consolida — o 028 produce el esqueleto primero.
2. **004 ↔ ADR-009 §3.2**: el criterio "enums reales + tabla de mapeo accesible" se reparte entre 003 (la tabla) y **004** (los enums del esquema) — 004 debe recogerlo explícitamente como criterio de aceptación.

## Puntos de decisión del dueño (fuera del backlog de Bolts)

- ⚑US: Bolts **018** (CLI) y **025** (shell WPF) — al proponerlos, decidir si se elevan a feature US (requerirían `AITL-US-Approval` y Bolt funcional) o quedan como migración no-funcional bajo `US-000`.
- BUG F-23, DISC F-34, RISK del retiro legacy (ADR-007 §3.3) y canal de distribución: pendientes de mandato/decisión.
