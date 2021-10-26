CREATE TABLE [dbo].[T_AssUAUACollegate] (
    [CodUA]          VARCHAR (20) NOT NULL,
    [CodUACollegata] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssUAUACollegate] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodUACollegata] ASC),
    CONSTRAINT [FK_T_AssUAUACollegate_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice]),
    CONSTRAINT [FK_T_AssUAUACollegate_T_UnitaAtomiche2] FOREIGN KEY ([CodUACollegata]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodUACollegata]
    ON [dbo].[T_AssUAUACollegate]([CodUACollegata] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_AssUAUACollegate]([CodUA] ASC);

