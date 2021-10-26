CREATE TABLE [dbo].[T_SchedeVersioni] (
    [CodScheda]        VARCHAR (20)  NOT NULL,
    [Versione]         INT           NOT NULL,
    [Descrizione]      VARCHAR (255) NULL,
    [FlagAttiva]       BIT           CONSTRAINT [DF_T_SchedeVersioni_FlagAttiva] DEFAULT ((1)) NULL,
    [Pubblicato]       BIT           NULL,
    [DtValI]           DATETIME      NULL,
    [DtValF]           DATETIME      NULL,
    [CampiRilevanti]   VARCHAR (MAX) NULL,
    [CampiObbligatori] VARCHAR (MAX) NULL,
    [Struttura_old]    XML           NULL,
    [Struttura]        XML           NULL,
    [Layout]           XML           NULL,
    [Layout_OLD]       XML           NULL,
    [StrutturaV3]      XML           NULL,
    [LayoutV3]         XML           NULL,
    CONSTRAINT [PK_T_SchedeVersioni] PRIMARY KEY CLUSTERED ([CodScheda] ASC, [Versione] ASC),
    CONSTRAINT [FK_T_SchedeVersioni_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Date]
    ON [dbo].[T_SchedeVersioni]([DtValI] ASC, [DtValF] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DtValI]
    ON [dbo].[T_SchedeVersioni]([DtValI] ASC)
    INCLUDE([FlagAttiva], [Pubblicato], [DtValF]);

