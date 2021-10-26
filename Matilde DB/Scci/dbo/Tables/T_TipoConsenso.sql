CREATE TABLE [dbo].[T_TipoConsenso] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_TipoConsenso] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

