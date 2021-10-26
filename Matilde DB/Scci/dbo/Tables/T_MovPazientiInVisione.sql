CREATE TABLE [dbo].[T_MovPazientiInVisione] (
    [ID]                        UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                     INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]                UNIQUEIDENTIFIER NULL,
    [CodRuoloInVisione]         VARCHAR (20)     NULL,
    [DataInizio]                DATETIME         NULL,
    [DataInizioUTC]             DATETIME         NULL,
    [DataFine]                  DATETIME         NULL,
    [DataFineUTC]               DATETIME         NULL,
    [CodStatoPazienteInVisione] VARCHAR (20)     NULL,
    [DataInserimento]           DATETIME         NULL,
    [DataInserimentoUTC]        DATETIME         NULL,
    [CodUtenteInserimento]      VARCHAR (100)    NULL,
    [DataUltimaModifica]        DATETIME         NULL,
    [DataUltimaModificaUTC]     DATETIME         NULL,
    [CodUtenteUltimaModifica]   VARCHAR (100)    NULL,
    CONSTRAINT [PK_T_MovPazientiInVisione] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovPazientiInVisione_T_LoginIns] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovPazientiInVisione_T_LoginMod] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovPazientiInVisione_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovPazientiInVisione_T_Ruoli] FOREIGN KEY ([CodRuoloInVisione]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_MovPazientiInVisione_T_StatoPazienteInVisione] FOREIGN KEY ([CodStatoPazienteInVisione]) REFERENCES [dbo].[T_StatoPazienteInVisione] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoPazienteInVisione]
    ON [dbo].[T_MovPazientiInVisione]([CodStatoPazienteInVisione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuoloInVisione]
    ON [dbo].[T_MovPazientiInVisione]([CodRuoloInVisione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MovPazientiInVisione]([IDPaziente] ASC);

