CREATE PROCEDURE [dbo].[MSP_SelMovSchedaAvanzato](@xParametri XML)
AS
BEGIN
		
	
				
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sIDEpisodio AS VARCHAR(50)					
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER		
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER	
	DECLARE @nVersione AS INTEGER
	DECLARE @nNumero AS INTEGER
	DECLARE @sStoricizzata AS VARCHAR(20)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME

	DECLARE @dDataInizioCreazione AS DATETIME
	DECLARE @dDataFineCreazione AS DATETIME
	DECLARE @sDaCompletare AS Varchar(20)
	DECLARE @uIDPazienteCalcolato AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @sCodEntita AS VARCHAR(1800)
	DECLARE @sCodScheda AS VARCHAR(1800)	
	DECLARE @sCodRuolo AS VARCHAR(20)	

	DECLARE @bDatiEstesi AS BIT
	DECLARE @bSoloRTF AS BIT
	DECLARE @bSoloDatiMancantiRTF AS BIT
	DECLARE @bSoloDatiInRilievoRTF AS BIT
	DECLARE @bFiltraEntitaDatiInRilievoRTF AS BIT
	DECLARE @bSoloAlertSchedeVuote AS BIT
	
	DECLARE @bSchedaSemplice AS BIT
	
		DECLARE @sCodStatoEpisodio AS VARCHAR(1800)	
	DECLARE @sCodStatoAppuntamento AS VARCHAR(1800)	
	DECLARE @sCodAgenda AS VARCHAR(1800)	
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER

		DECLARE @bJoinEpisodi AS BIT
	DECLARE @bJoinTrasferimenti AS BIT
	DECLARE @bJoinAppuntamenti AS BIT
	DECLARE @bJoinAgende AS BIT
	
	DECLARE @nCountAlias AS INTEGER

	SET @bJoinEpisodi=0
	SET @bJoinTrasferimenti=0
	SET @bJoinAppuntamenti=0
	SET @bJoinAgende=0
	
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')
	
		SET @bSchedaSemplice=(SELECT TOP 1 ValoreParametro.SchedaSemplice.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SchedaSemplice') as ValoreParametro(SchedaSemplice))											
	SET @bSchedaSemplice=ISNULL(@bSchedaSemplice,0)
	
		SET @sCodEntita =''
	SELECT	@sCodEntita  =  @sCodEntita  +
														CASE 
								WHEN @sCodEntita ='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodEntita.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita)
						 
	SET @sCodEntita =LTRIM(RTRIM(@sCodEntita ))
	IF	@sCodEntita ='''''' SET @sCodEntita =''
	SET @sCodEntita =UPPER(@sCodEntita )
	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
					  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					  
				
		SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
					  
	IF 	ISNULL(@sIDEpisodio,'') <> '' AND ISNULL(@sIDEpisodio,'') <> 'NULL' 	
		SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sIDEpisodio)		
	ELSE
		SET @uIDEpisodio=NULL	
							  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
		BEGIN
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
				
						SET @uIDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
		END
	ELSE
		BEGIN
						SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=CONVERT(UNIQUEIDENTIFIER,@sGUID)	)
		END

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
				
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaPadre.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaPadre') as ValoreParametro(IDSchedaPadre))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					  
		SET @nVersione=(SELECT TOP 1 ValoreParametro.Versione.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))					  
	SET @nVersione=ISNULL(@nVersione,0)
	
		SET @nNumero=(SELECT TOP 1 ValoreParametro.Numero.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Numero') as ValoreParametro(Numero))					  
	SET @nNumero=ISNULL(@nNumero,0)
	
	
		SET @sStoricizzata=(SELECT TOP 1 ValoreParametro.Storicizzata.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Storicizzata') as ValoreParametro(Storicizzata))					  
	SET @sStoricizzata=ISNULL(@sStoricizzata,'TUTTE')
	SET @sStoricizzata=UPPER(@sStoricizzata)
	
		SET @sDaCompletare=(SELECT TOP 1 ValoreParametro.DaCompletare.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DaCompletare') as ValoreParametro(DaCompletare))					  
	SET @sDaCompletare=ISNULL(@sDaCompletare,'TUTTE')
	SET @sDaCompletare=UPPER(@sDaCompletare)
	
	
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
				
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizioCreazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizioCreazione') as ValoreParametro(DataInizioCreazione))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizioCreazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizioCreazione =NULL								
		END
					  
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFineCreazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFineCreazione') as ValoreParametro(DataFineCreazione))					  	
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFineCreazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFineCreazione =NULL		
		END 

		SET @sCodScheda=''
	SELECT	@sCodScheda =  @sCodScheda +
														CASE 
								WHEN @sCodScheda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodScheda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda)
						 
	SET @sCodScheda=LTRIM(RTRIM(@sCodScheda))
	IF	@sCodScheda='''''' SET @sCodScheda=''
	

	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		
						  
		SET @sCodStatoEpisodio=''
	SELECT	@sCodStatoEpisodio =  @sCodStatoEpisodio +
														CASE 
								WHEN @sCodStatoEpisodio='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio)
						 
	SET @sCodStatoEpisodio=LTRIM(RTRIM(@sCodStatoEpisodio))
	IF	@sCodStatoEpisodio='''''' SET @sCodStatoEpisodio=''
	SET @sCodStatoEpisodio=UPPER(@sCodStatoEpisodio)
	
	
		SET @sCodStatoAppuntamento=''
	SELECT	@sCodStatoAppuntamento =  @sCodStatoAppuntamento +
														CASE 
								WHEN @sCodStatoAppuntamento='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoAppuntamento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoAppuntamento') as ValoreParametro(CodStatoAppuntamento)
						 
	SET @sCodStatoAppuntamento=LTRIM(RTRIM(@sCodStatoAppuntamento))
	IF	@sCodStatoAppuntamento='''''' SET @sCodStatoAppuntamento=''
	
	
		SET @sCodAgenda=''
	SELECT	@sCodAgenda =  @sCodAgenda +
														CASE 
								WHEN @sCodAgenda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodAgenda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)						 
	SET @sCodAgenda=LTRIM(RTRIM(@sCodAgenda))
	IF	@sCodAgenda='''''' SET @sCodAgenda=''
	SET @sCodAgenda=UPPER(@sCodAgenda)
		
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
		SET @bSoloRTF=(SELECT TOP 1 ValoreParametro.SoloRTF.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloRTF') as ValoreParametro(SoloRTF))											
	SET @bSoloRTF=ISNULL(@bSoloRTF,0)
	
		SET @bSoloDatiMancantiRTF=(SELECT TOP 1 ValoreParametro.SoloDatiMancantiRTF.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloDatiMancantiRTF') as ValoreParametro(SoloDatiMancantiRTF))											
	SET @bSoloDatiMancantiRTF=ISNULL(@bSoloDatiMancantiRTF,0)
	
		SET @bSoloDatiInRilievoRTF=(SELECT TOP 1 ValoreParametro.SoloDatiInRilievoRTF.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloDatiInRilievoRTF') as ValoreParametro(SoloDatiInRilievoRTF))											
	SET @bSoloDatiInRilievoRTF=ISNULL(@bSoloDatiInRilievoRTF,0)

		SET @bSoloAlertSchedeVuote=(SELECT TOP 1 ValoreParametro.SoloAlertSchedeVuote.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloAlertSchedeVuote') as ValoreParametro(SoloAlertSchedeVuote))											
	SET @bSoloAlertSchedeVuote=ISNULL(@bSoloAlertSchedeVuote,0)

		SET @bFiltraEntitaDatiInRilievoRTF=(SELECT TOP 1 ValoreParametro.FiltraEntitaDatiInRilievoRTF.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/FiltraEntitaDatiInRilievoRTF') as ValoreParametro(FiltraEntitaDatiInRilievoRTF))											
	SET @bFiltraEntitaDatiInRilievoRTF=ISNULL(@bFiltraEntitaDatiInRilievoRTF,0)

				
	IF @uIDPaziente IS NULL
		SET @uIDPazienteCalcolato=(SELECT TOP 1 IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodio)
	ELSE
		SET @uIDPazienteCalcolato=@uIDPaziente
		
	
				
	CREATE TABLE  #tmpPazientiAlias
	(
		IDPaziente UNIQUEIDENTIFIER
	)
	
	
	INSERT INTO  #tmpPazientiAlias(IDPaziente)
	VALUES (@uIDPazienteCalcolato)	
	
	INSERT INTO   #tmpPazientiAlias(IDPaziente)
	SELECT IDPazienteVecchio
	FROM 
		(SELECT IDPazienteVecchio
		 FROM T_PazientiAlias
		 WHERE 
			IDPaziente IN 
				(SELECT IDPaziente
				 FROM T_PazientiAlias WITH (NOLOCK)
				 WHERE IDPazienteVecchio=@uIDPazienteCalcolato
				) 
		GROUP BY IDPazienteVecchio						
		) AS Q
	WHERE
		IDPazienteVecchio NOT IN (SELECT IDPaziente FROM  #tmpPazientiAlias) 	
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 

	CREATE INDEX IX_IDPaziente ON  #tmpPazientiAlias (IDPaziente)  
		
	SET @nCountAlias=(SELECT COUNT(*) FROM #tmpPazientiAlias)
			
	
	
				
		SET @gIDSessione=NEWID()
	
	SET @sSQL='
					INSERT INTO T_TmpFiltriSchede(IDSessione,IDScheda,CodEntita)
					SELECT '''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' S.ID AS IDScheda			  
							,S.CodEntita
					FROM T_MovSchede S WITH (NOLOCK)		 
			   '


		
				SET @sWhere=''
	SET @sTmp=''
		

						
		IF @bSoloDatiInRilievoRTF=0 OR @bFiltraEntitaDatiInRilievoRTF=1
	BEGIN
				
		IF @sCodEntita <> ''
		BEGIN
			SET @sTmp= ' AND S.CodEntita IN ('+ @sCodEntita + ')'
			
			SET @sWhere= @sWhere + @sTmp	
		END				
	END
	ELSE
		BEGIN

															CREATE TABLE #tmpSchedeRuolo
				(
					Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
				)
	
			INSERT INTO #tmpSchedeRuolo(Codice)
					SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
					WHERE CodAzione='INS' AND
						  CodEntita='SCH' AND
						  CodRuolo =@sCodRuolo						
				  
			CREATE INDEX IX_CodUA ON #tmpSchedeRuolo (Codice)    
	
						INSERT INTO #tmpSchedeRuolo(Codice)
			SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
					WHERE CodAzione='VSR' AND
						  CodEntita='SCH' AND
						  CodRuolo =@sCodRuolo	AND 
						  CodVoce NOT IN (SELECT Codice FROM #tmpSchedeRuolo)
			
						IF @uIDEpisodio IS NOT NULL
			BEGIN
								CREATE TABLE #tmpTrasfCart
				(ID UNIQUEIDENTIFIER)

				INSERT INTO #tmpTrasfCart(ID)
				SELECT ID FROM T_MovTrasferimenti
				WHERE IDCartella=@uIDCartella

				
				SET @sTmp= ' AND 
								1<=
									(CASE  
																				WHEN S.CodEntita =''ALG'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovAlertGenerici MOV WITH (NOLOCK)
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoAlertGenerico=''DV'' 													
												 )
																				WHEN S.CodEntita =''ALA'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovAlertAllergieAnamnesi MOV WITH (NOLOCK)											 
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoAlertAllergiaAnamnesi=''AT''
												 )	
																				WHEN S.CodEntita =''DCL'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovDiarioClinico MOV WITH (NOLOCK)	
														INNER JOIN #tmpTrasfCart TC
															ON MOV.IDTrasferimento=TC.ID										 
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoDiario=''VA''
												 )	
																				WHEN S.CodEntita =''PVT'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovParametriVitali MOV WITH (NOLOCK)	
														INNER JOIN #tmpTrasfCart TC
															ON MOV.IDTrasferimento=TC.ID
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoParametroVitale NOT IN (''CA'',''AN'')
												 )	
																				WHEN S.CodEntita =''WKI'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovTaskInfermieristici MOV WITH (NOLOCK)
														INNER JOIN #tmpTrasfCart TC
																ON MOV.IDTrasferimento=TC.ID
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoTaskInfermieristico NOT IN (''CA'',''AN'',''ER'')
												 )	
												 
																				WHEN S.CodEntita =''PRF'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovPrescrizioni MOV WITH (NOLOCK)														
												 WHERE 
													MOV.ID=S.IDEntita AND													
													EXISTS
														(SELECT IDPrescrizione
														 FROM T_MovPrescrizioniTempi MOVT WITH (NOLOCK)
														 WHERE 
															MOVT.CodStatoPrescrizioneTempi IN (''VA'',''SS'') AND
															MOVT.IDPrescrizione = MOV.ID																)
												 )		

																				WHEN S.CodEntita =''PRT'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovPrescrizioniTempi MOV WITH (NOLOCK)
														INNER JOIN T_MovPrescrizioni MOVP
															ON MOV.IDPrescrizione=MOVP.ID														
												 WHERE 
													MOV.ID=S.IDEntita AND		
													MOV.CodStatoPrescrizioneTempi IN (''VA'',''SS'') 
												 )		
												 		 
																				WHEN S.CodEntita =''APP'' AND S.IDEpisodio IS NOT NULL THEN 
												(SELECT TOP 1 1
												 FROM T_MovAppuntamenti	MOV WITH (NOLOCK)	
															INNER JOIN #tmpTrasfCart TC
																ON MOV.IDTrasferimento=TC.ID								 
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoAppuntamento NOT IN (''CA'',''AN'')												
												 )		
												 
																				WHEN S.CodEntita =''APP'' AND S.IDEpisodio IS NULL THEN 
												(SELECT TOP 1 1
												 FROM T_MovAppuntamenti	MOV WITH (NOLOCK)									 
												 WHERE 
													MOV.ID=S.IDEntita AND
													MOV.CodStatoAppuntamento NOT IN (''CA'',''AN'') 												
												 )	

																				WHEN S.CodEntita =''CSG'' THEN 1

																				WHEN S.CodEntita =''CSP'' THEN 1

																				WHEN S.CodEntita =''EPI'' AND
												S.IDTrasferimento IN (SELECT ID FROM #tmpTrasfCart) THEN 1
											
																				WHEN S.CodEntita =''PAZ'' AND
												EXISTS (SELECT Codice 
														FROM
															 #tmpSchedeRuolo SC
														WHERE
															SC.Codice=S.CodScheda
														)	
												THEN 1
										ELSE 0	
									END)'						
					END
				ELSE
					BEGIN						
						
						SET @sTmp= ' AND 
								1<=
									(CASE  
																				WHEN S.CodEntita =''ALG'' THEN 0
																						
																				WHEN S.CodEntita =''ALA'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovAlertAllergieAnamnesi WITH (NOLOCK)											 
												 WHERE 
													ID=S.IDEntita AND
													CodStatoAlertAllergiaAnamnesi=''AT''
												 )	
																				WHEN S.CodEntita =''DCL'' THEN 0
																				
																				WHEN S.CodEntita =''PVT'' THEN 0
																						
																				WHEN S.CodEntita =''WKI'' THEN 0
										
																				WHEN S.CodEntita =''PRF'' THEN 0											
												 
																				WHEN S.CodEntita =''APP'' THEN 
												(SELECT TOP 1 1
												 FROM T_MovAppuntamenti	WITH (NOLOCK)									 
												 WHERE 
													ID=S.IDEntita AND
													CodStatoAppuntamento NOT IN (''CA'',''AN'') AND
													IDEpisodio IS NULL
												 )	
										
																				WHEN S.CodEntita =''PAZ'' AND
												EXISTS (SELECT Codice 
														FROM
															 #tmpSchedeRuolo SC
														WHERE
															SC.Codice=S.CodScheda
														)	
											THEN 1	
																																			
																				WHEN S.CodEntita =''EPI'' THEN 0
										
																				WHEN S.CodEntita =''WKI'' THEN 0

																				WHEN S.CodEntita =''CSG'' THEN 1

																				WHEN S.CodEntita =''CSP'' THEN 0

										ELSE 0
									END)'
					END					
			SET @sWhere= @sWhere + @sTmp								 
		END
	
	
						
		IF @uIDScheda IS NOT NULL 
	BEGIN
		SET @sTmp=' AND S.ID = ''' + CONVERT(VARCHAR(50),@uIDScheda) + ''''	
		SET @sWhere= @sWhere + @sTmp
	END
	
		IF @uIDSchedaPadre IS NOT NULL
	BEGIN
		SET @sWhere= @sWhere + ' AND S.IDSchedaPadre = ''' + CONVERT(VARCHAR(50),@uIDSchedaPadre) + ''''		
		SET @sWhere= @sWhere + @sTmp
	END
	
		IF @uIDPaziente IS NOT NULL
	BEGIN
				SET @sTmp=' AND S.IDPaziente IN (SELECT IDPaziente FROM #tmpPazientiAlias WITH (NOLOCK))'		
		SET @sWhere= @sWhere + @sTmp
	END
	
		IF @uIDEpisodio IS NOT NULL 
	BEGIN
				IF (@bSoloDatiInRilievoRTF=0 AND @bSoloDatiMancantiRTF=0) 
		BEGIN
			SET @sTmp= ' AND S.IDEpisodio = ''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''''		
			SET @sWhere= @sWhere + @sTmp
		END
		ELSE			
		BEGIN
									IF @nCountAlias >1
				BEGIN				
					SET @sTmp= ' AND (S.IDEpisodio = ''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' OR (S.IDPaziente IN (SELECT IDPaziente FROM #tmpPazientiAlias WITH (NOLOCK)) AND S.CodEntita=''PAZ''))'			
				END	
			ELSE
				SET @sTmp= ' AND (S.IDEpisodio = ''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' OR (S.IDPaziente = ''' + CONVERT(VARCHAR(50),@uIDPazienteCalcolato) + ''' AND S.CodEntita=''PAZ''))'			
			
			SET @sWhere= @sWhere + @sTmp
		END		
	END			
	
		IF @uIDTrasferimento IS NOT NULL
	BEGIN
		SET @sTmp= ' AND S.IDTrasferimento = ''' + CONVERT(VARCHAR(50),@uIDTrasferimento) + ''''	
		SET @sWhere= @sWhere + @sTmp	
	END	
	
		IF @uIDEntita IS NOT NULL
	BEGIN
		SET @sTmp= ' AND S.IDEntita = ''' + CONVERT(VARCHAR(50),@uIDEntita) + ''''	
		SET @sWhere= @sWhere + @sTmp	
	END	
	
		IF @nVersione<>0
	BEGIN
		SET @sTmp=  ' AND S.Versione = ' + CONVERT(VARCHAR(50),@nVersione) 
		SET @sWhere= @sWhere + @sTmp
	END	
	
		IF @nNumero<>0
	BEGIN
		SET @sTmp=  ' AND S.Numero = ' + CONVERT(VARCHAR(50),@nNumero) 
		SET @sWhere= @sWhere + @sTmp
	END	
	
	
		SET @sTmp= 	CASE 
					WHEN @sStoricizzata='SI' THEN ' AND S.Storicizzata = 1' 
					WHEN @sStoricizzata='NO' THEN ' AND S.Storicizzata = 0' 
					ELSE ''
				END	
	SET @sWhere= @sWhere + @sTmp
										
		IF @dDataInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataFine IS NULL 
								THEN ' AND S.DataUltimaModifica = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
							ELSE ' AND S.DataUltimaModifica >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND S.DataUltimaModifica <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
		IF @dDataInizioCreazione IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataFineCreazione IS NULL 
								THEN ' AND S.DataCreazione = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizioCreazione,120) +''',120)'									
							ELSE ' AND S.DataCreazione >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizioCreazione,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFineCreazione IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND S.DataCreazione <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFineCreazione,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END

		SET @sTmp= 	CASE 
					WHEN @sDaCompletare='SI' THEN ' AND S.DatiObbligatoriMancantiRTF IS NULL' 
					WHEN @sDaCompletare='NO' THEN ' AND S.DatiObbligatoriMancantiRTF IS NOT NULL' 
					ELSE ''
				END	
	SET @sWhere= @sWhere + @sTmp

		IF ISNULL(@bSoloAlertSchedeVuote,0)=1
		SET @sTmp= ' AND S.CodScheda IN (SELECT Codice FROM T_Schede WHERE ISNULL(AlertSchedaVuota,0)=1)'

	SET @sWhere= @sWhere + @sTmp
	
	
				
		
	IF @sCodStatoEpisodio<> '' AND 	@bSchedaSemplice=0	
	BEGIN
		IF @sCodStatoEpisodio='''Nessuno'''
		    			SET @sTmp=  ' AND S.CodEntità <> ''EPI'' '  	
		ELSE
			BEGIN
								SET @bJoinTrasferimenti=1
	
				SET @sSQL=@sSQL + ' 					
						LEFT JOIN T_MovTrasferimenti T WITH (NOLOCK)
							ON (S.IDTrasferimento=S.ID)	
						LEFT JOIN T_MovEpisodi E WITH (NOLOCK)
							ON (T.IDEpisodio = E.ID)
							
						'
							
								SET @sTmp=  ' AND 			
								 E.CodStatoEpisodio IN ('+ @sCodStatoEpisodio + ')
							'  			 					
			END 			
		SET @sWhere= @sWhere + @sTmp
	END	
	
	
		IF @sCodScheda<> ''
	BEGIN
		SET @sTmp=  ' AND S.CodScheda IN ('+ @sCodScheda + ')'  		
		SET @sWhere= @sWhere + @sTmp
	END	
	
			IF @sCodScheda<> ''
	BEGIN
		SET @sTmp=  ' AND S.CodStatoScheda <> (''CA'')'  		
		SET @sWhere= @sWhere + @sTmp
	END	
	
		
					IF @bSoloDatiMancantiRTF<> ''
	BEGIN
				SET @sTmp=  ' AND LenDatiObbligatoriMancantiRTF > 0 AND S.CodStatoScheda <> (''CA'')'  		
		SET @sWhere= @sWhere + @sTmp
	END	
	
					IF @bSoloDatiInRilievoRTF<> ''
	BEGIN
								SET @sTmp=  ' AND (LenDatiRilievoRTF > 116  AND S.CodStatoScheda <> (''CA''))'  		
		SET @sWhere= @sWhere + @sTmp
	END	
	
	
			
		
	
	IF @sCodStatoAppuntamento<> '' AND 	@bSchedaSemplice=0	
	BEGIN		
				SET @bJoinAppuntamenti=1
		
		SET @sSQL=@sSQL + ' 					
							LEFT JOIN T_MovAppuntamenti APP WITH (NOLOCK)
								ON (S.IDEntita=APP.ID)								
						'
	
			
		SET @sTmp=  ' AND 			
						 APP.CodStatoAppuntamento IN ('+ @sCodStatoAppuntamento + ')
						
					' 	
		SET @sWhere= @sWhere + @sTmp
	END	
			
		
	
	IF @sCodAgenda<> '' AND @bSchedaSemplice=0	
	BEGIN			
				IF @sCodAgenda='''Nessuna'''
			SET @sTmp=  ' AND S.CodEntita <> ''APP'''  	
		ELSE
			BEGIN					
								IF @bJoinAppuntamenti=0
					SET @sSQL=@sSQL + ' 					
							LEFT JOIN T_MovAppuntamenti APP WITH (NOLOCK)
								ON (S.IDEntita=APP.ID)								
						'										
								SET @bJoinAgende=1		
				SET @sSQL=@sSQL + ' 					
							LEFT JOIN T_MovAppuntamentiAgende AGE WITH (NOLOCK)
								ON (APP.ID=AGE.IDAppuntamento)							
							'							
																		 								
				SET @sTmp=  ' AND 			
								AGE.CodAgenda IN ('+ @sCodAgenda + ')
							'  																					
			END	
			
		SET @sWhere= @sWhere + @sTmp		
	END		
		
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		SET @sSQL=@sSQL +' OPTION (USE HINT(''QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'')) ' 
		
	END	
	
	
	PRINT @sSQL
	EXEC (@sSQL)
			
			
	SET @sSQL=''
		
		IF @bSoloRTF=0 
		SET @sSQL='
			SELECT 
				S.*,
				AnagS.Descrizione 
					+ CASE 
						WHEN ISNULL(AnagS.NumerositaMassima,0) > 1 THEN '', n. '' + CONVERT(VARCHAR(20),Numero) 
						ELSE ''''
					  END	
					 AS Descrizione, 
				LC.Descrizione AS DescrUtenteCreazione,
				CASE 
					WHEN LM.Codice IS NULL THEN LC.Descrizione
					ELSE LM.Descrizione 
				END AS DescrUtenteUltimaModifica,
				LV.Descrizione AS DescrUtenteValidazione
				'
	ELSE			
		SET @sSQL='
			SELECT 
				S.ID,
				AnagS.Descrizione 
					+ CASE 
						WHEN ISNULL(AnagS.NumerositaMassima,0) > 1 THEN '', n. '' + CONVERT(VARCHAR(20),Numero) 
						ELSE ''''
					  END	
					 AS Descrizione,							
				AnteprimaRTF,
				DatiObbligatoriMancantiRTF,
				DatiRilievoRTF
				'
	SET @sSQL=@sSQL+ '	
			FROM T_MovSchede S WITH (NOLOCK)		
				LEFT JOIN T_Schede AS AnagS WITH (NOLOCK)
					ON (S.CodScheda=AnagS.Codice)								
				LEFT JOIN T_Login LC 
					ON S.CodUtenteRilevazione = LC.Codice
				LEFT JOIN T_Login LM
					ON S.CodUtenteUltimaModifica = LM.Codice
				LEFT JOIN T_Login LV
					ON S.CodUtenteValidazione = LV.Codice
			INNER JOIN T_TmpFiltriSchede TMP WITH (NOLOCK)
					ON (S.ID=TMP.IDScheda)					
			'	  
				
	SET @sSQL=@sSQL+ '					
					LEFT JOIN 
						
							(SELECT ''SCH'' As  CodEntita, 10 As EntitaOrdine
							 UNION
							 SELECT ''ALA'' As  CodEntita, 20 As EntitaOrdine
							 UNION
							 SELECT ''ALG'' As  CodEntita, 30 As EntitaOrdine
							 UNION
							 SELECT ''DCL'' As  CodEntita, 40 As EntitaOrdine
							 UNION
							 SELECT ''PVT'' As  CodEntita, 50 As EntitaOrdine
							 UNION
							 SELECT ''WKI'' As  CodEntita, 60 As EntitaOrdine
							 UNION
							 SELECT ''PRF'' As  CodEntita, 70 As EntitaOrdine
							 UNION
							 SELECT ''APP'' As  CodEntita, 80 As EntitaOrdine
							) AS TORD
						ON (S.CodEntita=TORD.CodEntita)
				'
						
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	

							
	SET @sSQL=@sSQL + ' ORDER BY TORD.EntitaOrdine ASC , S.DataCreazione ASC '
	SET @sSQL=@sSQL + ' OPTION (USE HINT(''QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100''))  '
	
	 PRINT @sSQL 
	
		EXEC (@sSQL)
	
	SET @sSQL=''
	
					
			IF @bDatiEstesi=1 AND @bSchedaSemplice=0
	BEGIN	
		SET @sSQL='
		SELECT DISTINCT
			A.Codice,
			A.Descrizione
		FROM T_MovSchede S 
			INNER JOIN T_MovAppuntamenti APP WITH (NOLOCK)
					ON (S.IDEntita=APP.ID)	
			INNER JOIN T_MovAppuntamentiAgende AGE WITH (NOLOCK)
					ON (APP.ID=AGE.IDAppuntamento)						
			INNER JOIN T_TmpFiltriSchede TMP WITH (NOLOCK)
					ON (S.ID=TMP.IDScheda)
			INNER JOIN T_Agende A WITH (NOLOCK)
				ON A.Codice=AGE.CodAgenda						
		'	  

				SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
		SET @sSQL=@sSQL + ' OPTION (USE HINT(''QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'')) '
	
		 PRINT @sSQL 
				
				EXEC (@sSQL)
	END
	
			
		DELETE FROM T_TmpFiltriSchede 
	WHERE IDSessione=@gIDSessione

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
	
	DROP TABLE  #tmpPazientiAlias		
	
	RETURN 0
END