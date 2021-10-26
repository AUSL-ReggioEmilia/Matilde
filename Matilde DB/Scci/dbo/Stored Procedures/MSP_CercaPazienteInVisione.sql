CREATE PROCEDURE [dbo].[MSP_CercaPazienteInVisione](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)	
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @nNumRighe AS INTEGER
	
	DECLARE @sCodLogin AS VARCHAR(100)
	
		DECLARE @sSQL AS VARCHAR(MAX)	
	DECLARE @sSQLPazientiSeguiti AS VARCHAR(MAX)
	DECLARE @sSQLPIV AS VARCHAR(MAX)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
				
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')			
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
		
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 		
	
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))
	SET @nNumRighe=ISNULL(@nNumRighe,0)					
		
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
	

				
				
	DECLARE @xTmp AS XML
			
					

		
			
		

	
				
	DECLARE @sWhere AS VARCHAR(Max)
	
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
		SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''
		
					SET @sWhere= @sWhere + '     )' 					
	END				
						
		SET @sWhere= @sWhere + ' AND P.ID IN
								  (SELECT ISNULL(PA.IDPaziente,M.IDPaziente) AS IDPaziente
								   FROM 
								     T_MovPazientiInVisione M
										LEFT JOIN T_PazientiAlias PA	
											ON M.IDPaziente=PA.IDPazienteVecchio 
								   WHERE
										 M.CodRuoloInVisione	=''' + @sCodRuolo + '''							   
										 AND M.CodStatoPazienteInVisione IN (''IC'')		
										 AND DataInizio <=GetDate() AND DataFine>=GetDate() 								  
								   )' 		  								    			

				
							
	SET @sSQL='
		SELECT 		
			P.ID AS IDPaziente,
			
					CONVERT(VARBINARY(MAX),NULL) AS Icona,
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') 
			+ '' ('' + P.Sesso + '') '' 
			+ '' (''+
			+ 	CASE
					WHEN P.DataNascita IS NULL THEN ''''
					ELSE REPLACE(CONVERT(VARCHAR(10),P.DataNascita,105),''-'',''/'')
				END
						+ '', ''  
			+ 	CASE	
					WHEN P.ComuneNascita IS NULL THEN ''''
					ELSE ISNULL(P.ComuneNascita,'''') 
				END	
						+ '')'' 
			+ '' C.F. '' + ISNULL(P.CodiceFiscale,'''')
					AS Descrizione,
								
		ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''')   AS Paziente,
		P.Sesso AS Sesso,
			CASE
				WHEN P.DataNascita IS NULL THEN ''''
				ELSE REPLACE(CONVERT(VARCHAR(10),P.DataNascita,105),''-'',''/'')
			END 
			+ '' '' + 
			CASE	
				WHEN P.ComuneNascita IS NULL THEN ''''
				ELSE ISNULL(P.ComuneNascita,'''') 
			END
		AS NascitaDescrizione,
		ISNULL(P.CodiceFiscale,'''''''') AS CodiceFiscale,
		ISNULL(P.ComuneResidenza,'''''''') AS ComuneResidenza,	
		P.CodSAC		
		FROM 
			 T_Pazienti P		
		'	  					

		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END
	ELSE
		SET @sSQL=@sSQL +' WHERE 1=0'
	
		EXEC (@sSQL)
	
	SET @sSQL=''
		
	
	
				
										
		
           
	RETURN 0
	
END