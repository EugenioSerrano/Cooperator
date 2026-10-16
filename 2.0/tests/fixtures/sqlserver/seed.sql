-- ============================================================================
-- Cooperator — datos de fixture del ciclo de integración (SPEC §6.F.1).
-- SINTÉTICOS: dos padres con dos hijos cada uno. Ningún dato personal real.
-- Se aplican sólo tras `schema.sql`, y sólo contra una base cuyo nombre
-- contenga `cooperator_it` (guarda dura del arnés, SPEC §10).
-- ============================================================================

SET IDENTITY_INSERT dbo.ItParent OFF;

INSERT INTO dbo.ItParent (Code, Name, Amount, Quantity, IsActive, ExternalId, Notes)
VALUES (N'IT-0001', N'Fixture padre uno', 1234.5678, 7, 1, '11111111-1111-1111-1111-111111111111', N'nota uno');
INSERT INTO dbo.ItParent (Code, Name, Amount, Quantity, IsActive, ExternalId, Notes)
VALUES (N'IT-0002', N'Fixture padre dos', -0.0100, 0, 0, '22222222-2222-2222-2222-222222222222', NULL);

INSERT INTO dbo.ItChild (ParentId, Detail, Weight)
SELECT ParentId, N'Detalle A del padre ' + Code, 1.5 FROM dbo.ItParent WHERE Code = N'IT-0001';
INSERT INTO dbo.ItChild (ParentId, Detail, Weight)
SELECT ParentId, N'Detalle B del padre ' + Code, NULL FROM dbo.ItParent WHERE Code = N'IT-0001';
INSERT INTO dbo.ItChild (ParentId, Detail, Weight)
SELECT ParentId, N'Detalle C del padre ' + Code, 0.0 FROM dbo.ItParent WHERE Code = N'IT-0002';
