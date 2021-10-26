CREATE FUNCTION dbo.MF_SQLQueryAgendeEPI()
 RETURNS VARCHAR(MAX)
  AS
  BEGIN			
  
	DECLARE @sConfig VARCHAR(MAX)
	DECLARE @sTmp VARCHAR(MAX)
	DECLARE @xTmp XML
	
	SET @sConfig = (SELECT TOP 1 Valore FROM T_Config WHERE ID=80)
	SET @xTmp=CONVERT(xml,@sConfig)
		
	SET @sTmp=''
	SELECT	@sTmp =  @sTmp +
														CASE 
								WHEN @sTmp='' THEN ''
								ELSE  ' + '' '' + '
							END 
						  + ValoreParametro.string.value('.','VARCHAR(255)')						
						 FROM @xTmp.nodes('/ArrayOfString/string') as ValoreParametro(string)
	
	IF 	@sTmp<> ''
		SET @sTmp= 'IDPaziente,IDEpisodio,' +	@sTmp + ' AS Oggetto'
	ELSE
		SET @sTmp= 'IDPaziente,IDEpisodio, NULL AS Oggetto'		 
	
	SET @sTmp='SELECT ' + @sTmp + '
				FROM 
				Q_SelCampiAgendeEPI'
										
	RETURN @sTmp
  END