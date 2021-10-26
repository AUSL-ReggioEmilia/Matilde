

CREATE PROCEDURE [dbo].[MSP_SelMovOrdini_Trasfusionale] 
	 @idPaziente uniqueidentifier		,@filtroDati int = 0																																							AS
BEGIN
			SET NOCOUNT ON;

	IF (@filtroDati = 0) 		BEGIN
			SELECT 
				*
			FROM
				T_MovOrdini WITH (NOLOCK)
			WHERE
				IDPaziente = @idPaziente
				AND	(Eroganti LIKE '%Sistema Informativo Trasfusionale%' OR Eroganti LIKE '%SIT%')
				AND Prestazioni NOT LIKE '%GT_INIZIO%'
				AND Prestazioni NOT LIKE '%GT_FINE%'
				AND Prestazioni NOT LIKE '%GT_RESO%'
			ORDER BY 
				ISNULL(DataInoltro, DataProgrammazioneOE) DESC
		END
	ELSE IF (@filtroDati = 1) 		BEGIN
			SELECT 				
				*
			FROM
				T_MovOrdini WITH (NOLOCK)
			WHERE
				IDPaziente = @idPaziente
				AND	(Eroganti LIKE '%Sistema Informativo Trasfusionale%' OR Eroganti LIKE '%SIT%')
				AND Prestazioni LIKE '%GT_INIZIO%'
			ORDER BY 
				ISNULL(DataInoltro, DataProgrammazioneOE) DESC
		END
	ELSE IF (@filtroDati = 2) 		BEGIN
			SELECT 				
				*
			FROM
				T_MovOrdini WITH (NOLOCK)
			WHERE
				IDPaziente = @idPaziente
				AND	(Eroganti LIKE '%Sistema Informativo Trasfusionale%' OR Eroganti LIKE '%SIT%')
				AND Prestazioni LIKE '%GT_FINE%'
			ORDER BY 
				ISNULL(DataInoltro, DataProgrammazioneOE) DESC
		END
	ELSE IF (@filtroDati = 3) 		BEGIN
			SELECT 
				*
			FROM
				T_MovOrdini WITH (NOLOCK)
			WHERE
				IDPaziente = @idPaziente
				AND	(Eroganti LIKE '%Sistema Informativo Trasfusionale%' OR Eroganti LIKE '%SIT%')
				AND Prestazioni LIKE '%GT_RESO%'
			ORDER BY 
				ISNULL(DataInoltro, DataProgrammazioneOE) DESC
		END

END