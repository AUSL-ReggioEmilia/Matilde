CREATE TABLE [dbo].[T_MovNote] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [CodEntita]               VARCHAR (20)     NULL,
    [IDEntita]                UNIQUEIDENTIFIER NULL,
    [CodStatoNota]            VARCHAR (20)     NULL,
    [Oggetto]                 VARCHAR (2000)   NULL,
    [Descrizione]             VARCHAR (2000)   NULL,
    [Colore]                  VARCHAR (50)     NULL,
    [CodSezione]              VARCHAR (20)     NULL,
    [CodVoce]                 VARCHAR (600)    NULL,
    [IDGruppo]                UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [DataInizio]              DATETIME         NULL,
    [DataFine]                DATETIME         NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovNote] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovNote_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovNote_T_StatoNote] FOREIGN KEY ([CodStatoNota]) REFERENCES [dbo].[T_StatoNote] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoNota]
    ON [dbo].[T_MovNote]([CodStatoNota] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovNote]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio]
    ON [dbo].[T_MovNote]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MovNote]([IDPaziente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataInizio]
    ON [dbo].[T_MovNote]([DataInizio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entita]
    ON [dbo].[T_MovNote]([CodEntita] ASC, [IDEntita] ASC);

