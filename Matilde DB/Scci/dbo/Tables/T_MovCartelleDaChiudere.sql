CREATE TABLE [dbo].[T_MovCartelleDaChiudere] (
    [IDCartella]         UNIQUEIDENTIFIER NOT NULL,
    [IDNum]              INT              IDENTITY (1, 1) NOT NULL,
    [CodUtente]          VARCHAR (100)    NULL,
    [CodRuolo]           VARCHAR (20)     NULL,
    [DataInserimento]    DATETIME         NULL,
    [DataInserimentoUTC] DATETIME         NULL,
    CONSTRAINT [PK_T_MovCartelleDaChiudere] PRIMARY KEY NONCLUSTERED ([IDCartella] ASC),
    CONSTRAINT [FK_T_MovCartelleDaChiudere_T_LoginIns] FOREIGN KEY ([CodUtente]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleDaChiudere_T_MovCartelle] FOREIGN KEY ([IDCartella]) REFERENCES [dbo].[T_MovCartelle] ([ID]),
    CONSTRAINT [FK_T_MovCartelleDaChiudere_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);

