CREATE TABLE [dbo].[T_MovSegnalibri] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [IDNum]              INT              IDENTITY (1, 1) NOT NULL,
    [CodUtente]          VARCHAR (100)    NULL,
    [CodRuolo]           VARCHAR (20)     NULL,
    [IDPaziente]         UNIQUEIDENTIFIER NULL,
    [IDCartella]         UNIQUEIDENTIFIER NULL,
    [CodEntita]          VARCHAR (20)     NULL,
    [IDEntita]           UNIQUEIDENTIFIER NULL,
    [CodEntitaScheda]    VARCHAR (20)     NULL,
    [CodScheda]          VARCHAR (20)     NULL,
    [Numero]             INT              NULL,
    [DataInserimento]    DATETIME         NULL,
    [DataInserimentoUTC] DATETIME         NULL,
    CONSTRAINT [PK_T_MovSegnalibri] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovSegnalibri]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUtenteCodRuolo]
    ON [dbo].[T_MovSegnalibri]([CodUtente] ASC, [CodRuolo] ASC);

