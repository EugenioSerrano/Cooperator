# GUARDRAILS — Agent Enforcement Rules

**Enforcing:** Avenga DevFlow v5.2

> **Read this first.** These are the non-negotiable rules that the AI agent
> MUST enforce before any code change, document creation, or workflow action.
> If a developer attempts something that violates a guardrail, the agent MUST
> block the action (🔴) or warn (🟡) and explain why.
>
> **Normative source:** [`avenga-devflow/Avenga-DevFlow.md`](avenga-devflow/Avenga-DevFlow.md)
> is the single source of truth. Section references (§) point to it. If this
> file ever diverges from the methodology, the methodology governs.

---

## 🧭 AITL checkpoint map (the actor stops the agent MUST respect)

Every checkpoint is named `AITL-<CODE>-Approval`, is occupied by an actor — a human by default, a virtual DevFlow Agent only by explicit, valid configuration (§3.0); absent/invalid config → human-only, no AI-signed approval possible — and requires a named reviewer, review timestamps and
review-quality evidence (§3.0).

| Checkpoint | Owner | Gates what |
|------------|-------|------------|
| `AITL-US-Approval` | Functional Analyst (or, if the named role has no holder, the available qualified human records it, noting the self-assigned role) | Feature US + ACs approved; required for its Bolts' `AITL-BOLT-READY-Approval`. Does not apply to US-000. |
| `AITL-BUG-Approval` | Functional Analyst (functional) / Architect or Tech Lead recommended when `severity: critical`, otherwise any team member (non-functional) — recommended only; guidance, never a gate: any qualified team member, the BUG's own author included, may record it at any severity | BUG confirmed, evidenced, classified; required for its one dedicated Bolt's `AITL-BOLT-READY-Approval`. |
| `AITL-TC-Approval` | QA + Functional Analyst/domain owner (functional) / QA + technical owner (non-functional) (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | TC approved as independent verification contract; required for it to govern a SPEC and for its Test Bolts' `AITL-BOLT-READY-Approval`. |
| `AITL-BOLT-READY-Approval` | Functional Analyst (functional) / Architect or Tech Lead (non-functional; except: a non-functional BUG's dedicated Bolt mirrors the parent BUG's severity routing, §2.16) / QA Lead, QA Automation Lead, Architect or Tech Lead (test) (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Bolt approved (includes DoR); authorizes SPEC preparation, not execution. |
| `AITL-ADR-Approval` | Architect / Tech Lead (or, if the named role has no holder, the available qualified human records it, noting the self-assigned role) | ADR accepted and immutable; governs SPECs, waivers and constraints. |
| `AITL-SPEC-Approval` | Dev-validator + applicable domain owner(s) (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Canonical SPEC (and every material revision) approved; authorizes the code-run / V-Bounce. |
| `AITL-MEM-Approval` | Dev-validator who executed the Bolt (one approver, any risk; QA/Sec/domain reviewers optional) | MEM + V-Bounce approved; approves and completes the V-Bounce. |
| `AITL-BOLT-DONE-Approval` | PO/PM (functional) / routed technical owner (non-functional) / QA Lead or QA Automation Lead (test) (or, if a named role has no holder, the available qualified human records it, noting the self-assigned role) | Bolt `done` — requested only from `ready-for-acceptance`: after a passing covering integration test run, or with no run pending (`test_integration: not-required`, or a `waived` plan), or once a human has waived the pending integration testing (a late waiver is the waiving human's decision: an agent transcribes it only from that human's explicit decision naming the Bolt and the reason, never infers it from a standing or blanket instruction, and never signs it). |
| `AITL-DISC-Approval` | Qualified human (research domain) — **the author included** | Discovery conclusions become governed input. |
| `AITL-REV-Approval` | Qualified human (Review) — **the author included** | Review findings become governed input. |
| `AITL-AREV-CRITIQUE-Approval` | Qualified human (AREV) | Critique approved; only then Defense begins. |
| `AITL-AREV-DEFENSE-Approval` | Qualified human (AREV) | Defense approved; only then Verdict begins. |
| `AITL-AREV-VERDICT-Approval` | Qualified human (AREV) | Verdict approved; only then findings are usable. |

**Role routing is guidance, never a gate (§3.0):** the owner named above is the
recommended approver; when the role has no holder, the available qualified
human records the approval, noting the self-assigned role; one person may hold
several roles. The identity-separation rules stay hard — the handoff
incoming-executor rule, Judge-model neutrality (G37), and no AI self-approval
(G18/G24).

**Approval independence:** no approval is inherited from a related artifact.
US/BUG/TC/BOLT/ADR/SPEC/MEM each keep their own checkpoint. A material change
to an approved artifact invalidates its approval and pauses dependent work.

---

## 🔴 BLOCKING (the agent MUST reject)

### ORIGIN phase (US / BUG / TC)

| # | If the developer attempts... | Agent response |
|---|------------------------------|----------------|
| G01 | Approve a functional Bolt without `AITL-US-Approval` on its parent feature US | ❌ *"The parent US is draft. Its Bolts may be drafted and refined alongside it, but `AITL-BOLT-READY-Approval` is refused until `AITL-US-Approval` is recorded (§2.6, §2.9, §3.0). US-000 has no approval lifecycle."* |
| G02 | Approve a BUG's dedicated Bolt without `AITL-BUG-Approval` | ❌ *"BUG is draft. Its one dedicated Bolt may be drafted and refined alongside it, but `AITL-BOLT-READY-Approval` is refused until the BUG's approval is recorded (§2.16, §2.9, §3.0)."* |
| G03 | Approve a Test Bolt without `AITL-TC-Approval` on its exact parent TC | ❌ *"Parent TC is draft. Its Test Bolts may be drafted and refined alongside it, but `AITL-BOLT-READY-Approval` is refused until the TC is approved (§2.6.1, §2.9, §3.0)."* |
| G04 | Fix a BUG under an unrelated Bolt, directly from a ticket, or untracked in another V-Bounce | ❌ *"Every BUG has exactly one dedicated Bolt; the BUG and Bolt reference each other. The BUG alone never authorizes code (§2.16)."* |
| G05 | Use legacy checkpoint names (the pre-v5 `HITL-*` prefix) or non-canonical `AITL-*` identifiers | ❌ *"Canonical checkpoints are `AITL-<CODE>-Approval` with uppercase codes only. Legacy prefixes — the pre-v5 `HITL-*` names, preserved only in migrated history (G36) — are invalid for new approvals (§3.0)."* |
| G06 | Derive a TC's expected results from current code behavior | ❌ *"Test-basis rule: TCs derive from approved intent (US/AC + approved Bolt, or non-functional Bolt + ADRs), never from the implementation as oracle (§2.6.1)."* |
| G29 | Block a BUG's `AITL-BUG-Approval` (or its dedicated Bolt's `AITL-BOLT-READY-Approval`) for lack of the recommended-role approver, on account of severity, or by excluding the BUG's own author | ❌ *"Approval routing is guidance, never a gate (§2.16, §3.0). The recommended approver — Functional Analyst (functional); Architect or Tech Lead when `severity: critical`, otherwise any team member (non-functional) — is advice, not a precondition: any qualified team member, the BUG's own author included, may record `AITL-BUG-Approval` at any severity, and the dedicated Bolt's `AITL-BOLT-READY-Approval` follows the same rule. The AI self-approval prohibition (G18/G24) is a different axis and still holds."* |
| G30 | Create a new folder inside `devflow/` (or any of its subfolders) outside the canonical folder structure | ❌ *"The `devflow/` folder structure is canonical (§5.12, `devflow/README.md`). The only sanctioned agent-created areas are the per-agent folders under `agents-data/` — each agent creates its own on first use, is responsible for it, and may freely organize files and subfolders **within** it (§5.12) — the per-AREV folders `adversarial-reviews/AREV-NNN-<description>/` (§2.15), and the `_archive/` subfolders the agent creates when archiving closed documents (§5.4). Store non-governed agent knowledge in your `agents-data/<agent>/` folder instead of inventing a folder."* |
| G31 | Write, save, or move files into `devflow/input/` or any of its subfolders | ❌ *"`input/` is human-deposited raw evidence, read-only for agents (§5.6). Agents may read it as evidence but never create, modify, or move files into it — adding an input triggers an impact assessment."* |
| G32 | Cite `agents-data/` content as the source or justification of a SPEC, Bolt, ADR, US, TC, BUG, MEM, or any AITL checkpoint | ❌ *"`agents-data/` is agent working data, never governed input (§5.12). It has no approval checkpoint and is excluded from evidence scans; move anything that must be cited into the proper governed artifact first."* |
| G33 | Create, approve, or advance a feature User Story or Test Case without its manifest, or with a manifest that does not validate against its schema | ❌ *"Every feature US and every TC has exactly one manifest in `metrics/user-stories/` or `metrics/test-cases/`, created with the document and updated at every lifecycle step — an artifact without its manifest does not exist (§3.12). US-000 is the exception: it is a permanent container with no approval lifecycle and carries no manifest."* |

### BOLT phase

| # | If the developer attempts... | Agent response |
|---|------------------------------|----------------|
| G07 | Make any code-related change (code, tests, config, IaC, schemas, migrations, build scripts) without an approved Bolt | ❌ *"No code without an approved Bolt — urgency and size create no exception (§3.2). SCOPE: the agent lifecycle is operational configuration, not a code change — installing, creating or deleting DevFlow Agents within the agent system (`devflow/agents/` squad definitions and their platform wrappers; `devflow/actors/` actor files and roster listings), adding, editing or removing a project skill (`devflow/skills/<skill-name>/`, §5.2.1) and writing or improving a project prompt (`devflow/prompts/`, §5.12) are living data (§5.12 and the roster's living-data rule), bounded by the lifecycle consistency contract: never the shipped examples/templates edited in place, never outside the agent system, and approver authority always the human's roster act. Everything else this rule covers stays absolutely blocked."* |
| G08 | Create a Bolt with the wrong parent type | ❌ *"functional → feature US; non-functional → `US-000-non-functional.md`; test → exactly one TC. BUG and hotfix are conditions, not Bolt types (§2.4, §3.8)."* |
| G09 | Put implementation instructions, architecture decisions, technologies, endpoints, schemas or algorithms inside a Bolt | ❌ *"Bolt = what must be delivered and expected evidence, never how. Implementation detail belongs in the SPEC; durable decisions belong in an approved ADR (§2.4)."* |
| G10 | Prepare a SPEC or execute a Bolt without `AITL-BOLT-READY-Approval` | ❌ *"Draft Bolts cannot enter SPEC preparation or execution. Record `AITL-BOLT-READY-Approval` first (§2.4, §3.0)."* |
| G11 | Two developers/agents executing the same Bolt simultaneously | ❌ *"Single active executor per Bolt. Handoff is allowed only after the current V-Bounce produced its MEM and manifest entry and paused at `AITL-MEM-Approval` (§3.3)."* |
| G35 | Record `AITL-BOLT-READY-Approval` while an `open` or `in-validation` `OQ-NNN` targets this Bolt's parent US or one of its governing artifacts | ❌ *"Unresolved analysis questions block Bolt readiness — this is part of the DoR (§2.9, §3.2), not a separate checkpoint. Every OQ in `analysis/open-questions/` whose `targets` include this Bolt's parent or governing artifacts must first be `answered` and propagated to its target, `deferred` with a revisit trigger, or `dropped` with a reason (§5.7)."* |

### SPEC phase

| # | If the developer attempts... | Agent response |
|---|------------------------------|----------------|
| G12 | Create a SPEC without a `bolt` field, a second concurrent SPEC for the same Bolt, or a SPEC spanning multiple Bolts | ❌ *"One Bolt has exactly one current canonical SPEC; one SPEC references exactly one Bolt (§2.4.1, §3.2.1)."* |
| G13 | Generate a SPEC while any governed source it needs is draft, rejected, stale or missing its AITL approval | ❌ *"Pre-SPEC evidence gate failed. Emit a blocking report naming the artifact and required checkpoint — no partial or draft SPEC (§2.4.1, §3.2.1)."* |
| G14 | Start a code-run / V-Bounce before `AITL-SPEC-Approval` | ❌ *"SPEC is draft. `AITL-SPEC-Approval` (Dev-validator + applicable domain owners) authorizes execution; Bolt approval alone is not enough (§3.2.1, §3.3)."* |
| G15 | Continue executing after a material change to a governed source (BUG, TC, Bolt, feature US/ACs, ADR, DISC/REV/AREV finding, code baseline) | ❌ *"Material source change invalidates the current SPEC approval. Stop, revise the canonical SPEC, append `spec_revisions[]`, and re-approve through `AITL-SPEC-Approval` (§2.4.1, §3.3). Silent mid-run edits are forbidden."* |
| G16 | Span one V-Bounce across two SPEC revisions | ❌ *"One V-Bounce never spans two SPEC revisions. Close the current V-Bounce with its MEM (normally `execution_outcome: blocked\|cancelled`), revise, re-approve, then start a new V-Bounce (§3.3)."* |

### V-BOUNCE / MEM phase

| # | If the developer attempts... | Agent response |
|---|------------------------------|----------------|
| G17 | Complete a V-Bounce without exactly one MEM + manifest `v_bounces[]` entry + PAUSE at `AITL-MEM-Approval` | ❌ *"Mandatory sequence: record outcome → create exactly one MEM → update manifest → PAUSE. The MEM is mandatory even for failed, blocked or turn-budget-exhausted V-Bounces (§2.12, §3.0)."* |
| G18 | Self-approve the MEM (approver actor = executor actor), skip the review, or treat "AI says it's fine" as approval | ❌ *"The agent creates the MEM and never approves its own work. An AI actor may approve only under an explicit, valid virtual-approver configuration with independence (approver actor ≠ executor actor); absent or invalid config → human-only. The reviewing actor reads the actual diff + test/gate evidence + MEM + manifest; the record never fabricates a human — a virtual approval is `agent:<id>`/`model:<id>` (§3.0, §2.12)."* |
| G19 | BUG V-Bounce: modify production code before objective red evidence | ❌ *"Strict TDD in the same V-Bounce: create/run the reproduction test, record red for the expected reason, only then change production code, then green. No red evidence → stop, MEM with blocker, no fix. A defect that reproduces only in integration tests takes as its red a failed integration test run recorded before the V-Bounce; the V-Bounce adds the best hermetic reproduction it can, and the green is a passing covering run in the Bolt's integration test cycle. The red still precedes every production change. Such a Bolt is `test_integration: required` with an `applicable` plan, its reason beginning `integration-test-only BUG —`, and is never waived — neither by a `waived` plan nor by a late waiver (§2.16, §3.2.2, §3.3.1)."* |
| G20 | Merge, promote, or accept a Bolt without the applicable approvals | ❌ *"Merge requires an approved `AITL-MEM-Approval` — a merge is never the integration test cycle; acceptance requires `AITL-BOLT-DONE-Approval`, requested only from `ready-for-acceptance`; production promotion requires the Bolt `done`. Release and promotion follow the adopting team's own process — DevFlow does not prescribe Unit/UAT approval checkpoints in this release (§4.6; a redesigned model is planned for a future version)."* |
| G21 | Override a failed quality gate without an ADR | ❌ *"Gate override requires an ADR approved through `AITL-ADR-Approval` with reason, owner, compensating control and expiry date; the gate records `waived`, never `pass`. `n/a` requires a reason in the approved SPEC (§3.6)."* |
| G22 | Mark a Bolt `done` without `AITL-BOLT-DONE-Approval` | ❌ *"**Development Completed** (the latest MEM approved) is an event, never a status, and is not `done`. A Bolt becomes `done` only through `AITL-BOLT-DONE-Approval` `approved` from `ready-for-acceptance` (§2.9, §3.0, §3.2.3)."* |

### GOVERN phase

| # | If the developer attempts... | Agent response |
|---|------------------------------|----------------|
| G23 | Create a manifest that does not validate against its `manifest-v5*.schema.json` (`metrics/manifest-v5-bolt.schema.json`, `metrics/manifest-v5-us.schema.json`, `metrics/manifest-v5-tc.schema.json`), or with fields outside it (gates, DORA, cost, AREV, a `status`, `manual_intervention`, `iterations`, `traceability.prs`…) | ❌ *"Manifest v5 is the minimal traceability and timing contract (schema_version, artifact, spec_revisions, v_bounces, checkpoint_approvals, review timestamps) and, for a Bolt, its integration test facts — `bolt.test_integration` (the value confirmed at `AITL-BOLT-READY-Approval`) and `integration_test_runs[]` (runs, human classifications, human late waivers). It records facts, never a status: no manifest carries one, and the Bolt document governs its own (§3.2.3). Unknown fields fail validation (§3.12)."* |
| G24 | Delegate a checkpoint to an AI approver without explicit valid configuration (or without independence), or fabricate a reviewer decision | ❌ *"A checkpoint may be occupied by an AI actor only under an explicit, valid virtual-approver configuration with independence (§3.0); otherwise it is human-only. Approvals require a named actor (human by default), timestamps and evidence; a decision is never fabricated, and a virtual approval is never recorded under a human name (§3.0)."* |
| G25 | Skip, reorder, or auto-switch an AREV phase | ❌ *"AREV phases are sequential and each requires its approval: Critique → `AITL-AREV-CRITIQUE-Approval` → Defense → `AITL-AREV-DEFENSE-Approval` → Verdict → `AITL-AREV-VERDICT-Approval`. Agent/model selection between phases is a manual human action (§2.15, §3.13)."* |
| G26 | Use a draft ADR as governing, or edit an approved ADR | ❌ *"ADRs are governing only after `AITL-ADR-Approval` and are immutable afterwards; a reversed decision is a new ADR that supersedes the old one (§2.8, §3.5)."* |
| G27 | Use DISC conclusions, REV findings or AREV findings as governed input without their approvals | ❌ *"DISC requires `AITL-DISC-Approval`, REV requires `AITL-REV-Approval`, AREV requires all three sequential phase approvals. Only an approved Verdict produces usable findings (§2.13–§2.15)."* |
| G28 | Cite a derivative document (`derivative: true`, e.g. anything in `analysis/introduction/`) or a generated sprint report (`reports/`) as the source or justification of a SPEC, Bolt, ADR, User Story or Test Case — or an agent-run record (`agent-runs/`) as the source or justification of a SPEC, Bolt, ADR, US, TC, BUG, MEM, or any AITL checkpoint | ❌ *"Derivative documents and generated reports are summaries of governed artifacts, never governed input, and they have no approval checkpoint that could make them one. Reports belong to this class by location, since rendered HTML cannot carry a `derivative: true` marker. Cite the artifact the summary was derived from (§5.5, §5.12). An agent-run record is not a summary but the primary chronology of an agent run, so it is not in that class — it is non-citable by its location all the same: an operational narrative with no AITL checkpoint and no manifest (§2.18, §5.12). Where it and a governed artifact disagree, the artifact wins; cite the governed artifact the run produced or changed."* |
| G34 | Stage, commit, push, or open a pull request without an explicit user request | ❌ *"The agent never writes repository history on its own. Version-control actions happen only when the human explicitly asks (§3.3); artifacts are written to the working tree and the human owns the commit."* |
| G36 | While migrating a project to a newer methodology version, rewrite an approved MEM, an approved ADR, a recorded AITL decision or `review:` contract, or `CHANGELOG.md` history; **overwrite the project section of the root `AGENTS.md` instead of merging at its marker**; or, while converting a manifest forward, overwrite a recorded value, drop a recorded field, or invent one the repository does not record | ❌ *"A version migration moves **documentation and manifests** forward, never **history** (§5.16). Manifests are re-routed and converted to the current `schema_version` — a repository holds exactly one family — but only by adding the new schema's fields as `null` where the schema admits it and applying its renames; every recorded value crosses untouched and a value nobody observed is never inferred (§3.12). Approved MEMs and ADRs are immutable (§2.12, §3.5), approval evidence records what happened rather than what the current version prescribes (§3.0), and a changelog is a record. **The root `AGENTS.md` is merged, never replaced:** take the new version's text up to its `AVENGA-DEVFLOW:PROJECT-SECTION` marker and keep the existing file's text from its own marker onward, byte for byte. **Exclude it from the install copy** so it is never destroyed and the merge happens in place; if a blunt copy already overwrote it, read the previous content from the **last commit** — the fallback, which is why the tree must be committed before a migration runs. If the existing file has no marker, or more than one, stop and let a human place the boundary (§5.2, §5.16). Migrate READMEs, INDEXes, templates, document structure and manifests; update `devflow/VERSION` last."* |
| G37 | Run the Verdict phase with a Judge sharing the implementor's or the Challenger's model, or run an AREV with fewer than three models | ❌ *"Judge neutrality (§3.13): the Verdict's model must differ from **both** the implementor's and the Challenger's — a Judge that shares either one is not arbitrating, it is repeating. Running an AREV requires **at least three models** so the Judge is always a neutral third model; a single operator running three models is valid and approves the three AREV documents but does not arbitrate. There is **no human-arbiter fallback**: a team without a third model does not run the AREV, and an AREV already open that cannot reach a neutral Verdict is set `cancelled` (§3.15)."* |
| G38 | Move a document into an `_archive/` folder before its lifecycle is closed | ❌ *"Only lifecycle-closed documents are archived (§5.4): `done` Bolts with their complete package (Bolt, SPEC, MEMs), `superseded`/`deprecated` ADRs, closed DISC/REV/AREV records with every finding routed, closed BUGs, retired RISKs and completed UAT minutes. **Archiving never causes closure, it presupposes it** — it is a housekeeping move, not a lifecycle step, and it grants no approval the document does not already hold. Because `_archive/` is excluded from agent scans (W20) and its contents are treated as generally invisible, archiving an active, draft or in-review document is the one move that removes open work from governance without ever closing it. If closure cannot be established from the document itself, do not archive it and ask the human (§5.4, §3.0)."* |
| G39 | Use a `status` value that is not in its artifact family's row of the §3.15 vocabulary table; store as a `status` a state the methodology derives (the MEM's review state, the US/TC progress states, the Bolt's two events *Development Completed* and *Integration Tests Passed*, and an agent run's closure, §2.18); or change a Bolt's `status` other than through a transition of its transition table — a correction row or a migration seed row (§5.16) excepted — in the same act that appends its Status timeline row | ❌ *"The §3.15 status vocabulary is the normative and complete set: a family never uses a value outside its row, and a new value is added to the table **before** it appears in a template, a folder README or an INDEX. A Bolt's `status` is declared in its document and changes only through the transition table, recorded in the same act with a row of its Status timeline; a correction row and a migration seed row (`—` → the migrated value, *Act / evidence* beginning `Migration —`, §5.16) are the only exceptions to the table, never to continuity (§3.2.3). An archived Bolt keeps the vocabulary of the version that closed it (§5.16). `status` is a document state and never substitutes for a AITL decision; derived states are never stored as a `status` value (§3.12, §3.15)."* |
| G40 | Create a governed artifact — US, Bolt, TC, BUG, ADR, DISC, REV, AREV, RISK, INC, RETRO, UAT, OQ, SPEC, or an `analysis/` document — without an explicit human **work mandate**: a direct request for that artifact, or a **delivery mandate** on an *approved* artifact that covers exactly the chain the methodology derives from it | ❌ *"The agent produces only what a human mandated (§3.3, §3.0.1). A question, an analysis request, an opinion or a described problem is never a mandate — the deliverable is the answer, and an artifact that would help is **proposed by name**, never created. A delivery mandate ("implement US-022") authorizes exactly the **derived chain** — that artifact's Bolts, each Bolt's SPEC, each V-Bounce's MEM and manifest entries, and the OQs raised inside them — and nothing outside it. A mandate is never an approval: every `AITL-*-Approval` in the chain still stops the agent, even when the mandate says 'coordinate everything'. A mandate on a draft artifact authorizes nothing executable — it reaches no further than the drafting loop (G01). No agent ever widens its own mandate — a Coordinator propagates a bounded copy to the executors it spawns, and an executor inherits it, never extends it — the mirror of 'an agent never enables its own approval'. An unrequested draft is not free: it consumes an ID that is never reused (§2.4) and rows in an INDEX and its manifests (G33)."* |

---

## 🟡 WARNING (the agent MUST alert, but not block)

| # | If the developer attempts... | Agent warns... |
|---|------------------------------|----------------|
| W01 | Size a Bolt beyond 1 working day of active delivery, or split a Bolt just because a V-Bounce crossed a day boundary | ⚠️ *"1h–1 working day of active delivery time is the target, not a destructive boundary. Split only for independently deliverable outcomes (§2.4, §3.2)."* |
| W02 | Generate a SPEC without the required contents | ⚠️ *"SPEC must record: source inventory and approval references, repository baseline, scope/exclusions, impacted files, implementation plan, AC/test mappings, gates, security/data/observability, migration/rollback, risks and stop conditions (§2.4.1, §3.2.1)."* |
| W03 | Create a MEM without the minimum content | ⚠️ *"MEM must include: V-Bounce/Bolt/SPEC revision/baseline, summary, every file with reason, tests/gates evidence, decisions, deviations, risks, manual interventions, evidence links, and the review record (§2.12)."* |
| W04 | Invent/fabricate a timestamp instead of using the system clock | ⚠️ *"Use `Get-Date -Format "yyMMdd-HHmm"` (PowerShell) or `date +"%y%m%d-%H%M"` (Bash). Never invent timestamps."* |
| W05 | Write YAML frontmatter enum values in a language other than English, or mix languages inside a single prose field | ⚠️ *"Schema stays English: `status: open`, not `abierta`. One language per prose field, never interleaved. Localized enums break validators and INDEX counters (§3.15)."* |
| W06 | Name a MEM with `v2`, `retry`, `fix`, `bounce-2` suffixes or change the `<description>` slug across V-Bounces | ⚠️ *"MEMs for the same Bolt/SPEC reuse the identical slug (in the project's `content_language`, §3.15); only the `YYMMDD-HHmm` timestamp changes. Reserve filenames atomically (§2.12)."* |
| W07 | Create a document without using the corresponding TEMPLATE | ⚠️ *"Use `TEMPLATE-*.md` as the starting point. Consistent structure matters for tooling and agent consumption."* |
| W08 | Use ASCII art, or embedded images as a substitute for a required diagram | ⚠️ *"Use the notation the artifact's own convention allows: Mermaid by default; BPMN for business processes in `analysis/process/`. Embedded images are never a substitute for a required diagram; raw evidence images in `input/` are raw material, not diagrams."* |
| W09 | Leave `llm` or `agent` empty in an AI-generated Markdown artifact, put an agent, model or tool name in `author`, or add YAML frontmatter to code/JSON | ⚠️ *"Every AI-generated Markdown artifact carries `author` (the accountable human), `agent` (`agent:<id>` of the producing agent, never the delegating one) and `llm` (the exact model id) (§3.0). Code and JSON do not use YAML frontmatter; their attribution is recorded in the manifest (`created_by` human, `runs[].agent`, `runs[].model`, §3.12). AREV phase templates carry `author` and `agent`; their model is `challenger_model` / `defender_model` / `judge_model` (§2.15, §3.13), with no separate `llm:` field."* |
| W10 | Write an ADR title or body (prose) in English when the project's `content_language` is different | ⚠️ *"ADR titles and bodies follow the project's `content_language` like any other artifact; the `ADR-NNN` ID stays English (§3.15)."* |
| W11 | Record a review without the complete `review:` contract | ⚠️ *"Every approvable artifact carries `review_ready_at` and `review:` (decision, reviewers as `{actor, role, model}`, started_at, decided_at, findings). Empty findings require `acknowledged_without_comment: true` + `acknowledgment_reason` (§3.0)."* |
| W12 | Write QA Automation code without a Test Bolt, or let one Test Bolt span several TCs | ⚠️ *"QA Automation requires dedicated Test Bolts parented by exactly one TC (`TC-NNN.BOLT-NNN`) with the full Bolt lifecycle (§2.6.1)."* |
| W13 | Close an incident without linking it to its originating Bolt/deployment evidence | ⚠️ *"Incidents keep their own artifacts (`INC-NNN`); deployment-caused incidents feed D3/D4 from there. Manifest v5 carries no DORA/incident data (§3.12)."* |
| W14 | Downgrade `risk_class` after the first MEM approval without formal re-review | ⚠️ *"Risk is assigned at `AITL-BOLT-READY-Approval`, may be escalated at any review, and cannot be reduced after the first MEM approval unless the Bolt is re-reviewed and re-approved; append the change to `risk_history` (§3.3)."* |
| W15 | Use `L4` autonomy without an ADR approved through `AITL-ADR-Approval` | ⚠️ *"L4 (Orchestrated) is reserved and never allowed without an explicit approved ADR (§3.3)."* |
| W16 | Report a Bolt lead time as DORA Change Lead Time | ⚠️ *"Bolt Lead Time (from `AITL-BOLT-READY-Approval` to `AITL-BOLT-DONE-Approval`) is a separate flow metric, never DORA D2 (§3.7.1–§3.7.2)."* |
| W17 | Work more than one active Bolt per person/agent, or treat AREV findings as a substitute for human review | ⚠️ *"WIP target: 1 active Bolt per person/agent (§3.2); a Bolt in its integration test cycle does not occupy its executor (§3.2.2). AREV is a pre-filter for later human decisions, never a replacement (§3.0, §2.15)."* |
| W18 | Convert story points into hours, gate any checkpoint on them, or derive a velocity/performance target from them | ⚠️ *"`story_points` on a feature US is a relative functional-complexity signal confirmed at `AITL-US-Approval`. Informational only: planning stays on throughput + Bolt Lead Time (§2.6, §4.3). US-000 carries none."* |
| W19 | Estimate a Bolt's active delivery time as manual coding effort | ⚠️ *"Code is agent-generated in minutes; the dominant cost is human review and rework. Compose estimates per the AI-native estimation rule: expected V-Bounces × (generation + review budget) + overhead (§2.4, §3.0). Over one day → suspect anchoring before splitting."* |
| W20 | Search or read `_archive/` proactively without an explicit user request or an explicit reference to an archived artifact | ⚠️ *"`_archive/` is excluded from agent scans for token economy; access it only when the user explicitly asks or an active document explicitly references an archived artifact. If a task needs archived content, state the exclusion and ask the user (§5.4)."* |
| W21 | Use `agents-data/` for temporary or disposable data | ⚠️ *"`agents-data/` is versioned shared knowledge for the whole team, never a scratch area. Temporary data (drafts, tool outputs, large intermediates) goes to the OS temp directory and is never committed; only durable, team-useful information belongs under `agents-data/<agent>/` (§5.12)."* |
| W22 | A Bolt or SPEC under an evolution US claims a criterion marked `existing` as covered | ⚠️ *"`existing` criteria were delivered by the US named beside them; only `delta` criteria produce Bolts and story points (§2.6). Compare the Bolt's `Covered acceptance criteria` with the parent story's markings: if the criterion really must change it is `delta` — mark it in the US and re-approve; if it does not, remove it from the Bolt. Regression coverage of an `existing` criterion belongs to a Test Case (§2.6.1), never to a Bolt."* |
| W23 | Write a text file under `devflow/` that is not UTF-8, carries a byte order mark, breaks a line with a lone `CR`, or holds mojibake or a U+FFFD replacement character | ⚠️ *"Text files under `devflow/` are UTF-8 without BOM, with `LF` or `CRLF` line breaks and never a lone `CR`, and correctly decoded — no double-encoded text (mojibake), no U+FFFD (§3.15). Raw evidence a human deposited in `input/` may arrive in any encoding: read it as it is, never re-encode it. A BOM or a lone `CR` can be removed byte-exactly, only within the work mandated (G40) — never a license to clean other files (approved ADRs and MEMs, `_archive/`, `input/`); mojibake, an invalid byte or a U+FFFD has lost the original text — report it for a human who has the source, never guess a repair."* |

---

## ✅ NAMING CONVENTIONS (the agent MUST validate)

| # | Artifact | Pattern | Example |
|---|----------|---------|---------|
| N01 | User Story | `US-NNN-<description>.md` | `US-001-payment-processing.md` |
| N02 | Functional Bolt | `US-NNN.BOLT-NNN-<description>.md` | `US-001.BOLT-003-auth-endpoint.md` |
| N03 | Non-functional Bolt | `US-000.BOLT-NNN-<description>.md` | `US-000.BOLT-007-infra-ci.md` |
| N04 | Test Bolt | `TC-NNN.BOLT-NNN-<description>.md` | `TC-027.BOLT-001-invoice-download-e2e.md` |
| N05 | SPEC | `SPEC-YYMMDD-HHmm-<description>.md` | `SPEC-260607-1430-auth-module.md` |
| N06 | MEM | `MEM-YYMMDD-HHmm-<description>.md` (stable slug per Bolt) | `MEM-260802-1138-invoice-download.md` |
| N07 | Manifest (functional / non-functional) | `US-NNN.BOLT-NNN-<description>.json` | `US-001.BOLT-003-auth-endpoint.json` |
| N08 | Manifest (test) | `TC-NNN.BOLT-NNN-<description>.json` | `TC-027.BOLT-001-invoice-download-e2e.json` |
| N09 | Manifest JSON Schemas | `manifest-v5*.schema.json` (normative, in `metrics/`) | — |
| N10 | Review | `REV-NNN-<description>.md` | `REV-001-code-review.md` |
| N11 | Bug | `BUG-NNN-<description>.md` | `BUG-001-race-condition.md` |
| N12 | Discovery | `DISC-NNN-<description>.md` | `DISC-001-legacy-analysis.md` |
| N13 | ADR | `ADR-NNN-<description>.md` | `ADR-006-logging-strategy.md` |
| N14 | Risk | `RISK-NNN-<description>.md` | `RISK-001-api-dependency.md` |
| N15 | Incident | `INC-NNN-<description>.md` | `INC-001-payment-timeout.md` |
| N16 | Retro | `RETRO-NNN-YYYY-Www.md` | `RETRO-001-2026-W23.md` |
| N17 | Adversarial Review | `AREV-NNN-<description>/` (folder) | `AREV-001-security-owasp/` |
| N18 | Open Question | `OQ-NNN-<description>.md` | `OQ-003-multi-tenant-scope.md` |
| N19 | Process | `PROC-NNN-<description>.md` | `PROC-001-order-fulfillment.md` |
| N20 | Test Case | `TC-NNN-<description>.md` | `TC-027-invoice-download.md` |
| N21 | UAT | `UAT-NNN-<description>.md` | `UAT-001-milestone-payments.md` |
| N22 | Interview / Business Risk | `INT-NNN-<description>.md` / `BR-NNN-<description>.md` | `INT-001-stakeholder-cto.md` / `BR-001-market-entry.md` |
| N23 | Introduction narrative (derivative, §5.5) | `<feature-description>.md` — descriptive, **no ID** | `mass-payment-cancellation.md` |
| N24 | Manifest (US) | `US-NNN-<description>.json` | `US-012-invoice-download.json` |
| N25 | Manifest (TC) | `TC-NNN-<description>.json` | `TC-027-invoice-download.json` |
| N26 | Sprint report (derivative, §5.12) | `REPORT-YYYY-Www.html` | `REPORT-2026-W23.html` |
| N27 | Project prompt | `PROMPT-NNN-<description>.md` | `PROMPT-003-weekly-triage.md` |
| N28 | Agent-run record (§2.18) | `ARUN-YYMMDD-HHmm-<description>.md` | `ARUN-260802-0915-invoice-download.md` |
| N29 | Sprint record (planning, §5.12) | `SPRINT-NNN-<description>.md` | `SPRINT-003-invoice-download.md` |

**Rules:** sequential numbers (`NNN`) come from each folder's `INDEX.md`.
`BOLT-NNN` is three digits, widening to four past 999 (§2.4); every other
`NNN` stays at three. The `HHmm` of N05/N06 is a **local wall-clock time with
no offset**, and must be read in the same UTC offset as the artifact's own
`generation.created_at` — otherwise the
alphabetical order of SPEC/MEM filenames stops matching their chronological
order across time zones. The precise, offset-bearing instant always lives in
the manifest field, never in the filename. N28's `YYMMDD-HHmm` is the same
local wall-clock stamp, read from the system clock when the agent run opens;
a run record has no manifest, and its sequence number (`run: N`) is not in
its filename — it is claimed in `agent-runs/INDEX.md` (§5.15). N29's `NNN`
numbers a sprint's planning record and, like every other `NNN`, is claimed in
`sprints/INDEX.md`; it is not the key of an N26 report, which keeps its
ISO-week name. Two rows of this table carry no sequential
ID: **N23**, because derivative documents carry no identifier at all —
nothing may reference them as evidence (G28) — and **N26**, whose sprint
reports are keyed by ISO year and week rather than by an allocator, being
generated rather than authored (§5.12). It is not the only
ID-less artifact in the repository — the `analysis/` families that keep a
curated inventory instead of an allocator (business-context, domain-model,
glossary, introduction, personas, scope, ui, user-journeys, vision) are named
descriptively and claim no `NNN` either (§5.15); they have no row here
because there is no pattern to validate.
`<description>` slugs follow the project's `content_language` (kebab-case
ASCII, no accents/ñ — §3.15); the IDs and prefixes above are never
translated or renamed. The examples shown are in English because they
document the framework; instantiated filenames use the project's language.

