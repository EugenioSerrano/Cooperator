---
id: "ARUN-YYMMDD-HHmm" # the filename stem: the system-clock time the run opened (N28)
date: "YYYY-MM-DD" # the date the run opened, from the system clock
run: N # the run number claimed in INDEX.md — the next after the highest recorded, never reused (§5.15)
launched_by: "human:<user>" # REQUIRED — the human who launched the run (actor grammar); missing, or an agent: actor, makes the record invalid
coordinator: "" # optional — agent:<id> of the Coordinator that keeps this record, or null
artifacts: [] # paths of the artifacts the run touched, each added when the run first touches it
llm: "" # exact model id when an AI writes this record (e.g. claude-opus-5-5), or null when a human does (§3.0)
---

<!--
  LANGUAGE POLICY (§3.15): frontmatter keys, IDs, actor IDs and the section
  headings (##) stay in English (the schema). All prose — the mandate as the
  human gave it, the actions and results of the timeline, the outcome — goes
  in the project's content_language (declared in devflow/LANGUAGE).

  ⚠️ NEVER EVIDENCE (G28): a run record is an operational narrative — never
     the source or justification of a SPEC, Bolt, ADR, US, TC, BUG, MEM, or
     any AITL checkpoint. It has no AITL checkpoint, no manifest and no
     `status` (§3.15). Where it and a governed artifact disagree, the
     artifact wins: link the artifact, never state its approval as a fact here.
  ⚠️ OPENED BEFORE THE FIRST DELEGATION, on a human's launch: `launched_by`
     is required and is always a human:<user> actor. Claim `run` in INDEX.md.
  ⚠️ APPEND ONLY while the run is open: an earlier Timeline row is never
     edited — a correction is a new row naming the row it corrects by its When.
  ⚠️ FROZEN once the Outcome is written: no further edit, no further row.
  ⚠️ TIMESTAMPS from the system clock — the filename, `date` and every When;
     never invented.
  `run` is this run's sequence number. An agent run is neither an integration
  test run (§3.2.2) nor an entry of a manifest's `generation.runs[]` (§3.12).
-->

# ARUN-YYMMDD-HHmm — Run N: [short description]

## Mandate

<!-- The launching human's mandate, verbatim or faithfully quoted, with when it
     was given. The run does nothing beyond it (G40). -->

[the mandate]

## Standing rules

<!-- The constraints the human set for this run, one per bullet. A rule the
     human adds later is a Timeline row, never an edit here. -->

- [rule]

## Timeline

<!-- One row per step, appended in order and never edited. When: system
     clock, ISO 8601 with offset. Actor: human:<user> or agent:<id>.
     Artifact: a link to the artifact the step touched, or —. Result: what the
     step produced; a checkpoint decision lives in its artifact — link it. -->

| When | Actor | Action | Artifact | Result |
|------|-------|--------|----------|--------|
|      |       |        |          |        |

## Questions opened

<!-- Each OQ the run opened, as a link with its ID and title — never the
     question or answer text, which live only in the OQ file. -->

- [OQ-NNN — title](../analysis/open-questions/OQ-NNN-<description>.md)

## Artifacts & digests

<!-- Each artifact the run produced or changed, with its digest or state when
     the run closed. Its status and approvals are the artifact's own. -->

| Artifact | Change | Digest at close |
|----------|--------|-----------------|
|          |        |                 |

## Outcome

<!-- Written once, when the run closes: how it ended, what it left pending
     (checkpoints awaiting a decision, open questions) and when it closed
     (system clock). Writing this section closes the run and freezes the
     record. -->

[the outcome]
