CREATE TABLE [dbo].[T_AssUAIntestazioni] (
    [CodUA]           VARCHAR (20)  NOT NULL,
    [CodIntestazione] VARCHAR (20)  NOT NULL,
    [DataInizio]      DATETIME      NOT NULL,
    [DataFine]        DATETIME      NULL,
    [Intestazione]    VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_AssUAIntestazioni_1] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodIntestazione] ASC, [DataInizio] ASC),
    CONSTRAINT [FK_T_AssUAIntestazioni_T_TipoIntestazione] FOREIGN KEY ([CodIntestazione]) REFERENCES [dbo].[T_TipoIntestazione] ([Codice]),
    CONSTRAINT [FK_T_AssUAIntestazioni_T_UnitaAtomiche] FOREIGN KEY ([CodUA]) REFERENCES [dbo].[T_UnitaAtomiche] ([Codice])
);



