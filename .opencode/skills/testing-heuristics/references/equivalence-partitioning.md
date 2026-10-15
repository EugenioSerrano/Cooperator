# Equivalence Partitioning

Divide each relevant field or input into equivalence classes: valid and
invalid values the system should treat the same way.

## How to apply it

1. Identify every field or parameter with explicit or inferred validation
   rules (data type, format, range, allowed values).
2. For each field define at least:
   - one representative valid class;
   - one or more representative invalid classes (wrong format, wrong type,
     value outside the domain, empty or null where applicable).
3. Generate one scenario per representative class — not one per possible value
   inside the class.
4. If a field has no documented or inferable rules, do not invent ranges —
   record it as an open question in the story analysis instead of assuming
   limits here.
