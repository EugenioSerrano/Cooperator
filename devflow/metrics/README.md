# Metrics (Manifest Family v5)

**Methodology version:** 5.2

## Purpose

This folder stores the **manifest family** — one machine-readable JSON file
per **User Story**, per **Bolt**, and per **Test Case** — as the mechanical
traceability, timing, and AI-usage record of each artifact's lifecycle (see
§3.12 of [`../avenga-devflow/Avenga-DevFlow.md`](../avenga-devflow/Avenga-DevFlow.md)).

The manifest family records **only**:

1. the artifact (US, Bolt, or TC), its documentary sources and its
   AI-generation usage;
2. every material revision of the canonical SPEC (Bolt manifests), including
   its sources and AI-generation usage;
3. every V-Bounce, the SPEC revision executed, its code-generation usage,
   outcome and mandatory MEM (Bolt manifests);
4. the AITL decisions directly associated with that artifact's lifecycle;
5. the **timing of every step** (`created_at`, `review_ready_at`,
   `review_started_at`, `decided_at`) so lead times, queue times and review
   latencies are measurable;
6. the Bolt's **integration test facts** (Bolt manifests): the
   `test_integration` value confirmed at `AITL-BOLT-READY-Approval`, the
   integration test runs that verified the Bolt's commits, the human
   classification of each of those runs that failed, and a human's late
   waiver of pending integration testing — and, for an integration-test-only
   BUG Bolt, the run that is its red, which needs no classification (§3.3.1).

It is **not** a duplicate project-management system, test report, deployment
log, DORA database or cost ledger. Tests, gates, DORA, deployment, incidents,
costs, AREV phases and review evidence live in their own artifacts. For an
integration test run the manifest records the fact — what was covered,
where, the outcome, who and when — and points to the evidence; the test
results themselves stay in that evidence.

Unlike `memory/` (narrative MEM documents), this folder contains
**machine-readable JSON** designed to be validated and aggregated
automatically by tooling — e.g. the sprint reports in `devflow/reports/`,
whose generator is planned with the tooling track. Recording the manifests
correctly is what makes those reports computable retroactively, so nothing
here waits on that tooling.

---

## Manifest family — the three levels

| Level | Manifest | Folder | Covers | Schema |
|-------|----------|--------|--------|--------|
| **User Story** | `US-NNN-<description>.json` | `user-stories/` | story points, approval timing, child Bolts (ACs live in the US document, not the manifest) | [`manifest-v5-us.schema.json`](./manifest-v5-us.schema.json) |
| **Bolt** | `US-NNN.BOLT-NNN-<description>.json` / `TC-NNN.BOLT-NNN-<description>.json` | `bolts/` | SPEC revisions, V-Bounces, MEMs, AITL, tokens, timing | [`manifest-v5-bolt.schema.json`](./manifest-v5-bolt.schema.json) |
| **Test Case** | `TC-NNN-<description>.json` | `test-cases/` | covered ACs, approval timing, Test Bolts | [`manifest-v5-tc.schema.json`](./manifest-v5-tc.schema.json) |

All three share `schema_version: "5.0"` — the `<major>.0` of the family:
every 5.x methodology version keeps it. A **breaking** schema change means a
new major; an additive one (a new optional field, e.g.
`integration_test_runs[]` or `bolt.test_integration`) keeps `5.0` and
converts no manifest (§3.12). `US-000`
is a container, not a feature US: it has **no** US manifest.

**One family per repository.** `schema_version` names the family that
`devflow/VERSION` declares by its **major**, so a repository never holds two
at once. Within the same major, version upgrades convert no manifest and
change no recorded value; a migration may only add a new fact its own rule
records — the Bolt's confirmed `bolt.test_integration` (§5.16). When the
methodology is upgraded **across a major**, every manifest is re-routed and
converted forward as part of the
migration — lossless: the new schema's fields arrive as `null`
where the value was never captured and the schema admits `null`, and every
recorded value crosses untouched (§3.12, §5.16). Of the timing fields,
`generation.created_at` and `checkpoint_approvals[].decided_at` admit no
`null`: a migration that has to build one reads it from the repository
(`created_at` from the commit that added the document, §5.16) or reports the
manifest unresolved — never `null`. A manifest declaring a `schema_version`
from another family is an unfinished migration, not legacy evidence.

