CREATE PROCEDURE [dbo].[MSP_AggMovPazientiDaAnagra](@xParametri XML)
AS
BEGIN
		
	
			
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER			
	DECLARE @uID AS UNIQUEIDENTIFIER					DECLARE @binFoto AS VARBINARY(MAX)					DECLARE @txtFoto AS VARCHAR(MAX)					
		DECLARE @sGUID AS VARCHAR(50)
	
	    DECLARE @xTimeStampTemp AS XML	
	DECLARE @xTimeStamp AS XML	
			
		DECLARE @xTemp AS XML
	DECLARE @xOut AS XML
	DECLARE @xFoto AS XML


				
		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN
			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
			IF 	ISNULL(@sGUID,'') <> ''				
					SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)											
							END
				
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
 
		
				
	
		SET @binFoto=(SELECT Foto FROM T_Pazienti WHERE ID=@uIDPaziente)
	IF @binFoto IS NOT NULL 
		BEGIN
			SET @txtFoto = CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binFoto"))', 'varchar(max)')
			SET @txtFoto ='<Foto>'+@txtFoto+'</Foto>'
			SET @xFoto=CONVERT(XML,	@txtFoto)		
		END
	
		DECLARE  curMov CURSOR
		FOR 
			SELECT MP.ID
			FROM T_MovPazienti MP
				INNER JOIN T_MovEpisodi  ME
					ON MP.IDEpisodio=ME.ID
			WHERE 
				ME.DataDimissione IS NULL AND								MP.IDPaziente=@uIDPaziente
		
		
	OPEN curMov
	FETCH NEXT FROM curMov	
		INTO @uId
				
	WHILE @@FETCH_STATUS = 0
	BEGIN
								
				
				SET @xOut='<Parametri></Parametri>'	
	
				SET @xTemp=
		(SELECT * FROM 
							(SELECT 
							  @uId AS ID
							  							  							  							  ,CodSAC
							  ,Cognome
							  ,Nome
							  ,Sesso
							  ,CASE 
									WHEN ISNULL(DataNascita,'') ='' THEN ''
									ELSE Convert(varchar(10),DataNascita,105)	 
							   END AS	DataNascita															  ,CodiceFiscale
							  ,CodComuneNascita
							  ,ComuneNascita
							  ,CodProvinciaNascita
							  ,LocalitaNascita
							  ,CAPDomicilio
							  ,CodComuneDomicilio
							  ,ComuneDomicilio
							  ,IndirizzoDomicilio
							  ,LocalitaDomicilio
							  ,CodProvinciaDomicilio
							  ,ProvinciaDomicilio
							  ,CodRegioneDomicilio
							  ,RegioneDomicilio
							  ,CAPResidenza
							  ,CodComuneResidenza
							  ,ComuneResidenza
							  ,IndirizzoResidenza
							  ,LocalitaResidenza
							  ,CodProvinciaResidenza
							  ,ProvinciaResidenza
							  ,CodRegioneResidenza
							  ,RegioneResidenza 
							   ,CodMedicoBase
							  ,CodFiscMedicoBase
							  ,CognomeNomeMedicoBase
							  ,DistrettoMedicoBase
							  ,DataSceltaMedicoBase
							  ,ElencoEsenzioni
							  ,DataDecesso
							 FROM T_Pazienti
							 WHERE ID=@uIDPaziente											) AS [Table]
						FOR XML AUTO, ELEMENTS)
						
		 SET @xTemp=(SELECT TOP 1 ValoreParametro.Figli.query('./*')
						  FROM @xTemp.nodes('/Table') as ValoreParametro(Figli))	
						  
		 		 SET @xOut.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')		
		 
		 		 SET @xOut.modify('insert sql:variable("@xFoto") as last into (/Parametri)[1]')		
		 
		 		 
		 								SET @xTimeStampTemp=@xTimeStamp
		
				SET @xTimeStampTemp.modify('delete (/TimeStamp/IDEntita)[1]') 					
		SET @xTimeStampTemp.modify('insert <IDEntita>{sql:variable("@uID")}</IDEntita> into (/TimeStamp)[1]')
		
				SET @xTimeStampTemp.modify('delete (/TimeStamp/CodEntita)[1]') 					
		SET @xTimeStampTemp.modify('insert <CodEntita>PAZ</CodEntita> into (/TimeStamp)[1]')

				SET @xOut.modify('insert sql:variable("@xTimeStampTemp") as last into (/Parametri)[1]')	
					
				
						EXEC MSP_AggMovPazienti @xOut
		FETCH NEXT FROM curMov	
		INTO @uId
	END
	
	CLOSE curMov
	DEALLOCATE curMov
	RETURN 0
END