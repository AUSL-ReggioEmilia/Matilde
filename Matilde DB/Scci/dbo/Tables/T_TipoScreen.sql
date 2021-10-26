CREATE TABLE [dbo].[T_TipoScreen] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoScreen] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

