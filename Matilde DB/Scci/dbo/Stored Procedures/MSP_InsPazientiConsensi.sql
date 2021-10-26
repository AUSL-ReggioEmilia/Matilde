
CREATE PROCEDURE [dbo].[MSP_InsPazientiConsensi](@xParametri XML)
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
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sCodStatoConsensoCalcolato	AS VARCHAR(20)

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
				FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF ISNULL(@sGUID,'') <> '' 
		SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER, @sGUID)		
			
		SET @sCodTipoConsenso=(SELECT TOP 1 ValoreParametro.CodTipoConsenso.value('.','VARCHAR(20)')
							FROM @xParametri.nodes('/Parametri/CodTipoConsenso') as ValoreParametro(CodTipoConsenso))		
	
		SET @sCodSistemaProvenienza=(SELECT TOP 1 ValoreParametro.CodSistemaProvenienza.value('.','VARCHAR(50)')
									FROM @xParametri.nodes('/Parametri/CodSistemaProvenienza') as ValoreParametro(CodSistemaProvenienza))		
	
		SET @sIDProvenienza=(SELECT TOP 1 ValoreParametro.IDProvenienza.value('.','VARCHAR(100)')
							FROM @xParametri.nodes('/Parametri/IDProvenienza') as ValoreParametro(IDProvenienza))		

		SET @sCodStatoConsenso=(SELECT TOP 1 ValoreParametro.CodStatoConsenso.value('.','VARCHAR(20)')
							FROM @xParametri.nodes('/Parametri/CodStatoConsenso') as ValoreParametro(CodStatoConsenso))		

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataConsenso.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/DataConsenso') as ValoreParametro(DataConsenso))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	
	IF @sDataTmp<>'' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataConsenso=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataConsenso=NULL			
		END

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDisattivazione.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/DataDisattivazione') as ValoreParametro(DataDisattivazione))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataDisattivazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataDisattivazione=NULL			
		END

		SET @sCodOperatore=(SELECT TOP 1 ValoreParametro.CodOperatore.value('.','VARCHAR(255)')
						FROM @xParametri.nodes('/Parametri/CodOperatore') as ValoreParametro(CodOperatore))	

		SET @sCognomeOperatore=(SELECT TOP 1 ValoreParametro.CognomeOperatore.value('.','VARCHAR(255)')
							FROM @xParametri.nodes('/Parametri/CognomeOperatore') as ValoreParametro(CognomeOperatore))	
	SET @sCognomeOperatore=dbo.MF_TogliApici(@sCognomeOperatore)

		SET @sNomeOperatore=(SELECT TOP 1 ValoreParametro.NomeOperatore.value('.','VARCHAR(255)')
							FROM @xParametri.nodes('/Parametri/NomeOperatore') as ValoreParametro(NomeOperatore))	
	SET @sNomeOperatore=dbo.MF_TogliApici(@sNomeOperatore)
	
		SET @sComputerOperatore=(SELECT TOP 1 ValoreParametro.ComputerOperatore.value('.','VARCHAR(255)')
								FROM @xParametri.nodes('/Parametri/ComputerOperatore') as ValoreParametro(ComputerOperatore))	
								  	
			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
						
		SET @xTimeStampBase=@xTimeStamp

		SET @xTimeStamp.modify('delete (TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDPaziente")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (TimeStamp/Note)[1]') 			
	SET @xTimeStamp.modify('insert <Note>{sql:variable("@sCodTipoConsenso")}</Note> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (TimeStamp/IDPaziente)[1]') 						
	SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDPaziente")}</IDPaziente> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (Parametri/TimeStamp/CodEntita)[1]') 						
	SET @xTimeStamp.modify('insert <CodEntita>CNS</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (TimeStamp/CodAzione)[1]') 						
	SET @xTimeStamp.modify('insert <CodAzione>INS</CodAzione> into (/TimeStamp)[1]')

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')	
						
	BEGIN TRANSACTION
		
				INSERT INTO T_PazientiConsensi(
						IDPaziente
						,CodTipoConsenso
						,CodSistemaProvenienza
						,IDProvenienza
						,CodStatoConsenso
						,DataConsenso
						,DataDisattivazione
						,DataInserimento
						,DataInserimentoUTC
						,CodOperatore
						,CognomeOperatore
						,NomeOperatore
						,ComputerOperatore
				)
		VALUES
				( 					 
						@uIDPaziente
						,@sCodTipoConsenso
						,@sCodSistemaProvenienza
						,@sIDProvenienza
						,@sCodStatoConsenso
						,@dDataConsenso
						,@dDataDisattivazione
						,Getdate()												,GetUTCdate()											,@sCodOperatore
						,@sCognomeOperatore
						,@sNomeOperatore
						,@sComputerOperatore
				)				
print @@ERROR

	IF @@ERROR=0 
		BEGIN								
			EXEC MSP_InsMovTimeStamp @xTimeStamp										
		END	
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION					

															
						SET @sCodStatoConsensoCalcolato=dbo.MF_StatoConsensoCalcolato(@uIDPaziente)

						UPDATE T_Pazienti
			SET CodStatoConsensoCalcolato=@sCodStatoConsensoCalcolato
			WHERE ID=@uIDPaziente

			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT	IDPaziente
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
							,@sCodStatoConsensoCalcolato AS CodStatoConsensoCalcolato
					   FROM T_PazientiConsensi
					 WHERE IDPaziente=@uIDPaziente And CodTipoConsenso=@sCodTipoConsenso
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			
			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')							
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')						
			
			EXEC MSP_InsMovLog @xParLog			
							
						SELECT @uIDPaziente AS IDPaziente
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			SELECT NULL AS IDPaziente
		END	 
	
	RETURN 0
END