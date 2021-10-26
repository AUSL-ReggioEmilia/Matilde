CREATE TABLE [dbo].[T_TipoAlertAllergiaAnamnesi] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [CodScheda]   VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_TipoAlertAllergieAnamnesi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

