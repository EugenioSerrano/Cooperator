---
id: "ADR-NNN"
title: ""
date: "YYYY-MM-DD"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: ""  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft" # draft | accepted | rejected | deprecated | superseded
decision_makers: [] # Roles that participated in the decision
sources: [] # REVs, DISCs, previous ADRs, interviews that motivated this decision
supersedes: [] # ADRs this one replaces (§3.5 conflict resolution may supersede several)
conflicts_with: [] # ADRs whose decisions this ADR contradicts (optional; must be resolved by a superseding ADR before it governs a SPEC — §2.8)
tags: []
nfrs: [] # NFRs governed by this ADR (performance, security, availability, etc.)
waiver: # Only for gate-override ADRs (§3.6)
  gate: "" # Gate being waived
  reason: "" # Why the gate is waived
  owner: "" # Person responsible for the exception
  compensating_control: "" # Control that mitigates the waived gate
  expires_at: "" # YYYY-MM-DD — mandatory, no open-ended waivers
review_ready_at: "" # When this exact version is submitted for review (§3.0)
review: # AITL-ADR-Approval evidence — filled by the human reviewer (§3.0)
  decision: "" # approved | changes_requested | rejected
  reviewers: [] # [{actor, role, model}]
  written_by: [] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: ""
  decided_at: ""
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). ADR titles and prose —
  context, decision, consequences — go in the project's content_language
  (declared in devflow/LANGUAGE). `AITL-ADR-Approval` is never translated.
  See devflow/README.md -> Language policy.

  ⚠️ AITL-ADR-Approval: An ADR remains a DRAFT until an Architect or
  Tech Lead records AITL-ADR-Approval (§2.8, §3.5). A draft ADR cannot
  govern a SPEC, establish an NFR, authorize an exception, or be treated
  as an accepted decision. No approval is inherited from related artifacts.

  ⚠️ ADR CONFLICTS (§2.8): Before approval, check the decision log for
  active ADRs that contradict this one. If any exists, do NOT approve
  this ADR as-is: declare the conflict via `conflicts_with` and create
  the superseding ADR that explicitly overrides them. A SPEC whose
  `sources` include mutually exclusive active ADRs is blocked by the
  pre-SPEC evidence gate (§2.4.1) until the conflict is resolved.

  ⚠️ CONTENT SCOPE (§2.8): An ADR records architecture decisions and
  constraints — it is not a functional specification. Functional behaviour
  and functional requirements live in the feature User Story and its ACs;
  if this document specifies what the system does for the user, it is the
  wrong artifact.
-->

# ADR-NNN — [Descriptive title]

| Field          | Value |
|----------------|-------|
| **Status**     | [draft / accepted / rejected / deprecated / superseded] |
| **Decision-makers** | [team / roles] |
| **Sources**    | [REV-NNN, DISC-NNN, previous ADRs] |
| **Supersedes** | [ADR-NNN it replaces, or "None"] |
| **Conflicts with** | [ADRs contradicted by this decision, or "None" — §2.8] |

---

## 1. Context

[Problem or need that motivated the decision. Include constraints and
forces at play. Reference DISCs and REVs that evidence the problem.]

---

## 2. Alternatives considered

### Alternative A — [Name] (✅ Selected)

| Aspect   | Detail |
|----------|--------|
| **Pros** | [advantages] |
| **Cons** | [disadvantages] |

### Alternative B — [Name]

| Aspect   | Detail |
|----------|--------|
| **Pros** | [advantages] |
| **Cons** | [disadvantages] |

### Alternative C — [Name]

| Aspect   | Detail |
|----------|--------|
| **Pros** | [advantages] |
| **Cons** | [disadvantages] |

---

## 3. Decision

[Clear statement of what was decided. "We adopt Alternative A because…"]

---

## 4. Consequences

**Positive:**
- ...

**Trade-offs:**
- ...

**Technical debt:**
- ...

---

## 5. Applicable NFRs

[If this ADR defines or governs non-functional requirements. NFRs are
defined and governed inside approved ADRs — never in USs, ACs, Bolts or
SPECs (§2.7). Functional requirements are the mirror image: they live in
the feature User Story and its ACs — never in an ADR.]

| NFR | Description | Threshold | How it is measured |
|-----|-------------|-----------|-------------------|
|     |             |           |                   |

---

## 6. References

- [Source 1](url)
- Related ADRs: ADR-XXX

---

## 7. AITL-ADR-Approval

> **AITL-ADR-Approval** (Avenga DevFlow §2.8, §3.0, §3.5). An ADR does not
> become `accepted` — and therefore governing — without the approval of an
> Architect / Tech Lead. This ADR is the **source of truth for its own
> approval** (recorded in the `review` frontmatter block with review
> evidence); when it governs a SPEC revision, its path appears in that
> revision's `sources`. ADR approvals are never copied to the Bolt manifest.

| Field | Value |
|-------|-------|
| **Architect / Tech Lead** | `human:<user>` (git-email local part) or `agent:<id>` — actor grammar (§3.0) |
| **Role** | architect / tech_lead |
| **Decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings or acknowledged_without_comment + reason] |
