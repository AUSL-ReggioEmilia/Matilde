CREATE TABLE [dbo].[T_TmpFiltriAppuntamentiAgende] (
    [IDSessione]           UNIQUEIDENTIFIER NOT NULL,
    [IDAppuntamentoAgenda] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriAppuntamentiAgende] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDAppuntamentoAgenda] ASC)
);

