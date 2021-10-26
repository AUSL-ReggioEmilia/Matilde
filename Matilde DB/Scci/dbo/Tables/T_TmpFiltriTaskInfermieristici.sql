CREATE TABLE [dbo].[T_TmpFiltriTaskInfermieristici] (
    [IDSessione]            UNIQUEIDENTIFIER NOT NULL,
    [IDTaskInfermieristico] UNIQUEIDENTIFIER NOT NULL,
    [CodEntita]             VARCHAR (20)     NULL,
    CONSTRAINT [PK_T_TmpFiltriTaskInfermieristici] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDTaskInfermieristico] ASC)
);

