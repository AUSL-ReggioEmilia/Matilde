CREATE TABLE [dbo].[T_MovAppuntamenti] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [CodUA]                   VARCHAR (20)     NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [DataInizio]              DATETIME         NULL,
    [DataFine]                DATETIME         NULL,
    [CodTipoAppuntamento]     VARCHAR (20)     NULL,
    [CodStatoAppuntamento]    VARCHAR (20)     NULL,
    [ElencoRisorse]           VARCHAR (2000)   NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [CodSistema]              VARCHAR (20)     NULL,
    [IDSistema]               VARCHAR (50)     NULL,
    [IDGruppo]                VARCHAR (50)     NULL,
    [InfoSistema]             VARCHAR (50)     NULL,
    [Titolo]                  VARCHAR (2000)   NULL,
    CONSTRAINT [PK_T_MovAppuntamenti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovAppuntamenti_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_StatoAppuntamento] FOREIGN KEY ([CodStatoAppuntamento]) REFERENCES [dbo].[T_StatoAppuntamento] ([Codice]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_TipoAppuntamento] FOREIGN KEY ([CodTipoAppuntamento]) REFERENCES [dbo].[T_TipoAppuntamento] ([Codice]),
    CONSTRAINT [FK_T_MovAppuntamenti_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_DataInizioDataFineCodStato]
    ON [dbo].[T_MovAppuntamenti]([DataInizio] ASC, [DataFine] ASC, [CodStatoAppuntamento] ASC)
    INCLUDE([ID], [CodUA], [IDPaziente], [IDEpisodio], [IDTrasferimento], [CodTipoAppuntamento]);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovAppuntamenti]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDSistemaCodSistema]
    ON [dbo].[T_MovAppuntamenti]([IDSistema] ASC, [CodSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSistema]
    ON [dbo].[T_MovAppuntamenti]([InfoSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDSistema]
    ON [dbo].[T_MovAppuntamenti]([IDSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDGruppo]
    ON [dbo].[T_MovAppuntamenti]([IDGruppo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataFine]
    ON [dbo].[T_MovAppuntamenti]([DataFine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio_CodStatoAppuntamento]
    ON [dbo].[T_MovAppuntamenti]([IDEpisodio] ASC, [CodStatoAppuntamento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stato]
    ON [dbo].[T_MovAppuntamenti]([CodStatoAppuntamento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovAppuntamenti]([CodTipoAppuntamento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataOraInizio]
    ON [dbo].[T_MovAppuntamenti]([DataInizio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Paziente]
    ON [dbo].[T_MovAppuntamenti]([IDPaziente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovAppuntamenti]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UA]
    ON [dbo].[T_MovAppuntamenti]([CodUA] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPazienteIDSistemaCodSistemaCodTipoAppuntamento]
    ON [dbo].[T_MovAppuntamenti]([IDPaziente] ASC, [IDSistema] ASC, [CodSistema] ASC, [CodTipoAppuntamento] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_DataInizioDataFineCodStatoAppuntamento]
    ON [dbo].[T_MovAppuntamenti]([DataInizio] ASC, [DataFine] ASC, [CodStatoAppuntamento] ASC)
    INCLUDE([ID], [CodUA], [IDPaziente], [IDEpisodio], [IDTrasferimento], [DataEvento], [CodTipoAppuntamento]);

