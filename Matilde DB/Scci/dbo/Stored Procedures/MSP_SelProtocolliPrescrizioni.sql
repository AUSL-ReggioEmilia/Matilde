CREATE PROCEDURE [dbo].[MSP_SelProtocolliPrescrizioni](@xParametri AS XML )
AS
BEGIN
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodice AS Varchar(20)
	DECLARE @bIcona AS Bit
	DECLARE @bModelliPrescrizioni AS Bit
	DECLARE @sDescrizione AS VARCHAR(500)				

		SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	

		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 

		SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  	
	
		SET @sCodice=(SELECT	TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))
	
		SET @bIcona=(SELECT TOP 1 ValoreParametro.Icona.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/Icona') as ValoreParametro(Icona))
	
		SET @bModelliPrescrizioni=(SELECT TOP 1 ValoreParametro.ModelliPrescrizioni.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/ModelliPrescrizioni') as ValoreParametro(ModelliPrescrizioni))

		SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))						 
	SET @sDescrizione= ISNULL(@sDescrizione,'')
	SET @sDescrizione=LTRIM(RTRIM(@sDescrizione))	
			
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='PRP'
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
		
			
	
				
	SELECT 
			D.Codice,
			D.Descrizione,

			CASE 
				WHEN ISNULL(@bIcona,0)=0 THEN NULL
				ELSE
					D.Icona					
				END
			AS Icona,
			CASE 
				WHEN ISNULL(@bModelliPrescrizioni,0)=0 THEN NULL
				ELSE
					 D.ModelliPrescrizioni					
				END
			AS ModelliPrescrizioni,
			ISNULL(DataOraInizioObbligatoria,0) AS DataOraInizioObbligatoria,
			ISNULL(VersioneModello,1) AS VersioneModello
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
					INNER JOIN T_ProtocolliPrescrizioni D							ON D.Codice=W.Codice	
					
			WHERE 
				D.Codice=ISNULL(@sCodice,D.Codice)							
			ORDER BY D.Descrizione						

		
	
		DROP TABLE #tmpUA			
	RETURN 0
END