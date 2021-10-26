CREATE PROCEDURE [dbo].[MSP_SelSchedePadri](@xParametri AS XML)

AS
BEGIN
	
	
		DECLARE @sCodScheda AS VARCHAR(20)
	
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))
					  
	SET @sCodScheda=ISNULL(@sCodScheda,'')	
	
				
	;	
	WITH GerarchiaSchede(CodSchedaPadre,CodSchedaFiglia)
	AS
	( SELECT 		
			CodSchedaPadre,
			CodScheda AS CodSchedaFiglia
	  FROM T_SchedePadri P		
	  WHERE CodScheda=@sCodScheda
			
	  UNION ALL
	  SELECT
			P2.CodSchedaPadre,
			P2.CodScheda AS CodSchedaFiglia
	  FROM			
		T_SchedePadri P2						
			INNER JOIN GerarchiaSchede G
				ON P2.CodScheda=G.CodSchedaPadre	
	)
	SELECT	   
		CodSchedaPadre AS CodScheda
	FROM  GerarchiaSchede G		
	UNION
	SELECT @sCodScheda	
				
END