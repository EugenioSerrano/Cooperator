# Introduction (Plain-language Entry Point)

**Methodology version:** 5.2

## Purpose

This folder holds **plain-language explanations** of what each feature does. They are the
entry point for someone joining the project who does not yet know the system, the
business, or the methodology.

An introduction document answers exactly one question:

> **What is this about, told as a story, with no jargon?**

It reads in ten minutes, assumes no prior knowledge, and ends by pointing to **where to go
next** depending on what the reader needs to understand.

---

## What this folder is NOT

This is the most important section of this README.

> ⚠️ **Documents in this folder are NOT a source of truth.** They are narrative summaries
> derived from the artifacts in `analysis/` and `discovery/`. Where a narrative and an
> artifact disagree, **the artifact wins**.

An introduction document may **never**:

- introduce a business rule, decision or finding that is not already in a governed
  artifact;
- be cited as the basis of a SPEC, Bolt, ADR, User Story or test case;
- replace reading [`vision/`](../vision/), [`process/`](../process/) or
  [`domain-model/`](../domain-model/).

That is why the frontmatter carries `derivative: true`: it marks the document as living
**behind** the formal artifacts, not beside them.

### Governance

Introduction documents are **outside the AITL chain**. They have no approval checkpoint of
their own, and approving one would mean nothing: they add no content that could be
approved. By construction they are never governed input, which keeps them consistent with
the rule that only approved artifacts feed downstream work. **G28** makes this enforceable:
citing a document from this folder as the source of a SPEC, Bolt, ADR, User Story or Test
Case is a blocking violation. See Avenga DevFlow §5.5 (Derivative narrative documents).

The practical consequence: an introduction document may be written, corrected or discarded
at any time without a checkpoint — and it must never be the reason a decision was made.

---

## When to write it

**This is the last artifact of the analysis phase.** It is written *after* the artifacts it
summarizes, never before — and the sequencing is not a matter of taste:

| Prerequisite | Why the narrative needs it |
|--------------|----------------------------|
| [`vision/vision.md`](../vision/) | Provides section 5 ("what we are building") |
| [`scope/`](../scope/) — at least one milestone | Tells the reader what is in and out |
| [`domain-model/`](../domain-model/) — entities and enumerations | The "things" the story is about |
| [`ui/`](../ui/) — when the product has a surface | What the reader will actually see and how it behaves |
| [`process/`](../process/) — at least one process | The step-by-step spine of section 4 |
| [`glossary/`](../glossary/) | Guarantees the narrative uses the agreed words |
| [`open-questions/`](../open-questions/) | "What we still don't know" is part of the story |
| [`../../discovery/`](../../discovery/) — relevant DISC | Required when a legacy system is involved |

The gate is about **existence, not approval**. Draft artifacts are enough; waiting for
approval would push this document past the moment it is actually useful.

### Why the order matters

Written first, this document would be **inventing the domain** — precisely what
`derivative: true` forbids. Whatever it asserted would become the de-facto source of truth
simply by being the only readable document, and the formal artifacts would end up being
written to match the story instead of the other way around.

### Use the writing as a test

There is a second reason to write it last, and it is the more valuable one:

> **If you cannot tell the story simply from the artifacts, the problem is in the
> artifacts.**

Narrating forces every gap into the open — the rule nobody wrote down, the two documents
that contradict each other, the decision everyone assumed was made. When that happens,
**do not paper it over with a vague sentence.** Route it: open an `OQ-NNN`, or fix the
artifact, and then come back to the narrative. Used this way, writing the introduction
doubles as a coherence check on the whole analysis phase.

---

## Why it exists as a separate folder

| Folder | Answers |
|--------|---------|
| [`vision/`](../vision/) | Why we are doing this and where we are going |
| [`process/`](../process/) | How the flow works, in BPMN |
| [`glossary/`](../glossary/) | What each term means, one by one |
| [`domain-model/`](../domain-model/) | What "things" exist in the domain and how they relate |
| [`ui/`](../ui/) | What surfaces exist, what patterns and states govern them |
| **`introduction/`** | **What all of this is about, for someone who just arrived** |

None of the other artifacts covers that role: the vision talks about the future, the
processes are precise but dense, the glossary defines isolated words without the story that
connects them. What was missing is the narrative that puts everything in order **before**
the first formal read.

---

## Conventions

- **One document per feature or screen**, named descriptively
  (`mass-payment-cancellation.md`, `anulacion-masiva-de-pagos.md`), never named after an
  audience.
- **Start from [`TEMPLATE-INTRODUCTION.md`](TEMPLATE-INTRODUCTION.md).**
- **Required frontmatter:** `derivative: true`, and `sources` listing the artifacts the
  narrative was derived from.
- **Keep [`INDEX.md`](INDEX.md) up to date.**
- **Language policy (§3.15):** this README, the INDEX and the template
  are framework files and stay in **English**. Inside a document, the
  schema — frontmatter keys, status values — stays in English; the `##`
  section headings and the narrative prose go in the project's
  `content_language` (see [`../../LANGUAGE`](../../LANGUAGE)).

### Writing guidance

- **Tell, don't enumerate.** Short sentences. If a sentence only works for a reader who
  already knows the system, rewrite it.
- **Start in the real world**, not in the system. The first sentence should not mention a
  table, a screen or a process.
- **Be honest about what is uncomfortable.** A narrative that only lists benefits teaches
  the reader nothing and loses their trust on the first contradiction they find.
- **Order "where to read next" by question, not by folder.** A newcomer does not know what
  a "domain model" is, but does know what they want to understand.

### Maintenance

An introduction document **goes stale silently**: nothing breaks when it does, it simply
starts lying to the next person who arrives. Two rules keep that from happening:

1. When an artifact changes a rule the narrative mentions, update the narrative in the same
   pass and record it in its `History` table.
2. If narrative and artifact contradict each other and there is no time to reconcile them,
   mark the document `deprecated` rather than leave it circulating with false information.

---

## Status lifecycle

| Status | Meaning |
|--------|---------|
| `draft` | Written, not yet validated against the artifacts |
| `stable` | Reviewed: faithfully reflects the current artifacts |
| `deprecated` | Stale, or the feature changed — do not read |

---

**Last updated:** August 2026
