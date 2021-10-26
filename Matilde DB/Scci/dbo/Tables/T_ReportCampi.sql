CREATE TABLE [dbo].[T_ReportCampi] (
    [CodEntita]   VARCHAR (20)  NOT NULL,
    [CodCampo]    VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_ReportCampi] PRIMARY KEY CLUSTERED ([CodEntita] ASC, [CodCampo] ASC),
    CONSTRAINT [FK_T_ReportCampi_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);

