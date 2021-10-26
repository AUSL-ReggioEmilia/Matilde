CREATE PROCEDURE [dbo].[MSP_SelMovCartelleDaUtente](@xParametri XML)
AS
BEGIN

	
	
					
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
		
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @xTmp AS XML						  				 	
	DECLARE @sCodRuolo AS VARCHAR(100)	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

				
		SET @uIDPaziente=(SELECT TOP 1 P.IDPaziente 
	  				   FROM T_MovTrasferimenti T 
								INNER JOIN T_MovPazienti P
									ON T.IDEpisodio=P.IDEpisodio
						WHERE T.IDCartella=@uIDCartella 
							  AND CodStatoTrasferimento IN ('AT','DM','PR','PC','TR'))
	
	CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
		

	
		DECLARE cursore CURSOR LOCAL READ_ONLY  FOR 	
	SELECT  
		CodRuolo
	FROM 
		T_AssLoginRuoli
	WHERE 
		CodLogin=@sCodUtente																																
	
	OPEN cursore
		
	FETCH NEXT FROM cursore 
	INTO @sCodRuolo
				
	WHILE (@@FETCH_STATUS = 0)
	BEGIN		 
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')		
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
		
		FETCH NEXT FROM cursore 
		INTO @sCodRuolo
	END
	
		CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
		
		
	
	SELECT 
		QC.IDEpisodio,
		QC.IDTrasferimento,
		MC.ID AS IDCartella,
		MC.NumeroCartella, 
		UA.Descrizione AS Struttura,				
		ISNULL(ME.CodTipoEpisodio,'') +'-'+ ISNULL(ME.NumeroNosologico,ISNULL(ME.NumeroListaAttesa,'')) AS  NumeroEpisodio,
		CASE 
			WHEN MaxDataIngresso IS NOT NULL  THEN 
					CONVERT(varchar(10),MaxDataIngresso,105) + ' ' + CONVERT(varchar(5),MaxDataIngresso,14)
					
			ELSE ''
		END AS DataTrasferimento		
	FROM 
		T_MovCartelle MC
	INNER JOIN
			(SELECT 
				IDCartella,
				M.CodUA, 
				M.IDEpisodio,
				M.ID AS IDTrasferimento,
				MAX(DataIngresso) AS MaxDataIngresso,
				MAX(DataUscita) AS MaxDataUscita
			FROM 
				T_MovTrasferimenti M 
					INNER JOIN T_MovCartelle C
						ON M.IDCartella=C.ID				
			WHERE 				
				M.CodStatoTrasferimento IN ('AT','DM','PR') AND 
				M.CodUA IN
					(SELECT CodUA FROM #tmpUARuolo
					 GROUP BY CodUA) 
				AND
					C.CodStatoCartella='AP'					
				
				AND
				   M.IDEpisodio IN (
					SELECT IDEpisodio FROM
						T_MovPazienti
					WHERE 
						IDPaziente=@uIDPaziente
						OR
						 						IDPaziente IN 
							(SELECT IDPazienteVecchio
							 FROM T_PazientiAlias
							 WHERE 
								IDPaziente IN 
									(SELECT IDPaziente
									 FROM T_PazientiAlias
									 WHERE IDPazienteVecchio=@uIDPaziente
									)
								)			
					)
			GROUP BY IDCartella,M.CodUA,IDEpisodio,M.ID) AS QC
		ON MC.ID=QC.IDCartella		
	
	LEFT JOIN T_UnitaAtomiche UA
		ON QC.CodUA=UA.Codice
	LEFT JOIN T_MovEpisodi ME
		ON QC.IDEpisodio=ME.ID
	WHERE 
		MC.ID<>@uIDCartella 
		
	ORDER BY MC.IDNum
		
		
	
	RETURN 0
END