---
id: "SPEC-261016-0035"
title: "Resolutor de valores protegidos `aesgcm:`: AEAD con AAD por clave de configuración, provisión externa de claves, rotación por `kid` y validación de esquemas"
date: "2026-10-16"
author: "human:eugenioserrano" # human:<user> — the accountable human, never an agent (§3.0)
agent: "agent:dotnet-developer-agent"  # agent:<id> of the agent that produced it, or null — never the delegating agent (§3.0)
llm: "deepseek/deepseek-flash" # exact model id reported by the tool (e.g. claude-opus-5-5), or null (§3.0)
status: "approved" # draft | approved | blocked | obsolete
origin: "REV-001" # US-NNN, TC-NNN, BUG-NNN, DISC-NNN, REV-NNN, AREV-NNN, or ADR-NNN that motivated this SPEC
bolt: "US-000.BOLT-022" # ⚠️ MANDATORY — US-NNN.BOLT-NNN | US-000.BOLT-NNN | TC-NNN.BOLT-NNN
revision: 2 # material revision of this canonical SPEC (matches manifest spec_revisions[])
associated_adrs: ["ADR-007", "ADR-004", "ADR-001"] # mirrors the Bolt's `sources`
prerequisites: # SPECs ya en disco que este Bolt necesita
  - "SPEC-261016-0000-arnes-tests-y-build-reproducible.md" # arnés, build compartido y comprobaciones de invariantes (Bolt 001)
  - "SPEC-261016-0018-configuracion-portable.md" # el contrato de configuración y la validación de la FORMA del prefijo; el seam `IProtectedValueResolver` (Bolt 011)
risk_class: "high" # low | medium | high | critical (mirrors the Bolt's risk_class)
autonomy_level: "L2" # L1 | L2 | L3 | L4 — defaults by risk: low/medium→L3, high→L2, critical→L1; L4 requires an ADR (§3.3)
turn_budget: "" # OPTIONAL — agent loops without a green hermetic suite before stop-and-ask (integer ≥ 1); leave empty to use the platform/agent default (§3.3)
data_classification: "confidential" # public | internal | confidential | restricted — ordered (public < internal < confidential < restricted),
                                # normative definition in §3.6; mirrors the Bolt's (que declara `confidential`); this SPEC-declared value is the one the PII/DLP gate reads
integration_test_plan: # the Bolt's integration test cycle (§3.2.2) — parseable; changes only through a SPEC revision; agrees with the Bolt's test_integration
  applicability: "n/a" # applicable | n/a | waived — required → applicable or waived (never waived for an integration-test-only BUG Bolt); not-required → n/a
  tests: []         # applicable — the integration tests a run executes, whoever wrote them: this V-Bounce, another Bolt or a shared suite (paths or suite names, §8.2)
  environment: ""   # applicable — where they run (the project's environment name)
  data: ""          # applicable — the data or fixtures they need
  reason: "Bolt `test_integration: not-required` (hardening): el control se verifica sobre el valor protegido dentro del proceso — no hay superficie desplegada, servicio externo ni entorno que lo valide."
  waiver_adr: ""    # waived — ADR-NNN of this project: reason, owner, compensating control, expiry (G21)
review_ready_at: "2026-10-16T03:15:52-03:00" # When this version is submitted for review (§3.0) — ISO 8601 with seconds + offset, e.g. 2026-08-02T11:45:00-03:00
review: # AITL-SPEC-Approval — filled by the human reviewer (§3.0)
  decision: "approved" # approved | changes_requested | rejected
  reviewers: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}]
  written_by: [{actor: "agent:dotnet-developer-reviewer-agent", role: "dev_validator", model: "deepseek/deepseek-v4-pro"}] # [{actor, role, model}] who wrote this record — the reviewer, or the agent that transcribed the decision; writing is not signing (§3.0)
  started_at: "2026-10-16T03:15:52-03:00" # ISO 8601 with seconds + offset
  decided_at: "2026-10-16T03:15:52-03:00" # ISO 8601 with seconds + offset
  findings:
    - "Compliant — re-aprobación de REVISIÓN 2 (material PREVENTIVA, G15/G16): misma SPEC canónica `SPEC-261016-0035`, `revision: 2`, `status` re-abierto a `draft` y re-aprobado acá. Fuentes gobernadas sin cambio material: Bolt `US-000.BOLT-022` (`ready`), ADR-007/ADR-004/ADR-001 `accepted`, REV-001 `approved`, baseline = HEAD 6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4. Cursada **antes** de que la trampa muerda (sin V-Bounce aún): es la **cuarta** instancia del mismo patrón."
    - "CONFIRMACIÓN (§6/E.3 — política de nulabilidad preventiva) — CORRECTA: espejo exacto de los tres precedentes `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y `SPEC-261016-0037` §B.2, citados con sus mediciones (`MEM-261016-0107` §8.2, `MEM-261016-0209` §8.1, `MEM-261016-0227` §13); mecanismo (`#nullable disable` primera línea útil, antes de los `using`, sólo en aportados **con diagnóstico** `CS8618`/`CS8603`/`CS8625`); criterio objetivo (*archivo aportado + diagnóstico*) con **el build como prueba**; límites duros (**jamás** código nuevo —el resolutor `aesgcm:` conserva la nulabilidad activa— y **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`); límite del `using` muerto ⇒ parar (§15)."
    - "CONFIRMACIÓN (alternativas descartadas + matiz criptográfico) — CORRECTA: **anotar/corregir** el aportado es una transformación no ordenada **y** en este Bolt un riesgo de seguridad («tocar el camino criptográfico heredado sin ADR»); **diferir** dejaría al Bolt sin la pieza que debe retirar. El matiz de seguridad añade rigor al descarte y es el adecuado para un Bolt `confidential`/`high`."
    - "CONFIRMACIÓN (coherencia §4/§7-AC-11/§8.1/§9/§13/§14/§15/§16/§17) — CORRECTA: in-scope; AC-11 (directiva sólo en aportados + build + «ningún `.csproj` con la etiqueta»); §8.1 verificación por lectura + build + `using` muerto; §9 gate de Build; §13 riesgo «directiva de más/de menos»; §14 fila ⚠; §15 condición de extensión; §16 DoD; §17 precedentes referenciados."
    - "CONFIRMACIÓN (§18/§19) — CORRECTA: el historial tiene las **dos filas** (rev.1/rev.2, **sin reemplazo**); la nota al aprobador (§19) declara que **sólo la prevención cambia** (3 puntos) y cita los tres precedentes ya aprobados."
    - "ADJUDICACIÓN — CONFIRMADA: la autorización preventiva ALCANZA —es el **cuarto espejo** del mismo patrón ya endosado tres veces (SPEC-0004 §A.3, SPEC-0013 §A.3, SPEC-0037 §B.2)— y **no habilita nada más**: el `using` muerto hacia namespace inexistente queda fuera (§15), la directiva está prohibida en código nuevo y en `.csproj`, y el contrato criptográfico y el material de clave no cambian. Suficiente y agéntico."
  acknowledged_without_comment: false # must be true when findings is empty (§3.0)
  acknowledgment_reason: "" # required when acknowledged_without_comment is true
---

<!--
  LANGUAGE POLICY (§3.15): YAML keys, status enums, IDs and section headings (##) stay in English (the schema);
  all prose goes in the project's content_language (devflow/LANGUAGE).

  ⚠️ PRE-SPEC EVIDENCE GATE (§2.4.1): corrida antes de escribir esta revisión — §3 registra cada fuente gobernada
     usada, su checkpoint de aprobación y la línea base del repositorio. Ninguna fuente quedó en borrador.
  ⚠️ REVISIÓN 2 — REVISIÓN MATERIAL (G15/G16) PREVENTIVA, cursada **antes** de que la trampa muerda. El V-Bounce de
     este Bolt aportará **código del 1.4** (el caso declarado: `Cryptography.cs` y lo que el aporte traiga) y el
     árbol 2.0 hereda del Bolt 001 `Nullable=enable` + `TreatWarningsAsErrors=true`: es exactamente el escenario que
     ya detuvo a **tres** V-Bounces de esta ola (003/007/009) y que obligó a una **revisión material por cada uno**
     para autorizar la directiva. Esta revisión **adelanta esa autorización** —`#nullable disable` como primera
     línea útil de cada archivo **aportado del 1.4** que **produzca diagnósticos** (`CS8618`/`CS8603`/`CS8625`),
     **jamás** en código nuevo, **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado +
     diagnóstico*) y **el build como prueba**—, espejo exacto de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y
     `SPEC-261016-0037` §B.2. Ajusta §1, §4, §6 (nota de mecánica + §E.3), §7 (AC-11 y su mapeo), §8.1, §9, §13,
     §15, §16, §17, §18 y §19.
     ⚠️ **Nada más cambia:** el objeto del Bolt, el mecanismo `aesgcm:`, la composición y los tamaños publicados, la
     provisión de claves, el cuadro de errores tipados, el retiro de la superficie estática, las fronteras con 011/
     023/024 y el control `confidential` siguen **en vigor** (el `review` se re-abre vacío: es una revisión
     material).
  ⚠️ AITL-SPEC-Approval: esta revisión es un DRAFT. Un V-Bounce o code-run no puede empezar hasta que un
     Dev-validator (+ dueños de dominio aplicables) registren la decisión en el bloque `review` de arriba.
     Un cambio material en una fuente gobernada o en la línea base invalida la aprobación: parar, revisar el
     mismo SPEC canónico y re-aprobar (G15).
  ⚠️ UN BOLT POR SPEC: todas las fases de este documento implementan `US-000.BOLT-022`. El seam del contrato y la
     validación de la FORMA del prefijo son del Bolt 011; la **migración** del corpus ya cifrado, la inspección y
     la **rotación** como operación son del Bolt 023; el shell delgado del encriptador es del Bolt 024; el
     **BUG de seguridad de F-23** es su propio Bolt (este Bolt es el estándar contra el que su fix se verifica).
  ⚠️ INTEGRATION TEST PLAN (§3.2.2): `n/a` porque el Bolt declaró `test_integration: not-required`; plan y Bolt
     coinciden y esa coincidencia es parte del gate de §3.
  ⚠️ CLASIFICACIÓN `confidential`: espeja la del Bolt (que declara `confidential`, no `internal`), y es coherente
     con la materia —material de claves y valores protegidos—. Esta es la clasificación que lee el gate PII/DLP.
  ⚠️ LA CAPACIDAD LEGACY NO ESTÁ ACÁ (ADR-007 §3.3/§3.4): el descifrado del formato de 1.4 vive **sólo** en un
     componente interno de la herramienta de conversión (Bolt 023), con bandera explícita y registro de retiro.
     Este Bolt **no** la habilita, **no** la reutiliza y **no** deja un camino de degradación a texto plano.
-->

# SPEC-261016-0035 — Resolutor de valores protegidos `aesgcm:`: AEAD con AAD por clave de configuración, provisión externa de claves, rotación por `kid` y validación de esquemas

| Field | Value |
|-------|-------|
| **Origin** | REV-001 (§6 ítem 12) |
| **Bolt** | [US-000.BOLT-022](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) |
| **ADRs** | [ADR-007](../adrs/ADR-007-politica-criptografica.md) · [ADR-004](../adrs/ADR-004-cache-y-configuracion.md) · [ADR-001](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) |
| **Risk Class** | high |
| **Revision** | 2 |

---

## 1. Objective

Se entrega el **resolutor de valores protegidos** del framework: la implementación del esquema **`aesgcm:`**
—**cifrado autenticado** con clave externa, **nonce aleatorio por operación**, **verificación de integridad
antes de cualquier uso** y **datos autenticados adicionales atados a la clave de configuración** que contiene el
valor—, la **provisión de claves** por una fuente **externa al producto** (variables de entorno por defecto, hook
del host para un almacén propio) con una clave **activa** y un conjunto de claves **de lectura**, la **validación
de esquemas** (`plain:` identidad; esquema desconocido o sin resolvedor registrado ⇒ **error tipado**, nunca
intento de descifrado a ciegas ni degradación a texto plano) y el **retiro de la superficie estática con clave
implícita**.

