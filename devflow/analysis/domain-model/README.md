# Domain Model (Human-readable)

**Methodology version:** 5.2

## Purpose

This folder is the **human-readable representation** of the domain model:
entities, their properties, the relationships between them, and the
enumerations / value sets they use.

It is the **editable source of truth** for the domain. Code, database
schemas and API contracts should follow this — not the other way around.

> The technical formalization (DB schema, ER diagram, JSON Schema, etc.)
> lives in `discovery/` (legacy mapping) or in `spec/` (target design).
> `domain-model/` is the *business view*.

---

## Structure

```
domain-model/
├── README.md                            # This file
├── INDEX.md                             # Quick index of every entity
├── entities/
│   ├── README.md                        # What goes here, conventions
│   ├── TEMPLATE-ENTITY.md               # Copy this for each new entity
│   └── <EntityName>.md                  # One file per entity
├── enumerations/
│   ├── README.md                        # What goes here, conventions
│   ├── TEMPLATE-ENUM.md                 # Copy this for each new enum
│   └── <EnumName>.md                    # One file per enum
└── relationships/
    ├── README.md                        # What goes here, cardinality ref
    └── TEMPLATE-RELATIONSHIP.md         # Copy this for each new module file
```

---

## Per-entity file format

Each file under `entities/` follows
[TEMPLATE-ENTITY.md](entities/TEMPLATE-ENTITY.md):

- **YAML frontmatter** — parseable metadata (`entity`, `label`, `module`,
  `status`, `date`, `author`, `agent`, `llm`, `sources`, `tags`).
- **Description** — what this entity represents in the business.
- **Properties** — table with name, type, required, constraints,
  description.
- **Relationships** — table with target, cardinality, description (with a
  full cross-reference in the module's relationship file under
  [relationships/](relationships/)).
- **Business rules** — invariants and entity-level constraints.
- **Example** — a concrete instance to validate understanding.

---

## How to use

### Add a new entity

1. Copy [entities/TEMPLATE-ENTITY.md](entities/TEMPLATE-ENTITY.md) to
   `entities/<EntityName>.md`.
2. Fill in frontmatter, properties and relationships.
3. Add a row to [INDEX.md](INDEX.md).
4. If new relationships appeared, update the appropriate module file in
   [relationships/](relationships/) (use `TEMPLATE-RELATIONSHIP.md` to create one).
5. If new enums / coded states appeared, create a new file in
   `enumerations/` using [TEMPLATE-ENUM.md](enumerations/TEMPLATE-ENUM.md).

### Refactor an entity

- Mark the old entity `deprecated` in its frontmatter.
- Create the replacement and note the change in the History section of both
  files. Entities are **living documents**: they are edited in place and their
  history is the git log, so they carry no `version` field — only `vision/`
  and `scope/` are replaced as a whole by a numbered successor
  (see [`../README.md`](../README.md)).
- Open an ADR for any rename or split / merge that affects downstream Specs.

---

## Conventions

- Entities in **PascalCase** — `PaymentRequest.md`.
- Properties in **camelCase** — `dueDate`, `paymentMethod`.
- Data types follow XSD conventions: `string`, `integer`, `decimal`,
  `boolean`, `dateTime`, `date`.
- Cardinality: **per-side multiplicity** — `1`, `0..1`, `1..N`, `0..N` —
  written for both ends of a relationship as `<source> — <target>`
  (e.g. `1 — 0..N`). Pair notation (`1:N`, `M:N`) is **not** used: it cannot
  distinguish an optional end from a mandatory one, which is precisely what a
  domain model has to pin down. See
  [`relationships/README.md`](relationships/README.md).
- Enum properties marked as `→ Enum:<EnumName>` and cross-referenced in
  `enumerations/<EnumName>.md`.
- **Language:** YAML keys and status enums in English. Section headings,
  entity descriptions and property documentation in the project's
  `content_language` (see devflow/README.md -> Language policy, §3.15).

---

## Lifecycle (entity status)

| Status      | Meaning |
|-------------|---------|
| `draft`     | Identified but not fully modelled. Properties / relationships tentative. |
| `stable`    | Fully modelled and validated. Safe to use in User Stories and Specs. |
| `deprecated`| No longer relevant to the current domain. Kept as historical reference. |

`INDEX.md` reflects each entity's `status`.

---

## How to draft with AI

1. Feed the agent every transcript in `input/interviews/` plus any business
   documents in `input/documentation/`.
2. Ask in passes:
   - *"List candidate entities. For each: a 1-line definition and the
     interviews/docs where it was mentioned."*
   - *"For entity X, list candidate properties with type, required/optional,
     and any constraint heard in interviews."*
   - *"For entity X, list relationships to other entities with cardinality."*
   - *"Detect inconsistencies: properties that look like enums, candidate
     value sets, candidate sub-types."*
3. The analyst splits the agent's output into individual entity files,
   validates with stakeholders, and marks status.

---

## Relation to other folders

| Folder              | Relation                                                                  |
|---------------------|---------------------------------------------------------------------------|
| `../glossary/`      | Every entity name should match a glossary term                            |
| `../process/`       | Processes describe how entities flow; they reference entities defined here|
| `../../functional/` | User Stories use entity and property names from this model                |
| `../../adrs/`       | Structural decisions (entity split, rename, polymorphism) become ADRs     |
| `../../spec/`       | Specs implement what is modelled here                                     |
| `../../discovery/`  | Legacy DB / API mappings cross-link to entities here                      |

---

## Index

See **[INDEX.md](INDEX.md)** for the entity listing.

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
**last** in the analysis phase. It supplies the "things" the story is about.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
