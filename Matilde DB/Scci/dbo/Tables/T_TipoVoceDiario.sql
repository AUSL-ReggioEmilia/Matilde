CREATE TABLE [dbo].[T_TipoVoceDiario] (
    [Codice]            VARCHAR (20)  NOT NULL,
    [Descrizione]       VARCHAR (255) NULL,
    [CodTipoDiario]     VARCHAR (20)  NULL,
    [CodScheda]         VARCHAR (20)  NULL,
    [CopiaDaPrecedente] BIT           NULL,
    CONSTRAINT [PK_T_TipoVoceDiario] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoVoceDiario_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice]),
    CONSTRAINT [FK_T_TipoVoceDiario_T_TipoDiario] FOREIGN KEY ([CodTipoDiario]) REFERENCES [dbo].[T_TipoDiario] ([Codice])
);

