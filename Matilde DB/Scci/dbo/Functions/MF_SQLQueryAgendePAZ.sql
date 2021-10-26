CREATE FUNCTION dbo.MF_SQLQueryAgendePAZ()
 RETURNS VARCHAR(MAX)
  AS
  BEGIN  		
  
	DECLARE @sConfig VARCHAR(MAX)
	DECLARE @sTmp VARCHAR(MAX)
	DECLARE @xTmp XML
	
	SET @sConfig = (SELECT TOP 1 Valore FROM T_Config WHERE ID=90)
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
		SET @sTmp= 'IDPaziente,' +	@sTmp + ' AS Oggetto'
	ELSE
		SET @sTmp= 'IDPaziente,IDEpisodio, NULL AS Oggetto'		 
	
	SET @sTmp='SELECT ' + @sTmp + '
				FROM 
				Q_SelCampiAgendePAZ'
										
	RETURN @sTmp
  END