CREATE TABLE [dbo].[T_Protocolli] (
    [Codice]            VARCHAR (20)  NOT NULL,
    [Descrizione]       VARCHAR (255) NULL,
    [Continuita]        BIT           NULL,
    [Durata]            INT           NULL,
    [CodTipoProtocollo] VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_Protocolli] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

