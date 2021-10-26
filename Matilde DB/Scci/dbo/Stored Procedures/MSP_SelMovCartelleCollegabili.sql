
CREATE PROCEDURE [dbo].[MSP_SelMovCartelleCollegabili](@xParametri XML)
AS
BEGIN

	
	
					
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS  UNIQUEIDENTIFIER
	
		DECLARE @bAbilitaCollegaCartelle AS BIT	
	DECLARE @bAbilitaCollegaCartellePA AS BIT	
	DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @xTmp AS XML						  				 	
	DECLARE @sCodRuolo AS VARCHAR(100)	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodUARif AS VARCHAR(20)				
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
		SET @uIDCartella=NEWID()		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
						  				  
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')


			
			
		SET @sCodUA=(SELECT CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)	
	
		SET @bAbilitaCollegaCartelle=(SELECT TOP 1 ISNULL(AbilitaCollegaCartelle,0) FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
	
		SET @sCodUARif=(SELECT 
						CASE 
							WHEN 
								ISNULL(CodUANumerazioneCartella,'') ='' THEN @sCodUA
								ELSE CodUANumerazioneCartella
						END 	
							FROM T_UnitaAtomiche
							WHERE Codice=@sCodUA
					)
	 	CREATE TABLE #tmpUACollegabili
			(CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS)

		IF (@bAbilitaCollegaCartelle=1)
	BEGIN	
		INSERT INTO #tmpUACollegabili(CodUA)
			SELECT @sCodUA							UNION	
			SELECT @sCodUARif						UNION
			SELECT CodUACollegata					FROM T_AssUAUACollegate
			WHERE CodUA=@sCodUA		
	END

			
		SET @bAbilitaCollegaCartellePA=(SELECT TOP 1 ISNULL(AbilitaCollegaCartellePA,0) FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
	
		CREATE TABLE #tmpUACollegabiliPA
		(CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS)
	
	IF (@bAbilitaCollegaCartellePA=1)
	BEGIN
				INSERT INTO #tmpUACollegabiliPA (CodUA)				
			SELECT CodUACollegata					FROM T_AssUAUACollegatePA
			WHERE CodUA=@sCodUA
	END
		
				SELECT * FROM 
	(
				SELECT 
			QC.IDEpisodio,
			QC.IDTrasferimento,
			MC.ID AS IDCartella,
			MC.NumeroCartella, 
			UA.Descrizione AS Struttura,				
			ISNULL(ME.CodTipoEpisodio,'') +'-'+ ISNULL(ME.NumeroNosologico,ISNULL(ME.NumeroListaAttesa,'')) AS  NumeroEpisodio,
			SE.Descrizione AS DescrStatoEpisodio,
			CASE 
				WHEN MaxDataIngresso IS NOT NULL  THEN 
						CONVERT(varchar(10),MaxDataIngresso,105) + ' ' + CONVERT(varchar(5),MaxDataIngresso,14)
					
				ELSE ''
			END AS DataTrasferimento,			
			MC.IDNum	
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
					LEFT JOIN T_UnitaAtomiche A
						ON M.CodUA=A.Codice										
				WHERE 				
					M.CodStatoTrasferimento IN ('AT','DM','PR') AND 
										(						M.CodUA IN (SELECT CodUA FROM #tmpUACollegabili ) OR
						ISNULL(A.CodUANumerazioneCartella,'<NON DEFINITO>')=@sCodUARif
						)
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
		LEFT JOIN T_StatoEpisodio SE
				ON ME.CodStatoEpisodio=SE.Codice
		WHERE 
			MC.ID<>@uIDCartella AND
			1=@bAbilitaCollegaCartelle			
		UNION 		
		SELECT 
			QC.IDEpisodio,
			QC.IDTrasferimento,
			MC.ID AS IDCartella,
			MC.NumeroCartella, 
			UA.Descrizione AS Struttura,				
			ISNULL(ME.CodTipoEpisodio,'') +'-'+ ISNULL(ME.NumeroNosologico,ISNULL(ME.NumeroListaAttesa,'')) AS  NumeroEpisodio,
			SE.Descrizione AS DescrStatoEpisodio,
			CASE 
				WHEN MaxDataIngresso IS NOT NULL  THEN 
						CONVERT(varchar(10),MaxDataIngresso,105) + ' ' + CONVERT(varchar(5),MaxDataIngresso,14)
					
				ELSE ''
			END AS DataTrasferimento,				
			MC.IDNum
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
					LEFT JOIN T_UnitaAtomiche A
						ON M.CodUA=A.Codice										
				WHERE 				
					M.CodStatoTrasferimento IN ('PA') AND														(						M.CodUA IN (SELECT CodUA FROM #tmpUACollegabiliPA )
						)
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
		LEFT JOIN T_StatoEpisodio SE
			ON ME.CodStatoEpisodio=SE.Codice
		WHERE 
			MC.ID<>@uIDCartella AND
			1=@bAbilitaCollegaCartellePA				) AS LCC
	ORDER BY LCC.IDNum ASC
		
	RETURN 0
END