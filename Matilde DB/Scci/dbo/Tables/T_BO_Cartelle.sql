CREATE TABLE [dbo].[T_BO_Cartelle] (
    [IDNum]                INT              IDENTITY (1, 1) NOT NULL,
    [NumeroNosologico]     VARCHAR (20)     NULL,
    [NumeroListaAttesa]    VARCHAR (20)     NULL,
    [CodUA]                VARCHAR (20)     NULL,
    [NumCartella]          VARCHAR (20)     NULL,
    [CodStatoElaborazione] VARCHAR (20)     NULL,
    [DataElaborazione]     DATETIME         NULL,
    [IDPaziente]           UNIQUEIDENTIFIER NULL,
    [IDEpisodio]           UNIQUEIDENTIFIER NULL,
    [IDTrasferimento]      UNIQUEIDENTIFIER NULL,
    [IDCartella]           UNIQUEIDENTIFIER NULL,
    [Note]                 VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_T_BO_AperturaCartelle] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);

