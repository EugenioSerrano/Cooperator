---
id: "OQ-NNN"
title: ""                 # short, imperative — "Can a customer have multiple active subscriptions?"
date: "YYYY-MM-DD"        # date opened
author: ""                # human:<user> — the accountable human who raised the question, never an agent (§3.0)
agent: ""                 # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                   # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "open"            # open | in-validation | answered | deferred | dropped
priority: "P1"            # P0 (blocks analysis closure) | P1 (blocks a specific artifact) | P2 (nice-to-have)
owner: ""                 # person responsible for chasing the answer
validator: ""             # stakeholder who must validate the answer (filled when known)
targets:                  # canonical artifacts this OQ will update once answered
  - "../domain-model/entities/Customer.md"
sources:                  # where the gap surfaced
  - "../../input/interviews/INT-007.md"
related: []               # other OQ-NNN / RISK-NNN / ADR-NNN / DISC-NNN
tags: []
revisit_on: ""            # only if status=deferred — date or trigger
closed_on: ""             # YYYY-MM-DD (when status moves to answered/dropped)
closed_by: ""             # who closed it
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — question text,
  context, hypothesis, resolution — goes in the project's content_language.
  See devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.

  ⚠️ WRITE IT FOR THE PERSON WHO ANSWERS IT (§5.7): a stakeholder who has
  never seen the system, or not for years, must be able to answer from this
  document alone. Self-contained context, three to four neutral options, the
  recommendation kept apart.
-->

# OQ-NNN — [Title]

> **Summary for the validator** — read this first; the detail follows.
>
> - **Question:** [the question in one sentence]
> - **Options:** A — […] · B — […] · C — […] (· D — […])
> - **Recommended:** [option] — because [one line]. The decision is yours.

## 1. Question

> State the question in **one sentence**. Atomic. If you wrote "and" or "or",
> split it into two OQs.

## 2. Context

Write for someone who has never seen this system, or not for a long time,
in the language of the person who answers it — business terms for a business
question, technical terms for a technical one (non-functional, architecture,
code) — and explain every ID, acronym or internal term you keep. A technical
question is explained just as thoroughly.

### 2.1 What this is about

- **Existing system** (maintenance or migration): what this part of the
  system is for, and how it behaves **today** — the evidence (screen,
  report, interview, document) described for its reader. For a technical
  question: the component, service or file, the relevant code or
  configuration, with path and version.
- **Something new:** what the capability is and what it is for, in detail.

### 2.2 The question in detail

- **Exactly what is asked**, and the situation that raised it.
- **What the answer must weigh:** facts, business rules, constraints, the
  people or areas affected.
- **What is known, and what is not.**
- **What depends on the answer:** the artifacts blocked (link `targets`) and
  the impact of getting it wrong.

### 2.3 An example

A concrete case with data — the clearest way to make the question real.

## 3. Options

Three to four options, presented neutrally. Fewer only when the question is
genuinely binary — say why. Never add a filler option.

| # | Option | What it means | What changes (in `targets`) | Risk / cost |
|---|--------|---------------|-----------------------------|-------------|
| A |        |               |                             |             |
| B |        |               |                             |             |
| C |        |               |                             |             |
| D |        |               |                             |             |

## 4. Recommendation

The agent's proposal — the validator decides.

- **Recommended option:** …
- **Why:** …
- **Confidence:** high / medium / low
- **What would change it:** …

## 5. Working hypothesis

The recommended option, used to keep work flowing until the answer arrives.

> Example: *"Assuming option A (a customer has only one active subscription);
> if wrong, `Customer ↔ Subscription` becomes 1..N and billing logic changes."*

## 6. Investigation log (append-only)

| Date       | Author | Note |
|------------|--------|------|
| YYYY-MM-DD | @user  | Opened from interview INT-007, minute 14:32 |
|            |        |      |

## 7. Resolution

Filled in only when status becomes `answered` / `deferred` / `dropped`.

- **Decision:** …
- **Answer vs recommendation:** followed / a different option (which) /
  another answer — never inferred: as the validator gave it.
- **Validated by:** @stakeholder on YYYY-MM-DD (source: link)
- **Propagated to:** link to the exact section / commit in each `targets`
  artifact that absorbed the answer.
- **If deferred:** revisit trigger / date and why it does not block now.
- **If dropped:** reason (scope cut, duplicate of OQ-MMM, promoted to
  RISK-NNN / ADR-NNN, …).

## 8. History

| Date       | Change                                         | Author |
|------------|------------------------------------------------|--------|
| YYYY-MM-DD | Opened                                         | @user  |
| YYYY-MM-DD | Status → in-validation (answer drafted)        | @user  |
| YYYY-MM-DD | Status → answered, propagated to `Customer.md` | @user  |
