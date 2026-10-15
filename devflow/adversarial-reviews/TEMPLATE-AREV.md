---
id: "AREV-NNN"
title: ""
date: "YYYY-MM-DD"
author: ""       # human:<user> — the accountable human, never an agent (§3.0)
agent: ""        # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""          # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
type: "" # bolt | themed | ad-hoc
focus: "" # general | security | architecture | functionality | performance | other
implementor_model: "" # Model that generated the code (e.g. "Claude Opus"), or N/A if ad-hoc
spec_reviewed: "" # SPEC-YYMMDD-HHmm being reviewed, or N/A if ad-hoc
bolt: "" # Associated Bolt, or N/A if ad-hoc
governing_adrs: [] # ADRs against which the review is conducted
status: "draft" # draft | in-progress | active | closed | cancelled
requested_by: "" # Who requested the AREV and brief reason
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs, and section
  headings (##) stay in English (the schema). All prose — descriptions,
  motivation, findings — goes in the project's content_language (declared
  in devflow/LANGUAGE).

  ⚠️ SEQUENTIAL PHASE APPROVALS (§2.15, §3.0): each phase remains DRAFT
  until its named human checkpoint is approved. The next phase cannot
  begin until the current one is approved. AREV approvals and verdicts
  are recorded ONLY in these AREV artifacts — never in the Bolt manifest.
  Agent/model selection between phases is a MANUAL human action (§3.13).
-->

# AREV-NNN — [Descriptive title]

> **Note:** This file is an index for the AREV. The debate documents are in
> `01-CRITIQUE.md`, `02-DEFENSE.md` and `03-VERDICT.md` within this folder.

| Field | Value |
|-------|-------|
| **Type** | [bolt / themed / ad-hoc] |
| **Focus** | [general / security / architecture / functionality / performance / other] |
| **Bolt** | [Associated Bolt, or N/A] |
| **SPEC reviewed** | [SPEC-YYMMDD-HHmm — title, or N/A] |
| **Implementor model** | [LLM model that generated the code, or N/A] |
| **Status** | [draft / in-progress / active / closed / cancelled] |
| **Requested by** | [Who requested the AREV and brief reason] |
| **Scope** | [Files, modules or code areas under review] |
| **Reference sources** | [Context7, OWASP, official docs, etc. — or "None"] |

## 1. Motivation

[Why is this AREV requested? Brief context. Examples:
- "Review the security of the authentication module before release."
- "Verify that the implementation complies with architecture ADRs."
- "General review of Bolt-015 due to high risk class."
- "User requested functionality review using Context7 as guide."]

## 2. Phases (all three mandatory, sequential — each stops at its approval)

| Phase | Document | Model (manually selected) | Status | Approval |
|-------|----------|---------------------------|--------|----------|
| ① Critique | `01-CRITIQUE.md` | [Challenger model] | ⬜ pending | `AITL-AREV-CRITIQUE-Approval` |
| ② Defense | `02-DEFENSE.md` | [Defender model] | ⬜ pending | `AITL-AREV-DEFENSE-Approval` |
| ③ Verdict | `03-VERDICT.md` | [Judge model] | ⬜ pending | `AITL-AREV-VERDICT-Approval` |

Phase status: `pending` → `in-review` → `approved` / `changes_requested`.
Every initiated AREV runs all three phases (§2.15) — the next phase cannot
begin until the current one is approved.

## 3. Final verdict

[Completed at the end of Phase 3 — only an approved Verdict produces
actionable findings.]

## 4. AITL phase approvals

> Recorded in each phase document's `review` block (§3.0). The AREV keeps
> its own approval evidence; nothing is written to the Bolt manifest.
