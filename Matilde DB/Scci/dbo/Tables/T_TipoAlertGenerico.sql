CREATE TABLE [dbo].[T_TipoAlertGenerico] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [CodScheda]   VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_TipoAlertGenerici] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

