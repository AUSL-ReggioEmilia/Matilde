CREATE PROCEDURE [dbo].[MSP_EsisteTraferimentoInteraziendale]
	(	
		@xParametri XML
	)
AS
BEGIN
	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER		
	DECLARE @sCodAzi AS VARCHAR(20)
	DECLARE @sNumeroNosologico AS VARCHAR(20)

	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @bRet AS BIT
	DECLARE @nQta AS INTEGER

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
	
		SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))
	
			
		IF @uIDEpisodio IS NOT NULL
	BEGIN
		SET @nQta=(SELECT COUNT(*) 
			   FROM 
					T_MovTrasferimenti T
						INNER JOIN T_MovEpisodi E
							ON T.IDEpisodio=E.ID
						INNER JOIN T_UnitaOperative O
							ON (T.CodUO = O.Codice AND
								E.CodAzi= O.CodAzi)
			   WHERE 
					E.ID=@uIDEpisodio AND
					ISNULL(O.Interaziendale,0) = 1
			   )
	END
	ELSE
		BEGIN
						SET @nQta=(SELECT COUNT(*) 
			   FROM 
					T_MovTrasferimenti T
						INNER JOIN T_MovEpisodi E
							ON T.IDEpisodio=E.ID
						INNER JOIN T_UnitaOperative O
							ON (T.CodUO = O.Codice AND
								E.CodAzi= O.CodAzi)
			   WHERE 
					E.CodAzi=ISNULL(@sCodAzi,E.CodAzi) AND
					E.NumeroNosologico = @sNumeroNosologico AND
					ISNULL(O.Interaziendale,0) = 1
			   )
		END

	IF @nQta > 0
		SET @bRet=1
	ELSE
		SET @bRet=0

	SELECT @bRet AS Esiste
END