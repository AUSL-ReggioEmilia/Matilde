CREATE TABLE [dbo].[T_AssTipoPrescrizioneTempi] (
    [CodTipoPrescrizione]      VARCHAR (20) NOT NULL,
    [CodTipoPrescrizioneTempi] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssTipoPrescrizioneTempi] PRIMARY KEY CLUSTERED ([CodTipoPrescrizione] ASC, [CodTipoPrescrizioneTempi] ASC),
    CONSTRAINT [FK_T_AssTipoPrescrizione_Tempi] FOREIGN KEY ([CodTipoPrescrizioneTempi]) REFERENCES [dbo].[T_TipoPrescrizioneTempi] ([Codice])
);

