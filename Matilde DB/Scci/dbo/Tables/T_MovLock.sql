CREATE TABLE [dbo].[T_MovLock] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [IDNum]       INT              IDENTITY (1, 1) NOT NULL,
    [Data]        DATETIME         NULL,
    [DataUTC]     DATETIME         NULL,
    [CodLogin]    VARCHAR (100)    NULL,
    [CodRuolo]    VARCHAR (20)     NULL,
    [NomePC]      VARCHAR (500)    NULL,
    [IndirizzoIP] VARCHAR (50)     NULL,
    [CodAzione]   VARCHAR (20)     NULL,
    [CodEntita]   VARCHAR (20)     NULL,
    [IDEntita]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_T_MovLock] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovLock_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovLock_T_Login] FOREIGN KEY ([CodLogin]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovLock_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Lock]
    ON [dbo].[T_MovLock]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaIDAzione]
    ON [dbo].[T_MovLock]([CodEntita] ASC, [IDEntita] ASC, [CodAzione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaID]
    ON [dbo].[T_MovLock]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataOra]
    ON [dbo].[T_MovLock]([Data] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovLock]([IDNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'da T_Permessi, per ora..', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_MovLock', @level2type = N'COLUMN', @level2name = N'CodAzione';

