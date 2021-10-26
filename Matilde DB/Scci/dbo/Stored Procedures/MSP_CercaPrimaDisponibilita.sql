CREATE PROCEDURE [dbo].[MSP_CercaPrimaDisponibilita](@xParametri XML)
AS
BEGIN
	
				
			

		
	
			
	
	
			
	DECLARE @sDataTmp AS VARCHAR(20)
	
	DECLARE @sCodAgenda varchar(20)
	DECLARE @dDataInizio datetime
	DECLARE @dDataFine datetime
	DECLARE @nDimensioneSlotMinuti int
	

	SET @sCodAgenda = ''
	
		IF @xParametri.exist('/Parametri/CodAgenda')=1
		BEGIN			
			SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))					  

		END	


		
	IF @xParametri.exist('/Parametri/DataInizio')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInizio =NULL			
				END

		END	

			
		IF @xParametri.exist('/Parametri/DataFine')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataFine =NULL			
				END

		END	

		SET @nDimensioneSlotMinuti = (SELECT	TOP 1 ValoreParametro.DimensioneSlotMinuti.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/DimensioneSlotMinuti') as ValoreParametro(DimensioneSlotMinuti))	
	SET @nDimensioneSlotMinuti=ISNULL(@nDimensioneSlotMinuti,0)
	

	
	DECLARE @nGiorni int	
	SET @nGiorni = DATEDIFF(DAY, @dDataInizio, @dDataFine)
	
	
	
		CREATE TABLE #Tabella
	(id int IDENTITY(1,1), 
	Data datetime, 
	dayw int, 
	daywabs int,  
	minutiagenda int, 
	minutioccupati int DEFAULT 0, 
	NumAppPren int DEFAULT(0), 
	MinutiDisp int,
	orainizio datetime,
	orafine datetime)
	
	DECLARE @n int
	SET @n = 0
	
	WHILE @n <= @nGiorni
	BEGIN
	
		INSERT INTO #Tabella
		(data)
		VALUES
		(DATEADD(day,@n, @dDataInizio))

		SET @n = @n + 1
	END		
	
	
		UPDATE #Tabella
	SET dayw = DATEPART(weekday, data)


		UPDATE #Tabella
	SET daywabs = CASE 
						WHEN (((dayw + @@DATEFIRST) % 7) - 1) > 0 THEN ((dayw + @@DATEFIRST) % 7) - 1
						ELSE
							(((dayw + @@DATEFIRST) % 7) - 1) + 7
					END		
	

		UPDATE #Tabella
	SET daywabs = 0 WHERE daywabs = 7
	
					
		DELETE FROM #Tabella
	WHERE
	EXISTS 
	(SELECT F.Data FROM T_Festivita F 
	 WHERE 
		F.Data = #Tabella.data AND
		F.Data IN (SELECT Data FROM T_FestivitaAgende WHERE CodAgenda = @sCodAgenda)			 )
	
		DELETE FROM #Tabella
	WHERE
	EXISTS 
	(SELECT F.Data FROM T_Festivita F 
	 WHERE 
		F.Data = #Tabella.data AND
		F.Data NOT IN (SELECT Data FROM T_FestivitaAgende)			 )
	
		CREATE TABLE #OrarioInizio
	(id INT IDENTITY(0, 1), stringorainizio varchar(50), orainizio datetime)
	
	INSERT INTO #OrarioInizio
	(stringorainizio)
	SELECT  p.value('.', 'varchar(50)') AS OrarioInizio
	FROM T_Agende WITH(NOLOCK)
	CROSS APPLY OrariLavoro.nodes('WorkingHourTime/HourI/string') t(p)
	where Codice = @sCodAgenda
	
	
		UPDATE #OrarioInizio SET orainizio = CONVERT(datetime, stringorainizio, 108)
	
	
	 	CREATE TABLE #OrarioFine
	(id INT IDENTITY(0, 1), stringorafine varchar(50), orafine datetime)
	
	INSERT INTO #OrarioFine
	(stringorafine)
	SELECT  p.value('.', 'varchar(50)') AS OrarioFine
	FROM T_Agende WITH(NOLOCK)
	CROSS APPLY OrariLavoro.nodes('WorkingHourTime/HourF/string') t(p)
	where Codice = @sCodAgenda
	
		UPDATE #OrarioFine SET orafine = CONVERT(datetime, stringorafine, 108)



		 	CREATE TABLE #MinutiXML
	(id INT IDENTITY(0, 1), minuti int)
	
	INSERT INTO #MinutiXML
	(minuti)
	SELECT  p.value('.', 'int') AS Minuti
	FROM T_Agende WITH(NOLOCK)
	CROSS APPLY OrariLavoro.nodes('WorkingHourTime/WorkingMinutes/int') t(p)
	where Codice = @sCodAgenda
	
	
		 	CREATE TABLE #Minuti
	(id INT, orainizio datetime, orafine datetime, minuti int)
	
	INSERT INTO #Minuti
	(id, orainizio, orafine, minuti)
	SELECT
		iniz.id,
		iniz.orainizio,
		fine.orafine,
		CASE
			WHEN m.minuti is NULL THEN DATEDIFF(mi, iniz.orainizio, fine.orafine)
			ELSE m.minuti
		end as minuti
	from
	#OrarioInizio iniz
	inner join #OrarioFine fine on fine.id = iniz.id
	left outer join #MinutiXML m on m.id = iniz.id
	


			UPDATE #Tabella
	SET
	minutiagenda = m.minuti,
	orainizio = m.orainizio,
	orafine = m.orafine
	FROM
	#Tabella d
	INNER JOIN #Minuti m ON m.id = d.daywabs
	



	CREATE TABLE #MinutiPeriodi
	(id INT IDENTITY(1, 1), DataInizio datetime, DataFine datetime, GiornoSettXML int, stringOraInizio varchar(10), stringOraFine varchar(10), MinutiXML int,
	 GiornoSett int, OraInizio datetime, OraFine datetime, MinutiCalc int, Minuti int)

	INSERT INTO #MinutiPeriodi
	(DataInizio, DataFine, GiornoSettXML, stringOraInizio, stringOraFine, MinutiXML)
	SELECT A.DataInizio, A.DataFine,
	AR.WHDays.value('(DaysOfWeek)[1]', 'int') as GiornoSett,
	AR.WHDays.value('(HourI)[1]', 'varchar(10)') as oraInizio,
	AR.WHDays.value('(HourF)[1]', 'varchar(10)') as oraFine,
	AR.WHDays.value('(WorkingMinutes)[1]', 'int') as MinutiTab
	FROM T_AgendePeriodi A
	CROSS APPLY OrariLavoro.nodes('/ArrayOfWorkingHourTimeDaysOfWeek/WorkingHourTimeDaysOfWeek') as AR(WHDays)	
	WHERE
	CodAgenda=@sCodAgenda AND
	(
	(@dDataInizio BETWEEN DataInizio AND DataFine ) OR
	(@dDataFine BETWEEN DataInizio AND DataFine ) OR
	(@dDataInizio < DataInizio AND @dDataFine > DataFine)
	)

														
			UPDATE #MinutiPeriodi
	SET GiornoSett = GiornoSettXML,
		OraInizio = CONVERT(datetime, stringOraInizio, 108),
		OraFine = CONVERT(datetime, stringorafine, 108)

	UPDATE #MinutiPeriodi
	SET MinutiCalc = DATEDIFF(mi, OraInizio, OraFine)
	
	UPDATE #MinutiPeriodi
	SET Minuti = CASE
					WHEN MinutiXML IS NULL THEN MinutiCalc					
					ELSE MinutiXML
				END

	CREATE TABLE #MinutiPeriodiTot
	(DataInizio datetime, DataFine datetime, GGSett int, Minuti int)
	
	
	INSERT INTO #MinutiPeriodiTot
	SELECT DataInizio, DataFine, ID As GGSett, 0
	FROM
	(
	SELECT DataInizio, DataFine
	FROM #MinutiPeriodi
	GROUP BY DataInizio, DataFine
	) X
	CROSS JOIN (SELECT 1 AS ID UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 0) AS GG
	
	UPDATE #MinutiPeriodiTot
	SET Minuti = ISNULL((SELECT SUM(Minuti) FROM #MinutiPeriodi P WHERE P.DataInizio = #MinutiPeriodiTot.DataInizio AND 
												P.DataFine = #MinutiPeriodiTot.DataFine AND P.GiornoSett = #MinutiPeriodiTot.GGSett), 0)
	
	
	UPDATE #Tabella
	SET minutiagenda = PT.Minuti
	FROM
	#Tabella T
	INNER JOIN #MinutiPeriodiTot PT ON PT.GGSett = T.daywabs AND (T.Data BETWEEN PT.DataInizio AND PT.DataFine)
	
	






	
	
	CREATE TABLE #OrariAppuntamenti
	(id int IDENTITY(1,1), DataRifInizio datetime, DataRifFine datetime, OraInizio datetime, OraFineOrig datetime, OraFineAgg datetime, MinutiOccupati int)
	
	
				INSERT INTO #OrariAppuntamenti
	(DataRifInizio, DataRifFine, OraInizio, OraFineOrig, OraFineAgg)
	SELECT 
		CONVERT(date, datainizio) as DataRifInizio, 
		CONVERT(date, datafine) as DataRifFine,
		CONVERT(time, A.DataInizio) AS OraInizio, 
		CONVERT(time, A.DataFine) AS OraFineOrig,
		CONVERT(time, A.DataFine) AS OraFineAgg
	FROM 
	T_MovAppuntamenti A WITH(NOLOCK)
	INNER JOIN T_MovAppuntamentiAgende AA WITH(NOLOCK) ON AA.IDAppuntamento = A.ID
	WHERE
	A.CodStatoAppuntamento NOT IN ('AN', 'CA', 'SS', 'TR') AND
	AA.CodStatoAppuntamentoAgenda NOT IN ('AN', 'CA', 'SS', 'TR') AND
	AA.CodAgenda = @sCodAgenda AND
	A.DataInizio >= @dDataInizio AND
	A.DataInizio < DATEADD(day, 1, @dDataFine)
	
	
			UPDATE #OrariAppuntamenti
	SET OraFineAgg = CONVERT(datetime, '23:59', 108)
	WHERE DataRifInizio <> DataRifFine	
	
		UPDATE #OrariAppuntamenti
	SET MinutiOccupati = DATEDIFF(mi, OraInizio, OraFineAgg)
	



	CREATE TABLE #OrariNote
	(id int IDENTITY(1,1), 
	DataRifInizio datetime, 
	DataRifFine datetime, 
	OraInizio datetime, 
	OraFineOrig datetime, 
	OraFineAgg datetime, 
	OraInizioRettificata datetime,
	OraFineRettificata datetime,
	MinutiOccupati int)
	
	
		
	INSERT INTO #OrariNote
	(DataRifInizio, DataRifFine, OraInizio, OraFineOrig, OraFineAgg)
	SELECT 
		CONVERT(date, datainizio) as DataRifInizio, 
		CONVERT(date, datafine) as DataRifFine,
		CONVERT(time, A.DataInizio) AS OraInizio, 
		CONVERT(time, A.DataFine) AS OraFineOrig,
		CONVERT(time, A.DataFine) AS OraFineAgg
	FROM 
	T_MovNoteAgende A WITH(NOLOCK)
	WHERE
		A.CodAgenda = @sCodAgenda AND
	A.DataInizio >= @dDataInizio AND
	A.DataInizio < DATEADD(day, 1, @dDataFine) AND
	ISNULL(A.CodStatoNota , '') <> 'CA' AND
	ISNULL(A.EscludiDisponibilita,0) = 0			
				
			UPDATE #OrariNote
	SET OraFineAgg = CONVERT(datetime, '23:59', 108)
	WHERE DataRifInizio <> DataRifFine	
	
	
		
	
	CREATE TABLE #OrariNoteValide
	(id int IDENTITY(1,1), 
	DataRifInizio datetime, 
	DataRifFine datetime, 
	OraInizio datetime, 
	OraFineOrig datetime, 
	OraFineAgg datetime, 
	OraInizioRettificata datetime,
	OraFineRettificata datetime,
	MinutiOccupati int)



	INSERT INTO #OrariNoteValide
	(
	DataRifInizio,
	DataRifFine, 
	OraInizio, 
	OraFineOrig, 
	OraFineAgg,
	OraInizioRettificata,
	OraFineRettificata
	)
	SELECT 
	N.DataRifInizio,
	N.DataRifFine, 
	N.OraInizio, 
	N.OraFineOrig, 
	N.OraFineAgg,
	CASE
		WHEN N.OraInizio < T.orainizio THEN T.orainizio
		ELSE N.orainizio
	end as OraInizioRettificata,
	CASE
		WHEN N.OraFineAgg > T.orafine THEN T.orafine
		ELSE N.OraFineAgg
	END as OraFineRettificata
	FROM
	#OrariNote N
	INNER JOIN #Tabella T ON T.Data = N.DataRifInizio
	WHERE
	(N.OraInizio >= t.orainizio AND N.OraInizio <=T.orafine) OR
	(N.OraFineAgg >= t.orainizio AND N.OraFineAgg <= t.orafine) OR
	(N.OraInizio <= t.orainizio AND N.OraFineAgg >= t.orafine)
	
	
		

		UPDATE #OrariNoteValide
	SET MinutiOccupati = DATEDIFF(mi, OraInizioRettificata, OraFineRettificata)





	CREATE TABLE #MinutiOccupati
	(id int IDENTITY(1,1), DataRif datetime, NumAppPren int, MinutiOccupati int)
	
			INSERT INTO #MinutiOccupati
	(DataRif, NumAppPren, MinutiOccupati)
	SELECT 
	DataRifInizio, COUNT(*) as NumAppPren, SUM(ISNULL(MinutiOccupati, 0)) as MinutiOccupati
	FROM #OrariAppuntamenti
	GROUP BY DataRifInizio
	
	
	
	CREATE TABLE #MinutiOccupatiNote
	(id int IDENTITY(1,1), DataRif datetime, NumNotePren int, MinutiOccupati int)
	
			INSERT INTO #MinutiOccupatiNote
	(DataRif, NumNotePren, MinutiOccupati)
	SELECT 
	DataRifInizio, COUNT(*) as NumNotePren, SUM(ISNULL(MinutiOccupati, 0)) as MinutiOccupati
	FROM #OrariNoteValide
	GROUP BY DataRifInizio	
	
	
		UPDATE #Tabella
	SET
	NumAppPren = M.NumAppPren,
	minutioccupati = M.MinutiOccupati
	FROM
	#Tabella T1 
	INNER JOIN #MinutiOccupati M ON M.DataRif = T1.data



		

	
	UPDATE #Tabella
	SET
	minutioccupati = T1.minutioccupati + M.MinutiOccupati
	FROM
	#Tabella T1 
	INNER JOIN #MinutiOccupatiNote M ON M.DataRif = T1.data
	
	
	


	
		UPDATE #Tabella
	SET
	MinutiDisp = minutiagenda - minutioccupati
	
		
		SELECT Data, NumAppPren, MinutiDisp 
	FROM #Tabella
	WHERE
	MinutiDisp >= @nDimensioneSlotMinuti
	order by data


		drop table #Tabella
	drop table #OrarioInizio
	drop table #OrarioFine
	drop table #MinutiXML
	drop table #Minuti
	drop table #OrariAppuntamenti
	drop table #OrariNote
	drop table #MinutiOccupati
	drop table #MinutiOccupatiNote
	drop table #OrariNoteValide
	drop table #MinutiPeriodi
		
	RETURN 0
	
END