CREATE PROCEDURE [dbo].[MSP_EsistePazienteDaSAC](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @uCodSAC AS UNIQUEIDENTIFIER	
	DECLARE @nConta AS INTEGER
	DECLARE @nRet AS INTEGER
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	
		DECLARE @sGUID AS VARCHAR(Max)

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))
	IF 	ISNULL(@sGUID,'') <> '' SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
				
			
	SET @nRet=0
	SET @uIDPaziente=NULL
	
	IF @uCodSAC IS NOT NULL
		BEGIN
						SET @nConta=
				(SELECT 
					COUNT(*)					
				FROM T_Pazienti 
				WHERE CodSAC=@uCodSAC)
				
			IF @nConta>0 
				BEGIN
					SET @nRet=1	
					SET @uIDPaziente=(SELECT TOP 1 ID FROM T_Pazienti WHERE CodSAC=@uCodSAC)
				END	
			ELSE
				SET @nRet=0
		END
		
	ELSE
				SET @nRet=0
		
	SELECT 	@nRet AS Esiste, @uIDPaziente AS IDPaziente
							
	RETURN 0
END