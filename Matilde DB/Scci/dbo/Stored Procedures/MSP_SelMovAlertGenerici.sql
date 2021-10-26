CREATE PROCEDURE [dbo].[MSP_SelMovAlertGenerici](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDAlertGenerico AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoAlert AS VARCHAR(1800)
	DECLARE @sCodTipoAlertGenerico AS VARCHAR(1800)
	DECLARE @bVisualizzazioneSintetica AS BIT
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUA AS VARCHAR(20)
	
		DECLARE @nTemp AS INTEGER
	DECLARE @bCancella AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bVista AS BIT
	
		DECLARE @sGUID AS VARCHAR(Max)
							  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
							  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
		IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	END
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAlertGenerico.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAlertGenerico') as ValoreParametro(IDAlertGenerico))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDAlertGenerico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
						  				  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoAlert=''
	SELECT	@sCodStatoAlert =  @sCodStatoAlert +
														CASE 
								WHEN @sCodStatoAlert='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoAlert.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoAlert') as ValoreParametro(CodStatoAlert)
						 
	SET @sCodStatoAlert=LTRIM(RTRIM(@sCodStatoAlert))
	IF	@sCodStatoAlert='''''' SET @sCodStatoAlert=''
	SET @sCodStatoAlert=UPPER(@sCodStatoAlert)

	
		SET @sCodTipoAlertGenerico  =''
	SELECT	@sCodTipoAlertGenerico   =  @sCodTipoAlertGenerico   +
														CASE 
								WHEN @sCodTipoAlertGenerico  ='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoAlertGenerico.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoAlertGenerico  ') as ValoreParametro(CodTipoAlertGenerico  )
						 
	SET @sCodTipoAlertGenerico=LTRIM(RTRIM(@sCodTipoAlertGenerico))
	IF	@sCodTipoAlertGenerico='''''' SET @sCodTipoAlertGenerico=''
	SET @sCodTipoAlertGenerico=UPPER(@sCodTipoAlertGenerico)


		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
	
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
			
	
		SET @sCodUA=(SELECT CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento )	
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)  
	  
		  

			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='AlertG_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='AlertG_Vista'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bVista=1
	ELSE
		SET @bVista=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='AlertG_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0
						
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
	SET @sWhere=''		
	SET @sSQL='	SELECT	'
	
		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 
	
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
				SET @sSQL=@sSQL + ' 
						M.ID,
						P.IDPaziente,
						M.IDEpisodio,
						M.IDTrasferimento,
						M.DataEvento,
						M.DataEventoUTC,
						MS.AnteprimaRTF,
						M.CodTipoAlertGenerico AS CodTipo,
						T.Descrizione AS DescrTipo, 
						M.CodStatoAlertGenerico,
						S.Descrizione As DescrStato,
						M.CodUtenteRilevazione AS CodUtente,
						L.Descrizione AS DescrUtente,
						M.CodUtenteVisto,
						LVisto.Descrizione AS DescrUtenteVisto,
						M.DataVisto,
						M.DataVistoUTC,
						CONVERT(INTEGER,
							CASE 
								WHEN ISNULL(CodStatoAlertGenerico,'''') =''DV'' THEN 1
								ELSE 0
							END &	
							CONVERT(BIT, + ' + CONVERT(CHAR(1), @bCancella) + ')							
							&
							CASE 
								WHEN TUA.Codice IS NULL THEN 0
								ELSE 1
							END 
						) AS PermessoCancella,
						CONVERT(INTEGER,
							CASE 
								WHEN ISNULL(CodStatoAlertGenerico,'''')=''DV'' THEN 1
								ELSE 0
							END &	
							CONVERT(BIT, + ' + CONVERT(CHAR(1), @bVista) + ')
						) AS PermessoVista,
						CONVERT(INTEGER,
							CASE 
								WHEN ISNULL(CodStatoAlertGenerico,'''')=''DV'' THEN 1
								ELSE 0
							END &	
							CONVERT(BIT, + ' + CONVERT(CHAR(1), @bModifica) + ')
							&
							CASE 
								WHEN TUA.Codice IS NULL THEN 0
								ELSE 1
							END 
						) AS PermessoModifica					
						'
	ELSE
				SET @sSQL=@sSQL + ' 
						M.ID,						
						CASE 
						WHEN DataEvento IS NOT NULL THEN	
								Convert(VARCHAR(10),M.DataEvento,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataEvento,14),5) 									
						ELSE NULL 
						END AS DataEvento,				
						MS.AnteprimaRTF			
						'
	
		IF @bDatiEstesi=1
		SET @sSQL=@sSQL + ' ,I.Icona'
	
		SET @sSQL=@sSQL + ' 						
				FROM 
					T_MovAlertGenerici	M 
					LEFT JOIN T_TipoAlertGenerico T
							ON (M.CodTipoAlertGenerico=T.Codice)
					LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)																					
					LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita=''ALG'' AND							
									Storicizzata=0 AND
									CodStatoScheda <> ''CA''
								) AS MS
							ON (MS.IDEntita=M.ID AND
								MS.CodScheda=T.CodScheda)
					LEFT JOIN 								
						(SELECT DISTINCT
							A.CodVoce AS Codice		
						FROM	
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
									(A.CodUA=T.CodUA)
							WHERE A.CodEntita=''ALG''
						) AS TUA
					ON M.CodTipoAlertGenerico=TUA.Codice	
							
				'

	IF ISNULL(@bVisualizzazioneSintetica,0)=0
				SET @sSQL=@sSQL + ' 												
						LEFT JOIN T_StatoAlertGenerico S
							ON (M.CodStatoAlertGenerico=S.Codice)
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN T_Login LVisto
							ON (M.CodUtenteVisto=LVisto.Codice)		
							
					  '
	IF 	@bDatiEstesi=1				  
		SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  CodTipo,
							  CodStato,							 
							  Icona256 AS Icona
							 FROM T_Icone
							 WHERE CodEntita=''ALG''
							) AS I
								ON (M.CodTipoAlertGenerico=I.CodTipo AND 	
									M.CodStatoAlertGenerico=I.CodStato
									)
								
					'				
		IF @uIDPaziente IS NOT NULL
		BEGIN			
		  	SET @sTmp= ' AND 
					(P.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +'''
					 OR
					 					 P.IDPaziente IN 
								(SELECT IDPazienteVecchio
								 FROM T_PazientiAlias
								 WHERE 
									IDPaziente IN 
										(SELECT IDPaziente
										 FROM T_PazientiAlias
										 WHERE IDPazienteVecchio=''' + convert(varchar(50),@uIDPaziente) +'''
										)
								)
							
					 )			
					'
									
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
								
		IF @uIDAlertGenerico IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDAlertGenerico) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		

		
		IF @sCodStatoAlert NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoAlertGenerico IN ('+ @sCodStatoAlert + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
			IF @sCodStatoAlert NOT IN ('''Tutti''')
				BEGIN
										SET @sTmp=  ' AND 			
									 M.CodStatoAlertGenerico NOT IN (''AN'',''CA'')
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
			ELSE
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoAlertGenerico <> ''CA''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
		END						
		
 		IF @sCodTipoAlertGenerico NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoAlertGenerico IN ('+ @sCodTipoAlertGenerico + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	

		IF @bVisualizzazioneSintetica=1
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoAlertGenerico =''DV''
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
			
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	SET @sSQL=@sSQL +' ORDER BY M.DataEvento DESC ' 	
	PRINT @sSQL
 	
	EXEC (@sSQL)
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp
	
	DROP TABLE #tmpUA	
			
END