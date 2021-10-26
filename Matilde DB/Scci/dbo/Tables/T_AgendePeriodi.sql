CREATE TABLE [dbo].[T_AgendePeriodi] (
    [CodAgenda]   VARCHAR (20) NOT NULL,
    [DataInizio]  DATETIME     NOT NULL,
    [DataFine]    DATETIME     NOT NULL,
    [OrariLavoro] XML          NULL,
    CONSTRAINT [PK_T_AgendePeriodi] PRIMARY KEY CLUSTERED ([CodAgenda] ASC, [DataInizio] ASC, [DataFine] ASC),
    CONSTRAINT [FK_T_AgendePeriodi_T_Agende] FOREIGN KEY ([CodAgenda]) REFERENCES [dbo].[T_Agende] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_DataFine]
    ON [dbo].[T_AgendePeriodi]([DataFine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataInizio]
    ON [dbo].[T_AgendePeriodi]([DataInizio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodAgendaDataInizio]
    ON [dbo].[T_AgendePeriodi]([CodAgenda] ASC, [DataInizio] ASC);

