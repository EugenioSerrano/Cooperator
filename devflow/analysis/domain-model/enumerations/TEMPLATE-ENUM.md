---
enum: "EnumName"
label: "Human-readable name"
module: ""           # bounded context / module — or "general"
date: "YYYY-MM-DD"
author: ""           # human:<user> — the accountable human, never an agent (§3.0)
agent: ""            # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""              # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"      # draft | stable | deprecated
sources: []          # INT-NNN, documentation references
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  value definitions — goes in the project's content_language. See
  devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

# EnumName

## 1. Description

> What this enumeration represents in the business domain and when it is
> used.

## 2. Values

| Value      | Description                |
|------------|----------------------------|
| `VALUE_1`  | Description of value 1     |
| `VALUE_2`  | Description of value 2     |

## 3. Used by

| Entity | Property |
|--------|----------|
| `EntityName` | `propertyName` |

## 4. Notes

- *(Optional: legacy code mapping, sort order, extensibility rules,
  terminal vs. transitional states, etc.)*

## 5. Sources

| Source  | Where |
|---------|-------|
| INT-NNN | `../../../input/interviews/INT-NNN.md` |

## 6. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial draft | @user |
