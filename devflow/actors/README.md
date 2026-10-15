# actors/ — who is in the team

**Methodology version:** 5.2

> **Not `agents/`.** `devflow/actors/` is the **roster home**: who is in
> the team, humans and DevFlow Agents together as **actors**. `devflow/agents/`
> holds the **AI-member definitions** — the examples, the templates and
> your project's live agents (`agents/squad/`). A human actor is a roster
> row without a definition file; a DevFlow Agent is a roster row **plus**
> a live definition in `agents/squad/`.

## What is an Actor?

An **Actor** is a **member of the team** — a **human by default**, a
**virtual DevFlow Agent** only by explicit, valid configuration — who
**produces** the governed artifacts its role owns (FA → US, architect →
ADR, developer → SPEC + code, QA → TC/tests) as **executor** and
**participates** in AITL approvals as **approver** when configured, under
the independence floor. The normative definition — grammar, independence
layers, open roles and the safe default — lives in **§3.0.1 The Actor** of
[Avenga-DevFlow.md](../avenga-devflow/Avenga-DevFlow.md); this README only
teaches the concept and points there.

In one picture (the canonical diagram, §3.0.1):

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

## The family shape

`actors/` follows the standard family shape — **one file per actor, one
list for the team**:

| File | Purpose |
|------|---------|
| `roster.yaml` | **The team list — the single membership authority**: one entry per actor referencing its file. An actor file **not listed here is not in the team** |
| `TEMPLATE-ACTOR.yaml` | The template for creating an actor file (the v1 shape — the `capabilities` block returns with the v2 hardening) |
| `roster.schema.yaml` | Validates an actor file (fail fast; the safe default applies until fixed) |
| `INDEX.md` | The family docs index (the *team* lives in `roster.yaml`) |
| `examples/` | The worked examples — `example-human.yaml` (a human actor), `example-agent.yaml` (a DevFlow Agent actor), `example-roster.yaml` (a filled team list, illustrative) — copy as starting points |
| `actors/<actor-id>.yaml` | One file per actor — created from the template, **listed in `roster.yaml`** |

