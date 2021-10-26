CREATE TABLE [dbo].[T_AssRuoliModuli] (
    [CodRuolo]  VARCHAR (20) NOT NULL,
    [CodModulo] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_T_AssRuoliModuli] PRIMARY KEY CLUSTERED ([CodRuolo] ASC, [CodModulo] ASC),
    CONSTRAINT [FK_T_AssRuoliModuli_T_Moduli] FOREIGN KEY ([CodModulo]) REFERENCES [dbo].[T_Moduli] ([Codice]),
    CONSTRAINT [FK_T_AssRuoliModuli_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodModulo]
    ON [dbo].[T_AssRuoliModuli]([CodModulo] ASC);

