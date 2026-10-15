---
name: exploratory-scenario-generation
description: >
  Use after the testing heuristics, never before or instead of them, to
  explore generatively what the deterministic techniques did not reach: edge
  and corner cases, unexpected interactions, rare sequences, unprioritized
  condition combinations, error scenarios an experienced tester would try,
  and risks from the project's domain. Always reads the heuristic scenarios
  first to avoid duplicates; never invents rules the story does not support.
metadata:
  scope: role
  origin: "Adapted from TestWriter by Diego Delgado — contributed to Avenga DevFlow on 2026-08-28 under the terms confirmed by the maintainer; translated to English and generalized for any project."
---
<!-- projected from devflow/skills/exploratory-scenario-generation/ — generated, do not edit; fix the skill and reinstall -->

# Exploratory Scenario Generation

## When to use

- Always **after** `testing-heuristics`, never before it and never instead of
  it. This is the **generative** pass of the pipeline: it complements the
  deterministic coverage, it does not replace it.
- For one approved Bolt at a time, over that Bolt's `covered_acs`.

## What you need

- The story analysis note (explicit + inferred + accepted assumptions) —
  `agents-data/<agent>/<bolt-id>/01-analysis.md` when produced by an agent.
- The heuristic scenarios — `agents-data/<agent>/<bolt-id>/02-heuristic-scenarios.md`.
  **Read it completely before proposing anything**, to avoid duplicates.
- The project's analysis families for known domain risks (fragile
  integrations, rules historically prone to defects): `devflow/analysis/business-context/`,
  `devflow/analysis/domain-model/`, `devflow/analysis/personas/`,
  `devflow/analysis/glossary/`; and the project section of the root `AGENTS.md`.

## What to look for

- Edge and corner cases not covered by the equivalence classes or boundaries
  already defined.
- Unexpected interactions between parts of the story, or with related
  functionality.
- Rare usage sequences (going back, cancelling mid-flow, repeating an action
  quickly).
- Condition combinations the decision tables of the deterministic pass did
  not prioritize.
- Error scenarios and functional inconsistencies an experienced tester would
  try even though they are not documented explicitly.
- Risks derived from the project context (business domain, known
  integrations).

## Steps

1. Read `02-heuristic-scenarios.md` in full before proposing anything new.
2. For every new scenario, verify explicitly that it is not equivalent to one
   already listed; if it is a close variant, state how it differs.
3. Give every scenario a short justification — why an experienced tester would
   try it (not "just in case").
4. Never relabel a heuristic scenario as exploratory, nor the reverse.
5. Prioritize additional risks of multiple accounts, security, UI, errors and
   negatives, end-to-end flow and dependent stories **when the US or the
   analysis families support them**. If a behavior is not defined, document
   the gap — it becomes an open question (a draft `OQ-NNN` when it concerns the
   US) — without assuming the behavior.

## What you produce

The **exploratory scenarios** — working data, never a governed artifact:
`agents-data/<agent>/<bolt-id>/03-exploratory-scenarios.md` when produced
by an agent, one entry per scenario. It is an input of
`test-case-consolidation`. Nothing is written inside `devflow/skills/`.

```markdown
## Exploratory — <scenario title>
- Justification: <why it is worth testing>
- Covered ACs: <AC-1, AC-2, … or "none explicit — domain-derived risk">
- Precondition: …
- Steps: each step says what the user does or enters, on which element, what
  they observe, which visual / functional / navigation validation they perform,
  and which condition must not occur.
- Expected result: the observable outcome and the forbidden behavior.
```

## References

- None — the procedure is self-contained. Its output feeds
  `test-case-consolidation`.
