# User Journeys

**Methodology version:** 5.2

## Purpose

One file per **user journey**: the end-to-end experience of a persona
trying to accomplish a goal, across **all channels and touchpoints**
(web, mobile, call center, email, in-store, paper), with their
**actions, thoughts, emotions and pain points** at each stage.

User journeys answer **what the user lives through**. They are the
user-centric counterpart to BPMN processes.

---

## Journey vs. Process (do not confuse)

| Artifact | Lives in | Lens | Example |
|----------|----------|------|---------|
| **Process (BPMN)** | `../process/` | Internal / operational: how the business executes work | "Invoice issuance" - back-office activities, decisions, system tasks |
| **User Journey**   | `user-journeys/` (here) | External / experiential: what the user does and feels across channels | "Buying my first policy" - awareness, comparison, signup, onboarding, first claim |

A journey often **crosses several processes** and several channels. A BPMN
might miss the call to support after the email arrives; a journey will not.

---

## What goes in a journey file

- **Persona** - which persona is doing the journey (link to `../personas/`).
- **Goal** - what the persona is trying to achieve.
- **Stages** - the sequence the persona lives through. For each stage:
  actions, touchpoint / channel, thoughts, emotions, pain points,
  opportunities.
- **Moments of truth** - the make-or-break interactions.
- **Metrics** - how we measure each stage (time, drop-off, NPS).
- **Sources** - interviews, observations, analytics.

---

## How to draft with AI

1. Pick a persona (from `../personas/`) and a goal.
2. Feed the agent the relevant interviews and observation notes.
3. Ask: *"Map the end-to-end journey of [Persona] trying to [Goal].
   Include every channel they touch (digital and non-digital). For each
   stage: actions, thoughts, emotions (1-5), pain points, opportunities.
   Mark the moments of truth."*
4. The analyst refines stages, validates emotional curve with users,
   identifies the highest-impact opportunities.

---

## Conventions

- File name = `<short-goal-name>.md` - e.g. `first-policy-purchase.md`,
  `password-recovery.md`.
- 5-9 stages per journey is the sweet spot.
- Use a Mermaid `journey` diagram for the emotional curve.
- **Language:** YAML keys and status enums in English. Section headings and
  prose in the project's `content_language` (see devflow/README.md ->
  Language policy, §3.15).

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| `draft`      | Mapped, not yet validated with users |
| `stable`     | Validated - safe input for UX, US prioritization, UAT scenarios |
| `deprecated` | No longer represents the experience (channel removed, redesign) |

See **[INDEX.md](INDEX.md)** for the journey listing.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../README.md) -> Language policy, §3.15).
