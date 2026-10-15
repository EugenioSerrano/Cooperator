---
module: ""                # Bounded context or module name (e.g. "billing", "logistics")
date: "YYYY-MM-DD"
author: ""                # human:<user> — the accountable human, never an agent (§3.0)
agent: ""                 # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                   # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"           # draft | stable | deprecated
sources: []               # INT-NNN, documentation refs, entity files
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — relationship
  descriptions, business rules — goes in the project's content_language.
  See devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

# Relationships — [Module name]

## 1. ER Diagram

```mermaid
erDiagram
    ENTITY_A ||--o{ ENTITY_B : "has"
```

## 2. Relationship Catalog

| Source | Target | Cardinality (source — target) | Description | Business rule |
|--------|--------|-------------------------------|-------------|---------------|
| Entity A | Entity B | 1 — 0..N | ... | ... |

## 3. Notes

<!-- Additional context, constraints, or design decisions about these relationships. -->

## 4. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Created | ... |
