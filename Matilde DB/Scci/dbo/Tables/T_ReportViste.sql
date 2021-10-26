CREATE TABLE [dbo].[T_ReportViste] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NOT NULL,
    [OggettoSQL]  VARCHAR (255) NULL,
    CONSTRAINT [PK_T_ReportViste] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

