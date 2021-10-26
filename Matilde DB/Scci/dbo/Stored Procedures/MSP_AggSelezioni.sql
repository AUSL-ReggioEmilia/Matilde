CREATE PROCEDURE [dbo].[MSP_AggSelezioni](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodice AS VARCHAR(20)
	DECLARE @txtDescrizione VARCHAR(MAX)		
	DECLARE @sCodTipoSelezione AS VARCHAR(20)
	DECLARE @xSelezioni XML
	DECLARE @bFlagSistema AS BIT
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100) 
	DECLARE @sCodRuoloUltimaModifica AS VARCHAR(20) 
				
		DECLARE @binSelezioni VARBINARY(MAX)
	DECLARE @txtSelezioni VARCHAR(MAX)	
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)

	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
	
		SET @sSQL='UPDATE T_Selezioni ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			  					
	SET @sSET =''
	
		IF @xParametri.exist('/Parametri/Codice')=1
		BEGIN
					SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
					END
				

		IF @xParametri.exist('/Parametri/Descrizione')=1
	BEGIN
				SET @txtDescrizione =(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	

		IF @txtDescrizione  <> ''
			SET	@sSET= @sSET +',Descrizione=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrizione
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Descrizione=NULL '	+ CHAR(13) + CHAR(10)	
	END
				
		IF @xParametri.exist('/Parametri/CodTipoSelezione')=1
		BEGIN
			SET @sCodTipoSelezione=(SELECT TOP 1 ValoreParametro.CodTipoSelezione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoSelezione') as ValoreParametro(CodTipoSelezione))	

			IF ISNULL(@sCodTipoSelezione,'') <> ''
				SET	@sSET= @sSET + ',CodTipoSelezione=''' + @sCodTipoSelezione +''''	+ CHAR(13) + CHAR(10)						
		END

		
		IF @xParametri.exist('/Parametri/Selezioni')=1	
	BEGIN					  	
		SET @xSelezioni=(SELECT TOP 1 ValoreParametro.Selezioni.query('./*')
					     FROM @xParametri.nodes('/Parametri/Selezioni') as ValoreParametro(Selezioni))				
			
					
		SET @binSelezioni=CONVERT(VARBINARY(MAX),@xSelezioni)
		SET @txtSelezioni=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binSelezioni"))', 'varchar(max)')
		SET @binSelezioni=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtSelezioni"))', 'varbinary(max)')
					
		SET	@sSET= @sSET +',Selezioni=
									CONVERT(XML,
											CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtSelezioni + 
											'")'', ''varbinary(max)'') 
										)'  + CHAR(13) + CHAR(10)	
		
	END
	ELSE
			SET	@sSET= @sSET +',Selezioni=NULL '	+ CHAR(13) + CHAR(10)														
		
	
		IF @xParametri.exist('/Parametri/FlagSistema')=1
	BEGIN
		    SET @bFlagSistema=(SELECT TOP 1 ValoreParametro.FlagSistema.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/FlagSistema') as ValoreParametro(FlagSistema))	
			IF @bFlagSistema IS NOT NULL		
				SET	@sSET= @sSET + ',FlagSistema=' + CONVERT(VARCHAR(1),@bFlagSistema) +''	+ CHAR(13) + CHAR(10)					
	END


		IF @xParametri.exist('/Parametri/TimeStamp/CodLogin')=1
	BEGIN
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))		

		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica +''''	+ CHAR(13) + CHAR(10)			
	END

		IF @xParametri.exist('/Parametri/TimeStamp/CodRuolo')=1
	BEGIN
		SET @sCodRuoloUltimaModifica=(SELECT TOP 1 ValoreParametro.CodRuoloUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuoloUltimaModifica))		

		SET	@sSET= @sSET + ',CodRuoloUltimaModifica=''' + @sCodRuoloUltimaModifica +''''	+ CHAR(13) + CHAR(10)			
	END

		SET	@sSET= @sSET + ',DataUltimaModifica=GetDate()'	+ CHAR(13) + CHAR(10)	
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=GetUTCDate()'	+ CHAR(13) + CHAR(10)					
	


	 		
	
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
			IF ISNULL(@sCodice,'') <> ''
				SET @sWHERE =' WHERE Codice=''' + @sCodice +''''
			ELSE
				SET @sWHERE =' WHERE 1=0'

						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	

			PRINT @sSQL
			EXEC (@sSQL)

		END		
			      		
															
	RETURN 0
END