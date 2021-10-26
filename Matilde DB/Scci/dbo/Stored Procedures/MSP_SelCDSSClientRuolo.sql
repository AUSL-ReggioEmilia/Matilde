CREATE PROCEDURE [dbo].[MSP_SelCDSSClientRuolo](@xParametri AS XML)

AS
BEGIN
	

			
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodCDSS AS VARCHAR(20)
	DECLARE @sCodTipoCDSS AS VARCHAR(20)
	
		DECLARE @sSQL  AS VARCHAR(MAX)
	DECLARE @sWhere  AS VARCHAR(MAX)
	DECLARE @sTmp  AS VARCHAR(MAX)
				
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))

		SET @sCodTipoCDSS=(SELECT TOP 1 ValoreParametro.CodTipoCDSS.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoCDSS') as ValoreParametro(CodTipoCDSS))

		SET @sCodCDSS=(SELECT TOP 1 ValoreParametro.CodCDSS.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodCDSS') as ValoreParametro(CodCDSS))

	SET @sSQL='	SELECT 
			S.ID,
			S.CodRuolo,
			S.CodAzione,
			A.Descrizione AS DescrizioneAzione,
			S.CodPlugin,
			P.Descrizione AS DescrizionePlugin,
			P.NomePlugin AS NomePlugin,
			P.Comando AS ComandoPlugin,
			IsNull(S.Parametri,'''') AS ModalitaPlugin,
			IsNull(P.Ordine,1) AS OrdinePlugin,
			P.Icona AS IconaPlugin,
			S.Parametri AS Parametri
		FROM									
			T_CDSSStrutturaRuoli AS S
				INNER JOIN T_CDSSAzioni A
					ON S.CodAzione = A.Codice			
				INNER JOIN T_CDSSPlugins P
					ON S.CodPlugin = P.Codice'

				
	SET @sWhere=''									

	IF @sCodRuolo IS NOT NULL OR @sCodTipoCDSS IS NOT NULL OR @sCodCDSS IS NOT NULL
	BEGIN
		
				IF LEFT(@sCodCDSS,'5') = 'TILE_' 
			SET @sCodRuolo='EMA_SU'


				IF @sCodRuolo IS NOT NULL
		BEGIN
			SET @sTmp= ' AND S.CodRuolo=''' + @sCodRuolo +''''
			SET @sWhere= @sWhere + @sTmp
		END
		
				IF @sCodTipoCDSS IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.CodTipoCDSS =''' + @sCodTipoCDSS  +''''
			SET @sWhere= @sWhere + @sTmp
		END

				IF @sCodCDSS IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.Codice =''' + @sCodCDSS  +''''
			SET @sWhere= @sWhere + @sTmp
		END
	END
	ELSE
	BEGIN	
				SET @sWhere = ' AND 1=0'
	END

	IF ISNULL(@sWhere,'')<> ''
		BEGIN	
			SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		END	

	PRINT @sSQL
	EXEC (@sSQL)
END