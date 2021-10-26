CREATE TABLE [dbo].[T_Festivita] (
    [Data]        DATE          NOT NULL,
    [Descrizione] VARCHAR (255) NULL,
    CONSTRAINT [PK_T_Festivita] PRIMARY KEY CLUSTERED ([Data] ASC)
);

