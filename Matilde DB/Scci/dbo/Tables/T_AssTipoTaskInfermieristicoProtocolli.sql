CREATE TABLE [dbo].[T_AssTipoTaskInfermieristicoProtocolli] (
    [CodTipoTaskInfermieristico] VARCHAR (20) NOT NULL,
    [CodProtocollo]              VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssTipoTaskInfermieristicoProtocolli] PRIMARY KEY CLUSTERED ([CodTipoTaskInfermieristico] ASC, [CodProtocollo] ASC),
    CONSTRAINT [FK_T_AssTipoTaskInfermieristicoProtocolli_T_Protocolli] FOREIGN KEY ([CodProtocollo]) REFERENCES [dbo].[T_Protocolli] ([Codice]),
    CONSTRAINT [FK_T_AssTipoTaskInfermieristicoProtocolli_T_TipoTaskInfermieristico] FOREIGN KEY ([CodTipoTaskInfermieristico]) REFERENCES [dbo].[T_TipoTaskInfermieristico] ([Codice])
);

