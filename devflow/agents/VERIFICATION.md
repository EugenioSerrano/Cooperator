# Per-platform verification and usage notes

Status of the deployed role-agent wrappers per platform, re-verified
against current docs at implementation time. If a platform contract
changed since this note was written, re-derive the projected files —
wrappers and skills — from their canonical sources following the mapping
below and compare (see *Parity*).

## Canonical → wrapper mapping (field-level)

The wrappers project the canonical `agent.yaml` fields into each
platform's format. This table is the reference a MainAgent reads to
install or refresh a wrapper for its own platform. The charter body
(`prompt.md`) becomes the wrapper body.

| Canonical field | Claude Code | OpenCode | GitHub Copilot | OpenAI Codex |
|-----------------|-------------|----------|----------------|--------------|
| `id` — **the actor `id`**, never the definition's (see *Wrapper identity* below) | `name:` (frontmatter) | filename `<actor-id>.md` | `name:` (frontmatter) | `[agents.<actor-id>]` key |
| `description` | `description:` | `description:` | `description:` | `description = "..."` |
| `model` | `model:` | session default | `model:` | `model = "..."` |
| `capabilities.tools` | `tools:` (mapped Read/Grep/Glob/Bash/Edit/Write) | `permission:` (`read`/`edit`/`bash`/`write`: ask) | `tools:` (host-specific identifiers — VS Code vs Visual Studio 2026; see *Hosts*) | role config |
| `capabilities.mcp_servers` | `mcpServers:` (only when named) | permission globs | `mcp-servers:` (not in IDEs) | config layer |
| `approves` / `modes` | charter body | charter body | charter body | charter body (system prompt) |
| charter body | body after `---` | body after `---` | body after `---` | `[agents.<id>.prompt] system = """..."""` |
| `human_invocation` | **no control** — a human-started session exists (`--agent` / the `agent` setting; `@`-mention) and cannot be hidden; the section states the limit | `mode` + `hidden` — `mode: subagent` + `hidden: true` for `coordinator-only`; `mode: all` for `direct` | `user-invocable` — `false` for `coordinator-only`, omitted (default `true`) for `direct`; `disable-model-invocation` is **never** used (it suppresses agent-initiated invocation) | **no control documented** — the section states the limit |
| `skills` (references to `devflow/skills/<skill-name>/`) | `.claude/skills/<skill-name>/SKILL.md` (+ files; `user-invocable: false` on a `role` copy) **and** the wrapper's `skills:` frontmatter — a YAML list of the referenced skill names (preloaded at the subagent's startup) | `.opencode/skills/<skill-name>/SKILL.md` (+ files; the wrapper's `skill:` block denies, by name, the `role` skills the agent does not reference — never a wildcard rule) | `.github/skills/<skill-name>/SKILL.md` (+ files) | not confirmed — never-silent fallback (see *Skills projection*) |

### The permission-block derivation (deterministic)

A wrapper's permission set derives **only** from the definition's
`capabilities.tools` allowlist: each listed tool maps to its platform
permission per the table above, and **every tool absent from the allowlist
is explicitly denied** — nothing (`list`, `webfetch`, `websearch`, `task`
included) enters a wrapper without canonical backing. On OpenCode the tool
permission keys the wrapper writes are `read`, `grep`, `glob`, `list`,
`edit`, `write`, `bash`, `task`, `webfetch`, `websearch` (1.18.23); the
platform's own `question`, `plan_enter`, `plan_exit`, `doom_loop` and
`external_directory` defaults are left untouched; `skill` is governed by the
*Skills projection* section, never by this paragraph.
**The approver ceiling is not derived here.** It binds an agent **while it
acts as approver** — *"an agent acting as approver runs at capability tier
T0 (at most T1 with pinned, trusted sources), no write paths, no
transactional MCPs"* — and in v1 it governs as a **text rule** of the
approver ceiling (whose `capabilities` fields are deferred to the v2
hardening), not as a projection-time subtraction. A definition is therefore **never stripped of the tools its
`capabilities.tools` declares because it carries a non-empty `approves`**:
`approves` says what the actor may sign, not what its executor mode may
use. The one case where the ceiling does reach the wrapper is an
**approver-only** definition (`modes: [approver]`), which has no executor
mode to serve: it projects under the ceiling, denying `bash`, `edit`,
`write`, `task` and the web tools.

**Two projections of the same definition *and the same actor-instance* must
be byte-comparable.** Under N:1 two actors sharing one definition produce
**two** wrappers that differ by actor `id` and per-instance `model`
(`squad/README.md`), so they are not comparable byte-for-byte — and must
not be.

**Wrapper identity.** A wrapper belongs to **one actor-instance**, so it is
named by the **actor** `id` — not by the definition's. Under the N:1 reuse
this family advertises, naming by the definition would put two actors in one
file and collapse them into a single identity in the spawn folder; identity
is what approval independence is measured on (`approver.id ≠ executor.id`),
so that collapse would degrade the independence check itself
(`agents/squad/README.md`, *The lifecycle consistency contract*).

**`model: inherit`** is not a catalog value — at projection time the
wrapper **omits the model field entirely**, so each platform natively uses
its session/default model (deterministic and portable across the four).
Reviewer/approver-class agents should **pin a distinct model** from the
session's (model diversity — a model reviewing its own work is too
complacent).

