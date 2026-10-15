---
id: "US-000"
title: "Non-functional work container"
date: "2026-06-07"
author: "" # human:<user> — the accountable human, never an agent (§3.0)
agent: ""  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "active" # permanent container — always active
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs in English.
  Prose in the project's content_language (declared in devflow/LANGUAGE).

  ⚠️ US-000 IS NOT A FEATURE USER STORY (§2.6, §3.2): it is a permanent
  traceability container. It has NO Acceptance Criteria, approval status,
  approver, or AITL checkpoint. It is never submitted for approval and is
  not a substitute for approved ADRs or quality gates.
-->

# US-000 — Non-functional work container

> **This is a permanent container, not an actual User Story.** Despite its
> filename and location, it is the traceability parent for **every Bolt
> whose primary outcome is non-functional**: infrastructure, refactors,
> technical debt, hardening, security, performance, availability,
> observability, CI/CD, dependency upgrades, database maintenance,
> developer tooling (§3.2).

| Field       | Value |
|-------------|-------|
| **Layer**   | Full-stack |
| **Priority**| Continuous |

---

## Description

Every Bolt — regardless of whether it is a refactor, a hardening task, an
infrastructure change or a technical-debt payment — **must** belong to a
parent. US-000 is the canonical parent for technical work that does not
naturally fit into a business-feature User Story.

It is a **container rather than a User Story**: it has **no Acceptance
Criteria, approval status, approver, or AITL checkpoint** (§2.6). It does
not represent a user-facing capability and does not replace ADRs or quality
gates; it ensures that technical work is still governed through an
individually approved Bolt, SPEC, V-Bounce, MEM, manifest, and human
validation.

---

## Rules

1. **US-000 is always active** — it never closes.
2. **Every non-functional Bolt is assigned to US-000.** This includes
   infrastructure, refactoring, technical debt, hardening, security,
   performance, availability, observability, CI/CD, dependency upgrades,
   database maintenance, developer tooling, and similar technical changes
   (§3.2). A relationship to one or more feature USs may be recorded as a
   dependency or related reference, but it does not change the Bolt's
   parent.
3. **US-000 has no approval lifecycle** — never approved, rejected,
   version-approved, or re-approved by any role.
4. **US-000 Bolts still require their own `AITL-BOLT-READY-Approval`** by an
   Architect or Tech Lead and follow the same V-Bounce cycle
   (SPEC → V-Bounce → MEM → manifest).
5. **Non-functional BUGs** get their dedicated Bolt under US-000 — after
   `AITL-BUG-Approval` (recommended: Architect/Tech Lead when `severity:
   critical`, otherwise any team member — but guidance, never a gate: any
   qualified team member, the BUG's own author included, may record it at any
   severity). The dedicated Bolt's own `AITL-BOLT-READY-Approval`
   mirrors the same routing (§2.16).
6. **Classification follows primary outcome** (§2.4): business-visible
   behavior → feature US; technical outcome → US-000. No "quick fix",
   "chore", refactor, hardening, or infrastructure exception to Bolt
   traceability exists.

---

## Example Bolts under US-000

| Bolt | work_category | Description |
|------|---------------|-------------|
| `US-000.BOLT-001` | infra | Set up CI/CD pipeline with quality gates |
| `US-000.BOLT-002` | refactor | Extract shared validation library |
| `US-000.BOLT-003` | hardening | Add rate limiting to all endpoints |
| `US-000.BOLT-004` | debt | Upgrade framework to current LTS |
| `US-000.BOLT-005` | infra | Configure monitoring and alerting |
| `US-000.BOLT-006` | refactor | Fix ADR-003 compliance in auth module |

Each Bolt is a non-functional Bolt (`US-000.BOLT-NNN`) with its own
technical `AITL-BOLT-READY-Approval` by an Architect or Tech Lead.
