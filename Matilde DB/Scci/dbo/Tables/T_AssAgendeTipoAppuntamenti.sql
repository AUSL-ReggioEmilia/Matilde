CREATE TABLE [dbo].[T_AssAgendeTipoAppuntamenti] (
    [CodAgenda]              VARCHAR (20) NOT NULL,
    [CodTipoApp]             VARCHAR (20) NOT NULL,
    [EscludiSovrapposizioni] BIT          NULL,
    CONSTRAINT [PK_T_AssAgendeTipoAppuntamenti] PRIMARY KEY CLUSTERED ([CodAgenda] ASC, [CodTipoApp] ASC),
    CONSTRAINT [FK_T_AssAgendeTipoAppuntamenti_T_Agende] FOREIGN KEY ([CodAgenda]) REFERENCES [dbo].[T_Agende] ([Codice]),
    CONSTRAINT [FK_T_AssAgendeTipoAppuntamenti_T_TipoAppuntamento] FOREIGN KEY ([CodTipoApp]) REFERENCES [dbo].[T_TipoAppuntamento] ([Codice])
);



