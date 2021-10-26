
CREATE PROCEDURE [dbo].[MSP_SelMovCalendari](@xParametri AS XML)
AS
BEGIN
	   	
	   	
							
	DECLARE @sCodAgenda AS VARCHAR(1800)
	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sDataTmp AS VARCHAR(20)
	
	SET @sCodAgenda=''
	SELECT	@sCodAgenda =  @sCodAgenda +
														CASE 
								WHEN @sCodAgenda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodAgenda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)					 
	
	SET @sCodAgenda=LTRIM(RTRIM(@sCodAgenda))
	IF	@sCodAgenda='''''' SET @sCodAgenda=''
	SET @sCodAgenda=UPPER(@sCodAgenda)
											
				
	BEGIN
	
		SELECT A.Codice, A.Descrizione, A.Colore, A.IntervalloSlot, A.OrariLavoro
		FROM T_Agende A	
		WHERE A.Codice IN (@sCodAgenda)
	END
										
								
	RETURN 0
END