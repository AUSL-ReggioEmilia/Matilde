CREATE TABLE [dbo].[T_StatoNoteAgende] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_StatoNoteAgende] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

