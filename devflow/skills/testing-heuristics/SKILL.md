---
name: testing-heuristics
description: >
  Use after the story analysis, when no critical open question remains, to
  apply the deterministic test-design techniques systematically — equivalence
  partitioning, boundary value analysis, decision tables, state transition,
  CRUD scenarios, error guessing, roles and permissions, positive/negative
  testing — and produce the base set of scenarios, each traced to the
  technique and the acceptance criteria that originated it.
metadata:
  scope: shared
  origin: "Adapted from TestWriter by Diego Delgado — contributed to Avenga DevFlow on 2026-08-28 under the terms confirmed by the maintainer; translated to English and generalized for any project."
---

# Testing Heuristics

## When to use

- Always **after** `user-story-analysis`, and only when **no critical open
  question** is pending.
- It is the **deterministic** coverage base: it is never skipped and never
  replaced by generative exploration (the `exploratory-scenario-generation`
  skill builds on top of it, not instead of it).
- For one approved Bolt at a time, over that Bolt's `covered_acs`.

## What you need

- The analysis note of the story (explicit + inferred + accepted assumptions)
  — `agents-data/<agent>/<bolt-id>/01-analysis.md` when produced by an
  agent.
- The approved US's acceptance criteria (their `AC-n` identifiers).
- The project's analysis families when a technique needs them:
  `devflow/analysis/glossary/`, `devflow/analysis/personas/` (roles and
  permissions), `devflow/analysis/domain-model/`,
  `devflow/analysis/business-context/`.

## Techniques

Review each technique and apply the ones the story calls for — not all apply
every time. Each technique's detail lives in its own reference file; load it
only when you need the detail:

- [Equivalence Partitioning](references/equivalence-partitioning.md)
- [Boundary Value Analysis](references/boundary-value-analysis.md)
- [Decision Tables](references/decision-tables.md)
- [State Transition Testing](references/state-transition.md)
- [CRUD Scenarios](references/crud-scenarios.md)
- [Error Guessing](references/error-guessing.md)
- [Roles & Permissions](references/roles-permissions.md)
- **Positive / Negative Testing** — for every rule or AC, at least one explicit
  positive and one explicit negative scenario. Transversal to the techniques
  above; no separate reference.

To add a heuristic later: add a reference file and one line to this list.
Nothing else changes.

## Steps

1. For each applicable technique, generate its scenarios.
2. Every scenario states **which technique originated it** — no scenario
   without a technique.
3. Link every scenario to the **acceptance criteria it covers**, using the
   US's own identifiers (`AC-1`, `AC-2`, …).
4. Do not duplicate scenarios across techniques; when two techniques yield the
   same case, keep it under the more specific technique and note it.
5. Add scenarios for multiple accounts, security, UI validation, errors and
   negatives, end-to-end flow and dependent stories **only when the US or the
   analysis families support them**. A missing rule or dependency is a gap —
   record it as an open question (a draft `OQ-NNN` when it concerns the US),
   never invent it.

## What you produce

The **heuristic scenarios** — working data, never a governed artifact:
`agents-data/<agent>/<bolt-id>/02-heuristic-scenarios.md` when produced by
an agent, one entry per scenario. It is the mandatory input of
`exploratory-scenario-generation` (to avoid duplicates) and of
`test-case-consolidation`, which folds the scenarios into `TC-NNN` documents.

```markdown
## <Technique> — <scenario title>
- Covered ACs: <AC-1, AC-2, …>
- Precondition: …
- Steps: each step says what the user does or enters, on which element, what
  they observe, and which navigation, UI or functional validation they perform.
- Expected result: the visible / functional outcome for each step, and what
  must not happen.
```

## References

- [references/equivalence-partitioning.md](references/equivalence-partitioning.md) — valid and invalid classes per input
- [references/boundary-value-analysis.md](references/boundary-value-analysis.md) — the limits of each class
- [references/decision-tables.md](references/decision-tables.md) — combinations of independent conditions
- [references/state-transition.md](references/state-transition.md) — entities with states
- [references/crud-scenarios.md](references/crud-scenarios.md) — entity management stories
- [references/error-guessing.md](references/error-guessing.md) — experience-based common errors
- [references/roles-permissions.md](references/roles-permissions.md) — actors with different access levels