**Spawn topology:** only the Coordinator (the platform agent itself) may
spawn role agents — Claude Code `Agent(...)` allowlist in the main
agent's preamble; OpenCode `permission.task`; Copilot the `agent` alias;
Codex instruction-based. The role wrappers carry no spawn capability
(`task: deny` / no `Agent` tool / no `agent` alias).

**Execution evidence (attribution integrity):** an executor's production
may be persisted by the Coordinator (the shape where an executor with
`write_paths: []` still produces — the Coordinator persists its output),
but the persistence act must trace to a **real spawn**: a stamp of another
actor's identity on content that actor never produced is a false claim,
not governed authorship. When spawn is unavailable, the checkpoint
resolves to the human (an operational degradation — the approved
configuration stands), and a human-started reviewer session is legitimate
only under a definition declaring `direct`; the execution-evidence rule is
the compensating control. Evidence = the spawn trace / the separate
session / the commit history — never the `author:` field alone.

## How the agents are installed and used

- The kit ships the **example definitions and the templates**
  (`devflow/agents/examples/`, `agents/TEMPLATE-new-role/`) and this
  **mapping**; it does **not** ship pre-built role wrappers. The project's
  **live definitions live in `devflow/agents/squad/`**. The
  **Coordinator** — the AvengaDevFlow MainAgent itself, one per tool —
  **installs and refreshes its platform's wrappers — and the referenced
  skills, once per skill —** into the project's platform folders
  (`.claude/agents/`, `.opencode/agents/`, `.github/agents/`,
  `.codex/agents/` for wrappers; the skills surface per the *Skills
  projection* section) by projecting the live definition from `squad/` and
  the skills it references from `devflow/skills/` per the mapping above (the
  agent lifecycle, a governed operational act).
- Each platform loads its agent files from those folders **at session
  start** — after an install (or a wrapper change), start a fresh session
  so the new agents register.
- The role agents are spawned by the **Coordinator** — the Avenga DevFlow
  platform agent itself, evolved: the shared methodology body and each
  platform preamble declare the orchestrator identity (routes, delegates
  production, spawns approvers, records — **never signs**; the spawn
  topology per platform).
- **Install-act report (guidance, never a gate):** installing a wrapper whose
  definition declares `direct` while an actor referencing that definition has
  `approver` in `modes` is **reported** and the install **proceeds** — the
  default combination (`coordinator-only`) needs no report; no `G##` is
  created.

## Skills projection