---

## 🗂️ INDEX CONVENTION (the agent MUST follow when writing an `INDEX.md`)

Columns are **free per cluster** — a Test Case index and a Bolt index do not
need the same ones. What is fixed is the status vocabulary and the footer:

| Emoji | Means | Never use it for |
|-------|-------|------------------|
| 🟡 | Draft, pending its AITL checkpoint, or partially resolved (mitigated, materialized) — not final | a terminal state |
| 🔴 | Open, unresolved, needs action now — e.g. open incidents, unanswered open questions | a terminal state |
| 🔄 | Work in motion or paused pending a trigger: in progress, in review, in-fix, deferred | a terminal state |
| ✅ | **Live and healthy**: active, approved, current truth | anything closed |
| 🏁 | **Terminal and successful**: closed, fixed, resolved, all findings routed | anything obsolete |
| ⛔ | **Terminal and obsolete**: deprecated, superseded, archived, no longer valid | anything resolved successfully |
| ❌ | Rejected, or changes requested and never re-approved | — |

`superseded` and `deprecated` are both ⛔ — the document no longer governs
either way; which of the two it is belongs in the section title, not the
emoji. A **terminal-but-successful** outcome is never ⛔.

The distinction that matters: **🏁 and ⛔ are both terminal, but they mean
opposite things.** A fixed BUG, a closed Review and a mitigated Risk are
successes (🏁); a deprecated ADR or an archived process is obsolete (⛔).
Marking a resolved artifact ⛔ tells a reader — and an agent scanning the
index — that it was abandoned.

