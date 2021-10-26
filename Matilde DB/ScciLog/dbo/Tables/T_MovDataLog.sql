CREATE TABLE [dbo].[T_MovDataLog] (
    [ID]             BIGINT           IDENTITY (1, 1) NOT NULL,
    [Data]           DATETIME         NULL,
    [DataUTC]        DATETIME         NULL,
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
    CONSTRAINT [PK_T_DataLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Transito]
    ON [dbo].[T_MovDataLog]([Transito] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEvento]
    ON [dbo].[T_MovDataLog]([CodEvento] ASC);

