CREATE TABLE [dbo].[T_StatoAllegatoScheda] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Ordine]      INT             NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_StatoAllegatoScheda] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

