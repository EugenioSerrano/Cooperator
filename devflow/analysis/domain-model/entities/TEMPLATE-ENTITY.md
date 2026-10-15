---
entity: "EntityName"
label: "Human-readable name"
module: "module-or-bounded-context"
status: "draft"          # draft | stable | deprecated
date: "YYYY-MM-DD"
author: ""               # human:<user> — the accountable human, never an agent (§3.0)
agent: ""                # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                  # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
sources: []              # INT-NNN, documentation references
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  properties, business rules — goes in the project's content_language.
  See devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

# EntityName

## 1. Description

> Brief description of what this entity represents in the business domain.
> Include enough context for a business person to recognize the concept.

## 2. Properties

| Property | Type | Required | Constraints | Description |
|----------|------|:--------:|-------------|-------------|
| `property1` | string             | ✅ | max 50 chars     | Description of the property |
| `property2` | integer            | ✅ | PK               | Description of the property |
| `property3` | decimal            | ❌ | ≥ 0              | Description of the property |
| `property4` | boolean            | ✅ | —                | Description of the property |
| `property5` | dateTime           | ❌ | ISO 8601         | Description of the property |
| `property6` | → Enum:`EnumName`  | ✅ | see `../enumerations/EnumName.md` | Description of the property |

## 3. Relationships

| Relationship | Target | Cardinality (this — target) | Description |
|--------------|--------|:---------------------------:|-------------|
| `relationName`  | TargetEntity   | 1 — 0..N | Description |
| `otherRelation` | OtherEntity    | 0..N — 1 | Description |

> Mirror these rows in the appropriate module file under
> [`../relationships/`](../relationships/) (use `TEMPLATE-RELATIONSHIP.md` to create one).

## 4. Business rules

- **RULE-01:** Invariant or rule that this entity must satisfy.
- **RULE-02:** Another relevant business rule.

> Numbered **per entity** (`RULE-NN`, restarting at 01 in each file) — not to be
> confused with the repository-wide Business Risks `BR-NNN` of
> [`../../business-risks/`](../../business-risks/).

## 5. Example

```yaml
property1: "example value"
property2: 42
property3: 1500.00
property4: true
property5: "2026-01-15T10:30:00Z"
property6: "ENUM_VALUE"
```

## 6. Sources

| Source | Where |
|--------|-------|
| INT-NNN | `../../../input/interviews/INT-NNN.md` |
| Doc    | `../../../input/documentation/...`     |

## 7. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial draft | @user |
