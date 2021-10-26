CREATE PROCEDURE [dbo].[MSP_ContaEpisodiNosologico] (@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @sCodAzi AS VARCHAR(20)
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)

		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
	DECLARE @nRet AS INTEGER
	
				
	
	
		SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))
	SET @sNumeroNosologico=ISNULL(@sNumeroNosologico,'')
	
		SET @sNumeroListaAttesa = (SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa))
	SET @sNumeroListaAttesa = ISNULL(@sNumeroListaAttesa,'') 
				SET @nRet =0

	IF (@sNumeroNosologico <> '' OR @sNumeroListaAttesa <> '')
	BEGIN
		SET @sSQL='			
					SELECT COUNT(*) AS QTA
					FROM T_MovEpisodi M									
				  '										

		SET @sWhere=''								

		SET @sTmp= ' AND M.CodStatoEpisodio NOT IN (''CA'',''AN'') ' 
		SET @sWhere= @sWhere + @sTmp	

				IF @sNumeroNosologico <> ''
			BEGIN
				SET @sTmp= 'AND M.NumeroNosologico=''' + @sNumeroNosologico +''''
				SET @sWhere= @sWhere + @sTmp								
			END		
	
				IF @sNumeroListaAttesa <> ''
			BEGIN
				SET @sTmp= ' AND M.NumeroListaAttesa=''' + @sNumeroListaAttesa +''''
				SET @sWhere= @sWhere + @sTmp								
			END		
	
				IF ISNULL(@sWhere,'')<> ''
		BEGIN				
			SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		END	

		EXEC (@sSQL)
	END
	ELSE
	BEGIN	
		SELECT @nRet AS Qta
	END
							
	RETURN 0
END