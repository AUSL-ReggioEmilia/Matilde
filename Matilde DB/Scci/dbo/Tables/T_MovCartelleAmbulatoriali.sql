CREATE TABLE [dbo].[T_MovCartelleAmbulatoriali] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [IDNum]             INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [NumeroCartella]    VARCHAR (50)     NULL,
    [CodStatoCartella]  VARCHAR (20)     NULL,
    [CodUtenteApertura] VARCHAR (100)    NULL,
    [CodUtenteChiusura] VARCHAR (100)    NULL,
    [DataApertura]      DATETIME         NULL,
    [DataAperturaUTC]   DATETIME         NULL,
    [DataChiusura]      DATETIME         NULL,
    [DataChiusuraUTC]   DATETIME         NULL,
    [PDFCartella]       VARBINARY (MAX)  NULL,
    CONSTRAINT [PK_T_MovCartelleAmbulatoriali] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovCartelleAmbulatoriali_T_Login] FOREIGN KEY ([CodUtenteApertura]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleAmbulatoriali_T_Login2] FOREIGN KEY ([CodUtenteChiusura]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovCartelleAmbulatoriali_T_StatoCartella] FOREIGN KEY ([CodStatoCartella]) REFERENCES [dbo].[T_StatoCartella] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_NumeroCartella]
    ON [dbo].[T_MovCartelleAmbulatoriali]([NumeroCartella] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ID_Include1]
    ON [dbo].[T_MovCartelleAmbulatoriali]([ID] ASC)
    INCLUDE([NumeroCartella]);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoCartella_Include1]
    ON [dbo].[T_MovCartelleAmbulatoriali]([CodStatoCartella] ASC)
    INCLUDE([ID], [NumeroCartella]);

