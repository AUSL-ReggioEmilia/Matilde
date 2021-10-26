CREATE TABLE [dbo].[T_StatoPrescrizioneTempi] (
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_T_StatoPrescrizioneTempi] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

