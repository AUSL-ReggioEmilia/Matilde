CREATE TABLE [dbo].[T_Azioni] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_Azioni] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

