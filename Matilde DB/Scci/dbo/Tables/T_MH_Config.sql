CREATE TABLE [dbo].[T_MH_Config] (
    [ID]          INT           NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Valore]      VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_MH_Config] PRIMARY KEY CLUSTERED ([ID] ASC)
);

