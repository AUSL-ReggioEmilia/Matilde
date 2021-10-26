CREATE TABLE [dbo].[T_CampiAgende] (
    [Codice]      VARCHAR (50)  NOT NULL,
    [CodEntita]   VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_CampiAgende] PRIMARY KEY CLUSTERED ([Codice] ASC, [CodEntita] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_T_CampiAgende_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);

