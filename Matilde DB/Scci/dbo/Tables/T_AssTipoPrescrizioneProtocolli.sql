CREATE TABLE [dbo].[T_AssTipoPrescrizioneProtocolli] (
    [CodTipoPrescrizione] VARCHAR (20) NOT NULL,
    [CodProtocollo]       VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssTipoPrescrizioneProtocolli] PRIMARY KEY CLUSTERED ([CodTipoPrescrizione] ASC, [CodProtocollo] ASC),
    CONSTRAINT [FK_T_AssTipoPrescrizioneProtocolli_T_Protocolli] FOREIGN KEY ([CodProtocollo]) REFERENCES [dbo].[T_Protocolli] ([Codice]),
    CONSTRAINT [FK_T_AssTipoPrescrizioneProtocolli_T_TipoPrescrizione] FOREIGN KEY ([CodTipoPrescrizione]) REFERENCES [dbo].[T_TipoPrescrizione] ([Codice])
);

