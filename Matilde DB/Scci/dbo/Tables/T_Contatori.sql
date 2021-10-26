CREATE TABLE [dbo].[T_Contatori] (
    [Codice]                  VARCHAR (20)  NOT NULL,
    [Descrizione]             VARCHAR (200) NOT NULL,
    [Valore]                  BIGINT        NULL,
    [DataImpostazione]        DATETIME      NULL,
    [DataImpostazioneUTC]     DATETIME      NULL,
    [DataUltimaModifica]      DATETIME      NULL,
    [DataUltimaModificaUTC]   DATETIME      NULL,
    [CodUtenteImpostazione]   VARCHAR (100) NULL,
    [CodUtenteUltimaModifica] VARCHAR (100) NULL,
    [DataScadenza]            DATETIME      NULL,
    [CodUnitaScadenza]        VARCHAR (20)  NULL,
    [Sistema]                 BIT           NULL,
    CONSTRAINT [PK_T_Contatori] PRIMARY KEY CLUSTERED ([Codice] ASC) WITH (FILLFACTOR = 90)
);



