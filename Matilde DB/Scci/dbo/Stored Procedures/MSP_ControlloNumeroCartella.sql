CREATE PROCEDURE [dbo].[MSP_ControlloNumeroCartella](@xParametri XML)
AS
BEGIN
		
									
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @sNumeroCartella AS VARCHAR(50)	
	DECLARE @sCodUA AS VARCHAR(50)	
	DECLARE @sCodUARif AS VARCHAR(50)
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @uIDTemp AS UNIQUEIDENTIFIER		
	DECLARE @bOutput AS BIT
	DECLARE @sOpzione AS VARCHAR(50)			
				
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))						  	
					  			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
							  
		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))			
			
				
			
		SET @sCodUARif=(SELECT 
					CASE 
						WHEN 
							ISNULL(CodUANumerazioneCartella,'') ='' THEN @sCodUA
							ELSE CodUANumerazioneCartella
					END 	
						FROM T_UnitaAtomiche
						WHERE Codice=@sCodUA
					)

		
	SET @sOpzione=(SELECT TOP 1 ISNULL(Valore,'0') AS Valore FROM T_Config WHERE ID=39)
		
	IF @sOpzione='1'
				SET @uIDTemp=(SELECT TOP 1 M.ID 
						FROM T_MovTrasferimenti M
							INNER JOIN T_MovCartelle CR
								ON M.IDCartella=CR.ID
							INNER JOIN T_UnitaAtomiche UAT
								ON M.CodUA=UAT.Codice	
						WHERE CR.NumeroCartella=@sNumeroCartella						      AND (M.CodUA=@sCodUARif												   OR
							   ISNULL(UAT.CodUANumerazioneCartella,'@')=@sCodUARif
							   )
						  AND M.IDEpisodio<> @uIDEpisodio								 )
	ELSE
				SET @uIDTemp=(SELECT TOP 1 M.ID 
							FROM T_MovTrasferimenti M
							INNER JOIN T_MovCartelle CR
									ON M.IDCartella=CR.ID
							INNER JOIN T_UnitaAtomiche UAT
								ON M.CodUA=UAT.Codice		
						WHERE 
							  CR.NumeroCartella=@sNumeroCartella								  AND (M.CodUA=@sCodUARif														 OR
									ISNULL(UAT.CodUANumerazioneCartella,'@')=@sCodUARif
									 )
							  							  AND M.ID <> @uIDTrasferimento									  
						 )	 
	
	IF @uIDTemp IS NOT NULL
		SET @bOutput=1
	ELSE
		SET @bOutput=0

	SELECT @bOutput AS Usato
END