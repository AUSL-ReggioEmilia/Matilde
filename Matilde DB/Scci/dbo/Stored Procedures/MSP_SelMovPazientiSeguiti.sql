CREATE PROCEDURE [dbo].[MSP_SelMovPazientiSeguiti](@xParametri XML)
AS
BEGIN

	
	
				DECLARE @uIDPazienteSeguito AS UNIQUEIDENTIFIER	
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	
	DECLARE @sCognome AS VARCHAR(255)
	DECLARE @sNome AS VARCHAR(255)
	DECLARE @sLuogoNascita AS VARCHAR(255)
	DECLARE @sCodiceFiscale	AS VARCHAR(16)
	DECLARE @sDataNascita AS VARCHAR(10)
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @bPazientiSeguitiDaUtente AS BIT
	DECLARE @bPazientiSeguitiDaAltri AS BIT
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)	
	
		DECLARE @dDataNascita AS DATETIME
	DECLARE @nTemp AS INTEGER
	DECLARE @bCancella AS BIT
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sSQL AS VARCHAR(MAX)			
	DECLARE @sWhere AS VARCHAR(Max)				
	DECLARE @sTmp AS VARCHAR(Max)	
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
	
									
		SET @sGUID=(SELECT TOP 1 ValoreParametro.ID.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ID') as ValoreParametro(ID))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPazienteSeguito=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sCognome=(SELECT TOP 1 ValoreParametro.Cognome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Cognome') as ValoreParametro(Cognome))
	IF @sCognome IS NOT NULL
		SET  @sCognome=REPLACE(@sCognome,'''','''''')	
		
		SET @sNome=(SELECT TOP 1 ValoreParametro.Nome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Nome') as ValoreParametro(Nome))
	IF @sNome IS NOT NULL
		SET  @sNome=REPLACE(@sNome,'''','''''')	
		
		SET @sLuogoNascita=(SELECT TOP 1 ValoreParametro.LuogoNascita.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LuogoNascita') as ValoreParametro(LuogoNascita))
				  
	IF @sLuogoNascita IS NOT NULL
		SET  @sLuogoNascita=REPLACE(@sLuogoNascita,'''','''''')	
	
		
		SET @sCodiceFiscale=(SELECT TOP 1 ValoreParametro.CodiceFiscale.value('.','VARCHAR(16)')
					  FROM @xParametri.nodes('/Parametri/CodiceFiscale') as ValoreParametro(CodiceFiscale))
	IF @sCodiceFiscale IS NOT NULL
		SET  @sCodiceFiscale=REPLACE(@sCodiceFiscale,'''','''''')	
		
		SET @sDataNascita=(SELECT TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @bPazientiSeguitiDaUtente=(SELECT TOP 1 ValoreParametro.PazientiSeguitiDaUtente.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/PazientiSeguitiDaUtente') as ValoreParametro(PazientiSeguitiDaUtente))											
	SET @bPazientiSeguitiDaUtente=ISNULL(@bPazientiSeguitiDaUtente,0)
	
		SET @bPazientiSeguitiDaAltri=(SELECT TOP 1 ValoreParametro.PazientiSeguitiDaAltri.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/PazientiSeguitiDaAltri') as ValoreParametro(PazientiSeguitiDaAltri))											
	SET @bPazientiSeguitiDaAltri=ISNULL(@bPazientiSeguitiDaAltri,0)
	
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))
					  
				

	
	IF @uIDPazienteSeguito IS NOT  NULL	
		BEGIN
						 SELECT 
				M.*,				
				P.CodSAC				
			FROM 
				T_MovPazientiSeguiti M		
					LEFT JOIN T_Pazienti P
							ON M.IDPaziente=P.ID					
			WHERE				
				M.ID=@uIDPazienteSeguito				
		END	
	ELSE		
		IF @bPazientiSeguitiDaUtente=1
			BEGIN
								SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
					WHERE CodModulo='PazientiSeguiti_Cancella'	
						 AND CodRuolo=@sCodRuolo)
				IF @nTemp>=1 
					SET @bCancella=1
				ELSE
					SET @bCancella=0
										
								SET @sWhere=''
				SET @sSQL='
							SELECT 
								M.ID,			
								ISNULL(PA.IDPaziente,M.IDPaziente) AS IDPaziente,
								
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
								P.CodSAC,
								CONVERT(INTEGER,' + CONVERT(CHAR(1),@bCancella) + ') AS PermessoCancella
								
							FROM 
								T_MovPazientiSeguiti M		
									LEFT JOIN T_PazientiAlias PA	
										ON M.IDPaziente=PA.IDPazienteVecchio 
									LEFT JOIN T_Pazienti P
										ON ISNULL(PA.IDPaziente,M.IDPaziente)=P.ID '
			
						SET @sTmp= ' AND M.CodUtente= ''' + @sCodUtente +''''				
			SET @sWhere= @sWhere + @sTmp
			
						SET @sTmp= ' AND M.CodRuolo= ''' + + @sCodRuolo  +''''				
			SET @sWhere= @sWhere + @sTmp
			
						SET @sTmp= ' AND M.CodStatoPazienteSeguito IN (''IC'')'
			SET @sWhere= @sWhere + @sTmp
															
			
						IF @sCognome IS NOT NULL
				BEGIN
					SET @sTmp= ' AND P.Cognome Like ''%' + @sCognome + '%'''
					SET @sWhere= @sWhere + @sTmp
				END
				
						IF @sNome IS NOT NULL
				BEGIN
					SET @sTmp= ' AND P.Nome Like ''%' + @sNome + '%'''
					SET @sWhere= @sWhere + @sTmp
				END
						IF @sCodiceFiscale IS NOT NULL
				BEGIN
					SET @sTmp=' AND P.CodiceFiscale Like ''%' + @sCodiceFiscale + '%'''
					SET @sWhere= @sWhere + @sTmp					
				END
				
						IF @sLuogoNascita IS NOT NULL
				BEGIN
					SET @sTmp=' AND P.ComuneNascita Like ''%' + @sLuogoNascita + '%'''
					SET @sWhere= @sWhere + @sTmp
				END
				
						IF @sDataNascita IS NOT NULL
				BEGIN
					SET @sTmp=' AND P.DataNascita = 	convert(datetime,''' + @sDataNascita + ''',105) '	
					SET @sWhere= @sWhere + @sTmp					
				END
				
			
								IF ISNULL(@sCodFiltroSpeciale,'') <> ''
				BEGIN
					SET @sSQLFiltro=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
					IF ISNULL(@sSQLFiltro,'') <> '' 
					BEGIN									
						SET @sTmp=' AND ISNULL(PA.IDPaziente,P.ID) IN (' + 	@sSQLFiltro + ') '
						SET @sWhere= @sWhere + @sTmp												
					END
				END	
				
						IF ISNULL(@sWhere,'')<> ''
			BEGIN	
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-4)
			END	
			
			SET @sSQL= @sSQL + CHAR(13) + CHAR(10) + 
							' ORDER BY 
								 ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''')  ASC		
						'		
			PRINT @sSQL
			EXEC (@sSQL)			
			END	
	
				IF @bPazientiSeguitiDaAltri=1
		BEGIN
						SELECT 				
				M.CodUtente,
				M.CodRuolo,								
				MIN(L.Descrizione) AS Utente,
				MIN(R.Descrizione) AS Ruolo,				
				CONVERT(varchar(20),MIN(M.DataInserimento),105) + ' ' +  Convert(varchar(5),MIN(M.DataInserimento),108) AS Data
			FROM 
				T_MovPazientiSeguiti M
					LEFT JOIN T_Login L
						ON M.CodUtente=L.Codice
					LEFT JOIN T_Ruoli R
						ON M.CodRuolo = R.Codice				
			WHERE
					(M.CodUtente <> @sCodUtente  OR								M.CodRuolo <> @sCodRuolo) AND								M.IDPaziente = @uIDPaziente AND
					M.CodStatoPazienteSeguito IN ('IC')					GROUP BY
				M.CodUtente,
				M.CodRuolo	
			ORDER BY Data ASC									
		END
	
	RETURN 0
END