CREATE TABLE [dbo].[T_AzioniEntita] (
    [CodEntita]                VARCHAR (20) NOT NULL,
    [CodAzione]                VARCHAR (20) NOT NULL,
    [AbilitaPermessiDettaglio] BIT          NULL,
    [RegistraTimeStamp]        BIT          NULL,
    CONSTRAINT [PK_T_AzioniEntita] PRIMARY KEY CLUSTERED ([CodEntita] ASC, [CodAzione] ASC),
    CONSTRAINT [FK_T_AzioniEntita_T_Azioni] FOREIGN KEY ([CodAzione]) REFERENCES [dbo].[T_Azioni] ([Codice]),
    CONSTRAINT [FK_T_AzioniEntita_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);

