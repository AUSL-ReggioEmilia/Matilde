CREATE TABLE [dbo].[T_MovPrescrizioni] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [CodViaSomministrazione]  VARCHAR (20)     NULL,
    [CodTipoPrescrizione]     VARCHAR (20)     NULL,
    [CodStatoPrescrizione]    VARCHAR (20)     NULL,
    [CodStatoContinuazione]   VARCHAR (20)     NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [IDString]                VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovPrescrizioni] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovPrescrizioni_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovPrescrizioni_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovPrescrizioni_T_StatoContinuazione] FOREIGN KEY ([CodStatoContinuazione]) REFERENCES [dbo].[T_StatoContinuazione] ([Codice]),
    CONSTRAINT [FK_T_MovPrescrizioni_T_StatoPrescrizione] FOREIGN KEY ([CodStatoPrescrizione]) REFERENCES [dbo].[T_StatoPrescrizione] ([Codice]),
    CONSTRAINT [FK_T_MovPrescrizioni_T_TipoPrescrizione] FOREIGN KEY ([CodTipoPrescrizione]) REFERENCES [dbo].[T_TipoPrescrizione] ([Codice]),
    CONSTRAINT [FK_T_MovPrescrizioni_T_ViaSomministrazione] FOREIGN KEY ([CodViaSomministrazione]) REFERENCES [dbo].[T_ViaSomministrazione] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_IDString]
    ON [dbo].[T_MovPrescrizioni]([IDString] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Via]
    ON [dbo].[T_MovPrescrizioni]([CodViaSomministrazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovPrescrizioni]([CodTipoPrescrizione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovPrescrizioni]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovPrescrizioni]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovPrescrizioni]([IDTrasferimento] ASC)
    INCLUDE([ID]);

