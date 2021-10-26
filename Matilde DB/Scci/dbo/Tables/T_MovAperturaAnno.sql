CREATE TABLE [dbo].[T_MovAperturaAnno] (
    [Codice]           VARCHAR (200) NOT NULL,
    [Descrizione]      VARCHAR (255) NULL,
    [DataElaborazione] DATETIME      NULL,
    [Note]             VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_MovAperturaAnno] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

