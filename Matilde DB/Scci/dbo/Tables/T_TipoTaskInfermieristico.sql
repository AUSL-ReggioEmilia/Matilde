CREATE TABLE [dbo].[T_TipoTaskInfermieristico] (
    [Codice]            VARCHAR (20)    NOT NULL,
    [Descrizione]       VARCHAR (255)   NULL,
    [Colore]            VARCHAR (50)    NULL,
    [Icona]             VARBINARY (MAX) NULL,
    [CodScheda]         VARCHAR (20)    NULL,
    [Sigla]             VARCHAR (20)    NULL,
    [Anticipo]          INT             NULL,
    [Ripianificazione]  INT             NULL,
    [ErogazioneDiretta] BIT             NULL,
    CONSTRAINT [PK_T_TipoTaskInfermieristici] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoTaskInfermieristico_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);



