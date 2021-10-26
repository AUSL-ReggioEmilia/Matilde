CREATE TABLE [dbo].[T_SchedeCopia] (
    [CodScheda]      VARCHAR (20) NOT NULL,
    [CodSchedaCopia] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_SchedeCopia] PRIMARY KEY CLUSTERED ([CodScheda] ASC, [CodSchedaCopia] ASC),
    CONSTRAINT [FK_T_SchedeCopia_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice]),
    CONSTRAINT [FK_T_SchedeCopia_T_Schede1] FOREIGN KEY ([CodSchedaCopia]) REFERENCES [dbo].[T_Schede] ([Codice])
);


GO
CREATE NONCLUSTERED INDEX [IX_CodScheda]
    ON [dbo].[T_SchedeCopia]([CodScheda] ASC);

