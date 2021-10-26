CREATE TABLE [dbo].[T_ProtocolliAttivitaTempiTipoTask] (
    [CodProtocolloAttivitaTempi] VARCHAR (20) NOT NULL,
    [CodTipoTaskInfermieristico] VARCHAR (20) NOT NULL,
    [Ordine]                     INT          NULL,
    CONSTRAINT [PK_T_ProtocolliAttivitaTempiTipoTask] PRIMARY KEY CLUSTERED ([CodProtocolloAttivitaTempi] ASC, [CodTipoTaskInfermieristico] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CodTask]
    ON [dbo].[T_ProtocolliAttivitaTempiTipoTask]([CodTipoTaskInfermieristico] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodProtocolloAttivitaTempi]
    ON [dbo].[T_ProtocolliAttivitaTempiTipoTask]([CodProtocolloAttivitaTempi] ASC);

