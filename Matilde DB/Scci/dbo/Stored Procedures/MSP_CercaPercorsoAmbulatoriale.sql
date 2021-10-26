CREATE PROCEDURE [dbo].[MSP_CercaPercorsoAmbulatoriale](@xParametri AS XML)
AS
BEGIN
			

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @bDatiEstesi AS Bit	
	DECLARE @sDataNascita AS VARCHAR(10)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodStatoCartella AS Varchar(MAX)
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)	
	DECLARE @nNumRighe AS INTEGER
	DECLARE @sOrdinamento AS Varchar(500)
	DECLARE @sCodLogin AS VARCHAR(100)
	
		DECLARE @xTMP AS XML
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	

	SET @bDatiEstesi=1


		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')		


		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))

			SET @sCodStatoCartella=''
	SELECT	@sCodStatoCartella =  @sCodStatoCartella +
														CASE 
								WHEN @sCodStatoCartella='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoCartella.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella)	

		
												
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))	

		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))
	SET @nNumRighe=ISNULL(@nNumRighe,0)


		SET @sOrdinamento=(SELECT	TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))						 
	SET @sOrdinamento= ISNULL(@sOrdinamento,'')
	SET @sOrdinamento=LTRIM(RTRIM(@sOrdinamento))
				
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))



				
	CREATE TABLE #tmpFiltriSchede
		(
			IDScheda uniqueidentifier NOT NULL,
			CodScheda VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
		
					 	
	CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
			INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp

	CREATE TABLE #tmpPazientiInVisione
		(
			IDPaziente uniqueidentifier,
			
		)	

	
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sSQL AS VARCHAR(Max)
	
	SET @sWhere=''
	
		IF ISNULL(@sDataNascita,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND P.DataNascita = convert(datetime,''' + @sDataNascita + ''',105)'
	END

			IF ISNULL(@sFiltroGenerico,'') <> ''
	BEGIN
				SET @sWhere= @sWhere + ' AND ('
		SET @sWhere= @sWhere + '  P.Cognome like ''%' + @sFiltroGenerico + '%'''
		SET @sWhere= @sWhere + '  OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	
	
		SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''

		SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''
		
					SET @sWhere= @sWhere + '     )' 					
	END

		IF ISNULL(@sCodStatoCartella,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND CR.CodStatoCartella IN ('+ @sCodStatoCartella + ')'  
	END
	
		SET @sWhere= @sWhere + ' AND M.Storicizzata=0 AND CodStatoScheda <> ''CA'' AND M.IDCartellaAmbulatoriale IS NOT NULL'

																			
		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sSQLFiltro=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sSQLFiltro,'') <> '' 
		BEGIN						
			SET @sWhere= @sWhere + ' AND ' + 	@sSQLFiltro + ''
		END
	END	
	
		INSERT INTO #tmpPazientiInVisione(IDPaziente)
		SELECT ISNULL(PA.IDPaziente,M.IDPaziente) AS IDPaziente
		FROM 
		     T_MovPazientiInVisione M
				LEFT JOIN T_PazientiAlias PA	
						ON M.IDPaziente=PA.IDPazienteVecchio 
		 WHERE
			 M.CodRuoloInVisione	=@sCodRuolo AND
			 M.CodStatoPazienteInVisione IN ('IC') AND
			 DataInizio <=GetDate() AND DataFine>=GetDate() 								  

								

		SET @sSQL='
			INSERT #tmpFiltriSchede(IDScheda,CodScheda) 
			SELECT '
				+ CASE 
					WHEN(ISNULL(@nNumRighe,0)=0) THEN ''
					ELSE ' TOP ' + CONVERT(VARCHAR(20),@nNumRighe) 
				END 						
				+ ' M.ID AS IDScheda	
				,M.CodScheda		  
			FROM T_MovSchede M				

				INNER JOIN T_Schede S
					ON M.CodScheda = S.Codice					

				INNER JOIN T_Pazienti P
					ON M.IDPaziente=P.ID
										 
				INNER JOIN T_MovCartelleAmbulatoriali CR
					ON M.IDCartellaAmbulatoriale =CR.ID	
					
				INNER JOIN #tmpUARuolo Tmp		
					ON M.CodUA=Tmp.CodUA								
		'		

		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	

		IF @sOrdinamento<> ''
		SET @sSQL = @sSQL + ' ORDER BY ' + @sOrdinamento + ''

	PRINT @sSQL
	EXEC (@sSQL)
	
	
				
	SELECT 
		M.IDPaziente,
		ISNULL(CNC.IDPazienteFuso,M.IDPaziente) AS IDPazienteFuso,
		P.CodSAC,
		ISNULL(P.CodSACFuso,P.CodSac) AS CodSACFuso,
		M.ID AS IDScheda,
		M.IDCartellaAmbulatoriale AS IDCartella,
		ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' ' + 
				CASE 
					WHEN ISNULL(P.Sesso,'')='' THEN ''
					ELSE ' (' + P.Sesso +') '
				END +
				CASE 
					WHEN P.DataNascita IS NULL THEN ''
					ELSE Convert(varchar(10),P.DataNascita,105)
				END +
			
				CASE 
					WHEN ISNULL(P.ComuneNascita,'')='' THEN ''
					ELSE ', ' + P.ComuneNascita
				END +
									
				CASE 
					WHEN ISNULL(P.CodProvinciaNascita,'')='' THEN ''
					ELSE ' (' + P.CodProvinciaNascita + ')'
				END 
		AS Paziente,

		ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') AS Paziente2,

			CASE 
				WHEN ISNULL(P.Sesso,'')='' THEN ''
				ELSE ' (' + P.Sesso +') '
			END +
			+ ' '  +  
			CASE 
				WHEN P.DataNascita IS NULL THEN ''
				ELSE Convert(varchar(10),P.DataNascita,105)
			END +
			+
			CASE 
				WHEN P.DataNascita IS NOT NULL THEN ' (' + dbo.MF_CalcolaEtaPediatrica(P.DataNascita) + ')'
				ELSE ''
			END
			+
			CASE 
				WHEN ISNULL(P.ComuneNascita,'')='' THEN ''
				ELSE ', ' + P.ComuneNascita
			END +
								
			CASE 
				WHEN ISNULL(P.CodProvinciaNascita,'')='' THEN ''
				ELSE ' (' + P.CodProvinciaNascita + ')'
			END 
		AS Paziente3,

		
		S.Descrizione AS DescrScheda,
		C.NumeroCartella,
		C.CodStatoCartella,
		C.DataApertura,
		C.DataChiusura,
		ISNULL(GAA.Qta,0) AS NumAllergie,
		CASE
				WHEN CNC.IDPazienteFuso IS NULL THEN ISNULL(CNC.CodStatoConsensoCalcolato,'ND')
				ELSE  ISNULL(CNC.CodStatoConsensoCalcolatoFuso,'ND') 
		END AS CodStatoConsensoCalcolato,		
		M.CodUA As CodUA,
		A.Descrizione As DescrUA,
										CONVERT(INTEGER,1) AS FlagCartellaInVisione,
		CONVERT(INTEGER,1) AS FlagHoDatoCartellaInVisione
						
	FROM 
		T_MovSchede M
			INNER JOIN #tmpFiltriSchede F
				ON M.ID=F.IDScheda
			INNER JOIN T_Schede S
				ON M.CodScheda=S.Codice
			LEFT JOIN T_MovCartelleAmbulatoriali C
				ON M.IDCartellaAmbulatoriale=C.ID
			LEFT JOIN T_Pazienti P
				ON M.IDPaziente =  P.ID

			LEFT JOIN T_UnitaAtomiche A
				ON M.CodUA = A.Codice

						LEFT JOIN 
					(SELECT IDPaziente,
							COUNT(ID) AS Qta 
					  FROM 
							T_MovAlertAllergieAnamnesi 
					  WHERE CodStatoAlertAllergiaAnamnesi NOT IN ('AN','CA')
					  GROUP BY 	IDPaziente				  
					 ) AS GAA
				ON P.ID=GAA.IDPaziente

						LEFT JOIN 
					(SELECT 
						CncPaz.ID AS IDPaziente,
						CncPaz.CodStatoConsensoCalcolato AS CodStatoConsensoCalcolato,
						CncPazAlias.ID AS IDPazienteFuso,
						CncPazAlias.CodStatoConsensoCalcolato AS CodStatoConsensoCalcolatoFuso,
						CncPaz.CodSAC AS CodSAC,
						CncPaz.CodSACFuso As CodSACFuso						
				   FROM T_Pazienti CncPaz															
						LEFT JOIN T_PazientiAlias CncAlias
							ON (CncPaz.ID=CncAlias.IDPazienteVecchio)
						LEFT JOIN T_Pazienti AS CncPazAlias
							ON (CncPazAlias.ID=CncAlias.IDPaziente)
					) AS CNC
				ON CNC.IDPaziente= M.IDPaziente

						
		IF @bDatiEstesi=1
	BEGIN		
		SELECT Codice,Descrizione
		FROM T_Schede
		WHERE Codice IN (SELECT CodScheda  FROM #tmpFiltriSchede)
	END

						
		IF @bDatiEstesi=1
	BEGIN		
		SELECT Codice,Descrizione
		FROM T_StatoCartella
		WHERE Codice IN ('AP','CH')
		ORDER BY Codice
	END

	RETURN 0
END