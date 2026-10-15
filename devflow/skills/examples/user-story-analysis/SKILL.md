---
name: user-story-analysis
description: >
  Use when a User Story (with its acceptance criteria) must be evaluated for
  quality and testability before any test design: what is explicit, what is
  inferred, what is missing, what is only assumed, and which open questions
  block the work. Reads the approved US and Bolt plus the project's analysis
  families; never fills a gap by inventing a rule.
metadata:
  scope: shared
  origin: "Adapted from TestWriter by Diego Delgado — contributed to Avenga DevFlow on 2026-08-28 under the terms confirmed by the maintainer; translated to English and generalized for any project."
---

# User Story Analysis

## When to use

- Always the **first step** of test design for a User Story — before any
  heuristic or exploratory technique (the `testing-heuristics` skill comes
  next).
- Run it **for one approved Bolt at a time**: the story is the approved US, the
  slice is the Bolt's `covered_acs`, and the moment is after that Bolt's
  readiness approval and before its SPEC exists. Never for a Bolt that is not
  approved.
- Run it again when the US or its ACs change, or when previously open
  questions are answered.

## What you need

- The **approved User Story** and its acceptance criteria (the `AC-n`
  identifiers as the US writes them), and the **approved Bolt** it runs for
  (its `covered_acs`).
- The project's analysis families, read **before** marking anything as
  missing so that project-level knowledge is not reported as a gap:
  `devflow/analysis/glossary/` (terms), `devflow/analysis/personas/` (actors,
  roles and permissions), `devflow/analysis/domain-model/` (entities,
  enumerations, relationships), `devflow/analysis/business-context/` (stable
  rules and constraints).
- The project section of the root `AGENTS.md` (project conventions such as
  naming or export formats).
- Nothing else: no external tracker, no folder of your own — if something is
  not in the governed artifacts above, it is a gap, not a lookup.

## Steps

1. Read the whole US, its ACs and the analysis families named above.
2. Classify every relevant aspect — actors, roles and permissions,
   preconditions, postconditions, states, dependencies, data, error handling,
   business rules — into **exactly one** of these categories:
   - **Explicit** — written literally in the US or its ACs.
   - **Inferred** — deducible with reasonable confidence from the US plus the
     analysis families, but not stated; say where it is inferred from.
   - **Missing** — neither explicit nor safely inferable.
   - **Assumption** — something you would assume to move on, which must be
     confirmed; never treat it as final.
   - **Open question** — a concrete doubt the QA takes to the Functional
     Analyst, PO or stakeholder.
3. Mark every open question **critical** (it blocks test design — a central
   business rule is missing, two ACs contradict each other) or **non-critical**
   (a detail; design can proceed on a documented assumption).
4. Evaluate the acceptance criteria: are they verifiable? Do they cover
   negative cases? Are any ambiguous or mutually contradictory?
5. **Never** fill a gap by inventing rules, roles, states or data. A gap
   becomes an open question.
6. Route the gaps the governed way: each open question that concerns the US
   becomes a **draft `OQ-NNN`** in `devflow/analysis/open-questions/`
   targeting the US (search the OQ index first; link, never duplicate). If any
   question is **critical**, **stop here** and present it — do not continue to
   heuristics or consolidation until it is answered.

## What you produce

- The **analysis note** — working data, never a governed artifact: when run by
  a DevFlow Agent it goes to `devflow/agents-data/<agent>/<bolt-id>/01-analysis.md`;
  a human keeps it as session notes. It is the mandatory input of the
  `testing-heuristics` skill.
- The **draft `OQ-NNN` documents** for the open questions (the governed
  output of this step).

```markdown
# Story analysis — <US id> · <title> — for <Bolt id> (ACs: <covered_acs>)

## Explicit information
...

## Inferred information
- <fact> — inferred from: <source>

## Missing information
- <aspect not covered>

## Assumptions (not confirmed)
- <assumption>

## Open questions
### Critical (block test design) — each routed to a draft OQ
- <question> → OQ-<NNN>

### Non-critical
- <question> (→ OQ-<NNN> when it concerns the US)

## Acceptance criteria assessment
- <clarity, verifiability, negative coverage, contradictions>
```

## References

- None — the procedure is self-contained. Its output feeds the
  `testing-heuristics` skill.
