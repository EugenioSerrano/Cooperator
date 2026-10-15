---
id: "SPRINT-NNN" # the number claimed in INDEX.md — the next after the highest listed, never reused (N29, §5.15)
date: "YYYY-MM-DD" # the date the record was created, from the system clock
author: "human:<user>" # the human who requested the sprint record — a sprint record is created only on a human's request
agent: "" # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "" # exact model id when an AI writes this record (e.g. claude-opus-5-5), or null when a human does (§3.0)
status: "planned" # planned | active | closed | cancelled — created planned; changed only as the README allows; closed and cancelled are final
start_date: "YYYY-MM-DD" # the sprint's first day — the plan
end_date: "YYYY-MM-DD" # the sprint's last day — never earlier than start_date: a sprint whose end date is earlier than its start date is invalid
---

<!--
  LANGUAGE POLICY (§3.15): frontmatter keys, `status` values, IDs and the
  section headings (##) stay in English (the schema). The goal and the other
  prose go in the project's content_language (declared in devflow/LANGUAGE).

  ⚠️ CREATED ONLY ON A HUMAN'S REQUEST: an agent never opens a sprint record
     on its own initiative. Claim `id` in INDEX.md.
  ⚠️ STATUS: created `planned`; the only changes are planned → active,
     active → closed and planned → cancelled. Declared, never derived from
     the dates: a sprint whose end date has passed stays `active` until its
     `status` is changed (§3.15).
  ⚠️ LINKS, NEVER COPIES: the Scope lists the IDs of User Stories and Bolts
     as links; their state is read from their own documents.
  ⚠️ SCOPE CHANGES: while `planned`, the Scope may change without a log
     line; while `active`, every addition or removal also appends one line to
     the Scope-change log, which is never edited.
  ⚠️ FROZEN once `closed` or `cancelled`: nothing in this record is edited
     again; work still pending goes into another sprint's scope.
  A sprint record has no AITL checkpoint and no manifest.
-->

# SPRINT-NNN — [short description]

## Goal

<!-- What the sprint sets out to achieve. -->

[the goal]

## Scope

<!-- One line per User Story or Bolt in the sprint: a link whose text is its
     ID — never its status or any other state of it. -->

- [US-NNN](../functional/user-stories/US-NNN-<description>.md)
- [US-NNN.BOLT-NNN](../functional/bolts/US-NNN.BOLT-NNN-<description>.md)

## Scope-change log

<!-- Empty while the sprint is planned. While it is active, one row per
     addition or removal, appended in order and never edited or removed.
     Date: system clock, YYYY-MM-DD. Change: added or removed. Item: the
     User Story or Bolt ID. By: human:<user> or agent:<id>. -->

| Date | Change | Item | By |
|------|--------|------|----|
