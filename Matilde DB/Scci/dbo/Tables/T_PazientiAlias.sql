CREATE TABLE [dbo].[T_PazientiAlias] (
    [IDNum]             INT              IDENTITY (1, 1) NOT NULL,
    [IDPazienteVecchio] UNIQUEIDENTIFIER NOT NULL,
    [IDPaziente]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_T_PazientiAlias] PRIMARY KEY NONCLUSTERED ([IDPazienteVecchio] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_PazientiAlias]([IDPaziente] ASC);

