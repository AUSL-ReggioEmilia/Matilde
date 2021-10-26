CREATE PROCEDURE [dbo].[MSP_AperturaAnno]
AS
BEGIN


DECLARE @sSQL AS VARCHAR(MAX)
DECLARE @sTmp AS VARCHAR(MAX)
DECLARE @nAnno AS INTEGER
DECLARE @nCountAnnoElaborato AS INTEGER

			
		SET @nAnno = DATEPART(Year,GETDATE())

		SET @nCountAnnoElaborato=(SELECT COUNT(*) FROM T_MovAperturaAnno WHERE Codice=@nAnno)

		IF @nCountAnnoElaborato=0
	BEGIN	 
		BEGIN TRY
			BEGIN TRANSACTION
				
								SET @sTmp= REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(20),GETDATE(),120),'-',''),':',''),' ','_')

				SET  @sSQL =' SELECT * INTO XXX_' + @sTmp + '_T_UnitaAtomiche '
				SET  @sSQL = @sSQL + ' FROM T_UnitaAtomiche'

								EXEC (@sSQL)
	
								UPDATE 
					T_UnitaAtomiche
				SET 
					UltimoNumeroCartella=0
				WHERE 
					ISNULL(UltimoNumeroCartella,0) <> 0

								INSERT INTO T_MovAperturaAnno(Codice,Descrizione,DataElaborazione)
				VALUES (CONVERT(VARCHAR(4),@nAnno), 'Anno ' + CONVERT(VARCHAR(10),@nAnno), GETDATE())				
				
							COMMIT TRAN
			PRINT 'Elaborato'
			
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				BEGIN
					ROLLBACK
					PRINT 'Rollback'
					RAISERROR ('Errore elaborazione anno',16,1)
				END
		END CATCH

	END
	ELSE
	BEGIN
		PRINT 'Nessuna elaborazione'
	END

END