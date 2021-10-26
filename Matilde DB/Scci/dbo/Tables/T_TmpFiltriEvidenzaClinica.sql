CREATE TABLE [dbo].[T_TmpFiltriEvidenzaClinica] (
    [IDSessione]        UNIQUEIDENTIFIER NOT NULL,
    [IDEvidenzaClinica] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriEvidenzaClinica] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDEvidenzaClinica] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_IDSessione]
    ON [dbo].[T_TmpFiltriEvidenzaClinica]([IDSessione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEvidenzaClinica]
    ON [dbo].[T_TmpFiltriEvidenzaClinica]([IDEvidenzaClinica] ASC);

