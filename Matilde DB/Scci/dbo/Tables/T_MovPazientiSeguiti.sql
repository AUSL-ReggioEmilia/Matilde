CREATE TABLE [dbo].[T_MovPazientiSeguiti] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [CodUtente]               VARCHAR (100)    NULL,
    [CodRuolo]                VARCHAR (20)     NULL,
    [CodStatoPazienteSeguito] VARCHAR (20)     NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovPazientiSeguiti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovPazientiSeguiti_T_LoginIns] FOREIGN KEY ([CodUtente]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovPazientiSeguiti_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovPazientiSeguiti_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_MovPazientiSeguiti_T_StatoPazientiSeguiti] FOREIGN KEY ([CodStatoPazienteSeguito]) REFERENCES [dbo].[T_StatoPazientiSeguiti] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_UtenteRuoloStato]
    ON [dbo].[T_MovPazientiSeguiti]([CodUtente] ASC, [CodRuolo] ASC, [CodStatoPazienteSeguito] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStato]
    ON [dbo].[T_MovPazientiSeguiti]([CodStatoPazienteSeguito] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuolo]
    ON [dbo].[T_MovPazientiSeguiti]([CodRuolo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUtente]
    ON [dbo].[T_MovPazientiSeguiti]([CodUtente] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPazienteCodStatoPazienteSeguito]
    ON [dbo].[T_MovPazientiSeguiti]([IDPaziente] ASC, [CodStatoPazienteSeguito] ASC);

