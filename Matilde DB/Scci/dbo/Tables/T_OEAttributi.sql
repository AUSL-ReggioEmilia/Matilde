CREATE TABLE [dbo].[T_OEAttributi] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [CodEntita]             VARCHAR (20)   NOT NULL,
    [CodSistemaRichiedente] VARCHAR (20)   NOT NULL,
    [CodAgendaRichiedente]  VARCHAR (50)   NOT NULL,
    [MappaturaOE]           XML            NULL,
    [Note]                  VARCHAR (2000) NULL,
    CONSTRAINT [PK_T_OEAttributi] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CodEntitaCodSistemaRichiedenteCodAgendaRichiedente]
    ON [dbo].[T_OEAttributi]([CodEntita] ASC, [CodSistemaRichiedente] ASC, [CodAgendaRichiedente] ASC);

