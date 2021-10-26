CREATE PROCEDURE [dbo].[MSP_SelMovRelazioniEntita](@xParametri XML)
AS
BEGIN

		
	
				
		DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER	
	DECLARE @sCodEntitaFiltro AS VARCHAR(20)
	DECLARE @uIDEntitaFiltro AS UNIQUEIDENTIFIER


		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
	

		SET @uIDEntita=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','UNIQUEIDENTIFIER')
							  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))


		IF @xParametri.exist('/Parametri/CodEntitaFiltro')=1
		BEGIN
				SET @sCodEntitaFiltro=(SELECT TOP 1 ValoreParametro.CodEntitaFiltro.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaFiltro') as ValoreParametro(CodEntitaFiltro))	
				
		END
	
		IF @xParametri.exist('/Parametri/IDEntitaFiltro')=1
		BEGIN				
				SET @uIDEntita=(SELECT TOP 1 ValoreParametro.IDEntitaFiltro.value('.','UNIQUEIDENTIFIER')
							  FROM @xParametri.nodes('/Parametri/IDEntitaFiltro') as ValoreParametro(IDEntitaFiltro))
		END
		
				
	SELECT 
		CodEntitaCollegata AS CodEntita,
		IDEntitaCollegata AS IDEntita,
		Attributo1
	FROM T_MovRelazioniEntita		
	WHERE 
		  CodEntita=@sCodEntita AND
		  IDEntita=@uIDEntita AND		  
		  		  CodEntitaCollegata=ISNULL(@sCodEntitaFiltro,CodEntitaCollegata) AND
		  IDEntitaCollegata=ISNULL(@uIDEntitaFiltro,IDEntitaCollegata) 
	UNION
	SELECT CodEntita ,IDEntita,Attributo1
	FROM T_MovRelazioniEntita
	WHERE 
		CodEntitaCollegata=@sCodEntita AND
		IDEntitaCollegata=@uIDEntita AND
		CodEntita=ISNULL(@sCodEntitaFiltro,CodEntita) AND
		 IDEntita=ISNULL(@uIDEntitaFiltro,IDEntita) 
END