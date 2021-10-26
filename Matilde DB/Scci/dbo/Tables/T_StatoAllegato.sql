CREATE TABLE [dbo].[T_StatoAllegato] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Ordine]      INT             NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_StatoAllegato] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