**Each actor carries** `id` (kebab-case — the file/roster handle: the file
name and the `roster.yaml` listing), a project-chosen `name` (e.g. "Arq
Juan", ".NET Architect"), `role`, `modes`, `approves` — DevFlow Agent
actors **also `model` and a `definition` pointer to a live agent in
`agents/squad/`** (never to `agents/examples/`, which an upgrade
refreshes); **humans omit both** (`human:<user>` → model null, §3.0.1)
and **carry `user` instead** (below).
**Definitions are reusable** (N actors : 1 definition — two architects may
share one squad definition), each actor distinct by `id`. The **`model` is
per-instance** (authoritative; enables model-level independence between
actors sharing a definition at `high` risk). **`produces` is derived from
`role`** (the single role → artifacts mapping, §3.0.1) — never a field.

**A human's identity in every record is `human:<user>`, joined through
`user`.** `<user>` is the local part of the person's `git config
user.email` (§3.0), and a human actor file carries it in `user:`
**verbatim** — case kept, never lowercased or stripped, because records
are compared verbatim. Every join from the roster to a record —
`review.reviewers[]`, `checkpoint_approvals[].decided_by[]` and
`written_by[]`, the review↔manifest projection, self-approval routing
(G29) and the independence comparisons that read records — maps a human
actor through `user` to `human:<user>`; the kebab `id` is never the value
compared against a record. The schema accepts ASCII letters, digits,
`.`, `-`, `_` and `+`; a configured local part outside that set is
**reported for a recorded team decision, never normalized**. A DevFlow
Agent actor carries no `user` — it is recorded as `agent:<id>`.

## The roster is the enablement

**Enabling a virtual approver is the human's configuration act — the
roster entry IS the explicit, valid configuration.** A DevFlow Agent whose
schema-valid actor file carries `modes: [approver]` and a non-empty
`approves`, **and is listed in `roster.yaml`**, may occupy those AITL
checkpoint classes exactly like a human — no separate ADR, no policy
switch. The rules that make this safe:

- **Human-authored, always.** A human writes or merges the actor file and
  its `roster.yaml` listing — the git history is the record. **An agent
  never enables its own approval**: the Coordinator may scaffold an actor
  as an executor-only draft, but the authority fields (`modes:
  [approver]`, `approves`) are the human's act. **The commit completes the
  act**: the record materializes when the human commits; an uncommitted
  grant has no audit trail, and an agent never commits it for you (G34).
- **The schema is the validity gate.** A malformed actor file — including
  `approver` in `modes` with an empty `approves` — fails fast, and the
  safe default (humans) applies until fixed.
- **The ceiling is fixed.** `critical` and `regulatory` checkpoints are
  **human-only**, regardless of roster contents (§3.0.1) — not
  configurable.
- **Never a silent flag.** Installing an agent's wrapper never enables
  approval by itself; only the roster grant does.

**What a grant may include — and who decides.** The checkpoint table's
owner designation — *"Qualified humans"* included — is **routing
guidance**, never a hard boundary: any actor, human or agent, may be
granted `DISC`, `REV` or an `AREV-*` class by the team's configuration —
this roster. The one fixed boundary is the `critical`/`regulatory`
human-only ceiling above; the independence rules hold as usual.

## Resolution rules

The roster is a deterministic lookup. The rules:

1. **Role → actors.** A checkpoint's recommended role resolves to the
   actors holding it — humans and DevFlow Agents as peers. An **agent**
   holder counts only for the checkpoint classes **its own `approves`
   grants** (the roster entry is the enablement). A **human** holder's
   `approves` is the go-to approver list — **guidance, never a
   restriction**: a human may approve any checkpoint they are qualified
   for. Only an **agent's** `approves` is a hard grant.
2. **Who produces.** An artifact class resolves to the actors holding the
   role that owns it — the production mapping derives from `role` (the
   single role → artifacts mapping, §3.0.1).
3. **Independence ladder.** `approver.id ≠ executor.id` (the actor floor —
   the executor is the producer of the artifacts under review); at `high`
   risk additionally `approver.model ≠ executor.model` (model hardening —
   the per-instance `model` makes this possible); at
   `critical`/`regulatory` the roster resolves to humans regardless of
   contents. A violating routing is refused.
4. **Definitions shared, actors independent.** Two actors sharing one
   `definition` stay independent at the actor level (distinct `id`) — one
   may approve the other's work at low/medium; at `high`, model hardening
   requires distinct per-instance `model`s.
5. **Membership is the list.** Only actors listed in `roster.yaml` exist
   for the lookup — and every listed id must resolve to an existing
   `actors/<actor-id>.yaml` (the consistency check; the validator tooling
   automates it). **No two human actor files listed there carry the same
   `user`** — a consistency rule of the same class, enforced roster-side:
   the schema validates one file at a time, so a project that wants it
   checked adds its own check over its roster.
6. **Zero-config.** With no roster, an empty list — or no schema-valid
   approver entry — the project behaves byte-for-byte as pure HITL: every
   checkpoint resolves to humans.

## Single-maintainer / human-roster guarantees

The human roster's guarantees hold as a special case of the unified
actors roster:

- **External reviewers.** A single-maintainer team may name an external
  reviewer to fill a required role.
- **A cheap second opinion.** REV and AREV stay optional — but with a
  reviewer agent in the squad, ask for its REV before accepting a Bolt:
  a recommendation, never a gate.
- **Optionality.** An empty roster changes nothing — the methodology
  behaves exactly as before.
- **Migration.** The roster family travels with the §5.16 migration: your
  actor files are carried forward by the living-data signal, and
  `roster.yaml` — framework-shipped but project-filled — keeps its **old**
  value, in the same enumerated class as `LANGUAGE`. The kit's shipping
  `actors: []` never overwrites your team.
- **Upgrading to 5.2 — check the carried actor files.** From 5.2 these
  actor files, valid before, are invalid: a human actor file without
  `user`; a DevFlow Agent actor file whose `definition` does not point to
  `agents/squad/<kebab-id>/agent.yaml`; an executor-only file
  (`modes: [executor]`) with a non-empty `approves`. An invalid file grants
  nothing — the safe default (humans) applies until it is fixed, and
  nothing announces it. After the migration, check every actor file against
  `roster.schema.yaml` and report each invalid one with the rule it breaks.
  The repair is the human's: an agent never writes `modes` or `approves`
  (the human's configuration act) and never invents a `user`.
- **Living data.** Member join/leave updates require no approval — except
  an *approver's* authority fields (`modes`/`approves`), which are the
  **human's configuration act**: a human writes them, an agent never does.

## The grammar in one glance

| Actor | Recorded as | Model |
|-------|-------------|-------|
| Human | `human:<user>` — `<user>` is the actor file's `user` | `null` |
| DevFlow Agent | `agent:<id>` | its declared model (an attribute) |
| The Coordinator (MainAgent) | `agent:avenga-devflow` — one id in all four tools | the model it runs |

## What lives here

- **`roster.yaml`** — the team list (the membership authority; ships
  empty — your team is yours).
- The **actor files** — one per actor (`actors/<actor-id>.yaml`),
  validated by `roster.schema.yaml`, listed in `roster.yaml`.
- The **family shape** — `TEMPLATE-ACTOR.yaml`, `roster.schema.yaml`,
  `INDEX.md`, `examples/`.

**Zero-config unchanged:** with no roster entries — or none declares a
DevFlow Agent approver — every checkpoint resolves to a human actor, pure
HITL, exactly as before.
