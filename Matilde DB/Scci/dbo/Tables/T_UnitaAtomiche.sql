CREATE TABLE [dbo].[T_UnitaAtomiche] (
    [Codice]                      VARCHAR (20)   NOT NULL,
    [Descrizione]                 VARCHAR (255)  NULL,
    [Note]                        VARCHAR (4000) NULL,
    [CodPadre]                    VARCHAR (20)   NULL,
    [UltimoNumeroCartella]        VARCHAR (50)   NULL,
    [IntestazioneCartella]        VARCHAR (MAX)  NULL,
    [FirmaCartella]               VARCHAR (MAX)  NULL,
    [IntestazioneSintetica]       VARCHAR (MAX)  NULL,
    [CodUANumerazioneCartella]    VARCHAR (20)   NULL,
    [AbilitaCollegaCartelle]      BIT            NULL,
    [AccessoAmbulatoriale]        BIT            NULL,
    [OraApertura]                 VARCHAR (5)    NULL,
    [OraChiusura]                 VARCHAR (5)    NULL,
    [CodAzienda]                  VARCHAR (20)   NULL,
    [SpallaSinistra]              VARCHAR (MAX)  NULL,
    [AbilitaCollegaCartellePA]    BIT            NULL,
    [VisualizzaIconeAppuntamenti] BIT            NULL,
    CONSTRAINT [PK_T_UnitaAtomiche] PRIMARY KEY CLUSTERED ([Codice] ASC),
    CONSTRAINT [FK_T_T_UnitaAtomiche_T_Aziende_Codice] FOREIGN KEY ([CodAzienda]) REFERENCES [dbo].[T_Aziende] ([Codice])
);




GO
CREATE NONCLUSTERED INDEX [IX_Padre]
    ON [dbo].[T_UnitaAtomiche]([CodPadre] ASC);

