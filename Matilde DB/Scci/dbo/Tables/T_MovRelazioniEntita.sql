CREATE TABLE [dbo].[T_MovRelazioniEntita] (
    [IDNum]              INT              IDENTITY (1, 1) NOT NULL,
    [CodEntita]          VARCHAR (20)     NULL,
    [IDEntita]           UNIQUEIDENTIFIER NULL,
    [CodEntitaCollegata] VARCHAR (20)     NULL,
    [IDEntitaCollegata]  UNIQUEIDENTIFIER NULL,
    [Attributo1]         VARCHAR (200)    NULL,
    CONSTRAINT [PK_T_MovRelazioniEntita] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IXCodEntitaCodEntitaCollegataIDEntitaCollegata]
    ON [dbo].[T_MovRelazioniEntita]([CodEntita] ASC, [CodEntitaCollegata] ASC, [IDEntitaCollegata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Destinazione]
    ON [dbo].[T_MovRelazioniEntita]([CodEntitaCollegata] ASC, [IDEntitaCollegata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Origine]
    ON [dbo].[T_MovRelazioniEntita]([CodEntita] ASC, [IDEntita] ASC);

