CREATE TABLE [dbo].[T_TipoOrdine] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoOrdine] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