---

## Files in this folder

| File | Role |
|------|------|
| [`manifest-v5-bolt.schema.json`](./manifest-v5-bolt.schema.json) | **Normative JSON Schema** (draft 2020-12) for Bolt manifests. Strict: `additionalProperties: false` — unknown fields fail validation. |
| [`manifest-v5-us.schema.json`](./manifest-v5-us.schema.json) | **Normative JSON Schema** for User Story manifests. |
| [`manifest-v5-tc.schema.json`](./manifest-v5-tc.schema.json) | **Normative JSON Schema** for Test Case manifests. |
| [`TEMPLATE-MANIFEST-BOLT.json`](./TEMPLATE-MANIFEST-BOLT.json) | Example Bolt manifest (**functional** Bolt, origin `AITL-US-Approval` recorded at creation). Copy the example matching your Bolt type and replace values. |
| [`TEMPLATE-MANIFEST-BOLT-NONFUNCTIONAL.json`](./TEMPLATE-MANIFEST-BOLT-NONFUNCTIONAL.json) | Example Bolt manifest (**non-functional** Bolt under `US-000`): `checkpoint_approvals[]` starts with **no origin decision** — US-000 has no approval lifecycle. |
| [`TEMPLATE-MANIFEST-BOLT-TEST.json`](./TEMPLATE-MANIFEST-BOLT-TEST.json) | Example Bolt manifest (**Test** Bolt under one `TC-NNN`): origin `AITL-TC-Approval` included when already recorded, QA-side approvers. |
| [`TEMPLATE-MANIFEST-US.json`](./TEMPLATE-MANIFEST-US.json) | Example User Story manifest. |
| [`TEMPLATE-MANIFEST-TC.json`](./TEMPLATE-MANIFEST-TC.json) | Example Test Case manifest. |

---

## Naming conventions

```
bolts/US-NNN.BOLT-NNN-<description>.json      (functional and non-functional Bolts)
bolts/TC-NNN.BOLT-NNN-<description>.json      (Test Bolts)
user-stories/US-NNN-<description>.json        (feature User Stories)
test-cases/TC-NNN-<description>.json          (Test Cases)
```

The Bolt manifest filename is the Bolt filename with `.md` replaced by
`.json`; the US and TC manifest filenames are their document filenames with
`.md` replaced by `.json`.

---

## The timing contract (every step, every level)

Every manifest records the timestamp of **each step** of its lifecycle. All
timestamps are RFC 3339 with seconds (`YYYY-MM-DDTHH:mm:ss±HH:MM`, or `Z` for
UTC) and come from the artifact's
own review contract (§3.0) and generation records — never invented, never
estimated.

| Step | Field | Where |
|------|-------|-------|
| Artifact created | `generation.created_at` | artifact object (US/Bolt/TC), SPEC revision, V-Bounce code, MEM |
| Ready for human review | `review_ready_at` | artifact object, each SPEC revision, each V-Bounce |
| Review started by the human | `review_started_at` | artifact object, each SPEC revision, each V-Bounce |
| Decision recorded | `checkpoint_approvals[].decided_at` | approval array |

All four fields are **required**; only the review timestamps are
**nullable**. `review_ready_at` and `review_started_at` — on the artifact
object, on each SPEC revision and V-Bounce, and in the Bolt's `acceptance`
object — are `null` until their step happens, which forces the discipline
without blocking early lifecycle stages. `generation.created_at` and
`checkpoint_approvals[].decided_at` are **never `null`**: a `generation`
block is written only once its artifact exists and a decision entry only once
the decision is recorded, so each carries its instant from its first write.
The three schemas draw exactly this line: the review timestamps are their
`dateTimeOrNull` fields, `created_at` and `decided_at` plain `date-time`.

**Measurable times (derived at report time, never stored):**

