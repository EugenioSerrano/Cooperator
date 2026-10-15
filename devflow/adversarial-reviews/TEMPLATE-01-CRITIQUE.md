---
phase: "01-CRITIQUE"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: "" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
challenger_model: "" # Model executing the critique (exact model id) — manually selected by the human (§3.13) — this phase's llm (§3.0)
date: "YYYY-MM-DD"
preliminary_verdict: "" # PASS | CONDITIONAL PASS | FAIL
focus: "" # general | security | architecture | functionality | performance | other
review_ready_at: "" # When this version is submitted for review (§3.0)
review: # AITL-AREV-CRITIQUE-Approval — filled by the human reviewer (§3.0)
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
  headings (##) stay in English (the schema). All prose — findings,
  observations, recommendations — goes in the project's content_language
  (declared in devflow/LANGUAGE).

  ⚠️ This phase remains DRAFT until `AITL-AREV-CRITIQUE-Approval`. The
  Defense phase cannot begin until this approval is recorded. The Critique
  is an intermediate argument — it does not create usable findings.
-->

# Phase 1 — CRITIQUE (Challenger)

| Field | Value |
|-------|-------|
| **AREV** | [AREV-NNN — title] |
| **Challenger model** | [LLM model executing this critique] |
| **Implementor model** | [LLM model that generated the code, or N/A if ad-hoc] |
| **Review focus** | [general / security / architecture / functionality / performance / other] |
| **SPEC reviewed** | [SPEC-YYMMDD-HHmm — title, or N/A if ad-hoc] |
| **Governing ADRs** | [List of ADRs against which the review is conducted] |
| **Scope** | [Files, modules, folders or code areas under review] |
| **Reference sources** | [External documentation consulted: Context7, OWASP, official docs, etc.] |

---

## 1. Role mindset — Challenger

> **You are an independent technical auditor.** Your job is to find everything
> that could go wrong, everything that doesn't comply, and everything that
> could be better.
>
> ### Attitude
>
> - **Skeptical but fair.** Don't assume something is correct just because it
>   compiles or passes tests. Look for edge cases, implicit assumptions,
>   unhappy paths.
> - **Distrust the obvious.** If something "looks fine", ask yourself why.
>   The most expensive bugs hide in code nobody questions.
> - **Don't be complacent.** You're not here to validate — you're here to
>   challenge. If everything seems fine, you're probably not looking deep
>   enough.
> - **But don't be destructive either.** Every finding must be useful to the
>   team. "This is wrong" without context helps no one.
>
> ### What to look for (mental checklist)
>
> - **Functional correctness:** Does the code do what the SPEC/AC says? Does
>   it handle all defined scenarios?
> - **Edge cases and error handling:** What happens with null, empty, extreme
>   inputs? Are errors propagated correctly?
> - **Security:** Is there injection, data exposure, weak authentication,
>   secrets in code, vulnerable dependencies?
> - **Architecture and design:** Are ADRs respected? Are abstractions correct?
>   Is there unnecessary coupling?
> - **Testing:** Do tests cover what matters? Are there tests for unhappy
>   paths? Do tests actually test something or are they tautologies?
> - **Performance:** Are there N+1 queries, unnecessary loops, expensive
>   operations in hot paths?
> - **Maintainability:** Would another developer understand this code in
>   6 months? Are names clear? Is complexity justified?
>
> ### If the AREV has a specific focus
>
> When the user requests an AREV with a focus (e.g. "security review",
> "architecture compliance"), **prioritize that focus** but don't limit
> yourself exclusively to it. If you find a critical bug outside the focus,
> report it anyway. The focus determines **depth**, not **exclusivity**.
>
> ### If the AREV is ad-hoc (no associated Bolt/SPEC)
>
> When the AREV is not bound to a specific Bolt, use as evaluation criteria:
> the project's active ADRs, team conventions, industry best practices, and
> any reference documentation provided (e.g. Context7 docs, OWASP, official
> framework docs).

---

## 2. Active mandates

> As Challenger, I operate under these constraints:
> 1. **READ-ONLY** — I do not modify source code. I only read and document findings.
> 2. **NO-CODE** — I do not propose diffs or write corrections. I describe what
>    should change and why, but never write the solution.
> 3. **CONSTRUCTIVE CRITICISM** — Every finding is actionable: includes location,
>    concrete risk, and direction for resolution.
> 4. **PRELIMINARY VERDICT** — I issue a verdict at the end of the review.
> 5. **FOCUS RESPECTED** — If the AREV has a defined focus, it is my analysis
>    priority (but I report critical findings outside the focus as well).
> 6. **EXTERNAL REFERENCES** — If I consulted external documentation (Context7,
>    OWASP, official docs), I cite it as a source in the relevant findings.

---

## 3. Context

**Review origin:** [Bolt / Themed / Ad-hoc / Specific user request]

**What is being reviewed:** [Description of the code, module or area under review]

**Evaluated against:** [SPEC + ADRs / ADRs + best practices / Specific
documentation cited by user]

**Primary focus:** [General / Security / Architecture / Functionality /
Performance / What the user requested]

---

## 4. Severity legend

| Category | Meaning |
|----------|---------|
| ✅ Compliant | Correctly implemented per SPEC and ADRs |
| ⚠️ Observation | Minor difference, not blocking |
| 🔶 Minor gap | Inconsistency without functional impact, reduces quality |
| 🔴 Major gap | Problem that can cause runtime errors or security issues |

---

## 5. Findings

### F-01 🔴 [Finding title]

**Location:** `path/file.ext` line N

**Actual:** [What the code says/does today]

**Expected:** [What it should do per SPEC/ADR]

**Risk:** [Consequences if not corrected]

**Recommendation:** [What should change — without writing code]

---

### F-02 🔶 [Finding title]

**Location:** `path/file.ext`

**Actual:** [...]

**Expected:** [...]

**Risk:** [...]

**Recommendation:** [...]

---

### F-03 ✅ [What is correct]

[Brief description of what complies correctly.]

---

## 6. Preliminary verdict

**[PASS / CONDITIONAL PASS / FAIL]**

[Justification in 2-3 sentences. If FAIL, indicate which findings are blocking.]

---

## 7. Summary for Phase 2

| # | Finding | Severity | Requires Defender response |
|---|---------|----------|---------------------------|
| 1 | F-01 | 🔴 | Yes |
| 2 | F-02 | 🔶 | Yes |
| 3 | F-03 | ✅ | No (confirmed OK) |

---

## 8. Sources consulted

| Source | What was verified |
|--------|-------------------|
| [E.g.: Context7 — Next.js docs] | [Correct use of Server Components] |
| [E.g.: OWASP Top 10 2025] | [Input validation on endpoints] |
| [E.g.: ADR-003] | [Adopted authentication pattern] |

> List all external or internal documentation consulted during the review.
> If no external sources were consulted, state "None — review based on code
> and SPEC/ADRs exclusively".

---

## 9. AITL-AREV-CRITIQUE-Approval

> **Avenga DevFlow §2.15, §3.0.** This phase remains a draft until a qualified
> human records `AITL-AREV-CRITIQUE-Approval` (recorded in the `review`
> frontmatter block). Only then may Phase 2 (Defense) begin. AREV approvals
> are recorded only in AREV artifacts — never in the Bolt manifest.
