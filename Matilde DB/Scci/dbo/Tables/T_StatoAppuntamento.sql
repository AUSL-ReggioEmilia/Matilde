CREATE TABLE [dbo].[T_StatoAppuntamento] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Ordine]      INT             NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Riservato]   BIT             NULL,
    CONSTRAINT [PK_T_StatoAppuntamento] PRIMARY KEY CLUSTERED ([Codice] ASC)
);



