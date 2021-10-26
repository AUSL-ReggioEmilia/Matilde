CREATE TABLE [dbo].[T_TipoPrescrizione] (
    [Codice]                 VARCHAR (20)    NOT NULL,
    [Descrizione]            VARCHAR (255)   NULL,
    [Icona]                  VARBINARY (MAX) NULL,
    [CodScheda]              VARCHAR (20)    NULL,
    [CodViaSomministrazione] VARCHAR (20)    NULL,
    [PrescrizioneASchema]    BIT             NULL,
    [NonProseguibile]        BIT             NULL,
    [Path]                   VARCHAR (900)   NULL,
    [CodSchedaPosologia]     VARCHAR (20)    NULL,
    [ColoreGrafico]          VARCHAR (50)    NULL,
    CONSTRAINT [PK_T_TipoOEFarmacologico] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoPrescrizione_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);

