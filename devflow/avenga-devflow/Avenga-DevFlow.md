---
title: Avenga DevFlow — AI-Native SDLC
version: "5.2"
---

# 0 — Quick Start

If you read nothing else, this is how to ship your first Bolt:

1. **Start from raw evidence.** Process the source material in
   `devflow/input/` and iteratively update the relevant artifacts in
   `devflow/analysis/`.
2. **Choose the correct parent.** Business behavior belongs to a
   feature US. A technical outcome belongs to the permanent
   `US-000-non-functional.md` traceability container. New or materially changed
   feature User Stories stop at **AITL-US-Approval**; US-000 does not require
   functional approval. QA Automation belongs to a TC and uses the
   Test Bolt naming `TC-NNN.BOLT-NNN-<description>.md`.
3. **Approve BUGs before their Bolts, then define and approve every Bolt.** If
   the work corrects a defect, a Functional Analyst, Developer, or QA first
   drafts the BUG. `AITL-BUG-Approval`—functional by a Functional Analyst,
   non-functional by an Architect or Tech Lead when `severity: critical`,
   otherwise by any team member, the BUG's own author included—must be recorded;
   its one dedicated Bolt may be drafted and refined alongside it, but
   `AITL-BOLT-READY-Approval` is refused until then. A Functional Analyst creates a
   functional Bolt under the relevant feature US. A Developer, Architect, or
   Tech Lead may create a non-functional Bolt under US-000 without Functional
   Analyst approval. Each Bolt states what must be delivered, never how to
   implement it, and stops independently at **AITL-BOLT-READY-Approval**,
   routed to the Functional Analyst for functional Bolts and to an Architect
   or Tech Lead for non-functional Bolts — except the dedicated Bolt of a
   non-functional BUG, which mirrors its parent BUG's severity routing
   (§2.16). A Test Bolt is created by QA or a
   QA Automation Engineer from one TC and is approved by a QA Lead,
   QA Automation Lead, Architect, or Tech Lead.
4. **Design and approve the Test Cases from intent, not implementation.** For
   work that requires QA verification, QA derives each `TC-NNN` from the
   approved feature US/ACs and the exact approved Bolt, or from the approved
   non-functional Bolt plus its governing ADRs. Existing code may help locate
   interfaces and setup data, but it must never define the expected result.
   Every TC remains a draft until **`AITL-TC-Approval`**.
5. **Build and approve one SPEC for the Bolt.** Before generating it, the
   agent verifies the approval state of every governed source it will use.
   It then analyzes the approved Bolt, its approved feature US and ACs when
   functional, every applicable approved TC, all applicable approved ADRs, relevant `input/` and `analysis/`
   material, approved DISC/REV/AREV evidence when used, and the existing code,
   tests, configuration and schemas. The SPEC remains a draft until
   **AITL-SPEC-Approval**.
6. **Launch the agent only after `AITL-SPEC-Approval`** — the executor sets
   the Bolt `in-development` as it starts (§3.2.3); it generates code +
   tests and runs the hermetic tests itself until green — the integration
   tests it writes run later, in the Bolt's integration test cycle when its
   `test_integration` is `required` (§3.2.2). If the agent
   stalls beyond its turn budget (the configured number of agent loops without
   a green hermetic suite, §3.3), it **stops and
   asks only after creating the V-Bounce MEM and manifest entry** with the
   blocker and current evidence; you may patch manually and record that
   intervention in the MEM.
7. **CI gates run automatically, each at its level (§3.6)** — hermetic tests,
   static security and the applicable AI-native gates (prompt-injection,
   secret-leak, hallucination lint, IP provenance, PII/DLP,
   dependency-confusion) in every V-Bounce; integration tests, DAST,
   perf-smoke and observability in the integration test cycle. Every
   applicable gate must finish as `pass` or as an explicitly approved
   `waived`; a V-Bounce-level `fail` blocks merge and `AITL-MEM-Approval`, a
   `fail` at the integration test level blocks acceptance, and any `fail`
   blocks promotion. Non-applicable gates are recorded as `n/a` with a reason.
8. **Generate the mandatory MEM** — after implementation and verification, the
   agent creates exactly one `MEM` for the V-Bounce, listing what was done,
   every file added, changed, or deleted, verification evidence, decisions,
   deviations, risks, and traceability to the Bolt and approved SPEC. It then
   updates the manifest and pauses.
9. **Approve the MEM and close the V-Bounce** — a Dev-validator reads the
   **actual diff, test/gate evidence, MEM, and manifest** and records
   **`AITL-MEM-Approval`**. Only this approval completes and approves the
   V-Bounce. If it is the latest V-Bounce for the Bolt, approval is the
   **Development Completed** event: the Bolt becomes `in-integration-testing`
   (`test_integration: required`, plan `applicable`) or, when no integration
   test run is pending (`test_integration: not-required`, or a `waived` plan),
   `ready-for-acceptance` (§3.2.3). A request for changes **or a rejection**
   leaves that V-Bounce unapproved, keeps the Bolt `in-development`, and the
   next autonomous execution becomes a new V-Bounce with a new MEM.
10. **Run integration tests, accept, and promote** — When the Bolt's
   `test_integration` is `required` and the SPEC's integration test plan
   `applicable`, CI or QA run the integration tests against the approved
   change and record each run; a human classifies any failure, which sends the
   Bolt back to development or to a new run, and only a human may waive
   pending integration testing, with a written reason (§3.2.2). Once the Bolt
   is `ready-for-acceptance` — integration tests passed, `not-required`, a
   `waived` plan, or integration testing waived — PO/PM for functional Bolts,
   the routed technical owner for non-functional Bolts, or QA Lead/QA
   Automation Lead for Test Bolts record **`AITL-BOLT-DONE-Approval`**.
   Release and promotion follow the adopting team's own process (§4.6);
   DevFlow does not prescribe Unit/UAT approval checkpoints in this release.
   With no run pending and one human holding both roles, `AITL-MEM-Approval`
   and `AITL-BOLT-DONE-Approval` may be recorded in one explicit joint act
   (§3.2.2).

Everything else in this document explains the **why**, the **edge cases**
and the **governance** behind these steps.

**Non-negotiable rules:**

- A Bolt without a manifest in `devflow/metrics/bolts/` does not exist (and
  every US and TC has its own manifest too, §3.12).
- A feature User Story without **AITL-US-Approval** remains a draft; its
  functional Bolts may be drafted and refined alongside it, but none
  may be approved before the US is. US-000 is a permanent
  technical traceability container, not a feature US awaiting functional
  approval.
- A Bolt without its own **AITL-BOLT-READY-Approval** remains a draft and cannot
  enter SPEC preparation or technical execution.
- No code, configuration, infrastructure, schema, or test change happens
  without an approved Bolt — every SPEC references that Bolt.
- Test Cases are specification-side verification artifacts. They are derived
  from approved US/ACs and approved Bolts, never reverse-engineered from the
  current code as the source of expected behavior. A TC without
  `AITL-TC-Approval` remains a draft and cannot authorize QA Automation.
- Every TC selected for QA Automation may originate one or more
  dedicated Test Bolts. Each such Bolt references exactly one TC, is a direct
  child of that TC, and is named `TC-NNN.BOLT-NNN-<description>.md`; it follows
  the complete Bolt → SPEC → V-Bounce → MEM lifecycle.
- A Bolt has exactly one current canonical SPEC, and a SPEC belongs to exactly
  one Bolt. A SPEC without `AITL-SPEC-Approval` cannot start a code-run or
  V-Bounce.
- Every V-Bounce produces exactly one MEM. A V-Bounce without a complete MEM,
  an updated manifest, and `AITL-MEM-Approval` is not approved and cannot
  advance to merge, promotion, or Bolt acceptance.
- A Bolt may execute 1..n V-Bounces against its canonical SPEC. Every V-Bounce
  has a different MEM; previous MEMs are immutable history. Only an approved
  MEM for the latest V-Bounce is the **Development Completed** event; the Bolt
  then passes its integration test cycle — or has no run pending — to
  `ready-for-acceptance`, and `AITL-BOLT-DONE-Approval` is still required to
  make it `done`.
- The agent does not generate a SPEC while any governed artifact it needs is
  draft, rejected, stale, or missing its applicable AITL approval. It reports
  the blocker instead. Raw inputs, analysis artifacts and existing code do not
  have independent approval checkpoints, but must be inspected and traced.
- Every general-purpose non-functional Bolt is assigned to the permanent
  `US-000-non-functional.md` User Story. There are no "quick fix", "chore",
  refactor, hardening, or infrastructure exceptions to Bolt traceability. QA
  Automation uses the dedicated Test Bolt type parented by one TC.
- Functional Bolts deliver business behavior under a feature US;
  non-functional Bolts deliver general technical outcomes under US-000; Test
  Bolts deliver QA Automation under one TC. No Bolt type contains
  architectural decisions or implementation instructions.
- Functional Bolts are owned and approved by Functional Analysts.
  Non-functional Bolts may be created by Developers, Architects, or Tech Leads
  and receive technical approval without requiring a Functional Analyst.
- Every BUG remains a draft until `AITL-BUG-Approval`; its dedicated Bolt may
  be drafted and refined alongside it, and is approved only after the BUG is. A functional BUG recommends a Functional Analyst;
  a non-functional BUG recommends an Architect or Tech Lead when
  `severity: critical`, otherwise any team member — but the routing is guidance,
  never a gate: any qualified team member, the BUG's own author included, may
  record it at any severity.
- Every BUG has exactly one dedicated Bolt. A functional BUG's Bolt
  is a child of the affected feature US; a non-functional BUG's Bolt
  is a child of `US-000-non-functional.md`. The BUG alone never authorizes a
  code or test change.
- BUG correction uses strict TDD inside the same V-Bounce: first create and run
  the automated test that reproduces the defect and observe it fail (red),
  then change the implementation and run the tests until they pass (green).
  A defect that reproduces only in integration tests takes its red from a
  failed integration test run recorded before the V-Bounce and its green from
  a passing covering run in its integration test cycle (§3.3.1); the red
  still comes first.
- A checkpoint is occupied by a human **by default**; a virtual DevFlow Agent
  only by explicit, valid configuration — absent or invalid configuration,
  every checkpoint is human-only and no AI-signed approval is possible (§3.0).
- A failed quality gate cannot be waived without an ADR approved through
  `AITL-ADR-Approval`; the gate then records `waived`, never `pass`.
- The reviewer reads the **diff**, not the agent's self-summary.
- Agent/model selection between AREV phases is a manual human action in the
  development tool (§3.13); the framework never switches it automatically.
- One working day is the sizing target for a Bolt, not a destructive boundary.
  A Bolt is never split merely because it required multiple V-Bounces or its
  latest V-Bounce continued into the next day.

---

# 1 — Introduction

Methodically integrating Artificial Intelligence (AI) into the software life
cycle produces verifiable gains in **delivery speed** and **quality**, provided
it is combined with solid engineering and governance practices.

From a **delivery** standpoint, empirical evidence shows substantial
improvements in development time when teams use code agents: tasks that
previously took hours or days are completed in minutes or hours.

In **quality**, AI helps expand test coverage and review depth. Empirical
evaluations of LLM-based test generation report competitive average coverage —
better than recent automated techniques — accelerating early detection of
defects and regressions. Results improve with careful *prompting* and human
review.

To make sure these benefits translate into **sustainable outcomes**, it is
essential to instrument and **monitor delivery performance metrics** such as
DORA (change lead time, deployment frequency, failed deployment recovery
time, change fail rate and deployment rework rate), using
them as efficiency signals of the SDLC and linking them to business goals.
The longitudinal research synthesized in ***Accelerate*** shows that
systematic improvement of these metrics correlates with higher organizational
performance and operational stability; within this framework, AI — when
correctly integrated into the development and operations pipeline — **acts as
an accelerator** of these capabilities.

This document presents **Avenga DevFlow** as the operational methodology and
framework for AI-assisted software development, based on the paper *"AI-Driven
Development Life Cycle: Reimagining Software Engineering"* by Raja SP,
Principal Solutions Architect at AWS.

<https://aws.amazon.com/es/blogs/devops/ai-driven-development-life-cycle/>

We use the term "methodology" for the organized set of processes, practices
and metrics that guide the work; and "framework" for the conceptual and
technical scaffolding that articulates roles, artifacts, flows and controls.
The framework relies on software life cycle and security standards that favor
traceability and governance from day one.

The approach aligns with established software life cycle processes and is
**inspired by** the verification/validation discipline of the classic
**V-Model**, without claiming to be a strict V-Model derivative. V-Bounce is
a **horizontal iteration cycle** (SPEC approved through `AITL-SPEC-Approval`
→ autonomous generation and verification → mandatory MEM + manifest →
`AITL-MEM-Approval`); the V-Model is a
**vertical V&V mapping** (unit/integration/system/acceptance testing). We borrow the
*spirit* of explicit V&V, not the geometry.

**Avenga DevFlow is the proprietary methodology and framework of Avenga
LATAM**, developed by the research team to systematize AI-assisted software
development.

The process is governed by **flow and quality metrics** aligned with empirical
evidence on delivery performance, encourages knowledge transfer and stays
compatible with industry standards without losing focus on operational
reality. The outcome: turning ideas into deliverables in hours or a few days
in a predictable, auditable and scalable way.

Avenga DevFlow structures work into **Bolts** (units of work and tracking with
a sizing target of **1 hour to 1 working day of active delivery time** and a
demonstrable, measurable result) and
operationalizes each Bolt through **V-Bounce**.

**V-Bounce** is the operational micro-cycle of Avenga DevFlow defined by this
research team: it starts from a SPEC approved through `AITL-SPEC-Approval` (with an approved feature US and
covered ACs when functional, the US-000 container when non-functional, and
approved ADRs), and the
**AI agent generates the intended-final artifacts by default** (design, code,
tests) **while running and validating, in its own loop, the tests that need no
running environment**; the integration tests it writes are executed
afterwards, in the Bolt's integration test cycle when its `test_integration` is
`required` (§3.2.2). The agent
then creates exactly one MEM, updates the Bolt manifest, and submits the
complete package for `AITL-MEM-Approval`. Autonomous retries remain internal
to that V-Bounce. The V-Bounce is approved only when this checkpoint is
approved. If the human requests changes, the current V-Bounce remains
unapproved and the next agent execution is recorded as a new V-Bounce with a
new MEM.

Conceptually derived from the **V-Model** by making verification and
validation explicit, V-Bounce maps the established life cycle processes. Its
core is **actor-in-the-loop as validator** — a human by default, a virtual
DevFlow Agent only by explicit, valid configuration — ensuring end-to-end
quality, robustness and traceability. Management is anchored in **flow
metrics** (lead time, throughput and commitment rate) consistent with
delivery-performance
evidence (*Accelerate* / DORA).

> **Foundational principle (Actor-in-the-Loop — AITL, non-negotiable):** The AI
> agent generates the **intended-final draft** of code, design and tests of
> the work a human mandated (§3.3, G40) by default, and
> **runs and iterates on the hermetic tests in its own loop** until green —
> the integration tests it writes run in the Bolt's integration test cycle when its
> `test_integration` is `required`. Every named
> checkpoint (§3.0) is occupied by an **actor** who **steers** (Spec, prompts,
> ADRs), **reviews** (diff + evidence) and **approves** — a **human by default**,
> and a virtual DevFlow Agent only by explicit, valid configuration. With no or invalid configuration the project behaves exactly like
> pure Human-in-the-Loop: every checkpoint is a human approval and **no AI-signed
> approval is possible** (the safe-default invariant). Direct human code patches
> are a **legitimate fallback** — not the default — and are logged explicitly in
> the MEM. Bolt time is measured along **three axes** (AI-time, human-review-time,
> total cycle time; §3.7), not in person-hours. **No artifact reaches production
> without an approved checkpoint.** Bypassing a checkpoint breaks the flow and
> invalidates the Bolt. See §3.0 *Actor-in-the-Loop Charter*.

Operationally, the flow begins with the **raw source material preserved in
`devflow/input/`**, including stakeholder interviews, business documents,
legacy databases, third-party documentation, source code, and UI/UX evidence.
AI-assisted analysis iteratively updates the artifacts in `devflow/analysis/`
and may generate or update User Stories. A Functional Analyst must explicitly
approve each feature User Story at **AITL-US-Approval** and approve each
functional Bolt separately at **AITL-BOLT-READY-Approval**. Non-functional Bolts are
created under US-000 by Developers, Architects, or Tech Leads and receive
technical `AITL-BOLT-READY-Approval` from an Architect or Tech Lead, without requiring
Functional Analyst approval. Only an approved Bolt may enter SPEC preparation
and technical execution. For verifiable ACs, the
**Given/When/Then** BDD format is recommended.

Finally, the rationale relies on **mixed empirical evidence**: there are
productivity and satisfaction gains on bounded tasks with code assistants,
but also results pointing to **slowdowns** for senior developers working in
familiar repositories without proper orchestration and governance. Hence the
importance of controlled micro-cycles (V-Bounce), measurable artifacts
(Bolts) and flow metrics to capture benefits and limit risks.

## Methodology objectives

1. Accelerate value delivery using AI from conception to operation.
2. Raise quality by embedding testing and validation in every V-Bounce, with
   a clear Definition of Done.
3. Increase predictability by using Bolts as the unit of promise and
   measurement.
4. Improve traceability between interviews, requirements, design, code, tests
   and decisions (**ADRs**).
5. Reduce rework with clear **Definition of Ready / Done** and **automated
   gates** in CI/CD, backed by security and supply-chain practices.

## Expected benefits

1. Shorter cycles (**hours / days**) without losing rigor.
2. Better alignment with the business team: every Bolt has a specific demo
   criterion.
3. Transparency: **simple metrics** for management and **technical signals**
   for engineering.
4. Continuous learning: every V-Bounce captures decisions and lessons learned
   (**ADRs**).

## Target audience

Engineering, QA, product and project leaders who want to operate with AI as
the **primary generator of design, code and tests**, keeping the human as
validator and approver at critical points.

## When Avenga DevFlow fits — and when it does not

The methodology is empirically biased toward contexts where AI agents add
leverage. We name the limits explicitly (METR 2025 found senior developers
can be **slowed down** in familiar legacy repos without proper orchestration).

**Strong fit:**

- Greenfield or near-greenfield projects.
- Codebases with clear contracts (typed APIs, well-defined ACs).
- Teams with at least one Dev-validator trained to critically review LLM
  output (hallucinated APIs, plausible-but-wrong logic).
- Use cases where DORA-style telemetry is feasible.

**Use with care (run a 2-week pilot first):**

- Large legacy codebases highly familiar to senior developers.
- Domains under heavy regulation requiring model attestation.
- Teams without code review culture or CI baseline.

**Do not adopt as-is:**

- Safety-critical software (medical devices, avionics) without an additional
  certification layer on top.
- Codebases containing data classified as `confidential` or higher that
  cannot be sent to the configured LLM provider — require an on-prem /
  vetted model first.

**Repository topology assumption:** Avenga DevFlow assumes **one
product repository with the `devflow/` documentary root versioned inside
it** (§5.1). Cross-repository work — a Bolt that touches several repos, or
a monorepo where `devflow/` is shared across services — is **not** covered
by this version: the canonical SPEC, the Bolt manifest and the MEM all resolve
paths against the single repository baseline. Teams needing multi-repo
traceability must adapt the location of `devflow/` and the manifest's
repository-relative `ref`/`sources` semantics before adopting the
framework.

The next sections cover the glossary, principles, full flow (from raw input
to deployment), operational templates (US+AC, ADR, Bolt, V-Bounce), metrics
and a phased adoption plan.

---

# 2. Glossary and key concepts

This section is the normative source for concepts, definitions, taxonomies and
artifact contracts. Section §3 is the normative source for lifecycle, AITL,
gates, autonomy and metrics. Section §4 is an explanatory walkthrough and §5
is normative only for artifact locations, filenames and folder
responsibilities. When repeated text appears to diverge, the section that owns
that dimension governs and the other text must be corrected; no summary creates
an alternative rule. Where §2 and §3 both state an approval transition, §2 owns
the artifact's resulting status and §3 owns the lifecycle consequence of that
transition; the statements must remain identical.

## 2.1 Raw inputs (primary evidence)

**What they are:** The unprocessed source materials stored in
`devflow/input/`: business and regulatory documents, database evidence,
third-party documentation, recorded stakeholder interviews, legacy source
code, and UI/UX evidence.

**Purpose:** Raw inputs are the evidence base of the entire Avenga DevFlow.
AI agents and Functional Analysts process them to create or update the
artifacts in `devflow/analysis/`, which in turn support User Stories,
Acceptance Criteria, risks, ADRs, ADR-defined constraints, and draft Bolts.

**Recorded interviews:** Interviews are one especially valuable input type.
The AI may help transcribe them and extract goals, constraints, risks, success
metrics, and open questions, but interview-derived conclusions must remain
traceable to the original recording or transcript. The transcript itself is
raw input: it is **deposited by a human** into `input/interviews/` — agents
never write there, even when they produced the transcription (G31, §5.6).

**Good practices:** Preserve originals, record provenance, avoid silent
normalization, classify sensitive data, and document consent where recordings
or personal information are involved.

## 2.2 Intent

**What it is:** The high-level business or technical intention (the "why").
The objectives of the software product. A project may have one or more
Intents that aim to satisfy stakeholder needs.

**Expected output:** 1–3 sentences with tentative success metrics (e.g.,
"reduce onboarding time by 30%").

**Use:** The AI breaks it down into **Units** and proposes **initial Bolts**.

## 2.3 Unit

**What it is:** A cohesive block of value (similar to an Epic / Subdomain)
with clear boundaries, deployable and able to evolve relatively
autonomously.

**Relationship:** An **Intent** is split into **Units**; each **Unit** is
implemented through **Bolts**.

## 2.4 Bolt (Work and Tracking Unit)

**What it is:** A small unit that authorizes, delimits, assigns, tracks, and
measures a concrete piece of work. It states **what must be delivered and what
observable result will demonstrate completion**. It does not specify how the
solution must be implemented.

Every Bolt targets **1 hour to 1 working day of active delivery time** and must
produce a demonstrable, measurable result. Several V-Bounces may occur within
that day. If human feedback causes a later V-Bounce to continue on the next
working day, the same Bolt and canonical SPEC continue; the Bolt is not split
retroactively and its traceability is preserved.

**Ownership is determined by the Bolt's type:**

- A **Functional Analyst** creates and refines each functional Bolt from a
  feature User Story and its Acceptance Criteria.
- A **Developer, Architect, or Tech Lead** may create and refine a
  non-functional Bolt under US-000 from an approved ADR, approved Discovery
  conclusion, approved Review finding, technical-debt evidence, existing code,
  or another governed technical source.

For every type, the owner defines the work boundary and expected outcome
without designing the implementation inside the Bolt. A Functional Analyst is
neither required to create nor approve a non-functional or Test Bolt.

QA Automation uses a dedicated **Test Bolt** type. A QA or QA Automation
Engineer may create and refine Test Bolts from a Test Case; a QA
Lead, QA Automation Lead, Architect, or Tech Lead records their
  `AITL-BOLT-READY-Approval`. Each Test Bolt references exactly one TC, is a
direct child of that TC, and uses the filename
`TC-NNN.BOLT-NNN-<description>.md`. One TC may require 1..n Test Bolts only
when it contains independently deliverable automation outcomes, not because a
V-Bounce or human feedback crossed a day boundary.

**Role:** The Bolt is the **assignment, planning, tracking, and measurement
currency** (ownership, lead time, throughput, and commitment rate). No code or
code-related artifact may be generated or modified without an approved Bolt.

**Three and only three Bolt types:**

| Type | Parent | Creation and refinement | AITL approval owner | What it delivers |
|--------|--------|-------------------------|---------------------|------------------|
| **Functional Bolt** | A numbered feature User Story (`US-NNN`) | Functional Analyst | Functional Analyst | A demonstrable slice of user- or business-visible behavior that covers identified Acceptance Criteria |
| **Non-functional Bolt** | The permanent `US-000-non-functional.md` | Developer, Architect, or Tech Lead | Architect or Tech Lead¹ | A demonstrable technical outcome whose primary purpose is not new business behavior, such as reducing technical debt, refactoring, hardening, improving an existing technical characteristic, infrastructure work, or developer tooling |
| **Test Bolt** | One Test Case (`TC-NNN`) | QA or QA Automation Engineer | QA Lead, QA Automation Lead, Architect, or Tech Lead | QA Automation code and its supporting test-only assets for exactly one TC |

> ¹ Except: the dedicated Bolt of a non-functional BUG mirrors its parent
> BUG's severity-based routing — Architect/Tech Lead when `severity:
> critical`, otherwise any team member, the Bolt's own author included (§2.16).

A functional Bolt may reference its covered ACs — it never restates or
invents them, they belong to the parent US (§2.6) — and may contain its
objective, scope, exclusions,
dependencies, risks, and expected evidence. A non-functional Bolt may contain
its objective, current technical problem, scope, exclusions, dependencies,
governing artifact references, and measurable evidence. A Test Bolt contains
its approved source TC, automation outcome, scope, exclusions, dependencies,
supported execution context, and expected evidence. None may contain
architecture decisions, ADR-owned constraints copied as local definitions,
technologies, classes, endpoints, schemas, algorithms, or detailed
implementation instructions.

Functional versus non-functional classification is determined by the product
Bolt's **primary outcome**, not by the repository, layer, files, or technology
it touches. A Functional Bolt may be
constrained by approved ADRs without becoming non-functional. A Non-functional
Bolt may affect a feature without becoming functional; it remains assigned to
`US-000-non-functional.md` and may link the affected feature US for
traceability. A Test Bolt is selected by its explicit QA Automation purpose and
approved TC parent; it is not reclassified as non-functional merely because
its deliverable is technical test code.

BUG reporting does not alter this classification rule. A Functional Analyst,
Developer, or QA may create a BUG, but it remains a draft until its nature,
evidence, and expected behavior are approved through `AITL-BUG-Approval`.
The BUG's dedicated Bolt may be drafted alongside it and is approved only after that checkpoint:

- if the defect violates functional behavior governed by a feature US or its
  Acceptance Criteria, a Functional Analyst approves the BUG and creates its
  functional Bolt under that feature US;
- if the defect violates an ADR-defined constraint or another technical
  expectation whose primary outcome is non-functional, an Architect or Tech
  Lead approves the BUG, and its non-functional Bolt is created under
  `US-000-non-functional.md`.

The reporter's role, the affected code layer, and the team that detected the
failure do not determine the Bolt nature. The standard ownership and
`AITL-BOLT-READY-Approval` route for the selected nature still apply independently
after `AITL-BUG-Approval`.

Test Cases may verify functional or non-functional expectations. Once a
TC is selected for QA Automation, the resulting work uses the Test
Bolt type and retains direct traceability to the TC, the TC's source Bolt, and
the original feature US/ACs or technical governing sources.

**Bolt numbering scope:** `BOLT-NNN` is sequential within its direct parent.
Functional and non-functional Bolt numbers are scoped to `US-NNN` (therefore
US-000 has the shared sequence for all non-functional Bolts); Test Bolt numbers
are scoped to their `TC-NNN`. Reusing the same `BOLT-NNN` under two different
parents is valid because the complete ID includes the parent. **Numbers are
never reused**: a rejected, deprecated or superseded Bolt leaves a gap in the
sequence of its parent; the next Bolt takes the next unused number. Gaps are
normal and must not be filled.

`BOLT-NNN` is zero-padded to **three digits, widening to four past 999**
(`BOLT-001` … `BOLT-999`, then `BOLT-1000` … `BOLT-9999`). Three digits are
the norm; the fourth exists because `US-000` collects every non-functional
Bolt of the project for its whole life and numbers are never reused, so a
long-lived project would otherwise meet a hard ceiling. Once a parent crosses
999, alphabetical filename order stops matching numeric order for that parent
— sort by the parsed number, not by filename. `US-NNN` and `TC-NNN` stay at
three digits.

**The boundary between governed artifacts is normative:**

> **Bolt = what work must be delivered.**  
> **ADR = which architectural decisions and non-functional constraints govern it.**  
> **SPEC = how the Bolt will be implemented.**

A Bolt references its governing sources; it does not copy or redefine them. If
technical implementation detail appears while the Bolt is being drafted, that
detail must be removed from the Bolt and considered later during SPEC
preparation. If it represents a durable architectural decision or a
non-functional constraint, it must first be governed through an approved ADR.

**Approval status:** Every Bolt starts as `draft` and remains so until it
passes **AITL-BOLT-READY-Approval**. Approval sets its `status` to `ready` and
confirms or changes its `test_integration` (§3.2.2); from then on the status
follows the transition table of §3.2.3. A Functional Analyst approves a
functional Bolt; an Architect or Tech Lead approves a non-functional Bolt; a
QA Lead, QA Automation Lead, Architect, or Tech Lead approves a Test Bolt. The
decision is made independently for every Bolt. A `draft` Bolt cannot be
assigned for execution, used to prepare a SPEC, start a V-Bounce, or authorize
a code change.

```mermaid
flowchart TD
    W["Requested work"] --> P{"Source and Bolt type?"}
    P -->|"Business behavior"| F["Functional Analyst creates Functional Bolt"]
    P -->|"Technical outcome"| N["Developer / Architect / Tech Lead creates Non-functional Bolt"]
    P -->|"QA Automation"| T["QA creates Test Bolt from a TC"]
    F --> HF{"Functional AITL-BOLT-READY-Approval"}
    N --> HT{"Technical AITL-BOLT-READY-Approval"}
    T --> HQ{"QA / technical AITL-BOLT-READY-Approval"}
    HF -->|"Approved"| S["SPEC defines how to implement it"]
    HT -->|"Approved"| S
    HQ -->|"Approved"| S
```

**Relationship with V-Bounce:** A **Bolt** is executed using **1..n
V-Bounces**, followed, when its `test_integration` is `required`, by its
integration test cycle (§3.2.2), until it meets its **Definition of Done**
(DoD).

> **Sizing and elapsed-time measurement:** The one-working-day target is based
> on **active delivery time** (AI agent generation + active human review +
> active rework), not developer person-hours and not approval wait time.
> End-to-end elapsed cycle time is still measured separately for flow
> improvement. Crossing into another day because of a later V-Bounce or human
> availability is accepted and never forces the Bolt to be divided.
> **AI-time** is the sub-metric for how long the agent spent generating
> the Bolt, SPEC revisions, code and MEMs. It is recorded as
> `duration_seconds` in the corresponding manifest generation object and
> may be aggregated for flow analysis.

> **AI-native estimation rule:** When estimating a Bolt's active delivery
> time, never price code creation as human typing time — code, tests and
> documents are agent-generated in minutes, and the dominant cost is human
> review and rework. Compose the estimate from the delivery cycle itself:
>
> `estimate ≈ expected V-Bounces × (agent generation + MEM/V-Bounce review
> budget for the risk_class) + SPEC review + acceptance + setup and integration test
> overhead`
>
> using the recommended review budgets (§3.0). Under this rule most low- and
> medium-risk Bolts land between **1 and 4 hours** of active delivery; a full
> working day is the ceiling, not the norm. An estimate exceeding one day is
> **first** a signal of manual-effort anchoring, and only after that a reason
> to evaluate a split. Retrospectives compare estimates against actual
> manifest durations and decomposed Bolt Lead Time to recalibrate (§3.7.4).

### 2.4.1 Implementation SPEC (one per Bolt)

**What it is:** The implementation plan that translates one approved Bolt into
precise, repository-grounded instructions for the code agent. The Bolt defines
what must be delivered; the SPEC defines how that Bolt will be implemented.

**One-to-one rule:** Every approved Bolt has exactly one current canonical
SPEC, and every SPEC references exactly one Bolt through its mandatory `bolt`
field. A SPEC may be revised and versioned, but a second concurrent SPEC for
the same Bolt or a SPEC spanning multiple Bolts is invalid.

**Stable naming:** Every SPEC uses the filename pattern
`SPEC-YYMMDD-HHmm-<description>.md`, where `<description>` is a kebab-case
ASCII slug in the project's `content_language` (§3.15), derived from the
Bolt. The same canonical SPEC retains its
original filename across material revisions; the revision number lives inside
the SPEC metadata and the manifest's `spec_revisions[]`, not in the filename.
This prevents filename collisions when multiple developers commit
concurrently and makes it unambiguous which SPEC belongs to which Bolt.

**Pre-SPEC evidence gate:** Before writing any SPEC content, the agent builds a
source inventory and verifies that every approval-bearing artifact it intends
to use is approved:

- The exact Bolt has `AITL-BOLT-READY-Approval`.
- If the Bolt corrects a BUG, that exact BUG has `AITL-BUG-Approval` and the
  BUG and Bolt reference each other.
- A functional Bolt's parent feature US and covered ACs have
  `AITL-US-Approval`; a non-functional Bolt references the permanent US-000
  container, which has no approval lifecycle; a Test Bolt's exact parent TC
  has `AITL-TC-Approval`.
- Every applicable ADR has `AITL-ADR-Approval`.
- The SPEC's integration test plan agrees with the Bolt's `test_integration`:
  `required` → `applicable` or `waived` (never `waived` for an
  integration-test-only BUG Bolt); `not-required` → `n/a` (§3.2.2). A mismatch
  is a blocking report; the way forward is a Bolt amendment re-approved at
  `AITL-BOLT-READY-Approval`, never a plan that contradicts the Bolt.
- No two active ADRs in the SPEC's `sources` contradict each other: if the
  decision log shows mutually exclusive decisions (e.g., two ADRs choosing
  different technologies for the same use case, neither superseding the
  other), the gate blocks the SPEC with a conflict report naming the ADRs
  and the required superseding ADR (§2.8, §3.5).
- Every Discovery conclusion used has `AITL-DISC-Approval`.
- Every Review finding used has `AITL-REV-Approval`.
- Every Adversarial Review finding used comes from a Verdict reached after all
  three sequential AREV phase approvals.
- Every Test Case used as a verification contract has `AITL-TC-Approval`. A
  Test Bolt must reference exactly one such approved TC.

If any required approval is absent, rejected, outdated or unverifiable, the
agent **does not generate the SPEC**. It emits only a blocking report naming
the artifact and required checkpoint. Optional DISC, REV and AREV mechanisms
do not need to exist; the rule applies when their conclusions or findings are
used by the SPEC.

**Required analysis context:** After the evidence gate passes, the agent must
inspect all material relevant to the Bolt rather than rely on the Bolt alone:

- the Bolt, its parent and covered ACs, plus its approved BUG when applicable;
- every applicable approved TC, including the exact source US/ACs and source
  Bolt from which its expected results were derived;
- all applicable approved ADRs and approved DISC/REV/AREV evidence;
- relevant raw files in `input/` and affected documents in `analysis/`;
- the existing implementation: code, tests, configuration, infrastructure,
  schemas, migrations, dependencies, APIs and repository conventions;
- related risks, open questions, prior MEMs and manifests when they affect the
  implementation baseline.

The SPEC records exact paths, identifiers, versions or commit baseline for the
sources it used. When evidence conflicts, context is incomplete, or an
architectural decision is unresolved, the agent stops and requests resolution;
it must not fill the gap with an assumption. An unresolved architectural
decision follows the ADR lifecycle and the SPEC may resume only after
`AITL-ADR-Approval`.

**Approval status:** A generated SPEC is a draft until a qualified human
records `AITL-SPEC-Approval`. This checkpoint verifies source completeness,
approval evidence, consistency with the Bolt, US/ACs and ADRs, fidelity to the
existing codebase, implementation feasibility, test strategy, risks and
rollback needs. Only then may a code-run or V-Bounce begin.

Any material change to the BUG when applicable, an applied TC, the Bolt, functional parent US/ACs, an applied ADR, a
used DISC/REV/AREV finding, or the relevant code baseline invalidates the
current SPEC approval. The same canonical SPEC must be revised and approved
again before technical execution continues.

**Material vs. cosmetic (default conservative):** a change is **material**
when it alters the source inventory, the planned behavior or AC mapping, the
implementation approach (files, components, interfaces, algorithms), or the
test strategy/expected evidence of the approved revision — including a
change to any governing source or to the repository baseline the SPEC
recorded. A change is **cosmetic** only when it is limited to typos,
formatting or wording that cannot alter behavior or implementation.
When in doubt, treat the change as **material** and re-approve: silent or
merely logged mid-run SPEC changes are forbidden (§3.2.1).

```mermaid
flowchart TD
    B["Approved Bolt"] --> E["Collect governed evidence and repository context"]
    E --> G{"All required approvals valid?"}
    G -->|"No"| X["Stop: report blocking artifacts"]
    X --> E
    G -->|"Yes"| S["Generate one canonical SPEC for the Bolt"]
    S --> H{"AITL-SPEC-Approval"}
    H -->|"Changes requested"| S
    H -->|"Approved"| V["Code-run / V-Bounce may begin"]
```

## 2.5 V-Bounce (standard execution form)

**What it is:** A complete, traceable, and governed cycle in which an **AI
agent transforms a SPEC approved through `AITL-SPEC-Approval` into an implementation package submitted
for human validation**.

During its autonomous execution, the agent generates the required design,
code, tests (integration tests included, which it builds and checks
statically), configuration, and documentation; runs the hermetic tests;
detects failures; and iterates as many times as necessary until the package
is ready for review. These autonomous internal iterations are part of the same
V-Bounce and do **not** increase the V-Bounce count. Tests that need a running
environment are executed later, in the Bolt's integration test cycle when its
`test_integration` is `required` — a Test Bolt's automation in its TC's run
cycle (§3.2.2, §3.6).

Once execution is complete, the agent **must create exactly one MEM for that
V-Bounce**, update the Bolt manifest, and pause for human validation. The MEM
is not an optional summary: it is the mandatory narrative and evidence index
of the execution. The reviewer validates it against the generated code, the
actual diff, tests, gates, and supporting evidence, then records
`AITL-MEM-Approval`. This checkpoint decides the current V-Bounce:

- If `AITL-MEM-Approval` is approved, the V-Bounce is completed and approved.
- If changes are requested, the MEM and its V-Bounce remain in the historical
  record with a non-approved outcome. The agent's next autonomous execution,
  incorporating that feedback, is a **new V-Bounce** with a new MEM.

The Bolt's `status` follows its V-Bounces (§3.2.3): `in-development` from the
start of a V-Bounce, `blocked` while a blocked V-Bounce awaits its MEM review;
an approved latest V-Bounce is the **Development Completed** event, after
which the Bolt becomes `in-integration-testing` (`test_integration: required`,
plan `applicable`) or, when no integration test run is pending
(`test_integration: not-required`, or a `waived` plan),
`ready-for-acceptance`; `changes_requested` or `rejected` keeps it
`in-development`. None of these is `done`, which requires
`AITL-BOLT-DONE-Approval`.

Each V-Bounce is recorded as a separate entry in the Bolt manifest's
`v_bounces[]`. The
total number required to obtain human approval is measured as **V-Bounces per
Bolt**.

**Role:** Guarantees quality from minute zero (tests written with the change;
the hermetic ones executed in the V-Bounce's own loop, the integration tests in
the Bolt's integration test cycle when its `test_integration` is `required`),
preserves the history of every human-requested
rework cycle, and makes first-pass quality measurable.

## 2.6 User Stories (US) + Acceptance Criteria (AC)

**US:** "As a [role] I want [capability] so that [value]."

**AC:** Verifiable conditions in **Given/When/Then** style (functional
criteria only).

**Role:** They act as the **human ↔ AI contract**: they drive code and test
generation and enable quick validation.

**Approval status:** A generated or updated feature User Story remains a
**draft** until a Functional Analyst approves it at **AITL-US-Approval**. The
approval confirms that the US and its ACs faithfully represent the evidence in
`input/` and the current understanding in `analysis/`. Functional Bolts may be drafted and refined while the US is still draft;
`AITL-US-Approval` gates their `AITL-BOLT-READY-Approval`. US-000 is outside this
approval lifecycle.

**Evolution User Stories (changing behaviour already delivered):** A feature US
whose behaviour has been delivered — its Bolts accepted, each `done` — is
history: approved, decomposed, delivered and accepted, its AITL decisions
immutable (G36). When that behaviour must change, the work lands in a **new
feature US that declares what it evolves**; the delivered one is never
reopened, edited or deprecated by default. `done` is terminal for the Bolts
that delivered it, a feature US carries no state beyond `draft`, `approved` and
`deprecated` (§3.15), and "reopened" is not a state this methodology defines
(G39). Two exceptions are explicit: the delivered US is
marked `deprecated` when its behaviour is **withdrawn** rather than extended,
and a US **closed in error** — where the Definition of Done was in fact not
met — is corrected as a closure error, which is not new scope.

An evolution US states the **complete target behaviour** of the feature, never
only the increment: what already exists **and** what changes, as the behaviour
must work once the story is delivered. The reason is structural: a closed US
may be moved to `_archive/`, which is deliberately outside the agents' scans
(W20), so a story that describes only its delta and points at the previous one
leaves the current truth in a document nobody opens. Every acceptance criterion
is therefore marked **`existing`** — already delivered, naming the US that
delivered it — or **`delta`** — what this story changes or adds. **A criterion
that cannot be restated unchanged in substance is `delta` by definition.** Where
the evolution US and the delivered one read differently, the evolution US is the
behaviour **in force** and the delivered one remains the record of what was
approved at its own moment; it is never rewritten to match. When restating the
delivered criteria would take the story past fifteen criteria, the
`existing` ones are grouped by capability — one line per group, naming the
delivered US — while every `delta` criterion stays in full.

**Only `delta` criteria produce Bolts and story points.** The `existing` ones
are completeness, and legitimate regression scope for Test Cases (§2.6.1); no
Bolt claims them, and a Bolt or SPEC that does is a warning (W22). The link to
the evolved US is recorded in the story's own frontmatter and in its manifest
`sources`: there is no manifest field for it and the v5 schemas reject unknown
keys (G23), so the chain is traversable in the document layer, not in the
machine layer.

**Routing — which artifact.** Behaviour **differs from what was approved** (it
is wrong): a `BUG` with its dedicated Bolt (§2.16). Behaviour is **as approved
but must now be different**: an evolution US. **No behaviour change** at all
(refactor, infrastructure, hardening, debt): a non-functional Bolt under
`US-000` (§2.4). The authoring shapes for both story kinds ship as templates in
`functional/user-stories/`.

**Story points (functional complexity signal):** A feature US may carry a
`story_points` value on the **Fibonacci scale (1, 2, 3, 5, 8, 13)**
expressing the **relative complexity of its functional scope** — the number
and intricacy of its ACs, business rules, flows, integration surfaces and
unknowns — never time. The agent may propose the value when drafting the US;
the Functional Analyst confirms or corrects it as part of `AITL-US-Approval`.
Story points are **informational only**: no checkpoint, gate or guardrail
depends on them, weekly planning keeps forecasting with throughput and Bolt
Lead Time (§4.3), and no velocity metric or performance target may be derived
from them. Converting points into hours defeats their purpose — the time
dimension belongs to Bolt estimation and its AI-native estimation rule
(§2.4). US-000 never carries story points, and an **evolution US** scores only
its `delta` criteria — the complexity of what changes — never the delivered
behaviour it restates for completeness. Retrospectives may correlate
points against the US's actual aggregated Bolt Lead Time to recalibrate
future scoring (§3.7.4).

**Scoring rubric (how the agent proposes a value):** score each dimension
against the anchors below and take the **highest dimension, never the
average** — complexity is dominated by the worst factor (a US with 3 ACs but
one poorly documented external integration is a 5, not a 3). When approved
USs already exist in `functional/INDEX.md`, score relative to them; for the
first USs of a project, use the anchors as absolute. Open OQs in
`analysis/open-questions/` targeting the US are objective evidence for the
*Unknowns* dimension — check the OQ INDEX rather than guessing.

| Points | ACs | Business rules | Flows | Integrations | Unknowns |
|--------|-----|----------------|-------|--------------|----------|
| **1** | 1–2 trivial | None beyond field validation | One linear flow | None | None |
| **2** | 2–3 | Simple, single-entity | One flow, minor variation | None | None |
| **3** | 3–5 | Several, still single-entity | Main flow + alternate/error flows | One internal | None material |
| **5** | 5–8 | Cross-entity interactions | Multiple flows or roles | One external, or a new domain concept | Minor, already scoped |
| **8** | Many, hard to enumerate | Rule interactions need their own table | Multiple personas/journeys touched | Several, or one poorly documented | Open OQs targeting this US |

**13 is a splitting signal, not a target size:** a US that exceeds the 8
anchors on any dimension scores 13, and the agent proposes decomposing it
before `AITL-US-Approval` — the US-level equivalent of the one-day Bolt
ceiling (§2.4). The Functional Analyst may still approve it as 13 when
splitting is genuinely not viable.

**Expected decomposition bands (plausibility check, never a conversion):**
story points relate to the expected **number of independently deliverable
outcomes** — never to hours (W18):

| Story points | Typical decomposition |
|--------------|----------------------|
| 1–2 | 1–2 Bolts |
| 3–5 | 2–4 Bolts |
| 8 | 4+ Bolts |
| 13 | splitting signal — decompose the US first |

A decomposition falling far outside its band is a **signal to re-examine
either the score or the Bolt slicing** — never a target to force. Bolts are
sliced by independently deliverable outcomes (§2.4), and the band only
cross-checks that the complexity score and the resulting structure tell a
consistent story.

**Permanent non-functional container (US-000):**
`functional/user-stories/US-000-non-functional.md` is the permanent
traceability parent for every Bolt whose primary outcome is non-functional.
Despite its filename and location, it is a container rather than an actual
User Story: it has no Acceptance Criteria, approval status, approver, or AITL
checkpoint. It does not represent a user-facing capability and does not
replace ADRs or quality gates; it ensures that technical work is still
governed through an individually approved Bolt, SPEC, V-Bounce, MEM, manifest,
and human validation.

### 2.6.1 Test Case (TC) and QA Automation

**What it is:** A `TC-NNN-<description>.md` is an implementation-independent
verification contract. It defines preconditions, input data, steps or
stimulus, expected results, covered ACs or technical constraints, and the
evidence needed to determine pass or fail.

**Cardinality and required traceability:** Every TC references exactly one
approved `source_bolt`. A functional TC also records one `source_us` and one or
more `covered_acs`; a non-functional TC records `source_us: US-000` for
traceability plus every governing ADR or other technical source. A TC may
contain one coherent scenario with its variants or data sets, but it must be
split when independent outcomes, approvals, or setup would make pass/fail
ambiguous. Its minimum metadata includes `id`, `type` (`functional` or
`non-functional`), `source_bolt`, `source_us`, `covered_acs`, `governing_sources`,
`status`, `owner`, and the `AITL-TC-Approval` record.

**Regression Test Cases of an evolution US (§2.6).** A TC that verifies a
criterion marked `existing` records `source_us` = the **evolution** US (the
behaviour in force, with its own criterion numbering), `covered_acs` = that
criterion's id **in the evolution US**, and `source_bolt` = the Bolt of the
**delivered** US that produced the behaviour — the only Bolt whose outcome is
that behaviour, since no Bolt of an evolution US claims an `existing` criterion.
**This pair legitimately crosses User Stories:** `source_bolt` and `source_us`
are independent references and nothing requires the Bolt to belong to the
recorded US. The test basis is unaffected: an `existing` criterion is restated
unchanged in substance (§2.6), so its expected result still derives from
approved intent and never from the implementation (G06).

**Test-basis rule:** The expected behavior of a Test Case must be derived from
approved intent, never from the current implementation:

- A functional TC is based on the approved feature US/ACs and the exact
  approved functional Bolt whose outcome it verifies.
- A non-functional TC is based on the exact approved non-functional Bolt and
  its approved ADRs or other governed technical sources. US-000 provides
  traceability only and never supplies expected results.
- A BUG-related TC also references the approved BUG and its expected behavior.

Existing code, existing automated tests, configuration, schemas and runtime
behavior may be inspected only to understand interfaces, setup, data,
feasibility, regression surface and execution constraints. They are
**contextual evidence, not the test oracle**. The agent must not copy current
behavior into the expected result merely because the code already behaves
that way. When implementation behavior conflicts with the approved test
basis, the conflict is reported and routed through the BUG, analysis, ADR, or
change-control lifecycle rather than silently normalizing the TC to the code.

**Independent lifecycle:** A TC remains `draft` until a qualified human
records `AITL-TC-Approval`. Functional expected results require QA review plus
Functional Analyst or delegated business-domain approval. Non-functional
expected results require QA review plus the applicable Architect, Tech Lead,
Security, Performance, Data, or other technical owner. Approval confirms the
test basis, coverage, preconditions, data, steps, expected results, and
pass/fail evidence; it does not approve implementation code. Role routing here
is guidance, not a gate: if a named role has no holder, the available qualified
human records the approval, noting the self-assigned role.

When QA verification is applicable, Test Cases are drafted after
`AITL-BOLT-READY-Approval` and approved **before the implementation SPEC is
generated**, so the verification contract exists independently of both the
technical plan and the generated solution.
A functional Bolt normally has one or more approved TCs. A justified `n/a`
may be recorded during Bolt approval for documentary or purely internal work
that has no QA-verifiable outcome.

**QA Automation:** Automating a TC is optional, but any generated or modified
QA Automation code requires one or more dedicated Test Bolts parented by that
TC. The rules are:

- a TC may originate Test Bolts; its approval gates their readiness;
- each Test Bolt names that one TC in its `us` parent field and uses it as
  its direct parent;
- a TC may originate 1..n Test Bolts when automation has independently
  deliverable outcomes; elapsed time or additional V-Bounces alone do not
  justify a split;
- each filename is `TC-NNN.BOLT-NNN-<description>.md`;
- the parent TC satisfies the Test Bolt's verification-contract requirement;
  no recursive "TC for the Test Bolt" is created;
- every such Bolt receives its own `AITL-BOLT-READY-Approval`, one canonical SPEC,
  `AITL-SPEC-Approval`, 1..n V-Bounces, one MEM per V-Bounce,
  `AITL-MEM-Approval`, and `AITL-BOLT-DONE-Approval`;
- its SPEC treats the approved TC as a governed source and defines how to
  implement the automation without changing the TC's expected result;
- the Bolt authorizes QA test code and supporting test assets only. If
  automation exposes a product defect, production-code correction follows the
  approved BUG → dedicated Bolt lifecycle.

Test Bolt numbering is scoped to its parent TC. For example,
`TC-002.BOLT-001-prepare-invoice-test-data.md` and
`TC-002.BOLT-002-automate-invoice-download.md` are two independently approved
automation slices for `TC-002`; the number does not imply another US or a
shared SPEC.

Test Bolts are distinct from the developer tests required by a product or
BUG Bolt. Unit, integration, and strict TDD regression tests required by that
implementation SPEC remain part of the original Bolt — written in its
V-Bounce, the integration tests executed in its integration test cycle (§3.6); they
do not require a second QA Automation Bolt unless an approved TC is later
selected for independent QA automation.

```mermaid
flowchart TD
    S["Approved US/AC + approved Bolt"] --> T["Draft TC from intended behavior"]
    T --> H{"AITL-TC-Approval"}
    H -->|"Changes requested"| T
    H -->|"Approved"| A{"QA Automation selected?"}
    A -->|"Yes"| B["1..n TC-NNN Test Bolts"]
    B --> F["Each Bolt follows SPEC → V-Bounce → MEM"]
```

## 2.7 Non-functional constraints (ADR-owned)

The expression **Non-Functional Requirements (NFRs)** is used only as shorthand
for performance, security, availability, compliance, observability, and other
quality constraints that are **defined and governed inside approved ADRs**.
They are not independent DevFlow artifacts and must not be defined in User
Stories, Acceptance Criteria, Bolts, or SPECs. Functional requirements are
the mirror image: they are defined in the feature User Story and its
Acceptance Criteria, and are never ADR content.

When implementing or improving an ADR-defined constraint requires dedicated
work, a Developer, Architect, or Tech Lead creates one or more non-functional
Bolts under `US-000-non-functional.md`. The approved ADR remains the governing
source; US-000 and its Bolts provide assignment, execution, and measurement
traceability. No Functional Analyst approval is required for this technical
work definition.

## 2.8 ADR (Architecture Decision Record)

**What it is:** A short record of a relevant technical decision (context,
alternatives, decision, consequences). It records architecture decisions and
constraints, not functional behaviour: functional requirements live in the
feature User Story and its ACs.

**Role:** Maintains the traceability of trade-offs and prevents forgetting. It
must be drafted and approved before a SPEC may rely on its decision. If an
unresolved architectural decision emerges during SPEC preparation or
execution, work stops, the ADR lifecycle is completed, and the canonical SPEC
is revised and re-approved before the code-run continues.

**Approval status:** Every ADR remains a **draft** until an Architect or Tech
Lead approves it at `AITL-ADR-Approval`. Approval of a User Story, Bolt, SPEC,
or V-Bounce does not implicitly approve the ADR. A draft ADR cannot govern a
SPEC, establish a non-functional constraint, authorize an exception, or be treated as an accepted
architecture decision.

## 2.9 DoR / DoD (Definition of Ready / Definition of Done)

**DoR (evaluated inside `AITL-BOLT-READY-Approval`):** A functional Bolt has
an approved feature parent US and clear covered ACs; a non-functional Bolt is
correctly linked to US-000 with a clear technical outcome and governed source;
a Test Bolt has exactly one approved parent TC. Relevant ADRs are approved
with their architectural decisions and non-functional constraints; risks are
identified; context is accessible to the AI agent; no `open` or
`in-validation` Open Question targets the Bolt's parent or governing
artifacts; and completion evidence is defined, and the Bolt declares
`test_integration` with its one-line reason (§3.2.2). DoR is a readiness
criterion, not a separate sign-off or checkpoint.

**V-Bounce completion:** The applicable hermetic checks and tests (§3.6) are
green (or the applicable gates are explicitly waived), exactly one complete MEM is created, the Bolt manifest is updated,
and an approved
`AITL-MEM-Approval` recorded for that MEM and V-Bounce. Creating the MEM is
inevitable; approving it is what approves and completes the V-Bounce. A
request for changes retains the unapproved MEM and starts a new V-Bounce. An
approved latest V-Bounce is the **Development Completed** event; one awaiting
review, or reviewed `changes_requested` or `rejected`, keeps the Bolt
`in-development` (`blocked` while a blocked V-Bounce awaits review) (§3.2.3).

**DoD (per Bolt):** Every V-Bounce has its own immutable MEM and recorded
`AITL-MEM-Approval` decision; earlier `changes_requested` attempts remain
unapproved history, while the latest V-Bounce and MEM are approved. The Bolt
is `ready-for-acceptance`: a passing integration test run covers its latest
approved change; or it is `not-required` (plan `n/a` with a reason); or
`required` with a plan `waived` by the project's ADR (plus the executed
compensating-control evidence on `high`/`critical`); or its pending
integration testing was waived by a human with a written reason (§3.2.2).
Every applicable ADR is approved, gates have evidence, traceability is
complete, the result is ready for demo, and `AITL-BOLT-DONE-Approval` has made
the Bolt `done`. Development completion and Bolt acceptance are separate
lifecycle transitions.

## 2.10 Quality Gates (automated)

**What they are:** Universal plus SPEC-selected conditional checks in CI/CD
(tests, coverage, SAST/DAST, licenses/SBOM, performance smoke, policies).
Each gate runs at one level — the V-Bounce, the Bolt's integration test cycle or
the release (§3.6).

**Role:** Approved TCs, ACs and ADR-defined constraints are **translated** into
automated checks; a Bolt cannot reach **Done** while an applicable gate is
`fail`. A waiver requires an approved ADR; a non-applicable gate is `n/a` with
a reason.

## 2.11 Deployment Unit

**What it is:** An artifact ready to be promoted (image / function / IaC)
that has **already passed** every gate and is demonstrable.

**Role:** The "releasable" output of one or more Bolt sequences within a
Unit.

**Deployment definition:** for DORA purposes, a **production
deployment** is the promotion of a Deployment Unit to the **production
environment**, made traceable by a release tag, image digest or equivalent
identifier that links the deployment event to its included Bolts and
commits. Merge to `main` is **not** a production deployment by itself.
Teams define the granularity (per service, per platform, or whole-platform)
as part of their internal DORA baseline (§3.7.1) and record it in the
project's metrics conventions; the definition is reviewed in each retro.

## 2.12 Project memory

**What it is:** The set of artifacts (transcripts, US/AC, ADRs, code, tests,
metrics) accessible to AI and the team.

**Role:** Lets the AI generate with the correct context and ensures
**end-to-end** traceability.

**MEM (implementation memory):** Every V-Bounce generates exactly one
`MEM-YYMMDD-HHmm-<description>.md` after autonomous implementation and
verification, before human review.

**Stable naming across V-Bounces:** Every MEM produced for the same Bolt and
its canonical SPEC must reuse exactly the same kebab-case
`<description>` slug — ASCII, in the project's `content_language` (§3.15).
Only the creation timestamp (`YYMMDD-HHmm`) changes from
one V-Bounce to the next. If the V-Bounces occur on the same day, normally only
`HHmm` differs; if they cross a date boundary, `YYMMDD` changes as well. Do not
append `v2`, `retry`, `fix`, `bounce-2`, or similar suffixes. The V-Bounce
number belongs in the MEM metadata and the manifest's `v_bounces[]`, not in
the filename. Implementations must reserve filenames atomically; two V-Bounces
of the same Bolt never complete in the same minute, but if a collision would
otherwise occur, the later MEM is created in the next minute. Overwriting,
suffixing or reusing the earlier MEM is forbidden.

Example for three V-Bounces of one Bolt/SPEC:

- `MEM-260802-1015-invoice-download.md`
- `MEM-260802-1128-invoice-download.md`
- `MEM-260803-0904-invoice-download.md`

These are three immutable execution records for the same canonical plan; they
are not versions that overwrite one another.

At minimum, the MEM records:

- the V-Bounce iteration, Bolt, canonical SPEC version, repository baseline,
  and applicable ADRs;
- a concise summary of what was implemented and why;
- every file added, modified, renamed, or deleted, with its path, change type,
  and reason;
- tests and quality gates executed, their commands or evidence references, and
  results;
- implementation decisions, SPEC deviations, assumptions, manual
  interventions, unresolved risks, and follow-up items;
- links or identifiers for the diff, commit/PR when present, and the cumulative
  Bolt manifest entry;
- the final `AITL-MEM-Approval` record: reviewer, role, timestamps, decision,
  review evidence, comments, and findings.

The agent creates the MEM and never self-approves it. The MEM has no mutable
approval `status`: its review state is derived from the associated
`AITL-MEM-Approval` record—no decision means pending review, `approved` means
an approved V-Bounce, and `changes_requested` or `rejected` means an immutable
historical attempt that did not advance. The MEM is mandatory for every
V-Bounce attempt, including one that reaches a
stop condition, remains blocked, exhausts its turn budget, or cannot obtain
green tests. The qualified human must inspect the underlying evidence—not
merely trust the narrative—and record `AITL-MEM-Approval` in both the MEM and
the manifest. Approval is possible only when the implementation package and
required gates satisfy the approved SPEC. After the human decision, the MEM is
immutable. An approved MEM makes its V-Bounce approved; a non-approved MEM
remains evidence of that attempt, and the next attempt receives a new MEM.
When that approved MEM belongs to the latest V-Bounce, it is the **Development
Completed** event: the Bolt becomes `in-integration-testing`
(`test_integration: required`, plan `applicable`) or, when no integration test
run is pending (`test_integration: not-required`, or a `waived` plan),
`ready-for-acceptance` (§3.2.3). It does not make the Bolt `done`; acceptance
remains governed by `AITL-BOLT-DONE-Approval`.

## 2.13 Discovery

**What it is:** A focused, traceable investigation (`DISC-NNN`) used to reduce
an important uncertainty **before a User Story or Bolt is created or
materially refined**. Typical Discoveries examine an external API, an
unfamiliar library or framework, a legacy-system behavior, an integration
constraint, a technology option, data availability, or another question that
must be understood before the team can define the right work.

**Role:** A Discovery creates evidence and conclusions for backlog definition;
it does not inspect an existing project artifact for quality and does not
authorize implementation. Its approved findings may update `analysis/` and may
support the creation or refinement of User Stories, Bolts, ADRs, risks, or
other governed artifacts.

**Need-driven in the E2E flow:** A Discovery is not required for every User
Story or Bolt. Any stakeholder or team member may initiate one when a material
unknown would otherwise force the team to guess. Once initiated, it remains a
draft and its conclusions cannot be used as governed input until a qualified
human records `AITL-DISC-Approval`.

`AITL-DISC-Approval` confirms that the research question was answered
with adequate evidence, the limits and assumptions are explicit, and the
conclusions are reliable enough to guide backlog or architecture work. It does
not approve any downstream artifact. Each User Story, Bolt, ADR, or other
artifact created or changed from the Discovery follows its own lifecycle and
AITL approval.

```mermaid
flowchart TD
    Q["Material unknown before a US or Bolt"] --> D["Run DISC investigation"]
    D --> F["Document evidence, limits, and conclusions"]
    F --> H{"AITL-DISC-Approval"}
    H -->|"Changes requested"| D
    H -->|"Approved"| A["Update analysis and inform backlog decisions"]
    A --> U["Create or refine USs, Bolts, ADRs, or risks"]
    U --> O["Each artifact follows its own approval lifecycle"]
```

**Discovery versus Review:** A Discovery researches an **unknown that must be
understood before defining work**. A Review (`REV`) examines **documentation,
code, tests, ADRs, or any other existing project artifact or characteristic**
and produces findings about it. A Review may review a Discovery document, but
that optional Review and `AITL-REV-Approval` are separate from the
Discovery's own `AITL-DISC-Approval`.

| Dimension | Discovery (`DISC`) | Review (`REV`) / Adversarial Review (`AREV`) |
|-----------|--------------------|----------------------------------------------|
| Starting point | A material unanswered question before a US or Bolt is defined or refined | An existing project artifact, implementation, characteristic, or concern to inspect |
| Primary purpose | Learn, gather evidence, and reduce uncertainty | Evaluate, challenge, and identify findings |
| Typical subjects | External APIs, unfamiliar libraries, legacy behavior, technology options, data or integration constraints | Documentation, code, tests, USs, Bolts, ADRs, SPECs, architecture, security, performance, or process |
| Governed output | Approved evidence, assumptions, limits, and conclusions | Approved findings; AREV exposes them only after its approved Verdict |
| AITL governance | `AITL-DISC-Approval` | `AITL-REV-Approval`, or the three sequential AREV phase approvals |
| Downstream effect | Informs analysis and the creation or refinement of governed artifacts | May create or update any governed artifact |

An AREV is therefore a specialized adversarial form of Review, not a
Discovery.

A Discovery does not create an exception to the rule that no code-related
change may exist without an approved Bolt. If the research requires executable
prototype or spike code, that experiment must be authorized by an approved
non-functional Bolt under `US-000-non-functional.md` before any code is
generated.

> **Why this is not circular (§2.4):** the spike Bolt is an **investigation
> instrument**, not the product Bolt the Discovery informs. The Discovery
> may proceed entirely documentarily without code; only when it needs an
> executable experiment does it spawn a separate non-functional spike Bolt
> (a general technical outcome under US-000). The experiment's results then
> inform the Discovery's conclusions, which feed the *product* USs/Bolts.
> The spike Bolt does not depend on the Discovery's conclusions to exist —
> it depends only on the decision to experiment.

## 2.14 Review

**What it is:** An open, structured examination (`REV-NNN`) that may be
initiated by **any stakeholder or team member**, regardless of role. A Review
may examine functional or non-functional characteristics, User Stories,
Bolts, ADRs, SPECs, code, tests, architecture, security, performance,
processes, risks, documentation, or any other relevant concern.

**Optional in the E2E flow:** A Review is never a mandatory stage of the
standard end-to-end flow. Each stakeholder is responsible for initiating one
when additional scrutiny, evidence, or a second perspective is warranted.
Once initiated, however, its approval and traceability rules are mandatory.

**Role:** A Review produces findings that can feed the creation or update of
**any DevFlow artifact**. Findings remain draft and cannot be used as governed
input until a qualified human records `AITL-REV-Approval`.

`AITL-REV-Approval` validates the Review and its findings: scope,
supporting evidence, clarity, classification, and actionability. It does not
approve any downstream artifact or implementation. Every artifact created or
updated from an approved finding follows its own lifecycle and applicable
AITL approval. If a finding requires a code-related change, that change must
be authorized by an approved Bolt before a SPEC or V-Bounce can begin.

```mermaid
flowchart TD
    S["Any stakeholder starts a REV"] --> R["Review any artifact or concern"]
    R --> F["Draft findings"]
    F --> H{"AITL-REV-Approval"}
    H -->|"Changes requested"| R
    H -->|"Approved"| A["Actionable approved findings"]
    A --> D["Create or update affected artifacts"]
    D --> O["Each artifact follows its own approval lifecycle"]
```

## 2.15 Adversarial Review

**What it is:** A structured review protocol (`AREV-NNN`) in **three
sequential phases** — Critique → Defense → Verdict — performed by independent
agent roles. A Challenger identifies weaknesses, a Defender responds to the
critique and explains or revises the reasoning, and a Judge produces an
impartial verdict.

**Principle:** A model reviewing its own work may preserve the same blind
spots. Independent adversarial roles introduce competing perspectives, while
human approval governs every transition.

**Scope and initiation:** Any stakeholder or team member may initiate an
Adversarial Review. It may examine any functional or non-functional
characteristic or artifact, including User Stories, Bolts, ADRs, SPECs, code,
tests, architecture, security, performance, risks, processes, and
documentation. It may be Bolt-bound, themed, or ad hoc.

**Optional in the E2E flow:** An Adversarial Review is never a mandatory stage
of the standard end-to-end flow, including for high- or critical-risk work.
Each stakeholder is responsible for triggering it when adversarial challenge
would add value. Once initiated, all three phases and their approvals are
mandatory and sequential. **Running an AREV requires at least three models**
available — one each for the Critique, the Defense and the Verdict — so the
Judge is always a neutral third model (§3.13, G37); a single operator running
three models is valid. A team without a third model does not initiate the
AREV; an AREV already open that cannot reach a neutral Verdict is set
`cancelled` (§3.15).

**Not a stage of the V-Bounce:** an AREV is a standalone governance mechanism,
exactly like a Review (§2.14). It is **not** a step of the V-Bounce anatomy
(§3.3), it needs no Bolt, SPEC, User Story or any other prior artifact to
exist, and it never opens, closes or modifies a V-Bounce. Its approved Verdict
produces findings, and nothing else:

- A **Bolt-bound** AREV examines the closed package of a completed V-Bounce
  (diff, tests, gates, MEM, manifest), and its approved Verdict is a
  **pre-filter for the `AITL-MEM-Approval` decision** (§3.0). If the reviewer
  then requests changes, the ordinary rule applies — the MEM stays as
  unapproved history and the next agent execution is a **new V-Bounce with a
  new MEM and a new `v_bounces[]` entry** (§2.5). AREV-driven rework is
  therefore measured like any other rework; it is never absorbed silently into
  the V-Bounce that produced it.
- A **themed** or **ad hoc** AREV is attached to no Bolt at all. Its approved
  findings route to their own artifacts — BUG, Bolt, ADR, RISK, DISC — each
  with its own lifecycle and AITL approval, exactly as REV findings do
  (§2.14).

**Mandatory phase approvals:** Each phase remains draft until its named human
checkpoint is approved:

1. `AITL-AREV-CRITIQUE-Approval`
2. `AITL-AREV-DEFENSE-Approval`
3. `AITL-AREV-VERDICT-Approval`

If changes are requested, that phase is revised and submitted again; the next
phase cannot begin until the current one is approved. Critique and Defense are
intermediate arguments and do not create usable findings. Only an approved
Verdict produces actionable findings. Any artifact created or updated from
those findings must still follow its own lifecycle and applicable AITL
approval.

**Manual agent/model change:** Agent or model selection for AREV is performed
manually by the human in whichever development tool is being used. After each
phase checkpoint is approved, the human selects the agent/model for the next
phase before launching it. DevFlow does not auto-switch models, create a
regression-eval Bolt, or require a model-change ADR for this operational
selection. Each phase file records the agent/model that produced it so that
the AREV remains self-contained and auditable.

**No manifest impact:** AREV status, phase approvals, selected models and
Verdict are recorded only in the `AREV-NNN` artifacts. They are not written to
or derived from the Bolt manifest. If an approved Verdict causes a downstream
US, BUG, BOLT, ADR or SPEC change, that downstream artifact follows its own
normal lifecycle and is traced through its own identifiers.

```mermaid
flowchart TD
    C["Critique"] --> HC{"AITL-AREV-CRITIQUE-Approval"}
    HC -->|"Changes requested"| C
    HC -->|"Approved"| D["Defense"]
    D --> HD{"AITL-AREV-DEFENSE-Approval"}
    HD -->|"Changes requested"| D
    HD -->|"Approved"| V["Verdict"]
    V --> HV{"AITL-AREV-VERDICT-Approval"}
    HV -->|"Changes requested"| V
    HV -->|"Approved"| F["Actionable approved findings"]
```

> **Reference:** Concept inspired by
> [Adversarial Coding — Using Competing Models as Code Reviewers](https://www.subaud.io/adversarial-coding-competing-models-reviewers/)
> (Court Schuett, 2026).

## 2.16 BUG (confirmed defect)

**What it is:** A traceable record (`BUG-NNN`) of a confirmed difference
between expected and actual system behavior. A BUG captures the observation,
evidence, affected context, reproduction conditions, expected result, actual
result, impact, severity, and known links. It is not a work authorization and
does not contain the implementation plan.

**Who may create it:** A Functional Analyst, Developer, or QA may create a BUG,
depending on who detects and can document the defect. Creator role does not
determine whether the BUG is functional or non-functional.

**Approval gates the Bolt's readiness:** Every BUG starts as `draft`. Its one
dedicated Bolt may be drafted and refined alongside it, but
`AITL-BOLT-READY-Approval` is refused until the competent human validates that the defect is real,
the expected behavior and evidence are sufficiently clear, and its nature and
parent route are correct:

- a Functional Analyst records `AITL-BUG-Approval` for a functional BUG;
- for a non-functional BUG, an Architect or Tech Lead is the recommended
  approver when `severity: critical`, and any team member for `severity: high`,
  `medium`, or `low`.

**Guidance, never a gate:** the recommended approver above is advice, not a
precondition. Any qualified team member — the BUG's own author included — may
record `AITL-BUG-Approval` at any severity, noting the self-assigned role where
the recommended holder is unavailable. (The AI self-approval prohibition,
G18/G24, is a separate axis and still holds.)

Approval confirms the BUG as governed input; it does not approve its future
Bolt, SPEC, implementation, MEM, or acceptance. Each keeps its own checkpoint.

**Mandatory classification and Bolt:** Every BUG receives exactly one
dedicated Bolt:

| BUG nature | Violated expectation | Dedicated Bolt parent | Bolt creation and approval |
|------------|----------------------|-----------------------|----------------------------|
| **Functional BUG** | Approved feature behavior or Acceptance Criteria | The affected feature `US-NNN` | Functional Analyst creates/refines and separately approves the Bolt through `AITL-BOLT-READY-Approval` |
| **Non-functional BUG** | ADR-defined constraint or another technical expectation with a non-functional primary outcome | `US-000-non-functional.md` | Developer, Architect, or Tech Lead creates/refines the Bolt; the dedicated Bolt's `AITL-BOLT-READY-Approval` mirrors the parent BUG's routing — Architect or Tech Lead recommended when `severity: critical` — but is guidance, never a gate: any qualified team member, the Bolt's own author included, may approve it at any severity |

The BUG and dedicated Bolt reference each other. A BUG cannot be fixed under an
unrelated existing Bolt, directly from a ticket, or as an untracked addition to
another V-Bounce. If the defect contains several independently confirmable
defects or outcomes, `AITL-BUG-Approval` may request decomposition into
independently traceable BUGs. Each resulting BUG is approved separately and
receives its own dedicated Bolt. Additional V-Bounces or continuation on the
next working day are not, by themselves, reasons to split the BUG or its Bolt.

**One SPEC per BUG Bolt:** The normal SPEC cardinality applies. The dedicated
Bolt has one canonical SPEC, and that SPEC explicitly references the approved
BUG and all relevant approved governed evidence. The BUG does not bypass
`AITL-BOLT-READY-Approval`, `AITL-SPEC-Approval`, or any approval required by the
US, ADR, DISC, REV, or AREV evidence used by the SPEC. An unapproved or stale
BUG blocks both its Bolt creation and any SPEC that attempts to use it.

**Strict TDD correction rule:** Every BUG SPEC must prescribe this order inside
the same V-Bounce:

1. create or modify the smallest automated test that reproduces the reported
   defect against the pre-fix behavior;
2. execute that test and record objective evidence that it fails for the
   expected reason (**red**);
3. only after the red evidence exists, modify the production implementation;
4. execute the targeted test and all applicable regression gates until the
   required suite passes (**green**);
5. record the red and green evidence, commands, changed files, and results in
   that V-Bounce's MEM. The manifest records the V-Bounce execution outcome
   and MEM reference — and, for a defect that reproduces only in integration
   tests, the integration test runs that carry its red and its green.

Creating the reproduction test and implementing the correction are not two
Bolts and not two V-Bounces: they are mandatory phases of the same BUG Bolt,
the same approved SPEC, and the same V-Bounce. Autonomous retries remain
inside that V-Bounce. If the agent cannot produce a valid red test, it must not
change production code; it stops, creates the mandatory MEM and manifest entry
with the blocker, and pauses at `AITL-MEM-Approval`.

A defect that reproduces only in integration tests — only against real
dependencies, so the hermetic suite cannot be relied on to show its red —
keeps this order with its red and its green outside the V-Bounce: its red is
a failed integration test run recorded **before** the V-Bounce, and its green
is a passing covering run in the Bolt's integration test cycle. The V-Bounce
writes the integration test that reproduces the defect, runs the best
hermetic reproduction before any production change, and ends with the
hermetic suite green. The red still precedes every production change. Its
Bolt is the integration-test-only BUG Bolt (§3.2.2), never waived; without
such a run the stop above applies, and a hermetic red alone does not replace
it (§3.3.1).

```mermaid
flowchart TD
    B["Draft BUG"] --> HB{"AITL-BUG-Approval"}
    HB -->|"Changes requested"| B
    HB -->|"Approved"| C{"Violated expectation?"}
    C -->|"Functional behavior"| F["Functional Bolt under feature US"]
    C -->|"Technical constraint"| N["Non-functional Bolt under US-000"]
    F --> S["Approved BUG SPEC"]
    N --> S
    S --> Q{"Where does the defect reproduce?"}
    Q -->|"In the V-Bounce"| R["Create + run reproduction test: RED"]
    Q -->|"Only in integration tests,<br>a red run recorded"| RI["Failed integration test run,<br>recorded before the V-Bounce: RED"]
    Q -->|"Nowhere — or only in integration tests<br>with no red run recorded"| X["Stop: MEM + manifest with the blocker,<br>no production change"]
    RI --> HR["Write the integration test that reproduces it;<br>run the best hermetic reproduction"]
    R --> I["Modify implementation"]
    HR --> I
    I --> G["Run target + regression tests: GREEN"]
    G --> M["MEM + manifest → AITL-MEM-Approval"]
    M -.->|"Only in integration tests"| IG["Integration test cycle:<br>passing covering run: GREEN"]
```

## 2.17 Key metrics (DORA-first — teaser)

Avenga DevFlow uses **three concentric layers** of metrics. The full
definition, targets, slicing rules and decision logic live in **§3.7**.

- **A. DORA software-delivery metrics (primary, mandatory)** — delivery
  performance using the current five-metric model.
- **B. AI-native flow metrics (secondary)** — methodology health.
- **C. AITL governance metrics (mandatory)** — proof that human-by-default
  governance is real, not nominal.

See §3.7 for the full tables and decision rules.

## 2.18 Agent run

**What it is:** an **agent run** is a session launched by a human in which
the Coordinator delegates the work of that human's mandate to the project's
DevFlow Agents — the live agents of `agents/squad/` (§3.0.1, §5.2). It is
always launched by a human, never by an agent, and it authorizes nothing
beyond that human's work mandate (G40).

**Its record:** every agent run leaves exactly one record in the
`agent-runs/` family, `ARUN-YYMMDD-HHmm-<description>.md` (§5.12, §5.15),
and carries a sequence number (`run: N`) claimed in the family's
`INDEX.md`. The record is an **operational narrative**, the run's own
chronology: it has no AITL checkpoint, no manifest and no `status` (§3.15);
it is not a derivative document (§5.5); and it is never governed input —
never the source or justification of a SPEC, Bolt, ADR, US, TC, BUG, MEM, or
any AITL checkpoint (G28). Where a run record and a governed artifact
disagree, the artifact wins.

**Not an integration test run, not a `runs[]` entry:** an agent run is a
session of agents; the integration test runs of a Bolt (§3.2.2) and the
`runs[]` entries of a manifest's `generation` block (§3.12) share only the
word.

---

# 3. Operating principles and rules

This section defines the **Avenga DevFlow framework**: how we think and how
we act. It can be used as a "working contract" for the team.

## 3.0 Actor-in-the-Loop Charter (AITL)

**Actor-in-the-Loop (AITL)** is the **load-bearing principle** of Avenga DevFlow.
The AI agent is the generator; the **actor** at each checkpoint is the governor —
a **human by default**, and a virtual DevFlow Agent only by explicit, valid
configuration. **Human-in-the-Loop (HITL) is the default case
of AITL** (actor = human), not a separate paradigm: with no or invalid
configuration every checkpoint is a human approval and **no AI-signed approval is
possible** (the safe-default invariant). Speed is never traded for the loss of a
checkpoint.

**Role routing is guidance, never a gate (single-operator operability).** The
owner named for each checkpoint below is the *recommended* approver — it
records who *should* review, not a precondition that blocks. When the named
role has no holder in the team, the checkpoint is satisfied by the qualified
human(s) actually present: they record the approval, noting the self-assigned
role (e.g. "approved as QA: <user>"). **One person may hold several roles at
once**, and role assignment is living data, not a decision requiring approval
(a team roster may later resolve holders, including external reviewers). Every
AITL checkpoint is therefore satisfiable by a single-operator team. The **only
exceptions** — hard requirements because they concern a *different person or
model*, not a *different role* — are the identity-separation rules: the handoff
incoming-executor rule (§3.3), Judge-model neutrality (G37), and the
prohibition on the AI self-approving or fabricating a reviewer (G18, G24).

**Core mandatory human checkpoints (none are skippable when applicable):**

**Canonical naming rule:** Every human checkpoint uses
`AITL-<CANONICAL-ARTIFACT-OR-PHASE-CODE>-Approval`. The artifact and phase
codes are always uppercase (`US`, `BUG`, `TC`, `BOLT-READY`, `BOLT-DONE`, `ADR`, `SPEC`, `MEM`,
`DISC`, `REV`, `AREV-CRITIQUE`, `AREV-DEFENSE`, `AREV-VERDICT`). Legacy numbered
aliases and mixed-case artifact identifiers are invalid.

| Identifier | Checkpoint | Owner | Decision | Human by default |
|------------|------------|-------|----------|----------------------------|
| `AITL-US-Approval` | **Feature User Story approval** | Functional Analyst (or, if the named role has no holder, the available qualified human records it, noting the self-assigned role) | Does the feature US, including its ACs and source traceability, accurately represent the business need and the current analysis? This checkpoint does not apply to US-000. | ✅ |
| `AITL-BUG-Approval` | **Confirmed BUG approval** | Functional Analyst for functional BUGs; for non-functional BUGs, Architect or Tech Lead recommended when `severity: critical`, otherwise any team member — guidance, never a gate: any qualified team member, the BUG's own author included, may record it at any severity | Is this a real, sufficiently evidenced defect with clear expected and actual behavior, correctly classified and routed to the proper future Bolt parent? Its approval gates the dedicated Bolt's `AITL-BOLT-READY-Approval`. | ✅ |
| `AITL-TC-Approval` | **Test Case approval** | QA plus Functional Analyst/domain owner for functional expectations; QA plus applicable technical owner for non-functional expectations (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Is this TC independently derived from approved intent, complete, executable, free from implementation-derived expected-result bias, and ready to govern verification or QA Automation? | ✅ |
| `AITL-BOLT-READY-Approval` | **Bolt readiness** | Functional Analyst for functional Bolts; Architect or Tech Lead for non-functional Bolts (except: the dedicated Bolt of a non-functional BUG mirrors its parent BUG's severity routing — see §2.16); QA Lead, QA Automation Lead, Architect, or Tech Lead for Test Bolts (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Does this Bolt clearly define a valid, appropriately sliced outcome under the correct parent—an approved feature US, US-000, or one approved TC—without specifying the implementation? | ✅ |
| `AITL-ADR-Approval` | **Architecture Decision Record approval** | Architect/Tech Lead (or, if the named role has no holder, the available qualified human records it, noting the self-assigned role) | Is this ADR complete, technically sound, and accepted as a governing decision? | ✅ |
| `AITL-SPEC-Approval` | **Implementation SPEC approval** | Dev-validator + applicable domain owner(s) — minimum one approver (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Is this one-Bolt implementation plan complete, grounded in approved governed artifacts and the actual repository, feasible, testable, and safe to execute? | ✅ |
| `AITL-MEM-Approval` | **MEM and V-Bounce approval** | Dev-validator who executed the Bolt; the incoming executor after a recorded handoff (one approver, any risk; additional QA/Sec/domain reviewers optional) | Does the MEM faithfully account for the complete V-Bounce, and does direct inspection of the diff, tests, gates, and evidence show that the output is correct, safe, and aligned with the SPEC and ADRs? Approval of the latest V-Bounce is the **Development Completed** event: the Bolt becomes `in-integration-testing` (`test_integration: required`, plan `applicable`) or, when no integration test run is pending (`test_integration: not-required`, or a `waived` plan), `ready-for-acceptance` (§3.2.3). | ✅ |
| `AITL-BOLT-DONE-Approval` | **Bolt acceptance** | PO/PM for functional Bolts; routed technical owner for non-functional Bolts; QA Lead or QA Automation Lead for Test Bolts (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Does the approved V-Bounce output satisfy the Bolt's completion evidence, with its integration test evidence — a passing covering run, the `not-required` reason, the `waived` ADR, or a late waiver with its reason (§3.2.2)? | ✅ |

**Need-driven research and optional review mechanisms with mandatory governance
once triggered:**

DISC, REV and AREV are **not mandatory stages of every E2E flow**. Any
stakeholder or team member may initiate them when their purpose applies. Once
initiated, their named AITL checkpoints cannot be skipped.

| Identifier | Checkpoint | Owner | Decision | Human by default |
|------------|------------|-------|----------|----------------------------|
| `AITL-DISC-Approval` | **Discovery approval** | Qualified human designated for the research domain — **the author included** | Is the investigation sufficiently evidenced, explicit about limits, and reliable enough to guide backlog or architecture decisions? | ✅ |
| `AITL-REV-Approval` | **Review approval** | Qualified human designated for the Review — **the author included** | Are the Review and its findings evidence-based, clear, correctly classified, and actionable? | ✅ |
| `AITL-AREV-CRITIQUE-Approval` | **AREV Critique approval** | Qualified human designated for the AREV | Is the Critique sufficiently rigorous, relevant, and supported to advance? | ✅ |
| `AITL-AREV-DEFENSE-Approval` | **AREV Defense approval** | Qualified human designated for the AREV | Does the Defense address the approved Critique completely and with evidence? | ✅ |
| `AITL-AREV-VERDICT-Approval` | **AREV Verdict approval** | Qualified human designated for the AREV | Does the Verdict fairly adjudicate the approved arguments and produce actionable findings? | ✅ |

> **Human by default (§3.0):** a virtual DevFlow Agent may
> occupy a checkpoint only by explicit, valid configuration with independence;
> `critical` and `regulatory` stay human (§3.3 ceiling). With no or invalid
> configuration every checkpoint is human-only and no AI-signed approval is
> possible (the safe default).

`AITL-US-Approval` applies once to each feature User Story version that
materially changes its business intent or Acceptance Criteria. It **never
applies to US-000**, which is a permanent, non-approvable traceability
container rather than a business User Story. `AITL-BOLT-READY-Approval` applies to
**every Bolt**, with approval routed according to its type: functional to a
Functional Analyst, non-functional to an Architect or Tech Lead, and Test to a
QA Lead, QA Automation Lead, Architect, or Tech Lead. No Bolt
approval is inherited merely because a related artifact was approved.

`AITL-BUG-Approval` applies to every BUG and gates its dedicated Bolt's
readiness. It confirms the defect, evidence, nature, and routing. It never authorizes code
and never substitutes for `AITL-BOLT-READY-Approval`. A functional BUG is routed to
a Functional Analyst; a non-functional BUG is routed to an Architect or Tech
Lead when `severity: critical`, otherwise to any team member, the BUG's own
author included. The recommended approver is guidance, never a gate: any
qualified team member — the BUG's own author included — may record it at any
severity, noting the self-assigned role where the recommended holder is
unavailable (the operability principle — role routing never blocks). Developers
and QA may create either kind as a draft.

`AITL-TC-Approval` applies to every Test Case before it is used as a governed
verification contract or parent of a Test Bolt. Approval is independent from
the source US and Bolt approvals: those approve intent and work boundaries;
the TC checkpoint approves the independent verification design. A TC may not
inherit approval from current code, existing tests, its US, or its source
Bolt.

`AITL-ADR-Approval` applies separately to **every ADR**. The decision must be
recorded on the ADR with the approver, role, timestamp, outcome, and
review-quality evidence. Approval is never inherited from a related US, Bolt,
SPEC, V-Bounce, Review, or Adversarial Review.

`AITL-SPEC-Approval` applies to the one canonical SPEC for every Bolt and to
every material revision of that SPEC. It is independent from
`AITL-BOLT-READY-Approval`: Bolt approval includes its DoR and authorizes SPEC
generation, while SPEC approval authorizes technical execution. A SPEC
approval is never inherited from approval of its Bolt, US, ADR, Review,
Discovery or AREV.

`AITL-MEM-Approval` applies separately to **every V-Bounce and its unique
MEM**. The agent must generate the MEM and update the manifest before the
checkpoint. The reviewer approves or requests changes only after inspecting
the MEM against the actual implementation and verification evidence. An
approved `AITL-MEM-Approval` approves and completes that V-Bounce; no other
checkpoint implies it. A `changes_requested` decision preserves the MEM and
matching `v_bounces[]` entry as an unapproved attempt and requires a new V-Bounce for
additional autonomous execution.

The Bolt's `status` is declared in its document and moves by the transitions
of §3.2.3. While its latest V-Bounce awaits review it is `in-development`
(`blocked` when that V-Bounce stopped `blocked`); `changes_requested` or
`rejected` keeps it `in-development`; `approved` is the **Development
Completed** event, after which the Bolt becomes `in-integration-testing`
(`test_integration: required`, plan `applicable`) or, when no integration test
run is pending (`test_integration: not-required`, or a `waived` plan),
`ready-for-acceptance` (§3.2.3). `AITL-MEM-Approval` never substitutes for
`AITL-BOLT-DONE-Approval`, which alone makes the Bolt `done`.

`AITL-DISC-Approval` applies to every initiated DISC. An approved
Discovery becomes governed research input, but it does not approve any User
Story, Bolt, ADR, SPEC, or implementation derived from it.

`AITL-REV-Approval` applies to every initiated REV. The three AREV
approvals apply sequentially to every initiated AREV. Approval of a REV or an
AREV Verdict makes its findings usable as governed inputs; it never approves
the downstream artifacts created or changed from those findings.

A material change to an approved feature US invalidates its previous US
approval and requires the Functional Analyst to reassess every derived
functional Bolt. Each affected Bolt must obtain a new
`AITL-BOLT-READY-Approval` before it can advance again. This rule does not apply to
US-000 because US-000 has no approval lifecycle.

A material change to an approved TC invalidates its previous
`AITL-TC-Approval`. Every active dependent Test Bolt and canonical SPEC must
undergo impact assessment and re-approval before execution continues. A
completed automation implementation is updated through a new Test Bolt rather
than rewriting its historical execution evidence.

A material change to an approved Bolt invalidates its previous
`AITL-BOLT-READY-Approval` and the approval of its canonical SPEC. The Bolt returns
to its role-appropriate owner, and no new V-Bounce may begin until both the
Bolt and the materially revised SPEC are approved again. Historical MEMs and
manifest entries remain immutable.

A material change to an approved BUG invalidates its previous
`AITL-BUG-Approval` and pauses its dedicated Bolt. The BUG must be re-approved,
then the Bolt and its canonical SPEC must undergo impact assessment and
re-approval before execution resumes. Historical red→green evidence is never
rewritten.

**AITL operating rules:**

- Work in `input/` and `analysis/`, including the generation and refinement of
  draft feature User Stories and functional Bolts, may be iterative
  and AI-assisted. Technical contributors may likewise iteratively define
  non-functional Bolts from governed technical evidence. These
  work-definition iterations are **not V-Bounces**.
- QA may iteratively draft and refine TCs from approved intent   and Test
  Bolts from TCs. Neither drafting loop is a V-Bounce, and a Test Bolt's
  `AITL-BOLT-READY-Approval` is refused until its parent TC has `AITL-TC-Approval`.
- A Discovery remains draft until `AITL-DISC-Approval`. Its unapproved
  conclusions may not govern analysis, backlog, architecture, or
  implementation decisions.
- Discoveries investigate pre-definition unknowns; Reviews inspect existing
  project artifacts or characteristics. Neither approval substitutes for the
  other or for any downstream artifact approval.
- If the Functional Analyst requests changes at `AITL-US-Approval`, the US
  returns to the analysis and drafting loop,   where its Bolts are
  refined alongside it. No Bolt from that draft may be approved
  until the US is; a `draft` Bolt dropped by that refinement is marked
  `deprecated` and its ID is never reused (§2.4).
- If the applicable approver requests changes at `AITL-BOLT-READY-Approval`, that
  specific Bolt returns to its owner for refinement. Other Bolts require their
  own decisions and are unaffected unless a feature US changes materially; in
  that case, every affected functional Bolt returns to approval.
- A BUG may be drafted by a Functional Analyst, Developer, or QA; its one
  dedicated Bolt may be drafted and refined alongside it, but
  `AITL-BOLT-READY-Approval` is refused until `AITL-BUG-Approval` is recorded by the
  role-appropriate approver. A BUG approval cannot be inherited from its US,
  ADR, Review, or incident.
- No SPEC may be prepared for execution, and no V-Bounce may begin, unless the
  specific Bolt has recorded `AITL-BOLT-READY-Approval`. A functional Bolt also
  requires `AITL-US-Approval` on its feature parent. A non-functional Bolt
  requires no approval on US-000. A Test Bolt requires
  `AITL-TC-Approval` on its exact parent TC.
- Before generating a SPEC, the agent verifies every governed source it will
  use. If any applicable BUG, TC, Bolt, feature US, ADR, DISC, REV or AREV phase is not
  approved, the agent emits a blocking report instead of a SPEC. After a SPEC
  is generated, no code-run, code generation, test generation, configuration
  change or V-Bounce may begin until `AITL-SPEC-Approval` is recorded.
- Every ADR used by a SPEC, V-Bounce, gate waiver, or architecture policy
  must have a recorded `AITL-ADR-Approval`. If an unresolved architectural
  decision emerges during SPEC preparation or a V-Bounce, generation pauses;
  the ADR must be approved and the canonical SPEC revised and re-approved
  before technical execution resumes.
- DISC is need-driven and REV and AREV are stakeholder-triggered and optional
  in the E2E flow. A DISC cannot reach approved status without
  `AITL-DISC-Approval`. A REV cannot reach approved status without
  `AITL-REV-Approval`. An AREV cannot
  advance from Critique to Defense, from Defense to Verdict, or expose usable
  findings without the corresponding phase approval.
- Findings from an approved REV or AREV Verdict may create or update any
  DevFlow artifact, but every affected artifact retains its own lifecycle and
  AITL approval. A code-related outcome still requires an approved Bolt.
- A Bolt cannot move from any checkpoint to the next **without a named human
  reviewer, review timestamps and review-quality evidence**. Every approvable
  artifact uses this minimum review contract in its own machine-readable
  metadata (YAML or an equivalent structured block):

  ```yaml
  review_ready_at: 2026-08-02T11:45:00-03:00
  review:
    decision: approved
    reviewers:
      - actor: human:eugenio.serrano   # human:<user> | agent:<id>
        role: dev_validator
        model: null                     # null for a human; the model id for an agent
    written_by:                         # who wrote this record; writing is not signing
      - actor: human:eugenio.serrano
        role: dev_validator
        model: null
    started_at: 2026-08-02T11:55:00-03:00
    decided_at: 2026-08-02T12:10:00-03:00
    findings: []
    acknowledged_without_comment: true
    acknowledgment_reason: "Evidence inspected; no findings identified."
  ```

  `review_ready_at` is when that exact artifact version is formally submitted
  and made available for human review; it is independent from whether the
  artifact was produced by an AI agent or a person. It is a sibling key of
  `review`, not a field inside it, so validators can distinguish submission
  from review without ambiguity. `started_at` is when direct
  human inspection begins; `decided_at` is when the decision is recorded.
  `findings` contains the review findings or
  comments raised. When it is empty, `acknowledged_without_comment` must be
  `true` and `acknowledgment_reason` must explain the evidence inspected.

  **Canonical identity — the actor grammar (§3.0):** every recorded identity
  is an **actor** in one of two namespaces: **`human:<user>`**, where `<user>`
  is the **local part of the person's `git config user.email`**
  (e.g. `eugenio.serrano@avenga.com` → `human:eugenio.serrano`), or
  **`agent:<id>`**, the stable kebab-case id of a DevFlow Agent (resolved
  against the roster / agent definition). It carries no spaces, accents or
  display formatting and is compared verbatim.
  - **Machine records and review/enforcement fields are prefix-mandatory**
    (`^(human|agent):.+`): `review.reviewers[].actor`,
    `acceptance_review.reviewers[].actor`, `risk_history[].decided_by[].actor`,
    `review.written_by[].actor`, `acceptance_review.written_by[].actor`, and
    the manifest's `created_by`, `decided_by[].actor` and `written_by[].actor`.
  - **Three attribution fields in every document — `author`, `agent`, `llm`.**
    Each answers one question, and none is ever used for another:
    - **`author:` is the accountable human** — who mandated the work and
      answers for it: always `human:<user>` (a bare `<user>` is accepted as
      shorthand). Never an agent, a model, a tool or a product name.
    - **`agent:` is the producing agent** — `agent:<id>` of the DevFlow Agent
      that actually produced the document: the role agent when one was
      spawned, the Coordinator only for content it produced itself (§3.0.1),
      or `null` when a human wrote it without an agent. Never the delegating
      agent, never bare, never an improvised model, tool or product name.
    - **`llm:` is the model** that agent ran — the exact identifier the tool
      reports (e.g. `claude-opus-5-5`), or `null` when no AI was involved.
    - **A later edit by another agent updates `agent:` and `llm:`** to the
      producer of the current version; earlier contributions stay in the
      revision history and, for artifacts with a manifest, in `runs[]`.
      `author:` changes only when another human mandated the revision.
  - **`written_by` — who wrote the record of a decision — is a fourth
    question, with its own field.** It is neither who mandated the work
    (`author:`), who produced the artifact (`agent:`, `llm:`) nor who signed
    the decision (`reviewers`): it is the actor that actually wrote the
    record — the reviewer writing their own decision, or the agent that
    transcribed a decision dictated to it or returned to it, such as the
    Coordinator (§3.0.1). It is an optional list of the `reviewers` shape, a
    sibling of `reviewers` in `review:` and `acceptance_review:`, **never
    inside `reviewers`**: a human is `human:<user>` with `model: null`, an
    agent `agent:<id>` with its exact model id, and `role` is the writer's own
    role (`coordinator` for the Coordinator). **Writing is not signing:** a
    writer signs only by also being in `reviewers`; independence,
    self-approval and quorum read `reviewers` and `decided_by[]` only, and
    writing a record confers no approval. Absent or `[]` means the writer was
    not recorded — what every earlier record states; no record is backfilled
    and a writer is never inferred. A decision recorded on or after the
    repository's adoption of the version that carries the field names its
    writer (§3.12).
  - **Other descriptive frontmatter person fields take a bare shorthand:**
    `owner:`, `validator:`, `closed_by:`, `facilitator:` accept a bare
    `<user>` meaning `human:<user>`; an agent is **always** written
    `agent:<id>`, so a bare value is never ambiguous.
  - **Normalization:** wherever identities are compared or projected, a bare
    value compares equal to its `human:`-prefixed form — so self-approval
    routing (G29) and the review↔manifest projection and its mismatch detection
    (G18, G24) work verbatim as actor comparisons.

  `git config user.name` remains the human-readable label in prose; it is
  never the identity field.

  Each governed artifact carries the authoritative and complete approval
  evidence. For lifecycle checkpoints represented in the manifest family, the
  manifest entry is the minimal lifecycle projection of that same decision, and
  because both sides now use the actor grammar the projection is a
  **field-for-field copy**:
  `review.reviewers[]` (`{actor, role, model}`) maps directly to
  `checkpoint_approvals[].decided_by[]`, `review.written_by[]` to
  `checkpoint_approvals[].written_by[]` (absent stays absent; a mismatch is
  detected exactly as a `reviewers` one), `review.decision` to
  `checkpoint_approvals[].decision`, and `review.decided_at` to
  `checkpoint_approvals[].decided_at`; `review.findings` may
  be summarized in the
  optional manifest `comment`. (Whether a decision is virtual is **derived**
  from an `agent:` actor prefix, not stored — there is no `mode` field, §3.12.) `review_ready_at` and `review.started_at` are
  **copied** to the manifest as `review_ready_at` / `review_started_at`
  (§3.12 timing contract); the complete findings and acknowledgment fields
  are not. A Bolt's **acceptance review** (`AITL-BOLT-DONE-Approval`) uses
  the same contract on the same artifact — `acceptance_review_ready_at` and
  `acceptance_review:` — and projects to `bolt.acceptance.review_ready_at` /
  `bolt.acceptance.review_started_at`, so the readiness and acceptance
  review timings coexist without overwriting each other.
  `acceptance_review_ready_at` is set in the same act as the Bolt's transition
  into `ready-for-acceptance` — never at an earlier status — so the acceptance
  review budget measures the acceptance wait and the integration testing wait is
  measured separately (§3.7.2). In the joint act of §3.2.2 the three
  acceptance times coincide.
  A mismatch between the authoritative artifact evidence and its manifest
  projection is a validation error; it does not transfer authority to the
  manifest. Workflow telemetry may index review timestamps for metrics. AREV
  approvals and verdicts remain exclusively in the AREV artifacts.
- The human reviewer reads the **diff and the test evidence**, not only the
  agent's summary. "AI says it's fine" is not approval.
- `AITL-MEM-Approval` is recorded by the **Dev-validator who executed the
  Bolt**, after inspecting the actual diff, test/gate evidence, MEM and
  manifest. "Executed the Bolt" means the human who **operated the V-Bounce
  session** for that iteration — the one who launched the agent, steered its
  inputs and supervised the run; this is normally the Bolt's assigned owner
  in the weekly plan (§4.3). For a **Test Bolt**, the executing
  Dev-validator is the **QA / QA Automation Engineer** who operated the
  V-Bounce — "Dev-validator" denotes the technical validator of
  the output, not a specific department. The MEM is approved by the executing
  Dev-validator alone (one approver, any risk); after a recorded handoff, the
  incoming executor. QA, Security or domain reviewers may be added at the
  team's discretion but are never required by count. The executing
  Dev-validator is never the AI agent, and the agent never self-approves.
- Every V-Bounce inevitably produces exactly one MEM. The agent may not omit,
  defer, reuse, or overwrite it. `AITL-MEM-Approval` is prohibited until the
  MEM and manifest entry are complete, and merge/promotion is prohibited until
  this checkpoint is approved.
- If the agent loops more than the configured turn budget without a green
  hermetic suite, it must **stop and ask a human**, not push through. The human
  may patch the code manually; this is logged in the MEM (not hidden, not
  punished — measured).
- **Turn budget (definition):** the maximum number of autonomous agent
  iterations — measured in agent loops without a green hermetic suite —
  before the agent must stop and ask a human instead of pushing through. The
  default is configured in the agent definition installed for the team's
  tool (§5.2; e.g., 10 loops without a green hermetic suite); a SPEC may override it per Bolt via the optional `turn_budget`
  frontmatter field (an integer ≥ 1). It is not measured in tokens or wall
  clock: only failed-or-non-green iterations count. Exhausting it does not
  end or fail the V-Bounce; it triggers the mandatory stop-and-ask, and the
  MEM records the blocker and the current evidence (§0, §2.12, §3.3).
  It counts the V-Bounce's own loop only; the Bolt's integration test cycle
  consumes none (§3.2.2).
- Adversarial Review (when stakeholder-triggered) is a **pre-filter for later
  human decisions**, never a replacement for them.
- **Review-time budget for technical and delivery checkpoints** (recommended;
  tune per team):

  | Risk class | SPEC approval | MEM / V-Bounce approval | Bolt acceptance |
  |-----------|---------------|-------------|---------|
  | `low`      | ~5 min        | ~15 min     | ~5 min  |
  | `medium`   | ~10           | ~30         | ~10     |
  | `high`     | ~15           | ~60         | ~15     |
  | `critical` | ~30           | ~90         | ~30     |

  This table applies only to SPEC, MEM/V-Bounce and Bolt acceptance.
  Budgets for US, BUG, TC, ADR, DISC, REV and AREV reviews are defined by
  each project or team because their scope varies materially. These are
  recommendations, not mandatory minimums. Each reviewer is
  responsible for the quality of their sign-off. Review duration, when
  measured, is derived from the manifest timing contract
  (`decided_at` − `review_started_at`, §3.12) or, where a step timestamp is
  missing, from workflow telemetry.

- **Human-review escalation (Time-to-Human-Review consequence):** the target
  for starting a review is **< 4 h of working time** from `review_ready_at`
  (§3.7.3). Exceeding it has defined consequences — the checkpoint itself is
  never skipped, delegated or auto-approved:

  | Elapsed working time since `review_ready_at` | Action |
  |-------------|--------|
  | ≥ 4 h | The agent reminds the assigned reviewer (visibility); the pending review is recorded as a process defect for the next retro (§3.0, §3.7.3) |
  | ≥ 8 h | The agent escalates to the artifact owner / applicable lead, who either reviews it or reassigns the reviewer |
  | ≥ 24 h | The agent escalates to the PO / Tech Lead of the project, who resolves the assignment (review, reassign, or formally deprioritize with a reason) |

  These thresholds are per-artifact pending reviews, tuned per team like the
  budgets above. Escalation never replaces the reviewer's decision: the human
  who signs is still responsible for review quality, and an approval without
  evidence review remains a violation. Every escalated review is measured and
  reviewed in the retro; persistent lateness (> 24 h repeatedly) is addressed
  as a process defect, not by weakening the checkpoint.

- **AITL Coverage targets by Bolt type and applicable conditions** (§3.7.3):

  Every Bolt requires its own role-routed `AITL-BOLT-READY-Approval`, its canonical
  SPEC approved through `AITL-SPEC-Approval`, and
  `AITL-ADR-Approval` for every applicable ADR. A functional Bolt additionally
  requires traceable `AITL-US-Approval` for its feature parent. A
  non-functional Bolt is traced to US-000, which has no approval lifecycle. If
  the Bolt is a Test Bolt, it additionally requires `AITL-TC-Approval` for its
  exact parent TC. If
  the Bolt corrects a BUG, that BUG additionally requires
  `AITL-BUG-Approval` before the Bolt was created. If
  a DISC, REV or AREV is used by the Bolt or SPEC, all conditional approvals
  for that initiated mechanism are also required. The table below lists the additional
  execution and delivery checkpoints.

  | Bolt type | Required AITL checkpoints | Conditional additions | Coverage target |
  |-----------|---------------------------|-----------------------|-----------------|
  | `functional` | AITL-US-Approval + AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | AITL-BUG-Approval when BUG-driven | **100%** |
  | `non-functional` | AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | AITL-BUG-Approval when BUG-driven | **100%** |
  | `test` | AITL-TC-Approval + AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | — | **100%** |

  **Per-Bolt coverage is the whole coverage story in this release.** Release-level
  grouping, promotion and customer acceptance are the adopting team's own process
  (§4.6); DevFlow does not prescribe Unit/UAT approval checkpoints in this release
  (a redesigned model is planned for a future version). A `done` Bolt already reports **100%**
  coverage on its own checkpoints.

  `incident_hotfix`, `regulatory`, `feature_value` and `debt_hardening` are
  service classes used for prioritization (§3.8), not Bolt types. Likewise,
  `feature`, `refactor`, `infra`, `hardening`, `debt` and `qa_automation` are
  work categories used for reporting and approval routing (§3.11). A
  BUG-driven or hotfix Bolt remains one of the three canonical Bolt types.

  Anything below is a process defect logged in the next retro.

**What AITL is NOT:**

- Not micromanagement: the human may orient the SPEC and prompts and may
  correct course between executions, but does not micromanage the agent's
  autonomous loop. Manual code patches are an allowed, explicitly recorded
  fallback rather than the normal authorship mode.
- Not a rubber stamp: an approval without evidence review is a violation.
- Not optional under deadline pressure: hotfixes still require the applicable
  feature US approval (or US-000 routing), Bolt approval, SPEC approval and
  the same V-Bounce/MEM/acceptance path; checkpoints are never silently
  skipped.

### 3.0.1 The Actor

An **Actor** is a **member of the team** with two responsibilities: it
**produces** the governed artifacts its role owns — functional analyst →
US, architect → ADR, developer → SPEC + code, QA → TC/tests — in
**executor** mode; and it **participates** in AITL approvals in
**approver** mode when configured, under the independence floor. By
default an Actor is a **human**; a **virtual DevFlow Agent** participates
only by explicit, valid project configuration. **HITL is the default case
inside AITL** (actor = human): with no agents configured every checkpoint
is a human approval and **no AI-signed approval is possible** (the
safe-default invariant). An Actor's relationship to a checkpoint is
**executor**, **approver** or **neither** — e.g. the Coordinator routes
and records but never signs. The Actor is not merely a checkpoint
participant: **production is first-class** — the AI generates, the human
governs at every checkpoint.

**The Coordinator's actor id.** The Coordinator — the MainAgent — records
itself as **`agent:avenga-devflow`**, one id in all four tools (the tool is
recorded in `runs[].tool`, never in the id). It is the only agent id the
methodology fixes; role agents use their roster id. When the Coordinator
spawns a role agent to produce a document, that document records the role
agent's id, not the Coordinator's (§3.0). **At any depth** of delegation the
record names the agent that actually produced: a document's `agent:` and a
run's `agent` carry the producing agent's own id, never the delegating or
spawning agent's. When the Coordinator records a decision — a human's
dictated decision, or the one an approver agent returns — it records itself
as that record's writer (`written_by`, role `coordinator`), never as a
signer (§3.0).

**Two mandates.** What an actor *may* produce is its **productive mandate**
— derived from its role (the `produces` mapping above). What it produces
*now* is bounded by the human's **work mandate** — the explicit request or
delivery mandate it works under (§3.3, G40). Right artifact, right
authorization: an actor never widens either on its own — it never enables
its own approval, and it never extends its own mandate.

**Identity and grammar.** Identity belongs to the **actor**, never to the
model: a human actor is recorded `human:<user>`, a DevFlow Agent actor
`agent:<id>`; the model is an **attribute** of the agent actor (`model: null`
for humans).

**Independence layers.** Approval independence is measured first on the
actor: `approver.id ≠ executor.id` — the **actor floor**, generalizing the
human handoff rule (§3.3). At `high` risk it is hardened at the model level:
`approver.model ≠ executor.model`. At `critical`/`regulatory` the ceiling is
**human-only**, regardless of roster contents.

**Roles are open.** The methodology does not freeze a role enum; the kit
names recommended archetypes as examples — coordinator, functional-analyst,
architect, developer, qa, reviewer, project-defined… Independence is always
measured on the actor `id`, never on the role taxonomy.

**Safe default.** With no virtual agents configured — no roster entry, or
no schema-valid approver entry — every checkpoint resolves to a human
actor (zero-config = pure HITL). Enabling virtual approvers is always a
**human configuration act — a schema-valid roster entry granting the
checkpoint class** (`modes: [approver]` + a non-empty `approves`, listed
in the team's `roster.yaml`, `devflow/actors/`) — never a silent flag,
never the agent's own act.

```mermaid
flowchart TB
    R[("Roster: actors + roles + models<br/>humans + DevFlow Agents")]
    R --> A["Actor — carries a role<br/>FA · architect · developer · qa · reviewer · …"]
    A -->|"executor mode"| P["Produces the artifact its role owns<br/>US · ADR · SPEC · code · tests"]
    P --> K{"AITL checkpoint pause"}
    K -->|"approver mode · different actor<br/>approver.id ≠ executor.id"| D["Approve / request changes"]
    K -.->|"no valid config → human by default<br/>zero-config = pure HITL"| D
    D --> M[("checkpoint_approvals[]<br/>actor + role + model")]
    D -.->|"independence layers"| I["actor floor · model hardening @ high<br/>human ceiling @ critical / regulatory"]
```

## 3.1 Principles (non-negotiable)

- **AI generates by default; humans govern.**
  AI agents generate the intended-final deliverables (docs / design / code /
  tests) and **run their own tests**; the team steers through governed inputs,
  reviews, validates and approves. Direct human code patches are a legitimate
  fallback when the agent is blocked or correction is safer; they must be
  recorded in the MEM and remain subject to the same gates and approvals.

- **Small, demonstrable work.**
  Everything is broken down into Bolts with a target of **1 hour to 1 working
  day of active delivery time**, with a clear, observable completion
  criterion. Approval waits are measured separately and do not consume this
  sizing target.

- **V-Bounce as the standard execution form.**
  Each Bolt is executed through one or more cycles of: *AITL-approved SPEC → AI
  agent generates the intended-final change, runs the hermetic tests, and
  iterates autonomously → mandatory MEM
  + manifest update → `AITL-MEM-Approval`*. Approval completes the V-Bounce. A
  request for changes preserves it as an unapproved cycle; the next agent
  execution is a new V-Bounce with a new MEM.

- **Design and testing first.**
  Test Cases are derived and approved from intended behavior before the
  implementation plan is approved. Design (Domain / Logical) and
  **auto-generated tests** are born with the functionality, not afterwards —
  the hermetic ones executed in the V-Bounce, the integration tests in the
  Bolt's integration test cycle when its `test_integration` is `required`.

- **End-to-end traceability.**
  From **raw inputs or governed sources** → analysis → approved feature US/AC
  or permanent US-000 container → approved Bolt → approved Test Cases when
  applicable → approved applicable ADRs →
  canonical SPEC → `AITL-SPEC-Approval` → code → tests
  and their constraints → evidence. Everything linked.

- **Simple metrics, data-driven decisions.**
  We measure flow (Bolt Lead Time decomposed, weekly throughput,
  % commitment delivered) and first-pass quality (V-Bounces per Bolt, SPEC and
  V-Bounce first-review approval rates, spec drift). The normative
  definitions — and the limits on what may be derived from them — live in
  §3.7; nothing outside that section is a metric of this methodology.

- **Light cadence, focus on flow.**
  1 week for plan/demo; real work happens in **Bolts** and **V-Bounces**.

- **Automated quality.**
  A Bolt is not "Done" while any applicable **gate** (tests, security,
  licenses, perf-smoke) is `fail`. Applicable gates must be `pass` or
  explicitly `waived` through an approved ADR; non-applicable gates are `n/a`
  with a reason.

- **Knowledge is product.**
  Decisions and lessons learned are captured in every V-Bounce; secrets and
  sensitive payloads are never copied.

- **Attribution traceability:** Every AI-generated Markdown artifact records
  who answers for it, who produced it and with what model — `author`,
  `agent` and `llm` in its YAML frontmatter (the rule is in §3.0). Code and
  JSON do not use YAML frontmatter: their attribution lives in the manifest
  (`created_by`, `runs[].agent`, `runs[].model`, §3.12) and is described in
  the MEM. This supports human accountability and model-level analysis
  without duplicating telemetry across formats. The AREV phase templates
  carry `author` and `agent`; their model is recorded in `challenger_model` /
  `defender_model` / `judge_model` (§2.15, §3.13), with no separate `llm:`
  field.

- **Security and compliance by default.**
  Policies and *gates* are part of the flow, not an "extra".

## 3.2 Bolt rules (promise / measurement unit)

- **Size:** **1 hour to 1 working day** of active delivery time is the target.
  Several V-Bounces may belong to the same Bolt. If feedback or a later
  V-Bounce carries work into the next day, keep the same Bolt and record the
  elapsed time; do not split it retroactively. Split only when the scope
  contains independently deliverable outcomes that should be governed and
  accepted separately.

- **Creation responsibility:**
  - A functional Bolt is created and refined by a Functional Analyst.
  - A non-functional Bolt may be created and refined by a Developer,
    Architect, or Tech Lead without Functional Analyst participation or
    approval.
  - A Test Bolt is created and refined by QA or a QA Automation Engineer from
    one TC.
  - Other qualified specialists may provide governed inputs and evidence; the
    accountable creator and approval route are determined by the Bolt type.

- **Content:** A Bolt defines the requested outcome, scope, exclusions,
  dependencies, applicable governed sources, expected evidence, and an
  observable completion criterion. It defines **what work must be delivered**;
  it never becomes an ADR or an implementation SPEC.

- **Type:** Every Bolt is explicitly classified as `functional`,
  `non-functional`, or `test`:
  - A **functional Bolt** delivers a slice of behavior and belongs to the
    approved feature US whose Acceptance Criteria it covers.
  - A **non-functional Bolt** delivers a technical outcome and belongs only to
    `US-000-non-functional.md`, even when it affects or benefits a feature US.
  - A **Test Bolt** delivers QA Automation for exactly one TC, uses
    that TC as its direct parent, and is named
    `TC-NNN.BOLT-NNN-<description>.md`.
  - Functional/non-functional classification is independent of `layer`:
    backend, frontend,
    infrastructure, data, or full-stack work can participate in either nature.
    The Test type is determined by its QA Automation purpose.

- **Layer:** Each Bolt declares a `layer` in its frontmatter (`Backend`,
  `Frontend`, `Mobile`, `Data`, `Infra`, `QA-Automation`, `Full-stack`, or
  `Documentation`) to indicate the primary technology surface it touches.
  Project templates may extend this enum through normal methodology
  governance. Layer is orthogonal to the Bolt type.

- **DoR (Ready):**
  - Functional Bolt: correct feature parent approved through
    `AITL-US-Approval`.
  - Non-functional Bolt: assigned to the permanent US-000 container; no parent
    approval applies.
  - Test Bolt: assigned to exactly one TC approved through
    `AITL-TC-Approval`.
  - This specific Bolt approved through the role-appropriate
    `AITL-BOLT-READY-Approval`.
  - Functional Bolt: clear covered Acceptance Criteria and expected behavior.
  - Non-functional Bolt: clear technical outcome, originating evidence, and
    applicable governed sources.
  - Test Bolt: clear automation outcome, one source TC, test-only change
    boundary, supported execution context, and expected evidence.
  - Every relevant ADR approved through `AITL-ADR-Approval`.
  - Risks / controls identified.
  - `test_integration` and `test_integration_reason` declared — proposed by
    the drafting agent, confirmed or changed by this approver (§3.2.2).
  - Context available for the AI agent (links / artifacts).
  - **No unresolved analysis question:** no `OQ-NNN` in
    `analysis/open-questions/` whose `targets` include this Bolt's parent US
    or one of its governing artifacts is still `open` or `in-validation`.
    Each must be `answered` and propagated to its target, `deferred` with a
    revisit trigger, or `dropped` with a reason (§5.7, G35). An unanswered
    question about the parent is missing context by definition, which is why
    this belongs to the DoR rather than to a later checkpoint.
  - Demo criterion + estimation (active delivery time, composed per the
    AI-native estimation rule in §2.4), with approval wait measured
    separately.

- **DoD (Done):**
  - Applicable implementation checks and **agent-generated tests are executed
    and green** at their level (§3.6); gates are `pass` or approved `waived`,
    with justified `n/a` where a gate does not apply.
  - Human review **approved**: the latest V-Bounce's `AITL-MEM-Approval` is
    approved — the **Development Completed** event.
  - The Bolt is **`ready-for-acceptance`** (§3.2.2): when its
    `test_integration` is `required`, a passing integration test run covers
    its latest approved change, its plan is `waived` by the project's ADR
    (plus the executed compensating-control evidence on `high`/`critical`),
    or its pending integration testing was waived by a human with a written
    reason; when it is `not-required` (plan `n/a` with a reason), no run is
    pending.
  - Every ADR created or applied by the Bolt approved through
    `AITL-ADR-Approval`.
  - Evidence of **gates** (security, licenses, performance).
  - **Complete traceability** (story ↔ design ↔ code ↔ tests) and demo ready.
  - **`AITL-BOLT-DONE-Approval`** approved, which alone moves the Bolt from
    `ready-for-acceptance` to `done` (§3.2.3).

- **Weekly planning:** use **Commit (P85)** + **Stretch (P50)** with a
  **10–20% buffer** (operational procedure in §4.3).

- **WIP:** ideally **1 active Bolt per person / agent**. Avoid multitasking.
  A Bolt in its integration test cycle does not count against its executor's WIP; a
  failure that returns it to development comes back with priority (§3.2.2).
  The declared `in-development` (§3.2.3) shows the single active executor of a
  Bolt (G11).

- **File overlap between active Bolts (§2.4.1):** planning should avoid two
  active Bolts modifying the same file or component (the WIP limit and
  weekly planning are the primary prevention). If overlap happens anyway,
  there is **no priority ordering**: each Bolt's approved SPEC records the
  repository baseline it was generated against, and any material change to
  that baseline — including a merge from another Bolt — invalidates the
  SPEC approval (G15). The affected Bolt's executor must stop, revise and
  re-approve the SPEC before resuming. Conflicts are resolved by whoever
  merges second, following the normal re-approval path; no implicit
  ownership transfer is created.

- **No code without a Bolt:** An AI agent or human may not generate, propose,
  apply, or merge a code-related change unless an approved Bolt authorizes
  it. This includes source code, tests, configuration, infrastructure as code,
  database schemas and migrations, build scripts, and deployment definitions.
  Every implementation SPEC must reference exactly which approved Bolt
  authorizes the work. Urgency and size do not create an exception.

- **US-000 (all general-purpose non-functional Bolts):**
  `functional/user-stories/US-000-non-functional.md` is a permanent,
  always-active governance and traceability container. **Every Bolt whose
  primary outcome is non-functional must be assigned to US-000.** This
  includes infrastructure, refactoring, technical debt, hardening, security,
  performance, availability, observability, CI/CD, dependency upgrades,
  database maintenance, developer tooling, and similar technical changes.
  A relationship to one or more feature User Stories may be recorded as a
  dependency or related reference, but it does not change the Bolt's parent:
  a non-functional Bolt remains under US-000.

  US-000 is not a substitute for approved ADRs or automated quality gates.
  ADRs define the applicable architectural decisions and non-functional
  constraints; US-000 groups the Bolts that implement technical outcomes.
  US-000 is not approved, rejected, version-approved, or re-approved by any
  role. It exists only as the canonical parent and grouping mechanism. Every
  US-000 Bolt requires its own technical `AITL-BOLT-READY-Approval` by an Architect
  or Tech Lead and follows the same SPEC, V-Bounce, MEM, manifest, review, and
  metric rules as any functional Bolt.

- **Test Bolts (all QA Automation code):**
  A Test Bolt is a third first-class Bolt type whose canonical parent is one
  approved Test Case rather than a feature US or US-000. Its ID and filename
  use `TC-NNN.BOLT-NNN-<description>`. Each Test Bolt may automate only its
  parent TC; one TC may have multiple Test Bolts when the automation must be
  sliced. Test Bolts may change QA Automation code, test data, fixtures,
  mocks, runners, and test-only pipeline configuration within their approved
  scope, but they may not modify production behavior. A detected product
  defect enters the BUG lifecycle.

```mermaid
flowchart TD
    C["Governed source"] --> T{"Bolt type"}
    T -->|"Business behavior"| FB["Functional Analyst → Functional Bolt"]
    T -->|"Technical outcome"| NB["Technical contributor → Non-functional Bolt"]
    T -->|"Approved TC automation"| TB["QA → Test Bolt"]
    FB --> HF{"Functional Analyst approval"}
    NB --> HT{"Architect / Tech Lead approval"}
    TB --> HQ{"QA / technical approval"}
    HF -->|"Changes requested"| FB
    HT -->|"Changes requested"| NB
    HQ -->|"Changes requested"| TB
    HF -->|"AITL-BOLT-READY-Approval"| S["Generate canonical SPEC"]
    HT -->|"AITL-BOLT-READY-Approval"| S
    HQ -->|"AITL-BOLT-READY-Approval"| S
    S --> HS{"AITL-SPEC-Approval"}
    HS -->|"Approved"| V["V-Bounce → mandatory MEM"]
    V --> HM{"AITL-MEM-Approval"}
    HM -->|"Approved"| VC["V-Bounce approved"]
    HM -->|"Changes requested"| V2["New V-Bounce + new MEM"]
    V2 --> HM
```

### 3.2.1 SPEC rules (implementation plan per Bolt)

- **Purpose:** The SPEC is the executable technical plan for one Bolt. It may
  contain the concrete files, components, interfaces, schemas, algorithms,
  implementation phases, tests, observability, migration and rollback steps
  that must not appear in the Bolt.

- **Cardinality:** One Bolt has one current canonical SPEC; one SPEC references
  one and only one Bolt. Revisions update and version that SPEC rather than
  creating parallel plans for the same Bolt.

- **Generation owner:** The AI agent creates the SPEC. A human may steer it,
  request corrections and approve it, but the agent must first perform the
  pre-SPEC evidence gate.

- **Mandatory governed sources:** The exact approved Bolt; the approved parent
  feature US and covered ACs for functional work; the exact approved parent TC
  for Test Bolts; every approved TC used as a verification contract; all applicable approved ADRs;
  and every approved DISC, REV or AREV conclusion or finding the SPEC uses.
  US-000 is referenced without approval for non-functional Bolts. A BUG-driven
  SPEC also requires the exact BUG with `AITL-BUG-Approval`.

- **Mandatory contextual sources:** Relevant `input/` files, affected
  `analysis/` documents, existing code and tests, configuration,
  infrastructure, database schemas and migrations, external interfaces,
  dependency versions, repository conventions, risks, open questions and
  prior execution evidence. The agent determines relevance from the Bolt's
  scope, then records the exact sources and repository baseline used.

- **No hallucinated gap-filling:** The agent may derive implementation detail
  only from the collected evidence and the existing repository. Conflicting,
  missing or ambiguous material is a blocker. The agent records the question
  and stops instead of inventing behavior, architecture, APIs, schemas or
  constraints.

- **Pre-generation blocking:** If a required approval-bearing artifact is not
  approved, the output is a blocking report, not a partial or draft SPEC.
  Optional DISC, REV and AREV artifacts are not required to exist, but once
  used they must have completed their applicable approval lifecycle.

- **Required contents:** Source inventory and approval references; repository
  baseline; scope and exclusions; impacted components and files; detailed
  implementation plan; mappings to ACs or measurable technical outcomes;
  applicable ADR constraints; test strategy and expected evidence; the
  integration test plan (the `integration_test_plan` frontmatter field,
  agreeing with the Bolt's `test_integration`, §3.2.2); quality gates;
  security, data and observability considerations; migration, compatibility
  and rollback plan when applicable; risks, assumptions and explicit stop
  conditions.

- **BUG-specific contents:** A BUG SPEC explicitly defines the single-V-Bounce
  TDD sequence and evidence: reproduce the pre-fix defect with an automated
  test, execute and capture red for the expected reason, then modify production
  code and execute targeted plus applicable regression tests to green. For a
  defect that reproduces only in integration tests, it names the failed
  integration test run that is the red — or leaves it to the MEM when the run
  is recorded after approval — the best hermetic reproduction, and an
  `applicable` integration test plan whose tests include the integration test
  that reproduces the defect (§3.3.1). The SPEC forbids the production change
  before red evidence exists.

- **Test-Bolt-specific contents:** A Test Bolt SPEC maps exactly one approved
  parent TC to the QA Automation code and test-only assets needed to execute
  it. It preserves the TC's expected results, prohibits production-code
  changes, and defines the execution evidence that will demonstrate faithful
  automation.

- **Approval:** The completed draft stops at `AITL-SPEC-Approval`. The
  Dev-validator and applicable domain owner(s) review the actual SPEC and its
  source evidence. Approval authorizes the first code-run/V-Bounce; it does not
  approve the generated implementation.

- **Invalidation:** A material change to any governing source or relevant code
  baseline invalidates the current approval. The code-run pauses, the same
  canonical SPEC is revised, and `AITL-SPEC-Approval` is repeated before work
  resumes. Silent or merely logged mid-run SPEC changes are forbidden.

- **No implementation before approval:** Humans and agents may inspect the
  repository while preparing the SPEC, but may not generate or modify source
  code, automated tests, configuration, infrastructure, migrations, schemas,
  build scripts or deployment definitions until the SPEC is approved.

### 3.2.2 Integration test cycle and acceptance (per Bolt)

- **Where it sits:** After the latest V-Bounce's `AITL-MEM-Approval` and
  before `AITL-BOLT-DONE-Approval`, a Bolt whose `test_integration` is
  `required` and whose SPEC's integration test plan is `applicable` passes
  through its integration test cycle: the verification at the integration test
  level of §3.6 runs against the approved change, by CI, QA or whoever owns
  the environment. There is no integration test checkpoint: a passing run is a
  fact, recorded with the Bolt manifest's integration test runs (§3.12). The
  Bolt enters the cycle at the **Development Completed** event
  (`in-integration-testing`, §3.2.3); a Bolt with no run pending goes straight
  to `ready-for-acceptance`.

- **The Bolt's `test_integration` field:** Every Bolt declares whether its
  **outcome** touches anything verifiable only against real dependencies — a
  database, a queue, storage, an external service or third-party API, deploy
  or infrastructure, a running environment — in two flat frontmatter keys next
  to `bolt_type` and `work_category`: `test_integration` (`required` |
  `not-required`) and `test_integration_reason`, one line, always written —
  also when the value equals the rubric's start — naming what the outcome
  touches or why nothing real is involved (e.g. "feature — persists the
  invoice and calls the payment provider"; "feature — copy text only, no
  backend call"). It is part of the Bolt's WHAT: it names no test, tool,
  environment or technique (§2.4). The agent that drafts the Bolt proposes it
  from the rubric below, as it proposes story points for a feature US (§2.6);
  when a human writes the Bolt, the agent that prepares it for review proposes
  the value before `review_ready_at`. The human never fills it: the Bolt's
  `AITL-BOLT-READY-Approval` approver (§3.0) confirms or changes it in that
  decision, and a Bolt is not ready without the field and its reason (DoR,
  §2.9). After READY it changes only through a **Bolt amendment** re-approved
  at `AITL-BOLT-READY-Approval` and recorded in the Bolt's History; the
  amendment is always followed by a SPEC revision, because the plan must agree
  with the field, and while it is pending no V-Bounce starts. On a `draft`
  Bolt the field is simply edited; on a `done` or `deprecated` Bolt it never
  changes.

- **The rubric:** The value is a judgment on the outcome, never derived
  mechanically from another field. The drafting agent: (1) for a Test Bolt
  (`bolt_type: test`) proposes `not-required`, always, with no exception; (2)
  otherwise starts from `work_category` — `feature` and `infra` → `required`;
  `refactor`, `hardening` and `debt` → `not-required`; (3) asks one question
  of the outcome — does it touch anything verifiable only against real
  dependencies? — and when the answer contradicts the start, changes the value
  and writes why in `test_integration_reason`.

  | `bolt_type` | `work_category` | Start | Typically changes when… | Example of a change |
  |---|---|---|---|---|
  | `test` | `qa_automation` | `not-required` | never | — |
  | `functional` | `feature` | `required` | the outcome touches no real dependency | A feature that only changes copy text or a label → `not-required` |
  | `non-functional` | `infra` | `required` | the outcome changes nothing deployed or running | Lint or editor configuration in the repository only → `not-required` |
  | `non-functional` | `refactor` | `not-required` | the refactor reaches a persistence mapping, a query, a message contract or a client of an external API | Replacing the ORM mapping of an aggregate → `required` |
  | `non-functional` | `hardening` | `not-required` | the control is verifiable only on the running surface | TLS or security headers at the gateway (DAST) → `required` |
  | `non-functional` | `debt` | `not-required` | the outcome changes the pipeline, the deployment or a driver/SDK of a real dependency | A `debt` Bolt that changes the deployment pipeline → `required` |

  A BUG-driven Bolt follows the rubric of its type and category; one whose
  defect reproduces only in integration tests is `required` (below).

- **Project-level rules:** (4) An active ADR of the project declaring "no
  integration test cycle in this project" — stating why integration testing
  does not apply to the project, an owner, and the condition that ends it (the
  first real dependency or environment, a release, or an expiry date) — takes
  precedence over steps 2–3: from the next Bolt drafted after its approval,
  the drafting agent proposes `test_integration: not-required` for every Bolt,
  with `test_integration_reason: "project ADR-NNN: no integration test
  cycle"`, and the SPEC's plan is `n/a` citing that ADR. The Bolt approved at
  the READY where that ADR is recorded may be set the same way in that
  decision; active Bolts are not rewritten — an affected one is amended
  through a new READY. When the condition is met the ADR is superseded and the
  rubric applies again from the next Bolt drafted. The reason names the ADR so
  that "not integration-tested by project policy" is counted apart from
  "touches no real dependency" (§3.7.2). (5) The absence of integration tests
  is never turned into `not-required`: a project with no integration tests and
  no integration test environment has simply not started them, and the first
  Bolt that touches a real dependency is the one that writes the first
  integration test. When the agent finds none (no integration tests, no
  integration test environment or CI job), it says so once and proposes the
  ADR of rule 4 ("will this project have an integration test cycle?"); until
  that ADR exists it applies steps 1–3 and notes the observation in the reason
  (e.g. "required — touches the database; note: no integration tests exist yet
  in this project"). The human decides at READY: this Bolt starts the
  project's integration tests, or the ADR is recorded and this Bolt may be set
  `not-required` in the same READY, citing it.

- **The integration test plan:** The Bolt says WHAT; the approved SPEC says
  HOW, in its frontmatter (`integration_test_plan`, see
  `spec/TEMPLATE-SPEC.md`): `applicable`, with the tests, the environment and
  the data; `n/a`, with a reason; or `waived`, citing the project's ADR. It is
  `applicable` when any gate at the integration test level of the SPEC's §9
  applies, `waived` when every one that applies is waived by that ADR, and
  `n/a` when none applies. The plan is part of the SPEC and changes only
  through a SPEC revision. It agrees with the Bolt:

  | Bolt `test_integration` | Allowed `integration_test_plan.applicability` |
  |---|---|
  | `required` | `applicable` or `waived` (`waived` never for an integration-test-only BUG Bolt, below) |
  | `not-required` | `n/a` |

  A mismatch is caught by the pre-SPEC evidence gate (§2.4.1): the SPEC's
  author emits a blocking report instead of a SPEC, and `AITL-SPEC-Approval`
  is never recorded on an incoherent revision. When the SPEC's author
  concludes that the Bolt's value is wrong, the way forward is the Bolt
  amendment above, never a plan that contradicts the Bolt. The field and the
  plan tell the actor who records `AITL-MEM-Approval` which transition applies
  (§3.2.3, T07 or T08).

- **Who writes the integration tests:** The plan lists the tests a run
  executes, whoever wrote them: a developer or a tester in the Bolt's own
  V-Bounce — any executor, one at a time (G11), handoffs recorded in the
  Bolt's History — or another Bolt or suite, e.g. a Test Bolt already
  delivered or a shared integration test suite. A dependency on tests not yet
  delivered is declared in the Bolt's `dependencies`: the Bolt cannot reach a
  passing covering run before them. Integration tests written in a V-Bounce
  must build and pass static checks there and are executed in the integration
  test cycle. The run that verifies this Bolt is always recorded on this Bolt.

- **A failed run is classified by a human:** a recorded decision with an
  accountable person, not an approval — it adds no checkpoint, and the
  independence rules of §3.0 do not apply to it. The red run of an
  integration-test-only BUG Bolt — recorded before its V-Bounce, so before
  any approved change of the Bolt exists — needs no classification: it
  covers no approved change of the Bolt (§3.3.1); a covering run that fails
  is classified, even when it is the red of a correction V-Bounce. The
  executing Dev-validator is the recommended classifier — guidance, never a
  gate; any qualified human may record it. The cause decides the way forward:
  `code` or `test` in this
  Bolt's own change → a new V-Bounce; `spec` → a SPEC revision, then a new
  V-Bounce; `environment` → a new run; `test` in tests owned by another Bolt
  or suite → fixed there, while this Bolt waits in `integration-tests-blocked`
  for a new covering run, the reason naming the owner. The classifier applies
  the transition the cause gives (§3.2.3) and appends its timeline row;
  whoever records a run applies the run's transition the same way. When CI
  appends runs automatically, the actor who transcribes a run into the Bolt's
  records applies its transition in that act.

- **Skipping integration testing:**

  | Case | How | Path |
  |------|-----|------|
  | Integration testing does not apply (documentation, code with no external dependency, every Test Bolt — never an integration-test-only BUG Bolt) | `test_integration: not-required` at READY; plan `n/a` with a reason in the approved SPEC | `in-development` → `ready-for-acceptance` (T08) → `done` (T20), or the joint act |
  | It applies, but the team does not run it | `test_integration: required`; plan `waived`, citing an ADR of the project with reason, owner, compensating control and expiry (G21) | Same |
  | Same, on a `high` or `critical` Bolt | `waived` **plus the evidence that the compensating control was executed**, attached at `AITL-BOLT-DONE-Approval` | Same |
  | It applied and was planned, but this time it is not run | A human waives it (T23) from `in-integration-testing` or `integration-tests-blocked` with a one-line reason; compensating-control evidence recommended on `high`/`critical`, never required; `test_integration` stays `required` | → `ready-for-acceptance` (T23) → `done` (T20) |

  The SPEC-time rows block on `high`/`critical` (an ADR, the executed
  compensating control); the late waiver blocks at no risk class. The
  asymmetry is deliberate: a planned skip is a substitute chosen before the
  work, its control already chosen, so requiring it executed is cheap; a late
  waiver is an honest escape whose alternative is a parked Bolt or a
  fabricated run, so it is governed by visibility — human-only, a written
  reason, shown at acceptance, counted apart (§3.7.2). A skip is always
  recorded as `not-required` / `n/a`, `required` / `waived`, or a late waiver;
  `pass` exists only as the outcome of a run, so a skip never reads as `pass`.

- **Integration testing waived by a human (T23):** Only from
  `in-integration-testing` (no run yet) or `integration-tests-blocked` (the
  run could not execute, or the failure was classified `environment` or `test`
  owned elsewhere); **never from `integration-tests-failed`** — a failed run
  is classified first, so a waiver never hides a possible product defect.
  Never for an integration-test-only BUG Bolt (below). Signed only by a human,
  never by an agent, configured or not: a recorded decision, not an approval,
  with no checkpoint. The one-line reason is recorded twice in the same act:
  the timeline row, whose *Act / evidence* begins
  `Integration testing waived —`, and the waiver recorded with the Bolt
  manifest's integration test runs (§3.12) — who, when, the reason, the change
  it covers and the run(s) it closes. On a `high` or `critical` Bolt the
  evidence that a compensating control was executed is recommended, never
  required: when present it is recorded with the waiver and named in the row;
  when absent the row says "no compensating control recorded".
  `test_integration` stays `required` — no amendment, no new READY, no SPEC
  revision. The `AITL-BOLT-DONE-Approval` approver sees "integration testing
  waived — <reason>" in the acceptance evidence and may answer
  `changes_requested` to have the run after all. A repeated pattern of late
  waivers is the signal for the project ADR of rule 4. Example (illustrative;
  `t0…t7` are placeholders):

  | Date | From | To | Act / evidence |
  |---|---|---|---|
  | t0 | — | `draft` | Bolt created — `human:<author>`; `test_integration: required` proposed ("feature — writes the order and calls the payment provider") |
  | t1 | `draft` | `ready` | `AITL-BOLT-READY-Approval` approved — `human:<approver>`; `test_integration: required` confirmed |
  | t2 | `ready` | `specified` | `AITL-SPEC-Approval` approved, revision 1 (plan `applicable`) |
  | t3 | `specified` | `in-development` | V-Bounce 1 starts — `agent:<developer>` (T04) |
  | t4 | `in-development` | `in-integration-testing` | `AITL-MEM-Approval` approved, V-Bounce 1 — **Development Completed** |
  | t5 | `in-integration-testing` | `integration-tests-blocked` | Run 1 `blocked` — staging unavailable (CI log) |
  | t6 | `integration-tests-blocked` | `ready-for-acceptance` | Integration testing waived — staging down until next sprint; contract tests with doubles green; no compensating control recorded — `human:<dev-validator>`; closes run 1 |
  | t7 | `ready-for-acceptance` | `done` | `AITL-BOLT-DONE-Approval` approved — `human:<po>`, with "integration testing waived — staging down until next sprint" in view |

- **Test Bolts:** A Test Bolt (`bolt_type: test`,
  `work_category: qa_automation`) is always `test_integration: not-required`,
  its plan `n/a` with the reason "Test Bolt — executed in the TC's run cycle".
  Its V-Bounce builds the automation and runs what is hermetic; the real
  execution of that automation is the TC's own verification, outside the
  per-Bolt integration test cycle. Its acceptance criterion "TC automated with
  execution evidence" (§3.11) is unchanged.

- **The integration-test-only BUG Bolt:** A BUG Bolt whose defect reproduces
  only in integration tests is `test_integration: required` with plan
  `applicable`, and its `test_integration_reason` begins
  `integration-test-only BUG —`. Neither a `waived` plan nor the late waiver
  is available to it: its green evidence is the passing run. From
  `in-integration-testing` or `integration-tests-blocked` its only exits are a
  passing covering run, once the environment is fixed, or deprecation (T22),
  whose reason names what owns its already-merged change. Its red evidence
  before the V-Bounce follows §3.3.1.

- **Bolts in flight at adoption:** A Bolt whose approved SPEC predates the
  `integration_test_plan` field is read as `n/a` with the reason "SPEC
  predates the integration test plan" (T08); the migration gives it
  `test_integration: not-required` (§5.16), so the pair is coherent; its next
  SPEC revision declares a plan coherent with the field, and if the team
  judges the outcome `required`, the Bolt is amended first.

- **WIP and the turn budget:** A Bolt `in-integration-testing`,
  `integration-tests-failed` or `integration-tests-blocked` does not occupy
  its executor (§3.2 WIP); a failure classified `code`, `test` or `spec` in
  its own change returns it to the executor with priority. The turn budget
  counts the V-Bounce's own loop only: waiting for an environment, and the
  integration test cycle as a whole, consume no turns (§3.0).

- **What acceptance requires:** `AITL-BOLT-DONE-Approval` is requested only
  when the Bolt is `ready-for-acceptance` and requires, besides the completion
  evidence, its integration test evidence: a passing covering run; or the
  `not-required` reason (plan `n/a`); or the `waived` ADR — plus, on a `high`
  or `critical` Bolt, the executed compensating-control evidence; or a late
  waiver shown as "integration testing waived — <reason>" with its
  compensating-control evidence or "no compensating control recorded". Its
  outcomes and the ownership of a rejected Bolt's merged change are in §3.2.3
  (*The acceptance decision*).

- **The joint MEM + DONE act:** When all three hold — (a) the MEM approval
  takes the Bolt through T08, no integration test run pending (`not-required`,
  or `required` with a `waived` plan); (b) the signer holds both the
  Dev-validator role (the executor who signs `AITL-MEM-Approval`) and the
  Bolt's acceptance role by `work_category` (§3.11), the self-assignment rules
  of §3.0 applying to each; (c) the acceptance evidence is in the package (the
  completion evidence, the demo form of the category, the `n/a` reason or the
  `waived` ADR, and on a `high` or `critical` Bolt the executed
  compensating-control evidence) — a human may record `AITL-MEM-Approval` and
  `AITL-BOLT-DONE-Approval` in one act. Two checkpoints, two records: each in
  its own document block and its own `checkpoint_approvals[]` entry, with its
  own role; AITL coverage is unchanged. Two transitions, two timeline rows at
  the same timestamp — T08 (**Development Completed**), then T20 — and the
  three acceptance times take the act's time. **Never implicit:** the DONE is
  recorded only when the human says so; a bare MEM approval stops at
  `ready-for-acceptance`. When the MEM of such a Bolt is approved without that
  instruction and (b)–(c) can hold, the agent writing the record asks once, in
  the human's language, never asks again for the same approval, and never
  records the DONE on silence. Unavailable with a run pending, to a signer
  without the acceptance role (the agent says who holds it), to an agent
  approver (each checkpoint only as its own configuration allows), and after a
  late waiver (its DONE is its own act, T20).

### 3.2.3 Bolt status, transitions and the Status timeline

A Bolt's frontmatter `status` is where the Bolt stands. It is **declared** by
the actor who records the act that changes it, holds **exactly one value at a
time**, and is read from the Bolt document alone — never from a manifest, a
SPEC or a calculation. The values are schema (§3.15): English, kebab-case,
never translated; the *Shown as* labels are the human form, used in INDEX
tables, reports and diagrams.

**The values** (in §3.15 order):

| `status` | Shown as | The Bolt is… |
|---|---|---|
| `draft` | Draft | Written, not yet approved at `AITL-BOLT-READY-Approval` |
| `ready` | Ready | Approved at `AITL-BOLT-READY-Approval` (DoR met), with no approved SPEC revision governing it now — awaiting its first SPEC, or a new revision after a material change (G15) |
| `specified` | Specified | Governed by an approved SPEC revision that no V-Bounce has started executing |
| `in-development` | In Development | Being executed, or awaiting its next V-Bounce: a MEM pending review or not approved, or a failed integration test run classified `code`, `test` or `spec` in this Bolt's own change |
| `blocked` | Blocked | Its latest V-Bounce ended `blocked` and that V-Bounce's MEM is not yet decided — the stop-and-ask moment (§3.0) |
| `in-integration-testing` | In Integration Testing | Past **Development Completed** with `test_integration: required` and an `applicable` integration test plan; no covering run recorded yet |
| `integration-tests-failed` | Integration Tests Failed | A covering run failed; awaiting its human classification |
| `integration-tests-blocked` | Integration Tests Blocked | A covering run could not execute, or a failure was classified `environment` or `test` in tests owned by another Bolt or suite; awaiting a new run |
| `ready-for-acceptance` | Ready for Acceptance | Integration tests passed; or `test_integration: not-required`; or `required` with a `waived` plan; or its pending integration testing waived by a human (T23); `AITL-BOLT-DONE-Approval` may now be requested |
| `done` | Done | Accepted (`AITL-BOLT-DONE-Approval` approved) — terminal |
| `deprecated` | Deprecated | Withdrawn, with its reason — terminal |

**The events:**

| Event | What it is | Transition it causes |
|---|---|---|
| **Development Completed** | `AITL-MEM-Approval` `approved` on the Bolt's latest V-Bounce | T07 or T08, at once |
| **Integration Tests Passed** | A passing run covering the Bolt's latest approved change | T09 or T14, at once |

Neither is ever a `status` value. A report may show an event as a qualifier
of the status it led to (*Ready for Acceptance — integration tests passed*, *—
not required*, *— waived*, *— integration testing waived*).

**The transition table.** Every allowed change of `status` is a row of this
table; any other change is illegal, except a correction row of the Status
timeline (below). *Recorded by* is the actor who records the act — the actor
itself, or the Coordinator transcribing a human's decision (§3.0): the
Coordinator records, never signs. When the act is an AITL decision, that
actor is the decision record's `written_by` (§3.0); for the other acts the
column is the rule of who records them, with no field of its own.

| # | From | To | Act that causes it | Recorded by | Event |
|---|---|---|---|---|---|
| T01 | — | `draft` | The Bolt is created, with its proposed `test_integration` (§3.2.2) | Its author | |
| T02 | `draft` | `ready` | `AITL-BOLT-READY-Approval` `approved` — the `test_integration` value confirmed or changed in the same decision | The actor recording the decision | |
| T03 | `ready` | `specified` | `AITL-SPEC-Approval` `approved` on the latest SPEC revision, its integration test plan agreeing with `test_integration` (§3.2.2) | The actor recording the decision | |
| T04 | `specified` | `in-development` | V-Bounce *N* starts on the approved revision — the executor takes the Bolt (G11) | The executor | |
| T05 | `in-development` | `blocked` | V-Bounce *N* ends with `execution_outcome: blocked` (its MEM and manifest entry created) | The executor | |
| T06 | `blocked` | `in-development` | `AITL-MEM-Approval` `changes_requested` or `rejected` on the blocked V-Bounce | The actor recording the decision | |
| T07 | `in-development` | `in-integration-testing` | `AITL-MEM-Approval` `approved` on the latest V-Bounce; `test_integration: required` and plan `applicable` | The actor recording the decision | Development Completed |
| T08 | `in-development` | `ready-for-acceptance` | `AITL-MEM-Approval` `approved` on the latest V-Bounce; `test_integration: not-required` (plan `n/a`, or none because the SPEC predates the integration test plan, §3.2.2), or `required` with plan `waived` — the *Act / evidence* names the waiving ADR | The actor recording the decision | Development Completed |
| T09 | `in-integration-testing` | `ready-for-acceptance` | A covering run with outcome `pass` | The actor recording the run | Integration Tests Passed |
| T10 | `in-integration-testing` | `integration-tests-failed` | A covering run with outcome `fail` | The actor recording the run | |
| T11 | `in-integration-testing` | `integration-tests-blocked` | A covering run with outcome `blocked` | The actor recording the run | |
| T12 | `integration-tests-failed` | `in-development` | Classification of the failed run, cause `code`, `test` or `spec`, in this Bolt's own change | The human classifier (§3.2.2) | |
| T13 | `integration-tests-failed` | `integration-tests-blocked` | Classification of the failed run, cause `environment`; or cause `test` in tests owned by another Bolt or suite, the reason naming it | The human classifier | |
| T14 | `integration-tests-blocked` | `ready-for-acceptance` | A new covering run with outcome `pass` | The actor recording the run | Integration Tests Passed |
| T15 | `integration-tests-blocked` | `integration-tests-failed` | A new covering run with outcome `fail` | The actor recording the run | |
| T16 | `integration-tests-blocked` | `in-development` | A later classification of the same failed run, cause `code`, `test` or `spec` in this Bolt's own change (it supersedes the earlier one); or V-Bounce *N+1* starts | The human classifier / the executor | |
| T17 | `in-development` | `integration-tests-blocked` | A later classification of the same failed run, cause `environment` or `test` in tests owned elsewhere, before any V-Bounce or SPEC revision since the earlier classification | The human classifier | |
| T18 | `in-integration-testing` | `in-development` | V-Bounce *N+1* starts before any covering run | The executor | |
| T19 | `ready-for-acceptance` | `in-development` | V-Bounce *N+1* starts (a change needed at acceptance, not material to the SPEC) | The executor | |
| T20 | `ready-for-acceptance` | `done` | `AITL-BOLT-DONE-Approval` `approved` — alone, or in the joint act right after T08 (§3.2.2) | The actor recording the decision | |
| T21 | `specified`, `in-development`, `blocked`, `in-integration-testing`, `integration-tests-blocked`, `ready-for-acceptance` | `ready` | A new SPEC revision is appended (G15) — including the one that follows an amendment of `test_integration` (§3.2.2); the previous revision no longer governs | The SPEC's author | |
| T22 | any non-terminal | `deprecated` | Withdrawal with its reason: `AITL-BOLT-READY-Approval` `rejected` on a `draft` Bolt, `AITL-BOLT-DONE-Approval` `rejected`, or the outcome abandoned. From any status after a **Development Completed** event, the reason names who owns the merged change (below) | The actor recording the decision | |
| T23 | `in-integration-testing`, `integration-tests-blocked` | `ready-for-acceptance` | **Integration testing waived by a human:** the waiver is recorded with the Bolt manifest's integration test runs, with its one-line reason and the compensating-control evidence when there is one (§3.2.2); *Act / evidence* begins `Integration testing waived —`. Never from `integration-tests-failed`; never for an integration-test-only BUG Bolt | The human who waives it — never an agent | |

**Acts that change nothing** — no `status` change and no timeline row; the
fact is still recorded where it always is: `AITL-BOLT-READY-Approval` or
`AITL-SPEC-Approval` `changes_requested`; any `AITL-BOLT-READY-Approval`
decision on an amendment of a Bolt that is not `draft` (`rejected` there
refuses the amendment, not the Bolt); a V-Bounce ending `ready_for_review`,
`failed` or `cancelled`; `AITL-MEM-Approval` `changes_requested` or `rejected`
on a V-Bounce that is not `blocked`; a V-Bounce *N+1* starting while
`in-development`; a new run `blocked` while `integration-tests-blocked`;
`AITL-BOLT-DONE-Approval` `changes_requested`; a run that does not cover the
latest approved change. **Not an act at all:** a waiver while
`integration-tests-failed` — it is unavailable there; the failure is
classified first (T12, T13).

**Exits per status** — `draft` T02, T22 · `ready` T03, T22 · `specified` T04,
T21, T22 · `in-development` T05, T07, T08, T17, T21, T22 · `blocked` T06, T21,
T22 · `in-integration-testing` T09, T10, T11, T18, T21, T22, T23 ·
`integration-tests-failed` T12, T13, T22 · `integration-tests-blocked` T14,
T15, T16, T21, T22, T23 · `ready-for-acceptance` T19, T20, T21, T22 · `done`,
`deprecated` none (terminal). **Entries to `ready-for-acceptance`:** T08, T09,
T14, T23. An `integration-tests-failed` Bolt has no SPEC-revision, V-Bounce or
waiver exit: it leaves only through its classification. T23 shares its
(*From*, *To*) pairs with T09 and T14; its *Act / evidence* prefix tells it
apart.

**The transition rules:**

- **A V-Bounce that did not reach review.** `AITL-MEM-Approval` `approved` is
  available only when the V-Bounce's `execution_outcome` is
  `ready_for_review`: approval requires a package that satisfies the approved
  SPEC (§2.12). The MEM of a `failed`, `blocked` or `cancelled` V-Bounce is
  reviewed `changes_requested` — the way forward is a new V-Bounce, preceded
  by a SPEC revision when the cause is in the SPEC — or `rejected`. A
  `blocked` V-Bounce holds the Bolt `blocked` until that decision (T05, T06).
- **The most recent act wins.** Acts are applied in the order they happen,
  each against the current `status`. A V-Bounce newer than the last covering
  run returns the Bolt to `in-development` (T16, T18, T19); a SPEC revision
  returns it to `ready` (T21), then `specified` once approved (T03). One
  V-Bounce never spans two SPEC revisions (G16): a V-Bounce in progress first
  ends with its MEM. A later classification citing the same failed run
  supersedes the earlier one — both stay recorded — and the most recent
  governs (T16, T17).
- **The covering run.** Only a run recorded after the latest approved
  `AITL-MEM-Approval` and covering the commit that holds that V-Bounce's
  change moves the Bolt (T09–T11, T14, T15). Whether a run's coverage includes
  that change is checked by tooling against the repository: the manifest
  records the V-Bounce's baseline, not the merge commit.
  `ready-for-acceptance` reached through integration testing always refers to
  the latest approved change; any newer V-Bounce, or a SPEC revision, needs a
  new covering run. The same holds for a late waiver (T23): it covers the
  latest approved change only, and a newer V-Bounce needs a new run or a new
  waiver.
- **Batched runs.** A Bolt becomes `ready-for-acceptance` through integration
  testing only by a passing run that covered its change. A failing batched run
  moves **every** Bolt it covered to `integration-tests-failed`. Each covered
  Bolt records the run and its own classification in its own manifest (the
  same cause may be transcribed per Bolt), and its own transition and timeline
  row in its own document; it leaves `integration-tests-failed` only through a
  classification recorded for it. A Bolt cleared by its classification needs a
  new passing covering run. A late waiver is always per Bolt: waiving one Bolt
  of a batch moves no other.
- **Classification by cause.** `code` or `test` in this Bolt's own change →
  `in-development` until the new V-Bounce; `spec` → `in-development` until the
  SPEC revision (then T21); `environment` → `integration-tests-blocked` until
  a new run; `test` in tests owned by another Bolt or suite →
  `integration-tests-blocked` until the owner fixes them and a new run covers
  this Bolt's change (§3.2.2).
- **The acceptance decision.** `approved` → `done` (T20).
  `changes_requested` changes nothing: the Bolt stays `ready-for-acceptance`.
  Missing evidence (a passing run, the `n/a` reason, the `waived` ADR, the
  compensating-control evidence) is added and leads to a new acceptance
  decision; a needed change leads to a new V-Bounce (T19) and through the
  cycle again — through a SPEC revision first (T21, G15) when the change is
  material to the SPEC. `rejected` → `deprecated` (T22) with the reason; if
  the outcome is still needed, it is reformulated as a new Bolt, and a
  rejected Bolt is never left half-closed. **Its change is already merged**
  (G20), so the reason names what owns that code next: the replacement Bolt,
  which inherits and corrects it, or a reverting Bolt (G07). Merged code is
  never left without a Bolt.
- **The acceptance timing.** `acceptance_review_ready_at` (Bolt document) and
  `bolt.acceptance.review_ready_at` (manifest) are set in the same act as the
  transition into `ready-for-acceptance` (T08, T09, T14 or T23), never at an
  earlier status, and `AITL-BOLT-DONE-Approval` is never requested before it.
  The acceptance review budget then measures the acceptance wait, and the time
  in integration testing (§3.7.2) the integration testing wait, without
  overlap. In the joint act (§3.2.2) the three acceptance times —
  `acceptance_review_ready_at`, the acceptance review's start and the
  `AITL-BOLT-DONE-Approval` decision — take the act's time.
- **A second acceptance round.** The manifest keeps the first acceptance
  submission (`bolt.acceptance`, never rewritten); a later round is read from
  its own `AITL-BOLT-DONE-Approval` entry and the Bolt document's
  `acceptance_review` block, which holds the latest round.
- **A SPEC's own `blocked` status does not move the Bolt.** The Bolt is
  `blocked` only through a V-Bounce outcome (T05). A SPEC whose document
  status is `blocked` (an external dependency, §3.15) explains why a
  `specified` Bolt waits; it is not a Bolt act.
- **`done` is never reopened.** A failure found after `done` is a new BUG
  (§2.9, §2.16).

**Same act, two records — the Status timeline.** The actor who records an act
of the transition table does, **in the same act** (the same change to the
repository):

1. records the fact where it is recorded — the manifest's arrays, the review
   block of the document that carries the checkpoint;
2. sets the Bolt's `status` to the transition's *To*;
3. appends one row to the Bolt document's `## Status timeline` table.

| Column | Content |
|---|---|
| **Date** | ISO 8601 with seconds and offset, from the clock — the time of the act |
| **From** | The previous `status` (`—` for the first row) |
| **To** | The new `status` |
| **Act / evidence** | The act and its evidence, naming the actor: the checkpoint and decision, the V-Bounce number and MEM, the run number and its evidence link, the classification cause, the SPEC revision, the event, the waiving ADR on a T08 of a `required` Bolt, a late waiver's reason with its compensating-control evidence or the words "no compensating control recorded" (T23), the deprecation reason |

The timeline is **append-only**: a row is never edited or deleted. A
recording error is fixed by a **correction row** — *From* is the current
(wrong) `status`, *To* the right one, and *Act / evidence* begins
`Correction —` and cites the row it corrects and why. A correction row is
exempt from the transition table, never from continuity, and is never a way
to skip an act: the act it reflects must be recorded too. The Bolt template
(`functional/bolts/TEMPLATE-BOLT.md`) ships the section with its first row
and a legend of the values and events.

**The document governs the status; the manifests keep the facts:**

- The manifests keep every fact they record — timing,
  `checkpoint_approvals[]`, `spec_revisions[]`, `v_bounces[]`, the integration
  test runs (§3.12) — for metrics, reports and audit. They are never the
  source of the Bolt's status and carry no status field (§3.12).
- A mismatch between the Bolt's `status` and those facts is a **recording
  error**. For where the Bolt stands, the Bolt document governs; the error is
  corrected by recording the missing fact or by a correction row, never by
  editing history.
- The status **never substitutes for an approval** (§3.15: `status` is a
  document state). A Bolt whose `status` reads `done` without a recorded
  `AITL-BOLT-DONE-Approval` is not accepted: G20 and G22 still block
  promotion, and the missing record is the defect to fix. The same holds for
  every transition a checkpoint causes, the two of the joint act included
  (§3.2.2).

**A consistent Bolt document.** Reading only the Bolt document, it is
**consistent** when every rule below holds:

| # | Rule |
|---|---|
| C1 | `status` is one of the eleven values of the §3.15 Bolt row |
| C2 | It has a `## Status timeline` table with the columns Date · From · To · Act / evidence and at least one row |
| C3 | The first row is `—` → `draft`, or a migration seed row (`—` → any value, *Act / evidence* beginning `Migration —`, §5.16) |
| C4 | Each row's *From* equals the previous row's *To* |
| C5 | Each (*From*, *To*) pair is a transition of the table above, except a correction row |
| C6 | The last row's *To* equals `status` |
| C7 | Dates are ISO 8601 with offset and non-decreasing (equal dates are allowed: the joint act, §3.2.2) |
| C8 | `test_integration` is `required` or `not-required`, and `test_integration_reason` is not empty |
| C9 | `bolt_type: test` ⇒ `test_integration: not-required` |
| C10 | `test_integration: not-required` ⇒ `status` is none of `in-integration-testing`, `integration-tests-failed`, `integration-tests-blocked` |
| C11 | A row whose *Act / evidence* begins `Integration testing waived —` (T23) has *From* `in-integration-testing` or `integration-tests-blocked`, *To* `ready-for-acceptance`, and a reason after the prefix |
| C12 | A Bolt whose `test_integration_reason` begins `integration-test-only BUG —` is `test_integration: required` and has no `Integration testing waived —` row |

Legality is checked by pair: *Act / evidence* is prose for people, so a tool
can tell that no act could have produced an illegal pair, not which act
produced a legal one. Four fixed prefixes are the exceptions it does read:
`Correction —`, `Migration —` and `Integration testing waived —` mark an act
whose legality depends on more than the pair (C3, C5, C11), and
`integration-test-only BUG —`, in `test_integration_reason`, constrains the
field and its waivability (C12). For a `not-required` Bolt the document alone
tells T08 from T07 (C10); for a `required` one, a T08 row names the waiving
ADR. Comparing the status or the field with a manifest or the SPEC is outside
this check; a tool that adds such a comparison reports a warning and never
overrides the Bolt document. Archived Bolts keep the vocabulary of the version
that closed them (§5.16) and are not read by this check.

## 3.3 V-Bounce rules (execution form)

- **No commits without an explicit user request:** the agent never stages,
  commits, pushes, or opens pull requests on its own. Version-control
  actions happen only when the human explicitly asks (G34). Artifacts are
  written to the working tree; the human owns the repository history.
- **No unsolicited internet research:** the agent does not search the web
  proactively or crawl pages recursively. Web tools are used only when the
  human explicitly asks for an internet search, or when an approved
  SPEC/ADR/TC requires verifying third-party information (e.g. a library's
  API at implementation time). DevFlow investigations live in governed
  artifacts (DISC, analysis) — the web is not a default step and consumes
  tokens without governed value.
- **No unsolicited artifact creation:** the agent creates a governed
  artifact only inside an explicit **work mandate** from a human (G40): a
  direct request for that artifact, or a **delivery mandate** on an approved
  artifact ("implement US-022") that authorizes exactly the chain the
  methodology derives from it — its Bolts, each Bolt's SPEC, each V-Bounce's
  MEM and manifest entries, and the OQs raised inside those artifacts
  targeting them — and nothing else. A question, an analysis request, an
  opinion, a described problem or thinking aloud is never a mandate: the
  deliverable is the answer, and an artifact that would help is **proposed**
  by name and left for the human to extend the mandate. A mandate is never an
  approval — every `AITL-*-Approval` inside the chain still stops the agent,
  even when the mandate says "coordinate everything" — and a mandate on a
  draft artifact authorizes nothing executable — it reaches no further than the
  drafting loop (G01). No agent ever widens its own
  mandate: the Coordinator propagates a bounded copy to the executors it
  spawns; an executor inherits it, never extends it. An unrequested draft is
  not free: it consumes an ID that is never reused (§2.4) and rows in an
  INDEX and its manifests (G33).
- **Fixed anatomy:**
  1. **Approved SPEC** with recorded `AITL-SPEC-Approval`, referencing a Bolt with its individual
     `AITL-BOLT-READY-Approval`; a functional Bolt additionally references the
     `AITL-US-Approval` of its feature parent, while a non-functional Bolt
     references US-000 without parent approval. A BUG Bolt additionally
     references its exact BUG and `AITL-BUG-Approval`. A Test Bolt references
     exactly one parent TC and its `AITL-TC-Approval`. The executor who takes
     the Bolt to start the V-Bounce sets its `status` to `in-development` and
     appends its Status timeline row in that act (§3.2.3).
  2. **AI agent generates the intended-final docs/design/code/tests by
     default** and **runs the hermetic suite — build, static checks and the
     tests that need no running environment (§3.6) — inside its own loop
     until it is green or a mandatory stop condition is reached**.
     Integration tests are part of the change: the agent writes them here,
     from the same ACs and SPEC, and they must build and pass static checks,
     but they are **executed in the Bolt's integration test cycle** — a Bolt
     whose `test_integration` is `required` — never in this loop; a Test Bolt's
     automation runs in its TC's run cycle (§3.2.2). Waiting for an environment
     never happens here, so it never
     consumes the turn budget. Any retries or corrections performed without
     human intervention remain internal to this V-Bounce. A direct human patch is an allowed fallback only when it is
     recorded in the MEM and verified through the same gates and AITL review.
  3. **AI agent creates the mandatory MEM + updates manifest** — the agent
     writes exactly one narrative record
     (`memory/MEM-YYMMDD-HHmm-<description>.md`) for the
     V-Bounce, with all required content from §2.12 and no mutable approval
     status, then appends the corresponding `v_bounces[]` entry to the Bolt
     manifest in `devflow/metrics/bolts/`. This happens **before** human review so
     the reviewer receives the complete package. A V-Bounce that ends
     `blocked` also sets the Bolt `blocked` (§3.2.3).
  4. **`AITL-MEM-Approval`** (dev / QA / security / applicable domain) — the
     reviewer reads the **actual diff + test/gate evidence + MEM + manifest**,
     records the complete review evidence in the MEM/checkpoint record and the
     minimal decision in `checkpoint_approvals[]`, and thereby decides the current
     V-Bounce. An Adversarial Review, when a stakeholder triggered one on this
     package, is a **pre-filter for this decision** and not a stage of the
     V-Bounce (§2.15).
  5. **Outcome recorded:**
     - **Approved:** `AITL-MEM-Approval` approves and completes the V-Bounce;
       when this is the latest V-Bounce, this is the **Development Completed**
       event: the Bolt becomes `in-integration-testing`
       (`test_integration: required`, plan `applicable`) or, when no
       integration test run is pending (`test_integration: not-required`, or a
       `waived` plan), `ready-for-acceptance` (§3.2.3).
     - **Changes requested or rejected:** the V-Bounce and its MEM remain
       unapproved in history and the Bolt remains `in-development`. The next
       autonomous agent
       execution is a new V-Bounce, creates a new MEM, and is appended as a new
       `v_bounces[]` entry.

- **Mandatory post-execution sequence (non-negotiable):** After the agent
  reaches a reviewable result or a mandatory stop condition (step 2), and
  **before** yielding control for human review (step 4), it **must** execute
  these steps in this exact order:

  ```
  Record implementation + verification outcome (including blockers)
  → Create mandatory MEM → Update Bolt manifest
  → PAUSE at AITL-MEM-Approval
  ```

  The human reviewer receives the **complete package**: code + tests/gates +
  MEM + manifest. No partial packages. A V-Bounce without exactly one complete
  MEM and its manifest update is incomplete and cannot enter
  `AITL-MEM-Approval`; without an approved checkpoint it cannot be considered
  an approved V-Bounce. Red required gates, unresolved blockers, or an
  implementation that does not satisfy the SPEC force a non-approved decision,
  but never excuse omission of the MEM.

- **Single active executor per Bolt:** Only one developer/agent controls the
  active V-Bounce at a time. A handoff may occur only after the current
  V-Bounce has produced its MEM and manifest entry and paused at
  `AITL-MEM-Approval`. Any subsequent autonomous execution is a new V-Bounce;
  if governed scope or implementation instructions changed materially, the
  Bolt and/or canonical SPEC must be re-approved first. Historical entries
  remain immutable.
  - **Handoff protocol:** the handoff is documented in the Bolt's History
    section (date, outgoing executor, incoming executor, reason). After
    handoff, the **incoming executor** is the
    Dev-validator who reviews and approves the pending MEM of the previous
    V-Bounce (it is still their first action on the Bolt); `created_by` in
    the manifest records the executor of each generation — the next
    V-Bounce's `code_generation` uses the incoming executor's identity.
    A pending MEM cannot be approved by the outgoing executor once the
    handoff is recorded.

- **Bounces per Bolt:** as needed (typically 1–3). Count every package
  submitted for human validation, including rejected packages. Autonomous
  internal retries before submission do not increment this metric.

- **Time:** Active delivery time is used for the one-day sizing target. Total
  elapsed cycle time, including waits, is measured separately. Crossing a day
  boundary is an accepted variance and never creates a new Bolt by itself.
  AI-time remains a sub-metric.

- **Test-first where behavior changes:** For functional and BUG-driven Bolts,
  and for non-functional Bolts whose expected outcome is testable before the
  implementation change, the agent derives tests from ACs and governed
  constraints and uses them as the V-Bounce's brake. The applicable evidence
  is recorded in the MEM. Documentation-only, infrastructure-only and other
  work for which a pre-change automated test is not meaningful records the
  gate as `n/a` with a reason rather than manufacturing an empty test commit.
  For a BUG Bolt, the stronger strict-TDD rule in §3.3.1 applies: the agent
  must execute the reproduction test against pre-fix behavior, record the
  expected red result, and only then modify production code and proceed to
  green in the same V-Bounce — or, for a defect that reproduces only in
  integration tests, cite the failed integration test run recorded before the
  V-Bounce as its red, run the best hermetic reproduction before the change,
  and leave the green to a passing covering run in the integration test
  cycle.

- **SPEC changes during a V-Bounce:** A material SPEC amendment immediately
  stops the current execution. That V-Bounce is closed with its mandatory MEM
  and manifest entry against the SPEC revision it actually used, normally with
  `execution_outcome: blocked` or `cancelled`. The canonical SPEC is then
  revised, appended to `spec_revisions[]`, and re-approved through
  `AITL-SPEC-Approval`. Execution resumes only as a **new V-Bounce** that
  references the newly approved revision. One V-Bounce never spans two SPEC
  revisions.

- **Risk class rubric** — the applicable Bolt reviewer assigns `risk_class` in
  the Bolt frontmatter during `AITL-BOLT-READY-Approval` as part of the Bolt's DoR.
  QA or Security may escalate it at any subsequent review. Once the first MEM
  is approved, it cannot be reduced unless the Bolt itself is formally
  re-reviewed and re-approved; historical V-Bounces retain the risk class that
  governed them. Every reassignment is appended to the Bolt frontmatter's
  `risk_history` array; it never rewrites the historical review evidence:

  **Consequences of an escalation:**
  - **No retroactive invalidation.** V-Bounces and MEMs already approved
    under a lower risk class remain valid history — they are immutable.
  - **Current SPEC:** if the escalation reveals new scope, constraints or
    risks not covered by the approved SPEC, the canonical SPEC must be
    revised and re-approved at `AITL-SPEC-Approval` before execution
    resumes (G15). A pure approver-level escalation (e.g. adding QA/Sec to
    the remaining checkpoints) does not require SPEC revision.
  - **Remaining checkpoints** use the new (higher) risk class — e.g. more
    approvers at `AITL-MEM-Approval` and at `AITL-BOLT-DONE-Approval`.
  - **No automatic Bolt re-approval** is triggered by escalation alone;
    the Bolt's DoR is unchanged. Only a *reduction* requires formal
    re-review of the Bolt.

  ```yaml
  risk_history:
    - previous_class: medium
      new_class: high
      decided_by:
        - actor: human:security.name
          role: security
          model: null
      reason: "Authentication boundary affected."
      decided_at: 2026-08-02T14:20:00-03:00
  ```

  | Risk | Examples | REV / AREV | Min approvers at AITL-MEM-Approval |
  |------|----------|------------|---------------------|
   | `low`      | Internal tooling, docs, low-blast-radius UI | stakeholder-triggered, optional | 1 (the executing Dev-validator) |
   | `medium`   | Standard feature in product surface         | stakeholder-triggered, optional | 1 (the executing Dev-validator) |
   | `high`     | Auth, payments, data migration, public API  | stakeholder-triggered, optional | 1 (the executing Dev-validator; incoming executor after a handoff) |
   | `critical` | Money movement, PII at scale, prod IaC      | stakeholder-triggered, optional | 1 (the executing Dev-validator; incoming executor after a handoff) |

  Risk informs the stakeholder's decision but never triggers REV or AREV
  automatically. If either mechanism is initiated, its complete approval
  rules apply regardless of risk class.

- **Autonomy levels (L1–L4)** — how much the agent decides on its own.
  `autonomy_level` is declared in the frontmatter of every SPEC revision;
  the manifest does not duplicate it.

  | Level | Name | Agent decides | Human is asked when |
  |-------|------|---------------|---------------------|
  | **L1** | *Suggest*    | Generates a bounded proposal or diff without applying it.     | At the end of the bounded run, or earlier on a stop condition. |
  | **L2** | *Bounded*    | Implementation details within a documented choice set.       | Choosing pattern / library / strategy. |
  | **L3** | *Autonomous* | Full implementation of the Spec, including library choice.   | Stuck, ambiguous AC, ADR-class change. |
  | **L4** | *Orchestrated* | Selects and sequences several already approved Bolts, while executing each through its own SPEC, V-Bounces, MEM and manifest. | Cross-Bolt dependency, ADR, schema change, security/perf trade-off. |

  Defaults by risk: `low → L3`, `medium → L3`, `high → L2`, `critical → L1`.
  L4 is reserved for sandboxed experiments and is **never** allowed without
  an explicit ADR approved through `AITL-ADR-Approval`.

- **Quality criteria:**
  - **First-pass approval** as a target (reduces rework).
  - Universal and conditional gates are selected **per level** (V-Bounce,
    integration test cycle, release) and evaluated exactly as defined in §3.6;
    no Bolt fabricates unit/integration test, SAST/DAST or performance
    evidence for an inapplicable artifact type, and no V-Bounce reports a gate
    of the integration test level as run.

### 3.3.1 BUG correction rules (strict TDD)

- A Functional Analyst, Developer, or QA may draft a `BUG-NNN`; its one
  dedicated Bolt may be drafted alongside it, but `AITL-BOLT-READY-Approval` is
  refused until `AITL-BUG-Approval` confirms its evidence, classification, and
  correct parent route.
- Every BUG has exactly one dedicated Bolt. Functional BUG Bolts are
  children of the affected feature US; non-functional BUG Bolts are
  children of `US-000-non-functional.md`.
- The BUG, Bolt, SPEC, MEM, and manifest reference each other. The BUG never
  authorizes code by itself.
- Before generating a BUG SPEC, the agent verifies `AITL-BUG-Approval`,
  `AITL-BOLT-READY-Approval`, the functional parent's `AITL-US-Approval` when
  applicable, and every approval required by the other governed sources used.
- Every BUG SPEC defines a strict TDD sequence in the same V-Bounce: create the
  reproduction test → run it and capture the expected red result → modify
  production code → run the targeted and applicable regression suites until
  green.
- **A defect that reproduces only in integration tests** keeps the same order
  with its red before the V-Bounce and its green in the Bolt's integration
  test cycle. Its Bolt is the integration-test-only BUG Bolt of §3.2.2
  (`test_integration: required`, an `applicable` integration test plan, a
  `test_integration_reason` beginning `integration-test-only BUG —`, never
  waived):
  - **red, before the V-Bounce:** an integration test run of the
    reproduction fails for the expected reason. It is recorded in the Bolt
    manifest's `integration_test_runs[]` (§3.12), its `reason` naming the BUG
    and the symptom (a run made before the Bolt existed is transcribed with
    its own `ran_at` and evidence), and it covers the commit the V-Bounce
    starts from — the V-Bounce's `git_commit`; if the V-Bounce must start
    from a later commit, the run is repeated there first. The SPEC cites the
    run when it exists at approval; otherwise the MEM cites it;
  - **in the V-Bounce:** the integration test that reproduces the defect is
    written, built and statically checked; the best hermetic reproduction is
    written and run before any production change, and its result recorded;
    then the production code is changed and the hermetic suite run to green;
  - **green, in the integration test cycle:** a passing covering run whose
    tests include the integration test that reproduces the defect (§3.2.3).

  The red still precedes every production change. The red run changes no
  Bolt status (§3.2.3), and it needs no classification: it covers no
  approved change of the Bolt, and its `reason` already names the defect.
  The covering run changes the status as for any Bolt. If the best hermetic
  reproduction does show the red, that red is recorded too; it does not
  replace the red run, and the order is unchanged. When a covering run fails
  and a human classifies it `code` for the same defect, that run is the red
  of the correction V-Bounce.
- Production code may not be changed before objective red evidence exists. If
  the failure cannot be reproduced as an automated test in the V-Bounce — or,
  for a defect that reproduces only in integration tests, no recorded
  integration test run is its red as above (failed for the expected reason,
  its `reason` naming the BUG, covering the commit the V-Bounce starts
  from) — the agent stops, creates the mandatory MEM and manifest
  `v_bounces[]` entry with the blocker outcome, and pauses for
  `AITL-MEM-Approval` without applying the fix. For that case a hermetic red
  alone does not let the V-Bounce proceed: it is recorded in that MEM, and
  the fix waits for the red run, or for a Bolt amendment (§3.2.2) if the
  defect does reproduce in the V-Bounce.
- The MEM records the red command/result and the green command/result
  separately; the manifest records the V-Bounce and its MEM reference. For a
  defect that reproduces only in integration tests, the MEM cites the red run
  (its number and evidence) and records the best hermetic reproduction's
  result; its green, the passing covering run, comes after
  `AITL-MEM-Approval` and is recorded in `integration_test_runs[]`. A BUG
  V-Bounce without its red and green evidence in the MEM — for that case,
  without the cited red run and the hermetic reproduction's result — cannot
  receive approved `AITL-MEM-Approval`.
- Human-requested changes create a new V-Bounce and a new MEM. The same strict
  red-to-green evidence is required for every correction V-Bounce; prior MEMs
  remain immutable history.

## 3.4 Interview-based inception rules (backlog engine)

- **Recorded interviews** (audio/video) with consent; agenda of goals and
  metrics.

- **Transcription + AI elaboration:** US, AC, architectural concerns, risks, **grouping into
  Units** and proposal of **initial Bolts**.

- **Human validation:** close ambiguities, fix metrics and prioritize.

- **Minimum output:** prioritized backlog, **Risk Register**, Units → Bolts
  map, and demo criteria for the first 1–2 weeks.

## 3.5 Design and decisions rules (ADRs)

- **Dual design:**
  - **Domain Design** (tactical DDD) for business models — modeling the
    domain, never specifying what the system does for the user: functional
    specifications live in the feature US and its ACs.
  - **Logical Design** (patterns / quality attributes / platform) for technical decisions.

- **1-page ADR** per relevant decision (context, alternatives, decision,
  consequences).

- **Mandatory approval:** Every ADR stops at `AITL-ADR-Approval`. An Architect
  or Tech Lead either approves the decision or requests changes. Until approved,
  the ADR remains a draft and has no governing authority.

- **Timing:** An ADR that constrains implementation must be prepared and
  approved before the SPEC is generated. If the need for a new architectural
  decision emerges during SPEC preparation or a V-Bounce, the work stops; the
  ADR is drafted and approved, then the canonical SPEC is revised and approved
  again before technical execution resumes.

- **Immutability:** Once approved, an ADR is **read-only**. The substantive
  content (context, decision, consequences) is never modified. If an approved
  decision is reversed, create a new ADR that supersedes the original; the old
  ADR stays as-is with status `superseded`. Drafts may be refined in response
  to `AITL-ADR-Approval` feedback before approval.

- **Conflicts between approved ADRs:** two active ADRs may not contradict
  each other.
  - *Detection (at creation):* before requesting `AITL-ADR-Approval`, the
    drafter checks the decision log for active ADRs whose decisions this
    ADR contradicts and records them in the `conflicts_with` frontmatter
    field. The approver (Architect or Tech Lead) verifies this check as
    part of the approval review.
  - *Resolution (the rule):* a conflict between approved ADRs is always
    resolved by a **new ADR that supersedes the contradicting one(s)**
    — the new ADR states the contradiction in its context, lists the
    superseded ADRs in `supersedes`, and the old ADRs are marked
    `superseded`. No AREV or REV is required by default; they remain
    available as stakeholder escalation for high/critical conflicts.
  - *Enforcement (at use):* the pre-SPEC evidence gate (§2.4.1) blocks any
    SPEC whose `sources` include two or more active ADRs whose decisions
    are mutually exclusive, until the conflict is resolved by a superseding
    ADR (§3.5).

- **Approval evidence:** The ADR records `status`, approver, role, timestamp,
  decision, and review evidence. When the ADR governs a SPEC revision, its
  repository-relative path appears in that revision's `sources`; the ADR
  remains the source of truth for its own approval.

```mermaid
flowchart TD
    T["Technical decision needed"] --> P["Agent proposes the ADR — the human requests it (G40)"]
    P --> D["Create or update draft ADR"]
    D --> HA{"AITL-ADR-Approval"}
    HA -->|"Changes requested"| D
    HA -->|"Approved"| A["ADR becomes governing and immutable"]
```

## 3.6 Quality Gates (minimum mandatory)

Gates are the **automated arm** of AITL: they reject obviously broken or
unsafe agent output before a human spends time on it. Quantitative gate
results use the CI evidence and MEM format; the manifest deliberately does not
duplicate them.

**Data classification (`data_classification`):** every Bolt and SPEC declares
one of `public` · `internal` · `confidential` · `restricted` — schema values,
never translated (§3.15) — in **increasing sensitivity order**:
`public < internal < confidential < restricted`. The SPEC's value mirrors
its Bolt's and may tighten it, never loosen it. The PII/DLP gate blocks any
data **above** the Spec-declared class from the prompt-submit boundary, and
`confidential` or higher is never sent to an external LLM (§1).

**Three verification levels.** Every gate runs at exactly one level.

- **V-Bounce (hermetic):** what the agent runs on its own, inside its loop,
  with a result that depends only on the code — no database, queue,
  third-party API or shared environment is reached. A `fail` here blocks
  `AITL-MEM-Approval`.
- **Integration test cycle (per Bolt):** what needs a running environment. It runs,
  for a Bolt whose `test_integration` is `required`, after `AITL-MEM-Approval`
  and before `AITL-BOLT-DONE-Approval`, against the Bolt's approved commit, run
  by CI, QA or whoever owns the environment. The tests it executes were written
  in a V-Bounce — this Bolt's or another Bolt's — or come from another suite
  (§3.2.2); those written in a V-Bounce were built and statically checked there
  and reviewed with its MEM.
- **Release (across Bolts):** aggregated above the per-Bolt loop.

A test that replaces a dependency with a test double is a unit, component or
consumer-side contract test and belongs to the V-Bounce; it is never reported
as an integration test.

| Gate | Level |
|------|-------|
| Universal: manifest validation, repository policies, secret scanning, the applicable-gate rule | V-Bounce |
| Build, lint and type checks | V-Bounce |
| Unit and component tests | V-Bounce |
| Integration tests (real dependencies) | Integration test cycle |
| Contract or E2E within the Bolt's scope | Split — consumer-side, against test doubles: V-Bounce; against the real providers: integration test cycle |
| SAST | V-Bounce |
| DAST | Integration test cycle |
| Dependency scanning and licenses | V-Bounce |
| SBOM of the built distributable | Release |
| OWASP Top 10 | Split — static review: V-Bounce; dynamic part (with DAST): integration test cycle |
| Perf-smoke | Integration test cycle |
| Logs, metrics and traces (backend/services) | Integration test cycle |
| Infrastructure / data / documentation / Test Bolts | The SPEC selects the validation at each level |
| Prompt-injection · secret-leak · hallucination lint · IP/license provenance · PII/DLP · dependency-confusion · test-first evidence · bolt-manifest validation | V-Bounce |
| Behavioral reproducibility | V-Bounce, over the hermetic suite |
| Mutation testing | Release |
| Cross-Bolt E2E / contract tests | Release |

**Universal gates (every Bolt — V-Bounce level):**

- the Bolt manifest exists and validates against its normative
  manifest-family JSON Schema (§3.12);
- repository policies, required reviews and applicable style/format rules are
  satisfied;
- secret scanning passes for every changed artifact;
- every gate selected as applicable by the approved SPEC ends, at its level,
  as `pass` or an explicitly approved `waived`; a non-applicable gate is
  recorded as `n/a` with its reason.

**Conditional classic gates (required when applicable to the change), by
level:**

- **In the V-Bounce:**
  - *Behavioral code:* build, lint and type checks; unit and component tests
    **green**; consumer-side contract tests against test doubles when the
    change crosses component boundaries **within this Bolt's scope**.
  - *Security-relevant/runtime code:* SAST.
  - *Dependencies:* dependency scanning and **licenses**, over manifests and
    lockfiles, when the Bolt adds or changes dependencies — a Bolt that
    changes dependencies keeps this check in its own V-Bounce.
  - *Externally reachable surface:* the **static part of the OWASP Top 10
    review** — the changed code's input validation, output encoding,
    authentication and authorization logic, injection sinks and secrets
    handling — when the change exposes or alters an interface reachable from
    outside the process — public endpoints, web UIs, authentication
    boundaries. Otherwise the gate is `n/a` with a reason in the SPEC: the
    Top 10 is a web-security catalogue, and a gate that is almost always
    `n/a` is noise, so the SPEC decides applicability per Bolt.
- **In the integration test cycle:**
  - *Behavioral code:* integration tests **green** against the real
    dependencies; **contract or E2E verification against the real
    providers** when the change crosses component boundaries within this
    Bolt's scope. A double-backed contract test verifies the consumer side
    only; it cannot detect drift from the real provider, which is why the
    gate is split between the two levels. This per-Bolt gate is distinct
    from the release-level regression suite below, and one never substitutes
    for the other.
  - *Externally reachable surface with an executable attack surface:*
    **DAST** and the dynamic part of the OWASP review.
  - *Performance-sensitive code:* **perf-smoke** with SPEC-defined p95
    and/or p99 thresholds for the affected critical endpoints or workloads.
  - *Backend/services:* the required logs, metrics and traces are verified
    as emitted by the running service (the instrumentation itself is
    written in the V-Bounce).
- **Infrastructure/data/documentation/Test Bolts:** the SPEC selects the
  meaningful validation for the artifact **at each level**; a Test Bolt's
  automation runs for real in the TC's run cycle, never in a per-Bolt
  integration test cycle (§3.2.2); irrelevant product-code gates are `n/a`,
  never fabricated.

**Conditional AI-native gates (required when the stated risk exists — V-Bounce
level):**

- **Prompt-injection scan:** detect untrusted input being concatenated into
  prompts or system messages.
- **Secret-leak scan:** block commits containing API keys, tokens,
  credentials — even in tests or fixtures.
- **Hallucination lint:** static check for imports / APIs / files / env vars
  the agent referenced but that do not exist.
- **IP / license provenance scan:** detect AI-generated code that matches
  public/open-source snippets under incompatible licenses. Use GitHub Copilot
  code referencing/public-code matching, Black Duck Snippet Analysis/Snippet
  API, or an equivalent provenance control.
- **PII / DLP gate:** the prompt-submit boundary scans for PII, secrets and
  data above the Spec-declared `data_classification`. Confidential data is
  never sent to an external LLM.
- **Dependency-confusion scan:** every agent-suggested dependency name is
  cross-checked against a known-good registry **before install**; unknown
  package names block the Bolt.
- **Test-first evidence:** required for functional, BUG-driven and other
  behavior-changing Bolts where a meaningful pre-change automated test exists.
  The MEM records the red/green or equivalent sequence — for a defect that
  reproduces only in integration tests, and only for it, the failed
  integration test run recorded before the V-Bounce is accepted as the red,
  beside the best hermetic reproduction's result (§3.3.1); otherwise the gate
  is `n/a` with a reason.
- **Behavioral reproducibility check** *(replaces the older “determinism
  check” — LLMs are not bit-deterministic)*: re-running the V-Bounce from
  the captured SPEC + exact model run(s) must
  produce a solution where
  (a) the original **hermetic suite** still passes, (b) the public contract is
  unchanged, (c) the approved ADRs are still satisfied. The integration test cycle
  is not part of this check: its result depends on an environment, not on the
  run.
- **Bolt-manifest validation:** the manifest in `devflow/metrics/bolts/` (§3.12) must
  be present, valid against the manifest schema (§3.12), and contain the applicable
  lifecycle decisions in `checkpoint_approvals[]`. Approval detail that belongs to
  an ADR, DISC, REV or other source artifact is validated at that artifact.

**Release-level gates (aggregated above the per-Bolt loop, NOT per Bolt):**

- **Mutation testing** (where the language allows) at release / milestone level
  — not at every Bolt. Mutation tooling is too slow to be a per-Bolt gate and a
  low per-Bolt score is noisy; aggregating above the inner loop gives a signal
  without crushing it.
- **End-to-end / contract tests** for **cross-Bolt** regressions introduced
  by AI output that looked locally correct. This release-level suite does not
  replace the per-Bolt conditional gate above: a Bolt whose change crosses
  component boundaries still runs its own contract verification — against
  doubles in its V-Bounce, against the real providers in its integration test
  cycle — and may not record it as `n/a` on the grounds that a later release
  suite will cover it.
- **SBOM of the built distributable** — produced for what is released, not
  per Bolt; a Bolt that changes dependencies is still scanned in its own
  V-Bounce (above).

**Rule:** Every applicable gate must finish as `pass` or `waived`. `fail`
blocks the checkpoint its level feeds: a V-Bounce-level `fail` blocks
`AITL-MEM-Approval` and merge; a `fail` at the integration test level
blocks `AITL-BOLT-DONE-Approval`; a release-level `fail` blocks the release. Any
`fail` blocks promotion. A waiver
requires an ADR approved through `AITL-ADR-Approval` that records the reason,
owner, compensating control and expiry date; the gate result remains
`waived`, not `pass`. `n/a` is valid only when the approved SPEC explains why
the gate does not apply.

## 3.7 Official metrics — DORA-first

Metrics are the **steering wheel** of the methodology. Avenga DevFlow uses
the current **five DORA software-delivery metrics** as the primary signal of delivery performance,
extended with **AI-native flow metrics** and **AITL governance metrics**.
Every weekly demo opens with these numbers.

### 3.7.1 DORA Five (primary — mandatory)

| # | DORA metric | Canonical definition used by DevFlow | Source of truth |
|---|-------------|--------------------------------------|-----------------|
| D1 | **Deployment Frequency** | Number of successful production deployments per defined reporting period | CI/CD production-deployment events |
| D2 | **Change Lead Time** | Time from a change being committed to version control until it is deployed to production | VCS commit timestamp + CI/CD production timestamp |
| D3 | **Failed Deployment Recovery Time** | Time required to recover from a production deployment failure that needs immediate intervention | Deployment event + incident/recovery event |
| D4 | **Change Fail Rate** | Percentage of production deployments that require immediate intervention because they caused a failure | CI/CD deployments joined to deployment-caused incidents, rollbacks or fixes |
| D5 | **Deployment Rework Rate** | Percentage of production deployments that are unplanned work performed to address a user-facing production issue | CI/CD deployments classified by planned delivery versus production rework |

Teams define internal baselines and improvement objectives from their own
context and review them in retrospectives. DevFlow does not label fixed
thresholds as universal DORA "Elite" targets and does not turn a DORA metric
into an individual performance target. **Bolt Lead Time** remains a separate
DevFlow flow metric; it begins at `AITL-BOLT-READY-Approval` and must not be reported
as DORA Change Lead Time.

**AI-aware analysis without corrupting DORA definitions:**

- Keep the five DORA metrics calculated at deployment/service level from the
  sources above.
- Join deployments to their included Bolts and manifest model runs only for
  diagnostic slicing. When a deployment contains several Bolts or models,
  classify it as a `model_mix`; never attribute the whole outcome to one model
  without an unambiguous causal link.
- Decompose **Bolt Lead Time**, not DORA Change Lead Time, into
  `ai_generation_minutes | human_review_minutes | wait_minutes` and keep
  deployment time in CI/CD telemetry.
- Pair deployment frequency with first-pass and rework signals to detect high
  cadence with low quality, without redefining the DORA metric.

### 3.7.2 AI-native flow metrics (secondary)

| Metric | Definition | Why we track it |
|--------|-----------|-----------------|
| **Lead Time per Bolt (decomposed)** | From `AITL-BOLT-READY-Approval` to `AITL-BOLT-DONE-Approval`; AI generation comes from the manifest and human review/wait from the manifest timing contract or workflow telemetry (§3.12) | Sizes Bolts realistically; reveals the actual bottleneck without being confused with DORA Change Lead Time |
| **Model runs per Bolt** | Count of `runs[]` across Bolt, SPEC, code and MEM generation in the manifest | Provider-independent proxy for AI orchestration complexity |
| **Weekly Throughput** | # of Bolts closed per week | Forecasting input |
| **% Commitment delivered** | Committed vs. closed Bolts | Planning health |
| **V-Bounces per Bolt** | Total V-Bounces required to obtain human approval for a Bolt | Measures first-pass quality and the human-requested rework needed to complete the Bolt |
| **SPEC first-review approval rate** | SPEC revisions whose first `AITL-SPEC-Approval` decision is `approved` / SPEC revisions reviewed | SPEC preparation quality |
| **V-Bounce first-review approval rate** | Bolts whose first V-Bounce receives approved `AITL-MEM-Approval` / Bolts with at least one reviewed V-Bounce | Implementation first-pass quality |
| **Rework Ratio** | Additional V-Bounces after the first human review / total completed Bolts | Real DoR quality signal |
| **Spec Drift** | Agent questions plus material SPEC revisions per Bolt | Inverse signal of SPEC completeness and repository grounding; gameable if you only count questions |
| **Manual Intervention Rate** | % of Bolts whose MEMs report direct human code changes | Honest signal of where the agent fails; not punished, measured |
| **Integration test first-pass rate** | Bolts whose first executed integration test run passes / Bolts with an `applicable` plan and at least one executed run — counting `pass` and `fail` runs only: `blocked` runs say nothing about the code and are excluded, a late waiver is not a run, and the red run of an integration-test-only BUG Bolt — recorded before its V-Bounce, so before any approved change of the Bolt exists — is excluded too (§3.3.1) | Environment and integration test health |
| **Time in integration testing** | From the latest approved `AITL-MEM-Approval` `decided_at` to the `ran_at` of the passing covering run (= `bolt.acceptance.review_ready_at`); Bolts that left the cycle by a late waiver are excluded and their time to waiver is shown apart | Shows the integration testing wait apart from the acceptance wait; a waiver never reads as a fast pass |
| **V-Bounces caused by integration test failures** | V-Bounces appended after a classification with cause `code`, `test` or `spec` in the Bolt's own change, per Bolt and in total | How much rework integration testing finds |
| **Bolts `done` without integration testing** | `done` Bolts split three ways: `not-required` (plan `n/a`), showing apart those not integration-tested by project policy (reason `project ADR-NNN: no integration test cycle`) and those whose SPEC predates the integration test plan; SPEC-time `waived` (`required` with a `waived` plan); late waived (`required`, integration testing waived by a human — with and without a compensating control) | Honest visibility of skips |

The four integration test metrics (§3.2.2) are computed from the recorded
facts — the manifests (§3.12) and each Bolt's confirmed `test_integration`;
the Status timeline (§3.2.3) is a cross-check, never their source. Acceptances
recorded in a joint act (§3.2.2) are shown apart in every acceptance-wait
figure, so a zero wait never reads as a fast separate review.

### 3.7.3 AITL governance metrics (mandatory)

These exist to **prove that human-by-default governance is real**, not nominal:

| Metric | Definition | Target |
|--------|-----------|--------|
| **AITL Coverage** | % of Bolts whose required named checkpoints (per Bolt type and applicable conditions, §3.0), including individual role-routed `AITL-BOLT-READY-Approval`, `AITL-BUG-Approval` for every BUG-driven Bolt, `AITL-US-Approval` only for functional Bolts, `AITL-TC-Approval` for every applicable verification contract and every Test Bolt parent, `AITL-SPEC-Approval` for the canonical SPEC version executed, `AITL-ADR-Approval` for every applicable ADR, `AITL-MEM-Approval` for every V-Bounce/MEM pair, and every conditional DISC/REV checkpoint linked to the Bolt, are all signed off **with review-quality evidence**. AREV governance is measured from AREV artifacts, never from the Bolt manifest. | **100%** per Bolt type and applicable condition |
| **BUG Governance Coverage** | % of BUGs with `AITL-BUG-Approval` recorded and exactly one correctly parented dedicated Bolt, plus red→green evidence for every executed BUG V-Bounce — for a defect that reproduces only in integration tests, the red run cited in its MEM and the passing covering run (§3.3.1) | **100% of approved BUGs and BUG V-Bounces** |
| **TC Governance Coverage** | % of TCs whose source US/ACs, source Bolt, test basis, and `AITL-TC-Approval` are complete before the TC governs a SPEC or its Test Bolts are approved | **100% of used TCs** |
| **QA Automation Bolt Coverage** | % of QA Automation code changes authorized by a correctly parented `TC-NNN.BOLT-NNN`, with exactly one approved source TC and the complete standard Bolt lifecycle | **100% of QA Automation changes** |
| **Research and Review Governance Coverage** | % of initiated DISC artifacts with `AITL-DISC-Approval`, initiated REV artifacts with `AITL-REV-Approval`, and initiated AREV artifacts with all three sequential phase approvals | **100% of initiated mechanisms** |
| **MEM Governance Coverage** | % of `v_bounces[]` entries that reference exactly one complete MEM and have a matching manifest `AITL-MEM-Approval`; review-quality detail remains in the MEM/checkpoint evidence | **100% of V-Bounces** |
| **Time-to-Human-Review** | Working time from the governed artifact's canonical `review_ready_at`—when that exact version is formally submitted and available for review—to `review.started_at` | < 4 h working time; ≥ 4 h → recorded as process defect for the retro; ≥ 8 h → escalate to artifact owner/lead; ≥ 24 h → escalate to PO/Tech Lead (§3.0) |
| **Approval-without-Comment Rate** | % of approved AITL decisions with empty `review.findings`; slice by checkpoint and compare `started_at`→`decided_at` with the applicable review budget in §3.0 where one exists (US, BUG, TC, ADR, DISC, REV and AREV budgets are project-defined) | Investigate if > 70% together with unusually short reviews — possible rubber-stamping |
| **Human Override Rate** | % of V-Bounces where the human changed/rejected the agent's output, **sliced by risk class** | Empirical bands (tune in 90 days): `low` 5–20% · `medium` 10–30% · `high` 15–40% · `critical` 20–50% |
| **Adversarial Review Adoption** | % of work items for which a stakeholder elected to initiate AREV, sliced by scope, trigger source, and risk | Monitor only — no mandated adoption rate |
| **Defect escape rate** | Defects that reached UAT / prod; deployment-caused production defects may feed D4 after causal classification | ↓ trend |
| **Gate Override Rate** | % of gate failures bypassed with an approved waiver ADR in CI/MEM evidence | Monitor — investigate if rising |
| **Escalation Rate** | % of Bolts escalated from L3→L2 or L2→L1 during execution | Monitor — signals DoR or Spec quality issues |

The governance-metrics collector joins checkpoint evidence from the governed
artifacts by canonical artifact ID and checkpoint, then joins Bolt lifecycle
decisions from the manifest family by Bolt ref, SPEC revision and V-Bounce
number. Queue and review times (Time-to-Human-Review, active review,
review duration) come from the manifest timing contract
(`review_ready_at` / `review_started_at` / `decided_at`, §3.12), with
workflow telemetry as fallback when a step timestamp is missing. DISC, REV,
AREV, ADR and other artifact-owned approvals are read from those
artifacts rather than inferred from their absence in the Bolt manifest.
Workflow telemetry may index the same IDs and timestamps, but it must never
manufacture an approval that is absent from the governed source artifact.

### 3.7.4 How metrics drive decisions

- **Weekly retro opens with DORA Five** + AITL Coverage. No DORA review → no
  retro.
- If **Commit is missed** → reduce Bolt size or improve DoR.
- If **Rework Ratio rises** → the bottleneck is DoR / Spec quality, not the
  agent.
- If **Human Override Rate drops below 10%** → risk of rubber-stamping;
  rotate reviewers and let stakeholders consider a targeted REV or AREV.
- If **D4 (Change Fail Rate) rises** → tighten gates and ask the relevant
  stakeholders whether targeted REV or AREV coverage would help before
  increasing throughput.
- If **Spec Drift is high** → invest in `analysis/` (better domain model and
  glossary).
- If **Bolt estimates drift ≥ 2× from actual active delivery** → recalibrate
  with the AI-native estimation rule (§2.4) and check for manual-effort
  anchoring; where story points are used, correlate them against actual
  aggregated Bolt Lead Time per US (§2.6).

## 3.8 Prioritization and service classes

DevFlow keeps three orthogonal taxonomies; none creates an additional Bolt
type:

| Dimension | Field | Values | Purpose |
|-----------|-------|--------|---------|
| Bolt type | `bolt_type` | `functional`, `non-functional`, `test` | Parentage, scope boundary and core AITL routing |
| Work category | `work_category` | `feature`, `refactor`, `infra`, `hardening`, `debt`, `qa_automation` | Reporting and Bolt-acceptance routing |
| Service class | `service_class` | `regulatory`, `incident_hotfix`, `feature_value`, `debt_hardening` | Priority and capacity policy |

A BUG-driven Bolt and a hotfix Bolt remain functional or non-functional; a
Test Bolt remains `test`. BUG and hotfix are conditions, not Bolt types.

**Mapping constraint:** every work category maps to exactly one Bolt type —
`feature` pairs only with `bolt_type: functional` (a feature US);
`refactor`, `infra`, `hardening`, and `debt` pair only with
`bolt_type: non-functional` (US-000); `qa_automation` pairs only with
`bolt_type: test` (an approved TC), and `bolt_type: test` always carries
`work_category: qa_automation`. No other combination is valid (§3.11
restates this mapping where it routes `AITL-BOLT-DONE-Approval`).

- **Rules:**
  - Regulatory / Incident take **immediate precedence**.
  - A Bolt retains its identity across all required V-Bounces; split
    it only when independent deliverables or approvals justify separate Bolts.
  - Debt / Hardening: reserve **10–20%** per week.

## 3.9 Minimum roles

- **PO / PM:** Defines Intent / value, prioritizes **Bolts**, accepts demos.

- **Functional Analyst:** Governs `analysis/`, creates and approves feature
  User Stories and functional Bolts, and approves functional BUGs before their
  Bolts are created. Has no mandatory role in creating or
  approving non-functional Bolts under US-000.

- **Architect / Tech Lead:** Governs ADRs and approves non-functional Bolts.
  May also create and refine non-functional Bolts directly and approves
  `critical`-severity non-functional BUGs (and their dedicated Bolt) before
  the Bolt is created.

- **Dev-validator:** Orchestrates **V-Bounces**, reviews and validates the
  outputs generated by the AI agent, and is accountable for
  `AITL-SPEC-Approval` together with applicable domain owners. Drafts or
  maintains ADRs for approval by the Architect or Tech Lead. As a Developer,
  may draft functional or non-functional BUGs, create and refine
  non-functional Bolts under US-000, and approve non-functional BUGs (and
  their dedicated Bolt) at any severity — any qualified team member, the BUG's
  own author included, may record the approval (G29: guidance, never a gate).
  Does not normally author production code
  by hand, but may apply a recorded manual fallback under the approved SPEC;
  the same tests, gates and AITL review still apply.

- **AI Orchestrator / TL:** Tooling, memory, prompts, policies; supervises
  vibe-coding practices, gates and traceability. Configures and maintains the
  AI agents.

- **QA / Security:** Defines and maintains *gates*, co-designs the testing
  strategy with the AI agent, drafts Test Cases from approved intent, and may
  draft functional or non-functional BUGs from observed failures and evidence.
  QA or a QA Automation Engineer may create Test Bolts from TCs; a QA
  Lead, QA Automation Lead, Architect, or Tech Lead approves them.

## 3.10 Anti-patterns (what we DO NOT do)

- **"Elephant" Bolts** with several independent outcomes, or vague ones
  ("improve performance" with no thresholds). Elapsed time alone does not
  make a Bolt an elephant.

- **V-Bounces without tests** (no objective brake).

- **Test Cases reverse-engineered from code:** current implementation behavior
  is copied into expected results instead of deriving them from approved
  intent, creating confirmation bias.

- **QA Automation without a Test Bolt:** test code is created inside an
  unrelated product Bolt, or one Test Bolt spans several source TCs.

- **Direct BUG fixes:** creating the BUG's Bolt before `AITL-BUG-Approval`,
  fixing it under an unrelated Bolt, or changing production code before a
  reproduction test — or, for a defect that reproduces only in integration
  tests, a recorded integration test run — has produced the expected red
  evidence.

- **"Ghost" design** (decisions without ADR).

- **"Ghost" SPEC approval:** starting a code-run from a draft, stale or
  unapproved SPEC, or treating Bolt approval as implicit SPEC approval.

- **Sprints as "buckets"** to hide delays (the meaningful number is
  **Bolts Done**, not "being busy").

- **Dirty memory:** unlinked artifacts; decisions or lessons not captured.

- **Unrecorded human coding:** a human patch is hidden, bypasses the approved
  SPEC or gates, or is presented as AI-generated. Recorded manual fallback is
  permitted and measured; ungoverned implementation is the anti-pattern.

## 3.11 Approval and acceptance levels (Policy)

**Goal:** Clarify what "Approval" means at each instance and who decides.
These map 1:1 to the AITL checkpoints defined in §3.0.

**Levels:**

1. **User Story Approval — `AITL-US-Approval`**
   - **Validates:** the US, its Acceptance Criteria, and its traceability to
     raw inputs and analysis artifacts.
   - **Who:** Functional Analyst. If the named role has no holder, the
     available qualified human records the approval, noting the self-assigned
     role (role routing is guidance, not a gate).
   - **Output:** US status = approved. Its Bolts' `AITL-BOLT-READY-Approval`
     is now unlocked (§3.0).
   - **Applicability:** Feature User Stories only. US-000 is a permanent
     container and is never submitted for approval.

2. **BUG Approval — `AITL-BUG-Approval`**
   - **Validates:** that the reported defect is real and sufficiently
     evidenced, expected and actual behavior are clear, and functional versus
     non-functional classification and future Bolt parent are correct.
   - **Who:** Functional Analyst for a functional BUG; for a non-functional
      BUG, Architect or Tech Lead when `severity: critical`, otherwise any team
      member, the BUG's own author included — the recommended approver is
      guidance, never a gate; the author may approve at any severity. A Developer or QA may create either as
      draft. The named-role routing above is guidance, not a gate: if a named
      role has no holder, the available qualified human records the approval,
      noting the self-assigned role.
   - **Output:** BUG status = approved. Its one dedicated Bolt's `AITL-BOLT-READY-Approval`
     is now unlocked (§2.16). This does not approve that Bolt or authorize implementation.

3. **Test Case Approval — `AITL-TC-Approval`**
   - **Validates:** that the TC was derived from its approved US/ACs and source
     Bolt, or from an approved non-functional Bolt and governing technical
     sources; that preconditions, data, steps, expected results, coverage, and
     evidence are complete; and that current code was not treated as the
     source of expected behavior.
   - **Who:** QA plus a Functional Analyst or delegated business-domain owner
     for functional expectations; QA plus the applicable Architect, Tech Lead,
     Security, Performance, Data, or other technical owner for non-functional
     expectations. If a named role has no holder, the available qualified
     human records the approval, noting the self-assigned role (role routing
     is guidance, not a gate).
   - **Output:** TC status = approved. It may govern verification; its Test Bolts
     (1..n) may now be approved. It does not approve automation code or any
     downstream Bolt.

4. **Bolt Readiness — `AITL-BOLT-READY-Approval`**
   - **Validates:** the scope, value, slicing, dependencies, risks, and demo
     criterion of one specific Bolt against its correct parent. For a
     functional Bolt this is the approved feature US; for a non-functional
     Bolt it is the permanent US-000 container plus the referenced governed
     technical source; for a Test Bolt it is exactly one approved TC; and the
     Bolt's `test_integration` value with its reason, which this approver
     confirms or changes (§3.2.2).
   - **Who:** Functional Analyst for a functional Bolt; Architect or Tech Lead
     for a non-functional Bolt (except: the dedicated Bolt of a non-functional
     BUG mirrors its parent BUG's severity-based routing — see §2.16); QA
     Lead, QA Automation Lead, Architect, or Tech Lead for a Test Bolt.
     Functional Analyst approval is not required on the non-functional or
     Test Bolt paths. If a named role has no holder, the available qualified
     human records the approval, noting the self-assigned role (role routing
     is guidance, not a gate).
   - **Output:** the Bolt's `status` becomes `ready` (§3.2.3). This authorizes
     SPEC preparation for that Bolt, but does not authorize technical
     execution by itself.

5. **ADR Approval — `AITL-ADR-Approval`**
   - **Validates:** the ADR context, considered alternatives, decision,
     consequences, impact on ADR-defined constraints, and source traceability.
   - **Who:** Architect or Tech Lead. If the named role has no holder, the
     available qualified human records the approval, noting the self-assigned
     role (role routing is guidance, not a gate).
   - **Output:** ADR status = accepted. The decision becomes governing and the
     ADR becomes immutable. A request for changes leaves it in draft status.

6. **SPEC Approval — `AITL-SPEC-Approval`**
   - **Validates:** that the single-Bolt SPEC was generated from a complete
     inventory of approved governed artifacts and relevant repository evidence;
     faithfully implements the Bolt, feature US/ACs when applicable, and ADRs;
     and defines a feasible, testable and safe execution plan without invented
     assumptions.
   - **Who:** Dev-validator plus the applicable domain owner(s): Functional
     Analyst/PO for functional fidelity, Architect/Tech Lead for architectural
     or non-functional scope, and QA/Security/Data specialists when required by
     risk or affected domain. If a named role has no holder, the available
     qualified human records the approval, noting the self-assigned role (role
     routing is guidance, not a gate).
   - **Output:** SPEC status = approved. Only then may the code-run or V-Bounce
     begin. A request for changes leaves it in draft; a material source or
     baseline change invalidates the approval and requires re-approval.

7. **Discovery Approval — `AITL-DISC-Approval`** *(conditional)*
   - **Validates:** the research question, evidence, assumptions, limits, and
     conclusions of an initiated Discovery.
   - **Who:** Qualified human designated for the research domain — **the author included**.
   - **Output:** DISC status = approved and its conclusions become governed
     inputs for analysis and backlog or architecture decisions. Downstream
     artifacts are not implicitly approved.

8. **Review Approval — `AITL-REV-Approval`** *(conditional)*
   - **Validates:** an initiated REV, its evidence, classification, and
     actionable findings.
   - **Who:** Qualified human designated for that Review — **the author included**.
   - **Output:** REV status = approved and findings become governed inputs.
     Downstream artifacts are not implicitly approved.

9. **AREV Critique Approval — `AITL-AREV-CRITIQUE-Approval`** *(conditional)*
   - **Validates:** rigor, relevance, coverage, and evidence of the Critique.
   - **Who:** Qualified human designated for that AREV.
   - **Output:** approved Critique; only then may Defense begin.

10. **AREV Defense Approval — `AITL-AREV-DEFENSE-Approval`** *(conditional)*
   - **Validates:** whether the Defense addresses the approved Critique
     completely and with evidence.
   - **Who:** Qualified human designated for that AREV.
   - **Output:** approved Defense; only then may Verdict begin.

11. **AREV Verdict Approval — `AITL-AREV-VERDICT-Approval`** *(conditional)*
   - **Validates:** impartial adjudication of the approved Critique and
     Defense, plus the clarity and actionability of the resulting findings.
   - **Who:** Qualified human designated for that AREV.
   - **Output:** approved AREV Verdict and governed actionable findings.
     Downstream artifacts are not implicitly approved.

   DISC, REV and AREV are optional or need-driven to initiate. These five
   conditional approvals become mandatory only after the corresponding
   mechanism is started.

12. **MEM and V-Bounce Approval — `AITL-MEM-Approval`**
   - **Validates:** completeness and accuracy of the mandatory MEM against the
     actual diff, code, tests, gate results, approved SPEC, applicable ADRs,
     manifest, and other execution evidence.
   - **Who:** the Dev-validator who executed the Bolt — one approver at any
     risk. QA, Security, Data, SRE, or other domain reviewers may be added as
     optional reviewers, never required by risk.
   - **Output:** the corresponding V-Bounce = approved (the MEM has no status,
     §2.12). If it is the latest V-Bounce, this is the **Development Completed**
     event and the Bolt moves on by §3.2.3; it does not imply business
     acceptance, Bolt `done`, or production promotion.
   - **Changes requested:** The MEM and matching manifest `v_bounces[]` entry remain as evidence
     of an unapproved V-Bounce. Any subsequent autonomous implementation is a
     new V-Bounce and must generate a new MEM with its own
     `AITL-MEM-Approval`.

13. **Bolt Acceptance (Value / outcome) — `AITL-BOLT-DONE-Approval`**
   - **Validates:** the Bolt's completion criterion is met: relevant US+AC for
     functional work, measurable technical evidence for non-functional work,
     or faithful automation of the approved parent TC for Test Bolts — and its
     integration test evidence: a passing covering run, the `not-required`
     reason, or the `waived` ADR with, on `high`/`critical`, the executed
     compensating control — or a late waiver shown as "integration testing
     waived — <reason>" with its compensating-control evidence or "no
     compensating control recorded".
   - **Who:** PO / PM for functional Bolts; routed technical owner for
     non-functional Bolts; QA Lead or QA Automation Lead for Test Bolts. If a
     named role has no holder, the available qualified human records the
     approval, noting the self-assigned role (role routing is guidance, not a gate).
   - **When:** only once the Bolt is `ready-for-acceptance`; during the weekly
     demo or asynchronously (comment on PR / ticket). It may be recorded
     jointly with `AITL-MEM-Approval` under the joint act of §3.2.2.
   - **Output:** `approved` → the Bolt is `done`. `changes_requested` → it
     stays `ready-for-acceptance` until the missing evidence is added or a new
     V-Bounce takes it back through the cycle. `rejected` → it becomes
     `deprecated` with the reason, which names who owns its merged change; if
     the outcome is still needed, a new Bolt is created (§3.2.3).

  **Post-acceptance defects (§2.9):** `AITL-BOLT-DONE-Approval` is a
  point-in-time decision and is **never revoked retroactively**. A defect
  discovered after acceptance follows the normal **BUG lifecycle** — create
  `BUG-NNN`, record `AITL-BUG-Approval`, create its dedicated Bolt (under
  the affected feature US or US-000), and fix through strict TDD in
  one V-Bounce (§2.16, §3.3.1). The accepting PO/PM or technical owner may
  *pause* downstream promotion pending the fix, but the accepted Bolt's
  history remains immutable.

> **Release, promotion and customer acceptance are not prescribed by DevFlow
> in this release.** The governed flow ends at Bolt acceptance
> (`AITL-BOLT-DONE-Approval`); grouping Bolts into a deployable unit,
> environment promotion and customer acceptance (UAT) follow the **adopting
> team's own process**. A redesigned Unit/UAT model — informed by real
> environment/promotion complexity — is planned for a future version.
> The Deployment-Unit concept (§2.11) and DORA (§3.7.1) still describe how
> deployments are packaged and measured.

**AITL-BOLT-DONE-Approval routing by work category** — not every Bolt has a business-visible demo.
The `work_category` declared by the Bolt routes the
`AITL-BOLT-DONE-Approval` approver; the manifest does not duplicate it:

| Work category | AITL-BOLT-DONE-Approval approver | Demo form |
|-----------|-------------|-----------|
| `feature`   | PO / PM           | Business demo |
| `refactor`  | Tech Lead         | Before/after diff + test parity |
| `infra`     | Tech Lead + SRE   | Deployment evidence + perf-smoke |
| `hardening` | Tech Lead + Sec   | Fixed control + regression test |
| `debt`      | Tech Lead         | Metric/maintainability improvement |
| `qa_automation` | QA Lead / QA Automation Lead | TC automated with execution evidence |

> **Availability (operability principle, §3.0):** these approvers are the
> recommended defaults. Where a paired or named role has no holder (e.g. no
> SRE or Security member), the available qualified human records the
> acceptance, noting the self-assigned role — the routing never blocks.

`feature` uses `bolt_type: functional` and belongs to a feature US. The
`refactor`, `infra`, `hardening`, and `debt` categories use
`bolt_type: non-functional` and belong to `US-000-non-functional.md`.
`qa_automation` uses `bolt_type: test` and belongs to an approved `TC-NNN`.
Work categories refine reporting and approval routing without changing the
three canonical Bolt types or their parents.

A `feature` Bolt without a PO sign-off is **not Done**, regardless of how
green the gates are.

## 3.12 Manifest family v5 (minimal traceability, timing and AI usage)

Every **User Story, Bolt, and Test Case** produces exactly one JSON manifest
in `devflow/metrics/`, created by the agent at the same moment the artifact
document is created and updated at every lifecycle step. Filenames mirror the
artifact document with `.md` replaced by `.json`:

- `metrics/bolts/US-NNN.BOLT-NNN-<description>.json` — functional and
  non-functional Bolts.
- `metrics/bolts/TC-NNN.BOLT-NNN-<description>.json` — Test Bolts.
- `metrics/user-stories/US-NNN-<description>.json` — feature User Stories
  (US-000 is a container and has none).
- `metrics/test-cases/TC-NNN-<description>.json` — Test Cases.

The manifest family **v5** is intentionally small. Each manifest records
only:

1. the artifact, its documentary sources and its AI-generation usage;
2. every material revision of the canonical SPEC (Bolt manifests), including
   its sources and AI-generation usage;
3. every V-Bounce, the SPEC revision executed, its code-generation usage,
   outcome and mandatory MEM (Bolt manifests);
4. the AITL decisions directly associated with that artifact's lifecycle;
5. the **timing of every step** — `created_at`, `review_ready_at`,
   `review_started_at`, `decided_at` — so lead times, queue times and review
   latencies are measurable for reports (§3.7, `reports/`);
6. the Bolt's **integration test facts** (Bolt manifests): the
   `test_integration` value confirmed at `AITL-BOLT-READY-Approval`, the
   integration test runs that verified the Bolt's commits, the human
   classification of each of those runs that failed, and a human's late
   waiver of pending integration testing — and, for an integration-test-only
   BUG Bolt, the run that is its red, which needs no classification (§3.3.1).

The manifest is a traceability, timing and AI-consumption record. It is not a
duplicate project-management system, test report, deployment log, DORA
database or cost ledger. For an integration test run it records the fact —
what was covered, where, the outcome, who and when — and points to the
evidence; the test results themselves stay in that evidence.

The normative machine-readable contracts are:

- `devflow/metrics/manifest-v5-bolt.schema.json` — Bolt manifests;
- `devflow/metrics/manifest-v5-us.schema.json` — User Story manifests;
- `devflow/metrics/manifest-v5-tc.schema.json` — Test Case manifests.

Every manifest must validate against its matching schema. The example below
is illustrative but valid against the Bolt contract.

**Schema v5 example (valid JSON):**

```json
{
  "schema_version": "5.0",
  "bolt": {
    "id": "US-012.BOLT-003",
    "type": "functional",
    "test_integration": "required",
    "ref": "devflow/functional/bolts/US-012.BOLT-003-invoice-download.md",
    "sources": [
      "devflow/functional/user-stories/US-012-invoices.md"
    ],
    "generation": {
      "created_at": "2026-08-02T10:15:00-03:00",
      "created_by": "human:eugenio.serrano",
      "runs": [
        {
          "tool": "opencode",
          "provider": "openrouter",
          "model": "anthropic/claude-sonnet-4.7",
          "tokens": {
            "input_uncached": 4900,
            "input_cached_read": 1200,
            "input_cache_write": 0,
            "output": 950
          },
          "agent": null
        }
      ],
      "duration_seconds": 38
    },
    "review_ready_at": "2026-08-02T10:20:00-03:00",
    "review_started_at": "2026-08-02T10:22:00-03:00",
    "acceptance": {
      "review_ready_at": "2026-08-02T12:15:00-03:00",
      "review_started_at": "2026-08-02T12:18:00-03:00"
    }
  },
  "spec_revisions": [
    {
      "revision": 1,
      "ref": "devflow/spec/SPEC-260802-1042-invoice-download.md",
      "sources": [
        "devflow/functional/bolts/US-012.BOLT-003-invoice-download.md",
        "devflow/functional/user-stories/US-012-invoices.md",
        "devflow/tests/test-cases/TC-027-invoice-download.md",
        "devflow/adrs/ADR-009-billing-storage.md"
      ],
      "git_commit": "7f6c9a2",
      "generation": {
        "created_at": "2026-08-02T10:42:00-03:00",
        "created_by": "human:eugenio.serrano",
        "runs": [
          {
            "tool": "codex",
            "provider": "openai",
            "model": "gpt-5.6",
            "tokens": {
              "input_uncached": 13200,
              "input_cached_read": 5200,
              "input_cache_write": 0,
              "output": 3200
            },
            "agent": "avenga-devflow"
          }
        ],
        "duration_seconds": 164
      },
      "review_ready_at": "2026-08-02T10:50:00-03:00",
      "review_started_at": "2026-08-02T10:55:00-03:00"
    },
    {
      "revision": 2,
      "ref": "devflow/spec/SPEC-260802-1042-invoice-download.md",
      "sources": [
        "devflow/functional/bolts/US-012.BOLT-003-invoice-download.md",
        "devflow/functional/user-stories/US-012-invoices.md",
        "devflow/tests/test-cases/TC-027-invoice-download.md",
        "devflow/adrs/ADR-009-billing-storage.md"
      ],
      "git_commit": "7f6c9a2",
      "generation": {
        "created_at": "2026-08-02T11:18:00-03:00",
        "created_by": "human:eugenio.serrano",
        "runs": [
          {
            "tool": "codex",
            "provider": "openai",
            "model": "gpt-5.6",
            "tokens": {
              "input_uncached": 6500,
              "input_cached_read": 3100,
              "input_cache_write": 0,
              "output": 1250
            },
            "agent": null
          }
        ],
        "duration_seconds": 71
      },
      "review_ready_at": "2026-08-02T11:25:00-03:00",
      "review_started_at": "2026-08-02T11:26:00-03:00"
    }
  ],
  "v_bounces": [
    {
      "number": 1,
      "spec_revision": 2,
      "git_commit": "7f6c9a2",
      "execution_outcome": "ready_for_review",
      "code_generation": {
        "created_at": "2026-08-02T11:35:00-03:00",
        "created_by": "human:dev1",
        "runs": [
          {
            "tool": "opencode",
            "provider": "anthropic",
            "model": "claude-sonnet-4.7",
            "tokens": {
              "input_uncached": 30800,
              "input_cached_read": 11800,
              "input_cache_write": 0,
              "output": 8900
            },
            "agent": "developer-agent"
          }
        ],
        "duration_seconds": 1220
      },
      "mem": {
        "ref": "devflow/memory/MEM-260802-1138-invoice-download.md",
        "generation": {
          "created_at": "2026-08-02T11:38:00-03:00",
          "created_by": "human:dev1",
          "runs": [
            {
              "tool": "opencode",
              "provider": "anthropic",
              "model": "claude-sonnet-4.7",
              "tokens": {
                "input_uncached": 6100,
                "input_cached_read": 2100,
                "input_cache_write": 0,
                "output": 1450
              },
              "agent": "developer-agent"
            }
          ],
          "duration_seconds": 42
        }
      },
      "review_ready_at": "2026-08-02T11:40:00-03:00",
      "review_started_at": "2026-08-02T12:05:00-03:00"
    }
  ],
  "checkpoint_approvals": [
    {
      "checkpoint": "AITL-US-Approval",
      "subject": {
        "ref": "devflow/functional/user-stories/US-012-invoices.md"
      },
      "decision": "approved",
      "decided_by": [
        {
          "actor": "human:fa.name",
          "role": "functional_analyst",
          "model": null
        }
      ],
      "decided_at": "2026-08-02T09:55:00-03:00",
      "comment": null
    },
    {
      "checkpoint": "AITL-BOLT-READY-Approval",
      "subject": {
        "ref": "devflow/functional/bolts/US-012.BOLT-003-invoice-download.md"
      },
      "decision": "approved",
      "decided_by": [
        {
          "actor": "human:fa.name",
          "role": "functional_analyst",
          "model": null
        }
      ],
      "written_by": [
        {
          "actor": "human:fa.name",
          "role": "functional_analyst",
          "model": null
        }
      ],
      "decided_at": "2026-08-02T10:25:00-03:00",
      "comment": null
    },
    {
      "checkpoint": "AITL-SPEC-Approval",
      "subject": {
        "ref": "devflow/spec/SPEC-260802-1042-invoice-download.md",
        "revision": 1
      },
      "decision": "changes_requested",
      "decided_by": [
        {
          "actor": "human:eugenio.serrano",
          "role": "dev_validator",
          "model": null
        },
        {
          "actor": "human:fa.name",
          "role": "functional_analyst",
          "model": null
        }
      ],
      "written_by": [
        {
          "actor": "human:eugenio.serrano",
          "role": "dev_validator",
          "model": null
        }
      ],
      "decided_at": "2026-08-02T11:05:00-03:00",
      "comment": "Add explicit concurrency handling."
    },
    {
      "checkpoint": "AITL-SPEC-Approval",
      "subject": {
        "ref": "devflow/spec/SPEC-260802-1042-invoice-download.md",
        "revision": 2
      },
      "decision": "approved",
      "decided_by": [
        {
          "actor": "human:eugenio.serrano",
          "role": "dev_validator",
          "model": null
        },
        {
          "actor": "human:fa.name",
          "role": "functional_analyst",
          "model": null
        }
      ],
      "written_by": [
        {
          "actor": "agent:avenga-devflow",
          "role": "coordinator",
          "model": "gpt-5.6"
        }
      ],
      "decided_at": "2026-08-02T11:32:00-03:00",
      "comment": null
    },
    {
      "checkpoint": "AITL-MEM-Approval",
      "subject": {
        "ref": "devflow/memory/MEM-260802-1138-invoice-download.md",
        "v_bounce": 1
      },
      "decision": "approved",
      "decided_by": [
        {
          "actor": "human:dev1",
          "role": "dev_validator",
          "model": null
        }
      ],
      "written_by": [
        {
          "actor": "agent:avenga-devflow",
          "role": "coordinator",
          "model": "claude-sonnet-4.7"
        }
      ],
      "decided_at": "2026-08-02T12:10:00-03:00",
      "comment": null
    },
    {
      "checkpoint": "AITL-BOLT-DONE-Approval",
      "subject": {
        "ref": "devflow/functional/bolts/US-012.BOLT-003-invoice-download.md"
      },
      "decision": "approved",
      "decided_by": [
        {
          "actor": "human:po.name",
          "role": "product_owner",
          "model": null
        }
      ],
      "written_by": [
        {
          "actor": "human:po.name",
          "role": "product_owner",
          "model": null
        }
      ],
      "decided_at": "2026-08-02T12:30:00-03:00",
      "comment": null
    }
  ],
  "integration_test_runs": [
    {
      "kind": "run",
      "number": 1,
      "covers": "9b1e4d7",
      "environment": "staging",
      "outcome": "fail",
      "reason": null,
      "evidence": "https://ci.example.com/invoices/runs/101",
      "ran_by": "human:dev1",
      "runner": "github-actions",
      "ran_at": "2026-08-02T12:12:00-03:00"
    },
    {
      "kind": "classification",
      "run": 1,
      "cause": "environment",
      "reason": "The staging database restarted mid-run; the failing checks never reached the change under test.",
      "classified_by": "human:dev1",
      "classified_at": "2026-08-02T12:13:00-03:00"
    },
    {
      "kind": "run",
      "number": 2,
      "covers": "9b1e4d7",
      "environment": "staging",
      "outcome": "pass",
      "reason": null,
      "evidence": "https://ci.example.com/invoices/runs/102",
      "ran_by": "human:dev1",
      "runner": "github-actions",
      "ran_at": "2026-08-02T12:15:00-03:00"
    }
  ]
}
```

The example shows a functional Bolt. The same structure applies to all three
canonical Bolt types:

- A functional Bolt lists its parent feature US among `bolt.sources`.
- A non-functional Bolt lists `US-000-non-functional.md` among
  `bolt.sources`.
- A Test Bolt lists exactly one parent `TC-NNN` among `bolt.sources`.
- A BUG-driven functional or non-functional Bolt additionally lists its
  approved `BUG-NNN` among `bolt.sources`; BUG-driven is not a fourth Bolt
  type.

Each Bolt type has a complete example manifest in `devflow/metrics/`
(`TEMPLATE-MANIFEST-BOLT*.json`), and between them they show every
integration test entry kind: the functional example, like this one, a
failed run, its classification and a passing run; the non-functional
example a failed run classified `environment` and then closed by a human's
late waiver; the Test Bolt example `"test_integration": "not-required"` and
no integration test entry. Between them they also show who wrote each
decision (`written_by`): the signer writing their own decision — the
readiness approvals among them — or the Coordinator, `agent:avenga-devflow`
with role `coordinator`, transcribing a decision dictated to it or returned
by an approver agent — the approved SPEC revisions and the MEM approvals;
the earliest decision, `AITL-US-Approval`, carries none: it was recorded
before the repository adopted the field.

`checkpoint_approvals[]` records the applicable lifecycle checkpoints for this Bolt:

- all origin approvals that apply: `AITL-US-Approval`,
  `AITL-BUG-Approval` and/or `AITL-TC-Approval`;
- `AITL-BOLT-READY-Approval`;
- one `AITL-SPEC-Approval` decision for every reviewed SPEC revision;
- one `AITL-MEM-Approval` decision for every reviewed V-Bounce/MEM;
- `AITL-BOLT-DONE-Approval` when the Bolt is finally accepted.

Approval records are historical decisions, not only successful approvals.
`decision` therefore accepts `approved`, `changes_requested` or `rejected`.
Every new decision is appended; an earlier decision is never overwritten.
The exact subject is disambiguated with `revision` for a SPEC or `v_bounce` for
a MEM. `decided_by` is an array because
some checkpoints require joint approval; every element records the **actor**
(`human:<user>` or, under AITL, `agent:<id>`), its **role**, and its **model**
(`null` for a human, the model id for an agent). A single-approver decision
simply has one element. Whether a decision is **virtual** is **derived** from an
`agent:` actor prefix in `decided_by[]` — it is **not** a stored field (there is
no `mode`; a stored derived state would violate §3.15/G39). The safe default —
no virtual approver configured — records only `human:<user>` actors (AITL,
§3.0).

**Who wrote the decision — `written_by`, optional.** Beside `decided_by`, an
entry may carry `written_by`, an array of the same `{actor, role, model}`
shape naming the actor that actually wrote the record: the signer writing
their own decision, or the agent that transcribed it — the Coordinator as
`agent:avenga-devflow` with role `coordinator` and its model (§3.0.1). It is
projected field for field from the document's `review.written_by` /
`acceptance_review.written_by` (§3.0). Writing is not signing: `written_by`
sits beside `decided_by`, never inside it; a writer signs only by also being
in `decided_by`; independence, self-approval and quorum read `decided_by`
only; and writing confers no approval. Absent or `[]` means "not
recorded" — what every earlier record states; no manifest is converted or
backfilled to add it (G36), and a writer is never inferred. The schema keeps
it optional. A decision recorded on or after the repository's adoption of
the version that carries it (an upgrade's §5.16 entry in the root
`CHANGELOG.md`; a fresh project's installation) must carry it — a rule for
the validator, never for the schema, and never applied to earlier records
(the class of rule in the schema evolution policy below).

**Lifecycle:**

| Moment | Manifest update |
|--------|-----------------|
| **Artifact creation (US, Bolt, TC)** | Create the JSON from the matching manifest template with `schema_version`, the artifact object (`us` / `bolt` / `tc` with `id`, `ref`, `sources`, `generation`, whose `created_at` is the creation instant, never `null`), `review_ready_at` and `review_started_at` `null` — for a Bolt, both fields of its `acceptance` object too — and the applicable arrays. For a Bolt, `checkpoint_approvals[]` starts with the **origin decisions that already exist** at this moment: `AITL-US-Approval` for a functional Bolt, `AITL-TC-Approval` for a Test Bolt, and nothing for a non-functional Bolt under US-000, which has no origin approval. A BUG-driven Bolt additionally carries `AITL-BUG-Approval` — BUG is a **condition**, not a fourth Bolt type (§2.4). A US manifest starts with `story_points: null`; a TC manifest starts with `verifies` (exactly one `source_bolt`, its `source_us` and the covered ACs — `covered_acs` is empty for a non-functional TC under US-000). A Bolt manifest is created without `bolt.test_integration`: the value the drafting agent proposes is not yet a fact, and the field is first written at `AITL-BOLT-READY-Approval`. |
| **Ready for review** | Set the artifact's `review_ready_at` when it enters the human review queue (§3.0). |
| **Review started** | Set the artifact's `review_started_at` when the human begins reviewing. |
| **Artifact review (US / TC / Bolt)** | Append the matching decision (`AITL-US-Approval` / `AITL-TC-Approval` / `AITL-BOLT-READY-Approval`). Only an approved artifact may advance; only an approved Bolt may enter SPEC generation. On US approval, set `story_points` to the confirmed value. On Bolt approval, set `bolt.test_integration` to the `test_integration` value the decision confirms, in the same act; an amendment's re-approval overwrites it. |
| **SPEC generation or material revision** | Append one `spec_revisions[]` entry with its exact sources, repository baseline, generation usage and `review_ready_at`. |
| **SPEC review** | Set the revision's `review_started_at`; append the matching `AITL-SPEC-Approval` decision. Only an approved revision may be used by a V-Bounce. |
| **V-Bounce ready for review, failure, blocker or cancellation** | Append one `v_bounces[]` entry, including its execution outcome, code-generation usage, exactly one MEM with its own generation usage, and the entry's `review_ready_at` (the MEM review timing lives at V-Bounce level). |
| **MEM review** | Set the V-Bounce's `review_started_at`; append the matching `AITL-MEM-Approval` decision. Changes requested cause a new V-Bounce; they never rewrite the previous entry. |
| **Integration test run, classification or waiver** | Append one `integration_test_runs[]` entry: a `run` when a run finishes (whoever runs it), a `classification` when a human classifies a failed run, a `waiver` when a human waives pending integration testing (§3.2.2). |
| **Child Bolt created (US/TC manifests)** | Append the child Bolt's `ref` to the US `bolts[]` or TC `test_bolts[]`. |
| **Bolt acceptance** | Set `bolt.acceptance.review_ready_at` in the same act as the Bolt's transition into `ready-for-acceptance` (§3.2.3) — never at an earlier status; then `bolt.acceptance.review_started_at` when the human begins; append `AITL-BOLT-DONE-Approval`, which is never requested before `ready-for-acceptance`. In the joint act (§3.2.2) the acceptance timing and `AITL-BOLT-DONE-Approval` take the time of the act that also records `AITL-MEM-Approval`. |

Every row above that records an act of the Bolt transition table (§3.2.3) is
written in the same act as the Bolt document's `status` change and its Status
timeline row; the manifest itself never holds the status.

**Rules:**

- `schema_version` is exactly `5.0` for this family — the `<major>.0` convention: the manifest family carries its own major, bumped only by a **breaking** schema change (the v4→v5 change that renamed `hitl_approvals[]` → `checkpoint_approvals[]` and moved identity to the actor grammar), so the family major may lead the methodology version.

  **Schema evolution policy — additive within the major.** A project runs
  under a single methodology version (§5.16) and its manifests follow it by
  **major**: `schema_version` is the `<major>.0` of the family that the
  repository's `devflow/VERSION` declares, and the installed
  `manifest-v5*.schema.json` set is the one that version ships.
  - **Within a major the schema changes only additively:** a new
    **optional** field (absent — or `null` where the field allows it — means
    "not recorded"), a new value in an enum, or an additive change to a
    `$defs` entry. Never removing or renaming a field, making a field
    required, tightening a pattern, type, format or enum, or changing the
    meaning of a recorded value. **Relaxing** a constraint passes the test
    but weakens a guarantee, so it happens only through its own decision.
  - **The test of "additive":** every manifest that was valid against the
    previous schema validates against the new one, unconverted. So no
    manifest is converted within a major and a repository still holds
    exactly one family: validators and dashboards read exactly one. A
    repository validates with the schemas it installed; an older validator
    rejects a newer optional field or enum value.
  - **A new enum value that names a governed concept** — a checkpoint code,
    a Bolt type, a decision outcome — is created by its own decision in the
    methodology and its guardrails; the schema follows it. A schema edit
    never creates a checkpoint.
  - **A constraint that recorded history cannot satisfy through a lossless
    conversion is never put in the schema**, in any major. It is enforced by
    the validator for records created on or after the repository's adoption
    of the version that carries it — the first case is `created_by`, the
    second a decision's `written_by`. Until a repository's validator
    implements such a rule, the methodology text, the Coordinators'
    instructions and the manifest examples carry it.
  - **Only a breaking change bumps the major.** The normative schema files
    are then versioned alongside, and **every existing manifest is converted
    forward as part of the version upgrade** (§5.16); a manifest declaring a
    `schema_version` from another family is an unfinished migration rather
    than legacy evidence. The methodology change that bumps the version is
    recorded in `CHANGELOG.md`.

  **Conversion is lossless or it does not happen.** Converting a manifest
  forward may only add the fields the new schema introduces — set to `null`
  where the value was never captured and the schema admits `null` — and apply
  whatever that version renamed or relocated. It may never overwrite a
  recorded value, drop a recorded field, or infer a value nobody observed:
  `null` states *"not recorded"*, which is what actually happened, whereas a
  plausible timestamp is a fabrication. `null` is never written where the
  schema refuses it: `generation.created_at` and
  `checkpoint_approvals[].decided_at` are required and admit no `null`. A
  migration that has to build one — a manifest level the old version lacked
  (§5.16) — reads it as §5.16 prescribes (`created_at` from the commit that
  added the document, `decided_at` from the document's review contract); a
  value the repository does not record makes the manifest unresolved, never
  `null`. What a manifest preserves is the evidence it carries, not the
  envelope around it. Where a future delta cannot be applied under that rule,
  the migration stops and reports the affected manifests instead of guessing
  (§5.16, G36).
- Every manifest validates against its own schema: `bolt` against
  `manifest-v5-bolt.schema.json`, US against `manifest-v5-us.schema.json`, TC
  against `manifest-v5-tc.schema.json`. An artifact without its manifest, or
  with an invalid one, does not exist (G33).
- `bolt.type` accepts exactly `functional`, `non-functional` or `test`.
- All timestamps use RFC 3339 (the ISO 8601 profile of `format: date-time`)
  **with seconds** and a zone designator — `Z` or an explicit UTC offset, e.g.
  `2026-08-02T11:45:00-03:00`. A document's `date:` frontmatter carries a time
  **only when the document records a point-in-time event** (an incident
  detection, a UAT session); every other artifact uses a plain `YYYY-MM-DD`,
  because what needs sub-day precision already has its own field in the review
  contract (§3.0). Dates are
  compared as instants: convert to UTC and compare — the methodology
  prescribes no tool or language for this. JSON Schema's
  `format: date-time` is an **annotation only**: any validator or report
  tooling that wants to enforce the format, the monotonic order below, or
  the derived durations must implement those checks itself with whatever
  stack it already uses. The methodology never requires a specific runtime
  outside the reports generator (`reports/`, which is a tooling convenience,
  not a methodology dependency).
- **Timing contract:** every step records its timestamp — `created_at`
  (generation), `review_ready_at`, `review_started_at`, `decided_at`
  (approval). All four are required; only `review_ready_at` and
  `review_started_at` are nullable — on the artifact object, on each SPEC
  revision and V-Bounce, and in the Bolt's `acceptance` object they are
  `null` until their step happens. `generation.created_at` and
  `checkpoint_approvals[].decided_at` are never `null`: a `generation` block
  is written only once its artifact exists and a decision entry only once
  the decision is recorded, so each carries its instant from its first
  write. The review timestamps and `decided_at` are copied from the
  artifact's own review contract (§3.0); every timestamp is written once, in
  order — never invented, never backdated.
  The Bolt's acceptance review follows the same contract and is projected
  to `bolt.acceptance.review_ready_at` / `review_started_at`, keeping the
  readiness pair (`bolt.review_ready_at` / `bolt.review_started_at`) intact
  for the two distinct review moments of the Bolt.
  A second acceptance round keeps the first submission in `bolt.acceptance`
  (entries are never rewritten); the later round is read from its own
  `AITL-BOLT-DONE-Approval` entry and the Bolt document's `acceptance_review`
  block, which holds the latest round.
  The timestamps of one lifecycle are monotonic:
  `created_at ≤ review_ready_at ≤ review_started_at ≤ decided_at`; a
  violation is a validation error enforced by tooling, since no JSON Schema
  can express ordering.
- All `ref` and `sources` values are repository-relative paths. `sources`
  contains only documents actually used to generate that artifact.
- `created_by` identifies the **accountable human** (`human:<user>`) — the
  same identity as the document's `author:` — never an agent and never a
  model name. It is prefix-mandatory in the actor grammar (§3.0). The schema
  still accepts `agent:<id>` there so that existing records stay valid; the
  human-only rule is enforced by the methodology and, for manifests created
  on or after the repository's adoption of this version (an upgrade's §5.16
  entry in the root `CHANGELOG.md`; a fresh project's installation), by the
  validator.
- Every generation block contains `runs[]`. One tool/model invocation produces
  one run; fallbacks, subagents or model changes add runs rather than combining
  unlike usage. The normal single-model case therefore has one element.
- In every run, `tool` identifies the development surface, `provider`
  identifies the billing route, `model` uses the exact identifier reported
  by that tool, and **`agent`** records, **bare**, the id of the agent that
  produced that run — e.g. `"avenga-devflow"` for the Coordinator,
  `"developer-agent"` for a spawned role agent — never the delegating or
  spawning agent's, however deep the delegation, and `null` for a run no agent
  performed.
  Tool/provider/model are needed because the same model may have different
  prices through different providers; `agent` attributes the run to the actor,
  not merely the model (two role agents may share a model).
- Token values are provider-reported values, never estimates.
  `input_uncached`, `input_cached_read`, `input_cache_write` and `output` are
  separate because providers may price them differently. These categories are
  mutually exclusive for cost calculation; total tokens and cost are derived
  and are not duplicated.
- If a tool does not report a token category or duration, that value is `null`.
  For fully manual authoring, `created_by` is populated and `runs` is empty.
- The generation block's `duration_seconds` measures active wall-clock
  generation time for the artifact across its runs. Human review and approval
  waiting time are excluded; the value is `null` when the tool cannot report
  it reliably.
- `git_commit` identifies the repository baseline used for that SPEC revision
  or V-Bounce. It is `null` only when the work has not yet been versioned.
- Material changes to the canonical SPEC append a sequential
  `spec_revisions[]` entry and require a new `AITL-SPEC-Approval`. Earlier
  revisions and their decisions remain immutable.
- Every V-Bounce references exactly one existing and approved `spec_revision`.
  Internal autonomous retries remain inside that V-Bounce and their usage is
  accumulated in `code_generation`.
- `execution_outcome` accepts `ready_for_review`, `failed`, `blocked` or
  `cancelled`. It describes execution before human review; it never claims the
  V-Bounce is approved or completed. Every V-Bounce, regardless of execution
  outcome, produces exactly one MEM and one manifest entry. Completion is
  derived only from the matching approved `AITL-MEM-Approval`.
- **The confirmed `test_integration` (`bolt.test_integration`, Bolt
  manifests).** The field is **optional**: `"required"`, `"not-required"` or
  `null`. It records a **fact** — the Bolt's `test_integration` as
  `AITL-BOLT-READY-Approval` confirmed it (§3.2.2) — never a status. It is
  written in the **same act** as the READY decision that confirms the value;
  until then it is absent, because the value the drafting agent proposes on a
  `draft` Bolt is not yet a fact. A Bolt amendment re-approved at
  `AITL-BOLT-READY-Approval` overwrites it in the same act: the manifest
  holds the value confirmed by the latest READY decision, while the earlier
  value stays in the Bolt's History and in the repository history, and each
  READY decision keeps its own `checkpoint_approvals[]` entry. The one other
  overwrite is a migration's (§5.16): when the human changes a value the
  migration proposed for confirmation (row I4), the changed value is
  recorded over the proposed one — a value the migration itself added, never
  one carried forward from history (G36). A value the migration reports
  unresolved (row I5) is not written until the human supplies it, so that
  first write overwrites nothing. `null`, like absence, means "not
  recorded"; no new act writes it.
  `test_integration_reason` is never copied: it is prose, and the Bolt
  document governs — a mismatch between `bolt.test_integration` and the
  Bolt's `test_integration` is a recording error, corrected by recording the
  confirmed value here, never by editing the Bolt to match the manifest.
  With the waiver entries below, it tells apart from the manifests alone the
  three ways a Bolt reaches `done` without integration testing (§3.7.2):
  `not-required`; `required` with neither a passing covering run nor a
  waiver (a SPEC-time `waived` plan); `required` with a waiver.
- **Integration test runs (`integration_test_runs[]`, Bolt manifests).** The
  field is **optional** and **append-only**: absent and `[]` both mean "no
  integration test fact recorded". Entries are never rewritten or removed; a
  correction is a new entry. Every entry is one of **three kinds, told apart
  by `kind`**:
  - a **`run`** — a fact about one integration test run: `number` (sequential
    per manifest from 1, counting run entries only, like `v_bounces[].number`
    — the identity a classification cites); `covers` (the commit `<sha>` or
    the range `<sha>..<sha>`, oldest first, that the run verified —
    abbreviated or full lowercase SHA); `environment` (the environment's name
    as the project calls it, e.g. `staging`); `outcome`; `reason` (one line —
    **required for `blocked`**, otherwise `null` or a line); `evidence` (where
    the results live: a CI run URL or a repository-relative path, never the
    results themselves); `ran_by` (the accountable actor that started or owns
    the run, in the actor grammar: `human:<user>` or `agent:<id>`); `runner`
    (the system that executed it, bare — e.g. `github-actions` — or `null`
    when a person ran it by hand); and `ran_at` (when the run finished, RFC
    3339 like every timestamp here);
  - a **`classification`** — a human's reading of a failed run: `run` (the
    `number` of the failed run it classifies, in this manifest); `cause`; a
    required one-line `reason`; `classified_by`; and `classified_at` (when
    the classification was recorded).
  - a **`waiver`** — a human's decision to waive pending integration testing
    late (§3.2.2, T23 in §3.2.3): `covers` (the commit or range of the latest
    approved change it waives, in the same form as a run's); `closes_runs`
    (the `number`s of the runs it closes — the `blocked` runs, or the failed
    runs whose classification left the Bolt in `integration-tests-blocked` —
    and `[]` when no run was ever executed); a required one-line `reason`;
    `compensating_control` (a reference to the evidence that a compensating
    control was executed — a CI run URL or a repository-relative path — or
    `null`, read "no compensating control recorded"; the key is always
    present, so an absence is a recorded fact, never an omission);
    `waived_by`; and `waived_at` (when the waiver was recorded).

  **Run outcomes** are facts, never states:
  - `pass` — the run executed every check it was meant to execute against
    the covered commit or range, and all passed;
  - `fail` — the run executed and at least one check failed; the cause is
    unknown until a classification records it;
  - `blocked` — the run could not execute or could not complete (the
    environment, its data or a dependency unavailable), so it says nothing
    about the code; the one-line `reason` is required.

  **Classification causes** (a `fail` read by a human):
  - `code` — the change under test is wrong;
  - `test` — the integration test is wrong (the code is right);
  - `spec` — the approved SPEC is wrong or incomplete, so code and test
    faithfully implement the wrong thing;
  - `environment` — the run executed, but the environment, its data or a
    dependency produced the failure, not the change. This differs from
    `blocked`: the run produced a failing verdict that the human attributes
    to the environment.

  **The classifier is always a human** (`human:<user>`) — the schema
  requires it. A classification is a recorded decision with an accountable
  person, not an approval: it adds no checkpoint. The executing
  Dev-validator is the recommended classifier —
  guidance, never a gate. A later classification of the same run is
  appended; both stay recorded.

  **The waiver is always a human's too** (`waived_by` is `human:<user>`) —
  the schema requires it; no agent records one, configured or not. It is a
  recorded decision, not an approval: it adds no checkpoint, and it changes
  neither the Bolt's `test_integration` nor the SPEC's plan. Its
  compensating control is recommended on a `high` or `critical` Bolt, never
  required (§3.2.2).

  **A batched run** covering several Bolts is recorded in each covered
  Bolt's manifest, with the same `covers`, and each Bolt records its own
  classification.

  What the schema cannot check is checked by tooling: a classification's
  `run` names a run entry of the same manifest, recorded before it, whose
  `outcome` is `fail`; a waiver's `closes_runs` name run entries of the same
  manifest recorded before it; run `number`s are sequential without gaps;
  entries are appended in time order (`ran_at` / `classified_at` /
  `waived_at` non-decreasing); and a waiver keeps T23's limits, read against
  the Bolt document: never from `integration-tests-failed`, never for an
  integration-test-only BUG Bolt (§3.2.3, §3.3.1).
- `v_bounces[]`, `spec_revisions[]`, `checkpoint_approvals[]` and
  `integration_test_runs[]` are append-only. Existing entries are never
  rewritten; corrections are represented by new revisions, V-Bounces,
  decisions or integration test entries.
- **The Bolt document governs its status; the manifests keep the facts.** A
  Bolt's `status` is declared in its document and changes only through the
  transition table of §3.2.3. The manifests keep every fact they record and
  carry no status field; a mismatch between the status and the facts is a
  recording error, corrected by recording the missing fact or by a correction
  row of the Bolt's Status timeline, never by editing history. The status
  never substitutes for an approval: a `done` without a recorded
  `AITL-BOLT-DONE-Approval` is not accepted (G20, G22).
- The US and TC progress states are derived, never stored, from the declared
  statuses of their child Bolts: a User Story progresses `draft` → `approved`
  (its `AITL-US-Approval`) → `in_progress` (a child Bolt at any status from
  `ready` to `done`) → `completed` (every child Bolt `done` or `deprecated`,
  with at least one `done`); a Test Case progresses `draft` → `approved` (its
  `AITL-TC-Approval`) → `automated` (any of its Test Bolts `done`). The MEM's
  review state is derived from its `AITL-MEM-Approval` (§2.12).
- Source documents such as ADRs, DISCs, REVs or TCs keep their own approval
  evidence. When used, their paths appear in `sources`; their internal content
  is not copied into the manifest.

The following remain deliberately outside manifest v5:

- test and gate results, TDD evidence and modified-file lists, which belong in
  the MEM and CI evidence — an integration test run's outcome and the
  reference to its evidence are recorded in `integration_test_runs[]`; the
  results stay in the evidence;
- risk, autonomy, data classification and duplicated functional metadata;
- DORA metrics, deployment, promotion, UAT and incident data;
- pre-calculated monetary cost;
- AREV phases, selected agents/models and approvals;
- PR lists and any state already derivable from the artifacts and AITL
  decisions.

## 3.13 Manual agent/model selection for AREV

DevFlow does not automate agent or model switching during an AREV. The human
orchestrating the review selects the agent/model directly in the development
tool used by the team.

- The human launches the Critique with the selected Challenger agent/model.
- After `AITL-AREV-CRITIQUE-Approval`, the human manually changes the
  tool's agent/model before launching the Defense.
- After `AITL-AREV-DEFENSE-Approval`, the human manually changes it again
  before launching the Verdict.
- Each AREV phase records its own agent/model in that phase artifact.
- This operational selection does not create a regression-eval Bolt, does not
  require an ADR merely to switch phases, and does not update the Bolt
  manifest.

**Judge neutrality and the three-model requirement.** The Verdict's model must
differ from **both** the implementor's and the Challenger's: a Judge that
shares either one is not arbitrating, it is repeating. Running an AREV
therefore **requires at least three models** — one each for the Critique, the
Defense and the Verdict — so the Judge is always a genuine third model. A
single operator running three models is valid: the operator approves the three
AREV documents at their human checkpoints but does **not** arbitrate the
Verdict; the third model does. There is **no human-arbiter fallback**: a team
without a third model does not run the AREV (it is optional, §2.15), and an
AREV already initiated that cannot reach a neutral Verdict is set `cancelled`
(§3.15). A Judge sharing the implementor's or the Challenger's model is never a
valid Verdict.

Agent definitions for the supported development tools are installed from the
methodology distribution (§5.2), each at the location its own tool expects.
Governance of the manifest schema and any future organization-wide
model-baseline policy are intentionally handled separately from the AREV
phase-switching rule.

## 3.14 Developer development plan (skill atrophy mitigation)

If the agent writes everything, juniors never get the practice that turns
them into the seniors who can review the agent. This is a 2–5 year SDLC
risk; we manage it explicitly. **This plan is descriptive only**: it creates
no artifacts, records no evidence in `devflow/` documents, and uses none of
the governed ones.

- **Rotation:** every dev-validator rotates through Spec-author,
  Dev-validator and AI-Orchestrator roles at least once per quarter.
- **AI-review training:** onboarding includes a 1-day module on common LLM
  failure modes (hallucinated APIs, plausible-but-wrong logic, security
  smells the model misses).
- **Quarterly skill review:** team composition and skill gaps are reviewed
  quarterly; this drives hiring and training.

## 3.15 Language policy (schema in English, content in the project's language)

Avenga DevFlow artifacts are read by **two audiences**: humans (often
working in their local language) and the **AI agent + automated tooling**
(grep, CI gates, manifest validators, dashboards). To serve both without
friction we apply a **bilingual contract**: the *skeleton* of every artifact
stays in English; the *prose* follows the project's chosen language.

**Always in English (the schema — machine-facing):**

- YAML frontmatter **keys** (`status`, `priority`, `owner`, `targets`,
  `sources`, `closed_on`, `risk_class`, `autonomy_level`, …).
- **Enumerated values** of those keys (`low | medium | high | critical`,
  `L1 | L2 | L3 | L4`, `pass | fail | waived | n/a`, …). The per-family
  `status` vocabulary is normative and listed in full at the end of this
  section.
- **IDs** (`OQ-NNN`, `ADR-NNN`, `RISK-NNN`, `US-NNN`, `US-NNN.BOLT-NNN`,
  `TC-NNN`, `TC-NNN.BOLT-NNN`, `SPEC-YYMMDD-HHmm`, `MEM-YYMMDD-HHmm`,
  `INT-NNN`, `UAT-NNN`, …). IDs and prefixes are **never translated or
  renamed** — `US-001` stays `US-001` in every language.
- **Section headings of templates** when those headings act as anchors the
  agent or the gates look up (`## Resolution`, `## Investigation log`,
  `## History`, `## Sources`, `## Open questions`, …) — except in the
  localized families below, where headings follow the project's language
  and the agent matches anchors semantically. Template section numbers
  (`## 3. History`) are cosmetic ordering only: matching is by heading
  keyword, never by number, so inserting a section never breaks anchors.
  `AITL-*-Approval` checkpoint codes are **never translated**, even inside
  a localized heading.
- **Tags, labels and folder names.**
- **Bolt manifest field names and string enums** (§3.12).
- **Commit messages, branch names and PR titles.**

**In the project's language (the content — human-facing):**

- The narrative inside each section (description, context, decision
  rationale, investigation notes, justifications, comments).
- The manifest's optional AITL `comment` field.
- Interview transcripts and minutes (kept in the language they were
  recorded in).
- Demo scripts and UAT scenarios shown to business stakeholders.
- **File names (the descriptive part).** Every generated artifact filename
  keeps its English ID/prefix and localizes only the `<description>` slug:
  `US-001-procesamiento-de-pagos.md`, not `HU-001-…` and not
  `US-001-payment-processing.md` in a Spanish project. Slugs are always
  **kebab-case ASCII** — lowercase `a–z`, digits and hyphens, no accents,
  no `ñ`, in any language — so filenames stay stable across filesystems,
  tools and search.
- **Section headings in the localized families.** In `analysis/` (all its
  subfolders), feature User Stories and Test Cases, the `##` headings
  follow the project's language (`## 6. Resolución`, `## 7. Historial`);
  `AITL-*-Approval` codes inside those headings stay verbatim. In every
  other artifact family (Bolt, SPEC, MEM, ADR, BUG, DISC, REV, AREV, INC,
  RISK, RETRO, UAT), headings stay in English.
- **ADR titles** — like the body, they follow the project's language
  (`ADR-006-estrategia-de-logging.md`); the `ADR-NNN` ID stays English.

**Rules:**

- A project declares its content language **once**, in the `devflow/LANGUAGE`
  file (like `VERSION`; e.g. `en`, `es`). The AI agent reads it and writes
  content in that language for every artifact it generates. The value is
  updated as soon as the language of the project's raw inputs is detected.
- **Never translate the schema.** A `status: abierta` is a bug — the gates,
  the INDEX counters and the agent's queries all expect `status: open`.
- **Never mix languages inside the same prose field.** Either Spanish or
  English per section; do not interleave.
- Templates ship **with English schema, English placeholder prose and English
  headings**. When a project adopts them in another language, the team translates the placeholder prose and — in the
  localized families — the headings, never the schema.
- If a stakeholder name, a regulation, a product name or a domain term has
  no good translation, keep it in the original language and add it to
  `analysis/glossary/` once.

**Encoding — the bytes of every text file (normative):** every text file
under `devflow/` — Markdown, YAML, JSON, HTML and any other file that is not
binary, `_archive/` and `agents-data/` included — is:

- **UTF-8** — never UTF-16 and never a legacy code page such as
  Windows-1252 or ISO-8859-1;
- **without a byte order mark** — the bytes `EF BB BF` appear neither at the
  start of the file nor anywhere inside it;
- **broken into lines by `LF` or `CRLF`, never by a lone `CR`** — every
  carriage return (`0x0D`) is immediately followed by a line feed (`0x0A`);
  one file may mix `LF` and `CRLF` lines;
- **correctly decoded** — it holds the characters its author meant: no
  double-encoded text (*mojibake*: UTF-8 bytes read as Windows-1252 and
  written back, so that `é`, U+00E9, turns into the two characters U+00C3
  U+00A9) and no U+FFFD replacement character (the mark a lossy decode
  leaves where a character was lost).

The one exception is **raw evidence in `input/`**: a human deposits it
exactly as received (§5.6), so it may arrive in any encoding — a CSV
exported in Windows-1252, a UTF-16 log — and agents read it as it is and
never re-encode it (G31). The rule binds what is written into `devflow/`,
never what is received.

Every one of these defects is silent. A BOM moves a document's opening
`---` off the first byte, so its frontmatter no longer parses and the
document drops out of every INDEX, count and validator; git never
normalizes a lone `CR`, and line-based tools read the lines it joins as
one; mojibake and U+FFFD corrupt the text itself. A BOM or a lone `CR`
can be removed byte-exactly, only within the work mandated (G40) — never a
license to clean other files (approved ADRs and MEMs, `_archive/`,
`input/`). Mojibake, an invalid byte or a U+FFFD has lost the original
characters: an agent reports it — the file, the line, what it found — for
a human who has the source, and never guesses a repair. The usual cause is
a tool whose default is not UTF-8 without BOM — for example Windows
PowerShell 5.1's `Out-File` (UTF-16) and `Set-Content -Encoding UTF8`
(UTF-8 with a BOM) — so an agent writes text through a method that
produces UTF-8 without BOM (W23).

**Document `status` vocabulary (normative):** every artifact family declares
its lifecycle through a `status` frontmatter key. The table below is the
complete set: a family never uses a value outside its row, and a new value is
added here **before** it appears in a template, a folder README or an INDEX
(G39). The values are schema — never translated. Templates write them
pipe-separated in the frontmatter comment; they are listed here with `·`
only for readability.

| Artifact family | `status` values | Notes |
|-----------------|-----------------|-------|
| `analysis/` — business-context, business-risks (`BR`), glossary, personas, user-journeys, domain-model (entities, enumerations, relationships), ui, introduction | `draft` · `stable` · `deprecated` | `stable`, not `approved`: these artifacts carry no AITL checkpoint, so nothing ever approves them (§3.0) |
| `analysis/vision/`, `analysis/scope/` | `draft` · `stable` · `superseded` | The two families replaced as a whole by a numbered successor; they alone in `analysis/` also carry `version` (§5.7) |
| `analysis/process/` (`PROC`) | `draft` · `active` · `deprecated` | `active` instead of `stable`: a process describes a current or target business flow, not a reviewed statement |
| `analysis/open-questions/` (`OQ`) | `open` · `in-validation` · `answered` · `deferred` · `dropped` | `answered` requires propagation to the target artifact; `deferred` requires a revisit trigger; `dropped` requires a reason (§5.7) |
| Feature User Story | `draft` · `approved` · `deprecated` | `approved` means `AITL-US-Approval` is recorded (§2.6) |
| `US-000-non-functional.md` | `active` (fixed) | Permanent container with no approval lifecycle: never `draft`, never `approved`, never `deprecated`. A validator reading US statuses must special-case it (§2.6, §3.2) |
| Bolt — functional, non-functional, test | `draft` · `ready` · `specified` · `in-development` · `blocked` · `in-integration-testing` · `integration-tests-failed` · `integration-tests-blocked` · `ready-for-acceptance` · `done` · `deprecated` | Declared in the Bolt document, one value at a time, and changed only through a transition of the Bolt transition table, recorded in the same act in the document's Status timeline (§3.2.3). `draft` until `AITL-BOLT-READY-Approval` (§2.4). **Development Completed** and **Integration Tests Passed** are events, never values. Labels and meanings: §3.2.3 |
| SPEC | `draft` · `approved` · `blocked` · `obsolete` | `blocked`: approved and correct but unexecutable because of an external dependency — it resumes unchanged once the blocker clears. `obsolete`: the work itself was cancelled, so no successor document exists |
| MEM | *(none — deliberately)* | The MEM has no mutable status: its review state is derived from the associated `AITL-MEM-Approval` (§2.12) |
| Agent-run record (`ARUN`) | *(none — deliberately)* | A run record stores no state: a run is closed once its record's *Outcome* section is written — derived from the record, never declared. It has no AITL checkpoint and no manifest (§2.18, §5.12) |
| Test Case (`TC`) | `draft` · `approved` · `deprecated` | `approved` means `AITL-TC-Approval` is recorded (§2.6.1) |
| UAT | `draft` · `approved` · `approved-with-observations` · `rejected` | **Dormant/reserved (v4.2):** the UAT layer was removed from the active flow and the family is kept for a redesigned model planned in a future version (§4.7). `approved-with-observations` is a document status only; it never appears in the `review.decision` enum |
| ADR | `draft` · `accepted` · `rejected` · `deprecated` · `superseded` | `accepted`, not `approved` (§3.11). Only `accepted` governs; the other four never do (§2.8, §3.5) |
| BUG | `draft` · `approved` · `in-fix` · `fixed` · `closed` | `approved` means `AITL-BUG-Approval` is recorded; its one dedicated Bolt may now be approved (§2.16) |
| DISC | `draft` · `approved` · `deprecated` | `approved` means `AITL-DISC-Approval` is recorded (§2.13) |
| REV | `draft` · `approved` · `closed` | `closed` only once every approved finding has been routed to its own artifact (§2.14) |
| AREV (`AREV-NNN` index) | `draft` · `in-progress` · `active` · `closed` · `cancelled` | `cancelled` is the terminal state for an initiated AREV that cannot proceed (e.g. fewer than three models available and no neutral Verdict). Each phase additionally carries its own state inside the index: `pending` · `in-review` · `approved` · `changes_requested` (§2.15) |
| INC | `open` · `mitigated` · `closed` | |
| RISK | `open` · `mitigated` · `materialized` · `closed` | |
| RETRO | `draft` · `final` | |
| Sprint record (`SPRINT`) | `planned` · `active` · `closed` · `cancelled` | Declared in the sprint record and never derived from its dates: the dates are the plan, the `status` is the declared state, so a sprint whose end date has passed stays `active` until its `status` is changed. `closed` and `cancelled` are final. A sprint record has no AITL checkpoint and no manifest (§5.12) |

`status` is a **document** state and never substitutes for a AITL decision.
The review contract's `review.decision` enum — `approved`,
`changes_requested`, `rejected` (§3.0) — is universal across every approvable
artifact and is unaffected by the table above. Likewise, states the
methodology **derives** rather than stores — the MEM review state, the US/TC
progress states of §3.12, the Bolt's two events (§3.2.3) and an agent run's
closure (§2.18) — never appear as a `status` value.

**Why this matters:** the AI agent searches by `status: open`, gates check
machine-facing enum values, the INDEX of OQs aggregates by enum value, and
manifest validators expect exact names such as `schema_version`, `provider`,
`model`, `execution_outcome` and `approved`. Localizing those values breaks the
machinery silently. Localizing the prose, the filename slugs and the localized
family headings does not.

## 3.16 Defects in DevFlow itself

DevFlow's own material can be wrong: this methodology, `GUARDRAILS.md`, a
schema, a template or README, a shipped agent definition, a shipped skill
or, once one ships, a tool. A **DevFlow defect** is such material that is wrong,
contradicts another kit source, or cannot be executed as written. A problem
in the project's own artifacts is not one — it follows the project's flow
(a `BUG-NNN`, §2.16) — and neither is a preference ("I would have written
it differently").

- **Detected by cross-checking:** a tool's result against the kit text (the
  source of truth), an impossible result, two kit sources that contradict
  each other, a tool's own error. When the agent cannot tell, it says so
  ("either the tool has a bug, or the file was edited by hand into a shape
  it does not expect").
- **Said in three beats:** what the tool or text said; why it is a DevFlow
  defect and not the project's; that the DevFlow author should be told.
- **Never swallowed, never patched locally.** The agent works around the
  defect visibly when doing so breaks no rule; when the only way forward
  would break a guardrail or skip a checkpoint, it stops and names the
  blocked step. Kit text is never edited in the project to fix it — the next
  upgrade overwrites the patch, and until then the project drifts from the
  version it declares (§5.16).
- **The report is written for the author** and is self-contained: file and
  line, the exact quote, expected vs found, the rule, checkpoint or example
  it breaks, `devflow/VERSION`, the agent and model that met it (actor
  grammar, §3.0), the operating system.
- **The user sends; the agent only asks.** The agent asks the user to send
  its report to the channel named in `devflow/README.md`, with each file in
  question — named, with why the author needs it — and the exact tool call
  that exposes the error with its output, and it flags first any secret or
  personal data it sees in them. The agent never sends, and never tries to
  send, anything. Whether to send is the user's decision; the work
  continues either way.
- **Duplicates and the local record.** When the agent can see the channel's
  history it checks for a duplicate first. While the fix is pending, a line
  in the project's own record is enough — a `RISK-NNN` or a retrospective
  note, proposed and created on a human's request (G40). The fix arrives
  with a later version, through the upgrade (§5.16).

The report is a message in the conversation, not a governed artifact: it
carries no approval and no manifest, and no guardrail enforces this rule.

---

# 4. End-to-end process (from raw input to production)

This section describes **how we work from start to finish**: what comes in,
what goes out, who is involved and which artifacts remain. The backbone is
**raw input → iterative analysis → approved User Story → individually approved
product Bolts → independently designed and approved Test Cases when applicable
→ Test Bolts when QA Automation is selected → repository-grounded SPEC → `AITL-SPEC-Approval` → V-Bounces executed by
AI agents → mandatory MEM per V-Bounce → `AITL-MEM-Approval` → demonstrable
delivery**.

This section is an explanatory walkthrough. Section §2 governs concepts,
definitions, taxonomies and artifact contracts; §3 governs lifecycle, AITL,
gates, autonomy and metrics; and §5 governs structure, locations and names. If
a summary here appears to diverge, the section responsible for that dimension
governs and this walkthrough must be corrected. Normative keywords such as
*must*, *never* and *exactly one* are not independently redefined here.

## 4.1 Input-driven inception and functional analysis (backlog engine)

**Goal:** Turn the raw evidence in `devflow/input/` into analyzed, traceable,
and human-approved functional artifacts.

**Input**

- Every applicable raw source in `devflow/input/`: business material,
  databases, third-party documentation, interviews, legacy source code, and
  UI/UX evidence.
- Business goals, constraints, success metrics, and privacy or consent
  policies contained in or derived from those sources.

**Steps**

1. **Preserve raw evidence:** Store source material in the appropriate
   `input/` folder without silently normalizing or overwriting it.
2. **Process and analyze:** AI agents assist Functional Analysts in updating
   every affected artifact in `analysis/`, including vision, business context,
   risks, scope, personas, journeys, glossary, domain model, UI surfaces and
   patterns, processes, and open questions — each open question written for the person who answers
   it (§5.7). Once the artifacts for a feature exist at least in draft,
   a plain-language introduction narrative may be derived in
   `analysis/introduction/` as the last analysis artifact (derivative,
   non-governed — §5.5).
3. **Resolve material unknowns through Discovery when needed:** Within a
   mandated analysis pass, before creating or materially refining a User
   Story or Bolt, **propose** a `DISC-NNN` — created once the human confirms
   (G40) — when the team must investigate an external API, unfamiliar library, legacy
   behavior, integration constraint, technology option, or another material
   unknown. Its conclusions remain draft until `AITL-DISC-Approval`.
   Approved conclusions update the affected analysis artifacts; this research
   loop is not a V-Bounce.
4. **Classify the primary outcome:** A need for new business behavior follows
   the feature-US path. A need for a technical outcome follows the permanent
   `US-000-non-functional.md` path. The classification concerns the outcome,
   not the layer or technology affected.
5. **Generate or update the applicable User Story:** Under the human's work
   mandate (G40), for business behavior, produce a draft feature US with
   Acceptance Criteria and source
   traceability. For technical work, use the permanent US-000 container and
   reference the governed source of the need, such as an approved ADR,
   approved Review finding, approved Discovery conclusion, technical-debt
   evidence, existing code, or other project evidence. US-000 itself is not
   submitted for approval.
6. **Stop at `AITL-US-Approval` when the US is new or materially changed:**
   For a feature US, the Functional Analyst either approves it or requests
   changes. A request for changes returns the work to analysis and drafting.
   These iterations do not count as V-Bounces. This step does not apply to
   US-000.
7. **Generate draft Bolts:** The Functional Analyst decomposes a
   feature US into functional Bolts (the US approval gates their
   readiness, §3.0). A Developer, Architect, or Tech Lead may
   define a non-functional Bolt under US-000 from its governed technical
   source, without Functional Analyst participation. Each draft Bolt states the
   requested outcome, scope, exclusions, dependencies, risk, and expected
   evidence, but no implementation instructions. If a material unknown
   appears, pause the definition and run the Discovery cycle in step 3.
8. **Stop at `AITL-BOLT-READY-Approval` for each Bolt:** A Functional Analyst reviews
   each functional Bolt; an Architect or Tech Lead reviews each non-functional
   Bolt. A rejected Bolt returns to its owner for refinement; approval of
   another Bolt does not approve it by association.
9. **Release approved Bolts to refinement:** Only approved Bolts may proceed
   to SPEC preparation and the technical readiness flow.

```mermaid
flowchart TD
    I["Raw inputs in devflow/input/"] --> A["Update affected analysis artifacts"]
    A --> Q{"Material unknown?"}
    Q -->|"Yes"| D["Run DISC investigation"]
    D --> HD{"AITL-DISC-Approval"}
    HD -->|"Changes requested"| D
    HD -->|"Approved"| A
    Q -->|"No"| P{"Primary outcome?"}
    P -->|"Business behavior"| U["Feature US → AITL-US-Approval"]
    P -->|"Technical outcome"| Z["US-000 container + governed source"]
    U --> FB["Functional Analyst creates Functional Bolt"]
    Z --> NB["Technical contributor creates Non-functional Bolt"]
    FB --> HF{"Functional Analyst approval"}
    NB --> HT{"Architect / Tech Lead approval"}
    HF -->|"Changes requested"| FB
    HT -->|"Changes requested"| NB
    HF -->|"AITL-BOLT-READY-Approval"| S["SPEC preparation"]
    HT -->|"AITL-BOLT-READY-Approval"| S
```

The loops shown above belong to functional analysis and backlog refinement.
They do not count as V-Bounces. A V-Bounce begins only after the approvals
applicable to the Bolt's nature and the subsequent `AITL-SPEC-Approval` have
been obtained.

**Output**

- **Approved feature User Stories** (US + AC + source traceability) and the
  permanent, non-approvable `US-000-non-functional.md` container. Architectural decisions and
  non-functional constraints are documented in ADRs approved through
  `AITL-ADR-Approval`.
- **Approved Discovery conclusions when research was needed**, linked from the
  analysis and backlog artifacts they informed.
- **Individually approved product Bolts**, explicitly classified as functional
  under a feature US or non-functional under US-000. Test Bolts are created
  later from TCs through §4.1.1.
- **Initial Risk Register.**
- **Units → Bolts map** (first 1–2 weeks).
- **Demo criteria** per Bolt.
- **Updated project report** (links to everything).

**Inception checklist**

- Applicable raw inputs preserved and processed.
- Affected analysis artifacts updated and traceable to their sources.
- Feature US+AC generated and approved at `AITL-US-Approval` when functional
  work is in scope; US-000 available as the non-approvable container for
  technical work; architectural decisions and non-functional constraints
  captured in approved ADRs.
- Risks with owner / control.
- Suggested Units and Bolts.
- Every Bolt selected for refinement approved at `AITL-BOLT-READY-Approval`.
- Demo criteria defined.

### 4.1.1 Test Case design, approval, and Test Bolt creation

**Goal:** Define verification independently from the implementation so that QA
tests the approved intent rather than reproducing the assumptions or defects
of the current code.

**Steps**

1. **Select an approved source Bolt.** A functional TC uses the approved
   feature US/ACs and the exact approved functional Bolt. A non-functional TC
   uses the exact approved non-functional Bolt plus approved ADRs or other
   governed technical expectations.
2. **Draft the TC without using code as the oracle.** Define preconditions,
   data, steps or stimulus, expected results, covered criteria, and pass/fail
   evidence from the approved test basis. Inspect existing code only for
   interface, setup, data, feasibility, and regression context.
3. **Stop at `AITL-TC-Approval`.** QA and the applicable functional or
   technical domain owner approve the exact TC or request changes. A draft TC
   cannot govern verification; its Test Bolts may be drafted alongside it but
   cannot be approved before it.
4. **Feed the approved TC into the product Bolt SPEC.** When QA verification
   applies, the approved TC is part of the SPEC's governed source inventory
   and test strategy before `AITL-SPEC-Approval`.
5. **Optionally select the TC for QA Automation.** Create 1..n Test
   Bolts as direct children of that TC, using
   `TC-NNN.BOLT-NNN-<description>.md`. Each Test Bolt covers exactly one TC and
   represents one independently deliverable automation outcome.
6. **Stop at `AITL-BOLT-READY-Approval` for each Test Bolt.** A QA Lead, QA
   Automation Lead, Architect, or Tech Lead approves it. It then follows the
   same one-SPEC, V-Bounce, MEM, acceptance, and manifest lifecycle as every
   other Bolt.

```mermaid
flowchart TD
    B["Approved product Bolt + governed intent"] --> T["Draft independent TC"]
    T --> H{"AITL-TC-Approval"}
    H -->|"Changes requested"| T
    H -->|"Approved"| S["TC feeds product SPEC"]
    S --> O{"QA Automation selected?"}
    O -->|"Yes"| A["1..n TC-NNN.BOLT-NNN Test Bolts"]
    A --> HB{"AITL-BOLT-READY-Approval"}
    HB -->|"Approved"| P["Each Test Bolt prepares its own SPEC"]
```

## 4.2 SPEC preparation and approval

**Goal:** Convert exactly one approved Bolt into a complete, evidence-grounded
and human-approved implementation plan without allowing the agent to invent
missing behavior or architecture.

**Steps**

1. **Admit one approved Bolt:** Its `AITL-BOLT-READY-Approval` includes the Bolt DoR.
   There is no separate DoR sign-off. A functional Bolt also links the current
   `AITL-US-Approval`; a non-functional Bolt links US-000 without parent
   approval; a Test Bolt links its exact parent TC and
   `AITL-TC-Approval`. If this is a BUG Bolt, it also links the exact approved BUG and
   its `AITL-BUG-Approval`; a draft, rejected, or stale BUG blocks admission.
2. **Run the pre-SPEC evidence gate:** Verify approval of every applicable TC,
   ADR, Discovery, Review and AREV output that will govern or inform the plan. If any required
   artifact is not approved, stop and report it; do not generate a SPEC.
3. **Analyze the complete relevant context:** Read the Bolt, its parent artifact,
   applicable approved TCs, source US/ACs,
   approved BUG when applicable, applicable ADRs, relevant `input/` and `analysis/` documents, approved
   DISC/REV/AREV evidence, existing code and tests, configuration,
   infrastructure, schemas, migrations, dependencies, interfaces, risks, open
   questions, repository conventions and relevant previous execution evidence.
4. **Resolve gaps before drafting:** Conflicting evidence, unknown behavior or
   an unresolved architectural decision blocks SPEC generation. Route the gap
   through analysis, Discovery, Review or ADR as appropriate and complete its
   own approval lifecycle.
5. **Generate the one canonical SPEC:** Record its Bolt, repository baseline,
   complete source inventory, approval references, implementation steps,
   impacted files and components, test strategy and evidence, gates, risks,
   migration/rollback needs and stop conditions.
   For a BUG Bolt, also record the approved BUG and prescribe the strict
   single-V-Bounce TDD order: create/run the reproduction test to red before
   any production-code change, then implement and run targeted plus applicable
   regression tests to green. For a defect that reproduces only in integration
   tests, the red is the failed integration test run recorded before the
   V-Bounce, the V-Bounce adds the best hermetic reproduction, and the green
   is a passing covering run in the integration test cycle; the integration
   test plan is `applicable` (§3.3.1).
   For a Test Bolt, define only the QA Automation code and supporting test
   assets needed to automate its one approved parent TC; production-code
   changes are outside scope.
6. **Stop at `AITL-SPEC-Approval`:** Applicable human reviewers inspect the
   plan and its sources. Changes requested return to steps 3–5. Approval alone
   unlocks the code-run/V-Bounce.

```mermaid
flowchart TD
    B["AITL-BOLT-READY-Approval includes DoR"] --> G["Pre-SPEC approval gate"]
    G --> C["Analyze parent, TCs, ADRs, evidence and repository"]
    C --> Q{"Missing, conflicting or unapproved input?"}
    Q -->|"Yes"| R["Stop and resolve through its artifact lifecycle"]
    R --> G
    Q -->|"No"| S["Generate one canonical SPEC"]
    S --> H{"AITL-SPEC-Approval"}
    H -->|"Changes requested"| C
    H -->|"Approved"| V["Code-run / V-Bounce unlocked"]
```

**Output**

- One approved, versioned SPEC for one Bolt, with approval evidence and a
  repository baseline.
- A blocking report instead of a SPEC whenever required governed evidence is
  missing or unapproved.

## 4.3 Weekly planning (light cadence)

**Goal:** Set clear expectations without bureaucracy.

**Steps**

1. **Forecast with data:** historical throughput and bolt lead time
   (P50/P85).
2. **Commit + Stretch:** commit the **P85** and leave 1–3 **Stretch**
   (optional) items — the **P50** slice (§3.2).
3. **10–20% buffer:** reserve time for blockers, support, hardening.

**Output**

- **Weekly plan:** Commit and **Stretch** Bolt list, owners and demo
  criteria.

## 4.4 Executing Bolts with V-Bounce

**Goal:** Close each Bolt with built-in quality.

**V-Bounce anatomy (AI-agent cycle)**

1. **Approved SPEC** with `AITL-SPEC-Approval`, referencing an individually approved Bolt. For functional
   work, the feature parent US and its covered ACs are also approved. For
   non-functional work, the Bolt references the non-approvable US-000 container
   and the originating governed technical evidence, together with relevant
   approved ADRs. A BUG Bolt also references its exact approved BUG and
   `AITL-BUG-Approval`. A Test Bolt references exactly one approved parent TC,
   its `AITL-TC-Approval`, and the TC's original US/AC and source-Bolt
   traceability.
2. **AI agent generates the intended-final change by default** (design / code /
   tests) and **executes the hermetic suite (§3.6) in its own loop** until it
   is green or a mandatory stop condition is reached; the integration tests it
   writes are built and statically checked here and executed in the Bolt's
   integration test cycle when its `test_integration` is `required` — a Test
   Bolt's automation in its TC's run cycle (§3.2.2). Retries performed without human
   intervention are internal iterations, not additional V-Bounces. If an unresolved
   architectural decision appears, the agent stops: an ADR must be approved
   and the canonical SPEC revised and re-approved before execution continues.
   For a BUG Bolt, the agent first creates and runs the reproduction test
   against the pre-fix behavior and records the expected red result. Only then
   may it modify production code and run targeted plus applicable regression
   tests to green. Both phases belong to this same V-Bounce — except for a
   defect that reproduces only in integration tests, whose red is the failed
   integration test run recorded before this V-Bounce: the agent writes the
   integration test that reproduces it, runs the best hermetic reproduction
   before any production change, and leaves the green to a passing covering
   run in the integration test cycle (§3.3.1).
   For a Test Bolt, the agent changes only QA Automation code and supporting
   test assets required by its parent TC. A need to change production behavior
   stops the V-Bounce and enters the BUG lifecycle.
3. **AI agent creates the mandatory MEM + updates manifest** — the agent
   writes exactly one MEM for this V-Bounce, including the complete content
   required by §2.12 and any failure or blocker, and appends the matching
   `v_bounces[]` entry to the Bolt manifest in `devflow/metrics/bolts/`. This happens
   **before** human review and is mandatory even when execution stopped without
   a green hermetic suite.
4. **`AITL-MEM-Approval`** (dev / QA / security / applicable domain) — the
   reviewer reads the **actual diff + test/gate evidence + MEM + manifest**,
   with findings pre-filtered if a stakeholder ran an AREV on this package,
   and records the approval decision.
5. If `AITL-MEM-Approval` is **approved**, the V-Bounce is approved and
   completes successfully; when it is the latest V-Bounce, this is the
   **Development Completed** event: the Bolt becomes `in-integration-testing`
   (`test_integration: required`, plan `applicable`) or, when no integration
   test run is pending (`test_integration: not-required`, or a `waived` plan),
   `ready-for-acceptance` (§3.2.3). If **changes are requested** or the MEM is
   **rejected**, the MEM and V-Bounce remain unapproved, the Bolt remains
   `in-development`, and the next agent execution starts a new V-Bounce with a
   new MEM and `v_bounces[]` entry.

```mermaid
flowchart TD
    subgraph VB["V-Bounce — AI Agent Cycle"]
        direction LR
        S["SPEC: Bolt + governed evidence + ADRs"] --> AI["AI agent generates and verifies the implementation"]
        AI --> G["V-Bounce gates: hermetic classic + AI-native (§3.6)"]
        G -->|"fail"| AI
        G -->|"pass / approved waiver"| MM["Create mandatory MEM + manifest"]
        AI -->|"stop condition"| MM
        MM --> HR{"AITL-MEM-Approval"}
    end

    HR -->|"Approved"| AP["V-Bounce approved: Development Completed"]
    HR -->|"Changes requested / rejected"| NB["Next V-Bounce: new MEM + manifest entry"]
    NB --> S
    AP --> IP{"test_integration (Bolt) and integration test plan (SPEC)"}
    IP -->|"required + applicable"| IC["In Integration Testing: runs on the approved change"]
    IP -->|"not-required, or waived plan"| RA["Ready for Acceptance"]
    IC -->|"pass"| RA
    IC -->|"fail: classified code / test / spec"| NB
    IC -->|"blocked, environment or tests owned elsewhere: new run"| IC
    IC -->|"integration testing waived by a human"| RA
    RA --> AC{"AITL-BOLT-DONE-Approval"}
    AC -->|"approved"| DN["Done"]

    style AI fill:#fadbd8,stroke:#c0392b,stroke-width:2px
    style G  fill:#fdebd0,stroke:#b9770e,stroke-width:2px
    style HR fill:#d6eaf8,stroke:#2471a3,stroke-width:2px
    style NB fill:#fadbd8,stroke:#c0392b,stroke-width:2px
    style IC fill:#fdebd0,stroke:#b9770e,stroke-width:2px
```

**Rules**

- **1..n V-Bounces per Bolt** (typically 1–3).
- One working day is the active-delivery target. More V-Bounces or continuation
  on the next day keep the same Bolt; no retroactive split is performed.
- **Tests from minute zero where meaningful**, derived from ACs and governed
  constraints and written with the change: the hermetic ones are run by the
  agent itself in the V-Bounce, the integration tests are executed in the
  Bolt's integration test cycle when its `test_integration` is `required`;
  otherwise the approved SPEC records the
  appropriate verification and `n/a` reason.
- **BUG Bolts require strict red→green evidence**: the failing reproduction
  precedes every production-code change. When the defect reproduces in the
  V-Bounce, red and green belong to that same V-Bounce and both results are
  captured in the MEM; when it reproduces only in integration tests, the red
  is a failed integration test run recorded before the V-Bounce, cited in the
  MEM, and the green is a passing covering run in the integration test cycle
  (§3.3.1). The manifest keeps the V-Bounce execution outcome and MEM
  reference, and those runs in `integration_test_runs[]`.

**Bolt DoR (summary, validated within `AITL-BOLT-READY-Approval`)**

- Functional Bolt: correct feature parent approved at
  `AITL-US-Approval`. Non-functional Bolt: correctly assigned to US-000; no
  parent approval applies. Test Bolt: exactly one parent TC approved at
  `AITL-TC-Approval`, with test-only scope and a clear automation outcome.
- This Bolt approved at the role-appropriate `AITL-BOLT-READY-Approval`.
- Functional Bolt: clear covered ACs and expected behavior.
- Non-functional Bolt: clear technical outcome and governed source evidence.
- Test Bolt: clear automation outcome, supported execution context and expected
  evidence for its one approved parent TC.
- Applicable ADRs approved through `AITL-ADR-Approval`.
- Risks / controls identified.
- `test_integration` and `test_integration_reason` declared — proposed by the
  drafting agent, confirmed or changed by this approver (§3.2.2).
- Context accessible for the AI agent.
- No `open` or `in-validation` `OQ-NNN` targets this Bolt's parent or
  governing artifacts (§3.2, G35).
- Estimation (active delivery time) and demo criteria; approval wait and total
  elapsed cycle time are reported separately.

There is no independent DoR checkpoint. After Bolt approval, the separate
`AITL-SPEC-Approval` validates the implementation plan before this V-Bounce.

**Bolt DoD (summary)**

- Applicable implementation, tests and gates are **green** at their level
  (§3.6) or explicitly `waived`; non-applicable gates are `n/a` with a reason.
- Every V-Bounce has exactly one immutable MEM and one
  `AITL-MEM-Approval` decision. Earlier `changes_requested` MEMs remain
  unapproved history; the latest MEM must be approved.
- The latest V-Bounce has approved `AITL-MEM-Approval`, so the **Development
  Completed** event took place, and the Bolt is `ready-for-acceptance` before
  acceptance: a passing covering integration test run, `not-required`, a
  `waived` plan, or integration testing waived by a human with its reason
  (§3.2.2).
- Every applicable ADR approved through `AITL-ADR-Approval`.
- Evidence of gates (security / licenses / performance).
- **Complete traceability** and ready to demo.

**Output**

- Bolt `ready-for-acceptance` (integration tests passed; no run pending —
  `not-required`, or a `waived` plan; or integration testing waived), with
  evidence and traces to code / tests / ADRs, ready for
  `AITL-BOLT-DONE-Approval`.
- After that separate acceptance checkpoint, Bolt `done`.

## 4.5 Continuous integration and Quality Gates

**Goal:** Turn ACs and ADR-defined constraints into **automated
checks**.

**Minimum gates, by level (§3.6)**

- **V-Bounce:** the universal manifest, policy and secret-scanning gates;
  build, static checks, unit, component and double-backed contract tests when
  behavior changes; SAST, dependency scanning and licenses when their risk
  surface applies; the static OWASP review; the AI-native gates selected by
  the risks in §3.6.
- **Integration test cycle:** integration tests against real dependencies;
  real-provider contract or E2E verification when the change crosses
  component boundaries; DAST; perf-smoke with SPEC-defined p95 and/or p99
  thresholds for performance-sensitive endpoints or workloads; observability
  checks for backend/services.
- **Release:** cross-Bolt E2E/contract tests, mutation testing, the SBOM of
  the built distributable.

**Rule:** Every applicable gate must be `pass` or `waived` through an approved
ADR. `fail` blocks the checkpoint its level feeds: a V-Bounce-level `fail`
blocks `AITL-MEM-Approval` and merge; a `fail` at the integration test level
blocks `AITL-BOLT-DONE-Approval`; a release-level `fail` blocks the release. Any
`fail` blocks promotion. `n/a` requires a reason in the approved SPEC.

## 4.6 Packaging and deployment (Deployment Units)

**Goal:** Produce **deployable** artifacts.

**Steps**

1. Package (image / function / IaC) with traceable version (issue / commit /
   build).
2. Run **deployment gates** (smoke, security checks, policies).
3. Deploy per the **adopting team's own release/promotion process** — DevFlow
   does not prescribe Unit/UAT approval checkpoints in this release. Grouping
   and promotion are still described by the Deployment-Unit concept (§2.11) and
   measured by DORA (§3.7.1).

**Output**

- **Deployment Unit** deployable, with evidence.

## 4.7 Release and customer acceptance (not prescribed in this release)

The Unit/UAT approval-and-release layer was removed in v4.2: the governed flow
ends at Bolt acceptance (`AITL-BOLT-DONE-Approval`), and grouping into a
deployable unit, environment promotion and customer acceptance (UAT) follow the
**adopting team's own process**. A redesigned model — informed by real
environment/promotion complexity — is planned for a future version.

## 4.8 Production, observability and AI-assisted operations

**Goal:** Operate with **intelligent alerts** and **suggested actions**.

**Steps**

- Deploy the named Deployment Unit per the team's release process (plus change
  management when applicable).
- Real-time monitoring (metrics / logs / traces) with **anomaly detection**.
- AI suggests **actions** (scaling, tuning, rollback) → **human approval**.

**Output**

- Output mapped to Bolts / Units.
- **Runbooks** and **post-release checks** (errors / telemetry / UX).

## 4.9 Feedback loop and continuous improvement

**Goal:** Learn **throughout the week**.

**Rituals**

- **Weekly demo:** show **Bolts Done** (tangible value).
- **Short retro:** review metrics and anti-patterns; tune DoR/DoD, prompts
  and gates.
- **Memory update:** consolidate lessons, best prompts, frequent ADRs.

**Key metrics**

- Lead time per Bolt (median, total cycle time)
- AI-time per Bolt (agent generation time sub-metric)
- Weekly throughput (# Bolts)
- % Commitment delivered
- Average V-Bounces per Bolt
- SPEC first-review approval rate
- V-Bounce first-review approval rate
- Defect escape (UAT / prod)
- (Optional) Tokens per Bolt

## 4.10 Exception flows (Incidents and Hotfix)

**Goal:** Give urgent work immediate priority without creating a parallel or
weaker method.

**Service classes**

- **Incident / Hotfix:** **immediate priority**; target a small, bounded Bolt of no
  more than 4 active delivery hours when the scope permits.
- **Regulatory:** high priority with dates; reserve fixed capacity.
- **Debt / Hardening:** reserve **10–20%** per week.

**Rules**

- Classify the outcome through the normal paths. A functional hotfix uses the
  affected feature US and its dedicated functional Bolt. A non-functional
  hotfix uses a dedicated non-functional Bolt under `US-000-non-functional.md`.
  When the hotfix corrects a defect, first create and approve its functional or
  non-functional BUG through `AITL-BUG-Approval`, then create the dedicated
  Bolt under the corresponding parent.
- The hotfix Bolt follows the same lifecycle as any other Bolt —
  `AITL-BOLT-READY-Approval → SPEC → AITL-SPEC-Approval → V-Bounce → MEM →
  AITL-MEM-Approval`, then its integration test cycle when `test_integration`
  is `required` with an `applicable` plan, then `AITL-BOLT-DONE-Approval` once
  `ready-for-acceptance`; with no run pending, `AITL-MEM-Approval` and
  `AITL-BOLT-DONE-Approval` may be recorded in one joint act (§3.2.2, §3.2.3).
  Urgency changes priority, never traceability or approvals.
- DevFlow defines no separate after-hours approver, on-call substitution,
  checkpoint expiry, or retroactive acceptance mechanism. Team availability
  and operational staffing remain outside the methodology.
- Keep **minimum gates** even on hotfix (at least tests + basic security).
- Light post-mortem → **hardening bolts** if applicable.

## 4.11 End-to-end traceability

**Goal:** Everything **linked** for people and AI.

**Link map**

1. Raw Input → Analysis (domain-model / BPMN / Glossary) → classify primary
   outcome → approved feature US for functional work **or** permanent US-000
   container plus governed source for non-functional work → Draft Bolt →
   `AITL-BOLT-READY-Approval` → independent TC design → `AITL-TC-Approval`
   when QA verification applies → applicable ADRs approved through
   `AITL-ADR-Approval` → canonical SPEC → `AITL-SPEC-Approval` → V-Bounce(s) →
   Code/PR → Tests/gates → MEM + manifest → `AITL-MEM-Approval` (**Development
   Completed**) → integration test cycle (runs recorded in the Bolt manifest,
   a failure classified by a human, or integration testing waived by a human)
   or an `n/a` / `waived` plan → `ready-for-acceptance` →
   `AITL-BOLT-DONE-Approval` → `done` → Build → Deployment Unit → Prod (team
   release process)

   BUG branch: `BUG draft` → `AITL-BUG-Approval` → exactly one dedicated
   functional or non-functional Bolt → canonical SPEC → same-V-Bounce TDD
   red→green (a defect that reproduces only in integration tests: red
   integration test run → V-Bounce → green covering run) → MEM evidence plus
   the manifest's V-Bounce/MEM traceability and its `integration_test_runs[]`.

   QA Automation branch: approved `TC-NNN` → 1..n `TC-NNN.BOLT-NNN` Test Bolts
   → individual `AITL-BOLT-READY-Approval` → one canonical SPEC per Test Bolt
   → standard V-Bounce/MEM lifecycle; a Test Bolt is always
   `test_integration: not-required` — its automation runs in the TC's run
   cycle.

**Good practices**

- Use IDs / URLs of cards / issues / PRs.
- Capture **decisions** in every V-Bounce (knowledge is product).

## 4.12 Sample agenda (first week)

**Monday**

- 30-minute planning: **Commit** 6 Bolts + **Stretch** (aspirational) 2
  (team of 4 dev-validators + AI agents).
- Approve the first 3 Bolts, including their DoR criteria, through the
  applicable `AITL-BOLT-READY-Approval`.

**Tuesday–Thursday**

- Execute Bolts with **V-Bounce** (1–3 bounces each, run by AI agents).
- CI with gates (tests / security / performance).
- Promote to staging those meeting **DoD**.

**Friday**

- **Demo:** 5–7 Bolts Done (depending on throughput).
- **Metrics:** median lead time, % Commit, average bounces, gate failures.
- **Retro:** adjust Bolt size / DoR / gates; record lessons learned.

## 4.13 Operational summary

1. **Everything starts from evidence** preserved in `input/` or already
   governed elsewhere in the project → AI-assisted functional analysis updates
   `analysis/` as applicable → a Functional Analyst defines functional Bolts
   from feature USs, while Developers, Architects, or Tech Leads
   define non-functional Bolts under US-000 → each Bolt follows its own
   role-appropriate approval route.
2. **Test Cases are independent contracts:** QA derives them from approved
   US/ACs and Bolts, never from current code as the expected-behavior oracle;
   `AITL-TC-Approval` governs their use.
3. **QA Automation uses Test Bolts:** every TC selected for automation
   may originate 1..n `TC-NNN.BOLT-NNN` work units with the full standard
   lifecycle.
4. **Light weekly plan** with **Commit + Stretch**.
5. **Bolts targeting 1 hour to 1 working day of active delivery** as the unit
   of promise and measurement; elapsed cycle time and approval waits are
   measured separately, and an additional V-Bounce never forces a split.
6. **One canonical SPEC per Bolt:** the agent analyzes every relevant governed
   artifact and the actual repository; `AITL-SPEC-Approval` is mandatory before
   any code-run.
7. **V-Bounce** as the standard execution form: the AI agent generates the
   intended-final change by default, runs the hermetic tests, creates the
   mandatory MEM and manifest entry, and pauses at `AITL-MEM-Approval`; only
   approval completes the V-Bounce; on the latest one it is the **Development
   Completed** event, and the Bolt becomes `in-integration-testing`
   (`test_integration: required`, plan `applicable`) or, when no integration
   test run is pending (`test_integration: not-required`, or a `waived` plan),
   `ready-for-acceptance` (§3.2.3). Bolt `done` remains a separate acceptance
   decision.
8. **BUGs are governed before work definition:** any Functional Analyst,
   Developer, or QA may draft one; `AITL-BUG-Approval` is mandatory, and gates
   its one dedicated Bolt's readiness (the Bolt may be drafted first). Its approved SPEC enforces red-before-fix and
   green-after-fix inside the same V-Bounce — or, for a defect that
   reproduces only in integration tests, a red integration test run before
   the V-Bounce and a green covering run in its integration test cycle.
9. **Applicable gates** → `pass` or approved `waived`; `fail` blocks **Done**.
10. **Demo and metrics** every week → continuous improvement.
11. **Complete traceability** for people and AI.

---

# 5 — Repository structure and documentary implementation

Every methodology artifact is materialized as Documentation as Code. The
repository structure below is normative: teams may add project-specific files
inside the appropriate folders, but they must not rename, relocate, or change
the responsibility of the canonical folders without an approved methodology
change.

This section is normative only for artifact locations, filenames and folder
responsibilities. Lifecycle and governance descriptions in the folder tables
summarize §3 and do not create a second rule source.

**The tree below is a project's, not the methodology's.** A repository that
adopts Avenga DevFlow receives exactly two things at its root — the `devflow/`
documentary root and the cross-tool `AGENTS.md` — plus the agent definition
for the tool the team uses, installed wherever that tool expects it (§5.2).

- `devflow/` is the single source of truth for project documentation,
  traceability, governance evidence, and implementation memory.
- `AGENTS.md` is the cross-tool entry point several agents auto-load from the
  repository root; it names the source of truth, the guardrails and the
  language policy.

## 5.1 Canonical folder tree

```text
<project-repo>/
├── AGENTS.md                  ← cross-tool agent entry point
├── CLAUDE.md                  ← the agent definition for the tool in use;
│                                other tools read it from .github/agents/,
│                                .opencode/agents/ or .agents/skills/
├── CHANGELOG.md               ← the project's own history, including its
│                                methodology upgrades (§5.16). DevFlow does
│                                not ship one — this file belongs to the
│                                repository, not to the framework
│
└── devflow/
    ├── README.md
    ├── VERSION
    ├── LANGUAGE
    ├── GUARDRAILS.md
    ├── ONBOARDING.md
    │
    ├── input/
    │   ├── business/
    │   ├── databases/
    │   ├── documentation/
    │   ├── interviews/
    │   ├── source-code/
    │   └── ui-ux/
    │
    ├── analysis/
    │   ├── introduction/
    │   ├── vision/
    │   ├── business-context/
    │   ├── business-risks/
    │   ├── scope/
    │   ├── personas/
    │   ├── user-journeys/
    │   ├── glossary/
    │   ├── domain-model/
    │   │   ├── entities/
    │   │   ├── enumerations/
    │   │   └── relationships/
    │   ├── ui/
    │   ├── process/
    │   └── open-questions/
    │
    ├── discovery/
    ├── functional/
    │   ├── user-stories/
    │   └── bolts/
    ├── actors/                 ← who is in the team: the roster home (humans + DevFlow Agents as actors; §3.0.1)
    ├── agents/                 ← the DevFlow Agent definitions: examples/ (shipped references) · squad/ (the project's live agents) · TEMPLATE-new-role/ · VERIFICATION.md (the per-platform install mapping)
    ├── skills/                 ← project-level skills (one SKILL.md folder per skill, scope shared|role) referenced from the agents' skills: lists (§5.2.1)
    ├── adrs/
    ├── spec/
    ├── memory/
    ├── metrics/
    │   ├── manifest-v5-bolt.schema.json
    │   ├── manifest-v5-us.schema.json
    │   ├── manifest-v5-tc.schema.json
    │   ├── bolts/
    │   ├── user-stories/
    │   └── test-cases/
    ├── sprints/                ← sprint records, one planning record per sprint (§5.12)
    ├── reports/
    ├── reviews/
    ├── adversarial-reviews/
    ├── bugs/
    ├── incidents/
    ├── risks/
    ├── retros/
    ├── tests/
    │   ├── test-cases/
    │   └── uat/
    ├── bin/                    ← compiled tooling executables (optional by contract; replaced on upgrade, never copied forward, §5.16)
    ├── agents-data/
    ├── prompts/
    ├── agent-runs/             ← agent-run records, one per agent run (§2.18)
    └── avenga-devflow/
        ├── Avenga-DevFlow.md
        ├── README.md
        └── INDEX.md
```

## 5.2 Agent definitions

**What a project installs.** Two files reach the project repository from the
methodology distribution, and they are separate concerns:

| Installed file | Where it goes | What it is |
|----------------|---------------|------------|
| `AGENTS.md` | the project's repository root, next to `devflow/` | Cross-tool entry point, auto-loaded from the root by several agents regardless of platform. It points at the platform definition rather than replacing it. **Two owners, one file:** everything above its `AVENGA-DEVFLOW:PROJECT-SECTION` marker is framework and is replaced on upgrade; everything from the marker onward is the project's own and is never touched (§5.16). |
| One platform definition | wherever that tool expects it — `CLAUDE.md` at the root, `.agents/skills/`, `.github/agents/`, `.opencode/agents/` | The compact orchestration of the methodology for that specific tool |

Both files are installed from the Avenga DevFlow distribution, which ships one
definition per supported tool with the exact destination for each. They are
operational instructions consumed directly by development tools rather than
project documentation, which is why they live at the repository root and in
tool-specific locations instead of inside `devflow/`.

The platform definitions share their methodology sections **verbatim** — they
differ only in the wrapper each tool requires (tool names, todo mechanism,
memory wording). A team installs the one for the tool it uses; the others are
not needed.

**Platform memory is out of scope — the methodology defines the *what*, not
the *where*.** An agent definition never names a platform's memory location
(file path, folder, or frontmatter convention): each agent uses its own
platform's native mechanism, which the platform may change without notice.
What the methodology does define is which *kind* of information goes where:
personal preferences and session memory → the platform's native mechanism;
durable, team-shared knowledge → the agent's own `agents-data/<agent>/` area
(§5.12 — the Coordinator's is named per tool, not by its actor id);
governed implementation memory → `memory/` MEMs (§2.12), which nothing else
replaces.

These folders provide the reusable agent definitions for each tool. During an
AREV, the human manually selects and changes the active agent/model at the
phase checkpoints described in §3.13. The selected agent/model is recorded in
the corresponding AREV phase artifact and does not update a Bolt manifest.

### 5.2.1 Skills — `devflow/skills/`

Skills are **project-level capabilities**: one folder per skill under
`devflow/skills/<skill-name>/` — a mandatory `SKILL.md` (the open Agent Skills
shape: `name`, `description`, optional `license`, `compatibility`, `metadata`)
plus optional `references/` and `assets/` loaded on demand. Each skill declares
its **scope** in `metadata.scope`: `shared` (a procedure any actor may run —
a spawned agent, the Coordinator, the human) or `role` (background knowledge
for the agents that reference it; the default). A DevFlow Agent **declares the
skills it uses** in its definition's `skills:` list — **references** under
strict symmetry: every referenced skill must exist; a skill nobody references
is still a valid project capability; a definition without `skills:` is
complete. The Coordinator projects a skill **once** per platform, into that
platform's skills surface, together with the wrappers of the agents that
reference it (the mapping, the scope-driven invocation controls, the
provenance marker, the never-silent fallback and the parity rule live in
`devflow/agents/VERIFICATION.md`). Two invariants hold everywhere: **skills
are content, never authority** — nothing in a skill grants a tool, an MCP
server, a capability tier, spawn capability or approver mode, and a `SKILL.md`
carrying a granting field is refused at install — and **shipped skills are
tool-agnostic**. Skills are living data — like the project's live agents in
`devflow/agents/squad/`, a team adds, edits or removes them without a Bolt and
reinstalls the affected agents. The family's contract and
authoring guide live in `devflow/skills/README.md`; the agent definitions that
reference skills live in `devflow/agents/` and the roster in `devflow/actors/`
(each family carries its own README).

## 5.3 `devflow/` root files

The `devflow/` directory is the documentary root and single source of truth for
the methodology as applied to a project.

| File | Purpose |
|------|---------|
| `devflow/README.md` | Entry point and map of the framework: workflow, naming conventions, language policy, AITL checkpoints, golden rules, and project quick start. |
| `devflow/VERSION` | Declares the Avenga DevFlow version used by the project. |
| `devflow/LANGUAGE` | Declares the project's `content_language` (ISO 639-1, e.g. `en`, `es`). Updated as soon as the language of the raw inputs is detected; the AI agent reads it and writes prose accordingly (§3.15). |
| `devflow/GUARDRAILS.md` | Contains the non-negotiable agent rules for blocking conditions, warnings, naming, traceability, V-Bounce execution, AITL coverage, and approval routing. |
| `devflow/ONBOARDING.md` | Provides the recommended reading order, role-based guidance, glossary, FAQ, and developer skill-development guidance. |

## 5.4 Archiving — `_archive/` in each folder

Every `devflow/` folder may contain an `_archive/` subfolder to keep the
active workspace manageable as documents accumulate. This is the standard
mechanism for governing document growth (e.g., `US-000` with dozens of
completed non-functional Bolts).

- **Criterion (G38):** only documents whose lifecycle is closed are
  archived — `done` Bolts with their complete package (Bolt, SPEC, MEMs),
  `superseded`/`deprecated` ADRs, closed DISC/REV/AREV records with every
  finding routed to the artifacts it affects, closed BUGs, retired RISKs,
  and completed UAT minutes. **Archiving never causes closure, it
  presupposes it:** the move is housekeeping, not a lifecycle step, and it
  grants no approval the document does not already hold. Never archive an
  active, draft, or in-review document — because `_archive/` is excluded
  from agent scans and its contents are treated as generally invisible
  (below), that move is the only one that removes open work from
  governance without ever closing it. When closure cannot be established
  from the document itself, the agent does not archive it and asks.
- **Mechanism (AITL, periodic):** archiving is a housekeeping activity
  (e.g., end of sprint or when a folder's INDEX becomes unwieldy). It may be
  performed manually or by the agent under the same AITL governance as
  everything else: the agent proposes the moves and executes them only with
  the human's decision, and every move is visible in the conversation and
  in the diff. Move the closed documents from the folder into its
  `_archive/` subfolder.
- **References are NOT rewritten:** archived documents keep their original
  content, IDs and internal links. Cross-references from other documents or
  manifests point to the archived location only when a human updates them;
  traceability remains guaranteed by the immutable IDs (`SPEC-YYMMDD-HHmm`,
  `MEM-YYMMDD-HHmm-<description>`, `ADR-NNN`, `US-NNN.BOLT-NNN`), not by
  paths. A path that still points at the old location is acceptable for
  archived evidence.
- **INDEX discipline:** each folder's `INDEX.md` lists the active documents
  only; archived documents are excluded from it (optionally summarized in a
  dedicated `_archive/INDEX.md`). Naming and numbering rules still apply:
  archived IDs are never reused.
- **`_archive/` is excluded from agent scans** for token economy: agents do
  not search, list, or read `_archive/` proactively. They access it only
  when the user explicitly asks, or when an active document explicitly
  references an archived artifact (e.g., an incident linking to the
  deployment that caused it).
- **Transparency to the user:** whenever a task would require archived
  content, the agent states that `_archive/` files are excluded from its
  scan and asks the user whether to consult them. Users should treat
  archived files as generally invisible to agents.

## 5.5 Derivative narrative documents

Some folders hold **derivative documents**: plain-language narratives whose
entire content is derived from governed artifacts. They are marked with
`derivative: true` in their frontmatter and follow four rules:

- They are **never a source of truth**: where a narrative and an artifact
  disagree, the artifact wins.
- They are **outside the AITL chain**: they have no approval checkpoint of
  their own and are never governed input — they may not be cited as the
  basis of a SPEC, Bolt, ADR, User Story, or Test Case.
- They introduce **no business rule, decision, or finding of their own**;
  a gap discovered while writing one is routed to the proper artifact
  (`OQ-NNN`, or a fix to the source artifact), never papered over.
- They may be written, corrected, or discarded **at any time without a
  checkpoint**, and are marked `deprecated` when they go stale rather than
  left circulating with false information.

The canonical example is `analysis/introduction/` (§5.7): the plain-language
entry point written at the end of the analysis phase, once the artifacts it
summarizes exist at least in draft.

**Generated artifacts that cannot carry frontmatter** belong to this class by
their location instead of by a marker: the sprint reports in `reports/`
(§5.12) are rendered HTML derived entirely from the manifest family, so they
are governed by the four rules above and are never citable as the basis of a
governed artifact (G28), even though no `derivative: true` field can be
written into them.

**An agent-run record is not in this class.** A record in `agent-runs/`
(§2.18) is the primary chronology of an agent run, not a summary of governed
artifacts: it carries no `derivative: true`, and its rules are its family's
own (§5.12), not the four above. It shares only their outcome — it is never
governed input. G28 makes it non-citable by its location, with a wider list
than the derivative class: never the source or justification of a SPEC,
Bolt, ADR, US, TC, BUG, MEM, or any AITL checkpoint; and where a run record
and a governed artifact disagree, the artifact wins.

## 5.6 Raw inputs — `devflow/input/`

`input/` contains source material exactly as received from stakeholders,
customers, vendors, or legacy systems. It is treated as read-only evidence:
derived or normalized information belongs in `analysis/`, `discovery/`, or
another downstream folder. Only humans deposit material here: agents may
read `input/` as evidence but must never create, modify, or move files into
it (G31). These raw inputs are the evidence base for the
entire methodology: adding or materially changing an input triggers an impact
assessment and updates every affected analysis artifact before downstream
functional artifacts are approved or re-approved.

| Folder | Purpose |
|--------|---------|
| `input/business/` | Contractual, business, and regulatory source material such as RFPs, BRDs, SOWs, compliance requirements, and regulations. |
| `input/databases/` | Legacy database evidence such as DDL, schemas, entity-relationship diagrams, and approved data extracts. |
| `input/documentation/` | Third-party documentation such as API manuals, vendor PDFs, datasheets, and product guides. |
| `input/interviews/` | Original stakeholder interview transcripts, normally named `INT-NNN-<description>.md` and preserved in their source language. |
| `input/source-code/` | Legacy source code and configuration used as contextual evidence during analysis and implementation. |
| `input/ui-ux/` | Visual inputs such as screenshots, mockups, brand guidelines, prototypes, and UX research. |

## 5.7 Domain understanding — `devflow/analysis/`

`analysis/` turns raw evidence into a shared understanding of the product,
business, users, domain, scope, and unresolved questions. It describes the
problem space before implementation decisions are made. Processing may be
iterative and AI-assisted, but the Functional Analyst governs the result. The
analysis artifacts provide the immediate foundation for generating or updating
User Stories and their Acceptance Criteria.

| Folder | Purpose |
|--------|---------|
| `analysis/introduction/` | Plain-language, jargon-free narrative explanations of each feature — the entry point for someone joining the project. Derivative documents (`derivative: true`, §5.5): written at the end of the analysis phase from the existing artifacts, never a source of truth, never governed input, outside the AITL chain. One descriptively named file per feature. |
| `analysis/vision/` | Product vision, intended outcomes, strategic direction, and high-level Intents. |
| `analysis/business-context/` | Business goals, constraints, stakeholders, market context, compliance concerns, and success measures. |
| `analysis/business-risks/` | Pre-execution business risks (`BR-NNN`) involving regulation, market, adoption, or business-model viability. These are distinct from project and technical risks in `risks/`. |
| `analysis/scope/` | Scope decisions and milestone phases such as MVP, v1, and v2, including deferred items and dependencies between phases. |
| `analysis/personas/` | User personas and archetypes, preferably three to five. A persona may be `real`, when grounded in interviews, or an `archetype`, when it is a role-based composite. |
| `analysis/user-journeys/` | End-to-end user experiences, one goal or journey per file, including stages, touchpoints, pain points, moments of truth, and relevant measures. |
| `analysis/glossary/` | The agreed ubiquitous language of the domain, including definitions and usage notes. |
| `analysis/domain-model/` | The domain model as Markdown and Mermaid: entities, properties, relationships, states, codes, and reusable enumerations. |
| `analysis/domain-model/entities/` | One domain entity per file, normally using PascalCase filenames. |
| `analysis/domain-model/enumerations/` | Reusable domain enumerations, statuses, and code catalogs. |
| `analysis/domain-model/relationships/` | Centralized relationship views and entity-relationship diagrams. |
| `analysis/ui/` | The visual and interaction half of the conceptual model: surface inventories, canonical pattern galleries, the states each surface must present, visual contracts between a surface and its successor, and parity plans. Derived from `input/ui-ux/` and finished before the first User Story; `domain-model/` answers what things exist, `ui/` what the user sees and how it behaves. Living documents, no IDs, no AITL checkpoint. |
| `analysis/process/` | Business processes represented in BPMN or Mermaid and identified as `PROC-NNN`. |
| `analysis/open-questions/` | The centralized backlog of analysis questions (`OQ-NNN`), with lifecycle `open → in-validation → answered / deferred / dropped`. Unresolved questions must be handled by the applicable readiness checkpoint. An agent raises an OQ only inside an artifact it is producing under a work mandate (G40); a gap noticed outside one is reported and the OQ proposed, not created. |

### Writing an open question

An open question is written **for the person who answers it** — a business
owner, a user of a legacy system, a validator, or, for a technical question,
an architect, a developer or an operator — not for the team. Most are raised
during the analysis phase, before any User Story exists, and they go to
people who may never have seen the system, or not for years. An OQ must be
answerable from the document alone:

- **Functional or technical.** An OQ may be about the business or about the
  system (non-functional, architecture, code); technical OQs are frequent
  when migrating old code. It stays in the analysis, and **what its answer
  produces goes to its own artifact** — an ADR when a decision is needed, a
  non-functional Bolt under US-000 when something must be built, a DISC when
  nobody knows and it must be investigated, a REV when something already
  built must be examined. The agent proposes it; a human's request creates
  it (G40). The OQ links to what it produced.
- **Self-contained context, in the language of the person who answers it**
  — business terms for a business question, technical terms for a technical
  one; in both, no unexplained ID, acronym or internal jargon, and a
  technical question explained **just as thoroughly** (the component,
  service or file and what it is for, the relevant code or configuration,
  with path and version) — with a concrete example:
  - for an **existing system** (maintenance or migration): what that part of
    the system is for and how it behaves today, with the evidence described
    for its reader, written for a reader who may not have seen it in a long
    time;
  - for something **new**: what the capability is and what it is for, in
    detail;
  - in both cases, **the question itself explained**: exactly what is asked,
    the situation that raised it, the facts, rules, constraints and people
    the answer must weigh, what is known and what is not, and what depends
    on the answer.
- **A one-screen summary** on top: the question, the options and the
  recommendation, so the validator can decide from it.
- **Three to four options**, presented neutrally, each with what it means,
  what it changes in the targeted artifacts and its risk or cost. Fewer only
  when the question is genuinely binary, with the reason; never a filler
  option.
- **The agent's recommendation, kept apart from the options:** which
  option, why, how confident, and what would change it. The working
  hypothesis follows it until the answer arrives. It is a proposal: the
  validator decides, and an OQ never closes on the recommendation alone.
- **The resolution records whether the answer followed the recommendation**
  — followed, a different option, or another answer — in the body, never
  as a frontmatter key. It makes the human decision visible instead of a
  silent rubber stamp.

`TEMPLATE-OQ.md` carries this structure.

## 5.8 Discovery — `devflow/discovery/`

| Folder | Purpose |
|--------|---------|
| `discovery/` | Need-driven investigations (`DISC-NNN`) that reduce a material unknown before a User Story or Bolt is created or materially refined. Examples include external API analysis, learning an unfamiliar library or framework, legacy-system behavior, gap analysis, technology options, data availability, and integration constraints. Each Discovery records its question, scope, evidence, experiments, assumptions, limits, conclusions, affected analysis artifacts, and approval evidence. It remains draft until `AITL-DISC-Approval`; only then may its conclusions feed analysis, User Stories, Bolts, ADRs, risks, or other governed artifacts. A Discovery is not a Review and does not authorize code. |

## 5.9 Functional definition — `devflow/functional/`

`functional/` defines what must be delivered and contains the work units used
for assignment, execution, and measurement. The folder name reflects the
location of User Stories and Bolt work definitions; it does not imply that
every Bolt has a functional outcome or Functional Analyst ownership.

| Folder | Purpose |
|--------|---------|
| `functional/user-stories/` | User Stories (`US-NNN-<description>.md`) and their Acceptance Criteria, preferably expressed in Given/When/Then form. Each feature US remains a draft until a Functional Analyst records `AITL-US-Approval`; its functional Bolts may be drafted and refined alongside it, and none may be approved before the US is. `US-000-non-functional.md` is the permanent traceability parent for **every non-functional Bolt**. It is a container rather than a feature User Story, is never submitted for approval, and does not define architecture or non-functional constraints; approved ADRs do that. |
| `functional/bolts/` | Bolts are the atomic units of assignment, planning, tracking, and measurement. The three canonical types are functional (`US-NNN.BOLT-NNN-<description>.md` under a feature US), non-functional (`US-000.BOLT-NNN-<description>.md` under US-000), and Test (`TC-NNN.BOLT-NNN-<description>.md` under one TC). Functional Analysts govern functional Bolts; Architects or Tech Leads govern non-functional Bolts; QA or QA Automation Engineers create Test Bolts and QA Leads, QA Automation Leads, Architects, or Tech Leads approve them. A BUG Bolt's `AITL-BOLT-READY-Approval` requires its exact `BUG-NNN`'s `AITL-BUG-Approval`. Every type states what must be delivered and expected evidence, never how; every Bolt independently requires `AITL-BOLT-READY-Approval`. |

## 5.10 Architecture decisions — `devflow/adrs/`

| Folder | Purpose |
|--------|---------|
| `adrs/` | Architecture Decision Records (`ADR-NNN`) that capture technical decisions, alternatives, consequences, and all applicable non-functional constraints. Functional specifications are not ADR content — they live in the feature User Story and its ACs. Every ADR remains a draft until an Architect or Tech Lead records `AITL-ADR-Approval`. Once approved, it is governing and immutable; a changed decision is recorded in a new ADR that supersedes the previous one. |

## 5.11 V-Bounce execution evidence

The following three directories form the core documentary trail of each
V-Bounce: approved implementation instructions, narrative execution memory,
and mechanically measurable evidence.

| Folder | Purpose |
|--------|---------|
| `spec/` | One canonical implementation specification (`SPEC-YYMMDD-HHmm-<description>.md`) per Bolt. Before generating it, the agent verifies the required AITL approvals and inventories the approved BUG when applicable, approved TCs, relevant US/ACs, ADRs, DISC/REV/AREV evidence, `input/`, `analysis/`, repository code, tests, configuration, schemas and interfaces. A Test Bolt SPEC references exactly one approved parent TC and limits changes to QA Automation code and supporting test assets. The SPEC records its exact Bolt, source paths, approval references and repository baseline, then defines implementation phases, impacted components, test strategy, observability, gates, migration/rollback needs, risks and stop conditions. A BUG SPEC must explicitly enforce reproduction-test red before production changes and targeted/regression green afterward in the same V-Bounce — or, for a defect that reproduces only in integration tests, red from a failed integration test run recorded before the V-Bounce and green from a passing covering run in the integration test cycle (§3.3.1). It remains draft until `AITL-SPEC-Approval`; no code-run or V-Bounce may begin before that checkpoint. Material revisions update the same canonical SPEC and require re-approval. |
| `memory/` | Mandatory narrative implementation records (`MEM-YYMMDD-HHmm-<description>.md`), exactly one per V-Bounce. All MEMs for the same Bolt and canonical SPEC reuse the identical `<description>` slug; only their creation timestamp changes. The V-Bounce number is stored inside the MEM and manifest, never appended to the filename. Each MEM identifies the Bolt, SPEC version, V-Bounce iteration, repository baseline and ADRs; summarizes the implementation; enumerates every added, modified, renamed or deleted file with its reason; records tests, gates, decisions, deviations, risks, manual interventions and evidence links; and links to the final `AITL-MEM-Approval`. The MEM has no mutable approval status: its state is derived from the associated AITL decision. Approval of this checkpoint approves the V-Bounce; changes requested preserves the MEM as immutable evidence and requires a new V-Bounce with a new timestamped MEM. |
| `metrics/` | The manifest family v5: three normative schemas (`manifest-v5-bolt.schema.json`, `manifest-v5-us.schema.json`, `manifest-v5-tc.schema.json`) plus one validating manifest JSON per User Story (`user-stories/US-NNN-<description>.json`), per Bolt (`bolts/US-NNN.BOLT-NNN-<description>.json` or `bolts/TC-NNN.BOLT-NNN-<description>.json`) and per Test Case (`test-cases/TC-NNN-<description>.json`). Each manifest records sources, provider/tool/model runs and token/time usage for generation, material SPEC revisions, `v_bounces[]`, lifecycle AITL decisions, the Bolt's integration test facts (the confirmed `test_integration` value and the integration test runs, classifications and waivers — Bolt manifests), and the timing of every step (`created_at`, `review_ready_at`, `review_started_at`, `decided_at`). Gates, DORA, deployment and pre-calculated cost remain outside it (an integration test run is recorded as a fact that points to its evidence). |

Internal autonomous agent retries remain inside a single V-Bounce and are not
added as separate `v_bounces[]` entries. If `AITL-MEM-Approval` records changes
requested, the subsequent agent execution is a new V-Bounce with a new MEM and
therefore a new `v_bounces[]` entry.

## 5.12 Governance and learning

The `devflow/` folder structure is **canonical** (§5.1–§5.14 and the folder
map in `devflow/README.md`): no new folder may be created inside `devflow/`
outside this structure (G30). The only sanctioned agent-created areas are the
per-agent folders under `agents-data/` — each agent creates its own on first
use, is responsible for it, and is free to organize files and subfolders
**within** it — the per-AREV folders
`adversarial-reviews/AREV-NNN-<description>/` (§2.15), and the `_archive/`
subfolders the agent creates when archiving closed documents (§5.4).

| Folder | Purpose |
|--------|---------|
| `agents-data/` | Per-agent shared knowledge area, versioned with the repository and shared with the whole team. There are no pre-created subfolders: each agent creates its own `agents-data/<agent>/` folder on first use and is **responsible for everything inside it** — free to create files and subfolders there. **The Coordinator's folder is named per tool, not by its actor id:** the four platform definitions write `agents-data/claude/`, `agents-data/codex/`, `agents-data/gh-copilot/` and `agents-data/open-code/`, because one repository may be worked from more than one tool and each tool keeps its durable notes apart. The Coordinator writes only in the folder of the tool it runs in, and every record still names it `agent:avenga-devflow` (§3.0.1): the folder names the tool's working area, never the actor. It holds durable, useful knowledge (patterns, decisions, reusable information), never temporary data (W21). It is not governed input: it may not be cited as the source or justification of any governed artifact or checkpoint (G32), carries no AITL checkpoint, agents do not scan other agents' folders by default (token economy), and it is not a substitute for `memory/` MEMs (§2.12). |
| `prompts/` | Project prompts (`PROMPT-NNN-<description>.md`) — versioned, team-shared, copy-paste ready. Prompts are living data: created, modified and improved in this folder, with no approval and no manifest; they are never scattered into `agents-data/` (§5.12). |
| `agent-runs/` | Agent-run records (`ARUN-YYMMDD-HHmm-<description>.md`), one per agent run (§2.18), opened when a human launches the run. A record is an **operational narrative** — the run's own chronology, not a summary of governed artifacts — so it is **not** a derivative document (§5.5) and carries no `derivative: true`. It is still never governed input, by its location: it may not be cited as the source or justification of a SPEC, Bolt, ADR, US, TC, BUG, MEM, or any AITL checkpoint (G28), and where it and a governed artifact disagree, the artifact wins. It carries no AITL checkpoint, no manifest and no `status` (§3.15). Its `INDEX.md` is a curated inventory that also claims the next run number (`run: N`, §5.15). |
| `sprints/` | Sprint records (`SPRINT-NNN-<description>.md`), one per sprint: the sprint's **planning record** — its dates, its goal and the User Stories and Bolts in its scope, by reference — under the folder's `README.md`. Its `status` is declared, never derived from the dates (§3.15). It carries no AITL checkpoint and no manifest. Its `INDEX.md` is the family's allocator: the next number follows the highest listed and is never reused (§5.15). `SPRINT-NNN` identifies the planning record only — it is not the key of a sprint report, which keeps its own ISO-week name (`REPORT-YYYY-Www.html`, below). |
| `reports/` | Sprint progress reports for project management, generated from the manifest family in `metrics/`. Self-contained HTML (one per sprint). **Report generation is planned**: `TEMPLATE-REPORT.html` ships as a design reference with example data, and no generator — nothing in `devflow/` reads the manifests to emit a report yet; the generator arrives with the tooling track (`tools/`), not with the methodology. Reports are not governed evidence — never citable as the source of a SPEC, Bolt, ADR, US, TC or BUG (same class as derivative documents, §5.5). Reports follow the naming `REPORT-YYYY-Www.html` and may be archived under `_archive/` (§5.4). |
| `reviews/` | Optional, stakeholder-triggered Reviews (`REV-NNN`) of any functional or non-functional characteristic or artifact. Any team member or stakeholder may initiate one. Findings remain draft until `AITL-REV-Approval`, after which they may feed any DevFlow artifact; each affected artifact still follows its own lifecycle and approval. |
| `adversarial-reviews/` | Optional, stakeholder-triggered Adversarial Reviews (`AREV-NNN-<description>/`) composed of `01-CRITIQUE.md`, `02-DEFENSE.md`, and `03-VERDICT.md`. Each phase requires its corresponding AITL approval before the next begins. At each approved checkpoint, the human manually selects the agent/model for the next phase in the development tool. Each phase records its own agent/model; only an approved Verdict exposes actionable findings, and no AREV state or approval is written to a Bolt manifest. |
| `bugs/` | Defect records (`BUG-NNN`) that may be drafted by Functional Analysts, Developers, or QA. A functional BUG requires Functional Analyst `AITL-BUG-Approval`; a non-functional BUG requires `AITL-BUG-Approval` from an Architect or Tech Lead when `severity: critical`, otherwise from any team member, the BUG's own author included. Its exactly one dedicated Bolt may be drafted and refined alongside it; its readiness requires the BUG's approval, under the affected feature US or US-000 respectively. The BUG, Bolt, canonical SPEC, red→green evidence (in the V-Bounce's MEM or, for a defect that reproduces only in integration tests, also in its integration test runs), MEMs, and manifest remain bidirectionally traceable. |
| `incidents/` | Production incidents and blameless post-mortems (`INC-NNN`, severity `sev1`–`sev4`). Deployment-caused incident data feeds Change Fail Rate and Failed Deployment Recovery Time; production-fix deployments also support Deployment Rework Rate. Incidents link to the affected deployment and originating Bolts without forcing single-model attribution. |
| `risks/` | Project, technical, team, and dependency risks (`RISK-NNN`). Business risks originate in `analysis/business-risks/` and may feed this register when they affect delivery. |
| `retros/` | Weekly retrospectives (`RETRO-NNN-YYYY-Www`) driven by DORA, V-Bounce, and AITL evidence. Actions may adjust DoR, DoD, prompts, agent instructions, or quality gates through the applicable governance process. |

## 5.13 Human verification — `devflow/tests/`

Automated tests live with the source code and, when applicable, are generated
and executed by the agent during the V-Bounce. `devflow/tests/` contains
human-facing verification evidence only.

| Folder | Purpose |
|--------|---------|
| `tests/test-cases/` | Implementation-independent Test Cases (`TC-NNN-<description>.md`) derived from approved US/ACs and approved source Bolts, or from approved non-functional Bolts plus governed technical sources. Existing code may inform setup but never defines expected results. Each TC remains draft until `AITL-TC-Approval`; its 1..n Test Bolts may be drafted alongside it and approved only after it, named `TC-NNN.BOLT-NNN-<description>.md`. |
| `tests/uat/` | **Dormant/reserved (v4.2):** User Acceptance Testing minutes and stakeholder evidence (`UAT-NNN`). The UAT layer was removed from the active flow — release and customer acceptance follow the adopting team's own process — and the folder is kept for a redesigned model planned in a future version (§4.7). |

## 5.14 Methodology reference — `devflow/avenga-devflow/`

| File | Purpose |
|------|---------|
| `avenga-devflow/Avenga-DevFlow.md` | The complete methodology: foundations, glossary, principles, V-Bounce and Bolt rules, AITL governance, gates, metrics, manifest schema, language policy, end-to-end process, and repository structure. |
| `avenga-devflow/README.md` | A concise introduction and usage guide for the methodology documentation. |
| `avenga-devflow/INDEX.md` | The navigable index of the methodology content. |

## 5.15 Artifact routing summary

| Artifact | Canonical folder | Naming pattern |
|----------|------------------|----------------|
| Interview transcript | `input/interviews/` | `INT-NNN-<description>.md` |
| Introduction narrative (derivative, §5.5) | `analysis/introduction/` | `<feature-description>.md` (descriptive, no ID) |
| Business risk | `analysis/business-risks/` | `BR-NNN-<description>.md` |
| Open question | `analysis/open-questions/` | `OQ-NNN-<description>.md` |
| Business process | `analysis/process/` | `PROC-NNN-<description>.md` |
| Discovery investigation | `discovery/` | `DISC-NNN-<description>.md` |
| User Story | `functional/user-stories/` | `US-NNN-<description>.md` |
| Product Bolt | `functional/bolts/` | `US-NNN.BOLT-NNN-<description>.md` |
| Test Bolt | `functional/bolts/` | `TC-NNN.BOLT-NNN-<description>.md` |
| Architecture decision | `adrs/` | `ADR-NNN-<description>.md` |
| Implementation specification | `spec/` | `SPEC-YYMMDD-HHmm-<description>.md` |
| Implementation memory | `memory/` | `MEM-YYMMDD-HHmm-<description>.md` |
| US manifest | `metrics/user-stories/` | `US-NNN-<description>.json` |
| Bolt manifest | `metrics/bolts/` | `US-NNN.BOLT-NNN-<description>.json` or `TC-NNN.BOLT-NNN-<description>.json` |
| TC manifest | `metrics/test-cases/` | `TC-NNN-<description>.json` |
| Manifest JSON Schema | `metrics/` | `manifest-v5*.schema.json` |
| Sprint record (planning) | `sprints/` | `SPRINT-NNN-<description>.md` |
| Sprint report | `reports/` | `REPORT-YYYY-Www.html` |
| Project prompt | `prompts/` | `PROMPT-NNN-<description>.md` |
| Agent-run record | `agent-runs/` | `ARUN-YYMMDD-HHmm-<description>.md` |
| Actor file (roster member) | `actors/` | `<actor-id>.yaml` (kebab-case; listed in `roster.yaml`) |
| Live agent definition | `agents/squad/` | `<agent-id>/` (the folder is the unit: `agent.yaml` + `prompt.md`) |
| Project skill | `skills/` | `<skill-name>/` (the folder is the unit: `SKILL.md` + optional `references/`, `assets/`) |
| Formal review | `reviews/` | `REV-NNN-<description>.md` |
| Adversarial review | `adversarial-reviews/` | `AREV-NNN-<description>/` |
| Confirmed defect | `bugs/` | `BUG-NNN-<description>.md` |
| Production incident | `incidents/` | `INC-NNN-<description>.md` |
| Project risk | `risks/` | `RISK-NNN-<description>.md` |
| Retrospective | `retros/` | `RETRO-NNN-YYYY-Www.md` |
| Test Case | `tests/test-cases/` | `TC-NNN-<description>.md` |
| UAT evidence (dormant/reserved, §4.7) | `tests/uat/` | `UAT-NNN-<description>.md` |

`<description>` slugs in every filename follow the project's
`content_language` (kebab-case ASCII, no accents — §3.15); the IDs and
prefixes listed above are never translated or renamed.

Each documentary folder should include its own `README.md`, explaining local
purpose and conventions. Where a folder has none, **the parent folder's
README governs it** — as `functional/README.md` does for `user-stories/` and
`bolts/`.

**Whether a folder has an `INDEX.md` follows one rule:** an INDEX exists where
the ID is **sequential and needs a central allocator**, does not exist where
the **timestamp already assigns and orders it** or the contents are
machine-generated, and exists as a **curated inventory** where the folder
holds a small manually maintained set of documents without IDs; two curated
inventories, `prompts/` and `agent-runs/`, also claim the next number of a
sequence (below).

- **With INDEX — sequential `NNN` (allocator):** US, Bolt, TC, ADR, BUG, DISC,
  REV, RISK, INC, RETRO, UAT, OQ, INT, BR, PROC, AREV, SPRINT. The INDEX is where the
  next free number is claimed, which makes it also the place a duplicate claim
  surfaces: two branches taking the same `NNN` collide there as a merge
  conflict. That is the intended behaviour — an ID collision must be visible,
  never silent.
- **Without INDEX — timestamp IDs:** `spec/` (`SPEC-YYMMDD-HHmm`) and
  `memory/` (`MEM-YYMMDD-HHmm`). The timestamp assigns and orders every SPEC
  and MEM on its own, so no allocator is needed and **no shared file is edited
  on every V-Bounce** — an INDEX here would conflict on every concurrent
  branch while detecting nothing, because timestamps do not collide. The
  same-minute edge case is handled by the naming rules themselves (§2.4.1,
  §2.12). The file listing is the inventory. `agent-runs/` also names its
  files by timestamp (`ARUN-YYMMDD-HHmm`) but keeps an INDEX, for the run
  number its records carry — see the curated inventories below.
- **Without INDEX — unstructured or machine-readable:** `input/` (except
  `interviews/`, which allocates sequential `INT-NNN` and therefore has one,
  and `documentation/`, which keeps a curated inventory — see below),
  `metrics/` (inventory = file system) and `reports/` (generated HTML).
- **With INDEX — curated inventory (no IDs):** the `analysis/` families
  without sequential IDs — business-context, domain-model, glossary,
  introduction, personas, scope, ui, user-journeys, vision —,
  `input/documentation/`, `avenga-devflow/` (the methodology itself:
  a curated inventory of the current version), the **living-data
  families** — `actors/`, `agents/`, `skills/` and `prompts/` — and
  `agent-runs/`. The first three living-data families are inventories of
  folders and roster members, not of sequential IDs — the membership
  authority for actors is `roster.yaml`, not the INDEX. Their INDEX is a
  **manually maintained inventory**: it lists each document — with its
  `status` where its family has one (§3.15) — and is rebuilt by listing the
  folder, never from memory. It allocates no IDs and acts as no allocator,
  with **two exceptions**, each an inventory that also claims the next
  number: `prompts/`, whose INDEX allocates the sequential `PROMPT-NNN`; and
  `agent-runs/`, whose file IDs are timestamps (`ARUN-YYMMDD-HHmm`, assigned
  and ordered by the clock like `spec/` and `memory/`) but whose records are
  numbered in sequence — its INDEX claims the next run number (`run: N`),
  which a timestamp cannot give, and a number once claimed is never reused.
  In both, a duplicate claim surfaces in the INDEX as a merge conflict, like
  any allocator's.
- Subfolders covered by their parent's INDEX (`functional/user-stories/` and
  `functional/bolts/` → `functional/INDEX.md`) do not repeat it.

Before creating or routing an artifact, both humans and agents must read the
target folder's `README.md`, or the parent's where the folder has none.

## 5.16 Methodology version upgrade (migrating an existing project)

`devflow/VERSION` declares the **single** methodology version the whole
project runs under. Governed artifacts carry no per-artifact version field:
there is one version, and it is the current one. When a project adopts a newer
methodology version, its existing documentation is therefore **migrated
forward** so the repository stays coherent under that single version. The
upgrade is an explicit activity — normally AI-assisted — not something that
happens by editing `VERSION`.

**What the migration updates:** the framework documentation as instantiated in
the project — every folder `README.md` and `INDEX.md`, including its
`**Methodology version:**` header (§5.15); the `TEMPLATE-*` files; and the
structure of governed documents wherever the new version changed a required
section, a frontmatter key or a `status` value (§3.15).

**What a migration never touches — history is not documentation** (G36):

| Never rewritten | Why |
|-----------------|-----|
| Approved MEMs | Immutable after the `AITL-MEM-Approval` decision (§2.12). |
| Approved ADRs | Read-only once accepted; a changed decision is a new superseding ADR (§3.5). |
| Recorded AITL decisions and `review:` contracts | Approval evidence describes what happened, not what the current version prescribes (§3.0). |
| `CHANGELOG.md` history | A changelog is a record; correcting past entries would falsify it. |
| `input/` | Human-deposited raw evidence, read-only for agents (§5.6, G31). |

`metrics/**/*.json` is **not** on that list: manifests move forward with the
repository, and the values they already carry are what the guardrail protects.
See *"Manifests are migrated, never frozen"* below.

**Rules:**

- `devflow/VERSION` is updated **last**, once every other change is applied. A
  repository whose `VERSION` already claims the new version while its
  documents still describe the old one is worse off than one that has not
  started.
- A migration **never changes an approval outcome**. An artifact approved
  under an earlier version stays approved. If the new version adds a required
  checkpoint, it applies to work started after the upgrade — never
  retroactively to closed history.
- The migration is a change to governed documentation and is reviewed by a
  human before it is committed, like any other change.
- The upgrade is recorded in the project's own `CHANGELOG.md` **at the
  repository root**, naming the source and target versions. DevFlow ships no
  `CHANGELOG.md` of its own: the framework's history lives in the methodology's
  repository, and a project that has no changelog yet creates one.

**The migration procedure.** The upgrade runs inside the project repository,
with the two folders side by side: the existing `devflow/` renamed to
`devflowOLD/`, and the new version installed as `devflow/`. Install the new
agent definitions first — the migration is executed by the **new** agent,
because it is the one that knows the new structure.

What moves is defined by an **allowlist of origin**, never by comparing the
two folders file by file:

1. **`input/` in full — 100%, exactly as it stands, byte for byte — with one
   carve-out: the scaffolding the kit itself installs inside `input/`** (its
   `README.md` and `INDEX.md` files — the same framework files every other
   folder carries) **comes from the new version**, exactly like the
   README/INDEX of any other folder (below). If the project modified one of
   those scaffolding files, the modification is project content and the whole
   file is treated as evidence, preserved byte for byte like any other file
   in `input/`. Every other file in `input/` is raw evidence: never
   normalized, reorganized, filtered, renamed or partially copied (§5.6,
   G31). Afterwards, verify that `devflow/input/` and `devflowOLD/input/`
   have the identical tree and the identical file count, and that every
   non-scaffolding file is byte-identical — a byte-level diff of the
   evidence (e.g. `git diff --no-index devflowOLD/input devflow/input`) must
   show differences only in the scaffolding files. Any difference in an
   evidence file means the copy lost data and the migration stops.
2. **Every file the project created.** Two signals identify one: it carries an
   **artifact ID** from the naming table (§5.15, N01–N29) — `US-NNN`,
   `US-NNN.BOLT-NNN`, `TC-NNN.BOLT-NNN`, `SPEC-YYMMDD-HHmm`, `MEM-YYMMDD-HHmm`,
   `ADR-NNN`, `BUG-NNN`, `TC-NNN`, `DISC-NNN`, `REV-NNN`, `INC-NNN`,
   `RISK-NNN`, `RETRO-NNN`, `UAT-NNN`, `OQ-NNN`, `BR-NNN`, `PROC-NNN`,
   `INT-NNN`, `PROMPT-NNN`, `ARUN-YYMMDD-HHmm`, `SPRINT-NNN`, the manifest
   `.json` files, `REPORT-YYYY-Www.html` — **or** it
   lives in an area the project created: `agents-data/<agent>/`,
   `adversarial-reviews/AREV-NNN-<description>/`, any `_archive/`, and the
   **living-data families the project fills**: `actors/<actor-id>.yaml`,
   `agents/squad/<agent-id>/` and `skills/<skill-name>/` — the last two
    **excluding `agents/examples/` and `skills/examples/`**, which are the
    shipped shelves — framework content, refreshed by the new version like
    every framework file — **or**
   it is an ID-less document of an `analysis/` family (business-context,
   domain-model, glossary, introduction, personas, scope, ui, user-journeys,
   vision, §5.15). ID-less documents move with their folder and are placed at
   the destination **by family**, never by the folder they sat in (below).

**Everything else comes from the new version.** Folder `README.md` and
`INDEX.md` files, every `TEMPLATE-*`, the manifest schemas, `GUARDRAILS.md`,
`ONBOARDING.md`, `avenga-devflow/`, `US-000-non-functional.md` and **`bin/`**
(compiled tooling executables, the only compiled, executable content of the
distributable) are never copied forward. The `input/` scaffolding
README/INDEX files are covered by this rule with the carve-out stated in
rule 1: they are replaced by the new version unless the project modified
them — in which case they are evidence and are preserved byte for byte.
This is
what lets the procedure survive future versions without being rewritten, and
what makes it impossible to resurrect a file a new version deliberately
removed.

**Framework-shipped, project-configured files keep their old value.** A small
class of files ships with the framework but holds a value the **project**
supplies. Copying the new version over one of them does not refresh a
framework file — it silently discards the project's own configuration, and
because the shipped state is always valid the loss produces no error and no
blocked checkpoint. The class is **enumerated, never inferred**:

| File | What the project supplies | What a blind copy would do |
|------|---------------------------|----------------------------|
| `devflow/LANGUAGE` | the project's `content_language` | the agent starts writing prose in the wrong language |
| `devflow/actors/roster.yaml` | the team — every actor entry, including every DevFlow Agent approver | every AI approver silently reverts to unconfigured; the safe default renders that as pure HITL, so nothing fails visibly (the roster **is** the enablement) |

**A file is in this class only if it is listed above.** The enumeration is the
whole rule: a framework file that merely *looks* configurable is not in it,
and adding one is a change to this section. `VERSION` is the class's mirror —
it also ships with the framework, but it takes the **new** value, and it is
written last.

**`devflow/CHANGELOG.md` no longer exists.** Versions up to 4.1 installed one
inside `devflow/`, which meant every project carried the framework's own
development history and agents wrote project entries into it. This version
ships none: a changelog belongs to the repository, not to the framework. In an
upgrade from a version that still had one, the old file is **superseded** —
with one step before it is discarded: if the project wrote its own entries
there (upgrade records, local notes), those entries move to the
repository-root `CHANGELOG.md`. Moving them preserves the record; dropping
them would be the G36 violation.

**The root `AGENTS.md` is merged, never replaced.** It is the one installed
file with two owners: the framework writes everything above its
`AVENGA-DEVFLOW:PROJECT-SECTION` marker, and the project writes everything from
the marker onward — which is exactly what §5.2 invites it to do. An upgrade
that copies the new version over it destroys that section silently, so the
migration **merges** instead: take the new version's text up to its marker,
keep the existing file's text from its own marker onward, byte for byte.

**The install step excludes `AGENTS.md` from the copy.** The safest merge is the
one that never has to recover anything: when the new version is installed over
the project root, this one file is left out of the copy and the new framework
block is merged into it **in place**. Nothing the project authored is destroyed
at any point, so the guarantee stops depending on anyone having taken a
precaution first.

**Recovering from the last commit is the fallback, not the mechanism.** A
project that installs by copying everything — the natural thing to do — will
find the file already overwritten. In that case the previous content is read
from the **last commit** rather than from the working tree, and the merge is
still correct. This path exists because that copy will happen; it is the
second-best outcome, not the prescribed one.

**Precondition: the working tree is committed before a migration runs.** The
fallback reads from the last commit, so a project whose section was never
committed has nothing there to recover: the migration finds no marker, stops,
and the content is unrecoverable from the repository. Committing first is part
of the procedure, not a good habit around it.

The result is checkable, and the check is part of the migration rather than an
afterthought: after the merge, the text above the marker is byte-identical to
the new version's, and a diff of the file shows **no change from the marker
onward**. A difference below it means the merge failed and the migration stops.

Two boundary cases are resolved rather than guessed. If the **existing** file
carries no marker — it predates this rule, or someone removed it — the
migration **stops and reports it**: the boundary between framework and project
text is not inferable, and a human places it (§5.16, *stop and ask, never
guess*). If the file carries the marker **more than once**, the same applies.
The marker is matched as a **prefix** of the comment, so a version that
rewords the human-facing explanation beside it does not break the merge for
projects already carrying it.

The platform agent definitions have no such split: `CLAUDE.md`,
`.agents/skills/`, `.github/agents/` and `.opencode/agents/` are pure framework,
carry nothing the project authored, and are **overwritten** from the new
version like any other framework file.

**An artifact's destination is derived, never inherited.** Place each copied
file by its ID against the routing table (§5.15), not by where it sat in
`devflowOLD/`. A version that relocated a family therefore lands correctly
without any special-casing. **A document with no ID is placed by its family,
not by its old folder** — the ID-less `analysis/` families (§5.15) are routed
by what the document *is*, so a version that splits one family out of another
lands those documents in the new folder rather than carrying them along in the
old one.

**Manifests are migrated, never frozen.** `metrics/**/*.json` moves forward
like every other project file. Within the same major, they advance **intact**:
`schema_version` is the `<major>.0` of the family (§3.12), so a 4.x bump
changes no manifest value at all and no conversion runs. A migration may still
**record a new fact** in a field the schema added as optional — the Bolt's
confirmed `test_integration`, below: it adds a field and changes no recorded
value. Across a major, the migration is what makes them valid again: each
manifest is **re-routed** to the folder its family now uses (§5.15) and
**converted** to the new family's `schema_version`, so that the repository
ends the upgrade holding exactly one manifest family (§3.12). A repository
whose documents describe the new version while its manifests declare a
`schema_version` from another family has an unfinished migration, not
preserved history.

Conversion follows the lossless rule of §3.12: add the fields the new schema
introduces — `null` wherever the value was never captured and where the schema
admits it — apply its renames, and carry every recorded value across
untouched. `3.0` → `4.0` is exactly this shape: the timing fields
(`review_ready_at`, `review_started_at` and the Bolt's `acceptance` object)
did not exist in `3.0`, so `review_ready_at` and `review_started_at` arrive as
`null` and the `acceptance` object arrives with both of its own fields `null`
(the object itself is never `null`), and the manifest moves from the
`metrics/` root to `metrics/bolts/`. Every value the manifest already recorded
under `bolt`, `spec_revisions[]`, `v_bounces[]` and `hitl_approvals[]` crosses
unchanged. Anything that cannot be converted under that rule is
**unresolved** — reported, never guessed (G36).

`4.0` → `5.0` is a rename with a shape change: the approval array
`hitl_approvals[]` becomes `checkpoint_approvals[]` with a richer approver.
Every `4.0` approval was recorded by a human (agents did not exist in `4.0`), so
each entry converts its `decided_by` `{user, role}` → `{actor: "human:<user>",
role, model: null}`; likewise `created_by` becomes `human:<user>` and every
`runs[]` entry gains `agent: null` — the actor grammar (§3.0).
The checkpoint **name** in each converted entry is **re-expressed** in the
  current vocabulary (`HITL-<CODE>-Approval` → `AITL-<CODE>-Approval`): the decision — actor, timestamp, outcome — is immutable, only the
vocabulary label tracks the version, and because AITL ⊇ HITL the label becomes
truer, not falser (a historical `HITL-MEM-Approval` *is* an `AITL-MEM-Approval`
in human mode). The v5 `checkpoint` enum accepts **only** `AITL-*`; a v4 manifest
validates against the frozen v4 schema, which keeps `HITL-*`. G36 still forbids
altering the recorded actor, timestamp, outcome or evidence, or rewriting an
approved MEM/ADR body. `schema_version` becomes `"5.0"`; every other value
crosses unchanged.

**A manifest level the old version lacked is reconstructed from the
repository's own evidence.** A new version may introduce a level that did not
exist before — `4.0` added the US and TC manifests to a family that had only
Bolts — and G33 makes an artifact without its manifest nonexistent. Leaving
those gaps would render every migrated US and TC formally nonexistent the
moment `VERSION` is written, so the migration creates them, deriving every
field from something already in the repository:

| Field | Read from |
|-------|-----------|
| `id`, `ref` | the artifact's own ID and its path under the new routing table |
| `sources` | the document's frontmatter `sources:` (for a non-functional TC, `governing_sources:`) |
| `generation.created_at` | the commit that added the document; when the file was never committed, the frontmatter `date:` as recorded — a value already in RFC 3339 with seconds and a zone (§3.12) as it stands, a plain `YYYY-MM-DD` as that day at `00:00:00` with the migration's UTC offset (`2026-08-02` → `2026-08-02T00:00:00-03:00` under `-03:00`), any other form establishing no `created_at`. The offset is stated once by the human running the migration (the project's working offset, for instance): the agent stops and asks for it, never guesses it. The `00:00:00` is a declared convention, not an observed time: it establishes `created_at` only when it falls at or before every review timestamp the manifest records (`review_ready_at`, `review_started_at`, each `decided_at`), compared as instants (§3.12); otherwise the manifest is unresolved. Every manifest that used this fallback is listed in the report |
| `generation.created_by` | the document's frontmatter `author:`, as `human:<author>` (actor grammar, §3.0) |
| `generation.runs` | `[]` — no generation telemetry was recorded, which is what an empty `runs[]` already means (§3.12) |
| `generation.duration_seconds` | `null` |
| `review_ready_at`, `review_started_at` | the frontmatter `review_ready_at:` and `review.started_at` |
| `checkpoint_approvals[]` | the document's `review:` contract, through the standard manifest projection (§3.0) — reviewers as `human:<user>` actors |
| `story_points` (US) | the frontmatter `story_points:` |
| `bolts[]` (US), `test_bolts[]` (TC) | the Bolt manifests already converted in this migration |
| `verifies` (TC) | the frontmatter `source_bolt:`, `source_us:` and `covered_acs:` |

Every one of those, except the declared midnight, is transcription. A field
with no such source — a document carrying no `sources:`, an artifact whose
`created_at` cannot be established —
makes that manifest **unresolved**: it is listed in the report with the field
that is missing, and the human supplies it. Reconstructing what the repository
records is part of the migration; inventing a value it does not is the thing
G36 exists to stop.

**A changed Bolt `status` vocabulary is mapped once, never guessed.** When the
new version declares the Bolt's `status` with the transition table of §3.2.3
and the old one had only `draft` · `approved` · `deprecated` (with a
development state derived from the manifests), every **active** Bolt —
outside `_archive/` — gets its new `status` **derived once** from its
recorded facts by the table below, and then **declared**. The first matching
row applies; rows M3–M14 apply to a Bolt whose old `status` is `approved`.

| # | Recorded facts | New `status` |
|---|---|---|
| M1 | `status: draft` | `draft` |
| M2 | `status: deprecated` | `deprecated` |
| M3 | The latest `AITL-BOLT-DONE-Approval` is `approved` | `done` |
| M4 | The latest `AITL-BOLT-DONE-Approval` is `rejected` | **unresolved** — the human deprecates the Bolt with its reason (§3.2.3) |
| M5 | No SPEC revision, or the latest revision has no `approved` `AITL-SPEC-Approval` | `ready` |
| M6 | No V-Bounce has executed the latest approved revision | `specified` |
| M7 | The latest V-Bounce is `blocked` and has no `AITL-MEM-Approval` decision | `blocked` |
| M8 | The latest V-Bounce has no decision, or `changes_requested` or `rejected` | `in-development` |
| M9 | The latest V-Bounce is `approved` and its SPEC's integration test plan is `n/a`, `waived` or absent (the SPEC predates the plan) | `ready-for-acceptance` |
| M10 | The latest V-Bounce is `approved`, plan `applicable`, and no integration test run, classification or waiver recorded after that approval | `in-integration-testing` |
| M11 | Same, and the latest such entry is a run `pass` or a late waiver | `ready-for-acceptance` |
| M12 | Same, and the latest such entry is a run `fail` | `integration-tests-failed` |
| M13 | Same, and the latest such entry is a run `blocked` or a classification `environment` | `integration-tests-blocked` |
| M14 | Same, and the latest such entry is a classification `code`, `test` or `spec` | `in-development` |
| M15 | No manifest; an old `status` outside `draft` · `approved` · `deprecated`; or facts that match no row or contradict each other (e.g. an `approved` MEM on a V-Bounce that did not reach review, or a latest V-Bounce on an older revision than the latest approved one) | **unresolved** — the human supplies the value |

**The `test_integration` field is set in the same pass.** Every active Bolt,
terminal ones included, gets `test_integration` and `test_integration_reason`
once, from its recorded facts. Rows I1–I4 are tried in order and the first
match gives the value; then, whatever row gave it, I5 applies.

| # | Recorded facts | `test_integration` | `test_integration_reason` |
|---|---|---|---|
| I1 | `bolt_type: test` | `not-required` | `Test Bolt — executed in the TC's run cycle` |
| I2 | The latest approved SPEC revision's integration test plan is `applicable` or `waived` | `required` | `Migration — from the SPEC's plan (<value>)` |
| I3 | The latest approved SPEC revision's plan is `n/a`, or it has none (the SPEC predates the integration test plan) | `not-required` | `Migration — from the SPEC's plan (n/a)` or `Migration — SPEC predates the integration test plan` |
| I4 | No approved SPEC revision | the rubric of §3.2.2, applied by the migrating agent | the rubric's reason, **reported for confirmation** |
| I5 | The value contradicts the new `status` (`not-required` on a Bolt mapped to `in-integration-testing`, `integration-tests-failed` or `integration-tests-blocked` — C10) | **unresolved** — the human decides (for an active Bolt, through a SPEC revision) | — |

The human confirms the I4 values when reviewing the migration report, like
every proposed value of this procedure; a confirmed value needs no new
`AITL-BOLT-READY-Approval` — nothing approved is rewritten — and a changed one
is written before the Bolt's first SPEC. A Bolt mapped by I2 or I3 is coherent
with its governing SPEC by construction (§3.2.2).

Each mapped Bolt document gains its `## Status timeline` section **seeded with
one row**: `—` → the new value, *Act / evidence*
`Migration — <version> (§5.16): derived once from <the facts of the matching row>; test_integration <value> (<I-row>)`,
dated at the migration. Earlier history is **not reconstructed**: the manifest
still holds the earlier facts, and back-filled rows would date acts the old
version never recorded as status changes. A Bolt unresolved by M4, M15 or I5
is listed in the migration report with its row, keeps its old `status` until
the human supplies the value, and is seeded then. Nothing recorded is
rewritten: the old `status` is a document state, not a decision, and every
approval, MEM and manifest value stays untouched (G36).

**The Bolt manifest records the field in the same act.** For every Bolt
seeded above, the migration writes `bolt.test_integration` in the Bolt's
manifest with the value the Bolt received, in the same act as its seed row
(§3.12) — an added fact, never a rewritten value, so nothing recorded
changes (G36). A Bolt seeded later, once the human supplies its value, gets
it then.

**Archived Bolts are not migrated.** A Bolt under `_archive/` has a closed
lifecycle (G38): it is copied forward byte for byte, keeps its `status`, no
timeline and no `test_integration` field — and its manifest no
`bolt.test_integration`, absent meaning "not recorded" (§3.12) — and is read
under the vocabulary of the version that closed it, its closure attested by
its own acceptance or deprecation record.

**Rebuild every `INDEX.md` after the copy, from the migrated files
themselves** — never from the old INDEX, which may itself be stale. Every
artifact that arrived from `devflowOLD/` appears in the INDEX of its folder,
classified by reading its own frontmatter `status` against the new version's
section structure and status vocabulary (§3.15) — an agent-run record, which
has none, is listed without one. An INDEX left with the
template's empty placeholder rows while its folder holds migrated artifacts is
an unfinished migration.

**Numbering continuity is part of the rebuild.** The INDEX is where the next
free `NNN` is claimed (§5.15), so a rebuilt INDEX must leave each sequence
exactly where the project left it: the next number continues from the
**highest migrated ID** of that family, existing gaps stay as gaps, and no ID
is ever reused or renumbered (§2.4). An INDEX rebuilt as if the folder were
new would hand out a number the project has already spent. In `agent-runs/`
the sequence is the `run: N` the records carry, not their timestamp IDs: the
rebuilt INDEX continues from the highest migrated run number (§5.15).

**Then report, before the human reviews:** how many files were copied per
family; how many manifests were converted forward and how many were
reconstructed for a level the old version lacked; the UTC offset the human
stated for the `date:` fallback, if one was asked for, and every reconstructed
manifest whose `generation.created_at` came from the frontmatter `date:`, with
the `date:` it read; every `INDEX.md` rebuilt and with how many entries; the
`LANGUAGE` value preserved; and, explicitly, **anything that could not be
classified or converted**.

**Stop and ask, never guess.** A file in `devflowOLD/` that matches neither
signal above is not silently dropped and not silently copied: it is listed in
the report and the human decides. The same applies to an ID that collides with
one already present in the new folder.

**Reconciliation — nothing is lost.** Copying is not the last step. Walk
`devflowOLD/` in full and give **every file and every folder** exactly one
disposition:

| Disposition | Meaning |
|-------------|---------|
| **copied** | matched the allowlist and now exists under `devflow/` — for a manifest, re-routed and converted to the current `schema_version` |
| **superseded** | a framework file the new version replaces **or removes** (`README.md`, `INDEX.md`, `TEMPLATE-*`, schemas, `GUARDRAILS.md`, `ONBOARDING.md`, `avenga-devflow/`, `US-000-non-functional.md`, `VERSION`, and `CHANGELOG.md` when upgrading from a version that still shipped one) |
| **unresolved** | matched neither, or is a manifest whose conversion or reconstruction lacks a value the repository does not record — the human decides, one by one |

The report states the three counts, and they must **sum to the total number of
files in `devflowOLD/`**. A migration that cannot account for every file is
not finished, and an unresolved file is never resolved by the agent's
judgement. Folders count too: a directory present in `devflowOLD/` and absent
from `devflow/` is reported, never silently discarded.

**The files outside `devflow/` are reconciled too.** The walk above cannot see
them — they never lived inside `devflowOLD/` — so they are accounted for
separately, and the report states what happened to each:

| Installed file | Disposition |
|----------------|-------------|
| The platform agent definitions (`CLAUDE.md`, `.agents/skills/`, `.github/agents/`, `.opencode/agents/`) | **overwritten** from the new version — pure framework |
| `AGENTS.md` at the repository root | **merged** at its `AVENGA-DEVFLOW:PROJECT-SECTION` marker — framework above replaced, project section below preserved byte for byte |

Anything else the project keeps at its repository root is the project's own and
the migration does not touch it.

`devflowOLD/` is removed only after the human has reviewed the migration and
the upgrade entry is recorded in the repository-root `CHANGELOG.md`.

---

# References

- **AI-Driven Development Life Cycle: Reimagining Software Engineering,** Raja SP, Principal Solutions Architect at AWS.
  <https://aws.amazon.com/es/blogs/devops/ai-driven-development-life-cycle/>

- **Amershi, S., Begel, A., Bird, C., DeLine, R., Gall, H., Kamar, E., Nagappan, N., Nushi, B., & Zimmermann, T.** (2019). Software engineering for machine learning: A case study. *2019 IEEE/ACM 41st International Conference on Software Engineering: Software Engineering in Practice (ICSE-SEIP)*, 291–300. IEEE. <https://doi.org/10.1109/ICSE-SEIP.2019.00042>

- **Butler, J., Suh, J., Haniyur, S., & Hadley, C.** (2024). *Dear Diary: A randomized controlled trial of generative AI coding tools in the workplace*. arXiv. <https://arxiv.org/abs/2410.18334>

- **Cucumber.** (2025, January 26). *Gherkin reference*. <https://cucumber.io/docs/gherkin/reference/>

- **DevOps Research and Assessment (DORA).** (2024). *Accelerate State of DevOps Report 2024*. <https://dora.dev/research/2024/dora-report/>

- **DevOps Research and Assessment (DORA).** (2026). *DORA's software delivery performance metrics*. <https://dora.dev/guides/dora-metrics/>

- **DevOps Research and Assessment (DORA).** (2024). *A history of DORA's software delivery metrics*. <https://dora.dev/insights/dora-metrics-history/>

- **Forsberg, K., & Mooz, H.** (1992). The relationship of systems engineering to the project cycle. *Engineering Management Journal, 4*(3), 36–43. <https://doi.org/10.1080/10429247.1992.11414684>

- **Forsgren, N., Humble, J., & Kim, G.** (2018). *Accelerate: The science of lean software and DevOps—Building and scaling high-performing technology organizations*. IT Revolution. <https://itrevolution.com/product/accelerate/>

- **GitHub.** (2022, September 7). *Research: Quantifying GitHub Copilot's impact on developer productivity and happiness*. <https://github.blog/news-insights/research/research-quantifying-github-copilots-impact-on-developer-productivity-and-happiness/>

- **GitHub.** (2026). *GitHub Copilot code referencing*. <https://docs.github.com/en/copilot/concepts/completions/code-referencing>

- **Black Duck.** (2025, February 3). *Analyze AI-generated code with the Black Duck Snippet API*. <https://www.blackduck.com/blog/analyze-ai-generated-code-black-duck-snippet-api.html>

- **International Council on Systems Engineering (INCOSE).** (2015). *INCOSE systems engineering handbook: A guide for system life cycle processes and activities* (4th ed.). Wiley. <https://www.wiley.com/en-ie/INCOSE+Systems+Engineering+Handbook>

- **International Organization for Standardization, International Electrotechnical Commission, & Institute of Electrical and Electronics Engineers.** (2017). *ISO/IEC/IEEE 12207:2017—Systems and software engineering—Software life cycle processes*. <https://www.iso.org/standard/63712.html>

- **METR (Model Evaluation & Threat Research).** (2025, July 10). *Measuring the impact of early-2025 AI on experienced open-source developer productivity*. <https://metr.org/blog/2025-07-10-early-2025-ai-experienced-os-dev-study/>

- **Mosqueira-Rey, E., Hernández-Pereira, E., Alonso-Ríos, D., Bobes-Bascarán, J., & Fernández-Leal, Á.** (2023). Human-in-the-loop machine learning: A state of the art. *Artificial Intelligence Review, 56*(4), 3005–3054. <https://doi.org/10.1007/s10462-022-10246-w>

- **Nygard, M.** (2011, November 15). *Documenting architecture decisions*. Cognitect Blog. <https://www.cognitect.com/blog/2011/11/15/documenting-architecture-decisions>

- **Peng, S., Kalliamvakou, E., Cihon, P., & Demirer, M.** (2023). *The impact of AI on developer productivity: Evidence from GitHub Copilot*. arXiv. <https://arxiv.org/abs/2302.06590>

- **Schäfer, M., Nadi, S., Eghbali, A., & Tip, F.** (2023). *An empirical evaluation of using large language models for automated unit test generation*. arXiv. <https://arxiv.org/abs/2302.06527>

- **Schwaber, K., & Sutherland, J.** (2020, November). *The Scrum Guide*. <https://scrumguides.org/docs/scrumguide/v2020/2020-Scrum-Guide-US.pdf>

- **Souppaya, M., Scarfone, K., & Dodson, D.** (2022, February). *Secure Software Development Framework (SSDF) Version 1.1* (NIST SP 800-218). National Institute of Standards and Technology. <https://csrc.nist.gov/pubs/sp/800/218/final>

- **Yang, L., Chen, J., Zhao, Y., & Movaghar, A.** (2024). *Unit test generation with large language models*. arXiv. <https://arxiv.org/abs/2406.18181>

- **National Institute of Standards and Technology.** (2023). *Artificial Intelligence Risk Management Framework (AI RMF 1.0)* (NIST AI 100-1). <https://doi.org/10.6028/NIST.AI.100-1>

- **European Union.** (2024). *Regulation (EU) 2024/1689 (Artificial Intelligence Act)*. Official Journal of the European Union. <https://eur-lex.europa.eu/eli/reg/2024/1689/oj>

- **Schuett, C.** (2026, March). *Adversarial coding — Using competing models as code reviewers*. SubAud.io. <https://www.subaud.io/adversarial-coding-competing-models-reviewers/>

- **GitHub.** (2024). *GitHub Copilot Workspace technical preview — design & evaluation notes*. <https://githubnext.com/projects/copilot-workspace>

- **Anthropic.** (2024). *Building effective agents*. <https://www.anthropic.com/research/building-effective-agents>
