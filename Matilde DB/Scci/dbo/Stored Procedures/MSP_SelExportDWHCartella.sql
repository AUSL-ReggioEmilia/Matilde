CREATE PROCEDURE [dbo].[MSP_SelExportDWHCartella](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	
		
		DECLARE @sNumeroCartella VARCHAR(50)	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(MAX)
	DECLARE @bTimeStamp AS BIT
    DECLARE @xTimeStamp AS XML	
    
    
					
		IF @xParametri.exist('/Parametri/IDCartella')=1
		BEGIN			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
			IF 	ISNULL(@sGUID,'') <> '' 				
					SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
	

					IF @xParametri.exist('/Parametri/TimeStamp')=1
		BEGIN
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
					  
			SET @bTimeStamp=1
		END
		ELSE
			SET @bTimeStamp=0

			
	IF @uIDCartella IS NOT NULL		
		BEGIN			
			SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDCartella)		
			SET @sNumeroCartella=ISNULL(@sNumeroCartella,'')
		END
		
		
	IF @bTimeStamp=1
	BEGIN
				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
		SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCartella")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
		SET @xTimeStamp.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
		
				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
		SET @xTimeStamp.modify('insert <CodAzione>VIS</CodAzione> into (/TimeStamp)[1]')
		
				SET @xTimeStamp.modify('delete (/TimeStamp/Note)[1]') 			
		SET @xTimeStamp.modify('insert <Note>{sql:variable("@sNumeroCartella")}</Note> into (/TimeStamp)[1]')
		
		
		SET @xTimeStamp=CONVERT(XML,
							'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
							'</Parametri>')
	END
	

	SELECT 
						ISNULL(DataChiusura,GETDATE()) AS DataSequenza,
			M.ID AS IDCartella,
			M.NumeroCartella,
			Q.CodAzi AS AziendaErogante,
			'SCCI' AS SistemaErogante,
			Q.CodUA AS RepartoEroganteCodice,
			Q.DescrUA AS RepartoEroganteDescrizione,
			Q.CodSAC AS PazienteIDEsterno,
			Q.Cognome AS PazienteCognome,
			Q.Nome AS PazienteNome,
			Q.DataNascita As PazienteDataNascita,
			Q.Sesso AS PazienteSesso,
			Q.CodiceFiscale As PazienteCodiceFiscale,
			ISNULL(DataChiusura,DataApertura) AS DataReferto,
			M.ID AS IDReferto,
			Q.NumeroNosologico AS NumeroNosologico,			
			M.PDFCartella,
			CodUA + '#' +  NumeroCartella AS BarcodeCartella

		FROM 
			T_MovCartelle M
				INNER JOIN 
					(SELECT TOP 1 
						T.IDCartella,
						E.CodAzi,
						T.CodUA,
						A.Descrizione AS DescrUA,
						P.CodSAC,
						P.Cognome,
						P.Nome,
						P.DataNascita,
						P.Sesso,
						P.CodiceFiscale,
						ISNULL(E.NumeroNosologico,E.NumeroListaAttesa) AS NumeroNosologico
					 FROM 
						T_MovTrasferimenti T
							INNER JOIN T_MovEpisodi E	
								ON T.IDEpisodio=E.ID	
							INNER JOIN T_UnitaAtomiche A
								ON T.CodUA=A.Codice	
							INNER JOIN T_MovPazienti P
								ON P.IDEpisodio=E.ID						
					 WHERE 
						CodStatoTrasferimento NOT IN ('CA') AND
						T.IDCartella=@uIDCartella
					 ) AS Q
				ON 
					M.ID=Q.IDCartella	 
		WHERE 
			M.ID=@uIDCartella
	
						
	
				
END