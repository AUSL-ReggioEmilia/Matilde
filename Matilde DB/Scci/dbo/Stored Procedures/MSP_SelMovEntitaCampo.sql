CREATE PROCEDURE [dbo].[MSP_SelMovEntitaCampo](@xParametri XML)
AS 
BEGIN
	 
	
						
		DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodCampoScheda AS VARCHAR(50)
			
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sNomeTabella AS VARCHAR(255)
		
		SET  @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
			  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
		SET @uIDEntita=NULL		
	
		SET  @sCodCampoScheda=(SELECT TOP 1 ValoreParametro.CodCampoScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodCampoScheda') as ValoreParametro(CodCampoScheda))						  	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
										
				
				
		IF @sCodEntita='APP' SET @sNomeTabella='T_MovAppuntamenti'
	IF @sCodEntita='PVT' SET @sNomeTabella='T_MovParametriVitali'	
	
	SET @sSQL='
			SELECT 
			''' + + CONVERT(VARCHAR(50),@uIDEntita) + ''' AS IDEntita,
			T1.CodCampoScheda.value(''(Value)[1]'', ''varchar(50)'') AS CodValore,
			T1.CodCampoScheda.value(''(Transcodifica)[1]'', ''varchar(500)'') AS Transcodifica			
		FROM 					
			' + @sNomeTabella + ' TAB										
			INNER JOIN T_MovSchede SCH
				ON (SCH.IDEntita=TAB.ID AND
					SCH.CodEntita=''' + @sCodEntita + ''' AND
					SCH.Storicizzata=0)	
			CROSS APPLY 
				SCH.Dati.nodes(''/DcSchedaDati/Dati/Item/Value/DcDato[(ID[1]="' + CONVERT(VARCHAR(50),@sCodCampoScheda) +'")]'') as T1(CodCampoScheda)						
		WHERE
			TAB.ID=''' + CONVERT(VARCHAR(50),@uIDEntita) + ''''

        EXEC (@sSQL)
			
						
		
					
										
	
END