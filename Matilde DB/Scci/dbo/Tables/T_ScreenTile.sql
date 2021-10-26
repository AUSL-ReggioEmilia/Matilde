CREATE TABLE [dbo].[T_ScreenTile] (
    [ID]              NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [CodScreen]       VARCHAR (20)  NOT NULL,
    [Riga]            SMALLINT      NOT NULL,
    [Colonna]         SMALLINT      NOT NULL,
    [Altezza]         SMALLINT      NOT NULL,
    [Larghezza]       SMALLINT      NOT NULL,
    [InEvidenza]      BIT           NULL,
    [CodPlugin]       VARCHAR (20)  NULL,
    [Attributi]       XML           NULL,
    [NomeTile]        VARCHAR (255) NULL,
    [Fissa]           BIT           NULL,
    [NonCollassabile] BIT           NULL,
    [Collassata]      BIT           NOT NULL,
    CONSTRAINT [PK_T_ScreenTile] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_ScreenTile_T_Screen] FOREIGN KEY ([CodScreen]) REFERENCES [dbo].[T_Screen] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Composto]
    ON [dbo].[T_ScreenTile]([CodScreen] ASC, [Riga] ASC, [Colonna] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodScreen]
    ON [dbo].[T_ScreenTile]([CodScreen] ASC);

