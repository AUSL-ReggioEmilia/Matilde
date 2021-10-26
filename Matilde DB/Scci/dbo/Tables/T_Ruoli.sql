CREATE TABLE [dbo].[T_Ruoli] (
    [Codice]                 VARCHAR (20)   NOT NULL,
    [Descrizione]            VARCHAR (255)  NULL,
    [Note]                   VARCHAR (4000) NULL,
    [CodTipoDiario]          VARCHAR (20)   NULL,
    [NumMaxCercaEpi]         INT            NULL,
    [RichiediPassword]       BIT            NULL,
    [LimitaEVCAmbulatoriale] BIT            NULL,
    CONSTRAINT [PK_T_Ruoli] PRIMARY KEY CLUSTERED ([Codice] ASC)
);



