CREATE TABLE [dbo].[T_PlugIn] (
    [Codice]       VARCHAR (10)  NOT NULL,
    [Categoria]    TINYINT       NULL,
    [Tipo]         TINYINT       NULL,
    [Descrizione]  VARCHAR (150) NULL,
    [Nome]         VARCHAR (50)  NULL,
    [Parametri]    XML           NULL,
    [Disabilitato] BIT           NULL,
    CONSTRAINT [PK_T_PlugIn] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

