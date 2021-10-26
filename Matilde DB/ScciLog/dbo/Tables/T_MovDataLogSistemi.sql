CREATE TABLE [dbo].[T_MovDataLogSistemi] (
    [ID]             BIGINT           IDENTITY (1, 1) NOT NULL,
    [CodSistema]     VARCHAR (50)     NULL,
    [IDDataLog]      BIGINT           NULL,
    [Data]           DATETIME         NULL,
    [CodUtente]      VARCHAR (50)     NULL,
    [ComputerName]   VARCHAR (50)     NULL,
    [IpAddress]      VARCHAR (50)     NULL,
    [CodEvento]      VARCHAR (50)     NULL,
    [TipoOperazione] SMALLINT         NULL,
    [Operazione]     VARCHAR (50)     NULL,
    [LogPrima]       XML              NULL,
    [LogDopo]        XML              NULL,
    [Transito]       UNIQUEIDENTIFIER NULL,
    [DataTransito]   DATETIME         NULL,
    [InCarico]       BIT              NULL,
    [Output]         XML              NULL,
    [Trasmesso]      BIT              NULL,
    [Response]       TEXT             NULL,
    CONSTRAINT [PK_T_MovDataLogSistemi] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SistemaInCarico]
    ON [dbo].[T_MovDataLogSistemi]([CodSistema] ASC, [InCarico] ASC);

