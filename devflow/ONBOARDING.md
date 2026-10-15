# Onboarding — Avenga DevFlow

**Methodology version:** 5.2

> **Getting started.** Everything you need to know about how we work here and
> how to take your first Bolt. If it is not in this document, it is in the
> source of truth.

## 1. What this is

Avenga DevFlow is the project's AI-assisted development methodology: the **AI
generates**, the **human governs** through mandatory approval checkpoints
(AITL — an actor, human by default) at every step.

- **Source of truth:** [`avenga-devflow/Avenga-DevFlow.md`](avenga-devflow/Avenga-DevFlow.md) (v5.2)
- **Rules the agent enforces:** [`GUARDRAILS.md`](GUARDRAILS.md) (AITL stops, naming, traceability)
- **What is English, what is yours:** everything the kit ships — READMEs,
  templates, the example charters, skeleton headers — is **framework text
  in English**: copy it as a starting point; upgrades supersede it. Your
  project's **live text** — the charters under `agents/squad/<id>/` —
  follows your `content_language` (`devflow/LANGUAGE`). Structured fields,
  enums and ids stay English: the schema is never translated.

---

## 2. Reading order by role (5–10 min each path)

| Role | Read this | Why |
|------|-----------|-----|
| **Everyone** | `analysis/introduction/` (if present) → `README.md` → §0 of `Avenga-DevFlow.md` → `GUARDRAILS.md` (checkpoint map + blocking rules) | The plain-language story of each feature first (no jargon, not a source of truth), then the flow, the mandatory human stops, and what is forbidden |
| **Stakeholder / PO** | `analysis/vision/` + `functional/INDEX.md` | Review and contribute to USs and ACs; signs `AITL-BOLT-DONE-Approval` on every `feature` Bolt (without it the Bolt is not Done); does not write |
| **Functional Analyst** | `input/` + `analysis/README.md` + `functional/` | Create and approve USs, BUGs and functional Bolts |
| **Developer / Dev-validator** | `adrs/INDEX.md` + `spec/README.md` + `memory/README.md` + `metrics/README.md` + `tests/test-cases/` | Run V-Bounces, approve SPECs and MEMs, maintain manifests |
| **Architect / Tech Lead** | `adrs/` + `risks/` | Approve ADRs, non-functional Bolts, and `critical`-severity non-functional BUGs (lower severities may be approved by any team member, author included) and promotions |
| **QA / QA Automation** | `tests/` + `bugs/` + `reviews/` | Design TCs, report BUGs, create **and approve** Test Bolts (readiness and acceptance) |

---

## 3. The one Bolt path

All work follows the **same single path**. Every stop is a named human
approval (`AITL-<CODE>-Approval`) — none is skipped. Terms in **bold** are
defined in the glossary (§4).

```mermaid
flowchart LR
    ORIGEN["Trigger<br>US · BUG · TC · DISC · REV · AREV · ADR"] -->|"origin approved"| BOLT["BOLT<br>functional · non-functional · test"]
    BOLT -->|"AITL-BOLT-READY-Approval<br>(includes DoR)"| SPEC["SPEC<br>1 canonical per Bolt"]
    SPEC -->|"AITL-SPEC-Approval"| VB["V-Bounce<br>AI generates + runs hermetic tests"]
    VB --> MEM["MEM + manifest<br>v_bounces[] entry"]
    MEM -->|"AITL-MEM-Approval<br>the Dev who executed the Bolt"| INT["Integration test cycle<br>(or none pending)"]
    INT -->|"Ready for Acceptance"| ACC["AITL-BOLT-DONE-Approval<br>Bolt = Done"]
```

Golden rules of the path:

- **No code without an approved Bolt** — not a typo, not a config value (G07; its scope-out: the **living data** of `devflow/agents/` + `devflow/actors/` (the agent lifecycle), `devflow/skills/` and `devflow/prompts/` is operational config, not a code change).
- **One V-Bounce = one MEM** — previous MEMs are immutable history; if you request changes, the next attempt is a new V-Bounce with a new MEM.
- **The agent never self-approves** — `AITL-MEM-Approval` is signed by the Dev-validator who executed the Bolt (one approver, any risk; QA/Sec/domain reviewers optional).
- **The agent never extends its own mandate** — G40: a question, an analysis
  request or an opinion is never a work mandate. The deliverable is the
  answer; an artifact that would help is **proposed by name**, never created.
  This is the mirror of the rule above it: an agent never enables its own
  approval, and never widens its own instructions (§3.0.1).
- **An artifact without its manifest does not exist** — every US, Bolt and TC has a JSON manifest in `metrics/` that validates against its `manifest-v5*.schema.json`.

---

## 4. Minimal glossary (what you will see on day 1)

