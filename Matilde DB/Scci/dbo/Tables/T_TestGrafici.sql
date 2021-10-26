CREATE TABLE [dbo].[T_TestGrafici] (
    [ID]        NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [Tipo]      VARCHAR (20) NULL,
    [Parametro] VARCHAR (50) NULL,
    [Data]      DATETIME     NULL,
    [Valore]    INT          NULL,
    CONSTRAINT [PK_T_TestGrafici] PRIMARY KEY CLUSTERED ([ID] ASC)
);

