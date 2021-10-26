CREATE TABLE [dbo].[T_StatoAlertGenerico] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_T_StatoAlertGenerico] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

