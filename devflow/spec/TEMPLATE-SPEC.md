---
id: "SPEC-YYMMDD-HHmm"
title: ""
date: "YYYY-MM-DD"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: ""  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft" # draft | approved | blocked | obsolete
origin: "" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 1 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: []
prerequisites: [] # Prior SPECs this one depends on
risk_class: "" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "internal" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's; this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: ""        # n/a — why no integration testing applies (one line; a Test Bolt: "Test Bolt — executed in the TC's run cycle")
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "" # approved | changes_requested | rejected
  reviewers: [] # [{actor, role, model}]
  written_by: [] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "" # ISO 8601 with seconds + offset
  decided_at: "" # ISO 8601 with seconds + offset
  findings: []
  acknowledged_without_comment: true # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — descriptions,
  context, rationale, findings — goes in the project's content_language
  (declared in devflow/LANGUAGE).

  ⚠️ MANDATORY TIMESTAMP: To generate the ID and filename:
  1. Run: Get-Date -Format "yyMMdd-HHmm" (PowerShell) or date +"%y%m%d-%H%M" (Bash)
  2. Use that ACTUAL value to replace YYMMDD-HHmm in the id, title and filename.
  3. NEVER invent or estimate the time.

  ⚠️ MANDATORY ATTRIBUTION (§3.0): `author` = the accountable human; `agent` = `agent:<id>`
     of the agent producing this document (never the delegating one); `llm` = its exact model id.
  ⚠️ MANDATORY BOLT: The `bolt` field MUST reference an APPROVED Bolt (AITL-BOLT-READY-Approval).
     A SPEC without a Bolt reference is INVALID and cannot start implementation.
     Traceability: SPEC → BOLT → manifest (spec_revisions[]).
  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): before generating, verify every governed source
     is approved (BUG/TC/Bolt/US/ADR/DISC/REV/AREV). Any draft source → blocking
     report, never a partial SPEC.
  ⚠️ AITL-SPEC-Approval: a draft SPEC cannot start a code-run or V-Bounce. Material
     source changes invalidate the approval → stop, revise, re-approve (G15).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): declare `integration_test_plan` coherent
     with the Bolt's `test_integration` — `required` → `applicable` (tests,
     environment, data) or `waived` (ADR); `not-required` → `n/a` (reason). A
     mismatch is a blocking report of the pre-SPEC evidence gate; the way
     forward is a Bolt amendment. It decides whether the Bolt passes through
     its integration test cycle before acceptance.
     One V-Bounce never spans two SPEC revisions.
-->

# SPEC-YYMMDD-HHmm — [Descriptive title]

| Field | Value |
|-------|-------|
| **Origin** | [US-NNN / BUG-NNN / TC-NNN / DISC-NNN / REV-NNN / AREV-NNN / ADR-NNN] |
| **Bolt** | [US-NNN.BOLT-NNN] |
| **ADRs** | [links to associated ADRs] |
| **Risk Class** | [low / medium / high / critical] |
| **Revision** | [N] |

---

## 1. Objective

<!--
  ⚠️ MANDATORY: What is being built or modified. At least one paragraph.
  What problem does it solve? What happens if NOT implemented?
-->

[Concise description of what this SPEC will implement or modify.]

---

## 2. Context

<!--
  Where does this need come from? Reference the originating US/BUG/TC/DISC/REV/ADR and the
  approved Bolt. What hardware, infrastructure, or system constraints exist?
-->

[Business and technical context that motivated this SPEC.]

---

## 3. Source inventory and approval references

<!--
  ⚠️ MANDATORY: from the pre-SPEC evidence gate — every governed source used,
  its approval status, and the repository baseline.
-->

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [US-NNN.BOLT-NNN] | AITL-BOLT-READY-Approval ✓ |
| Feature US / TC / BUG | [ref] | AITL-US / AITL-TC / AITL-BUG-Approval ✓ |
| ADRs | [ADR-NNN] | AITL-ADR-Approval ✓ |
| DISC/REV/AREV evidence | [ref] | approval ✓ |
| Repository baseline | [git commit] | — |

---

## 4. Scope

### In scope

- [What this SPEC covers]

### Out of scope

- [What this SPEC explicitly does NOT cover]

---

## 5. Prerequisites and baseline

