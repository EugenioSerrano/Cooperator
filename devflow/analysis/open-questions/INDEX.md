# Open Questions — Index

**Methodology version:** 5.2

Centralized backlog of analysis-phase open questions. See
[README.md](README.md) for lifecycle, rules and templates.

> **Sunset rule (G35):** before `AITL-BOLT-READY-Approval` (readiness), every
> OQ whose `targets` include that Bolt's parent US or one of its governing
> artifacts must be `answered`, `deferred` or `dropped`. No `open` /
> `in-validation` OQ may survive into delivery.

---

## 🔴 Open (not yet investigated)

| ID     | Title | Priority | Status | Owner | Targets | Opened |
|--------|-------|----------|--------|-------|---------|--------|
| —      | —     | —        | —      | —     | —       | —      |

## 🔄 In validation / Deferred

| ID | Title | Revisit on | Owner | Reason |
|----|-------|------------|-------|--------|
|    |       |            |       |        |

## 🏁 Answered

| ID | Title | Status   | Closed on  | Closed by | Resolution / target |
|----|-------|----------|------------|-----------|---------------------|
| [OQ-001](OQ-001-particion-de-utility.md) | ¿Particionar Framework.Utility en modelo portable + tooling Windows? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7 — opción A (partición `Utility` → `Modeling` portable) |
| [OQ-002](OQ-002-soporte-system-web.md) | ¿La 2.0 corta System.Web o preserva apps web .NET Framework? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7 — opción C variante default-en-memoria (`ICacheProvider` intercambiable) |
| [OQ-003](OQ-003-gate-de-licenciamiento.md) | ¿Se preserva el gate de licenciamiento por PublicKeyToken? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7 — opción A (retirar; era anti-manipulación, no licencia) |
| [OQ-004](OQ-004-formato-del-coop-json.md) | ¿Formato oficial y extensión del archivo de modelo (.coop JSON)? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7 — `.coop2` (JSON) + migrador `.coop`→`.coop2` requerido |
| [OQ-005](OQ-005-alcance-features-en-riesgo.md) | ¿Qué features en riesgo (VB.NET, Oracle, Excel) siguen en alcance? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7/§6 — opción B: las tres se deprecan |
| [OQ-006](OQ-006-politica-de-compatibilidad.md) | ¿Qué política de compatibilidad rige la 2.0 para las apps generadas? | answered | 2026-10-15 | human:eugenioserrano | REV-001 §7 — opción A: moderno-only (.NET 10+; sin puente 4.8) |

## ⛔ Dropped (no longer relevant — not an answer)

| ID | Title | Status  | Closed on  | Closed by | Reason |
|----|-------|---------|------------|-----------|--------|
|    |       | dropped | YYYY-MM-DD | @user     |        |

---

## Counters

| Status        | Count |
|---------------|-------|
| open          | 0     |
| in-validation | 0     |
| answered      | 6     |
| deferred      | 0     |
| dropped       | 0     |
| **total**     | **6** |

> Update counters whenever an OQ changes state.

---

**Last updated:** 2026-10-15
