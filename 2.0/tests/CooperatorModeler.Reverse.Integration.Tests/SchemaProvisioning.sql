-- SPEC-261016-0027 6/E.1 y 8.2: el aprovisionamiento de la base de ESQUEMA CONOCIDO de la lane de integracion del
-- reverse. Es el INSUMO DECLARADO de las expectativas: el modelo esperado se deriva de este script, no de la
-- implementacion que se cambia (la paridad se verifica contra el esquema y la tabla de mapeo, no contra si misma).
--
-- Contiene, a proposito, las formas que la paridad cubre: tipos nativos variados, clave primaria simple y COMPUESTA
-- (en orden no alfabetico), claves foraneas, columna identidad, rowversion, columna anulable y una VISTA (que el
-- modelo lee como tabla: es la limitacion declarada de ADR-002 3.1, que este Bolt reporta y no resuelve).
--
-- Sin datos de negocio ni personales: solo estructura.

IF SCHEMA_ID('cooperator_reverse_test') IS NULL
    EXEC('CREATE SCHEMA cooperator_reverse_test');
GO

IF OBJECT_ID('cooperator_reverse_test.Parent', 'U') IS NOT NULL DROP TABLE cooperator_reverse_test.Parent;
IF OBJECT_ID('cooperator_reverse_test.Child', 'U') IS NOT NULL DROP TABLE cooperator_reverse_test.Child;
IF OBJECT_ID('cooperator_reverse_test.Composite', 'U') IS NOT NULL DROP TABLE cooperator_reverse_test.Composite;
IF OBJECT_ID('cooperator_reverse_test.AllNullable', 'U') IS NOT NULL DROP TABLE cooperator_reverse_test.AllNullable;
IF OBJECT_ID('cooperator_reverse_test.ParentView', 'V') IS NOT NULL DROP VIEW cooperator_reverse_test.ParentView;
GO

-- Clave primaria simple con identidad y rowversion; columnas anulables y no anulables de varios tipos nativos.
CREATE TABLE cooperator_reverse_test.Parent
(
    ParentId        int             IDENTITY(1,1)   NOT NULL,
    Name            nvarchar(120)                   NOT NULL,
    Code            varchar(32)                     NULL,
    Amount          decimal(18, 4)                  NOT NULL,
    Ratio           float                           NULL,
    IsActive        bit                             NOT NULL,
    CreatedOn       datetime                        NOT NULL,
    Day             date                            NULL,
    RowVersion      rowversion                      NOT NULL,
    Notes           ntext                           NULL,
    CONSTRAINT PK_Parent PRIMARY KEY (ParentId)
);
GO

-- Clave primaria COMPUESTA con orden no alfabetico (Zeta, Alpha) y una clave foranea hacia Parent.
CREATE TABLE cooperator_reverse_test.Composite
(
    Zeta            int             NOT NULL,
    Alpha           int             NOT NULL,
    ParentId        int             NOT NULL,
    Total           numeric(12, 2)  NULL,
    CONSTRAINT PK_Composite PRIMARY KEY (Zeta, Alpha),
    CONSTRAINT FK_Composite_Parent FOREIGN KEY (ParentId) REFERENCES cooperator_reverse_test.Parent (ParentId)
);
GO

-- Varias columnas anulables de tipos distintos (el borde de la tabla con todas las columnas nulas).
CREATE TABLE cooperator_reverse_test.Child
(
    ChildId         int             IDENTITY(1,1)   NOT NULL,
    ParentId        int                             NOT NULL,
    Payload         varbinary(64)                   NULL,
    Slug            uniqueidentifier                NULL,
    Kind            smallint                        NULL,
    Stamp           smalldatetime                   NULL,
    CONSTRAINT PK_Child PRIMARY KEY (ChildId),
    CONSTRAINT FK_Child_Parent FOREIGN KEY (ParentId) REFERENCES cooperator_reverse_test.Parent (ParentId)
);
GO

CREATE TABLE cooperator_reverse_test.AllNullable
(
    Id              int             NOT NULL,
    TextValue       nvarchar(50)    NULL,
    IntValue        int             NULL,
    CONSTRAINT PK_AllNullable PRIMARY KEY (Id)
);
GO

-- La vista: el modelo la lee COMO TABLA (limitacion declarada, verificada como comportamiento esperado).
CREATE VIEW cooperator_reverse_test.ParentView
AS
    SELECT ParentId, Name FROM cooperator_reverse_test.Parent;
GO
