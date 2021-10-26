CREATE PROCEDURE [dbo].[MSP_CercaConsegnaPrecedente](@xParametri XML)
AS
BEGIN

	
	

	
							
		DECLARE @sCodTipoConsegna  AS VARCHAR(20)	
	DECLARE @sCodUA  AS VARCHAR(20)	
	DECLARE @uIDMovConsegnaDaIgnorare  AS UNIQUEIDENTIFIER	


		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER		
	DECLARE @sSQL  AS VARCHAR(MAX)
	
	
		SET @sCodTipoConsegna=(SELECT TOP 1 ValoreParametro.CodTipoConsegna.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoConsegna') as ValoreParametro(CodTipoConsegna))	
	SET @sCodTipoConsegna=ISNULL(@sCodTipoConsegna,'')
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDMovConsegnaDaIgnorare.value('.','VARCHAR(50)')
				  FROM @xParametri.nodes('/Parametri/IDMovConsegnaDaIgnorare') as ValoreParametro(IDMovConsegnaDaIgnorare))
	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDMovConsegnaDaIgnorare=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
				
	SET @sSQL =	'SELECT TOP 1 ID AS IDMovConsegna
				 FROM T_MovConsegne
				 WHERE 
					CodUA=''' + @sCodUA + ''' AND 
					CodTipoConsegna=''' + @sCodTipoConsegna + ''' AND
					CodStatoConsegna =''IC'''

	IF @uIDMovConsegnaDaIgnorare IS NOT NULL
	BEGIN
		SET @sSQL =	@sSQL +' AND ID <> ''' + CONVERT(VARCHAR(50),@uIDMovConsegnaDaIgnorare) + ''''
	END

	SET @sSQL =	@sSQL +' ORDER BY DataEvento DESC'

	PRINT @sSQL
	EXEC (@sSQL)

	RETURN 0	 	
			
END