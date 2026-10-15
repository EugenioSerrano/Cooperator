# Scope

**Methodology version:** 5.2

## Purpose

This folder holds **scope and phasing decisions** — the bridge between the
high-level product vision and the detailed User Stories in `functional/`.

While `vision/` answers **what product we want to build** in 1–3 sentences
(plus a brief "Scope at a glance"), `scope/` answers:

> **What exactly are we building NOW, what comes LATER, and what is explicitly
> OUT — and why?**

Scope decisions emerge during analysis: as you interview stakeholders, model
the domain, map user journeys and define business processes, you discover
boundaries that need to be recorded. A scope document captures those
boundaries with rationale, so the team — and future readers — know not just
*what* was decided, but *why*.

---

## What belongs here

| Item | Description | Example |
|------|-------------|---------|
| **Phases / milestones** | Grouped scope by delivery phase | MVP, v1, v2, backlog |
| **In-scope items** | Features, entities, integrations, channels included in a phase | "User registration + basic dashboard in MVP" |
| **Out-of-scope items** | Explicitly excluded from a phase, with rationale | "Multi-tenant: excluded from MVP because early adopters are single-org" |
| **Deferred items** | Not in current phase, but planned for a later one | "Push notifications: v2" |
| **Scope decisions** | Individual decisions with rationale, stakeholders, and impact | "Admin panel is read-only in v1 to reduce build complexity" |
| **Dependencies between phases** | What must be delivered before another phase can start | "v2 reporting depends on v1 data model stabilisation" |

---

## Scope vs. other artifacts (boundaries)

| Artifact | What it holds | How scope relates |
|----------|---------------|-------------------|
| **`vision/` §5** | Product-level scope at a glance (3–5 bullets) | Vision gives the *direction*; scope gives the *detail*. Vision §5 links here. |
| **`open-questions/`** | Unresolved questions and assumptions | If we don't know whether something is in scope → OQ. Once decided → scope. |
| **`functional/`** | User Stories + Bolts | Each in-scope item becomes one or more US/Bolts. `scope/` feeds `functional/`. |
| **`adrs/`** | Architectural decisions | If a scope decision has architectural impact (e.g. "we'll use event sourcing for Phase 2") → ADR. Scope doc links to the ADR. |
| **`business-context/`** | Market, stakeholders, compliance, business risks | Business constraints (e.g. "GDPR applies") may drive scope decisions. Cross-link. |
| **`risks/`** | Project/technical risk register | Cutting something from scope may introduce or mitigate a risk. Scope doc references RISK-NNN. |

---

## When to create a scope document

- **During analysis ingestion** — when an interview surfaces a clear boundary
  (*"We don't need X for launch"*), record it here immediately instead of
  letting it float in notes.
- **At analysis closure** — consolidate all scope decisions into a
  milestone-level document (e.g. `mvp-scope.md`) before writing User Stories.
- **When scope changes** — if a stakeholder revisits a decision, update the
  document and record the change in the *History* table. A material scope
  change may require an ADR.

---

## Lifecycle

| State | Meaning |
|-------|---------|
| `draft` | Proposed scope, not yet validated with stakeholders |
| `stable` | Validated — safe to derive US/Bolts from it |
| `superseded` | Replaced by a newer scope document (link to successor) |

Scope documents evolve across phases. An MVP scope document may be `stable`
while the v2 scope document is still `draft`. This is normal — each phase
has its own lifecycle.

---

## Format

- One Markdown file per scope document, with YAML frontmatter.
- Recommended file per major milestone: `mvp-scope.md`, `v1-scope.md`,
  `v2-scope.md`.
- Use **[TEMPLATE-SCOPE.md](TEMPLATE-SCOPE.md)** as starting point.
- Diagrams in **Mermaid** where useful (e.g. phase timeline, dependency map).
- **Language:** schema (frontmatter keys, status enums, IDs) in
  **English**; section headings and prose follow the project's
  `content_language`. See
  [`../../LANGUAGE` → *Language declaration*](../../LANGUAGE) and Avenga DevFlow
  **§3.15**.

---

## Position in the flow

```
vision/  ──►  scope/  ──►  functional/ (US + Bolts)
  │                          │
  └── "Scope at a glance"    └── each in-scope item → one or more US/Bolts
      (summary, links here)
```

Scope sits between the directional product vision and the concrete User
Stories. It is the **translation layer** that turns *"we want a dashboard"*
into *"MVP dashboard includes charts A, B, C; filters X, Y; but NOT
export-to-CSV (v2) or custom dashboards (backlog)"*.

---

## Index

See **[INDEX.md](INDEX.md)** for the scope document listing.

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
**last** in the analysis phase. It supplies what is in and out of the milestone.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
