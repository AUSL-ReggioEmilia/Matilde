CREATE TABLE [dbo].[T_TipoTaskInfermieristicoTempi] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Ordine]      INT           NULL,
    CONSTRAINT [PK_T_TipoTaskInfermieristicoTempi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

