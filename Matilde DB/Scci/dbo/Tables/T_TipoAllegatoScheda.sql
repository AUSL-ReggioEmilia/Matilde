CREATE TABLE [dbo].[T_TipoAllegatoScheda] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoAllegatoScheda] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