| Metric | Derivation |
|--------|------------|
| Queue time (waiting for human) | `review_started_at` − `review_ready_at` (Time-to-Human-Review, target < 4 h, §3.0) |
| Active review time | `decided_at` − `review_started_at` |
| Total review latency | `decided_at` − `review_ready_at` (informational, no target) |
| Bolt lead time | BOLT-DONE `decided_at` − BOLT-READY `decided_at` (§3.7) |
| US lead time | last child Bolt BOLT-DONE `decided_at` − US `AITL-US-Approval` `decided_at` |
| V-Bounce cycle | MEM approval `decided_at` − code `generation.created_at` |
| AI generation time | `generation.duration_seconds` |
| Time in integration testing | latest approved MEM `decided_at` → passing covering run `ran_at`; Bolts closed by a late waiver excluded (§3.7.2) |

---

## Manifest lifecycle

### User Story (`user-stories/US-NNN-*.json`)

| Moment | Action |
|--------|--------|
| **US creation (draft)** | Copy [`TEMPLATE-MANIFEST-US.json`](./TEMPLATE-MANIFEST-US.json) and fill it — `us` (`id`, `ref`, `sources`, `generation`), `story_points: null`, empty `bolts` and empty `checkpoint_approvals`. Keep every field the template carries (*Complete from the first write*, below). |
| **US ready for review** | Set `us.review_ready_at` when the US enters review. |
| **US review** | Set `us.review_started_at`; append the `AITL-US-Approval` decision; set `story_points` to the confirmed value. |
| **Each child Bolt created** | Append the Bolt's `ref` to `bolts[]`. |

### Bolt (`bolts/*.json`)

