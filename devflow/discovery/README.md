# Discovery (Need-driven Investigation)

**Methodology version:** 5.2

## Purpose

This folder stores **focused, traceable investigations** (`DISC-NNN`) used to
reduce an **important uncertainty before a User Story or Bolt is created or
materially refined** (§2.13).

Typical Discoveries examine an external API, an unfamiliar library or
framework, a legacy-system behavior, an integration constraint, a technology
option, data availability, or another question that must be understood before
the team can define the right work.

A Discovery creates **evidence and conclusions for backlog definition** — it
does **not** inspect an existing project artifact for quality and does
**not** authorize implementation.

---

## Need-driven, not mandatory

A Discovery is **not required for every User Story or Bolt**. Any stakeholder
or team member may initiate one when a material unknown would otherwise force
the team to guess.

**Once initiated, its governance is mandatory:**

- It remains a **draft** until a qualified human — **the author included** —
  records `AITL-DISC-Approval`.
- Its conclusions **cannot be used as governed input** (analysis, USs,
  Bolts, ADRs, risks) until approved.
- `AITL-DISC-Approval` confirms the research question was answered with
  adequate evidence, the limits and assumptions are explicit, and the
  conclusions are reliable enough to guide backlog or architecture work.
- It does **not** approve any downstream artifact — each US, Bolt, ADR or
  risk created from the Discovery follows its own lifecycle and AITL
  approval.
- **No code without a Bolt:** if the research requires executable prototype
  or spike code, that experiment must be authorized by an **approved
  non-functional Bolt under `US-000-non-functional.md`** before any code is
  generated.

```mermaid
flowchart TD
    Q["Material unknown before a US or Bolt"] --> D["Run DISC investigation"]
    D --> F["Document evidence, limits, and conclusions"]
    F --> H{"AITL-DISC-Approval"}
    H -->|"Changes requested"| D
    H -->|"Approved"| A["Update analysis and inform backlog decisions"]
    A --> U["Create or refine USs, Bolts, ADRs, or risks"]
    U --> O["Each artifact follows its own approval lifecycle"]
```

---

## Discovery vs. Review vs. Adversarial Review

| Dimension | Discovery (`DISC`) | Review (`REV`) / Adversarial Review (`AREV`) |
|-----------|--------------------|----------------------------------------------|
| Starting point | A material unanswered question **before** a US or Bolt is defined or refined | An existing project artifact, implementation, characteristic, or concern to inspect |
| Primary purpose | Learn, gather evidence, reduce uncertainty | Evaluate, challenge, identify findings |
| Typical subjects | External APIs, unfamiliar libraries, legacy behavior, technology options, data or integration constraints | Documentation, code, tests, USs, Bolts, ADRs, SPECs, architecture, security, performance, or process |
| Governed output | Approved evidence, assumptions, limits, conclusions | Approved findings; AREV exposes them only after its approved Verdict |
| AITL governance | `AITL-DISC-Approval` | `AITL-REV-Approval`, or the three sequential AREV phase approvals |
| Downstream effect | Informs analysis and the creation/refinement of governed artifacts | May create or update any governed artifact |

> An AREV is a **specialized adversarial form of Review**, not a Discovery.
> A Discovery does not create an exception to the rule that no code-related
> change may exist without an approved Bolt (§2.13).

---

## What goes here

- **Third-party API investigation** — endpoints, auth, rate limits, error
  handling, SLAs, versioning strategy.
- **Unfamiliar library / framework evaluation** — how to use it correctly,
  deprecations, breaking changes, fit for the project.
- **Legacy-system behavior analysis** — schema analysis, stored procedures,
  business rules embedded in code, data migration paths.
- **Integration constraint mapping** — how external systems connect, data
  formats, protocols, retry/fallback behaviour.
- **Technology evaluation** — frameworks, libraries, tools under
  consideration (POC results, benchmarks, trade-offs).
- **Data availability research** — what data exists, its quality, and
  whether it is accessible.
- **Vendor documentation analysis** — summarized datasheets, SDK
  walkthroughs, configuration guides.
- **Regulatory / compliance research** — external regulation analysis
  (the compliance *impact on our project* goes to
  `analysis/business-context/`).

## What does NOT go here

- **Quality review of existing project artifacts** → `reviews/` (`REV-NNN`).
- **Adversarial debate** → `adversarial-reviews/` (`AREV-NNN`).
- **Business analysis** (vision, personas, journeys, domain model)
  → `analysis/`.
