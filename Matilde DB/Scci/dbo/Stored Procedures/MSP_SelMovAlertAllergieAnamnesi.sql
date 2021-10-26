CREATE PROCEDURE [dbo].[MSP_SelMovAlertAllergieAnamnesi](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDAllergieAnamnesi AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoAlert AS VARCHAR(1800)
	DECLARE @sCodTipoAlertAllergiaAnamnesi AS VARCHAR(1800)
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @bVisualizzazioneSintetica AS BIT
	
		DECLARE @nTemp AS INTEGER
	DECLARE @bAnnulla AS BIT
	DECLARE @bModifica AS BIT
		
		DECLARE @sGUID AS VARCHAR(Max)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
						  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAllergieAnamnesi.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAllergieAnamnesi') as ValoreParametro(IDAllergieAnamnesi))			
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDAllergieAnamnesi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
						  		  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodTipoAlertAllergiaAnamnesi=''
	SELECT	@sCodTipoAlertAllergiaAnamnesi =  @sCodTipoAlertAllergiaAnamnesi +
														CASE 
								WHEN @sCodTipoAlertAllergiaAnamnesi='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoAlertAllergiaAnamnesi.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoAlertAllergiaAnamnesi') as ValoreParametro(CodTipoAlertAllergiaAnamnesi)
						 
	SET @sCodTipoAlertAllergiaAnamnesi=LTRIM(RTRIM(@sCodTipoAlertAllergiaAnamnesi))
	IF	@sCodTipoAlertAllergiaAnamnesi='''''' SET @sCodTipoAlertAllergiaAnamnesi=''
	SET @sCodTipoAlertAllergiaAnamnesi=UPPER(@sCodTipoAlertAllergiaAnamnesi)
	
	
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

		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
	
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
							
				
			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='AlertAA_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0	
		
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='AlertAA_Modifica'	
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
					ID,
					IDPaziente,
					DataEvento,
					DataEventoUTC,
					MS.AnteprimaRTF,
					M.CodTipoAlertAllergiaAnamnesi AS CodTipo,
					T.Descrizione AS DescrTipo, 
					M.CodStatoAlertAllergiaAnamnesi As CodStato,
					M.CodSistema,
					M.IDSistema,
					M.IDGruppo,
					S.Descrizione As DescrStato,
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					M.DataUltimaModifica,
					M.DataUltimaModificaUTC,
					M.CodUtenteUltimaModifica,
					CONVERT(INTEGER,
						CASE 
							WHEN ISNULL(CodStatoAlertAllergiaAnamnesi,'''')=''AN'' THEN 0
							ELSE 1
						END &	
						CONVERT(BIT, + ' + CONVERT(CHAR(1), @bAnnulla) + ')
					) AS PermessoAnnulla,
					CONVERT(INTEGER,
						CASE 
							WHEN ISNULL(CodStatoAlertAllergiaAnamnesi,'''')=''AN'' THEN 0
							ELSE 1
						END &	
						CONVERT(BIT, + ' + CONVERT(CHAR(1), @bModifica) + ')
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
					MS.AnteprimaTXT,						
					MS.AnteprimaRTF
					'
		IF @bDatiEstesi=1
		SET @sSQL=@sSQL + ' ,I.Icona'
					
	

	SET @sSQL=@sSQL + ' 						
				FROM 
					T_MovAlertAllergieAnamnesi	M					
						LEFT JOIN T_TipoAlertAllergiaAnamnesi T
								ON (M.CodTipoAlertAllergiaAnamnesi=T.Codice)
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
								 FROM
									T_MovSchede WITH (NOLOCK, INDEX(IX_IDEntitaCodEntitaCodSchedaStoricizzataStato))
								 WHERE CodEntita=''ALA'' AND							
									Storicizzata=0 
								) AS MS
							ON (MS.IDEntita=M.ID AND
								MS.CodScheda=T.CodScheda)					
					'
	
	IF ISNULL(@bVisualizzazioneSintetica,0)=0	
						SET @sSQL=@sSQL + ' 												
						LEFT JOIN T_StatoAlertAllergiaAnamnesi S
							ON (M.CodStatoAlertAllergiaAnamnesi=S.Codice)
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)		
					  '
	IF 	@bDatiEstesi=1				  
		SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  CodTipo,
							  CodStato,							  
							  Icona256 AS Icona
							 FROM T_Icone WITH (NOLOCK)
							 WHERE CodEntita=''ALA''
							) AS I
								ON (M.CodTipoAlertAllergiaAnamnesi=I.CodTipo AND 	
									M.CodStatoAlertAllergiaAnamnesi=I.CodStato
									)								
					'				
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND 
								(M.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +'''
								 OR
								 								 M.IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias WITH (NOLOCK)
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias WITH (NOLOCK)
													 WHERE IDPazienteVecchio=''' + convert(varchar(50),@uIDPaziente) +'''
													)
											)
										
								 )			
								'
									
									
									
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @uIDAllergieAnamnesi IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDAllergieAnamnesi) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		

		IF @sCodStatoAlert NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoAlertAllergiaAnamnesi IN ('+ @sCodStatoAlert + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
			IF @sCodStatoAlert NOT IN ('''Tutti''')
				BEGIN
										SET @sTmp=  ' AND 			
									 M.CodStatoAlertAllergiaAnamnesi =''AT''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
			ELSE
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoAlertAllergiaAnamnesi <> ''CA''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
		END					
		
	IF @sCodTipoAlertAllergiaAnamnesi NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoAlertAllergiaAnamnesi IN ('+ @sCodTipoAlertAllergiaAnamnesi + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	
		
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	SET @sSQL=@sSQL +' ORDER BY M.DataEvento,IDNum DESC '		
 	
	EXEC (@sSQL)
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
			
END