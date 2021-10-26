CREATE TABLE [dbo].[T_PazientiConsensi] (
    [IDPaziente]            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                 INT              IDENTITY (1, 1) NOT NULL,
    [CodTipoConsenso]       VARCHAR (20)     NOT NULL,
    [CodSistemaProvenienza] VARCHAR (50)     NULL,
    [IDProvenienza]         VARCHAR (100)    NULL,
    [CodStatoConsenso]      VARCHAR (20)     NULL,
    [DataConsenso]          DATETIME         NULL,
    [DataDisattivazione]    DATETIME         NULL,
    [DataInserimento]       DATETIME         NULL,
    [DataInserimentoUTC]    DATETIME         NULL,
    [DataUltimaModifica]    DATETIME         NULL,
    [DataUltimaModificaUTC] DATETIME         NULL,
    [CodOperatore]          VARCHAR (255)    NULL,
    [CognomeOperatore]      VARCHAR (255)    NULL,
    [NomeOperatore]         VARCHAR (255)    NULL,
    [ComputerOperatore]     VARCHAR (255)    NULL,
    CONSTRAINT [PK_T_PazientiConsensi] PRIMARY KEY NONCLUSTERED ([IDPaziente] ASC, [CodTipoConsenso] ASC),
    CONSTRAINT [FK_T_PazientiConsensi_T_Pazienti] FOREIGN KEY ([IDPaziente]) REFERENCES [dbo].[T_Pazienti] ([ID]),
    CONSTRAINT [FK_T_PazientiConsensi_T_StatoConsenso] FOREIGN KEY ([CodStatoConsenso]) REFERENCES [dbo].[T_StatoConsenso] ([Codice]),
    CONSTRAINT [FK_T_PazientiConsensi_T_TipoConsenso] FOREIGN KEY ([CodTipoConsenso]) REFERENCES [dbo].[T_TipoConsenso] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDPazienteCodTipoConsenso_CodStatoConsenso]
    ON [dbo].[T_PazientiConsensi]([IDPaziente] ASC, [CodTipoConsenso] ASC, [CodStatoConsenso] ASC);

