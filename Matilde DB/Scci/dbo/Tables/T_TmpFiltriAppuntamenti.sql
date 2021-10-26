CREATE TABLE [dbo].[T_TmpFiltriAppuntamenti] (
    [IDSessione]     UNIQUEIDENTIFIER NOT NULL,
    [IDAppuntamento] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriAppuntamenti] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDAppuntamento] ASC)
);

