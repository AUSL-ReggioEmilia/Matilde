CREATE PROCEDURE [dbo].[MSP_SelMovSegnalibri](@xParametri XML)
AS
BEGIN

	
	
				
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
		
		DECLARE @sGUID AS VARCHAR(Max)
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
			  				  
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

			IF @uIDCartella IS NOT NULL
		BEGIN							
			SELECT 
				M.ID, 
				CONVERT(VARBINARY(MAX),NULL) AS Icona,
				M.CodEntita,
				M.IDEntita, 
				M.CodEntitaScheda,
				M.CodScheda,
				M.Numero, 
				CASE 
						WHEN S.ID IS NOT NULL THEN CONVERT(UNIQUEIDENTIFIER, S.ID)
						ELSE NULL
				END AS IDScheda,
				ISNULL(A.Descrizione,'') + ' ' +
										CASE	
						WHEN ISNULL(A.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
						ELSE '' 
					END +
					CASE 
						WHEN S.ID IS NULL OR ISNULL(S.CodStatoScheda,'CA') ='CA' THEN ' {CANCELLATA}'
						WHEN S.ID IS NULL OR ISNULL(S.CodStatoScheda,'AN') ='AN' THEN ' {ANNULLATA}'
						ELSE ''	
					END						
				AS Descrizione,
				CASE	
					WHEN ISNULL(S.CodStatoScheda,'CA') IN ('AN','CA') THEN CONVERT(INTEGER,0) 
					ELSE  CONVERT(INTEGER,1) 
				END	AS PermessoModifica
			FROM T_MovSegnalibri M
					LEFT JOIN T_Schede A
						ON M.CodScheda=A.Codice									
					LEFT JOIN	
												(SELECT 
							MS.IDEntita,
							MS.CodEntita,
							MS.CodScheda,
							MS.Numero,
							MIN(IDNum) AS IDNumScheda
						FROM T_MovSchede MS
						 WHERE
							MS.Storicizzata=0 AND
							MS.CodStatoScheda<>'CA'
						GROUP BY 	
							MS.IDEntita,
							MS.CodEntita,
							MS.CodScheda,
							MS.Numero)  AS S1
						ON 
							S1.IDEntita= M.IDEntita AND
							S1.CodEntita=M.CodEntitaScheda AND
							S1.CodScheda= M.CodScheda AND
							S1.Numero=M.Numero	
					LEFT JOIN
						T_MovSchede AS  S
							ON S.IDNum=IDNumScheda
				
			WHERE
				M.CodEntita='SCH' AND
				M.CodUtente=@sCodUtente AND
				M.CodRuolo=@sCodRuolo AND
				(M.IDCartella=@uIDCartella OR										(	M.CodEntitaScheda='PAZ' AND										(M.IDPaziente=@uIDPaziente OR
									 									 M.IDPaziente IN 
										(SELECT IDPazienteVecchio
										 FROM T_PazientiAlias
										 WHERE 
											IDPaziente IN 
												(SELECT IDPaziente
												 FROM T_PazientiAlias
												 WHERE IDPazienteVecchio=@uIDPaziente
												)
										)			
						)
					)	
				)

				
		END
	ELSE		
		IF @uIDPaziente IS NOT NULL
			BEGIN		
			
				SELECT 
					M.ID,
					CONVERT(VARBINARY(MAX),NULL) AS Icona,
					M.CodEntita,
					M.IDEntita,
					M.CodEntitaScheda,
					M.CodScheda,
					M.Numero,
					CASE 
						WHEN S.ID IS NOT NULL THEN CONVERT(UNIQUEIDENTIFIER, S.ID)
						ELSE NULL
					END AS IDScheda,					
					ISNULL(A.Descrizione,'') + ' ' +								
										CASE	
						WHEN ISNULL(A.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
						ELSE '' 
					END 
					+ 
					CASE 
						WHEN S.ID IS NULL OR ISNULL(S.CodStatoScheda,'CA') ='CA' THEN ' {CANCELLATA}'
						WHEN S.ID IS NULL OR ISNULL(S.CodStatoScheda,'AN') ='AN' THEN ' {ANNULLATA}'
						ELSE ''	
					END	
					AS Descrizione,
					CASE	
						WHEN ISNULL(S.CodStatoScheda,'CA') IN ('AN','CA') THEN CONVERT(INTEGER,0) 
						ELSE  CONVERT(INTEGER,1) 
					END	AS PermessoModifica
					
				FROM T_MovSegnalibri M
						LEFT JOIN T_Schede A
							ON M.CodScheda=A.Codice		
					LEFT JOIN	
														(SELECT 
								MS.IDEntita,
								MS.CodEntita,
								MS.CodScheda,
								MS.Numero,
								MIN(IDNum) AS IDNumScheda
							FROM T_MovSchede MS
							 WHERE
								MS.Storicizzata=0 AND
								MS.CodStatoScheda<>'CA'
							GROUP BY 	
								MS.IDEntita,
								MS.CodEntita,
								MS.CodScheda,
								MS.Numero)  AS S1
							ON 
								S1.IDEntita= M.IDEntita AND
								S1.CodEntita=M.CodEntitaScheda AND
								S1.CodScheda= M.CodScheda AND
								S1.Numero=M.Numero	
						LEFT JOIN
							T_MovSchede AS  S
								ON S.IDNum=IDNumScheda
										
					
				WHERE
					M.CodEntita='SCH' AND									
					M.CodUtente=@sCodUtente AND
					M.CodRuolo=@sCodRuolo AND
					M.IDCartella IS NULL AND
					(M.IDPaziente=@uIDPaziente OR
					 					 M.IDPaziente IN 
						(SELECT IDPazienteVecchio
						 FROM T_PazientiAlias
						 WHERE 
							IDPaziente IN 
								(SELECT IDPaziente
								 FROM T_PazientiAlias
								 WHERE IDPazienteVecchio=@uIDPaziente
								)
						)
					)
			END	
	RETURN 0
END