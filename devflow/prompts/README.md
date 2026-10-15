# Prompts

**Methodology version:** 5.2

## Purpose

This folder holds the **project's prompts**: the prompts the team creates,
modifies and improves to use with their AI agents. Each prompt is a file
whose body is **copied and pasted into the agent as-is**.

Prompts are **living data**, not governed artifacts:

- **No approval.** No AITL checkpoint applies to a prompt file.
- **No manifest.** Prompts are not part of the manifest family.
- **Versioned by git.** The folder is committed like any other project file;
  the repository's history is the prompt version control.
- **Shared with the team.** Everyone works from the same folder.

Project prompts belong here — **never** scattered in `agents-data/` or in
temporary locations.

---

## Usage

1. Create a new prompt file named `PROMPT-NNN-<description>.md` — the next
   sequential `NNN` is claimed in [INDEX.md](INDEX.md) (§5.15 convention).
2. Use [TEMPLATE-PROMPT.md](TEMPLATE-PROMPT.md): a title, an optional
   one-line description, and the prompt body.
3. Copy the **body** of the prompt and paste it into the agent.
4. When the team asks the agent to modify or improve a prompt, the updated
   version lands back in this same folder — versioned, ordered, shared.

> **Language policy (§3.15):** prompt bodies follow the project's
> `content_language`; file names and the `PROMPT-NNN` prefix stay in the
> schema's English.

---

## Index

See **[INDEX.md](INDEX.md)** for the prompt list.

---

## Relations to other folders

| Folder | Relation |
|---------|----------|
| `agents-data/` | Prompts are never stored there — `devflow/prompts/` is their home (contrast rule in §5.12) |
| `analysis/` | Prompt content may reference analysis artifacts, but prompts are not governed input |
