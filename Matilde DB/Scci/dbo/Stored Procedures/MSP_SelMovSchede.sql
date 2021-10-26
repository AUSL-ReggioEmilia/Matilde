CREATE PROCEDURE [dbo].[MSP_SelMovSchede](@xParametri XML)
AS
BEGIN
	

				
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER = NULL
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER = NULL
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER = NULL
	DECLARE @uIDtrasferimento AS UNIQUEIDENTIFIER = NULL
	DECLARE @sCodScheda VARCHAR(20) = ''
			
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX) = ''
	DECLARE @sWhere AS VARCHAR(Max) = ''
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,@sGUID)

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,@sGUID)

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF ISNULL(@sGUID,'') <> '' SET @uIDtrasferimento=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
						FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))
	SET @sCodScheda=ISNULL(@sCodScheda,'')
			
				
	SET @sSQL= 'SELECT
					M.*,
					S.Descrizione AS Scheda
				FROM 
					T_MovSchede M  WITH (NOLOCK)
						LEFT JOIN T_Schede S WITH (NOLOCK) ON M.CodScheda=S.Codice'

		SET @sWhere=@sWhere + ' WHERE M.CodStatoScheda <> ''CA'''	
	SET @sWhere=@sWhere + 'AND M.Storicizzata = 0'	
					
		IF @uIDScheda IS NOT NULL
		BEGIN											
			SET @sWhere=@sWhere + ' AND M.ID=''' + CONVERT(VARCHAR(50),@uIDScheda) + ''''
		END			

		IF @uIDPaziente IS NOT NULL
		BEGIN											
			SET @sWhere=@sWhere + ' AND M.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + ''''
		END			

		IF @uIDEpisodio IS NOT NULL
		BEGIN											
			SET @sWhere=@sWhere + ' AND M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''''
		END			

		IF @uIDtrasferimento IS NOT NULL
		BEGIN											
			SET @sWhere=@sWhere + ' AND M.IDTrasferimento=''' + CONVERT(VARCHAR(50),@uIDtrasferimento) + ''''
		END			
								
		IF ISNULL(@sCodScheda,'') <> ''
		BEGIN
			SET @sWhere=@sWhere + ' AND M.CodScheda=''' + @sCodScheda + ''''
		END														

		SET @sSQL=@sSQL + @sWhere
					
		SET @sSQL=@sSQL + ' ORDER BY ISNULL(DataUltimaModifica,DataCreazione) ASC'
	
	PRINT @sSQL			
	EXEC (@sSQL)
				 												
	RETURN 0

END