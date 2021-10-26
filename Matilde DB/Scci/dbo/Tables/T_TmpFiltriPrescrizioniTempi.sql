CREATE TABLE [dbo].[T_TmpFiltriPrescrizioniTempi] (
    [IDSessione]          UNIQUEIDENTIFIER NOT NULL,
    [IDPrescrizioneTempi] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TmpFiltriPrescrizioniTempi] PRIMARY KEY NONCLUSTERED ([IDSessione] ASC, [IDPrescrizioneTempi] ASC)
);

