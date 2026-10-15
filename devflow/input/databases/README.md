# Input — Databases (raw material)

**Methodology version:** 5.2

## Purpose

This folder stores **raw** material related to databases from the legacy system
or from systems the project integrates with. It is read-only material,
**before any analysis**.

---

## What goes here?

- Complete DDL (CREATE TABLE / VIEW / PROCEDURE) exported from the legacy system.
- Entity-Relationship diagrams exported (images, PDFs, files from tools like
  ERwin, DBeaver, MySQL Workbench).
- Structural backups (schema-only).
- Sample datasets or anonymized data samples.
- Vendor data catalog documentation.

## What does NOT go here?

- The **domain model** derived from analysis → `analysis/domain-model/`.
- Decisions about migration or future schema → `adrs/`.
- Database analysis findings → `discovery/DISC-NNN` (or `analysis/` when the
  analysis is domain understanding, not a material unknown).
- Credentials or backups with real data (PII). **Never** commit secrets.

---

## Conventions

- Keep files in their original format (`.sql`, `.png`, `.pdf`, `.xml`).
- Subfolders by source system if multiple sources exist:
  `databases/legacy-system-A/`, `databases/erp-B/`.
- If a file is very large (> 25 MB), reference external storage in a local
  `README-pointers.md` instead of committing the binary.
- Anonymize sensitive data before including samples.

---

## Flow

```
input/databases/  →  analysis/domain-model/ (entities, relationships)
                  →  discovery/DISC-NNN (material unknown: legacy behavior)
                  →  adrs/ (decisions)
```

---

## Document index

This folder does not use an INDEX.md — schemas, dumps and extracts are cited
as a folder, not file by file, and their contents change with every refresh.
See [`../README.md`](../README.md) for the INDEX criterion.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
