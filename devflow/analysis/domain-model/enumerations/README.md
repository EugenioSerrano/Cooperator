# Enumerations and Value Sets

**Methodology version:** 5.2

## Purpose

One Markdown file per **enumeration (coded value set)** used by entities in
the domain model — statuses, types, categories and any property whose
values come from a closed, known list.

---

## What goes here

- **Coded states** — e.g. `OrderStatus.md`: PENDING, SHIPPED, DELIVERED.
- **Type classifiers** — e.g. `CustomerType.md`: INDIVIDUAL, COMPANY.
- **Category lists** — e.g. `ProductCategory.md`: ELECTRONICS, CLOTHING.
- **Any closed value set** referenced by an entity property marked
  `→ Enum:<EnumName>`.

---

## How to add an enum

1. Copy [TEMPLATE-ENUM.md](TEMPLATE-ENUM.md) to `<EnumName>.md`
   (PascalCase).
2. Fill in the YAML frontmatter, values table and "Used by" section.
3. Add the enum to [`../INDEX.md`](../INDEX.md).
4. In the entity file, mark the property as `→ Enum:<EnumName>`.

---

## Conventions

- File name = enum name in **PascalCase** — `OrderStatus.md`.
- Value codes in **UPPER_SNAKE_CASE** — `PENDING`, `SHIPPED`.
- Each file documents a single enum with all its allowed values.

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| `draft`      | Values tentative, not yet validated |
| `stable`     | Values locked — safe to use in Specs and code |
| `deprecated` | No longer used; kept as historical reference |

Set the `status` field in the enum's YAML frontmatter.


## Language

YAML keys, status enums, and IDs stay in **English**
(the schema). Section headings and all prose — descriptions, context,
rationale, findings — go in the project's content_language (see
[devflow/README.md](../../../README.md) -> Language policy, §3.15).
