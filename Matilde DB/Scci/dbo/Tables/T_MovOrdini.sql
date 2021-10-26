CREATE TABLE [dbo].[T_MovOrdini] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [IDOrdineOE]              VARCHAR (50)     NULL,
    [NumeroOrdineOE]          VARCHAR (50)     NULL,
    [XMLOE]                   XML              NULL,
    [Eroganti]                VARCHAR (MAX)    NULL,
    [Prestazioni]             VARCHAR (MAX)    NULL,
    [CodStatoOrdine]          VARCHAR (20)     NULL,
    [CodUtenteInserimento]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [CodUtenteInoltro]        VARCHAR (100)    NULL,
    [DataProgrammazioneOE]    DATETIME         NULL,
    [DataProgrammazioneOEUTC] DATETIME         NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [DataInoltro]             DATETIME         NULL,
    [DataInoltroUTC]          DATETIME         NULL,
    [CodUAInserimento]        VARCHAR (20)     NULL,
    [CodUAUltimaModifica]     VARCHAR (20)     NULL,
    [CodPriorita]             VARCHAR (20)     NULL,
    [CodSistema]              VARCHAR (20)     NULL,
    [IDSistema]               VARCHAR (50)     NULL,
    [IDGruppo]                VARCHAR (50)     NULL,
    [InfoSistema]             VARCHAR (50)     NULL,
    [StrutturaDatiAccessori]  XML              NULL,
    [DatiDatiAccessori]       XML              NULL,
    [LayoutDatiAccessori]     XML              NULL,
    [InfoSistema2]            VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovOrdini] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovOrdini_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovOrdini_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovOrdini_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovOrdini_T_PrioritaOrdine] FOREIGN KEY ([CodPriorita]) REFERENCES [dbo].[T_PrioritaOrdine] ([Codice]),
    CONSTRAINT [FK_T_MovOrdini_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice]),
    CONSTRAINT [FK_T_MovOrdini_T_StatoOrdine] FOREIGN KEY ([CodStatoOrdine]) REFERENCES [dbo].[T_StatoOrdine] ([Codice]),
    CONSTRAINT [FK_T_MovOrdini_T_UnitaAtomiche_Inserimento] FOREIGN KEY ([CodUAInserimento]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice]),
    CONSTRAINT [FK_T_MovOrdini_T_UnitaAtomiche_Modifica] FOREIGN KEY ([CodUAUltimaModifica]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodPriorita]
    ON [dbo].[T_MovOrdini]([CodPriorita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataProgrammazione]
    ON [dbo].[T_MovOrdini]([DataProgrammazioneOE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NumeroOrdine]
    ON [dbo].[T_MovOrdini]([NumeroOrdineOE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSistema]
    ON [dbo].[T_MovOrdini]([InfoSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InfoSistema2]
    ON [dbo].[T_MovOrdini]([InfoSistema2] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDGruppo]
    ON [dbo].[T_MovOrdini]([IDGruppo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PazienteStato]
    ON [dbo].[T_MovOrdini]([IDPaziente] ASC, [CodStatoOrdine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataInoltro]
    ON [dbo].[T_MovOrdini]([DataInoltro] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUAInserimento]
    ON [dbo].[T_MovOrdini]([CodUAInserimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUAUltimaModifica]
    ON [dbo].[T_MovOrdini]([CodUAUltimaModifica] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataEvento]
    ON [dbo].[T_MovOrdini]([DataInoltro] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stato]
    ON [dbo].[T_MovOrdini]([CodStatoOrdine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Trasferimento]
    ON [dbo].[T_MovOrdini]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovOrdini]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Paziente]
    ON [dbo].[T_MovOrdini]([IDPaziente] ASC);


GO
CREATE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovOrdini]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDSistema]
    ON [dbo].[T_MovOrdini]([IDSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUtenteIDOrdineCodStatoOrdine]
    ON [dbo].[T_MovOrdini]([CodUtenteInserimento] ASC, [IDOrdineOE] ASC, [CodStatoOrdine] ASC)
    INCLUDE([ID], [IDPaziente], [IDEpisodio]);

