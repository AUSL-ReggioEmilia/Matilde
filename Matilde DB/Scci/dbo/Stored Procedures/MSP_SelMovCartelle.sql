CREATE PROCEDURE [dbo].[MSP_SelMovCartelle](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @sNumeroCartella VARCHAR(50)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @bDatiEstesi AS Bit	
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(MAX)
    DECLARE @xTimeStamp AS XML	
    
    
				
		IF @xParametri.exist('/Parametri/NumeroCartella')=1
	BEGIN
		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))	
		SET @sNumeroCartella=ISNULL(@sNumeroCartella,'')
	END	
	
		IF @xParametri.exist('/Parametri/IDCartella')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
	ELSE	
	BEGIN
			SET @uIDCartella=(SELECT TOP 1 ID FROM T_MovCartelle WHERE NumeroCartella=@sNumeroCartella)
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
	
		SET @xTimeStamp.modify('delete (/TimeStamp/Note)[1]') 			
	SET @xTimeStamp.modify('insert <Note>{sql:variable("@sNumeroCartella")}</Note> into (/TimeStamp)[1]')
	
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	
	SELECT 
			ID,
			IDNum,
			NumeroCartella,
			CodStatoCartella,
			CodUtenteApertura,
			CodUtenteChiusura,
			DataApertura,
			DataAperturaUTC,
			DataChiusura,
			DataChiusuraUTC,
			CASE 
				WHEN @bDatiEstesi=1 THEN PDFCartella
				ELSE NULL
			END AS PDFCartella,
			ISNULL(LA.Descrizione,'') AS UtenteApertura,
			ISNULL(LC.Descrizione,'') AS UtenteChiusura
		FROM 
			T_MovCartelle M
				LEFT JOIN T_Login AS LA
					ON M.CodUtenteApertura=LA.Codice
				LEFT JOIN T_Login AS LC
					ON M.CodUtenteChiusura=LC.Codice	
		WHERE 
			ID=@uIDCartella
	
	IF @@ROWCOUNT > 0
		BEGIN 
						EXEC MSP_InsMovTimeStamp @xTimeStamp
		END
	
	
				
END