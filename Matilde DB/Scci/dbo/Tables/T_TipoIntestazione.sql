CREATE TABLE [dbo].[T_TipoIntestazione] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_TipoIntestazione] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

