CREATE TABLE [dbo].[T_Letti] (
    [ID]          NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [CodAzi]      VARCHAR (20)  NOT NULL,
    [CodLetto]    VARCHAR (20)  NOT NULL,
    [CodSettore]  VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [CodStanza]   VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_Letti] PRIMARY KEY CLUSTERED ([CodAzi] ASC, [CodLetto] ASC, [CodSettore] ASC),
    CONSTRAINT [FK_T_Letti_T_Aziende] FOREIGN KEY ([CodAzi]) REFERENCES [dbo].[T_Aziende] ([Codice])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AziLettSet]
    ON [dbo].[T_Letti]([CodAzi] ASC, [CodLetto] ASC, [CodSettore] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Facoltativa', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_Letti', @level2type = N'COLUMN', @level2name = N'CodStanza';

