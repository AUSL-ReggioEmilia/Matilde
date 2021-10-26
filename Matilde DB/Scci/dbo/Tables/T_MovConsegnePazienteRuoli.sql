CREATE TABLE [dbo].[T_MovConsegnePazienteRuoli] (
    [ID]                            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                         INT              IDENTITY (1, 1) NOT NULL,
    [IDConsegnaPaziente]            UNIQUEIDENTIFIER NOT NULL,
    [CodStatoConsegnaPazienteRuolo] VARCHAR (20)     NULL,
    [CodRuolo]                      VARCHAR (20)     NULL,
    [CodUtenteInserimento]          VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]       VARCHAR (100)    NULL,
    [CodUtenteAnnullamento]         VARCHAR (100)    NULL,
    [CodUtenteCancellazione]        VARCHAR (100)    NULL,
    [CodUtenteVisione]              VARCHAR (100)    NULL,
    [DataInserimento]               DATETIME         NULL,
    [DataInserimentoUTC]            DATETIME         NULL,
    [DataUltimaModifica]            DATETIME         NULL,
    [DataUltimaModificaUTC]         DATETIME         NULL,
    [DataAnnullamento]              DATETIME         NULL,
    [DataAnnullamentoUTC]           DATETIME         NULL,
    [DataCancellazione]             DATETIME         NULL,
    [DataCancellazioneUTC]          DATETIME         NULL,
    [DataVisione]                   DATETIME         NULL,
    [DataVisioneUTC]                DATETIME         NULL,
    CONSTRAINT [PK_T_MovConsegnePazienteRuoli] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovConsegnePazienteRuoli_T_Login] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePazienteRuoli_T_Login_2] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePazienteRuoli_T_Login_3] FOREIGN KEY ([CodUtenteAnnullamento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePazienteRuoli_T_Login_4] FOREIGN KEY ([CodUtenteVisione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegnePazienteRuoli_T_StatoConsegnaPazienteRuoli] FOREIGN KEY ([CodStatoConsegnaPazienteRuolo]) REFERENCES [dbo].[T_StatoConsegnaPazienteRuoli] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_DataInserimento]
    ON [dbo].[T_MovConsegnePazienteRuoli]([DataInserimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuolo]
    ON [dbo].[T_MovConsegnePazienteRuoli]([CodRuolo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDConsengaPaziente]
    ON [dbo].[T_MovConsegnePazienteRuoli]([IDConsegnaPaziente] ASC);


GO
CREATE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovConsegnePazienteRuoli]([IDNum] ASC);

