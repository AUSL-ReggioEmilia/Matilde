CREATE PROCEDURE [dbo].[MSP_InsSelezioni](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodice AS VARCHAR(20)
	DECLARE @binDescrizione AS VARBINARY(MAX)
	DECLARE @sCodTipoSelezione AS VARCHAR(20)
	DECLARE @xSelezioni XML
	DECLARE @bFlagSistema AS BIT
	DECLARE @sCodUtenteInserimento AS VARCHAR(100) 
	DECLARE @sCodRuoloInserimento AS VARCHAR(20) 
				
			
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	

		SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
	
		SET @binDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))			
	
		SET @sCodTipoSelezione=(SELECT TOP 1 ValoreParametro.CodTipoSelezione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoSelezione') as ValoreParametro(CodTipoSelezione))	

		SET @xSelezioni=(SELECT TOP 1 ValoreParametro.Selezioni.query('./*')
					  FROM @xParametri.nodes('/Parametri/Selezioni') as ValoreParametro(Selezioni))		
	
	    SET @bFlagSistema=(SELECT TOP 1 ValoreParametro.FlagSistema.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/FlagSistema') as ValoreParametro(FlagSistema))	
	SET @bFlagSistema=ISNULL(@bFlagSistema,0)
	
		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))		

		SET @sCodRuoloInserimento=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	

	 		
	
			
			INSERT INTO T_Selezioni
				   (   Codice
					  ,Descrizione
					  ,CodTipoSelezione
					  ,Selezioni
					  ,FlagSistema
					  ,CodUtenteInserimento
					  ,CodRuoloInserimento
					  ,DataInserimento
					  ,DataInserimentoUTC
				   )
			VALUES
				(			
					@sCodice												   ,CONVERT(VARCHAR(MAX),@binDescrizione)					   ,@sCodTipoSelezione										   ,@xSelezioni												   ,@bFlagSistema											   ,@sCodUtenteInserimento									   ,@sCodRuoloInserimento									   ,GETDATE()												   ,GETUTCDATE()											 )  
			      		
															
	RETURN 0
END