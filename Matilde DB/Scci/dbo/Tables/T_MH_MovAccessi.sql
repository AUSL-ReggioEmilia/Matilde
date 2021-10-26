CREATE TABLE [dbo].[T_MH_MovAccessi] (
    [ID]                   UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                INT              IDENTITY (1, 1) NOT NULL,
    [CodMHLogin]           VARCHAR (100)    NOT NULL,
    [NumeroTelefono]       VARCHAR (100)    NOT NULL,
    [CodUtenteRilevazione] VARCHAR (100)    NULL,
    [CodSistema]           VARCHAR (20)     NULL,
    [DataEvento]           DATETIME         NOT NULL,
    [DataEventoUTC]        DATETIME         NOT NULL,
    [NomePC]               VARCHAR (50)     NULL,
    [IndirizzoIP]          VARCHAR (50)     NULL,
    [Note]                 VARCHAR (255)    NULL,
    CONSTRAINT [PK_T_MH_MovAccessi] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MH_MovAccessi_T_Login] FOREIGN KEY ([CodUtenteRilevazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_MovAccessi_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodMHLogin]
    ON [dbo].[T_MH_MovAccessi]([CodMHLogin] ASC);

