CREATE PROCEDURE [dbo].[MSP_CercaPreTrasferimenti](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)	
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @nNumRighe AS INTEGER
	DECLARE @sCodUA AS VARCHAR(20)
		
	DECLARE @sOrdinamento AS Varchar(500)
	DECLARE @sCodLogin AS VARCHAR(100)
		
		
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sCodUANumerazioneCartella AS VARCHAR(20)
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
				
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')			
	
		SET @sOrdinamento=(SELECT	TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))						 
	SET @sOrdinamento= ISNULL(@sOrdinamento,'')
	SET @sOrdinamento=LTRIM(RTRIM(@sOrdinamento))
	
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 		
	
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))
	SET @nNumRighe=ISNULL(@nNumRighe,0)
				
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
	
		SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))						 
	SET @sCodUA= LTRIM(RTRIM(ISNULL(@sCodUA,'')))
						  
						
				
		SET @sCodUANumerazioneCartella=(SELECT CodUANumerazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
	
		IF ISNULL(@sCodUANumerazioneCartella,'')='' 
		SET @sCodUANumerazioneCartella=@sCodUA
	
		
		CREATE TABLE #tmpFiltriEpisodi
		(
			IDTrasferimento uniqueidentifier NOT NULL
		)
			
			
		SET @sSQL='
			INSERT #tmpFiltriEpisodi(IDTrasferimento) ' 
	SET @sSQL= @sSQL + 
		'SELECT '
			+ CASE 
				WHEN(ISNULL(@nNumRighe,0)=0) THEN ''
				ELSE ' TOP ' + CONVERT(VARCHAR(20),@nNumRighe) 
			END 				
			+ ' T.ID AS IDTrasferimento		
				  
	 FROM T_MovEpisodi M				
			
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
			
			INNER JOIN T_MovPazienti P
				ON M.ID=P.IDEpisodio		
											 
			LEFT JOIN T_MovCartelle CR
				ON T.IDCartella=CR.ID	
												
						LEFT JOIN 	
				T_Letti LT
					ON 	(M.CodAzi=LT.CodAzi AND
						 T.CodSettore=LT.CodSettore AND
						 T.CodLetto=LT.CodLetto)							 		 			
	'		
	
									
				
	DECLARE @sWhere AS VARCHAR(Max)
	
	SET @sWhere=''
	
		IF ISNULL(@sDataNascita,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND P.DataNascita = convert(datetime,''' + @sDataNascita + ''',105)'
	END
			
		IF ISNULL(@sFiltroGenerico,'') <> ''
	BEGIN
				SET @sWhere= @sWhere + ' AND ('
		SET @sWhere= @sWhere + '  P.Cognome like ''%' + @sFiltroGenerico + '%'''
		SET @sWhere= @sWhere + '  OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR M.NumeroNosologico like ''%' + @sFiltroGenerico + '%'''					
		SET @sWhere= @sWhere + '  OR M.NumeroNosologico like ''%' + dbo.MF_ModificaNosologico(@sFiltroGenerico) + '%'''					
		SET @sWhere= @sWhere + '  OR M.NumeroListaAttesa like ''%' + @sFiltroGenerico + '%'''	
							SET @sWhere= @sWhere + '     )' 					
	END
		
		SET @sWhere= @sWhere + ' AND T.CodStatoTrasferimento IN (''AT'')' 
	
		SET @sWhere= @sWhere + ' AND M.CodTipoEpisodio IN (''RO'')' 
	
		SET @sWhere= @sWhere + ' AND M.ID NOT IN (SELECT IDEpisodio FROM T_MovTrasferimenti WHERE CodStatoTrasferimento=''PT'')' 
	
		SET @sWhere= @sWhere + ' AND
						1=CASE 												
				WHEN 
										EXISTS 
						(SELECT ID
						 FROM 
							T_MovTrasferimenti TI WITH (NOLOCK)
								INNER JOIN 
									T_UnitaAtomiche AI WITH (NOLOCK)
										ON (TI.CodUA=AI.Codice)
						 WHERE
							TI.CodStatoTrasferimento IN (''AT'') 																					AND 
								1=CASE 
									WHEN ISNULL(AI.CodUANumerazioneCartella,'''') ='''' AND																				TI.CodUA=''' + @sCodUANumerazioneCartella + ''' THEN 1																		WHEN ISNULL(AI.CodUANumerazioneCartella,'''') <> '''' AND																			ISNULL(AI.CodUANumerazioneCartella,'''')=''' + @sCodUANumerazioneCartella + '''  THEN 1										ELSE 0
								  END			
							AND TI.IDEpisodio=T.IDEpisodio 																												) THEN 0															
				ELSE 1
			  END' 
	
										
		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	
	PRINT @sSQL
	
		EXEC (@sSQL)
				
					
		SET @sSQL=''
		
	SET @sSQL='
		SELECT 
			T.ID AS IDTrasferimento,	
			M.ID AS IDEpisodio,			
			P.IDPaziente,
			P.CodSac,
			P.Cognome,
			P.Nome,
			P.DataNascita,				
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' '' + 
				CASE 
					WHEN ISNULL(Sesso,'''')='''' THEN ''''
					ELSE '' ('' + Sesso +'') ''
				END +
				CASE 
					WHEN DataNascita IS NULL THEN ''''
					ELSE Convert(varchar(10),DataNascita,105)
				END +
			
				CASE 
					WHEN ISNULL(ComuneNascita,'''')='''' THEN ''''
					ELSE '', '' + ComuneNascita
				END +
									
				CASE 
					WHEN ISNULL(CodProvinciaNascita,'''')='''' THEN ''''
					ELSE '' ('' + CodProvinciaNascita + '')''
				END 
				AS Paziente,
			
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') AS Paziente2,
			
		
			
			CASE 
				WHEN ISNULL(Sesso,'''')='''' THEN ''''
				ELSE '' ('' + Sesso +'') ''
			END +
			+ '' ''  +  
			CASE 
				WHEN DataNascita IS NULL THEN ''''
				ELSE Convert(varchar(10),DataNascita,105)
			END +
		
			CASE 
				WHEN ISNULL(ComuneNascita,'''')='''' THEN ''''
				ELSE '', '' + ComuneNascita
			END +
								
			CASE 
				WHEN ISNULL(CodProvinciaNascita,'''')='''' THEN ''''
				ELSE '' ('' + CodProvinciaNascita + '')''
			END 
			AS Paziente3,
					
			  P.CodiceFiscale,	
			  T.CodUA,
			  UA.Descrizione AS DescrUA,
			  ISNULL(T.DescrUO,'''') + '' - '' +   ISNULL(T.DescrSettore,'''') As [UO - Settore],
			  M.DataRicovero,	
			  M.DataDimissione,
			  T.DataIngresso,
			  T.DataUscita,
			  ISNULL(M.DataDimissione,T.DataUscita) AS DataOrdinamentoCartella,				
			  CASE 
				WHEN ISNULL(DataIngresso,getdate())=ISNULL(DataRicovero,getdate()) THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' ''
				
				WHEN DataIngresso IS NOT NULL  AND DataRicovero IS NULL 
					THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' '' 		
					
				WHEN DataIngresso IS NULL  AND DataRicovero IS NOT NULL 
					THEN Convert(varchar(20),DataRicovero,105) + '' '' + Convert(varchar(5),DataRicovero,114)		
					
				WHEN DataIngresso IS NOT NULL  AND DataRicovero IS NOT NULL 
					THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' '' +CHAR(13)+CHAR(10)			
						+ Convert(varchar(20),DataRicovero,105) + '' '' + Convert(varchar(5),DataRicovero,114) + '' '' 				
				END AS [Data Ingresso Data Ricovero],	
				
			  DataIngresso AS DataIngressoGriglia,
			  CASE 
				WHEN ISNULL(DataIngresso,getdate())=ISNULL(DataRicovero,getdate()) THEN NULL
				ELSE DataRicovero 
			  END
			  AS DataRicoveroGriglia,	
			  
			  CASE 
				WHEN ISNULL(DataUscita,getdate())=ISNULL(DataDimissione,getdate()) THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' ''
				WHEN DataUscita IS NOT NULL  AND DataDimissione IS NULL 
					THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' '' 		
				WHEN DataUscita IS NULL  AND DataDimissione IS NOT NULL 
					THEN Convert(varchar(20),DataDimissione,105) + '' '' + Convert(varchar(5),DataDimissione,114)		
				WHEN DataUscita IS NOT NULL  AND DataDimissione IS NOT NULL 
					THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' '' +CHAR(13)+CHAR(10)			
						+ Convert(varchar(20),DataDimissione,105) + '' '' + Convert(varchar(5),DataDimissione,114) + '' '' 				
				END AS [Data Dimissione Data Trasferimento],	
					
			  T.CodStatoTrasferimento,
			  ISNULL(ST.Descrizione,'''') AS DecrStato,
			  ST.Colore AS ColoreStatoTrasferimento,
			  T.CodUO,
			  T.DescrUO,
			  T.CodSettore,
			  T.DescrSettore,
			  ISNULL(T.CodStanza,LT.CodStanza) AS CodStanza,
			  ISNULL(T.DescrStanza,TS.Descrizione) AS DescrStanza,
			  T.CodLetto,
			  T.DescrLetto,
			  ISNULL(T.DescrLetto,'''') +  '' / '' + ISNULL(ISNULL(T.DescrStanza,TS.Descrizione),'''')   AS DescStanzaLetto,
			  M.CodTipoEpisodio,			  
			  M.NumeroNosologico,
			  M.NumeroListaAttesa,
			  ISNULL(M.CodTipoEpisodio,'''') + ''-'' + 
				CASE
					WHEN ISNULL(NumeroNosologico,'''')='''' THEN ISNULL(NumeroListaAttesa,'''')
					ELSE NumeroNosologico
				END AS DescEpisodio,				  			
							  			  			 						
			  ST.Descrizione +
			  CASE				
				WHEN T.CodStatoTrasferimento IN (''TR'',''DM'') THEN 
						 +CHAR(13)+CHAR(10)	+	
						''il '' + Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114)						
				ELSE ''''
			  END	AS DescrStatoGriglia,
			  T.IDCartella AS IDCartella,
			  ISNULL(CR.CodStatoCartella,''DA'') AS CodStatoCartella
	
				
		FROM T_MovEpisodi M
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
			INNER JOIN T_MovPazienti P
					ON M.ID=P.IDEpisodio
					
			LEFT JOIN T_MovCartelle CR
					ON T.IDCartella=CR.ID										
					
			INNER JOIN #tmpFiltriEpisodi TMP
					ON T.ID=TMP.IDTrasferimento	
						
			LEFT JOIN T_UnitaAtomiche UA
				ON T.CodUA=UA.Codice		
				
			LEFT JOIN T_StatoTrasferimento ST
				ON T.CodStatoTrasferimento=ST.Codice	
				
			LEFT JOIN T_StatoCartella SC
				ON CR.CodStatoCartella=SC.Codice										
						
		    			LEFT JOIN 	
					T_Letti LT
						ON 	(M.CodAzi=LT.CodAzi AND
							 T.CodSettore=LT.CodSettore AND
							  T.CodLetto=LT.CodLetto)
			LEFT JOIN	
					T_Stanze TS 
						ON (M.CodAzi=TS.CodAzi AND
							LT.CodStanza=TS.Codice)	
							
		
		WHERE
			T.CodStatoTrasferimento <> ''CA''
		'	  		

		IF @sOrdinamento<> ''
		SET @sSQL = @sSQL + ' ORDER BY ' + @sOrdinamento + ''
		
	
		EXEC (@sSQL)
	
	SET @sSQL=''
		
					
		DROP TABLE #tmpFiltriEpisodi 
								
	           
	RETURN 0
	
END