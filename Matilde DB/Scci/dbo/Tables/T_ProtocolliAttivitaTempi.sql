CREATE TABLE [dbo].[T_ProtocolliAttivitaTempi] (
    [Codice]                VARCHAR (20)  NOT NULL,
    [CodProtocolloAttivita] VARCHAR (20)  NULL,
    [Descrizione]           VARCHAR (255) NULL,
    [DeltaGiorni]           INT           NULL,
    [DeltaOre]              INT           NULL,
    [DeltaMinuti]           INT           NULL,
    [DeltaAlle00]           BIT           NULL,
    CONSTRAINT [PK_T_ProtocolliAttivitaTempi] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_ProtocolliAttivitaTempi_T_ProtocolliAttivita] FOREIGN KEY ([CodProtocolloAttivita]) REFERENCES [dbo].[T_ProtocolliAttivita] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_Protocollo]
    ON [dbo].[T_ProtocolliAttivitaTempi]([CodProtocolloAttivita] ASC);

