CREATE TABLE [dbo].[T_MovConsegne] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [CodUA]                   VARCHAR (20)     NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [CodTipoConsegna]         VARCHAR (20)     NULL,
    [CodStatoConsegna]        VARCHAR (20)     NULL,
    [CodUtenteRilevazione]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [CodUtenteAnnullamento]   VARCHAR (100)    NULL,
    [DataEvento]              DATETIME         NULL,
    [DataEventoUTC]           DATETIME         NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [DataAnnullamento]        DATETIME         NULL,
    [DataAnnullamentoUTC]     DATETIME         NULL,
    CONSTRAINT [PK_T_MovConsegne] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovConsegne_T_Login] FOREIGN KEY ([CodUtenteRilevazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegne_T_Login_2] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegne_T_Login_3] FOREIGN KEY ([CodUtenteAnnullamento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovConsegne_T_StatoDiario] FOREIGN KEY ([CodStatoConsegna]) REFERENCES [dbo].[T_StatoDiario] ([Codice]),
    CONSTRAINT [FK_T_MovConsegne_T_TipoConsegna] FOREIGN KEY ([CodTipoConsegna]) REFERENCES [dbo].[T_TipoConsegna] ([Codice]),
    CONSTRAINT [FK_T_MovConsegne_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodTipoConsegna]
    ON [dbo].[T_MovConsegne]([CodUA] ASC, [CodTipoConsegna] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUACodStatoConsegna]
    ON [dbo].[T_MovConsegne]([CodUA] ASC, [CodStatoConsegna] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoConsegna]
    ON [dbo].[T_MovConsegne]([CodStatoConsegna] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_MovConsegne]([CodUA] ASC);

