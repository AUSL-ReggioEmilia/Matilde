CREATE PROCEDURE [dbo].[MSP_SelOggettoAppuntamento](@xParametri AS XML)
AS
BEGIN
	   	
	   	
							
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @sCodEntita AS VARCHAR(20)
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sSQLQueryAgende AS VARCHAR(MAX)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
					  				  
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))
	SET @sCodEntita=ISNULL(@sCodEntita,'')
					  				  				  
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)

		
	IF @sCodEntita='EPI'
		BEGIN		
			SET @sSQLQueryAgende =(SELECT TOp 1 dbo.MF_SQLQueryAgendeEPI())
		
			SET @sWhere=' IDEpisodio=''' + CONVERT(varchar(50),@uIDEpisodio) + ''''
		END	
	ELSE	
		BEGIN
			SET @sSQLQueryAgende =(SELECT TOP 1 dbo.MF_SQLQueryAgendePAZ())
			SET @sWhere=' IDPaziente=''' + CONVERT(varchar(50),@uIDPaziente) + ''''
		END	
		
	SET @sSQL= @sSQLQueryAgende 
	
			
	SET @sSQL = @sSQL + ' WHERE ' + @sWhere
										
		EXEC (@sSQL)								
			
END