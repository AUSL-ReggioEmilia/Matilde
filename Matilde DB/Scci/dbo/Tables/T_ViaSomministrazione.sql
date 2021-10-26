CREATE TABLE [dbo].[T_ViaSomministrazione] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_TipoViaSomministrazione] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

