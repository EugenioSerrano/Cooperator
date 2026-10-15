# Relationships

**Methodology version:** 5.2

## Purpose

This folder holds the **centralized catalog of cross-entity relationships**
in the domain model. Each entity file lists its own relationships, but this
catalog provides the full cross-cutting view used for sanity checks and
ER-style diagrams.

---

## What goes here

- **Relationship entries** — source entity, target entity, cardinality,
  description.
- **ER diagram** (Mermaid `erDiagram`) — visual overview of the model.
- **Cardinality reference** — notation guide.

---

## File organization

One file is enough for most projects. Split by module / bounded context if
the model grows very large:

```
relationships/
+-- README.md                    # This file
+-- TEMPLATE-RELATIONSHIP.md     # Copy for a new module file
+-- billing-relationships.md     # Module-specific (optional)
+-- logistics-relationships.md   # Module-specific (optional)
```

---

## Cardinality notation

Cardinality is **per-side multiplicity**, written for both ends as
`<source> — <target>`. Each end independently states how many instances
participate, so optionality is never lost.

| Multiplicity | Meaning at that end |
|--------------|---------------------|
| `1`     | Exactly one — mandatory |
| `0..1`  | Zero or one — optional |
| `1..N`  | One or more — mandatory, repeating |
| `0..N`  | Zero or more — optional, repeating |

| Example | Reads as |
|---------|----------|
| `1 — 0..N`    | One source, zero or more targets |
| `1 — 1..N`    | One source, at least one target (the target end is mandatory) |
| `0..1 — 1`    | An optional source for exactly one target |
| `0..N — 0..N` | Many to many — requires a join entity in implementation |

> `1 — 0..N` and `1 — 1..N` both collapse to `1:N` in pair notation; that lost
> distinction is why pairs are not used here (see
> [`../README.md`](../README.md) → Conventions).

---

## How to add a relationship

1. Document the relationship in **both** the source entity file
   (`entities/<Entity>.md`, section 3) and in the catalog file
   (created from `TEMPLATE-RELATIONSHIP.md`).
2. Update the Mermaid `erDiagram` block if one exists.
3. Validate cardinality from both sides -- the multiplicities of `A — B` read
   in reverse must be the multiplicities recorded for `B — A`.

---

## Lifecycle

Relationship files follow the same lifecycle as entities: `draft` -->
`stable` --> `deprecated`. Set the status in the file's YAML frontmatter.

---

## Example: relationships table and ER diagram

When populating a concrete relationships file (created from
[TEMPLATE-RELATIONSHIP.md](TEMPLATE-RELATIONSHIP.md)), use the structure
below.

### Relationships table

| Relationship | Source | Target | Cardinality | Description |
|--------------|--------|--------|:-----------:|-------------|
| *(populate as entities are added)* |  |  |  |  |

### ER diagram (Mermaid)

```mermaid
erDiagram
    %% Add entities and relationships here as the model grows
    %% CUSTOMER ||--o{ CONTRACT : signs
    %% CONTRACT ||--o{ INVOICE  : generates
```


## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../../README.md) -> Language policy, §3.15).