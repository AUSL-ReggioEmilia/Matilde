CREATE TABLE [dbo].[T_TestiNotePredefiniti] (
    [Codice]          VARCHAR (20)   NOT NULL,
    [Descrizione]     VARCHAR (255)  NULL,
    [OggettoNota]     VARCHAR (2000) NULL,
    [DescrizioneNota] VARCHAR (2000) NULL,
    [Path]            VARCHAR (900)  NULL,
    [Colore]          VARCHAR (50)   NULL,
    CONSTRAINT [PK_T_TestiNotePredefiniti] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

