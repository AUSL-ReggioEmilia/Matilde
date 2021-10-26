CREATE PROCEDURE [dbo].[MSP_SelMovReport](@xParametri AS XML)
AS
BEGIN
	   	
	   	
							
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @sCodUtente AS VARCHAR(100)		
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
			
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET  @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
							
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
						' ' + RIGHT(@sDataTmp,5) +
						':59'
			IF ISDATE(@sDataTmp)=1
			  BEGIN
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)										
			  END
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
						
						SET @sSQL='	SELECT
							M.ID,													
							DataEvento AS Data,
							ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' '' + 
								CASE 
									WHEN ISNULL(Sesso,'''')='''' THEN ''''
									ELSE '' ('' + Sesso +'') ''
								END +
								CASE 
									WHEN DataNascita IS NULL THEN ''''
									ELSE Convert(varchar(10),DataNascita,105)
								END +
			
								CASE 
									WHEN ISNULL(ComuneNascita,'''')='''' THEN ''''
									ELSE '', '' + ComuneNascita
								END +
									
								CASE 
									WHEN ISNULL(CodProvinciaNascita,'''')='''' THEN ''''
									ELSE '' ('' + CodProvinciaNascita + '')''
								END 
							AS Paziente,
							R.Descrizione AS Stampa,
							L.Descrizione AS Utente
						  '					
			SET @sSQL=@sSQL + '
						FROM 			
								T_MovReport	M
									LEFT JOIN T_Report R
										ON M.CodReport=R.Codice
									LEFT JOIN T_Login L
										ON M.CodLogin=L.Codice
									LEFT JOIN T_Pazienti P
										ON P.ID=M.IDPaziente
						'
													
												
			SET @sWhere=''										
			
						IF 	@uIDPaziente IS NOT NULL
				BEGIN						
					SET @sTmp= ' AND 
								(M.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +'''
								 OR
								 								 M.IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias
													 WHERE IDPazienteVecchio=''' + convert(varchar(50),@uIDPaziente) +'''
													)
											)
										
								 )'		
					SET @sWhere= @sWhere + @sTmp														
				END	
					
						IF 	@sCodUtente IS NOT NULL
				BEGIN	
										SET @sTmp=  ' AND 			
									 M.CodLogin ='''+  @sCodUtente + '''
								'  				
					SET @sWhere= @sWhere + @sTmp														
				END	
																											
										
						IF @dDataInizio IS NOT NULL 
				BEGIN
					SET @sTmp= CASE 
									WHEN @dDataFine IS NULL 
											THEN ' AND M.DataEvento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
									ELSE ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
								END	
					SET @sWhere= @sWhere + @sTmp								
				END

						IF @dDataFine IS NOT NULL 
				BEGIN
					SET @sTmp= ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
					SET @sWhere= @sWhere + @sTmp								
				END
			
															
						IF ISNULL(@sWhere,'')<> ''
			BEGIN			
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
			END	
			ELSE
				SET @sSQL=@sSQL +' WHERE 1=0'
	
	PRINT @sSQL
 	SET @sSQL=@sSQL + ' ORDER BY DataEvento DESC '
 	
 	EXEC (@sSQL)
		
	
								
END