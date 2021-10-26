CREATE TABLE [dbo].[T_ConfigUtente] (
    [Codice]                 VARCHAR (100) NOT NULL,
    [Valore]                 XML           NULL,
    [WebCodRuoloSelezionato] VARCHAR (20)  NULL,
    [WebJsonData]            VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_ConfigUtente] PRIMARY KEY CLUSTERED ([Codice] ASC)
);



