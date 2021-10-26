CREATE PROCEDURE [dbo].[MSP_SelPazientiFiltriSpeciali] (@xParametri XML)
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
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	
		DECLARE @dDataNascita AS DATETIME
	DECLARE @nTemp AS INTEGER
	DECLARE @bCancella AS BIT
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sSQL AS VARCHAR(MAX)			
	DECLARE @sWhere AS VARCHAR(Max)				
	DECLARE @sTmp AS VARCHAR(Max)	
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
									
					
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
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))


					
		SET @sWhere=''
	SET @sSQL='
			SELECT *
			FROM 
				(SELECT 		
					DISTINCT 					
					ISNULL(PA.IDPaziente,P.ID) AS IDPaziente,
					
											CONVERT(VARBINARY(MAX),NULL) AS Icona,
						ISNULL(ISNULL(PF.Cognome,P.Cognome),'''') + '' '' + ISNULL(ISNULL(PF.Nome,P.Nome),'''') 
						+ '' ('' + ISNULL(PF.Sesso,P.Sesso) + '') '' 
						+ '' (''+
						+ 	CASE
								WHEN ISNULL(PF.DataNascita,P.DataNascita) IS NULL THEN ''''
								ELSE REPLACE(CONVERT(VARCHAR(10),ISNULL(PF.DataNascita,P.DataNascita),105),''-'',''/'')
							END
												+ '', ''  
						+ 	CASE	
								WHEN ISNULL(PF.ComuneNascita,P.ComuneNascita) IS NULL THEN ''''
								ELSE ISNULL(ISNULL(PF.ComuneNascita,P.ComuneNascita),'''') 
							END	
												+ '')'' 
						+ '' C.F. '' + ISNULL(ISNULL(PF.CodiceFiscale,P.CodiceFiscale),'''')
											AS Descrizione,
					
					ISNULL(ISNULL(PF.Cognome,P.Cognome),'''') + '' '' + ISNULL(ISNULL(PF.Nome,P.Nome),'''')   AS Paziente,
					ISNULL(PF.Sesso,P.Sesso) AS Sesso,
						CASE
							WHEN ISNULL(PF.DataNascita,P.DataNascita) IS NULL THEN ''''
							ELSE REPLACE(CONVERT(VARCHAR(10),ISNULL(PF.DataNascita,P.DataNascita),105),''-'',''/'')
						END 
						+ '' '' + 
						CASE	
							WHEN ISNULL(PF.ComuneNascita,P.ComuneNascita) IS NULL THEN ''''
							ELSE ISNULL(ISNULL(PF.ComuneNascita,P.ComuneNascita),'''') 
						END
					AS NascitaDescrizione,
					ISNULL(ISNULL(PF.CodiceFiscale,P.CodiceFiscale),'''''''') AS CodiceFiscale,
					ISNULL(ISNULL(PF.ComuneResidenza,P.ComuneResidenza),'''''''') AS ComuneResidenza,		
					ISNULL(PF.CodSAC,P.CodSAC) AS CodSAC
					
				FROM 
					T_Pazienti P WITH (NOLOCK)										
						LEFT JOIN T_PazientiAlias PA WITH (NOLOCK)
							ON P.ID=PA.IDPazienteVecchio 
						LEFT JOIN T_Pazienti PF WITH (NOLOCK)
							ON PA.IDPaziente = PF.ID
			'

	

	

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
						SET @sTmp=' AND ' + 	@sSQLFiltro + ' '
			SET @sWhere= @sWhere + @sTmp
		END
	END	
	
	
		
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-4)
	END	
	ELSE
	BEGIN
			SET @sSQL=@sSQL +' WHERE 1=0'
	END

	SET @sSQL= @sSQL + CHAR(13) + CHAR(10) + 
					' ) AS Q
					ORDER BY 
						Q.Descrizione  ASC		
				'		
	PRINT @sSQL
	EXEC (@sSQL)			
	
	RETURN 0
END