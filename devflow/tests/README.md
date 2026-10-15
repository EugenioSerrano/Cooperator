# Tests (human-facing verification artifacts)

**Methodology version:** 5.2

## Purpose

This folder holds **verification artifacts that require a human** —
UAT minutes and Test Cases (verification contracts).

> **Where do automated tests live?** Next to the code they verify, not
> here. The AI agent **writes** unit, integration, contract and e2e tests
> in the V-Bounce whose SPEC requires them, in the same loop that generates
> the code, and **runs** there the ones that need no running environment.
> Integration tests, and contract or E2E verification against the real
> providers, run in the Bolt's integration test cycle, after
> `AITL-MEM-Approval`, when its `test_integration` is `required` — a Test
> Bolt's automation runs in its TC's run cycle (see
> [Avenga DevFlow §2.5, §3.2.2, §3.3, §3.6](../avenga-devflow/Avenga-DevFlow.md)).
> Those tests are CI-gated and never leave the source tree.

`tests/` is the home of the **human side** of verification: what people
exercise, sign off and use as audit evidence.

---

## Structure

```
tests/
├── README.md
├── uat/           # dormant/reserved — the UAT approval layer was removed in v4.2
└── test-cases/    # Test Cases (TC-NNN) — independent verification contracts
```

| Subfolder | Purpose | Owner | Cadence |
|-----------|---------|-------|---------|
| [uat/](uat/)               | **Dormant/reserved** — the UAT approval layer was removed in v4.2; release/acceptance follows the team's own process | — | — |
| [test-cases/](test-cases/) | Test Cases: implementation-independent verification contracts (manual or QA-Automation-ready) | QA / Analyst | per Bolt / as needed |

---

## What does NOT belong here

- **Unit / integration / contract / e2e automated tests** → with the code.
- **Per-Bolt acceptance (`AITL-BOLT-DONE-Approval`)** → recorded in
  the Bolt's manifest (`checkpoint_approvals[]`).
- **Bug reports** → `bugs/`.
- **Incident post-mortems** → `incidents/`.
- **Business acceptance criteria definitions** → derived from `analysis/`
  (vision outcomes, process rules) and recorded on each User Story in
  `functional/`.

---

## Flow position

```
analysis/  →  functional/  →  spec/  →  code + automated tests (with code)
                                              │
                                              ▼
                                     tests/test-cases/    (verification contracts, AITL-TC-Approval)
                                              │
                                              ▼
                                     tests/uat/           (dormant/reserved)
```

---

## Traceability

- Each **Test Case** references exactly one approved `source_bolt` and
  carries `AITL-TC-Approval` (§2.6.1). A functional TC records `source_us`
  plus its `covered_acs`; a **non-functional TC still records
  `source_us: US-000`** — for traceability only, never as the source of the
  expected result — plus its governing ADRs, with `covered_acs` empty.
  **One case crosses User Stories:** a regression TC of an **evolution US**
  records `source_us` = the evolution US and `source_bolt` = the Bolt of the
  **delivered** US. §2.6.1 says the pair legitimately crosses; it must not be
  "corrected" to match.
- **UAT is dormant/reserved in v4.2:** the UAT approval layer was
  removed; release and customer acceptance follow the adopting team's own
  process until a redesigned model ships.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Test Case section headings and all prose — descriptions,
context, rationale, findings — go in the project's `content_language`;
UAT headings stay in English. Declared in [`../LANGUAGE`](../LANGUAGE)
(see §3.15).
