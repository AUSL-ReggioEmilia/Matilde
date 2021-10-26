CREATE TABLE [dbo].[T_MH_CodaSmsOutput] (
    [IdSequenza]                 INT           IDENTITY (1, 1) NOT NULL,
    [DataInserimento]            DATETIME      CONSTRAINT [DF_TableName_DataInserimento] DEFAULT (getutcdate()) NOT NULL,
    [Messaggio]                  VARCHAR (512) NOT NULL,
    [NumeroTelefonoDestinatario] VARCHAR (64)  NOT NULL,
    [Tipologia]                  VARCHAR (64)  NOT NULL,
    CONSTRAINT [PK_T_MH_CodaSmsOutput] PRIMARY KEY CLUSTERED ([IdSequenza] ASC)
);

