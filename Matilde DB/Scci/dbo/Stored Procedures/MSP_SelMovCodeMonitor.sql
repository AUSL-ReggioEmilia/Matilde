CREATE PROCEDURE [dbo].[MSP_SelMovCodeMonitor](@xParametri AS XML )
AS
BEGIN
	

				 
	DECLARE @nUltimoID AS INTEGER
	DECLARE @nTop AS INTEGER
	
		DECLARE @sSQL AS VARCHAR(MAX)	
	DECLARE @sDataTmp AS VARCHAR(50)	
	DECLARE @dtUltimaDataChiamata AS DATETIME
		
		SET @nTop=(SELECT TOP 1 ValoreParametro.nTop.value('.','INTEGER')
						 FROM @xParametri.nodes('/Parametri/Top') as ValoreParametro(nTop))
	
	SET @nTop=ISNULL(@nTop,0)
	
		SET @nUltimoID=(SELECT TOP 1 ValoreParametro.UltimoID.value('.','INTEGER')
						 FROM @xParametri.nodes('/Parametri/UltimoID') as ValoreParametro(UltimoID))
	
	SET @nUltimoID=ISNULL(@nUltimoID,0)
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.UltimaDataChiamata.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/UltimaDataChiamata') as ValoreParametro(UltimaDataChiamata))
	SET @sDataTmp=ISNULL(@sDataTmp,'01-01-1901 00:00:00.000')
	IF @sDataTmp='01-01-0001 00:00:00.000'  SET @sDataTmp='01-01-1901 00:00:00.000'
	
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,12)			
			IF ISDATE(@sDataTmp)=1
				SET	@dtUltimaDataChiamata=CONVERT(DATETIME,@sDataTmp,121)						
			ELSE
				SET	@dtUltimaDataChiamata =NULL		
		END 		
		
					CREATE TABLE #tmpAgende
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/CodAgenda')=1
		INSERT INTO #tmpAgende(Codice)	
			SELECT 	ValoreParametro.CodAgenda.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
																											
				CREATE TABLE #tmpTaskInfermieristici
	(
		Codice VARCHAR(20)  COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/CodTipoTaskInfermieristico')=1
		INSERT INTO #tmpTaskInfermieristici(Codice)	
			SELECT 	ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico)
		
		SET @sSQL='	SELECT '

	IF @nTop > 0
		SET @sSQL=@sSQL + ' TOP ' + CONVERT(VARCHAR(20),@nTop) 

			SET @sSQL=@sSQL + ' ID,Numero,Descrizione,CONVERT(BIT,Nuovo) AS Nuovo ,DataChiamata FROM '
		SET @sSQL=@sSQL + 
			 '(SELECT 
					MAX(E.IDNum) AS ID,				
					MAX(CONVERT(VARCHAR(20),C.NumeroCoda)) AS Numero,
					MAX(
						CASE 
							WHEN RTRIM(ISNULL(A.DescrizioneAlternativa,''''))='''' THEN  A.Descrizione
							ELSE A.DescrizioneAlternativa
						END
						) AS  Descrizione,
					MIN(CASE						
						WHEN E.DataChiamata > CONVERT(DATETIME,''' + CONVERT(VARCHAR(30),@dtUltimaDataChiamata,121) + ''',121) THEN CONVERT(INTEGER,1)
						ELSE CONVERT(INTEGER,0)
						END)	AS Nuovo,
					MAX(E.DataChiamata) AS DataChiamata
				FROM 
						T_MovCode C 
							INNER JOIN T_MovCodeEntita E
								ON (C.ID=E.IDCoda AND
									E.CodEntita=''APP'')
							INNER JOIN T_MovAppuntamenti APP
								ON E.IDEntita=APP.ID	
							INNER JOIN T_MovAppuntamentiAgende AGE
								ON AGE.IDAppuntamento=APP.ID
							INNER JOIN T_Agende A
								ON AGE.CodAgenda=A.Codice
				WHERE
					 					 E.DataChiamata > DATEADD(hour, -12,GETDATE()) AND
					 C.CodStatoCoda NOT IN (''CA'') 
					 AND APP.CodStatoAppuntamento NOT IN (''AN'',''CA'')
					 AND E.CodStatoCodaEntita=''CH''													
					 AND APP.ID IN (SELECT IDAppuntamento 
									FROM
										T_MovAppuntamentiAgende MA
											INNER JOIN #tmpAgende TMPA
												ON MA.CodAgenda=TMPA.Codice										
									)
				GROUP BY
					C.ID,
					E.CodEntita,
					A.Codice
				'			
		SET @sSQL=@sSQL + ' UNION ALL
				SELECT 
					MAX(E.IDNum) AS ID,				
					MAX(CONVERT(VARCHAR(20),C.NumeroCoda)) AS Numero,
					MAX(TT.Descrizione) AS Descrizione,
					MAX(CASE						
						WHEN E.DataChiamata > CONVERT(DATETIME,''' + CONVERT(VARCHAR(30),@dtUltimaDataChiamata,121) + ''',121) THEN CONVERT(INTEGER,1)						
						ELSE CONVERT(INTEGER,0)
					END)	AS Nuovo,
					MAX(E.DataChiamata) AS DataChiamata
				FROM 
					T_MovCode C 
						INNER JOIN T_MovCodeEntita E
							ON (C.ID=E.IDCoda AND
								E.CodEntita=''WKI'')
						INNER JOIN T_MovTaskInfermieristici WKI
							ON E.IDEntita=WKI.ID							
						INNER JOIN T_TipoTaskInfermieristico TT
							ON WKI.CodTipoTaskInfermieristico=TT.Codice
						INNER JOIN 
							#tmpTaskInfermieristici TMPWKI
								ON WKI.CodTipoTaskInfermieristico=TMPWKI.Codice	
				WHERE 
										E.DataChiamata > DATEADD(hour, -12,GETDATE()) AND
					C.CodStatoCoda NOT IN (''CA'') 
					AND WKI.CodStatoTaskInfermieristico NOT IN (''AN'',''CA'')
					AND E.CodStatoCodaEntita=''CH''
					
				GROUP BY
					C.ID,
					E.CodEntita,
					WKI.CodTipoTaskInfermieristico
				
				'
	SET @sSQL=@sSQL + ') AS Q ORDER BY DataChiamata DESC'			
				
	PRINT @sSQL			
	EXEC (@sSQL)	

	DROP TABLE #tmpAgende	
	DROP TABLE #tmpTaskInfermieristici
	
	RETURN 0

END