<!--
  Current build state, prior SPECs that must be completed first,
  environment requirements.
-->

- [Prerequisite 1]
- [Prerequisite 2]

---

## 6. Phases

<!--
  ⚠️ Each phase must be EXPLANATORY, not a telegram.
  Describe: what files are created/modified, what patterns are applied,
  how components interact, what ADRs constrain them.

  ⚠️ ONE BOLT PER SPEC: All phases in this SPEC implement the SAME Bolt
  (the one in the `bolt` frontmatter field). If your work spans multiple
  Bolts, create one SPEC per Bolt.

  ⚠️ BUG SPECS (§3.3.1): prescribe strict TDD in ONE V-Bounce — reproduction
  test (RED evidence) → production change → green. Production code may not
  change before red evidence exists. A defect that reproduces only in
  integration tests (the integration-test-only BUG Bolt, §3.2.2): the RED is
  a failed integration test run recorded before the V-Bounce
  (`integration_test_runs[]`), the V-Bounce adds the best hermetic
  reproduction, the GREEN is a passing covering run in the integration test
  cycle; the integration test plan is `applicable`, never `waived`.

  ⚠️ TEST BOLT SPECS (§3.2.1): map exactly one approved parent TC to the QA
  Automation code; preserve the TC's expected results; no production-code
  changes.

  Format: Phase A — [Name]
  Duration: Xh total cycle — Complexity: Low/Medium/High
-->

### Phase A — [Name]

**Duration:** Xh total cycle — **Complexity:** Low / Medium / High

#### A.1 [Sub-step description]

[Detailed description of what is built: components, patterns, dependencies, ADR references.]

**Files created:**
- `path/to/file.ts` — Purpose in the system

**Files modified:**
- `path/to/file.ts` — Description of change

#### A.2 [Sub-step description]

[Same level of detail.]

---

### Phase B — [Name]

**Duration:** Xh total cycle — **Complexity:** Low / Medium / High

[Same structure as Phase A.]

---

## 7. Acceptance criteria

<!--
  ⚠️ Each AC must be testable. Use Given/When/Then format (BDD).
  Define inputs, expected outputs, and edge cases.
-->

### AC-1: [Short description]

**Given** [precondition]
**When** [action]
**Then** [expected result]

### AC-2: [Short description]

**Given** [precondition]
**When** [action]
**Then** [expected result]

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| [US AC-1 / technical outcome] | [implementation approach] | [test or evidence] |
| | | |

---

## 8. Testing strategy

<!--
  Two per-Bolt levels (§3.6). The V-Bounce level is hermetic: the agent runs
  it in its own loop until green. The integration test level is written, built
  and statically checked in a V-Bounce — or comes from another Bolt or suite —
  and executed in the Bolt's integration test cycle, after AITL-MEM-Approval.
  A test against a test double is V-Bounce level, never the integration test
  level.
  For BUG SPECs: the reproduction test and its RED evidence come first (§3.3.1)
  — for a defect that reproduces only in integration tests, the RED is a
  failed integration test run recorded before the V-Bounce.
-->

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Unit tests:** [what and how many]
- **Component tests:** [what and how many, if applicable]
- **Consumer-side contract tests (test doubles):** [when the change crosses component boundaries]
- **Edge cases:** [list]
- **BUG evidence:** [RED command/result → GREEN command/result — for a defect that reproduces only in integration tests: the best hermetic reproduction and its result before the change, then the hermetic GREEN]

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> Declared in the frontmatter `integration_test_plan` (§3.2.2); this section details what it lists.

- **Integration tests (real dependencies):** [what, against which dependency, and who wrote them — this V-Bounce, another Bolt or a shared suite — or n/a with reason]
- **Contract / E2E against the real providers:** [when the change crosses component boundaries — or n/a with reason]
- **Edge cases:** [list]
- **BUG evidence (a defect that reproduces only in integration tests):** [the RED run — its `integration_test_runs[]` number, environment and evidence, covering the commit the V-Bounce starts from, or "cited in the MEM" when it is recorded later — and the integration test that reproduces the defect, which the covering run executes for the GREEN]

---

## 9. Quality gates

