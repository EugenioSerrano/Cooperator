---
id: "RETRO-NNN"
week: "YYYY-Www"      # e.g. 2026-W21
date: "YYYY-MM-DD"    # document date — the week is in the filename (RETRO-NNN-YYYY-Www)
author: ""            # human:<user> — the accountable human, never an agent (§3.0)
agent: ""             # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""               # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"       # draft | final
facilitator: ""
participants: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — notes,
  action items, reflections — goes in the project's content_language
  (declared in devflow/LANGUAGE).
-->

# RETRO-NNN — Week YYYY-Www

## 1. DORA Five (mandatory opening)

| Metric | This week | Previous | Δ | Baseline band |
|--------|-----------|----------|---|---------------|
| D1 Deployment Frequency | | | | |
| D2 Change Lead Time | | | | |
| D3 Failed Deployment Recovery Time | | | | |
| D4 Change Fail Rate | | | | |
| D5 Deployment Rework Rate | | | | |

> DORA is computed at deployment level from CI/CD + incidents (§3.7.1).
> **Bolt Lead Time** (from `AITL-BOLT-READY-Approval` to
> `AITL-BOLT-DONE-Approval`) is a separate flow metric — never
> reported as DORA D2.

## 2. AITL governance (§3.7.3)

| Metric | Value | Target |
|--------|-------|--------|
| AITL Coverage (by Bolt type) | functional=__ non-functional=__ test=__ | 100% |
| Time-to-Human-Review (`review_ready_at` → `review.started_at`) | | < 4h |
| Approval-without-Comment Rate | | < 70% |
| Human Override Rate (by risk) | low=__ med=__ high=__ crit=__ | within band |
| AREV Adoption (sliced by scope/trigger/risk) | | monitor only |
| Defect escape rate (UAT / prod) | | ↓ trend |
| Gate Override Rate (waivers) | | monitor |
| Escalation Rate (L3→L2 / L2→L1) | | monitor |

## 3. AI-native flow (§3.7.2)

- Model runs / Bolt (average): __
- Bolts Done: __  | % Commitment delivered: __%
- V-Bounces / Bolt (average): __
- SPEC first-review approval rate: __% | V-Bounce first-review approval rate: __%
- Rework Ratio: __
- Manual Intervention Rate: __%
- Spec Drift (questions + material SPEC revisions): __

## 4. What worked / what didn't

- [ok] […]
- [warn] […]
- [fail] […]

## 5. Decisions & actions (improvement Bolts)

- [ ] US-000.BOLT-NNN — [action] (non-functional under `US-000`, `work_category: hardening`)
- [ ] US-NNN.BOLT-NNN — [action]

### §3.7.4 decision rules — apply as applicable

- [ ] Commit missed → reduce Bolt size or improve DoR
- [ ] Rework Ratio rises → DoR / SPEC quality is the bottleneck, not the agent
- [ ] Human Override Rate < 10% → risk of rubber-stamping; rotate reviewers / consider targeted REV or AREV
- [ ] D4 (Change Fail Rate) rises → tighten gates; consider targeted REV or AREV before raising throughput
- [ ] Spec Drift high → invest in `analysis/` (domain model, glossary)
- [ ] Bolt estimates drift ≥ 2× from actual active delivery → recalibrate with the AI-native estimation rule (§2.4); check for manual-effort anchoring; correlate `story_points` vs. actual aggregated Bolt Lead Time per US where used (§2.6)

## 6. Proposed ADRs

- [ADR-NNN] (if the decision is material — approved at `AITL-ADR-Approval`)
