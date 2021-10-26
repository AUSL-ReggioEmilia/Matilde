CREATE TABLE [dbo].[T_ModuliUA] (
    [Codice]      VARCHAR (50)   NOT NULL,
    [Descrizione] VARCHAR (255)  NULL,
    [Note]        VARCHAR (4000) NULL,
    [Path]        VARCHAR (900)  NULL,
    [CodEntita]   VARCHAR (20)   NULL,
    CONSTRAINT [PK_T_ModuliUA] PRIMARY KEY NONCLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_ModuliUA_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);

