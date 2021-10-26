CREATE TABLE [dbo].[T_TmpFiltriAllegati] (
    [IDSessione] UNIQUEIDENTIFIER NOT NULL,
    [IDAllegato] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriAllegati] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDAllegato] ASC)
);

