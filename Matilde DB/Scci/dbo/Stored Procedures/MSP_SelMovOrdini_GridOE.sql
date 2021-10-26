
CREATE PROCEDURE [dbo].[MSP_SelMovOrdini_GridOE]
	 @codUtenteIns	varchar(255)		,@idOrdineEsc	varchar(50)			,@idCampoOE		varchar(255)	AS
BEGIN

	SET NOCOUNT ON;

	declare @idCampoOE_Old varchar(255) 
	SELECT @idCampoOE_Old = SUBSTRING(@idCampoOE, 3, LEN(@idCampoOE) -2 );  

	SELECT
		 @idCampoOE as IDCampoOEFiltrato
		,MO.ID
		,MO.IDOrdineOE 
		,MO.DataProgrammazioneOE
		,MO.Eroganti
		,MO.Prestazioni
		,MO.NumeroOrdineOE
		,EPI.NumeroNosologico
		,PAZ.Nome
		,PAZ.Cognome
		,PAZ.Sesso
		,PAZ.DataNascita
		,CONVERT(int, DATEDIFF(hour,PAZ.DataNascita,GETDATE())/8766.0 ) AS Eta
		,MO.StrutturaDatiAccessori
		,MO.DatiDatiAccessori
		,MO.LayoutDatiAccessori
		,MO.DataUltimaModifica as DataUltimaModifica
	FROM
		T_MovOrdini MO WITH (NOLOCK)
		INNER JOIN 
			T_MovEpisodi EPI WITH (NOLOCK)
			ON MO.IDEpisodio = EPI.ID
		INNER JOIN
			T_Pazienti PAZ WITH (NOLOCK)
			ON MO.IDPaziente = PAZ.ID
	WHERE 
			MO.CodUtenteInserimento = @codUtenteIns
		AND MO.IDOrdineOE <> @idOrdineEsc					AND MO.CodStatoOrdine <> 'CA'																			AND ( 
				(convert(varchar(max), StrutturaDatiAccessori) LIKE '%<ID>' + @idCampoOE + '</ID>%') OR 
				(convert(varchar(max), StrutturaDatiAccessori) LIKE '%<ID>' + @idCampoOE_Old + '</ID>%')
			)   
			
		AND MO.StrutturaDatiAccessori IS NOT NULL
		AND MO.LayoutDatiAccessori IS NOT NULL
		AND MO.DatiDatiAccessori IS NOT NULL
	ORDER BY
		MO.IDNUM DESC


END