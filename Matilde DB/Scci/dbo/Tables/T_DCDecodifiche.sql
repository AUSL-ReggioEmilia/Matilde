CREATE TABLE [dbo].[T_DCDecodifiche] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_DCDecodifiche] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

