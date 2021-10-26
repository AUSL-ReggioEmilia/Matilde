CREATE TABLE [dbo].[T_MovFolder] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NOT NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [IDFolderPadre]           UNIQUEIDENTIFIER NULL,
    [Descrizione]             VARCHAR (255)    NULL,
    [CodStatoFolder]          VARCHAR (20)     NOT NULL,
    [CodEntita]               VARCHAR (20)     NOT NULL,
    [CodUA]                   VARCHAR (20)     NOT NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataRilevazione]         DATETIME         NULL,
    [DataRilevazioneUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovFolder] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovFolder_T_EntitaAllegato] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_EntitaAllegato] ([Codice]),
    CONSTRAINT [FK_T_MovFolder_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovFolder_T_MovFolder] FOREIGN KEY ([IDFolderPadre]) REFERENCES [dbo].[T_MovFolder] ([ID]),
    CONSTRAINT [FK_T_MovFolder_T_MovPazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovFolder_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovFolder_T_StatoFolder] FOREIGN KEY ([CodStatoFolder]) REFERENCES [dbo].[T_StatoFolder] ([Codice]),
    CONSTRAINT [FK_T_MovFolder_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio]
    ON [dbo].[T_MovFolder]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovFolder]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MovFolder]([IDPaziente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntita]
    ON [dbo].[T_MovFolder]([CodEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_MovFolder]([CodUA] ASC);

