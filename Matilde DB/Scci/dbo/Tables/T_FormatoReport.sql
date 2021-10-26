CREATE TABLE [dbo].[T_FormatoReport] (
    [Codice]         VARCHAR (20)    NOT NULL,
    [Descrizione]    VARCHAR (255)   NULL,
    [Icona]          VARBINARY (MAX) NULL,
    [Attivo]         BIT             NULL,
    [Storicizzabile] BIT             NULL,
    [DaModello]      BIT             NULL,
    CONSTRAINT [PK_T_FormatiReport] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

