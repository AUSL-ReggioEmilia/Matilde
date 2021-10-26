CREATE TABLE [dbo].[T_MovAlertAllergieAnamnesi] (
    [ID]                            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                         INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]                    UNIQUEIDENTIFIER NULL,
    [DataEvento]                    DATETIME         NULL,
    [DataEventoUTC]                 DATETIME         NULL,
    [CodTipoAlertAllergiaAnamnesi]  VARCHAR (20)     NULL,
    [CodStatoAlertAllergiaAnamnesi] VARCHAR (20)     NULL,
    [CodUtenteRilevazione]          VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]       VARCHAR (100)    NULL,
    [DataUltimaModifica]            DATETIME         NULL,
    [DataUltimaModificaUTC]         DATETIME         NULL,
    [CodSistema]                    VARCHAR (20)     NULL,
    [IDSistema]                     VARCHAR (50)     NULL,
    [IDGruppo]                      VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovAlertAllergieAnamnesi] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovAlertAllergieAnamnesi_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovAlertAllergieAnamnesi_T_StatoAlertAllergiaAnamnesi] FOREIGN KEY ([CodStatoAlertAllergiaAnamnesi]) REFERENCES [dbo].[T_StatoAlertAllergiaAnamnesi] ([Codice]),
    CONSTRAINT [FK_T_MovAlertAllergieAnamnesi_T_TipoAlertAllergiaAnamnesi] FOREIGN KEY ([CodTipoAlertAllergiaAnamnesi]) REFERENCES [dbo].[T_TipoAlertAllergiaAnamnesi] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Tipo]
    ON [dbo].[T_MovAlertAllergieAnamnesi]([CodTipoAlertAllergiaAnamnesi] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Paziente]
    ON [dbo].[T_MovAlertAllergieAnamnesi]([IDPaziente] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovAlertAllergieAnamnesi]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDSistema]
    ON [dbo].[T_MovAlertAllergieAnamnesi]([IDSistema] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodSistemaIDSistema]
    ON [dbo].[T_MovAlertAllergieAnamnesi]([CodSistema] ASC, [IDSistema] ASC);

