# actors/ — Index

**Methodology version:** 5.2

The roster family: who is in the team — humans and DevFlow Agents as
**actors**, one file per actor, **membership declared in
[roster.yaml](roster.yaml)** (the team list — an actor file not listed
there is not in the team). The concept lives in [README.md](README.md);
the validation schema is [roster.schema.yaml](roster.schema.yaml); new
actors are created from [TEMPLATE-ACTOR.yaml](TEMPLATE-ACTOR.yaml); the
worked examples (a human actor, a DevFlow Agent actor and a filled roster)
live in [examples/](examples/).

## The family docs

| File | Purpose |
|------|---------|
| [roster.yaml](roster.yaml) | The team list — the single membership authority (ships empty) |
| [TEMPLATE-ACTOR.yaml](TEMPLATE-ACTOR.yaml) | Create an actor file (the v1 shape) |
| [roster.schema.yaml](roster.schema.yaml) | Validates an actor file — fail fast, safe default until fixed |
| [examples/example-human.yaml](examples/example-human.yaml) | Worked example — a human actor (no `model`, no `definition`; humans approve by default) |
| [examples/example-agent.yaml](examples/example-agent.yaml) | Worked example — a QA DevFlow Agent actor (`model` + `definition` → `agents/squad/`) |
| [examples/example-roster.yaml](examples/example-roster.yaml) | Worked example — a filled team list (illustrative; the consistency rule applies to the real `roster.yaml`) |
| [README.md](README.md) | The concept, the family shape, the enablement and the resolution rules |

## Notes

- **The team lives in `roster.yaml`** — this INDEX lists the family docs,
  never the actors.
- **One file per actor** — `actors/<actor-id>.yaml` (naming N-rule);
  created from `TEMPLATE-ACTOR.yaml`, listed in `roster.yaml`.
- **The roster entry is the enablement** — a schema-valid actor file with
  `modes: [approver]` + a non-empty `approves`, listed in `roster.yaml`,
  is the explicit human configuration that lets a DevFlow Agent sign
  those checkpoint classes. Human-authored, never self-enabled; the
  `critical`/`regulatory` ceiling stays human-only (§3.0.1).
- **Definitions are reusable** — N actors : 1 `definition` (two
  architects may share one `agents/squad/` definition); each actor is
  distinct by `id`.
- **The `model` is per-instance** — the actor file's `model` is
  authoritative and may differ from the definition's default (this is
  what makes model-level independence possible at `high` risk).
- **`produces` is derived from `role`** (the single role → artifacts
  mapping, §3.0.1) — never a field.
