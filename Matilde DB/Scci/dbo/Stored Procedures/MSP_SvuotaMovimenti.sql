CREATE PROCEDURE [dbo].[MSP_SvuotaMovimenti] (@sCodice AS Varchar(50))
AS
BEGIN

IF @sCodice='123456'
BEGIN
	DELETE FROM T_MovAlertAllergieAnamnesi
	DELETE FROM T_MovAlertGenerici
	DELETE FROM T_MovAllegati
	DELETE FROM T_MovAppuntamenti
	DELETE FROM T_MovAppuntamentiAgende
	DELETE FROM T_MovDiarioClinico
	DELETE FROM T_MovEvidenzaClinica
	DELETE FROM T_MovLock
	DELETE FROM T_MovNews
	DELETE FROM T_MovNoteAgende
	DELETE FROM T_MovOrdini
	DELETE FROM T_MovParametriVitali
	DELETE FROM T_MovPrescrizioniTempi	
	DELETE FROM T_MovPrescrizioni
	DELETE FROM T_MovTaskInfermieristici
	DELETE FROM T_MovTimeStamp
	DELETE FROM T_MovTrasferimenti
	DELETE FROM T_MovCartelle
	DELETE FROM T_MovReport
	DELETE FROM T_MovPazienti
	DELETE FROM T_MovEpisodi
	DELETE FROM T_MovSchede
	DELETE FROM T_MovPazientiAlias
	DELETE FROM T_Pazienti
END

END