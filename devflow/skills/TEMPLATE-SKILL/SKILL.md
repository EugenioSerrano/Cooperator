---
name: <skill-name>            # = the folder name (kebab-case)
description: >               # short and specific — platforms read this to decide when to load the skill
  <what this skill does and when to use it>
# license: <SPDX id or text>  # optional
# compatibility: <notes>      # optional — environment requirements
metadata:
  scope: role                 # shared | role (default role) — see ../README.md
  # origin: <contributed from …>   # optional — for contributed skills
---

<!--
  TEMPLATE — copy this folder to skills/<skill-name>/ and fill it in.

  MAY CARRY: name, description, license, compatibility, metadata.
  MAY NOT CARRY (refused at install): allowed-tools, disallowed-tools,
  model, effort, hooks, context, agent, paths, or any platform field that
  changes tools, permissions, model or spawn. Skills are content, never
  authority.

  Reference every supporting file from this body by relative path so the
  reader knows what it holds and when to open it:
    references/<topic>.md  — detail loaded on demand
    assets/<file>          — templates or data the skill uses
-->

# <Skill title>

## When to use

<the situation this skill applies to — one paragraph>

## What you need

<inputs: the artifacts to read (e.g. the approved US and its ACs), the
context families in `devflow/analysis/`, the project conventions in the
project section of `AGENTS.md`>

## Steps

1. <step>
2. <step>
3. <step>

## What you produce

<the artifact(s) this skill helps produce — governed artifacts follow their
family's template and approval; working notes go to the agent's
`devflow/agents-data/<agent>/` area>

## References

- [references/<topic>.md](references/<topic>.md) — <when to read it>
