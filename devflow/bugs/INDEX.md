# Bugs — Índice

Defectos confirmados con análisis de causa raíz, impacto y propuesta de corrección. Cada bug requiere intervención de código y se corrige siguiendo TDD estricto en dos fases (test skip → fix → test green).

---

## ✅ Vigentes

| ID | Documento | Severidad | Descripción |
|----|-----------|-----------|-------------|
| BUG-001 | [BUG-001-defecto-defaultlazyprovider-mapper-no-registrado.md](BUG-001-defecto-defaultlazyprovider-mapper-no-registrado.md) | 🔴 Crítica | Condición asimétrica entre DefaultLazyProvider.cs y Mapper.Auto.cs: el template exige `GenerateEntity` para registrar el Mapper en `_mappersCache` pero `Mapper.Auto.cs` no lo requiere para generarlo, produciendo Mappers inaccesibles vía `IMapperRepository.Get<T>()` y `NullReferenceException` en sistemas generados |

## ⛔ Resueltos (Archive)

| ID | Documento | Estado |
|----|-----------|--------|
| — | — | — |

---

**Última actualización:** Mayo 2026
