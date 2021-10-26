CREATE TABLE [dbo].[T_TipoAgenda] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoAgenda] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

