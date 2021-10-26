CREATE PROCEDURE [dbo].[MSP_BO_SpostaAppuntamentoRo](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN



              DECLARE @sCodUAInizio AS VARCHAR(20)
       DECLARE @sNumeroCartellaInizio AS VARCHAR(50) 
       DECLARE @sCodUAFine AS VARCHAR(20)
       DECLARE @sNumeroCartellaFine AS VARCHAR(50)           
       DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER   
       DECLARE @xTimeStamp AS XML
       
              DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER
       DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER
       DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER
       DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER   
       DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER
       DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER          
       DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER
       DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER     
       DECLARE @sCodRuolo AS VARCHAR(20)
       
       DECLARE @sGUID AS VARCHAR(50)
       DECLARE @nQTA AS INTEGER
       DECLARE @nRecord AS INTEGER       
       DECLARE @sInfoTimeStamp AS VARCHAR(255) 
       DECLARE @nNumTransazioni AS INTEGER
       
       
              IF @xParametri.exist('/Parametri/CodUAInizio')=1
             BEGIN
                    SET @sCodUAInizio=(SELECT  TOP 1 ValoreParametro.CodUAInizio.value('.','VARCHAR(20)')
                                        FROM @xParametri.nodes('/Parametri/CodUAInizio') as ValoreParametro(CodUAInizio))     
             END
       
       IF @xParametri.exist('/Parametri/NumeroCartellaInizio')=1   
             BEGIN
                    SET @sNumeroCartellaInizio=(SELECT      TOP 1 ValoreParametro.NumeroCartellaInizio.value('.','VARCHAR(50)')
                                        FROM @xParametri.nodes('/Parametri/NumeroCartellaInizio') as ValoreParametro(NumeroCartellaInizio))      
             END
       
       IF @xParametri.exist('/Parametri/CodUAFine')=1 
             BEGIN  
                    SET @sCodUAFine=(SELECT    TOP 1 ValoreParametro.CodUAFine.value('.','VARCHAR(20)')
                                        FROM @xParametri.nodes('/Parametri/CodUAFine') as ValoreParametro(CodUAFine))       
             END
             
       IF @xParametri.exist('/Parametri/NumeroCartellaFine')=1     
             BEGIN  
             SET @sNumeroCartellaFine=(SELECT TOP 1 ValoreParametro.NumeroCartellaFine.value('.','VARCHAR(50)')
                                        FROM @xParametri.nodes('/Parametri/NumeroCartellaFine') as ValoreParametro(NumeroCartellaFine))    
             END                                                  
             
       IF @xParametri.exist('/Parametri/IDEntitaInizio')=1   
             BEGIN                             
                    SET @sGUID=(SELECT  TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
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
             
       CREATE TABLE #tmpTrasferimenti
       (            
             IDTrasferimento  UNIQUEIDENTIFIER
       )      

                            IF @sCodUAInizio IS NOT NULL AND @sNumeroCartellaInizio IS NOT NULL
       BEGIN

                          SELECT @uIDCartellaInizio=IDCartella,
                      @nQta=COUNT(*)
             FROM T_MovTrasferimenti
             WHERE IDCartella 
                    IN (SELECT ID FROM T_MovCartelle
                           WHERE 
                           NumeroCartella=@sNumeroCartellaInizio
                           )
                    AND T_MovTrasferimenti.CodUA=@sCodUAInizio
             GROUP BY IDCartella
             
             IF @nQta =0 
                    BEGIN               
                           INSERT INTO #tmpErrori(Errore)
                                        VALUES('(GG001) ERRORE : nessuna cartella di origine trovata')
                           SET @bErrore=1
                    END
             ELSE
                    BEGIN                      
                                                      SELECT TOP 1                            
                                  @uIDEpisodioInizio=IDEpisodio
                           FROM T_MovTrasferimenti 
                           WHERE IDCartella=@uIDCartellaInizio
                                  
                                                      INSERT INTO #tmpTrasferimenti(IDTrasferimento)
                           SELECT ID
                           FROM T_MovTrasferimenti 
                           WHERE IDCartella=@uIDCartellaInizio                                  
                                                      
                           SET @uIDPazienteInizio=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioInizio)
                           
                                                                                 SET @uIDTrasferimentoInizio=(SELECT IDTrasferimento FROM T_MovAppuntamenti WHERE ID=@uIDEntitaInizio)                                                                           
                           
                                                      SET @nQTA=(SELECT COUNT(*) FROM #tmpTrasferimenti WHERE IDTrasferimento=@uIDTrasferimentoInizio)                    
                           IF @nQTA<1 
                                  BEGIN
                                        INSERT INTO #tmpErrori(Errore)
                                        VALUES('(GG002) ERRORE : trasferimento associato non di competenza della cartella')
                                        SET @bErrore=1
                                  END                                                                         
                    END
                    
       END

								IF @sCodUAFine IS NOT NULL AND @sNumeroCartellaFine IS NOT NULL
		BEGIN
		
						SELECT @uIDCartellaFine=IDCartella,
					@nQta=COUNT(*)
			FROM T_MovTrasferimenti
			WHERE IDCartella 
				IN (SELECT ID FROM T_MovCartelle
					WHERE 
					NumeroCartella=@sNumeroCartellaFine
					)
				AND T_MovTrasferimenti.CodUA=@sCodUAFine
				AND T_MovTrasferimenti.CodStatoTrasferimento <> 'CA'								GROUP BY IDCartella
				
			IF  @nQta =0 OR @uIDCartellaFine IS NULL									BEGIN							
					INSERT INTO #tmpErrori(Errore)
							VALUES('(GG003) ERRORE : nessuna cartella di destinazione trovata')
					SET @bErrore=1
				END
			ELSE		
				BEGIN			
					
										SELECT TOP 1 
						@uIDTrasferimentoFine=ID,
						@uIDEpisodioFine=IDEpisodio
					FROM T_MovTrasferimenti 
					WHERE 
						IDCartella=@uIDCartellaFine AND
						DataUscita IS NULL					
				
										IF @uIDTrasferimentoFine IS NULL OR @uIDEpisodioFine IS NULL
					BEGIN
						SELECT TOP 1 
							@uIDTrasferimentoFine=ID,
							@uIDEpisodioFine=IDEpisodio
						FROM T_MovTrasferimenti 
						WHERE 
							IDCartella=@uIDCartellaFine AND
							CodStatoTrasferimento='DM'
					END
										SET @uIDPazienteFine=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioFine)				
				END			
		END	

				SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)       

                     				IF (@uIDEpisodioFine IS NULL OR @uIDTrasferimentoFine IS NULL)
		BEGIN
			SELECT @uIDCartellaFine
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
				VALUES('(GG004) ERRORE IDEpisodio o IDTrasferimento di destinazione nulli')	
		END
		ELSE
		BEGIN
	        		   IF @uIDEntitaInizio IS NULL 
				 BEGIN
						SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
							   VALUES('(APPRO001) IDTaskAppuntamento NON specificato')
                    
				 END
		   ELSE
		   BEGIN                                                 
				 				 SET @nRecord=(SELECT COUNT(*) FROM T_MovAppuntamenti
										WHERE ID=@uIDEntitaInizio AND
												   IDPaziente=@uIDPazienteInizio AND              												   IDEpisodio=@uIDEpisodioInizio AND
												   IDTrasferimento=@uIDTrasferimentoInizio
									  )           
             
				 				 IF @nRecord =0
						BEGIN
							   							   SET @bErrore=1
							   INSERT INTO #tmpErrori(Errore)
											VALUES('(APPRO002) ERRORE Nessun Appuntamento da aggiornare') 
						END
				 ELSE
				 BEGIN  
												SET @nNumTransazioni=@@TRANCOUNT
                    
						BEGIN TRANSACTION                 
                    
							   							   UPDATE  T_MovAppuntamenti
							   SET 
									  IDPaziente=@uIDPazienteFine,                                									  IDEpisodio=@uIDEpisodioFine,
									  IDTrasferimento=@uIDTrasferimentoFine
							   WHERE ID=@uIDEntitaInizio
                           
							   IF @@ERROR=0
									  BEGIN
																						UPDATE  T_MovSchede
											SET                 
												   IDPaziente=@uIDPazienteFine,                   												   IDEpisodio=@uIDEpisodioFine,
												   IDTrasferimento=@uIDTrasferimentoFine
											WHERE IDEntita=@uIDEntitaInizio AND
													 CodEntita='APP'                                                                                                                                                                                  
                                                                                 
											IF @@ERROR=0                                                       
														  COMMIT                                                      
											ELSE
												   BEGIN                                          
														  														  IF @nNumTransazioni=0 ROLLBACK TRANSACTION
														  SET @bErrore=1
														  INSERT INTO #tmpErrori(Errore)
																VALUES('(APPRO003) ERRORE durante aggiornamento della scheda del APP')
												   END    
									  END            
							   ELSE
									  BEGIN                             
											ROLLBACK
											SET @bErrore=1
											INSERT INTO #tmpErrori(Errore)
																VALUES('(APPRO004) ERRORE durante aggiornamento del APP')
									  END    
						
												IF @bErrore=0 
						BEGIN               
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
                           
							   							   SET @sInfoTimeStamp='Azione: BO_SpostaAPPRo'
							   SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaInizio: ' + CONVERT(VARCHAR(50),@uIDCartellaInizio)  
							   SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDPazienteFine: ' + + CONVERT(VARCHAR(50),@uIDPazienteFine)                              
							   SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDCartellaFine:' + CONVERT(VARCHAR(50),@uIDCartellaFine)                 
							   SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]')              
							   SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
                                                      
							   							   SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]')         
									  SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
                    
							   SET @xTimeStamp=CONVERT(XML,
																'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
																'</Parametri>')
                                                            
							   							   EXEC MSP_InsMovTimeStamp @xTimeStamp                               
						END 				 END    		   END
       END      

                            SET @bErrore= ISNULL(@bErrore,0) 
                    
       SELECT Errore FROM #tmpErrori     
       
              DROP TABLE #tmpErrori
       DROP TABLE #tmpTrasferimenti
END