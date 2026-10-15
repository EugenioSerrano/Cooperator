# skills/ — Index

**Methodology version:** 5.2

The skills family: **project-level skills** — reusable, on-demand instructions
(`SKILL.md` + optional `references/` and `assets/`) that DevFlow Agents
reference from their definitions and that any actor may use when their scope
is `shared`. The contract lives in [README.md](README.md); new skills are
created from [TEMPLATE-SKILL/](TEMPLATE-SKILL/); the projection rules per
platform live in [../agents/VERIFICATION.md](../agents/VERIFICATION.md).

## The family docs

| File | Purpose |
|------|---------|
| [README.md](README.md) | What a skill is, the `SKILL.md` contract, scopes, how agents reference skills, the two invariants |
| [TEMPLATE-SKILL/](TEMPLATE-SKILL/) | Copy it to `skills/<skill-name>/` to author a skill (SKILL.md skeleton + `references/` + `assets/`) |
| [examples/](examples/) | The shelf — shipped example skills, read-only references: never projected, never referenced from `squad/`; copy the whole folder to `skills/<skill-name>/` to adopt |
| [INDEX.md](INDEX.md) | This list |

## Examples — the shelf (never projected until adopted)

| Skill | Scope | Referenced by | Status |
|-------|-------|---------------|--------|
| examples/user-story-analysis | shared | examples/test-designer | shipped — adapted from TestWriter (origin note in the skill); adopt by copying to `skills/user-story-analysis/` |
| examples/testing-heuristics | shared | examples/test-designer | shipped — adapted from TestWriter; 7 references; adopt by copying to `skills/testing-heuristics/` |
| examples/exploratory-scenario-generation | role | examples/test-designer | shipped — adapted from TestWriter; the generative pass after the heuristics; adopt by copying to `skills/exploratory-scenario-generation/` |
| examples/test-case-consolidation | role | examples/test-designer | shipped — adapted from TestWriter, re-targeted TC-first (one TC per scenario family, manifests, `AITL-TC-Approval`); `references/tc-mapping.md` + generic `assets/tc-export-projection.csv`; adopt by copying to `skills/test-case-consolidation/` |

> Shelf skills are references: present in every install, **never projected**
> by the Install act, **never referenceable** from `squad/`. *Referenced by*
> names the example definitions (`agents/examples/`) that list them.

## Skills

| Skill | Scope | Referenced by | Status |
|-------|-------|---------------|--------|
| —     | —     | —             | —      |

> Ships empty — the shelf above is not adopted. List every adopted or project
> skill here with its `metadata.scope` and the agents (by `id`) whose
> `agent.yaml` `skills:` references it; `—` when none (a `shared` procedure).

## Notes

- **One folder per skill; the folder name is the skill name.**
- **The shelf is not adopted.** Nothing under `examples/` is projected or
  referenceable; adoption = copy the whole folder to `skills/<skill-name>/`.
- **Skills are living data** — add, edit or remove without a Bolt; then have
  the Coordinator reinstall the affected agents.
- **Reference symmetry** — every skill an agent references must exist here;
  a skill nobody references is still a valid project capability.
- **Content, never authority** — no skill grants tools, MCP servers, tier,
  spawn or approval; a `SKILL.md` carrying a granting field is refused at
  install.
- **Shipped skills are tool-agnostic** — integrations are the adopting
  team's configuration.
