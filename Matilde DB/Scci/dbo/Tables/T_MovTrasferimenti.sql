CREATE TABLE [dbo].[T_MovTrasferimenti] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                 INT              IDENTITY (1, 1) NOT NULL,
    [IDEpisodio]            UNIQUEIDENTIFIER NULL,
    [Sequenza]              NUMERIC (18)     NULL,
    [CodUA]                 VARCHAR (20)     NULL,
    [CodStatoTrasferimento] VARCHAR (20)     NULL,
    [DataIngresso]          DATETIME         NULL,
    [DataIngressoUTC]       DATETIME         NULL,
    [DataUscita]            DATETIME         NULL,
    [DataUscitaUTC]         DATETIME         NULL,
    [CodUO]                 VARCHAR (20)     NULL,
    [DescrUO]               VARCHAR (255)    NULL,
    [CodSettore]            VARCHAR (20)     NULL,
    [DescrSettore]          VARCHAR (255)    NULL,
    [CodStanza]             VARCHAR (20)     NULL,
    [DescrStanza]           VARCHAR (255)    NULL,
    [CodLetto]              VARCHAR (20)     NULL,
    [DescrLetto]            VARCHAR (255)    NULL,
    [IDCartella]            UNIQUEIDENTIFIER NULL,
    [CodAziTrasferimento]   VARCHAR (20)     NULL,
    CONSTRAINT [PK_T_MovTrasferimenti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovTrasferimenti_T_Aziende] FOREIGN KEY ([CodAziTrasferimento]) REFERENCES [dbo].[T_Aziende] ([Codice]),
    CONSTRAINT [FK_T_MovTrasferimenti_T_MovCartelle] FOREIGN KEY ([IDCartella]) REFERENCES [dbo].[T_MovCartelle] ([ID]),
    CONSTRAINT [FK_T_MovTrasferimenti_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovTrasferimenti_T_StatoTrasferimento] FOREIGN KEY ([CodStatoTrasferimento]) REFERENCES [dbo].[T_StatoTrasferimento] ([Codice]),
    CONSTRAINT [FK_T_MovTrasferimenti_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodioIDCartellaIDNum]
    ON [dbo].[T_MovTrasferimenti]([IDNum] ASC, [IDEpisodio] ASC, [IDCartella] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataIngresso]
    ON [dbo].[T_MovTrasferimenti]([DataIngresso] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodStatoTrasferimento]
    ON [dbo].[T_MovTrasferimenti]([CodStatoTrasferimento] ASC)
    INCLUDE([ID], [IDEpisodio], [CodUA], [IDCartella]);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_MovTrasferimenti]([CodUA] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDCartella]
    ON [dbo].[T_MovTrasferimenti]([IDCartella] ASC)
    INCLUDE([IDEpisodio]);


GO
CREATE NONCLUSTERED INDEX [IXEpisodioSeq]
    ON [dbo].[T_MovTrasferimenti]([IDEpisodio] ASC, [Sequenza] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Letto]
    ON [dbo].[T_MovTrasferimenti]([DescrLetto] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Stanza]
    ON [dbo].[T_MovTrasferimenti]([DescrStanza] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Settore]
    ON [dbo].[T_MovTrasferimenti]([DescrSettore] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UO]
    ON [dbo].[T_MovTrasferimenti]([DescrUO] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodioIDCartella]
    ON [dbo].[T_MovTrasferimenti]([IDEpisodio] ASC, [IDCartella] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovTrasferimenti]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovTrasferimenti]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA2]
    ON [dbo].[T_MovTrasferimenti]([CodUA] ASC)
    INCLUDE([ID], [IDEpisodio]);


GO
CREATE NONCLUSTERED INDEX [IX_CodAziTrasferimento]
    ON [dbo].[T_MovTrasferimenti]([CodAziTrasferimento] ASC);

