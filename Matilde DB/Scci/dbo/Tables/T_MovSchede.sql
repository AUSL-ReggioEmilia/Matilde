CREATE TABLE [dbo].[T_MovSchede] (
    [ID]                            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                         INT              IDENTITY (1, 1) NOT NULL,
    [CodUA]                         VARCHAR (20)     NULL,
    [CodEntita]                     VARCHAR (20)     NULL,
    [IDEntita]                      UNIQUEIDENTIFIER NULL,
    [IDPaziente]                    UNIQUEIDENTIFIER NULL,
    [IDEpisodio]                    UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]               UNIQUEIDENTIFIER NULL,
    [CodScheda]                     VARCHAR (20)     NULL,
    [Versione]                      INT              NULL,
    [Numero]                        INT              NULL,
    [Dati]                          XML              NULL,
    [AnteprimaRTF]                  VARCHAR (MAX)    NULL,
    [AnteprimaTXT]                  VARCHAR (MAX)    NULL,
    [DatiObbligatoriMancantiRTF]    VARCHAR (MAX)    NULL,
    [DatiRilievoRTF]                VARCHAR (MAX)    NULL,
    [IDSchedaPadre]                 UNIQUEIDENTIFIER NULL,
    [Storicizzata]                  BIT              CONSTRAINT [DF_T_MovSchede_Storicizzata] DEFAULT ((0)) NULL,
    [CodStatoScheda]                VARCHAR (20)     NULL,
    [DataCreazione]                 DATETIME         NULL,
    [DataCreazioneUTC]              DATETIME         NULL,
    [CodUtenteRilevazione]          VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]       VARCHAR (100)    NULL,
    [DataUltimaModifica]            DATETIME         NULL,
    [DataUltimaModificaUTC]         DATETIME         NULL,
    [InEvidenza]                    BIT              NULL,
    [LenDatiRilievoRTF]             AS               (case when [DatiRilievoRTF] IS NOT NULL then len([DatiRilievoRTF]) else (0) end),
    [LenDatiObbligatoriMancantiRTF] AS               (case when [DatiObbligatoriMancantiRTF] IS NOT NULL then len([DatiObbligatoriMancantiRTF]) else (0) end),
    [Validabile]                    BIT              NULL,
    [Validata]                      BIT              NULL,
    [Riservata]                     BIT              NULL,
    [CodUtenteValidazione]          VARCHAR (100)    NULL,
    [DataValidazione]               DATETIME         NULL,
    [DataValidazioneUTC]            DATETIME         NULL,
    [CodRuoloRilevazione]           VARCHAR (20)     NULL,
    [CodStatoSchedaCalcolato]       VARCHAR (20)     NULL,
    [Revisione]                     BIT              NULL,
    [IDCartellaAmbulatoriale]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_T_MovSchede] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovSchede_T_Login_CodRuoloRilevazione] FOREIGN KEY ([CodRuoloRilevazione]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_MovSchede_T_Login_CodUtenteRilevazione] FOREIGN KEY ([CodUtenteRilevazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovSchede_T_Login_CodUtenteUltimaModifica] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovSchede_T_Login_CodUtenteValidazione] FOREIGN KEY ([CodUtenteValidazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovSchede_T_MovCartelleAmbulatoriali] FOREIGN KEY ([IDCartellaAmbulatoriale]) REFERENCES [dbo].[T_MovCartelleAmbulatoriali] ([ID]),
    CONSTRAINT [FK_T_MovSchede_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovSchede_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovSchede_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovSchede_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice]),
    CONSTRAINT [FK_T_MovSchede_T_StatoScheda] FOREIGN KEY ([CodStatoScheda]) REFERENCES [dbo].[T_StatoScheda] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_IDSchedaPadreCodEntitaCodSchedaStato]
    ON [dbo].[T_MovSchede]([IDSchedaPadre] ASC, [CodEntita] ASC, [CodScheda] ASC, [CodStatoScheda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEntitaCodEntitaCodSchedaStoricizzataStato]
    ON [dbo].[T_MovSchede]([IDEntita] ASC, [CodEntita] ASC, [CodScheda] ASC, [CodStatoScheda] ASC, [Storicizzata] ASC)
    INCLUDE([CodRuoloRilevazione]) WITH (FILLFACTOR = 90);




GO
CREATE NONCLUSTERED INDEX [IX_IDEntitaCodEntitaStoricizzataStato]
    ON [dbo].[T_MovSchede]([IDEntita] ASC, [CodEntita] ASC, [CodStatoScheda] ASC, [Storicizzata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SchedaPadre]
    ON [dbo].[T_MovSchede]([IDSchedaPadre] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataUltimaModificaDataCreazione]
    ON [dbo].[T_MovSchede]([DataUltimaModifica] ASC, [DataCreazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LenDatiObbligatoriMancantiRTF]
    ON [dbo].[T_MovSchede]([LenDatiObbligatoriMancantiRTF] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LenDatiRilievo]
    ON [dbo].[T_MovSchede]([Storicizzata] ASC, [CodStatoScheda] ASC, [LenDatiRilievoRTF] ASC)
    INCLUDE([ID], [CodEntita], [IDEntita], [IDPaziente], [IDEpisodio]);




GO



GO
CREATE NONCLUSTERED INDEX [IX_Gerarchia]
    ON [dbo].[T_MovSchede]([IDPaziente] ASC, [CodScheda] ASC, [CodStatoScheda] ASC, [Storicizzata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataUltimaModifica]
    ON [dbo].[T_MovSchede]([DataUltimaModifica] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataCreazione]
    ON [dbo].[T_MovSchede]([DataCreazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NumScheda]
    ON [dbo].[T_MovSchede]([CodEntita] ASC, [IDEntita] ASC, [CodScheda] ASC, [Numero] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTraferimento]
    ON [dbo].[T_MovSchede]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaID]
    ON [dbo].[T_MovSchede]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Scheda]
    ON [dbo].[T_MovSchede]([CodScheda] ASC, [Versione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovSchede]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Paziente]
    ON [dbo].[T_MovSchede]([IDPaziente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UA]
    ON [dbo].[T_MovSchede]([CodUA] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovSchede]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Include1]
    ON [dbo].[T_MovSchede]([DataUltimaModifica] DESC, [CodStatoScheda] ASC, [Storicizzata] ASC)
    INCLUDE([ID]);


GO
CREATE NONCLUSTERED INDEX [IX_IDSchedaPadreCodStatoStoricizzata]
    ON [dbo].[T_MovSchede]([IDSchedaPadre] ASC, [Storicizzata] ASC, [CodStatoScheda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDCartellaAmbulatoriale]
    ON [dbo].[T_MovSchede]([IDCartellaAmbulatoriale] ASC);

