CREATE TABLE [dbo].[T_StatoTaskInfermieristico] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Visibile]    BIT             NULL,
    CONSTRAINT [PK_T_StatoTaskInfermieristico] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

