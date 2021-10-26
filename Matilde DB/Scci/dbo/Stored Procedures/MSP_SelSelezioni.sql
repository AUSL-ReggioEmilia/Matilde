CREATE PROCEDURE [dbo].[MSP_SelSelezioni](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodice AS VARCHAR(20)	
	DECLARE @sCodTipoSelezione AS VARCHAR(20)
	DECLARE @bDatiEstesi AS BIT
	DECLARE @bSoloUtente AS BIT
	DECLARE @sCodLogin AS VARCHAR(100) 
	DECLARE @sCodRuolo AS VARCHAR(20) 
				
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	

		SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
			
		SET @sCodTipoSelezione=(SELECT TOP 1 ValoreParametro.CodTipoSelezione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoSelezione') as ValoreParametro(CodTipoSelezione))	

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))					  
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

	    SET @bSoloUtente=(SELECT TOP 1 ValoreParametro.SoloUtente.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/SoloUtente') as ValoreParametro(SoloUtente))	
	SET @bSoloUtente=ISNULL(@bSoloUtente,0)

		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))		
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))		
	
				
	SET @sWhere=''		
	SET @sSQL='	SELECT	
					   Codice
					  ,Descrizione
					  ,CodTipoSelezione'

	IF @bDatiEstesi=1 
		SET @sSQL=@sSQL + ',Selezioni'
	ELSE
		SET @sSQL=@sSQL + ', NULL AS Selezioni'
		
	SET @sSQL=@sSQL +',FlagSistema
					  ,CodUtenteInserimento
					  ,CodRuoloInserimento
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,CodUtenteUltimaModifica
					  ,CodRuoloUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,CONVERT(bit,
						CASE
							WHEN IsNull(FlagSistema, 0) = 1 THEN 0
							'

	IF @sCodLogin IS NOT NULL
		BEGIN
						SET @sSQL=@sSQL +' WHEN M.CodUtenteInserimento=''' + @sCodLogin +''' THEN 1 '
		END
	
	SET @sSQL=@sSQL +'		ELSE 0 
						END) As PERMESSOMODIFICA
				FROM 
					T_Selezioni	M 
				'
	
		IF @sCodice IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.Codice=''' + @sCodice +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
				
		IF @sCodTipoSelezione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.CodTipoSelezione=''' + @sCodTipoSelezione +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
								 
	
		IF @bSoloUtente=0 
	BEGIN
						SET @sTmp= ' AND ( M.Codice IN (SELECT CodSelezione FROM T_AssRuoliSelezioni WHERE CodRuolo=''' + @sCodRuolo +''')'
			
			IF @sCodLogin IS NOT NULL
				SET @sTmp= @sTmp + ' OR M.CodUtenteInserimento=''' + @sCodLogin +''''
			
			SET @sTmp= @sTmp + ')'
			SET @sWhere= @sWhere + @sTmp					
	END
	ELSE
	BEGIN
				IF @sCodLogin IS NOT NULL
			BEGIN
				SET @sTmp= ' AND M.CodUtenteInserimento=''' + @sCodLogin +''''
				SET @sWhere= @sWhere + @sTmp								
			END	
	END
					      		
				IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	

	SET @sSQL=@sSQL +' ORDER BY Descrizione ' 
	
				
	PRINT @sSQL 	
	EXEC (@sSQL)
													
	RETURN 0
END