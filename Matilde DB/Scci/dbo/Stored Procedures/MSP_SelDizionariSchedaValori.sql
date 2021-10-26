CREATE PROCEDURE [dbo].[MSP_SelDizionariSchedaValori](@xParametri XML)
AS
BEGIN
		
	
				
	DECLARE @sCodScheda AS VARCHAR(20)	
	
	DECLARE @nVersione AS INTEGER
	
	SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	

	SET @nVersione=(SELECT TOP 1 ValoreParametro.DataRif.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(DataRif))	
	
	SET @nVersione=ISNULL(@nVersione,0)
	
											
							
		
				
	SET @xParametri=CONVERT(XML,'<Parametri>  ' +
								'	<CodScheda>' + @sCodScheda + '</CodScheda> ' +
								'	<Versione>' + convert(varchar(20),@nVersione) + '</Versione> '+
								' </Parametri>')	
						
	CREATE TABLE #tmpDizionari
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
	
	INSERT #tmpDizionari EXEC MSP_SelDizionariScheda @xParametri
	
	CREATE INDEX IX_Codice ON #tmpDizionari (Codice)    
	
		
		SELECT	V.CodDec AS CodTab,
			V.Codice AS CodValore,
			V.Descrizione,
			V.Ordine,
									Icona
	FROM 
		T_DCDecodificheValori V
			INNER JOIN 	#tmpDizionari T
				ON V.CodDec=T.Codice
	WHERE DtValI<=GETDATE() AND
		  ISNULL(DtValF,GETDATE()) >=GETDATE()							
	ORDER BY V.CodDec,V.Ordine,V.Codice

		DROP TABLE #tmpDizionari	
	
	RETURN 0
END