CREATE TABLE [dbo].[T_MovCode] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                   INT              IDENTITY (1, 1) NOT NULL,
    [NumeroCoda]              INT              NULL,
    [CodContatore]            VARCHAR (20)     NULL,
    [CodStatoCoda]            VARCHAR (20)     NULL,
    [CodUtenteAssegnazione]   VARCHAR (100)    NULL,
    [CodUtenteInserimento]    VARCHAR (100)    NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NULL,
    [DataAssegnazione]        DATETIME         NULL,
    [DataAssegnazioneUTC]     DATETIME         NULL,
    [DataInserimento]         DATETIME         NULL,
    [DataInserimentoUTC]      DATETIME         NULL,
    [DataUltimaModifica]      DATETIME         NULL,
    [DataUltimaModificaUTC]   DATETIME         NULL,
    [Priorita]                INT              NULL,
    CONSTRAINT [PK_T_MovCode] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovCode_T_Contatori] FOREIGN KEY ([CodContatore]) REFERENCES [dbo].[T_Contatori] ([Codice]),
    CONSTRAINT [FK_T_MovCode_T_Login1] FOREIGN KEY ([CodUtenteInserimento]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCode_T_Login2] FOREIGN KEY ([CodUtenteUltimaModifica]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCode_T_Login3] FOREIGN KEY ([CodUtenteAssegnazione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCode_T_StatoCoda] FOREIGN KEY ([CodStatoCoda]) REFERENCES [dbo].[T_StatoCoda] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodStatoID]
    ON [dbo].[T_MovCode]([CodStatoCoda] ASC, [ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoCoda]
    ON [dbo].[T_MovCode]([CodStatoCoda] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovCode]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NumeroCodaCodContatoreCodStatoCoda_DataAssegnazione]
    ON [dbo].[T_MovCode]([NumeroCoda] ASC, [CodContatore] ASC, [CodStatoCoda] ASC, [DataAssegnazione] ASC);

