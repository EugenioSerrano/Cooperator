# Process (Business Processes — BPMN)

**Methodology version:** 5.2

## Purpose

This folder defines the **business processes** of the domain, modelled using
the **BPMN 2.0** notation. Processes describe the **dynamic behaviour** of
the system: which activities are executed, in what order, by whom, and which
domain entities are touched along the way.

Where `domain-model/` answers **what** the business deals with (static
entities), `process/` answers **how** those entities flow.

---

## What goes here

- **Business processes** — full workflows (e.g. "Customer onboarding",
  "Invoicing").
- **Sub-processes** — complex activities decomposed into smaller steps.
- **Events** — triggers (start), intermediate, terminating.
- **Gateways** — decision points (exclusive, parallel, inclusive).
- **Participants and roles** — who executes each activity (lanes / pools).
- **Data flows** — which entities from `domain-model/` are created, read,
  modified or deleted.

---

## Format

- One Markdown file per process, with YAML frontmatter.
- Diagrams in **Mermaid** (`flowchart TB` or BPMN-lite style).
- **Language:** YAML keys and status enums in English. Section headings and
  prose in the project's `content_language` (see devflow/README.md ->
  Language policy, §3.15).

Use **Mermaid** for inline diagrams inside Markdown (renders in GitHub and
VS Code). Optionally store full `.bpmn` (BPMN 2.0 XML) files alongside when
a specialised modeller is in use.

Each process file follows [TEMPLATE-PROCESS.md](TEMPLATE-PROCESS.md).

### Minimal Mermaid example

```mermaid
flowchart TB
    subgraph "Process: Customer onboarding"
        A["Receive request"] --> B{"Existing customer?"}
        B -->|Yes| C["Update data"]
        B -->|No|  D["Create Customer entity"]
        D --> E["Assign segment"]
        C --> F["Notify sales rep"]
        E --> F
        F --> G["End"]
    end
```

---

## File organization

**Flat, no subfolders.** The `devflow/` structure is canonical: creating a
folder inside `process/` (or anywhere else under `devflow/`) outside the
structure defined in §5.1 is a **G30 violation**. Group by naming the file,
not by nesting it:

```
process/
├── PROC-001-customer-onboarding.md
├── PROC-002-invoicing.md
├── PROC-003-invoicing-tax-validation.md   ← sub-process: its own PROC-NNN
├── PROC-004-sales-quoting.md
├── PROC-005-sales-deal-close.md
├── PROC-006-operations-dispatch.md
└── PROC-007-collections-dunning.md
```

Every file is `PROC-NNN-<description>.md` (**N19**) with its own sequential
`NNN` claimed in [INDEX.md](INDEX.md) — there are no `PROC-002a`-style
suffixes and no ID-less filenames. A sub-process is a process: it gets its
own ID and records its parent in the document's traceability section. When a
functional area needs to stand out, put it in the description slug
(`PROC-004-sales-quoting.md`) and group the rows by area inside the INDEX.

---

## How to draft with AI

1. Feed the agent all `input/interviews/` transcripts and any operational
   handbook in `input/documentation/`.
2. Ask in passes:
   - *"List every business process mentioned. For each: trigger, actors,
     end state and any cited rule."*
   - *"For process X, propose the activities in order, decisions and
     exceptions. Mark which `domain-model/` entities are touched."*
   - *"Detect process variants and edge cases the interviewees hinted at
     ('what if the customer cancels mid-flow?')."*
3. The analyst translates that into a `PROC-NNN-*.md` per process, draws
   the Mermaid diagram, and validates with stakeholders.

---

## How processes feed downstream

```
input/interviews/  →  process/ (BPMN)  ┐
                                       ├─►  functional/ (US + AC)
domain-model/ (entities)               ┘
```

Each activity (or coherent group of activities) in a process maps to one or
more **User Stories** with acceptance criteria derived from the process's
business rules and exceptions. Cross-link from each US to the originating
process.

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| `draft`      | Process identified but not yet validated with stakeholders. |
| `active`     | Process reflects the current (or target) business flow. |
| `deprecated` | No longer reflects current operations. Kept as historical reference. |

> **Note:** `process/` uses `active` instead of the `stable` used by most
> other `analysis/` subfolders because processes describe *dynamic behaviour*
> that is currently in effect, not a static reference that was reviewed once.

`INDEX.md` separates active from deprecated processes.

---

## Recommended tooling

- **Mermaid** — inline diagrams in Markdown (recommended for this flow).
- **Camunda Modeler** — free visual BPMN 2.0 editor.
- **bpmn.io** — open-source web editor.
- **Bizagi Modeler** — free visual editor (Windows).

---

## References

- [BPMN 2.0 Specification (OMG)](https://www.omg.org/spec/BPMN/2.0/)
- [Mermaid Flowchart Syntax](https://mermaid.js.org/syntax/flowchart.html)

---

## Index

See **[INDEX.md](INDEX.md)** for the list of documented processes.

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
**last** in the analysis phase. It supplies the step-by-step spine of the story.

That narrative is **derivative** (§5.5): it never introduces a rule of its own
and is never governed input (G28). When a change here alters something the
narrative states, update the narrative in the same pass — or mark it
`deprecated` rather than let it keep circulating.