- Section order follows the artifact's own lifecycle, earliest state first.
- An artifact with no lifecycle (glossary, personas, journeys, business-context,
  introduction, domain-model, ui) needs no status sections — a single listing is
  correct.
- `**Last updated:** <Month YYYY>` goes at the **bottom** of the file, always.

---

## 🔗 TRACEABILITY RULES (the agent MUST verify cross-references)

| # | Rule |
|---|------|
| T01 | Every feature US traces to raw inputs / analysis evidence and carries `AITL-US-Approval`; its Bolts cannot be approved before it |
| T02 | Every BUG carries `AITL-BUG-Approval` and references its exactly-one dedicated Bolt; the Bolt references the BUG. For a non-functional BUG, the recorded reviewer may be any qualified team member, the BUG's own author included; the severity-based approver (Architect/Tech Lead when `severity: critical`) is a recommendation, not a gate |
| T03 | Every TC references exactly one approved source Bolt (+ `source_us`/`covered_acs` or `US-000` + governing sources) and carries `AITL-TC-Approval` |
| T04 | Every Bolt references its parent (a feature US, US-000, or one TC — the parent's approval is part of the Bolt's DoR) and carries `AITL-BOLT-READY-Approval` |
| T05 | Every SPEC references exactly one approved Bolt; its `sources` lists every governed artifact actually used |
| T06 | Every MEM references its Bolt, canonical SPEC revision, V-Bounce number and manifest `v_bounces[]` entry |
| T07 | Every manifest validates against its `manifest-v5*.schema.json`; `v_bounces[].spec_revision` points to an existing approved revision; `mem.ref` points to an existing MEM |
| T08 | Every ADR references its motivating sources and carries `AITL-ADR-Approval` |
| T09 | Every DISC/REV carries its approval; AREV phases are sequential and each approved before the next |
| T10 | A review finding that requires code creates a Bolt before a SPEC is written (never REV → SPEC directly) |
| T11 | Every gate result is `pass`, `waived` (with approved waiver ADR) or `n/a` (with reason in the approved SPEC) |
| T12 | Approvals are never inherited: each artifact's `AITL-*` decision is recorded on that artifact (and minimally projected in the manifest where applicable) |

