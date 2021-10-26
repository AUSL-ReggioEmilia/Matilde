CREATE TABLE [dbo].[T_DCDecodificheValori] (
    [CodDec]      VARCHAR (20)    NOT NULL,
    [Codice]      VARCHAR (50)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Ordine]      INT             NULL,
    [DtValI]      DATETIME        NULL,
    [DtValF]      DATETIME        NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [InfoRTF]     VARCHAR (MAX)   NULL,
    [Path]        VARCHAR (255)   NULL,
    CONSTRAINT [PK_T_DCDecodificheValori] PRIMARY KEY CLUSTERED ([CodDec] ASC, [Codice] ASC),
    CONSTRAINT [FK_T_DCDecodificheValori_T_DCDecodifiche] FOREIGN KEY ([CodDec]) REFERENCES [dbo].[T_DCDecodifiche] ([Codice])
);

