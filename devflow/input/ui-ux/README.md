# Input — UI/UX (raw visual reference)

**Methodology version:** 5.2

## Purpose

This folder stores **raw UI/UX reference material** from legacy systems,
client-provided designs, and any visual artifacts that serve as input for
the analysis and design of the new system.

This is read-only reference material — it captures the "before" state of user
interfaces and design guidelines to inform analysis, Discovery and subsequent
development.

---

## What goes here?

- Screenshots of existing/legacy system screens and workflows.
- Client-provided wireframes, mockups, or prototypes (images, PDFs, Figma exports).
- Brand guidelines and style guides from the client.
- UX research materials: user journey maps, persona documents, usability reports.
- Design system specifications from the legacy system (if any exist).
- Annotated screenshots highlighting pain points or areas for improvement.
- Video recordings of user interactions with the current system.

## What does NOT go here?

- New designs created by the team → those belong in the project's design repository
  or `analysis/` once formalized.
- Analysis or interpretation of the visual material → `analysis/ui/` (surfaces,
  patterns, states, parity contracts), `analysis/` (domain understanding) or
  `discovery/DISC-NNN` (material unknown).
- Design decisions for the new system → `adrs/`.
- UI component specifications for implementation → `spec/`.

---

## Organization

Subfolders by system or design source:

```
ui-ux/
├── legacy-system/       → Screenshots and captures of the current application
├── client-mockups/      → Wireframes and mockups provided by the client
├── brand-guidelines/    → Logos, color palettes, typography specs
├── competitive-analysis/ → Screenshots of competing or reference products
└── user-research/       → Personas, journey maps, usability test recordings
```

---

## Conventions

- Keep files in their original format (`.png`, `.jpg`, `.pdf`, `.mp4`).
- Use descriptive filenames: `login-screen-current.png`, `checkout-flow-legacy.mp4`.
- Annotate screenshots externally (via companion `.md` notes), never modify originals.
- If files are very large, use external storage pointers via a local
  `README-pointers.md`.

---

## Flow

```
input/ui-ux/  →  analysis/ui/ (surfaces, patterns, states, parity contracts)
              →  analysis/user-journeys/ + personas/ (experience understanding)
              →  analysis/process/ (workflows)
              →  discovery/DISC-NNN (material unknown, if any)
              →  functional/ (User Stories)
              →  spec/ (UI implementation specs)
```

The visual evidence captured here feeds directly into understanding current user
workflows and identifying improvement opportunities for the new system.

---

## Document index

This folder does not use an INDEX.md — visual artifacts are organized by
subfolder. See each subfolder's contents for reference material.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in
[`../../LANGUAGE`](../../LANGUAGE) (see §3.15).
