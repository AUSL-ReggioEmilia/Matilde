CREATE TABLE [dbo].[T_TipoConsegna] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [CodScheda]   VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_TipoConsegna] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoConsegna_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);

