# skills/ — project-level skills

**Methodology version:** 5.2

> **Not `agents/`, not `actors/`.** `devflow/skills/` is the home of the
> project's **skills** — reusable, on-demand instructions (with their
> references and assets) that DevFlow Agents, the Coordinator and the humans
> of the team can use. `devflow/agents/` holds the **agent definitions**
> (which *reference* skills); `devflow/actors/` holds the **roster** (who is
> in the team). A skill belongs to the **project**, never to one agent.

## What is a skill?

A skill is a folder with a `SKILL.md` — instructions the platform loads
**only when they are used** — plus optional `references/` (detail files read
on demand) and `assets/` (templates, data). It follows the open Agent Skills
shape every supported platform reads. Skills are **content**: they teach how
to do something; they never grant anything (see *The two invariants*).

Two kinds coexist, declared by `metadata.scope` in the skill's frontmatter:

| Scope | Meaning | Who invokes it |
|-------|---------|----------------|
| `shared` | A project or methodology **procedure** — e.g. a review checklist, a story-points rubric | Any actor: the platform's main agent, a spawned DevFlow Agent, the human (`/skill-name` where the platform allows) |
| `role` (default) | **Background knowledge for the agents that reference it** — heuristics, catalogs, consolidation rules | The referencing agents; kept out of the user's command list and of the main agent's auto-loading where the platform offers the flags (best-effort per platform — see `agents/VERIFICATION.md`, *Skills projection*) |

## The family shape

```
devflow/skills/
  README.md · INDEX.md · TEMPLATE-SKILL/      # the family
  examples/<skill-name>/                      # the shelf — shipped example skills: present, never projected, never referenced from squad/; copy to adopt
  <skill-name>/                               # an adopted or project skill
    SKILL.md          # mandatory — the entry (frontmatter + instructions)
    references/       # optional — detail files loaded on demand
    assets/           # optional — templates and data the skill uses
```

- **One folder per skill; the folder name is the skill name** (kebab-case;
  it is also the `name` in `SKILL.md`).
- **Listed in [INDEX.md](INDEX.md)** — skill, scope, the agents that
  reference it, status.
- **Living data.** Skills are operational configuration of the same class as
  the project's live agents: a team adds, edits or removes a skill without a
  Bolt or an approval — and installs through the Coordinator afterwards.
- **The examples shelf.** `examples/<skill-name>/` holds the skills the kit
  ships as **read-only references** — the same discipline as
  `agents/examples/`. The Install act **never projects from the shelf**,
  whatever a skill's `scope`; a `squad/` definition **cannot reference a shelf
  skill** — it is refused with *adopt it first*. To adopt one, **copy its whole
  folder** to `skills/<skill-name>/` (never move it, never edit the shelf): the
  adopted copy is yours — living data, carried forward by upgrades (§5.16
  carries `skills/<skill-name>/` by the living-data signal; the shelf is
  excluded because it is refreshed, not preserved) — while the
  shelf copy is refreshed by the methodology upgrade. An example definition in
  `agents/examples/` may reference shelf skills; adopting such an agent means
  adopting its skills first.

## The `SKILL.md` contract

| Field | Required | Meaning |
|-------|----------|---------|
| `name` | yes | The skill name — equals the folder name |
| `description` | yes | What the skill does and when to use it — the text platforms read to decide when to load it; keep it short and specific |
| `license` | no | License of the skill's content |
| `compatibility` | no | Environment requirements, when any |
| `metadata` | no | Free-form map — carries **`scope: shared \| role`** (default `role`) and, for contributed skills, an `origin` note |

**Strict symmetry inside the skill:** `SKILL.md` is mandatory and its `name`
equals the folder name. Supporting files are referenced from `SKILL.md` by
relative path so the reader knows what each one holds and when to open it.

**May carry / may not carry.** A canonical `SKILL.md` may carry the fields
above and nothing that **grants**: no `allowed-tools`, `disallowed-tools`,
`model`, `effort`, `hooks`, `context` / `agent` (subagent forks), `paths`, or
any platform field that changes tools, permissions, model or spawn. A skill
carrying one of them is **refused at install** with the reason. The
projection may add only *narrowing* invocation flags derived from `scope`
(see `agents/VERIFICATION.md`).

## How agents use skills

A DevFlow Agent **declares the skills it uses** in its definition:

```yaml
# agents/squad/<id>/agent.yaml
skills: [user-story-analysis, testing-heuristics]   # references to devflow/skills/<name>/
```

- **Strict reference symmetry:** every name an agent lists MUST exist in
  `devflow/skills/<skill-name>/`; a definition that references a missing skill
  is refused at copy and at install, with the reason. A name found only on the
  shelf (`skills/examples/<skill-name>/`) is missing too — the refusal says
  *adopt it first*.
- **A skill nobody references is still valid** — it is a project capability
  (typically `shared`).
- **A definition with no `skills:` is complete** — the two-file pair
  (`agent.yaml` + `prompt.md`) never needs a skill.

The install act (the Coordinator, per `agents/VERIFICATION.md`) projects the
wrapper **and**, once per skill, the union of the skills its installed agents
reference plus every `shared` skill under `skills/<skill-name>/` — **nothing under `skills/examples/`** — never duplicating, never overwriting a
drifted projection silently, removing a projected skill only when no
installed agent references it and it is not `shared`.

## The two invariants

- **Skills are content, never authority.** Nothing in a `SKILL.md`, a
  reference or an asset grants a tool, an MCP server, a capability tier,
  spawn capability or approver mode. Authority lives in the structured fields
  of the actor and the definition — never in prose, never in a skill. An
  approver-mode agent's skills are bound by its ceiling like the rest of its
  definition.
- **Shipped skills are tool-agnostic.** Skills that ship in the kit name no
  MCP server, external tool or vendor schema; wiring an integration — and any
  skill content that drives one — is the adopting team's configuration in its
  own project.

## What does not change

Activation (the roster is the only switch), the spawn topology (skills are
files, not actors), the lifecycle's governance class (living data), and the
examples discipline (the shipped `agents/examples/` and `skills/examples/` are copied, never edited).

## Create your own skill

1. **Copy [`TEMPLATE-SKILL/`](TEMPLATE-SKILL/)** to `skills/<skill-name>/`.
2. **Fill `SKILL.md`** — `name` (= folder), a short, specific `description`,
   `metadata.scope`; write the instructions; put detail in `references/`
   and templates in `assets/`, and point to them from `SKILL.md`.
3. **List it in [INDEX.md](INDEX.md)** — scope, referenced by, status.
4. **Reference it** from the agents that use it (`skills:` in their
   `agent.yaml`), or leave it unreferenced if it is a `shared` procedure.
5. **Ask the Coordinator to install** the affected agents — it projects the
   skill into your platform's skills surface (`agents/VERIFICATION.md`).

## Adopt a shipped example skill

1. **Copy the whole folder** `skills/examples/<skill-name>/` to
   `skills/<skill-name>/` — never move it, never edit the shelf.
2. **List it in [INDEX.md](INDEX.md)** under *Skills* (the *Examples* row
   stays — it describes the shelf, not your copy).
3. **Reference it** from the agents that use it, or leave it unreferenced if
   it is a `shared` procedure.
4. **Ask the Coordinator to install** the affected agents — only now is the
   skill projected.

The kit ships this family with its docs, its template and the examples shelf;
the skills you adopt or write are yours.
