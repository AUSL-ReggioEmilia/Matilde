CREATE TABLE [dbo].[T_TmpFiltriSchede] (
    [IDSessione] UNIQUEIDENTIFIER NOT NULL,
    [IDScheda]   UNIQUEIDENTIFIER NOT NULL,
    [CodEntita]  VARCHAR (20)     NULL,
    CONSTRAINT [PK_T_TmpFiltriSchede] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDScheda] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Entita]
    ON [dbo].[T_TmpFiltriSchede]([IDSessione] ASC, [CodEntita] ASC);

