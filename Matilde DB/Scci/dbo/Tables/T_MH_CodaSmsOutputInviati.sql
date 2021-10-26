CREATE TABLE [dbo].[T_MH_CodaSmsOutputInviati] (
    [Id]                         UNIQUEIDENTIFIER CONSTRAINT [DF_T_MH_CodaSmsOutputInviati_Id] DEFAULT (newsequentialid()) NOT NULL,
    [IdSequenza]                 INT              NOT NULL,
    [DataInserimento]            DATETIME         NOT NULL,
    [Messaggio]                  VARCHAR (512)    NOT NULL,
    [NumeroTelefonoDestinatario] VARCHAR (64)     NOT NULL,
    [Tipologia]                  VARCHAR (64)     NOT NULL,
    [ServerName]                 VARCHAR (64)     NOT NULL,
    [DataInvio]                  DATETIME         NOT NULL,
    [DataCompletamento]          DATETIME         NULL,
    [Stato]                      VARCHAR (64)     NULL,
    [Errore]                     VARCHAR (2048)   NULL,
    CONSTRAINT [PK_T_MH_CodaSmsOutputInviati] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 95)
);


GO
CREATE NONCLUSTERED INDEX [IX_T_MH_CodaSmsOutputInviati_IdSequenza]
    ON [dbo].[T_MH_CodaSmsOutputInviati]([IdSequenza] ASC) WITH (FILLFACTOR = 95);

