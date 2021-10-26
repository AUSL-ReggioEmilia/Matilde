CREATE TABLE [dbo].[T_ProtocolliTempi] (
    [Codice]        VARCHAR (30)  NOT NULL,
    [CodProtocollo] VARCHAR (20)  NULL,
    [Descrizione]   VARCHAR (255) NULL,
    [Delta]         INT           NULL,
    [Ora]           DATETIME      NULL,
    CONSTRAINT [PK_T_ProtocolliTempi] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_ProtocolliTempi_T_Protocolli] FOREIGN KEY ([CodProtocollo]) REFERENCES [dbo].[T_Protocolli] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDProtocollo]
    ON [dbo].[T_ProtocolliTempi]([CodProtocollo] ASC);

