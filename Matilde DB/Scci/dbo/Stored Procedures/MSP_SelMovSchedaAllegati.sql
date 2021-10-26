CREATE PROCEDURE [dbo].[MSP_SelMovSchedaAllegati](@xParametri XML)
AS
BEGIN

		
	
				
	DECLARE @uIDSchedaAllegato AS UNIQUEIDENTIFIER
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoAllegatoScheda AS VARCHAR(1800)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sTipoRichiesta AS VARCHAR(1800)
	
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
	DECLARE @xTmpTS AS XML
	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaAllegato.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaAllegato') as ValoreParametro(IDSchedaAllegato))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaAllegato=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
					  
		SET @sCodStatoAllegatoScheda=''
	SELECT	@sCodStatoAllegatoScheda =  @sCodStatoAllegatoScheda +
														CASE 
								WHEN @sCodStatoAllegatoScheda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoAllegatoScheda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoAllegatoScheda') as ValoreParametro(CodStatoAllegatoScheda)
						 
	SET @sCodStatoAllegatoScheda=LTRIM(RTRIM(@sCodStatoAllegatoScheda))
	IF	(@sCodStatoAllegatoScheda='' OR @sCodStatoAllegatoScheda='''''') SET @sCodStatoAllegatoScheda='''IC'''
	SET @sCodStatoAllegatoScheda=UPPER(@sCodStatoAllegatoScheda)
	
		
		SET @sTipoRichiesta=(SELECT TOP 1 ValoreParametro.TipoRichiesta.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TipoRichiesta') as ValoreParametro(TipoRichiesta))
	
							  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


						
												
			
	IF ISNULL(@sTipoRichiesta,'')=''
	BEGIN
		SET @sSQL='	SELECT 
						MSA.*
						, IsNull(TA.Descrizione, '''') AS TipoAllegatoScheda
						, IsNull(SA.Descrizione, '''') AS StatoAllegatoScheda
						, IsNull(SA.Colore, '''') AS ColoreStatoAllegatoScheda'
						
			 						
	END
	
	
	IF ISNULL(@sTipoRichiesta,'')='LISTA'
	BEGIN
			SET @sSQL='	SELECT 
						  ID
						  ,IDNum
						  ,IDScheda
						  ,CodCampo
						  ,CodSezione
						  ,IsNull(Sequenza,1) As Sequenza
						  ,NULL Anteprima
						  ,NULL AS Documento
						  ,NomeFile
						  ,Estensione
						  ,DescrizioneAllegato
						  ,DescrizioneCampo
						  ,CodTipoAllegatoScheda
						  ,CodStatoAllegatoScheda
						  ,DataEvento
						  ,DataEventoUTC
						  ,CodUtenteRilevazione
						  ,CodUtenteUltimaModifica
						  ,DataRilevazione
						  ,DataRilevazioneUTC
						  ,DataUltimaModifica
						  ,DataUltimaModificaUTC
						  ,IsNull(TA.Descrizione, '''') AS TipoAllegatoScheda
						  ,IsNull(SA.Descrizione, '''') AS StatoAllegatoScheda
						  ,IsNull(SA.Colore, '''') AS ColoreStatoAllegatoScheda
						'
	END
		  
	IF ISNULL(@sTipoRichiesta,'')='COUNT'
	BEGIN
			SET @sSQL='	SELECT 
							COUNT(*) AS Qta
					  '		
	END
	
	IF ISNULL(@sTipoRichiesta,'')='THUMB'
	BEGIN
		SET @sSQL='	SELECT 
						  ID
						  ,IDNum
						  ,IDScheda
						  ,CodCampo
						  ,CodSezione
						  ,IsNull(Sequenza,1) As Sequenza
						  ,Anteprima
						  ,NULL AS Documento
						  ,NomeFile
						  ,Estensione
						  ,DescrizioneAllegato
						  ,DescrizioneCampo
						  ,CodTipoAllegatoScheda
						  ,CodStatoAllegatoScheda
						  ,DataEvento
						  ,DataEventoUTC
						  ,CodUtenteRilevazione
						  ,CodUtenteUltimaModifica
						  ,DataRilevazione
						  ,DataRilevazioneUTC
						  ,DataUltimaModifica
						  ,DataUltimaModificaUTC
						  ,IsNull(TA.Descrizione, '''') AS TipoAllegatoScheda
						  ,IsNull(SA.Descrizione, '''') AS StatoAllegatoScheda
						  ,IsNull(SA.Colore, '''') AS ColoreStatoAllegatoScheda
				'		  
			 						
	END
	
	IF ISNULL(@sTipoRichiesta,'')='DOC'
	BEGIN
			SET @sSQL='	SELECT 
						  ID
						  ,IDNum
						  ,IDScheda
						  ,CodCampo
						  ,CodSezione
						  ,IsNull(Sequenza,1) As Sequenza
						  ,NULL AS Anteprima
						  ,Documento
						  ,NomeFile
						  ,Estensione
						  ,DescrizioneAllegato
						  ,DescrizioneCampo
						  ,CodTipoAllegatoScheda
						  ,CodStatoAllegatoScheda
						  ,DataEvento
						  ,DataEventoUTC
						  ,CodUtenteRilevazione
						  ,CodUtenteUltimaModifica
						  ,DataRilevazione
						  ,DataRilevazioneUTC
						  ,DataUltimaModifica
						  ,DataUltimaModificaUTC
						  ,IsNull(TA.Descrizione, '''') AS TipoAllegatoScheda
						  ,IsNull(SA.Descrizione, '''') AS StatoAllegatoScheda
						  ,IsNull(SA.Colore, '''') AS ColoreStatoAllegatoScheda
					   '
			 						
	END	
			
	SET @sSQL=@sSQL +  '		FROM T_MovSchedeAllegati MSA
						LEFT JOIN T_TipoAllegatoScheda TA 
							ON (MSA.CodTipoAllegatoScheda = TA.Codice)
						LEFT JOIN T_StatoAllegatoScheda SA 
							ON (MSA.CodStatoAllegatoScheda = SA.Codice)
					'							
				
	SET @sWhere=''		
	
		IF @uIDSchedaAllegato IS NOT NULL
		BEGIN
			SET @sTmp= ' AND MSA.ID=''' + convert(varchar(50),@uIDSchedaAllegato) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			
	
		IF @uIDScheda IS NOT NULL
		BEGIN
			SET @sTmp= ' AND MSA.IDScheda=''' + convert(varchar(50),@uIDScheda) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			
	
	
		IF @sIDGruppo IS NOT NULL
		BEGIN
			SET @sTmp= ' AND MSA.IDGruppo=''' + convert(varchar(50),@sIDGruppo) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
		
		IF @sCodStatoAllegatoScheda NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 MSA.CodStatoAllegatoScheda IN ('+ @sCodStatoAllegatoScheda + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
	
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	
	PRINT @sSQL
 	
	EXEC (@sSQL)	
		
				
		 EXEC MSP_InsMovTimeStamp @xTimeStamp	
	
					
								
	
		RETURN 0
END