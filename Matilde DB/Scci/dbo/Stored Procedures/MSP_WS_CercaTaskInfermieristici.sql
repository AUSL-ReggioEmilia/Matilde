CREATE PROCEDURE [dbo].[MSP_WS_CercaTaskInfermieristici](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizioneTempi.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizioneTempi') as ValoreParametro(IDPrescrizioneTempi))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sCodTipoTaskInfermieristico=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))						 
	SET @sCodTipoTaskInfermieristico= LTRIM(RTRIM(ISNULL(@sCodTipoTaskInfermieristico,'')))	
			
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
				
		IF @sCodTipoTaskInfermieristico NOT IN ('')
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoTaskInfermieristico ='''+ @sCodTipoTaskInfermieristico + '''' 													
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
					M.ID AS IDDiarioClinico,	
					TRA.IDCartella,
					CAR.NumeroCartella,				
					M.DataEvento,				
					M.CodTipoTaskInfermieristico,
					T.Descrizione AS DescrizioneTipoTaskInfermieristico,
					M.IDGruppo,
					M.DataProgrammata,
					M.DataErogazione AS DataErogazioneAnnullamento,
					M.Note,
					M.PosologiaEffettiva,
					M.CodUtenteRilevazione,
					L.Descrizione AS DescrizioneUtenteRilevazione,
					M.CodUtenteUltimaModifica,
					LM.Descrizione AS DescrizioneUtenteUltimaModifica,
					M.DataUltimaModifica
					'	
	
	SET @sSQL=@sSQL + '
				FROM 					
						T_MovTaskInfermieristici	M																					
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_TipoTaskInfermieristico T
							ON (M.CodTipoTaskInfermieristico=T.Codice)						
							
						LEFT JOIN T_StatoTaskInfermieristico S
							ON (M.CodStatoTaskInfermieristico=S.Codice)	
																						
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)
														
						LEFT JOIN T_MovTrasferimenti TRA
							ON M.IDTrasferimento=TRA.ID	
						LEFT JOIN T_MovCartelle CAR
							ON TRA.IDCartella=CAR.ID				
						
					  '
	
	
	SET @sWhere= @sWhere + ' AND TRA.CodStatoTrasferimento <> ''CA'''
	
		IF @uIDCartella IS NOT NULL
		SET @sWhere= @sWhere + ' AND TRA.IDCartella =''' + CONVERT(VARCHAR(50),@uIDCartella) + ''''
	
		IF @uIDPrescrizioneTempi IS NOT NULL
	BEGIN		
		SET @sWhere= @sWhere + ' AND M.CodSistema=''PRF'' 
								 AND M.IDGruppo=''' + CONVERT(VARCHAR(50),@uIDPrescrizioneTempi) + '''
								'	
	END
			
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