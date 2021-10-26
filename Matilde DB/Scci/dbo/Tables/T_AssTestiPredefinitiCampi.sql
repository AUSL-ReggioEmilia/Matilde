CREATE TABLE [dbo].[T_AssTestiPredefinitiCampi] (
    [CodTestoPredefinito] VARCHAR (20) NOT NULL,
    [CodEntita]           VARCHAR (20) NOT NULL,
    [CodTipoEntita]       VARCHAR (20) NOT NULL,
    [CodCampo]            VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_T_AssTestiPredefinitiCampi] PRIMARY KEY CLUSTERED ([CodTestoPredefinito] ASC, [CodEntita] ASC, [CodTipoEntita] ASC, [CodCampo] ASC),
    CONSTRAINT [FK_T_AssTestiPredefinitiCampi_T_Entita] FOREIGN KEY ([CodEntita]) REFERENCES [dbo].[T_Entita] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodEntitaCodTipoEntitaCodCampo]
    ON [dbo].[T_AssTestiPredefinitiCampi]([CodEntita] ASC, [CodTipoEntita] ASC, [CodCampo] ASC);

