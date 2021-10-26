CREATE PROCEDURE [dbo].[MSP_WS_CercaAlertAllergieAnamnesi](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @sCodSAC AS VARCHAR(50) 
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
							  		
		SET @sCodSAC=(SELECT	TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))						 
	SET @sCodSAC= LTRIM(RTRIM(ISNULL(@sCodSAC,'')))
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
				
			
				

	
	SELECT 
		A.ID AS IDAlertGenerico,
		A.CodTipoAlertAllergiaAnamnesi,
		TA.Descrizione AS DescrizioneTipoAlertGenerico,
		A.CodStatoAlertAllergiaAnamnesi,
		SA.Descrizione AS DescrizioneStatoAlertGenerico,
		A.DataEvento,
		A.CodUtenteRilevazione,
		A.CodUtenteUltimaModifica,
		A.DataUltimaModifica   	

	FROM T_MovAlertAllergieAnamnesi A 
		INNER JOIN T_TipoAlertAllergiaAnamnesi AS TA
			ON A.CodTipoAlertAllergiaAnamnesi=TA.Codice 
		INNER JOIN T_StatoAlertGenerico AS SA
			ON A.CodStatoAlertAllergiaAnamnesi=SA.Codice 	
		INNER JOIN T_Pazienti P
			ON A.IDPaziente=P.ID
	WHERE 
		A.CodStatoAlertAllergiaAnamnesi NOT IN ('CA')  AND 
		P.CodSAC=@sCodSAC
	           
	RETURN 0
	
END