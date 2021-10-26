CREATE TABLE [dbo].[T_StatoTrasferimento] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Ordine]      INT             NULL,
    CONSTRAINT [PK_T_StatoTrasferimento] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

