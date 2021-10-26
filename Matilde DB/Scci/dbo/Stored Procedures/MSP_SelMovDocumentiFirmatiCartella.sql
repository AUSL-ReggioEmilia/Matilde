CREATE PROCEDURE [dbo].[MSP_SelMovDocumentiFirmatiCartella](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @bDatiEstesi AS Bit	

	
		
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(MAX)
    DECLARE @xTimeStamp AS XML	
    
    
						
		IF @xParametri.exist('/Parametri/IDCartella')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
				
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

						


			

		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCartella")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
	SET @xTimeStamp.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
	SET @xTimeStamp.modify('insert <CodAzione>VIS</CodAzione> into (/TimeStamp)[1]')
	
		
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	

	IF @bDatiEstesi=1
	BEGIN
		SELECT MDF.ID, MDF.IDNum
			, MDF.CodEntita, E.Descrizione As Entita, MDF.IDEntita
			, MDF.CodTipoDocumentoFirmato , TDF.Descrizione As TipoDocumento
			, MDF.Numero, MDF.CodUtenteInserimento, L.Descrizione As UtenteInserimento
			, MDF.DataInserimento, MDF.DataInserimentoUTC
			, MDF.PDFFirmato
			, MDF.FlagEsportato
			, MDF.DataEsportazione
			, MDF.NomeFileEsportatoP7M
			, MDF.NomeFileEsportatoXML
			, MDF.CodStatoEntita
			, SC.Descrizione AS DescrStatoEntita
		FROM T_MovDocumentiFirmati MDF
			LEFT JOIN T_Entita E
				ON (MDF.CodEntita = E.Codice)
			LEFT JOIN T_TipoDocumentoFirmato TDF 
				ON (MDF.CodTipoDocumentoFirmato = TDF.Codice)
			LEFT JOIN T_Login L 
				ON (MDF.CodUtenteInserimento = L.Codice)
			LEFT JOIN T_StatoCartella SC
				ON (MDF.CodStatoEntita = SC.Codice)
		WHERE 
			MDF.CodTipoDocumentoFirmato = 'CARFM01'
			AND MDF.CodEntita = 'CAR'
			AND MDF.IDEntita = @uIDCartella
	END
	ELSE
	BEGIN
		SELECT MDF.ID, MDF.IDNum
			, MDF.CodEntita, E.Descrizione As Entita, MDF.IDEntita
			, MDF.CodTipoDocumentoFirmato , TDF.Descrizione As TipoDocumento
			, MDF.Numero, MDF.CodUtenteInserimento, L.Descrizione As UtenteInserimento
			, MDF.DataInserimento, MDF.DataInserimentoUTC
			, MDF.CodStatoEntita
			, SC.Descrizione AS DescrStatoEntita
		FROM T_MovDocumentiFirmati MDF
			LEFT JOIN T_Entita E 
				ON (MDF.CodEntita = E.Codice)
			LEFT JOIN T_TipoDocumentoFirmato TDF 
				ON (MDF.CodTipoDocumentoFirmato = TDF.Codice)
			LEFT JOIN T_Login L 
				ON (MDF.CodUtenteInserimento = L.Codice)
			LEFT JOIN T_StatoCartella SC
				ON (MDF.CodStatoEntita = SC.Codice)
		WHERE 
			MDF.CodTipoDocumentoFirmato = 'CARFM01'
			AND MDF.CodEntita = 'CAR'
			AND MDF.IDEntita = @uIDCartella
	END
					
	IF @@ROWCOUNT > 0
		BEGIN 
						EXEC MSP_InsMovTimeStamp @xTimeStamp
		END			
END