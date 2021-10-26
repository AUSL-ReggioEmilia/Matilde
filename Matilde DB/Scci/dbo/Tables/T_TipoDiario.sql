CREATE TABLE [dbo].[T_TipoDiario] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoDiario] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

