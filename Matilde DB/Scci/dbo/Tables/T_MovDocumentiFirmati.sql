CREATE TABLE [dbo].[T_MovDocumentiFirmati] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [CodEntita]               VARCHAR (20)     NULL,
    [IDEntita]                UNIQUEIDENTIFIER NULL,
    [CodTipoDocumentoFirmato] VARCHAR (20)     NULL,
    [Numero]                  INT              NULL,
    [PDFFirmato]              VARBINARY (MAX)  NULL,
    [CodUtenteInserimento]    VARCHAR (100)    NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [FlagEsportato]           BIT              NULL,
    [DataEsportazione]        DATETIME         NULL,
    [NomeFileEsportatoP7M]    VARCHAR (100)    NULL,
    [NomeFileEsportatoXML]    VARCHAR (100)    NULL,
    [CodStatoEntita]          VARCHAR (20)     NULL,
    [PDFNonFirmato]           VARBINARY (MAX)  NULL,
    CONSTRAINT [PK_MovDocumentiFirmatie] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovDocumentiFirmati_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovDocumentiFirmati_T_Login] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovDocumentiFirmati_T_TipoDocumentoFirmato] FOREIGN KEY ([CodTipoDocumentoFirmato]) REFERENCES [dbo].[T_TipoDocumentoFirmato] ([Codice])
);






GO
CREATE NONCLUSTERED INDEX [IX_IDEntita]
    ON [dbo].[T_MovDocumentiFirmati]([IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaCodID]
    ON [dbo].[T_MovDocumentiFirmati]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovDocumentiFirmati]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaCodStatoEntita]
    ON [dbo].[T_MovDocumentiFirmati]([CodEntita] ASC, [CodStatoEntita] ASC);