| Moment | Action |
|--------|--------|
| **Bolt creation** | Copy the matching `TEMPLATE-MANIFEST-BOLT*.json` and fill it — `bolt` (`id`, `type`, `ref`, `sources`, `generation`), empty `spec_revisions` and empty `v_bounces`. Keep every field the template carries, `bolt.acceptance` included, **except `bolt.test_integration`** — leave it out: the examples carry a confirmed value, and it is first written at READY (*Complete from the first write*, below). `checkpoint_approvals[]` carries the **origin decisions that already exist** at this point, one per Bolt type: `AITL-US-Approval` (functional), `AITL-TC-Approval` (test), none (non-functional under US-000). A BUG-driven Bolt additionally carries `AITL-BUG-Approval`. |
| **Bolt ready / review** | Set `bolt.review_ready_at`, then `bolt.review_started_at`; append `AITL-BOLT-READY-Approval` and, in the same act, set `bolt.test_integration` to the `test_integration` value the decision confirms (an amendment's re-approval overwrites it). Only an approved Bolt may enter SPEC generation. |
| **SPEC generation or material revision** | Append one `spec_revisions[]` entry with its sources, repository baseline, generation usage and `review_ready_at`. |
| **SPEC review** | Set the revision's `review_started_at`; append the matching `AITL-SPEC-Approval` decision (with `subject.revision`). |
| **V-Bounce** | Append one `v_bounces[]` entry with its eight required fields: `number`, `spec_revision`, `git_commit`, `execution_outcome`, `code_generation`, `mem` (exactly one), `review_ready_at` and `review_started_at` — the last one `null` until the MEM review begins (§3.12). |
| **MEM review** | Append the matching `AITL-MEM-Approval` decision (with `subject.v_bounce`). |
| **Integration test run / classification / waiver** | Append one `integration_test_runs[]` entry: a `run` when a run finishes (whoever runs it), a `classification` when a human classifies a failed run, a `waiver` when a human waives pending integration testing (§3.2.2). |
| **Bolt acceptance** | Set `bolt.acceptance.review_ready_at` in the same act as the Bolt's transition into `ready-for-acceptance` (§3.2.3) — never at an earlier status; then `bolt.acceptance.review_started_at`; append `AITL-BOLT-DONE-Approval` (the acceptance review is a second review of the same artifact — readiness and acceptance timings coexist, §3.0, §3.12). A second acceptance round keeps the first submission in `bolt.acceptance`; the later round is read from its own `AITL-BOLT-DONE-Approval` entry. |

### Test Case (`test-cases/TC-NNN-*.json`)

| Moment | Action |
|--------|--------|
| **TC creation (draft)** | Copy [`TEMPLATE-MANIFEST-TC.json`](./TEMPLATE-MANIFEST-TC.json) and fill it — `tc` (`id`, `ref`, `sources`, `generation`), `verifies` (source Bolt + source US + covered ACs), empty `test_bolts` and empty `checkpoint_approvals`. Keep every field the template carries (*Complete from the first write*, below). |
| **TC ready / review** | Set `tc.review_ready_at`, then `tc.review_started_at`; append `AITL-TC-Approval`. |
| **Each Test Bolt created** | Append the Test Bolt's `ref` to `test_bolts[]`. |

**Complete from the first write:** every schema is
`additionalProperties: false` **and** lists its lifecycle fields as
`required`, so a **missing** field fails validation exactly like an extra
one (G23). A manifest is therefore written complete from creation, with
`null` wherever the step has not happened yet — never with the field left
out. That is what a `required` field that admits `null` buys — only the
review timestamps do (*The timing contract*, above): `null` records
*"not yet"*, an absent field records nothing and is indistinguishable from
an oversight. The easiest to forget are the ones no lifecycle step has
touched at creation time: `review_ready_at` and `review_started_at` at every
level, and the Bolt's `acceptance` object with both of its own. Copy the
matching `TEMPLATE-MANIFEST-*.json` and the shape is already right; the
three `manifest-v5*.schema.json` files remain the contract.

**Append-only rule:** `spec_revisions[]`, `v_bounces[]`, `bolts[]`,
`test_bolts[]`, `checkpoint_approvals[]` and `integration_test_runs[]` are
never rewritten. Corrections are represented by new revisions, new
V-Bounces, new decisions or new integration test entries.
`bolt.test_integration` is the one recorded value that is overwritten — by an
amendment's re-approval at READY, or by the human's change to a value a
§5.16 migration proposed for confirmation (row I4); a value the migration
reports unresolved (row I5) is first written when the human supplies it,
which overwrites nothing (§3.12). Lifecycle
timestamps (`review_ready_at`, `review_started_at`) are written once, in
order; they are never backdated.

---

## Schema family v5 — structure

The normative contracts are the three `manifest-v5*.schema.json` files.
Key shared structure:

### Common (all three)
- `schema_version` — exactly `"5.0"`.
- Artifact object (`us` / `bolt` / `tc`): `id` (pattern per type), `ref`,
  `sources`, `generation`, `review_ready_at`, `review_started_at`.
- `generation` — `created_at` (ISO 8601), `created_by` (the **accountable
  human**, `human:<user>` — the same identity as the document's `author:`;
  never an agent, a bare name or the model name. The schema still accepts
  `agent:<id>` there only so that existing records stay valid; for manifests
  created on or after the repository's adoption of this version — an
  upgrade's §5.16 entry in the root `CHANGELOG.md`, a fresh project's
  installation — the human-only rule is the validator's to enforce, and
  until a validator implements it the methodology text, the Coordinators'
  instructions and these examples carry it, §3.12), `duration_seconds`,
  `runs[]` (tool, provider, model, tokens and `agent` — its five required
  fields). A run's `agent` records, **bare**, the id of the agent that
  produced that run — e.g. `"avenga-devflow"` for the Coordinator,
  `"developer-agent"` for a spawned role agent — never the delegating or
  spawning agent's, however deep the delegation, and `null` for a run no
  agent performed. Fully manual authoring: `created_by` populated, `runs`
  empty.
- `checkpoint_approvals[]` — lifecycle decisions with `checkpoint`,
  `subject`, `decision` (`approved` | `changes_requested` | `rejected`),
  `decided_by` (array of `{actor, role, model}`), `decided_at`, `comment`,
  and the optional `written_by` (the same shape: who wrote the record — the
  signer, or the agent that transcribed it; writing confers no signature).
  Absent or `[]` means "not recorded", and no earlier record is backfilled;
  decisions recorded from the repository's adoption of this version carry
  it — a rule for the validator, never for the schema (§3.12).

### User Story manifest
- `story_points` — confirmed Fibonacci value `1|2|3|5|8|13`, or `null` until
  `AITL-US-Approval` (§2.6).
- `bolts[]` — refs of **every** child Bolt created under this US, BUG-driven
  ones included (a BUG-driven Bolt is still a child of the feature US).
- `checkpoint_approvals[]` — `AITL-US-Approval` decisions only.

### Bolt manifest
- `bolt.type` — `functional` | `non-functional` | `test`; `id` pattern
  enforced per type.
- `bolt.test_integration` — **optional**: `required` | `not-required` |
  `null`. The Bolt's `test_integration` as `AITL-BOLT-READY-Approval`
  confirmed it — written in that act, overwritten by an amendment's
  re-approval or by the human's change to a migration-proposed value (§5.16
  row I4), absent until then; `null` means "not recorded" and no new act
  writes it. The reason is never copied, and the Bolt document governs a
  mismatch (§3.12). With the waiver entries it tells apart the three ways a
  Bolt reaches `done` without integration testing (§3.7.2).
