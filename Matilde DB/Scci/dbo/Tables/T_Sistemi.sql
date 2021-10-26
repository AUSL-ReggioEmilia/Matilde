CREATE TABLE [dbo].[T_Sistemi] (
    [Codice]               VARCHAR (20)    NOT NULL,
    [Descrizione]          VARCHAR (255)   NULL,
    [Colore]               VARCHAR (50)    NULL,
    [Icona]                VARBINARY (MAX) NULL,
    [FlagSistemaRiservato] BIT             NULL,
    [CodDestinatario]      VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_Sistemi] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Sistemi_T_Integra_Destinatari] FOREIGN KEY ([CodDestinatario]) REFERENCES [dbo].[T_Integra_Destinatari] ([Codice])
);

