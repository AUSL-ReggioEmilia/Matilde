CREATE PROCEDURE [dbo].[MSP_CercaParametroVitalePrecedente](@xParametri XML)
AS
BEGIN

	
	

	
							
		DECLARE @sCodTipoParametroVitale  AS VARCHAR(20)	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	


		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @uIDParametroVitale AS UNIQUEIDENTIFIER
	
	
		SET @sCodTipoParametroVitale=(SELECT TOP 1 ValoreParametro.CodTipoParametroVitale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoParametroVitale') as ValoreParametro(CodTipoParametroVitale))	
	SET @sCodTipoParametroVitale=ISNULL(@sCodTipoParametroVitale,'')
	
		
		SET @uIDPaziente=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> ''
		 SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	
	
			
		SET @uIDParametroVitale=(SELECT TOP 1 M.ID
							 FROM T_MovParametriVitali M
								INNER JOIN T_MovPazienti P
									ON M.IDEpisodio=P.IDEpisodio
							 WHERE 
								P.IDPaziente=@uIDPaziente AND
								M.CodTipoParametroVitale=@sCodTipoParametroVitale AND															
								M.CodStatoParametroVitale NOT IN ('CA','AN')
							 ORDER BY 
								M.DataEvento DESC		
					)		
						    
	SELECT 	@uIDParametroVitale AS  ID
	
	RETURN 0	 	
			
END