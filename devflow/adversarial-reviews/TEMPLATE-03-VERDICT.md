---
phase: "03-VERDICT"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: "" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
judge_model: "" # Model that arbitrates (exact model id) — manually selected by
                # the human (§3.13). Must differ from the implementor's AND the Challenger's model.
                # An AREV requires at least three models; there is no human-arbiter fallback (§3.13, §3.15).
                # This field is this phase's llm (§3.0).
date: "YYYY-MM-DD"
final_verdict: "" # PASS | CONDITIONAL PASS | FAIL
findings_confirmed: 0
findings_dismissed: 0
findings_reclassified: 0
review_ready_at: "" # When this version is submitted for review (§3.0)
review: # AITL-AREV-VERDICT-Approval — filled by the human reviewer (§3.0)
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
  headings (##) stay in English (the schema). All prose — analysis,
  resolutions, justifications — goes in the project's content_language
  (declared in devflow/LANGUAGE).

  ⚠️ This phase CANNOT begin until `AITL-AREV-DEFENSE-Approval` is
  recorded. It remains DRAFT until `AITL-AREV-VERDICT-Approval`. ONLY an
  approved Verdict produces actionable findings; downstream artifacts
  follow their own lifecycle and AITL approval. AREV approvals and the
  Verdict are recorded ONLY in AREV artifacts — never in the Bolt manifest.
-->

# Phase 3 — VERDICT (Judge)

| Field | Value |
|-------|-------|
| **AREV** | [AREV-NNN — title] |
| **Judge model** | [LLM model that arbitrates] |
| **Challenger model** | [Model that executed Phase 1] |
| **Defender model** | [Model that executed Phase 2] |
| **Documents evaluated** | 01-CRITIQUE.md, 02-DEFENSE.md |

---

## 1. Role mindset — Judge

> **You are the final arbiter. Your verdict is what the human will read to
> make decisions.** You are neither the accuser nor the defender — you are
> the impartial judge who weighs evidence and delivers a clear, actionable
> result.
>
> ### Attitude
>
> - **Impartial but not passive.** Don't just average opinions. Evaluate the
>   quality of arguments. A rebuttal without evidence is worth less than a
>   well-documented finding. A vague finding is worth less than a defense
>   citing an ADR.
> - **Prioritize the human.** Your output is the document the dev-validator
>   will read. It must be **clear, concise and actionable**. The human should
>   not need to read the other 2 documents unless they want to dig into a
>   specific disputed point.
> - **Be definitive.** Every finding must have a clear resolution: CONFIRMED,
>   DISMISSED or RECLASSIFIED. "It depends" is not a verdict.
> - **Detect patterns.** If 4 of 6 findings are about the same topic (e.g.
>   security, error handling), mention it. That's a systemic signal worth
>   more than the individual findings.
>
> ### How to evaluate the debate
>
> For each disputed finding, ask yourself:
>
> 1. **Did the Challenger cite concrete evidence?** (location, risk,
>    SPEC/ADR reference, external documentation)
> 2. **Did the Defender respond with evidence or just opinion?** A rebuttal
>    backed by an ADR carries more weight than "I disagree".
> 3. **Who has the burden of proof?** Generally, if code diverges from the
>    SPEC or an ADR, the burden is on the Defender to justify it. If the
>    Challenger flags something not in the SPEC, the burden is on the
>    Challenger.
> 4. **Is the severity proposed by each side proportional?** Sometimes both
>    are partially right — the finding exists but the severity is different.
> 5. **Did the Defender rebut everything?** If the Defender rejected all
>    findings, treat each rebuttal with extra skepticism — it may be a
>    defensive pattern rather than honest argumentation.
>
> ### Verdict criteria
>
> - **PASS:** No confirmed findings with severity 🔴 or 🔶. Everything is
>   at ✅ or ⚠️ at most.
> - **CONDITIONAL PASS:** There are confirmed 🔶 findings but no 🔴.
>   The human decides whether to fix before or after.
> - **FAIL:** At least one confirmed 🔴 finding. Requires correction before
>   continuing.
>
> ### Ad-hoc AREVs
>
> When there's no Bolt/SPEC, evaluate findings against: active ADRs, team
> conventions, industry best practices, and any reference sources cited in
> the Critique.

---

## 2. Active mandates

> As Judge, I operate under these constraints:
> 1. **READ-ONLY** — I do not modify code. I only arbitrate and consolidate.
> 2. **IMPARTIALITY** — I evaluate arguments from both sides without bias.
>    Quality of evidence weighs more than quantity of words.
> 3. **FINAL VERDICT** — Every disputed finding receives a definitive
>    resolution and final severity. "It depends" is not a verdict.
> 4. **ACTIONABLE PLAN** — The verdict includes a clear action plan for the
>    human with concrete destinations (BUG/BOLT→SPEC/DISC/ADR/RISK).
> 5. **SYNTHESIS** — My document must be readable standalone. The human should
>    not need to read Critique + Defense to understand my conclusions.

---

## 3. Findings evaluation

### F-01 — [Finding title]

| Aspect | Detail |
|--------|--------|
| **Challenger sev.** | 🔴 |
| **Defender disposition** | ACCEPT / REBUT / PARTIAL |
| **Defender proposed sev.** | 🔶 |

**Debate analysis:**
[Impartial evaluation of arguments from both sides. Did the Challenger
identify a real problem? Did the Defender provide valid context that changes
the assessment? Is the cited evidence solid?]

**Resolution:** [CONFIRMED / DISMISSED / RECLASSIFIED]

**Final severity:** [🔴 / 🔶 / ⚠️ / ✅]

**Justification:** [Why it is confirmed, dismissed or reclassified. 1-2 sentences.]

---

### F-02 — [Finding title]

| Aspect | Detail |
|--------|--------|
| **Challenger sev.** | 🔶 |
| **Defender disposition** | ACCEPT / REBUT / PARTIAL |
| **Defender proposed sev.** | ⚠️ |

**Debate analysis:** [...]

**Resolution:** [CONFIRMED / DISMISSED / RECLASSIFIED]

**Final severity:** [...]

**Justification:** [...]

---

## 4. Resolution summary

| # | Finding | Challenger sev. | Defender disposition | Judge resolution | Final sev. |
|---|---------|-----------------|---------------------|-----------------|------------|
| 1 | F-01 | 🔴 | REBUT | CONFIRMED / DISMISSED / RECLASSIFIED | 🔴 / 🔶 / ⚠️ / ✅ |
| 2 | F-02 | 🔶 | ACCEPT | CONFIRMED | 🔶 |

---

## 5. Final verdict

**[PASS / CONDITIONAL PASS / FAIL]**

[Justification in 3-5 sentences. Synthesize the debate result:
- How many findings were confirmed vs. dismissed.
- Whether confirmed findings are blocking or not.
- How effective the implementor's defense was.
- General recommendation for the human dev-validator.]

---

## 6. Action plan for the dev-validator

> Applies only after `AITL-AREV-VERDICT-Approval`. Each destination follows
> its own lifecycle and AITL approval (e.g. a code-related outcome requires
> an approved Bolt).

| # | Finding | Final sev. | Recommended action | Destination |
|---|---------|------------|-------------------|-------------|
| 1 | F-01 | 🔴 | [Fix / Investigate / Document] | BUG / BOLT→SPEC / DISC / ADR / RISK |
| 2 | F-02 | 🔶 | [Fix / Defer / Accept risk] | BOLT→SPEC / RISK |

---

## 7. Dismissed findings (record)

| # | Finding | Original sev. | Reason for dismissal |
|---|---------|---------------|---------------------|
| — | — | — | — |

> Dismissed findings are recorded here for audit purposes.
> They generate no action, but document what was evaluated and why it was dismissed.

---

## 8. Judge observations

**Patterns detected:**
[Do findings point to a systemic problem? E.g.: "3 findings are related to
input validation — I suggest an ADR on validation strategy for the entire
project."]

**Debate quality:**
[Was the Challenger rigorous or superficial? Did the Defender provide
valuable context or was it defensive without substance? This helps the team
calibrate confidence in future AREVs with the same models.]

**Recommendations for future reviews:**
[Are there code areas that would merit a dedicated AREV? Does any finding
suggest an ADR, DISC, or convention change is needed?]

---

## 9. AITL-AREV-VERDICT-Approval

> **Avenga DevFlow §2.15, §3.0.** This phase cannot begin until
> `AITL-AREV-DEFENSE-Approval` is recorded, and remains a draft until a
> qualified human records `AITL-AREV-VERDICT-Approval` (recorded in the
> `review` frontmatter block). **Only an approved Verdict produces
> actionable findings.** AREV approvals and the Verdict are recorded only
> in AREV artifacts — never in the Bolt manifest.
