---
id: ""              # BR-NNN (auto-assigned from INDEX)
title: ""
date: "YYYY-MM-DD"
author: ""          # human:<user> — the accountable human, never an agent (§3.0)
agent: ""           # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""             # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"     # draft | stable | deprecated
category: ""        # market | regulation | adoption | business-model
sources: []         # INT-NNN interview IDs, document refs
tags: []
---

<!--
  LANGUAGE POLICY: YAML keys, status enums and IDs stay in English (the
  schema). Section headings (##) and all prose — risk descriptions, impact
  analysis — go in the project's content_language.
  See devflow/README.md -> Language policy. `AITL-*-Approval` codes are never translated.
-->

# BR-NNN — [Title]

## 1. Description

[What is the risk? In what context could it materialize?]

## 2. Category

`market` | `regulation` | `adoption` | `business-model`

## 3. Assessment

| Dimension | Value | Rationale |
|-----------|-------|-----------|
| Likelihood | `low` / `medium` / `high` | [Why this rating] |
| Impact     | `low` / `medium` / `high` | [Why this rating] |

## 4. Mitigation

[Actions to reduce likelihood or impact before the risk materializes.]

## 5. Contingency

[What to do if the risk materializes despite mitigation.]

## 6. Sources

- INT-NNN (stakeholder interviews)
- Compliance documents in `input/business/compliance/`

## 7. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Created | ... |
