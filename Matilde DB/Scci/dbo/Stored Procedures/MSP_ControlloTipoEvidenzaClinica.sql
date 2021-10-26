CREATE PROCEDURE [dbo].[MSP_ControlloTipoEvidenzaClinica](@xParametri XML)
AS
BEGIN

		
	
		
		DECLARE @sCodTipoEvidenzaClinicaDWH AS VARCHAR(MAX)	
	
		

	
				
	SET @sCodTipoEvidenzaClinicaDWH=''
	SELECT	@sCodTipoEvidenzaClinicaDWH =  @sCodTipoEvidenzaClinicaDWH +
														CASE 
								WHEN @sCodTipoEvidenzaClinicaDWH='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoEvidenzaClinicaDWH.value('.','VARCHAR(MAX)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoEvidenzaClinicaDWH') as ValoreParametro(CodTipoEvidenzaClinicaDWH)
						 
	
	CREATE TABLE #tmpTEVC
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)

	 INSERT INTO #tmpTEVC(Codice)	
		SELECT ValoreParametro.CodTipoEvidenzaClinicaDWH.value('.','VARCHAR(20)') 
		FROM @xParametri.nodes('/Parametri/CodTipoEvidenzaClinicaDWH') as ValoreParametro(CodTipoEvidenzaClinicaDWH)			 
	
	CREATE INDEX Codice ON #tmpTEVC (Codice)    
	
				INSERT INTO T_TipoEvidenzaClinica(Codice,Descrizione,Icona)
	SELECT 
		Codice,
		Codice AS Descrizione,
		DEF.Icona AS Icona
	FROM #tmpTEVC
		LEFT JOIN
						(SELECT Icona
				 FROM T_TipoEvidenzaClinica
				WHERE Codice='_DEFAULT_') AS DEF			
			ON 1=1 
	WHERE Codice NOT IN (SELECT Codice FROM T_TipoEvidenzaClinica)
		
		
				INSERT INTO T_AllDWHTipoEvidenzaClinica(CodTipoEvidenzaClinica,CodDWH)
	SELECT
		Codice,
		Codice
	FROM 	#tmpTEVC
		WHERE Codice NOT IN (SELECT CodDWH FROM T_AllDWHTipoEvidenzaClinica)
				INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione,Icona16,Icona32,Icona48,Icona256)
	SELECT
		'EVC',
		T.Codice AS CodTipo,
		ISNULL(SE.Codice,'') AS CodStato,
		T.Codice + ' - ' + SE.Descrizione AS Descrizione,
		DEF.Icona16 AS Icona16,
		DEF.Icona32 AS Icona32,
		DEF.Icona48 AS Icona48,
		DEF.Icona256 AS Icona256
	FROM
		#tmpTEVC T 
		LEFT JOIN 
			(SELECT CodTipo, CodStato,Icona16,Icona32,Icona48,Icona256
			 FROM T_Icone
			 WHERE CodEntita='EVC' AND CodTipo='_DEFAULT_') AS DEF	    
			ON 1=1 
			
		LEFT JOIN 
			T_StatoEvidenzaClinica SE	
				ON DEF.CodStato=SE.Codice
		LEFT JOIN 
			(SELECT 
				CodTipo,CodStato 
			  FROM T_Icone 
			  WHERE CodEntita='EVC') AS I			  
			ON (T.Codice=I.CodTipo AND
			    SE.Codice=I.CodStato)	
	WHERE 			
		I.CodTipo IS NULL AND
		ISNULL(I.CodStato,'') <> ''
				
				
	RETURN 0	
	
END