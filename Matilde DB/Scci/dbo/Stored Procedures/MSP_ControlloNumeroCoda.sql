CREATE PROCEDURE [dbo].[MSP_ControlloNumeroCoda](@xParametri AS XML )
AS
BEGIN
	
	
		DECLARE @sCodContatore AS VARCHAR(20)
	DECLARE @nNumeroCoda AS INTEGER
	DECLARE @bEsito AS INTEGER
	
		DECLARE @dMaxDataImpostazione AS DATETIME
	DECLARE @nConta AS INTEGER

		SET @sCodContatore=(SELECT	TOP 1 ValoreParametro.CodContatore.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodContatore') as ValoreParametro(CodContatore))

	SET @nNumeroCoda=(SELECT	TOP 1 ValoreParametro.NumeroCoda.value('.','INTEGER')
						 FROM @xParametri.nodes('/Parametri/NumeroCoda') as ValoreParametro(NumeroCoda))				 		
	
		SET @dMaxDataImpostazione=(SELECT ISNULL(MAX(DataImpostazione),CONVERT(DATETIME,'1901-01-01'))
								   FROM T_Contatori
								   WHERE Codice=@sCodContatore)

			   
	SET @nConta=(SELECT COUNT(IDNum) AS QTA
						FROM T_MovCode
						WHERE 
							CodContatore=@sCodContatore AND
							CodStatoCoda NOT IN ('CA') AND 
							DataAssegnazione >= @dMaxDataImpostazione AND
							NumeroCoda=@nNumeroCoda
						)	  
	SET @nConta=ISNULL(@nConta,0)
	IF @nConta=0 
		SET @bEsito=0
	ELSE
		SET @bEsito=1
				
		SELECT 	CONVERT(INTEGER,@bEsito) AS Esito
	
	RETURN 0				
END

select * from T_MovCode