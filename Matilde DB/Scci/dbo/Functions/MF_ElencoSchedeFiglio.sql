CREATE FUNCTION [dbo].[MF_ElencoSchedeFiglio](@uIDScheda AS UNIQUEIDENTIFIER)
RETURNS @tblSchedeFiglio TABLE
	(Livello INTEGER,
	 IDSchedaPadre UNIQUEIDENTIFIER  NOT NULL,
	 IDSchedaFiglia UNIQUEIDENTIFIER NOT NULL,
	 CodUAFiglia VARCHAR(20)
	)
AS
BEGIN
	
	DECLARE @nLivello AS INTEGER
	SET @nLivello = 1

	;
	WITH GerarchiaSCH(Livello, IDSchedaPadre,IDSchedaFiglia,CodUAFiglia)
	AS
	( SELECT 	
			 @nLivello,	
			 SP.IDSchedaPadre,
			 SP.ID AS IDSchedaFiglia,
			 SP.CodUA AS CodUAFiglia
	  FROM T_MovSchede SP WITH (INDEX(IX_IDSchedaPadreCodStatoStoricizzata))	  
	  WHERE 
		SP.CodStatoScheda <> 'CA' AND
		SP.Storicizzata =0 AND
		IDSchedaPadre = @uIDScheda
			  				
	  UNION ALL
	  SELECT 
			 G.Livello +1 AS Livello,
			 SCF.IDSchedaPadre, 
			 SCF.ID AS IDSchedaFiglia,
			 SCF.CodUA AS CodUAFiglia
	  FROM T_MovSchede SCF WITH (INDEX(IX_SchedaPadre))
		INNER JOIN GerarchiaSCH G
			ON (SCF.IDSchedaPadre=G.IDSchedaFiglia)
	   WHERE 
			SCF.CodStatoScheda <> 'CA' AND
			SCF.Storicizzata =0 
	)		
	INSERT INTO @tblSchedeFiglio(Livello,IDSchedaPadre,IDSchedaFiglia,CodUAFiglia)
	SELECT 
		Livello,
		IDSchedaPadre,
		IDSchedaFiglia,
		CodUAFiglia	   
	FROM  GerarchiaSCH G
		
	RETURN 
END