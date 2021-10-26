CREATE PROCEDURE [dbo].[MSP_AggMovPazientiDaPazienti_Entita](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteNuovo AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteVecchio AS UNIQUEIDENTIFIER			
 	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @uIDMov AS UNIQUEIDENTIFIER	
	DECLARE @sCodLogin AS VARCHAR(100)

	    DECLARE @xTimeStampBase AS XML		
	DECLARE @xTimeStamp AS XML
	DECLARE @xParametriOperazione AS XML
	
	
		
	

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPazienteNuovo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPazienteNuovo') as ValoreParametro(IDPazienteNuovo))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPazienteNuovo=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPazienteVecchio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPazienteVecchio') as ValoreParametro(IDPazienteVecchio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPazienteVecchio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
			
					
	IF @uIDEpisodio IS NOT NULL AND @uIDPazienteNuovo IS NOT NULL AND @uIDPazienteVecchio IS NOT NULL
	BEGIN

		SET @uIDMov = NULL
		DECLARE curAllegati CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovAllegati
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio
																																																							
		OPEN curAllegati
		
		FETCH NEXT FROM curAllegati INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>ALL</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
						
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovAllegati
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN				
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END
			FETCH NEXT FROM curAllegati INTO @uIDMov
		END

		CLOSE curAllegati
		DEALLOCATE curAllegati
				
						
		SET @uIDMov = NULL
		DECLARE curAppuntamenti CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovAppuntamenti
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio
																																																							
		OPEN curAppuntamenti
		
		FETCH NEXT FROM curAppuntamenti INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>APP</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
						
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovAppuntamenti
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN			
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END

				DECLARE @uIDMovSchedaApp AS UNIQUEIDENTIFIER

				DECLARE curSchedeApp CURSOR LOCAL READ_ONLY  FOR 
					SELECT   ID
						  FROM T_MovSchede
						  WHERE 
							IDEpisodio=@uIDEpisodio AND
							IDPaziente=@uIDPazienteVecchio AND
							IDEntita = @uIDMov AND
							CodEntita='APP'
																																																							
				OPEN curSchedeApp
		
				FETCH NEXT FROM curSchedeApp INTO @uIDMovSchedaApp
		
				WHILE (@@FETCH_STATUS = 0)
				BEGIN			

								
					SET @xTimeStampBase = @xTimeStamp		

										SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
					SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMovSchedaApp")}</IDEntita> into (/TimeStamp)[1]')

										SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
					SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

										SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
					SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

										SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
					SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


										SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
					SET @xTimeStampBase.modify('insert <CodEntita>SCH</CodEntita> into (/TimeStamp)[1]')
		
										SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
					SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
					SET @xParametriOperazione=@xTimeStampBase				
			
					SET @xParametriOperazione=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
										'</Parametri>')
			
										UPDATE T_MovSchede
					SET 
						IDPaziente=@uIDPazienteNuovo
					WHERE ID=@uIDMovSchedaApp
					
					IF @@ROWCOUNT > 0
					BEGIN						
						EXEC MSP_InsMovTimeStamp @xParametriOperazione
					END
					FETCH NEXT FROM curSchedeApp INTO @uIDMovSchedaApp
				END

				CLOSE curSchedeApp
				DEALLOCATE curSchedeApp
			
			FETCH NEXT FROM curAppuntamenti INTO @uIDMov
		END
		
		CLOSE curAppuntamenti
		DEALLOCATE curAppuntamenti

						
		SET @uIDMov = NULL
		DECLARE curNote CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovNote
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio
																																																							
		OPEN curNote
		
		FETCH NEXT FROM curNote INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>NTG</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
						
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovNote
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN				
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END
			FETCH NEXT FROM curNote INTO @uIDMov
		END

		CLOSE curNote
		DEALLOCATE curNote

						
		SET @uIDMov = NULL
		DECLARE cudOrdini CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovOrdini
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio
																																																							
		OPEN cudOrdini
		
		FETCH NEXT FROM cudOrdini INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>OE</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
						
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovOrdini
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN				
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END
			FETCH NEXT FROM cudOrdini INTO @uIDMov
		END

		CLOSE cudOrdini
		DEALLOCATE cudOrdini

						
		SET @uIDMov = NULL
		DECLARE curReport CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovReport
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio
																																																							
		OPEN curReport
		
		FETCH NEXT FROM curReport INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>RPT</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
						
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovReport
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN				
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END
			FETCH NEXT FROM curReport INTO @uIDMov
		END

		CLOSE curReport
		DEALLOCATE curReport

						
		SET @uIDMov = NULL
		DECLARE curSchede CURSOR LOCAL READ_ONLY  FOR 
				SELECT   ID
					  FROM T_MovSchede
					  WHERE 
						IDEpisodio=@uIDEpisodio AND
						IDPaziente=@uIDPazienteVecchio AND
						CodEntita='EPI'
																																																							
		OPEN curSchede
		
		FETCH NEXT FROM curSchede INTO @uIDMov
		
		WHILE (@@FETCH_STATUS = 0)
		BEGIN			

						
			SET @xTimeStampBase = @xTimeStamp		

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDMov")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStampBase.modify('insert <IDPaziente>{sql:variable("@uIDPazienteNuovo")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/IDEpisodio)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> into (/TimeStamp)[1]')

						SET @xTimeStampBase.modify('delete (/TimeStamp/Note)[1]') 						
			SET @xTimeStampBase.modify('insert <Note>Operazione di Merge ADT, IDPaziente Vecchio:{sql:variable("@uIDPazienteVecchio")}</Note> into (/TimeStamp)[1]')


						SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>SCH</CodEntita> into (/TimeStamp)[1]')
		
						SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
			SET @xParametriOperazione=@xTimeStampBase				
			
			SET @xParametriOperazione=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xParametriOperazione) +
								'</Parametri>')
			
						UPDATE T_MovSchede
			SET 
				IDPaziente=@uIDPazienteNuovo
			WHERE ID=@uIDMov
					
			IF @@ROWCOUNT > 0
			BEGIN				
				EXEC MSP_InsMovTimeStamp @xParametriOperazione
			END
			FETCH NEXT FROM curSchede INTO @uIDMov
		END

		CLOSE curSchede
		DEALLOCATE curSchede
	END
	RETURN 0
END