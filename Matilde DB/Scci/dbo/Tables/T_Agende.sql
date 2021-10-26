CREATE TABLE [dbo].[T_Agende] (
    [Codice]                      VARCHAR (20)  NOT NULL,
    [Descrizione]                 VARCHAR (255) NULL,
    [CodTipoAgenda]               VARCHAR (20)  NULL,
    [Colore]                      VARCHAR (255) NULL,
    [ElencoCampi]                 XML           NULL,
    [IntervalloSlot]              SMALLINT      NULL,
    [OrariLavoro]                 XML           NULL,
    [CodEntita]                   VARCHAR (20)  NULL,
    [Ordine]                      INT           NULL,
    [UsaColoreTipoAppuntamento]   BIT           NULL,
    [DescrizioneAlternativa]      VARCHAR (255) NULL,
    [MassimoAnticipoPrenotazione] INT           NULL,
    [MassimoRitardoPrenotazione]  INT           NULL,
    [Lista]                       BIT           NULL,
    [ParametriLista]              XML           NULL,
    [Risorse]                     XML           NULL,
    [EscludiFestivita]            BIT           NULL,
    [CodPeriodoDisponibilita]     VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_Agende] PRIMARY KEY CLUSTERED ([Codice] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_T_Agende_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_Agende_T_TipoAgenda] FOREIGN KEY ([CodTipoAgenda]) REFERENCES [dbo].[T_TipoAgenda] ([Codice])
);



