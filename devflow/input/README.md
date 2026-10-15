# Input (Raw Material)

**Methodology version:** 5.2

## Purpose

This folder stores **raw input material** — the unprocessed source materials
that serve as the evidence base of the entire Avenga DevFlow (§2.1): business
and regulatory documents, database evidence, third-party documentation,
recorded stakeholder interviews, legacy source code, and UI/UX evidence.

Unlike the other folders in the flow, material is deposited here **as
received**, without processing, classification, or interpretation. Once
analyzed, the extracted knowledge is documented in `analysis/` (domain
understanding) and, for material unknowns, in `discovery/` (DISC-NNN).

---

## What documents go here?

- Legacy source code from existing systems (complete repositories or extracts).
- Database schemas (DDL, ER diagrams, structural backups).
- Documentation of existing systems (manuals, functional specifications).
- Configuration files from legacy systems.
- PDFs, spreadsheets, diagrams, and any business reference material.
- Sample data dumps or test datasets from the current system.
- Screenshots, recordings, or any evidence of current system behavior.
- Datasheets and technical specifications for hardware or devices involved.

> **Note on interviews:** Transcriptions of stakeholder interviews go in
> `input/interviews/`. Only original recordings (audio/video) without
> transcription go in `input/` if needed as backup.

**Rule:** if the material comes from "outside" and has not been analyzed yet,
it goes here. It is **read-only evidence**: derived or normalized information
belongs in `analysis/`, `discovery/`, or another downstream folder (§5.6).

**Agents never write here.** Material is deposited by humans only. Agents may
read `input/` as evidence (SPEC inventory, analysis) but must not create,
modify, or move files into this folder or its subfolders (G31).

---

## Organization

Material is organized by **type of input**, each with its own subfolder and README:

```
input/
├── business/          → RFPs, BRDs, SOWs, compliance, regulations, business case
├── databases/         → Raw DB schemas, DDL, ER diagrams, sample data
├── documentation/     → Third-party reference: API manuals, vendor PDFs, datasheets
├── interviews/        → Stakeholder conversation transcriptions (primary analysis source)
├── source-code/       → Legacy source code and configuration files
└── ui-ux/             → Screenshots, mockups, brand guidelines, UX research
```

Each subfolder has its own `README.md` explaining what belongs there and how to
organize it. The goal is to preserve files in their **original format** — no
conversion to Markdown required.

---

## Relationship to the flow

```
INPUT (raw evidence) → analysis/ (domain understanding) + discovery/ (material unknowns)
                     → US / ADRs → SPEC → implementation
```

The material in `input/` **directly feeds** `analysis/` (AI-assisted,
Functional Analyst governs) and, when a material unknown must be investigated
before defining a US or Bolt, `discovery/` (§2.13). Original material is
preserved here as reference and traceability.

---

## Notes

- Files here are **never modified**. They are read-only.
- They do not require Markdown format — they are kept in their original format.
- **Record provenance:** for each deposited input, record its source, date and
  provider (filename convention or a companion note) so conclusions remain
  traceable (§2.1).
- Adding or materially changing an input triggers an **impact assessment**:
  every affected analysis artifact must be updated before downstream
  functional artifacts are approved or re-approved (§5.6).
- If a file is very large (e.g., DB backups), consider including only
  representative extracts or a pointer to external storage.
- **INDEX policy — one criterion, applied per subfolder:** a subfolder carries
  an `INDEX.md` **when its files are cited individually as evidence**; it does
  not when the folder is cited as a whole (§5.15).
  - `interviews/` (`INT-NNN`) and `documentation/` are cited one file at a
    time from `analysis/`, so both have an `INDEX.md` — `interviews/` as the
    `INT-NNN` allocator, `documentation/` as a curated inventory (its files
    carry no ID, §5.15).
  - `business/`, `ui-ux/`, `databases/` and `source-code/` are referenced as
    folders — their contents are unstructured or organized by subfolder — so
    they have none, and each states so in its own README.
  - `input/` itself has no root `INDEX.md`: it indexes nothing directly.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in [`../LANGUAGE`](../LANGUAGE)
(see §3.15). Interview transcripts and UAT minutes stay in the language they
were recorded in (§3.15).
