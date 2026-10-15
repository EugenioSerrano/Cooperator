# Input — Source Code (raw legacy)

**Methodology version:** 5.2

## Purpose

Legacy source code or code from integrated systems, **as received**. Read-only
material used as reference for analysis, Discovery and domain modeling.

---

## What goes here?

- Complete repositories or snapshots of legacy code.
- Specific modules extracted for targeted analysis.
- Configurations, deployment scripts, and build files relevant to
  understanding the current system.
- Stored procedures, triggers, or embedded DB logic if they come as files.

## What does NOT go here?

- Code for the **new product** — that lives outside `devflow/`, in the
  project's code repository.
- Legacy analysis conclusions → `discovery/DISC-NNN` (or `analysis/` when the
  analysis is domain understanding, not a material unknown).
- Migration/rewrite decisions → `adrs/`.
- Credentials, tokens, files with secrets.

---

## Conventions

- Subfolders by system or module:
  `source-code/legacy-system-A/`, `source-code/cli-tooling-B/`.
- If the legacy repo is enormous, consider `git submodule` or a
  `README-pointers.md` pointing to the external repo instead of duplicating
  everything.
- Do not modify code here. Any refactor or port-over goes to the new product
  repository.

---

## Flow

```
input/source-code/  →  analysis/domain-model/ (entities, processes)
                    →  discovery/DISC-NNN (material unknown, e.g. legacy behavior)
                    →  adrs/
                    →  spec/ (migration guides)
```

---

## Document index

This folder does not use an INDEX.md — legacy sources are read as a tree, not
cited file by file. See [`../README.md`](../README.md) for the INDEX criterion.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
