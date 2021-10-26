CREATE TABLE [dbo].[T_BO_CartelleCollega] (
    [IDNum]                       INT              IDENTITY (1, 1) NOT NULL,
    [IDTrasferimentoOrigine]      UNIQUEIDENTIFIER NULL,
    [IDCartellaOrigine]           UNIQUEIDENTIFIER NULL,
    [IDTrasferimentoDestinazione] UNIQUEIDENTIFIER NULL,
    [NumeroCartellaDestinazione]  VARCHAR (50)     NULL,
    [AnnoCartellaDestinazione]    VARCHAR (4)      NULL,
    [CodStatoElaborazione]        VARCHAR (20)     NULL,
    [DataElaborazione]            DATETIME         NULL,
    [IDCartellaDestinazione]      UNIQUEIDENTIFIER NULL,
    [Note]                        VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_T_BO_CartelleCollega] PRIMARY KEY CLUSTERED ([IDNum] ASC)
);

