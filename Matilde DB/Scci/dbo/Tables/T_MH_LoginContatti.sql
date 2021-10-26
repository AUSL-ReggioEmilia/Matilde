CREATE TABLE [dbo].[T_MH_LoginContatti] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [CodMHLogin]              VARCHAR (100)    NOT NULL,
    [NumeroTelefono]          VARCHAR (100)    NOT NULL,
    [Descrizione]             VARCHAR (255)    NOT NULL,
    [CodStatoMHContatto]      VARCHAR (20)     NOT NULL,
    [CodUtenteCreazione]      VARCHAR (100)    NOT NULL,
    [DataCreazione]           DATETIME         NOT NULL,
    [DataCreazioneUTC]        DATETIME         NOT NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MH_LoginContatti] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MH_LoginContatti_T_Login] FOREIGN KEY ([CodUtenteCreazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginContatti_T_Login1] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginContatti_T_MH_Login] FOREIGN KEY ([CodMHLogin]) REFERENCES [dbo].[T_MH_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginContatti_T_MH_StatoContatto] FOREIGN KEY ([CodStatoMHContatto]) REFERENCES [dbo].[T_MH_StatoContatto] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodMHLoginCodStato]
    ON [dbo].[T_MH_LoginContatti]([CodMHLogin] ASC, [CodStatoMHContatto] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodMHLogin]
    ON [dbo].[T_MH_LoginContatti]([CodMHLogin] ASC);

