CREATE TABLE [dbo].[T_StatoSchedaCalcolato] (
    [CodScheda]   VARCHAR (20)    NOT NULL,
    [Codice]      VARCHAR (20)    NOT NULL,
    [Descrizione] VARCHAR (255)   NULL,
    [Icona]       VARBINARY (MAX) NULL,
    [Colore]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_T_StatoSchedaCalcolato_1] PRIMARY KEY CLUSTERED ([CodScheda] ASC, [Codice] ASC)
);