Skills are projected **by reference**: the wrapper goes to the spawn folder,
and **the skills the installed agents reference — plus every `shared` skill
under `devflow/skills/<skill-name>/` — go to this platform's skills surface,
once per skill**: `<surface>/<skill-name>/SKILL.md` plus its `references/` and
`assets/` copied as they are from the canonical
`devflow/skills/<skill-name>/`. **Nothing under `devflow/skills/examples/` is
ever projected** — the shelf of shipped example skills is inert by location,
whatever a skill's scope, until a team adopts it by copying the whole folder to
`devflow/skills/<skill-name>/` (the family contract in
`devflow/skills/README.md`). A skill already
projected by a previous install is verified identical — drift fails the act; it
never overwrites silently. Every projected `SKILL.md` carries, as the **first
line after its frontmatter** (the frontmatter must open the file for the
platforms to parse it), a **provenance marker** naming its canonical source
— and every projected wrapper carries the same marker in the same place,
naming its definition (`<!-- projected from devflow/agents/squad/<id>/ —
generated, do not edit; fix the definition and reinstall -->`):

```
<!-- projected from devflow/skills/<skill-name>/ — generated, do not edit; fix the skill and reinstall -->
```

Projected skill files are generated artifacts: the Coordinator's re-derivation
check covers them exactly like wrappers (see *Parity*), and the marker is its
anchor.

| Platform | Skills surface | Projection shape | Caveats | Verified |
|----------|----------------|------------------|---------|----------|
| Claude Code | `.claude/skills/<skill-name>/SKILL.md` — project-level, loaded from the start directory and its parents up to the repository root (personal: `~/.claude/skills/`) | `SKILL.md` with frontmatter `name` (defaults to the folder name) + `description`; supporting files (`references/`, `assets/`, scripts) referenced from `SKILL.md` by relative path; the body loads only when the skill is used | The platform's frontmatter can also carry **capability-granting fields** (`allowed-tools`, `disallowed-tools`, `model`, `effort`, `hooks`, `context`, `agent`, `background`, `paths`, `shell`) — the projection **never emits them** (content, never authority). The only frontmatter the projection **adds** is the scope flag: a `role` skill is projected with `user-invocable: false`, a `shared` skill with nothing; `disable-model-invocation` is **never** used — the platform documents that it removes the skill from Claude's context and prevents subagent preloading. The referencing agents' wrappers list the skill in their `skills` field, so it is preloaded at their startup (see *Scope → invocation controls*). Install smoke: verified live 2026-08-28 on Claude Code 2.1.223 (headless) and 2.1.250 (interactive): a spawned subagent reported the preloaded role skill's content without invoking it, the skill was hidden from the / menu, and the main agent could still reach it through the Skill tool (role privacy best-effort, as documented). The skill listing (names + descriptions) is loaded into context, so keep descriptions short. A project skill named like a bundled one (`debug`, `code-review`, …) shadows it silently — see *Naming* | 2026-08-28 — https://code.claude.com/docs/en/skills · https://code.claude.com/docs/en/sub-agents |
| OpenCode | `.opencode/skills/<skill-name>/SKILL.md` (primary; `.claude/skills/` and `.agents/skills/` are also read), searched upward to the git worktree | `SKILL.md` frontmatter: `name` (1–64 chars, lowercase alphanumeric + hyphens), `description` (1–1024 chars); optional `license`, `compatibility`, `metadata` | Agents load skills through the native `skill` tool under **pattern-based permissions** (`allow` / `deny` / `ask`, in `opencode.json` or the agent frontmatter) — the projected wrapper's `skill:` block **denies by name** the `role` skills the agent does not reference and nothing else — no allow list, **never a wildcard rule**: an install smoke (2026-08-28, 1.18.25) showed that a block with `"*": deny` removes the skill tool from the subagent altogether, while the by-name shape keeps the tool, loads the referenced and `shared` skills and refuses the denied one with the platform's rule message | 2026-08-28 — https://opencode.ai/docs/skills/ |
| GitHub Copilot | `.github/skills/<skill-name>/SKILL.md` (repository-level; `.claude/skills/` and `.agents/skills/` are also recognized; personal: `~/.copilot/skills/`, `~/.agents/skills/`) | Folders of instructions, scripts and resources per the Agent Skills open standard (`SKILL.md` with `name` + `description`) | Supported by the cloud agent, code review, the CLI, the app and agent mode in VS Code / JetBrains — **re-verify per environment**; **no per-skill invocation control is documented**, so a `role` skill is visible to every surface (the platform's limit, recorded here); the platform's 30k-character prompt cap applies to the cloud agent, so a large skill set favors the skills surface over inlining | 2026-08-28 — https://docs.github.com/en/copilot/concepts/agents/about-agent-skills |
| OpenAI Codex | **not confirmed** — the official documentation could not be verified from its published domain (the site redirects to a domain outside the set this note is allowed to consult) | — | This kit's own Codex platform file already lives at `.agents/skills/avenga-devflow/SKILL.md` (the open-standard location the Copilot and OpenCode docs also recognize), which is the natural candidate surface — but until it is confirmed from Codex's own documentation the **never-silent fallback** applies | 2026-08-28 — https://developers.openai.com/codex (308 redirect; its target https://learn.chatgpt.com/docs was read once with authorization on 2026-08-28: OpenAI's general ChatGPT / developer documentation landing page, no Codex skills surface documented — surface not confirmed) |

**Before projecting, the Coordinator checks reference symmetry:** every name
in `skills:` exists in `devflow/skills/<skill-name>/` — a definition that
references a missing skill is **refused**, naming the missing skill; a name
found only on the shelf (`devflow/skills/examples/<skill-name>/`) is missing
too, and the refusal says *adopt it first: copy `skills/examples/<name>/` to
`skills/<name>/`*. Installing the wrapper while leaving a referenced skill
unprojected is an **incomplete install: the act fails**.

**Scope → invocation controls.** A skill's `metadata.scope` (`shared` |
`role`, default `role` — the skill contract in `devflow/skills/README.md`)
decides what the projected copy adds — additions that only ever **narrow**:

