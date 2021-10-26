
CREATE PROCEDURE [dbo].[MSP_SelDizionarioValori](@xParametri XML)
AS
BEGIN
		
	
				
	DECLARE @sCodDizionario AS VARCHAR(20)	
	DECLARE @sCodValore AS VARCHAR(20)	
	DECLARE @sCodPath AS VARCHAR(20)	
	
	DECLARE @bDatiEstesi AS Bit
			
	SET @sCodDizionario=(SELECT TOP 1 ValoreParametro.CodDizionario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodDizionario') as ValoreParametro(CodDizionario))	
	
	SET @sCodValore=(SELECT TOP 1 ValoreParametro.CodValore.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodValore') as ValoreParametro(CodValore))	
	
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))

	SET @sCodPath=(SELECT TOP 1 ValoreParametro.CodPath.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodPath') as ValoreParametro(CodPath))	

				
		CREATE TABLE #tmpPath
		(Valore VARCHAR(255))

	INSERT INTO #tmpPath(Valore)
		SELECT  value  FROM STRING_SPLIT(@sCodPath, '|') 
	
	
		IF ISNULL(@bDatiEstesi,0)=0
		IF @sCodPath IS NULL
		BEGIN
						SELECT
					V.Codice AS CodValore,
					V.Descrizione,
					V.Ordine,
					V.[Path] AS [Path]
			FROM 
				T_DCDecodificheValori V			
			WHERE DtValI<=GETDATE() AND
				  ISNULL(DtValF,GETDATE()) >=GETDATE() AND
				  V.CodDec=	@sCodDizionario				
			ORDER BY V.Ordine,V.Codice
		END
		ELSE
		BEGIN
						SELECT
					V.Codice AS CodValore,
					V.Descrizione,
					V.Ordine,
					V.[Path] AS [Path]
			FROM 
				T_DCDecodificheValori V			
			WHERE DtValI<=GETDATE() AND
				  ISNULL(DtValF,GETDATE()) >=GETDATE() AND
				  V.CodDec=	@sCodDizionario	AND
				  EXISTS
					(SELECT value AS Valore 
					 FROM STRING_SPLIT(V.[Path], '|')
					 WHERE value IN (SELECT Valore FROM #tmpPath)
					 )		
						
				  				  			ORDER BY V.Ordine,V.Codice
		END
	ELSE
	BEGIN		
		IF @sCodValore IS NULL
		BEGIN
									SELECT
					V.Codice AS CodValore,
					V.Descrizione,
					V.InfoRTF,
					V.[Path] AS [Path],
					V.Ordine,
					V.InfoRTF		
			FROM 
				T_DCDecodificheValori V			
			WHERE DtValI<=GETDATE() AND
				  ISNULL(DtValF,GETDATE()) >=GETDATE() AND
				  V.CodDec=	@sCodDizionario				
			ORDER BY V.Ordine,V.Codice
	    END
		ELSE
		BEGIN
									SELECT
					V.Codice AS CodValore,
					V.Descrizione,
					V.InfoRTF,
					V.[Path] AS [Path],
					V.Ordine,
					V.InfoRTF		
			FROM 
				T_DCDecodificheValori V			
			WHERE DtValI<=GETDATE() AND
				  ISNULL(DtValF,GETDATE()) >=GETDATE() AND
				  V.CodDec=	@sCodDizionario	AND
				  V.Codice=	@sCodValore		
			ORDER BY V.Ordine,V.Codice
		END
	END

	DROP TABLE #tmpPath
	RETURN 0
END