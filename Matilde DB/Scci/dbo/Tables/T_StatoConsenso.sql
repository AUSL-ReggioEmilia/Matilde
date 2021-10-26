CREATE TABLE [dbo].[T_StatoConsenso] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_StatoConsenso] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

