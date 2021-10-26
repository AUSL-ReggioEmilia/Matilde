CREATE PROCEDURE [dbo].[MSP_SelProtocolliAttivitaTempi](@xParametri AS XML )
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
				Codice,
				CodProtocolloAttivita,
				Descrizione,
				DeltaGiorni,
				DeltaOre,
				DeltaMinuti,
				DeltaAlle00
		FROM	
				T_ProtocolliAttivitaTempi T	
	WHERE 
				T.CodProtocolloAttivita=@sCodProtocolloAttivita			
			
			
		RETURN 0

		

END