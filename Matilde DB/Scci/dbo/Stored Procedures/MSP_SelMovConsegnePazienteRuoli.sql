CREATE PROCEDURE [dbo].[MSP_SelMovConsegnePazienteRuoli](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDConsegnaPazienteRuoli AS UNIQUEIDENTIFIER	
	DECLARE @uIDConsegnaPaziente AS UNIQUEIDENTIFIER	
		
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPazienteRuoli.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDConsegnaPazienteRuoli') as ValoreParametro(IDConsegnaPazienteRuoli))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDConsegnaPazienteRuoli=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDConsegnaPaziente') as ValoreParametro(IDConsegnaPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDConsegnaPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')		
	
				
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)

	SET @sSQL ='
			SELECT 
				M.ID, 
				M.IDConsegnaPaziente,
				M.CodStatoConsegnaPazienteRuolo,
				SC.Descrizione AS DescrStatoConsegnaPazienteRuolo,
				M.CodRuolo,
				M.CodUtenteInserimento,
				LI.Descrizione AS DescrUtenteInserimento,
				M.CodUtenteUltimaModifica,
				LM.Descrizione AS DescrUtenteUltimaModifica,
				M.CodUtenteAnnullamento,
				LA.Descrizione AS DescrUtenteAnnullamento,		
				M.CodUtenteCancellazione,
				LC.Descrizione AS DescrUtenteCancellazione,
				M.CodUtenteVisione,
				LV.Descrizione AS DescrUtenteVisione,		
				M.DataInserimento,
				M.DataInserimentoUTC,
				M.DataUltimaModifica,
				M.DataUltimaModificaUTC,
				M.DataAnnullamento,
				M.DataAnnullamentoUTC,
				M.DataCancellazione,
				M.DataCancellazioneUTC,		
				M.DataVisione,
				M.DataVisioneUTC
			FROM 
				T_MovConsegnePazienteRuoli M			  											
					LEFT JOIN T_StatoConsegnaPazienteRuoli SC ON M.CodStatoConsegnaPazienteRuolo = SC.Codice			
					LEFT JOIN T_Login LI ON M.CodUtenteInserimento = LI.Codice
					LEFT JOIN T_Login LM ON M.CodUtenteUltimaModifica = LM.Codice
					LEFT JOIN T_Login LA ON M.CodUtenteAnnullamento = LA.Codice
					LEFT JOIN T_Login LC ON M.CodUtenteCancellazione = LC.Codice
					LEFT JOIN T_Login LV ON M.CodUtenteVisione = LV.Codice
			WHERE 
		'

				
	SET @sWhere=''						

		IF @uIDConsegnaPazienteRuoli IS NOT NULL
		BEGIN
			SET @sWhere = ' M.ID=''' + convert(varchar(50),@uIDConsegnaPazienteRuoli) +''''
		END		
	
		IF @uIDConsegnaPaziente IS NOT NULL
		BEGIN
			SET @sWhere = ' M.IDConsegnaPaziente=''' + convert(varchar(50),@uIDConsegnaPaziente) +''''
		END		

	IF @sWhere=''
		BEGIN
			SET @sWhere = ' 0=1'
		END					
										 		
	SET @sSQL = @sSQL + @sWhere
	
	PRINT @sSQL
	EXEC (@sSQL)										

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

END