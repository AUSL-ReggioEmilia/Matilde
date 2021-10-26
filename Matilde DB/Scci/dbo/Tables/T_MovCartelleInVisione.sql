CREATE TABLE [dbo].[T_MovCartelleInVisione] (
    [ID]                        UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                     INT              IDENTITY (1, 1) NOT NULL,
    [IDCartella]                UNIQUEIDENTIFIER NULL,
    [IDEpisodio]                UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]           UNIQUEIDENTIFIER NULL,
    [CodRuoloInVisione]         VARCHAR (20)     NULL,
    [DataInizio]                DATETIME         NULL,
    [DataInizioUTC]             DATETIME         NULL,
    [DataFine]                  DATETIME         NULL,
    [DataFineUTC]               DATETIME         NULL,
    [CodStatoCartellaInVisione] VARCHAR (20)     NULL,
    [DataInserimento]           DATETIME         NULL,
    [DataInserimentoUTC]        DATETIME         NULL,
    [CodUtenteInserimento]      VARCHAR (100)    NULL,
    [DataUltimaModifica]        DATETIME         NULL,
    [DataUltimaModificaUTC]     DATETIME         NULL,
    [CodUtenteUltimaModifica]   VARCHAR (100)    NULL,
    CONSTRAINT [PK_T_MovCartelleInVisione] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_LoginIns] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_LoginMod] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_MovCartelle] FOREIGN KEY ([IDCartella]) REFERENCES [dbo].[T_MovCartelle] ([ID]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_Ruoli] FOREIGN KEY ([CodRuoloInVisione]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleInVisione_T_StatoCartellaInVisione] FOREIGN KEY ([CodStatoCartellaInVisione]) REFERENCES [dbo].[T_StatoCartellaInVisione] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [Ix_IDCartellaCodStatoCartellaInVisione]
    ON [dbo].[T_MovCartelleInVisione]([IDCartella] ASC, [CodStatoCartellaInVisione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuoloInVisioneCodStatoCartellaInVisioneDate]
    ON [dbo].[T_MovCartelleInVisione]([CodRuoloInVisione] ASC, [CodStatoCartellaInVisione] ASC, [DataInizio] ASC, [DataFine] ASC)
    INCLUDE([IDCartella]);