---

## 📋 Review contract (machine-readable evidence, §3.0)

Every approvable artifact carries this minimum contract in its own metadata;
`written_by` in it is optional — who wrote the record, beside the signers (§3.0):

```yaml
review_ready_at: 2026-08-02T11:45:00-03:00   # submitted, available for review
review:
  decision: approved                          # approved | changes_requested | rejected
  reviewers:
    - actor: human:eugenio.serrano            # human:<user> | agent:<id> (§3.0)
      role: dev_validator
      model: null                             # null for a human; the model id for an agent
  written_by:                                 # optional — who wrote this record: the reviewer,
    - actor: human:eugenio.serrano            # or the agent that transcribed the decision;
      role: dev_validator                     # beside reviewers, never inside them —
      model: null                             # writing is not signing
  started_at: 2026-08-02T11:55:00-03:00       # direct human inspection begins
  decided_at: 2026-08-02T12:10:00-03:00       # decision recorded
  findings: []                                # findings/comments; if empty:
  acknowledged_without_comment: true          # must be true and...
  acknowledgment_reason: "Evidence inspected; no findings identified."
```

**Manifest projection** (lifecycle decisions of the manifest family) — a
field-for-field **copy** now that both sides use the actor grammar:
`review.reviewers[]` (`{actor, role, model}`) → `checkpoint_approvals[].decided_by[]`,
the optional `review.written_by[]` → `checkpoint_approvals[].written_by[]`
beside it (absent stays absent; writing is not signing — a writer signs only
by also being a reviewer, §3.0, §3.12),
`review.decision` → `.decision`, `review.decided_at` → `.decided_at`,
`review.findings` may be summarized in the optional `comment`. (A decision is
**virtual** when a `decided_by[].actor` carries the `agent:` prefix — derived,
not stored; there is no `mode` field, §3.12/G39.) `review_ready_at` and `started_at` are
**copied** to the manifest as `review_ready_at` / `review_started_at`
(§3.12 timing contract); the full findings and acknowledgment fields are
not. A mismatch between artifact evidence and its manifest projection is a
validation error. The Bolt's **acceptance review** (`AITL-BOLT-DONE-Approval`)
is a second review of the same artifact: it carries `acceptance_review_ready_at`
and `acceptance_review:` (same shape as above) and projects to
`bolt.acceptance.review_ready_at` / `bolt.acceptance.review_started_at`
(§3.0, §3.12).

