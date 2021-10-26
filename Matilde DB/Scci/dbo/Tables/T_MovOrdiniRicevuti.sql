CREATE TABLE [dbo].[T_MovOrdiniRicevuti] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [IDNum]                 INT              IDENTITY (1, 1) NOT NULL,
    [IDOrdineOE]            VARCHAR (50)     NULL,
    [DatiOE]                XML              NOT NULL,
    [DataInserimento]       DATETIME         NULL,
    [DataInserimentoUTC]    DATETIME         NULL,
    [DataUltimaModifica]    DATETIME         NULL,
    [DataUltimaModificaUTC] DATETIME         NULL,
    CONSTRAINT [PK_T_MovOrdiniRicevuti] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

