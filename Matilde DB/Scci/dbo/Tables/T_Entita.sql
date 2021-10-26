CREATE TABLE [dbo].[T_Entita] (
    [Codice]                   VARCHAR (20)   NOT NULL,
    [Descrizione]              VARCHAR (255)  NULL,
    [SistemaEsterno]           BIT            NULL,
    [AbilitaPermessiDettaglio] BIT            NULL,
    [UsaSchede]                BIT            NULL,
    [UsaSchedaSemplificata]    BIT            NULL,
    [UsaTestiRTF]              BIT            NULL,
    [UsaAgende]                BIT            CONSTRAINT [DF_T_Entita_UsaAgende] DEFAULT ((0)) NULL,
    [AssociaUA]                BIT            NULL,
    [SorgenteReport]           BIT            NULL,
    [Note]                     VARCHAR (2000) NULL,
    [Tabella]                  VARCHAR (50)   NULL,
    CONSTRAINT [PK_T_Entita] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

