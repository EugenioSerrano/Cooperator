# ADRs — Index

**Methodology version:** 5.2

Architecture Decision Records: brief, immutable documents that capture
significant architectural decisions, their context, alternatives considered
and consequences. An ADR becomes governing only after `AITL-ADR-Approval`
(Architect / Tech Lead). Source code is the definitive reference for the
current implementation state; the approved ADR is the governing reference
for the decisions it records.

---

## 🟡 Draft (pending `AITL-ADR-Approval`)

| ID | Document | Description |
|----|----------|-------------|
| —  | —        | —           |

## ✅ Accepted

| ID | Document | Description |
|----|----------|-------------|
| ADR-001 | [ADR-001-layout-y-empaquetado-cooperator-2.md](ADR-001-layout-y-empaquetado-cooperator-2.md) | Layout, empaquetado y compatibilidad de Cooperator 2.0 (paquetes/TFMs, firma, dependencias) |
| ADR-002 | [ADR-002-formato-coop2-y-migrador.md](ADR-002-formato-coop2-y-migrador.md) | Formato `.coop2` (JSON canónico) y migrador `.coop` → `.coop2` (`net48`, CLI) |
| ADR-003 | [ADR-003-motor-scriban-headless-y-cli.md](ADR-003-motor-scriban-headless-y-cli.md) | Motor de generación headless (Scriban) + CLI determinista |
| ADR-004 | [ADR-004-cache-y-configuracion.md](ADR-004-cache-y-configuracion.md) | Caché (`ICacheProvider` en memoria) y configuración portable de Cooperator 2.0 |
| ADR-005 | [ADR-005-provider-de-datos.md](ADR-005-provider-de-datos.md) | Provider de datos: `Microsoft.Data.SqlClient`, transacciones y superficie de `Data` |
| ADR-006 | [ADR-006-carga-de-ensamblados.md](ADR-006-carga-de-ensamblados.md) | Carga de ensamblados de providers Lazy: nombres explícitos, hook del host, diagnósticos tipados |
| ADR-007 | [ADR-007-politica-criptografica.md](ADR-007-politica-criptografica.md) | Política criptográfica (AES-GCM con rotación) y migración de valores protegidos |
| ADR-008 | [ADR-008-compatibilidad-de-api.md](ADR-008-compatibilidad-de-api.md) | Compatibilidad de API: ruptura limpia documentada, `[Serializable]` retirado |
| ADR-009 | [ADR-009-shell-del-modeler.md](ADR-009-shell-del-modeler.md) | Shell del Modeler (WPF) y tooling sobre el motor headless |

## ❌ Rejected

| ID | Document | Reason |
|----|----------|--------|
| —  | —        | —      |

## ⛔ Superseded

| ID | Document | Superseded by |
|----|----------|---------------|
| —  | —        | —             |

## ⛔ Deprecated

| ID | Document | Status |
|----|----------|--------|
| —  | —        | —      |

---

**Last updated:** August 2026