| Term | Definition |
|------|------------|
| **V-Bounce** | Execution micro-cycle: approved SPEC → the AI generates code and tests and runs the hermetic ones → creates MEM + updates manifest → a human approves (the **Development Completed** event). The Bolt then passes its **integration test cycle** — when its `test_integration` is `required` — or goes straight to Ready for Acceptance. |
| **Hermetic (V-Bounce level)** | Only what the agent can run on its own, with a result that depends only on the code — no database, queue, third-party API or shared environment is reached (§3.6). A test that replaces a dependency with a test double belongs here; tests needing real dependencies run in the integration test cycle. |
| **Integration test cycle** | Per-Bolt verification against real dependencies, after `AITL-MEM-Approval` and before acceptance, for a Bolt whose `test_integration` is `required`; each run recorded in the Bolt manifest, a failure classified by a human, pending integration testing waivable only by a human with a written reason; no checkpoint of its own (§3.2.2). |
| **Bolt** | Work unit (sizing: 1h–1d of active delivery, estimated per the AI-native rule §2.4 — review and rework dominate, never manual coding time). Three types: `functional` (under a feature US), `non-functional` (under `US-000`), `test` (under a TC, `TC-NNN.BOLT-NNN`). |
| **Bolt status** | The Bolt's one declared state, in its own document — Draft, Ready, Specified, In Development, Blocked, In Integration Testing, Integration Tests Failed, Integration Tests Blocked, Ready for Acceptance, Done or Deprecated — changed only by the transition table and recorded with a row of its Status timeline (§3.2.3). |
| **US-000** | Permanent container for technical work (infra, refactors, hardening, CI/CD). Has no approval of its own. |
| **Story points** | Optional Fibonacci value (1\|2\|3\|5\|8\|13) on a feature US: relative functional complexity, confirmed at `AITL-US-Approval`. Informational only — never time, never a gate, no velocity. US-000 has none. |
| **TC** | Test Case: independent verification contract, derived from approved intent — never from current code. |
| **SPEC** | Implementation plan for one Bolt (one canonical SPEC per Bolt; versioned revisions). |
| **MEM** | Narrative record of one V-Bounce: what was done, files with reasons, test evidence, decisions. |
| **Manifest family** | One mechanical JSON per governed artifact, in three levels: feature US (`metrics/user-stories/`), Bolt (`metrics/bolts/`) and TC (`metrics/test-cases/`), each validated by its `manifest-v5*.schema.json`. Created with the document and updated at every step, recording origin, SPEC revisions, V-Bounces, the Bolt's integration test facts (its `test_integration` as confirmed at `AITL-BOLT-READY-Approval`, the integration test runs, their human classifications and a human's late waiver of pending integration testing), AITL decisions and step timings (`created_at`, `review_ready_at`, `review_started_at`, `decided_at`). An artifact without its manifest does not exist (G23, G33). US-000 carries none. |
| **Actor** | A member of the team who **produces** the governed artifacts its role owns (FA → US, architect → ADR, developer → SPEC + code, QA → TC/tests) as executor and **participates** in AITL approvals as approver when configured, under the independence floor — a **human by default**, a virtual **DevFlow Agent** only by explicit, valid configuration (§3.0.1). Recorded `human:<user>` / `agent:<id>`; the model is an attribute of the agent actor, never the identity. HITL is the default case inside AITL (actor = human); with no agents configured every checkpoint is a human approval and **no AI-signed approval is possible** (the safe-default invariant). |
| **Attribution (`author` / `agent` / `llm`)** | Every AI-generated document records three things: `author` — the accountable human (always `human:<user>`); `agent` — the agent that actually produced it (`agent:<id>`; the Coordinator is `agent:avenga-devflow`; never the agent that only delegated); `llm` — the exact model. Manifests record the same in `created_by`, `runs[].agent` and `runs[].model` (§3.0, §3.12). Who **wrote** the record of an AITL decision is a separate field, the optional `written_by` — the reviewer, or the agent that transcribed the decision, such as the Coordinator; it sits beside the signers (`reviewers`, `decided_by[]`), never among them: writing is not signing (§3.0, §3.12). |
| **Agent run** | A session launched by a human in which the Coordinator delegates the work of that human's mandate to the project's DevFlow Agents (§2.18). It authorizes nothing beyond that mandate (G40). The Coordinator keeps its record in `agent-runs/`, opened only when a human launches the run. Not an integration test run: the two share only the word. |
| **AITL checkpoint** | Mandatory human approval with name, timestamps and evidence: `AITL-US-Approval`, `AITL-BOLT-READY-Approval`, `AITL-BOLT-DONE-Approval`, `AITL-SPEC-Approval`, `AITL-MEM-Approval`, … (there is no `AITL-BOLT-Approval`: the Bolt has two distinct checkpoints, G05). The pre-v5 `HITL-*` prefix is invalid (G05). |
| **DoR / DoD** | Definition of Ready (validated inside `AITL-BOLT-READY-Approval`) / Definition of Done (completion evidence, validated at acceptance). |
| **ADR** | Immutable architecture decision (once approved, it is never edited). |
| **BUG** | Confirmed defect. Requires `AITL-BUG-Approval`, which gates its dedicated Bolt's readiness (the Bolt may be drafted first). |
| **DISC / REV / AREV** | Investigation / review / adversarial debate — optional (need-driven), but with mandatory approvals once initiated. |
| **UAT** | User Acceptance Testing. The UAT approval checkpoint was removed in v4.2; release/acceptance follows the team's own process until a redesigned model ships in a future version. |
| **`_archive/`** | Per-folder subfolder holding lifecycle-closed documents. Excluded from agent scans for token economy — the agent reads it only if you explicitly ask, or if an active document references an archived artifact (§5.4, W20). |
| **`agents-data/`** | Per-agent shared knowledge area, versioned and visible to the whole team. No pre-created subfolders: each agent creates its own `agents-data/<agent>/` on first use and is responsible for it — durable knowledge only, never temporary data (W21), and never a replacement for `memory/` MEMs (§2.12). Not evidence: never citable, no AITL. The `devflow/` folder structure is canonical — agents may not create new folders (G30) and never write into `input/`, which is human-deposited raw evidence (G31, §5.12). |
| **`prompts/`** | Project prompts (`PROMPT-NNN-<description>.md`) — versioned, team-shared, copy-paste ready. Living data: created, modified and improved in this folder, no approval and no manifest; never scattered into `agents-data/` (§5.12). |
| **`sprints/`** | Sprint records (`SPRINT-NNN-<description>.md`), one per sprint: the sprint's planning record — dates, goal and the User Stories and Bolts in its scope, by reference — with a declared `status`: `planned`, `active`, `closed` or `cancelled` (§3.15). Numbered from the folder's INDEX, never reused; no approval, no manifest. A planning record, not a report: its number is not the key of a `REPORT-YYYY-Www.html` (§5.12). |
| **`reports/`** | Sprint progress reports for project management (`REPORT-YYYY-Www.html`) built from the manifest family — delivery, quality, review latency, AITL coverage and AI usage. **Generation is planned** (tooling track): only a design reference with example data ships today. Derived, never governed: not citable as the source of any artifact (G28, §5.12). |
| **`agent-runs/`** | Agent-run records (`ARUN-YYMMDD-HHmm-<description>.md`), one per agent run, kept by the Coordinator under the folder's README: who launched the run, its timeline, links to the OQs it opened, its outcome. An operational narrative — not a derivative document, and still never governed: not citable as the source or justification of any artifact or AITL checkpoint (G28, §5.12); no approval, no manifest, no `status`. |

