CREATE FUNCTION [dbo].[MF_SQLQueryAgendeAGE](@sCodAgenda AS VARCHAR(20))
 RETURNS VARCHAR(MAX)
  AS
  BEGIN
 
	DECLARE @sConfig VARCHAR(MAX)
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @sTmp VARCHAR(MAX)
	DECLARE @xTmp XML
	
	SELECT TOP 1 @xTmp=ElencoCampi,
				@sCodEntita=CodEntita
		 FROM T_Agende WHERE Codice=@sCodAgenda
	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
		
	SET @sTmp=''
	SELECT	@sTmp =  @sTmp +
														CASE 
								WHEN @sTmp='' THEN ''
								ELSE  ' + '' '' + '
							END 
						  + ValoreParametro.string.value('.','VARCHAR(255)')						
						 FROM @xTmp.nodes('/ArrayOfString/string') as ValoreParametro(string)
	IF @sCodEntita='EPI'
		BEGIN			
			IF 	@sTmp<> ''
				SET @sTmp= 'IDPaziente,IDEpisodio,' +	@sTmp + ' AS Oggetto'
			ELSE
				SET @sTmp= 'IDPaziente,IDEpisodio, NULL AS Oggetto'		 
			
			SET @sTmp='SELECT ' + @sTmp + '
						FROM 
						Q_SelCampiAgendeEPI'
		END								
	ELSE
		IF @sCodEntita='PAZ'
			BEGIN
			
			IF 	@sTmp<> ''
				SET @sTmp= 'IDPaziente,' +	@sTmp + ' AS Oggetto'
			ELSE
				SET @sTmp= 'IDPaziente, NULL AS Oggetto'		 
			
			SET @sTmp='SELECT ' + @sTmp + '
						FROM 
						Q_SelCampiAgendeEPI'
			END
		ELSE
			SET @sTmp='SELECT NULL AS IDPaziente, NULL AS IDEpisodio, NULL AS Oggetto'
				
	RETURN @sTmp
  END