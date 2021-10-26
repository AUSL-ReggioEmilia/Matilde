CREATE TABLE [dbo].[T_MovCartelle] (
    [ID]                   UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [NumeroCartella]       VARCHAR (50)     NULL,
    [CodStatoCartella]     VARCHAR (20)     NULL,
    [CodUtenteApertura]    VARCHAR (100)    NULL,
    [CodUtenteChiusura]    VARCHAR (100)    NULL,
    [DataApertura]         DATETIME         NULL,
    [DataAperturaUTC]      DATETIME         NULL,
    [DataChiusura]         DATETIME         NULL,
    [DataChiusuraUTC]      DATETIME         NULL,
    [PDFCartella]          VARBINARY (MAX)  NULL,
    [CodStatoCartellaInfo] VARCHAR (20)     NULL,
    [MotivoRiapertura]     VARCHAR (500)    NULL,
    CONSTRAINT [PK_T_MovCartelle] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovCartelle_T_StatoCartella] FOREIGN KEY ([CodStatoCartella]) REFERENCES [dbo].[T_StatoCartella] ([Codice]),
    CONSTRAINT [FK_T_MovCartelle_T_StatoCartellaInfo] FOREIGN KEY ([CodStatoCartellaInfo]) REFERENCES [dbo].[T_StatoCartellaInfo] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_NumeroCartella]
    ON [dbo].[T_MovCartelle]([NumeroCartella] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ID_Include1]
    ON [dbo].[T_MovCartelle]([ID] ASC)
    INCLUDE([NumeroCartella]);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoCartella_Include1]
    ON [dbo].[T_MovCartelle]([CodStatoCartella] ASC)
    INCLUDE([ID], [NumeroCartella]);

