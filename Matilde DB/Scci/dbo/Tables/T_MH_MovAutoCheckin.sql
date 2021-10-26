CREATE TABLE [dbo].[T_MH_MovAutoCheckin] (
    [ID]             UNIQUEIDENTIFIER NOT NULL,
    [IDNum]          INT              IDENTITY (1, 1) NOT NULL,
    [CodMHLogin]     VARCHAR (100)    NOT NULL,
    [NumeroTelefono] VARCHAR (100)    NOT NULL,
    [IDCoda]         UNIQUEIDENTIFIER NULL,
    [CodSistema]     VARCHAR (20)     NULL,
    [DataEvento]     DATETIME         NOT NULL,
    [DataEventoUTC]  DATETIME         NOT NULL,
    [Note]           VARCHAR (255)    NULL,
    CONSTRAINT [PK_T_MH_MovAutoCheckin] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MH_MovAutoCheckin_T_MH_Login] FOREIGN KEY ([CodMHLogin]) REFERENCES [dbo].[T_MH_Login] ([Codice]),
    CONSTRAINT [FK_T_MH_MovAutoCheckin_T_MovCode] FOREIGN KEY ([IDCoda]) REFERENCES [dbo].[T_MovCode] ([ID]),
    CONSTRAINT [FK_T_MH_MovAutoCheckin_T_Sistemi] FOREIGN KEY ([CodSistema]) REFERENCES [dbo].[T_Sistemi] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDCoda]
    ON [dbo].[T_MH_MovAutoCheckin]([IDCoda] ASC);

