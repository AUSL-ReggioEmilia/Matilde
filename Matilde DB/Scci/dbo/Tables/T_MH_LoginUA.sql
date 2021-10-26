CREATE TABLE [dbo].[T_MH_LoginUA] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [CodMHLogin]              VARCHAR (100)    NULL,
    [CodUA]                   VARCHAR (20)     NULL,
    [CodStatoMHLoginUA]       VARCHAR (20)     NULL,
    [CodUtenteCreazione]      VARCHAR (100)    NULL,
    [DataCreazione]           DATETIME         NULL,
    [DataCreazioneUTC]        DATETIME         NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MH_LoginUA] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MH_LoginUA_T_Login] FOREIGN KEY ([CodUtenteCreazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginUA_T_Login1] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginUA_T_MH_Login] FOREIGN KEY ([CodMHLogin]) REFERENCES [dbo].[T_MH_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginUA_T_MH_StatoLoginUA] FOREIGN KEY ([CodStatoMHLoginUA]) REFERENCES [dbo].[T_MH_StatoLoginUA] ([Codice]),
    CONSTRAINT [FK_T_MH_LoginUA_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodMHLoginCodStato]
    ON [dbo].[T_MH_LoginUA]([CodMHLogin] ASC, [CodStatoMHLoginUA] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodMHLogin]
    ON [dbo].[T_MH_LoginUA]([CodMHLogin] ASC);

