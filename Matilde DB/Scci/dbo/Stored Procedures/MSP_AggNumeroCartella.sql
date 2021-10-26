CREATE PROCEDURE [dbo].[MSP_AggNumeroCartella](@xParametri XML)
AS
BEGIN
		
		DECLARE @sCodUA AS VARCHAR(20)
	
	DECLARE @nUltimoNumeroCartella AS INTEGER
	DECLARE @nAggiorna AS INTEGER
	
		DECLARE @sCodUARif AS VARCHAR(20)				
		
	
	
		
	SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	

					  
					
	IF @sCodUA<> ''
	BEGIN
				SET @sCodUARif=(SELECT ISNULL(CodUANumerazioneCartella,@sCodUA)
						FROM T_UnitaAtomiche
						WHERE Codice=@sCodUA
					)
						
						
				SET @nUltimoNumeroCartella=
					(SELECT TOP 1 ISNULL(UltimoNumeroCartella,0)
					 FROM T_UnitaAtomiche		
					 WHERE			
												Codice = @sCodUARif
					 )	
		SET @nUltimoNumeroCartella=ISNULL(@nUltimoNumeroCartella,0)
				
		
		SET @nAggiorna=(SELECT COUNT(*) AS QTA 
						FROM T_MovTrasferimenti
						WHERE
														CodUA IN (SELECT Codice FROM 
									 T_UnitaAtomiche
									 WHERE (Codice IN (@sCodUA,@sCodUARif)
											OR 
											ISNULL(CodUANumerazioneCartella,'')=@sCodUARif
										   )
									  )	
							AND
							IDCartella IN (SELECT ID 
										   FROM 
											T_MovCartelle
										   WHERE 
										   NumeroCartella 
												= CONVERT(VARCHAR(20),
														 @nUltimoNumeroCartella+1
														 ) + '/' + CONVERT(VARCHAR(4),YEAR(GETDATE()))
														 
																																											   )
						)				   			 	
		
		SET @nAggiorna=ISNULL(@nAggiorna,0)
		
		IF @nAggiorna > 0
		BEGIN
			UPDATE	T_UnitaAtomiche		
				SET UltimoNumeroCartella=ISNULL(UltimoNumeroCartella,0)+1		
			WHERE			
								Codice = @sCodUARif
		END				
	END			
END