CREATE TABLE [dbo].[T_TipoEpisodio] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoEpisodio] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

