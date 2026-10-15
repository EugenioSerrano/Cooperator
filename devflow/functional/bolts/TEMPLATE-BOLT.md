---
id: "US-NNN.BOLT-NNN" # or US-000.BOLT-NNN / TC-NNN.BOLT-NNN
title: ""
date: "YYYY-MM-DD"
author: ""       # human:<user> — the accountable human, never an agent (§3.0)
agent: ""        # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft" # draft | ready | specified | in-development | blocked | in-integration-testing | integration-tests-failed | integration-tests-blocked | ready-for-acceptance | done | deprecated — declared, one value at a time, changed only by a transition of §3.2.3 with its Status timeline row in the same act
owner: "" # Functional Analyst / Architect-TL / QA per type (per §2.4)
us: "" # Parent: feature US (functional), US-000 (non-functional), or one TC (test)
bug: "" # BUG-NNN when BUG-driven — the BUG and Bolt reference each other (§2.16)
bolt_type: "" # functional | non-functional | test — the three canonical types (§2.4)
work_category: "" # feature | refactor | infra | hardening | debt | qa_automation (§3.8)
test_integration: "" # required | not-required — does the outcome touch anything verifiable only against real dependencies? Proposed by the drafting agent from the rubric (§3.2.2), confirmed or changed at AITL-BOLT-READY-Approval; Test Bolts always not-required
test_integration_reason: "" # one line: why — required always, also when the value matches the work_category start
service_class: "" # regulatory | incident_hotfix | feature_value | debt_hardening (§3.8)
layer: "" # Backend | Frontend | Mobile | Data | Infra | QA-Automation | Full-stack | Documentation
risk_class: "" # low | medium | high | critical (assigned during AITL-BOLT-READY-Approval — §3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered, normative definition in §3.6
dependencies: [] # Prior Bolts/artifacts this one depends on
sources: [] # parent US/TC, approved ADRs, DISC/REV/AREV evidence — required, min 1 (§2.4)
review_ready_at: "" # When this version is submitted for review (§3.0)
review: # AITL-BOLT-READY-Approval — filled by the human reviewer (§3.0)
  decision: "" # approved | changes_requested | rejected
  reviewers: [] # [{actor, role, model}]
  written_by: [] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: ""
  decided_at: ""
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
acceptance_review_ready_at: "" # Set in the same act as the transition into ready-for-acceptance (§3.2.3) — never at an earlier status
acceptance_review: # AITL-BOLT-DONE-Approval — filled by the human reviewer (§3.0)
  decision: "" # approved | changes_requested | rejected
  reviewers: [] # [{actor, role, model}]
  written_by: [] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: ""
  decided_at: ""
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
risk_history: [] # [{previous_class, new_class, decided_by, reason, decided_at}] — risk cannot be downgraded after the first MEM approval without formal re-review (§3.3)
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — descriptions,
  acceptance criteria — goes in the project's content_language (declared
  in devflow/LANGUAGE).

  ⚠️ BOLT = WHAT, never HOW (§2.4): the Bolt states the requested outcome,
  scope, exclusions, dependencies, applicable governed sources, and
  expected evidence. No architecture decisions, technologies, endpoints,
  schemas, algorithms, or implementation instructions. Those belong in the
  SPEC (how) and ADRs (constraints).

  ⚠️ THREE AND ONLY THREE TYPES (§2.4): functional (feature US) |
  non-functional (US-000) | test (one TC). BUG and hotfix are
  conditions, not types. The classification follows the PRIMARY OUTCOME,
  not the layer or technology.

  ⚠️ AITL-BOLT-READY-Approval (§2.4, §3.0): a Bolt remains a DRAFT until its
  own approval is recorded (Functional Analyst for functional; Architect or
  Tech Lead for non-functional; QA Lead/QA Automation Lead/Architect/Tech
  Lead for test). Approval includes the DoR. No approval is inherited.
  Approval sets `status: ready`.

  ⚠️ STATUS (§3.2.3): declared, one value at a time. Whoever records an act of
  the transition table sets `status` and appends one row to
  `## Status timeline` in the same act. Never edit or delete a row; fix an
  error with a correction row.

  ⚠️ TEST_INTEGRATION (§3.2.2): proposed by the drafting agent with its
  one-line reason, never left blank for the human; confirmed or changed at
  AITL-BOLT-READY-Approval; afterwards changed only by an amendment
  re-approved at READY, followed by a SPEC revision.

  ⚠️ Manifest v5 (§3.12): when creating this Bolt, create its manifest JSON
  in devflow/metrics/bolts/ (schema_version, bolt, spec_revisions: [] and
  v_bounces: [] empty; checkpoint_approvals[] carries the origin decisions that
  already exist at this point: AITL-US-Approval (functional),
  AITL-TC-Approval (test), none (non-functional under US-000), plus
  AITL-BUG-Approval when BUG-driven. The manifest is progressively updated
  as each lifecycle step completes). A Bolt without a manifest does
  not exist. Validate against manifest-v5-bolt.schema.json.
