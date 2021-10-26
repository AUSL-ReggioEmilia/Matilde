CREATE TABLE [dbo].[T_FiltriSpeciali] (
    [Codice]                VARCHAR (20)  NOT NULL,
    [Descrizione]           VARCHAR (255) NULL,
    [SQL]                   VARCHAR (MAX) NULL,
    [CodTipoFiltroSpeciale] VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_FiltriSpeciali] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_FiltriSpeciali_T_TipoFiltroSpeciale] FOREIGN KEY ([CodTipoFiltroSpeciale]) REFERENCES [dbo].[T_TipoFiltroSpeciale] ([Codice])
);

