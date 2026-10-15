# UAT (User Acceptance Testing — per Unit / Milestone)

**Methodology version:** 5.2

> ⛔ **DORMANT / RESERVED (v4.2).** The UNIT/UAT approval-and-release layer was
> **removed from the active flow in v4.2** — `AITL-UAT-Approval` is **not an
> active checkpoint** in this release. The governed flow ends at Bolt
> acceptance (`AITL-BOLT-DONE-Approval`); release and customer acceptance
> follow the adopting team's own process. This folder and its template are kept
> **dormant** for a redesigned model planned in a future version. The
> text below describes the future (reserved) UAT process, not an active gate.

## Purpose

This folder records the **UAT minutes** for checkpoint
**`AITL-UAT-Approval`** (Avenga DevFlow §3.0). UAT validates the full scope
of a **Unit** or **Milestone** against the business acceptance criteria
derived from [`../../analysis/`](../../analysis/) (vision outcomes, process
rules, domain invariants), with explicit stakeholder sign-off.

It is the **human close** of the full cycle: not just "Bolt accepted" but
"a coherent slice of value delivered and accepted by the business".

**Sequence (§3.11, §4.6–§4.8):** UAT runs on a Unit that is technically
approved and deployed in staging — the staging `AITL-UNIT-Approval` is a
**precondition** of `AITL-UAT-Approval`. Once UAT passes, it is a
**precondition** of the production `AITL-UNIT-Approval` and release:
staging UNIT → UAT → production UNIT.

> `AITL-UNIT-Approval` is **reserved** (pending `units/` governance, §3.11
> entry 14): the sequence above is the intended rule and becomes blocking
> once the Unit recording artifact exists. In v4.2 this whole layer is
> **dormant/reserved** (see the banner above) — not an active checkpoint.

> UAT lives in `tests/` (not in `analysis/`) because it is a
> *verification artifact*. The *acceptance criteria themselves* are
> derived upstream in analysis (vision + business-context + domain-model
> + process) and on each User Story in [`../../functional/`](../../functional/).

---

## What goes here

- One `UAT-NNN-<description>.md` per UAT session (N21 naming).
- Results of every AC executed.
- List of agreed adjustments → new Bolts.
- Stakeholder sign-off (name + date) — `AITL-UAT-Approval`.

## What does NOT go here

- Automated tests → live with the code.
- Per-Bolt acceptance (`AITL-BOLT-DONE-Approval`) → recorded in the
  Bolt's manifest (`checkpoint_approvals[]`).
- Technical decisions → `adrs/`.

---

## How to draft with AI

Before the UAT session:

1. Ask the agent to read the Unit's Bolts (their MEMs and manifests) and
   the relevant [`../../analysis/process/`](../../analysis/process/) and
   [`../../analysis/vision/`](../../analysis/vision/) files.
2. Ask for a **draft AC list** mapped to vision outcomes and process rules.
3. The facilitator validates and locks the AC list before the session.

During the session, fill the template live. Afterwards, the UAT minutes in
`tests/uat/` are the evidence of `AITL-UAT-Approval` — the Bolt manifest
deliberately carries **no** UAT data (§3.12).

---

## Index

See **[INDEX.md](INDEX.md)** for the UAT listing.

---

## Lifecycle

| Status | Meaning |
|--------|---------|
| `draft` | AC list prepared, session not yet held |
| `approved` | All ACs passed, stakeholders signed off (`AITL-UAT-Approval`) |
| `approved-with-observations` | Passed with minor adjustments → new Bolts created |
| `rejected` | Critical ACs failed → rework required before re-test |

> **Status vs decision (§3.0):** `review.decision` uses the universal
> review-contract enum `approved | changes_requested | rejected` — UAT has
> no exemption. `approved-with-observations` is a **document status**, not
> a decision value: it is the lifecycle label for `review.decision:
> approved` with non-empty `findings[]`, where every finding routes to a
> new Bolt.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
