CREATE TABLE [dbo].[T_StatoDiario] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Ordine]      NUMERIC (18)    NULL,
    CONSTRAINT [PK_StatoDiario] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