---

## 5. FAQ

**I found a bug, what do I do?**
Document it in `bugs/BUG-NNN-<description>.md` → its dedicated Bolt may be drafted
alongside → `AITL-BUG-Approval` → strict TDD **inside the same V-Bounce** (red test →
fix → green). A defect that reproduces only in integration tests takes its
red from a failed integration test run recorded first and its green from a
passing covering run in the integration test cycle (§3.3.1). It is never
fixed under another Bolt or directly from a ticket.

**A delivered story has to change — do I reopen it?**
No. `done` is terminal for the Bolts that delivered it (§3.15). You write a **new** User Story that evolves it,
with `functional/user-stories/TEMPLATE-US-EVOLUTION.md`. That story states the
**complete** target behaviour — what already exists and what changes — with
every criterion marked `existing` or `delta`, and only the `delta` ones produce
Bolts and story points (§2.6). The delivered story stays untouched as the record
of what was approved then; you mark it `deprecated` only when its behaviour is
being withdrawn rather than extended. If the behaviour is simply **wrong**, that
is a `BUG`; if nothing about the behaviour changes, it is a non-functional Bolt
under `US-000`.

**Should I create a DISC or go straight to a US?**
If there is a material unknown to investigate (external API, library, legacy
behavior) → `DISC-NNN`. If you already know what to build → US directly
(business behavior) or a non-functional Bolt under `US-000` with its approved
technical source (ADR/DISC/REV).

**Can I edit an approved ADR?**
No. Decisions in an approved ADR are immutable; if the decision changes, you
create a new ADR that supersedes it.

**Who approves my MEM?**
The Dev-validator who executed the Bolt — the same developer who took it.
One approver at any risk; QA or Security may be added as optional reviewers, never required.

