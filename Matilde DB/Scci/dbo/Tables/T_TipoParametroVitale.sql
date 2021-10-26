CREATE TABLE [dbo].[T_TipoParametroVitale] (
    [Codice]       VARCHAR (20)    NOT NULL,
    [Descrizione]  VARCHAR (255)   NULL,
    [Icona]        VARBINARY (MAX) NULL,
    [Colore]       VARCHAR (50)    NULL,
    [CampiFUT]     VARCHAR (2000)  NULL,
    [CampiGrafici] XML             NULL,
    [CodScheda]    VARCHAR (20)    NULL,
    [Ordine]       INT             NULL,
    CONSTRAINT [PK_T_TipoParametriVitali] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoParametroVitale_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);

