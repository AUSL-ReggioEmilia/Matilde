CREATE TABLE [dbo].[T_Integra_AssCampiDestinatari] (
    [CodCampo]        VARCHAR (20) NOT NULL,
    [CodDestinatario] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_Integra] PRIMARY KEY CLUSTERED ([CodCampo] ASC, [CodDestinatario] ASC),
    CONSTRAINT [FK_T_Integra_AssCampiDestinatari_T_Integra_Campi] FOREIGN KEY ([CodCampo]) REFERENCES [dbo].[T_Integra_Campi] ([Codice]),
    CONSTRAINT [FK_T_Integra_AssCampiDestinatari_T_Integra_Destinatari] FOREIGN KEY ([CodDestinatario]) REFERENCES [dbo].[T_Integra_Destinatari] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_Destinatario]
    ON [dbo].[T_Integra_AssCampiDestinatari]([CodDestinatario] ASC);

