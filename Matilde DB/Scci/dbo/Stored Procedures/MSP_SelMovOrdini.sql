
CREATE PROCEDURE [dbo].[MSP_SelMovOrdini](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDOrdine AS UNIQUEIDENTIFIER				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sIDOrdineOE AS VARCHAR(50)	
	DECLARE @sNumeroOrdineOE AS VARCHAR(50)
	DECLARE @sCodTipoOrdine AS VARCHAR(1800)
	DECLARE @sCodStatoOrdine AS VARCHAR(1800)
	DECLARE @dDataInserimentoInizio AS DATETIME
	DECLARE @dDataInserimentoFine AS DATETIME
	DECLARE @dDataInoltroInizio AS DATETIME
	DECLARE @dDataInoltroFine AS DATETIME		
	DECLARE @dDataUltimaModificaInizio AS DATETIME
	DECLARE @dDataUltimaModificaFine AS DATETIME
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(1800)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sInfoSistema AS VARCHAR(50)
	DECLARE @sInfoSistema2 AS VARCHAR(50)
		
	DECLARE @bVisualizzazioneSintetica AS BIT	
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @bDatiScheda AS Bit		
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sIDEpisodio AS VARCHAR(50)											   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sOrderBy AS VARCHAR(MAX)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDOrdine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdine') as ValoreParametro(IDOrdine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDOrdine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sIDOrdineOE=(SELECT TOP 1 ValoreParametro.IDOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdineOE') as ValoreParametro(IDOrdineOE))	

		SET @sNumeroOrdineOE=(SELECT TOP 1 ValoreParametro.NumeroOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroOrdineOE') as ValoreParametro(NumeroOrdineOE))	
	
		SET @sCodTipoOrdine=''
	SELECT	@sCodTipoOrdine =  @sCodTipoOrdine +
														CASE 
								WHEN @sCodTipoOrdine='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoOrdine.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoOrdine') as ValoreParametro(CodTipoOrdine)
						 
	SET @sCodTipoOrdine=LTRIM(RTRIM(@sCodTipoOrdine))
	IF	@sCodTipoOrdine='''''' SET @sCodTipoOrdine=''
	SET @sCodTipoOrdine=UPPER(@sCodTipoOrdine)
	
	
		SET @sCodStatoOrdine=''
	SELECT	@sCodStatoOrdine =  @sCodStatoOrdine +
														CASE 
								WHEN @sCodStatoOrdine='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoOrdine.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoOrdine') as ValoreParametro(CodStatoOrdine)
						 				  					  				  							 
	SET @sCodStatoOrdine=LTRIM(RTRIM(@sCodStatoOrdine))
	IF	@sCodStatoOrdine='''''' SET @sCodStatoOrdine=''
	SET @sCodStatoOrdine=UPPER(@sCodStatoOrdine)

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimentoInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInserimentoInizio') as ValoreParametro(DataInserimentoInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInserimentoInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimentoFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInserimentoFine') as ValoreParametro(DataInserimentoFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInserimentoFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoFine =NULL		
		END 		
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltroInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInoltroInizio') as ValoreParametro(DataInoltroInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInoltroInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInoltroInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltroFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInoltroFine') as ValoreParametro(DataInoltroFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInoltroFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInoltroFine =NULL		
		END 
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaInizio .value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModificaInizio ') as ValoreParametro(DataUltimaModificaInizio ))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModificaInizio =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModificaInizio =NULL			
		END
			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModificaFine') as ValoreParametro(DataUltimaModificaFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModificaFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModificaFine =NULL		
		END 
				
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
		
		SET @sIDSistema=''
	SELECT	@sIDSistema =  @sIDSistema +
														CASE 
								WHEN @sIDSistema='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDSistema.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema)
						 
	SET @sIDSistema=LTRIM(RTRIM(@sIDSistema))
	IF	@sIDSistema='''''' SET @sIDSistema=''
	SET @sIDSistema=UPPER(@sIDSistema)

	
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
		SET @sInfoSistema=(SELECT TOP 1 ValoreParametro.InfoSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema') as ValoreParametro(InfoSistema))
	
		SET  @sInfoSistema2=(SELECT TOP 1 ValoreParametro.InfoSistema2.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema2') as ValoreParametro(InfoSistema2))

		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
	
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @bDatiScheda=(SELECT TOP 1 ValoreParametro.DatiScheda.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiScheda') as ValoreParametro(DatiScheda))
	SET @bDatiScheda=ISNULL(@bDatiScheda,0)

	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
				
				
		SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriOrdini(IDSessione,IDOrdine)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDEvidenzaClinica	
					FROM 
						T_MovOrdini	M									  							 
					'
	
					
					
	SET @sWhere=''				
	
		IF @uIDOrdine IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDOrdine) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
			
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	

		IF @uIDTrasferimento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDTrasferimento=''' + convert(varchar(50), @uIDTrasferimento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @sIDOrdineOE IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDOrdineOE=''' + convert(varchar(50),@sIDOrdineOE) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
					
		IF @sNumeroOrdineOE IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.NumeroOrdineOE=''' + convert(varchar(50),@sNumeroOrdineOE) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @sCodTipoOrdine	NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.ID IN (
										SELECT IDOrdine 
										FROM T_MovOrdiniEroganti
										WHERE CodTipoOrdine IN ('+ @sCodTipoOrdine + ')
									  ) 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
		IF @sCodStatoOrdine NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoOrdine IN ('+ @sCodStatoOrdine + ')							 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
			
	SET @sTmp=  ' AND M.CodStatoOrdine <> ''CA'''  				
	SET @sWhere= @sWhere + @sTmp		


		IF @dDataInserimentoInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataInserimentoFine IS NULL 
									THEN ' AND M.DataInserimento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'									
							ELSE ' AND M.DataInserimento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataInserimentoFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataInserimento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
		IF  @dDataUltimaModificaInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataInserimentoFine IS NULL 
									THEN ' AND M.DataUltimaModifica = CONVERT(datetime,'''  + convert(varchar(20), @dDataUltimaModificaInizio,120) +''',120)'									
							ELSE ' AND M.DataUltimaModifica >= CONVERT(datetime,'''  + convert(varchar(20), @dDataUltimaModificaInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataUltimaModificaFine	 IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataUltimaModifica <= CONVERT(datetime,'''  + convert(varchar(20),@dDataUltimaModificaFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
				
		IF  @dDataInoltroInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataInserimentoFine IS NULL 
									THEN ' AND M.DataInoltro = CONVERT(datetime,'''  + convert(varchar(20), @dDataInoltroInizio,120) +''',120)'									
							ELSE ' AND M.DataInoltro >= CONVERT(datetime,'''  + convert(varchar(20), @dDataInoltroInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataInoltroFine	 IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataInoltro <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInoltroFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
		IF @sCodSistema IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.CodSistema=''' + @sCodSistema +''''
			SET @sWhere= @sWhere + @sTmp		
		END
		
		IF  @sIDSistema NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M. IDSistema IN ('+  @sIDSistema + ')							 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	

		IF @sIDGruppo IS NOT NULL					  
		BEGIN
			SET @sTmp= ' AND M.IDGruppo=''' + @sIDGruppo +''''
			SET @sWhere= @sWhere + @sTmp		
		END
		
		IF @sInfoSistema IS NOT NULL					  
		BEGIN
			SET @sTmp= ' AND M.InfoSistema=''' + @sInfoSistema +''''
			SET @sWhere= @sWhere + @sTmp		
		END				  														
	
		IF @sInfoSistema2 IS NOT NULL					  
		BEGIN
			SET @sTmp= ' AND M.InfoSistema2=''' + @sInfoSistema2 +''''
			SET @sWhere= @sWhere + @sTmp		
		END			

		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
				SET @sOrderBy = ' ORDER BY ISNULL(M.DataInoltro,ISNULL(M.DataUltimaModifica,M.DataInserimento)) DESC '	
	SET @sSQL=@sSQL + @sOrderBy
	
	PRINT @sSQL
 	
	EXEC (@sSQL)
	
					
					
		
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
				SET @sSQL='	SELECT
					   ID
					  ,IDNum
					  ,IDPaziente
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,IDOrdineOE
					  ,NumeroOrdineOE				
					  ,Eroganti
					  ,Prestazioni					 
					  ,CodStatoOrdine
					  ,CodUtenteInserimento
					  ,LI.Descrizione AS DescrUtenteInserimento
					  ,CodUtenteUltimaModifica
					  ,LM.Descrizione AS DescrUtenteUltimaModifica
					  ,CodUtenteInoltro
					  ,LV.Descrizione AS DescrUtenteInoltro
					  ,CASE 
								WHEN DataInserimento IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataInserimento,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInserimento,14),5) 									
								ELSE NULL 
					   END AS DataInserimento					  
					  ,CASE 
								WHEN DataUltimaModifica IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataUltimaModifica,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataUltimaModifica,14),5) 									
								ELSE NULL 
					   END AS DataUltimaModifica
					  ,CASE 
								WHEN DataInoltro IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataInoltro,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInoltro,14),5) 									
								ELSE NULL 
					   END AS DataInoltro	
					  ,M.CodSistema
					  ,M.IDSistema
					  ,M.IDGruppo 
					  ,M.InfoSistema
					  ,M.InfoSistema2
					  ,M.DataProgrammazioneOE AS DataProgrammazione
					'
	ELSE
				SET @sSQL='	SELECT						
						CASE 
								WHEN DataInoltro IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataInoltro,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInoltro,14),5) 									
								ELSE NULL 
					   END AS Data,
					   DataProgrammazioneOE AS DataProgrammazione,
					   SO.Descrizione AS Stato,
					   Eroganti,
					   Prestazioni					
					'					
	IF @bDatiEstesi=1	
			SET @sSQL=@sSQL + ',M.XMLOE AS XMLOE'
				
		IF @bDatiEstesi=1
		SET @sSQL=@sSQL + ' ,I.Icona16,I.Icona32,I.Icona256'
		
		
	IF @bDatiScheda=1	
			SET @sSQL=@sSQL + ',M.StrutturaDatiAccessori, M.DatiDatiAccessori, M.LayoutDatiAccessori'
		
	SET @sSQL=@sSQL + '
				FROM 
					T_TmpFiltriOrdini TMP
						INNER JOIN T_MovOrdini	M	
								ON (TMP.IDOrdine=M.ID)																																	
						LEFT JOIN T_StatoOrdine SO 
							ON (M.CodStatoOrdine=SO.Codice)'
						
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
						SET @sSQL=@sSQL + '											
						LEFT JOIN T_Login LI
							ON (M.CodUtenteInserimento=LI.Codice)
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)
						LEFT JOIN T_Login LV
							ON (M.CodUtenteInoltro=LV.Codice)			
					  '

																		
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	
		SET @sSQL=@sSQL + @sOrderBy
	
	PRINT @sSQL						
	EXEC (@sSQL)
	
					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodStatoOrdine AS CodStato,
							T.Descrizione AS DescStato,							
							T.Icona
						FROM 
							T_TmpFiltriOrdini TMP
								INNER JOIN T_MovOrdini	M	
										ON (TMP.IDOrdine=M.ID)																													
								LEFT JOIN T_StatoOrdine T
									ON (M.CodStatoOrdine=T.Codice)													
						'
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
						
						EXEC (@sSQL)	
		END 			
	
	
				
			
		
					
		DELETE FROM T_TmpFiltriOrdini
	WHERE IDSessione=@gIDSessione
								
				
END