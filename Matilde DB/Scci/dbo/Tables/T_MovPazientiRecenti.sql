CREATE TABLE [dbo].[T_MovPazientiRecenti] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [CodUtente]               VARCHAR (100)    NULL,
    [CodStatoPazienteRecente] VARCHAR (20)     NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovPazientiRecenti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovPazientiRecenti_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_MovPazientiRecenti_T_StatoPazientiRecenti] FOREIGN KEY ([CodStatoPazienteRecente]) REFERENCES [dbo].[T_StatoPazientiRecenti] ([Codice])
);

