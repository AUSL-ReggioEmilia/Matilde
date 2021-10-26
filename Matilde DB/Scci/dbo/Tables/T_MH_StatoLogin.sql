CREATE TABLE [dbo].[T_MH_StatoLogin] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Colore]      VARCHAR (50)    NULL,
    [Icona]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_MH_StatoLogin] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

