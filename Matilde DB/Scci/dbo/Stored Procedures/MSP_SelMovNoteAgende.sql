CREATE PROCEDURE [dbo].[MSP_SelMovNoteAgende](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @bDatiEstesi AS Bit			
	DECLARE @sCodAgenda AS VARCHAR(1800)	
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
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDGruppo	=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodAgenda=''
	SELECT	@sCodAgenda =  @sCodAgenda +
														CASE 
								WHEN @sCodAgenda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodAgenda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
						 
	SET @sCodAgenda=LTRIM(RTRIM(@sCodAgenda))
	IF	@sCodAgenda='''''' SET @sCodAgenda=''
	SET @sCodAgenda=UPPER(@sCodAgenda)
				
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
								M.ID,								
								M.CodAgenda,
								A.Descrizione AS DescrAgenda,																	
								M.CodStatoNota,
								M.DataInizio, 
								M.DataFine,												
								M.Oggetto,
								M.Descrizione,
								M.Colore AS Colore,
								M.IDGruppo,
								M.DataEvento,
								ISNULL(M.TuttoIlGiorno,0) AS TuttoIlGiorno,
								ISNULL(M.EscludiDisponibilita,0) AS EscludiDisponibilita
						  '					
			SET @sSQL=@sSQL + '
						FROM 			
								T_MovNoteAgende	M																	
									LEFT JOIN T_Agende A
										ON (M.CodAgenda=A.Codice)
								'
													
												
			SET @sWhere=''
							
						IF 	@sCodAgenda NOT IN ('')
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.CodAgenda IN ('+ @sCodAgenda + ')
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