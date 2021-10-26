CREATE TABLE [dbo].[T_Integra_Campi] (
    [Codice]        VARCHAR (20)  NOT NULL,
    [CodEntita]     VARCHAR (20)  NOT NULL,
    [CodTipoEntita] VARCHAR (20)  NOT NULL,
    [Campo]         VARCHAR (50)  NOT NULL,
    [Note]          VARCHAR (500) NULL,
    CONSTRAINT [PK_T_Integra_Campi] PRIMARY KEY NONCLUSTERED ([Codice] ASC)
);

