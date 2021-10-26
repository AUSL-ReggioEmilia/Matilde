CREATE TABLE [dbo].[T_AssUAEntita] (
    [CodUA]     VARCHAR (20) NOT NULL,
    [CodEntita] VARCHAR (20) NOT NULL,
    [CodVoce]   VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssUAEntita] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodEntita] ASC, [CodVoce] ASC),
    CONSTRAINT [FK_T_AssUAEntita_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_AssUAEntita_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_AssUAEntita]([CodUA] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Codice]
    ON [dbo].[T_AssUAEntita]([CodVoce] ASC);

