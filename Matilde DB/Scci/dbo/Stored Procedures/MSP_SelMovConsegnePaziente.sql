CREATE PROCEDURE [dbo].[MSP_SelMovConsegnePaziente](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDConsegnaPaziente AS UNIQUEIDENTIFIER	
		
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML
	
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
	DECLARE @sTmp AS VARCHAR(Max)

	SET @sSQL ='
			SELECT 
				M.ID, 
				M.IDEpisodio,
				M.CodUA,
				A.Descrizione AS DescrUA,
				M.CodRuoloInserimento,
				M.CodTipoConsegnaPaziente,
				TC.Descrizione AS DescrTipoConsegnaPaziente,
				M.CodStatoConsegnaPaziente,
				SC.Descrizione AS DescrStatoConsegnaPaziente,
				M.CodUtenteInserimento,
				LI.Descrizione AS DescrUtenteInserimento,
				M.CodUtenteUltimaModifica,
				LM.Descrizione AS DescrUtenteUltimaModifica,
				M.CodUtenteAnnullamento,
				LA.Descrizione AS DescrUtenteAnnullamento,		
				M.CodUtenteCancellazione,
				LC.Descrizione AS DescrUtenteCancellazione,		
				S.ID AS IDScheda,
				S.CodScheda,
				S.Versione,
				M.DataInserimento,
				M.DataInserimentoUTC,
				M.DataUltimaModifica,
				M.DataUltimaModificaUTC,
				M.DataAnnullamento,
				M.DataAnnullamentoUTC,
				M.DataCancellazione,
				M.DataCancellazioneUTC,		
				S.AnteprimaRTF
			FROM 
				T_MovConsegnePaziente M			  											
					LEFT JOIN T_UnitaAtomiche A ON M.CodUA = A.Codice
					LEFT JOIN T_TipoConsegnaPaziente TC ON M.CodTipoConsegnaPaziente = TC.Codice
					LEFT JOIN T_StatoConsegnaPaziente SC ON M.CodStatoConsegnaPaziente = SC.Codice			
					LEFT JOIN T_Login LI ON M.CodUtenteInserimento = LI.Codice
					LEFT JOIN T_Login LM ON M.CodUtenteUltimaModifica = LM.Codice
					LEFT JOIN T_Login LA ON M.CodUtenteAnnullamento = LA.Codice
					LEFT JOIN T_Login LC ON M.CodUtenteCancellazione = LC.Codice
					LEFT JOIN T_MovSchede S
						ON (S.CodEntita=''CSP'' AND
							S.IDEntita=M.ID AND
							S.Storicizzata=0)
			WHERE 
		'

				
	SET @sWhere=''						

		IF @uIDConsegnaPaziente IS NOT NULL
		BEGIN
			SET @sWhere= @sWhere + ' M.ID=''' + convert(varchar(50),@uIDConsegnaPaziente) +''''
		END		
	ELSE
		BEGIN
			SET @sWhere = ' 0=1'
		END					
										 		
	SET @sSQL = @sSQL + @sWhere
	
	PRINT @sSQL
	EXEC (@sSQL)										

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

END