---
id: "INC-NNN"
date: "YYYY-MM-DDTHH:mm:ss±HH:MM"  # point-in-time event: incident detection
severity: "sev3"    # sev1 | sev2 | sev3 | sev4
service: ""
author: ""          # human:<user> — the accountable human, never an agent (§3.0)
agent: ""           # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""             # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "open"      # open | mitigated | closed
detected_at: ""     # ISO-8601 — detection (feeds D3 recovery time)
resolved_at: ""     # ISO-8601 — resolution (feeds D3 recovery time)
deployment_caused: false # true if this incident was caused by a deployment (feeds D4)
deployment_ref: ""  # deployment ID / tag that introduced the failure (if deployment_caused)
unplanned_rework: false # true if the fix was unplanned production rework (feeds D5)
caused_by_bolt: null # US-NNN.BOLT-NNN if applicable — referential link, no manifest update
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — timeline,
  root cause, resolution — goes in the project's content_language
  (declared in devflow/LANGUAGE).

  ⚠️ Manifest v5 (§3.12): the Bolt manifest carries NO DORA or incident
  data. The INC links to the originating Bolt and deployment referentially;
  DORA metrics are computed at deployment level from CI/CD + incidents
  (§3.7.1). No single-model attribution is forced (§5.12).
-->

# INC-NNN — [title]

## 1. Summary

[1–2 sentences: what failed, how it was detected, impact.]

## 2. Timeline

| Time (UTC) | Event |
|------------|-------|
| HH:mm | Detection |
| HH:mm | Triage |
| HH:mm | Mitigation |
| HH:mm | Resolution |

## 3. Impact

- Users affected: [#]
- Duration: [N min]
- SLO impacted: [yes/no, which]

## 4. Root cause

[Technical cause + process cause. Blameless.]

## 5. Why did the gates not catch it?

[Specific gap in gates / AREV / DoR / DoD that allowed the failure through.]

## 6. Actions (derived Bolts)

- [ ] US-000.BOLT-NNN — [hardening (non-functional under US-000) / new gate / prompt fix / etc.]
- [ ] If a defect is confirmed → BUG → `AITL-BUG-Approval` → dedicated Bolt (§4.10)

## 7. Origin Bolt link

If `caused_by_bolt` is not null, reference it here (US-NNN.BOLT-NNN). The
link is **referential** — the Bolt manifest carries no incident/DORA
fields (§3.12). Deployment-caused incident data feeds D3/D4/D5 from
CI/CD + `incidents/` (§3.7.1, §5.12).
