CREATE TABLE [dbo].[T_MovAlertGenerici] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [CodTipoAlertGenerico]    VARCHAR (20)     NULL,
    [CodStatoAlertGenerico]   VARCHAR (20)     NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [CodUtenteVisto]          VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [DataVisto]               DATETIME         NULL,
    [DataVistoUTC]            DATETIME         NULL,
    CONSTRAINT [PK_T_MovAlertGenerici] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovAlertGenerici_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovAlertGenerici_T_StatoAlertGenerico] FOREIGN KEY ([CodStatoAlertGenerico]) REFERENCES [dbo].[T_StatoAlertGenerico] ([Codice]),
    CONSTRAINT [FK_T_MovAlertGenerici_T_TipoAlertGenerico] FOREIGN KEY ([CodTipoAlertGenerico]) REFERENCES [dbo].[T_TipoAlertGenerico] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Trasferimento]
    ON [dbo].[T_MovAlertGenerici]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovAlertGenerici]([CodTipoAlertGenerico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovAlertGenerici]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovAlertGenerici]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio_Include1]
    ON [dbo].[T_MovAlertGenerici]([IDEpisodio] ASC)
    INCLUDE([ID]);

