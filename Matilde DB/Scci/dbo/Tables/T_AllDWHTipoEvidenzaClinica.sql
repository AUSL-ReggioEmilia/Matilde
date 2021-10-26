CREATE TABLE [dbo].[T_AllDWHTipoEvidenzaClinica] (
    [CodTipoEvidenzaClinica] VARCHAR (20) NOT NULL,
    [CodDWH]                 VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AllDWHTipoEvidenzaClinica] PRIMARY KEY CLUSTERED ([CodTipoEvidenzaClinica] ASC, [CodDWH] ASC)
);

