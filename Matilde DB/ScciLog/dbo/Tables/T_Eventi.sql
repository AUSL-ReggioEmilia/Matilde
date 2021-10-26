CREATE TABLE [dbo].[T_Eventi] (
    [Codice]         VARCHAR (50)  NOT NULL,
    [Descrizione]    VARCHAR (255) NULL,
    [Attivo]         BIT           NULL,
    [CodSistemaErog] VARCHAR (50)  NULL,
    CONSTRAINT [PK_T_DataLogEventi] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Eventi_T_SistemiErog] FOREIGN KEY ([CodSistemaErog]) REFERENCES [dbo].[T_SistemiErog] ([Codice])
);

