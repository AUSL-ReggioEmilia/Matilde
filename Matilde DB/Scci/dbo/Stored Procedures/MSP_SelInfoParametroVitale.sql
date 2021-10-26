
CREATE PROCEDURE [dbo].[MSP_SelInfoParametroVitale](@xParametri AS XML )
AS
BEGIN
	
	
				 
	DECLARE @sCodTipoParametroVitale AS Varchar(20)
				
	SET @sCodTipoParametroVitale=(SELECT TOP 1 ValoreParametro.CodTipoParametroVitale.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoParametroVitale') as ValoreParametro(CodTipoParametroVitale))
					
	SET @sCodTipoParametroVitale=ISNULL(@sCodTipoParametroVitale,'') 
			 
					  		
				
	SELECT * FROM T_TipoParametroVitale
	WHERE Codice=@sCodTipoParametroVitale
	
	RETURN 0
END