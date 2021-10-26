CREATE TABLE [dbo].[T_SchedeLayout] (
    [CodScheda]     VARCHAR (20)   NOT NULL,
    [Versione]      INT            NOT NULL,
    [Codice]        VARCHAR (20)   NOT NULL,
    [Descrizione]   VARCHAR (100)  NULL,
    [CodTipoLayout] VARCHAR (20)   NULL,
    [LayoutXML]     XML            NOT NULL,
    [Default]       BIT            NOT NULL,
    [Stylesheet]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_SchedeLayout_1] PRIMARY KEY CLUSTERED ([CodScheda] ASC, [Versione] ASC, [Codice] ASC),
    CONSTRAINT [FK_T_SchedeLayout_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice]),
    CONSTRAINT [FK_T_SchedeLayout_T_SchedeTipoLayout] FOREIGN KEY ([CodTipoLayout]) REFERENCES [dbo].[T_SchedeTipoLayout] ([Codice])
);

