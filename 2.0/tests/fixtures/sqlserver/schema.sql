-- ============================================================================
-- Cooperator — esquema de fixture del ciclo de integración (SPEC §6.F.1).
-- SINTÉTICO Y MÍNIMO: un par padre/hijo con identidad, rowversion y tipos
-- representativos. No representa el esquema de ningún cliente.
-- Guarda dura: el arnés sólo aplica esto contra una base cuyo nombre contenga
-- `cooperator_it` y aborta en cualquier otro caso (SPEC §10).
-- ============================================================================

IF OBJECT_ID(N'dbo.ItChild', N'U') IS NOT NULL DROP TABLE dbo.ItChild;
IF OBJECT_ID(N'dbo.ItParent', N'U') IS NOT NULL DROP TABLE dbo.ItParent;

CREATE TABLE dbo.ItParent
(
    ParentId        INT             IDENTITY(1, 1) NOT NULL,
    Code            VARCHAR(20)     NOT NULL,
    Name            NVARCHAR(100)   NOT NULL,
    Amount          DECIMAL(18, 4)  NULL,
    Quantity        SMALLINT        NULL,
    IsActive        BIT             NOT NULL CONSTRAINT DF_ItParent_IsActive DEFAULT (1),
    CreatedOn       DATETIME2(3)    NOT NULL CONSTRAINT DF_ItParent_CreatedOn DEFAULT (SYSUTCDATETIME()),
    ExternalId      UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ItParent_ExternalId DEFAULT (NEWID()),
    Notes           VARCHAR(MAX)    NULL,
    Version         ROWVERSION      NOT NULL,
    CONSTRAINT PK_ItParent PRIMARY KEY CLUSTERED (ParentId),
    CONSTRAINT UQ_ItParent_Code UNIQUE (Code)
);

CREATE TABLE dbo.ItChild
(
    ChildId         INT             IDENTITY(1, 1) NOT NULL,
    ParentId        INT             NOT NULL,
    Detail          NVARCHAR(200)   NOT NULL,
    Weight          FLOAT           NULL,
    Version         ROWVERSION      NOT NULL,
    CONSTRAINT PK_ItChild PRIMARY KEY CLUSTERED (ChildId),
    CONSTRAINT FK_ItChild_ItParent FOREIGN KEY (ParentId) REFERENCES dbo.ItParent (ParentId)
);

CREATE NONCLUSTERED INDEX IX_ItChild_ParentId ON dbo.ItChild (ParentId);
