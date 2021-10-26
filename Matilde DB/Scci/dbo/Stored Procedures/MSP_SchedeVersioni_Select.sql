CREATE PROCEDURE [dbo].[MSP_SchedeVersioni_Select] 
	
	 @codScheda		varchar(20)				,@versione		int			= NULL		,@dataVal		datetime	= NULL		
AS
BEGIN
			SET NOCOUNT ON;

	SELECT
		S.Codice
		,S.Descrizione
		,V.CodScheda
		,V.Versione
		,V.FlagAttiva
		,V.Pubblicato
		,V.DtValI
		,V.DtValF
		,V.Struttura
		,V.Layout
		,V.StrutturaV3
		,V.LayoutV3

	FROM
		T_SchedeVersioni (NOLOCK) V
		INNER JOIN T_Schede (NOLOCK) S
			ON V.CodScheda = S.Codice
	WHERE
		CodScheda = @codScheda AND
		(@versione IS NULL OR (Versione = @versione)) AND
		(@dataVal IS NULL OR 
			( (@dataVal >= [DtValI]) AND ( ([DtValF] IS NULL) OR (@dataVal <= [DtValF]) ) )
		)
	ORDER BY
		CodScheda, Versione
END