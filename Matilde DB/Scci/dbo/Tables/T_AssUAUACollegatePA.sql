CREATE TABLE [dbo].[T_AssUAUACollegatePA] (
    [CodUA]          VARCHAR (20) NOT NULL,
    [CodUACollegata] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssUAUACollegatePA] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodUACollegata] ASC),
    CONSTRAINT [FK_T_AssUAUACollegatePA_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice]),
    CONSTRAINT [FK_T_AssUAUACollegatePA_T_UnitaAtomiche2] FOREIGN KEY ([CodUACollegata]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);

