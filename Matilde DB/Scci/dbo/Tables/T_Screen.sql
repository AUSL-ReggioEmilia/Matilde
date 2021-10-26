CREATE TABLE [dbo].[T_Screen] (
    [Codice]               VARCHAR (20)  NOT NULL,
    [Descrizione]          VARCHAR (255) NULL,
    [Attributi]            XML           NULL,
    [Righe]                SMALLINT      NULL,
    [Colonne]              SMALLINT      NULL,
    [CodTipoScreen]        VARCHAR (20)  NULL,
    [AltezzaRigaGrid]      FLOAT (53)    NULL,
    [LarghezzaColonnaGrid] FLOAT (53)    NULL,
    [CaricaPerRiga]        BIT           NULL,
    [AdattaAltezzaRighe]   BIT           NULL,
    [Predefinito]          BIT           NULL,
    CONSTRAINT [PK_T_Screen] PRIMARY KEY CLUSTERED ([Codice] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_CodTipoScreen]
    ON [dbo].[T_Screen]([CodTipoScreen] ASC);

