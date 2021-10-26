CREATE PROCEDURE [dbo].[MSP_WS_CercaSchede](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @sCodSAC AS VARCHAR(50) 
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodSAC=(SELECT	TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))						 
	SET @sCodSAC= LTRIM(RTRIM(ISNULL(@sCodSAC,'')))
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
				
			
				
	SET @sSQL='	SELECT
					M.ID AS IDScheda,
					M.IDSchedaPadre,
					TRASF.IDCartella,
					CAR.NumeroCartella,
					M.CodScheda,
					S.Descrizione AS DescrizioneScheda,
					M.Versione,
					M.Numero,
					M.CodStatoScheda,
					ST.Descrizione AS DescrizioneStatoScheda,
					
					M.CodUA,
					AT.Descrizione AS DescrizioneUA,					
					M.CodUtenteRilevazione,
					UC.Descrizione AS DescrizioneUtenteRilevazione,
					DataCreazione,
					
					M.CodUtenteUltimaModifica,					
					U.Descrizione AS DescrizioneUtenteUltimaModifica,
					DataUltimaModifica,
					IsNUll(InEvidenza,0) AS InEvidenza
				FROM 
					T_MovSchede M  WITH (NOLOCK)
						LEFT JOIN T_Login U WITH (NOLOCK)
							ON M.CodUtenteUltimaModifica = U.Codice
						LEFT JOIN T_Login UC WITH (NOLOCK)
							ON M.CodUtenteRilevazione = UC.Codice	
						LEFT JOIN T_Schede S WITH (NOLOCK) ON
							M.CodScheda=S.Codice
						LEFT JOIN T_UnitaAtomiche AT WITH (NOLOCK) ON
							M.CodUA =AT.Codice
						INNER JOIN T_Pazienti P
							ON M.IDPaziente=P.ID
						LEFT JOIN T_StatoScheda ST
							ON M.CodStatoScheda=ST.Codice 
						LEFT JOIN T_MovTrasferimenti TRASF
							ON M.IDTrasferimento=TRASF.ID
						LEFT JOIN T_MovCartelle CAR
							ON TRASF.IDCartella=CAR.ID
				'

	SET @sWhere= ''
		SET @sWhere= @sWhere + ' AND Storicizzata=0							 
							 AND M.CodStatoScheda <> ''CA'''
							 
		IF ISNULL(@sCodSAC,'')<>'' 
	BEGIN
		SET @sWhere= @sWhere + ' AND (M.CodEntita=''PAZ'' 
							          AND P.CodSAC='''+ CONVERT(VARCHAR(50),@sCodSAC) + ''')'
	END
	
							 
		IF @uIDCartella IS NOT NULL
	BEGIN
		SET @sWhere= @sWhere + ' AND (M.CodEntita=''EPI'' AND
									  M.IDTrasferimento IN 
										(SELECT IDTrasferimento
										 FROM T_MovTrasferimenti 
										 WHERE 
											   CodStatoTrasferimento <> ''CA'' AND 
											   IDCartella ='''+ CONVERT(VARCHAR(50),@uIDCartella) + '''
										)
									  )'
	END
										  
							
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
			
		
	PRINT @sSQL					
	EXEC (@sSQL)							
	           
	RETURN 0
	
END