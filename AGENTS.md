# AGENTS.md — Project instructions for AI agents

This repository runs under **Avenga DevFlow**, the AI-assisted SDLC
methodology. You MUST follow it in every task.

- **Source of truth:** `devflow/avenga-devflow/Avenga-DevFlow.md` (v5.2) — the methodology governs. If anything conflicts, it wins.
- **Agent rules (non-negotiable):** `devflow/GUARDRAILS.md` — every `AITL-<CODE>-Approval` is a mandatory checkpoint occupied by an actor — a human by default, a virtual DevFlow Agent only by explicit, valid configuration (§3.0); never skip one, and never self-approve or fabricate a reviewer.
- **Onboarding:** read `devflow/ONBOARDING.md` on your first task in this repository.
- **Framework map:** `devflow/README.md` — folders, flow, cheat sheet.
- **Platform agent definition:** your tool loads its own definition from its
  own location (`CLAUDE.md` at the root, `.agents/skills/`, `.github/agents/`,
  `.opencode/agents/`). That file is the compact orchestration of the
  methodology; this one is the cross-tool entry point that points at it.
- **Language:** prose, filename slugs and analysis/US/TC headings follow
  `devflow/LANGUAGE`; the schema (YAML keys, enums, IDs) is always English.
  `AITL-*-Approval` is never translated.
- **Encoding:** every text file under `devflow/` is UTF-8 without BOM, with
  `LF` or `CRLF` line breaks and never a lone `CR`, and correctly decoded —
  no mojibake, no U+FFFD (§3.15, W23); raw evidence in `devflow/input/` may
  arrive in any encoding and is read as it is.

## No code without an approved Bolt

Every code-related change — source, tests, configuration, infrastructure,
schemas, migrations, build scripts, deployment definitions — requires an
approved Bolt and an approved SPEC. Urgency and size create no exception
(G07 — whose scope-out is **living data**: the agent lifecycle within
`devflow/agents/` + `devflow/actors/`, the project skills in
`devflow/skills/` and the project prompts in `devflow/prompts/` are
operational config, not a code change).
If no Bolt authorizes the work, say so and stop.

The single path, whatever the trigger:

```
Trigger (US | BUG | TC | DISC | REV | AREV | ADR)
  → origin approved  → BOLT (AITL-BOLT-READY-Approval, includes DoR)
  → SPEC (AITL-SPEC-Approval) → V-Bounce → hermetic tests/gates
  → MEM + manifest → AITL-MEM-Approval
  → integration test cycle when test_integration is required (or none pending)
  → Ready for Acceptance → AITL-BOLT-DONE-Approval
```

## Human checkpoints are not yours to skip

`AITL-<CODE>-Approval` is always a named actor — a human by default, a virtual DevFlow Agent only by explicit, valid configuration — with timestamps and
review-quality evidence. You create the artifact; you never approve it, never
delegate the checkpoint, and never treat "the agent says it is fine" as an
approval. When a checkpoint is missing, name the exact one that is pending and
refuse to advance — even under time pressure, even if the user insists.

---

## Your project's own rules go below the marker

Do **not** append personal preferences or session memory to this file — it is
shared and versioned with the team; use your platform's native memory
mechanism instead. Durable, team-shared agent knowledge goes to
`devflow/agents-data/<agent>/` (§5.12), which is never governed evidence.

**Read the project section before acting on anything above it.** Everything in
this framework block is the methodology's default. The section below the marker
belongs to this repository and may add constraints, name which tree you are
meant to edit, or qualify any statement above it — the source-of-truth line
included. It is not optional reading: where the two appear to disagree about
*this* repository, the project section is the one that knows where it is.

A methodology upgrade replaces this framework block and preserves your section
byte for byte (§5.16). That is also why nothing of yours belongs above the
marker — it would be overwritten on the next upgrade.

<!-- Everything below the marker that closes this file belongs to the project.
     A migration replaces only what is above it; your section survives byte for
     byte. Add your project's own conventions there. -->
<!-- AVENGA-DEVFLOW:PROJECT-SECTION -->