-->

# US-NNN.BOLT-NNN — [Descriptive title]

> **Naming convention (mandatory):** Files go in `bolts/`:
> `US-NNN.BOLT-NNN-<description>.md` (functional),
> `US-000.BOLT-NNN-<description>.md` (non-functional),
> `TC-NNN.BOLT-NNN-<description>.md` (test).

| Field        | Value |
|--------------|-------|
| **Parent**   | [US-NNN / US-000 / TC-NNN] |
| **Type**     | [functional / non-functional / test] |
| **work_category** | [feature / refactor / infra / hardening / debt / qa_automation] |
| **service_class** | [regulatory / incident_hotfix / feature_value / debt_hardening] |
| **Demo / completion evidence** | [How to verify: curl command, screenshot, specific test] |

---

## Status timeline

> **Avenga DevFlow §3.2.3.** This Bolt's `status` is declared here, one
> value at a time, and changes only through a transition of the Bolt
> transition table. Whoever records the act — the actor itself, or the
> Coordinator transcribing a human's decision — does three things **in the
> same act**: records the fact where it is recorded (the manifest, the
> review block), sets `status` to the new value, and appends one row
> below. When the act is an AITL decision, the actor who records it is
> that decision record's `written_by` — writing is not signing (§3.0).
>
> **Taking this Bolt to start a V-Bounce makes you its single active
> executor (G11): before generating anything, set
> `status: in-development` and append its row (T04 from `specified`; T16,
> T18 or T19 when a later V-Bounce starts from `integration-tests-blocked`,
> `in-integration-testing` or `ready-for-acceptance`).**
>
> The table is append-only: never edit or delete a row. A recording error
> is fixed by a **correction row** whose *Act / evidence* begins
> `Correction —` and cites the row it corrects.
>
> | `status` | Shown as | The Bolt is… |
> |---|---|---|
> | `draft` | Draft | Written, not yet approved at `AITL-BOLT-READY-Approval` |
> | `ready` | Ready | Approved at `AITL-BOLT-READY-Approval` (DoR met), with no approved SPEC revision governing it now — awaiting its first SPEC, or a new revision after a material change (G15) |
> | `specified` | Specified | Governed by an approved SPEC revision that no V-Bounce has started executing |
> | `in-development` | In Development | Being executed, or awaiting its next V-Bounce: a MEM pending review or not approved, or a failed integration test run classified `code`, `test` or `spec` in this Bolt's own change |
> | `blocked` | Blocked | Its latest V-Bounce ended `blocked` and that V-Bounce's MEM is not yet decided — the stop-and-ask moment (§3.0) |
> | `in-integration-testing` | In Integration Testing | Past **Development Completed** with `test_integration: required` and an `applicable` integration test plan; no covering run recorded yet |
> | `integration-tests-failed` | Integration Tests Failed | A covering run failed; awaiting its human classification |
> | `integration-tests-blocked` | Integration Tests Blocked | A covering run could not execute, or a failure was classified `environment` or `test` in tests owned by another Bolt or suite; awaiting a new run |
> | `ready-for-acceptance` | Ready for Acceptance | Integration tests passed; or `test_integration: not-required`; or `required` with a `waived` plan; or its pending integration testing waived by a human (T23); `AITL-BOLT-DONE-Approval` may now be requested |
> | `done` | Done | Accepted (`AITL-BOLT-DONE-Approval` approved) — terminal |
> | `deprecated` | Deprecated | Withdrawn, with its reason — terminal |
>
> **Events, never values:** **Development Completed** — `AITL-MEM-Approval`
> approved on the latest V-Bounce (→ `in-integration-testing` when
> `test_integration` is `required` with an `applicable` plan; otherwise
> `ready-for-acceptance`); **Integration Tests Passed** — a passing run
> covering the latest approved change (→ `ready-for-acceptance`). A late
> waiver's row begins `Integration testing waived —` (T23); only a human
> records it.

| Date | From | To | Act / evidence |
|------|------|----|----------------|
| `YYYY-MM-DDTHH:mm:ss±HH:MM` | — | `draft` | Bolt created — `human:<user>` (T01) |

---

## 1. Description

[What is delivered in this Bolt. One clear sentence — the WHAT, never the
HOW.]

---

## 2. Expected outcome and completion evidence

- **Objective:** [the demonstrable, measurable result]
- **Completion criterion:** [the observable result that demonstrates completion]
- **Expected evidence:** [how completion is verified]
- **Scope:** [what is covered]
- **Exclusions:** [what is explicitly NOT covered]
- **Risks / controls:** [identified risks and their controls]

