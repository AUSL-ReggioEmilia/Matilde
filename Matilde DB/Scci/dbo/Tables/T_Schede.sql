CREATE TABLE [dbo].[T_Schede] (
    [Codice]                          VARCHAR (20)   NOT NULL,
    [Descrizione]                     VARCHAR (255)  NULL,
    [Note]                            VARCHAR (2000) NULL,
    [Path]                            VARCHAR (900)  NULL,
    [CodTipoScheda]                   VARCHAR (20)   NULL,
    [SchedaSemplice]                  BIT            NULL,
    [CodEntita]                       VARCHAR (20)   NULL,
    [Ordine]                          VARCHAR (20)   NULL,
    [NumerositaMinima]                INT            NULL,
    [NumerositaMassima]               INT            NULL,
    [CreaDefault]                     BIT            NULL,
    [CodEntita2]                      VARCHAR (20)   NULL,
    [EsportaDWH]                      BIT            NULL,
    [IgnoraStampaCartella]            BIT            NULL,
    [Validabile]                      BIT            NULL,
    [Riservata]                       BIT            NULL,
    [EsportaDWHSingola]               BIT            NULL,
    [CodModalitaCopiaPrecedente]      VARCHAR (20)   NULL,
    [CodEntita3]                      VARCHAR (20)   NULL,
    [SistemaDWH]                      VARCHAR (50)   NULL,
    [Revisione]                       BIT            NULL,
    [CodPrestazioneDWH]               VARCHAR (50)   NULL,
    [DescrizionePrestazioneDWH]       VARCHAR (255)  NULL,
    [Contenitore]                     BIT            NULL,
    [AlertSchedaVuota]                BIT            NULL,
    [CopiaPrecedenteSelezione]        BIT            NULL,
    [FirmaDigitale]                   BIT            NULL,
    [CodReportDWH]                    VARCHAR (20)   NULL,
    [CancellaPrecedentiDWH]           BIT            NULL,
    [CartellaAmbulatorialeCodificata] BIT            NULL,
    [DescrizioneAlternativa]          VARCHAR (255)  NULL,
    [CodContatore]                    VARCHAR (20)   NULL,
    [EsportaLayerDWH]                 BIT            NULL,
    CONSTRAINT [PK_T_Schede] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_Schede_T_Contatori] FOREIGN KEY ([CodContatore]) REFERENCES [dbo].[T_Contatori] ([Codice]),
    CONSTRAINT [FK_T_Schede_T_ModalitaCopiaPrecedente] FOREIGN KEY ([CodModalitaCopiaPrecedente]) REFERENCES [dbo].[T_ModalitaCopiaPrecedente] ([Codice]),
    CONSTRAINT [FK_T_Schede_T_Report] FOREIGN KEY ([CodReportDWH]) REFERENCES [dbo].[T_Report] ([Codice])
);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Se schedaSemplice=1 allore CreaDefault=0.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_Schede', @level2type = N'COLUMN', @level2name = N'CreaDefault';

