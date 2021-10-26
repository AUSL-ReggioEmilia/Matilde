CREATE PROCEDURE [dbo].[MSP_WS_CercaPrescrizioniTempiTask](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	

	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizioneTempi.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizioneTempi') as ValoreParametro(IDPrescrizioneTempi))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		
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
	

					
		IF @dDataInizio IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
		END

		IF @dDataFine IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'												
		END	

			
	SET @sSQL='	SELECT
						M.ID AS IDPrescrizioneTempiTempi,
						M.IDPrescrizioneTempi AS IDPrescrizioneTempi,
						M.DataEvento,												
						M.CodTipoPrescrizioneTempi,
						T.Descrizione AS DescrizioneTipoPrescrizioneTempi,
						M.CodStatoPrescrizioneTempi,
						S.Descrizione As DescrizioneStatoPrescrizioneTempi,
						
						M.Posologia,				
						M.DataOraInizio,
						M.DataOraFine,
						M.AlBisogno,
						M.Durata,
						M.Continuita,
						M.PeriodicitaGiorni,
						M.PeriodicitaOre,
						M.PeriodicitaMinuti,
						M.CodProtocollo,
						TP.Descrizione AS DescrizioneProtocollo,
						M.Manuale,
						M.TempiManuali,
						
						M.CodUtenteRilevazione,
						L.Descrizione AS DescrizioneUtenteRilevazione,
						M.DataUltimaModifica AS DataUltimaModifica,
						M.CodUtenteUltimaModifica,
						LM.Descrizione AS DescrizioneUtenteUltimaModifica
					'

	SET @sSQL=@sSQL + '
				FROM
						T_MovPrescrizioniTempi M
						LEFT JOIN Q_SelPrescrizioniTempiTask PRETMPTASK							
							ON PRETMPTASK.IDPrescrizioneTempiTempi = CONVERT(VARCHAR(50),M.ID)
											
						LEFT JOIN
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede
								 WHERE CodEntita=''PRT'' AND
									Storicizzata=0
								) AS MS
							ON MS.IDEntita=M.ID					
						LEFT JOIN T_TipoPrescrizioneTempi T
							ON (M.CodTipoPrescrizioneTempi=T.Codice)
					
						LEFT JOIN T_StatoPrescrizioneTempi S
							ON (M.CodStatoPrescrizioneTempi=S.Codice)
						
						LEFT JOIN T_Protocolli TP
							ON (M.codProtocollo=TP.Codice)	
					
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)
					  '
					  
	SET @sWhere= @sWhere + ' AND M.CodStatoPrescrizioneTempi NOT IN (''CA'')
						     AND M.IDPrescrizioneTempi =''' + CONVERT(VARCHAR(50),@uIDPrescrizioneTempi) + '''
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