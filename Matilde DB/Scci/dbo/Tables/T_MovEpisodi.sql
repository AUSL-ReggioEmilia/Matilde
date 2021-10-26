CREATE TABLE [dbo].[T_MovEpisodi] (
    [ID]                             UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                          INT              IDENTITY (1, 1) NOT NULL,
    [CodAzi]                         VARCHAR (20)     NULL,
    [CodTipoEpisodio]                VARCHAR (20)     NULL,
    [CodStatoEpisodio]               VARCHAR (20)     NULL,
    [DataListaAttesa]                DATETIME         NULL,
    [DataListaAttesaUTC]             DATETIME         NULL,
    [DataRicovero]                   DATETIME         NULL,
    [DataRicoveroUTC]                DATETIME         NULL,
    [DataDimissione]                 DATETIME         NULL,
    [DataDimissioneUTC]              DATETIME         NULL,
    [NumeroNosologico]               VARCHAR (20)     NULL,
    [NumeroListaAttesa]              VARCHAR (20)     NULL,
    [DataAnnullamentoListaAttesa]    DATETIME         NULL,
    [DataAnnullamentoListaAttesaUTC] DATETIME         NULL,
    CONSTRAINT [PK_T_MovEpisodi] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovEpisodi_T_StatoEpisodio] FOREIGN KEY ([CodStatoEpisodio]) REFERENCES [dbo].[T_StatoEpisodio] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_DataRicovero]
    ON [dbo].[T_MovEpisodi]([DataRicovero] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodAziendaListaAttesa]
    ON [dbo].[T_MovEpisodi]([CodAzi] ASC, [NumeroListaAttesa] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodAziendaNosologico]
    ON [dbo].[T_MovEpisodi]([CodAzi] ASC, [NumeroNosologico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ListaAttesa]
    ON [dbo].[T_MovEpisodi]([NumeroListaAttesa] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Nosologico]
    ON [dbo].[T_MovEpisodi]([NumeroNosologico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovEpisodi]([CodTipoEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovEpisodi]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDNosologicoNumListaAttesa]
    ON [dbo].[T_MovEpisodi]([ID] ASC, [NumeroNosologico] ASC, [NumeroListaAttesa] ASC);