| Scope | Claude Code | OpenCode | GitHub Copilot | OpenAI Codex |
|-------|-------------|----------|----------------|--------------|
| `shared` — a procedure any actor may run | projected as is: model- and user-invocable | no rule needed (platform default `allow`) | visible to every surface | fallback |
| `role` (default) — background knowledge of the agents that reference it | `user-invocable: false` on the projected copy **+** the skill listed in the referencing wrappers' `skills` field (preloaded at startup) | by-name `deny` in the wrappers of the agents that do not reference it; never a wildcard | no documented control — visible to every surface (the row above says so) | fallback |

The flag is a **projection-time** addition to the projected copy — never
part of the canonical `SKILL.md`. **Role privacy is best-effort per
platform:** on Claude Code a `role` skill is hidden from users but loadable by
any agent in the session — `disable-model-invocation: true` would hide it from
Claude too (the platform documents that it removes the skill from Claude's
context and prevents subagent preloading), so it is never used; the agents
that reference the skill receive it through their wrapper's `skills` field,
preloaded at startup. **Scope governs which agents get a skill preloaded or invocable; it never hides the canonical file** — any agent with read access to the project can read `devflow/skills/` (observed 2026-08-28: an OpenCode subagent without the skill tool read the three canonical files with `read`). `role` is an organizing and context-budget scope, not a confidentiality boundary. Install smoke: verified live 2026-08-28 on Claude Code 2.1.223 (headless) and 2.1.250 (interactive): a spawned subagent reported the preloaded role skill's content without invoking it, the skill was hidden from the / menu, and the main agent could still reach it through the Skill tool (role privacy best-effort, as documented). The invariant that never bends
is *never silent* (below).

**Naming.** One skill = one name project-wide — the folder name — with no
per-agent prefix. A collision between a project skill and a platform-provided
skill of the same name is **reported at install**; the platform's precedence
rules decide, and the row's caveats record them once documented. Observed on Claude Code (2.1.223 / 2.1.250, 2026-08-28): a project skill whose name matches a bundled skill **shadows** it in the listing and the `/` routing with **no platform notice** — so the install act compares every projected name against the platform's bundled skill names (Claude Code lists them in `/skills`) and reports a match before projecting.

**What a projected `SKILL.md` may carry:** the provenance marker, `name`,
`description`, the optional descriptive fields of the open standard
(`license`, `compatibility`, `metadata` — `scope` included) and the
projection-time scope flags above — **nothing that grants**: no
`allowed-tools`, `disallowed-tools`, `model`, `effort`, `hooks`,
`context`/`agent`/`background` (subagent forks), `paths`, `shell` or any
platform field that changes tools, permissions, model, execution or spawn. A
canonical `SKILL.md` that carries such a field is refused at install exactly
like a definition referencing a missing skill.

