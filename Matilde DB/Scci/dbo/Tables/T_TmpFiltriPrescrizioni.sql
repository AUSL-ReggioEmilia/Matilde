CREATE TABLE [dbo].[T_TmpFiltriPrescrizioni] (
    [IDSessione]     UNIQUEIDENTIFIER NOT NULL,
    [IDPrescrizione] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriPrescrizioni] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDPrescrizione] ASC)
);