**Can I touch code without a Bolt?**
No. It is a blocker (G07): code, tests, config, IaC, schemas and migrations
require an approved Bolt. Urgency and size create no exception. The one
scope-out is **living data**: the agent lifecycle (installing/creating/
deleting DevFlow Agents within `devflow/agents/` + `devflow/actors/`), the
project skills in `devflow/skills/<skill-name>/` (§5.2.1) and the project
prompts in `devflow/prompts/` (§5.12) are operational config, not a code
change. Everything else G07 covers stays blocked.

**Can I work on several Bolts at once?**
Ideal WIP: **1 active Bolt per person/agent** (§3.2). No multitasking — if you
get blocked, it gets resolved or returned; Bolts are not left half-done.

**Where is my Bolt's manifest?**
In `metrics/bolts/`, named after the Bolt (`US-NNN.BOLT-NNN-<desc>.json` or
`TC-NNN.BOLT-NNN-<desc>.json`). If it does not exist or does not validate
against `manifest-v5-bolt.schema.json`, the Bolt does not exist. Every US and TC
has its own manifest too (`metrics/user-stories/`, `metrics/test-cases/`).

**Why does the agent not see my archived documents?**
By design (§5.4, W20). Each folder may keep an `_archive/` subfolder for
lifecycle-closed documents, and agents do **not** search, list or read it
proactively — that is what keeps their token cost down as the project grows.
Treat archived files as generally invisible to the agent: it reads them only
when you explicitly ask, or when an active document explicitly references an
archived artifact. If a task needs archived content, the agent will say so
and ask you first.

**Where do agents store their shared knowledge?**
In `devflow/agents-data/` — a versioned area visible to the whole team
(§5.12). There are no pre-created subfolders: each agent creates its own
`agents-data/<agent>/` folder on first use and is responsible for its
content. Everything there is **committed to the repository and shared with
everyone**; it holds durable, team-useful knowledge only — **never temporary
data** (that goes to the OS temp directory, W21) and **never a replacement
for `memory/`** (one MEM per V-Bounce stays mandatory, §2.12). It is never
evidence, carries no approvals, and agents do not scan other agents' folders
unless you ask. Agents may **not** create new folders inside `devflow/`
outside the canonical structure (G30), and they never write into `input/`:
that folder is deposited by humans only and is read-only for agents (G31).

**Can agents use `jq`, `rg` or other CLI tools?**
For **ad-hoc, throwaway queries** — inspecting JSON, searching text — prefer
standard CLI utilities when they are available in the environment: `jq` for
JSON, `rg` (ripgrep) for text/regex. They are a **convenience, never a
dependency**: if a tool is absent, fall back to a native/built-in path and do
not fail. **Never** use them for **governed operations** — validating or
editing the manifest family, rebuilding an `INDEX.md`, computing the next
sequential ID and the like run through the versioned tools in `devflow/bin/`
(when present) or the documented procedure, never an ad-hoc `jq` pipeline that
would silently break append-only, monotonic-timestamp or schema rules.

**When you reach for one of them and find it missing**, fall back and tell
the adopter — **once per session** — that installing it is recommended: these
tools return only the data you asked for, so less text enters your context,
which **lowers token cost** and **speeds up the work**. Point to
`devflow/README.md`'s *Optional environment tools* for the install commands.
Do not check for the tools up front, do not repeat the suggestion in the same
session, and never block or wait on it.

---

## 6. Language policy

The **schema** (YAML keys, enums, IDs) is always in
**English** — validators and dashboards require it. The **prose**
(descriptions, decisions, narrative) follows the project's `content_language`,
declared in `devflow/LANGUAGE` (the language of the raw inputs; see §3.15 of
the methodology). Filename `<description>` slugs follow `content_language`
(kebab-case ASCII, no accents); section headings follow `content_language`
in `analysis/`, feature User Stories and Test Cases and stay English
elsewhere; ADR titles and bodies follow `content_language`. `AITL-*-Approval`
codes are never translated. Never mix languages within one field.

---

## 7. Developer development plan (§3.14 — skill atrophy mitigation)

If the agent writes everything, juniors never build the skills to review the
agent. This is managed explicitly. **The plan is descriptive only** — it
creates no artifacts and no evidence of these practices is recorded in
`devflow/` documents (§3.14):

| Practice | Cadence |
|----------|---------|
| Role rotation (Spec-author ↔ Dev-validator ↔ AI-Orchestrator) | ≥1 per quarter |
| AI-review training (common failure modes) | 1-day onboarding module |
| Quarterly skill review | Quarterly |

---

## Deeper reading

- **`avenga-devflow/Avenga-DevFlow.md`** — The complete methodology (normative; §2/§3 govern).
- **`GUARDRAILS.md`** — The 40 blockers, 23 warnings, naming and traceability.
- **`README.md`** — Framework map, full flow and cheat sheet.
- Every subfolder has its own `README.md` — read it before creating documents there.
