CREATE TABLE [dbo].[T_Aziende] (
    [Codice]             VARCHAR (20)  NOT NULL,
    [Descrizione]        VARCHAR (255) NULL,
    [RTFStampaEstesa]    VARCHAR (MAX) NULL,
    [RTFStampaSintetica] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_T_Aziende] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

