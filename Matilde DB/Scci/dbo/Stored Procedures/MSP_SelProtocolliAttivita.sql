CREATE PROCEDURE [dbo].[MSP_SelProtocolliAttivita](@xParametri AS XML )
AS
BEGIN
	
	
			
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @bDatiEstesi AS Bit
	DECLaRE @sCodProtocolloAttivita	AS VARCHAR(20)
	
	SET @sCodProtocolloAttivita=(SELECT	TOP 1 ValoreParametro.CodProtocolloAttivita.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodProtocolloAttivita') as ValoreParametro(CodProtocolloAttivita))	
					
	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  	
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))					
				
					
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	

	
	IF ISNULL(@sCodProtocolloAttivita,'')=''
	BEGIN
	
				SET @sCodEntita='PRA'
	
								CREATE TABLE #tmpUA
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS ,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		DECLARE @xTmp AS XML
		
		SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
		
		INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
				
		
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    				
				
					
			SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
			SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
			
			IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1
			BEGIN		
								SELECT 
						W.Codice,
						D.Descrizione, 				
						CASE 
							WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
								ELSE
									D.Icona
							END AS Icona,
						D.Colore
						FROM						
							(SELECT DISTINCT 
								A.CodVoce AS Codice												
							 FROM					
								T_AssUAEntita A
									INNER JOIN #tmpUA T ON
										A.CodUA=T.CodUA	
									INNER JOIN T_AssRuoliAzioni Z
										ON (Z.CodEntita=A.CodEntita AND						
											Z.CodVoce=A.CodVoce AND
											Z.CodRuolo=@sCodRuolo AND
											Z.CodAzione =@sCodAzione)					
							 WHERE A.CodEntita=@sCodEntita			
							 ) AS W				
								INNER JOIN T_ProtocolliAttivita D										ON D.Codice=W.Codice	
						ORDER BY D.Descrizione							
			END
			ELSE
				BEGIN
										SELECT W.Codice,
						   D.Descrizione, 
						   CASE 
								WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
									ELSE
										D.Icona
								END AS Icona,
							D.Colore	
					FROM						
						(SELECT DISTINCT 
							A.CodVoce AS Codice												
						FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
									A.CodUA=T.CodUA					
						WHERE CodEntita=@sCodEntita			
						) AS W				
							INNER JOIN T_ProtocolliAttivita D										ON D.Codice=W.Codice	
					ORDER BY 	 D.Descrizione						  
				END		
			
						DROP TABLE #tmpUA			
		END
		ELSE
			BEGIN
									SELECT 
						D.Codice,
						D.Descrizione, 				
						CASE 
							WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
								ELSE
									D.Icona
							END AS Icona,
						D.Colore
						FROM	
							T_ProtocolliAttivita D	
						WHERE 
							D.Codice=@sCodProtocolloAttivita			
			END	
			
		RETURN 0

		

END