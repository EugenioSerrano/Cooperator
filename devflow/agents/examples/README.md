# examples/ — shipped role definitions (read-only references)

**Methodology version:** 5.2

The six example role definitions — `functional-analyst/`, `architect/`,
`developer/`, `qa/`, `reviewer/`, `test-designer/` — each a **definition
folder** (`agent.yaml` + `prompt.md`; a definition may **reference** skills
from `devflow/skills/` in its optional `skills:` list — five of them reference
none; `test-designer/` references the four TestWriter skills on the
`devflow/skills/examples/` shelf: **adopt the skills first**, then copy the
agent) following the definition contract (see [`../README.md`](../README.md)).

**The rules:**

- **Copy, never edit in place.** These are references: instantiate your
  own agent by copying the closest example's **definition folder** (or
  [`../TEMPLATE-new-role/`](../TEMPLATE-new-role/)) into
  [`../squad/`](../squad/) and adapting it there. The copy **checks the
  `skills:` references** against `devflow/skills/<skill-name>/` — a missing
  skill refuses the copy (add the skill first; a skill that exists only on the
  shelf `devflow/skills/examples/` is adopted first: copy its whole folder to
  `devflow/skills/<skill-name>/`); skills are **never copied into the agent
  folder**.
- **Never referenced by the roster.** The roster's (`actors/`)
  `definition:` pointers reference `agents/squad/<id>/…` only — an example
  carries no identity in your team.
- **Never installed.** The Coordinator projects wrappers from `squad/`,
  not from here.

The six examples are references, never a closed enum — your squad defines
the roles your project needs.