El problema que resuelve está documentado (REV-001 F-23 y §6 ítem 12): el cifrado de 1.4
(`Library/Cryptography.cs`) usa **clave literal embebida en el binario** (`_CryptoKey`, línea 44), **IV fijo**
(`_Iv`, línea 47), TripleDES con **clave derivada por MD5**, encoding **ASCII** y **sobrecargas estáticas sin
clave** (`Encrypt(string)`/`Decrypt(string)`, líneas 55/85) — ofuscación tratada como defecto de seguridad. La
consecuencia práctica es que el material de clave es **público** (está en el IL) y que un valor manipulado
produce basura silenciosa en lugar de un error.

Si no se implementa: el framework no tiene estándar criptográfico, el defecto de F-23 no tiene contra qué
verificarse y la migración del corpus ya cifrado (Bolt 023) no tiene destino.

**Autorización preventiva de la revisión 2 (la trampa conocida, adelantada).** Este Bolt **aporta código del 1.4**
(el caso declarado: `Cryptography.cs` y lo que el aporte traiga) y el árbol hereda `Nullable=enable` +
`TreatWarningsAsErrors` del Bolt 001: sin autorización explícita, el porteo de un archivo legacy produce
`CS8618`/`CS8603`/`CS8625`, el build los convierte en **errores** y el gate de Build no puede cerrar en `pass`.
Eso ya ocurrió **tres veces** en esta ola (Bolts 003, 007 y 009) y cada una necesitó su **revisión material**. Esta
revisión **autoriza de antemano** la respuesta ya probada —`#nullable disable` **por archivo aportado del 1.4** que
la requiera (§E.3)— y **nada más**, para que el V-Bounce de este Bolt no se detenga en una trampa conocida. El resto
del Bolt **no se reabre**.

---

## 2. Context

**De dónde viene la necesidad.** `REV-001` §6 ítem 12 ruteó F-23 (clave e IV hardcodeados) a un **ADR de política
criptográfica** más un **BUG de seguridad**; ADR-007 decidió el mecanismo y este Bolt entrega la **pieza
standard** —el resolutor y sus claves— **sin** corregir el defecto en sí (que es del BUG/Bolt dedicado) ni migrar
el corpus (Bolt 023).

**Qué quedó decidido aguas arriba** (no se reabre):

- **ADR-007 §3.1 (mecanismo, esquemas y superficie)** — los esquemas que el resolutor de ADR-004 §3.3 acepta son
  **`plain:`** (identidad) y **`aesgcm:<kid>:<base64>`** (mecanismo del framework), más los que el **host**
  registre por hook (`dpapi:`, `x509:`, `kv:`, …), con el patrón de **fijación única al arranque**; el mecanismo
  del framework es **`System.Security.Cryptography.AesGcm`** (confirmado en `netstandard2.1`), **AES-GCM de 256
  bits**, **nonce aleatorio por operación** (nunca reutilizado) y **tag verificado al resolver**; la carga se
  codifica en **Base64** con **`nonce | tag | ciphertext`** en ese orden, **documentado como contrato**, y los
  detalles que **este Bolt fija y publica** son: **nonce de 12 bytes**, **tag de 16 bytes** y **clave de entorno
  de 32 bytes codificada en Base64**; los **datos autenticados adicionales (AAD)** son el **nombre completo de la
  clave de configuración** que contiene el valor (así un valor **no** se puede mover de un slot a otro sin
  re-protegerlo — y re-targetear es operación de la herramienta de §3.5, no un copiar/pegar); un esquema **sin
  resolvedor registrado** es error tipado que dice exactamente eso; el **contrato** (`IProtectedValueResolver`:
  esquema + carga ⇒ valor, con `kid` disponible) vive en **`Cooperator.Framework.Core`** y la **implementación
  `aesgcm:`** en **`Cooperator.Framework.Library`** (BCL, portable); y **se retira** la API estática con clave
  implícita (`Cryptography.Encrypt(string)`/`Decrypt(string)`), de modo que **proteger y resolver exigen una
  referencia explícita de clave** — ruptura de superficie que va a las notas de migración —.
