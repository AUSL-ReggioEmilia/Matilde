CREATE PROCEDURE [dbo].[MSP_InsMovRelazioniEntita](@xParametri XML)
AS
BEGIN
		
	
							
												
								
		DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER	
	DECLARE @sCodEntitaCollegata AS VARCHAR(20)
	DECLARE @uIDEntitaCollegata AS UNIQUEIDENTIFIER	
	DECLARE @sAttributo1 AS VARCHAR(200)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')

		SET @uIDEntita=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','UNIQUEIDENTIFIER')
							  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
								  
		SET @sCodEntitaCollegata=(SELECT TOP 1 ValoreParametro.CodEntitaCollegata.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaCollegata') as ValoreParametro(CodEntitaCollegata))	
	SET @sCodEntitaCollegata=ISNULL(@sCodEntitaCollegata,'')
	
		SET @uIDEntitaCollegata=(SELECT TOP 1 ValoreParametro.IDEntitaCollegata.value('.','UNIQUEIDENTIFIER')
							  FROM @xParametri.nodes('/Parametri/IDEntitaCollegata') as ValoreParametro(IDEntitaCollegata))
	
		SET @sAttributo1=(SELECT TOP 1 ValoreParametro.Attributo1.value('.','VARCHAR(200)')
					  FROM @xParametri.nodes('/Parametri/Attributo1') as ValoreParametro(Attributo1))	
	
	
					
	
							INSERT INTO T_MovRelazioniEntita(	
					   CodEntita				  
					  ,IDEntita
					  ,CodEntitaCollegata
					  ,IDEntitaCollegata
					  ,Attributo1					  					  
				  )
		VALUES		
				(@sCodEntita,
				 @uIDEntita,
				 @sCodEntitaCollegata,
				 @uIDEntitaCollegata,
				 @sAttributo1
				)
				
		RETURN 0
END