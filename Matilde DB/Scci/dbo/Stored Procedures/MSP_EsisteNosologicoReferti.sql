CREATE PROCEDURE [dbo].[MSP_EsisteNosologicoReferti] (@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @sCodAzi AS VARCHAR(20)
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)
	DECLARE @sCodStatoEpisodio AS VARCHAR(20)

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	
	DECLARE @nRet AS INTEGER
	
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
	DECLARE @xParMovPaz  AS XML
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
	
	SET @uIDEpisodio=NULL
	SET @nRet=0
	
				
		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
		
		SET @sCodStatoEpisodio=(SELECT TOP 1 ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio))
	
		SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))
	SET @sNumeroNosologico=ISNULL(@sNumeroNosologico,'')
	
		SET @sNumeroListaAttesa = (SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa))
	SET @sNumeroListaAttesa = ISNULL(@sNumeroListaAttesa, '.') 	
		
	
															
						
			
		IF @sNumeroNosologico<>@sNumeroListaAttesa 
		BEGIN 
																																						 
																																
						IF ISNULL(@sNumeroListaAttesa,'') = '.' AND @sNumeroNosologico <> ''
				BEGIN				
					
										SET @uIDEpisodio= (SELECT TOP 1 ID 
										FROM T_MovEpisodi 
										WHERE
											NumeroNosologico = @sNumeroNosologico
											AND CodAzi = ISNULL(@sCodAzi,CodAzi)
											AND CodStatoEpisodio = ISNULL(@sCodStatoEpisodio,CodStatoEpisodio)
										)										 
				END
			ELSE
				BEGIN														
					IF 	(@sNumeroListaAttesa NOT IN ('','.'))
					BEGIN	
												SET @uIDEpisodio=
								(
								SELECT TOP 1 ID 
								FROM T_MovEpisodi 
								WHERE
									NumeroNosologico = @sNumeroNosologico AND 
									NumeroListaAttesa = @sNumeroListaAttesa AND
									CodAzi = ISNULL(@sCodAzi,CodAzi) AND
									CodStatoEpisodio = ISNULL(@sCodStatoEpisodio,CodStatoEpisodio)
								)
					END
					ELSE
					BEGIN
													SET @uIDEpisodio=
								(
								SELECT TOP 1 ID 
								FROM T_MovEpisodi 
								WHERE
									NumeroNosologico = @sNumeroNosologico AND 									
									CodAzi = ISNULL(@sCodAzi,CodAzi) AND
									CodStatoEpisodio = ISNULL(@sCodStatoEpisodio,CodStatoEpisodio)
								)					
					END

				END
					
		END
		ELSE
			BEGIN
								
																																																		
																SET @uIDEpisodio=
							(
							SELECT TOP 1 ID 
							FROM T_MovEpisodi 
							WHERE
								(NumeroNosologico = @sNumeroNosologico 
									OR
								 NumeroListaAttesa = @sNumeroListaAttesa
								)
										
								AND CodAzi = ISNULL(@sCodAzi,CodAzi)
								AND CodStatoEpisodio = ISNULL(@sCodStatoEpisodio,CodStatoEpisodio)
							)
						
			END

	IF @uIDEpisodio IS NULL
		SET @nRet=0
	ELSE
		SET @nRet=1				
	
	SELECT 	@nRet AS Esiste, 
			@uIDEpisodio AS IDEpisodio					
	RETURN 0
END