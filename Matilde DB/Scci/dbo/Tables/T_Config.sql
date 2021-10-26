CREATE TABLE [dbo].[T_Config] (
    [ID]          INT             NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Valore]      VARCHAR (MAX)   NULL,
    [Immagine]    VARBINARY (MAX) NULL,
    CONSTRAINT [PK_T_Config] PRIMARY KEY CLUSTERED ([ID] ASC)
);

