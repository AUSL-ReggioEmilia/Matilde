CREATE TABLE [dbo].[T_Integra_Destinatari] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Indirizzo]   VARCHAR (500) NULL,
    [Dominio]     VARCHAR (255) NULL,
    [Utente]      VARCHAR (255) NULL,
    [Password]    VARCHAR (255) NULL,
    [Note]        VARCHAR (500) NULL,
    [Https]       BIT           NULL,
    CONSTRAINT [PK_T_Integra_Destinatari] PRIMARY KEY NONCLUSTERED ([Codice] ASC)
);

