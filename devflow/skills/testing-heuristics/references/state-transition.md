# State Transition Testing

Applies when the story's entity has defined states (e.g. an operation that
moves from Pending to Succeeded or Rejected).

## How to apply it

1. List the entity's explicit or inferred states and the valid transitions
   between them.
2. Generate one scenario per documented valid transition.
3. Also generate invalid-transition scenarios (e.g. trying to move from a
   terminal state to another state) when the US or the analysis families
   indicate that it should be blocked.
4. If the full state diagram is not documented, do not invent it — record the
   states that do appear and raise an open question about the missing ones.
