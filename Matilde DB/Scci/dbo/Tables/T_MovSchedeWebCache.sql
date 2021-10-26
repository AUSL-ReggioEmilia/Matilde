CREATE TABLE [dbo].[T_MovSchedeWebCache] (
    [IDNum]                   NUMERIC (18)     IDENTITY (1, 1) NOT NULL,
    [IDMovSchede]             UNIQUEIDENTIFIER NOT NULL,
    [IDSessione]              UNIQUEIDENTIFIER NULL,
    [CodUtenteUltimaModifica] VARCHAR (100)    NOT NULL,
    [DataUltimaModifica]      DATETIME         NOT NULL,
    [Dati]                    NVARCHAR (MAX)   NOT NULL,
    [EavSchema]               NVARCHAR (MAX)   NULL,
    [CodScheda]               VARCHAR (20)     NULL,
    [Versione]                INT              NULL,
    [CodEntita]               VARCHAR (20)     NULL,
    [IDEntita]                UNIQUEIDENTIFIER NULL,
    [IDPaziente]              UNIQUEIDENTIFIER NULL,
    [IDEpisodio]              UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]         UNIQUEIDENTIFIER NULL,
    [IDSchedaPadre]           UNIQUEIDENTIFIER NULL,
    [InEvidenza]              BIT              NULL,
    [DataUltimaLettura]       DATETIME         NULL,
    CONSTRAINT [PK_T_MovSchedeWebCache] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_IDMovSchede_IdNum]
    ON [dbo].[T_MovSchedeWebCache]([IDMovSchede] ASC, [IDNum] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_IDMovSchede]
    ON [dbo].[T_MovSchedeWebCache]([IDMovSchede] ASC);

