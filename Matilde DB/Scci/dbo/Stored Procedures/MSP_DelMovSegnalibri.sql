CREATE PROCEDURE [dbo].[MSP_DelMovSegnalibri](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @uIDSegnalibro AS UNIQUEIDENTIFIER		
	
		DECLARE @sGUID AS VARCHAR(50)
	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSegnalibro.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSegnalibro') as ValoreParametro(IDSegnalibro))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDSegnalibro=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				
							
				
	IF @uIDSegnalibro IS NOT NULL
		DELETE FROM T_MovSegnalibri
		WHERE ID=@uIDSegnalibro      

															
	RETURN 0
END