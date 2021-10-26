CREATE TABLE [dbo].[T_SezioniFUT] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Ordine]      NUMERIC (18)    NULL,
    [CodEntita]   VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_SezioniFUT] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

