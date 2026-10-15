# Agents Data (Per-Agent Shared Knowledge)

**Methodology version:** 5.2

## Purpose

Per-agent shared knowledge areas under source control, visible to the whole
team. This folder exists so agents never improvise new folders inside
`devflow/` (G30): the canonical structure is fixed, and `agents-data/` is the
sanctioned free-form area.

There are **no pre-created subfolders**: each agent creates its own folder
on first use — `agents-data/<agent>/` — and is **responsible for
everything inside it**. Inside its own folder the agent is free to create
files and subfolders; that freedom is the point of the folder.

Everything here is **versioned with the repository and shared with the team**:
it holds durable, useful information — patterns, reusable knowledge,
environment notes, design decisions — that any team member or other agent can
read.

## How it works

- **First use:** create your folder `agents-data/<agent>/` the first
  time you need it. Creating it is sanctioned by G30 — any other new folder
  inside `devflow/` is not.
- **Folder name:** `<agent>` is the agent's own folder name. **The
  Coordinator's folder is named per tool, not by its actor id:** the four
  platform definitions write `agents-data/claude/`, `agents-data/codex/`,
  `agents-data/gh-copilot/` and `agents-data/open-code/`, because one
  repository may be worked from more than one tool and each tool keeps its
  durable notes apart. The Coordinator writes only in the folder of the tool
  it runs in, and every record still names it `agent:avenga-devflow`
  (§3.0.1): the folder names the tool's working area, never the actor.
- **Ownership:** each agent is responsible for its own folder — content,
  organization, and keeping it durable and team-useful. Do not create files
  in another agent's folder; if you need to share something, the user decides
  where it goes.
- **Scans:** agents use their own folder freely; they do not search other
  agents' folders proactively (token economy) — only when the user explicitly
  asks or references content there.

## What goes here

- Durable knowledge an agent wants to share with the team and persist in
  version control: patterns, conventions, learned behaviors, environment
  notes, reusable reference material.
- Anything that is useful beyond the session that produced it.

## What does NOT go here

- **Never temporary data.** Drafts, tool outputs, large intermediates, or any
  disposable content belong in the OS temp directory (W21) — never in a
  versioned, team-shared folder.
- **Never governed evidence:** nothing in `agents-data/` is governed input.
  It may never be cited as the source or justification of a SPEC, Bolt, ADR,
  US, TC, BUG, MEM, or any AITL checkpoint (G32). Treat it like
  `analysis/introduction/` (§5.5): narrative support, never evidence.
- **Not a replacement for `memory/`:** one MEM per V-Bounce remains mandatory
  (§2.12). Implementation memory is governed and immutable; `agents-data/`
  is informal shared knowledge.
- **Not personal memory:** preferences and session memory belong to each
  platform's **native memory mechanism** — the platform owns its location,
  which may change without notice; the methodology never dictates memory
  paths. `agents-data/` is committed, shared, and visible to the whole team.
- **Not a replacement for `_archive/`:** lifecycle-closed documents belong in
  each folder's `_archive/` subfolder (§5.4), not here.

## Rules

- **One area per agent:** each agent writes only inside its own
  `agents-data/<agent>/` folder, created by that agent on first use.
- **No other new folders:** creating a folder under `agents-data/` without
  being your own named area — or anywhere else inside `devflow/` — is a G30
  violation.
- **No AITL checkpoints:** nothing in this folder is approved, reviewed, or
  traceable; it has no `review:` contract and no lifecycle.
- **Shared and versioned:** content is committed like any other
  documentation; keep it durable, readable, and team-useful. Do not commit
  secrets, credentials, or environment-sensitive data.

---

## Language

IDs and any schema-like content stay in English. File name slugs and prose
follow the project's `content_language`, declared in
[`../LANGUAGE`](../LANGUAGE) (§3.15).
