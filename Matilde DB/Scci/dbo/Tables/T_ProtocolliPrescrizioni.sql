CREATE TABLE [dbo].[T_ProtocolliPrescrizioni] (
    [Codice]                    VARCHAR (20)    NOT NULL,
    [Descrizione]               VARCHAR (255)   NULL,
    [Colore]                    VARCHAR (50)    NULL,
    [Icona]                     VARBINARY (MAX) NULL,
    [ModelliPrescrizioni]       XML             NULL,
    [DataOraInizioObbligatoria] BIT             NULL,
    [VersioneModello]           INT             NULL,
    CONSTRAINT [PK_T_ProtocolliPrescrizioni] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

