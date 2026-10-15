---
topic: ""           # stakeholders | market | competitors | compliance | business-model | success-metrics
date: "YYYY-MM-DD"
author: ""          # human:<user> — the accountable human, never an agent (§3.0)
agent: ""           # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""             # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"     # draft | stable | deprecated
sources: []         # interview IDs, document refs
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  context, rationale, placeholder text — goes in the project's
  content_language. See devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

# [Topic] — [Short title]

## 1. Summary

[One paragraph: what this document covers and why it matters now.]

## 2. Content

> Pick the structure that fits the topic. Examples below.

### Stakeholders example

| Stakeholder | Role (sponsor/user/impacted/regulator) | Influence (H/M/L) | Interest (H/M/L) | Key concern |
|-------------|----------------------------------------|:----------------:|:----------------:|-------------|
|             |                                        |                  |                  |             |

### Success metrics example

| Metric | Linked vision outcome | Definition | Baseline | Target | Owner |
|--------|-----------------------|------------|----------|--------|-------|
|        |                       |            |          |        |       |

### Compliance example

| Regulation | Scope | Applies because… | Key obligation | Linked ADR/RISK |
|------------|-------|------------------|----------------|------------------|
|            |       |                  |                |                  |

## 3. Sources

| Source | Where |
|--------|-------|
| INT-NNN | `input/interviews/INT-NNN.md` |
| Doc    | `input/documentation/...`     |

## 4. Open questions / Follow-ups

[Unresolved items. Anything that blocks a Bolt's readiness belongs in
`../open-questions/OQ-NNN-<description>.md` instead, where the G35 sunset rule
applies — link it here, never duplicate its text.]

- [Question] → [`OQ-NNN`](../open-questions/)

## 5. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial version | @user |
