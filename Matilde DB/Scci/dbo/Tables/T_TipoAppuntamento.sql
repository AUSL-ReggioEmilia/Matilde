CREATE TABLE [dbo].[T_TipoAppuntamento] (
    [Codice]           VARCHAR (20)    NOT NULL,
    [Descrizione]      VARCHAR (255)   NULL,
    [Colore]           VARCHAR (50)    NULL,
    [Icona]            VARBINARY (MAX) NULL,
    [CodScheda]        VARCHAR (20)    NULL,
    [TimeSlotInterval] INT             NULL,
    [FormulaTitolo]    VARCHAR (2000)  NULL,
    [Multiplo]         BIT             NULL,
    [SenzaData]        BIT             NULL,
    [Settimanale]      BIT             NULL,
    [SenzaDataSempre]  BIT             NULL,
    [Ripianificazione] INT             NULL,
    CONSTRAINT [PK_T_TipoAppuntamenti] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_TipoAppuntamento_T_Schede] FOREIGN KEY ([CodScheda]) REFERENCES [dbo].[T_Schede] ([Codice])
);



