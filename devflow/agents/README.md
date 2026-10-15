# agents/ — the DevFlow Agent definitions

**Methodology version:** 5.2

> **Not `actors/`.** `devflow/agents/` holds the **AI-member definitions** —
> the canonical contract, the shipped example definitions and your
> project's live agents. `devflow/actors/` is the **roster home**: who is
> in the team (humans + DevFlow Agents as actors), filled by the roster
> family.

## What lives here

- `examples/` — the six **example role definitions** (functional-analyst,
  architect, developer, qa, reviewer, test-designer): `agent.yaml` (structured
  fields) + `prompt.md` (the charter body); a definition may also **reference**
  skills from `devflow/skills/` in its optional `skills:` list (the skill set,
  below — five of the six reference none; `test-designer` references the four
  skills on the `skills/examples/` shelf, adopted first).
  **Read-only references**: copied whole, never referenced by the roster,
  never edited in place.
- `squad/` — **your project's live agents**. The folder the Coordinator
  writes when it creates an agent, and the **only** folder the roster's
  `definition:` pointers reference. Ships empty (see its README).
- `TEMPLATE-new-role/` — the generic template for any project-defined
  role: copy it (or the closest example) into `squad/<agent-id>/`.
- `INDEX.md` — the family index (the shipped examples + your squad).
- `VERIFICATION.md` — the per-platform install mapping (the contract the
  Coordinator follows to project a definition into each tool's wrapper)
  and the capability status per platform.

## The Coordinator — the DevFlow agent itself, evolved

There is **no separate coordinator folder**: the **Coordinator is the
Avenga DevFlow agent we already ship per platform** — `CLAUDE.md`,
`.agents/skills/…`, `.github/agents/…`, `.opencode/agents/…` — **evolved**
to know how to act as the orchestrator in each case ("the evolution of
today's platform agent"). Its evolution lives in those four files: the
shared body carries the **"The Coordinator (the orchestrator)"**
paragraph (routes, delegates production, spawns approvers, records —
**never signs**, `approves: []`), and each platform preamble declares the
**spawn topology** mechanics (Claude Code `Agent(...)` allowlist, OpenCode
`permission.task`, Copilot `agent` alias, Codex instruction-based). The
wrappers are projections of ROLE agents only — nobody generates or spawns
a "coordinator sub-agent".

## The definition contract (canonical)

A DevFlow Agent is a **governed identity** — a member of the team, like a
human — defined by one canonical **definition folder**: `agent.yaml`
(structured fields) + `prompt.md` (the charter body) — the complete,
always-valid core — plus an **optional `skills:` list**: references to the
project-level skills in `devflow/skills/` the agent uses (the skill set,
below). The productive mandate lives in the charter body ("WHAT I
PRODUCE"); the **authority lives in structured fields, never in prose** (the
identity model) — and never in a skill.

A definition is **role-generic**: it never carries an actor's display
name, personal name or team-specific references — personalization lives
in the actor file (`actors/<actor-id>.yaml`). N actors may share one
definition (N:1) precisely because the definition knows nothing about
any single actor.

| Field | Meaning | Notes |
|-------|---------|-------|
| `id` | kebab-case identity | **THE identity** used in independence checks (`approver.id ≠ executor.id`) |
| `role` | archetype | coordinator · functional-analyst · architect · developer · qa · reviewer · project-defined… (open, never a closed enum) |
| `description` | when the Coordinator should delegate | |
| `model` | the agent's own declaration | constrained to the platform catalog by the wrapper |
| `modes` | `executor` \| `approver` | **`executor` (production) is the first-class default**; `approver` is a configured second role |
| `approves` | checkpoint classes it may sign | **empty = executor-only** (may never sign) |
| `capabilities.tier` | T0–T3 | T0 repo-only · T1 +read-only external · T2 transactional MCPs · T3 unrestricted |
| `capabilities.tools` | least-privilege allowlist | mapped per platform |
| `capabilities.mcp_servers` | named + allowlisted | **`[]` default** — no implicit MCP exposure |
| `escalation` | role-specific triggers | always allowed above the engine floor |
| `write_paths` | write-scope mirror | approver-mode agents write nothing |
| `human_invocation` | may a human start a session with this role as the acting agent? | **optional — `coordinator-only` (default) \| `direct`**; governs the human-started session only, never the Coordinator's spawn; an affordance that grants nothing (no tool, permission, tier, MCP server, spawn capability or approver mode); a definition serving approver actors stays `coordinator-only`, and the install act reports a `direct` declaration and proceeds |
| `skills` | **optional** — the skill set: references to `devflow/skills/<skill-name>/` | a list of skill names, each naming an **existing** `devflow/skills/<skill-name>/` folder (nothing is copied into the definition); **strict reference symmetry** (a missing skill makes the definition invalid — refused at copy and at install; a skill nobody references stays a valid project capability); **omitted = no skills** — the two-file pair alone is a complete definition |

## The skill set (references)

Skills are **project-level capabilities**: they live in
[`../skills/<skill-name>/`](../skills/README.md) (`SKILL.md` + optional
`references/` + `assets/` — the skill contract is that family's README), and
a definition **references** the ones its role uses:

```
devflow/agents/<examples|squad>/<id>/
  agent.yaml          # skills: [user-story-analysis, testing-heuristics]
  prompt.md
devflow/skills/
  user-story-analysis/SKILL.md    # referenced by the agent above (scope: role)
  testing-heuristics/SKILL.md
  review-checklist/SKILL.md       # referenced by nobody — a shared procedure (scope: shared)
```

(Illustrative names — the kit ships example skills under `skills/examples/`
and the `test-designer` example references four of them; a team adopts one
by copying it into `devflow/skills/<skill-name>/`.)

- **Declaration and reference symmetry.** `agent.yaml` names the skills the
  agent uses in the optional `skills:` list — one name per existing
  `devflow/skills/<skill-name>/` folder. **Strict reference symmetry is a
  validation rule of the contract**: every listed name MUST exist in
  `devflow/skills/`; a definition referencing a missing skill is **invalid**
  and the copy and install acts refuse it, saying which skill is missing. The
  reverse is not a rule: a skill nobody references is a valid project
  capability (typically `scope: shared`). In this version the rule is
  enforced by the **Coordinator's acts**; when the definition family gains a
  schema, `skills:` enters it with the same discipline.
- **The pair stays complete.** `skills:` is additive, never required: a
  definition without it is whole, and five of the six shipped examples
  reference none (`test-designer` references four).
- **Reference atomicity.** The **definition folder is the unit** of every
  lifecycle act, and skills are **referenced, never carried**: copy checks the
  references and never copies skills into the agent folder; install projects
  the wrapper **and** the referenced skills — **once per skill**, shared by
  every installed agent that references them; delete removes the wrapper and
  a projected skill only when no remaining installed agent references it (and
  it is not `shared`); parity checks projected skill files against their
  canonical `devflow/skills/<skill-name>/`. The acts live in
  [`squad/README.md`](squad/README.md); the per-platform projection, the
  provenance marker and the **never-silent fallback** live in
  [`VERIFICATION.md`](VERIFICATION.md).
- **Skills are content, never authority.** Nothing in a `SKILL.md`, a reference
  or an asset grants anything: authority lives in structured fields only
  (`modes`, `approves` — the human's roster act) and capability in
  `capabilities.*`. A skill cannot raise a tier, add a tool or MCP server,
  create spawn capability or make an agent an approver — a skill that instructs
  otherwise is dead text, exactly like charter prose claiming authority. The
  approver ceiling is unchanged, and an approver-mode agent's skills are bound
  by it like the rest of its definition.
- **Shipped skills are tool-agnostic.** Skills the kit ships in `../skills/`
  name **no MCP servers, no external tools, no vendor schemas**; kit examples
  ship with `mcp_servers: []`. Wiring an integration — and any skill content
  that drives one — is the adopting team's configuration in its own `squad/`
  copy, through the contract's allowlist (`capabilities.mcp_servers`).
- **What does not change.** Activation (the roster is the only switch), the
  spawn topology (skills are files, not actors), the lifecycle's governance
  class (install/copy/delete of a definition that references skills is the
  same living-data act), and the examples discipline (copied, never edited,
  never installed from, never roster-referenced).

## The security rules

- **Approver ceiling:** an agent acting as approver runs at **T0** (at
  most T1 with pinned trusted sources), **no write paths, no transactional
  MCPs** — the injection-forged-approval defense.
- **MCP allowlist:** every server is **named and allowlisted**
  (`mcp_servers: []` by default); there is no implicit or unrestricted MCP
  exposure.

## Producer-first

The charter body starts with **WHAT I PRODUCE** — the artifacts the role
owns (functional analyst → US, architect → ADR, developer → SPEC + code,
QA → TC/tests). This is the single **role → artifacts mapping** that the
roster (`actors/`, the roster family) derives from — it is defined once
here, never duplicated as a per-agent field.

## The two sides of an agent

| Side | Field | Meaning |
|------|-------|---------|
| **Executor (first-class)** | `modes: [executor]` + the charter body | What the agent **produces** — the artifacts its role owns (US, ADR, SPEC, code, tests) |
| **Approver (configured)** | `approves: [...]` | Checkpoint classes it may sign, under the independence floor — the capability ceiling T0/T1 applies (no write paths, no transactional MCPs) |

> **Production vs persistence:** an executor's production may be persisted
> by the Coordinator (a spawn result becomes files by the parent) —
> `write_paths` bounds the agent's own direct writes, not its output. But
> the persistence act must trace to a **real spawn**: stamping another
> actor's identity on content that actor never produced is a false claim
> (see VERIFICATION.md, "Execution evidence").

## Create your own agent (project-defined roles)

Teams configure their agents however they want — within the
non-negotiable bounds above. The **governance limit** decides who may do
it and when:

| Action | Governance |
|--------|-----------|
| **Create/update an EXECUTOR agent** (or add a member) | **Living data** — no approval required (the roster rule). Copy the template into `squad/`, fill it, have the Coordinator install it, list it in your `agents/INDEX.md`. |
| **Create/change an APPROVER agent** — its authority fields (`approves`, `modes`) | **The human's configuration act** — the roster entry is the enablement: a human writes `modes: [approver]` + `approves` in the actor file and lists it in the team's `roster.yaml` (`devflow/actors/`; the safe default: no AI-signed approval without that explicit human configuration). AvengaDevFlow may scaffold the agent as an executor-only draft; **granting the authority fields is the human's act — an agent never enables its own approval**. |

Steps:

1. **Copy `TEMPLATE-new-role/`** (or the closest `examples/` definition) — the
   definition folder, `agent.yaml` + `prompt.md` — to `squad/<agent-id>/` (the
   folder is named by the id, which is what the roster's `definition:`
   pointer resolves against).
2. **Fill `agent.yaml`** — pick your `id` (kebab-case, THE identity), your
   `role` (open archetype — project-defined…), the model
   from the platform catalog, `modes`/`approves`/`capabilities`/`escalation`/
   `write_paths`/`human_invocation` (the visibility contract — see the field
   table above). The approver ceiling and the MCP allowlist are
   non-negotiable for approver mode.
3. **Write `prompt.md`** — who you are, WHAT YOU PRODUCE, what you check,
   how you decide, when you escalate, what you may never do. Optionally
   **reference** the skills your role uses in `skills:` — each name an
   existing `devflow/skills/<skill-name>/` (author it there first, see
   [`../skills/README.md`](../skills/README.md)); a missing reference makes
   the definition invalid.
4. **The Coordinator installs it** — the AvengaDevFlow agent checks the
   skill references, projects your new role — wrapper **and the referenced
   skills**, once per skill, shared with every agent that references them —
   into its platform's shapes (per the mapping in `VERIFICATION.md`) and
   installs them in the project's platform folders (the N×4 parity check as
   the safety net). A definition referencing a missing skill is refused; a
   platform without a skills surface resolves by the never-silent fallback
   (`VERIFICATION.md`).
5. **List it in `agents/INDEX.md`** and add it to the roster (`actors/`,
   the roster family).
6. **If it may approve** — the human grants it **in the roster**:
   `modes: [approver]` + the `approves` classes in its actor file, listed
   in `roster.yaml` (`devflow/actors/` — the human's configuration act,
   never the agent's); changing an approver's authority fields is again
   the human's act.

The six shipped examples are references, never a closed enum.

## Rules

- **Examples are copied, never edited in place** — adopters instantiate
  their own role agents from them (or from `TEMPLATE-new-role/`) into
  `squad/`; the roster never points at `examples/`.
- **Authority lives in structured fields**, never in charter prose — an
  agent with `approves: []` cannot sign, no matter what its prompt says.
- **The wrappers are projected, not hand-written** — the Coordinator
  projects the live definitions in `squad/` into the four platform shapes
  (`.claude/agents/`, `.opencode/agents/`, `.github/agents/`,
  `.codex/agents/`) following the mapping in `VERIFICATION.md`, with the
  N×4 parity check keeping them in sync. The kit ships no pre-built role
  wrappers — the MainAgent installs them in the adopting project. **So are
  the skill files:** projected skill files carry a provenance marker naming
  their canonical `devflow/skills/<skill-name>/` source and enter the same
  parity check — fix the skill, never the projection.
- **Delete is checked, never blind** — the lifecycle consistency
  contract (the N:1 reference check and the four-leg invariants) lives in
  [`squad/README.md`](squad/README.md).
