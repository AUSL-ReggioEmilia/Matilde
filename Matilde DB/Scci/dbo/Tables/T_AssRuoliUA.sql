CREATE TABLE [dbo].[T_AssRuoliUA] (
    [CodRuolo] VARCHAR (20) NOT NULL,
    [CodUA]    VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssRuoliUA] PRIMARY KEY CLUSTERED ([CodRuolo] ASC, [CodUA] ASC),
    CONSTRAINT [FK_T_AssRuoliUA_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice]),
    CONSTRAINT [FK_T_AssRuoliUA_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodRuolo]
    ON [dbo].[T_AssRuoliUA]([CodRuolo] ASC);

