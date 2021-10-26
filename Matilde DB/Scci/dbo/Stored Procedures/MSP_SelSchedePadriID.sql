CREATE PROCEDURE [dbo].[MSP_SelSchedePadriID](@xParametri AS XML)

AS
BEGIN
	
	
		DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	
		DECLARE @sGUID AS VARCHAR(Max)
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))
					  
	SET @sCodScheda=ISNULL(@sCodScheda,'')	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
				
	;	
	WITH GerarchiaSchede(IDSchedaPadre,IDSchedaFiglia)
	AS
	( SELECT 		
			M.IDSchedaPadre,
			M.ID AS IDSchedaFiglia			
	  FROM T_MovSchede M
	  WHERE 
			CodStatoScheda<>'CA' AND
			Storicizzata=0 AND
			CodScheda=@sCodScheda AND
			IDPaziente=@uIDPaziente
			
	  UNION ALL
	  SELECT 		
			M2.IDSchedaPadre,
			M2.ID AS IDSchedaFiglia			
	  FROM T_MovSchede M2 
			INNER JOIN GerarchiaSchede G
				ON M2.ID=G.IDSchedaPadre
	  WHERE 
			CodStatoScheda<>'CA' AND
			Storicizzata=0 			
	)
	SELECT	
		DISTINCT   
		IDSchedaFiglia AS IDScheda
	FROM  GerarchiaSchede G		
						
END