

CREATE PROCEDURE [dbo].[MSP_SelMovNote](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @bDatiEstesi AS Bit			
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodEntita AS VARCHAR(1800)	
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	DECLARE @sCodSezione AS VARCHAR(1800)	
	DECLARE @sCodVoce AS VARCHAR(600)	
	DECLARE @uIDGruppo AS UNIQUEIDENTIFIER
	DECLARE @uIDNota AS UNIQUEIDENTIFIER
	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
		
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(Max)	
	
	DECLARE @nTemp AS INTEGER
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bCancella AS BIT
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDNota.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDNota') as ValoreParametro(IDNota))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDNota=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET  @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET  @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET  @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita	=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDGruppo	=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

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
	
		SET @sCodSezione=''
	SELECT	@sCodSezione =  @sCodSezione +
														CASE 
								WHEN @sCodSezione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodSezione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione)
						 
	SET @sCodSezione=LTRIM(RTRIM(@sCodSezione))
	IF	@sCodSezione='''''' SET @sCodSezione=''
	SET @sCodSezione=UPPER(@sCodSezione)


		SET @sCodVoce=(SELECT TOP 1 ValoreParametro.CodVoce.value('.','VARCHAR(600)')
					  FROM @xParametri.nodes('/Parametri/CodVoce') as ValoreParametro(CodVoce))	
					
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL		
		END 					
						  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
						
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
		
				
									
						SET @sSQL='	SELECT													
								*
						  '					
			SET @sSQL=@sSQL + '
						FROM 			
								T_MovNote	M																										
								'
													
												
			SET @sWhere=''
							
						IF 	@sCodEntita NOT IN ('')
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.CodEntita IN ('+ @sCodEntita + ')
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END		
																				
						IF 	@uIDEntita IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.IDEntita ='''+  CONVERT(VARCHAR(50),@uIDEntita) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
			
						IF 	@uIDPaziente IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.IDPaziente ='''+  CONVERT(VARCHAR(50),@uIDPaziente) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
			
						IF 	@uIDEpisodio IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.IDEpisodio ='''+  CONVERT(VARCHAR(50),@uIDEpisodio) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
						
						IF 	@uIDTrasferimento IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.IDTrasferimento ='''+  CONVERT(VARCHAR(50),@uIDTrasferimento) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
				
						IF 	@sCodSezione NOT IN ('')
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.CodSezione IN ('+ @sCodSezione + ')
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END																		
			
						IF 		@sCodVoce NOT IN ('')
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.CodVoce = ('''+ 	@sCodVoce + ''')
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
				
						IF 	@uIDNota IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.ID ='''+ CONVERT(VARCHAR(50),@uIDNota) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
						
										
						IF 	@uIDGruppo IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.IDGruppo ='''+  CONVERT(VARCHAR(50),@uIDGruppo) + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
									
						IF @dDataInizio IS NOT NULL 
				BEGIN
					SET @sTmp= CASE 
									WHEN @dDataFine IS NULL 
											THEN ' AND M.DataInizio = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
									ELSE ' AND M.DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
								END	
					SET @sWhere= @sWhere + @sTmp								
				END

						IF @dDataFine IS NOT NULL 
				BEGIN
					SET @sTmp= ' AND M.DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
					SET @sWhere= @sWhere + @sTmp								
				END
			
												
			SET @sTmp=  ' AND 			
							 M.CodStatoNota <> ''CA''				  
						'  	
				
						IF ISNULL(@sTmp,'') <> '' 		
				SET @sWhere= @sWhere + @sTmp	
					
						
						IF ISNULL(@sWhere,'')<> ''
			BEGIN
			
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
			END	
	
	PRINT @sSQL
 	SET @sSQL=@sSQL + ' ORDER BY DataInizio ASC '
 	
 	EXEC (@sSQL)
		
	
				EXEC MSP_InsMovTimeStamp @xTimeStamp	
															
				
END