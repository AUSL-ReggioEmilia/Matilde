CREATE TABLE [dbo].[T_MovCodeEntita] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDCoda]                  UNIQUEIDENTIFIER NOT NULL,
    [CodEntita]               VARCHAR (20)     NULL,
    [IDEntita]                UNIQUEIDENTIFIER NULL,
    [CodStatoCodaEntita]      VARCHAR (20)     NULL,
    [CodUtenteChiamata]       VARCHAR (100)    NULL,
    [CodUtenteInserimento]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataChiamata]            DATETIME         NULL,
    [DataChiamataUTC]         DATETIME         NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovCodeEntita] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovCodeEntita_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovCodeEntita_T_Login1] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCodeEntita_T_Login2] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCodeEntita_T_Login3] FOREIGN KEY ([CodUtenteChiamata]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCodeEntita_T_MovCode] FOREIGN KEY ([IDCoda]) REFERENCES [dbo].[T_MovCode] ([ID]),
    CONSTRAINT [FK_T_MovCodeEntita_T_StatoCodaEntita] FOREIGN KEY ([CodStatoCodaEntita]) REFERENCES [dbo].[T_StatoCodaEntita] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaIDEntita]
    ON [dbo].[T_MovCodeEntita]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoCodaEntita]
    ON [dbo].[T_MovCodeEntita]([CodStatoCodaEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovCodeEntita]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entita]
    ON [dbo].[T_MovCodeEntita]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Coda]
    ON [dbo].[T_MovCodeEntita]([IDCoda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEntitaCodStatoCodaEntita]
    ON [dbo].[T_MovCodeEntita]([IDEntita] ASC, [CodStatoCodaEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaCodStatoCodaEntitaDataChiamata]
    ON [dbo].[T_MovCodeEntita]([CodEntita] ASC, [CodStatoCodaEntita] ASC, [DataChiamata] ASC)
    INCLUDE([IDNum], [IDCoda], [IDEntita]);

