CREATE PROCEDURE [dbo].[MSP_SelMovPazientiRecenti](@xParametri XML)
AS
BEGIN

	
	
				DECLARE @sCodUtente AS VARCHAR(100)	
	
		DECLARE @sMaxRecord AS VARCHAR(20)
	DECLARE @sSQL AS VARCHAR(MAX)						
							
			SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sMaxRecord=(SELECT TOP 1 Valore FROM T_Config WHERE ID=170)
	IF @sMaxRecord='' SET @sMaxRecord=NULL 
	SET @sMaxRecord=ISNULL(@sMaxRecord,'20')
		
	IF @sCodUtente IS NOT NULL
		BEGIN
			SET @sSQL='SELECT TOP ' + @sMaxRecord + ' 
				ISNULL(PA.IDPaziente,M.IDPaziente) AS IDPaziente,
				ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') AS Paziente,
				P.Sesso,
					CASE
						WHEN P.DataNascita IS NULL THEN ''''
						ELSE REPLACE(CONVERT(VARCHAR(10),P.DataNascita,105),''-'',''/'')
					END
				+ 
					CASE	
						WHEN P.ComuneNascita IS NULL THEN ''''
						ELSE '' '' + ISNULL(P.ComuneNascita,'''') 
					END	
				AS [NascitaDescrizione],
				ISNULL(P.CodiceFiscale,'''') AS CodiceFiscale,
				ISNULL(P.ComuneResidenza,'''') AS ComuneResidenza,				
				P.CodSAC
				
			FROM 
				T_MovPazientiRecenti M	
					LEFT JOIN T_PazientiAlias PA	
							ON M.IDPaziente=PA.IDPazienteVecchio 
					LEFT JOIN T_Pazienti P
							ON ISNULL(PA.IDPaziente,M.IDPaziente)=P.ID 	
						
			WHERE				
				M.CodUtente= ''' + @sCodUtente +''' AND
				M.CodStatoPazienteRecente IN (''IC'')					ORDER BY 
				M.DataInserimento DESC '
				
			PRINT @sSQL	
			EXEC (@sSQL)		
		END		
	
	
	RETURN 0
END