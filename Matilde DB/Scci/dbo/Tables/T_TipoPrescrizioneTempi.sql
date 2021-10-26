CREATE TABLE [dbo].[T_TipoPrescrizioneTempi] (
    [Codice]      VARCHAR (20)  NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    [Ordine]      INT           NULL,
    CONSTRAINT [PK_T_TipoPrescrizioneTempi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

