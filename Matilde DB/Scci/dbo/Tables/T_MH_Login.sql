CREATE TABLE [dbo].[T_MH_Login] (
    [ID]                            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                         INT              IDENTITY (1, 1) NOT NULL,
    [Codice]                        VARCHAR (100)    NOT NULL,
    [PasswordAccesso]               VARBINARY (MAX)  NOT NULL,
    [IDPaziente]                    UNIQUEIDENTIFIER NOT NULL,
    [CodStatoMHLogin]               VARCHAR (20)     NOT NULL,
    [DataScadenza]                  DATETIME         NOT NULL,
    [DataScadenzaUTC]               DATETIME         NOT NULL,
    [CodUtenteCreazione]            VARCHAR (100)    NOT NULL,
    [DataCreazione]                 DATETIME         NOT NULL,
    [DataCreazioneUTC]              DATETIME         NOT NULL,
    [CodUtenteUltimaModifica]       VARCHAR (100)    NULL,
    [DataUltimaModifica]            DATETIME         NULL,
    [DataUltimaModificaUTC]         DATE             NULL,
    [DataUltimaModificaPassword]    DATETIME         NULL,
    [DataUltimaModificaPasswordUTC] DATETIME         NULL,
    [PrimoAccesso]                  BIT              NULL,
    CONSTRAINT [PK_T_MH_Login] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_MH_Login_T_Login] FOREIGN KEY ([CodUtenteCreazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_Login_T_Login1] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_Login_T_MH_Login] FOREIGN KEY ([Codice]) REFERENCES [dbo].[T_MH_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_Login_T_MH_StatoLogin] FOREIGN KEY ([CodStatoMHLogin]) REFERENCES [dbo].[T_MH_StatoLogin] ([Codice]),
    CONSTRAINT [FK_T_MH_Login_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MH_Login]([IDPaziente] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ID]
    ON [dbo].[T_MH_Login]([ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodiceCodStato]
    ON [dbo].[T_MH_Login]([Codice] ASC, [CodStatoMHLogin] ASC);

