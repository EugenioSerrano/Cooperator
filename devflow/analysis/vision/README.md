# Vision

**Methodology version:** 5.2

## Purpose

This folder holds the **product vision**: the short, directional statement
that answers *"what are we building, for whom, and why does it matter?"*. It
is the most stable document in the project and acts as an anchor for every
derived artifact (business-context, domain-model, US, ADRs, Specs).

The vision is **not** a backlog or a spec. It is **intent**, in 1–3 sentences,
plus an initial set of tentative success metrics that get refined as domain
knowledge grows.

> See Avenga DevFlow §2.2 *Intent*.

---

## What goes here

- **Vision statement** — 1–3 sentences. Classic elevator-pitch structure
  (adapt connectors to the project's `content_language`):
  > EN: *For `<user>` who `<need>`, `<product>` is a `<category>` that `<key benefit>`. Unlike `<alternative>`, we `<differentiator>`.*
  > ES: *Para `<usuario>` que `<necesidad>`, `<producto>` es una `<categoría>` que `<beneficio>`. A diferencia de `<alternativa>`, `<diferenciador>`.*
- **Desired outcomes** — what changes in the world if this works (NOT features).
- **Anti-goals** — what we explicitly do NOT want to be / do.
- **Tentative success metrics** — NSM and KPI candidates (refined later in
  `business-context/success-metrics.md`).
- **Scope at a glance** — brief in/out summary (3–5 bullets). The detailed
  scope decisions per phase/milestone live in [`../scope/`](../scope/).
- **Time horizon** — typical 12–18 months.

---

## Position in the flow

```
input/ (interviews, exec briefs)
  → vision/  → business-context/  → domain-model/ + process/  → functional/
                  │
                  └── scope/ (detailed phasing — bridges vision §5 to functional/)
```

Every User Story / Bolt should trace back to a vision outcome. If a US
contributes to no outcome it is a candidate to be cut from scope **or** to
trigger a vision update (with an ADR). Detailed scope decisions per phase
are recorded in [`../scope/`](../scope/).

---

## Format

- **One file**: `vision.md`, with YAML frontmatter.
- When a structural change happens, the document is **replaced as a whole by
  a numbered successor** (the `version` frontmatter key, §3.15/§5.7) and the
  previous version moves to `_archive/` (§5.4) — history is never deleted.
  Substantive changes are logged as an ADR.
- Diagrams in **Mermaid**.

Use [TEMPLATE-VISION.md](TEMPLATE-VISION.md) as starting point.

---

## How to draft it with AI

1. Feed the agent the founders/exec interview transcripts and any
   pitch/briefing documents from `input/`.
2. Ask for:
   - 3 alternative vision statements (different tones / abstractions).
   - Top 5 candidate outcomes with measurable signals.
   - Top 5 anti-goals derived from constraints mentioned in interviews.
   - A brief "Scope at a glance" for the first milestone (in/out/possibly-later).
     Save the detailed scope decisions in [`../scope/`](../scope/) using
     [TEMPLATE-SCOPE.md](../scope/TEMPLATE-SCOPE.md).
3. The human picks/edits, validates with the executive sponsor, and freezes
   v1.

---

## Operating notes

- **Very brief.** If it exceeds one page, it is poorly written.
- **Business language**, no technical jargon.
- Validate with stakeholders (record those sessions in
  [`../../input/interviews/`](../../input/interviews/)).

---

## Index

See **[INDEX.md](INDEX.md)** for the listing of vision documents (current and
historical).

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../README.md) -> Language policy, §3.15).

---

## Feeds the introduction narrative

Once this artifact exists — draft is enough — it feeds
[`../introduction/`](../introduction/), the plain-language entry point written
**last** in the analysis phase. It supplies the "what we are building" section.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
