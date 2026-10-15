# squad/ — your project's live agents

**Methodology version:** 5.2

This is the **live workspace** of the `agents/` family: one folder per
agent (`squad/<agent-id>/` — `agent.yaml` + `prompt.md`, optionally a
`skills:` list referencing `devflow/skills/<skill-name>/`; the same definition
contract as everywhere, see [`../README.md`](../README.md)).

**The rules:**

- **The Coordinator writes here.** Ask your AvengaDevFlow MainAgent to
  create an agent ("create me a reviewer agent") and it scaffolds the
  definition from [`../TEMPLATE-new-role/`](../TEMPLATE-new-role/) + the
  closest [`../examples/`](../examples/) reference into this folder — then
  installs the platform wrapper into your tool's spawn folder
  (`.claude/agents/`, `.opencode/agents/`, `.github/agents/`,
  `.codex/agents/`) **and the referenced skills into your tool's skills
  surface** — once per skill, shared by every agent that references them —
  per the mapping in [`../VERIFICATION.md`](../VERIFICATION.md).
- **The only roster-referenced folder.** The roster's (`actors/`)
  `definition:` pointers reference `agents/squad/<id>/…` — never
  `examples/`.
- **Ships empty.** The kit delivers no live agents — your squad is yours.
  List each agent in [`../INDEX.md`](../INDEX.md) and add its actor entry
  to the roster.

Reuse is expected: several actors may share one definition (N:1) — each
distinct by its actor `id`, each with its own per-instance `model` — and
**because the definition is role-generic**: it never carries an actor's
name or team-specific references; personalization lives in the actor file
(`actors/<actor-id>.yaml`).

## The lifecycle consistency contract

Every lifecycle act (install · create · delete) must leave the four legs
agreeing — and under N:1 reuse the **cardinality matters**:

- **Every installed wrapper belongs to ONE actor-instance** and points at
  one live definition in `squad/` — two actors sharing a definition have
  **two wrappers** (each with its own actor `id` and per-instance model).
  **Wrapper files are named by the actor `id`** — under N:1, two actors
  produce two distinctly-named wrappers (naming by the definition would
  collide in the spawn folder).
- **Every definition in `squad/`** has its row in [`../INDEX.md`](../INDEX.md)
  and **≥1 actor** referencing it — zero referencing actors makes it a
  deletion candidate (or it gets flagged; never a silent ghost).
- **Every agent actor** (`devflow/actors/<id>.yaml` with a `definition:`)
  is **listed in `roster.yaml`** — an unlisted actor file is not in the
  team: flag it to the human, never silently adopt it.

**Reference atomicity — the definition folder is the unit of every act;
skills are referenced, never carried:**

- **Copy** (example → `squad/`): the definition folder; its `skills:`
  references are **checked** against `devflow/skills/` — a missing skill
  refuses the copy (the skill ships with the kit or you add it first). Skills
  are never copied into the agent folder.
- **Install**: the Coordinator (1) checks reference symmetry (every listed
  skill exists — a definition referencing a missing skill is refused, with
  the reason), (2) projects the wrapper, (3) projects into this platform's
  skills surface the **union** of the skills referenced by the installed
  agents **plus every `shared` skill** — **once per skill**, idempotent: a
  skill already projected by a previous install is verified identical (drift
  → the act fails; it never overwrites silently). Installing the wrapper
  while leaving a referenced skill unprojected is an **incomplete install —
  the act fails**; it never degrades silently. The only sanctioned
  degradation is the never-silent fallback in
  [`../VERIFICATION.md`](../VERIFICATION.md), and it is explicit.
- **Delete** (an agent): removes its wrapper; a projected skill is removed
  only when **no remaining installed agent references it and its scope is
  not `shared`** — a reference count, the N:1 rule below applied to skills.
  Projected skill files belong to the skill, not to an actor-instance.
- **Parity**: the N×4 check regenerates and diffs **wrapper + projected skill
  files** per platform; a projected skill is diffed against its canonical
  `devflow/skills/<skill-name>/` (never against an agent folder), and its
  drift fails parity exactly like wrapper drift. Projected skill files are
  generated artifacts — **never hand-edited**; fix the canonical skill.

**The reference check (before any delete)** — per the shared body: check
`roster.yaml` and the actor files first. Concretely: enumerate every
actor file whose `definition:` points at the target
`squad/<id>/agent.yaml`, and each one's `roster.yaml` listing. Then:

- **Deleting an actor** removes ITS wrapper, its actor file and its
  roster listing — the definition stays while ANY other listed actor
  references it.
- **The definition falls only at zero**: when the enumeration finds no
  remaining referencing actor, the definition (and its INDEX row) may go
  with it.
- **Wrapper-only removal is legitimate**: the actor stops being spawnable
  on that platform until reinstalled; the definition and the roster entry
  stay.

**Edge cases:**

- *Shared definition (N:1):* deleting one actor removes its wrapper +
  actor file + listing; the definition and the other actors' wrappers
  stay.
- *The last actor:* the enumeration hits zero — the definition and its
  INDEX row may go with it.
- *Orphans* (a wrapper without a definition; a definition without an
  INDEX row): repair toward the invariants — reinstall or list — or
  remove; never leave a leg dangling.
- *An unlisted actor file:* outside the team (`roster.yaml` is the
  membership authority) — flag it to the human; the lifecycle never
  adopts it silently.
