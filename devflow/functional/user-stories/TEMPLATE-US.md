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
story_points: null # Fibonacci 1|2|3|5|8|13 — relative functional complexity
                   # (ACs, rules, flows, integrations, unknowns), NEVER time.
                   # Proposed by the agent, confirmed at AITL-US-Approval (§2.6).
                   # Informational only: no gate, no velocity, no hour conversion.
                   # Scoring: use the §2.6 rubric — score the HIGHEST dimension,
                   # never the average; compare against approved USs in INDEX.md
                   # when they exist; open OQs targeting this US = unknowns.
                   # 13 = splitting signal: propose decomposition before approval.
adrs: [] # Related ADRs
sources: [] # evidence the US was derived from — input/ refs, REV-NNN, AREV-NNN, interviews (required, min 1)
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
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — descriptions,
  acceptance criteria, scenarios — goes in the project's content_language
  (declared in devflow/LANGUAGE).
  `AITL-*-Approval` codes are never translated.

  ⚠️ AITL-US-Approval (§2.6, §3.0): a feature US remains DRAFT until a
  Functional Analyst records AITL-US-Approval. Its functional
  Bolts may be drafted and refined while it is DRAFT; none may be approved
  before it is. US-000 is outside this lifecycle. Approval is never
  inherited from related artifacts.

  ⚠️ Manifest v5 (§3.12, G33): when creating this feature US, create its
  manifest JSON in devflow/metrics/user-stories/ (schema_version "5.0", us,
  story_points, bolts: [], checkpoint_approvals: []). A feature US without
  its manifest does not exist. Validate against manifest-v5-us.schema.json.
  US-000 has no manifest — it is a permanent container with no approval
  lifecycle.
-->

> **Which template is this?** Use **this** one when the functionality is
> **new**. Use [`TEMPLATE-US-EVOLUTION.md`](TEMPLATE-US-EVOLUTION.md) when you
> are changing behaviour that is **already delivered** — that story states the
> complete target behaviour with each criterion marked `existing` or `delta`.
> A defect in what was delivered is a **BUG**, not a story; a change with no
> behaviour difference is a **non-functional Bolt** under `US-000`. See
> `../README.md`.

> **Naming convention (mandatory):** Files go in `user-stories/` with the
> format `US-NNN-<description>.md`. Example: `US-001-payment-processing.md`.
> The `US-NNN` prefix is required for traceability across ADRs, SPECs, MEMs,
> BUGs, and Bolt manifests.

# US-NNN — [User Story title]

| Field          | Value |
|----------------|-------|
| **Unit**       | [Unit / Epic] |
| **ADRs**       | [links to applicable ADRs] |
| **Status**     | [draft / approved / deprecated] |
| **Story points** | [1 / 2 / 3 / 5 / 8 / 13 — relative functional complexity, never time (§2.6)] |

---

**As a** [role], **I want** [capability], **so that** [value].

## 1. Acceptance criteria

- **Given** [context], **When** [action], **Then** [expected result].
- **Given** [context], **When** [action], **Then** [expected result].
- **Given** [context], **When** [action], **Then** [expected result].

> ACs are verifiable functional criteria only — non-functional constraints
> live in ADRs (§2.7).

## 2. Bolts

| # | Bolt | Type | Layer | Description | Est. active delivery |
|---|------|------|-------|-------------|----------------------|
| 1 | `BOLT-001` (`../bolts/US-NNN.BOLT-001-short-title.md`) | functional | Backend | [Description] | 3h |
| 2 | `BOLT-002` (`../bolts/US-NNN.BOLT-002-short-title.md`) | functional | Frontend | [Description] | 4h |

> **Note:** Bolts are detailed in separate documents using
> [TEMPLATE-BOLT.md](../bolts/TEMPLATE-BOLT.md) and MUST be placed in the
> `bolts/` subfolder with the naming convention
> `US-NNN.BOLT-NNN-<description>.md`. Feature USs decompose into
> functional Bolts; the US approval gates their readiness. Each Bolt has
> its own `AITL-BOLT-READY-Approval`,
> DoR (validated within it) and DoD.
>
> **Estimation:** the `Est. active delivery` column follows the AI-native
> estimation rule (§2.4) — composed from expected V-Bounces, review budgets
> per risk_class and setup and integration test overhead, never from manual
> coding effort. Typical low/medium Bolts: 1–4h.
>
> **Plausibility check (§2.6):** the Bolt count should roughly match the
> US's story points band (1–2 SP → 1–2 Bolts; 3–5 → 2–4; 8 → 4+). Far
> outside the band → re-examine the score or the slicing — never force a
> decomposition to fit the band, and never convert points to hours (W18).

---

## 3. Business rules

[Domain constraints and conditions.]

| # | Rule | Condition | Action |
|---|------|-----------|--------|
|   |      |           |        |

---

## 4. User flows

```mermaid
flowchart TB
    A["Start"] --> B["Step 1"] --> C["Step 2"] --> D["End"]
```

---

## 5. Impact

[Affected modules, dependencies, risks.]

---

## 6. SDLC tool alignment

[Associated Work Items, Sprint, Board — optional, per the team's own SDLC
integration.]

---

## 7. AITL-US-Approval

> **Avenga DevFlow §2.6, §3.0.** This feature US remains a draft until a
> Functional Analyst records `AITL-US-Approval` (recorded in the `review`
> frontmatter block), confirming that the US and its ACs faithfully
> represent the evidence in `input/` and the analysis in `analysis/`.
> Its functional Bolts may be drafted and refined while it is
> draft; none may be approved before it is. US-000 is outside this lifecycle.

---

## 8. Manifest creation (mandatory)

> ⚠️ **MANDATORY** — When this feature US is created, also create its
> manifest JSON in `devflow/metrics/user-stories/` with the same name
> (`.md` → `.json`): `schema_version: "5.0"`, the
> `us{id,ref,sources,generation,review_ready_at,review_started_at}`
> block, `story_points`, and empty `bolts` / `checkpoint_approvals`
> arrays. A feature US without its manifest **does not exist** (§3.12,
> G33). Validate against
> [`devflow/metrics/manifest-v5-us.schema.json`](../../metrics/manifest-v5-us.schema.json);
> use [`TEMPLATE-MANIFEST-US.json`](../../metrics/TEMPLATE-MANIFEST-US.json)
> as the example. The agent appends `bolts[]` and the `AITL-US-Approval`
> decision — with its `review_ready_at`, `review_started_at` and
> `decided_at` timings — as the lifecycle progresses.
>
> **US-000 is excluded:** it is a permanent traceability container with no
> approval lifecycle, and therefore carries no manifest.