- `spec_revisions[]` — one entry per material revision (revision ≥ 1), with
  `ref`, `sources`, `git_commit`, `generation`, `review_ready_at`,
  `review_started_at`.
- `v_bounces[]` — `number`, `spec_revision`, `git_commit`,
  `execution_outcome` (`ready_for_review` | `failed` | `blocked` |
  `cancelled`), `code_generation`, `mem` (exactly one: `ref` + `generation`),
  `review_ready_at`, `review_started_at`. Internal autonomous retries stay
  inside the V-Bounce and accumulate in `code_generation`.
- `checkpoint_approvals[]` — Bolt-lifecycle decisions only: `AITL-US-Approval`,
  `AITL-BUG-Approval`, `AITL-TC-Approval`, `AITL-BOLT-READY-Approval`,
  `AITL-SPEC-Approval` (subject requires `revision`), `AITL-MEM-Approval`
  (subject requires `v_bounce`), `AITL-BOLT-DONE-Approval`.
- `integration_test_runs[]` — **optional**, append-only (absent and `[]` both
  mean "no integration test fact recorded"). Three entry kinds, told apart by
  `kind`: a **`run`** (`number` ≥ 1, sequential over run entries; `covers` — a
  commit `<sha>` or a range `<sha>..<sha>`; `environment`; `outcome` `pass` |
  `fail` | `blocked`; `reason` — one line, required for `blocked`, otherwise
  `null` or a line; `evidence` — a CI run URL or repository path, never the
  results; `ran_by` — `human:<user>` or `agent:<id>`; `runner` — the executing
  system, or `null`; `ran_at`), a **`classification`** of a failed run (`run`
  — the run's `number`; `cause` `code` | `test` | `spec` | `environment`; a
  one-line `reason`; `classified_by`, always `human:<user>` — the schema
  requires it; `classified_at`) and a **`waiver`** of pending integration
  testing (`covers` — as a run's; `closes_runs` — the `number`s of the runs it
  closes, `[]` when none ran; a one-line `reason`; `compensating_control` — an
  evidence reference or `null`, the key always present; `waived_by`, always
  `human:<user>` — the schema requires it; `waived_at`). The meaning of every
  outcome, cause and waiver key is in §3.12.

### Test Case manifest
- `verifies` — `source_bolt` (the exact product Bolt verified), `source_us`
  (the US under verification) and `covered_acs[]`.
  `source_us` is the **US identifier** (`US-NNN`, or `US-000` for
  non-functional TCs) — it mirrors the TC document's frontmatter; the full
  repository paths of the sources live in `tc.sources`. For a non-functional
  TC (derived from `US-000.BOLT-NNN` + governing technical sources) there
  are no ACs: `covered_acs` is then an empty array and the governing
  sources appear in `tc.sources`.
- `test_bolts[]` — refs of the Test Bolts created from this TC.
- `checkpoint_approvals[]` — `AITL-TC-Approval` decisions only.

---

## Relationship with review evidence

The manifest is the **minimal lifecycle projection** of each approval; the
authoritative review evidence lives in the governed artifact's own
machine-readable `review:` contract (§3.0):

| Artifact field | Manifest projection |
|----------------|---------------------|
| `review_ready_at` | artifact `review_ready_at` (copied) |
| `review.started_at` | artifact `review_started_at` (copied) |
| `review.reviewers` | `checkpoint_approvals[].decided_by` (each as a `human:<user>` / `agent:<id>` actor) |
| `review.written_by` | `checkpoint_approvals[].written_by` (copied when present; absent stays absent) |
| `review.decision` | `checkpoint_approvals[].decision` |
| `review.decided_at` | `checkpoint_approvals[].decided_at` |
| `review.findings` | may be summarized in the optional `comment` |

