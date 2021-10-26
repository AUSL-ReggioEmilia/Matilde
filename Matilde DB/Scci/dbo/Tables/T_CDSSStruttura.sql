CREATE TABLE [dbo].[T_CDSSStruttura] (
    [ID]        NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [CodUA]     VARCHAR (20)  NOT NULL,
    [CodAzione] VARCHAR (200) NOT NULL,
    [CodPlugin] VARCHAR (20)  NOT NULL,
    [Parametri] XML           NULL,
    CONSTRAINT [PK_T_CDSSStruttura] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Composto]
    ON [dbo].[T_CDSSStruttura]([CodUA] ASC, [CodAzione] ASC, [CodPlugin] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodAzione]
    ON [dbo].[T_CDSSStruttura]([CodAzione] ASC)
    INCLUDE([CodUA], [CodPlugin], [Parametri]);

