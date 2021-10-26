CREATE TABLE [dbo].[T_AssUAModuli] (
    [CodUA]     VARCHAR (20) NOT NULL,
    [CodModulo] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_T_AssUAModuli] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodModulo] ASC),
    CONSTRAINT [FK_T_AssUAModuli_T_ModuliUA] FOREIGN KEY ([CodModulo]) REFERENCES [dbo].[T_ModuliUA] ([Codice]),
    CONSTRAINT [FK_T_AssUAModuli_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);

