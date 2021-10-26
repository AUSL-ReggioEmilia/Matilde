CREATE TABLE [dbo].[T_StatoNote] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_StatoNote] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

