# Avenga DevFlow — Development Flow

**Methodology version:** 5.2

## Purpose

This folder is the **single source of truth** for all project documentation —
business context, analysis, architecture decisions, implementation specs,
reviews, and execution memory. It embodies *Documentation as Code*: the docs
live with the code, are versioned with Git, and are maintained as part of the
normal workflow.

Everything starts in `input/` (raw material from the client or legacy system)
and flows through analysis, discovery, functional definition, architecture
decisions, implementation specs, and finally captured knowledge in `memory/`.

> **Normative hierarchy:** the methodology in
> [`avenga-devflow/Avenga-DevFlow.md`](avenga-devflow/Avenga-DevFlow.md) is the
> single source of truth. §2 owns concepts and artifact contracts; §3 owns
> lifecycle, AITL, gates, autonomy and metrics; §4 is an explanatory
> walkthrough; §5 owns structure, locations and names. When repeated text
> appears to diverge, the section that owns that dimension governs.

---

## Folder Map

```
devflow/
├── input/              ← Raw entry material (read-only, as received)
│   ├── business/           RFPs, BRDs, SOWs, compliance, regulations
│   ├── databases/          Legacy DB schemas, DDL, ER diagrams
│   ├── documentation/      Third-party docs: API manuals, datasheets, vendor PDFs
│   ├── interviews/         Stakeholder conversation transcriptions (INT-NNN)
│   ├── source-code/        Legacy source code and configs
│   └── ui-ux/              Screenshots, mockups, brand guidelines, UX research
│
├── analysis/           ← Domain analysis
│   ├── introduction/       Plain-language feature narratives (derivative, non-governed)
│   ├── vision/             Product vision and strategy
│   ├── business-context/   Business objectives, constraints, market context
│   ├── business-risks/     Pre-execution business risks (BR-NNN)
│   ├── scope/              Scope and phasing decisions per milestone
│   ├── personas/           User archetypes
│   ├── user-journeys/      End-to-end user experience maps
│   ├── glossary/           Ubiquitous language terms
│   ├── domain-model/       Entity definitions, relationships, enums
│   ├── ui/                 Surfaces, patterns, states and parity contracts
│   ├── process/            Business processes (BPMN/Mermaid, PROC-NNN)
│   └── open-questions/     Centralized backlog of analysis questions (OQ-NNN)
│
├── discovery/          ← Need-driven investigations (DISC-NNN, AITL-DISC-Approval)
├── functional/         ← User Stories + Bolts (WHAT to build)
│   ├── user-stories/       Feature US (US-NNN) + US-000-non-functional.md container
│   └── bolts/              Functional (US-NNN.BOLT-NNN) · non-functional (US-000.BOLT-NNN) · Test (TC-NNN.BOLT-NNN)
├── actors/             ← Who is in the team: the roster home (humans + DevFlow Agents as actors; roster.yaml)
├── agents/             ← DevFlow Agent definitions: examples/ · squad/ (live agents) · TEMPLATE-new-role/ · VERIFICATION.md
├── skills/             ← Project-level skills (SKILL.md folders, scope shared|role; examples/ = the shipped shelf, never projected until adopted) referenced from the agents' skills: lists
├── adrs/               ← Architecture Decision Records (ADR-NNN, AITL-ADR-Approval)
├── spec/               ← One canonical SPEC per Bolt (SPEC-YYMMDD-HHmm, AITL-SPEC-Approval)
├── memory/             ← One MEM per V-Bounce (MEM-YYMMDD-HHmm, AITL-MEM-Approval)
├── metrics/            ← Manifest family v5 (bolts/ · user-stories/ · test-cases/) + schemas
├── sprints/            ← Sprint records (SPRINT-NNN, one planning record per sprint; declared status)
├── reports/            ← Sprint progress reports (PM; generator planned, tools track)
├── tests/              ← Human-facing verification
│   ├── test-cases/         Test Cases (TC-NNN, AITL-TC-Approval)
│   └── uat/                UAT minutes (UAT-NNN) — dormant/reserved
├── reviews/            ← Stakeholder-triggered reviews (REV-NNN, AITL-REV-Approval)
├── adversarial-reviews/← LLM-vs-LLM debates (AREV-NNN: Critique → Defense → Verdict)
├── bugs/               ← Confirmed defects (BUG-NNN, AITL-BUG-Approval)
├── incidents/          ← Production incidents & post-mortems (INC-NNN)
├── risks/              ← Risk register (RISK-NNN)
├── retros/             ← Weekly retrospectives (RETRO-NNN)
├── agents-data/        ← Per-agent shared knowledge (each agent creates its own folder)
├── prompts/            ← Project prompts (PROMPT-NNN, copy-paste ready)
├── agent-runs/         ← Agent-run records (ARUN-YYMMDD-HHmm, one per agent run; never governed input)
├── bin/                ← Compiled tooling executables (optional; replaced on upgrade, §5.16)
├── avenga-devflow/     ← Methodology reference (single source of truth)
├── ONBOARDING.md       ← Onboarding guide for new team members
└── GUARDRAILS.md       ← Agent-enforced rules (AITL stops, naming, traceability)
```

