CREATE TABLE [dbo].[T_TmpFiltriNoteAgende] (
    [IDSessione]   UNIQUEIDENTIFIER NOT NULL,
    [IDNotaAgenda] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriNoteAgende] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDNotaAgenda] ASC)
);

