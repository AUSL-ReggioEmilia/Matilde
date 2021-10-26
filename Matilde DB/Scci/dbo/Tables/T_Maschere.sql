CREATE TABLE [dbo].[T_Maschere] (
    [Codice]               VARCHAR (20)  NOT NULL,
    [Descrizione]          VARCHAR (255) NULL,
    [Modale]               BIT           NULL,
    [InCache]              BIT           NULL,
    [Aggiorna]             BIT           NULL,
    [Massimizzata]         BIT           NULL,
    [TimerRefresh]         INT           NULL,
    [SegnalibroAdd]        BIT           NULL,
    [SegnalibroVisualizza] BIT           NULL,
    [InCacheDaPercorso]    BIT           NULL,
    [CambioPercorso]       BIT           NULL,
    CONSTRAINT [PK_T_Maschere] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

