CREATE TABLE [dbo].[T_FestivitaAgende] (
    [Data]      DATE         NOT NULL,
    [CodAgenda] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_FestivitaAgende] PRIMARY KEY CLUSTERED ([Data] ASC, [CodAgenda] ASC),
    CONSTRAINT [FK_T_FestivitaAgende_T_Agende] FOREIGN KEY ([CodAgenda]) REFERENCES [dbo].[T_Agende] ([Codice]),
    CONSTRAINT [FK_T_FestivitaAgende_T_Festivita] FOREIGN KEY ([Data]) REFERENCES [dbo].[T_Festivita] ([Data])
);


GO
CREATE NONCLUSTERED INDEX [IX_Data]
    ON [dbo].[T_FestivitaAgende]([Data] ASC);

