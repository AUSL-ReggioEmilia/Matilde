CREATE TABLE [dbo].[T_StatoFolder] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_StatoFolder] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

