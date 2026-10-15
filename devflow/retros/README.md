# Retros

**Methodology version:** 5.2

## Purpose

This folder contains **weekly retrospectives**. Every retro opens with the
**DORA Five + AITL Coverage** metrics (Avenga DevFlow §3.7.4): without that
reading, there is no retro.

---

## What belongs here?

- One `RETRO-NNN` per week with DORA metrics, AI-native flow indicators
  and improvement actions.
- Trend analysis: current week vs. previous week with deltas.
- Actionable items that become improvement Bolts.
- Proposed ADRs when decisions are material.

## What does NOT belong here?

- Production incident post-mortems → `incidents/` (`INC-NNN`).
- Individual bug records → `bugs/` (`BUG-NNN`).
- Architectural decisions → `adrs/` (`ADR-NNN`, linked from here).

---

## Naming convention

```
RETRO-NNN-YYYY-Www.md
```

- `RETRO` — Fixed prefix.
- `NNN` — 3-digit sequential number.
- `YYYY-Www` — ISO week (e.g. `2026-W21`).

---

## Rules

1. **Weekly cadence.** One `RETRO-NNN` per week — no exceptions.
2. **Metrics-first opening.** The retro MUST open with the DORA Five
   dashboard (§3.7.1) + AITL governance metrics (§3.7.3) + AI-native
   flow metrics (§3.7.2). Without data, there is no retro.
3. **Delta tracking.** Every metric includes current value, previous value
   and delta. If any DORA metric worsens vs. the previous week, an
   actionable item is mandatory.
4. **Improvement Bolts.** Actions are tracked as Bolts — process
   improvements, gate additions, prompt refinements, DoR/DoD updates —
   using the normal Bolt lifecycle (non-functional under `US-000` with
   `work_category: hardening`, approved at `AITL-BOLT-READY-Approval`).
5. **Material decisions → ADR.** If the retro produces a significant
   architectural or process decision, it becomes an ADR (approved at
   `AITL-ADR-Approval`).
6. **Diagnostic slicing, not attribution.** DORA is computed at deployment
   level (§3.7.1). When reviewing DORA, deployments may be joined to their
   included Bolts and model runs **only for diagnostic slicing** — a
   deployment with several Bolts/models is classified `model_mix`, never
   attributed to one model without an unambiguous causal link.

---

## Recommended structure (per retro)

1. **DORA Five** — Mandatory opening dashboard with week-over-week deltas
   and the team's own baseline bands.
2. **AITL governance** — Coverage, time-to-review, override rates (§3.7.3).
3. **AI-native flow** — Throughput, V-Bounces, first-review approval rates
   (SPEC + V-Bounce), Rework Ratio, spec drift (§3.7.2).
4. **What worked / what didn't** — Qualitative observations.
5. **Decisions & actions** — Improvement Bolts with owners.
6. **Proposed ADRs** — If any decision is material.

### Diagrams and visual elements

Use **Mermaid** for all diagrams, charts and any other visual element
(no ASCII art or embedded images).

---

## Retro status

| Status | Meaning |
|--------|---------|
| **draft** | Notes taken during or right after the session, still being completed. |
| **final** | Session closed: the retro is a dated, immutable record of what the team observed and decided. |

> **Why `draft | final` and not the common `draft | stable | deprecated`:** a
> retro is a **minute of a dated event**, not a living document. It is never
> revised into a newer truth and never becomes deprecated — a later retro
> does not supersede an earlier one, both remain valid records of their own
> week. Its improvement actions live on as Bolts, not as edits to the retro.
> Same precedent as `process/`'s `active` and `spec/`'s `blocked | obsolete`:
> an artifact-specific document status, with the `review.decision` enum
> unchanged (§3.0, W11).

## DORA Five reference (§3.7.1)

| Metric | What it measures | Source of truth |
|--------|------------------|-----------------|
| D1 Deployment Frequency | Production deployments per period | CI/CD deployment events |
| D2 Change Lead Time | Commit → production deploy | VCS + CI/CD timestamps |
| D3 Failed Deployment Recovery Time | Time to recover from a deployment failure | Deployment event + incident/recovery event |
| D4 Change Fail Rate | % of deployments causing a failure | CI/CD joined to deployment-caused incidents |
| D5 Deployment Rework Rate | % of deployments that are unplanned production rework | CI/CD classified planned vs rework |

Teams define their own internal baselines and improvement objectives from
their project context and review them in each retro. DevFlow does not label
fixed thresholds as universal DORA "Elite" targets and does not turn a DORA
metric into an individual performance target (§3.7.1). **Bolt Lead Time**
(from `AITL-BOLT-READY-Approval` to `AITL-BOLT-DONE-Approval`) is a separate
flow metric — never reported as DORA D2.

---

## Document index

See **[INDEX.md](INDEX.md)** for the full listing.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in [`../LANGUAGE`](../LANGUAGE)
(see §3.15).
