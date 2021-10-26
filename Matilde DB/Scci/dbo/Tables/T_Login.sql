CREATE TABLE [dbo].[T_Login] (
    [Codice]            VARCHAR (100)   NOT NULL,
    [Descrizione]       VARCHAR (100)   NULL,
    [Cognome]           VARCHAR (100)   NULL,
    [Nome]              VARCHAR (100)   NULL,
    [Note]              VARCHAR (255)   NULL,
    [FlagAdmin]         BIT             NULL,
    [FlagObsoleto]      BIT             NULL,
    [Foto]              VARBINARY (MAX) NULL,
    [FlagSistema]       BIT             NULL,
    [CodiceFiscale]     VARCHAR (16)    NULL,
    [UserPrincipalName] VARCHAR (255)   NULL,
    CONSTRAINT [PK_T_Login] PRIMARY KEY CLUSTERED ([Codice] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Login_UserPrincipalName]
    ON [dbo].[T_Login]([UserPrincipalName] ASC) WHERE ([UserPrincipalName] IS NOT NULL);

