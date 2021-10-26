CREATE PROCEDURE MSP_SvuotaCartella(@uIDCartella AS UNIQUEIDENTIFIER)
AS
BEGIN
	
	DELETE FROM T_MovAlertGenerici
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovAllegati
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovAppuntamentiAgende
	WHERE IDAppuntamento 
		IN (SELECT ID 
			FROM T_MovAppuntamenti
			WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)
			)

	DELETE FROM T_MovAppuntamenti
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovCartelleInVisione
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovDiarioClinico
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovNote
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovParametriVitali
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovPrescrizioniTempi
	WHERE IDPrescrizione IN 
						(SELECT ID 
						 FROM T_MovPrescrizioni WHERE
						 IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)
						 )	
						 
	DELETE FROM T_MovPrescrizioni 
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovReport
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovSegnalibri
	WHERE IDCartella=@uIDCartella

	DELETE FROM T_MovTaskInfermieristici
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	DELETE FROM T_MovSchede
	WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartella)

	UPDATE T_MovTrasferimenti
	SET IDCartella=NULL
	WHERE IDCartella=@uIDCartella
	
	UPDATE T_MovCartelle
	SET CodStatoCartella='CH',
		NumeroCartella=NumeroCartella +'_ERR'
	WHERE ID=@uIDCartella
		

END