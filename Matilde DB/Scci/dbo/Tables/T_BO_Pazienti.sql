CREATE TABLE [dbo].[T_BO_Pazienti] (
    [IDNum]                 INT              IDENTITY (1, 1) NOT NULL,
    [CodEntita]             VARCHAR (20)     NULL,
    [IDPazienteRiferimento] UNIQUEIDENTIFIER NULL,
    [NumeroNosologico]      VARCHAR (20)     NULL,
    [NumeroListaAttesa]     VARCHAR (20)     NULL,
    [CodStatoElaborazione]  VARCHAR (20)     NULL,
    [DataElaborazione]      DATETIME         NULL,
    [IDPaziente]            UNIQUEIDENTIFIER NULL,
    [IDEpisodio]            UNIQUEIDENTIFIER NULL,
    [Note]                  VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_T_BO_Pazienti] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);

