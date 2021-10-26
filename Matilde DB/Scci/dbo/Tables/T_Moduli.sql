CREATE TABLE [dbo].[T_Moduli] (
    [Codice]      VARCHAR (50)   NOT NULL,
    [Descrizione] VARCHAR (255)  NULL,
    [Note]        VARCHAR (4000) NULL,
    [Path]        VARCHAR (900)  NULL,
    [CodEntita]   VARCHAR (20)   NULL,
    CONSTRAINT [PK_T_Moduli] PRIMARY KEY NONCLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Moduli_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_Path]
    ON [dbo].[T_Moduli]([Path] ASC);

