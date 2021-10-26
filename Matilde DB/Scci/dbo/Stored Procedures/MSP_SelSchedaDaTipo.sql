
CREATE PROCEDURE MSP_SelSchedaDaTipo(@xParametri XML)
AS
BEGIN

	
		DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodTipo AS VARCHAR(20)

	
				
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')

		SET @sCodTipo=(SELECT TOP 1 ValoreParametro.CodTipo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipo') as ValoreParametro(CodTipo))	
	SET @sCodTipo=ISNULL(@sCodTipo,'')				  


		IF 	@sCodEntita='ALA'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoAlertAllergiaAnamnesi
		WHERE Codice=@sCodTipo	
	END
	
	
		IF 	@sCodEntita='ALG'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoAlertGenerico
		WHERE Codice=@sCodTipo	
	END
	
		IF 	@sCodEntita='APP'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoAppuntamento
		WHERE Codice=@sCodTipo	
	END
	
		IF 	@sCodEntita='DCL'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoVoceDiario
		WHERE Codice=@sCodTipo	
	END
	
		IF 	@sCodEntita='PRF'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoPrescrizione
		WHERE Codice=@sCodTipo	
	END
	
		IF 	@sCodEntita='PVT'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoParametroVitale
		WHERE Codice=@sCodTipo	
	END
	
		IF 	@sCodEntita='WKI'
	BEGIN
		SELECT CodScheda FROM 
			T_TipoTaskInfermieristico
		WHERE Codice=@sCodTipo	
	END
					  
END