---

## 3. Covered acceptance criteria

> Reference the ACs of the parent artifact — **never write new ones here**
> (§2.4, §2.6). Functional Bolt: the AC ids of its parent US, quoted for
> readability. Non-functional Bolt: leave `n/a` — US-000 has no ACs; the
> measurable outcome lives in section 2 and its constraints in the governing
> ADRs. Test Bolt: leave `n/a` — the expected results live in the parent TC.

- `AC-N` (US-NNN) — **Given** [context], **When** [action], **Then** [expected result].
- `AC-N` (US-NNN) — **Given** [context], **When** [action], **Then** [expected result].

---

## 4. Dependencies

- **Governing sources:** [parent US/ACs, TC, approved ADR,
  the BUG when BUG-driven, DISC/REV/AREV evidence]
- **Previous Bolts:** [Bolts that must be completed first]
- **External:** [APIs, packages, services required]

---

## 5. Does this Bolt need splitting?

> Sizing target: **1 hour to 1 working day of active delivery time** — not
> a destructive boundary (§2.4, §3.2). Crossing a day boundary or adding
> V-Bounces never splits a Bolt. Split only for **independently deliverable
> outcomes** that should be governed and accepted separately.
>
> Estimate active delivery with the **AI-native estimation rule (§2.4)**:
> expected V-Bounces × (agent generation + review budget for the risk_class)
> + SPEC review + acceptance + setup and integration test overhead. **Never price code
> creation as human typing time.** Typical low/medium Bolts land in 1–4h; an
> estimate over one day usually signals manual-effort anchoring before it
> signals a split.

| Heuristic | Split? |
|-----------|--------|
| **Independent deliverables** | If it contains several independently acceptable outcomes → split |
| **Layers** | If the Bolt spans domain + application + infrastructure + API as separate deliverables → separate |
| **Dependencies** | If Bolt B requires Bolt A to be finished → they are two distinct Bolts |
| **Testability** | If one part cannot be tested without the other → keep together; if independent → separate |
| **Demo criterion** | If you have more than one demo criterion → possible split (one Bolt = one demo) |
| **Risk** | If one part is high risk and another is trivial → separate so the simple part is not blocked |

---

## 6. AITL-BOLT-READY-Approval

> **Avenga DevFlow §2.4, §3.0.** This Bolt remains a **draft** until a
> qualified human records `AITL-BOLT-READY-Approval` (recorded in the `review`
> frontmatter block). The approval **includes the DoR**: parent approved,
> relevant ADRs approved, risks identified, context accessible, completion
> evidence defined. Approval authorizes SPEC preparation — it does **not**
> authorize technical execution by itself. Its decision moves the `status`:
> `approved` → `ready` (T02); `rejected` → `deprecated` with the reason (T22);
> `changes_requested` changes nothing (§3.2.3). The decision also confirms or
> changes `test_integration` (§3.2.2).

| Field | Value |
|-------|-------|
| **Approver** | Functional Analyst (functional) / Architect or Tech Lead (non-functional¹) / QA Lead, QA Automation Lead, Architect or Tech Lead (test) |
| **Role** | functional_analyst / architect / tech_lead / qa_lead / qa_automation_lead |
| **Decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Assigned risk** | low / medium / high / critical |
| **Findings** | [findings or acknowledged_without_comment + reason] |

> ¹ Except: the dedicated Bolt of a non-functional BUG mirrors its parent
> BUG's routing — Architect/Tech Lead recommended when `severity: critical`,
> otherwise any team member — guidance, never a gate: any qualified team
> member, the Bolt's own author included, may approve it at any severity (§2.16).

By signing I declare that: (a) parent and covered ACs are verifiable, (b)
relevant ADRs are approved, (c) risks are identified, (d) context is available
for the agent, (e) completion evidence is clear, (f) `test_integration` and
its reason fit the outcome (§3.2.2). Replicated in the Bolt manifest
(`checkpoint_approvals[]`).

---

## 7. AITL-BOLT-DONE-Approval (acceptance)

> **Avenga DevFlow §2.9, §3.0.** **Development Completed** (the latest MEM
> approved) is an event, not `done`, and not yet acceptable: the Bolt first
> passes its integration test cycle, or has no run pending, and becomes
> `ready-for-acceptance` (§3.2.2, §3.2.3). Bolt `done` requires acceptance: a
> qualified human records `AITL-BOLT-DONE-Approval` (recorded in the
> `acceptance_review` frontmatter block). The acceptance review follows the
> same contract as readiness (`acceptance_review_ready_at` +
> `acceptance_review`); its timing is projected to the manifest as
> `bolt.acceptance.review_ready_at` / `bolt.acceptance.review_started_at`
> (§3.12). A second acceptance round keeps the first submission in the
> manifest; this block holds the latest round.

