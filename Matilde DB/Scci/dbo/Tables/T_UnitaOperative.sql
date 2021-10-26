CREATE TABLE [dbo].[T_UnitaOperative] (
    [CodAzi]         VARCHAR (20)  NOT NULL,
    [Codice]         VARCHAR (20)  NOT NULL,
    [Descrizione]    VARCHAR (255) NULL,
    [Interaziendale] BIT           NULL,
    CONSTRAINT [PK_T_UnitaOperative] PRIMARY KEY CLUSTERED ([CodAzi] ASC, [Codice] ASC),
    CONSTRAINT [FK_T_UnitaOperative_T_Aziende] FOREIGN KEY ([CodAzi]) REFERENCES [dbo].[T_Aziende] ([Codice])
);



