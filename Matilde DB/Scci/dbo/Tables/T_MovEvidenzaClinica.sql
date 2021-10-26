CREATE TABLE [dbo].[T_MovEvidenzaClinica] (
    [ID]                             UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                          INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [IDEpisodio]                     UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]                UNIQUEIDENTIFIER NULL,
    [CodTipoEvidenzaClinica]         VARCHAR (20)     NULL,
    [CodStatoEvidenzaClinica]        VARCHAR (20)     NULL,
    [CodStatoEvidenzaClinicaVisione] VARCHAR (20)     NULL,
    [IDRefertoDWH]                   VARCHAR (50)     NULL,
    [NumeroRefertoDWH]               VARCHAR (50)     NULL,
    [DataEvento]                     DATETIME         NULL,
    [DataEventoUTC]                  DATETIME         NULL,
    [Anteprima]                      VARCHAR (MAX)    NULL,
    [PDFDWH]                         VARBINARY (MAX)  NULL,
    [CodUtenteInserimento]           VARCHAR (100)    NULL,
    [DataInserimento]                DATETIME         NULL,
    [DataInserimentoUTC]             DATETIME         NULL,
    [CodUtenteVisione]               VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]        VARCHAR (100)    NULL,
    [DataUltimaModifica]             DATETIME         NULL,
    [DataUltimaModificaUTC]          DATETIME         NULL,
    [DataVisione]                    DATETIME         NULL,
    [DataVisioneUTC]                 DATETIME         NULL,
    [DataEventoDWH]                  DATETIME         NULL,
    [DataEventoDWHUTC]               DATETIME         NULL,
    CONSTRAINT [PK_T_MovEvidenzaClinica] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovEvidenzaClinica_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovEvidenzaClinica_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovEvidenzaClinica_T_StatoEvidenzaClinica] FOREIGN KEY ([CodStatoEvidenzaClinica]) REFERENCES [dbo].[T_StatoEvidenzaClinica] ([Codice]),
    CONSTRAINT [FK_T_MovEvidenzaClinica_T_StatoEvidenzaClinicaVisione] FOREIGN KEY ([CodStatoEvidenzaClinicaVisione]) REFERENCES [dbo].[T_StatoEvidenzaClinicaVisione] ([Codice]),
    CONSTRAINT [FK_T_MovEvidenzaClinica_T_TipoEvidenzaClinica] FOREIGN KEY ([CodTipoEvidenzaClinica]) REFERENCES [dbo].[T_TipoEvidenzaClinica] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodioCodStatoEvidenzaCodStatoVisione]
    ON [dbo].[T_MovEvidenzaClinica]([IDEpisodio] ASC, [CodTipoEvidenzaClinica] ASC, [CodStatoEvidenzaClinicaVisione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDRefertoDWH]
    ON [dbo].[T_MovEvidenzaClinica]([IDRefertoDWH] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovEvidenzaClinica]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Data]
    ON [dbo].[T_MovEvidenzaClinica]([DataEvento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stato]
    ON [dbo].[T_MovEvidenzaClinica]([CodStatoEvidenzaClinica] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovEvidenzaClinica]([CodTipoEvidenzaClinica] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovEvidenzaClinica]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovEvidenzaClinica]([IDNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Data Ora Modifica Referto', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_MovEvidenzaClinica', @level2type = N'COLUMN', @level2name = N'PDFDWH';


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio_Include1]
    ON [dbo].[T_MovEvidenzaClinica]([IDEpisodio] ASC)
    INCLUDE([ID]);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoEvidenzaClinicaCodStatoEvidenzaClinicaVisione]
    ON [dbo].[T_MovEvidenzaClinica]([CodStatoEvidenzaClinica] ASC, [CodStatoEvidenzaClinicaVisione] ASC)
    INCLUDE([IDEpisodio]);

