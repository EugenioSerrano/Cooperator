# Sprints

**Methodology version:** 5.2

## Purpose

This folder holds the project's **sprint records**. A sprint record is the
**planning record** of one sprint: its start date, its end date, its goal, the
User Stories and Bolts in its scope and its declared `status`. **One sprint,
one document**: a sprint's dates, goal and scope live in its record and
nowhere else.

A sprint record is a planning record, not a report: its `SPRINT-NNN` number
is not the key of a sprint report, which keeps its own ISO-week name
(`REPORT-YYYY-Www.html`, in [`../reports/`](../reports/)).

- **Created only on a human's request.** A sprint record is opened when a
  human asks for it; an agent never opens one on its own initiative.
- **Links, never copies.** The record lists the IDs of the User Stories and
  Bolts in its scope, each as a link. Their state is read from their own
  documents — the sprint record never restates or overrides it.
- **No checkpoint, no manifest.** No AITL checkpoint applies to a sprint
  record, and no manifest records it.
- **Any number active, no team.** A project may have any number of sprints
  `active` at once, and a sprint record records no team.

---

## Creating a sprint record

1. **Claim the number** in [INDEX.md](INDEX.md) by adding the sprint's row
   under *Planned* before the record is created: the next number after the
   highest one listed there, in any section — the first sprint is
   `SPRINT-001`. A number once used — that of a `closed` or `cancelled`
   sprint included — is never reused or renumbered; gaps stay gaps (§5.15).
2. **Name the file** `SPRINT-NNN-<description>.md` (N29): `NNN` is that
   number; `<description>` is a short kebab-case ASCII slug in the project's
   `content_language`.
3. **Create it from [TEMPLATE-SPRINT.md](TEMPLATE-SPRINT.md)** with its start
   date, its end date, its goal, the links of the User Stories and Bolts in
   its scope and an empty scope-change log. Its `status` is `planned`.

A sprint whose end date is earlier than its start date is **invalid**.

---

## Status — one path

A sprint record's `status` is one of `planned`, `active`, `closed` and
`cancelled` — the values of §3.15's sprint record row — and a new record is
created `planned`. As that row states, the status is *Declared in the sprint
record and never derived from its dates: the dates are the plan, the `status`
is the declared state, so a sprint whose end date has passed stays `active`
until its `status` is changed. `closed` and `cancelled` are final.*

The only allowed changes are:

| From | To | When |
|------|----|------|
| `planned` | `active` | the sprint starts |
| `active` | `closed` | the sprint ends |
| `planned` | `cancelled` | the sprint is abandoned before it starts |

**No other change is permitted** — never back to an earlier value, never
`planned` → `closed` without passing through `active`, never `active` →
`cancelled`, and never any change once the sprint is `closed` or `cancelled`.

---

## Scope while the sprint runs

- **While `planned`**, the scope may still change: a User Story or Bolt is
  added to or removed from the *Scope* list without a log line.
- **While `active`**, adding or removing a User Story or Bolt updates the
  *Scope* list **and** appends one dated line to the *Scope-change log*: the
  date, what was added or removed, and by whom. The log is **append-only**:
  a line already in it is never edited or removed.

## When the sprint is closed or cancelled — frozen

A `closed` or `cancelled` sprint is never reopened, and its whole document is
**frozen**: nothing in it is edited again — not its `status`, its dates, its
goal, its scope or its scope-change log. Its number is never reused. Work
still pending belongs in another sprint's scope.

---

## Before this family is installed

A project whose installed methodology does not yet define the `sprints/`
family creates no `sprints/` folder by hand (G30). The family arrives with the
methodology upgrade that installs it (§5.16).

---

## Index

See **[INDEX.md](INDEX.md)** for the sprint list and the next sprint number.

---

## Language

File names, the `SPRINT-` prefix, frontmatter keys, `status` values and the
section headings of a sprint record stay in the schema's English, as in every
artifact family outside `analysis/`, feature User Stories and Test Cases. The
prose of a record and its `<description>` slug follow the project's
`content_language`, declared in [`../LANGUAGE`](../LANGUAGE) (§3.15).

---

## Relations to other folders

| Folder | Relation |
|--------|----------|
| `functional/` | The User Stories and Bolts a sprint links in its scope; their state lives there, never in the sprint record |
| `reports/` | Sprint progress reports, named by ISO week; a sprint record's number is not their key |
| `metrics/` | No manifest records a sprint; the manifests of the linked User Stories and Bolts are their own |
