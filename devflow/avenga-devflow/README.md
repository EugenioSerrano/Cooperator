# Development Methodology (SDLC)

**Methodology version:** 5.2

## Purpose

This folder stores the documentation of the **development methodology** used
in the project. It contains the formal definition of the software
development life cycle (SDLC) that governs how work is planned, executed,
verified and delivered.

The methodology travels with the code — every team member has direct access
without searching external wikis.

---

## What documents go here?

Documentation about the **working process**, not the project itself:

- Definition of the development methodology (full SDLC or adaptation).
- Work framework: roles, artifacts, ceremonies, quality gates.
- Definition of work units (Bolts: functional / non-functional / test).
- Operational micro-cycle (V-Bounce: approved SPEC → autonomous generation →
  mandatory MEM + manifest → AITL-MEM-Approval).
- Named AITL checkpoints (`AITL-US-Approval`, `AITL-BUG-Approval`, `AITL-TC-Approval`, `AITL-BOLT-READY-Approval`, `AITL-BOLT-DONE-Approval`, `AITL-ADR-Approval`, `AITL-SPEC-Approval`, `AITL-MEM-Approval`, …)..
- Definition of Ready (DoR, evaluated within AITL-BOLT-READY-Approval) and
  Definition of Done (DoD).
- Metrics strategy (DORA Five, Bolt Lead Time, AITL governance metrics).
- Integration of AI tooling into the workflow.
- SDLC artifact map (which document is produced at each phase).

---

## Naming convention

No numeric sequential prefix required (few documents, stable nature):

```
Methodology-name-description.md
```

The canonical methodology file is `Avenga-DevFlow.md` (hyphen, no spaces).

---

## Recommended structure

The canonical methodology document (`Avenga-DevFlow.md`) follows this
structure:

1. **Quick Start** — The single path: source evidence → US/TC/BUG → Bolt →
   SPEC → V-Bounce → MEM → AITL approvals.
2. **Introduction and philosophy** — Principles, goals, base frameworks.
3. **Glossary and key concepts** — Normative source for concepts,
   definitions, taxonomies and artifact contracts.
4. **Operating principles and rules** — Normative source for lifecycle,
   AITL, gates, autonomy and metrics.
5. **End-to-end process** — Explanatory walkthrough from raw input to
   production.
6. **Repository structure** — Normative for artifact locations, filenames
   and folder responsibilities.
7. **References** — External evidence base.

> **Normative hierarchy:** §2 owns concepts and artifact contracts; §3 owns
> lifecycle, AITL, gates, autonomy and metrics; §4 is an explanatory
> walkthrough; §5 owns structure, locations and names. When repeated text
> appears to diverge, the section that owns that dimension governs.

---

## Diagrams and Visual Elements

Use **Mermaid** mandatorily for every diagram, chart and any other visual
element (no ASCII art, no embedded images). The single exception is
**business process notation**: a process in `analysis/process/` may use BPMN
when the business already models in BPMN (W08, §5.7) — everything else,
including every diagram in this folder, is Mermaid.

---

## Guidelines

- The methodology is a **living but stable** document. It is updated with
  process improvements, not on every iteration.
- Changes must be agreed and communicated. Framework evolution is recorded in
  the methodology's own repository, not here; what this project records is its
  own upgrades, in the `CHANGELOG.md` at the repository root (see §5.16).
- This is not the place for project technical decisions (→ `adrs/`) nor for
  implementation specs (→ `spec/`).

---

## Document index

See **[INDEX.md](INDEX.md)** for the document listing.

---

## Language

YAML keys, status enums and IDs always stay in **English** (the schema).
Section headings follow the family: those of `analysis/`, feature User Stories
and Test Cases go in the project's `content_language`; headings of every other
artifact family — this folder included — stay in English. All prose — descriptions, context, rationale, findings — goes in
the project's content_language (see [devflow/README.md](../README.md) ->
Language policy, §3.15).
