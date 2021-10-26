CREATE PROCEDURE [dbo].[MSP_SelMovTrasferimenti](@xParametri XML)
AS
BEGIN

	
	
				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @dDataIngresso AS DATETIME
	DECLARE @sCodStatoTrasferimento AS VARCHAR(1800)
	DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @sCodAziTrasferimento AS VARCHAR(20)	
	
	
	DECLARE @sDataTmp AS VARCHAR(40)				
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER			
	DECLARE @xTmpTS AS XML
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
		SET @sCodStatoTrasferimento=''
	SELECT	@sCodStatoTrasferimento =  @sCodStatoTrasferimento +
														CASE 
								WHEN @sCodStatoTrasferimento='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento)
						 
	SET @sCodStatoTrasferimento=LTRIM(RTRIM(@sCodStatoTrasferimento))
	IF	@sCodStatoTrasferimento='''''' SET @sCodStatoTrasferimento=''
	SET @sCodStatoTrasferimento=UPPER(@sCodStatoTrasferimento)
					  
		  				  
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataIngresso.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataIngresso') as ValoreParametro(DataIngresso))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
		

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)									
			IF ISDATE(@sDataTmp)=1
				SET	@dDataIngresso=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataIngresso =NULL			
		END

								
		SET @sCodAziTrasferimento=(SELECT TOP 1 ValoreParametro.CodAziTrasferimento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAziTrasferimento') as ValoreParametro(CodAziTrasferimento))
	SET @sCodAziTrasferimento=ISNULL(@sCodAziTrasferimento,'')

		SET  @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	SET  @sCodUA=ISNULL(@sCodUA,'')
								  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


						
												
				
		
	SET @sWhere=''		
	SET @sSQL='	SELECT	* 
				FROM 
					T_MovTrasferimenti	M 
				'
	
		IF @uIDTrasferimento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDTrasferimento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END				
		
		IF @sCodStatoTrasferimento <> ''
	BEGIN
		SET @sTmp=  ' AND 			
					M.CodStatoTrasferimento IN ('+ @sCodStatoTrasferimento + ')
				'  				
		SET @sWhere= @sWhere + @sTmp	
	END	

		IF @sCodAziTrasferimento <> ''
	BEGIN
		SET @sTmp=  ' AND 			
					M.CodAziTrasferimento ='''+ @sCodAziTrasferimento +''''
				  				
		SET @sWhere= @sWhere + @sTmp	
	END	
	
		IF @sCodUA <> ''
	BEGIN
		SET @sTmp=  ' AND 			
					M.CodUA ='''+ @sCodUA +''''
				  				
		SET @sWhere= @sWhere + @sTmp	
	END	

		IF @dDataIngresso IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataIngresso = CONVERT(datetime,'''  + convert(varchar(30),@dDataIngresso,121) +''',121)'															
			SET @sWhere= @sWhere + @sTmp								
		END
				  
			IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
				
	PRINT @sSQL
 	
	 EXEC (@sSQL)
	
			
				
	
							
				
		
					
								
	
		RETURN 0
END