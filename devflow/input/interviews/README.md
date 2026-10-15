# Interviews (Stakeholder Conversations)

**Methodology version:** 5.2

## Purpose

This folder stores **raw transcriptions of conversations** with stakeholders,
users, domain experts, and anyone relevant to the business analysis.

> ⚠️ **Important:** This is raw input material, not processed documentation.
> There is **no formal template** for interview transcriptions — the structure
> suggested below is just a convenience guideline. Each interview is unique,
> and the format may vary depending on the session format, tools used, and
> the interviewer's style. The only hard requirement is that the content be
> readable and traceable back to the original conversation.

Interviews are one especially valuable input type (§2.1): the AI may help
transcribe them and extract goals, constraints, risks, success metrics, and
open questions, but interview-derived conclusions must remain **traceable to
the original recording or transcript**. The transcript itself is raw input:
a **human deposits it here** — agents never write into `input/` or its
subfolders, even when they produced the transcription (G31, §5.6). From them
the team proposes User
Stories, acceptance criteria (ACs), risks, ADR-defined constraints, domain
entities, and business processes — always validated by humans.

---

## What goes here?

- Full interview transcriptions (audio → text).
- Stakeholder meeting notes.
- Executive summaries of discovery sessions.
- Discovery workshop minutes.
- Any textual record of conversations with business people.

> **Note:** Original audio/video recordings (without transcription) go in the
> parent `input/` folder only as backup. Transcribed content always goes here —
> deposited by a human; agents never write here, even when the AI produced
> the transcription (G31).

---

## Best practices for interviews

- **Clear agenda** — Define objectives and questions before the session.
- **Recording consent** — Always ask permission before recording.
- **Clarification questions** — Don't assume; ask "what happens if...?".
- **Concrete examples** — Ask for real cases, not just abstract descriptions.
- **Success metrics** — Ask how the success of the process/feature is measured.
- **Privacy agreements** — Respect sensitive or confidential information.

---

## Naming conventions

Use the `INT-NNN` prefix for traceability across the methodology:

```
interviews/
├── INT-001-stakeholder-cto.md
├── INT-002-workshop-payments-module.md
└── INT-003-operator-user.md
```

Include the **participant/topic** in the filename description to make
searching easier.

---

## Suggested structure for a transcription

> 💡 **This is NOT a template.** It is a suggested structure for those who
> want a starting point. You can adapt, simplify, or ignore it entirely
> depending on the interview format. The only thing that matters is that the
> conversation content is captured faithfully.

```markdown
---
date: "YYYY-MM-DD"
participants: ["name1", "name2"]
duration: "45 min"
topic: "Brief description of the topic discussed"
tags: []
---

# Interview — [Topic]

## Context
[Why this interview was conducted, what we were trying to understand.]

## Transcription
[Textual content of the interview. Can be verbatim or summarized.]

## Key points extracted
- [Point 1]
- [Point 2]

## Entities mentioned
- [Entity 1] — [brief description]
- [Entity 2] — [brief description]

## Processes mentioned
- [Process 1] — [brief description]

## Open items / Follow-ups
- [ ] [Question or topic left open]
```

---

## Relationship to the flow

```
Interviews → analysis/domain-model/ (entities) + analysis/process/ (BPMN)
           → functional/ (User Stories + Bolts)
```

Transcriptions directly feed into:
- `analysis/domain-model/` — Extraction of entities, properties, and relationships.
- `analysis/process/` — Identification of business processes (BPMN).
- `analysis/personas/` and `analysis/user-journeys/` — Who uses the product and how.
- `analysis/business-context/` + `analysis/business-risks/` — Business risks (BR-NNN).
- `risks/` — Project/technical risks (RISK-NNN) that surface from the conversation.
- `functional/` — Derivation of User Stories and acceptance criteria.

### Mandatory inception outputs (§3.4)

After processing interviews, the AI agent + human validation MUST produce:

| Output | Destination | Description |
|--------|-------------|-------------|
| Prioritized backlog (US + ACs) | `functional/user-stories/` | User Stories with Given/When/Then ACs; feature USs stop at `AITL-US-Approval` |
| Units → Bolts map | `functional/INDEX.md` | Grouping of US into Units, initial Bolt breakdown |
| Initial Risk Register | `analysis/business-risks/` (BR-NNN) + `risks/` (RISK-NNN) | Business risks vs project/technical risks, routed per §5.7/§5.12 boundary |
| Demo criteria (first 1–2 weeks) | Bolt completion evidence | What to demo at `AITL-BOLT-DONE-Approval` for each initial Bolt |

These outputs are validated by the human before entering the V-Bounce cycle.

---

## Lifecycle of an interview

A transcribed interview is a **historical document**. It does not change — what
was said, was said. If a follow-up interview is conducted, a new document is
created.

This folder has no active/deprecated states. The INDEX.md lists all completed
interviews.

---

## Document index

See **[INDEX.md](INDEX.md)** for the complete listing of conducted interviews.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). Transcripts are kept in the language they were recorded in
(§3.15). Other prose goes in the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
