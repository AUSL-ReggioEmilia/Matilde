CREATE TABLE [dbo].[T_ConfigCE] (
    [ID]          INT             NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Valore]      VARCHAR (MAX)   NULL,
    [Immagine]    VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_ConfigCE] PRIMARY KEY CLUSTERED ([ID] ASC)
);

