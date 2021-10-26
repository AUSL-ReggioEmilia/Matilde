CREATE TABLE [dbo].[T_Icone] (
    [IDNum]       NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [CodEntita]   VARCHAR (20)    NOT NULL,
    [CodTipo]     VARCHAR (20)    NOT NULL,
    [CodStato]    VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (900)   NULL,
    [Icona16]     VARBINARY (MAX) NULL,
    [Icona32]     VARBINARY (MAX) NULL,
    [Icona48]     VARBINARY (MAX) NULL,
    [Icona256]    VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_Icone] PRIMARY KEY CLUSTERED ([CodEntita] ASC, [CodTipo] ASC, [CodStato] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_Icone]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntita]
    ON [dbo].[T_Icone]([CodEntita] ASC) WITH (FILLFACTOR = 95);

