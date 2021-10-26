CREATE TABLE [dbo].[T_MovAllegati] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [NumeroDocumento]         VARCHAR (50)     NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [TestoRTF]                VARCHAR (MAX)    NULL,
    [NotaRTF]                 VARCHAR (MAX)    NULL,
    [CodFormatoAllegato]      VARCHAR (20)     NULL,
    [CodTipoAllegato]         VARCHAR (20)     NULL,
    [CodStatoAllegato]        VARCHAR (20)     NULL,
    [Documento]               VARBINARY (MAX)  NULL,
    [NomeFile]                VARCHAR (2000)   NULL,
    [Estensione]              VARCHAR (10)     NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataRilevazione]         DATETIME         NULL,
    [DataRilevazioneUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [CodUA]                   VARCHAR (20)     NULL,
    [CodEntita]               VARCHAR (20)     NULL,
    [IDFolder]                UNIQUEIDENTIFIER NULL,
    [TestoTXT]                VARCHAR (MAX)    NULL,
    [NotaTXT]                 VARCHAR (MAX)    NULL,
    [TestoTXTBreve]           AS               (CONVERT([varchar](900),left([TestoTXT],(900)),0)),
    CONSTRAINT [PK_T_MovAllegati] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovAllegati_T_EntitaAllegato] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_EntitaAllegato] ([Codice]),
    CONSTRAINT [FK_T_MovAllegati_T_FormatoAllegati] FOREIGN KEY ([CodFormatoAllegato]) REFERENCES [dbo].[T_FormatoAllegati] ([Codice]),
    CONSTRAINT [FK_T_MovAllegati_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovAllegati_T_MovFolder] FOREIGN KEY ([IDFolder]) REFERENCES [dbo].[T_MovFolder] ([ID]),
    CONSTRAINT [FK_T_MovAllegati_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovAllegati_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovAllegati_T_StatoAllegato] FOREIGN KEY ([CodStatoAllegato]) REFERENCES [dbo].[T_StatoAllegato] ([Codice]),
    CONSTRAINT [FK_T_MovAllegati_T_TipoAllegato] FOREIGN KEY ([CodTipoAllegato]) REFERENCES [dbo].[T_TipoAllegato] ([Codice]),
    CONSTRAINT [FK_T_MovAllegati_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_TestoTXTBreve]
    ON [dbo].[T_MovAllegati]([TestoTXTBreve] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDFolder]
    ON [dbo].[T_MovAllegati]([IDFolder] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntita]
    ON [dbo].[T_MovAllegati]([CodEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_MovAllegati]([CodUA] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NumeroDocumento]
    ON [dbo].[T_MovAllegati]([NumeroDocumento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MovAllegati]([IDPaziente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovAllegati]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio]
    ON [dbo].[T_MovAllegati]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovAllegati]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Formato]
    ON [dbo].[T_MovAllegati]([CodFormatoAllegato] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovAllegati]([CodTipoAllegato] ASC);

