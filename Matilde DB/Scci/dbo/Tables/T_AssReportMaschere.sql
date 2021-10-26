CREATE TABLE [dbo].[T_AssReportMaschere] (
    [CodReport]   VARCHAR (20) NOT NULL,
    [CodMaschera] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AssReportMaschere] PRIMARY KEY CLUSTERED ([CodReport] ASC, [CodMaschera] ASC),
    CONSTRAINT [FK_T_AssReportMaschere_T_Maschere] FOREIGN KEY ([CodMaschera]) REFERENCES [dbo].[T_Maschere] ([Codice]),
    CONSTRAINT [FK_T_AssReportMaschere_T_Report] FOREIGN KEY ([CodReport]) REFERENCES [dbo].[T_Report] ([Codice])
);

