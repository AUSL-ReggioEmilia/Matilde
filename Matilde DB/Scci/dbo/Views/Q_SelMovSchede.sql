CREATE VIEW [dbo].[Q_SelMovSchede]
AS

	SELECT ID
      ,IDNum
      ,CodUA
      ,CodEntita
      ,IDEntita
      ,IDPaziente
      ,IDEpisodio
      ,IDTrasferimento
      ,CodScheda
      ,Versione
      ,Numero
            ,AnteprimaRTF
      ,AnteprimaTXT
      ,DatiObbligatoriMancantiRTF
      ,DatiRilievoRTF
      ,IDSchedaPadre
      ,Storicizzata
      ,CodStatoScheda
      ,DataCreazione
      ,DataCreazioneUTC
      ,CodUtenteRilevazione
      ,CodUtenteUltimaModifica
      ,DataUltimaModifica
      ,DataUltimaModificaUTC
  FROM T_MovSchede