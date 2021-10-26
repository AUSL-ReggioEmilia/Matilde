CREATE PROCEDURE [dbo].[MSP_AggParametriVitaliGrafici](@xParametri AS XML)
AS
BEGIN
	
		DECLARE @uIDParametroVitale AS UNIQUEIDENTIFIER

		DECLARE @uIDScheda AS UNIQUEIDENTIFIER
		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sCodTipoParametroVitale AS VARCHAR(20)
	DECLARE @xCampiGrafici AS XML
	DECLARE @xDati AS XML
	
	DECLARE @sRiga AS VARCHAR(50)
	DECLARE @sCampo AS VARCHAR(50)
		

							
		IF @xParametri.exist('(//IDParametroVitale)')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDParametroVitale.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDParametroVitale') as ValoreParametro(IDParametroVitale))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDParametroVitale=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END
		
						
	SET @sCodTipoParametroVitale=(SELECT CodTipoParametroVitale FROM T_MovParametriVitali WHERE ID=@uIDParametroVitale)
	SET @sCodTipoParametroVitale=ISNULL(@sCodTipoParametroVitale,'')
	
		SET @xCampiGrafici=(SELECT CampiGrafici FROM T_TipoParametroVitale WHERE Codice=@sCodTipoParametroVitale)


		SELECT @uIDScheda=ID,
		   @xDati=Dati
	FROM T_MovSchede 
	WHERE 
		CodEntita='PVT' AND
		IDEntita=@uIDParametroVitale AND
		Storicizzata=0 AND
		CodStatoScheda<> 'CA'
		
	IF @xCampiGrafici.exist('/Parametri/DatiClinici')=1
		BEGIN
			SET @sRiga=(SELECT TOP 1 ValoreParametro.Riga.value('.','VARCHAR(50)')
							  FROM @xCampiGrafici.nodes('/Parametri/Parametro') as ValoreParametro(Riga))
			SET @sCampo=(SELECT TOP 1 ValoreParametro.Campo.value('.','VARCHAR(50)')
							  FROM @xCampiGrafici.nodes('/Parametri/Parametro') as ValoreParametro(Campo))			
		END									
							
			 	 
	SELECT 	@sCampo,@sRiga,@xDati 	 
	RETURN 0
	
END