**The never-silent fallback.** On a platform with **no usable native skills
surface**, the Coordinator resolves the install in this order:

1. **Inline with warning** — fold the skills into the wrapper body only if the
   result stays under that platform's documented size cap; the install output
   states what was inlined and why.
2. **Degraded install with explicit notice** — install the wrapper without
   the skills **and record the degradation** (install output + this
   platform's row above and its section below), so the human knows this
   platform runs the agent without its skills.

**Prohibited:** installing the wrapper alone and saying nothing. An agent that
references skills that were never installed is the failure mode this section
exists to exclude.

**Skills are content, never authority.** No projected skill grants a tool,
an MCP server, a tier, spawn capability or approver mode — authority stays in
the structured fields the wrapper projects; the approver ceiling is unchanged.
Skills the kit ships in its `skills/` family name no MCP servers, external
tools or vendor schemas.

## Claude Code — full native coverage ✅

- **Folder:** `.claude/agents/*.md` — loaded from the project root at
  session start (the workspace trust dialog must be accepted for
  repo-level agents; a headless `claude -p` started **in the project's own
  folder** registers them too — verified 2026-08-28 on 2.1.223).
- Per-agent `model`, `tools` allowlist, `mcpServers`, hooks and spawn
  allowlists: all supported.
- **Spawn topology:** only the Coordinator carries the `Agent` tool; the
  role wrappers omit it, so executors cannot spawn approvers — approver
  agents are spawnable only through the Coordinator (or by a human).
- **Human invocation:** no visibility control is documented for agents — a
  subagent definition can run as the main session (`--agent` or the `agent`
  setting; `initialPrompt` applies there) and is `@`-mentionable, so `direct`
  is the platform's behaviour and `coordinator-only` is **unrealizable** on
  this platform; the row states the limit. Verified 2026-09-11 —
  https://code.claude.com/docs/en/sub-agents.
- The MCP allowlist is encoded in the canonical fields the wrappers project
  (`capabilities.mcp_servers`). The approver ceiling (T0/T1) is **not**: in
  v1 it governs as a text rule of the approver ceiling, because the
  `capabilities` fields are deferred — see *The permission-block derivation*
  above.
- **Skills:** `.claude/skills/<skill-name>/SKILL.md` (project-level, walked up to the repository root), projected from `devflow/skills/` once per skill; a projected skill carries its descriptive frontmatter plus the scope flag only (`role`: `user-invocable: false`, preloaded by the agents that reference it through their wrapper's `skills` field; `shared`: none; `disable-model-invocation` never — it removes the skill from Claude's context and prevents subagent preloading) — the platform's capability-granting frontmatter (`allowed-tools`, `disallowed-tools`, `model`, `effort`, `hooks`, `context`, `agent`, `background`, `paths`, `shell`) is never emitted. Verified 2026-08-28 against https://code.claude.com/docs/en/skills and https://code.claude.com/docs/en/sub-agents. Install smoke: verified live 2026-08-28 on Claude Code 2.1.223 (headless) and 2.1.250 (interactive): a spawned subagent reported the preloaded role skill's content without invoking it, the skill was hidden from the / menu, and the main agent could still reach it through the Skill tool (role privacy best-effort, as documented).

## OpenCode — solid, with the task gate ✅

- **Folder:** `.opencode/agents/*.md` — loaded from the project at
  session start.
- `permission` globs per tool supported.
- **Spawn topology:** `permission.task` — the Coordinator (the main
  agent) keeps it; every role wrapper carries `task: deny` (the task tool
  disappears from its description), so executors cannot spawn.
- **Human invocation:** `mode` + `hidden`. `coordinator-only` projects as
  `mode: subagent` + `hidden: true` — out of the `@` autocomplete menu while
  the Coordinator can still task it; `direct` projects as `mode: all`
  (selectable with Tab and taskable). The current docs define three mode
  values — `primary`, `subagent`, `all` (the default) — and `hidden` applies
  only to `mode: subagent`; hiding is menu-level (a user can always invoke a
  subagent by `@`), so it is best-effort. Verified 2026-09-11 —
  https://opencode.ai/docs/agents/.
- **Picker behavior:** the Tab picker lists **primary agents only** —
  subagents (`mode: subagent`) are visible via ctrl+X / `opencode agent
  list` and are invoked through the Coordinator's task tool (the spawn
  topology working as designed). A session reload registers new agents.