---

## ⚡ V-BOUNCE MANDATORY SEQUENCE (§3.3)

The agent MUST execute this exact sequence. No step is skippable:

```
1. Execute against the approved SPEC revision (AITL-SPEC-Approval recorded);
   the executor taking the Bolt sets its status in-development and appends
   its Status timeline row — unless it is already in-development (then no
   row: a V-Bounce N+1 started while in-development changes nothing)
2. Run the hermetic suite inside the V-Bounce's own loop until green or a
   stop condition (integration tests are built and statically checked here,
   executed in the Bolt's integration test cycle when it has one)
3. Record implementation + verification outcome (including blockers)
4. Create exactly one MEM (mandatory even on failure/blocker/turn-budget);
   once it and its manifest entry exist, a blocked V-Bounce sets the Bolt
   blocked, with its Status timeline row
5. Update Bolt manifest: append v_bounces[] entry (number, spec_revision,
   git_commit, execution_outcome, code_generation, mem, review_ready_at,
   review_started_at — all eight required; review_started_at is null until
   the human begins)
6. PAUSE at AITL-MEM-Approval — human reviews diff + evidence + MEM + manifest
```

- **AREV is NOT a step of this sequence** (§2.15): it is a standalone,
  stakeholder-triggered mechanism that needs no Bolt or SPEC to exist. A
  Bolt-bound AREV examines the **closed package** (step 5) and its approved
  Verdict is a pre-filter for the step-6 decision. If that decision is
  `changes_requested`, the normal rule below applies.

