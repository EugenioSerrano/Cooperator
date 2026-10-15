# Entities

**Methodology version:** 5.2

## Purpose

One Markdown file per **domain entity**. Each file captures the business
meaning, properties, relationships, business rules and a concrete example
of a single entity in the domain model.

---

## How to add an entity

1. Copy [TEMPLATE-ENTITY.md](TEMPLATE-ENTITY.md) to `<EntityName>.md`
   (PascalCase).
2. Fill in the YAML frontmatter and every section.
3. Add the entity to [`../INDEX.md`](../INDEX.md).
4. Mirror any new relationships in the appropriate module file under
   [`../relationships/`](../relationships/) (use `TEMPLATE-RELATIONSHIP.md`).
5. If the entity introduces new enums, add them to
   [`../enumerations/`](../enumerations/) (use `TEMPLATE-ENUM.md`).

---

## Conventions

- File name = entity name in **PascalCase** — `PaymentRequest.md`.
- Properties in **camelCase** — `dueDate`, `paymentMethod`.
- Data types: `string`, `integer`, `decimal`, `boolean`, `dateTime`, `date`.
- Required: ✅ / ❌.
- Enum references: `→ Enum:<EnumName>`.

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| `draft`      | Identified but not fully modelled |
| `stable`     | Validated — safe to use in User Stories and Specs |
| `deprecated` | No longer relevant; kept as historical reference |

Set the `status` field in the entity's YAML frontmatter.


## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../../README.md) -> Language policy, §3.15).
