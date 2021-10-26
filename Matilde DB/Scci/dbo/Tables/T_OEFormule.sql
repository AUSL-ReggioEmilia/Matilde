CREATE TABLE [dbo].[T_OEFormule] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [CodUA]             VARCHAR (20)   NOT NULL,
    [CodAzienda]        VARCHAR (20)   NOT NULL,
    [CodErogante]       VARCHAR (100)  NOT NULL,
    [CodPrestazione]    VARCHAR (100)  NOT NULL,
    [CodDatoAccessorio] VARCHAR (100)  NOT NULL,
    [Formula]           VARCHAR (4000) NULL,
    CONSTRAINT [PK_T_OEFormule_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_OEFormule_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CodUACodAziendaEogantePrestazioneDatoAccessorio]
    ON [dbo].[T_OEFormule]([CodUA] ASC, [CodAzienda] ASC, [CodErogante] ASC, [CodPrestazione] ASC, [CodDatoAccessorio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUA]
    ON [dbo].[T_OEFormule]([CodUA] ASC);

