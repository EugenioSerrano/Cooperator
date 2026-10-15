# Personas

**Methodology version:** 5.2

## Purpose

One file per **user archetype** — which can be either a **real persona** (grounded
in actual interviews) or a **role-based archetype** (when no real person data exists).

Personas turn vague User Stories (*"As a user…"*) into stories with intent (*"As a
busy parent who uses the app on the bus…"* or *"As a field technician working offline…"*).

Personas are the **input that lets the AI write meaningful US/AC**, design
useful UX, and pick relevant UAT scenarios.

---

## Real Personas vs. Archetypes

| Aspect | Real Persona (`persona_type: real`) | Archetype (`persona_type: archetype`) |
|--------|-------------------------------------|---------------------------------------|
| **Based on** | Actual interviews, direct observation, or verifiable user research | Domain knowledge, analytics, stakeholder input, or industry patterns |
| **Identifier** | Real person's name (e.g. `MariaGonzalez`) | Descriptive role name (e.g. `BusyParent`, `FieldTechnician`) |
| **Age range** | Actual age range from source | Not applicable — use "Varies" or omit |
| **Quote** | Verbatim from an interview | Not applicable — use "—" or state it explicitly |
| **Sources** | **Required** — must reference specific interviews (INT-NNN) | Optional — may reference research or domain docs |
| **Use when** | You have interview transcripts, user research sessions, or real user data | You need to model a user segment but lack direct user research |

### The Golden Rule

> **NEVER invent a person that does not exist.** Do not fabricate names, ages,
> occupations, family situations, or personal quotes. If you don't have real
> interview data, use an archetype. A fake persona is worse than no persona —
> it creates false precision and misleading empathy.

---

## Personas vs. Stakeholders (do not confuse)

| Artifact | Lives in | Granularity | Example |
|----------|----------|-------------|---------|
| **Stakeholder** | `../business-context/` | Organizational role / function | "Sales rep", "Compliance officer", "Internal IT" |
| **Persona**     | `personas/` (here)     | Concrete archetype of an end user | "Busy parent, low digital trust" or "Maria Gonzalez (real, from INT-003)" |

A stakeholder is **who is institutionally involved**; a persona is **who
actually uses the product**. They can overlap (a Sales rep can also be a
persona) but the lens is different.

---

## What goes in a persona file

### Always included (both types)

- **Type declaration** — `persona_type: real` or `persona_type: archetype`.
- **Identifier & label** — name (real) or role identifier (archetype) plus a human-readable description.
- **Role / context** — what they do when they encounter the product.
- **Goals** — what they are trying to achieve.
- **Pain points** — what frustrates them today.
- **Behaviour & context** — when / where / how they interact (device, attention level, environment).
- **Digital literacy** — comfort with technology.
- **Success criteria** — what winning looks like for them.
- **Anti-patterns** — what would alienate them.

### Only for real personas

- **Age range** — actual age from source.
- **Verbatim quote** — a real sentence from an interview, with source reference.
- **Sources** — mandatory interview references (INT-NNN).

---

## How to draft with AI

### When you have interviews

1. Feed the agent all transcripts in `../../input/interviews/`.
2. Ask: *"Identify the real individuals mentioned in these interviews who are
   end-users of the product. For each person: extract their name, age range,
   role, top 3 goals, top 3 pain points, context of use, digital literacy,
   and a representative verbatim quote with source reference."*
3. The analyst names them, refines with stakeholders, marks status.

### When you do NOT have interviews

1. Ask: *"Based on the domain context and stakeholder input, identify 3–5 user
   archetypes (roles). For each archetype: descriptive identifier (NOT a fake
   person name), role description, top 3 goals, top 3 pain points, context of
   use, and digital literacy. Do NOT invent names, ages, or personal details."*
2. Use `persona_type: archetype` for all of them.
3. The analyst validates with stakeholders and marks status.

### ⚠️ Anti-pattern: fabricated personas

When the agent creates a persona with a made-up name, age, and personal story,
it is generating **false data**. This is worse than having no persona because:

- It creates the illusion of user research where none exists.
- It can mislead design decisions with fake precision.
- It undermines the credibility of the analysis folder.

**If you catch this, stop and switch to archetypes.**

---

## Conventions

- **File name:**
  - Real persona: `<RealName>.md` in PascalCase — `MariaGonzalez.md`.
  - Archetype: `<RoleIdentifier>.md` in PascalCase — `BusyParent.md`, `FieldTechnician.md`.
- **3–5 personas is the sweet spot.** More than 7 is usually noise.
- **Always set `persona_type`** explicitly. The template defaults to `archetype`
  as the safer choice.
- **Language:** YAML keys and status enums in English. Section headings and
  prose in the project's `content_language` (see devflow/README.md ->
  Language policy, §3.15).

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| `draft`      | Identified, not yet validated with stakeholders |
| `stable`     | Validated — safe to use in US, UX and UAT |
| `deprecated` | No longer represents the target user |

See **[INDEX.md](INDEX.md)** for the persona listing.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../README.md) -> Language policy, §3.15).