| Field | Value |
|-------|-------|
| **Approver** | per `work_category` — see routing below |
| **Role** | product_owner / project_manager / tech_lead / sre / security / qa_lead / qa_automation_lead |
| **Decision** | approved / changes_requested / rejected |
| **acceptance_review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **acceptance_review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **acceptance_review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Integration test evidence** | passing integration test run N (Bolt manifest, evidence link) / not-required: [reason — plan n/a in the SPEC] / waived: ADR-NNN (+ executed compensating-control evidence on high / critical) / integration testing waived — [reason] ([compensating-control evidence] or "no compensating control recorded") |
| **Findings** | [findings or acknowledged_without_comment + reason] |

By signing I declare that the approved V-Bounce output satisfies the
Bolt's completion evidence **and that its integration test evidence is present**
(§2.9, §3.0, §3.2.2). Replicated in the Bolt manifest
(`checkpoint_approvals[]`, decision `AITL-BOLT-DONE-Approval`).

**Acceptance routing** — who signs `AITL-BOLT-DONE-Approval` depends
on `work_category` (§3.11):

| work_category | Acceptance approver | Demo form |
|---------------|---------------------|-----------|
| `feature`   | PO / PM           | Business demo |
| `refactor`  | Tech Lead         | Before/after diff + test parity |
| `infra`     | Tech Lead + SRE   | Deployment evidence + perf-smoke |
| `hardening` | Tech Lead + Sec   | Fixed control + regression test |
| `debt`      | Tech Lead         | Metric/maintainability improvement |
| `qa_automation` | QA Lead / QA Automation Lead | TC automated with execution evidence |

**Acceptance outcomes** (§3.2.3):

- `approved` → `done` (T20).
- `changes_requested` → no change: the missing evidence is added, or a new
  V-Bounce takes the Bolt back through the cycle (via a SPEC revision when
  the change is material to the SPEC, G15).
- `rejected` → `deprecated` (T22), with the reason naming who owns the merged
  change: the replacement Bolt, which inherits and corrects it, or a
  reverting Bolt (G07). If the outcome is still needed, it becomes a new
  Bolt.

> **The joint act** (§3.2.2): when the MEM approval takes this Bolt straight
> to `ready-for-acceptance` with no run pending (T08), the signer holds both
> the Dev-validator role and the acceptance role above, and the acceptance
> evidence is in the package, a human may record `AITL-MEM-Approval` and
> `AITL-BOLT-DONE-Approval` in one act — two records (each block and each
> `checkpoint_approvals[]` entry with its own role), two timeline rows at the
> same timestamp, the three acceptance times equal to the act's time. Never
> implicit: when such a MEM is approved without that instruction, the agent
> writing the record asks once, in the human's language, and never records the
> DONE on silence. Unavailable with a run pending, to a signer without the
> acceptance role, to an agent approver, and after a late waiver (T23).

---

## 8. Manifest creation (mandatory)

> ⚠️ **MANDATORY** — When this Bolt is created, also create its manifest
> JSON in `devflow/metrics/bolts/` with the same name (`.md` → `.json`):
> bolt-level fields (`schema_version: "5.0"`,
> `bolt{id,type,ref,sources,generation,review_ready_at,review_started_at,acceptance{review_ready_at,review_started_at}}`)
> and empty `spec_revisions` / `v_bounces`, with the required origin
> decisions already taken in `checkpoint_approvals[]`. A Bolt without a manifest
> **does not exist** (§0). Validate against
> [`devflow/metrics/manifest-v5-bolt.schema.json`](../../metrics/manifest-v5-bolt.schema.json);
> use the example matching the Bolt type:
> [`TEMPLATE-MANIFEST-BOLT.json`](../../metrics/TEMPLATE-MANIFEST-BOLT.json) (functional),
> [`TEMPLATE-MANIFEST-BOLT-NONFUNCTIONAL.json`](../../metrics/TEMPLATE-MANIFEST-BOLT-NONFUNCTIONAL.json) (US-000),
> [`TEMPLATE-MANIFEST-BOLT-TEST.json`](../../metrics/TEMPLATE-MANIFEST-BOLT-TEST.json) (Test Bolt).
> The agent appends `spec_revisions[]`, `v_bounces[]` and
> AITL decisions as the lifecycle progresses (§3.12).

---

## 9. History

> Handoff log (§3.3): the handoff is documented here — date, outgoing
> executor, incoming executor, reason. After a recorded handoff, the
> **incoming** executor is the Dev-validator who reviews and approves the
> pending MEM of the previous V-Bounce; the outgoing executor cannot.

| Date | Outgoing executor | Incoming executor | Reason |
|------|-------------------|-------------------|--------|
|      |                   |                   |        |
