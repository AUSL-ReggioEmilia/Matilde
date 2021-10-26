CREATE PROCEDURE [dbo].[MSP_WS_CercaPrescrizioni](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @sCodTipoPrescrizione AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	

	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodTipoPrescrizione=(SELECT	TOP 1 ValoreParametro.CodTipoPrescrizione.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoPrescrizione') as ValoreParametro(CodTipoPrescrizione))						 
	SET @sCodTipoPrescrizione= LTRIM(RTRIM(ISNULL(@sCodTipoPrescrizione,'')))	
	
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL		
		END 
		
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
					SET @sWhere=''
	
		IF @sCodTipoPrescrizione NOT IN ('')
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoPrescrizione ='''+ @sCodTipoPrescrizione + '''' 													
		END
		
					
		IF @dDataInizio IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
		END

		IF @dDataFine IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'												
		END	

			
	SET @sSQL='	SELECT
						M.ID AS IDPrescrizione,
						TRASF.IDCartella,					
						CAR.NumeroCartella,					
						M.DataEvento,												
						M.CodTipoPrescrizione,
						T.Descrizione AS DescrizioneTipoPrescrizione,
						M.CodStatoPrescrizione,
						S.Descrizione As DescrizioneStatoPrescrizione,
						ISNULL(M.CodStatoContinuazione,''AP'') AS CodStatoContinuazione,
						ISNULL(SC.Descrizione,''Aperta'') AS DescrizioneStatoContinuazione,
						M.CodViaSomministrazione,
						V.Descrizione as DescrizioneViaSomministazione,
						M.CodUtenteRilevazione,
						L.Descrizione AS DescrizioneUtenteRilevazione,
						M.DataUltimaModifica AS DataUltimaModifica,
						M.CodUtenteUltimaModifica,
						LM.Descrizione AS DescrizioneUtenteUltimaModifica
					'

	SET @sSQL=@sSQL + '
				FROM
						T_MovPrescrizioni M
						LEFT JOIN Q_SelPrescrizioniTask PRETASK							
							ON CONVERT(VARCHAR(50),M.ID)=PRETASK.IDPrescrizione
						LEFT JOIN T_MovTrasferimenti TRASF
								ON (M.IDTrasferimento=TRASF.ID)
						LEFT JOIN T_MovCartelle CAR
								ON (TRASF.IDCartella=CAR.ID)								
						LEFT JOIN
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede
								 WHERE CodEntita=''PRT'' AND
									Storicizzata=0
								) AS MS
							ON MS.IDEntita=M.ID
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)
						LEFT JOIN T_TipoPrescrizione T
							ON (M.CodTipoPrescrizione=T.Codice)
						LEFT JOIN T_StatoPrescrizione S
							ON (PRETASK.CodStatoPrescrizioneCalc=S.Codice)
						LEFT JOIN T_StatoContinuazione SC
							ON (M.CodStatoContinuazione=SC.Codice)	
						LEFT JOIN T_ViaSomministrazione V
							ON (M.CodViaSomministrazione=V.Codice)		
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)
					  '
					  
	SET @sWhere= @sWhere + ' AND M.CodStatoPrescrizione NOT IN (''CA'')
						     AND TRASF.IDCartella =''' + CONVERT(VARCHAR(50),@uIDCartella) + '''
							'
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	ELSE
		SET @sSQL=@sSQL +' WHERE 1=0'
	PRINT @sSQL					
	EXEC (@sSQL)

	RETURN 0
	
END