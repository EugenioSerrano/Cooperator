# Business Risks

**Methodology version:** 5.2

## Purpose

This folder captures **business-level risks** — market, regulatory, adoption,
and business-model threats that exist independently of how the system is built.

These are NOT project/technical risks. Project risks (dependencies, team,
technical debt) live in the transversal register [`../../risks/`](../../risks/).

Where `business-context/` answers **what world the product lives in**,
`business-risks/` answers **what could make that world stop being viable**.

---

## Boundary with project risks

We split risks on purpose. The boundary is **what the risk is about**, not
what phase we are in.

| Risk type | Examples | Home | Audience |
|-----------|----------|------|----------|
| **Business risk** | Market shrinks, regulation changes, low adoption, business model fails | `business-risks/BR-NNN-<description>.md` (here) | PO, stakeholders |
| **Project / technical / team / dependency risk** | Third-party integration unreliable, scaling unknowns, team capacity, security exposure | [`../../risks/RISK-NNN-<description>.md`](../../risks/) | Tech Lead, SRE, Sec, PO |

If a finding has **both angles** (business AND execution), record it in both
places and cross-link.

---

## What goes here

| Document type | Question it answers |
|---------------|---------------------|
| **Business risk** | What external or strategic threat could undermine the product's viability? |

Each risk is tracked with:
- Category (market / regulation / adoption / business-model)
- Likelihood and impact assessment
- Mitigation strategy
- Current status

---

## Position in the flow

```
vision/ + business-context/
       └── business-risks/ (business risks surfaced during analysis)
              └── risks/ (project risks may be derived from business risks)
```

Business risks identified during analysis feed the project risk register
(`../../risks/`) when they need active management by the delivery team.

---

## Naming convention

```
BR-NNN-short-description-in-kebab-case.md
```

Examples: `BR-001-market-entry-risk.md`, `BR-002-regulatory-change.md`

---

## How to draft with AI

1. Feed the agent every transcript in `input/interviews/`, compliance documents
   in `input/business/`, and any market/strategy material.
2. Ask the agent to extract every business-level threat mentioned or implied:
   - *"List every market, regulatory, adoption or business-model risk surfaced
     in these interviews. For each, assess likelihood and impact."*
3. Human analyst validates with PO/stakeholders and marks status.

---

## Lifecycle

| Status | Meaning |
|--------|---------|
| `draft` | Identified, not yet reviewed |
| `stable` | Reviewed and accepted — actively monitored |
| `deprecated` | No longer relevant (context changed) |

`INDEX.md` reflects status, one section per value: 🟡 Draft / ✅ Stable /
⛔ Deprecated (GUARDRAILS, INDEX convention).

---

## Index

See **[INDEX.md](INDEX.md)** for the business risk register.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — risk descriptions, impact
analysis, mitigation details — go in the project's `content_language`
(see [devflow/README.md](../../README.md) -> Language policy, §3.15).
