CREATE PROCEDURE [dbo].[MSP_SelUltimaPosologia](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
		DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	DECLARE @DataEvento  AS DATETIME
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sPosologia AS VARCHAR(2000)
	DECLARE @sIDPrescrizioneTempi AS VARCHAR(50)
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER
	DECLARE @sCodTipoTaskDaPrescrizione AS VARCHAR(20)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)


		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEvento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEvento') as ValoreParametro(DataEvento))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@DataEvento =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@DataEvento  =NULL			
		END

	
				
	SET @sPosologia=''
	
		SET @sCodTipoTaskDaPrescrizione =(SELECT TOP 1 Valore FROM T_Config WHERE ID=33)
	
	SET @sIDPrescrizioneTempi =(SELECT 
									  TOP 1 
									  CONVERT(VARCHAR(50),IDGruppo) AS IDPrescrizioneTempi
								FROM T_MovTaskInfermieristici
								WHERE CodSistema='PRF'
									  AND CodStatoTaskInfermieristico NOT IN ('CA','AN')
									  AND CodTipoTaskInfermieristico=@sCodTipoTaskDaPrescrizione
									  AND IDSistema=CONVERT(VARCHAR(50),@uIDPrescrizione) 
									  AND  ISNULL(DataErogazione,DataProgrammata) <= @DataEvento 
								ORDER BY
									  ISNULL(DataErogazione,DataProgrammata)  DESC
								)	 		

		IF ISNULL(@sIDPrescrizioneTempi,'') <> '' 
		BEGIN
			SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,@sIDPrescrizioneTempi)
			SET @sPosologia=(SELECT TOP 1 Posologia 
							 FROM T_MovPrescrizioniTempi 
							 WHERE ID=@uIDPrescrizioneTempi
							)
							
		END	
		
	SELECT ISNULL(@sPosologia,'') AS Posologia
	
	
	RETURN 0
			
END