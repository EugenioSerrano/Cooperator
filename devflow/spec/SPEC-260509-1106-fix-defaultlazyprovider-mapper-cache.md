---
id: "SPEC-260509-1106"
titulo: "Corregir condición asimétrica en DefaultLazyProvider.cs que omite Mappers en _mappersCache"
fecha: "2026-05-09"
autor: "Eugenio Serrano"
llm: "DeepSeek V4"
estado: "pendiente"
origen: "BUG-001"
fa_asociado: ""
adrs_asociados: []
bolts: ["BOLT-001 — Alinear condición de registro de Mappers en DefaultLazyProvider con Mapper.Auto.cs"]
prerequisitos: "Ninguno. El proyecto no tiene test suite."
build_baseline: "Proyecto CooperatorModeler compila (WinForms .NET 4.8, 0 errores)"
riesgo_estimado: "Bajo"
proyectos_afectados:
  - "1.4/Tools/CooperatorModeler/Templates/CSharpClasses/DefaultLazyProvider.cs"
  - "1.4/Tools/CooperatorModeler/Templates/VisualBasicClasses/DefaultLazyProvider.cs"
---

<!--
⚠️ TIMESTAMP: 260509-1106 (9 de mayo 2026, 11:06)
⚠️ LLM: DeepSeek V4
-->

# SPEC-260509-1106 — Corregir condición asimétrica en DefaultLazyProvider.cs que omite Mappers en _mappersCache

| Campo                   | Valor |
|-------------------------|-------|
| **Origen**              | BUG-001 — Condición asimétrica en DefaultLazyProvider.cs omite Mappers en MappersCache causando NullReferenceException en sistemas generados |
| **ADRs Aplicables**     | Ninguno específico. No hay ADRs que regulen la generación de templates. |
| **Prerequisitos**       | Ninguno. El proyecto CooperatorModeler no tiene test suite automatizada. |
| **Proyectos Afectados** | `Templates/CSharpClasses/DefaultLazyProvider.cs`, `Templates/VisualBasicClasses/DefaultLazyProvider.cs` |
| **Riesgo Estimado**     | Bajo — cambio puntual en 2 líneas de templates, no modifica lógica de negocio ni contratos. |
| **Build Baseline**      | Proyecto compila correctamente en .NET Framework 4.8. |

---

## 1. Contexto y motivación

### 1.1 Origen

Este SPEC deriva del **[BUG-001](../bugs/BUG-001-defecto-defaultlazyprovider-mapper-no-registrado.md)** (a su vez originado por el **BUG-009** de producción reportado por el usuario). El análisis de causa raíz reveló que el template `DefaultLazyProvider.cs` utiliza una condición más restrictiva que `Mapper.Auto.cs` para decidir qué entidades merecen tener su Mapper registrado en el diccionario `_mappersCache`:

- **`Mapper.Auto.cs`** genera el archivo `{Entity}Mapper.Auto.cs` evaluando: `GenerateObject && (PrimaryKeyFields.Count != 0 || GenerateAsReadOnly)`.
- **`DefaultLazyProvider.cs`** registra el Mapper en `_mappersCache` evaluando: `GenerateObject && GenerateEntity && PrimaryKeyFields.Count != 0`.

Esta discrepancia hace que, para entidades con `GenerateEntity = false` pero `GenerateObject = true` y `PrimaryKeyFields.Count > 0`, el Mapper **se genere** pero **no se registre** en el cache. El resultado es una `NullReferenceException` en los sistemas generados cuando se intenta acceder al Mapper vía `IMapperRepository.Get<T>()`, ya que `DefaultRepository` busca en `MappersCache.Values` y no encuentra ninguna instancia que implemente la interfaz del Mapper.

### 1.2 Objetivo

