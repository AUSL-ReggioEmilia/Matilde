CREATE PROCEDURE [dbo].[MSP_WS_CercaAlertGenerici](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
				
			
				

	
	SELECT 
		A.ID AS IDAlertGenerico,		
		TRASF.IDCartella,
		CAR.NumeroCartella,
		A.CodTipoAlertGenerico,
		TA.Descrizione AS DescrizioneTipoAlertGenerico,
		A.CodStatoAlertGenerico,
		SA.Descrizione AS DescrizioneStatoAlertGenerico,
		A.DataEvento,
		A.CodUtenteRilevazione,
		A.CodUtenteUltimaModifica,
		A.CodUtenteVisto,
		A.DataUltimaModifica,
		A.DataVisto

	FROM T_MovAlertGenerici A 
		INNER JOIN T_TipoAlertGenerico AS TA
			ON A.CodTipoAlertGenerico=TA.Codice 
		INNER JOIN T_StatoAlertGenerico AS SA
			ON A.CodStatoAlertGenerico=SA.Codice 	
		INNER JOIN T_MovTrasferimenti TRASF
			ON TRASF.ID=A.IDTrasferimento
		LEFT JOIN T_MovCartelle CAR
			ON CAR.ID=TRASF.IDCartella
			
	WHERE 
		A.CodStatoAlertGenerico NOT IN ('CA')  AND 
		TRASF.IDCartella =@uIDCartella 
		
		       
	RETURN 0
	
END