CREATE PROCEDURE [dbo].[MSP_BO_Elabora](@xParametri XML)
AS
BEGIN


	
		DECLARE @sCodOpe AS VARCHAR(50)	
	
	DECLARE @bErrore AS BIT	
	DECLARE @sInfo AS VARCHAR(MAX)
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER		

		IF @xParametri.exist('/Parametri/CodOpe')=1	
		BEGIN					 
			SET @sCodOpe=(SELECT TOP 1 ValoreParametro.CodOpe.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodOpe') as ValoreParametro(CodOpe))	
		END
			
		SET @bErrore=0
	SET @sInfo=''

				CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)
		
				
				IF @sCodOpe='SpostaSingoloParametriVitali' 
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaParametriVitali @xParametri,@bErrore OUTPUT				
	END		
				IF @sCodOpe='SpostaSingoloDiarioClinico'
	BEGIN					
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaDiarioClinico @xParametri,@bErrore OUTPUT				
	END		
	
				IF @sCodOpe='SpostaSingoloTaskInf'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaTaskInfermieristici @xParametri,@bErrore OUTPUT				
	END		
	
				IF @sCodOpe='SpostaSingoloAppPazRO'
	BEGIN						
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaAppuntamentoRo @xParametri,@bErrore OUTPUT					
	END		
				IF @sCodOpe='SpostaSingoloPrescrizione'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaPrescrizioni @xParametri,@bErrore OUTPUT				
	END	
	
				IF @sCodOpe='SpostaSingoloAppPazAmb'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaAppuntamentoAmb @xParametri,@bErrore OUTPUT				
	END		

	
				IF @sCodOpe='SpostaSingoloSchedaPaz'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaSchedaPaz @xParametri,@bErrore OUTPUT	
	END		

				IF @sCodOpe='SpostaSingoloSchedaEpi'
	BEGIN						
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaSchedaEpi @xParametri,@bErrore OUTPUT	
	END	
	
				IF @sCodOpe='SpostaSingoloEvidenzaClinica'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaEvidenzaClinica @xParametri,@bErrore OUTPUT	
	END	
	
				IF @sCodOpe='CancellaEvidenzaClinica'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_CancellaEvidenzaClinica @xParametri,@bErrore OUTPUT	
	END	
	
				IF @sCodOpe='SpostaSingoloAlertGenerico'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaAlertGenerico @xParametri,@bErrore OUTPUT	
	END	

				IF @sCodOpe='SpostaSingoloAppAmbToEpi'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaAppuntamentoAmbToEpi @xParametri,@bErrore OUTPUT	
	END	

				IF @sCodOpe='SpostaSingoloAppEpiToAmb'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SpostaAppuntamentoEpiToAmb @xParametri,@bErrore OUTPUT	
	END	
	
							IF @sCodOpe='SpostaGruppoPazAmb'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 				
		EXEC MSP_BO_SpostaGruppoAppuntamentoAmb @xParametri,@bErrore OUTPUT			
	END	
			
				IF @sCodOpe='SpostaGruppoSchedaPaz'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoSchedaPaz @xParametri,@bErrore OUTPUT			
	END	

				IF @sCodOpe='SpostaGruppoPazRO'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoAppuntamentoRO @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoDiarioClinico'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoDiarioClinico @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoPrescrizione'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoPrescrizioni @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoParametriVitali'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoParametriVitali @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoTaskInf'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoTaskInfermieristici @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoSchedaEpi'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
			 
		EXEC MSP_BO_SpostaGruppoSchedaEpi @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoEvidenzaClinica'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
					 
		EXEC MSP_BO_SpostaGruppoEvidenzaClinica @xParametri,@bErrore OUTPUT			
	END	
	
				IF @sCodOpe='SpostaGruppoAlertGenerico'
	BEGIN			
				SET @uIDSessione=NEWID()
		SET @xParametri.modify('delete (/IDSessione)[1]') 
		SET @xParametri.modify('insert <IDSessione>{sql:variable("@uIDSessione")}</IDSessione> into (/Parametri)[1]')	
					 
		EXEC MSP_BO_SpostaGruppoAlertGenerico @xParametri,@bErrore OUTPUT			
	END	
	

			
				IF @sCodOpe='SbloccoScheda'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_SbloccoScheda @xParametri,@bErrore OUTPUT	
	END	
	
				IF @sCodOpe='AnnullaCancellazioneScheda'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_AnnullaCancellazioneScheda @xParametri,@bErrore OUTPUT	
	END	


				IF @sCodOpe='RipristinoRevisioneScheda'
	BEGIN			
		INSERT INTO #tmpErrori 
			EXEC MSP_BO_RipristinoRevisioneScheda @xParametri,@bErrore OUTPUT	
	END	

				
				IF @sCodOpe='ModificaNumeroCartella'
	BEGIN			
	
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaCambiaNumero @xParametri,@bErrore OUTPUT
	END	

				IF @sCodOpe='CartellaCancella'
	BEGIN			
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaCancella @xParametri,@bErrore OUTPUT
	END	
	
				IF @sCodOpe='CartellaRiapri'
	BEGIN			
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaRiapri @xParametri,@bErrore OUTPUT
	END	
	
				IF @sCodOpe='CartellaScollega'
	BEGIN			
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaScollega @xParametri,@bErrore OUTPUT
	END	

			
				IF @sCodOpe='CartellaAmbulatorialeRiapri'
	BEGIN			
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaAmbulatorialeRiapri @xParametri,@bErrore OUTPUT
	END	

				IF @sCodOpe='ModificaNumeroCartellaAmbulatoriale'
	BEGIN			
		INSERT INTO #tmpErrori 
				EXEC MSP_BO_CartellaAmbulatorialeCambiaNumero @xParametri,@bErrore OUTPUT
	END	

				
	SET @bErrore=ISNULL(@bErrore,0)	
	IF ISNULL(@bErrore,0)=1
		BEGIN
			IF ISNULL(@sInfo,'')=''
				SET @sInfo='ERRORE : si sono verificati errori'
		END		
	ELSE
		BEGIN	
			IF ISNULL(@sInfo,'')=''
				SET @sInfo='OK : Elaborazione Terminata con successo'
		END
		
		SELECT 
		@bErrore AS Errore, 
		@sInfo AS DescrizioneErrore
	
		IF @uIDSessione IS NOT NULL
		BEGIN
			SELECT Errore
			FROM T_TmpBOErrori
			WHERE IDSessione=@uIDSessione
				
			DELETE FROM T_TmpBOErrori
			WHERE IDSessione=@uIDSessione
		END		
	ELSE	
		SELECT Errore  FROM #tmpErrori
	
	DROP TABLE #tmpErrori
	
END