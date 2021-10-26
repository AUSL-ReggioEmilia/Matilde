CREATE PROCEDURE [dbo].[MSP_ControlloPreAperturaCartella](@xParametri XML)
AS
BEGIN
		
		DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodSAC AS VARCHAR(50)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @nQTA AS INTEGER
	DECLARE @bAbilitaCollegaCartelle AS BIT	
				
	DECLARE @bEsito AS BIT	
	DECLARE @sMessaggio AS VARCHAR(2000)
	DECLARE @sTmp AS VARCHAR(MAX)
				
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	


		SET @sCodSAC=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))	
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
					  
				
	SET @nQTA=0
	SET @sTmp=''
	
		SET @bAbilitaCollegaCartelle=(SELECT TOP 1 ISNULL(AbilitaCollegaCartelle,0) FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
	
			SELECT 
			@nQTA=@nQTA +1 
			,@sTmp=@sTmp + 
					CASE	
						WHEN ISNULL(@sTmp,'')='' THEN ''
						ELSE ', '
					END + 	Q.NumeroCartella
		FROM
			(SELECT 
				T.IDCartella,MIN(NumeroCartella) AS NumeroCartella
			FROM
				T_MovPazienti P
				INNER JOIN T_MovEpisodi E
					ON P.IDEpisodio=E.ID
				INNER JOIN T_MovTrasferimenti T
					ON E.ID=T.IDEpisodio
				INNER JOIN T_MovCartelle C
					ON T.IDCartella=C.ID
			WHERE 
				C.CodStatoCartella='AP' AND									T.CodUA=@sCodUA	AND											P.IDEpisodio <> @uIDEpisodio AND							
				(				P.IDPaziente=@uIDPaziente 
				 OR
				 				 P.IDPaziente IN 
					(SELECT IDPazienteVecchio
					 FROM T_PazientiAlias
					 WHERE 
						IDPaziente IN 
							(SELECT IDPaziente
							 FROM T_PazientiAlias
							 WHERE IDPazienteVecchio=@uIDPaziente
							)
					)	
				)					
			GROUP BY IDCartella)AS Q
							
		IF @nQTA=0
			BEGIN
				SET @sMessaggio=''
				SET @bEsito=0
			END	
		ELSE
			BEGIN
				SET @sMessaggio='Esiste già una cartella aperta' + CHAR(13) + CHAR(10) + + CHAR(13) + CHAR(10)
								SET @sMessaggio=@sMessaggio + 'Numero Cartelle aperte: ' + LTRIM(@sTmp)  + CHAR(13) + CHAR(10)  + CHAR(13) + CHAR(10)
				SET @sMessaggio=@sMessaggio + 'Vuoi continuare ed aprire la cartella per l''episodio selezionato ?'
				
				SET @bEsito=1
			END	
	
	
	SELECT 
		@bEsito As Esito,
		@sMessaggio As Messaggio 
END