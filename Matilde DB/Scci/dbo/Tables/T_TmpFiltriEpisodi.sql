CREATE TABLE [dbo].[T_TmpFiltriEpisodi] (
    [IDSessione]      UNIQUEIDENTIFIER NOT NULL,
    [IDTrasferimento] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_T_TmpFiltriEpisodi] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDTrasferimento] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_IDSessione]
    ON [dbo].[T_TmpFiltriEpisodi]([IDSessione] ASC);

