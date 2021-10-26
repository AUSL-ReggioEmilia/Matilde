CREATE TABLE [dbo].[T_AssRuoliAzioni] (
    [CodRuolo]  VARCHAR (20)  NOT NULL,
    [CodEntita] VARCHAR (20)  NOT NULL,
    [CodVoce]   VARCHAR (20)  NOT NULL,
    [CodAzione] VARCHAR (20)  NOT NULL,
    [Parametri] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_AssRuoliAzioni] PRIMARY KEY CLUSTERED ([CodRuolo] ASC, [CodEntita] ASC, [CodVoce] ASC, [CodAzione] ASC),
    CONSTRAINT [FK_T_AssRuoliAzioni_T_Azioni] FOREIGN KEY ([CodAzione]) REFERENCES [dbo].[T_Azioni] ([Codice]),
    CONSTRAINT [FK_T_AssRuoliAzioni_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_AssRuoliAzioni_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaCodVoceCodAzione]
    ON [dbo].[T_AssRuoliAzioni]([CodEntita] ASC, [CodVoce] ASC, [CodAzione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaCodAzione]
    ON [dbo].[T_AssRuoliAzioni]([CodEntita] ASC, [CodAzione] ASC);

