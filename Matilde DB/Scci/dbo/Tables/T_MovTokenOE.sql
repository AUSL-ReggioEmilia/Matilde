CREATE TABLE [dbo].[T_MovTokenOE] (
    [IDNum]        NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [CodUtente]    VARCHAR (100) NULL,
    [IndirizzoIP]  VARCHAR (100) NULL,
    [DataScadenza] DATETIME      NULL,
    [Token]        XML           NULL,
    [CodAzienda]   VARCHAR (20)  NULL,
    CONSTRAINT [PK_T_MovTokenOE] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_DataScadenza]
    ON [dbo].[T_MovTokenOE]([DataScadenza] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodUtenteIP]
    ON [dbo].[T_MovTokenOE]([CodUtente] ASC, [IndirizzoIP] ASC);

