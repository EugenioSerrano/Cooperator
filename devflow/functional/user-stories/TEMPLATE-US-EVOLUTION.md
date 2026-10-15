---
id: "US-NNN"
title: ""
date: "YYYY-MM-DD"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: ""  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft" # draft | approved | deprecated
owner: "" # Functional Analyst (governs this US)
unit: "" # Unit or epic (e.g. "1 — Payments Module")
evolves: "" # ⚠️ MANDATORY HERE — the delivered US whose behaviour this story changes
            # (US-MMM). The delivered US is NEVER reopened, edited or deprecated
            # by default: `done` is terminal for its Bolts (§3.15). This field is what makes the
            # chain traversable when the older document is archived.
            # Also list it in `sources:` — that is what the manifest carries.
story_points: null # Fibonacci 1|2|3|5|8|13 — the complexity of the DELTA ONLY,
                   # never of the whole feature restated below, and never time.
                   # Proposed by the agent, confirmed at AITL-US-Approval (§2.6).
adrs: [] # Related ADRs
sources: [] # evidence — the delivered US (US-MMM), input/ refs, REV-NNN, interviews (required, min 1)
stakeholders: []
tags: []
review_ready_at: "" # When this version is submitted for review (§3.0)
review: # AITL-US-Approval — filled by the human reviewer (§3.0)
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
  LANGUAGE POLICY (§3.15): YAML keys, status enums and IDs stay in English.
  Section headings and all prose go in the project's content_language
  (declared in devflow/LANGUAGE). `AITL-*-Approval` codes are never
  translated.

  ⚠️ AITL-US-Approval (§2.6, §3.0): a feature US remains DRAFT until a
  Functional Analyst records AITL-US-Approval. Its Bolts may be
  drafted and refined while it is DRAFT; none may be approved before it is.

  ⚠️ Manifest v5 (§3.12, G33): create its manifest JSON in
  devflow/metrics/user-stories/ with `story_points` = the delta. The
  `evolves:` value goes in the manifest's `us.sources` — there is no
  manifest field for it, and the schemas reject unknown keys.

  ⚠️ THIS FILE IS A WORKED EXAMPLE. The content below is a complete,
  realistic evolution story: replace it with yours, keeping the shape and
  the four rules stated in section 1.
-->

> **Which template is this?** Use **this** one when the behaviour you are
> changing is **already delivered**. Use
> [`TEMPLATE-US.md`](TEMPLATE-US.md) when the functionality is **new**.
> Not sure? A defect in what was delivered is a **BUG**, not a story; a change
> with no behaviour difference (refactor, infra, hardening, debt) is a
> **non-functional Bolt** under `US-000`. See `../README.md`.

> **Naming convention (mandatory):** Files go in `user-stories/` with the
> format `US-NNN-<description>.md`. An evolution story is an ordinary feature
> US in every governed respect — same numbering, same lifecycle, same manifest.

# US-NNN — Annulment of confirmed purchase orders with double signature

| Field          | Value |
|----------------|-------|
| **Unit**       | [Unit / Epic] |
| **Evolves**    | **US-MMM** (delivered) — never reopened, never edited |
| **ADRs**       | [links to applicable ADRs] |
| **Status**     | [draft / approved / deprecated] |
| **Story points** | [the delta only — 3 in this example] |

---

**As a** procurement operator, **I want** to annul a CONFIRMED purchase order
before it goes into transit, giving a mandatory reason and the signature of two
different users, **so that** a mistaken confirmation can be undone without
losing the trace of who authorized it and why.

> **Context:** evolves **US-MMM** (delivered). The order lifecycle
> `IN_NEGOTIATION → CONFIRMED → IN_TRANSIT → RECEIVED` is deployed, with
> cancellation available from several states and a double-signature mechanism
> already used for approvals. This story adds **only** the annulment of
> CONFIRMED orders and reuses that mechanism.

## 1. Acceptance criteria — the complete target behaviour

> **How this section works — four rules.**
> 1. **State the whole feature as it must work when this story is done** —
>    what already exists *and* what changes. The delivered story may already be
>    archived, so this document has to stand on its own.
> 2. **Mark every criterion** `(existing — US-MMM)` or `(delta)`.
> 3. **Anything you cannot restate unchanged in substance is `delta`** — not
>    `existing`. If restating it made you hesitate, it is delta.
> 4. **Only `delta` criteria produce Bolts and story points.** The `existing`
>    ones are completeness, and legitimate regression scope for Test Cases;
>    they are never re-delivered.
>
> **How much to restate.** One condensed Given/When/Then line per delivered
> criterion. If the complete behaviour would exceed **15 criteria**, group the
> `existing` ones by capability — one line per group, naming the delivered US —
> and keep every `delta` criterion in full. The target is a document a reader
> can hold in one sitting.
>
> **If this story and the delivered one ever read differently:** this one is
> the behaviour **in force**; the delivered US remains the record of what was
> approved at its own moment and is never rewritten to match.

- `AC-1` *(existing — US-MMM)* — **Given** a purchase order in negotiation,
  **When** the operator confirms it with an authorized supplier and amount,
  **Then** it moves to CONFIRMED and its reference exchange rate is frozen.
