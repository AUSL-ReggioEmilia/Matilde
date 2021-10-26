CREATE PROCEDURE [dbo].[MSP_BO_SbloccoScheda](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStamp AS XML
	
		DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE	@uIDPazienteInizio AS UNIQUEIDENTIFIER	
	DECLARE	@uIDTrasferimentoInizio AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodRuolo AS VARCHAR(20)		
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER			
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @sSQL AS VARCHAR(MAX)			
	DECLARE @xParTmp AS XML
	DECLARE @xTimeStampBase AS XML
	
	
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
					
	IF @uIDEntitaInizio IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(CC001) ERRORE : IDScheda non valorizzato')
		SET @bErrore=1
	END
	ELSE
		BEGIN
						SELECT 
				  @nRecord=COUNT(*)
			FROM T_MovSchede
			WHERE 
				ID=@uIDEntitaInizio	AND
				Storicizzata=0 AND
				CodStatoScheda <> 'CA' 
				
			IF @nRecord =0 
				BEGIN			
					INSERT INTO #tmpErrori(Errore)
							VALUES('(CC002) ERRORE : ID passato non è una scheda valida, è una scheda storicizzata o cancellata')
					SET @bErrore=1
				END					
			
						IF @bErrore=0
			BEGIN						
				SELECT 
					  @nRecord=COUNT(*)
				FROM T_MovLock
				WHERE 
					IDEntita=@uIDEntitaInizio AND
					CodEntita='SCH'
					
				IF @nRecord =0 
					BEGIN			
						INSERT INTO #tmpErrori(Errore)
								VALUES('(CC003) ERRORE : Scheda non bloccata')
						SET @bErrore=1
					END		
			END					
		END	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

			IF @bErrore=0
		BEGIN					
		
			SELECT 
				@uIDEpisodioInizio=IDEpisodio,
				@uIDPazienteInizio=IDPaziente,
				@uIDTrasferimentoInizio=IDTrasferimento
			FROM T_MovSchede
			WHERE ID=@uIDEntitaInizio
			
						DELETE
			FROM T_MovLock
			WHERE 
				IDEntita=@uIDEntitaInizio AND
				CodEntita='SCH'				
		
								SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
				SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')

								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEntitaInizio")}</IDEntita> into (/TimeStamp)[1]')
				
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 					
				SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodioInizio")}</IDEpisodio> into (/TimeStamp)[1]')

								SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 					
				SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDPazienteInizio")}</IDPaziente> into (/TimeStamp)[1]')

								SET @xTimeStamp.modify('delete (/TimeStamp/IDTrasferimento)[1]') 					
				SET @xTimeStamp.modify('insert <IDTrasferimento>{sql:variable("@uIDTrasferimentoInizio")}</IDTrasferimento> into (/TimeStamp)[1]')
				
								SET @sInfoTimeStamp='Azione: BO_SbloccaScheda'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDScheda: ' + CONVERT(VARCHAR(50),@uIDEntitaInizio)  
				
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
								
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
					SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
									
								EXEC MSP_InsMovTimeStamp @xTimeStamp			
			
		END			
				SET @bErrore= ISNULL(@bErrore,0) 
				
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END