Alongside `devflow/`, the repository root also carries **`AGENTS.md`** — the
cross-tool entry point several agents auto-load — and the **agent definition
for the tool your team uses**, at the location that tool expects (`CLAUDE.md`
at the root, `.github/agents/`, `.opencode/agents/`, `.agents/skills/`). Both
are installed from the Avenga DevFlow distribution (§5.2).

---

## The Complete Flow

```mermaid
flowchart TB

    subgraph P1["📥 INPUT"]
        direction LR
        IN["input/<br>interviews · business · DBs<br>source-code · docs · ui-ux"]
    end

    subgraph P2["🔍 UNDERSTAND"]
        direction LR
        AN["analysis/<br>domain model · ui · personas<br>journeys · processes"]
        DI["discovery/<br>DISC-NNN"]
    end

    subgraph P3["📐 DEFINE"]
        direction LR
        FA["functional/<br>feature US + US-000"]
        TC["tests/test-cases/<br>TC-NNN"]
        AD["adrs/<br>ADR-NNN"]
    end

    subgraph P4["⚡ V-BOUNCE"]
        direction LR
        SP["spec/<br>one canonical SPEC"]
        CO["src/<br>code + tests"]
        MM["memory/<br>one MEM per V-Bounce"]
        MT["metrics/<br>manifest family v5"]
    end

    subgraph P5["🛡️ GOVERN"]
        direction LR
        RV["reviews/"]
        AV["adversarial-reviews/"]
        BG["bugs/"]
        IC["incidents/"]
        RK["risks/"]
        RT["retros/"]
        TT["tests/uat/"]
    end

    %% FORWARD FLOW (each stop is a AITL checkpoint)
    IN --> AN
    IN --> DI
    AN -->|"AITL-US-Approval"| FA
    AN --> AD
    DI --> FA
    DI --> AD
    FA -->|"AITL-BOLT-READY-Approval"| SP
    AD -->|"AITL-ADR-Approval"| SP
    TC -->|"AITL-TC-Approval"| SP
    SP -->|"AITL-SPEC-Approval"| CO
    CO --> MM
    CO --> MT
    MM --> MT

    %% GOVERNANCE
    MT -->|"audit"| RV
    RV -->|"finding → BUG/BOLT/DISC/ADR/RISK"| BG
    AV -->|"approved Verdict findings"| BG
    BG -->|"AITL-BUG-Approval → dedicated Bolt"| FA
    IC -->|"root cause"| BG
    IC -->|"hardening"| FA
    RT -.->|"reads"| MT
    RT -.->|"improves"| RV
    TT -->|"release feedback → new Bolts"| FA

    %% STYLES
    classDef c1 fill:#fef9e7,stroke:#b7950b,stroke-width:2px
    classDef c2 fill:#d5f5e3,stroke:#1e8449,stroke-width:2px
    classDef c3 fill:#d6eaf8,stroke:#2471a3,stroke-width:2px
    classDef c4 fill:#fadbd8,stroke:#c0392b,stroke-width:2px
    classDef c5 fill:#e8daef,stroke:#7d3c98,stroke-width:2px
    class P1 c1
    class P2 c2
    class P3 c3
    class P4 c4
    class P5 c5
```

### How to read it

