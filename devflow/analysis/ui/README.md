# UI Analysis

**Methodology version:** 5.2

## Purpose

This folder holds the **visual and interaction half of the conceptual model**:
the surfaces the product presents, the patterns that govern them, the states
each one can be in, and the contracts that must hold between them. It is
written during analysis, from the evidence in `input/ui-ux/`, and it is
finished **before the first User Story is drafted**.

> **What surfaces does the product present, what patterns and states govern
> them, and what contracts must hold between them?**

`domain-model/` and `ui/` are two halves of one conceptual model:
`domain-model/` answers *what things exist and how they relate*; `ui/` answers
*what the user sees and how it behaves*. Both are derived from `input/`, both
are finished before `functional/`, and a User Story that needs either one is
not ready until it exists.

Where `scope/` decides **what** gets built and in which phase, `ui/` documents
**how it is presented** — the catalogue of canonical patterns, the states every
surface can show, and the parity contracts between surfaces when one replaces
another.

---

## What belongs here

| Item | Description | Example |
|------|-------------|---------|
| **Surface inventory** | Every distinct view, dialog and page, enumerated and counted | "34 views, 12 dialogs, 4 wizards across two surfaces" |
| **Pattern gallery** | Canonical UI patterns with *when to use* and *when not to* | Tables, filters, empty states, destructive confirmations |
| **State catalogues** | The states each surface or pattern must present | loading, empty, partial, error, permission-denied |
| **Visual contracts** | Binding description of how one surface must match another — look, behaviour, states | "The successor must reproduce every filter combination the legacy grid accepts" |
| **Parity plans** | How parity between surfaces is reached, and in what order | "Grid first, then bulk actions, then export" |
| **Annotated references** | Wireframes or marked-up screens kept in-repo as analysis, derived from `input/ui-ux/` | An annotated legacy screen naming each region |

---

## What does NOT belong here (boundaries)

| Artifact | Where it lives |
|----------|----------------|
| Raw mockups, screenshots, exported designs | `input/ui-ux/` — human-deposited evidence, **read-only for agents** (§5.6, G31) |
| UI decisions: design system, component library, theming | `adrs/` (`ADR-NNN`) |
| Visual verification contracts | `tests/test-cases/` (`TC-NNN`) |
| Research about people and how they work | `personas/`, `user-journeys/`, `process/` |
| Which UI work lands in which phase | `scope/` |
| The entities and relationships behind the screens | `domain-model/` |
| Component-level implementation detail | `spec/` |
| Plain-language product narrative | `introduction/` |

---

## When to create a UI document

- **During analysis ingestion** — as soon as `input/ui-ux/` holds enough to
  enumerate surfaces. Counting them early is what turns *"we'll rebuild the
  admin"* into a bounded piece of work.
- **Before the first User Story that touches a surface.** A US whose
  acceptance criteria reference a state the catalogue does not define is not
  ready: the state belongs here first.
- **When a surface is being replaced.** The legacy surface is evidence in
  `input/ui-ux/`; the parity contract derived from it is written **before** the
  US that replaces it, not after.
- **When a pattern changes during delivery** — update the gallery or contract
  in the same pass (see *Lifecycle*).

---

## Lifecycle

| State | Meaning |
|-------|---------|
| `draft` | Proposed — not yet validated with the design owner or stakeholders |
| `stable` | Validated — safe to derive US, ACs and TCs from it |
| `deprecated` | No longer valid — the surface it described is gone, or the pattern was abandoned |

These are **living documents**, and what they must stay true to is **the
surface they describe** — during analysis, the evidence in `input/ui-ux/`;
after delivery, the surface that shipped. A gallery describing a pattern
nobody implements anymore is worse than no gallery, so when the UI changes,
the document changes in the same pass.

That obligation does not turn it into a record of the code: it never becomes
the verification contract. **One-way dependency:** a TC may reference a
document here; a document here never replaces a TC.

---

## Governance

Documents here are **analysis documentation**. They may be cited as context in
SPECs, Bolts and ADRs like any other `analysis/` family, but they **carry no
approval**: there is no AITL checkpoint for them, the pre-SPEC evidence gate
does not wait on them (§2.4.1, G13), and they may be written, corrected or
discarded at any time.

---

## Format

- One Markdown file per document, with YAML frontmatter.
- Descriptive **kebab-case** filenames, **no IDs** — the same rule as `scope/`
  and `vision/` (§5.15). If a document starts to behave like a decision or a
  verification contract, it has outgrown this folder: it belongs in `adrs/` or
  `tests/test-cases/`.
- Use **[TEMPLATE-UI.md](TEMPLATE-UI.md)** as starting point.
- Diagrams in **Mermaid** where useful (state charts, surface maps). Embedded
  images are raw material, never a substitute for a required diagram (W08).
- **Language:** schema (frontmatter keys, status enums) in **English**;
  section headings and prose follow the project's `content_language` (§3.15).

---

## How to draft it with AI

1. Feed the agent everything in `input/ui-ux/`, plus the interview transcripts
   that describe how people work on the current surface.
2. Ask in passes:
   - *"Enumerate every distinct surface — view, dialog, page. For each: a
     one-line purpose and the screenshot or file it came from."*
   - *"Group the surfaces by recurring layout. Name each pattern and list the
     surfaces that use it."*
   - *"For each pattern, list the states it must present — loading, empty,
     partial, error, permission-denied — and which surfaces actually show
     each one."*
   - *"Detect divergence: two surfaces that solve the same problem
     differently. Say which one looks canonical and why."*
   - *"For a surface being replaced: list every behaviour of the legacy one
     that must be preserved, and every one that must not."*
3. The analyst splits the output into documents, validates with the design
   owner and stakeholders, and marks status.

The divergence pass is the one worth running first: across dozens of screens
it finds the inconsistencies a human reviewer stops noticing.

---

## Position in the flow

```
input/ui-ux/       ─┐
input/interviews/  ─┴──►  analysis/ui/  ──►  functional/ (US + ACs)
                                │                    │
                          domain-model/              └──►  tests/test-cases/
                       (the other half of the              (may reference here;
                          conceptual model)                 never replaced by it)
```

`ui/` sits between the raw visual evidence and the User Stories. It is what
turns a folder of screenshots into a bounded, named set of surfaces, patterns
and states that an AC can point at.

---

## Index

See **[INDEX.md](INDEX.md)** for the document listing.

---

## Language

YAML keys and status enums stay in **English** (the schema). Section headings
and all prose — descriptions, rationale, findings — go in the project's
`content_language` (see [devflow/README.md](../../README.md) → Language
policy, §3.15).

---

## Feeds the introduction narrative

Once this artifact exists — draft is enough — it feeds
[`../introduction/`](../introduction/), the plain-language entry point written
**last** in the analysis phase. It supplies what the product looks like and
how it behaves.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
