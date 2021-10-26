CREATE TABLE [dbo].[T_CDSSPlugins] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [NomePlugin]  VARCHAR (50)    NULL,
    [Comando]     VARCHAR (255)   NULL,
    [Modalita]    VARCHAR (5)     NULL,
    [Ordine]      INT             NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [CodTipoCDSS] VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_CDSSEventi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CodTipoCDSS]
    ON [dbo].[T_CDSSPlugins]([CodTipoCDSS] ASC);

