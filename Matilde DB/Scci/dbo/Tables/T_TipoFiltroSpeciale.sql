CREATE TABLE [dbo].[T_TipoFiltroSpeciale] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_TipoFiltroSpeciale] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

