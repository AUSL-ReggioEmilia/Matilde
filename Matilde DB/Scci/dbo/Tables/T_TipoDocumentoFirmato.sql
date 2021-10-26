CREATE TABLE [dbo].[T_TipoDocumentoFirmato] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoDocumentoFirmato] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

