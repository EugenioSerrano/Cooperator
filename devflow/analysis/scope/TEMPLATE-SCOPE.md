---
milestone: ""          # MVP | v1 | v2 | backlog | custom label
version: "1.0"
date: "YYYY-MM-DD"
author: ""             # human:<user> — the accountable human, never an agent (§3.0)
agent: ""              # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"        # draft | stable | superseded
replaces: ""           # If superseding a previous scope document, link it here
sources: []            # Interview IDs, vision doc, stakeholder meetings
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  rationale, decisions — goes in the project's content_language. See
  devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

# Scope — [Milestone / Phase name]

## 1. Summary

> One paragraph: what this milestone delivers, for whom, and the strategic
> rationale behind the scope decisions. Reference the vision outcome(s) this
> milestone advances.

**Linked vision outcomes:** O1, O2, …

## 2. In scope (this milestone)

What is included, with rationale. Be specific — avoid vague terms.

| # | Item | Description | Rationale | Linked artifact |
|---|------|-------------|-----------|-----------------|
| S1 |      |             |           |                 |
| S2 |      |             |           |                 |
| S3 |      |             |           |                 |

<!--
  Linked artifact column: reference the entity, process, journey, persona, or
  ADR that backs this item. Example: "entity/order.md", "PROC-003", "ADR-012".
-->

## 3. Out of scope (this milestone)

What is explicitly excluded, with the reason. This is just as important as
the "in" list — it prevents scope creep and sets stakeholder expectations.

| # | Item | Reason for exclusion | Revisit at | Linked artifact |
|---|------|---------------------|------------|-----------------|
| X1 |      |                     |            |                 |
| X2 |      |                     |            |                 |
| X3 |      |                     |            |                 |

<!--
  "Revisit at" column: which milestone or trigger would cause re-evaluation.
  "Linked artifact": if an ADR or RISK explains the exclusion, link it.
-->

## 4. Deferred (planned for later)

Items acknowledged as valuable but intentionally postponed to a future phase.

| # | Item | Target milestone | Rationale for deferral |
|---|------|-----------------|------------------------|
| D1 |      |                  |                        |
| D2 |      |                  |                        |

## 5. Phase dependencies

What must be complete before this milestone can start, and what downstream
milestones depend on this one.

```mermaid
flowchart LR
    subgraph Before["Prerequisites"]
        P1[Item / milestone]
        P2[Item / milestone]
    end
    subgraph Current["This milestone"]
        C1[{{ milestone }}]
    end
    subgraph After["Depends on this"]
        A1[Item / milestone]
        A2[Item / milestone]
    end
    P1 --> C1
    P2 --> C1
    C1 --> A1
    C1 --> A2
```

| Dependency | Type | Detail |
|------------|------|--------|
|             | **Blocks this** / **Blocked by this** |         |

## 6. Scope decisions log

Individual decisions made during analysis that shaped this scope. Each
decision records what was chosen, what was discarded, and why.

| # | Decision | Alternatives considered | Rationale | Decided by | Date |
|---|----------|------------------------|-----------|------------|------|
|   |          |                        |           |            |      |

<!--
  If a decision has architectural impact, promote it to an ADR and link here.
-->

## 7. Impact assessment

What changes elsewhere as a result of these scope decisions.

| Area | Impact |
|------|--------|
| **Personas** | <!-- e.g. "Field technician persona not served until v2" --> |
| **User journeys** | <!-- e.g. "Refund journey deferred to v2" --> |
| **Domain model** | <!-- e.g. "Multi-org entity excluded from MVP" --> |
| **Processes** | <!-- e.g. "Approval workflow simplified in v1" --> |
| **Risks** | <!-- e.g. "Deferring audit trail increases RISK-004" --> |

## 8. Open questions

<!--
  Questions about this scope that are not yet resolved.
  For analysis-phase questions, create an OQ in open-questions/ and link it.
  Only keep a brief pointer here.
-->

- [ ] …
- [ ] …

## 9. Sources

| Source | Where |
|--------|-------|
| Vision | `../vision/vision.md` §5 |
| Interview INT-NNN | `../../input/interviews/INT-NNN.md` |

## 10. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial version | @user |
