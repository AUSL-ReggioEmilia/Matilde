CREATE PROCEDURE [dbo].[MSP_SelNumeroCartella](@xParametri XML)
AS
BEGIN
		
		DECLARE @sCodUA AS VARCHAR(20)
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
		
	DECLARE @sOpzione AS VARCHAR(50)			DECLARE @sNumeroCartellaUsato AS VARCHAR(50)		
	DECLARE @bNumeroGiaUsato AS BIT	
	DECLARE @sCodUARif AS VARCHAR(20)				
				
	SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					  			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)						  
					  
					
		SET @sCodUARif=(SELECT 
					CASE 
						WHEN 
							ISNULL(CodUANumerazioneCartella,'') ='' THEN @sCodUA
							ELSE CodUANumerazioneCartella
					END 	
						FROM T_UnitaAtomiche
						WHERE Codice=@sCodUA
					)

									
	SET @sOpzione = (SELECT TOP 1 ISNULL(Valore,'0') AS Valore FROM T_Config WHERE ID = 39)
	
		
		IF @sOpzione = '1'
				BEGIN			
			
						SET @sNumeroCartellaUsato =
							(SELECT TOP 1 
							CR.NumeroCartella 
							 FROM T_MovTrasferimenti T
									INNER JOIN T_MovCartelle CR
										ON T.IDCartella=CR.ID
									INNER JOIN T_UnitaAtomiche UAT
										ON T.CodUA=UAT.Codice	
							 WHERE 
								 T.IDEpisodio= @uIDEpisodio AND
								 T.ID <> @uIDTrasferimento AND
								 (T.CodUA=@sCodUARif OR
								  ISNULL(UAT.CodUANumerazioneCartella,'@')=@sCodUARif
								 )
								 AND												 CR.CodStatoCartella='AP'
					 )			
			
			SET @sNumeroCartellaUsato=ISNULL(@sNumeroCartellaUsato,'')
			IF 	@sNumeroCartellaUsato <> ''
				SET @bNumeroGiaUsato = 1						ELSE
				SET @bNumeroGiaUsato = 0								
		END
	ELSE
				SET @bNumeroGiaUsato = 0				
	IF 	@bNumeroGiaUsato = 1
				SELECT @sNumeroCartellaUsato As NumeroCartella, @bNumeroGiaUsato AS Usato	
	ELSE
				SELECT	
				ISNULL(UltimoNumeroCartella,0) + 1 As NumeroCartella, @bNumeroGiaUsato AS Usato
		FROM T_UnitaAtomiche
		WHERE			
				Codice = @sCodUARif					END