CREATE PROCEDURE [dbo].[MSP_SelIcona](@xParametri XML)
AS
BEGIN
	
		
	
				 	
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @sCodTipo VARCHAR(20)
	DECLARE @sCodStato VARCHAR(20)
	
	DECLARE @bDatiEstesi AS Bit			
								
	DECLARE @nIDIconaDefault AS INTEGER
	DECLARE @nRecordTrovati AS INTEGER
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))		
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,1)
					  
	SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	
	SET @sCodTipo=(SELECT TOP 1 ValoreParametro.CodTipo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipo') as ValoreParametro(CodTipo))	
	
	SET @sCodStato=(SELECT TOP 1 ValoreParametro.CodStato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStato') as ValoreParametro(CodStato))	
										
	
			
	SET @nRecordTrovati=(SELECT	
							COUNT(*)
						FROM 
							T_Icone WITH (NOLOCK)
						WHERE
							CodEntita = ISNULL(@sCodEntita,'')
							AND CodTipo = ISNULL(@sCodTipo,'')
							AND CodStato = ISNULL(@sCodStato,'')
						)	
	IF 	@nRecordTrovati>0
	BEGIN		
		SELECT	
		  IDNum
		  ,CodEntita
		  ,CodTipo
		  ,CodStato
		  ,Descrizione
		  ,CASE 
			WHEN @bDatiEstesi=1 THEN Icona16
			ELSE NULL
		  END	
		  AS Icona16
		  ,
		  CASE 
			WHEN @bDatiEstesi=1 THEN Icona32
			ELSE NULL
		  END	
		  AS Icona32
		  ,
		   CASE 
			WHEN @bDatiEstesi=1 THEN Icona48
			ELSE NULL
		  END	
		  AS Icona48
		  ,
		  CASE 
			WHEN @bDatiEstesi=1 THEN Icona256
			ELSE NULL
		  END 	
		  AS Icona256	    
		FROM 
			T_Icone WITH (NOLOCK)
		WHERE
			CodEntita = ISNULL(@sCodEntita,'')
			AND CodTipo = ISNULL(@sCodTipo,'')
			AND CodStato = ISNULL(@sCodStato,'')
	END
	ELSE
	BEGIN
	
		SET @nIDIconaDefault=(SELECT TOP 1 IDNum
					  FROM T_Icone WITH (NOLOCK)
						WHERE CodTipo='_DEFAULT_' AND CodEntita='EVC' AND CodStato='')
	 
		SELECT	
		  @nIDIconaDefault AS IDNum,
		  ISNULL(@sCodEntita,'') AS CodEntita,
		  ISNULL(@sCodTipo,''),
		  ISNULL(@sCodStato,''),
		  '' AS Descrizione,
		  NULL AS  Icona16,
		  NULL AS  Icona32,
		  NULL AS  Icona48,
		  NULL AS Icona256	    
		
	END
	
				
END