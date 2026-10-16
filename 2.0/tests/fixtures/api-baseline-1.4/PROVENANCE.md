# Procedencia -- linea base 1.4 del gate de compatibilidad de API

Generada por `2.0/build/frozen-1.4.ps1 -ProduceBaseline` (SPEC-261016-0000 seccion 6.C,
ADR-008 seccion 5).

| Campo | Valor |
|-------|-------|
| Commit de origen | `6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4` |
| Constructor | MSBuild (Visual Studio) `18.11.0.47511` |
| Configuracion | `Release` |
| Comando | `MSBuild 1.4/Framework.sln /t:Rebuild` con las propiedades deterministas fijadas y `/p:OutputPath=` redirigido fuera de `1.4/**` |
| Via usada | respaldo previsto por ADR-008 seccion 5 y SPEC 6.C.1: se usan los MISMOS ensamblados construidos por el build reproducible del 1.4 como artefacto de referencia (la propiedad de reference assembly no se honro de forma utilizable en los proyectos classic) |
| Alcance | `Cooperator.Framework.Core, Cooperator.Framework.Data, Cooperator.Framework.Library, Cooperator.Framework.Utility` |
| Excluidos | `CooperatorModeler.exe, ConnectionStringEncriptor.exe` -- registrados en el inventario de identidad (fase B), que es su uso real |
| Digests | `baseline-digests.json` (uno por archivo) |

## Por que estos cuatro y no los ejecutables

Los cuatro ensamblados del framework son **la superficie que la 2.0 reemplaza** y la
que las notas de migracion deberan cubrir miembro por miembro. `CooperatorModeler.exe`
y `ConnectionStringEncriptor.exe` se registran en el inventario de identidad
(fase B) y **no** entran al baseline de API.
