CREATE TABLE [dbo].[T_MovAppuntamentiAgende] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                      INT              IDENTITY (1, 1) NOT NULL,
    [IDAppuntamento]             UNIQUEIDENTIFIER NULL,
    [CodAgenda]                  VARCHAR (20)     NULL,
    [CodStatoAppuntamentoAgenda] VARCHAR (20)     NULL,
    [CodUtenteRilevazione]       VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica]    VARCHAR (100)    NULL,
    [DataUltimaModifica]         DATETIME         NULL,
    [DataUltimaModificaUTC]      DATETIME         NULL,
    [CodRaggr1]                  VARCHAR (50)     NULL,
    [DescrRaggr1]                VARCHAR (500)    NULL,
    [CodRaggr2]                  VARCHAR (50)     NULL,
    [DescrRaggr2]                VARCHAR (500)    NULL,
    [CodRaggr3]                  VARCHAR (50)     NULL,
    [DescrRaggr3]                VARCHAR (500)    NULL,
    CONSTRAINT [PK_T_MovAppuntamentiAgende] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovAppuntamentiAgende_T_Agende] FOREIGN KEY ([CodAgenda]) REFERENCES [dbo].[T_Agende] ([Codice]),
    CONSTRAINT [FK_T_MovAppuntamentiAgende_T_Login1] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovAppuntamentiAgende_T_MovAppuntamenti] FOREIGN KEY ([IDAppuntamento]) REFERENCES [dbo].[T_MovAppuntamenti] ([ID]),
    CONSTRAINT [FK_T_MovAppuntamentiAgende_T_StatoAppuntamentoAgende] FOREIGN KEY ([CodStatoAppuntamentoAgenda]) REFERENCES [dbo].[T_StatoAppuntamentoAgende] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodAgendaCodStatoAppuntamento]
    ON [dbo].[T_MovAppuntamentiAgende]([CodAgenda] ASC, [CodStatoAppuntamentoAgenda] ASC)
    INCLUDE([ID], [IDAppuntamento]);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoAppuntamentoAgenda]
    ON [dbo].[T_MovAppuntamentiAgende]([CodStatoAppuntamentoAgenda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Agenda]
    ON [dbo].[T_MovAppuntamentiAgende]([CodAgenda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Appuntamento]
    ON [dbo].[T_MovAppuntamentiAgende]([IDAppuntamento] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovAppuntamentiAgende]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoAppuntamento_Include1]
    ON [dbo].[T_MovAppuntamentiAgende]([CodStatoAppuntamentoAgenda] ASC)
    INCLUDE([ID], [IDAppuntamento], [CodAgenda]);

