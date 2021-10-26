CREATE TABLE [dbo].[T_TipoSelezione] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoSelezione] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

