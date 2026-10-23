-- V001: tabelas de infraestrutura (usuários, OTP, outbox, idempotência, auditoria)

IF OBJECT_ID(N'dbo.tblUsuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblUsuarios
    (
        Id         UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_tblUsuarios PRIMARY KEY,
        Nome       NVARCHAR(150)    NOT NULL,
        Email      VARCHAR(200)     NOT NULL,
        SenhaHash  VARCHAR(100)     NOT NULL,
        Roles      VARCHAR(500)     NOT NULL, -- JSON: ["Gerente","Atendente"]
        Ativo      BIT              NOT NULL CONSTRAINT DF_tblUsuarios_Ativo DEFAULT 1,
        CriadoEm   DATETIME2        NOT NULL CONSTRAINT DF_tblUsuarios_CriadoEm DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_tblUsuarios_Email UNIQUE (Email)
    );
END;

IF OBJECT_ID(N'dbo.tblOtp', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblOtp
    (
        Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_tblOtp PRIMARY KEY,
        Telefone    VARCHAR(20)      NOT NULL,
        CodigoHash  VARCHAR(100)     NOT NULL,
        ExpiraEm    DATETIME2        NOT NULL,
        Tentativas  INT              NOT NULL CONSTRAINT DF_tblOtp_Tentativas DEFAULT 0,
        Usado       BIT              NOT NULL CONSTRAINT DF_tblOtp_Usado DEFAULT 0,
        CriadoEm    DATETIME2        NOT NULL CONSTRAINT DF_tblOtp_CriadoEm DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_tblOtp_Telefone ON dbo.tblOtp (Telefone);
END;

IF OBJECT_ID(N'dbo.tblOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblOutbox
    (
        Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_tblOutbox PRIMARY KEY,
        EventId      UNIQUEIDENTIFIER NOT NULL,
        Tipo         VARCHAR(200)     NOT NULL,
        Payload      NVARCHAR(MAX)    NOT NULL,
        CriadoEm     DATETIME2        NOT NULL CONSTRAINT DF_tblOutbox_CriadoEm DEFAULT SYSUTCDATETIME(),
        PublicadoEm  DATETIME2        NULL,
        Tentativas   INT              NOT NULL CONSTRAINT DF_tblOutbox_Tentativas DEFAULT 0,
        UltimoErro   NVARCHAR(2000)   NULL,
        CONSTRAINT UQ_tblOutbox_EventId UNIQUE (EventId)
    );
    CREATE INDEX IX_tblOutbox_PublicadoEm ON dbo.tblOutbox (PublicadoEm);
END;

IF OBJECT_ID(N'dbo.tblIdempotencia', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblIdempotencia
    (
        Chave        VARCHAR(128)  NOT NULL CONSTRAINT PK_tblIdempotencia PRIMARY KEY,
        HashRequest  VARCHAR(64)   NOT NULL,
        StatusCode   INT           NOT NULL,
        Body         NVARCHAR(MAX) NULL,
        CriadoEm     DATETIME2     NOT NULL CONSTRAINT DF_tblIdempotencia_CriadoEm DEFAULT SYSUTCDATETIME(),
        ExpiraEm     DATETIME2     NOT NULL
    );
    CREATE INDEX IX_tblIdempotencia_ExpiraEm ON dbo.tblIdempotencia (ExpiraEm);
END;

IF OBJECT_ID(N'dbo.tblAuditoria', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblAuditoria
    (
        Id             UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_tblAuditoria PRIMARY KEY,
        Entidade       VARCHAR(100)     NOT NULL,
        EntidadeId     VARCHAR(64)      NOT NULL,
        Campo          VARCHAR(100)     NOT NULL,
        ValorAnterior  NVARCHAR(MAX)    NULL,
        ValorNovo      NVARCHAR(MAX)    NULL,
        UsuarioId      UNIQUEIDENTIFIER NULL,
        Em             DATETIME2        NOT NULL CONSTRAINT DF_tblAuditoria_Em DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_tblAuditoria_Entidade ON dbo.tblAuditoria (Entidade, EntidadeId);
END;