- **Architectural decisions** → `adrs/` (`ADR-NNN`).
  A discovery *feeds* an ADR; it is not the decision itself.
- **Risks** → `risks/` (`RISK-NNN`). A discovery may
  *surface* a risk, but the risk is tracked in the register.
- **Bug fixes** → `bugs/` (`BUG-NNN`).

---

## Naming convention

```
DISC-NNN-short-description-in-kebab-case.md
```

---

## DISC structure

The authoritative structure is [`TEMPLATE-DISC.md`](TEMPLATE-DISC.md) — its
**11 numbered sections**, plus the frontmatter:

- **Frontmatter** — ID, descriptive title, date, author, `llm`, status, sources and the `review` block.
1. **Research question** — What material unknown is being reduced.
2. **Scope** — What the investigation covers and, explicitly, what it does not.
3. **Executive summary** — Main finding or most relevant conclusion.
4. **Inventory / Mapping** — Detailed listing: endpoints, tables, signals, components, configurations.
5. **Detailed findings** — In-depth description with code snippets, pseudocode or Mermaid diagrams.
6. **Experiments performed (if any)** — What was run, against what, and the result.
7. **Assumptions and limits** — What was NOT found or could not be determined; explicit limits of the investigation.
8. **Conclusions and recommendations** — Next steps, ADRs needed, risks surfaced, estimated impact.
9. **Sources** — Links to datasheets, external docs, vendor portals.
10. **History** — Change log.
11. **`AITL-DISC-Approval`** — The checkpoint that makes the findings actionable.

Diagrams in **Mermaid** (mandatory — no ASCII art or embedded images).

---

## How discovery feeds downstream (only after approval)

```mermaid
flowchart LR
    DISC["DISC-NNN<br/>discovery/<br/>(AITL-DISC-Approval)"]

    DISC -->|"Design decision needed"| ADR["ADR-NNN<br/>adrs/ (AITL-ADR-Approval)"]
    DISC -->|"Risk identified"| RISK["RISK-NNN<br/>risks/"]
    DISC -->|"Implementation task"| BOLT["BOLT<br/>functional/bolts/ (AITL-BOLT-READY-Approval)"]
    BOLT -->|"Blueprint"| SPEC["SPEC<br/>spec/ (AITL-SPEC-Approval)"]
    DISC -->|"Business behavior"| US["US-NNN<br/>functional/user-stories/ (AITL-US-Approval)"]
    DISC -->|"Needs internal review"| REV["REV-NNN<br/>reviews/ (AITL-REV-Approval)"]
    DISC -->|"Domain insight"| AN["analysis/<br/>(business-context,<br/>domain-model)"]
```

Each downstream artifact follows its own lifecycle and *applicable* AITL approval —
nothing is implied by `AITL-DISC-Approval` (RISK has no AITL checkpoint).

---

## Lifecycle

| Status       | Meaning |
|--------------|---------|
| **draft**    | Investigation running; conclusions not yet usable as governed input. |
| **approved** | `AITL-DISC-Approval` recorded; conclusions may inform analysis and backlog decisions. |
| **deprecated** | No longer relevant (legacy retired, technology discarded, API decommissioned). Kept as historical reference. |

`INDEX.md` reflects status: Draft / Approved / Deprecated.

---

## Relations to other folders

| Folder | Relation |
|--------|----------|
| `adrs/` | Discoveries feed architectural decisions (approved first) |
| `risks/` | Discoveries surface external risks (dependency, SLA, integration) |
| `functional/user-stories/` | Approved conclusions may support creation or refinement of feature USs (AITL-US-Approval) |
| `functional/bolts/` | Discoveries feed Bolts (Bolt-first rule) that produce SPECs |
| `spec/` | SPECs are created from Bolts triggered by discoveries |
| `reviews/` | A review may trigger a discovery (unknown external dep found); a discovery may trigger a review (external change requires internal audit) |
| `adversarial-reviews/` | AREV is a specialized form of Review — never a Discovery |
| `analysis/` | External findings may enrich business-context or domain-model |
| `memory/` | Implementation informed by discoveries is documented in MEM |
| `input/` | Raw external docs (datasheets, vendor manuals) live in `input/documentation/` |

---

## Index

See **[INDEX.md](INDEX.md)** for the full listing.

---

## Language

YAML keys, status enums, IDs, and template section headings stay in **English**
(the schema). All prose — descriptions, context, rationale, findings — goes in
the project's `content_language`, declared in [`../LANGUAGE`](../LANGUAGE)
(see §3.15).
