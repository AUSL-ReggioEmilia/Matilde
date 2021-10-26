CREATE TABLE [dbo].[T_MovPrescrizioniTempi] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                      INT              IDENTITY (1, 1) NOT NULL,
    [IDPrescrizione]             UNIQUEIDENTIFIER NULL,
    [Posologia]                  VARCHAR (2000)   NULL,
    [CodStatoPrescrizioneTempi]  VARCHAR (20)     NULL,
    [CodTipoPrescrizioneTempi]   VARCHAR (20)     NULL,
    [DataEvento]                 DATETIME         NULL,
    [DataEventoUTC]              DATETIME         NULL,
    [DataOraInizio]              DATETIME         NULL,
    [DataOraFine]                DATETIME         NULL,
    [AlBisogno]                  BIT              NULL,
    [Durata]                     INT              NULL,
    [Continuita]                 BIT              NULL,
    [PeriodicitaGiorni]          INT              NULL,
    [PeriodicitaOre]             INT              NULL,
    [PeriodicitaMinuti]          INT              NULL,
    [CodProtocollo]              VARCHAR (20)     NULL,
    [CodUtenteRilevazione]       VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]    VARCHAR (100)    NULL,
    [CodUtenteValidazione]       VARCHAR (100)    NULL,
    [CodUtenteSospensione]       VARCHAR (100)    NULL,
    [DataValidazione]            DATETIME         NULL,
    [DataValidazioneUTC]         DATETIME         NULL,
    [DataSospensione]            DATETIME         NULL,
    [DataSospensioneUTC]         DATETIME         NULL,
    [DataUltimaModifica]         DATETIME         NULL,
    [DataUltimaModificaUTC]      DATETIME         NULL,
    [Manuale]                    BIT              NULL,
    [TempiManuali]               XML              NULL,
    [IDPrescrizioneTempiOrigine] UNIQUEIDENTIFIER NULL,
    [IDString]                   VARCHAR (50)     NULL,
    [IDPrescrizioneString]       VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovPrescrizioniTempi] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovPrescrizioniTempi_T_MovPrescrizioni] FOREIGN KEY ([IDPrescrizione]) REFERENCES [dbo].[T_MovPrescrizioni] ([ID]),
    CONSTRAINT [FK_T_MovPrescrizioniTempi_T_Protocolli] FOREIGN KEY ([CodProtocollo]) REFERENCES [dbo].[T_Protocolli] ([Codice]),
    CONSTRAINT [FK_T_MovPrescrizioniTempi_T_StatoPrescrizioneTempi] FOREIGN KEY ([CodStatoPrescrizioneTempi]) REFERENCES [dbo].[T_StatoPrescrizioneTempi] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDPrescrizioneString]
    ON [dbo].[T_MovPrescrizioniTempi]([IDPrescrizioneString] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDString]
    ON [dbo].[T_MovPrescrizioniTempi]([IDString] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPrescrizioneCodStato]
    ON [dbo].[T_MovPrescrizioniTempi]([IDPrescrizione] ASC, [CodStatoPrescrizioneTempi] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Prescrizione]
    ON [dbo].[T_MovPrescrizioniTempi]([IDPrescrizione] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovPrescrizioniTempi]([IDNum] ASC);

