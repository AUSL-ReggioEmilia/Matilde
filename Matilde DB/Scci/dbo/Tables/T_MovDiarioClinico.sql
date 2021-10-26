CREATE TABLE [dbo].[T_MovDiarioClinico] (
    [ID]                     UNIQUEIDENTIFIER NOT NULL,
    [CodUA]                  VARCHAR (20)     NULL,
    [IDNum]                  INT              IDENTITY (1, 1) NOT NULL,
    [DataEvento]             DATETIME         NULL,
    [DataEventoUTC]          DATETIME         NULL,
    [IDEpisodio]             UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]        UNIQUEIDENTIFIER NULL,
    [CodTipoDiario]          VARCHAR (20)     NULL,
    [CodTipoVoceDiario]      VARCHAR (20)     NULL,
    [CodTipoRegistrazione]   VARCHAR (20)     NULL,
    [CodEntitaRegistrazione] VARCHAR (20)     NULL,
    [IDEntitaRegistrazione]  UNIQUEIDENTIFIER NULL,
    [CodStatoDiario]         VARCHAR (20)     NULL,
    [CodUtenteRilevazione]   VARCHAR (100)    NULL,
    [DataInserimento]        DATETIME         NULL,
    [DataInserimentoUTC]     DATETIME         NULL,
    [DataValidazione]        DATETIME         NULL,
    [DataValidazioneUTC]     DATETIME         NULL,
    [DataAnnullamento]       DATETIME         NULL,
    [DataAnnullamentoUTC]    DATETIME         NULL,
    [CodSistema]             VARCHAR (20)     NULL,
    [IDSistema]              VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovDiarioClinico] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovDiarioClinico_T_Entita] FOREIGN KEY ([CodEntitaRegistrazione]) REFERENCES [dbo].[T_Entita] ([Codice]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_MovTrasferimenti] FOREIGN KEY ([IDTrasferimento]) REFERENCES [dbo].[T_MovTrasferimenti] ([ID]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_StatoDiario] FOREIGN KEY ([CodStatoDiario]) REFERENCES [dbo].[T_StatoDiario] ([Codice]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_TipoDiario] FOREIGN KEY ([CodTipoDiario]) REFERENCES [dbo].[T_TipoDiario] ([Codice]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_TipoRegistrazione] FOREIGN KEY ([CodTipoRegistrazione]) REFERENCES [dbo].[T_TipoRegistrazione] ([Codice]),
    CONSTRAINT [FK_T_MovDiarioClinico_T_TipoVoceDiario] FOREIGN KEY ([CodTipoVoceDiario]) REFERENCES [dbo].[T_TipoVoceDiario] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodStatoDiario_CodUtenteRilevazione]
    ON [dbo].[T_MovDiarioClinico]([CodUtenteRilevazione] ASC, [CodStatoDiario] ASC)
    INCLUDE([IDTrasferimento]);


GO
CREATE NONCLUSTERED INDEX [IX_EntitaRegistrazione]
    ON [dbo].[T_MovDiarioClinico]([CodEntitaRegistrazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TipoRegistrazione]
    ON [dbo].[T_MovDiarioClinico]([CodTipoRegistrazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TipoVoce]
    ON [dbo].[T_MovDiarioClinico]([CodTipoVoceDiario] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Episodio]
    ON [dbo].[T_MovDiarioClinico]([IDEpisodio] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovDiarioClinico]([IDNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'da Valorizzare solo per CodTipoSorgente=A (Automatico), impostata in automatico dal motore con filtro su SistemaEsterno=1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'T_MovDiarioClinico', @level2type = N'COLUMN', @level2name = N'CodEntitaRegistrazione';


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento_CodStatoDiario]
    ON [dbo].[T_MovDiarioClinico]([IDTrasferimento] ASC, [CodStatoDiario] ASC)
    INCLUDE([ID], [DataEvento], [CodTipoVoceDiario], [CodEntitaRegistrazione], [CodUtenteRilevazione], [DataValidazione]);

