
CREATE PROCEDURE [dbo].[MSP_CreaCoda] 	
AS
BEGIN	
	
	DECLARE @newID AS UNIQUEIDENTIFIER
	DECLARE @nRecord AS INTEGER
	DECLARE @bContinua AS BIT
	
	SET @newID = NEWID()
	SET @bContinua=1
	
	CREATE TABLE #tmpID
	(		
		ID BIGINT
	)
	
	CREATE INDEX IX_ID ON #tmpID (ID)	
		
	WHILE @bContinua=1
	BEGIN	
			
			INSERT INTO #tmpID(ID)
			SELECT TOP 100
				ID 
			FROM 
				T_MovDataLog 
			WHERE 
				Transito Is Null
						
			SET @nRecord=(SELECT COUNT(*) FROM #tmpID)

			IF @@ERROR=0 
			BEGIN
				IF @nRecord > 0
					BEGIN
											
						UPDATE 
							T_MovDataLog WITH (ROWLOCK)
						SET 
							Transito = @newID,
							DataTransito = GETDATE()
						WHERE 
							ID IN 
								(SELECT ID FROM #tmpID)	
																
						INSERT INTO T_MovDataLogSistemi
							(
							CodSistema, 
							IDDataLog, 
							Data, 
							CodUtente, 
							ComputerName, 
							IpAddress, 
							CodEvento, 
							TipoOperazione, 
							Operazione,
							LogPrima, 
							LogDopo, 
							Transito, 
							DataTransito, 
							InCarico, 
							[OutPut], 
							Trasmesso, 
							Response
							)
							SELECT
								A.CodSistema, 
								M.ID, 
								M.Data, 
								M.CodUtente, 
								M.ComputerName, 
								M.IpAddress, 
								M.CodEvento, 
								M.TipoOperazione, 
								M.Operazione,
								M.LogPrima, 
								M.LogDopo, 
								M.Transito, 
								M.DataTransito, 
								0, 
								'', 
								0, 
								''
							FROM
								 T_MovDataLog M 
									Inner Join T_AssEveSis A ON M.CodEvento = A.CodEvento
							WHERE 
								ID IN 
									(SELECT ID FROM #tmpID)	AND 
								A.Attivo = 1			
						END 
					ELSE
						BEGIN													
							SET @bContinua=0
						END	
										
					TRUNCATE TABLE 	#tmpID
			END
			ELSE
				BEGIN
					SET @bContinua=0
					TRUNCATE TABLE 	#tmpID	
				END			
	END	
END