# Scenario → Test Case mapping

How the consolidated scenarios become Test Case documents written from
`devflow/tests/test-cases/TEMPLATE-TC.md`, one per coherent scenario family.

## The bundling rule

- **Family** = scenarios with the same objective, the same preconditions and
  the same main path, differing only in data, equivalence class, boundary value
  or negative counterpart.
- **One Test Case per family.** The main path is the Test Case; every other
  member is an *Alternative / negative path* (§5) or a row of a data table.
- **Split** only when independent outcomes would make a single pass/fail
  ambiguous, or when preconditions or the acting role differ materially.
- Never one Test Case per scenario title; never a Test Case whose expected
  result is not derived from the approved US, ACs and Bolt.

## Field mapping

| Scenario material | Test Case (TEMPLATE-TC.md) |
|-------------------|----------------------------|
| Family objective | Title (the team's convention from the project section of `AGENTS.md`, else a plain descriptive title) and **§1 Objective** |
| The Bolt / US / ACs the family covers | Frontmatter `source_bolt`, `source_us`, `covered_acs`; **§7 Traceability** |
| `type: functional`, `status: draft`, `author` (`human:<user>` — the accountable human, never an agent), `agent` (`agent:<id>` of the agent producing the Test Case), `llm` (its exact model id), `owner` (the QA), `tags` | Frontmatter |
| Precondition (state, data, role) | **§2 Preconditions** |
| Main-path steps — what is done / entered, on which element, what is observed, which validation | **§3 Steps**: one row per step, `Action` and `Expected result` (the outcome **and** what must not happen) |
| State after the test | **§4 Postconditions** |
| Other family members (classes, boundaries, negatives, data sets) | **§5 Alternative / negative paths** — one bullet per variant: `**<variant>** — when … expect …` with its technique; data sets as a table |
| What proves pass / fail | **§6 Pass/fail evidence** |
| Technique and origin | **§7 Traceability**, two lines: `Technique: <name(s)>` · `Origin: heuristic | exploratory | both` |
| Exploratory justification; dependency on an open question | **§8 Notes**: `Justification: …`; `Depends on OQ-NNN: …` |
| Creation | **§9 History** first row |

## The manifest and the index

- Manifest from `devflow/metrics/TEMPLATE-MANIFEST-TC.json`: `tc{id, ref,
  sources (the US and the Bolt), generation, review_ready_at,
  review_started_at}`, `verifies{source_bolt, source_us, covered_acs}`,
  `test_bolts: []`, `checkpoint_approvals: []`. A Test Case without its manifest
  does not exist.
- Row in the **Draft** table of `devflow/tests/test-cases/INDEX.md`:
  `| TC-NNN | <title> | functional | <source Bolt> | <US> / <ACs> |`.

## Export row model (approved Test Cases only)

- One **header row** per Test Case: the generic columns filled, `Step`,
  `Action` and `Expected result` empty.
- Then **one row per step** of §3: `Step` (1, 2, …), `Action`, `Expected
  result`; the other columns empty.
- Each §5 variant with its own steps: an additional row group whose title is
  `<TC id> — <variant>`; a variant without steps becomes one row under the
  Test Case with the variant's `when … expect …` as `Action` / `Expected result`.
- The team's extra columns (from the project section of `AGENTS.md`) are
  appended after the generic ones; none is invented.
