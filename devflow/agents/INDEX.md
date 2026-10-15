# agents/ — Index

**Methodology version:** 5.2

The DevFlow Agent definitions family. The canonical contract and the
create-your-own-agent guide live in [README.md](README.md); the
per-platform wrappers — and the skills the definitions reference, from [`../skills/`](../skills/INDEX.md) — are projected by the **Coordinator** (the
AvengaDevFlow agent itself, one per tool) from the **live definitions in
[`squad/`](squad/)** into the project's platform folders
(`.claude/agents/`, `.opencode/agents/`, `.github/agents/`,
`.codex/agents/`) following the mapping in [VERIFICATION.md](VERIFICATION.md)
(N×4 parity verified) — the kit ships no pre-built role wrappers.

## Examples (shipped — read-only references)

| Example | Produces | Skills | Status |
|---------|----------|--------|--------|
| examples/functional-analyst/ | feature US | — | shipped |
| examples/architect/ | ADR | — | shipped |
| examples/developer/ | SPEC + code + tests | — | shipped |
| examples/qa/ | TC/tests + MEM reviews | — | shipped |
| examples/reviewer/ | REV + MEM/DISC approvals | — | shipped |
| examples/test-designer/ | draft `TC-NNN` + manifests (TC-first, one per scenario family) for `AITL-TC-Approval` · `OQ-NNN` for story gaps · optional CSV of approved TCs | user-story-analysis · testing-heuristics · exploratory-scenario-generation · test-case-consolidation (on the `skills/examples/` shelf — adopt first) | shipped — adapted from TestWriter; executor-only |
| TEMPLATE-new-role/ | **generic template** — copy it (or an example) into `squad/` to create any project-defined role | — | template |

> Examples are **copied, never edited in place and never
> roster-referenced** — see [examples/README.md](examples/README.md).

## Squad (live — your project's agents)

| Agent | Role | Produces | Skills | Status |
|-------|------|----------|--------|--------|
| squad/fa-agent/ | functional-analyst | US funcional | — | instalado |
| squad/fa-reviewer-agent/ | functional-analyst-reviewer | REV de US/ACs y análisis | — | instalado |
| squad/dotnet-architect-agent/ | architect | ADR (.NET) | — | instalado |
| squad/dotnet-architect-reviewer-agent/ | architect-reviewer | REV de ADRs/NFRs (.NET) | — | instalado |
| squad/dotnet-developer-agent/ | developer | SPEC + código .NET + tests | — | instalado |
| squad/dotnet-developer-reviewer-agent/ | developer-reviewer | REV del SPEC/V-Bounce .NET (código, tests, MEM) | — | instalado |
| squad/qa-agent/ | qa | TC/tests + revisiones de MEM | — | instalado |
| squad/qa-reviewer-agent/ | qa-reviewer | REV de TC/verificación y cobertura | — | instalado |
| squad/test-designer-agent/ | test-designer | borradores `TC-NNN` + manifiestos para `AITL-TC-Approval` · `OQ-NNN` de huecos de historia | user-story-analysis · testing-heuristics · exploratory-scenario-generation · test-case-consolidation | instalado |
| squad/test-designer-reviewer-agent/ | test-designer-reviewer | REV del diseño de pruebas (escenarios, familias, cobertura) | — | instalado |

> The Coordinator creates agents here and installs their wrappers — see
> [squad/README.md](squad/README.md). List each agent in this table — the
> **Skills** column names the skills it references
> ([`../skills/<skill-name>/`](../skills/INDEX.md), `—` when none) — and add
> its actor entry to the roster (`actors/`).

## The Coordinator

**No separate folder** — the Coordinator is the Avenga DevFlow agent
itself, shipped per platform (`CLAUDE.md`, `.agents/skills/…`,
`.github/agents/…`, `.opencode/agents/…`) and **evolved** to act as the
orchestrator: the shared body carries the "The Coordinator (the
orchestrator)" paragraph (routes, delegates production, spawns approvers,
records — **never signs**, `approves: []`), and each platform preamble
declares the spawn-topology mechanics. The wrappers are projections of
role agents only — no coordinator sub-agent exists.

## Notes

- **Examples are skeletons** — adopters copy them into `squad/` and
  instantiate; they never edit the kit's examples in place.
- **Project-defined agents** live in `squad/` and are listed in the Squad
  table above (the adopter's `agents/` family grows with their team).
- The **Coordinator is not generated**: its projections are the four
  platform agent files; only role agents get wrappers.
- Per-platform capability status: [VERIFICATION.md](VERIFICATION.md).
