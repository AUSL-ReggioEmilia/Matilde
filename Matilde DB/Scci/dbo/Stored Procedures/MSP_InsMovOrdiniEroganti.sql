CREATE PROCEDURE [dbo].[MSP_InsMovOrdiniEroganti](@xParametri XML)
AS
BEGIN
		
	
																			
		
	DECLARE @uIDOrdine AS UNIQUEIDENTIFIER			
	DECLARE @sCodTipoOrdine AS VARCHAR(20)
	DECLARE @sDescrizioneTipoOrdine AS VARCHAR(255)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	DECLARE @sCodTmp AS VARCHAR(20)
	DECLARE @vIcona AS VARBINARY(MAX)
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDOrdine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdine') as ValoreParametro(IDOrdine))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDOrdine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sCodTipoOrdine=(SELECT TOP 1 ValoreParametro.CodTipoOrdine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoOrdine') as ValoreParametro(CodTipoOrdine))	
	
	
		SET @sDescrizioneTipoOrdine=(SELECT TOP 1 ValoreParametro.DescrizioneTipoOrdine.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneTipoOrdine') as ValoreParametro(DescrizioneTipoOrdine))						  
					  
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
						
					
	SET @uGUID=NEWID()

								
	
			
				
	
	
	
	
		SET @sCodTmp=(SELECT TOP 1 Codice FROM T_TipoOrdine WHERE Codice=@sCodTipoOrdine)
	
	IF @sCodTmp IS NULL
	BEGIN
				SET @vIcona=(SELECT TOP 1 Icona FROM T_TipoEvidenzaClinica
					 WHERE Codice='_DEFAULT_')
					 
		INSERT INTO T_TipoOrdine(Codice,Descrizione,Icona)
		VALUES
			(@sCodTipoOrdine,
			@sDescrizioneTipoOrdine,
			@vIcona)
	END
	
		INSERT INTO T_MovOrdiniEroganti(	
					ID				
					,IDOrdine
					,CodTipoOrdine					
				  )
		VALUES
				(
					@uGUID														,@uIDOrdine													,@sCodTipoOrdine										)
	RETURN 0
END