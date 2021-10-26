CREATE PROCEDURE [dbo].[MSP_SelFUTRiga](@xParametri AS XML)
AS
BEGIN
	
	
	
	
				
	DECLARE @sCodSezione VARCHAR(20)
	DECLARE @sCodVoce VARCHAR(600)		
	
		DECLARE @sCodOutput VARCHAR(700)
	DECLARE @sCodSottoClasse VARCHAR(700)
	DECLARE @sTmp VARCHAR(700)
	
		SET @sCodSezione=(SELECT TOP 1 ValoreParametro.CodSezione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione))	
	
		SET @sCodVoce=(SELECT TOP 1 ValoreParametro.CodVoce.value('.','VARCHAR(600)')
					  FROM @xParametri.nodes('/Parametri/CodVoce') as ValoreParametro(CodVoce))
					  
					  
		SET @sCodOutput=NULL
	SET @sCodSottoClasse=''
	SET @sTmp=''

		IF ISNULL(@sCodSezione,'')='PFM'
	BEGIN
		SET @sCodSottoClasse=(SELECT TOP 1 M.Sottoclasse
							  FROM T_MovTaskInfermieristici	M							
							  WHERE
																		M.CodStatoTaskInfermieristico NOT IN ('TR','AN','CA') AND								
									ISNULL(M.CodSistema,'') ='PRF' AND
									ISNULL(M.IDGruppo,'') = @sCodVoce
							  ORDER BY 	M.Sottoclasse ASC
							  )		
												
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sCodVoce,'') + ISNULL(@sCodSottoClasse,'')
		
	END
	
	IF ISNULL(@sCodSezione,'') IN ('PFA')
	BEGIN												
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sCodVoce,'') 
		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('DCM','DCI')
	BEGIN				
		SET @sTmp=(SELECT TOP 1 M.CodTipoVoceDiario
							  FROM T_MovDiarioClinico M						
							  WHERE
									ID=@sCodVoce
							  )		
							  								
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sTmp,'')		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('PVT')
	BEGIN				
		SET @sTmp=(SELECT TOP 1 M.CodTipoParametroVitale
							  FROM T_MovParametriVitali M						
							  WHERE
									ID=@sCodVoce
							  )		
							  								
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sTmp,'')
		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('WKI')
	BEGIN				
		SET @sTmp=(SELECT TOP 1 M.CodTipoTaskInfermieristico
							  FROM T_MovTaskInfermieristici M						
							  WHERE
									ID=@sCodVoce
							  )		
							  								
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sTmp,'')		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('APP')
	BEGIN				
		SET @sTmp=(SELECT TOP 1 M.CodAgenda
							  FROM T_MovAppuntamentiAgende M						
							  WHERE
									IDAppuntamento=@sCodVoce AND
									M.CodStatoAppuntamentoAgenda NOT IN ('CA')
							  )		
							  								
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sTmp,'')		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('OE')
	BEGIN				
		SET @sTmp=(SELECT TOP 1 ME.CodTipoOrdine
							  FROM T_MovOrdini M
							  		INNER JOIN T_MovOrdiniEroganti ME
										ON (M.ID=ME.IDOrdine)						
									LEFT JOIN T_TipoOrdine T
										ON (ME.CodTipoOrdine = T.Codice)	
							  WHERE
									M.IDOrdineOE=@sCodVoce
							  )		
							  								
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + ISNULL(@sTmp,'')		
	END	
	
		IF ISNULL(@sCodSezione,'') IN ('ADT')
	BEGIN												
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + 'T'
		
	END	
			
		IF ISNULL(@sCodSezione,'') IN ('NTG')
	BEGIN												
		SET @sCodOutput=ISNULL(@sCodSezione,'') + '_' + 'N'		
	END	
					
	SELECT @sCodOutput AS CodVoce
	RETURN 0
END