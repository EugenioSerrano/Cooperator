---
surfaces: []           # The surface(s) this document covers, e.g. ["legacy-admin", "web-app"]
kind: ""               # inventory | gallery | states | contract | parity-plan | reference
date: "YYYY-MM-DD"
author: ""             # human:<user> — the accountable human, never an agent (§3.0)
agent: ""              # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"        # draft | stable | deprecated
sources: []            # input/ui-ux/ files, INT-NNN interview IDs, legacy screens
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  rationale, findings — goes in the project's content_language. See
  devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.

  This template flexes across the document kinds this folder holds (see
  README.md -> What belongs here). Keep the sections the document needs and
  delete the rest — an empty section is noise, not structure. Sections 1, 2
  and 9 are always kept.
-->

# <Document title>

## 1. Summary

[What this document establishes, in 2-4 sentences. If it is a contract, say
what must match what. If it is an inventory, give the counts.]

## 2. Surfaces covered

| Surface | What it is | Evidence |
|---------|-----------|----------|
| [Name] | [One line: the product area it presents] | [`input/ui-ux/<file>`, INT-NNN] |

## 3. Surface inventory

[For `kind: inventory`. Enumerate and count — a bounded list is what turns a
vague rebuild into scoped work.]

| # | Surface | Kind | Purpose | Evidence |
|---|---------|------|---------|----------|
| 1 | [Name] | view \| dialog \| page \| wizard | [One line] | [source] |

**Totals:** [N] views · [N] dialogs · [N] pages · [N] wizards

## 4. Patterns

[For `kind: gallery`. The canonical catalogue. *When not to use* is the half
that prevents drift.]

| Pattern | When to use | When NOT to use | Surfaces using it |
|---------|-------------|-----------------|-------------------|
| [Name] | [Condition] | [Condition + what to use instead] | [List] |

## 5. States

[For `kind: states`. An acceptance criterion cannot be written well against a
surface whose states are undefined — this section is what US and TC authors
point at.]

| Pattern / surface | Loading | Empty | Partial | Error | Permission denied |
|-------------------|---------|-------|---------|-------|-------------------|
| [Name] | [Behaviour] | [Behaviour] | [Behaviour] | [Behaviour] | [Behaviour] |

## 6. Contract

[For `kind: contract`. What must hold between two surfaces. Be specific enough
that a TC author can derive a check from each row, and note that a TC may
reference this — never the reverse (README.md -> Lifecycle).]

| # | The successor must… | Rationale | Verified by |
|---|---------------------|-----------|-------------|
| 1 | [Observable requirement] | [Why it matters] | [TC-NNN, once it exists] |

**Explicitly NOT preserved:**

- [Legacy behaviour that must not carry over, and why]

## 7. Parity plan

[For `kind: parity-plan`. Sequence, not a wish list.]

| Order | Slice | Depends on | Notes |
|-------|-------|-----------|-------|
| 1 | [Slice] | — | [Note] |

## 8. Divergences found

[Two surfaces solving the same problem differently. Name the canonical one —
this is the section the AI divergence pass fills, and the one a human reviewer
stops noticing across dozens of screens.]

| # | Divergence | Surfaces | Canonical choice | Rationale |
|---|-----------|----------|------------------|-----------|
| 1 | [What differs] | [A vs B] | [Which one] | [Why] |

## 9. Sources

| Source | What it contributed |
|--------|--------------------|
| [`input/ui-ux/<file>`] | [What was taken from it] |
| [INT-NNN] | [What the interview established] |

## 10. Open questions

[Unresolved items. Anything that blocks a Bolt's readiness belongs in
`../open-questions/OQ-NNN-<description>.md` instead, where the G35 sunset rule
applies — link it here, never duplicate its text.]

- [Question] → [`OQ-NNN`](../open-questions/)
