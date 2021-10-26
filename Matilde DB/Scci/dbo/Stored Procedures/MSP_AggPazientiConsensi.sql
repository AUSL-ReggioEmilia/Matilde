CREATE PROCEDURE [dbo].[MSP_AggPazientiConsensi](@xParametri XML)
AS
BEGIN
		
	
				
	 	DECLARE @uIDPaziente			AS UNIQUEIDENTIFIER
	DECLARE @sCodTipoConsenso		AS VARCHAR(20)
	DECLARE @sCodSistemaProvenienza	AS VARCHAR(50)
	DECLARE @sIDProvenienza			AS VARCHAR(100)
	DECLARE @sCodStatoConsenso		AS VARCHAR(20)
	DECLARE @dDataConsenso			AS DateTime
	DECLARE @dDataDisattivazione	AS DateTime
	DECLARE @sCodOperatore			AS VARCHAR(255)
	DECLARE @sCognomeOperatore		AS VARCHAR(255)
	DECLARE @sNomeOperatore			AS VARCHAR(255)
	DECLARE @sComputerOperatore		AS VARCHAR(255)
				
		DECLARE @sCodStatoConsensoCalcolatoPrima	AS VARCHAR(20)
	DECLARE @sCodStatoConsensoCalcolatoDopo	AS VARCHAR(20)

	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sTmp AS VARCHAR(MAX)	
		
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML		
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	DECLARE @xParMovPaz AS XML

		SET @sSQL='UPDATE T_PazientiConsensi ' + CHAR(13) + CHAR(10) +
			  'SET '  				  									
	SET @sSET =''	
	
		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
			IF ISNULL(@sGUID,'') <> '' 
				SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER, @sGUID)
		END					
			
		IF @xParametri.exist('/Parametri/CodTipoConsenso')=1
		BEGIN
			SET @sCodTipoConsenso=(SELECT TOP 1 ValoreParametro.CodTipoConsenso.value('.','VARCHAR(20)')
									FROM @xParametri.nodes('/Parametri/CodTipoConsenso') as ValoreParametro(CodTipoConsenso))		
		END

		IF @xParametri.exist('/Parametri/CodSistemaProvenienza')=1
		BEGIN			
			SET @sCodSistemaProvenienza=(SELECT TOP 1 ValoreParametro.CodSistemaProvenienza.value('.','VARCHAR(50)')
											FROM @xParametri.nodes('/Parametri/CodSistemaProvenienza') as ValoreParametro(CodSistemaProvenienza))		
			IF @sCodSistemaProvenienza <> '' 
				SET	@sSET=@sSET + ',CodSistemaProvenienza=''' + @sCodSistemaProvenienza + ''''	+ CHAR(13) + CHAR(10)
			ELSE
				SET	@sSET=@sSET + ',CodSistemaProvenienza=NULL' + CHAR(13) + CHAR(10)			
		END	
	
		IF @xParametri.exist('/Parametri/IDProvenienza')=1
		BEGIN			
			SET @sIDProvenienza=(SELECT TOP 1 ValoreParametro.IDProvenienza.value('.','VARCHAR(100)')
											FROM @xParametri.nodes('/Parametri/IDProvenienza') as ValoreParametro(IDProvenienza))		
			IF @sIDProvenienza <> '' 
				SET	@sSET=@sSET + ',IDProvenienza=''' + @sIDProvenienza + ''''	+ CHAR(13) + CHAR(10)
			ELSE
				SET	@sSET=@sSET + ',IDProvenienza=NULL' + CHAR(13) + CHAR(10)			
		END	

		IF @xParametri.exist('/Parametri/CodStatoConsenso')=1
		BEGIN			
			SET @sCodStatoConsenso=(SELECT TOP 1 ValoreParametro.CodStatoConsenso.value('.','VARCHAR(20)')
											FROM @xParametri.nodes('/Parametri/CodStatoConsenso') as ValoreParametro(CodStatoConsenso))		
			IF @sCodStatoConsenso <> '' 
				SET	@sSET=@sSET + ',CodStatoConsenso=''' + @sCodStatoConsenso + ''''	+ CHAR(13) + CHAR(10)
			ELSE
				SET	@sSET=@sSET + ',CodStatoConsenso=NULL' + CHAR(13) + CHAR(10)			
		END	
					
		IF @xParametri.exist('/Parametri/DataConsenso')=1
		BEGIN	
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataConsenso.value('.','VARCHAR(20)')
							FROM @xParametri.nodes('/Parametri/DataConsenso') as ValoreParametro(DataConsenso))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')		
			IF @sDataTmp<> '' 
				BEGIN			
					IF LEN(@sDataTmp) = 10
					BEGIN
												SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
									+ LEFT(@sDataTmp,2) + '-' +
									+ SUBSTRING(@sDataTmp,7,4) 							
					END
					ELSE
					BEGIN
												SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) 	+
								  ' ' + RIGHT(@sDataTmp,5)						
			
					END

					IF ISDATE(@sDataTmp)=1
						SET	@dDataConsenso=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataConsenso=NULL			
				END
				IF @dDataConsenso IS NOT NULL		
					SET	@sSET= @sSET + ',DataConsenso=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataConsenso,120) + ''',120)' + CHAR(13) + CHAR(10)		
				ELSE
					SET	@sSET= @sSET + ',DataConsenso=NULL' + CHAR(13) + CHAR(10)		
		END
		ELSE
		BEGIN
						SET	@sSET= @sSET + ',DataConsenso=NULL' + CHAR(13) + CHAR(10)	
		END

		IF @xParametri.exist('/Parametri/DataDisattivazione')=1
		BEGIN	
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDisattivazione.value('.','VARCHAR(20)')
							FROM @xParametri.nodes('/Parametri/DataDisattivazione') as ValoreParametro(DataDisattivazione))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')		
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) 	+
								  ' ' + RIGHT(@sDataTmp,5)			
					IF ISDATE(@sDataTmp)=1
						SET	@dDataDisattivazione=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataDisattivazione=NULL			
				END
				IF @dDataDisattivazione IS NOT NULL		
					SET	@sSET= @sSET + ',DataDisattivazione=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataDisattivazione,120) + ''',120)' + CHAR(13) + CHAR(10)		
				ELSE
					SET	@sSET= @sSET + ',DataDisattivazione=NULL' + CHAR(13) + CHAR(10)		
		END
		ELSE
		BEGIN
						SET	@sSET= @sSET + ',DataDisattivazione=NULL' + CHAR(13) + CHAR(10)	
		END

		IF @xParametri.exist('/Parametri/CodOperatore')=1
		BEGIN	
			SET @sCodOperatore=(SELECT TOP 1 ValoreParametro.CodOperatore.value('.','VARCHAR(255)')
								FROM @xParametri.nodes('/Parametri/CodOperatore') as ValoreParametro(CodOperatore))				
			IF @sCodOperatore <> ''
				SET	@sSET= @sSET + ',CodOperatore=''' + @sCodOperatore + '''' + CHAR(13) + CHAR(10)		
			ELSE
				SET @sSET= @sSET + ',CodOperatore=NULL' + CHAR(13) + CHAR(10)			
		END	

		IF @xParametri.exist('/Parametri/CognomeOperatore')=1
	BEGIN	
		SET @sCognomeOperatore=(SELECT TOP 1 ValoreParametro.CognomeOperatore.value('.','VARCHAR(255)')
								FROM @xParametri.nodes('/Parametri/CognomeOperatore') as ValoreParametro(CognomeOperatore))				
		IF @sCognomeOperatore <> ''
			BEGIN				
				SET @sTmp=dbo.MF_SQLVarcharInsert(@sCognomeOperatore)				
				SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
				SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
				SET	@sSET= @sSET + ',CognomeOperatore=' + @sTmp + '' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CognomeOperatore=NULL' + CHAR(13) + CHAR(10)			
	END	
			
		IF @xParametri.exist('/Parametri/NomeOperatore')=1
	BEGIN	
		SET @sNomeOperatore=(SELECT TOP 1 ValoreParametro.NomeOperatore.value('.','VARCHAR(255)')
								FROM @xParametri.nodes('/Parametri/NomeOperatore') as ValoreParametro(NomeOperatore))				
		IF @sNomeOperatore <> ''
			BEGIN				
				SET @sTmp=dbo.MF_SQLVarcharInsert(@sNomeOperatore)				
				SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
				SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
				SET	@sSET= @sSET + ',NomeOperatore=' + @sTmp + '' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',NomeOperatore=NULL' + CHAR(13) + CHAR(10)			
	END	
			
		IF @xParametri.exist('/Parametri/ComputerOperatore')=1
		BEGIN	
			SET @sComputerOperatore=(SELECT TOP 1 ValoreParametro.ComputerOperatore.value('.','VARCHAR(255)')
										FROM @xParametri.nodes('/Parametri/ComputerOperatore') as ValoreParametro(ComputerOperatore))				
			IF @sComputerOperatore <> ''
				SET	@sSET= @sSET + ',ComputerOperatore=''' + @sComputerOperatore + '''' + CHAR(13) + CHAR(10)		
			ELSE
				SET @sSET= @sSET + ',ComputerOperatore=NULL' + CHAR(13) + CHAR(10)			
		END	

		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=GetUTCdate() ' + CHAR(13) + CHAR(10)	
			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))				
					
				
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
					
						SET @sWHERE=' WHERE IDPaziente=''' + convert(varchar(50),@uIDPaziente) + ''' AND CodTipoConsenso=''' + @sCodTipoConsenso + ''''
				
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')			

												
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDPaziente")}</IDEntita> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDPaziente")}</IDPaziente> into (/TimeStamp)[1]')

						SET @xTimeStamp.modify('delete (TimeStamp/Note)[1]') 			
			SET @xTimeStamp.modify('insert <Note>{sql:variable("@sCodTipoConsenso")}</Note> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
			SET @xTimeStamp.modify('insert <CodEntita>CNS</CodEntita> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
			SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
	
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
			
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')							
										
						SET @sCodStatoConsensoCalcolatoPrima=dbo.MF_StatoConsensoCalcolato(@uIDPaziente)

			SET @xTemp=
				(SELECT * FROM 
					(SELECT IDPaziente
							,IDNum														  
							,CodTipoConsenso
							,CodSistemaProvenienza
							,IDProvenienza
							,CodStatoConsenso
							,DataConsenso					
							,DataDisattivazione
							,DataInserimento
							,DataInserimentoUTC
							,DataUltimaModifica
							,DataUltimaModificaUTC
							,CodOperatore
							,CognomeOperatore
							,NomeOperatore
							,ComputerOperatore
							,@sCodStatoConsensoCalcolatoPrima AS CodStatoConsensoCalcolato
					 FROM T_PazientiConsensi
					 WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso=@sCodTipoConsenso												) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																		
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				
			BEGIN TRANSACTION
												EXEC (@sSQL)
								
			IF @@ERROR=0 AND @@ROWCOUNT > 0
				BEGIN
										EXEC MSP_InsMovTimeStamp @xTimeStamp		
				END	
	
			IF @@ERROR = 0
				BEGIN
					COMMIT TRANSACTION
					
										SET @sCodStatoConsensoCalcolatoDopo=dbo.MF_StatoConsensoCalcolato(@uIDPaziente)

										UPDATE T_Pazienti
					SET CodStatoConsensoCalcolato=@sCodStatoConsensoCalcolatoDopo
					WHERE ID=@uIDPaziente

																				
					SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
					
					SET @xTemp=
						(SELECT * FROM 
							(SELECT
								 IDPaziente
								,IDNum														  
								,CodTipoConsenso
								,CodSistemaProvenienza
								,IDProvenienza
								,CodStatoConsenso
								,DataConsenso					
								,DataDisattivazione
								,DataInserimento
								,DataInserimentoUTC
								,DataUltimaModifica
								,DataUltimaModificaUTC
								,CodOperatore
								,CognomeOperatore
								,NomeOperatore
								,ComputerOperatore
								,@sCodStatoConsensoCalcolatoDopo AS CodStatoConsensoCalcolato
							FROM T_PazientiConsensi
							WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso=@sCodTipoConsenso											) AS [Table]
						FOR XML AUTO, ELEMENTS)

					SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
					SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
					
										SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
										EXEC MSP_InsMovLog @xParLog

				
					
					END	
			ELSE
				BEGIN
					ROLLBACK TRANSACTION						
				END	 
	END		
	
	RETURN 0
END