<!--
  Applicable gates selected by this SPEC (§3.6): classic + AI-native.
  Each gate runs at the level shown (§3.6): V-Bounce (hermetic, run by the
  agent) or Integration test (the Bolt's integration test cycle).
  Each ends pass | waived (ADR-NNN) | n/a (with reason).
-->

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | | |
| Unit / component tests | V-Bounce | | |
| Consumer-side contract tests (test doubles) | V-Bounce | | |
| SAST | V-Bounce | | |
| Dependency scanning / licenses | V-Bounce | | |
| OWASP Top 10 — static review | V-Bounce | | |
| Prompt-injection scan | V-Bounce | | |
| Secret-leak scan | V-Bounce | | |
| Hallucination lint | V-Bounce | | |
| IP / license provenance | V-Bounce | | |
| PII / DLP | V-Bounce | | |
| Dependency-confusion | V-Bounce | | |
| Test-first evidence | V-Bounce | | |
| Behavioral reproducibility (hermetic suite) | V-Bounce | | |
| Bolt-manifest validation | V-Bounce | | |
| Integration tests (real dependencies) | Integration test | | |
| Contract / E2E against the real providers | Integration test | | |
| DAST / OWASP Top 10 — dynamic | Integration test | | |
| Perf-smoke (p95/p99) | Integration test | | |
| Observability (logs / metrics / traces) | Integration test | | |

> Each gate ends `pass` / `waived` (ADR-NNN) / `n/a` (with reason) (§3.6).
> Non-applicable gates must still be listed with their `n/a` reason.
> Release-level gates — mutation testing, cross-Bolt E2E/contract, the SBOM
> of the built distributable — are not selected per Bolt (§3.6).
> The Integration test rows decide `integration_test_plan`: `applicable` if
> any applies, `waived` if every applicable one is waived by the same ADR,
> `n/a` if none applies — and it must agree with the Bolt's `test_integration`
> (§3.2.2).

---

## 10. Security and data

<!--
  ⚠️ MANDATORY: security considerations (auth, authorization, data
  exposure, secrets) and data handling per the SPEC's `data_classification`
  (§3.2.1).
-->

- [Security control / threat considered]
- [Data handling / classification notes]

---

## 11. Monitoring and observability

<!--
  Logs, metrics, traces, alerts that must be added or modified.
-->

- [Log/metric/alert 1]
- [Log/metric/alert 2]

---

## 12. Migration, compatibility and rollback

<!--
  Schema changes, feature flags, compatibility and rollback procedure.
  If not applicable, state it explicitly (§3.2.1).
-->

- **Migration:** [steps or "N/A"]
- **Compatibility:** [notes or "N/A"]
- **Rollback:** [steps or "N/A"]

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
|      |                   |             |            |

---

## 14. Decisions and trade-offs

<!--
  Micro-decisions that do not warrant a full ADR.
  Why one approach over another, what alternatives were discarded.
-->

| Decision | Reason |
|----------|--------|
|          |        |

---

## 15. Stop conditions

<!--
  Explicit conditions that halt the V-Bounce: an unresolved architectural
  decision → ADR lifecycle; unreproducible bug; material source change;
  conflicting, missing or ambiguous evidence → stop and request resolution
  (never fill the gap with an assumption, §2.4.1). The agent records the
  blocker in the MEM.
-->

- [Stop condition 1]

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded (a defect that reproduces only in integration tests: the red run cited and the hermetic reproduction's result recorded; its green, the covering run, comes after AITL-MEM-Approval)
- [ ] Code follows applicable ADRs
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: passing covering run / `n/a` reason / `waived` ADR / integration testing waived by a human with its reason (§3.2.2)

---

## 17. References

<!--
  Related documents: US, ADRs, DISCs, BUGs, RISKs, prior SPECs.
-->

- [Reference 1]
- [Reference 2]

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
|      |        |        |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** This SPEC remains a draft until the
> Dev-validator (+ applicable domain owners) records `AITL-SPEC-Approval`
> (in the `review` frontmatter block). Bolt approval (`AITL-BOLT-READY-Approval`)
> authorizes SPEC preparation; **SPEC approval** authorizes the code-run /
> V-Bounce. A material source change invalidates this approval — stop,
> revise, re-approve (G15).

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + applicable domain owner(s)] |
| **review.written_by** | [optional — who wrote this record: the reviewer, or the agent that transcribed the decision; beside the reviewers, never among them — writing is not signing (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings or acknowledged_without_comment + reason] |
