CREATE TABLE [dbo].[T_Sistemi] (
    [Codice]      VARCHAR (50)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Attivo]      BIT           NULL,
    [Nome]        VARCHAR (50)  NULL,
    [Versione]    VARCHAR (50)  NULL,
    [Parametri]   XML           NULL,
    CONSTRAINT [PK_T_Sistemi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

