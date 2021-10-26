CREATE TABLE [dbo].[T_AssRuoliSelezioni] (
    [CodRuolo]     VARCHAR (20) NOT NULL,
    [CodSelezione] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_T_AssRuoliSelezioni] PRIMARY KEY CLUSTERED ([CodRuolo] ASC, [CodSelezione] ASC),
    CONSTRAINT [FK_T_AssRuoliSelezioni_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_AssRuoliSelezioni_T_Selezioni] FOREIGN KEY ([CodSelezione]) REFERENCES [dbo].[T_Selezioni] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodSelezione]
    ON [dbo].[T_AssRuoliSelezioni]([CodSelezione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodRuolo]
    ON [dbo].[T_AssRuoliSelezioni]([CodRuolo] ASC);

