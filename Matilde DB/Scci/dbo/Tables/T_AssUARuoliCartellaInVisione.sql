CREATE TABLE [dbo].[T_AssUARuoliCartellaInVisione] (
    [CodUA]    VARCHAR (20) NOT NULL,
    [CodRuolo] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssUARuoliCartellaInVisione] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodRuolo] ASC),
    CONSTRAINT [FK_AssUARuoliCartelleInVisione_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice]),
    CONSTRAINT [FK_T_AssUARuoliCartellaInVisione_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);