- `AC-2` *(existing — US-MMM)* — **Given** a CONFIRMED order, **When** goods
  are dispatched, **Then** it moves to IN_TRANSIT and can no longer be edited.
- `AC-3` *(existing — US-MMM)* — **Given** an order in any state before
  transit, **When** an operator cancels it, **Then** it moves to CANCELLED and
  the reason is recorded.
- `AC-4` *(delta)* — **Given** a CONFIRMED order, **When** the operator starts
  an annulment with a mandatory reason, **Then** it moves to
  ANNULMENT_PENDING and is blocked for transit and reception until the
  signatures are complete.
- `AC-5` *(delta)* — **Given** a started annulment, **When** two signatures
  from two different users are collected, **Then** the order moves to
  ANNULLED, the reference exchange rate is unfrozen, and the user, date and
  reason are written to the audit trail.
- `AC-6` *(delta)* — **Given** an annulment with a single signature, **When**
  anyone tries to move the order to IN_TRANSIT, **Then** the transition is
  rejected (fail-closed).
- `AC-7` *(delta)* — **Given** an order already IN_TRANSIT or later, **When**
  an annulment is attempted, **Then** it is rejected: annulment applies only to
  CONFIRMED orders, and the cancellation of `AC-3` covers the other states.
- `AC-8` *(delta)* — **Given** an operator without the annulment permission,
  **When** they start one, **Then** the request is refused with the
  authorization error the project uses.
- `AC-9` *(delta)* — **Given** an annulled order with confirmation evidence
  attached, **When** the annulment completes, **Then** the evidence is
  preserved and marked in the audit trail — never deleted.

> ACs are verifiable functional criteria only — non-functional constraints
> live in ADRs (§2.7).

## 2. Bolts — the delta only

| # | Bolt | Type | Layer | Description | Est. active delivery |
|---|------|------|-------|-------------|----------------------|
| 1 | `BOLT-001` (`../bolts/US-NNN.BOLT-001-annulment-backend.md`) | functional | Backend | State machine for the annulment, the two-signature count, the audit entry and the endpoint (`AC-4`–`AC-7`, `AC-9`) | 3h |
| 2 | `BOLT-002` (`../bolts/US-NNN.BOLT-002-annulment-frontend.md`) | functional | Frontend | Annulment panel (reason + signatures) in the order detail, new state and badge (`AC-4`, `AC-5`, `AC-8`) | 2h |

> **No Bolt covers an `existing` criterion.** They are already delivered; a
> Bolt that claimed one would re-deliver approved work.
>
> **Plausibility (§2.6):** 3 SP (the delta) → 2 Bolts ✓.

## 3. Business rules

| # | Rule | Condition | Action | |
|---|------|-----------|--------|---|
| BR1 | Confirmation freezes the reference exchange rate | order confirmed | freeze the rate | *(existing — US-MMM)* |
| BR2 | Annulment only from CONFIRMED | state = CONFIRMED | allow starting the annulment | *(delta)* |
| BR3 | Two signatures from different users | signature requested | count signatures; one per user, reusing the approval constraint | *(delta)* |
| BR4 | Reason mandatory | starting an annulment | reject without a reason | *(delta)* |
| BR5 | Transitions blocked while pending | state = ANNULMENT_PENDING | reject transit and reception | *(delta)* |
| BR6 | Evidence preserved | annulment completed | keep files and links; record in the audit trail | *(delta)* |

## 4. User flows

[Main and alternative sequences of the **complete** behaviour — a diagram is
usually the clearest way to show that the new states belong to a lifecycle that
already existed. Mark the new states or transitions.]

## 5. Impact

- **Affects:** [the modules the delta touches].
- **Depends on:** [the delivered mechanisms it reuses, and the ADRs that govern them].
- **Not affected:** [state it explicitly — this is what lets a reviewer size
  the risk without reading the code].
- **The delivered US (`US-MMM`) is not modified.** It stays as the record of
  what was approved then. It is marked `deprecated` **only** if this story
  *withdraws* its behaviour rather than extending it.

## 6. SDLC tool alignment

[Associated Work Items, Sprint, Board — if the team uses an external tool.]

## 7. AITL-US-Approval

> **Avenga DevFlow §2.6, §3.0.** This story remains a draft until a Functional
> Analyst records `AITL-US-Approval` in the `review` frontmatter block,
> confirming that the complete target behaviour is stated, that every criterion
> is correctly marked `existing` or `delta`, and that `story_points` score the
> delta only. Its Bolts (section 2) may be drafted and refined while
> it is draft; none may be approved before it is.

## 8. Manifest creation (mandatory)

> ⚠️ **MANDATORY** — copy
> [`TEMPLATE-MANIFEST-US.json`](../../metrics/TEMPLATE-MANIFEST-US.json) to
> `devflow/metrics/user-stories/US-NNN-<description>.json`, with
> `story_points` = the delta and the delivered US listed in `us.sources`. It
> must validate against
> [`manifest-v5-us.schema.json`](../../metrics/manifest-v5-us.schema.json):
> under **G33**, a US without a valid manifest does not exist. There is no
> manifest field for `evolves:` — the schemas reject unknown keys.
