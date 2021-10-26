CREATE TABLE [dbo].[T_Selezioni] (
    [Codice]                  VARCHAR (50)  NOT NULL,
    [Descrizione]             VARCHAR (255) NULL,
    [CodTipoSelezione]        VARCHAR (20)  NOT NULL,
    [Selezioni]               XML           NULL,
    [FlagSistema]             BIT           NULL,
    [CodUtenteInserimento]    VARCHAR (100) NULL,
    [CodRuoloInserimento]     VARCHAR (20)  NULL,
    [DataInserimento]         DATETIME      NULL,
    [DataInserimentoUTC]      DATETIME      NULL,
    [CodUtenteUltimaModifica] VARCHAR (100) NULL,
    [CodRuoloUltimaModifica]  VARCHAR (20)  NULL,
    [DataUltimaModifica]      DATETIME      NULL,
    [DataUltimaModificaUTC]   DATETIME      NULL,
    CONSTRAINT [PK_T_Selezioni] PRIMARY KEY NONCLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Selezioni_T_Login_CodUtenteInserimento] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_Selezioni_T_Login_CodUtenteUltimaModifica] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_Selezioni_T_Ruoli_CodRuoloInserimento] FOREIGN KEY ([CodRuoloInserimento]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_Selezioni_T_Ruoli_CodRuoloUltimaModifica] FOREIGN KEY ([CodRuoloUltimaModifica]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_Selezioni_T_TipoSelezione] FOREIGN KEY ([CodTipoSelezione]) REFERENCES [dbo].[T_TipoSelezione] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodTipoSelezione]
    ON [dbo].[T_Selezioni]([CodTipoSelezione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUtenteInserimento]
    ON [dbo].[T_Selezioni]([CodUtenteInserimento] ASC);

