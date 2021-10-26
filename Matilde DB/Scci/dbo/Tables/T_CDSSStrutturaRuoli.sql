CREATE TABLE [dbo].[T_CDSSStrutturaRuoli] (
    [ID]        NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [CodRuolo]  VARCHAR (20)  NOT NULL,
    [CodAzione] VARCHAR (200) NOT NULL,
    [CodPlugin] VARCHAR (20)  NOT NULL,
    [Parametri] XML           NULL,
    CONSTRAINT [PK_T_CDSSStrutturaRuoli] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Composto]
    ON [dbo].[T_CDSSStrutturaRuoli]([CodRuolo] ASC, [CodAzione] ASC, [CodPlugin] ASC);

