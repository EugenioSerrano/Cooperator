# Boundary Value Analysis

Complements Equivalence Partitioning by focusing on the limits of each
equivalence class, where defects are most likely.

## How to apply it

1. For every field with explicit or inferred limits (minimum, maximum, length,
   number of attempts, amounts, dates), generate cases at:
   - the exact boundary value (minimum and maximum);
   - a value just below the boundary;
   - a value just above the boundary.
2. If the story mentions counters or caps (e.g. "at most 3 requests per
   month"), apply the analysis to the counter: 2, 3 and 4 uses.
3. Do not invent numeric limits that are not in the US, the ACs or the
   analysis families; if a limit is undefined, record an open question in the
   story analysis instead of assuming a value.
