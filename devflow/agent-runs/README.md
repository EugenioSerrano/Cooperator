# Agent runs

**Methodology version:** 5.2

## Purpose

This folder holds the project's **agent-run records**. An **agent run** is a
session launched by a human in which the Coordinator delegates the work of
that human's mandate to the project's DevFlow Agents (§2.18). Every agent run
leaves **exactly one record** here — one dated file that names the human who
launched the run, keeps its timeline and links the open questions it raised —
so that anyone can later reconstruct what the agents did, who launched it and
which questions it left behind.

An agent run is neither an integration test run (§3.2.2) nor an entry of a
manifest's `generation.runs[]` (§3.12) — they share only the word.

A run record is an **operational narrative**: the run's own chronology, not a
summary of governed artifacts. It is **never governed input**:

- **Never evidence.** A run record is never the source or justification of a
  SPEC, Bolt, ADR, US, TC, BUG, MEM, or any AITL checkpoint (G28). Cite the
  governed artifact the run produced or changed instead.
- **The artifact wins.** Where a run record and a governed artifact disagree
  — an approval, a status, a count — the artifact is the truth. A record
  links to the artifact; it never states an approval decision as a fact of
  its own.
- **No checkpoint, no manifest, no status.** No AITL checkpoint applies to a
  run record, no manifest records it, and it carries no `status` field
  (§3.15): a run is closed once its record's *Outcome* section is written.
- **Not a derivative document.** It carries no `derivative: true` (§5.5); its
  rules are the ones in this README.

---

## Opening a run record

A record is opened when a human launches an agent run, **before the first
delegation**:

1. **The launcher is a human.** `launched_by` is required and names the human
   who launched the run, in actor grammar (`human:<user>`). A record without
   `launched_by`, or with an `agent:` actor there, is **invalid**: an agent run
   is always launched by a human, and no record is opened without one. The
   human's launch is the record's mandate — an agent never opens a run record
   on its own initiative.
2. **Read the time from the system clock** — never invent it — and name the
   file `ARUN-YYMMDD-HHmm-<description>.md` (N28): `YYMMDD-HHmm` is the local
   wall-clock time the run opens; `<description>` is a short kebab-case ASCII
   slug in the project's `content_language`.
3. **Claim the run number** in [INDEX.md](INDEX.md): a new row whose number is
   the next after the highest one recorded there. A number once claimed is
   never reused (§5.15). `run: N` is that number and nothing else.
4. **Create the file from [TEMPLATE-AGENT-RUN.md](TEMPLATE-AGENT-RUN.md)** and
   write its *Mandate* and *Standing rules* before the first delegation.

**One run, one file.** A run that spans several sessions or days keeps the
same file and gains timeline rows; a new file is opened only for a new run.

---

## While the run is open — append only

- Each step of the run is a **new row** of the *Timeline*: `When` from the
  system clock, `Actor` in actor grammar (`human:<user>` or `agent:<id>`), the
  action, the artifact it touched and its result.
- An earlier row is **never edited**. A correction is a new row whose
  *Action* names the row it corrects by that row's `When`.
- Each artifact the run touches is added to the frontmatter's `artifacts`
  list, by its path, when the run first touches it.

## When the run closes — frozen

The run closes when its *Outcome* section is written, with the final
*Artifacts & digests*. From then on the record is **frozen**: nothing in it is
edited and nothing is appended. A later fact belongs to a new run or to the
governed artifact it concerns.

---

## Questions opened during a run

A question that arises during a run lives **only** as an `OQ-NNN` file in
[`../analysis/open-questions/`](../analysis/open-questions/), under that
family's own rules. The run's *Questions opened* section lists each one as a
link with its ID and title — **never the question or answer text**. A reader
who starts from the run reaches every OQ it opened by following those links.

---

## Before this family is installed

A project whose installed methodology does not yet define the `agent-runs/`
family keeps its run logs in the agent's own `agents-data/<agent>/` folder —
never citable (G32) — and creates no `agent-runs/` folder by hand (G30). The
family arrives with the methodology upgrade that installs it (§5.16).

---

## Index

See **[INDEX.md](INDEX.md)** for the run list and the next run number.

---

## Language

File names, the `ARUN-` prefix, frontmatter keys and actor IDs stay in the
schema's English, and so do the section headings of a run record, as in every
artifact family outside `analysis/`, feature User Stories and Test Cases. The
prose of a record and its `<description>` slug follow the project's
`content_language`, declared in [`../LANGUAGE`](../LANGUAGE) (§3.15).

---

## Relations to other folders

| Folder | Relation |
|--------|----------|
| `analysis/open-questions/` | The only home of a question a run raises; the run links it, never copies it |
| `agents-data/` | Never a run record's home once this family is installed; before it, where run logs stay (G32) |
| `memory/`, `metrics/` | A run record is neither a MEM nor a manifest: each V-Bounce a run triggers keeps its own MEM and manifest entry |
