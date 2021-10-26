CREATE TABLE [dbo].[T_StatoScheda] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_T_StatoScheda_1] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

