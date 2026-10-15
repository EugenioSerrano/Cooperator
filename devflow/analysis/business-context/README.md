# Business Context

**Methodology version:** 5.2

## Purpose

This folder documents the **business context** around the product: who we
serve, how we position ourselves, which market and regulatory constraints
apply, and how we measure success. It is the *map of the territory* before we
model the domain in `domain-model/` or the workflows in `process/`.

Where `vision/` answers **what product we want to build** (1–3 sentences +
tentative metrics), `business-context/` answers **what world that product
lives in**.

---

## What goes here

| Document type        | Question it answers                                              |
|----------------------|------------------------------------------------------------------|
| **Stakeholders**     | Who sponsors, who uses, who is impacted? (light RACI / influence map) |
| **Market / segment** | Which industry, region or niche are we targeting?               |
| **Competitors**      | Existing alternatives — what they do well / badly                |
| **Compliance**       | Applicable regulations (GDPR, HIPAA, SOX, ISO, PCI-DSS, etc.)    |
| **Business model**   | How we monetize / why this product exists                        |
| **Success metrics**  | NSM, KPIs, OKRs at the business level                            |
| **Business risks**   | Strategic / market / regulation / adoption threats. **Live in** [`../business-risks/`](../business-risks/) as `BR-NNN-<description>.md` files. Project / technical / team / dependency risks live in [`../../risks/`](../../risks/) (see below). |

> **Business risks vs. project risks.** Business risks (the kind that put
> the *product idea* at risk: market, regulation, adoption, business model)
> live in [`../business-risks/`](../business-risks/) as `BR-NNN-<description>.md` files.
> Risks about *executing* the project
> (technology, integrations, team capacity, dependencies, security exposure,
> schedule) live in the transversal register [`../../risks/`](../../risks/).
> A single interview can surface both — in that case record each in its
> proper place and cross-link. See the
> [routing table in `../README.md`](../README.md) and
> [`../../risks/README.md`](../../risks/README.md) for the full rules.

---

## Position in the flow

```
vision/ + business-context/
       └─► domain-model/ + process/
              └─► functional/ (US + Bolts)
              └─► metrics (DORA + business)
```

The business metrics defined here **must link to** the delivery metrics in
the methodology (Avenga DevFlow §3.7): an improvement in lead-time or
deployment-frequency must be traceable to a business KPI.

---

## Suggested files

One Markdown file per topic; YAML frontmatter with `author`, `date`,
`status`. Diagrams in **Mermaid**. Prose in the project's `content_language`
(see devflow/README.md -> Language policy, §3.15).

```
business-context/
├── stakeholders.md
├── market.md
├── competitors.md
├── compliance.md
├── business-model.md
└── success-metrics.md
```

> **Business risks** have their own folder and template:
> [`../business-risks/`](../business-risks/) with `BR-NNN-<description>.md` naming.

Use [TEMPLATE-BUSINESS-CONTEXT.md](TEMPLATE-BUSINESS-CONTEXT.md) as a
starting point for any of them.

---

## How to draft it with AI

1. Feed the agent every transcript in `input/interviews/`, public briefings
   and any `input/documentation/` material (decks, prior strategy docs).
2. Ask for one section at a time:
   - *"Extract every stakeholder mentioned. Classify by role
     (sponsor / user / impacted / regulator)."*
   - *"List all regulations explicitly or implicitly named. For each, cite
     the source."*
   - *"Propose 3–5 candidate success metrics aligned with each vision
     outcome."*
3. Human analyst validates with the sponsor and freezes v1.

---

## Operating notes

- Keep each document short (1–2 pages). If it grows, split it.
- Any quantitative claim must cite a source **or** be flagged `assumption`.
- If the context shifts (new regulation, new relevant competitor), update the
  file and reference the change in an ADR or RISK when it impacts decisions.

---

## Index

See **[INDEX.md](INDEX.md)** for the list of business-context documents.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../README.md) -> Language policy, §3.15).
