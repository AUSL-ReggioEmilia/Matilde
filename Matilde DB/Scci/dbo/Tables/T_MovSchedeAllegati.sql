CREATE TABLE [dbo].[T_MovSchedeAllegati] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDScheda]                UNIQUEIDENTIFIER NULL,
    [CodCampo]                VARCHAR (255)    NULL,
    [IDGruppo]                VARCHAR (50)     NULL,
    [Anteprima]               VARBINARY (MAX)  NULL,
    [Documento]               VARBINARY (MAX)  NULL,
    [NomeFile]                VARCHAR (2000)   NULL,
    [Estensione]              VARCHAR (10)     NULL,
    [DescrizioneAllegato]     VARCHAR (255)    NULL,
    [DescrizioneCampo]        VARCHAR (255)    NULL,
    [CodTipoAllegatoScheda]   VARCHAR (20)     NULL,
    [CodStatoAllegatoScheda]  VARCHAR (20)     NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataRilevazione]         DATETIME         NULL,
    [DataRilevazioneUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [CodSezione]              VARCHAR (255)    NULL,
    [Sequenza]                INT              NULL,
    CONSTRAINT [PK_T_MovSchedeAllegati] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovSchedeAllegati_T_MovSchede] FOREIGN KEY ([IDScheda]) REFERENCES [dbo].[T_MovSchede] ([ID]),
    CONSTRAINT [FK_T_MovSchedeAllegati_T_StatoAllegatoScheda] FOREIGN KEY ([CodStatoAllegatoScheda]) REFERENCES [dbo].[T_StatoAllegatoScheda] ([Codice]),
    CONSTRAINT [FK_T_MovSchedeAllegati_T_TipoAllegatoScheda] FOREIGN KEY ([CodTipoAllegatoScheda]) REFERENCES [dbo].[T_TipoAllegatoScheda] ([Codice])
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovSchedeAllegati]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDSchedaCodStatoAllegatoScheda]
    ON [dbo].[T_MovSchedeAllegati]([IDScheda] ASC, [CodStatoAllegatoScheda] ASC);

