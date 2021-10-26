CREATE PROCEDURE [dbo].[MSP_SelMovFolder](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDFolder AS UNIQUEIDENTIFIER
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)

	DECLARE @sCodStatoFolder AS VARCHAR(MAX)
		
	DECLARE @bCancella AS BIT
	DECLARE @bModifica AS BIT

		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
							  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') NOT IN ('','NULL') SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolder.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDFolder') as ValoreParametro(IDFolder))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDFolder=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
						  				  	
		SET @sCodStatoFolder=''
	SELECT	@sCodStatoFolder =  @sCodStatoFolder +
														CASE 
								WHEN @sCodStatoFolder='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoFolder.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoFolder') as ValoreParametro(CodStatoFolder)						 
	SET @sCodStatoFolder=LTRIM(RTRIM(@sCodStatoFolder))
	IF	@sCodStatoFolder='''''' SET @sCodStatoFolder=''
	SET @sCodStatoFolder=UPPER(@sCodStatoFolder)		
			
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sCodEntita=''
	SELECT	@sCodEntita =  @sCodEntita +
														CASE 
								WHEN @sCodEntita='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodEntita.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita)						 
	SET @sCodEntita=LTRIM(RTRIM(@sCodEntita))
	IF	@sCodEntita='''''' SET @sCodEntita=''
	SET @sCodEntita=UPPER(@sCodEntita)

	    DECLARE @xTimeStamp AS XML	
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')															
	
				
				
	DECLARE @xTmp AS XML
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)


	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
				
	SET @sWhere=''		
	
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND (M.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
							OR
														M.IDPaziente IN 
									(SELECT IDPazienteVecchio
										FROM T_PazientiAlias
										WHERE 
										IDPaziente IN 
											(SELECT IDPaziente
												FROM T_PazientiAlias
												WHERE IDPazienteVecchio=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
											)
									)
						)'
			SET @sWhere= @sWhere + @sTmp								
		END			


							
								
		IF @uIDFolder IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDFolder) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @sCodStatoFolder <> ''
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoFolder IN ('+ @sCodStatoFolder + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
						SET @sTmp=  ' AND 			
							 M.CodStatoFolder <> ''CA''
						'  	
			SET @sWhere= @sWhere + @sTmp	
		END

		IF @sCodEntita <> ''
	BEGIN				
		SET @sTmp=  ' AND 			
							M.CodEntita IN ('+ @sCodEntita + ')
					'  				
		SET @sWhere= @sWhere + @sTmp														
	END

	SET @sWhere=' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)				
			
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Allegati_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
					
	SET @bCancella=@bModifica

							
			
					
	SET @sSQL='	SELECT	
						M.ID,
						M.IDPaziente,
						M.IDEpisodio,
						M.IDTrasferimento,
						M.IDFolderPadre,
						M.Descrizione,
						M.CodStatoFolder,
						S.Descrizione AS StatoFolder,
						M.CodEntita,
						M.CodUA,
						UA.Descrizione AS UA,
						M.CodUtenteRilevazione,
						LR.Descrizione As DescrUtenteRilevazione,
						M.CodUtenteUltimaModifica,
						LM.Descrizione As DescrUtenteModifica,
						M.DataRilevazione,
						M.DataUltimaModifica,
						CONVERT(INTEGER, + ' + CONVERT(CHAR(1), @bCancella) + ') AS PermessoCancella,
				        CONVERT(INTEGER, + ' + CONVERT(CHAR(1), @bModifica) + ') AS PermessoModifica
						'
											    					    	
		SET @sSQL=@sSQL + ' 						
				FROM T_MovFolder M 
						INNER JOIN 	#tmpUARuolo U
							ON M.CodUA=U.CodUA
						LEFT JOIN T_StatoFolder S
							ON (M.CodStatoFolder=S.Codice)
						LEFT JOIN T_UnitaAtomiche UA
							ON (M.CodUA=UA.Codice)
						LEFT JOIN T_Login LR
							ON (M.CodUtenteRilevazione=LR.Codice)
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)	
					  '	
		SET @sSQL=@sSQL + @sWhere

		SET @sSQL=@sSQL +' ORDER BY M.IDFolderPadre ' 	

	PRINT @sSQL
 	
	EXEC (@sSQL)
		
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

END