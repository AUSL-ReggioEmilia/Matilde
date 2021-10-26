CREATE TABLE [dbo].[T_MovNewsLog] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [IDNum]            NUMERIC (18)     IDENTITY (1, 1) NOT NULL,
    [CodNews]          VARCHAR (20)     NULL,
    [CodUtenteVisione] VARCHAR (100)    NULL,
    [DataVisione]      DATETIME         NULL,
    [DataVisioneUTC]   DATETIME         NULL,
    CONSTRAINT [PK_T_MovNewsLog] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_T_MovNewsLog_T_Login] FOREIGN KEY ([CodUtenteVisione]) REFERENCES [dbo].[T_Login] ([Codice]),
    CONSTRAINT [FK_T_MovNewsLog_T_MovNews] FOREIGN KEY ([CodNews]) REFERENCES [dbo].[T_MovNews] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_CodUtente]
    ON [dbo].[T_MovNewsLog]([CodUtenteVisione] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodNews]
    ON [dbo].[T_MovNewsLog]([CodNews] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IDNum]
    ON [dbo].[T_MovNewsLog]([IDNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CodiceCodUtente]
    ON [dbo].[T_MovNewsLog]([CodUtenteVisione] ASC, [CodNews] ASC);

