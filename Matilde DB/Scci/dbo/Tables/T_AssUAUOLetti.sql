CREATE TABLE [dbo].[T_AssUAUOLetti] (
    [CodUA]      VARCHAR (20) NOT NULL,
    [CodAzi]     VARCHAR (20) NOT NULL,
    [CodUO]      VARCHAR (20) NOT NULL,
    [CodSettore] VARCHAR (20) NOT NULL,
    [CodLetto]   VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssUAUOLetti] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodAzi] ASC, [CodUO] ASC, [CodSettore] ASC, [CodLetto] ASC),
    CONSTRAINT [FK_T_AssUAUOLetti_T_Aziende] FOREIGN KEY ([CodAzi]) REFERENCES [dbo].[T_Aziende] ([Codice]),
    CONSTRAINT [FK_T_AssUAUOLetti_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_AssUAUOLetti]([CodUA] ASC);

