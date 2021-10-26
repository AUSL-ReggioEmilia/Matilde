CREATE TABLE [dbo].[T_AssTipoTaskInfermieristicoTempi] (
    [CodTipoTaskInfermieristico]      VARCHAR (20) NOT NULL,
    [CodTipoTaskInfermieristicoTempi] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssTipoTaskInfermieristicoTempi] PRIMARY KEY CLUSTERED ([CodTipoTaskInfermieristico] ASC, [CodTipoTaskInfermieristicoTempi] ASC),
    CONSTRAINT [FK_T_AssTipoTaskInfermieristicoTempi] FOREIGN KEY ([CodTipoTaskInfermieristicoTempi]) REFERENCES [dbo].[T_TipoTaskInfermieristicoTempi] ([Codice])
);

