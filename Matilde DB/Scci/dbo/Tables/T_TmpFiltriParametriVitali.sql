CREATE TABLE [dbo].[T_TmpFiltriParametriVitali] (
    [IDSessione]        UNIQUEIDENTIFIER NOT NULL,
    [IDParametroVitale] UNIQUEIDENTIFIER NOT NULL,
    [CodEntita]         VARCHAR (20)     NULL,
    CONSTRAINT [PK_TmpFiltriParametriVitali] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDParametroVitale] ASC)
);

