CREATE TABLE [dbo].[T_MovNews] (
    [ID]                      NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [Codice]                  VARCHAR (20)    NOT NULL,
    [DataOra]                 DATETIME        NULL,
    [DataInizioPubblicazione] DATETIME        NULL,
    [DataFinePubblicazione]   DATETIME        NULL,
    [Titolo]                  VARCHAR (255)   NULL,
    [TestoRTF]                VARCHAR (MAX)   NULL,
    [Rilevante]               BIT             NULL,
    [TestRTFbinario]          VARBINARY (MAX) NULL,
    [CodTipoNews]             VARCHAR (20)    NULL,
    CONSTRAINT [PK_T_MovNews_1] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_MovNews_T_TipoNews] FOREIGN KEY ([CodTipoNews]) REFERENCES [dbo].[T_TipoNews] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodTipoNews]
    ON [dbo].[T_MovNews]([CodTipoNews] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataFinePubblicazione]
    ON [dbo].[T_MovNews]([DataFinePubblicazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataInizioPubblicazione]
    ON [dbo].[T_MovNews]([DataInizioPubblicazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RilevanteData]
    ON [dbo].[T_MovNews]([Rilevante] ASC, [DataOra] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DataInizioDataFineCodTipoCodice]
    ON [dbo].[T_MovNews]([DataInizioPubblicazione] ASC, [DataFinePubblicazione] ASC, [CodTipoNews] ASC, [Codice] ASC);

