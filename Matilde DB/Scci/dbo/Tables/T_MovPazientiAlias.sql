CREATE TABLE [dbo].[T_MovPazientiAlias] (
    [IDNum]             INT              IDENTITY (1, 1) NOT NULL,
    [IDPazienteNuovo]   UNIQUEIDENTIFIER NULL,
    [CodSACNuovo]       UNIQUEIDENTIFIER NULL,
    [IDPazienteVecchio] UNIQUEIDENTIFIER NULL,
    [CodSACVecchio]     UNIQUEIDENTIFIER NULL,
    [DataEvento]        DATETIME         NULL,
    [DataEventoUTC]     DATE             NULL,
    [IDEvento]          VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovPazientiAlias] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_IDPazienteVecchio]
    ON [dbo].[T_MovPazientiAlias]([IDPazienteVecchio] ASC)
    INCLUDE([CodSACNuovo]);

