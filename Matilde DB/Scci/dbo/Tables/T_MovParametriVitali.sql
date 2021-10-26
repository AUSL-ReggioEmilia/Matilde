CREATE TABLE [dbo].[T_MovParametriVitali] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [CodTipoParametroVitale]  VARCHAR (20)     NULL,
    [CodStatoParametroVitale] VARCHAR (20)     NULL,
    [ValoriFUT]               XML              NULL,
    [ValoriGrafici]           XML              NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [CodSistema]              VARCHAR (20)     NULL,
    [IDSistema]               VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovParametriVitali] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovParametriVitali_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovParametriVitali_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovParametriVitali_T_StatoParametroVitale] FOREIGN KEY ([CodStatoParametroVitale]) REFERENCES [dbo].[T_StatoParametroVitale] ([Codice]),
    CONSTRAINT [FK_T_MovParametriVitali_T_TipoParametroVitale] FOREIGN KEY ([CodTipoParametroVitale]) REFERENCES [dbo].[T_TipoParametroVitale] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovParametriVitali]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovParametriVitali]([CodTipoParametroVitale] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovParametriVitali]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovParametriVitali]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodioCodStatoParametroVitale]
    ON [dbo].[T_MovParametriVitali]([IDEpisodio] ASC, [CodStatoParametroVitale] ASC)
    INCLUDE([ID], [DataEvento]);

