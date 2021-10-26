CREATE TABLE [dbo].[T_Schedulazioni] (
    [Codice]                 VARCHAR (20)  NOT NULL,
    [Descrizione]            VARCHAR (200) NULL,
    [Comando]                VARCHAR (MAX) NULL,
    [DataUltimoPeriodo]      DATETIME      NULL,
    [DataUltimaElaborazione] DATETIME      NULL,
    [Step]                   INT           NULL,
    CONSTRAINT [PK_T_Schedulazioni] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

