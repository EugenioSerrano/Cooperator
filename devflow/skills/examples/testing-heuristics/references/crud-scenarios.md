# CRUD Scenarios

Applies when the story describes managing an entity (create, read, update,
delete or deactivate).

## How to apply it

1. Identify which CRUD operations are explicitly in the story's scope (do not
   assume all four apply when only one is described).
2. For each operation in scope, generate valid create / read / update /
   delete cases and their invalid counterparts (duplicate data, non-existent
   entity, insufficient permissions).
3. Consider side effects between operations (e.g. what happens to related data
   when an entity is deleted or deactivated) only when the US or the analysis
   families mention them.
