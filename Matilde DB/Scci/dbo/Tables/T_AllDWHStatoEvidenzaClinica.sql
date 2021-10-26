CREATE TABLE [dbo].[T_AllDWHStatoEvidenzaClinica] (
    [CodStatoEvidenzaClinica] VARCHAR (20) NOT NULL,
    [CodDWH]                  VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_AllDWHStatoEvidenzaClinica] PRIMARY KEY CLUSTERED ([CodStatoEvidenzaClinica] ASC, [CodDWH] ASC)
);

