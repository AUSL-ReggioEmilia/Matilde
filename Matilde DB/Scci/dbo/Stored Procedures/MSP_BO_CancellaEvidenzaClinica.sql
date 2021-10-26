CREATE PROCEDURE [dbo].MSP_BO_CancellaEvidenzaClinica(@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN



		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER
	
	DECLARE @xTimeStamp AS XML
	
		DECLARE @sCodRuolo AS VARCHAR(20)	
		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER	
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
		
		IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END
				

		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
		END					  						

		SET @bErrore=0
	SET @nRecord=0

			
	CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)
		
			
					
	IF @uIDEntitaInizio IS NOT NULL 
	BEGIN
		
		SELECT  @nQta=COUNT(*)			
		FROM T_MovEvidenzaClinica
		WHERE ID=@uIDEntitaInizio 
		
		IF @nQta=0
		BEGIN
			INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : nessuna evidenza clinica trovata')
			SET @bErrore=1
		END	
		
				
		SELECT  @nQta=COUNT(*)			
		FROM T_MovEvidenzaClinica
		WHERE ID=@uIDEntitaInizio AND CodStatoEvidenzaClinica='CA'
		
		
		IF @nQta>0
		BEGIN
			INSERT INTO #tmpErrori(Errore)
						VALUES('(GG002) ERRORE : Evideza Clinica già cancellata')
			SET @bErrore=1		
		END				
	END		

		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	
				IF @bErrore=0
	BEGIN	
								
				UPDATE T_MovEvidenzaClinica
		SET 				
			CodStatoEvidenzaClinica='CA'
		WHERE ID=@uIDEntitaInizio
														
				IF @bErrore=0 
		BEGIN			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
			SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')

						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEntitaInizio")}</IDEntita> into (/TimeStamp)[1]')
									
						SET @sInfoTimeStamp='Azione: MSP_BO_CancellaEvidenzaClinica'
			
			SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
			SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
			SET @xTimeStamp.modify('insert <CodAzione>CAN</CodAzione> into (/TimeStamp)[1]')

			SET @xTimeStamp=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
								'</Parametri>')
								
						EXEC MSP_InsMovTimeStamp @xTimeStamp					
		END 	END	
	
				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END