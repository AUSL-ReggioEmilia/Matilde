CREATE TABLE [dbo].[T_MovLogImportSchede] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                 INT              IDENTITY (1, 1) NOT NULL,
    [CodSistema]            VARCHAR (20)     NULL,
    [IDSistema]             VARCHAR (50)     NULL,
    [IDScheda]              UNIQUEIDENTIFIER NULL,
    [DataOraImportazione]   DATETIME         NULL,
    [CodUtenteImportazione] NCHAR (10)       NULL,
    CONSTRAINT [PK_T_MovLogImportSchede] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

