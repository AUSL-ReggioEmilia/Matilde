CREATE TABLE [dbo].[T_AssLoginRuoli] (
    [CodLogin] VARCHAR (100) NOT NULL,
    [CodRuolo] VARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_T_AssLoginRuoli] PRIMARY KEY CLUSTERED ([CodLogin] ASC, [CodRuolo] ASC),
    CONSTRAINT [FK_T_AssLoginRuoli_T_Login] FOREIGN KEY ([CodLogin]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_AssLoginRuoli_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodRuolo]
    ON [dbo].[T_AssLoginRuoli]([CodRuolo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodLogin]
    ON [dbo].[T_AssLoginRuoli]([CodLogin] ASC);

