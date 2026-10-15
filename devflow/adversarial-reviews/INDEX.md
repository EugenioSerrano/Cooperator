# Adversarial Reviews — Index

**Methodology version:** 5.2

Structured adversarial debates: LLM models participate in a
**Critique → Defense → Verdict** protocol to evaluate code against SPEC/ADRs,
best practices, or specific reference sources. Read-only in all phases — they
only document findings, arguments and a final verdict.

AREVs can be **Bolt-bound** (they choose a completed V-Bounce package as
their subject — never a stage of the V-Bounce, §2.15), **themed** (focused on
security, architecture, performance, etc.) or **ad-hoc** (exploratory on any
part of the code). They are **optional for all risk classes** (stakeholder-
triggered), but once initiated the three phases are mandatory and sequential,
each requiring its own approval: `AITL-AREV-CRITIQUE-Approval`,
`AITL-AREV-DEFENSE-Approval`, `AITL-AREV-VERDICT-Approval`. AREV state is
never written to a Bolt manifest.

---

## 🟡 Draft (created — phases pending approval)

| ID | Folder | Type | Focus | Current phase | SPEC / Area reviewed |
|----|--------|------|-------|---------------|---------------------|
| —  | —      | —    | —     | —             | —                   |

## 🔄 In progress (phases running — each stops at its approval)

| ID | Folder | Type | Focus | Current phase | Approved phases |
|----|--------|------|-------|---------------|-----------------|
| —  | —      | —    | —     | —             | —               |

## ✅ Active (all three phases approved — findings pending action)

| ID | Folder | Type | Focus | Verdict | SPEC / Area reviewed |
|----|--------|------|-------|---------|---------------------|
| —  | —      | —    | —     | —       | —                   |

## 🏁 Closed (all findings processed — fixed, routed or dismissed)

| ID | Folder | Type | Focus | Verdict | SPEC / Area reviewed |
|----|--------|------|-------|---------|---------------------|
| —  | —      | —    | —     | —       | —                   |

## ⛔ Cancelled (unrun — no neutral Verdict possible)

| ID | Folder | Type | Focus | Reason | SPEC / Area reviewed |
|----|--------|------|-------|--------|---------------------|
| —  | —      | —    | —     | —      | —                   |

---

**Last updated:** August 2026
