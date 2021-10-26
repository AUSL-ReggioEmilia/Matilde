CREATE TABLE [dbo].[T_TipoEvidenzaClinica] (
    [Codice]             VARCHAR (20)    NOT NULL,
    [Descrizione]        VARCHAR (255)   NULL,
    [Icona]              VARBINARY (MAX) NULL,
    [Riservato]          BIT             NULL,
    [RiaperturaCartella] BIT             NULL,
    [AllegaInCartella]   BIT             NULL,
    CONSTRAINT [PK_T_TipoEvidenzaClinica] PRIMARY KEY CLUSTERED ([Codice] ASC)
);



