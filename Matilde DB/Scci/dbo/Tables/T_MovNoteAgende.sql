CREATE TABLE [dbo].[T_MovNoteAgende] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [CodAgenda]               VARCHAR (20)     NULL,
    [CodStatoNota]            VARCHAR (20)     NULL,
    [Oggetto]                 VARCHAR (2000)   NULL,
    [Descrizione]             VARCHAR (2000)   NULL,
    [Colore]                  VARCHAR (50)     NULL,
    [IDGruppo]                UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [DataInizio]              DATETIME         NULL,
    [DataFine]                DATETIME         NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [TuttoIlGiorno]           BIT              NULL,
    [EscludiDisponibilita]    BIT              NULL,
    CONSTRAINT [PK_T_MovNoteAgende] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovNoteAgende_T_Agende] FOREIGN KEY ([CodAgenda]) REFERENCES [dbo].[T_Agende] ([Codice]),
    CONSTRAINT [FK_T_MovNoteAgende_T_StatoNoteAgende] FOREIGN KEY ([CodStatoNota]) REFERENCES [dbo].[T_StatoNoteAgende] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_DataFine]
    ON [dbo].[T_MovNoteAgende]([DataFine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodAgenda]
    ON [dbo].[T_MovNoteAgende]([CodAgenda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDGruppo]
    ON [dbo].[T_MovNoteAgende]([IDGruppo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stato]
    ON [dbo].[T_MovNoteAgende]([CodStatoNota] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Data]
    ON [dbo].[T_MovNoteAgende]([DataInizio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovNoteAgende]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoNotaDataInizioDataFine_Include1]
    ON [dbo].[T_MovNoteAgende]([CodStatoNota] ASC, [DataInizio] ASC, [DataFine] ASC)
    INCLUDE([ID], [CodAgenda]);


GO
CREATE NONCLUSTERED INDEX [IX_CodAgendaCodStatoNotaDataInizio_DataFine]
    ON [dbo].[T_MovNoteAgende]([CodAgenda] ASC, [CodStatoNota] ASC, [DataInizio] ASC, [DataFine] ASC)
    INCLUDE([ID], [Oggetto], [Descrizione], [Colore], [IDGruppo], [DataEvento], [TuttoIlGiorno]);

