CREATE PROCEDURE [dbo].[MSP_CercaCartellaDaDWH](@xParametri AS XML)
AS
BEGIN
	

					
	DECLARE @sNumeroNosologico AS Varchar(20)
	DECLARE @sNumeroCartella AS Varchar(500)
	DECLARE @sCodUA AS Varchar(MAX)		
	
	DECLARE @nContaEpisodi AS INTEGER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoCartella AS Varchar(20)
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	
		DECLARE @sSQL AS VARCHAR(MAX)
	
			
		SET @sNumeroNosologico=(SELECT	TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))						 
	
		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))						 
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))						 
	
			
	SET @uIDCartella = NULL

	IF (@sNumeroNosologico IS NOT NULL AND @sNumeroCartella IS NOT NULL AND @sCodUA IS NOT NULL)
	BEGIN
		SET @nContaEpisodi=(SELECT COUNT(*) FROM T_MovEpisodi WHERE NumeroNosologico=@sNumeroNosologico AND CodStatoEpisodio<> 'CA')

				IF (@nContaEpisodi > 0)
		BEGIN
						IF (@nContaEpisodi = 1)
			BEGIN
								SET @uIDEpisodio = (SELECT TOP 1 ID 
									FROM T_MovEpisodi
									WHERE 
										NumeroNosologico=@sNumeroNosologico AND 
										CodStatoEpisodio<> 'CA')

								SELECT TOP 1 
					@uIDCartella = C.ID,
					@sCodStatoCartella = C.CodStatoCartella
				FROM 
											T_MovTrasferimenti T 
												INNER JOIN T_MovCartelle C
													ON T.IDCartella = C.ID
										 WHERE 
											T.IDEpisodio=@uIDEpisodio AND 											
											T.CodUA=@sCodUA AND
											C.NumeroCartella = @sNumeroCartella AND
											T.CodStatoTrasferimento <> 'CA' AND
											C.CodStatoCartella <> 'CA'				
			END
		END
	END

		SELECT 
		@uIDCartella AS IDCartella, 
		@sCodStatoCartella AS CodStatoCartella

	RETURN 0
	
END