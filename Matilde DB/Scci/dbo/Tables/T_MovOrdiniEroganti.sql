CREATE TABLE [dbo].[T_MovOrdiniEroganti] (
    [ID]            UNIQUEIDENTIFIER NOT NULL,
    [IDNum]         INT              IDENTITY (1, 1) NOT NULL,
    [IDOrdine]      UNIQUEIDENTIFIER NOT NULL,
    [CodTipoOrdine] VARCHAR (20)     NULL,
    CONSTRAINT [PK_T_MovOrdiniEroganti] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovOrdiniEroganti_T_MovOrdini] FOREIGN KEY ([IDOrdine]) REFERENCES [dbo].[T_MovOrdini] ([ID]),
    CONSTRAINT [FK_T_MovOrdiniEroganti_T_TipoOrdine] FOREIGN KEY ([CodTipoOrdine]) REFERENCES [dbo].[T_TipoOrdine] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodTipoOrdine]
    ON [dbo].[T_MovOrdiniEroganti]([CodTipoOrdine] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDOrdine]
    ON [dbo].[T_MovOrdiniEroganti]([IDOrdine] ASC);

