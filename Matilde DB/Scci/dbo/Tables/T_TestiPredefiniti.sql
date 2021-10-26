CREATE TABLE [dbo].[T_TestiPredefiniti] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [TestoRTF]    VARCHAR (MAX) NULL,
    [Path]        VARCHAR (900) NULL,
    [CodEntita]   VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_TestiPredefiniti] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TestiPredefiniti_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Da T_Entita con Filtro su ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_TestiPredefiniti', @level2type = N'COLUMN', @level2name = N'CodEntita';

