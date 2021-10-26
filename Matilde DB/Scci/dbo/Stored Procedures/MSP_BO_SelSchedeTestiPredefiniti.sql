CREATE PROCEDURE [dbo].[MSP_BO_SelSchedeTestiPredefiniti](@CodTestoPredefinito VARCHAR(20))  
AS
BEGIN

	


	DECLARE @sCodEntita VARCHAR(20)

	DECLARE @sTabellaEntita VARCHAR(255)
	DECLARE @sSQL VARCHAR(MAX)

		SET @sCodEntita =(SELECT TOP 1 CodEntita FROM T_TestiPredefiniti WHERE Codice=@CodTestoPredefinito)
	
				CREATE TABLE #tmpUATesto(CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS)
		
		INSERT INTO #tmpUATesto(CodUA)
	SELECT DISTINCT  
			f.CodUA COLLATE Latin1_General_CI_AS			
		FROM
			(
				(SELECT 
					CodUA 
				 FROM 
					T_AssUAEntita 
				WHERE 
					CodVoce = @CodTestoPredefinito AND     					CodEntita = 'TST') AS T		 			
			CROSS APPLY dbo.MFW_UAFiglie_SelectByCodUA(t.CodUA) as f
			)
			 
					
	CREATE TABLE #tmpUAEntita(CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS)				 	
	 
	SET @sSQL ='INSERT INTO #tmpUAEntita(CodUA)
				SELECT DISTINCT  
					f.CodUA COLLATE Latin1_General_CI_AS
					FROM
						(
							(SELECT 
								CodUA 
							 FROM 
								T_AssUAEntita 
							WHERE 								
								CodEntita = ''' + @sCodEntita + ''') AS T		 			
						CROSS APPLY dbo.MFW_UAFiglie_SelectByCodUA(t.CodUA) as f
						)
				WHERE
					f.CodUA  IN (SELECT CodUA FROM #tmpUATesto)
				'
	EXEC (@sSQL)

	
				
	IF (@sCodEntita='ALA') SET @sTabellaEntita='T_TipoAlertAllergiaAnamnesi'		
	IF (@sCodEntita='ALG') SET @sTabellaEntita='T_TipoAlertGenerico'		
	IF (@sCodEntita='DCL') SET @sTabellaEntita='T_TipoVoceDiario'
	IF (@sCodEntita='PVT') SET @sTabellaEntita='T_TipoParametroVitale'	
	IF (@sCodEntita='WKI') SET @sTabellaEntita='T_TipoTaskInfermieristico'			
	IF (@sCodEntita='SCH') SET @sTabellaEntita=''		

	
	SET @sSQL ='SELECT 
					Q.Codice,
					Q.Descrizione,
					Q.ID,
										Q.Struttura.query(''(/DcScheda/Sezioni/Item/Value/DcSezione/Voci/Item/Value/DcVoce)[ID=sql:column("Q.ID")]'').value(''(/DcVoce/Descrizione)[1]'',''VARCHAR(255)'') AS DescrizioneCampo
			
				FROM
					(SELECT '
	
					IF (@sCodEntita <> 'SCH')
					BEGIN		
						SET @sSQL = @sSQL + '	TP.Codice, 
												TP.Descrizione, '
					END
					ELSE
					BEGIN
						SET @sSQL = @sSQL + '	S.Codice, 
												S.Descrizione, '
					END

					SET @sSQL = @sSQL + '
									SVL.DCLAYOUT.value(''(ID)[1]'',''varchar(50)'') AS ID,
									SV.Struttura
								FROM T_SchedeVersioni SV
									 INNER JOIN T_Schede S On SV.CodScheda = S.Codice
										  CROSS APPLY SV.Layout.nodes(''/DcSchedaLayouts/Layouts/Item/Value/DcLayout'') As SVL(DCLAYOUT)
								'
		
												IF (@sCodEntita <> 'SCH')
							BEGIN
																SET @sSQL = @sSQL + '	INNER JOIN ' + @sTabellaEntita + ' TP ON (TP.CodScheda=S.Codice) '
							END

												SET @sSQL = @sSQL + '
								WHERE 
									 ISNULL(SV.FlagAttiva, 0) = 1
									 AND ISNULL(SV.Pubblicato,0) = 1
									 AND SV.DtValI <= GETDATE()
									 AND ISNULL(SV.DtValF,convert(datetime,''01-01-2100'')) >= GETDATE()
									 AND SVL.DCLAYOUT.value(''(TipoVoce)[1]'',''varchar(50)'') = ''TestoRtf''
								'		

												IF (@sCodEntita <> 'SCH')
							BEGIN
																SET @sSQL = @sSQL + ' AND (S.CodEntita = ''' + @sCodEntita + ''' OR S.CodEntita2 = ''' + @sCodEntita + ''')
													  AND SchedaSemplice = 1 '
							END
						ELSE
							BEGIN
																SET @sSQL = @sSQL + ' AND ISNULL(SchedaSemplice,0) = 0 '
							END					  
	
						

												IF (@sCodEntita <> 'SCH')
						BEGIN
							  SET @sSQL = @sSQL + ' 
									 AND 
										S.Codice IN 
												  (SELECT  TV.CodScheda                                  
												   FROM  
													' + @sTabellaEntita + ' TV
														INNER JOIN T_AssUAEntita AU
																	   ON (AU.CodVoce = TV.Codice AND
																		   AU.CodEntita =''' + @sCodEntita +'''
																		   )
														INNER JOIN #tmpUAEntita TE 
																ON (TE.CodUA =AU.CodUA)
													)        
								'
						END
						ELSE
						BEGIN
														  SET @sSQL = @sSQL + ' 
									 AND 
									 S.Codice IN  (SELECT  AU.CodVoce                               
												   FROM
													T_AssUAEntita AU
														INNER JOIN #tmpUAEntita TE 
																ON (TE.CodUA =AU.CodUA)
												   WHERE AU.CodEntita =''SCH''														   	
												   )        
								'
						END

					SET @sSQL = @sSQL + ') AS Q'
	          
		SET @sSQL = @sSQL + ' ORDER BY Q.Descrizione, Q.Codice, Q.ID'

	
		EXEC (@sSQL)
END