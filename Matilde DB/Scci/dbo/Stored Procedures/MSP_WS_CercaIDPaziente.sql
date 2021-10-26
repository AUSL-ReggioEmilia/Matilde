CREATE PROCEDURE [dbo].[MSP_WS_CercaIDPaziente](@xParametri AS XML)
AS
BEGIN
	

					
	DECLARE @sCodSAC AS VARCHAR(50) 
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
							  		
		SET @sCodSAC=(SELECT	TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))						 
	SET @sCodSAC= LTRIM(RTRIM(ISNULL(@sCodSAC,'')))
		
				
			
				

	
	SELECT 		
		TOP 1 ID AS IDPaziente
	FROM T_Pazienti P
	WHERE 		
		P.CodSAC=@sCodSAC
	           
	RETURN 0
	
END