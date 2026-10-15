# Decision Tables

Useful when the story combines several independent conditions that determine
different outcomes (business rules with multiple conditionals).

## How to apply it

1. Identify the relevant conditions explicit in the US and ACs (e.g. "the
   customer's account is active", "the supplier is authorized", "fewer than 3
   requests this month").
2. Build a table with the relevant combinations of those conditions — not
   necessarily every possible combination when there are many; prioritize the
   combinations with real business value and the ones the story states
   explicitly.
3. Each row (combination) is a test scenario with its expected result per the
   business rule.
4. If the outcome of a combination is not defined in the US, the ACs or the
   analysis families, do not complete it by your own logic — it is an open
   question for the story analysis.
