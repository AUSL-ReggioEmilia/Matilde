CREATE TABLE [dbo].[T_MovReport] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [IDNum]           INT              IDENTITY (1, 1) NOT NULL,
    [IDPaziente]      UNIQUEIDENTIFIER NULL,
    [IDEpisodio]      UNIQUEIDENTIFIER NULL,
    [IDTrasferimento] UNIQUEIDENTIFIER NULL,
    [IDCartella]      UNIQUEIDENTIFIER NULL,
    [CodReport]       VARCHAR (20)     NULL,
    [Documento]       VARBINARY (MAX)  NULL,
    [DataEvento]      DATETIME         NULL,
    [DataEventoUTC]   DATETIME         NULL,
    [CodLogin]        VARCHAR (100)    NULL,
    [CodRuolo]        VARCHAR (20)     NULL,
    [NomePC]          VARCHAR (50)     NULL,
    [IndirizzoIP]     VARCHAR (50)     NULL,
    CONSTRAINT [PK_T_MovReport] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovReport_T_MovEpisodi] FOREIGN KEY ([IDEpisodio]) REFERENCES [dbo].[T_MovEpisodi] ([ID]),
    CONSTRAINT [FK_T_MovReport_T_Report] FOREIGN KEY ([CodReport]) REFERENCES [dbo].[T_Report] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_IDTraferimento]
    ON [dbo].[T_MovReport]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDTrasferimento]
    ON [dbo].[T_MovReport]([IDTrasferimento] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDEpisodio]
    ON [dbo].[T_MovReport]([IDEpisodio] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IDPaziente]
    ON [dbo].[T_MovReport]([IDPaziente] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovReport]([IDNum] ASC);

