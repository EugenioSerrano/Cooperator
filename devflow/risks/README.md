# Risks (Project Risk Register)

**Methodology version:** 5.2

## Purpose

This folder is the **living risk register** of the project: identified
threats to delivery, quality or timeline, with assessment, mitigation plan
and current status.

It is a **transversal artifact** — it lives during the entire project, not
only during analysis. It is fed from many sources:

- `analysis/business-risks/` (business risks identified
  during analysis)
- `discovery/` (technical risks surfaced studying the legacy / domain)
- `reviews/` and `adversarial-reviews/` (risks found in audits)
- `retros/` (risks called out by the team weekly)
- `incidents/` (risks confirmed by something going wrong)
- The team's own experience

---

## Boundary with `analysis/business-risks/`

We split risks on purpose. The boundary is **not** "what stage we are in" —
it is **what the risk is about**.

| Risk type | Lives in | Audience | Lifecycle |
|-----------|----------|----------|-----------|
| **Business risk** -- market, regulation, adoption, business model | [`../analysis/business-risks/`](../analysis/business-risks/) (BR-NNN-<description>.md) | PO, stakeholders | Stabilizes early; part of *understanding the world* |
| **Project / technical / team / dependency risk** | `risks/RISK-NNN-<description>.md` (here) | Tech Lead, SRE, Sec, PO | Lives the full project; weekly review; open → mitigated → materialized → closed |

**For the AI agent:** when an interview, document or observation surfaces a
*"if X happens we are in trouble"* finding, route it:

- Business angle (*"if the regulator changes the rule, the product cannot
  exist as designed"*) → `analysis/business-risks/BR-NNN-<description>.md`.
- Execution angle (*"third-party API has 99.5% SLA, we need 99.99%"*,
  *"only one person knows the legacy module"*) → a new `RISK-NNN-<description>.md` here.
- If a finding has **both angles**, record it in both places and
  cross-link them.

`analysis/business-risks/` is the **input** that becomes one or more entries in
this register when the team decides to actively manage the risk.

---

## What goes here

- Technical risks identified during Inception or Discovery.
- Integration risks with third parties.
- Performance, security or scalability risks.
- External dependencies with deadlines or uncertainty.
- Team or process risks impacting delivery.

## What does NOT go here

- **Business risks** → `analysis/business-risks/BR-NNN-<description>.md`.
- **Confirmed defects** → `bugs/`.
- **Production incidents** → `incidents/`.
- **Architectural decisions on how to mitigate** → `adrs/` (with link
  back here).

---

## Naming convention

```
RISK-NNN-short-description-in-kebab-case.md
```

---

## RISK structure

The authoritative structure is [`TEMPLATE-RISK.md`](TEMPLATE-RISK.md) — its
**6 numbered sections**, plus the frontmatter that carries the assessment:

- **Frontmatter** — ID, title, date, author, `category`, `probability`, `impact`, `overall_severity` (probability × impact) and `status` (`open | mitigated | materialized | closed`). The assessment lives in fields, not in prose sections.
1. **Description** — What could happen and in which context.
2. **Analysis** — Evidence justifying the assigned probability and impact.
3. **Mitigation plan** — Actions to reduce probability or impact.
4. **Contingency plan** — What to do if the risk materializes.
5. **Impact on Bolt risk class (§3.3)** — How this risk influences Bolt classification.
6. **Relations** — Related DISCs, ADRs, REVs, BUGs, INCs.

Diagrams in **Mermaid**.

---

## Lifecycle

| Status            | Meaning |
|-------------------|---------|
| **open**          | Risk identified, no mitigation applied yet. |
| **mitigated**     | Mitigation plan implemented. The risk is still monitored. |
| **materialized**  | The risk happened. Contingency plan activated. |
| **closed**        | Risk no longer relevant (context changed, mitigation removed the threat). |

`INDEX.md` reflects status: 🔴 Open / 🟡 Mitigated and Materialized (partially
resolved, still monitored) / 🏁 Closed (GUARDRAILS, INDEX convention). A
closed risk is a **terminal success**, so it is 🏁 and never ⛔ — ⛔ means
obsolete or abandoned.

---

## Connection to Bolt risk classes (§3.3)

The risk class assigned to a Bolt during **`AITL-BOLT-READY-Approval`** (as part of
its DoR) directly determines its **autonomy level** and **review
requirements**. Risks documented here inform that classification:

| Risk class | Default autonomy | Min approvers at AITL-MEM-Approval | AREV |
|------------|-----------------|-------------------------------------|------|
| **low** | L3 (Autonomous) | 1 (executing Dev-validator) | optional |
| **medium** | L3 (Autonomous) | 1 (executing Dev-validator) | optional |
| **high** | L2 (Bounded) | 1 (the executing Dev-validator; QA/Sec optional) | optional |
| **critical** | L1 (Suggest) | 1 (the executing Dev-validator; QA/Sec optional) | optional |

When a RISK-NNN is linked to a Bolt, the Bolt's `risk_class` is recorded in
the **Bolt frontmatter** (assigned at `AITL-BOLT-READY-Approval`). `risk_class`
may be **escalated at any subsequent review** (QA/Sec); it **can never be
reduced after the first MEM approval** without formal re-review and
re-approval of the Bolt; every reassignment appends to the Bolt's
`risk_history` (§3.3, GUARDRAILS W14). The manifest deliberately does
not duplicate risk data (§3.12). AREV is **optional for all risk classes** —
stakeholder-triggered, never automatic (§2.15).

---

## Index

See **[INDEX.md](INDEX.md)** for the active register.


## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in [`../LANGUAGE`](../LANGUAGE)
(see §3.15).
