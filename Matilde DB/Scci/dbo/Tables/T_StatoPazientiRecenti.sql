CREATE TABLE [dbo].[T_StatoPazientiRecenti] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_StatoPazientiRecenti] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

