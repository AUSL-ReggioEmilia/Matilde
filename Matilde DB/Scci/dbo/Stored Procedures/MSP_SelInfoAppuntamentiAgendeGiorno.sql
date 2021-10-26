
CREATE PROCEDURE [dbo].[MSP_SelInfoAppuntamentiAgendeGiorno](@xParametri AS XML)
AS
BEGIN
	   	
	   	
					
	DECLARE @sCodAgenda AS VARCHAR(1800)	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
		
	DECLARE @sDataTmp AS VARCHAR(20)
		
	SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))

	SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL		
		END 				
	
										
				
		SELECT 		
		COUNT(*) AS Qta
		FROM
			T_MovAppuntamenti MA WITH (NOLOCK)
				INNER JOIN T_MovAppuntamentiAgende MAG WITH (NOLOCK)
					ON MA.ID=MAG.IDAppuntamento
				INNER JOIN T_TipoAppuntamento T WITH (NOLOCK)
					ON MA.CodTipoAppuntamento=T.Codice
	WHERE
		MAG.CodAgenda=@sCodAgenda 
		AND 
		 MA.DataInizio >=@dDataInizio AND
		 MA.DataInizio <=@dDataFine AND
		 MA.CodStatoAppuntamento NOT IN ('CA','AN','TR') 
END