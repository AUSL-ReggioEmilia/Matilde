CREATE TABLE [dbo].[T_AssEveSis] (
    [CodEvento]  VARCHAR (50) NOT NULL,
    [CodSistema] VARCHAR (50) NOT NULL,
    [Attivo]     BIT          NULL,
    CONSTRAINT [PK_T_AssEveSis] PRIMARY KEY CLUSTERED ([CodEvento] ASC, [CodSistema] ASC),
    CONSTRAINT [FK_T_AssEveSis_T_Eventi] FOREIGN KEY ([CodEvento]) REFERENCES [dbo].[T_Eventi] ([Codice]),
    CONSTRAINT [FK_T_AssEveSis_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice])
);

