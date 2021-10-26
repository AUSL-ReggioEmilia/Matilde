CREATE TABLE [dbo].[T_EBM] (
    [Codice]      VARCHAR (20)   NOT NULL,
    [Descrizione] VARCHAR (255)  NULL,
    [Note]        VARCHAR (2000) NULL,
    [Url]         VARCHAR (MAX)  NULL,
    [Ordine]      INT            NULL,
    CONSTRAINT [PK_T_EBM] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

