CREATE TABLE [dbo].[T_ConfigPc] (
    [CodPc]       VARCHAR (50)  NOT NULL,
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Valore]      XML           NULL,
    CONSTRAINT [PK_T_ConfigPc] PRIMARY KEY CLUSTERED ([CodPc] ASC, [Codice] ASC)
);

