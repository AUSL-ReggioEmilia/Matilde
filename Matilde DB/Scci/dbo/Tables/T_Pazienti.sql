CREATE TABLE [dbo].[T_Pazienti] (
    [ID]                        UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                     INT              IDENTITY (1, 1) NOT NULL,
    [CodSAC]                    VARCHAR (50)     NULL,
    [Cognome]                   VARCHAR (255)    NOT NULL,
    [Nome]                      VARCHAR (255)    NOT NULL,
    [Sesso]                     CHAR (1)         NULL,
    [DataNascita]               DATETIME         NULL,
    [CodiceFiscale]             VARCHAR (20)     NULL,
    [CodComuneNascita]          VARCHAR (10)     NULL,
    [ComuneNascita]             VARCHAR (255)    NULL,
    [LocalitaNascita]           VARCHAR (255)    NULL,
    [CodProvinciaNascita]       VARCHAR (10)     NULL,
    [ProvinciaNascita]          VARCHAR (50)     NULL,
    [CAPDomicilio]              VARCHAR (10)     NULL,
    [CodComuneDomicilio]        VARCHAR (10)     NULL,
    [ComuneDomicilio]           VARCHAR (255)    NULL,
    [IndirizzoDomicilio]        VARCHAR (255)    NULL,
    [LocalitaDomicilio]         VARCHAR (255)    NULL,
    [CodProvinciaDomicilio]     VARCHAR (10)     NULL,
    [ProvinciaDomicilio]        VARCHAR (50)     NULL,
    [CodRegioneDomicilio]       VARCHAR (10)     NULL,
    [RegioneDomicilio]          VARCHAR (50)     NULL,
    [CAPResidenza]              VARCHAR (10)     NULL,
    [CodComuneResidenza]        VARCHAR (10)     NULL,
    [ComuneResidenza]           VARCHAR (255)    NULL,
    [IndirizzoResidenza]        VARCHAR (255)    NULL,
    [LocalitaResidenza]         VARCHAR (255)    NULL,
    [CodProvinciaResidenza]     VARCHAR (10)     NULL,
    [ProvinciaResidenza]        VARCHAR (50)     NULL,
    [CodRegioneResidenza]       VARCHAR (10)     NULL,
    [RegioneResidenza]          VARCHAR (50)     NULL,
    [Foto]                      VARBINARY (MAX)  NULL,
    [CodMedicoBase]             VARCHAR (50)     NULL,
    [CodFiscMedicoBase]         VARCHAR (20)     NULL,
    [CognomeNomeMedicoBase]     VARCHAR (255)    NULL,
    [DistrettoMedicoBase]       VARCHAR (255)    NULL,
    [DataSceltaMedicoBase]      DATETIME         NULL,
    [ElencoEsenzioni]           VARCHAR (MAX)    NULL,
    [IDPazienteFuso]            UNIQUEIDENTIFIER NULL,
    [CodSACFuso]                VARCHAR (50)     NULL,
    [DataDecesso]               DATETIME         NULL,
    [CodStatoConsensoCalcolato] VARCHAR (20)     NULL,
    CONSTRAINT [PK_T_Pazienti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_Pazienti_T_StatoConsensoCalcolato] FOREIGN KEY ([CodStatoConsensoCalcolato]) REFERENCES [dbo].[T_StatoConsensoCalcolato] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodSACFuso]
    ON [dbo].[T_Pazienti]([CodSACFuso] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPazienteFuso]
    ON [dbo].[T_Pazienti]([IDPazienteFuso] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodSAC]
    ON [dbo].[T_Pazienti]([CodSAC] ASC);

