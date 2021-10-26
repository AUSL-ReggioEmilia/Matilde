CREATE TABLE [dbo].[T_TipoRegistrazione] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoRegistrazione] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

