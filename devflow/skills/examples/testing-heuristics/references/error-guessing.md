# Error Guessing

An experience-based technique to anticipate common errors that do not follow
from explicit formal rules.

## How to apply it

1. Think of the typical errors of the kind of functionality described (e.g.
   double click / double submit, connection lost mid-operation, retries,
   timeouts, special characters in text fields).
2. Prioritize the ones plausible for the story's domain (payments, forms,
   stateful flows, …) — not a generic list disconnected from the context.
3. Keep it distinct from the generative exploration of the
   `exploratory-scenario-generation` skill: this is about common, well-known
   testing errors, not open creative reasoning about the specific domain.
