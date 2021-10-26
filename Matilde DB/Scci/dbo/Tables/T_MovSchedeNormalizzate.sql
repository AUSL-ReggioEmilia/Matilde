CREATE TABLE [dbo].[T_MovSchedeNormalizzate] (
    [IDNum]               INT              IDENTITY (1, 1) NOT NULL,
    [IDScheda]            UNIQUEIDENTIFIER NULL,
    [DataNormalizzazione] DATETIME         NULL,
    [DataAggiornamento]   DATETIME         NULL,
    [FlagErrore]          BIT              NULL,
    [Errore]              NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_T_MovSchedeNormalizzate] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_IDScheda_Include_DataNormalizzazione]
    ON [dbo].[T_MovSchedeNormalizzate]([IDScheda] ASC)
    INCLUDE([DataNormalizzazione]);


GO
CREATE NONCLUSTERED INDEX [IX_DataNormalizzazione]
    ON [dbo].[T_MovSchedeNormalizzate]([DataNormalizzazione] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IDScheda]
    ON [dbo].[T_MovSchedeNormalizzate]([IDScheda] ASC);

