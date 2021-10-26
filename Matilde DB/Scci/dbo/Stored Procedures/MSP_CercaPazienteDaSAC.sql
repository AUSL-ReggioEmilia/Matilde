CREATE PROCEDURE [MSP_CercaPazienteDaSAC](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uCodSAC AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
	DECLARE @xParMovPaz  AS XML
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
	
	
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))
	IF 	ISNULL(@sGUID,'') <> '' SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		
					
	IF @uCodSAC IS NOT NULL
		BEGIN														
				SELECT TOP 1
					ID
				FROM T_Pazienti 
				WHERE CodSAC=@uCodSAC		    		   		
		END	
								
	RETURN 0
END