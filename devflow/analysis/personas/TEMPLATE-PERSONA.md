---
persona: ""                # Real name (real type) OR role identifier (archetype type), PascalCase
label: ""                  # Human-readable description of this persona/archetype
persona_type: "archetype"  # "real" = grounded in actual interview data | "archetype" = role-based (no real person behind it)
real_name: ""              # Only for persona_type: real — the actual person's full name
age_range: ""              # Only for persona_type: real — leave empty for archetypes
role: ""                   # e.g. "End customer", "Sales rep", "Field technician", "Primary caregiver"
date: "YYYY-MM-DD"
author: ""                 # human:<user> — the accountable human, never an agent (§3.0)
agent: ""                  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: ""                    # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "draft"            # draft | stable | deprecated
sources: []                # INT-NNN, documentation references — REQUIRED for real personas
tags: []
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs stay in English
  (the schema). Section
  headings (##) follow the project's content_language. All prose — persona
  descriptions, goals, pain points, quotes — goes in the project's
  content_language. See devflow/README.md -> Language policy.
  `AITL-*-Approval` codes are never translated.
-->

<!--
  ⚠️  CRITICAL RULE — READ BEFORE FILLING THIS TEMPLATE:
  NEVER invent a person that does not exist. Do NOT fabricate names, ages,
  occupations, or personal details. If no real interview or user research data
  is available, use persona_type: "archetype" and describe the role/archetype
  in abstract terms without fake personal attributes.
-->

# {{ persona }}

## 1. Type

<!-- Delete the option that does NOT apply -->

**Real persona** — This document is grounded in actual interviews or direct observation of a real person. Every personal detail (name, age, quote) has a verifiable source.

**Archetype** — This is a role-based composite representing a class of users. No real person with these exact attributes exists. The description captures patterns and needs common to this user segment without fabricating personal details.

## 2. Snapshot

> One paragraph describing this persona or archetype: who they are (or what role they represent), when they touch the product, what is on their mind.

| Field              | Value |
|--------------------|-------|
| **Type**           | {{ persona_type }} |
| **Age range**      | <!-- For real personas: e.g. 25-35. For archetypes: "Varies" or leave as N/A --> |
| **Role / context** | <!-- e.g. busy parent, field technician, casual buyer, system administrator --> |
| **Device / setup** | <!-- mobile, desktop, in-the-field, low bandwidth --> |
| **Digital literacy** | <!-- low / medium / high --> |

<!-- For archetype personas: remove or comment out age_range if not applicable -->

## 3. Goals

What this persona/archetype is trying to accomplish when interacting with the product.

- **Goal 1** —
- **Goal 2** —
- **Goal 3** —

## 4. Pain points

What frustrates them today (current system, current process, alternatives).

- **Pain 1** —
- **Pain 2** —
- **Pain 3** —

## 5. Context of use

- **When:** <!-- time / situation they use the product -->
- **Where:** <!-- environment - office, transit, store, home -->
- **How:** <!-- device, attention level, time available -->

## 6. Representative quote

<!--
  For real personas: use a VERBATIM quote from an interview. Include the source.
  For archetypes: use "—" or state "No direct quote available (archetype)."
  NEVER fabricate a quote.
-->

> *"[Verbatim quote from an interview, or '—' for archetypes]"*
> — Source: <!-- INT-NNN for real personas, or "N/A (archetype)" -->

## 7. What success looks like for them

<!-- The product wins with this persona/archetype when X happens. -->

## 8. Anti-patterns to avoid

<!-- Behaviours / UX choices that would alienate this persona/archetype. -->

## 9. Sources

<!--
  For real personas: REQUIRED. List every interview or document that grounds this persona.
  For archetypes: OPTIONAL. List any research, analytics, or domain knowledge used.
-->

| Source  | Where |
|---------|-------|
| <!-- INT-NNN --> | `../../input/interviews/INT-NNN.md` |

## 10. History

| Date | Change | Author |
|------|--------|--------|
| YYYY-MM-DD | Initial draft | @user |
