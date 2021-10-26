CREATE TABLE [dbo].[T_MovTranscodifiche] (
    [IDSessione]           UNIQUEIDENTIFIER NOT NULL,
    [CodEntita]            NCHAR (10)       NOT NULL,
    [IDOrigine]            UNIQUEIDENTIFIER NOT NULL,
    [IDDestinazione]       UNIQUEIDENTIFIER NULL,
    [CodUtenteInserimento] VARCHAR (100)    NULL,
    [DataInserimento]      DATETIME         NULL,
    CONSTRAINT [PK_T_MovTranscodifiche] PRIMARY KEY CLUSTERED ([IDSessione] ASC, [CodEntita] ASC, [IDOrigine] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaIDDestinazione]
    ON [dbo].[T_MovTranscodifiche]([CodEntita] ASC, [IDDestinazione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDDestinazione]
    ON [dbo].[T_MovTranscodifiche]([IDDestinazione] ASC);

