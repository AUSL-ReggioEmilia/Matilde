CREATE PROCEDURE [dbo].[MSP_SpostaCartella](
	@sNumeroCartella AS VARCHAR(50),
	@uIDCartella AS UNIQUEIDENTIFIER,
	@uIDEpisodioOrigine AS UNIQUEIDENTIFIER, 
	@uIDTrasferimentoDestinazione AS UNIQUEIDENTIFIER)
AS
BEGIN



												
	DECLARE @uIDEpisodioDestinazione AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteOrigine AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteDestinazione AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(50)
	DECLARE @sErrore AS VARCHAR(2000)
	DECLARE @nTmp AS INTEGER
	
	SET @sErrore=''
	
		IF ISNULL(@sNumeroCartella,'') =''
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'NumeroCartella passato non valorizzato'
	
		SET @uIDEpisodioDestinazione=(SELECT TOP 1 IDEpisodio FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimentoDestinazione)
	IF @uIDEpisodioDestinazione IS NULL
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'IDEpisodio di destinazione non trovato'
	
		SET @sTmp=(SELECT TOP 1 NumeroCartella	
				FROM 
					T_MovCartelle C 					
			    WHERE 
					ID=@uIDCartella
			    )
	
	IF @sTmp IS NULL 
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'NumeroCartella non trovato in base all''IDCartella passato'
		
	IF ISNULL(@sTmp,'') <> @sNumeroCartella
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'NumeroCartella passato non coerente con numero di cartella recuperato dall''IDCartella'
		
				
		CREATE TABLE #tmpIDTrasferimentiOrigine
	(
		ID UNIQUEIDENTIFIER 
	)
	
	INSERT INTO #tmpIDTrasferimentiOrigine(ID)
	SELECT
		ID
	FROM T_MovTrasferimenti 
		WHERE IDCartella=@uIDCartella AND
			  IDEpisodio=@uIDEpisodioOrigine
	
	SET @nTmp=(SELECT COUNT(*) FROM #tmpIDTrasferimentiOrigine)
	
	IF @nTmp=0
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'Nessun Trasferimento di origine trovato per la cartella e l''episodio passato'

	SET @uIDPazienteOrigine=(SELECT TOP 1 IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioOrigine)
	SET @uIDPazienteDestinazione=(SELECT TOP 1 IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioDestinazione)
	
		IF @uIDPazienteOrigine IS NULL 
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'IDPaziente di origine non trovato'
		
	IF @uIDPazienteDestinazione IS NULL 
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'IDPaziente di destinazione non trovato'		
		
	IF @uIDPazienteOrigine<> @uIDPazienteDestinazione
		SET @sErrore=@sErrore + CHAR(13) + CHAR(10) + 'IDPaziente di origine e IDPaziente di destinazione differenti'		
		
				
	IF @sErrore<> ''
		BEGIN
			SET @sErrore=@sErrore+ + CHAR(13) + CHAR(10) +' !!! ELABORAZIONE ANNULLATA : sono presenti errori, impossibile spostare la cartella'
			PRINT @sErrore
		END	
	ELSE
		BEGIN
												
			
			 			UPDATE T_MovAlertGenerici	
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
				
						UPDATE T_MovAllegati
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
					
						UPDATE T_MovAppuntamenti
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
					 
						UPDATE T_MovDiarioClinico
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
				
			
						UPDATE T_MovParametriVitali
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
						 
						UPDATE T_MovPrescrizioni
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
						 			 
			
						UPDATE T_MovSchede
			SET 
				IDEntita=@uIDEpisodioDestinazione,
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				CodEntita='EPI' AND
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
			
			UPDATE T_MovSchede
			SET 				
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				CodEntita IN ('ALA','ALG','DCL','PRF','PVT','WKI') AND
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
																	
						UPDATE T_MovTaskInfermieristici
			SET 
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)

						UPDATE T_MovOrdini
			SET
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)				
									
			UPDATE T_MovTrasferimenti
			SET IDCartella=NULL
			WHERE ID IN (SELECT ID FROM #tmpIDTrasferimentiOrigine)
			
			UPDATE T_MovTrasferimenti
			SET IDCartella=@uIDCartella
			WHERE ID=@uIDTrasferimentoDestinazione
			
		END	
			
		RETURN 0
		
END