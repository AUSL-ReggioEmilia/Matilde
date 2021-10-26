CREATE TABLE [dbo].[T_MovConsegnePaziente] (
    [ID]                       UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                    INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]               UNIQUEIDENTIFIER NULL,
    [CodUA]                    VARCHAR (20)     NULL,
    [CodRuoloInserimento]      VARCHAR (20)     NULL,
    [CodTipoConsegnaPaziente]  VARCHAR (20)     NULL,
    [CodStatoConsegnaPaziente] VARCHAR (20)     NULL,
    [CodUtenteInserimento]     VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]  VARCHAR (100)    NULL,
    [CodUtenteAnnullamento]    VARCHAR (100)    NULL,
    [CodUtenteCancellazione]   VARCHAR (100)    NULL,
    [DataInserimento]          DATETIME         NULL,
    [DataInserimentoUTC]       DATETIME         NULL,
    [DataUltimaModifica]       DATETIME         NULL,
    [DataUltimaModificaUTC]    DATETIME         NULL,
    [DataAnnullamento]         DATETIME         NULL,
    [DataAnnullamentoUTC]      DATETIME         NULL,
    [DataCancellazione]        DATETIME         NULL,
    [DataCancellazioneUTC]     DATETIME         NULL,
    CONSTRAINT [PK_T_MovConsegnePaziente] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_Login] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_Login_2] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_Login_3] FOREIGN KEY ([CodUtenteAnnullamento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_Ruoli] FOREIGN KEY ([CodRuoloInserimento]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_StatoConsegnaPaziente] FOREIGN KEY ([CodStatoConsegnaPaziente]) REFERENCES [dbo].[T_StatoConsegnaPaziente] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_TipoConsegnaPaziente] FOREIGN KEY ([CodTipoConsegnaPaziente]) REFERENCES [dbo].[T_TipoConsegnaPaziente] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePaziente_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio]
    ON [dbo].[T_MovConsegnePaziente]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuoloInserimento]
    ON [dbo].[T_MovConsegnePaziente]([CodRuoloInserimento] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovConsegnePaziente]([IDNum] ASC);