- **ADR-007 §3.2 (claves)** — **ninguna clave embebida** en ninguna forma (ni literal, ni derivada de una
  constante, ni en el repositorio, ni en el `.coop2`); **key providers**: (a) **por defecto, variables de
  entorno**, (b) **hook del host** para un almacén propio; el framework consume la **referencia** de la clave
  (`kid`), no el material; **`kid` explícito en el valor** con **dos roles** (una clave **activa** y un conjunto
  de claves **de lectura**), y un `kid` **sin clave disponible** ⇒ **error tipado** (nunca "probar con otra
  clave" ni degradar a texto plano); **rotación sin big-bang** con ventana de solapamiento **explícita en la
  configuración, con fecha**; y **nada de secretos en logs ni reportes** (los reportes identifican valores por
  **clave de configuración** y `kid`, nunca por contenido).
- **ADR-007 §3.4 (primitivas)** — permitidas: AEAD (AES-GCM 256), SHA-256 o superior, PBKDF2-HMAC-SHA-256 si
  alguna vez hiciera falta derivar, y el CSPRNG del runtime; **prohibidas**: MD5/SHA-1, DES/3DES/RC2, modo ECB,
  IV/nonce fijos o reutilizados, claves embebidas o derivadas de una constante, comparación de tags no constante
  y los tipos `*CryptoServiceProvider` concretos (se usan los factory). La **excepción única** (3DES+MD5 en el
  componente interno de la herramienta de migración) **no** la habilita este Bolt.
- **ADR-007 §5 (NFRs)** — *No embedded secrets* (**0** claves literales o derivadas de constantes en el árbol
  2.0; **0** secretos en configuración versionada o en el `.coop2`) y *Protected value integrity* (**100 %** de
  los casos de manipulación detectados con error tipado; **0** valores resueltos sin verificación de tag; **0**
  degradaciones silenciosas a texto plano), medidos con la batería de casos negativos y con la verificación
  explícita de que la clave del formato legacy **no reaparece en ninguna forma**.
- **ADR-004 §3.3 (forma del prefijo y quién invoca)** — el **prefijo de esquema es obligatorio**: un valor **sin**
  prefijo **no** se asume texto plano (**error tipado** que exige declarar `plain:`), un **prefijo desconocido**
  es error tipado que nombra la **clave** y el **prefijo ofensor**; y **quien invoca al resolutor es la capa que
  consume el valor** (`Data` para las cadenas, el tooling para el reverse), **no** la configuración.
- **ADR-001 §3.4/§3.5/§3.3** — dónde vive el contrato y dónde la implementación (capa base sin dependencias; la
  librería portable con BCL), **sin** dependencias de terceros ni paquete de plataforma; y el **gate por token
  retirado no vuelve por esta vía**: esto es **protección de valores**, no un control de integridad de despliegue.

**Qué decide este SPEC, dentro de esas restricciones.** El **punto de fijación/registro** de los resolvedores por
esquema (dónde vive y cómo se registra el del framework y los del host), la **familia de excepciones tipadas**
de la política, la **codificación concreta de la clave** en la provisión por entorno (**Base64**, como el ADR
delega), la **forma exacta de la operación de proteger** (que exige la referencia de clave) y la **frontera
precisa** entre la validación de la **forma** del prefijo (Bolt 011) y el error de **esquema sin resolvedor**
(este Bolt).

**Fronteras declaradas.** El **seam del contrato** (`IProtectedValueResolver` en `Core`) y la validación de la
**forma** del prefijo son del **Bolt 011** (se consumen); la **migración** de los valores ya cifrados de 1.4, la
**inspección**, la **rotación como operación** y las **herramientas** son del **Bolt 023** (con la capacidad
legacy **aislada y exceptuada**); el **shell delgado del encriptador** es del **Bolt 024**; la **corrección del
defecto** de F-23 es de su **BUG + Bolt** (este Bolt es el **estándar** contra el que ese fix se verifica,
ADR-007 §3.6); los **esquemas del host** (`dpapi:`, `x509:`, `kv:`) los aporta el host y el framework **no** los
implementa; y la **configuración** (catálogo de claves y precedencia) es del **ADR-004/Bolt 011**.

---

## 3. Source inventory and approval references

> Pre-SPEC evidence gate (§2.4.1) corrido el `2026-10-16` antes de escribir esta revisión. Resultado: **gate
> pasa** — ninguna fuente gobernada usada está en borrador, rechazada ni vencida, y el plan de integración
> coincide con el `test_integration` del Bolt.

| Source | Ref | Approval |
|--------|-----|----------|
| Bolt | [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — `status: ready`, `risk_class: high`, `data_classification: confidential`, `test_integration: not-required` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:09:32-03:00, agent:dotnet-architect-reviewer-agent) |
| Parent (container) | [`US-000`](../functional/user-stories/US-000-non-functional.md) | Contenedor permanente — sin ciclo de aprobación (§2.6) |
| Review finding | [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-23 (clave e IV hardcodeados, clave derivada por MD5, encoding ASCII: ofuscación como defecto de seguridad), F-24 (tipos `*CryptoServiceProvider` obsoletos) y §6 ítem 12 | `AITL-REV-Approval` ✓ (aprobado 2026-10-15) |
| ADR | [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (mecanismo, esquemas, composición `nonce \| tag \| ciphertext`, AAD por nombre de clave, esquemas del host, dónde vive el contrato y la implementación, retiro de la superficie estática), §3.2 (provisión y rotación de claves; nada de secretos en logs/reportes), §3.3 (migración legacy: **fuera** de este Bolt), §3.4 (primitivas permitidas y prohibidas; la excepción acotada que este Bolt no habilita), §3.6 (qué no cubre), §5 (NFRs *No embedded secrets* y *Protected value integrity*) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.3 (prefijo de esquema obligatorio, prohibición de la heurística, error tipado ante lo desconocido, y **quién** invoca al resolutor: la capa que consume el valor) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T23:11:40-03:00) |
| ADR | [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.4/§3.5 (dónde vive el contrato y la implementación; sin dependencias ni paquete de plataforma), §3.3 (el gate retirado no vuelve: protección de valores ≠ control de integridad de despliegue), §3.6 (criptografía como slot, y el canal de distribución pendiente) | `AITL-ADR-Approval` ✓ (`accepted`, 2026-10-15T22:32:47-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-011`](../functional/bolts/US-000.BOLT-011-configuracion-portable.md) — su SPEC canónica ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md)) entrega el contrato de configuración, el catálogo y la validación de la **forma** del prefijo; su seam `IProtectedValueResolver` vive en `Core` | `AITL-BOLT-READY-Approval` ✓ (2026-10-16T00:06:40-03:00) |
| Previous Bolt (dependency, no fuente gobernada) | [`US-000.BOLT-001`](../functional/bolts/US-000.BOLT-001-arnes-tests-y-build-reproducible.md) — arnés, build compartido y comprobaciones de invariantes | `AITL-BOLT-READY-Approval` ✓ (2026-10-15T23:47:42-03:00) |
| Repository baseline | `git` commit **`6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4`** (`6aa497f`, "devflow: ADR-004..009 accepted (routing complete, 9 ADRs) + run 3 record") | — |

**Contexto de código usado como evidencia** (árbol `1.4/` **congelado**, sólo lectura, sin editar una línea):
`1.4/Cooperator.Framework.Library/Cryptography.cs` — `public static class Cryptography` (línea 40) con
`private const string _CryptoKey = @"*1Am|9Kh)+/?sfGq88"` (línea 44), `private static readonly byte[] _Iv =
{ 241, 0, 34, 19, 23, 32, 44, 151 }` (línea 47), `Encrypt(string)`/`Decrypt(string)` que delegan en las
sobrecargas con clave (líneas 55/85), y esas sobrecargas con
`TripleDESCryptoServiceProvider` + `MD5CryptoServiceProvider` + `Encoding.ASCII` +
`des.IV = _Iv` (líneas 67-76 y 97-108) — es decir: **clave e IV en el binario**, **MD5** como derivación,
**3DES** y **ASCII**. Complementos: `1.4/Tools/ConnectionStringEncriptor/MainForm.cs` (el encriptador que usaba
esa superficie — su sucesor delgado es el Bolt 024) y `1.4/Cooperator.Framework.Data/BaseRule.cs:104-129` (la
heurística de detección por substrings, eliminada por ADR-004 §3.3 y no reintroducida acá).

**Coherencia del plan de integración:** el Bolt declara `test_integration: not-required` (§3.2.2) y este SPEC
declara `integration_test_plan.applicability: "n/a"` con su razón. **No hay desajuste** y por lo tanto no hay
reporte bloqueante.

---

## 4. Scope

### In scope

- La **implementación del esquema `aesgcm:`** en `Cooperator.Framework.Library`: AEAD de 256 bits, **nonce
  aleatorio por operación** (12 bytes), **tag de 16 bytes**, carga **Base64** con **`nonce | tag | ciphertext`**
  (contrato publicado), **AAD = nombre completo de la clave de configuración** que contiene el valor, y
  **verificación de integridad antes de cualquier uso**.
- La **operación de proteger** con **referencia explícita de clave** (nonce nuevo por operación) y la
  **operación de resolver** que valida esquema, `kid`, longitud de la carga y tag.
- La **provisión de claves**: fuente por defecto por **variables de entorno** (clave de **32 bytes** en
  **Base64**) y **hook del host** para un almacén propio, con **una clave activa** (con la que se protege) y un
  **conjunto de claves de lectura** (las que el resolutor acepta) y la **ventana de solapamiento con fecha** para
  una rotación sin big-bang (la **operación** de rotar es del Bolt 023).
- El **punto de fijación/registro** de los resolvedores por esquema (el del framework y los del host), con el
  patrón de **registro único al arranque**; el registro de un esquema del host sin resolvedor es **error tipado**.
- La **validación de esquemas**: `plain:` como **identidad**; esquema desconocido ⇒ error tipado (con la clave y
  el prefijo ofensores); esquema **sin resolvedor registrado** ⇒ error tipado propio.
- La **familia de errores tipados** de la política, sin filtrado de contenido ni material de clave en los
  mensajes.
- El **retiro de la superficie estática con clave implícita** (`Cryptography.Encrypt(string)`/`Decrypt(string)`),
  con su constancia de ruptura para las notas de migración (ADR-008 §3.1).
- La **evidencia**: el cuadro de casos negativos por causa, la constancia de que **no** hay material de clave en
  el árbol ni en configuración versionada (incluida la verificación de que la clave del formato legacy **no
  reaparece en ninguna forma**), la de que el **nonce no se reutiliza** y de que la **integridad precede al
  uso**, y el **contrato publicado** de la composición del valor y de la referencia de clave.
- La **política de nulabilidad del código aportado del 1.4** (revisión 2, §E.3): la directiva `#nullable disable`
  **por archivo aportado del 1.4** que produzca diagnósticos de nulabilidad —**jamás** en código nuevo (el
  resolutor nuevo conserva la nulabilidad activa) y **prohibida** a nivel de proyecto—, como mecánica de build del
  aporte.

### Out of scope

- El **seam del contrato** (`IProtectedValueResolver` en `Core`), el **catálogo de claves de configuración** y la
  validación de la **forma** del prefijo: **Bolt 011** (ADR-004 §3.2/§3.3). Este Bolt **consume** el seam.
- La **migración** de los valores ya cifrados de 1.4, la **inspección**, el **`--dry-run`**, el **`--apply`**, la
  **re-protección por rotación** y el **re-targeteo**: **Bolt 023** (ADR-007 §3.3/§3.5), con la capacidad legacy
  **aislada, exceptuada y con registro de retiro**. Este Bolt **no** la habilita ni la reutiliza.
- El **shell delgado del encriptador** (`ConnectionStringEncriptor`) y su empaquetado: **Bolt 024** (y ADR-009
  §3.6).
- La **corrección del defecto de F-23** (eliminación del cifrado legacy del runtime y de la clave embebida): su
  **BUG + Bolt**. Acá se entrega el **estándar** contra el que ese fix se verifica (ADR-007 §3.6).
- Los **esquemas del host** (`dpapi:`, `x509:`, `kv:`): los aporta el host por el hook; el framework **no** los
  implementa.
- La **precedencia de configuración** y el **archivo** que la declara: **ADR-004 §3.4 / Bolt 021**.
- Cualquier **dependencia de terceros** para criptografía: ADR-001 §3.4 y ADR-007 (el mecanismo es de la
  plataforma).
- La **política de primitivas para usos ajenos** a la protección de valores: es de ADR-007 §3.4 y se aplica en
  cada Bolt que la use; y **no** se reintroduce ningún control de integridad de despliegue (ADR-001 §3.3).

---

## 5. Prerequisites and baseline

- **Bolt 011 y su SPEC canónica
  ([`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md), `prerequisites`):** el **seam**
  `IProtectedValueResolver` en `Core` (esquema + carga ⇒ valor, con `kid` disponible), el **catálogo de claves**
  —de donde sale el **nombre completo de la clave de configuración** que este resolutor usa como **AAD** y como
  identificador en los diagnósticos— y la validación de la **forma** del prefijo. Si el contrato no está en el
  árbol 2.0 al iniciar el V-Bounce, **parar y reportar** (§15).
- **El proyecto `Cooperator.Framework.Library` en el árbol 2.0** (`netstandard2.1`, BCL puro, sin dependencias de
  terceros): es donde vive la implementación del esquema (ADR-007 §3.1). Si no existe, **parar y reportar**.
- **Bolt 001 y su SPEC canónica
  ([`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md)):** el arnés (donde corre la
  batería de casos negativos), el build compartido (`Directory.Build.props`/`Directory.Packages.props`) y las
  comprobaciones de invariantes, incluido el **gate de constantes de alta entropía** con el que ADR-007 §5 mide
  *No embedded secrets*.
- **Plataforma:** `System.Security.Cryptography.AesGcm` está en la superficie de `netstandard2.1` (confirmado por
  ADR-007 §3.1): **no** hace falta multi-target, shim ni paquete (`System.Security.Cryptography.Algorithms`
  queda descartado por ADR-001 §3.4).
- **Claves de prueba:** el V-Bounce y la suite usan claves **generadas en memoria** para las pruebas (nunca
  versionadas). La clave real de cada entorno la aporta el **entorno** o el **hook** del host: **no** es un
  prerequisito de este Bolt ni vive en el repositorio.
- **Árbol `1.4/` presente y congelado** en la línea base de §3, para el inventario de la superficie que se retira
  y para la verificación de que la clave del formato legacy no reaparece.
- **Build state de partida:** el árbol 2.0 compila y su suite corre; este Bolt **no** cambia TFMs, **no** agrega
  paquetes y **no** toca `Data` ni el tooling (sólo `Library`, y `Core` **no** se toca: ADR-007 §3.1 deja la
  criptografía fuera de la capa base).
- **Decisión de ejecución registrada (no es supuesto):** el control se verifica **en proceso** (proteger,
  resolver y fallar según el cuadro de casos negativos), sin superficie desplegada ni servicio externo: de ahí el
  plan de integración `n/a`.

---

## 6. Phases

> Todas las fases implementan **el mismo Bolt** (`US-000.BOLT-022`). Nada de este SPEC se implementa antes de
> `AITL-SPEC-Approval` (G14).
>
> **Mecánica de build del aporte del 1.4 (revisión 2, §E.3).** Cualquier archivo que este Bolt **aporte del 1.4**
> —el caso declarado: `Cryptography.cs` (legacy) y lo que el aporte traiga, en la fase que lo materialice— sigue la
> **política de nulabilidad de §E.3**: `#nullable disable` como primera línea útil **sólo** en los archivos
> aportados que **produzcan** `CS8618`/`CS8603`/`CS8625` (criterio objetivo; el build es la prueba), **nunca** en
> código nuevo y **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es la trampa que ya detuvo a
> los Bolts 003/007/009; esta revisión la deja autorizada **antes** del V-Bounce.

### Phase A — El registro de resolvedores y el esquema del framework

**Duration:** 3h total cycle — **Complexity:** High

#### A.1 Dónde se registran los resolvedores

El contrato (`IProtectedValueResolver`) ya existe en `Core` (Bolt 011) y **no** se toca: `Core` sigue **sin**
tocar criptografía (ADR-007 §3.1). Se implementa en `Library` el **punto de registro y resolución por esquema**,
con el mismo patrón de **fijación única al arranque** que ADR-004 §3.2 y ADR-006 §3.1 usan: el framework registra
su resolvedor de **`plain:`** (identidad) y de **`aesgcm:`**, y el **host** registra los suyos (`dpapi:`,
`x509:`, `kv:`, …) por el mismo punto. **Un esquema del host sin resolvedor registrado es error tipado** que dice
exactamente eso — nunca un intento de descifrado a ciegas.

La frontera con el Bolt 011 queda así de precisa: la **forma** del prefijo (que exista, que `plain:` sea
explícito, que un prefijo desconocido sea error tipado con la clave y el prefijo ofensores) es **de la
configuración** (ADR-004 §3.3); **este** Bolt resuelve los **esquemas registrados** y produce el error de
"esquema sin resolvedor" y los errores del mecanismo.

**Files created (en `Cooperator.Framework.Library`):**
- `…/Cryptography/ProtectedValues/ProtectedValueResolvers.cs` — el punto de registro/resolución por esquema (fijación única al arranque; `plain:` y `aesgcm:` del framework; los del host por el mismo punto)
- `…/Cryptography/ProtectedValues/PlainTextValueResolver.cs` — el resolvedor de identidad (`plain:`): devuelve la carga tal cual, sin interpretarla

#### A.2 La composición del valor, publicada

El valor protegido del framework es **`aesgcm:<kid>:<base64>`**: el `kid` identifica la clave y la carga es
**Base64** de **`nonce (12 B) | tag (16 B) | ciphertext`**, en ese orden. La composición, los tamaños y la
codificación de la clave de entorno (**32 bytes en Base64**) se **publican** en la documentación del paquete: son
**contrato** para la herramienta de migración y rotación (Bolt 023) y para cualquier consumidor que necesite
reproducir el formato —nunca para reimplementar el mecanismo—.

**Files created:**
- `…/Cryptography/ProtectedValues/ProtectedValueFormat.cs` — el parseo y la composición de `aesgcm:<kid>:<base64>` (validación estructural de la carga: longitudes mínimas, Base64 válido)

---

### Phase B — El mecanismo: proteger y resolver con integridad

**Duration:** 4h total cycle — **Complexity:** High

#### B.1 Proteger

Proteger **exige** la **referencia explícita de clave** (`kid` + key provider): **no** hay sobrecarga que use una
clave implícita (esa superficie se retira, §E). Cada operación genera un **nonce nuevo y aleatorio** (12 bytes,
CSPRNG del runtime) y **nunca** lo reutiliza; el **AAD** es el **nombre completo de la clave de configuración**
que va a contener el valor, y el resultado es la cadena `aesgcm:<kid>:<base64>` con la composición de §A.2.

**Files created:**
- `…/Cryptography/ProtectedValues/AesGcmValueResolver.cs` — la implementación de `aesgcm:`: proteger y resolver, con nonce por operación y verificación de tag
- `…/Cryptography/ProtectedValues/IProtectedValueKeyProvider.cs` — el contrato del proveedor de claves (referencia ⇒ clave), con el proveedor por **variables de entorno** como default y el **hook del host** para un almacén propio

#### B.2 Resolver

Resolver recibe el **esquema**, el **`kid`** y la **carga**, y **verifica la integridad antes de cualquier uso**:
si el tag no verifica —bit alterado, tag truncado, carga manipulada— **no** hay valor: hay **excepción tipada**
(NFR *Protected value integrity*: 100 % de los casos de manipulación detectados; 0 valores resueltos sin
verificación de tag). El **AAD** se compara contra el **nombre de la clave de configuración** que el consumidor
declara: un valor **movido** de un slot a otro **falla de forma explícita** en lugar de "funcionar" con la
credencial equivocada (ADR-007 §3.1).

**Garantías que la implementación debe sostener (y que la suite verifica):** cifrado **autenticado** (nunca sólo
confidencialidad), **nonce aleatorio** por operación y **nunca** reutilizado, **sin** comparación de tags no
constante, y **sin** ninguna primitiva de la lista prohibida de ADR-007 §3.4 (MD5, SHA-1, DES/3DES/RC2, ECB,
`*CryptoServiceProvider` concretos: se usan los factory).

---

### Phase C — Provisión de claves y rotación por `kid`

**Duration:** 3h total cycle — **Complexity:** Medium

#### C.1 La fuente de las claves es externa al producto

- **Por defecto: variables de entorno.** La clave de 32 bytes se lee de una variable de entorno **codificada en
  Base64** (elección que ADR-007 §3.1 delega al Bolt y que acá se **fija y documenta**), con la convención de
  jerarquía del entorno que ya usa la configuración del framework. Un valor con longitud distinta de 32 bytes o
  que no sea Base64 válido ⇒ **error tipado** (nunca una clave recortada, derivada ni completada con ceros).
- **Hook del host.** El host puede registrar su **proveedor de claves** (DPAPI sobre un archivo protegido por el
  SO, Key Vault/KMS, un gestor corporativo). El framework sólo consume la **referencia** (`kid`); el **material**
  es del proveedor. Un fallo del proveedor del host produce el **error tipado** correspondiente, con la
  referencia intentada y **sin** el material.
- **Nada de claves en el producto:** ni literales, ni derivadas de una constante, ni en el repositorio, ni en
  configuración versionada, ni en el `.coop2`. La verificación es explícita (gate de constantes de alta entropía
  de ADR-007 §5) e incluye la constancia de que **la clave del formato legacy no reaparece en ninguna forma**
  (ni como literal, ni transformada, ni como semilla, ni embebida en un fixture).

#### C.2 Una clave activa y un conjunto de lectura

El registro de claves tiene **dos roles** (ADR-007 §3.2): una clave **activa** —con la que se **protege**— y un
conjunto de claves **de lectura** —las que el resolutor acepta al resolver—. Un valor cuyo `kid` **no** tiene
clave disponible produce **error tipado** (nunca "probar con otra clave" ni degradar a texto plano). La **ventana
de solapamiento** de una rotación se declara en la **configuración**, con **fecha**: agregar la clave nueva como
activa y dejar la anterior en lectura es una operación de configuración; **re-proteger** los valores es la
**operación de la herramienta** (Bolt 023) y **no** de este Bolt.

**Files created:**
- `…/Cryptography/ProtectedValues/EnvironmentValueKeyProvider.cs` — el proveedor por variables de entorno (clave de 32 bytes en Base64), con la validación de forma y tamaño
- `…/Cryptography/ProtectedValues/ProtectedValueKeyRegistry.cs` — las claves registradas por `kid`, con el rol activo/lectura y la ventana de solapamiento declarada

---

### Phase D — Validación de esquemas y errores tipados

**Duration:** 2h total cycle — **Complexity:** Medium

#### D.1 El cuadro de errores (sin filtrado)

La familia de errores de la política, **tipada** y con **contexto suficiente para arreglar sin depurar** —la
**clave de configuración** y el **`kid`**, nunca el contenido ni el material—:

| Causa | Resultado |
|---|---|
| Valor **sin** prefijo | `CooperatorConfigurationException` (**del contrato**, Bolt 011 / ADR-004 §3.3): exige declarar `plain:` explícitamente — no se asume texto plano |
| Prefijo **desconocido** | `CooperatorConfigurationException` con la **clave** y el **prefijo ofensor** (ADR-004 §3.3) |
| Esquema **sin resolvedor registrado** | Error tipado propio del resolutor: dice exactamente que el esquema no tiene resolvedor registrado |
| Carga **manipulada** o **tag truncado** | Error tipado de **integridad**: el tag no verifica |
| **AAD** que no corresponde (valor movido de clave de configuración) | Error tipado: el valor no corresponde a esa clave de configuración |
| `kid` **ausente**, **desconocido** o **sin clave disponible** | Error tipado: la referencia de clave no tiene clave — **nunca** degradar a texto plano |
| **Clave de entorno mal formada** (no Base64 / longitud ≠ 32 B) | Error tipado con la **referencia** y la causa, sin el material |
| **Hook del host** que falla | Error tipado que indica que el fallo vino del proveedor del host + la excepción original |

**Regla transversal:** ningún mensaje, log ni reporte incluye el **valor**, la **clave** ni material derivado
(ADR-007 §3.2: los reportes identifican por **clave de configuración** y `kid`).

**Files created:**
- `…/Cryptography/ProtectedValues/ProtectedValueException.cs` — la excepción base de la política (con clave de configuración y `kid` en el contexto)
- `…/Cryptography/ProtectedValues/ProtectedValueIntegrityException.cs` — carga manipulada / tag que no verifica / AAD que no corresponde
- `…/Cryptography/ProtectedValues/ProtectedValueKeyNotFoundException.cs` — `kid` ausente, desconocido o sin clave disponible
- `…/Cryptography/ProtectedValues/ProtectedValueSchemeException.cs` — esquema sin resolvedor registrado
- `…/Cryptography/ProtectedValues/ProtectedValueKeyProviderException.cs` — fallo del proveedor (entorno mal formado o hook del host que falla)

---

### Phase E — El retiro de la superficie estática y la evidencia

**Duration:** 2h total cycle — **Complexity:** Low

#### E.1 Lo que deja de existir

Se retira de la superficie pública de `Library` la **API estática con clave implícita**:
`Cryptography.Encrypt(string)` y `Cryptography.Decrypt(string)` (las sobrecargas **sin** clave), y con ellas el
camino por el que una clave vive dentro del producto. Proteger y resolver pasan a exigir **referencia explícita
de clave** (`kid` + proveedor) y el resolutor del esquema (ADR-007 §3.1). Es una **ruptura de superficie** que se
registra con su reemplazo (ADR-008 §3.1) para las **notas de migración** (documento del Bolt 028): quien usara
`Library.Cryptography` directo pasa a los esquemas (`plain:` o `aesgcm:`), y quien tuviera valores cifrados con
el formato viejo los migra con la herramienta (Bolt 023).

> **Frontera:** el **cifrado legacy** (3DES+MD5 con la clave pública del 1.4) **no** entra acá ni se reutiliza:
> vive **sólo** en el componente interno de la herramienta (Bolt 023), detrás de su bandera y con su registro de
> retiro (ADR-007 §3.3/§3.4). Este Bolt **no** implementa ningún camino que lea el formato viejo.

#### E.2 Evidencia

- El **cuadro de casos negativos** de §D.1, cada uno con su **tipo de excepción** y con la verificación de que el
  mensaje **no** filtra contenido.
- La constancia de que **no** hay material de clave en el árbol, en el repositorio ni en configuración versionada
  —incluida la verificación explícita de que la **clave del 1.4 no reaparece en ninguna forma**— (NFR de
  ADR-007 §5).
- La evidencia de que el **nonce no se reutiliza** (N protecciones: N nonces distintos) y de que la **integridad
  precede al uso** (un valor manipulado no se resuelve).
- El **contrato publicado** de la composición del valor protegido y de la referencia de clave (§A.2).
- La constancia de que la **superficie estática con clave implícita ya no existe** y de que resolver exige la
  referencia de clave.
- La entrada de la **ruptura** en las notas de migración (ADR-008 §3.1).

#### E.3 La mecánica de build del aporte del 1.4: política de nulabilidad (revisión 2)

El árbol 2.0 hereda de `2.0/Directory.Build.props` (Bolt 001) `Nullable=enable` **y** `TreatWarningsAsErrors=true`,
mientras que el código del 1.4 es **anterior** a las anotaciones de nulabilidad. Es la trampa **ya medida** en esta
ola —`MEM-261016-0107` §8.2 (28 diagnósticos `CS86xx` al portar el modelo), `MEM-261016-0209` §8.1 (`CS8618` en
`BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`) y `MEM-261016-0227` §13 (el aviso declarado del
Bolt 009)— y afectaría **a este Bolt** en cuanto aporte un archivo del 1.4 con elementos sin inicializar, retornos
posiblemente nulos o `null` literales: los diagnósticos `CS8618`/`CS8603`/`CS8625` que el árbol convierte en
**errores** y que dejan el gate de Build sin poder cerrar en `pass`.

**Mecanismo autorizado (decidido por esta revisión; espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y
`SPEC-261016-0037` §B.2):** cada archivo **aportado del 1.4** que **produzca diagnósticos de nulabilidad** empieza
con la directiva `#nullable disable` (primera línea útil, antes de los `using`), y **sólo** esos archivos la llevan:

- **Sí la llevan:** los archivos **aportados del 1.4** por este Bolt que produzcan `CS8618`/`CS8603`/`CS8625`. El
  **criterio es objetivo** —*archivo aportado del 1.4 + diagnóstico de nulabilidad*— y la lista exacta la fija la
  **ejecución con el build**, no un nombre supuesto. La directiva **apaga el análisis**: no cambia una instrucción
  de IL y deja la semántica del 1.4 intacta.
- **No la llevan:** el **código nuevo** de este Bolt —el resolutor `aesgcm:`, el registro por esquema, el formato,
  la provisión de claves, la familia de excepciones y las pruebas—, que conserva la nulabilidad **activa** del
  árbol porque está escrito para el mundo anotado; y queda **prohibido** `<Nullable>disable</Nullable>` en
  **cualquier** `.csproj`, incluido el de `Library`: apagaría el análisis del proyecto entero.
- **El build es la prueba:** con `TreatWarningsAsErrors=true`, si la directiva **falta** donde hace falta el build
  **falla** con el diagnóstico exacto (`CS8618`/`CS8603`/`CS8625`); si **sobra** en código nuevo, se ve en la
  lectura de los archivos entregados. Se verifica en §8.1, §9 y AC-11.

**Un `using` que quede sin uso** como consecuencia del **retiro** de esta fase (la superficie estática con clave
implícita, §E.1) se retira **con él**: es parte de la forma ya resuelta del archivo. Un `using` muerto **hacia un
namespace que no existe en el árbol 2.0** que **no** provenga de un retiro propio **no** está cubierto por esta
autorización: se **detiene y se reporta** (§15).

**Alternativas descartadas (declaradas).** **Anotar o «corregir» el código aportado** (agregar `?`,
inicializadores o `null!`) es una **transformación** que cambia el código del 1.4 sin decisión que la ordene —y en
este Bolt sería además un riesgo de seguridad: tocar el camino criptográfico heredado sin ADR que lo ordene—;
**diferir el aporte** dejaría al Bolt sin la pieza que debe retirar. La directiva, en cambio, es de **una línea por
archivo**, su efecto queda **acotado a los archivos aportados** y **no toca el comportamiento ni el material de
clave**: es el mismo criterio con el que se resolvió en los Bolts 003, 007 y 009.

---

## 7. Acceptance criteria

> El Bolt es no-funcional (contra `US-000`): estos criterios son el **resultado medible** de su §2 y sus
> restricciones son los ADR que lo gobiernan.

### AC-1: Proteger y resolver exigen una referencia explícita de clave

**Given** el resolutor del framework
**When** se protege un valor
**Then** la operación exige la **referencia de clave** (`kid` + proveedor) y **no** existe una sobrecarga que use
una clave implícita **And Given** que se resuelve un valor, el `kid` del valor determina la clave usada, sin
"probar con otra"

### AC-2: La integridad precede a cualquier uso

**Given** un valor protegido y una **carga manipulada** (un bit alterado) o con **tag truncado**
**When** se intenta resolver
**Then** **no** se obtiene valor: se obtiene el **error tipado** de integridad, y el mensaje **no** filtra
contenido ni material (NFR *Protected value integrity*)

### AC-3: El AAD ata el valor a su clave de configuración

**Given** un valor protegido con el AAD de la clave de configuración `X`
**When** se intenta resolver como si perteneciera a la clave de configuración `Y`
**Then** falla con el error tipado de AAD —el valor **no** puede moverse de slot sin re-protegerlo, y re-targetear
es operación de la herramienta (Bolt 023)—

### AC-4: Un `kid` sin clave es un error, nunca una degradación

**Given** un valor con un `kid` **ausente**, **desconocido** o **sin clave disponible**
**When** se resuelve
**Then** se obtiene el error tipado de clave no encontrada, **nunca** un intento con otra clave y **nunca** una
degradación a texto plano

### AC-5: No hay material de clave en el producto

**Given** el árbol 2.0, el repositorio y la configuración versionada
**When** se busca material de clave
**Then** hay **0** claves literales o derivadas de constantes (gate de constantes de alta entropía), **0** claves
en configuración versionada o en el `.coop2`, y la constancia explícita de que **la clave del formato legacy del
1.4 no reaparece en ninguna forma** (NFR *No embedded secrets*)

### AC-6: El nonce es por operación y la composición está publicada

**Given** N operaciones de protección con la misma clave y el mismo valor
**When** se inspeccionan las cargas producidas
**Then** los **nonces son todos distintos** (12 bytes aleatorios, CSPRNG del runtime) y la composición
`aesgcm:<kid>:<base64>` con `nonce | tag | ciphertext` (más los tamaños y la codificación de la clave de entorno)
está **publicada** como contrato del paquete

### AC-7: Los esquemas se validan sin degradación

**Given** un valor `plain:` y un valor con un **esquema sin resolvedor registrado**
**When** se resuelven
**Then** `plain:` devuelve la carga **tal cual** (identidad) y el esquema sin resolvedor produce el **error
tipado** propio (dice exactamente eso) — y el valor **sin** prefijo o con un prefijo **desconocido** produce el
error tipado del **contrato de configuración** (Bolt 011), con la clave y el prefijo ofensores

### AC-8: Los esquemas del host conviven sin que el framework los implemente

**Given** un host que registra su resolvedor para su esquema por el punto de registro
**When** se resuelve un valor de ese esquema
**Then** lo resuelve **el host**, el framework **no** implementa ese esquema, y un fallo de ese resolvedor se
reporta como error tipado **del host** con la excepción original

### AC-9: La superficie estática con clave implícita se retira y la ruptura queda declarada

**Given** la superficie publicada de `Library` y el gate de compatibilidad de API
**When** se revisa
**Then** `Cryptography.Encrypt(string)` y `Cryptography.Decrypt(string)` (las sobrecargas sin clave) **no**
existen, resolver exige la referencia de clave, y la ruptura está registrada con su reemplazo en la lista que va
a las notas de migración (ADR-008 §3.1)

### AC-10: Ningún diagnóstico filtra el valor ni el material

**Given** cada uno de los casos negativos de §D.1
**When** se inspecciona el mensaje de la excepción y lo que el framework registra
**Then** el mensaje nombra la **clave de configuración** y el **`kid`** y **no** contiene el valor, la clave ni
material derivado

### AC-11: El código aportado del 1.4 no rompe el build por nulabilidad

**Given** los archivos del 1.4 que este Bolt aporta (`Cryptography.cs` y lo que el aporte traiga)
**When** se construye el árbol con advertencias tratadas como error
**Then** no queda **ningún** diagnóstico de nulabilidad: cada archivo **aportado del 1.4** que lo requiera lleva la
directiva de **§E.3** como primera línea útil, **ningún** archivo nuevo la lleva, y **ningún** `.csproj` declara
`<Nullable>disable</Nullable>`

### AC mapping to source (functional) / measurable outcome (non-functional)

| Source AC / outcome | How this SPEC satisfies it | Verifying test/evidence |
|---------------------|----------------------------|--------------------------|
| Bolt §2 — proteger y resolver exigen una referencia explícita de clave | §6/B.1, §6/E.1 | AC-1, AC-9 · superficie + casos |
| Bolt §2 — un valor manipulado no se resuelve: la integridad precede a cualquier uso | §6/B.2 | AC-2 · `ProtectedValueIntegrityTests` |
| Bolt §2 — una referencia de clave sin clave disponible es error tipado | §6/C.2, §6/D.1 | AC-4 · `KeyProvisionTests` |
| Bolt §2 — el esquema de identidad resuelve el valor tal cual; lo desconocido es error tipado | §6/A.1, §6/D.1 | AC-7 · `SchemeValidationTests` |
| Bolt §2 — ninguna clave, nonce o material derivado en el código, el repositorio ni un archivo versionado | §6/C.1, §6/E.2 | AC-5 · gate de constantes de alta entropía + verificación de la clave legacy |
| Bolt §2 — ningún diagnóstico filtra el contenido ni material de clave | §6/D.1 | AC-10 · casos negativos con inspección del mensaje |
| Bolt §2 — los esquemas del host conviven sin que el framework los implemente | §6/A.1 | AC-8 · `HostSchemeTests` |
| ADR-007 §3.1 — composición, AAD por clave de configuración, nonce por operación y retiro de la superficie estática | §6/A.2, §6/B.1, §6/B.2 | AC-2, AC-3, AC-6, AC-9 · casos + contrato publicado |
| ADR-007 §3.2 — provisión externa, clave activa + lectura, nada de secretos en reportes | §6/C.1, §6/C.2, §6/D.1 | AC-4, AC-5, AC-10 · casos + verificación de material |
| ADR-007 §5 — NFRs *No embedded secrets* y *Protected value integrity* | §6/C.1, §6/B.2 | AC-2, AC-5 · mediciones del NFR |
| ADR-004 §3.3 — prefijo obligatorio, sin heurística, error tipado y quién invoca al resolutor | §6/A.1, §6/D.1 | AC-7 · casos de esquema/prefijo |
| Bolt §2 — el código del 1.4 que este Bolt aporta no rompe el build por nulabilidad (mecánica ya medida en los Bolts 003/007/009) | §6/E.3: directiva `#nullable disable` **por archivo aportado del 1.4 + diagnóstico** | AC-11 · build de `Library` + lectura de los archivos entregados |

---

## 8. Testing strategy

### 8.1 V-Bounce level (hermetic — run by the agent)

- **Round-trip y composición:** proteger → resolver devuelve el mismo valor; la carga es Base64 válido con la
  estructura `nonce | tag | ciphertext` y los tamaños publicados (12/16); el `kid` presente en el valor coincide
  con el usado; `plain:` devuelve la carga tal cual (identidad, sin interpretar).
- **Cuadro de casos negativos (uno por causa de §D.1), cada uno verificando tipo de excepción y ausencia de
  contenido en el mensaje:** bit alterado en el ciphertext; bit alterado en el tag; **tag truncado**; carga con
  Base64 inválido; carga con longitud insuficiente; **AAD de otra clave de configuración** (valor movido de
  slot); `kid` ausente, `kid` desconocido, `kid` registrado **sin** clave disponible; clave de entorno con
  longitud ≠ 32 bytes; clave de entorno que no es Base64; esquema **sin resolvedor registrado**; **hook** del
  host que falla; valor **sin** prefijo y valor con prefijo **desconocido** (los dos del contrato de
  configuración, Bolt 011 —verificando que el error es el del contrato—).
- **Nonce:** N (p. ej. 1 000) protecciones del **mismo** valor con la **misma** clave ⇒ **todos** los nonces
  distintos (parseados de la carga), y ninguna estructura trivial (no constante, no secuencial, no derivada del
  texto plano): el nonce es aleatorio por operación, no un contador.
- **Provisión de claves:** proveedor por entorno (una variable de entorno de prueba con una clave generada **en
  memoria** y limpiada al terminar el test), clave activa usada al proteger, claves de lectura aceptadas al
  resolver, la anterior (en lectura) sigue resolviendo sus valores y **no** se usa para proteger, `kid` nuevo sin
  clave ⇒ error; hook del host registrado ⇒ se usa y no se consulta el entorno para ese esquema; hook que falla ⇒
  error tipado del host con la excepción original.
- **Esquemas y registro:** `plain:` y `aesgcm:` registrados por el framework; un esquema del host registrado se
  resuelve por su resolvedor; un esquema **no** registrado produce el error de "sin resolvedor"; y **no** existe
  un camino que pruebe descifrados alternativos ante un fallo (el fallo es terminal para ese valor).
- **Verificación de material y de superficie (invariantes):** gate de **constantes de alta entropía** sobre el
  árbol 2.0 (ADR-007 §5); búsqueda de la **clave del 1.4** (`_CryptoKey`, sus transformaciones y cualquier
  derivación) en el código, en los fixtures y en la configuración versionada: **0** apariciones; ausencia de
  `Cryptography.Encrypt(string)`/`Decrypt(string)` en la superficie pública (y en el diff del gate de API como
  ruptura declarada); ausencia de las primitivas prohibidas (MD5/SHA-1/DES/3DES/RC2/ECB/`*CryptoServiceProvider`
  concretos) en el camino de protección de valores.
- **Política de nulabilidad del código aportado (§E.3, revisión 2):** chequeo por **lectura** de los archivos
  entregados —cada archivo **aportado del 1.4** que produzca diagnósticos de nulabilidad empieza con
  `#nullable disable` y **ningún** archivo nuevo la lleva—, más el propio `build` con
  `TreatWarningsAsErrors=true` (la prueba real: sin la directiva, el build falla con `CS8618`/`CS8603`/`CS8625`). Se
  verifica además que **no** hay `<Nullable>disable</Nullable>` en ningún `.csproj` y que ningún `using` muerto
  hacia un namespace inexistente quedó sin resolver ni autorizado (`using` muerto que **no** provenga del retiro de
  §E.1 ⇒ parar, §15).
- **Consumer-side contract tests (test doubles):** los dos bordes se ejercitan con implementaciones de prueba:
  un **proveedor de claves** del host (hook) y un **resolvedor de un esquema del host**; y el conjunto se registra
  por el **punto de fijación única**, que es el contrato con el host (ADR-007 §3.1). El contrato de configuración
  (Bolt 011) se ejerce con su implementación de entorno y con un doble para el caso de clave ausente.
- **Edge cases:** valor vacío; valor con caracteres no ASCII (a diferencia del 1.4 **no** hay pérdida: el valor
  se trata como texto y su codificación es la del contrato); valor muy largo; clave con bytes nulos; dos `kid`
  distintos con la misma clave (permitido, documentado: la clave es del `kid`, no al revés); registro del mismo
  esquema dos veces (el punto de fijación única decide y lo documenta); `kid` con caracteres que exigen formato en
  el valor (se define el conjunto admitido y se rechaza lo demás con error tipado); resolver dos veces el mismo
  valor (idempotente y sin estado); proteger con la clave activa y resolver con las de lectura, incluidos los
  valores de la clave anterior; **FIPS**: no aplica a `AesGcm` (FIPS-compatible, ADR-007 §3.5) —se deja constancia
  de que la suite no requiere configuración FIPS especial y de que **no** hay primitivas legacy en este camino—.
- **BUG evidence:** `n/a` — no es un Bolt de BUG; no hay RED previo que exhibir ni protocolo TDD estricto (ese
  protocolo rige BUG Bolts, §3.3.1). La corrección del defecto de F-23 tiene su propio BUG/Bolt y **este** Bolt es
  el estándar contra el que su fix se verifica (ADR-007 §3.6).
- **Evidencia que produce el V-Bounce:** la batería del cuadro de casos negativos con su tipo de excepción y la
  inspección del mensaje; la corrida de nonces (N valores, todos distintos); la provisión por entorno y por hook
  con sus casos; la verificación de material (gate de entropía + búsqueda de la clave legacy: 0 apariciones); la
  ausencia de la superficie estática con su entrada de ruptura; y el contrato publicado del formato. Todo se pega
  al MEM.

### 8.2 Integration test level (written in a V-Bounce, executed in the integration test cycle)

> El frontmatter declara `applicability: "n/a"` porque el Bolt declaró `test_integration: not-required`
> (§3.2.2). Esta sección explicita por qué no hay nada que ejecutar en ese nivel.

- **Integration tests (real dependencies):** `n/a` — el Bolt es `hardening` y el control se verifica **en proceso**
  (proteger, resolver y fallar según el cuadro). No hay base de datos, cola, servicio, almacenamiento ni entorno
  desplegado que aporte una verificación adicional: el mecanismo es de la **plataforma** (`AesGcm`) y el material
  de claves vive en el entorno del proceso.
- **Contract / E2E against the real providers:** `n/a` — el único proveedor del caso real es el **entorno del
  host** (variables de entorno) o el **hook** del consumidor, y ambos se ejercitan en el nivel V-Bounce con la
  misma forma que tendrían en producción. Un proveedor real de un esquema del host (Key Vault, DPAPI) es del
  **consumidor** y su verificación es suya, no un requisito de este Bolt.
- **Edge cases:** los de §8.1, ejecutados herméticamente; no hay una segunda clase de bordes que sólo aparezca
  con dependencias reales (el borde operativo —claves mal configuradas en el entorno del cliente— es explícito por
  diseño: error tipado, nunca silencio).
- **BUG evidence (a defect that reproduces only in integration tests):** `n/a` — no es un Bolt de BUG.

---

## 9. Quality gates

> Gates seleccionados por este SPEC (§3.6) con su nivel. Cada uno termina `pass` / `waived` (ADR-NNN) /
> `n/a` (con razón). Las filas de nivel *Integration test* no aplican porque el plan de integración es `n/a`
> (§8.2) y **esa ausencia es la que hace coherente el `n/a` del frontmatter**.

| Gate | Level | Threshold | Status |
|------|-------|-----------|--------|
| Build / lint / type checks | V-Bounce | Sin errores ni warnings (warnings como error, Bolt 001); `Library` compila sobre `netstandard2.1` con `AesGcm` in-box; **sin diagnósticos de nulabilidad** (la directiva de §E.3, sólo en los archivos aportados del 1.4 que la requieren) | pass |
| Unit / component tests | V-Bounce | 100 % verdes, incluidos el cuadro de casos negativos, la corrida de nonces y la provisión por entorno/hook | pass |
| Consumer-side contract tests (test doubles) | V-Bounce | El contrato con el host (`IProtectedValueResolver` + el punto de registro único) se ejercita con un proveedor de claves y un resolvedor de esquema del host de prueba | pass |
| SAST | V-Bounce | 0 hallazgos altos; 0 claves literales, 0 primitivas prohibidas, 0 degradación a texto plano en el camino de resolución | pass |
| Dependency scanning / licenses | V-Bounce | **0** dependencias de terceros para criptografía (el mecanismo es de la plataforma, ADR-007/ADR-001 §3.4); ninguna dependencia nueva | pass |
| OWASP Top 10 — static review | V-Bounce | Revisión criptográfica: AEAD (autenticado), nonce aleatorio por operación, tag verificado **antes** de usar, comparación de tags por la API de la plataforma (constante), AAD por clave de configuración (anti-reuso entre slots), sin modos/primitivas prohibidas, sin fallback ni oráculo de contenidos en los errores | pass |
| Prompt-injection scan | V-Bounce | `n/a` — no se interpreta contenido textual como instrucción; se procesan valores de configuración con un formato declarado | n/a (razón) |
| Secret-leak scan | V-Bounce | **0** material de clave o valores en el código, en el repositorio, en la configuración versionada, en los fixtures y en los mensajes; **0** apariciones de la clave del formato legacy en cualquier forma (NFR *No embedded secrets*) | pass |
| Hallucination lint | V-Bounce | Cada componente trazable a ADR-007 §3.1/§3.2/§3.4 y al seam del Bolt 011; 0 primitivas o esquemas ajenos a la política; 0 capacidades de otro Bolt (migración, rotación operativa, encriptor) adelantadas | pass |
| IP / license provenance | V-Bounce | Código propio sobre la API de la plataforma; 0 código de terceros; la implementación no copia la del formato legacy | pass |
| PII / DLP | V-Bounce | `confidential` (mirror del Bolt): los **valores protegidos** son material sensible y las **claves** lo son más — la suite usa claves generadas **en memoria**, nunca reales, y ningún valor real entra al repositorio o a los reportes | pass |
| Dependency-confusion | V-Bounce | `n/a` en su forma clásica — no se agregan paquetes; y la resolución de esquemas es **explícita** (registro), sin búsqueda por nombre | n/a (razón) |
| Test-first evidence | V-Bounce | El **cuadro de casos negativos** se escribe desde ADR-007 §5 (los dos NFR y sus mediciones) **antes** de implementar el resolutor, y la verificación de material se escribe antes de retirar la superficie estática | pass |
| Behavioral reproducibility (hermetic suite) | V-Bounce | Dos corridas de la suite dan el mismo veredicto; el **payload** protegido es deliberadamente **no determinista** (nonce aleatorio por operación) y eso se **verifica como propiedad** (AC-6), no se trata como excepción | pass |
| Bolt-manifest validation | V-Bounce | El manifiesto del Bolt valida contra `manifest-v5-bolt.schema.json` tras registrar la revisión y el V-Bounce | pass |
| Integration tests (real dependencies) | Integration test | No aplica: plan de integración `n/a` (Bolt `not-required`) | n/a (razón) |
| Contract / E2E against the real providers | Integration test | No aplica: el proveedor del caso real (entorno/hook) se ejercita herméticamente con la misma forma | n/a (razón) |
| DAST / OWASP Top 10 — dynamic | Integration test | No aplica: sin superficie desplegada ni servicio | n/a (razón) |
| Perf-smoke (p95/p99) | Integration test | No aplica: no hay objetivo de rendimiento en el alcance; el costo por operación es el de la primitiva de la plataforma | n/a (razón) |
| Observability (logs / metrics / traces) | Integration test | No aplica: la librería no agrega telemetría (§11) y **no** debe registrar valores ni material | n/a (razón) |

---

## 10. Security and data

- **Clasificación declarada por este SPEC: `confidential`** — espeja la del **Bolt** (que declara
  `confidential`, no `internal`) y es coherente con la materia: **material de claves** y **valores protegidos**.
  Es la clasificación que lee el gate PII/DLP.
- **Este Bolt es, en sí, la respuesta a un defecto de seguridad** (F-23): entrega el **estándar** —AEAD, clave
  externa, nonce por operación, integridad verificada, AAD por slot— contra el que la corrección del cifrado
  legacy se verifica (ADR-007 §3.6). Lo que **no** hace: corregir el defecto en sí (su BUG/Bolt), migrar el
  corpus (Bolt 023) ni habilitar la capacidad legacy (que vive aislada en la herramienta).
- **Ninguna clave en el producto:** ni literal, ni derivada de una constante, ni en el repositorio, ni en
  configuración versionada, ni en el `.coop2`; la provisión es **externa** (entorno por defecto; hook del host
  para un almacén propio). La verificación incluye que la **clave del formato legacy no reaparezca en ninguna
  forma** — es la comprobación que ADR-007 §5 exige explícitamente.
- **Integridad antes que confidencialidad en el uso:** el valor **no** se entrega si el tag no verifica
  (manipulación detectada, 0 degradaciones silenciosas) y el **AAD** impide reutilizar un valor en otro slot
  (un copiar/pegar entre entornos falla en lugar de "funcionar" con la credencial equivocada).
- **Sin oráculo:** los diagnósticos nombran la **clave de configuración** y el **`kid`** y **nunca** el valor ni
  el material; el fallo de integridad no distingue "casi correcto" de "cualquier otra cosa" más allá de lo que
  la API de la plataforma permite.
- **Sin superficie nueva de red ni de archivos:** el resolutor no lee archivos (la configuración la aporta quien
  la lee, Bolt 011/021) ni se conecta a nada; el hook del host es **su** código y su **propia** política de
  confianza (Key Vault/KMS/DPAPI).
- **Ciclo de vida de las claves:** una **activa** y un conjunto de **lectura** con ventana de solapamiento
  **fechada**; un `kid` sin clave es error tipado (nunca silencio). La **disponibilidad** de las claves es una
  propiedad operativa del cliente y su fallo es explícito (deuda declarada por ADR-007 §4).
- **FIPS:** el camino vigente no depende de configuración FIPS —`AesGcm` es FIPS-compatible— y la suite deja
  constancia; las primitivas legacy (que sí lanzan con FIPS) **no** están en este Bolt (ADR-007 §3.5).

---

## 11. Monitoring and observability

- **`n/a`** en el sentido de telemetría: el entregable es una biblioteca portable (`Library`) y **no** emite
  logs, métricas ni trazas. No se le agrega instrumentación porque ninguna fuente lo pide y, además, hay una
  razón de política: **no debe** registrar valores ni material (ADR-007 §3.2).
- **La observabilidad de este Bolt es su evidencia:** la batería de casos negativos con su tipo de excepción, la
  corrida de nonces (N valores distintos), la provisión por entorno y por hook con sus fallos, la verificación de
  ausencia de material (gate de entropía + la clave legacy: 0 apariciones) y la ausencia de la superficie
  estática con su entrada de ruptura.
- **Diagnóstico en runtime (lo que sí existe y es parte del entregable):** errores **tipados** con la **clave de
  configuración** y el **`kid`**, y con la causa (integridad, AAD, clave ausente, esquema sin resolvedor, fallo
  del proveedor del host). Es el reemplazo del fallo característico del 1.4: basura silenciosa o texto plano.
- La telemetría del consumidor (qué valor protegió, con qué `kid`) es del consumidor: la herramienta de
  migración/rotación reporta eso por su Bolt (023), con la misma regla de no filtrar contenido.

---

## 12. Migration, compatibility and rollback

- **Migration:** **no** hay migración de valores en este Bolt (es del Bolt 023). Lo que sí hay es una **ruptura de
  superficie** y una **guía**: quien usaba `Library.Cryptography` directo —incluida la API estática con clave
  implícita— pasa a los **esquemas** (`plain:` para lo que no necesita protección, `aesgcm:` con la clave activa
  y su `kid`); y los valores que hoy están "cifrados" con el formato de 1.4 se migran con la **herramienta**
  (Bolt 023), que los descifra con la capacidad **aislada, exceptuada y con fecha de retiro** y los re-protege
  con este mecanismo (ADR-007 §3.3). Las notas de migración las documenta el **Bolt 028** (ADR-008 §3.1) y este
  Bolt aporta su entrada: **qué** desaparece (`Encrypt(string)`/`Decrypt(string)` sin clave, y con ellas la
  posibilidad de una clave implícita), **por qué** (ADR-007 §3.1: una clave en el producto es el defecto de F-23)
  y **con qué se reemplaza** (los esquemas con referencia explícita de clave).
- **Compatibility:** el `ICooperatorConfiguration` y su catálogo no cambian (Bolt 011); lo que cambia es que un
  valor de conexión (o cualquier valor) puede llegar con un **esquema** y resolverse **después** de que la capa
  que lo consume invoque al resolutor (ADR-004 §3.3). Los consumidores del framework (Data, tooling) ya están
  escritos con ese seam (§4); este Bolt lo **implementa**.
- **Lo que este Bolt deliberadamente no relaja:** el valor **sin** prefijo sigue siendo error tipado (nunca
  texto plano implícito) y un prefijo desconocido sigue fallando con la clave y el prefijo (ADR-004 §3.3). La
  protección **no** introduce una vía de "si no se puede descifrar, se usa el texto".
- **Rollback:** revertir el commit del V-Bounce. No hay estado persistido: la clave vive en el entorno del
  proceso y los valores protegidos, en la configuración del consumidor. Un valor protegido con este mecanismo
  deja de resolverse si se revierte el framework, por lo que la reversión del paquete y la de los valores van
  juntas (lo que refuerza la regla de no re-proteger masivamente sin ventana: la rotación es del Bolt 023, con
  solapamiento declarado).

---

## 13. Risk matrix

| Risk | Probability (1-5) | Impact (1-5) | Mitigation |
|------|-------------------|-------------|------------|
| **Cifrado mal usado**: nonce reutilizado o integridad no verificada (el defecto de F-23, repetido) | 2 | 5 | Nonce aleatorio **por operación** (CSPRNG, verificado como propiedad con N protecciones) + verificación de tag **antes** de usar + casos negativos de manipulación como **criterio de aceptación** (NFR de ADR-007 §5) |
| **Las claves vuelven al producto** (una clave literal, derivada o incluso la del 1.4 en un fixture) | 3 | 5 | Prov. externa por diseño + gate de constantes de alta entropía + búsqueda explícita de la clave legacy en **cualquier** forma (código, fixtures, configuración versionada): 0 apariciones |
| **Fuga por diagnóstico** (un mensaje que incluye el valor o el material) | 3 | 4 | AC-10: cada caso negativo verifica que el mensaje nombra **clave de configuración** y `kid`, **nunca** el contenido (exigido por ADR-007 §3.2/**§5**) |
| **Un valor movido entre entornos "funciona" con la credencial equivocada** | 3 | 4 | **AAD = nombre completo de la clave de configuración** (AC-3): el movimiento de slot falla de forma explícita; re-targetear es de la herramienta |
| **Esquema desconocido resuelto por degradación** (o "probemos con otra clave") | 3 | 5 | Cuadro de errores tipados (AC-4, AC-7): esquema sin resolvedor y `kid` sin clave **fallan**; **0** caminos de fallback y **0** degradación a texto plano |
| **Confusión de frontera con el Bolt 011** (implementar acá la forma del prefijo o allá la protección) | 3 | 3 | §6/A.1 y §14 declaran la frontera: la **forma** es del contrato (011); los **esquemas registrados**, el mecanismo y sus errores son de este Bolt |
| **Confusión de frontera con el Bolt 023/024** (migración, rotación operativa o encriptor colándose acá) | 3 | 4 | §4 lo excluye y §15 lo declara condición de parada: la capacidad legacy **no** se habilita ni se reutiliza en el runtime; la rotación **como operación** es de la herramienta |
| **Rotación fallida** (se agrega la clave nueva y se retira la vieja antes de re-proteger) | 3 | 4 | Dos roles explícitos (activa + lectura) y ventana **fechada** en configuración; un `kid` sin clave falla con error tipado en lugar de resolver mal |
| **Proveedor del host que falla en silencio** | 3 | 3 | AC-8: el fallo se reporta como error tipado **del host** con la excepción original; no hay caída al entorno ni al texto plano |
| **Dependencia criptográfica de terceros** ("una librería lo hace mejor") | 2 | 4 | ADR-007/ADR-001 §3.4: el mecanismo es de la plataforma; el gate de dependencias lo verifica |
| **El payload se vuelve determinista** (alguien "optimiza" reutilizando el nonce o derivándolo) | 2 | 5 | AC-6 lo verifica como **propiedad** del entregable: N protecciones ⇒ N nonces distintos; un payload repetido es un fallo de la suite, no una optimización |
| **Clave mal configurada en el entorno del cliente** | 4 | 3 | Error tipado por clave mal formada/ausente (nunca silencio) y documentación de la forma (32 bytes en Base64); la **disponibilidad** de claves es propiedad operativa del cliente (deuda declarada en ADR-007 §4) |
| **Ruptura sin declarar** (`Cryptography.Encrypt(string)`/`Decrypt(string)`) | 2 | 3 | AC-9: la ruptura entra en la lista con su reemplazo y el gate de API la cubre (ADR-008 §5) |
| **La directiva de nulabilidad se aplica de más** (a código nuevo o al `.csproj`) y apaga el análisis de un proyecto entero —o **de menos**, y el build falla con `CS8618`/`CS8603`/`CS8625` | 3 | 3 | §E.3 la acota por **archivo aportado del 1.4 + diagnóstico de nulabilidad**; §8.1/AC-11 la verifican por lectura **y** por el build (con advertencias como error, una aplicación incorrecta se ve de inmediato); `<Nullable>disable</Nullable>` en cualquier `.csproj` queda **prohibido** |

---

## 14. Decisions and trade-offs

> Micro-decisiones de este SPEC. Las decisiones de arquitectura ya estaban en ADR-007/ADR-004/ADR-001; acá se
> aplican. Las filas marcadas **(⚠ aprobador)** quedan a la vista para el `AITL-SPEC-Approval`.

| Decision | Reason |
|----------|--------|
| **(⚠ aprobador) El punto de registro/resolución por esquema vive en `Library`, no en `Core`** | ADR-007 §3.1 dice que el **contrato** vive en `Core` y la **implementación** en `Library`, y que "`Core` sigue sin tocar criptografía". El registro de resolvedores es parte del **mecanismo** (registrar el del framework y los del host) y por eso se implementa con la implementación: así `Core` queda intacto (cero criptografía) y el host que aporta un esquema referencia `Library`, que es portable y BCL puro. **Alternativa:** el registro en `Core` (sólo interfaces) para que un host con esquemas propios no dependa de `Library`; si el aprobador lo prefiere, es una revisión de este SPEC (cambia dónde vive un archivo, no el comportamiento) |
| **(⚠ aprobador) La codificación de la clave por entorno es Base64 (32 bytes)** | ADR-007 §3.1 delega explícitamente la elección ("Base64 o hex, el Bolt elige una y la documenta"). Se elige Base64 por coherencia con la carga del valor protegido (misma codificación en los dos extremos) y se **documenta** en el contrato del paquete |
| La familia de excepciones de la política se declara con tipos propios (integridad, clave no encontrada, esquema sin resolvedor, fallo del proveedor) más el error del **contrato** para la forma del prefijo | El Bolt exige "errores tipados" sin nombrarlos; nombrarlos en el SPEC los hace verificables (AC-4/7/8/10) y deja claro **qué** error corresponde a **qué** causa. La forma del prefijo **no** se reimplementa: usa la excepción del contrato (Bolt 011) |
| El registro de resolvedores se **fija una vez al arranque** (patrón de ADR-004 §3.2/ADR-006 §3.1) | Es lo que ADR-007 §3.1 pide para los esquemas del host y evita dos respuestas distintas para el mismo esquema dentro de un proceso. El comportamiento de un registro tardío se prueba y se documenta (no se deja indeterminado) |
| `plain:` se implementa acá como **resolvedor de identidad** | Es parte del despacho por esquema y es trivial; su **forma** (que el prefijo exista y sea explícito) sigue siendo del contrato (Bolt 011). Separarlo dejaría el despacho incompleto |
| La **verificación de material** incluye buscar la clave del 1.4 en **cualquier** forma (literal, transformada, semilla, fixture) | ADR-007 §5 lo exige explícitamente y es el riesgo más grave de este Bolt (que la clave "vuelva" al producto en un test o en un fixture "de compatibilidad", que además introduciría la capacidad legacy prohibida) |
| La **operación de proteger** exige la referencia de clave y **no** tiene sobrecarga implícita | Es exactamente lo que el ADR retira (`Encrypt(string)`/`Decrypt(string)`) y lo que hace imposible proteger sin declarar con qué clave: la ausencia de sobrecarga es la garantía, no una convención |
| El **AAD** se recibe como el **nombre completo de la clave de configuración** desde el consumidor | ADR-007 §3.1 lo fija así, y ADR-004 §3.3 dice que **quien consume** el valor es quien invoca al resolutor: por lo tanto el nombre de la clave se lo pasa el consumidor, y el resolutor **no** adivina de dónde salió el valor |
| El **nonce** y el **tag** no se exponen en la superficie | Son contrato del **formato** (publicado en la documentación), no API pública: exponerlos invitaría a construir valores a mano, que es el camino por el que un consumidor comete un error de seguridad |
| **No** se agrega ningún test vector con clave fija al repositorio | Un vector fijo con una clave de prueba en el repo sería material de clave **versionado** (prohibido por ADR-007 §5, aunque sea de prueba, y la regla no distingue "de prueba"). La suite genera claves **en memoria**; el determinismo se verifica como **propiedad** (nonce distinto y round-trip), no con un vector |
| No se incluye un comando ni una herramienta en este Bolt | Los comandos (`inspect`/`migrate`/`reprotect`) son del **Bolt 023** y el encriptor del **024** (ADR-007 §3.5): acá se entrega la capacidad, no su operación |
| **(⚠ aprobador) Política de nulabilidad del código aportado del 1.4: `#nullable disable` por archivo aportado que lo requiera** (revisión 2) | Es la trampa **ya medida** tres veces en esta ola (Bolts 003/007/009: 28 diagnósticos `CS86xx` en el modelo, `CS8618` en `BaseObject.cs(152,19)`/`DescriptionFieldEventArg.cs(48,16)`, y el aviso declarado del 009 —`MEM-261016-0107` §8.2, `MEM-261016-0209` §8.1, `MEM-261016-0227` §13—) y **este Bolt aporta código del 1.4**. El mecanismo es el **espejo exacto** de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y `SPEC-261016-0037` §B.2: preserva la semántica del 1.4 (apaga el análisis, **no cambia el IL**), es de **una línea por archivo**, su efecto queda **acotado a los archivos aportados** y **no toca el comportamiento ni el material de clave**. Se **descartan**, declaradas, las alternativas de **anotar/corregir** el código aportado (transformación no ordenada, y en este Bolt un riesgo de seguridad: tocar el camino criptográfico heredado sin ADR) y de **diferir** el aporte (dejaría al Bolt sin la pieza que debe retirar). Se elige la opción que **evita una cuarta revisión material por el mismo motivo** |

---

## 15. Stop conditions

- **El contrato del Bolt 011 (`IProtectedValueResolver` en `Core`) no está en el árbol 2.0**: parar y reportar.
  La salida es una enmienda de dependencia o una decisión de orden del dueño; **nunca** declarar el contrato acá
  (invadiría el Bolt 011 y crearía dos seams).
- **Se necesita leer o escribir el formato legacy de 1.4** (descifrar 3DES/MD5, "por compatibilidad" o para un
  test): parar. Esa capacidad vive **sólo** en el componente interno de la herramienta del Bolt 023, con su
  bandera y su registro de retiro (ADR-007 §3.3/§3.4); habilitarla acá rompería la condición **1** de la
  excepción acotada ("nunca en `Core`/`Library`/`Data` ni en ningún paquete desplegado").
- **Se pretende usar una primitiva prohibida** (MD5/SHA-1/DES/3DES/RC2/ECB, IV o nonce fijos, comparación de tags
  no constante, `*CryptoServiceProvider` concreto) o **una dependencia de terceros** para el mecanismo: parar.
  ADR-007 §3.4 y ADR-001 §3.4 lo fijan, y un cambio de primitivas exige un ADR que reemplace a ADR-007.
- **Aparece material de clave en el árbol, en el repositorio o en configuración versionada** (incluida una clave
  "de prueba" fija o un vector): parar. La regla de ADR-007 §5 no admite excepciones por comodidad.
- **Un caso del cuadro de errores no puede cumplirse sin filtrar el valor o el material** (por ejemplo un
  mensaje de la plataforma que incluya el payload): parar y reportar; el mensaje se envuelve con el contexto
  permitido (clave de configuración y `kid`) y **no** se propaga el original con contenido.
- **El entorno exige que el framework implemente los esquemas del host** (`dpapi:`, `x509:`, `kv:`): parar.
  ADR-007 §3.1 los deja al host por el hook.
- **Se pide que el valor sin prefijo se trate como texto plano** (o que un prefijo desconocido se intente
  descifrar): parar. Es la regla que ADR-004 §3.3 y ADR-007 §3.1 eliminaron y reintroducirla exigiría un ADR.
- **Se pretende corregir el defecto de F-23 o migrar valores en este Bolt**: parar y reportar; son de su
  BUG/Bolt y del Bolt 023 (ADR-007 §3.6). Este Bolt es el **estándar** contra el que se verifica.
- **La clave activa no puede provenir del entorno ni del hook** (alguien pide un archivo de claves versionado o
  una clave embebida "temporal"): parar. La provisión externa es la decisión (ADR-007 §3.2).
- **Cambia materialmente una fuente gobernada** (el Bolt, ADR-007/ADR-004/ADR-001, REV-001, la línea base de
  código): parar, revisar esta misma SPEC canónica y re-aprobar (G15).
- **Un gate de §9 no cierra en `pass`**: si la única salida fuera una excepción de gate, exige un ADR aprobado
  por `AITL-ADR-Approval` (G21) — nunca una decisión del ejecutor. Y en particular: **no** existe un waiver por
  conveniencia para el gate de *No embedded secrets*.
- **Alguien extiende la autorización de §E.3**: poner la directiva `#nullable disable` en **código nuevo** (el
  resolutor, el formato, la provisión, las pruebas) o `<Nullable>disable</Nullable>` en un `.csproj`; o intentar
  resolver con esa misma autorización un `using` muerto **hacia un namespace que no existe en el árbol 2.0** que
  **no** provenga del retiro de §E.1. Parar y reportar: la autorización es **una** y está acotada en §E.3.

---

## 16. Definition of Done (DoD)

- [ ] All phases implemented
- [ ] All acceptance criteria pass
- [ ] Hermetic tests GREEN (§8.1); integration tests written, built and statically checked (§8.2) — executed in the integration test cycle; BUG: red + green evidence recorded — **`n/a` para este Bolt: no es un BUG y su plan de integración es `n/a`**
- [ ] Code follows applicable ADRs (ADR-007 §3.1–§3.4 y §5, ADR-004 §3.3, ADR-001 §3.3/§3.4/§3.5)
- [ ] Applicable gates pass / waived (ADR) / n/a (reason)
- [ ] La directiva de nulabilidad de **§E.3** aplicada **sólo** a los archivos aportados del 1.4 que la requieren, y verificada (lectura + build); ningún `.csproj` con `<Nullable>disable</Nullable>` y ningún `using` muerto sin resolver ni autorizado
- [ ] MEM created in `devflow/memory/` (exactly one per V-Bounce)
- [ ] Manifest `v_bounces[]` entry appended in `devflow/metrics/bolts/`
- [ ] AITL-MEM-Approval recorded
- [ ] Integration test plan satisfied before acceptance: `n/a` con su razón (Bolt `test_integration: not-required`)

---

## 17. References

- [`US-000.BOLT-022`](../functional/bolts/US-000.BOLT-022-resolutor-aesgcm.md) — el Bolt que este SPEC implementa.
- [`US-000`](../functional/user-stories/US-000-non-functional.md) — contenedor padre (sin ciclo de aprobación).
- [`REV-001`](../reviews/REV-001-analisis-migracion-cooperator.md) — F-23 (clave e IV hardcodeados, TripleDES con clave derivada por MD5, encoding ASCII: ofuscación tratada como defecto de seguridad, con necesidad de política criptográfica y plan de migración), F-24 (tipos `*CryptoServiceProvider` obsoletos) y §6 ítem 12.
- [`ADR-007`](../adrs/ADR-007-politica-criptografica.md) — §3.1 (mecanismo y esquemas; composición `nonce | tag | ciphertext`; tamaños que este Bolt fija; AAD por nombre de clave de configuración; esquemas del host; contrato en `Core` e implementación en `Library`; retiro de la API estática con clave implícita), §3.2 (provisión por entorno y hook; clave activa y de lectura; ventana de rotación con fecha; nada de secretos en logs/reportes), §3.3 (migración legacy: **fuera**), §3.4 (primitivas permitidas y prohibidas; la excepción acotada que este Bolt no habilita), §3.5 (herramientas y FIPS), §3.6 (qué no cubre), §4 (trade-offs y deuda), §5 (NFRs *No embedded secrets* y *Protected value integrity*).
- [`ADR-004`](../adrs/ADR-004-cache-y-configuracion.md) — §3.2 (contrato de configuración y catálogo; de dónde sale el nombre de la clave que se usa como AAD), §3.3 (prefijo de esquema obligatorio, prohibición de la heurística, error tipado ante lo desconocido y **quién** invoca al resolutor).
- [`ADR-001`](../adrs/ADR-001-layout-y-empaquetado-cooperator-2.md) — §3.4/§3.5 (dónde vive el contrato y la implementación; capa portable sin terceros), §3.3 (el gate retirado no vuelve; la firma es identidad), §3.6 (criptografía como slot reservado y canal de distribución pendiente).
- [`SPEC-261016-0018`](./SPEC-261016-0018-configuracion-portable.md) — el contrato de configuración, el catálogo y la validación de la forma del prefijo (prerrequisito, Bolt 011).
- [`SPEC-261016-0000`](./SPEC-261016-0000-arnes-tests-y-build-reproducible.md) — arnés, build compartido y comprobaciones de invariantes (prerrequisito, Bolt 001).
- **Precedentes de la mecánica de nulabilidad (revisión 2):** [`SPEC-261016-0004`](./SPEC-261016-0004-particion-modeling.md) **§A.3** · [`SPEC-261016-0013`](./SPEC-261016-0013-retiro-de-serializable.md) **§A.3** · [`SPEC-261016-0037`](./SPEC-261016-0037-library-portable.md) **§B.2** — `#nullable disable` **por archivo aportado/portado del 1.4**, fijado por **revisión material** con su medición (`MEM-261016-0107` §8.2: 28 diagnósticos `CS86xx`; `MEM-261016-0209` §8.1: `CS8618` en `BaseObject.cs(152,19)` y `DescriptionFieldEventArg.cs(48,16)`; `MEM-261016-0227` §13: el aviso declarado del Bolt 009). Es el mecanismo que esta revisión **espeja**, **antes** de que el aporte del 1.4 de este Bolt lo requiera.
- [`US-000.BOLT-023`](../functional/bolts/US-000.BOLT-023-comandos-de-valores-protegidos.md) · [`US-000.BOLT-024`](../functional/bolts/US-000.BOLT-024-encriptor-shell-delgado.md) — la migración, la rotación como operación y el shell delgado del encriptor (fronteras declaradas).
- [`US-000.BOLT-028`](../functional/bolts/US-000.BOLT-028-notas-de-migracion.md) — el documento de notas que recibe la ruptura de la superficie estática.
- [`OQ-003`](../analysis/open-questions/OQ-003-gate-de-licenciamiento.md) — el gate por token sigue retirado: la integridad de despliegue **no** se cubre en código del framework (coherente con ADR-007 §3.6).

---

## 18. Revision history

| Date | Author | Change |
|------|--------|--------|
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 1** — redacción inicial del SPEC canónico para el Bolt aprobado `US-000.BOLT-022`: pre-SPEC evidence gate (§3), fases A–E, criterios AC-1…AC-10, mecanismo con su composición y tamaños publicados, provisión y rotación por `kid`, cuadro de errores tipados sin filtrado, retiro de la superficie estática, estrategia de pruebas (cuadro de casos negativos, corrida de nonces, verificación de material incluida la clave legacy), gates (con la revisión criptográfica y el gate de secretos como ejes), control de datos `confidential` (espeja el Bolt) y condiciones de parada. Declara las fronteras con los Bolts 011 (seam), 023 (migración/rotación) y 024 (encriptor). `integration_test_plan: n/a` coherente con el Bolt. |
| 2026-10-16 | `agent:dotnet-developer-agent` (llm: `deepseek/deepseek-flash`) | **Revisión 2 — revisión MATERIAL (G15/G16) PREVENTIVA**, cursada **antes** de que la trampa muerda y sin que exista todavía un V-Bounce de este Bolt: los tres V-Bounces que aportaron código legacy en esta ola (003/007/009) chocaron con la **nulabilidad** del árbol (`Nullable=enable` + `TreatWarningsAsErrors`, del Bolt 001) y cada uno necesitó su **revisión material** para autorizar la directiva, y **este Bolt aporta código del 1.4** (el caso declarado: `Cryptography.cs`). Cambia **una sola cosa**: la **política de nulabilidad del código aportado del 1.4** (§E.3, nueva) —`#nullable disable` como primera línea útil **sólo** en los archivos aportados que **produzcan** `CS8618`/`CS8603`/`CS8625`, **jamás** en código nuevo (el resolutor y sus piezas conservan la nulabilidad activa), **prohibido** a nivel de proyecto, criterio objetivo (*archivo aportado + diagnóstico*) y **el build como prueba**—, con el **límite** del `using` muerto hacia un namespace inexistente que no provenga del retiro de §E.1 (parada y reporte, §15). Es el espejo de `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y `SPEC-261016-0037` §B.2. Se actualizan §1, §4, §6 (nota de mecánica + §E.3), §7 (AC-11 y su mapeo), §8.1, §9, §13, §14, §15, §16 y §17. **Nada más cambia:** el mecanismo `aesgcm:`, la composición y los tamaños publicados, la provisión de claves, el cuadro de errores tipados, el retiro de la superficie estática, las fronteras con 011/023/024 y el control `confidential` siguen en vigor. `status: draft`, `review_ready_at: ""` y `review` vacío: la revisión se re-aprueba por el revisor. |

---

## 19. AITL-SPEC-Approval

> **Avenga DevFlow §2.4.1, §3.0.** Este SPEC sigue siendo un borrador hasta que el Dev-validator (+ dueños de
> dominio aplicables) registre `AITL-SPEC-Approval` en el bloque `review` del frontmatter. La aprobación del
> Bolt (`AITL-BOLT-READY-Approval`) autoriza **preparar** el SPEC; la **aprobación del SPEC** autoriza el
> code-run / V-Bounce. Un cambio material en una fuente gobernada invalida esta aprobación: parar, revisar esta
> misma SPEC y re-aprobar (G15).

**Nota al aprobador — qué cambia la revisión 2, y por qué (material, G15/G16).** Es una revisión **preventiva**:
**no** responde a un bloqueo de este Bolt —no hay V-Bounce todavía— sino a la **trampa ya medida tres veces** en esta
ola (los Bolts **003, 007 y 009** chocaron con la nulabilidad del árbol al aportar código del 1.4 y cada uno
necesitó una revisión material para autorizar la directiva), sabiendo que **este Bolt también aporta código del
1.4** (el caso declarado: `Cryptography.cs`). Cambia **una sola cosa**:

1. **Política de nulabilidad del código aportado del 1.4 (§E.3, nueva).** `#nullable disable` como **primera línea
   útil** **sólo** de los archivos **aportados del 1.4** que **produzcan** `CS8618`/`CS8603`/`CS8625` —criterio
   **objetivo**: *archivo aportado + diagnóstico de nulabilidad*; **el build es la prueba**—, **jamás** en código
   nuevo (el resolutor `aesgcm:`, el formato, la provisión, las excepciones y las pruebas conservan la nulabilidad
   **activa**) y **prohibido** `<Nullable>disable</Nullable>` en cualquier `.csproj`. Es el **espejo** de
   `SPEC-261016-0004` §A.3, `SPEC-261016-0013` §A.3 y `SPEC-261016-0037` §B.2 (los tres ya aprobados).
2. **Límite explícito (no autorizado):** un `using` muerto **hacia un namespace que no existe en el árbol** que
   **no** provenga del retiro de §E.1 sigue siendo **parada y reporte** (§E.3, §15); lo que **sí** se retira con el
   retiro de la superficie estática es el `using` que queda **sin uso** por él.
3. **Lo que NO cambia: todo lo demás.** El objeto del Bolt, el mecanismo `aesgcm:` con su composición y sus
   tamaños publicados, la provisión de claves (entorno + hook, clave activa y de lectura), el cuadro de errores
   tipados sin filtrado, el retiro de la superficie estática con su constancia, el control `confidential` (AC-8) y
   las fronteras con los Bolts 011/023/024 siguen **en vigor**. El `review` vuelve a vacío porque la revisión es
   material y se re-aprueba por el revisor.

| Field | Value |
|-------|-------|
| **review.reviewers** | [Dev-validator + dueño de dominio aplicable (**seguridad**, riesgo `high`)] |
| **review.written_by** | [opcional — quién escribió este registro: el revisor, o el agente que transcribió la decisión; al lado de los revisores, nunca entre ellos — escribir no es firmar (§3.0)] |
| **review.decision** | approved / changes_requested / rejected |
| **review_ready_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.started_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **review.decided_at** | `YYYY-MM-DDTHH:mm:ss±HH:MM` |
| **Findings** | [findings o acknowledged_without_comment + razón] |
