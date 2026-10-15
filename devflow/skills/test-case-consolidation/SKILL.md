---
name: test-case-consolidation
description: >
  Use as the last step of the pipeline to merge the deterministic (heuristic)
  and exploratory scenarios, remove duplicates and equivalents, map every case
  to the acceptance criteria, record the technique and origin of each, detect
  coverage gaps, and produce the Test Cases: draft TC-NNN documents from the
  project's TC template — one per coherent scenario family, variants bundled —
  each with its manifest, presented for approval. Optionally projects approved
  Test Cases to a generic CSV for the team's test-management tool.
metadata:
  scope: role
  origin: "Adapted from TestWriter by Diego Delgado — contributed to Avenga DevFlow on 2026-08-28 under the terms confirmed by the maintainer; translated to English and re-targeted from a CSV deliverable to DevFlow Test Case documents."
---

# Test Case Consolidation

## When to use

- Always at the **end** of the pipeline, after `testing-heuristics` and
  `exploratory-scenario-generation`, for one approved Bolt at a time.
- Only when **no critical open question** is pending on the story: a blocking
  gap stops this step before any Test Case is written.

## What you need

- `agents-data/<agent>/<bolt-id>/02-heuristic-scenarios.md` and
  `03-exploratory-scenarios.md`.
- The approved US's acceptance criteria (their `AC-n` identifiers) and the
  approved Bolt (its `covered_acs`) — from `01-analysis.md` or the US itself.
- The project's Test Case contract: `devflow/tests/test-cases/TEMPLATE-TC.md`,
  `devflow/tests/test-cases/INDEX.md` (where the next `TC-NNN` is claimed),
  `devflow/metrics/TEMPLATE-MANIFEST-TC.json`.
- The open-questions family: `devflow/analysis/open-questions/TEMPLATE-OQ.md`
  and its `INDEX.md`.
- The project section of the root `AGENTS.md` for the team's conventions
  (Test Case title format, export columns).

## Steps

1. **De-duplicate.** Compare the scenarios of `02` and `03` (and within each)
   for exact or functionally equivalent duplicates. Keep the more specific
   version, discard the other, and record which technique stays associated.
2. **Map to acceptance criteria.** Every scenario names the ACs it covers
   (the US's own identifiers). A scenario that maps to no explicit AC is kept
   and marked "domain-derived risk" — never discarded for that reason alone.
3. **Mark technique and origin.** Every scenario carries the technique that
   originated it (e.g. Boundary Value Analysis, Error Guessing, Exploratory)
   and its origin: `heuristic` or `exploratory`.
4. **Group into scenario families and bundle.** A **family** is the set of
   scenarios that share the same objective, the same preconditions and the
   same main path and differ only in data, equivalence class, boundary value or
   negative counterpart. **One Test Case per family**: the main path becomes the
   TC's steps, the other members become its *Alternative / negative paths* and
   data tables. Split a family only when independent outcomes would make
   pass/fail ambiguous, or when preconditions or roles differ materially. Never
   one Test Case per scenario title. The rules are in
   [references/tc-mapping.md](references/tc-mapping.md).
5. **Detect coverage gaps.** List the acceptance criteria with no Test Case and
   the relevant condition combinations left uncovered.
6. **Route the gaps.** A gap that is a story problem — a missing rule, a
   contradiction, an undefined outcome — becomes a **draft `OQ-NNN`** in
   `devflow/analysis/open-questions/` targeting the US (search the OQ index
   first; link, never duplicate). If the gap is **blocking** (a Test Case
   cannot state its expected result from the approved intent), **stop here**
   and present it — write no Test Case for that family. A non-blocking gap is
   recorded in the affected Test Case's *Notes* as a dependency on the `OQ-NNN`.
7. **Suggest a prioritization** only with the information available: cases
   critical to the main flow versus lower-impact ones. Never invent business
   criteria that are not documented; the suggestion lives in the coverage
   note, not in the Test Case.
8. **Write the Test Cases** as described below, then the coverage note.

## What you produce

### Primary output — draft Test Cases (governed artifacts)

For each scenario family, one **`TC-NNN-<slug>.md`** in
`devflow/tests/test-cases/`, written from
`devflow/tests/test-cases/TEMPLATE-TC.md`, plus its **manifest**
`devflow/metrics/test-cases/TC-NNN-<slug>.json` from
`devflow/metrics/TEMPLATE-MANIFEST-TC.json`, and its row in the **Draft**
table of `devflow/tests/test-cases/INDEX.md` (the next free `TC-NNN` is claimed
there). The mapping of scenario fields to the template's sections is in
[references/tc-mapping.md](references/tc-mapping.md). Expected results derive
from the **approved intent** — US, ACs and Bolt — never from the
implementation. The Test Cases are `status: draft`: **you present them for
`AITL-TC-Approval`; you never approve them.**

### Working data (never citable)

`agents-data/<agent>/<bolt-id>/04-coverage-gaps.md`:

```markdown
# Coverage gaps — <US id> · <Bolt id>

## Acceptance criteria with no Test Case
- <AC id> — <why it is uncovered, or what is missing to cover it> (→ OQ-<NNN> when it is a story gap)

## Relevant combinations / conditions left uncovered
- <description>

## Suggested prioritization (from documented information only)
- High: <list>
- Medium: <list>
- Low: <list>
```

### Optional — CSV export projection of approved Test Cases

When the team imports Test Cases into a test-management tool, project the
**approved** Test Cases (never drafts) to a CSV following the static schema in
[assets/tc-export-projection.csv](assets/tc-export-projection.csv): one header
row per Test Case, then one row per step; each *Alternative / negative path*
that has its own steps is an additional row group titled `<TC id> — <variant>`.
The generic columns are fixed; the team's extra columns and its title
convention are read from the project section of the root `AGENTS.md` — if the
project declares none, emit the generic columns only; never invent a value.
The file goes to `agents-data/<agent>/<bolt-id>/exports/` (derivative
working data). The CSV is a **view** of the Test Cases: when they disagree, the
Test Case is right and the CSV is regenerated.

**Nothing a run produces is written inside `devflow/skills/`** — this folder
holds instructions and static material only; the asset is read, never written.

## References

- [references/tc-mapping.md](references/tc-mapping.md) — scenario → Test Case
  mapping, the bundling rule, the export row model
- [assets/tc-export-projection.csv](assets/tc-export-projection.csv) — the
  generic export schema (header + one placeholder row); static, read-only
