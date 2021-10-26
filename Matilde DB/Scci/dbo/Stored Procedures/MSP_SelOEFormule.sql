CREATE PROCEDURE [dbo].[MSP_SelOEFormule](@xParametri XML)
  AS
  BEGIN
	
	
				DECLARE @sJolly AS VARCHAR(1)
	SET @sJolly = '*'


				
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sPrestazione AS VARCHAR(1000)
	
		SET @sCodUA = (SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	IF ISNULL(@sCodUA,'') = '' SET @sCodUA = ''	
	
	
				DECLARE @xTmp AS XML

	CREATE TABLE #tmpUAPadri
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS,
		Livello INTEGER
	)
	
	CREATE INDEX IX_CodUA ON #tmpUAPadri (CodUA) 
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
	INSERT #tmpUAPadri EXEC MSP_SelUAPadriLivello @xTmp	
	
								   	 				 

				CREATE TABLE #tmpPrestazioni
	(
		CodAzienda VARCHAR(500) COLLATE Latin1_General_CI_AS,
		CodErogante VARCHAR(500) COLLATE Latin1_General_CI_AS,
		CodPrestazione VARCHAR(500) COLLATE Latin1_General_CI_AS
	)

	INSERT INTO #tmpPrestazioni
	(CodAzienda, CodErogante, CodPrestazione)
	SELECT        
			Tbl.Col.value('(codazienda)[1]', 'varchar(500)') as CodAzienda,
			Tbl.Col.value('(coderogante)[1]', 'varchar(500)') as CodErogante,
			Tbl.Col.value('(codprestazione)[1]', 'varchar(500)') as CodPrestazione
	FROM   @xParametri.nodes('/Parametri/Prestazione') Tbl(Col)

	
			
	BEGIN

								CREATE TABLE #tmpFormule
		(id int identity(1,1),
		 codua varchar(20),
		 codazienda varchar(20),
		 coderogante varchar(100),
		 codprestazione varchar(100),
		 coddatoaccessorio VARCHAR(100),
		 formula varchar(4000),
		 njolly int,
		 livelloUA int,
		 pesocalcolato int
		 )

		 		INSERT INTO #tmpFormule
		(codua,codazienda,coderogante,codprestazione,coddatoaccessorio,formula, njolly, livelloUA)
		SELECT
		F.CodUA,F.CodAzienda,F.CodErogante,F.CodPrestazione,F.CodDatoAccessorio, F.Formula, 0, T.Livello
		FROM
		T_OEFormule F
		INNER JOIN #tmpUAPadri T ON T.CodUA=F.CodUA
		INNER JOIN #tmpPrestazioni P ON P.CodAzienda=F.CodAzienda AND P.CodErogante=F.CodErogante AND P.CodPrestazione=F.CodPrestazione


				INSERT INTO #tmpFormule
		(codua,codazienda,coderogante,codprestazione,coddatoaccessorio,formula, njolly, livelloUA)
		SELECT
		F.CodUA,F.CodAzienda,F.CodErogante,F.CodPrestazione,F.CodDatoAccessorio, F.Formula, 1, T.Livello
		FROM
		T_OEFormule F
		INNER JOIN #tmpUAPadri T ON T.CodUA=F.CodUA
		INNER JOIN #tmpPrestazioni P ON P.CodAzienda=F.CodAzienda AND P.CodErogante=F.CodErogante
		WHERE
		F.CodPrestazione=@sJolly



				INSERT INTO #tmpFormule
		(codua,codazienda,coderogante,codprestazione,coddatoaccessorio,formula, njolly, livelloUA)
		SELECT
		F.CodUA,F.CodAzienda,F.CodErogante,F.CodPrestazione,F.CodDatoAccessorio, F.Formula, 2, T.Livello
		FROM
		T_OEFormule F
		INNER JOIN #tmpUAPadri T ON T.CodUA=F.CodUA
		INNER JOIN #tmpPrestazioni P ON P.CodAzienda=F.CodAzienda
		WHERE
		F.CodPrestazione=@sJolly AND
		F.CodErogante=@sJolly



				INSERT INTO #tmpFormule
		(codua,codazienda,coderogante,codprestazione,coddatoaccessorio,formula, njolly, livelloUA)
		SELECT
		F.CodUA,F.CodAzienda,F.CodErogante,F.CodPrestazione,F.CodDatoAccessorio, F.Formula, 3, T.Livello
		FROM
		T_OEFormule F
		INNER JOIN #tmpUAPadri T ON T.CodUA=F.CodUA
		WHERE
		F.CodPrestazione=@sJolly AND
		F.CodErogante=@sJolly AND
		F.CodAzienda=@sJolly


		UPDATE #tmpFormule set pesocalcolato=livelloUA * 10 + njolly

		
		SELECT coddatoaccessorio, MIN(pesocalcolato) as minpesocalcolato into #tmpDatiAccessori
		from #tmpFormule
		group by coddatoaccessorio

						
		SELECT da.coddatoaccessorio, MIN(t.formula)
		FROM #tmpDatiAccessori da
		INNER JOIN #tmpFormule t on t.coddatoaccessorio=da.coddatoaccessorio and t.pesocalcolato=minpesocalcolato
		GROUP BY da.coddatoaccessorio

								DROP TABLE #tmpPrestazioni
		DROP TABLE #tmpUAPadri
		DROP TABLE #tmpFormule

	END 			
	
  END