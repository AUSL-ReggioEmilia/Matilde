CREATE TABLE [dbo].[T_MovTaskInfermieristici] (
    [ID]                          UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                       INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]                  UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]             UNIQUEIDENTIFIER NULL,
    [DataEvento]                  DATETIME         NULL,
    [DataEventoUTC]               DATETIME         NULL,
    [CodSistema]                  VARCHAR (20)     NULL,
    [IDSistema]                   VARCHAR (50)     NULL,
    [IDGruppo]                    VARCHAR (50)     NULL,
    [IDTaskIniziale]              UNIQUEIDENTIFIER NULL,
    [CodTipoTaskInfermieristico]  VARCHAR (20)     NULL,
    [CodStatoTaskInfermieristico] VARCHAR (20)     NULL,
    [CodTipoRegistrazione]        VARCHAR (20)     NULL,
    [CodProtocollo]               VARCHAR (20)     NULL,
    [CodProtocolloTempo]          VARCHAR (20)     NULL,
    [DataProgrammata]             DATETIME         NULL,
    [DataProgrammataUTC]          DATETIME         NULL,
    [DataErogazione]              DATETIME         NULL,
    [DataErogazioneUTC]           DATETIME         NULL,
    [Sottoclasse]                 VARCHAR (512)    NULL,
    [Note]                        VARCHAR (4000)   NULL,
    [DescrizioneFUT]              VARCHAR (500)    NULL,
    [CodUtenteRilevazione]        VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]     VARCHAR (100)    NULL,
    [DataUltimaModifica]          DATETIME         NULL,
    [DataUltimaModificaUTC]       DATETIME         NULL,
    [PosologiaEffettiva]          VARCHAR (2000)   NULL,
    [Alert]                       VARCHAR (2000)   NULL,
    [Barcode]                     VARCHAR (100)    NULL,
    CONSTRAINT [PK_T_MovTaskInfermieristici] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovTaskInfermieristici_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovTaskInfermieristici_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice]),
    CONSTRAINT [FK_T_MovTaskInfermieristici_T_StatoTaskInfermieristico] FOREIGN KEY ([CodStatoTaskInfermieristico]) REFERENCES [dbo].[T_StatoTaskInfermieristico] ([Codice]),
    CONSTRAINT [FK_T_MovTaskInfermieristici_T_TipoRegistrazione] FOREIGN KEY ([CodTipoRegistrazione]) REFERENCES [dbo].[T_TipoRegistrazione] ([Codice]),
    CONSTRAINT [FK_T_MovTaskInfermieristici_T_TipoTaskInfermieristico] FOREIGN KEY ([CodTipoTaskInfermieristico]) REFERENCES [dbo].[T_TipoTaskInfermieristico] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Barcode]
    ON [dbo].[T_MovTaskInfermieristici]([Barcode] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDGruppo]
    ON [dbo].[T_MovTaskInfermieristici]([IDGruppo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTaskIniziale]
    ON [dbo].[T_MovTaskInfermieristici]([IDTaskIniziale] ASC);


GO
CREATE NONCLUSTERED INDEX [DataProgrammata]
    ON [dbo].[T_MovTaskInfermieristici]([DataProgrammata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SistemaIDSistema]
    ON [dbo].[T_MovTaskInfermieristici]([CodSistema] ASC, [IDSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Trasferimento]
    ON [dbo].[T_MovTaskInfermieristici]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Data]
    ON [dbo].[T_MovTaskInfermieristici]([DataProgrammata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TipoRegistrazione]
    ON [dbo].[T_MovTaskInfermieristici]([CodTipoRegistrazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stato]
    ON [dbo].[T_MovTaskInfermieristici]([CodStatoTaskInfermieristico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovTaskInfermieristici]([CodTipoTaskInfermieristico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovTaskInfermieristici]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovTaskInfermieristici]([IDNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Conterrà in caso di annullamento la "Motivazione dell''annullamento" o in caso di Erogazione le note di "Erogazione"', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_MovTaskInfermieristici', @level2type = N'COLUMN', @level2name = N'Note';


GO
CREATE NONCLUSTERED INDEX [IX_IDSistema]
    ON [dbo].[T_MovTaskInfermieristici]([IDSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio_Include1]
    ON [dbo].[T_MovTaskInfermieristici]([IDEpisodio] ASC)
    INCLUDE([ID]);


GO
CREATE NONCLUSTERED INDEX [IX_CodSistemaCodStatoTaskInfermieristico]
    ON [dbo].[T_MovTaskInfermieristici]([CodSistema] ASC, [CodStatoTaskInfermieristico] ASC)
    INCLUDE([IDGruppo]);

