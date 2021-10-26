CREATE PROCEDURE [dbo].[MSP_SelProtocolliAttivitaTipoTask](@xParametri AS XML )
AS
BEGIN
	
	
			
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	
	DECLaRE @sCodProtocolloAttivita	AS VARCHAR(20)
	
	SET @sCodProtocolloAttivita=(SELECT	TOP 1 ValoreParametro.CodProtocolloAttivita.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodProtocolloAttivita') as ValoreParametro(CodProtocolloAttivita))	
				
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo)) 
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))				  	
											
				
				
					
	
			SELECT 
				@sCodProtocolloAttivita AS CodProtocolloAttivita,
				TS.CodProtocolloAttivitaTempi,
				TT.DeltaGiorni,
				TT.DeltaOre,
				TT.DeltaMinuti,
				TT.DeltaAlle00,
				CodTipoTaskInfermieristico,
				TTI.Descrizione AS DescrizioneTipoTask
		FROM	
				T_ProtocolliAttivitaTempiTipoTask TS 
					INNER JOIN
						T_ProtocolliAttivitaTempi TT
							ON TS.CodProtocolloAttivitaTempi=TT.Codice	
					LEFT JOIN T_TipoTaskInfermieristico TTI
						ON TS.CodTipoTaskInfermieristico=TTI.Codice
						
					
		WHERE 
				CodProtocolloAttivita=@sCodProtocolloAttivita
			
			
		RETURN 0

		

END