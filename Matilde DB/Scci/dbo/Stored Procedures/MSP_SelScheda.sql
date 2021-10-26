CREATE PROCEDURE [dbo].[MSP_SelScheda](@xParametri XML)
AS
BEGIN

		
	
				
	DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @sDataRif AS VARCHAR(50)
	DECLARE @dDataRif AS DATETIME
	DECLARE @nVersione AS INTEGER
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	

		SET @sDataRif=(SELECT TOP 1 ValoreParametro.DataRif.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/DataRif') as ValoreParametro(DataRif))	
					  	
	SET @sDataRif= ISNULL(@sDataRif,'')
	SET @sDataRif=LTRIM(RTRIM(@sDataRif))
	IF @sDataRif<> '' SET @dDataRif=CONVERT(DATETIME,@sDataRif,105)
	
		SET @nVersione=(SELECT TOP 1 ValoreParametro.Versione.value('.','INT')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))	
	SET @nVersione=ISNULL(@nVersione,0)

				
	IF @nVersione <> 0 
		BEGIN
			
			SELECT TOP 1 S.Codice,
				ISNULL(S.Descrizione, '') AS Descrizione,
				ISNULL(S.Note, '') AS Note,
				ISNULL(S.[Path], '') AS [Path],
				ISNULL(S.CodTipoScheda, '') AS CodTipoScheda,
				ISNULL(S.SchedaSemplice, 0) AS SchedaSemplice,
				ISNULL(S.CodEntita, '') AS CodEntita,
				ISNULL(S.Ordine, '') AS Ordine,
				ISNULL(S.NumerositaMinima, 0) AS NumerositaMinima,
				ISNULL(S.NumerositaMassima, 0) AS NumerositaMassima,
				ISNULL(S.CreaDefault, 0) AS CreaDefault,
				ISNULL(V.Versione, 0) AS Versione,
				CONVERT(VARCHAR(max), V.Struttura) AS StrutturaXML, 
				CONVERT(VARCHAR(max), V.Layout) AS LayoutXML,
				ISNULL(S.Validabile, 0) AS Validabile,
				ISNULL(S.Riservata, 0) AS Riservata,
				ISNULL(S.Contenitore,0) AS Contenitore,
				ISNULL(S.AlertSchedaVuota,0) AS AlertSchedaVuota,
				ISNULL(S.CopiaPrecedenteSelezione,0) AS CopiaPrecedenteSelezione,
				ISNULL(S.CartellaAmbulatorialeCodificata,0) AS CartellaAmbulatorialeCodificata,
				S.CodContatore
			FROM 
				T_Schede S INNER JOIN T_SchedeVersioni V ON
					S.Codice = V.CodScheda
			WHERE
				S.Codice = @sCodScheda AND																		ISNULL(V.FlagAttiva, 0) = 1	AND ISNULL(V.Pubblicato,0)=1 										AND 
					Versione=@nVersione																			
			ORDER BY Versione DESC
	  END
	ELSE
	BEGIN	
		
			SELECT TOP 1 S.Codice,
				ISNULL(S.Descrizione, '') AS Descrizione,
				ISNULL(S.Note, '') AS Note,
				ISNULL(S.[Path], '') AS [Path],
				ISNULL(S.CodTipoScheda, '') AS CodTipoScheda,
				ISNULL(S.SchedaSemplice, 0) AS SchedaSemplice,
				ISNULL(S.CodEntita, '') AS CodEntita,
				ISNULL(S.Ordine, '') AS Ordine,
				ISNULL(S.NumerositaMinima, 0) AS NumerositaMinima,
				ISNULL(S.NumerositaMassima, 0) AS NumerositaMassima,
				ISNULL(S.CreaDefault, 0) AS CreaDefault,
				ISNULL(V.Versione, 0) AS Versione,
				CONVERT(VARCHAR(max), V.Struttura) AS StrutturaXML, 
				CONVERT(VARCHAR(max), V.Layout) AS LayoutXML ,
				ISNULL(S.Validabile, 0) AS Validabile,
				ISNULL(S.Riservata, 0) AS Riservata,
				ISNULL(S.Contenitore,0) AS Contenitore,
				ISNULL(S.AlertSchedaVuota,0) AS AlertSchedaVuota,
				ISNULL(S.CopiaPrecedenteSelezione,0) AS CopiaPrecedenteSelezione,
				ISNULL(S.CartellaAmbulatorialeCodificata,0) AS CartellaAmbulatorialeCodificata,
				S.CodContatore
			FROM 
				T_Schede S INNER JOIN T_SchedeVersioni V ON
					S.Codice = V.CodScheda
			WHERE
				S.Codice = @sCodScheda AND																		ISNULL(V.FlagAttiva, 0) = 1	AND ISNULL(V.Pubblicato,0)=1 										AND 
					V.DtValI <= @dDataRif AND																		ISNULL(V.DtValF,convert(datetime,'01-01-2100')) >= @dDataRif 
														
			ORDER BY Versione DESC
	END	  		
END