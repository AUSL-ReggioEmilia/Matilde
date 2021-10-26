CREATE PROCEDURE [dbo].[MSP_SelMovTrasferimento](@xParametri XML)
AS
BEGIN

		
	
				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER

	DECLARE @xTmpTS AS XML
	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
							  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


						
												
				
				
	
		
					
				SELECT * 
		FROM T_MovTrasferimenti
		WHERE ID=@uIDTrasferimento
	
				
		
					
								
	
		RETURN 0
END