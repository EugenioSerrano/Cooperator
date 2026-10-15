---
phase: "02-DEFENSE"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: "" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
defender_model: "" # Model executing the defense (exact model id) — manually selected by the human (§3.13) — this phase's llm (§3.0)
date: "YYYY-MM-DD"
findings_accepted: 0
findings_rebutted: 0
findings_partial: 0
review_ready_at: "" # When this version is submitted for review (§3.0)
review: # AITL-AREV-DEFENSE-Approval — filled by the human reviewer (§3.0)
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
  headings (##) stay in English (the schema). All prose — arguments,
  evidence, rebuttals — goes in the project's content_language (declared
  in devflow/LANGUAGE).

  ⚠️ This phase CANNOT begin until `AITL-AREV-CRITIQUE-Approval` is
  recorded, and remains DRAFT until `AITL-AREV-DEFENSE-Approval`. Defense
  is an intermediate argument — it does not create usable findings.
-->

# Phase 2 — DEFENSE (Defender)

| Field | Value |
|-------|-------|
| **AREV** | [AREV-NNN — title] |
| **Defender model** | [LLM model defending (typically the implementor)] |
| **Challenger model** | [Model that executed Phase 1] |
| **Critique responded to** | [01-CRITIQUE.md] |

---

## 1. Role mindset — Defender

> **You are the defense attorney for technical decisions.** Your job is not
> to "win" a debate — it's to provide the **context the Challenger didn't
> have** and ensure findings are evaluated with complete information.
>
> ### Attitude
>
> - **Honest before defensive.** If the Challenger found a real bug, accept
>   it without excuses. Saying "yes, they're right" when they're right is a
>   sign of strength, not weakness. The complacency of denying everything is
>   as dangerous as the complacency of accepting everything.
> - **Context is your main weapon.** The Challenger sees the code from the
>   outside. You have access to the history: why that decision was made, what
>   trade-off was evaluated, what framework constraint forced it. Use that
>   context.
> - **Rebut only with evidence.** "I disagree" is not an argument. Cite the
>   ADR that justifies the decision, the documented technical constraint, the
>   SPEC section that permits it, or the framework documentation confirming
>   it's correct.
> - **Accept partially when appropriate.** Often the Challenger is right about
>   the direction but not the severity. Acknowledge what's valid and adjust
>   what doesn't apply.
>
> ### How to evaluate each finding
>
> Ask yourself for each Challenger finding:
>
> 1. **Is the finding correct?** → If yes, ACCEPT without excuses.
> 2. **Are they right but the severity is exaggerated?** → PARTIAL. Explain
>    why the actual severity is different.
> 3. **Are they wrong or missing context?** → REBUT. But only if you have
>    concrete evidence. If you don't, accept.
> 4. **Does the finding reveal a conscious trade-off?** → PARTIAL or REBUT
>    depending on the case. Explain the trade-off and why this option was chosen.
>
> ### Traps to avoid
>
> - **Don't rebut out of pride.** If something is wrong, it's wrong.
> - **Don't invent post-hoc justifications.** If the decision has no ADR or
>   documentation backing it, don't fabricate a justification now.
> - **Don't minimize everything.** If you reduce the severity of every finding,
>   the Judge will notice the pattern and you'll lose credibility.
> - **Don't ignore findings.** Every finding marked as "requires response" in
>   Phase 1 must have an explicit disposition.
>
> ### If the AREV is ad-hoc
>
> When there's no associated Bolt/SPEC, defend based on: active ADRs, team
> conventions, documented design decisions, and known technical constraints
> of the project.

---

## 2. Active mandates

> As Defender, I operate under these constraints:
> 1. **READ-ONLY** — I do not modify code. I only argue about findings.
> 2. **HONESTY** — I must accept valid findings. It's not about "winning"
>    but providing context the Challenger may not have had.
> 3. **EVIDENCE** — Every rebuttal cites ADRs, decisions, constraints or
>    concrete context. "I disagree" without justification is not valid.
> 4. **MANDATORY DISPOSITION** — Every finding receives ACCEPT/REBUT/PARTIAL.
>    No finding can be left without a response.
> 5. **PROPORTIONALITY** — If I accept most and rebut few, that's fine.
>    If I rebut all, I must have exceptionally solid evidence for each one.

---

## 3. Disposition legend

| Disposition | Meaning |
|-------------|---------|
| **ACCEPT** | The finding is correct. I confirm it is a real problem. |
| **REBUT** | The finding is incorrect or does not apply. I explain why with evidence. |
| **PARTIAL** | Part of the finding is valid, but the severity or scope is different. |

---

## 4. Responses to findings

### F-01 — [Original finding title] → **ACCEPT / REBUT / PARTIAL**

**Disposition:** [ACCEPT / REBUT / PARTIAL]

**Argument:**
[Detailed explanation of why it is accepted, rebutted or partially accepted.
If rebutted, cite ADRs, design decisions, technical constraints or context
the Challenger didn't have.]

**Evidence:**
[Concrete references: ADR-NNN, SPEC section, documented decision, etc.]

**Proposed severity:** [Maintain 🔴 / Reduce to 🔶 / Reduce to ⚠️ / Dismiss ✅]

---

### F-02 — [Original finding title] → **ACCEPT / REBUT / PARTIAL**

**Disposition:** [ACCEPT / REBUT / PARTIAL]

**Argument:** [...]

**Evidence:** [...]

**Proposed severity:** [...]

---

## 5. Disposition summary

| # | Finding | Original sev. | Disposition | Proposed sev. |
|---|---------|---------------|-------------|---------------|
| 1 | F-01 | 🔴 | ACCEPT / REBUT / PARTIAL | 🔴 / 🔶 / ⚠️ / ✅ |
| 2 | F-02 | 🔶 | ACCEPT / REBUT / PARTIAL | 🔶 / ⚠️ / ✅ |

---

## 6. Additional context for the Judge

[Any global context the Defender wants to provide to the Judge for Phase 3.
Include:
- Cross-cutting design decisions that affect multiple findings.
- Conscious trade-offs and why they were chosen.
- Known technical limitations of the framework, platform or infrastructure.
- Time, scope or resource constraints that influenced decisions.
- Any ADR, SPEC, or documentation the Challenger may not have consulted.]

---

## 7. Defender reflection

**Findings that surprised me:**
[Did any Challenger finding reveal something you genuinely hadn't considered?
This is valuable for the Judge and for the team.]

**Patterns identified:**
[Do findings point to a recurring pattern? E.g.: "3 of 5 findings are about
error handling — we may need an ADR on error handling strategy."]

---

## 8. AITL-AREV-DEFENSE-Approval

> **Avenga DevFlow §2.15, §3.0.** This phase cannot begin until
> `AITL-AREV-CRITIQUE-Approval` is recorded, and remains a draft until a
> qualified human records `AITL-AREV-DEFENSE-Approval` (recorded in the
> `review` frontmatter block). Only then may Phase 3 (Verdict) begin. AREV
> approvals are recorded only in AREV artifacts — never in the Bolt manifest.