- **Re-verification finding (2026-08-23, OpenCode 1.18.21 → re-verified
  2026-08-28, 1.18.23 and 1.18.25):** on 1.18.21 the custom agent files were **not
  registered** in headless sessions (`opencode run` / `opencode serve`); on
  **1.18.23+ they are** (`opencode agent list` and the Coordinator's task
  tool see `.opencode/agents/*.md` headless). Two headless facts to plan
  around: `opencode run --agent <name>` accepts **primary** agents only (a
  subagent name falls back to the default agent with a warning — probe
  subagents through the Coordinator's task tool), and any `ask` permission
  of the Coordinator is **auto-rejected** headless. The spawn mechanics work
  (task spawn → produce → control return).
- No native audit hooks — noted, not blocking.
- **Skills:** `.opencode/skills/<skill-name>/SKILL.md` (also reads `.claude/skills/` and `.agents/skills/`), projected from `devflow/skills/` once per skill; agents load them through the native `skill` tool under pattern permissions — the projected wrapper denies by name the `role` skills its definition does not reference and carries no other skill rule (a wildcard `deny` removes the skill tool — observed 2026-08-28). Verified 2026-08-28 against https://opencode.ai/docs/skills/; install smoke 2026-08-28 on 1.18.25.

## GitHub Copilot — viable with environment caveats 🟡

- **Folder:** `.github/agents/*.agent.md` — repo-level custom agents.
- **Spawn topology:** the Coordinator declares **no `tools:` list**, so each
  host applies its full default tool set — which includes the agent→agent
  invocation (`agent` on VS Code; verified 2026-08-29 with the field
  omitted — the tools panel offered the repository's other custom agents as
  delegation targets). The role wrappers omit the invocation tool by construction —
  executors cannot invoke approvers. A Coordinator that *restricts* `tools:`
  must keep the invocation tool: without it, delegated work is silently
  self-executed (verified 2026-08-25: a Coordinator whose restrictive list
  lacked the tool ran the delegated review itself and stamped the reviewer's
  identity on the result).
- **Hosts:** the file is read by **VS Code** (verified 2026-08-25 and
  2026-08-29) and by **Visual Studio 2026** (verified 2026-08-29 on Insiders
  build 12120.281; custom agents require VS 2026 18.4 or later). Their
  **tool vocabularies differ** — VS Code: `search/codebase`, `edit/editFiles`,
  `execute/runInTerminal`, `web/fetch`, `vscode/*`, …; Visual Studio 2026:
  `code_search`, `readfile`, `editfiles`, `find_references`,
  `runcommandinterminal`, `getwebpages`, `find_symbol` (per the Visual Studio
  documentation read on 2026-08-29: "Tool names vary across GitHub Copilot
  platforms"; with `tools` unspecified "all available tools are enabled") — so
  a `tools:` list written for one host is flagged *"required by this agent are
  unavailable"* by the other. The shipped Coordinator therefore declares no
  `tools:` list (host default). Known Visual Studio 2026 issue (18.6.0/18.6.1,
  documented): tool groups may start disabled or reset between turns even with
  no `tools` property — re-enable them in the **Tools** panel. **Role
  wrappers** projected for Copilot carry `tools:` mapped from
  `capabilities.tools` in the vocabulary of the host the team installs on — a
  team using both hosts installs per host or accepts the banner on the other;
  the Coordinator states which vocabulary it used in the wrapper's provenance
  marker line.
- **Wrapper visibility / human invocation:** role wrappers are
  **user-invocable by default**; the projection emits `user-invocable:
  false` for a definition declaring `coordinator-only` and omits the field
  (default `true`) for `direct`. `disable-model-invocation` is **never**
  emitted: it suppresses *agent-initiated* invocation (VS Code: prevents
  other agents from invoking it as a subagent; the cloud agent: forces
  manual selection) — using it would break the Coordinator's spawn, which
  this field must never touch. Direct human invocation of a role agent
  remains legitimate — it is the explicit `direct` opt-in. **Dual-install
  caveat:** VS Code also loads agent files from `.claude/agents/` (the
  Claude format carries no visibility flag), so with a dual installation
  `coordinator-only` is best-effort on that surface. Verified 2026-09-11 —
  https://docs.github.com/en/copilot/reference/custom-agents-configuration ·
  https://code.visualstudio.com/docs/copilot/customization/custom-agents.
- **Post-edit validation:** validate the roster after an agent edit — the
  edit tool may write TAB indentation that strict YAML parsers reject
  (observed 2026-08-25: an agent-written roster failed PyYAML scanning
  while rendering fine in the IDE).
- Caveats (re-verify at implementation): `model` and `mcp-servers` are
  environment-dependent (VS Code / JetBrains / Eclipse / Xcode) and
  `mcp-servers` is not honored in IDEs; the 30,000-char prompt cap
  documented by GitHub applies to the **cloud agent** — the full agent
  body (≈68.9k chars; the whole file ≈69.6k) verified loading in VS Code
  (2026-08-25); re-verify per environment.
- **Skills:** `.github/skills/<skill-name>/SKILL.md` (repository-level; `.claude/skills/` and `.agents/skills/` also recognized), projected from `devflow/skills/` once per skill — supported by the cloud agent, code review, the CLI, the app and agent mode in VS Code / JetBrains; no per-skill invocation control is documented, so `role` skills are visible to every surface; re-verify per environment, and prefer the skills surface over inlining under the cloud agent's 30k cap. Verified 2026-08-28 against https://docs.github.com/en/copilot/concepts/agents/about-agent-skills.

## OpenAI Codex — right primitives, known gaps 🟡

- **Folder:** `.codex/agents/*.toml` — role files with full config
  override (model, reasoning, sandbox).
- **Spawn topology:** no native per-agent spawn allowlist — the control
  is instruction-based (the Coordinator's charter: "approver agents are
  reachable only through me") + role `config_file`. The weakest platform
  control; re-verify when the platform adds allowlists.
- **Human invocation:** no invocation control is documented — custom agents
  (`.codex/agents/*.toml`) are spawned by Codex, and a human inspects and
  steers the threads; `coordinator-only` is **unrealizable** there and the
  row states the limit. Verified 2026-09-11 —
  https://learn.chatgpt.com/codex/agent-configuration/subagents ·
  https://learn.chatgpt.com/docs/config-file/config-reference.
- Known invocation gaps (open issues #14579/#15250): repo-local custom
  agents may not be invocable by name from tool-backed sessions — the
  documented fallback is the generic `agent_type` + explicit overrides.
- **Skills:** no native skills surface confirmed — the official documentation could not be verified from its published domain; the never-silent fallback applies to every referenced skill (inline with warning under the documented cap, else degraded with notice). Candidate surface once confirmed: `.agents/skills/<skill-name>/SKILL.md`, where this kit's own Codex platform file already lives. Verified 2026-08-28 against https://developers.openai.com/codex (308 redirect; the target https://learn.chatgpt.com/docs read once with authorization: OpenAI's general ChatGPT / developer documentation landing page — no Codex skills surface documented; surface not confirmed).

## Parity

The drift safety net is the Coordinator's own check — no tool is required:
**after any install or refresh, re-derive each projected file — wrappers and
projected skills — from its canonical source (`devflow/agents/squad/<id>/` for
a wrapper, `devflow/skills/<skill-name>/` for a skill) following the mapping
above — including the visibility flag the `human_invocation` row produces —
and compare it with the file in the platform folder.** Any difference is
drift: report it and fix the **canonical source**, never the projection; drift
in a projected skill file counts exactly like wrapper drift. **Never hand-edit
a wrapper or a projected skill file.** The per-platform install smokes exercise
the mapping on the real platforms and are the standing drift check.