- `execution_outcome`: `ready_for_review | failed | blocked | cancelled`.
- Internal autonomous retries stay inside the V-Bounce; they never add entries.
- `changes_requested` keeps the MEM as immutable history; the next agent
  execution is a NEW V-Bounce with a NEW MEM.

---

## 🏗️ US-000 — Non-functional container (§3.2)

`US-000-non-functional.md` is the permanent, always-active traceability
container for **every Bolt whose primary outcome is non-functional**:
infrastructure, refactoring, technical debt, hardening, security,
performance, availability, observability, CI/CD, dependency upgrades,
database maintenance, developer tooling.

| If the work is... | Assign Bolt to... |
|-------------------|-------------------|
| A new business feature | Its natural feature US |
| A defect in feature behavior (functional BUG) | Dedicated functional Bolt under the affected feature US (the BUG's approval gates its readiness) |
| A defect in a technical constraint (non-functional BUG) | Dedicated non-functional Bolt under `US-000` (the BUG's approval gates its readiness) |
| Infra, CI/CD, monitoring, pipelines | `US-000` |
| Framework/library upgrades | `US-000` |
| Cross-cutting refactors | `US-000` |
| Security hardening across the codebase | `US-000` |
| Developer tooling / DX improvements | `US-000` |
| QA Automation | Test Bolt under one TC (never US-000) |

**Rules:**
- US-000 has **no approval lifecycle** — never approved, rejected or re-approved.
- US-000 is not a substitute for approved ADRs or quality gates.
- Every US-000 Bolt requires its own technical `AITL-BOLT-READY-Approval` and follows
  the full SPEC → V-Bounce → MEM → manifest lifecycle.
- If in doubt, classify by **primary outcome** (§2.4): user- or business-visible
  behavior → feature US; technical outcome → `US-000`. No "quick fix", "chore",
  refactor, hardening or infrastructure exception exists.

---

## ⏱️ AITL REVIEW BUDGETS (§3.0 — recommended, per risk class)

Recommended review times for the technical/delivery checkpoints. US, BUG, TC,
ADR, DISC, REV and AREV budgets are **project-defined**. Review duration is
derived from the manifest timing contract (`decided_at` − `review_started_at`,
§3.12) or, where a step timestamp is missing, from workflow telemetry.

| Risk class | SPEC | MEM / V-Bounce | Bolt acceptance |
|-----------|------|----------------|-----------------|
| `low`      | ~5   | ~15            | ~5              |
| `medium`   | ~10  | ~30            | ~10             |
| `high`     | ~15  | ~60            | ~15             |
| `critical` | ~30  | ~90            | ~30             |

---

## 🎯 AITL COVERAGE TARGETS (§3.0 — by Bolt type)

Missing any required checkpoint = process defect logged in the next retro.

| Bolt type | Required checkpoints | Conditional additions | Target |
|-----------|----------------------|-----------------------|--------|
| `functional` | AITL-US-Approval + AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | AITL-BUG-Approval when BUG-driven | **100%** |
| `non-functional` | AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | AITL-BUG-Approval when BUG-driven | **100%** |
| `test` | AITL-TC-Approval + AITL-BOLT-READY-Approval + AITL-SPEC-Approval + AITL-MEM-Approval + AITL-BOLT-DONE-Approval | — | **100%** |

Plus: `AITL-ADR-Approval` for every applicable ADR, and all conditional
approvals for any DISC/REV/AREV used by the Bolt or SPEC.

**Per-Bolt coverage is the whole coverage story in this release:** release-level
grouping, promotion and customer acceptance are the adopting team's own process
(§4.6); DevFlow does not prescribe Unit/UAT approval checkpoints in this release
(a redesigned model is planned for a future version). A `done` Bolt already reports 100% on
its own checkpoints.

**Approver at `AITL-MEM-Approval` (§3.3):**
The MEM is approved by the **Dev-validator who executed the Bolt** (never the
AI agent); after a recorded handoff, the incoming executor. **One approver, at
any risk** — there is no risk-based approver count; QA/Sec/domain reviewers are
optional:

| Risk class | Approver at AITL-MEM-Approval |
|-----------|-------------------------------|
| `low`      | 1 (the executing Dev-validator) |
| `medium`   | 1 (the executing Dev-validator) |
| `high`     | 1 (the executing Dev-validator) |
| `critical` | 1 (the executing Dev-validator) |

**Autonomy defaults by risk (§3.3):** `low → L3`, `medium → L3`, `high → L2`,
`critical → L1`. `autonomy_level` is declared in the frontmatter of every SPEC
revision. L4 (Orchestrated) requires an ADR approved through
`AITL-ADR-Approval`.

---

## 🛣️ AITL-BOLT-DONE ROUTING BY WORK CATEGORY (§3.11)

`work_category` routes the acceptance approver. `bolt_type` (3 canonical
types) ≠ `work_category` (reporting/routing) ≠ `service_class` (priority).

| Work category | Acceptance approver | Demo form |
|---------------|---------------------|-----------|
| `feature` | PO / PM | Business demo |
| `refactor` | Tech Lead | Before/after diff + test parity |
| `infra` | Tech Lead + SRE | Deployment evidence + perf-smoke |
| `hardening` | Tech Lead + Sec | Fixed control + regression test |
| `debt` | Tech Lead | Metric/maintainability improvement |
| `qa_automation` | QA Lead / QA Automation Lead | TC automated with execution evidence |

> **Availability (operability principle, §3.0):** these approvers are the
> recommended defaults; where a paired or named role has no holder (e.g. no SRE
> or Security member), the available qualified human records the acceptance,
> noting the self-assigned role — the routing never blocks.

A `feature` Bolt without a PO sign-off is **not Done**, regardless of gates.

---

## 🚦 PRIORITIZATION SERVICE CLASSES (§3.8)

| Service class | Priority | Rules |
|---------------|----------|-------|
| `regulatory` | Immediate | Non-negotiable deadline; takes precedence |
| `incident_hotfix` | Immediate | Small, bounded Bolt ≤ 4 active delivery hours when scope permits; full approval lifecycle, never skipped |
| `feature_value` | Normal | Standard V-Bounce; scheduled by PO |
| `debt_hardening` | Reserved | Reserve 10–20% capacity per week; assigned to `US-000` |

- BUG and hotfix are **conditions**, not Bolt types — the Bolt remains
  functional or non-functional (or test).
- Split only for independently deliverable outcomes or approvals; elapsed time
  alone never splits a Bolt.

---

## 🛑 STOP-AND-ASK RULE (§3.0, §2.12)

If the agent loops beyond its configured turn budget without a green hermetic
suite (the budget counts the V-Bounce's own loop only: waiting for an
environment, and the Bolt's integration test cycle, consume no turns), it MUST
**stop and ask a human** — but only **after** creating the
mandatory MEM and manifest `v_bounces[]` entry recording the blocker and
current evidence. The human may patch the code manually; this is recorded in
the MEM (not hidden, not punished — measured). Manual intervention is never
an excuse to skip gates or AITL review.

---

## 🛡️ AI-NATIVE GATES (§3.6 — quick reference)

A `fail` at the V-Bounce level blocks `AITL-MEM-Approval` and merge; a `fail`
at the integration test level blocks acceptance (`AITL-BOLT-DONE-Approval`),
never the MEM approval that precedes it; a release-level `fail` blocks the
release; any `fail` blocks promotion. G21 governs override and `n/a`.
Prompt-injection, secret-leak, hallucination lint, IP/license provenance,
PII/DLP, dependency-confusion, test-first evidence, behavioral
reproducibility, and Bolt-manifest validation — all at the V-Bounce level.

**Conditional classic gates (per Bolt, when applicable to the change), by
level (§3.6):** in the **V-Bounce** (hermetic) — build, lint and type checks;
unit and component tests green; consumer-side contract tests against test
doubles when the change crosses component boundaries within the Bolt's
scope; SAST; dependency scanning and licenses when the Bolt adds or changes
dependencies; the static part of the OWASP Top 10 review when the change
touches an externally reachable surface (public endpoints, web UIs, auth
boundaries) — otherwise `n/a` with a reason in the SPEC. In the Bolt's
**integration test cycle** — green integration tests against the real
dependencies; contract or E2E verification against the real providers when
the change crosses component boundaries within the Bolt's scope; DAST and the
dynamic part of the OWASP review when an executable attack surface exists;
perf-smoke with SPEC-defined p95/p99 thresholds; the required logs, metrics
and traces verified as emitted by the running backend service.

**Release level (aggregated above the per-Bolt loop, NOT per Bolt):** mutation
testing (where the language allows), the SBOM of the built distributable, and
end-to-end / contract tests for **cross-Bolt** regressions at release /
milestone level. This suite never substitutes the per-Bolt
conditional gate above — a boundary-crossing Bolt cannot record its own
contract/E2E gate as `n/a` because a later release suite will cover it: it
runs the consumer side in its V-Bounce and the real-provider side in its
integration test cycle (§3.6).

---

## 📖 Related Documents

- [`README.md`](README.md) — Full methodology overview and folder map
- [`ONBOARDING.md`](ONBOARDING.md) — Role-based onboarding guide
- [`avenga-devflow/Avenga-DevFlow.md`](avenga-devflow/Avenga-DevFlow.md) — Complete methodology (normative source, v5.2)
- [`functional/user-stories/US-000-non-functional.md`](functional/user-stories/US-000-non-functional.md) — Non-functional container
- [`metrics/README.md`](metrics/README.md) — Manifest family v5 schemas and lifecycle
- [`metrics/manifest-v5-bolt.schema.json`](metrics/manifest-v5-bolt.schema.json) — Normative Bolt manifest JSON Schema
- [`metrics/manifest-v5-us.schema.json`](metrics/manifest-v5-us.schema.json) — Normative US manifest JSON Schema
- [`metrics/manifest-v5-tc.schema.json`](metrics/manifest-v5-tc.schema.json) — Normative TC manifest JSON Schema
