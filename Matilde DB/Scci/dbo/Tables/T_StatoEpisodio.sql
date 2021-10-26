CREATE TABLE [dbo].[T_StatoEpisodio] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_StatoEpisodio] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

