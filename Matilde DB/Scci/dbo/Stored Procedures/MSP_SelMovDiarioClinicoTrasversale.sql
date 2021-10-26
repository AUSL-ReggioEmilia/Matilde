
CREATE PROCEDURE [dbo].[MSP_SelMovDiarioClinicoTrasversale](@xParametri AS XML)
AS
BEGIN
	

	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @sIDDiarioClinico AS VARCHAR(MAX)
	DECLARE @sCodStatoDiario AS VARCHAR(1800)
	DECLARE @sCodUtente AS VARCHAR(100)
	
		DECLARE @sIDPaziente AS VARCHAR(50)
	DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> ''
		BEGIN 			
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			SET @sIDPaziente=@sGUID
		END
	ELSE	
		BEGIN			
			SET @uIDPaziente=NULL
			SET @sIDPaziente=''
		END	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDDiarioClinico.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDDiarioClinico') as ValoreParametro(IDDiarioClinico))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @sIDDiarioClinico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
		SET @sIDDiarioClinico=NULL
		
		SET @sIDDiarioClinico=''
	SELECT	@sIDDiarioClinico =  @sIDDiarioClinico +
														CASE 
								WHEN @sIDDiarioClinico='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDDiarioClinico.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDDiarioClinico') as ValoreParametro(IDDiarioClinico)
						 
	SET @sIDDiarioClinico=LTRIM(RTRIM(@sIDDiarioClinico))
	IF	@sIDDiarioClinico='''''' SET @sIDDiarioClinico=''
	SET @sIDDiarioClinico=UPPER(@sIDDiarioClinico)		
		
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	
		SET @sCodStatoDiario=(SELECT TOP 1 ValoreParametro.CodStatoDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoDiario') as ValoreParametro(CodStatoDiario))
	SET @sCodStatoDiario=ISNULL(@sCodStatoDiario,'IC')				
					  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
				
				
							   
						
					
	SET @sSQL='SELECT
					M.ID,
					P.IDPaziente,				
					M.IDEpisodio,
					M.IDTrasferimento,
					TRA.IDCartella,
						ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
						CASE
							WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
							ELSE ''''
						END +
						'' - '' + ISNULL(UA.Descrizione,'''') + 
						'' - '' + ISNULL(EPI.NumeroNosologico,'''') 
					AS DescrPaziente,
					M.DataEvento,
					M.DataEventoUTC,					
					M.CodTipoDiario AS CodTipoDiario,
					T.Descrizione AS DescrTipoDiario,
					M.CodTipoVoceDiario AS CodTipo,
					TV.Descrizione AS DescrTipo, 					 					
					M.CodTipoRegistrazione,
					TR.Descrizione As DescrTipoRegistrazione,
					M.CodStatoDiario AS CodStato,
					S.Descrizione As DescrStato,					
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					M.DataInserimento,
					M.DataInserimentoUTC, 	
					M.DataValidazione,
					M.DataValidazioneUTC,
					M.DataAnnullamento,
					M.DataAnnullamentoUTC,
					M.CodUA,
					MS.IDScheda,
					MS.CodScheda,
					MS.Versione,
					MS.AnteprimaRTF,
					CASE 
						WHEN  (''' + @sIDPaziente + '''='''') AND C.CodStatoCartella=''AP'' THEN 1 
						WHEN  (''' + @sIDPaziente + '''<>'''') AND
								 P.IDPaziente = '''  + @sIDPaziente + ''' 
								 AND C.CodStatoCartella=''AP''
								 THEN 1
						ELSE 0
					END AS PermessoModifica,
					CASE
						WHEN ISNULL(FUA.CodUA,'''')='''' THEN 0
						WHEN ISNULL(FUA.CodUA,'''')<>'''' AND C.CodStatoCartella=''AP'' THEN 1
						ELSE 0
					END AS PermessoUAFirma
FROM					
					 T_MovDiarioClinico	M						
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_MovEpisodi EPI
							ON (M.IDEpisodio=EPI.ID)						
						LEFT JOIN T_MovTrasferimenti TRA
							ON (M.IDTrasferimento=TRA.ID)
						LEFT JOIN T_MovCartelle C
							ON (TRA.IDCartella=C.ID)		
						LEFT JOIN T_UnitaAtomiche UA
							ON (TRA.CodUA=UA.Codice)		
						LEFT JOIN T_TipoDiario T
							ON (M.CodTipoDiario=T.Codice)
						LEFT JOIN T_TipoVoceDiario TV
							ON (M.CodTipoVoceDiario=TV.Codice)								
						LEFT JOIN T_StatoDiario S
							ON (M.CodStatoDiario=S.Codice)	
						LEFT JOIN T_TipoRegistrazione TR
							ON (M.CodTipoRegistrazione=TR.Codice)																	
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita=''DCL'' AND							
									Storicizzata=0 AND
									CodStatoScheda <> ''CA''
								) AS MS
							ON MS.IDEntita=M.ID	
						LEFT JOIN 
								(SELECT CodUA
								 FROM T_AssUAModuli
								 WHERE CodModulo=''FirmaD_Diario''
								 ) AS FUA
							ON TRA.CodUA=FUA.CodUA
						'

				
	SET @sWhere=''

		IF @sIDDiarioClinico NOT IN ('')
		BEGIN
			SET @sTmp=  ' AND 			
							 M.ID IN ('+ @sIDDiarioClinico + ')
						'  
			SET @sWhere= @sWhere + @sTmp								
		END
		
														
																				
		
		IF @sCodUtente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.CodUtenteRilevazione=''' + convert(varchar(100),@sCodUtente ) +''''
			SET @sWhere= @sWhere + @sTmp								
		END
		
		IF @sCodStatoDiario IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.CodStatoDiario=''' + convert(varchar(50),@sCodStatoDiario ) +''''
			IF @sCodStatoDiario='IC' 
				SET @sTmp= @sTmp + ' AND C.CodStatoCartella=''AP'''
						SET @sWhere= @sWhere + @sTmp								
		END

		
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	 PRINT @sSQL
	EXEC (@sSQL)
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp		  
END