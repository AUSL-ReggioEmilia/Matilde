CREATE TABLE [dbo].[T_Stanze] (
    [CodAzi]      VARCHAR (20)  NOT NULL,
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_Stanze_1] PRIMARY KEY CLUSTERED ([CodAzi] ASC, [Codice] ASC),
    CONSTRAINT [FK_T_Stanze_T_Aziende] FOREIGN KEY ([CodAzi]) REFERENCES [dbo].[T_Aziende] ([Codice])
);

