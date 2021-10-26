CREATE TABLE [dbo].[T_MovTimeStamp] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [IDNum]           INT              IDENTITY (1, 1) NOT NULL,
    [DataOra]         DATETIME         NULL,
    [DataOraUTC]      DATETIME         NULL,
    [CodLogin]        VARCHAR (100)    NULL,
    [CodRuolo]        VARCHAR (20)     NULL,
    [NomePC]          VARCHAR (50)     NULL,
    [IndirizzoIP]     VARCHAR (50)     NULL,
    [CodEntita]       VARCHAR (20)     NULL,
    [IDEntita]        UNIQUEIDENTIFIER NULL,
    [CodAzione]       VARCHAR (20)     NULL,
    [IDPaziente]      UNIQUEIDENTIFIER NULL,
    [IDEpisodio]      UNIQUEIDENTIFIER NULL,
    [IDTrasferimento] UNIQUEIDENTIFIER NULL,
    [Note]            VARCHAR (255)    NULL,
    [Info]            VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_T_MovTimeStamp] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovTimeStamp_T_Azioni] FOREIGN KEY ([CodAzione]) REFERENCES [dbo].[T_Azioni] ([Codice]),
    CONSTRAINT [FK_T_MovTimeStamp_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovTimeStamp_T_Ruoli] FOREIGN KEY ([CodRuolo]) REFERENCES [dbo].[T_Ruoli] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_DataOra]
    ON [dbo].[T_MovTimeStamp]([DataOra] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaIDAzione]
    ON [dbo].[T_MovTimeStamp]([CodEntita] ASC, [IDEntita] ASC, [CodAzione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaID]
    ON [dbo].[T_MovTimeStamp]([CodEntita] ASC, [IDEntita] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovTimeStamp]([IDNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'da T_Permessi, per ora..', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_MovTimeStamp', @level2type = N'COLUMN', @level2name = N'CodAzione';


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaDataOraCodAzione]
    ON [dbo].[T_MovTimeStamp]([CodEntita] ASC, [DataOra] ASC, [CodAzione] ASC)
    INCLUDE([CodLogin], [IDEntita]);

