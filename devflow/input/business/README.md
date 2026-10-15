# Input — Business (raw business context)

**Methodology version:** 5.2

## Purpose

This folder stores **raw business material** that defines the project's scope,
constraints, and regulatory context. These are documents received from the client
or the business domain that establish the *why* and *what* of the project before
any technical analysis begins.

This is read-only reference material — the foundational business context that
drives analysis, Discovery, User Stories, ADRs, and ultimately implementation.

---

## What goes here?

- **RFP / RFQ** (Request for Proposal / Quotation) — the client's original solicitation.
- **BRD** (Business Requirements Document) — formal business needs and objectives.
- **SOW** (Statement of Work) — contractual scope, deliverables, and timelines.
- **Business case & feasibility studies** — ROI analysis, cost/benefit, alternatives evaluated.
- **Regulatory & compliance documents** — industry regulations, legal requirements,
  data protection laws (GDPR, HIPAA, PCI-DSS, SOX, etc.) that constrain the solution.
- **Service Level Agreements (SLAs)** — performance, availability, and support commitments.
- **Organizational policies** — security policies, data governance, internal standards
  the client requires the solution to follow.
- **Domain glossary / terminology** — any existing business glossary or ubiquitous
  language documentation provided by the client.
- **Stakeholder org charts & RACI matrices** — who is who, who decides what.

## What does NOT go here?

- Analysis or interpretation of the business material → `analysis/` (`business-context/`)
  and, for material unknowns, `discovery/DISC-NNN`.
- Domain model derived from this material → `analysis/domain-model/`.
- Business process diagrams (BPMN) → `analysis/process/`.
- User Stories derived from requirements → `functional/`.
- Technical decisions driven by constraints → `adrs/`.

---

## Organization

Subfolders by category or source:

```
business/
├── rfp-sow/              → RFP, SOW, contractual scope documents
├── requirements/         → BRD, functional specs provided by the client
├── compliance/           → Regulations, legal requirements, data protection laws
├── policies/             → Client security policies, governance, internal standards
├── business-case/        → Feasibility studies, ROI analysis, market research
└── stakeholders/         → Org charts, RACI matrices, key contacts
```

---

## Conventions

- Keep files in their original format (`.pdf`, `.docx`, `.xlsx`).
- Use descriptive filenames that include the source and date:
  `2026-03-15-client-rfp-financial-module.pdf`.
- Do not modify original documents. If annotations are needed, create a
  companion `.md` file with your notes referencing the original.
- Sensitive or confidential documents should be stored externally with a
  `README-pointers.md` referencing their location — never commit PII or secrets.

---

## Flow

```
input/business/  →  analysis/business-context/ (stakeholders, constraints, success metrics)
                 →  analysis/business-risks/ (BR-NNN — market, regulatory, adoption)
                 →  analysis/domain-model/ (ubiquitous language, business entities)
                 →  analysis/process/ (business processes)
                                 ↓
                     analysis/ is the only path onward — raw input never
                     feeds functional/, adrs/ or risks/ directly (§2.1, §4.1)
                                 ↓
                 →  functional/ (User Stories, acceptance criteria)
                 →  adrs/ (architecture decisions driven by constraints)
                 →  risks/ (compliance risks, SLA risks)
```

The business context captured here defines the boundaries within which all
subsequent design and implementation decisions must operate. It is the most
fundamental input layer of the Avenga DevFlow.

---

## Document index

This folder does not use an INDEX.md — business documents are organized by
subfolder. See each subfolder's contents for reference material.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
