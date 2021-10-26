CREATE PROCEDURE [dbo].[MSP_SelMovCodeMaxContatore](@xParametri AS XML )
AS
BEGIN
	
	
		DECLARE @sCodContatore AS VARCHAR(20)
	
		DECLARE @dMaxDataImpostazione AS DATETIME
	DECLARE @nMaxContatore AS INTEGER

		SET @sCodContatore=(SELECT	TOP 1 ValoreParametro.CodContatore.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodContatore') as ValoreParametro(CodContatore))
						 		
	
		SET @dMaxDataImpostazione=(SELECT ISNULL(MAX(DataImpostazione),CONVERT(DATETIME,'1901-01-01'))
								   FROM T_Contatori
								   WHERE Codice=@sCodContatore)

			   
	SET @nMaxContatore=(SELECT MAX(NumeroCoda)
						FROM T_MovCode
						WHERE 
							CodContatore=@sCodContatore AND
							CodStatoCoda NOT IN ('CA') AND 
							DataAssegnazione >= @dMaxDataImpostazione
						)	  
	SET @nMaxContatore=ISNULL(@nMaxContatore,0)
	
		SELECT  @nMaxContatore AS UltimoNumero
	
	RETURN 0				
END

select * from T_MovCode