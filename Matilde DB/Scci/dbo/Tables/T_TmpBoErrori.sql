CREATE TABLE [dbo].[T_TmpBoErrori] (
    [IDSessione] UNIQUEIDENTIFIER NOT NULL,
    [Errore]     VARCHAR (500)    NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_IDSessione]
    ON [dbo].[T_TmpBoErrori]([IDSessione] ASC);

