CREATE TABLE [dbo].[T_TipoNews] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoNews] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

