CREATE TABLE [dbo].[T_SchedePadri] (
    [CodScheda]      VARCHAR (20) NOT NULL,
    [CodSchedaPadre] VARCHAR (20) NOT NULL,
    [AbilitaEPI]     BIT          NULL,
    [AbilitaPAZ]     BIT          NULL,
    CONSTRAINT [PK_T_AssSchedePadre] PRIMARY KEY CLUSTERED ([CodScheda] ASC, [CodSchedaPadre] ASC),
    CONSTRAINT [FK_T_SchedePadri_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice]),
    CONSTRAINT [FK_T_SchedePadri_T_Schede1] FOREIGN KEY ([CodSchedaPadre]) REFERENCES [dbo].[T_Schede] ([Codice])
);