Corregir los templates `DefaultLazyProvider.cs` (C#) y `DefaultLazyProvider.vb` (VB.NET) del Cooperator Modeler para que la condición que decide registrar un Mapper en `_mappersCache` sea **idéntica** a la que utiliza `Mapper.Auto.cs` para generar el Mapper. Esto garantiza que todo Mapper generado sea accesible a través del repositorio `IMapperRepository`, eliminando la raíz del BUG-009 y previniendo futuras instancias del mismo patrón.

El fix es aditivo (solo relaja condiciones, no las restringe), por lo que no hay riesgo de regresión: todos los Mappers que antes se registraban, siguen registrándose.

| Bolt | Descripción | Timebox |
|------|-------------|---------|
| BOLT-001 | Corregir condición en ambos templates de DefaultLazyProvider y verificar compilación | 15 min |

---

## 2. Alcance

### 2.1 Qué cubre

- ✅ Modificar la condición de registro de Mapper en `DefaultLazyProvider.cs` (C#) para que coincida con `Mapper.Auto.cs`.
- ✅ Modificar la condición de registro de Mapper en `DefaultLazyProvider.vb` (VB.NET) para que coincida con `Mapper.Auto.cs`.
- ✅ Verificar que el proyecto CooperatorModeler compila después del cambio.

### 2.2 Qué NO cubre

- ❌ No se agregan tests automatizados (no hay infraestructura de testing en CooperatorModeler).
- ❌ No se modifica la lógica de `Mapper.Auto.cs`, `Object.Auto.cs`, `Gateway.Auto.cs` ni `Entity.Auto.cs` (no tienen el bug).
- ❌ No se modifican sistemas generados existentes (el fix es en el generador, no en el código generado).
- ❌ No se regenera el código de ningún sistema cliente (eso es responsabilidad del usuario del Cooperator Modeler).

### 2.3 Impacto en proyectos

| Proyecto | Archivos nuevos | Archivos modificados | Tests nuevos |
|----------|----------------|---------------------|-------------|
| CooperatorModeler | 0 | 2 | 0 |

---

## 3. Stack tecnológico

| Componente | Tecnología | Versión | Notas |
|------------|------------|---------|-------|
| Cooperator Modeler | C# / VB.NET templates con sintaxis ASP-like (`<% %>`) | 1.4.5.0 | Templates compilados dinámicamente en runtime vía CodeDOM |
| Framework | .NET Framework 4.8 | 4.8 | Sin tests automatizados |
| Lenguaje de templates | C# incrustado en archivos `.cs` | — | Los templates son compilados por `CSharpCodeProvider` en runtime |

---

## 4. Implementación por fases

### Fase A — Corregir condiciones en DefaultLazyProvider (BOLT-001)

**Duración:** 15 min AI-time — **Complejidad:** Baja

#### A.1 Corregir `Templates/CSharpClasses/DefaultLazyProvider.cs`

**Archivo:** `1.4/Tools/CooperatorModeler/Templates/CSharpClasses/DefaultLazyProvider.cs`
**Línea a modificar:** 54
**Tipo de cambio:** Reemplazo de condición en bloque `if` dentro del loop `foreach (BaseTreeNode entityNode in Model.Children)`.

**ANTES (bug — requiere GenerateEntity, no contempla GenerateAsReadOnly):**
```csharp
if (currentEntity.GenerateObject && currentEntity.GenerateEntity && currentEntity.PrimaryKeyFields.Count != 0) {
```

**DESPUÉS (fix — idéntica a Mapper.Auto.cs línea 4):**
```csharp
if (currentEntity.GenerateObject && (currentEntity.PrimaryKeyFields.Count != 0 || currentEntity.GenerateAsReadOnly)) {
```

**Explicación del cambio:**
1. **Se elimina `currentEntity.GenerateEntity`**: `Mapper.Auto.cs` no exige `GenerateEntity` para generar el Mapper (línea 4: `if (currentEntity.GenerateObject && (currentEntity.PrimaryKeyFields.Count != 0 || currentEntity.GenerateAsReadOnly))`). El Mapper generado para entidades sin Entity opera sobre `{Entity}Object` y es completamente funcional. El `DefaultLazyProvider` debe registrar todo Mapper que `Mapper.Auto.cs` genere.
2. **Se agrega `|| currentEntity.GenerateAsReadOnly`**: `Mapper.Auto.cs` incluye esta cláusula para entidades sin PK marcadas como solo lectura. El `DefaultLazyProvider` debe reflejar el mismo alcance.

**Contexto en el template (líneas 52-59):**
```csharp
foreach (BaseTreeNode entityNode in Model.Children) {
    EntityNode currentEntity = (EntityNode)entityNode;
    if (currentEntity.GenerateObject && (currentEntity.PrimaryKeyFields.Count != 0 || currentEntity.GenerateAsReadOnly)) {%>
        DefaultLazyProvider._mappersCache.Add(
            "<%RulesProjectName%>.Entities.<%FullGenerateAs%>",
            <%RulesProjectName%>.Mappers.<%FullGenerateAs%>Mapper.Instance());
    <% } 
    if (currentEntity.GenerateObject && currentEntity.PrimaryKeyFields.Count != 0) {%>
        DefaultLazyProvider._mappersCache.Add(
            "<%RulesProjectName%>.Objects.<%FullGenerateAs%>Object",
            <%RulesProjectName%>.Gateways.<%FullGenerateAs%>Gateway.Instance());
<%}}%>
```

#### A.2 Corregir `Templates/VisualBasicClasses/DefaultLazyProvider.cs`

**Archivo:** `1.4/Tools/CooperatorModeler/Templates/VisualBasicClasses/DefaultLazyProvider.cs`
**Línea a modificar:** 47
**Tipo de cambio:** Reemplazo de condición en bloque `If` dentro del loop `For Each entityNode As BaseTreeNode In Model.Children`.

**ANTES (bug — requiere GenerateEntity, no contempla GenerateAsReadOnly):**
```vb
If currentEntity.GenerateObject AndAlso currentEntity.GenerateEntity AndAlso currentEntity.PrimaryKeyFields.Count <> 0 Then
```

**DESPUÉS (fix — idéntica a Mapper.Auto.cs línea 4):**
```vb
If currentEntity.GenerateObject AndAlso (currentEntity.PrimaryKeyFields.Count <> 0 OrElse currentEntity.GenerateAsReadOnly) Then
```

**Explicación del cambio:** Idéntica lógica que en A.1, adaptada a sintaxis VB.NET (`AndAlso` para short-circuit AND, `OrElse` para short-circuit OR, `<>` para desigualdad).

---

## 5. Criterios de aceptación (verificación manual)

Dado que CooperatorModeler no tiene test suite automatizada, la verificación es manual por parte del usuario (Eugenio Serrano).
La prueba se realiza de la misma forma en que el usuario ya usa la herramienta diariamente: conectándose a una base de datos, configurando el modelo, y generando código.

| AC | Descripción | Resultado esperado | Método de verificación |
|----|-------------|--------------------|-----------------------|
| AC-1 | **Mapper de entidad con `GenerateEntity=false` se registra en `_mappersCache`** | El `DefaultLazyProvider.cs` generado contiene una línea `_mappersCache.Add("...Entities.X", ...Mappers.XMapper.Instance())` para la entidad. | Destildar `GenerateEntity` en PropertyGrid para una entidad con PKs. Generar código. Abrir `DefaultLazyProvider.cs` y verificar que existe la entrada del Mapper. |
| AC-2 | **No hay regresión: Mapper de entidad con `GenerateEntity=true` sigue registrándose** | Las entidades normales (`GenerateEntity=true`, `GenerateObject=true`, PKs>0) siguen teniendo su Mapper en `_mappersCache`. | Repetir generación con una entidad normal. Verificar que el Mapper aparece en el `DefaultLazyProvider.cs` generado. |
| AC-3 | **No hay regresión: Gateway sigue registrándose** | Los Gateways de todas las entidades con `GenerateObject=true` y PKs>0 siguen registrados. | Verificar que las líneas de `_mappersCache.Add("...Objects...", ...Gateway.Instance())` siguen presentes para las mismas entidades de antes. |
| AC-4 | **Entidad ReadOnly sin PKs ahora registra el Mapper** | Una entidad con `GenerateAsReadOnly=true`, `GenerateObject=true`, `PKs=0` genera Mapper y se registra en `_mappersCache`. | Marcar `GenerateAsReadOnly=true` en PropertyGrid para una tabla/vista sin PK. Generar. Verificar. |
| AC-5 | **El proyecto CooperatorModeler compila** | `msbuild` o Visual Studio compila sin errores. | Abrir `CooperatorModeler.sln` y Build → Rebuild Solution. |

---

## 6. Checklist de completitud

- [x] Contexto explica POR QUÉ se hace (BUG-001 → BUG-009 → NullReferenceException en producción)
- [x] Alcance definido (solo 2 templates, 2 líneas, solo condición)
- [x] Stack tecnológico documentado (.NET 4.8, CodeDOM, templates ASP-like)
- [x] Fases de implementación detalladas con código antes/después y explicación
- [x] Cada fase menciona archivos concretos y líneas exactas
- [x] Criterios de aceptación con verificaciones manuales concretas (5 ACs)
- [x] Build baseline registrado (proyecto compila)
- [x] Un developer nuevo puede implementar esto sin preguntar nada
- [x] Nada requiere "inventar o decidir" — el fix es mecánico y verificable

---

## Referencias

- [BUG-001 — Análisis de causa raíz completo](../bugs/BUG-001-defecto-defaultlazyprovider-mapper-no-registrado.md)
- [BUG-009 (input) — NullReferenceException en producción](../input/BUG-009-NullRef-RecargoCobradoMapper-Cierre-Caja.md)
- [DISC-001 — Análisis del Cooperator Modeler](../discovery/DISC-001-analisis-cooperator-modeler.md)
- `Mapper.Auto.cs` línea 4 — condición de referencia: `if (currentEntity.GenerateObject && (currentEntity.PrimaryKeyFields.Count != 0 || currentEntity.GenerateAsReadOnly))`
- `DefaultLazyProvider.cs` línea 54 (C#) y 47 (VB) — líneas a modificar