Five phases, top to bottom. **Solid arrows** = forward flow. **Dashed arrows** = feedback loops.
Every forward stop is governed by a named AITL checkpoint (see [AITL Checkpoints](#checkpoints-actor-in-the-loop--aitl)).

| Phase | What happens |
|-------|-------------|
| **📥 Input** | Raw material lands here. Read-only. Everything starts from this. |
| **🔍 Understand** | `analysis/` extracts domain knowledge. `discovery/` researches material unknowns (DISC). |
| **📐 Define** | `functional/` defines WHAT (feature US / US-000 + Bolts). `adrs/` decides HOW. `tests/test-cases/` defines the independent verification contracts (TC). |
| **⚡ V-Bounce** | One canonical SPEC per Bolt → AI generates the intended-final change + tests → one MEM + manifest v5 entry → human review. |
| **🛡️ Govern** | Reviews, adversarial reviews, bugs, incidents, risks and retros feed the flow. |

### One path into V-Bounce

All work follows the **same single path** — regardless of whether it originates
from a feature, a defect, a review finding, or a QA Automation need:

```
Trigger (US | BUG | TC | DISC | REV | AREV | ADR)
  → origin approved (AITL-US | AITL-BUG | AITL-TC | AITL-DISC | AITL-REV |
                     AITL-AREV-VERDICT | AITL-ADR)
  → BOLT (AITL-BOLT-READY-Approval — includes DoR)
  → SPEC (AITL-SPEC-Approval)
  → V-Bounce → tests/gates
  → MEM + manifest v5 update
  → AITL-MEM-Approval
  → integration test cycle when test_integration is required (or none pending) → Ready for Acceptance
  → AITL-BOLT-DONE-Approval
```

| Step | What happens |
|------|-------------|
| **Trigger** | A feature US, a confirmed BUG, an approved TC, or approved DISC/REV/AREV conclusions or ADR evidence identify work to be done. |
| **Origin approval** | Feature US → `AITL-US-Approval`; BUG → `AITL-BUG-Approval` (gates its dedicated Bolt's readiness); TC → `AITL-TC-Approval` (before it governs); DISC/REV/AREV conclusions and ADRs carry their own approvals before governing a Bolt. |
| **Bolt** | One of three types: `functional` (feature US), `non-functional` (`US-000`), `test` (`TC-NNN.BOLT-NNN`). Approved individually at `AITL-BOLT-READY-Approval`. |
| **SPEC** | One canonical SPEC per Bolt (`bolt` field mandatory). The agent runs the pre-SPEC evidence gate; a human approves at `AITL-SPEC-Approval`. |
| **V-Bounce** | AI generates the intended-final change — its integration tests included — runs the hermetic tests until green (BUGs: strict red→green TDD inside the same V-Bounce; a defect that reproduces only in integration tests takes its red from a failed integration test run before it and its green from a passing covering run in the integration test cycle), then creates exactly one MEM and updates the manifest. |
| **Review** | The executing Dev-validator approves the MEM at `AITL-MEM-Approval` (one approver, any risk; QA/Sec/domain reviewers optional). |
| **Integration testing** | When the Bolt's `test_integration` is `required` and the SPEC's plan `applicable`, CI / QA run the integration tests against the approved change and record each run; a human classifies any failure, or waives pending integration testing with a written reason. Skipped only as `not-required` (plan `n/a`) or with a `waived` plan. |
| **Acceptance** | When the Bolt is `ready-for-acceptance`, `AITL-BOLT-DONE-Approval` makes it `done`; it requires the integration test evidence — a passing run, the `not-required` reason, the `waived` ADR or a late waiver's reason. With no run pending and one human holding both roles, it may be recorded jointly with `AITL-MEM-Approval` (§3.2.2). Release and promotion follow the adopting team's own process (§4.6). |

Every step that changes a Bolt's `status` appends a row to its Status
timeline (§3.2.3).

**No code change happens without an approved Bolt.** Every SPEC references a
Bolt; every V-Bounce produces exactly one MEM; previous MEMs are immutable
history.

---

## V-Bounce: The Execution Micro-Cycle

Every Bolt is executed through **V-Bounce** — the core execution pattern:

```mermaid
flowchart LR
    SPEC["📐 SPEC<br>AITL-SPEC-Approval"] --> AI["🤖 AI Agent<br>generates intended-final<br>code + tests<br>& runs hermetic tests"]
    AI --> GATES["🔒 CI Gates<br>pass | waived (ADR) | n/a"]
    GATES -->|"fail"| AI
    GATES -->|"pass"| ARTIFACTS["📝 AI creates exactly one MEM<br>+ appends v_bounces[] entry"]
    ARTIFACTS --> HUMAN["👤 AITL-MEM-Approval<br>executing Dev-validator reads<br>diff + test evidence + MEM + manifest"]
    HUMAN -->|"changes requested"| NB["New V-Bounce<br>new MEM"]
    NB --> SPEC
    HUMAN -->|"approved"| DONE["✅ V-Bounce approved<br>Development Completed →<br>integration test cycle or none pending<br>→ Ready for Acceptance"]
```

**Key principles:**
- The AI agent generates the **intended-final** artifacts by default (code, tests, config, docs) and **runs its own hermetic tests** until green or a stop condition; the integration tests it writes run in the Bolt's integration test cycle when its `test_integration` is `required`.
- After execution, the agent creates **exactly one MEM** and updates the Bolt manifest in `devflow/metrics/bolts/` — **before** human review.
- The human reviewer receives the **complete package**: code + tests/gates + MEM + manifest.
- `AITL-MEM-Approval` is recorded by the **Dev-validator who executed the Bolt** (one approver, any risk; QA/Sec/domain reviewers optional); the agent never self-approves.
- `changes_requested` preserves the MEM as immutable history; the next execution is a new V-Bounce with a new MEM.
- Every US, Bolt and TC produces a manifest in `devflow/metrics/` that validates against its `manifest-v5*.schema.json`.

---

## Checkpoints (Actor-in-the-Loop — AITL)

An approved checkpoint is **non-negotiable** — the approver is an **actor**, a
**human by default** and a virtual DevFlow Agent only by explicit, valid
configuration (with no or invalid config this is pure Human-in-the-Loop; **no
AI-signed approval is possible**). Checkpoints are named
`AITL-<CODE>-Approval`; the legacy pre-v5 `HITL-*` prefix is invalid.
Every approval requires a named reviewer (a human by default), review timestamps
and review-quality evidence (see [GUARDRAILS.md](GUARDRAILS.md) for the full map).

| Checkpoint | Owner | Validates |
|-----------|-------|-----------|
| `AITL-US-Approval` | Functional Analyst | Feature US + ACs approved; gates its Bolts' readiness |
| `AITL-BUG-Approval` | Functional Analyst (functional) / Architect-TL recommended if `severity: critical` else any team member (non-functional) — guidance, never a gate: any qualified member, the author included, may approve at any severity | BUG confirmed; gates its dedicated Bolt's readiness |
| `AITL-TC-Approval` | QA + domain owner | TC approved as independent verification contract |
| `AITL-BOLT-READY-Approval` | Functional Analyst (functional) / Architect-TL (non-functional; except a non-functional BUG's dedicated Bolt, which mirrors its parent BUG's severity routing) / QA Lead · QA Automation Lead · Architect · TL (test) | Bolt approved (includes DoR) |
| `AITL-ADR-Approval` | Architect / Tech Lead | ADR accepted and immutable |
| `AITL-SPEC-Approval` | Dev-validator + domain owners | One-Bolt implementation plan approved |
| `AITL-MEM-Approval` | Dev-validator who executed the Bolt (one approver, any risk; QA/Sec/domain optional) | MEM + V-Bounce approved |
| `AITL-BOLT-DONE-Approval` | PO/PM (functional) · technical owner (non-functional) · QA Lead / QA Automation Lead (test) | Bolt `done` — once `ready-for-acceptance`, with the integration test evidence |
| `AITL-DISC-Approval` · `AITL-REV-Approval` · `AITL-AREV-{CRITIQUE,DEFENSE,VERDICT}-Approval` | Qualified humans — **DISC/REV: the author included; AREV: the operator (the human orchestrating it, §3.13) included — G37 excludes a Judge sharing the implementor's or the Challenger's model, not an approver** | Conditional: mandatory once triggered |

No Bolt moves forward without its **required checkpoints signed with
review-quality evidence** (coverage by Bolt type in GUARDRAILS.md).

---

## Quick Start — Daily Workflow

Already have a configured project? Here's the day-to-day flow:

```mermaid
flowchart LR
    A["📋 Pick approved Bolt"] --> B["📐 Prepare SPEC<br>(evidence gate)"]
    B --> C["✍️ AITL-SPEC-Approval<br>SPEC-YYMMDD-HHmm"]
    C --> D["🤖 AI Agent<br>V-Bounce"]
    D --> E["📝 MEM + manifest<br>v_bounces[] entry"]
    E --> F["✅ AITL-MEM-Approval<br>diff + evidence"]
```

### Cheat Sheet

| You need to... | Do this |
|---------------|---------|
| Research something unknown | Create `DISC-NNN` in `discovery/` → `AITL-DISC-Approval` |
| Define business behavior | Create feature US in `functional/user-stories/` → `AITL-US-Approval` |
| Define a technical outcome | Create non-functional Bolt under `US-000` (no US approval needed) |
| Make a technical decision | Create `ADR-NNN` in `adrs/` → `AITL-ADR-Approval` |
| Implement a Bolt | `AITL-BOLT-READY-Approval` → `SPEC-YYMMDD-HHmm` → `AITL-SPEC-Approval` → V-Bounce → MEM + manifest |
| Verify a Bolt independently | Create `TC-NNN` in `tests/test-cases/` → `AITL-TC-Approval` |
| Automate QA | Create Test Bolt `TC-NNN.BOLT-NNN` from a TC |
| Document a defect | Create `BUG-NNN` in `bugs/` → `AITL-BUG-Approval` → dedicated Bolt → TDD in one V-Bounce (a defect that reproduces only in integration tests: red and green from integration test runs, §3.3.1) |
| Review quality | Create `REV-NNN` in `reviews/` → `AITL-REV-Approval` |
| Run an adversarial debate | Create `AREV-NNN/` in `adversarial-reviews/` (3 sequential phase approvals) |
| Log a risk | Create `RISK-NNN` in `risks/` |
| Store raw material | Drop in `input/` (business, databases, documentation, interviews, source-code, ui-ux) |

---

## Traceability Rules

Every document references the documents it depends on:

- A **DISC** references the `input/` material it analyzed and carries `AITL-DISC-Approval`.
- A **feature US** references the raw inputs / analysis evidence and carries `AITL-US-Approval`.
- A **BUG** carries `AITL-BUG-Approval` and references its exactly-one dedicated Bolt; the Bolt references the BUG.
- A **TC** references exactly one approved source Bolt (+ US/ACs or governing technical sources) and carries `AITL-TC-Approval`.
- A **Bolt** references its parent (a feature US, `US-000`, or one TC — the parent's approval is part of the Bolt's DoR) and carries `AITL-BOLT-READY-Approval`.
- A **SPEC** references exactly one approved Bolt; its `sources` lists every governed artifact actually used.
- An **ADR** references its motivating sources and carries `AITL-ADR-Approval`.
- A **REV** / **AREV** references the artifacts it audits; findings are governed only after their approvals.
- A **MEM** references its Bolt, canonical SPEC revision, V-Bounce number and manifest `v_bounces[]` entry.
- The **manifest family** validates against the `manifest-v5*.schema.json` schemas and records each artifact's lifecycle AITL decisions, its step timings and, for a Bolt, its integration test facts — the `test_integration` confirmed at `AITL-BOLT-READY-Approval`, the integration test runs, their human classifications and a human's late waiver of pending integration testing (minimal projection).

---

## Golden Rules

1. **Read the folder's README** before creating any document.
2. **Use the template** (`TEMPLATE-*.md`) as your starting point.
3. **The methodology governs** — `avenga-devflow/Avenga-DevFlow.md` is the single source of truth; §2/§3 own the rules, §4 is a walkthrough, §5 owns locations.
4. **No code without an approved Bolt** — no exceptions; urgency and size create none (the scope-out, G07: the **living data** of `devflow/agents/` + `devflow/actors/` (the agent lifecycle), `devflow/skills/` and `devflow/prompts/` is operational config — not a code change).
5. **Every approval is a named human checkpoint** — `AITL-<CODE>-Approval`, with timestamps and review-quality evidence. The agent never self-approves.
6. **One canonical SPEC per Bolt; exactly one MEM per V-Bounce** — previous MEMs are immutable history.
7. **An artifact without its manifest does not exist** — every US, Bolt and TC has a manifest in `devflow/metrics/` that validates against its `manifest-v5*.schema.json`.
8. **A failed gate cannot be overridden without an ADR** approved through `AITL-ADR-Approval` (owner + compensating control + expiry); the gate records `waived`, never `pass`.
9. **The reviewer reads the diff and the test evidence**, not the agent's self-summary.
10. **Mermaid for every diagram** (BPMN allowed for business processes); never ASCII art or embedded images as diagram substitutes.

---

## Starting a New Project

1. Install the distribution: the entire `devflow/` folder, `AGENTS.md` at the
   repository root, and the agent definition for your tool at the location it
   expects. **Install it at the project root — never nest the project inside
   another repository**: the platform may resolve the parent repository as
   the project and load its MainAgent instead of yours.
2. Drop all raw material into `input/` organized by subfolder.
3. Process `input/` into `analysis/` (AI-assisted; Functional Analyst governs).
4. Create feature USs and stop at `AITL-US-Approval`; technical work goes under `US-000`.
5. Define and approve Bolts (`AITL-BOLT-READY-Approval`) — functional, non-functional, and (from TCs) test.
6. Draft and approve Test Cases (`AITL-TC-Approval`) before the implementation SPEC.
7. Create ADRs (`adrs/`) as technical decisions are made (`AITL-ADR-Approval`).
8. For each Bolt: SPEC → `AITL-SPEC-Approval` → V-Bounce → MEM + manifest → `AITL-MEM-Approval` → its integration test cycle when the Bolt's `test_integration` is `required` (§3.2.2).
9. Accept Bolts once `ready-for-acceptance` (`AITL-BOLT-DONE-Approval` — jointly with the MEM approval when no run is pending and one human holds both roles); release/promotion follows the team's own process (§4.6).
10. Conduct reviews (`reviews/`, `adversarial-reviews/`) as stakeholders trigger them.

---

## Optional environment tools

DevFlow needs no external tools to run — the methodology is documentation, and
the optional first-party tooling ships as self-contained binaries in
`devflow/bin/`. For **ad-hoc queries** (inspecting JSON, searching text),
agents work better when two standard, open-source, cross-platform CLIs are
installed. They return only the data the agent asked for, so less text enters
the model's context: **lower token cost and a faster process**.

- **[`jq`](https://jqlang.org/)** — JSON query/transform.
  - Windows: `winget install jqlang.jq` (or `scoop install jq` · `choco install jq`)
  - macOS: `brew install jq`
  - Linux: `sudo apt-get install jq` · `sudo dnf install jq` · `sudo pacman -S jq`
- **[`rg` (ripgrep)](https://github.com/BurntSushi/ripgrep)** — fast recursive
  text/regex search.
  - Windows: `winget install BurntSushi.ripgrep.MSVC` (or `scoop install ripgrep` · `choco install ripgrep`)
  - macOS: `brew install ripgrep`
  - Linux: `sudo apt-get install ripgrep` · `sudo dnf install ripgrep` · `sudo pacman -S ripgrep`

**No admin rights?** Both ship as a single self-contained executable: download
it from the project's releases page
([jq](https://github.com/jqlang/jq/releases) ·
[ripgrep](https://github.com/BurntSushi/ripgrep/releases)) and put it on your
`PATH`.

Both are **optional**. Agents use them when present; when one is missing, the
agent mentions it **once per session** and carries on with native tooling.
They are never a dependency of the kit or of any governed operation.

---

## Known Limitations & Roadmap

What this version deliberately does **not** cover yet — read before adopting:

| Limitation | Status | Where it is governed |
|------------|--------|----------------------|
| **Release and promotion** | Out of scope — the governed flow ends at Bolt acceptance (`AITL-BOLT-DONE-Approval`); deployment, environment promotion and release approval follow the adopting team's own process. A model for this layer is planned for a future release. | §4.6 |
| **Adversarial reviews need three models** | Capability prerequisite — an AREV requires **at least three distinct models**, because the Judge must be a neutral third model differing from **both** the implementor's and the Challenger's. There is **no human-arbiter fallback**: a team without a third model does not run the AREV, and an initiated one that cannot reach a neutral Verdict is set `cancelled`. AREV is optional for every risk class, so this bounds the practice, never the core flow. | `GUARDRAILS.md` G37, §3.13, §3.15 |
| **Multi-repo / shared-monorepo `devflow/`** | Out of scope — SPEC, manifest and MEM resolve paths against a single repository baseline. To adapt: relocate `devflow/` and redefine the manifest's repository-relative `ref`/`sources` semantics. | §1 "Repository topology assumption" |
| **Monetary cost** | Deferred — no price catalog, no cost metric. Manifests keep recording provider/model/token usage in `runs[]`, so when pricing returns, costs are computable retroactively over all historical manifests. | §3.12 — `runs[]` keeps the token model; cost stays computable retroactively |
| **Validation tooling** | No validator ships with the methodology — G23/G33 schema and lifecycle validation remains procedural (agents and humans). Tooling arrives with the tools track, not with the methodology: optional by contract, with `devflow/bin/` reserved in the canonical tree for when it lands (§5.1). | `GUARDRAILS.md` G23/G33, §5.1 — tools track (arrives with `devflow/bin/`) |
| **Report generation** | Planned — `reports/TEMPLATE-REPORT.html` ships as a design reference with example data, not a generator. The manifest family already records everything a report needs (§3.12 timing contract), so reports stay computable retroactively once the tooling lands. | `reports/README.md`, §5.12 |

---

## Reporting a defect in DevFlow

DevFlow itself can be wrong — the methodology, a guardrail, a schema, a
template, a shipped agent definition, a shipped skill or, once one ships, a
tool.
When your agent meets such a defect it tells you in three beats — what the
tool or text said; why it is a DevFlow defect and not your project's; that
the DevFlow author should be told — works around it visibly or stops when a
rule would break, and never patches the kit text in your project
(methodology §3.16).

**Where to send it:** `eugenio.serrano@avenga.com` — the single channel.

**What to send:**

| Piece | Why the author needs it |
|-------|-------------------------|
| The agent's report | Self-contained: file and line, the exact quote, expected vs found, the rule it breaks, `devflow/VERSION`, the agent and model, the operating system |
| The file or files in question | The author reproduces against them; the agent names each one and says why |
| The tool call and its output | The exact command the agent ran and what it printed |

**The promise:** the agent never sends, and never tries to send, anything —
no mail, no upload, nothing automatic. Whether to send is your decision,
and you send it. Before you do, remove any secret or personal data: the
agent flags what it sees, but you are the one who checks. If you do not
send it, your work continues.

**How the loop closes:** the author triages the report; the fix ships in a
later version and reaches your project through the normal upgrade
(methodology §5.16) — never through
a local patch of the kit. Until then, a `RISK-NNN` or a retrospective note
in your project keeps it visible.

---

## Language Policy

The schema (YAML keys, enum values, IDs) is always in **English**; validators
and INDEX counters require it. The project's `content_language`, declared in
[`LANGUAGE`](LANGUAGE), governs the prose, the filename `<description>` slugs
(kebab-case ASCII, no accents), ADR titles, and the section headings of
`analysis/`, feature User Stories and Test Cases. Headings of every other
artifact family stay in English, and `AITL-*-Approval` codes are never
translated (§3.15).

---

## Further Reading

- **`avenga-devflow/Avenga-DevFlow.md`** — The full methodology (normative source): V-Bounce, three Bolt types, named AITL checkpoints, DORA Five metrics, gates, manifest family v5 and governance.
- **`GUARDRAILS.md`** — Agent-enforced rules: AITL stops, blocking/warning guardrails, naming, traceability.
- **`ONBOARDING.md`** — Recommended reading order and role-based map.
- **`metrics/README.md`** — Manifest family v5 schemas and lifecycle.
- Every subfolder has its own `README.md` — read it before creating documents there.
