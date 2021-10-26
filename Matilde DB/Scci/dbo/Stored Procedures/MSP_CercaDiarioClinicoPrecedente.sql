CREATE PROCEDURE [dbo].[MSP_CercaDiarioClinicoPrecedente](@xParametri XML)
AS
BEGIN

	
	

	
							
		DECLARE @sCodTipoVoceDiario  AS VARCHAR(20)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER		


		DECLARE @uIDMovDiarioClinico AS UNIQUEIDENTIFIER
	
	
		SET @sCodTipoVoceDiario=(SELECT TOP 1 ValoreParametro.CodTipoVoceDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoVoceDiario') as ValoreParametro(CodTipoVoceDiario))		
	
		SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	  
	
	
				
				SET @uIDMovDiarioClinico=
		 (SELECT TOP 1 ID
			 FROM T_MovDiarioClinico 
			 WHERE 
				CodStatoDiario='VA' AND											CodTipoVoceDiario=@sCodTipoVoceDiario AND						IDTrasferimento IN (SELECT ID 
									FROM T_MovTrasferimenti
									WHERE IDCartella=@uIDCartella)
			 ORDER BY
				DataInserimento DESC						
			)							
	
		SELECT @uIDMovDiarioClinico AS IDMovDiarioClinico
	
	RETURN 0	 	
			
END