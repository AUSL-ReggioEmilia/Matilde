CREATE PROCEDURE [dbo].[MSP_SelMovDocumentiFirmati](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @uIDDocumento AS UNIQUEIDENTIFIER	

	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER		
	DECLARE @sCodTipoDocumentoFirmato AS VARCHAR(20)
	DECLARE @nNumero AS INTEGER
	
		
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(MAX)
    DECLARE @xTimeStamp AS XML	
    
    
						
		IF @xParametri.exist('/Parametri/IDDocumento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDDocumento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDDocumento') as ValoreParametro(IDDocumento))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDDocumento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
	
		IF @xParametri.exist('/Parametri/CodEntita')=1
	BEGIN
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))		
	END
	
		IF @xParametri.exist('/Parametri/CodTipoDocumentoFirmato')=1
	BEGIN
		SET @sCodTipoDocumentoFirmato=(SELECT TOP 1 ValoreParametro.CodTipoDocumentoFirmato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoDocumentoFirmato') as ValoreParametro(CodTipoDocumentoFirmato))		
	END
	
		IF @xParametri.exist('/Parametri/IDEntita')=1
	BEGIN
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
		IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,@sGUID)		
	END
	
		IF @xParametri.exist('/Parametri/Numero')=1
	BEGIN
		SET @nNumero=(SELECT TOP 1 ValoreParametro.Numero.value('.','INTEGER')
			 FROM @xParametri.nodes('/Parametri/Numero') as ValoreParametro(Numero))
	
	END
	
				
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

						


			

		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDDocumento")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
	SET @xTimeStamp.modify('insert <CodEntita>DCF</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
	SET @xTimeStamp.modify('insert <CodAzione>VIS</CodAzione> into (/TimeStamp)[1]')
	
		
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	
	IF @uIDDocumento IS NULL
	BEGIN
				IF @nNumero IS NULL
			SET @nNumero=(SELECT MAX(Numero) FROM T_movDocumentiFirmati WHERE CodEntita=@sCodEntita AND IDEntita=@uIDEntita AND CodTipoDocumentoFirmato=@sCodTipoDocumentoFirmato)
			
		SET @uIDDocumento=(SELECT TOP 1 ID FROM T_movDocumentiFirmati WHERE CodEntita=@sCodEntita AND IDEntita=@uIDEntita AND CodTipoDocumentoFirmato=@sCodTipoDocumentoFirmato AND Numero=@nNumero)
		
					END	

	SELECT 
			*,
			CASE 
				WHEN CodEntita='CAR' AND CodStatoEntita='AP' THEN 'Aperta'
				WHEN CodEntita='CAR' AND CodStatoEntita='CH' THEN 'Chiusa'
				ELSE ''
			END AS DescrStatoEntita
		FROM 
			T_MovDocumentiFirmati M				
		WHERE 
			ID=@uIDDocumento
					
	IF @@ROWCOUNT > 0
		BEGIN 
						EXEC MSP_InsMovTimeStamp @xTimeStamp
		END
	
	
				
END