CREATE TABLE [dbo].[T_CDSSConfigPC] (
    [CodPc]       VARCHAR (50)  NOT NULL,
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Valore]      XML           NULL,
    CONSTRAINT [PK_T_CDSSConfigPC] PRIMARY KEY CLUSTERED ([CodPc] ASC, [Codice] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CodPC_Codice]
    ON [dbo].[T_CDSSConfigPC]([CodPc] ASC, [Codice] ASC);