Complete findings and acknowledgment fields are **not** copied to the
manifest. A mismatch between the artifact evidence and its manifest projection
is a validation error; it does not transfer authority to the manifest.

---

## Deliberately outside manifest v5

The following remain in their own artifacts and are never stored in the
manifests:

- test and gate results, TDD evidence and modified-file lists → MEM and CI evidence — an integration test run's outcome and the reference to its evidence are recorded in `integration_test_runs[]`; the results stay in the evidence;
- risk class, autonomy level and data classification → Bolt / SPEC frontmatter;
- DORA metrics, deployment, promotion, UAT and incident data → CI/CD, incidents/, tests/uat/;
- pre-calculated monetary cost;
- AREV phases, selected agents/models and approvals → AREV artifacts;
- PR lists and any state already derivable from the artifacts and AITL decisions.

`integration_test_runs[]` does not contradict this list. The manifest records
that an integration test run happened, what it covered, its outcome, who ran
it and when — and who waived pending integration testing, for which change and
why — and points to the evidence, as `execution_outcome` and `mem.ref` already
do for a V-Bounce. Test details, gate figures and logs stay in the evidence.

---

## Progress states (derived) and the Bolt status (declared)

US and TC progress are derived from the children's declared Bolt statuses
(§3.12); the Bolt's `status` is declared in its document (§3.2.3). The table
is the projection.

| Artifact | State | Derived from |
|----------|-------|--------------|
| User Story | `draft` → `approved` → `in_progress` → `completed` | US approval; a child Bolt from `ready` to `done`; every child Bolt `done` or `deprecated`, at least one `done` |
| Bolt | `status` — the eleven values of §3.2.3 | declared in the Bolt document and changed by the transition table; never derived, never in a manifest |
| Test Case | `draft` → `approved` → `automated` | TC approval; any Test Bolt `done` |

Derived states are never duplicated as mutable manifest fields, and no
manifest holds the Bolt status.

---

## Example flow

```
US-012 drafted → US manifest created with the document (draft, §3.12/G33)
   → US-012 approved (AITL-US-Approval) → US manifest updated
   → Bolt created in functional/bolts/ + manifest in metrics/bolts/
     (origin AITL-US-Approval already recorded)
   → AITL-BOLT-READY-Approval appended, bolt.test_integration recorded
   → SPEC generated → spec_revisions[1]
   → AITL-SPEC-Approval (changes_requested) → revision 2 → approved
   → V-Bounce 1 (spec_revision 2) → tests green → MEM → v_bounces[1]
   → AITL-MEM-Approval (approved: Development Completed) → test_integration required, plan applicable
   → status in-integration-testing → run 1 fail → integration-tests-failed
   → classified environment → integration-tests-blocked
   → run 2 pass (Integration Tests Passed) → ready-for-acceptance (acceptance.review_ready_at)
   → AITL-BOLT-DONE-Approval → status done → US manifest reflects it
   (every status change also appends a row to the Bolt's Status timeline, §3.2.3)
```

See the five `TEMPLATE-MANIFEST*.json` files for complete worked examples.

---

## Relationship with other folders

| Folder | Relationship |
|--------|-------------|
| `functional/user-stories/` | Defines the US → US manifest lives in `metrics/user-stories/` |
| `functional/bolts/` | Defines the Bolt → Bolt manifest lives in `metrics/bolts/` |
| `tests/test-cases/` | Defines the TC → TC manifest lives in `metrics/test-cases/` |
| `spec/` | Each canonical SPEC revision is recorded in `spec_revisions[]` |
| `memory/` | Each V-Bounce MEM is paired with a `v_bounces[]` entry |
| `reports/` | Sprint reports aggregate the whole family |
| `avenga-devflow/` | Methodology §3.12 defines this contract |

---

## Language

JSON keys, enum values, and IDs stay in **English** (the schema). Free-text
fields (`comment`) go in the project's `content_language` (see
[`devflow/README.md`](../README.md) → Language policy, §3.15).
