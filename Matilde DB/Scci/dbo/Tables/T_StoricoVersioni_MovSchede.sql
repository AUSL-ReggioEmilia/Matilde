CREATE TABLE [dbo].[T_StoricoVersioni_MovSchede] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [IDNum]        INT              IDENTITY (1, 1) NOT NULL,
    [IDMovScheda]  UNIQUEIDENTIFIER NULL,
    [CodScheda]    VARCHAR (20)     NULL,
    [Versione]     INT              NULL,
    [Numero]       INT              NULL,
    [Dati]         XML              NULL,
    [DataOraCopia] DATETIME         NULL,
    CONSTRAINT [PK_T_StoricoVersioni_MovSchede] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

