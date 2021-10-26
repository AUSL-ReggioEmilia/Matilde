CREATE TABLE [dbo].[T_TipoScheda] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoScheda] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

