# Test Cases (independent verification contracts)

**Methodology version:** 5.2

## Purpose

This folder holds **Test Cases (`TC-NNN`)** — implementation-independent
verification contracts derived from **approved intent**, never from the
current code (§2.6.1). Each TC defines preconditions, input data, steps or
stimulus, expected results, covered ACs or technical constraints, and the
evidence needed to determine pass or fail.

> Automated tests (unit, integration, contract, e2e) live next to the
> code and are generated + executed by the AI agent in every V-Bounce.
> See [`../README.md`](../README.md) for the boundary.

---

## What goes here

- **Verification contracts** for functional work: derived from the approved
  feature US/ACs and the exact approved functional Bolt.
- **Verification contracts** for non-functional work: derived from the
  exact approved non-functional Bolt plus its governing ADRs.
- **QA-Automation-ready TCs** — a TC may originate 1..n Test
  Bolts (`TC-NNN.BOLT-NNN`) when automation has independently deliverable
  outcomes (§2.6.1).

## What does NOT go here

- Automated tests → with the code.
- UAT sign-off minutes → [`../uat/`](../uat/).
- Per-Bolt acceptance (`AITL-BOLT-DONE-Approval`) → recorded in the
  Bolt's manifest (`checkpoint_approvals[]`).

---

## Test-basis rule (MANDATORY, §2.6.1)

The expected behavior of a Test Case must be derived from **approved
intent**, never from the current implementation:

- A functional TC is based on the approved feature US/ACs and the exact
  approved functional Bolt whose outcome it verifies.
- A non-functional TC is based on the exact approved non-functional Bolt and
  its approved ADRs or other governed technical sources.
- A BUG-related TC also references the approved BUG and its expected
  behavior.

**One case where `source_us` and `source_bolt` cross User Stories.** A TC
that gives regression coverage to an `existing` criterion of an **evolution
US** records `source_us` = the **evolution** US and `source_bolt` = the Bolt
of the **delivered** US that originally built the behaviour. §2.6.1 states
that this pair *"legitimately crosses User Stories"*, and the manifest schema
allows it: the two are independent patterns with no cross-constraint. Do not
"correct" such a pair to match — the crossing **is** the traceability. Every
other TC, including one covering a `delta` criterion of the same evolution
US, has the ordinary matching pair. Non-functional TCs are unaffected
(`source_us: US-000`, empty `covered_acs`).

Existing code, tests, configuration, schemas and runtime behavior may be
inspected **only** to understand interfaces, setup, data, feasibility and
regression surface — they are **contextual evidence, not the test oracle**
(G06). When implementation behavior conflicts with the approved test basis,
the conflict is reported and routed through the BUG/analysis/ADR lifecycle —
never silently normalized to the code.

---

## Lifecycle

| Status | Meaning |
|--------|---------|
| `draft` | Drafted, pending `AITL-TC-Approval` — cannot govern a SPEC; its Test Bolts may be drafted but not approved. |
| `approved` | `AITL-TC-Approval` recorded (QA + applicable domain/technical owner). |
| `deprecated` | Replaced by an automated test or no longer relevant. |

> **Cardinality (§2.6.1):** every TC references exactly **one approved
> `source_bolt`**; a functional TC records one `source_us` and one or more
> `covered_acs`; a non-functional TC records `source_us: US-000` plus every
> governing ADR or other technical source.

---

## How to add a test case

1. Copy [TEMPLATE-TC.md](TEMPLATE-TC.md) to `TC-NNN-short-title.md`.
2. Fill in preconditions, steps, expected results, source Bolt and
   traceability — derived from approved intent, never from current code.
3. Create the manifest — **mandatory** (the template's `Manifest creation`
   section, G33): copy
   [`TEMPLATE-MANIFEST-TC.json`](../../metrics/TEMPLATE-MANIFEST-TC.json) to
   `devflow/metrics/test-cases/TC-NNN-short-title.json`. It must validate
   against
   [`manifest-v5-tc.schema.json`](../../metrics/manifest-v5-tc.schema.json):
   **a TC without a valid manifest does not exist.**
4. Stop at `AITL-TC-Approval` (QA + domain owner) before the TC governs a
   SPEC and before its Test Bolts are approved (they may be drafted alongside
   it), and record that decision in the manifest.
5. Add the entry to [INDEX.md](INDEX.md).
6. Cross-link from the User Story / Bolt / process it verifies.

---

## File naming

`TC-NNN-short-title.md` — e.g. `TC-001-checkout-with-coupon.md`.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15). `AITL-*` codes are never
translated.
