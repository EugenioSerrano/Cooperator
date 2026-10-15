---
title: ""
version: "1.0"
date: "YYYY-MM-DD"
author: ""           # human:<user> — the accountable human, never an agent (§3.0)
agent: ""            # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""              # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"      # draft | stable | superseded
horizon: "12-18 months"
sponsor: ""          # executive sponsor / product owner
sources: []          # interview IDs, document refs the vision was built on
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums and IDs stay in English
  (the schema). Section headings (##) and all prose go in the project's
  content_language. See devflow/README.md -> Language policy.
-->

# Product Vision — [Product name]

## 1. Vision statement

<!--
  Classic elevator-pitch (Geoffrey Moore template). Translate the
  placeholder prose into the project's content_language when
  instantiating this template.
-->

> For **[target user]** who **[underserved need]**, **[product name]** is a
> **[category]** that **[key benefit]**.
> Unlike **[main alternative]**, we **[differentiator]**.

## 2. Desired outcomes

What changes in the world if this product succeeds? Outcomes, not features.

| # | Outcome | Signal we'll watch | Baseline | Target |
|---|---------|--------------------|----------|--------|
| O1 |        |                    |          |        |
| O2 |        |                    |          |        |
| O3 |        |                    |          |        |

## 3. Anti-goals

What we explicitly do **not** want to be or do.

- AG1 — …
- AG2 — …
- AG3 — …

## 4. Tentative success metrics

Candidates for NSM / KPIs. Refined later in
`business-context/success-metrics.md`.

| Metric | Definition | Tentative target |
|--------|------------|------------------|
|        |            |                  |

## 5. Scope at a glance

- **In scope (v1):** …
- **Out of scope (v1):** …
- **Possibly later:** …

## 6. Open questions

[Unresolved items. Anything that blocks a Bolt's readiness belongs in
`../open-questions/OQ-NNN-<description>.md` instead, where the G35 sunset rule
applies — link it here, never duplicate its text.]

- [Question] → [`OQ-NNN`](../open-questions/)

## 7. Sources

| Source | Where |
|--------|-------|
| Interview INT-NNN | `input/interviews/INT-NNN.md` |
| Exec briefing | `input/documentation/...` |

## 8. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial version | @user |
