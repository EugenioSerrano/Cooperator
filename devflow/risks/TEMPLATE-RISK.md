---
id: "RISK-NNN"
title: ""
date: "YYYY-MM-DD"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: ""  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
category: "" # technical | integration | security | performance | team | process
probability: "" # high | medium | low
impact: "" # critical | high | medium | low
overall_severity: "" # low | medium | high | critical (probability × impact)
status: "open" # open | mitigated | materialized | closed
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — descriptions,
  mitigation plans — goes in the project's content_language (declared in
  devflow/LANGUAGE).
-->

# RISK-NNN — [Descriptive title]

| Field           | Value |
|-----------------|-------|
| **Category**    | [technical / integration / security / performance / team / process] |
| **Probability** | [🔴 High / 🟡 Medium / 🟢 Low] |
| **Impact**      | [🔴 Critical / 🔴 High / 🟡 Medium / 🟢 Low] |
| **Overall severity** | [low / medium / high / critical (probability × impact)] |
| **Status**      | [open / mitigated / materialized / closed] |

---

## 1. Description

[What could happen and in which context. Clear risk description.]

---

## 2. Analysis

[Justification for the assigned probability and impact. Evidence.]

---

## 3. Mitigation plan

[Proactive actions to reduce probability or impact.]

1. [Action 1]
2. [Action 2]

---

## 4. Contingency plan

[What to do if the risk materializes.]

1. [Reactive action 1]
2. [Reactive action 2]

---

## 5. Impact on Bolt risk class (§3.3)

If this risk applies to a specific Bolt or set of Bolts, it influences the
Bolt's `risk_class`, assigned during **`AITL-BOLT-READY-Approval`** (as part of its
DoR) and recorded in the **Bolt frontmatter**. `risk_class` may be escalated
at any subsequent review (QA/Sec); it can **never be reduced after the first
MEM approval** without formal re-review and re-approval of the Bolt; every
reassignment appends to `risk_history` (§3.3, GUARDRAILS W14). The manifest
does not duplicate risk data (§3.12):

| Bolt risk_class | Default autonomy | Min approvers at AITL-MEM-Approval | AREV |
|-----------------|-----------------|-------------------------------------|------|
| low        | L3 (Autonomous) | 1 (executing Dev-validator) | optional |
| medium     | L3 (Autonomous) | 1 (executing Dev-validator) | optional |
| high       | L2 (Bounded)    | 1 (the executing Dev-validator; QA/Sec optional) | optional |
| critical   | L1 (Suggest)    | 1 (the executing Dev-validator; QA/Sec optional) | optional |

> AREV is optional for all risk classes — stakeholder-triggered, never
> automatic (§2.15).

---

## 6. Relations

- **Related DISCs:** [links]
- **Related ADRs:** [links]
- **Related REVs:** [links]
- **Related BUGs:** [links]
- **Related INCs:** [links if risk materialized]
