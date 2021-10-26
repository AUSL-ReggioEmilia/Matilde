CREATE TABLE [dbo].[T_TmpFiltriDiario] (
    [IDSessione]      UNIQUEIDENTIFIER NOT NULL,
    [IDDiarioClinico] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriDiario] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDDiarioClinico] ASC)
);

