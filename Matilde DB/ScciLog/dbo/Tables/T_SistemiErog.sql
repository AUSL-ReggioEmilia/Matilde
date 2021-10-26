CREATE TABLE [dbo].[T_SistemiErog] (
    [Codice]      VARCHAR (50)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_SistemiErog] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

