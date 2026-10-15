---
id: "UAT-NNN"
date: "YYYY-MM-DDTHH:mm:ss±HH:MM"  # point-in-time event: start of the UAT session
author: ""                # human:<user> — the accountable human, never an agent (§3.0)
agent: ""                 # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                   # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
unit: ""                  # Unit / Milestone validated
facilitator: ""           # human:<user> (git-email local part) | agent:<id> — actor grammar (§3.0)
status: "draft"           # draft | approved | approved-with-observations | rejected
bolts: []                 # Bolts covered
adrs: []                  # related ADRs
linked_vision_outcomes: [] # outcomes from vision/ this UAT validates
linked_processes: []      # PROC-NNN this UAT exercises
review_ready_at: ""       # When this version is submitted for review (§3.0)
review: # AITL-UAT-Approval — filled by the stakeholders (§3.0)
  decision: "" # approved | changes_requested | rejected
  reviewers: [] # [{actor, role, model}]
  written_by: [] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: ""
  decided_at: ""
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: ""
---

> ⛔ **DORMANT / RESERVED (v4.2).** The UNIT/UAT approval-and-release layer was
> **removed from the active flow in v4.2** — `AITL-UAT-Approval` is **not an
> active checkpoint** in this release. The governed flow ends at Bolt
> acceptance (`AITL-BOLT-DONE-Approval`). This template is kept **dormant** for
> a redesigned model planned in a future version. The frontmatter and
> body below describe the future (reserved) UAT process, not an active gate.

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — observations,
  feedback, sign-off notes — goes in the project's content_language
  (declared in devflow/LANGUAGE).

  ⚠️ AITL-UAT-Approval (§3.0): stakeholders sign off the Unit/Milestone.
  The UAT minutes in tests/uat/ are the evidence — the Bolt manifest
  carries NO UAT data (§3.12).

  ⚠️ Review contract (§3.0): review.decision uses the universal enum
  approved | changes_requested | rejected. The document status
  approved-with-observations is a UAT lifecycle label, not a decision
  value: it means review.decision: approved with non-empty findings[] —
  every finding routes to a new Bolt (section 4 below).
-->

# UAT-NNN — [Unit / Milestone]

## 1. Scope validated

[Summary of what this UAT session covers — which Bolts, which user-visible
behaviour, which process flows.]

## 2. Signing stakeholders

| Name | Role | Signature | Date |
|------|------|-----------|------|
| @user |     |           |      |

## 3. Results per criterion (business ACs)

| AC    | Description | Result        | Evidence | Observation |
|-------|-------------|---------------|----------|-------------|
| AC-01 |             | [ok] / [warn] / [fail] |          |             |
| AC-02 |             | [ok] / [warn] / [fail] |          |             |

> Each AC must trace to a vision outcome or process rule. List the link in
> the *Observation* column when not obvious.

## 4. Agreed adjustments (derived Bolts)

- [ ] US-NNN.BOLT-NNN — [adjustment]
- [ ] US-NNN.BOLT-NNN — [adjustment]

## 5. Final decision

- [ ] Approved
- [ ] Approved with observations (Bolts listed above)
- [ ] Rejected — reason: [...]

## 6. AITL-UAT-Approval

> **Avenga DevFlow §3.0.** Stakeholders sign off the Unit/Milestone here and
> in the `review` frontmatter block. The UAT minutes are the evidence —
> the Bolt manifest deliberately carries no UAT data (§3.12).

| Field | Value |
|-------|-------|
| **review.reviewers** | [stakeholders / customer] |
| **review.written_by** | [optional — who wrote this record: the reviewer, or the agent that transcribed the decision; beside the reviewers, never among them — writing is not signing (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings or acknowledged_without_comment + reason] |

## 7. Sources

| Source | Where |
|--------|-------|
| Vision outcomes | `../../analysis/vision/vision.md` *(from TEMPLATE-VISION.md)* |
| Processes      | `../../analysis/process/PROC-NNN.md` |
| Bolts          | `../../functional/bolts/US-NNN.BOLT-NNN-<description>.md` |
| V-Bounce evidence | `../../memory/MEM-YYMMDD-HHmm-<description>.md` |
