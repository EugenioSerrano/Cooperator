# Glossary (Ubiquitous Language)

**Methodology version:** 5.2

## Purpose

This folder contains the **domain glossary**: agreed-on definitions of the
business terms the team uses. It is the *ubiquitous language* (DDD) — the
shared vocabulary between stakeholders, analysts, developers and AI agents.

A term well defined here removes ambiguity from User Stories, Specs, code
and conversations. When two people use the same word with different
meanings, the glossary is the source of truth.

---

## What goes here

- **Business terms** — domain concepts with a precise definition.
- **Synonyms and aliases** — different words that refer to the same concept.
- **Context clarifications** — when a term shifts meaning across modules /
  bounded contexts.
- **Banned terms** — ambiguous words the team has decided NOT to use, and
  what to use instead.

---

## Format

Markdown with a simple tabular structure. One file for small projects,
multiple files (one per module / bounded context) for larger ones.
**Language:** YAML keys and status enums in English. Section headings and
term definitions in the project's `content_language` (see
devflow/README.md -> Language policy, §3.15).

### Entry shape

```markdown
### [Term]

| Field | Value |
|-------|-------|
| **Definition**       | What it means exactly in this project |
| **Synonyms**         | Other words that mean the same thing |
| **Do not confuse with** | Similar terms with different meaning |
| **Entity**           | Link to the entity in `domain-model/entities/` if any |
| **Example**          | Concrete case that illustrates the use |
| **Source**           | Interview / document / stakeholder where the term was first observed |
```

### Worked example

```markdown
### Customer

| Field | Value |
|-------|-------|
| **Definition**       | Individual or legal entity with at least one active contract with the company. A prospect is NOT a customer until they sign. |
| **Synonyms**         | Account, Client |
| **Do not confuse with** | Prospect (no contract), User (may be an internal employee) |
| **Entity**           | [Customer](../domain-model/entities/Customer.md) |
| **Example**          | "Jane Doe, ID 30.123.456, contract #FC-2024-001 active since 2024-03-15" |
| **Source**           | INT-002 (sales lead interview) |
```

Use [TEMPLATE-GLOSSARY.md](TEMPLATE-GLOSSARY.md) for new files.

---

## File organization

Free organization. Two common patterns:

```
glossary/
└── glossary-general.md       # everything in one file (small projects)
```

Or by bounded context:

```
glossary/
├── sales.md
├── billing.md
├── logistics.md
└── README.md
```

---

## How to build the glossary with AI

1. **Term extraction** — point the agent at `input/interviews/` and ask:
   > *"List every business term used. For each: best-guess definition,
   > synonyms heard, conflicting definitions (if any), source interview."*
2. **Conflict detection** — ask the agent to highlight terms used with
   different meanings across interviews. The human picks the official
   definition.
3. **Link to domain-model** — every term that is also an entity must
   reference its file in `domain-model/entities/`.
4. **Validate with the team** — the glossary is *consensual*, not imposed.
5. **Keep it alive** — update when new terms appear or definitions change.

---

## Position in the flow

```
input/interviews/  →  glossary/ (agreed terms)
                          ├─►  domain-model/ (formal model)
                          ├─►  functional/  (US use these terms)
                          └─►  spec/        (code uses these names)
```

The glossary is the bridge between business language and the technical
model. Entity names in `domain-model/`, variables in code and API field
names should match the terms defined here.

---

## Lifecycle

The glossary is a **living document**: edited in place when new terms
appear, definitions change or conflicts surface. It is not versioned per
entry — `INDEX.md` simply lists active files.

If a term becomes obsolete, mark it `deprecated` and explain what replaced
it. Do not delete history.

---

## Index

See **[INDEX.md](INDEX.md)** for the list of glossary files.

---

## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../README.md) -> Language policy, §3.15).

---

## Feeds the introduction narrative

Once this artifact exists — draft is enough — it feeds
[`../introduction/`](../introduction/), the plain-language entry point written
**last** in the analysis phase. It supplies the agreed words the narrative must use.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
