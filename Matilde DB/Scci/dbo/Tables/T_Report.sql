CREATE TABLE [dbo].[T_Report] (
    [Codice]                VARCHAR (20)    NOT NULL,
    [Descrizione]           VARCHAR (255)   NULL,
    [Note]                  VARCHAR (2000)  NULL,
    [CodFormatoReport]      VARCHAR (20)    NULL,
    [DaStoricizzare]        BIT             NULL,
    [Path]                  VARCHAR (900)   NULL,
    [Parametri]             XML             NULL,
    [CodReportVista]        VARCHAR (20)    NULL,
    [Variabili]             XML             NULL,
    [Modello]               VARBINARY (MAX) NULL,
    [PercorsoFile]          VARCHAR (MAX)   NULL,
    [ParametriXML]          XML             NULL,
    [NomePlugIn]            VARCHAR (255)   NULL,
    [FlagSistema]           BIT             NULL,
    [ApriBrowser]           BIT             NULL,
    [ApriIE]                BIT             NULL,
    [FlagRichiediStampante] BIT             NULL,
    CONSTRAINT [PK_T_TipoReport] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Report_T_FormatoReport] FOREIGN KEY ([CodFormatoReport]) REFERENCES [dbo].[T_FormatoReport] ([Codice]),
    CONSTRAINT [FK_T_Report_T_ReportViste] FOREIGN KEY ([CodReportVista]) REFERENCES [dbo].[T_ReportViste] ([Codice])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ha senso solo per report <> REM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_Report', @level2type = N'COLUMN', @level2name = N'DaStoricizzare';

